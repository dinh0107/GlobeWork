namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class countruyup : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Countries", "FilterJob", c => c.Boolean(nullable: false));
            AddColumn("dbo.Countries", "FilterStudy", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Countries", "FilterStudy");
            DropColumn("dbo.Countries", "FilterJob");
        }
    }
}
