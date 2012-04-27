namespace ReviewR.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveDatabaseTokens : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Tokens", "UserId", "Users");
            DropIndex("Tokens", new[] { "UserId" });
            DropTable("Tokens");
        }
        
        public override void Down()
        {
            CreateTable(
                "Tokens",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserId = c.Int(nullable: false),
                        Persistent = c.Boolean(nullable: false),
                        Value = c.String(),
                        Expires = c.DateTimeOffset(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("Tokens", "UserId");
            AddForeignKey("Tokens", "UserId", "Users", "Id", cascadeDelete: true);
        }
    }
}
