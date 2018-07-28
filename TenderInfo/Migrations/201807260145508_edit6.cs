namespace TenderInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit6 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SampleDelegation", "SampleDelegationState", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SampleDelegation", "SampleDelegationState");
        }
    }
}
