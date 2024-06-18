// Copyright (c) 2024 Softbery by Pawe³ Tobis

// Version: 1.0.0.13
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Player.Subtiles
{
    public class SrtSub
    {
        public int Id { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public List<string> Text { get; set; }

        public SrtSub()
        {

        }

        public void AddStartTime(TimeSpan startTime)
        {
            StartTime = $"{startTime.Hours:00}:{startTime.Minutes:00}:{startTime.Seconds:00},{startTime.TotalMilliseconds:000}"; ;
        }

        public void AddEndTime(TimeSpan endTime)
        {
            EndTime = $"{endTime.Hours:00}:{endTime.Minutes:00}:{endTime.Seconds:00},{endTime.TotalMilliseconds:000}";
        }
    }
}
