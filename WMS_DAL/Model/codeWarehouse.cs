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
    
    public partial class codeWarehouse
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public codeWarehouse()
        {
            this.codeItemModelWarehouse = new HashSet<codeItemModelWarehouse>();
            this.codeWarehouse1 = new HashSet<codeWarehouse>();
            this.codeWarehouseFocalPoint = new HashSet<codeWarehouseFocalPoint>();
            this.dataWarehouseFocalPoint = new HashSet<dataWarehouseFocalPoint>();
            this.codeWarehouseLanguage = new HashSet<codeWarehouseLanguage>();
            this.codeWarehouseStore = new HashSet<codeWarehouseStore>();
            this.dataItemInput = new HashSet<dataItemInput>();
            this.dataItemInputDetail = new HashSet<dataItemInputDetail>();
            this.dataItemOutput = new HashSet<dataItemOutput>();
            this.dataItemTransfer = new HashSet<dataItemTransfer>();
            this.dataItemTransfer1 = new HashSet<dataItemTransfer>();
            this.dataItemVerificationWarehousePeriod = new HashSet<dataItemVerificationWarehousePeriod>();
        }
    
        public System.Guid WarehouseGUID { get; set; }
        public Nullable<System.Guid> OrganizationInstanceGUID { get; set; }
        public Nullable<System.Guid> DutyStationGUID { get; set; }
        public Nullable<System.Guid> LocationGUID { get; set; }
        public Nullable<System.Guid> ParentGUID { get; set; }
        public bool Active { get; set; }
        public byte[] codeWarehouseRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<bool> Archived { get; set; }
        public Nullable<System.Guid> WarehouseTypeGUID { get; set; }
        public Nullable<System.Guid> WarehouseLocationGUID { get; set; }
    
        public virtual codeDutyStations codeDutyStations { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<codeItemModelWarehouse> codeItemModelWarehouse { get; set; }
        public virtual codeWarehouseLocation codeWarehouseLocation { get; set; }
        public virtual codeWarehouseType codeWarehouseType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<codeWarehouse> codeWarehouse1 { get; set; }
        public virtual codeWarehouse codeWarehouse2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<codeWarehouseFocalPoint> codeWarehouseFocalPoint { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataWarehouseFocalPoint> dataWarehouseFocalPoint { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<codeWarehouseLanguage> codeWarehouseLanguage { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<codeWarehouseStore> codeWarehouseStore { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataItemInput> dataItemInput { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataItemInputDetail> dataItemInputDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataItemOutput> dataItemOutput { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataItemTransfer> dataItemTransfer { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataItemTransfer> dataItemTransfer1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataItemVerificationWarehousePeriod> dataItemVerificationWarehousePeriod { get; set; }
        public virtual codeOrganizationsInstances codeOrganizationsInstances { get; set; }
    }
}
