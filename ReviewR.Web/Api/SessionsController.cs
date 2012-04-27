using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using ReviewR.Web.Infrastructure;
using ReviewR.Web.Models;
using ReviewR.Web.Models.Data;
using ReviewR.Web.Models.Response;
using ReviewR.Web.Services;

namespace ReviewR.Web.Api
{
    public class SessionsController : ReviewRApiController
    {
        public AuthenticationService Auth { get; set; }
        
        public SessionsController(AuthenticationService auth)
        {
            Auth = auth;
        }

        [AllowAnonymous]
        public async Task<HttpResponseMessage> Post(string authToken)
        {
            UserInfo id = await Auth.ResolveAuthTokenAsync(authToken);

            // Try to find or create a user with this identifier
            User user = GetOrCreateUser(id);
            if (user == null)
            {
                return BadRequest();
            }

            // Log in the user
            User = ReviewRPrincipal.FromUser(user);

            // Return the token and user data
            return Created(new
            {
                user = User.Identity,
                token = SessionToken
            });
        }

        public void Delete()
        {
            User = null;
        }

        private User GetOrCreateUser(UserInfo id)
        {
            User user = Auth.Login(id.Provider, id.Value);
            if (user == null)
            {
                // Can we find a user with that email?
                if (!String.IsNullOrEmpty(id.Email))
                {
                    user = Auth.GetUserByEmail(id.Email);
                    if (user != null)
                    {
                        // Associate this credential with the user
                        Auth.AddCredential(user.Id, id.Provider, id.Value);
                        return user;
                    }
                }
                else {
                    return null;
                }

                if (String.IsNullOrEmpty(id.DisplayName))
                {
                    return null;
                }

                // Nothing is missing, so we can just create the user
                user = Auth.Register(id, id.Email, id.DisplayName);
            }
            return user;
        }
    }
}