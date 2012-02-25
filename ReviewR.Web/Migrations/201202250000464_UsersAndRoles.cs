namespace ReviewR.Web.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class UsersAndRoles : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Roles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RoleName = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(nullable: false, maxLength: 255),
                        Password = c.String(maxLength: 255),
                        PasswordSalt = c.String(maxLength: 255),
                        DisplayName = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Email, unique: true);
            
            CreateTable(
                "UserRoles",
                c => new
                    {
                        User_Id = c.Int(nullable: false),
                        Role_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.User_Id, t.Role_Id })
                .ForeignKey("Users", t => t.User_Id, cascadeDelete: true)
                .ForeignKey("Roles", t => t.Role_Id, cascadeDelete: true)
                .Index(t => t.User_Id)
                .Index(t => t.Role_Id);
            
        }
        
        public override void Down()
        {
            DropIndex("UserRoles", new[] { "Role_Id" });
            DropIndex("UserRoles", new[] { "User_Id" });
            DropForeignKey("UserRoles", "Role_Id", "Roles");
            DropForeignKey("UserRoles", "User_Id", "Users");
            DropTable("UserRoles");
            DropTable("Users");
            DropTable("Roles");
        }
    }
}
