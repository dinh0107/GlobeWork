namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class lastdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companies", "LastEditDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Companies", "LastEditDate");
        }
    }
}
