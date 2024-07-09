// Version: 1.0.0.457
// Copyright (c) 2024 Softbery by Paweł Tobis
using Player.Subtiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Themedit.Subtitles
{
    /// <summary>
    /// Subtitles manager class.
    /// </summary>
    /// <example>
    /// SRT file:
    /// 
    /// 1
    /// 00:00:03,400 --> 00:00:06,177
    /// In this lesson, we're going to
    /// be talking about finance. And
    /// 
    /// 2
    /// 00:00:06,177 --> 00:00:10,009
    /// one of the most important aspects
    /// of finance is interest.
    /// 
    /// 3
    /// 00:00:10,009 --> 00:00:13,655
    /// When I go to a bank or some
    /// other lending institution
    /// 
    /// 4
    /// 00:00:13,655 --> 00:00:17,720
    /// to borrow money, the bank is happy
    /// to give me that money. But then I'm
    /// 
    /// 5
    /// 00:00:17,900 --> 00:00:21,480
    /// going to be paying the bank for the
    /// privilege of using their money. And that
    /// 
    /// 6
    /// 00:00:21,660 --> 00:00:26,440
    /// amount of money that I pay the bank is
    /// called interest. Likewise, if I put money
    /// </example>
    public class SubtitlesManager
    {
        private static string _subtitlesFile = "";
        private List<SrtSub> _startTimeList = new List<SrtSub>();
        private Dictionary<int, SrtSub> _subtitles = new Dictionary<int, SrtSub>();

        /// <summary>
        /// Subtitles line count.
        /// </summary>
        public int Count { get => _subtitles.Count; }
        /// <summary>
        /// Subtitles file path.
        /// </summary>
        public string SubtitlesFilePath { get; set; }
        /// <summary>
        /// Subtitles exist.
        /// </summary>
        /// <param name="time">TimeStamp</param>
        /// <returns>True or false</returns>
        public bool Exist(TimeSpan time)
        {
            return this.GetSubtitlesAsTimeSpan().ContainsKey(time);
        }
        /// <summary>
        /// Subtitles srt table.
        /// </summary>
        /// <param name="id">Int</param>
        /// <returns>Subtitles at id</returns>
        public SrtSub this[int id]
        {
            get => _subtitles[id];
            set => _subtitles[id] = value;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public SubtitlesManager()
        {
            _subtitlesFile = "";
        }
        /// <summary>
        /// Constructor with subtitles path
        /// </summary>
        /// <param name="path"></param>
        public SubtitlesManager(string path)
        {
            _subtitlesFile = path;
            readSrtRegex(path);
        }

        /// <summary>
        /// Get Subtitles dictionary
        /// </summary>
        /// <returns>Full subtitles dictionary</returns>
        public Dictionary<int, SrtSub> GetSubtitles()
        {
            return _subtitles;
        }

        public enum SubtitlesSearchType
        {
            TimeSpan,
            Decimal
        }

        /// <summary>
        /// Get Subtitles by TimeSpan
        /// </summary>
        /// <returns>Subtitles dictionary by time</returns>
        public Dictionary<TimeSpan, SrtSub> GetSubtitlesAsTimeSpan()
        {
            var dic = new Dictionary<TimeSpan, SrtSub>();
            foreach (var item in _subtitles)
            {
                dic.Add(item.Value.StartTime, item.Value);
            }
            return dic;
        }

        private void readSrtRegex(string path)
        {
            var file = new FileInfo(path);
            if (file.Exists)
            {
                var read = File.ReadAllText(file.FullName, Encoding.Default);
                Regex regex = new Regex(@"^\s*(\d+:\d+:\d+,\d+)[^\S\n]+-->[^\S\n]+(\d+:\d+:\d+,\d+)((?:\n(?!\d+:\d+:\d+,\d+\b|\n+\d+$).*)*)", RegexOptions.Multiline);
                var matches = regex.Matches(read);

                int i = 1;
                foreach (Match match in matches)
                {
                    var srtsub = new SrtSub();
                    srtsub.Id = i;
                    srtsub.StartTime = TimeSpan.Parse(match.Groups[1].Value);
                    srtsub.EndTime = TimeSpan.Parse(match.Groups[2].Value);
                    srtsub.Text = new List<string>();
                    var split = match.Groups[3].Value.Split('\n');
                    srtsub.Text.AddRange(split);

                    this._subtitles.Add(srtsub.Id, srtsub);
                    i++;
                }
            }
        }
    }
}
