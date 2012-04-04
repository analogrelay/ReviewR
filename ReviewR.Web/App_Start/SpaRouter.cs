using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;

//[assembly: WebActivator.PreApplicationStartMethod(typeof(ReviewR.Web.App_Start.SpaRouter), "Start")]
namespace ReviewR.Web.App_Start
{
    public class SpaRouter : IHttpModule
    {
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(SpaRouter));
        }

        public void Dispose()
        {
        }

        public void Init(HttpApplication app)
        {
            app.BeginRequest += app_BeginRequest;
        }

        void app_BeginRequest(object sender, EventArgs e)
        {
            HttpContextBase ctx = new HttpContextWrapper(HttpContext.Current);
            HandleRequest(HostingEnvironment.VirtualPathProvider, ctx);
        }

        private void HandleRequest(VirtualPathProvider pathProvider, HttpContextBase ctx)
        {
            string path = ctx.Request.AppRelativeCurrentExecutionFilePath;
            if (!path.EndsWith("html") && pathProvider.FileExists(path + ".html"))
            {
                ctx.RewritePath(path + ".html");
            }
            else if (!pathProvider.FileExists(path))
            {
                // Rewrite the request for the root of the app and let client-side code handle it
                ctx.RewritePath("~/");
            }
        }
    }
}