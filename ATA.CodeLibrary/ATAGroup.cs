#region namespaces

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using SusQTech.Data.DataObjects;
using System.Web.Security;
using ATA.LyrisProxy;
using ATA.LyrisProxy.LyrisSoapProxy;
using ATA.Member.Util;
using Microsoft.ApplicationBlocks.Data;

#endregion

namespace ATA.ObjectModel
{
    [DataObject("p_Group_Create", "p_Group_Update", "p_Group_Load", "p_Group_Delete")]
    public class ATAGroup : DataObjectBase
    {
        #region private members

        private int _groupId;
        private string _groupName;
        private bool _isOpenEnrollemnt;
        private int _appliesToSiteId;
        private string _lyrisListName;
        private string _lyrisShortDescription;
        private string _lyrisListType;
        private string _groupSiteUrl;
        private bool _isCommitteeSite;
        private bool _isChildGroup;
        private int _parentGroupId;
        private int _groupTypeId;
        private bool _isSecurityGroup;
        private DateTime _createdDate;
        private DateTime _modifiedDate;
        private string _liaison1;
        private string _liaison2;
        private int _liaison1UserId;
        private int _liaison2UserId;
        private int _divisionId;
        private int _departmentId;
        private string _mission;
        private int _lyrisSendId;
        private bool _bounceReports;
        private bool _gAB;
        private bool _isNewGroup;

        #endregion

        public ATAGroup()
        {
            this.GroupId = DataObjectBase.NullIntRowId;
            this.GroupName = string.Empty;
            this.IsOpenEnrollment = false;
        }
        public ATAGroup(int GroupId)
        {
            this.Load(GroupId);
        }
        /*****************************************/
        /***Group Create Methods***/
        /*****************************************/
        #region create group
        private const string LyrisTopic = "main";
        protected bool SkipLyrisManager { get { return Conf.SkipLyrisManager; } }
        private LyrisManager2 _lyrisManager = null;
        protected LyrisManager2 LyrisManager2
        {
            get
            {
                if (this._lyrisManager == null)
                {
                    this._lyrisManager = LyrisManager2.GetLyrisManager();
                }
                return (this._lyrisManager);
            }
        }

        //Added by NA on 10/24/2017 - to update informational groups based on parent groups
        public void updateChildGroups(ATAGroup parentGroup)
        {
            ATAGroup childGrp = parentGroup.GetCommitteeChildGroup();
            childGrp.IsCommittee = parentGroup.IsCommittee;
            childGrp.AppliesToSite = (AppliesToSite)parentGroup.AppliesToSiteId;//ATAMembershipUtility.Instance.CurrentSite;

            childGrp.GroupSiteUrl = parentGroup.GroupSiteUrl;
            childGrp.LyrisListName = string.Format("{0}info", parentGroup.LyrisListName);
            childGrp.LyrisListType = parentGroup.LyrisListType;
            childGrp.LyrisSendId = parentGroup.LyrisSendId;
            childGrp.ModifiedDate = DateTime.Now;
            childGrp.Liaison1UserId = parentGroup.Liaison1UserId;
            childGrp.Liaison1 = parentGroup.Liaison1;
            childGrp.DivisionId = parentGroup.DivisionId;
            childGrp.DepartmentId = parentGroup.DepartmentId;
            if (childGrp.Save())
            {
                try
                {
                    if (!SkipLyrisManager)
                        this.LyrisManager2.UpdateExistingList(childGrp.LyrisListName, childGrp.LyrisShortDescription);
                }
                catch (Exception err)
                {
                    //What do we do when a lyris list cannot be created or updated?  
                    Log.LogError("Error in updating Lyris list for child group " + childGrp.GroupName);
                    throw new Exception(string.Format("Error in updating Lyris list for child group {0}. Error {1} " + childGrp.GroupName, err.InnerException));
                }
            }
        }
        public void createChildGroups(ATAGroup parentGroup)
        {
            string groupName = "Info";
            ATAGroup group = new ATAGroup();
            //Tell the group to not hold its exceptions.
            group.ThrowExceptions = true;
            group.GroupName = string.Format("{0} {1}", parentGroup.GroupName, groupName);
            group.LyrisShortDescription = string.Format("{0}{1}", parentGroup.LyrisShortDescription, groupName);
            group.IsOpenEnrollment = false;
            group.IsCommittee = false;
            group.AppliesToSite = (AppliesToSite)parentGroup.AppliesToSiteId;
            group.IsChildGroup = true;
            group.ParentGroupId = parentGroup.GroupId;
            group.GroupSiteUrl = parentGroup.GroupSiteUrl;
            group.LyrisListName = string.Format("{0}info", parentGroup.LyrisListName);
            group.LyrisListType = parentGroup.LyrisListType;
            group.IsSecurityGroup = parentGroup.IsSecurityGroup;
            group.GroupTypeId = parentGroup.GroupTypeId;
            group.LyrisSendId = parentGroup.LyrisSendId;
            group.CreatedDate = DateTime.Now;
            group.ModifiedDate = DateTime.Now;
            group.Liaison1UserId = parentGroup.Liaison1UserId;
            group.Liaison1 = parentGroup.Liaison1;
            group.DivisionId = parentGroup.DivisionId;
            group.DepartmentId = parentGroup.DepartmentId;

            if (group.Save())
            {
                //if we were able to save our new group, try and create the lyris list for it.
                try
                {
                    if (!SkipLyrisManager)
                    {
                        group.LyrisListName = this.LyrisManager2.CreateList(group.LyrisListType, group.LyrisListName, group.LyrisShortDescription, LyrisTopic);
                    }
                }
                catch (Exception err)
                {
                    // what do we do when a lyris list cannot be created or updated?  
                    Log.LogError("Error in creating Lyris list for group " + group.GroupName);
                    throw new Exception(string.Format("Error in creating Lyris list for group {0}. Error {1} " + group.GroupName, err.InnerException));
                }
            }
            else
            {
                throw new Exception(string.Format("Could not create group: {0} for committee group {1}", groupName, parentGroup.GroupName));
            }
        }

        public int createChildGroupsRetID(ATAGroup parentGroup)
        {
            bool removeadmin = true;
            string groupName = "Info";
            ATAGroup group = new ATAGroup();
            //Tell the group to not hold its exceptions.
            group.ThrowExceptions = true;
            group.GroupName = string.Format("{0} {1}", parentGroup.GroupName, groupName);
            group.LyrisShortDescription = string.Format("{0}{1}", parentGroup.LyrisShortDescription, groupName);
            group.IsOpenEnrollment = false;
            group.IsCommittee = false;
            group.AppliesToSite = (AppliesToSite)parentGroup.AppliesToSiteId;
            group.IsChildGroup = true;
            group.ParentGroupId = parentGroup.GroupId;
            group.GroupSiteUrl = parentGroup.GroupSiteUrl;
            group.LyrisListName = string.Format("{0}info", parentGroup.LyrisListName);
            group.LyrisListType = parentGroup.LyrisListType;
            group.IsSecurityGroup = parentGroup.IsSecurityGroup;
            group.GroupTypeId = parentGroup.GroupTypeId;
            group.LyrisSendId = parentGroup.LyrisSendId;
            group.CreatedDate = DateTime.Now;
            group.ModifiedDate = DateTime.Now;
            group.Liaison1UserId = parentGroup.Liaison1UserId;
            group.Liaison1 = parentGroup.Liaison1;
            group.DivisionId = parentGroup.DivisionId;
            group.DepartmentId = parentGroup.DepartmentId;
            group.GAB = parentGroup.GAB;
            if (group.Save())
            {
                //if we were able to save our new group, try and create the lyris list for it.
                try
                {

                    if (false)
                    {
                        //group.LyrisListName = this.LyrisManager2.CreateList(group.LyrisListType, group.LyrisListName, group.LyrisShortDescription, LyrisTopic);
                        //group.updateList(group.LyrisListName, group.LyrisShortDescription, true);
                        //LyrisMember.UpdateLyrisSend(group.LyrisListName, group.LyrisSendId);
                        ///************Remove admin@a4a.org or any email set as admin and is added to Lyris - config - keyword - SusQTechLyrisManagerEmail ********************/
                        //removeadmin = LyrisMember.RemoveAdminfromList(group.LyrisListName);
                    }
                    return group.GroupId;
                }
                catch (Exception err)
                {
                    string message;
                    // what do we do when a lyris list cannot be created or updated?  
                    if (removeadmin)
                        message = string.Format("Error in creating Lyris list for group {0}. Error {1} " + group.GroupName, err.InnerException + ". There was an error in removing Admin email group!");
                    else
                        message = string.Format("Error in creating Lyris list for group {0}. Error {1} " + group.GroupName, err.InnerException);
                    Log.LogError("Error in creating Lyris list for group " + group.GroupName);
                    throw new Exception();

                }

            }
            else
            {
                throw new Exception(string.Format("Could not create group: {0} for committee group {1}", groupName, parentGroup.GroupName));

            }
        }
        #endregion

        #region update Lyris list
        public bool updateList(string listName, string description, bool isNewGroup)
        {
            if (SkipLyrisManager)
                return true;
            if (isNewGroup)
                this.LyrisManager2.UpdateNewList(listName, description);
            else
                this.LyrisManager2.UpdateExistingList(listName, description);
            return true;
        }
        #endregion 
        #region Group types
        private const string groupTypeSelectionStr = "SELECT GroupTypeName, GroupTypeId FROM GroupType WHERE {0} = 1";
        public static SortedDictionary<string, int> GetSecurityGroupTypes()
        {
            return DbUtil.GetSortedStringIdDictionaryBySql(string.Format(groupTypeSelectionStr, "IsSecurityCategory"));
        }

        public static SortedDictionary<string, int> GetDistributionGroupTypes()
        {
            return DbUtil.GetSortedStringIdDictionaryBySql(string.Format(groupTypeSelectionStr, "IsDistributionCategory"));
        }

        public static SortedDictionary<string, int> GetCommitteeGroupTypes()
        {
            return DbUtil.GetSortedStringIdDictionaryBySql(string.Format(groupTypeSelectionStr, "IsCommitteeCategory"));
        }

        public static Dictionary<int, string> GetAllGroupTypes()
        {
            return DbUtil.GetIdStringDictionary("Select GroupTypeId, GroupTypeName from GroupType where Active = 1 order by GroupTypeName", false, null);
        }
        //public static string GetGroupTypeIdByGroup(int groupId)
        //{
        //    string idParamName = "@groupId";
        //    string sql = "SELECT GroupTypeName FROM GroupType WHERE GroupTypeId = " + idParamName;

        //}
        public static string GetGroupTypeById(int groupTypeId)
        {
            string idParamName = "@GroupTypeId";
            string sql = "SELECT GroupTypeName FROM GroupType WHERE GroupTypeId = " + idParamName;
            return DbUtil.GetNameById(groupTypeId, idParamName, sql, true);
        }

        #endregion 
        #region public properties

        [DataObjectProperty("GroupId", SqlDbType.Int, true)]
        public int GroupId
        {
            get { return (this._groupId); }
            set { this._groupId = value; }
        }

        [DataObjectProperty("GroupName", SqlDbType.NVarChar, 50)]
        public string GroupName
        {
            get { return (this._groupName); }
            set { this._groupName = value; }
        }

        [DataObjectProperty("IsOpenEnrollment", SqlDbType.Bit)]
        public bool IsOpenEnrollment
        {
            get { return (this._isOpenEnrollemnt); }
            set { this._isOpenEnrollemnt = value; }
        }

        [DataObjectProperty("AppliesToSiteId", SqlDbType.Int)]
        public int AppliesToSiteId
        {
            get { return (this._appliesToSiteId); }
            set { this._appliesToSiteId = value; }
        }

        public AppliesToSite AppliesToSite
        {
            get { return ((AppliesToSite)this._appliesToSiteId); }
            set { this._appliesToSiteId = (int)value; }
        }

        [DataObjectProperty("LyrisListName", SqlDbType.NVarChar, 60)]
        public string LyrisListName
        {
            get { return (this._lyrisListName); }
            set { this._lyrisListName = value; }
        }

        [DataObjectProperty("LyrisShortDescription", SqlDbType.NVarChar, 200)]
        public string LyrisShortDescription
        {
            get { return (this._lyrisShortDescription); }
            set { this._lyrisShortDescription = value; }
        }

        [DataObjectProperty("LyrisListType", SqlDbType.NVarChar, 35)]
        public string LyrisListType
        {
            get { return (this._lyrisListType); }
            set { this._lyrisListType = value; }
        }

        [DataObjectProperty("GroupSiteUrl", SqlDbType.NVarChar, 512)]
        public string GroupSiteUrl
        {
            get { return (this._groupSiteUrl); }
            set { this._groupSiteUrl = value; }
        }

        [DataObjectProperty("IsCommittee", SqlDbType.Bit)]
        public bool IsCommittee
        {
            get { return (this._isCommitteeSite); }
            set { this._isCommitteeSite = value; }
        }

        [DataObjectProperty("IsChildGroup", SqlDbType.Bit)]
        public bool IsChildGroup
        {
            get { return this._isChildGroup; }
            set { this._isChildGroup = value; }
        }
        [DataObjectProperty("ParentGroupId", SqlDbType.Int)]
        public int ParentGroupId
        {
            get { return this._parentGroupId; }
            set { this._parentGroupId = value; }
        }

        [DataObjectProperty("GroupTypeId", SqlDbType.Int)]
        public int GroupTypeId
        {
            get { return this._groupTypeId; }
            set { this._groupTypeId = value; }
        }

        [DataObjectProperty("IsSecurityGroup", SqlDbType.Bit)]
        public bool IsSecurityGroup
        {
            get { return (this._isSecurityGroup); }
            set { this._isSecurityGroup = value; }
        }

        [DataObjectProperty("CreatedDate", SqlDbType.DateTime)]
        public DateTime CreatedDate
        {
            get { return (this._createdDate); }
            set { this._createdDate = value; }
        }

        [DataObjectProperty("ModifiedDate", SqlDbType.DateTime)]
        public DateTime ModifiedDate
        {
            get { return (this._modifiedDate); }
            set { this._modifiedDate = value; }
        }

        [DataObjectProperty("Liaison1", SqlDbType.NVarChar)]
        public string Liaison1
        {
            get { return (this._liaison1); }
            set { this._liaison1 = value; }
        }

        [DataObjectProperty("Liaison1UserId", SqlDbType.Int)]
        public int Liaison1UserId
        {
            get { return (this._liaison1UserId); }
            set { this._liaison1UserId = value; }
        }

        [DataObjectProperty("Liaison2", SqlDbType.NVarChar)]
        public string Liaison2
        {
            get { return (this._liaison2); }
            set { this._liaison2 = value; }
        }

        [DataObjectProperty("Liaison2UserId", SqlDbType.Int)]
        public int Liaison2UserId
        {
            get { return (this._liaison2UserId); }
            set { this._liaison2UserId = value; }
        }

        [DataObjectProperty("DivisionId", SqlDbType.Int)]
        public int DivisionId
        {
            get { return (this._divisionId); }
            set { this._divisionId = value; }
        }

        [DataObjectProperty("DepartmentId", SqlDbType.Int)]
        public int DepartmentId
        {
            get { return (this._departmentId); }
            set { this._departmentId = value; }
        }

        [DataObjectProperty("Mission", SqlDbType.NVarChar)]
        public string Mission
        {
            get { return (this._mission); }
            set { this._mission = value; }
        }

        [DataObjectProperty("LyrisSendId", SqlDbType.Int)]
        public int LyrisSendId
        {
            get { return (this._lyrisSendId); }
            set { this._lyrisSendId = value; }
        }

        [DataObjectProperty("BounceReports", SqlDbType.Bit)]
        public bool BounceReports
        {
            get { return (this._bounceReports); }
            set { this._bounceReports = value; }
        }

        [DataObjectProperty("GAB", SqlDbType.Bit)]
        public bool GAB
        {
            get { return (this._gAB); }
            set { this._gAB = value; }
        }
        #endregion
        [DataObjectProperty("IsNewGroup", SqlDbType.Bit)]
        public bool IsNewGroup
        {
            get { return (this._isNewGroup); }
            set { this._isNewGroup = value; }
        }


        #region Group Admin   
        public static List<int> GetGroupIdsByGroupAdminId(int userId)
        {
            List<int> allGroupIds = new List<int>();
            SqlParameter[] sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter("UserId", userId);
            using (SqlDataReader reader = SqlHelper.ExecuteReader(ConnectionFactory.DataObjectConnectionString, CommandType.StoredProcedure, "p_GroupGetGroupIdsByGroupAdmin", sqlParams))
            {
                while (reader.Read())
                {
                    allGroupIds.Add(reader.GetInt32(0));
                }
                reader.Close();
            }
            return allGroupIds;
        }

        public static bool IsUserGroupAdmin(int GroupId, int UserId)
        {
            List<int> admins = CodeLibrary.ATAGroupManager.GetGroupManageAdmins(GroupId);
            if (admins.Contains(UserId))//is admin
                return true;
            return false;
        }
        public static void AddGroupAdmin(ATAGroup group, int userId, bool isBounceAdmin)
        {
            int groupId = group.GroupId;
            SqlParameter[] sqlParams = CreateAdminParams(groupId, userId, isBounceAdmin);
            string sql = insertGroupAdminSql;
            try
            {
                int result = SqlHelper.ExecuteNonQuery(ConnectionFactory.DataObjectConnectionString, CommandType.Text, sql, sqlParams);
                if (result < 1)
                    Log.LogWarning(string.Format("Adding group admin to member DB for group {0} and user {1} return {2}", groupId, userId, result));
            }
            catch (Exception e)
            {
                Log.LogError(string.Format("Error in adding group admin to member DB for group {0} and user {1} because {2}", groupId, userId, e.ToString()));
                throw;
            }

        }

        private const string insertGroupAdminSql = @"INSERT INTO dbo.GroupAdmin(GroupId, UserId, IsBounceAdmin) VALUES (@GroupId, @UserId, @IsBounceAdmin)";
        private const string updateBounceAdminSql = @"UPDATE     dbo.GroupAdmin SET IsBounceAdmin=@IsBounceAdmin WHERE GroupId=@GroupId AND UserId=@UserId";
        private const string deleteGroupAdminSql = @"DELETE FROM dbo.GroupAdmin                                  WHERE GroupId=@GroupId AND UserId=@UserId";

        private static SqlParameter[] CreateAdminParams(int groupId, int userId, bool isBounceAdmin)
        {
            SqlParameter[] sqlParams = new SqlParameter[3];
            addAdminGroupIdUserIdParams(ref sqlParams, groupId, userId);
            sqlParams[2] = new SqlParameter("IsBounceAdmin", isBounceAdmin);
            return sqlParams;
        }

        private static void addAdminGroupIdUserIdParams(ref SqlParameter[] sqlParams, int groupId, int userId)
        {
            sqlParams[0] = new SqlParameter("GroupId", groupId);
            sqlParams[1] = new SqlParameter("UserId", userId);
        }

        private static SqlParameter[] CreateAdminParams(int groupId, int userId)
        {
            SqlParameter[] sqlParams = new SqlParameter[2];
            addAdminGroupIdUserIdParams(ref sqlParams, groupId, userId);
            return sqlParams;
        }


        public static void UpdateGroupBounceAdmin(ATAGroup group, int userId, bool isBounceAdmin)
        {
            int groupId = group.GroupId;
            SqlParameter[] sqlParams = CreateAdminParams(groupId, userId, isBounceAdmin);
            string sql = updateBounceAdminSql;
            try
            {
                int result = SqlHelper.ExecuteNonQuery(ConnectionFactory.DataObjectConnectionString, CommandType.Text, sql, sqlParams);
                if (result < 1)
                    Log.LogWarning(string.Format("Updating group bounce admin for group {0} and user {1} return {2}", groupId, userId, result));
            }
            catch (Exception e)
            {
                Log.LogError(string.Format("Error in Updating group bounce admin for group {0} and user {1} because {2}", groupId, userId, e.ToString()));
                throw;
            }

        }

        public static bool IsUserADAdmin(string userName)
        {
            string adManagerDomain = ConfigurationManager.AppSettings["SusQTechActiveDirectoryManagerDomain"];
            if (adManagerDomain.LastIndexOf(".") > 0)//remove the .com, .org or .net etc
            {
                adManagerDomain = adManagerDomain.Substring(0, adManagerDomain.LastIndexOf("."));
            }
            SusQtech.ActiveDirectoryServices.ADGroup group = GetAdAdminGroup();
            foreach (SusQtech.ActiveDirectoryServices.ADUser adUser in group.Users)
            {
                if (userName.ToLower() == string.Format(@"{0}\{1}", adManagerDomain, adUser.UserName).ToLower())
                {
                    return true;
                }
            }
            return false;
        }

        //public void AddGroupAdminsFromADGroup()
        //{
        //    try
        //    {   
        //        //need to get all users from this group and add them
        //        //if (SusQtech.ActiveDirectoryServices.ADManager.Instance.WorkingGroupExists(ConfigurationManager.AppSettings["MembersGroupAdminADGroup"]))
        //        //{
        //        //need to setup our config for ADManager INstance

        //        SusQtech.ActiveDirectoryServices.ADGroup group = GetAdAdminGroup();
        //        foreach (SusQtech.ActiveDirectoryServices.ADUser adUser in group.Users)
        //        {
        //            LyrisManager manager = LyrisManagerFactory.GetLyrisManager();
        //            if (!string.IsNullOrEmpty(adUser.Email))
        //            {
        //                manager.AddMemberToList(adUser.Email, adUser.DisplayName, this.LyrisListName);
        //                manager.UpdateListAdmin(this.LyrisListName, adUser.Email, true, true, true, true);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.EventLog.WriteEntry("ADGroup",ex.Message + Environment.NewLine+ ex.StackTrace + Environment.NewLine + Environment.NewLine + ConfigurationManager.AppSettings["MembersGroupAdminADGroup"], System.Diagnostics.EventLogEntryType.Error);
        //    }
        //}

        public static SusQtech.ActiveDirectoryServices.ADGroup GetAdAdminGroup()
        {
            string adManagerDomain = string.Empty;
            string adManagerUser = string.Empty;
            string adManagerPassword = string.Empty;
            string adManagerUsersPath = string.Empty;
            string adManagerWSSUrl = string.Empty;

            try
            {
                adManagerDomain = ConfigurationManager.AppSettings["SusQTechActiveDirectoryManagerDomain"];
                adManagerUser = ConfigurationManager.AppSettings["SusQTechActiveDirectoryManagerADUser"];
                adManagerPassword = ConfigurationManager.AppSettings["SusQTechActiveDirectoryManagerADPassword"];
                adManagerUsersPath = ConfigurationManager.AppSettings["SusQTechActiveDirectoryManagerADUsersPath"];
                adManagerWSSUrl = ConfigurationManager.AppSettings["SusQTechActiveDirectoryManagerWSSUrl"];
            }
            catch { }
            SusQtech.ActiveDirectoryServices.Configuration.Configuration.EstablishInstance(adManagerDomain, adManagerUser, adManagerPassword, adManagerUsersPath, adManagerWSSUrl);
            SusQtech.ActiveDirectoryServices.ADGroup group = SusQtech.ActiveDirectoryServices.ADManager.Instance.LoadGroup(ConfigurationManager.AppSettings["MembersGroupAdminADGroup"]);
            return group;
        }

        public static void RemoveGroupAdmin(ATAGroup group, int userId)
        {
            int groupId = group.GroupId;
            SqlParameter[] sqlParams = CreateAdminParams(groupId, userId);
            string sql = deleteGroupAdminSql;
            try
            {
                int result = SqlHelper.ExecuteNonQuery(ConnectionFactory.DataObjectConnectionString, CommandType.Text, sql, sqlParams);
                if (result < 1)
                    Log.LogWarning(string.Format("Deleting group admin from member DB for group {0} and user {1} return {2}", groupId, userId, result));
            }
            catch (Exception e)
            {
                Log.LogError(string.Format("Error in deleting group admin from member DB for group {0} and user {1} because {2}", groupId, userId, e.ToString()));
                throw;
            }
            //if (Conf.DoNotAutoPushGroupDataToLyris || Conf.SkipLyrisManager)
            //    return;
            //try
            //{
            //    LookupUser user = new LookupUser(userId);
            //    LyrisManager manager = LyrisManagerFactory.GetLyrisManager();
            //    manager.UpdateListAdmin(group.LyrisListName, user.Email, false, false, false, false);
            //    try
            //    {
            //        manager.RemoveMemberFromList(group.LyrisListName, user.Email);
            //    }
            //    catch { }
            //}
            //catch (Exception e)
            //{
            //    Log.LogError(string.Format("Error in removing group admin from Lyris list for group {0} and user {1} because {2}", groupId, userId, e.ToString()));
            //    throw;
            //} 
        }

        //TODO: This method is not used anywhere so comment it out
        //public void ClearAllGroupAdmins()
        //{
        //    if (this._groupAdmins == null)
        //        this._groupAdmins = new ManyToManyRelationship<ATAGroup, LookupUser>("GroupAdmin");

        //    LookupUser[] users = this._groupAdmins.GetSecondTypeByFirstId(this.GroupId, true);

        //    LyrisManager manager = LyrisManagerFactory.GetLyrisManager();

        //    foreach (LookupUser user in users)
        //        manager.UpdateListAdmin(this.LyrisListName, user.Email, false, false, false, false);

        //    this._groupAdmins.ClearAllByFirstTypeId(this.GroupId);
        //}

        #endregion

        #region IsInUse

        public bool IsInUse
        {
            get
            {
                bool isInUse = false;

                SqlCommand cmd = new SqlCommand("p_Group_IsInUse");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@GroupId", this.GroupId));
                cmd.Parameters.Add(new SqlParameter("@IsInUse", SqlDbType.Bit));
                cmd.Parameters[1].Direction = ParameterDirection.Output;
                cmd.Connection = ConnectionFactory.GetConnection();

                try
                {
                    cmd.Connection.Open();
                    isInUse = (bool)cmd.ExecuteScalar();
                }
                catch (Exception err)
                {
                    this.SetException(err);
                }
                finally
                {
                    cmd.Connection.Close();
                    cmd.Connection.Dispose();
                    cmd.Dispose();
                }

                return (isInUse);
            }
        }

        #endregion

        /*****************************/
        /**Edited by NA 10/24/2016 **/
        /*****************************/
        public static Dictionary<int, string> GetAllGroupDescription(AppliesToSite appliesToSite, string paramter, string value)
        {
            try
            {
                string sql = string.Format("SELECT GroupId, LyrisShortDescription FROM [Group]");

                Dictionary<int, string> groupMap = new Dictionary<int, string>();
                if (!String.IsNullOrEmpty(paramter))
                {
                    sql = string.Format("SELECT GroupId, LyrisShortDescription FROM [Group] where  {0} = {1} and AppliesToSiteId = {2}", paramter, value, appliesToSite);
                }
                else
                {
                    sql = string.Format("SELECT GroupId, LyrisShortDescription FROM [Group] where  AppliesToSiteId = {0}", appliesToSite);
                }
                int groupId = -1;
                string shortDesc = string.Empty;
                using (SqlDataReader reader = SqlHelper.ExecuteReader(Conf.ConnectionString, CommandType.Text, sql, null))
                {
                    while (reader.Read())
                    {
                        groupId = reader.GetInt32(0);
                        shortDesc = reader.IsDBNull(1) ? string.Empty : reader.GetString(1); ;
                        groupMap[groupId] = shortDesc;
                    }
                }
                return groupMap;
            }
            catch (Exception e)
            {
                Log.LogError(string.Format("Error in GetAllGroupDescription {0} ", e.ToString()));
                throw;
            }
        }

        private const string GetAllNonChildGroupsSP = "p_Group_GetAllNonChildGroups";
        public static List<ATAGroup> GetAllNonChildGroups(AppliesToSite appliesToSite)
        {
            SqlParameter[] sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter("@AppliesToSiteId", appliesToSite);
            DataObjectList<ATAGroup> groups = new DataObjectList<ATAGroup>(sqlParams, GetAllNonChildGroupsSP);
            return (groups as List<ATAGroup>);
        }
        //public static ATAGroup GetGroupbyGroupId(int gid)
        //{
        //    SqlParameter[] sqlParams = new SqlParameter[1];
        //    sqlParams[0] = new SqlParameter("@GroupId", gid);
        //    DataObjectList<ATAGroup> groups = new DataObjectList<ATAGroup>(sqlParams, GetAllNonChildGroupsSP);
        //    return (groups[0] as ATAGroup);
        //}


        public static SortedDictionary<string, int> GetAllGroupNamesWithId()
        {
            string sql = "SELECT GroupName, GroupId FROM [Group] WHERE IsChildGroup = 0";
            return DbUtil.GetSortedStringIdDictionaryBySql(sql);
        }
        public static SortedDictionary<string, int> GetDistributionGroupNamesWithId()
        {
            string sql = "SELECT GroupName, GroupId FROM [Group] WHERE IsChildGroup = 0 AND IsSecurityGroup = 0";
            return DbUtil.GetSortedStringIdDictionaryBySql(sql);
        }

        public static SortedDictionary<string, int> GetSecurityWithoutCommitteeGroupNamesWithId()
        {
            string sql = "SELECT GroupName, GroupId FROM [Group] WHERE IsChildGroup = 0 AND IsSecurityGroup = 1 AND IsCommittee = 0";
            return DbUtil.GetSortedStringIdDictionaryBySql(sql);
        }

        public static SortedDictionary<string, int> GetCommitteeGroupNamesWithId()
        {
            string sql = "SELECT GroupName, GroupId FROM [Group] WHERE IsChildGroup = 0 AND IsCommittee = 1";
            return DbUtil.GetSortedStringIdDictionaryBySql(sql);
        }

        public static SortedDictionary<string, int> GetCommitteeGroupShortDescriptionWithId()
        {
            string sql = "SELECT LyrisShortDescription, GroupId FROM [Group] WHERE IsChildGroup = 0 AND IsCommittee = 1";
            return DbUtil.GetSortedStringIdDictionaryBySql(sql);
        }

        public static SortedDictionary<string, int> GetCommitteeOtherGroupNamesWithId()
        {
            string sql = "SELECT GroupName, GroupId FROM [Group] WHERE IsChildGroup = 1 AND ParentGroupId > 0 AND GroupName like '% Other'";
            return DbUtil.GetSortedStringIdDictionaryBySql(sql);
        }

        public static SortedDictionary<string, int> GetAllUsersForCommitteeGroup(string CompanyName, string term)
        {
            StringBuilder sql = new StringBuilder();
            CompanyName = CompanyName.ToLower() == "all" ? "%" : CompanyName;
            term = term.ToLower() == "all" ? "%" : term;


            sql.Append("SELECT  u.LastName + ', ' + u.FirstName as Name, u.UserId FROM [User] u LEFT JOIN UserCompany uc ON uc.UserId = u.UserId LEFT JOIN Company c on uc.CompanyId = c.CompanyId WHERE   (u.IsActiveContact = 1  and u.IsActiveMB = 1)");
            if ((!string.IsNullOrEmpty(CompanyName)) && (!string.IsNullOrEmpty(term)))
            {
                //sql.Append(" and c.CompanyName like '" + CompanyName + "'  and (u.LastName + ', ' + u.FirstName ) like '%" + term +"%' ");
                sql.Append(" and c.CompanyName like '" + CompanyName + "'  and (u.LastName like '%" + term + "%' or u.FirstName like '%" + term + "%' or u.Username like '%" + term + "%')");
            }
            else
            if (!string.IsNullOrEmpty(CompanyName))
            {
                sql.Append(" and c.CompanyName like '" + CompanyName + "'");
            }
            else
            if (!string.IsNullOrEmpty(CompanyName))
            {
                sql.Append("  and (u.LastName + ', ' + u.FirstName ) like '%" + term + "%' ");
            }


            sql.Append("  ORDER BY LastName, FirstName");

            return DbUtil.GetSortedStringIdDictionaryBySql(sql.ToString());
        }

        public static void DeleteGroupAdmins(int groupId)
        {
            string sql = "Delete From GroupAdmin WHERE GroupId = " + groupId;
            DbUtil.ExecuteNonQuery(sql);
        }

        //[p_Group_GetAllUserCount]
        public int GetAllUserCount(int groupId)
        {
            SqlParameter[] sqlparams = new SqlParameter[1];
            sqlparams[0] = new SqlParameter("@GroupId", groupId);

            object o = SqlHelper.ExecuteScalar(Conf.ConnectionString, CommandType.StoredProcedure, "p_Group_GetAllUserCount", sqlparams);
            if (o != null)
                return (int)o;
            return -1;
        }

        //Modified version for the new UM - older version is in the bottom
        public string[] GetGroupEmailOnlyMembers(int groupId)
        {
            List<string> allUsers = new List<string>();
            SqlDataReader rdr = SqlHelper.ExecuteReader(Conf.ConnectionString, "p_Group_GetAllEmailMembers", new object[] { this.GroupId });
            while (rdr.Read())
            {
                allUsers.Add(rdr["Username"].ToString());
            }
            rdr.Close();
            return allUsers.ToArray();
        }

        public string[] GetGroupEmailMembersSubscriber(int groupId)
        {
            List<string> allUsers = new List<string>();
            SqlDataReader rdr = SqlHelper.ExecuteReader(Conf.ConnectionString, "p_Group_GetAllEmailMembersandSubscriber", new object[] { this.GroupId });
            while (rdr.Read())
            {
                allUsers.Add(rdr["Email"].ToString());
            }
            rdr.Close();
            return allUsers.ToArray();
        }

        public static int GetUserId(string Username)
        {
            using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
            {
                StringBuilder sql = new StringBuilder();

                using (SqlCommand cmd = new SqlCommand("Select UserId from [User] where UPPER(Username) = '" + Username + "'", con))
                {
                    con.Open();
                    cmd.CommandType = CommandType.Text;
                    var reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            return Int32.Parse(reader["UserId"].ToString());
                        }
                    }
                }
            }

            return 0;
        }
        #region Members
        //   public ATAMembershipUser[] GetGroupMembers()
        //    {
        //        List<ATAMembershipUser> allUsers = new List<ATAMembershipUser>();
        //    ATAMembershipProvider provider = ATAMembershipUtility.Instance.GetATAMembershipProvider();
        //    SqlDataReader rdr = SqlHelper.ExecuteReader(ConfigurationManager.ConnectionStrings[provider.ConnectionStringName].ConnectionString, "p_Group_GetAllMembers", new object[] { this.GroupId });
        //        while (rdr.Read())
        //        {
        //            allUsers.Add(provider.NewATAMembershipUserFromReader(rdr));
        //        }
        //rdr.Close();
        //        return allUsers.ToArray();
        //    }

        //      //  Added by NA on 6/27/18 - Because this was added for Copy Email addresses functionality.Not sure why the top part is commented.Figure that out later.
        //      public ATAMembershipUser[] GetGroupEmailOnlyMembers()
        //        {
        //    List<ATAMembershipUser> allUsers = new List<ATAMembershipUser>();
        //    ATAMembershipProvider provider = ATAMembershipUtility.Instance.GetATAMembershipProvider();
        //    SqlDataReader rdr = SqlHelper.ExecuteReader(ConfigurationManager.ConnectionStrings[provider.ConnectionStringName].ConnectionString, "p_Group_GetAllEmailMembers", new object[] { this.GroupId });
        //    while (rdr.Read())
        //    {
        //        allUsers.Add(provider.NewATAMembershipUserFromReader(rdr));
        //    }
        //    rdr.Close();
        //    return allUsers.ToArray();
        //}
        #endregion

        #region Child Groups
        public ATAGroup GetCommitteeChildGroup()
        {
            SqlParameter[] sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter("@ParentGroupId", this.GroupId);
            List<int> childGroupIds = DbUtil.GetIdsFromDatabase("p_Group_GetChildGroups", CommandType.StoredProcedure, sqlParams);
            if (childGroupIds.Count != 1)
                throw new Exception(this.GroupName + " should have one and only one child group");
            return childGroupIds.Count > 0 ? new ATAGroup(childGroupIds[0]) : null;
        }
        #endregion
    }
}