namespace TenderInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit7 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserInfo", "UserDeptID", "dbo.DeptInfo");
            DropForeignKey("dbo.UserDept", "DeptID", "dbo.DeptInfo");
            DropIndex("dbo.UserInfo", new[] { "UserDeptID" });
            DropIndex("dbo.UserDept", new[] { "DeptID" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.UserDept", "DeptID");
            CreateIndex("dbo.UserInfo", "UserDeptID");
            AddForeignKey("dbo.UserDept", "DeptID", "dbo.DeptInfo", "DeptID", cascadeDelete: true);
            AddForeignKey("dbo.UserInfo", "UserDeptID", "dbo.DeptInfo", "DeptID", cascadeDelete: true);
        }
    }
}
