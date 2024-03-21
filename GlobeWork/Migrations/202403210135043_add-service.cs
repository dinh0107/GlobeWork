namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addservice : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Services",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IntDate = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                        Sort = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        TypeService = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.JobPosts", "Service_Id", c => c.Int());
            CreateIndex("dbo.JobPosts", "Service_Id");
            AddForeignKey("dbo.JobPosts", "Service_Id", "dbo.Services", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.JobPosts", "Service_Id", "dbo.Services");
            DropIndex("dbo.JobPosts", new[] { "Service_Id" });
            DropColumn("dbo.JobPosts", "Service_Id");
            DropTable("dbo.Services");
        }
    }
}
