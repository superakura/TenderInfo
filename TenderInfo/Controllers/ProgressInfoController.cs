using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TenderInfo.Controllers
{
    public class ProgressInfoController : Controller
    {
        //ProgressInfo
        private Models.DB db = new Models.DB();

        public ViewResult ProgressMaterial()
        {
            return View();
        }

        public ViewResult ProgressProject()
        {
            return View();
        }

        public ViewResult ProgressFrame()
        {
            return View();
        }

        public JsonResult GetList()
        {
            var limit = 0;
            int.TryParse(Request.Form["limit"], out limit);
            var offset = 0;
            int.TryParse(Request.Form["offset"], out offset);
            var sampleName = Request.Form["sampleName"];//样品名称

            var result = from p in db.ProgressInfo
                         orderby p.ProgressInfoID
                         select p;
            return Json(new { total = result.Count(), rows = result.Skip(offset).Take(limit).ToList() });
        }
    }
}