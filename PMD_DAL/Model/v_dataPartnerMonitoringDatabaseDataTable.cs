
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
    
    public partial class v_dataPartnerMonitoringDatabaseDataTable
    {
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "EDMXID", ResourceType = typeof(resxDbFields))]
    	public int EDMXID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "PartnerMonitoringDBGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid PartnerMonitoringDBGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "DateOfReport", ResourceType = typeof(resxDbFields))]
    	public System.DateTime DateOfReport{ get; set; }
    	
        [Display(Name = "GovernorateGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> GovernorateGUID{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Governorate", ResourceType = typeof(resxDbFields))]
    	public string Governorate{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "admin1Name_en", ResourceType = typeof(resxDbFields))]
    	public string admin1Name_en{ get; set; }
    	
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "District", ResourceType = typeof(resxDbFields))]
    	public string District{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "admin2Name_en", ResourceType = typeof(resxDbFields))]
    	public string admin2Name_en{ get; set; }
    	
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "SubDistrict", ResourceType = typeof(resxDbFields))]
    	public string SubDistrict{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "admin3Name_en", ResourceType = typeof(resxDbFields))]
    	public string admin3Name_en{ get; set; }
    	
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "CommunityName", ResourceType = typeof(resxDbFields))]
    	public string CommunityName{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "admin4Name_en", ResourceType = typeof(resxDbFields))]
    	public string admin4Name_en{ get; set; }
    	
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Neighborhood", ResourceType = typeof(resxDbFields))]
    	public string Neighborhood{ get; set; }
    	
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Location", ResourceType = typeof(resxDbFields))]
    	public string Location{ get; set; }
    	
        [Display(Name = "Longitude", ResourceType = typeof(resxDbFields))]
    	public Nullable<double> Longitude{ get; set; }
    	
        [Display(Name = "Latitude", ResourceType = typeof(resxDbFields))]
    	public Nullable<double> Latitude{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ObjectiveGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid ObjectiveGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(500, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ObjectiveDescription", ResourceType = typeof(resxDbFields))]
    	public string ObjectiveDescription{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "OutputGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid OutputGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(500, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "OutputDescription", ResourceType = typeof(resxDbFields))]
    	public string OutputDescription{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "IndicatorGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid IndicatorGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(500, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "IndicatorDescription", ResourceType = typeof(resxDbFields))]
    	public string IndicatorDescription{ get; set; }
    	
        [Display(Name = "MeasurementTotal", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> MeasurementTotal{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "IsDirectActivity", ResourceType = typeof(resxDbFields))]
    	public bool IsDirectActivity{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ImplementingPartnerGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid ImplementingPartnerGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(256, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "PartnerDescription", ResourceType = typeof(resxDbFields))]
    	public string PartnerDescription{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(101, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "CreatedByName", ResourceType = typeof(resxDbFields))]
    	public string CreatedByName{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "CreatedBy", ResourceType = typeof(resxDbFields))]
    	public System.Guid CreatedBy{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "CreatedOn", ResourceType = typeof(resxDbFields))]
    	public System.DateTime CreatedOn{ get; set; }
    	
        [Display(Name = "UpdatedBy", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> UpdatedBy{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "UpdatedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> UpdatedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "UpdatedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalUpdatedOn { get { return new Portal().LocalTime(this.UpdatedOn); } }
    	
        [Display(Name = "IsVerifiedByFieldTech", ResourceType = typeof(resxDbFields))]
    	public Nullable<bool> IsVerifiedByFieldTech{ get; set; }
    	
        [Display(Name = "VerifiedByFieldTechGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> VerifiedByFieldTechGUID{ get; set; }
    	
        [StringLength(101, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "VerifiedByFullName", ResourceType = typeof(resxDbFields))]
    	public string VerifiedByFullName{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "VerifiedByFieldTechOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> VerifiedByFieldTechOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "VerifiedByFieldTechOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalVerifiedByFieldTechOn { get { return new Portal().LocalTime(this.VerifiedByFieldTechOn); } }
    	
        [Display(Name = "IsApprovedByCountryTech", ResourceType = typeof(resxDbFields))]
    	public Nullable<bool> IsApprovedByCountryTech{ get; set; }
    	
        [Display(Name = "ApprovedByCountryTechGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> ApprovedByCountryTechGUID{ get; set; }
    	
        [StringLength(101, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ApprovedByFullName", ResourceType = typeof(resxDbFields))]
    	public string ApprovedByFullName{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "ApprovedByCountryTechOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> ApprovedByCountryTechOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "ApprovedByCountryTechOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalApprovedByCountryTechOn { get { return new Portal().LocalTime(this.ApprovedByCountryTechOn); } }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "IsVerified", ResourceType = typeof(resxDbFields))]
    	public bool IsVerified{ get; set; }
    	
        [Display(Name = "VerifiedBy", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> VerifiedBy{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "VerifiedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> VerifiedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "VerifiedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalVerifiedOn { get { return new Portal().LocalTime(this.VerifiedOn); } }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public bool Active{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(8, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "dataPartnerMonitoringDBRowVersion", ResourceType = typeof(resxDbFields))]
    	public byte[] dataPartnerMonitoringDBRowVersion{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "LanguageID", ResourceType = typeof(resxDbFields))]
    	public string LanguageID{ get; set; }
    	
    }
}