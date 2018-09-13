using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TenderInfo.Models
{
    [Table("FileMinPrice")]
    public class FileMinPrice
    {
        /// <summary>
        /// 最低价--技术规格书审批表ID
        /// </summary>
        [Key]
        public int FileMinPriceID { get; set; }

        /// <summary>
        /// 技术规格书文件名
        /// </summary>
        [StringLength(200),Required]
        public string TechnicSpecificationFile { get; set; }

        /// <summary>
        /// 审批级别
        /// </summary>
        [StringLength(200), Required]
        public string ApproveLevel { get; set; }

        /// <summary>
        /// 审批状态
        /// </summary>
        [StringLength(200), Required]
        public string ApproveState { get; set; }

        /// <summary>
        /// 提交人员ID
        /// </summary>
        public int InputPersonID { get; set; }

        /// <summary>
        /// 提交人员姓名
        /// </summary>
        [StringLength(100)]
        public string InputPersonName { get; set; }

        /// <summary>
        /// 提交人员部门ID
        /// </summary>
        public int InputPersonDeptID { get; set; }

        /// <summary>
        /// 提交人员部门名称
        /// </summary>
        [StringLength(100)]
        public string InputPersonDeptName { get; set; }

        /// <summary>
        /// 提交日期时间
        /// </summary>
        public DateTime InputDateTime { get; set; }
    }
}