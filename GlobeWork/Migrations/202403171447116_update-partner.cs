namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatepartner : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Partners",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Url = c.String(maxLength: 500),
                        Image = c.String(maxLength: 500),
                        Sort = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropTable("dbo.Parters");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Parters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Sort = c.Int(nullable: false),
                        Title = c.String(nullable: false),
                        Url = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            DropTable("dbo.Partners");
        }
    }
}
