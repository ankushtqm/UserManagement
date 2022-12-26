#region namespaces

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using SusQTech.Data.DataObjects;

#endregion

namespace ATA.ObjectModel
{
    [DataObject("p_LookupUser_Load")]
    public class LookupUser : DataObjectBase
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
        private int _divisionId;
        private int _departmentId;
        private string _jobTitle;

        #endregion

        public LookupUser()
        {
            this.UserId = DataObjectBase.NullIntRowId;
            this.Username = string.Empty;
            this.Email = string.Empty;
        }

        public LookupUser(int UserId)
        {
            this.Load(UserId);
        }

        #region public properties

        [DataObjectProperty("UserId", SqlDbType.Int, true)]
        public int UserId
        {
            get { return (this._userId); }
            set { this._userId = value; }
        }

        [DataObjectProperty("Username", SqlDbType.NVarChar, 256)]
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

        [DataObjectProperty("DivisionId", SqlDbType.Int)]
        public int DivisionId
        {
            get { return (this._divisionId); }
            set { this._divisionId = value; }
        }

        [DataObjectProperty("DepartmentId", SqlDbType.Int)]
        public int DepartmentId
        {
            get { return (this._departmentId); }
            set { this._departmentId = value; }
        }

        [DataObjectProperty("JobTitle", SqlDbType.NVarChar)]
        public string JobTitle
        {
            get { return (this._jobTitle); }
            set { this._jobTitle = value; }
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

        #region IComparable
        public int CompareTo(object obj)
        {
            LookupUser otherUser = obj as LookupUser;

            string thisName = string.IsNullOrEmpty(this.Username) ? string.Empty : this.Username.ToLower();
            string otherName = string.IsNullOrEmpty(otherUser.Username) ? string.Empty : otherUser.Username.ToLower();

            if (thisName.CompareTo(otherName) < 0)
            {
                return -1;
            }
            else if (thisName.CompareTo(otherName) > 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        #endregion
    }
}
