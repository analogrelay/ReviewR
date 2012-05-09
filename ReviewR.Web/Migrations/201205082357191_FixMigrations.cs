namespace ReviewR.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixMigrations : DbMigration
    {
        public override void Up()
        {
            DropColumn("Participants", "Status");
            DropColumn("FileChanges", "ChangeType");
        }
        
        public override void Down()
        {
            AddColumn("FileChanges", "ChangeType", c => c.Int(nullable: false));
            AddColumn("Participants", "Status", c => c.Int(nullable: false));
        }
    }
}
