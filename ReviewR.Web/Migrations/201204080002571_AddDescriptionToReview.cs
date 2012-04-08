namespace ReviewR.Web.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddDescriptionToReview : DbMigration
    {
        public override void Up()
        {
            AddColumn("Reviews", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("Reviews", "Description");
        }
    }
}
