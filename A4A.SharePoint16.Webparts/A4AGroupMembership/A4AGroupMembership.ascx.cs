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


namespace A4A.SharePoint16.Webparts.A4AGroupMembership
{
    [ToolboxItemAttribute(false)]
    public partial class A4AGroupMembership : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public A4AGroupMembership()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
        }

        private const string GetMembers = "p_Group_GetMembers";
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

        protected DataTable GetPageList()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[ConfigurationManager.AppSettings["DataObjectConnectionStringName"]].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(GetMembers, con))
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

        protected DataTable GetGroupDetails()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[ConfigurationManager.AppSettings["DataObjectConnectionStringName"]].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(GetGrp, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@GroupId", 264);

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


        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            base.Render(writer);
            writer.Write(RenderStyles());
        }

        private string RenderStyles()
        {
            string href = "http://ajax.aspnetcdn.com/ajax/jquery.dataTables/1.9.4/css/jquery.dataTables.css";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"<link rel=""stylesheet"" href=""" + href + @""" type=""text/css"" />");
            return sb.ToString();
        }

        private void BindData()
        {
            try
            {
                DataTable resultDataTable = GetPageList();
                DataTable dt = new DataTable();

                resultDataTable.Columns.Add("Mobile", typeof(string));
                resultDataTable.Columns.Add("Office", typeof(string));


                foreach (DataRow dr in resultDataTable.Rows)
                {
                    if (dr["MobilePhone"] != null)
                    {
                        Int64 mnumber;
                        string s = Regex.Replace(dr["MobilePhone"].ToString(), "[^0-9.]", "");
                        Int64.TryParse(s, out mnumber);
                        if (mnumber > 0)
                        {
                            dr["Mobile"] = String.Format("{0:(###) ###-####}", mnumber);
                        }
                    }
                    if (dr["OfficePhone"] != null)
                    {
                        Int64 mnumber;
                        string s = Regex.Replace(dr["OfficePhone"].ToString(), "[^0-9.]", "");
                        Int64.TryParse(s, out mnumber);
                        if (mnumber > 0)
                        {
                            dr["Office"] = String.Format("{0:(###) ###-####}", mnumber);
                        }
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
