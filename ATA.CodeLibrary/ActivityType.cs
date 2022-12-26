#region namespaces

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using SusQTech.Data.DataObjects; 
using System.Web.Security;
//#BYNA 
//using ATA.Authentication;
//using ATA.Authentication.Providers;
using ATA.Member.Util;
using Microsoft.ApplicationBlocks.Data;

#endregion

namespace ATA.ObjectModel
{
    [DataObject("p_ActivityType_Create", "p_ActivityType_Update", "p_ActivityType_Load", "p_ActivityType_Delete")]
    public class ActivityType : DataObjectBase
    {
        #region private members

        private int _ActivityTypeId; 
        private string _ActivityTypeDescription;
        private int _CreateUserId; 
        private DateTime _CreateDate;
        private int _ModifiedUserId;
        private DateTime _ModifiedDate;
          
      
        #endregion
 
        public ActivityType()
        {
            this._ActivityTypeId = DataObjectBase.NullIntRowId; 
        }

        public ActivityType(int ActivityTypeId)
        {
            this.Load(ActivityTypeId);
        }

       
         

        #region public properties

        [DataObjectProperty("ActivityTypeId", SqlDbType.Int, true)]
        public int ActivityTypeId
        {
            get { return (this._ActivityTypeId); }
            set { this._ActivityTypeId = value; }
        }

      
        [DataObjectProperty("CreateUserId", SqlDbType.Int)]
        public int CreateUserId
        {
            get { return this._CreateUserId; }
            set { this._CreateUserId = value; }
        }


        [DataObjectProperty("ModifiedUserId", SqlDbType.Int)]
        public int ModifiedUserId
        {
            get { return this._ModifiedUserId; }
            set { this._ModifiedUserId = value; }
            
        }

        [DataObjectProperty("ActivityTypeDescription", SqlDbType.NVarChar, 300)]
        public string ActivityTypeDescription
        {
            get { return (this._ActivityTypeDescription); }
            set { this._ActivityTypeDescription = value; }
        }

         
        [DataObjectProperty("CreateDate", SqlDbType.DateTime)]
        public DateTime CreateDate
        {
            get { return (this._CreateDate); }
            set { this._CreateDate = value; }
        }


        [DataObjectProperty("ModifiedDate", SqlDbType.DateTime)]
        public DateTime ModifiedDate
        {
            get { return (this._ModifiedDate); }
            set { this._ModifiedDate = value; }
        }
        #endregion 
 
         

        #region Activity Type
       // public static Dictionary<int, string> GetAllActivityType()//GetAllMeetingTypes()
       // {
       //     return DbUtil.GetIdStringDictionary("SELECT [ActivityTypeId],[ActivityTypeDescription],[SponsorGroupId],[CreateUserId],[CreateDate] FROM  ActivityType ORDER BY ActivityTypeDescription", false, null);
       // }


        //public void SaveNew()
        //{
        //    SqlParameter[] sqlParams = new SqlParameter[3];
        //    sqlParams[0] = new SqlParameter("@CreateUserId", CreateUserId);
        //    sqlParams[1] = new SqlParameter("@ActivityTypeDescription", ActivityTypeDescription);
        //    sqlParams[2] = new SqlParameter("@CreateDate", CreateDate);

        //    SqlHelper.ExecuteNonQuery(ConnectionFactory.GetConnection(), CommandType.StoredProcedure, "p_ActivityType_Create", sqlParams);
        //}

        //public static bool Update(int AcitivtyTypeId, String ActivityTypeDescription,bool Active, int ModifiedUserId)
        //{
        //    bool edit = true; 

        //    try
        //    {
        //        SqlParameter[] sqlParams = new SqlParameter[3];
        //        sqlParams[0] = new SqlParameter("@ActivityTypeId", AcitivtyTypeId);
        //        sqlParams[1] = new SqlParameter("@ActivityTypeDescription", ActivityTypeDescription);
        //        sqlParams[2] = new SqlParameter("@Active", Active);
        //        sqlParams[3] = new SqlParameter("@ModifiedUserId", ModifiedUserId);


        //        SqlHelper.ExecuteNonQuery(ConnectionFactory.GetConnection(), CommandType.StoredProcedure, "p_ActivityType_Update", sqlParams);
        //    }
        //    catch (Exception ex)
        //    {
        //        edit = false;
        //    }

        //    return edit;
        //}


        //public void Delete(int AcitivtyTypeId)
        //{
        //    SqlParameter[] sqlParams = new SqlParameter[1];
        //    sqlParams[0] = new SqlParameter("@ActivityTypeId", AcitivtyTypeId); 
        //    SqlHelper.ExecuteNonQuery(ConnectionFactory.GetConnection(), CommandType.StoredProcedure, "p_ActivityType_Delete", sqlParams);
        //}

    

        //public DataTable GetActivityTypeAll()
        //{
        //    SqlParameter[] sqlParams = new SqlParameter[0];


        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionFactory.DataObjectConnectionString, CommandType.StoredProcedure, "p_ActivityType_GetAll", sqlParams);

        //    if (ds.Tables.Count > 0)
        //        return ds.Tables[0];
        //    else
        //        return null;
        //}


    

        #endregion 

    } 
}