#region namespaces

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.Configuration;
using System.Web.Security;
using ATA.Authentication.Providers;
using SusQTech.Data.DataObjects;
using Microsoft.ApplicationBlocks.Data;
using System.DirectoryServices.AccountManagement;
using ATA.ObjectModel;
using SusQTech.Utility;

#endregion

namespace ATA.Authentication
{
    public class ATAMembershipUtility
    {
        #region private members

        private static ATAMembershipUtility _instance = null;
        private ATAMembershipProvider _ataMembershipProvider = null;
        private ATARoleProvider _ataRoleProvider = null;
        private string _overrideCurrentSite = null;

        #endregion

        private ATAMembershipUtility() { }

        #region public properties

        public static ATAMembershipUtility Instance
        {
            get
            {
                if (ATAMembershipUtility._instance == null)
                    ATAMembershipUtility._instance = new ATAMembershipUtility();
                return (ATAMembershipUtility._instance);
            }
        }

        public AppliesToSite CurrentSite
        {
            get
            {
                string parsingValue = string.Empty;

                if (!string.IsNullOrEmpty(this._overrideCurrentSite))
                    parsingValue = this._overrideCurrentSite;
                else
                    parsingValue = ConfigurationManager.AppSettings["UserManagerCurrentSite"];

                if (string.IsNullOrEmpty(parsingValue))
                    throw new ConfigurationErrorsException("No value found for UserManagerCurrentSite key.");

                AppliesToSite value = AppliesToSite.Fuels;
                try
                {
                    value = (AppliesToSite)Enum.Parse(typeof(AppliesToSite), parsingValue);
                }
                catch (Exception err)
                {
                    throw new ConfigurationErrorsException(string.Format("Error parsing found value for UserManagerCurrentSite key.  Value: {0}", parsingValue), err);
                }
                return (value);
            }
        }


        #endregion

        #region public methods

        public void SetOverrideAppliesToSite(AppliesToSite value)
        {
            this._overrideCurrentSite = value.ToString();
        }

        public string ParseUsername(string ComplexUserName)
        {
            Utility.CheckParameter(ref ComplexUserName, true, true, true, 1024, "ComplexUserName");

            string username = ComplexUserName;
            if (username.Contains(@"\"))
            {
                string[] usernameparts = username.Split(new string[] { @"\" }, StringSplitOptions.RemoveEmptyEntries);
                if (usernameparts.Length == 2)
                    username = usernameparts[1];
            }
            else if (username.IndexOf(":") > 0)
            {
                username = username.Substring(username.IndexOf(":") + 1);
            }
            return (username);
        }

        public bool IsCurrentUserAdmin(string email)
        {
            bool role = false;
            // set up domain context
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain, "ata.org");
            //string editorUserName = ATAMembershipUtility.Instance.ParseUsername(email);
            // find a user
            UserPrincipal user = UserPrincipal.FindByIdentity(ctx, email);

            // find the group in question
            GroupPrincipal group = GroupPrincipal.FindByIdentity(ctx, "ITSecurity"); 
            if (user != null)
            {
                // check if user is member of that group
                if (user.IsMemberOf(group))
                {
                    role = true;
                }
            }
            return role;
        }

        public ATAMembershipUser GetCurrentUser(string email)
        {
            //// set up domain context 
            //string editorUserName = ATAMembershipUtility.Instance.ParseUsername(email);
            // find a user
            ATAMembershipProvider provider = ATAMembershipUtility.Instance.GetATAMembershipProvider();
            ATAMembershipUser user = (ATAMembershipUser)provider.GetUser(email, true);
            return user;
        }

        #endregion

        #region SendEmail 
        public bool SendEmail(string body, string to, string from, string subject, bool bodyIsHtml, bool throwException)
        {
            return this.SendEmail(body, to, from, subject, bodyIsHtml, throwException, null);
        }

        public bool SendEmail(string body, string to, string from, string subject, bool bodyIsHtml, bool throwException, List<System.Net.Mail.Attachment> attachments)
        {
            return (this.SendEmail(body, new string[] { to }, new string[] { }, new string[] { }, from, subject, bodyIsHtml, throwException, attachments));
        }

        public bool SendEmail(string body, string[] to, string[] cc, string[] bcc, string from, string subject, bool bodyIsHtml, bool throwException, List<System.Net.Mail.Attachment> attachments)
        {
            bool rValue = true;

            try
            {
                Utility.CheckParameter(ref from, true, true, false, 1000, "from");
                Utility.CheckParameter(ref subject, true, false, false, 1000, "subject");
                Utility.CheckParameter(ref body, true, false, false, 100000, "body");
            }
            catch (Exception err)
            {
                rValue = false;
                if (throwException)
                    throw err;
            }

            if (rValue)
            {
                using (System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage())
                {
                    message.From = new System.Net.Mail.MailAddress(from);
                    message.Body = body;
                    message.IsBodyHtml = bodyIsHtml;
                    message.Subject = subject;

                    foreach (string toAddress in to)
                    {
                        message.To.Add(toAddress);
                    }

                    foreach (string ccAddress in cc)
                    {
                        message.CC.Add(ccAddress);
                    }

                    foreach (string bccAddress in bcc)
                    {
                        message.Bcc.Add(bccAddress);
                    }

                    if ((attachments != null) && (attachments.Count > 0))
                    {
                        attachments.ForEach(delegate (System.Net.Mail.Attachment a)
                        {
                            message.Attachments.Add(a);
                        });
                    }

                    System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
                    client.Host = ConfigurationManager.AppSettings["ATASMTPServer"];

                    int port = -1;
                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ATASMTPServerPort"])
                        && int.TryParse(ConfigurationManager.AppSettings["ATASMTPServerPort"], out port))
                        client.Port = port;

                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ATADeliveryMethod"]))
                        client.DeliveryMethod = (System.Net.Mail.SmtpDeliveryMethod)Enum.Parse(typeof(System.Net.Mail.SmtpDeliveryMethod), ConfigurationManager.AppSettings["ATADeliveryMethod"]);

                    try
                    {
                        client.Send(message);
                    }
                    catch (Exception err)
                    {
                        rValue = false;
                        if (throwException)
                        {
                            StringBuilder sb = new StringBuilder();

                            while (err != null)
                            {
                                sb.AppendLine(string.Format("Message: {0}", err.Message));
                                sb.AppendLine(string.Format("Source: {0}", err.Source));
                                sb.AppendLine(string.Format("Data: {0}", err.Data));
                                sb.AppendLine(string.Format("StackTrace: {0}", err.StackTrace));
                                sb.AppendLine(string.Format("Target Site: {0}", err.TargetSite));
                                if (err.InnerException != null)
                                {
                                    sb.AppendLine("Inner Exception:");
                                }
                                err = err.InnerException;
                            }

                            throw new Exception(sb.ToString());
                        }
                    }
                }
            }
            return (rValue);
        }

        #endregion

        #region GetATAMembershipProvider

        public ATAMembershipProvider GetATAMembershipProvider()
        {
            if (this._ataMembershipProvider != null)
                return (this._ataMembershipProvider);

            if (Membership.Provider is ATAMembershipProvider)
            {
                this._ataMembershipProvider = Membership.Provider as ATAMembershipProvider;
            }
            else
            {
                foreach (MembershipProvider testProvider in Membership.Providers)
                {
                    if (testProvider is ATAMembershipProvider)
                    {
                        this._ataMembershipProvider = testProvider as ATAMembershipProvider;
                        break;
                    }
                }
            }

            if (this._ataMembershipProvider == null)
                throw new Exception("Could not locate a membership provider of type ATAMembershipProvider.");

            return (this._ataMembershipProvider);
        }

        #endregion

        #region GetATARoleProvider

        public ATARoleProvider GetATARoleProvider()
        {
            if (this._ataRoleProvider != null)
                return (this._ataRoleProvider);

            if (Roles.Provider is ATARoleProvider)
            {
                this._ataRoleProvider = Roles.Provider as ATARoleProvider;
            }
            else
            {
                foreach (RoleProvider testProvider in Roles.Providers)
                {
                    if (testProvider is ATARoleProvider)
                    {
                        this._ataRoleProvider = testProvider as ATARoleProvider;
                        break;
                    }
                }
            }

            if (this._ataRoleProvider == null)
                throw new Exception("Could not locate a role provider of type ATARoleProvider.");

            return (this._ataRoleProvider);
        }

        #endregion

        #region GetAll methods for filling drop downs.

        public Dictionary<int, string> GetAllMemberTypes()
        {
            SqlParameter[] sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter("@AppliesToSiteId", (int)this.CurrentSite);

            Dictionary<int, string> memberTypes = new Dictionary<int, string>();
            using (SqlDataReader dr = SqlHelper.ExecuteReader(ConnectionFactory.DataObjectConnectionString, CommandType.StoredProcedure, "p_Membership_GetAllMemberTypeNames", sqlParams))
            {
                while (dr.Read())
                {
                    memberTypes.Add(int.Parse(dr["MemberTypeId"].ToString()), dr["MemberTypeName"].ToString());
                }
                dr.Close();
            }
            return (memberTypes);
        }

        public Dictionary<int, string> GetAllGroups()
        {
            SqlParameter[] sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter("@AppliesToSiteId", (int)this.CurrentSite);

            Dictionary<int, string> groups = new Dictionary<int, string>();
            using (SqlDataReader dr = SqlHelper.ExecuteReader(ConnectionFactory.DataObjectConnectionString, CommandType.StoredProcedure, "p_Membership_GetAllGroupNames", sqlParams))
            {
                while (dr.Read())
                {
                    groups.Add(int.Parse(dr["GroupId"].ToString()), dr["GroupName"].ToString());
                }
                dr.Close();
            }
            return (groups);
        }

        public Dictionary<int, string> GetAllAddressTypes()
        {
            Dictionary<int, string> addressTypes = new Dictionary<int, string>();
            using (SqlDataReader dr = SqlHelper.ExecuteReader(ConnectionFactory.DataObjectConnectionString, CommandType.StoredProcedure, "p_Membership_GetAllAddressTypes"))
            {
                while (dr.Read())
                {
                    addressTypes.Add(int.Parse(dr["AddressTypeId"].ToString()), dr["AddressTypeDescription"].ToString());
                }
                dr.Close();
            }
            return (addressTypes);
        }

        public Dictionary<int, string> GetAllCompanies()
        {
            Dictionary<int, string> companies = new Dictionary<int, string>();
            using (SqlDataReader dr = SqlHelper.ExecuteReader(ConnectionFactory.DataObjectConnectionString, CommandType.StoredProcedure, "p_Membership_GetAllCompanies"))
            {
                while (dr.Read())
                {
                    companies.Add(int.Parse(dr["CompanyId"].ToString()), dr["CompanyName"].ToString());
                }
                dr.Close();
            }
            return (companies);
        }

        public Dictionary<int, string> GetAllCommitteeRoles()
        {
            Dictionary<int, string> roles = new Dictionary<int, string>();
            using (SqlDataReader dr = SqlHelper.ExecuteReader(ConnectionFactory.DataObjectConnectionString, CommandType.StoredProcedure, "p_CommitteeRoles_GetAll"))
            {
                while (dr.Read())
                {
                    roles.Add(int.Parse(dr["CommitteeRoleId"].ToString()), dr["CommitteeRoleName"].ToString());
                }
                dr.Close();
            }
            return (roles);
        }

        public Dictionary<int, string> GetAllCommittees()
        {
            SqlParameter[] sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter("@AppliesToSiteId", (int)this.CurrentSite);

            Dictionary<int, string> groups = new Dictionary<int, string>();
            using (SqlDataReader dr = SqlHelper.ExecuteReader(ConnectionFactory.DataObjectConnectionString, CommandType.StoredProcedure, "p_Group_GetAllCommittees", sqlParams))
            {
                while (dr.Read())
                {
                    groups.Add(int.Parse(dr["GroupId"].ToString()), dr["GroupName"].ToString());
                }
                dr.Close();
            }
            return (groups);
        }

        public Dictionary<string, string> GetAllCountries()
        {
            Dictionary<string, string> countries = new Dictionary<string, string>();
            using (SqlDataReader dr = SqlHelper.ExecuteReader(ConnectionFactory.DataObjectConnectionString, CommandType.StoredProcedure, "p_Country_GetAll"))
            {
                while (dr.Read())
                {
                    countries.Add(dr["CountryId"].ToString(), dr["CountryName"].ToString());
                }
                dr.Close();
            }
            return (countries);
        }

        #endregion

        #region GetGroupUsers

        public DataTable GetGroupUsers(int groupId)
        {
            SqlParameter[] sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter("@GroupId", groupId);

            DataSet ds = SqlHelper.ExecuteDataset(ConnectionFactory.DataObjectConnectionString, CommandType.StoredProcedure, "p_Group_GetMembers", sqlParams);

            if (ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        #endregion


        #region SetATARoleProviderFromRoleManagerConfigSection

        public bool SetATARoleProviderFromRoleManagerConfigSection(RoleManagerSection section)
        {
            bool rValue = false;

            RoleProviderCollection rpc = new RoleProviderCollection();

            ProvidersHelper.InstantiateProviders(section.Providers, rpc, typeof(RoleProvider));

            foreach (RoleProvider testProvider in rpc)
            {
                if (testProvider is ATARoleProvider)
                {
                    this._ataRoleProvider = testProvider as ATARoleProvider;
                    rValue = true;
                    break;
                }
            }

            return (rValue);
        }

        #endregion

        #region SetATAMembershipProviderFromMembershipSection

        public bool SetATAMembershipProviderFromMembershipSection(MembershipSection section)
        {
            bool rValue = false;

            MembershipProviderCollection mpc = new MembershipProviderCollection();

            ProvidersHelper.InstantiateProviders(section.Providers, mpc, typeof(MembershipProvider));

            foreach (MembershipProvider testProvider in mpc)
            {
                if (testProvider is ATAMembershipProvider)
                {
                    this._ataMembershipProvider = testProvider as ATAMembershipProvider;
                    rValue = true;
                    break;
                }
            }

            return (rValue);
        }

        #endregion
    }
}