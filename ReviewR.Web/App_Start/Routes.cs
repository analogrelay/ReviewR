using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Http;
using System.Web.Routing;

namespace ReviewR.Web.App_Start
{
    public static class Routes
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("Content/{*rest}");
            routes.IgnoreRoute("Scripts/{*rest}");
            routes.IgnoreRoute("Client/{*rest}");

            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional },
                constraints: new { controller = @"[A-Za-z]*" }
            );

            routes.MapRoute(
                name: "Template",
                url: "Templates/{*template}",
                defaults: new { controller = "Main", action = "Template" },
                constraints: new { controller = @"[A-Za-z]*", action = @"[A-Za-z]*" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{*all}",
                defaults: new { controller = "Main", action = "Index" },
                constraints: new { controller = @"[A-Za-z]*", action = @"[A-Za-z]*" }
            );

            DynamicData.Registration.Register(routes);
            
        }
    }
}