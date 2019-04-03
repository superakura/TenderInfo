using Aspose.Cells;
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
                    var accountChildFirst = db.AccountChild.Where(w => w.AccountID == item.AccountID && w.TableType == "first").ToList();
                    var accountChildSecond = db.AccountChild.Where(w => w.AccountID == item.AccountID && w.TableType == "second").ToList();
                    var accountChildThird = db.AccountChild.Where(w => w.AccountID == item.AccountID && w.TableType == "third").ToList();
                    var accountChildFour = db.AccountChild.Where(w => w.AccountID == item.AccountID && w.TableType == "four").ToList();
                    var accountChildFive = db.AccountChild.Where(w => w.AccountID == item.AccountID && w.TableType == "five").ToList();
                    viewList.AccountID = item.AccountID;
                    viewList.account = item;
                    viewList.accountChildFirst = accountChildFirst;
                    viewList.accountChildSecond = accountChildSecond;
                    viewList.accountChildThird = accountChildThird;
                    viewList.accountChildFour = accountChildFour;
                    viewList.accountChildFive = accountChildFive;
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
        #endregion

        #region CrudChild
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

                var userInfo = App_Code.Commen.GetUserFromSession();
                var info = db.AccountChild.Find(accountChildID);

                info.TenderFilePlanPayPerson = tenderFilePlanPayPersonEdit == string.Empty ? "-" : tenderFilePlanPayPersonEdit;
                info.TenderPerson = tenderPersonEdit == string.Empty ? "-" : tenderPersonEdit;
                info.ProductManufacturer = productManufacturerEdit == string.Empty ? "-" : productManufacturerEdit;
                info.QuotedPriceUnit = quotedPriceUnitEdit;
                info.QuotedPriceSum = quotedPriceSumEdit == string.Empty ? "-" : quotedPriceSumEdit;
                info.NegationExplain = negationExplain == string.Empty ? "-" : negationExplain;

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

        [HttpPost]
        public string InsertFour()
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

        [HttpPost]
        public string UpdateFour()
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

        [HttpPost]
        public string InsertFive()
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

        [HttpPost]
        public string UpdateFive()
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

        [HttpPost]
        public string DelEdit()
        {
            try
            {
                var accountChildID = 0;
                int.TryParse(Request.Form["tbxAccountChildID"], out accountChildID);
                var info = db.AccountChild.Find(accountChildID);
                db.AccountChild.Remove(info);
                db.SaveChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

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

        [HttpPost]
        public JsonResult GetListEdit()
        {
            try
            {
                var accountID = 0;
                int.TryParse(Request.Form["accountID"], out accountID);
                var type = Request.Form["type"];

                var info = db.AccountChild.Where(w => w.TableType == type && w.AccountID == accountID).ToList();
                return Json(info);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
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

                            info.QuotedPriceUnit = 0;
                            info.QuotedPriceSum = excel.Rows[i]["投标总价（万元）"].ToString().Trim() == "" ? "-" : excel.Rows[i]["投标总价（万元）"].ToString();

                            info.NegationExplain = excel.Rows[i]["初步评审是否被否决"].ToString().Trim() == "" ? "-" : excel.Rows[i]["初步评审是否被否决"].ToString();
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

            int row = 2;//开始生成表格行数  
            foreach (var info in accountList)
            {
                var startmergepos = row;//开始合并的行位置，默认从row开始。
                List<int> countList = new List<int>();
                var childlist = (from c in db.AccountChild
                                 where c.AccountID == info.AccountID
                                 select c).ToList();
                var firstList = childlist.Where(w => w.TableType == "first").ToList();
                var secondList = childlist.Where(w => w.TableType == "second").ToList();
                var thirdList = childlist.Where(w => w.TableType == "Third").ToList();
                var fourList = childlist.Where(w => w.TableType == "Four").ToList();
                var fiveList = childlist.Where(w => w.TableType == "Five").ToList();

                countList.Add(firstList.Count);
                countList.Add(secondList.Count);
                countList.Add(thirdList.Count);
                countList.Add(fourList.Count);
                countList.Add(fiveList.Count);

                //取子表中行数最多的列,如果子表为空，设置为1，设置为要合并的行数。
                var rowsorder = countList.Max() == 0 ? 1 : countList.Max();

                var firstRow = startmergepos;
                foreach (var item in firstList)
                {
                    cells[firstRow, 18].PutValue(item.TenderFilePlanPayPerson);
                    cells[firstRow, 18].SetStyle(style1);
                    cells[firstRow, 19].PutValue(item.TenderPerson);
                    cells[firstRow, 19].SetStyle(style1);
                    cells[firstRow, 20].PutValue(item.ProductManufacturer);
                    cells[firstRow, 20].SetStyle(style1);
                    cells[firstRow, 21].PutValue(item.QuotedPriceUnit);
                    cells[firstRow, 21].SetStyle(style1);
                    cells[firstRow, 22].PutValue(item.QuotedPriceSum);
                    cells[firstRow, 22].SetStyle(style1);
                    cells[firstRow, 23].PutValue(item.NegationExplain);
                    cells[firstRow, 23].SetStyle(style1);
                    firstRow += 1;
                }
                //当子表数据小于最大行子表时，补齐子表剩余行的边框
                if (rowsorder > firstList.Count)
                {
                    var reduceCount = startmergepos+firstList.Count;
                    for (int i = 0; i < rowsorder - firstList.Count; i++)
                    {
                        cells[reduceCount+i, 18].SetStyle(style1);
                        cells[reduceCount+i, 19].SetStyle(style1);
                        cells[reduceCount+i, 20].SetStyle(style1);
                        cells[reduceCount+i, 21].SetStyle(style1);
                        cells[reduceCount+i, 22].SetStyle(style1);
                        cells[reduceCount+i, 23].SetStyle(style1);
                    }
                }

                var secondRow = startmergepos;
                foreach (var item in secondList)
                {
                    cells[secondRow, 32].PutValue(item.EvaluationPersonName);
                    cells[secondRow, 32].SetStyle(style1);
                    cells[secondRow, 33].PutValue(item.EvaluationPersonDeptName);
                    cells[secondRow, 33].SetStyle(style1);
                    cells[secondRow, 34].PutValue(item.IsEvaluationDirector);
                    cells[secondRow, 34].SetStyle(style1);
                    cells[secondRow, 35].PutValue(item.EvaluationTime);
                    cells[secondRow, 35].SetStyle(style1);
                    cells[secondRow, 36].PutValue(item.EvaluationCost);
                    cells[secondRow, 36].SetStyle(style1);
                    secondRow += 1;
                }
                if (rowsorder > secondList.Count)
                {
                    var reduceCount = startmergepos+secondList.Count;
                    for (int i = 0; i <rowsorder-secondList.Count; i++)
                    {
                        cells[reduceCount + i, 32].SetStyle(style1);
                        cells[reduceCount + i, 33].SetStyle(style1);
                        cells[reduceCount + i, 34].SetStyle(style1);
                        cells[reduceCount + i, 35].SetStyle(style1);
                        cells[reduceCount + i, 36].SetStyle(style1);
                    }
                }

                var thirdRow = startmergepos;
                foreach (var item in thirdList)
                {
                    cells[thirdRow, 37].PutValue(item.TenderFileAuditPersonName);
                    cells[thirdRow, 37].SetStyle(style1);
                    cells[thirdRow, 38].PutValue(item.TenderFileAuditPersonDeptName);
                    cells[thirdRow, 38].SetStyle(style1);
                    cells[thirdRow, 40].PutValue(item.TenderFileAuditCost);
                    cells[thirdRow, 40].SetStyle(style1);
                    thirdRow += 1;
                }
                if (rowsorder > thirdList.Count)
                {
                    var reduceCount = startmergepos + thirdList.Count;
                    for (int i = 0; i < rowsorder - thirdList.Count; i++)
                    {
                        cells[reduceCount + i, 37].SetStyle(style1);
                        cells[reduceCount + i, 38].SetStyle(style1);
                        cells[reduceCount + i, 40].SetStyle(style1);
                    }
                }

                var fourRow = startmergepos;
                foreach (var item in fourList)
                {
                    cells[fourRow, 42].PutValue(item.ClarifyLaunchPerson);
                    cells[fourRow, 42].SetStyle(style1);
                    cells[fourRow, 43].PutValue(item.ClarifyLaunchDate.Value.ToString("yyyy-MM-dd"));
                    cells[fourRow, 43].SetStyle(style1);
                    cells[fourRow, 44].PutValue(item.ClarifyReason);
                    cells[fourRow, 44].SetStyle(style1);
                    cells[fourRow, 45].PutValue(item.ClarifyAcceptDate.Value.ToString("yyyy-MM-dd"));
                    cells[fourRow, 45].SetStyle(style1);
                    cells[fourRow, 46].PutValue(item.ClarifyDisposePerson);
                    cells[fourRow, 46].SetStyle(style1);
                    cells[fourRow, 47].PutValue(item.IsClarify);
                    cells[fourRow, 47].SetStyle(style1);
                    cells[fourRow, 48].PutValue(item.ClarifyDisposeInfo);
                    cells[fourRow, 48].SetStyle(style1);
                    cells[fourRow, 49].PutValue(item.ClarifyReplyDate==null?"":item.ClarifyReplyDate.Value.ToString("yyyy-MM-dd"));
                    cells[fourRow, 49].SetStyle(style1);
                    fourRow += 1;
                }
                if (rowsorder > fourList.Count)
                {
                    var reduceCount = startmergepos + fourList.Count;
                    for (int i = 0; i < rowsorder - fourList.Count; i++)
                    {
                        cells[reduceCount + i, 42].SetStyle(style1);
                        cells[reduceCount + i, 43].SetStyle(style1);
                        cells[reduceCount + i, 44].SetStyle(style1);
                        cells[reduceCount + i, 45].SetStyle(style1);
                        cells[reduceCount + i, 46].SetStyle(style1);
                        cells[reduceCount + i, 47].SetStyle(style1);
                        cells[reduceCount + i, 48].SetStyle(style1);
                        cells[reduceCount + i, 49].SetStyle(style1);
                    }
                }

                var fiveRow = startmergepos;
                foreach (var item in fiveList)
                {
                    cells[fiveRow, 50].PutValue(item.DissentLaunchPerson);
                    cells[fiveRow, 50].SetStyle(style1);
                    cells[fiveRow, 51].PutValue(item.DissentLaunchPersonPhone);
                    cells[fiveRow, 51].SetStyle(style1);
                    cells[fiveRow, 52].PutValue(item.DissentLaunchDate.Value.ToString("yyyy-MM-dd"));
                    cells[fiveRow, 52].SetStyle(style1);
                    cells[fiveRow, 53].PutValue(item.DissentReason);
                    cells[fiveRow, 53].SetStyle(style1);
                    cells[fiveRow, 54].PutValue(item.DissentAcceptDate.Value.ToString("yyyy-MM-dd"));
                    cells[fiveRow, 54].SetStyle(style1);
                    cells[fiveRow, 55].PutValue(item.DissentAcceptPerson);
                    cells[fiveRow, 55].SetStyle(style1);
                    cells[fiveRow, 56].PutValue(item.DissentDisposePerson);
                    cells[fiveRow, 56].SetStyle(style1);
                    cells[fiveRow, 57].PutValue(item.DissentDisposeInfo);
                    cells[fiveRow, 57].SetStyle(style1);
                    cells[fiveRow, 58].PutValue(item.DissentReplyDate.Value.ToString("yyyy-MM-dd"));
                    cells[fiveRow, 58].SetStyle(style1);
                    fiveRow += 1;
                }
                if (rowsorder > fiveList.Count)
                {
                    var reduceCount = startmergepos + fiveList.Count;
                    for (int i = 0; i < rowsorder - fiveList.Count; i++)
                    {
                        cells[reduceCount + i, 50].SetStyle(style1);
                        cells[reduceCount + i, 51].SetStyle(style1);
                        cells[reduceCount + i, 52].SetStyle(style1);
                        cells[reduceCount + i, 53].SetStyle(style1);
                        cells[reduceCount + i, 54].SetStyle(style1);
                        cells[reduceCount + i, 55].SetStyle(style1);
                        cells[reduceCount + i, 56].SetStyle(style1);
                        cells[reduceCount + i, 57].SetStyle(style1);
                        cells[reduceCount + i, 58].SetStyle(style1);
                    }
                }


                if (firstList.Count == 0)
                {
                    Range range18 = ws.Cells.CreateRange(startmergepos, 18, rowsorder, 1);
                    range18.ApplyStyle(style1, sf);
                    range18.Merge();

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

                    Range range23 = ws.Cells.CreateRange(startmergepos, 23, rowsorder, 1);
                    range23.ApplyStyle(style1, sf);
                    range23.Merge();
                }

                if (secondList.Count == 0)
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

                    Range range36 = ws.Cells.CreateRange(startmergepos, 36, rowsorder, 1);
                    range36.ApplyStyle(style1, sf);
                    range36.Merge();
                }

                if (thirdList.Count == 0)
                {
                    Range range37 = ws.Cells.CreateRange(startmergepos, 37, rowsorder, 1);
                    range37.ApplyStyle(style1, sf);
                    range37.Merge();

                    Range range38 = ws.Cells.CreateRange(startmergepos, 38, rowsorder, 1);
                    range38.ApplyStyle(style1, sf);
                    range38.Merge();

                    Range range40 = ws.Cells.CreateRange(startmergepos, 40, rowsorder, 1);
                    range40.ApplyStyle(style1, sf);
                    range40.Merge();
                }

                if (fourList.Count == 0)
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

                    Range range48 = ws.Cells.CreateRange(startmergepos, 48, rowsorder, 1);
                    range48.ApplyStyle(style1, sf);
                    range48.Merge();

                    Range range49 = ws.Cells.CreateRange(startmergepos, 49, rowsorder, 1);
                    range49.ApplyStyle(style1, sf);
                    range49.Merge();
                }

                if (fiveList.Count == 0)
                {
                    Range range50 = ws.Cells.CreateRange(startmergepos, 50, rowsorder, 1);
                    range50.ApplyStyle(style1, sf);
                    range50.Merge();

                    Range range51 = ws.Cells.CreateRange(startmergepos, 51, rowsorder, 1);
                    range51.ApplyStyle(style1, sf);
                    range51.Merge();

                    Range range52 = ws.Cells.CreateRange(startmergepos, 52, rowsorder, 1);
                    range52.ApplyStyle(style1, sf);
                    range52.Merge();

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
                }
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

                cells[startmergepos, 5].PutValue(info.UsingDeptName);
                Range range5 = ws.Cells.CreateRange(startmergepos, 5, rowsorder, 1);
                range5.ApplyStyle(style1, sf);
                range5.Merge();

                cells[startmergepos, 6].PutValue(info.ProjectResponsibleDeptName);
                Range range6 = ws.Cells.CreateRange(startmergepos, 6, rowsorder, 1);
                range6.ApplyStyle(style1, sf);
                range6.Merge();

                cells[startmergepos, 7].PutValue(info.ApplyPerson);
                Range range7 = ws.Cells.CreateRange(startmergepos, 7, rowsorder, 1);
                range7.ApplyStyle(style1, sf);
                range7.Merge();

                cells[startmergepos, 8].PutValue(info.InvestPlanApproveNum);
                Range range8 = ws.Cells.CreateRange(startmergepos, 8, rowsorder, 1);
                range8.ApplyStyle(style1, sf);
                range8.Merge();

                cells[startmergepos, 9].PutValue(info.TenderRange);
                Range range9 = ws.Cells.CreateRange(startmergepos, 9, rowsorder, 1);
                range9.ApplyStyle(style1, sf);
                range9.Merge();

                cells[startmergepos, 10].PutValue(info.TenderMode);
                Range range10 = ws.Cells.CreateRange(startmergepos, 10, rowsorder, 1);
                range10.ApplyStyle(style1, sf);
                range10.Merge();

                cells[startmergepos, 11].PutValue(info.BidEvaluation);
                Range range11 = ws.Cells.CreateRange(startmergepos, 11, rowsorder, 1);
                range11.ApplyStyle(style1, sf);
                range11.Merge();

                cells[startmergepos, 12].PutValue(info.SupplyPeriod);
                Range range12 = ws.Cells.CreateRange(startmergepos, 12, rowsorder, 1);
                range12.ApplyStyle(style1, sf);
                range12.Merge();

                cells[startmergepos, 13].PutValue(info.TenderProgramAuditDate == null ? "" : info.TenderProgramAuditDate.Value.ToString("yyyy-MM-dd"));
                Range range13 = ws.Cells.CreateRange(startmergepos, 13, rowsorder, 1);
                range13.ApplyStyle(style1, sf);
                range13.Merge();

                cells[startmergepos, 14].PutValue(info.ProgramAcceptDate == null ? "" : info.ProgramAcceptDate.Value.ToString("yyyy-MM-dd"));
                Range range14 = ws.Cells.CreateRange(startmergepos, 14, rowsorder, 1);
                range14.ApplyStyle(style1, sf);
                range14.Merge();

                cells[startmergepos, 15].PutValue(info.TenderFileSaleStartDate == null ? "" : info.TenderFileSaleStartDate.Value.ToString("yyyy-MM-dd hh:mm"));
                Range range15 = ws.Cells.CreateRange(startmergepos, 15, rowsorder, 1);
                range15.ApplyStyle(style1, sf);
                range15.Merge();

                cells[startmergepos, 16].PutValue(info.TenderFileSaleEndDate == null ? "" : info.TenderFileSaleEndDate.Value.ToString("yyyy-MM-dd hh:mm"));
                Range range16 = ws.Cells.CreateRange(startmergepos, 16, rowsorder, 1);
                range16.ApplyStyle(style1, sf);
                range16.Merge();

                cells[startmergepos, 17].PutValue(info.TenderStartDate == null ? "" : info.TenderStartDate.Value.ToString("yyyy-MM-dd hh:mm"));
                Range range17 = ws.Cells.CreateRange(startmergepos, 17, rowsorder, 1);
                range17.ApplyStyle(style1, sf);
                range17.Merge();

                cells[startmergepos, 24].PutValue(info.TenderSuccessPerson);
                Range range24 = ws.Cells.CreateRange(startmergepos, 24, rowsorder, 1);
                range24.ApplyStyle(style1, sf);
                range24.Merge();

                cells[startmergepos, 25].PutValue(info.PlanInvestPrice);
                Range range25 = ws.Cells.CreateRange(startmergepos, 25, rowsorder, 1);
                range25.ApplyStyle(style1, sf);
                range25.Merge();

                cells[startmergepos, 26].PutValue(info.QualificationExamMethod);
                Range range26 = ws.Cells.CreateRange(startmergepos, 26, rowsorder, 1);
                range26.ApplyStyle(style1, sf);
                range26.Merge();

                cells[startmergepos, 27].PutValue(info.TenderRestrictUnitPrice);
                Range range27 = ws.Cells.CreateRange(startmergepos, 27, rowsorder, 1);
                range27.ApplyStyle(style1, sf);
                range27.Merge();

                cells[startmergepos, 28].PutValue(info.TenderRestrictSumPrice);
                Range range28 = ws.Cells.CreateRange(startmergepos, 28, rowsorder, 1);
                range28.ApplyStyle(style1, sf);
                range28.Merge();

                cells[startmergepos, 29].PutValue(info.TenderSuccessUnitPrice);
                Range range29 = ws.Cells.CreateRange(startmergepos, 29, rowsorder, 1);
                range29.ApplyStyle(style1, sf);
                range29.Merge();

                cells[startmergepos, 30].PutValue(info.TenderSuccessSumPrice);
                Range range30 = ws.Cells.CreateRange(startmergepos, 30, rowsorder, 1);
                range30.ApplyStyle(style1, sf);
                range30.Merge();

                cells[startmergepos, 31].PutValue(info.SaveCapital);
                Range range31 = ws.Cells.CreateRange(startmergepos, 31, rowsorder, 1);
                range31.ApplyStyle(style1, sf);
                range31.Merge();

                cells[startmergepos, 39].PutValue(info.TenderFileAuditTime);
                Range range39 = ws.Cells.CreateRange(startmergepos, 39, rowsorder, 1);
                range39.ApplyStyle(style1, sf);
                range39.Merge();

                cells[startmergepos, 41].PutValue(info.TenderFailReason);
                Range range41 = ws.Cells.CreateRange(startmergepos, 41, rowsorder, 1);
                range41.ApplyStyle(style1, sf);
                range41.Merge();

                cells[startmergepos, 59].PutValue(info.ContractNum);
                Range range59 = ws.Cells.CreateRange(startmergepos, 59, rowsorder, 1);
                range59.ApplyStyle(style1, sf);
                range59.Merge();

                cells[startmergepos, 60].PutValue(info.ContractPrice);
                Range range60 = ws.Cells.CreateRange(startmergepos, 60, rowsorder, 1);
                range60.ApplyStyle(style1, sf);
                range60.Merge();

                cells[startmergepos, 61].PutValue(info.RelativePerson);
                Range range61 = ws.Cells.CreateRange(startmergepos, 61, rowsorder, 1);
                range61.ApplyStyle(style1, sf);
                range61.Merge();

                cells[startmergepos, 62].PutValue(info.TenderInfo);
                Range range62 = ws.Cells.CreateRange(startmergepos, 62, rowsorder, 1);
                range62.ApplyStyle(style1, sf);
                range62.Merge();

                cells[startmergepos, 63].PutValue(info.TenderRemark);
                Range range63 = ws.Cells.CreateRange(startmergepos, 63, rowsorder, 1);
                range63.ApplyStyle(style1, sf);
                range63.Merge();

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

            var filename = "台账统计信息" + accountType + App_Code.Commen.GetDateTimeString();

            string path = System.IO.Path.Combine(Server.MapPath("/"), "Template/ExportAccountFrame.xls");
            Workbook workbook = new Workbook();
            workbook.Open(path);
            Cells cells = workbook.Worksheets[0].Cells;
            Worksheet ws = workbook.Worksheets[0];

            #region 表格样式

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

                #region 生成合并子表
                var firstList = childlist.Where(w => w.TableType == "first").ToList();
                var secondList = childlist.Where(w => w.TableType == "second").ToList();
                var thirdList = childlist.Where(w => w.TableType == "Third").ToList();
                var fourList = childlist.Where(w => w.TableType == "Four").ToList();
                var fiveList = childlist.Where(w => w.TableType == "Five").ToList();

                countList.Add(firstList.Count);
                countList.Add(secondList.Count);
                countList.Add(thirdList.Count);
                countList.Add(fourList.Count);
                countList.Add(fiveList.Count);

                //取子表中行数最多的列,如果子表为空，设置为1，设置为要合并的行数。
                var rowsorder = countList.Max() == 0 ? 1 : countList.Max();

                var firstRow = startmergepos;
                foreach (var item in firstList)
                {
                    cells[firstRow, 19].PutValue(item.TenderFilePlanPayPerson);
                    cells[firstRow, 19].SetStyle(style1);
                    cells[firstRow, 20].PutValue(item.TenderPerson);
                    cells[firstRow, 20].SetStyle(style1);
                    cells[firstRow, 21].PutValue(item.ProductManufacturer);
                    cells[firstRow, 21].SetStyle(style1);
                    cells[firstRow, 22].PutValue(item.QuotedPriceSum);
                    cells[firstRow, 22].SetStyle(style1);
                    cells[firstRow, 23].PutValue(item.NegationExplain);
                    cells[firstRow, 23].SetStyle(style1);
                    firstRow += 1;
                }
                //当子表数据小于最大行子表时，补齐子表剩余行的边框
                if (rowsorder > firstList.Count)
                {
                    var reduceCount = startmergepos + firstList.Count;
                    for (int i = 0; i < rowsorder - firstList.Count; i++)
                    {
                        cells[reduceCount + i, 19].SetStyle(style1);
                        cells[reduceCount + i, 20].SetStyle(style1);
                        cells[reduceCount + i, 21].SetStyle(style1);
                        cells[reduceCount + i, 22].SetStyle(style1);
                        cells[reduceCount + i, 23].SetStyle(style1);
                    }
                }

                var secondRow = startmergepos;
                foreach (var item in secondList)
                {
                    cells[secondRow, 32].PutValue(item.EvaluationPersonName);
                    cells[secondRow, 32].SetStyle(style1);
                    cells[secondRow, 33].PutValue(item.EvaluationPersonDeptName);
                    cells[secondRow, 33].SetStyle(style1);
                    cells[secondRow, 34].PutValue(item.IsEvaluationDirector);
                    cells[secondRow, 34].SetStyle(style1);
                    cells[secondRow, 35].PutValue(item.EvaluationTime);
                    cells[secondRow, 35].SetStyle(style1);
                    cells[secondRow, 36].PutValue(item.EvaluationCost);
                    cells[secondRow, 36].SetStyle(style1);
                    secondRow += 1;
                }
                if (rowsorder > secondList.Count)
                {
                    var reduceCount = startmergepos + secondList.Count;
                    for (int i = 0; i < rowsorder - secondList.Count; i++)
                    {
                        cells[reduceCount + i, 32].SetStyle(style1);
                        cells[reduceCount + i, 33].SetStyle(style1);
                        cells[reduceCount + i, 34].SetStyle(style1);
                        cells[reduceCount + i, 35].SetStyle(style1);
                        cells[reduceCount + i, 36].SetStyle(style1);
                    }
                }

                var thirdRow = startmergepos;
                foreach (var item in thirdList)
                {
                    cells[thirdRow, 37].PutValue(item.TenderFileAuditPersonName);
                    cells[thirdRow, 37].SetStyle(style1);
                    cells[thirdRow, 38].PutValue(item.TenderFileAuditPersonDeptName);
                    cells[thirdRow, 38].SetStyle(style1);
                    cells[thirdRow, 40].PutValue(item.TenderFileAuditCost);
                    cells[thirdRow, 40].SetStyle(style1);
                    thirdRow += 1;
                }
                if (rowsorder > thirdList.Count)
                {
                    var reduceCount = startmergepos + thirdList.Count;
                    for (int i = 0; i < rowsorder - thirdList.Count; i++)
                    {
                        cells[reduceCount + i, 37].SetStyle(style1);
                        cells[reduceCount + i, 38].SetStyle(style1);
                        cells[reduceCount + i, 40].SetStyle(style1);
                    }
                }

                var fourRow = startmergepos;
                foreach (var item in fourList)
                {
                    cells[fourRow, 42].PutValue(item.ClarifyLaunchPerson);
                    cells[fourRow, 42].SetStyle(style1);
                    cells[fourRow, 43].PutValue(item.ClarifyLaunchDate.Value.ToString("yyyy-MM-dd"));
                    cells[fourRow, 43].SetStyle(style1);
                    cells[fourRow, 44].PutValue(item.ClarifyReason);
                    cells[fourRow, 44].SetStyle(style1);
                    cells[fourRow, 45].PutValue(item.ClarifyAcceptDate.Value.ToString("yyyy-MM-dd"));
                    cells[fourRow, 45].SetStyle(style1);
                    cells[fourRow, 46].PutValue(item.ClarifyDisposePerson);
                    cells[fourRow, 46].SetStyle(style1);
                    cells[fourRow, 47].PutValue(item.IsClarify);
                    cells[fourRow, 47].SetStyle(style1);
                    cells[fourRow, 48].PutValue(item.ClarifyDisposeInfo);
                    cells[fourRow, 48].SetStyle(style1);
                    cells[fourRow, 49].PutValue(item.ClarifyReplyDate.Value.ToString("yyyy-MM-dd"));
                    cells[fourRow, 49].SetStyle(style1);
                    fourRow += 1;
                }
                if (rowsorder > fourList.Count)
                {
                    var reduceCount = startmergepos + fourList.Count;
                    for (int i = 0; i < rowsorder - fourList.Count; i++)
                    {
                        cells[reduceCount + i, 42].SetStyle(style1);
                        cells[reduceCount + i, 43].SetStyle(style1);
                        cells[reduceCount + i, 44].SetStyle(style1);
                        cells[reduceCount + i, 45].SetStyle(style1);
                        cells[reduceCount + i, 46].SetStyle(style1);
                        cells[reduceCount + i, 47].SetStyle(style1);
                        cells[reduceCount + i, 48].SetStyle(style1);
                        cells[reduceCount + i, 49].SetStyle(style1);
                    }
                }

                var fiveRow = startmergepos;
                foreach (var item in fiveList)
                {
                    cells[fiveRow, 50].PutValue(item.DissentLaunchPerson);
                    cells[fiveRow, 50].SetStyle(style1);
                    cells[fiveRow, 51].PutValue(item.DissentLaunchPersonPhone);
                    cells[fiveRow, 51].SetStyle(style1);
                    cells[fiveRow, 52].PutValue(item.DissentLaunchDate.Value.ToString("yyyy-MM-dd"));
                    cells[fiveRow, 52].SetStyle(style1);
                    cells[fiveRow, 53].PutValue(item.DissentReason);
                    cells[fiveRow, 53].SetStyle(style1);
                    cells[fiveRow, 54].PutValue(item.DissentAcceptDate.Value.ToString("yyyy-MM-dd"));
                    cells[fiveRow, 54].SetStyle(style1);
                    cells[fiveRow, 55].PutValue(item.DissentAcceptPerson);
                    cells[fiveRow, 55].SetStyle(style1);
                    cells[fiveRow, 56].PutValue(item.DissentDisposePerson);
                    cells[fiveRow, 56].SetStyle(style1);
                    cells[fiveRow, 57].PutValue(item.DissentDisposeInfo);
                    cells[fiveRow, 57].SetStyle(style1);
                    cells[fiveRow, 58].PutValue(item.DissentReplyDate.Value.ToString("yyyy-MM-dd"));
                    cells[fiveRow, 58].SetStyle(style1);
                    fiveRow += 1;
                }
                if (rowsorder > fiveList.Count)
                {
                    var reduceCount = startmergepos + fiveList.Count;
                    for (int i = 0; i < rowsorder - fiveList.Count; i++)
                    {
                        cells[reduceCount + i, 50].SetStyle(style1);
                        cells[reduceCount + i, 51].SetStyle(style1);
                        cells[reduceCount + i, 52].SetStyle(style1);
                        cells[reduceCount + i, 53].SetStyle(style1);
                        cells[reduceCount + i, 54].SetStyle(style1);
                        cells[reduceCount + i, 55].SetStyle(style1);
                        cells[reduceCount + i, 56].SetStyle(style1);
                        cells[reduceCount + i, 57].SetStyle(style1);
                        cells[reduceCount + i, 58].SetStyle(style1);
                    }
                }

                if (firstList.Count == 0)
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

                    Range range23 = ws.Cells.CreateRange(startmergepos, 23, rowsorder, 1);
                    range23.ApplyStyle(style1, sf);
                    range23.Merge();
                }

                if (secondList.Count == 0)
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

                    Range range36 = ws.Cells.CreateRange(startmergepos, 36, rowsorder, 1);
                    range36.ApplyStyle(style1, sf);
                    range36.Merge();
                }

                if (thirdList.Count == 0)
                {
                    Range range37 = ws.Cells.CreateRange(startmergepos, 37, rowsorder, 1);
                    range37.ApplyStyle(style1, sf);
                    range37.Merge();

                    Range range38 = ws.Cells.CreateRange(startmergepos, 38, rowsorder, 1);
                    range38.ApplyStyle(style1, sf);
                    range38.Merge();

                    Range range40 = ws.Cells.CreateRange(startmergepos, 40, rowsorder, 1);
                    range40.ApplyStyle(style1, sf);
                    range40.Merge();
                }

                if (fourList.Count == 0)
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

                    Range range48 = ws.Cells.CreateRange(startmergepos, 48, rowsorder, 1);
                    range48.ApplyStyle(style1, sf);
                    range48.Merge();

                    Range range49 = ws.Cells.CreateRange(startmergepos, 49, rowsorder, 1);
                    range49.ApplyStyle(style1, sf);
                    range49.Merge();
                }

                if (fiveList.Count == 0)
                {
                    Range range50 = ws.Cells.CreateRange(startmergepos, 50, rowsorder, 1);
                    range50.ApplyStyle(style1, sf);
                    range50.Merge();

                    Range range51 = ws.Cells.CreateRange(startmergepos, 51, rowsorder, 1);
                    range51.ApplyStyle(style1, sf);
                    range51.Merge();

                    Range range52 = ws.Cells.CreateRange(startmergepos, 52, rowsorder, 1);
                    range52.ApplyStyle(style1, sf);
                    range52.Merge();

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
                }
                #endregion
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

                cells[startmergepos, 5].PutValue(info.UsingDeptName);
                Range range5 = ws.Cells.CreateRange(startmergepos, 5, rowsorder, 1);
                range5.ApplyStyle(style1, sf);
                range5.Merge();

                cells[startmergepos, 6].PutValue(info.ProjectResponsibleDeptName);
                Range range6 = ws.Cells.CreateRange(startmergepos, 6, rowsorder, 1);
                range6.ApplyStyle(style1, sf);
                range6.Merge();

                cells[startmergepos, 7].PutValue(info.ApplyPerson);
                Range range7 = ws.Cells.CreateRange(startmergepos, 7, rowsorder, 1);
                range7.ApplyStyle(style1, sf);
                range7.Merge();

                cells[startmergepos, 8].PutValue(info.InvestPlanApproveNum);
                Range range8 = ws.Cells.CreateRange(startmergepos, 8, rowsorder, 1);
                range8.ApplyStyle(style1, sf);
                range8.Merge();

                cells[startmergepos, 9].PutValue(info.TenderRange);
                Range range9 = ws.Cells.CreateRange(startmergepos, 9, rowsorder, 1);
                range9.ApplyStyle(style1, sf);
                range9.Merge();

                cells[startmergepos, 10].PutValue(info.IsHaveCount);
                Range range10 = ws.Cells.CreateRange(startmergepos, 10, rowsorder, 1);
                range10.ApplyStyle(style1, sf);
                range10.Merge();

                cells[startmergepos, 11].PutValue(info.TenderMode);
                Range range11 = ws.Cells.CreateRange(startmergepos, 11, rowsorder, 1);
                range11.ApplyStyle(style1, sf);
                range11.Merge();

                cells[startmergepos, 12].PutValue(info.BidEvaluation);
                Range range12 = ws.Cells.CreateRange(startmergepos, 12, rowsorder, 1);
                range12.ApplyStyle(style1, sf);
                range12.Merge();

                cells[startmergepos, 13].PutValue(info.SupplyPeriod);
                Range range13 = ws.Cells.CreateRange(startmergepos, 13, rowsorder, 1);
                range13.ApplyStyle(style1, sf);
                range13.Merge();

                cells[startmergepos, 14].PutValue(info.ProgramAcceptDate == null ? "" : info.ProgramAcceptDate.Value.ToString("yyyy-MM-dd"));
                Range range14 = ws.Cells.CreateRange(startmergepos, 14, rowsorder, 1);
                range14.ApplyStyle(style1, sf);
                range14.Merge();

                cells[startmergepos, 15].PutValue(info.TenderProgramAuditDate == null ? "" : info.TenderProgramAuditDate.Value.ToString("yyyy-MM-dd"));
                Range range15 = ws.Cells.CreateRange(startmergepos, 15, rowsorder, 1);
                range15.ApplyStyle(style1, sf);
                range15.Merge();

                cells[startmergepos, 16].PutValue(info.TenderFileSaleStartDate == null ? "" : info.TenderFileSaleStartDate.Value.ToString("yyyy-MM-dd hh:mm"));
                Range range16 = ws.Cells.CreateRange(startmergepos, 16, rowsorder, 1);
                range16.ApplyStyle(style1, sf);
                range16.Merge();

                cells[startmergepos, 17].PutValue(info.TenderFileSaleEndDate == null ? "" : info.TenderFileSaleEndDate.Value.ToString("yyyy-MM-dd hh:mm"));
                Range range17 = ws.Cells.CreateRange(startmergepos, 17, rowsorder, 1);
                range17.ApplyStyle(style1, sf);
                range17.Merge();

                cells[startmergepos, 18].PutValue(info.TenderStartDate == null ? "" : info.TenderStartDate.Value.ToString("yyyy-MM-dd hh:mm"));
                Range range18 = ws.Cells.CreateRange(startmergepos, 18, rowsorder, 1);
                range18.ApplyStyle(style1, sf);
                range18.Merge();

                cells[startmergepos, 24].PutValue(info.TenderSuccessPerson);
                Range range24 = ws.Cells.CreateRange(startmergepos, 24, rowsorder, 1);
                range24.ApplyStyle(style1, sf);
                range24.Merge();

                cells[startmergepos, 25].PutValue(info.PlanInvestPrice);
                Range range25 = ws.Cells.CreateRange(startmergepos, 25, rowsorder, 1);
                range25.ApplyStyle(style1, sf);
                range25.Merge();

                cells[startmergepos, 26].PutValue(info.QualificationExamMethod);
                Range range26 = ws.Cells.CreateRange(startmergepos, 26, rowsorder, 1);
                range26.ApplyStyle(style1, sf);
                range26.Merge();

                cells[startmergepos, 27].PutValue(info.TenderRestrictUnitPrice);
                Range range27 = ws.Cells.CreateRange(startmergepos, 27, rowsorder, 1);
                range27.ApplyStyle(style1, sf);
                range27.Merge();

                cells[startmergepos, 28].PutValue(info.TenderRestrictSumPrice);
                Range range28 = ws.Cells.CreateRange(startmergepos, 28, rowsorder, 1);
                range28.ApplyStyle(style1, sf);
                range28.Merge();

                cells[startmergepos, 29].PutValue(info.TenderSuccessUnitPrice);
                Range range29 = ws.Cells.CreateRange(startmergepos, 29, rowsorder, 1);
                range29.ApplyStyle(style1, sf);
                range29.Merge();

                cells[startmergepos, 30].PutValue(info.TenderSuccessSumPrice);
                Range range30 = ws.Cells.CreateRange(startmergepos, 30, rowsorder, 1);
                range30.ApplyStyle(style1, sf);
                range30.Merge();

                cells[startmergepos, 31].PutValue(info.SaveCapital);
                Range range31 = ws.Cells.CreateRange(startmergepos, 31, rowsorder, 1);
                range31.ApplyStyle(style1, sf);
                range31.Merge();

                cells[startmergepos, 39].PutValue(info.TenderFileAuditTime);
                Range range39 = ws.Cells.CreateRange(startmergepos, 39, rowsorder, 1);
                range39.ApplyStyle(style1, sf);
                range39.Merge();

                cells[startmergepos, 41].PutValue(info.TenderFailReason);
                Range range41 = ws.Cells.CreateRange(startmergepos, 41, rowsorder, 1);
                range41.ApplyStyle(style1, sf);
                range41.Merge();

                cells[startmergepos, 59].PutValue(info.ContractNum);
                Range range59 = ws.Cells.CreateRange(startmergepos, 59, rowsorder, 1);
                range59.ApplyStyle(style1, sf);
                range59.Merge();

                cells[startmergepos, 60].PutValue(info.ContractPrice);
                Range range60 = ws.Cells.CreateRange(startmergepos, 60, rowsorder, 1);
                range60.ApplyStyle(style1, sf);
                range60.Merge();

                cells[startmergepos, 61].PutValue(info.RelativePerson);
                Range range61 = ws.Cells.CreateRange(startmergepos, 61, rowsorder, 1);
                range61.ApplyStyle(style1, sf);
                range61.Merge();

                cells[startmergepos, 62].PutValue(info.TenderInfo);
                Range range62 = ws.Cells.CreateRange(startmergepos, 62, rowsorder, 1);
                range62.ApplyStyle(style1, sf);
                range62.Merge();

                cells[startmergepos, 63].PutValue(info.TenderRemark);
                Range range63 = ws.Cells.CreateRange(startmergepos, 63, rowsorder, 1);
                range63.ApplyStyle(style1, sf);
                range63.Merge();

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

            var filename = "台账统计信息--" + accountType + App_Code.Commen.GetDateTimeString();

            string path = System.IO.Path.Combine(Server.MapPath("/"), "Template/ExportAccountProject.xls");
            Workbook workbook = new Workbook();
            workbook.Open(path);
            Cells cells = workbook.Worksheets[0].Cells;
            Worksheet ws = workbook.Worksheets[0];

            #region 表格样式

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

                #region 生成合并子表
                var firstList = childlist.Where(w => w.TableType == "first").ToList();
                var secondList = childlist.Where(w => w.TableType == "second").ToList();
                var thirdList = childlist.Where(w => w.TableType == "Third").ToList();
                var fourList = childlist.Where(w => w.TableType == "Four").ToList();
                var fiveList = childlist.Where(w => w.TableType == "Five").ToList();

                countList.Add(firstList.Count);
                countList.Add(secondList.Count);
                countList.Add(thirdList.Count);
                countList.Add(fourList.Count);
                countList.Add(fiveList.Count);

                //取子表中行数最多的列,如果子表为空，设置为1，设置为要合并的行数。
                var rowsorder = countList.Max() == 0 ? 1 : countList.Max();

                var firstRow = startmergepos;
                foreach (var item in firstList)
                {
                    cells[firstRow, 16].PutValue(item.TenderFilePlanPayPerson);
                    cells[firstRow, 16].SetStyle(style1);
                    cells[firstRow, 17].PutValue(item.TenderPerson);
                    cells[firstRow, 17].SetStyle(style1);
                    cells[firstRow, 18].PutValue(item.QuotedPriceSum);
                    cells[firstRow, 18].SetStyle(style1);
                    firstRow += 1;
                }
                //当子表数据小于最大行子表时，补齐子表剩余行的边框
                if (rowsorder > firstList.Count)
                {
                    var reduceCount = startmergepos + firstList.Count;
                    for (int i = 0; i < rowsorder - firstList.Count; i++)
                    {
                        cells[reduceCount + i, 16].SetStyle(style1);
                        cells[reduceCount + i, 17].SetStyle(style1);
                        cells[reduceCount + i, 18].SetStyle(style1);
                    }
                }

                var secondRow = startmergepos;
                foreach (var item in secondList)
                {
                    cells[secondRow, 25].PutValue(item.EvaluationPersonName);
                    cells[secondRow, 25].SetStyle(style1);
                    cells[secondRow, 26].PutValue(item.EvaluationPersonDeptName);
                    cells[secondRow, 26].SetStyle(style1);
                    cells[secondRow, 27].PutValue(item.IsEvaluationDirector);
                    cells[secondRow, 27].SetStyle(style1);
                    cells[secondRow, 28].PutValue(item.EvaluationTime);
                    cells[secondRow, 28].SetStyle(style1);
                    cells[secondRow, 29].PutValue(item.EvaluationCost);
                    cells[secondRow, 29].SetStyle(style1);
                    secondRow += 1;
                }
                if (rowsorder > secondList.Count)
                {
                    var reduceCount = startmergepos + secondList.Count;
                    for (int i = 0; i < rowsorder - secondList.Count; i++)
                    {
                        cells[reduceCount + i, 25].SetStyle(style1);
                        cells[reduceCount + i, 26].SetStyle(style1);
                        cells[reduceCount + i, 27].SetStyle(style1);
                        cells[reduceCount + i, 28].SetStyle(style1);
                        cells[reduceCount + i, 29].SetStyle(style1);
                    }
                }

                var thirdRow = startmergepos;
                foreach (var item in thirdList)
                {
                    cells[thirdRow, 30].PutValue(item.TenderFileAuditPersonName);
                    cells[thirdRow, 30].SetStyle(style1);
                    cells[thirdRow, 31].PutValue(item.TenderFileAuditPersonDeptName);
                    cells[thirdRow, 31].SetStyle(style1);
                    cells[thirdRow, 33].PutValue(item.TenderFileAuditCost);
                    cells[thirdRow, 33].SetStyle(style1);
                    thirdRow += 1;
                }
                if (rowsorder > thirdList.Count)
                {
                    var reduceCount = startmergepos + thirdList.Count;
                    for (int i = 0; i < rowsorder - thirdList.Count; i++)
                    {
                        cells[reduceCount + i, 30].SetStyle(style1);
                        cells[reduceCount + i, 31].SetStyle(style1);
                        cells[reduceCount + i, 33].SetStyle(style1);
                    }
                }

                var fourRow = startmergepos;
                foreach (var item in fourList)
                {
                    cells[fourRow, 35].PutValue(item.ClarifyLaunchPerson);
                    cells[fourRow, 35].SetStyle(style1);
                    cells[fourRow, 36].PutValue(item.ClarifyLaunchDate.Value.ToString("yyyy-MM-dd"));
                    cells[fourRow, 36].SetStyle(style1);
                    cells[fourRow, 37].PutValue(item.ClarifyReason);
                    cells[fourRow, 37].SetStyle(style1);
                    cells[fourRow, 38].PutValue(item.ClarifyAcceptDate.Value.ToString("yyyy-MM-dd"));
                    cells[fourRow, 38].SetStyle(style1);
                    cells[fourRow, 39].PutValue(item.ClarifyDisposePerson);
                    cells[fourRow, 39].SetStyle(style1);
                    cells[fourRow, 40].PutValue(item.IsClarify);
                    cells[fourRow, 40].SetStyle(style1);
                    cells[fourRow, 41].PutValue(item.ClarifyDisposeInfo);
                    cells[fourRow, 41].SetStyle(style1);
                    cells[fourRow, 42].PutValue(item.ClarifyReplyDate.Value.ToString("yyyy-MM-dd"));
                    cells[fourRow, 42].SetStyle(style1);
                    fourRow += 1;
                }
                if (rowsorder > fourList.Count)
                {
                    var reduceCount = startmergepos + fourList.Count;
                    for (int i = 0; i < rowsorder - fourList.Count; i++)
                    {
                        cells[reduceCount + i, 35].SetStyle(style1);
                        cells[reduceCount + i, 36].SetStyle(style1);
                        cells[reduceCount + i, 37].SetStyle(style1);
                        cells[reduceCount + i, 38].SetStyle(style1);
                        cells[reduceCount + i, 39].SetStyle(style1);
                        cells[reduceCount + i, 40].SetStyle(style1);
                        cells[reduceCount + i, 41].SetStyle(style1);
                        cells[reduceCount + i, 42].SetStyle(style1);
                    }
                }

                var fiveRow = startmergepos;
                foreach (var item in fiveList)
                {
                    cells[fiveRow, 43].PutValue(item.DissentLaunchPerson);
                    cells[fiveRow, 43].SetStyle(style1);
                    cells[fiveRow, 44].PutValue(item.DissentLaunchPersonPhone);
                    cells[fiveRow, 44].SetStyle(style1);
                    cells[fiveRow, 45].PutValue(item.DissentLaunchDate.Value.ToString("yyyy-MM-dd"));
                    cells[fiveRow, 45].SetStyle(style1);
                    cells[fiveRow, 46].PutValue(item.DissentReason);
                    cells[fiveRow, 46].SetStyle(style1);
                    cells[fiveRow, 47].PutValue(item.DissentAcceptDate.Value.ToString("yyyy-MM-dd"));
                    cells[fiveRow, 47].SetStyle(style1);
                    cells[fiveRow, 48].PutValue(item.DissentAcceptPerson);
                    cells[fiveRow, 48].SetStyle(style1);
                    cells[fiveRow, 49].PutValue(item.DissentDisposePerson);
                    cells[fiveRow, 49].SetStyle(style1);
                    cells[fiveRow, 50].PutValue(item.DissentDisposeInfo);
                    cells[fiveRow, 50].SetStyle(style1);
                    cells[fiveRow, 51].PutValue(item.DissentReplyDate.Value.ToString("yyyy-MM-dd"));
                    cells[fiveRow, 51].SetStyle(style1);
                    fiveRow += 1;
                }
                if (rowsorder > fiveList.Count)
                {
                    var reduceCount = startmergepos + fiveList.Count;
                    for (int i = 0; i < rowsorder - fiveList.Count; i++)
                    {
                        cells[reduceCount + i, 43].SetStyle(style1);
                        cells[reduceCount + i, 44].SetStyle(style1);
                        cells[reduceCount + i, 45].SetStyle(style1);
                        cells[reduceCount + i, 46].SetStyle(style1);
                        cells[reduceCount + i, 47].SetStyle(style1);
                        cells[reduceCount + i, 48].SetStyle(style1);
                        cells[reduceCount + i, 49].SetStyle(style1);
                        cells[reduceCount + i, 50].SetStyle(style1);
                        cells[reduceCount + i, 51].SetStyle(style1);
                    }
                }

                if (firstList.Count == 0)
                {
                    Range range16 = ws.Cells.CreateRange(startmergepos, 16, rowsorder, 1);
                    range16.ApplyStyle(style1, sf);
                    range16.Merge();

                    Range range17 = ws.Cells.CreateRange(startmergepos, 17, rowsorder, 1);
                    range17.ApplyStyle(style1, sf);
                    range17.Merge();

                    Range range18 = ws.Cells.CreateRange(startmergepos, 18, rowsorder, 1);
                    range18.ApplyStyle(style1, sf);
                    range18.Merge();
                }

                if (secondList.Count == 0)
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

                    Range range29 = ws.Cells.CreateRange(startmergepos, 29, rowsorder, 1);
                    range29.ApplyStyle(style1, sf);
                    range29.Merge();
                }

                if (thirdList.Count == 0)
                {
                    Range range30 = ws.Cells.CreateRange(startmergepos, 30, rowsorder, 1);
                    range30.ApplyStyle(style1, sf);
                    range30.Merge();

                    Range range31 = ws.Cells.CreateRange(startmergepos, 31, rowsorder, 1);
                    range31.ApplyStyle(style1, sf);
                    range31.Merge();

                    Range range33 = ws.Cells.CreateRange(startmergepos, 33, rowsorder, 1);
                    range33.ApplyStyle(style1, sf);
                    range33.Merge();
                }

                if (fourList.Count == 0)
                {
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

                    Range range40 = ws.Cells.CreateRange(startmergepos, 40, rowsorder, 1);
                    range40.ApplyStyle(style1, sf);
                    range40.Merge();

                    Range range41 = ws.Cells.CreateRange(startmergepos, 41, rowsorder, 1);
                    range41.ApplyStyle(style1, sf);
                    range41.Merge();

                    Range range42 = ws.Cells.CreateRange(startmergepos, 42, rowsorder, 1);
                    range42.ApplyStyle(style1, sf);
                    range42.Merge();
                }

                if (fiveList.Count == 0)
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

                    Range range49 = ws.Cells.CreateRange(startmergepos, 49, rowsorder, 1);
                    range49.ApplyStyle(style1, sf);
                    range49.Merge();

                    Range range50 = ws.Cells.CreateRange(startmergepos, 50, rowsorder, 1);
                    range50.ApplyStyle(style1, sf);
                    range50.Merge();

                    Range range51 = ws.Cells.CreateRange(startmergepos, 51, rowsorder, 1);
                    range51.ApplyStyle(style1, sf);
                    range51.Merge();
                }
                #endregion
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

                cells[startmergepos, 13].PutValue(info.TenderFileSaleStartDate == null ? "" : info.TenderFileSaleStartDate.Value.ToString("yyyy-MM-dd hh:mm"));
                Range range13 = ws.Cells.CreateRange(startmergepos, 13, rowsorder, 1);
                range13.ApplyStyle(style1, sf);
                range13.Merge();

                cells[startmergepos, 14].PutValue(info.TenderFileSaleEndDate == null ? "" : info.TenderFileSaleEndDate.Value.ToString("yyyy-MM-dd hh:mm"));
                Range range14 = ws.Cells.CreateRange(startmergepos, 14, rowsorder, 1);
                range14.ApplyStyle(style1, sf);
                range14.Merge();

                cells[startmergepos, 15].PutValue(info.TenderStartDate == null ? "" : info.TenderStartDate.Value.ToString("yyyy-MM-dd hh:mm"));
                Range range15 = ws.Cells.CreateRange(startmergepos, 15, rowsorder, 1);
                range15.ApplyStyle(style1, sf);
                range15.Merge();

                cells[startmergepos, 19].PutValue(info.TenderSuccessPerson);
                Range range19 = ws.Cells.CreateRange(startmergepos, 19, rowsorder, 1);
                range19.ApplyStyle(style1, sf);
                range19.Merge();

                cells[startmergepos, 20].PutValue(info.PlanInvestPrice);
                Range range20 = ws.Cells.CreateRange(startmergepos, 20, rowsorder, 1);
                range20.ApplyStyle(style1, sf);
                range20.Merge();

                cells[startmergepos, 21].PutValue(info.QualificationExamMethod);
                Range range21 = ws.Cells.CreateRange(startmergepos, 21, rowsorder, 1);
                range21.ApplyStyle(style1, sf);
                range21.Merge();

                cells[startmergepos, 22].PutValue(info.TenderRestrictSumPrice);
                Range range22 = ws.Cells.CreateRange(startmergepos, 22, rowsorder, 1);
                range22.ApplyStyle(style1, sf);
                range22.Merge();

                cells[startmergepos, 23].PutValue(info.TenderSuccessSumPrice);
                Range range23 = ws.Cells.CreateRange(startmergepos, 23, rowsorder, 1);
                range23.ApplyStyle(style1, sf);
                range23.Merge();

                cells[startmergepos, 24].PutValue(info.SaveCapital);
                Range range24 = ws.Cells.CreateRange(startmergepos, 24, rowsorder, 1);
                range24.ApplyStyle(style1, sf);
                range24.Merge();

                cells[startmergepos, 32].PutValue(info.TenderFileAuditTime);
                Range range32 = ws.Cells.CreateRange(startmergepos, 32, rowsorder, 1);
                range32.ApplyStyle(style1, sf);
                range32.Merge();

                cells[startmergepos, 34].PutValue(info.TenderFailReason);
                Range range34 = ws.Cells.CreateRange(startmergepos, 34, rowsorder, 1);
                range34.ApplyStyle(style1, sf);
                range34.Merge();

                cells[startmergepos, 52].PutValue(info.ContractNum);
                Range range52 = ws.Cells.CreateRange(startmergepos, 52, rowsorder, 1);
                range52.ApplyStyle(style1, sf);
                range52.Merge();

                cells[startmergepos, 53].PutValue(info.ContractPrice);
                Range range53 = ws.Cells.CreateRange(startmergepos, 53, rowsorder, 1);
                range53.ApplyStyle(style1, sf);
                range53.Merge();

                cells[startmergepos, 54].PutValue(info.RelativePerson);
                Range range54 = ws.Cells.CreateRange(startmergepos, 54, rowsorder, 1);
                range54.ApplyStyle(style1, sf);
                range54.Merge();

                cells[startmergepos, 55].PutValue(info.TenderInfo);
                Range range55 = ws.Cells.CreateRange(startmergepos, 55, rowsorder, 1);
                range55.ApplyStyle(style1, sf);
                range55.Merge();

                cells[startmergepos, 56].PutValue(info.TenderRemark);
                Range range56 = ws.Cells.CreateRange(startmergepos, 56, rowsorder, 1);
                range56.ApplyStyle(style1, sf);
                range56.Merge();

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