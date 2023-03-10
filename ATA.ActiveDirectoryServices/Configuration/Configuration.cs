#region Namespaces

using System;
using System.Configuration;
using System.Xml;

#endregion

namespace SusQtech.ActiveDirectoryServices.Configuration
{
    public sealed class Configuration
    {
        #region Constants

        public const string DEFAULT_DOMAIN = "";
        public const string DEFAULT_ADUSER = "";
        public const string DEFAULT_ADPASSWORD = "";
        public const string DEFAULT_ADUSERSPATH = "";
        public const string DEFAULT_WSSURL = "";

        #endregion

        #region Class Variables

        // the one and only portal configuration instance
        private static Configuration _instance = null;

        public static Configuration Instance
        {
            get
            {
                if (Configuration._instance == null)
                    throw new Exception("With this version of the dll you must call EstablishInstance to create a Configuration object.");

                return (Configuration._instance);
            }
        }

        #endregion

        #region Instance Variables

        private string _domain = DEFAULT_DOMAIN;
        private string _aduser = DEFAULT_ADUSER;
        private string _adpassword = DEFAULT_ADPASSWORD;
        private string _aduserspath = DEFAULT_ADUSERSPATH;
        private string _wssUrl = DEFAULT_WSSURL;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="xWebConfiguration"/> instance.
        /// </summary>
        private Configuration(string Domain, string ADUser, string ADPassword, string ADUsersPath, string WSSUrl)
        {
            this._domain = Domain;
            this._aduser = ADUser;
            this._adpassword = ADPassword;
            this._aduserspath = ADUsersPath;
            this._wssUrl = WSSUrl;
        }

        #endregion

        #region EstablishInstance

        public static void EstablishInstance(string Domain, string ADUser, string ADPassword, string ADUsersPath, string WSSUrl)
        {
            Configuration._instance = new Configuration(Domain, ADUser, ADPassword, ADUsersPath, WSSUrl);
        }

        #endregion

        #region Instance Properties

        public string Domain
        {
            get { return _domain; }
        }

        public string ADUser
        {
            get { return _aduser; }
        }

        public string ADPassword
        {
            get { return _adpassword; }
        }

        public string ADUsersPath
        {
            get { return _aduserspath; }
        }

        public string WSSUrl
        {
            get { return _wssUrl; }
        }

        #endregion
    }
}
