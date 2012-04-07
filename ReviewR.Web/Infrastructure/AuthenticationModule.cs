using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ReviewR.Web.Models;
using ReviewR.Web.Services;

namespace ReviewR.Web.Infrastructure
{
    public class AuthenticationModule : IHttpModule
    {
        public TokenService Tokens { get; private set; }

        public AuthenticationModule(TokenService tokens)
        {
            Tokens = tokens;
        }

        public void Dispose()
        {
        }

        public void Init(HttpApplication app)
        {
            app.AuthenticateRequest += app_AuthenticateRequest;
        }

        void app_AuthenticateRequest(object sender, EventArgs e)
        {
            // Get the cookie
            HttpContextBase context = new HttpContextWrapper(HttpContext.Current);
            HttpCookie cookie = context.Request.Cookies[Tokens.CookieName];

            // Extract the value
            if (cookie != null)
            {
                ReviewRIdentity id = Tokens.ReadAuthCookie(cookie.Value);
                if (id != null)
                {
                    context.User = new ReviewRPrincipal(id);
                }
            }
        }
    }
}