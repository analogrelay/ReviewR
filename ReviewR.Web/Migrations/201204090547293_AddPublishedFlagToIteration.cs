namespace ReviewR.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPublishedFlagToIteration : DbMigration
    {
        public override void Up()
        {
            AddColumn("Iterations", "Published", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("Iterations", "Published");
        }
    }
}
