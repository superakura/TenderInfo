namespace TenderInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit21 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProgressInfo", "AccountID", c => c.Int(nullable: false));
            AddColumn("dbo.ProgressInfo", "IsSynchro", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProgressInfo", "IsSynchro");
            DropColumn("dbo.ProgressInfo", "AccountID");
        }
    }
}
