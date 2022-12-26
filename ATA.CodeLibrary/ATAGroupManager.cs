using System;
using System.Collections.Generic;
using System.Text;
using SusQTech.Data.DataObjects;
using System.Data; 
using System.Data.SqlClient;  
using ATA.ObjectModel;
using Microsoft.ApplicationBlocks.Data; 
using ATA.LyrisProxy;
//using ATA.Authentication.Providers; 
 
namespace ATA.CodeLibrary
{
    public class ATAGroupManager
    {
        #region Lyris  related 

        public static void PushATAGroupUsersToLyrisList(ATAGroup group, List<string> exemptedEmailsInUpperCase)
        {
            Dictionary<int, bool> admins = GetGroupAdmins(group.GroupId);
            List<int> memberIds = DbUtil.GetGroupSelectedATAMemberIds(group.GroupId);
            memberIds.AddRange(DbUtil.GetGroupSelectedNonATAMemberIds(group.GroupId));
            List<LyrisMember> lyrisMembers = new List<LyrisMember>();
            foreach (int id in memberIds)
            {
                LyrisMember lyrisMember = CreateLyrisMemberByUserId(id, ref lyrisMembers); 
                if (admins.ContainsKey(id))//is admin
                {
                    lyrisMember.IsAdmin = true;
                    lyrisMember.ReceiveAdminEmail = admins[id];
                    admins.Remove(id);
                } 
            }
            foreach (int adminId in admins.Keys)
            {
                LyrisMember lyrisMember = CreateLyrisMemberByUserId(adminId, ref lyrisMembers);
                lyrisMember.IsAdmin = true;
                lyrisMember.ReceiveAdminEmail = admins[adminId];
                lyrisMember.NoMail = true;
            }
            LyrisMember.OverWriteListMembers(lyrisMembers, group.LyrisListName, true, exemptedEmailsInUpperCase);
        }

        private static LyrisMember CreateLyrisMemberByUserId(int userId, ref List<LyrisMember> lyrisMembers)
        {
            LookupUser lookupUser = new LookupUser(userId);
            LyrisMember lyrisMember = new LyrisMember(lookupUser.Email);
            lyrisMember.FullName = lookupUser.FormattedDisplayName;
            lyrisMembers.Add(lyrisMember);
            return lyrisMember;
        }

        public static void PushATAGroupUsersToLyrisList(List<ATAGroup> groups, List<string> exemptedEmailsInUpperCase)
        {
            if(groups != null && groups.Count > 0){
                foreach(ATAGroup group in groups)
                    PushATAGroupUsersToLyrisList(group, exemptedEmailsInUpperCase);
            }
        }

        //public void AddGroupSubscriptionToGroup(int GroupId, int SubscribingGroupId, LyrisManager manager)
        //{ 
        //    ATAGroup group = new ATAGroup(GroupId);
        //    ATAGroup subscribingGroup = new ATAGroup(SubscribingGroupId);  
        //    string ListEmail = LyrisManager.MakeListEmailAddress(subscribingGroup.LyrisListName);
        //    //add the group as a member of the list.
        //    manager.AddMemberToList(ListEmail, subscribingGroup.GroupName, group.LyrisListName);
        //}
        #endregion

        //#region Group subScribing in ATA database
        //public void AddGroupSubscriptionToGroup(int GroupId, int SubscribingGroupId)
        //{
        //    SqlParameter[] sqlParams = new SqlParameter[2];
        //    sqlParams[0] = new SqlParameter("GroupId", GroupId);
        //    sqlParams[1] = new SqlParameter("SubscribingGroupId", SubscribingGroupId);
        //    SqlHelper.ExecuteNonQuery(ConnectionFactory.DataObjectConnectionString, CommandType.StoredProcedure, GroupSubscriptionManager.AddGroupSubscriptionProcedure, sqlParams);

        //    ATAGroup group = new ATAGroup(GroupId);
        //    ATAGroup subscribingGroup = new ATAGroup(SubscribingGroupId);

        //    string ListEmail = LyrisManager.MakeListEmailAddress(subscribingGroup.LyrisListName); 
        //    //add the group as a member of the list.
        //    this.Manager.AddMemberToList(ListEmail, subscribingGroup.GroupName, group.LyrisListName);
        //}
        //#endregion

        //#BYNA public static void DeletePrimaryAlternativeGroupUsersByCompany(int companyId, ATAMembershipProvider provider)
        //{ 
        //    using (SqlDataReader reader = SqlHelper.ExecuteReader(Conf.ConnectionString, "p_UserGroup_GetCommitteePrimaryAltRootGroupByComapny", companyId))
        //    {
        //        while (reader.Read())
        //        {
        //            string email = reader.GetString(0);
        //            int userId = reader.GetInt32(1);
        //            int groupId = reader.GetInt32(2);
        //            string lyrisListName = reader.GetString(3);
        //            ATAGroup parentGroup = new ATAGroup(groupId);
        //            //must delete child first before delete parent
        //            ATAGroup cGroup = parentGroup.GetCommitteeChildGroup();
        //            DeleteGroup(provider, userId, cGroup.GroupId, email, cGroup.LyrisListName); 
        //            DeleteGroup(provider, userId, groupId, email, lyrisListName);  
        //        }
        //    } 
        //#BYNA }
        //#BYNA private static void DeleteGroup(ATAMembershipProvider provider, int userId, int groupId, string email, string lyrisListName)
        //{ 
        //    provider.DeleteUserGroup(userId, groupId);
        //    ATA.LyrisProxy.LyrisMember.DeleteMemberFromList(email, lyrisListName);  
        //#BYNA }

        public static List<GroupUser> GetAllPossibleGroupUsers(int groupId, bool isSecurityGroup)
        {  
            SqlParameter[] sqlParams = new SqlParameter[2];
            sqlParams[0] = new SqlParameter("GroupId", groupId);
            sqlParams[1] = new SqlParameter("IsSecurityGroup", isSecurityGroup);
            return GetGroupUsers(sqlParams, "dbo.p_Group_GetAllPossibleGroupMembers"); 
        } 

        public static List<GroupUser> GetGroupNonAtaUsers(int groupId)
        {
            return GetGroupUsers(groupId, "dbo.p_Group_GetAllNonAtaMembers");
        }

        public static List<GroupUser> GetAllAtaUsers()
        {
            return GetGroupUsers(1, "dbo.p_Group_GetAllAtaMembers");
        }


        public static List<GroupUser> GetAllGroupMembers(int GroupId)
        {
            SqlParameter[] sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter("GroupId", GroupId);

            return GetGroupUsers(sqlParams, "dbo.p_Group_GetAllMembersandCompany");
        }

        public static List<GroupUser> GetAllMySponsoredMemberUsersForCommitteeGroupUI(int userId, bool isSupperAdmin)
        {
            SqlParameter[] sqlParams = new SqlParameter[2];
            sqlParams[0] = new SqlParameter("UserId", userId);
            sqlParams[1] = new SqlParameter("IsSupperAdmin", isSupperAdmin); 
            return GetGroupUsers(sqlParams, "p_Group_GetAllMyUsersForCommitteeGroupUI"); 
        }

        public static List<GroupUser> GetUsersForGroupUIByUserIds(List<int> ids)
        {
            DataObjectList<GroupUser> users = new DataObjectList<GroupUser>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT u.UserId, u.Prefix, u.FirstName, u.MiddleName, u.LastName, u.Suffix, c.CompanyName, c.CompanyTypeId, -1 as CommitteeRoleId , u.Email ");
            sb.Append("FROM [User] u LEFT JOIN UserCompany uc ON uc.UserId = u.UserId LEFT JOIN Company c on uc.CompanyId = c.CompanyId ");
            sb.AppendFormat("WHERE u.UserId" + DbUtil.ConvertIdsToSqlIn(ids));
            users.LoadBySqlText(null, sb.ToString()); 
            List<GroupUser> items = new List<GroupUser>(users.ToArray());
            items.Sort();
            return items;
        }

        public static List<int> GetGroupManageAdmins(int groupId)
        {
            List<int> groupAdmins = new List<int>();
            SqlParameter[] sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter("@GroupId", groupId);

            using (SqlDataReader reader = SqlHelper.ExecuteReader(ConnectionFactory.GetConnection().ConnectionString,CommandType.StoredProcedure,"p_group_getadmins", sqlParams))
            {
                while (reader.Read())
                {
                    groupAdmins.Add(reader.GetInt32(0)); 
                }
            }
            return groupAdmins;
        }

        public static Dictionary<int, bool> GetGroupAdmins(int groupId)
        {
            Dictionary<int, bool> groupAdmins = new Dictionary<int, bool>();

            string query = "select UserId, CAST((select COUNT(*) from[UserGroup] where userid = ug.userid and GroupId = @GroupId and CommitteeRoleId = 10) as bit) " +
                "from  [UserGroup] ug where GroupId = @GroupId and CommitteeRoleId = 9";
            using (SqlCommand cmd = new SqlCommand(query, ConnectionFactory.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@GroupId", groupId); 
                try
                {
                    cmd.Connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int userId = reader.GetInt32(0);
                        bool isBounceAdmin = false;
                        if(!reader.IsDBNull(1))
                            isBounceAdmin = reader.GetBoolean(1);
                        groupAdmins[userId] = isBounceAdmin; 
                    }
                    reader.Close(); 
                } 
                finally
                {
                    cmd.Connection.Close(); 
                }
            } 
            return groupAdmins; 
        }

        public static void UpdateGroupUserCommitteeRole(int UserId, int GroupId, int CommitteeRoleId)
        {
            SqlParameter[] sqlParams = new SqlParameter[3];
            sqlParams[0] = new SqlParameter("@UserId", UserId);
            sqlParams[1] = new SqlParameter("@GroupId", GroupId);
            sqlParams[2] = new SqlParameter("@CommitteeRoleId", CommitteeRoleId); //DataObjectBase.NullIntRowId 
            string updateCommitteeRoleSql = "UPDATE UserGroup SET CommitteeRoleId = @CommitteeRoleId WHERE UserId = @UserId AND GroupId = @GroupId";
            SqlHelper.ExecuteNonQuery(ConnectionFactory.GetConnection(), CommandType.Text, updateCommitteeRoleSql, sqlParams);
            
        }

        //Delete if not uncommented
        //public static List<UserGroupJsonModel> GetGroupsForUserId(int id )
        //{
        //    DataObjectList<UserGroupJsonModel> users = new DataObjectList<UserGroupJsonModel>();
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append("SELECT u.UserId, u.Prefix, u.FirstName, u.MiddleName, u.LastName, u.Suffix, c.CompanyName, c.CompanyTypeId, -1 as CommitteeRoleId , u.Email ");
        //    sb.Append("FROM [User] u LEFT JOIN UserCompany uc ON uc.UserId = u.UserId LEFT JOIN Company c on uc.CompanyId = c.CompanyId ");
        //    sb.AppendFormat("WHERE u.UserId" + DbUtil.ConvertIdsToSqlIn(ids));
        //    users.LoadBySqlText(null, sb.ToString());
        //    List<GroupUser> items = new List<GroupUser>(users.ToArray());
        //    items.Sort();
        //    return items;
        //}

        #region private methods
        private static List<GroupUser> GetGroupUsers(int groupId, string storedProcedureName)
        {
            SqlParameter[] sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter("GroupId", groupId); 
            return GetGroupUsers(sqlParams, storedProcedureName);
        }

        private static List<GroupUser> GetGroupUsers(SqlParameter[] sqlParams, string storedProcedureName)
        { 
            DataObjectList<GroupUser> users = new DataObjectList<GroupUser>(sqlParams, storedProcedureName);
            List<GroupUser> items = new List<GroupUser>(users.ToArray());
            items.Sort();
            return items;
        }
        #endregion

        [DataObject()]
        public class GroupUser : DataObjectBase, IComparable
        {
            #region private members 
            private int _userId; 
            private string _prefix;
            private string _firstName;
            private string _middleName;
            private string _lastName;
            private string _suffix; 
            private string _company;
            private int _companyTypeId; 
            private int _committeeRoleId;
            private int _companyId;
            private string _email;
            
            #endregion 
            public GroupUser()
            {
            } 
            #region public properties

            [DataObjectProperty("UserId", SqlDbType.Int, true)]
            public int UserId
            {
                get { return (this._userId); }
                set { this._userId = value; }
            } 

            [DataObjectProperty("Prefix", SqlDbType.NVarChar, 5)]
            public string Prefix
            {
                get { return (this._prefix); }
                set { this._prefix = value; }
            }

            [DataObjectProperty("FirstName", SqlDbType.NVarChar, 75)]
            public string FirstName
            {
                get { return (this._firstName); }
                set { this._firstName = value; }
            }

            [DataObjectProperty("MiddleName", SqlDbType.NVarChar, 50)]
            public string MiddleName
            {
                get { return (this._middleName); }
                set { this._middleName = value; }
            }

            [DataObjectProperty("LastName", SqlDbType.NVarChar, 100)]
            public string LastName
            {
                get { return (this._lastName); }
                set { this._lastName = value; }
            }

            [DataObjectProperty("Suffix", SqlDbType.NVarChar, 7)]
            public string Suffix
            {
                get { return (this._suffix); }
                set { this._suffix = value; }
            } 
            
            [DataObjectProperty("CompanyName", SqlDbType.NVarChar, 255)]
            public string Company
            {
                get { return this._company; }
                set { this._company = value; }
            }

            [DataObjectProperty("CompanyTypeId", SqlDbType.Int, true)]
            public int CompanyTypeId
            {
                get { return (this._companyTypeId); }
                set { this._companyTypeId = value; }
            }

            [DataObjectProperty("CommitteeRoleId", SqlDbType.Int, true)]
            public int CommitteeRoleId
            {
                get { return (this._committeeRoleId); }
                set { this._committeeRoleId = value; }
            }

            [DataObjectProperty("CompanyId", SqlDbType.Int, true)]
            public int CompanyId
            {
                get { return (this._companyId); }
                set { this._companyId = value; }
            }

            [DataObjectProperty("Email", SqlDbType.NVarChar, 256)]
            public string Email
            {
                get { return (this._email); }
                set { this._email = value; }
            }

            public string LastAndFirstName
            {
                get
                {
                    return string.Format("{0}, {1}", this._lastName, this._firstName); 
                }
            } 

            public string FormattedDisplayName
            {
                get
                { 
                    string result = string.Format("{0}, {1}", this._lastName, this._firstName);
                    return (string.IsNullOrEmpty(this.Company)) ? result : result + "(" + this.Company + ")";
                }
            } 
            #endregion

            #region IComparable

            public int CompareTo(object obj)
            {
                if (obj is GroupUser)
                {
                    GroupUser otherUser = obj as GroupUser; 
                    string strThisLast = string.IsNullOrEmpty(this.LastName) ? string.Empty : this.LastName.ToLower();
                    string strThisFirst = string.IsNullOrEmpty(this.FirstName) ? string.Empty : this.FirstName.ToLower();
                    string strOtherLast = string.IsNullOrEmpty(otherUser.LastName) ? string.Empty : otherUser.LastName.ToLower();
                    string strOtherFirst = string.IsNullOrEmpty(otherUser.FirstName) ? string.Empty : otherUser.FirstName.ToLower();

                    if (strThisLast.CompareTo(strOtherLast) < 0)
                        return -1;
                    else if (strThisLast.CompareTo(strOtherLast) > 0)
                        return 1;
                    else if (strThisFirst.CompareTo(strOtherFirst) < 0)
                        return -1;
                    else if (strThisFirst.CompareTo(strOtherFirst) > 0)
                        return 1;
                    else
                        return 0;
                }

                throw new ArgumentException("object is not a GroupUser");
            }

            #endregion
        }
    }
}
