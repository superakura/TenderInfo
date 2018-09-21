using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TenderInfo.Models
{
    [Table("RoleInfo")]
    public class RoleInfo
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        [Key]
        public int RoleID { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        [Required,StringLength(50)]
        public string RoleName { get; set; }

        /// <summary>
        /// 角色描述
        /// </summary>
        [StringLength(100)]
        public string RoleDescribe { get; set; }
    }
}