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
    
    public partial class codeDASDocumentCabinetShelf
    {
        public System.Guid DocumentCabinetShelfGUID { get; set; }
        public Nullable<System.Guid> DocumentCabinetGUID { get; set; }
        public string ShelfNumber { get; set; }
        public Nullable<int> MaxStorage { get; set; }
        public bool Active { get; set; }
        public byte[] codeDASDocumentCabinetShelfRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<bool> Archived { get; set; }
    
        public virtual codeDASDocumentCabinet codeDASDocumentCabinet { get; set; }
    }
}
