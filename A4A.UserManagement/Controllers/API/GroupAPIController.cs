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
using System.Configuration;
using System.Linq;
using ATA.CodeLibrary;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Web.Security;

namespace A4A.UM.Controllers
{
    public class GroupAPIController : ApiController
    {
        private const string LyrisTopic = "main";
        protected bool SkipLyrisManager { get { return true; } }
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
        public SortedDictionary<string, int> GetGroupsNamesWithId()
        {
            return ATAGroup.GetAllGroupNamesWithId();
        }
        [Route("api/getGroupType")]
        public string GetGroupType()
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
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

                }
            }

            return serializer.Serialize(rows);
        }
        //[Route("api/getallgroup")]  Commenting because not being used. the one in DBUITil is being used. If no errors Delete
        //public List<ATAGroup> GetAllGroups()
        //{
        //    var allGroups = ATAGroup.GetAllNonChildGroups(ATA.ObjectModel.AppliesToSite.All);
        //    return allGroups;
        //} 
        public string GetUserName()
        {
            string editorUserName = ATAMembershipUtility.Instance.ParseUsername("teqmavens");
            return editorUserName;
        }
        [Route("api/GetIsCurrentUserAdmin")]
        public bool GetIsCurrentUserAdmin()
        {
            string editorUserName = ATAMembershipUtility.Instance.ParseUsername("teqmavens");
            return DbUtil.GetIsCurrentUserAdmin(editorUserName, "UMReports");
        }

        [Route("api/GetIsCurrentGroupUserAdmin")]
        public bool GetIsCurrentGroupUserAdmin(int GroupId)
        {
            string editorUserName = ATAMembershipUtility.Instance.ParseUsername("teqmavens");
            using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("p_isGroupAdmin", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Username", editorUserName);
                    cmd.Parameters.AddWithValue("@GroupId", GroupId);
                    var reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        return true;
                    }
                }
                con.Close();
                con.Dispose();
            }
            return false;
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
            string editorUserName = ATAMembershipUtility.Instance.ParseUsername("teqmavens");
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

        [HttpGet]
        [Route("api/HasFuelsCompanyType")] //Note this method didnot work until I added Get at the begining of the method or add the http verb
        public bool HasFuelsCompanyType(int? CompanyId = null, string CompanyName = null)
        {
            if (CompanyId.Value > 0)
            {
                Company cmp = new Company(CompanyId.Value);
                return cmp.FuelsCompanyTypeId > 0;
            }
            return false;
        }


        [Route("api/GetIsCurrentUserITorECON")]
        public bool GetIsCurrentUserITorECON()
        {
            string editorUserName = ATAMembershipUtility.Instance.ParseUsername("teqmavens");
            return ((DbUtil.GetIsCurrentUserAdmin(editorUserName, "ITSecurity") || DbUtil.GetIsCurrentUserAdmin(editorUserName, "ECON")));  // && DbUtil.IsFuelsNonpublicUser()

        }


        [Route("api/GetIsCurrentUserITorACH")]
        public bool GetIsCurrentUserITorACH()
        {
            string editorUserName = ATAMembershipUtility.Instance.ParseUsername("teqmavens");
            return (DbUtil.GetIsCurrentUserAdmin(editorUserName, "ITSecurity") || DbUtil.GetIsCurrentUserAdmin(editorUserName, "ACH"));
        }


        [Route("api/GetCurrentUser")]
        public ATAMembershipUser GetCurrentUser()
        {
            // set up domain context 
            string editorUserName = ATAMembershipUtility.Instance.ParseUsername("teqmavens");
            // find a user
            ATAMembershipProvider provider = ATAMembershipUtility.Instance.GetATAMembershipProvider();
            ATAMembershipUser user = (ATAMembershipUser)provider.GetUser(editorUserName, true);
            return user;
        }
        [Route("api/getgrouplist")]
        public string GetGroupList(int type = 1, int? groupid = null, string term = null, string username = null)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            DataTable dt = new DataTable();
            string _username = username == "null" ? GetUserName() : username;

            if (type == 1)
            {
                _username = GetUserName();
            }
            using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
            {
                StringBuilder sql = new StringBuilder();
                using (SqlCommand cmd = new SqlCommand("p_Group_GetList", con))
                {
                    cmd.CommandTimeout = 120;
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
                    if (type != 4 && type > 0)
                    {
                        SqlParameter[] parm = new SqlParameter[2];
                        parm[0] = new SqlParameter("type", type);
                        parm[1] = new SqlParameter("UserName", _username);
                        cmd.Parameters.AddRange(parm);
                    }
                    else
                    {
                        SqlParameter[] spm = new SqlParameter[2];
                        spm[0] = new SqlParameter("type", type);
                        spm[1] = new SqlParameter("UserName", _username);
                        cmd.Parameters.AddRange(spm);
                    }
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

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
                }
            }
            return serializer.Serialize(rows);
        }
        [Route("api/getgroupuserlist")]
        public string GetGroupUserList(int type = 1, int? groupid = null, int? userid = null)
        {
            DataTable dt = new DataTable();
            StringBuilder sql = new StringBuilder();
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();

            using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
            {
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
                    if (userid.Value > 0)
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
                }
            }
            return serializer.Serialize(rows);
        }
        [Route("api/geta4ausergroups")]
        public List<UserGroupJsonModel> GetA4AUserGroups(int GroupId, bool IsA4AStaff)
        {
            var allA4AUserGroups = UserGroupJsonModel.GetA4AStaffGroups(GroupId, IsA4AStaff);
            return allA4AUserGroups;
        }
        [AcceptVerbs("POST")]
        [HttpPost]
        [Route("api/deletea4ausergroups")]
        public bool DeleteA4AUserGroups(int GroupId, int UserId, bool IsA4AStaff = false)
        {
            bool success = true;
            bool isSubscriber = false;
            try
            {
                List<UserGroupJsonModel> allA4AUserGroups = UserGroupJsonModel.GetA4AStaffGroups(GroupId, IsA4AStaff);

                UserGroupJsonModel ug = allA4AUserGroups.Find(x => x.UserId == UserId);

                ATAGroup grp = new ATAGroup(GroupId);
                RemoveA4AStafffromGroup(ug); //Added this code instead of the below repeat code.
                #region repeat code 
                //if (ug.StaffSubscribe)
                //{
                //    isSubscriber = true;
                //    success = UserGroupJsonModel.DeleteGroupA4AUser(ug.GroupId, ug.UserId, (int)CommitteeRole.StaffSubscribe);
                //    Transactions.setTransaction(ug.GroupId, DBUtilAPIController.CurrentUser().UserId, "UserId", ug.UserId.ToString(), "CommitteeRoleID", ((int)CommitteeRole.StaffSubscribe).ToString(), TransactionType.UserGroup_UserRemoved);
                //}
                //if (ug.EmailAdmin)
                //{
                //    isSubscriber = true;
                //    success = UserGroupJsonModel.DeleteGroupA4AUser(ug.GroupId, ug.UserId, (int)CommitteeRole.EmailAdmin);
                //    Transactions.setTransaction(ug.GroupId, DBUtilAPIController.CurrentUser().UserId, "UserId", ug.UserId.ToString(), "CommitteeRoleID", ((int)CommitteeRole.EmailAdmin).ToString(), TransactionType.UserGroup_UserRemoved);
                //}
                //if (ug.BounceReports)
                //{
                //    isSubscriber = true;
                //    success = UserGroupJsonModel.DeleteGroupA4AUser(ug.GroupId, ug.UserId, (int)CommitteeRole.BounceReports);
                //    Transactions.setTransaction(ug.GroupId, DBUtilAPIController.CurrentUser().UserId, "UserId", ug.UserId.ToString(), "CommitteeRoleID", ((int)CommitteeRole.BounceReports).ToString(), TransactionType.UserGroup_UserRemoved);
                //}
                //if (ug.ManageGroup)
                //{
                //    success = UserGroupJsonModel.DeleteGroupA4AUser(ug.GroupId, ug.UserId, (int)CommitteeRole.ManageGroup);
                //    Transactions.setTransaction(ug.GroupId, DBUtilAPIController.CurrentUser().UserId, "UserId", ug.UserId.ToString(), "CommitteeRoleID", ((int)CommitteeRole.ManageGroup).ToString(), TransactionType.UserGroup_UserRemoved);
                //}

                //if (isSubscriber) //The user doesn't have EmailAdmin, BounceAdmin or Subscriber turned off. Remove the user
                //{
                //    success = RemoveUserfromLyrisGroup(ug); //removes the user from Lyris - Added 9/20/2018
                //}

                #endregion

                //Check if it a committee/council group for a4a staff delete in child group also
                //TODO: HardCoded the grouptypeID's need to clean it up and add enum s 

                if ((grp.GroupTypeId == 1 || grp.GroupTypeId == 2 || grp.GroupTypeId == 7 || grp.GroupTypeId == 8 || grp.GroupTypeId == 9 || grp.GroupTypeId == 10) && IsA4AStaff)
                {

                    ATAGroup childGroup = grp.GetCommitteeChildGroup();
                    ug.GroupId = childGroup.GroupId;
                    RemoveA4AStafffromGroup(ug);

                }
            }
            catch
            {
                success = false;
            }

            return success;
        }

        //Note: 09/09/2020 - This method has been added to remove repating the same code for parent and child
        public bool RemoveA4AStafffromGroup(UserGroupJsonModel ug)
        {
            bool success = true;
            bool isSubscriber = false;

            try
            {
                if (ug.StaffSubscribe)
                {
                    isSubscriber = true;
                    success = UserGroupJsonModel.DeleteGroupA4AUser(ug.GroupId, ug.UserId, (int)CommitteeRole.StaffSubscribe);
                    Transactions.setTransaction(ug.GroupId, DBUtilAPIController.CurrentUser().UserId, "UserId", ug.UserId.ToString(), "CommitteeRoleID", ((int)CommitteeRole.StaffSubscribe).ToString(), TransactionType.UserGroup_UserRemoved);
                }
                if (ug.EmailAdmin)
                {
                    isSubscriber = true;
                    success = UserGroupJsonModel.DeleteGroupA4AUser(ug.GroupId, ug.UserId, (int)CommitteeRole.EmailAdmin);
                    Transactions.setTransaction(ug.GroupId, DBUtilAPIController.CurrentUser().UserId, "UserId", ug.UserId.ToString(), "CommitteeRoleID", ((int)CommitteeRole.EmailAdmin).ToString(), TransactionType.UserGroup_UserRemoved);
                }
                if (ug.BounceReports)
                {
                    isSubscriber = true;
                    success = UserGroupJsonModel.DeleteGroupA4AUser(ug.GroupId, ug.UserId, (int)CommitteeRole.BounceReports);
                    Transactions.setTransaction(ug.GroupId, DBUtilAPIController.CurrentUser().UserId, "UserId", ug.UserId.ToString(), "CommitteeRoleID", ((int)CommitteeRole.BounceReports).ToString(), TransactionType.UserGroup_UserRemoved);
                }
                if (ug.ManageGroup)
                {
                    success = UserGroupJsonModel.DeleteGroupA4AUser(ug.GroupId, ug.UserId, (int)CommitteeRole.ManageGroup);
                    Transactions.setTransaction(ug.GroupId, DBUtilAPIController.CurrentUser().UserId, "UserId", ug.UserId.ToString(), "CommitteeRoleID", ((int)CommitteeRole.ManageGroup).ToString(), TransactionType.UserGroup_UserRemoved);
                }

                if (isSubscriber) //The user doesn't have EmailAdmin, BounceAdmin or Subscriber turned off. Remove the user
                {
                    success = RemoveUserfromLyrisGroup(ug); //removes the user from Lyris - Added 9/20/2018
                }
            }
            catch
            {
                success = false;
            }

            return success;
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
                }
            }
            return count;
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
        [Route("api/isNewEmail")]
        public bool GetIfNewEmailGroup(ATAGroup grp)
        {
            return !(DbUtil.CheckifGroupEmailAlreadyExists(grp) > 1);
        }

        [Route("api/RemoveGroup")]
        //[AcceptVerbs("POST","GET")]
        public HttpResponseMessage RemoveGroup(int GroupId)
        {
            bool sqlDelete = false, sqlDeleteChld = false;
            bool lyrDelete = false, lyrDeleteChld = false, IsCommittee = false;
            int success = 0;
            var message = "";
            int cnt = 0, chldcnt = 0;
            ATAGroup grp = new ATAGroup(GroupId);
            IsCommittee = grp.IsCommittee;

            //First see if you can delete from the SQL (users/dependecies etc) 
            cnt = grp.GetAllUserCount(GroupId);

            if (IsCommittee)
            {
                try
                {
                    ATAGroup childgrp = grp.GetCommitteeChildGroup();
                    sqlDeleteChld = childgrp.Delete();
                    if (sqlDeleteChld)
                    {
                        lyrDeleteChld = this.LyrisManager2.DeleteList(childgrp.LyrisListName);
                    }
                    if (sqlDeleteChld && lyrDeleteChld) //If group was successfully deleted
                    {
                        Transactions.setTransaction(childgrp.GroupId, DBUtilAPIController.CurrentUser().UserId, "GroupName", childgrp.GroupName, "LyrisListName", childgrp.LyrisShortDescription, TransactionType.GroupDeleted);
                        success++;
                    }
                }
                catch (Exception ex)
                {
                    //todo
                }
            }
            sqlDelete = grp.Delete();
            if (sqlDelete)
            {
                lyrDelete = this.LyrisManager2.DeleteList(grp.LyrisListName);

            }
            if (sqlDelete && lyrDelete) //If group was successfully deleted
            {
                Transactions.setTransaction(grp.GroupId, DBUtilAPIController.CurrentUser().UserId, "GroupName", grp.GroupName, "LyrisListName", grp.LyrisShortDescription, TransactionType.GroupDeleted);
                success++;
            }
            if ((success == 1 && !IsCommittee) || (success == 2 && IsCommittee))
                return Request.CreateResponse(HttpStatusCode.OK);
            else
            {
                if (IsCommittee)
                    message = string.Format("Error in deleting the Group. Maken sure the deleted group has atleast one and only one child group. Contact IT if problem persists!");
                else
                    message = string.Format("Error in deleting the Group. Contact IT if problem persists!");
                HttpError err = new HttpError(message);
                return Request.CreateResponse(HttpStatusCode.NotFound, err);
            }

        }

        [Route("api/group")]
        public HttpResponseMessage Post(ATAGroup Group, bool isEdit = false)
        {
            var message = "";
            bool removeadmin = true;
            try
            {
                int li1 = 0;
                int currentUID = DBUtilAPIController.CurrentUser().UserId;
                Group.IsNewGroup = GetIfNewGroup(Group); //This was the original and working
                if (!isEdit && !Group.IsNewGroup)
                {
                    message += string.Format("The Group with this given name already exists!");
                    HttpError err = new HttpError(message);
                    return Request.CreateResponse(HttpStatusCode.NotFound, err);
                }

                if (!isEdit && !GetIfNewEmailGroup(Group))
                {
                    message += string.Format("The Group with this given email already exists!");
                    HttpError err = new HttpError(message);
                    return Request.CreateResponse(HttpStatusCode.NotFound, err);
                }

                int.TryParse(Group.Liaison1, out li1);
                LookupUser Liaison1 = new LookupUser(li1);

                Group.Liaison1UserId = li1;
                Group.Liaison1 = Liaison1.JobTitle;
                Group.ModifiedDate = DateTime.Now;

                Group.DepartmentId = Group.DepartmentId;
                if (string.IsNullOrEmpty(Group.GroupSiteUrl))
                    Group.GroupSiteUrl = string.Empty;
                //For below two properties update in the stored proc 4/11/17 
                Group.IsCommittee = (Group.GroupTypeId == 1 || Group.GroupTypeId == 2 || Group.GroupTypeId == 7 || Group.GroupTypeId == 8 || Group.GroupTypeId == 9 || Group.GroupTypeId == 10) ? true : false;
                Group.IsSecurityGroup = (Group.GroupTypeId == 1 || Group.GroupTypeId == 2 || Group.GroupTypeId == 7 || Group.GroupTypeId == 8 || Group.GroupTypeId == 3 || Group.GroupTypeId == 5 || Group.GroupTypeId == 9 || Group.GroupTypeId == 10) ? true : false;
                //If Grouptype is 1,2 or 7 and not a new group then retrieve the isChildGroup value.
                if (Group.IsCommittee && !Group.IsNewGroup)
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
                            //Note: Add once Lyris starts working.
                            Group.updateList(Group.LyrisListName, Group.LyrisShortDescription, true); /*Updates columns*/
                            LyrisMember.UpdateLyrisSend(Group.LyrisListName, Group.LyrisSendId);
                            /************Remove admin@a4a.org or any email set as admin and is added to Lyris - config - keyword - SusQTechLyrisManagerEmail ********************/
                            removeadmin = LyrisMember.RemoveAdminfromList(Group.LyrisListName);
                        }
                        Group.IsChildGroup = false;
                        Group.Save();


                        try
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

                            if (Group.IsCommittee)
                            {
                                int cgid = Group.createChildGroupsRetID(Group);
                                string LyrsSD = string.Format("{0}{1}", Group.LyrisShortDescription, "Info");
                                //Note: Save Transaction for Child Group
                                Transactions.setTransaction(cgid, currentUID, "GroupName", LyrsSD, "LyrisListName", Group.LyrisListName, TransactionType.GroupCreated);
                                //Note: Add current user as Manage Group Role so they can edit it in the future.
                                UserGroupJsonModel ugjm1 = new UserGroupJsonModel();
                                ugjm1.GroupId = cgid;
                                ugjm1.UserId = currentUID;
                                ugjm1.ManageGroup = true;
                                ugjm1.Save();
                                AddA4AUsertoLyrisGroup(ugjm);
                                //AddA4AUsertoLyrisGroup(ugjm);   - 9/20/2018 - JD wanted the Manage Group not be in Lyris
                                Transactions.setUserGroupTransaction(cgid, currentUID, TransactionType.UserGroup_UserAdded, CommitteeRole.ManageGroup);
                            }
                        }
                        catch (Exception ex)
                        {
                            message += string.Format("The Group has been created. But there was an error in Transaction Creation!" + ex.Message);
                            HttpError err = new HttpError(message);
                            return Request.CreateResponse(HttpStatusCode.NotFound, err);
                        }
                    }
                    else //Group Updation since its not a new group
                    {
                        try
                        {
                            //If GAB is false delete the group from AD - 11/15/17
                            if (Group.GAB)
                                DbUtil.CheckCreateADContact(Group);
                            else
                                DbUtil.DeleteADContact(Group);

                            Transactions.setTransaction(Group.GroupId, DBUtilAPIController.CurrentUser().UserId, "GroupName", Group.LyrisShortDescription, null, null, TransactionType.GroupModified);
                            if (Group.IsCommittee)
                            {
                                ATAGroup childgrp = Group.GetCommitteeChildGroup();
                                //Update Child group properties for council and committees -- 1

                                childgrp.AppliesToSite = (AppliesToSite)Group.AppliesToSiteId;
                                childgrp.GroupSiteUrl = Group.GroupSiteUrl;
                                childgrp.LyrisSendId = Group.LyrisSendId;
                                childgrp.CreatedDate = DateTime.Now;
                                childgrp.ModifiedDate = DateTime.Now;
                                childgrp.Liaison1UserId = Group.Liaison1UserId;
                                childgrp.Liaison1 = Group.Liaison1;
                                childgrp.DivisionId = Group.DivisionId;
                                childgrp.DepartmentId = Group.DepartmentId;
                                childgrp.GAB = Group.GAB;
                                childgrp.Mission = Group.Mission;
                                childgrp.Save();

                                //Update GAB for child group
                                if (Group.GAB)
                                    DbUtil.CheckCreateADContact(childgrp);
                                else
                                    DbUtil.DeleteADContact(childgrp);

                                Transactions.setTransaction(childgrp.GroupId, DBUtilAPIController.CurrentUser().UserId, "GroupName", childgrp.LyrisShortDescription, null, null, TransactionType.GroupModified);
                                //Update child Lyris group -- 03/6/2019 - something is missing..check what am I updating for the child group? 
                                if (!SkipLyrisManager)
                                {
                                    this.LyrisManager2.UpdateExistingList(childgrp.LyrisListName, childgrp.LyrisShortDescription);
                                    LyrisMember.UpdateLyrisSend(childgrp.LyrisListName, childgrp.LyrisSendId);
                                }
                            }
                            //Update Lyris group -- 12/6/2017 - something is missing..check what am I updating for the child group? 
                            if (!SkipLyrisManager)
                            {
                                this.LyrisManager2.UpdateExistingList(Group.LyrisListName, Group.LyrisShortDescription);
                                LyrisMember.UpdateLyrisSend(Group.LyrisListName, Group.LyrisSendId);
                            }
                        }
                        catch (Exception ex)
                        {
                            if (!removeadmin)
                                message += string.Format("There has been an error in removing the admin email from Lyris List (admin@a4a.org). <br/>");
                            message += string.Format("The Group has been updated. But there was an error in Transaction Creation!" + ex.Message);
                            HttpError err = new HttpError(message);
                            return Request.CreateResponse(HttpStatusCode.NotFound, err);
                        }
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
            catch (Exception ex)
            {
                message = string.Format("Error in saving the group. Error: {0}", ex.Message);
                HttpError err = new HttpError(message);
                return Request.CreateResponse(HttpStatusCode.NotFound, err);
            }
        }
        [Route("api/GetGroupUserlineItem")] //for taskforce and other view loads
        public string GetGroupUserlineItem(int GroupId = 0, string Company = null, string RoleId = null, int UserId = 0)
        {
            DataTable dt = new DataTable();
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;

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
                    }
                    else
                    if (GroupId > 0 && !string.IsNullOrEmpty(RoleId))
                    {
                        //Commented by NA - JD/SM wanted Committee& councils to work independently from 
                        //Get ChildGroup Informational Users if it is a Committee Group and Informational Role. 
                        //Uncommented after the issue of misundertsanding with conucilsandcommittes and now they want the cuncil committee to act like asingle group - 11/12/2018
                        if (RoleId.Contains(((int)CommitteeRole.Informational).ToString()))
                        {
                            ATAGroup grp = new ATAGroup(GroupId);
                            if (grp.IsCommittee && !grp.IsChildGroup)
                            {
                                try
                                {
                                    ATAGroup childGroup = grp.GetCommitteeChildGroup();
                                    GroupId = childGroup.GroupId; //Get ChildGroup's name if it is a Committee Group and not a committee Child
                                }
                                catch (Exception ex)
                                {
                                    throw ex;
                                }
                            }
                        }
                        SqlParameter[] spm = new SqlParameter[2];
                        spm[0] = new SqlParameter("GroupId", GroupId);
                        spm[1] = new SqlParameter("RoleId", SqlDbType.NVarChar);
                        spm[1].Value = RoleId ?? (object)DBNull.Value;
                        cmd.Parameters.AddRange(spm);
                    }
                    else
                    if (UserId > 0)
                    {
                        SqlParameter[] spm = new SqlParameter[3];
                        spm[0] = new SqlParameter("UserId", UserId);
                        spm[1] = new SqlParameter("CurrentUserId", DBUtilAPIController.CurrentUser().UserId);
                        spm[2] = new SqlParameter("IsCurrentUserAdmin", GetIsCurrentUserAdmin());
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
                }
            }

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

        protected void AddUsertoGroups(UserGroupJsonModel ug, CommitteeRole[] roles)
        {

            bool subscriberRole = false;

            foreach (CommitteeRole role in roles)
            {
                if ((bool)ug.GetType().GetProperty(role.ToString()).GetValue(ug, null))
                {
                    //If user is checked for the role,  check if they don't already exist
                    if (UserGroupJsonModel.IsA4AStaffExistinGroupsbyRole(ug.GroupId, ug.UserId, (int)role) < 1)
                    {
                        //Add the user if they don't already exists
                        UserGroupJsonModel.AddA4AStafftoGroupsbyRole(ug.GroupId, ug.UserId, (int)role);
                        Transactions.setTransaction(ug.GroupId, DBUtilAPIController.CurrentUser().UserId, "UserID", ug.UserId.ToString(), "CommitteeRoleID", ((int)role).ToString(), TransactionType.UserGroup_UserAdded);

                    }
                    //Lyris adding for roles other than MAnageGroup
                    if (role != CommitteeRole.ManageGroup)
                    {
                        subscriberRole = true;
                        AddA4AUsertoLyrisGroup(ug);
                    }
                }
                else
                {
                    if (UserGroupJsonModel.IsA4AStaffExistinGroupsbyRole(ug.GroupId, ug.UserId, (int)role) > 0)
                    {
                        UserGroupJsonModel.DeleteGroupA4AUser(ug.GroupId, ug.UserId, (int)role);

                        Transactions.setTransaction(ug.GroupId, DBUtilAPIController.CurrentUser().UserId, "UserId", ug.UserId.ToString(), "CommitteeRoleID", ((int)role).ToString(), TransactionType.UserGroup_UserRemoved);
                    }
                }
                if (!subscriberRole) //The user doesn't have EmailAdmin, BounceAdmin or Subscriber turned on. Remove the user
                {
                    RemoveUserfromLyrisGroup(ug); //removes the user from Lyris - Added 9/20/2018
                }
            }
        }
        [HttpGet]
        [Route("api/testapi")]
        public string testapi()
        {
            return "test";
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
        public HttpResponseMessage Post(List<UserGroupJsonModel> UsersinGroup, bool isContact = false)
        {
            return SavePost(UsersinGroup, false, false, 0, isContact);
        }

        [Route("api/UserGroupGGA")]
        public HttpResponseMessage SavePostGGA(List<UserGroupJsonModel> UsersinGroup)
        {
            return SavePost(UsersinGroup, false, true, 0);
        }
        /*******************************************************************************/
        /*  The below code works great when its single user - group being saved. For a bulk save go to
        /*******************************************************************************/
        [Route("api/A4AUserGroup")]
        public HttpResponseMessage SavePost(List<UserGroupJsonModel> UsersinGroup, bool IsStaff, bool IsGGA, int groupType = 0, bool isContact = false)
        {
            try
            {
                foreach (UserGroupJsonModel ug in UsersinGroup)
                {
                    ATAGroup grp = new ATAGroup(ug.GroupId);
                    IsGGA = (grp.GroupTypeId == 7 || grp.GroupTypeId == 8 || grp.GroupTypeId == 9 || grp.GroupTypeId == 10) ? true : IsGGA;
                    
                    //bool isGGAGroup = IsGGA; //todo: Remove the isGGA from api method parameter list
                    bool isDoublePrimary = (grp.GroupTypeId == 9 || grp.GroupTypeId == 10) ? true : false;

                    if (ug.GroupId < 1)
                    {
                        var message = string.Format("No GroupId found. So cannot add user!");
                        HttpError err = new HttpError(message);
                        return Request.CreateResponse(HttpStatusCode.NotFound, err);
                    }
                    if (IsStaff)
                    {
                        AddUsertoGroups(ug, new CommitteeRole[] { CommitteeRole.ManageGroup, CommitteeRole.EmailAdmin, CommitteeRole.BounceReports, CommitteeRole.StaffSubscribe });
                        //Check if committee and add a4a staff to child  --Removed 9/20/2018 JD/SM wanted the sync between committee group and info group and wanted them to function as independent groups.
                        if (grp.IsCommittee)
                        {
                            try
                            {
                                ATAGroup childGroup = grp.GetCommitteeChildGroup();
                                ug.GroupId = childGroup.GroupId;
                                AddUsertoGroups(ug, new CommitteeRole[] { CommitteeRole.ManageGroup, CommitteeRole.EmailAdmin, CommitteeRole.BounceReports, CommitteeRole.StaffSubscribe });   //Add the staff to DB & Lyris for Committee information group too.
                            }
                            catch (Exception ex)
                            {
                                throw new Exception("Error in accessing the child group", ex.InnerException);
                            }
                        }
                    }
                    else
                    {   //Checks if the user has a role in the group already - of yes then return error
                        if (UserGroupJsonModel.UserExistsinGroup(ug) < 1)
                        {
                            if (ug.GroupId > 0 || !string.IsNullOrEmpty(ug.CompanyName))
                            {
                                if ((ug.Primary || ug.Alternate || ug.Chair || ug.ViceChair))
                                {
                                    if (isDoublePrimary)
                                    {
                                        if (ug.Primary)
                                        {
                                            if (checkifcompanyhastwoprimary(ug))
                                            {
                                                var message = string.Format("Two primary has already been set for this company. Please delete any primary and then add this user!");
                                                HttpError err = new HttpError(message);
                                                return Request.CreateResponse(HttpStatusCode.NotFound, err);
                                            }
                                            else if (checkifcompanyhasprimary(ug))
                                            {
                                                if (checkifcompanyhasrole(ug.GroupId, ug.CompanyName, (int)CommitteeRole.Chair))
                                                {
                                                    var message = string.Format("The company has one primary and Chair assigned already. Cannot have more than one chair and primary. Please delete any primary and chair.");
                                                    HttpError err = new HttpError(message);
                                                    return Request.CreateResponse(HttpStatusCode.NotFound, err);
                                                }
                                                else if (checkifcompanyhasrole(ug.GroupId, ug.CompanyName, (int)CommitteeRole.ViceChair))
                                                {
                                                    var message = string.Format("The company has one primary and Vice Chair assigned already. Cannot have more than one Vice chair and primary. Please delete any primary or Vice chair.");
                                                    HttpError err = new HttpError(message);
                                                    return Request.CreateResponse(HttpStatusCode.NotFound, err);
                                                }
                                            }
                                            else
                                            {
                                                if (checkifcompanyhasrole(ug.GroupId, ug.CompanyName, (int)CommitteeRole.Chair))
                                                {
                                                    if (checkifcompanyhasrole(ug.GroupId, ug.CompanyName, (int)CommitteeRole.ViceChair))
                                                    {
                                                        var message = string.Format("The company has already Chair/Vice Chair assigned. Please delete any one of them to add new primary user.");
                                                        HttpError err = new HttpError(message);
                                                        return Request.CreateResponse(HttpStatusCode.NotFound, err);
                                                    }
                                                }
                                                else if (checkifcompanyhasrole(ug.GroupId, ug.CompanyName, (int)CommitteeRole.ViceChair))
                                                {
                                                    if (checkifcompanyhasrole(ug.GroupId, ug.CompanyName, (int)CommitteeRole.Chair))
                                                    {
                                                        var message = string.Format("The company has already Chair/Vice Chair assigned. Please delete any one of them to add new primary user.");
                                                        HttpError err = new HttpError(message);
                                                        return Request.CreateResponse(HttpStatusCode.NotFound, err);
                                                    }
                                                }
                                            }
                                        }
                                        else if (ug.Chair)
                                        {
                                            if (checkifcompanyhastwoprimary(ug))
                                            {
                                                var message = string.Format("The company has already two primary. Please delete any one of them to add new chair."); //A Chair has already been set for this Group. Please delete the current Chair and then add this user!");
                                                HttpError err = new HttpError(message);
                                                return Request.CreateResponse(HttpStatusCode.NotFound, err);
                                            }
                                            else if (checkifcompanyhasprimary(ug))
                                            {
                                                if (checkifcompanyhasrole(ug.GroupId, ug.CompanyName, (int)CommitteeRole.ViceChair))
                                                {
                                                    var message = string.Format("The company has one primary and Vice Chair assigned already. Cannot add chair. Please delete any primary or Vice Chair.");
                                                    HttpError err = new HttpError(message);
                                                    return Request.CreateResponse(HttpStatusCode.NotFound, err);
                                                }
                                            }

                                            Transactions.setUserGroupTransaction(ug.GroupId, TransactionType.UserGroup_UserRemoved, CommitteeRole.Chair, DBUtilAPIController.CurrentUser().UserId);
                                            if (ug.Type != "7")
                                            {
                                                UserGroupJsonModel.DeleteGroupUserbyRole(ug.GroupId, 3);
                                            }
                                        }

                                        else if (ug.ViceChair)
                                        {
                                            if (checkifcompanyhastwoprimary(ug))
                                            {
                                                var message = string.Format("The company has already two primary. Please delete any one of them to add new Vice Chair."); //A Chair has already been set for this Group. Please delete the current Chair and then add this user!");
                                                HttpError err = new HttpError(message);
                                                return Request.CreateResponse(HttpStatusCode.NotFound, err);
                                            }
                                            else if (checkifcompanyhasprimary(ug))
                                            {
                                                if (checkifcompanyhasrole(ug.GroupId, ug.CompanyName, (int)CommitteeRole.Chair))
                                                {
                                                    var message = string.Format("The company has one primary and Chair assigned already. Cannot add Vice Chair. Please delete any primary or Chair.");
                                                    HttpError err = new HttpError(message);
                                                    return Request.CreateResponse(HttpStatusCode.NotFound, err);
                                                }
                                            }

                                            Transactions.setUserGroupTransaction(ug.GroupId, TransactionType.UserGroup_UserRemoved, CommitteeRole.ViceChair, DBUtilAPIController.CurrentUser().UserId);
                                            if (ug.Type != "7")
                                            {
                                                UserGroupJsonModel.DeleteGroupUserbyRole(ug.GroupId, 4);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (ug.Primary && grp.GroupTypeId != 1 && grp.GroupTypeId != 2)
                                        {
                                            if (checkifcompanyhasprimary(ug))
                                            {
                                                var message = string.Format("A primary has already been set for this company. Please delete the current primary and then add this user!");
                                                HttpError err = new HttpError(message);
                                                return Request.CreateResponse(HttpStatusCode.NotFound, err);
                                            }
                                            if (checkifcompanyhasrole(ug.GroupId, ug.CompanyName, (int)CommitteeRole.Chair))
                                            {
                                                var message = string.Format("The company has a Chair assigned already. Cannot have Chair and Primary from the same company."); //A Chair has already been set for this Group. Please delete the current Chair and then add this user!");
                                                HttpError err = new HttpError(message);
                                                return Request.CreateResponse(HttpStatusCode.NotFound, err);
                                            }
                                            if (checkifcompanyhasrole(ug.GroupId, ug.CompanyName, (int)CommitteeRole.ViceChair))
                                            {
                                                var message = string.Format("The company has a Vice Chair assigned already. Cannot have Primary and Vice Chair from the same company."); //A Chair has already been set for this Group. Please delete the current Chair and then add this user!");
                                                HttpError err = new HttpError(message);
                                                return Request.CreateResponse(HttpStatusCode.NotFound, err);
                                            }
                                            Transactions.setUserGroupTransaction(ug, TransactionType.UserGroup_UserRemoved, CommitteeRole.Primary, DBUtilAPIController.CurrentUser().UserId);
                                            if (ug.Type != "7")
                                            {
                                                UserGroupJsonModel.DeleteGroupUserbyCompany(ug.CompanyName, ug.GroupId, 1);
                                            }
                                        }
                                        else
                                        if (ug.Alternate && !IsGGA)
                                        {
                                            if (checkifcompanyhasalternate(ug))
                                            {
                                                var message = string.Format("A alternate has already been set for this company. Please delete the current alternate and then add this user!");
                                                HttpError err = new HttpError(message);
                                                return Request.CreateResponse(HttpStatusCode.NotFound, err);
                                            }
                                            Transactions.setUserGroupTransaction(ug, TransactionType.UserGroup_UserRemoved, CommitteeRole.Alternate, DBUtilAPIController.CurrentUser().UserId);
                                            if (ug.Type != "7")
                                            {
                                                UserGroupJsonModel.DeleteGroupUserbyCompany(ug.CompanyName, ug.GroupId, 2);
                                            }
                                        }
                                        else
                                        if (ug.Chair)
                                        {
                                            if (checkifcompanyhasrole(ug.GroupId, ug.CompanyName, (int)CommitteeRole.Primary) && grp.GroupTypeId != 1 && grp.GroupTypeId != 2)
                                            {
                                                var message = string.Format("The company has a Primary assigned already. Cannot have Chair and Primary from the same company."); //A Chair has already been set for this Group. Please delete the current Chair and then add this user!");
                                                HttpError err = new HttpError(message);
                                                return Request.CreateResponse(HttpStatusCode.NotFound, err);
                                            }
                                            if (checkifcompanyhasrole(ug.GroupId, ug.CompanyName, (int)CommitteeRole.ViceChair))
                                            {
                                                var message = string.Format("The company has a Vice Chair assigned already. Cannot have Chair and Vice Chair from the same company."); //A Chair has already been set for this Group. Please delete the current Chair and then add this user!");
                                                HttpError err = new HttpError(message);
                                                return Request.CreateResponse(HttpStatusCode.NotFound, err);
                                            }
                                            if (checkifcompanyhasrole(ug.GroupId, ug.CompanyName, (int)CommitteeRole.Chair))
                                            {
                                                var message = string.Format("The company has a Chair assigned already. Please refresh the page and use the delete button to remove the existing chair and try again."); //A Chair has already been set for this Group. Please delete the current Chair and then add this user!");
                                                HttpError err = new HttpError(message);
                                                return Request.CreateResponse(HttpStatusCode.NotFound, err);
                                            }
                                            Transactions.setUserGroupTransaction(ug.GroupId, TransactionType.UserGroup_UserRemoved, CommitteeRole.Chair, DBUtilAPIController.CurrentUser().UserId);
                                            if (ug.Type != "7")
                                            {
                                                UserGroupJsonModel.DeleteGroupUserbyRole(ug.GroupId, 3);
                                            }
                                        }
                                        else if (ug.ViceChair)
                                        {
                                            if (checkifcompanyhasrole(ug.GroupId, ug.CompanyName, (int)CommitteeRole.Primary) && grp.GroupTypeId != 1 && grp.GroupTypeId != 2)
                                            {
                                                var message = string.Format("The company has a Primary assigned already. Cannot have Vice Chair and Primary from the same company."); //A Chair has already been set for this Group. Please delete the current Chair and then add this user!");
                                                HttpError err = new HttpError(message);
                                                return Request.CreateResponse(HttpStatusCode.NotFound, err);
                                            }
                                            if (checkifcompanyhasrole(ug.GroupId, ug.CompanyName, (int)CommitteeRole.Chair))
                                            {
                                                var message = string.Format("The company has a Chair assigned already. Cannot have Chair and Vice Chair from the same company."); //A Chair has already been set for this Group. Please delete the current Chair and then add this user!");
                                                HttpError err = new HttpError(message);
                                                return Request.CreateResponse(HttpStatusCode.NotFound, err);
                                            }
                                            if (checkifcompanyhasrole(ug.GroupId, ug.CompanyName, (int)CommitteeRole.ViceChair))
                                            {
                                                var message = string.Format("Vice chair already exists. Please refresh the page and use the delete button to remove the existing vice chair and try again.");// A Vice Chair has already been sefor this Group. Please delete the current Chair and then add this user");
                                                HttpError err = new HttpError(message);
                                                return Request.CreateResponse(HttpStatusCode.NotFound, err);
                                            }

                                            Transactions.setUserGroupTransaction(ug.GroupId, TransactionType.UserGroup_UserRemoved, CommitteeRole.ViceChair, DBUtilAPIController.CurrentUser().UserId);
                                            if (ug.Type != "7")
                                            {
                                                UserGroupJsonModel.DeleteGroupUserbyRole(ug.GroupId, 4);
                                            }
                                        }
                                        else
                                        {
                                            if (ug.Primary && isContact)
                                            {
                                                if (checkifcompanyhasprimary(ug))
                                                {
                                                    var message = string.Format("A primary has already been set for this company. Please delete the current primary and then add this user!");
                                                    HttpError err = new HttpError(message);
                                                    return Request.CreateResponse(HttpStatusCode.NotFound, err);
                                                }
                                                Transactions.setUserGroupTransaction(ug, TransactionType.UserGroup_UserRemoved, CommitteeRole.Primary, DBUtilAPIController.CurrentUser().UserId);
                                                if (ug.Type != "7")
                                                {
                                                    UserGroupJsonModel.DeleteGroupUserbyCompany(ug.CompanyName, ug.GroupId, 1);
                                                }
                                            }
                                            else
                                        if (ug.Chair && isContact)
                                            {
                                                if (checkifcompanyhasrole(ug.GroupId, ug.CompanyName, (int)CommitteeRole.Chair))
                                                {
                                                    var message = string.Format("The company has a Chair assigned already. Please refresh the page and use the delete button to remove the existing chair and try again."); //A Chair has already been set for this Group. Please delete the current Chair and then add this user!");
                                                    HttpError err = new HttpError(message);
                                                    return Request.CreateResponse(HttpStatusCode.NotFound, err);
                                                }
                                                Transactions.setUserGroupTransaction(ug.GroupId, TransactionType.UserGroup_UserRemoved, CommitteeRole.Chair, DBUtilAPIController.CurrentUser().UserId);
                                                if (ug.Type != "7")
                                                {
                                                    UserGroupJsonModel.DeleteGroupUserbyRole(ug.GroupId, 3);
                                                }
                                            }
                                            else if (ug.ViceChair && isContact)
                                            {
                                                if (checkifcompanyhasrole(ug.GroupId, ug.CompanyName, (int)CommitteeRole.ViceChair))
                                                {
                                                    var message = string.Format("Vice chair already exists. Please refresh the page and use the delete button to remove the existing vice chair and try again.");// A Vice Chair has already been sefor this Group. Please delete the current Chair and then add this user");
                                                    HttpError err = new HttpError(message);
                                                    return Request.CreateResponse(HttpStatusCode.NotFound, err);
                                                }

                                                Transactions.setUserGroupTransaction(ug.GroupId, TransactionType.UserGroup_UserRemoved, CommitteeRole.ViceChair, DBUtilAPIController.CurrentUser().UserId);
                                                if (ug.Type != "7")
                                                {
                                                    UserGroupJsonModel.DeleteGroupUserbyRole(ug.GroupId, 4);
                                                }
                                            }
                                        }
                                        if (CheckPrimaryAlternate(ug.GroupId, ug, ug.CompanyName, out string errorMsg) && (grp.GroupTypeId == 1 || grp.GroupTypeId == 2))
                                        {
                                            var message = string.Format(errorMsg);
                                            HttpError err = new HttpError(message);
                                            return Request.CreateResponse(HttpStatusCode.NotFound, err);
                                        }
                                    }
                                }

                                //Check if isCommittee and a parent group and CommitteeRoleID = 12 - Informational  -- Commented by NA 9/20/2018 - JD &SM wanted the council and committee lists to work independently
                                if (grp.IsCommittee && !grp.IsChildGroup && ug.Informational)
                                {
                                    try
                                    {
                                        ATAGroup childGroup = grp.GetCommitteeChildGroup();
                                        ug.GroupId = childGroup.GroupId;
                                        ug.Save();
                                        Transactions.setUserGroupTransaction(ug, TransactionType.UserGroup_UserAdded, DBUtilAPIController.CurrentUser().UserId, true);
                                        AddUsertoLyrisGroup(ug);
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new Exception("Error in accessing the child group", ex.InnerException);
                                    }
                                }
                                else
                                {
                                    ug.Save();
                                    Transactions.setUserGroupTransaction(ug, TransactionType.UserGroup_UserAdded, DBUtilAPIController.CurrentUser().UserId, true);
                                    AddUsertoLyrisGroup(ug);
                                }

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
                            var message = string.Format("User {0} already exists!", DbUtil.GetUserNamebyUserId(ug.UserId));
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
        // http://localhost:5198/group/index/?gid=1181#5

        [HttpGet]
        [Route("api/checkifcompanyhasrole")]
        public bool checkifcompanyhasrole(int GroupId, string CompanyName, int RoleId)
        {
            if (UserGroupJsonModel.CompanyExistsinRoleinGroup(GroupId, CompanyName, RoleId) > 0)
                return true;
            return false;
        }

        public bool checkifcompanyhasprimary(UserGroupJsonModel ug)
        {
            if (UserGroupJsonModel.CompanyExistsinRoleinGroup(ug, (int)CommitteeRole.Primary) > 0)
                return true;

            return false;
        }

        public bool checkifcompanyhastwoprimary(UserGroupJsonModel ug)
        {
            if (UserGroupJsonModel.CompanyExistsinRoleinGroup(ug, (int)CommitteeRole.Primary) > 1)
                return true;

            return false;
        }

        public bool checkifcompanyhasalternate(UserGroupJsonModel ug)
        {
            if (UserGroupJsonModel.CompanyExistsinRoleinGroup(ug, (int)CommitteeRole.Alternate) > 0)
                return true;

            return false;
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
                throw new Exception("In AddUsertoLyrisGroup: " + ex.Message);
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


                bool ret = AddUsertoLyrisDB(name, email, groupName);
                return ret;
            }
            catch (Exception ex)
            {
                throw new Exception("In AddUsertoLyrisGroup(UserGroupJsonModel): " + ex.Message);
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
            catch (Exception ex)
            {
                // return false;
                throw new Exception("In AddUsertoLyrisDB(str,str,str): " + ex.Message);
            }
        }

        public bool RemoveUserfromLyrisGroup(UserGroupJsonModel ug)
        {
            try
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
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("In RemoveUserfromLyrisGroup(UGJM): " + ex.Message);
            }
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
                        return true;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("In RemoveUserfromLyrisGroupbyRoleId: " + ex.Message);
            }
            return true;
        }
        public bool RemoveUserfromLyrisGroupbyCompanyName(DataTable dt)//string CompanyName, int GroupId, int RoleId)
        {
            try
            {
                foreach (DataRow dr in dt.Rows) //Assumption is that there is one or no user for that group/role/companyname and it will return the first time it is a sucess. or not enter is there are no rows.
                {
                    //DataRow dr = dt.Rows[0]; // Assumption is that there is only one user for that group/role/companyname
                    string name = dr["Name"].ToString();
                    string email = dr["Email"].ToString();
                    string groupName = dr["GroupName"].ToString();
                    if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(groupName))
                    {
                        LyrisMember.DeleteMemberFromList(email, groupName);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("In RemoveUserfromLyrisGroupbyCompanyName: " + ex.Message);
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
                    spm[0] = new SqlParameter("@GroupId", GroupId);
                    spm[1] = new SqlParameter("@UserId", UserId);
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
                    //  con.Open();
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
                    //con.Open();
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

        [Route("api/RemoveGroupUser")]
        public HttpResponseMessage RemoveGroupUser(List<UserGroupJsonModel> UsersinGroup, int GroupTypeId = 0, bool IsA4AStaff = false)
        {

            bool result = true;
            string s = "1";
            foreach (UserGroupJsonModel ug in UsersinGroup)
            {
                s += "2";
                result = DeleteGroupUser(ug);
                s += "3" + result;
                if (!result)
                    break;
                s += "4" + result;
                //Check if it a committee/council group for a4a staff delete in child group also
                //TODO: HardCoded the grouptypeID's need to clean it up and add enum 
                if ((GroupTypeId == 1 || GroupTypeId == 2 || GroupTypeId == 7 || GroupTypeId == 8 || GroupTypeId == 9 || GroupTypeId == 10) && IsA4AStaff)
                {
                    ATAGroup grp = new ATAGroup(ug.GroupId);
                    ATAGroup childGroup = grp.GetCommitteeChildGroup();
                    ug.GroupId = childGroup.GroupId;
                    s += "5" + result;
                    result = DeleteGroupUser(ug);
                    s += "6" + result;
                    if (!result)
                        break;
                }
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            if (!result)
            {
                response = Request.CreateResponse(HttpStatusCode.PreconditionFailed, s);
            }
            return response;
        }

        public bool DeleteGroupUser(UserGroupJsonModel ug)
        {
            bool result = true, roleset = false;
            //Check if any roles are set            
            foreach (CommitteeRole role in Enum.GetValues(typeof(CommitteeRole)))
            {
                if ((bool)ug.GetType().GetProperty(role.ToString()).GetValue(ug, null))
                {
                    roleset = true;
                    break;// We can move to next step now because some role is set for deletion and not all roles are empty
                }
            }
            if (roleset)
            {
                try
                {
                    /*Below if statement is added for CommitteeCouncil InfoGroups to get the child group */
                    DataTable dt;
                    if (ug.Informational)
                    {
                        ATAGroup grp = new ATAGroup(ug.GroupId);
                        if (grp.IsCommittee)
                        {
                            ATAGroup childGroup = grp.GetCommitteeChildGroup();
                            ug.GroupId = childGroup.GroupId;
                        }
                        else
                            ug.GroupId = grp.GroupId;
                    }
                    dt = GetUserGroupDetailsbyUserId(ug.GroupId, ug.UserId);
                    result = UserGroupJsonModel.DeleteGroupsUser(ug);
                    if (!result)
                        throw new Exception("Deletion of the user from the group was unsuccessful.");
                    bool lyrresult = RemoveUserfromLyrisGroup(ug); //removes the user from Lyris
                    if (!lyrresult)
                        throw new Exception("Deletion of the user from the UM Group was successful but failed to delete from Lyris. Contact IT for help.");

                    Transactions.setTransaction(dt, DBUtilAPIController.CurrentUser().UserId, TransactionType.UserGroup_UserRemoved); //Added 12/18
                }
                catch
                {
                    return false;
                }

                return result;
            }
            else
                return false;  //None of the roles have a true value. So deletion is not going to happen
        }


        [Route("api/RemoveGrpUsrbyUserId")]
        //Deletes all users for the giveb groupid and roleid
        public HttpResponseMessage RemoveGroupUserbyUserId(int GroupId, int UserId, int RoleId)
        {
            if (RoleId == 5)
            {
                ATAGroup grp = new ATAGroup(GroupId);
                ATAGroup childGroup = grp.GetCommitteeChildGroup();
                bool result1 = UserGroupJsonModel.DeleteGroupUserbyUserID(childGroup.GroupId, UserId, RoleId);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            DataTable dt = GetUserGroupDetailsbyUserId(GroupId, UserId);
            if (dt.Rows.Count == 0)
            {
                var message = string.Format("No user defined!");
                HttpError err = new HttpError(message);
                return Request.CreateResponse(HttpStatusCode.NotFound, err);
            }
            bool result = UserGroupJsonModel.DeleteGroupUserbyUserID(GroupId, UserId, RoleId);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            if (!result)
            {
                var message = string.Format("Couldn't delete the user");
                HttpError err = new HttpError(message);
                return Request.CreateResponse(HttpStatusCode.NotFound, err);
            }
            else
            {
                bool lyrresult = RemoveUserfromLyrisGroupbyRoleId(dt);
                if (lyrresult)
                    Transactions.setTransaction(GroupId, DBUtilAPIController.CurrentUser().UserId, "UserId", UserId.ToString(), "CommitteeRoleID", RoleId.ToString(), TransactionType.UserGroup_UserRemoved);
                else
                {
                    var message = string.Format("Couldn't delete the user from Lyris. Please contact IT help.");
                    HttpError err = new HttpError(message);
                    return Request.CreateResponse(HttpStatusCode.NotFound, err);
                }
            }

            
            return response;
        }


        [Route("api/RemoveGrpUsrbyRole")]
        //Deletes all users for the giveb groupid and roleid
        public HttpResponseMessage RemoveGroupUserbyRoleId(int GroupId, int RoleId)
        {
            //get the details to use it later to remove the deleted users from Lyris
            DataTable dt = GetUserGroupDetailsbyRoleId(GroupId, RoleId);
            if (dt.Rows.Count == 0)
            {
                var message = string.Format("No user defined in this group for the given role!");
                HttpError err = new HttpError(message);
                return Request.CreateResponse(HttpStatusCode.NotFound, err);
            }
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
                bool lyrresult = RemoveUserfromLyrisGroupbyRoleId(dt);
                if (lyrresult)
                    Transactions.setTransaction(dt, DBUtilAPIController.CurrentUser().UserId, TransactionType.UserGroup_UserRemoved); //Added 7/28/17
                else
                {
                    var message = string.Format("Couldn't delete the user from Lyris. Please contact IT help.");
                    HttpError err = new HttpError(message);
                    return Request.CreateResponse(HttpStatusCode.NotFound, err);
                }
            }
            return response;

            //RemoveUserfromLyrisGroupbyRoleId(dt);
            //Transactions.setTransaction(dt, DBUtilAPIController.CurrentUser().UserId, TransactionType.UserGroup_UserRemoved); //Added 7/28/17   
            //Transactions.setUserGroupTransaction(GroupId, TransactionType.UserGroup_UserRemoved, (CommitteeRole)RoleId, DBUtilAPIController.CurrentUser().UserId);
        }

        [Route("api/RemoveGrpUsrbyComp")]
        public HttpResponseMessage RemoveGroupUserbyCompany(string CompanyName, int GroupId, int RoleId)
        {
            //UserGroupJsonModel.DeleteGroupUserbyCompany(CompanyName, GroupId, RoleId); //for some reason I had repetative code
            //Get the details of the user being removed so you can remove them from Lyris
            DataTable dt = GetUserGroupDetailsbyCompanyName(CompanyName, GroupId, RoleId);
            if (dt.Rows.Count == 0)
            {
                var message = string.Format("No user defined for this company in this role!");
                HttpError err = new HttpError(message);
                return Request.CreateResponse(HttpStatusCode.NotFound, err);
            }
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
            {
                bool lyrresult = RemoveUserfromLyrisGroupbyCompanyName(dt);
                if (lyrresult)
                    Transactions.setTransaction(dt, DBUtilAPIController.CurrentUser().UserId, TransactionType.UserGroup_UserRemoved); //Added 7/28/17
                else
                {
                    var message = string.Format("Couldn't delete the user from Lyris. Please contact IT help.");
                    HttpError err = new HttpError(message);
                    return Request.CreateResponse(HttpStatusCode.NotFound, err);
                }

                //Delete was successful so save the 
                //RemoveUserfromLyrisGroupbyCompanyName(dt); 
                //Transactions.setTransaction(dt, DBUtilAPIController.CurrentUser().UserId, TransactionType.UserGroup_UserRemoved); //Added 7/28/17
                //Transactions.setUserGroupTransaction(GroupId, TransactionType.UserGroup_UserRemoved, (CommitteeRole)RoleId, DBUtilAPIController.CurrentUser().UserId);
            }
            return response;
        }
        [Route("api/getTransactions")]
        public string GetTransactionsList(int? groupid = null, int? userid = null, int? count = 0)
        {
            DataTable dt = new DataTable();
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;

            using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
            {
                StringBuilder sql = new StringBuilder();

                using (SqlCommand cmd = new SqlCommand("p_Transaction_GetList", con))
                {
                    con.Open();
                    if (groupid > 0)
                    {
                        SqlParameter[] spm = new SqlParameter[2];
                        spm[0] = new SqlParameter("groupid", groupid);
                        spm[1] = new SqlParameter("Count", count);
                        cmd.Parameters.AddRange(spm);
                    }
                    else
                    if (userid.Value > 0)
                    {
                        SqlParameter[] spm = new SqlParameter[2];
                        spm[0] = new SqlParameter("userid", userid); //Userid was given
                        spm[1] = new SqlParameter("Count", count);
                        cmd.Parameters.AddRange(spm);
                    }

                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    foreach (DataRow dr in dt.Rows)
                    {
                        row = new Dictionary<string, object>();
                        foreach (DataColumn col in dt.Columns)
                        {
                            row.Add(col.ColumnName, dr[col]);
                        }
                        rows.Add(row);
                    }
                }
            }
            return serializer.Serialize(rows);
        }

        [HttpGet]
        [Route("api/getManageGroupCount")]
        public int GetManageGroupCount(int groupId = 0)
        {
            int manageGroupcount = 0;
            using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
            {
                StringBuilder sql = new StringBuilder();

                using (SqlCommand cmd = new SqlCommand("p_getManageGroup_Count", con))
                {
                    con.Open();
                    SqlParameter[] spm = new SqlParameter[1];
                    spm[0] = new SqlParameter("GroupId", groupId);
                    cmd.Parameters.AddRange(spm);


                    cmd.CommandType = CommandType.StoredProcedure;
                    var reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            manageGroupcount = int.Parse(reader["ManageGroupCount"].ToString());
                        }
                    }

                }
            }
            return manageGroupcount;
        }

        [HttpGet]
        [Route("api/CheckPrimaryAlternate")]
        public bool CheckPrimaryAlternate(int groupId, UserGroupJsonModel user, string companyName, out string msg)
        {
            bool isValid = false;
            string role = user.Alternate ? "Alternate" : user.Primary ? "Primary" : user.Chair ? "Chair" : user.ViceChair ? "ViceChair" : string.Empty;
            if (String.IsNullOrEmpty(role))
            {
                msg = "";
                return false;
            }
            try
            {
                using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
                {
                    StringBuilder sql = new StringBuilder();

                    using (SqlCommand cmd = new SqlCommand("p_userGroupPrimaryAlternate_Check", con))
                    {
                        con.Open();
                        SqlParameter[] spm = new SqlParameter[3];
                        spm[0] = new SqlParameter("GroupId", groupId);
                        spm[1] = new SqlParameter("Role", role);
                        spm[2] = new SqlParameter("CompanyName", companyName);
                        cmd.Parameters.AddRange(spm);
                        cmd.CommandType = CommandType.StoredProcedure;
                        var reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                msg = reader["Msg"].ToString();
                                return bool.Parse(reader["Flag"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            msg = string.Empty;
            return isValid;
        }

        [Route("api/GetGroupEmailMembersandsubscriber")]
        public string GetGroupMembersAndSubscribers(int GroupId)
        {
            ATAGroup grp = new ATAGroup(GroupId);
            string[] users = grp.GetGroupEmailMembersSubscriber(GroupId);
            StringBuilder sb = new StringBuilder();

            foreach (string u in users)
            {
                sb.Append(u);
                sb.Append(";");
            }
            return sb.ToString();
        }

        [Route("api/updategroupuserroles")]
        [HttpPost]
        public string UpdateGroupUserRoles(List<GroupUserRoles> groupUserRole)
        {
            try
            {

                string userName = groupUserRole.Select(z => z.UserName).First();
                string currentUser = GetUserName();

                if (String.IsNullOrEmpty(userName))
                {
                    userName = currentUser;
                }

                int userId = ATAGroup.GetUserId(userName);

                DataTable dt = groupUserRole.ToDataTable<GroupUserRoles>();

                using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
                {
                    StringBuilder sql = new StringBuilder();

                    using (SqlCommand cmd = new SqlCommand("p_UpdateUserGroupRoles", con))
                    {
                        con.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserGroupRoles", dt);
                        cmd.Parameters.AddWithValue("@UserName", userName);
                        cmd.Parameters.AddWithValue("@CurrentUser", currentUser);
                        var reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                groupUserRole.Add(new GroupUserRoles()
                                {
                                    GroupId = Int32.Parse(reader["GroupId"].ToString()),
                                    Role = reader["CommitteeRoleName"].ToString(),
                                    Value = reader["Value"].ToString(),
                                    UserName = userName
                                });
                            }
                        }
                    }
                }

                List<UserGroupJsonModel> userGroupJsonModels = MapUGtoGU(groupUserRole, userId);

                foreach (var userGroup in userGroupJsonModels)
                {
                    UpdateLyrisStaffRoles(userGroup, new CommitteeRole[] { CommitteeRole.ManageGroup, CommitteeRole.EmailAdmin, CommitteeRole.BounceReports, CommitteeRole.StaffSubscribe });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in saving group user roles.", ex.InnerException);
            }
            return "Succesfully Updated!";
        }

        private List<UserGroupJsonModel> MapUGtoGU(List<GroupUserRoles> groupUserRoles, int userId)
        {
            List<UserGroupJsonModel> userGroupJsonModels = new List<UserGroupJsonModel>();
            List<int> groupIds = groupUserRoles.Select(x => x.GroupId).Distinct().ToList();
            foreach (var groupId in groupIds)
            {
                var data = groupUserRoles.Where(x => x.GroupId == groupId).Select(y => new { y.GroupId, y.Role, y.UserName, y.Value }).ToList();
                userGroupJsonModels.Add(new UserGroupJsonModel()
                {
                    GroupId = groupId,
                    UserId = userId,
                    ManageGroup = data.Where(m => m.Role == "Manage Group").Select(s => s.Value).FirstOrDefault() == "True" ? true : false,
                    StaffSubscribe = data.Where(m => m.Role == "A4A Staff Subscribe").Select(s => s.Value).FirstOrDefault() == "True" ? true : false,
                    EmailAdmin = data.Where(m => m.Role == "Send Emails as Administrator").Select(s => s.Value).FirstOrDefault() == "True" ? true : false,
                    BounceReports = data.Where(m => m.Role == " Receive Bounce Reports").Select(s => s.Value).FirstOrDefault() == "True" ? true : false
                });
            }

            return userGroupJsonModels;
        }

        private void UpdateLyrisStaffRoles(UserGroupJsonModel ug, CommitteeRole[] roles)
        {
            if (ug.StaffSubscribe || ug.EmailAdmin || ug.BounceReports)
            {
                AddA4AUsertoLyrisGroup(ug);
            }
            else
            {
                RemoveUserfromLyrisGroup(ug);
            }
        }

        [Route("api/SaveCouncilCommitteeCompanyDtl")]
        [HttpPost]
        public string SaveCouncilCommitteeCompanyDtl(List<GroupCompanyUserRoles> groupCompanyUserRole)
        {
            string Message = string.Empty;
            try
            {
                UserGroupJsonModel userGroup = new UserGroupJsonModel();
                for (int i = 0; i < groupCompanyUserRole.Count; i++)
                {
                    int GroupId = groupCompanyUserRole[i].GroupId;
                    string CompanyName = groupCompanyUserRole[i].CompanyName;
                    bool CheckStatus = groupCompanyUserRole[i].Value;
                    int RoleId = groupCompanyUserRole[i].RoleId;

                    DataTable dt = new DataTable();
                    int UsereId = 0;
                    using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
                    {
                        StringBuilder sql = new StringBuilder();
                        using (SqlCommand cmd = new SqlCommand("p_Get_UserGroup_Council_Committee_Company_User_Dtl", con))
                        {
                            con.Open();
                            SqlParameter[] spm = new SqlParameter[4];
                            spm[0] = new SqlParameter("@GroupId", GroupId);
                            spm[1] = new SqlParameter("@CompanyName", CompanyName);
                            spm[2] = new SqlParameter("@Value", CheckStatus);
                            spm[3] = new SqlParameter("@RoleId", RoleId);
                            cmd.Parameters.AddRange(spm);
                            cmd.CommandType = CommandType.StoredProcedure;
                            UsereId = Int32.Parse(cmd.ExecuteScalar().ToString());
                            if (UsereId == 0)
                            {
                                Message = "Please select user";
                            }
                            else
                            {
                                Message = "Council/Committee Contacts changes saved successfully";
                            }
                        }
                    }

                    using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
                    {
                        StringBuilder sql = new StringBuilder();
                        using (SqlCommand cmd = new SqlCommand("p_Save_UserGroup_Council_Committee_Company_User_Dtl", con))
                        {
                            con.Open();
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@GroupId", GroupId);
                            cmd.Parameters.AddWithValue("@CompanyName", CompanyName);
                            cmd.Parameters.AddWithValue("@Value", CheckStatus);
                            cmd.Parameters.AddWithValue("@RoleId", RoleId);
                            var reader = cmd.ExecuteReader();
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    groupCompanyUserRole.Add(new GroupCompanyUserRoles()
                                    {
                                        GroupId = Int32.Parse(reader["GroupId"].ToString()),
                                        CompanyName = Convert.ToString(reader["CompanyName"].ToString()),
                                        Value = Convert.ToBoolean(reader["Value"].ToString()),
                                        RoleId = Convert.ToInt32(reader["RoleId"].ToString())
                                    });
                                }
                            }
                        }

                        userGroup = new UserGroupJsonModel()
                        {
                            GroupId = GroupId,
                            UserId = UsereId,
                            ManageGroup = false,
                            EmailAdmin = CheckStatus,
                            BounceReports = false,
                            StaffSubscribe = false
                        };
                        UpdateLyrisStaffRoles(userGroup, new CommitteeRole[] { CommitteeRole.ManageGroup, CommitteeRole.EmailAdmin, CommitteeRole.BounceReports, CommitteeRole.StaffSubscribe });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in saving group user roles.", ex.InnerException);
            }
            return Message;
        }

        [Route("api/SaveCouncilCommitteeStaffDtl")]
        [HttpPost]
        public string SaveCouncilCommitteeStaffDtl(List<GroupStaffUserRoles> groupStaffUserRole)
        {
            try
            {
                UserGroupJsonModel userGroup = new UserGroupJsonModel();
                for (int i = 0; i < groupStaffUserRole.Count; i++)
                {
                    int GroupId = groupStaffUserRole[i].GroupId;
                    int UserId = groupStaffUserRole[i].UserId;
                    bool CheckStatus = groupStaffUserRole[i].Value;
                    int RoleId = groupStaffUserRole[i].RoleId;
                    DataTable dt = groupStaffUserRole.ToDataTable<GroupStaffUserRoles>();
                    using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
                    {
                        StringBuilder sql = new StringBuilder();
                        using (SqlCommand cmd = new SqlCommand("p_Save_UserGroup_Council_Committee_Group_User_Dtl", con))
                        {
                            con.Open();
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@GroupId", GroupId);
                            cmd.Parameters.AddWithValue("@Value", CheckStatus);
                            cmd.Parameters.AddWithValue("@RoleId", RoleId);
                            var reader = cmd.ExecuteReader();
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    groupStaffUserRole.Add(new GroupStaffUserRoles()
                                    {
                                        GroupId = Int32.Parse(reader["GroupId"].ToString()),
                                        Value = Convert.ToBoolean(reader["Value"].ToString()),
                                        RoleId = Convert.ToInt32(reader["RoleId"].ToString())
                                    });
                                }
                            }
                        }

                        userGroup = new UserGroupJsonModel()
                        {
                            GroupId = GroupId,
                            UserId = UserId,
                            ManageGroup = false,
                            EmailAdmin = CheckStatus,
                            BounceReports = false,
                            StaffSubscribe = false
                        };
                        UpdateLyrisStaffRoles(userGroup, new CommitteeRole[] { CommitteeRole.ManageGroup, CommitteeRole.EmailAdmin, CommitteeRole.BounceReports, CommitteeRole.StaffSubscribe });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in saving group user roles.", ex.InnerException);
            }
            return "Succesfully Updated!";
        }

        [Route("api/savecommitteeroles")]
        [HttpPost]
        public string SaveCouncilCommitteeDtl(List<GroupCommitteeUserRoles> groupCommitteeUserRole)
        {
            try
            {
                UserGroupJsonModel userGroup = new UserGroupJsonModel();
                for (int i = 0; i < groupCommitteeUserRole.Count; i++)
                {
                    int GroupId = groupCommitteeUserRole[i].GroupId;
                    int UserId = groupCommitteeUserRole[i].UserId;
                    bool CheckStatus = groupCommitteeUserRole[i].Value;
                    DataTable dt = groupCommitteeUserRole.ToDataTable<GroupCommitteeUserRoles>();
                    using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
                    {
                        StringBuilder sql = new StringBuilder();
                        using (SqlCommand cmd = new SqlCommand("p_Save_UserGroup_Council_Committee_Dtl", con))
                        {
                            con.Open();
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@GroupId", GroupId);
                            cmd.Parameters.AddWithValue("@UserId", UserId);
                            cmd.Parameters.AddWithValue("@Value", CheckStatus);
                            var reader = cmd.ExecuteReader();
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    groupCommitteeUserRole.Add(new GroupCommitteeUserRoles()
                                    {
                                        GroupId = Int32.Parse(reader["GroupId"].ToString()),
                                        UserId = Int32.Parse(reader["UserId"].ToString()),
                                        Value = Convert.ToBoolean(reader["Value"].ToString())
                                    });
                                }
                            }
                        }
                    }
                    userGroup = new UserGroupJsonModel()
                    {
                        GroupId = GroupId,
                        UserId = UserId,
                        ManageGroup = false,
                        EmailAdmin = CheckStatus,
                        BounceReports = false,
                        StaffSubscribe = false
                    };
                    UpdateLyrisStaffRoles(userGroup, new CommitteeRole[] { CommitteeRole.ManageGroup, CommitteeRole.EmailAdmin, CommitteeRole.BounceReports, CommitteeRole.StaffSubscribe });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in saving group user roles.", ex.InnerException);
            }
            return "Succesfully Updated!";
        }
    }
}