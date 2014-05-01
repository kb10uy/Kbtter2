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
using TweetSharp;
using Kbtter;
using KbtterPolyethylene.Common;
using Newtonsoft.Json;
using System.IO;

namespace KbtterPolyethylene.View.Authenticate
{
    /// <summary>
    /// AccountSelectWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class AccountSelectWindow : Window
    {
        KbtterContext ctx;
        List<OAuthAccessToken> tokens;

        public AccountSelectWindow(KbtterContext c)
        {
            InitializeComponent();
            ctx = c;

            tokens = File.Exists("accounts.json") ? 
                JsonConvert.DeserializeObject<OAuthAccessToken[]>(File.ReadAllText("accounts.json")).ToList() : 
                new List<OAuthAccessToken>();

        }

        public OAuthAccessToken SelectToken()
        {
            ShowDialog();
            return ListBoxAccounts.SelectedIndex == -1 ? null : tokens[ListBoxAccounts.SelectedIndex];
        }

        #region UIイベント
        private void ButtonNewAccount_Click(object sender, RoutedEventArgs e)
        {
            var req = ctx.Kbtter.GetRequest();
            var naw = new NewAccountWindow(req.RequestUri);
            req.PinCode = naw.GetPIN();
            var atk = ctx.Kbtter.GetAccessToken(req);
            if (atk == null) return;
            tokens.Add(atk);
            File.WriteAllText("accounts.json", JsonConvert.SerializeObject(tokens.ToArray()));
            ListBoxAccounts.Items.Add(String.Format("@{0}({1})", atk.ScreenName, atk.UserId));
        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ListBoxAccounts_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ListBoxAccounts.SelectedIndex != -1) ButtonStart.IsEnabled = true;
        }

        private void ListBoxAccounts_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListBoxAccounts.SelectedIndex == -1) return;
            Close();
        }
        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var i in tokens)
            {
                ListBoxAccounts.Items.Add(String.Format("@{0}({1})", i.ScreenName, i.UserId));
            }
        }

    }
}
