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
                    result = result.Where(w=>w.ProjectResponsiblePersonID==userInfo.UserID);
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
                int.TryParse(Request.Form["ddlProjectResponsiblePerson"].ToString(),out projectResponsiblePersonID);
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

        #region CrudMaterialEdit
        [HttpPost]
        public string InsertMaterialFirst()
        {
            try
            {
                var accountID = 0;
                int.TryParse(Request.Form["tbxAccountFirstID"],out accountID);
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