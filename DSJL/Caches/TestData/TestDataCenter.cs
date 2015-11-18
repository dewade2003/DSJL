using DSJL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace DSJL.Caches.TestData
{
    /// <summary>
    /// 测试数据中心，用来从xml加载测试数据并缓存
    /// </summary>
    public class TestDataCenter
    {
        private static Dictionary<string, Model.TestData.TestData> testDataDict = new Dictionary<string, Model.TestData.TestData>();

        public static Model.TestData.TestData GetTestDataByFileName(string fileName) {
            if (testDataDict.Keys.Contains(fileName))
            {
                return testDataDict[fileName];
            }
            else
            {
                string xmlFileName = AppPath.XmlDataDirPath + fileName;
                XDocument xdoc = XDocument.Load(xmlFileName);
                Model.TestData.TestData testData = new Model.TestData.TestData() { XDoc = xdoc };
                testDataDict.Add(fileName, testData);
                return testData;
            }
        }

        public static Model.TestData.TestData GetTestDataByFileName(Model.TestInfoModel model) {
            return GetTestDataByFileName(model.DataFileName);
        }
    }
}
