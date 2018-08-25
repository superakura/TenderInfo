namespace TenderInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit16 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Account", "IsHaveCount", c => c.String(maxLength: 200));
            AddColumn("dbo.Account", "TenderRestrictSumPrice", c => c.String(maxLength: 200));
            AddColumn("dbo.AccountChild", "ClarifyLaunchPerson", c => c.String(maxLength: 500));
            AddColumn("dbo.AccountChild", "ClarifyLaunchDate", c => c.DateTime());
            AddColumn("dbo.AccountChild", "ClarifyReason", c => c.String(maxLength: 500));
            AddColumn("dbo.AccountChild", "ClarifyAcceptDate", c => c.DateTime());
            AddColumn("dbo.AccountChild", "ClarifyDisposePerson", c => c.String(maxLength: 100));
            AddColumn("dbo.AccountChild", "IsClarify", c => c.String(maxLength: 100));
            AddColumn("dbo.AccountChild", "ClarifyDisposeInfo", c => c.String(maxLength: 500));
            AddColumn("dbo.AccountChild", "ClarifyReplyDate", c => c.DateTime());
            AddColumn("dbo.AccountChild", "DissentLaunchPerson", c => c.String(maxLength: 100));
            AddColumn("dbo.AccountChild", "DissentLaunchPersonPhone", c => c.String(maxLength: 100));
            AddColumn("dbo.AccountChild", "DissentLaunchDate", c => c.DateTime());
            AddColumn("dbo.AccountChild", "DissentReason", c => c.String(maxLength: 500));
            AddColumn("dbo.AccountChild", "DissentAcceptDate", c => c.DateTime());
            AddColumn("dbo.AccountChild", "DissentAcceptPerson", c => c.String(maxLength: 100));
            AddColumn("dbo.AccountChild", "DissentDisposePerson", c => c.String(maxLength: 100));
            AddColumn("dbo.AccountChild", "DissentDisposeInfo", c => c.String(maxLength: 500));
            AddColumn("dbo.AccountChild", "DissentReplyDate", c => c.DateTime());
            AddColumn("dbo.AccountChild", "FrameFile", c => c.String(maxLength: 500));
            DropColumn("dbo.Account", "TenderSuccessFileDate");
            DropColumn("dbo.Account", "EvaluationTime");
            DropColumn("dbo.Account", "ClarifyLaunchPerson");
            DropColumn("dbo.Account", "ClarifyLaunchDate");
            DropColumn("dbo.Account", "ClarifyReason");
            DropColumn("dbo.Account", "ClarifyAcceptDate");
            DropColumn("dbo.Account", "ClarifyDisposePerson");
            DropColumn("dbo.Account", "IsClarify");
            DropColumn("dbo.Account", "ClarifyDisposeInfo");
            DropColumn("dbo.Account", "ClarifyReplyDate");
            DropColumn("dbo.Account", "DissentLaunchPerson");
            DropColumn("dbo.Account", "DissentLaunchPersonPhone");
            DropColumn("dbo.Account", "DissentLaunchDate");
            DropColumn("dbo.Account", "DissentReason");
            DropColumn("dbo.Account", "DissentAcceptDate");
            DropColumn("dbo.Account", "DissentAcceptPerson");
            DropColumn("dbo.Account", "DissentDisposePerson");
            DropColumn("dbo.Account", "DissentDisposeInfo");
            DropColumn("dbo.Account", "DissentReplyDate");
            DropColumn("dbo.AccountChild", "TenderFileAuditTime");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AccountChild", "TenderFileAuditTime", c => c.String(maxLength: 200));
            AddColumn("dbo.Account", "DissentReplyDate", c => c.DateTime());
            AddColumn("dbo.Account", "DissentDisposeInfo", c => c.String(maxLength: 500));
            AddColumn("dbo.Account", "DissentDisposePerson", c => c.String(maxLength: 100));
            AddColumn("dbo.Account", "DissentAcceptPerson", c => c.String(maxLength: 100));
            AddColumn("dbo.Account", "DissentAcceptDate", c => c.DateTime());
            AddColumn("dbo.Account", "DissentReason", c => c.String(maxLength: 500));
            AddColumn("dbo.Account", "DissentLaunchDate", c => c.DateTime());
            AddColumn("dbo.Account", "DissentLaunchPersonPhone", c => c.String(maxLength: 100));
            AddColumn("dbo.Account", "DissentLaunchPerson", c => c.String(maxLength: 100));
            AddColumn("dbo.Account", "ClarifyReplyDate", c => c.DateTime());
            AddColumn("dbo.Account", "ClarifyDisposeInfo", c => c.String(maxLength: 500));
            AddColumn("dbo.Account", "IsClarify", c => c.String(maxLength: 100));
            AddColumn("dbo.Account", "ClarifyDisposePerson", c => c.String(maxLength: 100));
            AddColumn("dbo.Account", "ClarifyAcceptDate", c => c.DateTime());
            AddColumn("dbo.Account", "ClarifyReason", c => c.String(maxLength: 500));
            AddColumn("dbo.Account", "ClarifyLaunchDate", c => c.DateTime());
            AddColumn("dbo.Account", "ClarifyLaunchPerson", c => c.String(maxLength: 500));
            AddColumn("dbo.Account", "EvaluationTime", c => c.String(maxLength: 200));
            AddColumn("dbo.Account", "TenderSuccessFileDate", c => c.DateTime());
            DropColumn("dbo.AccountChild", "FrameFile");
            DropColumn("dbo.AccountChild", "DissentReplyDate");
            DropColumn("dbo.AccountChild", "DissentDisposeInfo");
            DropColumn("dbo.AccountChild", "DissentDisposePerson");
            DropColumn("dbo.AccountChild", "DissentAcceptPerson");
            DropColumn("dbo.AccountChild", "DissentAcceptDate");
            DropColumn("dbo.AccountChild", "DissentReason");
            DropColumn("dbo.AccountChild", "DissentLaunchDate");
            DropColumn("dbo.AccountChild", "DissentLaunchPersonPhone");
            DropColumn("dbo.AccountChild", "DissentLaunchPerson");
            DropColumn("dbo.AccountChild", "ClarifyReplyDate");
            DropColumn("dbo.AccountChild", "ClarifyDisposeInfo");
            DropColumn("dbo.AccountChild", "IsClarify");
            DropColumn("dbo.AccountChild", "ClarifyDisposePerson");
            DropColumn("dbo.AccountChild", "ClarifyAcceptDate");
            DropColumn("dbo.AccountChild", "ClarifyReason");
            DropColumn("dbo.AccountChild", "ClarifyLaunchDate");
            DropColumn("dbo.AccountChild", "ClarifyLaunchPerson");
            DropColumn("dbo.Account", "TenderRestrictSumPrice");
            DropColumn("dbo.Account", "IsHaveCount");
        }
    }
}
