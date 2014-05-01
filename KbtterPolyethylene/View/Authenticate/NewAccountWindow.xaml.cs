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
using System.Windows.Shapes;
using System.Diagnostics;

namespace KbtterPolyethylene.View.Authenticate
{
    /// <summary>
    /// NewAccountWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class NewAccountWindow : Window
    {
        internal string PIN;
        public NewAccountWindow(Uri uri)
        {
            InitializeComponent();
            Process.Start(uri.ToString());
        }

        internal string GetPIN()
        {
            ShowDialog();
            return PIN;
        }

        private void ButtonSend_Click(object sender, RoutedEventArgs e)
        {
            PIN = TextBoxPINCole.Text;
            Close();
        }
    }
}
