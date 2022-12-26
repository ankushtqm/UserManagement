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
    public class UserCompany : DataObjectBase
    {
        #region private members

        //user company fields
        private int _companyId; 
        private string _responsibilities;

        //child objects
        private Company _company = null; 
        #endregion

        public UserCompany()
        {   
            this._companyId = DataObjectBase.NullIntRowId;
        }

        #region public properties

        [DataObjectProperty("CompanyId", SqlDbType.Int)]
        public int CompanyId
        {
            get { return (this._companyId); }
            set { this._companyId = value; }
        } 

        [DataObjectProperty("Responsibilities", SqlDbType.NVarChar, 1024)]
        public string JobResponsibilities
        {
            get { return (this._responsibilities); }
            set { this._responsibilities = value; }
        } 
       

        public Company Company
        {
            get { return (this._company); }
        }

        #endregion

        #region PostLoad

        public override bool PostLoad()
        {
            bool rValue = base.PostLoad();
            if (rValue)
            {  
                if (this._companyId > 0)
                    this._company = new Company(this._companyId);
            }
            return (rValue);
        }

        #endregion
    }
}
