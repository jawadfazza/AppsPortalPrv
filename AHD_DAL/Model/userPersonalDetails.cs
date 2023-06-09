
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AHD_DAL.Model
{
    
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using RES_Repo.Globalization;
    using System.ComponentModel.DataAnnotations.Schema;
    using Portal_BL.Library;
    
    public partial class userPersonalDetails
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public userPersonalDetails()
        {
            this.dataAHDInternationalStaffEntitlement = new HashSet<dataAHDInternationalStaffEntitlement>();
            this.dataAHDInternationalStaffEntitlementFlow = new HashSet<dataAHDInternationalStaffEntitlementFlow>();
            this.dataInternationalStaffAttendance = new HashSet<dataInternationalStaffAttendance>();
            this.dataInternationalStaffAttendance1 = new HashSet<dataInternationalStaffAttendance>();
            this.dataRestAndRecuperationRequest = new HashSet<dataRestAndRecuperationRequest>();
            this.dataRestAndRecuperationRequest1 = new HashSet<dataRestAndRecuperationRequest>();
            this.dataRestAndRecuperationRequest2 = new HashSet<dataRestAndRecuperationRequest>();
            this.dataRestAndRecuperationRequest3 = new HashSet<dataRestAndRecuperationRequest>();
            this.dataRestAndRecuperationRequest4 = new HashSet<dataRestAndRecuperationRequest>();
            this.dataRestAndRecuperationRequest5 = new HashSet<dataRestAndRecuperationRequest>();
            this.dataRestAndRecuperationRequest6 = new HashSet<dataRestAndRecuperationRequest>();
            this.dataRestAndRecuperationRequestLeaveFlow = new HashSet<dataRestAndRecuperationRequestLeaveFlow>();
            this.dataStaffEligibleForDangerPay = new HashSet<dataStaffEligibleForDangerPay>();
            this.dataStaffRenwalResidency = new HashSet<dataStaffRenwalResidency>();
            this.userPersonalDetailsLanguage = new HashSet<userPersonalDetailsLanguage>();
        }
    
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "UserGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid UserGUID{ get; set; }
    	
        [Display(Name = "CountryGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> CountryGUID{ get; set; }
    	
        [Display(Name = "GenderGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> GenderGUID{ get; set; }
    	
        [DataType(DataType.Date)]
    	[Display(Name = "DateOfBirth", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DateOfBirth{ get; set; }
    	
        [StringLength(5, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "PreferedLanguageID", ResourceType = typeof(resxDbFields))]
    	public string PreferedLanguageID{ get; set; }
    	
        [StringLength(5, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "BloodGroup", ResourceType = typeof(resxDbFields))]
    	public string BloodGroup{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public bool Active{ get; set; }
    	
        public byte[] userPersonalDetailsRowVersion{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }
    	
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataAHDInternationalStaffEntitlement> dataAHDInternationalStaffEntitlement { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataAHDInternationalStaffEntitlementFlow> dataAHDInternationalStaffEntitlementFlow { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataInternationalStaffAttendance> dataInternationalStaffAttendance { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataInternationalStaffAttendance> dataInternationalStaffAttendance1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataRestAndRecuperationRequest> dataRestAndRecuperationRequest { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataRestAndRecuperationRequest> dataRestAndRecuperationRequest1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataRestAndRecuperationRequest> dataRestAndRecuperationRequest2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataRestAndRecuperationRequest> dataRestAndRecuperationRequest3 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataRestAndRecuperationRequest> dataRestAndRecuperationRequest4 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataRestAndRecuperationRequest> dataRestAndRecuperationRequest5 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataRestAndRecuperationRequest> dataRestAndRecuperationRequest6 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataRestAndRecuperationRequestLeaveFlow> dataRestAndRecuperationRequestLeaveFlow { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataStaffEligibleForDangerPay> dataStaffEligibleForDangerPay { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataStaffRenwalResidency> dataStaffRenwalResidency { get; set; }
        public virtual userAccounts userAccounts { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<userPersonalDetailsLanguage> userPersonalDetailsLanguage { get; set; }
    }
}
