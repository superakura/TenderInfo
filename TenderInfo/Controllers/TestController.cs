using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TenderInfo.Controllers
{
    public class TestController : Controller
    {
        private Models.DB db = new Models.DB();
        // GET: Test
        public ViewResult FixColumnBsTable()
        {
            return View();
        }

        public ViewResult LogInfo(IEnumerable<Models.Log> logView)
        {
            logView = db.Log.ToList();
            return View(logView);
        }
    }
}