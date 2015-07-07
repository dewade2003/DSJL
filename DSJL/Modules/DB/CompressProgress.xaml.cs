using System;
using System.Windows;
using System.IO;
using System.Collections.Generic;
using DSJL.Model;
using System.Threading;
using ICSharpCode.SharpZipLib.Zip;

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

        public CompressProgress()
        {
            InitializeComponent();

            rpdProgress = new RefrenshProgressDelegate(RefrenshProgress);
        }

        private void RefrenshProgress(int index, string fileName) {
            tbState.Text = fileName;
            pbCompression.Value = index;
            if (index == pbCompression.Maximum) {
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           
            if (Directory.Exists(Model.AppPath.XmlDataDirPath))
            {
                fileNames.AddRange(Directory.GetFiles(Model.AppPath.XmlDataDirPath));
            }
            fileNames.Add(Model.AppPath.RootPath + "\\AppData\\DSJLDB.mdb");
            pbCompression.Maximum = fileNames.Count;
            
            Thread t = new Thread(new ThreadStart(Compression));
            t.Start();
        }

        private void Compression() {
            string outputPath = OutputPath;
            using (ZipOutputStream zopStream = new ZipOutputStream(File.Create(outputPath)))
            {
                zopStream.Password = "CISS";
                zopStream.SetLevel(9);
                byte[] buffer = new byte[4096];

                for (int i = 0; i < fileNames.Count; i++) {
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
    }
}
