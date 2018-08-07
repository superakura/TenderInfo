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

        //一次编码表、送样委托文件（招标中心填写部分）上传
        [HttpPost]
        public JsonResult UploadFirstCodingFile(HttpPostedFileBase firstCodingFile, HttpPostedFileBase sampleDelegationFirstFile)
        {
            var sampleName = Request.Form["tbxSampleName"];//样品名称
            var startTenderDate = Convert.ToDateTime(Request.Form["tbxStartTenderDate"]);//开标时间

            var projectResponsiblePerson = 0;//招标项目负责人
            int.TryParse(Request.Form["ddlProjectResponsiblePerson"], out projectResponsiblePerson);

            var sampleDelegationAcceptPerson = 0;//技术处送样委托单接收人
            int.TryParse(Request.Form["ddlSampleDelegationAcceptPerson"], out sampleDelegationAcceptPerson);

            var user = App_Code.Commen.GetUserFromSession();

            var sampleDelegation = new Models.SampleDelegation();
            sampleDelegation.StartTenderDate = startTenderDate;
            sampleDelegation.SampleName = sampleName;
            sampleDelegation.ProjectResponsiblePerson = projectResponsiblePerson;
            sampleDelegation.SampleDelegationAcceptPerson = sampleDelegationAcceptPerson;
            sampleDelegation.SampleDelegationState = "一次编码表上传完成";
            try
            {
                if (firstCodingFile != null)
                {
                    var fileExt = Path.GetExtension(firstCodingFile.FileName).ToLower();
                    var newName = "一次编码表_" + Guid.NewGuid() + fileExt;
                    var filePath = Request.MapPath("~/FileUpload");
                    var fullName = Path.Combine(filePath, newName);
                    firstCodingFile.SaveAs(fullName);

                    sampleDelegation.FirstCodingFileName = newName;
                    sampleDelegation.FirstCodingInputDate = DateTime.Now;
                    sampleDelegation.FirstCodingInputPerson = user.UserID;
                    sampleDelegation.FirstCodingInputPersonName = user.UserName;
                }
                if (sampleDelegationFirstFile != null)
                {
                    var fileExt = Path.GetExtension(sampleDelegationFirstFile.FileName).ToLower();
                    var newName = "送样委托单_" + Guid.NewGuid() + fileExt;
                    var filePath = Request.MapPath("~/FileUpload");
                    var fullName = Path.Combine(filePath, newName);
                    sampleDelegationFirstFile.SaveAs(fullName);

                    //添加送样委托单信息，供查看进度
                    sampleDelegation.SampleDelegationFileName = newName;
                    sampleDelegation.SampleDelegationInputDate = DateTime.Now;
                    sampleDelegation.SampleDelegationInputPerson = user.UserID;
                    sampleDelegation.SampleDelegationInputPersonName = user.UserName;

                    //第一次送样委托单填写，留存的记录
                    sampleDelegation.SampleDelegationFileNameOne = newName;
                    sampleDelegation.SampleDelegationInputDateOne = DateTime.Now;
                    sampleDelegation.SampleDelegationInputPersonOne = user.UserID;
                    sampleDelegation.SampleDelegationInputPersonNameOne = user.UserName;
                }
                db.SampleDelegation.Add(sampleDelegation);
                db.SaveChanges();
                return Json(new { state = "ok" });
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        //二次编码表、送样委托文件（技术处填写部分）上传
        [HttpPost]
        public JsonResult UploadSecondCodingFile(HttpPostedFileBase secondCodingFile, HttpPostedFileBase sampleDelegationFile)
        {
            var user = App_Code.Commen.GetUserFromSession();
            var id = 0;
            int.TryParse(Request.Form["secondInfoID"], out id);
            var sampleDelegation = db.SampleDelegation.Find(id);

            try
            {
                if (secondCodingFile != null)
                {
                    var fileExt = Path.GetExtension(secondCodingFile.FileName).ToLower();
                    var newName = "二次编码表_" + Guid.NewGuid() + fileExt;
                    var filePath = Request.MapPath("~/FileUpload");
                    var fullName = Path.Combine(filePath, newName);
                    secondCodingFile.SaveAs(fullName);

                    sampleDelegation.SecondCodingFileName = newName;
                    sampleDelegation.SecondCodingInputDate = DateTime.Now;
                    sampleDelegation.SecondCodingInputPerson = user.UserID;
                    sampleDelegation.SecondCodingInputPersonName = user.UserName;
                }
                if (sampleDelegationFile != null)
                {
                    var fileExt = Path.GetExtension(sampleDelegationFile.FileName).ToLower();
                    var newName = "送样委托单_" + Guid.NewGuid() + fileExt;
                    var filePath = Request.MapPath("~/FileUpload");
                    var fullName = Path.Combine(filePath, newName);
                    sampleDelegationFile.SaveAs(fullName);

                    //添加送样委托单信息，供查看进度
                    sampleDelegation.SampleDelegationFileName = newName;
                    sampleDelegation.SampleDelegationInputDate = DateTime.Now;
                    sampleDelegation.SampleDelegationInputPerson = user.UserID;
                    sampleDelegation.SampleDelegationInputPersonName = user.UserName;
                }
                sampleDelegation.SampleDelegationState = "二次编码表上传完成";
                db.SaveChanges();
                return Json(new { state = "ok" });
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        //检验报告上传
        [HttpPost]
        public JsonResult UploadCheckReportFile(HttpPostedFileBase checkReportFile)
        {
            var user = App_Code.Commen.GetUserFromSession();
            var id = 0;
            int.TryParse(Request.Form["CheckReportInfoID"], out id);
            var sampleDelegation = db.SampleDelegation.Find(id);

            try
            {
                if (checkReportFile != null)
                {
                    var fileExt = Path.GetExtension(checkReportFile.FileName).ToLower();
                    var newName = "检验报告_" + Guid.NewGuid() + fileExt;
                    var filePath = Request.MapPath("~/FileUpload");
                    var fullName = Path.Combine(filePath, newName);
                    checkReportFile.SaveAs(fullName);

                    sampleDelegation.CheckReportFileName = newName;
                    sampleDelegation.CheckReportInputDate = DateTime.Now;
                    sampleDelegation.CheckReportInputPerson = user.UserID;
                    sampleDelegation.CheckReportInputPersonName = user.UserName;
                }
                sampleDelegation.SampleDelegationState = "检验报告上传完成";
                db.SaveChanges();
                return Json(new { state = "ok" });
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
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
                                 s.StartTenderDate,
                                 FirstCodingFileName = isShow == 0 ? s.FirstCodingFileName : s.StartTenderDate <= DateTime.Now ? s.FirstCodingFileName : "",
                                 s.FirstCodingInputDate,
                                 s.FirstCodingInputPersonName,
                                 s.FirstCodingInputPerson,
                                 SecondCodingFileName = isShow == 0 ? s.SecondCodingFileName : s.StartTenderDate <= DateTime.Now ? s.SecondCodingFileName : "",
                                 s.SecondCodingInputDate,
                                 s.SecondCodingInputPersonName,
                                 s.SampleDelegationFileName,
                                 s.SampleDelegationInputDate,
                                 s.SampleDelegationInputPersonName,
                                 s.SampleDelegationFileNameOne,
                                 CheckReportFileName = isShow == 0 ? s.CheckReportFileName : s.StartTenderDate <= DateTime.Now ? s.CheckReportFileName : "",
                                 s.CheckReportInputDate,
                                 s.CheckReportInputPersonName,
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
                if (User.IsInRole("一次编码表录入"))
                {
                    result = result.Where(w => w.FirstCodingInputPerson == userInfo.UserID);
                }
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
                        var file1 = info.SampleDelegationFileName;
                        var file2 = info.SampleDelegationFileNameOne;
                        var file3 = info.FirstCodingFileName;
                        var file4 = info.SecondCodingFileName;
                        var file5 = info.CheckReportFileName;


                        var fullName1 = Path.Combine(filePath, file1 ?? "");
                        var fullName2 = Path.Combine(filePath, file2 ?? "");
                        var fullName3 = Path.Combine(filePath, file3 ?? "");
                        var fullName4 = Path.Combine(filePath, file4 ?? "");
                        var fullName5 = Path.Combine(filePath, file5 ?? "");
                        if (System.IO.File.Exists(fullName1))
                        {
                            System.IO.File.Delete(fullName1);
                        }
                        if (System.IO.File.Exists(fullName2))
                        {
                            System.IO.File.Delete(fullName2);
                        }
                        if (System.IO.File.Exists(fullName3))
                        {
                            System.IO.File.Delete(fullName3);
                        }
                        if (System.IO.File.Exists(fullName4))
                        {
                            System.IO.File.Delete(fullName4);
                        }
                        if (System.IO.File.Exists(fullName5))
                        {
                            System.IO.File.Delete(fullName5);
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
                            var file1 = info.SampleDelegationFileName;
                            var file2 = info.SecondCodingFileName;
                            var fullName1 = Path.Combine(filePath, file1);
                            var fullName2 = Path.Combine(filePath, file2);
                            if (System.IO.File.Exists(fullName1))
                            {
                                System.IO.File.Delete(fullName1);
                            }
                            if (System.IO.File.Exists(fullName2))
                            {
                                System.IO.File.Delete(fullName2);
                            }

                            info.SecondCodingFileName = null;
                            info.SecondCodingInputDate = null;
                            info.SecondCodingInputPerson = 0;
                            info.SecondCodingInputPersonName = null;

                            info.SampleDelegationFileName = null;
                            info.SampleDelegationInputDate = null;
                            info.SampleDelegationInputPerson = 0;
                            info.SampleDelegationInputPersonName = null;

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

                    if (User.IsInRole("检验报告录入"))
                    {
                        //质检人员执行删除，删除检验报告文件，将相应数据字段置空，变更状态未“二次编码表上传完成”
                        if (info.SampleDelegationState == "检验报告上传完成")
                        {
                            var file = info.CheckReportFileName;
                            var fullName = Path.Combine(filePath, file);
                            if (System.IO.File.Exists(fullName))
                            {
                                System.IO.File.Delete(fullName);
                            }
                            info.CheckReportFileName = null;
                            info.CheckReportInputDate = null;
                            info.CheckReportInputPerson = 0;
                            info.CheckReportInputPersonName = null;

                            info.SampleDelegationState = "二次编码表上传完成";

                            var log = new Models.Log();
                            var userInfo = App_Code.Commen.GetUserFromSession();
                            log.InputPersonID = userInfo.UserID;
                            log.InputPersonName = userInfo.UserName;
                            log.InputDateTime = DateTime.Now;
                            log.LogContent = "删除检验报告：样品名称【" + info.SampleName + "】开标时间【" + info.StartTenderDate + "】";
                            log.LogType = "送样委托删除";
                            db.Log.Add(log);
                        }
                        else
                        {
                            return "只能删除检验报告文件！";
                        }
                    }

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
    }
}