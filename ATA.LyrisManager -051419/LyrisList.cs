using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ATA.Member.Util;
using Microsoft.ApplicationBlocks.Data; 
using System.Configuration;
using System.Data;
using System.Data.SqlClient;


namespace ATA.LyrisProxy
{
    public class LyrisList
    {
        public static string GetListDescriptionByName(string listName)
        {
            if (string.IsNullOrEmpty(listName))
                return listName;
            try{
    
                string sql = "select [DescShort_] from [lists_] where  Lower([Name_])=@listName";
                SqlParameter[] parameters = new SqlParameter[1]; 
                parameters[0] = new SqlParameter("@listName", listName.ToLower());
                
                object o = SqlHelper.ExecuteScalar(LyrisMember.lyrisConnStr, CommandType.Text, sql, parameters);
                return (o == null) ? listName : o.ToString();
            }
            catch (Exception e)
            {
                //Log.LogError(string.Format("Error in GetListDescriptionByName for list {0} because {1}", listName, e.ToString()));
                return listName;
            } 
        }

        public static void SetNewListExtraSetting(string listName)
        {
            try
            {
                //NoMidRewr_ true in code 
                //change merge capabiltiy 2 in code
                // DKIMListSign_ null
                // DKSenderListSign_F 
                //DKSenderListSign_ T
                //DKIMListSign_=null, DKSenderListSign_='F', DKSenderListSign_='T' 

                //“NoEmailSub_” in dev is set to “F”. It should be set to “T”
                string sql = "Update dbo.[lists_] set mergecapabilities_ = 2, NoMidRewr_='T', DeliveryReports_=1, NoEmailSub_='T' " 
                + " where  Name_='" + listName.Replace("'", "''" )+ "'";
                //Log.LogInfo("Executing sql " + sql);
                SqlHelper.ExecuteNonQuery(LyrisMember.lyrisConnStr, CommandType.Text, sql, null);
                
            }
            catch (Exception e)
            {
                //Log.LogError(string.Format("Error in SetNewListSetting for list {0} because {1}", listName, e.ToString()));
                throw;
            } 
        }

        public static string MakeListEmailAddress(string ListName)
        {
            return string.Format("{0}{1}", ListName, ListEmailAddressBase);
        }

        public static string ListEmailAddressBase
        {
            get
            {
                return ConfigurationManager.AppSettings["SusQTechLyrisManagerEmailAddressBase"];
            }
        }

        public static bool ListExists(string listName)
        { 
            try
            { 
                string sql = "select 1 from [lists_] where  Lower([Name_])=@listName";
                SqlParameter[] parameters = new SqlParameter[1];
                parameters[0] = new SqlParameter("@listName", listName.ToLower()); 
                object o = SqlHelper.ExecuteScalar(LyrisMember.lyrisConnStr, CommandType.Text, sql, parameters);
                return (o != null);
            }
            catch (Exception e)
            {
                //Log.LogError(string.Format("Error in ListExists for list {0} because {1}", listName, e.ToString()));
                return false;
            }
        } 
    }
}
