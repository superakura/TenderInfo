namespace TenderInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit26 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SampleDelegation", "ChangeStartTenderDateState", c => c.String(maxLength: 100));
            AddColumn("dbo.SampleDelegation", "CheckResultAllError", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SampleDelegation", "CheckResultAllError");
            DropColumn("dbo.SampleDelegation", "ChangeStartTenderDateState");
        }
    }
}
