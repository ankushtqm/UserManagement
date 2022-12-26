#region namespaces

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

#endregion

namespace SusQTech.Data.DataObjects
{
    public class ManyToManyRelationship<FirstType, SecondType> : ExceptionHoldingObject
        where FirstType : DataObjectBase
        where SecondType : DataObjectBase
    {
        #region private members

        private string _xTableName;
        private string _firstTypeDbIdFieldName = string.Empty;
        private string _secondTypeDbIdFieldName = string.Empty;

        #endregion

        #region contructors

        /// <summary>
        /// Creates a new Many-to-Many relationship object.  This constructor assumes that the column names
        /// in the X table are the same as the ID field names in the data tables.
        /// </summary>
        /// <param name="XTableName">Name of the table which holds the many to many relationships.</param>
        public ManyToManyRelationship(string XTableName)
        {
            this._xTableName = XTableName;
            FirstType firstType = System.Activator.CreateInstance<FirstType>();
            SecondType secondType = System.Activator.CreateInstance<SecondType>();

            this._firstTypeDbIdFieldName = firstType.GetDatabaseIdFieldName();
            this._secondTypeDbIdFieldName = secondType.GetDatabaseIdFieldName();
        }

        /// <summary>
        /// Creates a new Many-to-Many relationship object.  This construcor uses the provided field names are the 
        /// column names in the X table.
        /// </summary>
        /// <param name="XTableName">Name of the table which holds the many to many relationships.</param>
        /// <param name="FirstTypeDbIdFieldName">Name of the column in the X table where the ID of the FirstType is stored.</param>
        /// <param name="SecondTypeDbIdFieldName">Name of the column in the X table where the ID of the SecondType is stored.</param>
        public ManyToManyRelationship(string XTableName, string FirstTypeDbIdFieldName, string SecondTypeDbIdFieldName)
        {
            this._xTableName = XTableName;
            this._firstTypeDbIdFieldName = FirstTypeDbIdFieldName;
            this._secondTypeDbIdFieldName = SecondTypeDbIdFieldName;
        }

        #endregion

        #region GetFirstTypeBySecondId

        public FirstType[] GetFirstTypeBySecondId(object lookupIdValue, bool SupressPreAndPostLoad)
        {
            string query = string.Format("SELECT [{0}] FROM [{1}] WHERE [{2}]=@LookupId", this._firstTypeDbIdFieldName, this._xTableName, this._secondTypeDbIdFieldName);

            List<FirstType> objects = new List<FirstType>();

            using (SqlCommand cmd = new SqlCommand(query, ConnectionFactory.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@LookupId", lookupIdValue);

                try
                {
                    cmd.Connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        FirstType o = System.Activator.CreateInstance<FirstType>();
                       
                        o.ThrowExceptions = true;

                        o.Load(reader[this._firstTypeDbIdFieldName]);

                        o.ThrowExceptions = false;

                        objects.Add(o);
                    }
                }
                catch (Exception err)
                {
                    this.SetException(err);
                }
                finally
                {
                    cmd.Connection.Close();
                    cmd.Connection.Dispose();
                }
            }

            return (objects.ToArray());
        }

        #endregion

        #region GetSecondTypeByFirstId

        public SecondType[] GetSecondTypeByFirstId(object lookupIdValue, bool SupressPreAndPostLoad)
        {
            string query = string.Format("SELECT [{0}] FROM [{1}] WHERE [{2}]=@LookupId", this._secondTypeDbIdFieldName, this._xTableName, this._firstTypeDbIdFieldName);

            List<SecondType> objects = new List<SecondType>();

            using (SqlCommand cmd = new SqlCommand(query, ConnectionFactory.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@LookupId", lookupIdValue);

                try
                {
                    cmd.Connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        SecondType o = System.Activator.CreateInstance<SecondType>();
                        
                        o.ThrowExceptions = true;

                        o.Load(reader[this._secondTypeDbIdFieldName]);

                        o.ThrowExceptions = false;

                        objects.Add(o);
                    }
                }
                catch (Exception err)
                {
                    this.SetException(err);
                }
                finally
                {
                    cmd.Connection.Close();
                    cmd.Connection.Dispose();
                }
            }

            return (objects.ToArray());
        }

        #endregion

        #region AddEntry

        public void AddEntry(object FirstTypeId, object SecondTypeId)
        {
            string query = string.Format("INSERT INTO [{0}] ([{1}], [{2}]) VALUES (@FirstTypeId, @SecondTypeId)", this._xTableName, this._firstTypeDbIdFieldName, this._secondTypeDbIdFieldName);

            using (SqlCommand cmd = new SqlCommand(query, ConnectionFactory.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@FirstTypeId", FirstTypeId);
                cmd.Parameters.AddWithValue("@SecondTypeId", SecondTypeId);

                try
                {
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception err)
                {
                    this.SetException(err);
                }
                finally
                {
                    cmd.Connection.Close();
                    cmd.Connection.Dispose();
                }

            }
        }

        #endregion

        #region DeleteEntry

        public void DeleteEntry(object FirstTypeId, object SecondTypeId)
        {
            string query = string.Format("DELETE FROM [{0}] WHERE [{1}]=@FirstTypeId AND [{2}]=@SecondTypeId", this._xTableName, this._firstTypeDbIdFieldName, this._secondTypeDbIdFieldName);

            using (SqlCommand cmd = new SqlCommand(query, ConnectionFactory.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@FirstTypeId", FirstTypeId);
                cmd.Parameters.AddWithValue("@SecondTypeId", SecondTypeId);

                try
                {
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception err)
                {
                    this.SetException(err);
                }
                finally
                {
                    cmd.Connection.Close();
                    cmd.Connection.Dispose();
                }

            }
        }

        #endregion

        #region ClearAllByFirstTypeId

        public void ClearAllByFirstTypeId(object FirstTypeId)
        {
            string query = string.Format("DELETE FROM [{0}] WHERE [{1}]=@DeleteId", this._xTableName, this._firstTypeDbIdFieldName);

            using (SqlCommand cmd = new SqlCommand(query, ConnectionFactory.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@DeleteId", FirstTypeId);
                try
                {
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception err)
                {
                    this.SetException(err);
                }
                finally
                {
                    cmd.Connection.Close();
                    cmd.Connection.Dispose();
                }
            }
        }

        #endregion

        #region ClearAllBySecondTypeId

        public void ClearAllBySecondTypeId(object SecondTypeId)
        {
            string query = string.Format("DELETE FROM [{0}] WHERE [{1}]=@DeleteId", this._xTableName, this._secondTypeDbIdFieldName);

            using (SqlCommand cmd = new SqlCommand(query, ConnectionFactory.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@DeleteId", SecondTypeId);
                try
                {
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception err)
                {
                    this.SetException(err);
                }
                finally
                {
                    cmd.Connection.Close();
                    cmd.Connection.Dispose();
                }
            }
        }

        #endregion
    }
}
