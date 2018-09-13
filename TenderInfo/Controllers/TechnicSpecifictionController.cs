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

        /// <summary>
        /// 技术规格书审批--最低价法视图
        /// </summary>
        /// <returns></returns>
        public ViewResult MinPrice()
        {
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
        /// 最低价评标法文件上传，提交审核
        /// </summary>
        /// <param name="fileMinPrice"></param>
        /// <returns></returns>
        [HttpPost]
        public string InsertMinPriceFile(HttpPostedFileBase fileMinPrice)
        {
            try
            {
                var userInfo = App_Code.Commen.GetUserFromSession();
                var info = new Models.FileMinPrice();
                if (fileMinPrice != null)
                {
                    var fileExt = Path.GetExtension(fileMinPrice.FileName).ToLower();
                    var fileName = Path.GetFileNameWithoutExtension(fileMinPrice.FileName).ToLower();
                    var newName = fileName + Guid.NewGuid() + fileExt;
                    var filePath = Request.MapPath("~/FileUpload");
                    var fullName = Path.Combine(filePath, newName);
                    fileMinPrice.SaveAs(fullName);
                    info.TechnicSpecificationFile = newName;
                }
                info.ApproveState = "待审核";
                info.InputDateTime = DateTime.Now;
                info.InputPersonID = userInfo.UserID;
                info.InputPersonName = userInfo.UserName;

                var userDeptFatherID = db.DeptInfo.Where(w => w.DeptID == userInfo.UserDeptID).FirstOrDefault().DeptFatherID;
                info.InputPersonDeptID = userDeptFatherID;
                info.InputPersonDeptName = db.DeptInfo.Where(w => w.DeptID == userDeptFatherID).FirstOrDefault().DeptName;
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}