namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class uppdatesomedata : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Skills", "TypeSkill", c => c.Int(nullable: false));
            AddColumn("dbo.Contacts", "TypeContact", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Contacts", "TypeContact");
            DropColumn("dbo.Skills", "TypeSkill");
        }
    }
}
