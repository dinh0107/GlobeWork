namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatearticle : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Articles", "ShowHot", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Articles", "ShowHot");
        }
    }
}
