
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ISS_DAL.Model
{
    
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using RES_Repo.Globalization;
    using System.ComponentModel.DataAnnotations.Schema;
    using Portal_BL.Library;
    
    public partial class codeISSStock
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public codeISSStock()
        {
            this.codeISSStockLanguage = new HashSet<codeISSStockLanguage>();
            this.dataItemPipeline = new HashSet<dataItemPipeline>();
            this.dataItemStockBalance = new HashSet<dataItemStockBalance>();
            this.codeISSStock1 = new HashSet<codeISSStock>();
        }
    
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "StockGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid StockGUID{ get; set; }
    	
        [Display(Name = "OrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> OrganizationInstanceGUID{ get; set; }
    	
        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> DutyStationGUID{ get; set; }
    	
        [Display(Name = "LocationGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> LocationGUID{ get; set; }
    	
        [Display(Name = "ParentGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> ParentGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public bool Active{ get; set; }
    	
        public byte[] codeISSStockRowVersion{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }
    	
        [Display(Name = "Archived", ResourceType = typeof(resxDbFields))]
    	public Nullable<bool> Archived{ get; set; }
    	
        [Display(Name = "OrderId", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> OrderId{ get; set; }
    	
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<codeISSStockLanguage> codeISSStockLanguage { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataItemPipeline> dataItemPipeline { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataItemStockBalance> dataItemStockBalance { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<codeISSStock> codeISSStock1 { get; set; }
        public virtual codeISSStock codeISSStock2 { get; set; }
    }
}
