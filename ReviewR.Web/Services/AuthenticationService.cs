using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ReviewR.Web.Models;
using ReviewR.Web.Models.Data;
using VibrantUtils;
using ReviewR.Web.Infrastructure;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace ReviewR.Web.Services
{
    public class AuthenticationService
    {
        public TimeSpan Timeout { get; set; }
        public IDataRepository Data { get; set; }
        public TokenService Tokens { get; set; }
        public ISettings Settings { get; set; }

        protected AuthenticationService() { }

        public AuthenticationService(IDataRepository data, TokenService tokens, ISettings settings)
        {
            Data = data;
            Tokens = tokens;
            Settings = settings;
            Timeout = TimeSpan.FromMinutes(30);
        }

        public virtual User GetUserByEmail(string email)
        {
            return Data.Users.Where(u => u.Email == email).FirstOrDefault();
        }

        public async virtual Task<UserInfo> ResolveAuthTokenAsync(string authenticationToken)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage resp = await client.GetAsync(String.Format(
                "https://rpxnow.com/api/v2/auth_info?apiKey={0}&token={1}",
                Settings.Get("Janrain:ApiKey"),
                authenticationToken), HttpCompletionOption.ResponseContentRead);

            if (!resp.IsSuccessStatusCode)
            {
                return null;
            }
            string content = await resp.Content.ReadAsStringAsync();
            var identifier = JsonConvert.DeserializeAnonymousType(content, new
            {
                profile = new
                {
                    name = new
                    {
                        formatted = ""
                    },
                    identifier = "",
                    providerName = "",
                    displayName = "",
                    verifiedEmail = ""
                }
            });
            string name = identifier.profile.displayName;
            if (identifier.profile.name != null && !String.IsNullOrEmpty(identifier.profile.name.formatted))
            {
                name = identifier.profile.name.formatted;
            }
            return new UserInfo(
                identifier.profile.providerName,
                identifier.profile.identifier,
                name,
                identifier.profile.verifiedEmail);
        }

        public virtual User Login(string provider, string identifier)
        {
            // Try to get a user for this identifier
            Credential cred = Data.Credentials
                                  .Include("User")
                                  .Where(c => c.Identifier == identifier && c.Provider == provider)
                                  .SingleOrDefault();
            if (cred == null)
            {
                return null;
            }
            return cred.User;
        }

        public virtual User Register(UserInfo identifier, string email, string displayName)
        {
            // Create the user
            User u = new User()
            {
                Email = email,
                DisplayName = displayName,
                Credentials = new List<Credential>() {
                    new Credential()
                    {
                        Provider = identifier.Provider,
                        Identifier = identifier.Value
                    }
                }
            };
            Data.Users.Add(u);
            Data.SaveChanges();
            return u;
        }

        public virtual void AddCredential(int userId, string provider, string identifier)
        {
            Data.Credentials.Add(new Credential()
            {
                UserId = userId,
                Provider = provider,
                Identifier = identifier
            });
            Data.SaveChanges();
        }
    }
}