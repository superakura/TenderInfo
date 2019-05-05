namespace TenderInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit4 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AccountApprove", "ApproveTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AccountApprove", "ApproveTime", c => c.DateTime(nullable: false));
        }
    }
}
