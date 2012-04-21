using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Hosting;
using System.Web.Security;
using Newtonsoft.Json;
using ReviewR.Web.Models;
using ReviewR.Web.Models.Data;
using ReviewR.Web.Models.Response;
using ReviewR.Web.Services;

namespace ReviewR.Web.Infrastructure
{
    public class AuthenticationHandler : DelegatingHandler
    {
        public AuthenticationService Auth { get; set; }

        public AuthenticationHandler(AuthenticationService auth)
        {
            Auth = auth;
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // If there's a basic authorization header...
            var authHeader = request.Headers.Authorization;
            if (authHeader != null && String.Equals(authHeader.Scheme, "Basic", StringComparison.OrdinalIgnoreCase))
            {
                // The parameter is the encrypted session token, use it to get the user
                User currentUser = Auth.GetUserFromSessionToken(authHeader.Parameter);
                if (currentUser != null)
                {
                    request.Properties[HttpPropertyKeys.UserPrincipalKey] = ReviewRPrincipal.FromUser(currentUser);
                }
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }
}