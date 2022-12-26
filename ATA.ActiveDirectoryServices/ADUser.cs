#region Namespaces

using System.Diagnostics;
using Microsoft.VisualBasic;
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using SusQtech.ActiveDirectoryServices;
using System.DirectoryServices;
using System.Data.SqlClient;
using System.Text;
using System.Security.Principal;
using System.Xml.Serialization;
using System.Reflection;

#endregion

namespace SusQtech.ActiveDirectoryServices
{
    [Serializable()]
    public class ADUser
    {
        #region Private Property Variables

        private string _FirstName; //givenName
        private string _MiddleInitial; //initials
        private string _LastName; //sn
        private string _CommonName; //CN
        private string _DisplayName; //Name
        private string _UserPrincipalName; //userPrincipalName (e.g. user@domain.local)
        private string _PostalAddress;
        private string _MailingAddress; //StreetAddress
        private string _ResidentialAddress; //HomePostalAddress
        private string _Title;
        private string _HomePhone;
        private string _OfficePhone; //TelephoneNumber
        private string _Mobile;
        private string _Fax; //FacsimileTelephoneNumber
        private string _Email; //mail
        private string _Url;
        private string _UserName; //sAMAccountName
        private string _OU;
        private string _Password;
        private string _Company;
        private string _DistinguishedName;
        private bool _IsAccountActive; //userAccountControl
        private ArrayList _Groups;
        private string _ldapPath;
        private string _AffiliateName;
        private string _AffiliateID;
        private string _AffiliationName;
        private string _PrimaryRelationship;
        private string _ConfirmationContactEmail;
        private string _AffiliateBoardMember;
        private string _NationalBoardMember;
        private string _SecurityQuestion;
        private string _SecurityAnswer;
        private string _BusinessAddressStreet;
        private string _BusinessAddressCity;
        private string _BusinessAddressState;
        private string _BusinessAddressZip;
        private string _BusinessAddressCountry;

        #endregion

        #region Public Properties

        /// <summary></summary>
        public string LDAPPath
        {
            get { return _ldapPath; }
            set { _ldapPath = value; }
        }

        /// <summary></summary>
        public string FirstName
        {
            get { return _FirstName; }
            set { _FirstName = value; }
        }

        /// <summary></summary>
        public string MiddleInitial
        {
            get { return _MiddleInitial; }
            set
            {
                if (value.Length > 6)
                    throw (new Exception("MiddleInitial cannot be more than six characters"));
                else
                    _MiddleInitial = value;
            }
        }

        /// <summary></summary>
        public string LastName
        {
            get { return _LastName; }
            set { _LastName = value; }
        }

        /// <summary></summary>
        public string CommonName
        {
            get { return _CommonName; }
            set { _CommonName = value; }
        }

        /// <summary></summary>
        public string DisplayName
        {
            get { return _DisplayName; }
            set { _DisplayName = value; }
        }

        /// <summary></summary>
        public string UserPrincipalName
        {
            get { return _UserPrincipalName; }
            set { _UserPrincipalName = value; }
        }

        /// <summary></summary>
        public string PostalAddress
        {
            get { return _PostalAddress; }
            set { _PostalAddress = value; }
        }

        /// <summary></summary>
        public string MailingAddress
        {
            get { return _MailingAddress; }
            set { _MailingAddress = value; }
        }

        /// <summary></summary>
        public string ResidentialAddress
        {
            get { return _ResidentialAddress; }
            set { _ResidentialAddress = value; }
        }

        /// <summary></summary>
        public string Title
        {
            get { return _Title; }
            set { _Title = value; }
        }

        /// <summary></summary>
        public string HomePhone
        {
            get { return _HomePhone; }
            set { _HomePhone = value; }
        }

        /// <summary></summary>
        public string OfficePhone
        {
            get { return _OfficePhone; }
            set { _OfficePhone = value; }
        }

        /// <summary></summary>
        public string Mobile
        {
            get { return _Mobile; }
            set { _Mobile = value; }
        }

        /// <summary></summary>
        public string Fax
        {
            get { return _Fax; }
            set { _Fax = value; }
        }

        /// <summary></summary>
        public string Email
        {
            get { return _Email; }
            set { _Email = value; }
        }

        /// <summary></summary>
        public string Url
        {
            get { return _Url; }
            set { _Url = value; }
        }

        /// <summary></summary>
        public string UserName
        {
            get { return _UserName; }
            set { _UserName = value; }
        }

        /// <summary></summary>
        public string Company
        {
            get { return _Company; }
            set { _Company = value; }
        }

        /// <summary></summary>
        public string Password
        {
            get { return _Password; }
            set { _Password = value; }
        }

        /// <summary></summary>
        public string DistinguishedName
        {
            get { return _DistinguishedName; }
            set { _DistinguishedName = value; }
        }

        /// <summary></summary>
        public string OU
        {
            get { return _OU; }
            set { _OU = value; }
        }

        /// <summary></summary>
        public bool IsAccountActive
        {
            get { return _IsAccountActive; }
            set { _IsAccountActive = value; }
        }

        public string AffiliateName
        {
            get { return _AffiliateName; }
            set { _AffiliateName = value; }
        }

        public string AffiliateID
        {
            get { return _AffiliateID; }
            set { _AffiliateID = value; }
        }

        public string AffiliationName
        {
            get { return _AffiliationName; }
            set { _AffiliationName = value; }
        }

        /// <summary></summary>
        public ArrayList Groups
        {
            get
            {
                if (_Groups == null)
                    _Groups = ADGroup.LoadByUser(DistinguishedName);

                return _Groups;
            }
            set { _Groups = value; }
        }

        public string PrimaryRelationship
        {
            get { return _PrimaryRelationship; }
            set { _PrimaryRelationship = value; }
        }

        public string ConfirmationContactEmail
        {
            get { return _ConfirmationContactEmail; }
            set { _ConfirmationContactEmail = value; }
        }

        public string AffiliateBoardMember
        {
            get { return _AffiliateBoardMember; }
            set { _AffiliateBoardMember = value; }
        }

        public string NationalBoardMember
        {
            get { return _NationalBoardMember; }
            set { _NationalBoardMember = value; }
        }

        public string SecurityQuestion
        {
            get { return _SecurityQuestion; }
            set { _SecurityQuestion = value; }
        }

        public string SecurityAnswer
        {
            get { return _SecurityAnswer; }
            set { _SecurityAnswer = value; }
        }

        public string BusinessAddressStreet
        {
            get { return _BusinessAddressStreet; }
            set { _BusinessAddressStreet = value; }
        }

        public string BusinessAddressCity
        {
            get { return _BusinessAddressCity; }
            set { _BusinessAddressCity = value; }
        }

        public string BusinessAddressState
        {
            get { return _BusinessAddressState; }
            set { _BusinessAddressState = value; }
        }

        public string BusinessAddressZip
        {
            get { return _BusinessAddressZip; }
            set { _BusinessAddressZip = value; }
        }

        public string BusinessAddressCountry
        {
            get { return _BusinessAddressCountry; }
            set { _BusinessAddressCountry = value; }
        }

        #endregion

        #region Static Functions

        /// <summary></summary>
        /// <param name="DistinguishedName"></param>
        /// <returns></returns>
        internal static ArrayList LoadByGroup(string DistinguishedName)
        {
            return GetUsers(DistinguishedName);
        }

        /// <summary></summary>
        /// <param name="DistinguishedName"></param>
        /// <returns></returns>
        internal static Dictionary<string, ADUser> LoadAllByGroup(string DistinguishedName)
        {
            return GetAllUsers(DistinguishedName);
        }

        #endregion

        #region Public Functions

        /// <summary></summary>
        public void Update()
        {
            try
            {
                DirectoryEntry de = GetDirectoryObject(UserName);
                Utility.SetProperty(de, "displayName", DisplayName);
                Utility.SetProperty(de, "givenName", FirstName);
                Utility.SetProperty(de, "initials", MiddleInitial);
                Utility.SetProperty(de, "sn", LastName);
                Utility.SetProperty(de, "UserPrincipalName", UserName + "@" + Utility.ADDomainName);
                Utility.SetProperty(de, "PostalAddress", PostalAddress);
                Utility.SetProperty(de, "StreetAddress", MailingAddress);
                Utility.SetProperty(de, "HomePostalAddress", ResidentialAddress);
                Utility.SetProperty(de, "Title", Title);
                Utility.SetProperty(de, "HomePhone", HomePhone);
                Utility.SetProperty(de, "TelephoneNumber", OfficePhone);
                Utility.SetProperty(de, "Mobile", Mobile);
                Utility.SetProperty(de, "FacsimileTelephoneNumber", Fax);
                Utility.SetProperty(de, "mail", Email);
                Utility.SetProperty(de, "Url", Url);
                Utility.SetProperty(de, "sAMAccountName", UserName);
                Utility.SetProperty(de, "UserPassword", Password);
                Utility.SetProperty(de, "physicalDeliveryOfficeName", AffiliateName);
                Utility.SetProperty(de, "Company", Company);
                Utility.SetProperty(de, "extensionAttribute5", PrimaryRelationship);
                Utility.SetProperty(de, "extensionAttribute6", ConfirmationContactEmail);
                Utility.SetProperty(de, "extensionAttribute7", AffiliateBoardMember);
                Utility.SetProperty(de, "extensionAttribute8", NationalBoardMember);
                Utility.SetProperty(de, "extensionAttribute9", SecurityQuestion);
                Utility.SetProperty(de, "extensionAttribute10", SecurityAnswer);
                Utility.SetProperty(de, "extensionAttribute11", AffiliateID);
                Utility.SetProperty(de, "streetAddress", BusinessAddressStreet);
                Utility.SetProperty(de, "l", BusinessAddressCity);
                Utility.SetProperty(de, "st", BusinessAddressState);
                Utility.SetProperty(de, "postalCode", BusinessAddressZip);
                Utility.SetProperty(de, "co", BusinessAddressCountry);

                if (IsAccountActive == true)
                    de.Properties["userAccountControl"][0] = Utility.UserStatus.Enable;
                else
                    de.Properties["userAccountControl"][0] = Utility.UserStatus.Disable;

                de.CommitChanges();
            }
            catch (Exception ex)
            {
                throw (new Exception("User cannot be updated" + ex.Message));
            }
        }

        /// <summary></summary>
        /// <param name="newPassword"></param>
        public void SetPassword(string newPassword)
        {
            try
            {
                DirectoryEntry de = GetDirectoryObject(UserName);
                Utility.SetUserPassword(de, newPassword);
                de.CommitChanges();
            }
            catch (Exception ex)
            {
                throw (new Exception("User Password cannot be set" + ex.Message));
            }
        }

        /// <summary></summary>
        /// <param name="UserName"></param>
        /// <param name="newPassword"></param>
        public void SetPassword(string UserName, string newPassword)
        {
            try
            {
                DirectoryEntry de = GetDirectoryObject(UserName);
                Utility.SetUserPassword(de, newPassword);
                de.CommitChanges();
            }
            catch (Exception ex)
            {
                throw (new Exception("User Password cannot be set" + ex.Message));
            }
        }

        #endregion

        #region Private Functions

        /// <summary></summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        private DirectoryEntry GetDirectoryObject(string UserName)
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
        /// <param name="DistinguishedName"></param>
        /// <returns></returns>
        private static ArrayList GetUsers(string DistinguishedName)
        {
            DirectoryEntry _de = Utility.GetDirectoryObjectByDistinguishedName(DistinguishedName);
            ArrayList list = new ArrayList();

            for (int index = 0; index <= _de.Properties["member"].Count - 1; index++)
            {
                ADUser user = Load(Utility.GetDirectoryObjectByDistinguishedName(Utility.ADPath + "/" + _de.Properties["member"][index].ToString()));
                if (!list.Contains(user))
                    list.Add(user);
            }

            return list;
        }

        /// <summary></summary>
        /// <param name="DistinguishedName"></param>
        /// <returns>a dictionary with the key = the username.ToLower() and the value = the ADUser object</returns>
        private static Dictionary<string, ADUser> GetAllUsers(string DistinguishedName)
        {
            DirectoryEntry _de = Utility.GetDirectoryObjectByDistinguishedName(DistinguishedName);
            Dictionary<string, ADUser> list = new Dictionary<string, ADUser>();

            for (int i = 0; i < _de.Properties["member"].Count; i++)
            {
                DirectoryEntry candidate = Utility.GetDirectoryObjectByDistinguishedName(Utility.ADPath + "/" + _de.Properties["member"][i].ToString());
                if (candidate.SchemaClassName.Equals("group"))
                {
                    Dictionary<string, ADUser>.Enumerator enumer = GetAllUsers(candidate.Path).GetEnumerator();
                    while (enumer.MoveNext())
                    {
                        if (!list.ContainsKey(enumer.Current.Key.ToLower()))
                            list.Add(enumer.Current.Key.ToLower(), enumer.Current.Value);
                    }
                }
                else
                {
                    ADUser u = Load(candidate);
                    if (!list.ContainsKey(u.UserName.ToLower()))
                        list.Add(u.UserName.ToLower(), u);
                }
            }

            return list;
        }

        /// <summary></summary>
        /// <param name="de"></param>
        /// <returns></returns>
        private static ADUser Load(DirectoryEntry de)
        {
            ADUser ADUser = new ADUser();
            try
            {
                ADUser.DisplayName = Utility.GetProperty(de, "displayName");
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
                ADUser.IsAccountActive = Utility.IsAccountActive(Convert.ToInt32(Utility.GetProperty(de, "userAccountControl")));
            }
            catch { }
            return ADUser;
        }

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

        #endregion
    }
}