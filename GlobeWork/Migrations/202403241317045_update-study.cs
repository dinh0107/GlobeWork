namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatestudy : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StudyAbroads", "ServiceId", c => c.Int());
            CreateIndex("dbo.StudyAbroads", "ServiceId");
            AddForeignKey("dbo.StudyAbroads", "ServiceId", "dbo.Services", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StudyAbroads", "ServiceId", "dbo.Services");
            DropIndex("dbo.StudyAbroads", new[] { "ServiceId" });
            DropColumn("dbo.StudyAbroads", "ServiceId");
        }
    }
}
