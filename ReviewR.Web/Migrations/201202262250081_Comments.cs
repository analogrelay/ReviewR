namespace ReviewR.Web.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Comments : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Comments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FileId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        DiffLineIndex = c.Int(nullable: false),
                        Content = c.String(),
                        PostedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Users", t => t.UserId)
                .ForeignKey("FileChanges", t => t.FileId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.FileId);
            
        }
        
        public override void Down()
        {
            DropIndex("Comments", new[] { "FileId" });
            DropIndex("Comments", new[] { "UserId" });
            DropForeignKey("Comments", "FileId", "FileChanges");
            DropForeignKey("Comments", "UserId", "Users");
            DropTable("Comments");
        }
    }
}
