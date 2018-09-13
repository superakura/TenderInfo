namespace TenderInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit27 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FileMinPrice",
                c => new
                    {
                        FileMinPriceID = c.Int(nullable: false, identity: true),
                        TechnicSpecificationFile = c.String(nullable: false, maxLength: 200),
                        ApproveLevel = c.String(nullable: false, maxLength: 200),
                        ApproveState = c.String(nullable: false, maxLength: 200),
                        InputPersonID = c.Int(nullable: false),
                        InputPersonName = c.String(maxLength: 100),
                        InputPersonDeptID = c.Int(nullable: false),
                        InputPersonDeptName = c.String(maxLength: 100),
                        InputDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.FileMinPriceID);
            
            CreateTable(
                "dbo.FileMinPriceChild",
                c => new
                    {
                        FileMinPriceChildID = c.Int(nullable: false, identity: true),
                        FileMinPriceID = c.Int(nullable: false),
                        ApproveLevel = c.String(nullable: false, maxLength: 200),
                        ApproveState = c.String(nullable: false, maxLength: 200),
                        ApprovePersonID = c.Int(nullable: false),
                        ApprovePersonName = c.String(maxLength: 100),
                        ApprovePersonDeptID = c.Int(nullable: false),
                        ApprovePersonDeptName = c.String(maxLength: 100),
                        ApproveDateTime = c.DateTime(nullable: false),
                        ApproveBackReason = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.FileMinPriceChildID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.FileMinPriceChild");
            DropTable("dbo.FileMinPrice");
        }
    }
}
