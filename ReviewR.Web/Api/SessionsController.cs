using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ReviewR.Web.Infrastructure;
using ReviewR.Web.Models;
using ReviewR.Web.Models.Data;
using ReviewR.Web.Models.Request;
using ReviewR.Web.Services;
using System.Threading.Tasks;
using ReviewR.Web.Models.Response;
using System.Security.Cryptography;
using System.Security;

namespace ReviewR.Web.Api
{
    public class SessionsController : ReviewRApiController
    {
        public AuthenticationService Auth { get; set; }
        public TokenService Tokens { get; set; }

        public SessionsController(AuthenticationService auth, TokenService tokens)
        {
            Auth = auth;
            Tokens = tokens;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> New(string authToken)
        {
            UserInfo id = await Auth.ResolveAuthTokenAsync(authToken);

            // Try to find or create a user with this identifier
            User user = GetOrCreateUser(id);
            if (user == null)
            {
                return BadRequest();
            }

            // Issue a new token pair
            TokenPair pair = Auth.IssueTokenPair(user.Id);

            return Created(new
            {
                user = UserModel.FromUser(user),
                tokens = pair
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage Restore(string persistentToken)
        {
            // Get the user for this token
            var result = Auth.Login(persistentToken);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                user = UserModel.FromUser(result.Item1),
                token = result.Item2
            });
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