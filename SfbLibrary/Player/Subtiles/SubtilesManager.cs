// Copyright (c) 2024 Softbery by Pawe³ Tobis

// Version: 1.0.0.13
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Player.Subtiles
{

    /// <summary>
    /// Example srt file
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
    /// </summary>

    public class SubtilesManager
    {
        private static string _subtilesFile = "";
        private List<SrtSub> _startTimeList = new List<SrtSub>();
        private Dictionary<int, SrtSub> _subtiles = new Dictionary<int, SrtSub>();

        public int Count { get => _subtiles.Count; }
        //public SrtSub[] StartList { get => _startTimeList.ToArray(); }
        public bool Exist(string time) 
        {
            return this.GetSubtilesByStringTime().ContainsKey(time);
        }

        public SrtSub this[int id]
        {
            get => _subtiles[id];
            set => _subtiles[id] = value;
        }
        public SubtilesManager()
        {

        }

        public SubtilesManager(string path)
        {
            _subtilesFile = path;
            readSRT(path);
        }

        private void readSRT(string path)
        {
            var file = new FileInfo(path);
            if (file.Exists)
            {
                var read = File.ReadAllLines(file.FullName);
                
                for (int j = 0; j < read.Length - 1; j++)
                {
                    var strsub = new SrtSub();
                    strsub.Id = Convert.ToInt32(read[j]);
                    var split = read[j + 1].Replace("-->", "|");
                    var splits = split.Split('|');
                    
                    strsub.StartTime = splits[0];
                    strsub.EndTime = splits[1];
                    
                    strsub.Text = new List<string>();
                    
                    var i = j + 2;
                    
                    while (true)
                    {
                        if (read[i]!="")
                        {
                            strsub.Text.Add(read[i]);
                            i++;
                        }
                        else
                        {
                            j = i;
                            break;
                        }
                    }

                    this.
                    _subtiles.Add(strsub.Id, strsub);
                }
            }
        }
        private TimeSpan timerSpan(string time)
        {
            return TimeSpan.Parse(time);
        }

        public Dictionary<int, SrtSub> GetSubtiles()
        {
            return _subtiles;
        }

        public Dictionary<string, SrtSub> GetSubtilesByStringTime()
        {
            var dic = new Dictionary<string, SrtSub>();
            foreach (var item in _subtiles)
            {
                dic.Add(item.Value.StartTime, item.Value);
            }
            return dic;
        }
    }

    public static class TimeSpanFromString
    {
        public static TimeSpan GetTimeSpan(this string txt)
        {
            txt = txt.Trim();
            return TimeSpan.Parse(txt);
        }
    }
}
