using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TenderInfo.Controllers
{
    public class AccountController : Controller
    {
        private Models.DB db = new Models.DB();
        // GET: Account
        public ViewResult Material()
        {
            return View();
        }

        public ViewResult Project()
        {
            return View();
        }

        public ViewResult Service()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetMaterialList()
        {
            try
            {
                var limit = 0;
                int.TryParse(Request.Form["limit"], out limit);
                var offset = 0;
                int.TryParse(Request.Form["offset"], out offset);

                var result = from m in db.AccountMaterial
                             select m;
                return Json(new { total = result.Count(), rows = result.OrderBy(o => o.AccountMaterialID).Skip(offset).Take(limit).ToList() });
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
    }
}