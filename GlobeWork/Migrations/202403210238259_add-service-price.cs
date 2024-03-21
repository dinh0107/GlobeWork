namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addserviceprice : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Services", "Amount", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Services", "Amount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
