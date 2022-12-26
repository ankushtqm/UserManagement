using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using SusQTech.Data.DataObjects;

namespace ATA.ObjectModel
{
    [DataObject("p_GroupType_Create", "p_GroupType_Update", "p_GroupType_Load", "p_GroupType_Delete")]
    public class GroupType : DataObjectBase
    {
        #region private members

        private int _companyTypeId;
        private string _companyTypeName;
        private bool _isA4aMember;

        #endregion

        public GroupType()
        {
            this.GroupTypeId = DataObjectBase.NullIntRowId;
            this.GroupTypeName = string.Empty;
        }

        public GroupType(int companyTypeId)
        {
            this.Load(companyTypeId);
        }

        #region public properties

        [DataObjectProperty("GroupTypeId", SqlDbType.Int, true)]
        public int GroupTypeId { get; set; }

        [DataObjectProperty("GroupTypeName", SqlDbType.NVarChar, 100)]
        public string GroupTypeName { get; set; }

        [DataObjectProperty("IsCommitteeCategory", SqlDbType.Bit)]
        public Nullable<bool> IsCommitteeCategory { get; set; }

        [DataObjectProperty("IsSecurityCategory", SqlDbType.Bit)]
        public Nullable<bool> IsSecurityCategory { get; set; }

        [DataObjectProperty("IsDistributionCategory", SqlDbType.Bit)]
        public Nullable<bool> IsDistributionCategory { get; set; }


        #endregion

        #region Get Methods

        private const string GetAllNonChildGroupsSP = "p_Group_GetAllNonChildGroups";
        public static List<ATAGroup> GetAllNonChildGroups(AppliesToSite appliesToSite)
        {
            SqlParameter[] sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter("@AppliesToSiteId", appliesToSite);
            DataObjectList<ATAGroup> groups = new DataObjectList<ATAGroup>(sqlParams, GetAllNonChildGroupsSP);
            return (groups as List<ATAGroup>);
        }
        #endregion 
    }
}
