using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

namespace DSJL.Export
{
    /// <summary>
    /// 生成多人平均对比报告的xml
    /// </summary>
    class GenerateAvgReportXML
    {
        private static readonly List<string> paramNameList = new List<string>() { 
            "moment",
            "maxangle",
            "relativemoment",
            "acting",
            "relativeacting",
            "maxpower",
            "maxrelativepower",
            "endangle",
            "fiftymoment",
            "hundredmoment",
            "slope1",
            "tweentypercenttime",
            "seventypercenttime",
            "slope2",
            "fatigue1",
            "fatigue2",
            "momentratio",
            "actingratio",
            "powerratio"
        };

        //报名标题
        private string title = "多人对比测试报告";

        /// <summary>
        /// 测试信息列表
        /// </summary>
        public List<Model.TestInfoModel> TestInfoModelList
        {
            get;
            set;
        }

        /// <summary>
        /// 自定义标题
        /// </summary>
        public string CurrentTitle {
            set {
                title = value;
            }
        }

        public XDocument Generate() {
            XDocument doc = new XDocument();//生成的xml文档
            XElement rootEle = new XElement("root");
            doc.Add(rootEle);

            XDocument reportDoc = XDocument.Load(AppDomain.CurrentDomain.BaseDirectory + "\\AppTemplate\\avgreport.xml");//文档模板
            XElement titleEle = reportDoc.Descendants("title").ElementAt<XElement>(0);//设置title节点
            titleEle.Value = title;
            rootEle.Add(titleEle);
            IEnumerable<XElement> tableEles = reportDoc.Descendants("table");//table节点列表,第一个为测试信息表格，第二个为参数信息表格
            List<XElement> oddEleList = new List<XElement>();//每个测试信息的动作1 的参数节点
            List<XElement> evenEleList = new List<XElement>();//每个测试信息的动作2 的参数节点
            foreach (Model.TestInfoModel infoModel in TestInfoModelList) {
                XDocument xd = XDocument.Load(Model.AppPath.XmlDataDirPath + infoModel.DataFileName);
                oddEleList.AddRange(xd.Descendants("action1").ElementAt(0).Elements());
                evenEleList.AddRange(xd.Descendants("action2").ElementAt(0).Elements());

                #region 创建测试信息表格
                XElement testInfoEle = tableEles.ElementAt<XElement>(0);
                testInfoEle.Attribute("remark").Value = "受试者：" + infoModel.Ath_Name;
                IEnumerable<XElement> testInfoCells = testInfoEle.Descendants("cell");
                testInfoCells.ElementAt(0).Attribute("value").Value = infoModel.Ath_TestAddress;
                testInfoCells.ElementAt(1).Attribute("value").Value = infoModel.Ath_TestState;
                testInfoCells.ElementAt(2).Attribute("value").Value = infoModel.Ath_TestDate.ToString("yyyy年MM月dd");

                testInfoCells.ElementAt(3).Attribute("value").Value = infoModel.DJoint;
                testInfoCells.ElementAt(4).Attribute("value").Value = infoModel.DJointSide;
                testInfoCells.ElementAt(5).Attribute("value").Value = infoModel.DTestMode;

                testInfoCells.ElementAt(6).Attribute("value").Value = infoModel.DPlane;
                testInfoCells.ElementAt(7).Attribute("value").Value = infoModel.MotionRange;
                testInfoCells.ElementAt(8).Attribute("value").Value = infoModel.Speed;

                testInfoCells.ElementAt(9).Attribute("value").Value = infoModel.Break;
                testInfoCells.ElementAt(10).Attribute("value").Value = infoModel.NOOfSets;
                testInfoCells.ElementAt(11).Attribute("value").Value = infoModel.NOOfRepetitions;

                testInfoCells.ElementAt(12).Attribute("value").Value = infoModel.Ath_Remark;
                rootEle.Add(testInfoEle);//添加测试信息
                #endregion
            }

            XElement paramsTableEle = tableEles.ElementAt<XElement>(1);
            IEnumerable<XElement> rowEles = paramsTableEle.Descendants("row");
            for (int i = 3; i < rowEles.Count(); i++)
            {
                int paramIndex = i - 3;//参数名称索引

                string ac1max = "";//动作1最大值平均
                string ac1avg = "";//动作1平均值平均
                string ac2max = "";//动作2最大值平均
                string ac2avg = "";//动作2平均值平均

                //动作1最大值平均
                List<XElement> ac1maxNoEmptyEleList = oddEleList.Where(x => x.Name == paramNameList[paramIndex] && x.Attribute("max").Value != "").ToList<XElement>();
                if (ac1maxNoEmptyEleList.Count > 0) {
                    ac1max = Math.Round(ac1maxNoEmptyEleList.Average(x => double.Parse(x.Attribute("max").Value)), 2).ToString();
                }
                //动作1平均值平均
                List<XElement> ac1avgNoEmptyEleList = oddEleList.Where(x => x.Name == paramNameList[paramIndex] && x.Attribute("avg").Value != "").ToList<XElement>();
                if (ac1avgNoEmptyEleList.Count > 0)
                {
                    ac1avg = Math.Round(ac1avgNoEmptyEleList.Average(x => double.Parse(x.Attribute("avg").Value)), 2).ToString();
                }
                //动作2最大值平均
                List<XElement> ac2maxNoEmptyEleList = evenEleList.Where(x => x.Name == paramNameList[paramIndex] && x.Attribute("max").Value != "").ToList<XElement>();
                if (ac2maxNoEmptyEleList.Count > 0)
                {
                    ac2max = Math.Round(ac2maxNoEmptyEleList.Average(x => double.Parse(x.Attribute("max").Value)), 2).ToString();
                }
                //动作2平均值平均
                List<XElement> ac2avgNoEmptyEleList = evenEleList.Where(x => x.Name == paramNameList[paramIndex] && x.Attribute("avg").Value != "").ToList<XElement>();
                if (ac2avgNoEmptyEleList.Count > 0)
                {
                    ac2avg = Math.Round(ac2avgNoEmptyEleList.Average(x => double.Parse(x.Attribute("avg").Value)), 2).ToString();
                }
                IEnumerable<XElement> cellEles = rowEles.ElementAt(i).Elements();
                cellEles.ElementAt(1).Attribute("label").Value = ac1max;
                cellEles.ElementAt(2).Attribute("label").Value = ac1avg;
                cellEles.ElementAt(3).Attribute("label").Value = ac2max;
                cellEles.ElementAt(4).Attribute("label").Value = ac2avg;
            }

            rootEle.Add(paramsTableEle);//添加平均参数信息

            return doc;
        }
    }
}
