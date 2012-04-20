namespace ReviewR.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OAuth : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Tokens",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserId = c.Int(nullable: false),
                        Persistent = c.Boolean(nullable: false),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "Credentials",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Provider = c.String(),
                        Identifier = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            DropColumn("Users", "Password");
            DropColumn("Users", "PasswordSalt");
        }
        
        public override void Down()
        {
            AddColumn("Users", "PasswordSalt", c => c.String(maxLength: 255));
            AddColumn("Users", "Password", c => c.String(maxLength: 255));
            DropIndex("Credentials", new[] { "UserId" });
            DropIndex("Tokens", new[] { "UserId" });
            DropForeignKey("Credentials", "UserId", "Users");
            DropForeignKey("Tokens", "UserId", "Users");
            DropTable("Credentials");
            DropTable("Tokens");
        }
    }
}
