namespace TenderInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit35 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FileComprehensiveChild", "ApproveType", c => c.String(maxLength: 200));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FileComprehensiveChild", "ApproveType");
        }
    }
}
