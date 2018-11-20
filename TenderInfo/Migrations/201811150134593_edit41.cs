namespace TenderInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit41 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.FileComprehensive", "TechnologyScoreStandardFile", c => c.String(maxLength: 200));
            AlterColumn("dbo.FileComprehensive", "TechnologyScoreStandardFileShow", c => c.String(maxLength: 200));
            AlterColumn("dbo.FileComprehensive", "BusinessScoreStandardFile", c => c.String(maxLength: 200));
            AlterColumn("dbo.FileComprehensive", "BusinessScoreStandardFileShow", c => c.String(maxLength: 200));
            AlterColumn("dbo.FileComprehensive", "ApproveLevelTechnology", c => c.String(maxLength: 200));
            AlterColumn("dbo.FileComprehensive", "ApproveStateTechnology", c => c.String(maxLength: 200));
            AlterColumn("dbo.FileComprehensive", "ApproveLevelBusiness", c => c.String(maxLength: 200));
            AlterColumn("dbo.FileComprehensive", "ApproveStateBusiness", c => c.String(maxLength: 200));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.FileComprehensive", "ApproveStateBusiness", c => c.String(nullable: false, maxLength: 200));
            AlterColumn("dbo.FileComprehensive", "ApproveLevelBusiness", c => c.String(nullable: false, maxLength: 200));
            AlterColumn("dbo.FileComprehensive", "ApproveStateTechnology", c => c.String(nullable: false, maxLength: 200));
            AlterColumn("dbo.FileComprehensive", "ApproveLevelTechnology", c => c.String(nullable: false, maxLength: 200));
            AlterColumn("dbo.FileComprehensive", "BusinessScoreStandardFileShow", c => c.String(nullable: false, maxLength: 200));
            AlterColumn("dbo.FileComprehensive", "BusinessScoreStandardFile", c => c.String(nullable: false, maxLength: 200));
            AlterColumn("dbo.FileComprehensive", "TechnologyScoreStandardFileShow", c => c.String(nullable: false, maxLength: 200));
            AlterColumn("dbo.FileComprehensive", "TechnologyScoreStandardFile", c => c.String(nullable: false, maxLength: 200));
        }
    }
}
