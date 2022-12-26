using System;
using System.Web;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using SusQTech.Data.DataObjects;
using Microsoft.ApplicationBlocks.Data;
using ATA.Member.Util;
using System.Text;
using ATA.CodeLibrary;
using System.DirectoryServices;
using System.Text.RegularExpressions;
using System.DirectoryServices.AccountManagement;
using System.Configuration;

namespace ATA.ObjectModel
{
    public class DbUtil
    {
        private const string SELECT_ATA_STAFF_USERS_PROC = "p_User_GetAllATAStaffUsers";
        private const string SELECT_ALL_CONTACT_USERS_PROC = "p_User_GetAllContacts";
        private const string Fuels_GetNonpublicUsers = "p_Fuels_GetNotPublicUsers";
        public static SortedDictionary<string, int> GetSortedAtaStaffNameToId()
        {
            DataObjectList<LookupUser> users = DbUtil.GetAllAtaStaff();
            SortedDictionary<string, int> sorted = new SortedDictionary<string, int>();
            foreach (LookupUser user in users)
            {
                if (!string.IsNullOrEmpty(user.FormattedDisplayName))
                {
                    sorted[user.FormattedDisplayName] = user.UserId;
                }
            }
            return sorted;
        }
        public static Dictionary<int, string> GetA4AStaffNameTitleToId()
        {
            return DbUtil.GetIdStringDictionary(" Select cast(ROW_NUMBER() OVER(ORDER BY LastName +', '+FirstName ASC) as int)  as Row#,(LastName +', '+FirstName + ':'+ CAST( UserId AS VARCHAR(10) )) as Name from [User] where IsATAStaff = 1 ", false, null);
        }
        public static SortedDictionary<string, int> GetAllContactsNameTitleToId(string term, int groupType) //Added GroupType to show only members on 10/23/2017
        {
            StringBuilder strQuery = new StringBuilder();
            term = term.Trim();
            string firstName = string.Empty, lastName = string.Empty, company = string.Empty;

            strQuery.Append("Select  (LastName + ', ' + FirstName + ' ('+  c.CompanyName +') ') as Name,u.UserId from [User] u inner join UserCompany uc on uc.UserId = u.UserId inner join Company c on c.CompanyId =  uc.CompanyId  where u.IsATAStaff = 0 and IsActiveContact = 1");
            if (!String.IsNullOrEmpty(term))
            {
                if (groupType > 0 && (groupType == 1 || groupType == 2 || groupType == 7 || groupType == 8 || groupType == 9 || groupType == 10))
                {
                    strQuery.Append(" and c.CompanyTypeId in (1,2) ");// Members only for councils and committes
                }
                if (groupType > 0 && (groupType == 1 || groupType == 2 || groupType == 3 || groupType == 5 || groupType == 7 || groupType == 8 || groupType == 9 || groupType == 10)) //MP active for all except Distro groups
                {
                    strQuery.Append(" and u.isActiveMB = 1    ");
                }
                if (!term.ToLower().Contains(":"))
                {
                    strQuery.Append(" and ( UserName like '%" + term + "%'  or" + " LastName like '%" + term + "%' ) ");//or" + " CompanyName like '%" + term + "%' ");
                }
                else
                {
                    //Get First Name
                    if (term.ToLower().Contains("f:"))
                    {
                        int fbindex = term.IndexOf("f:");
                        string temp = term.Substring(fbindex + 2);
                        if (temp.IndexOf(",") > 0)
                        {
                            int feindex = temp.IndexOf(",");
                            int len = feindex - fbindex;
                            firstName = temp.Substring(fbindex, len);
                        }
                        else
                            firstName = temp;
                    }

                    //Get Last Name
                    if (term.ToLower().Contains("l:"))
                    {
                        int fbindex = term.IndexOf("l:");
                        string temp = term.Substring(fbindex + 2);
                        if (temp.IndexOf(",") > 0)
                        {
                            int feindex = temp.IndexOf(",");
                            int len = feindex - fbindex;
                            lastName = temp.Substring(fbindex, len);
                        }
                        else
                            lastName = temp;
                    }
                    //Get company
                    if (term.ToLower().Contains("c:"))
                    {
                        int fbindex = term.IndexOf("c:");
                        string temp = term.Substring(fbindex + 2);
                        if (temp.IndexOf(",") > 0)
                        {
                            int feindex = temp.IndexOf(",");
                            int len = feindex - fbindex;
                            company = temp.Substring(fbindex, len);
                        }
                        else
                            company = temp;
                    }
                    string fQ = "", lQ = "", cQ = "";

                    if (!string.IsNullOrEmpty(firstName))
                    {
                        fQ = "FirstName like '%" + firstName + "%'";
                    }
                    if (!string.IsNullOrEmpty(lastName))
                    {
                        lQ = " LastName like '%" + lastName + "%' ";
                    }
                    if (!string.IsNullOrEmpty(company))
                    {
                        cQ = " CompanyName like '%" + company + "%' ";
                    }

                    if (!string.IsNullOrEmpty(firstName))
                    {
                        strQuery.Append(" and " + fQ);

                        if (!string.IsNullOrEmpty(lastName))
                        {
                            strQuery.Append(" and " + lQ);
                        }
                        if (!string.IsNullOrEmpty(company))
                        {
                            strQuery.Append(" and " + cQ);
                        }
                    }
                    else
                    if (!string.IsNullOrEmpty(lastName))
                    {
                        strQuery.Append(" and " + lQ);

                        if (!string.IsNullOrEmpty(company))
                        {
                            strQuery.Append(" and " + cQ);
                        }
                    }
                    else
                    if (!string.IsNullOrEmpty(company))
                    {
                        strQuery.Append(" and " + cQ);
                    }
                }
            }
            strQuery.Append(" Order by LastName,FirstName ");
            //return DbUtil.GetIdStringDictionary(strQuery.ToString(), false, null);
            return GetSortedStringIdDictionaryBySql(strQuery.ToString());
        }
        public static Dictionary<int, string> GetRolesbyGroupType(int GroupType)
        {
            return DbUtil.GetIdStringDictionary("select CommitteeRoleName as Name,CommitteeRoleId as Id from  v_CommitteeRolebyGroupType  where GroupTypeid=" + GroupType, false, null);
        }
        public static SortedDictionary<string, int> GetRolesbyGroupID(int GroupId)
        {
            string Sql = "select CommitteeRoleName as Name,CommitteeRoleId as Id  from [Group] g   inner join v_CommitteeRolebyGroupType cv" +
                         " on cv.GroupTypeId = g.grouptypeid where g.GroupId=" + GroupId + "  order by CommitteeRoleName ";
            return DbUtil.GetSortedStringIdDictionaryBySql(Sql);
        }

        public static bool GetIsCurrentUserAdmin(string editorUserName, string groupName)
        {
            bool role = true;
            // set up domain context
            //PrincipalContext ctx = new PrincipalContext(ContextType.Domain, "ata.org");
            //// find a user
            //UserPrincipal user = UserPrincipal.FindByIdentity(ctx, editorUserName);
            //// find the group in question
            //GroupPrincipal group = GroupPrincipal.FindByIdentity(ctx, groupName); //"ITSecurity"
            //if (user != null)
            //{  // check if user is member of that group
            //    if (user.IsMemberOf(group))
            //    {
            //        role = true;
            //    }
            //}
            return role;
        }

        //term : Provides the term to search for; CompanyId: Provides the company id, isActiveMB : Provides if it is active Member Portal, isAdmin : Provides if the current user belongs to IT; 
        public static Dictionary<int, string> GetAllGroupNameId(string term, int? CompanyId, bool isActiveMB, bool isAdmin, int CurrentUserId)
        {
            StringBuilder strQuery = new StringBuilder();
            term = term.Trim();
            SqlParameter[] parameters = new SqlParameter[0];
            try
            {
                if (isAdmin && (CompanyId != null && CompanyId > 0))
                {   //Return all groups because the current user requesting belongs to IT
                    parameters = new SqlParameter[4];
                    parameters[0] = new SqlParameter("@term", term);
                    parameters[1] = new SqlParameter("@isAdmin", isAdmin);
                    parameters[2] = new SqlParameter("@isActiveMB", isActiveMB);
                    parameters[3] = new SqlParameter("@CompanyId", CompanyId);
                }
                else
                if (CompanyId != null && CompanyId > 0)
                {
                    parameters = new SqlParameter[4];
                    parameters[0] = new SqlParameter("@term", term);
                    parameters[1] = new SqlParameter("@isActiveMB", isActiveMB);
                    parameters[2] = new SqlParameter("@CompanyId", CompanyId);
                    parameters[3] = new SqlParameter("@currentUserId", CurrentUserId);
                }
                else
                {
                    parameters = new SqlParameter[2];
                    parameters[0] = new SqlParameter("@term", term);
                    parameters[1] = new SqlParameter("@currentUserId", CurrentUserId);
                }
                return DbUtil.GetIdStringDictionary("p_UserGroup_GetbyUserCompany", true, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Error in Getting Groups for the user because {0}", e.ToString()));
            }
        }
        public static void CreateADContact(ATAGroup dr)
        {
            try
            {
                // create LDAP connection object
                DirectoryEntry myLdapConnection = createDirectoryEntry();
                createContact(dr, myLdapConnection);
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex, "Error in CreateADContact method while creating an AD contact for the new group");
            }
        }
        public static void CheckCreateADContact(ATAGroup dr)
        {
            try
            {   // Create LDAP connection object
                DirectoryEntry myLdapConnection = createDirectoryEntry();
                string Email = dr.LyrisListName.Trim().Replace(" ", "") + ConfigurationManager.AppSettings["LyrisEmailDomain"];
                DirectorySearcher search = new DirectorySearcher(myLdapConnection)
                {
                    SearchScope = SearchScope.Subtree,
                    Filter = "(&" +
                             "(objectClass=contact)" +
                             "(mail=" + Email.Trim() + ")" +
                             ")"
                };
                SearchResult result = search.FindOne();
                if (result == null) //Create the contact,if not already existing
                    createContact(dr, myLdapConnection);
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex, "Error in CreateADContact method while creating an AD contact for the new group");
            }
        }
        public static void DeleteADContact(ATAGroup dr)
        {
            try
            {   // Create LDAP connection object
                DirectoryEntry myLdapConnection = createDirectoryEntry();
                string Email = dr.LyrisListName.Trim().Replace(" ", "") + ConfigurationManager.AppSettings["LyrisEmailDomain"];
                DirectorySearcher search = new DirectorySearcher(myLdapConnection)
                {
                    SearchScope = SearchScope.Subtree,
                    Filter = "(&" +
                             "(objectClass=contact)" +
                             "(mail=" + Email.Trim() + ")" +
                             ")"
                };
                SearchResult result = search.FindOne();
                if (result != null)
                {
                    DirectoryEntry oDE = result.GetDirectoryEntry();
                    oDE.DeleteTree(); //Delete Group from AD
                }
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex, "Error in CreateADContact method while creating an AD contact for the new group");
            }
        }

        public static void createContact(ATAGroup row, DirectoryEntry myLdapConnection)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            string Email = row.LyrisListName.Trim().Replace(" ", "") + ConfigurationManager.AppSettings["LyrisEmailDomain"];
            string firstName = row.GroupName.Trim().Replace(" ", "");
            string lastName = row.GroupName.Trim().Replace(" ", "");
            {
                firstName = rgx.Replace(firstName, "");
                lastName = rgx.Replace(lastName, "");

                try
                {
                    DirectoryEntry oDE = myLdapConnection.Children.Add(string.Format("CN={0}", firstName), "contact");
                    SetProperty(oDE, "displayName", firstName);
                    SetProperty(oDE, "name", firstName);
                    SetProperty(oDE, "givenName", firstName);
                    SetProperty(oDE, "SN", firstName);
                    SetProperty(oDE, "mail", Email);
                    SetProperty(oDE, "proxyAddresses", string.Format("SMTP:{0}", Email));
                    oDE.CommitChanges();
                }
                catch (Exception ex)
                {
                    SendErrorMessage(ex, "Error in createContact method while creating an AD contact for the new group");
                }
            }
        }
        public static void SendErrorMessage(Exception e, String s)
        {
            System.Net.Mail.MailMessage mailMsg = new System.Net.Mail.MailMessage();
            mailMsg.From = new System.Net.Mail.MailAddress("UserManagement@airlines.org");
            mailMsg.To.Add(new System.Net.Mail.MailAddress("naamidala@airlines.org"));

            mailMsg.Subject = "An Error has occured in User Management!";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("<br/><br/>  An Error has occured at " + DateTime.Now.ToString() + "" + s);
            if (e != null)
            {
                sb.AppendLine();
                sb.AppendLine(" \n\nFollowing is the error: " + e.Message);
            }
            mailMsg.Body = sb.ToString();
            System.Net.Mail.SmtpClient _smtp = new System.Net.Mail.SmtpClient("mail.ata.org");
            _smtp.EnableSsl = false;
            _smtp.Send(mailMsg);
            _smtp = null;

        }
        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        private static void SetProperty(DirectoryEntry oDE, string PropertyName, string PropertyValue)
        {
            if ((PropertyValue != string.Empty) && (PropertyValue != null))
            {
                if (oDE.Properties.Contains(PropertyName))
                {
                    oDE.Properties[PropertyName][0] = PropertyValue;
                }
                else
                {
                    oDE.Properties[PropertyName].Value = PropertyValue;
                }
            }
        }
        private static void SetProperty(DirectoryEntry oDE, string PropertyName, string PropertyValue, bool addAnotherValue)
        {
            if ((PropertyValue != string.Empty) && (PropertyValue != null))
            {
                if (oDE.Properties.Contains(PropertyName))
                {
                    oDE.Properties[PropertyName].Add(PropertyValue);
                }
                else
                {
                    oDE.Properties[PropertyName].Value = PropertyValue;
                }
            }
        }

        static DirectoryEntry createDirectoryEntry()
        {
            // create and return new LDAP connection with desired settings

            // DirectoryEntry ldapConnection = new DirectoryEntry(string.Format("LDAP://OU={0},{1}", ConfigurationManager.AppSettings["OUName"], ConfigurationManager.AppSettings["OUPath"]), ConfigurationManager.AppSettings["ADUser"], ConfigurationManager.AppSettings["ADPassword"], AuthenticationTypes.Secure);
            DirectoryEntry ldapConnection = new DirectoryEntry(string.Format("LDAP://{0}", ConfigurationManager.AppSettings["ADPath"]), ConfigurationManager.AppSettings["ADUser"], ConfigurationManager.AppSettings["ADPassword"], AuthenticationTypes.Secure);
            return ldapConnection;
        }
        public static SortedDictionary<string, int> GetSortedStringIdDictionaryBySql(string sql)
        {
            return GetSortedStringIdDictionary(sql, false, null);
        }

        public static SortedDictionary<string, int> GetSortedStringIdDictionary(string sql, bool isStoreProcedure, params SqlParameter[] commandParameters)
        {
            CommandType commandType = isStoreProcedure ? CommandType.StoredProcedure : CommandType.Text;
            SortedDictionary<string, int> sorted = new SortedDictionary<string, int>();
            using (SqlDataReader reader = SqlHelper.ExecuteReader(Conf.ConnectionString, commandType, sql, commandParameters))
            {
                while (reader.Read())
                {
                    sorted[reader[0].ToString()] = reader.GetInt32(1);
                }
            }
            return sorted;
        }

        public static Dictionary<int, string> GetIdStringDictionary(string sql, bool isStoreProcedure, params SqlParameter[] commandParameters)
        {
            Dictionary<int, string> IdNameMapping = new Dictionary<int, string>();
            CommandType commandType = isStoreProcedure ? CommandType.StoredProcedure : CommandType.Text;
            using (SqlDataReader reader = SqlHelper.ExecuteReader(Conf.ConnectionString, commandType, sql, commandParameters))
            {
                while (reader.Read())
                {
                    IdNameMapping[reader.GetInt32(0)] = reader.GetString(1);
                }
            }
            return IdNameMapping;
        }

        public static List<string> GetExemptedLyrisEmailAddressesInUpperCase()
        {
            try
            {
                string sql = @"SELECT UPPER(EmailAddress) FROM [dbo].[EmailsOnlyManagedInLyris]";
                List<string> exemptedEmailsInUpperCase = new List<string>();
                using (SqlDataReader reader = SqlHelper.ExecuteReader(Conf.ConnectionString, CommandType.Text, sql, null))
                {
                    while (reader.Read())
                    {
                        exemptedEmailsInUpperCase.Add(reader.GetString(0));
                    }
                }
                return exemptedEmailsInUpperCase;
            }
            catch (Exception e)
            {
                Log.LogError(string.Format("Error in GetExemptedLyrisEmailAddresses because {0}", e.ToString()));
                throw;
            }
        }
        public static void ChangeUserName(int userId, string userName)
        {
            //Check user name before update
            string sql = "select 1 from [User] where  Lower([Username])=@Username";
            SqlParameter[] parameters = new SqlParameter[1];
            parameters[0] = new SqlParameter("@Username", userName.ToLower());
            object o = SqlHelper.ExecuteScalar(Conf.ConnectionString, CommandType.Text, sql, parameters);
            if (o != null)
                throw new Exception(userName + " already used in User table.");

            sql = "Update [User] set Username = @Username where  UserId=@UserId";
            parameters = new SqlParameter[2];
            parameters[0] = new SqlParameter("@Username", userName);
            parameters[1] = new SqlParameter("@UserId", userId);
            SqlHelper.ExecuteNonQuery(Conf.ConnectionString, CommandType.Text, sql, parameters);
        }
        public static void UpdateCompanyId(int oldComanyId, int newCompanyId)
        {
            string sql = "Update [UserCompany] set CompanyId = @newCompanyId where  CompanyId=@oldCompanyId";
            SqlParameter[] parameters = new SqlParameter[2];
            parameters[0] = new SqlParameter("@newCompanyId", newCompanyId);
            parameters[1] = new SqlParameter("@oldCompanyId", oldComanyId);
            SqlHelper.ExecuteNonQuery(Conf.ConnectionString, CommandType.Text, sql, parameters);
        }
        #region User Sponsor group related
        public static List<int> GetGetSponsoredUserIds(int userId)
        {
            try
            {
                SqlParameter[] sqlParams = new SqlParameter[2];
                sqlParams[0] = new SqlParameter("@UserId", userId);
                sqlParams[1] = AppliesToSiteIdSqlParameter;
                return GetIdsFromDatabase("p_User_GetSponsoredUserIds", CommandType.StoredProcedure, sqlParams);
            }
            catch (Exception e)
            {
                Log.LogError(string.Format("Error in GetGetSponsoredUserIds for user id {0} because {1}", userId, e.ToString()));
                throw;
            }
        }
        public static List<int> GetSponsorGroupIdsForUser(int userId)
        {
            try
            {
                SqlParameter[] sqlParams = new SqlParameter[1];
                sqlParams[0] = new SqlParameter("@UserId", userId);
                string sql = @"SELECT distinct SponsorGroupId  FROM UserToSponsorGroup WHERE UserId=@UserId";
                return GetIdsFromDatabase(sql, CommandType.Text, sqlParams);
            }
            catch (Exception e)
            {
                Log.LogError(string.Format("Error in GetSponsorGroupIdsForUser for user id {0} because {1}", userId, e.ToString()));
                throw;
            }
        }
        public static List<int> GetMySponsorGroupsByEditUser(int userId)
        {
            try
            {
                SqlParameter[] sqlParams = new SqlParameter[1];
                sqlParams[0] = new SqlParameter("@UserId", userId);
                string sql = @"SELECT distinct SponsorGroupId  FROM SponsorGroupUser WHERE UserId=@UserId";
                return GetIdsFromDatabase(sql, CommandType.Text, sqlParams);
            }
            catch (Exception e)
            {
                Log.LogError(string.Format("Error in GetSponsorGroupIdsForUser for user id {0} because {1}", userId, e.ToString()));
                throw;
            }
        }
        #endregion 
        public static List<int> GetSecurityGroupIds()
        {
            try
            {
                List<int> ids = new List<int>();
                string sql = @"SELECT GroupId FROM dbo.[Group] WHERE IsSecurityGroup=1 OR IsCommittee=1";
                return GetIdsFromDatabase(sql, CommandType.Text, null);
            }
            catch (Exception e)
            {
                Log.LogError("Error in GetSecurityGroupIds because: " + e.ToString());
                throw;
            }
        }
        public static Dictionary<int, int> GetSelectedGroupsWithCommitteeRole(int userId)
        {
            try
            {
                Dictionary<int, int> ids = new Dictionary<int, int>();
                SqlParameter[] sqlParams = new SqlParameter[1];
                sqlParams[0] = new SqlParameter("@UserId", userId);
                string sql = "SELECT u.GroupId, u.CommitteeRoleId FROM [Group] g INNER JOIN UserGroup u ON g.GroupId = u.GroupId WHERE g.IsChildGroup=0 AND u.UserId=@UserId"
                + " UNION  SELECT parent.GroupId, 5  FROM [Group] g INNER JOIN UserGroup u ON g.GroupId = u.GroupId  INNER JOIN [Group] parent ON g.ParentGroupId = parent.GroupId  WHERE g.IsChildGroup=1 AND  u.UserId=@UserId";
                int groupId = -1;
                int committeeRoleId = -1;
                using (SqlDataReader reader = SqlHelper.ExecuteReader(Conf.ConnectionString, CommandType.Text, sql, sqlParams))
                {
                    while (reader.Read())
                    {
                        groupId = reader.GetInt32(0);
                        committeeRoleId = reader.IsDBNull(1) ? -1 : reader.GetInt32(1);
                        ids[groupId] = committeeRoleId;
                    }
                }
                return ids;
            }
            catch (Exception e)
            {
                Log.LogError(string.Format("Error in GetSelectedGroupsWithCommitteeRole for user id {0} because {1}", userId, e.ToString()));
                throw;
            }
        }
        public static List<int> GetGroupSelectedATAMemberIds(int groupId)
        {
            return GetGroupSelectedMemberIds(groupId, true);
        }
        public static List<int> GetGroupSelectedNonATAMemberIds(int groupId)
        {
            return GetGroupSelectedMemberIds(groupId, false);
        }
        /// <summary>
        /// Gets all ata staff.
        /// </summary>
        /// <returns></returns>
        public static DataObjectList<LookupUser> GetAllAtaStaff()
        {
            SqlParameter[] sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter("@SelectingUserId", -1);
            return new DataObjectList<LookupUser>(sqlParams, SELECT_ATA_STAFF_USERS_PROC);
        }
        public static DataObjectList<LookupUser> GetAllContacts()
        {
            SqlParameter[] sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter("@SelectingUserId", -1);
            return new DataObjectList<LookupUser>(sqlParams, SELECT_ALL_CONTACT_USERS_PROC);
        }
        public static SortedDictionary<string, int> GetLyrisSend()
        {
            string sql = "SELECT Value,Id FROM [LyrisSend]";
            return DbUtil.GetSortedStringIdDictionaryBySql(sql);
        }

        #region Fuels 
        public static DataObjectList<ATAGroupManager.GroupUser> GetAllFuelsAirlinesAndOperators()
        {
            return GetFuelsNonpublicUsers(-1);//-1 will retrieve all users
        }

        public static bool IsFuelsNonpublicUser(int userId)
        {
            DataObjectList<ATAGroupManager.GroupUser> users = GetFuelsNonpublicUsers(userId);
            return users.Count > 0;
        }

        public static DataObjectList<ATAGroupManager.GroupUser> GetFuelsNonpublicUsers(int userId)
        {
            SqlParameter[] sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter("@UserId", userId);
            return new DataObjectList<ATAGroupManager.GroupUser>(sqlParams, Fuels_GetNonpublicUsers);
        }
        #endregion

        public static DataObjectList<ATAGroup> GetTopLevelGroups()
        {
            SqlParameter[] sqlParams = new SqlParameter[1];
            sqlParams[0] = AppliesToSiteIdSqlParameter;
            return new DataObjectList<ATAGroup>(sqlParams, "p_Group_GetTopGroups");
        }

        public static string ConvertIdsToSqlIn(List<int> ids)
        {
            StringBuilder sb = new StringBuilder(" IN (");
            bool isFirst = true;
            foreach (int id in ids)
            {
                if (isFirst)
                    isFirst = false;
                else
                    sb.Append(", ");
                sb.Append(id);
            }
            sb.Append(") ");
            return sb.ToString();
        }

        public static Dictionary<int, int> GetUserCompanyIdMapByUserIds(List<int> ids)
        {
            Dictionary<int, int> userToCompanyMap = new Dictionary<int, int>();
            string sql = "SELECT u.UserId, c.CompanyId FROM [User] u INNER JOIN UserCompany c ON u.UserId = c.UserId WHERE u.UserId" + ConvertIdsToSqlIn(ids);
            int userId;
            int companyId;
            using (SqlDataReader reader = SqlHelper.ExecuteReader(Conf.ConnectionString, CommandType.Text, sql, null))
            {
                while (reader.Read())
                {
                    userId = reader.GetInt32(0);
                    companyId = reader.IsDBNull(1) ? -1 : reader.GetInt32(1);
                    if (companyId > 0)
                        userToCompanyMap[userId] = companyId;
                }
            }
            return userToCompanyMap;
        }

        public static int GetUserCompanyIdByUserId(int userId)
        {
            Dictionary<int, int> userToCompanyMap = new Dictionary<int, int>();
            string sql = "select c.CompanyId FROM UserCompany c where c.UserId = " + userId;
            object o = SqlHelper.ExecuteScalar(Conf.ConnectionString, CommandType.Text, sql);
            if (o != null)
                return (int)o;
            return -1;
        }
        public static string GetUserNamebyUserId(int userid)
        {
            string o = string.Empty;
            try
            {
                Dictionary<int, int> userToCompanyMap = new Dictionary<int, int>();
                string sql = "select c.CompanyId FROM UserCompany c where c.UserId = " + userid;
                o = (string)SqlHelper.ExecuteScalar(Conf.ConnectionString, CommandType.Text, sql);
            }
            catch { }
            return o;
        }

        public static DataTable GetUserGroupsbyUserId(int userid)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("p_UserGroup_GetList", con))
                {
                    con.Open();
                    if (userid > 0)
                    {
                        SqlParameter[] spm = new SqlParameter[1];
                        spm[0] = new SqlParameter("userid", userid); //Userid was given
                        cmd.Parameters.AddRange(spm);
                    }

                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    return dt;
                }
            }

        }
        public static string GetGroupNameByGroupId(int groupId)
        {
            return GetNameById(groupId, "@groupId", "p_Group_Get_GroupName");
        }
        public static int CheckifGroupAlreadyExists(ATAGroup grp)
        {
            string sql = "SELECT GroupId FROM [Group] WHERE GroupName ='" + grp.GroupName + "'";
            object o = SqlHelper.ExecuteScalar(Conf.ConnectionString, CommandType.Text, sql);
            if (o != null)
                return (int)o;
            else
                return 0;
        }

        public static int CheckifGroupEmailAlreadyExists(ATAGroup grp)
        {
            string sql = "SELECT GroupId FROM [Group] WHERE LOWER(LyrisListName) = '" + grp.LyrisListName.ToLower() + "'";
            object o = SqlHelper.ExecuteScalar(Conf.ConnectionString, CommandType.Text, sql);
            if (o != null)
                return (int)o;
            else
                return 0;
        }
        public static List<int> GetGroupMemberIdsByCommitteeRoleId(int groupId, int committeeRoleId)
        {
            try
            {
                SqlParameter[] sqlParams = new SqlParameter[2];
                sqlParams[0] = new SqlParameter("@GroupId", groupId);
                sqlParams[1] = new SqlParameter("@CommitteeRoleId", committeeRoleId);
                return GetIdsFromDatabase("p_GroupGetMemberIdsByCommitteeRoleId", CommandType.StoredProcedure, sqlParams);
            }
            catch (Exception e)
            {
                Log.LogError(string.Format("Error in GetGroupMemberIdsByCommitteeRole for group {0} and committeeRoleId {1} because {2}",
                    groupId, committeeRoleId, e.ToString()));
                throw;
            }
        }

        public static List<int> GetGroupAlternateOrPrimaryMemberIdsByCompany(int groupId, int companyId, bool isAlternate)
        {
            try
            {
                SqlParameter[] sqlParams = new SqlParameter[3];
                sqlParams[0] = new SqlParameter("@GroupId", groupId);
                sqlParams[1] = new SqlParameter("@CompanyId", companyId);
                sqlParams[2] = new SqlParameter("@IsAlternate", isAlternate);
                return GetIdsFromDatabase("p_Group_GetAlternateOrPrimaryUserIdsByCompany", CommandType.StoredProcedure, sqlParams);
            }
            catch (Exception e)
            {
                Log.LogError(string.Format("Error in GetGroupMemberIdsByCompanyAndCommitteeRole for group {0} and company {1} because {2}",
                    groupId, companyId, e.ToString()));
                throw;
            }
        }


        public static DataTable ExecuteDataTable(string storeProcedureName, params object[] parameterValues)
        {
            DataSet dataSet = SqlHelper.ExecuteDataset(Conf.ConnectionString, storeProcedureName, parameterValues);
            if (dataSet != null && dataSet.Tables.Count > 0)
                return dataSet.Tables[0];
            return null;

        }

        public static string GetNameById(int id, string idParamName, string storedProcedureName)
        {
            return GetNameById(id, idParamName, storedProcedureName, false);
        }
        public static string GetNameById(int id, string idParamName, string sql, bool isSql)
        {
            try
            {
                SqlParameter[] sqlParams = new SqlParameter[1];
                sqlParams[0] = new SqlParameter(idParamName, id);
                CommandType commandType = isSql ? CommandType.Text : CommandType.StoredProcedure;
                return (string)SqlHelper.ExecuteScalar(Conf.ConnectionString, commandType, sql, sqlParams);
            }
            catch (Exception e)
            {
                Log.LogError(string.Format("Error in running stored procedure {0} for id of {1} because {2}", sql, id, e.ToString()));
                throw;
            }
        }

        public static void ExecuteNonQuery(string sql)
        {
            try
            {
                SqlHelper.ExecuteNonQuery(Conf.ConnectionString, CommandType.Text, sql);
            }
            catch (Exception e)
            {
                Log.LogError(string.Format("Error in ExecuteNonQuery {0} because {1}", sql, e.ToString()));
                throw;
            }
        }
        public static List<Dictionary<string, object>> DataTabletoDictionary(DataTable dt)
        {
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
            return rows;
        }

        public static SqlParameter AppliesToSiteIdSqlParameter { get { return new SqlParameter("@AppliesToSiteId", 3/*#BYNA (int)ATAMembershipUtility.Instance.CurrentSite*/); } }

        #region private methods 
        private static List<int> GetGroupSelectedMemberIds(int groupId, bool isATAStaff)
        {
            try
            {
                SqlParameter[] sqlParams = new SqlParameter[1];
                sqlParams[0] = new SqlParameter("@GroupId", groupId);
                string sql = @"SELECT u.UserId FROM  [User] u JOIN UserGroup g ON g.UserId = u.UserId WHERE GroupId = @GroupId AND IsATAStaff ";
                sql = sql + ((isATAStaff) ? "= 1" : "<> 1");
                return GetIdsFromDatabase(sql, CommandType.Text, sqlParams);
            }
            catch (Exception e)
            {
                Log.LogError(string.Format("Error in GetGroupSelectedMemberIds for group id {0} because {1}", groupId, e.ToString()));
                throw;
            }
        }

        public static List<int> GetIdsFromReader(SqlDataReader reader)
        {
            List<int> ids = new List<int>();
            while (reader.Read())
            {
                ids.Add(reader.GetInt32(0));
            }
            return ids;
        }

        public static List<int> GetIdsFromDatabase(string sql, CommandType commandType, SqlParameter[] sqlParams)
        {
            List<int> ids = new List<int>();
            using (SqlDataReader reader = SqlHelper.ExecuteReader(Conf.ConnectionString, commandType, sql, sqlParams))
            {
                ids = GetIdsFromReader(reader);
            }
            return ids;
        }
        #endregion
    }
}
