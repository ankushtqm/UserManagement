#region namespaces

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

#endregion

namespace SusQTech.Data.DataObjects
{
    public class FieldMapper
    {
        #region private memebers

        private Dictionary<string, string> _fieldMap;

        #endregion

        public FieldMapper()
        {
            this._fieldMap = new Dictionary<string, string>();
        }

        public void AddMapping(string SourceFieldName, string DestinationFieldName)
        {
            this._fieldMap.Add(SourceFieldName, DestinationFieldName);
        }

        public void RemoveMapping(string SourceFieldName)
        {
            this._fieldMap.Remove(SourceFieldName);
        }

        public List<MappedField>.Enumerator GetEnumerator()
        {
            List<MappedField> list = new List<MappedField>();
            Dictionary<string, string>.Enumerator fields = this._fieldMap.GetEnumerator();
            while (fields.MoveNext())
            {
                list.Add(new MappedField(fields.Current.Key, fields.Current.Value));
            }
            return (list.GetEnumerator());
        }

        #region PerformStringIndexerMapping

        public void PerformStringIndexerMapping(object SourceObject, object DestinationObject)
        {
            Type sourceType = SourceObject.GetType();
            Type destinationType = DestinationObject.GetType();

            PropertyInfo sourceIndexerProperty = this.GetStringIndexerProperty(sourceType);
            PropertyInfo targetIndexerProperty = this.GetStringIndexerProperty(destinationType);

            if (sourceIndexerProperty == null || targetIndexerProperty == null)
            {
                throw new Exception("One of the supplied objects does not have an indexer with a string key.");
            }

            Dictionary<string, string>.Enumerator fields = this._fieldMap.GetEnumerator();
            while (fields.MoveNext())
            {
                object value = sourceIndexerProperty.GetValue(SourceObject, new string[] { fields.Current.Key });
                targetIndexerProperty.SetValue(DestinationObject, value, new string[] { fields.Current.Value });
            }
        }

        #endregion

        #region PerformPropertyNameMapping

        public void PerformPropertyNameMapping(object SourceObject, object DestinationObject)
        {
            Type sourceType = SourceObject.GetType();
            Type destinationType = DestinationObject.GetType();

            PropertyInfo sourceProperty = null;
            PropertyInfo targetProperty = null;

            Dictionary<string, string>.Enumerator fields = this._fieldMap.GetEnumerator();
            while (fields.MoveNext())
            {
                sourceProperty = sourceType.GetProperty(fields.Current.Key);
                targetProperty = destinationType.GetProperty(fields.Current.Value);

                if (sourceProperty != null && targetProperty != null)
                {
                    object value = sourceProperty.GetValue(SourceObject, null);
                    targetProperty.SetValue(DestinationObject, value, null);
                }
            }
        }

        #endregion

        #region GetStringIndexerProperty

        private PropertyInfo GetStringIndexerProperty(Type t)
        {
            PropertyInfo returnPropertyInfo = null;

            foreach (PropertyInfo property in t.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                ParameterInfo[] paramInfo = property.GetIndexParameters();
                if (paramInfo.Length == 1 && paramInfo[0].ParameterType == typeof(string))
                {
                    returnPropertyInfo = property;
                    break;
                }
            }

            return(returnPropertyInfo);
        }

        #endregion

        #region MappedField class

        public class MappedField
        {
            public readonly string SourceFieldName;
            public readonly string DestinationFieldName;

            internal MappedField(string SourceFieldName, string DestinationFieldName)
            {
                this.SourceFieldName = SourceFieldName;
                this.DestinationFieldName = DestinationFieldName;
            }
        }

        #endregion
    }
}
