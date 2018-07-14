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
        [Key]
        public int UserRoleID { get; set; }//用户角色关系表ID

        public int UserID { get; set; }//用户ID

        public int RoleID { get; set; }//角色ID

        [ForeignKey("UserID")]
        public UserInfo UserInfo { get; set; }

        [ForeignKey("RoleID")]
        public RoleInfo RoleInfo { get; set; }
    }
}