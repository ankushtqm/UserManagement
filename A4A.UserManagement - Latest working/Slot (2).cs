//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace A4A.UM
{
    using System;
    using System.Collections.Generic;
    
    public partial class Slot
    {
        public int SlotId { get; set; }
        public string SlotNo { get; set; }
        public int AirportId { get; set; }
        public Nullable<int> SlotTime { get; set; }
        public string ArriveDeparture { get; set; }
        public System.DateTime EffectiveDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> Modified { get; set; }
        public string ModifiedBy { get; set; }
    
        public virtual Airport Airport { get; set; }
    }
}
