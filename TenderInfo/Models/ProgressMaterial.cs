using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TenderInfo.Models
{
    [Table("ProgressMaterial")]
    public class ProgressMaterial
    {
        [Key]
        public int ProgressMaterialID { get; set; }

        [StringLength(100)]
        public string ProgressType { get; set; }//物资、框架

        [StringLength(100)]
        public string ProgressTypeChild { get; set; }//子分类（动设备、静设备、化工三剂等）

        [StringLength(200)]
        public string ProjectName { get; set; }//拟招标项目名称或物资名称

        [StringLength(200)]
        public string InvestPrice { get; set; }//投资金额（万元）

        [StringLength(200)]
        public string MaterialCount { get; set; }//物资数量

        [StringLength(200)]
        public string ProjectResponsiblePerson { get; set; }//招标项目负责人

        [StringLength(100)]
        public string ContractResponsiblePerson { get; set; }//合同签订部门项目责任人

        [StringLength(500)]
        public string TechnicalSpecificationAddDate { get; set; }//技术规格书（技术文件、图纸等）提报时间--用户提出需要文字说明

        [StringLength(500)]
        public string TechnicalSpecificationExplain { get; set; }//技术规格书（技术文件、图纸等）对接需说明情况

        public DateTime? TechnicalSpecificationApproveDate { get; set; }//技术规格书（技术文件、图纸等）定稿时间

        public DateTime? SynthesizeEvaluationRuleApproveDate { get; set; }//综合法评标标准定稿时间

        public DateTime? TenderProgramAuditDate { get; set; }//招标方案联审时间--可空

        public DateTime? ProgramAcceptDate { get; set; }//招标方案接收时间--可空

        public DateTime? TenderFileSaleStartDate { get; set; }//发售招标文件开始日期--可空

        public DateTime? TenderFileSaleEndDate { get; set; }//发售招标文件截止日期--可空

        public DateTime? TenderStartDate { get; set; }//开标日期--可空

        public DateTime? TenderSuccessFileDate { get; set; }//中标通知书发出时间--可空

        [StringLength(1000)]
        public string OtherExplain { get; set; }//其他需说明情况

        [StringLength(1000)]
        public string Remark { get; set; }//备注
    }
}