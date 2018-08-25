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
        [StringLength(200)]
        public string QuotedPriceUnit { get; set; }//

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
        [StringLength(200)]
        public string EvaluationTime { get; set; }

        /// <summary>
        /// 评标委员会--评审费
        /// </summary>
        [StringLength(200)]
        public string EvaluationCost { get; set; }
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
        [StringLength(200)]
        public string TenderFileAuditCost { get; set; }
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
        #endregion

        /// <summary>
        ///框架附属文件
        /// </summary>
        [StringLength(500)]
        public string  FrameFile{ get; set; }

        /// <summary>
        /// 信息录入日期时间
        /// </summary>
        public DateTime InputDate { get; set; }

        /// <summary>
        /// 信息录入人员ID
        /// </summary>
        public int InputPerson { get; set; }
    }
}