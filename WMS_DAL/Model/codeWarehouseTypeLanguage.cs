//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WMS_DAL.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class codeWarehouseTypeLanguage
    {
        public System.Guid WarehouseTypeLanguageGUID { get; set; }
        public Nullable<System.Guid> WarehouseTypeGUID { get; set; }
        public string LanguageID { get; set; }
        public string WarehouseTypeDescription { get; set; }
        public bool Active { get; set; }
        public byte[] codeWarehouseTypeLanguageRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<bool> Archived { get; set; }
    
        public virtual codeWarehouseType codeWarehouseType { get; set; }
    }
}
