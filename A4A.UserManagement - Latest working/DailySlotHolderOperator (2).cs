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
    
    public partial class DailySlotHolderOperator
    {
        public int DailySlotHolderOperatorId { get; set; }
        public string SlotNo { get; set; }
        public int AirportId { get; set; }
        public int SlotTime { get; set; }
        public string ArriveDeparture { get; set; }
        public System.DateTime CurrentDate { get; set; }
        public Nullable<int> HolderId { get; set; }
        public Nullable<int> OperatorId { get; set; }
    
        public virtual Airport Airport { get; set; }
        public virtual Company Company { get; set; }
        public virtual Company Company1 { get; set; }
    }
}
