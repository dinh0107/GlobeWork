namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatepartnerfooter : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Partners", "Footer", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Partners", "Footer");
        }
    }
}
