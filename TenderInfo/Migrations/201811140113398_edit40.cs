namespace TenderInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit40 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GroupLeader",
                c => new
                    {
                        GroupLeaderID = c.Int(nullable: false, identity: true),
                        LeaderUserID = c.Int(nullable: false),
                        MemberUserID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.GroupLeaderID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.GroupLeader");
        }
    }
}
