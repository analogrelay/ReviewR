using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using ReviewR.Web.Infrastructure;
using ReviewR.Web.Models;
using ReviewR.Web.Models.Request;
using ReviewR.Web.Models.Response;
using ReviewR.Web.Services;

namespace ReviewR.Web.Api
{
    public class UsersController : ReviewRApiController
    {
        public AuthenticationService Auth { get; set; }

        public UsersController(AuthenticationService auth)
        {
            Auth = auth;
        }

        // POST /api/users
        [AllowAnonymous]
        public HttpResponseMessage Post(CreateUserRequestModel model)
        {
            var response = Request.CreateResponse();
            if (ModelState.IsValid)
            {
                var result = Auth.CreateUser(model.Email, model.DisplayName, model.Password);
                if (result.Item2 == CreateUserResult.Success)
                {
                    // Set the authentication token
                    User = new ReviewRPrincipal(result.Item1);

                    // This will send an unencrypted copy of the token down to the client so they can optimize the UI based on that.
                    return new HttpResponseMessage<ReviewRIdentity>(User.Identity, HttpStatusCode.Created);
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.Conflict);
                }
            }
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }
    }
}