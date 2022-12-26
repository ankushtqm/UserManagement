using System;
using System.Collections.Generic;
using System.Text;
//using Microsoft.SharePoint;
using System.Configuration;
using System.Collections.Specialized;
using System.Web.Configuration;

namespace ATA.Member.Util
{ 

    public static class Conf
    {
        private static NameValueCollection _siteSettings = ConfigurationManager.AppSettings;
        private static KeyValueConfigurationCollection _webSettings;
        private static ConnectionStringSettingsCollection _connectionSettings = ConfigurationManager.ConnectionStrings; 
       
        /// <summary></summary>
        public static string ConnectionString
        {
            get
            {
                string connectionStringName = GetValueFromConfig("connectionStringName");
                if (string.IsNullOrEmpty(connectionStringName))
                    connectionStringName = "ATA_MembershipConnection";
                return _connectionSettings[connectionStringName].ConnectionString; 
            }
        }

        public static bool DoNotAutoPushGroupDataToLyris
        {
            get
            {
                return true;
            }
        }

        public static bool SkipLyrisManager
        {
            get
            {
                string s = GetValueFromConfig("SkipLyrisManager");
                return true;
            }
        }

        
        private static bool logRoleProviderVerboseLoaded = false;
        private static bool logRoleProviderVerbose = false;
        public static bool LogRoleProviderVerbose
        {
            get
            {
                if (!logRoleProviderVerboseLoaded)
                {
                    string s = GetValueFromConfig("LogRoleProviderVerbose");
                    logRoleProviderVerbose = !string.IsNullOrEmpty(s);
                    logRoleProviderVerboseLoaded = true;
                }
                return logRoleProviderVerbose;
            }
        } 
       

        // public static string GroupEditUrl
        //{
        //    get
        //    {
        //        string s = GetValueFromConfig("UserManagerEditGroupUrl");
        //        return (s);
        //    }
        //}  


        #region for Timer job to set the application
        // only used from a SharePoint job to be able to specify web.config values
        //public static void SetSiteContext(SPSite site)
        //{   
        //    if (site == null) 
        //        throw new ArgumentNullException("site"); 
        //    SetWebAppName(site.WebApplication.Name);  
        //}


        public static void SetWebAppName(string webAppName)
        { 
            Configuration webConfiguration = WebConfigurationManager.OpenWebConfiguration("/", webAppName);
            _webSettings = webConfiguration.AppSettings.Settings;
            _connectionSettings = webConfiguration.ConnectionStrings.ConnectionStrings;
        }
        #endregion

        #region Private methods
        private static string GetValueFromConfig(string key)
        {
            string value = null;

            if (_webSettings != null)
                value = _webSettings[key].Value;
            else
                value = _siteSettings[key];

            return value;
        }
        #endregion
    }
}
