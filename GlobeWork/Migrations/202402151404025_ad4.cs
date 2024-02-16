namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ad4 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.JobPostCandidates", newName: "CandidateJobPosts");
            DropPrimaryKey("dbo.CandidateJobPosts");
            AddColumn("dbo.ApplyJobs", "TypeApply", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.CandidateJobPosts", new[] { "Candidate_UserId", "JobPost_Id" });
            DropColumn("dbo.ApplyJobs", "FullName");
            DropColumn("dbo.ApplyJobs", "Email");
            DropColumn("dbo.ApplyJobs", "Phone");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ApplyJobs", "Phone", c => c.String(maxLength: 20));
            AddColumn("dbo.ApplyJobs", "Email", c => c.String(nullable: false, maxLength: 100));
            AddColumn("dbo.ApplyJobs", "FullName", c => c.String(nullable: false, maxLength: 50));
            DropPrimaryKey("dbo.CandidateJobPosts");
            DropColumn("dbo.ApplyJobs", "TypeApply");
            AddPrimaryKey("dbo.CandidateJobPosts", new[] { "JobPost_Id", "Candidate_UserId" });
            RenameTable(name: "dbo.CandidateJobPosts", newName: "JobPostCandidates");
        }
    }
}
