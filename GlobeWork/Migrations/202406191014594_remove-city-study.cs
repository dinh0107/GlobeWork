namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removecitystudy : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Cities", "StudyAbroad_Id", "dbo.StudyAbroads");
            DropIndex("dbo.Cities", new[] { "StudyAbroad_Id" });
            DropColumn("dbo.Cities", "StudyAbroad_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Cities", "StudyAbroad_Id", c => c.Int());
            CreateIndex("dbo.Cities", "StudyAbroad_Id");
            AddForeignKey("dbo.Cities", "StudyAbroad_Id", "dbo.StudyAbroads", "Id");
        }
    }
}
