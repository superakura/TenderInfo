namespace TenderInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit29 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FileMinPrice", "InputPersonDeptName", c => c.String(maxLength: 100));
            AddColumn("dbo.FileMinPrice", "InputPersonFatherDeptID", c => c.Int(nullable: false));
            AddColumn("dbo.FileMinPriceChild", "ApprovePersonDeptName", c => c.String(maxLength: 100));
            AddColumn("dbo.FileMinPriceChild", "ApprovePersonFatherDeptID", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FileMinPriceChild", "ApprovePersonFatherDeptID");
            DropColumn("dbo.FileMinPriceChild", "ApprovePersonDeptName");
            DropColumn("dbo.FileMinPrice", "InputPersonFatherDeptID");
            DropColumn("dbo.FileMinPrice", "InputPersonDeptName");
        }
    }
}
