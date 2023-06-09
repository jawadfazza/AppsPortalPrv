
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
    
    public partial class dataAHDInternationalStaffEntitlementDetail
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public dataAHDInternationalStaffEntitlementDetail()
        {
            this.dataAHDInternationalStaffEntitlementRoomTaken = new HashSet<dataAHDInternationalStaffEntitlementRoomTaken>();
        }
    
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "InternationalStaffEntitlementDetailGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid InternationalStaffEntitlementDetailGUID{ get; set; }
    	
        [Display(Name = "InternationalStaffEntitlementGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> InternationalStaffEntitlementGUID{ get; set; }
    	
        [Display(Name = "EntitlementTypeGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> EntitlementTypeGUID{ get; set; }
    	
        [Display(Name = "BasePeriodAmount", ResourceType = typeof(resxDbFields))]
    	public Nullable<decimal> BasePeriodAmount{ get; set; }
    	
        [Display(Name = "TotalAmount", ResourceType = typeof(resxDbFields))]
    	public Nullable<decimal> TotalAmount{ get; set; }
    	
        [Display(Name = "TotalDays", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> TotalDays{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "IsToAdd", ResourceType = typeof(resxDbFields))]
    	public bool IsToAdd{ get; set; }
    	
        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
    	public string Comments{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public bool Active{ get; set; }
    	
        public byte[] dataAHDInternationalStaffEntitlementDetailRowVersion{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }
    	
        [Display(Name = "Archived", ResourceType = typeof(resxDbFields))]
    	public Nullable<bool> Archived{ get; set; }
    	
    
        public virtual dataAHDInternationalStaffEntitlement dataAHDInternationalStaffEntitlement { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataAHDInternationalStaffEntitlementRoomTaken> dataAHDInternationalStaffEntitlementRoomTaken { get; set; }
        public virtual codeAHDEntitlementType codeAHDEntitlementType { get; set; }
    }
}
