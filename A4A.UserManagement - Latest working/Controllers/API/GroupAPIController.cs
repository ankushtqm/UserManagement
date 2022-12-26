using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Web.Http; 
using ATA.ObjectModel;
using ATA.Member.Util;
using ATA.LyrisProxy.LyrisSoapProxy;
using ATA.LyrisProxy;
using System.Data;
using System.Data.SqlClient;
using ATA.Authentication;
using ATA.Authentication.Providers;
using System.Dynamic;

namespace A4A.UM.Controllers
{ 
    public class GroupAPIController : ApiController
    {
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
     
        [Route("api/group")]
        public IEnumerable<ATAGroup> Get()
        {
            return ATAGroup.GetAllNonChildGroups(AppliesToSite.Both);
        } 
        [Route("api/getGroupNamesWithId")]
        public SortedDictionary<string,int> GetGroupsNamesWithId()
        { 
            return ATAGroup.GetAllGroupNamesWithId();
        } 
        [Route("api/getGroupType")]
        public  string GetGroupType()
        { 
            //Commented by NA - becasue the group name was not working for non IT users
            //bool isAdmin = GetIsCurrentUserAdmin();
            //if(isAdmin)
            //{ 
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
            {
                    StringBuilder sql = new StringBuilder();
                    using (SqlCommand cmd = new SqlCommand("Select GroupTypeId, GroupTypeName from GroupType where Active = 1 order by GroupTypeName", con))
                    {
                        con.Open(); 
                        cmd.CommandType = CommandType.Text;
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                        System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                        Dictionary<string, object> row;
                        foreach (DataRow dr in dt.Rows)
                        {
                            row = new Dictionary<string, object>();
                            foreach (DataColumn col in dt.Columns)
                            {
                                row.Add(col.ColumnName, dr[col]);
                            }
                            rows.Add(row);
                        }
                        return serializer.Serialize(rows);
                    }
                }
            //}
            //else
            //{
            //    StringBuilder sb = new StringBuilder();
            //    sb.Append("[{\"GroupTypeId\":6,\"GroupTypeName\":\"Distribution Group\"}]");
            //    return sb.ToString();
            //} 
        } 
        //[Route("api/getallgroup")]  Commenting because not being used. the one in DBUITil is being used. If no errors Delete
        //public List<ATAGroup> GetAllGroups()
        //{
        //    var allGroups = ATAGroup.GetAllNonChildGroups(ATA.ObjectModel.AppliesToSite.All);
        //    return allGroups;
        //} 
        public string GetUserName()
        {
            string editorUserName = ATAMembershipUtility.Instance.ParseUsername(this.User.Identity.Name); 
            return editorUserName;
        }
        [Route("api/GetIsCurrentUserAdmin")]
        public  bool GetIsCurrentUserAdmin()
        { 
            string editorUserName = ATAMembershipUtility.Instance.ParseUsername(this.User.Identity.Name);
            return DbUtil.GetIsCurrentUserAdmin(editorUserName, "ITSecurity"); 
        }
        [Route("api/GetIsCurrentUserGroupAdmin")]
        public bool GetIsCurrentUserGroupAdmin(int GroupId)
        {
            int UserId = GetCurrentUser().UserId;
            return ATAGroup.IsUserGroupAdmin(GroupId, UserId);
        }

        [Route("api/GetIsCurrentUseEcon")]
        public bool GetIsCurrentUserEcon()
        {
            string editorUserName = ATAMembershipUtility.Instance.ParseUsername(this.User.Identity.Name);
            return DbUtil.GetIsCurrentUserAdmin(editorUserName, "ECON");
            #region Delete
            //bool role = false;
            //// set up domain context
            //PrincipalContext ctx = new PrincipalContext(ContextType.Domain, "ata.org");
            //string editorUserName = ATAMembershipUtility.Instance.ParseUsername(this.User.Identity.Name);
            //// find a user
            //UserPrincipal user = UserPrincipal.FindByIdentity(ctx, editorUserName);
            //// find the group in question
            //GroupPrincipal group = GroupPrincipal.FindByIdentity(ctx, "ECON");
            //System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            //if (user != null)
            //{  // check if user is member of that group
            //    if (user.IsMemberOf(group))
            //    {
            //        role = true;
            //    }
            //}
            //return role;
            #endregion
        }

        [Route("api/GetIsCurrentUserITorECON")]
        public bool GetIsCurrentUserITorECON()
        {
            string editorUserName = ATAMembershipUtility.Instance.ParseUsername(this.User.Identity.Name);
            return (DbUtil.GetIsCurrentUserAdmin(editorUserName, "ITSecurity") || DbUtil.GetIsCurrentUserAdmin(editorUserName, "ECON"));

            #region Delete
            //bool role = false;
            //// set up domain context
            //PrincipalContext ctx = new PrincipalContext(ContextType.Domain, "ata.org");
            //string editorUserName = ATAMembershipUtility.Instance.ParseUsername(this.User.Identity.Name);
            //// find a user
            //UserPrincipal user = UserPrincipal.FindByIdentity(ctx, editorUserName);

            //// find the group in question
            //GroupPrincipal group = GroupPrincipal.FindByIdentity(ctx, "ITSecurity");
            //GroupPrincipal group1 = GroupPrincipal.FindByIdentity(ctx, "ECON");

            //System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            //if (user != null)
            //{
            //    // check if user is member of that group
            //    if (user.IsMemberOf(group) || user.IsMemberOf(group1))
            //    {
            //        role = true;
            //    }
            //}
            //return role;
            #endregion
        }
        

        [Route("api/GetIsCurrentUserITorACH")]
        public bool GetIsCurrentUserITorACH()
        {
            string editorUserName = ATAMembershipUtility.Instance.ParseUsername(this.User.Identity.Name);
            return (DbUtil.GetIsCurrentUserAdmin(editorUserName, "ITSecurity") || DbUtil.GetIsCurrentUserAdmin(editorUserName, "ACH")); 
        }

        [Route("api/GetCurrentUser")]
        public ATAMembershipUser GetCurrentUser()
        {
            // set up domain context 
            string editorUserName = ATAMembershipUtility.Instance.ParseUsername(this.User.Identity.Name);
            // find a user
            ATAMembershipProvider provider = ATAMembershipUtility.Instance.GetATAMembershipProvider();
            ATAMembershipUser user = (ATAMembershipUser)provider.GetUser(editorUserName, true);
            return user;
        }
        [Route("api/getgrouplist")]
        public string GetGroupList(int type = 1, int? groupid = null, string term = null)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
            {
                StringBuilder sql = new StringBuilder(); 
                using (SqlCommand cmd = new SqlCommand("p_Group_GetList", con))
                {
                    con.Open();
                    if (!string.IsNullOrEmpty(term))
                    {
                        SqlParameter parm = new SqlParameter("term", term);
                        cmd.Parameters.Add(parm);
                    }
                    else
                    if (groupid > 0)
                    {
                        SqlParameter[] spm = new SqlParameter[2];
                        spm[0] = new SqlParameter("type", null);
                        spm[1] = new SqlParameter("groupid", groupid);
                        cmd.Parameters.AddRange(spm);
                    }
                    else
                    if (type != 4 && type >0)
                    { 
                        SqlParameter parm = new SqlParameter("type", type);
                        cmd.Parameters.Add(parm);
                    }
                    else
                    {
                        SqlParameter[] spm = new SqlParameter[2];
                        spm[0] = new SqlParameter("type", type);
                        spm[1] = new SqlParameter("UserName", GetUserName()); 
                        cmd.Parameters.AddRange(spm);
                    }
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                    Dictionary<string, object> row;
                    foreach (DataRow dr in dt.Rows)
                    {
                        row = new Dictionary<string, object>();
                        foreach (DataColumn col in dt.Columns)
                        {
                            row.Add(col.ColumnName, dr[col]);
                        }
                        rows.Add(row);
                    }
                    return serializer.Serialize(rows);
                }
            }
        } 
        [Route("api/getgroupuserlist")]
        public string GetGroupUserList(int type = 1, int? groupid = null, int? userid = null)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
            {
                StringBuilder sql = new StringBuilder();

                using (SqlCommand cmd = new SqlCommand("p_UserGroup_GetList", con))
                {
                    con.Open();
                    if (groupid > 0)
                    {
                        SqlParameter[] spm = new SqlParameter[2];
                        spm[0] = new SqlParameter("type", type); //1 - a4astaff, 2=non a4astaff, 0 all
                        spm[1] = new SqlParameter("groupid", groupid);
                        cmd.Parameters.AddRange(spm); 
                    }
                    else
                    if(userid.Value > 0)
                    {  
                        SqlParameter[] spm = new SqlParameter[1];
                        spm[0] = new SqlParameter("userid", userid); //Userid was given
                        cmd.Parameters.AddRange(spm);
                    }
                    else
                    {
                        SqlParameter[] spm = new SqlParameter[1];
                        spm[0] = new SqlParameter("type", type); //1 - a4astaff, 2=non a4astaff, 0 all 
                        cmd.Parameters.AddRange(spm);
                    }
                    
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                    Dictionary<string, object> row;
                    foreach (DataRow dr in dt.Rows)
                    {
                        row = new Dictionary<string, object>();
                        foreach (DataColumn col in dt.Columns)
                        {
                            row.Add(col.ColumnName, dr[col]);
                        }
                        rows.Add(row);
                    }
                    return serializer.Serialize(rows);
                }
            }
        } 
        [Route("api/geta4ausergroups")]
        public List<UserGroupJsonModel> GetA4AUserGroups(int GroupId, bool IsA4AStaff)
        {
            var allA4AUserGroups = UserGroupJsonModel.GetA4AStaffGroups(GroupId,IsA4AStaff);
            return allA4AUserGroups;
        }

        [Route("api/isSecurityGroupCount")]
        public int GetUserSecurityGroupCount(int uid)
        {
            int count = -1;

            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
            {
                StringBuilder sql = new StringBuilder();

                using (SqlCommand cmd = new SqlCommand("P_UserGroup_GetUserSecurityGroupCount", con))
                {
                    con.Open();
                    if (uid > 0)
                    {
                        SqlParameter[] spm = new SqlParameter[1];
                        spm[0] = new SqlParameter("userid", uid); //Userid was given
                        cmd.Parameters.AddRange(spm);
                    }

                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    int.TryParse(dt.Rows[0][0].ToString(), out count);
                    return count;
                }

            }
        }


        [Route("api/group")]
        public ATAGroup Get(int gid)
        {
            ATAGroup gp = new ATAGroup(gid);
            return gp;

        }
        [Route("api/isNewGroup")]
        public bool GetIfNewGroup(ATAGroup grp)
        {
            return !(DbUtil.CheckifGroupAlreadyExists(grp) > 1);
        }


        [Route("api/RemoveGroup")]
        //[AcceptVerbs("POST","GET")]
        public HttpResponseMessage RemoveGroup(int GroupId)
        {
            bool sqlDelete = false;
            bool lyrDelete = false;
            ATAGroup grp = new ATAGroup(GroupId);
            //First see if you can delete from the SQL (users/dependecies etc)
            sqlDelete = grp.Delete();
            if (sqlDelete)
            {
                lyrDelete = this.LyrisManager2.DeleteList(grp.LyrisListName);
            }
            if (sqlDelete && lyrDelete) //If group was successfully deleted
            {
                Transactions.setTransaction(grp.GroupId, DBUtilAPIController.CurrentUser().UserId, "GroupName", grp.GroupName, "LyrisListName", grp.LyrisShortDescription, TransactionType.GroupDeleted);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            else
            {
                var message = string.Format("Error in deleting the Group. Please make sure you removed all users/dependencies before deleting. Contact IT if problem persists!");
                HttpError err = new HttpError(message);
                return Request.CreateResponse(HttpStatusCode.NotFound, err);
            } 
        }
        //POST api/student
        [Route("api/group")]
        public HttpResponseMessage Post(ATAGroup Group)
        {
            var message = "" ;
            try
            {
                int li1 = 0;
                int currentUID = DBUtilAPIController.CurrentUser().UserId;
                Group.IsNewGroup = GetIfNewGroup(Group); //This was the original and working
                int.TryParse(Group.Liaison1, out li1);
                LookupUser Liaison1 = new LookupUser(li1);

                Group.Liaison1UserId = li1;
                Group.Liaison1 = Liaison1.JobTitle; 
                Group.ModifiedDate = DateTime.Now;
                
                Group.DepartmentId = Group.DepartmentId;
                if (string.IsNullOrEmpty(Group.GroupSiteUrl))
                    Group.GroupSiteUrl = string.Empty;
                //For below two properties update in the stored proc 4/11/17 
                Group.IsCommittee = (Group.GroupTypeId == 1 || Group.GroupTypeId == 2 || Group.GroupTypeId == 7) ? true : false;
                Group.IsSecurityGroup = (Group.GroupTypeId == 1 || Group.GroupTypeId == 2 || Group.GroupTypeId == 7 || Group.GroupTypeId == 3 || Group.GroupTypeId == 5) ? true : false;
                //If Grouptype is 1,2 or 7 and not a new group then retrieve the isChildGroup value.
                if(Group.IsCommittee && !Group.IsNewGroup)
                {
                    ATAGroup g = new ATAGroup(Group.GroupId);
                    Group.IsChildGroup = g.IsChildGroup;
                }

                Group.LyrisListName = !string.IsNullOrEmpty(Group.LyrisListName) ? Group.LyrisListName : Group.LyrisListName.Replace(" ", "");
                Group.LyrisListType = LyrisManager2.MarketingTypeString;
                Group.AppliesToSite = AppliesToSite.Members; //Updated as per req from John

                if (Group.ParentGroupId < 1)
                    Group.ParentGroupId = 0;//Since the this column requires value. So we can't insert null. DataObjectBase.NullIntRowId;
                if (Group.Save())
                {
                    //need to update or create a lyris list as appropriate
                    if (Group.IsNewGroup)//Group.IsNew
                    {
                        //Set Created Date
                        Group.CreatedDate = DateTime.Now; //Added to make sure all new groups have CreateDate

                        if (!SkipLyrisManager)
                        {
                            Group.LyrisListName = this.LyrisManager2.CreateList(Group.LyrisListType, Group.LyrisListName, Group.LyrisShortDescription, LyrisTopic);
                            LyrisMember.UpdateLyrisSend(Group.LyrisListName, Group.LyrisSendId);
                        }                        
                        Group.IsChildGroup = false;
                        Group.Save();
                        //Note: Add once Lyris starts working.
                        Group.updateList(Group.LyrisListName, Group.LyrisShortDescription, Group.IsNew);

                        if (Group.IsCommittee)
                        {
                            int cgid =  Group.createChildGroupsRetID(Group);
                            string LyrsSD = string.Format("{0}{1}", Group.LyrisShortDescription, "Info");
                            //Note: Save Transaction for Child Group
                            Transactions.setTransaction(cgid, currentUID, "GroupName", LyrsSD, "LyrisListName", Group.LyrisListName, TransactionType.GroupCreated);
                            //Note: Add current user as Manage Group Role so they can edit it in the future.
                            UserGroupJsonModel ugjm = new UserGroupJsonModel();
                            ugjm.GroupId = cgid;
                            ugjm.UserId = currentUID;
                            ugjm.ManageGroup = true;
                            ugjm.Save();
                            //AddA4AUsertoLyrisGroup(ugjm);   - 9/20/2018 - JD wanted the Manage Group not be in Lyris
                            Transactions.setUserGroupTransaction(cgid, currentUID, TransactionType.UserGroup_UserAdded, CommitteeRole.ManageGroup);
                         }
                    }
                    else
                    {  
                        //Update child group -- 12/6/2017 - something is missing..check what am I updating for the child group? 
                        if (!SkipLyrisManager)
                        {
                            this.LyrisManager2.UpdateExistingList(Group.LyrisListName, Group.LyrisShortDescription);
                            LyrisMember.UpdateLyrisSend(Group.LyrisListName, Group.LyrisSendId);
                        }
                    } 
                    ////Create the Lyris List Contacts in AD 
                    try
                    {
                        if (Group.IsNewGroup)
                        {
                            //Save Transaction for Group Creation
                            Transactions.setTransaction(Group.GroupId, currentUID, "GroupName", Group.LyrisShortDescription, "LyrisListName", Group.LyrisListName, TransactionType.GroupCreated);
                            //Add current user as "ManageGroup" Role so they can edit it in the future.
                            UserGroupJsonModel ugjm = new UserGroupJsonModel();
                            ugjm.GroupId = Group.GroupId;
                            ugjm.UserId = currentUID;
                            ugjm.ManageGroup = true;
                            ugjm.Save();
                            Transactions.setUserGroupTransaction(Group.GroupId, currentUID, TransactionType.UserGroup_UserAdded, CommitteeRole.ManageGroup);

                            //AddA4AUsertoLyrisGroup(ugjm); removed by NA on 9/15/2018 - JD/SM want to not make Manage Contact a subscriber in the Lyris
                            
                            //If GAB is true add the group to the Exchange
                            if (Group.GAB)
                                DbUtil.CreateADContact(Group); 
                        }
                        else
                        {
                            //If GAB is false delete the group from AD - 11/15/17
                            if (Group.GAB)
                                DbUtil.CheckCreateADContact(Group);
                            else
                                DbUtil.DeleteADContact(Group); 

                            Transactions.setTransaction(Group.GroupId, DBUtilAPIController.CurrentUser().UserId, "GroupName", Group.LyrisShortDescription, null, null, TransactionType.GroupModified);
                        }
                    }
                    catch (Exception ex)
                    {
                        message += string.Format("The Group has been created. But there was an error in Transaction Creation!" + ex.Message);
                        HttpError err = new HttpError(message);
                        return Request.CreateResponse(HttpStatusCode.NotFound, err);
                    }
                } 
                else
                {
                    message += string.Format("Could not create New Group.");
                    HttpError err = new HttpError(message);
                    return Request.CreateResponse(HttpStatusCode.NotFound, err); 
                } 
                var response = Request.CreateResponse(HttpStatusCode.Created, Group);
                return response;
            }
            catch(Exception ex)
            {
                 message = string.Format("Error in saving the group. Error: {0}",ex.Message);
                HttpError err = new HttpError(message);
                return Request.CreateResponse(HttpStatusCode.NotFound, err);
            }
        }
        [Route("api/GetGroupUserlineItem")] //for taskforce and other view loads
        public string GetGroupUserlineItem(int GroupId = 0, string Company = null, string RoleId = null, int UserId = 0)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
            {
                StringBuilder sql = new StringBuilder(); 
                using (SqlCommand cmd = new SqlCommand("p_UserGroup_LoadLI", con))
                {
                    con.Open();
                    if (GroupId > 0 && !string.IsNullOrEmpty(RoleId) && !string.IsNullOrEmpty(Company))
                    {
                        SqlParameter[] spm = new SqlParameter[3];
                        spm[0] = new SqlParameter("GroupId", GroupId);
                        spm[1] = new SqlParameter("RoleId", SqlDbType.NVarChar);
                        spm[1].Value = RoleId ?? (object)DBNull.Value; 
                        spm[2] = new SqlParameter("Company", SqlDbType.NVarChar);
                        spm[2].Value = Company ?? (object)DBNull.Value;
                        cmd.Parameters.AddRange(spm);
                    } else
                    if (GroupId > 0 && !string.IsNullOrEmpty(RoleId))
                    {
                        //Commented by NA - JD/SM wanted Committee& councils to work independently from 
                        //Get ChildGroup Informational Users if it is a Committee Group and Informational Role. 
                        //if (RoleId.Contains(((int)CommitteeRole.Informational).ToString()))
                        //{
                        //    ATAGroup grp = new ATAGroup(GroupId);
                        //    if(grp.IsCommittee && !grp.IsChildGroup)
                        //    {
                        //        ATAGroup childGroup = grp.GetCommitteeChildGroup();
                        //        GroupId = childGroup.GroupId; //Get ChildGroup's name if it is a Committee Group and not a committee Child
                        //    }
                        //}
                        SqlParameter[] spm = new SqlParameter[2];
                        spm[0] = new SqlParameter("GroupId", GroupId);
                        spm[1] = new SqlParameter("RoleId", SqlDbType.NVarChar);
                        spm[1].Value = RoleId ?? (object)DBNull.Value; 
                        cmd.Parameters.AddRange(spm);
                    }else
                    if (UserId > 0 )
                    {
                        SqlParameter[] spm = new SqlParameter[3];
                        spm[0] = new SqlParameter("UserId", UserId);
                        spm[1] = new SqlParameter("CurrentUserId", DBUtilAPIController.CurrentUser().UserId);
                        spm[2] = new SqlParameter("IsCurrentUserAdmin",GetIsCurrentUserAdmin());
                        cmd.Parameters.AddRange(spm);
                    }
                    else
                    if (GroupId > 0)
                    {
                        SqlParameter[] spm = new SqlParameter[1];
                        spm[0] = new SqlParameter("GroupId", GroupId);
                        cmd.Parameters.AddRange(spm);
                    }
                    
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                    Dictionary<string, object> row;
                    foreach (DataRow dr in dt.Rows)
                    {
                        row = new Dictionary<string, object>();
                        foreach (DataColumn col in dt.Columns)
                        {
                            row.Add(col.ColumnName, dr[col]);
                        }
                        rows.Add(row);
                    }
                    return serializer.Serialize(rows);
                }
            }
        }

        protected void AddUsertoGroups(UserGroupJsonModel ug, CommitteeRole[] roles) 
        {
            Transactions.setUserGroupTransaction(ug, TransactionType.UserGroup_UserRemoved, DBUtilAPIController.CurrentUser().UserId, true);
            UserGroupJsonModel.DeleteGroupA4AUser(ug.GroupId, ug.UserId);
            ug.Save();
            bool subscriberRole = false;

            foreach (CommitteeRole role in roles)
            {
                if ((bool)ug.GetType().GetProperty(role.ToString()).GetValue(ug, null))
                {
                    AddA4AUsertoLyrisGroup(ug);
                    subscriberRole = true;
                    break;
                }
            }
            if (!subscriberRole) //The user doesn't have EmailAdmin, BounceAdmin or Subscriber turned off. Remove the user
            { 
                RemoveUserfromLyrisGroup(ug); //removes the user from Lyris - Added 9/20/2018
            }
            Transactions.setUserGroupTransaction(ug, TransactionType.UserGroup_UserAdded, DBUtilAPIController.CurrentUser().UserId, true);           
        }
        [Route("api/GetGroupEmailOnlyMembers")]
        public string GetGroupMembers(int GroupId)
        {
            ATAGroup grp = new ATAGroup(GroupId);
            string[] users = grp.GetGroupEmailOnlyMembers(GroupId);
            StringBuilder sb = new StringBuilder();

            foreach (string u in users)
            {
                sb.Append(u);
                sb.Append(";");
            }
            return sb.ToString();
        }
        [Route("api/UserGroup")]
        public HttpResponseMessage Post(List<UserGroupJsonModel> UsersinGroup)
        {           
            return SavePost(UsersinGroup, false,false, 0);
        }

        [Route("api/UserGroupGGA")]
        public HttpResponseMessage SavePostGGA(List<UserGroupJsonModel> UsersinGroup)
        {
            return SavePost(UsersinGroup,false, true, 0);
        } 
        /*******************************************************************************/
        /*  The below code works great when its single user - group being saved. For a bulk save go to
        /*******************************************************************************/
        [Route("api/A4AUserGroup")]
        public HttpResponseMessage SavePost(List<UserGroupJsonModel> UsersinGroup,bool IsStaff,bool IsGGA,int groupType = 0)
        {
            try
             {
                foreach (UserGroupJsonModel ug in UsersinGroup)
                {
                    ATAGroup grp = new ATAGroup(ug.GroupId);
                    if (ug.GroupId < 1)
                    {
                        var message = string.Format("No GroupId found. So cannot add user!");
                        HttpError err = new HttpError(message);
                        return Request.CreateResponse(HttpStatusCode.NotFound, err); 
                    }
                    if (IsStaff)
                    {
                        AddUsertoGroups(ug, new CommitteeRole[] { CommitteeRole.EmailAdmin , CommitteeRole.BounceReports, CommitteeRole.StaffSubscribe }); 
                        //Check if committee and add a4a staff to child  --Removed 9/20/2018 JD/SM wanted the sync between committee group and info group and wanted them to function as independent groups.
                        //if (grp.IsCommittee)
                        //{
                        //    ATAGroup childGroup = grp.GetCommitteeChildGroup();
                        //    ug.GroupId = childGroup.GroupId;
                        //    AddUsertoGroups(ug, new CommitteeRole[] { CommitteeRole.EmailAdmin, CommitteeRole.BounceReports, CommitteeRole.StaffSubscribe });   //Add the staff to DB & Lyris for Committee information group too.
                        //} 
                    }
                    else
                    {   //Checks if the user has a role in the group already - of yes then return error
                        if (UserGroupJsonModel.UserExistsinGroup(ug) < 1)
                        {
                            if (ug.GroupId > 0 || !string.IsNullOrEmpty(ug.CompanyName))
                            {
                                if ((ug.Primary || ug.Alternate || ug.Chair || ug.ViceChair))
                                {
                                    if (ug.Primary)
                                    {
                                        Transactions.setUserGroupTransaction(ug, TransactionType.UserGroup_UserRemoved, CommitteeRole.Primary,DBUtilAPIController.CurrentUser().UserId);
                                        UserGroupJsonModel.DeleteGroupUserbyCompany(ug.CompanyName, ug.GroupId, 1);
                                    }
                                    else
                                    if (ug.Alternate && !IsGGA)
                                    {
                                        Transactions.setUserGroupTransaction(ug, TransactionType.UserGroup_UserRemoved, CommitteeRole.Alternate, DBUtilAPIController.CurrentUser().UserId);
                                        UserGroupJsonModel.DeleteGroupUserbyCompany(ug.CompanyName, ug.GroupId, 2);
                                    }
                                    else
                                    if (ug.Chair)
                                    {
                                        Transactions.setUserGroupTransaction(ug.GroupId, TransactionType.UserGroup_UserRemoved, CommitteeRole.Chair, DBUtilAPIController.CurrentUser().UserId);
                                        UserGroupJsonModel.DeleteGroupUserbyRole(ug.GroupId, 3);
                                    }
                                    else
                                    if (ug.ViceChair)
                                    {
                                        Transactions.setUserGroupTransaction(ug.GroupId, TransactionType.UserGroup_UserRemoved, CommitteeRole.ViceChair, DBUtilAPIController.CurrentUser().UserId);
                                        UserGroupJsonModel.DeleteGroupUserbyRole(ug.GroupId, 4);
                                    }
                                }

                                //Check if isCommittee and a parent group and CommitteeRoleID = 12 - Informational  -- Commented by NA 9/20/2018 - JD &SM wanted the council and committee lists to work independently
                                //if (grp.IsCommittee && !grp.IsChildGroup && ug.Informational)
                                //{
                                //    ATAGroup childGroup = grp.GetCommitteeChildGroup();
                                //    ug.GroupId = childGroup.GroupId;
                                //    AddUsertoGroups(ug, new CommitteeRole[] { CommitteeRole.Informational });   //Add the staff to DB & Lyris for Committee information group too.
                                //}
                                //else
                                //{
                                    ug.Save();
                                //}
                                Transactions.setUserGroupTransaction(ug, TransactionType.UserGroup_UserAdded, DBUtilAPIController.CurrentUser().UserId,true);
                                AddUsertoLyrisGroup(ug);
                            }
                            else
                            {
                                var message = string.Format("GroupId or CompanyName was not supplied");
                                HttpError err = new HttpError(message);
                                return Request.CreateResponse(HttpStatusCode.NotFound, err);
                            }
                        }
                        else
                        {
                            var message = string.Format("User {0} already exists in the given group/parent group/child group. Remove him and save again. ", DbUtil.GetUserNamebyUserId(ug.UserId));
                            HttpError err = new HttpError(message);
                            return Request.CreateResponse(HttpStatusCode.NotFound, err);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var message = string.Format("Error in saving Group User. Error: {0}", ex.Message);
                HttpError err = new HttpError(message);
                return Request.CreateResponse(HttpStatusCode.NotAcceptable, err);
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        //Save A4A Staff 
        public bool AddA4AUsertoLyrisGroup(UserGroupJsonModel ug)
        {
            bool ret = false;
            try
            {   //Get the User and Group details first.
                DataTable dt = GetUserGroupDetails(ug);
                DataRow dr = dt.Rows[0];
                string name = dr["Name"].ToString();
                string email = dr["Email"].ToString();
                string groupName = dr["GroupName"].ToString();

                LyrisMember lyruser = new LyrisMember(email);
                lyruser.ReceiveAdminEmail = ug.BounceReports;
                lyruser.IsAdmin = ug.EmailAdmin;
                lyruser.NoMail = !ug.StaffSubscribe; //ug.Participant; It is staffsubscribe for a4a users , participant is for other contcats - chaning this because subscribe want working

                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(groupName))
                {
                    //Check if the user already exists and save if it doesn't 
                    List<string> previousSelectedListNames = LyrisMember.GetListNamesByMemberEmail(email);
                    if (!previousSelectedListNames.Contains(groupName))
                    {
                        LyrisMember.AddMemberToList(email, name, groupName);
                        LyrisMember.UpdateListAdmin(lyruser.EmailAddress, groupName, lyruser.IsAdmin, lyruser.ReceiveAdminEmail, lyruser.NoMail);
                    }
                    else
                    {
                        LyrisMember.UpdateListAdmin(lyruser.EmailAddress, groupName, lyruser.IsAdmin, lyruser.ReceiveAdminEmail, lyruser.NoMail);
                    } 
                }
                ret = true;
            }
            catch (Exception ex)
            {
                throw new Exception("In AddUsertoLyrisGroup: " + ex.Message + ex.Message);
            }

            return ret;
        }

        public bool AddUsertoLyrisGroup(UserGroupJsonModel ug)
        {
            try
            {   //Get the User and Group details first.
                DataTable dt = GetUserGroupDetails(ug);
                DataRow dr = dt.Rows[0];
                string name = dr["Name"].ToString();
                string email = dr["Email"].ToString();
                string groupName = dr["GroupName"].ToString();
 

                bool ret = AddUsertoLyrisDB(name, email,groupName);
                return ret;
            }
            catch(Exception ex)
            {
                throw new Exception("In AddUsertoLyrisGroup: "+ ex.Message  + ex.Message);
            } 
        } 
         
        public bool AddUsertoLyrisDB(string Name, string Email, string GroupName)
        {
            try
            {
                string name = Name;
                string email = Email;
                string groupName = GroupName;

                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(groupName))
                {
                    //Check if the user already exisits and save if it doesn't 
                    List<string> previousSelectedListNames = LyrisMember.GetListNamesByMemberEmail(email);
                    if (!previousSelectedListNames.Contains(groupName))
                        LyrisMember.AddMemberToList(email, name, groupName);
                }
                return true;
            }
            catch(Exception ex)
            {
                return false;
                throw ex;
            }
        }

        public bool RemoveUserfromLyrisGroup(UserGroupJsonModel ug)
        {
            //Get the User and Group details first.
            DataTable dt = GetUserGroupDetails(ug);
            DataRow dr = dt.Rows[0];
            string name = dr["Name"].ToString();
            string email = dr["Email"].ToString();
            string groupName = dr["GroupName"].ToString();

            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(groupName))
            {
                LyrisMember.DeleteMemberFromList(email, groupName);
            }
            return true;
        }
        public bool RemoveUserfromLyrisGroupbyRoleId(DataTable dt)//int GroupId, int RoleId)
        {
            try
            { 
                foreach (DataRow dr in dt.Rows)
                {
                    string name = dr["Name"].ToString();
                    string email = dr["Email"].ToString();
                    string groupName = dr["GroupName"].ToString();
                    if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(groupName))
                    {
                        LyrisMember.DeleteMemberFromList(email, groupName);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
                //throw ex;
            }

        }
        public bool RemoveUserfromLyrisGroupbyCompanyName(DataTable dt)//string CompanyName, int GroupId, int RoleId)
        {
            try
            {
               
                DataRow dr = dt.Rows[0]; // Assumption is that there is only one user for that group/role/companyname
                string name = dr["Name"].ToString();
                string email = dr["Email"].ToString();
                string groupName = dr["GroupName"].ToString();
                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(groupName))
                {
                    LyrisMember.DeleteMemberFromList(email, groupName);
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public DataTable GetUserGroupDetailsbyUserId(int GroupId, int UserId)
        {
            //Check if already exisiting
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
            {
                StringBuilder sql = new StringBuilder();
                using (SqlCommand cmd = new SqlCommand("p_UserDataforLyrisGroupbyUser", con))
                {
                    con.Open();
                    SqlParameter[] spm = new SqlParameter[2];
                    spm[0] = new SqlParameter("Groupid", GroupId);
                    spm[1] = new SqlParameter("UserId", UserId);
                    cmd.Parameters.AddRange(spm);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            return dt;
        }

        public DataTable GetUserGroupDetailsbyRoleId(int GroupId, int RoleId)
        {
            //Check if already exisiting
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
            {
                StringBuilder sql = new StringBuilder();
                using (SqlCommand cmd = new SqlCommand("p_UserDataforLyrisGroupbyRole", con))
                {
                    con.Open();
                    SqlParameter[] spm = new SqlParameter[2]; 
                    spm[0] = new SqlParameter("Groupid", GroupId);
                    spm[1] = new SqlParameter("RoleId", RoleId);
                    cmd.Parameters.AddRange(spm);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            return dt;
        }
        public DataTable GetUserGroupDetailsbyCompanyName(String CompanyName, int GroupId, int RoleId)
        { 
            //Check if already exisiting
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
            {
                StringBuilder sql = new StringBuilder();
                using (SqlCommand cmd = new SqlCommand("p_UserDataforLyrisGroupbyCompanyName", con))
                {
                    con.Open();
                    SqlParameter[] spm = new SqlParameter[3];
                    spm[0] = new SqlParameter("CompanyName", CompanyName);
                    spm[1] = new SqlParameter("Groupid", GroupId);
                    spm[2] = new SqlParameter("RoleId", RoleId); 
                    cmd.Parameters.AddRange(spm);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            return dt;
        }
        public DataTable GetUserGroupDetails(UserGroupJsonModel ug)
        {
            //Check if already exisiting
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
            {
                StringBuilder sql = new StringBuilder();
                using (SqlCommand cmd = new SqlCommand("p_UserDataforLyrisGroup", con))
                {
                    con.Open();
                    SqlParameter[] spm = new SqlParameter[2];
                    spm[0] = new SqlParameter("groupid", ug.GroupId);
                    spm[1] = new SqlParameter("userid", ug.UserId);

                    cmd.Parameters.AddRange(spm);

                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            return dt;

        }
        //Get User Group Details based on Company
        //public DataTable GetUserGroupDetails(UserGroupJsonModel ug)
        //{
        //    //Check if already exisiting
        //    DataTable dt = new DataTable();
        //    using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
        //    {
        //        StringBuilder sql = new StringBuilder();
        //        using (SqlCommand cmd = new SqlCommand("p_UserDataforLyrisGroup", con))
        //        {
        //            con.Open();
        //            SqlParameter[] spm = new SqlParameter[2];
        //            spm[0] = new SqlParameter("groupid", ug.GroupId);
        //            spm[1] = new SqlParameter("userid", ug.UserId);

        //            cmd.Parameters.AddRange(spm);

        //            cmd.CommandType = CommandType.StoredProcedure;
        //            SqlDataAdapter da = new SqlDataAdapter(cmd);
        //            da.Fill(dt);
        //        }
        //    }
        //    return dt;

        //}

        [Route("api/RemoveGroupUser")]
        public HttpResponseMessage RemoveGroupUser(List<UserGroupJsonModel> UsersinGroup)
        {
           
            bool result = true;
            foreach (UserGroupJsonModel ug in UsersinGroup)
            {
                //get the details to use it later to remove the deleted users from Lyris
                DataTable dt = GetUserGroupDetailsbyUserId(ug.GroupId, ug.UserId);


                result = UserGroupJsonModel.DeleteGroupsUser(ug);                
                if (!result)
                    break;
                else
                {
                    RemoveUserfromLyrisGroup(ug); //removes the user from Lyris
                    Transactions.setTransaction(dt, DBUtilAPIController.CurrentUser().UserId, TransactionType.UserGroup_UserRemoved); //Added 7/28/17
                    //Transactions.setUserGroupTransaction(ug, TransactionType.UserGroup_UserRemoved, DBUtilAPIController.CurrentUser().UserId, true);
                }
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            if (!result)
            {
                response = Request.CreateResponse(HttpStatusCode.PreconditionFailed);
            }
            return response;
        }

        [Route("api/RemoveGrpUsrbyRole")]
        //Deletes all users for the giveb groupid and roleid
        public HttpResponseMessage RemoveGroupUserbyRoleId(int GroupId, int RoleId)
        {
            //get the details to use it later to remove the deleted users from Lyris
            DataTable dt = GetUserGroupDetailsbyRoleId(GroupId, RoleId);

            bool result = UserGroupJsonModel.DeleteGroupUserbyRole(GroupId, RoleId);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            if (!result)
            {
                var message = string.Format("Couldn't delete the user");
                HttpError err = new HttpError(message);
                return Request.CreateResponse(HttpStatusCode.NotFound, err);
            }
            else
            {
                RemoveUserfromLyrisGroupbyRoleId(dt);
                Transactions.setTransaction(dt, DBUtilAPIController.CurrentUser().UserId, TransactionType.UserGroup_UserRemoved); //Added 7/28/17
                //Transactions.setUserGroupTransaction(GroupId, TransactionType.UserGroup_UserRemoved, (CommitteeRole)RoleId, DBUtilAPIController.CurrentUser().UserId);
            }
            
            return response;
        }
       
        [Route("api/RemoveGrpUsrbyComp")]
        public HttpResponseMessage RemoveGroupUserbyCompany(string CompanyName, int GroupId, int RoleId)
        {
            //UserGroupJsonModel.DeleteGroupUserbyCompany(CompanyName, GroupId, RoleId); //for some reason I had repetative code
            //Get the details of the user being removed so you can remove them from Lyris
            DataTable dt = GetUserGroupDetailsbyCompanyName(CompanyName, GroupId, RoleId);
            //Note the transaction before the records are deleted
            bool result = UserGroupJsonModel.DeleteGroupUserbyCompany(CompanyName, GroupId, RoleId);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            if (!result)
            {

                var message = string.Format("Couldn't delete the user");
                HttpError err = new HttpError(message);
                return Request.CreateResponse(HttpStatusCode.NotFound, err);
            }
            else
            {  //Delete was successful so save the 
                RemoveUserfromLyrisGroupbyCompanyName(dt); 
                Transactions.setTransaction(dt, DBUtilAPIController.CurrentUser().UserId, TransactionType.UserGroup_UserRemoved); //Added 7/28/17
                //Transactions.setUserGroupTransaction(GroupId, TransactionType.UserGroup_UserRemoved, (CommitteeRole)RoleId, DBUtilAPIController.CurrentUser().UserId);
            }
            return response;
        }
        [Route("api/getTransactions")]
        public string GetTransactionsList(int? groupid = null, int? userid = null)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
            {
                StringBuilder sql = new StringBuilder();

                using (SqlCommand cmd = new SqlCommand("p_Transaction_GetList", con))
                {
                    con.Open();
                    if (groupid > 0)
                    {
                        SqlParameter[] spm = new SqlParameter[1]; 
                        spm[0] = new SqlParameter("groupid", groupid);
                        cmd.Parameters.AddRange(spm);
                    }
                    else
                    if (userid.Value > 0)
                    {
                        SqlParameter[] spm = new SqlParameter[1];
                        spm[0] = new SqlParameter("userid", userid); //Userid was given
                        cmd.Parameters.AddRange(spm);
                    }
                   
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                    Dictionary<string, object> row;
                    foreach (DataRow dr in dt.Rows)
                    {
                        row = new Dictionary<string, object>();
                        foreach (DataColumn col in dt.Columns)
                        {
                            row.Add(col.ColumnName, dr[col]);
                        }
                        rows.Add(row);
                    }
                    return serializer.Serialize(rows);
                }
            }
        }

    }
}

