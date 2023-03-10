using System;
using System.Collections.Generic; 
using System.Text;
using System.IO;
using ATA.Member.Util;

namespace ATA.Member.Util.ConsoleUtil
{
    public class FileLog : IDisposable
    {
        private StreamWriter _writer;
        public FileLog() : this("OutputLog")
        { 
        }

        public FileLog(string fileNamePrefix) 
        {
            InitializeLog(fileNamePrefix, null);
        }

        public FileLog(string fileNamePrefix, string filePath)
        {
            InitializeLog(fileNamePrefix, filePath);
        } 
            
        public void LogError(string error)
        {
            _writer.WriteLine(error);
        }

        public void LogInfo(string info)
        {
            _writer.WriteLine(info);
        } 

        

        #region IDisposable Members 
        public void Dispose()
        {
            if (_writer != null)
            {
                try
                {
                    _writer.Flush();
                    _writer.Close();
                    _writer.Dispose();
                }
                catch (Exception) { }
            }
        } 
        #endregion

        #region Private methods
        private void InitializeLog(string fileNamePrefix, string filePath)
        {
            try
            {
                string outputFileName = fileNamePrefix + DateTime.Now.Day + ".txt";
                if (!string.IsNullOrEmpty(filePath))
                    outputFileName = Path.Combine(filePath, outputFileName);
                _writer = new StreamWriter(outputFileName);
            }
            catch (Exception e)
            {
                Log.LogError("Error in creating " + fileNamePrefix + " log file: " + e.ToString());
                throw;
            }
        }
        #endregion
    }
}
