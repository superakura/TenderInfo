namespace TenderInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit24 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SampleDelegation", "InputPerson", c => c.Int(nullable: false));
            AddColumn("dbo.SampleDelegation", "InputPersonName", c => c.String(maxLength: 100));
            AddColumn("dbo.SampleDelegation", "InputDateTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SampleDelegation", "InputDateTime");
            DropColumn("dbo.SampleDelegation", "InputPersonName");
            DropColumn("dbo.SampleDelegation", "InputPerson");
        }
    }
}
