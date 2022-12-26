#region namespaces

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using SusQTech.Utility;

#endregion

namespace SusQTech.Data.DataObjects
{
    public class GenericSqlDictionary<keyType, valueType>
    {
        #region private members

        private Exception _lastException;
        private Dictionary<keyType, valueType> _dictionary;

        #endregion

        public GenericSqlDictionary()
        {
            this._dictionary = null;
        }

        public GenericSqlDictionary(string ProcedureName, SqlParameter[] sqlParams, string KeyFieldName, string ValueFieldName)
        {
            this.LoadDictionary(ProcedureName, sqlParams, KeyFieldName, ValueFieldName);
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

        public Dictionary<keyType, valueType> Dictionary
        {
            get { return (this._dictionary); }
        }

        #endregion

        #region LoadDictionary
        
        public void LoadDictionary(string ProcedureName, SqlParameter[] sqlParams, string KeyFieldName, string ValueFieldName)
        {
            Utility.Utility.CheckParameter(ref ProcedureName, true, true, true, 1024, "ProcedureName");
            Utility.Utility.CheckParameter(ref KeyFieldName, true, true, true, 1024, "KeyFieldName");
            Utility.Utility.CheckParameter(ref ValueFieldName, true, true, true, 1024, "ValueFieldName");

            this._dictionary = new Dictionary<keyType,valueType>();

            SqlCommand cmd = new SqlCommand(ProcedureName, ConnectionFactory.GetConnection());
            cmd.Parameters.Clear();
            cmd.CommandType = CommandType.StoredProcedure;
            foreach (SqlParameter param in sqlParams)
            {
                cmd.Parameters.Add(param);
            }

            try
            {
                cmd.Connection.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        this._dictionary.Add((keyType)reader[KeyFieldName],(valueType)reader[ValueFieldName]);
                    }
                    reader.Close();
                }
            }
            catch (Exception err)
            {
                this._lastException = err;
            }
            finally
            {
                cmd.Connection.Close();
                cmd.Connection.Dispose();
                cmd.Dispose();
            }
        }

        #endregion
    }
}
