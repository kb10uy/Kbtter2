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
            context = new KbtterContext("fV3meTB3URhtSx7WGjQ", "3AVAf20e64Al9edgrrJnJjI5a67fp2WUPxP9xtnLsY");
            context.Kbtter.Authenticate("318376822-6cD62AWw4RW2hQAPNIWS1DeILgOBXooXbxinTUiD", "jBBlzqiBDiT0UukgPFyfK1uWd5VU3P832cD5RpyoO36Hl");

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

        public ControlTemplate GetTemplate(string name)
        {
            return TryFindResource(name) as ControlTemplate;
        }

        void ToggleNewTweetPane()
        {
            ExpanderNewTweet.IsExpanded = !ExpanderNewTweet.IsExpanded;
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
