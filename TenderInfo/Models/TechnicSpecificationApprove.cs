using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TenderInfo.Models
{
    [Table("TechnicSpecificationApprove")]
    public class TechnicSpecificationApprove
    {
        [Key]
        public int TechnicSpecificationApproveID { get; set; }

        [StringLength(100)]
        public string TechnicSpecificationType { get; set; }//技术规格书类型

        public int ApplyPerson { get; set; }//申请人员

        public DateTime? ApplyDate { get; set; }//申请时间

        public int ApprovePerson { get; set; }//审批人员

        public DateTime? ApproveDate { get; set; }//审批时间

        public int InputPerson { get; set; }//上传人员

        public DateTime InputDate { get; set; }//上传时间
    }
}