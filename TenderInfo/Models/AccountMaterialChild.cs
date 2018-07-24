using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TenderInfo.Models
{
    [Table("AccountMaterialChild")]
    public class AccountMaterialChild
    {
        [Key]
        public int AccountMaterialChildID { get; set; }

        [StringLength(200)]
        public string TenderFilePlanPayPerson { get; set; }//购买招标文件潜在投标人

        [StringLength(200)]
        public string TenderPerson { get; set; }//投标人

        [StringLength(200)]
        public string ProductManufacturer { get; set; }//产品制造商（代理、贸易商投标时填写）

        [StringLength(200)]
        public string QuotedPriceUnit { get; set; }//报价（元）初步评审被否决时在此栏说明--单价

        [StringLength(200)]
        public string QuotedPriceSum { get; set; }//报价（元）初步评审被否决时在此栏说明--总价

        [StringLength(100)]
        public string EvaluationPersonName { get; set; }//评标委员会--姓名

        [StringLength(200)]
        public string EvaluationPersonDept { get; set; }//评标委员会--单位

        [StringLength(100)]
        public string IsEvaluationDirector { get; set; }//评标委员会--是否评标委员会主任

        [StringLength(200)]
        public string EvaluationCost { get; set; }//评标委员会--评审费

        [StringLength(200)]
        public string TenderFileAuditPersonName { get; set; }//招标文件联审--姓名

        [StringLength(200)]
        public string TenderFileAuditPersonDept { get; set; }//招标文件联审--单位

        [StringLength(200)]
        public string TenderFileAuditCost { get; set; }//招标文件联审--费用

        public DateTime InputDate { get; set; }//信息录入日期时间

        public int InputPerson { get; set; }//信息录入人员
    }
}