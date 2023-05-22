
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GTP_DAL.Model
{
    
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using RES_Repo.Globalization;
    using System.ComponentModel.DataAnnotations.Schema;
    using Portal_BL.Library;
    
    public partial class dataPersonalHistoryWorkExperience
    {
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "GTPPHWorkExperienceGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid GTPPHWorkExperienceGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "GTPApplicationGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid GTPApplicationGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[DataType(DataType.DateTime)]
    	[Display(Name = "StartDate", ResourceType = typeof(resxDbFields))]
    	public System.DateTime StartDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "StartDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalStartDate { get { return new Portal().LocalTime(this.StartDate); } }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "EndDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> EndDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "EndDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalEndDate { get { return new Portal().LocalTime(this.EndDate); } }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "FullTimeOrPartTime", ResourceType = typeof(resxDbFields))]
    	public int FullTimeOrPartTime{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Employer", ResourceType = typeof(resxDbFields))]
    	public string Employer{ get; set; }
    	
        [Display(Name = "OnGoingJob", ResourceType = typeof(resxDbFields))]
    	public Nullable<bool> OnGoingJob{ get; set; }
    	
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "JobTitle", ResourceType = typeof(resxDbFields))]
    	public string JobTitle{ get; set; }
    	
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "SupervisorName", ResourceType = typeof(resxDbFields))]
    	public string SupervisorName{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "TypeOfEmployment", ResourceType = typeof(resxDbFields))]
    	public System.Guid TypeOfEmployment{ get; set; }
    	
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "OtherTypeOfEmploymentDesc", ResourceType = typeof(resxDbFields))]
    	public string OtherTypeOfEmploymentDesc{ get; set; }
    	
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "TypeOfWorkersSupervised", ResourceType = typeof(resxDbFields))]
    	public string TypeOfWorkersSupervised{ get; set; }
    	
        [Display(Name = "NumberOfPersonSupervised", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> NumberOfPersonSupervised{ get; set; }
    	
        [DataType(DataType.EmailAddress)]
    	[StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "SupervisorEmail", ResourceType = typeof(resxDbFields))]
    	public string SupervisorEmail{ get; set; }
    	
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "SupervisorPhone", ResourceType = typeof(resxDbFields))]
    	public string SupervisorPhone{ get; set; }
    	
        [Display(Name = "EndingPayRateAmount", ResourceType = typeof(resxDbFields))]
    	public Nullable<double> EndingPayRateAmount{ get; set; }
    	
        [StringLength(10, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Currency", ResourceType = typeof(resxDbFields))]
    	public string Currency{ get; set; }
    	
        [StringLength(500, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ReasonForLeaving", ResourceType = typeof(resxDbFields))]
    	public string ReasonForLeaving{ get; set; }
    	
        [Display(Name = "DescriptionOfDuties", ResourceType = typeof(resxDbFields))]
    	public string DescriptionOfDuties{ get; set; }
    	
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "EmployerAddressLine1", ResourceType = typeof(resxDbFields))]
    	public string EmployerAddressLine1{ get; set; }
    	
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "EmployerAddressLine2", ResourceType = typeof(resxDbFields))]
    	public string EmployerAddressLine2{ get; set; }
    	
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "EmployerAddressLine3", ResourceType = typeof(resxDbFields))]
    	public string EmployerAddressLine3{ get; set; }
    	
        [Display(Name = "EmployerAddressCityGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> EmployerAddressCityGUID{ get; set; }
    	
        [Display(Name = "EmployerAddressCountryGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> EmployerAddressCountryGUID{ get; set; }
    	
        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "EmployerAddressPostalCode", ResourceType = typeof(resxDbFields))]
    	public string EmployerAddressPostalCode{ get; set; }
    	
        [Display(Name = "TypeOfBusinessGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> TypeOfBusinessGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "IsUNExperience", ResourceType = typeof(resxDbFields))]
    	public bool IsUNExperience{ get; set; }
    	
        [Display(Name = "GradeGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> GradeGUID{ get; set; }
    	
        [Display(Name = "UNIndexNumber", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> UNIndexNumber{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "IsUNHCRExperience", ResourceType = typeof(resxDbFields))]
    	public bool IsUNHCRExperience{ get; set; }
    	
        [Display(Name = "UNHCRMsrpID", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> UNHCRMsrpID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ContractTypeGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid ContractTypeGUID{ get; set; }
    	
        [Display(Name = "IfOtherContractTypeDesc", ResourceType = typeof(resxDbFields))]
    	public string IfOtherContractTypeDesc{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public bool Active{ get; set; }
    	
        public byte[] dataPersonalHistoryWorkExperienceRowVersion{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }
    	
    
        public virtual dataGTPApplication dataGTPApplication { get; set; }
    }
}
