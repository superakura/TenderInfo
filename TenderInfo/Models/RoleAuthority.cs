using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TenderInfo.Models
{
    [Table("RoleAuthority")]
    public class RoleAuthority
    {
        /// <summary>
        /// 权限角色关系表ID
        /// </summary>
        [Key]
        public int RoleAuthorityID { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        public int RoleID { get; set; }

        /// <summary>
        /// 权限ID
        /// </summary>
        public int AuthorityID { get; set; }

        [ForeignKey("RoleID")]
        public RoleInfo RoleInfo { get; set; }

        [ForeignKey("AuthorityID")]
        public AuthorityInfo AuthorityInfo { get; set; }
    }
}
