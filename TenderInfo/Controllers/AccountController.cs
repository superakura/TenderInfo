using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
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

        #region view
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
            return View();
        }

        public ViewResult EditAccountFrame(string id, string type)
        {
            ViewBag.id = id;
            ViewBag.type = type;
            return View();
        }

        public ViewResult EditAccountProject(string id, string type)
        {
            ViewBag.id = id;
            ViewBag.type = type;
            return View();
        }
        #endregion

        #region 获取二级单位、与招标进度数据同步
        //获取二级单位，为使用单位、项目主责部门、评标委员会单位提供数据
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

        //获取台账信息，供进度同步信息时，选择要同步到哪个台账
        [HttpPost]
        public JsonResult GetAccountListForSelect()
        {
            try
            {
                var accountType = Request.Form["accountType"];
                var userInfo = App_Code.Commen.GetUserFromSession();
                var result = from m in db.Account
                             where m.ProjectResponsiblePersonID == userInfo.UserID&&m.IsSynchro!="是"
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

        //招标进度模块，同步数据到招标台账
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

        //根据招标进度ID,删除招标进度、招标台账同步关系
        [HttpPost]
        public string DelSynchroByProgressID()
        {
            try
            {
                var progressID = 0;
                int.TryParse(Request.Form["progressID"], out progressID);

                var infoAccount = db.Account.Where(w=>w.ProgressID==progressID).FirstOrDefault();
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
        #endregion

        #region Crud

        //获取招标台账列表
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

                if (User.IsInRole("招标管理"))
                {
                    result = result.Where(w => w.ProjectResponsiblePersonID == userInfo.UserID);
                }

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

                var accountList = result.OrderBy(o => o.AccountID).Skip(offset).Take(limit).ToList();

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

        //新建招标台账
        [HttpPost]
        public string Insert()
        {
            try
            {
                var projectName = Request.Form["tbxProjectName"].ToString();
                var projectType = Request.Form["tbxProjectType"].ToString();
                var tenderFileNum = Request.Form["tbxTenderFileNum"].ToString();
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

        //更新招标台账内容
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
                info.TenderMode = Request.Form["tbxTenderModeEdit"] ?? null;
                info.BidEvaluation = Request.Form["tbxBidEvaluationEdit"] ?? null;
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
                decimal.TryParse(Request.Form["tbxTenderRestrictUnitPriceEdit"],out tenderRestrictUnitPrice);
                info.TenderRestrictUnitPrice = tenderRestrictUnitPrice;

                decimal tenderRestrictSumPrice =0;
                decimal.TryParse(Request.Form["tbxTenderRestrictSumPriceEdit"],out tenderRestrictSumPrice);
                info.TenderRestrictSumPrice = tenderRestrictSumPrice;

                decimal tenderSuccessUnitPrice = 0;
                decimal.TryParse(Request.Form["tbxTenderSuccessUnitPriceEdit"],out tenderSuccessUnitPrice);
                info.TenderSuccessUnitPrice = tenderSuccessUnitPrice;

                decimal tenderSuccessSumPrice = 0;
                decimal.TryParse(Request.Form["tbxTenderSuccessSumPriceEdit"],out tenderSuccessSumPrice);
                info.TenderSuccessSumPrice = tenderSuccessSumPrice;

                decimal saveCapital = 0;
                decimal.TryParse(Request.Form["tbxSaveCapitalEdit"],out saveCapital);
                info.SaveCapital = saveCapital;
                #endregion

                //招标文件联审--联审时间（小时）
                decimal tenderFileAuditTime = 0;
                decimal.TryParse(Request.Form["tbxTenderFileAuditTimeEdit"], out tenderFileAuditTime);
                info.TenderFileAuditTime = tenderFileAuditTime;

                //招标失败原因
                info.TenderFailReason = Request.Form["tbxTenderFailReasonEdit"];

                #region 合同信息&备注
                info.ContractNum = Request.Form["tbxContractNumEdit"];

                decimal contractPrice = 0;
                decimal.TryParse(Request.Form["tbxContractPriceEdit"],out contractPrice);
                info.ContractPrice = contractPrice;

                info.RelativePerson = Request.Form["tbxRelativePersonEdit"];
                info.TenderInfo = Request.Form["ddlTenderInfoEdit"];

                info.TenderRemark = Request.Form["tbxTenderRemarkEdit"];
                #endregion

                info.InputDate = DateTime.Now;
                info.InputPersonID = userInfo.UserID;

                db.SaveChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        //获取一项招标台账信息
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
                decimal.TryParse(Request.Form["tbxQuotedPriceUnitEdit"],out quotedPriceUnitEdit);
                decimal quotedPriceSumEdit = 0;
                decimal.TryParse(Request.Form["tbxQuotedPriceSumEdit"],out quotedPriceSumEdit);
                var negationExplain = Request.Form["ddlNegationExplain"] ?? "-";

                var userInfo = App_Code.Commen.GetUserFromSession();
                var info = new Models.AccountChild();

                info.TableType = "first";
                info.AccountID = accountID;

                info.TenderFilePlanPayPerson = tenderFilePlanPayPersonEdit == string.Empty ? "-" : tenderFilePlanPayPersonEdit;
                info.TenderPerson = tenderPersonEdit == string.Empty ? "-" : tenderPersonEdit;
                info.ProductManufacturer = productManufacturerEdit == string.Empty ? "-" : productManufacturerEdit;
                info.QuotedPriceUnit = quotedPriceUnitEdit;
                info.QuotedPriceSum = quotedPriceSumEdit;
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
                decimal quotedPriceSumEdit = 0;
                decimal.TryParse(Request.Form["tbxQuotedPriceSumEdit"], out quotedPriceSumEdit);
                var negationExplain = Request.Form["ddlNegationExplain"] ?? "-";

                var userInfo = App_Code.Commen.GetUserFromSession();
                var info = db.AccountChild.Find(accountChildID);

                info.TenderFilePlanPayPerson = tenderFilePlanPayPersonEdit == string.Empty ? "-" : tenderFilePlanPayPersonEdit;
                info.TenderPerson = tenderPersonEdit == string.Empty ? "-" : tenderPersonEdit;
                info.ProductManufacturer = productManufacturerEdit == string.Empty ? "-" : productManufacturerEdit;
                info.QuotedPriceUnit = quotedPriceUnitEdit;
                info.QuotedPriceSum = quotedPriceSumEdit;
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
                decimal.TryParse(Request.Form["tbxEvaluationCostEdit"],out evaluationCostEdit);
                decimal evaluationTime = 0;
                decimal.TryParse(Request.Form["tbxEvaluationTimeEdit"],out evaluationTime);

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
                decimal.TryParse(Request.Form["tbxTenderFileAuditCostEdit"],out tenderFileAuditCostEdit);

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

        //框架文件上传
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

        //框架文件删除
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
                                excel.Rows[i]["购买招标文件潜在投标人"].ToString().Trim()==""?"-": excel.Rows[i]["购买招标文件潜在投标人"].ToString();
                            info.TenderPerson = excel.Rows[i]["投标人"].ToString().Trim()==""?"-": excel.Rows[i]["投标人"].ToString();
                            info.ProductManufacturer = null;
                            info.NegationExplain = null;
                            decimal price = 0;
                            decimal.TryParse(excel.Rows[i]["报价（万元）"].ToString(), out price);
                            info.QuotedPriceSum = price;
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
                            info.ProductManufacturer = excel.Rows[i]["产品制造商（代理、贸易商投标时填写）"].ToString().Trim()==""?"-": excel.Rows[i]["产品制造商（代理、贸易商投标时填写）"].ToString();

                            decimal priceOne = 0;
                            decimal.TryParse(excel.Rows[i]["报价--单价"].ToString(), out priceOne);
                            info.QuotedPriceUnit = priceOne;

                            decimal priceSum = 0;
                            decimal.TryParse(excel.Rows[i]["报价--总价（万元）"].ToString(), out priceSum);
                            info.QuotedPriceSum = priceSum;

                            info.NegationExplain = excel.Rows[i]["初步评审是否被否决"].ToString().Trim()==""?"-": excel.Rows[i]["初步评审是否被否决"].ToString();
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

                            info.QuotedPriceUnit = 0 ;

                            decimal priceSum = 0;
                            decimal.TryParse(excel.Rows[i]["投标总价（万元）"].ToString(), out priceSum);
                            info.QuotedPriceSum = priceSum;

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
        #endregion
    }
}