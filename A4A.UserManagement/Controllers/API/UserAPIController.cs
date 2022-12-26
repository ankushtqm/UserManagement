using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web;
using ATA.ObjectModel;
using ATA.Member.Util;
using ATA.Authentication;
using ATA.Authentication.Providers;
using System.Web.Security;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Data;
using System.Data.SqlClient;
using SusQTech.Data.DataObjects;
using System.Configuration;

namespace A4A.UM.Controllers
{
    /// <summary>
    /// Student Api controller
    /// </summary>
    public class UserAPIController : ApiController
    {

        public ATAMembershipUser _currentUser;
        public ATAMembershipUser CurrentUser()
        {
            if (_currentUser == null)
            {
                ATAMembershipProvider provider = ATAMembershipUtility.Instance.GetATAMembershipProvider();
                string editorUserName = ATAMembershipUtility.Instance.ParseUsername(HttpContext.Current.User.Identity.Name);
                ATAMembershipUser _currentUser = (ATAMembershipUser)provider.GetUser("teqmavens", true);
            }
            return _currentUser;
        }
        [Route("api/User")]
        public IEnumerable<LookupUser> Get()
        {
            return DbUtil.GetAllContacts();
        }
        /*Older Method used for validating if email already exisited*/
        protected bool IsEmailInUse(string email)
        {
            ATAMembershipProvider provider = ATAMembershipUtility.Instance.GetATAMembershipProvider();
            return provider.EmailExists(email);
        }

        [Route("api/getuseraddress")]
        public string GetUserAddresses(int UserId)
        {
            DataTable dt = new DataTable();
            StringBuilder sql = new StringBuilder();
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("p_User_GetUserAddresses", con))
                {
                    con.Open();
                    SqlParameter[] spm = new SqlParameter[1];
                    spm[0] = new SqlParameter("UserId", UserId);
                    cmd.Parameters.AddRange(spm);

                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    ATAMembershipProvider provider = ATAMembershipUtility.Instance.GetATAMembershipProvider();
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

        [Route("api/GetUserActivities")]
        public string GetUserActivities(string email = null, int? userid = null)
        {
            DataTable dt = new DataTable();
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
            {
                StringBuilder sql = new StringBuilder();
                using (SqlCommand cmd = new SqlCommand("p_Activity_Getlist", con))
                {
                    con.Open();
                    SqlParameter[] spm = new SqlParameter[1];
                    if (userid.Value > 0)
                    {
                        spm[0] = new SqlParameter("UserId", userid.Value);
                        cmd.Parameters.AddRange(spm);
                    }

                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

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


        [Route("api/GetIsUserExisting")]
        public string GetIsUserExisting(string email = null, int? userid = null, string term = null)
        {
            DataTable dt = new DataTable();
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
            {
                StringBuilder sql = new StringBuilder();
                using (SqlCommand cmd = new SqlCommand("p_User_Load", con))
                {
                    cmd.CommandTimeout = 120;
                    con.Open();
                    SqlParameter[] spm = new SqlParameter[1];
                    if (!string.IsNullOrEmpty(term))
                    {
                        spm[0] = new SqlParameter("Term", term);
                    }
                    else
                    if (!string.IsNullOrEmpty(email))
                    {
                        spm[0] = new SqlParameter("Username", email);
                    }
                    else
                    {
                        spm[0] = new SqlParameter("UserId", userid.Value);
                    }
                    cmd.Parameters.AddRange(spm);

                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    ATAMembershipProvider provider = ATAMembershipUtility.Instance.GetATAMembershipProvider();

                    foreach (DataRow dr in dt.Rows)
                    {

                        row = new Dictionary<string, object>();
                        foreach (DataColumn col in dt.Columns)
                        {
                            if (col.ColumnName.ToLower().Equals("password"))
                            {
                                string result = provider.UnEncodePassword(dr[col].ToString(), (int)provider.PasswordFormat);
                                row.Add(col.ColumnName, result);
                            }
                            else
                            {
                                row.Add(col.ColumnName, dr[col]);
                            }
                        }
                        rows.Add(row);
                    }
                }
            }
            return serializer.Serialize(rows);
        }

        public string IsEmailInUseActive(string email)
        {
            string Active = "None";
            ATAMembershipProvider provider = ATAMembershipUtility.Instance.GetATAMembershipProvider();
            if (provider.EmailExists(email))
            {
                ATAMembershipUser user = (ATAMembershipUser)provider.GetUser(email, true);

                if (user != null)
                {
                    if (user.IsActiveContact)
                    {
                        Active = "true";
                    }
                    else
                    {
                        Active = "inactive";
                    }
                }
            }
            else
            {
                Active = "false";
            }

            return Active;
        }

        public void SendNewUserEmail(ATAMembershipUser newUser)
        {
            //if (!(HasMemberPortalSiteAccess || HasFuelPortalSiteAccess) || !cbSendEmail.Checked)
            //    return;

            ////sends the email to the new user with username, password, link to login page, 
            ////company name, first name, last name 

            //string subject = "Welcome to the A4A Web Portal";
            //if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["NewUserEmailSubjectFormatString"]))
            //    subject = ConfigurationManager.AppSettings["NewUserEmailSubjectFormatString"];

            //string from = ConfigurationManager.AppSettings["NewUserEmailFromAddress"]; //editor.Email;
            //string to = newUser.Email;

            //StringBuilder sb = new StringBuilder();

            //string styleSheetUrl = string.Empty;
            //try
            //{
            //    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["NewUserEmailStyleSheetUrlFormatString"]))
            //        styleSheetUrl = string.Format(ConfigurationManager.AppSettings["NewUserEmailStyleSheetUrlFormatString"], SPContext.Current.Site.RootWeb.Url.TrimEnd(new char[] { '/' }));
            //}
            //catch { }

            //sb.AppendLine("<html>");
            //sb.AppendFormat("<head>");
            //sb.AppendFormat("<title>{0}</title>", subject);
            //if (!string.IsNullOrEmpty(styleSheetUrl))
            //    sb.AppendFormat("<link rel=\"stylesheet\" type=\"text/css\" href=\"{0}\"/>", styleSheetUrl);
            //sb.AppendLine("</head>");
            //sb.AppendLine("<body>");
            //sb.AppendLine(this.GenerateNewUserEmailBlock(newUser));
            //if (!string.IsNullOrEmpty(txtNewUserEmailAdditionalText.Text))
            //{
            //    sb.AppendLine("<br />");
            //    sb.AppendLine(txtNewUserEmailAdditionalText.Text);
            //}
            //sb.AppendLine("</body>");
            //sb.AppendLine("</html>");
            //try
            //{
            //    ATAMembershipUtility.Instance.SendEmail(sb.ToString(), to, from, subject, true, true);
            //}
            //catch (Exception err)
            //{
            //    throw new Exception(string.Format("There was an error sending the email: {0}", err.Message), err);
            //}
        }
        public string GenerateNewUserEmailBlock(ATAMembershipUser newUser)
        {
            return "";
            //bool isActiveFF = this.HasFuelPortalSiteAccess;
            //bool isAcitveMemberPortalUser = this.HasMemberPortalSiteAccess;
            //string emailTemplateFileName = null;
            //if (isActiveFF && isAcitveMemberPortalUser)
            //    emailTemplateFileName = "MemberAndFuelsNewUserEamil.txt";
            //else if (isActiveFF)
            //    emailTemplateFileName = "FuelsNewUserEamil.txt";
            //else if (isAcitveMemberPortalUser)
            //    emailTemplateFileName = "MemberNewUserEamil.txt";
            //else
            //    return string.Empty;
            //StringBuilder emailTemplate = new StringBuilder(ReadEmailTemplateFromFile(emailTemplateFileName));


            //string membersSiteFormsAuthURL = ConfigurationManager.AppSettings["MembersSiteFormsAuthURL"];
            //emailTemplate.Replace("{MembersSiteFormsAuthURL}", membersSiteFormsAuthURL);

            //string fuelsSiteFormsAuthURL = ConfigurationManager.AppSettings["FuelsSiteFormsAuthURL"];
            //emailTemplate.Replace("{FuelsSiteFormsAuthURL}", fuelsSiteFormsAuthURL);
            //emailTemplate.Replace("{FirstName}", firstName);

            //StringBuilder sb = new StringBuilder();
            //string displayPassword = (newUser == null) ? "[Password]" : newUser.GetPassword();
            //sb.AppendLine("<table cellpadding=\"2\" cellspacing=\"1\" border=\"0\">");
            //sb.AppendFormat("<tr><th align=\"left\"><label>Username:</label></th><td align=\"left\">{0}</td></tr>", userEmail);
            //sb.AppendFormat("<tr><th align=\"left\"><label>Password:</label></th><td align=\"left\">{0}</td></tr>", displayPassword);
            ////sb.AppendFormat("<tr><th align=\"left\"><label>First Name:</label></th><td align=\"left\">{0}</td></tr>", firstName);
            ////sb.AppendFormat("<tr><th align=\"left\"><label>Last Name:</label></th><td align=\"left\">{0}</td></tr>", lastName);
            ////sb.AppendFormat("<tr><th align=\"left\"><label>Company:</label></th><td align=\"left\">{0}</td></tr>", companyName);  
            //sb.AppendLine("</table>");
            ////sb.AppendLine("<br />"); 
            //emailTemplate.Replace("{userDetailPlaceHolder}", sb.ToString());
            //return emailTemplate.ToString();

        }

        [Route("api/RemoveAddress")]
        public void RemoveAddress(int UserId)
        {
            using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
            {
                StringBuilder sql = new StringBuilder();
                using (SqlCommand cmd = new SqlCommand("p_UserAddress_Delete", con))
                {
                    con.Open();
                    SqlParameter[] spm = new SqlParameter[1];

                    spm[0] = new SqlParameter("UserId", UserId);
                    cmd.Parameters.AddRange(spm);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
            }
        }
        [Route("api/SaveAddress")]
        public void SaveAddress(Address add, int addressType, string email)
        {
            string address1 = add.Address1;
            string address2 = add.Address2;
            if (string.IsNullOrEmpty(address1) && string.IsNullOrEmpty(address2))
                return;
            string city = add.City;
            string state = add.State;
            string zip = add.ZipCode;
            string province = add.Province;
            string country = add.Country;
            int addressTypeId = addressType;
            ATAMembershipProvider provider = ATAMembershipUtility.Instance.GetATAMembershipProvider();
            if (provider.EmailExists(email))
            {
                ATAMembershipUser user = (ATAMembershipUser)provider.GetUser(email, true);
                user.AddAddress(addressTypeId, address1, address2, city, state, zip, province, country);
            }
        }
        [Route("api/SaveUpdateAddress")]
        public void SaveUpdateAddress(Address add, int addressType, string email)
        {
            string address1 = add.Address1;
            string address2 = add.Address2;
            if (string.IsNullOrEmpty(address1) && string.IsNullOrEmpty(address2))
                return;
            string city = add.City;
            string state = add.State;
            string zip = add.ZipCode;
            string province = add.Province;
            string country = add.Country;
            int addressTypeId = addressType;
            ATAMembershipProvider provider = ATAMembershipUtility.Instance.GetATAMembershipProvider();
            if (provider.EmailExists(email))
            {
                ATAMembershipUser user = (ATAMembershipUser)provider.GetUser(email, true);
                using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
                {
                    StringBuilder sql = new StringBuilder();
                    using (SqlCommand cmd = new SqlCommand("p_Address_CreateUpdate", con))
                    {
                        con.Open();
                        SqlParameter[] spm = new SqlParameter[8];

                        spm[0] = new SqlParameter("UserId", user.UserId);
                        spm[1] = new SqlParameter("Address1", address1);
                        spm[2] = new SqlParameter("Address2", address2);
                        spm[3] = new SqlParameter("City", city);
                        spm[4] = new SqlParameter("State", state);
                        spm[5] = new SqlParameter("ZipCode", zip);
                        spm[6] = new SqlParameter("Province", province);
                        spm[7] = new SqlParameter("Country", country);
                        cmd.Parameters.AddRange(spm);

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
        [Route("api/User")]
        public HttpResponseMessage Post(ATAMembershipUser editor, int CompanyId, int? oldCompanyId, string isA4APortal, string isActiveContact, string password = null, string oldEmail = null, string companyName = null)
        {
            var message = string.Empty;
            bool isA4A = true;
            bool isActive = false;
            int idx = companyName.LastIndexOf('-');

            if (!isA4APortal.Equals("undefined"))
            {
                isA4A = bool.Parse(isA4APortal);
            }

            if (!isActiveContact.Equals("undefined"))
            {
                isActive = bool.Parse(isActiveContact);
            }

            if (!companyName.Contains("-"))
            {
                HttpError err = new HttpError("Please make selection from dropdown or create new company");
                return Request.CreateResponse(HttpStatusCode.BadRequest, err);
            }

            if (!(companyName.Contains("Member") || companyName.Contains("Non Member")))
            {
                HttpError err = new HttpError("Please make selection from dropdown or create new company");
                return Request.CreateResponse(HttpStatusCode.BadRequest, err);
            }

            if (!GetCompanyName(companyName.Substring(0, idx).Trim()))
            {
                HttpError err = new HttpError("Please make selection from dropdown or create new company");
                return Request.CreateResponse(HttpStatusCode.BadRequest, err);
            }
            //Get our provider, create user for existing object
            ATAMembershipProvider provider = ATAMembershipUtility.Instance.GetATAMembershipProvider();
            ATAMembershipUser user;
            if (string.IsNullOrEmpty(oldEmail) || oldEmail.Equals("undefined"))
            {
                user = (ATAMembershipUser)provider.GetUser(editor.Email, true);
            }
            else
            {
                user = (ATAMembershipUser)provider.GetUser(oldEmail, true);
            }
            string editorUserName = ATAMembershipUtility.Instance.ParseUsername(this.User.Identity.Name);
            ATAMembershipUser currentUser = (ATAMembershipUser)provider.GetUser(editorUserName, true);
            if (user != null)
            {
                try
                {
                    if (!oldEmail.Equals(editor.Email)) //validating for unique email address
                    {
                        if (provider.RequiresUniqueEmail && provider.EmailExists(editor.Email))
                        {
                            message = "Email already exist.";
                            HttpError err = new HttpError(message);
                            return Request.CreateResponse(HttpStatusCode.NotAcceptable, err);
                        }
                    }

                    user.FirstName = editor.FirstName;
                    user.MiddleName = editor.MiddleName == null ? string.Empty : editor.MiddleName;
                    user.LastName = editor.LastName;
                    user.PreferredName = editor.PreferredName;
                    user.JobTitle = editor.JobTitle;
                    user.HomePhone = editor.HomePhone == null ? string.Empty : editor.HomePhone;
                    user.OfficePhone = editor.OfficePhone == null ? string.Empty : editor.OfficePhone;
                    user.OfficePhoneExtension = editor.OfficePhoneExtension == null ? string.Empty : editor.OfficePhoneExtension;
                    user.MobilePhone = editor.MobilePhone == null ? string.Empty : editor.MobilePhone;
                    user.PrimaryFax = editor.PrimaryFax == null ? string.Empty : editor.PrimaryFax;
                    user.WebPage = editor.WebPage == null ? string.Empty : editor.WebPage;
                    user.Twitter = editor.Twitter == null ? string.Empty : editor.Twitter;
                    user.Facebook = editor.Facebook == null ? string.Empty : editor.Facebook;
                    user.LinkedIn = editor.LinkedIn == null ? string.Empty : editor.LinkedIn;
                    user.GooglePlus = editor.GooglePlus == null ? string.Empty : editor.GooglePlus;
                    user.Pinterest = editor.Pinterest == null ? string.Empty : editor.Pinterest;
                    user.Email = editor.Email;
                    user.IsActiveContact = editor.IsActiveContact;
                    user.IsActiveFF = editor.IsActiveFF;
                    user.IsActiveMB = editor.IsActiveMB;
                    user.IsActiveACH = editor.IsActiveACH;
                    user.Address1 = editor.Address1;
                    user.Address2 = editor.Address2;
                    user.City = editor.City;
                    user.State = editor.State;
                    user.Zipcode = editor.Zipcode;
                    user.Province = editor.Province;
                    user.Country = editor.Country;

                    if (string.IsNullOrEmpty(password))  /*If password is empty then set CureatingUserId, else set the lastupdateduserid*/
                    {
                        user.CreatingUserId = currentUser.UserId;
                    }
                    else
                    {
                        user.LastUpdatedDate = DateTime.Now;
                        user.LastUpdatingUserId = currentUser.UserId;
                    }
                    // provider.UpdateATAUser(user);  //Not working for some reason - worked before but stopped working 7/25/2017
                    provider.UpdateUser(user);
                    if (CompanyId != oldCompanyId)
                    {
                        user.AddCompany(string.Empty, CompanyId);
                        Transactions.setTransaction(user.UserId, currentUser.UserId, "Username", user.Email, "Company", oldCompanyId.ToString(), TransactionType.UserCompany_Changed);
                    }
                    if (!oldEmail.Equals(user.Email))
                    {
                        Transactions.setTransaction(user.UserId, currentUser.UserId, "Username", user.Email, "Email", oldEmail, TransactionType.UserEmail_Changed);
                    }
                    else
                    {
                        Transactions.setTransaction(user.UserId, currentUser.UserId, "Username", user.UserName, "PreferredName", user.PreferredName, TransactionType.UserModified);
                    }
                    if (!string.IsNullOrEmpty(password))
                    {
                        user.SetPassword(password);
                    }

                    if (isA4A != editor.IsActiveMB)
                    {
                        Transactions.setTransaction(user.UserId, currentUser.UserId, "Username", user.Email, "IsA4APortal", editor.IsActiveMB == true ? "Access Granted" : "Access Removed", TransactionType.A4A_Portal);
                    }


                    if (isActive != editor.IsActiveContact)
                    {
                        Transactions.setTransaction(user.UserId, currentUser.UserId, "Username", user.Email, "IsActiveContact", editor.IsActiveContact == true ? "Contact Activated" : "Contact Deactivated", TransactionType.IsActiveContact);
                    }

                    if (!oldEmail.Equals("undefined"))
                    {
                        if (!oldEmail.Equals(user.Email))
                        {
                            UpdateUsername(user.UserId, user.Email);
                            UpdateLyrisUserDetail(user.Email, oldEmail);
                        }
                    }
                    //Create a custom object to return with the password
                    dynamic testData = new System.Dynamic.ExpandoObject();
                    testData.UserId = user.UserId;
                    testData.Password = password;

                    var response = Request.CreateResponse(HttpStatusCode.Created, (object)testData); //user
                    return response;
                }
                catch (Exception ex)
                {
                    message = "Could not update the user." + ex.Message;
                    HttpError err = new HttpError(message);
                    return Request.CreateResponse(HttpStatusCode.NotFound, err);
                }
            }
            else
            {
                //get our provider, create our empty status object
                ATAMembershipUser newUser = null;
                MembershipCreateStatus status = MembershipCreateStatus.ProviderError;

                string newUserName = editor.Email;
                string newUserEmail = editor.Email;
                int newUserCompanyId = CompanyId;
                string newUserPassword = provider.GenerateRandomPassword(provider.MinRequiredPasswordLength);
                string newUserFirstname = editor.FirstName;
                string newUserLastname = editor.LastName;
                string newUserPreferredName = string.Format("{0} {1}", newUserFirstname, newUserLastname);
                string newUserMiddleName = editor.MiddleName == null ? string.Empty : editor.MiddleName;

                bool isActiveFF = editor.IsActiveFF;
                bool isActiveMB = editor.IsActiveMB;
                bool IsActiveACH = editor.IsActiveACH;

                string OfficePhone = editor.OfficePhone == null ? string.Empty : editor.OfficePhone;
                string MobilePhone = editor.MobilePhone == null ? string.Empty : editor.MobilePhone;
                string HomePhone = editor.HomePhone == null ? string.Empty : editor.HomePhone;
                string PrimaryFax = editor.PrimaryFax == null ? string.Empty : editor.PrimaryFax;
                string OfficePhoneExtension = editor.OfficePhoneExtension == null ? string.Empty : editor.OfficePhoneExtension;
                string WebPage = editor.WebPage == null ? string.Empty : editor.WebPage;

                string Pinterest = editor.Pinterest == null ? string.Empty : editor.Pinterest;
                string Twitter = editor.Twitter == null ? string.Empty : editor.Twitter;
                string JobTitle = editor.JobTitle == null ? string.Empty : editor.JobTitle;
                string Facebook = editor.Facebook == null ? string.Empty : editor.Facebook;
                string LinkedIn = editor.LinkedIn == null ? string.Empty : editor.LinkedIn;
                string GooglePlus = editor.GooglePlus == null ? string.Empty : editor.GooglePlus;
                string Address1 = editor.Address1;
                string Address2 = editor.Address2;
                string City = editor.City;
                string State = editor.State;
                string Zipcode = editor.Zipcode;
                string Province = editor.Province;
                string Country = editor.Country;

                int providerUserKey = DataObjectBase.NullIntRowId;
                int reportsToId;

                if (!(editor.ReportsToId > 0))
                    reportsToId = DataObjectBase.NullIntRowId;
                else
                    reportsToId = editor.ReportsToId;

                //Create User
                try
                {
                    newUser = provider.CreateATAUser(newUserName, newUserPassword, newUserEmail, providerUserKey, out status, reportsToId,
                    string.Empty, newUserFirstname, string.Empty, newUserLastname, string.Empty, HomePhone,
                    OfficePhone, MobilePhone, PrimaryFax, string.Empty, DataObjectBase.NullDateValue, isActiveMB, isActiveFF, IsActiveACH,
                    false, false, false, string.Empty, currentUser.UserId, newUserPreferredName, true, true, Address1, Address2, City, State, Zipcode, Province, Country);
                }
                catch (Exception ex)
                {
                    message = string.Format("Could not create new user!" + ex.ToString());
                }

                if (status == MembershipCreateStatus.Success && newUser != null)
                {
                    try
                    {

                        if (!oldEmail.Equals("undefined"))
                        {
                            Transactions.setTransaction(user.UserId, currentUser.UserId, "Username", user.Email, "Email", oldEmail, TransactionType.UserEmail_Changed);
                        }
                        else
                        {
                            //Add Transaction to DB
                            Transactions.setTransaction(newUser.UserId, currentUser.UserId, "Username", newUser.UserName, "PreferredName", newUser.PreferredName, TransactionType.UserCreated);
                        }
                        //Add other stuff
                        newUser.OfficePhoneExtension = OfficePhoneExtension;
                        newUser.WebPage = WebPage;
                        newUser.Facebook = Facebook;
                        newUser.LinkedIn = LinkedIn;
                        newUser.GooglePlus = GooglePlus;
                        newUser.Pinterest = Pinterest;
                        newUser.Twitter = Twitter;
                        newUser.JobTitle = JobTitle;
                        newUser.LastLoginDate = DateTime.MinValue;
                        // provider.UpdateATAUser(user);  //Not working for some reason - worked before but stopped working 7/25/2017
                        provider.UpdateUser(newUser);
                        //if they selected a company, add that here
                        if (newUserCompanyId > 0)
                            newUser.AddCompany(string.Empty, newUserCompanyId);

                        //Get the Password and Create a Dynamic object to pass back
                        string passwd = newUser.GetPassword();

                        dynamic testData = new System.Dynamic.ExpandoObject();
                        testData.UserId = newUser.UserId;
                        testData.Password = passwd;

                        var response = Request.CreateResponse(HttpStatusCode.Created, (object)testData);
                        return response;
                    }
                    catch (Exception ex)
                    {
                        message += string.Format("Error in creating a new user!{0}", ex.Message);
                        HttpError err = new HttpError(message);
                        return Request.CreateResponse(HttpStatusCode.NotFound, err);
                    }
                }
                else
                {
                    if (status == MembershipCreateStatus.DuplicateEmail)
                    {
                        message += string.Format("Contact with same email already exist.");
                    }
                    else
                    {
                        message += string.Format("Could not create new user.");
                    }
                    HttpError err = new HttpError(message);
                    return Request.CreateResponse(HttpStatusCode.NotFound, err);
                }
            }
        }

        [Route("api/GetUser")]
        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        [Route("api/GetNonMember")]
        public bool GetNonMember(int userId = 0)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
                {
                    StringBuilder sql = new StringBuilder();

                    using (SqlCommand cmd = new SqlCommand("p_getNonMember", con))
                    {
                        con.Open();
                        SqlParameter[] spm = new SqlParameter[1];
                        spm[0] = new SqlParameter("UserId", userId);
                        cmd.Parameters.AddRange(spm);


                        cmd.CommandType = CommandType.StoredProcedure;
                        var reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                return int.Parse(reader["NonMember"].ToString()) == 1;
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
            return false;
        }

        [HttpPost]
        [Route("api/UpdateUsername")]
        public void UpdateUsername(int userId, string username)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
                {
                    StringBuilder sql = new StringBuilder();

                    using (SqlCommand cmd = new SqlCommand("p_UpdateUsername", con))
                    {
                        con.Open();
                        SqlParameter[] spm = new SqlParameter[2];
                        spm[0] = new SqlParameter("UserId", userId);
                        spm[1] = new SqlParameter("Username", username);
                        cmd.Parameters.AddRange(spm);


                        cmd.CommandType = CommandType.StoredProcedure;
                        var reader = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
        }

        [HttpPost]
        [Route("api/UpdateLyrisUserDetail")]
        public void UpdateLyrisUserDetail(string email, string oldEmail)
        {
            try
            {
                string[] array = email.ToLower().Split('@');
                string UsernameLC = array[0];
                string Domain = array[1];

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["LyrisListDb"].ConnectionString))
                {
                    StringBuilder sql = new StringBuilder();

                    using (SqlCommand cmd = new SqlCommand("p_UpdateLyrisUserDetail", con))
                    {
                        con.Open();
                        SqlParameter[] spm = new SqlParameter[4];
                        spm[0] = new SqlParameter("OldUserEmail", oldEmail);
                        spm[1] = new SqlParameter("UserNameLC", UsernameLC);
                        spm[2] = new SqlParameter("DomainLC", Domain);
                        spm[3] = new SqlParameter("UserEmail", email);
                        cmd.Parameters.AddRange(spm);
                        cmd.CommandType = CommandType.StoredProcedure;
                        var reader = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
        }

        [Route("api/IsAddressExist")]
        public bool GetAddress(int userId = 0)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
                {
                    StringBuilder sql = new StringBuilder();

                    using (SqlCommand cmd = new SqlCommand("p_User_CheckUserAddress", con))
                    {
                        con.Open();
                        SqlParameter[] spm = new SqlParameter[1];
                        spm[0] = new SqlParameter("UserId", userId);
                        cmd.Parameters.AddRange(spm);


                        cmd.CommandType = CommandType.StoredProcedure;
                        var reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                return int.Parse(reader["IsExist"].ToString()) == 1;
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
            return false;
        }

        [Route("api/CheckCompanyName")]
        public bool GetCompanyName(string companyName = null)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
                {
                    StringBuilder sql = new StringBuilder();

                    using (SqlCommand cmd = new SqlCommand("p_User_CheckCompanyName", con))
                    {
                        con.Open();
                        SqlParameter[] spm = new SqlParameter[1];
                        spm[0] = new SqlParameter("CompanyName", companyName);
                        cmd.Parameters.AddRange(spm);


                        cmd.CommandType = CommandType.StoredProcedure;
                        var reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                return int.Parse(reader["IsExist"].ToString()) == 1;
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
            return false;
        }
    }
}
