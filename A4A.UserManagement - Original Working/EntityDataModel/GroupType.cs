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
    
    public partial class GroupType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public GroupType()
        {
            this.Groups = new HashSet<Group>();
        }
    
        public int GroupTypeId { get; set; }
        public string GroupTypeName { get; set; }
        public Nullable<bool> IsCommitteeCategory { get; set; }
        public Nullable<bool> IsSecurityCategory { get; set; }
        public Nullable<bool> IsDistributionCategory { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Group> Groups { get; set; }
    }
}
