// Copyright (c) 2024 Softbery by Paweł Tobis
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SfbLibrary
{
    public class Log
    {
        public string Date { get; set; }
        public string Time { get; set; }
        public string Message { get; set; }
        public string Version { get; private set; } = "ver.1.0.0.2";

        public Log()
        {
            Date = DateTime.Now.ToString("dd.MM.yyyy dddd");
            Time = DateTime.Now.ToString("HH: mm: ss");
            Message = $"{this.Version}";
        }

        public Log(string message)
        {
            Date = DateTime.Now.ToString("dd.MM.yyyy dddd");
            Time = DateTime.Now.ToString("HH: mm: ss");
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public Log(string message, Exception ex)
        {
            Date = DateTime.Now.ToString("dd.MM.yyyy dddd");
            Time = DateTime.Now.ToString("HH: mm: ss");
            Message = $"{message}: \n\n\n[{ex.HResult}]\n[{ex.Message}]" ?? throw new ArgumentNullException(nameof(message));
        }
        public Log(string time, string message)
        {
            Date = DateTime.Now.ToString("dd.MM.yyyy dddd");
            Time = time ?? throw new ArgumentNullException(nameof(time));
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public Log(string date, string time, string message)
        {
            Date = date ?? throw new ArgumentNullException(nameof(date));
            Time = time ?? throw new ArgumentNullException(nameof(time));
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public override string ToString()
        {
            return $"{Message}";
        }
    }
}
