namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adviseupdate : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Advises", "JobPostId", "dbo.JobPosts");
            DropForeignKey("dbo.Advises", "StudyAbroadId", "dbo.StudyAbroads");
            DropIndex("dbo.Advises", new[] { "JobPostId" });
            DropIndex("dbo.Advises", new[] { "StudyAbroadId" });
            AddColumn("dbo.Advises", "Name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Advises", "Name");
            CreateIndex("dbo.Advises", "StudyAbroadId");
            CreateIndex("dbo.Advises", "JobPostId");
            AddForeignKey("dbo.Advises", "StudyAbroadId", "dbo.StudyAbroads", "Id");
            AddForeignKey("dbo.Advises", "JobPostId", "dbo.JobPosts", "Id");
        }
    }
}
