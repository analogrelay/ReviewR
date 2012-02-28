using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Web;
using ReviewR.Web.Models;
using VibrantUtils;

namespace ReviewR.Web.Services
{
    public class AuthenticationService
    {
        private const string SessionIdKey = "AuthenticationService.CurrentUserId";

        public IDataRepository Data { get; set; }
        public HashService Hasher { get; set; }
        public HttpContextBase Context { get; set; }

        public virtual bool IsAuthenticated
        {
            get { return Context.Request.IsAuthenticated; }
        }

        protected AuthenticationService() { }

        public AuthenticationService(IDataRepository data, HashService hasher, HttpContextBase context)
        {
            Data = data;
            Hasher = hasher;
            Context = context;
        }

        public virtual User LogIn(string email, string password)
        {
            Requires.NotNullOrEmpty(email, "email");
            Requires.NotNullOrEmpty(password, "password");

            // Get the user from the repository
            User user = Data.Users.Where(u => u.Email == email).FirstOrDefault();
            if (user == null)
            {
                // No such user!
                return null;
            }

            // Generate the hashed version of the inputted password using the user's salt
            string inputtedHash = Hasher.GenerateHash(password, user.PasswordSalt);

            // Compare it
            if (String.Equals(inputtedHash, user.Password, StringComparison.Ordinal))
            {
                return user;
            }
            else
            {
                return null;
            }
        }

        public virtual CreateUserResult CreateUser(string email, string displayName, string password)
        {
            Requires.NotNullOrEmpty(email, "email");
            Requires.NotNullOrEmpty(displayName, "displayName");
            Requires.NotNullOrEmpty(password, "password");

            // Check for duplicate email
            // TODO: Transactions?
            if (Data.Users.Where(u => u.Email == email).Any())
            {
                return CreateUserResult.EmailTaken;
            }

            string salt = Hasher.GenerateSalt();
            User user = new User()
            {
                Email = email,
                DisplayName = displayName,
                PasswordSalt = salt,
                Password = Hasher.GenerateHash(password, salt)
            };
            Data.Users.Add(user);
            Data.SaveChanges();
            return CreateUserResult.Success;
        }

        public virtual User GetUser(string email)
        {
            return Data.Users
                       .Include("Roles")
                       .Where(u => u.Email == email)
                       .Single();
        }

        public virtual User GetCurrentUser()
        {
            var tup = GetCurrentUserInfo();
            if (tup.Item2 == null)
            {
                return Data.Users.Where(u => u.Id == tup.Item1).FirstOrDefault();
            }
            else
            {
                return tup.Item2;
            }
        }

        public virtual int GetCurrentUserId()
        {
            return GetCurrentUserInfo().Item1;
        }

        private Tuple<int, User> GetCurrentUserInfo() {
            // Check Session Cookie
            string cached = Context.Session[SessionIdKey] as string;
            int id;
            if (!String.IsNullOrEmpty(cached) && Int32.TryParse(cached, out id))
            {
                return Tuple.Create(id, (User)null);
            }
            else if (Context.Request.IsAuthenticated)
            {
                // Find the user
                User user = Data.Users
                                .Where(u => u.Email == Context.User.Identity.Name)
                                .SingleOrDefault();
                if (user != null)
                {
                    id = user.Id;
                    Context.Session[SessionIdKey] = id.ToString();
                    return Tuple.Create(id, user);
                }
            }
            throw new SecurityException("This action requires a currently logged in user");
        }
    }
}
