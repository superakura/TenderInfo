namespace TenderInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit36 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FileComprehensiveChild", "ApproveLevel", c => c.String(nullable: false, maxLength: 200));
            AddColumn("dbo.FileComprehensiveChild", "ApproveState", c => c.String(nullable: false, maxLength: 200));
            AddColumn("dbo.FileComprehensiveChild", "ApprovePersonID", c => c.Int(nullable: false));
            AddColumn("dbo.FileComprehensiveChild", "ApprovePersonName", c => c.String(maxLength: 100));
            AddColumn("dbo.FileComprehensiveChild", "ApprovePersonDeptID", c => c.Int(nullable: false));
            AddColumn("dbo.FileComprehensiveChild", "ApprovePersonDeptName", c => c.String(maxLength: 100));
            AddColumn("dbo.FileComprehensiveChild", "ApprovePersonFatherDeptID", c => c.Int(nullable: false));
            AddColumn("dbo.FileComprehensiveChild", "ApprovePersonFatherDeptName", c => c.String(maxLength: 100));
            AddColumn("dbo.FileComprehensiveChild", "ApproveDateTime", c => c.DateTime());
            AddColumn("dbo.FileComprehensiveChild", "ApproveBackReason", c => c.String(maxLength: 100));
            DropColumn("dbo.FileComprehensiveChild", "SpecificationApproveLevel");
            DropColumn("dbo.FileComprehensiveChild", "SpecificationApproveState");
            DropColumn("dbo.FileComprehensiveChild", "SpecificationApprovePersonID");
            DropColumn("dbo.FileComprehensiveChild", "SpecificationApprovePersonName");
            DropColumn("dbo.FileComprehensiveChild", "SpecificationApprovePersonDeptID");
            DropColumn("dbo.FileComprehensiveChild", "SpecificationApprovePersonDeptName");
            DropColumn("dbo.FileComprehensiveChild", "SpecificationApprovePersonFatherDeptID");
            DropColumn("dbo.FileComprehensiveChild", "SpecificationApprovePersonFatherDeptName");
            DropColumn("dbo.FileComprehensiveChild", "SpecificationApproveDateTime");
            DropColumn("dbo.FileComprehensiveChild", "SpecificationApproveBackReason");
            DropColumn("dbo.FileComprehensiveChild", "TechnologyApproveLevel");
            DropColumn("dbo.FileComprehensiveChild", "TechnologyApproveState");
            DropColumn("dbo.FileComprehensiveChild", "TechnologyApprovePersonID");
            DropColumn("dbo.FileComprehensiveChild", "TechnologyApprovePersonName");
            DropColumn("dbo.FileComprehensiveChild", "TechnologyApprovePersonDeptID");
            DropColumn("dbo.FileComprehensiveChild", "TechnologyApprovePersonDeptName");
            DropColumn("dbo.FileComprehensiveChild", "TechnologyApprovePersonFatherDeptID");
            DropColumn("dbo.FileComprehensiveChild", "TechnologyApprovePersonFatherDeptName");
            DropColumn("dbo.FileComprehensiveChild", "TechnologyApproveDateTime");
            DropColumn("dbo.FileComprehensiveChild", "TechnologyApproveBackReason");
            DropColumn("dbo.FileComprehensiveChild", "BusinessApproveLevel");
            DropColumn("dbo.FileComprehensiveChild", "BusinessApproveState");
            DropColumn("dbo.FileComprehensiveChild", "BusinessApprovePersonID");
            DropColumn("dbo.FileComprehensiveChild", "BusinessApprovePersonName");
            DropColumn("dbo.FileComprehensiveChild", "BusinessApprovePersonDeptID");
            DropColumn("dbo.FileComprehensiveChild", "BusinessApprovePersonDeptName");
            DropColumn("dbo.FileComprehensiveChild", "BusinessApprovePersonFatherDeptID");
            DropColumn("dbo.FileComprehensiveChild", "BusinessApprovePersonFatherDeptName");
            DropColumn("dbo.FileComprehensiveChild", "BusinessApproveDateTime");
            DropColumn("dbo.FileComprehensiveChild", "BusinessApproveBackReason");
        }
        
        public override void Down()
        {
            AddColumn("dbo.FileComprehensiveChild", "BusinessApproveBackReason", c => c.String(maxLength: 100));
            AddColumn("dbo.FileComprehensiveChild", "BusinessApproveDateTime", c => c.DateTime());
            AddColumn("dbo.FileComprehensiveChild", "BusinessApprovePersonFatherDeptName", c => c.String(maxLength: 100));
            AddColumn("dbo.FileComprehensiveChild", "BusinessApprovePersonFatherDeptID", c => c.Int(nullable: false));
            AddColumn("dbo.FileComprehensiveChild", "BusinessApprovePersonDeptName", c => c.String(maxLength: 100));
            AddColumn("dbo.FileComprehensiveChild", "BusinessApprovePersonDeptID", c => c.Int(nullable: false));
            AddColumn("dbo.FileComprehensiveChild", "BusinessApprovePersonName", c => c.String(maxLength: 100));
            AddColumn("dbo.FileComprehensiveChild", "BusinessApprovePersonID", c => c.Int(nullable: false));
            AddColumn("dbo.FileComprehensiveChild", "BusinessApproveState", c => c.String(maxLength: 200));
            AddColumn("dbo.FileComprehensiveChild", "BusinessApproveLevel", c => c.String(maxLength: 200));
            AddColumn("dbo.FileComprehensiveChild", "TechnologyApproveBackReason", c => c.String(maxLength: 100));
            AddColumn("dbo.FileComprehensiveChild", "TechnologyApproveDateTime", c => c.DateTime());
            AddColumn("dbo.FileComprehensiveChild", "TechnologyApprovePersonFatherDeptName", c => c.String(maxLength: 100));
            AddColumn("dbo.FileComprehensiveChild", "TechnologyApprovePersonFatherDeptID", c => c.Int(nullable: false));
            AddColumn("dbo.FileComprehensiveChild", "TechnologyApprovePersonDeptName", c => c.String(maxLength: 100));
            AddColumn("dbo.FileComprehensiveChild", "TechnologyApprovePersonDeptID", c => c.Int(nullable: false));
            AddColumn("dbo.FileComprehensiveChild", "TechnologyApprovePersonName", c => c.String(maxLength: 100));
            AddColumn("dbo.FileComprehensiveChild", "TechnologyApprovePersonID", c => c.Int(nullable: false));
            AddColumn("dbo.FileComprehensiveChild", "TechnologyApproveState", c => c.String(maxLength: 200));
            AddColumn("dbo.FileComprehensiveChild", "TechnologyApproveLevel", c => c.String(maxLength: 200));
            AddColumn("dbo.FileComprehensiveChild", "SpecificationApproveBackReason", c => c.String(maxLength: 100));
            AddColumn("dbo.FileComprehensiveChild", "SpecificationApproveDateTime", c => c.DateTime());
            AddColumn("dbo.FileComprehensiveChild", "SpecificationApprovePersonFatherDeptName", c => c.String(maxLength: 100));
            AddColumn("dbo.FileComprehensiveChild", "SpecificationApprovePersonFatherDeptID", c => c.Int(nullable: false));
            AddColumn("dbo.FileComprehensiveChild", "SpecificationApprovePersonDeptName", c => c.String(maxLength: 100));
            AddColumn("dbo.FileComprehensiveChild", "SpecificationApprovePersonDeptID", c => c.Int(nullable: false));
            AddColumn("dbo.FileComprehensiveChild", "SpecificationApprovePersonName", c => c.String(maxLength: 100));
            AddColumn("dbo.FileComprehensiveChild", "SpecificationApprovePersonID", c => c.Int(nullable: false));
            AddColumn("dbo.FileComprehensiveChild", "SpecificationApproveState", c => c.String(maxLength: 200));
            AddColumn("dbo.FileComprehensiveChild", "SpecificationApproveLevel", c => c.String(maxLength: 200));
            DropColumn("dbo.FileComprehensiveChild", "ApproveBackReason");
            DropColumn("dbo.FileComprehensiveChild", "ApproveDateTime");
            DropColumn("dbo.FileComprehensiveChild", "ApprovePersonFatherDeptName");
            DropColumn("dbo.FileComprehensiveChild", "ApprovePersonFatherDeptID");
            DropColumn("dbo.FileComprehensiveChild", "ApprovePersonDeptName");
            DropColumn("dbo.FileComprehensiveChild", "ApprovePersonDeptID");
            DropColumn("dbo.FileComprehensiveChild", "ApprovePersonName");
            DropColumn("dbo.FileComprehensiveChild", "ApprovePersonID");
            DropColumn("dbo.FileComprehensiveChild", "ApproveState");
            DropColumn("dbo.FileComprehensiveChild", "ApproveLevel");
        }
    }
}
