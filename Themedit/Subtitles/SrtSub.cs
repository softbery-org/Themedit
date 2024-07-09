// Version: 1.0.0.448
// Copyright (c) 2024 Softbery by Paweł Tobis
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Themedit.Subtitles
{
    public class SrtSub
    {
        /// <summary>
        /// Subtitles id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Subtitles start time
        /// </summary>
        public TimeSpan StartTime { get; set; }
        /// <summary>
        /// Subtitles end time
        /// </summary>
        public TimeSpan EndTime { get; set; }
        /// <summary>
        /// Subtitles show text
        /// </summary>
        public List<string> Text { get; set; }
        /// <summary>
        /// Srt subtitles table.
        /// </summary>
        /// <param name="id">Subtitle id</param>
        /// <returns>Subtitles on id.</returns>
        public SrtSub this[int id]
        {
            get => this[id];
            set => this[id] = value;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public SrtSub()
        {

        }

        public void Add(int id, TimeSpan start, TimeSpan end, string[] text)
        {
            this[id] = new SrtSub { 
                                    Id = id, 
                                    StartTime = start, 
                                    EndTime = end, 
                                    Text = text.ToList() 
                                   };
        }

        public void Remove(int id)
        {
            
        }

        public void AddSubtitles(string[] text, TimeSpan start, TimeSpan end)
        {
            
        }

        /*/// <summary>
        /// AddTrack subtitles
        /// </summary>
        /// <param name="startTime"></param>
        public void AddStartTime(TimeSpan startTime)
        {
            StartTime = $"{startTime.Hours:00}:{startTime.Minutes:00}:{startTime.Seconds:00},{startTime.TotalMilliseconds:000}"; ;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endTime"></param>
        public void AddEndTime(TimeSpan endTime)
        {
            EndTime = $"{endTime.Hours:00}:{endTime.Minutes:00}:{endTime.Seconds:00},{endTime.TotalMilliseconds:000}";
        }*/
    }
}
