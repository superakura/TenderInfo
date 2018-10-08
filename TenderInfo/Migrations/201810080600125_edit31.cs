namespace TenderInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit31 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FileComprehensive",
                c => new
                    {
                        FileComprehensiveID = c.Int(nullable: false, identity: true),
                        TechnicSpecificationFileMerge = c.String(nullable: false, maxLength: 200),
                        TechnicSpecificationFile = c.String(nullable: false, maxLength: 200),
                        TechnologyScoreStandardFile = c.String(nullable: false, maxLength: 200),
                        BusinessScoreStandardFile = c.String(nullable: false, maxLength: 200),
                        ApproveLevelSpecification = c.String(nullable: false, maxLength: 200),
                        ApproveStateSpecification = c.String(nullable: false, maxLength: 200),
                        ApproveLevelTechnology = c.String(nullable: false, maxLength: 200),
                        ApproveStateTechnology = c.String(nullable: false, maxLength: 200),
                        ApproveLevelBusiness = c.String(nullable: false, maxLength: 200),
                        ApproveStateBusiness = c.String(nullable: false, maxLength: 200),
                        InputPersonID = c.Int(nullable: false),
                        InputPersonName = c.String(maxLength: 100),
                        InputPersonDeptID = c.Int(nullable: false),
                        InputPersonDeptName = c.String(maxLength: 100),
                        InputPersonFatherDeptID = c.Int(nullable: false),
                        InputPersonFatherDeptName = c.String(maxLength: 100),
                        InputDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.FileComprehensiveID);
            
            CreateTable(
                "dbo.FileComprehensiveChild",
                c => new
                    {
                        FileComprehensiveChildID = c.Int(nullable: false, identity: true),
                        FileComprehensiveID = c.Int(nullable: false),
                        SpecificationApproveLevel = c.String(nullable: false, maxLength: 200),
                        SpecificationApproveState = c.String(nullable: false, maxLength: 200),
                        SpecificationApprovePersonID = c.Int(nullable: false),
                        SpecificationApprovePersonName = c.String(maxLength: 100),
                        SpecificationApprovePersonDeptID = c.Int(nullable: false),
                        SpecificationApprovePersonDeptName = c.String(maxLength: 100),
                        SpecificationApprovePersonFatherDeptID = c.Int(nullable: false),
                        SpecificationApprovePersonFatherDeptName = c.String(maxLength: 100),
                        SpecificationApproveDateTime = c.DateTime(),
                        SpecificationApproveBackReason = c.String(maxLength: 100),
                        TechnologyApproveLevel = c.String(nullable: false, maxLength: 200),
                        TechnologyApproveState = c.String(nullable: false, maxLength: 200),
                        TechnologyApprovePersonID = c.Int(nullable: false),
                        TechnologyApprovePersonName = c.String(maxLength: 100),
                        TechnologyApprovePersonDeptID = c.Int(nullable: false),
                        TechnologyApprovePersonDeptName = c.String(maxLength: 100),
                        TechnologyApprovePersonFatherDeptID = c.Int(nullable: false),
                        TechnologyApprovePersonFatherDeptName = c.String(maxLength: 100),
                        TechnologyApproveDateTime = c.DateTime(),
                        TechnologyApproveBackReason = c.String(maxLength: 100),
                        BusinessApproveLevel = c.String(nullable: false, maxLength: 200),
                        BusinessApproveState = c.String(nullable: false, maxLength: 200),
                        BusinessApprovePersonID = c.Int(nullable: false),
                        BusinessApprovePersonName = c.String(maxLength: 100),
                        BusinessApprovePersonDeptID = c.Int(nullable: false),
                        BusinessApprovePersonDeptName = c.String(maxLength: 100),
                        BusinessApprovePersonFatherDeptID = c.Int(nullable: false),
                        BusinessApprovePersonFatherDeptName = c.String(maxLength: 100),
                        BusinessApproveDateTime = c.DateTime(),
                        BusinessApproveBackReason = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.FileComprehensiveChildID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.FileComprehensiveChild");
            DropTable("dbo.FileComprehensive");
        }
    }
}
