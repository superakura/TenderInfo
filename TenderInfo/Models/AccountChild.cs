using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TenderInfo.Models
{
    [Table("AccountChild")]
    public class AccountChild
    {
        [Key]
        public int AccountChildID { get; set; }

        public int AccountID { get; set; }

        /// <summary>
        ///招标台账子表类型,【first】【second】【third】
        /// </summary>
        [StringLength(200)]
        public string TableType { get; set; }

        /// <summary>
        /// 使用单位名称--物资、框架
        /// </summary>
        [StringLength(200)]
        public string UsingDeptName { get; set; }

        /// <summary>
        /// 使用单位ID--物资、框架
        /// </summary>
        public int UsingDeptID { get; set; }

        /// <summary>
        ///框架附属文件
        /// </summary>
        [StringLength(500)]
        public string FrameFile { get; set; }

        /// <summary>
        /// 信息录入日期时间
        /// </summary>
        public DateTime InputDate { get; set; }

        /// <summary>
        /// 信息录入人员ID
        /// </summary>
        public int InputPerson { get; set; }

        #region 投标人信息
        /// <summary>
        /// 购买招标文件潜在投标人
        /// </summary>
        [StringLength(200)]
        public string TenderFilePlanPayPerson { get; set; }

        /// <summary>
        /// 投标人
        /// </summary>
        [StringLength(200)]
        public string TenderPerson { get; set; }

        /// <summary>
        /// 产品制造商（代理、贸易商投标时填写）
        /// </summary>
        [StringLength(200)]
        public string ProductManufacturer { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal? QuotedPriceUnit { get; set; }

        /// <summary>
        /// 【报价（万元）--工程、服务、框架】【总价（万元）--物资】
        /// </summary>
        [StringLength(200)]
        public string QuotedPriceSum { get; set; }

        /// <summary>
        /// 初步评审是否被否决--是/否
        /// </summary>
        [StringLength(500)]
        public string NegationExplain { get; set; }

        /// <summary>
        /// 否决原因
        /// </summary>
        [StringLength(500)]
        public string VetoReason { get; set; }

        /// <summary>
        /// 投标人--评标复议版本
        /// </summary>
        [StringLength(500)]
        public string TenderPersonVersion { get; set; }
        #endregion

        #region 评标委员会
        /// <summary>
        /// 评标委员会--姓名
        /// </summary>
        [StringLength(100)]
        public string EvaluationPersonName { get; set; }

        /// <summary>
        /// 评标委员会--单位名称
        /// </summary>
        [StringLength(200)]
        public string EvaluationPersonDeptName { get; set; }

        /// <summary>
        /// 评标委员会--单位ID
        /// </summary>
        public int EvaluationPersonDeptID { get; set; }

        /// <summary>
        /// 评标委员会--是否评标委员会主任
        /// </summary>
        [StringLength(100)]
        public string IsEvaluationDirector { get; set; }

        /// <summary>
        /// 评标委员会--评审时间（小时）
        /// </summary>
        public decimal? EvaluationTime { get; set; }

        /// <summary>
        /// 评标委员会--评审费
        /// </summary>
        public decimal? EvaluationCost { get; set; }

        /// <summary>
        /// 评标委员会--评标复议版本
        /// </summary>
        [StringLength(500)]
        public string EvaluationVersion { get; set; }
        #endregion

        #region 招标文件联审信息
        /// <summary>
        /// 招标文件联审--姓名
        /// </summary>
        [StringLength(200)]
        public string TenderFileAuditPersonName { get; set; }

        /// <summary>
        /// 招标文件联审--单位名称
        /// </summary>
        [StringLength(200)]
        public string TenderFileAuditPersonDeptName { get; set; }

        /// <summary>
        /// 招标文件联审--单位ID
        /// </summary>
        public int TenderFileAuditPersonDeptID { get; set; }

        /// <summary>
        /// 招标文件联审--费用
        /// </summary>
        public decimal? TenderFileAuditCost { get; set; }
        #endregion

        #region 澄清（修改）
        /// <summary>
        /// 澄清（修改）--提起人
        /// </summary>
        [StringLength(500)]
        public string ClarifyLaunchPerson { get; set; }

        /// <summary>
        /// 澄清（修改）--提出时间
        /// </summary>
        public DateTime? ClarifyLaunchDate { get; set; }

        /// <summary>
        /// 澄清（修改）--事由
        /// </summary>
        [StringLength(500)]
        public string ClarifyReason { get; set; }

        /// <summary>
        /// 澄清（修改）--受理时间
        /// </summary>
        public DateTime? ClarifyAcceptDate { get; set; }

        /// <summary>
        /// 澄清（修改）--处理人
        /// </summary>
        [StringLength(100)]
        public string ClarifyDisposePerson { get; set; }

        /// <summary>
        /// 澄清（修改）--澄清/修改
        /// </summary>
        [StringLength(100)]
        public string IsClarify { get; set; }

        /// <summary>
        /// 澄清（修改）--处理情况
        /// </summary>
        [StringLength(500)]
        public string ClarifyDisposeInfo { get; set; }

        /// <summary>
        /// 澄清（修改）--答复时间
        /// </summary>
        public DateTime? ClarifyReplyDate { get; set; }

        /// <summary>
        /// 澄清（修改）附件
        /// </summary>
        [StringLength(500)]
        public string ClarifyFile { get; set; }
        #endregion

        #region 异议处理
        /// <summary>
        /// 异议处理--提起人
        /// </summary>
        [StringLength(100)]
        public string DissentLaunchPerson { get; set; }

        /// <summary>
        /// 异议处理--提起人联系方式
        /// </summary>
        [StringLength(100)]
        public string DissentLaunchPersonPhone { get; set; }

        /// <summary>
        /// 异议处理--异议时间
        /// </summary>
        public DateTime? DissentLaunchDate { get; set; }

        /// <summary>
        /// 异议处理--事由
        /// </summary>
        [StringLength(500)]
        public string DissentReason { get; set; }

        /// <summary>
        /// 异议处理--受理时间
        /// </summary>
        public DateTime? DissentAcceptDate { get; set; }

        /// <summary>
        /// 异议处理--受理人
        /// </summary>
        [StringLength(100)]
        public string DissentAcceptPerson { get; set; }

        /// <summary>
        /// 异议处理--处理人
        /// </summary>
        [StringLength(100)]
        public string DissentDisposePerson { get; set; }

        /// <summary>
        /// 异议处理--处理情况
        /// </summary>
        [StringLength(500)]
        public string DissentDisposeInfo { get; set; }

        /// <summary>
        /// 异议处理--答复时间
        /// </summary>
        public DateTime? DissentReplyDate { get; set; }

        /// <summary>
        /// 异议处理--异议提出阶段
        /// </summary>
        [StringLength(500)]
        public string DissentProposedStage { get; set; }

        /// <summary>
        /// 异议处理--附件
        /// </summary>
        [StringLength(500)]
        public string DissentFile { get; set; }
        #endregion

        #region 项目前期沟通记录
        /// <summary>
        /// 对接人
        /// </summary>
        [StringLength(500)]
        public string ConnectPerson { get; set; }

        /// <summary>
        /// 对接时间
        /// </summary>
        public DateTime? ConnectDateTime { get; set; }

        /// <summary>
        /// 对接内容
        /// </summary>
        [StringLength(500)]
        public string ConnectContent { get; set; }

        /// <summary>
        /// 存在问题
        /// </summary>
        [StringLength(500)]
        public string ConnectExistingProblems { get; set; }
        #endregion

        #region 中标人信息
        /// <summary>
        /// 中标人名称
        /// </summary>
        [StringLength(200)]
        public string TenderSuccessPerson { get; set; }

        /// <summary>
        /// 中标候选人公示开始时间
        /// </summary>
        public DateTime? TenderSuccessPersonStartDate { get; set; }

        /// <summary>
        /// 中标候选人公示结束时间
        /// </summary>
        public DateTime? TenderSuccessPersonEndDate { get; set; }

        /// <summary>
        /// 中标人名称版本--评标复议
        /// </summary>
        [StringLength(200)]
        public string TenderSuccessPersonVersion { get; set; }
        #endregion
    }
}