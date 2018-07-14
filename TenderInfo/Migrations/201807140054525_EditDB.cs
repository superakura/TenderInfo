namespace TenderInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EditDB : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AuthorityInfo",
                c => new
                    {
                        AuthorityID = c.Int(nullable: false, identity: true),
                        AuthorityName = c.String(nullable: false, maxLength: 50),
                        AuthorityDescribe = c.String(maxLength: 100),
                        AuthorityType = c.String(nullable: false, maxLength: 50),
                        ConflictCode = c.Int(nullable: false),
                        MenuUrl = c.String(maxLength: 100),
                        MenuOrder = c.Int(nullable: false),
                        MenuFatherID = c.Int(nullable: false),
                        MenuIcon = c.String(maxLength: 50),
                        MenuName = c.String(maxLength: 20),
                    })
                .PrimaryKey(t => t.AuthorityID);
            
            CreateTable(
                "dbo.DeptInfo",
                c => new
                    {
                        DeptID = c.Int(nullable: false, identity: true),
                        DeptName = c.String(nullable: false, maxLength: 50),
                        DeptFatherID = c.Int(nullable: false),
                        DeptState = c.Int(nullable: false),
                        DeptRemark = c.String(maxLength: 200),
                        DeptOrder = c.Int(nullable: false),
                        Open = c.Byte(nullable: false),
                        DeptCreateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.DeptID);
            
            CreateTable(
                "dbo.Log",
                c => new
                    {
                        LogID = c.Int(nullable: false, identity: true),
                        LogType = c.String(nullable: false, maxLength: 100),
                        LogContent = c.String(nullable: false, maxLength: 500),
                        LogDataID = c.Int(nullable: false),
                        LogReason = c.String(maxLength: 500),
                        InputPersonID = c.Int(nullable: false),
                        InputPersonName = c.String(maxLength: 50),
                        InputDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.LogID);
            
            CreateTable(
                "dbo.NoticeInfo",
                c => new
                    {
                        NoticeID = c.Int(nullable: false, identity: true),
                        NoticeTitle = c.String(nullable: false, maxLength: 200),
                        Content = c.String(nullable: false, maxLength: 1000),
                        ContentType = c.String(maxLength: 50),
                        ContentCount = c.Int(nullable: false),
                        InsertPersonID = c.Int(nullable: false),
                        InsertDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.NoticeID)
                .ForeignKey("dbo.UserInfo", t => t.InsertPersonID, cascadeDelete: true)
                .Index(t => t.InsertPersonID);
            
            CreateTable(
                "dbo.UserInfo",
                c => new
                    {
                        UserID = c.Int(nullable: false, identity: true),
                        UserNum = c.String(nullable: false, maxLength: 20),
                        UserName = c.String(nullable: false, maxLength: 20),
                        UserPassword = c.String(maxLength: 100),
                        UserPhone = c.String(maxLength: 50),
                        UserMobile = c.String(maxLength: 50),
                        UserDuty = c.String(maxLength: 100),
                        UserEmail = c.String(),
                        UserRemark = c.String(maxLength: 200),
                        UserState = c.Int(nullable: false),
                        UserDeptID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserID)
                .ForeignKey("dbo.DeptInfo", t => t.UserDeptID, cascadeDelete: true)
                .Index(t => t.UserDeptID);
            
            CreateTable(
                "dbo.RoleAuthority",
                c => new
                    {
                        RoleAuthorityID = c.Int(nullable: false, identity: true),
                        RoleID = c.Int(nullable: false),
                        AuthorityID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RoleAuthorityID)
                .ForeignKey("dbo.AuthorityInfo", t => t.AuthorityID, cascadeDelete: true)
                .ForeignKey("dbo.RoleInfo", t => t.RoleID, cascadeDelete: true)
                .Index(t => t.RoleID)
                .Index(t => t.AuthorityID);
            
            CreateTable(
                "dbo.RoleInfo",
                c => new
                    {
                        RoleID = c.Int(nullable: false, identity: true),
                        RoleName = c.String(nullable: false, maxLength: 50),
                        RoleDescribe = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.RoleID);
            
            CreateTable(
                "dbo.UserDept",
                c => new
                    {
                        UserDeptID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(nullable: false),
                        DeptID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserDeptID)
                .ForeignKey("dbo.DeptInfo", t => t.DeptID, cascadeDelete: true)
                .Index(t => t.DeptID);
            
            CreateTable(
                "dbo.UserRole",
                c => new
                    {
                        UserRoleID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(nullable: false),
                        RoleID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserRoleID)
                .ForeignKey("dbo.RoleInfo", t => t.RoleID, cascadeDelete: true)
                .ForeignKey("dbo.UserInfo", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID)
                .Index(t => t.RoleID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserRole", "UserID", "dbo.UserInfo");
            DropForeignKey("dbo.UserRole", "RoleID", "dbo.RoleInfo");
            DropForeignKey("dbo.UserDept", "DeptID", "dbo.DeptInfo");
            DropForeignKey("dbo.RoleAuthority", "RoleID", "dbo.RoleInfo");
            DropForeignKey("dbo.RoleAuthority", "AuthorityID", "dbo.AuthorityInfo");
            DropForeignKey("dbo.NoticeInfo", "InsertPersonID", "dbo.UserInfo");
            DropForeignKey("dbo.UserInfo", "UserDeptID", "dbo.DeptInfo");
            DropIndex("dbo.UserRole", new[] { "RoleID" });
            DropIndex("dbo.UserRole", new[] { "UserID" });
            DropIndex("dbo.UserDept", new[] { "DeptID" });
            DropIndex("dbo.RoleAuthority", new[] { "AuthorityID" });
            DropIndex("dbo.RoleAuthority", new[] { "RoleID" });
            DropIndex("dbo.UserInfo", new[] { "UserDeptID" });
            DropIndex("dbo.NoticeInfo", new[] { "InsertPersonID" });
            DropTable("dbo.UserRole");
            DropTable("dbo.UserDept");
            DropTable("dbo.RoleInfo");
            DropTable("dbo.RoleAuthority");
            DropTable("dbo.UserInfo");
            DropTable("dbo.NoticeInfo");
            DropTable("dbo.Log");
            DropTable("dbo.DeptInfo");
            DropTable("dbo.AuthorityInfo");
        }
    }
}
