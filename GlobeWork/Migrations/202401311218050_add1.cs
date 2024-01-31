namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Employers", "Rank_Id", "dbo.Ranks");
            DropIndex("dbo.Employers", new[] { "Rank_Id" });
            RenameColumn(table: "dbo.Employers", name: "Rank_Id", newName: "RankId");
            AlterColumn("dbo.Employers", "RankId", c => c.Int(nullable: false));
            CreateIndex("dbo.Employers", "RankId");
            AddForeignKey("dbo.Employers", "RankId", "dbo.Ranks", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Employers", "RankId", "dbo.Ranks");
            DropIndex("dbo.Employers", new[] { "RankId" });
            AlterColumn("dbo.Employers", "RankId", c => c.Int());
            RenameColumn(table: "dbo.Employers", name: "RankId", newName: "Rank_Id");
            CreateIndex("dbo.Employers", "Rank_Id");
            AddForeignKey("dbo.Employers", "Rank_Id", "dbo.Ranks", "Id");
        }
    }
}
