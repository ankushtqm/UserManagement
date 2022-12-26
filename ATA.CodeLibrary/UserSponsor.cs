#region namespaces

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using SusQTech.Data.DataObjects;

#endregion

namespace ATA.ObjectModel
{
    //public class UserSponsor : DataObjectBase
    //{
    //    #region private members

    //    private int _sponsorId;
    //    private bool _isPrimary;
    //    private string _prefix;
    //    private string _firstName;
    //    private string _middleName;
    //    private string _lastName;
    //    private string _suffix;
    //    private string _preferredName;
    //    private string _email;
    //    private bool _isAddedAsSuperAdmin;
    //    private string _sponsorUserName;

    //    #endregion

    //    public UserSponsor()
    //    {
    //        this._isPrimary = false;
    //        this._sponsorId = DataObjectBase.NullIntRowId;
    //        this._email = string.Empty;
    //        this._isAddedAsSuperAdmin = false;
    //    }

    //    #region public properties

    //    [DataObjectProperty("SponsorId", SqlDbType.Int)]
    //    public int SponsorId
    //    {
    //        get { return (this._sponsorId); }
    //        set { this._sponsorId = value; }
    //    }

    //    [DataObjectProperty("Username", SqlDbType.NVarChar, 256)]
    //    public string SponsorUserName
    //    {
    //        get { return (this._sponsorUserName); }
    //        set { this._sponsorUserName = value; }
    //    }

    //    [DataObjectProperty("IsPrimary", SqlDbType.Bit)]
    //    public bool IsPrimary
    //    {
    //        get { return (this._isPrimary); }
    //        set { this._isPrimary = value; }
    //    }

    //    [DataObjectProperty("IsAddedAsSuperAdmin", SqlDbType.Bit)]
    //    public bool IsAddedAsSuperAdmin
    //    {
    //        get { return (this._isAddedAsSuperAdmin); }
    //        set { this._isAddedAsSuperAdmin = value; }
    //    }

    //    [DataObjectProperty("Prefix", SqlDbType.NVarChar, 5)]
    //    public string Prefix
    //    {
    //        get { return (this._prefix); }
    //        set { this._prefix = value; }
    //    }

    //    [DataObjectProperty("FirstName", SqlDbType.NVarChar, 75)]
    //    public string FirstName
    //    {
    //        get { return (this._firstName); }
    //        set { this._firstName = value; }
    //    }

    //    [DataObjectProperty("MiddleName", SqlDbType.NVarChar, 50)]
    //    public string MiddleName
    //    {
    //        get { return (this._middleName); }
    //        set { this._middleName = value; }
    //    }

    //    [DataObjectProperty("LastName", SqlDbType.NVarChar, 100)]
    //    public string LastName
    //    {
    //        get { return (this._lastName); }
    //        set { this._lastName = value; }
    //    }

    //    [DataObjectProperty("Suffix", SqlDbType.NVarChar, 7)]
    //    public string Suffix
    //    {
    //        get { return (this._suffix); }
    //        set { this._suffix = value; }
    //    }

    //    [DataObjectProperty("PreferredName", SqlDbType.NVarChar, 50)]
    //    public string PreferredName
    //    {
    //        get { return (this._preferredName); }
    //        set { this._preferredName = value; }
    //    }

    //    public string FormattedDisplayName
    //    {
    //        get
    //        {
    //            object[] strs = { this._prefix, string.Format("{0} {1}", this._firstName, this._middleName).Trim(), this._lastName, this._suffix };
    //            return (string.Format("{0} {1} {2} {3}", strs).Trim());
    //        }
    //    }

    //    [DataObjectProperty("Email", SqlDbType.NVarChar, 256)]
    //    public string Email
    //    {
    //        get { return (this._email); }
    //        set { this._email = value; }
    //    }

    //    #endregion
    //}
}
