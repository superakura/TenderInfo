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
                    var newName = "一次编码表_" + DateTime.Now.ToString("yyyyMMddHHmmss") + fileExt;
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
                    var newName = "送样委托单_" + DateTime.Now.ToString("yyyyMMddHHmmss") + fileExt;
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
                    //var newName = "二次编码表_" + DateTime.Now.ToString("yyyyMMddHHmmss") + fileExt;
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
                    //var newName = "送样委托单_" + DateTime.Now.ToString("yyyyMMddHHmmss") + fileExt;
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
                int.TryParse(Request.Form["projectResponsiblePerson"],out projectResponsiblePerson);//招标负责人
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
                                 FirstCodingFileName =isShow==0?s.FirstCodingFileName: s.StartTenderDate <= DateTime.Now ? s.FirstCodingFileName : "",
                                 s.FirstCodingInputDate,
                                 s.FirstCodingInputPersonName,
                                 SecondCodingFileName = isShow == 0 ?s.SecondCodingFileName: s.StartTenderDate <= DateTime.Now ? s.SecondCodingFileName : "",
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
                if (projectResponsiblePerson!=0)
                {
                    result = result.Where(w => w.ProjectResponsiblePerson== projectResponsiblePerson);
                }
                if (!string.IsNullOrEmpty(sampleDelegationState))
                {
                    result = result.Where(w => w.SampleDelegationState==sampleDelegationState);
                }
                if (!string.IsNullOrEmpty(startTenderDateBegin) & !string.IsNullOrEmpty(startTenderDateEnd))
                {
                    var dateStart = Convert.ToDateTime(startTenderDateBegin);
                    var dateEnd = Convert.ToDateTime(startTenderDateEnd);
                    result = result.Where(w => System.Data.Entity.DbFunctions.DiffDays(w.StartTenderDate, dateStart) <= 0 && System.Data.Entity.DbFunctions.DiffDays(w.StartTenderDate, dateEnd) >= 0);
                }
                if (User.IsInRole("二次编码表录入"))
                {
                    result = result.Where(w => w.SampleDelegationAcceptPerson == userInfo.UserID);
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

        [HttpPost]
        public JsonResult GetProjectResponsiblePersonList()
        {
            var list = from u in db.UserInfo
                       join ur in db.UserRole on u.UserID equals ur.UserID
                       join r in db.RoleInfo on ur.RoleID equals r.RoleID
                       join ra in db.RoleAuthority on r.RoleID equals ra.RoleID
                       where ra.AuthorityID == 48//样品检测业务员查看
                       select new { u.UserID, u.UserName, u.UserNum };
            return Json(list);
        }
    }
}