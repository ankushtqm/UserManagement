#region namespaces

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using SusQTech.Data.DataObjects;

#endregion

namespace ATA.ObjectModel
{
    [DataObject("p_Address_Create", "p_Address_Update", "p_Address_Load", "p_Address_Delete")]
    public class Address : DataObjectBase
    {
        #region private members

        private int _addressId;
        private string _address1;
        private string _address2;
        private string _city;
        private string _state;
        private string _zipCode;
        private string _province;
        private string _country;

        #endregion

        public Address()
        {
            this.AddressId = DataObjectBase.NullIntRowId;
        }

        public Address(int AddressId)
        {
            this.Load(AddressId);
        }

        #region public properties

        [DataObjectProperty("AddressId", SqlDbType.Int, true)]
        public int AddressId
        {
            get { return (this._addressId); }
            set { this._addressId = value; }
        }

        [DataObjectProperty("Address1", SqlDbType.NVarChar, 125)]
        public string Address1
        {
            get { return (this._address1); }
            set { this._address1 = value; }
        }

        [DataObjectProperty("Address2", SqlDbType.NVarChar, 125)]
        public string Address2
        {
            get { return (this._address2); }
            set { this._address2 = value; }
        }

        [DataObjectProperty("City", SqlDbType.NVarChar, 50)]
        public string City
        {
            get { return (this._city); }
            set { this._city = value; }
        }

        [DataObjectProperty("State", SqlDbType.NVarChar, 50)]
        public string State
        {
            get { return (this._state); }
            set { this._state = value; }
        }

        [DataObjectProperty("ZipCode", SqlDbType.NVarChar, 12)]
        public string ZipCode
        {
            get { return (this._zipCode); }
            set { this._zipCode = value; }
        }

        [DataObjectProperty("Province", SqlDbType.NVarChar, 75)]
        public string Province
        {
            get { return (this._province); }
            set { this._province = value; }
        }

        [DataObjectProperty("Country", SqlDbType.NVarChar, 50)]
        public string Country
        {
            get { return (this._country); }
            set { this._country = value; }
        }

        #endregion
    }
}
