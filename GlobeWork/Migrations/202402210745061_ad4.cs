namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ad4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ConfigSites", "NapTien", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ConfigSites", "NapTien");
        }
    }
}
