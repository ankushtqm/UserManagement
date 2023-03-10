#region namespaces

using System.Diagnostics;
using Microsoft.VisualBasic;
using System;
using System.Data;
using System.Collections;
using System.DirectoryServices;
using System.Data.SqlClient;
using System.Text;
using System.Security.Principal;
using SusQtech.ActiveDirectoryServices;

#endregion

namespace SusQtech.ActiveDirectoryServices
{
    [Serializable()]
    public class ADContact
    {
        #region private members

        private string _firstName;
        private string _lastName;
        private string _email;
        private string _commonName;
        private string _OU;
        private string _displayName;

        #endregion

        #region public properties

        public string FirstName
        {
            get { return (this._firstName); }
            set { this._firstName = value; }
        }

        public string LastName
        {
            get { return (this._lastName); }
            set { this._lastName = value; }
        }

        public string Email
        {
            get { return (this._email); }
            set { this._email = value; }
        }

        public string CommonName
        {
            get { return (this._commonName); }
            set { this._commonName = value; }
        }

        public string OU
        {
            get { return (this._OU); }
            set { this._OU = value; }
        }

        public string DisplayName
        {
            get { return (this._displayName); }
            set { this._displayName = value; }
        }

        #endregion

        /// <summary></summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        private DirectoryEntry GetDirectoryObject(string CommonName)
        {
            DirectoryEntry de = Utility.GetDirectoryObject();
            DirectorySearcher deSearch = new DirectorySearcher();
            deSearch.SearchRoot = de;
            deSearch.Filter = "(&(objectClass=contact)(CN=" + CommonName + "))";
            deSearch.SearchScope = SearchScope.Subtree;
            SearchResult results = deSearch.FindOne();

            if (!(results == null))
                return new DirectoryEntry(results.Path, Utility.ADUser, Utility.ADPassword, AuthenticationTypes.Secure);
            else
                return null;
        }

        public void Update()
        {
            try
            {
                DirectoryEntry de = GetDirectoryObject(this.CommonName);
                Utility.SetProperty(de, "givenName", this.FirstName);
                Utility.SetProperty(de, "sn", this.LastName);
                Utility.SetProperty(de, "mail", this.Email);
                de.CommitChanges();
            }
            catch (Exception ex)
            {
                throw (new Exception("Contact cannot be updated" + ex.Message));
            }
        }
    }
}
