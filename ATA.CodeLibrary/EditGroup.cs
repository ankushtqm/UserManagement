//#region namespaces

//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Text;
//using SusQTech.Data.DataObjects;

//#endregion

//namespace ATA.ObjectModel
//{
//    public class EditGroup : DataObjectBase
//    {

////        ALTER  PROCEDURE [dbo].[p_User_GetAllEditGroups]
////@UserId int,
////@AppliesToSiteId int
////AS
////BEGIN


////    SELECT
////        g.GroupId,
////        g.GroupName,
////        isnull(LyrisShortDescription, GroupName) as LyrisShortDescription, 
////        g.IsOpenEnrollment, 
////        g.IsCommittee, 
////        isnull(g.IsSecurityGroup, 0) as IsSecurityGroup,  
////        isnull(ug.CommitteeRoleId, 0) as CommitteeRoleId, 
////        cast (isnull(ug.UserId, 0) as bit) as IsSelected
////    FROM
////        [Group] g
////    left join
////        [UserGroup] ug ON ug.GroupId = g.GroupId AND ug.UserId = @UserId
////    WHERE
////        g.AppliesToSiteId = @AppliesToSiteId AND g.IsChildGroup = 0 
////END  
////Go
        
        
//        #region private members

//        private int _groupId;
//        private string _groupName;
//        private string _lyrisShortDescription;
//        private bool _isOpenEnrollment; 
//        private int _committeeRoleId; 
//        private bool _isSelected;
//        private bool _isCommittee;
//        private bool _isSecurityGroup; 
//        #endregion

//        public EditGroup()
//        {
//        }

//        #region public properties

//        [DataObjectProperty("GroupId", SqlDbType.Int)]
//        public int GroupId
//        {
//            get { return (this._groupId); }
//            set { this._groupId = value; }
//        }

//        [DataObjectProperty("GroupName", SqlDbType.NVarChar, 50)]
//        public string GroupName
//        {
//            get { return (this._groupName); }
//            set { this._groupName = value; }
//        }

//        [DataObjectProperty("LyrisShortDescription", SqlDbType.NVarChar, 200)]
//        public string LyrisShortDescription
//        {
//            get { return (this._lyrisShortDescription); }
//            set { this._lyrisShortDescription = value; }
//        }

//        [DataObjectProperty("IsOpenEnrollment", SqlDbType.Bit)]
//        public bool IsOpenEnrollment
//        {
//            get { return (this._isOpenEnrollment); }
//            set { this._isOpenEnrollment = value; }
//        }

//        [DataObjectProperty("IsCommittee", SqlDbType.Bit)]
//        public bool IsCommittee
//        {
//            get { return (this._isCommittee); }
//            set { this._isCommittee = value; }
//        }

//        [DataObjectProperty("CommitteeRoleId", SqlDbType.Int)]
//        public int CommitteeRoleId
//        {
//            get { return (this._committeeRoleId); }
//            set { this._committeeRoleId = value; }
//        } 

//        [DataObjectProperty("IsSelected", SqlDbType.Bit)]
//        public bool IsSelected
//        {
//            get { return (this._isSelected); }
//            set { this._isSelected = value; }
//        }  

//        [DataObjectProperty("IsSecurityGroup", SqlDbType.Bit)]
//        public bool IsSecurityGroup
//        {
//            get { return (this._isSecurityGroup); }
//            set { this._isSecurityGroup = value; }
//        }

//        #endregion
//    }
//}
