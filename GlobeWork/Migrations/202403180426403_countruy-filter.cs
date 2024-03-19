namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class countruyfilter : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Countries", "Filter", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Countries", "Filter");
        }
    }
}
