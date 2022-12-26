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
//{
//    [DataObject("p_MeetingListUser_Create", "p_MeetingListUser_Update", "p_MeetingListUser_Load", "p_MeetingListUser_Delete")] 
//    public class MeetingListUser : DataObjectBase
//    {
//        public MeetingListUser(int companyId, int userId, bool isMainGroupUser, string displayName, int meetingListId, bool isA4AUser,string email)  
//        {
//            this.IsMainGroupUser = isMainGroupUser;
//            this.CompanyId = companyId;
//            this.UserId = userId;
//            this.DisplayName = displayName;
//            this.MeetingListId = meetingListId;
//            this.IsA4AUser = isA4AUser;
//            this.MeetingListUserId = DataObjectBase.NullIntRowId;
//            this.Email = email; 

//        }

//        public MeetingListUser()
//        {
//            this.MeetingListUserId = DataObjectBase.NullIntRowId; 
//        }

//        [DataObjectProperty("MeetingListUserId", SqlDbType.Int, true)]
//        public int MeetingListUserId { get; set; }

//        [DataObjectProperty("MeetingListId", SqlDbType.Int)]
//        public int MeetingListId { get; set; }

//        [DataObjectProperty("IsMainGroupUser", SqlDbType.Bit)]
//        public bool IsMainGroupUser { get; set; }

//        [DataObjectProperty("CompanyId", SqlDbType.Int)]
//        public int CompanyId { get; set; }

//        [DataObjectProperty("UserId", SqlDbType.Int)]
//        public int UserId { get; set; }

//        [DataObjectProperty("DisplayName", SqlDbType.NVarChar, 400)]
//        public string DisplayName { get; set; }


//        [DataObjectProperty("IsA4AUser", SqlDbType.Bit)]
//        public bool IsA4AUser { get; set; }

//        [DataObjectProperty("Email", SqlDbType.NVarChar, 400)]
//        public string Email { get; set; }


//        public void SaveNew()
//        {
//            SqlParameter[] sqlParams = new SqlParameter[7];
//            sqlParams[0] = new SqlParameter("@DisplayName", DisplayName);
//            sqlParams[1] = new SqlParameter("@UserId", UserId);
//            sqlParams[2] = new SqlParameter("@MeetingListId", MeetingListId);
//            if(CompanyId > 0)
//                sqlParams[3] = new SqlParameter("@CompanyId", CompanyId);
//            else
//                sqlParams[3] = new SqlParameter("@CompanyId", DBNull.Value);
//            sqlParams[4] = new SqlParameter("@IsMainGroupUser", IsMainGroupUser);
//            sqlParams[5] = new SqlParameter("@IsA4AUser", IsA4AUser);
//             sqlParams[6] = new SqlParameter("@Email", Email);
            
//            SqlHelper.ExecuteNonQuery(ConnectionFactory.GetConnection(), CommandType.StoredProcedure, "p_MeetingListUser_Create", sqlParams);
//        } 

//        public static DataObjectList<MeetingListUser> GetMeetingListUsers(int meetingListId)
//        {
//            SqlParameter[] sqlParams = new SqlParameter[1];
//            sqlParams[0] = new SqlParameter("@MeetingListId", meetingListId);
//            return new DataObjectList<MeetingListUser>(sqlParams, "p_MeetingListUser_GetAll");
//        }
//    }


//    [DataObject("p_MeetingListSourceUser_Create", "p_MeetingListSourceUse_Update", "p_MeetingListSourceUse_Load", "p_MeetingListSourceUse_Delete")]
//    public class MeetingListSourceUser : DataObjectBase
//    {
//        public MeetingListSourceUser(int companyId, int userId, int committeeRoleId, string displayName, int meetingListId)
//        {
//            this.CommitteeRoleId = committeeRoleId;
//            this.CompanyId = companyId;
//            this.UserId = userId;
//            this.DisplayName = displayName;
//            this.MeetingListId = meetingListId;
//            this.MeetingListSourceUserId = DataObjectBase.NullIntRowId; 
//        }

//        public MeetingListSourceUser()
//        {
//            this.MeetingListSourceUserId = DataObjectBase.NullIntRowId; 
//        }

//        [DataObjectProperty("MeetingListSourceUserId", SqlDbType.Int, true)]
//        public int MeetingListSourceUserId { get; set; }

//        [DataObjectProperty("MeetingListId", SqlDbType.Int)]
//        public int MeetingListId { get; set; }

//        [DataObjectProperty("CommitteeRoleId", SqlDbType.Int)]
//        public int CommitteeRoleId { get; set; }

//        [DataObjectProperty("CompanyId", SqlDbType.Int)]
//        public int CompanyId { get; set; }

//        [DataObjectProperty("UserId", SqlDbType.Int)]
//        public int UserId { get; set; }

//        [DataObjectProperty("DisplayName", SqlDbType.NVarChar, 400)]
//        public string DisplayName { get; set; }

//        public void SaveNew()
//        {
//            SqlParameter[] sqlParams = new SqlParameter[5];
//            sqlParams[0] = new SqlParameter("@DisplayName", DisplayName);
//            sqlParams[1] = new SqlParameter("@UserId", UserId);
//            sqlParams[2] = new SqlParameter("@MeetingListId", MeetingListId); 
//            sqlParams[3] = new SqlParameter("@CompanyId", CompanyId); 
//            sqlParams[4] = new SqlParameter("@CommitteeRoleId", CommitteeRoleId);  
//            SqlHelper.ExecuteNonQuery(ConnectionFactory.GetConnection(), CommandType.StoredProcedure, "p_MeetingListSourceUser_Create", sqlParams);  
//        }

//        public static DataObjectList<MeetingListSourceUser> GetMeetingListSourceUsers(int meetingListId)
//        {
//            SqlParameter[] sqlParams = new SqlParameter[1];
//            sqlParams[0] = new SqlParameter("@MeetingListId", meetingListId);
//            return new DataObjectList<MeetingListSourceUser>(sqlParams, "p_MeetingListSourceUser_GetAll");
//        }

//    } 
//}
