using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using SusQTech.Data.DataObjects; 

namespace ATA.ObjectModel
{
    [DataObject("p_CompanyType_Create", "p_CompanyType_Update", "p_CompanyType_Load", "p_CompanyType_Delete")]
    public class CompanyType : DataObjectBase
    {
        #region private members

        private int _companyTypeId;
        private string _companyTypeName;
        private bool _isA4aMember;

        #endregion

        public CompanyType()
        {
            this.CompanyTypeId = DataObjectBase.NullIntRowId;
            this.CompanyTypeName = string.Empty;
        }

        public CompanyType(int companyTypeId)
        {
            this.Load(companyTypeId);
        }

        #region public properties

        [DataObjectProperty("CompanyTypeId", SqlDbType.Int, true)]
        public int CompanyTypeId
        {
            get { return (this._companyTypeId); }
            set { this._companyTypeId = value; }
        }

        [DataObjectProperty("Relationship", SqlDbType.NVarChar, 30)]
        public string CompanyTypeName
        {
            get { return (this._companyTypeName); }
            set { this._companyTypeName = value; }
        }

        [DataObjectProperty("IsA4aMember", SqlDbType.Bit)]
        public bool IsA4aMember
        {
            get { return (this._isA4aMember); }
            set { this._isA4aMember = value; }
        }

        #endregion

      
    }
}

