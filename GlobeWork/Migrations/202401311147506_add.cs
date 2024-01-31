namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CityCandidates", "City_Id", "dbo.Cities");
            DropForeignKey("dbo.CityCandidates", "Candidate_UserId", "dbo.Candidates");
            DropForeignKey("dbo.JobPostCities", "JobPost_Id", "dbo.JobPosts");
            DropForeignKey("dbo.JobPostCities", "City_Id", "dbo.Cities");
            DropIndex("dbo.CityCandidates", new[] { "City_Id" });
            DropIndex("dbo.CityCandidates", new[] { "Candidate_UserId" });
            DropIndex("dbo.JobPostCities", new[] { "JobPost_Id" });
            DropIndex("dbo.JobPostCities", new[] { "City_Id" });
            CreateTable(
                "dbo.Districts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Sort = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                        Prefix = c.String(maxLength: 20),
                        CityId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Cities", t => t.CityId, cascadeDelete: true)
                .Index(t => t.CityId);
            
            CreateTable(
                "dbo.Employers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(nullable: false, maxLength: 100),
                        Password = c.String(nullable: false, maxLength: 20),
                        FullName = c.String(nullable: false, maxLength: 100),
                        Gender = c.String(),
                        PhoneNumber = c.String(nullable: false, maxLength: 10),
                        CompanyName = c.String(),
                        Avatar = c.String(maxLength: 500),
                        Address = c.String(),
                        Active = c.Boolean(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        Token = c.String(),
                        Rank_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Ranks", t => t.Rank_Id)
                .Index(t => t.Rank_Id);
            
            AddColumn("dbo.Cities", "Name", c => c.String(nullable: false, maxLength: 50));
            AddColumn("dbo.Cities", "Sort", c => c.Int(nullable: false));
            AddColumn("dbo.Cities", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.Cities", "Prefix", c => c.String(maxLength: 20));
            AddColumn("dbo.Cities", "ShipFee", c => c.Int());
            AddColumn("dbo.Cities", "JobPost_Id", c => c.Int());
            AddColumn("dbo.Cities", "Candidate_UserId", c => c.Int());
            CreateIndex("dbo.Cities", "JobPost_Id");
            CreateIndex("dbo.Cities", "Candidate_UserId");
            AddForeignKey("dbo.Cities", "JobPost_Id", "dbo.JobPosts", "Id");
            AddForeignKey("dbo.Cities", "Candidate_UserId", "dbo.Candidates", "UserId");
            DropColumn("dbo.Cities", "CityName");
            DropColumn("dbo.Cities", "Code");
            DropColumn("dbo.Users", "TypeUser");
            DropTable("dbo.CityCandidates");
            DropTable("dbo.JobPostCities");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.JobPostCities",
                c => new
                    {
                        JobPost_Id = c.Int(nullable: false),
                        City_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.JobPost_Id, t.City_Id });
            
            CreateTable(
                "dbo.CityCandidates",
                c => new
                    {
                        City_Id = c.Int(nullable: false),
                        Candidate_UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.City_Id, t.Candidate_UserId });
            
            AddColumn("dbo.Users", "TypeUser", c => c.Int(nullable: false));
            AddColumn("dbo.Cities", "Code", c => c.String(maxLength: 50));
            AddColumn("dbo.Cities", "CityName", c => c.String(nullable: false, maxLength: 50));
            DropForeignKey("dbo.Cities", "Candidate_UserId", "dbo.Candidates");
            DropForeignKey("dbo.Employers", "Rank_Id", "dbo.Ranks");
            DropForeignKey("dbo.Cities", "JobPost_Id", "dbo.JobPosts");
            DropForeignKey("dbo.Districts", "CityId", "dbo.Cities");
            DropIndex("dbo.Employers", new[] { "Rank_Id" });
            DropIndex("dbo.Districts", new[] { "CityId" });
            DropIndex("dbo.Cities", new[] { "Candidate_UserId" });
            DropIndex("dbo.Cities", new[] { "JobPost_Id" });
            DropColumn("dbo.Cities", "Candidate_UserId");
            DropColumn("dbo.Cities", "JobPost_Id");
            DropColumn("dbo.Cities", "ShipFee");
            DropColumn("dbo.Cities", "Prefix");
            DropColumn("dbo.Cities", "Active");
            DropColumn("dbo.Cities", "Sort");
            DropColumn("dbo.Cities", "Name");
            DropTable("dbo.Employers");
            DropTable("dbo.Districts");
            CreateIndex("dbo.JobPostCities", "City_Id");
            CreateIndex("dbo.JobPostCities", "JobPost_Id");
            CreateIndex("dbo.CityCandidates", "Candidate_UserId");
            CreateIndex("dbo.CityCandidates", "City_Id");
            AddForeignKey("dbo.JobPostCities", "City_Id", "dbo.Cities", "Id", cascadeDelete: true);
            AddForeignKey("dbo.JobPostCities", "JobPost_Id", "dbo.JobPosts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.CityCandidates", "Candidate_UserId", "dbo.Candidates", "UserId", cascadeDelete: true);
            AddForeignKey("dbo.CityCandidates", "City_Id", "dbo.Cities", "Id", cascadeDelete: true);
        }
    }
}
