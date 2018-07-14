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
        [Key]
        public int RoleAuthorityID { get; set; }//权限角色关系表ID

        public int RoleID { get; set; }//角色ID

        public int AuthorityID { get; set; }//权限ID

        [ForeignKey("RoleID")]
        public RoleInfo RoleInfo { get; set; }

        [ForeignKey("AuthorityID")]
        public AuthorityInfo AuthorityInfo { get; set; }
    }
}
