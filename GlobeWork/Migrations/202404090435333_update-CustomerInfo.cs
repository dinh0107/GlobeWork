namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateCustomerInfo : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Advises", "CustomerInfo_Address", c => c.String(maxLength: 200));
            AlterColumn("dbo.Advises", "CustomerInfo_Email", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Advises", "CustomerInfo_Email", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Advises", "CustomerInfo_Address", c => c.String(nullable: false, maxLength: 200));
        }
    }
}
