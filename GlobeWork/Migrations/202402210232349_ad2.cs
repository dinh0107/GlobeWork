namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ad2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ApplyJobs", "JobPostId", "dbo.JobPosts");
            DropIndex("dbo.ApplyJobs", new[] { "JobPostId" });
            AddColumn("dbo.ApplyJobs", "StudyAbroadId", c => c.Int());
            AddColumn("dbo.StudyAbroads", "ListImage", c => c.String());
            AlterColumn("dbo.ApplyJobs", "JobPostId", c => c.Int());
            CreateIndex("dbo.ApplyJobs", "JobPostId");
            CreateIndex("dbo.ApplyJobs", "StudyAbroadId");
            AddForeignKey("dbo.ApplyJobs", "StudyAbroadId", "dbo.StudyAbroads", "Id");
            AddForeignKey("dbo.ApplyJobs", "JobPostId", "dbo.JobPosts", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ApplyJobs", "JobPostId", "dbo.JobPosts");
            DropForeignKey("dbo.ApplyJobs", "StudyAbroadId", "dbo.StudyAbroads");
            DropIndex("dbo.ApplyJobs", new[] { "StudyAbroadId" });
            DropIndex("dbo.ApplyJobs", new[] { "JobPostId" });
            AlterColumn("dbo.ApplyJobs", "JobPostId", c => c.Int(nullable: false));
            DropColumn("dbo.StudyAbroads", "ListImage");
            DropColumn("dbo.ApplyJobs", "StudyAbroadId");
            CreateIndex("dbo.ApplyJobs", "JobPostId");
            AddForeignKey("dbo.ApplyJobs", "JobPostId", "dbo.JobPosts", "Id", cascadeDelete: true);
        }
    }
}
