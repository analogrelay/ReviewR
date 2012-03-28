using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

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

            DbMigrator migrator = new DbMigrator(new Migrations.Configuration());
            migrator.Update();

            Routes.RegisterRoutes(RouteTable.Routes);
        }
    }
}