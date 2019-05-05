using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TenderInfo.Models
{
    [Table("AccountApprove")]
    public class AccountApprove
    {
        /// <summary>
        /// 台账修改审批表ID
        /// </summary>
        [Key]
        public int AccountApproveID { get; set; }

        /// <summary>
        /// 台账信息表ID
        /// </summary>
        public int AccountID { get; set; }

        /// <summary>
        /// 审批状态
        /// </summary>
        [StringLength(50)]
        public string ApproveState { get; set; }

        /// <summary>
        /// 审批时间
        /// </summary>
        public DateTime? ApproveTime { get; set; }

        /// <summary>
        /// 审批人员ID
        /// </summary>
        public int ApprovePersonID { get; set; }

        /// <summary>
        /// 审批人员姓名
        /// </summary>
        [StringLength(50)]
        public string ApprovePersonName { get; set; }

        /// <summary>
        /// 审批回退原因
        /// </summary>
        [StringLength(50)]
        public string ApproveBackReason { get; set; }

        /// <summary>
        /// 提交人员ID
        /// </summary>
        public int SubmitPersonID { get; set; }

        /// <summary>
        /// 提交人员姓名
        /// </summary>
        [StringLength(50)]
        public string SubmitPersonName { get; set; }

        /// <summary>
        /// 提交时间
        /// </summary>
        public DateTime SubmitTime { get; set; }

        /// <summary>
        /// 修改原因
        /// </summary>
        [StringLength(500)]
        public string SubmitEditReason { get; set; }
    }
}