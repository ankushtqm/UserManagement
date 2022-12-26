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
    [DataObject("p_Airport_Create", "p_Airport_Update", "p_Airport_Load", "p_Airport_Delete")]
    public class Airport : DataObjectBase
    {
        #region private members

        private int _airportId;
        private string _airportCode;
        private Guid _sharepointUniqueId;

        private const string LoadByAirportCodeProcedureName = "p_Airport_LoadByAirportCode";
        private const string LoadBySPListItemUniqueIdProcedureName = "p_Airport_LoadBySPListItemUniqueId";

        #endregion

        public Airport()
        {
            this.AirportId = DataObjectBase.NullIntRowId;
            this.AirportCode = string.Empty;
            this.SPListItemUniqueId = Guid.Empty;
        }

        public Airport(int AirportId)
        {
            this.Load(AirportId);
        }

        #region public properties

        [DataObjectProperty("AirportId", SqlDbType.Int, true)]
        public int AirportId
        {
            get { return (this._airportId); }
            set { this._airportId = value; }
        }

        [DataObjectProperty("AirportCode", SqlDbType.NVarChar, 4)]
        public string AirportCode
        {
            get { return (this._airportCode); }
            set { this._airportCode = value; }
        }

        [DataObjectProperty("SPListItemUniqueId", SqlDbType.UniqueIdentifier)]
        public Guid SPListItemUniqueId
        {
            get { return (this._sharepointUniqueId); }
            set { this._sharepointUniqueId = value; }
        }

        #endregion

        #region LoadByAirportCode

        public bool LoadByAirportCode(string AirportCode)
        {
            if (string.IsNullOrEmpty(AirportCode))
                return (false);

            bool rValue = true;

            using (SqlCommand cmd = new SqlCommand(Airport.LoadByAirportCodeProcedureName, ConnectionFactory.GetConnection()))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@AirportCode", AirportCode);

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

            using (SqlCommand cmd = new SqlCommand(Airport.LoadBySPListItemUniqueIdProcedureName, ConnectionFactory.GetConnection()))
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