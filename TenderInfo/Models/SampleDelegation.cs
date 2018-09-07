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
        /// <summary>
        /// 送样委托表主键ID
        /// </summary>
        [Key]
        public int SampleDelegationID { get; set; }

        /// <summary>
        /// 样品名称
        /// </summary>
        [StringLength(200)]
        public string SampleName { get; set; }

        /// <summary>
        /// 样品数量
        /// </summary>
        public int? SampleNum { get; set; }

        /// <summary>
        /// 送样委托状态
        /// 技术要求录入、质检接收审核、质检接收回退、质检领导确认、检验报告上传、全否
        /// </summary>
        [StringLength(200)]
        public string SampleDelegationState { get; set; }

        /// <summary>
        /// 送样检验技术要求--技术人员填写
        /// </summary>
        [StringLength(2000)]
        public string SampleTechnicalRequirement { get; set; }

        /// <summary>
        /// 开标时间
        /// </summary>
        public DateTime? StartTenderDate { get; set; }

        /// <summary>
        /// 技术接收人ID
        /// </summary>
        public int SampleDelegationAcceptPerson { get; set; }

        /// <summary>
        /// 招标项目负责人ID
        /// </summary>
        public int ProjectResponsiblePerson { get; set; }

        #region 一次编码表
        /// <summary>
        /// 一次编码表文件名
        /// </summary>
        [StringLength(500)]
        public string FirstCodingFileName { get; set; }

        /// <summary>
        /// 一次编码表上传人员ID
        /// </summary>
        public int FirstCodingInputPerson { get; set; }

        /// <summary>
        /// 一次编码表上传人员姓名
        /// </summary>
        [StringLength(100)]
        public string FirstCodingInputPersonName { get; set; }

        /// <summary>
        /// 一次编码表上传时间
        /// </summary>
        public DateTime? FirstCodingInputDate { get; set; }
        #endregion

        #region 二次编码表
        /// <summary>
        /// 二次编码表文件名
        /// </summary>
        [StringLength(500)]
        public string SecondCodingFileName { get; set; }

        /// <summary>
        /// 二次编码表上传人员ID
        /// </summary>
        public int SecondCodingInputPerson { get; set; }

        /// <summary>
        /// 二次编码表上传人员姓名
        /// </summary>
        [StringLength(100)]
        public string SecondCodingInputPersonName { get; set; }

        /// <summary>
        /// 二次编码表上传时间
        /// </summary>
        public DateTime? SecondCodingInputDate { get; set; }
        #endregion

        /// <summary>
        /// 添加人员ID
        /// </summary>
        public int InputPerson { get; set; }

        /// <summary>
        /// 添加人员姓名
        /// </summary>
        [StringLength(100)]
        public string InputPersonName { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime? InputDateTime { get; set; }

        /// <summary>
        /// 修改开标时间状态，【修改中、空值】
        /// </summary>
        [StringLength(100)]
        public string ChangeStartTenderDateState { get; set; }

        /// <summary>
        /// 检验报告全否选项，【全否、空值】
        /// </summary>
        [StringLength(100)]
        public string CheckResultAllError { get; set; }
    }
}