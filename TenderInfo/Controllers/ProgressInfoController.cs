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
            var sampleName = Request.Form["sampleName"];//样品名称

            var userInfo = App_Code.Commen.GetUserFromSession();

            var result = from p in db.ProgressInfo
                         select p;

            if (User.IsInRole("招标管理"))
            {
                result = result.Where(w=>w.ProjectResponsiblePersonID==userInfo.UserID);
            }
            return Json(new { total = result.Count(), rows = result.OrderBy(o=>o.ProgressInfoID).Skip(offset).Take(limit).ToList() });
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
                info.InvestPrice = Request.Form["tbxInvestPriceEdit"];
                info.MaterialCount = Request.Form["tbxMaterialCountEdit"];
                info.ContractResponsiblePerson = Request.Form["tbxContractResponsiblePersonEdit"];

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
                if (Request.Form["tbxTenderProgramAuditDateEdit"] != string.Empty)
                {
                    info.TenderProgramAuditDate = Convert.ToDateTime(Request.Form["tbxTenderProgramAuditDateEdit"]);
                }

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

                info.OtherExplain = Request.Form["tbxOtherExplainEdit"];
                info.Remark = Request.Form["tbxRemarkEdit"];

                info.InputDateTime = DateTime.Now;
                info.YearInfo = DateTime.Now.Year.ToString();
                info.IsOver = Request.Form["tbxTenderSuccessFileDateEdit"].Trim() != string.Empty ? "已完成" : "未完成";

                db.SaveChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}