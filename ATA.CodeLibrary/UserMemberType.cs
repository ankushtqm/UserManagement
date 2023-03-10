#region namespaces

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using SusQTech.Data.DataObjects;

#endregion

namespace ATA.ObjectModel
{
    public class UserMemberType : DataObjectBase
    {
        #region private members

        private int _memberTypeId;

        private MemberType _memberType;

        #endregion

        public UserMemberType()
        {
            this._memberTypeId = DataObjectBase.NullIntRowId;
        }

        #region public properties

        [DataObjectProperty("MemberTypeId", SqlDbType.Int)]
        public int MemberTypeId
        {
            get { return (this._memberTypeId); }
            set { this._memberTypeId = value; }
        }

        public MemberType MemberType
        {
            get { return (this._memberType); }
        }

        #endregion

        #region PostLoad

        public override bool PostLoad()
        {
            bool rValue = base.PostLoad();
            if (rValue)
            {
                if (this.MemberTypeId > 0)
                    this._memberType = new MemberType(this.MemberTypeId);
            }
            return (rValue);
        }

        #endregion
    }
}
