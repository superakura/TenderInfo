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
        [Key]
        public int RoleID { get; set; }//角色ID

        [Required]
        [StringLength(50)]
        public string RoleName { get; set; }//角色名称

        [StringLength(100)]
        public string RoleDescribe { get; set; }//角色描述
    }
}