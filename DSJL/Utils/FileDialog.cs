using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DSJL.Utils
{
    public class FileDialog
    {

        public static readonly string excelExt = "xls";
        public static readonly string excelFilter = "Excel文件(*.xls)|*.xls";
        public static bool openSaveFileDialog(out string fileName,string filter,string ext,string defaultFileName) {
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
    }
}
