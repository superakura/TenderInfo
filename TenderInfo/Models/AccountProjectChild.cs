using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TenderInfo.Models
{
    [Table("AccountProjectChild")]
    public class AccountProjectChild
    {
        [Key]
        public int AccountProjectChildID { get; set; }

        [StringLength(200)]
        public string TenderFilePlanPayPerson { get; set; }//购买招标文件潜在投标人

        [StringLength(200)]
        public string TenderPerson { get; set; }//投标人

        [StringLength(200)]
        public string QuotedPrice { get; set; }//报价（万元）

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