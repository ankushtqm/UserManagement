using System;
using System.Collections.Generic;
using System.Collections;
using System.Configuration;
using System.Text;
using ATA.Authentication;
using ATA.Authentication.Providers;
using ATA.ObjectModel;
using SusQTech.Data;
using SusQTech.Data.DataObjects;
using System.Data.SqlClient;
using ATA.LyrisProxy;
using ATA.CodeLibrary;
using System.IO;
using ATA.Member.Util;
using System.Data;

namespace UMShowdifference
{
    class ShowGroupMemberDiff
    {
        public void Run()
        {
            using (OutputLog log = new OutputLog())
            {
                log.LogInfo("Start...");
                try
                {
                    Execute(log);
                }
                catch (Exception e)
                {
                    log.LogError(e.ToString());
                }
            }
        }

        private DataTable GetGroupUsers(int GroupId)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
            {
                StringBuilder sql = new StringBuilder();
                using (SqlCommand cmd = new SqlCommand("p_UserGroup_LoadLI", con))
                {
                    con.Open();

                    if (GroupId > 0)
                    {
                        SqlParameter[] spm = new SqlParameter[1];
                        spm[0] = new SqlParameter("GroupId", GroupId);
                        cmd.Parameters.AddRange(spm);


                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                    }
                }
            }
            return dt;            
        }
        private List<LyrisMember> CreateLyrisMembersFromATAGroup(ATAGroup group)
        {
            DataTable admins = GetGroupUsers(group.GroupId);
            List<UserGroupJsonModel> allGroupUsers = UserGroupJsonModel.GetGroupMembers(group.GroupId);
            List<LyrisMember> lyrisMembers = new List<LyrisMember>();

            foreach (UserGroupJsonModel ug in allGroupUsers)
            {
                LyrisMember lyrisMember = CreateLyrisMemberByUserId(ug.UserId, ref lyrisMembers);
                if(ug.EmailAdmin)
                lyrisMember.IsAdmin = true; 

                if(ug.BounceReports)
                lyrisMember.ReceiveAdminEmail = true;
                 
                //TODO: come up with better logic for now I am checking that the user is not a A4AStaff by checking that all 4 roles are false 
                if(!(ug.ManageGroup || ug.EmailAdmin || ug.BounceReports || ug.StaffSubscribe) || ug.StaffSubscribe)
                lyrisMember.NoMail = true;
            }
            
            return lyrisMembers;
        }

        //private List<LyrisMember> CreateLyrisMembersFromATAGroup(ATAGroup group)
        //{
        //    Dictionary<int, bool> admins = ATAGroupManager.GetGroupAdmins(group.GroupId);
        //    List<int> memberIds = DbUtil.GetGroupSelectedATAMemberIds(group.GroupId);
        //    memberIds.AddRange(DbUtil.GetGroupSelectedNonATAMemberIds(group.GroupId));
        //    List<LyrisMember> lyrisMembers = new List<LyrisMember>();
        //    foreach (int id in memberIds)
        //    {
        //        LyrisMember lyrisMember = CreateLyrisMemberByUserId(id, ref lyrisMembers);
        //        if (admins.ContainsKey(id))//is admin
        //        {
        //            lyrisMember.IsAdmin = true;
        //            lyrisMember.ReceiveAdminEmail = admins[id];
        //            admins.Remove(id);
        //        }
        //    }
        //    foreach (int adminId in admins.Keys)
        //    {
        //        LyrisMember lyrisMember = CreateLyrisMemberByUserId(adminId, ref lyrisMembers);
        //        lyrisMember.IsAdmin = true;
        //        lyrisMember.ReceiveAdminEmail = admins[adminId];
        //        lyrisMember.NoMail = true;
        //    }
        //    return lyrisMembers;
        //}

        private bool HasPropertyDiff(LyrisMember matchingOne, LyrisMember sourceMember)
        {
            return (matchingOne.IsAdmin != sourceMember.IsAdmin
                 || sourceMember.ReceiveAdminEmail != matchingOne.ReceiveAdminEmail
                 || sourceMember.NoMail != matchingOne.NoMail);

        }

        private void printMissingAdminOrBounceAdmin(ATAGroup group, List<LyrisMember> members, string databaseName, OutputLog log)
        {
            bool hasAdmin = false;
            bool hasBounceAdmin = false;
            foreach (LyrisMember member in members)
            {
                if (member.IsAdmin)
                    hasAdmin = true;
                if (member.ReceiveAdminEmail)
                    hasBounceAdmin = true;
            }
            if (!hasAdmin || !hasBounceAdmin)
            {
                printGroupTitle(group, log);
                if (!hasAdmin)
                    log.LogInfo(string.Format("missing Admin in {0}.", databaseName));
                if (!hasBounceAdmin)
                    log.LogInfo(string.Format("missing BounceAdmin in {0}.", databaseName));
            }
        }

        private void printGroupTitle(ATAGroup group, OutputLog log)
        {
            if (!groupTitlePrinted)
            {
                log.LogInfo("");
                log.LogInfo(string.Format("{0} ({1}):", group.GroupName, group.LyrisListName));
                groupTitlePrinted = true;
            }
        }

        private bool groupTitlePrinted;
        public void Execute(OutputLog log)
        {
            List<ATAGroup> allGroups = GetAllGroups(AppliesToSite.Members);
            List<string> exceptedEmails = DbUtil.GetExemptedLyrisEmailAddressesInUpperCase();
            if (exceptedEmails == null)
                exceptedEmails = new List<string>();
            foreach (ATAGroup group in allGroups)
            {
                groupTitlePrinted = false;
                List<LyrisMember> sourceMembers = CreateLyrisMembersFromATAGroup(group);
                printMissingAdminOrBounceAdmin(group, sourceMembers, "Member DB", log);

                List<LyrisMember> existingMembers = LyrisMember.GetListMembersByListName(group.LyrisListName);
                printMissingAdminOrBounceAdmin(group, existingMembers, "Lyris", log);

                Dictionary<string, LyrisMember> existingMembersMap = IndexByEmail(existingMembers);
                Dictionary<string, LyrisMember> sourceMembersMap = IndexByEmail(sourceMembers);

                List<LyrisMember> missingInLyris = new List<LyrisMember>();
                foreach (string sourceEmail in sourceMembersMap.Keys)
                {
                    //Leave it alone if it is exempted
                    if (exceptedEmails.Contains(sourceEmail.ToUpper()))
                        continue;

                    LyrisMember sourceMember = sourceMembersMap[sourceEmail];
                    if (!existingMembersMap.ContainsKey(sourceEmail))
                        missingInLyris.Add(sourceMember);

                    else//match
                    {
                        LyrisMember matchingOne = existingMembersMap[sourceEmail];
                        existingMembersMap.Remove(sourceEmail);//remove the matching item
                        if (HasPropertyDiff(matchingOne, sourceMember))
                        {
                            printGroupTitle(group, log);
                            StringBuilder sb = new StringBuilder(matchingOne.EmailAddress + " has role difference between member and Lyris: ");
                            sb.AppendFormat("In Lyris ( {0} {1} {2})", (matchingOne.IsAdmin ? "Admin" : ""),
                                (matchingOne.ReceiveAdminEmail ? "BounceAdmin" : ""), (matchingOne.NoMail ? "NoEmail" : ""));
                            sb.AppendFormat(". In MemberDB ( {0} {1} {2})", (sourceMember.IsAdmin ? "Admin" : ""),
                                (sourceMember.ReceiveAdminEmail ? "BounceAdmin" : ""), (sourceMember.NoMail ? "NoEmail" : ""));
                            log.LogInfo(sb.ToString());
                        }
                    }
                }
                printMissingMembers(group, log, missingInLyris, "Lyris");

                foreach (string exceptedEmail in exceptedEmails)
                {
                    if (existingMembersMap.ContainsKey(exceptedEmail))
                        existingMembersMap.Remove(exceptedEmail);
                }
                printMissingMembers(group, log, existingMembersMap.Values, "MemberDB");

            }
        }

        private void printMissingMembers(ATAGroup group, OutputLog log, ICollection items, string location)
        {
            if (items.Count > 0)
            {
                printGroupTitle(group, log);
                StringBuilder ab = new StringBuilder();
                foreach (LyrisMember item in items)
                {
                    ab.Append(item.EmailAddress);
                    ab.Append("  ");
                }
                log.LogInfo(string.Format("Missing user in {0}: {1}", location, ab.ToString()));
            }
        }

        private static LyrisMember CreateLyrisMemberByUserId(int userId, ref List<LyrisMember> lyrisMembers)
        {
            LookupUser lookupUser = new LookupUser(userId);
            LyrisMember lyrisMember = new LyrisMember(lookupUser.Email);
            lyrisMember.FullName = lookupUser.FormattedDisplayName;
            lyrisMembers.Add(lyrisMember);
            return lyrisMember;
        }

        private static Dictionary<string, LyrisMember> IndexByEmail(List<LyrisMember> members)
        {
            Dictionary<string, LyrisMember> mapping = new Dictionary<string, LyrisMember>();
            foreach (LyrisMember member in members)
            {
                if (!string.IsNullOrEmpty(member.EmailAddress))
                {
                    mapping[member.EmailAddress.ToLower().Trim()] = member;
                }
            }
            return mapping;
        }


        private const string GetAllGroupsSP = "p_Group_GetAll";
        private static List<ATAGroup> GetAllGroups(AppliesToSite appliesToSite)
        {
            SqlParameter[] sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter("@AppliesToSiteId", appliesToSite);
            DataObjectList<ATAGroup> groups = new DataObjectList<ATAGroup>(sqlParams, GetAllGroupsSP);
            return (groups as List<ATAGroup>);
        }
    }
}
