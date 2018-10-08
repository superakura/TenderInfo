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

        #region 技术规格书流程
        /// <summary>
        /// 技术规格书--审批级别，【一级、二级】
        /// </summary>
        [StringLength(200), Required]
        public string SpecificationApproveLevel { get; set; }

        /// <summary>
        /// 技术规格书--审批状态，【同意、退回、待审批】
        /// </summary>
        [StringLength(200), Required]
        public string SpecificationApproveState { get; set; }

        /// <summary>
        /// 技术规格书--审批人员ID
        /// </summary>
        public int SpecificationApprovePersonID { get; set; }

        /// <summary>
        /// 技术规格书--审批人员姓名
        /// </summary>
        [StringLength(100)]
        public string SpecificationApprovePersonName { get; set; }

        /// <summary>
        /// 技术规格书--审批人员部门ID
        /// </summary>
        public int SpecificationApprovePersonDeptID { get; set; }

        /// <summary>
        /// 技术规格书--审批人员部门名称
        /// </summary>
        [StringLength(100)]
        public string SpecificationApprovePersonDeptName { get; set; }

        /// <summary>
        /// 技术规格书--审批人员父级部门ID
        /// </summary>
        public int SpecificationApprovePersonFatherDeptID { get; set; }

        /// <summary>
        /// 技术规格书--审批人员父级部门名称
        /// </summary>
        [StringLength(100)]
        public string SpecificationApprovePersonFatherDeptName { get; set; }

        /// <summary>
        /// 技术规格书--审批日期时间
        /// </summary>
        public DateTime? SpecificationApproveDateTime { get; set; }

        /// <summary>
        /// 技术规格书--审批回退原因
        /// </summary>
        [StringLength(100)]
        public string SpecificationApproveBackReason { get; set; }
        #endregion

        #region 评分标准--技术--流程
        /// <summary>
        /// 评分标准--技术--审批级别，【一级、二级】
        /// </summary>
        [StringLength(200), Required]
        public string TechnologyApproveLevel { get; set; }

        /// <summary>
        /// 评分标准--技术--审批状态，【同意、退回、待审批】
        /// </summary>
        [StringLength(200), Required]
        public string TechnologyApproveState { get; set; }

        /// <summary>
        /// 评分标准--技术--审批人员ID
        /// </summary>
        public int TechnologyApprovePersonID { get; set; }

        /// <summary>
        /// 评分标准--技术--审批人员姓名
        /// </summary>
        [StringLength(100)]
        public string TechnologyApprovePersonName { get; set; }

        /// <summary>
        /// 评分标准--技术--审批人员部门ID
        /// </summary>
        public int TechnologyApprovePersonDeptID { get; set; }

        /// <summary>
        /// 评分标准--技术--审批人员部门名称
        /// </summary>
        [StringLength(100)]
        public string TechnologyApprovePersonDeptName { get; set; }

        /// <summary>
        /// 评分标准--技术--审批人员父级部门ID
        /// </summary>
        public int TechnologyApprovePersonFatherDeptID { get; set; }

        /// <summary>
        /// 评分标准--技术--审批人员父级部门名称
        /// </summary>
        [StringLength(100)]
        public string TechnologyApprovePersonFatherDeptName { get; set; }

        /// <summary>
        /// 评分标准--技术--审批日期时间
        /// </summary>
        public DateTime? TechnologyApproveDateTime { get; set; }

        /// <summary>
        /// 评分标准--技术--审批回退原因
        /// </summary>
        [StringLength(100)]
        public string TechnologyApproveBackReason { get; set; }
        #endregion

        #region 评分标准--商务--流程
        /// <summary>
        /// 评分标准--商务--审批级别，【一级、二级】
        /// </summary>
        [StringLength(200), Required]
        public string BusinessApproveLevel { get; set; }

        /// <summary>
        /// 评分标准--商务--审批状态，【同意、退回、待审批】
        /// </summary>
        [StringLength(200), Required]
        public string BusinessApproveState { get; set; }

        /// <summary>
        /// 评分标准--商务--审批人员ID
        /// </summary>
        public int BusinessApprovePersonID { get; set; }

        /// <summary>
        /// 评分标准--商务--审批人员姓名
        /// </summary>
        [StringLength(100)]
        public string BusinessApprovePersonName { get; set; }

        /// <summary>
        /// 评分标准--商务--审批人员部门ID
        /// </summary>
        public int BusinessApprovePersonDeptID { get; set; }

        /// <summary>
        /// 评分标准--商务--审批人员部门名称
        /// </summary>
        [StringLength(100)]
        public string BusinessApprovePersonDeptName { get; set; }

        /// <summary>
        /// 评分标准--商务--审批人员父级部门ID
        /// </summary>
        public int BusinessApprovePersonFatherDeptID { get; set; }

        /// <summary>
        /// 评分标准--商务--审批人员父级部门名称
        /// </summary>
        [StringLength(100)]
        public string BusinessApprovePersonFatherDeptName { get; set; }

        /// <summary>
        /// 评分标准--商务--审批日期时间
        /// </summary>
        public DateTime? BusinessApproveDateTime { get; set; }

        /// <summary>
        /// 评分标准--商务--审批回退原因
        /// </summary>
        [StringLength(100)]
        public string BusinessApproveBackReason { get; set; }
        #endregion
    }
}