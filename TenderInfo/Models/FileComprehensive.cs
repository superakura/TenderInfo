using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace TenderInfo.Models
{
    [Table("FileComprehensive")]
    public class FileComprehensive
    {
        /// <summary>
        /// 综合评标法审批表ID
        /// </summary>
        [Key]
        public int FileComprehensiveID { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        [StringLength(200)]
        public string ProjectName { get; set; }

        /// <summary>
        /// 审批是否全部完成
        /// </summary>
        [StringLength(50)]
        public string ApproveSuccessState { get; set; }

        /// <summary>
        /// 合并后的技术规格书文件名
        /// </summary>
        [StringLength(200)]
        public string TechnicSpecificationFileMerge { get; set; }

        /// <summary>
        /// 合并后的技术规格书文件名--显示用
        /// </summary>
        [StringLength(200)]
        public string TechnicSpecificationFileMergeShow { get; set; }

        /// <summary>
        /// 技术规格书文件名
        /// </summary>
        [StringLength(200), Required]
        public string TechnicSpecificationFile { get; set; }

        /// <summary>
        /// 技术规格书文件名--显示用
        /// </summary>
        [StringLength(200), Required]
        public string TechnicSpecificationFileShow { get; set; }

        /// <summary>
        /// 评分标准--技术
        /// </summary>
        [StringLength(200), Required]
        public string TechnologyScoreStandardFile { get; set; }

        /// <summary>
        /// 评分标准--技术--显示用
        /// </summary>
        [StringLength(200), Required]
        public string TechnologyScoreStandardFileShow { get; set; }

        /// <summary>
        /// 评分标准--商务
        /// </summary>
        [StringLength(200), Required]
        public string BusinessScoreStandardFile { get; set; }

        /// <summary>
        /// 评分标准--商务--显示用
        /// </summary>
        [StringLength(200), Required]
        public string BusinessScoreStandardFileShow { get; set; }

        /// <summary>
        /// 审批级别--技术规格书
        /// </summary>
        [StringLength(200), Required]
        public string ApproveLevelSpecification { get; set; }

        /// <summary>
        /// 审批状态--技术规格书
        /// </summary>
        [StringLength(200), Required]
        public string ApproveStateSpecification { get; set; }

        /// <summary>
        /// 审批级别--评分标准--技术
        /// </summary>
        [StringLength(200), Required]
        public string ApproveLevelTechnology { get; set; }

        /// <summary>
        /// 审批状态--评分标准--技术
        /// </summary>
        [StringLength(200), Required]
        public string ApproveStateTechnology { get; set; }

        /// <summary>
        /// 审批级别--评分标准--商务
        /// </summary>
        [StringLength(200), Required]
        public string ApproveLevelBusiness { get; set; }

        /// <summary>
        /// 审批状态--评分标准--商务
        /// </summary>
        [StringLength(200), Required]
        public string ApproveStateBusiness { get; set; }

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
        /// 提交人员父级部门ID
        /// </summary>
        public int InputPersonFatherDeptID { get; set; }

        /// <summary>
        /// 提交人员父级部门名称
        /// </summary>
        [StringLength(100)]
        public string InputPersonFatherDeptName { get; set; }

        /// <summary>
        /// 提交日期时间
        /// </summary>
        public DateTime InputDateTime { get; set; }
    }
}