namespace TenderInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit32 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FileComprehensive", "TechnicSpecificationFileMergeShow", c => c.String(maxLength: 200));
            AddColumn("dbo.FileComprehensive", "TechnicSpecificationFileShow", c => c.String(nullable: false, maxLength: 200));
            AddColumn("dbo.FileComprehensive", "TechnologyScoreStandardFileShow", c => c.String(nullable: false, maxLength: 200));
            AddColumn("dbo.FileComprehensive", "BusinessScoreStandardFileShow", c => c.String(nullable: false, maxLength: 200));
            AlterColumn("dbo.FileComprehensive", "TechnicSpecificationFileMerge", c => c.String(maxLength: 200));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.FileComprehensive", "TechnicSpecificationFileMerge", c => c.String(nullable: false, maxLength: 200));
            DropColumn("dbo.FileComprehensive", "BusinessScoreStandardFileShow");
            DropColumn("dbo.FileComprehensive", "TechnologyScoreStandardFileShow");
            DropColumn("dbo.FileComprehensive", "TechnicSpecificationFileShow");
            DropColumn("dbo.FileComprehensive", "TechnicSpecificationFileMergeShow");
        }
    }
}
