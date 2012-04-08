namespace ReviewR.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDatesToReviewAndIteration : DbMigration
    {
        public override void Up()
        {
            AddColumn("Reviews", "CreatedOn", c => c.DateTimeOffset(nullable: false));
            AddColumn("Iterations", "StartedOn", c => c.DateTimeOffset(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("Iterations", "StartedOn");
            DropColumn("Reviews", "CreatedOn");
        }
    }
}
