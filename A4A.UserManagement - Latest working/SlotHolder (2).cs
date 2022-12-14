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
    
    public partial class SlotHolder
    {
        public int ID { get; set; }
        public string SlotNo { get; set; }
        public int AirportId { get; set; }
        public int SlotTime { get; set; }
        public string ArriveDeparture { get; set; }
        public System.DateTime EffectiveDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public int Frequency { get; set; }
        public string CarrierMo { get; set; }
        public string CarrierTu { get; set; }
        public string CarrierWe { get; set; }
        public string CarrierTh { get; set; }
        public string CarrierFr { get; set; }
        public string CarrierSa { get; set; }
        public string CarrierSu { get; set; }
        public int CompanyId { get; set; }
        public string CreationTransactionNo { get; set; }
        public string TransactionNo { get; set; }
        public string TransactionRefNo { get; set; }
        public string Concurrence { get; set; }
        public string Notes { get; set; }
        public Nullable<System.DateTime> FAAApprovalDate { get; set; }
        public int StatusCodeId { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> Modified { get; set; }
        public string ModifiedBy { get; set; }
    
        public virtual Airport Airport { get; set; }
        public virtual Company Company { get; set; }
        public virtual StatusCode StatusCode { get; set; }
    }
}
