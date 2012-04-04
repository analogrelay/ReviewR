using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Hosting;
using ReviewR.Web.Models;
using ReviewR.Web.Services;

namespace ReviewR.Web.Infrastructure
{
    public class AuthenticationHandler : DelegatingHandler
    {
        public TokenService Tokens { get; private set; }
        
        public AuthenticationHandler(TokenService tokens)
        {
            Tokens = tokens;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Pull out the authentication cookie if it's present
            var currentCookie =
                request.Headers.GetCookies().Where(
                    c => c.Cookies.Where(
                        s => String.Equals(
                            s.Name,
                            Tokens.CookieName,
                            StringComparison.OrdinalIgnoreCase)).Any()).FirstOrDefault();

            ReviewRPrincipal user = null;
            if (currentCookie != null)
            {
                user = new ReviewRPrincipal(Tokens.ReadAuthCookie(currentCookie));
                request.Properties[HttpPropertyKeys.UserPrincipalKey] = user;
            }
            return base.SendAsync(request, cancellationToken);
        }
    }
}