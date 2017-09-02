namespace BalangaAMS.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedChurchIdAndOtherLocalTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OtherLocalLogs",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ChurchId = c.String(nullable: false),
                        DateTime = c.DateTime(nullable: false),
                        IsLate = c.Boolean(nullable: false),
                        GatheringSession_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GatheringSessions", t => t.GatheringSession_Id, cascadeDelete: true)
                .Index(t => t.GatheringSession_Id);
            
            CreateTable(
                "dbo.ChurchIds",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Code = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.OtherLocalLogs", new[] { "GatheringSession_Id" });
            DropForeignKey("dbo.OtherLocalLogs", "GatheringSession_Id", "dbo.GatheringSessions");
            DropTable("dbo.ChurchIds");
            DropTable("dbo.OtherLocalLogs");
        }
    }
}
