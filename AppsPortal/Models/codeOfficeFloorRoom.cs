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
    
    public partial class codeOfficeFloorRoom
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public codeOfficeFloorRoom()
        {
            this.StaffCoreDataHistory = new HashSet<StaffCoreDataHistory>();
        }
    
        public System.Guid OfficeFloorRoomGUID { get; set; }
        public System.Guid OfficeFloorGUID { get; set; }
        public string RoomNumber { get; set; }
        public bool Active { get; set; }
        public byte[] codeOfficeFloorRoomRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<double> RoomSize { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StaffCoreDataHistory> StaffCoreDataHistory { get; set; }
        public virtual codeOfficeFloor codeOfficeFloor { get; set; }
    }
}
