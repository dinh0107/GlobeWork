namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class uodateservice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Services", "Description", c => c.String(nullable: false, maxLength: 500));
            AddColumn("dbo.Services", "Image", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Services", "Image");
            DropColumn("dbo.Services", "Description");
        }
    }
}
