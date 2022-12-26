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
    public class UserAddress : DataObjectBase
    {
        #region private members

        //user address fields
        private int _addressId; 
        private int _addressTypeId;

        //child objects
        private Address _address = null;
        private AddressType _addressType = null;

        #endregion

        public UserAddress()
        {
            this._addressTypeId = DataObjectBase.NullIntRowId;
            this._addressId = DataObjectBase.NullIntRowId; 
        }

        #region public properties

        [DataObjectProperty("AddressId", SqlDbType.Int)]
        public int AddressId
        {
            get { return (this._addressId); }
            set { this._addressId = value; }
        } 

        [DataObjectProperty("AddressTypeId", SqlDbType.Int)]
        public int AddressTypeId
        {
            get { return (this._addressTypeId); }
            set { this._addressTypeId = value; }
        }

        public Address Address
        {
            get { return (this._address); }
        }

        public AddressType AddressType
        {
            get { return (this._addressType); }
        }

        #endregion

        #region PostLoad

        public override bool PostLoad()
        {
            bool rValue = base.PostLoad();
            if (rValue)
            {
                if (this.AddressId > 0)
                    this._address = new Address(this.AddressId);

                if (this.AddressTypeId > 0)
                    this._addressType = new AddressType(this.AddressTypeId);
            }
            return (rValue);
        }

        #endregion
    }
}
