using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TenderInfo.Models
{
    [Table("UserDept")]
    public class UserDept
    {
        /// <summary>
        /// 用户部门关系表ID
        /// </summary>
        [Key]
        public int UserDeptID { get; set; }

        /// <summary>
        /// 用户表ID
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// 部门表ID
        /// </summary>
        public int DeptID { get; set; }

        //[ForeignKey("DeptID")]
        //public DeptInfo DeptInfo { get; set; }
    }
}