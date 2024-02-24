namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ad : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Certificates", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.Projects", "Active", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Certificates", "Description", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Certificates", "Description", c => c.String(maxLength: 500));
            DropColumn("dbo.Projects", "Active");
            DropColumn("dbo.Certificates", "Active");
        }
    }
}
