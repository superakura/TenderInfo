using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TenderInfo.Models
{
    [Table("CheckReportFile")]
    public class CheckReportFile
    {
        /// <summary>
        /// 检验报告文件表主键ID
        /// </summary>
        [Key]
        public int CheckReportFileID { get; set; }

        /// <summary>
        /// 送样委托表ID
        /// </summary>
        public int SampleDelegationID { get; set; }

        /// <summary>
        /// 检验报告文件名
        /// </summary>
        [StringLength(500)]
        public string CheckReportFileName { get; set; }

        /// <summary>
        /// 检验报告上传人员ID
        /// </summary>
        public int CheckReportInputPerson { get; set; }

        /// <summary>
        /// 检验报告上传人员姓名
        /// </summary>
        [StringLength(100)]
        public string CheckReportInputPersonName { get; set; }

        /// <summary>
        /// 检验报告上传时间
        /// </summary>
        public DateTime? CheckReportInputDate { get; set; }
    }
}