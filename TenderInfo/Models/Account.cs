using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TenderInfo.Models
{
    [Table("Account")]
    public class Account
    {
        [Key]
        public int AccountID { get; set; }//招标台账信息ID

        /// <summary>
        /// 项目类型，物资、框架、工程、服务
        /// </summary>
        [StringLength(50)]
        public string ProjectType { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        [Required][StringLength(200)]
        public string ProjectName { get; set; }

        /// <summary>
        /// 项目文件编号
        /// </summary>
        [StringLength(200)]
        public string TenderFileNum { get; set; }

        /// <summary>
        /// 
        /// </summary>线上/线下
        [StringLength(200)]
        public string IsOnline { get; set; }

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
        /// 使用单位名称--物资、框架
        /// </summary>
        [StringLength(200)]
        public string UsingDeptName { get; set; }

        /// <summary>
        /// 使用单位ID--物资、框架
        /// </summary>
        public int UsingDeptID { get; set; }

        /// <summary>
        /// 项目主责部门名称
        /// </summary>
        [StringLength(200)]
        public string ProjectResponsibleDeptName { get; set; }

        /// <summary>
        /// 项目主责部门ID
        /// </summary>
        public int ProjectResponsibleDeptID { get; set; }

        /// <summary>
        /// 申请人
        /// </summary>
        [StringLength(200)]
        public string ApplyPerson { get; set; }

        /// <summary>
        /// 投资计划批复文号
        /// </summary>
        [StringLength(200)]
        public string InvestPlanApproveNum { get; set; }

        /// <summary>
        /// 资金来源--工程、服务
        /// </summary>
        [StringLength(200)]
        public string InvestSource { get; set; }

        /// <summary>
        /// 招标范围
        /// </summary>
        [StringLength(200)]
        public string TenderRange { get; set; }

        /// <summary>
        /// 是否带量--框架
        /// </summary>
        [StringLength(200)]
        public string IsHaveCount { get; set; }

        /// <summary>
        /// 工期--工程、服务
        /// </summary>
        [StringLength(200)]
        public string ProjectTimeLimit { get; set; }

        /// <summary>
        /// 招标方式
        /// </summary>
        [StringLength(200)]
        public string TenderMode { get; set; }

        /// <summary>
        /// 评标方法
        /// </summary>
        [StringLength(200)]
        public string BidEvaluation { get; set; }

        /// <summary>
        /// 供货期
        /// </summary>
        [StringLength(200)]
        public string SupplyPeriod { get; set; }

        /// <summary>
        /// 招标方案联审时间--可空
        /// </summary>
        public DateTime? TenderProgramAuditDate { get; set; }

        /// <summary>
        /// 收到方案日期--【物资、框架】 收到申请日期（线上项目填方案分派日期）--【工程、服务】
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
        /// 中标人名称
        /// </summary>
        [StringLength(200)]
        public string TenderSuccessPerson { get; set; }

        /// <summary>
        /// 预计投资（万元）
        /// </summary>
        public decimal? PlanInvestPrice { get; set; }

        /// <summary>
        /// 招标控制价--单价
        /// </summary>
        public decimal? TenderRestrictUnitPrice { get; set; }

        /// <summary>
        /// 招标控制价--总价（万元）【物资、框架】招标控制价（万元）【工程、服务】
        /// </summary>
        public decimal? TenderRestrictSumPrice { get; set; }

        /// <summary>
        /// 中标金额--单价
        /// </summary>
        public decimal? TenderSuccessUnitPrice { get; set; }

        /// <summary>
        /// 【物资、框架】中标金额（万元）--总价【工程、服务】中标金额（万元）
        /// </summary>
        public decimal? TenderSuccessSumPrice { get; set; }

        /// <summary>
        /// 【物资、框架】与控制价比节约资金（万元）【工程、服务】节约资金
        /// </summary>
        public decimal? SaveCapital { get; set; }

        /// <summary>
        /// 招标文件联审--联审时间（小时）
        /// </summary>
        public decimal? TenderFileAuditTime { get; set; }

        /// <summary>
        /// 招标失败原因
        /// </summary>
        [StringLength(500)]
        public string TenderFailReason { get; set; }

        /// <summary>
        /// 合同编号
        /// </summary>
        [StringLength(100)]
        public string ContractNum { get; set; }

        /// <summary>
        /// 合同金额
        /// </summary>
        public decimal? ContractPrice { get; set; }

        /// <summary>
        /// 相对人名称
        /// </summary>
        [StringLength(100)]
        public string RelativePerson { get; set; }

        /// <summary>
        /// 招标情况
        /// </summary>
        [StringLength(500)]
        public string TenderInfo { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string TenderRemark { get; set; }

        /// <summary>
        /// 台账信息变更日期时间
        /// </summary>
        public DateTime? InputDate { get; set; }

        /// <summary>
        /// 台账信息变更人员ID
        /// </summary>
        public int InputPersonID { get; set; }

        /// <summary>
        /// 建立台账信息日期时间
        /// </summary>
        public DateTime? InsertDate { get; set; }

        /// <summary>
        /// 建立台账信息人员
        /// </summary>
        public int InsertPersonID { get; set; }

        /// <summary>
        /// 招标进度ID
        /// </summary>
        public int? ProgressID { get; set; }

        /// <summary>
        /// 是否与招标进度数据完成同步
        /// </summary>
        [StringLength(50)]
        public string IsSynchro { get; set; }
    }
}