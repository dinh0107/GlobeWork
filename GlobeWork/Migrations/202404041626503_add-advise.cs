namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addadvise : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Advises", "CompanyName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Advises", "CompanyName");
        }
    }
}
