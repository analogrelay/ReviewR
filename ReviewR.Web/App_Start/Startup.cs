using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Ninject;
using Ninject.Modules;
using ReviewR.Web.Infrastructure;

[assembly: WebActivator.PreApplicationStartMethod(typeof(ReviewR.Web.App_Start.Startup), "Start")]
namespace ReviewR.Web.App_Start
{
    public static class Startup
    {
        public static void Start()
        {
            GlobalConfiguration.Configuration.Filters.Add(new AuthorizeAttribute());
            GlobalConfiguration.Configuration.Formatters.Remove(GlobalConfiguration.Configuration.Formatters.JsonFormatter);
            GlobalConfiguration.Configuration.Formatters.Insert(0,
                new BetterJsonMediaTypeFormatter()
                {
                    SerializerSettings = new JsonSerializerSettings()
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    }
                });
            
            DbMigrator migrator = new DbMigrator(new Migrations.Configuration());
            migrator.Update();

            Routes.RegisterRoutes(RouteTable.Routes);
        }
    }
}