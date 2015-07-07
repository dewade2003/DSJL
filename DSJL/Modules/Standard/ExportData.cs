using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Office.Interop.Excel;
using System.Reflection;
using DSJL.Model;

namespace DSJL.Modules.Standard
{
    class ExportData
    {
        private static readonly string templatePath = Model.AppPath.RootPath + "\\AppTemplate\\exportdata.xls";
        private string choosePath = "";

        private const int titleRowCount = 2;//excel 头的说明文字的行数
        private const int testInfoColumnCount = 14;//测试信息占用的列数
        private const int oneParamColumnCount = 4;//一个参数需要的列数
        private string[,] paramContents = new string[1, 4];

        private static ApplicationClass excelApp;
        private static Microsoft.Office.Interop.Excel.Worksheet mySheet1;//工作簿1
        private static object missing = Missing.Value;

        public ExportData() {
            paramContents[0, 0] = "动作1(极值)";
            paramContents[0, 1] = "动作1(平均值)";
            paramContents[0, 2] = "动作2(极值)";
            paramContents[0, 3] = "动作2(平均值)";
        }

        public void Export(List<List<XElement>> datalist,string savePath) {

            try
            {
                excelApp = new ApplicationClass();
                Workbooks myWorkBooks = excelApp.Workbooks;
                myWorkBooks.Open(templatePath, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing);
                Sheets sheets = excelApp.Sheets;
                mySheet1 = (Worksheet)sheets[1];
                mySheet1.Activate();

                int rowCount = 1 + 2; ;//总行数

                //写入参数信息
                int paramCount = 0;//参数行数
                List<DSJL.Model.Parameter> paramList = DSJL.Model.Parameter.GetAllParams();
                paramCount = paramList.Count * 4;
                for (int i = 0; i < paramList.Count; i++)
                {
                    Model.Parameter p = paramList[i];

                    Range r = mySheet1.get_Range(mySheet1.Cells[1, testInfoColumnCount + i * 4 + 1], mySheet1.Cells[1, testInfoColumnCount + i * 4 + 4]);
                    r.Merge();
                    r.Value = p.ParamName;
                    r.Font.Bold = true;
                    r.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                    r.VerticalAlignment = XlHAlign.xlHAlignCenter;

                    Range r1 = mySheet1.get_Range(mySheet1.Cells[2, testInfoColumnCount + i * 4 + 1], mySheet1.Cells[2, testInfoColumnCount + i * 4 + 4]);
                    r1.Value2 = paramContents;
                    r1.Font.Bold = true;

                    r1.EntireColumn.AutoFit();
                }

                //写入测试信息
                string[,] content = new string[rowCount, testInfoColumnCount + paramCount];
                //double?[,] paramContent = new double?[rowCount, paramCount];



                BLL.TB_Dict dictBLL = new BLL.TB_Dict();

                int rowIndex = 0;


                //content[rowIndex, 0] ="";//测试顺序
                //content[rowIndex, 1] = model.Ath_Name;//姓名
                //content[rowIndex, 2] = model.TestDate.ToString("yyyy-MM-dd HH:mm");//测试日期
                //content[rowIndex, 3] = model.DJoint;//测试关节
                //content[rowIndex, 4] = model.DJointSide;//测试侧
                //content[rowIndex, 5] = model.DPlane;//运动方式
                //content[rowIndex, 6] = model.DTestMode;//测试模式
                //content[rowIndex, 7] = model.MotionRange;//运动范围
                //content[rowIndex, 8] = model.Speed;//测试速度
                //content[rowIndex, 9] = model.Break;//休息时间
                //content[rowIndex, 10] = model.NOOfSets;//测试组数
                //content[rowIndex, 11] = model.NOOfRepetitions;//重复次数
                //content[rowIndex, 12] = model.DInsuredSide;//受伤测
                //content[rowIndex, 13] = model.DGravitycomp;//重力补偿

                List<XElement> action1 = datalist[0];
                List<XElement> action2 = datalist[1];
                for (int j = 0; j < paramList.Count; j++)
                {
                    int paramOneColumnIndex = j * 4;
                    double p1;
                    if (double.TryParse(action1[paramList[j].Index].Attribute("max").Value, out p1))
                    {
                        //paramContent[rowIndex, paramOneColumnIndex] = p1;
                        mySheet1.Cells[rowIndex + 3, paramOneColumnIndex + testInfoColumnCount + 1] = p1;
                    }
                    double p2;
                    if (double.TryParse(action1[paramList[j].Index].Attribute("avg").Value, out p2))
                    {
                        //paramContent[rowIndex, paramOneColumnIndex + 1] = p2;
                        mySheet1.Cells[rowIndex + 3, paramOneColumnIndex + testInfoColumnCount + 2] = p2;
                    }
                    double p3;
                    if (double.TryParse(action2[paramList[j].Index].Attribute("max").Value, out p3))
                    {
                        //paramContent[rowIndex, paramOneColumnIndex + 2] = p3;
                        mySheet1.Cells[rowIndex + 3, paramOneColumnIndex + testInfoColumnCount + 3] = p3;
                    }
                    double p4;
                    if (double.TryParse(action2[paramList[j].Index].Attribute("avg").Value, out p4))
                    {
                        //paramContent[rowIndex, paramOneColumnIndex + 3] = p4;
                        mySheet1.Cells[rowIndex + 3, paramOneColumnIndex + testInfoColumnCount + 4] = p4;
                    }

                }

                //写入测试信息
                Range range1 = mySheet1.get_Range(mySheet1.Cells[3, 1], mySheet1.Cells[rowCount, testInfoColumnCount]);
                range1.Value2 = content;

                mySheet1.SaveAs(savePath, missing, missing, missing, missing, missing, missing, missing, missing, missing);
                myWorkBooks.Close();
                excelApp.Quit();

            }
            catch (Exception ee)
            {
                throw ee;
            }
            finally {
                DSJL.Model.Parameter.CheckDefault();
            }

        }
    }
}
