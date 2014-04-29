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
        public TweetPage(KbtterContext ct, TwitterStatus st)
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
            HyperlinkScreenName.NavigateUri = new Uri("MEN" + stat.User.ScreenName, UriKind.RelativeOrAbsolute);
            HyperlinkScreenName.RequestNavigate += hl_RequestNavigate;

            SetMainText();

            if (rtuser != null)
            {
                TextBlockRetweetedBy.Text = "Retweeted by " + rtuser.Name;
            }
            else
            {
                TextBlockRetweetedBy.Visibility = Visibility.Hidden;
            }
            TextBlockFavoriteCount.Text = ctx.GetTiny((int)dynraw.favorite_count);
            TextBlockRetweetCount.Text = ctx.GetTiny(stat.RetweetCount);

        }

        void SetMainText()
        {
            var url = ctx.GetReplaceUrlList(stat);
            var spl = new List<Tuple<string, bool, Tuple<string, string>>>();
            spl.Add(new Tuple<string, bool, Tuple<string, string>>(stat.TextDecoded, false, null));
            foreach (var i in url)
            {
                bool suc = false;
                Tuple<string, bool, Tuple<string, string>> bfs = null, urs = null, afs = null;
                int id = 0;
                foreach (var s in spl.Where(p => !p.Item2))
                {
                    id = spl.IndexOf(s);
                    int fui = s.Item1.IndexOf(i.Item1);
                    if (fui != -1)
                    {
                        //現在のスパンにurl発見
                        suc = true;
                        var bef = s.Item1.Substring(0, fui);
                        var aft = s.Item1.Substring(bef.Length + i.Item1.Length);
                        bfs = new Tuple<string, bool, Tuple<string, string>>(bef, false, null);
                        urs = new Tuple<string, bool, Tuple<string, string>>(
                            i.Item1, true,
                            new Tuple<string, string>(i.Item2, i.Item3));
                        afs = new Tuple<string, bool, Tuple<string, string>>(aft, false, null);
                    }
                }
                if (suc)
                {
                    spl.RemoveAt(id);
                    spl.Insert(id, bfs);
                    spl.Insert(id + 1, urs);
                    spl.Insert(id + 2, afs);
                }
            }
            TextBlockMainText.Inlines.Clear();
            foreach (var i in spl)
            {
                if (i.Item2)
                {
                    Hyperlink hl = new Hyperlink();
                    hl.NavigateUri = new Uri(i.Item3.Item2, UriKind.RelativeOrAbsolute);
                    hl.Inlines.Add(i.Item3.Item1);
                    hl.RequestNavigate += hl_RequestNavigate;
                    TextBlockMainText.Inlines.Add(hl);
                }
                else
                {
                    TextBlockMainText.Inlines.Add(i.Item1);
                }
            }
        }

        void hl_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            e.Handled = true;
            var type = e.Uri.ToString().Substring(0, 3);
            var act = e.Uri.ToString().Substring(3);
            switch (type)
            {
                case "WEB":
                    break;
                case "MED":
                    break;
                case "MEN":
                    break;
                case "TAG":
                    break;
                default:
                    break;
            }
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
