using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Security;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using ReviewR.Web.Infrastructure;
using ReviewR.Web.Services;

[assembly: WebActivator.PreApplicationStartMethod(typeof(ReviewR.Web.AuthenticationModule), "Start")]
namespace ReviewR.Web
{
    public class AuthenticationModule : IHttpModule
    {
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(AuthenticationModule));
        }

        public void Init(HttpApplication context)
        {
            context.PostAuthenticateRequest += OnPostAuthenticateRequest;
        }

        void OnPostAuthenticateRequest(object sender, EventArgs e)
        {
            var context = HttpContext.Current;
            var request = HttpContext.Current.Request;
            if (request.IsAuthenticated)
            {
                HttpCookie authCookie = request.Cookies[FormsAuthentication.FormsCookieName];
                if (authCookie != null)
                {
                    FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                    var roles = authTicket.UserData.Split('|');
                    var user = new ReviewRPrincipal(context.User.Identity, new HashSet<string>(roles));
                    context.User = Thread.CurrentPrincipal = user;
                }
            }
        }

        public void Dispose()
        {
        }
    }
}