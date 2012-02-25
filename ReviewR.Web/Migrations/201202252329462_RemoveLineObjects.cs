namespace ReviewR.Web.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class RemoveLineObjects : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("DiffLines", "ChangeId", "FileChanges");
            DropIndex("DiffLines", new[] { "ChangeId" });
            AddColumn("FileChanges", "Diff", c => c.String());
            DropTable("DiffLines");
        }
        
        public override void Down()
        {
            CreateTable(
                "DiffLines",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ChangeId = c.Int(nullable: false),
                        Content = c.String(),
                        SourceLine = c.Int(nullable: false),
                        ModifiedLine = c.Int(nullable: false),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            DropColumn("FileChanges", "Diff");
            CreateIndex("DiffLines", "ChangeId");
            AddForeignKey("DiffLines", "ChangeId", "FileChanges", "Id", cascadeDelete: true);
        }
    }
}
