namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatecoutruyt : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Countries", "Job", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Countries", "Job");
        }
    }
}
