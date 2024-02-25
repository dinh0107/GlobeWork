namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ad : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.StudyAbroadCareers", "StudyAbroad_Id", "dbo.StudyAbroads");
            DropForeignKey("dbo.StudyAbroadCareers", "Career_Id", "dbo.Careers");
            DropIndex("dbo.StudyAbroadCareers", new[] { "StudyAbroad_Id" });
            DropIndex("dbo.StudyAbroadCareers", new[] { "Career_Id" });
            AddColumn("dbo.StudyAbroads", "CareerId", c => c.Int());
            CreateIndex("dbo.StudyAbroads", "CareerId");
            AddForeignKey("dbo.StudyAbroads", "CareerId", "dbo.Careers", "Id");
            DropTable("dbo.StudyAbroadCareers");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.StudyAbroadCareers",
                c => new
                    {
                        StudyAbroad_Id = c.Int(nullable: false),
                        Career_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.StudyAbroad_Id, t.Career_Id });
            
            DropForeignKey("dbo.StudyAbroads", "CareerId", "dbo.Careers");
            DropIndex("dbo.StudyAbroads", new[] { "CareerId" });
            DropColumn("dbo.StudyAbroads", "CareerId");
            CreateIndex("dbo.StudyAbroadCareers", "Career_Id");
            CreateIndex("dbo.StudyAbroadCareers", "StudyAbroad_Id");
            AddForeignKey("dbo.StudyAbroadCareers", "Career_Id", "dbo.Careers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.StudyAbroadCareers", "StudyAbroad_Id", "dbo.StudyAbroads", "Id", cascadeDelete: true);
        }
    }
}
