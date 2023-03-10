using System;
using System.Collections.Generic;
using System.Text;

namespace SusQTech.Logging
{
    public abstract class BaseLogger
    {
        protected bool output = false;
        protected bool outputIfError = false;
        protected bool includeStackTrace = false;
        protected string separator = Environment.NewLine;
        protected string logSource = string.Empty;
        protected StringBuilder sb = new StringBuilder();

        public abstract void LogInfo(string strInfo);

        public abstract void PrintInfo(string strInfo);

        public abstract void PrintError(Exception ex, string strMessage);

        public void PrintToQueue(string strInfo)
        {
            if (output)
                sb.Append(strInfo + separator);
        }

        public void PrintQueue()
        {
            PrintInfo(sb.ToString());
            sb = null;
            sb = new StringBuilder();
        }

        public abstract void LogError(string error);

        public void LogError(Exception ex, string strMessage)
        {
            if (includeStackTrace)
                LogError(strMessage + ": " + ex.Message + separator + ex.StackTrace);
            else
                LogError(strMessage + ": " + ex.Message);
        }

        public string LogSource
        {
            get { return logSource; }
            set { logSource = value; }
        }

        public bool Output
        {
            get { return output; }
            set { output = value; }
        }

        public bool OutputIfError
        {
            get { return (this.outputIfError); }
            set { this.outputIfError = value; }
        }

        public bool IncludeStackTrace
        {
            get { return includeStackTrace; }
            set { includeStackTrace = value; }
        }

        public string Separator
        {
            get { return separator; }
            set { separator = value; }
        }
    }
}
