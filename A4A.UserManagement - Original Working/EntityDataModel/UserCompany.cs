//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KnockoutMVC.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserCompany
    {
        public int UserId { get; set; }
        public int CompanyId { get; set; }
        public string Responsibilities { get; set; }
    
        public virtual Company Company { get; set; }
        public virtual JobTitle JobTitle { get; set; }
        public virtual User User { get; set; }
    }
}
