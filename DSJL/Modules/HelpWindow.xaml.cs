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

namespace DSJL.Modules
{
    /// <summary>
    /// HelpWindow.xaml 的交互逻辑
    /// </summary>
    public partial class HelpWindow : Window
    {
        List<string> menuSourceList = new List<string>();
        List<string> contentList;
        public HelpWindow()
        {
            InitializeComponent();
            menuSourceList = new List<string>() { "关于软件", "法律声明", "联系方式" };
            lbMenu.ItemsSource = menuSourceList;

            contentList = new List<string>() { 
                "软件名称：Isomed等速肌力测试数据分析及评价软件\r\n版本： 演示版 3.0\r\n版权所有：国家体育总局体育科学研究所\r\n软件设计：胡水清  米奕翔\r\n编程人员：南鹤  詹宝群  冯葆欣\r\n软件说明书编写：胡水清",
                "    本软件为国家体育总局体育科学研究所基本业务费支持课题《优秀运动员等速肌力实验室测试及评价方法的建立》(课题编号13-07) 的成果之一，软件版权归国家体育总局体育科学研究所所有，受法律保护。\r\n    未经国家体育总局体育科学研究所许可，任何单位及个人不得以任何方式或理由对本软件产品及材料的任何部分进行使用、复制、修改、抄录、传播或与其它产品捆绑使用、销售。\r\n    凡侵犯本软件著作权等知识产权的，必依法追究其法律责任。特此郑重法律声明！",
                "    Isomed2000等速肌力测试数据分析及评价软件1.1.2版本可以永久免费使用。如果您在使用中遇到问题或者您对软件有什么好的建议，请通过下面的方式与我们联系，您的问题或建议可以帮助我们更好地改善软件的功能，感谢您的支持。同时，我们希望能够实现实验室之间的数据分享，如果您愿意分享您的测试数据，您也将获得其他实验室的数据。分享的数据仅作为参考值引用，不能用于论文发表、课题研究等使用。\r\n    希望通过该软件的分享，能够带来更广泛的实验室间交流、分享与合作。\r\n\r\n联系人：胡水清\r\n地址：北京市东城区体育馆路11号，邮编：100061\r\n电话：010-87182574，13810327180\r\n传真：010-87182600\r\nEmail：hushuiqing@ciss.cn，24218343@qq.com"
            };
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lbMenu.SelectedIndex = 0;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void lbMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tbContent.Text = contentList[lbMenu.SelectedIndex];
            //switch (lbMenu.SelectedIndex) {
            //    case 0:
            //    case 1:
            //    case 2:
                   
            //        break;
            //    case 3:
            //        break;
            //}
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
