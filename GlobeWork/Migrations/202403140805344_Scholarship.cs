namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Scholarship : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StudyAbroadCategories", "Scholarship", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.StudyAbroadCategories", "Scholarship");
        }
    }
}
