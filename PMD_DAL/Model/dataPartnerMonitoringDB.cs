
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
    
    public partial class dataPartnerMonitoringDB
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public dataPartnerMonitoringDB()
        {
            this.dataPartnerMonitoringAttachement = new HashSet<dataPartnerMonitoringAttachement>();
            this.dataPMDDomesticUnitOfAchievement = new HashSet<dataPMDDomesticUnitOfAchievement>();
            this.dataPMDHealthUnitOfAchievement = new HashSet<dataPMDHealthUnitOfAchievement>();
        }
    
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "PartnerMonitoringDBGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid PartnerMonitoringDBGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[DataType(DataType.DateTime)]
    	[Display(Name = "DateOfReport", ResourceType = typeof(resxDbFields))]
    	public System.DateTime DateOfReport{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DateOfReport", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDateOfReport { get { return new Portal().LocalTime(this.DateOfReport); } }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ObjectiveGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid ObjectiveGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "OutputGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid OutputGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "IndicatorGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid IndicatorGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ObjectiveStatusGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid ObjectiveStatusGUID{ get; set; }
    	
        [Display(Name = "GovernorateGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> GovernorateGUID{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Governorate", ResourceType = typeof(resxDbFields))]
    	public string Governorate{ get; set; }
    	
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "District", ResourceType = typeof(resxDbFields))]
    	public string District{ get; set; }
    	
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "SubDistrict", ResourceType = typeof(resxDbFields))]
    	public string SubDistrict{ get; set; }
    	
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "CommunityName", ResourceType = typeof(resxDbFields))]
    	public string CommunityName{ get; set; }
    	
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Neighborhood", ResourceType = typeof(resxDbFields))]
    	public string Neighborhood{ get; set; }
    	
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Location", ResourceType = typeof(resxDbFields))]
    	public string Location{ get; set; }
    	
        [Display(Name = "PMDCommunityCenterGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> PMDCommunityCenterGUID{ get; set; }
    	
        [Display(Name = "Longitude", ResourceType = typeof(resxDbFields))]
    	public Nullable<double> Longitude{ get; set; }
    	
        [Display(Name = "Latitude", ResourceType = typeof(resxDbFields))]
    	public Nullable<double> Latitude{ get; set; }
    	
        [Display(Name = "UnitOfAchievementGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> UnitOfAchievementGUID{ get; set; }
    	
        [Display(Name = "MeasurementTotal", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> MeasurementTotal{ get; set; }
    	
        [Display(Name = "RefTotal", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> RefTotal{ get; set; }
    	
        [Display(Name = "IdpTotal", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> IdpTotal{ get; set; }
    	
        [Display(Name = "RetTotal", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> RetTotal{ get; set; }
    	
        [Display(Name = "AsrTotal", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> AsrTotal{ get; set; }
    	
        [Display(Name = "HostCommunity", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> HostCommunity{ get; set; }
    	
        [Display(Name = "OtherTotal", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> OtherTotal{ get; set; }
    	
        [Display(Name = "Boys", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> Boys{ get; set; }
    	
        [Display(Name = "Girls", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> Girls{ get; set; }
    	
        [Display(Name = "Men", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> Men{ get; set; }
    	
        [Display(Name = "Women", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> Women{ get; set; }
    	
        [Display(Name = "ElderlyWomen", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> ElderlyWomen{ get; set; }
    	
        [Display(Name = "ElderlyMen", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> ElderlyMen{ get; set; }
    	
        [Display(Name = "AgeGroupTotal", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> AgeGroupTotal{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ImplementingPartnerGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid ImplementingPartnerGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "IsDirectActivity", ResourceType = typeof(resxDbFields))]
    	public bool IsDirectActivity{ get; set; }
    	
        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
    	public string Comments{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "CreatedBy", ResourceType = typeof(resxDbFields))]
    	public System.Guid CreatedBy{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[DataType(DataType.DateTime)]
    	[Display(Name = "CreatedOn", ResourceType = typeof(resxDbFields))]
    	public System.DateTime CreatedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "CreatedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalCreatedOn { get { return new Portal().LocalTime(this.CreatedOn); } }
    	
        [Display(Name = "UpdatedBy", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> UpdatedBy{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "UpdatedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> UpdatedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "UpdatedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalUpdatedOn { get { return new Portal().LocalTime(this.UpdatedOn); } }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "IsVerified", ResourceType = typeof(resxDbFields))]
    	public bool IsVerified{ get; set; }
    	
        [Display(Name = "VerifiedByOffice", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> VerifiedByOffice{ get; set; }
    	
        [Display(Name = "VerifiedBy", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> VerifiedBy{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "VerifiedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> VerifiedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "VerifiedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalVerifiedOn { get { return new Portal().LocalTime(this.VerifiedOn); } }
    	
        [Display(Name = "IsVerifiedByFieldTech", ResourceType = typeof(resxDbFields))]
    	public Nullable<bool> IsVerifiedByFieldTech{ get; set; }
    	
        [Display(Name = "VerifiedByFieldTechGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> VerifiedByFieldTechGUID{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "VerifiedByFieldTechOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> VerifiedByFieldTechOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "VerifiedByFieldTechOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalVerifiedByFieldTechOn { get { return new Portal().LocalTime(this.VerifiedByFieldTechOn); } }
    	
        [Display(Name = "NotVerifiedByFieldReason", ResourceType = typeof(resxDbFields))]
    	public string NotVerifiedByFieldReason{ get; set; }
    	
        [Display(Name = "IsApprovedByProgramme", ResourceType = typeof(resxDbFields))]
    	public Nullable<bool> IsApprovedByProgramme{ get; set; }
    	
        [Display(Name = "ApprovedByProgrammeGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> ApprovedByProgrammeGUID{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "ApprovedByProgrammeOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> ApprovedByProgrammeOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "ApprovedByProgrammeOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalApprovedByProgrammeOn { get { return new Portal().LocalTime(this.ApprovedByProgrammeOn); } }
    	
        [Display(Name = "IsApprovedByCountryTech", ResourceType = typeof(resxDbFields))]
    	public Nullable<bool> IsApprovedByCountryTech{ get; set; }
    	
        [Display(Name = "ApprovedByCountryTechGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> ApprovedByCountryTechGUID{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "ApprovedByCountryTechOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> ApprovedByCountryTechOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "ApprovedByCountryTechOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalApprovedByCountryTechOn { get { return new Portal().LocalTime(this.ApprovedByCountryTechOn); } }
    	
        [Display(Name = "NotApprovedByCountryReason", ResourceType = typeof(resxDbFields))]
    	public string NotApprovedByCountryReason{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public bool Active{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }
    	
        public byte[] dataPartnerMonitoringDBRowVersion{ get; set; }
    	
        [Display(Name = "CRILocationTypeGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> CRILocationTypeGUID{ get; set; }
    	
        [Display(Name = "CRIModalityGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> CRIModalityGUID{ get; set; }
    	
        [Display(Name = "CRIResponseTypeGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> CRIResponseTypeGUID{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "CRIDistributionStartDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> CRIDistributionStartDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "CRIDistributionStartDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalCRIDistributionStartDate { get { return new Portal().LocalTime(this.CRIDistributionStartDate); } }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "CRIDistributionEndDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> CRIDistributionEndDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "CRIDistributionEndDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalCRIDistributionEndDate { get { return new Portal().LocalTime(this.CRIDistributionEndDate); } }
    	
        [Display(Name = "CRIDistributionMonth", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> CRIDistributionMonth{ get; set; }
    	
        [Display(Name = "ShelterNumberOfDoors", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> ShelterNumberOfDoors{ get; set; }
    	
        [Display(Name = "ShelterNumberOfElectric", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> ShelterNumberOfElectric{ get; set; }
    	
        [Display(Name = "ShelterNumberOfSolar", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> ShelterNumberOfSolar{ get; set; }
    	
        [Display(Name = "ShelterNumberOfWash", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> ShelterNumberOfWash{ get; set; }
    	
        [Display(Name = "ShelterNumberOfEmergency", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> ShelterNumberOfEmergency{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ShelterTargetPopulation", ResourceType = typeof(resxDbFields))]
    	public string ShelterTargetPopulation{ get; set; }
    	
        [Display(Name = "NumberOfHHs", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> NumberOfHHs{ get; set; }
    	
        [Display(Name = "NumberOfIndividualsPOC", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> NumberOfIndividualsPOC{ get; set; }
    	
        [Display(Name = "TargetStudentCapacity", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> TargetStudentCapacity{ get; set; }
    	
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataPartnerMonitoringAttachement> dataPartnerMonitoringAttachement { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataPMDDomesticUnitOfAchievement> dataPMDDomesticUnitOfAchievement { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataPMDHealthUnitOfAchievement> dataPMDHealthUnitOfAchievement { get; set; }
    }
}
