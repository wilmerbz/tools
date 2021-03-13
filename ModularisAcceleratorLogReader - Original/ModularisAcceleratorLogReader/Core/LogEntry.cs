using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModularisAcceleratorLogReader.Core
{
    public class LogEntry
    {
        public int Number { get; set; }
        public string EventID { get; set; }
        public string EventType { get; set; }
        public string Importance { get; set; }
        public string Title { get; set; }
        public string DateTime { get; set; }
        public string Source { get; set; }
        public string Content { get; set; }
        public string FullContent { get; set; }

    }

    public enum LogEntryType
    {
        Info,
        Error,
        Debug
    }
}
