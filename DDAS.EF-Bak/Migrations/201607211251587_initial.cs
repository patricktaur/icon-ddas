namespace DDAS.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Artists",
                c => new
                    {
                        RecId = c.Long(nullable: false, identity: true),
                        ArtistName = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
                        GenderPId = c.Int(nullable: false),
                        YearOfBirth = c.Int(),
                        YearOfDeath = c.Int(),
                        CreatedOn = c.DateTime(nullable: false, precision: 0),
                        CreatedBy = c.String(unicode: false),
                        UpdatedOn = c.DateTime(nullable: false, precision: 0),
                        UpdatedBy = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.RecId)
                .ForeignKey("dbo.Params", t => t.GenderPId, cascadeDelete: true)
                .Index(t => t.ArtistName, unique: true)
                .Index(t => t.GenderPId);
            
            CreateTable(
                "dbo.Params",
                c => new
                    {
                        RecId = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false, precision: 0),
                        UpdatedOn = c.DateTime(nullable: false, precision: 0),
                        Description = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                        ParId = c.Int(),
                    })
                .PrimaryKey(t => t.RecId)
                .ForeignKey("dbo.Params", t => t.ParId)
                .Index(t => t.ParId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Params", "ParId", "dbo.Params");
            DropForeignKey("dbo.Artists", "GenderPId", "dbo.Params");
            DropIndex("dbo.Params", new[] { "ParId" });
            DropIndex("dbo.Artists", new[] { "GenderPId" });
            DropIndex("dbo.Artists", new[] { "ArtistName" });
            DropTable("dbo.Params");
            DropTable("dbo.Artists");
        }
    }
}
