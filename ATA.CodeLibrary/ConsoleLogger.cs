using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace SusQTech.Logging
{
    public class ConsoleLogger : BaseLogger
    {
        public ConsoleLogger(string eventLogSource)
        {
            LogSource = eventLogSource;
            separator = Environment.NewLine;
        }

        public override void LogInfo(string strInfo)
        {
            if (output)
                Console.WriteLine(strInfo);
            else
                EventLog.WriteEntry(LogSource, strInfo, EventLogEntryType.Information);
        }

        public override void PrintInfo(string strInfo)
        {
            if (output)
                Console.WriteLine(strInfo);
        }

        public override void PrintError(Exception ex, string strMessage)
        {
            PrintInfo(strMessage + ": " + ex.Message + separator + ex.StackTrace);
        }

        public override void LogError(string error)
        {
            if (output)
                Console.Write(error);
            else
                EventLog.WriteEntry(LogSource, error, EventLogEntryType.Error);
        }
    }
}
