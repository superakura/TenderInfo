using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TenderInfo.Models
{
    [Table("AccountMaterial")]
    public class AccountMaterial
    {
        [Key]
        public int AccountMaterialID { get; set; }//招标（物资）台账信息ID

        [Required]
        [StringLength(200)]
        public string ProjectName { get; set; }//项目名称

        [StringLength(200)]
        public string TenderFileNum { get; set; }//项目文件编号

        [StringLength(200)]
        public string IsOnline { get; set; }//线上/线下

        [StringLength(200)]
        public string ProjectResponsiblePersonName { get; set; }//招标项目负责人姓名

        public int ProjectResponsiblePersonID { get; set; }//招标项目负责人ID

        [StringLength(200)]
        public string UsingDept { get; set; }//使用单位

        [StringLength(200)]
        public string ProjectResponsibleDept { get; set; }//项目主责部门

        [StringLength(200)]
        public string ApplyPerson { get; set; }//申请人

        [StringLength(200)]
        public string InvestPlanApproveNum { get; set; }//投资计划批复文号

        [StringLength(200)]
        public string TenderRange { get; set; }//招标范围

        [StringLength(200)]
        public string TenderMode { get; set; }//招标方式

        [StringLength(200)]
        public string BidEvaluation { get; set; }//评标方法

        [StringLength(200)]
        public string SupplyPeriod { get; set; }//供货期

        public DateTime? TenderProgramAuditDate { get; set; }//招标方案联审时间--可空

        public DateTime? ProgramAcceptDate { get; set; }//收到方案日期--可空

        public DateTime? TenderFileSaleStartDate { get; set; }//发售招标文件开始日期--可空

        public DateTime? TenderFileSaleEndDate { get; set; }//发售招标文件截止日期--可空

        public DateTime? TenderStartDate { get; set; }//开标日期--可空

        public DateTime? TenderSuccessFileDate { get; set; }//中标通知书发出时间--可空

        [StringLength(200)]
        public string TenderSuccessPerson { get; set; }//中标人名称

        [StringLength(200)]
        public string PlanInvestPrice { get; set; }//预计投资（招标控制总价）（元）

        [StringLength(200)]
        public string TenderRestrictUnitPrice { get; set; }//招标控制单价

        [StringLength(200)]
        public string TenderSuccessUnitPrice { get; set; }//中标金额（元）--单价

        [StringLength(200)]
        public string TenderSuccessSumPrice { get; set; }//中标金额（元）--总价

        [StringLength(200)]
        public string SaveCapital { get; set; }//与控制价比节约资金（元）

        [StringLength(200)]
        public string EvaluationTime { get; set; }//评标委员会--评审时间（小时）

        [StringLength(200)]
        public string TenderFileAuditTime { get; set; }//招标文件联审--联审时间（小时）

        [StringLength(500)]
        public string TenderFailReason { get; set; }//招标失败原因

        [StringLength(500)]
        public string ClarifyLaunchPerson { get; set; }//澄清（修改）--提起人

        public DateTime? ClarifyLaunchDate { get; set; }//澄清（修改）--提出时间

        [StringLength(500)]
        public string ClarifyReason { get; set; }//澄清（修改）--事由

        public DateTime? ClarifyAcceptDate { get; set; }//澄清（修改）--受理时间

        [StringLength(100)]
        public string ClarifyDisposePerson { get; set; }//澄清（修改）--处理人

        [StringLength(100)]
        public string IsClarify { get; set; }//澄清（修改）--澄清/修改

        [StringLength(500)]
        public string ClarifyDisposeInfo { get; set; }//澄清（修改）--处理情况

        public DateTime? ClarifyReplyDate { get; set; }//澄清（修改）--答复时间

        [StringLength(100)]
        public string DissentLaunchPerson { get; set; }//异议处理--提起人

        [StringLength(100)]
        public string DissentLaunchPersonPhone { get; set; }//异议处理--提起人联系方式

        public DateTime? DissentLaunchDate { get; set; }//异议处理--异议时间

        [StringLength(500)]
        public string DissentReason { get; set; }//异议处理--事由

        public DateTime? DissentAcceptDate { get; set; }//异议处理--受理时间

        [StringLength(100)]
        public string DissentAcceptPerson { get; set; }//异议处理--受理人

        [StringLength(100)]
        public string DissentDisposePerson { get; set; }//异议处理--处理人

        [StringLength(500)]
        public string DissentDisposeInfo { get; set; }//异议处理--处理情况

        public DateTime? DissentReplyDate { get; set; }//异议处理--答复时间

        [StringLength(100)]
        public string ContractNum { get; set; }//合同编号

        [StringLength(100)]
        public string ContractPrice { get; set; }//合同金额

        [StringLength(100)]
        public string RelativePerson { get; set; }//相对人名称

        [StringLength(500)]
        public string TenderInfo { get; set; }//招标情况

        [StringLength(500)]
        public string TenderRemark { get; set; }//备注

        public DateTime? InputDate { get; set; }//台账信息变更日期时间

        public int InputPersonID { get; set; }//台账信息变更人员ID

        public DateTime? InsertDate { get; set; }//建立台账信息日期时间

        public int InsertPersonID { get; set; }//建立台账信息人员
    }        
}