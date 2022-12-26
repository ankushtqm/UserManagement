using System;
using System.ComponentModel;
using System.Web.UI.WebControls.WebParts;
using System.Data.SqlClient;
using System.Data;
using System.Configuration; 
using SusQTech.Data.DataObjects;

namespace A4A.SharePoint16.Webparts.A4AGroupMission
{
    [ToolboxItemAttribute(false)]
    public partial class A4AGroupMission : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public A4AGroupMission()
        {
        } 

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
        }

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

        protected DataTable GetGroupDetails()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[ConfigurationManager.AppSettings["DataObjectConnectionStringName"]].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("select mission from [Group] where groupid =  "+ GroupID, con))
                {
                    cmd.CommandType = CommandType.Text;
                    //cmd.Parameters.AddWithValue("@GroupId", GroupID);

                    try
                    {
                        cmd.Connection.Open();
                        using (var da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                    catch (Exception err)
                    {
                        lblError.InnerHtml += "<br />Error: " + err.Message;
                        lblError.InnerHtml += " ";
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

        public void BindData()
        {
            DataTable dt = GetGroupDetails();
            if (dt.Rows[0][0] != null && dt.Rows[0][0].ToString().Length > 0)
                divMission.InnerHtml = dt.Rows[0][0].ToString();
            else
                divMission.InnerHtml = "No Mission stated";

        }
    }
}
