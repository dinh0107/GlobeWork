namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ad : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Likes", "StudyAbroadId", c => c.Int());
            CreateIndex("dbo.Likes", "StudyAbroadId");
            AddForeignKey("dbo.Likes", "StudyAbroadId", "dbo.StudyAbroads", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Likes", "StudyAbroadId", "dbo.StudyAbroads");
            DropIndex("dbo.Likes", new[] { "StudyAbroadId" });
            DropColumn("dbo.Likes", "StudyAbroadId");
        }
    }
}
