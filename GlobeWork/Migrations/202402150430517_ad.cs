namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ad : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "Cover", c => c.String(maxLength: 500));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "Cover");
        }
    }
}
