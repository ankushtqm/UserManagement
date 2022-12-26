#region namespaces

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
//using System.Globalization;
//using System.Text;
//using System.Web;
using System.Web.Hosting;
//using Microsoft.SharePoint;

#endregion

namespace SusQTech.Utility
{
    public static class Utility
    { 
        #region CheckParameter

        public static void CheckParameter(ref string param, bool checkForNull, bool checkIfEmpty, bool checkForCommas, int maxSize, string paramName)
        {
            if (param == null)
            {
                if (checkForNull)
                {
                    throw new ArgumentNullException(paramName);
                }
            }
            else
            {
                param = param.Trim();
                if (checkIfEmpty && string.IsNullOrEmpty(param))
                {
                    throw new ArgumentException("Parameter_can_not_be_empty", paramName);
                }
                if ((maxSize > 0) && (param.Length > maxSize))
                {
                    throw new ArgumentException("Parameter_too_long", paramName);
                }
                if (checkForCommas && param.Contains(","))
                {
                    throw new ArgumentException("Parameter_can_not_contain_comma", paramName);
                }
            }
        }

        #endregion

        #region CheckPasswordParameter

        public static void CheckPasswordParameter(ref string param, int maxSize, string paramName)
        {
            if (param == null)
            {
                throw new ArgumentNullException(paramName);
            }
            if (param.Length < 1)
            {
                throw new ArgumentException("Parameter_can_not_be_empty", paramName);
            }
            if ((maxSize > 0) && (param.Length > maxSize))
            {
                throw new ArgumentException("Parameter_too_long", paramName);
            }
        }

        #endregion 

        #region GetBooleanValue

        public static bool GetBooleanValue(NameValueCollection config, string valueName, bool defaultValue)
        {
            bool result;
            string text = config[valueName];
            if (text == null)
            {
                return defaultValue;
            }
            if (!bool.TryParse(text, out result))
            {
                throw new ProviderException("Value_must_be_boolean");
            }
            return result;
        }

        #endregion

        #region GetDefaultAppName

        public static string GetDefaultAppName()
        {
            try
            {
                string applicationVirtualPath = HostingEnvironment.ApplicationVirtualPath;
                if (string.IsNullOrEmpty(applicationVirtualPath))
                {
                    applicationVirtualPath = Process.GetCurrentProcess().MainModule.ModuleName;
                    int startIndex = applicationVirtualPath.IndexOf('.');
                    if (startIndex != -1)
                    {
                        applicationVirtualPath = applicationVirtualPath.Remove(startIndex);
                    }
                }
                if (string.IsNullOrEmpty(applicationVirtualPath))
                {
                    return "/";
                }
                return applicationVirtualPath;
            }
            catch
            {
                return "/";
            }
        }

        #endregion

        #region GetIntValue

        public static int GetIntValue(NameValueCollection config, string valueName, int defaultValue, bool zeroAllowed, int maxValueAllowed)
        {
            int result;
            string s = config[valueName];
            if (s == null)
            {
                return defaultValue;
            }
            if (!int.TryParse(s, out result))
            {
                if (zeroAllowed)
                {
                    throw new ProviderException("Value_must_be_non_negative_integer");
                }
                throw new ProviderException("Value_must_be_positive_integer");
            }
            if (zeroAllowed && (result < 0))
            {
                throw new ProviderException("Value_must_be_non_negative_integer");
            }
            if (!zeroAllowed && (result <= 0))
            {
                throw new ProviderException("Value_must_be_positive_integer");
            }
            if ((maxValueAllowed > 0) && (result > maxValueAllowed))
            {
                throw new ProviderException("Value_too_big");
            }
            return result;
        }

        #endregion

        //public static SPList GetList(Guid siteID, string webUrl, string listName)
        //{
        //    SPList list = null;

        //    SPSecurity.RunWithElevatedPrivileges(delegate()
        //    {
        //        SPSite site = null;
        //        try
        //        {
        //            site = new SPSite(siteID);
        //        }
        //        catch { }

        //        if (site != null)
        //        {
        //            SPWeb web = null;
        //            try
        //            {
        //                web = site.AllWebs[webUrl];
        //            }
        //            catch { }
                    
        //            if (web != null)
        //            {
        //                try
        //                {
        //                    list = web.Lists[listName];
        //                }
        //                catch { }

        //                web.Dispose();
        //            }

        //            site.RootWeb.Dispose();
        //            site.Dispose();
        //        }
        //    });

        //    return list;
        //}

        #region ValidateParameter

        public static bool ValidateParameter(ref string param, bool checkForNull, bool checkIfEmpty, bool checkForCommas, int maxSize)
        {
            if (param == null)
            {
                return !checkForNull;
            }
            param = param.Trim();
            return (((!checkIfEmpty || (param.Length >= 1)) && ((maxSize <= 0) || (param.Length <= maxSize))) && (!checkForCommas || !param.Contains(",")));
        }

        #endregion
         
    }
}
