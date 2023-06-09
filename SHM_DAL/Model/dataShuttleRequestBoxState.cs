
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SHM_DAL.Model
{
    
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using RES_Repo.Globalization;
    using System.ComponentModel.DataAnnotations.Schema;
    using Portal_BL.Library;
    
    public partial class dataShuttleRequestBoxState
    {
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ShuttleRequestBoxStateGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid ShuttleRequestBoxStateGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ShuttleRequestGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid ShuttleRequestGUID{ get; set; }
    	
        [Display(Name = "ShuttleRequestRouteStepGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> ShuttleRequestRouteStepGUID{ get; set; }
    	
        [Display(Name = "StartLocationGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> StartLocationGUID{ get; set; }
    	
        [Display(Name = "EndLocationGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> EndLocationGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "IsBoxStateDroped", ResourceType = typeof(resxDbFields))]
    	public bool IsBoxStateDroped{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "IsDeparture", ResourceType = typeof(resxDbFields))]
    	public bool IsDeparture{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public bool Active{ get; set; }
    	
        public byte[] dataShuttleRequestBoxStateRowVersion{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }
    	
    
        public virtual dataShuttleRequest dataShuttleRequest { get; set; }
        public virtual dataShuttleRequestRouteStep dataShuttleRequestRouteStep { get; set; }
    }
}
