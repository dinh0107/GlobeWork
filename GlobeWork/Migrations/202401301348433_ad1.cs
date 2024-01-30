namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ad1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "GoogleId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "GoogleId");
        }
    }
}
