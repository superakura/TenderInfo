namespace TenderInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit25 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CheckReportFile",
                c => new
                    {
                        CheckReportFileID = c.Int(nullable: false, identity: true),
                        SampleDelegationID = c.Int(nullable: false),
                        CheckReportFileName = c.String(maxLength: 500),
                        CheckReportInputPerson = c.Int(nullable: false),
                        CheckReportInputPersonName = c.String(maxLength: 100),
                        CheckReportInputDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.CheckReportFileID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CheckReportFile");
        }
    }
}
