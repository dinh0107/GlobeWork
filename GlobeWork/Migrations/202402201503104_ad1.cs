namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ad1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Likes", "JobId", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Likes", "JobId", c => c.Int(nullable: false));
        }
    }
}
