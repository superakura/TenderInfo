namespace TenderInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit18 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Account", "PlanInvestPrice", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Account", "PlanInvestPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}