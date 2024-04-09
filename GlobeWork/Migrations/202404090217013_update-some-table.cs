namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatesometable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "Height", c => c.String());
            AddColumn("dbo.Users", "Weight", c => c.String());
            AddColumn("dbo.Users", "Marriage", c => c.Int(nullable: false));
            AddColumn("dbo.Users", "Health", c => c.Int(nullable: false));
            AddColumn("dbo.Countries", "Image", c => c.String(maxLength: 500));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Countries", "Image");
            DropColumn("dbo.Users", "Health");
            DropColumn("dbo.Users", "Marriage");
            DropColumn("dbo.Users", "Weight");
            DropColumn("dbo.Users", "Height");
        }
    }
}
