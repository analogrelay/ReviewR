namespace ReviewR.Web.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using ReviewR.Web.Models.Data;

    public sealed class Configuration : DbMigrationsConfiguration<ReviewRDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ReviewRDbContext context)
        {
            if (!context.Roles.Where(r => r.RoleName == "Admin").Any())
            {
                context.Roles.Add(new Role() { RoleName = "Admin" });
                context.SaveChanges();
            }
        }
    }
}
