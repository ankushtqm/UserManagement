#region namespaces

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using SusQTech.Data.DataObjects;

#endregion

namespace ATA.ObjectModel
{
    [DataObject("p_AddressType_Create", "p_AddressType_Update", "p_AddressType_Load", "p_AddressType_Delete")]
    public class AddressType : DataObjectBase
    {
        #region private members

        private int _addressTypeId;
        private string _addressTypeDescription;

        #endregion

        public AddressType()
        {
            this.AddressTypeId = DataObjectBase.NullIntRowId;
            this.Description = string.Empty;
        }

        public AddressType(int AddressTypeId)
        {
            this.Load(AddressTypeId);
        }

        #region public properties

        [DataObjectProperty("AddressTypeId", SqlDbType.Int, true)]
        public int AddressTypeId
        {
            get { return (this._addressTypeId); }
            set { this._addressTypeId = value; }
        }

        [DataObjectProperty("AddressTypeDescription", SqlDbType.NVarChar, 30)]
        public string Description
        {
            get { return (this._addressTypeDescription); }
            set { this._addressTypeDescription = value; }
        }

        #endregion
    }
}
