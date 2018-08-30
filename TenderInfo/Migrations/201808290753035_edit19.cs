namespace TenderInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit19 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Account", "TenderRestrictUnitPrice", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Account", "TenderRestrictSumPrice", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Account", "TenderSuccessUnitPrice", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Account", "TenderSuccessSumPrice", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Account", "SaveCapital", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Account", "TenderFileAuditTime", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Account", "ContractPrice", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.AccountChild", "QuotedPriceUnit", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.AccountChild", "QuotedPriceSum", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.AccountChild", "EvaluationTime", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.AccountChild", "EvaluationCost", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.AccountChild", "TenderFileAuditCost", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AccountChild", "TenderFileAuditCost", c => c.String(maxLength: 200));
            AlterColumn("dbo.AccountChild", "EvaluationCost", c => c.String(maxLength: 200));
            AlterColumn("dbo.AccountChild", "EvaluationTime", c => c.String(maxLength: 200));
            AlterColumn("dbo.AccountChild", "QuotedPriceSum", c => c.String(maxLength: 200));
            AlterColumn("dbo.AccountChild", "QuotedPriceUnit", c => c.String(maxLength: 200));
            AlterColumn("dbo.Account", "ContractPrice", c => c.String(maxLength: 100));
            AlterColumn("dbo.Account", "TenderFileAuditTime", c => c.String(maxLength: 200));
            AlterColumn("dbo.Account", "SaveCapital", c => c.String(maxLength: 200));
            AlterColumn("dbo.Account", "TenderSuccessSumPrice", c => c.String(maxLength: 200));
            AlterColumn("dbo.Account", "TenderSuccessUnitPrice", c => c.String(maxLength: 200));
            AlterColumn("dbo.Account", "TenderRestrictSumPrice", c => c.String(maxLength: 200));
            AlterColumn("dbo.Account", "TenderRestrictUnitPrice", c => c.String(maxLength: 200));
        }
    }
}
