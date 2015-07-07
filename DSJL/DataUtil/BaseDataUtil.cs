using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using Microsoft.Office.Interop.Excel;
using System.Reflection;

namespace DSJL.DataUtil
{
    //数据处理业务逻辑
    //1、创建节点，写入原始数据，和计算的参数的节点   WriteBaseData
    //2、校正数据，计算出开始的节点索引，校正测试次数，计算出总的测试次数不写入 CorrectData
    //3、计算参数，写入文件 ComputeParams

    /// <summary>
    /// 原始数据处理
    /// </summary>
    class BaseDataUtil
    {
        private XDocument xdoc;

        private string weight="";//体重
        private string gravitycomp = "0";//重力补偿
        private string momentColumn = "c2";//力矩列
        private int smoothValue = 10;

        /// <summary>
        /// 体重
        /// </summary>
        public string Weight {
            set {
                weight = value;
            }
        }

        public string Gravitycomp {
            set {
                gravitycomp = value;
                //if (value == "1") {
                //    momentColumn = "c2";
                //}
            }
        }

        /// <summary>
        /// 平滑系数
        /// </summary>
        public int SmoothValue {
            set {
                smoothValue = value;
            }
        }

        public BaseDataUtil() {
        }

        /// <summary>
        /// 写入原始数据到xml文件
        /// </summary>
        /// <returns>xml文件名</returns>
        public string WriteBaseData(string[] dataLines)
        {
            string fileName = DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".xml";
            string absuloteFileName = AppDomain.CurrentDomain.BaseDirectory + "\\AppData\\XmlData\\" + fileName;
            if (!Directory.Exists(Model.AppPath.RootPath + "\\AppData\\XmlData")) {
                Directory.CreateDirectory(Model.AppPath.RootPath + "\\AppData\\XmlData");
            }
            xdoc = new XDocument();
            XElement rootEle = new XElement("root");
            XElement datasEle = new XElement("datas");
            datasEle.SetAttributeValue("startindex", "0");
            rootEle.Add(datasEle);

            //保存数据行
            List<string[]> smoothDataList = Smooth(smoothValue, dataLines);//平滑
            datasEle.SetAttributeValue("smoothvalue", smoothValue);
            foreach (string[] dataArray in smoothDataList) {
                if (dataArray.Length >= 8)
                {
                    XElement childEle = new XElement("data");
                    for (int i = 0; i < dataArray.Length; i++)
                    {
                        childEle.SetAttributeValue("c" + i.ToString(), dataArray[i]);
                    }
                    datasEle.Add(childEle);
                }
            }
          
            //for (int a = 101; a < dataLines.Length; a++)
            //{
            //    string s = dataLines[a];
            //    string[] datas = s.Split(' ');
            //    if (datas.Count() >= 8) {
            //        XElement childEle = new XElement("data");
            //        for (int i = 0; i < datas.Length; i++)
            //        {
            //            childEle.SetAttributeValue("c" + i.ToString(), datas[i]);
            //        }
            //        datasEle.Add(childEle);
            //    }
            //}
            //计算测试次数
            string testIndexs1 = "";
            string testIndexs2 = "";
            int allCount = int.Parse(datasEle.Elements().Last<XElement>().Attribute("c5").Value);
            int testCount = 0;
            if (allCount % 2 == 0)
            { //偶数
                testCount = allCount / 2;
                datasEle.SetAttributeValue("testcount", testCount * 2);
                for (int i = 1; i <= testCount; i++)
                {
                    testIndexs2 += i.ToString();
                    testIndexs1 += i.ToString();
                    if (i != testCount)
                    {
                        testIndexs1 += ",";
                        testIndexs2 += ",";
                    }
                }
            }
            else {
                testCount = (allCount + 1) / 2;
                datasEle.SetAttributeValue("testcount", testCount * 2 - 1);
                for (int i = 1; i <= testCount; i++)
                {
                    testIndexs1 += i.ToString();
                    if (i != testCount)
                    {
                        testIndexs2 += i.ToString();
                        testIndexs1 += ",";
                        testIndexs2 += ",";
                    }
                }
            }
            

            XElement statisticsEle = new XElement("statistics");//统计节点
            rootEle.Add(statisticsEle);

            XElement actionEle1 = new XElement("action1");
            actionEle1.SetAttributeValue("index", testIndexs1);
            XElement actionEle2 = new XElement("action2");
            actionEle2.SetAttributeValue("index", testIndexs2);

            XElement momentEle = CreateParamElement("moment", "峰值力矩(N·m)");
            XElement maxAngleEle = CreateParamElement("maxangle", "峰力矩时关节角(°)");
            XElement relativeMomentEle = CreateParamElement("relativemoment", "相对峰力矩(Nm/Kg)");
            XElement actingEle = CreateParamElement("acting", "做功(J)");
            XElement relativeactingEle = CreateParamElement("relativeacting", "相对做功(J/Kg)");
            XElement maxPowerEle = CreateParamElement("maxpower", "峰值功率(W)");
            XElement maxrelativepowerEle = CreateParamElement("maxrelativepower", "峰值相对功率(W/Kg)");
            XElement endAngleEle = CreateParamElement("endangle", "终点角度(°)");
            XElement fiftyMomentEle = CreateParamElement("fiftymoment", "慢速测试前50ms力矩(N·m)");
            XElement hundredMomentEle = CreateParamElement("hundredmoment", "慢速测试前100ms力矩(N·m)");
            XElement slopeEle1 = CreateParamElement("slope1", "50-100ms力矩斜率(N*m/ms)");
            XElement tweentypercenttimeEle = CreateParamElement("tweentypercenttime", "达到最大力矩20%所用时间(ms)");
            XElement seventypercenttimeEle = CreateParamElement("seventypercenttime", "达到最大力矩70%所用时间(ms)");
            XElement slopeEle2 = CreateParamElement("slope2", "20-70%最大力矩的斜率(N*m/ms)");
            XElement fatigueEle1 = CreateParamElement("fatigue1", "疲劳下降率1");
            XElement fatigueEle2 = CreateParamElement("fatigue2", "疲劳下降率2");
            XElement momentRatioEle = CreateParamElement("momentratio", "峰值力矩比值");
            XElement actingRatioEle = CreateParamElement("actingratio", "做功比值");
            XElement powerRatioEle = CreateParamElement("powerratio", "功率比值");

            actionEle1.Add(momentEle, maxAngleEle, relativeMomentEle, actingEle, relativeactingEle, maxPowerEle, maxrelativepowerEle, endAngleEle, fiftyMomentEle, hundredMomentEle, slopeEle1, tweentypercenttimeEle, seventypercenttimeEle, slopeEle2, fatigueEle1, fatigueEle2, momentRatioEle, actingRatioEle, powerRatioEle);
            actionEle2.Add(momentEle, maxAngleEle, relativeMomentEle, actingEle, relativeactingEle, maxPowerEle, maxrelativepowerEle, endAngleEle, fiftyMomentEle, hundredMomentEle, slopeEle1, tweentypercenttimeEle, seventypercenttimeEle, slopeEle2, fatigueEle1, fatigueEle2, momentRatioEle, actingRatioEle, powerRatioEle);

            statisticsEle.Add(actionEle1, actionEle2);

            XElement curveEle = new XElement("avgcurve");//平均曲线节点
            rootEle.Add(curveEle);
            XElement oddCurveEle = new XElement("odd");
            XElement evenCurveEle = new XElement("even");
            XElement oddAvgCurveEle = new XElement("oddavg");
            XElement evenAvgCurveEle = new XElement("evenavg");
            curveEle.Add(oddCurveEle);
            curveEle.Add(evenCurveEle);
            curveEle.Add(oddAvgCurveEle);
            curveEle.Add(evenAvgCurveEle);

            xdoc.Add(rootEle);

            Console.WriteLine("Start-----FindStartIndex");
            FindStartIndex();//查找起点
            Console.WriteLine("End-----FindStartIndex");

            Console.WriteLine("Start-----CorrectDirction");
            CorrectDirction();
            Console.WriteLine("End-----CorrectDirction");

            xdoc.Save(absuloteFileName);
            Console.WriteLine("Start-----ComputeParams");
            ComputeParams(absuloteFileName);
            Console.WriteLine("End-----ComputeParams");

            return fileName;
        }

        /// <summary>
        /// 创建xml节点
        /// </summary>
        /// <param name="paramName">参数名称</param>
        /// <param name="remark">备注</param>
        /// <returns></returns>
        private XElement CreateParamElement(string paramName, string remark) {
            XElement ele = new XElement(paramName);
            ele.SetAttributeValue("max", "");
            ele.SetAttributeValue("avg", "");
            ele.SetAttributeValue("remark", remark);
            return ele;
        }

        /// <summary>
        /// 平滑曲线
        /// </summary>
        /// <param name="smoothValue">平滑系数</param>
        /// <param name="baseDataArray">原始数据</param>
        /// <returns></returns>
        private List<string[]> Smooth(int smoothValue, string[] baseDataArray)
        {
            List<string[]> smoothedDataList = new List<string[]>();
            int startIndex = 101;
            int count = baseDataArray.Length - smoothValue;
            for (int i = startIndex; i < count; i++)
            {
                string str = baseDataArray[i];
                str = str.Replace("\t", " ");
                string[] currentDataArray = str.Split(' ');
                if (smoothValue > 0)
                {
                    double sum = 0;
                    for (int j = 0; j < smoothValue; j++)
                    {
                        string ljStr = baseDataArray[i + j].Split(' ')[2];
                        sum += double.Parse(ljStr);
                    }
                    string ljAvgStr = Math.Round(sum / smoothValue).ToString();
                    currentDataArray[2] = ljAvgStr;
                }
                smoothedDataList.Add(currentDataArray);
            }
            return smoothedDataList;
        }

        /// <summary>
        /// 校正数据
        /// </summary>
        private void FindStartIndex() {
            List<XElement> dataEles = xdoc.Descendants("data").ToList<XElement>();

            List<XElement> firstEles = new List<XElement>();
            //把第一次测试提取出来
            for (int i = 0; i < dataEles.Count; i++)
            {
                XElement xe = dataEles[i];
                if (xe.Attribute("c5") != null) {
                    if (xe.Attribute("c5").Value == "1")
                    {
                        firstEles.Add(xe);
                    }
                    else
                    {
                        break;
                    }
                }
               
            }

            #region 
            int startIndex = 0;//开始的索引
            int growCount = 0;//连续增长的次数
            int tempStartIndex = -1;//
            int forCount = firstEles.Count - 1;
            for (int i = 0; i < forCount; i++)
            {
                var beforeDataEle = dataEles.ElementAt(i);
                var nextDataEle = dataEles.ElementAt(i + 1);
                int a = int.Parse(beforeDataEle.Attribute("c3").Value);
                int b = int.Parse(nextDataEle.Attribute("c3").Value);
                if (a >= 0)
                {//1、判断是否大于0
                    int absa = Math.Abs(a);
                    int absb = Math.Abs(b);
                    if (absb > absa)
                    {
                        growCount += 1;
                        if (growCount == 1)
                        {
                            tempStartIndex = i;
                        }
                    }
                    else
                    {
                        growCount = 0;
                        tempStartIndex = -1;
                    }
                    if (growCount == 10)//2、速度连续增长10次
                    {
                        //如果连续增长10次以上，说明已经找到开始测试的点，跳出循环
                        startIndex = tempStartIndex;

                        //找力矩连续增长10次
                        forCount=forCount-startIndex;
                        tempStartIndex = -1;
                        growCount = 0;
                        for (int j = startIndex; j < forCount; j++) {
                            beforeDataEle = dataEles.ElementAt(j);
                            nextDataEle = dataEles[j + 1];
                            a = int.Parse(beforeDataEle.Attribute(momentColumn).Value);
                            b = int.Parse(nextDataEle.Attribute(momentColumn).Value);
                            if (b > a)
                            {
                                growCount += 1;
                                if (growCount == 1)
                                {
                                    tempStartIndex = j;
                                }
                            }
                            else {
                                growCount = 0;
                                tempStartIndex = -1;
                            }
                            if (growCount == 10) {
                                startIndex = tempStartIndex;
                                break;
                            }
                        }
                        break;
                    }
                }
                else
                {
                    growCount = 0;
                    tempStartIndex = -1;
                }
            }
            #endregion
            var datasEle = xdoc.Descendants("datas").ElementAt(0);
            datasEle.Attribute("startindex").Value = startIndex.ToString();
        }

        /// <summary>
        /// 校正方向
        /// </summary>
        private void CorrectDirction() {
            List<XElement> dataEles = xdoc.Descendants("data").ToList<XElement>();
            int allCount = int.Parse(xdoc.Descendants("datas").First<XElement>().Attribute("testcount").Value);//最大测试次数
            int startIndex = int.Parse(xdoc.Descendants("datas").First<XElement>().Attribute("startindex").Value);//起始索引
            List<XElement> firstEles = new List<XElement>();

            List<XElement> zList = new List<XElement>();//正数
            List<XElement> fList = new List<XElement>();//负数
            //把第一次测试提取出来
            for (int i = startIndex; i < dataEles.Count; i++) {
                XElement xe = dataEles[i];
                if (xe.Attribute("c5") != null) {
                    if (xe.Attribute("c5").Value == "1")
                    {
                        firstEles.Add(xe);
                        int momentValue = int.Parse(xe.Attribute(momentColumn).Value);
                        if (momentValue >= 0)
                        {
                            zList.Add(xe);
                        }
                        else {
                            fList.Add(xe);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                
            }

            XElement maxEle = null;

            int currentTestIndex = 1;
            int dirction = 1;//获取第一次测试最大的时候的方向
            if (zList.Count > fList.Count)
            {
                dirction = 1;
                maxEle = zList.OrderByDescending(x => int.Parse(x.Attribute(momentColumn).Value)).ElementAt(0);
            }
            else {
                dirction = 0;
                maxEle = fList.OrderBy(x => int.Parse(x.Attribute(momentColumn).Value)).ElementAt(0);
            }
            int firstMaxIndex = dataEles.IndexOf(maxEle);
            for (int i = firstMaxIndex; i < dataEles.Count; i++) {
                XElement xe = dataEles[i];
                int currentDirction = int.Parse(xe.Attribute(momentColumn).Value) > 0 ? 1 : 0;
                if (currentDirction != dirction) {
                    dirction = currentDirction;
                    currentTestIndex++;
                    for (int j = i; j < 100000; j++) {
                        if (j == (dataEles.Count - 1)) {
                            break;
                        }
                        XElement xe1 = dataEles[j];
                        if (xe1.Attribute("c5").Value == currentTestIndex.ToString()) {
                            i = j;
                            break;
                        }
                        xe1.Attribute("c5").Value = currentTestIndex.ToString();
                    }
                }
                if (currentTestIndex == allCount)
                {
                    break;
                }
            }

            //int dirction = int.Parse(maxEle.Attribute(momentColumn).Value) > 0 ? 1 : 0;//获取第一次测试最大的时候的方向
            //bool dirctionChange = false;
            //string count = "1";
            //for (int i = firstMaxIndex; i < dataEles.Count; i++) {
            //    XElement xe = dataEles[i];
            //    int lastDirction = dirction;
            //    dirction = int.Parse(xe.Attribute(momentColumn).Value) > 0 ? 1 : 0;
            //    if (dirction != lastDirction)
            //    {
            //        dirctionChange = true;
            //        count = (int.Parse(count) + 1).ToString();
            //    }
            //    if (dirctionChange)
            //    {
            //        dirctionChange = false;
            //        for (int j = i; j < 1000 + i; j++)
            //        {
            //            try
            //            {
            //                var ele = dataEles.ElementAt(j);
            //                string currentCount = ele.Attribute("c5").Value;
            //                if (currentCount != count)
            //                {
            //                    ele.Attribute("c5").Value = count;
            //                }
            //                else
            //                {
            //                    i = j;
            //                    break;
            //                }
            //            }
            //            catch {
            //                break;
            //            }
            //        }
            //        if (count == allCount)
            //        {//如果当前的测试次数等于总次数，跳出循环
            //            break;
            //        }
            //    }
            //}
        }

        /// <summary>
        /// 平滑接口
        /// </summary>
        /// <param name="saveFileName">保存数据的路径</param>
        /// <param name="xmlFileName">数据文件路径</param>
        public void Smooth(string xmlFileName,string saveFileName) {
            xdoc = XDocument.Load(xmlFileName);
            XElement datasEle = xdoc.Descendants("datas").ElementAt(0);
            XAttribute smoothValueAtt = datasEle.Attribute("smoothvalue");
            if (smoothValueAtt != null)
            {
                smoothValueAtt.Value = smoothValue.ToString();
            }
            else {
                datasEle.SetAttributeValue("smoothvalue", smoothValue);
            }
          
            List<XElement> dataEleList = datasEle.Elements().ToList<XElement>();
            datasEle.RemoveNodes();

            int startIndex = 0;
            int count = dataEleList.Count - smoothValue;
            for (int i = startIndex; i < count; i++)
            {
                XElement xe = dataEleList[i];

                if (smoothValue > 0)
                {
                    double sum = 0;
                    for (int j = 0; j < smoothValue; j++)
                    {
                        XElement xe1 = dataEleList[i + j];
                        string ljStr = xe1.Attribute(momentColumn).Value;
                        sum += double.Parse(ljStr);
                    }
                    string ljAvgStr = Math.Round(sum / smoothValue).ToString();
                    xe.Attribute(momentColumn).Value = ljAvgStr;
                }
                datasEle.Add(xe);
            }

            CorrectDirction();

            xdoc.Save(saveFileName);
        }

        /// <summary>
        /// 计算参数并写入
        /// </summary>
        /// <param name="xmlFileName">数据xml文件名</param>
        public void ComputeParams(string xmlFileName)
        { 
            xdoc = XDocument.Load(xmlFileName);
            XElement datasEle = xdoc.Descendants("datas").ElementAt(0);
            int count = int.Parse(datasEle.Attribute("testcount").Value);//总测试次数
            int startIndex = int.Parse(datasEle.Attribute("startindex").Value);//起点
            string oddTestNum=xdoc.Descendants("action1").ElementAt(0).Attribute("index").Value;//奇数测试次数
            string evenTestNum = xdoc.Descendants("action2").ElementAt(0).Attribute("index").Value;//偶数测试次数
            string[] oddTestNumArr = oddTestNum.Split(',');
            string[] evenTestNumArr = evenTestNum.Split(',');
            IEnumerable<XElement> dataEles = xdoc.Descendants("data");//所有数据

            List<List<XElement>> oddDict = new List<List<XElement>>();//奇数
            List<List<XElement>> evenDict = new List<List<XElement>>();//偶数

            List<XElement> oddMaxMomentList = new List<XElement>();//奇数每次最大
            List<XElement> evenMaxMomentList = new List<XElement>();//偶数每次最大

            if (oddTestNumArr.Contains("1")) {
                List<XElement> firstEles = (from x in dataEles where x.Attribute("c5").Value == "1" select x).ToList<XElement>();
                firstEles.RemoveRange(0, startIndex);
                oddDict.Add(firstEles);
                var maxEle = firstEles.OrderByDescending(x => Math.Abs(int.Parse(x.Attribute(momentColumn).Value))).ElementAt(0);
                oddMaxMomentList.Add(maxEle);
            }
             
            for (int i = 2; i <= count; i++)
            {
                List<XElement> ixe = (from x in dataEles where x.Attribute("c5").Value == i.ToString() select x).ToList<XElement>();
                if (ixe.Count > 0) {
                    var maxEle = ixe.OrderByDescending(x => Math.Abs(int.Parse(x.Attribute(momentColumn).Value))).ElementAt(0);
                    if (i % 2 == 0)//偶数次数
                    {
                        if (evenTestNumArr.Contains((i / 2).ToString()))
                        {
                            evenDict.Add(ixe);
                            evenMaxMomentList.Add(maxEle);
                        }
                    }
                    else
                    {//奇数次数
                        if (oddTestNumArr.Contains(((i + 1) / 2).ToString()))
                        {
                            oddDict.Add(ixe);
                            oddMaxMomentList.Add(maxEle);
                        }
                    }
                }
            }

            IEnumerable<XElement> paramEles;

            var momentMaxEle1 = oddMaxMomentList.OrderByDescending(x => Math.Abs(int.Parse(x.Attribute(momentColumn).Value))).ElementAt(0);//动作1力矩最大的节点
            double oddMaxMoment = Math.Round(double.Parse(momentMaxEle1.Attribute(momentColumn).Value) * 0.1, 1);
            double oddAvgMoment = Math.Round(oddMaxMomentList.Average(x => Math.Abs(int.Parse(x.Attribute(momentColumn).Value))) * 0.1, 1);

            string evenMaxMoment = "";
            string evenAvgMoment = "";
            XElement momentMaxEle2 = null;
            if (evenMaxMomentList.Count > 0) {
                momentMaxEle2 = evenMaxMomentList.OrderByDescending(x => Math.Abs(int.Parse(x.Attribute(momentColumn).Value))).ElementAt(0);//动作2力矩最大的节点
                evenMaxMoment = Math.Round(Math.Abs(double.Parse(momentMaxEle2.Attribute(momentColumn).Value)) * 0.1, 1).ToString();
                evenAvgMoment = Math.Round(evenMaxMomentList.Average(x => Math.Abs(int.Parse(x.Attribute(momentColumn).Value))) * 0.1, 1).ToString();
            }
        
            //计算峰值力矩
            GetElementAndSetAllAttribute("moment", oddMaxMoment, oddAvgMoment, evenMaxMoment, evenAvgMoment);
            Console.WriteLine("计算峰值力矩");

            //峰力矩时关节角
            double oddMaxAngle = double.Parse(momentMaxEle1.Attribute("c1").Value) * 0.1;
            double oddAvgAngle = Math.Round(oddMaxMomentList.Average(x => Math.Abs(int.Parse(x.Attribute("c1").Value))) * 0.1, 1);
            string evenMaxAngle = "";
            string evenAvgAngle = "";
            if (evenMaxMomentList.Count > 0) {
                evenMaxAngle = (double.Parse(momentMaxEle2.Attribute("c1").Value) * 0.1).ToString();
                evenAvgAngle = Math.Round(evenMaxMomentList.Average(x => Math.Abs(int.Parse(x.Attribute("c1").Value))) * 0.1, 1).ToString();
            }
         
            GetElementAndSetAllAttribute("maxangle", oddMaxAngle, oddAvgAngle, evenMaxAngle, evenAvgAngle);

            Console.WriteLine("峰力矩时关节角");

            //相对峰力矩
            if (weight != "") {
                double dweight = double.Parse(weight);
                if (evenMaxMomentList.Count > 0)
                {
                    GetElementAndSetAllAttribute("relativemoment",
                 Math.Round(oddMaxMoment / dweight, 2),
                 Math.Round(oddAvgMoment / dweight, 2),
                 Math.Round(double.Parse(evenMaxMoment) / dweight, 2),
                 Math.Round(double.Parse(evenAvgMoment) / dweight, 2)
                 );
                }
                else {
                    GetElementAndSetAllAttribute("relativemoment", Math.Round(oddMaxMoment / dweight, 2), Math.Round(oddAvgMoment / dweight, 2), "", "");
                }
             
            }
            Console.WriteLine("相对峰力矩");

            //做功
            List<List<double>> oddactingAndPower = ComputeActing(oddDict);
            List<double> oddActing = oddactingAndPower[0];//奇数每次做功
            double oddMaxActing = oddActing.Max();
            double oddAvgActing = Math.Round(oddActing.Average(), 1);
            string evenMaxActing = "";
            string evenAvgActing = "";
            List<List<double>> evenActingAndPower = null;
            if (evenMaxMomentList.Count > 0) {
                evenActingAndPower = ComputeActing(evenDict);
                List<double> evenActing = evenActingAndPower[0];//偶数每次做功
                evenMaxActing = evenActing.Max().ToString();
                evenAvgActing = Math.Round(evenActing.Average(), 1).ToString();
            }

            GetElementAndSetAllAttribute("acting", oddMaxActing, oddAvgActing, evenMaxActing, evenAvgActing);

            Console.WriteLine("做功");

            //相对做功
            if (weight != "") {
                double dweight = double.Parse(weight);
                if (evenMaxMomentList.Count > 0)
                {
                    GetElementAndSetAllAttribute("relativeacting",
                        Math.Round(oddMaxActing / dweight, 2)
                        , Math.Round(oddAvgActing / dweight, 2)
                        , Math.Round(double.Parse(evenMaxActing) / dweight, 2)
                        , Math.Round(double.Parse(evenAvgActing) / dweight, 2)
                    );
                }
                else {
                    GetElementAndSetAllAttribute("relativeacting", Math.Round(oddMaxActing / dweight, 2), Math.Round(oddAvgActing / dweight, 2), "", "");
                }
            
            }

            Console.WriteLine("相对做功");

            //峰值功率
            List<double> oddPower = oddactingAndPower[1];//奇数功率
            double oddMaxPower = oddPower.Max();
            double oddAvgPower = Math.Round(oddPower.Average(), 1);
            string evenMaxPower = "";
            string evenAvgPower = "";
            if (evenMaxMomentList.Count > 0) {
                List<double> evenPower = evenActingAndPower[1];//偶数功率
                evenMaxPower = evenPower.Max().ToString();
                evenAvgPower = Math.Round(evenPower.Average(), 1).ToString();
            }

            GetElementAndSetAllAttribute("maxpower", oddMaxPower, oddAvgPower, evenMaxPower, evenAvgPower);

            Console.WriteLine("峰值功率");

            //相对峰值功率
            if (weight != "") {
                double dweight = double.Parse(weight);
                if (evenMaxMomentList.Count > 0)
                {
                    GetElementAndSetAllAttribute("maxrelativepower",
                        Math.Round(oddMaxPower / dweight, 2)
                        , Math.Round(oddAvgPower / dweight, 2)
                        , Math.Round(double.Parse(evenMaxPower) / dweight, 2)
                        , Math.Round(double.Parse(evenAvgPower) / dweight, 2)
                        );
                }
                else {
                    GetElementAndSetAllAttribute("maxrelativepower",
                       Math.Round(oddMaxPower / dweight, 2)
                       , Math.Round(oddAvgPower / dweight, 2)
                       , ""
                       , ""
                       );
                }
            }
            Console.WriteLine("相对峰值功率");

            //终点角度
            List<double> oddEndAngles = GetEndAngle(oddDict);
            if (evenMaxMomentList.Count > 0)
            {
                List<double> evenEndAngles = GetEndAngle(evenDict);
                GetElementAndSetAllAttribute("endangle", oddEndAngles.Max(), Math.Round(oddEndAngles.Average(), 1)
                    , evenEndAngles.Max(), Math.Round(evenEndAngles.Average(), 2));
            }
            else
            {
                GetElementAndSetAllAttribute("endangle", oddEndAngles.Max(), Math.Round(oddEndAngles.Average(), 1)
                  , "", "");
            }

            Console.WriteLine("终点角度");

            //慢速测试前50ms屈肌力矩
            List<double> oddFiftyMaxMomentList = GetMaxMomentInSomeTime(oddDict, 50);
            if (evenMaxMomentList.Count > 0)
            {
                List<double> evenFiftyMaxMomentList = GetMaxMomentInSomeTime(evenDict, 50);
                GetElementAndSetAllAttribute("fiftymoment", oddFiftyMaxMomentList.Max(), Math.Round(oddFiftyMaxMomentList.Average(), 1),
                    evenFiftyMaxMomentList.Max(), Math.Round(evenFiftyMaxMomentList.Average(), 1));
            }
            else {
                GetElementAndSetAllAttribute("fiftymoment", oddFiftyMaxMomentList.Max(), Math.Round(oddFiftyMaxMomentList.Average(), 1),
                  "", "");
            }

            Console.WriteLine("慢速测试前50ms屈肌力矩");

            //慢速测试前100ms屈肌力矩
            List<double> oddHundredMaxMomentList = GetMaxMomentInSomeTime(oddDict, 100);
            if (evenMaxMomentList.Count > 0)
            {
                List<double> evenHundredMaxMomentList = GetMaxMomentInSomeTime(evenDict, 100);
                GetElementAndSetAllAttribute("hundredmoment", oddHundredMaxMomentList.Max(), Math.Round(oddHundredMaxMomentList.Average(), 1),
                    evenHundredMaxMomentList.Max(), Math.Round(evenHundredMaxMomentList.Average(), 1));
            }
            else {
                GetElementAndSetAllAttribute("hundredmoment", oddHundredMaxMomentList.Max(), Math.Round(oddHundredMaxMomentList.Average(), 1),
                  "", "");
            }

            Console.WriteLine("慢速测试前100ms屈肌力矩");

            //50-100ms屈肌力矩斜率
            //double oddSlope1 = GetSlope(oddFiftyMaxMomentList,oddHundredMaxMomentList);
            //double evenSlope1 = GetSlope(evenFiftyMaxMomentList, evenHundredMaxMomentList);
            List<double> oddSlopeList = GetFiftyToHunderSlope(oddDict);
            if (evenMaxMomentList.Count > 0)
            {
                List<double> evenSlopeList = GetFiftyToHunderSlope(evenDict);
                GetElementAndSetAllAttribute("slope1", oddSlopeList.Max().ToString(), Math.Round(oddSlopeList.Average(), 2),
                    evenSlopeList.Max(), Math.Round(evenSlopeList.Average(), 2));
            }
            else {
                GetElementAndSetAllAttribute("slope1", oddSlopeList.Max().ToString(), Math.Round(oddSlopeList.Average(), 2),
                   "", "");
            }

            Console.WriteLine("50-100ms屈肌力矩斜率");

            //达到最大力矩20%所用时间
            List<double> oddTweentyTimeList = GetTimeWhenMomentOnMaxMomentPercent(oddDict, 0.2, oddMaxMomentList);
            if (evenMaxMomentList.Count > 0)
            {
                List<double> evenTweentyTimeList = GetTimeWhenMomentOnMaxMomentPercent(evenDict, 0.2, evenMaxMomentList);
                GetElementAndSetAllAttribute("tweentypercenttime", oddTweentyTimeList.Min(), Math.Round(oddTweentyTimeList.Average(), 1), evenTweentyTimeList.Min(), Math.Round(evenTweentyTimeList.Average(), 1));
            }
            else {
                GetElementAndSetAllAttribute("tweentypercenttime", oddTweentyTimeList.Min(), Math.Round(oddTweentyTimeList.Average(), 1), "", "");
            }
            

            Console.WriteLine("达到最大力矩20%所用时间");

            //达到最大力矩70%所用时间
            List<double> oddSeventyTimeList = GetTimeWhenMomentOnMaxMomentPercent(oddDict, 0.7, oddMaxMomentList);
            if (evenMaxMomentList.Count > 0)
            {
                List<double> evenSeventyTimeList = GetTimeWhenMomentOnMaxMomentPercent(evenDict, 0.7, evenMaxMomentList);
                GetElementAndSetAllAttribute("seventypercenttime", oddSeventyTimeList.Min(), Math.Round(oddSeventyTimeList.Average(), 2), evenSeventyTimeList.Min(), Math.Round(evenSeventyTimeList.Average(), 2));
            }
            else {
                GetElementAndSetAllAttribute("seventypercenttime", oddSeventyTimeList.Min(), Math.Round(oddSeventyTimeList.Average(), 2), "", "");
            }

            Console.WriteLine("达到最大力矩70%所用时间");

            //20-70%最大力矩的斜率
            //double oddSlope2 = GetSlope(oddTweentyTimeList, oddSeventyTimeList);
            //double evenSlope2 = GetSlope(evenTweentyTimeList, evenSeventyTimeList);
            List<double> oddPercentSlopeList = GetTweentyToSeventyPercentSlope(oddDict, oddMaxMomentList);
            if (evenMaxMomentList.Count > 0)
            {
                List<double> evenPercentSlopeList = GetTweentyToSeventyPercentSlope(evenDict, evenMaxMomentList);
                GetElementAndSetAllAttribute("slope2", oddPercentSlopeList.Max(), Math.Round(oddPercentSlopeList.Average(), 2), evenPercentSlopeList.Max(), Math.Round(evenPercentSlopeList.Average(), 2));
            }
            else {
                GetElementAndSetAllAttribute("slope2", oddPercentSlopeList.Max(), Math.Round(oddPercentSlopeList.Average(), 2), "","");
            }

            Console.WriteLine("20-70%最大力矩的斜率");

            int oddTestCount = oddTestNum.Split(',').Length;
            int evenTestCount = evenTestNum.Split(',').Length;
            //疲劳下降率1(达到25次才计算)
            string fatigue1 = "";
            string fatigue2 = "";
            if (oddTestCount >= 25) {
                fatigue1 = GetFatigue1(oddMaxMomentList);

            }
            if (evenTestCount >= 25) {
                fatigue2 = GetFatigue1(evenMaxMomentList);
            }
            GetElementAndSetAllAttribute("fatigue1", fatigue1, "", fatigue2, "");

            Console.WriteLine("疲劳下降率1(达到25次才计算)");

            //疲劳下降率2（达到10次才计算）
            string oddFatigueSlope = "";
            string evenFatigueSlope = "";
            if (oddTestCount >= 10) {
                oddFatigueSlope = GetSlope(oddMaxMomentList).ToString();
            }
            if (evenTestCount >= 10) {
                evenFatigueSlope = GetSlope(evenMaxMomentList).ToString();
            }
            GetElementAndSetAllAttribute("fatigue2", oddFatigueSlope, "", evenFatigueSlope, "");

            //峰值力矩比值
            if (evenMaxMomentList.Count > 0)
            {
                double momentratio1 = Math.Round(oddMaxMoment / double.Parse(evenMaxMoment), 2);
                double momentratio2 = Math.Round(double.Parse(evenMaxMoment) / oddMaxMoment, 2);
                double momentratioavg1 = Math.Round(oddAvgMoment / double.Parse(evenAvgMoment), 2);
                double momentratioavg2 = Math.Round(double.Parse(evenAvgMoment) / oddAvgMoment, 2);
                GetElementAndSetAllAttribute("momentratio", momentratio1, momentratioavg1, momentratio2, momentratioavg2);
            }
            else {
                GetElementAndSetAllAttribute("momentratio", "", "", "", "");
            }

            Console.WriteLine("峰值力矩比值");

            //做功比值
            if (evenMaxMomentList.Count > 0)
            {
                double actingratio1 = Math.Round(oddMaxActing / double.Parse(evenMaxActing), 2);
                double actingratio2 = Math.Round(double.Parse(evenMaxActing) / oddMaxActing, 2);
                double actingratioavg1 = Math.Round(oddAvgActing / double.Parse(evenAvgActing), 2);
                double actingratioavg2 = Math.Round(double.Parse(evenAvgActing) / oddAvgActing, 2);
                GetElementAndSetAllAttribute("actingratio", actingratio1, actingratioavg1, actingratio2, actingratioavg2);
            }
            else {
                GetElementAndSetAllAttribute("actingratio", "", "", "", "");
            }

            Console.WriteLine("做功比值");

            //功率比值
            paramEles = xdoc.Descendants("powerratio");
            if (evenMaxMomentList.Count > 0)
            {
                double powerratio1 = Math.Round(oddMaxPower / double.Parse(evenMaxPower), 2);
                double powerratio2 = Math.Round(double.Parse(evenMaxPower) / oddMaxPower, 2);
                double powerratioavg1 = Math.Round(oddAvgPower / double.Parse(evenAvgPower), 2);
                double powerratioavg2 = Math.Round(double.Parse(evenAvgPower) / oddAvgPower, 2);
                GetElementAndSetAllAttribute("powerratio", powerratio1, powerratioavg1, powerratio2, powerratioavg2);
            }
            else
            {
                GetElementAndSetAllAttribute("powerratio", "", "", "", "");
            }

            Console.WriteLine("功率比值");

            //计算平均曲线
         
            XElement oddEle = xdoc.Descendants("odd").ElementAt(0) ;
            XElement evenEle = xdoc.Descendants("even").ElementAt(0);
            XElement oddAvgEle = xdoc.Descendants("oddavg").ElementAt(0);
            XElement evenAvgEle = xdoc.Descendants("evenavg").ElementAt(0);
            oddEle.RemoveAll();
            evenEle.RemoveAll();
            oddAvgEle.RemoveAll();
            evenAvgEle.RemoveAll();

            List<List<double>> oddAvgCurveList = ComputeAvgCurve.Compute(oddDict);
            List<double> oddAvgCurve = ComputeAvgCurve.ComputeAvg(oddAvgCurveList);
            WriteAvgEle(oddEle, oddAvgCurveList);
            WriteAvgEle(oddAvgEle, oddAvgCurve);
            if (evenMaxMomentList.Count > 0) {
                List<List<double>> evenAvgCurveList = ComputeAvgCurve.Compute(evenDict);
                List<double> evenAvgCurve = ComputeAvgCurve.ComputeAvg(evenAvgCurveList);
                WriteAvgEle(evenEle, evenAvgCurveList);
                WriteAvgEle(evenAvgEle, evenAvgCurve);
            }
            xdoc.Save(xmlFileName);
        }

        private void WriteAvgEle(XElement xe,List<List<double>> list) {
            for (int i = 0; i < list.Count; i++) {
                List<double> valueList = list[i];
                XElement indexEle = new XElement("index" + i);
                WriteAvgEle(indexEle, valueList);
                xe.Add(indexEle);
            }
        }

        private void WriteAvgEle(XElement xe, List<double> list) {
            for (int j = 0; j < list.Count; j++)
            {
                XElement valueEle = new XElement("node");
                valueEle.SetAttributeValue("c2", list[j].ToString());
                xe.Add(valueEle);
            }
        }

        #region 计算方法
        /// <summary>
        /// 获取参数节点并设置其属性
        /// </summary>
        /// <param name="eleName">节点名</param>
        /// <param name="oddMaxValue">奇数最大值</param>
        /// <param name="oddAvgValue">奇数平均值</param>
        /// <param name="evenMaxValue">偶数最大值</param>
        /// <param name="evenAvgValue">偶数平均值</param>
        private void GetElementAndSetAllAttribute(string eleName, object oddMaxValue, object oddAvgValue, object evenMaxValue, object evenAvgValue)
        {
            IEnumerable<XElement> paramEles = xdoc.Descendants(eleName);
            paramEles.ElementAt(0).Attribute("max").Value = oddMaxValue.ToString();
            paramEles.ElementAt(0).Attribute("avg").Value = oddAvgValue.ToString();
            paramEles.ElementAt(1).Attribute("max").Value = evenMaxValue.ToString();
            paramEles.ElementAt(1).Attribute("avg").Value = evenAvgValue.ToString();
        }

        /// <summary>
        /// 计算做功
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        private List<List<double>> ComputeActing(List<List<XElement>> dict)
        {
            List<double> actings=new List<double>();
            List<double> powers = new List<double>();
            foreach (List<XElement> ixe in dict)
            {
                double acting = 0;
                int allCount = ixe.Count();
                for (int a = 0; a < allCount; a++)
                {
                    if (a != (ixe.Count() - 1))
                    {
                        XElement currentEle = ixe.ElementAt(a);
                        XElement nextEle = ixe.ElementAt(a + 1);
                        double startMoment = double.Parse(currentEle.Attribute(momentColumn).Value)*0.1;
                        double startAngle = double.Parse(currentEle.Attribute("c1").Value)*0.1;
                        double endAngle = double.Parse(nextEle.Attribute("c1").Value)*0.1;
                        acting += Math.Abs(startMoment * ((2 * Math.PI) / 360) * (endAngle - startAngle));
                    }
                }
                acting = Math.Round(acting, 1);
                actings.Add(acting);

                double startTime = double.Parse(ixe.ElementAt(0).Attribute("c0").Value)*0.001;
                double endTime = double.Parse(ixe.ElementAt(ixe.Count - 1).Attribute("c0").Value)*0.001;
                double power = Math.Round(acting / (endTime - startTime), 1);
                powers.Add(power);
            }
            return new List<List<double>>() { actings, powers };
        }

        /// <summary>
        /// 获取终点角度
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        private List<double> GetEndAngle(List<List<XElement>> dict) {
            List<double> endAngles = new List<double>();
            foreach (List<XElement> xe in dict) {
                double endAngle = double.Parse(xe.ElementAt(xe.Count - 1).Attribute("c1").Value);
                endAngles.Add(Math.Round(endAngle * 0.1, 1));
            }
            return endAngles;
        }

        /// <summary>
        /// 获取一定时间内的最大力矩
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        private List<double> GetMaxMomentInSomeTime(List<List<XElement>> dict,int time) {
            List<double> maxMoments = new List<double>();
            int maxIndex = time / 5;
            foreach (List<XElement> xe in dict) {
                List<XElement> xeList = new List<XElement>();
                if (xe.Count >= maxIndex)//如果行数大于最大的索引，则计算，否则添加为0
                {
                    for (int i = 0; i < maxIndex; i++)
                    {
                        try
                        {
                            xeList.Add(xe[i]);
                        }
                        catch
                        {
                            break;
                        }
                    }
                    var eles = xeList.OrderByDescending(x => Math.Abs(double.Parse(x.Attribute(momentColumn).Value)));
                    var maxEle = eles.ElementAt(0);

                    double maxMoment = Math.Abs(double.Parse(maxEle.Attribute(momentColumn).Value));
                    //Math.Abs(double.Parse(xeList.OrderByDescending(x => Math.Abs(double.Parse(x.Attribute(momentColumn).Value))).ElementAt(0).Attribute(momentColumn).Value));
                    maxMoments.Add(maxMoment * 0.1);
                }
                else {
                    maxMoments.Add(0);
                }
            }
            return maxMoments;
        }

        /// <summary>
        /// 获取达到最大力矩百分比的时间点
        /// </summary>
        /// <param name="dict">按测试次数分类的数据数组</param>
        /// <param name="percent">百分比</param>
        /// <param name="maxMoment">最大的力矩</param>
        /// <returns></returns>
        private List<double> GetTimeWhenMomentOnMaxMomentPercent(List<List<XElement>> dict, double percent,List<XElement> maxMomentList) {
            List<double> timeList = new List<double>();
            //double moment = maxMoment * percent;
            for (int i = 0; i < dict.Count; i++) {
                List<XElement> xe = dict.ElementAt(i);
                double startTime = double.Parse(xe.ElementAt(0).Attribute("c0").Value);
                double moment = Math.Abs(double.Parse(maxMomentList[i].Attribute(momentColumn).Value) * percent);
                foreach (XElement ele in xe)
                {
                    double m = Math.Abs(double.Parse(ele.Attribute(momentColumn).Value));
                    if (m >= moment)
                    {
                        double endTime = double.Parse(ele.Attribute("c0").Value);
                        timeList.Add((endTime - startTime));
                        break;
                    }
                }
            }
            return timeList;
        }

        /// <summary>
        /// 获取疲劳下降率1
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private string GetFatigue1(List<XElement> list) {
            List<double> list1 = new List<double>();
            List<double> list2 = new List<double>();
            for (int i = 0; i < 5; i++) {
                double d1 = double.Parse(list[i].Attribute(momentColumn).Value);
                double d2 = double.Parse(list[20 + i].Attribute(momentColumn).Value);
                list1.Add(d1);
                list2.Add(d2);
            }
            double avg1=list1.Average();
            double avg2=list2.Average();
            string fatigue = (Math.Round((avg1 - avg2) / avg1, 4) * 100).ToString() + "%";
            return fatigue;
        }

        /// <summary>
        /// 获取斜率
        /// </summary>
        /// <param name="smallList"></param>
        /// <param name="bigList"></param>
        /// <returns></returns>
        //private double GetSlope(List<double> smallList, List<double> bigList)
        //{
        //    double[] index = new double[smallList.Count];
        //    double[] value = new double[smallList.Count];
        //    for (int i = 0; i < smallList.Count; i++)
        //    {
        //        index[i] = i + 1;
        //        value[i] = (bigList[i] - smallList[i]) / 50 * 0.1;
        //    }
        //    return SlopeFunc(value, index);

        //}

        private List<double> GetFiftyToHunderSlope(List<List<XElement>> dict) {
            List<double> list = new List<double>();
            foreach (List<XElement> xeList in dict) {
                List<double> valueList = new List<double>();
                List<double> indexList = new List<double>();
                if (xeList.Count >= 20) {
                    for (int i = 9; i < 20; i++)
                    {
                        try
                        {
                            indexList.Add(double.Parse(xeList[i].Attribute("c0").Value));
                            valueList.Add(Math.Abs(double.Parse(xeList[i].Attribute(momentColumn).Value)));
                        }
                        catch
                        {
                            break;
                        }
                    }
                    double slope = Math.Round(SlopeFunc(valueList.ToArray(), indexList.ToArray()), 2);
                    list.Add(slope);
                }
               
            }
            return list;
        }

        private List<double> GetTweentyToSeventyPercentSlope(List<List<XElement>> dict,List<XElement> maxMomentList) {
            List<double> list = new List<double>();
            for (int i = 0; i < dict.Count; i++)
            {
                List<double> valueList = new List<double>();
                List<double> indexList = new List<double>();

                List<XElement> xeList=dict[i];
                double tp = Math.Abs(double.Parse(maxMomentList[i].Attribute(momentColumn).Value) * 0.2);
                double sp = Math.Abs(double.Parse(maxMomentList[i].Attribute(momentColumn).Value) * 0.7);

                List<XElement> resultList = new List<XElement>();
                foreach (XElement xe in xeList) {
                    double moment = Math.Abs(double.Parse(xe.Attribute(momentColumn).Value));
                    if (moment >= tp && moment <= sp) {
                        valueList.Add(Math.Abs(double.Parse(xe.Attribute(momentColumn).Value)));
                        indexList.Add(double.Parse(xe.Attribute("c0").Value));
                    }
                    if (int.Parse(xe.Attribute("c0").Value) == int.Parse(maxMomentList[i].Attribute("c0").Value)) {
                        break;
                    }
                }
                if (valueList.Count > 0)
                {
                    double slope = Math.Round(SlopeFunc(valueList.ToArray(), indexList.ToArray()), 2);
                    list.Add(slope);
                }
                else {
                    list.Add(0);
                }
            }
            return list;
        }

        /// <summary>
        /// 获取斜率，用于计算疲劳下降率2
        /// </summary>
        /// <param name="xeList"></param>
        /// <returns></returns>
        private double GetSlope(List<XElement> xeList)
        {
            double[] index = new double[xeList.Count];
            double[] value = new double[xeList.Count];
            for (int i = 0; i < xeList.Count; i++)
            {
                index[i] = double.Parse(xeList[i].Attribute("c0").Value);
                value[i] = double.Parse(xeList[i].Attribute(momentColumn).Value);
            }
            return SlopeFunc(value, index);
        }

        public double SlopeFunc(double[] value, double[] index)
        {
            double[] d1 = value;
            double[] d2 = index;

            double x = d1.Average();
            double y = d2.Average();

            double a = 0;
            double b = 0;
            for (int i = 0; i < value.Length; i++)
            {
                a += ((d2[i] - y) * (d1[i] - x));
                b += Math.Pow((d2[i] - y), 2);
            }

            double c = Math.Round(a / b, 2);
            return c;
        }
        #endregion

        #region testcode
        public void Test() {
            Console.WriteLine("-----------start testcode");

            //XDocument xdocument = new XDocument();
            //XElement rootEle = new XElement("root");
            //XElement datasEle = new XElement("datas");
            //datasEle.SetAttributeValue("testcount", 50);
            //datasEle.SetAttributeValue("startindex", "0");

            //int forCount = 50;
            //int oneCount = 200;
            //int step = 5;
            //Random rd = new Random();

            ////ApplicationClass excelApp;
            ////Worksheet mySheet1;//工作簿1
            ////object missing = Missing.Value;
            ////string excelFileName = AppDomain.CurrentDomain.BaseDirectory + "\\test.xlsx";
            ////excelApp = new ApplicationClass();
            ////Workbooks myWorkBooks = excelApp.Workbooks;
            ////myWorkBooks.Open(excelFileName, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing);
            ////Sheets sheets = excelApp.Sheets;
            ////mySheet1 = (Worksheet)sheets[1];

            //for (int i = 0; i < forCount; i++) {
            //    for (int j = 1; j <= oneCount; j++)
            //    {
            //        int jd=rd.Next(100, 340);//终止角度随机数
            //        int excelRow = (i * oneCount) + j + 1;

            //        XElement dataEle = new XElement("data");
            //        dataEle.SetAttributeValue("c0", (j * step) + (i * oneCount * step));//时间
            //        //mySheet1.Cells[excelRow, 1] = (j * step) + (i * oneCount * step);
                 
            //        dataEle.SetAttributeValue("c1", jd);//终止角度
            //        //mySheet1.Cells[excelRow, 2] = jd;
            //        int lj = 0;
            //        if (j <= 100)
            //        {
            //            lj = j;
            //        }
            //        else {
            //            lj = 100 - (j - 100);
            //        }

            //        if ((i+1) % 2 == 0)
            //        {
            //            dataEle.SetAttributeValue(momentColumn, -lj-i);//力矩
            //            //mySheet1.Cells[excelRow, 3] = -lj - i;
            //        }
            //        else {
            //            dataEle.SetAttributeValue(momentColumn, lj+i);//力矩
            //            //mySheet1.Cells[excelRow, 3] = lj + i;
            //        }
            //        dataEle.SetAttributeValue("c3", "");
            //        //mySheet1.Cells[excelRow, 4] = "";
            //        dataEle.SetAttributeValue("c4", "");
            //        //mySheet1.Cells[excelRow, 5] = "";
            //        dataEle.SetAttributeValue("c5", i+1);//组数
            //        //mySheet1.Cells[excelRow, 6] = i+1;
            //        dataEle.SetAttributeValue("c6", "");
            //        //mySheet1.Cells[excelRow, 7] = "";
            //        dataEle.SetAttributeValue("c7", "");
            //        //mySheet1.Cells[excelRow, 8] = "";

            //        datasEle.Add(dataEle);
            //    }
            //}
            //////myWorkBooks.Close();
            //////excelApp.Quit();
            //////excelApp = null;

            //rootEle.Add(datasEle);

            //XElement statisticsEle = new XElement("statistics");//统计节点
            //rootEle.Add(statisticsEle);

            //XElement actionEle1 = new XElement("action1");
            //actionEle1.SetAttributeValue("index", "1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25");
            //XElement actionEle2 = new XElement("action2");
            //actionEle2.SetAttributeValue("index", "1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25");

            //XElement momentEle = CreateParamElement("moment", "峰值力矩(N·m)");
            //XElement maxAngleEle = CreateParamElement("maxangle", "峰力矩时关节角(°)");
            //XElement relativeMomentEle = CreateParamElement("relativemoment", "相对峰力矩(Nm/Kg)");
            //XElement actingEle = CreateParamElement("acting", "做功(J)");
            //XElement relativeactingEle = CreateParamElement("relativeacting", "相对做功(J/Kg)");
            //XElement maxPowerEle = CreateParamElement("maxpower", "峰值功率(W)");
            //XElement maxrelativepowerEle = CreateParamElement("maxrelativepower", "峰值相对功率(W/Kg)");
            //XElement endAngleEle = CreateParamElement("endangle", "终点角度(°)");
            //XElement fiftyMomentEle = CreateParamElement("fiftymoment", "慢速测试前50ms力矩(N·m)");
            //XElement hundredMomentEle = CreateParamElement("hundredmoment", "慢速测试前100ms力矩(N·m)");
            //XElement slopeEle1 = CreateParamElement("slope1", "50-100ms力矩斜率(N*m/ms)");
            //XElement tweentypercenttimeEle = CreateParamElement("tweentypercenttime", "达到最大力矩20%所用时间(s)");
            //XElement seventypercenttimeEle = CreateParamElement("seventypercenttime", "达到最大力矩70%所用时间(s)");
            //XElement slopeEle2 = CreateParamElement("slope2", "20-70%最大力矩的斜率(N*m/ms)");
            //XElement fatigueEle1 = CreateParamElement("fatigue1", "疲劳下降率1");
            //XElement fatigueEle2 = CreateParamElement("fatigue2", "疲劳下降率2");
            //XElement momentRatioEle = CreateParamElement("momentratio", "峰值力矩比值");
            //XElement actingRatioEle = CreateParamElement("actingratio", "做功比值");
            //XElement powerRatioEle = CreateParamElement("powerratio", "功率比值");

            //actionEle1.Add(momentEle, maxAngleEle, relativeMomentEle, actingEle, relativeactingEle, maxPowerEle, maxrelativepowerEle, endAngleEle, fiftyMomentEle, hundredMomentEle, slopeEle1, tweentypercenttimeEle, seventypercenttimeEle, slopeEle2, fatigueEle1, fatigueEle2, momentRatioEle, actingRatioEle, powerRatioEle);
            //actionEle2.Add(momentEle, maxAngleEle, relativeMomentEle, actingEle, relativeactingEle, maxPowerEle, maxrelativepowerEle, endAngleEle, fiftyMomentEle, hundredMomentEle, slopeEle1, tweentypercenttimeEle, seventypercenttimeEle, slopeEle2, fatigueEle1, fatigueEle2, momentRatioEle, actingRatioEle, powerRatioEle);

            //statisticsEle.Add(actionEle1, actionEle2);

            //XElement curveEle = new XElement("avgcurve");//平均曲线节点
            //rootEle.Add(curveEle);
            //XElement oddCurveEle = new XElement("odd");
            //XElement evenCurveEle = new XElement("even");
            //XElement oddAvgCurveEle = new XElement("oddavg");
            //XElement evenAvgCurveEle = new XElement("evenavg");
            //curveEle.Add(oddCurveEle);
            //curveEle.Add(evenCurveEle);
            //curveEle.Add(oddAvgCurveEle);
            //curveEle.Add(evenAvgCurveEle);

            //xdocument.Add(rootEle);

            string fileName = "test.xml";
            //xdocument.Save(fileName);

            weight = "50";
            ComputeParams(fileName);

            Console.WriteLine("-----------end testcode");
        }
        #endregion
    }
}
