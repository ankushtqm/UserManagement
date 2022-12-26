#region Namespaces

using System.Diagnostics;
using Microsoft.VisualBasic;
using System;
using System.Data;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using System.Globalization;
using System.DirectoryServices;
using System.Data.SqlClient;
using System.Security.Principal;

#endregion

namespace SusQtech.ActiveDirectoryServices
{
    public class Utility
    {
        #region Constant Variables

        public readonly static DateTime DefaultDate = DateTime.Parse("12/30/1899");
        public static string ADUser = Configuration.Configuration.Instance.ADUser;
        public static string ADPassword = Configuration.Configuration.Instance.ADPassword;
        public static string ADUsersPath = Configuration.Configuration.Instance.ADUsersPath;
        public static string ADDomainName = Configuration.Configuration.Instance.Domain;
        public static string ADPath = "LDAP://" + ADDomainName;
        public static string WSSUrl = Configuration.Configuration.Instance.WSSUrl;

        #endregion

        #region Private Functions

        /// <summary></summary>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        internal static DirectoryEntry GetDirectoryObject(string UserName, string Password)
        {
            return new DirectoryEntry(ADPath, UserName, Password, AuthenticationTypes.Secure);
        }

        #endregion

        #region Public Enums

        /// <summary></summary>
        public enum LoginResult
        {
            LOGIN_OK = 0,
            LOGIN_USER_DOESNT_EXIST,
            LOGIN_USER_ACCOUNT_INACTIVE
        }

        /// <summary></summary>
        internal enum UserStatus
        {
            Enable = 544,
            Disable = 546
        }

        /// <summary></summary>
        internal enum GroupScope
        {
            ADS_GROUP_TYPE_DOMAIN_LOCAL_GROUP = -2147483644,
            ADS_GROUP_TYPE_GLOBAL_GROUP = -2147483646,
            ADS_GROUP_TYPE_UNIVERSAL_GROUP = -2147483640
        }

        /// <summary></summary>
        internal enum ADAccountOptions
        {
            UF_TEMP_DUPLICATE_ACCOUNT = 256,
            UF_NORMAL_ACCOUNT = 512,
            UF_INTERDOMAIN_TRUST_ACCOUNT = 2048,
            UF_WORKSTATION_TRUST_ACCOUNT = 4096,
            UF_SERVER_TRUST_ACCOUNT = 8192,
            UF_DONT_EXPIRE_PASSWD = 65536,
            UF_SCRIPT = 1,
            UF_ACCOUNTDISABLE = 2,
            UF_HOMEDIR_REQUIRED = 8,
            UF_LOCKOUT = 16,
            UF_PASSWD_NOTREQD = 32,
            UF_PASSWD_CANT_CHANGE = 64,
            UF_ACCOUNT_LOCKOUT = 16,
            UF_ENCRYPTED_TEXT_PASSWORD_ALLOWED = 128
        }

        #endregion

        #region Functions

        /// <summary></summary>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        internal static DirectoryEntry GetUser(string UserName, string Password)
        {
            DirectoryEntry de = GetDirectoryObject(UserName, Password);
            DirectorySearcher deSearch = new DirectorySearcher();
            deSearch.SearchRoot = de;
            deSearch.Filter = "(&(objectClass=user)(sAMAccountName=" + UserName + "))";
            deSearch.SearchScope = SearchScope.Subtree;
            SearchResult results = deSearch.FindOne();

            if (!(results == null))
                return new DirectoryEntry(results.Path, ADUser, ADPassword, AuthenticationTypes.Secure);
            else
                return null;
        }

        /// <summary></summary>
        /// <param name="oDE"></param>
        internal static void EnableUserAccount(DirectoryEntry oDE)
        {
            oDE.Properties["userAccountControl"].Value = UserStatus.Enable;
            oDE.CommitChanges();
            oDE.Close();
        }

        /// <summary></summary>
        /// <param name="oDE"></param>
        internal static void DisableUserAccount(DirectoryEntry oDE)
        {
            oDE.Properties["userAccountControl"].Value = UserStatus.Disable;
            oDE.CommitChanges();
            oDE.Close();
        }

        /// <summary></summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        internal static DirectoryEntry GetGroup(string Name)
        {
            DirectoryEntry de = Utility.GetDirectoryObject();
            DirectorySearcher deSearch = new DirectorySearcher();
            deSearch.SearchRoot = de;
            deSearch.Filter = "(&(objectClass=group) (cn=" + Name + "))";
            deSearch.SearchScope = SearchScope.Subtree;
            SearchResult results = deSearch.FindOne();

            if (!(results == null))
                return new DirectoryEntry(results.Path, ADUser, ADPassword, AuthenticationTypes.Secure);
            else
                return null;
        }

        /// <summary></summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        internal static DirectoryEntry WorkingGetGroup(string Name)
        {
            string fullpath = string.Format("LDAP://{0}/{1}{2}", Utility.ADDomainName, Utility.ADUsersPath, Utility.GetLDAPDomain());
            DirectoryEntry de = new DirectoryEntry(fullpath, ADUser, ADPassword, AuthenticationTypes.Secure);
            DirectorySearcher deSearch = new DirectorySearcher();
            deSearch.SearchRoot = de;
            deSearch.Filter = "(&(objectClass=group) (cn=" + Name + "))";
            deSearch.SearchScope = SearchScope.Subtree;
            SearchResult results = deSearch.FindOne();

            if (!(results == null))
                return new DirectoryEntry(results.Path, ADUser, ADPassword, AuthenticationTypes.Secure);
            else
                return null;
        }

        /// <summary></summary>
        /// <param name="DomainReference"></param>
        /// <returns></returns>
        internal static DirectoryEntry GetDirectoryObject(string DomainReference)
        {
            return new DirectoryEntry(Utility.ADPath + DomainReference, Utility.ADUser, Utility.ADPassword, AuthenticationTypes.Secure);
        }

        /// <summary></summary>
        /// <param name="oDE"></param>
        /// <param name="Password"></param>
        internal static void SetUserPassword(DirectoryEntry oDE, string Password)
        {
            oDE.Invoke("SetPassword", new object[] { Password });
        }

        /// <summary></summary>
        /// <param name="userAccountControl"></param>
        /// <returns></returns>
        internal static bool IsAccountActive(int userAccountControl)
        {
            int userAccountControl_Disabled = Convert.ToInt32(ADAccountOptions.UF_ACCOUNTDISABLE);
            int flagExists = userAccountControl & userAccountControl_Disabled;

            if (flagExists > 0)
                return false;
            else
                return true;
        }

        /// <summary></summary>
        /// <returns></returns>
        public static string GetLDAPDomain()
        {
            StringBuilder LDAPDomain = new StringBuilder();
            string serverName = Utility.ADDomainName;
            string[] LDAPDC = serverName.Split(System.Convert.ToChar("."));
            int i = 0;

            while (i < LDAPDC.GetUpperBound(0) + 1)
            {
                LDAPDomain.Append("DC=" + LDAPDC[i].ToLower());
                if (i < LDAPDC.GetUpperBound(0))
                    LDAPDomain.Append(",");

                i += 1;
            }

            return LDAPDomain.ToString();
        }

        /// <summary></summary>
        /// <param name="ObjectPath"></param>
        /// <returns></returns>
        internal static DirectoryEntry GetDirectoryObjectByDistinguishedName(string ObjectPath)
        {
            return new DirectoryEntry(ObjectPath, Utility.ADUser, Utility.ADPassword, AuthenticationTypes.Secure);
        }

        /// <summary></summary>
        /// <param name="oDE"></param>
        /// <param name="PropertyName"></param>
        /// <param name="PropertyValue"></param>
        internal static void SetProperty(DirectoryEntry oDE, string PropertyName, string PropertyValue)
        {
            if ((PropertyValue != string.Empty) && (PropertyValue != null))
            {
                if (oDE.Properties.Contains(PropertyName))
                    oDE.Properties[PropertyName][0] = PropertyValue;
                else
                    oDE.Properties[PropertyName].Value = PropertyValue;
            }
        }

        /// <summary></summary>
        /// <returns></returns>
        internal static DirectoryEntry GetDirectoryObject()
        {

            return new DirectoryEntry(Utility.ADPath, Utility.ADUser, Utility.ADPassword, AuthenticationTypes.Secure);
        }

        /// <summary></summary>
        /// <param name="oDE"></param>
        /// <param name="PropertyName"></param>
        /// <returns></returns>
        internal static string GetProperty(DirectoryEntry oDE, string PropertyName)
        {
            if (oDE.Properties.Contains(PropertyName))
            {
                return oDE.Properties[PropertyName][0].ToString();
            }
            else
                return string.Empty;
        }

        #endregion
    }
}