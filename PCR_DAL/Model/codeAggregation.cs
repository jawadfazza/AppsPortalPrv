
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PCR_DAL.Model
{
    
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using RES_Repo.Globalization;
    using System.ComponentModel.DataAnnotations.Schema;
    using Portal_BL.Library;
    
    public partial class codeAggregation
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public codeAggregation()
        {
            this.dataPartnerReportDetail = new HashSet<dataPartnerReportDetail>();
            this.codeCategoryAggregation = new HashSet<codeCategoryAggregation>();
        }
    
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "AggregationGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid AggregationGUID{ get; set; }
    	
        [Display(Name = "GroupGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> GroupGUID{ get; set; }
    	
        [Display(Name = "GenderGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> GenderGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "AggregationDescription", ResourceType = typeof(resxDbFields))]
    	public string AggregationDescription{ get; set; }
    	
        [StringLength(5, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ColumnCharacter", ResourceType = typeof(resxDbFields))]
    	public string ColumnCharacter{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public bool Active{ get; set; }
    	
        public byte[] codeAggregationRowVersion{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }
    	
        [Display(Name = "ReportGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> ReportGUID{ get; set; }
    	
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataPartnerReportDetail> dataPartnerReportDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<codeCategoryAggregation> codeCategoryAggregation { get; set; }
    }
}
