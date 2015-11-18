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
using DSJL.DataUtil;
using System.Data;
using DSJL.DBUtility;
using System.Globalization;

namespace DSJL.Modules.Test
{
    /// <summary>
    /// ImportTestDataWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ImportTestDataWindow : Window
    {
        BLL.TB_AthleteInfo athleteInfoBLL = new BLL.TB_AthleteInfo();
        BLL.TB_TestInfo testInfoBLL = new BLL.TB_TestInfo();

        private int importFlag = 0;//导入标志，当为1时取消导入

        List<Model.TestInfoModel> testInfoModelList;

        List<Model.ExistsFileModel> existsFiles = new List<Model.ExistsFileModel>();
        List<Model.ImportDataErrorModel> errorModelList = new List<Model.ImportDataErrorModel>();
        string existsIDs = "";

        static Dictionary<List<string>, Model.TB_AthleteInfo> noCheckedAthInfoDict = new Dictionary<List<string>, Model.TB_AthleteInfo>();

        private delegate void UpdateProgressBarDelegate(System.Windows.DependencyProperty dp, Object value);

        DateTimeFormatInfo dtPointFormat = new DateTimeFormatInfo();
        DateTimeFormatInfo dtLineFormat = new DateTimeFormatInfo();

        private double smoothValue = 0;//平滑系数
        private string[] fileNames;

        public ImportTestDataWindow()
        {
            InitializeComponent();

            dtPointFormat.ShortDatePattern = "dd.MM.yyyy";
            dtPointFormat.ShortTimePattern = "HH:mm";

            dtLineFormat.ShortDatePattern = "yyyy-MM-dd";
        }

        public Model.TB_TestManager SelectedTestItem
        {
            get;
            set;
        }

        //拖动
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

        //关闭
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (errorModelList.Count > 0)
            {
                if (MessageBox.Show("上次导入存在导入错误，确定不另存错误文件吗？", "系统信息", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    return;
                }
                else
                {
                    this.DialogResult = true;
                    this.Close();
                }
            }
            else {
                this.DialogResult = true;
                this.Close();
            }
        }
        //选择文件
        private void btnChoolseFiles_Click(object sender, RoutedEventArgs e)
        {
            if (errorModelList.Count > 0) {
                if (MessageBox.Show("上次导入存在导入错误，确定不另存错误文件吗？", "系统信息", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    return;
                }
                else {
                    ErrorPanel.Visibility = Visibility.Hidden;
                    errorModelList.Clear();
                    tbErrorCount.Text = "";
                }
            }
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "选择测试数据文件";
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == true)
            {
                existsFiles = new List<Model.ExistsFileModel>();
                existsIDs = "";
                testInfoModelList = new List<Model.TestInfoModel>();
                tbFileCount.Text = "总共" + ofd.FileNames.Count() + "个数据文件";

                fileNames = ofd.FileNames;
                btnImport.IsEnabled = true;
            }
        }

        //导入
        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            if (btnImport.Content.ToString() == "开始导入")
            {
                btnImport.Content = "取消导入";
                Import(fileNames, true);
                if (importFlag == 0)
                {
                    MessageBox.Show("导入完成！", "系统信息");
                }
                importFlag = 0;
                importProgress.Value = 0;
                btnImport.Content = "开始导入";
                btnImport.IsEnabled = false;
                fileNames = null;
                tbFileCount.Text = "";
                tbProgress.Text = "";
            }
            else {
                importFlag = 2;
                MessageBoxResult mr = MessageBox.Show("确定取消导入吗？", "系统信息", MessageBoxButton.OKCancel);
                if (mr == MessageBoxResult.OK)
                {
                    importFlag = 1;
                }
                else {
                    importFlag = 0;
                }
            }
        }

        //导入方法
        private void Import(string [] fileNames,bool ischeckRepeate) {
            importProgress.Maximum = fileNames.Count();
            importProgress.Minimum = 0;
            importProgress.Value = 0;
            double value = 0;
            UpdateProgressBarDelegate updatePbDelegate = new UpdateProgressBarDelegate(importProgress.SetValue);
            for (int i = 0; i < fileNames.Count(); i++) {
                if (importFlag == 1) {
                    break;
                }
                if (importFlag == 2) {
                    i--;
                    continue;
                }
                value += 1;
                Dispatcher.Invoke(updatePbDelegate,
                    System.Windows.Threading.DispatcherPriority.Background,
                    new object[] { ProgressBar.ValueProperty, value });

                Model.ImportDataErrorModel errorModel=null;

                string baseFileName=fileNames[i];

                #region 写入进度
                tbFileName.Text = "文件名：" + baseFileName.Substring(baseFileName.LastIndexOf("\\") + 1);
                tbContent1.Text = "";
                tbContent2.Text = "";
                tbContent3.Text = "";
                tbProgress.Text = (i + 1) + "/" + fileNames.Count();
                #endregion

                #region 读取数据文件信息
                string[] contents = File.ReadAllLines(baseFileName);

                if (!CheckFile(baseFileName, contents))
                {
                    continue;
                }
                for (int a = 0; a < 11; a++)
                {
                    tbContent1.Text += contents[a] + "\r\n";
                    tbContent2.Text += contents[11 + a] + "\r\n";
                    tbContent3.Text += contents[22 + a] + "\r\n";
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

                Model.TB_AthleteInfo athModel = new Model.TB_AthleteInfo();
                athModel.Ath_PinYin = athModel.Ath_Name = GetLineValue(contents[4]);
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
                else {
                    athModel.Ath_Birthday = null;
                }
                athModel.Ath_Height = GetLineValue(contents[26]);
                athModel.Ath_Weight = GetLineValue(contents[25]);

                Model.TB_AthleteInfo testAthInfoModel = CheckAthInfo(athModel);
                if (testAthInfoModel == null) {
                    continue;
                }

                Model.TB_TestInfo testInfo = new Model.TB_TestInfo();
                testInfo.BaseFileName = contents[0].Trim();
                testInfo.TestTime = testInfo.TestDate = testDateTime;
                testInfo.Joint_Side = GetLineValue(contents[5]);
                testInfo.Test_Mode = GetLineValue(contents[6]);
                testInfo.Joint = GetLineValue(contents[7]);
                testInfo.Plane = GetLineValue(contents[8]);
                testInfo.Motion_Start = GetLineValue(contents[10]);
                testInfo.Motion_End = GetLineValue(contents[11]);
                testInfo.Speed1 = GetLineValue(contents[12]);
                testInfo.Speed2 = GetLineValue(contents[13]);
                testInfo.Acceleration1 = GetLineValue(contents[14]);
                testInfo.Acceleration2 = GetLineValue(contents[15]);
                testInfo.Break = GetLineValue(contents[18]);
                testInfo.NOOfSets = GetLineValue(contents[22]);
                testInfo.NOOfRepetitions = GetLineValue(contents[23]);
                testInfo.InsuredSide = GetLineValue(contents[28]);
                testInfo.Therapist = GetLineValue(contents[30]);
                testInfo.Gravitycomp = GetLineValue(contents[31]);

                testInfo.DataFileName = "";

                testInfo.Ath_ID = testAthInfoModel.ID;
                #endregion

                #region 检查是否重复导入
                if (ischeckRepeate) {
                    //检测是否有相同时间的数据导入
                    try
                    {
                        //判断该测试信息是否导入，判断条件 用户ID 测试日期,数据文件名
                        List<Model.TB_TestInfo> existsTestInfoList = testInfoBLL.GetModelList("ath_id=" + testInfo.Ath_ID + " and testdate=#" + testInfo.TestDate + "# and BaseFileName='" + testInfo.BaseFileName + "'");
                        if (existsTestInfoList.Count > 0)//有重复数据
                        {
                            existsIDs += existsTestInfoList[0].ID + ",";
                            Model.ExistsFileModel exFileModel = new Model.ExistsFileModel();
                            exFileModel.FileName = baseFileName;
                            exFileModel.RealName = baseFileName.Substring(baseFileName.LastIndexOf("\\") + 1);
                            existsFiles.Add(exFileModel);
                            continue;
                        }
                    }
                    catch (Exception ee)
                    {
                        MessageBox.Show("检查此文件是否导入时出错！\r\n" + ee.Message, "系统错误");
                        errorModel = new Model.ImportDataErrorModel();
                        errorModel.ErrorString = "3";
                        errorModel.FileName = baseFileName;
                        errorModelList.Add(errorModel);
                        continue;
                    }
                }
                #endregion

                //导入
                try
                {
                    BaseDataUtil dataUtil = new BaseDataUtil();
                    dataUtil.SmoothValue = (int)smoothValue;
                    dataUtil.Weight = testAthInfoModel.Ath_Weight;
                    if (testInfo.Gravitycomp.Trim() != "") {
                        dataUtil.Gravitycomp = testInfo.Gravitycomp;
                    }
                    string fileName = dataUtil.WriteBaseData(contents);

                    testInfo.DataFileName = fileName;
                }
                catch (Exception ee)
                {
                    MessageBox.Show("计算出错！\r\n" + ee.Message, "系统错误");
                    errorModel = new Model.ImportDataErrorModel();
                    errorModel.ErrorString = "4";
                    errorModel.FileName = baseFileName;
                    errorModelList.Add(errorModel);
                    continue;
                }
                //添加到数据库
                try
                {
                    testInfoBLL.Add(testInfo);
                }
                catch (Exception ee)
                {
                    MessageBox.Show("添加测试信息出错，请稍候重试！\r\n" + ee.Message, "系统错误");
                    errorModel = new Model.ImportDataErrorModel();
                    errorModel.ErrorString = "5";
                    errorModel.FileName = baseFileName;
                    errorModelList.Add(errorModel);
                    continue;
                }
              
            }

            if (errorModelList.Count > 0) {
                tbErrorCount.Text = errorModelList.Count + "个文件导入错误";
                ErrorPanel.Visibility = Visibility.Visible;
            }

            if (ischeckRepeate)
            {//如果检查重复并且存在重复数据就刷新数据列表
                if (existsFiles.Count > 0)
                {
                    btnReImport.IsEnabled = true;
                    RefrenshDataGridSource();
                }
                else {
                    btnReImport.IsEnabled = false;
                }
            }
         
        }

        //检查文件是否为数据文件
        private bool CheckFile(string fileName, string[] fileContents)
        {
            bool checkResult = true;
            Model.ImportDataErrorModel errorModel;

            if (fileContents.Length > 0)
            {
                string line2 = fileContents[1];
                if (!line2.Contains("File-type"))
                {

                    errorModel = new Model.ImportDataErrorModel();
                    errorModel.ErrorString = "1";
                    errorModel.FileName = fileName;
                    errorModelList.Add(errorModel);
                    checkResult = false;
                }
            }
            else
            {
                errorModel = new Model.ImportDataErrorModel();
                errorModel.ErrorString = "2";
                errorModel.FileName = fileName;
                errorModelList.Add(errorModel);
                checkResult = false;
            }


            return checkResult;
        }

        private List<List<Model.TB_AthleteInfo>> localAthInfoList = new List<List<Model.TB_AthleteInfo>>();

        private Model.TB_AthleteInfo CheckAthInfo(Model.TB_AthleteInfo athModel) {
            Model.TB_AthleteInfo findedAthInfoModel=null;

            bool isExists = false;
            //先从本地查找
            foreach (List<Model.TB_AthleteInfo> aths in localAthInfoList)
            {
                Model.TB_AthleteInfo ath = aths[0];
                if (athModel.Ath_Name == ath.Ath_Name && athModel.Ath_Sex == ath.Ath_Sex && athModel.Ath_Height == ath.Ath_Height && athModel.Ath_Weight == ath.Ath_Weight && athModel.Ath_Birthday == ath.Ath_Birthday && athModel.Ath_TestDate.ToString("yyyyMMdd") == ath.Ath_TestDate.ToString("yyyyMMdd"))
                {
                    isExists = true;
                    findedAthInfoModel = aths[1];
                    break;
                }
            }
            //本地没有查找到
            if (!isExists) {
                if (athModel.Ath_Birthday != null)//根据拼音，性别，测试日期，生日从数据库查找
                {
                    findedAthInfoModel = FindAthInfoFromDataBase(athModel);
                }
                if (findedAthInfoModel == null) { //进入手动模式选择人员
                    if (MessageBox.Show("未找到测试人员信息,是否手动选择受测者？", "系统信息", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        findedAthInfoModel = FindAthInfoHM(athModel);

                        if (MessageBox.Show("是否将同姓名、同出生、同测试日期的其余测试数据全部对应到该受试者？", "系统信息", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            List<Model.TB_AthleteInfo> athL = new List<Model.TB_AthleteInfo>() { athModel, findedAthInfoModel };
                            localAthInfoList.Add(athL);
                        }
                    }
                }

            }

            return findedAthInfoModel;
        }

        /// <summary>
        /// 从数据库里查找测试者信息
        /// </summary>
        /// <param name="athModel"></param>
        /// <returns></returns>
        private Model.TB_AthleteInfo FindAthInfoFromDataBase(Model.TB_AthleteInfo athModel) {
            Model.TB_AthleteInfo ath=null;

            DateTime birthday = (DateTime)athModel.Ath_Birthday;
            List<Model.TB_AthleteInfo> athList = athleteInfoBLL.GetModelList("ath_pinyin='" + athModel.Ath_PinYin + "' and ath_sex='" + athModel.Ath_Sex + "' and ath_testdate=#" + athModel.Ath_TestDate.ToString("yyyy-MM-dd") + "# and (format(ath_birthday,'yyyy-MM-dd')='" + birthday.ToString("yyyy-MM-dd") + "' or format(ath_birthday,'yyyy-MM')='" + birthday.ToString("yyyy-MM") + "')");

            if (athList.Count > 0) {
                ath = athList[0];
            }

            return ath;
        }

        /// <summary>
        /// 手动选择人员信息
        /// </summary>
        /// <param name="athModel"></param>
        /// <returns></returns>
        private Model.TB_AthleteInfo FindAthInfoHM(Model.TB_AthleteInfo athModel) {
            Model.TB_AthleteInfo ath = null;

            CheckAthWindow checkAthWindow = new CheckAthWindow();
            checkAthWindow.SelectedTestItem = SelectedTestItem;
            checkAthWindow.CurrentAthInfo = athModel;
            checkAthWindow.Owner = Application.Current.MainWindow;

            if (checkAthWindow.ShowDialog() == true) {
                ath = checkAthWindow.CheckedAth;
            }

            return ath;
        }

        private Model.TB_AthleteInfo CheckAthInfoFromNoCheckedInfoList(List<string> baseData) {
            Model.TB_AthleteInfo ath = null;
            for (int i = 0; i < noCheckedAthInfoDict.Count; i++) {
                List<string> key = noCheckedAthInfoDict.Keys.ElementAt(i);
                if (key[0] == baseData[0] && key[1] == baseData[1] && key[2] == baseData[2] && key[3] == baseData[3] && key[4] == baseData[4] && key[5] == baseData[5])
                {
                    ath = noCheckedAthInfoDict.Values.ElementAt(i);
                    break;
                }
            }
            return ath;
        }

        //刷新数据表
        private void RefrenshDataGridSource()
        {
            testInfoModelList = new List<Model.TestInfoModel>();
            existsIDs += "0";
            string sql = "select ath.*,test.*,"
               + "(select dict_value from tb_dict where dict_groupid=3 and dict_key=test.joint_side) as djointside,"
               + "(select dict_value from tb_dict where dict_groupid=1 and dict_key=test.test_mode) as dtestmode,"
               + "(select dict_value from tb_dict where dict_groupid=2 and dict_key=test.joint) as djoint,"
               + "(select dict_value from tb_dict where dict_groupid=(select id from tb_dict where dict_groupid=2 and dict_key=test.joint) and dict_key=test.plane and instr(dict_groupid2,test.test_mode)>0) as dplane,"
               + "(select dict_value from tb_dict where dict_groupid=4 and dict_key=test.InsuredSide) as dInsuredSide,"
               + "(select dict_value from tb_dict where dict_groupid=5 and dict_key=test.Gravitycomp) as dGravitycomp "
               + "from tb_athleteinfo as ath inner join tb_testinfo as test on ath.id=test.ath_id where 0=0 ";
            sql += " and test.id in ("+existsIDs+")";

            DataSet ds = DbHelperOleDb.Query(sql);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                DataRow dr = ds.Tables[0].Rows[i];
                Model.TestInfoModel testInfoModel = new Model.TestInfoModel();
                testInfoModel.Index = i + 1;
                testInfoModel.DGravitycomp = dr["dGravitycomp"].ToString();
                testInfoModel.DInsuredSide = dr["dInsuredSide"].ToString();
                testInfoModel.DJoint = dr["djoint"].ToString();
                testInfoModel.DJointSide = dr["djointside"].ToString();
                testInfoModel.DPlane = dr["dplane"].ToString();
                testInfoModel.DTestMode = dr["dtestmode"].ToString();
                testInfoBLL.GetModelFromDataRow(dr, testInfoModel);
                athleteInfoBLL.GetAthleteInfoFromDataRow(dr, testInfoModel);
                testInfoModelList.Add(testInfoModel);
            }
            dgTestInfo.ItemsSource = testInfoModelList;
            lbFileNames.ItemsSource = existsFiles;
        }

        //获取数据文件前32行每行的值
        private string GetLineValue(string line)
        {
            string value = line.Substring(line.IndexOf(':') + 1).Trim();
            return value;
        }

        //重新导入
        private void btnReImport_Click(object sender, RoutedEventArgs e)
        {
            if (errorModelList.Count > 0)
            {
                if (MessageBox.Show("上次导入存在导入错误，确定不另存错误文件吗？", "系统信息", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    return;
                }
                else
                {
                    ErrorPanel.Visibility = Visibility.Hidden;
                    errorModelList.Clear();
                    tbErrorCount.Text = "";
                }
            }
            List<Model.ExistsFileModel> checkedFileModelList = existsFiles.FindAll(x => x.IsChecked == true);
            int selectedCount = checkedFileModelList.Count;
            if (selectedCount <= 0) {
                MessageBox.Show("请至少选择一个重复数据的文件名进行导入！","系统信息");
                return;
            }
            string[] fns=new string[selectedCount];
            for (int i = 0; i < selectedCount; i++) {
                fns[i] = checkedFileModelList[i].FileName;
            }
            Import(fns, false); 
            for (int i = 0; i < existsFiles.Count; i++) {
                if (existsFiles[i].IsChecked == true) {
                    testInfoModelList.RemoveAt(i);
                    existsFiles.RemoveAt(i);
                    i--;
                }
            }

            MessageBox.Show("导入完成！", "系统信息");

            dgTestInfo.ItemsSource = testInfoModelList;
            lbFileNames.ItemsSource = existsFiles;
            dgTestInfo.Items.Refresh();
            lbFileNames.Items.Refresh();

            if (existsFiles.Count <= 0)
            {
                btnReImport.IsEnabled = false;
            }
        }

        //重复文件列表点击
        private void lbFileNames_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var selectedFileNames = lbFileNames.SelectedItems;
            if (selectedFileNames.Count > 0) {
                foreach (Model.ExistsFileModel model in existsFiles)
                {
                    model.IsChecked = false;
                }
                if (selectedFileNames.Count > 1)
                {
                    foreach (var item in selectedFileNames) {
                        Model.ExistsFileModel model = item as Model.ExistsFileModel;
                        model.IsChecked = true;
                    }
                }
                else {
                    Model.ExistsFileModel model = lbFileNames.SelectedValue as Model.ExistsFileModel;
                    existsFiles.Find(x => x.RealName == model.RealName).IsChecked = true;
                    try
                    {
                        dgTestInfo.SelectedIndex = lbFileNames.SelectedIndex;
                        dgTestInfo.ScrollIntoView(dgTestInfo.Items[dgTestInfo.SelectedIndex]);
                    }
                    catch
                    {
                    }

                    string fileName = existsFiles[dgTestInfo.SelectedIndex].FileName;
                    string[] fileLines = File.ReadAllLines(fileName);
                    tbFileName.Text = "文件名：" + fileName.Substring(fileName.LastIndexOf("\\") + 1);
                    tbContent1.Text = "";
                    tbContent2.Text = "";
                    tbContent3.Text = "";
                    for (int a = 0; a < 11; a++)
                    {
                        tbContent1.Text += fileLines[a] + "\r\n";
                        tbContent2.Text += fileLines[11 + a] + "\r\n";
                        tbContent3.Text += fileLines[22 + a] + "\r\n";
                    }
                }
            }
        }

        //保存错误文件
        private void btnSaveErrorFile_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.ShowNewFolderButton = true;
            fbd.Description = "选择保存导入错误文件的位置";
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                string selectedPath = fbd.SelectedPath;
                foreach (Model.ImportDataErrorModel model in errorModelList) {
                    string fileName = model.FileName.Substring(model.FileName.LastIndexOf("\\") + 1);
                    try
                    {
                        File.Copy(model.FileName, selectedPath + "\\" + model.ErrorString + "-" + fileName,true);
                    }
                    catch {
                        continue;
                    }
                }
                ErrorPanel.Visibility = Visibility.Hidden;
                errorModelList.Clear();
                tbErrorCount.Text = "";
            }
        }

        //选择所有重复的文件列表
        private void checkAllRepeatFile_Checked(object sender, RoutedEventArgs e)
        {
            foreach (Model.ExistsFileModel fileModel in existsFiles) {
                if (checkAllRepeatFile.IsChecked == true)
                {
                    fileModel.IsChecked = true;
                }
                else
                {
                    fileModel.IsChecked = false;
                }
            }
          
        }

        private void lbFileNames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedFileNames = lbFileNames.SelectedItems;
            if (selectedFileNames.Count > 0)
            {
                foreach (Model.ExistsFileModel model in existsFiles)
                {
                    model.IsChecked = false;
                }
                if (selectedFileNames.Count > 1)
                {
                    foreach (var item in selectedFileNames)
                    {
                        Model.ExistsFileModel model = item as Model.ExistsFileModel;
                        model.IsChecked = true;
                    }
                }
                else
                {
                    Model.ExistsFileModel model = lbFileNames.SelectedValue as Model.ExistsFileModel;
                    existsFiles.Find(x => x.RealName == model.RealName).IsChecked = true;
                    try
                    {
                        dgTestInfo.SelectedIndex = lbFileNames.SelectedIndex;
                        dgTestInfo.ScrollIntoView(dgTestInfo.Items[dgTestInfo.SelectedIndex]);
                    }
                    catch
                    {
                    }

                    string fileName = existsFiles[dgTestInfo.SelectedIndex].FileName;
                    string[] fileLines = File.ReadAllLines(fileName);
                    tbFileName.Text = "文件名：" + fileName.Substring(fileName.LastIndexOf("\\") + 1);
                    tbContent1.Text = "";
                    tbContent2.Text = "";
                    tbContent3.Text = "";
                    for (int a = 0; a < 11; a++)
                    {
                        tbContent1.Text += fileLines[a] + "\r\n";
                        tbContent2.Text += fileLines[11 + a] + "\r\n";
                        tbContent3.Text += fileLines[22 + a] + "\r\n";
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
