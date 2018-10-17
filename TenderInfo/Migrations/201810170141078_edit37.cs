namespace TenderInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit37 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FileComprehensive", "ApproveSuccessState", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FileComprehensive", "ApproveSuccessState");
        }
    }
}
