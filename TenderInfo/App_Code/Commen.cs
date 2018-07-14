using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TenderInfo.App_Code
{
    public class Commen
    {
        public static Models.UserInfo GetUserFromSession()
        {
            var userInfo = (Models.UserInfo)System.Web.HttpContext.Current.Session["user"];
            return userInfo;
        }

        public static System.Data.DataTable ReadExcel(String strFileName)
        {
            Workbook book = new Workbook();
            book.Open(strFileName);
            Worksheet sheet = book.Worksheets[0];
            Cells cells = sheet.Cells;

            return cells.ExportDataTableAsString(0, 0, cells.MaxDataRow + 1, cells.MaxDataColumn + 1, true);
        }
    }
}