using ModularisAcceleratorLogReader.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModularisAcceleratorLogReader.Core
{
    public static class LogReader
    {

        public static LogEntries LogEntriesCollection = new LogEntries();
        private const string EntryStartLine = "======================================";
        private const string EntryInformationLine = "             INFORMATION";
        private const string EntryErrorLine = "                ERROR";
        private const string DebugLine = "                DEBUG";
        private static string[] EntryTypeLines = new[] { EntryInformationLine, EntryErrorLine, DebugLine };
        private const string DateTimeLinePrefix = " Date/Time:        ";
        private const string SourceLinePrefix = " Source:           ";
        private const string ImportanceLinePrefix = " Importance:       ";
        private const string TitleLinePrefix = " Title:            ";

        private const string EventIDLinePrefix = " Event ID:         ";
        private const string ThreadIDLinePrefix = " Thread ID:        ";
        private const string TraceIDPrefix = " Trace ID:         ";
        private const string TraceIndexPrefix = " Trace Index:      ";

        public static LogEntries ReadLogFile(string filePath)
        {
            try
            {
                LogEntriesCollection.Clear();

                var fileContent = File.ReadAllLines(filePath);
                LogEntries logEntries = new LogEntries();
                LogEntry currentLogEntry = null;
                int logEntryNumber = 0;
                for (int i = 0; i < fileContent.Length; i++)
                {
                    var line = fileContent[i];

                    if (line == EntryStartLine && (i + 1) < fileContent.Length && EntryTypeLines.Contains(fileContent[i + 1]))
                    {
                        logEntryNumber++;
                        currentLogEntry = new LogEntry();
                        currentLogEntry.Number = logEntryNumber;
                        currentLogEntry.Content = string.Empty;
                        currentLogEntry.FullContent = string.Empty;
                        currentLogEntry.EventType = fileContent[i + 1].Trim();

                        logEntries.Add(currentLogEntry);
                    }
                    else if (currentLogEntry != null)
                    {

                        if (line.StartsWith(DateTimeLinePrefix))
                        {
                            currentLogEntry.DateTime = line.Replace(DateTimeLinePrefix, "");
                        }
                        else if (line.StartsWith(SourceLinePrefix))
                        {
                            currentLogEntry.Source = line.Replace(SourceLinePrefix, "");
                        }
                        else if (line.StartsWith(ImportanceLinePrefix))
                        {
                            currentLogEntry.Importance = line.Replace(ImportanceLinePrefix, "");
                        }
                        else if (line.StartsWith(TitleLinePrefix))
                        {
                            currentLogEntry.Title = line.Replace(TitleLinePrefix, "");
                        }
                        else if (line.StartsWith(EventIDLinePrefix))
                        {
                            currentLogEntry.EventID = line.Replace(EventIDLinePrefix, "");
                        }
                        else if (line.StartsWith(ThreadIDLinePrefix))
                        {
                            currentLogEntry.ThreadID = line.Replace(ThreadIDLinePrefix, "");
                        }
                        else if (line.StartsWith(TraceIDPrefix))
                        {
                            currentLogEntry.TraceID = line.Replace(TraceIDPrefix, "");
                        }
                        else if (line.StartsWith(TraceIndexPrefix))
                        {
                            currentLogEntry.TraceIndex = line.Replace(TraceIndexPrefix, "");
                        }

                    }


                    if (currentLogEntry != null)
                    {
                        currentLogEntry.FullContent += line;
                        currentLogEntry.FullContent += System.Environment.NewLine;
                    }

                }

                for (int i = logEntries.Count - 1; i >= 0; i--)
                {
                    var logEntry = logEntries[i];
                    if (logEntry.Title == "ExecuteCommandOnCollection:Modularis.GetAgents.Start" || logEntry.Title == "ExecuteCommandOnCollection:Modularis.GetAgents.End")
                    {
                        continue;
                    }
                    LogEntriesCollection.Add(logEntry);
                }

            }
            catch (Exception)
            {
            }

            return LogEntriesCollection;
        }

        public static void ClearLogFile(string filePath)
        {
            try
            {
                var lines = new string[] { "" };
                File.WriteAllLines(filePath, lines);
            }
            catch (Exception)
            {

            }
        }

        public static LogEntries FilterLogEntries(string searchText)
        {
            var filteredLogItems = new LogEntries();

            if (LogEntriesCollection == null || LogEntriesCollection.Count == 0) return filteredLogItems;


            if (string.IsNullOrEmpty(searchText)) return LogEntriesCollection;

            LogEntry logEntry = null;
            for (int i = 0; i < LogEntriesCollection.Count; i++)
            {
                logEntry = LogEntriesCollection[i];

                if (logEntry.FullContent.IndexOf(searchText, StringComparison.InvariantCultureIgnoreCase) > -1)
                {
                    filteredLogItems.Add(logEntry);
                }
            }

            return filteredLogItems;
        }

    }
}
