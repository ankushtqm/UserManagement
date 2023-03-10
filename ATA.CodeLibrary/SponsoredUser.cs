#region namespaces

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using SusQTech.Data.DataObjects;

#endregion


namespace ATA.ObjectModel
{
    /// <summary>
    /// Class used when generating a collection of a user's sponsored users
    /// so we don't have to load full ATAMembershipUser objects to fill a grid.
    /// </summary>
    public class SponsoredUser : DataObjectBase
    {
        #region private members

        private int _userId;
        private string _username;
        private string _prefix;
        private string _firstName;
        private string _middleName;
        private string _lastName;
        private string _suffix;
        private string _preferredName;
        private string _email;
        private string _email2;
        private bool _isActiveMB;
        private bool _isActiveFF;

        #endregion

        public SponsoredUser()
        {
            this._userId = DataObjectBase.NullIntRowId;
            this._username = string.Empty;
        }
 
        #region public properties

        [DataObjectProperty("UserId", SqlDbType.Int)]
        public int UserId
        {
            get { return (this._userId); }
            set { this._userId = value; }
        }

        [DataObjectProperty("Username", SqlDbType.NVarChar, 30)]
        public string Username
        {
            get { return (this._username); }
            set { this._username = value; }
        }

        [DataObjectProperty("Prefix", SqlDbType.NVarChar, 5)]
        public string Prefix
        {
            get { return (this._prefix); }
            set { this._prefix = value; }
        }

        [DataObjectProperty("FirstName", SqlDbType.NVarChar, 75)]
        public string FirstName
        {
            get { return (this._firstName); }
            set { this._firstName = value; }
        }

        [DataObjectProperty("MiddleName", SqlDbType.NVarChar, 50)]
        public string MiddleName
        {
            get { return (this._middleName); }
            set { this._middleName = value; }
        }

        [DataObjectProperty("LastName", SqlDbType.NVarChar, 100)]
        public string LastName
        {
            get { return (this._lastName); }
            set { this._lastName = value; }
        }

        [DataObjectProperty("Suffix", SqlDbType.NVarChar, 7)]
        public string Suffix
        {
            get { return (this._suffix); }
            set { this._suffix = value; }
        }

        [DataObjectProperty("PreferredName", SqlDbType.NVarChar, 50)]
        public string PreferredName
        {
            get { return (this._preferredName); }
            set { this._preferredName = value; }
        }

        [DataObjectProperty("Email", SqlDbType.NVarChar, 256)]
        public string Email
        {
            get { return (this._email); }
            set { this._email = value; }
        }

        [DataObjectProperty("Email2", SqlDbType.NVarChar, 256)]
        public string Email2
        {
            get { return (this._email2); }
            set { this._email2 = value; }
        }

        [DataObjectProperty("IsActiveFF", SqlDbType.Bit)]
        public bool IsActiveFF
        {
            get { return (this._isActiveFF); }
            set { this._isActiveFF = value; }
        }

        [DataObjectProperty("IsActiveMB", SqlDbType.Bit)]
        public bool IsActiveMB
        {
            get { return (this._isActiveMB); }
            set { this._isActiveMB = value; }
        }

        public string FormattedDisplayName
        {
            get
            {
                object[] strs = { this._prefix, string.Format("{0} {1}", this._firstName, this._middleName).Trim(), this._lastName, this._suffix };
                return (string.Format("{0} {1} {2} {3}", strs).Trim());
            }
        }

        #endregion
    }
}
