
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
    
    public partial class dataPPAForm
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public dataPPAForm()
        {
            this.dataPPAFormHistory = new HashSet<dataPPAFormHistory>();
        }
    
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "PPAFormGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid PPAFormGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ImplementingPartnerGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid ImplementingPartnerGUID{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "PPANumber", ResourceType = typeof(resxDbFields))]
    	public string PPANumber{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "PPAAmendmentNumber", ResourceType = typeof(resxDbFields))]
    	public string PPAAmendmentNumber{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "CostCenter", ResourceType = typeof(resxDbFields))]
    	public string CostCenter{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "PillarGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid PillarGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ImpactAreaGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid ImpactAreaGUID{ get; set; }
    	
        [Display(Name = "ImpactStatementGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> ImpactStatementGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "OutcomeAreaGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid OutcomeAreaGUID{ get; set; }
    	
        [Display(Name = "OutcomeStatementGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> OutcomeStatementGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "OutputStatementGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid OutputStatementGUID{ get; set; }
    	
        [Display(Name = "AllocatedBudget", ResourceType = typeof(resxDbFields))]
    	public Nullable<double> AllocatedBudget{ get; set; }
    	
        [Display(Name = "AllocatedBudgetUSD", ResourceType = typeof(resxDbFields))]
    	public Nullable<double> AllocatedBudgetUSD{ get; set; }
    	
        [Display(Name = "Expenditures", ResourceType = typeof(resxDbFields))]
    	public Nullable<double> Expenditures{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "ExpendituresAsOfDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> ExpendituresAsOfDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "ExpendituresAsOfDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalExpendituresAsOfDate { get { return new Portal().LocalTime(this.ExpendituresAsOfDate); } }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "IndicatorGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid IndicatorGUID{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "PPAUnit", ResourceType = typeof(resxDbFields))]
    	public string PPAUnit{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "PPATarget", ResourceType = typeof(resxDbFields))]
    	public string PPATarget{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "PPAPopulationType", ResourceType = typeof(resxDbFields))]
    	public string PPAPopulationType{ get; set; }
    	
        [StringLength(500, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "PPALocation", ResourceType = typeof(resxDbFields))]
    	public string PPALocation{ get; set; }
    	
        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
    	public string Comments{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public bool Active{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }
    	
        public byte[] dataPPAFormRowVersion{ get; set; }
    	
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataPPAFormHistory> dataPPAFormHistory { get; set; }
    }
}
