#region namespaces

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using SusQTech.Data.DataObjects; 

#endregion

namespace ATA.ObjectModel
{
    [DataObject()]
    public class SubscriptionGroup : DataObjectBase, IComparable
    {
        #region private members

        private int _groupId;
        private string _groupName;
        private string _lyrisListName;
        private bool _isSubscribed;

        #endregion

        public SubscriptionGroup()
        {
        }

        #region public properties

        [DataObjectProperty("GroupId", SqlDbType.Int, true)]
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

        [DataObjectProperty("IsSubscribed", SqlDbType.Bit)]
        public bool IsSubscribed
        {
            get { return (this._isSubscribed); }
            set { this._isSubscribed = value; }
        }
         

        #endregion

        #region IComparable

        public int CompareTo(object obj)
        {
            try
            {
                if (obj != null && obj is SubscriptionGroup)
                {
                    SubscriptionGroup otherGroup = obj as SubscriptionGroup;

                    string thisName = string.IsNullOrEmpty(this.GroupName) ? string.Empty : this.GroupName.ToLower();
                    string otherName = string.IsNullOrEmpty(otherGroup.GroupName) ? string.Empty : otherGroup.GroupName.ToLower();

                    return thisName.CompareTo(otherName);
                }
                return 0;
            }
            catch { return 0; }
        }

        #endregion
    }
}
