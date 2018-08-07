using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TenderInfo.Models
{
    [Table("Log")]
    public class Log
    {
        [Key]
        public int LogID { get; set; }

        [Required]
        [StringLength(100)]
        public string LogType { get; set; }//日志记录类型

        [Required]
        [StringLength(500)]
        public string LogContent { get; set; }//日志内容

        public int LogDataID { get; set; }//日志操作数据ID

        [StringLength(500)]
        public string LogReason { get; set; }//日志产生原因

        public int InputPersonID { get; set; }//操作人员ID

        [StringLength(50)]
        public string InputPersonName { get; set; }//操作人员姓名

        public DateTime InputDateTime { get; set; }//操作日期

        [StringLength(500)]
        public string Col1 { get; set; }//备用字段1

        [StringLength(500)]
        public string Col2 { get; set; }//备用字段2

        [StringLength(500)]
        public string Col3 { get; set; }//备用字段3

        [StringLength(500)]
        public string Col4 { get; set; }//备用字段4

        [StringLength(500)]
        public string Col5 { get; set; }//备用字段5
    }
}