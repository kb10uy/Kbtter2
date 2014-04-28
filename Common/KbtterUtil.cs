using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Kbtter;
using TweetSharp;


namespace KbtterPolyethylene.Common
{
    internal static class KbtterUtil
    {
        public static void Dispatch(this UIElement elm, Action act)
        {
            elm.Dispatcher.BeginInvoke(act);
        }
    }

    public class KbtterShortcut
    {
        public Key MainKey { get; set; }
        public ModifierKeys Modifers{ get; set; }
        public Action Action{ get; set; }
    }
}
