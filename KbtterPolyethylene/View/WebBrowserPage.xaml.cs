using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace KbtterPolyethylene.View
{
    /// <summary>
    /// WebBrowserPage.xaml の相互作用ロジック
    /// </summary>
    public partial class WebBrowserPage : Page
    {
        Uri nowuri;
        bool isref = false;
        public WebBrowserPage(string uri)
        {
            InitializeComponent();
            nowuri = new Uri(uri);
            WebBrowserMain.Navigate(nowuri);
        }

        #region イベント
        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserMain.GoBack();
        }

        private void ButtonNext_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserMain.GoForward();
        }

        private void ButtonNavigate_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserMain.Navigate(TextBoxUrl.Text);
        }

        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserMain.Refresh();
        }

        private void ButtonOpenDefault_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(nowuri.ToString());
        }
        #endregion

        private void WebBrowserMain_Navigated(object sender, NavigationEventArgs e)
        {
            nowuri = e.Uri;
            ButtonBack.IsEnabled = WebBrowserMain.CanGoBack;
            ButtonNext.IsEnabled = WebBrowserMain.CanGoForward;
            TextBoxUrl.Text = nowuri.ToString();
        }
    }
}
