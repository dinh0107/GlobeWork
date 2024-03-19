namespace GlobeWork.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcountruycompany : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companies", "CountryId", c => c.Int());
            CreateIndex("dbo.Companies", "CountryId");
            AddForeignKey("dbo.Companies", "CountryId", "dbo.Countries", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Companies", "CountryId", "dbo.Countries");
            DropIndex("dbo.Companies", new[] { "CountryId" });
            DropColumn("dbo.Companies", "CountryId");
        }
    }
}
