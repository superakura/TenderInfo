using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TenderInfo.Controllers
{
    public class NoticeInfoController : Controller
    {
        private TenderInfo.Models.DB db = new Models.DB();

        //通知公告管理视图
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ViewResult NoticeShow()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public string Insert()
        {
            try
            {
                var infoList =
   JsonConvert.DeserializeObject<Dictionary<String, Object>>(HttpUtility.UrlDecode(Request.Form.ToString()));

                var title = infoList["title"].ToString();
                var content = infoList["content"].ToString();

                var contentCount = 0;
                int.TryParse(infoList["contentCount"].ToString(), out contentCount);

                Models.NoticeInfo noticeInfo = new Models.NoticeInfo();
                noticeInfo.NoticeTitle = title;
                noticeInfo.Content = content;
                noticeInfo.ContentCount = contentCount;
                noticeInfo.InsertPersonID = (Session["user"] as Models.UserInfo).UserID;
                noticeInfo.InsertDate = DateTime.Now;
                db.NoticeInfo.Add(noticeInfo);
                if (db.SaveChanges() == 1)
                {
                    return "ok";
                }
                else
                {
                    return "error";
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public string Update()
        {
            try
            {
                var infoList =
   JsonConvert.DeserializeObject<Dictionary<String, Object>>(HttpUtility.UrlDecode(Request.Form.ToString()));
                var id = Convert.ToInt32(infoList["id"].ToString());
                var noticeInfo = db.NoticeInfo.Find(id);

                var title = infoList["title"].ToString();
                var content = infoList["content"].ToString();

                var contentCount = 0;
                int.TryParse(infoList["contentCount"].ToString(), out contentCount);

                noticeInfo.NoticeTitle = title;
                noticeInfo.Content = content;
                noticeInfo.ContentCount = contentCount;
                noticeInfo.InsertPersonID = (Session["user"] as Models.UserInfo).UserID;
                noticeInfo.InsertDate = DateTime.Now;

                var isSuccess = db.SaveChanges();
                if (isSuccess == 1)
                {
                    return "ok";
                }
                else
                {
                    return "error";
                }
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

            var result = from n in db.NoticeInfo
                         join u in db.UserInfo on n.InsertPersonID equals u.UserID
                         orderby n.InsertDate descending
                         select new { n.NoticeTitle, n.NoticeID, n.InsertDate, u.UserName, n.ContentCount };

            return Json(new { total = result.Count(), rows = result.Skip(offset).Take(limit).ToList() });
        }

        [HttpPost]
        public JsonResult GetOne()
        {
            var postList =
   JsonConvert.DeserializeObject<Dictionary<String, Object>>(HttpUtility.UrlDecode(Request.Form.ToString()));
            var id = 0;
            int.TryParse(postList["id"].ToString(), out id);
            var result = db.NoticeInfo.Find(id);
            return Json(result);
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
                db.NoticeInfo.Remove(db.NoticeInfo.Find(id));
                db.SaveChanges();
                return "ok";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult GetNoticeListForLogin()
        {
            return Json(db.NoticeInfo.OrderByDescending(o => o.InsertDate).Take(6).Select(s => new { s.NoticeID, s.NoticeTitle }).ToList());
        }

        [AllowAnonymous]
        public JsonResult GetNoticeInfoForLogin()
        {
            var postList =
   JsonConvert.DeserializeObject<Dictionary<String, Object>>(HttpUtility.UrlDecode(Request.Form.ToString()));
            var id = 0;
            int.TryParse(postList["id"].ToString(), out id);
            var result = from n in db.NoticeInfo
                         join u in db.UserInfo on n.InsertPersonID equals u.UserID
                         where n.NoticeID == id
                         select new { n.InsertDate, n.NoticeTitle, n.Content, u.UserName };
            return Json(result);
        }
    }
}