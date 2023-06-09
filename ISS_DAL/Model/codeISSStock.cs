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
    
    public partial class codeISSStock
    {
        public codeISSStock()
        {
            this.codeISSStockLanguage = new HashSet<codeISSStockLanguage>();
            this.dataItemPipeline = new HashSet<dataItemPipeline>();
            this.dataItemStockBalance = new HashSet<dataItemStockBalance>();
            this.codeISSStock1 = new HashSet<codeISSStock>();
        }
    
        public System.Guid StockGUID { get; set; }
        public Nullable<System.Guid> OrganizationInstanceGUID { get; set; }
        public Nullable<System.Guid> DutyStationGUID { get; set; }
        public Nullable<System.Guid> LocationGUID { get; set; }
        public Nullable<System.Guid> ParentGUID { get; set; }
        public bool Active { get; set; }
        public byte[] codeISSStockRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<bool> Archived { get; set; }
        public Nullable<int> OrderId { get; set; }
    
        public virtual ICollection<codeISSStockLanguage> codeISSStockLanguage { get; set; }
        public virtual ICollection<dataItemPipeline> dataItemPipeline { get; set; }
        public virtual ICollection<dataItemStockBalance> dataItemStockBalance { get; set; }
        public virtual ICollection<codeISSStock> codeISSStock1 { get; set; }
        public virtual codeISSStock codeISSStock2 { get; set; }
    }
}
