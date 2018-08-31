using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TenderInfo.Models
{
    [Table("ProgressInfo")]
    public class ProgressInfo
    {
        [Key]
        public int ProgressInfoID { get; set; }

        /// <summary>
        /// 【物资】、【框架】、【工程、服务】
        /// </summary>
        [StringLength(100)]
        public string ProgressType { get; set; }

        /// <summary>
        /// 子分类（动设备、静设备、化工三剂等）
        /// </summary>
        [StringLength(100)]
        public string ProgressTypeChild { get; set; }

        /// <summary>
        /// 进度状态,对接、实施
        /// </summary>
        [StringLength(100)]
        public string ProgressState { get; set; }

        /// <summary>
        /// 拟招标项目名称或物资名称
        /// </summary>
        [StringLength(200)]
        public string ProjectName { get; set; }

        /// <summary>
        /// 投资金额（万元）
        /// </summary>
        public decimal? InvestPrice { get; set; }

        /// <summary>
        /// 招标项目负责人姓名
        /// </summary>
        [StringLength(200)]
        public string ProjectResponsiblePersonName { get; set; }

        /// <summary>
        /// 招标项目负责人ID
        /// </summary>
        public int ProjectResponsiblePersonID { get; set; }

        /// <summary>
        /// 合同签订部门项目责任人
        /// </summary>
        [StringLength(100)]
        public string ContractResponsiblePerson { get; set; }

        /// <summary>
        /// 物资数量--物资、框架类需要填写
        /// </summary>
        [StringLength(200)]
        public string MaterialCount { get; set; }

        #region 项目前期对接进度--物资、框架
        /// <summary>
        /// 技术规格书（技术文件、图纸等）提报时间--用户提出需要文字说明
        /// </summary>
        [StringLength(500)]
        public string TechnicalSpecificationAddDate { get; set; }

        /// <summary>
        /// 技术规格书（技术文件、图纸等）对接需说明情况
        /// </summary>
        [StringLength(500)]
        public string TechnicalSpecificationExplain { get; set; }

        /// <summary>
        /// 技术规格书（技术文件、图纸等）定稿时间
        /// </summary>
        public DateTime? TechnicalSpecificationApproveDate { get; set; }

        /// <summary>
        /// 综合法评标标准定稿时间
        /// </summary>
        public DateTime? SynthesizeEvaluationRuleApproveDate { get; set; }

        /// <summary>
        /// 招标方案联审时间--可空
        /// </summary>
        public DateTime? TenderProgramAuditDate { get; set; }
        #endregion

        #region 项目前期对接进度--工程、服务
        /// <summary>
        /// 接到合同签订部门（电话或邮件）时间
        /// </summary>
        [StringLength(100)]
        public string ContractDeptContactDate { get; set; }

        /// <summary>
        /// 项目对接需说明情况
        /// </summary>
        [StringLength(500)]
        public string ProjectExplain { get; set; }
        #endregion

        #region 项目实施进度
        /// <summary>
        /// 招标方案接收时间--可空
        /// </summary>
        public DateTime? ProgramAcceptDate { get; set; }

        /// <summary>
        /// 发售招标文件开始日期--可空
        /// </summary>
        public DateTime? TenderFileSaleStartDate { get; set; }

        /// <summary>
        /// 发售招标文件截止日期--可空
        /// </summary>
        public DateTime? TenderFileSaleEndDate { get; set; }

        /// <summary>
        /// 开标日期--可空
        /// </summary>
        public DateTime? TenderStartDate { get; set; }

        /// <summary>
        /// 中标通知书发出时间--可空
        /// </summary>
        public DateTime? TenderSuccessFileDate { get; set; }
        #endregion

        /// <summary>
        /// 其他需说明情况
        /// </summary>
        [StringLength(1000)]
        public string OtherExplain { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(1000)]
        public string Remark { get; set; }

        /// <summary>
        /// 是否已结束
        /// </summary>
        [StringLength(100)]
        public string IsOver { get; set; }

        /// <summary>
        /// 年度信息
        /// </summary>
        [StringLength(100)]
        public string YearInfo { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? InputDateTime { get; set; }

        /// <summary>
        /// 招标台账ID
        /// </summary>
        public int? AccountID { get; set; }

        /// <summary>
        /// 是否与招标台账数据完成同步
        /// </summary>
        [StringLength(50)]
        public string IsSynchro { get; set; }
    }
}