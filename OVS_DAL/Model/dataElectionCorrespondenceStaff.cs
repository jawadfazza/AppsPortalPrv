
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OVS_DAL.Model
{
    
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using RES_Repo.Globalization;
    using System.ComponentModel.DataAnnotations.Schema;
    using Portal_BL.Library;
    
    public partial class dataElectionCorrespondenceStaff
    {
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ElectionCorrespondenceStaffGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid ElectionCorrespondenceStaffGUID{ get; set; }
    	
        [Display(Name = "ElectionCorrespondenceGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> ElectionCorrespondenceGUID{ get; set; }
    	
        [Display(Name = "ElectionStaffGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> ElectionStaffGUID{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "ReceivedDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> ReceivedDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "ReceivedDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalReceivedDate { get { return new Portal().LocalTime(this.ReceivedDate); } }
    	
        [Display(Name = "IsReceived", ResourceType = typeof(resxDbFields))]
    	public Nullable<bool> IsReceived{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public bool Active{ get; set; }
    	
        public byte[] dataElectionCorrespondenceStaffRowVersion{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }
    	
    
        public virtual dataElectionCorrespondence dataElectionCorrespondence { get; set; }
        public virtual dataElectionStaff dataElectionStaff { get; set; }
    }
}
