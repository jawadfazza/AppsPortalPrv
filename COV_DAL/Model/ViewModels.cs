using RES_Repo.Globalization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COV_DAL.Model
{

    public class CovidUNHCRResponseStrategyDataTableModel
    {
        public Guid CovidUNHCRResponseStrategyGUID { get; set; }
        public DateTime DateOfReport { get; set; }
        public string CovStatus { get; set; }
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
        public bool IsVerified { get; set; }
        public string CovIndicatorTechnicalUnitGUID { get; set; }
        public string CovIndicatorTechnicalUnit { get; set; }
        public int Quantity { get; set; }
        public bool Active { get; set; }
        public byte[] dataCovidUNHCRResponseStrategyRowVersion { get; set; }
    }

    public class DataCovidUNHCRResponseStrategyUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "CovidUNHCRResponseStrategyGUID", ResourceType = typeof(resxDbFields))]
        public Guid CovidUNHCRResponseStrategyGUID { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DateOfReport", ResourceType = typeof(resxDbFields))]
        public DateTime? DateOfReport { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "CovStatus", ResourceType = typeof(resxDbFields))]
        public string CovStatus { get; set; }
        public Guid? GovernorateGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Governorate", ResourceType = typeof(resxDbFields))]
        public string Governorate { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "District", ResourceType = typeof(resxDbFields))]
        public string District { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SubDistrict", ResourceType = typeof(resxDbFields))]
        public string SubDistrict { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "CommunityName", ResourceType = typeof(resxDbFields))]
        public string CommunityName { get; set; }

        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Neighborhood", ResourceType = typeof(resxDbFields))]
        public string Neighborhood { get; set; }

        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Location", ResourceType = typeof(resxDbFields))]
        public string Location { get; set; }

        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Longitude", ResourceType = typeof(resxDbFields))]
        public double? Longitude { get; set; }

        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Latitude", ResourceType = typeof(resxDbFields))]
        public double? Latitude { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ObjectiveGUID", ResourceType = typeof(resxDbFields))]
        public Guid ObjectiveGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "OutputGUID", ResourceType = typeof(resxDbFields))]
        public Guid OutputGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "IndicatorGUID", ResourceType = typeof(resxDbFields))]
        public Guid IndicatorGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "CovIndicatorTechnicalUnitGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid CovIndicatorTechnicalUnitGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Measure_Achievments", ResourceType = typeof(resxDbFields))]
        public string Measure_Achievments { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "UNHCRStrategy", ResourceType = typeof(resxDbFields))]
        public int? UNHCRStrategy { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DirectActivities", ResourceType = typeof(resxDbFields))]
        public bool? DirectActivities { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Associated", ResourceType = typeof(resxDbFields))]
        public bool? Associated { get; set; }

        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ImplementingPartner", ResourceType = typeof(resxDbFields))]
        public string ImplementingPartner { get; set; }

        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Quantity", ResourceType = typeof(resxDbFields))]
        public int? Quantity { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "CovUnitGUID", ResourceType = typeof(resxDbFields))]
        public Guid CovUnitGUID { get; set; }

        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "UnitCost", ResourceType = typeof(resxDbFields))]
        public double? UnitCost { get; set; }

        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Month", ResourceType = typeof(resxDbFields))]
        public int? Month { get; set; }

        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "CostEstimationThreeMonthes", ResourceType = typeof(resxDbFields))]
        public double? CostEstimationThreeMonthes { get; set; }

        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }

        [Display(Name = "Feedback", ResourceType = typeof(resxDbFields))]
        public string Feedback { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "IsVerified", ResourceType = typeof(resxDbFields))]
        public bool IsVerified { get; set; }

        [Display(Name = "VerifiedBy", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> VerifiedBy { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "VerifiedOn", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> VerifiedOn { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public DateTime? DeletedOn { get; set; }

        public byte[] dataCovidUNHCRResponseStrategyRowVersion { get; set; }

    }
}
