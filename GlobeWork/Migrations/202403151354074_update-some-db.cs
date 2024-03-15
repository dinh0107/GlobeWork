namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatesomedb : DbMigration
    {
        public override void Up()
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
            
            AddColumn("dbo.JobPosts", "CounId", c => c.Int(nullable: false));
            AddColumn("dbo.JobPosts", "Country_Id", c => c.Int());
            CreateIndex("dbo.JobPosts", "Country_Id");
            AddForeignKey("dbo.JobPosts", "Country_Id", "dbo.Countries", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.JobPosts", "Country_Id", "dbo.Countries");
            DropIndex("dbo.JobPosts", new[] { "Country_Id" });
            DropColumn("dbo.JobPosts", "Country_Id");
            DropColumn("dbo.JobPosts", "CounId");
            DropTable("dbo.Parters");
        }
    }
}
