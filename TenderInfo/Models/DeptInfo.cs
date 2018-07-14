using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TenderInfo.Models
{
    [Table("DeptInfo")]
    public class DeptInfo
    {
        [Key]
        public int DeptID { get; set; }//部门ID

        [Required]
        [StringLength(50)]
        public string DeptName { get; set; }//部门名称

        public int DeptFatherID { get; set; }//父级部门ID

        public int DeptState { get; set; }//部门状态 0：正常 1：删除

        [StringLength(200)]
        public string DeptRemark { get; set; }//部门备注

        public int DeptOrder { get; set; }//部门顺序

        public byte Open { get; set; }//菜单是否展开

        public DateTime DeptCreateDate { get; set; }//部门创建日期时间
    }
}