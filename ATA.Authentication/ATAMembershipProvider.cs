#region namespaces

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.Profile;
using ATA.Authentication;
using Microsoft.ApplicationBlocks.Data;
using SusQTech.Data.DataObjects;
using SusQTech.Utility;

#endregion

namespace ATA.Authentication.Providers
{
    public class ATAMembershipProvider : MembershipProvider
    {
        #region Private Members

        private System.Collections.Specialized.NameValueCollection _config;

        #endregion

        #region Initialize

        /// <summary></summary>
        /// <param name="name"></param>
        /// <param name="config"></param>
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            _config = config;
            base.Initialize(name, config);
        }

        #endregion

        #region ATA specific methods

        #region CreateATAUser
        public ATAMembershipUser CreateATAUser(string username, string password, string email, object providerUserKey,
            out MembershipCreateStatus status, int ReportsToId, string Prefix, string FirstName, string MiddleName,
            string LastName, string Suffix, string HomePhone, string OfficePhone, string MobilePhone, string PrimaryFax,
            string TopicsOfInterest, DateTime ExpirationDate, bool IsActiveMB, bool IsActiveFF, bool IsActiveACH, bool IsATAStaff,
            bool IsAcceptedLicense, bool IsPrivate, string Email2, int CreatingUserId, string PreferredName,
            bool RequiresPasswordChange, string address1, string address2, string city, string state, string zipcode, string province, string country) 
        {
            return CreateATAUser(username, password, email, providerUserKey,
            out status, ReportsToId, Prefix, FirstName, MiddleName,
             LastName, Suffix, HomePhone, OfficePhone, MobilePhone, PrimaryFax,
             TopicsOfInterest, ExpirationDate, IsActiveMB, IsActiveFF, IsActiveACH, IsATAStaff,
             IsAcceptedLicense, IsPrivate, Email2, CreatingUserId, PreferredName,
             RequiresPasswordChange, true,address1,address2,city,state,zipcode,province,country);
        }

        public ATAMembershipUser CreateATAUser(string username, string password, string email, object providerUserKey,
            out MembershipCreateStatus status, int ReportsToId, string Prefix, string FirstName, string MiddleName,
            string LastName, string Suffix, string HomePhone, string OfficePhone, string MobilePhone, string PrimaryFax,
            string TopicsOfInterest, DateTime ExpirationDate, bool IsActiveMB, bool IsActiveFF, bool IsActiveACH, bool IsATAStaff,
            bool IsAcceptedLicense, bool IsPrivate, string Email2, int CreatingUserId, string PreferredName,
            bool RequiresPasswordChange, bool isActiveContact, string address1, string address2,string city, string state, string zipcode, string province, string country)
        {
            if (!Utility.ValidateParameter(ref password, true, true, true, 50))
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }
            if (!Utility.ValidateParameter(ref username, true, true, true, 256))
            {
                status = MembershipCreateStatus.InvalidUserName;
                return null;
            }
            if (this.UsernameExists(username))
            {
                status = MembershipCreateStatus.DuplicateUserName;
                return null;
            }
            if (!Utility.ValidateParameter(ref email, this.RequiresUniqueEmail, this.RequiresUniqueEmail, false, 256))
            {
                status = MembershipCreateStatus.InvalidEmail;
                return null;
            }
            if (this.RequiresUniqueEmail && this.EmailExists(email))
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }
            if ((providerUserKey != null) && !(providerUserKey is int))
            {
                status = MembershipCreateStatus.InvalidProviderUserKey;
                return null;
            }
            if (password.Length < this.MinRequiredPasswordLength)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }
            int num = 0;
            for (int i = 0; i < password.Length; i++)
            {
                if (!char.IsLetterOrDigit(password, i))
                {
                    num++;
                }
            }
            if (num < this.MinRequiredNonAlphanumericCharacters)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }
            if ((this.PasswordStrengthRegularExpression.Length > 0) && !Regex.IsMatch(password, this.PasswordStrengthRegularExpression))
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }
            ValidatePasswordEventArgs e = new ValidatePasswordEventArgs(username, password, true);
            this.OnValidatingPassword(e);
            if (e.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            try
            {
                password = this.EncodePassword(password, (int)this.PasswordFormat, this.GenerateSalt());

                SqlParameter[] sqlParams = new SqlParameter[35];
                sqlParams[0] = new SqlParameter("@UserId", SqlDbType.Int);
                sqlParams[0].Direction = ParameterDirection.Output;
                sqlParams[1] = new SqlParameter("@Username", username);
                sqlParams[2] = new SqlParameter("@Password", password);

                if (ReportsToId == DataObjectBase.NullIntRowId)
                    sqlParams[3] = new SqlParameter("@ReportsToId", DBNull.Value);
                else
                    sqlParams[3] = new SqlParameter("@ReportsToId", ReportsToId);

                sqlParams[4] = new SqlParameter("@Prefix", Prefix);
                sqlParams[5] = new SqlParameter("@FirstName", FirstName);
                sqlParams[6] = new SqlParameter("@MiddleName", MiddleName);
                sqlParams[7] = new SqlParameter("@LastName", LastName);
                sqlParams[8] = new SqlParameter("@Suffix", Suffix);
                sqlParams[9] = new SqlParameter("@PreferredName", PreferredName);
                sqlParams[10] = new SqlParameter("@Email", email);
                sqlParams[11] = new SqlParameter("@Email2", Email2);
                sqlParams[12] = new SqlParameter("@HomePhone", HomePhone);
                sqlParams[13] = new SqlParameter("@OfficePhone", OfficePhone);
                sqlParams[14] = new SqlParameter("@MobilePhone", MobilePhone);
                sqlParams[15] = new SqlParameter("@PrimaryFax", PrimaryFax);
                sqlParams[16] = new SqlParameter("@TopicsOfInterest", TopicsOfInterest);

                if (ExpirationDate == DataObjectBase.NullDateValue)
                    sqlParams[17] = new SqlParameter("@ExpirationDate", DBNull.Value);
                else
                    sqlParams[17] = new SqlParameter("@ExpirationDate", ExpirationDate);

                sqlParams[18] = new SqlParameter("@IsActiveMB", IsActiveMB);
                sqlParams[19] = new SqlParameter("@IsActiveFF", IsActiveFF);
                sqlParams[20] = new SqlParameter("@IsATAStaff", IsATAStaff);
                sqlParams[21] = new SqlParameter("@IsAcceptedLicense", IsAcceptedLicense);
                sqlParams[22] = new SqlParameter("@IsPrivate", IsPrivate);
                sqlParams[23] = new SqlParameter("@CreateUserId", CreatingUserId);
                sqlParams[24] = new SqlParameter("@RequiresPasswordChange", RequiresPasswordChange);
                sqlParams[25] = new SqlParameter("@IsActiveContact", isActiveContact);
                sqlParams[26] = new SqlParameter("@IsActiveACH", IsActiveACH);
                sqlParams[27] = new SqlParameter("@Address1", address1);
                sqlParams[28] = new SqlParameter("@Address2", address2);
                sqlParams[29] = new SqlParameter("@City", city);
                sqlParams[30] = new SqlParameter("@State", state);
                sqlParams[31] = new SqlParameter("@Zipcode", zipcode);
                sqlParams[32] = new SqlParameter("@Province", province);
                sqlParams[33] = new SqlParameter("@Country", country);
                sqlParams[34] = new SqlParameter("@FromUI", 1);


                int result = SqlHelper.ExecuteNonQuery(this.connectionString, CommandType.StoredProcedure, "p_Membership_CreateATAUser", sqlParams);

                providerUserKey = int.Parse(sqlParams[0].Value.ToString());
            }
            catch(Exception err)
            {
                throw err;
                status = MembershipCreateStatus.ProviderError;
                return (null);
            }

            DateTime time = DateTime.UtcNow.ToLocalTime();
            ATAMembershipUser user = new ATAMembershipUser(this.Name, username, providerUserKey, email, time, ReportsToId, Prefix, FirstName, MiddleName, LastName, Suffix, HomePhone, OfficePhone, MobilePhone, PrimaryFax, TopicsOfInterest, ExpirationDate, time, IsActiveMB, IsActiveFF, IsActiveACH, IsATAStaff, IsAcceptedLicense, CreatingUserId, CreatingUserId, Email2, PreferredName, IsPrivate, RequiresPasswordChange, time, address1,address2,city,state,zipcode,province,country);

            user.IsActiveContact = isActiveContact;
            status = MembershipCreateStatus.Success;
            return (user);
        }

        #endregion

        #region internal User collection add/delete methods

        #region User Address

        public void AddUserAddress(int UserId, int AddressId, int AddressTypeId)
        {
            SqlParameter[] sqlParams = new SqlParameter[3];
            sqlParams[0] = new SqlParameter("@UserId", UserId);
            sqlParams[1] = new SqlParameter("@AddressId", AddressId);
            sqlParams[2] = new SqlParameter("@AddressTypeId", AddressTypeId); 
            SqlHelper.ExecuteNonQuery(this.connectionString, CommandType.StoredProcedure, "p_User_AddUserAddress", sqlParams);
        }

        public void UpdateUserAddress(int UserId, int AddressId, int AddressTypeId)
        {
            SqlParameter[] sqlParams = new SqlParameter[3];
            sqlParams[0] = new SqlParameter("@UserId", UserId);
            sqlParams[1] = new SqlParameter("@AddressId", AddressId);
            sqlParams[2] = new SqlParameter("@AddressTypeId", AddressTypeId);             
            SqlHelper.ExecuteNonQuery(this.connectionString, CommandType.StoredProcedure, "p_User_UpdateUserAddress", sqlParams);
        }

        public void DeleteUserAddress(int UserId, int AddressId)
        {
            SqlParameter[] sqlParams = new SqlParameter[2];
            sqlParams[0] = new SqlParameter("@UserId", UserId);
            sqlParams[1] = new SqlParameter("@AddressId", AddressId);

            SqlHelper.ExecuteNonQuery(this.connectionString, CommandType.StoredProcedure, "p_User_DeleteUserAddress", sqlParams);
        } 
        #endregion

        #region User Company

        internal void AddUserCompany(int UserId, int CompanyId, string Responsiblities)
        {
            SqlParameter[] sqlParams = new SqlParameter[3];
            sqlParams[0] = new SqlParameter("@UserId", UserId);
            sqlParams[1] = new SqlParameter("@CompanyId", CompanyId); 
            sqlParams[2] = new SqlParameter("@Responsibilities", Responsiblities);

            SqlHelper.ExecuteNonQuery(this.connectionString, CommandType.StoredProcedure, "p_User_AddUserCompany", sqlParams);
        }


        internal void UpdateUserCompany(int UserId, int CompanyId, string Responsiblities)
        {
            SqlParameter[] sqlParams = new SqlParameter[3];
            sqlParams[0] = new SqlParameter("@UserId", UserId);
            sqlParams[1] = new SqlParameter("@CompanyId", CompanyId);  
            sqlParams[2] = new SqlParameter("@Responsibilities", Responsiblities);

            SqlHelper.ExecuteNonQuery(this.connectionString, CommandType.StoredProcedure, "p_User_UpdateUserCompany", sqlParams);
        }

        internal void DeleteUserCompany(int UserId, int CompanyId)
        {
            SqlParameter[] sqlParams = new SqlParameter[2];
            sqlParams[0] = new SqlParameter("@UserId", UserId);
            sqlParams[1] = new SqlParameter("@CompanyId", CompanyId);

            SqlHelper.ExecuteNonQuery(this.connectionString, CommandType.StoredProcedure, "p_User_DeleteUserCompany", sqlParams);
        }

        #endregion

        #region User Group  
        public void AddUserGroup(int UserId, int GroupId,  int CommitteeRoleId)
        {
            SqlParameter[] sqlParams = new SqlParameter[5];
            sqlParams[0] = new SqlParameter("@UserId", UserId);
            sqlParams[1] = new SqlParameter("@GroupId", GroupId);
            if (CommitteeRoleId == SusQTech.Data.DataObjects.DataObjectBase.NullIntRowId)
                sqlParams[2] = new SqlParameter("@CommitteeRoleId", DBNull.Value);
            else
                sqlParams[2] = new SqlParameter("@CommitteeRoleId", CommitteeRoleId);
            sqlParams[3] = new SqlParameter("@PreferredEmailUserId", UserId);//might be deleted
            sqlParams[4] = new SqlParameter("@IsPreferredEmailPrimary", true);//might be deleted

            SqlHelper.ExecuteNonQuery(this.connectionString, CommandType.StoredProcedure, "p_User_AddUserGroup", sqlParams);
        }

        public void DeleteUserGroup(int UserId, int GroupId)
        {
            SqlParameter[] sqlParams = new SqlParameter[2];
            sqlParams[0] = new SqlParameter("@UserId", UserId);
            sqlParams[1] = new SqlParameter("@GroupId", GroupId);

            SqlHelper.ExecuteNonQuery(this.connectionString, CommandType.StoredProcedure, "p_User_DeleteUserGroup", sqlParams);
        }

        #endregion

        #region User Member Type

        internal void AddUserMemberType(int UserId, int MemberTypeId)
        {
            SqlParameter[] sqlParams = new SqlParameter[2];
            sqlParams[0] = new SqlParameter("@UserId", UserId);
            sqlParams[1] = new SqlParameter("@MemberTypeId", MemberTypeId);

            SqlHelper.ExecuteNonQuery(this.connectionString, CommandType.StoredProcedure, "p_User_AddUserMemberType", sqlParams);
        }

        internal void DeleteUserMemberType(int UserId, int MemberTypeId)
        {
            SqlParameter[] sqlParams = new SqlParameter[2];
            sqlParams[0] = new SqlParameter("@UserId", UserId);
            sqlParams[1] = new SqlParameter("@MemberTypeId", MemberTypeId);

            SqlHelper.ExecuteNonQuery(this.connectionString, CommandType.StoredProcedure, "p_User_DeleteUserMemberType", sqlParams);
        }

        #endregion 

        #region User Group Sponsor

        public void AddUserToSponsorGroup(int UserId, int SponsorGroupId)
        {
            SqlParameter[] sqlParams = new SqlParameter[2];
            sqlParams[0] = new SqlParameter("@UserId", UserId);
            sqlParams[1] = new SqlParameter("@SponsorGroupId", SponsorGroupId); 

            SqlHelper.ExecuteNonQuery(this.connectionString, CommandType.StoredProcedure, "p_User_AddUserToSponsorGroup", sqlParams);
        }

        public void ClearUserToSponsorGroup(int UserId)
        {
            SqlParameter[] sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter("@UserId", UserId); 
            SqlHelper.ExecuteNonQuery(this.connectionString, CommandType.StoredProcedure, "p_User_ClearUserToSponsorGroup", sqlParams);
        } 
        #endregion

        #region User Issues

        internal void AddUserIssue(int UserId, int IssueId)
        {
            SqlParameter[] sqlParams = new SqlParameter[2];
            sqlParams[0] = new SqlParameter("@UserId", UserId);
            sqlParams[1] = new SqlParameter("@IssueId", IssueId);

            SqlHelper.ExecuteNonQuery(this.connectionString, CommandType.StoredProcedure, "p_User_AddUserIssue", sqlParams);
        }

        public void DeleteIssue(int IssueId)
        {
            SqlParameter[] sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter("@IssueId", IssueId);

            SqlHelper.ExecuteNonQuery(this.connectionString, CommandType.StoredProcedure, "p_Issue_DeleteIssue", sqlParams);
        }

        internal void DeleteUserIssue(int UserId, int IssueId)
        {
            SqlParameter[] sqlParams = new SqlParameter[2];
            sqlParams[0] = new SqlParameter("@UserId", UserId);
            sqlParams[1] = new SqlParameter("@IssueId", IssueId);

            SqlHelper.ExecuteNonQuery(this.connectionString, CommandType.StoredProcedure, "p_User_DeleteUserIssue", sqlParams);
        }

        internal void ClearUserIssues(int UserId)
        {
            SqlParameter[] sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter("@UserId", UserId);

            SqlHelper.ExecuteNonQuery(this.connectionString, CommandType.StoredProcedure, "p_User_ClearUserIssues", sqlParams);
        }

        #endregion 

        #endregion

        #region generate random password

        public string GenerateRandomPassword(int size)
        {
            return (RandomPasswordGenerator.Generate(size));
        }

        #endregion

        #region update my profile last updated date methods

        internal void SetMyProfileLastViewedDate(int UserId)
        {
            SqlParameter[] sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter("@UserId", UserId);

            SqlHelper.ExecuteNonQuery(this.connectionString, CommandType.StoredProcedure, "p_MyProfile_UpdateLastUpdatedDate", sqlParams);
        }

        internal void SetMyProfileLastViewedDate(int UserId, DateTime newDateTime)
        {
            SqlParameter[] sqlParams = new SqlParameter[2];
            sqlParams[0] = new SqlParameter("@UserId", UserId);
            sqlParams[1] = new SqlParameter("@LastMyProfileUpdate", newDateTime);

            SqlHelper.ExecuteNonQuery(this.connectionString, CommandType.StoredProcedure, "p_MyProfile_UpdateLastUpdatedDate", sqlParams);
        }

        #endregion

        #region GetUserEmail

        public string GetUserEmail(string Username)
        {
            SqlParameter[] sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter("@Username", Username);

            string userEmail = (string)SqlHelper.ExecuteScalar(this.connectionString, CommandType.StoredProcedure, "p_Membership_GetEmailForUsername", sqlParams);

            return (userEmail);
        }

        #endregion

        #endregion
        
        #region standard membership provider methods (using ATAMemberUser)

        #region ChangePassword

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            oldPassword = this.EncodePassword(oldPassword, (int)this.PasswordFormat, this.GenerateSalt());
            newPassword = this.EncodePassword(newPassword, (int)this.PasswordFormat, this.GenerateSalt());

            SqlParameter[] sqlParams = new SqlParameter[3];
            sqlParams[0] = new SqlParameter("@Username", username);
            sqlParams[1] = new SqlParameter("@OldPassword", oldPassword);
            sqlParams[2] = new SqlParameter("@NewPassword", newPassword);

            //int result = Convert.ToInt32(SqlHelper.ExecuteScalar(this.connectionString, CommandType.StoredProcedure, "p_Membership_ChangePassword", sqlParams));

            int result = SqlHelper.ExecuteNonQuery(this.connectionString, CommandType.StoredProcedure, "p_Membership_ChangePassword", sqlParams);

            return (result > 0);
        }

        #endregion

        #region CreateUser

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            if (!Utility.ValidateParameter(ref password, true, true, true, 50))
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }
            if (!Utility.ValidateParameter(ref username, true, true, true, 256))
            {
                status = MembershipCreateStatus.InvalidUserName;
                return null;
            }
            if (this.UsernameExists(username))
            {
                status = MembershipCreateStatus.DuplicateUserName;
                return null;
            }
            if (!Utility.ValidateParameter(ref email, this.RequiresUniqueEmail, this.RequiresUniqueEmail, false, 256))
            {
                status = MembershipCreateStatus.InvalidEmail;
                return null;
            }
            if (this.RequiresUniqueEmail && this.EmailExists(email))
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }
            if ((providerUserKey != null) && !(providerUserKey is int))
            {
                status = MembershipCreateStatus.InvalidProviderUserKey;
                return null;
            }
            if (password.Length < this.MinRequiredPasswordLength)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }
            int num = 0;
            for (int i = 0; i < password.Length; i++)
            {
                if (!char.IsLetterOrDigit(password, i))
                {
                    num++;
                }
            }
            if (num < this.MinRequiredNonAlphanumericCharacters)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }
            if ((this.PasswordStrengthRegularExpression.Length > 0) && !Regex.IsMatch(password, this.PasswordStrengthRegularExpression))
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }
            ValidatePasswordEventArgs e = new ValidatePasswordEventArgs(username, password, true);
            this.OnValidatingPassword(e);
            if (e.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            try
            {
                password = this.EncodePassword(password, (int)this.PasswordFormat, this.GenerateSalt());

                SqlParameter[] sqlParams = new SqlParameter[4];
                sqlParams[0] = new SqlParameter("@Username", username);
                sqlParams[1] = new SqlParameter("@Password", password);
                sqlParams[2] = new SqlParameter("@Email", email);
                sqlParams[3] = new SqlParameter("@UserId", SqlDbType.Int);
                sqlParams[3].Direction = ParameterDirection.Output;

                int result = SqlHelper.ExecuteNonQuery(this.connectionString, CommandType.StoredProcedure, "p_Membership_CreateUser", sqlParams);

                providerUserKey = int.Parse(sqlParams[3].Value.ToString());
            }
            catch
            {
                status = MembershipCreateStatus.ProviderError;
                return (null);
            }

            DateTime time = DateTime.UtcNow.ToLocalTime();
            MembershipUser user = new MembershipUser(this.Name, username, providerUserKey, email, string.Empty, null, true, false, time, time, time, time, new DateTime(0x6da, 1, 1));

            status = MembershipCreateStatus.Success;
            return (user);
        }

        #endregion

        #region DeleteUser

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            SqlParameter[] sqlParams = new SqlParameter[2];
            sqlParams[0] = new SqlParameter("@Username", username);
            sqlParams[1] = new SqlParameter("@DeleteAllRelatedData", deleteAllRelatedData);

            ATAMembershipUser user = this.GetUser(username, false) as ATAMembershipUser;
            int userId = -1;
            if (user != null)
                userId = user.UserId;

            int result = SqlHelper.ExecuteNonQuery(this.connectionString, CommandType.StoredProcedure, "p_Membership_DeleteUser", sqlParams);

            if (result > 0 && userId != -1)
                ClearUserIssues(userId);

            return (result > 0);
        }

        #endregion

        #region FindUsersByEmail  
        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            MembershipUserCollection users = new MembershipUserCollection();
            SqlParameter[] sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter("@Email", emailToMatch);

            DateTime empty = Convert.ToDateTime("1/1/1900");

            SqlDataReader dr = SqlHelper.ExecuteReader(this.connectionString, CommandType.StoredProcedure, "p_Membership_FindUsersByEmail", sqlParams);
            while (dr.Read())
            {
                MembershipUser user = this.NewATAMembershipUserFromReader(dr);
                users.Add(user);
            }

            dr.Close();
            totalRecords = users.Count;

            return users;
        }

        #endregion

        #region FindUsersByName

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            MembershipUserCollection users = new MembershipUserCollection();
            SqlParameter[] sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter("@Username", usernameToMatch);

            DateTime empty = Convert.ToDateTime("1/1/1900");

            SqlDataReader dr = SqlHelper.ExecuteReader(this.connectionString, CommandType.StoredProcedure, "p_Membership_FindUsersByName", sqlParams);
            while (dr.Read())
            {
                MembershipUser user = this.NewATAMembershipUserFromReader(dr);
                users.Add(user);
            }

            dr.Close();
            totalRecords = users.Count;

            return users;
        }

        #endregion

        #region GetAllUsers

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            MembershipUserCollection users = new MembershipUserCollection();

            DateTime empty = Convert.ToDateTime("1/1/1900");

            SqlDataReader dr = SqlHelper.ExecuteReader(this.connectionString, CommandType.StoredProcedure, "p_Membership_GetAllUsers");
            while (dr.Read())
            {
                ATAMembershipUser user = this.NewATAMembershipUserFromReader(dr);
                users.Add(user);
            }

            dr.Close();
            totalRecords = users.Count;

            return users;
        }

        public List<ATAMembershipUser> GetAllATAStaffUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            List<ATAMembershipUser> ret = new List<ATAMembershipUser>();

            DateTime empty = Convert.ToDateTime("1/1/1900");

            SqlDataReader dr = SqlHelper.ExecuteReader(this.connectionString, CommandType.StoredProcedure, "p_Membership_GetAllATAStaffUsers");
            while (dr.Read())
            {
                ATAMembershipUser user = this.NewATAMembershipUserFromReader(dr);
                ret.Add(user);
            }

            dr.Close();
            totalRecords = ret.Count;

            return ret;
        }

        #endregion

        #region GetPassword

        public override string GetPassword(string username, string answer)
        {
            SqlParameter[] sqlParams = new SqlParameter[2];
            sqlParams[0] = new SqlParameter("@Username", username);
            sqlParams[1] = new SqlParameter("@QuestionAnswer", answer);
            string result = (string)SqlHelper.ExecuteScalar(this.connectionString, CommandType.StoredProcedure, "p_Membership_GetPassword", sqlParams);

            result = this.UnEncodePassword(result, (int)this.PasswordFormat);

            return (result);
        }

        #endregion

        #region GetUser

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            ATAMembershipUser user;
            SqlParameter[] sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter("@Username", username);

            DateTime empty = Convert.ToDateTime("1/1/1900");

            SqlDataReader dr = SqlHelper.ExecuteReader(this.connectionString, CommandType.StoredProcedure, "p_Membership_GetUser", sqlParams);
            if (dr.Read())
            {
                user = this.NewATAMembershipUserFromReader(dr);
              //  user.LoadChildCollections(); //Commentfor UM 2016 as we don't need the methods tp 
            }
            else
            {
                user = null;
            }
            dr.Close();

            return (user);
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            ATAMembershipUser user;
            SqlParameter[] sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter("@UserId", providerUserKey);

            DateTime empty = Convert.ToDateTime("1/1/1900");

            SqlDataReader dr = SqlHelper.ExecuteReader(this.connectionString, CommandType.StoredProcedure, "p_Membership_GetUserById", sqlParams);
            if (dr.Read())
            {
                user = this.NewATAMembershipUserFromReader(dr);
                user.LoadChildCollections();
            }
            else
            {
                user = null;
            }
            dr.Close();

            return (user);
        }

        #endregion

        #region GetUserNameByEmail

        public override string GetUserNameByEmail(string email)
        {
            SqlParameter[] sqlParams = new SqlParameter[2];
            sqlParams[0] = new SqlParameter("@Email", email);
            sqlParams[1] = new SqlParameter("@Username", SqlDbType.NVarChar, 30);
            sqlParams[1].Direction = ParameterDirection.Output;

            object result = SqlHelper.ExecuteScalar(this.connectionString, CommandType.StoredProcedure, "p_Membership_GetUserNameByEmail", sqlParams);

            if (result.Equals(DBNull.Value))
                return (string.Empty);

            return (result.ToString());
        }

        public bool EmailExists(string email)
        {
            SqlParameter[] sqlParams = new SqlParameter[2];
            sqlParams[0] = new SqlParameter("@Email", email);
            sqlParams[1] = new SqlParameter("@EmailExists", SqlDbType.Bit);
            sqlParams[1].Direction = ParameterDirection.Output;

            bool result = (bool)SqlHelper.ExecuteScalar(this.connectionString, CommandType.StoredProcedure, "p_Membership_EmailExists", sqlParams);

            return (result);
        }


        #endregion

        #region ResetPassword

        public override string ResetPassword(string username, string answer)
        {
            string newPassword = this.GenerateRandomPassword(this.MinRequiredPasswordLength);
            newPassword = this.EncodePassword(newPassword, (int)this.PasswordFormat, this.GenerateSalt());

            SqlParameter[] sqlParams = new SqlParameter[3];
            sqlParams[0] = new SqlParameter("@Username", username);
            sqlParams[1] = new SqlParameter("@QuestionAnswer", answer);
            sqlParams[2] = new SqlParameter("@NewPassword", newPassword);

            string result = (string)SqlHelper.ExecuteScalar(this.connectionString, CommandType.StoredProcedure, "p_Membership_ResetPassword", sqlParams);

            return (result);
        }

        #endregion

        #region UpdateUser

        public override void UpdateUser(MembershipUser user)
        {
            ATAMembershipUser ataUser = user as ATAMembershipUser;
            if (ataUser != null)
            {
                SqlParameter[] sqlParams = new SqlParameter[43];
                sqlParams[0] = new SqlParameter("@UserId", ataUser.UserId);
                sqlParams[1] = new SqlParameter("@Username", ataUser.UserName);

                if (ataUser.ReportsToId == DataObjectBase.NullIntRowId)
                    sqlParams[2] = new SqlParameter("@ReportsToId", DBNull.Value);
                else
                    sqlParams[2] = new SqlParameter("@ReportsToId", ataUser.ReportsToId);

                sqlParams[3] = new SqlParameter("@Prefix", ataUser.Prefix);
                sqlParams[4] = new SqlParameter("@FirstName", ataUser.FirstName);
                sqlParams[5] = new SqlParameter("@MiddleName", ataUser.MiddleName);
                sqlParams[6] = new SqlParameter("@LastName", ataUser.LastName);
                sqlParams[7] = new SqlParameter("@Suffix", ataUser.Suffix);
                sqlParams[8] = new SqlParameter("@Email", ataUser.Email);
                sqlParams[9] = new SqlParameter("@HomePhone", ataUser.HomePhone);
                sqlParams[10] = new SqlParameter("@OfficePhone", ataUser.OfficePhone);
                sqlParams[11] = new SqlParameter("@MobilePhone", ataUser.MobilePhone);
                sqlParams[12] = new SqlParameter("@PrimaryFax", ataUser.PrimaryFax);
                sqlParams[13] = new SqlParameter("@TopicsOfInterest", ataUser.TopicsOfInterest);

                if (ataUser.ExpirationDate == DataObjectBase.NullDateValue)
                    sqlParams[14] = new SqlParameter("@ExpirationDate", DBNull.Value);
                else
                    sqlParams[14] = new SqlParameter("@ExpirationDate", ataUser.ExpirationDate);

                sqlParams[15] = new SqlParameter("@IsActiveMB", ataUser.IsActiveMB);
                sqlParams[16] = new SqlParameter("@IsActiveFF", ataUser.IsActiveFF);
                sqlParams[17] = new SqlParameter("@IsATAStaff", ataUser.IsATAStaff);
                sqlParams[18] = new SqlParameter("@IsAcceptedLicense", ataUser.IsAcceptedLicense);
                sqlParams[19] = new SqlParameter("@PreferredName", ataUser.PreferredName);
                sqlParams[20] = new SqlParameter("@Email2", ataUser.Email2);
                sqlParams[21] = new SqlParameter("@UpdateUserId", ataUser.LastUpdatingUserId);
                sqlParams[22] = new SqlParameter("@IsPrivate", ataUser.IsPrivate);
                sqlParams[23] = new SqlParameter("@RequiresPasswordChange", ataUser.RequiresPasswordChange);
                //sqlParams[24] = new SqlParameter("@LastLoginDate", ataUser.LastLoginDate);
                if (ataUser.LastLoginDate == DateTime.MinValue)
                    sqlParams[24] = new SqlParameter("@LastLoginDate", DBNull.Value);
                else
                    sqlParams[24] = new SqlParameter("@LastLoginDate", ataUser.LastLoginDate);

                sqlParams[25] = new SqlParameter("@OfficePhoneExtension", ataUser.OfficePhoneExtension);
                sqlParams[26] = new SqlParameter("@JobTitle", ataUser.JobTitle);
                sqlParams[27] = new SqlParameter("@IsActiveContact", ataUser.IsActiveContact);
                sqlParams[28] = new SqlParameter("@WebPage", ataUser.WebPage);
                sqlParams[29] = new SqlParameter("@Facebook", ataUser.Facebook);
                sqlParams[30] = new SqlParameter("@Twitter", ataUser.Twitter);
                sqlParams[31] = new SqlParameter("@LinkedIn", ataUser.LinkedIn);
                sqlParams[32] = new SqlParameter("@GooglePlus", ataUser.GooglePlus);
                sqlParams[33] = new SqlParameter("@Pinterest", ataUser.Pinterest);
                sqlParams[34] = new SqlParameter("@IsActiveACH", ataUser.IsActiveACH);
                sqlParams[35] = new SqlParameter("@Address1", ataUser.Address1);
                sqlParams[36] = new SqlParameter("@Address2", ataUser.Address2);
                sqlParams[37] = new SqlParameter("@City", ataUser.City);
                sqlParams[38] = new SqlParameter("@State", ataUser.State);
                sqlParams[39] = new SqlParameter("@Zipcode", ataUser.Zipcode);
                sqlParams[40] = new SqlParameter("@Province", ataUser.Province);
                sqlParams[41] = new SqlParameter("@Country", ataUser.Country);
                sqlParams[42] = new SqlParameter("@FromUI", 1);



                int result = SqlHelper.ExecuteNonQuery(this.connectionString, CommandType.StoredProcedure, "p_Membership_UpdateATAUser", sqlParams);

                if (result > 0)
                    ataUser.LoadChildCollections();
            }
        }

        //Added newly in 2016 UM - to prevent loadingchildcollections
        public  void UpdateATAUser(MembershipUser user)
        {
            ATAMembershipUser ataUser = user as ATAMembershipUser;
            if (ataUser != null)
            {
                SqlParameter[] sqlParams = new SqlParameter[43];
                sqlParams[0] = new SqlParameter("@UserId", ataUser.UserId);
                sqlParams[1] = new SqlParameter("@Username", ataUser.UserName);

                if (ataUser.ReportsToId == DataObjectBase.NullIntRowId)
                    sqlParams[2] = new SqlParameter("@ReportsToId", DBNull.Value);
                else
                    sqlParams[2] = new SqlParameter("@ReportsToId", ataUser.ReportsToId);

                sqlParams[3] = new SqlParameter("@Prefix", ataUser.Prefix);
                sqlParams[4] = new SqlParameter("@FirstName", ataUser.FirstName);
                sqlParams[5] = new SqlParameter("@MiddleName", ataUser.MiddleName);
                sqlParams[6] = new SqlParameter("@LastName", ataUser.LastName);
                sqlParams[7] = new SqlParameter("@Suffix", ataUser.Suffix);
                sqlParams[8] = new SqlParameter("@Email", ataUser.Email);
                sqlParams[9] = new SqlParameter("@HomePhone", ataUser.HomePhone);
                sqlParams[10] = new SqlParameter("@OfficePhone", ataUser.OfficePhone);
                sqlParams[11] = new SqlParameter("@MobilePhone", ataUser.MobilePhone);
                sqlParams[12] = new SqlParameter("@PrimaryFax", ataUser.PrimaryFax);
                sqlParams[13] = new SqlParameter("@TopicsOfInterest", ataUser.TopicsOfInterest);

                if (ataUser.ExpirationDate == DataObjectBase.NullDateValue)
                    sqlParams[14] = new SqlParameter("@ExpirationDate", DBNull.Value);
                else
                    sqlParams[14] = new SqlParameter("@ExpirationDate", ataUser.ExpirationDate);

                sqlParams[15] = new SqlParameter("@IsActiveMB", ataUser.IsActiveMB);
                sqlParams[16] = new SqlParameter("@IsActiveFF", ataUser.IsActiveFF);
                sqlParams[17] = new SqlParameter("@IsATAStaff", ataUser.IsATAStaff);
                sqlParams[18] = new SqlParameter("@IsAcceptedLicense", ataUser.IsAcceptedLicense);
                sqlParams[19] = new SqlParameter("@PreferredName", ataUser.PreferredName);
                sqlParams[20] = new SqlParameter("@Email2", ataUser.Email2);
                sqlParams[21] = new SqlParameter("@UpdateUserId", ataUser.LastUpdatingUserId);
                sqlParams[22] = new SqlParameter("@IsPrivate", ataUser.IsPrivate);
                sqlParams[23] = new SqlParameter("@RequiresPasswordChange", ataUser.RequiresPasswordChange);
                sqlParams[24] = new SqlParameter("@LastLoginDate", ataUser.LastLoginDate);

                sqlParams[25] = new SqlParameter("@OfficePhoneExtension", ataUser.OfficePhoneExtension);
                sqlParams[26] = new SqlParameter("@JobTitle", ataUser.JobTitle);
                sqlParams[27] = new SqlParameter("@IsActiveContact", ataUser.IsActiveContact);
                sqlParams[28] = new SqlParameter("@WebPage", ataUser.WebPage);
                sqlParams[29] = new SqlParameter("@Facebook", ataUser.Facebook);
                sqlParams[30] = new SqlParameter("@Twitter", ataUser.Twitter);
                sqlParams[31] = new SqlParameter("@LinkedIn", ataUser.LinkedIn);
                sqlParams[32] = new SqlParameter("@GooglePlus", ataUser.GooglePlus);
                sqlParams[33] = new SqlParameter("@Pinterest", ataUser.Pinterest);
                sqlParams[34] = new SqlParameter("@IsActiveACH", ataUser.IsActiveACH);
                sqlParams[35] = new SqlParameter("@Address1", ataUser.Address1);
                sqlParams[36] = new SqlParameter("@Address2", ataUser.Address2);
                sqlParams[37] = new SqlParameter("@City", ataUser.City);
                sqlParams[38] = new SqlParameter("@State", ataUser.State);
                sqlParams[39] = new SqlParameter("@Zipcode", ataUser.Zipcode);
                sqlParams[40] = new SqlParameter("@Province", ataUser.Province);
                sqlParams[41] = new SqlParameter("@Country", ataUser.Country);
                sqlParams[42] = new SqlParameter("@FromUI", 1);

                int result = SqlHelper.ExecuteNonQuery(this.connectionString, CommandType.StoredProcedure, "p_Membership_UpdateATAUser", sqlParams);
                 
            }
        }

        public void UpdateUserLastLoginDate(MembershipUser user)
        {
            ATAMembershipUser ataUser = user as ATAMembershipUser;
            if (ataUser != null)
            {
                SqlParameter[] sqlParams = new SqlParameter[1];
                sqlParams[0] = new SqlParameter("@UserId", ataUser.UserId);  
                SqlHelper.ExecuteNonQuery(this.connectionString, CommandType.StoredProcedure, "p_Membership_UpdateATAUserLastLoginDate", sqlParams); 
            }
        }

        #endregion

        #region ValidateUser

        public override bool ValidateUser(string username, string password)
        {
            password = this.EncodePassword(password, (int)this.PasswordFormat, this.GenerateSalt());

            SqlParameter[] sqlParams = new SqlParameter[3];
            sqlParams[0] = new SqlParameter("@Username", username);
            sqlParams[1] = new SqlParameter("@Password", password);
            sqlParams[2] = new SqlParameter("@UserIsValid", SqlDbType.Bit);
            sqlParams[2].Direction = ParameterDirection.Output;

            bool result = (bool)SqlHelper.ExecuteScalar(this.connectionString, CommandType.StoredProcedure, "p_Membership_ValidateUser", sqlParams);

            if (result)
            {
                //Commented by NA 5/23/2018 - Because ACH authentication was throwing a NULL for user.HasMemberType for nischalaaamidala@yahoo.com, which didnot have the membertype.

                ////do not validate 'Contacts' member types
                ATAMembershipUser user = this.GetUser(username, false) as ATAMembershipUser;
                if (user.HasMemberType(38))
                    return false;
            }

            return (result);
        }

        #endregion

        #region methods not implemented

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool UnlockUser(string userName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #endregion

        #region Properties

        /// <summary></summary>
        public override string ApplicationName
        {
            get
            {
                if (HttpContext.Current.Items["ApplicationName"] == null || HttpContext.Current.Items["ApplicationName"].ToString() == "")
                    return "/";
                else
                    return HttpContext.Current.Items["ApplicationName"].ToString();
            }
            set { HttpContext.Current.Items["ApplicationName"] = value; }
        }

        /// <summary></summary>
        public string ConnectionStringName
        {
            get
            {
                if (_config["connectionStringName"] == null)
                    throw new Exception("Connectiong String Name Not Provided");
                else
                    return _config["connectionStringName"];
            }
        }

        /// <summary></summary>
        public override bool EnablePasswordReset
        {
            get { return false; }
        }

        /// <summary></summary>
        public override bool EnablePasswordRetrieval
        {
            get { return false; }
        }

        /// <summary></summary>
        public override int MaxInvalidPasswordAttempts
        {
            get { return _config["passwordAttemptThreshold"] != null ? Convert.ToInt32(_config["passwordAttemptThreshold"]) : 3; }
        }

        /// <summary></summary>
        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return _config["minRequiredNonalphanumericCharacters"] != null ? Convert.ToInt32(_config["minRequiredNonalphanumericCharacters"]) : 0; }
        }

        /// <summary></summary>
        public override int MinRequiredPasswordLength
        {
            get { return _config["minRequiredPasswordLength"] != null ? Convert.ToInt32(_config["minRequiredPasswordLength"]) : 8; }
        }

        /// <summary></summary>
        public override int PasswordAttemptWindow
        {
            get { return 1; }
        }

        /// <summary></summary>
        public override MembershipPasswordFormat PasswordFormat
        {
            get { return MembershipPasswordFormat.Encrypted; }
        }

        /// <summary></summary>
        public override string PasswordStrengthRegularExpression
        {
            //get { return @"^.*(?=.{8,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%^&+=]).*$"; }
            get { return this._config["passwordStrengthRegularExpression"]; }
        }

        /// <summary></summary>
        public override bool RequiresQuestionAndAnswer
        {
            get { return false; }
        }

        /// <summary></summary>
        public override bool RequiresUniqueEmail
        {
            get
            {
                return _config["requiresUniqueEmail"] != null ?
              Convert.ToBoolean(_config["requiresUniqueEmail"]) : true;
            }
        }

        #endregion

        #region private properties

        private string _connectionString = string.Empty;

        /// <summary></summary>
        private string connectionString
        {
            get
            {
                if (!string.IsNullOrEmpty(this._connectionString))
                    return (this._connectionString);
                else if (System.Configuration.ConfigurationManager.ConnectionStrings[this.ConnectionStringName] != null)
                    return System.Configuration.ConfigurationManager.ConnectionStrings[this.ConnectionStringName].ConnectionString;
                else
                    throw new Exception("Cannot find connection string for " + this.ConnectionStringName);
            }
        }

        public void SetConnectionString(string connectionString)
        {
            this._connectionString = connectionString;
        }

        #endregion

        #region private methods

        public ATAMembershipUser NewATAMembershipUserFromReader(SqlDataReader reader)
        {
            int reportsToId = ((int.TryParse(reader["ReportsToId"].ToString(), out reportsToId)) ? reportsToId : DataObjectBase.NullIntRowId);
            int creatingUserId = ((int.TryParse(reader["CreateUserId"].ToString(), out creatingUserId)) ? creatingUserId : DataObjectBase.NullIntRowId);
            int lastModifiedUserId = ((int.TryParse(reader["LastUpdatedUserId"].ToString(), out lastModifiedUserId)) ? lastModifiedUserId : DataObjectBase.NullIntRowId);
            DateTime expirationDate = ((DateTime.TryParse(reader["ExpirationDate"].ToString(), out expirationDate)) ? expirationDate : DataObjectBase.NullDateValue);
            DateTime lastLoginDate = ((DateTime.TryParse(reader["LastLoginDate"].ToString(), out lastLoginDate)) ? lastLoginDate : DataObjectBase.NullDateValue);

            ATAMembershipUser user = new ATAMembershipUser(this.Name, reader["Username"].ToString(), reader["UserId"], reader["Email"].ToString(),
                DateTime.Parse(reader["CreateDate"].ToString()), reportsToId, reader["Prefix"].ToString(), reader["FirstName"].ToString(),
                reader["MiddleName"].ToString(), reader["LastName"].ToString(), reader["Suffix"].ToString(), reader["HomePhone"].ToString(),
                reader["OfficePhone"].ToString(), reader["MobilePhone"].ToString(), reader["PrimaryFax"].ToString(),
                reader["TopicsOfInterest"].ToString(), expirationDate, DateTime.Parse(reader["LastUpdatedDate"].ToString()),
                (bool)reader["IsActiveMB"], (bool)reader["IsActiveFF"], (bool)reader["IsActiveACH"], (bool)reader["IsATAStaff"], (bool)reader["IsAcceptedLicense"],
                creatingUserId, lastModifiedUserId, reader["Email2"].ToString(), reader["PreferredName"].ToString(), (bool)reader["IsPrivate"],
                (bool)reader["RequiresPasswordChange"], lastLoginDate, reader["Address1"].ToString(), reader["Address2"].ToString(), reader["City"].ToString(), reader["State"].ToString(),
                reader["Zipcode"].ToString(), reader["Province"].ToString(), reader["Country"].ToString());

            // set phone extension and job title that are not in the base membership user
            user.OfficePhoneExtension = reader["OfficePhoneExtension"].ToString();
            user.JobTitle = reader["jobTitle"].ToString();
            user.IsActiveContact = (bool)reader["IsActiveContact"];
            user.WebPage = reader["WebPage"].ToString();
            user.Facebook = reader["Facebook"].ToString();
            user.Twitter = reader["Twitter"].ToString();
            user.LinkedIn = reader["LinkedIn"].ToString();
            user.GooglePlus = reader["GooglePlus"].ToString();
            user.Pinterest = reader["Pinterest"].ToString();  
            return (user); 
        }

        private bool UsernameExists(string username)
        {
            SqlParameter[] sqlParams = new SqlParameter[2];
            sqlParams[0] = new SqlParameter("@Username", username);
            sqlParams[1] = new SqlParameter("@UsernameExists", SqlDbType.Bit);
            sqlParams[1].Direction = ParameterDirection.Output;

            bool result = (bool)SqlHelper.ExecuteScalar(this.connectionString, CommandType.StoredProcedure, "p_Membership_UsernameExists", sqlParams);

            return (result);
        } 
       
        #endregion

        #region Password Encryption methods

        public string GenerateSalt()
        {
            //we aren't hasing passwords for ATA, so this method is just here incase we need it later
            //the salt should be stored with each user.  we return a constant

            byte[] buf = new byte[16] { 1, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144, 233, 1, 1, 2 };
            //byte[] buf = new byte[16];
            //(new RNGCryptoServiceProvider()).GetBytes(buf);
            return Convert.ToBase64String(buf);
        }

        public string EncodePassword(string pass, int passwordFormat, string salt)
        {
            if (passwordFormat == 0) // MembershipPasswordFormat.Clear
                return pass;

            byte[] bIn = Encoding.Unicode.GetBytes(pass);
            byte[] bSalt = Convert.FromBase64String(salt);
            byte[] bAll = new byte[bSalt.Length + bIn.Length];
            byte[] bRet = null;

            Buffer.BlockCopy(bSalt, 0, bAll, 0, bSalt.Length);
            Buffer.BlockCopy(bIn, 0, bAll, bSalt.Length, bIn.Length);
            if (passwordFormat == 1)
            { // MembershipPasswordFormat.Hashed
                HashAlgorithm s = HashAlgorithm.Create(Membership.HashAlgorithmType);
                bRet = s.ComputeHash(bAll);
            }
            else
            {
                bRet = EncryptPassword(bAll);
            }

            return Convert.ToBase64String(bRet);
        }

        public string UnEncodePassword(string pass, int passwordFormat)
        {
            switch (passwordFormat)
            {
                case 0: // MembershipPasswordFormat.Clear:
                    return pass;
                case 1: // MembershipPasswordFormat.Hashed:
                    throw new ProviderException(Resources.Messages.ProviderCannotDecodeHashedPasswords);
                default:
                    byte[] bIn = Convert.FromBase64String(pass);
                    byte[] bRet = DecryptPassword(bIn);
                    if (bRet == null)
                        return null;
                    return Encoding.Unicode.GetString(bRet, 16, bRet.Length - 16);
            }
        }

        #endregion
    }
}