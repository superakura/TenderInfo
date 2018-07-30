using System;
using System.Data.Entity;
using System.Linq;
namespace TenderInfo.Models
{
    public class DB : DbContext
    {
        //您的上下文已配置为从您的应用程序的配置文件(App.config 或 Web.config)
        //使用“DB”连接字符串。默认情况下，此连接字符串针对您的 LocalDb 实例上的
        //“TenderInfo.DB”数据库。
        // 
        //如果您想要针对其他数据库和/或数据库提供程序，请在应用程序配置文件中修改“DB”
        //连接字符串。
        public DB()
            : base("name=DB")
        {
        }

        //为您要在模型中包含的每种实体类型都添加 DbSet。有关配置和使用 Code First  模型
        //的详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=390109。

        public virtual DbSet<DeptInfo> DeptInfo { get; set; }//部门信息表
        public virtual DbSet<UserInfo> UserInfo { get; set; }//用户信息表
        public virtual DbSet<AuthorityInfo> AuthorityInfo { get; set; }//权限信息表
        public virtual DbSet<RoleAuthority> RoleAuthority { get; set; }//角色权限关系表
        public virtual DbSet<RoleInfo> RoleInfo { get; set; }//角色信息表
        public virtual DbSet<UserRole> UserRole { get; set; }//用户角色关系表
        public virtual DbSet<NoticeInfo> NoticeInfo { get; set; }//通知公告信息表
        public virtual DbSet<UserDept> UserDept { get; set; }//用户部门关系表
        public virtual DbSet<Log> Log { get; set; }//日志信息表

        public virtual DbSet<AccountMaterial> AccountMaterial { get; set; }//招标台账信息表--（物资）
        public virtual DbSet<AccountMaterialChild> AccountMaterialChild { get; set; }//招标台账信息子表--（物资）
        public virtual DbSet<AccountProject> AccountProject { get; set; }//招标台账信息表--（工程、服务）
        public virtual DbSet<AccountProjectChild> AccountProjectChild { get; set; }//招标台账信息子表--（工程、服务）

        public virtual DbSet<ProgressMaterial> ProgressMaterial { get; set; }//招标进度--（工程、服务）
        public virtual DbSet<ProgressProject> ProgressProject { get; set; }//招标进度--（物资、框架）
        public virtual DbSet<ProgressInfo> ProgressInfo { get; set; }//招标进度

        //public virtual DbSet<TechnicSpecificationApprove> TechnicSpecificationApprove { get; set; }//技术规格书审批表
        public virtual DbSet<SampleDelegation> SampleDelegation { get; set; }//送样委托表
    }
}