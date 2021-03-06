﻿using Aspose.Cells;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.SqlServer;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TenderInfo.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        private Models.DB db = new Models.DB();

        #region 视图
        public ViewResult Index()
        {
            return View();
        }

        public ViewResult TableInfo()
        {
            return View();
        }

        public ViewResult Material()
        {
            return View();
        }

        public ViewResult Frame()
        {
            return View();
        }

        public ViewResult Project()
        {
            return View();
        }

        public ViewResult Service()
        {
            return View();
        }

        public ViewResult EditAccountMaterial(string id, string type)
        {
            ViewBag.id = id;
            ViewBag.type = type;

            var accountID = 0;
            int.TryParse(id, out accountID);
            var userInfo = App_Code.Commen.GetUserFromSession();
            var accountInfo = db.Account.Find(accountID);
            if (userInfo.UserID == accountInfo.ProjectResponsiblePersonID)
            {
                ViewBag.editRole = "yes";
            }
            else
            {
                ViewBag.editRole = "no";
            }
            ViewBag.IsComplete = accountInfo.IsComplete;
            return View();
        }

        public ViewResult EditAccountFrame(string id, string type)
        {
            ViewBag.id = id;
            ViewBag.type = type;

            var accountID = 0;
            int.TryParse(id, out accountID);
            var userInfo = App_Code.Commen.GetUserFromSession();
            var accountInfo = db.Account.Find(accountID);
            if (userInfo.UserID == accountInfo.ProjectResponsiblePersonID)
            {
                ViewBag.editRole = "yes";
            }
            else
            {
                ViewBag.editRole = "no";
            }

            return View();
        }

        public ViewResult EditAccountProject(string id, string type)
        {
            ViewBag.id = id;
            ViewBag.type = type;

            var accountID = 0;
            int.TryParse(id, out accountID);
            var userInfo = App_Code.Commen.GetUserFromSession();
            var accountInfo = db.Account.Find(accountID);
            if (userInfo.UserID == accountInfo.ProjectResponsiblePersonID)
            {
                ViewBag.editRole = "yes";
            }
            else
            {
                ViewBag.editRole = "no";
            }

            return View();
        }
        #endregion

        #region 获取二级单位、与招标进度数据同步
        /// <summary>
        /// 获取二级单位，为使用单位、项目主责部门、评标委员会单位提供数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetDeptForSelect()
        {
            try
            {
                var info = db.DeptInfo.Where(w => w.DeptFatherID == 1).OrderBy(o => o.DeptOrder).ToList();
                return Json(info);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        /// <summary>
        /// 获取台账信息，供进度同步信息时，选择要同步到哪个台账
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetAccountListForSelect()
        {
            try
            {
                var accountType = Request.Form["accountType"];
                var userInfo = App_Code.Commen.GetUserFromSession();
                var result = from m in db.Account
                             where m.ProjectResponsiblePersonID == userInfo.UserID && m.IsSynchro != "是"
                             orderby m.AccountID
                             select new
                             {
                                 m.AccountID,
                                 m.ProjectType,
                                 m.ProjectName,
                                 m.TenderFileNum,
                                 m.IsOnline,
                                 m.ProjectResponsiblePersonName,
                                 m.UsingDeptName,
                                 m.ProjectResponsibleDeptName,
                                 m.ApplyPerson
                             };
                switch (accountType)
                {
                    case "工程":
                        result = result.Where(w => w.ProjectType == "工程" || w.ProjectType == "服务");
                        break;
                    case "物资":
                        result = result.Where(w => w.ProjectType == "物资");
                        break;
                    case "框架":
                        result = result.Where(w => w.ProjectType == "框架");
                        break;
                }
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        /// <summary>
        /// 招标进度模块，同步数据到招标台账
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string UpdateProgress()
        {
            try
            {
                var accountID = 0;
                int.TryParse(Request.Form["accountID"], out accountID);

                var progressID = 0;
                int.TryParse(Request.Form["progressID"], out progressID);

                var infoAccount = db.Account.Find(accountID);
                var infoProgress = db.ProgressInfo.Find(progressID);

                infoAccount.TenderProgramAuditDate = infoProgress.TenderProgramAuditDate;
                infoAccount.ProgramAcceptDate = infoProgress.ProgramAcceptDate;
                infoAccount.TenderFileSaleStartDate = infoProgress.TenderFileSaleStartDate;
                infoAccount.TenderFileSaleEndDate = infoProgress.TenderFileSaleEndDate;
                infoAccount.TenderStartDate = infoProgress.TenderStartDate;
                infoAccount.IsSynchro = "是";
                infoAccount.ProgressID = progressID;

                infoProgress.IsSynchro = "是";
                infoProgress.AccountID = accountID;

                db.SaveChanges();

                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 根据招标进度ID,删除招标进度、招标台账同步关系
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string DelSynchroByProgressID()
        {
            try
            {
                var progressID = 0;
                int.TryParse(Request.Form["progressID"], out progressID);

                var infoAccount = db.Account.Where(w => w.ProgressID == progressID).FirstOrDefault();
                var infoProgress = db.ProgressInfo.Find(progressID);

                infoAccount.IsSynchro = null;
                infoAccount.ProgressID = null;

                infoProgress.AccountID = null;
                infoProgress.IsSynchro = null;
                db.SaveChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpPost]
        public string GetAccountNameByProgressID()
        {
            try
            {
                var postList =
   JsonConvert.DeserializeObject<Dictionary<String, Object>>(HttpUtility.UrlDecode(Request.Form.ToString()));

                var progressID = 0;
                int.TryParse(postList["progressID"].ToString(), out progressID);
                return db.Account.Where(w => w.ProgressID == progressID).FirstOrDefault().ProjectName;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion

        #region Crud

        /// <summary>
        /// 获取招标台账列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetList()
        {
            try
            {
                var limit = 0;
                int.TryParse(Request.Form["limit"], out limit);
                var offset = 0;
                int.TryParse(Request.Form["offset"], out offset);
                var accountType = Request.Form["projectType"];//台账类别,物资、框架、工程、服务

                var projectName = Request.Form["projectName"];//项目名称
                var tenderFileNum = Request.Form["tenderFileNum"];//项目文件编号

                //招标项目负责人ID
                var projectResponsiblePersonID = 0;
                int.TryParse(Request.Form["projectResponsiblePersonID"], out projectResponsiblePersonID);

                var tenderInfo = Request.Form["tenderInfo"];//招标情况
                var applyPerson = Request.Form["applyPerson"];//申请人
                var tenderSuccessPerson = Request.Form["tenderSuccessPerson"];//中标人名称

                var tenderStartDateStart = Request.Form["tenderStartDateStart"];//开标日期开始
                var tenderStartDateEnd = Request.Form["tenderStartDateEnd"];//开标日期结束

                var planInvestPriceStart = Request.Form["planInvestPriceStart"];//预计投资范围开始
                var planInvestPriceEnd = Request.Form["planInvestPriceEnd"];//预计投资范围结束

                var userInfo = App_Code.Commen.GetUserFromSession();
                var result = from a in db.Account
                             where a.ProjectType == accountType
                             select a;

                if (projectName.Trim() != string.Empty)
                {
                    result = result.Where(w => w.ProjectName.Contains(projectName));
                }

                if (tenderFileNum.Trim() != string.Empty)
                {
                    result = result.Where(w => w.TenderFileNum.Contains(tenderFileNum));
                }

                if (tenderInfo != string.Empty)
                {
                    result = result.Where(w => w.TenderInfo == tenderInfo);
                }

                if (applyPerson != string.Empty)
                {
                    result = result.Where(w => w.ApplyPerson.Contains(applyPerson));
                }

                if (tenderSuccessPerson != string.Empty)
                {
                    result = result.Where(w => w.TenderSuccessPerson.Contains(tenderSuccessPerson));
                }

                if (projectResponsiblePersonID != 0)
                {
                    result = result.Where(w => w.ProjectResponsiblePersonID == projectResponsiblePersonID);
                }

                if (!string.IsNullOrEmpty(tenderStartDateStart) & !string.IsNullOrEmpty(tenderStartDateEnd))
                {
                    var dateStart = Convert.ToDateTime(tenderStartDateStart);
                    var dateEnd = Convert.ToDateTime(tenderStartDateEnd);
                    result = result.Where(w => System.Data.Entity.DbFunctions.DiffMinutes(w.TenderStartDate, dateStart) <= 0 && System.Data.Entity.DbFunctions.DiffMinutes(w.TenderStartDate, dateEnd) >= 0);
                }

                if (!string.IsNullOrEmpty(planInvestPriceStart) & !string.IsNullOrEmpty(planInvestPriceEnd))
                {
                    var priceStart = Convert.ToDecimal(planInvestPriceStart);
                    var priceEnd = Convert.ToDecimal(planInvestPriceEnd);
                    result = result.Where(w => w.PlanInvestPrice >= priceStart && w.PlanInvestPrice <= priceEnd);
                }
                //if (User.IsInRole("招标管理"))
                //{
                //    if (!User.IsInRole("新建招标台账"))
                //    {
                //        result = result.Where(w => w.ProjectResponsiblePersonID == userInfo.UserID);
                //    }
                //}

                if (!User.IsInRole("领导查看"))
                {
                    if (!User.IsInRole("组长查看"))
                    {
                        if (User.IsInRole("招标管理"))
                        {
                            result = result.Where(w => w.ProjectResponsiblePersonID == userInfo.UserID);
                        }
                    }
                    else
                    {
                        //查看本组的人员，包括自己
                        if (User.IsInRole("组长查看"))
                        {
                            List<int> personList = new List<int>();
                            personList.Add(userInfo.UserID);//添加自己

                            //添加组内成员
                            var memberList = db.GroupLeader.Where(w => w.LeaderUserID == userInfo.UserID).ToList();
                            foreach (var item in memberList)
                            {
                                personList.Add(item.MemberUserID);
                            }

                            result = result.Where(w => personList.Contains(w.ProjectResponsiblePersonID));
                        }
                    }
                }

                var accountList = result.OrderByDescending(o => o.AccountID).Skip(offset).Take(limit).ToList();

                List<Models.ViewAccout> list = new List<Models.ViewAccout>();
                foreach (var item in accountList)
                {
                    var viewList = new Models.ViewAccout();
                    var accountChildFirst = db.AccountChild.Where(w => w.AccountID == item.AccountID && w.TableType == "first"&&w.TenderPersonVersion=="0").ToList();
                    var accountChildSecond = db.AccountChild.Where(w => w.AccountID == item.AccountID && w.TableType == "second"&&w.EvaluationVersion=="0").ToList();
                    var accountChildThird = db.AccountChild.Where(w => w.AccountID == item.AccountID && w.TableType == "third").ToList();
                    var accountChildFour = db.AccountChild.Where(w => w.AccountID == item.AccountID && w.TableType == "four").ToList();
                    var accountChildFive = db.AccountChild.Where(w => w.AccountID == item.AccountID && w.TableType == "five").ToList();
                    var accountChildConnect = db.AccountChild.Where(w => w.AccountID == item.AccountID && w.TableType == "Connect").ToList();
                    var accountChildDept = db.AccountChild.Where(w => w.AccountID == item.AccountID && w.TableType == "Dept").ToList();
                    var accountChildTenderSuccess = db.AccountChild.Where(w => w.AccountID == item.AccountID && w.TableType == "TenderSuccess"&&w.TenderSuccessPersonVersion=="0").ToList();

                    viewList.AccountID = item.AccountID;
                    viewList.account = item;
                    viewList.accountChildFirst = accountChildFirst;
                    viewList.accountChildSecond = accountChildSecond;
                    viewList.accountChildThird = accountChildThird;
                    viewList.accountChildFour = accountChildFour;
                    viewList.accountChildFive = accountChildFive;
                    viewList.accountChildConnect = accountChildConnect;
                    viewList.accountChildDept = accountChildDept;
                    viewList.accountChildTenderSuccess = accountChildTenderSuccess;
                    list.Add(viewList);
                }

                return Json(new { total = result.Count(), rows = list });
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        /// <summary>
        /// 新建招标台账
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string Insert()
        {
            try
            {
                var projectName = Request.Form["tbxProjectName"].ToString();
                var projectType = Request.Form["tbxProjectType"].ToString();
                var tenderFileNum = Request.Form["tbxTenderFileNum"].ToString();
                decimal planInvestPriceEdit = 0;
                decimal.TryParse(Request.Form["tbxPlanInvestPriceEdit"].ToString(), out planInvestPriceEdit);
                var isOnline = Request.Form["ddlIsOnline"].ToString();
                var projectResponsiblePersonID = 0;
                int.TryParse(Request.Form["ddlProjectResponsiblePerson"].ToString(), out projectResponsiblePersonID);
                var projectResponsiblePersonName = db.UserInfo.Find(projectResponsiblePersonID).UserName;

                var user = App_Code.Commen.GetUserFromSession();

                var info = new Models.Account();
                info.InsertDate = DateTime.Now;
                info.InsertPersonID = user.UserID;

                info.ProjectName = projectName;
                info.ProjectType = projectType;
                info.TenderFileNum = tenderFileNum;
                info.PlanInvestPrice = planInvestPriceEdit;
                info.IsOnline = isOnline;
                info.IsComplete = "否";
                info.ProjectResponsiblePersonID = projectResponsiblePersonID;
                info.ProjectResponsiblePersonName = projectResponsiblePersonName;
                db.Account.Add(info);
                db.SaveChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 更新招标台账内容
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string Update()
        {
            try
            {
                var accountID = 0;
                int.TryParse(Request.Form["tbxAccountIDEdit"], out accountID);
                var info = db.Account.Find(accountID);
                var userInfo = App_Code.Commen.GetUserFromSession();

                #region 使用单位~供货期
                //使用单位--物资、框架
                if (Request.Form["ddlUsingDeptEdit"] != null)
                {
                    var usingDeptID = 0;
                    int.TryParse(Request.Form["ddlUsingDeptEdit"], out usingDeptID);
                    info.UsingDeptID = usingDeptID;
                    info.UsingDeptName = db.DeptInfo.Find(usingDeptID).DeptName;
                }

                //项目主责单位
                var projectResponsibleDeptID = 0;
                int.TryParse(Request.Form["ddlProjectResponsibleDeptEdit"], out projectResponsibleDeptID);
                info.ProjectResponsibleDeptID = projectResponsibleDeptID;
                info.ProjectResponsibleDeptName = db.DeptInfo.Find(projectResponsibleDeptID).DeptName;

                info.ApplyPerson = Request.Form["tbxApplyPersonEdit"];
                info.InvestPlanApproveNum = Request.Form["tbxInvestPlanApproveNumEdit"];
                info.TenderRange = Request.Form["tbxTenderRangeEdit"];

                //物资、框架
                info.TenderMode = Request.Form["ddlTenderModeEdit"] ?? null;
                info.BidEvaluation = Request.Form["ddlBidEvaluationEdit"] ?? null;
                info.SupplyPeriod = Request.Form["tbxSupplyPeriodEdit"] ?? null;
                info.IsHaveCount = Request.Form["ddlIsHaveCountEdit"] ?? null;

                //工程、服务
                info.InvestSource = Request.Form["tbxInvestSourceEdit"] ?? null;
                info.ProjectTimeLimit = Request.Form["tbxProjectTimeLimitEdit"] ?? null;
                #endregion

                #region 招标方案联审时间~开标日期
                if (Request.Form["tbxTenderProgramAuditDateEdit"] != string.Empty)
                {
                    info.TenderProgramAuditDate = Convert.ToDateTime(Request.Form["tbxTenderProgramAuditDateEdit"]);
                }
                else
                {
                    info.TenderProgramAuditDate = null;
                }
                if (Request.Form["tbxProgramAcceptDateEdit"] != string.Empty)
                {
                    info.ProgramAcceptDate = Convert.ToDateTime(Request.Form["tbxProgramAcceptDateEdit"]);
                }
                else
                {
                    info.ProgramAcceptDate = null;
                }
                if (Request.Form["tbxTenderFileSaleStartDateEdit"] != string.Empty)
                {
                    info.TenderFileSaleStartDate = Convert.ToDateTime(Request.Form["tbxTenderFileSaleStartDateEdit"]);
                }
                else
                {
                    info.TenderFileSaleStartDate = null;
                }
                if (Request.Form["tbxTenderFileSaleEndDateEdit"] != string.Empty)
                {
                    info.TenderFileSaleEndDate = Convert.ToDateTime(Request.Form["tbxTenderFileSaleEndDateEdit"]);
                }
                else
                {
                    info.TenderFileSaleEndDate = null;
                }
                if (Request.Form["tbxTenderStartDateEdit"] != string.Empty)
                {
                    info.TenderStartDate = Convert.ToDateTime(Request.Form["tbxTenderStartDateEdit"]);
                }
                else
                {
                    info.TenderStartDate = null;
                }
                if (Request.Form["tbxTenderSuccessFileDateEdit"] != string.Empty)
                {
                    info.TenderSuccessFileDate = Convert.ToDateTime(Request.Form["tbxTenderSuccessFileDateEdit"]);
                }
                else
                {
                    info.TenderSuccessFileDate = null;
                }
                #endregion

                #region 中标人名称~与控制价比节约资金（元）
                info.TenderSuccessPerson = Request.Form["tbxTenderSuccessPersonEdit"];

                decimal planInvestPrice = 0;
                decimal.TryParse(Request.Form["tbxPlanInvestPriceEdit"], out planInvestPrice);
                info.PlanInvestPrice = planInvestPrice;

                decimal tenderRestrictUnitPrice = 0;
                decimal.TryParse(Request.Form["tbxTenderRestrictUnitPriceEdit"], out tenderRestrictUnitPrice);
                info.TenderRestrictUnitPrice = tenderRestrictUnitPrice;

                info.TenderRestrictSumPrice = Request.Form["tbxTenderRestrictSumPriceEdit"];

                decimal tenderSuccessUnitPrice = 0;
                decimal.TryParse(Request.Form["tbxTenderSuccessUnitPriceEdit"], out tenderSuccessUnitPrice);
                info.TenderSuccessUnitPrice = tenderSuccessUnitPrice;

                info.TenderSuccessSumPrice = Request.Form["tbxTenderSuccessSumPriceEdit"];

                decimal saveCapital = 0;
                decimal.TryParse(Request.Form["tbxSaveCapitalEdit"], out saveCapital);
                info.SaveCapital = saveCapital;
                #endregion

                //资格审查方式
                info.QualificationExamMethod = Request.Form["ddlQualificationExamMethod"];

                //招标文件联审--联审时间（小时）
                decimal tenderFileAuditTime = 0;
                decimal.TryParse(Request.Form["tbxTenderFileAuditTimeEdit"], out tenderFileAuditTime);
                info.TenderFileAuditTime = tenderFileAuditTime;

                //招标失败原因
                info.TenderFailReason = Request.Form["tbxTenderFailReasonEdit"];

                #region 合同信息&备注
                info.ContractNum = Request.Form["tbxContractNumEdit"];

                decimal contractPrice = 0;
                decimal.TryParse(Request.Form["tbxContractPriceEdit"], out contractPrice);
                info.ContractPrice = contractPrice;

                info.RelativePerson = Request.Form["tbxRelativePersonEdit"];
                info.TenderInfo = Request.Form["ddlTenderInfoEdit"];

                info.TenderRemark = Request.Form["tbxTenderRemarkEdit"];
                #endregion

                info.InputDate = DateTime.Now;
                info.InputPersonID = userInfo.UserID;

                if (info.IsSynchro == "是")
                {
                    var infoProgress = db.ProgressInfo.Find(info.ProgressID);
                    infoProgress.TenderProgramAuditDate = info.TenderProgramAuditDate;
                    infoProgress.ProgramAcceptDate = info.ProgramAcceptDate;
                    infoProgress.TenderFileSaleStartDate = info.TenderFileSaleStartDate;
                    infoProgress.TenderFileSaleEndDate = info.TenderFileSaleEndDate;
                    infoProgress.TenderStartDate = info.TenderStartDate;
                    infoProgress.TenderSuccessFileDate = info.TenderSuccessFileDate;
                    infoProgress.Remark = info.TenderRemark;
                }

                db.SaveChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 获取一项招标台账信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetOne()
        {
            try
            {
                var accountID = 0;
                int.TryParse(Request.Form["accountID"], out accountID);
                var info = db.Account.Find(accountID);
                return Json(info);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// 删除招标台账
        /// </summary>
        /// <returns>ok</returns>
        public string Del()
        {
            //删除台账信息，删除台账子表详细信息，删除日志台账信息，删除框架文件信息,删除同步招标进度的对应关系
            try
            {
                var id = 0;
                int.TryParse(Request.Form["id"], out id);

                var infoAccount = db.Account.Find(id);
                var infoAccountChild = db.AccountChild.Where(w => w.AccountID == id);

                //删除与招标进度的同步关系
                if (infoAccount.IsSynchro == "是")
                {
                    var infoProgress = db.ProgressInfo.Where(w => w.AccountID == id).FirstOrDefault();
                    infoProgress.IsSynchro = null;
                    infoProgress.AccountID = null;
                }

                //删除框架文件信息
                var fileList = infoAccountChild.Where(w => w.TableType == "Six").ToList();
                var filePath = Request.MapPath("~/FileUpload");
                foreach (var item in fileList)
                {
                    if (item.FrameFile != null)
                    {
                        var file = item.FrameFile;
                        var fullName = Path.Combine(filePath, file ?? "");
                        if (System.IO.File.Exists(fullName))
                        {
                            System.IO.File.Delete(fullName);
                        }
                    }
                }
                db.Account.Remove(infoAccount);
                db.AccountChild.RemoveRange(infoAccountChild);

                var userInfo = App_Code.Commen.GetUserFromSession();

                //删除招标台账，写入日志信息
                var logInfo = new Models.Log();
                logInfo.InputDateTime = DateTime.Now;
                logInfo.InputPersonID = userInfo.UserID;
                logInfo.InputPersonName = userInfo.UserName;
                logInfo.LogContent = "删除项目：" + infoAccount.ProjectName + "-" + "类型：【" + infoAccount.ProjectType + "】";
                logInfo.LogType = "删除招标台账";
                db.Log.Add(logInfo);

                db.SaveChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 台账完成提交
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string CompeletSubmit()
        {
            try
            {
                var accountID = 0;
                int.TryParse(Request.Form["tbxAccountIDEdit"], out accountID);
                var info = db.Account.Find(accountID);
                var userInfo = App_Code.Commen.GetUserFromSession();

                info.CompleteTime = DateTime.Now;
                info.IsComplete = "是";

                var approve = new Models.AccountApprove();
                approve.AccountID = accountID;
                approve.SubmitPersonID = userInfo.UserID;
                approve.SubmitPersonName = userInfo.UserName;
                approve.SubmitTime = DateTime.Now;
                approve.ApproveState = "提交台账完成";
                db.AccountApproves.Add(approve);

                db.SaveChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 新增台账修改申请
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string InsertApproveEdit()
        {
            try
            {
                var accountID = 0;
                int.TryParse(Request.Form["tbxNewApproveEditAccountID"], out accountID);
                var approveReason = Request.Form["tbxApproveReason"].ToString();

                var userInfo = App_Code.Commen.GetUserFromSession();

                var info = new Models.AccountApprove();
                info.AccountID = accountID;
                info.SubmitEditReason = approveReason;
                info.SubmitPersonID = userInfo.UserID;
                info.SubmitPersonName = userInfo.UserName;
                info.SubmitTime = DateTime.Now;
                info.ApproveState = "等待审核";
                db.AccountApproves.Add(info);

                var infoAccount = db.Account.Find(accountID);
                infoAccount.IsComplete = "修改审核";

                db.SaveChanges();

                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 获取台账修改审核信息列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetEditApproveList()
        {
            try
            {
                var accountID = 0;
                int.TryParse(Request.Form["accountID"], out accountID);

                var approveInfo = db.AccountApproves.Where(w => w.AccountID == accountID).OrderByDescending(o=>o.AccountApproveID).ToList();

                return Json(approveInfo);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        /// <summary>
        /// 台账修改审批
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string ApproveEdit()
        {
            try
            {
                var accountID = 0;
                int.TryParse(Request.Form["accountID"], out accountID);
                var approveType = Request.Form["approveType"].ToString();
                var backReason= Request.Form["backReason"].ToString();
                var userInfo = App_Code.Commen.GetUserFromSession();

                var info = db.AccountApproves.Where(w => w.AccountID == accountID).OrderByDescending(o => o.AccountApproveID).FirstOrDefault();
                info.ApproveState =approveType=="ok"? "审核通过" : "审核回退";
                info.ApprovePersonID = userInfo.UserID;
                info.ApprovePersonName = userInfo.UserName;
                info.ApproveTime = DateTime.Now;
                info.ApproveBackReason = backReason;

                var infoAccount = db.Account.Find(accountID);
                infoAccount.IsComplete = approveType == "ok" ? "否" : "是";

                db.SaveChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 评标复议插入数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string InsertVersion()
        {
            try
            {
                var id = 0;
                int.TryParse(Request.Form["id"], out id);

                //评标委员会信息
                var listEvaluation = db.AccountChild.Where(w => w.AccountID == id && w.TableType == "second").ToList();
                //投标人信息
                var listTenderPerson= db.AccountChild.Where(w => w.AccountID == id && w.TableType == "first").ToList();
                //中标人信息
                var listTenderSuccess= db.AccountChild.Where(w => w.AccountID == id && w.TableType == "tenderSuccess").ToList();

                var evealuationVersionTemp = 0;
                int.TryParse(listEvaluation.Last().EvaluationVersion,out evealuationVersionTemp);
                var tenderPersonVersionTemp = 0;
                int.TryParse(listEvaluation.Last().EvaluationVersion,out tenderPersonVersionTemp);
                var tenderSuccessVersionTemp = 0;
                int.TryParse(listEvaluation.Last().EvaluationVersion,out tenderSuccessVersionTemp);

                var listEvaluationLast = listEvaluation.Where(w => w.EvaluationVersion == evealuationVersionTemp.ToString());
                var listTenderPersonLast = listTenderPerson.Where(w => w.TenderPersonVersion == tenderPersonVersionTemp.ToString());
                var listTenderSuccessLast = listTenderSuccess.Where(w => w.TenderSuccessPersonVersion == tenderSuccessVersionTemp.ToString());

                var evealuationVersion = evealuationVersionTemp + 1;
                var tenderPersonVersion = tenderPersonVersionTemp + 1;
                var tenderSuccessVersion = tenderSuccessVersionTemp + 1;

                var insertEvaluationList = new List<Models.AccountChild>();
                foreach (var item in listEvaluationLast)
                {
                    var info = new Models.AccountChild();
                    info.TableType = "second";
                    info.AccountID = id;

                    info.EvaluationPersonName = item.EvaluationPersonName;
                    info.EvaluationPersonDeptID = item.EvaluationPersonDeptID;
                    info.EvaluationPersonDeptName =item.EvaluationPersonDeptName;
                    info.IsEvaluationDirector = item.IsEvaluationDirector;
                    info.EvaluationCost = item.EvaluationCost;
                    info.EvaluationTime = item.EvaluationTime;

                    info.EvaluationVersion = evealuationVersion.ToString();
                    info.InputDate = DateTime.Now;
                    info.InputPerson = item.InputPerson;
                    insertEvaluationList.Add(info);
                }
                db.AccountChild.AddRange(insertEvaluationList);

                var insertTenderPersonList = new List<Models.AccountChild>();
                foreach (var item in listTenderPersonLast)
                {
                    var info = new Models.AccountChild();

                    info.TableType = "first";
                    info.AccountID = id;

                    info.TenderFilePlanPayPerson = item.TenderFilePlanPayPerson;
                    info.TenderPerson = item.TenderPerson;
                    info.ProductManufacturer = item.ProductManufacturer;
                    info.QuotedPriceUnit = item.QuotedPriceUnit;
                    info.QuotedPriceSum = item.QuotedPriceSum;
                    info.NegationExplain = item.NegationExplain;
                    info.VetoReason = item.VetoReason;

                    info.TenderPersonVersion = tenderPersonVersion.ToString();
                    info.InputDate = DateTime.Now;
                    info.InputPerson = item.InputPerson;
                    insertTenderPersonList.Add(info);
                }
                db.AccountChild.AddRange(insertTenderPersonList);

                var insertTenderSuccessList = new List<Models.AccountChild>();
                foreach (var item in listTenderSuccessLast)
                {
                    var info = new Models.AccountChild();

                    info.TableType = "TenderSuccess";
                    info.AccountID = id;
                    info.TenderSuccessPerson = item.TenderSuccessPerson;
                    info.TenderSuccessPersonStartDate = item.TenderSuccessPersonStartDate;
                    info.TenderSuccessPersonEndDate = item.TenderSuccessPersonEndDate;
                    info.TenderSuccessPersonVersion = tenderSuccessVersion.ToString();

                    info.InputDate = DateTime.Now;
                    info.InputPerson = item.InputPerson;
                    insertTenderSuccessList.Add(info);
                }
                db.AccountChild.AddRange(insertTenderSuccessList);

                db.SaveChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion

        #region CrudChild
        /// <summary>
        /// 投标人信息--新增
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string InsertFirst()
        {
            try
            {
                var accountID = 0;
                int.TryParse(Request.Form["tbxAccountFirstID"], out accountID);

                var tenderFilePlanPayPersonEdit = Request.Form["tbxTenderFilePlanPayPersonEdit"] ?? "-";
                var tenderPersonEdit = Request.Form["tbxTenderPersonEdit"] ?? "-";
                var productManufacturerEdit = Request.Form["tbxProductManufacturerEdit"] ?? "-";
                decimal quotedPriceUnitEdit = 0;
                decimal.TryParse(Request.Form["tbxQuotedPriceUnitEdit"], out quotedPriceUnitEdit);
                var quotedPriceSumEdit = Request.Form["tbxQuotedPriceSumEdit"];
                var negationExplain = Request.Form["ddlNegationExplain"] ?? "-";
                var vetoReason = Request.Form["tbxVetoReasonEdit"] ?? "-";

                var userInfo = App_Code.Commen.GetUserFromSession();
                var info = new Models.AccountChild();

                info.TableType = "first";
                info.AccountID = accountID;

                info.TenderFilePlanPayPerson = tenderFilePlanPayPersonEdit == string.Empty ? "-" : tenderFilePlanPayPersonEdit;
                info.TenderPerson = tenderPersonEdit == string.Empty ? "-" : tenderPersonEdit;
                info.ProductManufacturer = productManufacturerEdit == string.Empty ? "-" : productManufacturerEdit;
                info.QuotedPriceUnit = quotedPriceUnitEdit;
                info.QuotedPriceSum = quotedPriceSumEdit == string.Empty ? "-" : quotedPriceSumEdit;
                info.NegationExplain = negationExplain == string.Empty ? "-" : negationExplain;
                info.VetoReason = vetoReason == string.Empty ? "-" : vetoReason;
                info.TenderPersonVersion = "0";

                info.InputDate = DateTime.Now;
                info.InputPerson = userInfo.UserID;
                db.AccountChild.Add(info);
                db.SaveChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 投标人信息-修改
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string UpdateFirst()
        {
            try
            {
                var accountChildID = 0;
                int.TryParse(Request.Form["tbxAccountChildFirstID"], out accountChildID);

                var tenderFilePlanPayPersonEdit = Request.Form["tbxTenderFilePlanPayPersonEdit"] ?? "-";
                var tenderPersonEdit = Request.Form["tbxTenderPersonEdit"] ?? "-";
                var productManufacturerEdit = Request.Form["tbxProductManufacturerEdit"] ?? "-";
                decimal quotedPriceUnitEdit = 0;
                decimal.TryParse(Request.Form["tbxQuotedPriceUnitEdit"], out quotedPriceUnitEdit);
                var quotedPriceSumEdit = Request.Form["tbxQuotedPriceSumEdit"];
                var negationExplain = Request.Form["ddlNegationExplain"] ?? "-";
                var vetoReason = Request.Form["tbxVetoReasonEdit"] ?? "-";

                var userInfo = App_Code.Commen.GetUserFromSession();
                var info = db.AccountChild.Find(accountChildID);

                info.TenderFilePlanPayPerson = tenderFilePlanPayPersonEdit == string.Empty ? "-" : tenderFilePlanPayPersonEdit;
                info.TenderPerson = tenderPersonEdit == string.Empty ? "-" : tenderPersonEdit;
                info.ProductManufacturer = productManufacturerEdit == string.Empty ? "-" : productManufacturerEdit;
                info.QuotedPriceUnit = quotedPriceUnitEdit;
                info.QuotedPriceSum = quotedPriceSumEdit == string.Empty ? "-" : quotedPriceSumEdit;
                info.NegationExplain = negationExplain == string.Empty ? "-" : negationExplain;
                info.VetoReason = vetoReason == string.Empty ? "-" : vetoReason;

                info.InputDate = DateTime.Now;
                info.InputPerson = userInfo.UserID;
                db.SaveChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 评标委员会--新增
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string InsertSecond()
        {
            try
            {
                var accountID = 0;
                int.TryParse(Request.Form["tbxAccountSecondID"], out accountID);

                var evaluationPersonNameEdit = Request.Form["tbxEvaluationPersonNameEdit"] ?? "-";
                var evaluationPersonDeptIDEdit = 0;
                int.TryParse(Request.Form["ddlEvaluationPersonDeptEdit"], out evaluationPersonDeptIDEdit);
                var evaluationPersonDeptNameEdit = db.DeptInfo.Find(evaluationPersonDeptIDEdit).DeptName;

                var isEvaluationDirectorEdit = Request.Form["ddlIsEvaluationDirectorEdit"];
                decimal evaluationCostEdit = 0;
                decimal.TryParse(Request.Form["tbxEvaluationCostEdit"], out evaluationCostEdit);
                decimal evaluationTime = 0;
                decimal.TryParse(Request.Form["tbxEvaluationTimeEdit"], out evaluationTime);

                var userInfo = App_Code.Commen.GetUserFromSession();
                var info = new Models.AccountChild();

                info.TableType = "second";
                info.AccountID = accountID;
                info.EvaluationPersonName = evaluationPersonNameEdit == string.Empty ? "-" : evaluationPersonNameEdit;
                info.EvaluationPersonDeptID = evaluationPersonDeptIDEdit;
                info.EvaluationPersonDeptName = evaluationPersonDeptNameEdit;
                info.IsEvaluationDirector = isEvaluationDirectorEdit;
                info.EvaluationCost = evaluationCostEdit;
                info.EvaluationTime = evaluationTime;
                info.EvaluationVersion = "0";

                info.InputDate = DateTime.Now;
                info.InputPerson = userInfo.UserID;
                db.AccountChild.Add(info);
                db.SaveChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 评标委员会--修改
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string UpdateSecond()
        {
            try
            {
                var accountChildID = 0;
                int.TryParse(Request.Form["tbxAccountChildSecondID"], out accountChildID);

                var evaluationPersonNameEdit = Request.Form["tbxEvaluationPersonNameEdit"] ?? "-";
                var evaluationPersonDeptIDEdit = 0;
                int.TryParse(Request.Form["ddlEvaluationPersonDeptEdit"], out evaluationPersonDeptIDEdit);
                var evaluationPersonDeptNameEdit = db.DeptInfo.Find(evaluationPersonDeptIDEdit).DeptName;

                var isEvaluationDirectorEdit = Request.Form["ddlIsEvaluationDirectorEdit"];
                decimal evaluationCostEdit = 0;
                decimal.TryParse(Request.Form["tbxEvaluationCostEdit"], out evaluationCostEdit);
                decimal evaluationTime = 0;
                decimal.TryParse(Request.Form["tbxEvaluationTimeEdit"], out evaluationTime);

                var userInfo = App_Code.Commen.GetUserFromSession();
                var info = db.AccountChild.Find(accountChildID);

                info.EvaluationPersonName = evaluationPersonNameEdit == string.Empty ? "-" : evaluationPersonNameEdit;
                info.EvaluationPersonDeptID = evaluationPersonDeptIDEdit;
                info.EvaluationPersonDeptName = evaluationPersonDeptNameEdit;
                info.IsEvaluationDirector = isEvaluationDirectorEdit;
                info.EvaluationCost = evaluationCostEdit;
                info.EvaluationTime = evaluationTime;

                info.InputDate = DateTime.Now;
                info.InputPerson = userInfo.UserID;
                db.SaveChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 招标文件联审--新增
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string InsertThird()
        {
            try
            {
                var accountID = 0;
                int.TryParse(Request.Form["tbxAccountThirdID"], out accountID);

                var tenderFileAuditPersonNameEdit = Request.Form["tbxTenderFileAuditPersonNameEdit"] ?? "-";
                var tenderFileAuditPersonDeptIDEdit = 0;
                int.TryParse(Request.Form["ddlTenderFileAuditPersonDeptEdit"], out tenderFileAuditPersonDeptIDEdit);
                var tenderFileAuditPersonDeptNameEdit = db.DeptInfo.Find(tenderFileAuditPersonDeptIDEdit).DeptName;
                decimal tenderFileAuditCostEdit = 0;
                decimal.TryParse(Request.Form["tbxTenderFileAuditCostEdit"], out tenderFileAuditCostEdit);

                var userInfo = App_Code.Commen.GetUserFromSession();
                var info = new Models.AccountChild();

                info.TableType = "Third";
                info.AccountID = accountID;
                info.TenderFileAuditPersonName = tenderFileAuditPersonNameEdit == string.Empty ? "-" : tenderFileAuditPersonNameEdit;
                info.TenderFileAuditPersonDeptID = tenderFileAuditPersonDeptIDEdit;
                info.TenderFileAuditPersonDeptName = tenderFileAuditPersonDeptNameEdit;
                info.TenderFileAuditCost = tenderFileAuditCostEdit;
                info.InputDate = DateTime.Now;
                info.InputPerson = userInfo.UserID;
                db.AccountChild.Add(info);
                db.SaveChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 招标文件联审--修改
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string UpdateThird()
        {
            try
            {
                var accountChildID = 0;
                int.TryParse(Request.Form["tbxAccountChildThirdID"], out accountChildID);

                var tenderFileAuditPersonNameEdit = Request.Form["tbxTenderFileAuditPersonNameEdit"] ?? "-";
                var tenderFileAuditPersonDeptIDEdit = 0;
                int.TryParse(Request.Form["ddlTenderFileAuditPersonDeptEdit"], out tenderFileAuditPersonDeptIDEdit);
                var tenderFileAuditPersonDeptNameEdit = db.DeptInfo.Find(tenderFileAuditPersonDeptIDEdit).DeptName;
                decimal tenderFileAuditCostEdit = 0;
                decimal.TryParse(Request.Form["tbxTenderFileAuditCostEdit"], out tenderFileAuditCostEdit);

                var userInfo = App_Code.Commen.GetUserFromSession();
                var info = db.AccountChild.Find(accountChildID);

                info.TenderFileAuditPersonName = tenderFileAuditPersonNameEdit == string.Empty ? "-" : tenderFileAuditPersonNameEdit;
                info.TenderFileAuditPersonDeptID = tenderFileAuditPersonDeptIDEdit;
                info.TenderFileAuditPersonDeptName = tenderFileAuditPersonDeptNameEdit;
                info.TenderFileAuditCost = tenderFileAuditCostEdit;
                info.InputDate = DateTime.Now;
                info.InputPerson = userInfo.UserID;
                db.SaveChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 澄清/修改--新增
        /// </summary>
        /// <param name="clarifyFile"></param>
        /// <returns></returns>
        [HttpPost]
        public string InsertFour(HttpPostedFileBase clarifyFile)
        {
            try
            {
                var accountID = 0;
                int.TryParse(Request.Form["tbxAccountFourID"], out accountID);

                var userInfo = App_Code.Commen.GetUserFromSession();
                var info = new Models.AccountChild();

                info.TableType = "Four";
                info.AccountID = accountID;

                var clarifyLaunchPerson = Request.Form["tbxClarifyLaunchPersonEdit"].Trim() ?? "-";
                info.ClarifyLaunchPerson = clarifyLaunchPerson.Trim() == string.Empty ? "-" : clarifyLaunchPerson;
                if (Request.Form["tbxClarifyLaunchDateEdit"] != string.Empty)
                {
                    info.ClarifyLaunchDate = Convert.ToDateTime(Request.Form["tbxClarifyLaunchDateEdit"]);
                }
                else
                {
                    info.ClarifyLaunchDate = null;
                }
                if (Request.Form["tbxClarifyAcceptDateEdit"] != string.Empty)
                {
                    info.ClarifyAcceptDate = Convert.ToDateTime(Request.Form["tbxClarifyAcceptDateEdit"]);
                }
                else
                {
                    info.ClarifyAcceptDate = null;
                }

                var clarifyDisposePerson = Request.Form["tbxClarifyDisposePersonEdit"].Trim() ?? "-";
                info.ClarifyDisposePerson = clarifyDisposePerson.Trim() == string.Empty ? "-" : clarifyDisposePerson;
                info.IsClarify = Request.Form["ddlIsClarifyEdit"];
                if (Request.Form["tbxClarifyReplyDateEdit"] != string.Empty)
                {
                    info.ClarifyReplyDate = Convert.ToDateTime(Request.Form["tbxClarifyReplyDateEdit"]);
                }
                else
                {
                    info.ClarifyReplyDate = null;
                }

                var clarifyReason = Request.Form["tbxClarifyReasonEdit"].Trim() ?? "-";
                info.ClarifyReason = clarifyReason.Trim() == string.Empty ? "-" : clarifyReason;
                var clarifyDisposeInfo = Request.Form["tbxClarifyDisposeInfoEdit"].Trim() ?? "-";
                info.ClarifyDisposeInfo = clarifyDisposeInfo.Trim() == string.Empty ? "-" : clarifyDisposeInfo;

                if (clarifyFile != null)
                {
                    var fileExt = Path.GetExtension(clarifyFile.FileName).ToLower();
                    var fileName = Path.GetFileNameWithoutExtension(clarifyFile.FileName).ToLower();
                    var dateString = App_Code.Commen.GetDateTimeString();
                    var newName = fileName + dateString + fileExt;
                    var filePath = Request.MapPath("~/FileUpload");
                    var fullName = Path.Combine(filePath, newName);
                    clarifyFile.SaveAs(fullName);

                    info.ClarifyFile = newName;
                }

                info.InputDate = DateTime.Now;
                info.InputPerson = userInfo.UserID;
                db.AccountChild.Add(info);
                db.SaveChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 澄清/修改--更新(更新附件时，删除旧的上传文件)
        /// </summary>
        /// <param name="clarifyFile"></param>
        /// <returns></returns>
        [HttpPost]
        public string UpdateFour(HttpPostedFileBase clarifyFile)
        {
            try
            {
                var accountChildID = 0;
                int.TryParse(Request.Form["tbxAccountChildFourID"], out accountChildID);

                var userInfo = App_Code.Commen.GetUserFromSession();
                var info = db.AccountChild.Find(accountChildID);

                var clarifyLaunchPerson = Request.Form["tbxClarifyLaunchPersonEdit"].Trim() ?? "-";
                info.ClarifyLaunchPerson = clarifyLaunchPerson.Trim() == string.Empty ? "-" : clarifyLaunchPerson;
                if (Request.Form["tbxClarifyLaunchDateEdit"] != string.Empty)
                {
                    info.ClarifyLaunchDate = Convert.ToDateTime(Request.Form["tbxClarifyLaunchDateEdit"]);
                }
                else
                {
                    info.ClarifyLaunchDate = null;
                }
                if (Request.Form["tbxClarifyAcceptDateEdit"] != string.Empty)
                {
                    info.ClarifyAcceptDate = Convert.ToDateTime(Request.Form["tbxClarifyAcceptDateEdit"]);
                }
                else
                {
                    info.ClarifyAcceptDate = null;
                }

                var clarifyDisposePerson = Request.Form["tbxClarifyDisposePersonEdit"].Trim() ?? "-";
                info.ClarifyDisposePerson = clarifyDisposePerson.Trim() == string.Empty ? "-" : clarifyDisposePerson;
                info.IsClarify = Request.Form["ddlIsClarifyEdit"];
                if (Request.Form["tbxClarifyReplyDateEdit"] != string.Empty)
                {
                    info.ClarifyReplyDate = Convert.ToDateTime(Request.Form["tbxClarifyReplyDateEdit"]);
                }
                else
                {
                    info.ClarifyReplyDate = null;
                }

                var clarifyReason = Request.Form["tbxClarifyReasonEdit"].Trim() ?? "-";
                info.ClarifyReason = clarifyReason.Trim() == string.Empty ? "-" : clarifyReason;
                var clarifyDisposeInfo = Request.Form["tbxClarifyDisposeInfoEdit"].Trim() ?? "-";
                info.ClarifyDisposeInfo = clarifyDisposeInfo.Trim() == string.Empty ? "-" : clarifyDisposeInfo;

                if (clarifyFile != null)
                {
                    var filePath = Request.MapPath("~/FileUpload");

                    var fileOld = info.ClarifyFile;
                    //删除旧的上传文件
                    var fullNameOld = Path.Combine(filePath, fileOld ?? "");
                    if (System.IO.File.Exists(fullNameOld))
                    {
                        System.IO.File.Delete(fullNameOld);
                    }

                    var fileExt = Path.GetExtension(clarifyFile.FileName).ToLower();
                    var fileName = Path.GetFileNameWithoutExtension(clarifyFile.FileName).ToLower();
                    var dateString = App_Code.Commen.GetDateTimeString();
                    var newName = fileName + dateString + fileExt;
                    var fullName = Path.Combine(filePath, newName);
                    clarifyFile.SaveAs(fullName);

                    info.ClarifyFile = newName;
                }

                info.InputDate = DateTime.Now;
                info.InputPerson = userInfo.UserID;
                db.SaveChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 异议处理--新增
        /// </summary>
        /// <param name="dissentFile"></param>
        /// <returns></returns>
        [HttpPost]
        public string InsertFive(HttpPostedFileBase dissentFile)
        {
            try
            {
                var accountID = 0;
                int.TryParse(Request.Form["tbxAccountFiveID"], out accountID);

                var userInfo = App_Code.Commen.GetUserFromSession();
                var info = new Models.AccountChild();

                info.TableType = "Five";
                info.AccountID = accountID;

                var dissentLaunchPerson = Request.Form["tbxDissentLaunchPersonEdit"] ?? "-";
                info.DissentLaunchPerson = dissentLaunchPerson.Trim() == string.Empty ? "-" : dissentLaunchPerson;

                var dissentLaunchPersonPhone = Request.Form["tbxDissentLaunchPersonPhoneEdit"] ?? "-";
                info.DissentLaunchPersonPhone = dissentLaunchPersonPhone.Trim() == string.Empty ? "-" : dissentLaunchPersonPhone;

                if (Request.Form["tbxDissentLaunchDateEdit"] != string.Empty)
                {
                    info.DissentLaunchDate = Convert.ToDateTime(Request.Form["tbxDissentLaunchDateEdit"]);
                }
                else
                {
                    info.DissentLaunchDate = null;
                }
                if (Request.Form["tbxDissentAcceptDateEdit"] != string.Empty)
                {
                    info.DissentAcceptDate = Convert.ToDateTime(Request.Form["tbxDissentAcceptDateEdit"]);
                }
                else
                {
                    info.DissentAcceptDate = null;
                }
                if (Request.Form["tbxDissentReplyDateEdit"] != string.Empty)
                {
                    info.DissentReplyDate = Convert.ToDateTime(Request.Form["tbxDissentReplyDateEdit"]);
                }
                else
                {
                    info.DissentReplyDate = null;
                }
                var dissentAcceptPerson = Request.Form["tbxDissentAcceptPersonEdit"] ?? "-";
                info.DissentAcceptPerson = dissentAcceptPerson.Trim() == string.Empty ? "-" : dissentAcceptPerson;

                var dissentDisposePerson = Request.Form["tbxDissentDisposePersonEdit"] ?? "-";
                info.DissentDisposePerson = dissentDisposePerson.Trim() == string.Empty ? "-" : dissentDisposePerson;

                var dissentReason = Request.Form["tbxDissentReasonEdit"] ?? "-";
                info.DissentReason = dissentReason == string.Empty ? "-" : dissentReason;

                var dissentDisposeInfo = Request.Form["tbxDissentDisposeInfoEdit"] ?? "-";
                info.DissentDisposeInfo = dissentDisposeInfo.Trim() == string.Empty ? "-" : dissentDisposeInfo;

                if (dissentFile != null)
                {
                    var fileExt = Path.GetExtension(dissentFile.FileName).ToLower();
                    var fileName = Path.GetFileNameWithoutExtension(dissentFile.FileName).ToLower();
                    var dateString = App_Code.Commen.GetDateTimeString();
                    var newName = fileName + dateString + fileExt;
                    var filePath = Request.MapPath("~/FileUpload");
                    var fullName = Path.Combine(filePath, newName);
                    dissentFile.SaveAs(fullName);

                    info.DissentFile = newName;
                }

                info.InputDate = DateTime.Now;
                info.InputPerson = userInfo.UserID;
                db.AccountChild.Add(info);
                db.SaveChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 异议处理--更新(更新附件时，删除旧的上传文件)
        /// </summary>
        /// <param name="dissentFile"></param>
        /// <returns></returns>
        [HttpPost]
        public string UpdateFive(HttpPostedFileBase dissentFile)
        {
            try
            {
                var accountChildID = 0;
                int.TryParse(Request.Form["tbxAccountChildFiveID"], out accountChildID);

                var userInfo = App_Code.Commen.GetUserFromSession();
                var info = db.AccountChild.Find(accountChildID);

                var dissentLaunchPerson = Request.Form["tbxDissentLaunchPersonEdit"] ?? "-";
                info.DissentLaunchPerson = dissentLaunchPerson.Trim() == string.Empty ? "-" : dissentLaunchPerson;

                var dissentLaunchPersonPhone = Request.Form["tbxDissentLaunchPersonPhoneEdit"] ?? "-";
                info.DissentLaunchPersonPhone = dissentLaunchPersonPhone.Trim() == string.Empty ? "-" : dissentLaunchPersonPhone;

                if (Request.Form["tbxDissentLaunchDateEdit"] != string.Empty)
                {
                    info.DissentLaunchDate = Convert.ToDateTime(Request.Form["tbxDissentLaunchDateEdit"]);
                }
                else
                {
                    info.DissentLaunchDate = null;
                }
                if (Request.Form["tbxDissentAcceptDateEdit"] != string.Empty)
                {
                    info.DissentAcceptDate = Convert.ToDateTime(Request.Form["tbxDissentAcceptDateEdit"]);
                }
                else
                {
                    info.DissentAcceptDate = null;
                }
                if (Request.Form["tbxDissentReplyDateEdit"] != string.Empty)
                {
                    info.DissentReplyDate = Convert.ToDateTime(Request.Form["tbxDissentReplyDateEdit"]);
                }
                else
                {
                    info.DissentReplyDate = null;
                }
                var dissentAcceptPerson = Request.Form["tbxDissentAcceptPersonEdit"] ?? "-";
                info.DissentAcceptPerson = dissentAcceptPerson.Trim() == string.Empty ? "-" : dissentAcceptPerson;

                var dissentDisposePerson = Request.Form["tbxDissentDisposePersonEdit"] ?? "-";
                info.DissentDisposePerson = dissentDisposePerson.Trim() == string.Empty ? "-" : dissentDisposePerson;

                var dissentReason = Request.Form["tbxDissentReasonEdit"] ?? "-";
                info.DissentReason = dissentReason == string.Empty ? "-" : dissentReason;

                var dissentDisposeInfo = Request.Form["tbxDissentDisposeInfoEdit"] ?? "-";
                info.DissentDisposeInfo = dissentDisposeInfo.Trim() == string.Empty ? "-" : dissentDisposeInfo;

                if (dissentFile != null)
                {
                    var filePath = Request.MapPath("~/FileUpload");

                    var fileOld = info.DissentFile;
                    //删除旧的上传文件
                    var fullNameOld = Path.Combine(filePath, fileOld ?? "");
                    if (System.IO.File.Exists(fullNameOld))
                    {
                        System.IO.File.Delete(fullNameOld);
                    }

                    var fileExt = Path.GetExtension(dissentFile.FileName).ToLower();
                    var fileName = Path.GetFileNameWithoutExtension(dissentFile.FileName).ToLower();
                    var dateString = App_Code.Commen.GetDateTimeString();
                    var newName = fileName + dateString + fileExt;
                    var fullName = Path.Combine(filePath, newName);
                    dissentFile.SaveAs(fullName);

                    info.DissentFile = newName;
                }

                info.InputDate = DateTime.Now;
                info.InputPerson = userInfo.UserID;
                db.SaveChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 前期沟通记录--新增
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string InsertConnect()
        {
            try
            {
                var accountID = 0;
                int.TryParse(Request.Form["tbxAccountConnectID"], out accountID);

                var userInfo = App_Code.Commen.GetUserFromSession();
                var info = new Models.AccountChild();

                info.TableType = "Connect";
                info.AccountID = accountID;

                var connectPerson = Request.Form["tbxConnectPersonEdit"] ?? "-";
                info.ConnectPerson = connectPerson.Trim() == string.Empty ? "-" : connectPerson;

                if (Request.Form["tbxConnectDateTimeEdit"] != string.Empty)
                {
                    info.ConnectDateTime = Convert.ToDateTime(Request.Form["tbxConnectDateTimeEdit"]);
                }
                else
                {
                    info.ConnectDateTime = null;
                }
                var connectContent = Request.Form["tbxConnectContentEdit"] ?? "-";
                info.ConnectContent = connectContent.Trim() == string.Empty ? "-" : connectContent;

                var connectExistingProblems = Request.Form["tbxConnectExistingProblemsEdit"] ?? "-";
                info.ConnectExistingProblems = connectExistingProblems.Trim() == string.Empty ? "-" : connectExistingProblems;

                info.InputDate = DateTime.Now;
                info.InputPerson = userInfo.UserID;
                db.AccountChild.Add(info);
                db.SaveChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 前期沟通记录--修改
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string UpdateConnect()
        {
            try
            {
                var accountChildID = 0;
                int.TryParse(Request.Form["tbxAccountChildConnectID"], out accountChildID);

                var userInfo = App_Code.Commen.GetUserFromSession();
                var info = db.AccountChild.Find(accountChildID);

                var connectPerson = Request.Form["tbxConnectPersonEdit"] ?? "-";
                info.ConnectPerson = connectPerson.Trim() == string.Empty ? "-" : connectPerson;

                if (Request.Form["tbxConnectDateTimeEdit"] != string.Empty)
                {
                    info.ConnectDateTime = Convert.ToDateTime(Request.Form["tbxConnectDateTimeEdit"]);
                }
                else
                {
                    info.ConnectDateTime = null;
                }
                var connectContent = Request.Form["tbxConnectContentEdit"] ?? "-";
                info.ConnectContent = connectContent.Trim() == string.Empty ? "-" : connectContent;

                var connectExistingProblems = Request.Form["tbxConnectExistingProblemsEdit"] ?? "-";
                info.ConnectExistingProblems = connectExistingProblems.Trim() == string.Empty ? "-" : connectExistingProblems;

                info.InputDate = DateTime.Now;
                info.InputPerson = userInfo.UserID;
                db.SaveChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 使用单位--新增
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string InsertDept()
        {
            try
            {
                var accountID = 0;
                int.TryParse(Request.Form["tbxAccountDeptID"], out accountID);

                var userInfo = App_Code.Commen.GetUserFromSession();
                var info = new Models.AccountChild();

                info.TableType = "Dept";
                info.AccountID = accountID;

                if (Request.Form["ddlUsingDeptEdit"] != null)
                {
                    var usingDeptID = 0;
                    int.TryParse(Request.Form["ddlUsingDeptEdit"], out usingDeptID);
                    info.UsingDeptID = usingDeptID;
                    info.UsingDeptName = db.DeptInfo.Find(usingDeptID).DeptName;
                }

                info.InputDate = DateTime.Now;
                info.InputPerson = userInfo.UserID;
                db.AccountChild.Add(info);
                db.SaveChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 使用单位--修改
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string UpdateDept()
        {
            try
            {
                var accountChildID = 0;
                int.TryParse(Request.Form["tbxAccountChildDeptID"], out accountChildID);

                var userInfo = App_Code.Commen.GetUserFromSession();
                var info = db.AccountChild.Find(accountChildID);

                if (Request.Form["ddlUsingDeptEdit"] != null)
                {
                    var usingDeptID = 0;
                    int.TryParse(Request.Form["ddlUsingDeptEdit"], out usingDeptID);
                    info.UsingDeptID = usingDeptID;
                    info.UsingDeptName = db.DeptInfo.Find(usingDeptID).DeptName;
                }

                info.InputDate = DateTime.Now;
                info.InputPerson = userInfo.UserID;
                db.SaveChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 中标人信息--新增
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string InsertTenderSuccess()
        {
            try
            {
                var accountID = 0;
                int.TryParse(Request.Form["tbxAccountTenderSuccessID"], out accountID);

                var userInfo = App_Code.Commen.GetUserFromSession();
                var info = new Models.AccountChild();

                info.TableType = "TenderSuccess";
                info.AccountID = accountID;
                info.TenderSuccessPerson = Request.Form["tbxTenderSuccessPersonEdit"];
                if (Request.Form["tbxTenderSuccessPersonStartDateEdit"] != string.Empty)
                {
                    info.TenderSuccessPersonStartDate = Convert.ToDateTime(Request.Form["tbxTenderSuccessPersonStartDateEdit"]);
                }
                else
                {
                    info.TenderSuccessPersonStartDate = null;
                }
                if (Request.Form["tbxTenderSuccessPersonEndDateEdit"] != string.Empty)
                {
                    info.TenderSuccessPersonEndDate = Convert.ToDateTime(Request.Form["tbxTenderSuccessPersonEndDateEdit"]);
                }
                else
                {
                    info.TenderSuccessPersonEndDate = null;
                }
                info.TenderSuccessPersonVersion = "0";

                info.InputDate = DateTime.Now;
                info.InputPerson = userInfo.UserID;
                db.AccountChild.Add(info);
                db.SaveChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 中标人信息--修改
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string UpdateTenderSuccess()
        {
            try
            {
                var accountChildID = 0;
                int.TryParse(Request.Form["tbxAccountChildTenderSuccessID"], out accountChildID);

                var userInfo = App_Code.Commen.GetUserFromSession();
                var info = db.AccountChild.Find(accountChildID);

                info.TenderSuccessPerson = Request.Form["tbxTenderSuccessPersonEdit"];
                if (Request.Form["tbxTenderSuccessPersonStartDateEdit"] != string.Empty)
                {
                    info.TenderSuccessPersonStartDate = Convert.ToDateTime(Request.Form["tbxTenderSuccessPersonStartDateEdit"]);
                }
                else
                {
                    info.TenderSuccessPersonStartDate = null;
                }
                if (Request.Form["tbxTenderSuccessPersonEndDateEdit"] != string.Empty)
                {
                    info.TenderSuccessPersonEndDate = Convert.ToDateTime(Request.Form["tbxTenderSuccessPersonEndDateEdit"]);
                }
                else
                {
                    info.TenderSuccessPersonEndDate = null;
                }
                info.TenderSuccessPersonVersion = "0";

                info.InputDate = DateTime.Now;
                info.InputPerson = userInfo.UserID;
                db.SaveChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 获取招标台账子表单项信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetOneEdit()
        {
            try
            {
                var accountChildID = 0;
                int.TryParse(Request.Form["tbxAccountChildID"], out accountChildID);
                var info = db.AccountChild.Find(accountChildID);
                return Json(info);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        /// <summary>
        /// 获取招标台账子表列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetListEdit()
        {
            try
            {
                var accountID = 0;
                int.TryParse(Request.Form["accountID"], out accountID);
                var type = Request.Form["type"];

                ////将物资、框架历史的使用单位信息，写入台账子表的单位使用信息,用于转移历史数据
                ////将历史单一一项使用单位信息，变为多个使用单位信息
                //if (type == "Dept")
                //{
                //    var infoAccount = db.Account.Find(accountID);
                //    var infoChild = db.AccountChild.Where(w => w.AccountID == accountID && w.TableType == "Dept").ToList();
                //    if (infoChild.Count == 0)
                //    {
                //        if (infoAccount.UsingDeptID != 0)
                //        {
                //            var child = new Models.AccountChild();
                //            child.UsingDeptID = infoAccount.UsingDeptID;
                //            child.UsingDeptName = infoAccount.UsingDeptName;
                //            child.TableType = "Dept";
                //            child.AccountID = accountID;
                //            child.InputDate = DateTime.Now;
                //            child.InputPerson = App_Code.Commen.GetUserFromSession().UserID;

                //            db.AccountChild.Add(child);
                //            db.SaveChanges();
                //        }
                //    }
                //}
                var info = db.AccountChild.Where(w => w.TableType == type && w.AccountID == accountID).ToList();
                return Json(info);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        /// <summary>
        /// 删除招标台账子表单项信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string DelEdit()
        {
            try
            {
                var accountChildID = 0;
                int.TryParse(Request.Form["tbxAccountChildID"], out accountChildID);
                var info = db.AccountChild.Find(accountChildID);

                var filePath = Request.MapPath("~/FileUpload");

                //删除旧的上传文件
                var fileClarify = info.ClarifyFile;
                var fileDissent = info.DissentFile;
                var fullNameClarify = Path.Combine(filePath, fileClarify ?? "");
                var fullNameDissent = Path.Combine(filePath, fileDissent ?? "");
                if (System.IO.File.Exists(fullNameClarify))
                {
                    System.IO.File.Delete(fullNameClarify);
                }
                if (System.IO.File.Exists(fullNameDissent))
                {
                    System.IO.File.Delete(fullNameDissent);
                }

                db.AccountChild.Remove(info);
                db.SaveChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 框架文件上传
        /// </summary>
        /// <param name="frameFile"></param>
        /// <returns></returns>
        [HttpPost]
        public string UploadFrameFile(HttpPostedFileBase frameFile)
        {
            var accountID = 0;
            int.TryParse(Request.Form["tbxAccountSixID"], out accountID);

            var userInfo = App_Code.Commen.GetUserFromSession();
            var info = new Models.AccountChild();

            info.TableType = "Six";
            info.AccountID = accountID;

            try
            {
                if (frameFile != null)
                {
                    var fileExt = Path.GetExtension(frameFile.FileName).ToLower();
                    var fileName = Path.GetFileNameWithoutExtension(frameFile.FileName).ToLower();
                    var dateString = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString();
                    var newName = fileName + dateString + fileExt;
                    var filePath = Request.MapPath("~/FileUpload");
                    var fullName = Path.Combine(filePath, newName);
                    frameFile.SaveAs(fullName);

                    info.FrameFile = newName;
                    info.InputDate = DateTime.Now;
                    info.InputPerson = userInfo.UserID;
                }
                db.AccountChild.Add(info);
                db.SaveChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 框架文件删除
        /// </summary>
        /// <returns></returns>
        public string DelFrameFile()
        {
            try
            {
                var accountChildID = 0;
                int.TryParse(Request.Form["tbxAccountChildID"], out accountChildID);
                var info = db.AccountChild.Find(accountChildID);

                var filePath = Request.MapPath("~/FileUpload");
                var file = info.FrameFile;

                var fullName = Path.Combine(filePath, file ?? "");
                if (System.IO.File.Exists(fullName))
                {
                    System.IO.File.Delete(fullName);
                }
                db.AccountChild.Remove(info);
                db.SaveChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 批量上传投标人信息
        /// </summary>
        /// <param name="tbxImportFile"></param>
        /// <returns></returns>
        [HttpPost]
        public string UploadImportFirst(HttpPostedFileBase tbxImportFile)
        {
            try
            {
                var accountID = 0;
                int.TryParse(Request.Form["tbxImportID"], out accountID);
                var typeFile = Request.Form["tbxImportType"].ToString();

                //将文件临时上传，写入数据库后删除
                var fileExt = Path.GetExtension(tbxImportFile.FileName).ToLower();
                var filePath = Request.MapPath("~/FileUpload");
                var newName = Guid.NewGuid() + fileExt;
                var fullName = Path.Combine(filePath, newName);
                tbxImportFile.SaveAs(fullName);

                //调用excel读取方法，将excel表中的数据读取到dataset
                var excel = App_Code.Commen.ReadExcel(fullName);

                var userInfo = App_Code.Commen.GetUserFromSession();
                List<Models.AccountChild> list = new List<Models.AccountChild>();
                switch (typeFile)
                {
                    case "FirstProject":
                        for (int i = 0; i < excel.Rows.Count; i++)
                        {
                            var info = new Models.AccountChild();
                            info.TableType = "first";
                            info.AccountID = accountID;
                            info.TenderFilePlanPayPerson =
                                excel.Rows[i]["购买招标文件潜在投标人"].ToString().Trim() == "" ? "-" : excel.Rows[i]["购买招标文件潜在投标人"].ToString();
                            info.TenderPerson = excel.Rows[i]["投标人"].ToString().Trim() == "" ? "-" : excel.Rows[i]["投标人"].ToString();
                            info.ProductManufacturer = null;
                            info.NegationExplain = null;
                            info.QuotedPriceSum = excel.Rows[i]["报价（万元）"].ToString().Trim() == "" ? "-" : excel.Rows[i]["报价（万元）"].ToString();
                            info.QuotedPriceUnit = 0;
                            info.InputDate = DateTime.Now;
                            info.InputPerson = userInfo.UserID;

                            list.Add(info);
                        }
                        break;
                    case "FirstMaterial":
                        for (int i = 0; i < excel.Rows.Count; i++)
                        {
                            var info = new Models.AccountChild();
                            info.TableType = "first";
                            info.AccountID = accountID;
                            info.TenderFilePlanPayPerson =
                                excel.Rows[i]["购买招标文件潜在投标人"].ToString().Trim() == "" ? "-" : excel.Rows[i]["购买招标文件潜在投标人"].ToString();
                            info.TenderPerson = excel.Rows[i]["投标人"].ToString().Trim() == "" ? "-" : excel.Rows[i]["投标人"].ToString();
                            info.ProductManufacturer = excel.Rows[i]["产品制造商（代理、贸易商投标时填写）"].ToString().Trim() == "" ? "-" : excel.Rows[i]["产品制造商（代理、贸易商投标时填写）"].ToString();

                            decimal priceOne = 0;
                            decimal.TryParse(excel.Rows[i]["报价--单价"].ToString(), out priceOne);
                            info.QuotedPriceUnit = priceOne;

                            info.QuotedPriceSum = excel.Rows[i]["报价--总价（万元）"].ToString().Trim() == "" ? "-" : excel.Rows[i]["报价--总价（万元）"].ToString();
                            info.NegationExplain = excel.Rows[i]["初步评审是否被否决"].ToString().Trim() == "" ? "-" : excel.Rows[i]["初步评审是否被否决"].ToString();
                            info.VetoReason = excel.Rows[i]["否决原因"].ToString().Trim() == "" ? "-" : excel.Rows[i]["否决原因"].ToString();
                            info.InputDate = DateTime.Now;
                            info.InputPerson = userInfo.UserID;

                            list.Add(info);
                        }
                        break;
                    case "FirstFrame":
                        for (int i = 0; i < excel.Rows.Count; i++)
                        {
                            var info = new Models.AccountChild();
                            info.TableType = "first";
                            info.AccountID = accountID;
                            info.TenderFilePlanPayPerson =
                                excel.Rows[i]["购买招标文件潜在投标人"].ToString().Trim() == "" ? "-" : excel.Rows[i]["购买招标文件潜在投标人"].ToString();
                            info.TenderPerson = excel.Rows[i]["投标人"].ToString().Trim() == "" ? "-" : excel.Rows[i]["投标人"].ToString();
                            info.ProductManufacturer = excel.Rows[i]["产品制造商（代理、贸易商投标时填写）"].ToString().Trim() == "" ? "-" : excel.Rows[i]["产品制造商（代理、贸易商投标时填写）"].ToString();

                            decimal quotedPriceUnit = 0;
                            decimal.TryParse(excel.Rows[i]["投标单价（元）"].ToString(), out quotedPriceUnit);
                            info.QuotedPriceUnit = quotedPriceUnit;

                            info.QuotedPriceSum = excel.Rows[i]["投标总价（万元）"].ToString().Trim() == "" ? "-" : excel.Rows[i]["投标总价（万元）"].ToString();

                            info.NegationExplain = excel.Rows[i]["初步评审是否被否决"].ToString().Trim() == "" ? "-" : excel.Rows[i]["初步评审是否被否决"].ToString();
                            info.VetoReason = excel.Rows[i]["否决原因"].ToString().Trim() == "" ? "-" : excel.Rows[i]["否决原因"].ToString();
                            info.InputDate = DateTime.Now;
                            info.InputPerson = userInfo.UserID;

                            list.Add(info);
                        }
                        break;
                }
                db.AccountChild.AddRange(list);
                db.SaveChanges();

                if (System.IO.File.Exists(fullName))
                {
                    System.IO.File.Delete(fullName);
                }
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 清空投标人信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string RemoveFirst()
        {
            try
            {
                var accountID = 0;
                int.TryParse(Request.Form["accountID"], out accountID);

                var list = db.AccountChild.Where(w => w.TableType == "first" && w.AccountID == accountID).ToList();
                db.AccountChild.RemoveRange(list);
                db.SaveChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 批量上传评标委员会信息
        /// </summary>
        /// <param name="tbxEvaluationFile"></param>
        /// <returns></returns>
        [HttpPost]
        public string UploadImportEvaluation(HttpPostedFileBase tbxEvaluationFile)
        {
            try
            {
                var accountID = 0;
                int.TryParse(Request.Form["tbxEvaluationID"], out accountID);

                //将文件临时上传，写入数据库后删除
                var fileExt = Path.GetExtension(tbxEvaluationFile.FileName).ToLower();
                var filePath = Request.MapPath("~/FileUpload");
                var newName = Guid.NewGuid() + fileExt;
                var fullName = Path.Combine(filePath, newName);
                tbxEvaluationFile.SaveAs(fullName);

                //调用excel读取方法，将excel表中的数据读取到dataset
                var excel = App_Code.Commen.ReadExcel(fullName);

                var userInfo = App_Code.Commen.GetUserFromSession();
                List<Models.AccountChild> list = new List<Models.AccountChild>();

                for (int i = 0; i < excel.Rows.Count; i++)
                {
                    var info = new Models.AccountChild();
                    info.TableType = "second";
                    info.AccountID = accountID;

                    info.EvaluationPersonName =
                        excel.Rows[i]["姓名"].ToString().Trim() == "" ? "-" : excel.Rows[i]["姓名"].ToString();

                    info.EvaluationPersonDeptName = excel.Rows[i]["单位名称"].ToString().Trim() == "" ? "-" : excel.Rows[i]["单位名称"].ToString();
                    info.EvaluationPersonDeptID = 0;

                    info.IsEvaluationDirector = excel.Rows[i]["是否评标委员会主任"].ToString();

                    decimal evaluationTime = 0;
                    decimal.TryParse(excel.Rows[i]["评审时间（小时）"].ToString(), out evaluationTime);
                    info.EvaluationTime = evaluationTime;

                    decimal evaluationPrice = 0;
                    decimal.TryParse(excel.Rows[i]["评审费"].ToString(), out evaluationPrice);
                    info.EvaluationCost = evaluationPrice;

                    info.InputDate = DateTime.Now;
                    info.InputPerson = userInfo.UserID;

                    list.Add(info);
                }
                db.AccountChild.AddRange(list);
                db.SaveChanges();

                if (System.IO.File.Exists(fullName))
                {
                    System.IO.File.Delete(fullName);
                }
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 清空评标委员会信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string RemoveEvaluation()
        {
            try
            {
                var accountID = 0;
                int.TryParse(Request.Form["accountID"], out accountID);

                var list = db.AccountChild.Where(w => w.TableType == "second" && w.AccountID == accountID).ToList();
                db.AccountChild.RemoveRange(list);
                db.SaveChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion

        #region 导出到Excel
        /// <summary>
        /// 导出物资台账到Excel
        /// </summary>
        /// <returns></returns>
        public FileResult AccountToExcel()
        {
            #region 获取台账数据
            var accountType = Request.QueryString["projectType"];//台账类别,物资、框架、工程、服务
            var projectName = Request.QueryString["projectName"];//项目名称
            var tenderFileNum = Request.QueryString["tenderFileNum"];//项目文件编号
            //招标项目负责人ID
            var projectResponsiblePersonID = 0;
            int.TryParse(Request.QueryString["projectResponsiblePersonID"], out projectResponsiblePersonID);

            var tenderInfo = Request.QueryString["tenderInfo"];//招标情况
            var applyPerson = Request.QueryString["applyPerson"];//申请人
            var tenderSuccessPerson = Request.QueryString["tenderSuccessPerson"];//中标人名称

            var tenderStartDateStart = Request.QueryString["tenderStartDateStart"];//开标日期开始
            var tenderStartDateEnd = Request.QueryString["tenderStartDateEnd"];//开标日期结束

            var planInvestPriceStart = Request.QueryString["planInvestPriceStart"];//预计投资范围开始
            var planInvestPriceEnd = Request.QueryString["planInvestPriceEnd"];//预计投资范围结束

            var userInfo = App_Code.Commen.GetUserFromSession();
            var result = from a in db.Account
                         where a.ProjectType == accountType
                         select a;
            if (projectName.Trim() != string.Empty)
            {
                result = result.Where(w => w.ProjectName.Contains(projectName));
            }

            if (tenderFileNum.Trim() != string.Empty)
            {
                result = result.Where(w => w.TenderFileNum.Contains(tenderFileNum));
            }

            if (tenderInfo != string.Empty)
            {
                result = result.Where(w => w.TenderInfo == tenderInfo);
            }

            if (applyPerson != string.Empty)
            {
                result = result.Where(w => w.ApplyPerson.Contains(applyPerson));
            }

            if (tenderSuccessPerson != string.Empty)
            {
                result = result.Where(w => w.TenderSuccessPerson.Contains(tenderSuccessPerson));
            }

            if (projectResponsiblePersonID != 0)
            {
                result = result.Where(w => w.ProjectResponsiblePersonID == projectResponsiblePersonID);
            }

            if (!string.IsNullOrEmpty(tenderStartDateStart) & !string.IsNullOrEmpty(tenderStartDateEnd))
            {
                var dateStart = Convert.ToDateTime(tenderStartDateStart);
                var dateEnd = Convert.ToDateTime(tenderStartDateEnd);
                result = result.Where(w => System.Data.Entity.DbFunctions.DiffMinutes(w.TenderStartDate, dateStart) <= 0 && System.Data.Entity.DbFunctions.DiffMinutes(w.TenderStartDate, dateEnd) >= 0);
            }

            if (!string.IsNullOrEmpty(planInvestPriceStart) & !string.IsNullOrEmpty(planInvestPriceEnd))
            {
                var priceStart = Convert.ToDecimal(planInvestPriceStart);
                var priceEnd = Convert.ToDecimal(planInvestPriceEnd);
                result = result.Where(w => w.PlanInvestPrice >= priceStart && w.PlanInvestPrice <= priceEnd);
            }

            if (!User.IsInRole("领导查看"))
            {
                if (!User.IsInRole("组长查看"))
                {
                    if (User.IsInRole("招标管理"))
                    {
                        result = result.Where(w => w.ProjectResponsiblePersonID == userInfo.UserID);
                    }
                }
                else
                {
                    //查看本组的人员，包括自己
                    if (User.IsInRole("组长查看"))
                    {
                        List<int> personList = new List<int>();
                        personList.Add(userInfo.UserID);//添加自己

                        //添加组内成员
                        var memberList = db.GroupLeader.Where(w => w.LeaderUserID == userInfo.UserID).ToList();
                        foreach (var item in memberList)
                        {
                            personList.Add(item.MemberUserID);
                        }

                        result = result.Where(w => personList.Contains(w.ProjectResponsiblePersonID));
                    }
                }
            }

            var accountList = result.OrderBy(o => o.AccountID).ToList();
            #endregion

            #region 设置文件路径和样式
            var filename = "台账统计信息" + App_Code.Commen.GetDateTimeString();
            string path = System.IO.Path.Combine(Server.MapPath("/"), "Template/ExportAccountMaterial.xls");
            Workbook workbook = new Workbook();
            workbook.Open(path);
            Cells cells = workbook.Worksheets[0].Cells;
            Worksheet ws = workbook.Worksheets[0];

            StyleFlag sf = new StyleFlag();
            sf.HorizontalAlignment = true;
            sf.VerticalAlignment = true;
            sf.WrapText = true;
            sf.Borders = true;

            Style style1 = workbook.Styles[workbook.Styles.Add()];//新增样式  
            style1.HorizontalAlignment = TextAlignmentType.Center;//文字居中 
            style1.VerticalAlignment = TextAlignmentType.Center;
            style1.IsTextWrapped = true;//单元格内容自动换行  
            style1.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; //应用边界线 左边界线  
            style1.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin; //应用边界线 右边界线  
            style1.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin; //应用边界线 上边界线  
            style1.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; //应用边界线 下边界线  
            #endregion

            int row = 2;//开始生成表格行数  
            foreach (var info in accountList)
            {
                var startmergepos = row;//开始合并的行位置，默认从row开始。
                List<int> countList = new List<int>();
                var childlist = (from c in db.AccountChild
                                 where c.AccountID == info.AccountID
                                 select c).ToList();
                #region 子表写入excel
                
                var firstList = childlist.Where(w => w.TableType == "first").ToList();//投标人
                var secondList = childlist.Where(w => w.TableType == "second").ToList();//评标委员会
                var thirdList = childlist.Where(w => w.TableType == "Third").ToList();//招标文件联审
                var fourList = childlist.Where(w => w.TableType == "Four").ToList();//澄清
                var fiveList = childlist.Where(w => w.TableType == "Five").ToList();//异议处理
                var deptList = childlist.Where(w => w.TableType == "Dept").ToList();//使用单位
                var connectList = childlist.Where(w => w.TableType == "Connect").ToList();//前期沟通记录
                var tenderSuccessList = childlist.Where(w => w.TableType == "TenderSuccess").ToList();//中标人信息

                countList.Add(firstList.Count);
                countList.Add(secondList.Count);
                countList.Add(thirdList.Count);
                countList.Add(fourList.Count);
                countList.Add(fiveList.Count);
                countList.Add(deptList.Count);
                countList.Add(connectList.Count);
                countList.Add(tenderSuccessList.Count);

                //取子表中行数最多的列,如果子表为空，设置为1，设置为要合并的行数。
                var rowsorder = countList.Max() == 0 ? 1 : countList.Max();
                
                var firstRow = startmergepos;//投标人信息
                foreach (var item in firstList)
                {
                    cells[firstRow, 23].PutValue(item.TenderFilePlanPayPerson);
                    cells[firstRow, 23].SetStyle(style1);
                    cells[firstRow, 24].PutValue(item.TenderPerson);
                    cells[firstRow, 24].SetStyle(style1);
                    cells[firstRow, 25].PutValue(item.ProductManufacturer);
                    cells[firstRow, 25].SetStyle(style1);
                    cells[firstRow, 26].PutValue(item.QuotedPriceUnit);
                    cells[firstRow, 26].SetStyle(style1);
                    cells[firstRow, 27].PutValue(item.QuotedPriceSum);
                    cells[firstRow, 27].SetStyle(style1);
                    cells[firstRow, 28].PutValue(item.NegationExplain);
                    cells[firstRow, 28].SetStyle(style1);
                    cells[firstRow, 29].PutValue(item.VetoReason);
                    cells[firstRow, 29].SetStyle(style1);
                    cells[firstRow, 30].PutValue(item.TenderPersonVersion);
                    cells[firstRow, 30].SetStyle(style1);
                    firstRow += 1;
                }
                //当子表数据小于最大行子表时，补齐子表剩余行的边框
                if (rowsorder > firstList.Count)
                {
                    var reduceCount = startmergepos + firstList.Count;
                    for (int i = 0; i < rowsorder - firstList.Count; i++)
                    {
                        cells[reduceCount + i, 23].SetStyle(style1);
                        cells[reduceCount + i, 24].SetStyle(style1);
                        cells[reduceCount + i, 25].SetStyle(style1);
                        cells[reduceCount + i, 26].SetStyle(style1);
                        cells[reduceCount + i, 27].SetStyle(style1);
                        cells[reduceCount + i, 28].SetStyle(style1);
                        cells[reduceCount + i, 29].SetStyle(style1);
                        cells[reduceCount + i, 30].SetStyle(style1);
                    }
                }

                var secondRow = startmergepos;//评标委员会
                foreach (var item in secondList)
                {
                    cells[secondRow, 42].PutValue(item.EvaluationPersonName);
                    cells[secondRow, 42].SetStyle(style1);
                    cells[secondRow, 43].PutValue(item.EvaluationPersonDeptName);
                    cells[secondRow, 43].SetStyle(style1);
                    cells[secondRow, 44].PutValue(item.IsEvaluationDirector);
                    cells[secondRow, 44].SetStyle(style1);
                    cells[secondRow, 45].PutValue(item.EvaluationTime);
                    cells[secondRow, 45].SetStyle(style1);
                    cells[secondRow, 46].PutValue(item.EvaluationCost);
                    cells[secondRow, 46].SetStyle(style1);
                    cells[secondRow, 47].PutValue(item.EvaluationVersion);
                    cells[secondRow, 47].SetStyle(style1);
                    secondRow += 1;
                }
                if (rowsorder > secondList.Count)
                {
                    var reduceCount = startmergepos + secondList.Count;
                    for (int i = 0; i < rowsorder - secondList.Count; i++)
                    {
                        cells[reduceCount + i, 42].SetStyle(style1);
                        cells[reduceCount + i, 43].SetStyle(style1);
                        cells[reduceCount + i, 44].SetStyle(style1);
                        cells[reduceCount + i, 45].SetStyle(style1);
                        cells[reduceCount + i, 46].SetStyle(style1);
                        cells[reduceCount + i, 47].SetStyle(style1);
                    }
                }

                var thirdRow = startmergepos;//招标文件联审
                foreach (var item in thirdList)
                {
                    cells[thirdRow, 48].PutValue(item.TenderFileAuditPersonName);
                    cells[thirdRow, 48].SetStyle(style1);
                    cells[thirdRow, 49].PutValue(item.TenderFileAuditPersonDeptName);
                    cells[thirdRow, 49].SetStyle(style1);
                    cells[thirdRow, 51].PutValue(item.TenderFileAuditCost);
                    cells[thirdRow, 51].SetStyle(style1);
                    thirdRow += 1;
                }
                if (rowsorder > thirdList.Count)
                {
                    var reduceCount = startmergepos + thirdList.Count;
                    for (int i = 0; i < rowsorder - thirdList.Count; i++)
                    {
                        cells[reduceCount + i, 48].SetStyle(style1);
                        cells[reduceCount + i, 49].SetStyle(style1);
                        cells[reduceCount + i, 51].SetStyle(style1);
                    }
                }

                var fourRow = startmergepos;//澄清
                foreach (var item in fourList)
                {
                    cells[fourRow, 53].PutValue(item.ClarifyLaunchPerson);
                    cells[fourRow, 53].SetStyle(style1);
                    cells[fourRow, 54].PutValue(item.ClarifyLaunchDate.Value.ToString("yyyy-MM-dd"));
                    cells[fourRow, 54].SetStyle(style1);
                    cells[fourRow, 55].PutValue(item.ClarifyReason);
                    cells[fourRow, 55].SetStyle(style1);
                    cells[fourRow, 56].PutValue(item.ClarifyAcceptDate.Value.ToString("yyyy-MM-dd"));
                    cells[fourRow, 56].SetStyle(style1);
                    cells[fourRow, 57].PutValue(item.ClarifyDisposePerson);
                    cells[fourRow, 57].SetStyle(style1);
                    cells[fourRow, 58].PutValue(item.IsClarify);
                    cells[fourRow, 58].SetStyle(style1);
                    cells[fourRow, 59].PutValue(item.ClarifyDisposeInfo);
                    cells[fourRow, 59].SetStyle(style1);
                    cells[fourRow, 60].PutValue(item.ClarifyReplyDate == null ? "" : item.ClarifyReplyDate.Value.ToString("yyyy-MM-dd"));
                    cells[fourRow, 60].SetStyle(style1);
                    fourRow += 1;
                }
                if (rowsorder > fourList.Count)
                {
                    var reduceCount = startmergepos + fourList.Count;
                    for (int i = 0; i < rowsorder - fourList.Count; i++)
                    {
                        cells[reduceCount + i, 53].SetStyle(style1);
                        cells[reduceCount + i, 54].SetStyle(style1);
                        cells[reduceCount + i, 55].SetStyle(style1);
                        cells[reduceCount + i, 56].SetStyle(style1);
                        cells[reduceCount + i, 57].SetStyle(style1);
                        cells[reduceCount + i, 58].SetStyle(style1);
                        cells[reduceCount + i, 59].SetStyle(style1);
                        cells[reduceCount + i, 60].SetStyle(style1);
                    }
                }

                var fiveRow = startmergepos;//异议处理
                foreach (var item in fiveList)
                {
                    cells[fiveRow, 61].PutValue(item.DissentLaunchPerson);
                    cells[fiveRow, 61].SetStyle(style1);
                    cells[fiveRow, 62].PutValue(item.DissentLaunchPersonPhone);
                    cells[fiveRow, 62].SetStyle(style1);
                    cells[fiveRow, 63].PutValue(item.DissentLaunchDate.Value.ToString("yyyy-MM-dd"));
                    cells[fiveRow, 63].SetStyle(style1);
                    cells[fiveRow, 64].PutValue(item.DissentProposedStage);
                    cells[fiveRow, 64].SetStyle(style1);
                    cells[fiveRow, 65].PutValue(item.DissentReason);
                    cells[fiveRow, 65].SetStyle(style1);
                    cells[fiveRow, 66].PutValue(item.DissentAcceptDate.Value.ToString("yyyy-MM-dd"));
                    cells[fiveRow, 66].SetStyle(style1);
                    cells[fiveRow, 67].PutValue(item.DissentAcceptPerson);
                    cells[fiveRow, 67].SetStyle(style1);
                    cells[fiveRow, 68].PutValue(item.DissentDisposePerson);
                    cells[fiveRow, 68].SetStyle(style1);
                    cells[fiveRow, 69].PutValue(item.DissentDisposeInfo);
                    cells[fiveRow, 69].SetStyle(style1);
                    cells[fiveRow, 70].PutValue(item.DissentReplyDate.Value.ToString("yyyy-MM-dd"));
                    cells[fiveRow, 70].SetStyle(style1);
                    fiveRow += 1;
                }
                if (rowsorder > fiveList.Count)
                {
                    var reduceCount = startmergepos + fiveList.Count;
                    for (int i = 0; i < rowsorder - fiveList.Count; i++)
                    {
                        cells[reduceCount + i, 61].SetStyle(style1);
                        cells[reduceCount + i, 62].SetStyle(style1);
                        cells[reduceCount + i, 63].SetStyle(style1);
                        cells[reduceCount + i, 64].SetStyle(style1);
                        cells[reduceCount + i, 65].SetStyle(style1);
                        cells[reduceCount + i, 66].SetStyle(style1);
                        cells[reduceCount + i, 67].SetStyle(style1);
                        cells[reduceCount + i, 68].SetStyle(style1);
                        cells[reduceCount + i, 69].SetStyle(style1);
                        cells[reduceCount + i, 70].SetStyle(style1);
                    }
                }

                var deptRow = startmergepos;//使用单位
                foreach (var item in deptList)
                {
                    cells[deptRow, 5].PutValue(item.UsingDeptName);
                    cells[deptRow, 5].SetStyle(style1);

                    deptRow += 1;
                }
                if (rowsorder > deptList.Count)
                {
                    var reduceCount = startmergepos + deptList.Count;
                    for (int i = 0; i < rowsorder - deptList.Count; i++)
                    {
                        cells[reduceCount + i, 5].SetStyle(style1);
                    }
                }

                var connectRow = startmergepos;//前期沟通
                foreach (var item in connectList)
                {
                    cells[connectRow, 19].PutValue(item.ConnectPerson);
                    cells[connectRow, 19].SetStyle(style1);
                    cells[connectRow, 20].PutValue(item.ConnectDateTime.Value.ToString("yyyy-MM-dd"));
                    cells[connectRow, 20].SetStyle(style1);
                    cells[connectRow, 21].PutValue(item.ConnectContent);
                    cells[connectRow, 21].SetStyle(style1);
                    cells[connectRow, 22].PutValue(item.ConnectExistingProblems);
                    cells[connectRow, 22].SetStyle(style1);

                    connectRow += 1;
                }
                if (rowsorder > connectList.Count)
                {
                    var reduceCount = startmergepos + connectList.Count;
                    for (int i = 0; i < rowsorder - connectList.Count; i++)
                    {
                        cells[reduceCount + i, 19].SetStyle(style1);
                        cells[reduceCount + i, 20].SetStyle(style1);
                        cells[reduceCount + i, 21].SetStyle(style1);
                        cells[reduceCount + i, 22].SetStyle(style1);
                    }
                }

                var tenderSuccessRow = startmergepos;//中标人信息
                foreach (var item in tenderSuccessList)
                {
                    cells[tenderSuccessRow, 31].PutValue(item.TenderSuccessPerson);
                    cells[tenderSuccessRow, 31].SetStyle(style1);
                    cells[tenderSuccessRow, 32].PutValue(item.TenderSuccessPersonStartDate.Value.ToString("yyyy-MM-dd"));
                    cells[tenderSuccessRow, 32].SetStyle(style1);
                    cells[tenderSuccessRow, 33].PutValue(item.TenderSuccessPersonEndDate.Value.ToString("yyyy-MM-dd"));
                    cells[tenderSuccessRow, 33].SetStyle(style1);
                    cells[tenderSuccessRow, 34].PutValue(item.TenderSuccessPersonVersion);
                    cells[tenderSuccessRow, 34].SetStyle(style1);

                    tenderSuccessRow += 1;
                }
                if (rowsorder > tenderSuccessList.Count)
                {
                    var reduceCount = startmergepos + tenderSuccessList.Count;
                    for (int i = 0; i < rowsorder - tenderSuccessList.Count; i++)
                    {
                        cells[reduceCount + i, 31].SetStyle(style1);
                        cells[reduceCount + i, 32].SetStyle(style1);
                        cells[reduceCount + i, 33].SetStyle(style1);
                        cells[reduceCount + i, 34].SetStyle(style1);
                    }
                }

                //投标人信息
                if (firstList.Count == 0)
                {
                    Range range23 = ws.Cells.CreateRange(startmergepos, 23, rowsorder, 1);
                    range23.ApplyStyle(style1, sf);
                    range23.Merge();

                    Range range24 = ws.Cells.CreateRange(startmergepos, 24, rowsorder, 1);
                    range24.ApplyStyle(style1, sf);
                    range24.Merge();

                    Range range25 = ws.Cells.CreateRange(startmergepos, 25, rowsorder, 1);
                    range25.ApplyStyle(style1, sf);
                    range25.Merge();

                    Range range26 = ws.Cells.CreateRange(startmergepos, 26, rowsorder, 1);
                    range26.ApplyStyle(style1, sf);
                    range26.Merge();

                    Range range27 = ws.Cells.CreateRange(startmergepos, 27, rowsorder, 1);
                    range27.ApplyStyle(style1, sf);
                    range27.Merge();

                    Range range28 = ws.Cells.CreateRange(startmergepos, 28, rowsorder, 1);
                    range28.ApplyStyle(style1, sf);
                    range28.Merge();

                    Range range29 = ws.Cells.CreateRange(startmergepos, 29, rowsorder, 1);
                    range29.ApplyStyle(style1, sf);
                    range29.Merge();

                    Range range30 = ws.Cells.CreateRange(startmergepos, 30, rowsorder, 1);
                    range30.ApplyStyle(style1, sf);
                    range30.Merge();
                }

                //评标委员会
                if (secondList.Count == 0)
                {
                    Range range42 = ws.Cells.CreateRange(startmergepos, 42, rowsorder, 1);
                    range42.ApplyStyle(style1, sf);
                    range42.Merge();

                    Range range43 = ws.Cells.CreateRange(startmergepos, 43, rowsorder, 1);
                    range43.ApplyStyle(style1, sf);
                    range43.Merge();

                    Range range44 = ws.Cells.CreateRange(startmergepos, 44, rowsorder, 1);
                    range44.ApplyStyle(style1, sf);
                    range44.Merge();

                    Range range45 = ws.Cells.CreateRange(startmergepos, 45, rowsorder, 1);
                    range45.ApplyStyle(style1, sf);
                    range45.Merge();

                    Range range46 = ws.Cells.CreateRange(startmergepos, 46, rowsorder, 1);
                    range46.ApplyStyle(style1, sf);
                    range46.Merge();

                    Range range47 = ws.Cells.CreateRange(startmergepos, 47, rowsorder, 1);
                    range47.ApplyStyle(style1, sf);
                    range47.Merge();
                }

                //招标文件联审
                if (thirdList.Count == 0)
                {
                    Range range48 = ws.Cells.CreateRange(startmergepos, 48, rowsorder, 1);
                    range48.ApplyStyle(style1, sf);
                    range48.Merge();

                    Range range49 = ws.Cells.CreateRange(startmergepos, 49, rowsorder, 1);
                    range49.ApplyStyle(style1, sf);
                    range49.Merge();

                    Range range51 = ws.Cells.CreateRange(startmergepos, 51, rowsorder, 1);
                    range51.ApplyStyle(style1, sf);
                    range51.Merge();
                }

                //澄清
                if (fourList.Count == 0)
                {
                    Range range53 = ws.Cells.CreateRange(startmergepos, 53, rowsorder, 1);
                    range53.ApplyStyle(style1, sf);
                    range53.Merge();

                    Range range54 = ws.Cells.CreateRange(startmergepos, 54, rowsorder, 1);
                    range54.ApplyStyle(style1, sf);
                    range54.Merge();

                    Range range55 = ws.Cells.CreateRange(startmergepos, 55, rowsorder, 1);
                    range55.ApplyStyle(style1, sf);
                    range55.Merge();

                    Range range56 = ws.Cells.CreateRange(startmergepos, 56, rowsorder, 1);
                    range56.ApplyStyle(style1, sf);
                    range56.Merge();

                    Range range57 = ws.Cells.CreateRange(startmergepos, 57, rowsorder, 1);
                    range57.ApplyStyle(style1, sf);
                    range57.Merge();

                    Range range58 = ws.Cells.CreateRange(startmergepos, 58, rowsorder, 1);
                    range58.ApplyStyle(style1, sf);
                    range58.Merge();

                    Range range59 = ws.Cells.CreateRange(startmergepos, 59, rowsorder, 1);
                    range59.ApplyStyle(style1, sf);
                    range59.Merge();

                    Range range60 = ws.Cells.CreateRange(startmergepos, 60, rowsorder, 1);
                    range60.ApplyStyle(style1, sf);
                    range60.Merge();
                }

                //异议处理
                if (fiveList.Count == 0)
                {
                    Range range61 = ws.Cells.CreateRange(startmergepos, 61, rowsorder, 1);
                    range61.ApplyStyle(style1, sf);
                    range61.Merge();

                    Range range62 = ws.Cells.CreateRange(startmergepos, 62, rowsorder, 1);
                    range62.ApplyStyle(style1, sf);
                    range62.Merge();

                    Range range63 = ws.Cells.CreateRange(startmergepos, 63, rowsorder, 1);
                    range63.ApplyStyle(style1, sf);
                    range63.Merge();

                    Range range64 = ws.Cells.CreateRange(startmergepos, 64, rowsorder, 1);
                    range64.ApplyStyle(style1, sf);
                    range64.Merge();

                    Range range65 = ws.Cells.CreateRange(startmergepos, 65, rowsorder, 1);
                    range65.ApplyStyle(style1, sf);
                    range65.Merge();

                    Range range66 = ws.Cells.CreateRange(startmergepos, 66, rowsorder, 1);
                    range66.ApplyStyle(style1, sf);
                    range66.Merge();

                    Range range67 = ws.Cells.CreateRange(startmergepos, 67, rowsorder, 1);
                    range67.ApplyStyle(style1, sf);
                    range67.Merge();

                    Range range68 = ws.Cells.CreateRange(startmergepos, 68, rowsorder, 1);
                    range68.ApplyStyle(style1, sf);
                    range68.Merge();

                    Range range69 = ws.Cells.CreateRange(startmergepos, 69, rowsorder, 1);
                    range69.ApplyStyle(style1, sf);
                    range69.Merge();

                    Range range70 = ws.Cells.CreateRange(startmergepos, 70, rowsorder, 1);
                    range70.ApplyStyle(style1, sf);
                    range70.Merge();
                }

                //使用单位
                if (deptList.Count == 0)
                {
                    Range range5 = ws.Cells.CreateRange(startmergepos, 5, rowsorder, 1);
                    range5.ApplyStyle(style1, sf);
                    range5.Merge();
                }

                //前期沟通
                if (connectList.Count == 0)
                {
                    Range range19 = ws.Cells.CreateRange(startmergepos, 19, rowsorder, 1);
                    range19.ApplyStyle(style1, sf);
                    range19.Merge();

                    Range range20 = ws.Cells.CreateRange(startmergepos, 20, rowsorder, 1);
                    range20.ApplyStyle(style1, sf);
                    range20.Merge();

                    Range range21 = ws.Cells.CreateRange(startmergepos, 21, rowsorder, 1);
                    range21.ApplyStyle(style1, sf);
                    range21.Merge();

                    Range range22 = ws.Cells.CreateRange(startmergepos, 22, rowsorder, 1);
                    range22.ApplyStyle(style1, sf);
                    range22.Merge();
                }

                //中标人信息
                if (tenderSuccessList.Count == 0)
                {
                    Range range31 = ws.Cells.CreateRange(startmergepos, 31, rowsorder, 1);
                    range31.ApplyStyle(style1, sf);
                    range31.Merge();

                    Range range32 = ws.Cells.CreateRange(startmergepos, 32, rowsorder, 1);
                    range32.ApplyStyle(style1, sf);
                    range32.Merge();

                    Range range33 = ws.Cells.CreateRange(startmergepos, 33, rowsorder, 1);
                    range33.ApplyStyle(style1, sf);
                    range33.Merge();

                    Range range34 = ws.Cells.CreateRange(startmergepos, 34, rowsorder, 1);
                    range34.ApplyStyle(style1, sf);
                    range34.Merge();
                }
                #endregion

                #region 非子表写入excel

                cells[startmergepos, 0].PutValue(accountList.IndexOf(info) + 1);//序号
                Range range0 = ws.Cells.CreateRange(startmergepos, 0, rowsorder, 1);
                range0.ApplyStyle(style1, sf);
                range0.Merge();

                cells[startmergepos, 1].PutValue(info.ProjectName);//项目名称
                Range range1 = ws.Cells.CreateRange(startmergepos, 1, rowsorder, 1);
                range1.ApplyStyle(style1, sf);
                range1.Merge();

                cells[startmergepos, 2].PutValue(info.TenderFileNum);//项目文件编号
                Range range2 = ws.Cells.CreateRange(startmergepos, 2, rowsorder, 1);
                range2.ApplyStyle(style1, sf);
                range2.Merge();

                cells[startmergepos, 3].PutValue(info.IsOnline);//线上/线下
                Range range3 = ws.Cells.CreateRange(startmergepos, 3, rowsorder, 1);
                range3.ApplyStyle(style1, sf);
                range3.Merge();

                cells[startmergepos, 4].PutValue(info.ProjectResponsiblePersonName);//招标项目负责人
                Range range4 = ws.Cells.CreateRange(startmergepos, 4, rowsorder, 1);
                range4.ApplyStyle(style1, sf);
                range4.Merge();

                //cells[startmergepos, 5].PutValue(info.UsingDeptName);
                //Range range5 = ws.Cells.CreateRange(startmergepos, 5, rowsorder, 1);
                //range5.ApplyStyle(style1, sf);
                //range5.Merge();

                cells[startmergepos, 6].PutValue(info.ProjectResponsibleDeptName);//项目主责部门
                Range range6 = ws.Cells.CreateRange(startmergepos, 6, rowsorder, 1);
                range6.ApplyStyle(style1, sf);
                range6.Merge();

                cells[startmergepos, 7].PutValue(info.ApplyPerson);//申请人
                Range range7 = ws.Cells.CreateRange(startmergepos, 7, rowsorder, 1);
                range7.ApplyStyle(style1, sf);
                range7.Merge();

                cells[startmergepos, 8].PutValue(info.InvestPlanApproveNum);//投资计划批复文号
                Range range8 = ws.Cells.CreateRange(startmergepos, 8, rowsorder, 1);
                range8.ApplyStyle(style1, sf);
                range8.Merge();

                cells[startmergepos, 9].PutValue(info.TenderRange);//招标范围
                Range range9 = ws.Cells.CreateRange(startmergepos, 9, rowsorder, 1);
                range9.ApplyStyle(style1, sf);
                range9.Merge();

                cells[startmergepos, 10].PutValue(info.TenderMode);//招标方式
                Range range10 = ws.Cells.CreateRange(startmergepos, 10, rowsorder, 1);
                range10.ApplyStyle(style1, sf);
                range10.Merge();

                cells[startmergepos, 11].PutValue(info.BidEvaluation);//评标方法
                Range range11 = ws.Cells.CreateRange(startmergepos, 11, rowsorder, 1);
                range11.ApplyStyle(style1, sf);
                range11.Merge();

                cells[startmergepos, 12].PutValue(info.SupplyPeriod);//供货期
                Range range12 = ws.Cells.CreateRange(startmergepos, 12, rowsorder, 1);
                range12.ApplyStyle(style1, sf);
                range12.Merge();

                //招标方案联审时间
                cells[startmergepos, 13].PutValue(info.TenderProgramAuditDate == null ? "" : info.TenderProgramAuditDate.Value.ToString("yyyy-MM-dd"));
                Range range13 = ws.Cells.CreateRange(startmergepos, 13, rowsorder, 1);
                range13.ApplyStyle(style1, sf);
                range13.Merge();

                //收到方案日期
                cells[startmergepos, 14].PutValue(info.ProgramAcceptDate == null ? "" : info.ProgramAcceptDate.Value.ToString("yyyy-MM-dd"));
                Range range14 = ws.Cells.CreateRange(startmergepos, 14, rowsorder, 1);
                range14.ApplyStyle(style1, sf);
                range14.Merge();

                //发售招标文件开始日期
                cells[startmergepos, 15].PutValue(info.TenderFileSaleStartDate == null ? "" : info.TenderFileSaleStartDate.Value.ToString("yyyy-MM-dd HH:mm"));
                Range range15 = ws.Cells.CreateRange(startmergepos, 15, rowsorder, 1);
                range15.ApplyStyle(style1, sf);
                range15.Merge();

                //发售招标文件截止日期
                cells[startmergepos, 16].PutValue(info.TenderFileSaleEndDate == null ? "" : info.TenderFileSaleEndDate.Value.ToString("yyyy-MM-dd HH:mm"));
                Range range16 = ws.Cells.CreateRange(startmergepos, 16, rowsorder, 1);
                range16.ApplyStyle(style1, sf);
                range16.Merge();

                //开标日期
                cells[startmergepos, 17].PutValue(info.TenderStartDate == null ? "" : info.TenderStartDate.Value.ToString("yyyy-MM-dd HH:mm"));
                Range range17 = ws.Cells.CreateRange(startmergepos, 17, rowsorder, 1);
                range17.ApplyStyle(style1, sf);
                range17.Merge();

                //中标通知书发出时间
                cells[startmergepos, 18].PutValue(info.TenderSuccessFileDate == null ? "" : info.TenderSuccessFileDate.Value.ToString("yyyy-MM-dd HH:mm"));
                Range range18 = ws.Cells.CreateRange(startmergepos, 18, rowsorder, 1);
                range18.ApplyStyle(style1, sf);
                range18.Merge();

                //cells[startmergepos, 24].PutValue(info.TenderSuccessPerson);
                //Range range24 = ws.Cells.CreateRange(startmergepos, 24, rowsorder, 1);
                //range24.ApplyStyle(style1, sf);
                //range24.Merge();

                cells[startmergepos, 35].PutValue(info.PlanInvestPrice);//预计投资（万元）
                Range range35 = ws.Cells.CreateRange(startmergepos, 35, rowsorder, 1);
                range35.ApplyStyle(style1, sf);
                range35.Merge();

                cells[startmergepos, 36].PutValue(info.QualificationExamMethod);//资格审查方式
                Range range36 = ws.Cells.CreateRange(startmergepos, 36, rowsorder, 1);
                range36.ApplyStyle(style1, sf);
                range36.Merge();

                cells[startmergepos, 37].PutValue(info.TenderRestrictUnitPrice);//招标控制价--单价
                Range range37 = ws.Cells.CreateRange(startmergepos, 37, rowsorder, 1);
                range37.ApplyStyle(style1, sf);
                range37.Merge();

                cells[startmergepos, 38].PutValue(info.TenderRestrictSumPrice);//招标控制价--总价
                Range range38 = ws.Cells.CreateRange(startmergepos, 38, rowsorder, 1);
                range38.ApplyStyle(style1, sf);
                range38.Merge();

                cells[startmergepos, 39].PutValue(info.TenderSuccessUnitPrice);//中标金额--单价
                Range range39 = ws.Cells.CreateRange(startmergepos, 39, rowsorder, 1);
                range39.ApplyStyle(style1, sf);
                range39.Merge();

                cells[startmergepos, 40].PutValue(info.TenderSuccessSumPrice);//中标金额--总价
                Range range40 = ws.Cells.CreateRange(startmergepos, 40, rowsorder, 1);
                range40.ApplyStyle(style1, sf);
                range40.Merge();

                cells[startmergepos, 41].PutValue(info.SaveCapital);//节约资金（万元）
                Range range41 = ws.Cells.CreateRange(startmergepos, 41, rowsorder, 1);
                range41.ApplyStyle(style1, sf);
                range41.Merge();

                cells[startmergepos, 50].PutValue(info.TenderFileAuditTime);//联审时间（小时）
                Range range50 = ws.Cells.CreateRange(startmergepos, 50, rowsorder, 1);
                range50.ApplyStyle(style1, sf);
                range50.Merge();

                cells[startmergepos, 52].PutValue(info.TenderFailReason);//招标失败原因
                Range range52 = ws.Cells.CreateRange(startmergepos, 52, rowsorder, 1);
                range52.ApplyStyle(style1, sf);
                range52.Merge();

                //cells[startmergepos, 59].PutValue(info.ContractNum);
                //Range range59 = ws.Cells.CreateRange(startmergepos, 59, rowsorder, 1);
                //range59.ApplyStyle(style1, sf);
                //range59.Merge();

                //cells[startmergepos, 60].PutValue(info.ContractPrice);
                //Range range60 = ws.Cells.CreateRange(startmergepos, 60, rowsorder, 1);
                //range60.ApplyStyle(style1, sf);
                //range60.Merge();

                //cells[startmergepos, 61].PutValue(info.RelativePerson);
                //Range range61 = ws.Cells.CreateRange(startmergepos, 61, rowsorder, 1);
                //range61.ApplyStyle(style1, sf);
                //range61.Merge();

                cells[startmergepos, 71].PutValue(info.TenderInfo);//招标情况
                Range range71 = ws.Cells.CreateRange(startmergepos, 71, rowsorder, 1);
                range71.ApplyStyle(style1, sf);
                range71.Merge();

                cells[startmergepos, 72].PutValue(info.TenderRemark);//备注
                Range range72 = ws.Cells.CreateRange(startmergepos, 72, rowsorder, 1);
                range72.ApplyStyle(style1, sf);
                range72.Merge();
                #endregion

                //这是合并单元格后的行数，一定注意，要加上合并的行数
                row = startmergepos + rowsorder;
            }

            string fileToSave = System.IO.Path.Combine(Server.MapPath("/"), "ExcelOutPut/" + filename + ".xls");
            if (System.IO.File.Exists(fileToSave))
            {
                System.IO.File.Delete(fileToSave);
            }
            workbook.Save(fileToSave, FileFormatType.Excel97To2003);
            return File(fileToSave, "application/vnd.ms-excel", filename + ".xls");
        }

        /// <summary>
        /// 导出框架台账到Excel
        /// </summary>
        /// <returns></returns>
        public FileResult FrameToExcel()
        {
            #region 获取台账数据
            var accountType = Request.QueryString["projectType"];//台账类别,物资、框架、工程、服务
            var projectName = Request.QueryString["projectName"];//项目名称
            var tenderFileNum = Request.QueryString["tenderFileNum"];//项目文件编号
            //招标项目负责人ID
            var projectResponsiblePersonID = 0;
            int.TryParse(Request.QueryString["projectResponsiblePersonID"], out projectResponsiblePersonID);

            var tenderInfo = Request.QueryString["tenderInfo"];//招标情况
            var applyPerson = Request.QueryString["applyPerson"];//申请人
            var tenderSuccessPerson = Request.QueryString["tenderSuccessPerson"];//中标人名称

            var tenderStartDateStart = Request.QueryString["tenderStartDateStart"];//开标日期开始
            var tenderStartDateEnd = Request.QueryString["tenderStartDateEnd"];//开标日期结束

            var planInvestPriceStart = Request.QueryString["planInvestPriceStart"];//预计投资范围开始
            var planInvestPriceEnd = Request.QueryString["planInvestPriceEnd"];//预计投资范围结束

            var userInfo = App_Code.Commen.GetUserFromSession();
            var result = from a in db.Account
                         where a.ProjectType == accountType
                         select a;

            if (projectName.Trim() != string.Empty)
            {
                result = result.Where(w => w.ProjectName.Contains(projectName));
            }

            if (tenderFileNum.Trim() != string.Empty)
            {
                result = result.Where(w => w.TenderFileNum.Contains(tenderFileNum));
            }

            if (tenderInfo != string.Empty)
            {
                result = result.Where(w => w.TenderInfo == tenderInfo);
            }

            if (applyPerson != string.Empty)
            {
                result = result.Where(w => w.ApplyPerson.Contains(applyPerson));
            }

            if (tenderSuccessPerson != string.Empty)
            {
                result = result.Where(w => w.TenderSuccessPerson.Contains(tenderSuccessPerson));
            }

            if (projectResponsiblePersonID != 0)
            {
                result = result.Where(w => w.ProjectResponsiblePersonID == projectResponsiblePersonID);
            }

            if (!string.IsNullOrEmpty(tenderStartDateStart) & !string.IsNullOrEmpty(tenderStartDateEnd))
            {
                var dateStart = Convert.ToDateTime(tenderStartDateStart);
                var dateEnd = Convert.ToDateTime(tenderStartDateEnd);
                result = result.Where(w => System.Data.Entity.DbFunctions.DiffMinutes(w.TenderStartDate, dateStart) <= 0 && System.Data.Entity.DbFunctions.DiffMinutes(w.TenderStartDate, dateEnd) >= 0);
            }

            if (!string.IsNullOrEmpty(planInvestPriceStart) & !string.IsNullOrEmpty(planInvestPriceEnd))
            {
                var priceStart = Convert.ToDecimal(planInvestPriceStart);
                var priceEnd = Convert.ToDecimal(planInvestPriceEnd);
                result = result.Where(w => w.PlanInvestPrice >= priceStart && w.PlanInvestPrice <= priceEnd);
            }

            if (!User.IsInRole("领导查看"))
            {
                if (!User.IsInRole("组长查看"))
                {
                    if (User.IsInRole("招标管理"))
                    {
                        result = result.Where(w => w.ProjectResponsiblePersonID == userInfo.UserID);
                    }
                }
                else
                {
                    //查看本组的人员，包括自己
                    if (User.IsInRole("组长查看"))
                    {
                        List<int> personList = new List<int>();
                        personList.Add(userInfo.UserID);//添加自己

                        //添加组内成员
                        var memberList = db.GroupLeader.Where(w => w.LeaderUserID == userInfo.UserID).ToList();
                        foreach (var item in memberList)
                        {
                            personList.Add(item.MemberUserID);
                        }

                        result = result.Where(w => personList.Contains(w.ProjectResponsiblePersonID));
                    }
                }
            }

            var accountList = result.OrderBy(o => o.AccountID).ToList();
            #endregion

            #region 设置文件路径和样式
            var filename = "台账统计信息" + accountType + App_Code.Commen.GetDateTimeString();
            string path = System.IO.Path.Combine(Server.MapPath("/"), "Template/ExportAccountFrame.xls");
            Workbook workbook = new Workbook();
            workbook.Open(path);
            Cells cells = workbook.Worksheets[0].Cells;
            Worksheet ws = workbook.Worksheets[0];

            StyleFlag sf = new StyleFlag();
            sf.HorizontalAlignment = true;
            sf.VerticalAlignment = true;
            sf.WrapText = true;
            sf.Borders = true;

            Style style1 = workbook.Styles[workbook.Styles.Add()];//新增样式  
            style1.HorizontalAlignment = TextAlignmentType.Center;//文字居中 
            style1.VerticalAlignment = TextAlignmentType.Center;
            style1.IsTextWrapped = true;//单元格内容自动换行  
            style1.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; //应用边界线 左边界线  
            style1.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin; //应用边界线 右边界线  
            style1.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin; //应用边界线 上边界线  
            style1.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; //应用边界线 下边界线  
            #endregion

            int row = 2;//开始生成表格行数  
            foreach (var info in accountList)
            {
                var startmergepos = row;//开始合并的行位置，默认从row开始。
                List<int> countList = new List<int>();
                var childlist = (from c in db.AccountChild
                                 where c.AccountID == info.AccountID
                                 select c).ToList();

                #region 子表写入excel

                var firstList = childlist.Where(w => w.TableType == "first").ToList();//投标人
                var secondList = childlist.Where(w => w.TableType == "second").ToList();//评标委员会
                var thirdList = childlist.Where(w => w.TableType == "Third").ToList();//招标文件联审
                var fourList = childlist.Where(w => w.TableType == "Four").ToList();//澄清
                var fiveList = childlist.Where(w => w.TableType == "Five").ToList();//异议处理
                var deptList = childlist.Where(w => w.TableType == "Dept").ToList();//使用单位
                var connectList = childlist.Where(w => w.TableType == "Connect").ToList();//前期沟通记录
                var tenderSuccessList = childlist.Where(w => w.TableType == "TenderSuccess").ToList();//中标人信息

                countList.Add(firstList.Count);
                countList.Add(secondList.Count);
                countList.Add(thirdList.Count);
                countList.Add(fourList.Count);
                countList.Add(fiveList.Count);
                countList.Add(deptList.Count);
                countList.Add(connectList.Count);
                countList.Add(tenderSuccessList.Count);

                //取子表中行数最多的列,如果子表为空，设置为1，设置为要合并的行数。
                var rowsorder = countList.Max() == 0 ? 1 : countList.Max();

                var firstRow = startmergepos;//投标人信息
                foreach (var item in firstList)
                {
                    cells[firstRow, 24].PutValue(item.TenderFilePlanPayPerson);
                    cells[firstRow, 24].SetStyle(style1);
                    cells[firstRow, 25].PutValue(item.TenderPerson);
                    cells[firstRow, 25].SetStyle(style1);
                    cells[firstRow, 26].PutValue(item.ProductManufacturer);
                    cells[firstRow, 26].SetStyle(style1);
                    cells[firstRow, 27].PutValue(item.QuotedPriceUnit);
                    cells[firstRow, 27].SetStyle(style1);
                    cells[firstRow, 28].PutValue(item.QuotedPriceSum);
                    cells[firstRow, 28].SetStyle(style1);
                    cells[firstRow, 29].PutValue(item.NegationExplain);
                    cells[firstRow, 29].SetStyle(style1);
                    cells[firstRow, 30].PutValue(item.VetoReason);
                    cells[firstRow, 30].SetStyle(style1);
                    cells[firstRow, 31].PutValue(item.TenderPersonVersion);
                    cells[firstRow, 31].SetStyle(style1);
                    firstRow += 1;
                }
                //当子表数据小于最大行子表时，补齐子表剩余行的边框
                if (rowsorder > firstList.Count)
                {
                    var reduceCount = startmergepos + firstList.Count;
                    for (int i = 0; i < rowsorder - firstList.Count; i++)
                    {
                        cells[reduceCount + i, 24].SetStyle(style1);
                        cells[reduceCount + i, 25].SetStyle(style1);
                        cells[reduceCount + i, 26].SetStyle(style1);
                        cells[reduceCount + i, 27].SetStyle(style1);
                        cells[reduceCount + i, 28].SetStyle(style1);
                        cells[reduceCount + i, 29].SetStyle(style1);
                        cells[reduceCount + i, 30].SetStyle(style1);
                        cells[reduceCount + i, 31].SetStyle(style1);
                    }
                }

                var secondRow = startmergepos;//评标委员会
                foreach (var item in secondList)
                {
                    cells[secondRow, 43].PutValue(item.EvaluationPersonName);
                    cells[secondRow, 43].SetStyle(style1);
                    cells[secondRow, 44].PutValue(item.EvaluationPersonDeptName);
                    cells[secondRow, 44].SetStyle(style1);
                    cells[secondRow, 45].PutValue(item.IsEvaluationDirector);
                    cells[secondRow, 45].SetStyle(style1);
                    cells[secondRow, 46].PutValue(item.EvaluationTime);
                    cells[secondRow, 46].SetStyle(style1);
                    cells[secondRow, 47].PutValue(item.EvaluationCost);
                    cells[secondRow, 47].SetStyle(style1);
                    cells[secondRow, 48].PutValue(item.EvaluationVersion);
                    cells[secondRow, 48].SetStyle(style1);
                    secondRow += 1;
                }
                if (rowsorder > secondList.Count)
                {
                    var reduceCount = startmergepos + secondList.Count;
                    for (int i = 0; i < rowsorder - secondList.Count; i++)
                    {
                        cells[reduceCount + i, 43].SetStyle(style1);
                        cells[reduceCount + i, 44].SetStyle(style1);
                        cells[reduceCount + i, 45].SetStyle(style1);
                        cells[reduceCount + i, 46].SetStyle(style1);
                        cells[reduceCount + i, 47].SetStyle(style1);
                        cells[reduceCount + i, 48].SetStyle(style1);
                    }
                }

                var thirdRow = startmergepos;//招标文件联审
                foreach (var item in thirdList)
                {
                    cells[thirdRow, 49].PutValue(item.TenderFileAuditPersonName);
                    cells[thirdRow, 49].SetStyle(style1);
                    cells[thirdRow, 50].PutValue(item.TenderFileAuditPersonDeptName);
                    cells[thirdRow, 50].SetStyle(style1);
                    cells[thirdRow, 52].PutValue(item.TenderFileAuditCost);
                    cells[thirdRow, 52].SetStyle(style1);
                    thirdRow += 1;
                }
                if (rowsorder > thirdList.Count)
                {
                    var reduceCount = startmergepos + thirdList.Count;
                    for (int i = 0; i < rowsorder - thirdList.Count; i++)
                    {
                        cells[reduceCount + i, 49].SetStyle(style1);
                        cells[reduceCount + i, 50].SetStyle(style1);
                        cells[reduceCount + i, 52].SetStyle(style1);
                    }
                }

                var fourRow = startmergepos;//澄清
                foreach (var item in fourList)
                {
                    cells[fourRow, 54].PutValue(item.ClarifyLaunchPerson);
                    cells[fourRow, 54].SetStyle(style1);
                    cells[fourRow, 55].PutValue(item.ClarifyLaunchDate.Value.ToString("yyyy-MM-dd"));
                    cells[fourRow, 55].SetStyle(style1);
                    cells[fourRow, 56].PutValue(item.ClarifyReason);
                    cells[fourRow, 56].SetStyle(style1);
                    cells[fourRow, 57].PutValue(item.ClarifyAcceptDate.Value.ToString("yyyy-MM-dd"));
                    cells[fourRow, 57].SetStyle(style1);
                    cells[fourRow, 58].PutValue(item.ClarifyDisposePerson);
                    cells[fourRow, 58].SetStyle(style1);
                    cells[fourRow, 59].PutValue(item.IsClarify);
                    cells[fourRow, 59].SetStyle(style1);
                    cells[fourRow, 60].PutValue(item.ClarifyDisposeInfo);
                    cells[fourRow, 60].SetStyle(style1);
                    cells[fourRow, 61].PutValue(item.ClarifyReplyDate == null ? "" : item.ClarifyReplyDate.Value.ToString("yyyy-MM-dd"));
                    cells[fourRow, 61].SetStyle(style1);
                    fourRow += 1;
                }
                if (rowsorder > fourList.Count)
                {
                    var reduceCount = startmergepos + fourList.Count;
                    for (int i = 0; i < rowsorder - fourList.Count; i++)
                    {
                        cells[reduceCount + i, 54].SetStyle(style1);
                        cells[reduceCount + i, 55].SetStyle(style1);
                        cells[reduceCount + i, 56].SetStyle(style1);
                        cells[reduceCount + i, 57].SetStyle(style1);
                        cells[reduceCount + i, 58].SetStyle(style1);
                        cells[reduceCount + i, 59].SetStyle(style1);
                        cells[reduceCount + i, 60].SetStyle(style1);
                        cells[reduceCount + i, 61].SetStyle(style1);
                    }
                }

                var fiveRow = startmergepos;//异议处理
                foreach (var item in fiveList)
                {
                    cells[fiveRow, 62].PutValue(item.DissentLaunchPerson);
                    cells[fiveRow, 62].SetStyle(style1);
                    cells[fiveRow, 63].PutValue(item.DissentLaunchPersonPhone);
                    cells[fiveRow, 63].SetStyle(style1);
                    cells[fiveRow, 64].PutValue(item.DissentLaunchDate.Value.ToString("yyyy-MM-dd"));
                    cells[fiveRow, 64].SetStyle(style1);
                    cells[fiveRow, 65].PutValue(item.DissentProposedStage);
                    cells[fiveRow, 65].SetStyle(style1);
                    cells[fiveRow, 66].PutValue(item.DissentReason);
                    cells[fiveRow, 66].SetStyle(style1);
                    cells[fiveRow, 67].PutValue(item.DissentAcceptDate.Value.ToString("yyyy-MM-dd"));
                    cells[fiveRow, 67].SetStyle(style1);
                    cells[fiveRow, 68].PutValue(item.DissentAcceptPerson);
                    cells[fiveRow, 68].SetStyle(style1);
                    cells[fiveRow, 69].PutValue(item.DissentDisposePerson);
                    cells[fiveRow, 69].SetStyle(style1);
                    cells[fiveRow, 70].PutValue(item.DissentDisposeInfo);
                    cells[fiveRow, 70].SetStyle(style1);
                    cells[fiveRow, 71].PutValue(item.DissentReplyDate.Value.ToString("yyyy-MM-dd"));
                    cells[fiveRow, 71].SetStyle(style1);
                    fiveRow += 1;
                }
                if (rowsorder > fiveList.Count)
                {
                    var reduceCount = startmergepos + fiveList.Count;
                    for (int i = 0; i < rowsorder - fiveList.Count; i++)
                    {
                        cells[reduceCount + i, 62].SetStyle(style1);
                        cells[reduceCount + i, 63].SetStyle(style1);
                        cells[reduceCount + i, 64].SetStyle(style1);
                        cells[reduceCount + i, 65].SetStyle(style1);
                        cells[reduceCount + i, 66].SetStyle(style1);
                        cells[reduceCount + i, 67].SetStyle(style1);
                        cells[reduceCount + i, 68].SetStyle(style1);
                        cells[reduceCount + i, 69].SetStyle(style1);
                        cells[reduceCount + i, 70].SetStyle(style1);
                        cells[reduceCount + i, 71].SetStyle(style1);
                    }
                }

                var deptRow = startmergepos;//使用单位
                foreach (var item in deptList)
                {
                    cells[deptRow, 5].PutValue(item.UsingDeptName);
                    cells[deptRow, 5].SetStyle(style1);

                    deptRow += 1;
                }
                if (rowsorder > deptList.Count)
                {
                    var reduceCount = startmergepos + deptList.Count;
                    for (int i = 0; i < rowsorder - deptList.Count; i++)
                    {
                        cells[reduceCount + i, 5].SetStyle(style1);
                    }
                }

                var connectRow = startmergepos;//前期沟通
                foreach (var item in connectList)
                {
                    cells[connectRow, 20].PutValue(item.ConnectPerson);
                    cells[connectRow, 20].SetStyle(style1);
                    cells[connectRow, 21].PutValue(item.ConnectDateTime.Value.ToString("yyyy-MM-dd"));
                    cells[connectRow, 21].SetStyle(style1);
                    cells[connectRow, 22].PutValue(item.ConnectContent);
                    cells[connectRow, 22].SetStyle(style1);
                    cells[connectRow, 23].PutValue(item.ConnectExistingProblems);
                    cells[connectRow, 23].SetStyle(style1);

                    connectRow += 1;
                }
                if (rowsorder > connectList.Count)
                {
                    var reduceCount = startmergepos + connectList.Count;
                    for (int i = 0; i < rowsorder - connectList.Count; i++)
                    {
                        cells[reduceCount + i, 20].SetStyle(style1);
                        cells[reduceCount + i, 21].SetStyle(style1);
                        cells[reduceCount + i, 22].SetStyle(style1);
                        cells[reduceCount + i, 23].SetStyle(style1);
                    }
                }

                var tenderSuccessRow = startmergepos;//中标人信息
                foreach (var item in tenderSuccessList)
                {
                    cells[tenderSuccessRow, 32].PutValue(item.TenderSuccessPerson);
                    cells[tenderSuccessRow, 32].SetStyle(style1);
                    cells[tenderSuccessRow, 33].PutValue(item.TenderSuccessPersonStartDate.Value.ToString("yyyy-MM-dd"));
                    cells[tenderSuccessRow, 33].SetStyle(style1);
                    cells[tenderSuccessRow, 34].PutValue(item.TenderSuccessPersonEndDate.Value.ToString("yyyy-MM-dd"));
                    cells[tenderSuccessRow, 34].SetStyle(style1);
                    cells[tenderSuccessRow, 35].PutValue(item.TenderSuccessPersonVersion);
                    cells[tenderSuccessRow, 35].SetStyle(style1);

                    tenderSuccessRow += 1;
                }
                if (rowsorder > tenderSuccessList.Count)
                {
                    var reduceCount = startmergepos + tenderSuccessList.Count;
                    for (int i = 0; i < rowsorder - tenderSuccessList.Count; i++)
                    {
                        cells[reduceCount + i, 32].SetStyle(style1);
                        cells[reduceCount + i, 33].SetStyle(style1);
                        cells[reduceCount + i, 34].SetStyle(style1);
                        cells[reduceCount + i, 35].SetStyle(style1);
                    }
                }

                //投标人信息
                if (firstList.Count == 0)
                {
                    Range range24 = ws.Cells.CreateRange(startmergepos, 24, rowsorder, 1);
                    range24.ApplyStyle(style1, sf);
                    range24.Merge();

                    Range range25 = ws.Cells.CreateRange(startmergepos, 25, rowsorder, 1);
                    range25.ApplyStyle(style1, sf);
                    range25.Merge();

                    Range range26 = ws.Cells.CreateRange(startmergepos, 26, rowsorder, 1);
                    range26.ApplyStyle(style1, sf);
                    range26.Merge();

                    Range range27 = ws.Cells.CreateRange(startmergepos, 27, rowsorder, 1);
                    range27.ApplyStyle(style1, sf);
                    range27.Merge();

                    Range range28 = ws.Cells.CreateRange(startmergepos, 28, rowsorder, 1);
                    range28.ApplyStyle(style1, sf);
                    range28.Merge();

                    Range range29 = ws.Cells.CreateRange(startmergepos, 29, rowsorder, 1);
                    range29.ApplyStyle(style1, sf);
                    range29.Merge();

                    Range range30 = ws.Cells.CreateRange(startmergepos, 30, rowsorder, 1);
                    range30.ApplyStyle(style1, sf);
                    range30.Merge();

                    Range range31 = ws.Cells.CreateRange(startmergepos, 31, rowsorder, 1);
                    range31.ApplyStyle(style1, sf);
                    range31.Merge();
                }

                //评标委员会
                if (secondList.Count == 0)
                {
                    Range range43 = ws.Cells.CreateRange(startmergepos, 43, rowsorder, 1);
                    range43.ApplyStyle(style1, sf);
                    range43.Merge();

                    Range range44 = ws.Cells.CreateRange(startmergepos, 44, rowsorder, 1);
                    range44.ApplyStyle(style1, sf);
                    range44.Merge();

                    Range range45 = ws.Cells.CreateRange(startmergepos, 45, rowsorder, 1);
                    range45.ApplyStyle(style1, sf);
                    range45.Merge();

                    Range range46 = ws.Cells.CreateRange(startmergepos, 46, rowsorder, 1);
                    range46.ApplyStyle(style1, sf);
                    range46.Merge();

                    Range range47 = ws.Cells.CreateRange(startmergepos, 47, rowsorder, 1);
                    range47.ApplyStyle(style1, sf);
                    range47.Merge();

                    Range range48 = ws.Cells.CreateRange(startmergepos, 48, rowsorder, 1);
                    range48.ApplyStyle(style1, sf);
                    range48.Merge();
                }

                //招标文件联审
                if (thirdList.Count == 0)
                {
                    Range range49 = ws.Cells.CreateRange(startmergepos, 49, rowsorder, 1);
                    range49.ApplyStyle(style1, sf);
                    range49.Merge();

                    Range range50 = ws.Cells.CreateRange(startmergepos, 50, rowsorder, 1);
                    range50.ApplyStyle(style1, sf);
                    range50.Merge();

                    Range range52 = ws.Cells.CreateRange(startmergepos, 52, rowsorder, 1);
                    range52.ApplyStyle(style1, sf);
                    range52.Merge();
                }

                //澄清
                if (fourList.Count == 0)
                {
                    Range range54 = ws.Cells.CreateRange(startmergepos, 54, rowsorder, 1);
                    range54.ApplyStyle(style1, sf);
                    range54.Merge();

                    Range range55 = ws.Cells.CreateRange(startmergepos, 55, rowsorder, 1);
                    range55.ApplyStyle(style1, sf);
                    range55.Merge();

                    Range range56 = ws.Cells.CreateRange(startmergepos, 56, rowsorder, 1);
                    range56.ApplyStyle(style1, sf);
                    range56.Merge();

                    Range range57 = ws.Cells.CreateRange(startmergepos, 57, rowsorder, 1);
                    range57.ApplyStyle(style1, sf);
                    range57.Merge();

                    Range range58 = ws.Cells.CreateRange(startmergepos, 58, rowsorder, 1);
                    range58.ApplyStyle(style1, sf);
                    range58.Merge();

                    Range range59 = ws.Cells.CreateRange(startmergepos, 59, rowsorder, 1);
                    range59.ApplyStyle(style1, sf);
                    range59.Merge();

                    Range range60 = ws.Cells.CreateRange(startmergepos, 60, rowsorder, 1);
                    range60.ApplyStyle(style1, sf);
                    range60.Merge();

                    Range range61 = ws.Cells.CreateRange(startmergepos, 61, rowsorder, 1);
                    range61.ApplyStyle(style1, sf);
                    range61.Merge();
                }

                //异议处理
                if (fiveList.Count == 0)
                {
                    Range range62 = ws.Cells.CreateRange(startmergepos, 62, rowsorder, 1);
                    range62.ApplyStyle(style1, sf);
                    range62.Merge();

                    Range range63 = ws.Cells.CreateRange(startmergepos, 63, rowsorder, 1);
                    range63.ApplyStyle(style1, sf);
                    range63.Merge();

                    Range range64 = ws.Cells.CreateRange(startmergepos, 64, rowsorder, 1);
                    range64.ApplyStyle(style1, sf);
                    range64.Merge();

                    Range range65 = ws.Cells.CreateRange(startmergepos, 65, rowsorder, 1);
                    range65.ApplyStyle(style1, sf);
                    range65.Merge();

                    Range range66 = ws.Cells.CreateRange(startmergepos, 66, rowsorder, 1);
                    range66.ApplyStyle(style1, sf);
                    range66.Merge();

                    Range range67 = ws.Cells.CreateRange(startmergepos, 67, rowsorder, 1);
                    range67.ApplyStyle(style1, sf);
                    range67.Merge();

                    Range range68 = ws.Cells.CreateRange(startmergepos, 68, rowsorder, 1);
                    range68.ApplyStyle(style1, sf);
                    range68.Merge();

                    Range range69 = ws.Cells.CreateRange(startmergepos, 69, rowsorder, 1);
                    range69.ApplyStyle(style1, sf);
                    range69.Merge();

                    Range range70 = ws.Cells.CreateRange(startmergepos, 70, rowsorder, 1);
                    range70.ApplyStyle(style1, sf);
                    range70.Merge();

                    Range range71 = ws.Cells.CreateRange(startmergepos, 71, rowsorder, 1);
                    range71.ApplyStyle(style1, sf);
                    range71.Merge();
                }

                //使用单位
                if (deptList.Count == 0)
                {
                    Range range5 = ws.Cells.CreateRange(startmergepos, 5, rowsorder, 1);
                    range5.ApplyStyle(style1, sf);
                    range5.Merge();
                }

                //前期沟通
                if (connectList.Count == 0)
                {
                    Range range20 = ws.Cells.CreateRange(startmergepos, 20, rowsorder, 1);
                    range20.ApplyStyle(style1, sf);
                    range20.Merge();

                    Range range21 = ws.Cells.CreateRange(startmergepos, 21, rowsorder, 1);
                    range21.ApplyStyle(style1, sf);
                    range21.Merge();

                    Range range22 = ws.Cells.CreateRange(startmergepos, 22, rowsorder, 1);
                    range22.ApplyStyle(style1, sf);
                    range22.Merge();

                    Range range23 = ws.Cells.CreateRange(startmergepos, 23, rowsorder, 1);
                    range23.ApplyStyle(style1, sf);
                    range23.Merge();
                }

                //中标人信息
                if (tenderSuccessList.Count == 0)
                {
                    Range range32 = ws.Cells.CreateRange(startmergepos, 32, rowsorder, 1);
                    range32.ApplyStyle(style1, sf);
                    range32.Merge();

                    Range range33 = ws.Cells.CreateRange(startmergepos, 33, rowsorder, 1);
                    range33.ApplyStyle(style1, sf);
                    range33.Merge();

                    Range range34 = ws.Cells.CreateRange(startmergepos, 34, rowsorder, 1);
                    range34.ApplyStyle(style1, sf);
                    range34.Merge();

                    Range range35 = ws.Cells.CreateRange(startmergepos, 35, rowsorder, 1);
                    range35.ApplyStyle(style1, sf);
                    range35.Merge();
                }
                #endregion

                #region 非子表写入excel

                cells[startmergepos, 0].PutValue(accountList.IndexOf(info) + 1);//序号
                Range range0 = ws.Cells.CreateRange(startmergepos, 0, rowsorder, 1);
                range0.ApplyStyle(style1, sf);
                range0.Merge();

                cells[startmergepos, 1].PutValue(info.ProjectName);//项目名称
                Range range1 = ws.Cells.CreateRange(startmergepos, 1, rowsorder, 1);
                range1.ApplyStyle(style1, sf);
                range1.Merge();

                cells[startmergepos, 2].PutValue(info.TenderFileNum);//项目文件编号
                Range range2 = ws.Cells.CreateRange(startmergepos, 2, rowsorder, 1);
                range2.ApplyStyle(style1, sf);
                range2.Merge();

                cells[startmergepos, 3].PutValue(info.IsOnline);//线上/线下
                Range range3 = ws.Cells.CreateRange(startmergepos, 3, rowsorder, 1);
                range3.ApplyStyle(style1, sf);
                range3.Merge();

                cells[startmergepos, 4].PutValue(info.ProjectResponsiblePersonName);//招标项目负责人
                Range range4 = ws.Cells.CreateRange(startmergepos, 4, rowsorder, 1);
                range4.ApplyStyle(style1, sf);
                range4.Merge();

                //cells[startmergepos, 5].PutValue(info.UsingDeptName);
                //Range range5 = ws.Cells.CreateRange(startmergepos, 5, rowsorder, 1);
                //range5.ApplyStyle(style1, sf);
                //range5.Merge();

                cells[startmergepos, 6].PutValue(info.ProjectResponsibleDeptName);//项目主责部门
                Range range6 = ws.Cells.CreateRange(startmergepos, 6, rowsorder, 1);
                range6.ApplyStyle(style1, sf);
                range6.Merge();

                cells[startmergepos, 7].PutValue(info.ApplyPerson);//申请人
                Range range7 = ws.Cells.CreateRange(startmergepos, 7, rowsorder, 1);
                range7.ApplyStyle(style1, sf);
                range7.Merge();

                cells[startmergepos, 8].PutValue(info.InvestPlanApproveNum);//投资计划批复文号
                Range range8 = ws.Cells.CreateRange(startmergepos, 8, rowsorder, 1);
                range8.ApplyStyle(style1, sf);
                range8.Merge();

                cells[startmergepos, 9].PutValue(info.TenderRange);//招标范围
                Range range9 = ws.Cells.CreateRange(startmergepos, 9, rowsorder, 1);
                range9.ApplyStyle(style1, sf);
                range9.Merge();

                cells[startmergepos, 10].PutValue(info.IsHaveCount);//是否带量
                Range range10 = ws.Cells.CreateRange(startmergepos, 10, rowsorder, 1);
                range10.ApplyStyle(style1, sf);
                range10.Merge();

                cells[startmergepos, 11].PutValue(info.TenderMode);//招标方式
                Range range11 = ws.Cells.CreateRange(startmergepos, 11, rowsorder, 1);
                range11.ApplyStyle(style1, sf);
                range11.Merge();

                cells[startmergepos, 12].PutValue(info.BidEvaluation);//评标方法
                Range range12 = ws.Cells.CreateRange(startmergepos, 12, rowsorder, 1);
                range12.ApplyStyle(style1, sf);
                range12.Merge();

                cells[startmergepos, 13].PutValue(info.SupplyPeriod);//供货期
                Range range13 = ws.Cells.CreateRange(startmergepos, 13, rowsorder, 1);
                range13.ApplyStyle(style1, sf);
                range13.Merge();

                //招标方案联审时间
                cells[startmergepos, 14].PutValue(info.TenderProgramAuditDate == null ? "" : info.TenderProgramAuditDate.Value.ToString("yyyy-MM-dd"));
                Range range14 = ws.Cells.CreateRange(startmergepos, 14, rowsorder, 1);
                range14.ApplyStyle(style1, sf);
                range14.Merge();

                //收到方案日期
                cells[startmergepos, 15].PutValue(info.ProgramAcceptDate == null ? "" : info.ProgramAcceptDate.Value.ToString("yyyy-MM-dd"));
                Range range15 = ws.Cells.CreateRange(startmergepos, 15, rowsorder, 1);
                range15.ApplyStyle(style1, sf);
                range15.Merge();

                //发售招标文件开始日期
                cells[startmergepos, 16].PutValue(info.TenderFileSaleStartDate == null ? "" : info.TenderFileSaleStartDate.Value.ToString("yyyy-MM-dd HH:mm"));
                Range range16 = ws.Cells.CreateRange(startmergepos, 16, rowsorder, 1);
                range16.ApplyStyle(style1, sf);
                range16.Merge();

                //发售招标文件截止日期
                cells[startmergepos, 17].PutValue(info.TenderFileSaleEndDate == null ? "" : info.TenderFileSaleEndDate.Value.ToString("yyyy-MM-dd HH:mm"));
                Range range17 = ws.Cells.CreateRange(startmergepos, 17, rowsorder, 1);
                range17.ApplyStyle(style1, sf);
                range17.Merge();

                //开标日期
                cells[startmergepos, 18].PutValue(info.TenderStartDate == null ? "" : info.TenderStartDate.Value.ToString("yyyy-MM-dd HH:mm"));
                Range range18 = ws.Cells.CreateRange(startmergepos, 18, rowsorder, 1);
                range18.ApplyStyle(style1, sf);
                range18.Merge();

                //中标通知书发出时间
                cells[startmergepos, 19].PutValue(info.TenderSuccessFileDate == null ? "" : info.TenderSuccessFileDate.Value.ToString("yyyy-MM-dd HH:mm"));
                Range range19 = ws.Cells.CreateRange(startmergepos, 19, rowsorder, 1);
                range19.ApplyStyle(style1, sf);
                range19.Merge();

                //cells[startmergepos, 24].PutValue(info.TenderSuccessPerson);
                //Range range24 = ws.Cells.CreateRange(startmergepos, 24, rowsorder, 1);
                //range24.ApplyStyle(style1, sf);
                //range24.Merge();

                cells[startmergepos, 36].PutValue(info.PlanInvestPrice);//预计投资（万元）
                Range range36 = ws.Cells.CreateRange(startmergepos, 36, rowsorder, 1);
                range36.ApplyStyle(style1, sf);
                range36.Merge();

                cells[startmergepos, 37].PutValue(info.QualificationExamMethod);//资格审查方式
                Range range37 = ws.Cells.CreateRange(startmergepos, 37, rowsorder, 1);
                range37.ApplyStyle(style1, sf);
                range37.Merge();

                cells[startmergepos, 38].PutValue(info.TenderRestrictUnitPrice);//招标控制价--单价
                Range range38 = ws.Cells.CreateRange(startmergepos, 38, rowsorder, 1);
                range38.ApplyStyle(style1, sf);
                range38.Merge();

                cells[startmergepos, 39].PutValue(info.TenderRestrictSumPrice);//招标控制价--总价
                Range range39 = ws.Cells.CreateRange(startmergepos, 39, rowsorder, 1);
                range39.ApplyStyle(style1, sf);
                range39.Merge();

                cells[startmergepos, 40].PutValue(info.TenderSuccessUnitPrice);//中标金额--单价
                Range range40 = ws.Cells.CreateRange(startmergepos, 40, rowsorder, 1);
                range40.ApplyStyle(style1, sf);
                range40.Merge();

                cells[startmergepos, 41].PutValue(info.TenderSuccessSumPrice);//中标金额--总价
                Range range41 = ws.Cells.CreateRange(startmergepos, 41, rowsorder, 1);
                range41.ApplyStyle(style1, sf);
                range41.Merge();

                cells[startmergepos, 42].PutValue(info.SaveCapital);//节约资金（万元）
                Range range42 = ws.Cells.CreateRange(startmergepos, 42, rowsorder, 1);
                range42.ApplyStyle(style1, sf);
                range42.Merge();

                cells[startmergepos, 51].PutValue(info.TenderFileAuditTime);//联审时间（小时）
                Range range51 = ws.Cells.CreateRange(startmergepos, 51, rowsorder, 1);
                range51.ApplyStyle(style1, sf);
                range51.Merge();

                cells[startmergepos, 53].PutValue(info.TenderFailReason);//招标失败原因
                Range range53 = ws.Cells.CreateRange(startmergepos, 53, rowsorder, 1);
                range53.ApplyStyle(style1, sf);
                range53.Merge();

                //cells[startmergepos, 59].PutValue(info.ContractNum);
                //Range range59 = ws.Cells.CreateRange(startmergepos, 59, rowsorder, 1);
                //range59.ApplyStyle(style1, sf);
                //range59.Merge();

                //cells[startmergepos, 60].PutValue(info.ContractPrice);
                //Range range60 = ws.Cells.CreateRange(startmergepos, 60, rowsorder, 1);
                //range60.ApplyStyle(style1, sf);
                //range60.Merge();

                //cells[startmergepos, 61].PutValue(info.RelativePerson);
                //Range range61 = ws.Cells.CreateRange(startmergepos, 61, rowsorder, 1);
                //range61.ApplyStyle(style1, sf);
                //range61.Merge();

                cells[startmergepos, 72].PutValue(info.TenderInfo);//招标情况
                Range range72 = ws.Cells.CreateRange(startmergepos, 72, rowsorder, 1);
                range72.ApplyStyle(style1, sf);
                range72.Merge();

                cells[startmergepos, 73].PutValue(info.TenderRemark);//备注
                Range range73 = ws.Cells.CreateRange(startmergepos, 73, rowsorder, 1);
                range73.ApplyStyle(style1, sf);
                range73.Merge();
                #endregion

                //这是合并单元格后的行数，一定注意，要加上合并的行数
                row = startmergepos + rowsorder;
            }

            string fileToSave = System.IO.Path.Combine(Server.MapPath("/"), "ExcelOutPut/" + filename + ".xls");
            if (System.IO.File.Exists(fileToSave))
            {
                System.IO.File.Delete(fileToSave);
            }
            workbook.Save(fileToSave, FileFormatType.Excel97To2003);
            return File(fileToSave, "application/vnd.ms-excel", filename + ".xls");
        }

        /// <summary>
        /// 导出工程、服务台账到Excel
        /// </summary>
        /// <returns></returns>
        public FileResult ProjectToExcel()
        {
            #region 获取台账数据
            var accountType = Request.QueryString["projectType"];//台账类别,物资、框架、工程、服务
            var projectName = Request.QueryString["projectName"];//项目名称
            var tenderFileNum = Request.QueryString["tenderFileNum"];//项目文件编号
            //招标项目负责人ID
            var projectResponsiblePersonID = 0;
            int.TryParse(Request.QueryString["projectResponsiblePersonID"], out projectResponsiblePersonID);

            var tenderInfo = Request.QueryString["tenderInfo"];//招标情况
            var applyPerson = Request.QueryString["applyPerson"];//申请人
            var tenderSuccessPerson = Request.QueryString["tenderSuccessPerson"];//中标人名称

            var tenderStartDateStart = Request.QueryString["tenderStartDateStart"];//开标日期开始
            var tenderStartDateEnd = Request.QueryString["tenderStartDateEnd"];//开标日期结束

            var planInvestPriceStart = Request.QueryString["planInvestPriceStart"];//预计投资范围开始
            var planInvestPriceEnd = Request.QueryString["planInvestPriceEnd"];//预计投资范围结束

            var userInfo = App_Code.Commen.GetUserFromSession();
            var result = from a in db.Account
                         where a.ProjectType == accountType
                         select a;

            if (projectName.Trim() != string.Empty)
            {
                result = result.Where(w => w.ProjectName.Contains(projectName));
            }

            if (tenderFileNum.Trim() != string.Empty)
            {
                result = result.Where(w => w.TenderFileNum.Contains(tenderFileNum));
            }

            if (tenderInfo != string.Empty)
            {
                result = result.Where(w => w.TenderInfo == tenderInfo);
            }

            if (applyPerson != string.Empty)
            {
                result = result.Where(w => w.ApplyPerson.Contains(applyPerson));
            }

            if (tenderSuccessPerson != string.Empty)
            {
                result = result.Where(w => w.TenderSuccessPerson.Contains(tenderSuccessPerson));
            }

            if (projectResponsiblePersonID != 0)
            {
                result = result.Where(w => w.ProjectResponsiblePersonID == projectResponsiblePersonID);
            }

            if (!string.IsNullOrEmpty(tenderStartDateStart) & !string.IsNullOrEmpty(tenderStartDateEnd))
            {
                var dateStart = Convert.ToDateTime(tenderStartDateStart);
                var dateEnd = Convert.ToDateTime(tenderStartDateEnd);
                result = result.Where(w => System.Data.Entity.DbFunctions.DiffMinutes(w.TenderStartDate, dateStart) <= 0 && System.Data.Entity.DbFunctions.DiffMinutes(w.TenderStartDate, dateEnd) >= 0);
            }

            if (!string.IsNullOrEmpty(planInvestPriceStart) & !string.IsNullOrEmpty(planInvestPriceEnd))
            {
                var priceStart = Convert.ToDecimal(planInvestPriceStart);
                var priceEnd = Convert.ToDecimal(planInvestPriceEnd);
                result = result.Where(w => w.PlanInvestPrice >= priceStart && w.PlanInvestPrice <= priceEnd);
            }

            if (!User.IsInRole("领导查看"))
            {
                if (!User.IsInRole("组长查看"))
                {
                    if (User.IsInRole("招标管理"))
                    {
                        result = result.Where(w => w.ProjectResponsiblePersonID == userInfo.UserID);
                    }
                }
                else
                {
                    //查看本组的人员，包括自己
                    if (User.IsInRole("组长查看"))
                    {
                        List<int> personList = new List<int>();
                        personList.Add(userInfo.UserID);//添加自己

                        //添加组内成员
                        var memberList = db.GroupLeader.Where(w => w.LeaderUserID == userInfo.UserID).ToList();
                        foreach (var item in memberList)
                        {
                            personList.Add(item.MemberUserID);
                        }

                        result = result.Where(w => personList.Contains(w.ProjectResponsiblePersonID));
                    }
                }
            }

            var accountList = result.OrderBy(o => o.AccountID).ToList();
            #endregion

            #region 设置文件路径和样式
            var filename = "台账统计信息--" + accountType + App_Code.Commen.GetDateTimeString();
            string path = System.IO.Path.Combine(Server.MapPath("/"), "Template/ExportAccountProject.xls");
            Workbook workbook = new Workbook();
            workbook.Open(path);
            Cells cells = workbook.Worksheets[0].Cells;
            Worksheet ws = workbook.Worksheets[0];

            StyleFlag sf = new StyleFlag();
            sf.HorizontalAlignment = true;
            sf.VerticalAlignment = true;
            sf.WrapText = true;
            sf.Borders = true;

            Style style1 = workbook.Styles[workbook.Styles.Add()];//新增样式  
            style1.HorizontalAlignment = TextAlignmentType.Center;//文字居中 
            style1.VerticalAlignment = TextAlignmentType.Center;
            style1.IsTextWrapped = true;//单元格内容自动换行  
            style1.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; //应用边界线 左边界线  
            style1.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin; //应用边界线 右边界线  
            style1.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin; //应用边界线 上边界线  
            style1.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; //应用边界线 下边界线  
            #endregion

            int row = 2;//开始生成表格行数  
            foreach (var info in accountList)
            {
                var startmergepos = row;//开始合并的行位置，默认从row开始。
                List<int> countList = new List<int>();
                var childlist = (from c in db.AccountChild
                                 where c.AccountID == info.AccountID
                                 select c).ToList();

                #region 子表写入excel
                //工程、服务类，没有使用单位
                //工程、服务类，没有使用单位
                //工程、服务类，没有使用单位

                var firstList = childlist.Where(w => w.TableType == "first").ToList();//投标人
                var secondList = childlist.Where(w => w.TableType == "second").ToList();//评标委员会
                var thirdList = childlist.Where(w => w.TableType == "Third").ToList();//招标文件联审
                var fourList = childlist.Where(w => w.TableType == "Four").ToList();//澄清
                var fiveList = childlist.Where(w => w.TableType == "Five").ToList();//异议处理

                var connectList = childlist.Where(w => w.TableType == "Connect").ToList();//前期沟通记录
                var tenderSuccessList = childlist.Where(w => w.TableType == "TenderSuccess").ToList();//中标人信息

                countList.Add(firstList.Count);
                countList.Add(secondList.Count);
                countList.Add(thirdList.Count);
                countList.Add(fourList.Count);
                countList.Add(fiveList.Count);
                countList.Add(connectList.Count);
                countList.Add(tenderSuccessList.Count);

                //取子表中行数最多的列,如果子表为空，设置为1，设置为要合并的行数。
                var rowsorder = countList.Max() == 0 ? 1 : countList.Max();

                var firstRow = startmergepos;//投标人信息
                foreach (var item in firstList)
                {
                    cells[firstRow, 21].PutValue(item.TenderFilePlanPayPerson);
                    cells[firstRow, 21].SetStyle(style1);
                    cells[firstRow, 22].PutValue(item.TenderPerson);
                    cells[firstRow, 22].SetStyle(style1);
                    cells[firstRow, 23].PutValue(item.QuotedPriceSum);
                    cells[firstRow, 23].SetStyle(style1);
                    cells[firstRow, 24].PutValue(item.TenderPersonVersion);
                    cells[firstRow, 24].SetStyle(style1);
                    firstRow += 1;
                }
                //当子表数据小于最大行子表时，补齐子表剩余行的边框
                if (rowsorder > firstList.Count)
                {
                    var reduceCount = startmergepos + firstList.Count;
                    for (int i = 0; i < rowsorder - firstList.Count; i++)
                    {
                        cells[reduceCount + i, 21].SetStyle(style1);
                        cells[reduceCount + i, 22].SetStyle(style1);
                        cells[reduceCount + i, 23].SetStyle(style1);
                        cells[reduceCount + i, 24].SetStyle(style1);
                    }
                }

                var secondRow = startmergepos;//评标委员会
                foreach (var item in secondList)
                {
                    cells[secondRow, 34].PutValue(item.EvaluationPersonName);
                    cells[secondRow, 34].SetStyle(style1);
                    cells[secondRow, 35].PutValue(item.EvaluationPersonDeptName);
                    cells[secondRow, 35].SetStyle(style1);
                    cells[secondRow, 36].PutValue(item.IsEvaluationDirector);
                    cells[secondRow, 36].SetStyle(style1);
                    cells[secondRow, 37].PutValue(item.EvaluationTime);
                    cells[secondRow, 37].SetStyle(style1);
                    cells[secondRow, 38].PutValue(item.EvaluationCost);
                    cells[secondRow, 38].SetStyle(style1);
                    cells[secondRow, 39].PutValue(item.EvaluationVersion);
                    cells[secondRow, 39].SetStyle(style1);
                    secondRow += 1;
                }
                if (rowsorder > secondList.Count)
                {
                    var reduceCount = startmergepos + secondList.Count;
                    for (int i = 0; i < rowsorder - secondList.Count; i++)
                    {
                        cells[reduceCount + i, 34].SetStyle(style1);
                        cells[reduceCount + i, 35].SetStyle(style1);
                        cells[reduceCount + i, 36].SetStyle(style1);
                        cells[reduceCount + i, 37].SetStyle(style1);
                        cells[reduceCount + i, 38].SetStyle(style1);
                        cells[reduceCount + i, 39].SetStyle(style1);
                    }
                }

                var thirdRow = startmergepos;//招标文件联审
                foreach (var item in thirdList)
                {
                    cells[thirdRow, 40].PutValue(item.TenderFileAuditPersonName);
                    cells[thirdRow, 40].SetStyle(style1);
                    cells[thirdRow, 41].PutValue(item.TenderFileAuditPersonDeptName);
                    cells[thirdRow, 41].SetStyle(style1);
                    cells[thirdRow, 43].PutValue(item.TenderFileAuditCost);
                    cells[thirdRow, 43].SetStyle(style1);
                    thirdRow += 1;
                }
                if (rowsorder > thirdList.Count)
                {
                    var reduceCount = startmergepos + thirdList.Count;
                    for (int i = 0; i < rowsorder - thirdList.Count; i++)
                    {
                        cells[reduceCount + i, 40].SetStyle(style1);
                        cells[reduceCount + i, 41].SetStyle(style1);
                        cells[reduceCount + i, 43].SetStyle(style1);
                    }
                }

                var fourRow = startmergepos; //澄清
                foreach (var item in fourList)
                {
                    cells[fourRow, 45].PutValue(item.ClarifyLaunchPerson);
                    cells[fourRow, 45].SetStyle(style1);
                    cells[fourRow, 46].PutValue(item.ClarifyLaunchDate.Value.ToString("yyyy-MM-dd"));
                    cells[fourRow, 46].SetStyle(style1);
                    cells[fourRow, 47].PutValue(item.ClarifyReason);
                    cells[fourRow, 47].SetStyle(style1);
                    cells[fourRow, 48].PutValue(item.ClarifyAcceptDate.Value.ToString("yyyy-MM-dd"));
                    cells[fourRow, 48].SetStyle(style1);
                    cells[fourRow, 49].PutValue(item.ClarifyDisposePerson);
                    cells[fourRow, 49].SetStyle(style1);
                    cells[fourRow, 50].PutValue(item.IsClarify);
                    cells[fourRow, 50].SetStyle(style1);
                    cells[fourRow, 51].PutValue(item.ClarifyDisposeInfo);
                    cells[fourRow, 51].SetStyle(style1);
                    cells[fourRow, 52].PutValue(item.ClarifyReplyDate.Value.ToString("yyyy-MM-dd"));
                    cells[fourRow, 52].SetStyle(style1);
                    fourRow += 1;
                }
                if (rowsorder > fourList.Count)
                {
                    var reduceCount = startmergepos + fourList.Count;
                    for (int i = 0; i < rowsorder - fourList.Count; i++)
                    {
                        cells[reduceCount + i, 45].SetStyle(style1);
                        cells[reduceCount + i, 46].SetStyle(style1);
                        cells[reduceCount + i, 47].SetStyle(style1);
                        cells[reduceCount + i, 48].SetStyle(style1);
                        cells[reduceCount + i, 49].SetStyle(style1);
                        cells[reduceCount + i, 50].SetStyle(style1);
                        cells[reduceCount + i, 51].SetStyle(style1);
                        cells[reduceCount + i, 52].SetStyle(style1);
                    }
                }

                var fiveRow = startmergepos;//异议处理
                foreach (var item in fiveList)
                {
                    cells[fiveRow, 53].PutValue(item.DissentLaunchPerson);
                    cells[fiveRow, 53].SetStyle(style1);
                    cells[fiveRow, 54].PutValue(item.DissentLaunchPersonPhone);
                    cells[fiveRow, 54].SetStyle(style1);
                    cells[fiveRow, 55].PutValue(item.DissentLaunchDate.Value.ToString("yyyy-MM-dd"));
                    cells[fiveRow, 55].SetStyle(style1);
                    cells[fiveRow, 56].PutValue(item.DissentProposedStage);
                    cells[fiveRow, 56].SetStyle(style1);
                    cells[fiveRow, 57].PutValue(item.DissentReason);
                    cells[fiveRow, 57].SetStyle(style1);
                    cells[fiveRow, 58].PutValue(item.DissentAcceptDate.Value.ToString("yyyy-MM-dd"));
                    cells[fiveRow, 58].SetStyle(style1);
                    cells[fiveRow, 59].PutValue(item.DissentAcceptPerson);
                    cells[fiveRow, 59].SetStyle(style1);
                    cells[fiveRow, 60].PutValue(item.DissentDisposePerson);
                    cells[fiveRow, 60].SetStyle(style1);
                    cells[fiveRow, 61].PutValue(item.DissentDisposeInfo);
                    cells[fiveRow, 61].SetStyle(style1);
                    cells[fiveRow, 62].PutValue(item.DissentReplyDate.Value.ToString("yyyy-MM-dd"));
                    cells[fiveRow, 62].SetStyle(style1);
                    fiveRow += 1;
                }
                if (rowsorder > fiveList.Count)
                {
                    var reduceCount = startmergepos + fiveList.Count;
                    for (int i = 0; i < rowsorder - fiveList.Count; i++)
                    {
                        cells[reduceCount + i, 53].SetStyle(style1);
                        cells[reduceCount + i, 54].SetStyle(style1);
                        cells[reduceCount + i, 55].SetStyle(style1);
                        cells[reduceCount + i, 56].SetStyle(style1);
                        cells[reduceCount + i, 57].SetStyle(style1);
                        cells[reduceCount + i, 58].SetStyle(style1);
                        cells[reduceCount + i, 59].SetStyle(style1);
                        cells[reduceCount + i, 60].SetStyle(style1);
                        cells[reduceCount + i, 61].SetStyle(style1);
                        cells[reduceCount + i, 62].SetStyle(style1);
                    }
                }

                var connectRow = startmergepos;//前期沟通
                foreach (var item in connectList)
                {
                    cells[connectRow, 17].PutValue(item.ConnectPerson);
                    cells[connectRow, 17].SetStyle(style1);
                    cells[connectRow, 18].PutValue(item.ConnectDateTime.Value.ToString("yyyy-MM-dd"));
                    cells[connectRow, 18].SetStyle(style1);
                    cells[connectRow, 19].PutValue(item.ConnectContent);
                    cells[connectRow, 19].SetStyle(style1);
                    cells[connectRow, 20].PutValue(item.ConnectExistingProblems);
                    cells[connectRow, 20].SetStyle(style1);

                    connectRow += 1;
                }
                if (rowsorder > connectList.Count)
                {
                    var reduceCount = startmergepos + connectList.Count;
                    for (int i = 0; i < rowsorder - connectList.Count; i++)
                    {
                        cells[reduceCount + i, 17].SetStyle(style1);
                        cells[reduceCount + i, 18].SetStyle(style1);
                        cells[reduceCount + i, 19].SetStyle(style1);
                        cells[reduceCount + i, 20].SetStyle(style1);
                    }
                }

                var tenderSuccessRow = startmergepos;//中标人信息
                foreach (var item in tenderSuccessList)
                {
                    cells[tenderSuccessRow, 25].PutValue(item.TenderSuccessPerson);
                    cells[tenderSuccessRow, 25].SetStyle(style1);
                    cells[tenderSuccessRow, 26].PutValue(item.TenderSuccessPersonStartDate.Value.ToString("yyyy-MM-dd"));
                    cells[tenderSuccessRow, 26].SetStyle(style1);
                    cells[tenderSuccessRow, 27].PutValue(item.TenderSuccessPersonEndDate.Value.ToString("yyyy-MM-dd"));
                    cells[tenderSuccessRow, 27].SetStyle(style1);
                    cells[tenderSuccessRow, 28].PutValue(item.TenderSuccessPersonVersion);
                    cells[tenderSuccessRow, 28].SetStyle(style1);

                    tenderSuccessRow += 1;
                }
                if (rowsorder > tenderSuccessList.Count)
                {
                    var reduceCount = startmergepos + tenderSuccessList.Count;
                    for (int i = 0; i < rowsorder - tenderSuccessList.Count; i++)
                    {
                        cells[reduceCount + i, 25].SetStyle(style1);
                        cells[reduceCount + i, 26].SetStyle(style1);
                        cells[reduceCount + i, 27].SetStyle(style1);
                        cells[reduceCount + i, 28].SetStyle(style1);
                    }
                }

                //投标人信息
                if (firstList.Count == 0)
                {
                    Range range21 = ws.Cells.CreateRange(startmergepos, 21, rowsorder, 1);
                    range21.ApplyStyle(style1, sf);
                    range21.Merge();

                    Range range22 = ws.Cells.CreateRange(startmergepos, 22, rowsorder, 1);
                    range22.ApplyStyle(style1, sf);
                    range22.Merge();

                    Range range23 = ws.Cells.CreateRange(startmergepos, 23, rowsorder, 1);
                    range23.ApplyStyle(style1, sf);
                    range23.Merge();

                    Range range24 = ws.Cells.CreateRange(startmergepos, 24, rowsorder, 1);
                    range24.ApplyStyle(style1, sf);
                    range24.Merge();
                }

                //评标委员会
                if (secondList.Count == 0)
                {
                    Range range34 = ws.Cells.CreateRange(startmergepos, 34, rowsorder, 1);
                    range34.ApplyStyle(style1, sf);
                    range34.Merge();

                    Range range35 = ws.Cells.CreateRange(startmergepos, 35, rowsorder, 1);
                    range35.ApplyStyle(style1, sf);
                    range35.Merge();

                    Range range36 = ws.Cells.CreateRange(startmergepos, 36, rowsorder, 1);
                    range36.ApplyStyle(style1, sf);
                    range36.Merge();

                    Range range37 = ws.Cells.CreateRange(startmergepos, 37, rowsorder, 1);
                    range37.ApplyStyle(style1, sf);
                    range37.Merge();

                    Range range38 = ws.Cells.CreateRange(startmergepos, 38, rowsorder, 1);
                    range38.ApplyStyle(style1, sf);
                    range38.Merge();

                    Range range39 = ws.Cells.CreateRange(startmergepos, 39, rowsorder, 1);
                    range39.ApplyStyle(style1, sf);
                    range39.Merge();
                }

                //招标文件联审
                if (thirdList.Count == 0)
                {
                    Range range40 = ws.Cells.CreateRange(startmergepos, 40, rowsorder, 1);
                    range40.ApplyStyle(style1, sf);
                    range40.Merge();

                    Range range41 = ws.Cells.CreateRange(startmergepos, 41, rowsorder, 1);
                    range41.ApplyStyle(style1, sf);
                    range41.Merge();

                    Range range43 = ws.Cells.CreateRange(startmergepos, 43, rowsorder, 1);
                    range43.ApplyStyle(style1, sf);
                    range43.Merge();
                }

                //澄清
                if (fourList.Count == 0)
                {
                    Range range45 = ws.Cells.CreateRange(startmergepos, 45, rowsorder, 1);
                    range45.ApplyStyle(style1, sf);
                    range45.Merge();

                    Range range46 = ws.Cells.CreateRange(startmergepos, 46, rowsorder, 1);
                    range46.ApplyStyle(style1, sf);
                    range46.Merge();

                    Range range47 = ws.Cells.CreateRange(startmergepos, 47, rowsorder, 1);
                    range47.ApplyStyle(style1, sf);
                    range47.Merge();

                    Range range48 = ws.Cells.CreateRange(startmergepos, 48, rowsorder, 1);
                    range48.ApplyStyle(style1, sf);
                    range48.Merge();

                    Range range49 = ws.Cells.CreateRange(startmergepos, 49, rowsorder, 1);
                    range49.ApplyStyle(style1, sf);
                    range49.Merge();

                    Range range50 = ws.Cells.CreateRange(startmergepos, 50, rowsorder, 1);
                    range50.ApplyStyle(style1, sf);
                    range50.Merge();

                    Range range51 = ws.Cells.CreateRange(startmergepos, 51, rowsorder, 1);
                    range51.ApplyStyle(style1, sf);
                    range51.Merge();

                    Range range52 = ws.Cells.CreateRange(startmergepos, 52, rowsorder, 1);
                    range52.ApplyStyle(style1, sf);
                    range52.Merge();
                }

                //异议处理
                if (fiveList.Count == 0)
                {
                    Range range53 = ws.Cells.CreateRange(startmergepos, 53, rowsorder, 1);
                    range53.ApplyStyle(style1, sf);
                    range53.Merge();

                    Range range54 = ws.Cells.CreateRange(startmergepos, 54, rowsorder, 1);
                    range54.ApplyStyle(style1, sf);
                    range54.Merge();

                    Range range55 = ws.Cells.CreateRange(startmergepos, 55, rowsorder, 1);
                    range55.ApplyStyle(style1, sf);
                    range55.Merge();

                    Range range56 = ws.Cells.CreateRange(startmergepos, 56, rowsorder, 1);
                    range56.ApplyStyle(style1, sf);
                    range56.Merge();

                    Range range57 = ws.Cells.CreateRange(startmergepos, 57, rowsorder, 1);
                    range57.ApplyStyle(style1, sf);
                    range57.Merge();

                    Range range58 = ws.Cells.CreateRange(startmergepos, 58, rowsorder, 1);
                    range58.ApplyStyle(style1, sf);
                    range58.Merge();

                    Range range59 = ws.Cells.CreateRange(startmergepos, 59, rowsorder, 1);
                    range59.ApplyStyle(style1, sf);
                    range59.Merge();

                    Range range60 = ws.Cells.CreateRange(startmergepos, 60, rowsorder, 1);
                    range60.ApplyStyle(style1, sf);
                    range60.Merge();

                    Range range61 = ws.Cells.CreateRange(startmergepos, 61, rowsorder, 1);
                    range61.ApplyStyle(style1, sf);
                    range61.Merge();

                    Range range62 = ws.Cells.CreateRange(startmergepos, 62, rowsorder, 1);
                    range62.ApplyStyle(style1, sf);
                    range62.Merge();
                }

                //前期沟通
                if (connectList.Count == 0)
                {
                    Range range17 = ws.Cells.CreateRange(startmergepos, 17, rowsorder, 1);
                    range17.ApplyStyle(style1, sf);
                    range17.Merge();

                    Range range18 = ws.Cells.CreateRange(startmergepos, 18, rowsorder, 1);
                    range18.ApplyStyle(style1, sf);
                    range18.Merge();

                    Range range19 = ws.Cells.CreateRange(startmergepos, 19, rowsorder, 1);
                    range19.ApplyStyle(style1, sf);
                    range19.Merge();

                    Range range20 = ws.Cells.CreateRange(startmergepos, 20, rowsorder, 1);
                    range20.ApplyStyle(style1, sf);
                    range20.Merge();
                }

                //中标人信息
                if (tenderSuccessList.Count == 0)
                {
                    Range range25 = ws.Cells.CreateRange(startmergepos, 25, rowsorder, 1);
                    range25.ApplyStyle(style1, sf);
                    range25.Merge();

                    Range range26 = ws.Cells.CreateRange(startmergepos, 26, rowsorder, 1);
                    range26.ApplyStyle(style1, sf);
                    range26.Merge();

                    Range range27 = ws.Cells.CreateRange(startmergepos, 27, rowsorder, 1);
                    range27.ApplyStyle(style1, sf);
                    range27.Merge();

                    Range range28 = ws.Cells.CreateRange(startmergepos, 28, rowsorder, 1);
                    range28.ApplyStyle(style1, sf);
                    range28.Merge();
                }
                #endregion

                #region 非子表写入excel
                //序号
                cells[startmergepos, 0].PutValue(accountList.IndexOf(info) + 1);
                Range range0 = ws.Cells.CreateRange(startmergepos, 0, rowsorder, 1);
                range0.ApplyStyle(style1, sf);
                range0.Merge();

                cells[startmergepos, 1].PutValue(info.ProjectName);
                Range range1 = ws.Cells.CreateRange(startmergepos, 1, rowsorder, 1);
                range1.ApplyStyle(style1, sf);
                range1.Merge();

                cells[startmergepos, 2].PutValue(info.TenderFileNum);
                Range range2 = ws.Cells.CreateRange(startmergepos, 2, rowsorder, 1);
                range2.ApplyStyle(style1, sf);
                range2.Merge();

                cells[startmergepos, 3].PutValue(info.IsOnline);
                Range range3 = ws.Cells.CreateRange(startmergepos, 3, rowsorder, 1);
                range3.ApplyStyle(style1, sf);
                range3.Merge();

                cells[startmergepos, 4].PutValue(info.ProjectResponsiblePersonName);
                Range range4 = ws.Cells.CreateRange(startmergepos, 4, rowsorder, 1);
                range4.ApplyStyle(style1, sf);
                range4.Merge();

                cells[startmergepos, 5].PutValue(info.ProjectResponsibleDeptName);
                Range range5 = ws.Cells.CreateRange(startmergepos, 5, rowsorder, 1);
                range5.ApplyStyle(style1, sf);
                range5.Merge();

                cells[startmergepos, 6].PutValue(info.ApplyPerson);
                Range range6 = ws.Cells.CreateRange(startmergepos, 6, rowsorder, 1);
                range6.ApplyStyle(style1, sf);
                range6.Merge();

                cells[startmergepos, 7].PutValue(info.InvestPlanApproveNum);
                Range range7 = ws.Cells.CreateRange(startmergepos, 7, rowsorder, 1);
                range7.ApplyStyle(style1, sf);
                range7.Merge();

                cells[startmergepos, 8].PutValue(info.InvestSource);
                Range range8 = ws.Cells.CreateRange(startmergepos, 8, rowsorder, 1);
                range8.ApplyStyle(style1, sf);
                range8.Merge();

                cells[startmergepos, 9].PutValue(info.TenderRange);
                Range range9 = ws.Cells.CreateRange(startmergepos, 9, rowsorder, 1);
                range9.ApplyStyle(style1, sf);
                range9.Merge();

                cells[startmergepos, 10].PutValue(info.ProjectTimeLimit);
                Range range10 = ws.Cells.CreateRange(startmergepos, 10, rowsorder, 1);
                range10.ApplyStyle(style1, sf);
                range10.Merge();

                cells[startmergepos, 11].PutValue(info.ProgramAcceptDate == null ? "" : info.ProgramAcceptDate.Value.ToString("yyyy-MM-dd"));
                Range range11 = ws.Cells.CreateRange(startmergepos, 11, rowsorder, 1);
                range11.ApplyStyle(style1, sf);
                range11.Merge();

                cells[startmergepos, 12].PutValue(info.TenderProgramAuditDate == null ? "" : info.TenderProgramAuditDate.Value.ToString("yyyy-MM-dd"));
                Range range12 = ws.Cells.CreateRange(startmergepos, 12, rowsorder, 1);
                range12.ApplyStyle(style1, sf);
                range12.Merge();

                cells[startmergepos, 13].PutValue(info.TenderFileSaleStartDate == null ? "" : info.TenderFileSaleStartDate.Value.ToString("yyyy-MM-dd HH:mm"));
                Range range13 = ws.Cells.CreateRange(startmergepos, 13, rowsorder, 1);
                range13.ApplyStyle(style1, sf);
                range13.Merge();

                cells[startmergepos, 14].PutValue(info.TenderFileSaleEndDate == null ? "" : info.TenderFileSaleEndDate.Value.ToString("yyyy-MM-dd HH:mm"));
                Range range14 = ws.Cells.CreateRange(startmergepos, 14, rowsorder, 1);
                range14.ApplyStyle(style1, sf);
                range14.Merge();

                cells[startmergepos, 15].PutValue(info.TenderStartDate == null ? "" : info.TenderStartDate.Value.ToString("yyyy-MM-dd HH:mm"));
                Range range15 = ws.Cells.CreateRange(startmergepos, 15, rowsorder, 1);
                range15.ApplyStyle(style1, sf);
                range15.Merge();

                cells[startmergepos, 16].PutValue(info.TenderSuccessFileDate == null ? "" : info.TenderSuccessFileDate.Value.ToString("yyyy-MM-dd HH:mm"));
                Range range16 = ws.Cells.CreateRange(startmergepos, 16, rowsorder, 1);
                range16.ApplyStyle(style1, sf);
                range16.Merge();

                //cells[startmergepos, 19].PutValue(info.TenderSuccessPerson);
                //Range range19 = ws.Cells.CreateRange(startmergepos, 19, rowsorder, 1);
                //range19.ApplyStyle(style1, sf);
                //range19.Merge();

                cells[startmergepos, 29].PutValue(info.PlanInvestPrice);
                Range range29 = ws.Cells.CreateRange(startmergepos, 29, rowsorder, 1);
                range29.ApplyStyle(style1, sf);
                range29.Merge();

                cells[startmergepos, 30].PutValue(info.QualificationExamMethod);
                Range range30 = ws.Cells.CreateRange(startmergepos, 30, rowsorder, 1);
                range30.ApplyStyle(style1, sf);
                range30.Merge();

                cells[startmergepos, 31].PutValue(info.TenderRestrictSumPrice);
                Range range31 = ws.Cells.CreateRange(startmergepos, 31, rowsorder, 1);
                range31.ApplyStyle(style1, sf);
                range31.Merge();

                cells[startmergepos, 32].PutValue(info.TenderSuccessSumPrice);
                Range range32 = ws.Cells.CreateRange(startmergepos, 32, rowsorder, 1);
                range32.ApplyStyle(style1, sf);
                range32.Merge();

                cells[startmergepos, 33].PutValue(info.SaveCapital);
                Range range33 = ws.Cells.CreateRange(startmergepos, 33, rowsorder, 1);
                range33.ApplyStyle(style1, sf);
                range33.Merge();

                cells[startmergepos, 42].PutValue(info.TenderFileAuditTime);
                Range range42 = ws.Cells.CreateRange(startmergepos, 42, rowsorder, 1);
                range42.ApplyStyle(style1, sf);
                range42.Merge();

                cells[startmergepos, 44].PutValue(info.TenderFailReason);
                Range range44 = ws.Cells.CreateRange(startmergepos, 44, rowsorder, 1);
                range44.ApplyStyle(style1, sf);
                range44.Merge();

                //cells[startmergepos, 52].PutValue(info.ContractNum);
                //Range range52 = ws.Cells.CreateRange(startmergepos, 52, rowsorder, 1);
                //range52.ApplyStyle(style1, sf);
                //range52.Merge();

                //cells[startmergepos, 53].PutValue(info.ContractPrice);
                //Range range53 = ws.Cells.CreateRange(startmergepos, 53, rowsorder, 1);
                //range53.ApplyStyle(style1, sf);
                //range53.Merge();

                //cells[startmergepos, 54].PutValue(info.RelativePerson);
                //Range range54 = ws.Cells.CreateRange(startmergepos, 54, rowsorder, 1);
                //range54.ApplyStyle(style1, sf);
                //range54.Merge();

                cells[startmergepos, 63].PutValue(info.TenderInfo);
                Range range63 = ws.Cells.CreateRange(startmergepos, 63, rowsorder, 1);
                range63.ApplyStyle(style1, sf);
                range63.Merge();

                cells[startmergepos, 64].PutValue(info.TenderRemark);
                Range range64 = ws.Cells.CreateRange(startmergepos, 64, rowsorder, 1);
                range64.ApplyStyle(style1, sf);
                range64.Merge();
                #endregion

                //这是合并单元格后的行数，一定注意，要加上合并的行数
                row = startmergepos + rowsorder;
            }

            string fileToSave = System.IO.Path.Combine(Server.MapPath("/"), "ExcelOutPut/" + filename + ".xls");
            if (System.IO.File.Exists(fileToSave))
            {
                System.IO.File.Delete(fileToSave);
            }
            workbook.Save(fileToSave, FileFormatType.Excel97To2003);
            return File(fileToSave, "application/vnd.ms-excel", filename + ".xls");
        }
        #endregion
    }
}
