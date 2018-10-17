using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TenderInfo.Controllers
{
    public class RoleInfoController : Controller
    {
        private Models.DB db = new Models.DB();

        public ViewResult Index()
        {
            return View();
        }

        public ViewResult RoleInfoAddOrEdit()
        {
            return View();
        }

        [HttpPost]
        public string Insert()
        {
            try
            {
                var infoList =
   JsonConvert.DeserializeObject<Dictionary<String, Object>>(HttpUtility.UrlDecode(Request.Form.ToString()));
                var name = infoList["name"].ToString();
                var desc = infoList["desc"].ToString();
                var authorityList =
                    JsonConvert.DeserializeObject<Dictionary<String, Object>>(infoList["authorityList"].ToString());
                Models.RoleInfo roleInfo = new Models.RoleInfo();
                roleInfo.RoleName = name;
                roleInfo.RoleDescribe = desc;
                db.RoleInfo.Add(roleInfo);
                db.SaveChanges();
                var roleID = roleInfo.RoleID;
                foreach (var item in authorityList)
                {
                    Models.RoleAuthority roleAuthority = new Models.RoleAuthority();
                    roleAuthority.AuthorityID = Convert.ToInt32(item.Value);
                    roleAuthority.RoleID = roleID;
                    db.RoleAuthority.Add(roleAuthority);
                    db.SaveChanges();
                }
                return "ok";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        [HttpPost]
        public JsonResult GetList()
        {
            var limit = Convert.ToInt32(Request.Form["limit"]);
            var offset = Convert.ToInt32(Request.Form["offset"]);
            var roleName = Request.Form["roleNameSearch"];
            var result = db.RoleInfo.OrderBy(o => o.RoleName).AsQueryable();

            if (!string.IsNullOrEmpty(roleName))
            {
                result = result.Where(w => w.RoleName.Contains(roleName));
            }
            return Json(new { total = result.Count(), rows = result.Skip(offset).Take(limit).ToList() });
        }

        [HttpPost]
        public JsonResult GetListAll()
        {
            return Json(db.RoleInfo.OrderBy(o => o.RoleName).ToList());
        }

        [HttpPost]
        public JsonResult GetOne()
        {
            var postList =
  JsonConvert.DeserializeObject<Dictionary<String, Object>>(HttpUtility.UrlDecode(Request.Form.ToString()));
            var id = Convert.ToInt32(postList["id"].ToString());
            var roleInfo = db.RoleInfo.Find(id);
            var roleAuthority = db.RoleAuthority.Where(w => w.RoleID == id).ToList();
            Dictionary<string, object> result = new Dictionary<string, object>();
            result.Add("roleInfo", roleInfo);
            result.Add("roleAuthority", roleAuthority);
            return Json(result);
        }

        [HttpPost]
        public string Update()
        {
            try
            {
                var infoList =
   JsonConvert.DeserializeObject<Dictionary<String, Object>>(HttpUtility.UrlDecode(Request.Form.ToString()));
                var roleID = Convert.ToInt32(infoList["id"].ToString());
                var name = infoList["name"].ToString();
                var desc = infoList["desc"].ToString();
                var authorityList =
                    JsonConvert.DeserializeObject<Dictionary<String, Object>>(infoList["authorityList"].ToString());
                var roleInfo = db.RoleInfo.Find(roleID);
                roleInfo.RoleName = name;
                roleInfo.RoleDescribe = desc;
                db.RoleAuthority.RemoveRange(db.RoleAuthority.Where(w => w.RoleID == roleID));
                db.SaveChanges();
                foreach (var item in authorityList)
                {
                    Models.RoleAuthority roleAuthority = new Models.RoleAuthority();
                    roleAuthority.AuthorityID = Convert.ToInt32(item.Value);
                    roleAuthority.RoleID = roleID;
                    db.RoleAuthority.Add(roleAuthority);
                    db.SaveChanges();
                }
                return "ok";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        [HttpPost]
        public JsonResult GetListForUser()
        {
            return Json(db.RoleInfo.OrderBy(o => o.RoleName));
        }
    }
}