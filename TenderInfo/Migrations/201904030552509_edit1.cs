namespace TenderInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccountApprove",
                c => new
                    {
                        AccountApproveID = c.Int(nullable: false, identity: true),
                        AccountID = c.Int(nullable: false),
                        ApproveState = c.String(maxLength: 50),
                        ApproveTime = c.DateTime(nullable: false),
                        ApprovePersonID = c.Int(nullable: false),
                        ApprovePersonName = c.String(maxLength: 50),
                        ApproveBackReason = c.String(maxLength: 50),
                        SubmitPersonID = c.Int(nullable: false),
                        SubmitPersonName = c.String(maxLength: 50),
                        SubmitTime = c.DateTime(nullable: false),
                        SubmitEditReason = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.AccountApproveID);
            
            AddColumn("dbo.Account", "IsComplete", c => c.String(maxLength: 50));
            AddColumn("dbo.AccountChild", "UsingDeptName", c => c.String(maxLength: 200));
            AddColumn("dbo.AccountChild", "UsingDeptID", c => c.Int(nullable: false));
            AddColumn("dbo.AccountChild", "VetoReason", c => c.String(maxLength: 500));
            AddColumn("dbo.AccountChild", "TenderPersonVersion", c => c.String(maxLength: 500));
            AddColumn("dbo.AccountChild", "EvaluationVersion", c => c.String(maxLength: 500));
            AddColumn("dbo.AccountChild", "ClarifyFile", c => c.String(maxLength: 500));
            AddColumn("dbo.AccountChild", "DissentProposedStage", c => c.String(maxLength: 500));
            AddColumn("dbo.AccountChild", "DissentFile", c => c.String(maxLength: 500));
            AddColumn("dbo.AccountChild", "ConnectPerson", c => c.String(maxLength: 500));
            AddColumn("dbo.AccountChild", "ConnectDateTime", c => c.DateTime());
            AddColumn("dbo.AccountChild", "ConnectContent", c => c.String(maxLength: 500));
            AddColumn("dbo.AccountChild", "ConnectExistingProblems", c => c.String(maxLength: 500));
            AddColumn("dbo.AccountChild", "TenderSuccessPerson", c => c.String(maxLength: 200));
            AddColumn("dbo.AccountChild", "TenderSuccessPersonStartDate", c => c.DateTime());
            AddColumn("dbo.AccountChild", "TenderSuccessPersonEndDate", c => c.DateTime());
            AddColumn("dbo.AccountChild", "TenderSuccessPersonVersion", c => c.String(maxLength: 200));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AccountChild", "TenderSuccessPersonVersion");
            DropColumn("dbo.AccountChild", "TenderSuccessPersonEndDate");
            DropColumn("dbo.AccountChild", "TenderSuccessPersonStartDate");
            DropColumn("dbo.AccountChild", "TenderSuccessPerson");
            DropColumn("dbo.AccountChild", "ConnectExistingProblems");
            DropColumn("dbo.AccountChild", "ConnectContent");
            DropColumn("dbo.AccountChild", "ConnectDateTime");
            DropColumn("dbo.AccountChild", "ConnectPerson");
            DropColumn("dbo.AccountChild", "DissentFile");
            DropColumn("dbo.AccountChild", "DissentProposedStage");
            DropColumn("dbo.AccountChild", "ClarifyFile");
            DropColumn("dbo.AccountChild", "EvaluationVersion");
            DropColumn("dbo.AccountChild", "TenderPersonVersion");
            DropColumn("dbo.AccountChild", "VetoReason");
            DropColumn("dbo.AccountChild", "UsingDeptID");
            DropColumn("dbo.AccountChild", "UsingDeptName");
            DropColumn("dbo.Account", "IsComplete");
            DropTable("dbo.AccountApprove");
        }
    }
}
