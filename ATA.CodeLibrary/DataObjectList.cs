using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace SusQTech.Data.DataObjects
{
    /// <summary>
    /// Summary description for DataStoreObjectCollection
    /// </summary>
    [Serializable]
    public class DataObjectList<T> : List<T> where T : DataObjectBase
    {
        #region private members 
        private Exception _lastException;
        private CommandType _commandType = CommandType.StoredProcedure;

        #endregion

        public DataObjectList()
        {  
        }

        public DataObjectList(SqlParameter[] LoadParameters, string LoadProcedureName)
        {
            this.Load(LoadParameters, LoadProcedureName);
        }

        #region public properties

        public Exception LastException
        {
            get { return (this._lastException); }
        }

        public bool HasException
        {
            get { return ((this._lastException != null)); }
        }

        #endregion

        #region Load

        public bool Load(SqlParameter[] LoadParameters, string LoadProcedureName)
        {
            return (this.LoadData(LoadParameters, LoadProcedureName));
        }

        public bool LoadBySqlText(SqlParameter[] LoadParameters, string sql)
        {
            _commandType = CommandType.Text;
            return (this.LoadData(LoadParameters, sql));
        }


        private bool LoadData(SqlParameter[] loadParameters, string sql)
        {
            bool rValue = false;

            if (string.IsNullOrEmpty(sql))
                return (false);

            SqlCommand cmd = new SqlCommand(sql, ConnectionFactory.GetConnection());
            cmd.Parameters.Clear();
            cmd.CommandType = _commandType;
            if (loadParameters != null)
            {
                foreach (SqlParameter param in loadParameters)
                {
                    cmd.Parameters.Add(param);
                }
            }

            try
            {
                cmd.Connection.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        T dso = System.Activator.CreateInstance<T>();
                        if (dso.PreLoad())
                        {
                            dso.LoadFromReader(reader);
                            if (dso.PostLoad())
                                this.Add(dso);
                        }
                    }
                    reader.Close();
                }
                rValue = true;
            }
            catch (Exception err)
            {
                this._lastException = err;
                rValue = false;
            }
            finally
            {
                cmd.Connection.Close();
                cmd.Connection.Dispose();
                cmd.Dispose();
            }

            return (rValue);
        }

        #endregion

        #region SaveCollection

        public bool SaveCollection()
        {
            try
            {
                using (Enumerator items = this.GetEnumerator())
                {
                    while (items.MoveNext())
                    {
                        items.Current.Save();
                    }
                }
            }
            catch (Exception err)
            {
                this._lastException = err;
                return (false);
            }
            return (true);
        }

        #endregion

        #region DeleteCollection

        public bool DeleteCollection()
        {
            try
            {
                using (Enumerator items = this.GetEnumerator())
                {
                    while (items.MoveNext())
                    {
                        items.Current.Delete();
                    }
                }
            }
            catch (Exception err)
            {
                this._lastException = err;
                return (false);
            }
            return (true);
        }

        #endregion
    }
}