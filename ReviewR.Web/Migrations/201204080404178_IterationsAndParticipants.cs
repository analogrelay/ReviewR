namespace ReviewR.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IterationsAndParticipants : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("FileChanges", "ReviewId", "Reviews");
            DropIndex("FileChanges", new[] { "ReviewId" });
            CreateTable(
                "Participants",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ReviewId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        Required = c.Boolean(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Reviews", t => t.ReviewId, cascadeDelete: true)
                .ForeignKey("Users", t => t.UserId)
                .Index(t => t.ReviewId)
                .Index(t => t.UserId);
            
            CreateTable(
                "Iterations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ReviewId = c.Int(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Reviews", t => t.ReviewId, cascadeDelete: true)
                .Index(t => t.ReviewId);
            
            AddColumn("FileChanges", "IterationId", c => c.Int(nullable: false));
            AddColumn("FileChanges", "ChangeType", c => c.Int(nullable: false));
            AddForeignKey("FileChanges", "IterationId", "Iterations", "Id", cascadeDelete: true);
            CreateIndex("FileChanges", "IterationId");
            DropColumn("FileChanges", "ReviewId");
            DropColumn("FileChanges", "Discriminator");
        }
        
        public override void Down()
        {
            AddColumn("FileChanges", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            AddColumn("FileChanges", "ReviewId", c => c.Int(nullable: false));
            DropIndex("FileChanges", new[] { "IterationId" });
            DropIndex("Iterations", new[] { "ReviewId" });
            DropIndex("Participants", new[] { "UserId" });
            DropIndex("Participants", new[] { "ReviewId" });
            DropForeignKey("FileChanges", "IterationId", "Iterations");
            DropForeignKey("Iterations", "ReviewId", "Reviews");
            DropForeignKey("Participants", "UserId", "Users");
            DropForeignKey("Participants", "ReviewId", "Reviews");
            DropColumn("FileChanges", "ChangeType");
            DropColumn("FileChanges", "IterationId");
            DropTable("Iterations");
            DropTable("Participants");
            CreateIndex("FileChanges", "ReviewId");
            AddForeignKey("FileChanges", "ReviewId", "Reviews", "Id", cascadeDelete: true);
        }
    }
}
