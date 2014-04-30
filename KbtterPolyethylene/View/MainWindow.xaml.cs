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
using Kbtter;
using KbtterPolyethylene.Common;
using TweetSharp;

namespace KbtterPolyethylene.View
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        KbtterContext context;

        public MainWindow()
        {
            InitializeComponent();
            InitializeKbtterCore();
            InitializeShortcut();
        }

        void InitializeKbtterCore()
        {
            context = new KbtterContext("5bI3XiTNEMHiamjMV5Acnqkex", "ni2jGjwKTLcdpp1x6nr3yFo9bRrSWRdZfYbzEAZLhKz4uDDErN");
            context.Kbtter.Authenticate("AccessToken", "AccessTokenSecret");

            context.RequestMainTabNew += AddNewTab;

            context.Kbtter.StreamingStatus += (p) => this.Dispatch(() => Kbtter_StreamingStatus(p));
            context.Kbtter.StartStreaming();
        }

        void InitializeShortcut()
        {
            context.Shortcut.Add(new KbtterShortcut
            {
                MainKey = Key.N,
                Modifers = ModifierKeys.Control,
                Action = ToggleNewTweetPane
            });
            context.Shortcut.Add(new KbtterShortcut
            {
                MainKey = Key.Enter,
                Modifers = ModifierKeys.Control,
                Action = SendTweet
            });
        }

        void StartShortcut(Key key, ModifierKeys mod)
        {
            var hits = context.Shortcut.Where((p) => (p.MainKey == key && p.Modifers == mod));
            foreach (var s in hits) s.Action();
        }

        void Kbtter_StreamingStatus(TwitterUserStreamStatus obj)
        {
            AddStatusToTimeline(obj.Status);
        }

        void Exit()
        {
            context.Kbtter.StopStreaming();
            Application.Current.Shutdown();
        }



        #region UI隠蔽

        void AddNewTab(string cmd)
        {
            string type = cmd.Substring(0, 3);
            string target = cmd.Substring(3);
            switch (type)
            {
                case "WEB":
                case "MED":
                    var wbt = CreateMainTab(new TextBlock { Text = target.Substring(0, 20) + "..." },
                                            new Frame { Content = new WebBrowserPage(target) });
                    TabControlMain.Items.Add(wbt);
                    wbt.IsSelected = true;
                    break;

                case "MEN":
                    var uit = CreateMainTab(new TextBlock { Text = target + "さんの情報" },
                                            new Frame { Content = new UserPage() });
                    TabControlMain.Items.Add(uit);
                    uit.IsSelected = true;
                    break;
                case "TAG":
                    break;
                default:
                    break;
            }
        }

        TabItem CreateMainTab(UIElement h, UIElement c)
        {
            var tab = new TabItem();
            var sp = new StackPanel { Orientation = Orientation.Horizontal };
            sp.Children.Add(h);

            var cb = GetCloseButton();
            cb.Tag = tab;
            cb.Click += (s, e) =>
            {
                CloseButton_Click((s as Button).Tag);
            };
            sp.Children.Add(cb);
            tab.Header = sp;
            tab.Content = c;
            tab.Style = TryFindResource("RoundTabItem") as Style;
            return tab;
        }

        //閉じるボタン関係
        // ////////////////////////////////////////////////////
        public Button GetCloseButton()
        {
            Button bt = new Button();
            bt.Template = GetTemplate("CloseButton");
            return bt;
        }

        void CloseButton_Click()
        {
            TabControlMain.Items.RemoveAt(TabControlMain.SelectedIndex);
        }

        void CloseButton_Click(object tab)
        {
            TabControlMain.Items.Remove(tab);
        }

        public ControlTemplate GetTemplate(string name)
        {
            return TryFindResource(name) as ControlTemplate;
        }

        //新しいツイート関係
        // ////////////////////////////////////////////////////
        async void SendTweet()
        {
            if (!ExpanderNewTweet.IsExpanded) return;
            await context.Kbtter.TweetTaskAsync(TextBoxNewTweetText.Text);
            TextBoxNewTweetText.Text = "";
            ExpanderNewTweet.IsExpanded = false;
        }

        void ToggleNewTweetPane()
        {
            ExpanderNewTweet.IsExpanded = !ExpanderNewTweet.IsExpanded;
            if (!ExpanderNewTweet.IsExpanded) TextBoxNewTweetText.Text = "";
        }

        private void TextBoxNewTweetText_TextChanged(object sender, TextChangedEventArgs e)
        {
            var twt = TextBoxNewTweetText.Text;
            TextBlockNewTweetRestLetters.Text = (140 - twt.Length).ToString();
        }


        void AddStatusToTimeline(TwitterStatus st)
        {
            var te = GetTemplate("SelectBorderListBoxItem");
            var el = new ListBoxItem { Content = new Frame { Content = new TweetPage(context, st) }, Template = te };
            if (context.TimelineReverse)
            {
                ListBoxTimeline.Items.Add(el);
                if (ListBoxTimeline.Items.Count > context.TimelineMaxStatusCount)
                {
                    ListBoxTimeline.Items.RemoveAt(0);
                }
            }
            else
            {
                ListBoxTimeline.Items.Insert(0, el);
                if (ListBoxTimeline.Items.Count > context.TimelineMaxStatusCount)
                {
                    ListBoxTimeline.Items.RemoveAt(context.TimelineMaxStatusCount);
                }
            }
        }

        private void GridMain_KeyDown(object sender, KeyEventArgs e)
        {
            StartShortcut(e.Key, Keyboard.Modifiers);
        }


        private void Window_Closed(object sender, EventArgs e)
        {
            Exit();
        }

        private void ExpanderNewTweet_Collapsed(object sender, RoutedEventArgs e)
        {
        }

        #endregion

        #region メニューイベントハンドラ

        private void MenuSetting_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Exit();
        }

        #endregion


    }
}
