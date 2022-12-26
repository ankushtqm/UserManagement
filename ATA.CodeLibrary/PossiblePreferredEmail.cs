#region namespaces

using System;
using System.Collections.Generic;
using System.Data;
using System.ComponentModel;
using System.Text;
using SusQTech.Data.DataObjects;

#endregion

namespace ATA.ObjectModel
{
    public class PossiblePreferredEmail : DataObjectBase
    {
        #region private members

        private int _userId;
        private string _emailAddress;
        private bool _isPrimary;

        #endregion

        public PossiblePreferredEmail()
        {
        }

        #region public properties

        [DataObjectProperty("UserId", SqlDbType.Int)]
        public int UserId
        {
            get { return (this._userId); }
            set { this._userId = value; }
        }

        [DataObjectProperty("Email", SqlDbType.NVarChar, 255)]
        public string EmailAddress
        {
            get { return (this._emailAddress); }
            set { this._emailAddress = value; }
        }


        [TypeConverter(typeof(bool))]
        [DataObjectProperty("IsPrimary", SqlDbType.Bit)]
        public bool IsPrimary
        {
            get { return (this._isPrimary); }
            set { this._isPrimary = value; }
        }

        public string ValueForDropDown
        {
            get { return (PossiblePreferredEmail.GetValueForDropDown(this.UserId, this.IsPrimary)); }
        }

        #endregion

        #region methods to support compound values in drop down lists

        public static int GetUserIdFromDropDownValue(string value)
        {
            int i = DataObjectBase.NullIntRowId;

            if (string.IsNullOrEmpty(value))
                return (i);

            string[] values = value.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
            if (values.Length != 2)
                return(i);

            if (int.TryParse(values[0], out i))
                return(i);

            return(DataObjectBase.NullIntRowId);
        }

        public static bool GetIsPrimaryFromDropDownValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return (false);

            string[] values = value.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
            if (values.Length != 2)
                return (false);

            bool b = false;

            if (bool.TryParse(values[1], out b))
                return (b);

            return (false);
        }

        public static string GetValueForDropDown(int UserId, bool IsPrimary)
        {
            return (string.Format("{0}_{1}", UserId, IsPrimary.ToString()));
        }

        #endregion
    }
}
