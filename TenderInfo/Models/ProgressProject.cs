using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TenderInfo.Models
{
    [Table("ProgressProject")]
    public class ProgressProject
    {
        [Key]
        public int ProgressProjectID { get; set; }

        [StringLength(100)]
        public string ProgressType { get; set; }//工程、服务、劳保消防物资采购

        [StringLength(200)]
        public string ProjectName { get; set; }//拟招标项目名称

        [StringLength(200)]
        public string InvestPrice { get; set; }//投资金额（万元）

        [StringLength(200)]
        public string ProjectResponsiblePerson { get; set; }//招标项目负责人

        [StringLength(100)]
        public string ContractResponsiblePerson { get; set; }//合同签订部门项目责任人

        [StringLength(100)]
        public string ContractDeptContactDate { get; set; }//接到合同签订部门（电话或邮件）时间

        [StringLength(500)]
        public string ProjectExplain { get; set; }//项目对接需说明情况

        public DateTime? ProgramAcceptDate { get; set; }//招标方案接收时间--可空

        public DateTime? TenderFileSaleStartDate { get; set; }//发售招标文件开始日期--可空

        public DateTime? TenderFileSaleEndDate { get; set; }//发售招标文件截止日期--可空

        public DateTime? TenderStartDate { get; set; }//开标日期--可空

        public DateTime? TenderSuccessFileDate { get; set; }//中标通知书发出时间--可空

        [StringLength(1000)]
        public string OtherExplain { get; set; }//其他需说明情况

        [StringLength(1000)]
        public string Remark { get; set; }//备注

        [StringLength(100)]
        public string IsOver { get; set; }//是否已结束

        [StringLength(100)]
        public string YearInfo { get; set; }//年度信息
    }
}