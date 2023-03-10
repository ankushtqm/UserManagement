#region Namespaces

using System.Diagnostics;
using Microsoft.VisualBasic;
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Data.SqlClient;
using System.Text;
using System.Security.Principal;
using System.Xml.Serialization;
using System.Reflection;
using SusQtech.ActiveDirectoryServices;

#endregion

namespace SusQtech.ActiveDirectoryServices
{
    [Serializable()]
    public class ADGroup
    {
        #region Private Property Variables

        private string _Name; //(cn ) in Active Directory
        private string _DisplayName;
        private string _DistinguishedName;
        private string _Description;
        private ArrayList _Users;
        private Dictionary<string, ADUser> _AllUsers;
        private string _ldapPath;

        #endregion

        #region Public Properties

        /// <summary></summary>
        public string LDAPPath
        {
            get { return _ldapPath; }
            set { _ldapPath = value; }
        }

        /// <summary></summary>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        /// <summary></summary>
        public string DisplayName
        {
            get { return _DisplayName; }
            set { _DisplayName = value; }
        }

        /// <summary></summary>
        public string DistinguishedName
        {
            get { return _DistinguishedName; }
            set { _DistinguishedName = value; }
        }

        /// <summary></summary>
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        /// <summary></summary>
        public ArrayList Users
        {
            get
            {
                if (_Users == null)
                    _Users = ADUser.LoadByGroup(DistinguishedName);

                return _Users;
            }
            set { _Users = value; }
        }

        /// <summary>gets all users of this group recursively, no groups will be returned as part of this just ADUser objects</summary>
        public Dictionary<string, ADUser> AllUsers
        {
            get
            {
                if (_AllUsers == null)
                    _AllUsers = ADUser.LoadAllByGroup(DistinguishedName);

                return _AllUsers;
            }
            set { _AllUsers = value; }
        }

        #endregion

        #region Friend Functions

        /// <summary></summary>
        /// <param name="DistinguishedName"></param>
        /// <returns></returns>
        internal static ArrayList LoadByUser(string DistinguishedName)
        {
            return GetGroups(DistinguishedName);
        }

        #endregion

        #region Public Functions

        /// <summary></summary>
        public void Update()
        {
            try
            {
                DirectoryEntry de = Utility.GetGroup(Name);
                Utility.SetProperty(de, "DisplayName", DisplayName);
                Utility.SetProperty(de, "Description", Description);
                de.CommitChanges();
            }
            catch (Exception ex)
            {
                throw (new Exception("User cannot be updated" + ex.Message));
            }
        }

        #endregion

        #region Private Functions

        /// <summary></summary>
        /// <param name="DistinguishedName"></param>
        /// <returns></returns>
        private static ArrayList GetGroups(string DistinguishedName)
        {
            DirectoryEntry _de = Utility.GetDirectoryObjectByDistinguishedName(DistinguishedName);
            ArrayList list = new ArrayList();

            for (int index = 0; index <= _de.Properties["memberOf"].Count - 1; index++)
                list.Add(Load(Utility.GetDirectoryObjectByDistinguishedName(Utility.ADPath + "/" + _de.Properties["memberOf"][index].ToString())));

            return list;
        }

        /// <summary></summary>
        /// <param name="de"></param>
        /// <returns></returns>
        private static ADGroup Load(DirectoryEntry de)
        {
            ADGroup _ADGroup = new ADGroup();
            _ADGroup.Name = Utility.GetProperty(de, "cn");
            _ADGroup.DisplayName = Utility.GetProperty(de, "DisplayName");
            _ADGroup.DistinguishedName = Utility.ADPath + "/" + Utility.GetProperty(de, "DistinguishedName");
            _ADGroup.Description = Utility.GetProperty(de, "Description");
            return _ADGroup;
        }

        /// <summary></summary>
        /// <param name="deCollection"></param>
        /// <returns></returns>
        private ArrayList Load(DirectoryEntries deCollection)
        {
            ArrayList list = new ArrayList();
            DirectoryEntry de;

            foreach (DirectoryEntry tempLoopVar_de in deCollection)
            {
                de = tempLoopVar_de;
                list.Add(Load(de));
            }

            return list;
        }

        #endregion
    }
}