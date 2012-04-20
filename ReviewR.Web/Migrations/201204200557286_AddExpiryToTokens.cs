namespace ReviewR.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddExpiryToTokens : DbMigration
    {
        public override void Up()
        {
            AddColumn("Tokens", "Expires", c => c.DateTimeOffset(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("Tokens", "Expires");
        }
    }
}
