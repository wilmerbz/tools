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

        public static List<LogEntry> LogEntries = new List<LogEntry>();
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

        public static List<LogEntry> ReadLogFile(string filePath)
        {
            try
            {
                var fileContent = File.ReadAllLines(filePath);
                List<LogEntry> logEntries = new List<LogEntry>();
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
                        else
                        {
                            currentLogEntry.Content += line;
                            currentLogEntry.Content += System.Environment.NewLine;
                        }

                    }


                    if (currentLogEntry != null)
                    {
                        currentLogEntry.FullContent += line;
                        currentLogEntry.FullContent += System.Environment.NewLine;
                    }

                }

                for (int i = logEntries.Count-1; i >= 0; i--)
                {
                    var logEntry = logEntries[i];
                    if(logEntry.Title == "ExecuteCommandOnCollection:Modularis.GetAgents.Start" || logEntry.Title == "ExecuteCommandOnCollection:Modularis.GetAgents.End")
                    {
                        logEntries.RemoveAt(i);
                    }
                }

                logEntries.Reverse();
                LogEntries = logEntries;

            }
            catch (Exception)
            {
            }

            return LogEntries;
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
    }
}
