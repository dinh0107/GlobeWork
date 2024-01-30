namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ad : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FullName = c.String(),
                        Username = c.String(maxLength: 50),
                        Email = c.String(nullable: false, maxLength: 100),
                        Password = c.String(maxLength: 60),
                        Phone = c.String(maxLength: 20),
                        Avatar = c.String(maxLength: 500),
                        Active = c.Boolean(nullable: false),
                        FaceBookID = c.String(),
                        Token = c.String(),
                        CreateDate = c.DateTime(nullable: false),
                        TypeUser = c.Int(nullable: false),
                        TypeRegister = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Users");
        }
    }
}
