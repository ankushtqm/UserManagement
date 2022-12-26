using System;
using System.Collections.Generic;
using System.Text;

namespace SusQTech.Logging
{
    public class LoggerJob
    {
        protected BaseLogger logger;
                
        #region Logging

        public void LogError(Exception ex, string error)
        {
            if (logger != null)
                logger.LogError(ex, error);
        }

        public void LogError(string error)
        {
            if (logger != null)
                logger.LogError(error);
        }

        public void LogInfo(string strInfo)
        {
            if (logger != null)
                logger.LogInfo(strInfo);
        }

        public void PrintError(Exception ex, string error)
        {
            if (logger != null)
                logger.PrintError(ex, error);
        }

        public void PrintInfo(string strInfo)
        {
            if (logger != null)
                logger.PrintInfo(strInfo);
        }

        public void PrintToQueue(string strInfo)
        {
            if (logger != null)
                logger.PrintToQueue(strInfo);
        }

        public void PrintQueue()
        {
            if (logger != null)
                logger.PrintQueue();
        }

        #endregion

        public BaseLogger Logger
        {
            get { return logger; }
            set { logger = value; }
        }
    }
}
