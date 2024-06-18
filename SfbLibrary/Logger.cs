// Copyright (c) 2024 Softbery by Paweł Tobis
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SfbLibrary
{
    public class Logger
    {
        public Log Log { get; private set; }
        public static Action<Log> Write;
        public List<Log> Logs { get; private set; }

        public Logger()
        {
            Log = new Log();
            Logs = new List<Log>();
        }

        private string setMessage(string message)
        {
            var log = new Log(message);
            Log = log;
            return $"[{log.Date} {log.Time}]: {log.Message}";
        }

        private string setMessage(string date, string time, string message)
        {
            var log = new Log(date, time, message);
            Log = log;
            return $"[{log.Date} {log.Time}]: {log.Message}";
        }

        private void Logger_Load(string message)
        {
            Log = new Log(message);
            setMessage(message);
        }

        private void Logger_Load(Log log)
        {
            Log = log;
        }

        public override string ToString()
        {
            var log = new Log();

            return setMessage(log.Message);
        }
    }
}
