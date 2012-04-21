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
        private const string SessionTokenPurpose = "Session";
        private const string PersistentTokenPurpose = "Persistent";

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

        public virtual User GetUserFromSessionToken(string encryptedToken)
        {
            return GetUser(Tokens.DecryptToken(encryptedToken, SessionTokenPurpose));
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
            var identifier = JsonConvert.DeserializeAnonymousType(await resp.Content.ReadAsStringAsync(), new
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

        public virtual Tuple<User, string> Login(string persistentToken)
        {
            AuthenticationToken token = null;
            try
            {
                token = Tokens.DecryptToken(persistentToken, PersistentTokenPurpose);
            }
            catch (CryptographicException)
            {
                // Bad token
                return null;
            }
            User user = GetUser(token);
            if (user == null)
            {
                return null;
            }

            // Issue a fresh session token
            string session = IssueSessionToken(user.Id);
            return Tuple.Create(user, session);
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

        public string IssueSessionToken(int userId)
        {
            AuthenticationToken sessionToken = new AuthenticationToken(Guid.NewGuid(), DateTimeOffset.UtcNow + Timeout);
            string session = Tokens.EncryptToken(sessionToken, SessionTokenPurpose);
            Data.Tokens.Add(new Token()
            {
                Id = sessionToken.TokenId,
                Persistent = false,
                UserId = userId,
                Value = Convert.ToBase64String(sessionToken.EncodeToken()),
                Expires = sessionToken.Expires
            });
            Data.SaveChanges();
            return session;
        }

        public TokenPair IssueTokenPair(int userId)
        {
            // Create a pair of tokens, one persistent, one not
            AuthenticationToken sessionToken = new AuthenticationToken(Guid.NewGuid(), DateTimeOffset.UtcNow + Timeout);
            AuthenticationToken persistentToken = new AuthenticationToken(Guid.NewGuid(), DateTimeOffset.MaxValue);
            string session = Tokens.EncryptToken(sessionToken, SessionTokenPurpose);
            string persistent = Tokens.EncryptToken(persistentToken, PersistentTokenPurpose);

            Data.Tokens.Add(new Token()
            {
                Id = sessionToken.TokenId,
                Persistent = false,
                UserId = userId,
                Value = Convert.ToBase64String(sessionToken.EncodeToken()),
                Expires = sessionToken.Expires
            });
            Data.Tokens.Add(new Token()
            {
                Id = persistentToken.TokenId,
                Persistent = true,
                UserId = userId,
                Value = Convert.ToBase64String(persistentToken.EncodeToken()),
                Expires = persistentToken.Expires
            });
            Data.SaveChanges();
            return new TokenPair(session, persistent);
        }

        private User GetUser(AuthenticationToken token)
        {
            // TODO: Don't go to the DB if expired, just add the token to a queue to be swept up in the background
            string tokenString = Convert.ToBase64String(token.EncodeToken());
            Token dbToken = Data.Tokens
                                .Include("User")
                                .Include("User.Roles")
                                .Where(t => t.Value == tokenString && t.Id == token.TokenId)
                                .SingleOrDefault();
            if (token.Expires <= DateTimeOffset.UtcNow)
            {
                Data.Tokens.Remove(dbToken);
                Data.SaveChanges();
                throw new SecurityException("Token has expired");
            }
            if (dbToken == null)
            {
                return null;
            }
            return dbToken.User;
        }
    }
}