using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TenderInfo.Models
{
    public class LogView
    {
        Models.DB db = new DB();
        public IEnumerable<Log> logList
        {
            get { return db.Log; }
        }
    }
}