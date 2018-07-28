namespace TenderInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProgressMaterial", "IsOver", c => c.String(maxLength: 100));
            AddColumn("dbo.ProgressMaterial", "YearInfo", c => c.String(maxLength: 100));
            AddColumn("dbo.ProgressProject", "IsOver", c => c.String(maxLength: 100));
            AddColumn("dbo.ProgressProject", "YearInfo", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProgressProject", "YearInfo");
            DropColumn("dbo.ProgressProject", "IsOver");
            DropColumn("dbo.ProgressMaterial", "YearInfo");
            DropColumn("dbo.ProgressMaterial", "IsOver");
        }
    }
}
