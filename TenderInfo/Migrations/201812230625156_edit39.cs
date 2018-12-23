namespace TenderInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit39 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Account", "QualificationExamMethod", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Account", "QualificationExamMethod");
        }
    }
}
