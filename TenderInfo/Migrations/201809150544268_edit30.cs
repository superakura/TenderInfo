namespace TenderInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit30 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FileMinPrice", "TechnicSpecificationFileShow", c => c.String(maxLength: 200));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FileMinPrice", "TechnicSpecificationFileShow");
        }
    }
}
