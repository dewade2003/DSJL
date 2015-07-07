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
using DSJL.Model;

namespace DSJL.Modules.Test.Export
{
    /// <summary>
    /// CheckParamsPage.xaml 的交互逻辑
    /// </summary>
    public partial class CheckParamsPage : Page
    {
        List<Model.Parameter> paramList;
        public CheckParamsPage()
        {
            InitializeComponent();
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            ExportProgressPage.ParamList = paramList.FindAll(x => x.IsChecked == true);
            if (ExportProgressPage.ParamList.Count > 0)
            {
                ExportProgressPage.IsExportParams = true;
            }
            else {
                ExportProgressPage.IsExportParams = false;
            }
            NavigationService.Navigate(new Uri(@"Modules/Test/Export/ExportProgressPage.xaml", UriKind.Relative));
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            BindParam();
        }

        private void BindParam() {
            paramList = Parameter.GetAllParams();
            Binding b = new Binding() { Source = paramList };
            lbParams.SetBinding(ListBox.ItemsSourceProperty, b);
        }

        private void btnExortInfo_Click(object sender, RoutedEventArgs e)
        {
       
            ExportProgressPage.IsExportParams = false;
            NavigationService.Navigate(new Uri(@"Modules/Test/Export/ExportProgressPage.xaml", UriKind.Relative));
        }

        //选择默认
        private void rbDefault_Checked(object sender, RoutedEventArgs e)
        {
            Parameter.CheckDefault();
            BindParam();
        }

        //选择全部
        private void rbCheckAll_Checked(object sender, RoutedEventArgs e)
        {
            Parameter.CheckAll();
            BindParam();
        }
    }
}
