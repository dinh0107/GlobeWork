namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ad3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ApplyJobs", "CompanyId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ApplyJobs", "CompanyId");
        }
    }
}
