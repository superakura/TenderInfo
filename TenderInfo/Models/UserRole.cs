using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TenderInfo.Models
{
    [Table("UserRole")]
    public class UserRole
    {
        /// <summary>
        /// 用户角色关系表ID
        /// </summary>
        [Key]
        public int UserRoleID { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        public int RoleID { get; set; }

        [ForeignKey("UserID")]
        public UserInfo UserInfo { get; set; }

        [ForeignKey("RoleID")]
        public RoleInfo RoleInfo { get; set; }
    }
}