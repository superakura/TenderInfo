using Newtonsoft.Json;
using TenderInfo.App_Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TenderInfo.Controllers
{
    public class UserInfoController : Controller
    {
        //1、更改用户所在单位操作,对用户的调动操作权限。
        //2、用户根据所管理单位范围，管理相应人员。
        //3、根据用户【可授权】的角色，加载相应角色，赋予相应人员权限。
        //4、修改用户时，权限和管理部门有时无法加载。
        //5、限制向二级单位里面增加用户操作

        private Models.DB db = new Models.DB();

        public ViewResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetList()
        {
            var limit = 0;
            int.TryParse(Request.Form["limit"], out limit);

            var offset = 0;
            int.TryParse(Request.Form["offset"], out offset);

            var userName = Request.Form["userName"];
            var userNum = Request.Form["userNum"];

            var deptID = 0;
            int.TryParse(Request.Form["deptID"], out deptID);

            var userID = Commen.GetUserFromSession().UserID;

            //用户所能控制得部门列表
            var userDept = db.UserDept.Where(w => w.UserID == userID).Select(s => s.DeptID).ToArray();

            var result = (from u in db.UserInfo
                          join d in db.DeptInfo on u.UserDeptID equals d.DeptID
                          join f in db.DeptInfo on d.DeptFatherID equals f.DeptID
                          where u.UserState == 0
                          orderby u.UserNum
                          select new
                          {
                              u.UserDuty,
                              u.UserName,
                              u.UserID,
                              u.UserNum,
                              u.UserPhone,
                              u.UserEmail,
                              d.DeptName,
                              fatherDeptName = f.DeptName,
                              u.UserDeptID,
                              u.UserMobile,
                              u.UserRemark
                          });
            //判断所选单位是否为大庆炼化公司
            if (deptID != 1)
            {
                var count = db.UserInfo.Where(w => w.UserDeptID == deptID).Count();
                if (count == 0)
                {
                    result = result.Where(w => db.DeptInfo.Where(t => t.DeptFatherID == deptID).Select(s => s.DeptID).Contains(w.UserDeptID));
                }
                else
                {
                    result = result.Where(w => w.UserDeptID == deptID);
                }
            }

            if (!string.IsNullOrEmpty(userName))
            {
                result = result.Where(w => w.UserName.Contains(userName));
            }
            if (!string.IsNullOrEmpty(userNum))
            {
                result = result.Where(w => w.UserNum.Contains(userNum));
            }
            return Json(new { total = result.Count(), rows = result.Skip(offset).Take(limit).ToList() });
        }

        [HttpPost]
        public string Insert()
        {
            try
            {
                var infoList =
  JsonConvert.DeserializeObject<Dictionary<String, Object>>(HttpUtility.UrlDecode(Request.Form.ToString()));
                var userNum = infoList["userNum"].ToString();//员工编号
                var userName = infoList["userName"].ToString();
                var userEmail = infoList["userEmail"].ToString();
                var deptID = 0;
                int.TryParse(infoList["deptID"].ToString(), out deptID);//用户所在单位
                var userDuty = infoList["userDuty"].ToString();//职务
                var userPhone = infoList["userPhone"].ToString();//办公电话
                var userMobile = infoList["userMobile"].ToString();//手机
                var userRemark = infoList["userRemark"].ToString();

                //按员工编号userNum检查数据库中用户信息是否存在
                Models.UserInfo userInfo = db.UserInfo.Where(w => w.UserNum == userNum).FirstOrDefault();
                if (userInfo==null)//如果用户不存在，直接插入用户信息
                {
                    #region 插入用户基本信息
                    userInfo = new Models.UserInfo();
                    userInfo.UserName = userName;
                    userInfo.UserNum = userNum;
                    userInfo.UserDuty = userDuty;
                    userInfo.UserState = 0;
                    userInfo.UserDeptID = deptID;
                    userInfo.UserEmail = userEmail == string.Empty ? null : userEmail;
                    userInfo.UserPhone = userPhone;
                    userInfo.UserRemark = userRemark;
                    userInfo.UserMobile = userMobile;

                    db.UserInfo.Add(userInfo);
                    db.SaveChanges();
                    #endregion
                }
                else//如果不存在用户信息，进一步判断用户是否删除。
                {
                    var isDelUser = userInfo.UserState;
                    if (isDelUser == 0)//如果未删除，不能修改用户信息，返回用户已存在，不能修改。
                    {
                        return "用户信息已存在！";
                    }
                    else//如果已删除，更新用户信息，将用户状态更改为未删除状态，更新用户所在部门
                    {
                        #region 更新已标记删除用户的信息，将用户状态更改为未删除
                        userInfo.UserName = userName;
                        userInfo.UserDuty = userDuty;
                        userInfo.UserState = 0;//将用户状态更改为未删除
                        userInfo.UserDeptID = deptID;//将用户部门更改为选择的部门
                        userInfo.UserEmail = userEmail == string.Empty ? null : userEmail;
                        userInfo.UserPhone = userPhone;
                        userInfo.UserRemark = userRemark;
                        userInfo.UserMobile = userMobile;
                        db.SaveChanges();
                        #endregion
                    }
                }

                #region 删除用户已经存在的权限和管理部门
                var userDeptExist = db.UserDept.Where(w => w.UserID == userInfo.UserID).ToList();
                if (userDeptExist.Count != 0)
                {
                    db.UserDept.RemoveRange(userDeptExist);
                }
                var userRoleExist = db.UserRole.Where(w => w.UserID == userInfo.UserID).ToList();
                if (userRoleExist != null)
                {
                    db.UserRole.RemoveRange(userRoleExist);
                }
                db.SaveChanges();
                #endregion

                #region 添加用户所拥有的角色
                Dictionary<string, object> roleList =
                    JsonConvert.DeserializeObject<Dictionary<String, Object>>(infoList["roleList"].ToString());
                foreach (var item in roleList)
                {
                    Models.UserRole userRole = new Models.UserRole();
                    var roleID = 0;
                    int.TryParse(item.Value.ToString(), out roleID);
                    userRole.RoleID = roleID;
                    userRole.UserID = userInfo.UserID;
                    db.UserRole.Add(userRole);
                    db.SaveChanges();
                }
                #endregion

                #region 添加用户管理的部门
                Dictionary<string, object> deptList =
                    JsonConvert.DeserializeObject<Dictionary<String, Object>>(infoList["deptList"].ToString());
                foreach (var item in deptList)
                {
                    Models.UserDept userDept = new Models.UserDept();
                    var deptIDManagerment = 0;
                    int.TryParse(item.Value.ToString(), out deptIDManagerment);
                    userDept.DeptID = deptIDManagerment;
                    userDept.UserID = userInfo.UserID;
                    db.UserDept.Add(userDept);
                    db.SaveChanges();
                }
                #endregion
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
                var infoList =
   JsonConvert.DeserializeObject<Dictionary<String, Object>>(HttpUtility.UrlDecode(Request.Form.ToString()));

                #region 修改用户基本信息,不修改用户的员工编号和用户所在的部门
                var userID = 0;
                int.TryParse(infoList["userID"].ToString(), out userID);
                var userName = infoList["userName"].ToString();
                var userEmail = infoList["userEmail"].ToString();
                var userDuty = infoList["userDuty"].ToString();
                var userPhone = infoList["userPhone"].ToString();
                var userMobile = infoList["userMobile"].ToString();
                var userRemark = infoList["userRemark"].ToString();

                var userInfo = db.UserInfo.Find(userID);
                userInfo.UserName = userName;
                userInfo.UserEmail = userEmail;
                userInfo.UserDuty = userDuty;
                userInfo.UserPhone = userPhone;
                userInfo.UserMobile = userMobile;
                userInfo.UserRemark = userRemark;
                db.SaveChanges();
                #endregion

                #region 删除用户已经存在的权限和管理部门
                var userDeptExist = db.UserDept.Where(w => w.UserID == userInfo.UserID).ToList();
                if (userDeptExist.Count != 0)
                {
                    db.UserDept.RemoveRange(userDeptExist);
                }
                var userRoleExist = db.UserRole.Where(w => w.UserID == userInfo.UserID).ToList();
                if (userRoleExist != null)
                {
                    db.UserRole.RemoveRange(userRoleExist);
                }
                db.SaveChanges();
                #endregion

                #region 添加用户所拥有的角色
                Dictionary<string, object> roleList =
                    JsonConvert.DeserializeObject<Dictionary<String, Object>>(infoList["roleList"].ToString());

                foreach (var item in roleList)
                {
                    Models.UserRole userRole = new Models.UserRole();
                    var roleID = 0;
                    int.TryParse(item.Value.ToString(), out roleID);
                    userRole.RoleID = roleID;
                    userRole.UserID = userInfo.UserID;
                    db.UserRole.Add(userRole);
                    db.SaveChanges();
                }
                #endregion

                #region 添加用户管理的部门
                Dictionary<string, object> deptList =
                    JsonConvert.DeserializeObject<Dictionary<String, Object>>(infoList["deptList"].ToString());

                foreach (var item in deptList)
                {
                    Models.UserDept userDept = new Models.UserDept();
                    var deptIDManagerment = 0;
                    int.TryParse(item.Value.ToString(), out deptIDManagerment);
                    userDept.DeptID = deptIDManagerment;
                    userDept.UserID = userInfo.UserID;
                    db.UserDept.Add(userDept);
                    db.SaveChanges();
                }
                #endregion
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        
        [HttpPost]
        public JsonResult GetOne()
        {
            try
            {
                var infoList =
   JsonConvert.DeserializeObject<Dictionary<String, Object>>(HttpUtility.UrlDecode(Request.Form.ToString()));
                var userID = 0;
                int.TryParse(infoList["userID"].ToString(), out userID);

                var userInfo = db.UserInfo.Where(w=>w.UserID==userID)
                    .Select(s=>new { s.UserID,s.UserName,s.UserNum,s.UserMobile,s.UserPhone,s.UserRemark,s.UserEmail,s.UserDuty});//用户的基本信息
                var userRole = db.UserRole.Where(w => w.UserID == userID).ToList();//用户所拥有的权限
                var userDept = db.UserDept.Where(w => w.UserID == userID).ToList();//用户所能管理的单位
                return Json(new {userInfo=userInfo, userRole=userRole, userDept =userDept});
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpPost]
        public string Del()
        {
            try
            {
                var infoList =
   JsonConvert.DeserializeObject<Dictionary<String, Object>>(HttpUtility.UrlDecode(Request.Form.ToString()));
                var userID = 0;
                int.TryParse(infoList["userID"].ToString(), out userID);

                var userInfo = db.UserInfo.Find(userID);
                userInfo.UserState = 1;
                db.SaveChanges();

                db.UserRole.RemoveRange(db.UserRole.Where(w => w.UserID == userID));
                db.UserDept.RemoveRange(db.UserDept.Where(w=>w.UserID==userID));
                db.SaveChanges();

                var person = App_Code.Commen.GetUserFromSession();
                var log = new Models.Log();
                log.InputDateTime = DateTime.Now;
                log.InputPersonID = person.UserID;
                log.InputPersonName = person.UserName;
                log.LogType = "用户信息管理";
                log.LogContent = "删除用户："+userInfo.UserName+","+"员工编号："+userInfo.UserNum;
                log.LogDataID = userID;
                db.Log.Add(log);
                db.SaveChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}