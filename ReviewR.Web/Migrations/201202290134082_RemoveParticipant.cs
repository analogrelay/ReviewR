namespace ReviewR.Web.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class RemoveParticipant : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Participants", "ReviewId", "Reviews");
            DropForeignKey("Participants", "UserId", "Users");
            DropIndex("Participants", new[] { "ReviewId" });
            DropIndex("Participants", new[] { "UserId" });
            DropTable("Participants");
        }
        
        public override void Down()
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
                .PrimaryKey(t => t.Id);
            
            CreateIndex("Participants", "UserId");
            CreateIndex("Participants", "ReviewId");
            AddForeignKey("Participants", "UserId", "Users", "Id", cascadeDelete: true);
            AddForeignKey("Participants", "ReviewId", "Reviews", "Id", cascadeDelete: true);
        }
    }
}
