using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Office.Interop.Excel;
using System.Reflection;
using Microsoft.Win32;
using System.Xml.Linq;

namespace DSJL.Modules.Test.Export
{
    /// <summary>
    /// ExportProgressPage.xaml 的交互逻辑
    /// </summary>
    public partial class ExportProgressPage : System.Windows.Controls.Page
    {
        private static readonly string templatePath = Model.AppPath.RootPath + "\\AppTemplate\\exportdata.xls";
        private string choosePath = "";

        private const int titleRowCount = 2;//excel 头的说明文字的行数
        private const int testInfoColumnCount = 14;//测试信息占用的列数
        private const int oneParamColumnCount = 4;//一个参数需要的列数
        private string[,] paramContents = new string[1, 4];

        public ExportProgressPage()
        {
            InitializeComponent();

            paramContents[0, 0] = "动作1(极值)";
            paramContents[0, 1] = "动作1(平均值)";
            paramContents[0, 2] = "动作2(极值)";
            paramContents[0, 3] = "动作2(平均值)";
        }

        /// <summary>
        /// 测试信息list
        /// </summary>
        public static List<Model.TestInfoModel> TestInfoList
        {
            get;
            set;
        }

        public static string FileNamePreExt
        {
            get;
            set;
        }

        /// <summary>
        /// 参数list
        /// </summary>
        public static List<Model.Parameter> ParamList
        {
            get;
            set;
        }

        /// <summary>
        /// 是否导出参数
        /// </summary>
        public static bool IsExportParams
        {
            get;
            set;
        }

        private void btnPre_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri(@"Modules/Test/Export/CheckParamsPage.xaml", UriKind.Relative));
        }

        private static ApplicationClass excelApp;
        private static Microsoft.Office.Interop.Excel.Worksheet mySheet1;//工作簿1
        private static object missing = Missing.Value;

        private delegate void UpdateProgressBarDelegate(System.Windows.DependencyProperty dp, Object value);

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            exportProgress.Minimum = 0;
            exportProgress.Maximum = TestInfoList.Count ;
            exportProgress.Value = 0;
            double value = 0;
            UpdateProgressBarDelegate updatePbDelegate = new UpdateProgressBarDelegate(exportProgress.SetValue);
            try
            {
                excelApp = new ApplicationClass();
                Workbooks myWorkBooks = excelApp.Workbooks;
                myWorkBooks.Open(templatePath, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing);
                Sheets sheets = excelApp.Sheets;
                mySheet1 = (Worksheet)sheets[1];
                mySheet1.Activate();

                int rowCount = TestInfoList.Count + 2; ;//总行数

                //写入参数信息
                int paramCount = 0;//参数行数
                if (IsExportParams)
                {
                    paramCount = ParamList.Count * 4;
                    for (int i = 0; i < ParamList.Count; i++)
                    {
                        Model.Parameter p = ParamList[i];

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
                }

                //写入测试信息
                string[,] content = new string[rowCount, testInfoColumnCount + paramCount];
                //double?[,] paramContent = new double?[rowCount, paramCount];


                XDocument xdoc;
                BLL.TB_Dict dictBLL = new BLL.TB_Dict();
                for (int i = 0; i < TestInfoList.Count; i++)
                {
                    int rowIndex = i;
                    Model.TestInfoModel model = TestInfoList[i];

                    content[rowIndex, 0] = string.Format("测试{0}", i + 1);//测试顺序
                    content[rowIndex, 1] = model.Ath_Name;//姓名
                    content[rowIndex, 2] = model.TestDate.ToString("yyyy-MM-dd HH:mm");//测试日期
                    content[rowIndex, 3] = model.DJoint;//测试关节
                    content[rowIndex, 4] = model.DJointSide;//测试侧
                    content[rowIndex, 5] = model.DPlane;//运动方式
                    content[rowIndex, 6] = model.DTestMode;//测试模式
                    content[rowIndex, 7] = model.MotionRange;//运动范围
                    content[rowIndex, 8] = model.Speed;//测试速度
                    content[rowIndex, 9] = model.Break;//休息时间
                    content[rowIndex, 10] = model.NOOfSets;//测试组数
                    content[rowIndex, 11] = model.NOOfRepetitions;//重复次数
                    content[rowIndex, 12] = model.DInsuredSide;//受伤测
                    content[rowIndex, 13] = model.DGravitycomp;//重力补偿
                    if (IsExportParams)
                    {
                        string xmlPath = Model.AppPath.XmlDataDirPath + model.DataFileName;
                        xdoc = XDocument.Load(xmlPath);
                        List<XElement> action1 = xdoc.Descendants("action1").Elements<XElement>().ToList<XElement>();
                        List<XElement> action2 = xdoc.Descendants("action2").Elements<XElement>().ToList<XElement>();
                        for (int j = 0; j < ParamList.Count; j++)
                        {
                            int paramOneColumnIndex = j * 4;
                            double p1;
                            if (double.TryParse(action1[ParamList[j].Index].Attribute("max").Value, out p1)) {
                                //paramContent[rowIndex, paramOneColumnIndex] = p1;
                                mySheet1.Cells[rowIndex + 3, paramOneColumnIndex + testInfoColumnCount + 1] = p1;
                            }
                            double p2;
                            if (double.TryParse(action1[ParamList[j].Index].Attribute("avg").Value, out p2))
                            {
                                //paramContent[rowIndex, paramOneColumnIndex + 1] = p2;
                                mySheet1.Cells[rowIndex + 3, paramOneColumnIndex + testInfoColumnCount + 2] = p2;
                            }
                            double p3;
                            if (double.TryParse(action2[ParamList[j].Index].Attribute("max").Value, out p3))
                            {
                                //paramContent[rowIndex, paramOneColumnIndex + 2] = p3;
                                mySheet1.Cells[rowIndex + 3, paramOneColumnIndex + testInfoColumnCount + 3] = p3;
                            }
                            double p4;
                            if (double.TryParse(action2[ParamList[j].Index].Attribute("avg").Value, out p4))
                            {
                                //paramContent[rowIndex, paramOneColumnIndex + 3] = p4;
                                mySheet1.Cells[rowIndex + 3, paramOneColumnIndex + testInfoColumnCount + 4] = p4;
                            }
                           
                        }
                    }

                    //写进度条
                    value += 1;
                    Dispatcher.Invoke(updatePbDelegate,
                        System.Windows.Threading.DispatcherPriority.Background,
                        new object[] { ProgressBar.ValueProperty, value });
                }
                //写入测试信息
                Range range1 = mySheet1.get_Range(mySheet1.Cells[3, 1], mySheet1.Cells[rowCount, testInfoColumnCount]);
                range1.Value2 = content;
                //写入参数信息
                //Range range2 = mySheet1.get_Range(mySheet1.Cells[3, testInfoColumnCount + 1], mySheet1.Cells[rowCount, testInfoColumnCount + paramCount]);
                //range2.Value2 = paramContent;




                //if (IsExportParams)
                //{
                //    rowCount = TestInfoList.Count + (ParamList.Count + 1) * TestInfoList.Count + 1;//信息行数+信息行数×参数行数+第一行列头信息
                //    paramCount = ParamList.Count + 1;//参数行数加1行参数名
                //}
                //else {
                //    rowCount = TestInfoList.Count + 1;
                //}

                //string[,] content = new string[rowCount, 13];


                //XDocument xdoc;
                //Model.TB_Dict actionModel;
                //BLL.TB_Dict dictBLL = new BLL.TB_Dict();
                //for (int i = 0; i < TestInfoList.Count; i++) {
                //    int rowIndex = i + i * paramCount;
                //    Model.TestInfoModel model = TestInfoList[i];

                //    content[rowIndex, 0] = model.Ath_Name;//姓名
                //    content[rowIndex, 1] = model.TestDate.ToString("yyyy-MM-dd HH:mm");//测试日期
                //    content[rowIndex, 2] = model.DJoint;//测试关节
                //    content[rowIndex, 3] = model.DJointSide;//测试侧
                //    content[rowIndex, 4] = model.DPlane;//运动方式
                //    content[rowIndex, 5] = model.DTestMode;//测试模式
                //    content[rowIndex, 6] = model.MotionRange;//运动范围
                //    content[rowIndex, 7] = model.Speed;//测试速度
                //    content[rowIndex, 8] = model.Break;//休息时间
                //    content[rowIndex, 9] = model.NOOfSets;//测试组数
                //    content[rowIndex, 10] = model.NOOfRepetitions;//重复次数
                //    content[rowIndex, 11] = model.DInsuredSide;//受伤测
                //    content[rowIndex, 12] = model.DGravitycomp;//重力补偿
                //    if (IsExportParams) {
                //        //写入参数信息
                //        actionModel = dictBLL.GetModel(model.Joint, model.Plane, model.Test_Mode);
                //        content[rowIndex + 1, 0] = "所选测试顺序";
                //        content[rowIndex + 1, 1] = "参数";
                //        content[rowIndex + 1, 2] = actionModel.actionone + "(极值)";
                //        content[rowIndex + 1, 3] = actionModel.actionone + "(平均值)";
                //        content[rowIndex + 1, 4] = actionModel.actiontwo + "(极值)";
                //        content[rowIndex + 1, 5] = actionModel.actiontwo + "(平均值)";
                //        string xmlPath = Model.AppPath.XmlDataDirPath + model.DataFileName;
                //        xdoc = XDocument.Load(xmlPath);
                //        List<XElement> action1 = xdoc.Descendants("action1").Elements<XElement>().ToList<XElement>();
                //        List<XElement> action2 = xdoc.Descendants("action2").Elements<XElement>().ToList<XElement>();
                //        for (int j = 0; j < ParamList.Count; j++)
                //        {
                //            content[rowIndex + 1 + j + 1, 0] = "测试" + (i + 1);
                //            content[rowIndex + 1 + j + 1, 1] = ParamList[j].ParamName;
                //            content[rowIndex + 1 + j + 1, 2] = action1[ParamList[j].Index].Attribute("max").Value;
                //            content[rowIndex + 1 + j + 1, 3] = action1[ParamList[j].Index].Attribute("avg").Value;
                //            content[rowIndex + 1 + j + 1, 4] = action2[ParamList[j].Index].Attribute("max").Value;
                //            content[rowIndex + 1 + j + 1, 5] = action2[ParamList[j].Index].Attribute("avg").Value;
                //        }
                //    }

                //    //写进度条
                //    value += 1;
                //    Dispatcher.Invoke(updatePbDelegate,
                //        System.Windows.Threading.DispatcherPriority.Background,
                //        new object[] { ProgressBar.ValueProperty, value });
                //}
                //Range range = mySheet1.get_Range(mySheet1.Cells[2, 1], mySheet1.Cells[rowCount, 13]);
                //range.Value2 = content;
                mySheet1.SaveAs(choosePath, missing, missing, missing, missing, missing, missing, missing, missing, missing);
                myWorkBooks.Close();
                excelApp.Quit();
                MessageBox.Show("导出成功！", "系统信息");
                System.Windows.Window.GetWindow(this).Close();
            }
            catch (Exception ee) {
                MessageBox.Show("导出出错！\r\n错误信息为：" + ee.Message, "系统错误");
            }
        }

        private void btnChoosePath_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "请选择保存文件的路径";
            sfd.DefaultExt = "xls";
            sfd.FileName =FileNamePreExt+ "测试数据导出(" + DateTime.Now.ToString("yyyy-MM-dd") + ")";
            sfd.OverwritePrompt = true;
            sfd.AddExtension = true;
            sfd.Filter = "Excel文件(*.xls)|*.xls";
            if (sfd.ShowDialog() == true)
            {
                choosePath = txtPath.Text = sfd.FileName;
                btnExport.IsEnabled = true;
            }
        }
    }
}
