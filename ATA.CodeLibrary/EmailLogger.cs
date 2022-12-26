using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;

namespace SusQTech.Logging
{
    public class EmailLogger : BaseLogger
    {
        string _to = string.Empty;
        string _bcc = string.Empty;
        string _from = string.Empty;
        string _smtp = string.Empty;
        string _subject = string.Empty;

        public EmailLogger(string to, string bcc, string from, string subject, string smtpServer, string eventLogSource)
        {
            _to = to;
            _bcc = bcc;
            _from = from;
            _subject = subject;
            _smtp = smtpServer;
            logSource = eventLogSource;
            separator = Environment.NewLine;            
        }

        public override void LogInfo(string strInfo)
        {
            if (output)
            {
                SendEmail(strInfo);
            }
            else
                EventLog.WriteEntry(LogSource, strInfo, EventLogEntryType.Information);
        }

        public override void PrintInfo(string strInfo)
        {
            if (output)
                SendEmail(strInfo);
        }

        public override void PrintError(Exception ex, string strMessage)
        {
            if (outputIfError)
                output = true;

            if (output)
                SendEmail(strMessage + ": " + ex.Message + Environment.NewLine + ex.StackTrace);
        }

        public override void LogError(string error)
        {
            if (outputIfError)
                output = true;

            if (output)
                SendEmail(error);
            else
                EventLog.WriteEntry(LogSource, error, EventLogEntryType.Error);
        }

        public void SendEmail(string message)
        {
            string from = _from;
            string to = _to;
            string subject = _subject;
            string body = message;
            string smtpServer = _smtp;

            if (to != string.Empty)
            {
                MailMessage mm = new MailMessage(from, to, subject, body);

                SmtpClient smtp = new SmtpClient(smtpServer);
                try
                {
                    smtp.Send(mm);
                }
                catch { }
            }
        }

        #region Public Properties 

        public string To
        {
            get { return _to; }
            set { _to = value; }
        }

        public string BCC
        {
            get { return _bcc; }
            set { _bcc = value; }
        }

        public string From
        {
            get { return _from; }
            set { _from = value; }
        }

        public string Subject
        {
            get { return _subject; }
            set { _subject = value; }
        }

        public string SMTPServer
        {
            get { return _smtp; }
            set { _smtp = value; }
        }

        #endregion
    }
}
