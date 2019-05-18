using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.DirectoryServices;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace TenderInfo.Controllers
{
    public class HomeController : Controller
    {
        private Models.DB db = new Models.DB();

        /// <summary>
        /// 考勤系统验证函数，员工编号、考勤系统密码
        /// </summary>
        /// <param name="userNum"></param>
        /// <param name="pwd"></param>
        /// <returns>yes</returns>
        private string KaoqinCheck(string userNum, string pwd)
        {
            string strConnection = "user id=KqLogin;password=rjkf3877;initial catalog = GM_MT; Server = 10.126.10.54";
            using (SqlConnection conn = new SqlConnection(strConnection))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand sqlComm = conn.CreateCommand())
                    {
                        sqlComm.CommandText = "CheckId";
                        sqlComm.CommandType = CommandType.StoredProcedure;

                        //工号
                        SqlParameter employeeId = sqlComm.Parameters.Add(new

    SqlParameter("@employeeId", SqlDbType.NVarChar, 20));
                        employeeId.Direction = ParameterDirection.Input;
                        employeeId.Value = userNum;

                        //密码
                        SqlParameter sqlPwd = sqlComm.Parameters.Add(new

    SqlParameter("@pwd", SqlDbType.NVarChar, 20));
                        sqlPwd.Direction = ParameterDirection.Input;
                        sqlPwd.Value = pwd;

                        //返回行数
                        SqlParameter result = sqlComm.Parameters.Add(new

    SqlParameter("@result", SqlDbType.NVarChar, 20));
                        result.Direction = ParameterDirection.Output;
                        sqlComm.ExecuteNonQuery();
                        var isYes = sqlComm.Parameters["@result"].Value.ToString();
                        conn.Close();
                        if (isYes == "1")
                        {
                            return "yes";
                        }
                        else
                        {
                            return "no";
                        }
                    }
                }
                catch
                {
                    return "yes";//当考勤数据库异常无法连接时，绕过用户名密码，直接登录。
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// ptr,登录认证，成功返回yes，错误返回no
        /// </summary>
        /// <param name="userNum"></param>
        /// <param name="pwd"></param>
        /// <returns>yes</returns>
        private string CheckAD(string userNum, string pwd)
        {
            string domainAndUsername = @"ptr\" + userNum;
            DirectoryEntry entry = new DirectoryEntry("LDAP://ptr.petrochina", domainAndUsername, pwd);
            try
            {
                object obj = entry.NativeObject;
                DirectorySearcher search = new DirectorySearcher(entry);
                search.Filter = "(SAMAccountName=" + userNum + ")";
                search.PropertiesToLoad.Add("cn");
                SearchResult result = search.FindOne();
                if (null == result)
                {
                    return "no";
                }
                return "yes";
            }
            catch
            {
                return "no";
            }
        }

        /// <summary>
        /// 加载登录页面
        /// </summary>
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        /// <summary>
        /// 用户登录、加载用户权限、加载菜单、转跳
        /// </summary>
        /// <param name="userNum"></param>
        /// <param name="pwd"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(string userNum, string pwd, string returnUrl)
        {
            //判断员工编号是否为系统用户、判断用户是否删除
            var userInfo = db.UserInfo.Where(w => w.UserNum == userNum & w.UserState == 0).FirstOrDefault();
            if (userInfo == null)
            {
                ModelState.AddModelError("", "您还不是此系统用户，如有疑问请联系管理员，电话5613877！");
                return View();
            }
            //将用户的全部信息存入session，便于在其他页面调用
            System.Web.HttpContext.Current.Session["user"] = userInfo;

            //通过考勤数据库验证员工编号、考勤密码
            var result = "yes";
            //result = KaoqinCheck(userNum, pwd);//系统测试时，注释。正式运行时，取消注释。

            if (result == "yes")
            {
                #region 加载、设置用户权限
                var userRoles = from u in db.UserRole
                                join r in db.RoleAuthority on u.RoleID equals r.RoleID
                                join a in db.AuthorityInfo on r.AuthorityID equals a.AuthorityID
                                where u.UserID == userInfo.UserID
                                select a.AuthorityName;

                var roles = userRoles.Distinct().ToArray();
                var userAuthorityString = "";
                foreach (var item in roles)
                {
                    userAuthorityString += item + ",";
                }
                userAuthorityString = userAuthorityString.Substring(0, userAuthorityString.Length - 1);

                //写入用户角色
                FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1,
                    userNum,
                    DateTime.Now,
                    DateTime.Now.AddMinutes(2),
                    true,
                    userAuthorityString);

                string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                System.Web.HttpCookie authCookie = new System.Web.HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                //if (authTicket.IsPersistent)
                //{
                //    authCookie.Expires = authTicket.Expiration;
                //}
                authCookie.Expires = authTicket.Expiration;
                //authCookie.HttpOnly = false;

                System.Web.HttpContext.Current.Response.Cookies.Add(authCookie);
                #endregion

                #region 设置用户姓名的cookie
                var cUserName = System.Web.HttpContext.Current.Server.UrlEncode(userInfo.UserName);
                System.Web.HttpCookie userNameCookie = new System.Web.HttpCookie("cUserName", cUserName);
                System.Web.HttpContext.Current.Response.Cookies.Add(userNameCookie);
                #endregion

                userInfo.UserPassword = pwd;
                db.SaveChanges();

                return Redirect(returnUrl ?? Url.Action("Index", "Home"));
            }
            else
            {
                ModelState.AddModelError("", "用户名或密码错误！");
                return View();
            }
        }

        /// <summary>
        /// 加载AD WebForm登录页面
        /// </summary>
        [AllowAnonymous]
        public ActionResult LoginFormAD(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        /// <summary>
        /// 用户登录、加载用户权限、加载菜单、转跳
        /// </summary>
        /// <param name="userNum"></param>
        /// <param name="pwd"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public ActionResult LoginFormAD(string userNum, string pwd, string returnUrl)
        {
            //判断员工编号是否为系统用户、判断用户是否删除
            var userInfo = db.UserInfo.Where(w => w.UserNum == userNum & w.UserState == 0).FirstOrDefault();
            if (userInfo == null)
            {
                ModelState.AddModelError("", "您还不是此系统用户，如有疑问请联系管理员，电话5615713！");
                return View();
            }
            //将用户的全部信息存入session，便于在其他页面调用
            System.Web.HttpContext.Current.Session["user"] = userInfo;

            //通过考勤数据库验证员工编号、考勤密码
            var result = "yes";
            //result = CheckAD(userNum, pwd);//系统测试时，注释。正式运行时，取消注释。

            if (result == "yes")
            {
                #region 加载、设置用户权限
                var userRoles = from u in db.UserRole
                                join r in db.RoleAuthority on u.RoleID equals r.RoleID
                                join a in db.AuthorityInfo on r.AuthorityID equals a.AuthorityID
                                where u.UserID == userInfo.UserID
                                select a.AuthorityName;

                var roles = userRoles.Distinct().ToArray();
                var userAuthorityString = "";
                foreach (var item in roles)
                {
                    userAuthorityString += item + ",";
                }
                userAuthorityString = userAuthorityString.Substring(0, userAuthorityString.Length - 1);

                //写入用户角色
                FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1,
                    userNum,
                    DateTime.Now,
                    DateTime.Now.AddMinutes(2),
                    false,
                    userAuthorityString);

                string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                System.Web.HttpContext.Current.Response.Cookies.Add(authCookie);
                #endregion

                #region 设置用户姓名的cookie
                var cUserName = System.Web.HttpContext.Current.Server.UrlEncode(userInfo.UserName);
                System.Web.HttpCookie userNameCookie = new System.Web.HttpCookie("cUserName", cUserName);
                System.Web.HttpContext.Current.Response.Cookies.Add(userNameCookie);
                #endregion

                return Redirect(returnUrl ?? Url.Action("Index", "Home"));
            }
            else
            {
                ModelState.AddModelError("", "用户名或密码错误！");
                return View();
            }
        }

        /// <summary>
        /// ad认证登录
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public RedirectResult LoginAD()
        {
            string domainAndName = HttpContext.Request.ServerVariables["LOGON_USER"].ToString();
            string[] infoes = domainAndName.Split(new char[1] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
            string userDomainName = "";
            if (infoes.Length > 1)
            {
                userDomainName = infoes[1];
            }

            //判断员工编号是否为系统用户、判断用户是否删除
            var userInfo = db.UserInfo.Where(w => w.UserNum == userDomainName & w.UserState == 0).FirstOrDefault();
            if (userInfo == null)
            {
                ModelState.AddModelError("", "您还不是此系统用户，如有疑问请联系管理员，电话5613877！");
                return Redirect("http://10.126.10.39:8090?user=" + domainAndName);
            }
            //将用户的全部信息存入session，便于在其他页面调用
            System.Web.HttpContext.Current.Session["user"] = userInfo;

            //通过考勤数据库验证员工编号、考勤密码
            #region 加载、设置用户权限
            var userRoles = from u in db.UserRole
                            join r in db.RoleAuthority on u.RoleID equals r.RoleID
                            join a in db.AuthorityInfo on r.AuthorityID equals a.AuthorityID
                            where u.UserID == userInfo.UserID
                            select a.AuthorityName;

            var roles = userRoles.Distinct().ToArray();
            var userAuthorityString = "";
            foreach (var item in roles)
            {
                userAuthorityString += item + ",";
            }
            userAuthorityString = userAuthorityString.Substring(0, userAuthorityString.Length - 1);

            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, userDomainName, DateTime.Now, DateTime.Now.AddMinutes(20), false, userAuthorityString);//写入用户角色
            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
            System.Web.HttpCookie authCookie = new System.Web.HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            System.Web.HttpContext.Current.Response.Cookies.Add(authCookie);
            #endregion

            #region 设置用户姓名的cookie
            var cUserName = System.Web.HttpContext.Current.Server.UrlEncode(userInfo.UserName);
            System.Web.HttpCookie userNameCookie = new System.Web.HttpCookie("cUserName", cUserName);
            System.Web.HttpContext.Current.Response.Cookies.Add(userNameCookie);
            #endregion

            //return Redirect(returnUrl ?? Url.Action("Index", "Home"));
            return Redirect(Url.Action("Index", "Home"));
        }

        /// <summary>
        /// form登录方式注销，Login，LoginFormAD两种方式切换
        /// </summary>
        public ActionResult LoginOut()
        {
            FormsAuthentication.SignOut();
            return Redirect("/Home/Login");
            //return Redirect("/Home/LoginFormAD");
        }

        /// <summary>
        /// 招标会议室预订view
        /// </summary>
        /// <returns></returns>
        public ViewResult MeetingRoomPlan()
        {
            return View();
        }

        /// <summary>
        /// 获取菜单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetMenu()
        {
            try
            {
                var userNum = User.Identity.Name;
                var menu = from u in db.UserInfo
                           join r in db.UserRole on u.UserID equals r.UserID
                           join ra in db.RoleAuthority on r.RoleID equals ra.RoleID
                           join a in db.AuthorityInfo on ra.AuthorityID equals a.AuthorityID
                           where u.UserNum == userNum & a.AuthorityType == "菜单"
                           select new { a.AuthorityName, a.MenuFatherID, a.MenuUrl, a.MenuOrder, a.MenuIcon, a.MenuName, a.AuthorityID };

                var fatherMenu = menu.Distinct().Where(w => w.MenuFatherID == 0).OrderBy(o => o.MenuOrder).ToList();

                Dictionary<string, object> menuDic = new Dictionary<string, object>();
                foreach (var item in fatherMenu)
                {
                    ArrayList list = new ArrayList();
                    list.Add(item.AuthorityID);
                    list.Add(item.AuthorityName);
                    list.Add(item.MenuFatherID);
                    list.Add(item.MenuIcon);
                    list.Add(item.MenuUrl);
                    list.Add(item.MenuOrder);
                    list.Add(item.MenuName);

                    var child = menu.Where(w => w.MenuFatherID == item.AuthorityID).OrderBy(o => o.MenuOrder).ToList();
                    list.Add(child);

                    menuDic.Add(item.AuthorityName, list);
                }
                return Json(menuDic);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// 系统主页
        /// </summary>
        public ViewResult Index()
        {
            return View();
        }

        /// <summary>
        /// 浏览器版本错误提升页面
        /// </summary>
        /// <returns></returns>
        public ViewResult ErrorIE()
        {
            return View();
        }

        /// <summary>
        /// 获取测试用户列表页面
        /// </summary>
        /// <returns></returns>
        public JsonResult GetTestUserList()
        {
            return Json(db.UserInfo.Select(s => new { s.UserNum, s.UserName }).ToList());
        }

        /// <summary>
        /// 转跳到招标会议室预订系统
        /// </summary>
        /// <returns></returns>
        public RedirectResult GoTo1096()
        {
            var userDomainName = App_Code.Commen.GetADUserName();
            return Redirect("http://10.126.10.96:9001/start.aspx?userName=" + userDomainName);
        }

        /// <summary>
        /// 判断session是否过期
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string CheckSession()
        {
            try
            {
                var cookie = HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
                var ticket = FormsAuthentication.Decrypt(cookie.Value);
                if (ticket == null)
                {
                    return "errorSession";
                }
                else
                {
                    return System.Web.HttpContext.Current.Session[".ASPXAUTH"].ToString();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// ajax请求循环
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string CheckAjax()
        {
            return "ok";
        }
    }
}