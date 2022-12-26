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
    [DataObject("p_Issue_Create", "p_Issue_Update", "p_Issue_Load", "p_Issue_Delete")]
    public class Issue : DataObjectBase
    {
        #region private members

        private int _issueId;
        private string _title;
        private Guid _sharepointUniqueId;

        private const string LoadBySPListItemUniqueIdProcedureName = "p_Issue_LoadBySPListItemUniqueId";

        #endregion

        public Issue()
        {
            this.IssueId = DataObjectBase.NullIntRowId;
            this.Title = string.Empty;
            this.SPListItemUniqueId = Guid.Empty;
        }

        public Issue(int IssueId)
        {
            this.Load(IssueId);
        }

        #region public properties

        [DataObjectProperty("IssueId", SqlDbType.Int, true)]
        public int IssueId
        {
            get { return (this._issueId); }
            set { this._issueId = value; }
        }

        [DataObjectProperty("Title", SqlDbType.NVarChar, 256)]
        public string Title
        {
            get { return (this._title); }
            set { this._title = value; }
        }

        [DataObjectProperty("SPListItemUniqueId", SqlDbType.UniqueIdentifier)]
        public Guid SPListItemUniqueId
        {
            get { return (this._sharepointUniqueId); }
            set { this._sharepointUniqueId = value; }
        }

        #endregion

        #region LoadBySPListItemUniqueId

        public bool LoadBySPListItemUniqueId(Guid ListItemUniqueId)
        {
            if (ListItemUniqueId.Equals(Guid.Empty))
                return (false);

            bool rValue = true;

            using (SqlCommand cmd = new SqlCommand(Issue.LoadBySPListItemUniqueIdProcedureName, ConnectionFactory.GetConnection()))
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
