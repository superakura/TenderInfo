using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TenderInfo.Models
{
    /// <summary>
    /// 招标组长信息表
    /// </summary>
    [Table("GroupLeader")]
    public class GroupLeader
    {
        /// <summary>
        /// 招标组长信息表ID
        /// </summary>
        [Key]
        public int GroupLeaderID { get; set; }

        /// <summary>
        /// 组长用户信息ID
        /// </summary>
        public int LeaderUserID { get; set; }

        /// <summary>
        /// 组内成员ID
        /// </summary>
        public int MemberUserID { get; set; }
    }
}