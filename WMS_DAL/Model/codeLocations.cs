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
    
    public partial class codeLocations
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public codeLocations()
        {
            this.codeLocationsLanguages = new HashSet<codeLocationsLanguages>();
            this.dataItemOutputDetail = new HashSet<dataItemOutputDetail>();
        }
    
        public System.Guid LocationGUID { get; set; }
        public System.Guid LocationTypeGUID { get; set; }
        public System.Guid CountryGUID { get; set; }
        public Nullable<System.Guid> LocationParentGUID { get; set; }
        public string LocationPCode { get; set; }
        public int LocationlevelID { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string LocationPhoneCode { get; set; }
        public bool Active { get; set; }
        public byte[] codeLocationsRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<codeLocationsLanguages> codeLocationsLanguages { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataItemOutputDetail> dataItemOutputDetail { get; set; }
    }
}
