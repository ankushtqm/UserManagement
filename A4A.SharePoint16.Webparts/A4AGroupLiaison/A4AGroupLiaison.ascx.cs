using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Linq;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint;

namespace A4A.SharePoint16.Webparts.A4AGroupLiaison
{
    [ToolboxItemAttribute(false)]
    public partial class A4AGroupLiaison : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public A4AGroupLiaison()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
        }


        private const string GetLiaison = "p_Group_GetLiaisons";
        private const string GetGrp = "p_Group_Load";

        [Personalizable(PersonalizationScope.Shared), WebBrowsable(true), WebDisplayNameAttribute("GroupID"), WebDescription("Enter the GroupID:"),
        CategoryAttribute("A4A WebPart Settings")]
        public int GroupID
        {
            get;
            set;
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

        protected string GetConnectionString()
        {

            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["DataObjectConnectionStringName"]))
                throw new Exception("No connection string name specified in AppSettings:DataObjectConnectionStringName");

            if (ConfigurationManager.ConnectionStrings[ConfigurationManager.AppSettings["DataObjectConnectionStringName"]] == null)
                throw new Exception(string.Format("No connection string found with name {0}", ConfigurationManager.AppSettings["DataObjectConnectionStringName"]));

            return ConfigurationManager.ConnectionStrings[ConfigurationManager.AppSettings["DataObjectConnectionStringName"]].ToString();
        }


        protected DataTable GetGroupDetails()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[ConfigurationManager.AppSettings["DataObjectConnectionStringName"]].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(GetLiaison, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@GroupId", GroupID);

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
                        lblMessage1.InnerHtml += "<br />Error: " + err.Message;
                        lblMessage2.InnerHtml +=  " ";
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


        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            base.Render(writer);
            writer.Write(RenderStyles());
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
                DataTable resultDataTable = GetGroupDetails();
                if (resultDataTable.Rows.Count > 0)
                {
                    DataRow dr = resultDataTable.Rows[0];
                    //lblMessage1.InnerHtml += "<b>Group Liaison</b><br/>"; //Hidden for John
                    //lblGroupName.Text = dr["GroupName"].ToString();
                    string email1 = dr["Liaison1Email"].ToString();
                    if (dr["Liaison1CompanyId"].ToString().Trim().Length < 1)
                        email1 = dr["Liaison1Email"].ToString() + "@airlines.org";
                    lblMessage1.InnerHtml += dr["Liaison1FName"] + " " + dr["Liaison1LName"] + "</br>" + dr["Liaison1Title"] + " </br>" + email1 + " </br>" + dr["Liaison1Company"];
                  }
                else
                {
                    lblMessage1.InnerHtml += "No Liaison Assigned";
                }
            }
            catch (Exception ex)
            {
                lblMessage1.InnerHtml = "<br /><br/>Error displaying the 'Group Liaisons'.  " + ex.Message + "<br />";
                lblMessage1.InnerHtml = "";
            }
        }
    }
}
