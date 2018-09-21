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
        /// <summary>
        /// 权限信息表ID
        /// </summary>
        [Key]
        public int AuthorityID { get; set; }

        /// <summary>
        /// 权限名称
        /// </summary>
        [Required,StringLength(50)]
        public string AuthorityName { get; set; }

        /// <summary>
        /// 权限描述
        /// </summary>
        [StringLength(100)]
        public string AuthorityDescribe { get; set; }

        /// <summary>
        /// 权限类型【菜单】【功能】
        /// </summary>
        [Required,StringLength(50)]
        public string AuthorityType { get; set; }

        /// <summary>
        /// 权限互斥检测，例如会计和出纳的互斥码都为1，那么建立角色时不能同时选择这2个权限。
        /// </summary>
        public int ConflictCode { get; set; }

        /// <summary>
        /// 菜单地址
        /// </summary>
        [StringLength(100)]
        public string MenuUrl { get; set; }

        /// <summary>
        /// 菜单顺序码
        /// </summary>
        [Required]
        public int? MenuOrder { get; set; }

        /// <summary>
        /// 上级菜单ID
        /// </summary>
        [Required]
        public int? MenuFatherID { get; set; }

        /// <summary>
        /// 菜单图标
        /// </summary>
        [StringLength(50)]
        public string MenuIcon { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        [StringLength(20)]
        public string MenuName { get; set; }
    }
}