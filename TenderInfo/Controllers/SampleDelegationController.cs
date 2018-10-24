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

        /// <summary>
        /// 样品委托录入视图
        /// </summary>
        /// <returns></returns>
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
            if (User.IsInRole("质检领导确认"))
            {
                ViewBag.PageRole = "质检领导确认";
            }
            return View();
        }

        /// <summary>
        /// 添加送样委托单
        /// </summary>
        /// <returns>ok</returns>
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

        /// <summary>
        /// 获取送样委托信息列表
        /// </summary>
        /// <returns>json</returns>
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
                                 FirstCodingFileName = s.CheckResultAllError == "全否" ? s.FirstCodingFileName : isShow == 0 ? s.FirstCodingFileName : s.StartTenderDate <= DateTime.Now ? s.FirstCodingFileName : "",
                                 s.FirstCodingInputDate,
                                 s.InputPerson,
                                 s.InputPersonName,
                                 s.InputDateTime,
                                 SecondCodingFileName = s.CheckResultAllError == "全否" ? s.SecondCodingFileName : isShow == 0 ? s.SecondCodingFileName : s.StartTenderDate <= DateTime.Now ? s.SecondCodingFileName : "",
                                 s.SecondCodingInputDate,
                                 ProjectResponsiblePersonName = up.UserName,
                                 ProjectResponsiblePersonPhone = up.UserPhone + "/" + up.UserMobile,
                                 s.ProjectResponsiblePerson,
                                 SampleDelegationAcceptPersonName = ua.UserName,
                                 SampleDelegationAcceptPersonPhone = ua.UserPhone + "/" + ua.UserMobile,
                                 s.SampleDelegationAcceptPerson,
                                 s.ChangeStartTenderDateState,
                                 s.CheckResultAllError,
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
                    result = result.Where(w => w.InputPerson == userInfo.UserID);
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

                var sampleDelegationList = result.Skip(offset).Take(limit).ToList();
                List<Models.ViewSampleDelegation> viewList = new List<Models.ViewSampleDelegation>();
                foreach (var item in sampleDelegationList)
                {
                    var viewRow = new Models.ViewSampleDelegation();
                    var checkFile = db.CheckReportFile.Where(w => w.SampleDelegationID == item.SampleDelegationID).ToList();
                    viewRow.sampleDelegation = item;
                    if (User.IsInRole("样品检测业务员查看"))
                    {
                        if (item.CheckResultAllError == "全否")
                        {
                            viewRow.checkReportFile = checkFile;
                        }
                        else
                        {
                            if (item.StartTenderDate <= DateTime.Now)
                            {
                                viewRow.checkReportFile = checkFile;
                            }
                            else
                            {
                                viewRow.checkReportFile = null;
                            }
                        }
                    }
                    else
                    {
                        viewRow.checkReportFile = checkFile;
                    }

                    viewList.Add(viewRow);
                }
                return Json(new { total = result.Count(), rows = viewList });
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        /// <summary>
        /// 获取技术处接收人员列表
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 获取招标业务员列表
        /// </summary>
        /// <returns>json</returns>
        [HttpPost]
        public JsonResult GetProjectResponsiblePersonList()
        {
            var list = from u in db.UserInfo
                       join ur in db.UserRole on u.UserID equals ur.UserID
                       join r in db.RoleInfo on ur.RoleID equals r.RoleID
                       join ra in db.RoleAuthority on r.RoleID equals ra.RoleID
                       //where ra.AuthorityID == 48//样品检测业务员查看
                       where ra.AuthorityID == 50//招标管理
                       select new { u.UserID, u.UserName, u.UserNum };

            var user = App_Code.Commen.GetUserFromSession();
            if (User.IsInRole("新建招标台账"))
            {
                return Json(list);
            }
            if (User.IsInRole("招标管理"))
            {
                list = list.Where(w => w.UserID == user.UserID);
            }
            return Json(list);
        }

        /// <summary>
        /// 获取招标进度业务员列表
        /// </summary>
        /// <returns>json</returns>
        [HttpPost]
        public JsonResult GetProgressResponsiblePersonList()
        {
            var list = from u in db.UserInfo
                       join ur in db.UserRole on u.UserID equals ur.UserID
                       join r in db.RoleInfo on ur.RoleID equals r.RoleID
                       join ra in db.RoleAuthority on r.RoleID equals ra.RoleID
                       //where ra.AuthorityID == 48//样品检测业务员查看
                       where ra.AuthorityID == 50//招标管理
                       select new { u.UserID, u.UserName, u.UserNum };

            var user = App_Code.Commen.GetUserFromSession();
            if (User.IsInRole("招标管理"))
            {
                list = list.Where(w => w.UserID == user.UserID);
            }
            return Json(list);
        }

        /// <summary>
        /// 获取样品检测业务员列表
        /// </summary>
        /// <returns>json</returns>
        [HttpPost]
        public JsonResult GetSampleResponsiblePersonList()
        {
            var list = from u in db.UserInfo
                       join ur in db.UserRole on u.UserID equals ur.UserID
                       join r in db.RoleInfo on ur.RoleID equals r.RoleID
                       join ra in db.RoleAuthority on r.RoleID equals ra.RoleID
                       where ra.AuthorityID == 48//样品检测业务员查看
                       //where ra.AuthorityID == 50//招标管理
                       select new { u.UserID, u.UserName, u.UserNum };

            var user = App_Code.Commen.GetUserFromSession();
            if (User.IsInRole("样品检测业务员查看"))
            {
                list = list.Where(w => w.UserID == user.UserID);
            }
            return Json(list);
        }

        /// <summary>
        /// 修改招标开始时间
        /// </summary>
        /// <param name="editTenderDateFile"></param>
        /// <returns>json</returns>
        [HttpPost]
        public string EditTenderStartDate(HttpPostedFileBase editTenderDateFile)
        {
            try
            {
                var user = App_Code.Commen.GetUserFromSession();
                var id = 0;
                int.TryParse(Request.Form["EditTenderDateInfoID"], out id);
                var editTenderStartDate = Convert.ToDateTime(Request.Form["tbxEditTenderDate"]);//要修改的招标开始时间
                var editTenderDateReason = Request.Form["tbxEditTenderDateReason"];//修改原因
                var sampleDelegation = db.SampleDelegation.Find(id);

                if (CheckTenderStartDateTime(sampleDelegation.StartTenderDate) == 0)
                {
                    if (sampleDelegation.ChangeStartTenderDateState == "修改中")
                    {
                        return "error";
                    }

                    //上传修改招标开始时间说明文件
                    var newName = string.Empty;
                    if (editTenderDateFile != null)
                    {
                        var fileExt = Path.GetExtension(editTenderDateFile.FileName).ToLower();
                        newName = "修改招标开始时间说明文件_" + DateTime.Now.ToString("yyMMddhhmmss") + DateTime.Now.Millisecond + fileExt;
                        var filePath = Request.MapPath("~/FileUpload");
                        var fullName = Path.Combine(filePath, newName);
                        editTenderDateFile.SaveAs(fullName);
                    }

                    var log = new Models.Log();
                    log.InputDateTime = DateTime.Now;
                    log.InputPersonID = user.UserID;
                    log.InputPersonName = user.UserName;
                    log.LogDataID = id;
                    log.LogType = "修改招标开始时间";
                    log.LogContent = "开标时间由【" + sampleDelegation.StartTenderDate + "】修改为【" + editTenderStartDate + "】";
                    log.LogReason = editTenderDateReason;
                    log.Col1 = newName;//招标开始时间修改说明文件
                    log.Col2 = sampleDelegation.StartTenderDate.ToString();//原招标开始时间
                    log.Col3 = editTenderStartDate.ToString();//要修改的招标开始时间
                    db.Log.Add(log);

                    sampleDelegation.ChangeStartTenderDateState = "修改中";
                    db.SaveChanges();
                    return "ok";
                }
                else
                {
                    //如果超过开标时间，则不能删除检验报告文件
                    return "errorTime";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 获取修改招标开始时间记录信息
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 招标科长送样委托删除操作，【删除修改开标时间、质检审批log】，【一、二次编码表文件，检验报告文件】
        /// </summary>
        /// <returns>ok</returns>
        [HttpPost]
        public string Del()
        {
            try
            {
                var infoList =
  JsonConvert.DeserializeObject<Dictionary<String, Object>>(HttpUtility.UrlDecode(Request.Form.ToString()));
                var id = 0;//送样委托单ID
                int.TryParse(infoList["id"].ToString(), out id);

                var filePath = Request.MapPath("~/FileUpload");
                var info = db.SampleDelegation.Find(id);

                //超过开标时间，禁止删除记录
                if (CheckTenderStartDateTime(info.StartTenderDate) == 0)
                {
                    #region 删除一、二次编码表
                    var fileFirst = info.FirstCodingFileName;
                    var fileSecond = info.SecondCodingFileName;

                    var fullNameFirst = Path.Combine(filePath, fileFirst ?? "");
                    var fullNameSecond = Path.Combine(filePath, fileSecond ?? "");
                    //删除一次编码表
                    if (System.IO.File.Exists(fullNameFirst))
                    {
                        System.IO.File.Delete(fullNameFirst);
                    }
                    //删除二次编码表
                    if (System.IO.File.Exists(fullNameSecond))
                    {
                        System.IO.File.Delete(fullNameSecond);
                    }
                    #endregion

                    #region 删除检验报告文件和表记录
                    var checkFileList = db.CheckReportFile.Where(w => w.SampleDelegationID == id).ToList();
                    //循环删除检验报告文件
                    foreach (var item in checkFileList)
                    {
                        var fullNameCheckFile = Path.Combine(filePath, item.CheckReportFileName ?? "");
                        if (System.IO.File.Exists(fullNameCheckFile))
                        {
                            System.IO.File.Delete(fullNameCheckFile);
                        }
                    }
                    //删除检验报告表信息
                    db.CheckReportFile.RemoveRange(checkFileList);
                    #endregion

                    #region 将删除送样委托单操作，写入日志信息
                    var log = new Models.Log();
                    var userInfo = App_Code.Commen.GetUserFromSession();
                    log.InputPersonID = userInfo.UserID;
                    log.InputPersonName = userInfo.UserName;
                    log.InputDateTime = DateTime.Now;
                    log.LogContent = "删除送样委托单：样品名称【" + info.SampleName + "】开标时间【" + info.StartTenderDate + "】";
                    log.LogType = "删除送样委托单";
                    db.Log.Add(log);
                    #endregion

                    #region 删除修改招标时间日志，删除送样委托质检审核日志，删除修改招标开始时间说明文件
                    var logList = db.Log.Where(w => w.LogDataID == id).ToList();
                    foreach (var item in logList)
                    {
                        var logFile = Path.Combine(filePath, item.Col1 ?? "");
                        if (System.IO.File.Exists(logFile))
                        {
                            System.IO.File.Delete(logFile);
                        }
                    }
                    db.Log.RemoveRange(logList);
                    #endregion

                    db.SampleDelegation.Remove(info);//最后删除送样委托单记录
                    db.SaveChanges();
                    return "ok";
                }
                else
                {
                    return "errorTime";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 添加检验技术要求
        /// </summary>
        /// <returns>ok</returns>
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
                if (CheckTenderStartDateTime(info.StartTenderDate) == 0)
                {
                    var content = infoList["content"].ToString();
                    info.SampleTechnicalRequirement = content;
                    info.SampleDelegationState = "质检接收审核";

                    var log = new Models.Log();
                    var userInfo = App_Code.Commen.GetUserFromSession();
                    log.InputPersonID = userInfo.UserID;
                    log.InputPersonName = userInfo.UserName;
                    log.InputDateTime = DateTime.Now;
                    log.LogDataID = id;
                    log.LogContent = "样品检验技术要求更新：样品名称【" + info.SampleName + "】";
                    log.LogType = "送样委托质检审核";
                    db.Log.Add(log);

                    db.SaveChanges();
                    return "ok";
                }
                else
                {
                    return "errorTime";
                }
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
                var id = 0;
                int.TryParse(Request.Form["UploadInfoID"], out id);
                var info = db.SampleDelegation.Find(id);

                if (CheckTenderStartDateTime(info.StartTenderDate) == 0)
                {
                    if (uploadFile == null)
                    {
                        return "noFile";
                    }
                    var filePath = Request.MapPath("~/FileUpload");
                    var user = App_Code.Commen.GetUserFromSession();

                    if (User.IsInRole("一次编码表录入"))
                    {
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
                        checkReportFile.SampleDelegationID = id;
                        db.CheckReportFile.Add(checkReportFile);
                    }

                    db.SaveChanges();
                    return "ok";
                }
                else
                {
                    //如果超过开标时间，则不能删除检验报告文件
                    return "errorTime";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 质检技术人员审核，确认接收委托单
        /// </summary>
        /// <returns>ok</returns>
        [HttpPost]
        public string AuditOk()
        {
            try
            {
                var infoList =
  JsonConvert.DeserializeObject<Dictionary<String, Object>>(HttpUtility.UrlDecode(Request.Form.ToString()));
                var id = 0;
                int.TryParse(infoList["id"].ToString(), out id);
                var info = db.SampleDelegation.Find(id);

                info.SampleDelegationState = "质检领导确认";

                var log = new Models.Log();
                var userInfo = App_Code.Commen.GetUserFromSession();
                log.InputPersonID = userInfo.UserID;
                log.InputPersonName = userInfo.UserName;
                log.InputDateTime = DateTime.Now;
                log.LogDataID = id;
                log.LogContent = "质检技术人员同意接收：样品名称【" + info.SampleName + "】";
                log.LogType = "送样委托质检审核";
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
        /// 质检技术人员审核，回退委托单，添加回退原因
        /// </summary>
        /// <returns>ok</returns>
        [HttpPost]
        public string AuditBack()
        {
            try
            {
                var infoList =
  JsonConvert.DeserializeObject<Dictionary<String, Object>>(HttpUtility.UrlDecode(Request.Form.ToString()));
                var id = 0;
                int.TryParse(infoList["id"].ToString(), out id);

                var backReason = infoList["reason"].ToString();
                var info = db.SampleDelegation.Find(id);

                info.SampleDelegationState = "质检接收回退";

                var log = new Models.Log();
                var userInfo = App_Code.Commen.GetUserFromSession();
                log.InputPersonID = userInfo.UserID;
                log.InputPersonName = userInfo.UserName;
                log.InputDateTime = DateTime.Now;
                log.LogDataID = id;
                log.LogContent = "质检回退送样委托：样品名称【" + info.SampleName + "】";
                log.LogReason = backReason;
                log.LogType = "送样委托质检审核";
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
        /// 获取送样委托单质检接收审核过程
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetAuditLog()
        {
            try
            {
                var infoList =
  JsonConvert.DeserializeObject<Dictionary<String, Object>>(HttpUtility.UrlDecode(Request.Form.ToString()));
                var id = 0;
                int.TryParse(infoList["id"].ToString(), out id);

                return Json(db.Log.Where(w => w.LogType == "送样委托质检审核" && w.LogDataID == id).OrderBy(o => o.InputDateTime).ToList());
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        /// <summary>
        /// 质检领导确认
        /// </summary>
        /// <returns>ok</returns>
        [HttpPost]
        public string AuditLeader()
        {
            try
            {
                var infoList =
  JsonConvert.DeserializeObject<Dictionary<String, Object>>(HttpUtility.UrlDecode(Request.Form.ToString()));
                var id = 0;
                int.TryParse(infoList["id"].ToString(), out id);
                var info = db.SampleDelegation.Find(id);

                info.SampleDelegationState = "检验报告上传";

                var log = new Models.Log();
                var userInfo = App_Code.Commen.GetUserFromSession();
                log.InputPersonID = userInfo.UserID;
                log.InputPersonName = userInfo.UserName;
                log.InputDateTime = DateTime.Now;
                log.LogDataID = id;
                log.LogContent = "质检领导确认：样品名称【" + info.SampleName + "】";
                log.LogType = "送样委托质检审核";
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
        /// 删除已上传的检验报告文件
        /// </summary>
        /// <returns>ok</returns>
        [HttpPost]
        public string DelCheckFile()
        {
            try
            {
                var infoList =
  JsonConvert.DeserializeObject<Dictionary<String, Object>>(HttpUtility.UrlDecode(Request.Form.ToString()));
                var id = 0;
                int.TryParse(infoList["id"].ToString(), out id);
                var info = db.CheckReportFile.Find(id);
                var sampleDelegation = db.SampleDelegation.Find(info.SampleDelegationID);
                if (CheckTenderStartDateTime(sampleDelegation.StartTenderDate) == 0)
                {
                    var filePath = Request.MapPath("~/FileUpload");

                    if (info.CheckReportFileName != null)
                    {
                        var file = info.CheckReportFileName;
                        var fullName = Path.Combine(filePath, file ?? "");
                        if (System.IO.File.Exists(fullName))
                        {
                            System.IO.File.Delete(fullName);
                        }
                    }
                    db.CheckReportFile.Remove(info);
                    db.SaveChanges();
                    return "ok";
                }
                else
                {
                    //如果超过开标时间，则不能删除检验报告文件
                    return "errorTime";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 招标中心领导，审核招标开始时间，同意
        /// </summary>
        /// <returns>ok</returns>
        [HttpPost]
        public string ChangeTenderDateOk()
        {
            try
            {
                var infoList =
  JsonConvert.DeserializeObject<Dictionary<String, Object>>(HttpUtility.UrlDecode(Request.Form.ToString()));
                var id = 0;
                int.TryParse(infoList["id"].ToString(), out id);

                var info = db.SampleDelegation.Find(id);
                var logInfo = db.Log
                    .Where(w => w.LogDataID == id && w.LogType == "修改招标开始时间")
                    .OrderBy(o => o.InputDateTime)
                    .ToList();

                info.ChangeStartTenderDateState = null;//将修改招标开始时间状态变为null
                info.StartTenderDate = Convert.ToDateTime(logInfo.Last().Col3);//领导同意后，将要修改的招标开始时间进行赋值

                var log = new Models.Log();
                var userInfo = App_Code.Commen.GetUserFromSession();
                log.InputPersonID = userInfo.UserID;
                log.InputPersonName = userInfo.UserName;
                log.InputDateTime = DateTime.Now;
                log.LogDataID = id;
                log.LogContent = "修改开标时间审核通过：样品名称【" + info.SampleName + "】";
                log.LogType = "修改招标开始时间";
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
        /// 招标中心领导，审核招标开始时间，回退
        /// </summary>
        /// <returns>ok</returns>
        [HttpPost]
        public string ChangeTenderDateBack()
        {
            try
            {
                var infoList =
  JsonConvert.DeserializeObject<Dictionary<String, Object>>(HttpUtility.UrlDecode(Request.Form.ToString()));
                var id = 0;
                int.TryParse(infoList["id"].ToString(), out id);
                var backReason = infoList["reason"].ToString();

                var info = db.SampleDelegation.Find(id);
                var logInfo = db.Log.Where(w => w.LogDataID == id && w.LogType == "修改招标开始时间").FirstOrDefault();
                info.ChangeStartTenderDateState = null;//将修改招标开始时间状态变为null

                var log = new Models.Log();
                var userInfo = App_Code.Commen.GetUserFromSession();
                log.InputPersonID = userInfo.UserID;
                log.InputPersonName = userInfo.UserName;
                log.InputDateTime = DateTime.Now;
                log.LogDataID = id;
                log.Col4 = backReason;
                log.LogContent = "修改开标始时间审核回退：样品名称【" + info.SampleName + "】";
                log.LogType = "修改招标开始时间";
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
        /// 判断招标开始时间是否大于当前时间,小于等于返回1（int），大于返回0（int）
        /// </summary>
        /// <returns>int</returns>
        public int CheckTenderStartDateTime(DateTime? tenderStartDateTime)
        {
            return tenderStartDateTime <= DateTime.Now ? 1 : 0;
        }


        /// <summary>
        /// 设置送样委托检验报告全否，如果全否，则不到开标日期也显示所以文件
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string SetCheckFileAllError()
        {
            try
            {
                var infoList =
  JsonConvert.DeserializeObject<Dictionary<String, Object>>(HttpUtility.UrlDecode(Request.Form.ToString()));
                var id = 0;//送样委托单ID
                int.TryParse(infoList["id"].ToString(), out id);
                var info = db.SampleDelegation.Find(id);
                if (CheckTenderStartDateTime(info.StartTenderDate) == 0)
                {
                    info.CheckResultAllError = "全否";
                    db.SaveChanges();

                    return "ok";
                }
                else
                {
                    //如果超过开标时间，则不能删除检验报告文件
                    return "errorTime";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 取消送样委托检验报告全否
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string ResetCheckFileAllError()
        {
            try
            {
                var infoList =
  JsonConvert.DeserializeObject<Dictionary<String, Object>>(HttpUtility.UrlDecode(Request.Form.ToString()));
                var id = 0;//送样委托单ID
                int.TryParse(infoList["id"].ToString(), out id);
                var info = db.SampleDelegation.Find(id);
                if (CheckTenderStartDateTime(info.StartTenderDate) == 0)
                {
                    info.CheckResultAllError = null;
                    db.SaveChanges();

                    return "ok";
                }
                else
                {
                    //如果超过开标时间，则不能删除检验报告文件
                    return "errorTime";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        //if (CheckTenderStartDateTime(sampleDelegation.StartTenderDate) == 0)
        //        {

        //    }
        //    else
        //        {
        //            如果超过开标时间，则不能删除检验报告文件
        //            return "errorTime";
        //        }
    }
}