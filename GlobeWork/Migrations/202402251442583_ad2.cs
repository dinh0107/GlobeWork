namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ad2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Advises", "EmployerId", c => c.Int(nullable: false));
            CreateIndex("dbo.Advises", "EmployerId");
            AddForeignKey("dbo.Advises", "EmployerId", "dbo.Employers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Advises", "EmployerId", "dbo.Employers");
            DropIndex("dbo.Advises", new[] { "EmployerId" });
            DropColumn("dbo.Advises", "EmployerId");
        }
    }
}
