using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ReviewR.Web.Infrastructure;
using ReviewR.Web.Models;
using ReviewR.Web.Models.Data;
using VibrantUtils;

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
            Requires.NotNull(data, "data");
            Requires.NotNull(tokens, "tokens");
            Requires.NotNull(settings, "settings");

            Data = data;
            Tokens = tokens;
            Settings = settings;
            Timeout = TimeSpan.FromMinutes(30);
        }

        public virtual User GetUserByEmail(string email)
        {
            Requires.NotNullOrEmpty(email, "email");

            return Data.Users.Where(u => u.Email == email).FirstOrDefault();
        }

        public virtual Task<UserInfo> ResolveAuthTokenAsync(string authenticationToken)
        {
            // Do arg checking before we start the async task
            Requires.NotNullOrEmpty(authenticationToken, "authenticationToken");
            return DoResolveAuthTokenAsync(authenticationToken);
        }

        public virtual User Login(string provider, string identifier)
        {
            Requires.NotNullOrEmpty(provider, "provider");
            Requires.NotNullOrEmpty(identifier, "identifier");

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

        public virtual User Register(string provider, string identifier, string email, string displayName)
        {
            Requires.NotNullOrEmpty(provider, "provider");
            Requires.NotNullOrEmpty(identifier, "identifier");
            Requires.NotNullOrEmpty(email, "email");
            Requires.NotNullOrEmpty(displayName, "displayName");

            // Create the user
            User u = new User()
            {
                Email = email,
                DisplayName = displayName,
                Credentials = new List<Credential>() {
                    new Credential()
                    {
                        Provider = provider,
                        Identifier = identifier
                    }
                }
            };
            Data.Users.Add(u);
            Data.SaveChanges();
            return u;
        }

        public virtual void AddCredential(int userId, string provider, string identifier)
        {
            Requires.InRange(userId > 0, "userId");
            Requires.NotNullOrEmpty(provider, "provider");
            Requires.NotNullOrEmpty(identifier, "identifier");

            Data.Credentials.Add(new Credential()
            {
                UserId = userId,
                Provider = provider,
                Identifier = identifier
            });
            Data.SaveChanges();
        }

        protected internal async virtual Task<HttpResponseMessage> ExchangeToken(string token)
        {
            HttpClient client = new HttpClient();
            return await client.GetAsync(String.Format(
                "https://rpxnow.com/api/v2/auth_info?apiKey={0}&token={1}",
                Settings.Get("Janrain:ApiKey"),
                token), HttpCompletionOption.ResponseContentRead);
        }

        private async Task<UserInfo> DoResolveAuthTokenAsync(string authenticationToken)
        {
            HttpResponseMessage resp = await ExchangeToken(authenticationToken);

            if (!resp.IsSuccessStatusCode)
            {
                return null;
            }
            string content = await resp.Content.ReadAsStringAsync();
            dynamic json = JObject.Parse(content);

            if (json == null || json.profile == null)
            {
                return null;
            }
            
            string name = json.profile.displayName;
            if (json.profile.name != null)
            {
                string formatted = json.profile.name.formatted;
                if (!String.IsNullOrEmpty(formatted))
                {
                    name = json.profile.name.formatted;
                }
            }
            string provider = json.profile.providerName;
            string id = json.profile.identifier;
            string verifiedEmail = json.profile.verifiedEmail;
            return new UserInfo(
                provider,
                id,
                name,
                verifiedEmail);
        }
    }
}
