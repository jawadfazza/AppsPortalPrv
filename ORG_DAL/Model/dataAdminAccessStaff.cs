//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ORG_DAL.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class dataAdminAccessStaff
    {
        public System.Guid AdminAccessStaffGUID { get; set; }
        public Nullable<System.Guid> UserGUID { get; set; }
        public string EmailAddress { get; set; }
        public string FullName { get; set; }
        public bool Active { get; set; }
        public byte[] dataAdminAccessStaffRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<bool> Archived { get; set; }
    
        public virtual StaffCoreData StaffCoreData { get; set; }
    }
}
