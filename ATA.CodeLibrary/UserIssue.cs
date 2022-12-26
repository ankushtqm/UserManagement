#region namespaces

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using SusQTech.Data.DataObjects;

#endregion

namespace ATA.ObjectModel
{
    public class UserIssue : DataObjectBase
    {
        #region private members

        private int _issueId;

        #endregion

        public UserIssue()
        {
            this._issueId = DataObjectBase.NullIntRowId;
        }

        #region public properties

        [DataObjectProperty("IssueId", SqlDbType.Int)]
        public int IssueId
        {
            get { return (this._issueId); }
            set { this._issueId = value; }
        }

        #endregion
    }
}
