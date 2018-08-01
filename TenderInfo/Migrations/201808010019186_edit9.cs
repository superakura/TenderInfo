namespace TenderInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit9 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Log", "InputPersonNameID", c => c.Int(nullable: false));
            AddColumn("dbo.Log", "Col1", c => c.String(maxLength: 500));
            AddColumn("dbo.Log", "Col2", c => c.String(maxLength: 500));
            AddColumn("dbo.Log", "Col3", c => c.String(maxLength: 500));
            AddColumn("dbo.Log", "Col4", c => c.String(maxLength: 500));
            AddColumn("dbo.Log", "Col5", c => c.String(maxLength: 500));
            AddColumn("dbo.ProgressInfo", "ProjectResponsiblePersonName", c => c.String(maxLength: 200));
            AddColumn("dbo.ProgressInfo", "ProjectResponsiblePersonID", c => c.Int(nullable: false));
            AddColumn("dbo.ProgressInfo", "InputDateTime", c => c.DateTime());
            DropColumn("dbo.ProgressInfo", "ProjectResponsiblePerson");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProgressInfo", "ProjectResponsiblePerson", c => c.String(maxLength: 200));
            DropColumn("dbo.ProgressInfo", "InputDateTime");
            DropColumn("dbo.ProgressInfo", "ProjectResponsiblePersonID");
            DropColumn("dbo.ProgressInfo", "ProjectResponsiblePersonName");
            DropColumn("dbo.Log", "Col5");
            DropColumn("dbo.Log", "Col4");
            DropColumn("dbo.Log", "Col3");
            DropColumn("dbo.Log", "Col2");
            DropColumn("dbo.Log", "Col1");
            DropColumn("dbo.Log", "InputPersonNameID");
        }
    }
}
