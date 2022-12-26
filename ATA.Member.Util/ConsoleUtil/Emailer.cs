using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Net.Mail;

namespace ATA.Member.Util.ConsoleUtil
{
    public class ErrorEmailer
    {
        private static string _email = "naamidala@airlines.org";
        private static string _bccEmail = null;
        private static string _errorEmailTitle = "Error on Airlines.org";
        private static string _errorEmailFromAddress = "errors@Fuels.airlines.org";
        private static string _sMTPServer = null;
        private static bool initialized = false;

        public static void SendErrorEmail(string error)
        {
            Initialize();
            MailMessage errorReport = new MailMessage(new MailAddress(_errorEmailFromAddress), new MailAddress(_email));
            if (!string.IsNullOrEmpty(_bccEmail))
                errorReport.CC.Add(new MailAddress(_bccEmail));
            errorReport.IsBodyHtml = false;
            errorReport.Subject = _errorEmailTitle;
            errorReport.Body = error;
            SmtpClient client = new SmtpClient(_sMTPServer);
            client.Send(errorReport);
            errorReport.Dispose();
        }

        private static void Initialize()
        {
            if (initialized)
                return;
            _email = RetrieveValueFromConf("ReportLogErrorTo", _email);
            _bccEmail = RetrieveValueFromConf("BccLogErrorTo", _bccEmail);
            _errorEmailTitle = RetrieveValueFromConf("ErrorEmailTitle", _errorEmailTitle);
            _errorEmailFromAddress = RetrieveValueFromConf("ErrorEmailFromAddress", _errorEmailFromAddress);
            _sMTPServer = RetrieveValueFromConf("SMTPServer", _sMTPServer);
            if (_sMTPServer == null)
            {
                string errorMsg = "SMTPServer key is missing in AppSettings";
                throw new InvalidOperationException(errorMsg);
            }
            initialized = true;

        }

        private static string RetrieveValueFromConf(string key, string defaultValue)
        {
            return string.IsNullOrEmpty(ConfigurationSettings.AppSettings[key]) ?
                        defaultValue : ConfigurationSettings.AppSettings[key];
        }
    }
}
