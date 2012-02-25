namespace ReviewR.Web.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Reviews : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Reviews",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "FileChanges",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ReviewId = c.Int(nullable: false),
                        FileName = c.String(maxLength: 255),
                        Content = c.String(),
                        NewFileName = c.String(maxLength: 255),
                        NewContent = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Reviews", t => t.ReviewId, cascadeDelete: true)
                .Index(t => t.ReviewId);
            
            CreateTable(
                "DiffHunks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ChangeId = c.Int(nullable: false),
                        SourceLine = c.Int(nullable: false),
                        ModifiedLine = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("FileChanges", t => t.ChangeId, cascadeDelete: true)
                .Index(t => t.ChangeId);
            
            CreateTable(
                "DiffLines",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        HunkId = c.Int(nullable: false),
                        Content = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("DiffHunks", t => t.HunkId, cascadeDelete: true)
                .Index(t => t.HunkId);
            
        }
        
        public override void Down()
        {
            DropIndex("DiffLines", new[] { "HunkId" });
            DropIndex("DiffHunks", new[] { "ChangeId" });
            DropIndex("FileChanges", new[] { "ReviewId" });
            DropIndex("Reviews", new[] { "UserId" });
            DropForeignKey("DiffLines", "HunkId", "DiffHunks");
            DropForeignKey("DiffHunks", "ChangeId", "FileChanges");
            DropForeignKey("FileChanges", "ReviewId", "Reviews");
            DropForeignKey("Reviews", "UserId", "Users");
            DropTable("DiffLines");
            DropTable("DiffHunks");
            DropTable("FileChanges");
            DropTable("Reviews");
        }
    }
}
