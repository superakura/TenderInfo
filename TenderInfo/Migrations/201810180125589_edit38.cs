namespace TenderInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit38 : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.AccountMaterial");
            DropTable("dbo.AccountMaterialChild");
            DropTable("dbo.AccountProject");
            DropTable("dbo.AccountProjectChild");
            DropTable("dbo.ProgressMaterial");
            DropTable("dbo.ProgressProject");
        }
        
        public override void Down()
        {
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
                        IsOver = c.String(maxLength: 100),
                        YearInfo = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.ProgressProjectID);
            
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
                        IsOver = c.String(maxLength: 100),
                        YearInfo = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.ProgressMaterialID);
            
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
                "dbo.AccountProject",
                c => new
                    {
                        AccountProjectID = c.Int(nullable: false, identity: true),
                        ProjectName = c.String(nullable: false, maxLength: 200),
                        ProjectType = c.String(maxLength: 50),
                        TenderFileNum = c.String(maxLength: 200),
                        IsOnline = c.String(maxLength: 200),
                        ProjectResponsiblePersonName = c.String(maxLength: 200),
                        ProjectResponsiblePersonID = c.Int(nullable: false),
                        UsingDeptName = c.String(maxLength: 200),
                        UsingDeptID = c.Int(nullable: false),
                        ProjectResponsibleDeptName = c.String(maxLength: 200),
                        ProjectResponsibleDeptID = c.Int(nullable: false),
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
                        InputDate = c.DateTime(),
                        InputPersonID = c.Int(nullable: false),
                        InsertDate = c.DateTime(),
                        InsertPersonID = c.Int(nullable: false),
                        ProgressID = c.Int(nullable: false),
                        IsSynchro = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.AccountProjectID);
            
            CreateTable(
                "dbo.AccountMaterialChild",
                c => new
                    {
                        AccountMaterialChildID = c.Int(nullable: false, identity: true),
                        AccountMaterialID = c.Int(nullable: false),
                        TableType = c.String(maxLength: 200),
                        TenderFilePlanPayPerson = c.String(maxLength: 200),
                        TenderPerson = c.String(maxLength: 200),
                        ProductManufacturer = c.String(maxLength: 200),
                        QuotedPriceUnit = c.String(maxLength: 200),
                        QuotedPriceSum = c.String(maxLength: 200),
                        EvaluationPersonName = c.String(maxLength: 100),
                        EvaluationPersonDeptName = c.String(maxLength: 200),
                        EvaluationPersonDeptID = c.Int(nullable: false),
                        IsEvaluationDirector = c.String(maxLength: 100),
                        EvaluationTime = c.String(maxLength: 200),
                        EvaluationCost = c.String(maxLength: 200),
                        NegationExplain = c.String(maxLength: 500),
                        TenderFileAuditPersonName = c.String(maxLength: 200),
                        TenderFileAuditPersonDeptName = c.String(maxLength: 200),
                        TenderFileAuditPersonDeptID = c.Int(nullable: false),
                        TenderFileAuditTime = c.String(maxLength: 200),
                        TenderFileAuditCost = c.String(maxLength: 200),
                        InputDate = c.DateTime(nullable: false),
                        InputPerson = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AccountMaterialChildID);
            
            CreateTable(
                "dbo.AccountMaterial",
                c => new
                    {
                        AccountMaterialID = c.Int(nullable: false, identity: true),
                        ProjectName = c.String(nullable: false, maxLength: 200),
                        TenderFileNum = c.String(maxLength: 200),
                        IsOnline = c.String(maxLength: 200),
                        ProjectResponsiblePersonName = c.String(maxLength: 200),
                        ProjectResponsiblePersonID = c.Int(nullable: false),
                        UsingDeptName = c.String(maxLength: 200),
                        UsingDeptID = c.Int(nullable: false),
                        ProjectResponsibleDeptName = c.String(maxLength: 200),
                        ProjectResponsibleDeptID = c.Int(nullable: false),
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
                        InputDate = c.DateTime(),
                        InputPersonID = c.Int(nullable: false),
                        InsertDate = c.DateTime(),
                        InsertPersonID = c.Int(nullable: false),
                        ProgressID = c.Int(nullable: false),
                        IsSynchro = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.AccountMaterialID);
            
        }
    }
}
