namespace TenderInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit11 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.SampleDelegation", "SampleDelegationState", c => c.String(maxLength: 200));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SampleDelegation", "SampleDelegationState", c => c.String());
        }
    }
}
