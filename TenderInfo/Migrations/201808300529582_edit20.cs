namespace TenderInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit20 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProgressInfo", "ProgressState", c => c.String(maxLength: 100));
            AlterColumn("dbo.ProgressInfo", "InvestPrice", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ProgressInfo", "InvestPrice", c => c.String(maxLength: 200));
            DropColumn("dbo.ProgressInfo", "ProgressState");
        }
    }
}
