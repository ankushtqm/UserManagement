#region Namespaces

using System.Diagnostics;
using Microsoft.VisualBasic;
using System;
using System.Data;
using System.Collections;
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
    public class ADContainer
    {
        #region Private Property Variables

        private string _Name;
        private string _ldapPath;
        private string _distinguishedName;
        private string _ou;
        private string _instanceType;
        private string _objectGUID;
        private string _objectCategory;

        #endregion

        #region Public Properties

        public string DistinguishedName
        {
            get { return _distinguishedName; }
            set { _distinguishedName = value; }
        }

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
        public string OU
        {
            get { return _ou; }
            set { _ou = value; }
        }

        public string InstanceType
        {
            get { return _instanceType; }
            set { _instanceType = value; }
        }

        public string ObjectGUID
        {
            get { return _objectGUID; }
            set { _objectGUID = value; }
        }

        public string ObjectCategory
        {
            get { return _objectCategory; }
            set { _objectCategory = value; }
        }

        #endregion

        #region Private Functions

        /// <summary></summary>
        /// <param name="de"></param>
        /// <returns></returns>
        private static ADContainer Load(DirectoryEntry de)
        {
            ADContainer _ADNode = new ADContainer();
            _ADNode.Name = Utility.GetProperty(de, "name");
            _ADNode.LDAPPath = de.Path;
            _ADNode.DistinguishedName = Utility.ADPath + "/" + Utility.GetProperty(de, "distinguishedName");
            _ADNode.OU = Utility.GetProperty(de, "ou");
            _ADNode.InstanceType = Utility.GetProperty(de, "instanceType");
            _ADNode.ObjectCategory = Utility.GetProperty(de, "objectCategory");
            _ADNode.ObjectGUID = Utility.GetProperty(de, "objectGUID");
            return _ADNode;
        }

        /// <summary></summary>
        /// <param name="deCollection"></param>
        /// <returns></returns>
        private ArrayList Load(DirectoryEntries deCollection)
        {
            ArrayList list = new ArrayList();

            foreach (DirectoryEntry de in deCollection)
            {
                list.Add(Load(de));
            }

            return list;
        }

        #endregion
    }
}