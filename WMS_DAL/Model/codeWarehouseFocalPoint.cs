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
    
    public partial class codeWarehouseFocalPoint
    {
        public System.Guid WarehouseFocalPointGUID { get; set; }
        public Nullable<System.Guid> WarehouseGUID { get; set; }
        public Nullable<System.Guid> UserGUID { get; set; }
        public Nullable<System.DateTime> StartWorkingDate { get; set; }
        public Nullable<System.DateTime> EndWorkingDate { get; set; }
        public bool Active { get; set; }
        public byte[] codeWarehouseFocalPointRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<bool> Archived { get; set; }
        public Nullable<bool> IsFocalPoint { get; set; }
    
        public virtual userAccounts userAccounts { get; set; }
        public virtual codeWarehouse codeWarehouse { get; set; }
    }
}
