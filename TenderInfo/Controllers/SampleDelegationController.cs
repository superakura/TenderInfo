using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TenderInfo.Controllers
{
    public class SampleDelegationController : Controller
    {
        //SampleDelegation
        private Models.DB db = new Models.DB();

        // 样品委托查询
        public ViewResult Search()
        {
            return View();
        }

        // 样品委托录入
        public ViewResult Input()
        {
            if (User.IsInRole("一次编码表录入"))
            {
                ViewBag.PageRole = "一次编码表录入";
            }
            if (User.IsInRole("二次编码表录入"))
            {
                ViewBag.PageRole = "二次编码表录入";
            }
            if (User.IsInRole("检验报告录入"))
            {
                ViewBag.PageRole = "检验报告录入";
            }
            if (User.IsInRole("样品检测业务员查看"))
            {
                ViewBag.PageRole = "样品检测业务员查看";
            }
            return View();
        }

        //添加送样委托单
        [HttpPost]
        public string Insert()
        {
            try
            {
                var sampleName = Request.Form["tbxSampleName"];//样品名称

                var sampleNum = 0;//样品数量
                int.TryParse(Request.Form["tbxSampleNum"], out sampleNum);

                var startTenderDate = Convert.ToDateTime(Request.Form["tbxStartTenderDate"]);//开标时间

                var projectResponsiblePerson = 0;//招标项目负责人
                int.TryParse(Request.Form["ddlProjectResponsiblePerson"], out projectResponsiblePerson);

                var sampleDelegationAcceptPerson = 0;//技术处送样委托单接收人
                int.TryParse(Request.Form["ddlSampleDelegationAcceptPerson"], out sampleDelegationAcceptPerson);

                var user = App_Code.Commen.GetUserFromSession();

                var sampleDelegation = new Models.SampleDelegation();
                sampleDelegation.StartTenderDate = startTenderDate;
                sampleDelegation.SampleName = sampleName;
                sampleDelegation.SampleNum = sampleNum;
                sampleDelegation.ProjectResponsiblePerson = projectResponsiblePerson;
                sampleDelegation.SampleDelegationAcceptPerson = sampleDelegationAcceptPerson;
                sampleDelegation.SampleDelegationState = "技术要求录入";
                sampleDelegation.InputPerson = user.UserID;
                sampleDelegation.InputPersonName = user.UserName;
                sampleDelegation.InputDateTime = DateTime.Now;

                db.SampleDelegation.Add(sampleDelegation);
                db.SaveChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        //获取送样委托信息列表
        [HttpPost]
        public JsonResult GetList()
        {
            try
            {
                var limit = 0;
                int.TryParse(Request.Form["limit"], out limit);
                var offset = 0;
                int.TryParse(Request.Form["offset"], out offset);
                var sampleName = Request.Form["sampleName"];//样品名称
                var projectResponsiblePerson = 0;
                int.TryParse(Request.Form["projectResponsiblePerson"], out projectResponsiblePerson);//招标负责人
                var sampleDelegationState = Request.Form["sampleDelegationState"];//进度，状态
                var startTenderDateBegin = Request.Form["startTenderDateBegin"];//开标日期开始
                var startTenderDateEnd = Request.Form["startTenderDateEnd"];//开标日期结束

                var userInfo = App_Code.Commen.GetUserFromSession();

                //如果是招标业务员查看，则根据招标时间判断是否显示文件信息。
                //如果是招标科长、技术处、质检，则显示全部文件。
                var isShow = 0;
                if (User.IsInRole("样品检测业务员查看"))
                {
                    isShow = 1;
                }

                var result = from s in db.SampleDelegation
                             join up in db.UserInfo on s.ProjectResponsiblePerson equals up.UserID
                             join ua in db.UserInfo on s.SampleDelegationAcceptPerson equals ua.UserID
                             orderby s.SampleDelegationID descending
                             select new
                             {
                                 s.SampleDelegationID,
                                 s.SampleName,
                                 s.SampleNum,
                                 s.SampleTechnicalRequirement,
                                 s.StartTenderDate,
                                 FirstCodingFileName = isShow == 0 ? s.FirstCodingFileName : s.StartTenderDate <= DateTime.Now ? s.FirstCodingFileName : "",
                                 s.InputPerson,
                                 s.InputPersonName,
                                 s.InputDateTime,
                                 SecondCodingFileName = isShow == 0 ? s.SecondCodingFileName : s.StartTenderDate <= DateTime.Now ? s.SecondCodingFileName : "",
                                 s.SecondCodingInputDate,
                                 s.SecondCodingInputPersonName,
                                 ProjectResponsiblePersonName = up.UserName,
                                 ProjectResponsiblePersonPhone = up.UserPhone + "/" + up.UserMobile,
                                 s.ProjectResponsiblePerson,
                                 SampleDelegationAcceptPersonName = ua.UserName,
                                 SampleDelegationAcceptPersonPhone = ua.UserPhone + "/" + ua.UserMobile,
                                 s.SampleDelegationAcceptPerson,
                                 s.SampleDelegationState
                             };
                if (!string.IsNullOrEmpty(sampleName))
                {
                    result = result.Where(w => w.SampleName.Contains(sampleName));
                }
                if (projectResponsiblePerson != 0)
                {
                    result = result.Where(w => w.ProjectResponsiblePerson == projectResponsiblePerson);
                }
                if (!string.IsNullOrEmpty(sampleDelegationState))
                {
                    result = result.Where(w => w.SampleDelegationState == sampleDelegationState);
                }
                if (!string.IsNullOrEmpty(startTenderDateBegin) & !string.IsNullOrEmpty(startTenderDateEnd))
                {
                    var dateStart = Convert.ToDateTime(startTenderDateBegin);
                    var dateEnd = Convert.ToDateTime(startTenderDateEnd);
                    result = result.Where(w => System.Data.Entity.DbFunctions.DiffMinutes(w.StartTenderDate, dateStart) <= 0 && System.Data.Entity.DbFunctions.DiffMinutes(w.StartTenderDate, dateEnd) >= 0);
                }
                //if (User.IsInRole("一次编码表录入"))
                //{
                //    result = result.Where(w => w.FirstCodingInputPerson == userInfo.UserID);
                //}
                if (User.IsInRole("二次编码表录入"))
                {
                    result = result.Where(w => w.SampleDelegationAcceptPerson == userInfo.UserID);
                }
                if (User.IsInRole("检验报告录入"))
                {
                    result = result.Where(w => w.SampleDelegationState != "一次编码表上传完成");
                }
                if (User.IsInRole("样品检测业务员查看"))
                {
                    result = result.Where(w => w.ProjectResponsiblePerson == userInfo.UserID);
                }
                return Json(new { total = result.Count(), rows = result.Skip(offset).Take(limit).ToList() });
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        //获取技术处接收人员列表
        [HttpPost]
        public JsonResult GetAcceptPersonList()
        {
            var list = from u in db.UserInfo
                       join ur in db.UserRole on u.UserID equals ur.UserID
                       join r in db.RoleInfo on ur.RoleID equals r.RoleID
                       join ra in db.RoleAuthority on r.RoleID equals ra.RoleID
                       where ra.AuthorityID == 45//技术处人员录入二次编码表
                       select new { u.UserID, u.UserName, u.UserNum };
            return Json(list);
        }

        //获取招标业务员列表
        [HttpPost]
        public JsonResult GetProjectResponsiblePersonList()
        {
            var list = from u in db.UserInfo
                       join ur in db.UserRole on u.UserID equals ur.UserID
                       join r in db.RoleInfo on ur.RoleID equals r.RoleID
                       join ra in db.RoleAuthority on r.RoleID equals ra.RoleID
                       where ra.AuthorityID == 48//样品检测业务员查看
                       select new { u.UserID, u.UserName, u.UserNum };

            var user = App_Code.Commen.GetUserFromSession();
            if (User.IsInRole("样品检测业务员查看"))
            {
                list = list.Where(w => w.UserID == user.UserID);
            }
            return Json(list);
        }

        //修改招标开始时间
        [HttpPost]
        public JsonResult EditTenderStartDate(HttpPostedFileBase editTenderDateFile)
        {
            try
            {
                var user = App_Code.Commen.GetUserFromSession();
                var id = 0;
                int.TryParse(Request.Form["EditTenderDateInfoID"], out id);
                var editTenderStartDate = Convert.ToDateTime(Request.Form["tbxEditTenderDate"]);
                var editTenderDateReason = Request.Form["tbxEditTenderDateReason"];

                var newName = string.Empty;
                if (editTenderDateFile != null)
                {
                    var fileExt = Path.GetExtension(editTenderDateFile.FileName).ToLower();
                    newName = "修改招标开始时间说明文件_" + DateTime.Now.ToString("yyMMddhhmmss") + DateTime.Now.Millisecond + fileExt;
                    var filePath = Request.MapPath("~/FileUpload");
                    var fullName = Path.Combine(filePath, newName);
                    editTenderDateFile.SaveAs(fullName);
                }

                var sampleDelegation = db.SampleDelegation.Find(id);
                var log = new Models.Log();
                log.InputDateTime = DateTime.Now;
                log.InputPersonID = user.UserID;
                log.InputPersonName = user.UserName;
                log.LogDataID = id;
                log.LogType = "修改招标开始时间";
                log.LogContent = "招标开始时间由【" + sampleDelegation.StartTenderDate + "】修改为【" + editTenderStartDate + "】";
                log.LogReason = editTenderDateReason;
                log.Col1 = newName;
                log.Col2 = sampleDelegation.StartTenderDate + "#" + editTenderStartDate;//记录用，原招标开始时间，用#分隔，修改后的招标开始时间
                db.Log.Add(log);

                sampleDelegation.StartTenderDate = editTenderStartDate;

                db.SaveChanges();
                return Json(new { state = "ok" });
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        //获取修改招标开始时间记录信息
        [HttpPost]
        public JsonResult GetEditTenderDateLog()
        {
            try
            {
                var id = 0;
                int.TryParse(Request.Form["id"], out id);

                return Json(db.Log.Where(w => w.LogType == "修改招标开始时间" && w.LogDataID == id).OrderByDescending(o => o.InputDateTime).ToList());
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        //不同权限的人员删除操作
        [HttpPost]
        public string Del()
        {
            try
            {
                var id = 0;
                int.TryParse(Request.Form["id"], out id);

                var info = db.SampleDelegation.Find(id);
                if (info.StartTenderDate > DateTime.Now)
                {
                    var filePath = Request.MapPath("~/FileUpload");

                    if (User.IsInRole("一次编码表录入"))
                    {
                        //如果招标中心科长，执行删除操作，将5个文件删除，将数据库中记录删除，
                        //将log表中的修改开标时间数据删除，在log表中增加删除操作的记录。
                        var file3 = info.FirstCodingFileName;
                        var file4 = info.SecondCodingFileName;

                        var fullName3 = Path.Combine(filePath, file3 ?? "");
                        var fullName4 = Path.Combine(filePath, file4 ?? "");
                        if (System.IO.File.Exists(fullName3))
                        {
                            System.IO.File.Delete(fullName3);
                        }
                        if (System.IO.File.Exists(fullName4))
                        {
                            System.IO.File.Delete(fullName4);
                        }
                        db.SampleDelegation.Remove(info);

                        var log = new Models.Log();
                        var userInfo = App_Code.Commen.GetUserFromSession();
                        log.InputPersonID = userInfo.UserID;
                        log.InputPersonName = userInfo.UserName;
                        log.InputDateTime = DateTime.Now;
                        log.LogContent = "删除送样委托单：样品名称【" + info.SampleName + "】开标时间【" + info.StartTenderDate + "】";
                        log.LogType = "删除送样委托单";
                        db.Log.Add(log);

                        var logDel = db.Log.Where(w => w.LogType == "修改招标开始时间" && w.LogDataID == id).FirstOrDefault();
                        if (logDel != null)
                        {
                            var logFile = Path.Combine(filePath, logDel.Col1 ?? "");
                            if (System.IO.File.Exists(logFile))
                            {
                                System.IO.File.Delete(logFile);
                            }
                            db.Log.Remove(logDel);
                        }
                    }

                    if (User.IsInRole("二次编码表录入"))
                    {
                        //技术处接收人员，执行删除操作：删除送样委托单、二次编码表文件，将状态改为“一次编码表完成”
                        //将数据表中的上传人员、上传时间变为空
                        if (info.SampleDelegationState == "二次编码表上传完成")
                        {
                            var file2 = info.SecondCodingFileName;
                            var fullName2 = Path.Combine(filePath, file2);
                            if (System.IO.File.Exists(fullName2))
                            {
                                System.IO.File.Delete(fullName2);
                            }

                            info.SecondCodingFileName = null;
                            info.SecondCodingInputDate = null;
                            info.SecondCodingInputPerson = 0;
                            info.SecondCodingInputPersonName = null;

                            info.SampleDelegationState = "一次编码表上传完成";

                            var log = new Models.Log();
                            var userInfo = App_Code.Commen.GetUserFromSession();
                            log.InputPersonID = userInfo.UserID;
                            log.InputPersonName = userInfo.UserName;
                            log.InputDateTime = DateTime.Now;
                            log.LogContent = "删除二次编码表：样品名称【" + info.SampleName + "】开标时间【" + info.StartTenderDate + "】";
                            log.LogType = "送样委托删除";
                            db.Log.Add(log);
                        }
                        else
                        {
                            return "只能删除二次编码表和送样委托单！";
                        }
                    }

                    //if (User.IsInRole("检验报告录入"))
                    //{
                    //    //质检人员执行删除，删除检验报告文件，将相应数据字段置空，变更状态未“二次编码表上传完成”
                    //    if (info.SampleDelegationState == "检验报告上传完成")
                    //    {
                    //        var file = info.CheckReportFileName;
                    //        var fullName = Path.Combine(filePath, file);
                    //        if (System.IO.File.Exists(fullName))
                    //        {
                    //            System.IO.File.Delete(fullName);
                    //        }
                    //        info.CheckReportFileName = null;
                    //        info.CheckReportInputDate = null;
                    //        info.CheckReportInputPerson = 0;
                    //        info.CheckReportInputPersonName = null;

                    //        info.SampleDelegationState = "二次编码表上传完成";

                    //        var log = new Models.Log();
                    //        var userInfo = App_Code.Commen.GetUserFromSession();
                    //        log.InputPersonID = userInfo.UserID;
                    //        log.InputPersonName = userInfo.UserName;
                    //        log.InputDateTime = DateTime.Now;
                    //        log.LogContent = "删除检验报告：样品名称【" + info.SampleName + "】开标时间【" + info.StartTenderDate + "】";
                    //        log.LogType = "送样委托删除";
                    //        db.Log.Add(log);
                    //    }
                    //    else
                    //    {
                    //        return "只能删除检验报告文件！";
                    //    }
                    //}

                    db.SaveChanges();
                    return "ok";
                }
                else
                {
                    return "已超过招标开始时间，不能执行删除操作！";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        //添加检验技术要求
        [HttpPost]
        [ValidateInput(false)]
        public string UpdateSampleTechnicalRequirement()
        {
            try
            {
                var infoList =
  JsonConvert.DeserializeObject<Dictionary<String, Object>>(HttpUtility.UrlDecode(Request.Form.ToString()));
                var id = 0;
                int.TryParse(infoList["id"].ToString(), out id);
                var info = db.SampleDelegation.Find(id);

                var content = infoList["content"].ToString();
                info.SampleTechnicalRequirement = content;
                info.SampleDelegationState = "质检接收审核";

                var log = new Models.Log();
                var userInfo = App_Code.Commen.GetUserFromSession();
                log.InputPersonID = userInfo.UserID;
                log.InputPersonName = userInfo.UserName;
                log.InputDateTime = DateTime.Now;
                log.LogContent = "样品检验技术要求更新：样品名称【" + info.SampleName + "】";
                log.LogType = "样品检验技术要求";
                db.Log.Add(log);

                db.SaveChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 根据登录用户的权限，分别执行一次、二次编码表、检验报告的文件上传
        /// </summary>
        /// <param name="uploadFile"></param>
        /// <returns>ok</returns>
        [HttpPost]
        public string UploadFile(HttpPostedFileBase uploadFile)
        {
            try
            {
                if (uploadFile == null)
                {
                    return "noFile";
                }
                var filePath = Request.MapPath("~/FileUpload");
                var user = App_Code.Commen.GetUserFromSession();

                if (User.IsInRole("一次编码表录入"))
                {
                    var id = 0;
                    int.TryParse(Request.Form["UploadInfoID"], out id);
                    var info = db.SampleDelegation.Find(id);

                    //如果存在一次编码表文件，则先执行删除文件操作。
                    //再上传新的文件进行覆盖。
                    if (info.FirstCodingFileName != null)
                    {
                        var firstCodingFile = info.FirstCodingFileName;
                        var firstCodingFullName = Path.Combine(filePath, firstCodingFile ?? "");
                        if (System.IO.File.Exists(firstCodingFullName))
                        {
                            System.IO.File.Delete(firstCodingFullName);
                        }
                    }

                    var fileExt = Path.GetExtension(uploadFile.FileName).ToLower();
                    var newName = "一次编码表_" + Guid.NewGuid() + fileExt;
                    var fullName = Path.Combine(filePath, newName);
                    uploadFile.SaveAs(fullName);

                    info.FirstCodingFileName = newName;
                    info.FirstCodingInputDate = DateTime.Now;
                    info.FirstCodingInputPerson = user.UserID;
                    info.FirstCodingInputPersonName = user.UserName;
                }

                if (User.IsInRole("二次编码表录入"))
                {
                    var id = 0;
                    int.TryParse(Request.Form["UploadInfoID"], out id);
                    var info = db.SampleDelegation.Find(id);

                    //如果存在二次编码表文件，则先执行删除文件操作。
                    //再上传新的文件进行覆盖。
                    if (info.SecondCodingFileName != null)
                    {
                        var secondCodingFile = info.SecondCodingFileName;
                        var secondCodingFullName = Path.Combine(filePath, secondCodingFile ?? "");
                        if (System.IO.File.Exists(secondCodingFullName))
                        {
                            System.IO.File.Delete(secondCodingFullName);
                        }
                    }
                    var fileExt = Path.GetExtension(uploadFile.FileName).ToLower();
                    var newName = "二次编码表_" + Guid.NewGuid() + fileExt;
                    var fullName = Path.Combine(filePath, newName);
                    uploadFile.SaveAs(fullName);

                    info.SecondCodingFileName = newName;
                    info.SecondCodingInputDate = DateTime.Now;
                    info.SecondCodingInputPerson = user.UserID;
                    info.SecondCodingInputPersonName = user.UserName;
                }

                if (User.IsInRole("检验报告录入"))
                {
                    var sampleDelegationID = 0;
                    int.TryParse(Request.Form["UploadInfoID"], out sampleDelegationID);

                    var checkReportFile = new Models.CheckReportFile();

                    var fileExt = Path.GetExtension(uploadFile.FileName).ToLower();
                    var fileName = Path.GetFileNameWithoutExtension(uploadFile.FileName).ToLower();
                    //检验报告格式设定为源文件名，加上当前日期时间毫秒字符串形式。
                    var newName = fileName + App_Code.Commen.GetDateTimeString() + fileExt;
                    var fullName = Path.Combine(filePath, newName);
                    uploadFile.SaveAs(fullName);

                    checkReportFile.CheckReportFileName = newName;
                    checkReportFile.CheckReportInputDate = DateTime.Now;
                    checkReportFile.CheckReportInputPerson = user.UserID;
                    checkReportFile.CheckReportInputPersonName = user.UserName;
                    checkReportFile.SampleDelegationID = sampleDelegationID;
                    db.CheckReportFile.Add(checkReportFile);
                }

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