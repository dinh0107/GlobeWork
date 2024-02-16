namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ad6 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ApplyJobs", "UserId", "dbo.Candidates");
            DropIndex("dbo.ApplyJobs", new[] { "UserId" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.ApplyJobs", "UserId");
            AddForeignKey("dbo.ApplyJobs", "UserId", "dbo.Candidates", "UserId", cascadeDelete: true);
        }
    }
}
