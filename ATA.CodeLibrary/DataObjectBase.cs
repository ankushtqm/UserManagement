#region namespaces

using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Reflection;

#endregion

namespace SusQTech.Data.DataObjects
{
    /// <summary>
    /// Summary description for DataStoreObject
    /// </summary>
    public abstract class DataObjectBase : ExceptionHoldingObject
    {
        #region public constants

        public const int NullIntRowId = int.MinValue;
        public static readonly DateTime NullDateValue = DateTime.MinValue;

        #endregion

        #region private members

        private string _idFieldPropertyName = string.Empty;
        private string _idDatabaseFieldFieldName = string.Empty;

        #endregion

        #region GetIdPropertyFieldName

        /// <summary>
        /// Method to recover the name of this object's property which holds the Id Field
        /// </summary>
        /// <returns>Property name</returns>
        /// <exception cref="ReflectionTypeLoadException">Thrown if this object does not have a DataStoreObjectAttribute defined.</exception>
        public virtual string GetIdPropertyFieldName()
        {
            if (this._idFieldPropertyName == string.Empty)
                this.LoadIdFieldLookupValues();

            return (this._idFieldPropertyName);
        }

        /// <summary>
        /// Method to recover the name of this object's cooresponding database id field
        /// </summary>
        /// <returns>Database Id field name</returns>
        /// <exception cref="ReflectionTypeLoadException">Thrown if this object does not have a DataStoreObjectAttribute defined.</exception>
        public virtual string GetDatabaseIdFieldName()
        {
            if (this._idDatabaseFieldFieldName == string.Empty)
                this.LoadIdFieldLookupValues();

            return (this._idDatabaseFieldFieldName);
        }

        /// <summary>
        /// Fills both the if field property and database field names
        /// </summary>
        private void LoadIdFieldLookupValues()
        {
            Type objType = this.GetType();
            PropertyInfo[] props = objType.GetProperties();
            foreach (PropertyInfo prop in props)
            {
                DataObjectPropertyAttribute[] propAtts = (DataObjectPropertyAttribute[])prop.GetCustomAttributes(typeof(DataObjectPropertyAttribute), true);
                if (propAtts.Length > 0 && propAtts[0].IsIdField)
                {
                    this._idFieldPropertyName = prop.Name;
                    this._idDatabaseFieldFieldName = propAtts[0].FieldName;
                    break;
                }
            }
        }

        #endregion

        #region IsNew

        /// <summary>
        /// Determines if this object is "new"
        /// </summary>
        public virtual bool IsNew
        {
            get
            {
                string IdField = this.GetIdPropertyFieldName();
                if (IdField == string.Empty)
                    throw new Exception("No id field was found, please specify one using a DataObjectAttribute");

                Type oType = this.GetType();
                PropertyInfo propInfo = oType.GetProperty(IdField);

                if (propInfo.PropertyType == typeof(Guid))
                    return ((Guid)propInfo.GetValue(this, null) == Guid.Empty);
                else if (propInfo.PropertyType == typeof(int))
                    return ((int)propInfo.GetValue(this, null) == DataObjectBase.NullIntRowId);

                throw new Exception("Unknown Id Field Type.  Guid and int are currently supported.");
            }
        }

        #endregion

        #region GetIdValue

        /// <summary>
        /// Gets the value of the id property for this object
        /// </summary>
        /// <returns>Value of the id property for this object</returns>
        public object GetIdValue()
        {
            string IdField = this.GetIdPropertyFieldName();
            if (string.IsNullOrEmpty(IdField))
                throw new CustomAttributeFormatException("No id field was found, please specify one using a DataStoreObjectAttribute");

            return (this[IdField]);
        }

        #endregion

        #region SetIdValue

        /// <summary>
        /// Sets the id property value for this object
        /// </summary>
        /// <param name="IdValue">The new value for the id property</param>
        public void SetIdValue(object IdValue)
        {
            string IdField = this.GetIdPropertyFieldName();
            if (string.IsNullOrEmpty(IdField))
                throw new CustomAttributeFormatException("No id field was found, please specify one using a DataStoreObjectAttribute");

            this[IdField] = IdValue;
        }

        #endregion

        #region GetFieldValueAsString

        /// <summary>
        /// Helper method to enable easier loading of text boxes with values for display
        /// null int and date time values are returned as empty strings
        /// </summary>
        /// <param name="FieldName">Field Name whose value should be returned as a string.</param>
        /// <returns>A friendly string representation of the field's value</returns>
        public string GetFieldValueAsString(string FieldName)
        {
            if (string.IsNullOrEmpty(FieldName))
                throw new CustomAttributeFormatException("No FieldName was supplied, please specify one using a DataStoreObjectAttribute");

            Type oType = this.GetType();
            PropertyInfo propInfo = oType.GetProperty(FieldName);

            if (propInfo == null)
                throw new Exception(string.Format("This object does not appear to have a property named {0}.", FieldName));

            DataObjectPropertyAttribute[] propAtt = (DataObjectPropertyAttribute[])propInfo.GetCustomAttributes(typeof(DataObjectPropertyAttribute), false);

            if (propAtt.Length == 0)
                throw new CustomAttributeFormatException(string.Format("No custom attribute of type DataObjectPropertyAttribute was found for the field {0}", FieldName));


            string rString = string.Empty;
            if (propAtt[0].ParameterType == SqlDbType.DateTime && (DateTime)this[FieldName] == DataObjectBase.NullDateValue)
                return (rString);

            if (propAtt[0].ParameterType == SqlDbType.Int && (int)this[FieldName] == DataObjectBase.NullIntRowId)
                return (rString);


            rString = this[FieldName].ToString();

            return (rString);
        }

        #endregion

        #region SetFieldValueFromString

        /// <summary>
        /// Helper method to enable easier parsing of int and date time values from text boxes
        /// handles all the checking and parsing operations.  Empty string are converted to null
        /// for int and date time
        /// </summary>
        /// <param name="FieldName">Name of the field whose values should be set</param>
        /// <param name="NewValue">New value for the field</param>
        public void SetFieldValueFromString(string FieldName, string NewValue)
        {

            if (string.IsNullOrEmpty(FieldName))
                throw new CustomAttributeFormatException("No FieldName was supplied, please specify one using a DataStoreObjectAttribute");

            Type oType = this.GetType();
            PropertyInfo propInfo = oType.GetProperty(FieldName);

            if (propInfo == null)
                throw new Exception(string.Format("This object does not appear to have a property named {0}.", FieldName));

            DataObjectPropertyAttribute[] propAtt = (DataObjectPropertyAttribute[])propInfo.GetCustomAttributes(typeof(DataObjectPropertyAttribute), false);

            if (propAtt.Length == 0)
                throw new CustomAttributeFormatException(string.Format("No custom attribute of type DataObjectPropertyAttribute was found for the field {0}", FieldName));

            if (propAtt[0].ParameterType == SqlDbType.DateTime)
            {
                DateTime testDateTime;
                if (DateTime.TryParse(NewValue, out testDateTime) && testDateTime != DataObjectBase.NullDateValue)
                {
                    this[FieldName] = testDateTime;
                }
                else
                {
                    this[FieldName] = DataObjectBase.NullDateValue;
                }

            }

            if (propAtt[0].ParameterType == SqlDbType.Int)
            {
                int testInt;
                if (int.TryParse(NewValue, out testInt) && testInt != DataObjectBase.NullIntRowId)
                {
                    this[FieldName] = testInt;
                }
                else
                {
                    this[FieldName] = DataObjectBase.NullIntRowId;
                }
            }

            this[FieldName] = NewValue;
        }

        #endregion

        #region indexer

        /// <summary>
        /// Indexer to object properties
        /// </summary>
        /// <param name="FieldName">Name of the property whose value should be returned or set.</param>
        /// <returns>The value of the specified property</returns>
        public object this[string FieldName]
        {
            get
            {
                if (string.IsNullOrEmpty(FieldName))
                    throw new CustomAttributeFormatException("No FieldName was supplied.");

                Type oType = this.GetType();
                PropertyInfo propInfo = oType.GetProperty(FieldName);

                if (propInfo == null)
                    throw new Exception(string.Format("This object does not appear to have a property named {0}.", FieldName));

                return (propInfo.GetValue(this, null));
            }

            set
            {
                if (string.IsNullOrEmpty(FieldName))
                    throw new CustomAttributeFormatException("No FieldName was supplied.");

                Type oType = this.GetType();
                PropertyInfo propInfo = oType.GetProperty(FieldName);

                if (propInfo == null)
                    throw new Exception(string.Format("This object does not appear to have a property named {0}.", FieldName));

                propInfo.SetValue(this, value, null);
            }
        }

        #endregion

        #region Load

        /// <summary>
        /// Can be used by inheriting classes to perform pre-load actions
        /// </summary>
        /// <returns>True if loading should continue, false otherwise</returns>
        public virtual bool PreLoad()
        {
            return (true);
        }

        /// <summary>
        /// Can be used by inheriting classes to perform and post-load actions
        /// such as loading child object collections
        /// </summary>
        /// <returns>True if successful, false otherwise</returns>
        public virtual bool PostLoad()
        {
            return (true);
        }

        /// <summary>
        /// Loads this object by its database row id
        /// </summary>
        /// <param name="ObjectId"></param>
        /// <returns></returns>
        public bool Load(object ObjectId)
        {
            this.SetIdValue(ObjectId);
            return (this.Load());
        }

        /// <summary>
        /// Performs the actual loading
        /// </summary>
        /// <returns>True if the load was successful, false otherwise</returns>
        protected virtual bool Load()
        {
            bool rValue = this.PreLoad();
            if (!rValue)
                return (rValue);

            Type oType = this.GetType();

            //need object atribute
            DataObjectAttribute[] att = (DataObjectAttribute[])oType.GetCustomAttributes(typeof(DataObjectAttribute), true);
            if (att.Length > 0)
            {
                PropertyInfo idProperty = oType.GetProperty(this.GetIdPropertyFieldName());
                DataObjectPropertyAttribute[] propAtt = (DataObjectPropertyAttribute[])idProperty.GetCustomAttributes(typeof(DataObjectPropertyAttribute), true);
                if (propAtt.Length == 0)
                    throw new CustomAttributeFormatException("Missing DataStoreObjectPropertyAttribute on id property.");


                SqlCommand cmd = new SqlCommand(att[0].LoadProcedureName, ConnectionFactory.GetConnection());

                if (string.IsNullOrEmpty(cmd.CommandText))
                {
                    this.SetException(new Exception("No command text specified for Load."));
                    return (false);
                }

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(propAtt[0].ParameterName, propAtt[0].ParameterType).Value = this.GetIdValue();

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
                    cmd.Dispose();
                }
            }

            if (rValue)
                rValue = this.PostLoad();

            return (rValue);
        }

        #endregion

        #region Save

        /// <summary>
        /// Can be used by inheriting classes to perform pre-save actions
        /// </summary>
        /// <returns>True if saving should continue, false otherwise.</returns>
        public virtual bool PreSave()
        {
            return (true);
        }

        /// <summary>
        /// Can be used by inheriting classes to perform post-save actions such as
        /// saving child collections
        /// </summary>
        /// <returns>True if saving should continue, false otherwise.</returns>
        public virtual bool PostSave()
        {
            return (true);
        }

        /// <summary>
        /// Performs the actual save
        /// </summary>
        /// <returns>True if successful, false otherwise.</returns>
        public virtual bool Save()
        {
            bool rValue = this.PreSave();
            if (!rValue)
                return (rValue);

            Type oType = this.GetType();
            object fieldValue;

            //need object atribute
            DataObjectAttribute[] att = (DataObjectAttribute[])oType.GetCustomAttributes(typeof(DataObjectAttribute), true);
            if (att.Length > 0)
            {
                SqlCommand cmd;
                string idParamName = string.Empty;
                if (this.IsNew)
                    cmd = new SqlCommand(att[0].CreateProcedureName);
                else
                    cmd = new SqlCommand(att[0].UpdateProcedureName);

                if (string.IsNullOrEmpty(cmd.CommandText))
                {
                    this.SetException(new Exception("No command text specified for Save."));
                    return (false);
                }

                cmd.Connection = ConnectionFactory.GetConnection();
                cmd.CommandType = CommandType.StoredProcedure;

                PropertyInfo[] props = oType.GetProperties();
                foreach (PropertyInfo prop in props)
                {
                    //add our parameters for each property that has an associated DataStoreObjectPropertyAttribute
                    DataObjectPropertyAttribute[] propAtt = (DataObjectPropertyAttribute[])prop.GetCustomAttributes(typeof(DataObjectPropertyAttribute), true);
                    if (propAtt.Length > 0)
                    {
                        if (propAtt[0].IsIdField)
                        {
                            SqlParameter param = new SqlParameter(propAtt[0].ParameterName, propAtt[0].ParameterType);
                            idParamName = param.ParameterName;
                            if (this.IsNew)
                                param.Direction = ParameterDirection.Output;
                            else
                                param.Value = prop.GetValue(this, null);
                            cmd.Parameters.Add(param);
                        }
                        else
                        {
                            fieldValue = prop.GetValue(this, null);

                            if ((fieldValue == null) || (propAtt[0].ParameterType == SqlDbType.Int && (int)fieldValue == DataObjectBase.NullIntRowId))
                                fieldValue = DBNull.Value;

                            if (propAtt[0].ParameterType == SqlDbType.DateTime && (DateTime)fieldValue == DataObjectBase.NullDateValue)
                                fieldValue = DBNull.Value;

                            if (propAtt[0].HasLength)
                                cmd.Parameters.Add(propAtt[0].ParameterName, propAtt[0].ParameterType, propAtt[0].Length).Value = fieldValue;
                            else
                                cmd.Parameters.Add(propAtt[0].ParameterName, propAtt[0].ParameterType).Value = fieldValue;
                        }
                    }
                }

                try
                {
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                    rValue = true;
                    if (this.IsNew)
                        this.SetIdValue(cmd.Parameters[idParamName].Value);
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
                    cmd.Dispose();
                }
            }

            if (rValue)
                rValue = this.PostSave();

            return (rValue);
        }

        #endregion

        #region Delete

        /// <summary>
        /// Can be used by inheriting classes to perform pre-delete actions
        /// </summary>
        /// <returns>True if deletion should continue, false otherwise.</returns>
        public virtual bool PreDelete()
        {
            return (true);
        }

        /// <summary>
        /// Can be used by inheriting classes to perform post-delete actions such as
        /// deleting child objects
        /// </summary>
        /// <returns>True if successful, false otherwise.</returns>
        public virtual bool PostDelete()
        {
            return (true);
        }

        /// <summary>
        /// Provides a method to delete an object of this type without having to first load it.  Note that both pre and post delete
        /// methods are called, so if an inheriting classes pre/post depends on the class being fully loaded, they will fail.
        /// </summary>
        /// <param name="ObjectId">The database row id to delete.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool Delete(object ObjectId)
        {
            this.SetIdValue(ObjectId);
            return (this.Delete());
        }

        /// <summary>
        /// Performs the actual delete
        /// </summary>
        /// <returns>True if successful, false otherwise.</returns>
        public virtual bool Delete()
        {
            if (this.IsNew)
                return (true);

            bool rValue = this.PreDelete();

            if (!rValue)
                return (rValue);

            Type oType = this.GetType();

            //need object atribute
            DataObjectAttribute[] att = (DataObjectAttribute[])oType.GetCustomAttributes(typeof(DataObjectAttribute), true);
            if (att.Length > 0)
            {
                PropertyInfo idProperty = oType.GetProperty(this.GetIdPropertyFieldName());
                DataObjectPropertyAttribute[] propAtt = (DataObjectPropertyAttribute[])idProperty.GetCustomAttributes(typeof(DataObjectPropertyAttribute), true);
                if (propAtt.Length == 0)
                    throw new CustomAttributeFormatException("Missing DataStoreObjectPropertyAttribute on id property.");

                SqlCommand cmd = new SqlCommand(att[0].DeleteProcedueName, ConnectionFactory.GetConnection());

                if (string.IsNullOrEmpty(cmd.CommandText))
                {
                    this.SetException(new Exception("No command text specified for Delete."));
                    return (false);
                }

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(propAtt[0].ParameterName, propAtt[0].ParameterType).Value = this.GetIdValue();

                try
                {
                    cmd.Connection.Open();
                    int iResult = cmd.ExecuteNonQuery();
                    rValue = (iResult > 0);
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
                    cmd.Dispose();
                }

                if (rValue)
                {
                    if (idProperty.PropertyType == typeof(Guid))
                        this.SetIdValue(Guid.Empty);
                    else if (idProperty.PropertyType == typeof(int))
                        this.SetIdValue(DataObjectBase.NullIntRowId);
                }
            }

            if (rValue)
                rValue = this.PostDelete();

            return (rValue);
        }

        #endregion

        #region LoadFromReader

        /// <summary>
        /// Loads (this) from an SqlDataReader
        /// </summary>
        /// <param name="reader">SqlDataReader containing the values to load.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool LoadFromReader(SqlDataReader reader)
        {
            Type oType = this.GetType();

            bool rValue = false;

            try
            {
                //get all the props and set by name
                PropertyInfo[] props = oType.GetProperties();
                foreach (PropertyInfo prop in props)
                {
                    DataObjectPropertyAttribute[] propAtt = (DataObjectPropertyAttribute[])prop.GetCustomAttributes(typeof(DataObjectPropertyAttribute), true);
                    if (propAtt.Length > 0)
                    {
                        if (propAtt[0].ParameterType == SqlDbType.Int && reader[propAtt[0].FieldName] == DBNull.Value)
                        {
                            prop.SetValue(this, DataObjectBase.NullIntRowId, null);
                        }
                        else if (propAtt[0].ParameterType == SqlDbType.DateTime && reader[propAtt[0].FieldName] == DBNull.Value)
                        {
                            prop.SetValue(this, DataObjectBase.NullDateValue, null);
                        }
                        else if (propAtt[0].ParameterType == SqlDbType.NVarChar && reader[propAtt[0].FieldName] == DBNull.Value)
                        {
                            prop.SetValue(this, string.Empty, null);
                        }
                        else if (propAtt[0].ParameterType == SqlDbType.Bit && reader[propAtt[0].FieldName] == DBNull.Value)
                        {
                            prop.SetValue(this, false, null);
                        }
                        else
                        { 
                            prop.SetValue(this, reader[propAtt[0].FieldName], null); 
                        }
                    }
                }
                rValue = true;
            }
            catch (Exception err)
            {
                this.SetException(err);
                rValue = false;
            }
            return (rValue);
        }

        #endregion
    }
}
