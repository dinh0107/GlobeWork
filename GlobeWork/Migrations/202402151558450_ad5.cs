namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ad5 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ApplyJobs", "Url", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ApplyJobs", "Url");
        }
    }
}
