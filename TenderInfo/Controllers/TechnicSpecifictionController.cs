using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TenderInfo.Controllers
{
    public class TechnicSpecifictionController : Controller
    {
        private Models.DB db = new Models.DB();
        // TechnicSpecifiction

        #region view

        /// <summary>
        /// 技术规格书审批--最低价法视图
        /// </summary>
        /// <returns></returns>
        public ViewResult MinPrice()
        {
            if (User.IsInRole("技术规格书提报"))
            {
                ViewBag.userRole = "技术规格书提报";
            }
            if (User.IsInRole("技术规格书上传"))
            {
                ViewBag.userRole = "技术规格书上传";
            }
            if (User.IsInRole("技术规格书审批"))
            {
                ViewBag.userRole = "技术规格书审批";
            }
            return View();
        }

        /// <summary>
        /// 技术规格书审批--综合评标法视图
        /// </summary>
        /// <returns></returns>
        public ViewResult Comprehensive()
        {
            if (User.IsInRole("技术规格书提报"))
            {
                ViewBag.userRole = "技术规格书提报";
            }
            return View();
        }

        /// <summary>
        /// 技术规格书审批--综合评标法添加视图
        /// </summary>
        /// <returns></returns>
        public ViewResult AddComprehensive()
        {
            return View();
        }

        public ViewResult ApproveComprehensive()
        {
            return View();
        }

        /// <summary>
        /// 技术规格书审批后上传视图
        /// </summary>
        /// <returns></returns>
        public ViewResult Upload()
        {
            return View();
        }

        /// <summary>
        /// 技术规格书查询视图
        /// </summary>
        /// <returns></returns>
        public ViewResult select()
        {
            return View();
        }

        /// <summary>
        /// 转跳到技术规格书查询
        /// </summary>
        /// <returns></returns>
        public RedirectResult SearchRedirect()
        {
            return Redirect("http://10.126.10.64:8020/");
        }

        /// <summary>
        /// 技术规格书查询视图
        /// </summary>
        /// <returns></returns>
        public ViewResult Search()
        {
            return View();
        }
        #endregion

        #region MinPrice--最低价评标法审批

        /// <summary>
        /// 最低价评标法文件上传，提交审核
        /// </summary>
        /// <param name="fileMinPrice"></param>
        /// <returns></returns>
        [HttpPost]
        public string InsertMinPriceFile(HttpPostedFileBase fileMinPrice)
        {
            try
            {
                #region 获取一、二级审批人员ID列表

                var firstList = Request.Form["tbxFirstList"].ToString().Substring(0, Request.Form["tbxFirstList"].ToString().Length - 1);
                var secondList = string.Empty;
                if (Request.Form["tbxSecondList"] != string.Empty)
                {
                    secondList = Request.Form["tbxSecondList"].ToString().Substring(0, Request.Form["tbxSecondList"].ToString().Length - 1);
                }

                var firstApprovePersonIDStr = firstList.Split('&');
                var secondApprovePersonIDStr = secondList == "" ? null : secondList.Split('&');

                List<int> firstApprovePersonID = new List<int>();
                List<int> secondApprovePersonID = new List<int>();

                for (int i = 0; i < firstApprovePersonIDStr.Length; i++)
                {
                    firstApprovePersonID.Add(Convert.ToInt32(firstApprovePersonIDStr[i]));
                }

                if (secondApprovePersonIDStr != null)
                {
                    for (int i = 0; i < secondApprovePersonIDStr.Length; i++)
                    {
                        secondApprovePersonID.Add(Convert.ToInt32(secondApprovePersonIDStr[i]));
                    }
                }
                #endregion

                var userInfo = App_Code.Commen.GetUserFromSession();
                var info = new Models.FileMinPrice();
                var newName = string.Empty;
                var showName = string.Empty;
                if (fileMinPrice != null)
                {
                    var fileExt = Path.GetExtension(fileMinPrice.FileName).ToLower();
                    var fileName = Path.GetFileNameWithoutExtension(fileMinPrice.FileName).ToLower();
                    newName = fileName + Guid.NewGuid() + fileExt;
                    var filePath = Request.MapPath("~/FileUpload");
                    var fullName = Path.Combine(filePath, newName);
                    fileMinPrice.SaveAs(fullName);
                    info.TechnicSpecificationFile = newName;
                    info.TechnicSpecificationFileShow = fileName + fileExt;
                    showName = fileName + fileExt;
                }
                info.ApproveState = "待审核";
                info.ApproveLevel = secondApprovePersonIDStr == null ? "一级" : "二级";
                info.InputDateTime = DateTime.Now;
                info.InputPersonID = userInfo.UserID;
                info.InputPersonName = userInfo.UserName;

                var userDeptInfo = db.DeptInfo.Where(w => w.DeptID == userInfo.UserDeptID).FirstOrDefault();
                info.InputPersonDeptID = userDeptInfo.DeptID;
                info.InputPersonDeptName = userDeptInfo.DeptName;
                var userFatherDeptInfo = db.DeptInfo.Where(w => w.DeptID == userDeptInfo.DeptFatherID).FirstOrDefault();
                info.InputPersonFatherDeptID = userDeptInfo.DeptFatherID;
                info.InputPersonFatherDeptName = userFatherDeptInfo.DeptName;

                db.FileMinPrice.Add(info);
                db.SaveChanges();

                List<Models.FileMinPriceChild> list = new List<Models.FileMinPriceChild>();
                foreach (var item in firstApprovePersonID)
                {
                    var first = new Models.FileMinPriceChild();
                    first.ApproveLevel = "一级";
                    first.ApproveState = "待审批";
                    first.ApprovePersonID = item;
                    var firstUser = db.UserInfo.Where(w => w.UserID == item).FirstOrDefault();
                    first.ApprovePersonName = firstUser.UserName;
                    first.ApprovePersonDeptID = firstUser.UserDeptID;
                    var deptInfo = db.DeptInfo.Where(w => w.DeptID == firstUser.UserDeptID).FirstOrDefault();
                    first.ApprovePersonDeptName = deptInfo.DeptName;
                    var deptFatherInfo = db.DeptInfo.Where(w => w.DeptID == deptInfo.DeptFatherID).FirstOrDefault();
                    first.ApprovePersonFatherDeptName = deptFatherInfo.DeptName;
                    first.ApprovePersonFatherDeptID = deptInfo.DeptFatherID;
                    first.FileMinPriceID = info.FileMinPriceID;

                    list.Add(first);
                }
                foreach (var item in secondApprovePersonID)
                {
                    var second = new Models.FileMinPriceChild();
                    second.ApproveLevel = "二级";
                    second.ApproveState = "待审批";
                    second.ApprovePersonID = item;
                    var secondUser = db.UserInfo.Where(w => w.UserID == item).FirstOrDefault();
                    second.ApprovePersonName = secondUser.UserName;
                    second.ApprovePersonDeptID = secondUser.UserDeptID;
                    var deptInfo = db.DeptInfo.Where(w => w.DeptID == secondUser.UserDeptID).FirstOrDefault();
                    second.ApprovePersonDeptName = deptInfo.DeptName;
                    var deptFatherInfo = db.DeptInfo.Where(w => w.DeptID == deptInfo.DeptFatherID).FirstOrDefault();
                    second.ApprovePersonFatherDeptName = deptFatherInfo.DeptName;
                    second.ApprovePersonFatherDeptID = deptInfo.DeptFatherID;
                    second.FileMinPriceID = info.FileMinPriceID;
                    list.Add(second);
                }
                db.FileMinPriceChild.AddRange(list);

                #region 写入日志，新建技术规格书文件审批
                var log = new Models.Log();
                log.LogType = "最低价法审批";
                log.LogDataID = info.FileMinPriceID;
                log.InputDateTime = DateTime.Now;
                log.InputPersonID = userInfo.UserID;
                log.InputPersonName = userInfo.UserName;
                log.LogContent = "新建技术规格书审批";
                log.Col1 = newName;//记录上传存储的文件名
                log.Col2 = showName;//记录上传显示的文件名
                db.Log.Add(log);
                #endregion
                db.SaveChanges();

                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 获取最低价技术规格书审批列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetMinPriceList()
        {
            try
            {
                var limit = 0;
                int.TryParse(Request.Form["limit"], out limit);
                var offset = 0;
                int.TryParse(Request.Form["offset"], out offset);
                var fileName = Request.Form["tbxFileNameSearch"];//文件名称
                var approveLevel = Request.Form["ddlApproveLevelSearch"];//审批级别
                var approveState = Request.Form["ddlApproveStateSearch"];//审批状态
                var inputPersonFatherDeptID = 0;
                int.TryParse(Request.Form["ddlApproveDeptSearch"], out inputPersonFatherDeptID);

                var userInfo = App_Code.Commen.GetUserFromSession();
                var result = from m in db.FileMinPrice
                             select m;
                if (User.IsInRole("技术规格书提报"))
                {
                    result = result.Where(w => w.InputPersonID == userInfo.UserID);
                }
                if (User.IsInRole("技术规格书审批"))
                {
                    var approvePersonIDList = db.FileMinPriceChild
                        .Where(w => w.ApprovePersonID == userInfo.UserID)
                        .Select(s => s.FileMinPriceID)
                        .ToList();
                    result = result.Where(w => approvePersonIDList.Contains(w.FileMinPriceID));
                }
                if (!string.IsNullOrEmpty(fileName))
                {
                    result = result.Where(w => w.TechnicSpecificationFileShow.Contains(fileName));
                }
                if (!string.IsNullOrEmpty(approveLevel))
                {
                    result = result.Where(w => w.ApproveLevel == approveLevel);
                }
                if (!string.IsNullOrEmpty(approveState))
                {
                    result = result.Where(w => w.ApproveState == approveState);
                }
                if (inputPersonFatherDeptID != 0)
                {
                    result = result.Where(w => w.InputPersonFatherDeptID == inputPersonFatherDeptID);
                }
                return Json(new { total = result.Count(), rows = result.OrderByDescending(o => o.InputDateTime).Skip(offset).Take(limit).ToList() });
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        /// <summary>
        /// 一级审批，根据审批类型判断同意或回退
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string FirstApprove()
        {
            try
            {
                var fileMinPriceID = 0;
                int.TryParse(Request["tbxFirstID"], out fileMinPriceID);

                var approveType = Request["tbxFirstType"];
                var backReason = Request["tbxFirstBackReason"];

                var userInfo = App_Code.Commen.GetUserFromSession();
                var fileMinPrice = db.FileMinPrice.Find(fileMinPriceID);
                var fileMinPriceChild = db.FileMinPriceChild
                    .Where(w => w.FileMinPriceID == fileMinPriceID && w.ApproveLevel == "一级" && w.ApprovePersonID == userInfo.UserID)
                    .FirstOrDefault();
                if (fileMinPriceChild.ApproveState != "待审批")
                {
                    return "不能重复进行审批！";
                }

                fileMinPriceChild.ApproveDateTime = DateTime.Now;
                if (approveType == "back")
                {
                    fileMinPriceChild.ApproveBackReason = backReason;
                    fileMinPriceChild.ApproveState = "一级审批回退";
                    #region 写入日志，一级审批回退原因
                    var log = new Models.Log();
                    log.LogType = "最低价法审批";
                    log.LogDataID = fileMinPriceID;
                    log.InputDateTime = DateTime.Now;
                    log.InputPersonID = userInfo.UserID;
                    log.InputPersonName = userInfo.UserName;
                    log.LogContent = "一级审批回退";
                    log.LogReason = backReason;
                    log.Col2 = fileMinPrice.TechnicSpecificationFileShow;
                    log.Col1 = fileMinPrice.TechnicSpecificationFile;
                    db.Log.Add(log);
                    #endregion
                }
                else
                {
                    fileMinPriceChild.ApproveState = "一级审批同意";
                }
                db.SaveChanges();

                #region 判断一级审批全部的完成状态
                var fileMinPriceInfo = db.FileMinPrice.Find(fileMinPriceID);
                var fileMinPriceChildFirstList = db.FileMinPriceChild
                    .Where(w => w.FileMinPriceID == fileMinPriceID && w.ApproveLevel == "一级")
                    .ToList();

                var okSum = 0;
                foreach (var item in fileMinPriceChildFirstList)
                {
                    if (item.ApproveState == "一级审批回退")
                    {
                        fileMinPriceInfo.ApproveState = "一级审批回退";
                    }
                    if (item.ApproveState == "一级审批同意")
                    {
                        okSum += 1;
                    }
                }

                if (okSum == fileMinPriceChildFirstList.Count)
                {
                    fileMinPriceInfo.ApproveState = "一级审批完成";
                }

                db.SaveChanges();
                #endregion
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 二级审批，根据审批类型判断同意或回退
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string SecondApprove()
        {
            try
            {
                var fileMinPriceID = 0;
                int.TryParse(Request["tbxSecondID"], out fileMinPriceID);

                var approveType = Request["tbxSecondType"];
                var backReason = Request["tbxSecondBackReason"];

                var userInfo = App_Code.Commen.GetUserFromSession();
                var fileMinPrice = db.FileMinPrice.Find(fileMinPriceID);
                var fileMinPriceChild = db.FileMinPriceChild
                    .Where(w => w.FileMinPriceID == fileMinPriceID && w.ApproveLevel == "二级" && w.ApprovePersonID == userInfo.UserID)
                    .FirstOrDefault();
                if (fileMinPriceChild.ApproveState != "待审批")
                {
                    return "不能重复进行审批！";
                }

                fileMinPriceChild.ApproveDateTime = DateTime.Now;
                if (approveType == "back")
                {
                    fileMinPriceChild.ApproveBackReason = backReason;
                    fileMinPriceChild.ApproveState = "二级审批回退";
                    #region 写入日志，二级审批回退原因
                    var log = new Models.Log();
                    log.LogType = "最低价法审批";
                    log.LogDataID = log.LogDataID = fileMinPriceID;
                    log.InputDateTime = DateTime.Now;
                    log.InputPersonID = userInfo.UserID;
                    log.InputPersonName = userInfo.UserName;
                    log.LogContent = "二级审批回退";
                    log.LogReason = backReason;
                    log.Col2 = fileMinPrice.TechnicSpecificationFileShow;
                    log.Col1 = fileMinPrice.TechnicSpecificationFile;
                    db.Log.Add(log);
                    #endregion
                }
                else
                {
                    fileMinPriceChild.ApproveState = "二级审批同意";
                }
                db.SaveChanges();

                #region 判断二级审批全部的完成状态
                var fileMinPriceInfo = db.FileMinPrice.Find(fileMinPriceID);
                var fileMinPriceChildSecondList = db.FileMinPriceChild
                    .Where(w => w.FileMinPriceID == fileMinPriceID && w.ApproveLevel == "二级")
                    .ToList();

                var okSum = 0;
                foreach (var item in fileMinPriceChildSecondList)
                {
                    if (item.ApproveState == "二级审批回退")
                    {
                        fileMinPriceInfo.ApproveState = "二级审批回退";
                    }
                    if (item.ApproveState == "二级审批同意")
                    {
                        okSum += 1;
                    }
                }

                if (okSum == fileMinPriceChildSecondList.Count)
                {
                    fileMinPriceInfo.ApproveState = "二级审批完成";
                }
                db.SaveChanges();
                #endregion
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 获取一级审批人员列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetFirstPersonList()
        {
            try
            {
                var infoList =
  JsonConvert.DeserializeObject<Dictionary<String, Object>>(HttpUtility.UrlDecode(Request.Form.ToString()));
                var minPriceID = 0;//最低价审批id
                int.TryParse(infoList["id"].ToString(), out minPriceID);
                var list = db.FileMinPriceChild.Where(w => w.FileMinPriceID == minPriceID && w.ApproveLevel == "一级").ToList();

                return Json(list);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        /// <summary>
        /// 获取二级审批人员列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetSecondPersonList()
        {
            try
            {
                var infoList =
  JsonConvert.DeserializeObject<Dictionary<String, Object>>(HttpUtility.UrlDecode(Request.Form.ToString()));
                var minPriceID = 0;//最低价审批id
                int.TryParse(infoList["id"].ToString(), out minPriceID);
                var list = db.FileMinPriceChild.Where(w => w.FileMinPriceID == minPriceID && w.ApproveLevel == "二级").ToList();

                return Json(list);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        /// <summary>
        /// 获取二级单位信息，供查询使用
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetDeptForSearch()
        {
            try
            {
                var userInfo = App_Code.Commen.GetUserFromSession();
                var list = db.DeptInfo.Where(w => w.DeptFatherID == 1);
                if (User.IsInRole("技术规格书提报"))
                {
                    var deptFatherID = db.DeptInfo.Where(w => w.DeptID == userInfo.UserDeptID).FirstOrDefault().DeptFatherID;
                    list = list.Where(w => w.DeptID == deptFatherID);
                }
                return Json(list.ToList());
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        /// <summary>
        /// 判断最低价法审批用户，是否能够执行一级审批操作
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string CheckMinPriceFirstPerson()
        {
            try
            {
                var infoList =
  JsonConvert.DeserializeObject<Dictionary<String, Object>>(HttpUtility.UrlDecode(Request.Form.ToString()));
                var minPriceID = 0;//最低价审批id
                int.TryParse(infoList["id"].ToString(), out minPriceID);
                var userInfo = App_Code.Commen.GetUserFromSession();
                var userIDList = db.FileMinPriceChild
                    .Where(w => w.FileMinPriceID == minPriceID && w.ApproveLevel == "一级" && w.ApproveState == "待审批")
                    .Select(s => s.ApprovePersonID)
                    .ToList();
                if (userIDList.Contains(userInfo.UserID))
                {
                    return "ok";
                }
                else
                {
                    return "error";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 判断最低价法审批用户，是否能够执行二级审批操作
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string CheckMinPriceSecondPerson()
        {
            try
            {
                var infoList =
  JsonConvert.DeserializeObject<Dictionary<String, Object>>(HttpUtility.UrlDecode(Request.Form.ToString()));
                var minPriceID = 0;//最低价审批id
                int.TryParse(infoList["id"].ToString(), out minPriceID);
                var userInfo = App_Code.Commen.GetUserFromSession();
                var userIDList = db.FileMinPriceChild
                    .Where(w => w.FileMinPriceID == minPriceID && w.ApproveLevel == "二级" && w.ApproveState == "待审批")
                    .Select(s => s.ApprovePersonID)
                    .ToList();
                if (userIDList.Contains(userInfo.UserID))
                {
                    return "ok";
                }
                else
                {
                    return "error";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 重新上传技术规格书文件
        /// </summary>
        /// <param name="fileMinPriceReLoad"></param>
        /// <returns></returns>
        [HttpPost]
        public string MinPriceReLoad(HttpPostedFileBase fileMinPriceReLoad)
        {
            try
            {
                var minPriceID = 0;
                int.TryParse(Request["tbxMinPriceIDReLoad"], out minPriceID);
                var minPriceInfo = db.FileMinPrice.Find(minPriceID);
                var userInfo = App_Code.Commen.GetUserFromSession();
                var newName = "";
                var showName = "";
                //文件上传
                if (fileMinPriceReLoad != null)
                {
                    var fileExt = Path.GetExtension(fileMinPriceReLoad.FileName).ToLower();
                    var fileName = Path.GetFileNameWithoutExtension(fileMinPriceReLoad.FileName).ToLower();
                    newName = fileName + Guid.NewGuid() + fileExt;
                    var filePath = Request.MapPath("~/FileUpload");
                    var fullName = Path.Combine(filePath, newName);
                    fileMinPriceReLoad.SaveAs(fullName);
                    minPriceInfo.TechnicSpecificationFile = newName;
                    minPriceInfo.TechnicSpecificationFileShow = fileName + fileExt;
                    showName = fileName + fileExt;
                }
                //如果是一级审批回退，将所有【一级】审批人员的审批状态变为【待审核】
                if (minPriceInfo.ApproveState == "一级审批回退")
                {
                    var firstList = db.FileMinPriceChild.Where(w => w.FileMinPriceID == minPriceID && w.ApproveLevel == "一级").ToList();
                    foreach (var item in firstList)
                    {
                        item.ApproveState = "待审批";
                        item.ApproveBackReason = null;
                        item.ApproveDateTime = null;
                    }
                }
                //如果是二级审批回退，将所有【一级】和【二级】审批人员的审批状态变为【待审核】
                if (minPriceInfo.ApproveState == "二级审批回退")
                {
                    var firstList = db.FileMinPriceChild.Where(w => w.FileMinPriceID == minPriceID && w.ApproveLevel == "一级").ToList();
                    foreach (var item in firstList)
                    {
                        item.ApproveState = "待审批";
                        item.ApproveBackReason = null;
                        item.ApproveDateTime = null;
                    }
                    var secondList = db.FileMinPriceChild.Where(w => w.FileMinPriceID == minPriceID && w.ApproveLevel == "二级").ToList();
                    foreach (var item in firstList)
                    {
                        item.ApproveState = "待审批";
                        item.ApproveBackReason = null;
                        item.ApproveDateTime = null;
                    }
                }
                minPriceInfo.ApproveState = "待审核";

                #region 写入日志，重新上传技术规格书文件
                var log = new Models.Log();
                log.LogType = "最低价法审批";
                log.LogDataID = minPriceID;
                log.InputDateTime = DateTime.Now;
                log.InputPersonID = userInfo.UserID;
                log.InputPersonName = userInfo.UserName;
                log.LogContent = "重新上传技术规格书文件";
                log.Col1 = newName;//记录上传存储的文件名
                log.Col2 = showName;//记录上传显示的文件名
                db.Log.Add(log);
                #endregion
                db.SaveChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 获取最低价上传、审批记录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetMinPriceLog()
        {
            try
            {
                var id = 0;
                int.TryParse(Request.Form["id"], out id);
                var list = db.Log.Where(w => w.LogType == "最低价法审批" & w.LogDataID == id).ToList();
                return Json(list);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
        #endregion

        #region Comprehensive--综合评标法审批
        /// <summary>
        /// 新建综合评标法审批
        /// </summary>
        /// <param name="fileTechnicalSpecification"></param>
        /// <param name="fileComprehensiveTechnical"></param>
        /// <param name="fileComprehensiveBusiness"></param>
        /// <returns></returns>
        [HttpPost]
        public string InsertComprehensiveFile(HttpPostedFileBase fileTechnicalSpecification, HttpPostedFileBase fileComprehensiveTechnical, HttpPostedFileBase fileComprehensiveBusiness)
        {
            try
            {
                #region 获取一、二级审批人员ID列表--技术规格书审批人员

                var firstListOne = Request.Form["tbxFirstListOne"].ToString().Substring(0, Request.Form["tbxFirstListOne"].ToString().Length - 1);
                var secondListOne = string.Empty;
                if (Request.Form["tbxSecondListOne"] != string.Empty)
                {
                    secondListOne = Request.Form["tbxSecondListOne"].ToString().Substring(0, Request.Form["tbxSecondListOne"].ToString().Length - 1);
                }

                var firstApprovePersonIDStrOne = firstListOne.Split('&');
                var secondApprovePersonIDStrOne = secondListOne == "" ? null : secondListOne.Split('&');

                List<int> firstApprovePersonIDOne = new List<int>();
                List<int> secondApprovePersonIDOne = new List<int>();

                for (int i = 0; i < firstApprovePersonIDStrOne.Length; i++)
                {
                    firstApprovePersonIDOne.Add(Convert.ToInt32(firstApprovePersonIDStrOne[i]));
                }

                if (secondApprovePersonIDStrOne != null)
                {
                    for (int i = 0; i < secondApprovePersonIDStrOne.Length; i++)
                    {
                        secondApprovePersonIDOne.Add(Convert.ToInt32(secondApprovePersonIDStrOne[i]));
                    }
                }
                #endregion
                #region 获取一、二级审批人员ID列表--评分标准（技术）审批人员

                var firstListTwo = Request.Form["tbxFirstListTwo"].ToString().Substring(0, Request.Form["tbxFirstListTwo"].ToString().Length - 1);
                var secondListTwo = string.Empty;
                if (Request.Form["tbxSecondListTwo"] != string.Empty)
                {
                    secondListTwo = Request.Form["tbxSecondListTwo"].ToString().Substring(0, Request.Form["tbxSecondListTwo"].ToString().Length - 1);
                }

                var firstApprovePersonIDStrTwo = firstListTwo.Split('&');
                var secondApprovePersonIDStrTwo = secondListTwo == "" ? null : secondListTwo.Split('&');

                List<int> firstApprovePersonIDTwo = new List<int>();
                List<int> secondApprovePersonIDTwo = new List<int>();

                for (int i = 0; i < firstApprovePersonIDStrTwo.Length; i++)
                {
                    firstApprovePersonIDTwo.Add(Convert.ToInt32(firstApprovePersonIDStrTwo[i]));
                }

                if (secondApprovePersonIDStrTwo != null)
                {
                    for (int i = 0; i < secondApprovePersonIDStrTwo.Length; i++)
                    {
                        secondApprovePersonIDTwo.Add(Convert.ToInt32(secondApprovePersonIDStrTwo[i]));
                    }
                }
                #endregion
                #region 获取一、二级审批人员ID列表--评分标准（商务）审批人员

                var firstListThird = Request.Form["tbxFirstListThird"].ToString().Substring(0, Request.Form["tbxFirstListThird"].ToString().Length - 1);
                var secondListThird = string.Empty;
                if (Request.Form["tbxSecondListThird"] != string.Empty)
                {
                    secondListThird = Request.Form["tbxSecondListThird"].ToString().Substring(0, Request.Form["tbxSecondListThird"].ToString().Length - 1);
                }

                var firstApprovePersonIDStrThird = firstListThird.Split('&');
                var secondApprovePersonIDStrThird = secondListThird == "" ? null : secondListThird.Split('&');

                List<int> firstApprovePersonIDThird = new List<int>();
                List<int> secondApprovePersonIDThird = new List<int>();

                for (int i = 0; i < firstApprovePersonIDStrThird.Length; i++)
                {
                    firstApprovePersonIDThird.Add(Convert.ToInt32(firstApprovePersonIDStrThird[i]));
                }

                if (secondApprovePersonIDStrThird != null)
                {
                    for (int i = 0; i < secondApprovePersonIDStrThird.Length; i++)
                    {
                        secondApprovePersonIDThird.Add(Convert.ToInt32(secondApprovePersonIDStrThird[i]));
                    }
                }
                #endregion

                var userInfo = App_Code.Commen.GetUserFromSession();
                var info = new Models.FileComprehensive();

                //技术规格书文件上传
                var newNameSpecification = string.Empty;
                var showNameSpecification = string.Empty;
                if (fileTechnicalSpecification != null)
                {
                    var fileExt = Path.GetExtension(fileTechnicalSpecification.FileName).ToLower();
                    var fileName = Path.GetFileNameWithoutExtension(fileTechnicalSpecification.FileName).ToLower();
                    newNameSpecification = fileName + Guid.NewGuid() + fileExt;
                    var filePath = Request.MapPath("~/FileUpload");
                    var fullName = Path.Combine(filePath, newNameSpecification);
                    fileTechnicalSpecification.SaveAs(fullName);
                    info.TechnicSpecificationFile = newNameSpecification;
                    info.TechnicSpecificationFileShow = fileName + fileExt;
                    showNameSpecification = fileName + fileExt;
                }
                //评分标准--技术上传
                var newNameTechnical = string.Empty;
                var showNameTechnical = string.Empty;
                if (fileComprehensiveTechnical != null)
                {
                    var fileExt = Path.GetExtension(fileComprehensiveTechnical.FileName).ToLower();
                    var fileName = Path.GetFileNameWithoutExtension(fileComprehensiveTechnical.FileName).ToLower();
                    newNameTechnical = fileName + Guid.NewGuid() + fileExt;
                    var filePath = Request.MapPath("~/FileUpload");
                    var fullName = Path.Combine(filePath, newNameTechnical);
                    fileComprehensiveTechnical.SaveAs(fullName);
                    info.TechnologyScoreStandardFile = newNameTechnical;
                    info.TechnologyScoreStandardFileShow = fileName + fileExt;
                    showNameTechnical = fileName + fileExt;
                }
                //评分标准--商务上传
                var newNameBusiness = string.Empty;
                var showNameBusiness = string.Empty;
                if (fileComprehensiveTechnical != null)
                {
                    var fileExt = Path.GetExtension(fileComprehensiveBusiness.FileName).ToLower();
                    var fileName = Path.GetFileNameWithoutExtension(fileComprehensiveBusiness.FileName).ToLower();
                    newNameBusiness = fileName + Guid.NewGuid() + fileExt;
                    var filePath = Request.MapPath("~/FileUpload");
                    var fullName = Path.Combine(filePath, newNameBusiness);
                    fileComprehensiveBusiness.SaveAs(fullName);
                    info.BusinessScoreStandardFile = newNameBusiness;
                    info.BusinessScoreStandardFileShow = fileName + fileExt;
                    showNameBusiness = fileName + fileExt;
                }

                #region 提交人员信息
                info.InputDateTime = DateTime.Now;
                info.InputPersonID = userInfo.UserID;
                info.InputPersonName = userInfo.UserName;
                var userDeptInfo = db.DeptInfo.Where(w => w.DeptID == userInfo.UserDeptID).FirstOrDefault();
                info.InputPersonDeptID = userDeptInfo.DeptID;
                info.InputPersonDeptName = userDeptInfo.DeptName;
                var userFatherDeptInfo = db.DeptInfo.Where(w => w.DeptID == userDeptInfo.DeptFatherID).FirstOrDefault();
                info.InputPersonFatherDeptID = userDeptInfo.DeptFatherID;
                info.InputPersonFatherDeptName = userFatherDeptInfo.DeptName;
                #endregion 

                //分别判断，技术规格书、评分标准（技术）、评分标准（商务）审批级别
                info.ApproveStateSpecification = "待审核";
                info.ApproveLevelSpecification = secondApprovePersonIDStrOne == null ? "一级" : "二级";

                info.ApproveStateTechnology = "待审核";
                info.ApproveLevelTechnology = secondApprovePersonIDStrTwo == null ? "一级" : "二级";

                info.ApproveStateBusiness = "待审核";
                info.ApproveLevelBusiness = secondApprovePersonIDStrThird == null ? "一级" : "二级";

                info.ProjectName = Request.Form["tbxProjectName"].ToString();

                //添加综合评标法审批记录，获取自增ID
                db.FileComprehensive.Add(info);
                db.SaveChanges();

                //将技术规格书、评分标准（技术）、评分标准（商务）审批人员写入审批流程信息表
                List<Models.FileComprehensiveChild> list = new List<Models.FileComprehensiveChild>();
                foreach (var item in firstApprovePersonIDOne)
                {
                    var child = new Models.FileComprehensiveChild();

                    child.FileComprehensiveID = info.FileComprehensiveID;
                    child.ApproveType = "技术规格书";
                    child.ApproveLevel = "一级";
                    child.ApproveState = "待审批";
                    child.ApprovePersonID = item;
                    var firstUser = db.UserInfo.Where(w => w.UserID == item).FirstOrDefault();
                    child.ApprovePersonName = firstUser.UserName;
                    child.ApprovePersonDeptID = firstUser.UserDeptID;
                    var deptInfo = db.DeptInfo.Where(w => w.DeptID == firstUser.UserDeptID).FirstOrDefault();
                    child.ApprovePersonDeptName = deptInfo.DeptName;
                    var deptFatherInfo = db.DeptInfo.Where(w => w.DeptID == deptInfo.DeptFatherID).FirstOrDefault();
                    child.ApprovePersonFatherDeptName = deptFatherInfo.DeptName;
                    child.ApprovePersonFatherDeptID = deptInfo.DeptFatherID;

                    list.Add(child);
                }
                foreach (var item in secondApprovePersonIDOne)
                {
                    var child = new Models.FileComprehensiveChild();

                    child.FileComprehensiveID = info.FileComprehensiveID;
                    child.ApproveType = "技术规格书";
                    child.ApproveLevel = "二级";
                    child.ApproveState = "待审批";
                    child.ApprovePersonID = item;
                    var firstUser = db.UserInfo.Where(w => w.UserID == item).FirstOrDefault();
                    child.ApprovePersonName = firstUser.UserName;
                    child.ApprovePersonDeptID = firstUser.UserDeptID;
                    var deptInfo = db.DeptInfo.Where(w => w.DeptID == firstUser.UserDeptID).FirstOrDefault();
                    child.ApprovePersonDeptName = deptInfo.DeptName;
                    var deptFatherInfo = db.DeptInfo.Where(w => w.DeptID == deptInfo.DeptFatherID).FirstOrDefault();
                    child.ApprovePersonFatherDeptName = deptFatherInfo.DeptName;
                    child.ApprovePersonFatherDeptID = deptInfo.DeptFatherID;

                    list.Add(child);
                }

                foreach (var item in firstApprovePersonIDTwo)
                {
                    var child = new Models.FileComprehensiveChild();

                    child.FileComprehensiveID = info.FileComprehensiveID;
                    child.ApproveType = "评分标准(技术)";
                    child.ApproveLevel = "一级";
                    child.ApproveState = "待审批";
                    child.ApprovePersonID = item;
                    var firstUser = db.UserInfo.Where(w => w.UserID == item).FirstOrDefault();
                    child.ApprovePersonName = firstUser.UserName;
                    child.ApprovePersonDeptID = firstUser.UserDeptID;
                    var deptInfo = db.DeptInfo.Where(w => w.DeptID == firstUser.UserDeptID).FirstOrDefault();
                    child.ApprovePersonDeptName = deptInfo.DeptName;
                    var deptFatherInfo = db.DeptInfo.Where(w => w.DeptID == deptInfo.DeptFatherID).FirstOrDefault();
                    child.ApprovePersonFatherDeptName = deptFatherInfo.DeptName;
                    child.ApprovePersonFatherDeptID = deptInfo.DeptFatherID;

                    list.Add(child);
                }
                foreach (var item in secondApprovePersonIDTwo)
                {
                    var child = new Models.FileComprehensiveChild();

                    child.FileComprehensiveID = info.FileComprehensiveID;
                    child.ApproveType = "评分标准(技术)";
                    child.ApproveLevel = "二级";
                    child.ApproveState = "待审批";
                    child.ApprovePersonID = item;
                    var firstUser = db.UserInfo.Where(w => w.UserID == item).FirstOrDefault();
                    child.ApprovePersonName = firstUser.UserName;
                    child.ApprovePersonDeptID = firstUser.UserDeptID;
                    var deptInfo = db.DeptInfo.Where(w => w.DeptID == firstUser.UserDeptID).FirstOrDefault();
                    child.ApprovePersonDeptName = deptInfo.DeptName;
                    var deptFatherInfo = db.DeptInfo.Where(w => w.DeptID == deptInfo.DeptFatherID).FirstOrDefault();
                    child.ApprovePersonFatherDeptName = deptFatherInfo.DeptName;
                    child.ApprovePersonFatherDeptID = deptInfo.DeptFatherID;

                    list.Add(child);
                }

                foreach (var item in firstApprovePersonIDThird)
                {
                    var child = new Models.FileComprehensiveChild();

                    child.FileComprehensiveID = info.FileComprehensiveID;
                    child.ApproveType = "评分标准(商务)";
                    child.ApproveLevel = "一级";
                    child.ApproveState = "待审批";
                    child.ApprovePersonID = item;
                    var firstUser = db.UserInfo.Where(w => w.UserID == item).FirstOrDefault();
                    child.ApprovePersonName = firstUser.UserName;
                    child.ApprovePersonDeptID = firstUser.UserDeptID;
                    var deptInfo = db.DeptInfo.Where(w => w.DeptID == firstUser.UserDeptID).FirstOrDefault();
                    child.ApprovePersonDeptName = deptInfo.DeptName;
                    var deptFatherInfo = db.DeptInfo.Where(w => w.DeptID == deptInfo.DeptFatherID).FirstOrDefault();
                    child.ApprovePersonFatherDeptName = deptFatherInfo.DeptName;
                    child.ApprovePersonFatherDeptID = deptInfo.DeptFatherID;

                    list.Add(child);
                }
                foreach (var item in secondApprovePersonIDThird)
                {
                    var child = new Models.FileComprehensiveChild();

                    child.FileComprehensiveID = info.FileComprehensiveID;
                    child.ApproveType = "评分标准(商务)";
                    child.ApproveLevel = "二级";
                    child.ApproveState = "待审批";
                    child.ApprovePersonID = item;
                    var firstUser = db.UserInfo.Where(w => w.UserID == item).FirstOrDefault();
                    child.ApprovePersonName = firstUser.UserName;
                    child.ApprovePersonDeptID = firstUser.UserDeptID;
                    var deptInfo = db.DeptInfo.Where(w => w.DeptID == firstUser.UserDeptID).FirstOrDefault();
                    child.ApprovePersonDeptName = deptInfo.DeptName;
                    var deptFatherInfo = db.DeptInfo.Where(w => w.DeptID == deptInfo.DeptFatherID).FirstOrDefault();
                    child.ApprovePersonFatherDeptName = deptFatherInfo.DeptName;
                    child.ApprovePersonFatherDeptID = deptInfo.DeptFatherID;

                    list.Add(child);
                }

                db.FileComprehensiveChild.AddRange(list);

                #region 写入日志，新建技术规格书文件审批
                var log = new Models.Log();
                log.LogType = "综合评标法审批";
                log.LogDataID = info.FileComprehensiveID;
                log.InputDateTime = DateTime.Now;
                log.InputPersonID = userInfo.UserID;
                log.InputPersonName = userInfo.UserName;
                log.LogContent = "新建综合评标法审批";
                db.Log.Add(log);
                #endregion

                db.SaveChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 获取综合评标法审批列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetComprehensiveList()
        {
            try
            {
                var limit = 0;
                int.TryParse(Request.Form["limit"], out limit);
                var offset = 0;
                int.TryParse(Request.Form["offset"], out offset);
                var projectName = Request.Form["tbxProjectNameSearch"];//项目名称

                var inputPersonFatherDeptID = 0;//提报部门
                int.TryParse(Request.Form["ddlApproveDeptSearch"], out inputPersonFatherDeptID);

                var userInfo = App_Code.Commen.GetUserFromSession();

                var result = from m in db.FileComprehensive
                             select m;

                if (User.IsInRole("技术规格书提报"))
                {
                    result = result.Where(w => w.InputPersonID == userInfo.UserID);
                }
                if (User.IsInRole("技术规格书审批"))
                {
                    var approvePersonIDList = db.FileComprehensiveChild
                        .Where(w => w.ApprovePersonID == userInfo.UserID)
                        .Select(s => s.FileComprehensiveID)
                        .ToList();
                    result = result.Where(w => approvePersonIDList.Contains(w.FileComprehensiveID));
                }
                if (!string.IsNullOrEmpty(projectName))
                {
                    result = result.Where(w => w.ProjectName.Contains(projectName));
                }

                if (inputPersonFatherDeptID != 0)
                {
                    result = result.Where(w => w.InputPersonFatherDeptID == inputPersonFatherDeptID);
                }
                return Json(new { total = result.Count(), rows = result.OrderByDescending(o => o.InputDateTime).Skip(offset).Take(limit).ToList() });
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        /// <summary>
        /// 获取综合评标法上传、审批记录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetComprehensiveLog()
        {
            try
            {
                var id = 0;
                int.TryParse(Request.Form["id"], out id);
                var list = db.Log.Where(w => w.LogType == "综合评标法审批" & w.LogDataID == id).ToList();
                return Json(list);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        /// <summary>
        /// 获取审批人员信息列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetComprehensiveApproveInfo()
        {
            try
            {
                var postList =
   JsonConvert.DeserializeObject<Dictionary<String, Object>>(HttpUtility.UrlDecode(Request.Form.ToString()));

                var approveType = postList["approveType"].ToString();//审批类型
                var approveLevel = postList["approveLevel"].ToString();//审批级别

                var fileComprehensiveID = 0;//综合评标法审批表ID
                int.TryParse(postList["fileComprehensiveID"].ToString(), out fileComprehensiveID);

                var list = db.FileComprehensiveChild
                    .Where(w => w.FileComprehensiveID == fileComprehensiveID & w.ApproveType == approveType & w.ApproveLevel == approveLevel)
                    .ToList();
                return Json(list);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        /// <summary>
        /// 获取综合评标法文件信息列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetComprehensiveFileListByID()
        {
            try
            {
                var id = 0;
                int.TryParse(Request.Form["id"], out id);
                var list = db.FileComprehensive.Where(w => w.FileComprehensiveID == id).ToList();
                return Json(list);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        /// <summary>
        /// 执行技术规格书、评分标准(技术)、评分标准(商务)，判断同意或回退
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string ApproveComprehensiveChild()
        {
            try
            {
                var userInfo = App_Code.Commen.GetUserFromSession();

                var postList =
   JsonConvert.DeserializeObject<Dictionary<String, Object>>(HttpUtility.UrlDecode(Request.Form.ToString()));

                var approveType = postList["approveType"].ToString();//审批类型
                var approveResult = postList["approveResult"].ToString();//审批结果
                var backReason = postList["backReason"].ToString();//回退原因
                var fileComprehensiveID = 0;
                int.TryParse(postList["fileComprehensiveID"].ToString(), out fileComprehensiveID);
                var fileComprehensive = db.FileComprehensive.Find(fileComprehensiveID);
                var fileApproveState = string.Empty;//文件审批状态
                var fileApproveLevel = string.Empty;//文件审批级别
                var col1 = string.Empty;//存储文件名
                var col2 = string.Empty;//显示文件名
                switch (approveType)
                {
                    case "技术规格书":
                        col1 = fileComprehensive.TechnicSpecificationFile;
                        col2 = fileComprehensive.TechnicSpecificationFileShow;
                        fileApproveState = fileComprehensive.ApproveStateSpecification;
                        fileApproveLevel = fileComprehensive.ApproveLevelSpecification;
                        break;
                    case "评分标准(技术)":
                        col1 = fileComprehensive.TechnologyScoreStandardFile;
                        col2 = fileComprehensive.TechnologyScoreStandardFileShow;
                        fileApproveState = fileComprehensive.ApproveStateTechnology;
                        fileApproveLevel = fileComprehensive.ApproveLevelTechnology;
                        break;
                    case "评分标准(商务)":
                        col1 = fileComprehensive.BusinessScoreStandardFile;
                        col2 = fileComprehensive.BusinessScoreStandardFileShow;
                        fileApproveState = fileComprehensive.ApproveStateBusiness;
                        fileApproveLevel = fileComprehensive.ApproveLevelBusiness;
                        break;
                }

                var child = db.FileComprehensiveChild
                    .Where(w => w.FileComprehensiveID == fileComprehensiveID & w.ApproveType == approveType & w.ApprovePersonID == userInfo.UserID)
                    .FirstOrDefault();

                if (fileApproveState == "二级审批回退" || fileApproveState == "一级审批回退")
                {
                    return "审批已回退，不需要继续审批！";
                }
                if (child != null)
                {
                    if (child.ApproveState == "待审批")
                    {
                        //只有一级用户全部审批完成后，二级用户才能执行审批
                        if (fileApproveLevel == "二级")
                        {
                            if (child.ApproveLevel == "二级")
                            {
                                if (fileApproveState != "一级审批完成")
                                {
                                    return "只有一级审批完成后，才能进行二级审批操作！";
                                }
                            }
                        }
                        child.ApproveDateTime = DateTime.Now;
                        if (approveResult == "yes")
                        {
                            child.ApproveState = child.ApproveLevel == "一级" ? "一级审批同意" : "二级审批同意";
                        }
                        else
                        {
                            child.ApproveState = child.ApproveLevel == "一级" ? "一级审批回退" : "二级审批回退";
                            child.ApproveBackReason = backReason;
                            #region 写入日志，审批回退原因
                            var log = new Models.Log();
                            log.LogType = "综合评标法审批";
                            log.LogDataID = log.LogDataID = fileComprehensiveID;
                            log.InputDateTime = DateTime.Now;
                            log.InputPersonID = userInfo.UserID;
                            log.InputPersonName = userInfo.UserName;
                            log.LogContent = child.ApproveLevel == "一级" ? "一级审批回退" : "二级审批回退";
                            log.LogReason = backReason;
                            log.Col2 = col2;
                            log.Col1 = col1;
                            log.Col3 = approveType;//审批文件的类型，规格书、评分标准(技术)、评分标准(商务)
                            db.Log.Add(log);
                            #endregion
                        }
                        db.SaveChanges();

                        #region 判断文件的审批状态，写入审批信息表
                        var okSumFirst = 0;//一级审批数量
                        var childListFirst = db.FileComprehensiveChild
                            .Where(w => w.ApproveType == approveType & w.ApproveLevel == "一级" & w.FileComprehensiveID == fileComprehensiveID)
                            .ToList();
                        foreach (var item in childListFirst)
                        {
                            if (item.ApproveState == "一级审批回退")
                            {
                                switch (approveType)
                                {
                                    case "技术规格书":
                                        fileComprehensive.ApproveStateSpecification = "一级审批回退";
                                        break;
                                    case "评分标准(技术)":
                                        fileComprehensive.ApproveStateTechnology = "一级审批回退";
                                        break;
                                    case "评分标准(商务)":
                                        fileComprehensive.ApproveStateBusiness = "一级审批回退";
                                        break;
                                }
                            }
                            if (item.ApproveState == "一级审批同意")
                            {
                                okSumFirst += 1;
                            }
                        }
                        //如果全部审批通过，则设置相应的文件状态为“一级审批完成”
                        if (okSumFirst == childListFirst.Count)
                        {
                            switch (approveType)
                            {
                                case "技术规格书":
                                    fileComprehensive.ApproveStateSpecification = "一级审批完成";
                                    break;
                                case "评分标准(技术)":
                                    fileComprehensive.ApproveStateTechnology = "一级审批完成";
                                    break;
                                case "评分标准(商务)":
                                    fileComprehensive.ApproveStateBusiness = "一级审批完成";
                                    break;
                            }
                        }

                        var okSumSecond = 0;//二级审批数量
                        var childListSecond = db.FileComprehensiveChild
                            .Where(w => w.ApproveType == approveType & w.ApproveLevel == "二级" & w.FileComprehensiveID == fileComprehensiveID)
                            .ToList();
                        //如果存在二级审批的时候，执行判断审批状态
                        if (childListSecond.Count != 0)
                        {
                            foreach (var item in childListSecond)
                            {
                                if (item.ApproveState == "二级审批回退")
                                {
                                    switch (approveType)
                                    {
                                        case "技术规格书":
                                            fileComprehensive.ApproveStateSpecification = "二级审批回退";
                                            break;
                                        case "评分标准(技术)":
                                            fileComprehensive.ApproveStateTechnology = "二级审批回退";
                                            break;
                                        case "评分标准(商务)":
                                            fileComprehensive.ApproveStateBusiness = "二级审批回退";
                                            break;
                                    }
                                }
                                if (item.ApproveState == "二级审批同意")
                                {
                                    okSumSecond += 1;
                                }
                            }
                            //如果全部审批通过，则设置相应的文件状态为“二级审批完成”
                            if (okSumSecond == childListSecond.Count)
                            {
                                switch (approveType)
                                {
                                    case "技术规格书":
                                        fileComprehensive.ApproveStateSpecification = "二级审批完成";
                                        break;
                                    case "评分标准(技术)":
                                        fileComprehensive.ApproveStateTechnology = "二级审批完成";
                                        break;
                                    case "评分标准(商务)":
                                        fileComprehensive.ApproveStateBusiness = "二级审批完成";
                                        break;
                                }
                            }
                        }
                        db.SaveChanges();
                        #endregion

                        #region 判断3种文件的状态，如果全部审批完成，则执行文件合并操作
                        var fileSuccessCount = 0;//文件审批成功数量
                        //判断技术规格书审批情况
                        if (fileComprehensive.ApproveLevelSpecification == "二级")
                        {
                            if (fileComprehensive.ApproveStateSpecification == "二级审批完成")
                            {
                                fileSuccessCount += 1;
                            }
                        }
                        else
                        {
                            if (fileComprehensive.ApproveStateSpecification == "一级审批完成")
                            {
                                fileSuccessCount += 1;
                            }
                        }
                        //判断评分标准(技术)审批情况
                        if (fileComprehensive.ApproveLevelTechnology == "二级")
                        {
                            if (fileComprehensive.ApproveStateTechnology == "二级审批完成")
                            {
                                fileSuccessCount += 1;
                            }
                        }
                        else
                        {
                            if (fileComprehensive.ApproveStateTechnology == "一级审批完成")
                            {
                                fileSuccessCount += 1;
                            }
                        }
                        //判断评分标准(商务)审批情况
                        if (fileComprehensive.ApproveLevelBusiness == "二级")
                        {
                            if (fileComprehensive.ApproveStateBusiness == "二级审批完成")
                            {
                                fileSuccessCount += 1;
                            }
                        }
                        else
                        {
                            if (fileComprehensive.ApproveStateBusiness == "一级审批完成")
                            {
                                fileSuccessCount += 1;
                            }
                        }
                        if (fileSuccessCount == 3)
                        {
                            //执行文件合并操作
                            var filePath = Request.MapPath("~/FileUpload");
                            var filePathTemplate = Request.MapPath("~/Template");
                            List<string> fileList = new List<string>();
                            fileList.Add(Path.Combine(filePath, fileComprehensive.TechnicSpecificationFile));
                            fileList.Add(Path.Combine(filePath, fileComprehensive.TechnologyScoreStandardFile));
                            fileList.Add(Path.Combine(filePath, fileComprehensive.BusinessScoreStandardFile));
                            #region word合并
                            //var outFile = Path.Combine(filePath, "综合评标法--"+fileComprehensive.ProjectName + Guid.NewGuid() + ".doc");
                            //var wordClass = new App_Code.WordClass();
                            //wordClass.InsertMerge(Path.Combine(filePathTemplate, "ComprehensiveFile.doc"), fileList, outFile);
                            #endregion
                            var fileName = "综合评标法--" + fileComprehensive.ProjectName + Guid.NewGuid() + ".pdf";
                            var outFile = Path.Combine(filePath,fileName );

                            //App_Code.Commen.MergePdfFilesBySpire(fileList, outFile);
                            App_Code.Commen.MergePdfFilesByiTextSharp(fileList, outFile);

                            fileComprehensive.TechnicSpecificationFileMerge = fileName;
                            fileComprehensive.TechnicSpecificationFileMergeShow = "综合评标法--" + fileComprehensive.ProjectName + ".pdf";
                            fileComprehensive.ApproveSuccessState = "审批完成";
                            db.SaveChanges();
                        }
                        #endregion
                        return "ok";
                    }
                    else
                    {
                        return "不能重复审批！";
                    }
                }
                else
                {
                    return "不是审批用户！";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 重新上传综合评标法文件
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string ReLoadFileComprehensive(HttpPostedFileBase fileOneReLoad, HttpPostedFileBase fileTwoReLoad, HttpPostedFileBase fileThirdReLoad)
        {
            try
            {
                var userInfo = App_Code.Commen.GetUserFromSession();
                var fileType = string.Empty;
                var idString = string.Empty;
                if (Request.Form["tbxReLoadOneID"] != null)
                {
                    fileType = Request.Form["tbxReLoadOneType"];
                    idString = Request.Form["tbxReLoadOneID"];
                }
                if (Request.Form["tbxReLoadTwoID"] != null)
                {
                    fileType = Request.Form["tbxReLoadTwoType"];
                    idString = Request.Form["tbxReLoadTwoID"];
                }
                if (Request.Form["tbxReLoadThirdID"] != null)
                {
                    fileType = Request.Form["tbxReLoadThirdType"];
                    idString = Request.Form["tbxReLoadThirdID"];
                }
                var comprehensiveID = 0;
                int.TryParse(idString, out comprehensiveID);

                var comprehensiveInfo = db.FileComprehensive.Find(comprehensiveID);

                switch (fileType)
                {
                    case "技术规格书":
                        var newNameOne = "";
                        var showNameOne = "";
                        //文件上传
                        if (fileOneReLoad != null)
                        {
                            var fileExt = Path.GetExtension(fileOneReLoad.FileName).ToLower();
                            var fileName = Path.GetFileNameWithoutExtension(fileOneReLoad.FileName).ToLower();
                            newNameOne = fileName + Guid.NewGuid() + fileExt;
                            var filePath = Request.MapPath("~/FileUpload");
                            var fullName = Path.Combine(filePath, newNameOne);
                            fileOneReLoad.SaveAs(fullName);
                            comprehensiveInfo.TechnicSpecificationFile = newNameOne;
                            comprehensiveInfo.TechnicSpecificationFileShow = fileName + fileExt;
                            showNameOne = fileName + fileExt;
                        }

                        //如果是一级审批回退，将所有【一级】审批人员的审批状态变为【待审核】
                        if (comprehensiveInfo.ApproveStateSpecification == "一级审批回退")
                        {
                            var firstList = db.FileComprehensiveChild
                                .Where(w => w.FileComprehensiveID == comprehensiveID & w.ApproveType == "技术规格书" & w.ApproveLevel == "一级")
                                .ToList();
                            foreach (var item in firstList)
                            {
                                item.ApproveState = "待审批";
                                item.ApproveBackReason = null;
                                item.ApproveDateTime = null;
                            }
                        }

                        //如果是二级审批回退，将所有【一级】和【二级】审批人员的审批状态变为【待审核】
                        if (comprehensiveInfo.ApproveStateSpecification == "二级审批回退")
                        {
                            var firstList = db.FileComprehensiveChild
                                .Where(w => w.FileComprehensiveID == comprehensiveID & w.ApproveType == "技术规格书" & w.ApproveLevel == "一级")
                                .ToList();
                            foreach (var item in firstList)
                            {
                                item.ApproveState = "待审批";
                                item.ApproveBackReason = null;
                                item.ApproveDateTime = null;
                            }
                            var secondList = db.FileComprehensiveChild
                                .Where(w => w.FileComprehensiveID == comprehensiveID & w.ApproveType == "技术规格书" & w.ApproveLevel == "二级")
                                .ToList();
                            foreach (var item in secondList)
                            {
                                item.ApproveState = "待审批";
                                item.ApproveBackReason = null;
                                item.ApproveDateTime = null;
                            }
                        }

                        //将技术规格书审批状态变为“待审核”
                        comprehensiveInfo.ApproveStateSpecification = "待审核";

                        #region 写入日志，重新上传技术规格书文件
                        var logOne = new Models.Log();
                        logOne.LogType = "综合评标法审批";
                        logOne.LogDataID = comprehensiveID;
                        logOne.InputDateTime = DateTime.Now;
                        logOne.InputPersonID = userInfo.UserID;
                        logOne.InputPersonName = userInfo.UserName;
                        logOne.LogContent = "重新上传技术规格书文件";
                        logOne.Col1 = newNameOne;//记录上传存储的文件名
                        logOne.Col2 = showNameOne;//记录上传显示的文件名
                        logOne.Col3 = "技术规格书";//审批文件类型
                        db.Log.Add(logOne);
                        #endregion
                        break;
                    case "评分标准(技术)":
                        var newNameTwo = "";
                        var showNameTwo = "";
                        //文件上传
                        if (fileTwoReLoad != null)
                        {
                            var fileExt = Path.GetExtension(fileTwoReLoad.FileName).ToLower();
                            var fileName = Path.GetFileNameWithoutExtension(fileTwoReLoad.FileName).ToLower();
                            newNameTwo = fileName + Guid.NewGuid() + fileExt;
                            var filePath = Request.MapPath("~/FileUpload");
                            var fullName = Path.Combine(filePath, newNameTwo);
                            fileTwoReLoad.SaveAs(fullName);
                            comprehensiveInfo.TechnologyScoreStandardFile = newNameTwo;
                            comprehensiveInfo.TechnologyScoreStandardFileShow = fileName + fileExt;
                            showNameTwo = fileName + fileExt;
                        }

                        //如果是一级审批回退，将所有【一级】审批人员的审批状态变为【待审核】
                        if (comprehensiveInfo.ApproveStateTechnology == "一级审批回退")
                        {
                            var firstList = db.FileComprehensiveChild
                                .Where(w => w.FileComprehensiveID == comprehensiveID & w.ApproveType == "评分标准(技术)" & w.ApproveLevel == "一级")
                                .ToList();
                            foreach (var item in firstList)
                            {
                                item.ApproveState = "待审批";
                                item.ApproveBackReason = null;
                                item.ApproveDateTime = null;
                            }
                        }

                        //如果是二级审批回退，将所有【一级】和【二级】审批人员的审批状态变为【待审核】
                        if (comprehensiveInfo.ApproveStateTechnology == "二级审批回退")
                        {
                            var firstList = db.FileComprehensiveChild
                                .Where(w => w.FileComprehensiveID == comprehensiveID & w.ApproveType == "评分标准(技术)" & w.ApproveLevel == "一级")
                                .ToList();
                            foreach (var item in firstList)
                            {
                                item.ApproveState = "待审批";
                                item.ApproveBackReason = null;
                                item.ApproveDateTime = null;
                            }
                            var secondList = db.FileComprehensiveChild
                                .Where(w => w.FileComprehensiveID == comprehensiveID & w.ApproveType == "评分标准(技术)" & w.ApproveLevel == "二级")
                                .ToList();
                            foreach (var item in secondList)
                            {
                                item.ApproveState = "待审批";
                                item.ApproveBackReason = null;
                                item.ApproveDateTime = null;
                            }
                        }

                        //将技术规格书审批状态变为“待审核”
                        comprehensiveInfo.ApproveStateTechnology = "待审核";

                        #region 写入日志，重新上传评分标准(技术)文件
                        var logTwo = new Models.Log();
                        logTwo.LogType = "综合评标法审批";
                        logTwo.LogDataID = comprehensiveID;
                        logTwo.InputDateTime = DateTime.Now;
                        logTwo.InputPersonID = userInfo.UserID;
                        logTwo.InputPersonName = userInfo.UserName;
                        logTwo.LogContent = "重新上传评分标准(技术)文件";
                        logTwo.Col1 = newNameTwo;//记录上传存储的文件名
                        logTwo.Col2 = showNameTwo;//记录上传显示的文件名
                        logTwo.Col3 = "评分标准(技术)";//审批文件类型
                        db.Log.Add(logTwo);
                        #endregion
                        break;
                    case "评分标准(商务)":
                        var newNameThird = "";
                        var showNameThird = "";
                        //文件上传
                        if (fileThirdReLoad != null)
                        {
                            var fileExt = Path.GetExtension(fileThirdReLoad.FileName).ToLower();
                            var fileName = Path.GetFileNameWithoutExtension(fileThirdReLoad.FileName).ToLower();
                            newNameThird = fileName + Guid.NewGuid() + fileExt;
                            var filePath = Request.MapPath("~/FileUpload");
                            var fullName = Path.Combine(filePath, newNameThird);
                            fileThirdReLoad.SaveAs(fullName);
                            comprehensiveInfo.BusinessScoreStandardFile = newNameThird;
                            comprehensiveInfo.BusinessScoreStandardFileShow = fileName + fileExt;
                            showNameThird = fileName + fileExt;
                        }

                        //如果是一级审批回退，将所有【一级】审批人员的审批状态变为【待审核】
                        if (comprehensiveInfo.ApproveStateBusiness == "一级审批回退")
                        {
                            var firstList = db.FileComprehensiveChild
                                .Where(w => w.FileComprehensiveID == comprehensiveID & w.ApproveType == "评分标准(商务)" & w.ApproveLevel == "一级")
                                .ToList();
                            foreach (var item in firstList)
                            {
                                item.ApproveState = "待审批";
                                item.ApproveBackReason = null;
                                item.ApproveDateTime = null;
                            }
                        }

                        //如果是二级审批回退，将所有【一级】和【二级】审批人员的审批状态变为【待审核】
                        if (comprehensiveInfo.ApproveStateBusiness == "二级审批回退")
                        {
                            var firstList = db.FileComprehensiveChild
                                .Where(w => w.FileComprehensiveID == comprehensiveID & w.ApproveType == "评分标准(商务)" & w.ApproveLevel == "一级")
                                .ToList();
                            foreach (var item in firstList)
                            {
                                item.ApproveState = "待审批";
                                item.ApproveBackReason = null;
                                item.ApproveDateTime = null;
                            }
                            var secondList = db.FileComprehensiveChild
                                .Where(w => w.FileComprehensiveID == comprehensiveID & w.ApproveType == "评分标准(商务)" & w.ApproveLevel == "二级")
                                .ToList();
                            foreach (var item in secondList)
                            {
                                item.ApproveState = "待审批";
                                item.ApproveBackReason = null;
                                item.ApproveDateTime = null;
                            }
                        }

                        //将技术规格书审批状态变为“待审核”
                        comprehensiveInfo.ApproveStateTechnology = "待审核";

                        #region 写入日志，重新上传评分标准(技术)文件
                        var logThird = new Models.Log();
                        logThird.LogType = "综合评标法审批";
                        logThird.LogDataID = comprehensiveID;
                        logThird.InputDateTime = DateTime.Now;
                        logThird.InputPersonID = userInfo.UserID;
                        logThird.InputPersonName = userInfo.UserName;
                        logThird.LogContent = "重新上传评分标准(商务)文件";
                        logThird.Col1 = newNameThird;//记录上传存储的文件名
                        logThird.Col2 = showNameThird;//记录上传显示的文件名
                        logThird.Col3 = "评分标准(商务)";//审批文件类型
                        db.Log.Add(logThird);
                        #endregion
                        break;
                }
                db.SaveChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion
    }
}