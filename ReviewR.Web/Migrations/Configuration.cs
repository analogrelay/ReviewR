namespace ReviewR.Web.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using ReviewR.Web.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<DefaultDataRepository>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(DefaultDataRepository context)
        {
            context.Roles.Add(new Role() { RoleName = "Admin" });
            context.SaveChanges();
        }
    }
}
