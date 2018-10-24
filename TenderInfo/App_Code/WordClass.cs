//using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace TenderInfo.App_Code
{
    public class WordClass
    {
        //Microsoft.Office.Interop.Word.Application objApp = null;
        //Document objDocLast = null;
        //Document objDocBeforeLast = null;
        //public WordClass()
        //{
        //    objApp = new Application();
        //}

        //#region 打开文件
        //public void Open(string tempDoc)
        //{
        //    object objTempDoc = tempDoc;
        //    object objMissing = System.Reflection.Missing.Value;

        //    objDocLast = objApp.Documents.Open(
        //       ref objTempDoc, //FileName 
        //       ref objMissing, //ConfirmVersions 
        //       ref objMissing, //ReadOnly 
        //       ref objMissing, //AddToRecentFiles 
        //       ref objMissing, //PasswordDocument 
        //       ref objMissing, //PasswordTemplate 
        //       ref objMissing, //Revert 
        //       ref objMissing, //WritePasswordDocument 
        //       ref objMissing, //WritePasswordTemplate 
        //       ref objMissing, //Format 
        //       ref objMissing, //Enconding 
        //       ref objMissing, //Visible 
        //       ref objMissing, //OpenAndRepair 
        //       ref objMissing, //DocumentDirection 
        //       ref objMissing, //NoEncodingDialog 
        //       ref objMissing //XMLTransform 
        //       );
        //    objDocLast.Activate();
        //}
        //#endregion

        //#region 保存文件到输出模板
        //public void SaveAs(string outDoc)
        //{
        //    object objMissing = System.Reflection.Missing.Value;
        //    object objOutDoc = outDoc;
        //    objDocLast.SaveAs(
        //    ref objOutDoc, //FileName 
        //    ref objMissing, //FileFormat 
        //    ref objMissing, //LockComments 
        //    ref objMissing, //PassWord 
        //    ref objMissing, //AddToRecentFiles 
        //    ref objMissing, //WritePassword 
        //    ref objMissing, //ReadOnlyRecommended 
        //    ref objMissing, //EmbedTrueTypeFonts 
        //    ref objMissing, //SaveNativePictureFormat 
        //    ref objMissing, //SaveFormsData 
        //    ref objMissing, //SaveAsAOCELetter, 
        //    ref objMissing, //Encoding 
        //    ref objMissing, //InsertLineBreaks 
        //    ref objMissing, //AllowSubstitutions 
        //    ref objMissing, //LineEnding 
        //    ref objMissing //AddBiDiMarks 
        //    );
        //}
        //#endregion

        //#region 循环合并多个文件（复制合并重复的文件）
        ///// <summary> 
        ///// 循环合并多个文件（复制合并重复的文件） 
        ///// </summary> 
        ///// <param name="tempDoc">模板文件</param> 
        ///// <param name="arrCopies">需要合并的文件</param> 
        ///// <param name="outDoc">合并后的输出文件</param> 
        //public void CopyMerge(string tempDoc, string[] arrCopies, string outDoc)
        //{
        //    object objMissing = Missing.Value;
        //    object objFalse = false;
        //    object objTarget = WdMergeTarget.wdMergeTargetSelected;
        //    object objUseFormatFrom = WdUseFormattingFrom.wdFormattingFromSelected;
        //    try
        //    {
        //        //打开模板文件 
        //        Open(tempDoc);
        //        foreach (string strCopy in arrCopies)
        //        {
        //            objDocLast.Merge(
        //            strCopy, //FileName 
        //            ref objTarget, //MergeTarget 
        //            ref objMissing, //DetectFormatChanges 
        //            ref objUseFormatFrom, //UseFormattingFrom 
        //            ref objMissing //AddToRecentFiles 
        //            );
        //            objDocBeforeLast = objDocLast;
        //            objDocLast = objApp.ActiveDocument;
        //            if (objDocBeforeLast != null)
        //            {
        //                objDocBeforeLast.Close(
        //                ref objFalse, //SaveChanges 
        //                ref objMissing, //OriginalFormat 
        //                ref objMissing //RouteDocument 
        //                );
        //            }
        //        }
        //        //保存到输出文件 
        //        SaveAs(outDoc);
        //        foreach (Document objDocument in objApp.Documents)
        //        {
        //            objDocument.Close(
        //            ref objFalse, //SaveChanges 
        //            ref objMissing, //OriginalFormat 
        //            ref objMissing //RouteDocument 
        //            );
        //        }
        //    }
        //    finally
        //    {
        //        objApp.Quit(
        //        ref objMissing, //SaveChanges 
        //        ref objMissing, //OriginalFormat 
        //        ref objMissing //RoutDocument 
        //        );
        //        objApp = null;
        //    }
        //}
        ///// <summary> 
        ///// 循环合并多个文件（复制合并重复的文件） 
        ///// </summary> 
        ///// <param name="tempDoc">模板文件</param> 
        ///// <param name="arrCopies">需要合并的文件</param> 
        ///// <param name="outDoc">合并后的输出文件</param> 
        //public void CopyMerge(string tempDoc, string strCopyFolder, string outDoc)
        //{
        //    string[] arrFiles = Directory.GetFiles(strCopyFolder);
        //    CopyMerge(tempDoc, arrFiles, outDoc);
        //}
        //#endregion

        //#region 循环合并多个文件（插入合并文件）
        ///// <summary> 
        ///// 循环合并多个文件（插入合并文件） 
        ///// </summary> 
        ///// <param name="tempDoc">模板文件</param> 
        ///// <param name="arrCopies">需要合并的文件</param> 
        ///// <param name="outDoc">合并后的输出文件</param> 
        //public void InsertMerge(string tempDoc, List<string> arrCopies, string outDoc)
        //{
        //    object objMissing = Missing.Value;
        //    object objFalse = false;
        //    object confirmConversion = false;
        //    object link = false;
        //    object attachment = false;
        //    try
        //    {
        //        //打开模板文件 
        //        Open(tempDoc);
        //        foreach (string strCopy in arrCopies)
        //        {
        //            objApp.Selection.InsertFile(
        //            strCopy,
        //            ref objMissing,
        //            ref confirmConversion,
        //            ref link,
        //            ref attachment
        //            );
        //        }
        //        //保存到输出文件 
        //        SaveAs(outDoc);
        //        foreach (Document objDocument in objApp.Documents)
        //        {
        //            objDocument.Close(
        //            ref objFalse, //SaveChanges 
        //            ref objMissing, //OriginalFormat 
        //            ref objMissing //RouteDocument 
        //            );
        //        }
        //    }
        //    finally
        //    {
        //        objApp.Quit(
        //        ref objMissing, //SaveChanges 
        //        ref objMissing, //OriginalFormat 
        //        ref objMissing //RoutDocument 
        //        );
        //        objApp = null;
        //    }
        //}
        ///// <summary> 
        ///// 循环合并多个文件（插入合并文件） 
        ///// </summary> 
        ///// <param name="tempDoc">模板文件</param> 
        ///// <param name="arrCopies">需要合并的文件</param> 
        ///// <param name="outDoc">合并后的输出文件</param> 
        //public void InsertMerge(string tempDoc, string strCopyFolder, string outDoc)
        //{
        //    string[] arrFiles = Directory.GetFiles(strCopyFolder);
        //    List<string> files = new List<string>();
        //    for (int i = 0; i < arrFiles.Count(); i++)
        //    {
        //        if (arrFiles[i].Contains("doc"))
        //        {
        //            files.Add(arrFiles[i]);
        //        }
        //    }
        //    InsertMerge(tempDoc, files, outDoc);
        //}
        //#endregion

        //#region 合并文件夹下的所有txt文件

        ///// <summary>
        ///// 合并多个txt文件
        ///// </summary>
        ///// <param name="infileName">文件存在的路劲</param>
        ///// <param name="outfileName">输出文件名称</param>
        //public void CombineFile(string filePath, string outfileName)
        //{
        //    string[] infileName = Directory.GetFiles(filePath, "*.txt");
        //    int b;
        //    int n = infileName.Length;
        //    FileStream[] fileIn = new FileStream[n];
        //    using (FileStream fileOut = new FileStream(outfileName, FileMode.Create))
        //    {
        //        for (int i = 0; i < n; i++)
        //        {
        //            try
        //            {
        //                fileIn[i] = new FileStream(infileName[i], FileMode.Open);
        //                while ((b = fileIn[i].ReadByte()) != -1)
        //                    fileOut.WriteByte((byte)b);
        //            }
        //            catch (System.Exception ex)
        //            {
        //                Console.WriteLine(ex.Message);
        //            }
        //            finally
        //            {
        //                fileIn[i].Close();
        //            }
        //        }
        //    }
        //}
        //#endregion
    }
}