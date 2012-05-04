using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ReviewR.Web.App_Start;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using ReviewR.Web.Models;
using ReviewR.Web.Services;

[assembly: WebActivator.PreApplicationStartMethod(typeof(DbAdminAuthorizationModule), "Start")]

namespace ReviewR.Web.App_Start
{
    public class DbAdminAuthorizationModule : IHttpModule
    {
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(DbAdminAuthorizationModule));
        }

        public void Dispose()
        {
        }

        public void Init(HttpApplication app)
        {
            app.AuthenticateRequest += context_AuthenticateRequest;
        }

        void context_AuthenticateRequest(object sender, EventArgs e)
        {
            HttpContextBase ctx = new HttpContextWrapper(HttpContext.Current);
            SessionToken token = Utils.GetActiveSessionToken(ctx);
            if (token != null)
            {
                ctx.User = token.User;
            }
        }
    }
}