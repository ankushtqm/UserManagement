using System;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using System.Linq;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;
using System.ComponentModel;
using ATA.Authentication;
using ATA.Authentication.Providers; 
using ATA.ObjectModel;
using System.Web;
using Microsoft.SharePoint; 


namespace A4A.SharePoint16.Webparts.TestSiteMapWebpart
{
    [ToolboxItemAttribute(false)]
    public partial class TestSiteMapWebpart : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public TestSiteMapWebpart()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    BindData();
                }
            }
            catch (Exception ex)
            {

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
            return memberUser.UserId;
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

        protected DataTable GetAllGroupSites()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[ConfigurationManager.AppSettings["DataObjectConnectionStringName"]].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("p_Group_GetLiaisons", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure; 
                    try
                    {
                        cmd.Connection.Open();
                        using (var da = new SqlDataAdapter(cmd))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            da.Fill(dt); 
                        }
                    }
                    catch (Exception err)
                    {
                        lblMessage.Text += "<br />Error: " + err.Message;
                    }
                    finally
                    {
                        cmd.Connection.Close();
                        cmd.Connection.Dispose();
                    }
                }
            }
            return dt;
        }

        private string RenderStyles()
        {
            string href = "https://ajax.aspnetcdn.com/ajax/jquery.dataTables/1.9.4/css/jquery.dataTables.css";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"<link rel=""stylesheet"" href=""" + href + @""" type=""text/css"" />");
            return sb.ToString();
        }

        private void BindData()
        {
            try
            {
                DataTable resultDataTable = GetAllGroupSites();
                DataTable dt = new DataTable();

                resultDataTable.Columns.Add("Liaison1", typeof(string));
                resultDataTable.Columns.Add("Liaison2", typeof(string));


                foreach (DataRow dr in resultDataTable.Rows)
                {
                    if (dr["Liaison1FName"].ToString().Trim().Length > 0 && dr["Liaison1LName"].ToString().Trim().Length > 0)
                    {
                        dr["Liaison1"] = dr["Liaison1FName"] + " " + dr["Liaison1LName"];// + "(" + dr["Liaison1Company"] + ")";
                    }
                    if (dr["Liaison2FName"].ToString().Trim().Length > 0 && dr["Liaison2LName"].ToString().Trim().Length > 0)
                    {
                        dr["Liaison2"] = dr["Liaison2FName"] + " " + dr["Liaison2LName"];// + "(" + dr["Liaison2Company"] + ")";
                    }
                }

                if (resultDataTable.Rows.Count > 0)
                {
                    rep1.DataSource = resultDataTable;
                    rep1.DataBind();
                    lblMessage.Text = string.Empty;
                    rep1.Visible = true;

                }
                else
                {
                    lblMessage.Text = "No Contacts were found for the specified filters. <br/>";
                    rep1.Visible = false;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text += "<br /><br/>Error in Contacts: " + ex.Message + "<br />";
            }
        }

    }
}

