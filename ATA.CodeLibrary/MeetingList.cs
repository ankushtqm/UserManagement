//#region namespaces

//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Data;
//using System.Data.SqlClient;
//using System.Text;
//using SusQTech.Data.DataObjects; 
//using System.Web.Security;

//using ATA.Authentication;
//using ATA.Authentication.Providers;
//using ATA.Member.Util;
//using Microsoft.ApplicationBlocks.Data;

//#endregion

//namespace ATA.ObjectModel
////{
//    [DataObject("p_MeetingList_Create", "p_MeetingList_Update", "p_MeetingList_Load", "p_MeetingList_Delete")]
//    public class MeetingList : DataObjectBase
//    {
//        #region private members

//        private int _MeetingListId;
//        private int _GroupId;
//        private int _MeetingGroupId;
//        private int _MeetingTypeId;
//        private string _MeetingName;  
//        private string _MeetingLocation;
//        private string _MeetingPurpose;
//        private DateTime _StartDate;
//        private DateTime _EndDate;
//        private DateTime _Created;
//        private int _Createdby;
//        private int _Modifiedby;
//        private DateTime _Modified;
      
//        #endregion
 
//        public MeetingList()
//        {
//            this._MeetingListId = DataObjectBase.NullIntRowId;
            
//        }

//        public MeetingList(int MeetingListId)
//        {
//            this.Load(MeetingListId);
//        }

    

//        #region public properties

//        [DataObjectProperty("MeetingListId", SqlDbType.Int, true)]
//        public int MeetingListId
//        {
//            get { return (this._MeetingListId); }
//            set { this._MeetingListId = value; }
//        }

//        [DataObjectProperty("GroupId", SqlDbType.Int)]
//        public int GroupId
//        {
//            get { return (this._GroupId); }
//            set { this._GroupId = value; }
//        }

        
//        [DataObjectProperty("MeetingTypeId", SqlDbType.Int)]
//        public int MeetingTypeId
//        {
//            get { return this._MeetingTypeId; }
//            set { this._MeetingTypeId = value; }
//        }

//        [DataObjectProperty("MeetingName", SqlDbType.NVarChar, 400)]
//        public string MeetingName
//        {
//            get { return (this._MeetingName); }
//            set { this._MeetingName = value; }
//        }  
 

//        [DataObjectProperty("MeetingLocation", SqlDbType.NVarChar, 500)]
//        public string MeetingLocation
//        {
//            get { return (this._MeetingLocation); }
//            set { this._MeetingLocation = value; }
//        }


//        [DataObjectProperty("StartDate", SqlDbType.DateTime)]
//        public DateTime StartDate
//        {
//            get { return (this._StartDate); }
//            set { this._StartDate = value; }
//        }

//        [DataObjectProperty("EndDate", SqlDbType.DateTime)]
//        public DateTime EndDate
//        {
//            get { return this._EndDate; }
//            set { this._EndDate = value; }
//        }


//        [DataObjectProperty("MeetingPurpose", SqlDbType.NText)]
//        public string MeetingPurpose
//        {
//            get { return (this._MeetingPurpose); }
//            set { this._MeetingPurpose = value; }
//        }

//        [DataObjectProperty("MeetingGroupId", SqlDbType.Int)]
//        public int MeetingGroupId
//        {
//            get { return (this._MeetingGroupId); }
//            set { this._MeetingGroupId = value; }
//        }
        
//        [DataObjectProperty("Createdby", SqlDbType.Int)]
//        public int Createdby
//        {
//            get { return this._Createdby; }
//            set { this._Createdby = value; }
//        }

//        [DataObjectProperty("Modifiedby", SqlDbType.Int)]
//        public int Modifiedby
//        {
//            get { return this._Modifiedby; }
//            set { this._Modifiedby = value; }
//        }

//        [DataObjectProperty("Created", SqlDbType.DateTime)]
//        public DateTime Created
//        {
//            get { return (this._Created); }
//            set { this._Created = value; }
//        }

//        [DataObjectProperty("Modified", SqlDbType.DateTime)]
//        public DateTime Modified
//        {
//            get { return this._Modified; }
//            set { this._Modified = value; }
//        }

//        #endregion 

 

        
//        #region Meeting Users
//        //public List<int> GetAllMeetingUsers()
//        //{
//        //    SqlParameter[] sqlParams = new SqlParameter[1];
//        //    sqlParams[0] = new SqlParameter("@MeetingListId", this.MeetingListId); 
//        //    return DbUtil.GetIdsFromDatabase("SELECT UserId  FROM  MeetingUser where MeetingListId = @MeetingListId", CommandType.Text, sqlParams);
//        //}

//        //public void UpdateMeetingUsers(List<int> userIds, bool deleteOldRecords)
//        //{
//        //    if (deleteOldRecords)
//        //    { 
//        //        SqlParameter[] sqlParams = new SqlParameter[1];
//        //        sqlParams[0] = new SqlParameter("@MeetingListId", this.MeetingListId);
//        //        SqlHelper.ExecuteNonQuery(Conf.ConnectionString, CommandType.Text, "DELETE FROM  MeetingUser where MeetingListId = @MeetingListId", sqlParams);
//        //    }
//        //    string sql = "INSERT INTO [MeetingUser]([MeetingListId], [UserId]) VALUES(@MeetingListId, @UserId)";
//        //    foreach (int userId in userIds)
//        //    { 
//        //        SqlParameter[]  parameters = new SqlParameter[2];
//        //        parameters[0] = new SqlParameter("@MeetingListId", this.MeetingListId);
//        //        parameters[1] = new SqlParameter("@UserId", userId);
//        //        SqlHelper.ExecuteNonQuery(Conf.ConnectionString, CommandType.Text, sql, parameters);
//        //    } 
//        //} 
//        #endregion
         

//        #region Meeting Type
//        public static Dictionary<int, string> GetAllMeetingTypes()
//        {
//            return DbUtil.GetIdStringDictionary("SELECT MeetingTypeId,  MeetingTypeName FROM  MeetingType ORDER BY DisplayOrder", false, null);
//        }
//        #endregion 

//        #region Meeting reports
//        public static Dictionary<int, List<MeetingReportItem>> GetMeetingReportData(int companyId, int groupId, DateTime startDate, DateTime endDate)
//        {
//            string sql = GetMeetingReportSql(companyId, groupId, startDate, endDate);
//            Dictionary<int, List<MeetingReportItem>> reportDetail = new Dictionary<int, List<MeetingReportItem>>();
//            using (SqlDataReader reader = SqlHelper.ExecuteReader(Conf.ConnectionString, CommandType.Text, sql, null))
//            {
//                while (reader.Read())
//                {
//                    int meetGroupId = reader.GetInt32(0);
//                    if (!reportDetail.ContainsKey(meetGroupId))
//                    {
//                        reportDetail.Add(meetGroupId, new List<MeetingReportItem>());
//                    }
//                    reportDetail[meetGroupId].Add(new MeetingReportItem(reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3))); 
//                }
//            }
//            return reportDetail;
            
//        }  

//        private static string GetMeetingReportSql(int companyId, int groupId, DateTime startDate, DateTime endDate)
//        {
//            StringBuilder sb = new StringBuilder();
//            sb.Append(" SELECT MeetingList.GroupId, MeetingList.MeetingListId, MeetingListUser.CompanyId,  MeetingListUser.UserId FROM  MeetingList ");
//            sb.Append(" INNER JOIN  MeetingListUser ON MeetingList.MeetingListId = MeetingListUser.MeetingListId "); 
//            sb = AddFilters(companyId, groupId, startDate, endDate, sb);
//            return sb.ToString();
//        }

//        private static StringBuilder AddFilters(int companyId, int groupId, DateTime startDate, DateTime endDate, StringBuilder sb)
//        {
//            if (companyId > 0 || groupId > 0 || startDate != DateTime.MinValue || endDate != DateTime.MaxValue)
//                sb.Append(" WHERE ");
//            bool isFirst = true;
//            if (companyId > 0)
//            {
//                AppendAnd(ref sb, ref  isFirst);
//                sb.AppendFormat(" MeetingListUser.CompanyId = {0} ", companyId);
//            }
//            if (groupId > 0)
//            {
//                AppendAnd(ref sb, ref  isFirst);
//                sb.AppendFormat(" MeetingList.GroupId = {0} ", groupId);
//            }
//            if (startDate != DateTime.MinValue)
//            {
//                AppendAnd(ref sb, ref  isFirst);
//                sb.AppendFormat(" MeetingList.StartDate >= '{0}'", startDate.ToShortDateString());
//            }
//            if (endDate != DateTime.MaxValue)
//            {
//                AppendAnd(ref sb, ref  isFirst);
//                sb.AppendFormat(" MeetingList.StartDate < '{0}'", endDate.ToShortDateString());
//            }
//            return sb;
//        }

//        private static string GetMeetingCountSql(int groupId, DateTime startDate, DateTime endDate)
//        {
//            StringBuilder sb = new StringBuilder();
//            sb.Append(" SELECT MeetingList.GroupId, count(MeetingList.MeetingListId)  FROM  MeetingList ");
//            sb = AddFilters(-1, groupId, startDate, endDate, sb); 
//            sb.Append(" group by MeetingList.GroupId ");  
//            return sb.ToString();
//        }

//        public static Dictionary<int, int> GetMeetingCount(int groupId, DateTime startDate, DateTime endDate)
//        {
//            string sql = GetMeetingCountSql(groupId, startDate, endDate); 
//            Dictionary<int, int> reportDetail = new Dictionary<int, int>();
//            using (SqlDataReader reader = SqlHelper.ExecuteReader(Conf.ConnectionString, CommandType.Text, sql, null))
//            {
//                while (reader.Read())
//                {
//                    int meetGroupId = reader.GetInt32(0);
//                    reportDetail[meetGroupId] = reader.GetInt32(1); 
//                }
//            }
//            return reportDetail; 
//        }  

//        private static void AppendAnd(ref StringBuilder sb, ref bool isFirst)
//        {
//            if (isFirst)
//                isFirst = false;
//            else
//                sb.Append(" AND ");
//        }
//        #endregion 
//    } 

//    public class MeetingReportItem
//    {
//        public MeetingReportItem(int meetingId, int companyId, int userId)
//        {
//            this.MeetingId = meetingId;
//            this.CompanyId = companyId;
//            this.UserId = userId;
//        }
//        public int MeetingId { get; set; }
//        public int CompanyId { get; set; }
//        public int UserId { get; set; }
//    }
//}