namespace TenderInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit10 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Log", "InputPersonNameID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Log", "InputPersonNameID", c => c.Int(nullable: false));
        }
    }
}
