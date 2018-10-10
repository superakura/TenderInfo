namespace TenderInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit33 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.FileComprehensiveChild", "SpecificationApproveLevel", c => c.String(maxLength: 200));
            AlterColumn("dbo.FileComprehensiveChild", "SpecificationApproveState", c => c.String(maxLength: 200));
            AlterColumn("dbo.FileComprehensiveChild", "TechnologyApproveLevel", c => c.String(maxLength: 200));
            AlterColumn("dbo.FileComprehensiveChild", "TechnologyApproveState", c => c.String(maxLength: 200));
            AlterColumn("dbo.FileComprehensiveChild", "BusinessApproveLevel", c => c.String(maxLength: 200));
            AlterColumn("dbo.FileComprehensiveChild", "BusinessApproveState", c => c.String(maxLength: 200));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.FileComprehensiveChild", "BusinessApproveState", c => c.String(nullable: false, maxLength: 200));
            AlterColumn("dbo.FileComprehensiveChild", "BusinessApproveLevel", c => c.String(nullable: false, maxLength: 200));
            AlterColumn("dbo.FileComprehensiveChild", "TechnologyApproveState", c => c.String(nullable: false, maxLength: 200));
            AlterColumn("dbo.FileComprehensiveChild", "TechnologyApproveLevel", c => c.String(nullable: false, maxLength: 200));
            AlterColumn("dbo.FileComprehensiveChild", "SpecificationApproveState", c => c.String(nullable: false, maxLength: 200));
            AlterColumn("dbo.FileComprehensiveChild", "SpecificationApproveLevel", c => c.String(nullable: false, maxLength: 200));
        }
    }
}
