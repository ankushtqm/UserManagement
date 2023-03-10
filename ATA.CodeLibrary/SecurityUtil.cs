using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
//using ATA.Member.Util;
//using Microsoft.SharePoint;

namespace ATA.CodeLibrary
{
    public class SecurityUtil
    { 
    //    public static bool IsCurrentUserInSharepointGroup(string adGroupName)
    //    {
    //        try
    //        {
    //            SPGroup group = SPContext.Current.Web.SiteGroups[adGroupName];
    //            if (group.ContainsCurrentUser)
    //            {
    //                return true;
    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            Log.LogError(string.Format("Error in IsCurrentUserInSharepointGroup method for group {0} because {1}", adGroupName, e.ToString()));
    //        }
    //        return false;
    //    }

    //    public static bool IsCurrentUserInAdGroup(string adGroupName, HttpRequest request)
    //    {
    //        try
    //        {
    //            foreach (System.Security.Principal.IdentityReference group in request.LogonUserIdentity.Groups)
    //            {
    //                string groupName = RemoveDomainName(group.Translate(typeof(System.Security.Principal.NTAccount)).ToString());
    //                if (string.Compare(adGroupName, groupName, true) == 0)
    //                    return true;
    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            Log.LogError(string.Format("Error in IsCurrentUserInAdGroup method for group {0} because {1}", adGroupName, e.ToString()));
    //        }
    //        return false;
    //    }

    //    private static readonly char[] NAME_PATH_SEPARATOR = { '\\' };
    //    public static string RemoveDomainName(string path)
    //    {
    //        string[] userPath = path.Split(NAME_PATH_SEPARATOR);
    //        return userPath[userPath.Length - 1];
    //    }
    }
}
