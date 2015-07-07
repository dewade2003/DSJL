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
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using System.Globalization;
using DSJL.DataUtil;
using System.ComponentModel;

namespace DSJL.Modules.Test
{
    /// <summary>
    /// QuickImportTestDataWindow.xaml 的交互逻辑
    /// </summary>
    public partial class QuickImportTestDataWindow : Window
    {

        private BLL.TB_TestInfo testInfoBLL;
        private BLL.TB_AthleteInfo athBLL;
        private BLL.TB_TestManager testManagerBLL;

        private int importFlag = 0;

        private List<Model.ImportDataErrorModel> errorModelList = new List<Model.ImportDataErrorModel>();

        private List<Model.TB_AthleteInfo> athInfoList = new List<Model.TB_AthleteInfo>();

        private string[] choosedFileNames;

        DateTimeFormatInfo dtPointFormat = new DateTimeFormatInfo();
        DateTimeFormatInfo dtLineFormat = new DateTimeFormatInfo();

        private BackgroundWorker worker = new BackgroundWorker();

        private double smoothValue = 0;

        public QuickImportTestDataWindow()
        {
            InitializeComponent();
            dtPointFormat.ShortDatePattern = "dd.MM.yyyy";
            dtPointFormat.ShortTimePattern = "HH:mm";

            dtLineFormat.ShortDatePattern = "yyyy-MM-dd";
        }

        #region 关闭 拖动
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

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        #endregion

        //选择数据文件
        private void btnChoolseFiles_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "选择测试数据文件";
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == true)
            {
                importProgress.Maximum = ofd.FileNames.Length;
                importProgress.Value = 0;

                tbFileCount.Text = "总共" + ofd.FileNames.Length + "个数据文件";
                tbProgress.Text = "0/"+ofd.FileNames.Length;

                choosedFileNames = ofd.FileNames;
                btnImport.IsEnabled = true;
            }
        }
      
        //导入
        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            if (btnImport.Content.ToString() == "开始导入")
            {
                btnChoolseFiles.IsEnabled = false;
                btnClose.IsEnabled = false;

                testManagerBLL = new BLL.TB_TestManager();
                athBLL = new BLL.TB_AthleteInfo();
                testInfoBLL = new BLL.TB_TestInfo();
                errorModelList = new List<Model.ImportDataErrorModel>();

                worker.DoWork += new DoWorkEventHandler(worker_DoWork);
                worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
                worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
                worker.WorkerReportsProgress = true;
                worker.WorkerSupportsCancellation = false;

                worker.RunWorkerAsync();

                btnImport.Content = "取消导入";
            }
            else {
                importFlag = 2;
                MessageBoxResult mr = MessageBox.Show("确定取消导入吗？", "系统信息", MessageBoxButton.OKCancel);
                if (mr == MessageBoxResult.OK)
                {
                    importFlag = 1;
                }
                else
                {
                    importFlag = 0;
                }
            }
        }

        //后台导入完成
        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            worker.DoWork -= new DoWorkEventHandler(worker_DoWork);
            worker.ProgressChanged -= new ProgressChangedEventHandler(worker_ProgressChanged);
            worker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);

            if (errorModelList.Count > 0)
            {
                MessageBoxResult mbResult = MessageBox.Show("导入完成！\r\n导入过程中存在文件读取错误，是否另存错误文件？", "系统信息", MessageBoxButton.YesNo);
                if (mbResult == MessageBoxResult.Yes)
                {
                    SaveErrorFile();
                }
            }
            else
            {
                btnImport.Content = "开始导入";
                if (importFlag == 0) {
                    MessageBox.Show("导入完成！", "系统信息");
                }
            }
            importFlag = 0;

            btnClose.IsEnabled = true;
            btnImport.IsEnabled = false;
            btnChoolseFiles.IsEnabled = true;
            tbFileCount.Text = "";
            tbProgress.Text = "";
            importProgress.Value = 0;
        }

        //后台进度更新
        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            importProgress.Value = e.ProgressPercentage;
            tbProgress.Text = e.ProgressPercentage + "/" + choosedFileNames.Length;

            object userState = e.UserState;
            if (userState != null) {
                string[] contents = (string[])userState;
                tbFileName.Text = "文件名：" + contents[0];
                tbContent1.Text = "";
                tbContent2.Text = "";
                tbContent3.Text = "";

                //写入正在导入到文件信息
                for (int a = 0; a < 11; a++)
                {
                    tbContent1.Text += contents[a] + "\r\n";
                    tbContent2.Text += contents[11 + a] + "\r\n";
                    tbContent3.Text += contents[22 + a] + "\r\n";
                }
            }
        }

        //后台导入
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Model.ImportDataErrorModel errorModel;//错误模型

            int currentManagerID = AddTestManagerModel();//添加测试项目

            for (int i = 0; i < choosedFileNames.Length; i++)
            {
                if (importFlag == 1) {
                    break;

                }
                if (importFlag == 2) {
                    i--;
                    continue;
                }

                string fileName = choosedFileNames[i];
                string[] contents = File.ReadAllLines(fileName);

                //检查数据文件是否是测试数据文件
                if (!CheckFile(fileName, contents))
                {
                    worker.ReportProgress(i + 1, null);
                    continue;
                }
                else {
                    worker.ReportProgress(i + 1, contents);
                }

                string testDate = GetLineValue(contents[2]);
                string testTime = GetLineValue(contents[3]);
                DateTime testDateTime = DateTime.Now;
                if (testDate != "" && testTime != "")
                {
                    try
                    {
                        testDateTime = Convert.ToDateTime(testDate + " " + testTime, dtPointFormat);
                    }
                    catch {
                        testDateTime = DateTime.Now;
                    }
                }

                //添加人员信息
                Model.TB_AthleteInfo athModel = new Model.TB_AthleteInfo();
                athModel.Ath_PinYin = athModel.Ath_Name = GetLineValue(contents[4]);
                athModel.Ath_TestID = currentManagerID;
                athModel.Ath_Sex = DataUtil.AthleteSexUtil.GetSex(GetLineValue(contents[29]));
                athModel.Ath_TestDate = testDateTime;
                string birthdayStr = GetLineValue(contents[27]);
                if (birthdayStr != "")
                {
                    try
                    {
                        athModel.Ath_Birthday = Convert.ToDateTime(birthdayStr, dtLineFormat);
                    }
                    catch {
                        athModel.Ath_Birthday = DateTime.Now;
                    }
                }
                athModel.Ath_Height = GetLineValue(contents[26]);
                athModel.Ath_Weight = GetLineValue(contents[25]);

                int athID = AddAthInfo(athModel);
                if (athID == -1)
                {//添加失败，继续
                    errorModel = new Model.ImportDataErrorModel();
                    errorModel.ErrorString = "添加人员信息出错";
                    errorModel.FileName = fileName;
                    errorModelList.Add(errorModel);
                    continue;
                }


                string Gravitycomp = GetLineValue(contents[31]);
                string dataFileName = "";
                //处理数据文件
                try
                {
                    BaseDataUtil dataUtil = new BaseDataUtil();
                    dataUtil.SmoothValue = (int)smoothValue;
                    dataUtil.Weight = athModel.Ath_Weight;
                    if (Gravitycomp != "")
                    {
                        dataUtil.Gravitycomp = Gravitycomp;
                    }
                    dataFileName = dataUtil.WriteBaseData(contents);
                }
                catch
                {
                    errorModel = new Model.ImportDataErrorModel();
                    errorModel.ErrorString = "计算出错";
                    errorModel.FileName = fileName;
                    errorModelList.Add(errorModel);
                    continue;
                }
                //添加到数据库
                try
                {
                    Model.TB_TestInfo testInfoModel = new Model.TB_TestInfo();
                    testInfoModel.Gravitycomp = Gravitycomp;
                    testInfoModel.DataFileName = dataFileName;
                    testInfoModel.BaseFileName = contents[0].Trim();
                    testInfoModel.Ath_ID = athID;

                    testInfoModel.TestDate = testInfoModel.TestTime = testDateTime;

                    testInfoModel.Joint_Side = GetLineValue(contents[5]);
                    testInfoModel.Test_Mode = GetLineValue(contents[6]);
                    testInfoModel.Joint = GetLineValue(contents[7]);
                    testInfoModel.Plane = GetLineValue(contents[8]);
                    testInfoModel.Motion_Start = GetLineValue(contents[10]);
                    testInfoModel.Motion_End = GetLineValue(contents[11]);
                    testInfoModel.Speed1 = GetLineValue(contents[12]);
                    testInfoModel.Speed2 = GetLineValue(contents[13]);
                    testInfoModel.Acceleration1 = GetLineValue(contents[14]);
                    testInfoModel.Acceleration2 = GetLineValue(contents[15]);
                    testInfoModel.Break = GetLineValue(contents[18]);
                    testInfoModel.NOOfSets = GetLineValue(contents[22]);
                    testInfoModel.NOOfRepetitions = GetLineValue(contents[23]);
                    testInfoModel.InsuredSide = GetLineValue(contents[28]);
                    testInfoModel.Therapist = GetLineValue(contents[30]);

                    testInfoBLL.Add(testInfoModel);
                }
                catch
                {
                    errorModel = new Model.ImportDataErrorModel();
                    errorModel.ErrorString = "保存测试信息出错";
                    errorModel.FileName = fileName;
                    errorModelList.Add(errorModel);
                    continue;
                }
            }
        }

        //添加人员信息
        private int AddAthInfo(Model.TB_AthleteInfo athModel) {
            int athID = 0;

            bool isExists = false;
            foreach (Model.TB_AthleteInfo ath in athInfoList) {
                if (athModel.Ath_Name == ath.Ath_Name && athModel.Ath_Sex == ath.Ath_Sex && athModel.Ath_Height == ath.Ath_Height && athModel.Ath_Weight == ath.Ath_Weight && athModel.Ath_Birthday == ath.Ath_Birthday && athModel.Ath_TestDate.ToString("yyyyMMdd") == ath.Ath_TestDate.ToString("yyyyMMdd"))
                {
                    isExists = true;
                    athID = ath.ID;
                    break;
                }
            }
            //在本地没有对应的信息，则添加
            if (!isExists) {
                string idStr = "";
                try
                {
                    athBLL.Add(athModel, out idStr);
                }
                catch
                {
                    athID = -1;
                }
                if (idStr != "")
                {
                    athID = int.Parse(idStr);
                }
                else
                {
                    athID = athBLL.GetMaxId();
                }
                athModel.ID = athID;
                athInfoList.Add(athModel);
            }
            
            return athID;
        }

        //添加测试项目
        private int AddTestManagerModel() {
            Model.TB_TestManager testManagerModel = new Model.TB_TestManager();
            testManagerModel.Remark = "";
            testManagerModel.TestEndDate = testManagerModel.TestStartDate = DateTime.Now.ToString("yyyy-MM-dd");
            testManagerModel.TestName = "未命名测试项目";
            testManagerBLL.Add(testManagerModel);

            int managerID = testManagerBLL.GetMaxId();
            return managerID;
        }

        //检查文件是否为数据文件
        private bool CheckFile(string fileName, string[] fileContents) {
            bool checkResult = true;
            Model.ImportDataErrorModel errorModel;

            if (fileContents.Length > 0)
            {
                string line2 = fileContents[1];
                if (!line2.Contains("File-type"))
                {
                    
                    errorModel = new Model.ImportDataErrorModel();
                    errorModel.ErrorString = "未检测到File-type";
                    errorModel.FileName = fileName;
                    errorModelList.Add(errorModel);
                    checkResult = false;
                }
            }
            else
            {
                errorModel = new Model.ImportDataErrorModel();
                errorModel.ErrorString = "数据文件为空";
                errorModel.FileName = fileName;
                errorModelList.Add(errorModel);
                checkResult = false;
            }


            return checkResult;
        }

        //获取数据文件前32行每行的值
        private string GetLineValue(string line)
        {
            string value = line.Substring(line.IndexOf(':') + 1).Trim();
            return value;
        }

        private void SaveErrorFile() {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.ShowNewFolderButton = true;
            fbd.Description = "选择保存导入错误文件的位置";
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string selectedPath = fbd.SelectedPath;
                foreach (Model.ImportDataErrorModel model in errorModelList)
                {
                    string fileName = model.FileName.Substring(model.FileName.LastIndexOf("\\") + 1);
                    try
                    {
                        File.Copy(model.FileName, selectedPath + "\\" + model.ErrorString + "-" + fileName, true);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
        }

        private void smoothSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            smoothValue = e.NewValue;
        }
    }
}
