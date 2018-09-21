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
        /// <summary>
        /// 用户ID
        /// </summary>
        [Key]
        public int UserID { get; set; }

        /// <summary>
        /// 员工编号
        /// </summary>
        [Required,StringLength(20)]
        public string UserNum { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        [Required,StringLength(20)]
        public string UserName { get; set; }

        /// <summary>
        /// 用户密码
        /// </summary>
        [StringLength(100)]
        public string UserPassword { get; set; }

        /// <summary>
        /// 用户办公电话
        /// </summary>
        [StringLength(50)]
        public string UserPhone { get; set; }

        /// <summary>
        /// 用户手机
        /// </summary>
        [StringLength(50)]
        public string UserMobile { get; set; }

        /// <summary>
        /// 用户职务
        /// </summary>
        [StringLength(100)]
        public string UserDuty { get; set; }

        /// <summary>
        /// 用户电子邮件
        /// </summary>
        [DataType(DataType.EmailAddress)]
        public string UserEmail { get; set; }

        /// <summary>
        /// 用户备注
        /// </summary>
        [StringLength(200)]
        public string UserRemark { get; set; }

        /// <summary>
        /// 用户状态【0：正常】【1：删除】
        /// </summary>
        [Required]
        public int UserState { get; set; }

        /// <summary>
        /// 用户部门ID外键
        /// </summary>
        [Required]
        public int UserDeptID { get; set; }

        //[ForeignKey("UserDeptID")]
        //public DeptInfo DeptInfo { get; set; }
    }
}