#region namespaces

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using SusQTech.Data.DataObjects;
using Microsoft.ApplicationBlocks.Data;
using ATA.Member.Util;

#endregion

namespace ATA.ObjectModel
{
    [DataObject("p_Company_Create", "p_Company_Update", "p_Company_Load", "p_Company_Delete")]
    public class Company : DataObjectBase
    {
        #region private members

        private int _companyId;
        private int _companyTypeId;
        private int _fuelsCompanyTypeId;
        private string _companyName;
        private bool _isAirline;
        private Guid _sharepointUniqueId;
        private ManyToManyRelationship<Company, Facility> _facilities = null;

        private const string LoadByCompanyNameProcedureName = "p_Company_LoadByCompanyName";
        private const string LoadBySPListItemUniqueIdProcedureName = "p_Company_LoadBySPListItemUniqueId";
        private const string GetAllCompaniesSP = "p_Company_GetAll";

        #endregion

        public Company()
        {
            this.CompanyId = DataObjectBase.NullIntRowId;
            _companyTypeId = 5;//default to non-member;
            _fuelsCompanyTypeId = DataObjectBase.NullIntRowId;
            this.Name = string.Empty;
            this.IsAirline = false;
            this._sharepointUniqueId = Guid.Empty;
        }

        public Company(int CompanyId)
        {
            this.Load(CompanyId);
        }

        #region Many-to-Many Company<->Facility methods

        public Facility[] GetCompanyFacilities()
        {
            if (this._facilities == null)
                this._facilities = new ManyToManyRelationship<Company, Facility>("X_CompanyFacility");

            return(this._facilities.GetSecondTypeByFirstId(this.CompanyId, false));
        }

        public void AddFacility(int FacilityId)
        {
            if (this._facilities == null)
                this._facilities = new ManyToManyRelationship<Company, Facility>("X_CompanyFacility");

            this._facilities.AddEntry(this.CompanyId, FacilityId);
        }

        public void RemoveFacility(int FacilityId)
        {
            if (this._facilities == null)
                this._facilities = new ManyToManyRelationship<Company, Facility>("X_CompanyFacility");

            this._facilities.DeleteEntry(this.CompanyId, FacilityId);
        }

        public void ClearAllFacilityAssociations()
        {
            if (this._facilities == null)
                this._facilities = new ManyToManyRelationship<Company, Facility>("X_CompanyFacility");

            this._facilities.ClearAllByFirstTypeId(this.CompanyId);
        }

        #endregion

        #region public properties

        [DataObjectProperty("CompanyId", SqlDbType.Int, true)]
        public int CompanyId
        {
            get { return (this._companyId); }
            set { this._companyId = value; }
        }

        [DataObjectProperty("CompanyTypeId", SqlDbType.Int)]
        public int CompanyTypeId
        {
            get { return (this._companyTypeId); }
            set { this._companyTypeId = value; }
        }


        [DataObjectProperty("FuelsCompanyTypeId", SqlDbType.Int)]
        public int FuelsCompanyTypeId
        {
            get { return (this._fuelsCompanyTypeId); }
            set { this._fuelsCompanyTypeId = value; }
        }  

        [DataObjectProperty("CompanyName", SqlDbType.NVarChar, 75)]
        public string Name
        {
            get { return (this._companyName); }
            set { this._companyName = value; }
        }

        [DataObjectProperty("IsAirline", SqlDbType.Bit)]
        public bool IsAirline
        {
            get { return (this._isAirline); }
            set { this._isAirline = value; }
        }

        [DataObjectProperty("SPListItemUniqueId", SqlDbType.UniqueIdentifier)]
        public Guid SPListItemUniqueId
        {
            get { return (this._sharepointUniqueId); }
            set { this._sharepointUniqueId = value; }
        }

        #endregion

        #region LoadByCompanyName

        public bool LoadByCompanyName(string CompanyName)
        {
            if (string.IsNullOrEmpty(CompanyName))
                return (false);

            bool rValue = true;

            using (SqlCommand cmd = new SqlCommand(Company.LoadByCompanyNameProcedureName, ConnectionFactory.GetConnection()))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CompanyName", CompanyName);

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

        #region LoadBySPListItemUniqueId

        public bool LoadBySPListItemUniqueId(Guid ListItemUniqueId)
        {
            if (ListItemUniqueId.Equals(Guid.Empty))
                return (false);

            bool rValue = true;

            using (SqlCommand cmd = new SqlCommand(Company.LoadBySPListItemUniqueIdProcedureName, ConnectionFactory.GetConnection()))
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


        public static List<Company> GetAllCompanies()
        {
            DataObjectList<Company> companies = new DataObjectList<Company>(null, GetAllCompaniesSP);
            List<Company> comp = new List<Company>();
            string sql = "SELECT * FROM Company ORDER BY CompanyName";
           
            using (DataSet ds = SqlHelper.ExecuteDataset(Conf.ConnectionString, CommandType.Text, sql,null))
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count ;i++  )
                {
                    int companyid = 0;
                    int.TryParse(ds.Tables[0].Rows[i]["CompanyId"].ToString(),out companyid);

                    comp.Add(new Company(companyid));
                }
            }
            return (comp as List<Company>);
        }

        public static List<string> GetCompaniesContainingName(string name)
        {
            List<string> companyNames = new List<string>();
            string sql = "SELECT CompanyName FROM Company WHERE CompanyName LIKE '%{0}%' ORDER BY CompanyName";
            sql = string.Format(sql, name.Replace("'", "''"));
            using (SqlDataReader reader = SqlHelper.ExecuteReader(Conf.ConnectionString, CommandType.Text, sql, null))
            {
                while (reader.Read())
                {
                    companyNames.Add(reader.GetString(0));
                } 
            } 
            return companyNames;
        }

        public static void ChangeUserCompany(int oldComanyId, int newCompanyId)
        {
            //NA - 1/27/17
            //string sql = "UPDATE UserCompany SET CompanyId = {0} WHERE CompanyId = {1}";
            //sql = string.Format(sql, newCompanyId, oldComanyId);
            //SqlHelper.ExecuteNonQuery(Conf.ConnectionString, CommandType.Text, sql);
        }

        public static SortedDictionary<string, int> GetMemberOrNonMemberCompanies(bool isMember)
        {
            try
            {
                SqlParameter[] parameters = new SqlParameter[1];
                parameters[0] = new SqlParameter("@IsA4aMember", isMember);
                return DbUtil.GetSortedStringIdDictionary("p_Company_GetMemberOrNonMemberCompanies", true, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Error in GetMemberOrNonMemberCompanies because {0}", e.ToString()));
                 
            }
        }

        public static SortedDictionary<string, int> GetCompanyList(string company)
        {
            try
            {
                SqlParameter[] parameters = new SqlParameter[1];
                parameters[0] = new SqlParameter("@company", company);
                return DbUtil.GetSortedStringIdDictionary("p_Company_GetList", true, parameters); 
            }
            catch (Exception e)
            { 
                throw new Exception(string.Format("Error in GetCompanyList because {0}", e.ToString()));
            }
        }

        public static int GetFuelsCompanyTypeIdByName(string fuelsCompanyTypeName)
        {
            if (string.IsNullOrEmpty(fuelsCompanyTypeName))
                return DataObjectBase.NullIntRowId;
            string sql = "SELECT FuelsCompanyTypeId FROM FuelsCompanyType where FuelsCompanyTypeName = @FuelsCompanyTypeName"; 
            SqlParameter parameter  = new SqlParameter("@FuelsCompanyTypeName", fuelsCompanyTypeName);
            object result = SqlHelper.ExecuteScalar(Conf.ConnectionString, CommandType.Text, sql, parameter);
            if(result == null)
                return DataObjectBase.NullIntRowId;
            return (int)result;
        }

        public static Dictionary<int, string> GetMemberUsersByCompanyId(int companyId)
        {
            try
            { 
                SqlParameter[] sqlParams = new SqlParameter[1];
                sqlParams[0] = new SqlParameter("@CompanyId", companyId);
                return DbUtil.GetIdStringDictionary("p_Company_GetMemberUsers", true, sqlParams);
            }
            catch (Exception e)
            {
                Log.LogError(string.Format("Error in GetMemberUsersByCompanyId for company id {0} because {1}", companyId, e.ToString()));               
                throw;
            }
        }
    }
}
