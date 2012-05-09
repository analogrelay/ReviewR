namespace ReviewR.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeDateTimeOffsetsToDateTimes : DbMigration
    {
        public override void Up()
        {
            DropColumn("Reviews", "CreatedOn");
            DropColumn("Iterations", "StartedOn");
            AddColumn("Reviews", "CreatedOn", c => c.DateTime(nullable: false));
            AddColumn("Iterations", "StartedOn", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("Reviews", "CreatedOn");
            DropColumn("Iterations", "StartedOn");
            AddColumn("Reviews", "CreatedOn", c => c.DateTimeOffset(nullable: false));
            AddColumn("Iterations", "StartedOn", c => c.DateTimeOffset(nullable: false));
        }
    }
}
