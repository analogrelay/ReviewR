namespace ReviewR.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GoBackToTablePerHierarchyForFileChange : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Participants", "ReviewId", "Reviews");
            DropForeignKey("Participants", "UserId", "Users");
            DropIndex("Participants", new[] { "ReviewId" });
            DropIndex("Participants", new[] { "UserId" });
            AddColumn("FileChanges", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            DropTable("Participants");
        }
        
        public override void Down()
        {
            CreateTable(
                "Participants",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ReviewId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        Required = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropColumn("FileChanges", "Discriminator");
            CreateIndex("Participants", "UserId");
            CreateIndex("Participants", "ReviewId");
            AddForeignKey("Participants", "UserId", "Users", "Id");
            AddForeignKey("Participants", "ReviewId", "Reviews", "Id", cascadeDelete: true);
        }
    }
}
