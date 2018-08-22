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

        [StringLength(200)]
        public string TableType { get; set; }//TableType,【first】【second】【third】

        [StringLength(200)]
        public string TenderFilePlanPayPerson { get; set; }//购买招标文件潜在投标人

        [StringLength(200)]
        public string TenderPerson { get; set; }//投标人

        [StringLength(200)]
        public string ProductManufacturer { get; set; }//产品制造商（代理、贸易商投标时填写）

        [StringLength(200)]
        public string QuotedPriceUnit { get; set; }//报价（元）--单价

        //报价（万元）--工程、服务
        //报价（万元）--总价--物资、框架
        [StringLength(200)]
        public string QuotedPriceSum { get; set; }

        [StringLength(500)]
        public string NegationExplain { get; set; }//初步评审被否决时在此栏说明

        [StringLength(100)]
        public string EvaluationPersonName { get; set; }//评标委员会--姓名

        [StringLength(200)]
        public string EvaluationPersonDeptName { get; set; }//评标委员会--单位名称

        public int EvaluationPersonDeptID { get; set; }//评标委员会--单位ID

        [StringLength(100)]
        public string IsEvaluationDirector { get; set; }//评标委员会--是否评标委员会主任

        [StringLength(200)]
        public string EvaluationTime { get; set; }//评标委员会--评审时间（小时）

        [StringLength(200)]
        public string EvaluationCost { get; set; }//评标委员会--评审费


        [StringLength(200)]
        public string TenderFileAuditPersonName { get; set; }//招标文件联审--姓名

        [StringLength(200)]
        public string TenderFileAuditPersonDeptName { get; set; }//招标文件联审--单位名称

        public int TenderFileAuditPersonDeptID { get; set; }//招标文件联审--单位ID

        [StringLength(200)]
        public string TenderFileAuditTime { get; set; }//招标文件联审--联审时间（小时）--预设，暂时不用

        [StringLength(200)]
        public string TenderFileAuditCost { get; set; }//招标文件联审--费用

        public DateTime InputDate { get; set; }//信息录入日期时间

        public int InputPerson { get; set; }//信息录入人员
    }
}