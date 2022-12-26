//#region namespaces

//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.SqlClient;
//using System.Text;
//using SusQTech.Data.DataObjects;
//using Microsoft.ApplicationBlocks.Data; 
//using ATA.Member.Util;

//#endregion

//namespace ATA.ObjectModel
//{
//    public class GroupSubscriptionManager
//    {
//        #region private members

//        private static GroupSubscriptionManager _instance = null;

//        private const string AddGroupSubscriptionProcedure = "p_Subscription_AddGroupToGroup"; 
//        private const string RemoveGroupSubscriptionProcedure = "p_Subscription_RemoveGroupFromGroup"; 
//        private const string GetAllGroupSubscribersProcedure = "p_Subscription_GetGroupSubscribersOfGroup"; 
//        private const string GetAllPossibleGroupSubscribersProcedure = "p_Subscription_GetPossibleGroupSubscribersOfGroup"; 

//        private LyrisManager _manager = null;

//        #endregion

//        private GroupSubscriptionManager() {}

//        public static GroupSubscriptionManager Instance
//        {
//            get
//            {
//                if (GroupSubscriptionManager._instance == null)
//                {
//                    GroupSubscriptionManager._instance = new GroupSubscriptionManager();
//                }
//                return (GroupSubscriptionManager._instance);
//            }
//        }

//        private LyrisManager Manager
//        {
//            get
//            {
//                if (this._manager == null)
//                    this._manager = LyrisManagerFactory.GetLyrisManager();

//                return (this._manager);
//            }
//        }

//        #region Get All Subscribers methods 

//        public SubscriptionGroup[] GetAllGroupSubscribersOfGroup(int GroupId)
//        {
//            SqlParameter[] sqlParams = new SqlParameter[1];
//            sqlParams[0] = new SqlParameter("GroupId", GroupId);

//            DataObjectList<SubscriptionGroup> groups = new DataObjectList<SubscriptionGroup>(sqlParams, GroupSubscriptionManager.GetAllGroupSubscribersProcedure);

//            return (groups.ToArray());
//        }

//        #endregion

//        #region Get All POSSIBLE Subscribers methods

//        public SubscriptionGroup[] GetAllPossibleGroupSubscribersOfGroup(int GroupId, bool isSecurityGroup)
//        {
//            SqlParameter[] sqlParams = new SqlParameter[2];
//            sqlParams[0] = new SqlParameter("GroupId", GroupId);
//            sqlParams[1] = new SqlParameter("IsSecurityGroup", isSecurityGroup);  
//            DataObjectList<SubscriptionGroup> groups = new DataObjectList<SubscriptionGroup>(sqlParams, GroupSubscriptionManager.GetAllPossibleGroupSubscribersProcedure);

//            return (groups.ToArray());
//        } 
//        #endregion

//        #region Remove Subscribers Methods 

//        public void RemoveGroupSubscriptionsFromGroup(int GroupId, int[] UnsubscribeGroupIds)
//        {
//            foreach (int UnsubscribeGroupId in UnsubscribeGroupIds)
//                this.RemoveGroupSubscriptionFromGroup(GroupId, UnsubscribeGroupId);
//        } 

//        public void RemoveGroupSubscriptionFromGroup(int GroupId, int UnsubscribingGroupId)
//        {
//            SqlParameter[] sqlParams = new SqlParameter[2];
//            sqlParams[0] = new SqlParameter("GroupId", GroupId);
//            sqlParams[1] = new SqlParameter("UnsubscribingGroupId", UnsubscribingGroupId);
//            SqlHelper.ExecuteNonQuery(ConnectionFactory.DataObjectConnectionString,CommandType.StoredProcedure, GroupSubscriptionManager.RemoveGroupSubscriptionProcedure, sqlParams);

//            ATAGroup group = new ATAGroup(GroupId);
//            ATAGroup unsubscribingGroup = new ATAGroup(UnsubscribingGroupId);
//            if (!Conf.SkipLyrisManager)
//                this.Manager.RemoveMemberFromList(group.LyrisListName, LyrisManager.MakeListEmailAddress(unsubscribingGroup.LyrisListName));
//        }

//        #endregion

//        #region Add Subscriber Methods 
       

//        public void AddGroupSubscriptionsToGroup(int GroupId, int[] SubscribingGroupIds)
//        {
//            foreach (int SubscribingGroupId in SubscribingGroupIds)
//                this.AddGroupSubscriptionToGroup(GroupId, SubscribingGroupId);
//        }

        

//        public void AddGroupSubscriptionToGroup(int GroupId, int SubscribingGroupId)
//        {
//            SqlParameter[] sqlParams = new SqlParameter[2];
//            sqlParams[0] = new SqlParameter("GroupId", GroupId);
//            sqlParams[1] = new SqlParameter("SubscribingGroupId", SubscribingGroupId);
//            SqlHelper.ExecuteNonQuery(ConnectionFactory.DataObjectConnectionString, CommandType.StoredProcedure, GroupSubscriptionManager.AddGroupSubscriptionProcedure, sqlParams);

//            ATAGroup group = new ATAGroup(GroupId);
//            ATAGroup subscribingGroup = new ATAGroup(SubscribingGroupId);

//            string ListEmail = LyrisManager.MakeListEmailAddress(subscribingGroup.LyrisListName);

//            //add the group as a member of the list.
//            if(!Conf.SkipLyrisManager)
//                this.Manager.AddMemberToList(ListEmail, subscribingGroup.GroupName, group.LyrisListName);
//        }

//        #endregion
//    }
//}
