using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using ReviewR.Web.Infrastructure;

namespace ReviewR.Web.App_Start
{
    public static class Routes
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.Ignore("{resource}.axd/{*pathInfo}");
            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional },
                constraints: new { controller = @"[A-Za-z]*" }
            );

            DynamicData.Registration.Register(routes);

            routes.Add(new WebPageRoute("~/Index.cshtml"));
        }
    }
}