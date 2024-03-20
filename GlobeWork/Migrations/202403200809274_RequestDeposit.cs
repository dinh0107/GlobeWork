namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RequestDeposit : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RequestDeposits",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CompanyId = c.Int(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StatusRequest = c.Int(nullable: false),
                        Company_EmployerId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.Company_EmployerId)
                .Index(t => t.Company_EmployerId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RequestDeposits", "Company_EmployerId", "dbo.Companies");
            DropIndex("dbo.RequestDeposits", new[] { "Company_EmployerId" });
            DropTable("dbo.RequestDeposits");
        }
    }
}
