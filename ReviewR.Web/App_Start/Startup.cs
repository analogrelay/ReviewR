using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ReviewR.Web.Infrastructure;

[assembly: WebActivator.PreApplicationStartMethod(typeof(ReviewR.Web.App_Start.Startup), "Start")]
namespace ReviewR.Web.App_Start
{
    public static class Startup
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void Start()
        {
            RegisterGlobalFilters(GlobalFilters.Filters);

            ValueProviderFactories.Factories.Add(new RequestDataValueProviderFactory());

            DbMigrator migrator = new DbMigrator(new Migrations.Configuration());
            migrator.Update();

            Routes.RegisterRoutes(RouteTable.Routes);

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine()
            {
                AreaMasterLocationFormats = new string[0],
                AreaPartialViewLocationFormats = new string[0],
                AreaViewLocationFormats = new string[0],
                PartialViewLocationFormats = new [] { "~/Views/{1}/{0}.cshtml", "~/Views/{0}.cshtml" },
                ViewLocationFormats = new[] { "~/Views/{1}/{0}.cshtml", "~/Views/{0}.cshtml" },
                MasterLocationFormats = new[] { "~/Views/{1}/{0}.cshtml", "~/Views/{0}.cshtml" },
                FileExtensions = new[] { "cshtml" }
            });
        }
    }
}