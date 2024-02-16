namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ad1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Certificates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(maxLength: 500),
                        Date = c.DateTime(),
                        UserId = c.Int(nullable: false),
                        Url = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Projects",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(maxLength: 500),
                        StarDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        UserId = c.Int(nullable: false),
                        Url = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            AddColumn("dbo.Users", "Skill_Id", c => c.Int());
            CreateIndex("dbo.Users", "Skill_Id");
            AddForeignKey("dbo.Users", "Skill_Id", "dbo.Skills", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "Skill_Id", "dbo.Skills");
            DropForeignKey("dbo.Projects", "UserId", "dbo.Users");
            DropForeignKey("dbo.Certificates", "UserId", "dbo.Users");
            DropIndex("dbo.Projects", new[] { "UserId" });
            DropIndex("dbo.Certificates", new[] { "UserId" });
            DropIndex("dbo.Users", new[] { "Skill_Id" });
            DropColumn("dbo.Users", "Skill_Id");
            DropTable("dbo.Projects");
            DropTable("dbo.Certificates");
        }
    }
}
