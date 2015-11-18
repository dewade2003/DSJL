using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using DSJL.Control;
using System.Threading.Tasks;
using System;
using System.Threading;
using System.Linq;
using System.Xml.Linq;
using DSJL.Caches.ChartCache;
using System.IO;

namespace DSJL.Modules.Standard
{
    /// <summary>
    /// ExportStandardWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ExportStandardWindow : Window
    {
        BLL.TB_StandardInfo standBLL;
        BLL.TB_StandTestRefe refeBLL;

        List<Model.TB_StandardInfo> standList;

        List<Model.TestInfoModel> testInfoModelList;

        public Model.TB_StandardInfo selectStandInfo1, selectStandInfo2;
        public List<Model.TestInfoModel> TestInfoModelList
        {
            set
            {
                testInfoModelList = value;
            }
        }

        public ExportStandardWindow()
        {
            InitializeComponent();

            testInfoModelList = new List<Model.TestInfoModel>();
            standBLL = new BLL.TB_StandardInfo();
            refeBLL = new BLL.TB_StandTestRefe();
            standList = new List<Model.TB_StandardInfo>();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            standList = standBLL.GetModelList("");
            List<Model.TB_StandardInfo> standListLevel1 = standList.FindAll(x => x.Stand_Level == 1);
            lbLevel1.ItemsSource = standListLevel1;

            //lbLevel1.SelectedIndex = 0;
            lbLevel1.SelectedIndex = -1;
            if (selectStandInfo1 != null)
            {
                for (int i = 0; i < standListLevel1.Count; i++)
                {
                    if (standListLevel1[i].ID == selectStandInfo1.ID)
                    {
                        lbLevel1.SelectedIndex = i;
                        break;
                    }
                }
            }
            if (selectStandInfo2!=null)
            {
                for (int i = 0; i < standListLevel1.Count; i++)
                {
                    if (standListLevel1[i].ID == selectStandInfo2.Stand_ParentID)
                    {
                        lbLevel1.SelectedIndex = i;
                        break;
                    }
                }
            }
            if (lbLevel1.SelectedIndex==-1)
            {
                lbLevel1.SelectedIndex = 0;
            }

            //lbLevel1.SelectedValue = selectStandInfo1;
        }

        bool isCancleExport = false;
        string exportPath = "";
        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            List<Model.TB_StandardInfo> checkedStandList = GetCheckedStand();
            if (checkedStandList.Count==0)
            {
                DSJL.Tools.MessageBoxTool.ShowConfirmMsgBox("请选择要导出的测试参考值！");
                return;
            }
            if (DSJL.Tools.ShowFileDialogTool.ShowSaveFileDialog(out exportPath, "", "dsf", "等速肌力参考值导出") == false)
            {
                return;
            }
            exportPath = exportPath.Substring(0, exportPath.LastIndexOf("\\") + 1);
            Console.WriteLine("export path is:{0}",exportPath);

            ProgressWindow window = new ProgressWindow();
            window.WindowTilte = "导出参考值进度";
            window.MaxValue = checkedStandList.Count;
            window.MinValue = 0;
            window.CancleMessage = "确定取消导出吗？";
            window.onCancling += Window_onCancling;
            window.Owner = this;

            Task task = new Task(() => {
                int progress = 0;
                foreach (var item in checkedStandList)
                {
                    if (isCancleExport)
                    {
                        break;
                    }
                    //1、查询测试信息
                    List<Model.TestInfoModel> testInfoModelList = Caches.Util.AthTestInfoModelUtil.AthTestUtil(refeBLL.GetStandTestInfoModelList(item.ID));
                    if (testInfoModelList.Count==0)
                    {
                        continue;
                    }
                    Model.TestInfoModel avgTestInfoModel = GetAvgTestInfoModel(testInfoModelList);
                    string testInfoModelJson = Newtonsoft.Json.JsonConvert.SerializeObject(avgTestInfoModel);
                   // Console.WriteLine(testInfoModelJson);
                    //2、计算平均值
                    List<List<XElement>> paramList = DSJL.Export.GenerateCompareResportXml.ComputeAvg(testInfoModelList);
                    string paramJson = Newtonsoft.Json.JsonConvert.SerializeObject(paramList);
                   // Console.WriteLine(paramJson);
                    Dictionary<DataPointsType, List<List<double>>> dataPointsDict = StandardChartCache.GetStandardDataPoints(item, testInfoModelList);
                    List<List<double>> oddavgsd = dataPointsDict[DataPointsType.ODDAvgSD];
                    List<List<double>> evenavgsd = dataPointsDict[DataPointsType.EVENAVGSD];
                    string oddavgsdjson = Newtonsoft.Json.JsonConvert.SerializeObject(oddavgsd);
                    string evenavgsdjson = Newtonsoft.Json.JsonConvert.SerializeObject(evenavgsd);
                    // Console.WriteLine(oddavgsdjson);
                    // Console.WriteLine(evenavgsdjson);
                    //3、写入文件
                    Model.TB_StandardInfo parentStandModel = standList.Find(x => x.ID == item.Stand_ParentID);
                    Model.ExportStandModel exportStandModel = new Model.ExportStandModel();
                    exportStandModel.ParentName = parentStandModel.Stand_Name;
                    exportStandModel.StandName = item.Stand_Name;
                    exportStandModel.TestModel = avgTestInfoModel;
                    exportStandModel.ParamList = paramList;
                    exportStandModel.OddAvgSD = oddavgsd;
                    exportStandModel.EvenAvgSD = evenavgsd;
                    string standJson = Newtonsoft.Json.JsonConvert.SerializeObject(exportStandModel);
                    standJson= DSJL.Tools.DES.Encrypt(standJson, "cissdsjl");
                    string filename = string.Format("{0}{1}.dsf", exportPath, item.Stand_Name);
                    StreamWriter sw = new StreamWriter(filename);
                    sw.Write(standJson);
                    sw.Close();
                    progress++;
                    Dispatcher.BeginInvoke(new Action(()=> {
                        window.CurrentValue = progress;
                    }));
                }
                DSJL.Tools.MessageBoxTool.ShowConfirmMsgBox("导出完成！");
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    window.Close();
                    this.Close();
                }));
            });
            task.Start();
            window.ShowDialog();
        }

        public List<Model.TB_StandardInfo> GetCheckedStand() {
            List<Model.TB_StandardInfo> checkedStandList = standList.FindAll(x => x.Stand_Level == 2 && x.IsChecked == true);
            return checkedStandList;
        }

        public Model.TestInfoModel GetAvgTestInfoModel(List<Model.TestInfoModel> modelList) {
            Model.TestInfoModel testInfoModel = new Model.TestInfoModel();
            testInfoModel.Ath_Name = string.Format("{0}人", modelList.Count);
            testInfoModel.BaseFileName = "-";
            testInfoModel.TestDate = testInfoModel.TestTime = DateTime.MinValue;
            foreach (var item in modelList)
            {
                if (DateTime.Compare(item.TestDate,testInfoModel.TestDate)==1)
                {
                    testInfoModel.TestDate = testInfoModel.TestTime = item.TestDate;
                }

            }
            var jointSide = modelList.GroupBy(x => x.Joint_Side);

            foreach (var jointSideList in jointSide)
            {
                testInfoModel.DJointSide += string.Format("{0}({1}),", jointSideList.ElementAt(0).DJointSide, jointSideList.Count());
            }
            testInfoModel.DJointSide = testInfoModel.DJointSide.Substring(0, testInfoModel.DJointSide.Length - 1);

            var testModeGroupList = modelList.GroupBy(x => x.Test_Mode);
            foreach (var testModeList in testModeGroupList)
            {
                testInfoModel.DTestMode += string.Format("{0}({1}),", testModeList.ElementAt(0).DTestMode, testModeList.Count());
            }
            testInfoModel.DTestMode = testInfoModel.DTestMode.Substring(0, testInfoModel.DTestMode.Length - 1);

            var jointGroupList = modelList.GroupBy(x=>x.Joint);
            foreach (var jointList in jointGroupList)
            {
                testInfoModel.DJoint += string.Format("{0}({1}),", jointList.ElementAt(0).DJoint, jointList.Count());
            }
            testInfoModel.DJoint = testInfoModel.DJoint.Substring(0, testInfoModel.DJoint.Length - 1);

            var planeGroupList = modelList.GroupBy(x => x.Plane);
            foreach (var planeList in planeGroupList)
            {
                testInfoModel.DPlane += string.Format("{0}({1}),", planeList.ElementAt(0).DPlane, planeList.Count());
            }
            testInfoModel.DPlane = testInfoModel.DPlane.Substring(0, testInfoModel.DPlane.Length - 1);
            return testInfoModel;
        }

        private void Window_onCancling()
        {
            isCancleExport = true;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Point mousePoint = e.GetPosition(this);

                if (e.ButtonState == System.Windows.Input.MouseButtonState.Pressed && e.ChangedButton == System.Windows.Input.MouseButton.Left && mousePoint.Y <= 30)
                {

                    this.DragMove();
                }
            }
            catch { }
        }

        private void lbLevel1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Model.TB_StandardInfo selectedLevel1 = lbLevel1.SelectedValue as Model.TB_StandardInfo;
            List<Model.TB_StandardInfo> standInfoList2 = standList.FindAll(x => x.Stand_ParentID == selectedLevel1.ID);
            if (standInfoList2.Count > 0 && selectStandInfo2 != null)
            {
                Model.TB_StandardInfo findedStandInfo = standInfoList2.Find(x => x.ID == selectStandInfo2.ID);
                if (findedStandInfo != null)
                {
                    findedStandInfo.IsChecked = true;
                }

            }
            Binding b = new Binding() { Source = standInfoList2 };
            lbLevel2.SetBinding(ListBox.ItemsSourceProperty, b);

            selectStandInfo1 = selectedLevel1;
        }

        private void lbLevel2_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Model.TB_StandardInfo selectedInfo = (lbLevel2.SelectedValue as Model.TB_StandardInfo);
            standList.Find(x => x.ID == selectedInfo.ID).IsChecked = !selectedInfo.IsChecked;

            selectStandInfo2 = selectedInfo;
        }
    }
}
