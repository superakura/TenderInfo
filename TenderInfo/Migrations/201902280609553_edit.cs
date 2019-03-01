namespace TenderInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Account", "TenderRestrictSumPrice", c => c.String(maxLength: 200));
            AlterColumn("dbo.Account", "TenderSuccessSumPrice", c => c.String(maxLength: 200));
            AlterColumn("dbo.AccountChild", "QuotedPriceSum", c => c.String(maxLength: 200));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AccountChild", "QuotedPriceSum", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Account", "TenderSuccessSumPrice", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Account", "TenderRestrictSumPrice", c => c.Decimal(precision: 18, scale: 2));
        }
    }
}
