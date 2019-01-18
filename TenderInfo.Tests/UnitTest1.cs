using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TenderInfo.Controllers;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using System.Linq;

namespace TenderInfo.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            TestController controller = new TestController();
            List<TenderInfo.Models.Log> list = new List<Models.Log>();

            TenderInfo.Models.Log logMoq = new Models.Log();
            logMoq.InputDateTime = DateTime.Now;
            logMoq.InputPersonID = 12;
            logMoq.InputPersonName = "田兴旺";
            logMoq.LogContent = "test";
            logMoq.LogType = "testType";
            logMoq.LogID = 1;

            list.Add(logMoq);

            TenderInfo.Models.LogView log = (TenderInfo.Models.LogView)controller.LogInfo(list).Model;
            
            TenderInfo.Models.Log[] logTest=log.logList.ToArray();
            Assert.AreEqual(logTest[0].LogContent,logMoq.LogContent);
            Assert.AreEqual(logTest[0].InputPersonName,logMoq.InputPersonName);
        }
    }
}
