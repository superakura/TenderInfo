using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TenderInfo.Models
{
    [Table("AuthorityInfo")]
    public class AuthorityInfo
    {
        [Key]
        public int AuthorityID { get; set; }//权限信息表ID

        [Required]
        [StringLength(50)]
        public string AuthorityName { get; set; }//权限名称

        [StringLength(100)]
        public string AuthorityDescribe { get; set; }//权限描述

        [Required]
        [StringLength(50)]
        public string AuthorityType { get; set; }//权限类型【菜单】【功能】

        public int ConflictCode { get; set; }//权限互斥检测，例如会计和出纳的互斥码都为1，那么建立角色时不能同时选择这2个权限。

        [StringLength(100)]
        public string MenuUrl { get; set; }//菜单地址

        [Required]
        public int? MenuOrder { get; set; }//菜单顺序码

        [Required]
        public int? MenuFatherID { get; set; }//上级菜单ID

        [StringLength(50)]
        public string MenuIcon { get; set; }//菜单图标

        [StringLength(20)]
        public string MenuName { get; set; }//菜单名称
    }
}