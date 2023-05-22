using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.ViewModels;
using AutoMapper;
using FineUploader;
using iTextSharp.text.pdf.qrcode;
using LinqKit;

using Microsoft.Reporting.WebForms;

using ORG_DAL.Model;
using RES_Repo.Globalization;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel.Activities;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;


using static AppsPortal.Library.DataTableNames;

using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.Text;
using static AppsPortal.Library.LookupTables;
using System.DirectoryServices;

namespace AppsPortal.Areas.ORG.Controllers
{
    public class StaffProfileController : ORGBaseController
    {
        // GET: ORG/StaffProfile
        public ActionResult Index()
        {

            if (!CMS.HasAction(Permissions.StaffProfile.Update, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            return View();
        }
        [Route("ORG/StaffProfileICT")]
        public ActionResult IndexICT()
        {

            if (!CMS.HasAction(Permissions.StaffProfile.Update, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }

            return View("~/Areas/ORG/Views/StaffProfile/IndexICT.cshtml");
        }
        #region Staff Account
        public ActionResult SyncData()
        {
            var asd = (from a in DbCMS.userAccounts.Where(x => x.Active) select a).ToList();
            List<StaffCoreData> cores = new List<StaffCoreData>();
            foreach (var item in asd)
            {
                StaffCoreData staffCoreData = new StaffCoreData();
                staffCoreData.UserGUID = item.UserGUID;

                var serviceHistory = item.userServiceHistory.FirstOrDefault();
                staffCoreData.EmailAddress = serviceHistory.EmailAddress;
                var found = (from a in DbCMS.StaffCoreData.Where(x => x.Active && x.EmailAddress == staffCoreData.EmailAddress) select a).FirstOrDefault();
                if (found != null) { continue; }
                var userProfile = serviceHistory.userProfiles.OrderByDescending(x => x.FromDate).FirstOrDefault();
                staffCoreData.DutyStationGUID = userProfile.DutyStationGUID.HasValue ? userProfile.DutyStationGUID.Value : Guid.Parse("FD4A8AE4-EB97-46B5-AED1-45B6ADD86D18");
                staffCoreData.DepartmentGUID = userProfile.DepartmentGUID.HasValue ? userProfile.DepartmentGUID.Value : Guid.Parse("90E12C9C-C113-49F5-ADF1-BD0140772E2B");
                staffCoreData.JobTitleGUID = userProfile.JobTitleGUID.HasValue ? userProfile.JobTitleGUID.Value : Guid.Parse("6FCE11E5-43F6-4A6D-B692-208D4F8B1740");

                var personalD = (from a in DbORG.userPersonalDetails.Where(x => x.UserGUID == item.UserGUID) select a).FirstOrDefault();
                if (personalD.CountryGUID.HasValue)
                {
                    staffCoreData.NationalityGUID = personalD.CountryGUID;
                }

                cores.Add(staffCoreData);
            }
            DbORG.StaffCoreData.AddRange(cores);
            try { DbORG.SaveChanges(); }
            catch (Exception ex) { }

            return null;
        }

        public ActionResult StaffProfileIndex()
        {

            if (!CMS.HasAction(Permissions.StaffProfile.Update, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            return View("~/Areas/ORG/Views/StaffProfile/Index.cshtml");
        }

        [Route("ORG/StaffProfile/StaffProfileDataTable")]
        public ActionResult StaffProfileDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<StaffProfileDataTable, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<StaffProfileDataTable>(DataTable.Filters);
            }
            var All = (from a in DbORG.StaffCoreData.AsExpandable().Where(x => x.Active && (x.IsOldRecord != true))
                       join b in DbORG.userPersonalDetails.AsExpandable().Where(x => x.Active) on a.UserGUID equals b.UserGUID into LJ1
                       from R1 in LJ1.DefaultIfEmpty()
                       join c in DbORG.userPersonalDetailsLanguage.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on a.UserGUID equals c.UserGUID into LJ2
                       from R2 in LJ2.DefaultIfEmpty()
                       join d in DbORG.codeDutyStationsLanguages.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on a.DutyStationGUID equals d.DutyStationGUID into LJ3
                       from R3 in LJ3.DefaultIfEmpty()
                       join e in DbORG.codeDepartmentsLanguages.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on a.DepartmentGUID equals e.DepartmentGUID into LJ4
                       from R4 in LJ4.DefaultIfEmpty()
                       join f in DbORG.codeJobTitlesLanguages.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on a.JobTitleGUID equals f.JobTitleGUID into LJ5
                       from R5 in LJ5.DefaultIfEmpty()
                       join k in DbORG.codeTablesValuesLanguages.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on a.RecruitmentTypeGUID equals k.ValueGUID into LJ6
                       from R6 in LJ6.DefaultIfEmpty()
                       join l in DbORG.codeTablesValuesLanguages.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on a.StaffStatusGUID equals l.ValueGUID into LJ7
                       from R7 in LJ7.DefaultIfEmpty()
                       join m in DbORG.codeTablesValuesLanguages.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on a.ContractTypeGUID equals m.ValueGUID into LJ8
                       from R8 in LJ8.DefaultIfEmpty()
                       select new StaffProfileDataTable
                       {
                           UserGUID = a.UserGUID.ToString(),
                           AccessLevel = 1,
                           LastDepartureDate = a.LastDepartureDate,
                           StaffStatus = R7.ValueDescription,
                           StaffStatusGUID = R7.ValueGUID.ToString(),
                           StaffName = R2.FirstName + " " + R2.Surname,
                           RecruitmentType = R6.ValueDescription,
                           RecruitmentTypeGUID = R6.ValueGUID.ToString(),
                           EmailAddress = a.EmailAddress,
                           EmploymentID = a.EmploymentID.ToString(),
                           StaffPhoto = a.StaffPhoto,
                           
                           OfficeTypeGUID = a.OfficeTypeGUID.ToString(),
                           JobTitleGUID = a.JobTitleGUID.ToString(),
                           JobTitleDescription = R5.JobTitleDescription,
                           DutyStationGUID = a.DutyStationGUID.ToString(),
                           DutyStationDescription = R3.DutyStationDescription,
                           DepartmentGUID = a.DepartmentGUID.ToString(),
                           DepartmentDescription = R4.DepartmentDescription,
                           ContractTypeGUID = R8.ValueGUID.ToString(),
                           ContractType = R8.ValueDescription,
                           StaffGradeGUID = a.StaffGradeGUID.ToString(),
                           LastConfirmationStatus = a.LastConfirmationStatus,
                           LastConfirmationStatusGUID = a.LastConfirmationStatusGUID.ToString(),
                           ContractEndDate = a.ContractEndDate,
                           ReportToGUID = a.ReportToGUID.ToString(),

                           StaffEOD = a.StaffEOD,
                           Gender = R1.GenderGUID.ToString(),
                           Active = a.Active,
                           StaffCoreDataRowVersion = a.StaffCoreDataRowVersion,
                       }).Distinct().Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<StaffProfileDataTable> Result = Mapper.Map<List<StaffProfileDataTable>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("ORG/StaffProfile/Create")]
        public ActionResult StaffProfileCreate()
        {

            if (!CMS.HasAction(Permissions.StaffProfileHRSection.Create, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            StaffProfileUpdateModel model = new StaffProfileUpdateModel();
            Random rnd = new Random();

            var check = string.Concat(9999, rnd.Next(50), rnd.Next(10));
            model.EmploymentID = int.Parse(check);


            return View("~/Areas/ORG/Views/StaffProfile/StaffProfile.cshtml", null, model);
        }


        [Route("ORG/StaffProfile/Update/{PK}")]
        public ActionResult StaffProfileUpdate(Guid PK)
        {


            if (!CMS.HasAction(Permissions.StaffProfile.Update, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }                //return null;
            StaffCoreData staffCoreData = (from a in DbORG.StaffCoreData.AsNoTracking().Where(x => x.UserGUID == PK && x.Active)
                                           select a).FirstOrDefault();
            

            StaffProfileUpdateModel model = new StaffProfileUpdateModel();
            userPersonalDetails userPersonalDetails = DbORG.userPersonalDetails.AsNoTracking().Where(x => x.UserGUID == PK && x.Active).FirstOrDefault();
            // (from a in DbORG.userPersonalDetails.AsNoTracking().Where(x => x.UserGUID == PK && x.Active) select a).FirstOrDefault();
            List<userPersonalDetailsLanguage> userPersonalDetailsLanguages = userPersonalDetails.userPersonalDetailsLanguage.ToList();
            model.UserGUID = staffCoreData.UserGUID;
            model.MediaPath = new Portal().GetUserProfilePhoto(model.UserGUID);

            model.StaffStatus = staffCoreData.StaffStatus.Value;
            model.JobTitleGUID = staffCoreData.JobTitleGUID;
            model.DutyStationGUID = staffCoreData.DutyStationGUID;
            model.OfficeTypeGUID = staffCoreData.OfficeTypeGUID;
            if (staffCoreData.DepartmentGUID != Guid.Empty && staffCoreData.DepartmentGUID != null)
            {
                model.DepartmentGUID = (Guid)staffCoreData.DepartmentGUID;


            }
            var _staffPositions = DbORG.dataStaffPosition.Where(x => x.UserGUID == model.UserGUID).Select(x => x.PositionGUID).ToList();
            var _tablePositions = DbORG.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && _staffPositions.Contains(x.ValueGUID)).ToList();

            model.showPositions = " ";

            foreach (var item in _staffPositions)
            {
                model.showPositions += _tablePositions.Where(x => x.ValueGUID == item).FirstOrDefault().ValueDescription + " ";

            }
            if (_staffPositions.Count > 0)
            {
                model.AllStaffPositionsGUID.AddRange(_staffPositions);
            }
            model.ReportToGUID = staffCoreData.ReportToGUID;
            model.GenderGUID = userPersonalDetails.GenderGUID;
            model.ICTComments = staffCoreData.ICTComments;
            //model.GenderGUID = staffCoreData.;
            model.NationalityGUID = staffCoreData.NationalityGUID;
            model.Nationality1Arabic = model.NationalityGUID != null ? (DbAHD.codeCountriesLanguages.Where(x => x.CountryGUID == model.NationalityGUID && x.LanguageID == "AR").FirstOrDefault() != null ? DbAHD.codeCountriesLanguages.Where(x => x.CountryGUID == model.NationalityGUID && x.LanguageID == "AR").FirstOrDefault().CountryDescription : "") : "";
            model.Nationality2Arabic = model.Nationality2GUID != null ? (DbAHD.codeCountriesLanguages.Where(x => x.CountryGUID == model.Nationality2GUID && x.LanguageID == "AR").FirstOrDefault() != null ? DbAHD.codeCountriesLanguages.Where(x => x.CountryGUID == model.Nationality2GUID && x.LanguageID == "AR").FirstOrDefault().CountryDescription : "") : "";
            model.Nationality3Arabic = model.Nationality3GUID != null ? (DbAHD.codeCountriesLanguages.Where(x => x.CountryGUID == model.Nationality3GUID && x.LanguageID == "AR").FirstOrDefault() != null ? DbAHD.codeCountriesLanguages.Where(x => x.CountryGUID == model.Nationality3GUID && x.LanguageID == "AR").FirstOrDefault().CountryDescription : "") : "";
            //model.AssignmentType = staffCoreData.AssignmentType;
            model.PositionInOrganigram = staffCoreData.PositionInOrganigram;
            model.SyrianNationalIDNumber = staffCoreData.SyrianNationalIDNumber;
            model.DateOfBirth = staffCoreData.DateOfBirth;
            model.LastDepartureDate = staffCoreData.LastDepartureDate;
            model.PlaceOfBirthGUID = staffCoreData.PlaceOfBirthGUID;
            model.PlaceOfBirthCountryAR = model.PlaceOfBirthGUID != null ? (DbAHD.codeCountriesLanguages.Where(x => x.CountryGUID == model.PlaceOfBirthGUID && x.LanguageID == "AR").FirstOrDefault() != null ? DbAHD.codeCountriesLanguages.Where(x => x.CountryGUID == model.PlaceOfBirthGUID && x.LanguageID == "AR").FirstOrDefault().CountryDescription : "") : "";
            model.StaffPhoto = staffCoreData.StaffPhoto;
            model.NationalPassportPhoto = staffCoreData.NationalPassportPhoto;
            model.SyrianNationalIDPhoto = staffCoreData.SyrianNationalIDPhoto;
            model.NationalPassportPhoto = staffCoreData.NationalPassportPhoto;
            model.RecruitmentTypeGUID = staffCoreData.RecruitmentTypeGUID;
            model.UNLPPhoto = staffCoreData.UNLPPhoto;
            model.UNLPNumber = staffCoreData.UNLPNumber;
            model.UNLPDateOfIssue = staffCoreData.UNLPDateOfIssue;
            model.UNLPDateOfExpiry = staffCoreData.UNLPDateOfExpiry;
            //model.DangerPay = staffCoreData.DangerPay;
            model.EmailAddress = staffCoreData.EmailAddress;
            model.BloodGroup = staffCoreData.BloodGroup;
            model.NationalityGUID = staffCoreData.NationalityGUID;
            model.CallSign = staffCoreData.CallSign;
            model.EmploymentID = (int?)staffCoreData.EmploymentID;
            model.StaffPrefix = staffCoreData.StaffPrefix;
            model.ExpiryOfResidencyVisa = staffCoreData.ExpiryOfResidencyVisa;
            model.PaymentEligibilityStatusGUID = staffCoreData.PaymentEligibilityStatusGUID;
            model.PlaceOfBirthGUID = staffCoreData.PlaceOfBirthGUID;
            model.LastJobEn = staffCoreData.LastJobEn;
            model.LastJobAr = staffCoreData.LastJobAr;
            model.PlaceBirthCityEn = staffCoreData.PlaceBirthCityEn;
            model.PlaceBirthCityAr = staffCoreData.PlaceBirthCityAr;
            model.UNHCRIDNumber = staffCoreData.UNHCRIDNumber;
            model.NationalPassportNumber = staffCoreData.NationalPassportNumber;
            model.NationalPassportDateOfIssue = staffCoreData.NationalPassportDateOfIssue;
            model.NationalPassportNumber = staffCoreData.NationalPassportNumber;
            model.ContractTypeGUID = staffCoreData.ContractTypeGUID;
            model.StaffEOD = staffCoreData.StaffEOD;
            model.DegreeAr = staffCoreData.DegreeAr;
            model.DegreeEn = staffCoreData.DegreeEn;
            model.StaffPrefixGUID = staffCoreData.StaffPrefixGUID;
            model.StaffStatusGUID = staffCoreData.StaffStatusGUID;
            model.Nationality2GUID = staffCoreData.Nationality2GUID;
            model.Nationality3GUID = staffCoreData.Nationality3GUID;
            model.OfficeRoomNumberBuilding = staffCoreData.OfficeRoomNumberBuilding;
            model.TitleEnglishMSRP = staffCoreData.TitleEnglishMSRP;
            model.TitleArabicMSRP = staffCoreData.TitleArabicMSRP;
            model.JobTitleAR = model.JobTitleGUID != null ? (DbAHD.codeJobTitlesLanguages.Where(x => x.JobTitleGUID == model.JobTitleGUID && x.LanguageID == "AR").FirstOrDefault() != null ? (model.StaffPrefixGUID == Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E3772") ? DbAHD.codeJobTitlesLanguages.Where(x => x.JobTitleGUID == model.JobTitleGUID && x.LanguageID == "AR").FirstOrDefault().JobTitleDescriptionFemale : DbAHD.codeJobTitlesLanguages.Where(x => x.JobTitleGUID == model.JobTitleGUID && x.LanguageID == "AR").FirstOrDefault().JobTitleDescription) : "") : "";
            model.StaffGradeGUID = staffCoreData.StaffGradeGUID;
            model.ContractStartDate = staffCoreData.ContractStartDate;
            model.ContractEndDate = staffCoreData.ContractEndDate;
            model.PositionNumber = staffCoreData.PositionNumber;
            model.StaffPositionGUID = staffCoreData.StaffPositionGUID;
            model.PermanentAddressEn = staffCoreData.PermanentAddressEn;
            model.PermanentAddressAr = staffCoreData.PermanentAddressAr;
            model.CurrentAddressEn = staffCoreData.CurrentAddressEn;
            model.CurrentAddressAr = staffCoreData.CurrentAddressAr;
            model.HomeTelephoneNumberLandline = staffCoreData.HomeTelephoneNumberLandline;
            model.SATPhoneNumber = staffCoreData.SATPhoneNumber;
            model.HomeTelephoneNumberMobile = staffCoreData.HomeTelephoneNumberMobile;
            model.OfficialMobileNumber = staffCoreData.OfficialMobileNumber;
            model.OfficialExtensionNumber = staffCoreData.OfficialExtensionNumber;
            model.DamascusExtensionNumber = staffCoreData.DamascusExtensionNumber;
            model.HQExtensionNumber = staffCoreData.HQExtensionNumber;
            model.KinMobileNumber = staffCoreData.KinMobileNumber;
            model.NextOfKinName = staffCoreData.NextOfKinName;
            model.BSAFECertAcquired = staffCoreData.BSAFECertAcquired;
            model.BSAFEExpiryDate = staffCoreData.BSAFEExpiryDate;
            model.NumberOfDependants = staffCoreData.NumberOfDependants;
            model.DependantsName = staffCoreData.DependantsName;
            model.Active = staffCoreData.Active;
            model.StaffCoreDataRowVersion = staffCoreData.StaffCoreDataRowVersion;
            model.FirstNameEN = userPersonalDetailsLanguages.Where(x => x.LanguageID == "EN").FirstOrDefault().FirstName;
            model.SurnameEN = userPersonalDetailsLanguages.Where(x => x.LanguageID == "EN").FirstOrDefault().Surname;
            model.FatherNameEN = userPersonalDetailsLanguages.Where(x => x.LanguageID == "EN").FirstOrDefault().FatherName;
            model.MotherNameEN = userPersonalDetailsLanguages.Where(x => x.LanguageID == "EN").FirstOrDefault().MotherName;
            model.FirstNameAR = userPersonalDetailsLanguages.Where(x => x.LanguageID == "AR").FirstOrDefault() != null ? userPersonalDetailsLanguages.Where(x => x.LanguageID == "AR").FirstOrDefault().FirstName : "";
            model.SurnameAR = userPersonalDetailsLanguages.Where(x => x.LanguageID == "AR").FirstOrDefault() != null ? userPersonalDetailsLanguages.Where(x => x.LanguageID == "AR").FirstOrDefault().Surname : "";
            model.FatherNameAR = userPersonalDetailsLanguages.Where(x => x.LanguageID == "AR").FirstOrDefault() != null ? userPersonalDetailsLanguages.Where(x => x.LanguageID == "AR").FirstOrDefault().FatherName : "";
            model.MotherNameAR = userPersonalDetailsLanguages.Where(x => x.LanguageID == "AR").FirstOrDefault() != null ? userPersonalDetailsLanguages.Where(x => x.LanguageID == "AR").FirstOrDefault().MotherName : "";


            Guid hrAccessGUID = Guid.Parse("788F6BCD-44CE-41B5-BAED-F971E7816B93");
            Guid adminAccessGUID = Guid.Parse("4EDD5DBB-3348-4C58-B246-0CFB46D8B207");
            Guid finainceAccessGUID = Guid.Parse("F8C4523B-FF7F-4FF4-B445-490A48E29920");
            Guid ictAccessGUID = Guid.Parse("6A32EBE4-1AAA-4169-8EC0-209C3774446C");
            Guid SecurityAccessGUID = Guid.Parse("30CF6EB8-3690-4D78-AC3D-D5C9291DB2AF");

            var userperm = DbORG.userPermissions.Where(x => x.UserProfileGUID == UserProfileGUID && x.Active).ToList();
            var total = userperm.Where(x => x.ActionGUID == hrAccessGUID).ToList();
            var totalfin = userperm.Where(x => x.ActionGUID == finainceAccessGUID).ToList();
            var totalAdmin = userperm.Where(x => x.ActionGUID == adminAccessGUID).ToList();
            var totalICT = userperm.Where(x => x.ActionGUID == ictAccessGUID).ToList();
            var totalSecuriry = userperm.Where(x => x.ActionGUID == SecurityAccessGUID).ToList();
            if (total.Count() > 0)
            {
                Guid unhcrMangerperm = Guid.Parse("9001ba39-6996-4042-a651-36f68912c5a7");
                Guid unopsMangerPerm = Guid.Parse("b49a0e2f-7819-4558-b165-473e4fc19630");
                if (staffCoreData.ContractTypeGUID == Guid.Parse("1506F90E-F377-49B6-A8CD-116250246A44") && userperm.Where(x => x.ActionGUID == unopsMangerPerm).Count() <= 0)
                {
                    return Json(DbORG.PermissionError());

                }

                if (staffCoreData.ContractTypeGUID != Guid.Parse("1506F90E-F377-49B6-A8CD-116250246A44") && userperm.Where(x => x.ActionGUID == unhcrMangerperm).Count() <= 0)
                {
                    return Json(DbORG.PermissionError());

                }


                return View("~/Areas/ORG/Views/StaffProfile/StaffProfile.cshtml", null, model);
            }

            else if (totalAdmin.Count() > 0)
            {
                return View("~/Areas/ORG/Views/StaffProfile/AdminStaffProfile.cshtml", null, model);

            }


            else if (totalfin.Count() > 0)
            {
                return View("~/Areas/ORG/Views/StaffProfile/FinanceStaffProfile.cshtml", null, model);
            }

            else if (totalICT.Count() > 0)
            {
                if (staffCoreData.DepartmentGUID != Guid.Empty && staffCoreData.DepartmentGUID != null)
                {
                    model.Department = DbORG.codeDepartmentsLanguages.Where(x => x.DepartmentGUID == staffCoreData.DepartmentGUID && x.LanguageID == LAN).FirstOrDefault().DepartmentDescription;
                }
                if (staffCoreData.DutyStationGUID != Guid.Empty && staffCoreData.DutyStationGUID != null)
                {
                    model.DutyStation = DbORG.codeDutyStationsLanguages.Where(x => x.DutyStationGUID == staffCoreData.DutyStationGUID && x.LanguageID == LAN).FirstOrDefault().DutyStationDescription;
                }
                if (staffCoreData.StaffPositionGUID != Guid.Empty && staffCoreData.StaffPositionGUID != null)
                {
                    model.PositionName = DbORG.codeTablesValuesLanguages.Where(x => x.ValueGUID == staffCoreData.StaffPositionGUID && x.LanguageID == LAN).FirstOrDefault().ValueDescription;
                }

                if (staffCoreData.JobTitleGUID != Guid.Empty && staffCoreData.JobTitleGUID != null)
                {
                    model.JobTitle = DbORG.codeJobTitlesLanguages.Where(x => x.JobTitleGUID == staffCoreData.JobTitleGUID && x.LanguageID == LAN).FirstOrDefault().JobTitleDescription;
                }
                return View("~/Areas/ORG/Views/StaffProfile/ICTStaffProfile.cshtml", null, model);
            }

            else if (totalSecuriry.Count > 0)
            {
               
                return View("~/Areas/ORG/Views/StaffSecurityInformation/SecurityStaffProfile.cshtml", null, model);
            }

            //int _accessLevel= total.Count()>0?1: (totalAdmin.Count>0?3: (totalfin.Count>0?4:2));

            else
            {
                return Json(DbORG.PermissionError());

            }


        }
        [HttpPost,ValidateAntiForgeryToken]
        public ActionResult StaffProfileUpdate(StaffProfileUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.StaffProfileHRSection.Update, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            if (!ModelState.IsValid || ActiveStaff(model)) return PartialView("~/Areas/ORG/Views/StaffProfile/_StaffProfileForm.cshtml", model);
            if (model.EmploymentID == null)
            {
                return Json(DbORG.PermissionError());
            }

            if (!string.IsNullOrEmpty(model.HomeTelephoneNumberMobile))
            {
                string _checkPrivate = model.HomeTelephoneNumberMobile.Substring(0, 4);
                if (_checkPrivate != "+963")
                {
                    return PartialView("~/Areas/ORG/Views/StaffProfile/_StaffProfileForm.cshtml", model);
                }
            }

            DateTime ExecutionTime = DateTime.Now;

            StaffCoreData staffCoreData = (from a in DbORG.StaffCoreData.Where(x => x.Active && x.UserGUID == model.UserGUID) select a).FirstOrDefault();
            var _myCurrPosition = staffCoreData.StaffPositionGUID;


            userPersonalDetails userPersonalDetails = (from a in DbORG.userPersonalDetails.Where(x => x.Active && x.UserGUID == model.UserGUID) select a).FirstOrDefault();
            userPersonalDetailsLanguage userPersonalDetailsLanguageEN = (from a in DbORG.userPersonalDetailsLanguage.Where(x => x.Active && x.UserGUID == model.UserGUID && x.LanguageID == "EN") select a).FirstOrDefault();
            userPersonalDetailsLanguage userPersonalDetailsLanguageAR = (from a in DbORG.userPersonalDetailsLanguage.Where(x => x.Active && x.UserGUID == model.UserGUID && x.LanguageID == "AR") select a).FirstOrDefault();


            staffCoreData.JobTitleGUID = model.JobTitleGUID.Value;
            staffCoreData.DutyStationGUID = model.DutyStationGUID;
            staffCoreData.OfficeTypeGUID = model.OfficeTypeGUID;
            staffCoreData.DepartmentGUID = (Guid)model.DepartmentGUID;
            staffCoreData.StaffStatus = model.StaffStatus;
            staffCoreData.ReportToGUID = model.ReportToGUID;
            staffCoreData.NationalityGUID = model.NationalityGUID;
            staffCoreData.RecruitmentTypeGUID = model.RecruitmentTypeGUID;
            staffCoreData.Active = true;
            staffCoreData.DateOfBirth = model.DateOfBirth;
            staffCoreData.LastDepartureDate = model.LastDepartureDate;
            staffCoreData.PlaceOfBirthGUID = model.PlaceOfBirthGUID;



            staffCoreData.StaffPrefix = model.StaffPrefix;
            staffCoreData.StaffPrefixGUID = model.StaffPrefixGUID;
            staffCoreData.SyrianNationalIDNumber = model.SyrianNationalIDNumber;
            staffCoreData.LastJobEn = model.LastJobEn;
            staffCoreData.LastJobAr = model.LastJobAr;
            staffCoreData.PlaceBirthCityEn = model.PlaceBirthCityEn;
            staffCoreData.PlaceBirthCityAr = model.PlaceBirthCityAr;
            staffCoreData.UNLPNumber = model.UNLPNumber;
            staffCoreData.UNLPDateOfIssue = model.UNLPDateOfIssue;
            staffCoreData.UNLPDateOfExpiry = model.UNLPDateOfExpiry;
            staffCoreData.UNHCRIDNumber = model.UNHCRIDNumber;
            staffCoreData.NationalPassportNumber = model.NationalPassportNumber;
            staffCoreData.NationalPassportDateOfIssue = model.NationalPassportDateOfIssue;
            staffCoreData.ContractTypeGUID = model.ContractTypeGUID;
            staffCoreData.PositionNumber = model.PositionNumber;
            staffCoreData.StaffPositionGUID = model.StaffPositionGUID;
            staffCoreData.StaffEOD = model.StaffEOD;
            staffCoreData.DegreeAr = model.DegreeAr;
            staffCoreData.DegreeEn = model.DegreeEn;
            staffCoreData.StaffStatusGUID = model.StaffStatusGUID;
            staffCoreData.BloodGroup = model.BloodGroup;
            staffCoreData.OfficeRoomNumberBuilding = model.OfficeRoomNumberBuilding;


            staffCoreData.TitleEnglishMSRP = model.TitleEnglishMSRP;
            staffCoreData.TitleArabicMSRP = model.TitleArabicMSRP;
            //staffCoreData.TitleEnglishMOFA = model.TitleEnglishMOFA;
            //staffCoreData.TitleArabicMOFA = model.TitleArabicMOFA;


            staffCoreData.StaffGradeGUID = model.StaffGradeGUID;
            staffCoreData.ContractStartDate = model.ContractStartDate;
            staffCoreData.ContractEndDate = model.ContractEndDate;

            staffCoreData.PermanentAddressEn = model.PermanentAddressEn;
            staffCoreData.PermanentAddressAr = model.PermanentAddressAr;
            staffCoreData.CurrentAddressEn = model.CurrentAddressEn;
            staffCoreData.CurrentAddressAr = model.CurrentAddressAr;
            staffCoreData.HomeTelephoneNumberLandline = model.HomeTelephoneNumberLandline;
            staffCoreData.HomeTelephoneNumberMobile = model.HomeTelephoneNumberMobile;
            //staffCoreData.OfficialMobileNumber = model.OfficialMobileNumber;
            staffCoreData.OfficialExtensionNumber = model.OfficialExtensionNumber;
            staffCoreData.Nationality2GUID = model.Nationality2GUID;
            staffCoreData.Nationality3GUID = model.Nationality3GUID;
            staffCoreData.SATPhoneNumber = model.SATPhoneNumber;
            staffCoreData.DamascusExtensionNumber = model.DamascusExtensionNumber;
            staffCoreData.HQExtensionNumber = model.HQExtensionNumber;



            staffCoreData.PositionInOrganigram = model.PositionInOrganigram;
            staffCoreData.UNHCRIDNumber = model.UNHCRIDNumber;
            staffCoreData.StaffPhoto = model.StaffPhoto;
            staffCoreData.NationalPassportPhoto = model.NationalPassportPhoto;
            staffCoreData.StaffPhoto = model.StaffPhoto;
            staffCoreData.UNLPPhoto = model.UNLPPhoto;
            staffCoreData.CallSign = model.CallSign;
            staffCoreData.EmploymentID = model.EmploymentID;
            staffCoreData.ExpiryOfResidencyVisa = model.ExpiryOfResidencyVisa;
            staffCoreData.PaymentEligibilityStatusGUID = model.PaymentEligibilityStatusGUID;


            staffCoreData.EmailAddress = model.EmailAddress;



            //Update Personal Details 
            userPersonalDetails.BloodGroup = model.BloodGroup;
            userPersonalDetails.UserGUID = staffCoreData.UserGUID;
            userPersonalDetails.CountryGUID = model.NationalityGUID;
            userPersonalDetails.DateOfBirth = model.DateOfBirth;
            //userPersonalDetails.GenderGUID = model.GenderGUID;

            if (model.StaffPrefixGUID == Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3771"))
            {
                userPersonalDetails.GenderGUID = Guid.Parse("688B11E0-24FB-44B8-94CE-D8568C4742C7");
                staffCoreData.Gender = Guid.Parse("688B11E0-24FB-44B8-94CE-D8568C4742C7");
            }
            else if (model.StaffPrefixGUID == Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3772"))
            {
                userPersonalDetails.GenderGUID = Guid.Parse("DBF9D307-CE9F-4029-BD1E-D7AF6739975C");
                staffCoreData.Gender = Guid.Parse("DBF9D307-CE9F-4029-BD1E-D7AF6739975C");
            }
            if (userPersonalDetailsLanguageEN != null)
            {
                userPersonalDetailsLanguageEN.FirstName = model.FirstNameEN;
                userPersonalDetailsLanguageEN.Surname = model.SurnameEN;
                userPersonalDetailsLanguageEN.FatherName = model.FatherNameEN;
                userPersonalDetailsLanguageEN.MotherName = model.MotherNameEN;
                DbORG.Update(userPersonalDetailsLanguageEN, Permissions.StaffProfile.UpdateGuid, ExecutionTime, DbCMS);
            }
            else if (model.FirstNameEN != null && model.SurnameEN != null)
            {
                userPersonalDetailsLanguageEN = new userPersonalDetailsLanguage();
                userPersonalDetailsLanguageEN.UserGUID = staffCoreData.UserGUID;
                userPersonalDetailsLanguageEN.FirstName = model.FirstNameEN;
                userPersonalDetailsLanguageEN.Surname = model.SurnameEN;
                userPersonalDetailsLanguageEN.LanguageID = "EN";
                userPersonalDetailsLanguageEN.FatherName = model.FatherNameEN;
                userPersonalDetailsLanguageEN.MotherName = model.MotherNameEN;
                DbORG.Create(userPersonalDetailsLanguageEN, Permissions.StaffProfile.CreateGuid, ExecutionTime, DbCMS);
            }

            if (userPersonalDetailsLanguageAR != null)
            {
                userPersonalDetailsLanguageAR.FirstName = model.FirstNameAR;
                userPersonalDetailsLanguageAR.Surname = model.SurnameAR;
                userPersonalDetailsLanguageAR.FatherName = model.FatherNameAR;
                userPersonalDetailsLanguageAR.MotherName = model.MotherNameAR;
                DbORG.Update(userPersonalDetailsLanguageAR, Permissions.StaffProfile.UpdateGuid, ExecutionTime, DbCMS);
            }
            else if (model.FirstNameAR != null && model.SurnameAR != null)
            {
                userPersonalDetailsLanguageAR = new userPersonalDetailsLanguage();
                userPersonalDetailsLanguageAR.UserGUID = staffCoreData.UserGUID;
                userPersonalDetailsLanguageAR.FirstName = model.FirstNameAR;
                userPersonalDetailsLanguageAR.Surname = model.SurnameAR;
                userPersonalDetailsLanguageAR.FatherName = model.FatherNameAR;
                userPersonalDetailsLanguageAR.MotherName = model.MotherNameAR;
                userPersonalDetailsLanguageAR.LanguageID = "AR";
                DbORG.Create(userPersonalDetailsLanguageAR, Permissions.StaffProfile.CreateGuid, ExecutionTime, DbCMS);
            }

            dataPhoneDirectory phoneDirectory = (from a in DbORG.dataPhoneDirectory.Where(x => x.Active && x.UserGUID == model.UserGUID) select a).FirstOrDefault();
            if (phoneDirectory == null)
            {
                dataPhoneDirectory myphoneDirectory = new dataPhoneDirectory();
                myphoneDirectory.PhoneDirectoryGUID = Guid.NewGuid();
                myphoneDirectory.PhoneHolderTypeGUID = Guid.Parse("20aab4c0-a6f8-4f87-9316-94d1d5f5bd8f");
                myphoneDirectory.UserGUID = model.UserGUID;
                myphoneDirectory.FullName = model.FirstNameEN + " " + model.SurnameEN;
                myphoneDirectory.DutyStationGUID = model.DutyStationGUID;
                myphoneDirectory.DepartmentGUID = model.DepartmentGUID;
                myphoneDirectory.JobTitleGUID = model.JobTitleGUID;
                myphoneDirectory.EmailAddress = model.EmailAddress;
                if (model.StaffStatusGUID != Guid.Empty || model.StaffStatusGUID == Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E3612"))
                {
                    myphoneDirectory.Active = false;
                }
                DbORG.Create(myphoneDirectory, Permissions.StaffProfile.UpdateGuid, ExecutionTime, DbCMS);
            }
            else
            {
                if (model.StaffStatusGUID != Guid.Empty || model.StaffStatusGUID == Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E3612"))
                {
                    phoneDirectory.Active = false;
                }



                phoneDirectory.FullName = model.FirstNameEN + " " + model.SurnameEN;
                phoneDirectory.DutyStationGUID = model.DutyStationGUID;
                phoneDirectory.DepartmentGUID = model.DepartmentGUID;
                phoneDirectory.JobTitleGUID = model.JobTitleGUID;
                phoneDirectory.EmailAddress = model.EmailAddress;
                DbORG.Update(phoneDirectory, Permissions.StaffProfile.UpdateGuid, ExecutionTime, DbCMS);

            }



            DbORG.Update(staffCoreData, Permissions.StaffProfile.UpdateGuid, ExecutionTime, DbCMS);
            DbORG.Update(userPersonalDetails, Permissions.StaffProfile.UpdateGuid, ExecutionTime, DbCMS);
            List<dataStaffPosition> _allPosistions = new List<dataStaffPosition>();

            if (model.AllStaffPositionsGUID != null && model.AllStaffPositionsGUID.Count > 0)
            {

                var toCheck = DbORG.dataStaffPosition.Where(x => x.UserGUID == model.UserGUID).ToList();
                if (toCheck.Count > 0)
                {
                    DbORG.dataStaffPosition.RemoveRange(toCheck);
                    DbORG.SaveChanges();
                }
                foreach (var item in model.AllStaffPositionsGUID)
                {

                    dataStaffPosition _staffposition = new dataStaffPosition
                    {
                        StaffPositionGUID = Guid.NewGuid(),
                        UserGUID = model.UserGUID,
                        PositionGUID = item,
                        CreateByGUID = UserGUID,
                        CreateDate = ExecutionTime,

                        Active = true,

                    };
                    _allPosistions.Add(_staffposition);


                }
                DbORG.CreateBulk(_allPosistions, Permissions.StaffProfile.CreateGuid, ExecutionTime, DbCMS);
            }

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.StaffProfile.Create, Apps.ORG, new UrlHelper(Request.RequestContext).Action("StaffProfile/Create", "StaffProfile", new { Area = "ORG" })), Container = "StaffProfileFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.StaffProfile.Update, Apps.ORG), Container = "StaffProfileFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.StaffProfile.Delete, Apps.ORG), Container = "StaffProfileFormControls" });

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                if (model.AllStaffPositionsGUID != null && model.AllStaffPositionsGUID.Count > 0)
                {
                    string URL = "";
                    string Anchor = "";
                    string Link = "";
                    var _allpositionName = DbORG.codeTablesValuesLanguages.Where(x =>
                                model.AllStaffPositionsGUID.Contains(x.ValueGUID) && x.Active == true
                                && x.LanguageID == LAN).Select(x => x.ValueDescription).ToList();


                    //var currentDanger = allDan.Where(x => x.dataStaffEligibleForDangerPay.UserGUID == user.UserGUID).FirstOrDefault();
                    //URL = AppSettingsKeys.Domain + "/ORG/StaffProfile/Update/" + new Portal().GUIDToString(staffCoreData.UserGUID);
                    //Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ClickToConfirm + "</a>";
                    //Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
                    //string myFirstName = user.FirstName;
                    //string mySurName = user.Surname;
                    string _positionName = string.Join(" ;", _allpositionName);
                    string SubjectMessage = "Staff - Change Position";
                    string _message = resxEmails.ChangePositionForStaff
                        //.Replace("$FullName", user.FullName)
                        .Replace("$StaffName", userPersonalDetailsLanguageEN.FirstName + " " + userPersonalDetailsLanguageEN.Surname)
                        .Replace("$Position", _positionName)
                        /*.Replace("$VerifyLink", Anchor)*/
                        ;


                    if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                    int isRec = 1;
                    //var myEmail = alluserAccounts.Where(x => x.UserGUID == user.UserGUID).Select(x => x.EmailAddress).FirstOrDefault();
                    //Guid dma = Guid.Parse("00000000-0000-0000-0000-000000000001");
                    //var warehousesGUIDs = DbWMS.codeWarehouse.Where(x => x.WarehouseGUID == dma || x.ParentGUID == dma).Select(x => x.WarehouseGUID).Distinct().ToList();
                    //var currentWarehouseGuids = DbWMS.codeWarehouseFocalPoint.Where(x => warehousesGUIDs.Contains((Guid)x.WarehouseGUID) && x.IsFocalPoint == true).
                    //    Select(x => x.UserGUID).Distinct().ToList();

                    //var users = DbWMS.StaffCoreData.Where(x => currentWarehouseGuids.Contains(x.UserGUID)).Select(x => x.EmailAddress).Distinct().ToList();

                    string copyEmails = string.Join(" ;", "sahhar@unhcr.org");
                    Send(copyEmails, SubjectMessage, _message, 1, "isac@unhcr.org", null);
                }
                DateTime? executiontime = DateTime.Now;
                if (staffCoreData.StaffStatusGUID == Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E3612") && (staffCoreData.ContractEndDate != null && staffCoreData.ContractEndDate < executiontime))
                {
                    string URL = "";
                    string Anchor = "";
                    string Link = "";
                    Guid pendingGUID = Guid.Parse("55962180-3634-44a4-874c-db7c3481d888");
                    Guid confirmedGUID = Guid.Parse("55962180-3634-44A4-874C-DB7C3481D882");

                    var _service = DbORG.dataStaffServiceProvided.Where(x => (x.LastFlowStatusGUID == pendingGUID || x.LastFlowStatusGUID == confirmedGUID)
                                   && x.Active && x.UserGUID == staffCoreData.UserGUID).ToList();
                    if (_service.Count > 0)
                    {
                        //var currentDanger = allDan.Where(x => x.dataStaffEligibleForDangerPay.UserGUID == user.UserGUID).FirstOrDefault();
                        URL = AppSettingsKeys.Domain + "/ORG/StaffProfile/Update/" + new Portal().GUIDToString(staffCoreData.UserGUID);
                        Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ClickToConfirm + "</a>";
                        Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
                        //string myFirstName = user.FirstName;
                        //string mySurName = user.Surname;


                        //var myEmail = alluserAccounts.Where(x => x.UserGUID == user.UserGUID).Select(x => x.EmailAddress).FirstOrDefault();
                        //Guid dma = Guid.Parse("00000000-0000-0000-0000-000000000001");

                        //var warehousesGUIDs = DbWMS.codeWarehouse.Where(x => x.WarehouseGUID == dma || x.ParentGUID == dma).Select(x => x.WarehouseGUID).Distinct().ToList();
                        //var currentWarehouseGuids = DbWMS.codeWarehouseFocalPoint.Where(x => warehousesGUIDs.Contains((Guid)x.WarehouseGUID) && x.IsFocalPoint == true).
                        //    Select(x => x.UserGUID).Distinct().ToList();
                        var services = DbORG.dataICTServiceAuthUser.Where(x => x.DutyStationGUID == staffCoreData.DutyStationGUID).ToList();
                        var _serviceGUID = services.Select(x => x.ServiceGUID).Distinct().ToList();
                        string SubjectMessage = "Staff Departure -ICT services to remove";
                        string table =
                                  "<br><table class='table table-bordered' style='border-collapse: collapse; width: 900px'><tr><th style='border: 1px solid  #333; vertical-align:middle'>IT Service</th><th style='border: 1px solid  #333; vertical-align:middle'>FP</th><th style='border: 1px solid  #333; vertical-align: middle'>Status</th></tr>";
                        int total = 0;
                        string _status = "Confirmed";
                        List<string> _allEmails = new List<string>();


                        foreach (var item in _serviceGUID.Distinct())
                        {
                            var focalpoint = services.Where(x => x.IsMainFocalPoint == true && x.ServiceGUID == item).FirstOrDefault().EmailAddress;
                            var _backupUsers = services.Where(x => x.IsMainFocalPoint == false && x.ServiceGUID == item).Select(x => x.EmailAddress).Distinct().ToList();


                            //string copyEmails = string.Join(" ;", _backupUsers);


                            //copyEmails = string.Join(copyEmails, ";isac@unhcr.org");


                            string servicename = DbAHD.codeTablesValuesLanguages.Where(x => x.ValueGUID == item && x.Active
                            && x.LanguageID == LAN).FirstOrDefault().ValueDescription;



                            var allfocalpointUserGUIDs = services.Where(x => x.ServiceGUID == item).Select(x => x.UserGUID).Distinct();
                            var _allFPInfo = DbAHD.v_StaffProfileInformation.Where(x => allfocalpointUserGUIDs.Contains(x.UserGUID)).Distinct().ToList();
                            var _myNames = _allFPInfo.Select(x => x.FullName).Distinct().ToList();
                            var _myAllEmails = _allFPInfo.Select(x => x.EmailAddress).Distinct().ToList();
                            string copyNames = string.Join(" ;", _myNames);

                            _allEmails.AddRange(_myAllEmails);


                            table += "<tr><td style='border: 1px solid  #333; vertical-align: middle'>" + servicename + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + copyNames + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + _status + "</td></tr>";

                            total++;




                        }
                        table += "</table>";

                        _allEmails.Remove("isac@unhcr.org");
                        var _noDupEmail = _allEmails.Select(x => x).Distinct();
                        string copyEmails = string.Join(" ;", _noDupEmail);

                        var _staff = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == model.UserGUID).FirstOrDefault();

                        string _message = resxEmails.NewRemoveStaffFromAddedAllServiceProvided
                                     //.Replace("$FullName", user.FullName)
                                     .Replace("$StaffName", userPersonalDetailsLanguageEN.FirstName + " " + userPersonalDetailsLanguageEN.Surname)
                                    .Replace("$DepartureDate", model.LastDepartureDate.Value.ToLongDateString())
                                       .Replace("$dutystation", _staff.DutyStation)
                                       .Replace("$table", table)
                                       ;





                        if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                        int isRec = 1;
                        Send(copyEmails, SubjectMessage, _message, 1, "isac@unhcr.org", null);



                        string _messageApps = resxEmails.RemoveStaffFromAddedServiceProvided
                               //.Replace("$FullName", user.FullName)
                               .Replace("$StaffName", userPersonalDetailsLanguageEN.FirstName + " " + userPersonalDetailsLanguageEN.Surname)
                               .Replace("$DepartureDate", model.LastDepartureDate.Value.ToLongDateString())
                               .Replace("$VerifyLink", Anchor)
                               .Replace("$targetservice", "Apps");

                        string copyapps = string.Join(" ;", "Shaban@unhcr.org", "KARKOUSH@unhcr.org", "ALFAZZAA@unhcr.org", "maksoud@unhcr.org");

                        Send(copyapps, SubjectMessage, _messageApps, 1, "isac@unhcr.org", null);

                        //var users = DbWMS.StaffCoreData.Where(x => currentWarehouseGuids.Contains(x.UserGUID)).Select(x => x.EmailAddress).Distinct().ToList();
                        //string copyEmails = string.Join(" ;", users);
                        //foreach (var item in users)
                        //{
                        //    Send(item, SubjectMessage, _message, 1, null);

                        //}


                        //Send("isac@unhcr.org", SubjectMessage, _message, isRec, null);
                        // string copyEmails = string.Join(" ;", users, "Shaban@unhcr.org", "KARKOUSH@unhcr.org", "ALFAZZAA@unhcr.org", "shaglil@unhcr.org");
                        // Send(copyEmails, SubjectMessage, _message, 1, "isac@unhcr.org");
                    }
                }
                return Json(DbORG.SingleCreateMessage(DbORG.PrimaryKeyControl(staffCoreData), DbORG.RowVersionControls(Portal.SingleToList(staffCoreData)), null, "", UIButtons));

                //return Json(DbORG.SingleCreateMessage(DbORG.PrimaryKeyControl(staffCoreData), DbORG.RowVersionControls(Portal.SingleToList(staffCoreData)), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffProfileCreate(StaffProfileUpdateModel model)
        {

            if (!CMS.HasAction(Permissions.StaffProfileHRSection.Update, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }


            var checkStaff = DbORG.StaffCoreData.Where(x => x.EmailAddress == model.EmailAddress).FirstOrDefault();
            if (checkStaff == null || model.EmploymentID == null)
            {
                DateTime ExecutionTime = DateTime.Now;

                Guid newUserGUID = Guid.NewGuid();
                Guid newStaffPhotoGUID = Guid.NewGuid();
                Guid newSyrianNationalIDPhotoGUID = Guid.NewGuid();
                Guid newNationalPassportPhotoGUID = Guid.NewGuid();
                Guid newUNLPPhotoGUID = Guid.NewGuid();
                userAccounts userAccounts = new userAccounts();
                userAccounts.UserGUID = newUserGUID;
                userAccounts.IsFirstLogin = true;
                userAccounts.Active = true;
                userAccounts.RequestedOn = DateTime.Now;

                userAccounts.TimeZone = "Syria Standard Time";
                userAccounts.AccountStatusID = 10;
                userAccounts.SecurityQuestionGUID = Guid.Parse("EE813F52-8768-4E4A-8E07-42F43093859E"); //Who is your favorite artist?
                userAccounts.SecurityAnswer = "Automated security answer! Change it please";
                userAccounts.Active = true;



                StaffCoreData staffCoreData = Mapper.Map(model, new StaffCoreData());
                staffCoreData.UserGUID = newUserGUID;
                staffCoreData.JobTitleGUID = model.JobTitleGUID.Value;
                staffCoreData.DutyStationGUID = model.DutyStationGUID;
                staffCoreData.OfficeTypeGUID = model.OfficeTypeGUID;
                staffCoreData.DepartmentGUID = (Guid)model.DepartmentGUID;
                staffCoreData.ReportToGUID = model.ReportToGUID;

                staffCoreData.RecruitmentTypeGUID = model.RecruitmentTypeGUID;
                staffCoreData.Active = true;
                staffCoreData.DateOfBirth = model.DateOfBirth;
                staffCoreData.LastDepartureDate = model.LastDepartureDate;
                staffCoreData.PlaceOfBirthGUID = model.PlaceOfBirthGUID;
                staffCoreData.Gender = model.GenderGUID;
                staffCoreData.StaffPrefix = model.StaffPrefix;
                staffCoreData.SyrianNationalIDNumber = model.SyrianNationalIDNumber;
                staffCoreData.LastJobEn = model.LastJobEn;
                staffCoreData.LastJobAr = model.LastJobAr;
                model.PaymentEligibilityStatusGUID = model.PaymentEligibilityStatusGUID;
                staffCoreData.PlaceBirthCityEn = model.PlaceBirthCityEn;
                staffCoreData.PlaceBirthCityAr = model.PlaceBirthCityAr;
                staffCoreData.UNLPNumber = model.UNLPNumber;
                staffCoreData.UNLPDateOfIssue = model.UNLPDateOfIssue;
                staffCoreData.UNLPDateOfExpiry = model.UNLPDateOfExpiry;
                staffCoreData.UNHCRIDNumber = model.UNHCRIDNumber;
                staffCoreData.NationalPassportNumber = model.NationalPassportNumber;
                staffCoreData.NationalPassportDateOfIssue = model.NationalPassportDateOfIssue;
                staffCoreData.ContractTypeGUID = model.ContractTypeGUID;
                staffCoreData.PositionNumber = model.PositionNumber;
                staffCoreData.StaffPositionGUID = model.StaffPositionGUID;
                staffCoreData.StaffEOD = model.StaffEOD;
                staffCoreData.BankAccountHolderNameAr = model.BankAccountHolderNameAr;
                staffCoreData.BankAccountHolderNameEn = model.BankAccountHolderNameEn;
                staffCoreData.BloodGroup = model.BloodGroup;
                staffCoreData.BankAccountNumberSyr = model.BankAccountNumberSyr;
                staffCoreData.SATPhoneNumber = model.SATPhoneNumber;
                staffCoreData.BankNameSyr = model.BankNameSyr;
                staffCoreData.StaffGradeGUID = model.StaffGradeGUID;
                staffCoreData.ContractStartDate = model.ContractStartDate;
                staffCoreData.ContractEndDate = model.ContractEndDate;
                staffCoreData.StaffStatus = model.StaffStatus;
                staffCoreData.StaffStatusGUID = model.StaffStatusGUID;
                staffCoreData.NationalityGUID = model.NationalityGUID;
                staffCoreData.Nationality2GUID = model.Nationality2GUID;
                staffCoreData.Nationality3GUID = model.Nationality3GUID;
                staffCoreData.OfficeRoomNumberBuilding = model.OfficeRoomNumberBuilding;


                staffCoreData.TitleEnglishMSRP = model.TitleEnglishMSRP;
                staffCoreData.TitleArabicMSRP = model.TitleArabicMSRP;
                //staffCoreData.TitleEnglishMOFA = model.TitleEnglishMOFA;
                //staffCoreData.TitleArabicMOFA = model.TitleArabicMOFA;

                staffCoreData.PermanentAddressEn = model.PermanentAddressEn;
                staffCoreData.PermanentAddressAr = model.PermanentAddressAr;
                staffCoreData.CurrentAddressEn = model.CurrentAddressEn;
                staffCoreData.CurrentAddressAr = model.CurrentAddressAr;
                staffCoreData.HomeTelephoneNumberLandline = model.HomeTelephoneNumberLandline;
                staffCoreData.HomeTelephoneNumberMobile = model.HomeTelephoneNumberMobile;
                // staffCoreData.OfficialMobileNumber = model.OfficialMobileNumber;
                staffCoreData.OfficialExtensionNumber = model.OfficialExtensionNumber;
                staffCoreData.DamascusExtensionNumber = model.DamascusExtensionNumber;
                staffCoreData.HQExtensionNumber = model.HQExtensionNumber;
                staffCoreData.LastConfirmationStatusGUID = Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E3747");
                staffCoreData.LastConfirmationStatus = "Pending Confirmation";
                //staffCoreData.LastConfirmationStatus = "";
                //staffCoreData.LastConfirmationStatusGUID = Guid.Parse("");




                //if (model.GenderGUID == StaffGender.Male)
                //{
                //    staffCoreData.StaffPrefix = "Mr.";
                //}
                //else if (model.GenderGUID == StaffGender.Male)
                //{
                //    staffCoreData.StaffPrefix = "Mrs.";
                //}



                userPersonalDetails userPersonalDetails = new userPersonalDetails();
                userPersonalDetailsLanguage userPersonalDetailsLanguageEN = new userPersonalDetailsLanguage();
                userPersonalDetailsLanguage userPersonalDetailsLanguageAR = new userPersonalDetailsLanguage();


                userPersonalDetails.UserGUID = newUserGUID;
                //userPersonalDetails.GenderGUID = model.GenderGUID;
                //if (model.StaffPrefix == "Mr.")
                //{
                //    userPersonalDetails.GenderGUID = Guid.Parse("688B11E0-24FB-44B8-94CE-D8568C4742C7");
                //    staffCoreData.Gender = Guid.Parse("688B11E0-24FB-44B8-94CE-D8568C4742C7");
                //}
                //else if (model.StaffPrefix == "Mrs.")
                //{
                //    userPersonalDetails.GenderGUID = Guid.Parse("DBF9D307-CE9F-4029-BD1E-D7AF6739975C");
                //    staffCoreData.Gender = Guid.Parse("DBF9D307-CE9F-4029-BD1E-D7AF6739975C");
                //}

                if (model.StaffPrefixGUID == Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3771"))
                {
                    userPersonalDetails.GenderGUID = Guid.Parse("688B11E0-24FB-44B8-94CE-D8568C4742C7");
                    staffCoreData.Gender = Guid.Parse("688B11E0-24FB-44B8-94CE-D8568C4742C7");
                }
                else if (model.StaffPrefixGUID == Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3772"))
                {
                    userPersonalDetails.GenderGUID = Guid.Parse("DBF9D307-CE9F-4029-BD1E-D7AF6739975C");
                    staffCoreData.Gender = Guid.Parse("DBF9D307-CE9F-4029-BD1E-D7AF6739975C");
                }
                userPersonalDetails.DateOfBirth = model.DateOfBirth;
                userPersonalDetails.BloodGroup = model.BloodGroup;
                userPersonalDetails.CountryGUID = model.NationalityGUID;

                userPersonalDetailsLanguageEN.PersonalDetailsLanguageGUID = Guid.NewGuid();
                userPersonalDetailsLanguageEN.UserGUID = newUserGUID;
                userPersonalDetailsLanguageEN.FirstName = model.FirstNameEN;
                userPersonalDetailsLanguageEN.Surname = model.SurnameEN;
                userPersonalDetailsLanguageEN.FatherName = model.FatherNameEN;
                userPersonalDetailsLanguageEN.MotherName = model.MotherNameEN;
                userPersonalDetailsLanguageEN.LanguageID = "EN";


                userPersonalDetailsLanguageAR.PersonalDetailsLanguageGUID = Guid.NewGuid();
                userPersonalDetailsLanguageAR.UserGUID = newUserGUID;
                userPersonalDetailsLanguageAR.FirstName = model.FirstNameAR;
                userPersonalDetailsLanguageAR.Surname = model.SurnameAR;
                userPersonalDetailsLanguageAR.FatherName = model.FatherNameAR;
                userPersonalDetailsLanguageAR.MotherName = model.MotherNameAR;
                userPersonalDetailsLanguageAR.LanguageID = "AR";
                userPasswords userPasswords = new userPasswords();
                userProfiles userProfiles = new userProfiles();
                if (!staffCoreData.EmailAddress.ToLower().EndsWith("@unhcr.org"))
                {
                    userPasswords.PasswordGUID = Guid.NewGuid();
                    userPasswords.UserGUID = newUserGUID;
                    userPasswords.ActivationDate = DateTime.Now;
                    userPasswords.Active = true;
                    userPasswords.Password = HashingHelper.EncryptPassword("P@ssw0rd123", Portal.GUIDToString(newUserGUID));
                    DbORG.Create(userPasswords, Permissions.StaffProfile.CreateGuid, ExecutionTime, DbCMS);
                }


                userServiceHistory userServiceHistory = new userServiceHistory();
                Guid NewServiceHistoryGUiD = Guid.NewGuid();
                userServiceHistory.ServiceHistoryGUID = NewServiceHistoryGUiD;
                userServiceHistory.UserGUID = newUserGUID;
                Guid SyriaOrganizationGUID = Guid.Parse("E156C022-EC72-4A5A-BE09-163BD85C68EF");
                Guid organizationGuid = (from a in DbCMS.codeOrganizationsInstances where a.OrganizationInstanceGUID == SyriaOrganizationGUID select a.OrganizationGUID).FirstOrDefault();
                userServiceHistory.OrganizationGUID = organizationGuid;
                userServiceHistory.EmailAddress = model.EmailAddress;
                userServiceHistory.Active = true;



                userProfiles.UserProfileGUID = Guid.NewGuid();
                userProfiles.ServiceHistoryGUID = NewServiceHistoryGUiD;
                userProfiles.OrganizationInstanceGUID = SyriaOrganizationGUID;
                userProfiles.DutyStationGUID = model.DutyStationGUID;
                userProfiles.DepartmentGUID = model.DepartmentGUID;
                userProfiles.JobTitleGUID = model.JobTitleGUID;
                userProfiles.FromDate = new DateTime(1900, 1, 1);
                userProfiles.Active = true;



                dataPhoneDirectory myphoneDirectory = new dataPhoneDirectory();
                myphoneDirectory.PhoneDirectoryGUID = Guid.NewGuid();
                myphoneDirectory.PhoneHolderTypeGUID = Guid.Parse("20aab4c0-a6f8-4f87-9316-94d1d5f5bd8f");
                myphoneDirectory.UserGUID = newUserGUID;
                myphoneDirectory.FullName = model.FirstNameEN + " " + model.SurnameEN;
                myphoneDirectory.DutyStationGUID = model.DutyStationGUID;
                myphoneDirectory.DepartmentGUID = model.DepartmentGUID;
                myphoneDirectory.JobTitleGUID = model.JobTitleGUID;
                myphoneDirectory.EmailAddress = model.EmailAddress;




                DbORG.Create(userAccounts, Permissions.StaffProfile.CreateGuid, ExecutionTime, DbCMS);
                DbORG.Create(myphoneDirectory, Permissions.StaffProfile.CreateGuid, ExecutionTime, DbCMS);
                DbORG.Create(staffCoreData, Permissions.StaffProfile.CreateGuid, ExecutionTime, DbCMS);
                DbORG.Create(userPersonalDetails, Permissions.StaffProfile.CreateGuid, ExecutionTime, DbCMS);
                DbORG.Create(userPersonalDetailsLanguageEN, Permissions.StaffProfile.CreateGuid, ExecutionTime, DbCMS);
                DbORG.Create(userPersonalDetailsLanguageAR, Permissions.StaffProfile.CreateGuid, ExecutionTime, DbCMS);



                DbORG.Create(userProfiles, Permissions.StaffProfile.CreateGuid, ExecutionTime, DbCMS);
                DbORG.Create(userServiceHistory, Permissions.StaffProfile.CreateGuid, ExecutionTime, DbCMS);

                Guid ictService = Guid.Parse("55962180-3634-44A4-874C-DB7C3481D66A");
                Guid _securityGUID = Guid.Parse("55962180-3634-44A4-874C-DB7C8881D66A");
                var _ictservices = DbAHD.codeTablesValuesLanguages.Where(x => x.codeTablesValues.TableGUID == ictService && x.Active && x.LanguageID == LAN
                && x.ValueGUID != _securityGUID).ToList();
                List<dataStaffServiceProvided> _allservices = new List<dataStaffServiceProvided>();
                Guid pendingGUID = Guid.Parse("55962180-3634-44a4-874c-db7c3481d888");

                foreach (var item in _ictservices)
                {
                    dataStaffServiceProvided _myService = new dataStaffServiceProvided
                    {
                        StaffServiceProvidedGUID = Guid.NewGuid(),
                        UserGUID = staffCoreData.UserGUID,

                        ServiceTypeGUID = item.ValueGUID,
                        ServiceName = item.ValueDescription,
                        LastFlowStatusGUID = pendingGUID,
                        LastFlowStatusName = "Pending",
                        StartDate = staffCoreData.ContractStartDate,
                        CreateDate = ExecutionTime,
                        CreateByGUID = UserGUID,
                        Active = true


                    };
                    _allservices.Add(_myService);

                }
                DbORG.CreateBulk(_allservices, Permissions.StaffProfile.CreateGuid, ExecutionTime, DbCMS);

                List<dataStaffPosition> _allPosistions = new List<dataStaffPosition>();
                if (model.AllStaffPositionsGUID != null && model.AllStaffPositionsGUID.Count > 0)
                {
                    var toCheck = DbORG.dataStaffPosition.Where(x => x.UserGUID == model.UserGUID).ToList();
                    foreach (var item in model.AllStaffPositionsGUID)
                    {
                        if (toCheck.Where(x => x.PositionGUID == item) == null)
                        {
                            dataStaffPosition _staffposition = new dataStaffPosition
                            {
                                StaffPositionGUID = Guid.NewGuid(),
                                UserGUID = model.UserGUID,
                                PositionGUID = item,
                                CreateByGUID = UserGUID,
                                CreateDate = ExecutionTime,

                                Active = true,

                            };
                            _allPosistions.Add(_staffposition);
                        }

                    }
                    DbORG.CreateBulk(_allPosistions, Permissions.StaffProfile.CreateGuid, ExecutionTime, DbCMS);
                }

                try
                {
                    DbORG.SaveChanges();
                    DbCMS.SaveChanges();
                    //if(staffCoreData.RecruitmentTypeGUID==Guid.Parse(""))

                    string URL = "";
                    string Anchor = "";
                    string Link = "";



                    //var currentDanger = allDan.Where(x => x.dataStaffEligibleForDangerPay.UserGUID == user.UserGUID).FirstOrDefault();
                    URL = AppSettingsKeys.Domain + "/ORG/StaffProfile/Update/?PK=" + new Portal().GUIDToString(staffCoreData.UserGUID);
                    Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ClickToConfirm + "</a>";
                    Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
                    //string myFirstName = user.FirstName;
                    //string mySurName = user.Surname;

                    string SubjectMessage = "Staff Arrival - ICT service to add ";


                    var services = DbORG.dataICTServiceAuthUser.Where(x => x.DutyStationGUID == staffCoreData.DutyStationGUID).ToList();
                    var _serviceGUID = services.Select(x => x.ServiceGUID).Distinct().ToList();
                    string table =
                                 "<br><table class='table table-bordered' style='border-collapse: collapse; width: 900px'><tr><th style='border: 1px solid  #333; vertical-align:middle'>IT Service</th><th style='border: 1px solid  #333; vertical-align:middle'>FP</th><th style='border: 1px solid  #333; vertical-align: middle'>Status</th></tr>";
                    int total = 0;
                    string _status = "Pending";
                    List<string> _allEmails = new List<string>();
                    foreach (var item in _serviceGUID)
                    {

                        var focalpoint = services.Where(x => x.IsMainFocalPoint == true && x.ServiceGUID == item).FirstOrDefault().EmailAddress;
                        var _backupUsers = services.Where(x => x.IsMainFocalPoint == false && x.ServiceGUID == item).Select(x => x.EmailAddress).Distinct().ToList();
                        //_backupUsers.Add("isac@unhcr.org");
                        var allfocalpointUserGUIDs = services.Where(x => x.ServiceGUID == item).Select(x => x.UserGUID).Distinct();
                        var _allFPInfo = DbAHD.v_StaffProfileInformation.Where(x => allfocalpointUserGUIDs.Contains(x.UserGUID)).Distinct().ToList();
                        var _myNames = _allFPInfo.Select(x => x.FullName).Distinct().ToList();
                        var _myAllEmails = _allFPInfo.Select(x => x.EmailAddress).Distinct().ToList();
                        string copyNames = string.Join(" ;", _myNames);

                        _allEmails.AddRange(_myAllEmails);

                        string servicename = DbAHD.codeTablesValuesLanguages.Where(x => x.ValueGUID == item && x.Active && x.LanguageID == LAN).FirstOrDefault().ValueDescription;

                        table += "<tr><td style='border: 1px solid  #333; vertical-align: middle'>" + servicename + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + copyNames + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + _status + "</td></tr>";

                        total++;



                    }
                    table += "</table>";

                    _allEmails.Remove("isac@unhcr.org");
                    var _noDupEmail = _allEmails.Select(x => x).Distinct();
                    string copyEmails = string.Join(" ;", _noDupEmail);


                    var _staff = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == staffCoreData.UserGUID).FirstOrDefault();
                    string _message = resxEmails.NewStaffToAddServiceProvidedAllServices
                                       //.Replace("$FullName", user.FullName)
                                       .Replace("$StaffName", userPersonalDetailsLanguageEN.FirstName + " " + userPersonalDetailsLanguageEN.Surname)
                                       .Replace("$ContractStartDate", staffCoreData.StaffEOD.Value.ToLongDateString())
                                         .Replace("$dutystation", _staff.DutyStation)
                                           .Replace("$email", _staff.EmailAddress)
                                                 .Replace("$jobtile", _staff.JobTitle)
                                                 .Replace("$unit", _staff.DepartmentDescription)

                                         .Replace("$table", table)
                                         ;
                    /*.Replace("$VerifyLink", Anchor  )*/
                    ;

                    if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                    int isRec = 1;
                    if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                    Send(copyEmails, SubjectMessage, _message, 1, "isac@unhcr.org", null);


                    string _messageApps = resxEmails.NewStaffToAddServiceProvidedApps
                                       //.Replace("$FullName", user.FullName)
                                       .Replace("$StaffName", userPersonalDetailsLanguageEN.FirstName + " " + userPersonalDetailsLanguageEN.Surname)
                                       .Replace("$ContractStartDate", model.StaffEOD.Value.ToLongDateString())
                                         .Replace("$targetservice", "Apps");
                    /*.Replace("$VerifyLink", Anchor  )*/
                    ;

                    string copyapps = string.Join(" ;", "Shaban@unhcr.org", "KARKOUSH@unhcr.org", "ALFAZZAA@unhcr.org", "maksoud@unhcr.org");

                    Send(copyapps, SubjectMessage, _messageApps, 1, "isac@unhcr.org", null);



                    string url1 = "https://unhcr365.sharepoint.com/:b:/t/mena-syr-ICTUnit/EYaZxtVCng1GlrrvrZ9z460BEUc8XGb2N678haiH9lZQ6g?e=r0oypa";
                    string url2 = "https://unhcr365.sharepoint.com/:b:/t/mena-syr-ICTUnit/EYSfAQlDz-xGg28SQSH8jsgBqYzieI0rJv-8mfzkKH9ZWw?e=9ykR8f";
                    string url3 = "https://unhcr365.sharepoint.com/:u:/t/mena-syr-ICTUnit/EajqJixT-jpPtnXjtC51P-kBjSVZSOEQBUB6RxYfLWD5qQ";
                    string url4 = "https://sway.office.com/knaGaLHraLU16Q1N?ref=Link&loc=play";
                    string url5 = "https://prv.unhcrsyria.org/";

                    string url6 = "https://unhcr365.sharepoint.com/:p:/t/mena-syr-ICTUnit/EcYH--FfIO5JifTs1nvHeNEBRZNBMDAVdRNtCAw0u53GOA?e=ZVohNW";

                    string url7 = "https://unhcr365.sharepoint.com/:b:/t/mena-syr-ICTUnit/EXLZksEi-ZhKmZFNudWVzZUBzd0ztKxslIEwkC3PRs_QAw?e=8pSv5I";

                    string Anchor1 = "<a href='" + url1 + "' target='_blank'>" + resxUIControls.ClickHere + "</a>";
                    string Anchor2 = "<a href='" + url2 + "' target='_blank'>" + resxUIControls.ClickHere + "</a>";
                    string Anchor3 = "<a href='" + url3 + "' target='_blank'>" + resxUIControls.ClickHere + "</a>";
                    string Anchor4 = "<a href='" + url4 + "' target='_blank'>" + resxUIControls.ClickHere + "</a>";
                    string Anchor5 = "<a href='" + url5 + "' target='_blank'>" + resxUIControls.ClickHere + "</a>";
                    string Anchor6 = "<a href='" + url6 + "' target='_blank'>" + resxUIControls.ClickHere + "</a>";
                    string Anchor7 = "<a href='" + url7 + "' target='_blank'>" + resxUIControls.ClickHere + "</a>";




                    string table_newStaff =
                            "<br><table class='table table-bordered' style='border-collapse: collapse; width: 900px'><tr><th style='border: 1px solid  #333; vertical-align:middle'>Common IT services</th><th style='border: 1px solid  #333; vertical-align:middle'>Link</th></tr>";

                    table_newStaff += "<tr><td style='border: 1px solid  #333; vertical-align: middle'>How to handle office issued laptop</td><td style='border: 1px solid  #333; vertical-align: middle'>" + Anchor1 + "</td></tr>";
                    table_newStaff += "<tr><td style='border: 1px solid  #333; vertical-align: middle'>How to handle office issued cell phone</td><td style='border: 1px solid  #333; vertical-align: middle'>" + Anchor2 + "</td></tr>";
                    table_newStaff += "<tr><td style='border: 1px solid  #333; vertical-align: middle'>Syria IT Web Portal (User Technical Guides & Documentation) </td><td style='border: 1px solid  #333; vertical-align: middle'>" + Anchor3 + "</td></tr>";
                    table_newStaff += "<tr><td style='border: 1px solid  #333; vertical-align: middle'>Syria IT Presentation</td><td style='border: 1px solid  #333; vertical-align: middle'>" + Anchor4 + "</td></tr>";
                    table_newStaff += "<tr><td style='border: 1px solid  #333; vertical-align: middle'>Syria locally developed applications</td><td style='border: 1px solid  #333; vertical-align: middle'>" + Anchor5 + "</td></tr>";

                    table_newStaff +=
                            "<tr><th style='border: 1px solid  #333; vertical-align:middle' ><br/></th><th style='border: 1px solid  #333; vertical-align:middle'><br/></th></tr>";


                    table_newStaff +=
                            "<tr><th style='border: 1px solid  #333; vertical-align:middle'>If you are working from Damascus Four Seasons Hotel</th><th style='border: 1px solid  #333; vertical-align:middle'></th></tr>";

                    table_newStaff += "<tr><td style='border: 1px solid  #333; vertical-align: middle'>Syria IT services at FSH </td><td style='border: 1px solid  #333; vertical-align: middle'>" + Anchor6 + "</td></tr>";
                    table_newStaff += "<tr><td style='border: 1px solid  #333; vertical-align: middle'>How to Connect to UNHCR Wi-Fi at FSH </td><td style='border: 1px solid  #333; vertical-align: middle'>" + Anchor7 + "</td></tr>";




                    table_newStaff += "</table>";

                    string subject_notificiton = "Syria IT Notification | Starting with UNHCR Syria IT services";

                    string _NotificationForStaff = resxEmails.NewNotificationInformStaffByService
                                       .Replace("$StaffName", userPersonalDetailsLanguageEN.FirstName + " " + userPersonalDetailsLanguageEN.Surname)


                                        .Replace("$table", table_newStaff)

                                        ;
                    //string emailsCC = "DLSyria-DamascusICTSu@unhcr.org;DLSyriaAllICTStaff@unhcr.org;";



                    //string emailsBCC = string.Join(";", GetAllMembers("DLSyriaAllICTStaff").ToArray()) + ";";
                    //emailsBCC += string.Join(";", GetAllMembers("DLSyria-DamascusICTSu").ToArray()) + ";";


                    Send(staffCoreData.EmailAddress, subject_notificiton, _NotificationForStaff, isRec, "", "");

                    return Json(DbCMS.SingleCreateMessage());
                }

                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw;
                }
            }
            else
            {

                return Json("Staff Already Exists");

            }

        }

        private static List<string> GetAllMembers(string groupName)
        {
            List<string> answer = new List<string>();
            DirectoryEntry entry = new DirectoryEntry("LDAP://unhcr.local/OU=Organisation,DC=UNHCR,DC=LOCAL", "SA-SYRDAFNP-ADM", "6osi6wogo@ih&q+sPabi", AuthenticationTypes.Secure);
            DirectorySearcher ds = new DirectorySearcher(entry);
            ds.Filter = String.Format("(&(cn={0})(objectClass=group))", groupName);
            ds.PropertiesToLoad.Add("member");
            SearchResult sr = ds.FindOne();
            for (int i = 0; i < sr.Properties["member"].Count; i++)
                answer.Add(sr.Properties["member"][i].ToString().Split(',')[0].Split('=')[1] + "@unhcr.org");

            return answer;
        }

        public ActionResult StaffProfileUpdateOtherUnit(Guid PK)
        {
            if (CMS.HasAction(Permissions.StaffProfileSecuritySection.Access, Apps.ORG))
            {
                //StaffProfileSecuritySectionUpdateModel model =DbCMS.StaffCoreData.Where(x => x.UserGUID == PK).Select(x=>new StaffProfileSecuritySectionUpdateModel {
                //    EmailAddress=x.EmailAddress,
                //    UNHCRIDNumber=x.UNHCRIDNumber,
                //    BloodGroup=x.BloodGroup,
                //    CallSign=x.CallSign,
                //    PermanentAddressEn=x.PermanentAddressEn,
                //    PermanentAddressAr=x.PermanentAddressAr,
                //    CurrentAddressEn=x.CurrentAddressEn,
                //    CurrentAddressAr=x.CurrentAddressAr,
                //    HomeTelephoneNumberLandline=x.HomeTelephoneNumberLandline,
                //    HomeTelephoneNumberMobile=x.HomeTelephoneNumberMobile,
                //    Active=x.Active,
                //    UserGUID=x.UserGUID


                //}).FirstOrDefault();

                return PartialView("~/Areas/ORG/Views/StaffProfile/_StaffProfileSecuritySectionUpdateModal.cshtml", DbORG.StaffCoreData.Find(PK));
            }







            else
            {
                return Json(DbORG.PermissionError());
            }
        }

        [HttpPost]
        public ActionResult StaffProfileAdminUpdate(StaffProfileUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.StaffProfileAdminSection.Update, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            //if (!ModelState.IsValid || ActiveStaff(model)) return PartialView("~/Areas/ORG/Views/StaffProfile/_StaffProfileForm.cshtml", model);
            //if (model.EmploymentID == null)
            //{
            //    return Json(DbORG.PermissionError());
            //}

            DateTime ExecutionTime = DateTime.Now;

            StaffCoreData staffCoreData = (from a in DbORG.StaffCoreData.Where(x => x.Active && x.UserGUID == model.UserGUID) select a).FirstOrDefault();





            staffCoreData.LastJobEn = model.LastJobEn;
            staffCoreData.LastJobAr = model.LastJobAr;

            staffCoreData.OfficeRoomNumberBuilding = model.OfficeRoomNumberBuilding;



            staffCoreData.ExpiryOfResidencyVisa = model.ExpiryOfResidencyVisa;




            //Update Personal Details 


            DbORG.Update(staffCoreData, Permissions.StaffProfile.UpdateGuid, ExecutionTime, DbCMS);


            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.StaffProfile.Create, Apps.ORG, new UrlHelper(Request.RequestContext).Action("StaffProfile/Create", "StaffProfile", new { Area = "ORG" })), Container = "StaffProfileFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.StaffProfile.Update, Apps.ORG), Container = "StaffProfileFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.StaffProfile.Delete, Apps.ORG), Container = "StaffProfileFormControls" });

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.SingleCreateMessage(DbORG.PrimaryKeyControl(staffCoreData), DbORG.RowVersionControls(Portal.SingleToList(staffCoreData)), null, "", UIButtons));

                //return Json(DbORG.SingleCreateMessage(DbORG.PrimaryKeyControl(staffCoreData), DbORG.RowVersionControls(Portal.SingleToList(staffCoreData)), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }




        //[HttpPost, ValidateAntiForgeryToken]
        //public ActionResult StaffProfileCreate(StaffProfileUpdateModel model)
        //{
        //    if (!CMS.HasAction(Permissions.StaffProfile.Create, Apps.ORG))
        //    {
        //        return Json(DbORG.PermissionError());
        //    }
        //    if (!ModelState.IsValid || ActiveStaff(model)) return PartialView("~/Areas/ORG/Views/StaffProfile/_StaffProfileForm.cshtml", model);

        //    DateTime ExecutionTime = DateTime.Now;
        //    Guid EntityPK = Guid.NewGuid();
        //    if (model.UserGUID != Guid.Empty)
        //    {
        //        EntityPK = model.UserGUID;
        //    }

        //    StaffCoreData staffCoreData = new StaffCoreData();
        //    userPersonalDetails userPersonalDetails = new userPersonalDetails();
        //    userPersonalDetailsLanguage userPersonalDetailsLanguageEN = new userPersonalDetailsLanguage();
        //    userPersonalDetailsLanguage userPersonalDetailsLanguageAR = new userPersonalDetailsLanguage();

        //    staffCoreData.UserGUID = EntityPK;
        //    staffCoreData.JobTitleGUID = model.JobTitleGUID;
        //    staffCoreData.DutyStationGUID = model.DutyStationGUID;
        //    staffCoreData.DepartmentGUID = model.DepartmentGUID;
        //    staffCoreData.ReportToGUID = model.ReportToGUID;
        //    staffCoreData.Gender = model.GenderGUID;
        //    staffCoreData.NationalityGUID = model.NationalityGUID;
        //    staffCoreData.AssignmentType = model.AssignmentType;
        //    staffCoreData.PositionInOrganigram = model.PositionInOrganigram;
        //    staffCoreData.IDNumber = model.IDNumber;
        //    staffCoreData.StaffPhoto = model.StaffPhoto;
        //    staffCoreData.NationalPassportPhoto = model.NationalPassportPhoto;
        //    staffCoreData.PersonalIDPhoto = model.PersonalIDPhoto;
        //    staffCoreData.PassportPhoto = model.PassportPhoto;
        //    staffCoreData.AttendancePhoto = model.PassportPhoto;
        //    staffCoreData.DangerPay = model.DangerPay;
        //    staffCoreData.EmailAddress = model.EmailAddress;
        //    userPersonalDetails.BloodGroup = model.BloodGroup;
        //    userPersonalDetails.UserGUID = EntityPK;
        //    userPersonalDetails.CountryGUID = model.NationalityGUID;
        //    userPersonalDetails.DateOfBirth = model.DateOfBirth;
        //    userPersonalDetails.GenderGUID = model.GenderGUID;
        //    userPersonalDetailsLanguageEN.UserGUID = EntityPK;
        //    userPersonalDetailsLanguageEN.FirstName = model.FirstNameEN;
        //    userPersonalDetailsLanguageEN.Surname = model.SurnameEN;
        //    userPersonalDetailsLanguageEN.LanguageID = "EN";

        //    userPersonalDetailsLanguageAR.UserGUID = EntityPK;
        //    userPersonalDetailsLanguageAR.FirstName = model.FirstNameAR;
        //    userPersonalDetailsLanguageAR.Surname = model.SurnameAR;
        //    userPersonalDetailsLanguageAR.LanguageID = "AR";

        //    if (model.AssignmentType == Guid.Parse("fc4b2e79-2b97-4252-a50b-915b07a12310"))
        //    {
        //        staffCoreData.StaffMemberOnGTA = model.StaffMemberOnGTA;
        //        staffCoreData.GTAStartDate = model.GTAStartDate;
        //        staffCoreData.GTAEndDate = model.GTAEndDate;
        //        staffCoreData.WifeHusbandName = model.WifeHusbandName;
        //        staffCoreData.ContractTypeGUID = model.ContractTypeGUID;
        //        staffCoreData.AssignmentDuration = model.AssignmentDuration;
        //        staffCoreData.CallSign = model.CallSign;
        //        staffCoreData.BankAccountNumber = model.BankAccountNumber;
        //        staffCoreData.CurrentLocationByDate = model.CurrentLocationByDate;
        //        staffCoreData.ExpiryOfResidencyVisa = model.ExpiryOfResidencyVisa;
        //        staffCoreData.DurationOfStayInSyria = model.DurationOfStayInSyria;
        //        staffCoreData.Accommodation = model.Accommodation;
        //        staffCoreData.AccommodationStartingDate = model.AccommodationStartingDate;
        //        staffCoreData.DetailsOfMissionAssignment = model.DetailsOfMissionAssignment;
        //        staffCoreData.StartOfMissionAssignmentDate = model.StartOfMissionAssignmentDate;
        //        staffCoreData.EndOfMissionAssignmentDate = model.EndOfMissionAssignmentDate;
        //    }
        //    else if (model.AssignmentType == Guid.Parse("41651262-08ac-42dc-a691-9bbce28c95d1"))
        //    {

        //    }

        //    DbORG.Create(staffCoreData, Permissions.StaffProfile.CreateGuid, ExecutionTime, DbCMS);
        //    DbORG.Create(userPersonalDetails, Permissions.StaffProfile.CreateGuid, ExecutionTime, DbCMS);
        //    DbORG.Create(userPersonalDetailsLanguageEN, Permissions.StaffProfile.CreateGuid, ExecutionTime, DbCMS);
        //    DbORG.Create(userPersonalDetailsLanguageAR, Permissions.StaffProfile.CreateGuid, ExecutionTime, DbCMS);


        //    //List<PartialViewModel> Partials = new List<PartialViewModel>();
        //    //Partials.Add(Portal.PartialView(EntityPK, DataTableNames.StaffContactsInformationDataTable, ControllerContext, "ContactInformationsContainer"));
        //    //Partials.Add(Portal.PartialView(EntityPK, DataTableNames.StaffContactsInformationDataTable, ControllerContext, "AddressInformationsContainer"));

        //    List<UIButtons> UIButtons = new List<UIButtons>();
        //    UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.StaffProfile.Create, Apps.ORG, new UrlHelper(Request.RequestContext).Action("StaffProfile/Create", "StaffProfile", new { Area = "ORG" })), Container = "StaffProfileFormControls" });
        //    UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.StaffProfile.Update, Apps.ORG), Container = "StaffProfileFormControls" });
        //    UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.StaffProfile.Delete, Apps.ORG), Container = "StaffProfileFormControls" });

        //    try
        //    {
        //        DbORG.SaveChanges();
        //        DbCMS.SaveChanges();
        //        //return Json(DbORG.SingleCreateMessage(DbORG.PrimaryKeyControl(staffCoreData), DbORG.RowVersionControls(Portal.SingleToList(staffCoreData)), Partials, "", UIButtons));
        //        return Json(DbORG.SingleCreateMessage(DbORG.PrimaryKeyControl(staffCoreData), DbORG.RowVersionControls(Portal.SingleToList(staffCoreData)), null, "", UIButtons));

        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(DbCMS.ErrorMessage(ex.Message));
        //    }
        //}

        [HttpPost, ValidateAntiForgeryToken]


        private bool ActiveStaff(StaffProfileUpdateModel model)
        {
            if (model.StaffStatusGUID == Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E3612") && model.LastDepartureDate == null)
            {
                ModelState.AddModelError("Staff", "Kindly choose the departure date for staff has In-Active value");
                return true;
            }


            return (false);
        }

        public ActionResult StaffPhoto()
        {
            return PartialView("~/Areas/ORG/Views/StaffProfile/_StaffPhoto.cshtml");
        }

        #endregion


        #region Staff Contact Information Region
        public ActionResult StaffContactsInformationDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/ORG/Views/StaffProfile/_ContactInformationsDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<StaffContactsInformationDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<StaffContactsInformationDataTableModel>(DataTable.Filters);
            }
            var Result = (from a in DbORG.dataStaffPhone.AsNoTracking().AsExpandable().Where(x => x.UserGUID == PK && x.Active)
                          join b in DbORG.codeTablesValuesLanguages.AsNoTracking().AsExpandable().Where(x => x.LanguageID == LAN && x.Active) on a.PhoneTypeGUID equals b.ValueGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          join c in DbORG.codeTablesValuesLanguages.AsNoTracking().AsExpandable().Where(x => x.LanguageID == LAN && x.Active) on a.PhoneUsageTypeGUID equals c.ValueGUID into LJ2
                          from R2 in LJ2.DefaultIfEmpty()
                          select new StaffContactsInformationDataTableModel
                          {
                              StaffSimGUID = a.StaffSimGUID,
                              UserGUID = a.UserGUID,
                              PhoneTypeGUID = a.PhoneTypeGUID,
                              PhoneTypeDescription = R1.ValueDescription,
                              PhoneUsageTypeGUID = a.PhoneUsageTypeGUID,
                              PhoneUsageTypeDescription = R2.ValueDescription,
                              PhoneNumber = a.PhoneNumber,
                              FromDate = a.FromDate,
                              ToDate = a.ToDate,
                              Active = a.Active,
                              dataStaffPhoneRowVersion = a.dataStaffPhoneRowVersion
                          }).Where(Predicate);


            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result.ToList()), JsonRequestBehavior.AllowGet);
        }

        [Route("ORG/StaffProfile/StaffContactInformation/Create")]
        public ActionResult StaffContactsInformationCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Create, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            return PartialView("~/Areas/ORG/Views/StaffProfile/_ContactInformationsUpdateModal.cshtml", new StaffPhoneUpdateModel { UserGUID = FK });
        }

        [Route("ORG/StaffProfile/StaffContactInformation/Update/{PK}")]
        public ActionResult StaffContactsInformationUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Update, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }

            var model = (from a in DbORG.dataStaffPhone.WherePK(PK)
                         select new StaffPhoneUpdateModel
                         {
                             StaffSimGUID = a.StaffSimGUID,
                             UserGUID = a.UserGUID,
                             PhoneNumber = a.PhoneNumber,
                             TelecomCompanyOperationGUID = a.TelecomCompanyOperationGUID,
                             PhoneTypeGUID = a.PhoneTypeGUID,
                             PhoneUsageTypeGUID = a.PhoneUsageTypeGUID,
                             FromDate = a.FromDate,
                             ToDate = a.ToDate,
                             Active = a.Active,
                             dataStaffPhoneRowVersion = a.dataStaffPhoneRowVersion
                         }).FirstOrDefault();

            //if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("StaffPhones", "Configuration", new { Area = "ORG" }));

            return PartialView("~/Areas/ORG/Views/StaffProfile/_ContactInformationsUpdateModal.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffContactsInformationCreate(StaffPhoneUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Create, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }

            if (!ModelState.IsValid) return PartialView("~/Areas/ORG/Views/StaffProfile/_ContactInformationsUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();
            dataStaffPhone StaffPhone = Mapper.Map(model, new dataStaffPhone());
            StaffPhone.StaffSimGUID = EntityPK;
            DbORG.Create(StaffPhone, Permissions.StaffProfile.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.SingleUpdateMessage(DataTableNames.StaffPhonesDataTable,
                   DbORG.PrimaryKeyControl(StaffPhone),
                   DbORG.RowVersionControls(Portal.SingleToList(StaffPhone))));
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffContactsInformationUpdate(StaffPhoneUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Update, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }

            if (!ModelState.IsValid) return PartialView("~/Areas/ORG/Views/StaffProfile/_ContactInformationsUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            dataStaffPhone StaffPhone = Mapper.Map(model, new dataStaffPhone());

            DbORG.Update(StaffPhone, Permissions.StaffProfile.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.SingleUpdateMessage(DataTableNames.StaffPhonesDataTable,
                  DbORG.PrimaryKeyControl(StaffPhone),
                  DbORG.RowVersionControls(Portal.SingleToList(StaffPhone))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyStaffPhones(model.StaffSimGUID);
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffContactsInformationDelete(dataStaffPhone model)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Delete, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }

            List<dataStaffPhone> DeletedStaffPhones = DeleteStaffContactsInformation(Portal.SingleToList(model));

            try
            {
                int CommitedRows = DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.SingleDeleteMessage(DeletedStaffPhones, DataTableNames.StaffContactsInformationDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyStaffPhones(model.StaffSimGUID);
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffContactsInformationRestore(dataStaffPhone model)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Restore, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }

            //if (ActiveApplication(model))
            //{
            //    return Json(DbCMS.RecordExists());
            //}

            List<dataStaffPhone> RestoredStaffPhones = RestoreStaffContactsInformation(Portal.SingleToList(model));

            try
            {
                int CommitedRows = DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.SingleRestoreMessage(RestoredStaffPhones, DataTableNames.StaffContactsInformationDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyStaffPhones(model.StaffSimGUID);
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult StaffContactsInformationDataTableDelete(List<dataStaffPhone> models)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Delete, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            List<dataStaffPhone> DeletedStaffPhones = DeleteStaffContactsInformation(models);

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.PartialDeleteMessage(DeletedStaffPhones, models, DataTableNames.StaffContactsInformationDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult StaffContactsInformationDataTableRestore(List<dataStaffPhone> models)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Restore, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            List<dataStaffPhone> RestoredStaffPhones = RestoreStaffContactsInformation(models);

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.PartialRestoreMessage(RestoredStaffPhones, models, DataTableNames.StaffContactsInformationDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        private List<dataStaffPhone> DeleteStaffContactsInformation(List<dataStaffPhone> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<dataStaffPhone> DeletedStaffPhones = new List<dataStaffPhone>();

            //THIS IS FOR PERMISSION TEST//
            //DO NOT DELETE THIS CODE
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = (from a in DbCMS.codeApplications
            //                    from f in DbCMS.codeApplicationsFactorForTest
            //                    where a.ApplicationGUID == f.ApplicationGUID
            //                    select new { a.ApplicationGUID, a.codeApplicationsRowVersion, f.OperationGUID, f.OrganizationGUID, f.CountryGUID }).AsQueryable().ToString();

            string query = DbORG.QueryBuilder(models, Permissions.StaffProfile.DeleteGuid, SubmitTypes.Delete, "");

            var Records = DbORG.Database.SqlQuery<dataStaffPhone>(query).ToList();
            foreach (var record in Records)
            {
                DeletedStaffPhones.Add(DbORG.Delete(record, ExecutionTime, Permissions.StaffProfile.DeleteGuid, DbCMS));
            }


            return DeletedStaffPhones;
        }

        private List<dataStaffPhone> RestoreStaffContactsInformation(List<dataStaffPhone> models)
        {
            Guid DeleteActionGUID = Permissions.StaffProfile.DeleteGuid;
            Guid RestoreActionGUID = Permissions.StaffProfile.RestoreGuid;
            DateTime RestoringTime = DateTime.Now;

            List<dataStaffPhone> RestoredStaffPhones = new List<dataStaffPhone>();

            //THIS IS FOR PERMISSION TEST//
            //DO NOT DELETE THIS CODE
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = (from a in DbCMS.codeApplications
            //                    from f in DbCMS.codeApplicationsFactorForTest
            //                    where a.ApplicationGUID == f.ApplicationGUID
            //                    select new
            //                    {
            //                        a.ApplicationGUID,
            //                        a.codeApplicationsRowVersion,
            //                        C2 = f.OperationGUID + "," + f.OrganizationGUID,
            //                    }).AsQueryable().ToString();//.Replace("[C2]", "[FactorsToken]");

            string query = DbORG.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, "");


            var Records = DbORG.Database.SqlQuery<dataStaffPhone>(query).ToList();
            foreach (var record in Records)
            {
                //if (!ActiveApplication(record))
                //{
                RestoredStaffPhones.Add(DbORG.Restore(record, DeleteActionGUID, RestoreActionGUID, RestoringTime, DbCMS));
                //}
            }
            return RestoredStaffPhones;
        }

        private JsonResult ConcrrencyStaffPhones(Guid PK)
        {
            StaffPhoneUpdateModel dbModel = new StaffPhoneUpdateModel();

            var StaffPhone = DbORG.dataStaffPhone.Where(a => a.StaffSimGUID == PK).FirstOrDefault();
            var dbStaffPhone = DbORG.Entry(StaffPhone).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbStaffPhone, dbModel);


            if (StaffPhone.dataStaffPhoneRowVersion.SequenceEqual(dbModel.dataStaffPhoneRowVersion))
            {
                return Json(DbORG.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbORG, dbModel, null));
        }

        #endregion
        public ActionResult StaffAddressInformationDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/ORG/Views/StaffProfile/_AddressInformationsDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<StaffContactsInformationDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<StaffContactsInformationDataTableModel>(DataTable.Filters);
            }
            var Result = (from a in DbORG.dataStaffPhone.AsNoTracking().AsExpandable().Where(x => x.UserGUID == PK && x.Active)
                          join b in DbORG.codeTablesValuesLanguages.AsNoTracking().AsExpandable().Where(x => x.LanguageID == LAN && x.Active) on a.PhoneTypeGUID equals b.ValueGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          join c in DbORG.codeTablesValuesLanguages.AsNoTracking().AsExpandable().Where(x => x.LanguageID == LAN && x.Active) on a.PhoneUsageTypeGUID equals c.ValueGUID into LJ2
                          from R2 in LJ2.DefaultIfEmpty()
                          select new StaffContactsInformationDataTableModel
                          {
                              StaffSimGUID = a.StaffSimGUID,

                              UserGUID = a.UserGUID,
                              PhoneTypeGUID = a.PhoneTypeGUID,
                              PhoneTypeDescription = R1.ValueDescription,
                              PhoneUsageTypeGUID = a.PhoneUsageTypeGUID,
                              PhoneUsageTypeDescription = R2.ValueDescription,
                              PhoneNumber = a.PhoneNumber,
                              FromDate = a.FromDate,
                              ToDate = a.ToDate,
                              Active = a.Active,
                              dataStaffPhoneRowVersion = a.dataStaffPhoneRowVersion
                          }).Where(Predicate);


            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result.ToList()), JsonRequestBehavior.AllowGet);
        }

        #region Bank Accounts


        //[Route("ORG/StaffBankAccountDataTable/{PK}")]
        public ActionResult StaffBankAccountDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/ORG/Views/StaffProfile/_BanksDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<dataStaffBankAccount, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<dataStaffBankAccount>(DataTable.Filters);
            }

            var Result = DbORG.dataStaffBankAccount.AsNoTracking().AsExpandable().Where(x => x.UserGUID == PK && x.Active == true).Where(Predicate)
                              .Select(x => new
                              {
                                  x.StaffBankAccountGUID,
                                  x.UserGUID,
                                  BankGUID = x.BankGUID,
                                  x.BankHolderNameAr,
                                  x.BankHolderNameEn,
                                  BankAccountNumber = x.AccountNumber,
                                  BankName = x.codeBank.codeBankLanguage.Where(f => f.LanguageID == LAN).FirstOrDefault().BankDescription,
                                  x.dataStaffBankAccountRowVersion
                              });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }



        public ActionResult StaffBankAccountGetUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Update, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            return PartialView("~/Areas/ORG/Views/StaffProfile/_StaffBankUpdateModal.cshtml", DbORG.dataStaffBankAccount.Find(PK));
        }
        public ActionResult StaffBankAccountUpdate(dataStaffBankAccount model)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Update, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            if (!ModelState.IsValid || model.BankGUID == null || model.AccountNumber == null || model.BankHolderNameEn == null) return PartialView("~/Areas/ORG/Views/StaffProfile/_StaffBankUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbORG.Update(model, Permissions.StaffProfile.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.SingleUpdateMessage(DataTableNames.StaffBankAccountDataTable,
                    DbORG.PrimaryKeyControl(model),
                    DbORG.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyStaffBankAccount(model.StaffBankAccountGUID);
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }
        public ActionResult StaffBankAccountCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Create, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            return PartialView("~/Areas/ORG/Views/StaffProfile/_StaffBankUpdateModal.cshtml",
                new dataStaffBankAccount { UserGUID = FK });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffBankAccountCreate(dataStaffBankAccount model)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Create, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            if (!ModelState.IsValid || ActiveStaffBankAccount(model)) return PartialView("~/Areas/ORG/Views/StaffProfile/_StaffBankUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbORG.Create(model, Permissions.StaffProfile.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.SingleUpdateMessage(DataTableNames.StaffBankAccountDataTable, DbORG.PrimaryKeyControl(model), DbORG.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]


        public ActionResult StaffBankAccountDeletePerTable(Guid PK)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Delete, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            return PartialView("~/Areas/ORG/Views/StaffProfile/_BankDelete.cshtml",
                new dataStaffBankAccount { StaffBankAccountGUID = PK });
        }
        [HttpPost, ValidateAntiForgeryToken]

        public ActionResult StaffBankAccountDeletePerTable(dataStaffBankAccount model)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Delete, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            DateTime ExecutionTime = DateTime.Now;
            var currBank = DbORG.dataStaffBankAccount.Where(x => x.StaffBankAccountGUID == model.StaffBankAccountGUID).FirstOrDefault();
            currBank.Active = false;
            //DbORG.Update(currBank, Permissions.StaffProfile.UpdateGuid, ExecutionTime, DbCMS);
            try
            {

                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                //return RedirectToAction("ConfirmReceivingModelEmail", new { id = model.ItemOutputDetailGUID });
                //return View("Login");
                dataStaffBankAccount myModel = DbORG.dataStaffBankAccount.Find(model.StaffBankAccountGUID);
                List<PartialViewModel> Partials = new List<PartialViewModel>();


                List<UIButtons> UIButtons = new List<UIButtons>();
                List<dataStaffBankAccount> deleted = new List<dataStaffBankAccount>();
                deleted.Add(DbORG.dataStaffBankAccount.FirstOrDefault());
                UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.StaffProfile.Create, Apps.ORG, new UrlHelper(Request.RequestContext).Action("StaffProfile/Create", "StaffProfile", new { Area = "ORG" })), Container = "StaffProfileFormControls" });
                UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.StaffProfile.Update, Apps.ORG), Container = "StaffProfileFormControls" });
                UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.StaffProfile.Delete, Apps.ORG), Container = "StaffProfileFormControls" });
                return Json(DbORG.SingleDeleteMessage(deleted, DataTableNames.StaffBankAccountDataTable));

                //  return Json(DbORG.SingleCreateMessage(DbORG.PrimaryKeyControl(myModel), DbORG.RowVersionControls(Portal.SingleToList(myModel)), null, "", UIButtons));



                //return Json(DbAHD.SingleCreateMessage(DbORG.PrimaryKeyControl(model), DbAHD.RowVersionControls(model, model), Partials, "", UIButtons));
                //return Json(DbTBS.SingleCreateMessage("Record has been deleted !"));

                //return RedirectToAction("StaffProfileUpdate", new { PK = model.UserGUID });
                // return View("~/Areas/ORG/Views/StaffProfile/Confirm.cshtml", new dataStaffBankAccount());

            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }


        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffBankAccountDelete(dataStaffBankAccount model)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Delete, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            List<dataStaffBankAccount> DeletedLanguages = DeleteStaffBankAccount(new List<dataStaffBankAccount> { model });

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.SingleDeleteMessage(DeletedLanguages, DataTableNames.StaffBankAccountDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyStaffBankAccount(model.StaffBankAccountGUID);
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffBankAccountRestore(dataStaffBankAccount model)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Restore, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            if (ActiveStaffBankAccount(model))
            {
                return Json(DbORG.RecordExists());
            }

            List<dataStaffBankAccount> RestoredLanguages = RestoreStaffBankAccount(Portal.SingleToList(model));

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.SingleRestoreMessage(RestoredLanguages, DataTableNames.StaffBankAccountDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyStaffBankAccount(model.StaffBankAccountGUID);
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }


        [HttpPost, ValidateAntiForgeryToken]

        public JsonResult StaffBankAccountDataTableDelete(List<dataStaffBankAccount> models)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Delete, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            List<dataStaffBankAccount> DeletedLanguages = DeleteStaffBankAccount(models);

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.StaffBankAccountDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult StaffBankAccountDataTableRestore(List<dataStaffBankAccount> models)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Restore, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            List<dataStaffBankAccount> RestoredLanguages = RestoreStaffBankAccount(models);

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.StaffBankAccountDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        private List<dataStaffBankAccount> DeleteStaffBankAccount(List<dataStaffBankAccount> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataStaffBankAccount> DeletedStaffBankAccount = new List<dataStaffBankAccount>();

            string query = DbORG.QueryBuilder(models, Permissions.StaffProfile.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbORG.Database.SqlQuery<dataStaffBankAccount>(query).ToList();

            foreach (var language in languages)
            {
                DeletedStaffBankAccount.Add(DbORG.Delete(language, ExecutionTime, Permissions.StaffProfile.DeleteGuid, DbCMS));
            }

            return DeletedStaffBankAccount;
        }

        private List<dataStaffBankAccount> RestoreStaffBankAccount(List<dataStaffBankAccount> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataStaffBankAccount> RestoredLanguages = new List<dataStaffBankAccount>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbORG.QueryBuilder(models, Permissions.StaffProfile.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbORG.Database.SqlQuery<dataStaffBankAccount>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveStaffBankAccount(language))
                {
                    RestoredLanguages.Add(DbORG.Restore(language, Permissions.StaffProfile.DeleteGuid, Permissions.StaffProfile.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyStaffBankAccount(Guid PK)
        {
            dataStaffBankAccount dbModel = new dataStaffBankAccount();

            var Language = DbORG.dataStaffBankAccount.Where(l => l.StaffBankAccountGUID == PK).FirstOrDefault();
            var dbLanguage = DbORG.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.dataStaffBankAccountRowVersion.SequenceEqual(dbModel.dataStaffBankAccountRowVersion))
            {
                return Json(DbORG.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbORG, dbModel, "LanguagesContainer"));
        }

        private bool ActiveStaffBankAccount(dataStaffBankAccount model)
        {
            int LanguageID = DbORG.dataStaffBankAccount
                                  .Where(x => x.BankGUID == model.BankGUID &&
                                              x.UserGUID == model.UserGUID &&
                                              x.AccountNumber == model.AccountNumber &&
                                              x.Active).Count();
            if (LanguageID > 0)
            {
                //Fix
                ModelState.AddModelError("LanguageID", "Bank Already Exist");
            }

            return (LanguageID > 0);
        }

        #endregion Banks


        #region Nationaol Passports


        //[Route("ORG/StaffBankAccountDataTable/{PK}")]
        public ActionResult StaffNationalPassportDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/ORG/Views/StaffProfile/_NationalPassportDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<StaffCorePassportDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<StaffCorePassportDataTableModel>(DataTable.Filters);
            }

            var Result = DbORG.dataStaffCorePassport.AsNoTracking().AsExpandable().Where(x => x.UserGUID == PK)
                              .Select(x => new StaffCorePassportDataTableModel
                              {
                                  StaffCorePassportGUID = x.StaffCorePassportGUID,
                                  UserGUID = x.UserGUID.ToString(),
                                  NationalPassportNumber = x.NationalPassportNumber,
                                  NationalPassportDateOfIssue = x.NationalPassportDateOfIssue,
                                  NationalPassportDateOfExpiry = x.NationalPassportDateOfExpiry,
                                  Active = x.Active,
                                  dataStaffCorePassportRowVersion = x.dataStaffCorePassportRowVersion
                              }).Where(Predicate);

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult StaffCorePassportCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Create, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            return PartialView("~/Areas/ORG/Views/StaffProfile/_NationalPassportUpdateModal.cshtml",
                new StaffCorePassportUpdateModel { UserGUID = FK });
        }

        public ActionResult StaffCorePassportUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Update, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            return PartialView("~/Areas/ORG/Views/StaffProfile/_NationalPassportUpdateModal.cshtml", DbORG.dataStaffCorePassport.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffCorePassportCreate(dataStaffCorePassport model)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Create, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            if (!ModelState.IsValid || (model.NationalPassportNumber == null || (model.NationalPassportDateOfIssue >= model.NationalPassportDateOfExpiry) || model.NationalPassportDateOfIssue == null || model.NationalPassportDateOfExpiry == null)) return PartialView("~/Areas/ORG/Views/StaffProfile/_NationalPassportUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbORG.Create(model, Permissions.StaffProfile.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.SingleUpdateMessage(DataTableNames.StaffNationalPassportDataTable, DbORG.PrimaryKeyControl(model), DbORG.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffCorePassportUpdate(dataStaffCorePassport model)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Update, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            if (!ModelState.IsValid || model.UserGUID == null || model.NationalPassportNumber == null || (model.NationalPassportDateOfIssue >= model.NationalPassportDateOfExpiry) || model.NationalPassportDateOfIssue == null || model.NationalPassportDateOfExpiry == null) return PartialView("~/Areas/ORG/Views/StaffProfile/StaffBankUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbORG.Update(model, Permissions.StaffProfile.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.SingleUpdateMessage(DataTableNames.StaffNationalPassportDataTable,
                    DbORG.PrimaryKeyControl(model),
                    DbORG.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyStaffCorePassport(model.StaffCorePassportGUID);
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffCorePassportDelete(dataStaffCorePassport model)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Delete, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            List<dataStaffCorePassport> DeletedLanguages = DeleteStaffCorePassport(new List<dataStaffCorePassport> { model });

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.SingleDeleteMessage(DeletedLanguages, DataTableNames.StaffNationalPassportDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyStaffCorePassport(model.StaffCorePassportGUID);
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffCorePassportRestore(dataStaffCorePassport model)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Restore, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            if (ActiveStaffCorePassport(model))
            {
                return Json(DbORG.RecordExists());
            }

            List<dataStaffCorePassport> RestoredLanguages = RestoreStaffCorePassport(Portal.SingleToList(model));

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.SingleRestoreMessage(RestoredLanguages, DataTableNames.StaffNationalPassportDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyStaffCorePassport(model.StaffCorePassportGUID);
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult StaffNationalPassportDataTableDelete(List<dataStaffCorePassport> models)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Delete, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            List<dataStaffCorePassport> DeletedLanguages = DeleteStaffCorePassport(models);

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.StaffNationalPassportDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult StaffNationalPassportDataTableRestore(List<dataStaffCorePassport> models)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Restore, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            List<dataStaffCorePassport> RestoredLanguages = RestoreStaffCorePassport(models);

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.StaffNationalPassportDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        private List<dataStaffCorePassport> DeleteStaffCorePassport(List<dataStaffCorePassport> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataStaffCorePassport> DeletedStaffBankAccount = new List<dataStaffCorePassport>();

            string query = DbORG.QueryBuilder(models, Permissions.StaffProfile.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbORG.Database.SqlQuery<dataStaffCorePassport>(query).ToList();

            foreach (var language in languages)
            {
                DeletedStaffBankAccount.Add(DbORG.Delete(language, ExecutionTime, Permissions.StaffProfile.DeleteGuid, DbCMS));
            }

            return DeletedStaffBankAccount;
        }

        private List<dataStaffCorePassport> RestoreStaffCorePassport(List<dataStaffCorePassport> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataStaffCorePassport> RestoredLanguages = new List<dataStaffCorePassport>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbORG.QueryBuilder(models, Permissions.StaffProfile.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbORG.Database.SqlQuery<dataStaffCorePassport>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveStaffCorePassport(language))
                {
                    RestoredLanguages.Add(DbORG.Restore(language, Permissions.StaffProfile.DeleteGuid, Permissions.StaffProfile.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyStaffCorePassport(Guid PK)
        {
            dataStaffCorePassport dbModel = new dataStaffCorePassport();

            var Language = DbORG.dataStaffCorePassport.Where(l => l.StaffCorePassportGUID == PK).FirstOrDefault();
            var dbLanguage = DbORG.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.dataStaffCorePassportRowVersion.SequenceEqual(dbModel.dataStaffCorePassportRowVersion))
            {
                return Json(DbORG.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbORG, dbModel, "LanguagesContainer"));
        }

        private bool ActiveStaffCorePassport(dataStaffCorePassport model)
        {
            int LanguageID = DbORG.dataStaffCorePassport
                                  .Where(x =>
                                              x.UserGUID == model.UserGUID &&
                                              x.NationalPassportNumber == model.NationalPassportNumber &&

                                              x.Active).Count();
            if (LanguageID > 0)
            {
                //Fix
                ModelState.AddModelError("LanguageID", "Already Exist");
            }

            return (LanguageID > 0);
        }

        #endregion Banks


        #region Nationaol Documents


        //[Route("ORG/StaffBankAccountDataTable/{PK}")]
        public ActionResult StaffCoreDocumentDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/ORG/Views/StaffProfile/_DocumentDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<StaffCoreDocumentDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<StaffCoreDocumentDataTableModel>(DataTable.Filters);
            }

            Guid myDocumentTypeGUID = Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E3733");
            var Result = (from a in DbORG.dataStaffCoreDocument.AsExpandable().Where(x => x.Active && (x.UserGUID == PK))

                          join b in DbORG.codeTablesValuesLanguages.AsExpandable().Where(x => x.Active && x.LanguageID == LAN && x.codeTablesValues.TableGUID == myDocumentTypeGUID) on a.DocumentTypeGUID equals b.ValueGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          select new StaffCoreDocumentDataTableModel
                          {
                              StaffCoreDocumentGUID = a.StaffCoreDocumentGUID,
                              UserGUID = a.UserGUID.ToString(),
                              DocumentNumber = a.DocumentNumber,
                              DocumentTypeGUID = a.DocumentTypeGUID.ToString(),
                              DocumentType = R1.ValueDescription,
                              DocumentDateOfIssue = a.DocumentDateOfIssue,
                              DocumentDateOfExpiry = a.DocumentDateOfExpiry,
                              Active = a.Active,
                              dataStaffCoreDocumentRowVersion = a.dataStaffCoreDocumentRowVersion

                          }).Where(Predicate);


            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult StaffCoreDocumentCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Create, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            return PartialView("~/Areas/ORG/Views/StaffProfile/_DocumentUpdateModal.cshtml",
                new StaffCoreDocumentUpdateModel { UserGUID = FK });
        }

        public ActionResult DownloadStaffDocumentFile(Guid id)
        {

            if (!CMS.HasAction(Permissions.StaffProfile.Create, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }

            var model = DbORG.dataStaffCoreDocument.Where(x => x.StaffCoreDocumentGUID == id).FirstOrDefault();
            var fullPath = model.StaffCoreDocumentGUID + "." + model.ReportExtensionType;


            string sourceFile = Server.MapPath("~/Uploads/ORG/UploadedDocuments/" + model.UserGUID + "/" + fullPath);


            byte[] fileBytes = System.IO.File.ReadAllBytes(sourceFile);

            string fileName = DateTime.Now.ToString("yyMMdd") + fullPath;
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);








            // return Json(DbCMS.SingleUpdateMessage(null, null, null, null, "Please Wait...."));
        }



        [HttpPost]
        public FineUploaderResult UploadStaffCoreDocuments(FineUpload upload, Guid userGUID, string documentNumber, Guid DocumentTypeGUID, DateTime documentDateOfIssue, DateTime? documentDateOfExpiry)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Create, Apps.ORG))
            {
                return new FineUploaderResult(false, null);
            }

            return new FineUploaderResult(true, new { path = UploadDocument(upload, userGUID, documentNumber, DocumentTypeGUID, documentDateOfIssue, documentDateOfExpiry), success = true });
        }

        public string UploadDocument(FineUpload upload, Guid userGUID, string documentNumber, Guid DocumentTypeGUID, DateTime documentDateOfIssue, DateTime? documentDateOfExpiry)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Create, Apps.ORG))
            {
                return "";
            }
            var _stearm = upload.InputStream;
            DateTime ExecutionTime = DateTime.Now;
            //string _ext = Path.GetExtension(upload.FileName).Split('.')[1];
            dataStaffCoreDocument documentUplod = new dataStaffCoreDocument();
            documentUplod.StaffCoreDocumentGUID = Guid.NewGuid();
            //string FilePath = Server.MapPath("~/Areas/ORG/UploadedDocuments/" + documentUplod.ItemIntpuDetailUploadedDocumentGUID + _ext);

            string _ext = Path.GetExtension(upload.FileName).Split('.')[1];

            string FolderPath = Server.MapPath("~/Uploads/ORG/UploadedDocuments/" + userGUID.ToString());
            Directory.CreateDirectory(FolderPath);
            //int LatestFileVersion = 0;
            //try { LatestFileVersion = (from a in DbPPA.PPAFileVersion.Where(x => x.PPAOriginalFileGUID == PPAOriginalFileGUID && x.FileActionByUserGUID == UserGUID) select a.FileVersion).Max(); } catch { }
            //if (LatestFileVersion == -1) LatestFileVersion = 0;



            string FilePath = FolderPath + "/" + documentUplod.StaffCoreDocumentGUID.ToString() + "." + _ext;

            using (var fileStream = System.IO.File.Create(FilePath))
            {
                upload.InputStream.Seek(0, SeekOrigin.Begin);
                upload.InputStream.CopyTo(fileStream);
            }
            documentUplod.UserGUID = userGUID;
            documentUplod.DocumentNumber = documentNumber;
            documentUplod.ReportExtensionType = _ext;
            documentUplod.DocumentTypeGUID = DocumentTypeGUID;
            documentUplod.DocumentDateOfIssue = documentDateOfIssue;
            documentUplod.DocumentDateOfExpiry = documentDateOfExpiry;

            //documentUplod.Comments = ItemInputDetailGUID;
            //documentUplod.CreatedByGUID = UserGUID;
            //documentUplod.CreatedDate = ExecutionTime;
            DbORG.Create(documentUplod, Permissions.StaffProfile.CreateGuid, ExecutionTime, DbCMS);
            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }


            //Server.MapPath("~/Areas/ORG/temp/Standard_Operating_Procedure_for_issuing_ICT_equipment_to_staff" + DateTime.Now.ToBinary() + ".pdf");


            return "~/Areas/ORG/UploadedDocuments/" + documentUplod.StaffCoreDocumentGUID + ".xlsx";
        }




        public ActionResult StaffCoreDocumentUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Update, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            StaffCoreDocumentUpdateModel model = DbORG.dataStaffCoreDocument.Where(x => x.StaffCoreDocumentGUID == PK).Select(f => new StaffCoreDocumentUpdateModel
            {

                DocumentTypeGUID = (Guid)f.DocumentTypeGUID,
                DocumentDateOfExpiry = f.DocumentDateOfExpiry,
                DocumentDateOfIssue = f.DocumentDateOfIssue,
                DocumentNumber = f.DocumentNumber,
                StaffCoreDocumentGUID = f.StaffCoreDocumentGUID,
                UserGUID = (Guid)f.UserGUID,
                Active = f.Active

            }).FirstOrDefault();
            return PartialView("~/Areas/ORG/Views/StaffProfile/_DocumentUpdateModal.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffCoreDocumentCreate(dataStaffCoreDocument model)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Create, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            if (!ModelState.IsValid || (model.DocumentNumber == null || (model.DocumentDateOfIssue >= model.DocumentDateOfExpiry) || model.DocumentDateOfIssue == null || model.DocumentDateOfExpiry == null)) return PartialView("~/Areas/ORG/Views/StaffProfile/_DocumentUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbORG.Create(model, Permissions.StaffProfile.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.SingleUpdateMessage(DataTableNames.StaffCoreDocumentDataTable, DbORG.PrimaryKeyControl(model), DbORG.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffCoreDocumentUpdate(dataStaffCoreDocument model)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Update, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            if (!ModelState.IsValid || model.UserGUID == null || model.DocumentNumber == null || (model.DocumentDateOfIssue >= model.DocumentDateOfExpiry) || model.DocumentDateOfIssue == null || model.DocumentDateOfExpiry == null) return PartialView("~/Areas/ORG/Views/StaffProfile/StaffBankUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbORG.Update(model, Permissions.StaffProfile.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.SingleUpdateMessage(DataTableNames.StaffCoreDocumentDataTable,
                    DbORG.PrimaryKeyControl(model),
                    DbORG.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyStaffCoreDocument(model.StaffCoreDocumentGUID);
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffCoreDocumentDelete(dataStaffCoreDocument model)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Delete, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            List<dataStaffCoreDocument> DeletedLanguages = DeleteStaffCoreDocument(new List<dataStaffCoreDocument> { model });

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.SingleDeleteMessage(DeletedLanguages, DataTableNames.StaffCoreDocumentDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyStaffCoreDocument(model.StaffCoreDocumentGUID);
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffCoreDocumentRestore(dataStaffCoreDocument model)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Restore, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            if (ActiveStaffCoreDocument(model))
            {
                return Json(DbORG.RecordExists());
            }

            List<dataStaffCoreDocument> RestoredLanguages = RestoreStaffDocument(Portal.SingleToList(model));

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.SingleRestoreMessage(RestoredLanguages, DataTableNames.StaffCoreDocumentDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyStaffCoreDocument(model.StaffCoreDocumentGUID);
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult StaffCoreDocumentDataTableDelete(List<dataStaffCoreDocument> models)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Delete, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            List<dataStaffCoreDocument> DeletedLanguages = DeleteStaffCoreDocument(models);

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.StaffCoreDocumentDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult StaffCoreDocumentDataTableModelRestore(List<dataStaffCoreDocument> models)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Restore, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            List<dataStaffCoreDocument> RestoredLanguages = RestoreStaffDocument(models);

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.StaffCoreDocumentDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        private List<dataStaffCoreDocument> DeleteStaffCoreDocument(List<dataStaffCoreDocument> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataStaffCoreDocument> DeletedStaffBankAccount = new List<dataStaffCoreDocument>();

            string query = DbORG.QueryBuilder(models, Permissions.StaffProfile.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbORG.Database.SqlQuery<dataStaffCoreDocument>(query).ToList();

            foreach (var language in languages)
            {
                DeletedStaffBankAccount.Add(DbORG.Delete(language, ExecutionTime, Permissions.StaffProfile.DeleteGuid, DbCMS));
            }

            return DeletedStaffBankAccount;
        }

        private List<dataStaffCoreDocument> RestoreStaffDocument(List<dataStaffCoreDocument> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataStaffCoreDocument> RestoredLanguages = new List<dataStaffCoreDocument>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbORG.QueryBuilder(models, Permissions.StaffProfile.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbORG.Database.SqlQuery<dataStaffCoreDocument>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveStaffCoreDocument(language))
                {
                    RestoredLanguages.Add(DbORG.Restore(language, Permissions.StaffProfile.DeleteGuid, Permissions.StaffProfile.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyStaffCoreDocument(Guid PK)
        {
            dataStaffCoreDocument dbModel = new dataStaffCoreDocument();

            var Language = DbORG.dataStaffCoreDocument.Where(l => l.StaffCoreDocumentGUID == PK).FirstOrDefault();
            var dbLanguage = DbORG.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.dataStaffCoreDocumentRowVersion.SequenceEqual(dbModel.dataStaffCoreDocumentRowVersion))
            {
                return Json(DbORG.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbORG, dbModel, "LanguagesContainer"));
        }

        private bool ActiveStaffCoreDocument(dataStaffCoreDocument model)
        {
            int LanguageID = DbORG.dataStaffCoreDocument
                                  .Where(x =>
                                              x.UserGUID == model.UserGUID &&
                                              x.DocumentNumber == model.DocumentNumber &&

                                              x.Active).Count();
            if (LanguageID > 0)
            {
                //Fix
                ModelState.AddModelError("LanguageID", "Already Exist");
            }

            return (LanguageID > 0);
        }

        #endregion Banks

        #region Change Lists
        public JsonResult GetStaffBirthCityAR(Guid myGuid)
        {
            var couuntry = DbAHD.codeCountriesLanguages.Where(x => x.CountryGUID == myGuid && x.LanguageID == "AR").FirstOrDefault();
            return Json(new { country = couuntry != null ? couuntry.CountryDescription : "" }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetStaffJobTitleAR(Guid myGuid)
        {
            var jobtitle = DbAHD.codeJobTitlesLanguages.Where(x => x.JobTitleGUID == myGuid && x.LanguageID == "AR").FirstOrDefault();
            return Json(new { jobtitle = jobtitle != null ? jobtitle.JobTitleDescription : "" }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Photo
        public ActionResult Photo(string id)
        {
            ViewBag.userGUID = id;
            return PartialView("~/Areas/ORG/Views/Photo/_Photo.cshtml");

        }


        [HttpPost]
        public JsonResult UploadStaffPhoto(FineUpload upload, string PK)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Create, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }

            Guid myGUID = Guid.Parse(PK);
            var userAccount = DbORG.userAccounts.Where(x => x.UserGUID == myGUID).FirstOrDefault();

            try
            {
                using (var image = Image.FromStream(upload.InputStream))
                {
                    //Original Size
                    var dir = new FileInfo(WebConfigurationManager.AppSettings["DataFolder"] + "\\Users\\ProfilePhotos\\").Directory;
                    if (dir != null) dir.Create();
                    image.Save(WebConfigurationManager.AppSettings["DataFolder"] + "\\Users\\ProfilePhotos\\LG_" + userAccount.UserGUID + ".jpg", ImageFormat.Jpeg);

                    //1024 x 1024 pixel
                    using (var newImage = ScaleImage(image, 300, 300))
                    {
                        var directory = new FileInfo(WebConfigurationManager.AppSettings["DataFolder"] + "\\Users\\ProfilePhotos\\").Directory;
                        if (directory != null) directory.Create();
                        newImage.Save(WebConfigurationManager.AppSettings["DataFolder"] + "\\Users\\ProfilePhotos\\" + userAccount.UserGUID + ".jpg", ImageFormat.Jpeg);
                    }
                    //100 x 100 pixel
                    using (var newImage = ScaleImage(image, 100, 100))
                    {
                        var directory = new FileInfo(WebConfigurationManager.AppSettings["DataFolder"] + "\\Users\\ProfilePhotos\\").Directory;
                        if (directory != null) directory.Create();
                        newImage.Save(WebConfigurationManager.AppSettings["DataFolder"] + "\\Users\\ProfilePHotos\\XS_" + userAccount.UserGUID + ".jpg", ImageFormat.Jpeg);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { Error = ex.Message });
            }

            string HeaderPath = WebConfigurationManager.AppSettings["MediaURL"] + "Users/ProfilePhotos/XS_" + userAccount.UserGUID + ".jpg";
            string ProfilePath = WebConfigurationManager.AppSettings["MediaURL"] + "Users/ProfilePhotos/" + userAccount.UserGUID + ".jpg";

            // the anonymous object in the result below will be convert to json and set back to the browser
            return Json(new { HeaderPath = HeaderPath, ProfilePath = ProfilePath });
        }

        private Image ScaleImage(Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);

            return newImage;
        }
        #endregion
        #region  Phone Dirctory 

        // GET: ORG/IndexStaffPhoneDirectory
        [Route("ORG/StaffProfile/IndexStaffPhoneDirectory")]
        public ActionResult IndexStaffPhoneDirectory()
        {

            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Update, Apps.WMS))
            {
                return Json(DbORG.PermissionError());
            }
            return View("~/Areas/ORG/Views/StaffProfile/IndexPhoneDirectory.cshtml");

        }

        [Route("ORG/StaffProfile/StaffPhoneDirectoryDataTable")]
        public ActionResult StaffPhoneDirectoryDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<PhoneDirectoryDataTable, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<PhoneDirectoryDataTable>(DataTable.Filters);
            }
            var All = (from a in DbORG.v_dataSyriaPhoneDirectory


                       select new PhoneDirectoryDataTable
                       {

                           PhoneDirectoryGUID = a.PhoneDirectoryGUID.ToString(),
                           UserGUID = a.UserGUID.ToString(),
                           DutyStation = a.DutyStation,
                           ContactType = a.HolderType,
                           PhoneHolderTypeGUID = a.PhoneHolderTypeGUID.ToString(),
                           DutyStationGUID = a.DutyStationGUID.ToString(),
                           DepartmentGUID = a.DepartmentGUID.ToString(),
                           JobTitleGUID = a.JobTitleGUID.ToString(),

                           CallSign = a.CallSign,

                           DepartmentDescription = a.DepartmentDescription,
                           FullName = a.FullName,

                           EmailAddress = a.EmailAddress,
                           JobTitleDescription = a.JobTitle,
                           OfficialExtensionNumber = a.OfficialExtensionNumber,
                           OfficialMobileNumber = a.OfficialMobileNumber,
                           HomeTelephoneNumberLandline = a.HomeTelephoneNumberLandline,
                           SATPhoneNumber = a.SATPhoneNumber,
                           HQExtensionNumber = a.HQExtensionNumber,
                           DutyStationExtensionNumber = a.DutyStationExtensionNumber,










                           Active = a.Active,
                           dataPhoneDirectoryRowVersion = a.dataPhoneDirectoryRowVersion,
                       }).Distinct().Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<PhoneDirectoryDataTable> Result = Mapper.Map<List<PhoneDirectoryDataTable>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult StaffPhoneDirectoryCreate()
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Create, Apps.WMS))
            {
                return Json(DbORG.PermissionError());
            }
            return PartialView("~/Areas/ORG/Views/StaffProfile/_StaffPhoneDirectory.cshtml",
                new PhoneDirectoryUpdateModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffPhoneDirectoryCreate(PhoneDirectoryUpdateModel mymodel)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Create, Apps.WMS))
            {
                return Json(DbORG.PermissionError());
            }
            if (!ModelState.IsValid || mymodel.PhoneHolderTypeGUID == Guid.Empty || mymodel.PhoneHolderTypeGUID == null || String.IsNullOrEmpty(mymodel.FullName))
                return PartialView("~/Areas/ORG/Views/StaffProfile/_StaffPhoneDirectory.cshtml", mymodel);

            DateTime ExecutionTime = DateTime.Now;
            dataPhoneDirectory model = Mapper.Map(mymodel, new dataPhoneDirectory());

            DbORG.Create(model, Permissions.WarehouseItemsEntry.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.SingleUpdateMessage(DataTableNames.StaffPhoneDirectoryDataTable, DbORG.PrimaryKeyControl(model), DbORG.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        [HttpPost]
        public ActionResult StaffProfileICTUpdate(StaffProfileUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.StaffProfileICTSection.Update, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            //if (!ModelState.IsValid || ActiveStaff(model)) return PartialView("~/Areas/ORG/Views/StaffProfile/_StaffProfileForm.cshtml", model);
            //if (model.EmploymentID == null)
            //{
            //    return Json(DbORG.PermissionError());
            //}

            DateTime ExecutionTime = DateTime.Now;

            StaffCoreData staffCoreData = (from a in DbORG.StaffCoreData.Where(x => x.Active && x.UserGUID == model.UserGUID) select a).FirstOrDefault();
            staffCoreData.HomeTelephoneNumberLandline = model.HomeTelephoneNumberLandline;
            staffCoreData.HomeTelephoneNumberMobile = model.HomeTelephoneNumberMobile;
            //staffCoreData.OfficialMobileNumber = model.OfficialMobileNumber;
            staffCoreData.HQExtensionNumber = model.HQExtensionNumber;
            staffCoreData.DamascusExtensionNumber = model.DamascusExtensionNumber;
            staffCoreData.CallSign = model.CallSign;
            staffCoreData.OfficeRoomNumberBuilding = model.OfficeRoomNumberBuilding;
            staffCoreData.OfficialExtensionNumber = model.OfficialExtensionNumber;
            staffCoreData.SATPhoneNumber = model.SATPhoneNumber;
            staffCoreData.ICTComments = model.ICTComments;
            DbORG.Update(staffCoreData, Permissions.StaffProfileICTSection.UpdateGuid, ExecutionTime, DbCMS);

            #region Contacts 

            dataPhoneDirectory myPhone = DbORG.dataPhoneDirectory.Where(x => x.UserGUID == model.UserGUID).FirstOrDefault();
            if (myPhone != null)
            {
                myPhone.UserGUID = model.UserGUID;
                myPhone.DutyStationGUID = model.DutyStationGUID;
                myPhone.DepartmentGUID = model.DepartmentGUID;
                myPhone.JobTitleGUID = model.JobTitleGUID;
                myPhone.EmailAddress = model.EmailAddress;
                myPhone.OfficialExtensionNumber = model.OfficialExtensionNumber;
                // myPhone.OfficialMobileNumber = model.OfficialMobileNumber;
                myPhone.SATPhoneNumber = model.SATPhoneNumber;
                myPhone.CallSign = model.CallSign;
                myPhone.HQExtensionNumber = model.HQExtensionNumber;
                myPhone.DutyStationExtensionNumber = model.DamascusExtensionNumber;
                DbORG.Update(myPhone, Permissions.StaffProfileICTSection.UpdateGuid, ExecutionTime, DbCMS);
            }
            #endregion



            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.StaffProfile.Create, Apps.ORG, new UrlHelper(Request.RequestContext).Action("StaffProfile/Create", "StaffProfile", new { Area = "ORG" })), Container = "StaffProfileFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.StaffProfile.Update, Apps.ORG), Container = "StaffProfileFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.StaffProfile.Delete, Apps.ORG), Container = "StaffProfileFormControls" });

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.SingleCreateMessage(DbORG.PrimaryKeyControl(staffCoreData), DbORG.RowVersionControls(Portal.SingleToList(staffCoreData)), null, "", UIButtons));

                //return Json(DbORG.SingleCreateMessage(DbORG.PrimaryKeyControl(staffCoreData), DbORG.RowVersionControls(Portal.SingleToList(staffCoreData)), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        //[Route("ORG/StaffForAdminDBPhoneDirectory/Update/{PK}")]
        //public ActionResult StaffForAdminDBPhoneDirectory(Guid PK)
        //{
        //    if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Access, Apps.WMS))
        //    {
        //        return Json(DbORG.PermissionError());
        //    }

        //    PhoneDirectoryUpdateModel model = DbORG.v_dataSyriaPhoneDirectory.Where(x => x.UserGUID == PK).Select(x => new
        //                                           PhoneDirectoryUpdateModel
        //    {
        //        PhoneDirectoryGUID = x.PhoneDirectoryGUID,
        //        UserGUID = x.UserGUID,
        //        PhoneHolderTypeGUID = x.PhoneHolderTypeGUID,
        //        FullName = x.FullName,
        //        CallSign = x.CallSign,
        //        DutyStationGUID = x.DutyStationGUID,
        //        DutyStation = x.DutyStation,
        //        DepartmentDescription = x.DepartmentDescription,
        //        DepartmentGUID = x.DepartmentGUID,
        //        JobTitleGUID = x.JobTitleGUID,
        //        JobTitleDescription = x.JobTitle,
        //        EmailAddress = x.EmailAddress,
        //        HomeTelephoneNumberLandline = x.HomeTelephoneNumberLandline,
        //        OfficialExtensionNumber = x.OfficialExtensionNumber,
        //        OfficialMobileNumber = x.OfficialMobileNumber,
        //        SATPhoneNumber = x.SATPhoneNumber,
        //        HQExtensionNumber = x.HQExtensionNumber,
        //        DutyStationExtensionNumber = x.DutyStationExtensionNumber,
        //        Active = x.Active,

        //    }).FirstOrDefault();
        //    return PartialView("~/Areas/ORG/Views/StaffProfile/_StaffPhoneDirectory.cshtml", model);
        //}

        [Route("ORG/StaffPhoneDirectory/Update/{PK}")]
        public ActionResult StaffPhoneDirectoryUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Access, Apps.WMS))
            {
                return Json(DbORG.PermissionError());
            }

            PhoneDirectoryUpdateModel model = DbORG.v_dataSyriaPhoneDirectory.Where(x => x.PhoneDirectoryGUID == PK).Select(x => new
                                                   PhoneDirectoryUpdateModel
            {
                PhoneDirectoryGUID = x.PhoneDirectoryGUID,
                UserGUID = x.UserGUID,
                PhoneHolderTypeGUID = x.PhoneHolderTypeGUID,
                FullName = x.FullName,
                CallSign = x.CallSign,
                DutyStationGUID = x.DutyStationGUID,
                DutyStation = x.DutyStation,
                DepartmentDescription = x.DepartmentDescription,
                DepartmentGUID = x.DepartmentGUID,
                JobTitleGUID = x.JobTitleGUID,
                JobTitleDescription = x.JobTitle,
                EmailAddress = x.EmailAddress,
                HomeTelephoneNumberLandline = x.HomeTelephoneNumberLandline,
                OfficialExtensionNumber = x.OfficialExtensionNumber,
                OfficialMobileNumber = x.OfficialMobileNumber,
                SATPhoneNumber = x.SATPhoneNumber,
                HQExtensionNumber = x.HQExtensionNumber,
                DutyStationExtensionNumber = x.DutyStationExtensionNumber,
                Active = x.Active,

            }).FirstOrDefault();
            return PartialView("~/Areas/ORG/Views/StaffProfile/_StaffPhoneDirectory.cshtml", model);
        }
        public ActionResult StaffPhoneDirectoryUpdate(PhoneDirectoryUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Update, Apps.WMS))
            {
                return Json(DbORG.PermissionError());
            }
            if (!ModelState.IsValid || model.PhoneHolderTypeGUID == Guid.Empty || string.IsNullOrEmpty(model.FullName))
                return PartialView("~/Areas/ORG/Views/StaffProfile/_StaffPhoneDirectory .cshtml", model);
            Guid staffTypeGUID = Guid.Parse("20aab4c0-a6f8-4f87-9316-94d1d5f5bd8f");
            DateTime ExecutionTime = DateTime.Now;
            if (model.PhoneHolderTypeGUID == staffTypeGUID)
            {
                StaffCoreData myModel = DbORG.StaffCoreData.Where(x => x.UserGUID == model.UserGUID).FirstOrDefault();
                myModel.OfficialExtensionNumber = model.OfficialExtensionNumber;
                myModel.OfficialMobileNumber = model.OfficialMobileNumber;
                myModel.SATPhoneNumber = model.SATPhoneNumber;
                myModel.HQExtensionNumber = model.HQExtensionNumber;
                myModel.DamascusExtensionNumber = model.DutyStationExtensionNumber;
                myModel.CallSign = model.CallSign;
                DbORG.Update(myModel, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);
            }

            dataPhoneDirectory myPhone = DbORG.dataPhoneDirectory.Where(x => x.PhoneDirectoryGUID == model.PhoneDirectoryGUID).FirstOrDefault();

            myPhone.PhoneHolderTypeGUID = model.PhoneHolderTypeGUID;
            myPhone.FullName = model.FullName;
            myPhone.UserGUID = model.UserGUID;
            myPhone.DutyStationGUID = model.DutyStationGUID;
            myPhone.DepartmentGUID = model.DepartmentGUID;
            myPhone.JobTitleGUID = model.JobTitleGUID;
            myPhone.EmailAddress = model.EmailAddress;
            myPhone.OfficialExtensionNumber = model.OfficialExtensionNumber;
            myPhone.OfficialMobileNumber = model.OfficialMobileNumber;
            myPhone.SATPhoneNumber = model.SATPhoneNumber;
            myPhone.CallSign = model.CallSign;
            myPhone.HQExtensionNumber = model.HQExtensionNumber;
            myPhone.DutyStationExtensionNumber = model.DutyStationExtensionNumber;


            DbORG.Update(myPhone, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.SingleUpdateMessage(DataTableNames.StaffPhoneDirectoryDataTable,
                    DbORG.PrimaryKeyControl(myPhone),
                    DbORG.RowVersionControls(Portal.SingleToList(myPhone))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyStaffBankAccount(model.PhoneDirectoryGUID);
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }



        #endregion
        

        #region Staff Feedback 


        //[Route("ORG/StaffBankAccountDataTable/{PK}")]
        public ActionResult StaffProfileFeedbackDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/ORG/Views/StaffProfile/_StaffFeedbacktDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<StaffProfileFeedbackDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<StaffProfileFeedbackDataTableModel>(DataTable.Filters);
            }

            Guid feedbackGUID = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3555");
            var Result = (from a in DbORG.dataStaffProfileFeedback.AsExpandable().Where(x => x.Active && (x.UserGUID == PK))

                          join b in DbORG.codeTablesValuesLanguages.AsExpandable().Where(x => x.Active && x.LanguageID == LAN && x.codeTablesValues.TableGUID == feedbackGUID) on a.FeedbackTypeGUID equals b.ValueGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          select new StaffProfileFeedbackDataTableModel
                          {
                              StaffProfileFeedbackGUID = a.StaffProfileFeedbackGUID,
                              UserGUID = a.UserGUID.ToString(),
                              FeedbackTypeGUID = a.FeedbackTypeGUID.ToString(),
                              FeedbackDescription = a.FeedbackDescription,
                              FeedbackType = R1.ValueDescription,
                              ResloveDescription = a.ResloveDescription,
                              LastFlowStatusGUID = a.LastFlowStatusGUID.ToString(),
                              LastFeedbackStatus = a.LastFlowStatusName,
                              CreateDate = a.CreateDate,
                              Active = a.Active,
                              dataStaffProfileFeedbackRowVersion = a.dataStaffProfileFeedbackRowVersion

                          }).Where(Predicate);


            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult StaffFeedbackCreate(Guid FK)
        {
            if (UserGUID != FK)
            {
                if (!CMS.HasAction(Permissions.StaffProfile.Restore, Apps.ORG))
                {
                    return Json(DbORG.PermissionError());
                }
            }
            return PartialView("~/Areas/ORG/Views/StaffProfile/_StaffFeedbackUpdateModal.cshtml",
                new StaffProfileFeedbackUpdateModel { UserGUID = FK, sameUser = UserGUID == FK ? 1 : 0 });
        }








        public ActionResult StaffFeedbackUpdate(Guid PK)
        {
            if (UserGUID != PK)
            {
                if (!CMS.HasAction(Permissions.StaffProfile.Restore, Apps.ORG))
                {
                    return Json(DbORG.PermissionError());
                }
            }
            var myFeedback = DbORG.dataStaffProfileFeedback.Where(x => x.StaffProfileFeedbackGUID == PK).FirstOrDefault();
            if (myFeedback.LastFlowStatusGUID == Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3972"))
            {
                return Json(DbORG.PermissionError());
            }
            StaffProfileFeedbackUpdateModel model = DbORG.dataStaffProfileFeedback.Where(x => x.StaffProfileFeedbackGUID == PK).Select(f => new StaffProfileFeedbackUpdateModel
            {

                StaffProfileFeedbackGUID = (Guid)f.StaffProfileFeedbackGUID,
                UserGUID = (Guid)f.UserGUID,
                FeedbackTypeGUID = (Guid)f.FeedbackTypeGUID,
                FeedbackDescription = f.FeedbackDescription,
                ResloveDescription = f.ResloveDescription,
                CreateDate = f.CreateDate,
                LastFlowStatusGUID = (Guid)f.LastFlowStatusGUID,
                LastFlowStatusName = f.LastFlowStatusName,
                Active = f.Active

            }).FirstOrDefault();
            return PartialView("~/Areas/ORG/Views/StaffProfile/_StaffFeedbackUpdateModal.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffFeedbackCreate(StaffProfileFeedbackUpdateModel model)
        {
            if (UserGUID != model.UserGUID && !CMS.HasAction(Permissions.StaffProfile.Create, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            if (!ModelState.IsValid) return PartialView("~/Areas/ORG/Views/StaffProfile/_StaffFeedbackUpdateModal.cshtml", model);
            dataStaffProfileFeedback myModel = new dataStaffProfileFeedback();

            DateTime ExecutionTime = DateTime.Now;
            if (model.sameUser == 1)
            {
                myModel.FeedbackTypeGUID = Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E3551");
            }
            else
            {
                myModel.FeedbackTypeGUID = (model.FeedbackTypeGUID == null || model.FeedbackTypeGUID == Guid.Empty) ? Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E3552") : model.FeedbackTypeGUID;
            }
            myModel.LastFlowStatusGUID = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3977");
            //Flow closed ="b9cd375c-a576-4aa4-8af4-ff3c1c4e3972"
            myModel.LastFlowStatusName = "Pending";
            myModel.StaffProfileFeedbackGUID = Guid.NewGuid();
            myModel.CreateDate = ExecutionTime;
            myModel.UserGUID = model.UserGUID;
            myModel.FeedbackDescription = model.FeedbackDescription;
            dataStaffProfileFeedbackFlow myFlow = new dataStaffProfileFeedbackFlow
            {

                StaffProfileFeedbackFlowGUID = Guid.NewGuid(),
                StaffProfileFeedbackGUID = myModel.StaffProfileFeedbackGUID,
                CreatedByGUID = UserGUID,
                FlowStatusGUID = myModel.LastFlowStatusGUID,
                ActionDate = ExecutionTime,
                IsLastAction = true,
                Active = true,

            };
            DbORG.CreateNoAudit(myModel);
            DbORG.CreateNoAudit(myFlow);



            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                #region Mail
                string URL = "";
                string Anchor = "";
                string Link = "";

                Guid actionGUID = Guid.Parse("1B9200B6-60B6-4CDF-8AB8-0D3DFB94821B");

                var allUserInHr = DbORG.dataAdminAccessStaff.Where(x => x.Active == true).ToList();
                //var userGUids = DbCMS.userProfiles.Where(x => userProfilesGUIDs.Contains(x.UserProfileGUID)).Select(x => x.userServiceHistory.UserGUID).ToList();

                //var allUsers = DbAHD.userPersonalDetailsLanguage.Where(x => (userGUids.Contains(x.UserGUID)

                //)
                //                                                              && x.LanguageID == LAN ).ToList();
                //var alluserAccounts = DbAHD.userServiceHistory.Where(x => userGUids.Contains(x.UserGUID)

                //).ToList();

                var myStaff = DbORG.userPersonalDetailsLanguage.Where(x => x.UserGUID == myModel.UserGUID && x.LanguageID == LAN).FirstOrDefault();
                var myStaffCore = DbORG.StaffCoreData.Where(x => x.UserGUID == myModel.UserGUID).FirstOrDefault();

                if (myModel.FeedbackTypeGUID == Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E3551"))
                {
                    string SubjectMessage = "Staff Profile Information Issue ";



                    foreach (var user in allUserInHr)
                    {
                        //var currentDanger = allDan.Where(x => x.dataStaffEligibleForDangerPay.UserGUID == user.UserGUID).FirstOrDefault();
                        URL = AppSettingsKeys.Domain + "/ORG/StaffProfile/Update/?PK=" + new Portal().GUIDToString(myModel.UserGUID);
                        Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ClickToConfirm + "</a>";
                        Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
                        //string myFirstName = user.FirstName;
                        //string mySurName = user.Surname;


                        string _message = resxEmails.StaffUserRaisFeedbackIssueEmail
                            .Replace("$FullName", user.FullName)
                            .Replace("$StaffName", myStaff.FirstName + " " + myStaff.Surname)
                            .Replace("$Reason", model.FeedbackDescription)
                            .Replace("$VerifyLink", Anchor)
                           ;
                        if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                        int isRec = 1;
                        //var myEmail = alluserAccounts.Where(x => x.UserGUID == user.UserGUID).Select(x => x.EmailAddress).FirstOrDefault();
                        Send(user.EmailAddress, SubjectMessage, _message, isRec, myStaffCore.EmailAddress, null);
                    }
                }
                else if (myModel.FeedbackTypeGUID == Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E3552"))
                {
                    string SubjectMessage = "User Feedback";



                    var myEmails = allUserInHr.Select(x => x.EmailAddress).ToList();
                    string copyEmails = string.Join(" ;", myEmails);


                    //var currentDanger = allDan.Where(x => x.dataStaffEligibleForDangerPay.UserGUID == user.UserGUID).FirstOrDefault();
                    URL = AppSettingsKeys.Domain + "/Profile/StaffProfile/";
                    Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ClickToConfirm + "</a>";
                    Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";

                    var changer = DbAHD.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active && x.UserGUID == myFlow.CreatedByGUID).FirstOrDefault();

                    string _message = resxEmails.StaffProfileConfirmationRequest
                        .Replace("$FullName", myStaff.FirstName + " " + myStaff.Surname)
                        .Replace("$Profilechanger", changer.FirstName + " " + changer.Surname)
                        .Replace("$datechange", myFlow.ActionDate.Value.ToLongDateString())
                        .Replace("$VerifyLink", Anchor)
                       ;
                    if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                    int isRec = 1;
                    //var myEmail = alluserAccounts.Where(x => x.UserGUID == model.UserGUID).Select(x => x.EmailAddress).FirstOrDefault();
                    Send(myStaffCore.EmailAddress, SubjectMessage, _message, isRec, copyEmails, null);
                }

                #endregion




                return Json(DbORG.SingleUpdateMessage(DataTableNames.StaffProfileFeedbackDataTable, DbORG.PrimaryKeyControl(myModel), DbORG.RowVersionControls(Portal.SingleToList(myModel))));
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }
        public void Send(string recipients, string subject, string body, int? isRec, string copy_recipients, string blind)
        {

            string blind_copy_recipients = blind;
            string body_format = "HTML";
            string importance = "Normal";
            string file_attachments = null;
            string _body = "<div style='font-family:Arial;'>" + body.Replace("\r\n", "<br/>") + "</div>";
            if (LAN == "AR") { _body = "<p align='right'>" + _body + "</p>"; }
            //DbAHD.SendEmailHR("maksoud@unhcr.org", "", "", subject, _body, body_format, importance, file_attachments);
            DbCMS.SendEmailHR(recipients, copy_recipients, blind_copy_recipients, subject, _body, body_format, importance, file_attachments);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffFeedbackUpdate(dataStaffProfileFeedback model)
        {
            if (UserGUID != model.UserGUID)
            {
                if (!CMS.HasAction(Permissions.StaffProfile.Restore, Apps.ORG))
                {
                    return Json(DbORG.PermissionError());
                }
            }
            if (!ModelState.IsValid) return PartialView("~/Areas/ORG/Views/StaffProfile/StaffBankUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbORG.UpdateNoAudit(model);

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.SingleUpdateMessage(DataTableNames.StaffProfileFeedbackDataTable,
                    DbORG.PrimaryKeyControl(model),
                    DbORG.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyStaffFeedback(model.StaffProfileFeedbackGUID);
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffFeedbackDelete(dataStaffProfileFeedback model)
        {
            if (UserGUID != model.UserGUID)
            {
                if (!CMS.HasAction(Permissions.StaffProfile.Restore, Apps.ORG))
                {
                    return Json(DbORG.PermissionError());
                }
            }
            List<dataStaffProfileFeedback> DeletedLanguages = DeleteStaffFeedback(new List<dataStaffProfileFeedback> { model });

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.SingleDeleteMessage(DeletedLanguages, DataTableNames.StaffProfileFeedbackDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyStaffFeedback(model.StaffProfileFeedbackGUID);
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffFeedbackRestore(dataStaffProfileFeedback model)
        {
            if (UserGUID != model.UserGUID)
            {
                if (!CMS.HasAction(Permissions.StaffProfile.Restore, Apps.ORG))
                {
                    return Json(DbORG.PermissionError());
                }
            }
            //if (ActiveStaffFeedback(model))
            //{
            //    return Json(DbORG.RecordExists());
            //}

            List<dataStaffProfileFeedback> RestoredLanguages = RestoreStaffFeedback(Portal.SingleToList(model));

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.SingleRestoreMessage(RestoredLanguages, DataTableNames.StaffProfileFeedbackDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyStaffFeedback(model.StaffProfileFeedbackGUID);
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult StaffProfileFeedbackDataTableDelete(List<dataStaffProfileFeedback> models)
        {
            if (UserGUID != models.FirstOrDefault().UserGUID)
            {
                if (!CMS.HasAction(Permissions.StaffProfile.Restore, Apps.ORG))
                {
                    return Json(DbORG.PermissionError());
                }
            }
            List<dataStaffProfileFeedback> DeletedLanguages = DeleteStaffFeedback(models);

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.StaffProfileFeedbackDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult StaffProfileFeedbackDataTableModelRestore(List<dataStaffProfileFeedback> models)
        {
            if (UserGUID != models.FirstOrDefault().UserGUID)
            {
                if (!CMS.HasAction(Permissions.StaffProfile.Restore, Apps.ORG))
                {
                    return Json(DbORG.PermissionError());
                }
            }
            List<dataStaffProfileFeedback> RestoredLanguages = RestoreStaffFeedback(models);

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.StaffProfileFeedbackDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        private List<dataStaffProfileFeedback> DeleteStaffFeedback(List<dataStaffProfileFeedback> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataStaffProfileFeedback> DeletedStaffBankAccount = new List<dataStaffProfileFeedback>();

            string query = DbORG.QueryBuilder(models, Permissions.StaffProfile.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbORG.Database.SqlQuery<dataStaffProfileFeedback>(query).ToList();

            foreach (var language in languages)
            {
                DeletedStaffBankAccount.Add(DbORG.Delete(language, ExecutionTime, Permissions.StaffProfile.DeleteGuid, DbCMS));
            }

            return DeletedStaffBankAccount;
        }

        private List<dataStaffProfileFeedback> RestoreStaffFeedback(List<dataStaffProfileFeedback> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataStaffProfileFeedback> RestoredLanguages = new List<dataStaffProfileFeedback>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbORG.QueryBuilder(models, Permissions.StaffProfile.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbORG.Database.SqlQuery<dataStaffProfileFeedback>(query).ToList();
            foreach (var language in Languages)
            {
                //if (!ActiveStaffFeedback(language))
                //{
                RestoredLanguages.Add(DbORG.Restore(language, Permissions.StaffProfile.DeleteGuid, Permissions.StaffProfile.RestoreGuid, RestoringTime, DbCMS));
                //}
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyStaffFeedback(Guid PK)
        {
            dataStaffProfileFeedback dbModel = new dataStaffProfileFeedback();

            var Language = DbORG.dataStaffProfileFeedback.Where(l => l.StaffProfileFeedbackGUID == PK).FirstOrDefault();
            var dbLanguage = DbORG.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.dataStaffProfileFeedbackRowVersion.SequenceEqual(dbModel.dataStaffProfileFeedbackRowVersion))
            {
                return Json(DbORG.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbORG, dbModel, "LanguagesContainer"));
        }

        public ActionResult StaffFeedbackFlowCreate(Guid? PK)
        {
            var check = DbORG.dataStaffProfileFeedback.Where(x => x.StaffProfileFeedbackGUID == PK).FirstOrDefault();
            var accessListGUIDs = DbORG.dataAdminAccessStaff.Where(x => x.Active).Select(x => x.UserGUID).ToList();
            if (check.UserGUID != UserGUID)
            {
                if (!CMS.HasAction(Permissions.StaffProfile.Create, Apps.ORG))
                {
                    return Json(DbORG.PermissionError());
                }
            }
            if (check.UserGUID == UserGUID)
            {
                //report issue ,pending
                //confirm staff,pending
                if (!accessListGUIDs.Contains(UserGUID) && (check.FeedbackTypeGUID == Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3551") && check.LastFlowStatusGUID == Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3977")))
                {
                    return Json(DbORG.PermissionError());
                }

                if (check.UserGUID != UserGUID && check.FeedbackTypeGUID == Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3552") && check.LastFlowStatusGUID == Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3977"))
                {
                    return Json(DbORG.PermissionError());
                }
                if (check.UserGUID != UserGUID && (check.FeedbackTypeGUID == Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3551") && check.LastFlowStatusGUID == Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e8999")))
                {
                    return Json(DbORG.PermissionError());
                }
            }
            var model = DbORG.dataStaffProfileFeedback.Where(x => x.StaffProfileFeedbackGUID == PK).Select(x => new StaffProfileFeedbackUpdateModel
            {
                StaffProfileFeedbackGUID = x.StaffProfileFeedbackGUID,
                UserGUID = (Guid)x.UserGUID,
                FeedbackDescription = x.FeedbackDescription,
                ResloveDescription = x.ResloveDescription,
                LastFlowStatusGUID = x.LastFlowStatusGUID,
                LastFlowStatusName = x.LastFlowStatusName,
                FeedbackTypeGUID = (Guid)x.FeedbackTypeGUID,
                flowStep = 2
            }).FirstOrDefault();
            model.flowStep = model.LastFlowStatusGUID == Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3977") ? (model.FeedbackTypeGUID == Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E3551") ? 4 : 1) : (model.LastFlowStatusGUID == Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e8999") ? 2 : 3);
            return PartialView("~/Areas/ORG/Views/StaffProfile/_ProfileFeedbackFlowUpdateModal.cshtml",
                  model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffFeedbackFlowCreate(StaffProfileFeedbackUpdateModel model)
        {
            if ((model.LastFlowStatusGUID == null) || (model.LastFlowStatusGUID == Guid.Empty))
            {
                return PartialView("~/Areas/ORG/Views/StaffProfile/_ProfileFeedbackFlowUpdateModal.cshtml",
         new StaffProfileFeedbackUpdateModel { StaffProfileFeedbackGUID = (Guid)model.StaffProfileFeedbackGUID });
            }
            dataStaffProfileFeedback _stafffeedback = DbORG.dataStaffProfileFeedback.Where(x => x.StaffProfileFeedbackGUID == model.StaffProfileFeedbackGUID).FirstOrDefault();

            if (!string.IsNullOrEmpty(model.ResloveDescription))
            {
                _stafffeedback.ResloveDescription = model.ResloveDescription;
            }
            _stafffeedback.LastFlowStatusGUID = model.LastFlowStatusGUID;

            //closed Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3972");
            //fixed "b9cd375c-a576-4aa4-8af4-ff3c1c4e8999"
            _stafffeedback.LastFlowStatusName = DbORG.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active && x.ValueGUID == model.LastFlowStatusGUID).FirstOrDefault().ValueDescription;
            DbORG.UpdateNoAudit(_stafffeedback);

            DateTime exectime = DateTime.Now;

            dataStaffProfileFeedbackFlow flow = DbORG.dataStaffProfileFeedbackFlow.Where(x => x.StaffProfileFeedbackGUID == model.StaffProfileFeedbackGUID && x.IsLastAction == true).FirstOrDefault();

            DbORG.UpdateNoAudit(_stafffeedback);
            flow.IsLastAction = false;
            DbORG.UpdateNoAudit(flow);

            dataStaffProfileFeedbackFlow newflow = new dataStaffProfileFeedbackFlow
            {
                StaffProfileFeedbackFlowGUID = Guid.NewGuid(),
                StaffProfileFeedbackGUID = model.StaffProfileFeedbackGUID,
                CreatedByGUID = UserGUID,
                FlowStatusGUID = model.LastFlowStatusGUID,
                ActionDate = exectime,
                IsLastAction = true,
            };

            DbORG.CreateNoAudit(newflow);
            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();

                #region Mail
                string URL = "";
                string Anchor = "";
                string Link = "";

                Guid actionGUID = Guid.Parse("1B9200B6-60B6-4CDF-8AB8-0D3DFB94821B");

                var allUserInHr = DbORG.dataAdminAccessStaff.Where(x => x.Active == true).ToList();


                var myStaff = DbORG.userPersonalDetailsLanguage.Where(x => x.UserGUID == model.UserGUID && x.LanguageID == LAN).FirstOrDefault();
                var myStaffCore = DbORG.StaffCoreData.Where(x => x.UserGUID == model.UserGUID).FirstOrDefault();

                if (_stafffeedback.FeedbackTypeGUID == Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E3551"))
                {
                    string SubjectMessage = "Staff Profile Information Issue ";

                    var myEmails = allUserInHr.Select(x => x.EmailAddress).ToList();
                    string copyEmails = string.Join(" ;", myEmails);


                    //var currentDanger = allDan.Where(x => x.dataStaffEligibleForDangerPay.UserGUID == user.UserGUID).FirstOrDefault();
                    URL = AppSettingsKeys.Domain + "/Profile/StaffProfile/";
                    Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ClickToConfirm + "</a>";
                    Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";

                    // var changer = DbAHD.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active && x.UserGUID == myFlow.CreatedByGUID).FirstOrDefault();
                    if (model.LastFlowStatusGUID == Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E8999"))
                    {
                        string _message = resxEmails.StaffProfileIssueFixed
                            .Replace("$FullName", myStaff.FirstName + " " + myStaff.Surname)
                            //.Replace("$Profilechanger", changer.FirstName + " " + changer.Surname)
                            //.Replace("$datechange", myFlow.ActionDate.Value.ToLongDateString())
                            .Replace("$VerifyLink", Anchor)
                           ;
                        if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                        int isRec = 1;
                        //var myEmail = alluserAccounts.Where(x => x.UserGUID == model.UserGUID).Select(x => x.EmailAddress).FirstOrDefault();
                        Send(myStaffCore.EmailAddress, SubjectMessage, _message, isRec, copyEmails, null);
                    }
                    else
                    {
                        foreach (var user in allUserInHr)
                        {
                            //var currentDanger = allDan.Where(x => x.dataStaffEligibleForDangerPay.UserGUID == user.UserGUID).FirstOrDefault();
                            URL = AppSettingsKeys.Domain + "/ORG/StaffProfile/Update/?PK=" + new Portal().GUIDToString(model.UserGUID);
                            Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ClickToConfirm + "</a>";
                            Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
                            //string myFirstName = user.FirstName;
                            //string mySurName = user.Surname;


                            string _message = resxEmails.StaffProfileIssueFixedProfileInformationEmail
                                .Replace("$FullName", user.FullName)
                                .Replace("$StaffName", myStaff.FirstName + " " + myStaff.Surname)
                               //.Replace("$Reason", model.FeedbackDescription)
                               //.Replace("$VerifyLink", Anchor)
                               ;
                            if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                            int isRec = 1;
                            //var myEmail = alluserAccounts.Where(x => x.UserGUID == user.UserGUID).Select(x => x.EmailAddress).FirstOrDefault();
                            Send(user.EmailAddress, SubjectMessage, _message, isRec, myStaffCore.EmailAddress, null);
                        }


                    }





                }
                //confirm by staff 
                else if (_stafffeedback.FeedbackTypeGUID == Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E3552"))
                {
                    string SubjectMessage = "User Feedback";



                    var myEmails = allUserInHr.Select(x => x.EmailAddress).ToList();
                    string copyEmails = string.Join(" ;", myEmails);


                    foreach (var user in allUserInHr)
                    {
                        //var currentDanger = allDan.Where(x => x.dataStaffEligibleForDangerPay.UserGUID == user.UserGUID).FirstOrDefault();
                        URL = AppSettingsKeys.Domain + "/ORG/StaffProfile/Update/?PK=" + new Portal().GUIDToString(model.UserGUID);
                        Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ClickToConfirm + "</a>";
                        Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
                        //string myFirstName = user.FirstName;
                        //string mySurName = user.Surname;


                        string _message = resxEmails.StaffProfileConfirmProfileInformationEmail
                            .Replace("$FullName", user.FullName)
                            .Replace("$StaffName", myStaff.FirstName + " " + myStaff.Surname)
                           //.Replace("$Reason", model.FeedbackDescription)
                           //.Replace("$VerifyLink", Anchor)
                           ;
                        if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                        int isRec = 1;
                        //var myEmail = alluserAccounts.Where(x => x.UserGUID == user.UserGUID).Select(x => x.EmailAddress).FirstOrDefault();
                        Send(user.EmailAddress, SubjectMessage, _message, isRec, myStaffCore.EmailAddress, null);
                    }


                }

                #endregion


                string callBackFunc = "CheckCreateNewRelease('0')";
                //return Json(DbORG.SuccessMessage("Saved successfully"));
                dataStaffProfileFeedback myX = DbORG.dataStaffProfileFeedback.Where(x => x.StaffProfileFeedbackGUID == model.StaffProfileFeedbackGUID).FirstOrDefault();
                myX.FeedbackTypeGUID = Guid.NewGuid();

                return Json(DbWMS.SingleUpdateMessage(DataTableNames.StaffProfileFeedbackDataTable, DbORG.PrimaryKeyControl(myX), DbORG.RowVersionControls(Portal.SingleToList(myX))));
                //return Json(DbCMS.SingleUpdateMessage(DataTableNames.StaffProfileFeedbackDataTable, DbORG.PrimaryKeyControl(model), DbORG.RowVersionControls(Portal.SingleToList(model)), ""));
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }


        public ActionResult ConfirmProfileStaffInofrmation(Guid userGUID)
        {
            if (userGUID != UserGUID)
            {
                return Json(DbORG.PermissionError());
            }
            var staffcore = DbORG.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            staffcore.LastConfirmationStatus = "Staff Confirmed";
            staffcore.LastConfirmationStatusGUID = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3742");
            DbORG.UpdateNoAudit(staffcore);




            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.StaffProfile.Create, Apps.ORG, new UrlHelper(Request.RequestContext).Action("StaffProfile/Create", "StaffProfile", new { Area = "ORG" })), Container = "StaffProfileFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.StaffProfile.Update, Apps.ORG), Container = "StaffProfileFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.StaffProfile.Delete, Apps.ORG), Container = "StaffProfileFormControls" });

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();


                string SubjectMessage = "User Profile Confirmation";


                #region email
                Guid hrAccessGUID = Guid.Parse("788F6BCD-44CE-41B5-BAED-F971E7816B93");
                Guid maksoud = Guid.Parse("8F7EF83F-FD3E-4F8C-8735-8A22D3D61B75");
                Guid amer = Guid.Parse("0F0189C0-76D9-4D42-BAF6-8DB5113BB71A");
                Guid Jawad = Guid.Parse("66BEA969-8D70-4AAB-8A84-AEF4C8D13AE0");
                Guid haitham = Guid.Parse("F83879A9-2113-4606-8B01-2B28FC82D536");
                Guid ihsan = Guid.Parse("1497DEE6-1B00-4054-AB4A-F50BB7CB275C");

                Guid unopsLica = Guid.Parse("1506F90E-F377-49B6-A8CD-116250246A44");

                var tempPerm = DbORG.userPermissions.Where(x => (x.ActionGUID == hrAccessGUID || x.ActionGUID == StaffContractManagerPermssion.UNHCR ||
                x.ActionGUID == StaffContractManagerPermssion.UNOPS) && x.Active

                ).ToList();


                if (staffcore.ContractTypeGUID == unopsLica)
                {
                    tempPerm = tempPerm.Where(x => x.ActionGUID == Guid.Parse("B49A0E2F-7819-4558-B165-473E4FC19630")).ToList();

                }
                else
                {
                    tempPerm = tempPerm.Where(x => x.ActionGUID == Guid.Parse("9001BA39-6996-4042-A651-36F68912C5A7")).ToList();
                }

                var userperm = tempPerm.Select(x => x.UserProfileGUID).Distinct().ToList();






                var userProfilesGuids = DbORG.userProfiles.Where(x => userperm.Contains(x.UserProfileGUID)).Select(x => x.ServiceHistoryGUID).Distinct().ToList();
                var users = DbORG.userServiceHistory.Where(x => userProfilesGuids.Contains(x.ServiceHistoryGUID) && (x.UserGUID != maksoud) && x.UserGUID != amer
                && x.UserGUID != Jawad
                && x.UserGUID != haitham
                && x.UserGUID != ihsan).Distinct().ToList();
                var myStaff = DbORG.userPersonalDetailsLanguage.Where(x => x.UserGUID == UserGUID && x.LanguageID == LAN).FirstOrDefault();

                string _message = resxEmails.StaffConfimedProfileInformation
                        .Replace("$FullName", myStaff.FirstName + " " + myStaff.Surname)
                       //.Replace("$StaffName", myStaff.FirstName + " " + myStaff.Surname)
                       //.Replace("$Reason", model.FeedbackDescription)
                       //.Replace("$VerifyLink", Anchor)
                       ;

                var myEmails = users.Select(x => x.EmailAddress).ToList();
                string copyEmails = string.Join(" ;", myEmails);
                if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }

                Send(staffcore.EmailAddress, SubjectMessage, _message, 1, copyEmails, null);
                #endregion
                return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
                //  return Json(DbORG.SingleCreateMessage(DbORG.PrimaryKeyControl(staffcore), DbORG.RowVersionControls(Portal.SingleToList(staffcore)), null, "", UIButtons));

                //return Json(DbORG.SingleCreateMessage(DbORG.PrimaryKeyControl(staffCoreData), DbORG.RowVersionControls(Portal.SingleToList(staffCoreData)), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }
        public ActionResult NotifyStaffToConfirmProfile(Guid userGUID)
        {
            var staffcore = DbORG.StaffCoreData.Where(x => x.UserGUID == userGUID).FirstOrDefault();
            staffcore.LastConfirmationStatus = "Pending Confirmation";
            staffcore.LastConfirmationStatusGUID = Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E3747");
            DbORG.UpdateNoAudit(staffcore);
            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.StaffProfile.Create, Apps.ORG, new UrlHelper(Request.RequestContext).Action("StaffProfile/Create", "StaffProfile", new { Area = "ORG" })), Container = "StaffProfileFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.StaffProfile.Update, Apps.ORG), Container = "StaffProfileFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.StaffProfile.Delete, Apps.ORG), Container = "StaffProfileFormControls" });
            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                string SubjectMessage = "User Profile Confirmation";
                #region email
                Guid hrAccessGUID = Guid.Parse("788F6BCD-44CE-41B5-BAED-F971E7816B93");
                Guid maksoud = Guid.Parse("8F7EF83F-FD3E-4F8C-8735-8A22D3D61B75");
                Guid amer = Guid.Parse("0F0189C0-76D9-4D42-BAF6-8DB5113BB71A");
                Guid Jawad = Guid.Parse("66BEA969-8D70-4AAB-8A84-AEF4C8D13AE0");
                Guid haitham = Guid.Parse("F83879A9-2113-4606-8B01-2B28FC82D536");
                Guid ihsan = Guid.Parse("1497DEE6-1B00-4054-AB4A-F50BB7CB275C");

                Guid unopsLica = Guid.Parse("1506F90E-F377-49B6-A8CD-116250246A44");

                var tempPerm = DbORG.userPermissions.Where(x => (x.ActionGUID == hrAccessGUID || x.ActionGUID == StaffContractManagerPermssion.UNHCR ||
                x.ActionGUID == StaffContractManagerPermssion.UNOPS) && x.Active

                ).ToList();


                if (staffcore.ContractTypeGUID == unopsLica)
                {
                    tempPerm = tempPerm.Where(x => x.ActionGUID == Guid.Parse("B49A0E2F-7819-4558-B165-473E4FC19630")).ToList();

                }
                else
                {
                    tempPerm = tempPerm.Where(x => x.ActionGUID == Guid.Parse("9001BA39-6996-4042-A651-36F68912C5A7")).ToList();
                }

                var userperm = tempPerm.Select(x => x.UserProfileGUID).Distinct().ToList();
                var userProfilesGuids = DbORG.userProfiles.Where(x => userperm.Contains(x.UserProfileGUID)).Select(x => x.ServiceHistoryGUID).Distinct().ToList();
                var users = DbORG.userServiceHistory.Where(x => userProfilesGuids.Contains(x.ServiceHistoryGUID) && (x.UserGUID != maksoud) && x.UserGUID != amer
                && x.UserGUID != Jawad
                && x.UserGUID != haitham
                && x.UserGUID != ihsan).Distinct().ToList();
                var myStaff = DbORG.userPersonalDetailsLanguage.Where(x => x.UserGUID == userGUID && x.LanguageID == LAN).FirstOrDefault();
                string URL = "";
                string Anchor = "";
                string Link = "";
                URL = AppSettingsKeys.Domain + "/Profile/StaffProfile/";
                Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ClickToConfirm + "</a>";
                string _message = resxEmails.HRToStaffToConfirmProfileInformation
                        .Replace("$FullName", myStaff.FirstName + " " + myStaff.Surname)
                       //.Replace("$StaffName", myStaff.FirstName + " " + myStaff.Surname)
                       //.Replace("$Reason", model.FeedbackDescription)
                       .Replace("$Link", Anchor)
                       ;

                var myEmails = users.Select(x => x.EmailAddress).ToList();
                string copyEmails = string.Join(" ;", myEmails);
                if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }

                Send(staffcore.EmailAddress, SubjectMessage, _message, 1, copyEmails, null);






                #endregion
                return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
                //  return Json(DbORG.SingleCreateMessage(DbORG.PrimaryKeyControl(staffcore), DbORG.RowVersionControls(Portal.SingleToList(staffcore)), null, "", UIButtons));

                //return Json(DbORG.SingleCreateMessage(DbORG.PrimaryKeyControl(staffCoreData), DbORG.RowVersionControls(Portal.SingleToList(staffCoreData)), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }




        }



        #endregion Banks

        #region Staff Others 
        public JsonResult StaffCustodyWarehosueItemsDataTable(DataTableRecievedOptions options, Guid PK)
        {
            var e = Request;

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<STAFFWarehouseModelEntryMovementDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<STAFFWarehouseModelEntryMovementDataTableModel>(DataTable.Filters);
            }





            var All = (from a in DbWMS.v_EntryMovementDataTable.Where(x => x.LastCustdianNameGUID == PK).AsExpandable().Where(x =>
                //warehouseAuthGUIDs.Contains((Guid)x.WarehouseOwnerGUID) && 
                (x.IsDeterminanted == true || x.IsDeterminanted == null))
                       select new STAFFWarehouseModelEntryMovementDataTableModel
                       {


                           //ItemInputDetailGUID = a.ItemInputDetailGUID,
                           Active = (bool)a.Active,

                           //ItemModelWarehouseGUID = a.ItemModelWarehouseGUID.ToString(),
                           ModelDescription = a.ModelDescription,
                           ItemDescription = a.WarehouseItemDescription,
                           //WarehouseItemClassificationGUID = a.WarehouseItemClassificationGUID.ToString(),
                           //WarehouseItemGUID = a.WarehouseItemGuid.ToString(),
                           //BrandGUID = a.BrandGUID.ToString(),
                           //WarehouseOwnerGUID = a.WarehouseOwnerGUID.ToString(),
                           BarcodeNumber = a.BarcodeNumber,
                           SerialNumber = a.SerialNumber,

                           IME1 = a.IMEI,
                           GSM = a.GSM,
                           MAC = a.MAC,

                           MSRPID = a.MSRPID,






                           Comments = a.Comments,
                           DeliveryStatus = a.LastFlow,
                           //DeliveryStatusGUID = a.LastFlowTypeGUID.ToString(),

                           dataItemInputDetailRowVersion = a.dataItemInputDetailRowVersion
                       }).Where(Predicate);



            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<STAFFWarehouseModelEntryMovementDataTableModel> Result = Mapper.Map<List<STAFFWarehouseModelEntryMovementDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }


        public JsonResult StaffGrantedSoftwareApplicationsDataTable(DataTableRecievedOptions options, Guid PK)
        {
            var e = Request;

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<STAFFGrantedSoftwareApplicationDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<STAFFGrantedSoftwareApplicationDataTableModel>(DataTable.Filters);
            }
            //var serviceHistory = DbCMS.userServiceHistory.Where(x => x.UserGUID == PK).FirstOrDefault();
            //var _userProfile = DbCMS.userProfiles.Where(x => x.ServiceHistoryGUID == serviceHistory.ServiceHistoryGUID).FirstOrDefault();

            //var userPerm = DbCMS.userPermissions.Where(x => x.UserProfileGUID == _userProfile.UserProfileGUID && x.Active==true).Select(x => x.ActionGUID).Distinct().ToList();
            //var userAppGUIDs = DbCMS.codeActions.Where(x => userPerm.Contains(x.ActionGUID) && x.Active==true).Select(x => x.codeActionsCategories.ApplicationGUID).Distinct().ToList();
            //  var _app = DbCMS.v_currentUserPermissions.Where(x => x.UserGUID == UserGUID).Select(x => x.ApplicationGUID).Distinct().ToList();
            var _app = DbCMS.v_currentUserPermissions.Where(x => x.UserGUID == PK).Select(x => x.ApplicationGUID).Distinct().ToList();

            var All = (from a in DbCMS.codeApplicationsLanguages.Where(x => _app.Contains(x.ApplicationGUID) && x.Active && x.LanguageID == LAN).AsExpandable()
                       select new STAFFGrantedSoftwareApplicationDataTableModel
                       {
                           ApplicationLanguageGUID = a.ApplicationLanguageGUID.ToString(),
                           ApplicationGUID = a.ApplicationGUID.ToString(),


                           //ItemInputDetailGUID = a.ItemInputDetailGUID,
                           Active = (bool)a.Active,

                           //ItemModelWarehouseGUID = a.ItemModelWarehouseGUID.ToString(),
                           ApplicationName = a.ApplicationDescription,

                           codeApplicationsLanguagesRowVersion = a.codeApplicationsLanguagesRowVersion
                       }).Where(Predicate);



            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<STAFFGrantedSoftwareApplicationDataTableModel> Result = Mapper.Map<List<STAFFGrantedSoftwareApplicationDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }



        #endregion


        #region Security Section
        [HttpPost]
        public ActionResult StaffProfileSecuritySectionUpdate(StaffCoreData model)
        {

            if (!CMS.HasAction(Permissions.StaffProfileSecuritySection.Update, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            if (!ModelState.IsValid || model.UserGUID == null) return View("~/Areas/ORG/Views/StaffSecurityInformation/SecurityStaffProfile.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;
            var staffPers = DbORG.userPersonalDetails.Where(x => x.UserGUID == model.UserGUID).FirstOrDefault();
            staffPers.BloodGroup = model.BloodGroup;
            var staffCore = DbORG.StaffCoreData.Where(x => x.UserGUID == model.UserGUID).FirstOrDefault();
            staffCore.BloodGroup = model.BloodGroup;
            staffCore.UNHCRIDNumber = model.UNHCRIDNumber;
            staffCore.PermanentAddressEn = model.PermanentAddressEn;
            staffCore.PermanentAddressAr = model.PermanentAddressAr;
            staffCore.CurrentAddressEn = model.CurrentAddressEn;
            staffCore.KinMobileNumber = model.KinMobileNumber;
            staffCore.NextOfKinName = model.NextOfKinName;
            staffCore.BSAFECertAcquired = model.BSAFECertAcquired;
            staffCore.BSAFEExpiryDate = model.BSAFEExpiryDate;
            staffCore.NumberOfDependants = model.NumberOfDependants;
            staffCore.DependantsName = model.DependantsName;
            //staffCore.HomeTelephoneNumberMobile = model.HomeTelephoneNumberMobile;
            //staffCore.HomeTelephoneNumberLandline = model.HomeTelephoneNumberLandline;

            staffCore.CallSign = model.CallSign;
            //staffCore.LastConfirmationStatus = "Confirmed BY Security";

            DbORG.Update(staffPers, Permissions.StaffProfileSecuritySection.UpdateGuid, ExecutionTime, DbCMS);
            DbORG.Update(staffCore, Permissions.StaffProfileSecuritySection.UpdateGuid, ExecutionTime, DbCMS);

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.StaffProfile.Create, Apps.ORG, new UrlHelper(Request.RequestContext).Action("StaffProfile/Create", "StaffProfile", new { Area = "ORG" })), Container = "StaffProfileFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.StaffProfile.Update, Apps.ORG), Container = "StaffProfileFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.StaffProfile.Delete, Apps.ORG), Container = "StaffProfileFormControls" });
            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.SingleCreateMessage(DbORG.PrimaryKeyControl(staffCore), DbORG.RowVersionControls(Portal.SingleToList(staffCore)), null, "", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyStaffBankAccount(model.UserGUID);
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }



        }
        #region Online Training


        //[Route("ORG/StaffBankAccountDataTable/{PK}")]
        public ActionResult StaffOnlineTrainingDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/ORG/Views/StaffSecurityInformation/_OnlineTrainingDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<StaffOnlineTrainingDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<StaffOnlineTrainingDataTableModel>(DataTable.Filters);
            }

            Guid trainingTypes = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c4e7479");
            var Result = (from a in DbORG.dataStaffOnlineTraining.AsExpandable().Where(x => x.Active && (x.UserGUID == PK))

                          join b in DbORG.codeTablesValuesLanguages.AsExpandable().Where(x => x.Active && x.LanguageID == LAN && x.codeTablesValues.TableGUID == trainingTypes) on a.OnlineTrainingTypeGUID equals b.ValueGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          select new StaffOnlineTrainingDataTableModel
                          {
                              StaffOnlineTrainingGUID = a.StaffOnlineTrainingGUID,
                              OnlineTrainingTypeGUID = a.OnlineTrainingTypeGUID.ToString(),
                              UserGUID = a.UserGUID.ToString(),
                              StartDate = a.StartDate,
                              ExpiryDate = a.ExpiryDate,
                              CreatedByGUID = a.CreatedByGUID.ToString(),
                              CreateDate = a.CreateDate,
                              TrainingStatusGUID = a.TrainingStatusGUID.ToString(),
                              StatusName = a.StatusName,
                              TrainingName = R1.ValueDescription,
                              Comments = a.Comments,

                              Active = a.Active,
                              dataStaffOnlineTrainingRowVersion = a.dataStaffOnlineTrainingRowVersion

                          }).Where(Predicate);


            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult StaffOnlineTrainingCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.StaffProfileSecuritySection.Update, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            return PartialView("~/Areas/ORG/Views/StaffSecurityInformation/_OnlineTrainingUpdateModal.cshtml",
                new StaffOnlineTrainingUpdateModel { UserGUID = FK });
        }



        public ActionResult StaffOnlineTrainingUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.StaffProfileSecuritySection.Access, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            StaffOnlineTrainingUpdateModel model = DbORG.dataStaffOnlineTraining.Where(x => x.StaffOnlineTrainingGUID == PK).Select(f => new StaffOnlineTrainingUpdateModel
            {

                StaffOnlineTrainingGUID = (Guid)f.StaffOnlineTrainingGUID,
                OnlineTrainingTypeGUID = f.OnlineTrainingTypeGUID,
                UserGUID = f.UserGUID,
                StartDate = f.StartDate,
                ExpiryDate = f.ExpiryDate,
                CreatedByGUID = (Guid)f.CreatedByGUID,
                CreateDate = f.CreateDate,
                TrainingStatusGUID = f.TrainingStatusGUID,
                StatusName = f.StatusName,
                Comments = f.Comments,

                Active = f.Active

            }).FirstOrDefault();
            return PartialView("~/Areas/ORG/Views/StaffSecurityInformation/_OnlineTrainingUpdateModal.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffOnlineTrainingCreate(StaffOnlineTrainingUpdateModel mymodel)
        {
            if (!CMS.HasAction(Permissions.StaffProfileSecuritySection.Access, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            if (!ModelState.IsValid || (mymodel.OnlineTrainingTypeGUID == null || (mymodel.StartDate >= mymodel.ExpiryDate) || mymodel.StartDate == null || mymodel.ExpiryDate == null)) return PartialView("~/Areas/ORG/Views/StaffSecurityInformation/_OnlineTrainingUpdateModal.cshtml", mymodel);

            DateTime ExecutionTime = DateTime.Now;
            dataStaffOnlineTraining model = Mapper.Map(mymodel, new dataStaffOnlineTraining());
            model.CreatedByGUID = UserGUID;
            model.CreateDate = ExecutionTime;
            model.Comments = mymodel.Comments;


            DbORG.Create(model, Permissions.StaffProfileSecuritySection.AccessGuid, ExecutionTime, DbCMS);

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.SingleUpdateMessage(DataTableNames.StaffOnlineTrainingDataTable, DbORG.PrimaryKeyControl(model), DbORG.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffOnlineTrainingUpdate(StaffOnlineTrainingUpdateModel mymodel)
        {
            if (!CMS.HasAction(Permissions.StaffProfileSecuritySection.Access, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            if (!ModelState.IsValid || (mymodel.OnlineTrainingTypeGUID == null || (mymodel.StartDate >= mymodel.ExpiryDate) || mymodel.StartDate == null || mymodel.ExpiryDate == null)) return PartialView("~/Areas/ORG/Views/StaffSecurityInformation/_OnlineTrainingUpdateModal.cshtml", mymodel);
            var model = DbORG.dataStaffOnlineTraining.Where(x => x.StaffOnlineTrainingGUID == mymodel.StaffOnlineTrainingGUID).FirstOrDefault();

            DateTime ExecutionTime = DateTime.Now;
            model.OnlineTrainingTypeGUID = mymodel.OnlineTrainingTypeGUID;
            model.StartDate = mymodel.StartDate;
            model.ExpiryDate = mymodel.ExpiryDate;
            model.Comments = mymodel.Comments;
            DbORG.Update(model, Permissions.StaffProfileSecuritySection.AccessGuid, ExecutionTime, DbCMS);

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.SingleUpdateMessage(DataTableNames.StaffOnlineTrainingDataTable,
                    DbORG.PrimaryKeyControl(model),
                    DbORG.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyStaffOnlineTraining(model.StaffOnlineTrainingGUID);
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffOnlineTrainingDelete(dataStaffOnlineTraining model)
        {
            if (!CMS.HasAction(Permissions.StaffProfileSecuritySection.Update, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            List<dataStaffOnlineTraining> DeletedLanguages = DeleteStaffOnlineTraining(new List<dataStaffOnlineTraining> { model });

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.SingleDeleteMessage(DeletedLanguages, DataTableNames.StaffOnlineTrainingDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyStaffOnlineTraining(model.StaffOnlineTrainingGUID);
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffOnlineTrainingRestore(dataStaffOnlineTraining model)
        {
            if (!CMS.HasAction(Permissions.StaffProfileSecuritySection.Update, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            if (ActiveStaffOnlineTraining(model))
            {
                return Json(DbORG.RecordExists());
            }

            List<dataStaffOnlineTraining> RestoredLanguages = RestoreStaffOnlineTraining(Portal.SingleToList(model));

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.SingleRestoreMessage(RestoredLanguages, DataTableNames.StaffOnlineTrainingDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyStaffOnlineTraining(model.StaffOnlineTrainingGUID);
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult StaffOnlineTrainingDataTableDelete(List<dataStaffOnlineTraining> models)
        {
            if (!CMS.HasAction(Permissions.StaffProfileSecuritySection.Update, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            List<dataStaffOnlineTraining> DeletedLanguages = DeleteStaffOnlineTraining(models);

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.StaffOnlineTrainingDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult StaffOnlineTrainingDataTableModelRestore(List<dataStaffOnlineTraining> models)
        {
            if (!CMS.HasAction(Permissions.StaffProfileSecuritySection.Update, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            List<dataStaffOnlineTraining> RestoredLanguages = RestoreStaffOnlineTraining(models);

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.StaffOnlineTrainingDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        private List<dataStaffOnlineTraining> DeleteStaffOnlineTraining(List<dataStaffOnlineTraining> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataStaffOnlineTraining> DeletedStaffBankAccount = new List<dataStaffOnlineTraining>();

            string query = DbORG.QueryBuilder(models, Permissions.StaffProfileSecuritySection.UpdateGuid, SubmitTypes.Delete, "");

            var languages = DbORG.Database.SqlQuery<dataStaffOnlineTraining>(query).ToList();

            foreach (var language in languages)
            {
                DeletedStaffBankAccount.Add(DbORG.Delete(language, ExecutionTime, Permissions.StaffProfileSecuritySection.UpdateGuid, DbCMS));
            }

            return DeletedStaffBankAccount;
        }

        private List<dataStaffOnlineTraining> RestoreStaffOnlineTraining(List<dataStaffOnlineTraining> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataStaffOnlineTraining> RestoredLanguages = new List<dataStaffOnlineTraining>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbORG.QueryBuilder(models, Permissions.StaffProfileSecuritySection.UpdateGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbORG.Database.SqlQuery<dataStaffOnlineTraining>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveStaffOnlineTraining(language))
                {
                    RestoredLanguages.Add(DbORG.Restore(language, Permissions.StaffProfileSecuritySection.UpdateGuid, Permissions.StaffProfileSecuritySection.UpdateGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyStaffOnlineTraining(Guid PK)
        {
            dataStaffOnlineTraining dbModel = new dataStaffOnlineTraining();

            var Language = DbORG.dataStaffOnlineTraining.Where(l => l.StaffOnlineTrainingGUID == PK).FirstOrDefault();
            var dbLanguage = DbORG.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.dataStaffOnlineTrainingRowVersion.SequenceEqual(dbModel.dataStaffOnlineTrainingRowVersion))
            {
                return Json(DbORG.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbORG, dbModel, "LanguagesContainer"));
        }

        private bool ActiveStaffOnlineTraining(dataStaffOnlineTraining model)
        {
            int LanguageID = DbORG.dataStaffOnlineTraining
                                  .Where(x =>
                                              x.UserGUID == model.UserGUID &&
                                              x.OnlineTrainingTypeGUID == model.OnlineTrainingTypeGUID &&
                                              x.TrainingStatusGUID == model.TrainingStatusGUID &&

                                              x.Active).Count();
            if (LanguageID > 0)
            {
                //Fix
                ModelState.AddModelError("LanguageID", "Already Exist");
            }

            return (LanguageID > 0);
        }

        #endregion Online Training 

        #region Staff Relatives


        //[Route("ORG/StaffBankAccountDataTable/{PK}")]
        public ActionResult StaffRelativeDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/ORG/Views/StaffProfile/_StaffRelativeDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<StaffRelativeDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<StaffRelativeDataTableModel>(DataTable.Filters);
            }

            Guid trainingTypes = Guid.Parse("946a4dda-1745-46f9-8e56-4db1374c46e4");
            var Result = (from a in DbORG.dataStaffRelative.AsExpandable().Where(x => x.Active && (x.UserGUID == PK))

                          join b in DbORG.codeTablesValuesLanguages.AsExpandable().Where(x => x.Active && x.LanguageID == LAN && x.codeTablesValues.TableGUID == trainingTypes) on a.RelativeTypeGUID equals b.ValueGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          select new StaffRelativeDataTableModel
                          {
                              StaffRelativeGUID = a.StaffRelativeGUID,
                              RelativeTypeGUID = a.RelativeTypeGUID.ToString(),
                              UserGUID = a.UserGUID.ToString(),
                              RelativeName = a.RelativeName,
                              Phone = a.Phone,
                              CreatedByGUID = a.CreatedByGUID.ToString(),
                              CreateDate = a.CreateDate,

                              RelativeType = R1.ValueDescription,
                              Comments = a.Comments,

                              Active = a.Active,
                              dataStaffRelativeRowVersion = a.dataStaffRelativeRowVersion

                          }).Where(Predicate);


            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult StaffRelativeCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.StaffProfileSecuritySection.Update, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            return PartialView("~/Areas/ORG/Views/StaffSecurityInformation/_StaffRelativeUpdateModal.cshtml",
                new StaffRelativeUpdateModel { UserGUID = FK });
        }



        public ActionResult StaffRelativeUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.StaffProfileSecuritySection.Access, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            StaffRelativeUpdateModel model = DbORG.dataStaffRelative.Where(x => x.StaffRelativeGUID == PK).Select(f => new StaffRelativeUpdateModel
            {

                StaffRelativeGUID = (Guid)f.StaffRelativeGUID,
                RelativeTypeGUID = f.RelativeTypeGUID,
                UserGUID = f.UserGUID,
                RelativeName = f.RelativeName,
                Phone = f.Phone,
                CreatedByGUID = (Guid)f.CreatedByGUID,
                CreateDate = f.CreateDate,

                Comments = f.Comments,

                Active = f.Active

            }).FirstOrDefault();
            return PartialView("~/Areas/ORG/Views/StaffSecurityInformation/_StaffRelativeUpdateModal.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffRelativeCreate(StaffRelativeUpdateModel mymodel)
        {
            if (!CMS.HasAction(Permissions.StaffProfileSecuritySection.Access, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            if (!ModelState.IsValid || (mymodel.RelativeTypeGUID == null || (mymodel.RelativeName == null))) return PartialView("~/Areas/ORG/Views/StaffSecurityInformation/_StaffRelativeUpdateModal.cshtml", mymodel);

            DateTime ExecutionTime = DateTime.Now;
            dataStaffRelative model = Mapper.Map(mymodel, new dataStaffRelative());
            model.CreatedByGUID = UserGUID;
            model.CreateDate = ExecutionTime;
            model.Comments = mymodel.Comments;


            DbORG.Create(model, Permissions.StaffProfileSecuritySection.AccessGuid, ExecutionTime, DbCMS);

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.SingleUpdateMessage(DataTableNames.StaffRelativeDataTable, DbORG.PrimaryKeyControl(model), DbORG.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffRelativeUpdate(StaffRelativeUpdateModel mymodel)
        {
            if (!CMS.HasAction(Permissions.StaffProfileSecuritySection.Access, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            if (!ModelState.IsValid || (mymodel.RelativeTypeGUID == null || (mymodel.RelativeName == null))) return PartialView("~/Areas/ORG/Views/StaffSecurityInformation/_StaffRelativeUpdateModal.cshtml", mymodel);
            var model = DbORG.dataStaffRelative.Where(x => x.StaffRelativeGUID == mymodel.StaffRelativeGUID).FirstOrDefault();

            DateTime ExecutionTime = DateTime.Now;
            model.RelativeTypeGUID = mymodel.RelativeTypeGUID;
            model.RelativeName = mymodel.RelativeName;
            model.Phone = mymodel.Phone;
            model.Comments = mymodel.Comments;
            DbORG.Update(model, Permissions.StaffProfileSecuritySection.AccessGuid, ExecutionTime, DbCMS);

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.SingleUpdateMessage(DataTableNames.StaffRelativeDataTable,
                    DbORG.PrimaryKeyControl(model),
                    DbORG.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyStaffRelative(model.StaffRelativeGUID);
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffRelativeDelete(dataStaffRelative model)
        {
            if (!CMS.HasAction(Permissions.StaffProfileSecuritySection.Update, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            List<dataStaffRelative> DeletedLanguages = DeleteStaffRelative(new List<dataStaffRelative> { model });

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.SingleDeleteMessage(DeletedLanguages, DataTableNames.StaffRelativeDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyStaffRelative(model.StaffRelativeGUID);
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffRelativeRestore(dataStaffRelative model)
        {
            if (!CMS.HasAction(Permissions.StaffProfileSecuritySection.Update, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            if (ActiveStaffRelative(model))
            {
                return Json(DbORG.RecordExists());
            }

            List<dataStaffRelative> RestoredLanguages = RestoreStaffRelative(Portal.SingleToList(model));

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.SingleRestoreMessage(RestoredLanguages, DataTableNames.StaffRelativeDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyStaffRelative(model.StaffRelativeGUID);
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult StaffRelativeDataTableDelete(List<dataStaffRelative> models)
        {
            if (!CMS.HasAction(Permissions.StaffProfileSecuritySection.Update, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            List<dataStaffRelative> DeletedLanguages = DeleteStaffRelative(models);

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.StaffRelativeDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult StaffRelativeDataTableModelRestore(List<dataStaffRelative> models)
        {
            if (!CMS.HasAction(Permissions.StaffProfileSecuritySection.Update, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            List<dataStaffRelative> RestoredLanguages = RestoreStaffRelative(models);

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.StaffRelativeDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        private List<dataStaffRelative> DeleteStaffRelative(List<dataStaffRelative> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataStaffRelative> DeletedStaffBankAccount = new List<dataStaffRelative>();

            string query = DbORG.QueryBuilder(models, Permissions.StaffProfileSecuritySection.UpdateGuid, SubmitTypes.Delete, "");

            var languages = DbORG.Database.SqlQuery<dataStaffRelative>(query).ToList();

            foreach (var language in languages)
            {
                DeletedStaffBankAccount.Add(DbORG.Delete(language, ExecutionTime, Permissions.StaffProfileSecuritySection.UpdateGuid, DbCMS));
            }

            return DeletedStaffBankAccount;
        }

        private List<dataStaffRelative> RestoreStaffRelative(List<dataStaffRelative> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataStaffRelative> RestoredLanguages = new List<dataStaffRelative>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbORG.QueryBuilder(models, Permissions.StaffProfileSecuritySection.UpdateGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbORG.Database.SqlQuery<dataStaffRelative>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveStaffRelative(language))
                {
                    RestoredLanguages.Add(DbORG.Restore(language, Permissions.StaffProfileSecuritySection.UpdateGuid, Permissions.StaffProfileSecuritySection.UpdateGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyStaffRelative(Guid PK)
        {
            dataStaffRelative dbModel = new dataStaffRelative();

            var Language = DbORG.dataStaffRelative.Where(l => l.StaffRelativeGUID == PK).FirstOrDefault();
            var dbLanguage = DbORG.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.dataStaffRelativeRowVersion.SequenceEqual(dbModel.dataStaffRelativeRowVersion))
            {
                return Json(DbORG.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbORG, dbModel, "LanguagesContainer"));
        }

        private bool ActiveStaffRelative(dataStaffRelative model)
        {
            int LanguageID = DbORG.dataStaffRelative
                                  .Where(x =>
                                              x.UserGUID == model.UserGUID &&
                                              x.RelativeTypeGUID == model.RelativeTypeGUID &&
                                              x.RelativeName == model.RelativeName &&

                                              x.Active).Count();
            if (LanguageID > 0)
            {
                //Fix
                ModelState.AddModelError("LanguageID", "Already Exist");
            }

            return (LanguageID > 0);
        }

        #endregion Staff Relatives

        #endregion


        #region Staff Service Provided


        //[Route("ORG/StaffBankAccountDataTable/{PK}")]
        public ActionResult StaffServiceProvidedDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/ORG/Views/StaffServiceProvided/_StaffServiceProvidedDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<StaffServiceProvidedDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<StaffServiceProvidedDataTableModel>(DataTable.Filters);
            }

            Guid serviceprovided = Guid.Parse("55962180-3634-44a4-874c-db7c3481d66a");
            var Result = (from a in DbORG.dataStaffServiceProvided.AsExpandable().Where(x => x.Active && (x.UserGUID == PK))

                          join b in DbORG.codeTablesValuesLanguages.AsExpandable().Where(x => x.Active && x.LanguageID == LAN && x.codeTablesValues.TableGUID == serviceprovided) on a.ServiceTypeGUID equals b.ValueGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()

                          join c in DbORG.userPersonalDetailsLanguage.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on a.CreateByGUID equals c.UserGUID into LJ2
                          from R2 in LJ2.DefaultIfEmpty()

                          join d in DbORG.userPersonalDetailsLanguage.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on a.UpdateByGUID equals d.UserGUID into LJ3
                          from R3 in LJ3.DefaultIfEmpty()

                          join e in DbORG.userPersonalDetailsLanguage.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on a.ActivatedByGUID equals e.UserGUID into LJ4
                          from R4 in LJ4.DefaultIfEmpty()

                          join f in DbORG.userPersonalDetailsLanguage.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on a.CaneledByGUID equals f.UserGUID into LJ5
                          from R5 in LJ5.DefaultIfEmpty()

                          select new StaffServiceProvidedDataTableModel
                          {
                              StaffServiceProvidedGUID = a.StaffServiceProvidedGUID,
                              ServiceTypeGUID = a.ServiceTypeGUID.ToString(),
                              UserGUID = a.UserGUID.ToString(),
                              ServiceName = a.ServiceName,
                              LastFlowStatusGUID = a.LastFlowStatusGUID.ToString(),
                              LastFlowStatusName = a.LastFlowStatusName,
                              StartDate = a.StartDate,
                              ExpiryDate = a.ExpiryDate,
                              ServiceStatus = a.LastFlowStatusName,
                              CreatedByGUID = a.CreateByGUID.ToString(),
                              CreateDate = a.CreateDate,
                              ActivatedBy = R4.FirstName + " " + R4.Surname,
                              CancelledBy = R5.FirstName + " " + R5.Surname,

                              CreateBy = R2.FirstName + " " + R2.Surname,
                              UpdateBy = R3.FirstName + " " + R3.Surname,
                              Comments = a.Comments,
                              CancelledDate = a.CaneledDate,
                              ActivatedDate = a.ActivatedDate,

                              Active = a.Active,
                              dataStaffServiceProvidedRowVersion = a.dataStaffServiceProvidedRowVersion

                          }).Where(Predicate);


            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult StaffServiceProvidedCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.StaffProfileICTSection.Update, Apps.ORG))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/ORG/Views/StaffServiceProvided/_StaffServiceProvidedUpdateModal.cshtml",
                new StaffServiceProvidedUpdateModel { UserGUID = FK });
        }



        public ActionResult StaffServiceProvidedUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.StaffProfileICTSection.Access, Apps.ORG))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            StaffServiceProvidedUpdateModel model = DbORG.dataStaffServiceProvided.Where(x => x.StaffServiceProvidedGUID == PK).Select(f => new StaffServiceProvidedUpdateModel
            {

                StaffServiceProvidedGUID = (Guid)f.StaffServiceProvidedGUID,
                ServiceTypeGUID = f.ServiceTypeGUID,
                ServiceName = f.ServiceName,

                UserGUID = f.UserGUID,
                LastFlowStatusGUID = f.LastFlowStatusGUID,
                LastFlowStatusName = f.LastFlowStatusName,
                StartDate = f.StartDate,
                //ExpiryDate = f.ExpiryDate,
                //CreateDate = f.CreateDate,
                CreateByGUID = f.CreateByGUID,
                UpdateByGUID = f.UpdateByGUID,
                UpdateDate = f.UpdateDate,


                Comments = f.Comments,

                Active = f.Active

            }).FirstOrDefault();
            return PartialView("~/Areas/ORG/Views/StaffServiceProvided/_StaffServiceProvidedUpdateModal.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffServiceProvidedCreate(StaffServiceProvidedUpdateModel mymodel)
        {
            if (!CMS.HasAction(Permissions.StaffProfileICTSection.Access, Apps.ORG))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || (mymodel.ServiceTypeGUID == null || (mymodel.LastFlowStatusGUID == null) || (mymodel.StartDate > mymodel.ExpiryDate))) return PartialView("~/Areas/ORG/Views/StaffServiceProvided/_StaffServiceProvidedUpdateModal.cshtml", mymodel);
            var myStaff = DbORG.StaffCoreData.Where(x => x.UserGUID == mymodel.UserGUID).FirstOrDefault();
            DateTime ExecutionTime = DateTime.Now;
            dataStaffServiceProvided model = Mapper.Map(mymodel, new dataStaffServiceProvided());
            model.CreateByGUID = UserGUID;
            model.CreateDate = ExecutionTime;
            model.StartDate = myStaff.StaffEOD;
            model.ExpiryDate = myStaff.ContractEndDate;
            model.Comments = mymodel.Comments;
            model.LastFlowStatusName = DbAHD.codeTablesValuesLanguages.Where(x => x.ValueGUID == mymodel.LastFlowStatusGUID && x.LanguageID == LAN && x.Active).FirstOrDefault().ValueDescription;
            model.ServiceName = DbAHD.codeTablesValuesLanguages.Where(x => x.ValueGUID == mymodel.ServiceTypeGUID && x.LanguageID == LAN && x.Active).FirstOrDefault().ValueDescription;

            DbORG.Create(model, Permissions.StaffProfileICTSection.AccessGuid, ExecutionTime, DbCMS);

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.SingleUpdateMessage(DataTableNames.StaffServiceProvidedDataTable, DbORG.PrimaryKeyControl(model), DbORG.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffServiceProvidedUpdate(StaffServiceProvidedUpdateModel mymodel)
        {
            if (!CMS.HasAction(Permissions.StaffProfileICTSection.Update, Apps.ORG))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || (mymodel.ServiceTypeGUID == null || (mymodel.LastFlowStatusGUID == null) || (mymodel.StartDate > mymodel.ExpiryDate))) return PartialView("~/Areas/ORG/Views/StaffServiceProvided/_StaffServiceProvidedUpdateModal.cshtml", mymodel);
            var model = DbORG.dataStaffServiceProvided.Where(x => x.StaffServiceProvidedGUID == mymodel.StaffServiceProvidedGUID).FirstOrDefault();

            DateTime ExecutionTime = DateTime.Now;
            model.LastFlowStatusGUID = mymodel.LastFlowStatusGUID;
            //model.StartDate = mymodel.StartDate;
            //model.ExpiryDate = mymodel.ExpiryDate;
            model.UpdateByGUID = UserGUID;
            model.UpdateDate = ExecutionTime;
            if (mymodel.LastFlowStatusGUID == Guid.Parse("55962180-3634-44A4-874C-DB7C3481D882"))
            {
                model.ActivatedByGUID = UserGUID;
                model.ActivatedDate = ExecutionTime;
            }
            if (mymodel.LastFlowStatusGUID == Guid.Parse("55962180-3634-44A4-874C-DB7C3481D855"))
            {
                model.CaneledByGUID = UserGUID;
                model.CaneledDate = ExecutionTime;
            }

            model.Comments = mymodel.Comments;
            model.LastFlowStatusName = DbAHD.codeTablesValuesLanguages.Where(x => x.ValueGUID == mymodel.LastFlowStatusGUID && x.LanguageID == LAN && x.Active).FirstOrDefault().ValueDescription;
            model.ServiceName = DbAHD.codeTablesValuesLanguages.Where(x => x.ValueGUID == mymodel.ServiceTypeGUID && x.LanguageID == LAN && x.Active).FirstOrDefault().ValueDescription;
            model.ServiceTypeGUID = mymodel.ServiceTypeGUID;
            DbORG.Update(model, Permissions.StaffProfileICTSection.AccessGuid, ExecutionTime, DbCMS);

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.SingleUpdateMessage(DataTableNames.StaffServiceProvidedDataTable,
                    DbORG.PrimaryKeyControl(model),
                    DbORG.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyStaffServiceProvided(model.StaffServiceProvidedGUID);
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffServiceProvidedDelete(dataStaffServiceProvided model)
        {
            if (!CMS.HasAction(Permissions.StaffProfileICTSection.Update, Apps.ORG))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataStaffServiceProvided> DeletedLanguages = DeleteStaffServiceProvided(new List<dataStaffServiceProvided> { model });

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.SingleDeleteMessage(DeletedLanguages, DataTableNames.StaffServiceProvidedDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyStaffServiceProvided(model.StaffServiceProvidedGUID);
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffServiceProvidedRestore(dataStaffServiceProvided model)
        {
            if (!CMS.HasAction(Permissions.StaffProfileICTSection.Update, Apps.ORG))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveStaffServiceProvided(model))
            {
                return Json(DbORG.RecordExists());
            }

            List<dataStaffServiceProvided> RestoredLanguages = RestoreStaffServiceProvided(Portal.SingleToList(model));

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.SingleRestoreMessage(RestoredLanguages, DataTableNames.StaffServiceProvidedDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyStaffServiceProvided(model.StaffServiceProvidedGUID);
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult StaffServiceProvidedDataTableDelete(List<dataStaffServiceProvided> models)
        {
            if (!CMS.HasAction(Permissions.StaffProfileICTSection.Update, Apps.ORG))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataStaffServiceProvided> DeletedLanguages = DeleteStaffServiceProvided(models);

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.StaffServiceProvidedDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult StaffServiceProvidedDataTableModelRestore(List<dataStaffServiceProvided> models)
        {
            if (!CMS.HasAction(Permissions.StaffProfileICTSection.Update, Apps.ORG))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataStaffServiceProvided> RestoredLanguages = RestoreStaffServiceProvided(models);

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.StaffServiceProvidedDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        private List<dataStaffServiceProvided> DeleteStaffServiceProvided(List<dataStaffServiceProvided> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataStaffServiceProvided> DeletedStaffBankAccount = new List<dataStaffServiceProvided>();

            string query = DbORG.QueryBuilder(models, Permissions.StaffProfileICTSection.UpdateGuid, SubmitTypes.Delete, "");

            var languages = DbORG.Database.SqlQuery<dataStaffServiceProvided>(query).ToList();

            foreach (var language in languages)
            {
                DeletedStaffBankAccount.Add(DbORG.Delete(language, ExecutionTime, Permissions.StaffProfileICTSection.UpdateGuid, DbCMS));
            }

            return DeletedStaffBankAccount;
        }

        private List<dataStaffServiceProvided> RestoreStaffServiceProvided(List<dataStaffServiceProvided> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataStaffServiceProvided> RestoredLanguages = new List<dataStaffServiceProvided>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbORG.QueryBuilder(models, Permissions.StaffProfileICTSection.UpdateGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbORG.Database.SqlQuery<dataStaffServiceProvided>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveStaffServiceProvided(language))
                {
                    RestoredLanguages.Add(DbORG.Restore(language, Permissions.StaffProfileICTSection.UpdateGuid, Permissions.StaffProfileICTSection.UpdateGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyStaffServiceProvided(Guid PK)
        {
            dataStaffServiceProvided dbModel = new dataStaffServiceProvided();

            var Language = DbORG.dataStaffServiceProvided.Where(l => l.StaffServiceProvidedGUID == PK).FirstOrDefault();
            var dbLanguage = DbORG.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.dataStaffServiceProvidedRowVersion.SequenceEqual(dbModel.dataStaffServiceProvidedRowVersion))
            {
                return Json(DbORG.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbORG, dbModel, "LanguagesContainer"));
        }

        private bool ActiveStaffServiceProvided(dataStaffServiceProvided model)
        {
            int LanguageID = DbORG.dataStaffServiceProvided
                                  .Where(x =>
                                              x.UserGUID == model.UserGUID &&
                                              x.ServiceTypeGUID == model.ServiceTypeGUID &&
                                              x.ServiceName == model.ServiceName &&

                                              x.Active).Count();
            if (LanguageID > 0)
            {
                //Fix
                ModelState.AddModelError("LanguageID", "Already Exist");
            }

            return (LanguageID > 0);
        }

        #endregion Staff Relatives


        #region Upload Images

        public ActionResult UploadUserPhoto(Guid id)
        {
            UserPhotoUpdateModel model = new UserPhotoUpdateModel();
            model.UserGUID = id;
            return PartialView("~/Areas/ORG/Views/StaffProfile/_PhotoUpdateModal.cshtml", model);
        }
        [HttpPost]
        public FineUploaderResult UploadStaffImage(FineUpload upload, Guid userGUID)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Access, Apps.ORG))
            {
                return new FineUploaderResult(false, new { path = UploadImage(upload, userGUID), success = true });
            }

            return new FineUploaderResult(true, new { path = UploadImage(upload, userGUID), success = true });
        }

        public string UploadImage(FineUpload upload, Guid userGUID)
        {
            if (UserGUID != Guid.Empty)
            {
                var _stearm = upload.InputStream;
                DateTime ExecutionTime = DateTime.Now;

                string _ext = Path.GetExtension(upload.FileName).Split('.')[1];

                string FolderPath = Server.MapPath("~/Uploads/ORG/StaffPhotos/" + userGUID.ToString());
                Directory.CreateDirectory(FolderPath);


                string FilePath = FolderPath + "/" + userGUID.ToString() + "." + _ext;

                using (var fileStream = System.IO.File.Create(FilePath))
                {
                    byte[] m_Bytes = ReadFully(upload.InputStream);
                    var staffCore = DbORG.StaffCoreData.Where(x => x.UserGUID == userGUID).FirstOrDefault();
                    staffCore.Photo = m_Bytes;
                    DbORG.Update(staffCore, Permissions.StaffProfile.UpdateGuid, ExecutionTime, DbCMS);
                    upload.InputStream.Seek(0, SeekOrigin.Begin);
                    upload.InputStream.CopyTo(fileStream);
                }


                try
                {
                    DbORG.SaveChanges();
                }
                catch (Exception)
                {

                    throw;
                }
                return "/Uploads/ORG/StaffPhotos/" + UserGUID + "/" + UserGUID + ".jpg";
            }
            return "/Uploads/ORG/StaffPhotos/" + UserGUID + "/" + UserGUID + ".jpg";
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        #endregion



    }
}