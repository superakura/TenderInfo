using System;
using System.Collections.Generic;
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

        public ViewResult Project()
        {
            return View();
        }

        public ViewResult Service()
        {
            return View();
        }

        public ViewResult EditAccountMaterial(string id)
        {
            ViewBag.id = id;
            return View();
        }
        #endregion

        //获取二级单位，为使用单位、项目主责部门、评标委员会单位提供数据
        [HttpPost]
        public JsonResult GetDeptForSelect()
        {
            try
            {
                var info = db.DeptInfo.Where(w => w.DeptFatherID == 1).OrderBy(o=>o.DeptOrder).ToList();
                return Json(info);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        //获取台账列表
        [HttpPost]
        public JsonResult GetList()
        {
            try
            {
                var limit = 0;
                int.TryParse(Request.Form["limit"], out limit);
                var offset = 0;
                int.TryParse(Request.Form["offset"], out offset);
                var accountType = Request.Form["projectType"];//台账类别

                var userInfo = App_Code.Commen.GetUserFromSession();
                var result = from a in db.Account
                             where a.ProjectType==accountType
                             select a;

                if (User.IsInRole("招标管理"))
                {
                    result = result.Where(w => w.ProjectResponsiblePersonID == userInfo.UserID);
                }

                var accountList = result.OrderBy(o => o.AccountID).Skip(offset).Take(limit).ToList();

                List<Models.ViewAccout> list = new List<Models.ViewAccout>();
                foreach (var item in accountList)
                {
                    var viewList = new Models.ViewAccout();
                    var accountChildFirst = db.AccountChild.Where(w=>w.AccountID==item.AccountID&&w.TableType=="first").ToList();
                    var accountChildSecond = db.AccountChild.Where(w=>w.AccountID==item.AccountID&&w.TableType=="second").ToList();
                    var accountChildThird = db.AccountChild.Where(w=>w.AccountID==item.AccountID&&w.TableType=="third").ToList();
                    viewList.account = item;
                    viewList.accountChildFirst = accountChildFirst;
                    viewList.accountChildSecond = accountChildSecond;
                    viewList.accountChildThird = accountChildThird;
                    list.Add(viewList);
                }

                return Json(new { total = result.Count(), rows = list});
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        //获取台账信息，供进度同步信息时，选择要同步到哪个台账
        [HttpPost]
        public JsonResult GetMaterialListForSelect()
        {
            try
            {
                var userInfo = App_Code.Commen.GetUserFromSession();
                var result = from m in db.AccountMaterial
                             where m.ProjectResponsiblePersonID == userInfo.UserID
                             orderby m.AccountMaterialID
                             select new
                             {
                                 m.AccountMaterialID,
                                 m.ProjectName,
                                 m.TenderFileNum,
                                 m.IsOnline,
                                 m.ProjectResponsiblePersonName,
                                 m.UsingDeptName,
                                 m.ProjectResponsibleDeptName,
                                 m.ApplyPerson
                             };
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        //招标进度模块，同步数据到招标台账
        [HttpPost]
        public string UpdateProgressMaterial()
        {
            try
            {
                var accountMaterialID = 0;
                int.TryParse(Request.Form["accountMaterialID"], out accountMaterialID);

                var progressMaterialID = 0;
                int.TryParse(Request.Form["progressMaterialID"], out progressMaterialID);

                var infoAccount = db.AccountMaterial.Find(accountMaterialID);
                var infoProgress = db.ProgressInfo.Find(progressMaterialID);

                infoAccount.TenderProgramAuditDate = infoProgress.TenderProgramAuditDate;
                infoAccount.ProgramAcceptDate = infoProgress.ProgramAcceptDate;
                infoAccount.TenderFileSaleStartDate = infoProgress.TenderFileSaleStartDate;
                infoAccount.TenderFileSaleEndDate = infoProgress.TenderFileSaleEndDate;
                infoAccount.TenderStartDate = infoProgress.TenderStartDate;
                infoAccount.TenderSuccessFileDate = infoProgress.TenderSuccessFileDate;
                db.SaveChanges();

                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
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

        //更新物资招标台账内容
        [HttpPost]
        public string UpdateMaterial()
        {
            try
            {
                var accountID = 0;
                int.TryParse(Request.Form["tbxAccountMaterialIDEdit"], out accountID);
                var info = db.Account.Find(accountID);
                var userInfo = App_Code.Commen.GetUserFromSession();

                #region 使用单位~供货期
                var usingDeptID = 0;
                int.TryParse(Request.Form["ddlUsingDeptEdit"],out usingDeptID);
                info.UsingDeptID = usingDeptID;
                info.UsingDeptName = db.DeptInfo.Find(usingDeptID).DeptName;

                var projectResponsibleDeptID = 0;
                int.TryParse(Request.Form["ddlProjectResponsibleDeptEdit"],out projectResponsibleDeptID);
                info.ProjectResponsibleDeptID = projectResponsibleDeptID;
                info.ProjectResponsibleDeptName = db.DeptInfo.Find(projectResponsibleDeptID).DeptName;

                info.ApplyPerson = Request.Form["tbxApplyPersonEdit"];
                info.InvestPlanApproveNum = Request.Form["tbxInvestPlanApproveNumEdit"];

                info.TenderRange = Request.Form["tbxTenderRangeEdit"];
                info.TenderMode = Request.Form["tbxTenderModeEdit"];
                info.BidEvaluation = Request.Form["tbxBidEvaluationEdit"];
                info.SupplyPeriod = Request.Form["tbxSupplyPeriodEdit"];
                #endregion

                #region 招标方案联审时间~中标通知书发出时间
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
                info.PlanInvestPrice = Request.Form["tbxPlanInvestPriceEdit"];
                info.TenderRestrictUnitPrice = Request.Form["tbxTenderRestrictUnitPriceEdit"];

                info.TenderSuccessUnitPrice = Request.Form["tbxTenderSuccessUnitPriceEdit"];
                info.TenderSuccessSumPrice = Request.Form["tbxTenderSuccessSumPriceEdit"];
                info.SaveCapital = Request.Form["tbxSaveCapitalEdit"];
                #endregion

                //info.EvaluationTime = Request.Form["tbxEvaluationTimeEdit"];//评标委员会--评审时间（小时）
                info.TenderFileAuditTime = Request.Form["tbxTenderFileAuditTimeEdit"];//招标文件联审--联审时间（小时）
                info.TenderFailReason = Request.Form["tbxTenderFailReasonEdit"];//招标失败原因

                #region 澄清（修改）
                info.ClarifyLaunchPerson = Request.Form["tbxClarifyLaunchPersonEdit"];
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

                info.ClarifyDisposePerson = Request.Form["tbxClarifyDisposePersonEdit"];
                info.IsClarify = Request.Form["tbxIsClarifyEdit"];
                if (Request.Form["tbxClarifyReplyDateEdit"] != string.Empty)
                {
                    info.ClarifyReplyDate = Convert.ToDateTime(Request.Form["tbxClarifyReplyDateEdit"]);
                }
                else
                {
                    info.ClarifyReplyDate = null;
                }

                info.ClarifyReason = Request.Form["tbxClarifyReasonEdit"];
                info.ClarifyDisposeInfo = Request.Form["tbxClarifyDisposeInfoEdit"];
                #endregion
                #region 异议处理
                info.DissentLaunchPerson = Request.Form["tbxDissentLaunchPersonEdit"];
                info.DissentLaunchPersonPhone = Request.Form["tbxDissentLaunchPersonPhoneEdit"];
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
                info.DissentAcceptPerson = Request.Form["tbxDissentAcceptPersonEdit"];
                info.DissentDisposePerson = Request.Form["tbxDissentDisposePersonEdit"];
                if (Request.Form["tbxDissentReplyDateEdit"] != string.Empty)
                {
                    info.DissentReplyDate = Convert.ToDateTime(Request.Form["tbxDissentReplyDateEdit"]);
                }
                else
                {
                    info.DissentReplyDate = null;
                }
                info.DissentReason = Request.Form["tbxDissentReasonEdit"];
                info.DissentDisposeInfo = Request.Form["tbxDissentDisposeInfoEdit"];
                #endregion
                #region 合同信息&备注
                info.ContractNum = Request.Form["tbxContractNumEdit"];
                info.ContractPrice = Request.Form["tbxContractPriceEdit"];
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

        //获取一条招标信息
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

        #region CrudMaterialEdit
        [HttpPost]
        public string InsertFirst()
        {
            try
            {
                var accountID = 0;
                int.TryParse(Request.Form["tbxAccountFirstID"], out accountID);
                var tenderFilePlanPayPersonEdit = Request.Form["tbxTenderFilePlanPayPersonEdit"];
                var tenderPersonEdit = Request.Form["tbxTenderPersonEdit"];
                var productManufacturerEdit = Request.Form["tbxProductManufacturerEdit"];
                var quotedPriceUnitEdit = Request.Form["tbxQuotedPriceUnitEdit"];
                var quotedPriceSumEdit = Request.Form["tbxQuotedPriceSumEdit"];
                var negationExplain = Request.Form["tbxNegationExplain"];

                var userInfo = App_Code.Commen.GetUserFromSession();
                var info = new Models.AccountChild();

                info.TableType = "first";
                info.AccountID = accountID;
                info.TenderFilePlanPayPerson = tenderFilePlanPayPersonEdit;
                info.TenderPerson = tenderPersonEdit;
                info.ProductManufacturer = productManufacturerEdit;
                info.QuotedPriceUnit = quotedPriceUnitEdit;
                info.QuotedPriceSum = quotedPriceSumEdit;
                info.NegationExplain = negationExplain;

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
                var tenderFilePlanPayPersonEdit = Request.Form["tbxTenderFilePlanPayPersonEdit"];
                var tenderPersonEdit = Request.Form["tbxTenderPersonEdit"];
                var productManufacturerEdit = Request.Form["tbxProductManufacturerEdit"];
                var quotedPriceUnitEdit = Request.Form["tbxQuotedPriceUnitEdit"];
                var quotedPriceSumEdit = Request.Form["tbxQuotedPriceSumEdit"];
                var negationExplain = Request.Form["tbxNegationExplain"];

                var userInfo = App_Code.Commen.GetUserFromSession();
                var info = db.AccountChild.Find(accountChildID);

                info.TenderFilePlanPayPerson = tenderFilePlanPayPersonEdit;
                info.TenderPerson = tenderPersonEdit;
                info.ProductManufacturer = productManufacturerEdit;
                info.QuotedPriceUnit = quotedPriceUnitEdit;
                info.QuotedPriceSum = quotedPriceSumEdit;
                info.NegationExplain = negationExplain;

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
                var evaluationPersonNameEdit = Request.Form["tbxEvaluationPersonNameEdit"];

                var evaluationPersonDeptIDEdit = 0;
                int.TryParse(Request.Form["ddlEvaluationPersonDeptEdit"],out evaluationPersonDeptIDEdit);
                var evaluationPersonDeptNameEdit = db.DeptInfo.Find(evaluationPersonDeptIDEdit).DeptName;

                var isEvaluationDirectorEdit = Request.Form["tbxIsEvaluationDirectorEdit"];
                var evaluationCostEdit = Request.Form["tbxEvaluationCostEdit"];
                var evaluationTime = Request.Form["tbxEvaluationTimeEdit"];

                var userInfo = App_Code.Commen.GetUserFromSession();
                var info = new Models.AccountChild();

                info.TableType = "second";
                info.AccountID = accountID;
                info.EvaluationPersonName = evaluationPersonNameEdit;
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
                var evaluationPersonNameEdit = Request.Form["tbxEvaluationPersonNameEdit"];

                var evaluationPersonDeptIDEdit = 0;
                int.TryParse(Request.Form["ddlEvaluationPersonDeptEdit"], out evaluationPersonDeptIDEdit);
                var evaluationPersonDeptNameEdit = db.DeptInfo.Find(evaluationPersonDeptIDEdit).DeptName;

                var isEvaluationDirectorEdit = Request.Form["tbxIsEvaluationDirectorEdit"];
                var evaluationCostEdit = Request.Form["tbxEvaluationCostEdit"];
                var evaluationTime = Request.Form["tbxEvaluationTimeEdit"];

                var userInfo = App_Code.Commen.GetUserFromSession();
                var info = db.AccountChild.Find(accountChildID);

                info.EvaluationPersonName = evaluationPersonNameEdit;
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
                var tenderFileAuditPersonNameEdit = Request.Form["tbxTenderFileAuditPersonNameEdit"];
                var tenderFileAuditPersonDeptIDEdit = 0;
                int.TryParse(Request.Form["ddlTenderFileAuditPersonDeptEdit"],out tenderFileAuditPersonDeptIDEdit);
                var tenderFileAuditPersonDeptNameEdit = db.DeptInfo.Find(tenderFileAuditPersonDeptIDEdit).DeptName;
                var tenderFileAuditCostEdit = Request.Form["tbxTenderFileAuditCostEdit"];

                var userInfo = App_Code.Commen.GetUserFromSession();
                var info = new Models.AccountChild();

                info.TableType = "Third";
                info.AccountID = accountID;
                info.TenderFileAuditPersonName = tenderFileAuditPersonNameEdit;
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
                var tenderFileAuditPersonNameEdit = Request.Form["tbxTenderFileAuditPersonNameEdit"];
                var tenderFileAuditPersonDeptIDEdit = 0;
                int.TryParse(Request.Form["ddlTenderFileAuditPersonDeptEdit"], out tenderFileAuditPersonDeptIDEdit);
                var tenderFileAuditPersonDeptNameEdit = db.DeptInfo.Find(tenderFileAuditPersonDeptIDEdit).DeptName;
                var tenderFileAuditCostEdit = Request.Form["tbxTenderFileAuditCostEdit"];

                var userInfo = App_Code.Commen.GetUserFromSession();
                var info = db.AccountChild.Find(accountChildID);

                info.TenderFileAuditPersonName = tenderFileAuditPersonNameEdit;
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
        #endregion
    }
}