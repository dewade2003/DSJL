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
using DSJL.Model;
using System.Threading;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace DSJL.Modules.DB
{
    /// <summary>
    /// ExtractProgressWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ExtractProgressWindow : Window
    {
        private delegate void RefrenshUIDelegate(double done,double all, string state);
        private RefrenshUIDelegate rui;

        private delegate void CloseDelegate();
        private CloseDelegate closeWindow;

     
        private string tempExtrctorPath = AppPath.RootPath + "\\appdatatemp\\";
        public ExtractProgressWindow()
        {
            InitializeComponent();
            rui = new RefrenshUIDelegate(RefrenshUI);
            closeWindow = new CloseDelegate(CloseWindow);
         
        }

        private void CloseWindow() {
            this.Close();
        }

        /// <summary>
        /// 备份文件
        /// </summary>
        public string BackupFile
        {
            get;
            set;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(tempExtrctorPath))
            {
                Directory.Delete(tempExtrctorPath,true);
            }

            Thread extrctorThread = new Thread(new ThreadStart(Extrctor));
            extrctorThread.Start();
        }
        //解压方法
        private void Extrctor()
        {
            if (!Directory.Exists(tempExtrctorPath))
            {
                Directory.CreateDirectory(tempExtrctorPath);
            }

            FileStream fs = File.OpenRead(BackupFile);
            long length = fs.Length;
            using (ZipInputStream s = new ZipInputStream(fs))
            {
                s.Password = "CISS";
                ZipEntry theEntry;
                double done = 0;
               
                long completeLength = 0;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    completeLength += theEntry.Size;
                    done = completeLength / length;
                    this.Dispatcher.Invoke(rui, done, 10, "正在解压缩备份文件....");
                    string fileName = System.IO.Path.GetFileName(theEntry.Name);
                    if (fileName != String.Empty)
                    {
                        using (FileStream streamWriter = File.Create(tempExtrctorPath + theEntry.Name))
                        {

                            int size = 2048;
                            byte[] data = new byte[2048];
                            while (true)
                            {
                                size = s.Read(data, 0, data.Length);
                                if (size > 0)
                                {
                                    streamWriter.Write(data, 0, size);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                 
                }
            }

            Restore();
        }

        //刷新界面
        private void RefrenshUI(double done,double all, string state)
        {
            tbState.Text = state;
            pbExtract.Maximum = all;
            pbExtract.Value = done;
        }

        //还原文件
        private void Restore() {
       
            //检查解压到文件中是否有mdb数据库文件
            List<string> tempFileNames = new List<string>();
            tempFileNames.AddRange(Directory.GetFiles(tempExtrctorPath));
            string mdbFile = "";
            try
            {
                mdbFile = tempFileNames.Single(x => x.Substring(x.LastIndexOf(".")) == ".mdb");
                tempFileNames.Remove(mdbFile);
            }
            catch (Exception ee)
            {
                Directory.Delete(tempExtrctorPath, true);
                MessageBox.Show("没有找到数据库文件，该文件不是软件的备份文件！", "系统信息");
                this.Close();
            }
            //还原数据库文件
            File.Copy(mdbFile, Model.AppPath.RootPath + "\\AppData\\DSJLDB.mdb", true);
            //删除旧文件
            if (Directory.Exists(Model.AppPath.XmlDataDirPath))
            {

                string[] oldFileNames = Directory.GetFiles(Model.AppPath.XmlDataDirPath);
                foreach (string oldFileName in oldFileNames)
                {
                    File.Delete(oldFileName);
                }

                //复制新文件
                for (int i = 0; i < tempFileNames.Count; i++) {
                    string xmlFile = tempFileNames[i];
            
                    string name = xmlFile.Substring(xmlFile.LastIndexOf("\\"));
                    File.Copy(xmlFile, Model.AppPath.XmlDataDirPath + name);
                    this.Dispatcher.Invoke(rui, (double)(i + 1), (double)tempFileNames.Count, "正在还原数据文件...");
                }
            }
            else
            {
                Directory.CreateDirectory(Model.AppPath.XmlDataDirPath);
                //复制新文件
                for (int i = 0; i < tempFileNames.Count; i++)
                {
                    string xmlFile = tempFileNames[i];
                    string name = xmlFile.Substring(xmlFile.LastIndexOf("\\"));
                    File.Copy(xmlFile, Model.AppPath.XmlDataDirPath + name);
                    this.Dispatcher.Invoke(rui, (double)(i + 1), (double)tempFileNames.Count, "正在还原数据文件...");
                }
            }
            Directory.Delete(tempExtrctorPath, true);
            this.Dispatcher.Invoke(rui, 1,1, "还原完成!");

            this.Dispatcher.Invoke(closeWindow);
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
    }
}
