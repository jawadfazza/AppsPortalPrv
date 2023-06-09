
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
    
    public partial class dataShuttle
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public dataShuttle()
        {
            this.dataShuttleVehicle = new HashSet<dataShuttleVehicle>();
        }
    
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ShuttleGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid ShuttleGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[DataType(DataType.DateTime)]
    	[Display(Name = "DepartureDateTime", ResourceType = typeof(resxDbFields))]
    	public System.DateTime DepartureDateTime{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DepartureDateTime", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDepartureDateTime { get { return new Portal().LocalTime(this.DepartureDateTime); } }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "ReturnDateTime", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> ReturnDateTime{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "ReturnDateTime", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalReturnDateTime { get { return new Portal().LocalTime(this.ReturnDateTime); } }
    	
        [Display(Name = "OrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> OrganizationInstanceGUID{ get; set; }
    	
        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> DutyStationGUID{ get; set; }
    	
        [Display(Name = "DeparturePointGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> DeparturePointGUID{ get; set; }
    	
        [Display(Name = "DropOffPointGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> DropOffPointGUID{ get; set; }
    	
        [Display(Name = "StartLocationGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> StartLocationGUID{ get; set; }
    	
        [Display(Name = "EndLocationGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> EndLocationGUID{ get; set; }
    	
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "DeparturePointFreeText", ResourceType = typeof(resxDbFields))]
    	public string DeparturePointFreeText{ get; set; }
    	
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "DropOffPointFreeText", ResourceType = typeof(resxDbFields))]
    	public string DropOffPointFreeText{ get; set; }
    	
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "PassByLocations", ResourceType = typeof(resxDbFields))]
    	public string PassByLocations{ get; set; }
    	
        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "SharedBy", ResourceType = typeof(resxDbFields))]
    	public string SharedBy{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "ShareDatetime", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> ShareDatetime{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "ShareDatetime", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalShareDatetime { get { return new Portal().LocalTime(this.ShareDatetime); } }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public bool Active{ get; set; }
    	
        public byte[] dataShuttleRowVersion{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }
    	
    
        public virtual codeDutyStations codeDutyStations { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataShuttleVehicle> dataShuttleVehicle { get; set; }
    }
}
