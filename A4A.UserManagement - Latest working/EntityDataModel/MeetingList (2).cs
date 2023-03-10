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
    
    public partial class MeetingList
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MeetingList()
        {
            this.MeetingListSourceUsers = new HashSet<MeetingListSourceUser>();
            this.MeetingListUsers = new HashSet<MeetingListUser>();
        }
    
        public int MeetingListId { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public string MeetingName { get; set; }
        public string MeetingLocation { get; set; }
        public string MeetingPurpose { get; set; }
        public Nullable<int> MeetingGroupId { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<System.DateTime> Modified { get; set; }
    
        public virtual Group Group { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
        public virtual MeetingType MeetingType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MeetingListSourceUser> MeetingListSourceUsers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MeetingListUser> MeetingListUsers { get; set; }
    }
}
