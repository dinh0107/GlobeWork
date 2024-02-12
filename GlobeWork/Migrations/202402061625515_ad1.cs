namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ad1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StudyAbroads", "Incentives", c => c.String());
            AddColumn("dbo.StudyAbroads", "ExpirationDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.StudyAbroads", "ExpirationDate");
            DropColumn("dbo.StudyAbroads", "Incentives");
        }
    }
}
