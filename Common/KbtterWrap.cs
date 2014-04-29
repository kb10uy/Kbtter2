using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kbtter;
using KbtterPolyethylene.Common;
using Kb10uy.Extension;
using TweetSharp;

namespace KbtterPolyethylene.Common
{
    public partial class KbtterContext
    {
        List<TwitterStatus> _stcache;

        public KbtterCore Kbtter { get; private set; }
        public IList<TwitterStatus> StatusCache { get { return _stcache; } }
        public IList<KbtterShortcut> Shortcut { get; private set; }

        public int TimelineStartupLoadStatusCount { get; set; }
        public int TimelineMaxStatusCount { get; set; }
        public bool TimelineReverse { get; set; }
        public bool TimelineStatusTimeAbsolute { get; set; }

        bool _tlstarted;

        public KbtterContext(string ck, string cs)
        {
            Kbtter = new KbtterCore(ck, cs);
            Shortcut = new List<KbtterShortcut>();
            _stcache = new List<TwitterStatus>();

            _tlstarted = false;
            Kbtter.StreamingStatus += Kbtter_StreamingStatus;

            TimelineStartupLoadStatusCount = 100;
            TimelineMaxStatusCount = 200;
            TimelineReverse = false;
            TimelineStatusTimeAbsolute = false;
        }

        void Kbtter_StreamingStatus(TwitterUserStreamStatus obj)
        {
            _stcache.Add(obj.Status);
        }

        public Task<IEnumerable<TwitterStatus>> GetHomeTimelineStatusesByRest()
        {
            return Task<IEnumerable<TwitterStatus>>.Run(() =>
            {
                if (_tlstarted) return null;
                _tlstarted = true;
                var t = Kbtter.GetHomeTimelineStatuses(TimelineStartupLoadStatusCount);
                _stcache.AddRange(t);
                return t;
            });
        }
    }

    public partial class KbtterContext
    {
        public string GetTiny(int num)
        {
            var dn = (double)num;
            var d = num.GetDigitCount();
            if (d <= 3)
            {
                return num.ToString();
            }
            else if (d <= 6)
            {
                dn /= 1000;
                return dn >= 100 ? dn.ToString("#") + "k" : dn.ToString("#.#") + "k";
            }
            else
            {
                dn /= 1000000;
                return dn >= 100 ? dn.ToString("#") + "M" : dn.ToString("#.#") + "M";
            }
        }

        public IList<KbtterHyperlinkInfo> GetReplaceUrlList(TwitterStatus st)
        {
            var ret = new List<KbtterHyperlinkInfo>();
            foreach (var i in st.Entities)
            {
                var t = new KbtterHyperlinkInfo();
                t.Start = i.StartIndex;
                t.End = i.EndIndex;
                switch (i.EntityType)
                {
                    case TwitterEntityType.Url:
                        var u = i as TwitterUrl;
                        t.Display = u.DisplayUrl;
                        t.Navigate = "WEB" + u.ExpandedValue;
                        break;
                    case TwitterEntityType.Media:
                        var d = i as TwitterMedia;
                        t.Display = d.DisplayUrl;
                        t.Navigate = "MED" + d.ExpandedUrl;
                        break;
                    case TwitterEntityType.Mention:
                        var m = i as TwitterMention;
                        t.Display = "@" + m.ScreenName;
                        t.Navigate = "MEN" + m.ScreenName;
                        break;
                    case TwitterEntityType.HashTag:
                        var h = i as TwitterHashTag;
                        t.Display = "#" + h.Text;
                        t.Navigate = "TAG" + h.Text;
                        break;
                }
                ret.Add(t);
            }
            return ret;
        }
    }

    public class KbtterHyperlinkInfo
    {
        public string Display { get; set; }
        public string Navigate { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
    }
}
