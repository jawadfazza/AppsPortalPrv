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
    
    public partial class userProfiles
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public userProfiles()
        {
            this.dataAuditActions = new HashSet<dataAuditActions>();
            this.dataAuditLogins = new HashSet<dataAuditLogins>();
            this.userApplicationToken = new HashSet<userApplicationToken>();
            this.userManagersHistory = new HashSet<userManagersHistory>();
            this.userStepsHistory = new HashSet<userStepsHistory>();
        }
    
        public System.Guid UserProfileGUID { get; set; }
        public Nullable<bool> ProfileConfirmed { get; set; }
        public System.Guid ServiceHistoryGUID { get; set; }
        public System.Guid OrganizationInstanceGUID { get; set; }
        public Nullable<System.Guid> DutyStationGUID { get; set; }
        public Nullable<System.Guid> DepartmentGUID { get; set; }
        public Nullable<System.Guid> SiteCategoryGUID { get; set; }
        public string PositionNumber { get; set; }
        public Nullable<System.Guid> JobTitleGUID { get; set; }
        public string Grade { get; set; }
        public System.DateTime FromDate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
        public bool Active { get; set; }
        public byte[] userProfilesRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataAuditActions> dataAuditActions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataAuditLogins> dataAuditLogins { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<userApplicationToken> userApplicationToken { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<userManagersHistory> userManagersHistory { get; set; }
        public virtual codeDutyStations codeDutyStations { get; set; }
        public virtual codeJobTitles codeJobTitles { get; set; }
        public virtual userServiceHistory userServiceHistory { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<userStepsHistory> userStepsHistory { get; set; }
        public virtual codeDepartments codeDepartments { get; set; }
    }
}