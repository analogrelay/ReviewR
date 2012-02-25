namespace ReviewR.Web.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class RemoveDiffHunks : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("DiffHunks", "ChangeId", "FileChanges");
            DropForeignKey("DiffLines", "HunkId", "DiffHunks");
            DropIndex("DiffHunks", new[] { "ChangeId" });
            DropIndex("DiffLines", new[] { "HunkId" });
            RenameColumn(table: "DiffLines", name: "HunkId", newName: "Hunk_Id");
            AddColumn("DiffLines", "ChangeId", c => c.Int(nullable: false));
            AddColumn("DiffLines", "SourceLine", c => c.Int(nullable: false));
            AddColumn("DiffLines", "ModifiedLine", c => c.Int(nullable: false));
            AddForeignKey("DiffLines", "Hunk_Id", "FileChanges", "Id");
            CreateIndex("DiffLines", "Hunk_Id");
            DropColumn("FileChanges", "Content");
            DropColumn("FileChanges", "NewContent");
            DropTable("DiffHunks");
        }
        
        public override void Down()
        {
            CreateTable(
                "DiffHunks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ChangeId = c.Int(nullable: false),
                        SourceLine = c.Int(nullable: false),
                        ModifiedLine = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("FileChanges", "NewContent", c => c.String());
            AddColumn("FileChanges", "Content", c => c.String());
            DropIndex("DiffLines", new[] { "Hunk_Id" });
            DropForeignKey("DiffLines", "Hunk_Id", "FileChanges");
            DropColumn("DiffLines", "ModifiedLine");
            DropColumn("DiffLines", "SourceLine");
            DropColumn("DiffLines", "ChangeId");
            RenameColumn(table: "DiffLines", name: "Hunk_Id", newName: "HunkId");
            CreateIndex("DiffLines", "HunkId");
            CreateIndex("DiffHunks", "ChangeId");
            AddForeignKey("DiffLines", "HunkId", "DiffHunks", "Id", cascadeDelete: true);
            AddForeignKey("DiffHunks", "ChangeId", "FileChanges", "Id", cascadeDelete: true);
        }
    }
}
