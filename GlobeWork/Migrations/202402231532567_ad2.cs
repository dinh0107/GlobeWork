namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ad2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Projects", "Customer", c => c.String(nullable: false));
            AddColumn("dbo.Projects", "Postion", c => c.String(nullable: false));
            AddColumn("dbo.Projects", "Member", c => c.String(nullable: false));
            AddColumn("dbo.Projects", "Tech", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Projects", "Tech");
            DropColumn("dbo.Projects", "Member");
            DropColumn("dbo.Projects", "Postion");
            DropColumn("dbo.Projects", "Customer");
        }
    }
}
