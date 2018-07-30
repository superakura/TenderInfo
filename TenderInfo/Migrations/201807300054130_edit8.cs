namespace TenderInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit8 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProgressInfo",
                c => new
                    {
                        ProgressInfoID = c.Int(nullable: false, identity: true),
                        ProgressType = c.String(maxLength: 100),
                        ProgressTypeChild = c.String(maxLength: 100),
                        ProjectName = c.String(maxLength: 200),
                        InvestPrice = c.String(maxLength: 200),
                        ProjectResponsiblePerson = c.String(maxLength: 200),
                        ContractResponsiblePerson = c.String(maxLength: 100),
                        MaterialCount = c.String(maxLength: 200),
                        TechnicalSpecificationAddDate = c.String(maxLength: 500),
                        TechnicalSpecificationExplain = c.String(maxLength: 500),
                        TechnicalSpecificationApproveDate = c.DateTime(),
                        SynthesizeEvaluationRuleApproveDate = c.DateTime(),
                        TenderProgramAuditDate = c.DateTime(),
                        ContractDeptContactDate = c.String(maxLength: 100),
                        ProjectExplain = c.String(maxLength: 500),
                        ProgramAcceptDate = c.DateTime(),
                        TenderFileSaleStartDate = c.DateTime(),
                        TenderFileSaleEndDate = c.DateTime(),
                        TenderStartDate = c.DateTime(),
                        TenderSuccessFileDate = c.DateTime(),
                        OtherExplain = c.String(maxLength: 1000),
                        Remark = c.String(maxLength: 1000),
                        IsOver = c.String(maxLength: 100),
                        YearInfo = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.ProgressInfoID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ProgressInfo");
        }
    }
}
