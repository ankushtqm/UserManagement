#region namespaces

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.Profile;
using Microsoft.ApplicationBlocks.Data;
using ATA.Member.Util;
using System.Configuration;
#endregion

namespace ATA.Authentication.Providers
{
    public class ATARoleProvider : RoleProvider
    {
        #region Private Members

        /// <summary></summary>
        private System.Collections.Specialized.NameValueCollection _config;

        #endregion

        #region Initialize

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            _config = config;
            base.Initialize(name, config);
        }

        #endregion

        #region FindUsersInRole

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            List<string> users = new List<string>();
            SqlParameter[] sqlParams = new SqlParameter[3];
            sqlParams[0] = new SqlParameter("@RoleName", roleName);
            sqlParams[1] = new SqlParameter("@UsernameToMatch", usernameToMatch.Trim());
            sqlParams[2] = new SqlParameter("@AppliesToSiteId", (int)ATAMembershipUtility.Instance.CurrentSite);
            using (SqlDataReader dr = SqlHelper.ExecuteReader(this.connectionString, CommandType.StoredProcedure, "p_Roles_FindUsersInRole", sqlParams))
            {
                while (dr.Read())
                {
                    users.Add(dr["Username"].ToString());
                }
                dr.Close();
            } 
            return (users.ToArray());

        }

        #endregion

        #region GetAllRoles

        public override string[] GetAllRoles()
        {
            SqlParameter[] sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter("@AppliesToSiteId", (int)ATAMembershipUtility.Instance.CurrentSite);
            List<string> roles = new List<string>();
            using (SqlDataReader dr = SqlHelper.ExecuteReader(this.connectionString, CommandType.StoredProcedure, "p_Roles_GetAllRoles", sqlParams))
            {
                while (dr.Read())
                {
                    roles.Add(dr["RoleName"].ToString());
                }
                dr.Close();
            }
            return (roles.ToArray());
        }

        #endregion

        #region GetRolesForUser

        public override string[] GetRolesForUser(string username)
        {
            try
            {
                List<string> roles = new List<string>();
                SqlParameter[] sqlParams = new SqlParameter[2];
                sqlParams[0] = new SqlParameter("@Username", username);
                sqlParams[1] = new SqlParameter("@AppliesToSiteId", (int)ATAMembershipUtility.Instance.CurrentSite);
                using (SqlDataReader dr = SqlHelper.ExecuteReader(this.connectionString, CommandType.StoredProcedure, "p_Roles_GetRolesForUser", sqlParams))
                {
                    while (dr.Read())
                    {
                        roles.Add(dr["RoleName"].ToString());
                    }
                    dr.Close();
                }
                if (Conf.LogRoleProviderVerbose)
                {
                    Log.LogInfo(string.Format("{0} has roles : {1}", username, string.Join(", ", roles.ToArray())));
                }
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["CommerceFaxNumber"]))
                    Log.LogInfo("GetRolesForUser returns following for user " + username + ": " + string.Join(", ", roles.ToArray()));
                return (roles.ToArray());
            }
            catch (Exception e)
            {
                Log.LogError("Error in GetRolesForUser: " + username + " because " + e.ToString());
                throw;
            }
        }

        #endregion

        #region GetUsersInRole

        public override string[] GetUsersInRole(string roleName)
        {
            List<string> users = new List<string>();
            SqlParameter[] sqlParams = new SqlParameter[2];
            sqlParams[0] = new SqlParameter("@RoleName", roleName);
            sqlParams[1] = new SqlParameter("@AppliesToSiteId", (int)ATAMembershipUtility.Instance.CurrentSite);
            using (SqlDataReader dr = SqlHelper.ExecuteReader(this.connectionString, CommandType.StoredProcedure, "p_Roles_GetUsersInRole", sqlParams))
            {
                while (dr.Read())
                {
                    users.Add(dr["Username"].ToString());
                }
                dr.Close();
            }
            if (Conf.LogRoleProviderVerbose)
            {
                Log.LogInfo(string.Format("{0} has users : {1}", roleName, string.Join(", ", users.ToArray())));
            }
            return (users.ToArray());
        }

        #endregion

        #region IsUserInRole

        public override bool IsUserInRole(string username, string roleName)
        {
            SqlParameter[] sqlParams = new SqlParameter[4];
            sqlParams[0] = new SqlParameter("@Username", username);
            sqlParams[1] = new SqlParameter("@RoleName", roleName);
            sqlParams[2] = new SqlParameter("@AppliesToSiteId", (int)ATAMembershipUtility.Instance.CurrentSite);
            sqlParams[3] = new SqlParameter("@IsUserInRole", SqlDbType.Bit);
            sqlParams[3].Direction = ParameterDirection.Output;

            bool result = (bool)SqlHelper.ExecuteScalar(this.connectionString, CommandType.StoredProcedure, "p_Roles_IsUserInRole", sqlParams);

            if (Conf.LogRoleProviderVerbose)
            {
                Log.LogInfo(string.Format("{0} in role {1}: {2}", username, roleName, (result ? "Yes" : "No")));
            }
            return (result);
        }

        #endregion

        #region RoleExists

        public override bool RoleExists(string roleName)
        {
            SqlParameter[] sqlParams = new SqlParameter[3];
            sqlParams[0] = new SqlParameter("@RoleName", roleName);
            sqlParams[1] = new SqlParameter("@AppliesToSiteId", (int)ATAMembershipUtility.Instance.CurrentSite);
            sqlParams[2] = new SqlParameter("@RoleExists", SqlDbType.Bit);
            sqlParams[2].Direction = ParameterDirection.Output;

            bool result = (bool)SqlHelper.ExecuteScalar(this.connectionString, CommandType.StoredProcedure, "p_Roles_RoleExists", sqlParams);

            return (result);
        }

        #endregion

        #region properties

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new Exception("The method or operation is not implemented.");
        }

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
        private string connectionString
        {
            get
            {
                if (System.Configuration.ConfigurationManager.ConnectionStrings[this.ConnectionStringName] != null)
                    return System.Configuration.ConfigurationManager.ConnectionStrings[this.ConnectionStringName].ConnectionString;
                else
                    throw new Exception("Cannot find connection string for " +
                        this.ConnectionStringName);
            }
        }

        #endregion

        #region methods not implemented

        public override void CreateRole(string roleName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

    }
}
