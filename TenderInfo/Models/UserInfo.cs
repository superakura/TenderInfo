using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TenderInfo.Models
{
    [Table("UserInfo")]
    public class UserInfo
    {
        [Key]
        public int UserID { get; set; }//用户ID

        [Required]
        [StringLength(20)]
        public string UserNum { get; set; }//用户的员工编号

        [Required]
        [StringLength(20)]
        public string UserName { get; set; }//用户姓名

        [StringLength(100)]
        public string UserPassword { get; set; }//用户密码

        [StringLength(50)]
        public string UserPhone { get; set; }//用户办公电话

        [StringLength(50)]
        public string UserMobile { get; set; }//用户手机

        [StringLength(100)]
        public string UserDuty { get; set; }//用户职务

        [DataType(DataType.EmailAddress)]
        public string UserEmail { get; set; }//用户电子邮件

        [StringLength(200)]
        public string UserRemark { get; set; }//用户备注

        [Required]
        public int UserState { get; set; }//用户状态【0：正常】【1：删除】

        [Required]
        public int UserDeptID { get; set; }//用户部门ID外键

        //[ForeignKey("UserDeptID")]
        //public DeptInfo DeptInfo { get; set; }
    }
}