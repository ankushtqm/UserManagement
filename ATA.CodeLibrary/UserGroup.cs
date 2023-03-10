#region namespaces

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using SusQTech.Data.DataObjects;

#endregion

namespace ATA.ObjectModel
{
    [Serializable]
    [DataObject("p_UserGroup_Create", "p_UserGroup_Update", "p_UserGroup_Load", "p_UserGroup_Delete")]
    public class UserGroup : DataObjectBase
    {
        #region private members

        private int _groupId;
        private int _userId;
        private int _committeeRoleId;
        private ATAGroup _group = null; 

        #endregion

        public UserGroup()
        {
            this._groupId = DataObjectBase.NullIntRowId;
            this._userId = 0;
            this._committeeRoleId = 0;
        }

        public UserGroup(int GroupId)
        {
            this.Load(GroupId);
        }
        #region public properties

        [DataObjectProperty("GroupId", SqlDbType.Int)]
        public int GroupId
        {
            get { return (this._groupId); }
            set { this._groupId = value; }
        }

        [DataObjectProperty("UserId", SqlDbType.Int)]
        public int UserId
        {
            get { return (this._userId); }
            set { this._userId = value; }
        }


        [DataObjectProperty("CommitteeRoleId", SqlDbType.Int)]
        public int CommitteeRoleId
        {
            get { return (this._committeeRoleId); }
            set { this._committeeRoleId = value; }
        }

        public ATAGroup Group
        {
            get { return (this._group); }
        } 
        #endregion

        #region PostLoad
        public override bool PostLoad()
        {
            bool rValue = base.PostLoad();
            if (rValue)
            {
                if (this.GroupId > 0)
                    this._group = new ATAGroup(this.GroupId); 
                 
            }
            return (rValue);
        }
        #endregion
    }
}
