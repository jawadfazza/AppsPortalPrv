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
    
    public partial class codeCountries
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public codeCountries()
        {
            this.StaffCoreDataHistory = new HashSet<StaffCoreDataHistory>();
            this.StaffCoreDataHistory1 = new HashSet<StaffCoreDataHistory>();
            this.StaffCoreDataHistory2 = new HashSet<StaffCoreDataHistory>();
            this.StaffCoreDataHistory3 = new HashSet<StaffCoreDataHistory>();
            this.codeCountriesLanguages = new HashSet<codeCountriesLanguages>();
        }
    
        public System.Guid CountryGUID { get; set; }
        public string CountryA2Code { get; set; }
        public string CountryA3Code { get; set; }
        public string PhoneCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool Active { get; set; }
        public byte[] codeCountriesRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public string CountryNameCIMT { get; set; }
        public string CountryA3CodeCIMT { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StaffCoreDataHistory> StaffCoreDataHistory { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StaffCoreDataHistory> StaffCoreDataHistory1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StaffCoreDataHistory> StaffCoreDataHistory2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StaffCoreDataHistory> StaffCoreDataHistory3 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<codeCountriesLanguages> codeCountriesLanguages { get; set; }
    }
}
