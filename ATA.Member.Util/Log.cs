using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Win32; 
//using Microsoft.SharePoint;
namespace ATA.Member.Util
{
    public static class Log
    {
        private const string _logName = "ATAMember";
        private const string defaultLogSource = "ATAMember";
        public const string FuelsLogSource = "Fuels";
        
        public static void LogError(string message)
        {
            LogError(message, defaultLogSource);
        }

        public static void LogErrorFormat(string message, params string[] args)
        {
            LogError(string.Format(message,args) , defaultLogSource);
        }

        public static void LogWarning(string message)
        {
            LogWarning(message, defaultLogSource);
        }

        public static void LogInfo(string message)
        {
            LogInfo(message, defaultLogSource);
        }

        public static void LogError(string message, string logSource)
        {
            WriteMessage(message, EventLogEntryType.Error, logSource);
        }

        public static void LogWarning(string message, string logSource)
        {
            WriteMessage(message, EventLogEntryType.Warning, logSource);
        }

        public static void LogInfo(string message, string logSource)
        {
            WriteMessage(message, EventLogEntryType.Information, logSource);
        }

        public static void WriteLog(string message, EventLogEntryType eventType, string logSource)
        {
            WriteMessage(message, eventType, logSource);
        } 

        #region Private methods
        private static void WriteMessage(string message, EventLogEntryType logType, string logSource)
        {
            EventLog eventLog = null; 
            try
            {
                eventLog = new EventLog(_logName);
                eventLog.Source = logSource;
                eventLog.WriteEntry(message, logType); 
            }
            catch (Exception)
            { 
            //    SPSecurity.RunWithElevatedPrivileges(delegate()
            //  {
            //      logWithHightPrevilege(message, logSource, logType);
            //  });  
            }
            if (eventLog != null)
            {
                try
                {
                    eventLog.Dispose();
                }
                catch (Exception) { }//ignore it
            } 
        }

        private static void logWithHightPrevilege(string message, string logSource,  EventLogEntryType logType)
        {
            EventLog eventLog = null;
            try
            {
                eventLog = new EventLog(_logName);
                eventLog.Source = logSource;
                eventLog.WriteEntry(message, logType);
            }
            catch (Exception)//ignore the error
            { 
            }
            if (eventLog != null)
            {
                try
                {
                    eventLog.Dispose();
                }
                catch (Exception) { }//ignore it
            }
        }
        #endregion
    }
}


