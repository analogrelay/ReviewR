namespace ReviewR.Web.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Participants : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Participants",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Required = c.Boolean(nullable: false),
                        ReviewId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Reviews", t => t.ReviewId, cascadeDelete: true)
                .ForeignKey("Users", t => t.UserId, cascadeDelete: false)
                .Index(t => t.ReviewId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropIndex("Participants", new[] { "UserId" });
            DropIndex("Participants", new[] { "ReviewId" });
            DropForeignKey("Participants", "UserId", "Users");
            DropForeignKey("Participants", "ReviewId", "Reviews");
            DropTable("Participants");
        }
    }
}
