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
    [DataObject("p_MemberType_Create", "p_MemberType_Update", "p_MemberType_Load", "p_MemberType_Delete")]
    public class MemberType : DataObjectBase
    {
        #region private members

        private int _memberTypeId;
        private string _memberTypeName;
        private int _appliesToSiteId;

        private const string LoadByMemberTypeNameProcedureName = "p_MemberType_LoadByName";

        #endregion

        public MemberType()
        {
            this.MemberTypeId = DataObjectBase.NullIntRowId;
            this.Name = string.Empty;
        }

        public MemberType(int MemberTypeId)
        {
            this.Load(MemberTypeId);
        }

        #region public properties

        [DataObjectProperty("MemberTypeId", SqlDbType.Int, true)]
        public int MemberTypeId
        {
            get { return (this._memberTypeId); }
            set { this._memberTypeId = value; }
        }

        [DataObjectProperty("MemberTypeName", SqlDbType.NVarChar, 75)]
        public string Name
        {
            get { return (this._memberTypeName); }
            set { this._memberTypeName = value; }
        }

        [DataObjectProperty("AppliesToSiteId", SqlDbType.Int)]
        public int AppliesToSiteId
        {
            get { return (this._appliesToSiteId); }
            set { this._appliesToSiteId = value; }
        }

        public AppliesToSite AppliesToSite
        {
            get { return ((AppliesToSite)this._appliesToSiteId); }
            set { this._appliesToSiteId = (int)value; }
        }

        #endregion

        #region IsInUse

        public bool IsInUse
        {
            get
            {
                bool isInUse = false;

                SqlCommand cmd = new SqlCommand("p_MemberType_IsInUse");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@MemberTypeId", this.MemberTypeId));
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

            using (SqlCommand cmd = new SqlCommand(MemberType.LoadByMemberTypeNameProcedureName, ConnectionFactory.GetConnection()))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MemberTypeName", MemberTypeName);

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
