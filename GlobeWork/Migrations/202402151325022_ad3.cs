namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ad3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Follows", "UserId", "dbo.Companies");
            RenameColumn(table: "dbo.CompanyCandidates", name: "Company_UserId", newName: "Company_EmployerId");
            RenameColumn(table: "dbo.CompanyCareers", name: "Company_UserId", newName: "Company_EmployerId");
            RenameColumn(table: "dbo.Companies", name: "UserId", newName: "EmployerId");
            RenameIndex(table: "dbo.Companies", name: "IX_UserId", newName: "IX_EmployerId");
            RenameIndex(table: "dbo.CompanyCandidates", name: "IX_Company_UserId", newName: "IX_Company_EmployerId");
            RenameIndex(table: "dbo.CompanyCareers", name: "IX_Company_UserId", newName: "IX_Company_EmployerId");
            AddColumn("dbo.Follows", "Company_EmployerId", c => c.Int());
            CreateIndex("dbo.Follows", "Company_EmployerId");
            AddForeignKey("dbo.Follows", "Company_EmployerId", "dbo.Companies", "EmployerId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Follows", "Company_EmployerId", "dbo.Companies");
            DropIndex("dbo.Follows", new[] { "Company_EmployerId" });
            DropColumn("dbo.Follows", "Company_EmployerId");
            RenameIndex(table: "dbo.CompanyCareers", name: "IX_Company_EmployerId", newName: "IX_Company_UserId");
            RenameIndex(table: "dbo.CompanyCandidates", name: "IX_Company_EmployerId", newName: "IX_Company_UserId");
            RenameIndex(table: "dbo.Companies", name: "IX_EmployerId", newName: "IX_UserId");
            RenameColumn(table: "dbo.Companies", name: "EmployerId", newName: "UserId");
            RenameColumn(table: "dbo.CompanyCareers", name: "Company_EmployerId", newName: "Company_UserId");
            RenameColumn(table: "dbo.CompanyCandidates", name: "Company_EmployerId", newName: "Company_UserId");
            AddForeignKey("dbo.Follows", "UserId", "dbo.Companies", "UserId", cascadeDelete: true);
        }
    }
}
