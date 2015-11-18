using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DSJL.Tools
{
    public class ShowFileDialogTool
    {
        public static readonly string zipExt = "zip";
        public static readonly string zipFilter = "等速肌力数据库备份文件(*.zip)|*.zip";

        public static readonly string excelExt = "xls";
        public static readonly string excelFilter = "Excel文件(*.xls)|*.xls";

        public static readonly string pdfExt = "pdf";
        public static readonly string pdfFilter = "PDF文件(*.pdf)|*.pdf";

        public static readonly string dsfExt = "dsf";
        public static readonly string dsfFilter = "等速肌力测试参考值文件(*.dsf)|*.dsf";

        public static bool ShowSaveFileDialog(out string fileName,string filter,string ext,string defaultFileName) {
            bool result = false;
            fileName = "";
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "请选择保存文件的路径";
            sfd.DefaultExt = ext;
            sfd.FileName = defaultFileName;
            sfd.OverwritePrompt = true;
            sfd.AddExtension = true;
            sfd.Filter = filter;
            if(sfd.ShowDialog()==DialogResult.OK){
                fileName = sfd.FileName;
                result = true;
            }
            return result;
        }

        public static bool ShowOpenFileDialog(out string[] fileName, string filter, string ext)
        {
            bool result = false;
            fileName =  new string[1];
            OpenFileDialog sfd = new OpenFileDialog();
            sfd.Title = "等速肌力查找文件";
            sfd.Multiselect = true;
            sfd.DefaultExt = ext;
            sfd.Filter = filter;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                fileName = sfd.FileNames;
                result = true;
            }
            return result;
        }
    }
}
