using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;

namespace SusQTech.Logging
{
    public class EventLogger : BaseLogger
    {        
        public EventLogger(string eventLogSource)
        {
            logSource = eventLogSource;
            separator = Environment.NewLine;            
        }

        public override void LogInfo(string strInfo)
        {
            EventLog.WriteEntry(LogSource, strInfo, EventLogEntryType.Information);
        }

        public override void PrintInfo(string strInfo)
        {
            if (output)
                EventLog.WriteEntry(LogSource, strInfo, EventLogEntryType.Information);
        }

        public override void PrintError(Exception ex, string strMessage)
        {
            if (output)
                EventLog.WriteEntry(LogSource, strMessage, EventLogEntryType.Information);
        }

        public override void LogError(string error)
        {
            if (error.IndexOf("Save Conflict") < 0)
                EventLog.WriteEntry(LogSource, error, EventLogEntryType.Error);
        }        
    }
}
