using System; 
using System.Collections.Generic; 
using SusQTech.Data.DataObjects;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data; 
using ATA.Member.Util;
using System.Runtime.InteropServices;
//using ATA.Member.Util;


namespace ATA.ObjectModel
{
    [DataObject("p_UserGroupJsonModel_Create", "p_UserGroupJsonModel_Create", "p_UserGroupJsonModel_Load", "p_UserGroupJsonModel_Delete")]
    public class UserGroupJsonModel : DataObjectBase
    {
        #region private members

        private int _groupId;
        private int _userId;
        private bool _isManageGroup;
        private bool _isEmailAdmin;
        private bool _isBounceReports;
        private bool _isParticipant;
        private bool _isStaffSubscribe;
        private bool _isAlerts;
        private bool _isPrimary;
        private bool _isAlternate;
        private bool _isChair;
        private bool _isViceChair;
        private bool _isInformational;
        private bool _isSpokesperson;
        private bool _isGroupLeader;
        private bool _isSecretary;

        private bool _isCoChair;
        private bool _isFAAChair;
        private bool _isFAACoChair;
        private bool _isTeamLeader;
        private bool _isObserver;
        private bool _isAdministrator;

        private string _companyName;
        private string _userName;
        private string _Type;
        #endregion

        #region public properties
        [DataObjectProperty("GroupId", SqlDbType.Int, true)]
        public int GroupId
        {
            get
            {
                return _groupId;
            }

            set
            {
                _groupId = value;
            }
        }

        [DataObjectProperty("UserId", SqlDbType.Int)]
        public int UserId
        {
            get
            {
                return _userId;
            }

            set
            {
                _userId = value;
            }
        }
        
        [DataObjectProperty("UserName", SqlDbType.NVarChar)]
        public string UserName
        {
            get
            {
                return _userName;
            }

            set
            {
                _userName = value;
            }
        }

        [DataObjectProperty("ManageGroup", SqlDbType.Bit)]
        public bool ManageGroup
        {
            get
            {
                return _isManageGroup;
            }

            set
            {
                _isManageGroup = value;
            }
        }

        [DataObjectProperty("EmailAdmin", SqlDbType.Bit)]
        public bool EmailAdmin
        {
            get
            {
                return _isEmailAdmin;
            }

            set
            {
                _isEmailAdmin = value;
            }
        }

        [DataObjectProperty("BounceReports", SqlDbType.Bit)]
        public bool BounceReports
        {
            get
            {
                return _isBounceReports;
            }

            set
            {
                _isBounceReports = value;
            }
        }

        [DataObjectProperty("Participant", SqlDbType.Bit)]
        public bool Participant
        {
            get
            {
                return _isParticipant;
            }

            set
            {
                _isParticipant = value;
            }
        }

        [DataObjectProperty("StaffSubscribe", SqlDbType.Bit)]
        public bool StaffSubscribe
        {
            get
            {
                return _isStaffSubscribe;
            }

            set
            {
                _isStaffSubscribe = value;
            }
        }

        [DataObjectProperty("Alerts", SqlDbType.Bit)]
        public bool Alerts
        {
            get
            {
                return _isAlerts;
            }

            set
            {
                _isAlerts = value;
            }
        }

        [DataObjectProperty("Primary", SqlDbType.Bit)]
        public bool Primary
        {
            get
            {
                return _isPrimary;
            }

            set
            {
                _isPrimary = value;
            }
        }

        [DataObjectProperty("Alternate", SqlDbType.Bit)]
        public bool Alternate
        {
            get
            {
                return _isAlternate;
            }

            set
            {
                _isAlternate = value;
            }
        }

        [DataObjectProperty("Chair", SqlDbType.Bit)]
        public bool Chair
        {
            get
            {
                return _isChair;
            }

            set
            {
                _isChair = value;
            }
        }

        [DataObjectProperty("ViceChair", SqlDbType.Bit)]
        public bool ViceChair
        {
            get
            {
                return _isViceChair;
            }

            set
            {
                _isViceChair = value;
            }
        }

        [DataObjectProperty("Informational", SqlDbType.Bit)]
        public bool Informational
        {
            get
            {
                return _isInformational;
            }

            set
            {
                _isInformational = value;
            }
        }

        [DataObjectProperty("Spokesperson", SqlDbType.Bit)]
        public bool Spokesperson
        {
            get
            {
                return _isSpokesperson;
            }

            set
            {
                _isSpokesperson = value;
            }
        }

        [DataObjectProperty("GroupLeader", SqlDbType.Bit)]
        public bool GroupLeader
        {
            get
            {
                return _isGroupLeader;
            }

            set
            {
                _isGroupLeader = value;
            }
        }

        [DataObjectProperty("Secretary", SqlDbType.Bit)]
        public bool Secretary
        {
            get
            {
                return _isSecretary;
            }

            set
            {
                _isSecretary = value;
            }
        }



        [DataObjectProperty("FAAChair", SqlDbType.Bit)]
        public bool FAAChair
        {
            get
            {
                return _isFAAChair;
            }

            set
            {
                _isFAAChair = value;
            }
        }

        [DataObjectProperty("FAACoChair", SqlDbType.Bit)]
        public bool FAACoChair
        {
            get
            {
                return _isFAACoChair;
            }

            set
            {
                _isFAACoChair = value;
            }
        }

        [DataObjectProperty("TeamLeader", SqlDbType.Bit)]
        public bool TeamLeader
        {
            get
            {
                return _isTeamLeader;
            }

            set
            {
                _isTeamLeader = value;
            }
        }

        [DataObjectProperty("Observer", SqlDbType.Bit)]
        public bool Observer
        {
            get
            {
                return _isObserver;
            }

            set
            {
                _isObserver = value;
            }
        }

        [DataObjectProperty("Administrator", SqlDbType.Bit)]
        public bool Administrator
        {
            get
            {
                return _isAdministrator;
            }

            set
            {
                _isAdministrator = value;
            }
        }

        [DataObjectProperty("CoChair", SqlDbType.Bit)]
        public bool CoChair
        {
            get
            {
                return _isCoChair;
            }

            set
            {
                _isCoChair = value;
            }
        }

        [DataObjectProperty("CompanyName", SqlDbType.NVarChar)]
        public string CompanyName
        {
            get
            {
                return _companyName;
            }

            set
            {
                _companyName = value;
            }
        }

        [DataObjectProperty("Type", SqlDbType.Int, true)]
        public string Type
        {
            get
            {
                return _Type;
            }

            set
            {
                _Type = value;
            }
        }
        #endregion

        #region Constructor
        public UserGroupJsonModel()
        {
        }

        public UserGroupJsonModel(int GroupId)
        {
            this.Load(GroupId);
        }

        #endregion

        private const string GetA4AUserGroupJsonModel = "p_UserGroupJsonModel_Load";
        public static List<UserGroupJsonModel> GetA4AStaffGroups(int GroupId,bool A4AStaff)
        {
            SqlParameter[] sqlParams = new SqlParameter[2];
            sqlParams[0] = new SqlParameter("@GroupId", GroupId);
            sqlParams[1] = new SqlParameter("@A4AStaff", A4AStaff);
            DataObjectList<UserGroupJsonModel> groups = new DataObjectList<UserGroupJsonModel>(sqlParams, GetA4AUserGroupJsonModel);
            return (groups as List<UserGroupJsonModel>);
        }

        public static List<UserGroupJsonModel> GetGroupMembers(int GroupId)
        {
            SqlParameter[] sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter("@GroupId", GroupId); 
            DataObjectList<UserGroupJsonModel> groups = new DataObjectList<UserGroupJsonModel>(sqlParams, GetA4AUserGroupJsonModel);
            return (groups as List<UserGroupJsonModel>);
        }
        public static bool DeleteGroupsUser(UserGroupJsonModel ug)
        { 
            try  
            {
                using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("p_UserGroupJsonModel_Delete", con))
                    {
                        con.Open();
                        cmd.CommandType = CommandType.StoredProcedure; 
                        cmd.Parameters.Add(new SqlParameter("@GroupId", ug.GroupId));
                        cmd.Parameters.Add(new SqlParameter("@UserId", ug.UserId));
                        cmd.Parameters.Add(new SqlParameter("@Primary", ug.Primary));
                        cmd.Parameters.Add(new SqlParameter("@Alternate", ug.Alternate));
                        cmd.Parameters.Add(new SqlParameter("@Chair", ug.Chair));
                        cmd.Parameters.Add(new SqlParameter("@ViceChair", ug.ViceChair));
                        cmd.Parameters.Add(new SqlParameter("@Informational", ug.Informational));
                        cmd.Parameters.Add(new SqlParameter("@Spokesperson", ug.Spokesperson));
                        cmd.Parameters.Add(new SqlParameter("@GroupLeader", ug.GroupLeader));
                        cmd.Parameters.Add(new SqlParameter("@Secretary", ug.Secretary));
                        cmd.Parameters.Add(new SqlParameter("@ManageGroup", ug.ManageGroup));
                        cmd.Parameters.Add(new SqlParameter("@EmailAdmin", ug.EmailAdmin));
                        cmd.Parameters.Add(new SqlParameter("@BounceReports", ug.BounceReports));
                        cmd.Parameters.Add(new SqlParameter("@Participant", ug.Participant));
                        cmd.Parameters.Add(new SqlParameter("@Alerts", ug.Alerts));
                        cmd.Parameters.Add(new SqlParameter("@StaffSubscribe", ug.StaffSubscribe));
                        cmd.Parameters.Add(new SqlParameter("@CoChair", ug.CoChair));
                        cmd.Parameters.Add(new SqlParameter("@FAAChair", ug.FAAChair));
                        cmd.Parameters.Add(new SqlParameter("@FAACoChair", ug.FAACoChair));
                        cmd.Parameters.Add(new SqlParameter("@TeamLeader", ug.TeamLeader));
                        cmd.Parameters.Add(new SqlParameter("@Observer", ug.Observer));
                        cmd.Parameters.Add(new SqlParameter("@Administrator", ug.Administrator));
                        //cmd.ExecuteNonQuery();
                        int count = cmd.ExecuteNonQuery(); //Added on 2/25/2020 by NA to make sure delete worked properly 
                        return count > 0 ? true : false;
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }  
        }

        public static int UserExistsinGroup(UserGroupJsonModel ug)
        {  
            int c = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("p_UserGroup_IsUserinGroup", con))
                    {
                        con.Open();
                        cmd.CommandType = CommandType.StoredProcedure; 
                        cmd.Parameters.Add(new SqlParameter("@GroupId", ug.GroupId));
                        cmd.Parameters.Add(new SqlParameter("@UserId", ug.UserId)); 

                        SqlParameter sqlParams = new SqlParameter("@Count", SqlDbType.Int);
                        sqlParams.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(sqlParams);

                        cmd.ExecuteNonQuery();
                        c = Convert.ToInt32(sqlParams.Value);
                    }
                }
              
            }
            catch (Exception ex)
            { throw ex; }
           
            return c;
        }

        public static int CompanyExistsinRoleinGroup(UserGroupJsonModel ug, int RoleId)
        { 
            int c = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("p_UserGroup_CompanyExistsinRole", con))
                    {
                        con.Open();
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new SqlParameter("@GroupId", ug.GroupId));
                        cmd.Parameters.Add(new SqlParameter("@CompanyName", ug.CompanyName));
                        cmd.Parameters.Add(new SqlParameter("@RoleId", RoleId));
                        SqlParameter sqlParams = new SqlParameter("@Count", SqlDbType.Int);
                        sqlParams.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(sqlParams); 
                        
                        cmd.ExecuteNonQuery();
                        c = Convert.ToInt32(sqlParams.Value);
                    }
                } 
            }
            catch(Exception ex)
            {
                throw ex;
            } 
            return c;
        }

        public static int CompanyExistsinRoleinGroup(int GroupId, string CompanyName, int RoleId)
        {
            int c = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("p_UserGroup_CompanyExistsinRole", con))
                    {
                        con.Open();
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new SqlParameter("@GroupId",GroupId));
                        cmd.Parameters.Add(new SqlParameter("@CompanyName", CompanyName));
                        cmd.Parameters.Add(new SqlParameter("@RoleId", RoleId));
                        SqlParameter sqlParams = new SqlParameter("@Count", SqlDbType.Int);
                        sqlParams.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(sqlParams);

                        cmd.ExecuteNonQuery();
                        c = Convert.ToInt32(sqlParams.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return c;
        }

        public static bool DeleteGroupUserbyCompany(UserGroupJsonModel ug,int roleid)
        { 
            try
            {
                using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("p_UserGroup_delete", con))
                    {
                        con.Open();
                        cmd.CommandType = CommandType.StoredProcedure; 
                        cmd.Parameters.Add(new SqlParameter("@GroupId", ug.GroupId));
                        cmd.Parameters.Add(new SqlParameter("@CommitteeRoleId", roleid));
                        cmd.Parameters.Add(new SqlParameter("@CompanyName", ug.CompanyName));
                        // cmd.ExecuteNonQuery();
                        int count = cmd.ExecuteNonQuery(); //Added on 2/25/2020 by NA to make sure delete worked properly 
                        return count > 0 ? true : false;
                    }
                } 
            }
            catch (Exception ex)
            {
                throw ex;
            }  
        }
        //For deleteing Chair/ViceChair before creating them.
        public static bool DeleteGroupUserbyRole(int GroupId, int roleid)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("p_UserGroup_delete", con))
                    {
                        con.Open();
                        cmd.CommandType = CommandType.StoredProcedure; 
                        cmd.Parameters.Add(new SqlParameter("@GroupId", GroupId));
                        cmd.Parameters.Add(new SqlParameter("@CommitteeRoleId", roleid));
                        int count = cmd.ExecuteNonQuery(); //Added on 2/25/2020 by NA to make sure delete worked properly 
                        return count > 0 ? true : false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }  
        }
        
        public static bool DeleteGroupUserbyCompany(string CompanyName, int GroupId, int RoleId)
        {  
            try
            {
                using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("p_UserGroup_delete", con))
                    {
                        con.Open();
                        cmd.CommandType = CommandType.StoredProcedure; 
                        cmd.Parameters.Add(new SqlParameter("@GroupId", GroupId));
                        cmd.Parameters.Add(new SqlParameter("@CommitteeRoleId", RoleId));
                        cmd.Parameters.Add(new SqlParameter("@CompanyName", CompanyName));
                        //cmd.ExecuteNonQuery();

                        int count = cmd.ExecuteNonQuery(); //Added on 2/25/2020 by NA to make sure delete worked properly

                        return count > 0 ? true : false;
                    }
                } 
            }
            catch (Exception ex)
            { 
                throw ex;
            } 
        }

        public static bool DeleteGroupUserbyUserID(int GroupId, int UserId,int RoleId)
        { 
           try
            {
                using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("p_UserGroup_delete", con))
                    {
                        con.Open();
                        cmd.CommandType = CommandType.StoredProcedure; 
                        cmd.Parameters.Add(new SqlParameter("@GroupId", GroupId));
                        cmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                        cmd.Parameters.Add(new SqlParameter("@CommitteeRoleId", RoleId));
                        int count = cmd.ExecuteNonQuery(); //Added on 2/25/2020 by NA to make sure delete worked properly 
                        return count > 0 ? true : false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            } 
            return true;
        }

        public static bool DeleteGroupA4AUser(int GroupId, int UserId, int RoleId)
        {         
            try
            {
                using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("p_UserGroup_delete", con))
                    {
                        con.Open();
                        cmd.CommandType = CommandType.StoredProcedure;  
                        cmd.Parameters.Add(new SqlParameter("@GroupId", GroupId));
                        cmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                        cmd.Parameters.Add(new SqlParameter("@CommitteeRoleId", RoleId));
                        int count = cmd.ExecuteNonQuery(); //Added on 2/25/2020 by NA to make sure delete worked properly 
                        return count > 0 ? true : false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            } 
            return true;
        }

        public static int IsA4AStaffExistinGroupsbyRole(int GroupId, int UserId,int RoleId)
        {
            int c = 0; 
            try
            {
                using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("p_UserGroup_IsUserinGroup", con))
                    { 

                        con.Open();
                        cmd.CommandType = CommandType.StoredProcedure; 
                        cmd.Parameters.Add(new SqlParameter("@GroupId", GroupId));
                        cmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                        cmd.Parameters.Add(new SqlParameter("@CommitteeRoleId", RoleId));
                        SqlParameter sqlParams = new SqlParameter("@Count", SqlDbType.Int);
                        sqlParams.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(sqlParams);
                        cmd.ExecuteNonQuery(); 
                        c = Convert.ToInt32(sqlParams.Value);  
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            } 
            return c;
        }

        public static bool AddA4AStafftoGroupsbyRole(int GroupId, int UserId, int RoleId)
        { 
            try
            {
                using (SqlConnection con = new SqlConnection(Conf.ConnectionString))
                { 
                    using (SqlCommand cmd = new SqlCommand("p_UserGroup_SavebyRole", con))
                    {
                        con.Open();
                        cmd.CommandType = CommandType.StoredProcedure; 
                        cmd.Parameters.Add(new SqlParameter("@GroupId", GroupId));
                        cmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                        cmd.Parameters.Add(new SqlParameter("@RoleId", RoleId)); 
                        cmd.ExecuteNonQuery();
                    }
                }    }
            catch (Exception ex)
            {
                throw ex;
            } 
            return true;
        } 
    }
}
