namespace TenderInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit22 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Account", "ProgressID", c => c.Int());
            AlterColumn("dbo.ProgressInfo", "AccountID", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ProgressInfo", "AccountID", c => c.Int(nullable: false));
            AlterColumn("dbo.Account", "ProgressID", c => c.Int(nullable: false));
        }
    }
}
