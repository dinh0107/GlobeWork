namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class uodatejobpost : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.JobPosts", name: "Service_Id", newName: "ServiceId");
            RenameIndex(table: "dbo.JobPosts", name: "IX_Service_Id", newName: "IX_ServiceId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.JobPosts", name: "IX_ServiceId", newName: "IX_Service_Id");
            RenameColumn(table: "dbo.JobPosts", name: "ServiceId", newName: "Service_Id");
        }
    }
}
