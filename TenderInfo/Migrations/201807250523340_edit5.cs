namespace TenderInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit5 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.SampleDelegation", "SampleDelegationFileNameTwo");
            DropColumn("dbo.SampleDelegation", "SampleDelegationInputPersonTwo");
            DropColumn("dbo.SampleDelegation", "SampleDelegationInputPersonNameTwo");
            DropColumn("dbo.SampleDelegation", "SampleDelegationInputDateTwo");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SampleDelegation", "SampleDelegationInputDateTwo", c => c.DateTime());
            AddColumn("dbo.SampleDelegation", "SampleDelegationInputPersonNameTwo", c => c.String(maxLength: 100));
            AddColumn("dbo.SampleDelegation", "SampleDelegationInputPersonTwo", c => c.Int(nullable: false));
            AddColumn("dbo.SampleDelegation", "SampleDelegationFileNameTwo", c => c.String(maxLength: 500));
        }
    }
}
