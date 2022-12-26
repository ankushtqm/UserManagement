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
    [DataObject("p_FuelCompanyType_Create", "p_FuelCompanyType_Update", "p_FuelCompanyType_Load", "p_FuelCompanyType_Delete")]
    public class FuelCompanyType : DataObjectBase
    {
        #region private members

        private int _FuelsCompanyTypeId;
        private string _FuelsCompanyTypeName; 

        private const string LoadByMemberTypeNameProcedureName = "p_FuelCompanyType_LoadByName";

        #endregion

        public FuelCompanyType()
        {
            this.FuelsCompanyTypeId = DataObjectBase.NullIntRowId;
            this.FuelsCompanyTypeName = string.Empty;
        }

        public FuelCompanyType(int MemberTypeId)
        {
            this.Load(MemberTypeId);
        }

        #region public properties

        [DataObjectProperty("FuelsCompanyTypeId", SqlDbType.Int, true)]
        public int FuelsCompanyTypeId
        {
            get { return (this._FuelsCompanyTypeId); }
            set { this._FuelsCompanyTypeId = value; }
        }

        [DataObjectProperty("FuelsCompanyTypeName", SqlDbType.NVarChar, 75)]
        public string FuelsCompanyTypeName
        {
            get { return (this._FuelsCompanyTypeName); }
            set { this._FuelsCompanyTypeName = value; }
        }

       

        #endregion

        #region IsInUse

        public bool IsInUse
        {
            get
            {
                bool isInUse = false;

                SqlCommand cmd = new SqlCommand("p_FuelsCompanyType_IsInUse");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@FuelsCompanyTypeId", this.FuelsCompanyTypeId));
                cmd.Parameters.Add(new SqlParameter("@IsInUse", SqlDbType.Bit));
                cmd.Parameters[1].Direction = ParameterDirection.Output;
                cmd.Connection = ConnectionFactory.GetConnection();

                try
                {
                    cmd.Connection.Open();
                    isInUse = (bool)cmd.ExecuteScalar();
                }
                catch (Exception err)
                {
                    this.SetException(err);
                }
                finally
                {
                    cmd.Connection.Close();
                    cmd.Connection.Dispose();
                    cmd.Dispose();
                }

                return (isInUse);
            }
        }

        #endregion

        #region LoadByMemberTypeName

        public bool LoadByMemberTypeName(string MemberTypeName)
        {
            if (string.IsNullOrEmpty(MemberTypeName))
                return (false);

            bool rValue = true;

            using (SqlCommand cmd = new SqlCommand(FuelCompanyType.LoadByMemberTypeNameProcedureName, ConnectionFactory.GetConnection()))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FuelsCompanyTypeName", MemberTypeName);

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
    }
}
