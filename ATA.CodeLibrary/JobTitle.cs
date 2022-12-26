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
    //[DataObject("p_JobTitle_Create", "p_JobTitle_Update", "p_JobTitle_Load", "p_JobTitle_Delete")]
    //public class JobTitle : DataObjectBase
    //{
    //    #region private members

    //    private int _jobTitleId;
    //    private string _jobTitleName;

    //    #endregion

    //    public JobTitle()
    //    {
    //        this.JobTitleId = DataObjectBase.NullIntRowId;
    //        this.Name = string.Empty;
    //    }

    //    public JobTitle(int JobTitleId)
    //    {
    //        this.Load(JobTitleId);
    //    }

    //    #region public properties

    //    [DataObjectProperty("JobTitleId", SqlDbType.Int, true)]
    //    public int JobTitleId
    //    {
    //        get { return (this._jobTitleId); }
    //        set { this._jobTitleId = value; }
    //    }

    //    [DataObjectProperty("JobTitleName", SqlDbType.NVarChar, 30)]
    //    public string Name
    //    {
    //        get { return (this._jobTitleName); }
    //        set { this._jobTitleName = value; }
    //    }

    //    #endregion

    //    #region IsInUse

    //    public bool IsInUse
    //    {
    //        get
    //        {
    //            bool isInUse = false;

    //            SqlCommand cmd = new SqlCommand("p_JobTitle_IsInUse");
    //            cmd.CommandType = CommandType.StoredProcedure;
    //            cmd.Parameters.Add(new SqlParameter("@JobTitleId", this.JobTitleId));
    //            cmd.Parameters.Add(new SqlParameter("@IsInUse", SqlDbType.Bit));
    //            cmd.Parameters[1].Direction = ParameterDirection.Output;
    //            cmd.Connection = ConnectionFactory.GetConnection();

    //            try
    //            {
    //                cmd.Connection.Open();
    //                isInUse = (bool)cmd.ExecuteScalar();
    //            }
    //            catch (Exception err)
    //            {
    //                this.SetException(err);
    //            }
    //            finally
    //            {
    //                cmd.Connection.Close();
    //                cmd.Connection.Dispose();
    //                cmd.Dispose();
    //            }

    //            return (isInUse);
    //        }
    //    }

    //    #endregion
    //}
}
