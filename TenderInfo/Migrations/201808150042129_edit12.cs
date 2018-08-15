namespace TenderInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit12 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AccountMaterial", "ProjectResponsiblePersonName", c => c.String(maxLength: 200));
            AddColumn("dbo.AccountMaterial", "ProjectResponsiblePersonID", c => c.Int(nullable: false));
            AddColumn("dbo.AccountMaterial", "InputPersonID", c => c.Int(nullable: false));
            AddColumn("dbo.AccountMaterial", "InsertDate", c => c.DateTime());
            AddColumn("dbo.AccountMaterial", "InsertPersonID", c => c.Int(nullable: false));
            AddColumn("dbo.AccountProject", "ProjectResponsiblePersonName", c => c.String(maxLength: 200));
            AddColumn("dbo.AccountProject", "ProjectResponsiblePersonID", c => c.Int(nullable: false));
            AddColumn("dbo.AccountProject", "InputPersonID", c => c.Int(nullable: false));
            AddColumn("dbo.AccountProject", "InsertDate", c => c.DateTime());
            AddColumn("dbo.AccountProject", "InsertPersonID", c => c.Int(nullable: false));
            AlterColumn("dbo.AccountMaterial", "InputDate", c => c.DateTime());
            AlterColumn("dbo.AccountProject", "InputDate", c => c.DateTime());
            DropColumn("dbo.AccountMaterial", "ProjectResponsiblePerson");
            DropColumn("dbo.AccountMaterial", "InputPerson");
            DropColumn("dbo.AccountProject", "ProjectResponsiblePerson");
            DropColumn("dbo.AccountProject", "InputPerson");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AccountProject", "InputPerson", c => c.Int(nullable: false));
            AddColumn("dbo.AccountProject", "ProjectResponsiblePerson", c => c.String(maxLength: 200));
            AddColumn("dbo.AccountMaterial", "InputPerson", c => c.Int(nullable: false));
            AddColumn("dbo.AccountMaterial", "ProjectResponsiblePerson", c => c.String(maxLength: 200));
            AlterColumn("dbo.AccountProject", "InputDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.AccountMaterial", "InputDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.AccountProject", "InsertPersonID");
            DropColumn("dbo.AccountProject", "InsertDate");
            DropColumn("dbo.AccountProject", "InputPersonID");
            DropColumn("dbo.AccountProject", "ProjectResponsiblePersonID");
            DropColumn("dbo.AccountProject", "ProjectResponsiblePersonName");
            DropColumn("dbo.AccountMaterial", "InsertPersonID");
            DropColumn("dbo.AccountMaterial", "InsertDate");
            DropColumn("dbo.AccountMaterial", "InputPersonID");
            DropColumn("dbo.AccountMaterial", "ProjectResponsiblePersonID");
            DropColumn("dbo.AccountMaterial", "ProjectResponsiblePersonName");
        }
    }
}
