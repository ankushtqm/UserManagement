﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ATA_SLOTSEntities : DbContext
    {
        public ATA_SLOTSEntities()
            : base("name=ATA_SLOTSEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Airport> Airports { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<DailySlotHolderOperator> DailySlotHolderOperators { get; set; }
        public virtual DbSet<NextUniqueIdHolder> NextUniqueIdHolders { get; set; }
        public virtual DbSet<Slot> Slots { get; set; }
        public virtual DbSet<SlotHolder> SlotHolders { get; set; }
        public virtual DbSet<SlotOperator> SlotOperators { get; set; }
        public virtual DbSet<SlotTransaction> SlotTransactions { get; set; }
        public virtual DbSet<StatusCode> StatusCodes { get; set; }
        public virtual DbSet<DirtySlotHolderOperator> DirtySlotHolderOperators { get; set; }
    }
}
