namespace TenderInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit13 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AccountProject", "ProjectType", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AccountProject", "ProjectType");
        }
    }
}
