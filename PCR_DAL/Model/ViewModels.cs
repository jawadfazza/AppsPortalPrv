using RES_Repo.Globalization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppsPortal.PCR.ViewModels
{
    public class PCRReportParametersList
    {
        public Guid ReportGUID { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string DutyStationDescription { get; set; }
        public string OrganizationInstanceDescription { get; set; }
        public string PartnerCenterDescription { get; set; }
        public string CategoryDescription2 { get; set; }
        public string CategoryDescription3 { get; set; }
    }

    public class PCRReportParametersMultiple
    {
        [Display(Name = "ReportName", ResourceType = typeof(resxDbFields))]
        public string ReportName { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ReportGUID", ResourceType = typeof(resxDbFields))]
        public Guid ReportGUID { get; set; }

        [Display(Name = "StartDate", ResourceType = typeof(resxDbFields))]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "EndDate", ResourceType = typeof(resxDbFields))]
        public DateTime? EndDate { get; set; }

        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public Guid[] DutyStationGUID { get; set; }

        [Display(Name = "OrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
        public Guid[] OrganizationInstanceGUID { get; set; }

        [Display(Name = "PartnerCenterGUID", ResourceType = typeof(resxDbFields))]
        public Guid[] PartnerCenterGUID { get; set; }

        [Display(Name = "CategoryGUID2", ResourceType = typeof(resxDbFields))]
        public Guid[] CategoryGUID2 { get; set; }

        [Display(Name = "CategoryGUID3", ResourceType = typeof(resxDbFields))]
        public Guid[] CategoryGUID3 { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "AggregationGUID", ResourceType = typeof(resxDbFields))]
        public Guid[] AggregationGUID { get; set; }

        [Display(Name = "GenderGUID", ResourceType = typeof(resxDbFields))]
        public Guid[] GenderGUID { get; set; }

        [Display(Name = "AgeGUID", ResourceType = typeof(resxDbFields))]
        public Guid[] AgeGUID { get; set; }

        [Display(Name = "ProfileGUID", ResourceType = typeof(resxDbFields))]
        public Guid[] ProfileGUID { get; set; }
        //Referrals' Reasons

        [Display(Name = "ReferralReasonGUID", ResourceType = typeof(resxDbFields))]
        public Guid[] ReferralReasonGUID { get; set; }

        public Guid[] FindAge(Guid[] ageGUIDs, Guid[] genderGUID)
        {
            List<Guid> Age = new List<Guid>();

            if (genderGUID.Contains(new Guid("DBF9D307-CE9F-4029-BD1E-D7AF6739975C")) && ageGUIDs.Length <= 3)
            {
                if (genderGUID.Contains(new Guid("688B11E0-24FB-44B8-94CE-D8568C4742C7"))) { Age.AddRange(ageGUIDs); }
                foreach (Guid age in AgeGUID)
                {
                    int num = Convert.ToInt32(age.ToString().Substring(34)) + 3;
                    if (num <= 11)
                    {
                        if (num >= 10) { Age.Add(new Guid("00000000-0000-0000-0000-0000000000" + num)); }
                        else { Age.Add(new Guid("00000000-0000-0000-0000-00000000000" + num)); }
                    }
                    else
                    {
                        Age.Add(age);
                    }
                }
            }
            else { Age.AddRange(ageGUIDs); }

            return Age.ToArray();
        }

        public Guid[] FindProfile(Guid[] profileGUID, Guid[] genderGUID)
        {
            List<Guid> Profile = new List<Guid>();

            if (genderGUID.Contains(new Guid("DBF9D307-CE9F-4029-BD1E-D7AF6739975C")) && profileGUID.Length <= 5)
            {
                if (genderGUID.Contains(new Guid("688B11E0-24FB-44B8-94CE-D8568C4742C7"))) { Profile.AddRange(profileGUID); }
                foreach (Guid profile in profileGUID)
                {
                    int num = Convert.ToInt32(profile.ToString().Substring(34)) + 5;
                    if (num >= 21)
                    {
                        Profile.Add(profile);
                    }
                    else
                    {
                        Profile.Add(new Guid("00000000-0000-0000-0000-0000000000" + num));

                    }
                }
            }
            else { Profile.AddRange(profileGUID); }

            return Profile.ToArray();
        }
    }
    public class PartnerCentersDataTableModel
    {
        public System.Guid PartnerCenterGUID { get; set; }
        public string PartnerCenterDescription { get; set; }
        public string OrganizationInstanceDescription { get; set; }
        public string DutyStationDescription { get; set; }
        public System.String OrganizationInstanceGUID { get; set; }
        public System.String DutyStationGUID { get; set; }
        public Nullable<int> Sequence { get; set; }
        public bool Active { get; set; }
        public byte[] codePartnerCenterRowVersion { get; set; }

    }

    public class PartnerCenterUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "PartnerCenterGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid PartnerCenterGUID { get; set; }
        [Display(Name = "Sequence", ResourceType = typeof(resxDbFields))]
        public Nullable<int> Sequence { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "OrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid OrganizationInstanceGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid DutyStationGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "PartnerCenterDescription", ResourceType = typeof(resxDbFields))]
        public string PartnerCenterDescription { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] codePartnerCenterRowVersion { get; set; }
        public byte[] codePartnerCenterLanguageRowVersion { get; set; }
    }

    public class PartnerReportUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "PartnerReportGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid PartnerReportGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "PartnerCenterGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid PartnerCenterGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "OrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid OrganizationInstanceGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid DutyStationGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataPartnerReportRowVersion { get; set; }
    }
    public class PartnerReportsDataTableModel
    {
        public System.Guid PartnerReportGUID { get; set; }
        public System.String OrganizationInstanceGUID { get; set; }
        public System.String DutyStationGUID { get; set; }
        public string PartnerCenterDescription { get; set; }
        public string OrganizationInstanceDescription { get; set; }
        public string DutyStationDescription { get; set; }
        public bool Active { get; set; }
        public byte[] dataPartnerReportRowVersion { get; set; }
    }

    public class PartnerReportCompiledsDataTableModel
    {
        public System.Guid FileReportGUID { get; set; }
        public System.Guid PartnerReportGUID { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string ReportDescription { get; set; }
        public bool ShowReport { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<System.DateTime> UploadDateTime { get; set; }
        public string UploadByUser{ get; set; }
        public bool? ErrorFound { get; set; }
        public bool Active { get; set; }
        public byte[] dataFileReportRowVersion { get; set; }
    }

    public class ReportsVisibilityModel
    {

        public Guid PartnerGUID_forPublish { get; set; }
        public List<Guid> CenterGUIDs_forPublish { get; set; }
        public DateTime EndDate_forPublish { get; set; }
        public bool ShowReport_forPublish { get; set; }
    }
}
