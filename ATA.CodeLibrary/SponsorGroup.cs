#region namespaces 
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using SusQTech.Data.DataObjects; 
using System.Web.Security;
//#BYNA using ATA.Authentication;
//using ATA.Authentication.Providers; 
using Microsoft.ApplicationBlocks.Data;
#endregion

namespace ATA.CodeLibrary
{
    public class SponsorGroup
    {
        private const string allSponsorGroupSql = "SELECT SponsorGroupId, GroupName FROM SponsorGroup";

        //public static SortedDictionary<string, int> GetSortedSponsorGroupNameToId()
        //{  
        //    SortedDictionary<string, int> sorted = new SortedDictionary<string, int>(); 
        //    using (SqlDataReader reader = SqlHelper.ExecuteReader(ConnectionFactory.DataObjectConnectionString, CommandType.Text, allSponsorGroupSql))
        //    {
        //        while (reader.Read())
        //        {
        //            sorted[reader.GetString(1)] = reader.GetInt32(0); 
        //        }
        //        reader.Close();
        //    } 
        //    return sorted;
        //} 
    }
}
