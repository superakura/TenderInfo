namespace TenderInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit23 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SampleDelegation", "SampleNum", c => c.Int());
            AddColumn("dbo.SampleDelegation", "SampleTechnicalRequirement", c => c.String(maxLength: 2000));
            DropColumn("dbo.SampleDelegation", "ProjectName");
            DropColumn("dbo.SampleDelegation", "SampleDelegationFileName");
            DropColumn("dbo.SampleDelegation", "SampleDelegationInputPerson");
            DropColumn("dbo.SampleDelegation", "SampleDelegationInputPersonName");
            DropColumn("dbo.SampleDelegation", "SampleDelegationInputDate");
            DropColumn("dbo.SampleDelegation", "SampleDelegationFileNameOne");
            DropColumn("dbo.SampleDelegation", "SampleDelegationInputPersonOne");
            DropColumn("dbo.SampleDelegation", "SampleDelegationInputPersonNameOne");
            DropColumn("dbo.SampleDelegation", "SampleDelegationInputDateOne");
            DropColumn("dbo.SampleDelegation", "CheckReportFileName");
            DropColumn("dbo.SampleDelegation", "CheckReportInputPerson");
            DropColumn("dbo.SampleDelegation", "CheckReportInputPersonName");
            DropColumn("dbo.SampleDelegation", "CheckReportInputDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SampleDelegation", "CheckReportInputDate", c => c.DateTime());
            AddColumn("dbo.SampleDelegation", "CheckReportInputPersonName", c => c.String(maxLength: 100));
            AddColumn("dbo.SampleDelegation", "CheckReportInputPerson", c => c.Int(nullable: false));
            AddColumn("dbo.SampleDelegation", "CheckReportFileName", c => c.String(maxLength: 500));
            AddColumn("dbo.SampleDelegation", "SampleDelegationInputDateOne", c => c.DateTime());
            AddColumn("dbo.SampleDelegation", "SampleDelegationInputPersonNameOne", c => c.String(maxLength: 100));
            AddColumn("dbo.SampleDelegation", "SampleDelegationInputPersonOne", c => c.Int(nullable: false));
            AddColumn("dbo.SampleDelegation", "SampleDelegationFileNameOne", c => c.String(maxLength: 500));
            AddColumn("dbo.SampleDelegation", "SampleDelegationInputDate", c => c.DateTime());
            AddColumn("dbo.SampleDelegation", "SampleDelegationInputPersonName", c => c.String(maxLength: 100));
            AddColumn("dbo.SampleDelegation", "SampleDelegationInputPerson", c => c.Int(nullable: false));
            AddColumn("dbo.SampleDelegation", "SampleDelegationFileName", c => c.String(maxLength: 500));
            AddColumn("dbo.SampleDelegation", "ProjectName", c => c.String(maxLength: 200));
            DropColumn("dbo.SampleDelegation", "SampleTechnicalRequirement");
            DropColumn("dbo.SampleDelegation", "SampleNum");
        }
    }
}
