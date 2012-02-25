using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReviewR.Web.Models;
using VibrantUtils;

namespace ReviewR.Web.Services
{
    public class AuthenticationService
    {
        public IDataRepository Data { get; set; }
        public HashService Hasher { get; set; }

        public AuthenticationService(IDataRepository data, HashService hasher)
        {
            Data = data;
            Hasher = hasher;
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

        public CreateUserResult CreateUser(string email, string displayName, string password)
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
    }
}
