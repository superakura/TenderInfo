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

        [HttpPost]
        public JsonResult GetMaterialList()
        {
            try
            {
                var limit = 0;
                int.TryParse(Request.Form["limit"], out limit);
                var offset = 0;
                int.TryParse(Request.Form["offset"], out offset);

                var userInfo = App_Code.Commen.GetUserFromSession();
                var result = from m in db.AccountMaterial
                             select m;

                if (User.IsInRole("招标管理"))
                {
                    result = result.Where(w => w.ProjectResponsiblePersonID == userInfo.UserID);
                }
                return Json(new { total = result.Count(), rows = result.OrderBy(o => o.AccountMaterialID).Skip(offset).Take(limit).ToList() });
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpPost]
        public string InsertMaterial()
        {
            try
            {
                var projectName = Request.Form["tbxProjectName"].ToString();
                var tenderFileNum = Request.Form["tbxTenderFileNum"].ToString();
                var isOnline = Request.Form["ddlIsOnline"].ToString();
                var projectResponsiblePersonID = 0;
                int.TryParse(Request.Form["ddlProjectResponsiblePerson"].ToString(), out projectResponsiblePersonID);
                var projectResponsiblePersonName = db.UserInfo.Find(projectResponsiblePersonID).UserName;

                var user = App_Code.Commen.GetUserFromSession();

                var info = new Models.AccountMaterial();
                info.InsertDate = DateTime.Now;
                info.InsertPersonID = user.UserID;
                info.ProjectName = projectName;
                info.TenderFileNum = tenderFileNum;
                info.IsOnline = isOnline;
                info.ProjectResponsiblePersonID = projectResponsiblePersonID;
                info.ProjectResponsiblePersonName = projectResponsiblePersonName;
                db.AccountMaterial.Add(info);
                db.SaveChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpPost]
        public string UpdateMaterial()
        {
            try
            {
                var accountID = 0;
                int.TryParse(Request.Form["tbxAccountMaterialIDEdit"], out accountID);
                var info = db.AccountMaterial.Find(accountID);
                var userInfo = App_Code.Commen.GetUserFromSession();

                #region 使用单位~供货期
                info.UsingDept = Request.Form["tbxUsingDeptEdit"];
                info.ProjectResponsibleDept = Request.Form["tbxProjectResponsibleDeptEdit"];
                info.ApplyPerson = Request.Form["tbxApplyPersonEdit"];
                info.InvestPlanApproveNum = Request.Form["tbxInvestPlanApproveNumEdit"];

                info.TenderRange = Request.Form["tbxTenderRangeEdit"];
                info.TenderMode = Request.Form["tbxTenderModeEdit"];
                info.BidEvaluation = Request.Form["tbxBidEvaluationEdit"];
                info.SupplyPeriod = Request.Form["tbxSupplyPeriodEdit"];
                #endregion

                #region 中标人名称~与控制价比节约资金（元）
                info.TenderSuccessPerson = Request.Form["tbxTenderSuccessPersonEdit"];
                info.PlanInvestPrice = Request.Form["tbxPlanInvestPriceEdit"];
                info.TenderRestrictUnitPrice = Request.Form["tbxTenderRestrictUnitPriceEdit"];

                info.TenderSuccessUnitPrice = Request.Form["tbxTenderSuccessUnitPriceEdit"];
                info.TenderSuccessSumPrice = Request.Form["tbxTenderSuccessSumPriceEdit"];
                info.SaveCapital = Request.Form["tbxSaveCapitalEdit"];
                #endregion

                info.EvaluationTime = Request.Form["tbxEvaluationTimeEdit"];//评标委员会--评审时间（小时）
                info.TenderFileAuditTime = Request.Form["tbxTenderFileAuditTimeEdit"];//招标文件联审--联审时间（小时）
                info.TenderFailReason = Request.Form["tbxTenderFailReasonEdit"];//招标失败原因

                #region 澄清（修改）
                info.ClarifyLaunchPerson = Request.Form["tbxClarifyLaunchPersonEdit"];
                if (Request.Form["tbxClarifyLaunchDateEdit"] != string.Empty)
                {
                    info.ClarifyLaunchDate = Convert.ToDateTime(Request.Form["tbxClarifyLaunchDateEdit"]);
                }
                if (Request.Form["tbxClarifyAcceptDateEdit"] != string.Empty)
                {
                    info.ClarifyAcceptDate = Convert.ToDateTime(Request.Form["tbxClarifyAcceptDateEdit"]);
                }

                info.ClarifyDisposePerson = Request.Form["tbxClarifyDisposePersonEdit"];
                info.IsClarify = Request.Form["tbxIsClarifyEdit"];
                if (Request.Form["tbxClarifyReplyDateEdit"] != string.Empty)
                {
                    info.ClarifyReplyDate = Convert.ToDateTime(Request.Form["tbxClarifyReplyDateEdit"]);
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
                if (Request.Form["tbxDissentAcceptDateEdit"] != string.Empty)
                {
                    info.DissentAcceptDate = Convert.ToDateTime(Request.Form["tbxDissentAcceptDateEdit"]);
                }
                info.DissentAcceptPerson = Request.Form["tbxDissentAcceptPersonEdit"];
                info.DissentDisposePerson = Request.Form["tbxDissentDisposePersonEdit"];
                if (Request.Form["tbxDissentReplyDateEdit"] != string.Empty)
                {
                    info.DissentReplyDate = Convert.ToDateTime(Request.Form["tbxDissentReplyDateEdit"]);
                }
                info.DissentReason = Request.Form["tbxDissentReasonEdit"];
                info.DissentDisposeInfo = Request.Form["tbxDissentDisposeInfoEdit"];
                #endregion
                #region 合同信息&备注
                info.ContractNum = Request.Form["tbxContractNumEdit"];
                info.ContractPrice = Request.Form["tbxContractPriceEdit"];
                info.RelativePerson = Request.Form["tbxRelativePersonEdit"];
                info.TenderInfo = Request.Form["tbxTenderInfoEdit"];

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

        [HttpPost]
        public JsonResult GetOneMaterial()
        {
            try
            {
                var accountID = 0;
                int.TryParse(Request.Form["accountID"], out accountID);
                var info = db.AccountMaterial.Find(accountID);
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
        public string InsertMaterialFirst()
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

                var userInfo = App_Code.Commen.GetUserFromSession();
                var info = new Models.AccountMaterialChild();

                info.TableType = "first";
                info.AccountMaterialID = accountID;
                info.TenderFilePlanPayPerson = tenderFilePlanPayPersonEdit;
                info.TenderPerson = tenderPersonEdit;
                info.ProductManufacturer = productManufacturerEdit;
                info.QuotedPriceUnit = quotedPriceUnitEdit;
                info.QuotedPriceSum = quotedPriceSumEdit;
                info.InputDate = DateTime.Now;
                info.InputPerson = userInfo.UserID;
                db.AccountMaterialChild.Add(info);
                db.SaveChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpPost]
        public string UpdateMaterialFirst()
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

                var userInfo = App_Code.Commen.GetUserFromSession();
                var info = db.AccountMaterialChild.Find(accountChildID);

                info.TenderFilePlanPayPerson = tenderFilePlanPayPersonEdit;
                info.TenderPerson = tenderPersonEdit;
                info.ProductManufacturer = productManufacturerEdit;
                info.QuotedPriceUnit = quotedPriceUnitEdit;
                info.QuotedPriceSum = quotedPriceSumEdit;
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
        public string InsertMaterialSecond()
        {
            try
            {
                var accountID = 0;
                int.TryParse(Request.Form["tbxAccountSecondID"], out accountID);
                var evaluationPersonNameEdit = Request.Form["tbxEvaluationPersonNameEdit"];
                var evaluationPersonDeptEdit = Request.Form["tbxEvaluationPersonDeptEdit"];
                var isEvaluationDirectorEdit = Request.Form["tbxIsEvaluationDirectorEdit"];
                var evaluationCostEdit = Request.Form["tbxEvaluationCostEdit"];

                var userInfo = App_Code.Commen.GetUserFromSession();
                var info = new Models.AccountMaterialChild();

                info.TableType = "second";
                info.AccountMaterialID = accountID;
                info.EvaluationPersonName = evaluationPersonNameEdit;
                info.EvaluationPersonDept = evaluationPersonDeptEdit;
                info.IsEvaluationDirector = isEvaluationDirectorEdit;
                info.EvaluationCost = evaluationCostEdit;
                info.InputDate = DateTime.Now;
                info.InputPerson = userInfo.UserID;
                db.AccountMaterialChild.Add(info);
                db.SaveChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpPost]
        public string UpdateMaterialSecond()
        {
            try
            {
                var accountChildID = 0;
                int.TryParse(Request.Form["tbxAccountChildSecondID"], out accountChildID);
                var evaluationPersonNameEdit = Request.Form["tbxEvaluationPersonNameEdit"];
                var evaluationPersonDeptEdit = Request.Form["tbxEvaluationPersonDeptEdit"];
                var isEvaluationDirectorEdit = Request.Form["tbxIsEvaluationDirectorEdit"];
                var evaluationCostEdit = Request.Form["tbxEvaluationCostEdit"];

                var userInfo = App_Code.Commen.GetUserFromSession();
                var info = db.AccountMaterialChild.Find(accountChildID);

                info.EvaluationPersonName = evaluationPersonNameEdit;
                info.EvaluationPersonDept = evaluationPersonDeptEdit;
                info.IsEvaluationDirector = isEvaluationDirectorEdit;
                info.EvaluationCost = evaluationCostEdit;
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
        public string InsertMaterialThird()
        {
            try
            {
                var accountID = 0;
                int.TryParse(Request.Form["tbxAccountThirdID"], out accountID);
                var tenderFileAuditPersonNameEdit = Request.Form["tbxTenderFileAuditPersonNameEdit"];
                var tenderFileAuditPersonDeptEdit = Request.Form["tbxTenderFileAuditPersonDeptEdit"];
                var tenderFileAuditCostEdit = Request.Form["tbxTenderFileAuditCostEdit"];

                var userInfo = App_Code.Commen.GetUserFromSession();
                var info = new Models.AccountMaterialChild();

                info.TableType = "Third";
                info.AccountMaterialID = accountID;
                info.TenderFileAuditPersonName = tenderFileAuditPersonNameEdit;
                info.TenderFileAuditPersonDept = tenderFileAuditPersonDeptEdit;
                info.TenderFileAuditCost = tenderFileAuditCostEdit;
                info.InputDate = DateTime.Now;
                info.InputPerson = userInfo.UserID;
                db.AccountMaterialChild.Add(info);
                db.SaveChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpPost]
        public string UpdateMaterialThird()
        {
            try
            {
                var accountChildID = 0;
                int.TryParse(Request.Form["tbxAccountChildThirdID"], out accountChildID);
                var tenderFileAuditPersonNameEdit = Request.Form["tbxTenderFileAuditPersonNameEdit"];
                var tenderFileAuditPersonDeptEdit = Request.Form["tbxTenderFileAuditPersonDeptEdit"];
                var tenderFileAuditCostEdit = Request.Form["tbxTenderFileAuditCostEdit"];

                var userInfo = App_Code.Commen.GetUserFromSession();
                var info = db.AccountMaterialChild.Find(accountChildID);

                info.TenderFileAuditPersonName = tenderFileAuditPersonNameEdit;
                info.TenderFileAuditPersonDept = tenderFileAuditPersonDeptEdit;
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
        public string DelMaterialEdit()
        {
            try
            {
                var accountChildID = 0;
                int.TryParse(Request.Form["tbxAccountChildID"], out accountChildID);
                var info = db.AccountMaterialChild.Find(accountChildID);
                db.AccountMaterialChild.Remove(info);
                db.SaveChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpPost]
        public JsonResult GetOneMaterialEdit()
        {
            try
            {
                var accountChildID = 0;
                int.TryParse(Request.Form["tbxAccountChildID"], out accountChildID);
                var info = db.AccountMaterialChild.Find(accountChildID);
                return Json(info);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult GetListMaterialEdit()
        {
            try
            {
                var accountID = 0;
                int.TryParse(Request.Form["accountID"], out accountID);
                var type = Request.Form["type"];

                var info = db.AccountMaterialChild.Where(w => w.TableType == type && w.AccountMaterialID == accountID).ToList();
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