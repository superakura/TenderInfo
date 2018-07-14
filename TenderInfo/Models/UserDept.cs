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
        [Key]
        public int UserDeptID { get; set; }//用户部门关系表ID

        public int UserID { get; set; }//用户表ID

        public int DeptID { get; set; }//部门表ID

        [ForeignKey("DeptID")]
        public DeptInfo DeptInfo { get; set; }
    }
}