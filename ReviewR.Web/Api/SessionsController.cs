using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using ReviewR.Web.Infrastructure;
using ReviewR.Web.Models;
using ReviewR.Web.Models.Data;
using ReviewR.Web.Models.Response;
using ReviewR.Web.Services;
using VibrantUtils;

namespace ReviewR.Web.Api
{
    public class SessionsController : ReviewRApiController
    {
        public AuthenticationService Auth { get; set; }

        protected SessionsController() { }
        public SessionsController(AuthenticationService auth)
        {
            Requires.NotNull(auth, "auth");
            Auth = auth;
        }

        [AllowAnonymous]
        public Task<HttpResponseMessage> Post(string id, string token)
        {
            Requires.NotNullOrEmpty(id, "id");
            Requires.NotNullOrEmpty(token, "token");

            return Auth.AuthenticateWithProviderAsync(id, token).Then<AuthenticationResult, HttpResponseMessage>(r =>
            {
                if (r.Outcome == AuthenticationOutcome.MissingFields)
                {
                    return BadRequest(new { missingFields = r.MissingFields });
                }
                else
                {
                    User = ReviewRPrincipal.FromUser(r.User);
                    return Created(new
                    {
                        user = User.Identity,
                        token = SessionToken
                    });
                }
            });
        }

        public void Delete()
        {
            User = null;
        }
    }
}