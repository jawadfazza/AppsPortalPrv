
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
    
    public partial class dataShuttleRequest
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public dataShuttleRequest()
        {
            this.dataShuttleRequestStaff = new HashSet<dataShuttleRequestStaff>();
        }
    
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ShuttleRequestGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid ShuttleRequestGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[DataType(DataType.DateTime)]
    	[Display(Name = "DepartureDate", ResourceType = typeof(resxDbFields))]
    	public System.DateTime DepartureDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DepartureDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDepartureDate { get { return new Portal().LocalTime(this.DepartureDate); } }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "ReturnDateTime", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> ReturnDateTime{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "ReturnDateTime", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalReturnDateTime { get { return new Portal().LocalTime(this.ReturnDateTime); } }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ReferralStatusGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid ReferralStatusGUID{ get; set; }
    	
        [Display(Name = "DeparturePointGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> DeparturePointGUID{ get; set; }
    	
        [Display(Name = "DropOffPointGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> DropOffPointGUID{ get; set; }
    	
        [Display(Name = "StartLocationGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> StartLocationGUID{ get; set; }
    	
        [Display(Name = "EndLocationGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> EndLocationGUID{ get; set; }
    	
        [Display(Name = "DeparturePointReturnGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> DeparturePointReturnGUID{ get; set; }
    	
        [Display(Name = "DropOffPointReturnGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> DropOffPointReturnGUID{ get; set; }
    	
        [Display(Name = "StartLocationReturnGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> StartLocationReturnGUID{ get; set; }
    	
        [Display(Name = "EndLocationReturnGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> EndLocationReturnGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "UserGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid UserGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ShuttleTravelPurposeGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid ShuttleTravelPurposeGUID{ get; set; }
    	
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "AdminComment", ResourceType = typeof(resxDbFields))]
    	public string AdminComment{ get; set; }
    	
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "StaffComment", ResourceType = typeof(resxDbFields))]
    	public string StaffComment{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "FileAttached", ResourceType = typeof(resxDbFields))]
    	public bool FileAttached{ get; set; }
    	
        [Display(Name = "OrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> OrganizationInstanceGUID{ get; set; }
    	
        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> DutyStationGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public bool Active{ get; set; }
    	
        public byte[] dataShuttleRequestRowVersion{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }
    	
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataShuttleRequestStaff> dataShuttleRequestStaff { get; set; }
    }
}
