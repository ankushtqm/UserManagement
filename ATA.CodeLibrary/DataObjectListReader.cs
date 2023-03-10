using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace SusQTech.Data.DataObjects
{
    public class DataObjectListReader<T> : ExceptionHoldingObject,IDisposable where T : DataObjectBase
    {
        #region private members

        private T _current = null;
        private SqlConnection _conn = null;
        private SqlDataReader _reader = null;
        private bool _supressPreAndPostLoad = true;

        #endregion

        #region contructor

        public DataObjectListReader(string ReadProcedureName, bool SupressPreAndPostLoad) : this(new SqlParameter[0], ReadProcedureName, SupressPreAndPostLoad) { }

        public DataObjectListReader(SqlParameter[] ReadParameters, string ReadProcedureName, bool SupressPreAndPostLoad)
        {
            Utility.Utility.CheckParameter(ref ReadProcedureName, true, true, true, 1000, "ReadProcedureName");

            this._supressPreAndPostLoad = SupressPreAndPostLoad;

            this._conn = ConnectionFactory.GetConnection();

            SqlCommand cmd = new SqlCommand(ReadProcedureName, this._conn);
            cmd.CommandType = CommandType.StoredProcedure;

            foreach (SqlParameter sqlParam in ReadParameters)
            {
                cmd.Parameters.Add(sqlParam);
            }

            try
            {
                cmd.Connection.Open();
                this._reader = cmd.ExecuteReader();
            }
            catch (Exception err)
            {
                this.SetException(err);
            }
            finally
            {
                cmd.Dispose();
            }
        }

        #endregion

        #region public properties

        public T Current
        {
            get { return (this._current); }
        }

        public bool SupressPreAndPostLoad
        {
            get { return (this._supressPreAndPostLoad); }
            set { this._supressPreAndPostLoad = value; }
        }

        #endregion

        #region Read

        public bool Read()
        {
            if (this._reader == null)
                return (false);

            bool rValue = this._reader.Read();
            if (rValue)
            {
                try
                {
                    this._current = System.Activator.CreateInstance<T>();
                    
                    if (!this.SupressPreAndPostLoad)
                        this._current.PreLoad();
                    
                    this._current.LoadFromReader(this._reader);

                    if (!this.SupressPreAndPostLoad)
                        this._current.PostLoad();
                }
                catch (Exception err)
                {
                    this.SetException(err);
                    rValue = false;
                }
            }

            return (rValue);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            this._current = null;
            if (this._reader != null)
            {
                this._reader.Close();
                this._reader.Dispose();
            }
            if (this._conn != null)
            {
                this._conn.Close();
                this._conn.Dispose();
            }
        }

        #endregion
    }
}
