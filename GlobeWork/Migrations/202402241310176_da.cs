namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class da : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Cities", "JobPost_Id", "dbo.JobPosts");
            DropIndex("dbo.Cities", new[] { "JobPost_Id" });
            CreateTable(
                "dbo.CityJobPosts",
                c => new
                    {
                        City_Id = c.Int(nullable: false),
                        JobPost_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.City_Id, t.JobPost_Id })
                .ForeignKey("dbo.Cities", t => t.City_Id, cascadeDelete: true)
                .ForeignKey("dbo.JobPosts", t => t.JobPost_Id, cascadeDelete: true)
                .Index(t => t.City_Id)
                .Index(t => t.JobPost_Id);
            
            DropColumn("dbo.Cities", "JobPost_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Cities", "JobPost_Id", c => c.Int());
            DropForeignKey("dbo.CityJobPosts", "JobPost_Id", "dbo.JobPosts");
            DropForeignKey("dbo.CityJobPosts", "City_Id", "dbo.Cities");
            DropIndex("dbo.CityJobPosts", new[] { "JobPost_Id" });
            DropIndex("dbo.CityJobPosts", new[] { "City_Id" });
            DropTable("dbo.CityJobPosts");
            CreateIndex("dbo.Cities", "JobPost_Id");
            AddForeignKey("dbo.Cities", "JobPost_Id", "dbo.JobPosts", "Id");
        }
    }
}
