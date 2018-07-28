namespace TenderInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SampleDelegation", "ProjectName", c => c.String(maxLength: 200));
            AddColumn("dbo.SampleDelegation", "SampleName", c => c.String(maxLength: 200));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SampleDelegation", "SampleName");
            DropColumn("dbo.SampleDelegation", "ProjectName");
        }
    }
}
