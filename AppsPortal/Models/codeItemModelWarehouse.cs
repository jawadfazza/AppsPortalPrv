//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AppsPortal.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class codeItemModelWarehouse
    {
        public System.Guid ItemModelWarehouseGUID { get; set; }
        public Nullable<System.Guid> WarehouseItemModelGUID { get; set; }
        public Nullable<System.Guid> WarehouseGUID { get; set; }
        public Nullable<int> LowestAmountAllowed { get; set; }
        public Nullable<int> ReOrderedLimit { get; set; }
        public Nullable<bool> IsNeedApproval { get; set; }
        public bool Active { get; set; }
        public byte[] codeItemModelWarehouseRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<bool> Archived { get; set; }
        public Nullable<int> TotalAvaiable { get; set; }
        public Nullable<int> TotalEntry { get; set; }
        public Nullable<int> TotalRelease { get; set; }
        public Nullable<int> TotalPending { get; set; }
        public Nullable<System.Guid> FlowTypeGUID { get; set; }
    
        public virtual codeWarehouseItemModel codeWarehouseItemModel { get; set; }
        public virtual codeWarehouse codeWarehouse { get; set; }
    }
}
