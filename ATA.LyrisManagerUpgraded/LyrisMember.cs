using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ATA.Member.Util;
using Microsoft.ApplicationBlocks.Data;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ATA.LyrisProxy
{
    public class LyrisMember
    {
        #region Constants
        private const string ATAGetListNamesByMemberEmail = "P_ATA_GetListNamesByMemberEmail"; 
        private const string ATADeleteMemberByEmailAndListName = "P_ATA_DeleteMemberByEmailAndListName";
        private const string ATADeleteMemberFromAllLists = "P_ATA_DeleteMemberFromAllLists";
        private const string ATAAddMemberToList = "P_ATA_AddMemberToList";
        private const string ATAUpdateListAdmin = "P_ATA_UpdateListAdmin"; 
        private const string ATAGetMembersByListName_EmailOptional = "P_ATA_GetMembersByListName_EmailOptional";
        private const string ATAGroupUpdateLyrisSend = "P_ATA_GroupUpdateLyrisSend";

        private const string lyrisDBTrueValue = "T";
        private const string lyrisDBFalseValue = "F";
        class Column
        {
            public const string EmailAddress = "EmailAddr_";
            public const string IsListAdmin = "IsListAdm_";
            public const string RcvAdmMail = "RcvAdmMail_";
            public const string NotifySubModificagtion = "NotifySubm_"; 
        }
        
        private const string lyrisDBConnKey = "LyrisListDb"; 

        #endregion

        #region Constructors
        public LyrisMember()
        {
            this.IsAdmin = false;
            this.ReceiveAdminEmail = false;
            this.NoMail = false;//default receive email
        }

        public LyrisMember(string emailAddress) : this()
        {
            this.EmailAddress = emailAddress;
        }

        public LyrisMember(string emailAddress, string fullName) : this(emailAddress) 
        {
            this.FullName = fullName;
        }
        #endregion 

        #region properties
        public string EmailAddress { get; set; }
        public string FullName { get; set; } 
        public bool IsAdmin { get; set; }
        public bool NoMail { get; set; } 

        //Receive Admin Email is called Bounce Admin by ATA. maps to RcvAdmMail_
        public bool ReceiveAdminEmail { get; set; }

        //maps to NotifySubm_
        //public bool ReceiveModerationNotification { get; set; }
        #endregion  
        
        #region Public  query Methods 
        public static bool IsEmailAddressListAdmin(string listName, string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;
            List<LyrisMember> members = GetListMembersByListName_emailOptional(listName, email);
            if (members.Count > 0 && members[0].IsAdmin)
                return true;
            return false;
        }

        public static List<LyrisMember> GetListMembersByListName(string listName)
        {
            return GetListMembersByListName_emailOptional(listName, null);
        }   

        public static List<LyrisMember> GetListMembersByListName_emailOptional(string listName, string email)
        {
            try
            {
                bool hasEmail = !string.IsNullOrEmpty(email);
                SqlParameter[] parameters = (hasEmail) ? new SqlParameter[3] : new SqlParameter[1];
                AddListNameToParameters(listName, parameters, 0);//list name
                if(hasEmail)
                    AddUserNameAndDomainToParameters(email, parameters, 1);
                List<LyrisMember> listMembers = new List<LyrisMember>();
                using (SqlDataReader reader = SqlHelper.ExecuteReader(lyrisConnStr, CommandType.StoredProcedure, ATAGetMembersByListName_EmailOptional, parameters))
                {
                    while (reader.Read())
                    {
                        LyrisMember lyrisMember = new LyrisMember();
                        lyrisMember.EmailAddress = reader[Column.EmailAddress].ToString();
                        lyrisMember.IsAdmin = GetLyrisBoolFromReader(reader, Column.IsListAdmin);
                        lyrisMember.ReceiveAdminEmail = GetLyrisBoolFromReader(reader, Column.RcvAdmMail);
                        lyrisMember.NoMail = Convert.ToBoolean(reader["NoMail"]);//"NoMail" is a column alians in the stored procedure  
                        //lyrisMember.ReceiveModerationNotification = GetLyrisBoolFromReader(reader, Column.NotifySubModificagtion); 
                        listMembers.Add(lyrisMember);
                    }
                }
                return listMembers;
            }
            catch (Exception e)
            {
                //Log.LogError(string.Format("Error in GetListMembersListName_emailOptional for list {0} because {1}", listName, e.ToString()));
                throw;
            }
        }  

        public static List<string> GetListNamesByMemberEmail(string emailAddress)
        { 
            try
            {
                SqlParameter[] parameters = new SqlParameter[2];
                AddUserNameAndDomainToParameters(emailAddress, parameters, 0);
                return GetStringList(ATAGetListNamesByMemberEmail, parameters);
            }
            catch (Exception e)
            {
                //Log.LogError(string.Format("Error in GetListNamesByMemberEmail for email {0} because {1}", emailAddress, e.ToString()));
                throw;
            }
        }
 
        #endregion

        #region Update methods
        private static Dictionary<string, LyrisMember> IndexByEmail(List<LyrisMember> members)
        {
            Dictionary<string, LyrisMember> mapping = new Dictionary<string,LyrisMember>();
            foreach (LyrisMember member in members)
            {
               if(!string.IsNullOrEmpty(member.EmailAddress)){
                   mapping[member.EmailAddress.ToLower().Trim()] = member;
               }
            }
            return mapping;
        }
        public static void OverWriteListMembers(List<LyrisMember> sourceMembers, string listName, bool overWriteAdmin)
        {
            LyrisMember.OverWriteListMembers(sourceMembers, listName, overWriteAdmin, null);
        }

        public static void OverWriteListMembers(List<LyrisMember> sourceMembers, string listName, bool overWriteAdmin, List<string> exemptedEmailsInUpperCase)
        {
            List<LyrisMember> existingMembers = GetListMembersByListName(listName);
            Dictionary<string, LyrisMember> existingMembersMap = IndexByEmail(existingMembers);
            Dictionary<string, LyrisMember> sourceMembersMap = IndexByEmail(sourceMembers); 
            foreach (string sourceEmail in sourceMembersMap.Keys)
            {
                //Leave it alone if it is exempted
                if (exemptedEmailsInUpperCase != null && exemptedEmailsInUpperCase.Contains(sourceEmail.ToUpper()))
                    continue; 

                LyrisMember sourceMember = sourceMembersMap[sourceEmail];
                if (!existingMembersMap.ContainsKey(sourceEmail))
                {
                    AddMemberToList(sourceMember, listName);
                }
                else//match
                {
                    LyrisMember matchingOne = existingMembersMap[sourceEmail];
                    existingMembersMap.Remove(sourceEmail);//remove the matching item
                    if (overWriteAdmin && 
                        (matchingOne.IsAdmin != sourceMember.IsAdmin 
                        || sourceMember.ReceiveAdminEmail != matchingOne.ReceiveAdminEmail
                        || sourceMember.NoMail != matchingOne.NoMail))
                    {
                        UpdateListAdmin(sourceMember.EmailAddress, listName, sourceMember.IsAdmin, sourceMember.ReceiveAdminEmail, sourceMember.NoMail); 
                    } 
                }
            }

            foreach (string existingEmail in existingMembersMap.Keys)
            {
                //Leave it alone if it is exempted
                if (exemptedEmailsInUpperCase != null && exemptedEmailsInUpperCase.Contains(existingEmail.ToUpper()))
                    continue; 

                LyrisMember existingMember = existingMembersMap[existingEmail]; 
                
                //leave it alone if it is admin and overWriteAdmin is false
                if (!overWriteAdmin && (existingMember.IsAdmin || existingMember.ReceiveAdminEmail))
                    continue;// 
                
                //TODO: Do we need to remove the admin (call update admin) before delete the member in case of admin? 
                DeleteMemberFromList(existingMember.EmailAddress, listName);
            } 
        }

        public static bool RemoveAdminfromList(string groupname)
        { 
            string email = ConfigurationManager.AppSettings["SusQTechLyrisManagerEmail"];

            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(groupname))
            {
                LyrisMember.DeleteMemberFromList(email, groupname); 
            }
            else
                return false;
            return true;
        }
        public static void DeleteMemberFromList(string emailAddress, string listName)
        {
            try
            {
                SqlParameter[] parameters = new SqlParameter[3];
                AddUserNameAndDomainToParameters(emailAddress, parameters, 0);
                AddListNameToParameters(listName, parameters, 2);
                SqlHelper.ExecuteNonQuery(lyrisConnStr, CommandType.StoredProcedure, ATADeleteMemberByEmailAndListName, parameters);
            }
            catch (Exception e)
            {
                //Log.LogError(string.Format("Error in DeleteMemberFromList for email {0}  and list {1} because {2}",     emailAddress, listName, e.ToString()));
                throw;
            }
        }

        public static void DeleteMemberFromAllLists(string emailAddress)
        {
            try
            {
                SqlParameter[] parameters = new SqlParameter[2];
                AddUserNameAndDomainToParameters(emailAddress, parameters, 0);
                SqlHelper.ExecuteNonQuery(lyrisConnStr, CommandType.StoredProcedure, ATADeleteMemberFromAllLists, parameters);
            }
            catch (Exception e)
            {
                //Log.LogError(string.Format("Error in DeleteMemberFromAllLists for email {0} because {1}", emailAddress,  e.ToString()));
                throw;
            }
        }

        public static void AddMemberToList(LyrisMember lyrisMember, string listName)
        {
            AddMemberToList(lyrisMember.EmailAddress, lyrisMember.FullName, listName);
            if (lyrisMember.IsAdmin || lyrisMember.ReceiveAdminEmail)
            {
                UpdateListAdmin(lyrisMember.EmailAddress, listName, lyrisMember.IsAdmin, lyrisMember.ReceiveAdminEmail, lyrisMember.NoMail);
            } 
        }

        public static void AddMemberToList(string emailAddress, string fullName, string listName)
        {
            try
            {
                SqlParameter[] parameters = new SqlParameter[5];
                AddUserNameAndDomainToParameters(emailAddress, parameters, 0);
                AddListNameToParameters(listName, parameters, 2);
                parameters[3] = new SqlParameter("@UserEmail", emailAddress);
                parameters[4] = new SqlParameter("@FullName", fullName);
                SqlHelper.ExecuteNonQuery(lyrisConnStr, CommandType.StoredProcedure, ATAAddMemberToList, parameters);
            }
            catch (Exception e)
            {
                //Log.LogError(string.Format("Error in DeleteMemberFromList for email {0}  and list {1} because {2}", emailAddress, listName, e.ToString()));

                throw;
            }
        }

        public static void ChangeUserEmail(string oldEmailAddress, string newEmailAddress)
        {
            try
            {
                string sql = "Update members_ set EmailAddr_ = @UserEmail, Domain_=@newDomainLC , UserNameLC_=@newUserNameLC where  Domain_=@DomainLC and UserNameLC_ = @UserNameLC";  
                SqlParameter[] parameters = new SqlParameter[5]; 
                parameters[0] = new SqlParameter("@UserEmail", newEmailAddress);
                AddUserNameAndDomainToParameters(newEmailAddress, parameters, 1, "@newDomainLC", "@newUserNameLC");
                AddUserNameAndDomainToParameters(oldEmailAddress, parameters, 3);

                SqlHelper.ExecuteNonQuery(lyrisConnStr, CommandType.Text, sql, parameters);
            }
            catch (Exception e)
            {
                //Log.LogError(string.Format("Error in ChangeUserEmail email from {0} to {1} because {2}", oldEmailAddress, newEmailAddress, e.ToString()));
                throw;
            }
        }

        //private const string emailAddressPattern = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
        //private static bool IsValidEmailAddress(string email)
        //{
        //    System.Text.RegularExpressions.Regex regEx = new System.Text.RegularExpressions.Regex(emailAddressPattern);
        //    if (regEx.IsMatch(email))
        //        return true;
        //    return false;
        //} 
        public static void UpdateLyrisSend(  string listName, int lyrisSend)
        {
            try
            {
                SqlParameter[] parameters = new SqlParameter[2]; 
                AddListNameToParameters(listName, parameters, 0);
                parameters[1] = new SqlParameter("@Lyrissend", lyrisSend); 
                SqlHelper.ExecuteNonQuery(lyrisConnStr, CommandType.StoredProcedure, ATAGroupUpdateLyrisSend, parameters);
            }
            catch (Exception e)
            {
                //Log.LogError(string.Format("Error in UpdateListAdmin for email {0}  and list {1} because {2}", emailAddress, listName, e.ToString()));
                throw;
            }
        }

        public static void UpdateListAdmin(string emailAddress, string listName, bool isAdmin, bool isBounceAdmin, bool noMail)
        {
            try
            {
                SqlParameter[] parameters = new SqlParameter[7];
                AddUserNameAndDomainToParameters(emailAddress, parameters, 0);
                AddListNameToParameters(listName, parameters, 2);
                parameters[3] = new SqlParameter("@IsListAdm", ChangeBoolToLyrisDbBoolValue(isAdmin));
                parameters[4] = new SqlParameter("@RcvAdmMail", ChangeBoolToLyrisDbBoolValue(isBounceAdmin));
                parameters[5] = new SqlParameter("@NotifySubm", ChangeBoolToLyrisDbBoolValue(isAdmin));
                parameters[6] = new SqlParameter("@NoMail", noMail);
                SqlHelper.ExecuteNonQuery(lyrisConnStr, CommandType.StoredProcedure, ATAUpdateListAdmin, parameters);
            }
            catch (Exception e)
            {
                //Log.LogError(string.Format("Error in UpdateListAdmin for email {0}  and list {1} because {2}", emailAddress, listName, e.ToString()));
                throw;
            }
        } 
      #endregion

        #region Util method

        private static string ChangeBoolToLyrisDbBoolValue(bool boolValue)
        {
            return boolValue ? lyrisDBTrueValue : lyrisDBFalseValue;
        }

        private static bool GetLyrisBoolFromReader(SqlDataReader reader, string columName)
        {
            object o = reader[columName];
            if (o != null)
            {
                string result = o.ToString().Trim();
                if (string.Equals(lyrisDBTrueValue, result, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        private static List<string> GetStringList(string storedProcedureName, SqlParameter[] parameters)
        {
            List<string> listNames = new List<string>();
            using (SqlDataReader reader = SqlHelper.ExecuteReader(lyrisConnStr, CommandType.StoredProcedure, storedProcedureName, parameters))
            {
                while (reader.Read())
                {
                    listNames.Add(reader.GetString(0));
                }
            }
            return listNames;
        }

        private static void AddUserNameAndDomainToParameters(string emailAddress, SqlParameter[] parameters, int startIndex)
        { 
            AddUserNameAndDomainToParameters(emailAddress, parameters, startIndex, "@DomainLC", "@UserNameLC");
        }

        private static void AddUserNameAndDomainToParameters(string emailAddress, SqlParameter[] parameters, int startIndex, string domainParamName, string userNameParamName)
        {
            string userNameLC, domainLC;
            SplitEmailForLyrisDB(emailAddress, out userNameLC, out domainLC);
            parameters[startIndex] = new SqlParameter(domainParamName, domainLC);
            parameters[startIndex + 1] = new SqlParameter(userNameParamName, userNameLC);
        }

        private static void AddListNameToParameters(string listName, SqlParameter[] parameters, int startIndex)
        {
            parameters[startIndex] = new SqlParameter("@listName", listName);  
        }

        private static void SplitEmailForLyrisDB(string email, out string userNameLC, out string domainLC)
        {
            string lcEmail = email.ToLower().Trim();
            int index = lcEmail.IndexOf("@");
            if (index < 1 || index > (lcEmail.Length - 3))
                throw new InvalidOperationException(email + " is not a valid email address.");
            userNameLC = lcEmail.Substring(0, index);
            domainLC = lcEmail.Substring(index + 1);
        }
        #endregion

        #region Connection string related code
        private static readonly string _lyrisConnStr;
        private static readonly string _lyrisConnStrError = null;
        static LyrisMember()
        {
            ConnectionStringSettings listConSettig = ConfigurationManager.ConnectionStrings[lyrisDBConnKey];
            if (listConSettig == null)
            {
                _lyrisConnStrError = lyrisDBConnKey + " connection string is missing in Web.Conf.";
                //Log.LogError(_lyrisConnStrError); 
            }
            _lyrisConnStr = listConSettig.ConnectionString;
        }

        static internal string lyrisConnStr
        {
            get
            {
                if (_lyrisConnStrError != null)
                    throw new Exception(_lyrisConnStrError);
                return _lyrisConnStr;
            }
        }
        #endregion
    }
}
