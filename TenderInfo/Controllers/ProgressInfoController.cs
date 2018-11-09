using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TenderInfo.Controllers
{
    public class ProgressInfoController : Controller
    {
        //ProgressInfo
        private Models.DB db = new Models.DB();

        public ViewResult ProgressMaterial()
        {
            return View();
        }

        public ViewResult ProgressProject()
        {
            return View();
        }

        public ViewResult ProgressFrame()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetList()
        {
            var limit = 0;
            int.TryParse(Request.Form["limit"], out limit);
            var offset = 0;
            int.TryParse(Request.Form["offset"], out offset);

            var progressType = Request.Form["progressType"];//类别
            var progressTypeChild = Request.Form["progressTypeChild"];//子类
            var isOver = Request.Form["isOver"];//是否完成
            var progressState = Request.Form["progressState"];//进度
            var projectName = Request.Form["projectName"];//项目名称
            var projectResponsiblePerson = 0;//项目负责人
            int.TryParse(Request.Form["projectResponsiblePerson"], out projectResponsiblePerson);
            var contractResponsiblePerson = Request.Form["contractResponsiblePerson"];//项目负责人

            var tenderSuccessFileDateBegin = Request.Form["tenderSuccessFileDateBegin"];//招标开始时间范围Begin
            var tenderSuccessFileDateEnd = Request.Form["tenderSuccessFileDateEnd"];//招标开始时间范围End

            var userInfo = App_Code.Commen.GetUserFromSession();

            var result = from p in db.ProgressInfo
                         where p.ProgressType == progressType
                         select p;

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
                }
            }

            if (isOver != string.Empty)
            {
                result = result.Where(w => w.IsOver == isOver);
            }
            if (progressState != string.Empty)
            {
                result = result.Where(w => w.ProgressState == progressState);
            }
            if (progressTypeChild != string.Empty)
            {
                result = result.Where(w => w.ProgressTypeChild == progressTypeChild);
            }
            if (projectName != string.Empty)
            {
                result = result.Where(w => w.ProjectName.Contains(projectName));
            }
            if (projectResponsiblePerson != 0)
            {
                result = result.Where(w => w.ProjectResponsiblePersonID == projectResponsiblePerson);
            }
            if (contractResponsiblePerson != string.Empty)
            {
                result = result.Where(w => w.ContractResponsiblePerson.Contains(contractResponsiblePerson));
            }
            if (!string.IsNullOrEmpty(tenderSuccessFileDateBegin) & !string.IsNullOrEmpty(tenderSuccessFileDateEnd))
            {
                var dateStart = Convert.ToDateTime(tenderSuccessFileDateBegin);
                var dateEnd = Convert.ToDateTime(tenderSuccessFileDateEnd);
                result = result.Where(w => System.Data.Entity.DbFunctions.DiffDays(w.TenderFileSaleStartDate, dateStart) <= 0 && System.Data.Entity.DbFunctions.DiffMinutes(w.TenderFileSaleEndDate, dateEnd) >= 0);
            }
            return Json(new { total = result.Count(), rows = result.OrderBy(o => o.ProgressInfoID).Skip(offset).Take(limit).ToList() });
        }

        [HttpPost]
        public string Insert()
        {
            try
            {
                var projectName = Request.Form["tbxProjectName"].ToString();
                var contractResponsiblePerson = Request.Form["tbxContractResponsiblePerson"].ToString();
                var progressTypeChild = Request.Form["ddlProgressTypeChild"].ToString();
                var progressType = Request.Form["tbxProgressType"].ToString();

                var user = App_Code.Commen.GetUserFromSession();

                var info = new Models.ProgressInfo();
                info.ProjectName = projectName;
                info.ContractResponsiblePerson = contractResponsiblePerson;
                info.ProgressTypeChild = progressTypeChild;
                info.ProgressType = progressType;
                info.ProjectResponsiblePersonID = user.UserID;
                info.ProjectResponsiblePersonName = user.UserName;
                info.IsOver = "未完成";
                info.InputDateTime = DateTime.Now;
                info.YearInfo = DateTime.Now.Year.ToString();

                db.ProgressInfo.Add(info);
                db.SaveChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpPost]
        public JsonResult GetOne()
        {
            try
            {
                var id = 0;
                int.TryParse(Request.Form["id"], out id);

                var info = db.ProgressInfo.Find(id);
                return Json(info);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        /// <summary>
        /// 修改招标进度信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string Update()
        {
            try
            {
                var id = 0;
                int.TryParse(Request.Form["tbxProgressIDEdit"], out id);

                var info = db.ProgressInfo.Find(id);
                info.ProgressType = Request.Form["tbxProgressTypeEdit"];
                info.ProgressTypeChild = Request.Form["ddlProgressTypeChildEdit"];
                info.ProjectName = Request.Form["tbxProjectNameEdit"];
                info.ProgressState = Request.Form["ddlProgressStateEdit"];
                decimal investPrice = 0;
                decimal.TryParse(Request.Form["tbxInvestPriceEdit"], out investPrice);
                info.InvestPrice = investPrice;
                info.ContractResponsiblePerson = Request.Form["tbxContractResponsiblePersonEdit"];

                #region 项目前期对接进度
                if (Request.Form["tbxProgressTypeEdit"] == "工程")
                {
                    info.ContractDeptContactDate = Request.Form["tbxContractDeptContactDateEdit"];
                    info.ProjectExplain = Request.Form["tbxProjectExplainEdit"];
                }
                else
                {
                    info.MaterialCount = Request.Form["tbxMaterialCountEdit"];//物资数量

                    info.TechnicalSpecificationAddDate = Request.Form["tbxTechnicalSpecificationAddDateEdit"];
                    info.TechnicalSpecificationExplain = Request.Form["tbxTechnicalSpecificationExplainEdit"];
                    if (Request.Form["tbxTechnicalSpecificationApproveDateEdit"] != string.Empty)
                    {
                        info.TechnicalSpecificationApproveDate = Convert.ToDateTime(Request.Form["tbxTechnicalSpecificationApproveDateEdit"]);
                    }
                    if (Request.Form["tbxSynthesizeEvaluationRuleApproveDateEdit"] != string.Empty)
                    {
                        info.SynthesizeEvaluationRuleApproveDate = Convert.ToDateTime(Request.Form["tbxSynthesizeEvaluationRuleApproveDateEdit"]);
                    }
                }
                if (Request.Form["tbxTenderProgramAuditDateEdit"] != string.Empty)
                {
                    info.TenderProgramAuditDate = Convert.ToDateTime(Request.Form["tbxTenderProgramAuditDateEdit"]);
                }
                #endregion
                #region 项目实施进度
                if (Request.Form["tbxProgramAcceptDateEdit"] != string.Empty)
                {
                    info.ProgramAcceptDate = Convert.ToDateTime(Request.Form["tbxProgramAcceptDateEdit"]);
                }
                if (Request.Form["tbxTenderFileSaleStartDateEdit"] != string.Empty)
                {
                    info.TenderFileSaleStartDate = Convert.ToDateTime(Request.Form["tbxTenderFileSaleStartDateEdit"]);
                }
                if (Request.Form["tbxTenderFileSaleEndDateEdit"] != string.Empty)
                {
                    info.TenderFileSaleEndDate = Convert.ToDateTime(Request.Form["tbxTenderFileSaleEndDateEdit"]);
                }
                if (Request.Form["tbxTenderStartDateEdit"] != string.Empty)
                {
                    info.TenderStartDate = Convert.ToDateTime(Request.Form["tbxTenderStartDateEdit"]);
                }
                if (Request.Form["tbxTenderSuccessFileDateEdit"] != string.Empty)
                {
                    info.TenderSuccessFileDate = Convert.ToDateTime(Request.Form["tbxTenderSuccessFileDateEdit"]);
                }
                #endregion

                info.OtherExplain = Request.Form["tbxOtherExplainEdit"];
                info.Remark = Request.Form["tbxRemarkEdit"];

                info.InputDateTime = DateTime.Now;
                info.YearInfo = DateTime.Now.Year.ToString();
                info.IsOver = Request.Form["tbxTenderSuccessFileDateEdit"].Trim() != string.Empty ? "已完成" : "未完成";
                if (info.IsSynchro == "是")
                {
                    var infoAccount = db.Account.Find(info.AccountID);
                    infoAccount.TenderProgramAuditDate = info.TenderProgramAuditDate;
                    infoAccount.ProgramAcceptDate = info.ProgramAcceptDate;
                    infoAccount.TenderFileSaleStartDate = info.TenderFileSaleStartDate;
                    infoAccount.TenderFileSaleEndDate = info.TenderFileSaleEndDate;
                    infoAccount.TenderStartDate = info.TenderStartDate;
                }

                db.SaveChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpPost]
        public string Del()
        {
            try
            {
                var id = 0;
                int.TryParse(Request.Form["id"], out id);
                var progressInfo = db.ProgressInfo.Find(id);
                db.ProgressInfo.Remove(progressInfo);

                var userInfo = App_Code.Commen.GetUserFromSession();
                var logInfo = new Models.Log();
                logInfo.InputDateTime = DateTime.Now;
                logInfo.InputPersonID = userInfo.UserID;
                logInfo.InputPersonName = userInfo.UserName;
                logInfo.LogContent = "删除项目：" + progressInfo.ProjectName + "-" + "类型：【" + progressInfo.ProgressType + "】【" + progressInfo.ProgressTypeChild + "】";
                logInfo.LogType = "删除招标进度";
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
        /// 判断是否业务员本人修改，只有招标业务员本人能修改自己的招标进度信息。
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string CheckEdit()
        {
            try
            {
                var id = 0;
                int.TryParse(Request.Form["id"], out id);

                var info = db.ProgressInfo.Find(id);
                var userInfo = App_Code.Commen.GetUserFromSession();
                if (info.ProjectResponsiblePersonID == userInfo.UserID)
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
    }
}