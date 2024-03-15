namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class city : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cities", "CountruyId", c => c.Int(nullable: false));
            DropColumn("dbo.Cities", "CounId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Cities", "CounId", c => c.Int(nullable: false));
            DropColumn("dbo.Cities", "CountruyId");
        }
    }
}
