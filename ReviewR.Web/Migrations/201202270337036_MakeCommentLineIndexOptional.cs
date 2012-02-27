namespace ReviewR.Web.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class MakeCommentLineIndexOptional : DbMigration
    {
        public override void Up()
        {
            AlterColumn("Comments", "DiffLineIndex", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("Comments", "DiffLineIndex", c => c.Int(nullable: false));
        }
    }
}
