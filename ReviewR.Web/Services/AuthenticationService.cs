using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ReviewR.Web.Infrastructure;
using ReviewR.Web.Models;
using ReviewR.Web.Models.Data;
using ReviewR.Web.Services.Authenticators;
using VibrantUtils;

namespace ReviewR.Web.Services
{
    public class AuthenticationService
    {
        public ISettings Settings { get; set; }
        public IDictionary<string, Authenticator> Authenticators { get; set; }
        public IDataRepository Data { get; set; }

        protected AuthenticationService() { }

        public AuthenticationService(IEnumerable<Authenticator> authenticators, ISettings settings, IDataRepository data)
        {
            Requires.NotNull(authenticators, "authenticators");
            Requires.NotNull(settings, "settings");
            Requires.NotNull(data, "data");

            Authenticators = authenticators.ToDictionary(a => a.Id);
            Settings = settings;
            Data = data;
        }

        public virtual IDictionary<string, string> GetClientIds()
        {
            return Authenticators.Values.ToDictionary(a => a.Id, a => a.GetAppId(Settings));
        }

        public virtual Task<AuthenticationResult> AuthenticateWithProviderAsync(string type, string accessToken)
        {
            Requires.NotNullOrEmpty(type, "type");
            Requires.NotNullOrEmpty(accessToken, "accessToken");

            // Find the authenticator
            Authenticator auth;
            if (!Authenticators.TryGetValue(type, out auth))
            {
                return TaskHelpers.FromError<AuthenticationResult>(new NotSupportedException("No such authentication provider: " + type));
            }
            return ExchangeToken(accessToken, auth).Then(u =>
            {
                // Check for a credential for this user
                Credential cred = Data.Credentials
                                      .Where(c => c.Provider == auth.DisplayName && c.Identifier == u.Identifier)
                                      .FirstOrDefault();
                if (cred != null)
                {
                    // Done! Return this user
                    return AuthenticationResult.LoggedIn(cred.User);
                }

                // Ok, check for a user by email address
                IList<string> missingFields = new List<string>();
                if (!String.IsNullOrEmpty(u.Email))
                {
                    User user = Data.Users
                                    .Where(usr => usr.Email == u.Email)
                                    .FirstOrDefault();

                    if (user != null)
                    {
                        // Ok, associate a new credential and successfully log that user in
                        cred = new Credential()
                        {
                            Provider = auth.DisplayName,
                            Identifier = u.Identifier
                        };
                        user.Credentials.Add(cred);
                        Data.SaveChanges();
                        return AuthenticationResult.Associated(user);
                    }
                }
                else
                {
                    missingFields.Add("email");
                }

                // Do we have enough info to register a new user?
                if (String.IsNullOrEmpty(u.DisplayName))
                {
                    missingFields.Add("displayName");
                }

                if (missingFields.Any())
                {
                    return AuthenticationResult.MissingData(missingFields);
                }

                // Yes! Register a new user and log them in
                var newUser = new User()
                {
                    DisplayName = u.DisplayName,
                    Email = u.Email,
                    Credentials = new List<Credential>() {
                        new Credential() {
                            Provider = auth.DisplayName,
                            Identifier = u.Identifier
                        }
                    }
                };
                Data.Users.Add(newUser);
                Data.SaveChanges();
                return AuthenticationResult.Registered(newUser);
            });
        }

        protected internal virtual Task<UserInfo> ExchangeToken(string accessToken, Authenticator auth)
        {
            return auth.CompleteAuthentication(Settings, accessToken);
        }
    }
}
