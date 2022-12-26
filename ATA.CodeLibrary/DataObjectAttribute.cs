using System;

namespace SusQTech.Data.DataObjects
{
    /// <summary>
    /// Describes the procedure names
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DataObjectAttribute : Attribute
    {
        #region private members

        private readonly string _createProcedureName;
        private readonly string _updateProcedureName;
        private readonly string _loadProcedureName;
        private readonly string _deleteProcedureName;

        #endregion

        #region constructors

        public DataObjectAttribute() : this(string.Empty, string.Empty, string.Empty, string.Empty) { }

        public DataObjectAttribute(string LoadSPName) : this(string.Empty, string.Empty, LoadSPName, string.Empty) { }

        public DataObjectAttribute(string CreateSPName, string UpdateSPName, string LoadSPName, string DeleteSPName)
        {
            this._createProcedureName = CreateSPName;
            this._updateProcedureName = UpdateSPName;
            this._loadProcedureName = LoadSPName;
            this._deleteProcedureName = DeleteSPName;
        }

        #endregion

        #region properties

        public string CreateProcedureName
        {
            get { return (this._createProcedureName); }
        }

        public string UpdateProcedureName
        {
            get { return (this._updateProcedureName); }
        }

        public string LoadProcedureName
        {
            get { return (this._loadProcedureName); }
        }

        public string DeleteProcedueName
        {
            get { return (this._deleteProcedureName); }
        }

        #endregion
    }
}
