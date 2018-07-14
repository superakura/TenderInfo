using Newtonsoft.Json;
using TenderInfo.App_Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TenderInfo.Controllers
{
    public class DeptController : Controller
    {
        private TenderInfo.Models.DB db = new Models.DB();

        public ViewResult Index()
        {
            return View();
        }

        public JsonResult LoadEditDeptInfo(string deptID)
        {
            var id = Convert.ToInt32(deptID);
            var deptInfo = (from d in db.DeptInfo
                            join dp in db.DeptInfo on d.DeptFatherID equals dp.DeptID
                            where d.DeptID == id
                            select new
                            { d.DeptFatherID, dp.DeptName }
                        );

            return Json(deptInfo, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public string AddDept()
        {
            try
            {
                var infoList =
   JsonConvert.DeserializeObject<Dictionary<String, Object>>(HttpUtility.UrlDecode(Request.Form.ToString()));

                var deptName = infoList["deptName"].ToString();

                var deptFatherID = 0;
                int.TryParse(infoList["detpFatherID"].ToString(), out deptFatherID);

                var userID = Commen.GetUserFromSession().UserID;

                Models.DeptInfo dept = new Models.DeptInfo();
                dept.DeptFatherID = deptFatherID;
                dept.DeptName = deptName;
                dept.DeptOrder = 0;
                dept.DeptState = 0;
                dept.Open = 0;
                dept.DeptCreateDate = DateTime.Now;
                db.DeptInfo.Add(dept);
                db.SaveChanges();

                //添加单位的用户，默认拥有该单位的管理权限。
                Models.UserDept userDept = new Models.UserDept();
                userDept.DeptID = dept.DeptID;
                userDept.UserID = userID;
                db.UserDept.Add(userDept);
                db.SaveChanges();

                return "ok";
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }

        [HttpPost]
        public string EditDept()
        {
            try
            {
                var infoList =
   JsonConvert.DeserializeObject<Dictionary<String, Object>>(HttpUtility.UrlDecode(Request.Form.ToString()));
                var deptID = Convert.ToInt32(infoList["deptID"].ToString());
                var deptName = infoList["deptName"].ToString();
                var deptFatherID = Convert.ToInt32(infoList["detpFatherID"].ToString());
                var deptInfo = db.DeptInfo.Find(deptID);
                deptInfo.DeptFatherID = deptFatherID;
                deptInfo.DeptName = deptName;
                deptInfo.DeptCreateDate = DateTime.Now;
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
        public string DelDept()
        {
            try
            {
                var infoList =
   JsonConvert.DeserializeObject<Dictionary<String, Object>>(HttpUtility.UrlDecode(Request.Form.ToString()));
                var deptID = Convert.ToInt32(infoList["deptID"].ToString());
                var deptInfo = db.DeptInfo.Find(deptID);
                deptInfo.DeptState = 1;
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

        //根据用户能管理的部门生成列表，匿名访问
        [AllowAnonymous]
        public ViewResult iFrameDeptTree()
        {
            return View();
        }

        //供查询用部门列表，匿名访问
        [AllowAnonymous]
        public ViewResult iFrameDeptTreeNoRole()
        {
            return View();
        }

        [AllowAnonymous]
        public JsonResult LoadDeptList()
        {
            var userID = Commen.GetUserFromSession().UserID;

            var userDept = db.UserDept.Where(w => w.UserID == userID).Select(s => s.DeptID).ToArray();

            //var deptList = db.DeptInfo.Where(w => w.DeptState == 0 & userDept.Contains(w.DeptID)).OrderBy(o => o.DeptOrder)
            //    .Select(s => new { id = s.DeptID, name = s.DeptName, pId = s.DeptFatherID, open = s.Open });

            var deptList = db.DeptInfo.Where(w => w.DeptState == 0).OrderBy(o => o.DeptOrder)
                .Select(s => new { id = s.DeptID, name = s.DeptName, pId = s.DeptFatherID, open = s.Open });//无限制，测试用。

            return Json(deptList, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult LoadDeptListNoRole()
        {
            var userID = Commen.GetUserFromSession().UserID;

            var userDept = db.UserDept.Where(w => w.UserID == userID).Select(s => s.DeptID).ToArray();

            var deptList = db.DeptInfo.Where(w => w.DeptState == 0).OrderBy(o => o.DeptOrder)
                .Select(s => new { id = s.DeptID, name = s.DeptName, pId = s.DeptFatherID, open = s.Open });

            return Json(deptList, JsonRequestBehavior.AllowGet);
        }
    }
}