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
using System.Data;
using DSJL.DBUtility;
using System.IO;
using DSJL.Modules.Test.Export;
using Microsoft.Win32;
using DSJL.Modules.Standard;
using System.Xml.Linq;

namespace DSJL.Modules
{
    /// <summary>
    /// PageStandard.xaml 的交互逻辑
    /// </summary>
    public partial class PageStandard : Page
    {
        List<Model.TestInfoModel> testInfoModelList = new List<Model.TestInfoModel>();
        BLL.TB_TestInfo testInfoBLL = new BLL.TB_TestInfo();
        BLL.TB_AthleteInfo athleteInfoBLL = new BLL.TB_AthleteInfo();

        BLL.TB_Dict dictBLL = new BLL.TB_Dict();

        BLL.TB_StandTestRefe refeBLL;

        DSJL.Modules.Standard.PageAvgCurve pageAvgCurve = new Standard.PageAvgCurve();
        public PageStandard()
        {
            InitializeComponent();
            testInfoModelList = new List<Model.TestInfoModel>();
            refeBLL = new BLL.TB_StandTestRefe();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            pageAvgCurve.Width = 800;
            pageAvgCurve.Height = 300;
            pageAvgCurve.HorizontalAlignment = HorizontalAlignment.Left;
            frame.Navigate(pageAvgCurve);
            ReferenshList();
        }

        private void ReferenshList()
        {
            if (standManager.SelectedItem != null)
            {
                if (standManager.SelectedItem.Stand_Level == 2) {
                    testInfoModelList.Clear();
                    testInfoModelList = refeBLL.GetStandTestInfoModelList(standManager.SelectedItem.ID);
                    Binding b = new Binding() { Source = testInfoModelList };
                    dgTestInfo.SetBinding(DataGrid.ItemsSourceProperty, b);

                    pageAvgCurve.ModelList = testInfoModelList;
                }
                
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            List<Model.TestInfoModel> selectedTestInfoList = testInfoModelList.FindAll(x => x.IsChecked == true);
            if (selectedTestInfoList.Count == 0)
            {
                MessageBox.Show("请至少选择一条信息删除！", "系统信息");
                return;
            }
            if (MessageBox.Show("删除信息将不能恢复，确定删除选择的信息吗？", "系统信息", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    foreach (Model.TestInfoModel model in selectedTestInfoList)
                    {
                        refeBLL.Delete(model.TestID, standManager.SelectedItem.ID);
                    }
                    ReferenshList();
                }
                catch (Exception ee)
                {
                    MessageBox.Show("删除错误！\r\n" + ee.Message, "系统信息");
                }
            }
        }

        private void standManager_ItemSelectionChangedEvent(Model.TB_StandardInfo selectedItem)
        {

            ReferenshList();
        }

        private void dgTestInfo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgTestInfo.SelectedIndex >= 0)
            {

                foreach (Model.TestInfoModel model in testInfoModelList)
                {
                    model.IsChecked = false;
                }
                if (dgTestInfo.SelectedItems.Count > 1)
                {
                    foreach (var item in dgTestInfo.SelectedItems)
                    {
                        Model.TestInfoModel testModel = item as Model.TestInfoModel;
                        testModel.IsChecked = true;
                    }
                }
                else
                {
                    Model.TestInfoModel testModel = dgTestInfo.SelectedItem as Model.TestInfoModel;
                    testModel.IsChecked = true;
                }

                pageAvgCurve.ModelList = testInfoModelList;
            }
        }

        //全选
        private void checkAll_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            if (cb.IsChecked == true)
            {
                foreach (Model.TestInfoModel bi in testInfoModelList)
                {
                    bi.IsChecked = true;
                }
                dgTestInfo.SelectAll();
            }
            else
            {
                foreach (Model.TestInfoModel bi in testInfoModelList)
                {
                    bi.IsChecked = false;
                }
                dgTestInfo.UnselectAll();
            }
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            if (testInfoModelList.Count == 0)
            {
                MessageBox.Show("所选的参考值下没有测试信息，不能导出！", "系统信息");
                return;
            }
            try
            {
                //刷新图表
                pageAvgCurve.ModelList = testInfoModelList;

                string pdfFileName = "";
                string reportTitle = "";
                string standName = "";
                DSJL.Export.ExportModeEnum exportMode = DSJL.Export.ExportModeEnum.Mode1;

                List<Model.TestInfoModel> checkedModel = testInfoModelList.FindAll(x => x.IsChecked == true);

                if (rbMode1.IsChecked == true)
                {
                    //if (checkedModel.Count < 2)
                    //{
                    //    MessageBox.Show("请至少选择两条测试信息进行对比报告导出！", "系统信息");
                    //    return;
                    //}
                    exportMode = DSJL.Export.ExportModeEnum.Mode1;
                    pdfFileName = "等速肌力互相对比报告(" + DateTime.Now.ToString("yyyyMMddHHmmss") + ")";
                    reportTitle = "等速肌力互相对比报告";
                }
                else if (rbMode2.IsChecked == true) {
                    exportMode = DSJL.Export.ExportModeEnum.Mode2;
                    pdfFileName = "等速肌力个人与平均曲线报告(" + DateTime.Now.ToString("yyyyMMddHHmmss") + ")";
                    reportTitle = "等速肌力个人与平均曲线报告";
                }
                else if (rbMode3.IsChecked == true)
                {
                    //if (checkedModel.Count < 1)
                    //{
                    //    MessageBox.Show("请至少选择一条测试信息进行对比报告导出！", "系统信息");
                    //    return;
                    //}
                    if (standManager.SelectedItem == null)
                    {
                        MessageBox.Show("请选择测试参考值！", "系统信息");
                        return;
                    }
                    else
                    {
                        standName = standManager.SelectedItem.Stand_Name;
                    }
                    exportMode = DSJL.Export.ExportModeEnum.Mode3;
                    pdfFileName = "等速肌力个人与参考值对比报告(" + DateTime.Now.ToString("yyyyMMddHHmmss") + ")";
                    reportTitle = "等速肌力个人与参考值对比报告";
                }
                //生成数据xml对象，供导出报告使用
                DSJL.Export.GenerateCompareResportXml garxml = new DSJL.Export.GenerateCompareResportXml(exportMode);
                garxml.CurrentTitle = reportTitle;
                garxml.TestInfoModelList = GetSortedTestInfoModelList();
                garxml.StandardTestInfoModelList = testInfoModelList;
                garxml.StandName = standName;
                System.Xml.Linq.XDocument xdoc = garxml.GenerateXDoc();

                System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
                fbd.Description = "选择保存报告的位置";
                fbd.ShowNewFolderButton = true;
                if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //生成平均曲线图片
                    DSJL.Export.SaveUIElementToImage.SaveToImage(pageAvgCurve, AppDomain.CurrentDomain.BaseDirectory + "avg.jpg");
                    //开始导出
                    DSJL.Export.ExportCompareReport exportReport = new DSJL.Export.ExportCompareReport(xdoc);
                    exportReport.Export(fbd.SelectedPath + "\\" + pdfFileName + ".pdf");

                    //删除平均曲线图片
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "avg.jpg");
                    MessageBox.Show("导出成功！", "系统信息");
                }

            }
            catch (Exception ee)
            {
                MessageBox.Show("导出出错！\r\n" + ee.Message, "系统错误");
            }
        }

        /// <summary>
        ///
        /// </summary>
        private List<Model.TestInfoModel> GetSortedTestInfoModelList()
        {
            List<Model.TestInfoModel> models = new List<Model.TestInfoModel>();
            foreach (var item in dgTestInfo.Items)
            {
                Model.TestInfoModel tim = item as Model.TestInfoModel;
                //if (tim.IsChecked)
                //{
                models.Add(tim);
                //}
            }
            return models;
        }

        private void btnExportData_Click(object sender, RoutedEventArgs e)
        {
            string standName = "";
            if (standManager.SelectedItem == null)
            {
                MessageBox.Show("请选择测试参考值！", "系统信息");
                return;
            }
            else
            {
                standName = standManager.SelectedItem.Stand_Name;
            }
            if (testInfoModelList.Count == 0)
            {
                MessageBox.Show("所选的参考值下没有测试信息，不能导出！", "系统信息");
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "请选择保存文件的路径";
            sfd.DefaultExt = "xls";
            sfd.FileName =standName+ "测试参考值数据导出(" + DateTime.Now.ToString("yyyy-MM-dd") + ")";
            sfd.OverwritePrompt = true;
            sfd.AddExtension = true;
            sfd.Filter = "Excel文件(*.xls)|*.xls";
            if (sfd.ShowDialog() == true)
            {
                string savePath = sfd.FileName;
                
                List<List<XElement>> dataList = DSJL.Export.GenerateCompareResportXml.ComputeAvg(testInfoModelList);
                ExportData exportData = new ExportData();
                try
                {
                    exportData.Export(dataList, savePath);
                    MessageBox.Show("导出成功！","系统信息");
                }
                catch (Exception ee) {
                    MessageBox.Show("导出数据异常！\r\n" + ee.Message,"系统错误");
                }
              
            }
        }
    }
}
