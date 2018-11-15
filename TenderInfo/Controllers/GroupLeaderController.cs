using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TenderInfo.Controllers
{
    public class GroupLeaderController : Controller
    {
        private Models.DB db = new Models.DB();

        // GET: GroupLeader
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public string Insert()
        {
            try
            {
                var leaderID = 0;
                int.TryParse(Request.Form["leaderID"], out leaderID);

                var memberID = 0;
                int.TryParse(Request.Form["memberID"], out memberID);

                var info = new Models.GroupLeader();
                info.LeaderUserID = leaderID;
                info.MemberUserID = memberID;
                db.GroupLeader.Add(info);
                db.SaveChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpPost]
        public string Del()
        {
            try
            {
                var groupLeaderID = 0;
                int.TryParse(Request.Form["groupLeaderID"], out groupLeaderID);
                var info = db.GroupLeader.Find(groupLeaderID);
                db.GroupLeader.Remove(info);
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
            try
            {
                var result = from g in db.GroupLeader
                             join l in db.UserInfo on g.LeaderUserID equals l.UserID
                             join m in db.UserInfo on g.LeaderUserID equals m.UserID
                             orderby g.LeaderUserID
                             select new
                             {
                                 g.GroupLeaderID,
                                 leaderName = l.UserName,
                                 leaderID = g.LeaderUserID,
                                 memberName = m.UserName,
                                 memberID = g.MemberUserID
                             };
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult GetLeaderList()
        {
            try
            {
                var roleList = db.RoleAuthority.Where(w => w.AuthorityID == 60).Select(s=>s.RoleID).ToList();//组长查看权限
                var userList = db.UserRole.Where(w => roleList.Contains(w.RoleID)).Select(s => s.UserID).ToList();
                return Json(db.UserInfo.Where(w => userList.Contains(w.UserID)));
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult GetMemberList()
        {
            try
            {
                var userList = db.UserRole.Where(w => w.RoleID == 19).Select(s => s.UserID).ToList();
                return Json(db.UserInfo.Where(w => userList.Contains(w.UserID)));
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
    }
}