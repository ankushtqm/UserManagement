using System;
using System.Collections.Generic;
using System.Collections;
using System.Configuration;
using System.Text;
using ATA.Authentication;
using ATA.Authentication.Providers;
using ATA.ObjectModel;
using SusQTech.Data;
using SusQTech.Data.DataObjects;
using System.Data.SqlClient;
using ATA.LyrisProxy;
using ATA.CodeLibrary;
using System.IO; 
using ATA.Member.Util;

namespace UMShowdifference
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                new ShowGroupMemberDiff().Run();
                Console.WriteLine("No error");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
            } 
        } 
    }

    class OutputLog : IDisposable
    {
        private StreamWriter _writer;
        public OutputLog()
        {
            string outputFileName = "LyrisMemberDiff" + DateTime.Now.Day + ".txt";
            _writer = new StreamWriter(outputFileName);
        }

        public void LogError(string error)
        {
            _writer.WriteLine(error);
        }

        public void LogInfo(string info)
        {
            _writer.WriteLine(info);
        }


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
    }
}
