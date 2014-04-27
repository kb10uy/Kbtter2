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
using Kbtter;
using TweetSharp;
using KbtterPolyethylene.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KbtterPolyethylene.View
{
    /// <summary>
    /// TweetPage.xaml の相互作用ロジック
    /// </summary>
    public partial class TweetPage : Page
    {
        KbtterContext ctx;
        TwitterStatus stat;
        TwitterUser rtuser;
        int fav, rt;
        dynamic dynraw;
        public TweetPage(KbtterContext ct,TwitterStatus st)
        {
            InitializeComponent();
            ctx = ct;
            stat = st;
            dynraw = JObject.Parse(st.RawSource);
            if (stat.RetweetedStatus != null)
            {
                rtuser = stat.User;

                stat = stat.RetweetedStatus;
                dynraw = dynraw.retweeted_status;
                this.Background = new SolidColorBrush(new Color { R = 200, G = 255, B = 200, A = 127 });
            }
            else
            {
                this.Background = new SolidColorBrush(new Color { R = 200, G = 255, B = 255, A = 127 });
            }
            SetStatus();
        }

        void SetStatus()
        {
            ImageUserIcon.Source = new BitmapImage(new Uri(stat.User.ProfileImageUrlHttps));
            TextBlockUserName.Text = stat.User.Name;
            HyperlinkScreenName.Inlines.Clear();
            HyperlinkScreenName.Inlines.Add(stat.User.ScreenName);

            TextBlockMainText.Text = stat.TextDecoded;
            if (rtuser != null)
            {
                TextBlockRetweetedBy.Text = "Retweeted by " + rtuser.Name;
            }
            else
            {
                TextBlockRetweetedBy.Visibility = Visibility.Hidden;
            }
            ButtonFavoriteContent.Children.Add(new TextBlock { FontSize = 8, Text = ctx.GetTiny((int)dynraw.favorite_count) });

        }

        #region UIイベント
        private async void ButtonFavorite_Click(object sender, RoutedEventArgs e)
        {
            if (ButtonFavorite.IsChecked ?? false)
            {
                await ctx.Kbtter.FavoriteTaskAsync(stat.Id);
            }
            else
            {
                await ctx.Kbtter.UnfavoriteTaskAsync(stat.Id);
            }
            
        }

        private void ButtonRetweet_Click(object sender, RoutedEventArgs e)
        {

        }

        private void HyperlinkScreenName_MouseEnter(object sender, MouseEventArgs e)
        {
            HyperlinkScreenName.Foreground = Brushes.DeepSkyBlue;
        }

        private void HyperlinkScreenName_MouseLeave(object sender, MouseEventArgs e)
        {
            HyperlinkScreenName.Foreground = Brushes.Gray;
        }

        private void HyperlinkScreenName_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion


    }
}
