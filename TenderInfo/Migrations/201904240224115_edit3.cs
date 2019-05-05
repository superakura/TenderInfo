namespace TenderInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Account", "CompleteTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Account", "CompleteTime");
        }
    }
}
