using ExpressiveAnnotations.Attributes;
using RES_Repo.Globalization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMD_DAL.Model
{
    #region CRI
    public class ItemsTransferBlukItemUpdateModel
    {
        public System.Guid ItemsTransferGUID { get; set; }
        public List<PMDItemsTransferItmesDetail> ItemsTransferDetails { get; set; }
    }

    public class PMDItemsTransferItmesDetail
    {
        public Guid UnitOfAchievementGUID { get; set; }

        public System.Guid? ItemsTransferDetailGUID { get; set; }
        public string UnitOfAchievementGroupingDescription { get; set; }
        public string UnitOfAchievementDescription { get; set; }
        public string UnitOfAchievementGuidance { get; set; }
        public int? UnitOfAchievementOrder { get; set; }
        public int MeasurementTotal { get; set; }
    }

    public class DamagedLostDistributionBlukItemUpdateModel
    {
        public System.Guid DamagedLostDistributionGUID { get; set; }
        public List<PMDDamagedLostDistributionItmesDetail> damagedLostDistributionDetails { get; set; }
    }

    public class PMDDamagedLostDistributionItmesDetail
    {
        public Guid UnitOfAchievementGUID { get; set; }

        public System.Guid? DamagedLostDistributionDetailGUID { get; set; }
        public string UnitOfAchievementGroupingDescription { get; set; }
        public string UnitOfAchievementDescription { get; set; }
        public string UnitOfAchievementGuidance { get; set; }
        public int? UnitOfAchievementOrder { get; set; }
        public int MeasurementTotal { get; set; }
    }

    public class DispatchBlukItemUpdateModel
    {
        public System.Guid DispatchGUID { get; set; }
        public  List<PMDDispatchItmesDetail> dispatchDetails { get; set; }
    }

    public class PMDDispatchItmesDetail
    {
        public Guid UnitOfAchievementGUID { get; set; }

        public System.Guid? DispatchDetailGUID { get; set; }
        public string UnitOfAchievementGroupingDescription { get; set; }
        public string UnitOfAchievementDescription { get; set; }
        public string UnitOfAchievementGuidance { get; set; }
        public int? UnitOfAchievementOrder { get; set; }
        public int MeasurementTotal { get; set; }
    }

    public class PMDWarehouseUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "PmdWarehouseGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid PmdWarehouseGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ImplementingPartnerGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ImplementingPartnerGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "GovernorateGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid GovernorateGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "PMDWarehouseDescription", ResourceType = typeof(resxDbFields))]
        public string PMDWarehouseDescription { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "admin1Pcode", ResourceType = typeof(resxDbFields))]
        public string admin1Pcode { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "District", ResourceType = typeof(resxDbFields))]
        public string District { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SubDistrict", ResourceType = typeof(resxDbFields))]
        public string SubDistrict { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "admin4Pcode", ResourceType = typeof(resxDbFields))]
        public string admin4Pcode { get; set; }


        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }
        public byte[] codePmdWarehouseRowVersion { get; set; }
    }
    public class PMDWarehousesDataTableModel
    {
        public System.Guid PmdWarehouseGUID { get; set; }
        public string PMDWarehouseDescription { get; set; }
        public string ImplementingPartnerGUID { get; set; }
        public string GovernorateGUID { get; set; }
        public string ImplementingPartner { get; set; }
        public string admin1Pcode { get; set; }
        public bool Active { get; set; }
        public byte[] codePmdWarehouseRowVersion { get; set; }
    }
    public class PMDReportParametersList
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        public int Report { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.Date)]
        [Display(Name = "StartDate", ResourceType = typeof(resxDbFields))]
        public System.DateTime? StartDate { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.Date)]
        [Display(Name = "EndDate", ResourceType = typeof(resxDbFields))]
        public System.DateTime? EndDate { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        public string[] OrganizationInstance { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        public string[] Governorate { get; set; }
    }
    public class DamagedLostDistributionUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DamagedLostDistributionGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid DamagedLostDistributionGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "OrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid OrganizationInstanceGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.Date)]
        [Display(Name = "DamagedLostDistributionDate", ResourceType = typeof(resxDbFields))]
        public System.DateTime? DamagedLostDistributionDate { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "GovernorateGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid GovernorateGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "admin1Pcode", ResourceType = typeof(resxDbFields))]
        public string admin1Pcode { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "admin4Pcode", ResourceType = typeof(resxDbFields))]
        public string admin4Pcode { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "PmdWarehouseGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid? PmdWarehouseGUID { get; set; }

        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataPMDDamagedLostDistributionRowVersion { get; set; }

        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "District", ResourceType = typeof(resxDbFields))]
        public string District { get; set; }

        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SubDistrict", ResourceType = typeof(resxDbFields))]
        public string SubDistrict { get; set; }
    }
    public class DamagedLostDistributionDataTableModel
    {
        public System.Guid DamagedLostDistributionGUID { get; set; }
        public string OrganizationInstanceGUID { get; set; }
        public string OrganizationInstanceDescription { get; set; }
        public System.DateTime DamagedLostDistributionDate { get; set; }
        public string GovernorateGUID { get; set; }
        public string admin1Pcode { get; set; }
        public string admin4Pcode { get; set; }
        public string Comments { get; set; }
        public bool Active { get; set; }
        public byte[] dataPMDDamagedLostDistributionRowVersion { get; set; }
    }
    public class ItemsTransferUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ItemsTransferGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ItemsTransferGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "FromOrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid FromOrganizationInstanceGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ToOrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ToOrganizationInstanceGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.Date)]
        [Display(Name = "ItemsTransferDate", ResourceType = typeof(resxDbFields))]
        public System.DateTime? ItemsTransferDate { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "FromGovernorateGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid FromGovernorateGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Fromadmin1Pcode", ResourceType = typeof(resxDbFields))]
        public string Fromadmin1Pcode { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Fromadmin4Pcode", ResourceType = typeof(resxDbFields))]
        public string Fromadmin4Pcode { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ToGovernorateGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ToGovernorateGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Toadmin1Pcode", ResourceType = typeof(resxDbFields))]
        public string Toadmin1Pcode { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Toadmin4Pcode", ResourceType = typeof(resxDbFields))]
        public string Toadmin4Pcode { get; set; }


        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "FromPmdWarehouseGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid? FromPmdWarehouseGUID { get; set; }


        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ToPmdWarehouseGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid? ToPmdWarehouseGUID { get; set; }


        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataPMDItemsTransferRowVersion { get; set; }

        public byte[] dataPMDDispatchRowVersion { get; set; }

        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "District", ResourceType = typeof(resxDbFields))]
        public string FromDistrict { get; set; }

        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SubDistrict", ResourceType = typeof(resxDbFields))]
        public string FromSubDistrict { get; set; }

 

        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "District", ResourceType = typeof(resxDbFields))]
        public string ToDistrict { get; set; }

        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SubDistrict", ResourceType = typeof(resxDbFields))]
        public string ToSubDistrict { get; set; }


    }
    public class ItemsTransferDataTableModel
    {
        public System.Guid ItemsTransferGUID { get; set; }
        public string FromOrganizationInstanceGUID { get; set; }
        public string FromOrganizationInstanceDescription { get; set; }
        public string ToOrganizationInstanceGUID { get; set; }
        public string ToOrganizationInstanceDescription { get; set; }
        public System.DateTime ItemsTransferDate { get; set; }
        public string FromGovernorateGUID { get; set; }
        public string Fromadmin1Pcode { get; set; }
        public string Fromadmin4Pcode { get; set; }

        public string ToGovernorateGUID { get; set; }
        public string Toadmin1Pcode { get; set; }
        public string Toadmin4Pcode { get; set; }
        public string Comments { get; set; }
        public bool Active { get; set; }
        public byte[] dataPMDItemsTransferRowVersion { get; set; }
    }
    public class DispatchDataTableModel
    {
        public System.Guid DispatchGUID { get; set; }
        public string OrganizationInstanceGUID { get; set; }
        public string OrganizationInstanceDescription { get; set; }
        public System.DateTime DispatchDate { get; set; }
        public string GovernorateGUID { get; set; }
        public string admin1Pcode { get; set; }
        public string admin4Pcode { get; set; }
        public string Comments { get; set; }
        public bool Active { get; set; }
        public byte[] dataPMDDispatchRowVersion { get; set; }
    }

    public class DispatchUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DispatchGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid DispatchGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "OrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid OrganizationInstanceGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.Date)]
        [Display(Name = "DispatchDate", ResourceType = typeof(resxDbFields))]
        public System.DateTime? DispatchDate { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "GovernorateGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid GovernorateGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "admin1Pcode", ResourceType = typeof(resxDbFields))]
        public string admin1Pcode { get; set; }

        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "District", ResourceType = typeof(resxDbFields))]
        public string District { get; set; }

        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SubDistrict", ResourceType = typeof(resxDbFields))]
        public string SubDistrict { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "admin4Pcode", ResourceType = typeof(resxDbFields))]
        public string admin4Pcode { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "PmdWarehouseGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid? PmdWarehouseGUID { get; set; }

        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataPMDDispatchRowVersion { get; set; }

     
    }
    #endregion
    public class PartnerMonitoringDatabaseDataTableModel
    {
        public Guid PartnerMonitoringDBGUID { get; set; }
        public string ObjectiveGUID { get; set; }
        public string ObjectiveDescription { get; set; }
        public string OutputGUID { get; set; }
        public string OutputDescription { get; set; }
        public string IndicatorGUID { get; set; }
        public string IndicatorDescription { get; set; }
        public string Governorate { get; set; }
        public string admin1PCode { get; set; }
        public string District { get; set; }
        public string SubDistrict { get; set; }
        public string CommunityName { get; set; }
        public string ImplementingPartner { get; set; }

        public bool IsVerifiedByFieldTech { get; set; }

        //public Nullable<System.Guid> VerifiedByFieldTechGUID { get; set; }   
        //public Nullable<System.DateTime> VerifiedByFieldTechOn { get; set; }
        //[Display(Name = "IsApprovedByCountryTech", ResourceType = typeof(resxDbFields))]

        public bool IsApprovedByCountryTech { get; set; }

        //public Nullable<System.Guid> ApprovedByCountryTechGUID { get; set; }
        //public Nullable<System.DateTime> ApprovedByCountryTechOn { get; set; }


        public bool IsVerified { get; set; }
        public bool Active { get; set; }
        public byte[] dataPartnerMonitoringDBRowVersion { get; set; }


    }

    public class PartnerMonitoringDatabaseUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "PartnerMonitoringDBGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid PartnerMonitoringDBGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.Date)]
        [Display(Name = "DateOfReport", ResourceType = typeof(resxDbFields))]
        public System.DateTime? DateOfReport { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ObjectiveGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ObjectiveGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "OutputGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid OutputGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "IndicatorGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid IndicatorGUID { get; set; }

        public string IndicatorGuidance { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ObjectiveStatusGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ObjectiveStatusGUID { get; set; }


        [Display(Name = "GovernorateGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> GovernorateGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Governorate", ResourceType = typeof(resxDbFields))]
        public string Governorate { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "District", ResourceType = typeof(resxDbFields))]
        public string District { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SubDistrict", ResourceType = typeof(resxDbFields))]
        public string SubDistrict { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "CommunityName", ResourceType = typeof(resxDbFields))]
        public string CommunityName { get; set; }

        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Neighborhood", ResourceType = typeof(resxDbFields))]
        public string Neighborhood { get; set; }

        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Location", ResourceType = typeof(resxDbFields))]
        public string Location { get; set; }

        [Display(Name = "Longitude", ResourceType = typeof(resxDbFields))]
        public Nullable<double> Longitude { get; set; }

        [Display(Name = "PMDCommunityCenterGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> PMDCommunityCenterGUID { get; set; }

        [Display(Name = "Latitude", ResourceType = typeof(resxDbFields))]
        public Nullable<double> Latitude { get; set; }

        [Display(Name = "UnitOfAchievementGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> UnitOfAchievementGUID { get; set; }

        public string UnitOfAchievementGuidance { get; set; }


        [RequiredIf("ObjectiveGUIDBool == true", ErrorMessage = "Field is Required")]
        [Range(0, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        [Display(Name = "MeasurementTotal", ResourceType = typeof(resxDbFields))]
        public Nullable<int> MeasurementTotal { get; set; }

        [NotMapped]
        public bool ObjectiveGUIDBool { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        [Display(Name = "RefTotal", ResourceType = typeof(resxDbFields))]
        public Nullable<int> RefTotal { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        [Display(Name = "IdpTotal", ResourceType = typeof(resxDbFields))]
        public Nullable<int> IdpTotal { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        [Display(Name = "RetTotal", ResourceType = typeof(resxDbFields))]
        public Nullable<int> RetTotal { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        [Display(Name = "HostCommunity", ResourceType = typeof(resxDbFields))]
        public Nullable<int> HostCommunity { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        [Display(Name = "Boys", ResourceType = typeof(resxDbFields))]
        public Nullable<int> Boys { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        [Display(Name = "Girls", ResourceType = typeof(resxDbFields))]
        public Nullable<int> Girls { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        [Display(Name = "Men", ResourceType = typeof(resxDbFields))]
        public Nullable<int> Men { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        [Display(Name = "Women", ResourceType = typeof(resxDbFields))]
        public Nullable<int> Women { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        [Display(Name = "ElderlyWomen", ResourceType = typeof(resxDbFields))]
        public Nullable<int> ElderlyWomen { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        [Display(Name = "ElderlyMen", ResourceType = typeof(resxDbFields))]
        public Nullable<int> ElderlyMen { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        [Display(Name = "AgeGroupTotal", ResourceType = typeof(resxDbFields))]
        public Nullable<int> AgeGroupTotal { get; set; }


        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ImplementingPartnerGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ImplementingPartnerGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "IsDirectActivity", ResourceType = typeof(resxDbFields))]
        public bool IsDirectActivity { get; set; }

        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "CreatedBy", ResourceType = typeof(resxDbFields))]
        public System.Guid CreatedBy { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.DateTime)]
        [Display(Name = "CreatedOn", ResourceType = typeof(resxDbFields))]
        public System.DateTime CreatedOn { get; set; }



        [Display(Name = "UpdatedBy", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> UpdatedBy { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "UpdatedOn", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> UpdatedOn { get; set; }



        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "IsVerified", ResourceType = typeof(resxDbFields))]
        public bool IsVerified { get; set; }

        [Display(Name = "VerifiedByOffice", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> VerifiedByOffice { get; set; }

        [Display(Name = "VerifiedBy", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> VerifiedBy { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "VerifiedOn", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> VerifiedOn { get; set; }

        [Display(Name = "IsVerifiedByFieldTech", ResourceType = typeof(resxDbFields))]
        public Nullable<bool> IsVerifiedByFieldTech { get; set; }

        [Display(Name = "VerifiedByFieldTechGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> VerifiedByFieldTechGUID { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "VerifiedByFieldTechOn", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> VerifiedByFieldTechOn { get; set; }

        [Display(Name = "NotVerifiedByFieldReason", ResourceType = typeof(resxDbFields))]
        public string NotVerifiedByFieldReason { get; set; }

        [Display(Name = "IsApprovedByProgramme", ResourceType = typeof(resxDbFields))]
        public Nullable<bool> IsApprovedByProgramme { get; set; }

        [Display(Name = "ApprovedByProgrammeGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ApprovedByProgrammeGUID { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "ApprovedByProgrammeOn", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> ApprovedByProgrammeOn { get; set; }

        [Display(Name = "IsApprovedByCountryTech", ResourceType = typeof(resxDbFields))]
        public Nullable<bool> IsApprovedByCountryTech { get; set; }

        [Display(Name = "ApprovedByCountryTechGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ApprovedByCountryTechGUID { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "ApprovedByCountryTechOn", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> ApprovedByCountryTechOn { get; set; }

        [Display(Name = "NotApprovedByCountryReason", ResourceType = typeof(resxDbFields))]
        public string NotApprovedByCountryReason { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> DeletedOn { get; set; }


        public byte[] dataPartnerMonitoringDBRowVersion { get; set; }



        public List<PMDHealthUnitOfAchievement> PMDHealthUnitOfAchievements { get; set; }
        public List<PMDDomesticUnitOfAchievement> PMDDomesticUnitOfAchievements { get; set; }
    }

    public class PMDHealthUnitOfAchievement
    {

        public Guid PMDHealthUnitOfAchievementGUID { get; set; }
        public Guid PartnerMonitoringDBGUID { get; set; }
        public Guid UnitOfAchievementGUID { get; set; }
        public string UnitOfAchievementDescription { get; set; }
        public string UnitOfAchievementGuidance { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        [Display(Name = "MeasurementTotal", ResourceType = typeof(resxDbFields))]
        public int MeasurementTotal { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public byte[] dataPMDHealthUnitOfAchievementRowVersion { get; set; }


    }

    public class PMDDomesticUnitOfAchievement
    {

        public Guid PMDDomesticUnitOfAchievementGUID { get; set; }
        public Guid PartnerMonitoringDBGUID { get; set; }
        public Guid UnitOfAchievementGUID { get; set; }
        public string UnitOfAchievementDescription { get; set; }
        public string UnitOfAchievementGuidance { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        [Display(Name = "MeasurementTotal", ResourceType = typeof(resxDbFields))]
        public int MeasurementTotal { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "DistributionDate", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> DistributionDate { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        [Display(Name = "StockValue", ResourceType = typeof(resxDbFields))]
        public Nullable<int> StockValue { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        [Display(Name = "ResponseValue", ResourceType = typeof(resxDbFields))]
        public Nullable<int> ResponseValue { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public byte[] dataPMDDomesticUnitOfAchievementRowVersion { get; set; }


    }
}
