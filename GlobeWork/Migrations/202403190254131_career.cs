namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class career : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Careers", "TypeCareer", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Careers", "TypeCareer");
        }
    }
}
