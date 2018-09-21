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
        /// <summary>
        /// 部门ID
        /// </summary>
        [Key]
        public int DeptID { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        [Required,StringLength(50)]
        public string DeptName { get; set; }

        /// <summary>
        /// 父级部门ID
        /// </summary>
        public int DeptFatherID { get; set; }

        /// <summary>
        /// 部门状态 0：正常 1：删除
        /// </summary>
        public int DeptState { get; set; }

        /// <summary>
        /// 部门备注
        /// </summary>
        [StringLength(200)]
        public string DeptRemark { get; set; }

        /// <summary>
        /// 部门顺序
        /// </summary>
        public int DeptOrder { get; set; }

        /// <summary>
        /// 菜单是否展开
        /// </summary>
        public byte Open { get; set; }

        /// <summary>
        /// 部门创建日期时间
        /// </summary>
        public DateTime DeptCreateDate { get; set; }
    }
}