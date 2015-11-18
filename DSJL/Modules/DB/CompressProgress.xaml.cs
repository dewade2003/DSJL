using System;
using System.Windows;
using System.IO;
using System.Collections.Generic;
using DSJL.Model;
using System.Threading;
using ICSharpCode.SharpZipLib.Zip;
using DSJL.Tools;

namespace DSJL.Modules.DB
{
    /// <summary>
    /// CompressProgress.xaml 的交互逻辑
    /// </summary>
    public partial class CompressProgress : Window
    {

        private delegate void RefrenshProgressDelegate(int index,string fileName);
        private RefrenshProgressDelegate rpdProgress;

        private List<string> fileNames = new List<string>();

        bool cancled = false;

        public CompressProgress()
        {
            InitializeComponent();

            rpdProgress = new RefrenshProgressDelegate(RefrenshProgress);
        }

        private void RefrenshProgress(int index, string fileName) {
            tbState.Text = fileName;
            pbCompression.Value = index;
            if (index == pbCompression.Maximum) {
                this.DialogResult = true;
                this.Close();
            }
        }

        /// <summary>
        /// 输出路径
        /// </summary>
        public string OutputPath
        {
            get;
            set;
        }

        public string DataFilePath {
            get;set;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataFilePath?.Equals("")==true)//没有设置数据目录，则压缩默认数据目录的数据文件
            {
                if (Directory.Exists(Model.AppPath.XmlDataDirPath))
                {
                    fileNames.AddRange(Directory.GetFiles(Model.AppPath.XmlDataDirPath));
                }
                fileNames.Add(Model.AppPath.RootPath + "\\AppData\\DSJLDB.mdb");
             
            }
            else
            {
                if (!Directory.Exists(DataFilePath))
                {
                    MessageBoxTool.ShowConfirmMsgBox("未找到数据文件目录！");
                    this.Close();
                }
                else
                {
                    fileNames.AddRange(Directory.GetFiles(DataFilePath));

                }
            }
            pbCompression.Maximum = fileNames.Count;

            Thread t = new Thread(new ThreadStart(CompressionThread));
            t.Start();
        }

        private void CompressionThread() {
            string outputPath = OutputPath;
            using (ZipOutputStream zopStream = new ZipOutputStream(File.Create(outputPath)))
            {
                zopStream.Password = "CISS";
                zopStream.SetLevel(9);
                byte[] buffer = new byte[4096];

                for (int i = 0; i < fileNames.Count; i++) {
                    if (cancled)
                    {
                        break;
                    }
                    string fileName = fileNames[i];
              
                    ZipEntry entry = new ZipEntry(System.IO.Path.GetFileName(fileName));
                    entry.DateTime = DateTime.Now;
                    
                    zopStream.PutNextEntry(entry);
                    using (FileStream fs = File.OpenRead(fileName))
                    {
                        int sourceBytes;
                        do
                        {
                            sourceBytes = fs.Read(buffer, 0, buffer.Length);
                            zopStream.Write(buffer, 0, sourceBytes);
                        } while (sourceBytes > 0);
                    }
                    this.Dispatcher.Invoke(rpdProgress, i + 1, fileName.Substring(fileName.LastIndexOf("\\") + 1));
                }
                zopStream.Finish();
                zopStream.Close();

                if (cancled)
                {
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        this.Close();
                    }));
                }
            }
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
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

        private void btnCancle_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxTool.ShowAskMsgBox("确定取消吗？")==MessageBoxResult.Yes)
            {
                cancled = true;
            }
        }
    }
}
