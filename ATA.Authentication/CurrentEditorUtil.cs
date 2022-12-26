//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Net;

//namespace ATA.Authentication
//{
//    public class CurrentEditorUtil
//    {
//        //    public static bool IsCurrentEditorSupperAdmin()
//        //    {
//        //        SPUser currentUser = .Current.Web.CurrentUser;
//        //        if (currentUser != null && currentUser.IsSiteAdmin)
//        //            return true;
//        //        string userName = GetCurrentEditorUserName();
//        //        return ATAGroup.IsUserADAdmin(userName);
//        //    }

//        //    public static bool IsCurrentEditorFuelPortalAdmin()
//        //    {
//        //        SPUser currentUser = SPContext.Current.Web.CurrentUser;
//        //        if (currentUser != null && currentUser.IsSiteAdmin)
//        //            return true;
//        //        if (SecurityUtil.IsCurrentUserInSharepointGroup(Constants.SharepointGroupName_FuelsPortalAdmin))
//        //            return true;
//        //        return false;
//        //    }

//        //    public static bool IsCurrentEditorSASAdmin()
//        //    {
//        //        SPUser currentUser = SPContext.Current.Web.CurrentUser;
//        //        if (currentUser != null && currentUser.IsSiteAdmin)
//        //            return true;
//        //        if (SecurityUtil.IsCurrentUserInSharepointGroup(Constants.SharepointGroupName_EconGroupAdmin))
//        //            return true;
//        //        return false;
//        //    }

//        //    public static bool CanEditGroup(int groupId)
//        //    {
//        //        if (IsCurrentEditorSupperAdmin())
//        //            return true;

//        //        ATAMembershipUser user = CurrentEditorUtil.GetCurrentEditorUser();
//        //        return CurrentEditorUtil.IsCurrentEditorGroupAdmin(groupId, user.UserId);
//        //    }




//        //    public static bool IsCurrentEditorGroupAdmin(int groupId, int crrentEditorUserId)
//        //    {
//        //        Dictionary<int, bool> groupAdmins = ATAGroupManager.GetGroupAdmins(groupId);
//        //        return groupAdmins.ContainsKey(crrentEditorUserId);
//        //    }

//        //public static string GetCurrentEditorUserName()
//        //{
//        //  // string userName = SPContext.Current.Web.CurrentUser.LoginName
//        //   // if (userName.ToLower() == "sharepoint\\system")//if the current user used for the app pool then SPContext.Current.Web.CurrentUser.LoginName is sharepoint\\system
//        //    string userName = HttpContext.Current.User.Identity.Name;
//        //    return userName;
//        //}

//        public static int GetCurrentEditorUserId()
//        {
//            ATAMembershipUser memberUser = GetCurrentEditorUser();
//            return memberUser.UserId;
//        }

//        public static string GetCurrentEditorUserEmail()
//        {
//            ATAMembershipUser memberUser = GetCurrentEditorUser();
//            return memberUser.Email;
//        }

//        public static ATAMembershipUser GetCurrentEditorUser(string username)
//        {
//            string userName = GetCurrentEditorUserName();
//            return GetMembershipUserByUserName(userName);
//        }

//        //    #region private methods
//        //    private static ATAMembershipUser GetMembershipUserByUserName(string userName)
//        //    {
//        //        return GetMembershipUserByUserName(userName, ATAMembershipUtility.Instance.GetATAMembershipProvider());
//        //    }

//        //    private static ATAMembershipUser GetMembershipUserByUserName(string userName, ATAMembershipProvider provider)
//        //    {
//        //        string parsedUserName = ATAMembershipUtility.Instance.ParseUsername(userName);
//        //        ATAMembershipUser membershipUser = (ATAMembershipUser)provider.GetUser(parsedUserName, false);
//        //        if (membershipUser == null)
//        //            throw new Exception(string.Format("Could not load user record for: {0}", userName));
//        //        return membershipUser;
//        //    }
//        //   #endregion
//    }
//}
