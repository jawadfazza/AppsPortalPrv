using RES_Repo.Globalization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWS_DAL.Model
{
    public class PartnersContributionsDataTableModel
    {
        public Guid PartnerContributionGUID { get; set; }
        public string AgencyGUID { get; set; }
        public string AgencyDescription { get; set; }
        public string ImplementingPartnerGUID { get; set; }
        public string ImplementingPartnerDescription { get; set; }
        public string LocationLong { get; set; }
        public int ReportingMonth { get; set; }
        public string HRPProject { get; set; }
        public string ActivityLong { get; set; }
        public string ActivityDetails { get; set; }
        public string DeliveryMode { get; set; }

        public string FacilityGUID { get; set; }
        public string DeliveryFacility { get; set; }
        public string FWSUnit { get; set; }
        public string FWSAnalysisUnit { get; set; }
        public string BeneficiariesTypeGUID { get; set; }
        public string BeneficiariesTypeDescription { get; set; }
        public string PWD { get; set; }
        public string SurvivorsOfExplosiveHazards { get; set; }
        public int TotalReach { get; set; }
        public string SumBreakdown { get; set; }
        public string Girls { get; set; }
        public string Boys { get; set; }
        public string AdolescentGirls { get; set; }
        public string AdolescentBoys { get; set; }
        public string Women { get; set; }
        public string Men { get; set; }
        public string ElderlyWomen { get; set; }
        public string ElderlyMen { get; set; }
        public string SumBreakdownNew { get; set; }
        public string GirlsNew { get; set; }
        public string BoysNew { get; set; }
        public string AdolescentGirlsNew { get; set; }
        public string AdolescentBoysNew { get; set; }
        public string WomenNew { get; set; }
        public string MenNew { get; set; }
        public string ElderlyWomenNew { get; set; }
        public string ElderlyMenNew { get; set; }
        public string Admin1Code { get; set; }
        public string Admin2Code { get; set; }
        public string Admin3Code { get; set; }
        public string Admin4Code { get; set; }
        public string LocationCode { get; set; }
        public string Admin1Name { get; set; }
        public string Admin2Name { get; set; }
        public string Admin3Name { get; set; }
        public string Admin4Name { get; set; }
        public string LocationName { get; set; }
        public string FWSSubSector { get; set; }
        public string FWSActivity { get; set; }
        public string FWSSubActivity { get; set; }
        public bool IsDataApproved { get; set; }
        public Guid? ApprovedBy { get; set; }
        public string ApprovedByName { get; set; }
        public DateTime? ApprovedOn { get; set; }
        public bool Active { get; set; }
        public byte[] dataPartnerContributionRowVersion { get; set; }
    }

    public class PartnersContributionsUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "PartnerContributionGUID", ResourceType = typeof(resxDbFields))]
        public Guid PartnerContributionGUID { get; set; }

        [Display(Name = "PartnerContributionFileGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> PartnerContributionFileGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "AgencyGUID", ResourceType = typeof(resxDbFields))]
        public Guid AgencyGUID { get; set; }

        [Display(Name = "ImplementingPartnerGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ImplementingPartnerGUID { get; set; }

        [Display(Name = "HubGUID", ResourceType = typeof(resxDbFields))]
        public Guid HubGUID { get; set; }

        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "LocationGUID", ResourceType = typeof(resxDbFields))]
        public Guid LocationGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(500, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "LocationLong", ResourceType = typeof(resxDbFields))]
        public string LocationLong { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ReportingMonth", ResourceType = typeof(resxDbFields))]
        public int ReportingMonth { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ReportingYear", ResourceType = typeof(resxDbFields))]
        public int ReportingYear { get; set; }


        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "HRPProject", ResourceType = typeof(resxDbFields))]
        public bool HRPProject { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ActivityLong", ResourceType = typeof(resxDbFields))]
        public string ActivityLong { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ActivityDetails", ResourceType = typeof(resxDbFields))]
        public string ActivityDetails { get; set; }

        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DeliveryMode", ResourceType = typeof(resxDbFields))]
        public string DeliveryMode { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DeliveryMode", ResourceType = typeof(resxDbFields))]
        public Guid DeliveryModeGUID { get; set; }

        [Display(Name = "FacilityGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid? FacilityGUID { get; set; }

        [StringLength(500, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DeliveryFacility", ResourceType = typeof(resxDbFields))]
        public string DeliveryFacility { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "FWSUnit", ResourceType = typeof(resxDbFields))]
        public string FWSUnit { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "FWSAnalysisUnit", ResourceType = typeof(resxDbFields))]
        public string FWSAnalysisUnit { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "BeneficiariesTypeGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid BeneficiariesTypeGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "PWD", ResourceType = typeof(resxDbFields))]
        public bool PWD { get; set; }

        [Display(Name = "SurvivorsOfExplosiveHazards", ResourceType = typeof(resxDbFields))]
        public Nullable<bool> SurvivorsOfExplosiveHazards { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "TotalReach", ResourceType = typeof(resxDbFields))]
        public int TotalReach { get; set; }


        [Display(Name = "SumBreakdown", ResourceType = typeof(resxDbFields))]
        public Nullable<int> SumBreakdown { get; set; }

        [Display(Name = "Girls", ResourceType = typeof(resxDbFields))]
        public Nullable<int> Girls { get; set; }

        [Display(Name = "Boys", ResourceType = typeof(resxDbFields))]
        public Nullable<int> Boys { get; set; }

        [Display(Name = "AdolescentGirls", ResourceType = typeof(resxDbFields))]
        public Nullable<int> AdolescentGirls { get; set; }

        [Display(Name = "AdolescentBoys", ResourceType = typeof(resxDbFields))]
        public Nullable<int> AdolescentBoys { get; set; }

        [Display(Name = "Women", ResourceType = typeof(resxDbFields))]
        public Nullable<int> Women { get; set; }

        [Display(Name = "Men", ResourceType = typeof(resxDbFields))]
        public Nullable<int> Men { get; set; }

        [Display(Name = "ElderlyWomen", ResourceType = typeof(resxDbFields))]
        public Nullable<int> ElderlyWomen { get; set; }

        [Display(Name = "ElderlyMen", ResourceType = typeof(resxDbFields))]
        public Nullable<int> ElderlyMen { get; set; }

        [Display(Name = "SumBreakdownNew", ResourceType = typeof(resxDbFields))]
        public Nullable<int> SumBreakdownNew { get; set; }

        [Display(Name = "GirlsNew", ResourceType = typeof(resxDbFields))]
        public Nullable<int> GirlsNew { get; set; }

        [Display(Name = "BoysNew", ResourceType = typeof(resxDbFields))]
        public Nullable<int> BoysNew { get; set; }

        [Display(Name = "AdolescentGirlsNew", ResourceType = typeof(resxDbFields))]
        public Nullable<int> AdolescentGirlsNew { get; set; }

        [Display(Name = "AdolescentBoysNew", ResourceType = typeof(resxDbFields))]
        public Nullable<int> AdolescentBoysNew { get; set; }

        [Display(Name = "WomenNew", ResourceType = typeof(resxDbFields))]
        public Nullable<int> WomenNew { get; set; }

        [Display(Name = "MenNew", ResourceType = typeof(resxDbFields))]
        public Nullable<int> MenNew { get; set; }

        [Display(Name = "ElderlyWomenNew", ResourceType = typeof(resxDbFields))]
        public Nullable<int> ElderlyWomenNew { get; set; }

        [Display(Name = "ElderlyMenNew", ResourceType = typeof(resxDbFields))]
        public Nullable<int> ElderlyMenNew { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Admin1Code", ResourceType = typeof(resxDbFields))]
        public string Admin1Code { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Admin2Code", ResourceType = typeof(resxDbFields))]
        public string Admin2Code { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Admin3Code", ResourceType = typeof(resxDbFields))]
        public string Admin3Code { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Admin4Code", ResourceType = typeof(resxDbFields))]
        public string Admin4Code { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(500, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "LocationCode", ResourceType = typeof(resxDbFields))]
        public string LocationCode { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Admin1Name", ResourceType = typeof(resxDbFields))]
        public string Admin1Name { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Admin2Name", ResourceType = typeof(resxDbFields))]
        public string Admin2Name { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Admin3Name", ResourceType = typeof(resxDbFields))]
        public string Admin3Name { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Admin4Name", ResourceType = typeof(resxDbFields))]
        public string Admin4Name { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "LocationName", ResourceType = typeof(resxDbFields))]
        public string LocationName { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SubSectorGUID", ResourceType = typeof(resxDbFields))]
        public Guid SubSectorGUID { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SubSectorID", ResourceType = typeof(resxDbFields))]
        public string SubSectorID { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "FWSSubSector", ResourceType = typeof(resxDbFields))]
        public string FWSSubSector { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "FWSActivity", ResourceType = typeof(resxDbFields))]
        public string FWSActivity { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "FWSSubActivity", ResourceType = typeof(resxDbFields))]
        public string FWSSubActivity { get; set; }

        [Display(Name = "IsDataApproved", ResourceType = typeof(resxDbFields))]
        public bool IsDataApproved { get; set; }

        [Display(Name = "ApprovedBy", ResourceType = typeof(resxDbFields))]
        public Guid? ApprovedBy { get; set; }

        [Display(Name = "ApprovedByName", ResourceType = typeof(resxDbFields))]
        public string ApprovedByName { get; set; }

        [Display(Name = "ApprovedOn", ResourceType = typeof(resxDbFields))]
        public DateTime? ApprovedOn { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataPartnerContributionRowVersion { get; set; }
    }

    public class PartnersContributionsUploadFormModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ReportingMonth", ResourceType = typeof(resxDbFields))]
        public string ReportingMonth { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ReportingYear", ResourceType = typeof(resxDbFields))]
        public int ReportingYear { get; set; }
    }
}
