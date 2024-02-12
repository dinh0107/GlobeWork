namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ad2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Companies", "CompanySize", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Companies", "CompanySize", c => c.String(maxLength: 10));
        }
    }
}
