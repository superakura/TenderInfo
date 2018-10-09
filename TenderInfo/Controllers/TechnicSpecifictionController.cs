﻿using Newtonsoft.Json;
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
            return View();
        }

        /// <summary>
        /// 技术规格书审批--综合评标法视图
        /// </summary>
        /// <returns></returns>
        public ViewResult Comprehensive()
        {
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
        #endregion

        #region MinPrice

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
                    showName= fileName + fileExt;
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
                    var deptFatherInfo=db.DeptInfo.Where(w => w.DeptID == deptInfo.DeptFatherID).FirstOrDefault();
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
                    result = result.Where(w=>w.InputPersonID==userInfo.UserID);
                }
                if (User.IsInRole("技术规格书审批"))
                {
                    var approvePersonIDList = db.FileMinPriceChild
                        .Where(w => w.ApprovePersonID == userInfo.UserID)
                        .Select(s=>s.FileMinPriceID)
                        .ToList();
                    result = result.Where(w => approvePersonIDList.Contains(w.FileMinPriceID));
                }
                if (!string.IsNullOrEmpty(fileName))
                {
                    result = result.Where(w => w.TechnicSpecificationFileShow.Contains(fileName));
                }
                if (!string.IsNullOrEmpty(approveLevel))
                {
                    result = result.Where(w => w.ApproveLevel==approveLevel);
                }
                if (!string.IsNullOrEmpty(approveState))
                {
                    result = result.Where(w => w.ApproveState==approveState);
                }
                if (inputPersonFatherDeptID!=0)
                {
                    result = result.Where(w => w.InputPersonFatherDeptID==inputPersonFatherDeptID);
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
                if (fileMinPriceChild.ApproveState!= "待审批")
                {
                    return "不能重复进行审批！";
                }

                fileMinPriceChild.ApproveDateTime = DateTime.Now;
                if (approveType=="back")
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
                    if (item.ApproveState== "一级审批回退")
                    {
                        fileMinPriceInfo.ApproveState = "一级审批回退";
                    }
                    if (item.ApproveState == "一级审批同意")
                    {
                        okSum += 1;
                    }
                }

                if (okSum== fileMinPriceChildFirstList.Count)
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
                if (fileMinPriceChild.ApproveState!= "待审批")
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

                #region 判断一级审批全部的完成状态
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
                    .Where(w => w.FileMinPriceID == minPriceID&&w.ApproveLevel=="一级"&&w.ApproveState== "待审批")
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
                    .Where(w => w.FileMinPriceID == minPriceID && w.ApproveLevel == "二级"&&w.ApproveState== "待审批")
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
                if (minPriceInfo.ApproveState== "一级审批回退")
                {
                    var firstList = db.FileMinPriceChild.Where(w => w.FileMinPriceID == minPriceID).ToList();
                    foreach (var item in firstList)
                    {
                        item.ApproveState = "待审批";
                        item.ApproveBackReason = null;
                        item.ApproveDateTime = null;
                    }
                }
                //如果是一级审批回退，将所有【一级】和【二级】审批人员的审批状态变为【待审核】
                if (minPriceInfo.ApproveState == "二级审批回退")
                {
                    var firstList = db.FileMinPriceChild.Where(w => w.FileMinPriceID == minPriceID&&w.ApproveLevel== "一级").ToList();
                    foreach (var item in firstList)
                    {
                        item.ApproveState = "待审批";
                        item.ApproveBackReason = null;
                        item.ApproveDateTime = null;
                    }
                    var secondList= db.FileMinPriceChild.Where(w => w.FileMinPriceID == minPriceID && w.ApproveLevel == "二级").ToList();
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
    }
}