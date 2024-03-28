namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class upadtesutdy : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StudyAbroads", "WageScholarship", c => c.Int(nullable: false));
            DropColumn("dbo.StudyAbroads", "Wages");
        }
        
        public override void Down()
        {
            AddColumn("dbo.StudyAbroads", "Wages", c => c.Int(nullable: false));
            DropColumn("dbo.StudyAbroads", "WageScholarship");
        }
    }
}
