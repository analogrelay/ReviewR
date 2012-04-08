using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ReviewR.Web.Infrastructure;
using ReviewR.Web.Models;
using ReviewR.Web.Models.Data;
using ReviewR.Web.Models.Request;
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
        public HttpResponseMessage Post(CreateSessionRequestModel model)
        {
            if (ModelState.IsValid)
            {
                User user = Auth.LogIn(model.Email, model.Password);
                if (user != null)
                {
                    User = new ReviewRPrincipal(user);
                    User.Identity.RememberMe = model.RememberMe;
                    return new HttpResponseMessage<ReviewRIdentity>(User.Identity, HttpStatusCode.Created);
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.Forbidden);
                }
            }
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }

        public HttpResponseMessage Delete()
        {
            User = null;
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}