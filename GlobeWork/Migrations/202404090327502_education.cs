namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class education : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Educations", "Majors", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Educations", "Majors", c => c.String(nullable: false));
        }
    }
}
