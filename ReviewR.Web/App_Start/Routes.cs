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

            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional },
                constraints: new { controller = @"[A-Za-z]*" }
            );

            routes.MapRoute(
                name: "View",
                url: "{controller}/{id}/{action}",
                defaults: new { action = "View" },
                constraints: new { controller = @"[A-Za-z]*", id = @"\d+" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}",
                defaults: new { controller = "Home", action = "Index" },
                constraints: new { controller = @"[A-Za-z]*", action = @"[A-Za-z]*" }
            );

            DynamicData.Registration.Register(routes);
            
        }
    }
}