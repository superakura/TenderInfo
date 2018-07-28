using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TenderInfo.Models
{
    [Table("SampleDelegation")]
    public class SampleDelegation
    {
        [Key]
        public int SampleDelegationID { get; set; }

        [StringLength(200)]
        public string ProjectName { get; set; }//项目名称

        [StringLength(200)]
        public string SampleName { get; set; }//样品名称

        public DateTime? StartTenderDate { get; set; }//开标时间

        public int SampleDelegationAcceptPerson { get; set; }//技术处送样委托单接收人

        public int ProjectResponsiblePerson { get; set; }//招标项目负责人

        [StringLength(500)]
        public string FirstCodingFileName { get; set; }//一次编码表文件名

        public int FirstCodingInputPerson { get; set; }//一次编码表上传人员

        [StringLength(100)]
        public string FirstCodingInputPersonName { get; set; }//一次编码表上传人员姓名

        public DateTime? FirstCodingInputDate { get; set; }//一次编码表上传时间

        [StringLength(500)]
        public string SecondCodingFileName { get; set; }//二次编码表文件名

        public int SecondCodingInputPerson { get; set; }//二次编码表上传人员

        [StringLength(100)]
        public string SecondCodingInputPersonName { get; set; }//二次编码表上传人员姓名

        public DateTime? SecondCodingInputDate { get; set; }//二次编码表上传时间

        [StringLength(500)]
        public string SampleDelegationFileName { get; set; }//送样委托单文件名

        public int SampleDelegationInputPerson { get; set; }//送样委托单上传人员

        [StringLength(100)]
        public string SampleDelegationInputPersonName { get; set; }//送样委托单上传人员姓名

        public DateTime? SampleDelegationInputDate { get; set; }//送样委托单上传时间

        [StringLength(500)]
        public string SampleDelegationFileNameOne { get; set; }//送样委托单文件名One

        public int SampleDelegationInputPersonOne { get; set; }//送样委托单上传人员One

        [StringLength(100)]
        public string SampleDelegationInputPersonNameOne { get; set; }//送样委托单上传人员姓名One

        public DateTime? SampleDelegationInputDateOne { get; set; }//送样委托单上传时间One

        [StringLength(500)]
        public string CheckReportFileName { get; set; }//检验报告文件名

        public int CheckReportInputPerson { get; set; }//检验报告上传人员

        [StringLength(100)]
        public string CheckReportInputPersonName { get; set; }//检验报告上传人员姓名

        public DateTime? CheckReportInputDate { get; set; }//检验报告上传时间

        public string SampleDelegationState { get; set; }//送样委托状态
    }
}