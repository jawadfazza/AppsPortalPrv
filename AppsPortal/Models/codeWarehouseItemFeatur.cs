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
    
    public partial class codeWarehouseItemFeatur
    {
        public System.Guid WarehouseItemFeaturGUID { get; set; }
        public Nullable<System.Guid> WarehouseItemGUID { get; set; }
        public Nullable<System.Guid> WarehouseGeneralFeatureGUID { get; set; }
        public bool Active { get; set; }
        public byte[] codeItemFeaturRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<bool> Archived { get; set; }
    
        public virtual codeWarehouseGeneralFeature codeWarehouseGeneralFeature { get; set; }
        public virtual codeWarehouseItem codeWarehouseItem { get; set; }
    }
}
