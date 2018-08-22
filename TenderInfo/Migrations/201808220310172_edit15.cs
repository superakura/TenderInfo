namespace TenderInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit15 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Account",
                c => new
                    {
                        AccountID = c.Int(nullable: false, identity: true),
                        ProjectType = c.String(maxLength: 50),
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
                        InvestSource = c.String(maxLength: 200),
                        TenderRange = c.String(maxLength: 200),
                        ProjectTimeLimit = c.String(maxLength: 200),
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
                .PrimaryKey(t => t.AccountID);
            
            CreateTable(
                "dbo.AccountChild",
                c => new
                    {
                        AccountChildID = c.Int(nullable: false, identity: true),
                        AccountID = c.Int(nullable: false),
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
                .PrimaryKey(t => t.AccountChildID);
            
            AddColumn("dbo.AccountMaterial", "UsingDeptName", c => c.String(maxLength: 200));
            AddColumn("dbo.AccountMaterial", "UsingDeptID", c => c.Int(nullable: false));
            AddColumn("dbo.AccountMaterial", "ProjectResponsibleDeptName", c => c.String(maxLength: 200));
            AddColumn("dbo.AccountMaterial", "ProjectResponsibleDeptID", c => c.Int(nullable: false));
            AddColumn("dbo.AccountMaterial", "ProgressID", c => c.Int(nullable: false));
            AddColumn("dbo.AccountMaterial", "IsSynchro", c => c.String(maxLength: 50));
            AddColumn("dbo.AccountMaterialChild", "EvaluationPersonDeptName", c => c.String(maxLength: 200));
            AddColumn("dbo.AccountMaterialChild", "EvaluationPersonDeptID", c => c.Int(nullable: false));
            AddColumn("dbo.AccountMaterialChild", "EvaluationTime", c => c.String(maxLength: 200));
            AddColumn("dbo.AccountMaterialChild", "NegationExplain", c => c.String(maxLength: 500));
            AddColumn("dbo.AccountMaterialChild", "TenderFileAuditPersonDeptName", c => c.String(maxLength: 200));
            AddColumn("dbo.AccountMaterialChild", "TenderFileAuditPersonDeptID", c => c.Int(nullable: false));
            AddColumn("dbo.AccountMaterialChild", "TenderFileAuditTime", c => c.String(maxLength: 200));
            AddColumn("dbo.AccountProject", "UsingDeptName", c => c.String(maxLength: 200));
            AddColumn("dbo.AccountProject", "UsingDeptID", c => c.Int(nullable: false));
            AddColumn("dbo.AccountProject", "ProjectResponsibleDeptName", c => c.String(maxLength: 200));
            AddColumn("dbo.AccountProject", "ProjectResponsibleDeptID", c => c.Int(nullable: false));
            AddColumn("dbo.AccountProject", "ProgressID", c => c.Int(nullable: false));
            AddColumn("dbo.AccountProject", "IsSynchro", c => c.String(maxLength: 50));
            DropColumn("dbo.AccountMaterial", "UsingDept");
            DropColumn("dbo.AccountMaterial", "ProjectResponsibleDept");
            DropColumn("dbo.AccountMaterialChild", "EvaluationPersonDept");
            DropColumn("dbo.AccountMaterialChild", "TenderFileAuditPersonDept");
            DropColumn("dbo.AccountProject", "ProjectResponsibleDept");
            DropColumn("dbo.AccountProject", "ApplyPerson");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AccountProject", "ApplyPerson", c => c.String(maxLength: 200));
            AddColumn("dbo.AccountProject", "ProjectResponsibleDept", c => c.String(maxLength: 200));
            AddColumn("dbo.AccountMaterialChild", "TenderFileAuditPersonDept", c => c.String(maxLength: 200));
            AddColumn("dbo.AccountMaterialChild", "EvaluationPersonDept", c => c.String(maxLength: 200));
            AddColumn("dbo.AccountMaterial", "ProjectResponsibleDept", c => c.String(maxLength: 200));
            AddColumn("dbo.AccountMaterial", "UsingDept", c => c.String(maxLength: 200));
            DropColumn("dbo.AccountProject", "IsSynchro");
            DropColumn("dbo.AccountProject", "ProgressID");
            DropColumn("dbo.AccountProject", "ProjectResponsibleDeptID");
            DropColumn("dbo.AccountProject", "ProjectResponsibleDeptName");
            DropColumn("dbo.AccountProject", "UsingDeptID");
            DropColumn("dbo.AccountProject", "UsingDeptName");
            DropColumn("dbo.AccountMaterialChild", "TenderFileAuditTime");
            DropColumn("dbo.AccountMaterialChild", "TenderFileAuditPersonDeptID");
            DropColumn("dbo.AccountMaterialChild", "TenderFileAuditPersonDeptName");
            DropColumn("dbo.AccountMaterialChild", "NegationExplain");
            DropColumn("dbo.AccountMaterialChild", "EvaluationTime");
            DropColumn("dbo.AccountMaterialChild", "EvaluationPersonDeptID");
            DropColumn("dbo.AccountMaterialChild", "EvaluationPersonDeptName");
            DropColumn("dbo.AccountMaterial", "IsSynchro");
            DropColumn("dbo.AccountMaterial", "ProgressID");
            DropColumn("dbo.AccountMaterial", "ProjectResponsibleDeptID");
            DropColumn("dbo.AccountMaterial", "ProjectResponsibleDeptName");
            DropColumn("dbo.AccountMaterial", "UsingDeptID");
            DropColumn("dbo.AccountMaterial", "UsingDeptName");
            DropTable("dbo.AccountChild");
            DropTable("dbo.Account");
        }
    }
}
