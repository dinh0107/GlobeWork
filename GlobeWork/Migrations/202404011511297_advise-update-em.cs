namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adviseupdateem : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Advises", "EmployerId", "dbo.Employers");
            DropIndex("dbo.Advises", new[] { "EmployerId" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.Advises", "EmployerId");
            AddForeignKey("dbo.Advises", "EmployerId", "dbo.Employers", "Id", cascadeDelete: true);
        }
    }
}
