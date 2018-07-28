namespace TenderInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccountMaterial",
                c => new
                    {
                        AccountMaterialID = c.Int(nullable: false, identity: true),
                        ProjectName = c.String(nullable: false, maxLength: 200),
                        TenderFileNum = c.String(maxLength: 200),
                        IsOnline = c.String(maxLength: 200),
                        ProjectResponsiblePerson = c.String(maxLength: 200),
                        UsingDept = c.String(maxLength: 200),
                        ProjectResponsibleDept = c.String(maxLength: 200),
                        ApplyPerson = c.String(maxLength: 200),
                        InvestPlanApproveNum = c.String(maxLength: 200),
                        TenderRange = c.String(maxLength: 200),
                        TenderMode = c.String(maxLength: 200),
                        BidEvaluation = c.String(maxLength: 200),
                        SupplyPeriod = c.String(maxLength: 200),
                        TenderProgramAuditDate = c.DateTime(),
                        ProgramAcceptDate = c.DateTime(),
                        TenderFileSaleStartDate = c.DateTime(),
                        TenderFileSaleEndDate = c.DateTime(),
                        TenderStartDate = c.DateTime(),
                        TenderSuccessFileDate = c.DateTime(),
                        TenderSuccessPerson = c.String(maxLength: 200),
                        PlanInvestPrice = c.String(maxLength: 200),
                        TenderRestrictUnitPrice = c.String(maxLength: 200),
                        TenderSuccessUnitPrice = c.String(maxLength: 200),
                        TenderSuccessSumPrice = c.String(maxLength: 200),
                        SaveCapital = c.String(maxLength: 200),
                        EvaluationTime = c.String(maxLength: 200),
                        TenderFileAuditTime = c.String(maxLength: 200),
                        TenderFailReason = c.String(maxLength: 500),
                        ClarifyLaunchPerson = c.String(maxLength: 500),
                        ClarifyLaunchDate = c.DateTime(),
                        ClarifyReason = c.String(maxLength: 500),
                        ClarifyAcceptDate = c.DateTime(),
                        ClarifyDisposePerson = c.String(maxLength: 100),
                        IsClarify = c.String(maxLength: 100),
                        ClarifyDisposeInfo = c.String(maxLength: 500),
                        ClarifyReplyDate = c.DateTime(),
                        DissentLaunchPerson = c.String(maxLength: 100),
                        DissentLaunchPersonPhone = c.String(maxLength: 100),
                        DissentLaunchDate = c.DateTime(),
                        DissentReason = c.String(maxLength: 500),
                        DissentAcceptDate = c.DateTime(),
                        DissentAcceptPerson = c.String(maxLength: 100),
                        DissentDisposePerson = c.String(maxLength: 100),
                        DissentDisposeInfo = c.String(maxLength: 500),
                        DissentReplyDate = c.DateTime(),
                        ContractNum = c.String(maxLength: 100),
                        ContractPrice = c.String(maxLength: 100),
                        RelativePerson = c.String(maxLength: 100),
                        TenderInfo = c.String(maxLength: 500),
                        TenderRemark = c.String(maxLength: 500),
                        InputDate = c.DateTime(nullable: false),
                        InputPerson = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AccountMaterialID);
            
            CreateTable(
                "dbo.AccountMaterialChild",
                c => new
                    {
                        AccountMaterialChildID = c.Int(nullable: false, identity: true),
                        TenderFilePlanPayPerson = c.String(maxLength: 200),
                        TenderPerson = c.String(maxLength: 200),
                        ProductManufacturer = c.String(maxLength: 200),
                        QuotedPriceUnit = c.String(maxLength: 200),
                        QuotedPriceSum = c.String(maxLength: 200),
                        EvaluationPersonName = c.String(maxLength: 100),
                        EvaluationPersonDept = c.String(maxLength: 200),
                        IsEvaluationDirector = c.String(maxLength: 100),
                        EvaluationCost = c.String(maxLength: 200),
                        TenderFileAuditPersonName = c.String(maxLength: 200),
                        TenderFileAuditPersonDept = c.String(maxLength: 200),
                        TenderFileAuditCost = c.String(maxLength: 200),
                        InputDate = c.DateTime(nullable: false),
                        InputPerson = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AccountMaterialChildID);
            
            CreateTable(
                "dbo.AccountProject",
                c => new
                    {
                        AccountProjectID = c.Int(nullable: false, identity: true),
                        ProjectName = c.String(nullable: false, maxLength: 200),
                        TenderFileNum = c.String(maxLength: 200),
                        IsOnline = c.String(maxLength: 200),
                        ProjectResponsiblePerson = c.String(maxLength: 200),
                        ProjectResponsibleDept = c.String(maxLength: 200),
                        ApplyPerson = c.String(maxLength: 200),
                        InvestPlanApproveNum = c.String(maxLength: 200),
                        InvestSource = c.String(maxLength: 200),
                        TenderRange = c.String(maxLength: 200),
                        ProjectTimeLimit = c.String(maxLength: 200),
                        ProgramAcceptDate = c.DateTime(),
                        TenderProgramAuditDate = c.DateTime(),
                        TenderFileSaleStartDate = c.DateTime(),
                        TenderFileSaleEndDate = c.DateTime(),
                        TenderStartDate = c.DateTime(),
                        TenderSuccessPerson = c.String(maxLength: 200),
                        PlanInvestPrice = c.String(maxLength: 200),
                        TenderRestrictUnitPrice = c.String(maxLength: 200),
                        TenderSuccessPrice = c.String(maxLength: 200),
                        SaveCapital = c.String(maxLength: 200),
                        EvaluationTime = c.String(maxLength: 200),
                        TenderFileAuditTime = c.String(maxLength: 200),
                        TenderFailReason = c.String(maxLength: 500),
                        ClarifyLaunchPerson = c.String(maxLength: 500),
                        ClarifyLaunchDate = c.DateTime(),
                        ClarifyReason = c.String(maxLength: 500),
                        ClarifyAcceptDate = c.DateTime(),
                        ClarifyDisposePerson = c.String(maxLength: 100),
                        IsClarify = c.String(maxLength: 100),
                        ClarifyDisposeInfo = c.String(maxLength: 500),
                        ClarifyReplyDate = c.DateTime(),
                        DissentLaunchPerson = c.String(maxLength: 100),
                        DissentLaunchPersonPhone = c.String(maxLength: 100),
                        DissentLaunchDate = c.DateTime(),
                        DissentReason = c.String(maxLength: 500),
                        DissentAcceptDate = c.DateTime(),
                        DissentAcceptPerson = c.String(maxLength: 100),
                        DissentDisposePerson = c.String(maxLength: 100),
                        DissentDisposeInfo = c.String(maxLength: 500),
                        DissentReplyDate = c.DateTime(),
                        ContractNum = c.String(maxLength: 100),
                        ContractPrice = c.String(maxLength: 100),
                        RelativePerson = c.String(maxLength: 100),
                        TenderInfo = c.String(maxLength: 500),
                        TenderRemark = c.String(maxLength: 500),
                        InputDate = c.DateTime(nullable: false),
                        InputPerson = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AccountProjectID);
            
            CreateTable(
                "dbo.AccountProjectChild",
                c => new
                    {
                        AccountProjectChildID = c.Int(nullable: false, identity: true),
                        TenderFilePlanPayPerson = c.String(maxLength: 200),
                        TenderPerson = c.String(maxLength: 200),
                        QuotedPrice = c.String(maxLength: 200),
                        EvaluationPersonName = c.String(maxLength: 100),
                        EvaluationPersonDept = c.String(maxLength: 200),
                        IsEvaluationDirector = c.String(maxLength: 100),
                        EvaluationCost = c.String(maxLength: 200),
                        TenderFileAuditPersonName = c.String(maxLength: 200),
                        TenderFileAuditPersonDept = c.String(maxLength: 200),
                        TenderFileAuditCost = c.String(maxLength: 200),
                        InputDate = c.DateTime(nullable: false),
                        InputPerson = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AccountProjectChildID);
            
            CreateTable(
                "dbo.ProgressMaterial",
                c => new
                    {
                        ProgressMaterialID = c.Int(nullable: false, identity: true),
                        ProgressType = c.String(maxLength: 100),
                        ProgressTypeChild = c.String(maxLength: 100),
                        ProjectName = c.String(maxLength: 200),
                        InvestPrice = c.String(maxLength: 200),
                        MaterialCount = c.String(maxLength: 200),
                        ProjectResponsiblePerson = c.String(maxLength: 200),
                        ContractResponsiblePerson = c.String(maxLength: 100),
                        TechnicalSpecificationAddDate = c.String(maxLength: 500),
                        TechnicalSpecificationExplain = c.String(maxLength: 500),
                        TechnicalSpecificationApproveDate = c.DateTime(),
                        SynthesizeEvaluationRuleApproveDate = c.DateTime(),
                        TenderProgramAuditDate = c.DateTime(),
                        ProgramAcceptDate = c.DateTime(),
                        TenderFileSaleStartDate = c.DateTime(),
                        TenderFileSaleEndDate = c.DateTime(),
                        TenderStartDate = c.DateTime(),
                        TenderSuccessFileDate = c.DateTime(),
                        OtherExplain = c.String(maxLength: 1000),
                        Remark = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.ProgressMaterialID);
            
            CreateTable(
                "dbo.ProgressProject",
                c => new
                    {
                        ProgressProjectID = c.Int(nullable: false, identity: true),
                        ProgressType = c.String(maxLength: 100),
                        ProjectName = c.String(maxLength: 200),
                        InvestPrice = c.String(maxLength: 200),
                        ProjectResponsiblePerson = c.String(maxLength: 200),
                        ContractResponsiblePerson = c.String(maxLength: 100),
                        ContractDeptContactDate = c.String(maxLength: 100),
                        ProjectExplain = c.String(maxLength: 500),
                        ProgramAcceptDate = c.DateTime(),
                        TenderFileSaleStartDate = c.DateTime(),
                        TenderFileSaleEndDate = c.DateTime(),
                        TenderStartDate = c.DateTime(),
                        TenderSuccessFileDate = c.DateTime(),
                        OtherExplain = c.String(maxLength: 1000),
                        Remark = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.ProgressProjectID);
            
            CreateTable(
                "dbo.SampleDelegation",
                c => new
                    {
                        SampleDelegationID = c.Int(nullable: false, identity: true),
                        StartTenderDate = c.DateTime(),
                        SampleDelegationAcceptPerson = c.Int(nullable: false),
                        ProjectResponsiblePerson = c.Int(nullable: false),
                        FirstCodingFileName = c.String(maxLength: 500),
                        FirstCodingInputPerson = c.Int(nullable: false),
                        FirstCodingInputDate = c.DateTime(),
                        SecondCodingFileName = c.String(maxLength: 500),
                        SecondCodingInputPerson = c.Int(nullable: false),
                        SecondCodingInputDate = c.DateTime(),
                        SampleDelegationFileName = c.String(maxLength: 500),
                        SampleDelegationInputPerson = c.Int(nullable: false),
                        SampleDelegationInputDate = c.DateTime(),
                        CheckReportFileName = c.String(maxLength: 500),
                        CheckReportInputPerson = c.Int(nullable: false),
                        CheckReportInputDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.SampleDelegationID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SampleDelegation");
            DropTable("dbo.ProgressProject");
            DropTable("dbo.ProgressMaterial");
            DropTable("dbo.AccountProjectChild");
            DropTable("dbo.AccountProject");
            DropTable("dbo.AccountMaterialChild");
            DropTable("dbo.AccountMaterial");
        }
    }
}
