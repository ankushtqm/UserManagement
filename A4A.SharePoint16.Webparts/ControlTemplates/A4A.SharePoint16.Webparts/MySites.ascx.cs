using System;
using System.Web.UI; 
using System.Data;
using System.Configuration;
using System.Data.SqlClient; 
using ATA.Authentication;
using ATA.Authentication.Providers; 
using ATA.ObjectModel;
using System.Web;
using Microsoft.SharePoint;
using System.Web.UI.WebControls;

namespace A4A.SharePoint16.Webparts
{
    public partial class MySites : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    #region hide
                    //string un = SPContext.Current.Web.CurrentUser.LoginName.ToString();

                    //lblMessage.Text += "<h5> GetCurrentEditorUserName : </h5>" + GetCurrentEditorUserName();

                    //int UserId = GetCurrentEditorUserId();

                    //lblMessage.Text += "<br/><h5> GetCurrentEditorUserId : </h5>" + UserId;

                    //lblMessage.Text += "<br/><h5>FirstName: </h5>" + GetCurrentEditorUser().FirstName + "<br/><h5> LastName: : </h5>" + GetCurrentEditorUser().LastName;

                    //int CompanyId = DbUtil.GetUserCompanyIdByUserId(UserId);

                    //Company company = new Company(CompanyId);

                    //lblMessage.Text += "<br/><h5>Company Name: </h5>" + company.Name;

                    //lblMessage.Text += "<br/><h5>Fuels Company Type Id: </h5>" + company.FuelsCompanyTypeId;
                    #endregion
                  
                    GetUserMySites(); 
                });
            }
            catch (Exception ex)
            {
                lblMessage.Text += "2. Error:" + ex.Message +"<br/>"+ ex.InnerException + "<br/>" + ex.StackTrace ;
            }
        }

        private Company GetCompanyNameById(int compnayId)
        {
            Company company = new Company(compnayId);
            return company;
        }

        public static string GetCurrentEditorUserName()
        {
            string userName = SPContext.Current.Web.CurrentUser.LoginName;
            if (userName.ToLower() == "sharepoint\\system")//if the current user used for the app pool then SPContext.Current.Web.CurrentUser.LoginName is sharepoint\\system
                userName = HttpContext.Current.User.Identity.Name;

            if (SPContext.Current.Web.CurrentUser.LoginName.Contains("atamembershipprovider"))
            {
                userName = userName.Substring(29, userName.Length - 29);
            }
           
            return userName;
        }

        public static int GetCurrentEditorUserId()
        {
            ATAMembershipUser memberUser = GetCurrentEditorUser(); 
            return memberUser == null ? 0 : memberUser.UserId;
        }

        public static ATAMembershipUser GetCurrentEditorUser()
        {
            string userName = GetCurrentEditorUserName();
            return GetMembershipUserByUserName(userName);
        }

        #region private methods
        private static ATAMembershipUser GetMembershipUserByUserName(string userName)
        {
            return GetMembershipUserByUserName(userName, ATAMembershipUtility.Instance.GetATAMembershipProvider());
        }

        private static ATAMembershipUser GetMembershipUserByUserName(string userName, ATAMembershipProvider provider)
        {
            string parsedUserName = ATAMembershipUtility.Instance.ParseUsername(userName);
            ATAMembershipUser membershipUser = (ATAMembershipUser)provider.GetUser(parsedUserName, false);
            if (membershipUser == null)
                throw new Exception(string.Format("Could not load user record for: {0}", userName));
            return membershipUser;
        }
        #endregion

        protected DataTable GetUserMySites()
        {
            int userid = GetCurrentEditorUserId();
            DataTable dt = new DataTable();
            if (userid > 0)
            { 
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[ConfigurationManager.AppSettings["DataObjectConnectionStringName"]].ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("p_User_GetMySites", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserId", GetCurrentEditorUserId());
                        cmd.Parameters.AddWithValue("@AppliesToSiteId", AppliesToSite.Members);

                        try
                        {
                            cmd.Connection.Open();
                            using (var da = new SqlDataAdapter(cmd))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                da.Fill(dt);

                                ddlMySites.DataSource = dt;
                                ddlMySites.DataTextField = "GroupName";
                                ddlMySites.DataValueField = "GroupSiteUrl";
                                ddlMySites.DataBind();

                                ddlMySites.Items.Insert(0, new ListItem("My Sites", "no_page"));
                            }
                        }
                        catch (Exception err)
                        {
                            lblMessage.Text += "1. <br />Error: " + err.Message;
                        }
                        finally
                        {
                            cmd.Connection.Close();
                            cmd.Connection.Dispose();
                        }
                    }
                }
            }
            return dt;
        }
    }
}

