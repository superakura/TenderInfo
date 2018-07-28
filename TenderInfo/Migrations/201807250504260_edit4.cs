namespace TenderInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SampleDelegation", "SampleDelegationFileNameOne", c => c.String(maxLength: 500));
            AddColumn("dbo.SampleDelegation", "SampleDelegationInputPersonOne", c => c.Int(nullable: false));
            AddColumn("dbo.SampleDelegation", "SampleDelegationInputPersonNameOne", c => c.String(maxLength: 100));
            AddColumn("dbo.SampleDelegation", "SampleDelegationInputDateOne", c => c.DateTime());
            AddColumn("dbo.SampleDelegation", "SampleDelegationFileNameTwo", c => c.String(maxLength: 500));
            AddColumn("dbo.SampleDelegation", "SampleDelegationInputPersonTwo", c => c.Int(nullable: false));
            AddColumn("dbo.SampleDelegation", "SampleDelegationInputPersonNameTwo", c => c.String(maxLength: 100));
            AddColumn("dbo.SampleDelegation", "SampleDelegationInputDateTwo", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SampleDelegation", "SampleDelegationInputDateTwo");
            DropColumn("dbo.SampleDelegation", "SampleDelegationInputPersonNameTwo");
            DropColumn("dbo.SampleDelegation", "SampleDelegationInputPersonTwo");
            DropColumn("dbo.SampleDelegation", "SampleDelegationFileNameTwo");
            DropColumn("dbo.SampleDelegation", "SampleDelegationInputDateOne");
            DropColumn("dbo.SampleDelegation", "SampleDelegationInputPersonNameOne");
            DropColumn("dbo.SampleDelegation", "SampleDelegationInputPersonOne");
            DropColumn("dbo.SampleDelegation", "SampleDelegationFileNameOne");
        }
    }
}
