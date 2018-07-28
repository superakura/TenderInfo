namespace TenderInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SampleDelegation", "FirstCodingInputPersonName", c => c.String(maxLength: 100));
            AddColumn("dbo.SampleDelegation", "SecondCodingInputPersonName", c => c.String(maxLength: 100));
            AddColumn("dbo.SampleDelegation", "SampleDelegationInputPersonName", c => c.String(maxLength: 100));
            AddColumn("dbo.SampleDelegation", "CheckReportInputPersonName", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SampleDelegation", "CheckReportInputPersonName");
            DropColumn("dbo.SampleDelegation", "SampleDelegationInputPersonName");
            DropColumn("dbo.SampleDelegation", "SecondCodingInputPersonName");
            DropColumn("dbo.SampleDelegation", "FirstCodingInputPersonName");
        }
    }
}
