namespace TenderInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit28 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FileMinPrice", "InputPersonFatherDeptName", c => c.String(maxLength: 100));
            AddColumn("dbo.FileMinPriceChild", "ApprovePersonFatherDeptName", c => c.String(maxLength: 100));
            AlterColumn("dbo.FileMinPriceChild", "ApproveDateTime", c => c.DateTime());
            DropColumn("dbo.FileMinPrice", "InputPersonDeptName");
            DropColumn("dbo.FileMinPriceChild", "ApprovePersonDeptName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.FileMinPriceChild", "ApprovePersonDeptName", c => c.String(maxLength: 100));
            AddColumn("dbo.FileMinPrice", "InputPersonDeptName", c => c.String(maxLength: 100));
            AlterColumn("dbo.FileMinPriceChild", "ApproveDateTime", c => c.DateTime(nullable: false));
            DropColumn("dbo.FileMinPriceChild", "ApprovePersonFatherDeptName");
            DropColumn("dbo.FileMinPrice", "InputPersonFatherDeptName");
        }
    }
}
