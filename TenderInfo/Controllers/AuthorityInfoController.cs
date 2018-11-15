using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TenderInfo.Controllers
{
    public class AuthorityInfoController : Controller
    {
        private TenderInfo.Models.DB db = new Models.DB();

        public ViewResult Index()
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
                var type = infoList["type"].ToString();
                var desc = infoList["desc"].ToString();
                var url = infoList["url"].ToString();
                var menuName = infoList["menuName"].ToString();
                var menuIcon = infoList["menuIcon"].ToString();

                var code = 0;
                int.TryParse(infoList["code"].ToString(), out code);

                var order = 0;
                int.TryParse(infoList["order"].ToString(), out order);

                var fatherID = 0;
                int.TryParse(infoList["fatherID"].ToString(), out fatherID);

                Models.AuthorityInfo authorityInfo = new Models.AuthorityInfo();

                authorityInfo.AuthorityDescribe = desc;
                authorityInfo.AuthorityName = name;
                authorityInfo.AuthorityType = type;
                authorityInfo.ConflictCode = code;
                authorityInfo.MenuUrl = url;
                authorityInfo.MenuFatherID = fatherID;
                authorityInfo.MenuOrder = order;
                authorityInfo.MenuName = menuName;
                authorityInfo.MenuIcon = menuIcon;

                db.AuthorityInfo.Add(authorityInfo);
                db.SaveChanges();
                return "ok";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        [HttpPost]
        public string Update()
        {
            try
            {
                var postList =
   JsonConvert.DeserializeObject<Dictionary<String, Object>>(HttpUtility.UrlDecode(Request.Form.ToString()));
                var id = Convert.ToInt32(postList["id"].ToString());

                var name = postList["name"].ToString();
                var type = postList["type"].ToString();
                var desc = postList["desc"].ToString();
                var url = postList["url"].ToString();
                var menuName = postList["menuName"].ToString();
                var menuIcon = postList["menuIcon"].ToString();

                var code = 0;
                int.TryParse(postList["code"].ToString(), out code);

                var order = 0;
                int.TryParse(postList["order"].ToString(), out order);

                var fatherID = 0;
                int.TryParse(postList["fatherID"].ToString(), out fatherID);

                var authorityInfo = db.AuthorityInfo.Find(id);
                authorityInfo.AuthorityDescribe = desc;
                authorityInfo.AuthorityName = name;
                authorityInfo.AuthorityType = type;
                authorityInfo.ConflictCode = code;
                authorityInfo.MenuUrl = url;
                authorityInfo.MenuFatherID = fatherID;
                authorityInfo.MenuOrder = order;
                authorityInfo.MenuName = menuName;
                authorityInfo.MenuIcon = menuIcon;

                db.SaveChanges();
                return "ok";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        [HttpPost]
        public string Del()
        {
            try
            {
                var infoList =
JsonConvert.DeserializeObject<Dictionary<String, Object>>(HttpUtility.UrlDecode(Request.Form.ToString()));
                var id = 0;
                int.TryParse(infoList["id"].ToString(), out id);
                db.AuthorityInfo.Remove(db.AuthorityInfo.Find(id));
                db.SaveChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpPost]
        public JsonResult GetList()
        {
            var limit =Convert.ToInt32(Request.Form["limit"]);
            var offset =Convert.ToInt32(Request.Form["offset"]);
            var authorityName = Request.Form["authorityName"];
            var authorityType = Request.Form["authorityType"];

            var result = db.AuthorityInfo.OrderBy(o => o.AuthorityType).ThenBy(o => o.AuthorityID).AsQueryable();
            if (!string.IsNullOrEmpty(authorityName))
            {
                result = result.Where(w => w.AuthorityName.Contains(authorityName));
            }
            if (!string.IsNullOrEmpty(authorityType))
            {
                result = result.Where(w => w.AuthorityType == authorityType);
            }
            return Json(new { total = result.Count(), rows = result.Skip(offset).Take(limit).ToList() });
        }

        /// <summary>
        /// 获取全部权限信息，按类型、权限名称排序
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetListAll()
        {
            return Json(db.AuthorityInfo.OrderBy(o => o.AuthorityType).ThenBy(o=>o.AuthorityName).ToList());
        }

        [HttpPost]
        public JsonResult GetOne()
        {
            var postList =
   JsonConvert.DeserializeObject<Dictionary<String, Object>>(HttpUtility.UrlDecode(Request.Form.ToString()));
            var id = Convert.ToInt32(postList["id"].ToString());
            var result = db.AuthorityInfo.Find(id);
            return Json(result);
        }
    }
}