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
    [DataObject("p_Facility_Create", "p_Facility_Update", "p_Facility_Load", "p_Facility_Delete")]
    public class Facility : DataObjectBase
    {
        #region private members

        private int _facilityId;
        private int _airportId;
        private string _facilityName;
        private Guid _sharepointUniqueId;
        private ManyToManyRelationship<Facility, Company> _companies = null;

        //child objects
        private Airport _airport;

        private const string LoadByFacilityNameProcedureName = "p_Facility_LoadByName";
        private const string LoadBySPListItemUniqueIdProcedureName = "p_Facility_LoadBySPListItemUniqueId";

        #endregion

        public Facility()
        {
            this.FacilityId = DataObjectBase.NullIntRowId;
            this.AirportId = DataObjectBase.NullIntRowId;
            this._airport = null;
            this._sharepointUniqueId = Guid.Empty;
        }

        public Facility(int FacilityId)
        {
            this.Load(FacilityId);
        }

        #region Many-to-Many Facility<->Company methods

        public Company[] GetFacilityCompanies()
        {
            if (this._companies == null)
                this._companies = new ManyToManyRelationship<Facility, Company>("X_CompanyFacility");

            return(this._companies.GetSecondTypeByFirstId(this.FacilityId, false));
        }

        public void AddCompany(int CompanyId)
        {
            if (this._companies == null)
                this._companies = new ManyToManyRelationship<Facility, Company>("X_CompanyFacility");

            this._companies.AddEntry(this.FacilityId, CompanyId);
        }

        public void RemoveCompany(int CompanyId)
        {
            if (this._companies == null)
                this._companies = new ManyToManyRelationship<Facility, Company>("X_CompanyFacility");

            this._companies.DeleteEntry(this.FacilityId, CompanyId);
        }

        public void ClearCompanyAssociations()
        {
            if (this._companies == null)
                this._companies = new ManyToManyRelationship<Facility, Company>("X_CompanyFacility");

            this._companies.ClearAllByFirstTypeId(this.FacilityId);
        }

        #endregion

        #region public properties

        [DataObjectProperty("FacilityId", SqlDbType.Int, true)]
        public int FacilityId
        {
            get { return (this._facilityId); }
            set { this._facilityId = value; }
        }

        [DataObjectProperty("AirportId", SqlDbType.Int)]
        public int AirportId
        {
            get { return (this._airportId); }
            set { this._airportId = value; }
        }

        [DataObjectProperty("FacilityName", SqlDbType.NVarChar, 75)]
        public string FacilityName
        {
            get { return (this._facilityName); }
            set { this._facilityName = value; }
        }

        [DataObjectProperty("SPListItemUniqueId", SqlDbType.UniqueIdentifier)]
        public Guid SPListItemUniqueId
        {
            get { return (this._sharepointUniqueId); }
            set { this._sharepointUniqueId = value; }
        }

        public Airport Airport
        {
            get { return (this._airport); }
        }

        #endregion

        #region Load

        public override bool PostLoad()
        {
            bool rValue = base.PostLoad();

            if (this.AirportId > 0)
                this._airport = new Airport(this.AirportId);

            return (rValue);
        }

        #endregion

        #region Save

        public override bool PostSave()
        {
            bool rValue = base.PostSave();

            if (this.AirportId > 0)
                this._airport = new Airport(this.AirportId);

            return (rValue);
        }

        #endregion

        #region LoadByFacilityName

        public bool LoadByFacilityName(string FacilityName)
        {
            if (string.IsNullOrEmpty(FacilityName))
                return (false);

            bool rValue = true;

            using (SqlCommand cmd = new SqlCommand(Facility.LoadByFacilityNameProcedureName, ConnectionFactory.GetConnection()))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FacilityName", FacilityName);

                try
                {
                    cmd.Connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            this.LoadFromReader(reader);
                        }
                        reader.Close();
                    }
                }
                catch (Exception err)
                {
                    this.SetException(err);
                    rValue = false;
                }
                finally
                {
                    cmd.Connection.Close();
                    cmd.Connection.Dispose();
                }
            }

            return (rValue);
        }

        #endregion

        #region LoadBySPListItemUniqueId

        public bool LoadBySPListItemUniqueId(Guid ListItemUniqueId)
        {
            if (ListItemUniqueId.Equals(Guid.Empty))
                return (false);

            bool rValue = true;

            using (SqlCommand cmd = new SqlCommand(Facility.LoadBySPListItemUniqueIdProcedureName, ConnectionFactory.GetConnection()))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ListItemUniqueId", ListItemUniqueId);

                try
                {
                    cmd.Connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            this.LoadFromReader(reader);
                        }
                        else
                        {
                            rValue = false;
                        }
                        reader.Close();
                    }
                }
                catch (Exception err)
                {
                    this.SetException(err);
                    rValue = false;
                }
                finally
                {
                    cmd.Connection.Close();
                    cmd.Connection.Dispose();
                }
            }

            return (rValue);
        }

        #endregion
    }
}
