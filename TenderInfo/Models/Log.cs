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

        /// <summary>
        /// 日志记录类型，非空
        /// </summary>
        [Required][StringLength(100)]
        public string LogType { get; set; }

        /// <summary>
        /// 日志内容，非空
        /// </summary>
        [Required][StringLength(500)]
        public string LogContent { get; set; }

        /// <summary>
        /// 日志操作数据ID，非空
        /// </summary>
        public int LogDataID { get; set; }

        /// <summary>
        /// 日志产生原因，可空
        /// </summary>
        [StringLength(500)]
        public string LogReason { get; set; }

        /// <summary>
        /// 操作人员ID，非空
        /// </summary>
        public int InputPersonID { get; set; }

        /// <summary>
        /// 操作人员姓名，可空
        /// </summary>
        [StringLength(50)]
        public string InputPersonName { get; set; }

        /// <summary>
        /// 操作日期时间，非空
        /// </summary>
        public DateTime InputDateTime { get; set; }

        /// <summary>
        /// 备用字段1，可空
        /// </summary>
        [StringLength(500)]
        public string Col1 { get; set; }

        /// <summary>
        /// 备用字段2，可空
        /// </summary>
        [StringLength(500)]
        public string Col2 { get; set; }

        /// <summary>
        /// 备用字段2，可空
        /// </summary>
        [StringLength(500)]
        public string Col3 { get; set; }

        /// <summary>
        /// 备用字段2，可空
        /// </summary>
        [StringLength(500)]
        public string Col4 { get; set; }

        /// <summary>
        /// 备用字段2，可空
        /// </summary>
        [StringLength(500)]
        public string Col5 { get; set; }
    }
}