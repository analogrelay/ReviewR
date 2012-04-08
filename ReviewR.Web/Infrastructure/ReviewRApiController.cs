using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using Ninject;
using ReviewR.Web.Models;
using ReviewR.Web.Services;

namespace ReviewR.Web.Infrastructure
{
    public class ReviewRApiController : ApiController
    {
        private ReviewRPrincipal _sessionUser;
        private bool _sessionUserSet = false;

        [Inject]
        public TokenService Tokens { get; set; }
        
        public ReviewRPrincipal User {
            get { return _sessionUserSet ? _sessionUser : (Request.GetUserPrincipal() as ReviewRPrincipal); }
            set { _sessionUser = value; _sessionUserSet = true; }
        }

        public override Task<HttpResponseMessage> ExecuteAsync(HttpControllerContext controllerContext, CancellationToken cancellationToken)
        {
            return base.ExecuteAsync(controllerContext, cancellationToken).ContinueWith(t => PostProcessMessage(t));
        }

        private HttpResponseMessage PostProcessMessage(Task<HttpResponseMessage> task)
        {
            if (!task.IsCompleted)
            {
                return task.Result;
            }
            
            HttpResponseMessage resp = task.Result;
            var currentCookie =
                Request.Headers.GetCookies().Where(
                    c => c.Cookies.Where(
                        s => String.Equals(
                            s.Name,
                            Tokens.CookieName,
                            StringComparison.OrdinalIgnoreCase)).Any()).FirstOrDefault();

            if (resp.Headers.CacheControl == null)
            {
                resp.Headers.CacheControl = new CacheControlHeaderValue();
            }
            resp.Headers.CacheControl.NoCacheHeaders.Add("Set-Cookie");
            if (User != null)
            {
                // Refresh the cookie
                resp.Headers.AddCookies(new[] {
                    Tokens.CreateAuthCookie(User.Identity)
                });
            }
            else if(currentCookie != null)
            {
                // We need to clear the existing cookie, so we set an already expired empty cookie in it's place
                resp.Headers.AddCookies(new[] {
                    Tokens.CreateSignoutCookie()
                });
            }
            return resp;
        }
    }
}