//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ISS_DAL.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class codeISSItem
    {
        public codeISSItem()
        {
            this.codeISSItemLanguage = new HashSet<codeISSItemLanguage>();
            this.dataItemOverview = new HashSet<dataItemOverview>();
            this.dataItemPipeline = new HashSet<dataItemPipeline>();
            this.dataItemStockBalance = new HashSet<dataItemStockBalance>();
            this.dataItemStockEmergencyReserve = new HashSet<dataItemStockEmergencyReserve>();
        }
    
        public System.Guid ItemGUID { get; set; }
        public Nullable<int> SequencId { get; set; }
        public bool Active { get; set; }
        public byte[] codeISSItemRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<bool> Archived { get; set; }
    
        public virtual ICollection<codeISSItemLanguage> codeISSItemLanguage { get; set; }
        public virtual ICollection<dataItemOverview> dataItemOverview { get; set; }
        public virtual ICollection<dataItemPipeline> dataItemPipeline { get; set; }
        public virtual ICollection<dataItemStockBalance> dataItemStockBalance { get; set; }
        public virtual ICollection<dataItemStockEmergencyReserve> dataItemStockEmergencyReserve { get; set; }
    }
}