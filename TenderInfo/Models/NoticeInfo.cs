using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TenderInfo.Models
{
    [Table("NoticeInfo")]
    public class NoticeInfo
    {
        [Key]
        public int NoticeID { get; set; }//通知公告ID

        [Required]
        [StringLength(200)]
        public string NoticeTitle { get; set; }//通知公告标题

        [Required]
        [StringLength(1000)]
        public string Content { get; set; }//通知公告内容，最多1000字

        [StringLength(50)]
        public string ContentType { get; set; }//通知公告类型，【可空】

        [Required]
        public int ContentCount { get; set; } //通知内容字数
        
        [Required]
        public int InsertPersonID { get; set; }//通知公告发布人员ID

        [Required]
        public DateTime InsertDate { get; set; }//通知公告发布日期

        [ForeignKey("InsertPersonID")]
        public UserInfo UserInfo { get; set; }
    }
}