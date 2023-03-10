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
    public class IsSelectedLookupUser : LookupUser
    {
        #region private memebrs

        private bool _isSelected;

        #endregion

        public IsSelectedLookupUser() : base() { }

        public IsSelectedLookupUser(int UserId)
            : base(UserId)
        {
            this.IsSelected = false;
        }

        #region public properties

        [DataObjectProperty("IsSelected", SqlDbType.Bit)]
        public bool IsSelected
        {
            get { return (this._isSelected); }
            set { this._isSelected = value; }
        }

        #endregion
    }
}
