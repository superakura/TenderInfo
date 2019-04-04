namespace TenderInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Account", "TenderSuccessFileDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Account", "TenderSuccessFileDate");
        }
    }
}
