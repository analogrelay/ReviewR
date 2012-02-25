namespace ReviewR.Web.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class FixMisnamedChangeIdColumn : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("DiffLines", "Hunk_Id", "FileChanges");
            DropIndex("DiffLines", new[] { "Hunk_Id" });
            AddForeignKey("DiffLines", "ChangeId", "FileChanges", "Id", cascadeDelete: true);
            CreateIndex("DiffLines", "ChangeId");
            DropColumn("DiffLines", "Hunk_Id");
        }
        
        public override void Down()
        {
            AddColumn("DiffLines", "Hunk_Id", c => c.Int());
            DropIndex("DiffLines", new[] { "ChangeId" });
            DropForeignKey("DiffLines", "ChangeId", "FileChanges");
            CreateIndex("DiffLines", "Hunk_Id");
            AddForeignKey("DiffLines", "Hunk_Id", "FileChanges", "Id");
        }
    }
}
