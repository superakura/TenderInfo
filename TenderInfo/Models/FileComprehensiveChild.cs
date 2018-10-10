using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TenderInfo.Models
{
    [Table("FileComprehensiveChild")]
    public class FileComprehensiveChild
    {
        /// <summary>
        /// 综合评标法审批流程表ID
        /// </summary>
        [Key]
        public int FileComprehensiveChildID { get; set; }

        /// <summary>
        /// 综合评标法审批表ID
        /// </summary>
        public int FileComprehensiveID { get; set; }

        /// <summary>
        /// 审批类型，技术规格书、评分标准(技术)、评分标准(商务)
        /// </summary>
        [StringLength(200)]
        public string ApproveType { get; set; }

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
        /// 审批人员父级部门ID
        /// </summary>
        public int ApprovePersonFatherDeptID { get; set; }

        /// <summary>
        /// 审批人员父级部门名称
        /// </summary>
        [StringLength(100)]
        public string ApprovePersonFatherDeptName { get; set; }

        /// <summary>
        /// 审批日期时间
        /// </summary>
        public DateTime? ApproveDateTime { get; set; }

        /// <summary>
        /// 审批回退原因
        /// </summary>
        [StringLength(100)]
        public string ApproveBackReason { get; set; }
    }
}