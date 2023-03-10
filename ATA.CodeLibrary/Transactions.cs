using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using ATA.ObjectModel;
using ATA.Member.Util;
using SusQTech.Data.DataObjects; 
using Microsoft.ApplicationBlocks.Data;

namespace ATA.ObjectModel
{
    [DataObject("p_Transactions_Create", "p_Transactions_Update", "p_Transactions_Load", "p_Transactions_Delete")]
    public class Transactions : DataObjectBase
    { 

        public Transactions()
        {
            this.TransactionID = DataObjectBase.NullIntRowId;
        }

        public Transactions(int transactionsId)
        {
            this.Load(transactionsId);
        }

        #region public properties

        [DataObjectProperty("TransactionID", SqlDbType.Int, true)]
        public int TransactionID { get; set; }

        [DataObjectProperty("TransactionTypeID", SqlDbType.Int)]
        public int TransactionTypeID { get; set; }

        [DataObjectProperty("ItemID", SqlDbType.Int)]
        public int ItemID { get; set; }

        
        [DataObjectProperty("Name", SqlDbType.NVarChar, 100)]
        public string Name { get; set; }

        [DataObjectProperty("Value", SqlDbType.NVarChar, 400)]
        public string Value { get; set; }

        [DataObjectProperty("Name1", SqlDbType.NVarChar, 100)]
        public string Name1 { get; set; }

        [DataObjectProperty("Value1", SqlDbType.NVarChar, 400)]
        public string Value1 { get; set; }

        [DataObjectProperty("ModifiedBy", SqlDbType.Int)]
        public int ModifiedBy { get; set; }

        [DataObjectProperty("TransactionDate", SqlDbType.DateTime)]
        public DateTime TransactionDate { get; set; }


        #endregion

        #region Get Methods

        public static void setTransaction(int ItemID, int ModifiedBy, string Name, string Value, string Name1, string Value1, TransactionType TransactionTypeID)
        {
            try
            {
                Transactions t = new Transactions();
                t.ItemID = ItemID;
                t.ModifiedBy = ModifiedBy;// DBUtilAPIController.CurrentUser().UserId;
                t.TransactionDate = DateTime.Now;
                t.Name = Name;
                t.Value = Value;
                t.Name1 = Name1;
                t.Value1 = Value1;
                t.TransactionTypeID = (int)TransactionTypeID;
                t.Save();
            }
            catch (Exception ex)
            {
                var message = string.Format("Error saving Transaction - Method 1!" + ex.Message);
                throw new Exception(message);
            }
        }
        //Was written to be used for Remove user's from group  methods
        public static void setTransaction(DataTable dt, int ModifiedBy, TransactionType TransactionTypeID)
        {
            try
            {
                foreach (DataRow dr in dt.Rows)
                {
                    int GroupId = 0;
                    int.TryParse(dr["GroupId"].ToString(),out GroupId);
                    int RoleId = 0;
                    int.TryParse(dr["RoleId"].ToString(), out RoleId);
                    int UserId = 0;
                    int.TryParse(dr["UserId"].ToString(),out UserId);

                    if (GroupId > 0 && RoleId > 0 && UserId > 0)
                    {
                        Transactions t = new Transactions();
                        t.ItemID = GroupId;
                        t.ModifiedBy = ModifiedBy;//DBUtilAPIController.CurrentUser().UserId;
                        t.TransactionDate = DateTime.Now;
                        t.Name = "UserID";
                        t.Value = UserId.ToString();
                        t.Name1 = "CommitteeRoleID";
                        t.Value1 = RoleId.ToString();
                        t.TransactionTypeID = (int)TransactionTypeID;
                        t.Save();
                    }
                }
            }
            catch (Exception ex)
            {
                var message = string.Format("Error saving Transaction - Method 1!" + ex.Message);
                throw new Exception(message);
            }
        }
        public static void setUserGroupTransaction(UserGroupJsonModel ug, TransactionType ty, CommitteeRole cr,int UserID)
        {
            if (ty == TransactionType.UserGroup_UserRemoved)
            {
                int groupId = ug.GroupId;
                #region old code - delete later
                //SqlParameter[] sqlParams = new SqlParameter[10];
                //sqlParams[0] = new SqlParameter("GroupId", groupId);
                //sqlParams[1] = new SqlParameter("UserId", 0);
                //sqlParams[2] = new SqlParameter("CompanyName", ug.CompanyName);
                //sqlParams[3] = new SqlParameter("CommitteeRoleId", (int)cr);
                //sqlParams[4] = new SqlParameter("TransactionTypeID", (int)ty);
                //sqlParams[5] = new SqlParameter("ItemID", 0);
                //sqlParams[6] = new SqlParameter("Name", null);
                //sqlParams[7] = new SqlParameter("Value",null);
                //sqlParams[8] = new SqlParameter("ModifiedBy",1);
                //sqlParams[9] = new SqlParameter("TransactionDate", null);
                #endregion
                try
                {
                    using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
                    {
                        StringBuilder sql = new StringBuilder();
                        using (SqlCommand cmd = new SqlCommand("p_Transactions_UserGroup_Create", con))
                        {
                            con.Open(); 
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("GroupId", groupId));
                            cmd.Parameters.Add(new SqlParameter("CompanyName", ug.CompanyName));
                            cmd.Parameters.Add(new SqlParameter("CommitteeRoleId", (int)cr));
                            cmd.Parameters.Add(new SqlParameter("TransactionTypeID", (int)ty)); 
                            cmd.Parameters.Add(new SqlParameter("ModifiedBy", UserID)); 
                            cmd.ExecuteNonQuery();  
                        }
                    }  
                }
                catch (Exception ex)
                {
                    var message = string.Format("Error saving Transaction - Method 2!" + ex.Message);
                    throw new Exception(message);
                }
            }
        }

        public static void setUserGroupTransaction(int GroupId, TransactionType ty, CommitteeRole cr,int UserID)
        {
            if (ty == TransactionType.UserGroup_UserRemoved)
            { 
                try
                {
                    using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
                    {
                        StringBuilder sql = new StringBuilder();
                        using (SqlCommand cmd = new SqlCommand("p_Transactions_UserGroup_Create", con))
                        {
                            con.Open();
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("GroupId", GroupId));
                            cmd.Parameters.Add(new SqlParameter("CompanyName",null));
                            cmd.Parameters.Add(new SqlParameter("CommitteeRoleId", (int)cr));
                            cmd.Parameters.Add(new SqlParameter("TransactionTypeID", (int)ty));
                            cmd.Parameters.Add(new SqlParameter("ItemID", 0));
                            cmd.Parameters.Add(new SqlParameter("Name", null));
                            cmd.Parameters.Add(new SqlParameter("Value", null));
                            cmd.Parameters.Add(new SqlParameter("ModifiedBy", UserID));
                            cmd.Parameters.Add(new SqlParameter("TransactionDate", null));
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    var message = string.Format("Error saving Transaction - Method 3!" + ex.Message);
                    throw new Exception(message);
                }
            }
        }
        public static void setUserGroupTransaction(int GroupId, int UserId,TransactionType ty, CommitteeRole cr)
        { 
            try
            { 
                    using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
                    {
                        StringBuilder sql = new StringBuilder();
                        using (SqlCommand cmd = new SqlCommand("p_Transactions_UserGroup_Create", con))
                        {
                            con.Open();
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("GroupId", GroupId));
                            cmd.Parameters.Add(new SqlParameter("UserId", UserId));
                            cmd.Parameters.Add(new SqlParameter("CommitteeRoleId", (int)cr));
                            cmd.Parameters.Add(new SqlParameter("TransactionTypeID", (int)ty));
                            cmd.Parameters.Add(new SqlParameter("ItemID", 0));
                            cmd.Parameters.Add(new SqlParameter("Name", null));
                            cmd.Parameters.Add(new SqlParameter("Value", null));
                            cmd.Parameters.Add(new SqlParameter("ModifiedBy", UserId));
                            cmd.Parameters.Add(new SqlParameter("TransactionDate", null));
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    var message = string.Format("Error saving Transaction - Method 4!" + ex.Message);
                    throw new Exception(message);
                } 
        }
         
        public static void setUserGroupTransaction(UserGroupJsonModel ug, TransactionType ty,int UserID,bool isA4AStaff)
        { 
            //if (ty == TransactionType.UserGroup_UserRemoved)
            //{
                if (isA4AStaff && (ty == TransactionType.UserGroup_UserRemoved))
                {
                    int groupId = ug.GroupId;
                    try
                    {
                        //int result = SqlHelper.ExecuteNonQuery(ConnectionFactory.DataObjectConnectionString, "p_Transactions_UserGroup_Create", sqlParams);
                        using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
                        {
                            StringBuilder sql = new StringBuilder();
                            using (SqlCommand cmd = new SqlCommand("p_Transactions_UserGroup_Create", con))
                            {
                                con.Open();
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Add(new SqlParameter("GroupId", groupId));
                                cmd.Parameters.Add(new SqlParameter("UserId", ug.UserId));
                                cmd.Parameters.Add(new SqlParameter("CompanyName", ug.CompanyName));
                                cmd.Parameters.Add(new SqlParameter("CommitteeRoleId", null));
                                cmd.Parameters.Add(new SqlParameter("TransactionTypeID", (int)ty));
                                cmd.Parameters.Add(new SqlParameter("ItemID", 0));
                                cmd.Parameters.Add(new SqlParameter("Name", null));
                                cmd.Parameters.Add(new SqlParameter("Value", null));
                                cmd.Parameters.Add(new SqlParameter("ModifiedBy", UserID));
                                cmd.Parameters.Add(new SqlParameter("TransactionDate", null));

                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        var message = string.Format("Error saving Transaction - Method 5!" + ex.Message);
                        throw new Exception(message);
                    }               
            }
            else 
            //if(ty == TransactionType.UserGroup_UserAdded)
            {  
                if (ug.StaffSubscribe) 
                    Transactions.setTransaction(ug.GroupId,UserID, "UserID", ug.UserId.ToString(), "CommitteeRoleID", ((int)CommitteeRole.StaffSubscribe).ToString(), ty);
               
                if (ug.Alerts) 
                    Transactions.setTransaction(ug.GroupId,UserID, "UserID", ug.UserId.ToString(), "CommitteeRoleID", ((int)CommitteeRole.Alerts).ToString(), ty);
                
                if (ug.Participant) 
                    Transactions.setTransaction(ug.GroupId,UserID, "UserID", ug.UserId.ToString(), "CommitteeRoleID", ((int)CommitteeRole.Participant).ToString(), ty);
                
                if (ug.BounceReports) 
                    Transactions.setTransaction(ug.GroupId,UserID, "UserID", ug.UserId.ToString(), "CommitteeRoleID", ((int)CommitteeRole.BounceReports).ToString(), ty);
                 
                if (ug.EmailAdmin) 
                    Transactions.setTransaction(ug.GroupId,UserID, "UserID", ug.UserId.ToString(), "CommitteeRoleID", ((int)CommitteeRole.EmailAdmin).ToString(), ty);
                 
                if (ug.ManageGroup) 
                    Transactions.setTransaction(ug.GroupId,UserID, "UserID", ug.UserId.ToString(), "CommitteeRoleID", ((int)CommitteeRole.ManageGroup).ToString(), ty);
                 
                if (ug.Secretary) 
                    Transactions.setTransaction(ug.GroupId,UserID, "UserID", ug.UserId.ToString(), "CommitteeRoleID", ((int)CommitteeRole.Secretary).ToString(), ty);
                 
                if (ug.GroupLeader) 
                    Transactions.setTransaction(ug.GroupId,UserID, "UserID", ug.UserId.ToString(), "CommitteeRoleID", ((int)CommitteeRole.GroupLeader).ToString(), ty);
                  
                if (ug.Spokesperson) 
                    Transactions.setTransaction(ug.GroupId,UserID, "UserID", ug.UserId.ToString(), "CommitteeRoleID", ((int)CommitteeRole.Spokesperson).ToString(), ty);
                 
                if (ug.Informational) 
                    Transactions.setTransaction(ug.GroupId,UserID, "UserID", ug.UserId.ToString(), "CommitteeRoleID", ((int)CommitteeRole.Informational).ToString(), ty);
                 
                if (ug.ViceChair) 
                    Transactions.setTransaction(ug.GroupId,UserID, "UserID", ug.UserId.ToString(), "CommitteeRoleID", ((int)CommitteeRole.ViceChair).ToString(), ty);
                 
                if (ug.Chair) 
                    Transactions.setTransaction(ug.GroupId,UserID, "UserID", ug.UserId.ToString(), "CommitteeRoleID", ((int)CommitteeRole.Chair).ToString(), ty);
                 
                if (ug.Alternate) 
                    Transactions.setTransaction(ug.GroupId,UserID, "UserID", ug.UserId.ToString(), "CommitteeRoleID", ((int)CommitteeRole.Alternate).ToString(), ty);
                 
                if (ug.Primary) 
                    Transactions.setTransaction(ug.GroupId,UserID, "UserID", ug.UserId.ToString(), "CommitteeRoleID", ((int)CommitteeRole.Primary).ToString(), ty);

                if (ug.Administrator)
                    Transactions.setTransaction(ug.GroupId, UserID, "UserID", ug.UserId.ToString(), "CommitteeRoleID", ((int)CommitteeRole.Administrator).ToString(), ty);

                if (ug.Observer)
                    Transactions.setTransaction(ug.GroupId, UserID, "UserID", ug.UserId.ToString(), "CommitteeRoleID", ((int)CommitteeRole.Observer).ToString(), ty);

                if (ug.TeamLeader)
                    Transactions.setTransaction(ug.GroupId, UserID, "UserID", ug.UserId.ToString(), "CommitteeRoleID", ((int)CommitteeRole.TeamLeader).ToString(), ty);

                if (ug.FAACoChair)
                    Transactions.setTransaction(ug.GroupId, UserID, "UserID", ug.UserId.ToString(), "CommitteeRoleID", ((int)CommitteeRole.FAACoChair).ToString(), ty);

                if (ug.FAAChair)
                    Transactions.setTransaction(ug.GroupId, UserID, "UserID", ug.UserId.ToString(), "CommitteeRoleID", ((int)CommitteeRole.FAAChair).ToString(), ty);

                if (ug.CoChair)
                    Transactions.setTransaction(ug.GroupId, UserID, "UserID", ug.UserId.ToString(), "CommitteeRoleID", ((int)CommitteeRole.CoChair).ToString(), ty);

                /*

Co chair
FAA Chair
FAA Co Chair
Team Leader
Observer
Administrator
   */
            }
        }
        #endregion
    }    
}



