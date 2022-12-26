#region namespaces

using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.Profile;
using ATA.Authentication.Providers;
using Microsoft.ApplicationBlocks.Data;
using SusQTech.Data.DataObjects;
using ATA.ObjectModel; 
using ATA.LyrisProxy;

#endregion

namespace ATA.Authentication
{
    [Serializable]
    public class ATAMembershipUser : MembershipUser
    {
        #region private members

        private int _reportsToId;
        private string _prefix;
        private string _firstName;
        private string _middleName;
        private string _lastName;
        private string _suffix;
        private string _homePhone;
        private string _officePhone;
        // office phone extension added 2008-12-12
        private string _officePhoneExtension;
        private string _mobilePhone;
        private string _primaryFax;
        private string _webPage;
        private string _twitter;
        private string _facebook;
        private string _linkedIn;
        private string _googlePlus;
        private string _pinterest;
        private string _topicsOfInterest;
        private DateTime _expirationDate;
        private DateTime _lastUpdatedDate;
        private bool _isActiveMB;
        private bool _isActiveFF;
        private bool _isATAStaff;
        private bool _isAcceptedLicense;
        private int _creatingUserId;
        private int _lastUpdatingUserId;
        private string _email2;
        private string _preferredName;
        private bool _isPrivate;
        private bool _requiresPasswordChange;
        private string _jobTitle;
        private bool _isActiveContact;

        //child object collections, always loaded.
        private DataObjectList<UserAddress> _userAddresses = null; 
        private DataObjectList<UserIssue> _userIssues = null;
        private DataObjectList<UserCompany> _userCompanies = null;
        private DataObjectList<UserGroup> _userGroups = null;
        private DataObjectList<UserMemberType> _userMemberTypes = null; 
        //helper collections for editor pages, only loaded if needed
        private DataObjectList<PossiblePreferredEmail> _possiblePreferredEmails = null; 
        private DataObjectList<SponsoredUser> _mySponsoredUsers = null;
        private DataObjectList<OpenEnrollmentGroup> _openEnrollmentGroups = null;
        private DataObjectList<SponsoredUser> _myInactiveSponsoredUsers = null;
        private DataObjectList<EditIssue> _editIssues = null;

        private const string UserAddressCollectionLoad = "p_User_GetUserAddresses"; 
        private const string UserIssueCollectionLoad = "p_User_GetUserIssues";
        private const string UserCompanyCollectionLoad = "p_User_GetUserCompanies";
        private const string UserGroupCollectionLoad = "p_User_GetUserGroups";
        private const string UserMemberTypeCollectionLoad = "p_User_GetUserMemberTypes";
         
        private const string GetAllPossiblePreferredEmails = "p_User_GetAllPossiblePreferredEmails";
        //private const string GetGroupsForEditPage = "p_User_GetAllEditGroups"; 
        private const string ATAGetMySponsoredUsers = "p_User_GetSponsoredUsers";
        private const string ATAGetAvailableOpenEnrollmentGroups = "p_User_GetAllOpenEnrollmentGroups";
        private const string ATAGetMyInactiveSponsoredUsers = "p_User_GetInactiveSponsoredUsers";
        private const string GetIssuesForEditPage = "p_User_GetAllEditIssues";

        private Dictionary<int, string> _possibleReportsToUsers = null;

        #endregion

        #region contructors

        public ATAMembershipUser() : base() { }

        public ATAMembershipUser(string providerName, string name, object providerUserKey, string email, DateTime creationDate,
            int ReportsToId, string Prefix, string FirstName, string MiddleName, string LastName, string Suffix,
            string HomePhone, string OfficePhone, string MobilePhone, string PrimaryFax, string TopicsOfInterest,
            DateTime ExpirationDate, DateTime LastUpdatedDate, bool IsActiveMB, bool IsActiveFF, bool IsATAStaff,
            bool IsAcceptedLicense, int CreatingUserId, int LastUpdatingUserId, string Email2, string PreferredName,
            bool IsPrivate, bool RequiresPasswordChange, DateTime LastLoginDate)
            : base(providerName, name, providerUserKey, email, string.Empty, string.Empty, true, false, creationDate, LastLoginDate, DateTime.Now, Convert.ToDateTime("1/1/1900"), Convert.ToDateTime("1/1/1900"))
        {
            this._reportsToId = ReportsToId;
            this._prefix = Prefix;
            this._firstName = FirstName;
            this._middleName = MiddleName;
            this._lastName = LastName;
            this._suffix = Suffix;
            this._homePhone = HomePhone;
            this._officePhone = OfficePhone;
            // phone extension is not in the base MembershipUser contructor signature so it must be set separately
            this._officePhoneExtension = string.Empty;
            this._mobilePhone = MobilePhone;
            this._primaryFax = PrimaryFax;
            this._topicsOfInterest = TopicsOfInterest;
            this._expirationDate = ExpirationDate;
            this._lastUpdatedDate = LastUpdatedDate;
            this._isActiveMB = IsActiveMB;
            this._isActiveFF = IsActiveFF;
            this._isATAStaff = IsATAStaff;
            this._isAcceptedLicense = IsAcceptedLicense;
            this._creatingUserId = CreatingUserId;
            this._lastUpdatingUserId = LastUpdatingUserId;
            this._email2 = Email2;
            this._preferredName = PreferredName;
            this._isPrivate = IsPrivate;
            this._requiresPasswordChange = RequiresPasswordChange;
            // job title is not in the base MembershipUser contructor signature so it must be set separately
            this._jobTitle = string.Empty; 
        }

        #endregion



        #region Child Collection Accessors

        public UserAddress[] UserAddresses
        {
            get { return (this._userAddresses.ToArray()); }
        }  

        public UserIssue[] UserIssues
        {
            get { return (this._userIssues.ToArray()); }
        }

        public UserCompany[] UserCompanies
        {
            get { return (this._userCompanies.ToArray()); }
        }

        public UserGroup[] UserGroups
        {
            get { return (this._userGroups.ToArray()); }
        }

        public UserMemberType[] UserMemberTypes
        {
            get { return (this._userMemberTypes.ToArray()); }
        }

        public string GetPassword()
        {
            string strPassword = string.Empty;

            ATAMembershipProvider provider = Membership.Providers[this.ProviderName] as ATAMembershipProvider;
            if (provider != null)
            {
                strPassword = provider.GetPassword(this.UserName, string.Empty);
            }

            return strPassword;
        }

        public PossiblePreferredEmail[] GetPossiblePreferredEmails(bool forceRefresh)
        {
            if (this._possiblePreferredEmails == null || forceRefresh)
            {
                this._possiblePreferredEmails = new DataObjectList<PossiblePreferredEmail>(new SqlParameter[] { new SqlParameter("@UserId", this.UserId) }, ATAMembershipUser.GetAllPossiblePreferredEmails);
            }
            return (this._possiblePreferredEmails.ToArray());
        }
         

        public EditIssue[] GetEditIssues(bool forceRefresh)
        {
            if (this._editIssues == null || forceRefresh)
            {
                SqlParameter[] sqlparams = new SqlParameter[2];
                sqlparams[0] = new SqlParameter("@UserId", this.UserId);
                sqlparams[1] = new SqlParameter("@AppliesToSiteId", (int)ATAMembershipUtility.Instance.CurrentSite);
                this._editIssues = new DataObjectList<EditIssue>(sqlparams, ATAMembershipUser.GetIssuesForEditPage);
            }
            return (this._editIssues.ToArray());
        }

        public OpenEnrollmentGroup[] GetAvailableOpenEnrollmentGroups(bool forceRefresh)
        {
            if (this._openEnrollmentGroups == null || forceRefresh)
            {
                SqlParameter[] sqlparams = new SqlParameter[2];
                sqlparams[0] = new SqlParameter("@UserId", this.UserId);
                sqlparams[1] = new SqlParameter("@AppliesToSiteId", (int)ATAMembershipUtility.Instance.CurrentSite);
                this._openEnrollmentGroups = new DataObjectList<OpenEnrollmentGroup>(sqlparams, ATAMembershipUser.ATAGetAvailableOpenEnrollmentGroups);
            }
            return (this._openEnrollmentGroups.ToArray());
        } 

        public SponsoredUser[] GetMySponsoredUsers(bool forceRefresh, bool filterBySite)
        {
            if (this._mySponsoredUsers == null || forceRefresh)
            {
                List<SqlParameter> sqlParams = new List<SqlParameter>();
                sqlParams.Add(new SqlParameter("@UserId", this.UserId));
                if (filterBySite)
                {
                    sqlParams.Add(new SqlParameter("@FilterBySite", true));
                    sqlParams.Add(new SqlParameter("@AppliesToSiteId", (int)ATAMembershipUtility.Instance.CurrentSite));
                    this._mySponsoredUsers = new DataObjectList<SponsoredUser>(sqlParams.ToArray(), ATAMembershipUser.ATAGetMySponsoredUsers);
                }
                else
                {
                    this._mySponsoredUsers = new DataObjectList<SponsoredUser>(sqlParams.ToArray(), ATAMembershipUser.ATAGetMySponsoredUsers);
                }
            }
            return (this._mySponsoredUsers.ToArray());
        }

        public SponsoredUser[] GetMyInactiveSponsoredUsers(bool forceRefresh, bool filterBySite)
        {
            if (this._myInactiveSponsoredUsers == null || forceRefresh)
            {
                List<SqlParameter> sqlParams = new List<SqlParameter>();
                sqlParams.Add(new SqlParameter("@UserId", this.UserId));
                if (filterBySite)
                {
                    sqlParams.Add(new SqlParameter("@FilterBySite", true));
                    sqlParams.Add(new SqlParameter("@AppliesToSiteId", (int)ATAMembershipUtility.Instance.CurrentSite));
                    this._myInactiveSponsoredUsers = new DataObjectList<SponsoredUser>(sqlParams.ToArray(), ATAMembershipUser.ATAGetMyInactiveSponsoredUsers);
                }
                else
                {
                    this._myInactiveSponsoredUsers = new DataObjectList<SponsoredUser>(sqlParams.ToArray(), ATAMembershipUser.ATAGetMyInactiveSponsoredUsers);
                }
            }
            return (this._myInactiveSponsoredUsers.ToArray());
        }

        public Dictionary<int, string> GetAllPossibleReportsToUsers(bool forceRefresh)
        {
            if (this._possibleReportsToUsers == null || forceRefresh)
            {
                this._possibleReportsToUsers = new Dictionary<int, string>();

                SqlParameter[] sqlParams = new SqlParameter[1];
                sqlParams[0] = new SqlParameter("@UserId", this.UserId);
                using (SqlDataReader dr = SqlHelper.ExecuteReader(ConnectionFactory.DataObjectConnectionString, CommandType.StoredProcedure, "p_User_GetAllPossibleReportsTo", sqlParams))
                {
                    while (dr.Read())
                    {
                        this._possibleReportsToUsers.Add(int.Parse(dr["UserId"].ToString()), dr["DisplayName"].ToString());
                    }
                    dr.Close();
                }
            }
            return (this._possibleReportsToUsers);
        }

        #endregion

        #region Get groups administrated by this user

        public ATAGroup[] GetMyAdminGroups()
        {
            ManyToManyRelationship<LookupUser, ATAGroup> adminGroups = new ManyToManyRelationship<LookupUser, ATAGroup>("GroupAdmin");
            ATAGroup[] groups = adminGroups.GetSecondTypeByFirstId(this.UserId, false);

            List<ATAGroup> returnGroups = new List<ATAGroup>();

            foreach (ATAGroup group in groups)
            {
                if (group.AppliesToSite == ATAMembershipUtility.Instance.CurrentSite)
                    returnGroups.Add(group);
            }

            return (returnGroups.ToArray());
        }

        #endregion

        #region public properties

        public int UserId
        {
            get { return ((int)this.ProviderUserKey); }
        }
        public int ReportsToId
        {
            get { return (this._reportsToId); }
            set { this._reportsToId = value; }
        }
        public string Prefix
        {
            get { return (this._prefix); }
            set { this._prefix = value; }
        }
        public string FirstName
        {
            get { return (this._firstName); }
            set { this._firstName = value; }
        }
        public string MiddleName
        {
            get { return (this._middleName); }
            set { this._middleName = value; }
        }
        public string LastName
        {
            get { return (this._lastName); }
            set { this._lastName = value; }
        }
        public string Suffix
        {
            get { return (this._suffix); }
            set { this._suffix = value; }
        }
        public string HomePhone
        {
            get { return (this._homePhone); }
            set { this._homePhone = value; }
        }
        public string OfficePhone
        {
            get { return (this._officePhone); }
            set { this._officePhone = value; }
        }
        public string OfficePhoneExtension
        {
            get { return (this._officePhoneExtension); }
            set { this._officePhoneExtension = value; }
        }
        public string MobilePhone
        {
            get { return (this._mobilePhone); }
            set { this._mobilePhone = value; }
        }
        public string PrimaryFax
        {
            get { return (this._primaryFax); }
            set { this._primaryFax = value; }
        }

        public string WebPage
        {
            get { return (this._webPage); }
            set { this._webPage = value; }
        }

        public string Twitter
        {
            get { return (this._twitter); }
            set { this._twitter = value; }
        }

        public string Facebook
        {
            get { return (this._facebook); }
            set { this._facebook = value; }
        }

        public string LinkedIn
        {
            get { return (this._linkedIn); }
            set { this._linkedIn = value; }
        }

        public string GooglePlus
        {
            get { return (this._googlePlus); }
            set { this._googlePlus = value; }
        }

        public string Pinterest
        {
            get { return (this._pinterest); }
            set { this._pinterest = value; }
        } 
        
        public string TopicsOfInterest
        {
            get { return (this._topicsOfInterest); }
            set { this._topicsOfInterest = value; }
        }
        public DateTime ExpirationDate
        {
            get { return (this._expirationDate); }
            set { this._expirationDate = value; }
        }
        public DateTime LastUpdatedDate
        {
            get { return (this._lastUpdatedDate); }
            set { this._lastUpdatedDate = value; }
        }
        public bool IsActiveMB
        {
            get { return (this._isActiveMB); }
            set { this._isActiveMB = value; }
        }
        public bool IsActiveFF
        {
            get { return (this._isActiveFF); }
            set { this._isActiveFF = value; }
        }
        public bool IsATAStaff
        {
            get { return (this._isATAStaff); }
            set { this._isATAStaff = value; }
        }
        public bool IsAcceptedLicense
        {
            get { return (this._isAcceptedLicense); }
            set { this._isAcceptedLicense = value; }
        }
        public int CreatingUserId
        {
            get { return (this._creatingUserId); }
            set { this._creatingUserId = value; }
        }
        public int LastUpdatingUserId
        {
            get { return (this._lastUpdatingUserId); }
            set { this._lastUpdatingUserId = value; }
        }
        public string Email2
        {
            get { return (this._email2); }
            set { this._email2 = value; }
        }
        public string PreferredName
        {
            get { return (this._preferredName); }
            set { this._preferredName = value; }
        }
        public bool IsPrivate
        {
            get { return (this._isPrivate); }
            set { this._isPrivate = value; }
        }
        public bool RequiresPasswordChange
        {
            get { return (this._requiresPasswordChange); }
            set { this._requiresPasswordChange = value; }
        }
        public string JobTitle
        {
            get { return (this._jobTitle); }
            set { this._jobTitle = value; }
        }

        public bool IsActiveContact
        {
            get { return (this._isActiveContact); }
            set { this._isActiveContact = value; }
        }

        
        #endregion

        #region UserAddress methods

        /// <summary>
        /// Adds a new UserAddress for this user.
        /// </summary> 
        /// <param name="AddressTypeId">The id of the associated address type from the [AddressType] table.</param>
        /// <param name="AddressId">The id of an existing address in the [Address] table.</param>
        public void AddAddress(int AddressTypeId, int AddressId)
        {
            ATAMembershipProvider provider = Membership.Providers[this.ProviderName] as ATAMembershipProvider;
            if (provider != null)
            {
                provider.AddUserAddress(this.UserId, AddressId, AddressTypeId);
                this.LoadChildAddresses();
            }
        }

        /// <summary>
        /// Adds a new UserAddress for this user, creating a new address record
        /// </summary>     
        /// <param name="AddressTypeId">The id of the associated address type from the [AddressType] table.</param>
        /// <param name="Address1">The first line of the address</param>
        /// <param name="Address2">The second line of the address</param>
        /// <param name="City">Address city</param>
        /// <param name="State">Address state</param>
        /// <param name="ZipCode">Address Zip/Postal Code</param>
        public void AddAddress(int AddressTypeId, string Address1, string Address2, string City, string State, string ZipCode, string Province, string Country)
        {
            Address address = new Address();
            address.Address1 = Address1;
            address.Address2 = Address2;
            address.City = City;
            address.State = State;
            address.ZipCode = ZipCode;
            address.Province = Province;
            address.Country = Country;
            address.Save();
            this.AddAddress(AddressTypeId, address.AddressId);
        }

        /// <summary>
        /// Adds a new UserAddress for this user, creating a new address record and a new 
        /// address type record
        /// </summary>        
        /// <param name="AddressTypeDescription">The new address type description to create</param>
        /// <param name="Address1">The first line of the address</param>
        /// <param name="Address2">The second line of the address</param>
        /// <param name="City">Address city</param>
        /// <param name="State">Address state</param>
        /// <param name="ZipCode">Address Zip/Postal Code</param>
        public void AddAddress(string AddressTypeDescription, string Address1, string Address2, string City, string State, string ZipCode, string Province, string Country)
        {
            AddressType addressType = new AddressType();
            addressType.Description = AddressTypeDescription;
            addressType.Save();
            this.AddAddress(addressType.AddressTypeId, Address1, Address2, City, State, ZipCode, Province, Country);
        }

        /// <summary>
        /// Deletes a [User] [Address] association, the Address and User record remain unchanged.
        /// </summary>
        /// <param name="AddressId"></param>
        public void DeleteAddress(int AddressId)
        {
            ATAMembershipProvider provider = Membership.Providers[this.ProviderName] as ATAMembershipProvider;
            if (provider != null)
            {
                provider.DeleteUserAddress(this.UserId, AddressId);
                this.LoadChildAddresses();
            }
        }
 
        public void SetPassword(string strPassword)
        {
            ATAMembershipProvider provider = Membership.Providers[this.ProviderName] as ATAMembershipProvider;
            if (provider != null)
            {
                provider.ChangePassword(this.UserName, GetPassword(), strPassword);
            }
        }

        public UserAddress GetUserAddressByAddressId(int AddressId)
        {
            UserAddress rAddress = null;
            foreach (UserAddress address in this._userAddresses)
            {
                if (address.AddressId == AddressId)
                {
                    rAddress = address;
                    break;
                }
            }
            return (rAddress);
        }

        public void UpdateAddress(int AddressTypeId, int AddressId)
        {
            ATAMembershipProvider provider = Membership.Providers[this.ProviderName] as ATAMembershipProvider;
            if (provider != null)
            {
                provider.UpdateUserAddress(this.UserId, AddressId, AddressTypeId);
                this.LoadChildAddresses();
            }
        }

        #endregion

        #region UserCompany methods

        /// <summary>
        /// Adds a new User company for this user
        /// </summary>
        /// <param name="Responsibilities">A description of this users responsiblities at this company</param> 
        /// <param name="CompanyId">The id of the existing [Company] record</param>
        public void AddCompany(string Responsibilities, int CompanyId)
        {
            ATAMembershipProvider provider = Membership.Providers[this.ProviderName] as ATAMembershipProvider;
            if (provider != null)
            {
                provider.AddUserCompany(this.UserId, CompanyId, Responsibilities);
                this.LoadChildCompanies();
            }
        }  

        /// <summary>
        /// Deletes a [User] [Company] association, the Company and User record remain unchanged.
        /// </summary>
        /// <param name="CompanyId"></param>
        public void DeleteCompany(int CompanyId)
        {
            ATAMembershipProvider provider = Membership.Providers[this.ProviderName] as ATAMembershipProvider;
            if (provider != null)
            {
                provider.DeleteUserCompany(this.UserId, CompanyId);
                this.LoadChildCompanies();
            }
        }


        public void UpdateCompany(string Responsibilities, int CompanyId)
        {
            ATAMembershipProvider provider = Membership.Providers[this.ProviderName] as ATAMembershipProvider;
            if (provider != null)
            {
                provider.UpdateUserCompany(this.UserId, CompanyId, Responsibilities);
                this.LoadChildCompanies();
            }
        }

        #endregion 

        #region UserGroup methods  
        public void AddGroup(int GroupId)
        {
            AddGroup(GroupId, DataObjectBase.NullIntRowId, true);
        }

        public void AddGroup(int GroupId, int CommitteeRoleId)
        {
            AddGroup(GroupId, CommitteeRoleId, true);
        }

        public void AddGroup(int GroupId, int CommitteeRoleId, bool reloadGroups)
        {
            ATAMembershipProvider provider = Membership.Providers[this.ProviderName] as ATAMembershipProvider;
            if (provider != null)
            {
                AddGroup(GroupId, CommitteeRoleId, reloadGroups, provider);
            }
        }

        public void AddGroup(int GroupId, int CommitteeRoleId, bool reloadGroups, ATAMembershipProvider provider)
        {
            ATAGroup group = new ATAGroup(GroupId);
            AddGroup(GroupId, CommitteeRoleId, reloadGroups, provider, group.LyrisListName);
        }

        public void AddGroup(int GroupId, int CommitteeRoleId, bool reloadGroups, ATAMembershipProvider provider, string LyrisListName)
        {
            provider.AddUserGroup(this.UserId, GroupId, CommitteeRoleId);
        } 

        /// <summary>
        /// Deletes a [User] [Group] association, the Group and User record remain unchanged.
        /// </summary>
        /// <param name="GroupId">The id of the group whose association should be removed</param>
        public void DeleteGroup(int GroupId)
        {
            ATAMembershipProvider provider = Membership.Providers[this.ProviderName] as ATAMembershipProvider;
            if (provider != null)
            {
                provider.DeleteUserGroup(this.UserId, GroupId);
                RemoveUserSubscriptionFromGroup(GroupId, this.Email);
                this.LoadChildGroups();
            }
        }

        public void RemoveUserSubscriptionFromGroup(int GroupId, string userEmail)
        {
            ATAGroup group = new ATAGroup(GroupId);
            ATA.LyrisProxy.LyrisMember.DeleteMemberFromList(userEmail, group.LyrisListName);
        }

        public void DeleteGroupFromUser(int GroupId)
        { 
            foreach (UserGroup group in this._userGroups)
            {
                if (group.GroupId == GroupId)
                {
                    ATAMembershipProvider provider = Membership.Providers[this.ProviderName] as ATAMembershipProvider;
                    if (provider != null)
                    {
                        provider.DeleteUserGroup(this.UserId, GroupId);
                        RemoveUserSubscriptionFromGroup(GroupId,  this.Email); 
                    }
                    this._userGroups.Remove(group);
                    break;
                }
            } 
        }

        public void ClearGroups()
        {
            foreach (UserGroup userGroup in this._userGroups)
        {
                this.DeleteGroup(userGroup.GroupId);
            }
            this._userGroups.Clear();
        }

        #endregion

        #region UserIssue methods

        /// <summary>
        /// Creates an association between this user and the provided issue
        /// </summary>
        /// <param name="IssueId">The id of the [Issue] record to associate</param>
        public void AddIssue(int IssueId)
        {
            ATAMembershipProvider provider = Membership.Providers[this.ProviderName] as ATAMembershipProvider;
            if (provider != null)
            {
                provider.AddUserIssue(this.UserId, IssueId);
                this.LoadChildIssues();
            }
        }

        /// <summary>
        /// Deletes a [User] [Issue] association, the User record remains unchanged.
        /// </summary>
        /// <param name="IssueId">The id of the issue whose association should be removed</param>
        public void DeleteUserIssue(int IssueId)
        {
            ATAMembershipProvider provider = Membership.Providers[this.ProviderName] as ATAMembershipProvider;
            if (provider != null)
            {
                provider.DeleteUserIssue(this.UserId, IssueId);
                this.LoadChildIssues();
            }
        }

        public void ClearIssues()
        {
            foreach (UserIssue userIssue in this._userIssues)
            {
                this.DeleteUserIssue(userIssue.IssueId);
            }
            this._userIssues.Clear();
        }

        #endregion
         

        #region MemberType methods

        /// <summary>
        /// Creates an association between this user and the provided member type
        /// </summary>
        /// <param name="MemberTypeId">he id of the [MemberType] record to associate</param>
        public void AddMemberType(int MemberTypeId)
        {
            ATAMembershipProvider provider = Membership.Providers[this.ProviderName] as ATAMembershipProvider;
            if (provider != null)
            {
                provider.AddUserMemberType(this.UserId, MemberTypeId);
                this.LoadChildMemberTypes();
            }
        }

        /// <summary>
        /// Creates an association between this user and the provided member type
        /// </summary>
        /// <param name="MemberTypeId">he id of the [MemberType] record to associate</param>
        public void AddMemberTypes(int[] MemberTypeIds)
        {
            ATAMembershipProvider provider = Membership.Providers[this.ProviderName] as ATAMembershipProvider;
            if (provider != null)
            {
                foreach (int memberTypeId in MemberTypeIds)
                {
                    provider.AddUserMemberType(this.UserId, memberTypeId);
                    this.LoadChildMemberTypes();
                }
            }
        }

        /// <summary>
        /// Deletes a [User] [MemberType] association, the MemberType and User record remain unchanged.
        /// </summary>
        /// <param name="MemberTypeId"></param>
        public void DeleteMemberType(int MemberTypeId)
        {
            ATAMembershipProvider provider = Membership.Providers[this.ProviderName] as ATAMembershipProvider;
            if (provider != null)
            {
                provider.DeleteUserMemberType(this.UserId, MemberTypeId);
                this.LoadChildMemberTypes();
            }
        }

        public bool HasMemberType(int MemberTypeId)
        {
            bool rValue = false;
            foreach (UserMemberType memberType in this._userMemberTypes)
            {
                if (memberType.MemberTypeId == MemberTypeId)
                {
                    rValue = true;
                    break;
                }
            }
            return (rValue);
        }

        public void ClearMemberTypes()
        {
            foreach (UserMemberType memberType in this._userMemberTypes)
            {
                this.DeleteMemberType(memberType.MemberTypeId);
            }
            this._userMemberTypes.Clear();
        }

        #endregion

        #region UpdateMyProfileLastViewDate

        public void UpdateMyProfileLastViewDate()
        {
            ATAMembershipProvider provider = Membership.Providers[this.ProviderName] as ATAMembershipProvider;
            if (provider != null)
            {
                provider.SetMyProfileLastViewedDate(this.UserId);
            }
        }

        public void UpdateMyProfileLastViewDate(DateTime newDateTime)
        {
            ATAMembershipProvider provider = Membership.Providers[this.ProviderName] as ATAMembershipProvider;
            if (provider != null)
            {
                provider.SetMyProfileLastViewedDate(this.UserId, newDateTime);
            }
        }

        #endregion

        #region CanEditUser

        public bool CanEditUser(ATAMembershipUser editUser, bool IsMyProfilePage)
        {
            bool rValue = false;

            if (IsMyProfilePage && (this.UserId == editUser.UserId))
            {
                rValue = true;
            }
            else
            {
                //allowing editing of all users by ata staff
                if (this.IsATAStaff)
                {
                    rValue = true;
                }
                else
                {
                    //allow editing if this user is a sponsor of the edit user
                    foreach (SponsoredUser sponsoredUser in this.GetMySponsoredUsers(false, true))
                    {
                        if (sponsoredUser.UserId == editUser.UserId)
                        {
                            rValue = true;
                            break;
                        }
                    }
                }
            }

            return (rValue);
        }

        #endregion 
    }
}