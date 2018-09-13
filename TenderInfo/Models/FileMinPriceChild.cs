using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TenderInfo.Models
{
    [Table("FileMinPriceChild")]
    public class FileMinPriceChild
    {
        /// <summary>
        /// 最低价--技术规格书，一、二级审批流程表ID
        /// </summary>
        [Key]
        public int FileMinPriceChildID { get; set; }

        /// <summary>
        /// 最低价--技术规格书审批表ID
        /// </summary>
        [Required]
        public int FileMinPriceID { get; set; }

        /// <summary>
        /// 审批级别，【一级、二级】
        /// </summary>
        [StringLength(200), Required]
        public string ApproveLevel { get; set; }

        /// <summary>
        /// 审批状态，【同意、退回、待审批】
        /// </summary>
        [StringLength(200), Required]
        public string ApproveState { get; set; }

        /// <summary>
        /// 审批人员ID
        /// </summary>
        public int ApprovePersonID { get; set; }

        /// <summary>
        /// 审批人员姓名
        /// </summary>
        [StringLength(100)]
        public string ApprovePersonName { get; set; }

        /// <summary>
        /// 审批人员部门ID
        /// </summary>
        public int ApprovePersonDeptID { get; set; }

        /// <summary>
        /// 审批人员部门名称
        /// </summary>
        [StringLength(100)]
        public string ApprovePersonDeptName { get; set; }

        /// <summary>
        /// 审批日期时间
        /// </summary>
        public DateTime ApproveDateTime { get; set; }

        /// <summary>
        /// 审批回退原因
        /// </summary>
        [StringLength(100)]
        public string ApproveBackReason { get; set; }
    }
}