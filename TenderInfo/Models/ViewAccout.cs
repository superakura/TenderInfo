using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TenderInfo.Models
{
    public class ViewAccout
    {
        public int AccountID { get; set; }
        public Models.Account account { get; set; }
        public List<AccountChild> accountChildFirst { get; set; }
        public List<AccountChild> accountChildSecond { get; set; }
        public List<AccountChild> accountChildThird { get; set; }
        public List<AccountChild> accountChildFour { get; set; }
        public List<AccountChild> accountChildFive { get; set; }
        public List<AccountChild> accountChildSix { get; set; }
    }
}