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
    
    public partial class dataItemOutputDetail
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public dataItemOutputDetail()
        {
            this.dataItemOutputDetailFlow = new HashSet<dataItemOutputDetailFlow>();
            this.dataItemOutputNotification = new HashSet<dataItemOutputNotification>();
            this.dataPendingVerficationReminder = new HashSet<dataPendingVerficationReminder>();
            this.dataItemOutputDetailDamagedTrack = new HashSet<dataItemOutputDetailDamagedTrack>();
        }
    
        public System.Guid ItemOutputDetailGUID { get; set; }
        public Nullable<System.Guid> ItemOutputGUID { get; set; }
        public Nullable<System.Guid> ItemRequestTypeGUID { get; set; }
        public Nullable<System.Guid> ItemInputDetailGUID { get; set; }
        public Nullable<int> RequestedQunatity { get; set; }
        public Nullable<System.DateTime> ExpectedStartDate { get; set; }
        public Nullable<System.DateTime> ExpectedReturenedDate { get; set; }
        public Nullable<System.DateTime> ActualReturenedDate { get; set; }
        public Nullable<bool> IsRecevied { get; set; }
        public string Comments { get; set; }
        public Nullable<bool> IsRommming { get; set; }
        public Nullable<int> PackageSize { get; set; }
        public Nullable<System.Guid> CreatedByGUID { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public bool Active { get; set; }
        public byte[] dataItemOutputDetailRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<bool> Archived { get; set; }
        public Nullable<System.Guid> LocationGUID { get; set; }
        public Nullable<System.Guid> ItemStatuOnReturnGUID { get; set; }
        public Nullable<System.Guid> RetunedByGUID { get; set; }
        public Nullable<System.DateTime> ReturnedDate { get; set; }
        public Nullable<System.Guid> WarehouseLocationGUID { get; set; }
        public Nullable<System.Guid> WarehouseSubLocationGUID { get; set; }
        public Nullable<System.Guid> WarehouseLocationOnReturnGUID { get; set; }
        public Nullable<bool> HasCharger { get; set; }
        public Nullable<bool> HasLaptopMouse { get; set; }
        public Nullable<bool> HasHeadPhone { get; set; }
        public Nullable<bool> HasBag { get; set; }
        public Nullable<bool> HasHeadsetUSB { get; set; }
        public Nullable<bool> HasHeadsetStereoJack { get; set; }
        public Nullable<bool> HasBackbag { get; set; }
        public Nullable<bool> HasHandbag { get; set; }
        public Nullable<bool> HasFlashMemory { get; set; }
        public Nullable<System.Guid> ItemTagGUID { get; set; }
        public Nullable<bool> HasRoaming { get; set; }
        public Nullable<bool> HasInternationalAccess { get; set; }
        public string SIMPackageSize { get; set; }
        public Nullable<bool> IsOfficialSIM { get; set; }
        public Nullable<System.Guid> SIMPackageSizeGUID { get; set; }
        public Nullable<System.DateTime> LicenseStartDate { get; set; }
        public Nullable<System.DateTime> LicenseExpiryDate { get; set; }
        public Nullable<System.Guid> ItemServiceStatusGUID { get; set; }
        public Nullable<System.Guid> ItemStatusGUID { get; set; }
    
        public virtual userAccounts userAccounts { get; set; }
        public virtual codeLocations codeLocations { get; set; }
        public virtual codeWarehouseLocation codeWarehouseLocation { get; set; }
        public virtual codeWarehouseLocation codeWarehouseLocation1 { get; set; }
        public virtual codeWarehouseSubLocation codeWarehouseSubLocation { get; set; }
        public virtual codeWMSItemTag codeWMSItemTag { get; set; }
        public virtual dataItemInputDetail dataItemInputDetail { get; set; }
        public virtual dataItemOutput dataItemOutput { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataItemOutputDetailFlow> dataItemOutputDetailFlow { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataItemOutputNotification> dataItemOutputNotification { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataPendingVerficationReminder> dataPendingVerficationReminder { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataItemOutputDetailDamagedTrack> dataItemOutputDetailDamagedTrack { get; set; }
    }
}