using ATA.LyrisProxy.A4A.LyrisSoap;
using System;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Net;
//using ATA.Member.Util;

namespace ATA.LyrisProxy.LyrisSoapProxy
{
    public sealed class LyrisManager2
    {
        private lmapi _api;

        private const string lyrisUserNameKey = "SusQTechLyrisManagerUsername";
        private const string lyrisUserPassKey = "SusQTechLyrisManagerPassword";

        public static string MarketingTypeString { get { return ListTypeEnum.marketing.ToString(); } }
        

        public static LyrisManager2 GetLyrisManager()
        {
            string param = ConfigurationManager.AppSettings["SusQTechLyrisManagerUrl"];
            string str2 = ConfigurationManager.AppSettings[lyrisUserNameKey];
            string str3 = ConfigurationManager.AppSettings[lyrisUserPassKey];
            return new LyrisManager2(param, str2, str3);
        }

        public static LyrisManager2 GetLyrisManager(string LyrisManagerUrl, string LyrisUsername, string LyrisPassword)
        { 
            return new LyrisManager2(LyrisManagerUrl, LyrisUsername, LyrisPassword);
        }

        private LyrisManager2(string LyrisManagerUrl, string Username, string Password)
        {
            CheckParameter(ref LyrisManagerUrl, true, true, false, 0x800, "LyrisManagerUrl");
            CheckParameter(ref Username, true, true, true, 60, "Username");
            CheckPasswordParameter(ref Password, 50, "Password");
            this._api = new lmapi();
            this._api.Url = LyrisManagerUrl;
            this._api.Credentials = new NetworkCredential(Username, Password);  
        }
        #region Create list
        public string CreateList(string listTypeString, string ListNameFromGroup, string ShortDescription, string Topic)
        {
            string adminName = ConfigurationManager.AppSettings[lyrisUserNameKey];
            string adminPassWord = ConfigurationManager.AppSettings[lyrisUserPassKey];
            string AdminEmail = ConfigurationManager.AppSettings["SusQTechLyrisManagerEmail"];
            return CreateList(listTypeString, ListNameFromGroup, ShortDescription, adminName, AdminEmail, adminPassWord, Topic);
        }

        public string CreateList(string listTypeString, string ListNameFromGroup, string ShortDescription, string AdminName, string AdminEmail, string AdminPassword, string Topic)
        {
            ListTypeEnum listType = (ListTypeEnum)Enum.Parse(typeof(ListTypeEnum), listTypeString);
            string ListName = this.EscapeListName(ListNameFromGroup);
            ShortDescription = this.EscapeSpecialChars(ShortDescription);
            AdminName = this.EscapeSpecialChars(AdminName);
            AdminEmail = this.EscapeSpecialChars(AdminEmail);
            AdminPassword = this.EscapeSpecialChars(AdminPassword);
            Topic = this.EscapeSpecialChars(Topic);
            CheckParameter(ref ListName, true, true, true, 60, "ListName");
            CheckParameter(ref ShortDescription, true, true, false, 200, "ShortDescription");
            CheckParameter(ref AdminName, true, true, true, 50, "AdminName");
            CheckParameter(ref AdminEmail, true, true, false, 100, "AdminEmail");
            CheckPasswordParameter(ref AdminPassword, 50, "AdminPassword");
            try
            {
                if (this.ListExists(ListName))
                {
                    throw new Exception("A list already exists in the system for name: " + ListName);
                }
                int result = this._api.CreateList(listType, ListName, ShortDescription, AdminName, AdminEmail, AdminPassword, Topic);
                if (result < 1)
                    throw new Exception("Create list failed for list name " + ListName);
            }
            catch (Exception ex)
            {
                //Log.LogError(string.Format("Error in create lyris list {0} because: {1}", ListName, ex.ToString())); 
                throw;
            } 
            return ListName;
        }
        #endregion

        #region update and delete Lyris list
        public bool UpdateNewList(string listName, string description)
        {
            ListStruct list = this.GetListByName(listName);
            if (list != null)
            {
                list.ShortDescription = description; 
                
                list.AllowInfo = true;
                list.AllowInfoSpecified = true;

                list.BlankSubjectOk = true;
                list.BlankSubjectOkSpecified = true;

                list.CleanAuto = true;
                list.CleanAutoSpecified = true;

                list.DefaultSubsetID = null;
                list.DefaultSubsetIDSpecified = true;

                list.DeliveryReports = 1;//new string[] { "1" }; edited after API update 051419

                list.MaxMessageSize = (Nullable<int>)50000; 

                list.MergeCapOverride = MergeCapOverrideEnum.Item3;
                list.MergeCapOverrideSpecified = true;  

                list.MessageFooterHTML = "<html dir=\"ltr\"><head><title></title> </head><body></body></html>";

                list.MessageHeaderHTML = "<html dir=\"ltr\"><head><title></title></head><body></body></html>";

                list.NoBodyOk = true;
                list.NoBodyOkSpecified = true;

                list.NoSearch = true;
                list.NoSearchSpecified = true;

                list.Visitors = true;
                list.VisitorsSpecified = true;

                list.MailStreamID = 1;
                list.MailStreamIDSpecified = true;  
                 

                //Admin only
                //list.AnyoneCanPost = (Nullable<bool>)false;
                list.OnlyAllowAdminSend = (Nullable<bool>)true;
                list.PostPassword = (Nullable<PostPasswordEnum>)PostPasswordEnum.Item0;
                //"anyone": 
                //list.OnlyAllowAdminSend = (Nullable<bool>)false;
                //list.PostPassword = (Nullable<PostPasswordEnum>)PostPasswordEnum.Item0; 

                //these settings should disable message moderation.
                list.Moderated = (Nullable<ModeratedEnum>)ModeratedEnum.no;
                list.ApproveNum = (Nullable<int>)(-1);
                list.ReleasePending = (Nullable<int>)0;
                list.AutoReleaseHour = (Nullable<int>)0;

                //ATA default settings see Service Items 53-62 etc.
                list.Disabled = (Nullable<bool>)false;
                list.NoEmail = (Nullable<bool>)false;
                list.RecencyEmailEnabled = (Nullable<bool>)false;
                list.RecencySequentialEnabled = (Nullable<bool>)false;
                list.RecencyTriggeredEnabled = (Nullable<bool>)false;
                list.RecencyWebEnabled = (Nullable<bool>)false;
                list.AllowDuplicatePosts = (Nullable<bool>)false;
                list.NoListHeader = (Nullable<bool>)false;
                list.ModifyHeaderDate = (Nullable<bool>)false;
                list.MakePostsAnonymous = (Nullable<bool>)false;
                list.AddListNameToSubject = (Nullable<bool>)false;
                list.ArchiveNum = (Nullable<int>)10;
                list.KeepOutmailPostings = (Nullable<int>)7; 
                
                list.NoArchive = (Nullable<bool>)false;
                list.ConfirmSubscribes = (Nullable<bool>)false;
                list.NewSubscriberSecurity = (Nullable<NewSubscriberPolicyEnum>)NewSubscriberPolicyEnum.closed;
                list.MessageFooter = string.Empty;
                list.MessageHeader = string.Empty;
                list.AnyoneCanPost = (Nullable<bool>)false;
                list.DigestFooter = string.Empty;
                list.To = "nochange";
              
                try
                {
                    bool updateWorked = this._api.UpdateList(list);
                    if (!updateWorked)
                        throw new Exception("Failed to Update Lyris List" + listName);
                    ATA.LyrisProxy.LyrisList.SetNewListExtraSetting(listName);//update extra field value
                }
                catch (Exception err)
                {
                    //Log.LogError(string.Format("Error in update lyris list {0} because: {1}", listName, err.ToString()));
                    throw; 
                } 
                return true; 
            }
            else
            {
                throw new Exception("Error recovering the Lyris list for update for list name: " + listName);
            }
        }

        /// <summary>
        /// When updating a list "listName" is the key so it can't be changed.
        /// </summary>
        /// <param name="listName"></param>
        /// <param name="newDescription"></param>
        /// <param name="newTopic"></param>
        public void UpdateExistingList(string listName,  string newDescription, string newTopic)
        { 
            ListStruct list = this.GetListByName(listName); 
            list.ShortDescription = newDescription;
            list.Topic = newTopic;
            this._api.UpdateList(list); 
        }

        /// <summary>
        /// When updating a list "listName" is the key so it can't be changed.
        /// </summary>
        /// <param name="listName"></param>
        /// <param name="newDescription"></param>
        /// <param name="newTopic"></param>
        public void UpdateExistingList(string listName, string newDescription)
        {
            ListStruct list = this.GetListByName(listName);
            list.ShortDescription = newDescription; 
            this._api.UpdateList(list);
        }

        public bool DeleteList(string ListName)
        {
            ListName = this.EscapeListName(ListName);
            try
            {
                return this._api.DeleteList(ListName);
            }
            catch (Exception err)
            {
                //Log.LogError(string.Format("Error in delete lyris list {0} because: {1}", ListName, err.ToString()));
                throw err; 
            }
        }
        #endregion  

       


        public bool AddMemberToList(string Email, string FullName, string ListName)
        {
            ListName = this.EscapeListName(ListName);
            Email = this.EscapeSpecialChars(Email);
            FullName = this.EscapeSpecialChars(FullName);
            CheckParameter(ref Email, true, true, true, 100, "Email");
            CheckParameter(ref FullName, true, true, false, 100, "FullName");
            CheckParameter(ref ListName, true, true, true, 60, "ListName");
            int num = 0;
            try
            {
                num = this._api.CreateSingleMember(Email, FullName, ListName);
            }
            catch (Exception exception)
            {
                num = 0;
                //Log.LogError(string.Format("Error in add member {0} to list {1} because: {2}", Email, ListName, exception.ToString()));
                throw;
            }
            return (num > 0);
        }

        public bool UpdateListAdmin(string ListName, string MemberEmailAddress, bool IsAdmin, bool ReceiveListAdminEmail, bool ReceiveModerationNotification, bool BypassListModeration)
        {
            bool flag = false;
            //ListName = this.EscapeListName(ListName);
            //MemberEmailAddress = this.EscapeSpecialChars(MemberEmailAddress);
            //CheckParameter(ref ListName, true, true, false, 60, "ListName");
            //CheckParameter(ref MemberEmailAddress, true, true, true, 100, "MemberEmailAddress");
            //FilterCriteriaBuilder builder = new FilterCriteriaBuilder();
            //builder.AddCriteria(FilterFieldNames.Email, MemberEmailAddress);
            //builder.AddCriteria(FilterFieldNames.ListName, ListName);
            //SimpleMemberStruct[] structArray = null;
            //try
            //{
            //    structArray = this._api.SelectSimpleMembers(builder.ToArray());
            //}
            //catch (Exception exception)
            //{
            //    LogToEventLog(string.Format("Message: {0}\n\nStack Trace: {1}", exception.Message, exception.StackTrace), true);
            //    structArray = null;
            //}
            //if ((structArray != null) && (structArray.Length > 0))
            //{
            //    flag = this._api.UpdateListAdmin(structArray[0], IsAdmin, ReceiveListAdminEmail, ReceiveModerationNotification, BypassListModeration);
            //}
            return flag;
        }


        public bool UpdateMemberPassword(string Email, string Password)
        {
            bool flag = false;
            Email = this.EscapeSpecialChars(Email);
            CheckParameter(ref Email, true, true, true, 100, "Email");
            CheckParameter(ref Password, true, true, true, 50, "Password");
            try
            {
                string[] strArray = this._api.EmailOnWhatLists(Email);
                SimpleMemberStruct simpleMemberStructIn = new SimpleMemberStruct();
                simpleMemberStructIn.EmailAddress = Email;
                for (int i = 0; i < strArray.Length; i++)
                {
                    simpleMemberStructIn.ListName = strArray[i];
                    this._api.UpdateMemberPassword(simpleMemberStructIn, Password);
                }
            }
            catch (Exception exception)
            {
                //Log.LogError(string.Format("Error in UpdateMemberPassword for member {0}  because: {1}", Email, exception.ToString()));
                flag = false;
            }
            return flag;
            //Old update code use Susqutech dll
            //LyrisManager manager = LyrisManagerFactory.GetLyrisManager();
            //manager.UpdateMemberPassword(user.Email, txtPassword1.Text);
        }

        #region Private methods
        private string EscapeListName(string inString)
        {
            if (!string.IsNullOrEmpty(inString))
            {
                inString = Regex.Replace(inString, @"\W", "");
                inString = inString.ToLower();
            }
            return inString;
        }

        private string EscapeSpecialChars(string inString)
        {
            inString = inString.Replace(@"\", @"\");
            return inString;
        }

        private static void CheckParameter(ref string param, bool checkForNull, bool checkIfEmpty, bool checkForCommas, int maxSize, string paramName)
        {
            if (param == null)
            {
                if (checkForNull)
                {
                    throw new ArgumentNullException(paramName);
                }
            }
            else
            {
                param = param.Trim();
                if (checkIfEmpty && string.IsNullOrEmpty(param))
                {
                    throw new ArgumentException("Parameter_can_not_be_empty", paramName);
                }
                if ((maxSize > 0) && (param.Length > maxSize))
                {
                    throw new ArgumentException("Parameter_too_long", paramName);
                }
                if (checkForCommas && param.Contains(","))
                {
                    throw new ArgumentException("Parameter_can_not_contain_comma", paramName);
                }
            }
        }

        private static void CheckPasswordParameter(ref string param, int maxSize, string paramName)
        {
            if (string.IsNullOrEmpty(param)) 
                throw new ArgumentException("Parameter_can_not_be_empty", paramName); 
            if ((maxSize > 0) && (param.Length > maxSize))
            {
                throw new ArgumentException("Parameter_too_long", paramName);
            }
        }

        #region List query
        private ListStruct GetListByName(string ListName)
        {
            ListName = this.EscapeListName(ListName);
            CheckParameter(ref ListName, true, true, false, 60, "ListName");
            ListStruct[] structArray = null;
            try
            {
                structArray = this._api.SelectLists(ListName, string.Empty);
            }
            catch (Exception exception)
            {
                //Log.LogError(string.Format("Error in get lyris list by name {0} because: {1}", ListName, exception.ToString()));
                structArray = null;
            }
            ListStruct struct2 = null;
            if ((structArray != null) && (structArray.Length > 0))
            {
                struct2 = structArray[0];
            }
            return struct2;
        }


        private bool ListExists(string ListName)
        {
            ListName = this.EscapeListName(ListName);
            CheckParameter(ref ListName, true, true, true, 60, "ListName");
            ListStruct listByName = null;
            try
            {
                listByName = this.GetListByName(ListName);
            }
            catch (Exception exception)
            {
                //Log.LogError(string.Format("Error in get lyris list exists by name {0} because: {1}", ListName, exception.ToString()));
                listByName = null;
            }
            return (listByName != null);
        }
        #endregion
        #endregion 
    }
}
