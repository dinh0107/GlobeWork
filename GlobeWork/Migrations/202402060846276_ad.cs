namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ad : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.StudyAbroadCategories", "Country_Id", "dbo.Countries");
            DropIndex("dbo.StudyAbroadCategories", new[] { "Country_Id" });
            RenameColumn(table: "dbo.StudyAbroadCategories", name: "Country_Id", newName: "CountryId");
            AlterColumn("dbo.StudyAbroadCategories", "CountryId", c => c.Int(nullable: false));
            CreateIndex("dbo.StudyAbroadCategories", "CountryId");
            AddForeignKey("dbo.StudyAbroadCategories", "CountryId", "dbo.Countries", "Id", cascadeDelete: true);
            DropColumn("dbo.StudyAbroadCategories", "CounId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.StudyAbroadCategories", "CounId", c => c.Int(nullable: false));
            DropForeignKey("dbo.StudyAbroadCategories", "CountryId", "dbo.Countries");
            DropIndex("dbo.StudyAbroadCategories", new[] { "CountryId" });
            AlterColumn("dbo.StudyAbroadCategories", "CountryId", c => c.Int());
            RenameColumn(table: "dbo.StudyAbroadCategories", name: "CountryId", newName: "Country_Id");
            CreateIndex("dbo.StudyAbroadCategories", "Country_Id");
            AddForeignKey("dbo.StudyAbroadCategories", "Country_Id", "dbo.Countries", "Id");
        }
    }
}
