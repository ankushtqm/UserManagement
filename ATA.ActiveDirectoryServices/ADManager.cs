#region Namespaces

using System.Diagnostics;
using Microsoft.VisualBasic;
using System;
using System.Data;
using System.Collections;
using System.DirectoryServices;
using System.Data.SqlClient;
using System.Text;
using System.Security.Principal;
using SusQtech.ActiveDirectoryServices;

#endregion

namespace SusQtech.ActiveDirectoryServices
{
    public class ADManager
    {
        #region Data Members

        private static ADManager _instance;

        #endregion

        #region Constructors

        /// <summary></summary>
        public ADManager()
        { }

        #endregion

        #region Static Instance

        /// <summary></summary>
        public static ADManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ADManager();

                return _instance;
            }
        }

        #endregion

        #region Private Properties

        /// <summary></summary>
        /// <param name="deCollection"></param>
        /// <returns></returns>
        private ArrayList Load(DirectoryEntries deCollection)
        {
            ArrayList list = new ArrayList();
            DirectoryEntry de;

            foreach (DirectoryEntry tempLoopVar_de in deCollection)
            {
                de = tempLoopVar_de;
                list.Add(Load(de));
            }

            return list;
        }

        /// <summary></summary>
        /// <param name="seCollection"></param>
        /// <returns></returns>
        private ArrayList Load(SearchResultCollection seCollection)
        {
            ArrayList list = new ArrayList();
            ADUser adUser;

            foreach (SearchResult se in seCollection)
            {
                adUser = Load(new DirectoryEntry(se.Path, Utility.ADUser, Utility.ADPassword, AuthenticationTypes.Secure));
                list.Add(adUser);
            }

            return list;
        }

        /// <summary></summary>
        /// <param name="seCollection"></param>
        /// <returns></returns>
        private ArrayList LoadGroup(SearchResultCollection seCollection)
        {
            ArrayList list = new ArrayList();
            SearchResult se;

            foreach (SearchResult tempLoopVar_se in seCollection)
            {
                se = tempLoopVar_se;
                list.Add(LoadGroup(new DirectoryEntry(se.Path, Utility.ADUser, Utility.ADPassword, AuthenticationTypes.Secure)));
            }

            return list;
        }

        private ArrayList LoadAllOrgUnits(SearchResultCollection seCollection)
        {
            ArrayList list = new ArrayList();

            foreach (SearchResult se in seCollection)
                list.Add(LoadOrgUnit(new DirectoryEntry(se.Path, Utility.ADUser, Utility.ADPassword, AuthenticationTypes.Secure)));

            return list;
        }

        private ArrayList LoadAllNodes(SearchResultCollection seCollection)
        {
            ArrayList list = new ArrayList();

            foreach (SearchResult se in seCollection)
            {
                switch (Convert.ToString(se.Properties["objectclass"][1]).ToLower())
                {
                    case "person":
                        list.Add(Load(new DirectoryEntry(se.Path, Utility.ADUser, Utility.ADPassword, AuthenticationTypes.Secure)));
                        break;
                    case "group":
                        list.Add(LoadGroup(new DirectoryEntry(se.Path, Utility.ADUser, Utility.ADPassword, AuthenticationTypes.Secure)));
                        break;
                    case "organizationalunit":
                        list.Add(LoadOrgUnit(new DirectoryEntry(se.Path, Utility.ADUser, Utility.ADPassword, AuthenticationTypes.Secure)));
                        break;
                }
            }

            return list;
        }

        /// <summary></summary>
        /// <param name="de"></param>
        /// <returns></returns>
        private ADOrgUnit LoadOrgUnit(DirectoryEntry de)
        {
            ADOrgUnit _ADNode = new ADOrgUnit();
            _ADNode.Name = Utility.GetProperty(de, "name");
            _ADNode.LDAPPath = de.Path;
            _ADNode.DistinguishedName = Utility.ADPath + "/" + Utility.GetProperty(de, "distinguishedName");
            _ADNode.OU = Utility.GetProperty(de, "ou");
            _ADNode.InstanceType = Utility.GetProperty(de, "instanceType");
            _ADNode.ObjectCategory = Utility.GetProperty(de, "objectCategory");
            _ADNode.ObjectGUID = Utility.GetProperty(de, "objectGUID");
            return _ADNode;
        }

        /// <summary></summary>
        /// <param name="de"></param>
        /// <returns></returns>
        private ADGroup LoadGroup(DirectoryEntry de)
        {
            ADGroup _ADGroup = new ADGroup();
            _ADGroup.Name = Utility.GetProperty(de, "cn");
            _ADGroup.DisplayName = Utility.GetProperty(de, "DisplayName");
            _ADGroup.DistinguishedName = Utility.ADPath + "/" + Utility.GetProperty(de, "DistinguishedName");
            _ADGroup.Description = Utility.GetProperty(de, "Description");
            return _ADGroup;
        }

        /// <summary></summary>
        /// <returns></returns>
        private static SearchResultCollection GetUsers()
        {
            DirectoryEntry de = Utility.GetDirectoryObject();
            DirectorySearcher deSearch = new DirectorySearcher();
            deSearch.SearchRoot = de;
            deSearch.Filter = "(&(objectClass=user)(objectCategory=person))";
            deSearch.SearchScope = SearchScope.Subtree;
            return deSearch.FindAll();
        }

        private static SearchResultCollection GetAllOrgUnits()
        {
            DirectoryEntry de = Utility.GetDirectoryObject();
            DirectorySearcher deSearch = new DirectorySearcher();
            deSearch.SearchRoot = de;
            deSearch.Filter = "(&(objectClass=organizationalUnit))";
            deSearch.SearchScope = SearchScope.Subtree;
            return deSearch.FindAll();
        }

        private static SearchResultCollection GetAllNodes()
        {
            DirectoryEntry de = Utility.GetDirectoryObject();
            DirectorySearcher deSearch = new DirectorySearcher();
            deSearch.SearchRoot = de;
            deSearch.Filter = "(|(objectClass=organizationalUnit)(objectClass=group)(objectClass=user))";
            deSearch.SearchScope = SearchScope.Subtree;
            return deSearch.FindAll();
        }

        /// <summary></summary>
        /// <returns></returns>
        private static SearchResultCollection GetGroups()
        {
            DataSet dsGroup = new DataSet();
            DirectoryEntry de = Utility.GetDirectoryObject();
            DirectorySearcher deSearch = new DirectorySearcher();
            deSearch.SearchRoot = de;
            deSearch.Filter = "(&(objectClass=group))";
            return deSearch.FindAll();
        }

        /// <summary></summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        private static DirectoryEntry GetUser(string UserName)
        {
            DirectoryEntry de = Utility.GetDirectoryObject();
            DirectorySearcher deSearch = new DirectorySearcher();
            deSearch.SearchRoot = de;
            deSearch.Filter = "(&(objectClass=user)(sAMAccountName=" + UserName + "))";
            deSearch.SearchScope = SearchScope.Subtree;
            SearchResult results = deSearch.FindOne();

            if (!(results == null))
                return new DirectoryEntry(results.Path, Utility.ADUser, Utility.ADPassword, AuthenticationTypes.Secure);
            else
                return null;
        }

        /// <summary></summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        private static DirectoryEntry GetUserByEmail(string email)
        {
            DirectoryEntry de = Utility.GetDirectoryObject();
            DirectorySearcher deSearch = new DirectorySearcher();
            deSearch.SearchRoot = de;
            deSearch.Filter = "(&(objectClass=user)(mail=" + email + "))";
            deSearch.SearchScope = SearchScope.Subtree;
            SearchResult results = deSearch.FindOne();

            if (!(results == null))
                return new DirectoryEntry(results.Path, Utility.ADUser, Utility.ADPassword, AuthenticationTypes.Secure);
            else
                return null;
        }

        /// <summary></summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        private static DirectoryEntry GetContact(string CommonName)
        {
            DirectoryEntry de = Utility.GetDirectoryObject();
            DirectorySearcher deSearch = new DirectorySearcher();
            deSearch.SearchRoot = de;
            deSearch.Filter = "(&(objectClass=contact)(CN=" + CommonName + "))";
            deSearch.SearchScope = SearchScope.Subtree;
            SearchResult results = deSearch.FindOne();

            if (!(results == null))
                return new DirectoryEntry(results.Path, Utility.ADUser, Utility.ADPassword, AuthenticationTypes.Secure);
            else
                return null;
        }

        /// <summary></summary>
        /// <param name="de"></param>
        /// <returns></returns>
        private ADUser Load(DirectoryEntry de)
        {
            if (de != null)
            {
                ADUser ADUser = new ADUser();
                ADUser.FirstName = Utility.GetProperty(de, "givenName");
                ADUser.MiddleInitial = Utility.GetProperty(de, "initials");
                ADUser.LastName = Utility.GetProperty(de, "sn");
                ADUser.UserPrincipalName = Utility.GetProperty(de, "UserPrincipalName");
                ADUser.PostalAddress = Utility.GetProperty(de, "PostalAddress");
                ADUser.MailingAddress = Utility.GetProperty(de, "StreetAddress");
                ADUser.ResidentialAddress = Utility.GetProperty(de, "HomePostalAddress");
                ADUser.Title = Utility.GetProperty(de, "Title");
                ADUser.HomePhone = Utility.GetProperty(de, "HomePhone");
                ADUser.OfficePhone = Utility.GetProperty(de, "TelephoneNumber");
                ADUser.Mobile = Utility.GetProperty(de, "Mobile");
                ADUser.Fax = Utility.GetProperty(de, "FacsimileTelephoneNumber");
                ADUser.Email = Utility.GetProperty(de, "mail");
                ADUser.Url = Utility.GetProperty(de, "Url");
                ADUser.UserName = Utility.GetProperty(de, "sAMAccountName");
                ADUser.DistinguishedName = Utility.ADPath + "/" + Utility.GetProperty(de, "DistinguishedName");
                ADUser.AffiliateID = Utility.GetProperty(de, "extensionAttribute11");
                ADUser.AffiliateName = Utility.GetProperty(de, "physicalDeliveryOfficeName");
                try
                {
                    ADUser.IsAccountActive = Utility.IsAccountActive(Convert.ToInt32(Utility.GetProperty(de, "userAccountControl")));
                }
                catch
                {
                    ADUser.IsAccountActive = false;
                }
                return ADUser;
            }
            else
            {
                return null;
            }
        }

        /// <summary></summary>
        /// <param name="Name"></param>
        /// <param name="DisplayName"></param>
        /// <param name="DistinguishedName"></param>
        /// <param name="Description"></param>
        /// <returns></returns>
        private DirectoryEntry AddGroup(string Name, string DisplayName, string DistinguishedName, string Description)
        {
            string RootDSE = Utility.ADPath + "/";
            try
            {
                RootDSE = RootDSE + Utility.ADUsersPath;
                RootDSE = RootDSE + Utility.GetLDAPDomain();

                DirectoryEntry myDE = new DirectoryEntry(RootDSE, Utility.ADUser, Utility.ADPassword, AuthenticationTypes.Secure);
                DirectoryEntries myEntries = myDE.Children;
                DirectoryEntry myDirectoryEntry = myEntries.Add("CN=" + Name, "Group");
                Utility.SetProperty(myDirectoryEntry, "cn", Name);
                Utility.SetProperty(myDirectoryEntry, "DisplayName", DisplayName);
                Utility.SetProperty(myDirectoryEntry, "Description", Description);
                Utility.SetProperty(myDirectoryEntry, "sAMAccountName", Name);
                Utility.SetProperty(myDirectoryEntry, "groupType", System.Convert.ToString(Utility.GroupScope.ADS_GROUP_TYPE_GLOBAL_GROUP));

                myDirectoryEntry.CommitChanges();
                myDirectoryEntry = Utility.GetGroup(Name);
                return myDirectoryEntry;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        /// <summary></summary>
        /// <param name="FirstName"></param>
        /// <param name="MiddleInitial"></param>
        /// <param name="LastName"></param>
        /// <param name="UserPrincipalName"></param>
        /// <param name="PostalAddress"></param>
        /// <param name="MailingAddress"></param>
        /// <param name="ResidentialAddress"></param>
        /// <param name="Title"></param>
        /// <param name="HomePhone"></param>
        /// <param name="OfficePhone"></param>
        /// <param name="Mobile"></param>
        /// <param name="Fax"></param>
        /// <param name="Email"></param>
        /// <param name="Url"></param>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <param name="DistinguishedName"></param>
        /// <param name="IsAccountActive"></param>
        /// <param name="OU"></param>
        /// <returns></returns>
        private DirectoryEntry Add(string CommonName, string DisplayName, string FirstName, string MiddleInitial, string LastName, string UserPrincipalName, string PostalAddress, string MailingAddress, string ResidentialAddress, string Title, string HomePhone, string OfficePhone, string Mobile, string Fax, string Email, string Url, string UserName, string Password, string DistinguishedName, string Company, bool IsAccountActive, string OU, string AffiliateName, string AffiliateID, string AffiliationName, string PrimaryRelationship, string ConfirmationContactEmail, string AffiliateBoardMember, string NationalBoardMember, string SecurityQuestion, string SecurityAnswer, string BusinessAddressStreet, string BusinessAddressCity, string BusinessAddressState, string BusinessAddressZip, string BusinessAddressCountry)
        {
            string RootDSE = Utility.ADPath + "/" + Utility.ADUsersPath + Utility.GetLDAPDomain();
            try
            {
                if (!string.IsNullOrEmpty(OU))
                    RootDSE = OU;

                DirectoryEntry myDE = new DirectoryEntry(RootDSE, Utility.ADUser, Utility.ADPassword, AuthenticationTypes.Secure);
                DirectoryEntries myEntries = myDE.Children;
                //DirectoryEntry myDirectoryEntry = myEntries.Add("CN=" + FirstName + " " + LastName, "user");
                DirectoryEntry myDirectoryEntry = myEntries.Add("CN=" + CommonName, "user");

                //Utility.SetProperty(myDirectoryEntry, "displayname", FirstName + " " + LastName);
                Utility.SetProperty(myDirectoryEntry, "displayname", DisplayName);
                Utility.SetProperty(myDirectoryEntry, "givenName", FirstName);
                Utility.SetProperty(myDirectoryEntry, "initials", MiddleInitial);
                Utility.SetProperty(myDirectoryEntry, "sn", LastName);
                Utility.SetProperty(myDirectoryEntry, "UserPrincipalName", UserName + "@" + Utility.ADDomainName);
                Utility.SetProperty(myDirectoryEntry, "PostalAddress", PostalAddress);
                Utility.SetProperty(myDirectoryEntry, "StreetAddress", MailingAddress);
                Utility.SetProperty(myDirectoryEntry, "HomePostalAddress", ResidentialAddress);
                Utility.SetProperty(myDirectoryEntry, "Title", Title);
                Utility.SetProperty(myDirectoryEntry, "HomePhone", HomePhone);
                Utility.SetProperty(myDirectoryEntry, "TelephoneNumber", OfficePhone);
                Utility.SetProperty(myDirectoryEntry, "Mobile", Mobile);
                Utility.SetProperty(myDirectoryEntry, "FacsimileTelephoneNumber", Fax);
                Utility.SetProperty(myDirectoryEntry, "mail", Email);
                Utility.SetProperty(myDirectoryEntry, "Url", Url);
                Utility.SetProperty(myDirectoryEntry, "sAMAccountName", UserName);
                Utility.SetProperty(myDirectoryEntry, "UserPassword", Password);
                Utility.SetProperty(myDirectoryEntry, "physicalDeliveryOfficeName", AffiliateName);
                Utility.SetProperty(myDirectoryEntry, "Company", Company);
                Utility.SetProperty(myDirectoryEntry, "extensionAttribute5", PrimaryRelationship);
                Utility.SetProperty(myDirectoryEntry, "extensionAttribute6", ConfirmationContactEmail);
                Utility.SetProperty(myDirectoryEntry, "extensionAttribute7", AffiliateBoardMember);
                Utility.SetProperty(myDirectoryEntry, "extensionAttribute8", NationalBoardMember);
                Utility.SetProperty(myDirectoryEntry, "extensionAttribute9", SecurityQuestion);
                Utility.SetProperty(myDirectoryEntry, "extensionAttribute10", SecurityAnswer);
                Utility.SetProperty(myDirectoryEntry, "extensionAttribute11", AffiliateID);
                Utility.SetProperty(myDirectoryEntry, "streetAddress", BusinessAddressStreet);
                Utility.SetProperty(myDirectoryEntry, "l", BusinessAddressCity);
                Utility.SetProperty(myDirectoryEntry, "st", BusinessAddressState);
                Utility.SetProperty(myDirectoryEntry, "postalCode", BusinessAddressZip);
                Utility.SetProperty(myDirectoryEntry, "co", BusinessAddressCountry);

                myDirectoryEntry.Properties["userAccountControl"].Value = Utility.UserStatus.Enable;
                myDirectoryEntry.CommitChanges();
                myDirectoryEntry = GetUser(UserName);
                Utility.SetUserPassword(myDirectoryEntry, Password);
                return myDirectoryEntry;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public void MoveUser(ADUser user, string newPath)
        {
            DirectoryEntry entry = GetUser(user.UserName);
            //new DirectoryEntry(user.DistinguishedName, Utility.ADUser, Utility.ADPassword, AuthenticationTypes.Secure);
            DirectoryEntry newParent = new DirectoryEntry(newPath, Utility.ADUser, Utility.ADPassword, AuthenticationTypes.Secure);
            entry.MoveTo(newParent);
        }


        #endregion

        #region Contact stuff

        public ADContact CreateADContact(string OU, string FirstName, string LastName, string Email)
        {
            ADContact newContact = null;
            string RootDSE = Utility.ADPath + "/" + Utility.ADUsersPath + Utility.GetLDAPDomain();

            if (!string.IsNullOrEmpty(OU))
                RootDSE = OU;

            DirectoryEntry myDE = new DirectoryEntry(RootDSE, Utility.ADUser, Utility.ADPassword, AuthenticationTypes.Secure);
            DirectoryEntries myEntries = myDE.Children;

            string commonName = string.Format("{0} {1}", FirstName, LastName);

            DirectoryEntry newDirectoryEntry = myEntries.Add("CN=" + commonName, "contact");
            newDirectoryEntry.CommitChanges();
            Utility.SetProperty(newDirectoryEntry, "givenName", FirstName);
            Utility.SetProperty(newDirectoryEntry, "sn", LastName);
            Utility.SetProperty(newDirectoryEntry, "mail", Email);
            Utility.SetProperty(newDirectoryEntry, "displayName", commonName);
            newDirectoryEntry.CommitChanges();

            newContact = LoadContact(newDirectoryEntry);

            return (newContact);
        }

        private ADContact LoadContact(DirectoryEntry de)
        {
            ADContact contact = null;
            if (de != null)
            {
                contact = new ADContact();
                contact.CommonName = Utility.GetProperty(de, "CN");
                contact.FirstName = Utility.GetProperty(de, "givenName");
                contact.LastName = Utility.GetProperty(de, "sn");
                contact.Email = Utility.GetProperty(de, "mail");
                contact.DisplayName = Utility.GetProperty(de, "displayName");
            }
            return (contact);
        }

        public ADContact LoadContact(string CommonName)
        {
            return (LoadContact(GetContact(CommonName)));
        }

        #endregion

        #region Public Properties

        /// <summary></summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public ADUser LoadUser(string userName)
        {
            return Load(GetUser(userName));
        }

        public ADUser LoadUserByEmail(string email)
        {
            return Load(GetUserByEmail(email));
        }

        public object LoadByPath(string ldapPath)
        {
            DirectoryEntry de = new DirectoryEntry(ldapPath, Utility.ADUser, Utility.ADPassword, AuthenticationTypes.Secure);
            switch (de.SchemaClassName.ToLower())
            {
                case "user":
                    return Load(de); 
                case "contact":
                    return (LoadContact(de)); 
                case "organizationalunit":
                    return LoadOrgUnit(de); 
                case "group":
                    return LoadGroup(de); 
                default:
                    return null; 
            }
        }

        /// <summary></summary>
        /// <param name="GroupName"></param>
        /// <returns></returns>
        public ADGroup LoadGroup(string GroupName)
        {
            return LoadGroup(Utility.GetGroup(GroupName));
        }

        /// <summary></summary>
        /// <param name="GroupName"></param>
        /// <returns></returns>
        public ADGroup WorkingLoadGroup(string GroupName)
        {
            return LoadGroup(Utility.WorkingGetGroup(GroupName));
        }

        /// <summary></summary>
        /// <returns></returns>
        public ArrayList LoadAllUsers()
        {
            return Load(GetUsers());
        }

        /// <summary></summary>
        /// <returns></returns>
        public ArrayList LoadAllOrgUnits()
        {
            return LoadAllOrgUnits(GetAllOrgUnits());
        }

        public ArrayList LoadAllNodes()
        {
            return LoadAllNodes(GetAllNodes());
        }

        /// <summary></summary>
        /// <returns></returns>
        public ArrayList LoadAllGroups()
        {
            return LoadGroup(GetGroups());
        }

        /// <summary></summary>
        /// <param name="ADUser"></param>
        /// <returns></returns>
        public ADUser CreateADUser(ADUser ADUser)
        {
            return Load(Add(ADUser.CommonName, ADUser.DisplayName, ADUser.FirstName, ADUser.MiddleInitial, ADUser.LastName, ADUser.UserPrincipalName, ADUser.PostalAddress, ADUser.MailingAddress, ADUser.ResidentialAddress, ADUser.Title, ADUser.HomePhone, ADUser.OfficePhone, ADUser.Mobile, ADUser.Fax, ADUser.Email, ADUser.Url, ADUser.UserName, ADUser.Password, ADUser.DistinguishedName, ADUser.Company, ADUser.IsAccountActive, ADUser.OU, ADUser.AffiliateName, ADUser.AffiliateID, ADUser.AffiliationName, ADUser.PrimaryRelationship, ADUser.ConfirmationContactEmail, ADUser.AffiliateBoardMember, ADUser.NationalBoardMember, ADUser.SecurityQuestion, ADUser.SecurityAnswer, ADUser.BusinessAddressStreet, ADUser.BusinessAddressCity, ADUser.BusinessAddressState, ADUser.BusinessAddressZip, ADUser.BusinessAddressCountry));
        }

        /// <summary></summary>
        /// <param name="ADGroup"></param>
        /// <returns></returns>
        public ADGroup CreateADGroup(ADGroup ADGroup)
        {
            return LoadGroup(AddGroup(ADGroup.Name, ADGroup.DisplayName, ADGroup.DistinguishedName, ADGroup.Description));
        }

        /// <summary></summary>
        /// <param name="UserDistinguishedName"></param>
        /// <param name="GroupDistinguishedName"></param>
        public void AddUserToGroup(string UserDistinguishedName, string GroupDistinguishedName)
        {
            DirectoryEntry oGroup = Utility.GetDirectoryObjectByDistinguishedName(GroupDistinguishedName);
            oGroup.Invoke("Add", new object[] { UserDistinguishedName });
            oGroup.Close();
        }

        /// <summary></summary>
        /// <param name="UserDistinguishedName"></param>
        /// <param name="GroupDistinguishedName"></param>
        public void RemoveUserFromGroup(string UserDistinguishedName, string GroupDistinguishedName)
        {
            DirectoryEntry oGroup = Utility.GetDirectoryObjectByDistinguishedName(GroupDistinguishedName);
            oGroup.Invoke("Remove", new object[] { UserDistinguishedName });
            oGroup.Close();
        }

        /// <summary></summary>
        /// <param name="UserName"></param>
        public void DisableUserAccount(string UserName)
        {
            Utility.DisableUserAccount(GetUser(UserName));
        }

        /// <summary></summary>
        /// <param name="UserName"></param>
        public void EnableUserAccount(string UserName)
        {
            Utility.EnableUserAccount(GetUser(UserName));
        }

        /// <summary></summary>
        /// <param name="UserName"></param>
        public void DeleteUserAccount(string UserName)
        {
            DirectoryEntry de = GetUser(UserName);
            de.DeleteTree();
            de.CommitChanges();
        }

        /// <summary></summary>
        /// <param name="usrName"></param>
        /// <param name="newUserPass"></param>
        public void UpdateUserPassword(string usrName, string newUserPass)
        {
            try
            {
                ADUser adu = ADManager.Instance.LoadUser(usrName);
                adu.SetPassword(usrName, newUserPass);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary></summary>
        /// <param name="GroupName"></param>
        public void DeleteGroupAccount(string GroupName)
        {
            DirectoryEntry de = Utility.GetGroup(GroupName);
            de.DeleteTree();
            de.CommitChanges();
        }

        /// <summary></summary>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public bool IsUserValid(string UserName, string Password)
        {
            try
            {
                DirectoryEntry deUser = Utility.GetUser(UserName, Password);
                deUser.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string AuthenticateUser(string email, string password)
        {
            DirectoryEntry de = Utility.GetDirectoryObject();
            DirectorySearcher deSearch = new DirectorySearcher();
            deSearch.SearchRoot = de;
            deSearch.Filter = "(&(objectClass=user)(mail=" + email + "))";
            deSearch.SearchScope = SearchScope.Subtree;
            SearchResult results = deSearch.FindOne();

            if (!(results == null))
                return Convert.ToString(results.Properties["sAMAccountName"][0]);
            else
                return string.Empty;
        }

        /// <summary></summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public bool UserExists(string UserName)
        {
            DirectoryEntry de = Utility.GetDirectoryObject();
            DirectorySearcher deSearch = new DirectorySearcher();
            deSearch.SearchRoot = de;
            deSearch.SearchScope = SearchScope.Subtree;
            deSearch.Filter = "(&(objectClass=user) (sAMAccountName=" + UserName + "))";
            SearchResultCollection results = deSearch.FindAll();

            if (results.Count == 0)
                return false;
            else
                return true;
        }

        /// <summary></summary>
        /// <param name="GroupName"></param>
        /// <returns></returns>
        public bool GroupExists(string GroupName)
        {
            DirectoryEntry de = Utility.GetDirectoryObject();
            DirectorySearcher deSearch = new DirectorySearcher();
            deSearch.SearchRoot = de;
            deSearch.Filter = "(&(objectClass=group) (cn=" + GroupName + "))";
            SearchResultCollection results = deSearch.FindAll();

            if (results.Count == 0)
                return false;
            else
                return true;
        }

        /// <param name="GroupName"></param>
        /// <returns></returns>
        public bool WorkingGroupExists(string GroupName)
        {
            string fullpath = string.Format("LDAP://{0}/{1}{2}", Utility.ADDomainName, Utility.ADUsersPath, Utility.GetLDAPDomain());
            DirectoryEntry de = new DirectoryEntry(fullpath, Utility.ADUser, Utility.ADPassword, AuthenticationTypes.Secure);
            DirectorySearcher deSearch = new DirectorySearcher();
            deSearch.SearchRoot = de;
            deSearch.Filter = "(&(objectClass=group) (cn=" + GroupName + "))";
            deSearch.SearchScope = SearchScope.Subtree;
            SearchResultCollection results = deSearch.FindAll();

            if (results.Count == 0)
                return false;
            else
                return true;
        }

        /// <summary></summary>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public Utility.LoginResult Login(string UserName, string Password)
        {
            if (IsUserValid(UserName, Password))
            {
                DirectoryEntry de = GetUser(UserName);
                if (!(de == null))
                {
                    if (Utility.UserStatus.Enable == (Utility.UserStatus)(de.Properties["userAccountControl"][0]))
                    {
                        de.Close();
                        return Utility.LoginResult.LOGIN_OK;
                    }
                    else
                    {
                        de.Close();
                        return Utility.LoginResult.LOGIN_USER_ACCOUNT_INACTIVE;
                    }

                }
                else
                    return Utility.LoginResult.LOGIN_USER_DOESNT_EXIST;
            }
            else
                return Utility.LoginResult.LOGIN_USER_DOESNT_EXIST;
        }

        #endregion
    }
}