
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PMD_DAL.Model
{
    
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using RES_Repo.Globalization;
    using System.ComponentModel.DataAnnotations.Schema;
    using Portal_BL.Library;
    
    public partial class dataPartnerMonitoringObjectiveStatus
    {
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "DataObjectiveStatusGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid DataObjectiveStatusGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ObjectiveStatusGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid ObjectiveStatusGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ObjectiveGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid ObjectiveGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public bool Active{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }
    	
        public byte[] dataPartnerMonitoringObjectiveStatusRowVersion{ get; set; }
    	
    }
}
