using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATA.CodeLibrary
{
    public class GroupStaffUserRoles
    {
        public int GroupId { get; set; }
        public int UserId { get; set; }
        public bool Value { get; set; }
        public int RoleId { get; set; }
    }
}