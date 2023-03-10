using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Http;
using System.Web;
using ATA.CodeLibrary;
using ATA.ObjectModel;
using ATA.Member.Util; 
using System.Data;
using System.Data.SqlClient; 
using ATA.Authentication;
using ATA.Authentication.Providers;
using System.DirectoryServices.AccountManagement;

namespace A4A.UM.Controllers
{
    public class DBUtilAPIController : ApiController
    { 
        public static ATAMembershipUser _currentUser;
        public static ATAMembershipUser CurrentUser()
        { 
            //Commenting this because the value is current user is persisting among multiple sessions.
              //if(_currentUser == null)
              //  {
                    ATAMembershipProvider provider = ATAMembershipUtility.Instance.GetATAMembershipProvider();
                    string editorUserName = ATAMembershipUtility.Instance.ParseUsername(HttpContext.Current.User.Identity.Name);
                      _currentUser = (ATAMembershipUser)provider.GetUser(editorUserName, true);
                //}
                return _currentUser;
         }

        // GET api/<controller>
        [Route("api/GetA4AStaff")]
        public IEnumerable<LookupUser> GetA4AStaff()
        {
            return DbUtil.GetAllAtaStaff();
        } 
        [Route("api/GetA4AStaffwTitle")]
        public  Dictionary<int, string> GetA4AStaffwTitle()
        {
            return DbUtil.GetA4AStaffNameTitleToId();
        }

        //[Route("api/GetAllContactsNameTitleToId")]
        //public Dictionary<int, string> GetAllContactsNameTitleToId()
        //{
        //    return DbUtil.GetAllContactsNameTitleToId(string.Empty,0);
        //}        
        [Route("api/GetAllContacts")]
        public SortedDictionary<string, int> GetAllContacts(string term,int grouptype = 0)
        {
          return DbUtil.GetAllContactsNameTitleToId(term, grouptype); //.Where(x => x.Value.Contains(term)).ToDictionary(x => x.Key, x => x.Value);
        } 
        [Route("api/GetAllGroups")]
        public Dictionary<int, string> GetAllGroups(string term, bool isMember = false, bool isActiveMB = false,int? CompanyId = null)
        {
            //Commenting on 7/16/2018 Because of UM comments.docs #24 
            //string editorUserName = ATAMembershipUtility.Instance.ParseUsername(this.User.Identity.Name);
            //bool isAdmin = DbUtil.GetIsCurrentUserAdmin(editorUserName, "ITSecurity"); 
            return DbUtil.GetAllGroupNameId(term, CompanyId, isActiveMB, false, DBUtilAPIController.CurrentUser().UserId); //.Where(x => x.Value.Contains(term)).ToDictionary(x => x.Key, x => x.Value);
        }
        [Route("api/GetCommitteRolesbyGType")]
        public Dictionary<int,string> GetCommitteRolesbyGType(int GroupType)
        {
            return DbUtil.GetRolesbyGroupType(GroupType); //.Where(x => x.Value.Contains(term)).ToDictionary(x => x.Key, x => x.Value);
        }
        [Route("api/GetRolesbyGroupID")]
        public SortedDictionary<string, int> GetRolesbyGroupID(int GroupId)
        {
            return DbUtil.GetRolesbyGroupID(GroupId); //.Where(x => x.Value.Contains(term)).ToDictionary(x => x.Key, x => x.Value);
        }
        
        [Route("api/LyrisSend")]
        public SortedDictionary<string, int> GetLyrisSend()
        {
            return DbUtil.GetLyrisSend();
        }

        [Route("api/GetComapnyUsers")] //CompanyName/CompanyId
        public SortedDictionary<string, int> GetComapnyUsers(string CompanyName, string term)
        {
            return ATAGroup.GetAllUsersForCommitteeGroup(CompanyName, term);
        }

        [Route("api/GetCompanies")] //CompanyName/CompanyId
        public SortedDictionary<string, int> GetComapnies(bool isMember)
        {
            return Company.GetMemberOrNonMemberCompanies(isMember);
        }
        [Route("api/GetAllCompanies")]
        public SortedDictionary<string, int> GetAllCompanies(string term)
        {
            return  Company.GetCompanyList(term);
        }
         
        [Route("api/SendEmail")]
        public bool SendEmail(string body = null,string to = null,string from = null,string subject = null,bool bodyIsHtml = false )
        {
            ATAMembershipUtility.Instance.SendEmail(body,to,from,subject,bodyIsHtml,true); 
            return false;
        }
        [Route("api/GetCurrentUser")]
        public ATAMembershipUser GetCurrentUser()
        {
            //set up domain context 
            string editorUserName = ATAMembershipUtility.Instance.ParseUsername(this.User.Identity.Name);
            //find a user
            ATAMembershipProvider provider = ATAMembershipUtility.Instance.GetATAMembershipProvider();
            ATAMembershipUser user = (ATAMembershipUser)provider.GetUser(editorUserName, true);
            return user;
        }
        [Route("api/GetDept")] //for taskforce and other view loads
        public string GetDepartmentList()
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
            {
                StringBuilder sql = new StringBuilder();
                using (SqlCommand cmd = new SqlCommand("select DepartmentId,DepartmentDetail from Department order by DepartmentDetail", con))
                {
                   // con.Open(); 
                    cmd.CommandType = CommandType.Text;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt); 
                    Dictionary<string, object> row;
                    foreach (DataRow dr in dt.Rows)
                    {
                        row = new Dictionary<string, object>();
                        foreach (DataColumn col in dt.Columns)
                        {
                            row.Add(col.ColumnName, dr[col]);
                        }
                        rows.Add(row);
                    } 
                }
            }

            return serializer.Serialize(rows);
        }

        [Route("api/GetComp")] //for taskforce and other view loads
        public string GetCompanyList()
        {
            DataTable dt = new DataTable();
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
            {
                StringBuilder sql = new StringBuilder();
                using (SqlCommand cmd = new SqlCommand("select CompanyId ,CompanyName from Company order by CompanyName", con))
                {
                    //con.Open();
                    cmd.CommandType = CommandType.Text;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt); 
                    Dictionary<string, object> row;
                    foreach (DataRow dr in dt.Rows)
                    {
                        row = new Dictionary<string, object>();
                        foreach (DataColumn col in dt.Columns)
                        {
                            row.Add(col.ColumnName, dr[col]);
                        }
                        rows.Add(row);
                    } 
                }
            }
            return serializer.Serialize(rows);
        }

        

        [Route("api/Gettest")]
        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}