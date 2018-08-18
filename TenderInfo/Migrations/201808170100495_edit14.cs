namespace TenderInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit14 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AccountMaterialChild", "AccountMaterialID", c => c.Int(nullable: false));
            AddColumn("dbo.AccountMaterialChild", "TableType", c => c.String(maxLength: 200));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AccountMaterialChild", "TableType");
            DropColumn("dbo.AccountMaterialChild", "AccountMaterialID");
        }
    }
}
