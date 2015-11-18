using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace DSJL.Tools
{
    public class MessageBoxTool
    {

        public static void ShowConfirmMsgBox(string msg)
        {
            MessageBox.Show(msg, "系统信息", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void ShowErrorMsgBox(string msg)
        {
            MessageBox.Show(msg, "系统错误", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static MessageBoxResult ShowAskMsgBox(string askMsg)
        {
            return MessageBox.Show(askMsg, "系统信息", MessageBoxButton.YesNo, MessageBoxImage.Question);
        }
    }
}
