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

namespace KbtterPolyethylene.View
{
    /// <summary>
    /// UserPage.xaml の相互作用ロジック
    /// </summary>
    public partial class UserPage : Page
    {

        KbtterContext ctx;
        string target;
        public UserPage(KbtterContext ct, string ta)
        {
            InitializeComponent();
            ctx = ct;
            target = ta;
        }

        async void SetInfomation(string ta)
        {
            var us = await ctx.Kbtter.GetUserTaskAsync(ta);
            if (us == null)
            {
                //rate limited
                //そのユーザーの最終ツイートのキャッシュから取得
                var cache = ctx.StatusCache.Where(p => p.User.ScreenName == ta).LastOrDefault();
                if (cache == null)
                {
                    TextBlockUserName.Text = "ユーザー情報が取得できませんでした。";
                }
                else
                {
                    us = cache.User;
                }
            }
            ImageUserIcon.Source = new BitmapImage(new Uri(us.ProfileImageUrlHttps));
            var bis = new ImageBrush(new BitmapImage(new Uri(us.ProfileBackgroundImageUrlHttps)));
            bis.Stretch = Stretch.UniformToFill;
            bis.Opacity = 0.25;
            GridMain.Background = bis;

            TextBlockUserName.Text = us.Name ?? "";
            TextBlockUserScreenName.Text = us.ScreenName;
            TextBlockUserId.Text = us.Id.ToString();
            TextBlockUserBio.Text = us.Description ?? "";

            TextBlockUserLocation.Text = us.Location ?? "";
            HyperlinkUserUrl.Inlines.Clear();
            if (us.Url != null)
            {
                HyperlinkUserUrl.Inlines.Add(us.Url);
                HyperlinkUserUrl.NavigateUri = new Uri(us.Url);
            }

        }

        #region イベント
        private void HyperlinkUserUrl_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            ctx.RequestAddNewTab("WEB" + e.Uri.ToString());
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            SetInfomation(target);
        }
        #endregion


    }
}
