#region namespaces

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using SusQTech.Data.DataObjects;

#endregion

namespace ATA.ObjectModel
{
    public class EditIssue : DataObjectBase
    {
        #region private members

        private int _issueId;
        private string _title;
        private bool _isSelected;

        #endregion

        public EditIssue() { }

        #region public properties

        [DataObjectProperty("IssueId", SqlDbType.Int)]
        public int IssueId
        {
            get { return (this._issueId); }
            set { this._issueId = value; }
        }

        [DataObjectProperty("Title", SqlDbType.NVarChar)]
        public string Title
        {
            get { return (this._title); }
            set { this._title = value; }
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
