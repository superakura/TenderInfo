namespace TenderInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit34 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FileComprehensive", "ProjectName", c => c.String(maxLength: 200));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FileComprehensive", "ProjectName");
        }
    }
}
