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

        public IList<Tuple<string, string, string>> GetReplaceUrlList(TwitterStatus st)
        {
            var ret = new List<Tuple<string, string, string>>();
            for (int i = 0; i < st.Entities.Urls.Count; i++)
            {
                ret.Add(new Tuple<string, string, string>(
                    st.Entities.Urls[i].Value,
                    st.Entities.Urls[i].DisplayUrl,
                    "WEB" + st.Entities.Urls[i].ExpandedValue));
            }

            for (int i = 0; i < st.Entities.Media.Count; i++)
            {
                ret.Add(new Tuple<string, string, string>(
                    st.Entities.Media[i].Url,
                    st.Entities.Media[i].DisplayUrl,
                    "MED" + st.Entities.Media[i].ExpandedUrl));
            }

            for (int i = 0; i < st.Entities.Mentions.Count; i++)
            {
                ret.Add(new Tuple<string, string, string>(
                    "@" + st.Entities.Mentions[i].ScreenName,
                    "@" + st.Entities.Mentions[i].ScreenName,
                    "MEN" + st.Entities.Mentions[i].ScreenName));
            }

            for (int i = 0; i < st.Entities.HashTags.Count; i++)
            {
                ret.Add(new Tuple<string, string, string>(
                    "#" + st.Entities.HashTags[i].Text,
                    "#" + st.Entities.HashTags[i].Text,
                    "TAG" + st.Entities.HashTags[i].Text));
            }

            return ret;
        }
    }
}
