namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class advise : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Advises", "UserId", "dbo.Users");
            DropIndex("dbo.Advises", new[] { "UserId" });
            AddColumn("dbo.Advises", "CustomerInfo_Fullname", c => c.String(nullable: false, maxLength: 50));
            AddColumn("dbo.Advises", "CustomerInfo_Address", c => c.String(nullable: false, maxLength: 200));
            AddColumn("dbo.Advises", "CustomerInfo_Mobile", c => c.String(nullable: false, maxLength: 11));
            AddColumn("dbo.Advises", "CustomerInfo_Email", c => c.String(nullable: false, maxLength: 50));
            AddColumn("dbo.Advises", "CustomerInfo_Body", c => c.String(maxLength: 200));
            AddColumn("dbo.Advises", "CustomerInfo_IsNewMember", c => c.Boolean(nullable: false));
            DropColumn("dbo.Advises", "UserId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Advises", "UserId", c => c.Int());
            DropColumn("dbo.Advises", "CustomerInfo_IsNewMember");
            DropColumn("dbo.Advises", "CustomerInfo_Body");
            DropColumn("dbo.Advises", "CustomerInfo_Email");
            DropColumn("dbo.Advises", "CustomerInfo_Mobile");
            DropColumn("dbo.Advises", "CustomerInfo_Address");
            DropColumn("dbo.Advises", "CustomerInfo_Fullname");
            CreateIndex("dbo.Advises", "UserId");
            AddForeignKey("dbo.Advises", "UserId", "dbo.Users", "Id");
        }
    }
}
