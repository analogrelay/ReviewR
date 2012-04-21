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
                name: "Sessions",
                routeTemplate: "api/v1/sessions/{action}/{id}",
                defaults: new { controller = "sessions", id = RouteParameter.Optional },
                constraints: new { controller = @"[A-Za-z]*" }
            );

            routes.MapHttpRoute(
                name: "MyStuff",
                routeTemplate: "api/v1/my/{action}",
                defaults: new { controller = "my", id = RouteParameter.Optional, },
                constraints: new { controller = @"[A-Za-z]*" }
            );

            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/v1/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional },
                constraints: new { controller = @"[A-Za-z]*" }
            );

            DynamicData.Registration.Register(routes);

            routes.Add(new WebPageRoute("~/Index.cshtml"));
        }
    }
}