using Aspose.Cells;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
//using Spire.Pdf;

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

        public static string GetDateTimeString()
        {
            return DateTime.Now.Year.ToString() +
                DateTime.Now.Month.ToString() +
                DateTime.Now.Day.ToString() +
                DateTime.Now.Hour.ToString() +
                DateTime.Now.Minute.ToString() +
                DateTime.Now.Second.ToString() +
                DateTime.Now.Millisecond.ToString();
        }

        public static void MergePdfFilesBySpire(List<string> fileList, string outMergeFile)
        {
            //PdfDocumentBase doc = Spire.Pdf.PdfDocument.MergeFiles(fileList.ToArray());
            //doc.Save(outMergeFile, FileFormat.PDF);
            //System.Diagnostics.Process.Start(outMergeFile);
        }

        /// <summary>
        /// 合成pdf文件
        /// </summary>
        /// <param name="fileList">文件名list</param>
        /// <param name="outMergeFile">输出路径</param>
        public static void MergePdfFilesByiTextSharp(List<string> fileList, string outMergeFile)
        {
            PdfReader reader;
            List<PdfReader> readerList = new List<PdfReader>();
            Document document = new Document();
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(outMergeFile, FileMode.Create));
            document.Open();
            PdfContentByte cb = writer.DirectContent;
            PdfImportedPage newPage;
            for (int i = 0; i < fileList.Count; i++)
            {
                reader = new PdfReader(fileList[i]);

                int iPageNum = reader.NumberOfPages;
                for (int j = 1; j <= iPageNum; j++)
                {
                    document.NewPage();
                    newPage = writer.GetImportedPage(reader, j);
                    cb.AddTemplate(newPage, 0, 0);
                }
                readerList.Add(reader);
            }
            document.Close();
            foreach (var rd in readerList)//清理占用
            {
                rd.Close();
            }
        }
    }
}