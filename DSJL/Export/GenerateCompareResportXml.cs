using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace DSJL.Export
{
    /// <summary>
    /// 生成对比报告的xml数据
    /// 如果TestInfoModelList只有一条数据，只导出与参考值的对比信息
    /// 如果有2条数据，可以导出2条数据互相对比和2条数据与参考值的对比信息
    /// 如果大于2条数据，可以导出每条数据和多条数据平均值的对比信息和每条数据与参考值的对比信息
    /// </summary>
    class GenerateCompareResportXml
    {
        private ExportModeEnum exportMode;//导出模式

        public GenerateCompareResportXml(ExportModeEnum exportMode)
        {
            this.exportMode = exportMode;
        }

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
        private List<Model.TestInfoModel> checkedTestInfoModelList=new List<Model.TestInfoModel>();

        private List<Model.TestInfoModel> testInfoModelList;

        /// <summary>
        /// 测试信息列表
        /// </summary>
        public List<Model.TestInfoModel> TestInfoModelList
        {
            get {
                return testInfoModelList;
            }
            set {
                testInfoModelList = value;
                foreach (Model.TestInfoModel m in testInfoModelList) {
                    if (m.IsChecked == true) {
                        checkedTestInfoModelList.Add(m);
                    }
                }
            }
        }

        /// <summary>
        /// 参考值包含的测试信息列表
        /// </summary>
        public List<Model.TestInfoModel> StandardTestInfoModelList
        {
            get;
            set;
        }

        /// <summary>
        /// 自定义标题
        /// </summary>
        public string CurrentTitle
        {
            set
            {
                title = value;
            }
        }

        /// <summary>
        /// 当导出与参考值的对比报告时提供参考值的名称
        /// </summary>
        public string StandName
        {
            get;
            set;
        }

        int gap1Count = 3;//动作1对比前的row节点个数
        int gap2Count = 2;//动作2对比前的row节点个数

        /// <summary>
        /// 生成报告数据的xml文档
        /// </summary>
        /// <returns></returns>
        public XDocument GenerateXDoc()
        {
            XDocument doc = new XDocument();//生成的xml文档
            XElement rootEle = new XElement("root");
            doc.Add(rootEle);
            XElement titleEle = new XElement("title");
            titleEle.Value = title;
            rootEle.Add(titleEle);

            //添加testinfotable ，comparetable节点
            XElement titableXEle = new XElement("testinfotable");
            rootEle.Add(titableXEle);
            XElement cmptableXEle = new XElement("comparetable");
            rootEle.Add(cmptableXEle);

            //从模板中获取测试信息table和对比table节点模板
            XDocument templateDoc = XDocument.Load(AppDomain.CurrentDomain.BaseDirectory + "\\AppTemplate\\CompareResportTempalte.xml");//文档模板
            IEnumerable<XElement> tableEles = templateDoc.Descendants("table");
            XElement testinfotableXEle = tableEles.ElementAt<XElement>(0);//testinfotable节点
            XElement comparetableXEle = tableEles.ElementAt<XElement>(1);//comparetableXEle节点
            

            //写入对比信息
            switch (exportMode)
            {
                case ExportModeEnum.Mode1://导出互相或与平均值的对比
                    if (checkedTestInfoModelList.Count != 2)
                    {
                        throw new Exception("仅当选择的数据条目数量为两条时，才能导出互相对比报告！");
                    }

                    List<List<XElement>> t1Params = GetTestDataFileParamValues(checkedTestInfoModelList[0]);//第一个测试信息的参数信息
                    List<List<XElement>> t2Params = GetTestDataFileParamValues(checkedTestInfoModelList[1]);//第二个测试信息的参数信息
                    XElement xe = GetCompareTable(comparetableXEle, "1、" + checkedTestInfoModelList[0].Ath_Name, "2、" + checkedTestInfoModelList[1].Ath_Name, t1Params, t2Params);
                    xe.Attribute("remark").Value = "1、" + checkedTestInfoModelList[0].Ath_Name + "与2、" + checkedTestInfoModelList[1].Ath_Name + "的对比信息";
                    cmptableXEle.Add(xe);

                    break;
                case ExportModeEnum.Mode2:
                    if (checkedTestInfoModelList.Count < 1)
                    {
                        throw new Exception("请至少选择一条测试数据！");
                    }
                    List<List<XElement>> avgParam = ComputeAvg(TestInfoModelList);
                    string avgtitle = "多条平均";
                    for (int i = 0; i < checkedTestInfoModelList.Count; i++)
                    {
                        List<List<XElement>> tiParam = GetTestDataFileParamValues(checkedTestInfoModelList[i]);
                        XElement xe1 = GetCompareTable(comparetableXEle, (i + 1) + "、" + checkedTestInfoModelList[i].Ath_Name, avgtitle, tiParam, avgParam);
                        xe1.Attribute("remark").Value = (i + 1) + "、" + checkedTestInfoModelList[i].Ath_Name + "与平均曲线的对比信息";
                        cmptableXEle.Add(xe1);
                    }
                    break;
                case ExportModeEnum.Mode3://导出与参考值的对比
                    if (checkedTestInfoModelList.Count < 1)
                    {
                        throw new Exception("请至少选择一条测试数据！");
                    }
                    if (StandardTestInfoModelList.Count <=0)
                    {
                        throw new Exception("该测试参考值没有测试信息，无法导出与参考值对比的报告！");
                    }
                    else
                    {
                        List<List<XElement>> standParam = ComputeAvg(StandardTestInfoModelList);
                        for (int i = 0; i < checkedTestInfoModelList.Count; i++)
                        {
                            List<List<XElement>> tiParam = GetTestDataFileParamValues(checkedTestInfoModelList[i]);
                            XElement xe2 = GetCompareTable(comparetableXEle, (i + 1) + "、" + checkedTestInfoModelList[i].Ath_Name, "参考值", tiParam, standParam);
                            xe2.Attribute("remark").Value = (i + 1) + "、" + checkedTestInfoModelList[i].Ath_Name + "与测试参考值" + StandName + "的对比信息";
                            cmptableXEle.Add(xe2);
                        }
                    }
                    break;
            }

            #region 写入测试信息
            for (int i = 0; i < checkedTestInfoModelList.Count; i++)
            {
                Model.TestInfoModel infoModel = checkedTestInfoModelList[i];

                testinfotableXEle.Attribute("remark").Value = (i + 1) + "、受试者：" + infoModel.Ath_Name;
                IEnumerable<XElement> testInfoCells = testinfotableXEle.Descendants("cell");
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

                titableXEle.Add(testinfotableXEle);
            }
            #endregion

            return doc;
        }

        private XElement GetCompareTable(XElement compartTableXEle, string title1, string title2, List<List<XElement>> t1Params, List<List<XElement>> t2Params)
        {
            IEnumerable<XElement> rowEles = compartTableXEle.Elements("row");//对比table的row列表
            IEnumerable<XElement> row1Cells = rowEles.ElementAt<XElement>(0).Elements();//row1的cells
            row1Cells.ElementAt<XElement>(1).Attribute("label").Value = title1;
            row1Cells.ElementAt<XElement>(3).Attribute("label").Value = title2;

            for (int a = 0; a < paramNameList.Count; a++)
            {
                string t1ac1maxStr = t1Params[0][a].Attribute("max").Value;
                string t1ac1avgStr = t1Params[0][a].Attribute("avg").Value;
                string t1ac2maxStr = t1Params[1][a].Attribute("max").Value;
                string t1ac2avgStr = t1Params[1][a].Attribute("avg").Value;

                string t2ac1maxStr = t2Params[0][a].Attribute("max").Value;
                string t2ac1avgStr = t2Params[0][a].Attribute("avg").Value;
                string t2ac2maxStr = t2Params[1][a].Attribute("max").Value;
                string t2ac2avgStr = t2Params[1][a].Attribute("avg").Value;

                XElement rowaac1ele = rowEles.ElementAt<XElement>(gap1Count + a);
                IEnumerable<XElement> rowaac1cells = rowaac1ele.Elements();
                rowaac1cells.ElementAt<XElement>(1).Attribute("label").Value = t1ac1maxStr;
                rowaac1cells.ElementAt<XElement>(2).Attribute("label").Value = t1ac1avgStr;
                rowaac1cells.ElementAt<XElement>(3).Attribute("label").Value = t2ac1maxStr;
                rowaac1cells.ElementAt<XElement>(4).Attribute("label").Value = t2ac1avgStr;
                rowaac1cells.ElementAt<XElement>(5).Attribute("label").Value = FormatP(t1ac1maxStr, t2ac1maxStr);
                rowaac1cells.ElementAt<XElement>(6).Attribute("label").Value = FormatP(t1ac1avgStr, t2ac1avgStr);

                XElement rowaac2ele = rowEles.ElementAt<XElement>(gap1Count + paramNameList.Count + gap2Count + a);
                IEnumerable<XElement> rowaac2cells = rowaac2ele.Elements();
                rowaac2cells.ElementAt<XElement>(1).Attribute("label").Value = t1ac2maxStr;
                rowaac2cells.ElementAt<XElement>(2).Attribute("label").Value = t1ac2avgStr;
                rowaac2cells.ElementAt<XElement>(3).Attribute("label").Value = t2ac2maxStr;
                rowaac2cells.ElementAt<XElement>(4).Attribute("label").Value = t2ac2avgStr;
                rowaac2cells.ElementAt<XElement>(5).Attribute("label").Value = FormatP(t1ac2maxStr, t2ac2maxStr);
                rowaac2cells.ElementAt<XElement>(6).Attribute("label").Value = FormatP(t1ac2avgStr, t2ac2avgStr);
            }
            return compartTableXEle;
        }

        /// <summary>
        /// 获取数据文件中的参数信息
        /// </summary>
        /// <param name="testInfoModel">测试信息</param>
        /// <returns>列表中第一个为动作1的参数信息，第二个为动作2的参数信息</returns>
        private List<List<XElement>> GetTestDataFileParamValues(Model.TestInfoModel testInfoModel)
        {
            List<List<XElement>> lists = new List<List<XElement>>();

            List<XElement> oddEleList = new List<XElement>();//每个测试信息的动作1 的参数节点
            List<XElement> evenEleList = new List<XElement>();//每个测试信息的动作2 的参数节点

            XDocument xd = XDocument.Load(Model.AppPath.XmlDataDirPath + testInfoModel.DataFileName);
            oddEleList.AddRange(xd.Descendants("action1").ElementAt(0).Elements());
            evenEleList.AddRange(xd.Descendants("action2").ElementAt(0).Elements());

            lists.Add(oddEleList);
            lists.Add(evenEleList);
            return lists;
        }

        /// <summary>
        /// 计算第一个数字字符串和第二个数字字符串的比值并格式化为百分比
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        private string FormatP(string str1, string str2)
        {
            double d1, d2 = 0;
            if (str1 != "" && str2 != "")
            {
                d1 = double.Parse(str1);
                d2 = double.Parse(str2);
                if (d2 != 0)
                {
                    return string.Format("{0:P}", d1 / d2);
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }

        public XDocument Generate()
        {
            XDocument doc = new XDocument();//生成的xml文档
            XElement rootEle = new XElement("root");
            doc.Add(rootEle);

            XDocument reportDoc = XDocument.Load(AppDomain.CurrentDomain.BaseDirectory + "\\AppTemplate\\CompareResportTempalte.xml");//文档模板
            XElement titleEle = reportDoc.Descendants("title").ElementAt<XElement>(0);//设置title节点
            titleEle.Value = title;
            rootEle.Add(titleEle);

            //添加testinfotable ，comparetable节点
            XElement titableXEle = new XElement("testinfotable");
            rootEle.Add(titableXEle);
            XElement cmptableXEle = new XElement("comparetable");
            rootEle.Add(cmptableXEle);

            XElement testinfotableXEle = reportDoc.Descendants("testinfotable").ElementAt<XElement>(0);//testinfotable节点
            XElement testInfoEle = testinfotableXEle.Element("table");
            IEnumerable<XElement> tableEles = reportDoc.Descendants("table");//table节点列表,第一个为测试信息表格，第二个为参数信息表格
            List<XElement> oddEleList = new List<XElement>();//每个测试信息的动作1 的参数节点
            List<XElement> evenEleList = new List<XElement>();//每个测试信息的动作2 的参数节点
            for (int i = 0; i < TestInfoModelList.Count; i++)
            {
                Model.TestInfoModel infoModel = TestInfoModelList[i];

                XDocument xd = XDocument.Load(Model.AppPath.XmlDataDirPath + infoModel.DataFileName);
                oddEleList.AddRange(xd.Descendants("action1").ElementAt(0).Elements());
                evenEleList.AddRange(xd.Descendants("action2").ElementAt(0).Elements());

                #region 创建测试信息表格
                testInfoEle.Attribute("remark").Value = (i + 1) + "、受试者：" + infoModel.Ath_Name;
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
                //rootEle.Add(testInfoEle);//添加测试信息
                titableXEle.Add(testInfoEle);
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
                if (ac1maxNoEmptyEleList.Count > 0)
                {
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

        public static List<List<XElement>> ComputeAvg(List<Model.TestInfoModel> modelList)
        {
            var groupedModelList = modelList.GroupBy(x=>x.Ath_ID);


            List<XElement> oddEleList = new List<XElement>();//每个测试信息的动作1 的参数节点
            List<XElement> evenEleList = new List<XElement>();//每个测试信息的动作2 的参数节点
            for (int i = 0; i < modelList.Count; i++)
            {
                Model.TestInfoModel infoModel = modelList[i];

                XDocument xd = XDocument.Load(Model.AppPath.XmlDataDirPath + infoModel.DataFileName);
                oddEleList.AddRange(xd.Descendants("action1").ElementAt(0).Elements());
                evenEleList.AddRange(xd.Descendants("action2").ElementAt(0).Elements());
            }

            List<XElement> list1 = new List<XElement>();
            List<XElement> list2 = new List<XElement>();

            for (int i = 0; i < paramNameList.Count; i++)
            {
                int paramIndex = i;//参数名称索引

                string ac1max = "";//动作1最大值平均
                string ac1avg = "";//动作1平均值平均
                string ac2max = "";//动作2最大值平均
                string ac2avg = "";//动作2平均值平均

                //动作1最大值平均
                List<XElement> ac1maxNoEmptyEleList = oddEleList.Where(x => x.Name == paramNameList[paramIndex] && x.Attribute("max").Value != "").ToList<XElement>();
                if (ac1maxNoEmptyEleList.Count > 0)
                {
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
                XElement xe = new XElement(paramNameList[i]);
                xe.SetAttributeValue("max", ac1max);
                xe.SetAttributeValue("avg", ac1avg);
                list1.Add(xe);
                XElement xe2 = new XElement(paramNameList[i]);
                xe2.SetAttributeValue("max", ac2max);
                xe2.SetAttributeValue("avg", ac2avg);
                list2.Add(xe2);
            }

            List<List<XElement>> list = new List<List<XElement>>();
            list.Add(list1);
            list.Add(list2);
            return list;
        }
    }

    enum ExportModeEnum
    {
        /// <summary>
        /// 模式1，互相对比
        /// </summary>
        Mode1,
        /// <summary>
        /// 模式2，导出个人与平均值的对比
        /// </summary>
        Mode2,
        /// <summary>
        /// 模式3，导出个人与参考值的对比
        /// </summary>
        Mode3
    }
}
