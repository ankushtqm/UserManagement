using System;
using System.Data;

namespace SusQTech.Data.DataObjects
{
    /// <summary>
    /// Summary description for DataStoreObjectPropertyAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DataObjectPropertyAttribute : Attribute
    {
        #region private members

        private bool _isId;
        private readonly string _fieldName;
        private readonly SqlDbType _sqlDbType;
        private readonly int _length;

        #endregion

        #region contructors

        public DataObjectPropertyAttribute(string FieldName, SqlDbType ParameterType) : this(FieldName, ParameterType, int.MinValue) { }

        public DataObjectPropertyAttribute(string FieldName, SqlDbType ParameterType, int Length)
        {
            this._isId = false;
            this._fieldName = FieldName;
            this._sqlDbType = ParameterType;
            this._length = Length;
        }

        public DataObjectPropertyAttribute(string FieldName, SqlDbType ParameterType, bool IsIdField)
        {
            this._fieldName = FieldName;
            this._sqlDbType = ParameterType;
            this._isId = IsIdField;
            this._length = int.MinValue;
        }

        #endregion

        #region properties

        public bool IsIdField
        {
            get { return (this._isId); }
        }

        public bool HasLength
        {
            get { return ((this._sqlDbType == SqlDbType.NVarChar || this._sqlDbType == SqlDbType.VarChar) && (this._length > 0)); }
        }

        public string FieldName
        {
            get { return (this._fieldName); }
        }

        public SqlDbType ParameterType
        {
            get { return (this._sqlDbType); }
        }

        public int Length
        {
            get { return (this._length); }
        }

        public string ParameterName
        {
            get { return (string.Format("@{0}", this.FieldName)); }
        }

        #endregion
    }
}
