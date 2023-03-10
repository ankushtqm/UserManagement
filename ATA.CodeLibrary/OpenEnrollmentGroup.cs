#region namespaces

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using SusQTech.Data.DataObjects;

#endregion

namespace ATA.ObjectModel
{
    public class OpenEnrollmentGroup : DataObjectBase
    {
        #region private members

        private int _groupId;
        private string _groupName;
        private string _lyrisShortDescription;
        private string _lyrisListName;
        private string _lyrisTopic;
        private int _jobTitleId;
        private int _preferredEmailUserId;
        private bool _isPreferredEmailPrimary;
        private int _appliesToSiteId;
        private bool _isSelected;

        #endregion

        public OpenEnrollmentGroup()
        {
        }

        #region public properties

        [DataObjectProperty("GroupId", SqlDbType.Int)]
        public int GroupId
        {
            get { return (this._groupId); }
            set { this._groupId = value; }
        }

        [DataObjectProperty("GroupName", SqlDbType.NVarChar, 50)]
        public string GroupName
        {
            get { return (this._groupName); }
            set { this._groupName = value; }
        }

        [DataObjectProperty("LyrisListName", SqlDbType.NVarChar, 60)]
        public string LyrisListName
        {
            get { return (this._lyrisListName); }
            set { this._lyrisListName = value; }
        }

        //[DataObjectProperty("LyrisTopic", SqlDbType.NVarChar, 50)]
        //public string LyrisTopic
        //{
        //    get { return (this._lyrisTopic); }
        //    set { this._lyrisTopic = value; }
        //}

        [DataObjectProperty("LyrisShortDescription", SqlDbType.NVarChar, 200)]
        public string LyrisShortDescription
        {
            get { return (this._lyrisShortDescription); }
            set { this._lyrisShortDescription = value; }
        } 

        [DataObjectProperty("PreferredEmailUserId", SqlDbType.Int)]
        public int PreferredEmailUserId
        {
            get { return (this._preferredEmailUserId); }
            set { this._preferredEmailUserId = value; }
        }

        [DataObjectProperty("IsPreferredEmailPrimary", SqlDbType.Bit)]
        public bool IsPreferredEmailPrimary
        {
            get { return (this._isPreferredEmailPrimary); }
            set { this._isPreferredEmailPrimary = value; }
        }

        [DataObjectProperty("AppliesToSiteId", SqlDbType.Int)]
        public int AppliesToSiteId
        {
            get { return (this._appliesToSiteId); }
            set { this._appliesToSiteId = value; }
        }

        [DataObjectProperty("IsSelected", SqlDbType.Bit)]
        public bool IsSelected
        {
            get { return (this._isSelected); }
            set { this._isSelected = value; }
        }

        #endregion
    }
}
