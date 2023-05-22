using AppsPortal.BaseControllers;
using AppsPortal.Data;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.Models;
using AppsPortal.ViewModels;
using AutoMapper;
using LinqKit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Configuration;
using System.Web.Mvc;

namespace AppsPortal.Controllers
{

    public class ProfileController : PortalBaseController
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult StaffProfile()
        {
            StaffCoreData staffCoreData = (from a in DbCMS.StaffCoreData.AsNoTracking().Where(x => x.UserGUID == UserGUID && x.Active)
                                           select a).FirstOrDefault();
            StaffProfileReadlOnlyModel model = new StaffProfileReadlOnlyModel();
            userPersonalDetails userPersonalDetails = DbCMS.userPersonalDetails.AsNoTracking().Where(x => x.UserGUID == UserGUID && x.Active).FirstOrDefault();
            // (from a in DbORG.userPersonalDetails.AsNoTracking().Where(x => x.UserGUID == PK && x.Active) select a).FirstOrDefault();
            List<userPersonalDetailsLanguage> userPersonalDetailsLanguages = userPersonalDetails.userPersonalDetailsLanguage.ToList();
            model.UserGUID = staffCoreData.UserGUID;
            model.LastConfirmationStatus = staffCoreData.LastConfirmationStatus;
            model.LastConfirmationStatusGUID = staffCoreData.LastConfirmationStatusGUID;

            model.StaffStatus = staffCoreData.StaffStatusGUID == Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E3611") ? "Active" : "Not Active";
            model.JobTitleGUID = staffCoreData.JobTitleGUID;
            model.DutyStationGUID = staffCoreData.DutyStationGUID;
            if (staffCoreData.DepartmentGUID != Guid.Empty && staffCoreData.DepartmentGUID != null)
                model.DepartmentGUID = (Guid)staffCoreData.DepartmentGUID;
            model.ReportToGUID = staffCoreData.ReportToGUID;
            model.GenderGUID = userPersonalDetails.GenderGUID;
            //model.GenderGUID = staffCoreData.;
            model.NationalityGUID = staffCoreData.NationalityGUID;
            model.Nationality1Arabic = model.NationalityGUID != null ? (DbAHD.codeCountriesLanguages.Where(x => x.CountryGUID == model.NationalityGUID && x.LanguageID == "AR").FirstOrDefault() != null ? DbAHD.codeCountriesLanguages.Where(x => x.CountryGUID == model.NationalityGUID && x.LanguageID == "AR").FirstOrDefault().CountryDescription : "") : "";
            model.Nationality2Arabic = model.Nationality2GUID != null ? (DbAHD.codeCountriesLanguages.Where(x => x.CountryGUID == model.Nationality2GUID && x.LanguageID == "AR").FirstOrDefault() != null ? DbAHD.codeCountriesLanguages.Where(x => x.CountryGUID == model.Nationality2GUID && x.LanguageID == "AR").FirstOrDefault().CountryDescription : "") : "";
            model.Nationality3Arabic = model.Nationality3GUID != null ? (DbAHD.codeCountriesLanguages.Where(x => x.CountryGUID == model.Nationality3GUID && x.LanguageID == "AR").FirstOrDefault() != null ? DbAHD.codeCountriesLanguages.Where(x => x.CountryGUID == model.Nationality3GUID && x.LanguageID == "AR").FirstOrDefault().CountryDescription : "") : "";
            //model.AssignmentType = staffCoreData.AssignmentType;
            model.PositionInOrganigram = staffCoreData.PositionInOrganigram;
            model.SyrianNationalIDNumber = staffCoreData.SyrianNationalIDNumber;
            model.DateOfBirth = staffCoreData.DateOfBirth;
            model.PlaceOfBirthGUID = staffCoreData.PlaceOfBirthGUID;
            var alllocations = DbCMS.codeCountriesLanguages.ToList();
            string en = "EN";

            if (staffCoreData.PlaceOfBirthGUID != null)
            {
                model.PlaceOfBirth = alllocations.Where(x => x.LanguageID == en && x.CountryGUID == staffCoreData.PlaceOfBirthGUID).FirstOrDefault().CountryDescription;
            }

            if (staffCoreData.NationalityGUID != null)
            {
                model.Nationality1 = alllocations.Where(x => x.LanguageID == en && x.CountryGUID == staffCoreData.NationalityGUID).FirstOrDefault().Nationality;
            }

            if (staffCoreData.Nationality2GUID != null)
            {
                model.Nationality2 = alllocations.Where(x => x.LanguageID == en && x.CountryGUID == staffCoreData.Nationality2GUID).FirstOrDefault().Nationality;
            }
            if (staffCoreData.Nationality3GUID != null)
            {
                model.Nationality3 = alllocations.Where(x => x.LanguageID == en && x.CountryGUID == staffCoreData.Nationality3GUID).FirstOrDefault().Nationality;
            }
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
            
            model.ExpiryOfResidencyVisa = staffCoreData.ExpiryOfResidencyVisa;
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
            model.StaffPrefix = model.StaffPrefixGUID == Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E3772") ? "Ms." : "Mr.";
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
            if (staffCoreData.RecruitmentTypeGUID != null)
            {
                model.RecruitmentType = DbCMS.codeTablesValuesLanguages.Where(x => x.ValueGUID == staffCoreData.RecruitmentTypeGUID && x.LanguageID == LAN).FirstOrDefault().ValueDescription;
            }

            if (staffCoreData.JobTitleGUID != null)
            {


                model.JobTitleMoFAEN = (DbAHD.codeJobTitlesLanguages.Where(x => x.JobTitleGUID == model.JobTitleGUID && x.LanguageID == "EN").FirstOrDefault()) != null ? DbAHD.codeJobTitlesLanguages.Where(x => x.JobTitleGUID == model.JobTitleGUID && x.LanguageID == "EN").FirstOrDefault().JobTitleDescription : "";
            }
            if (staffCoreData.ContractTypeGUID != null)
            {
                model.ContractType = DbCMS.codeTablesValuesLanguages.Where(x => x.ValueGUID == staffCoreData.ContractTypeGUID && x.LanguageID == LAN && x.codeTablesValues.TableGUID == LookupTables.StaffContractTypes).FirstOrDefault().ValueDescription;
            }
            if (staffCoreData.DutyStationGUID != null)
            {
                model.DutyStation = DbCMS.codeDutyStationsLanguages.Where(x => x.LanguageID == LAN && x.DutyStationGUID == staffCoreData.DutyStationGUID).FirstOrDefault().DutyStationDescription;
            }
            if (staffCoreData.DepartmentGUID != null)
            {
                model.Department = DbCMS.codeDepartmentsLanguages.Where(x => x.LanguageID == LAN && x.DepartmentGUID == staffCoreData.DepartmentGUID).FirstOrDefault().DepartmentDescription;
            }
            if (staffCoreData.StaffGradeGUID != null)
            {
                model.Grade = DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.ValueGUID == staffCoreData.StaffGradeGUID).FirstOrDefault().ValueDescription;
            }
            if (staffCoreData.ReportToGUID != null)
            {
                model.ReportTo = DbCMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.UserGUID == staffCoreData.ReportToGUID).FirstOrDefault().FirstName + " " + DbCMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.UserGUID == staffCoreData.ReportToGUID).FirstOrDefault().Surname;
            }


            model.NextOfKinName = staffCoreData.NextOfKinName;
            model.KinMobileNumber = staffCoreData.KinMobileNumber;
            model.BSAFECertAcquired = staffCoreData.BSAFECertAcquired;
            model.BSAFEExpiryDate = staffCoreData.BSAFEExpiryDate;
            model.NumberOfDependants = staffCoreData.NumberOfDependants;
            model.DependantsName = staffCoreData.DependantsName;
            model.NextOfKinName = staffCoreData.NextOfKinName;


            return View("~/Views/Profile/StaffProfile.cshtml", null, model);
        }

        #region Personal Details

        public ActionResult PersonalDetailsView()
        {
            var model = (from a in DbCMS.userPersonalDetails.Where(x => x.UserGUID == UserGUID)
                         join b in DbCMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN) on a.UserGUID equals b.UserGUID into R1
                         join c in DbCMS.codeCountriesLanguages.Where(x => x.LanguageID == LAN) on a.CountryGUID equals c.CountryGUID into R2
                         join d in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN) on a.GenderGUID equals d.ValueGUID into R3
                         join e in DbCMS.codeLanguages on a.PreferedLanguageID equals e.LanguageID into R4

                         from personalDetails in R1.DefaultIfEmpty()
                         from country in R2.DefaultIfEmpty()
                         from gender in R3.DefaultIfEmpty()
                         from lang in R4.DefaultIfEmpty()

                         select new PersonalDetailsModel
                         {
                             Nationality = country.Nationality,
                             Gender = gender.ValueDescription,
                             DateOfBirth = a.DateOfBirth,
                             PreferedLanguage = lang.LanguageNameLocal,
                             BloodGroup = a.BloodGroup,
                             FirstName = personalDetails.FirstName,
                             Surname = personalDetails.Surname,
                             Active = a.Active,
                         }).FirstOrDefault();
            model.MediaPath = new Portal().ProfilePhoto();

            return PartialView("~/Views/Profile/PersonalDetails/_DetailsView.cshtml", model);
        }

        public ActionResult PersonalDetailsNamesDataTable(DataTableRecievedOptions options)
        {
            if (options.columns == null)//Inject the datatable JavaScript
            {
                return PartialView("_PersonalDetailsNamesDataTable");
            }
            else //It is loading or filter or sorting
            {
                DataTableOptions DataTable = ConvertOptions.Fill(options);

                Expression<Func<userPersonalDetailsLanguage, bool>> LanguagePredicate;
                LanguagePredicate = b => true;
                if (DataTable.Filters.FilterRules != null)
                {
                    LanguagePredicate = SearchHelper.CreateSearchPredicate<userPersonalDetailsLanguage>(DataTable.Filters);
                }

                var Result = DbCMS.userPersonalDetailsLanguage.AsExpandable().Where(LanguagePredicate).Where(x => x.UserGUID == UserGUID && x.LanguageID != LAN)
                    .Select(x => new
                    {
                        x.PersonalDetailsLanguageGUID,
                        x.LanguageID,
                        x.FirstName,
                        x.FatherName,
                        x.GrandFatherName,
                        x.Surname,
                        x.userPersonalDetailsLanguageRowVersion
                    });

                Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

                return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult PersonalDetailsUpdate()
        {
            var User = DbCMS.userPersonalDetails.Find(UserGUID);
            var Names = DbCMS.userPersonalDetailsLanguage.Where(x => x.UserGUID == UserGUID && x.Active && x.LanguageID == LAN).Select(u => new { u.FirstName, u.FatherName, u.GrandFatherName, u.Surname, u.userPersonalDetailsLanguageRowVersion }).FirstOrDefault();
            var NamesList = DbCMS.userPersonalDetailsLanguage.Where(x => x.UserGUID == UserGUID && x.Active && x.LanguageID == LAN).ToList();

            PersonalDetailsUpdateModel model = new PersonalDetailsUpdateModel();

            Mapper.Map(User, model);
            Mapper.Map(Names, model);

            model.MediaPath = new Portal().ProfilePhoto();
            return PartialView("~/Views/Profile/PersonalDetails/_DetailsUpdate.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PersonalDetailsUpdate(PersonalDetailsUpdateModel model)
        {
            if (!ModelState.IsValid || !ValidatePersonalInfo(model)) return PartialView("~/Views/Profile/PersonalDetails/_DetailsUpdate", model);

            DateTime ExecutionTime = DateTime.Now;

            model.UserGUID = UserGUID;

            userPersonalDetails userPersonalDetails = new userPersonalDetails();

            Mapper.Map(model, userPersonalDetails);

            DbCMS.Update(userPersonalDetails, Permissions.PersonalDetails.UpdateGuid, ExecutionTime);

            var UserDetailsLanguage = DbCMS.userPersonalDetailsLanguage.Where(u => u.UserGUID == UserGUID && u.LanguageID == LAN).FirstOrDefault();
            if (UserDetailsLanguage != null)
            {
                Mapper.Map(model, UserDetailsLanguage);
                DbCMS.Update(UserDetailsLanguage, Permissions.PersonalDetailsLanguage.UpdateGuid, ExecutionTime);
            }
            else
            {
                UserDetailsLanguage = Mapper.Map<userPersonalDetailsLanguage>(model);
                UserDetailsLanguage.PersonalDetailsLanguageGUID = Guid.NewGuid();
                UserDetailsLanguage.LanguageID = LAN;
                UserDetailsLanguage.UserGUID = UserGUID;
                DbCMS.Create(UserDetailsLanguage, Permissions.PersonalDetailsLanguage.CreateGuid, ExecutionTime);
            }
            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(null, null, null, "LoadUrl('PanelForPersonalDetailsForm','" + Url.Action("PersonalDetailsView", "Profile") + "')", "Personal Details Updated Successfully"));
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                return Json(JsonMessages.ErrorMessage(DbCMS, s));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PersonalDetailsProfilePhotoDelete()
        {
            //This function will delete the photo only.
            System.IO.File.Delete(WebConfigurationManager.AppSettings["DataFolder"] + "\\Users\\ProfilePhotos\\" + UserGUID.ToString() + ".jpg");
            System.IO.File.Delete(WebConfigurationManager.AppSettings["DataFolder"] + "\\Users\\ProfilePhotos\\LG_" + UserGUID.ToString() + ".jpg");
            System.IO.File.Delete(WebConfigurationManager.AppSettings["DataFolder"] + "\\Users\\ProfilePhotos\\XS_" + UserGUID.ToString() + ".jpg");

            return Json(DbCMS.SingleUpdateMessage(null, null, null, "RemoveProfilePhoto('" + Constants.UserProfilePhotoTemplate + "')", "Photo deleted successfully"));
        }

        private bool ValidatePersonalInfo(PersonalDetailsUpdateModel model)
        {
            bool valid = true;
            if (model.DateOfBirth >= DateTime.Now)
            {
                ModelState.AddModelError("DateOfBirth", "Date of Birth cannot be in future.");
                valid = false;
            }

            return valid;
        }

        #endregion

        #region Photograph
        public ActionResult Photo()
        {
            return PartialView("~/Views/Profile/Photo/_Photo.cshtml");
        }

        #endregion

        #region Personal Details Names
        public ActionResult PersonalDetailsNamesUpdate(string PK)
        {
            if (!string.IsNullOrEmpty(PK))
            {
                Guid _PK = Guid.Parse(PK);
                userPersonalDetailsLanguage model = Mapper.Map<userPersonalDetailsLanguage>(DbCMS.userPersonalDetailsLanguage.Find(_PK));
                return PartialView("~/Views/Profile/PersonalDetails/_NamesUpdateModal.cshtml", model);
            }
            else
            {
                return PartialView("~/Views/Profile/PersonalDetails/_NamesUpdateModal.cshtml", new userPersonalDetailsLanguage());
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PersonalDetailsNamesCreate(userPersonalDetailsLanguage model)
        {
            if (!ModelState.IsValid || ActiveName(model)) return PartialView("~/Views/Profile/PersonalDetails/_NamesUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            model.UserGUID = UserGUID;
            DbCMS.Create(model, Permissions.PersonalDetailsLanguage.CreateGuid, ExecutionTime);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.PersonalDetailsNamesDataTable,
                   DbCMS.PrimaryKeyControl(model),
                   DbCMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PersonalDetailsNamesUpdate(userPersonalDetailsLanguage model)
        {
            if (!ModelState.IsValid || ActiveName(model)) return PartialView("~/Views/Profile/PersonalDetails/_NamesUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbCMS.Update(model, Permissions.PersonalDetailsLanguage.UpdateGuid, ExecutionTime);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.PersonalDetailsNamesDataTable,
                    DbCMS.PrimaryKeyControl(model),
                    DbCMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyPersonalDetailsNames(model.PersonalDetailsLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PersonalDetailsNamesDelete(userPersonalDetailsLanguage model)
        {
            List<userPersonalDetailsLanguage> DeletedNames = DeleteNames(new List<userPersonalDetailsLanguage> { model });
            try
            {
                int CommitedRows = DbCMS.SaveChanges();
                return Json(DbCMS.SingleDeleteMessage(DeletedNames, DataTableNames.PersonalDetailsNamesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyPersonalDetailsNames(model.PersonalDetailsLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PersonalDetailsNamesRestore(userPersonalDetailsLanguage model)
        {
            List<userPersonalDetailsLanguage> RestoredNames = RestoreNames(Portal.SingleToList(model));

            try
            {
                int CommitedRows = DbCMS.SaveChanges();
                return Json(DbCMS.SingleRestoreMessage(CommitedRows, RestoredNames, DbCMS.PrimaryKeyControl(RestoredNames.FirstOrDefault()), Url.Action(DataTableNames.PersonalDetailsNamesDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", null));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyPersonalDetailsNames(model.PersonalDetailsLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PersonalDetailsNamesDataTableDelete(List<userPersonalDetailsLanguage> models)
        {

            List<userPersonalDetailsLanguage> DeletedNames = DeleteNames(models);
            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialDeleteMessage(DeletedNames, models, DataTableNames.PersonalDetailsNamesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult PersonalDetailsNamesDataTableResotre(List<userPersonalDetailsLanguage> models)
        {
            List<userPersonalDetailsLanguage> RestoredLanguages = RestoreNames(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.PersonalDetailsNamesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        private List<userPersonalDetailsLanguage> DeleteNames(List<userPersonalDetailsLanguage> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<userPersonalDetailsLanguage> DeletedPersonalNames = new List<userPersonalDetailsLanguage>();

            string query = DbCMS.QueryBuilder(models, Permissions.PersonalDetailsLanguage.DeleteGuid, SubmitTypes.Delete, "");

            var names = DbCMS.Database.SqlQuery<userPersonalDetailsLanguage>(query).ToList();

            foreach (var name in names)
            {
                DeletedPersonalNames.Add(DbCMS.Delete(name, ExecutionTime, Permissions.PersonalDetailsLanguage.DeleteGuid));
            }

            return DeletedPersonalNames;
        }

        private List<userPersonalDetailsLanguage> RestoreNames(List<userPersonalDetailsLanguage> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<userPersonalDetailsLanguage> RestoredLanguages = new List<userPersonalDetailsLanguage>();
            string query = DbCMS.QueryBuilder(models, Permissions.PersonalDetailsLanguage.DeleteGuid, SubmitTypes.Restore, "");

            var Languages = DbCMS.Database.SqlQuery<userPersonalDetailsLanguage>(query).ToList();

            foreach (var language in Languages)
            {
                if (!ActiveName(language))
                {
                    RestoredLanguages.Add(DbCMS.Restore(language, Permissions.PersonalDetailsLanguage.DeleteGuid, Permissions.PersonalDetailsLanguage.RestoreGuid, RestoringTime));
                }
            }

            return RestoredLanguages;
        }

        private bool ActiveName(userPersonalDetailsLanguage model)
        {
            //Language is the constrain
            int Avilable = DbCMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == model.LanguageID && x.Active && x.UserGUID == model.UserGUID && x.UserGUID != model.UserGUID).Count();
            if (Avilable > 0)
            {
                ModelState.AddModelError("", "Name in selected language already exists."); //From resource ?????? Amer  
            }
            return (Avilable > 0);
        }

        private JsonResult ConcrrencyPersonalDetailsNames(Guid PK)
        {
            userPersonalDetailsLanguage Language = DbCMS.userPersonalDetailsLanguage.Where(x => x.PersonalDetailsLanguageGUID == PK).FirstOrDefault();
            var dbEntity = DbCMS.Entry(Language).GetDatabaseValues().ToObject();
            userPersonalDetailsLanguage dbLanguage = new userPersonalDetailsLanguage();
            dbLanguage = Mapper.Map(dbEntity, dbLanguage);
            if (Language.userPersonalDetailsLanguageRowVersion.SequenceEqual(dbLanguage.userPersonalDetailsLanguageRowVersion))
            {
                return Json(DbCMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbCMS, dbLanguage, "LanguagesContainer"));
        }
        #endregion

        #region Contact Details Section
        public ActionResult ContactDetailsView()
        {
            var contactDetails = (from a in DbCMS.userContactDetails
                                  where a.UserGUID == UserGUID
                                  select a).FirstOrDefault();

            return PartialView("~/Views/Profile/ContactDetails/_DetailsView.cshtml", contactDetails);
        }

        public ActionResult ContactDetailsUpdate()
        {
            var Result = DropDownList.CountriesPhoneCode();
            ViewBag.JsonCountries = JsonConvert.SerializeObject(Result);
            ViewBag.ddlCountriesPhoneCode = Result.Select(ddl => new { Value = ddl.id, Text = ddl.text }).ToList();

            var model = DbCMS.userContactDetails.Where(x => x.UserGUID == UserGUID).FirstOrDefault();

            return PartialView("~/Views/Profile/ContactDetails/_DetailsUpdate.cshtml", model);

        }

        public JsonResult CountryCodes()
        {
            var Result = (from a in DbCMS.codeCountries
                          from b in DbCMS.codeCountriesLanguages
                          where a.CountryGUID == b.CountryGUID
                          where b.LanguageID == LAN
                          select new { id = a.PhoneCode, text = b.CountryDescription, src = a.CountryA3Code }).ToList();
            return Json(Result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ContactDetailsUpdate(userContactDetails model)
        {
            DateTime ExecutionTime = DateTime.Now;

            model.UserGUID = UserGUID;

            var userContactDetails = DbCMS.userContactDetails.Find(model.ContactDetailsGUID);

            if (userContactDetails != null)
            {
                DbCMS.Update(model, Permissions.ContactDetails.UpdateGuid, ExecutionTime);
            }
            else
            {
                DbCMS.Create(model, Permissions.ContactDetails.CreateGuid, ExecutionTime);
            }
            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(null, null, null, "LoadUrl('PanelForContactDetailsForm','" + Url.Action("ContactDetailsView", "Profile") + "')"));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                //ViewBag.ErrorSummary = "Data has been modified by another user. <a href='" + Url.Action("Index", "Profile") + "'>Reload Page</a>";
                //ViewBag.ddlCountriesPhoneCode = CMS.ddlCountriesPhoneCode();
                ////Mapper.Map(DbCMS.userContactDetails.Where(s => s.UserGUID == UserGUID).FirstOrDefault(), model);
                //return PartialView("_ContactDetailsUpdate", model);

                return null;
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }
        #endregion

        #region Service History Section
        public ActionResult ServiceHistoryView()
        {
            Guid SupervisorID = Guid.Parse("A296279A-6B5E-4FA3-AA04-5C349E141000");
            Guid ReviewingOfficerID = Guid.Parse("91D63688-6763-4EC8-8EFB-DEAF7079BE9D");

            var model = (from a in DbCMS.userProfiles.Where(x => x.Active)
                         join b in DbCMS.userServiceHistory.Where(x => x.Active && x.UserGUID == UserGUID) on a.ServiceHistoryGUID equals b.ServiceHistoryGUID
                         orderby a.FromDate descending
                         select new ServiceHistoryModel
                         {
                             Organization = b.codeOrganizations.codeOrganizationsLanguages.Where(x => x.Active && x.LanguageID == LAN).FirstOrDefault().OrganizationDescription,
                             OrganizationInstanceDescription = b.codeOrganizations.codeOrganizationsInstances.Where(x => x.OrganizationInstanceGUID == a.OrganizationInstanceGUID).FirstOrDefault().codeOrganizationsInstancesLanguages.Where(x => x.Active && x.LanguageID == LAN).FirstOrDefault().OrganizationInstanceDescription,
                             EmailAddress = b.EmailAddress,
                             IndexNumber = b.IndexNumber,
                             EmployeeNumber = b.EmployeeNumber,
                             JobTitle = a.codeJobTitles.codeJobTitlesLanguages.Where(x => x.Active && x.LanguageID == LAN).FirstOrDefault().JobTitleDescription,
                             DutyStation = a.codeDutyStations.codeDutyStationsLanguages.Where(x => x.Active && x.LanguageID == LAN).FirstOrDefault().DutyStationDescription,
                             Grade = a.Grade,
                             Step = a.userStepsHistory.OrderBy(x => x.FromDate).FirstOrDefault().Step,
                             Department = a.codeDepartments.codeDepartmentsLanguages.Where(x => x.Active && x.LanguageID == LAN).FirstOrDefault().DepartmentDescription,
                             Supervisor = DbCMS.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN && x.UserGUID == a.userManagersHistory.Where(y => y.Active && y.UserProfileGUID == a.UserProfileGUID && y.ManagerTypeGUID == SupervisorID).OrderByDescending(y => y.FromDate).FirstOrDefault().ManagerGUID).Select(z => z.FirstName + " " + z.Surname).FirstOrDefault(),
                             ReviewingOfficer = DbCMS.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN && x.UserGUID == a.userManagersHistory.Where(y => y.Active && y.UserProfileGUID == a.UserProfileGUID && y.ManagerTypeGUID == ReviewingOfficerID).OrderByDescending(y => y.FromDate).FirstOrDefault().ManagerGUID).Select(z => z.FirstName + " " + z.Surname).FirstOrDefault(),
                         }).FirstOrDefault();

            #region
            //            var model = (from a in DbCMS.userServiceHistory.Where(x=>x.Active && x.UserGUID == UserGUID)
            //                         join org in DbCMS.codeOrganizationsLanguages.Where(x => x.LanguageID == LAN) on a.OrganizationGUID equals org.OrganizationGUID into R1

            //                         from r1 in R1.DefaultIfEmpty()
            //                         join b in DbCMS.userProfiles.Where(x => x.Active).OrderByDescending(x => x.FromDate) on a.ServiceHistoryGUID equals b.ServiceHistoryGUID into R2

            //                         from r2 in R2.DefaultIfEmpty()
            //                         join c in DbCMS.codeOrganizationsInstancesLanguages.Where(x => x.LanguageID == LAN && x.Active) on r2.OrganizationInstanceGUID equals c.OrganizationInstanceGUID into R3

            //                         from r3 in R3.DefaultIfEmpty()
            //                         join d in DbCMS.userServiceHistory.Where(x => x.Active) on r2.ServiceHistoryGUID equals d.ServiceHistoryGUID into R4

            //                         from r4 in R4.DefaultIfEmpty()
            //                         join e in DbCMS.codeDutyStationsLanguages.Where(x => x.LanguageID == LAN && x.Active) on r2.DutyStationGUID equals e.DutyStationGUID into R5

            //                         from r5 in R5.DefaultIfEmpty()
            //                         join f in DbCMS.codeDepartmentsLanguages.Where(x => x.LanguageID == LAN && x.Active) on r2.DepartmentGUID equals f.DepartmentGUID into R6

            //                         from r6 in R6.DefaultIfEmpty()
            //                         join g in DbCMS.codeJobTitlesLanguages.Where(x => x.LanguageID == LAN && x.Active) on r2.JobTitleGUID equals g.JobTitleGUID into R7

            //                         from r7 in R7.DefaultIfEmpty()
            //                         join h in DbCMS.userStepsHistory.Where(x => x.Active) on r2.UserProfileGUID equals h.UserProfileGUID into R8

            //                         from r8 in R8.DefaultIfEmpty()
            //                         join i in DbCMS.userManagersHistory //Should be a function
            //                         .Where(m => m.Active && m.FromDate == (from d in DbCMS.userManagersHistory
            //                                                                where d.userProfiles.userServiceHistory.UserGUID == UserGUID
            //                                                                select d.FromDate).Max())
            //                                                 on r2.UserProfileGUID equals i.UserProfileGUID into R9

            //                         from r9 in R9.DefaultIfEmpty()
            //                         join j in DbCMS.userPersonalDetailsLanguage.Where(l => l.LanguageID == LAN && l.Active) on r9.ManagerGUID equals j.UserGUID into R10

            //                         from r10 in R10.DefaultIfEmpty()
            //                         join k in DbCMS.userPersonalDetailsLanguage.Where(l => l.LanguageID == LAN && l.Active) on
            //                         r9.userProfiles.userManagersHistory //Should be a function
            //                         .Where(m => m.FromDate == (from d in DbCMS.userManagersHistory
            //                                                    where d.userProfiles.userServiceHistory.UserGUID == r9.ManagerGUID && d.Active
            //                                                    select d.FromDate).Max()).FirstOrDefault().ManagerGUID equals k.UserGUID into R11

            //                         from r11 in R11.DefaultIfEmpty()
            //                         select new ServiceHistoryModel
            //                         {
            //                             Organization = r1.OrganizationDescription,
            //                             EmailAddress = a.EmailAddress,
            //                             JobTitle = r7.JobTitleDescription,
            //                             OrganizationInstanceDescription = r3.OrganizationInstanceDescription,
            //                             OrganizationInstance = r3.OrganizationInstanceDescription,
            //                             DutyStation = r5.DutyStationDescription,
            //                             IndexNumber = a.IndexNumber,
            //                             EmployeeNumber = a.EmployeeNumber,
            //                             Department = r6.DepartmentDescription,
            //                             Grade = r2.Grade,
            //                             Step = r8.Step,
            //                             Supervisor = r10.FirstName + " " + r10.Surname,
            //                             ReviewingOfficer = r11.FirstName + " " + r11.Surname

            //                        }).FirstOrDefault();
            #endregion

            return PartialView("~/Views/Profile/ServiceHistory/_DetailsView.cshtml", model);
        }

        public ActionResult ServiceHistoryUpdate()
        {
            //Still need to find a way to order master table based on children values --Amer/ solved --Ayas
            List<userServiceHistory> model = (from a in DbCMS.userServiceHistory.Where(x => x.UserGUID == UserGUID && x.Active)
                                              select a).ToList();
            return PartialView("~/Views/Profile/ServiceHistory/_DetailsUpdate.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ServiceHistoryUpdate(string fakeParam)
        {
            return Json(DbCMS.SingleUpdateMessage(null, null, null, "LoadUrl('PanelForServiceHistoryForm','" + Url.Action("ServiceHistoryView", "Profile") + "')"));
        }

        public ActionResult ExperienceCreate()
        {
            return PartialView("~/Views/Profile/ServiceHistory/_ExperienceUpdateModal.cshtml", new userServiceHistoryModel());
        }

        public ActionResult ExperienceUpdate(string PK)
        {
            userServiceHistoryModel model = new userServiceHistoryModel();
            Guid _PK = Guid.Parse(PK);
            model = Mapper.Map(DbCMS.userServiceHistory.Where(s => s.ServiceHistoryGUID == _PK).FirstOrDefault(), model);
            return PartialView("~/Views/Profile/ServiceHistory/_ExperienceUpdateModal.cshtml", model);
        }

        public ActionResult ExperienceView(string PK)
        {
            userServiceHistory model = new userServiceHistory();
            Guid _PK = Guid.Parse(PK);
            model = Mapper.Map(DbCMS.userServiceHistory.Where(s => s.ServiceHistoryGUID == _PK).FirstOrDefault(), model);
            return PartialView("~/Views/Profile/ServiceHistory/_ExperienceView.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ExperienceCreate(userServiceHistoryModel model)
        {
            if (!ModelState.IsValid) return PartialView("~/Views/Profile/ServiceHistory/_ExperienceUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            userServiceHistory userServiceHistory = new userServiceHistory();
            userServiceHistory = Mapper.Map(model, userServiceHistory);
            userServiceHistory.UserGUID = UserGUID;

            DbCMS.Create(userServiceHistory, Permissions.OrganizationalExperience.CreateGuid, ExecutionTime);
            try
            {
                DbCMS.SaveChanges();
                JsonReturn jr = new JsonReturn();
                jr.Notify = new Notify { Type = MessageTypes.Success, Message = "Record Added Successfully" };
                string url = Url.Action("ExperienceView", "Profile", new { PK = userServiceHistory.ServiceHistoryGUID });
                jr.CallbackFunction = "$('#newExperience').load('" + url + "');";
                return Json(jr);
            }
            catch (DbUpdateConcurrencyException ex) { return null; }
            catch (Exception ex) { return Json(DbCMS.ErrorMessage(ex.Message)); }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ExperienceUpdate(userServiceHistoryModel model)
        {
            if (!ModelState.IsValid) return PartialView("~/Views/Profile/ServiceHistory/_ExperienceUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid UserServiceHistoryGUID = Guid.Parse(Session[SessionKeys.ServiceHistoryGUID].ToString());

            //Dont trust the client
            var UserService = DbCMS.userServiceHistory.Where(x => x.ServiceHistoryGUID == UserServiceHistoryGUID).FirstOrDefault();

            if (UserService != null)
            {
                UserService.IndexNumber = model.IndexNumber;
                UserService.EmployeeNumber = model.EmployeeNumber;
                UserService.UserGUID = UserGUID;
            }

            try
            {
                DbCMS.Update(UserService, Permissions.OrganizationalExperience.UpdateGuid, ExecutionTime);
                DbCMS.SaveChanges();
                JsonReturn jr = new JsonReturn();
                jr.Notify = new Notify { Type = MessageTypes.Success, Message = "Record Updated Successfully" };
                string url = Url.Action("ExperienceView", "Profile", new { PK = model.ServiceHistoryGUID });
                jr.CallbackFunction = "$('#" + model.ServiceHistoryGUID + "').load('" + url + "');";

                return Json(jr);
            }
            catch (DbUpdateConcurrencyException ex) { return null; }
            catch (Exception ex) { return Json(DbCMS.ErrorMessage(ex.Message)); }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ExperienceDelete(userServiceHistory model)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<userServiceHistory> DeletedUserServiceHistories = new List<userServiceHistory>();

            string query = DbCMS.QueryBuilder(new List<userServiceHistory> { model }, Permissions.OrganizationalExperience.RemoveGuid, SubmitTypes.Delete, "");

            var serviceHistory = DbCMS.Database.SqlQuery<userServiceHistory>(query).ToList();

            foreach (var sh in serviceHistory)
            {
                DeletedUserServiceHistories.Add(DbCMS.Delete(sh, ExecutionTime, Permissions.OrganizationalExperience.RemoveGuid));
            }
            try
            {
                int CommitedRows = DbCMS.SaveChanges();
                JsonReturn jr = new JsonReturn();
                jr.Notify = new Notify { Type = MessageTypes.Success, Message = "Record Deleted Successfully" };
                jr.CallbackFunction = "$('#" + model.ServiceHistoryGUID + "').remove()";
                return Json(jr);
            }
            catch (DbUpdateConcurrencyException)
            {
                return null;
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        public ActionResult JobCreate(Guid PK)
        {
            userProfilesModel model = new userProfilesModel()
            {
                ServiceHistoryGUID = PK
            };
            return PartialView("~/Views/Profile/ServiceHistory/_JobUpdateModal.cshtml", model);
        }

        public ActionResult JobUpdate(string PK)
        {
            userProfilesModel model = new userProfilesModel();
            Guid _PK = Guid.Parse(PK);
            model = Mapper.Map(DbCMS.userProfiles.Where(s => s.UserProfileGUID == _PK).FirstOrDefault(), model);
            return PartialView("~/Views/Profile/ServiceHistory/_JobUpdateModal.cshtml", model);
        }

        public ActionResult JobView(Guid PK)
        {
            var model = (from a in DbCMS.userProfiles.Where(x => x.Active && x.UserProfileGUID == PK)
                         join b in DbCMS.codeJobTitlesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.JobTitleGUID equals b.JobTitleGUID
                         join c in DbCMS.codeOrganizationsInstancesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.OrganizationInstanceGUID equals c.OrganizationInstanceGUID
                         join d in DbCMS.codeDutyStationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.DutyStationGUID equals d.DutyStationGUID into LJ1
                         from l1 in LJ1.DefaultIfEmpty()
                         join e in DbCMS.codeDepartmentsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.DepartmentGUID equals e.DepartmentGUID into LJ2
                         from l2 in LJ2.DefaultIfEmpty()
                         select new JobModel
                         {
                             UserProfileGUID = a.UserProfileGUID,
                             FromDate = a.FromDate,
                             JobTitle = b.JobTitleDescription,
                             Grade = a.Grade,
                             PositionNumber = a.PositionNumber,
                             OrganizationInstance = c.OrganizationInstanceDescription,
                             DutyStation = l1.DutyStationDescription,
                             Department = l2.DepartmentDescription,
                             StepHistoryList = a.userStepsHistory,
                             ManagerHistoryList = a.userManagersHistory
                         }).FirstOrDefault();

            //userProfiles model = new userProfiles();
            //Guid _PK = Guid.Parse(PK);
            //model = Mapper.Map(DbCMS.userProfiles.Where(s => s.UserProfileGUID == _PK).FirstOrDefault(), model);
            return PartialView("~/Views/Profile/ServiceHistory/_JobView.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult JobCreate(userProfilesModel model)
        {
            if (!ModelState.IsValid || !ValidateJobInfo(model)) return PartialView("~/Views/Profile/ServiceHistory/_JobUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            userProfiles userProfiles = new userProfiles();
            userProfiles = Mapper.Map(model, userProfiles);
            DbCMS.Create(userProfiles, Permissions.Jobs.CreateGuid, ExecutionTime);
            try
            {
                DbCMS.SaveChanges();
                JsonReturn jr = new JsonReturn();
                jr.Notify = new Notify { Type = MessageTypes.Success, Message = "Record Added Successfully" };
                string url = Url.Action("JobView", "Profile", new { PK = userProfiles.UserProfileGUID });
                string func1 = "$('#" + userProfiles.ServiceHistoryGUID + "NewJob').load('" + url + "');";
                string func2 = "$('#" + userProfiles.ServiceHistoryGUID + "NewJob').attr('id','" + userProfiles.UserProfileGUID + "');";
                string id = userProfiles.ServiceHistoryGUID.ToString() + "NewJob";
                string div = "<div id =\"" + id + "\" ></div>";
                string func3 = "$('#" + userProfiles.ServiceHistoryGUID + "JobSection').prepend('" + div + "');";
                jr.CallbackFunction = func1 + func2 + func3;
                //jr.CallbackFunction = "$('#" + userProfiles.ServiceHistoryGUID + "NewJob').load('" + url + "');";
                //jr.CallbackFunction = new List<string>();
                //string url = Url.Action("JobView", "Profile", new { PK = userProfiles.UserProfileGUID });
                //string func1 = "$('#" + userProfiles.ServiceHistoryGUID + "NewJob').load('" + url + "');";
                //string func2 = "$('#" + userProfiles.ServiceHistoryGUID + "NewJob').attr('id','" + userProfiles.UserProfileGUID + "Job');";
                //string func3 = "$('#" + userProfiles.ServiceHistoryGUID + "JobSection').prepend('<div id='"+ userProfiles.ServiceHistoryGUID + "NewJob'></div>');";
                //jr.CallbackFunction.Add(func1);
                //jr.CallbackFunction.Add(func2);
                //jr.CallbackFunction.Add(func3);
                return Json(jr);
            }
            catch (DbUpdateConcurrencyException ex) { return null; }
            catch (Exception ex) { return Json(DbCMS.ErrorMessage(ex.Message)); }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult JobUpdate(userProfilesModel model)
        {
            if (!ModelState.IsValid || !ValidateJobInfo(model)) return PartialView("~/Views/Profile/ServiceHistory/_JobUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            userProfiles userProfiles = new userProfiles();
            userProfiles = Mapper.Map(model, userProfiles);
            DbCMS.Update(userProfiles, Permissions.Jobs.UpdateGuid, ExecutionTime);
            try
            {
                DbCMS.SaveChanges();


                JsonReturn jr = new JsonReturn();
                jr.Notify = new Notify { Type = MessageTypes.Success, Message = "Record Updated Successfully" };
                string url = Url.Action("JobView", "Profile", new { PK = userProfiles.UserProfileGUID });
                jr.CallbackFunction = "$('#" + userProfiles.UserProfileGUID + "').load('" + url + "');";
                return Json(jr);
            }
            catch (DbUpdateConcurrencyException ex) { return null; }
            catch (Exception ex) { return Json(DbCMS.ErrorMessage(ex.Message)); }
        }

        private bool ValidateJobInfo(userProfilesModel model)
        {
            bool valid = true;
            if (model.FromDate >= model.ToDate)
            {
                ModelState.AddModelError("", "Job start must be less than job end date.");
                valid = false;
            }
            return valid;
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult JobDelete(userProfiles model)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<userProfiles> DeletedUserProfiles = new List<userProfiles>();

            string query = DbCMS.QueryBuilder(new List<userProfiles> { model }, Permissions.Jobs.RemoveGuid, SubmitTypes.Delete, "");

            var userProfile = DbCMS.Database.SqlQuery<userProfiles>(query).ToList();

            foreach (var up in userProfile)
            {
                DeletedUserProfiles.Add(DbCMS.Delete(up, ExecutionTime, Permissions.Jobs.RemoveGuid));
            }
            try
            {
                int CommitedRows = DbCMS.SaveChanges();
                JsonReturn jr = new JsonReturn();
                jr.Notify = new Notify { Type = MessageTypes.Success, Message = "Record Deleted Successfully" };
                jr.CallbackFunction = "$('#" + model.UserProfileGUID + "').remove()";
                return Json(jr);
            }
            catch (DbUpdateConcurrencyException)
            {
                return null;
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        public ActionResult StepCreate(Guid PK)
        {
            StepModel model = new StepModel();
            model.UserProfileGUID = PK;
            return PartialView("~/Views/Profile/ServiceHistory/_StepUpdateModal.cshtml", model);
        }

        public ActionResult StepUpdate(string PK)
        {
            StepModel model = new StepModel();
            Guid _PK = Guid.Parse(PK);
            model = Mapper.Map(DbCMS.userStepsHistory.Where(s => s.UserStepsHistoryGUID == _PK).FirstOrDefault(), model);
            return PartialView("~/Views/Profile/ServiceHistory/_StepUpdateModal.cshtml", model);
        }

        public ActionResult StepsView(string PK)
        {
            List<userStepsHistory> model = new List<userStepsHistory>();
            Guid _PK = Guid.Parse(PK);
            var test = DbCMS.userProfiles.Where(s => s.UserProfileGUID == _PK).ToList();
            model = Mapper.Map(DbCMS.userStepsHistory.Where(s => s.UserProfileGUID == _PK).ToList(), model);
            return PartialView("~/Views/Profile/ServiceHistory/_StepsView.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StepCreate(StepModel model)
        {
            if (!ModelState.IsValid) return PartialView("~/Views/Profile/ServiceHistory/_StepUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            userStepsHistory userStepsHistory = new userStepsHistory();
            userStepsHistory = Mapper.Map(model, userStepsHistory);
            DbCMS.Create(userStepsHistory, Permissions.Steps.CreateGuid, ExecutionTime);
            try
            {
                DbCMS.SaveChanges();
                JsonReturn jr = new JsonReturn();
                jr.Notify = new Notify { Type = MessageTypes.Success, Message = "Record Deleted Successfully" };
                string url = Url.Action("StepsView", "Profile", new { PK = userStepsHistory.UserProfileGUID });
                jr.CallbackFunction = "$('#" + userStepsHistory.UserProfileGUID + "Step').load('" + url + "');";
                return Json(jr);
            }
            catch (DbUpdateConcurrencyException ex) { return null; }
            catch (Exception ex) { return Json(DbCMS.ErrorMessage(ex.Message)); }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StepUpdate(StepModel model)
        {
            if (!ModelState.IsValid) return PartialView("~/Views/Profile/ServiceHistory/_StepUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            userStepsHistory userStepsHistory = new userStepsHistory();
            userStepsHistory = Mapper.Map(model, userStepsHistory);
            userStepsHistory.UserProfileGUID = UserProfileGUID;
            DbCMS.Update(userStepsHistory, Permissions.Steps.UpdateGuid, ExecutionTime);
            try
            {
                DbCMS.SaveChanges();
                JsonReturn jr = new JsonReturn();
                jr.Notify = new Notify { Type = MessageTypes.Success, Message = "Record Updated Successfully" };
                string url = Url.Action("StepsView", "Profile", new { PK = userStepsHistory.UserProfileGUID });
                jr.CallbackFunction = "$('#" + userStepsHistory.UserProfileGUID + "Step').load('" + url + "');";
                return Json(jr);
            }
            catch (DbUpdateConcurrencyException ex) { return null; }
            catch (Exception ex) { return Json(DbCMS.ErrorMessage(ex.Message)); }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StepDelete(userStepsHistory model)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<userStepsHistory> DeleteduserStepsHistory = new List<userStepsHistory>();

            string query = DbCMS.QueryBuilder(new List<userStepsHistory> { model }, Permissions.Steps.RemoveGuid, SubmitTypes.Delete, "");

            var serviceHistory = DbCMS.Database.SqlQuery<userStepsHistory>(query).ToList();

            foreach (var sh in serviceHistory)
            {
                DeleteduserStepsHistory.Add(DbCMS.Delete(sh, ExecutionTime, Permissions.Steps.RemoveGuid));
            }

            try
            {
                DbCMS.SaveChanges();
                JsonReturn jr = new JsonReturn();
                jr.Notify = new Notify { Type = MessageTypes.Success, Message = "Record Updated Successfully" };
                string url = Url.Action("StepsView", "Profile", new { PK = UserProfileGUID });
                jr.CallbackFunction = "$('#" + UserProfileGUID + "Step').load('" + url + "');";
                return Json(jr);
            }
            catch (DbUpdateConcurrencyException)
            {
                return null;
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }


        public ActionResult ManagerCreate(Guid PK)
        {
            userManagersHistory model = new userManagersHistory()
            {
                UserProfileGUID = PK
            };

            return PartialView("~/Views/Profile/ServiceHistory/_ManagerUpdateModal.cshtml", model);
        }

        public ActionResult ManagerUpdate(string PK)
        {
            userManagersHistory model = new userManagersHistory();
            Guid _PK = Guid.Parse(PK);
            model = Mapper.Map(DbCMS.userManagersHistory.Where(s => s.UserManagersHistoryGUID == _PK).FirstOrDefault(), model);
            return PartialView("~/Views/Profile/ServiceHistory/_ManagerUpdateModal.cshtml", model);
        }

        public ActionResult ManagersView(string PK)
        {
            List<userManagersHistory> model = new List<userManagersHistory>();
            Guid _PK = Guid.Parse(PK);
            model = Mapper.Map(DbCMS.userManagersHistory.Where(s => s.UserProfileGUID == _PK).ToList(), model);
            return PartialView("~/Views/Profile/ServiceHistory/_ManagersView.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ManagerCreate(userManagersHistory model)
        {
            if (!ModelState.IsValid) return PartialView("~/Views/Profile/ServiceHistory/_ManagerUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            if (ManagersValidationFailed(model))
            {
                return PartialView("~/Views/Profile/ServiceHistory/_ManagerUpdateModal.cshtml", model);
            }

            userManagersHistory userManagersHistory = new userManagersHistory();
            userManagersHistory = Mapper.Map(model, userManagersHistory);

            DbCMS.Create(userManagersHistory, Permissions.Managers.CreateGuid, ExecutionTime);

            try
            {
                DbCMS.SaveChanges();
                JsonReturn jr = new JsonReturn();
                jr.Notify = new Notify { Type = MessageTypes.Success, Message = "Record Added Successfully" };
                string url = Url.Action("ManagersView", "Profile", new { PK = userManagersHistory.UserProfileGUID });
                jr.CallbackFunction = "$('#" + userManagersHistory.UserProfileGUID + "Manager').load('" + url + "');";
                return Json(jr);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return null;
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ManagerUpdate(userManagersHistory model)
        {
            if (!ModelState.IsValid) return PartialView("~/Views/Profile/ServiceHistory/_ManagerUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            //////////VALIDATION////////////
            if (ManagersValidationFailed(model))
            {
                return PartialView("~/Views/Profile/ServiceHistory/_ManagerUpdateModal.cshtml", model);
            }
            //////////VALIDATION////////////
            userManagersHistory userManagersHistory = new userManagersHistory();
            userManagersHistory = Mapper.Map(model, userManagersHistory);
            userManagersHistory.UserProfileGUID = UserProfileGUID;
            DbCMS.Update(userManagersHistory, Permissions.Managers.UpdateGuid, ExecutionTime);
            try
            {
                DbCMS.SaveChanges();
                JsonReturn jr = new JsonReturn();
                jr.Notify = new Notify { Type = MessageTypes.Success, Message = "Record Updated Successfully" };
                string url = Url.Action("ManagersView", "Profile", new { PK = userManagersHistory.UserProfileGUID });
                jr.CallbackFunction = "$('#" + userManagersHistory.UserProfileGUID + "Manager').load('" + url + "');";
                return Json(jr);
            }
            catch (DbUpdateConcurrencyException ex) { return null; }
            catch (Exception ex) { return Json(DbCMS.ErrorMessage(ex.Message)); }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ManagerDelete(userManagersHistory model)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<userManagersHistory> DeleteduserManagersHistory = new List<userManagersHistory>();

            string query = DbCMS.QueryBuilder(new List<userManagersHistory> { model }, Permissions.Managers.RemoveGuid, SubmitTypes.Delete, "");

            var serviceHistory = DbCMS.Database.SqlQuery<userManagersHistory>(query).ToList();

            foreach (var sh in serviceHistory)
            {
                DeleteduserManagersHistory.Add(DbCMS.Delete(sh, ExecutionTime, Permissions.Managers.RemoveGuid));
            }
            try
            {

                DbCMS.SaveChanges();
                JsonReturn jr = new JsonReturn();
                jr.Notify = new Notify { Type = MessageTypes.Success, Message = "Record Deleted Successfully" };
                string url = Url.Action("ManagersView", "Profile", new { PK = UserProfileGUID });
                jr.CallbackFunction = "$('#" + UserProfileGUID + "Manager').load('" + url + "');";
                return Json(jr);
            }
            catch (DbUpdateConcurrencyException)
            {
                return null;
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        private bool ManagersValidationFailed(userManagersHistory model)
        {
            var Result = DbCMS.userManagersHistory.AsNoTracking().Where(
                s => s.ManagerGUID == model.ManagerGUID &&
                s.UserProfileGUID == model.UserProfileGUID &&
                s.FromDate == model.FromDate &&
                s.ToDate == model.ToDate &&
                s.UserManagersHistoryGUID != model.UserManagersHistoryGUID
                ).Count();

            if (Result > 0)
            {
                ModelState.AddModelError("ManagerGUID", "Manager already exists."); //From resource ?????? Amer  
            }
            return (Result > 0);
        }

        #endregion

        #region Home Address

        public ActionResult HomeAddressView()
        {
            var model = (from a in DbCMS.userHomeAddress.Where(x => x.UserGUID == UserGUID)
                         join b in DbCMS.userHomeAddressLanguage.Where(x => x.LanguageID == LAN) on a.UserGUID equals b.UserGUID into R1

                         from ha in R1.DefaultIfEmpty()

                         select new HomeAddressModel
                         {
                             HomeAddress = ha.HomeAddress,
                             HomePhone = a.HomePhoneCountryCode + " " + a.HomePhoneNumber,
                             Latitude = a.Latitude.ToString(),
                             Longitude = a.Longitude.ToString()
                         }).FirstOrDefault();

            return PartialView("~/Views/Profile/HomeAddress/_HomeAddressView.cshtml", model);
        }

        public ActionResult HomeAddressLanguageDataTable(DataTableRecievedOptions options)
        {
            if (options.columns == null)//Inject the datatable JavaScript
            {
                return PartialView("_HomeAddressLanguage");
            }
            else //It is loading or filter or sorting
            {
                DataTableOptions DataTable = ConvertOptions.Fill(options);

                Expression<Func<userHomeAddressLanguage, bool>> LanguagePredicate;
                LanguagePredicate = b => true;
                if (DataTable.Filters.FilterRules != null)
                {
                    LanguagePredicate = SearchHelper.CreateSearchPredicate<userHomeAddressLanguage>(DataTable.Filters);
                }

                var Result = DbCMS.userHomeAddressLanguage.AsExpandable().Where(LanguagePredicate).Where(u => u.UserGUID == UserGUID && u.LanguageID != LAN)
                    .Select(x => new
                    {
                        x.HomeAddressLanguageGUID,
                        x.LanguageID,
                        x.HomeAddress,
                        x.userHomeAddressLanguageRowVersion
                    });

                Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

                return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult HomeAddressUpdate()
        {
            var Result = DropDownList.CountriesPhoneCode();
            ViewBag.JsonCountries = JsonConvert.SerializeObject(Result);
            ViewBag.ddlCountriesPhoneCode = Result.Select(ddl => new { Value = ddl.id, Text = ddl.text }).ToList();
            var User = DbCMS.userHomeAddress.Find(UserGUID);
            var Languages = DbCMS.userHomeAddressLanguage.Where(x => x.UserGUID == UserGUID && x.LanguageID == LAN).Select(u => new { u.HomeAddress, u.userHomeAddressLanguageRowVersion }).FirstOrDefault();

            HomeAddressUpdateModel model = new HomeAddressUpdateModel();

            Mapper.Map(User, model);
            Mapper.Map(Languages, model);

            return PartialView("~/Views/Profile/HomeAddress/_HomeAddressUpdate.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult HomeAddressUpdate(HomeAddressUpdateModel model)
        {
            if (!ModelState.IsValid) return PartialView("~/Views/Profile/HomeAddress/_HomeAddressUpdate", model);

            DateTime ExecutionTime = DateTime.Now;

            model.UserGUID = UserGUID;

            userHomeAddress userHomeAddress = new userHomeAddress();
            Mapper.Map(model, userHomeAddress);

            if (DbCMS.userHomeAddress.Find(UserGUID) == null) {
                DbCMS.Create(userHomeAddress, Permissions.HomeAddress.CreateGuid, ExecutionTime);
            }
            else {
                DbCMS.Update(userHomeAddress, Permissions.HomeAddress.UpdateGuid, ExecutionTime);
            }

            var UserHomeAddressLanguage = DbCMS.userHomeAddressLanguage.Where(u => u.UserGUID == UserGUID && u.LanguageID == LAN).FirstOrDefault();

            if (UserHomeAddressLanguage != null)
            {
                Mapper.Map(model, UserHomeAddressLanguage);
                DbCMS.Update(UserHomeAddressLanguage, Permissions.HomeAddressLanguage.UpdateGuid, ExecutionTime);
            }
            else
            {
                UserHomeAddressLanguage = Mapper.Map<userHomeAddressLanguage>(model);
                UserHomeAddressLanguage.HomeAddressLanguageGUID = Guid.NewGuid();
                UserHomeAddressLanguage.LanguageID = LAN;
                UserHomeAddressLanguage.UserGUID = UserGUID;
                DbCMS.Create(UserHomeAddressLanguage, Permissions.HomeAddressLanguage.CreateGuid, ExecutionTime);
            }
            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(null, null, null, "LoadUrl('PanelForHomeAddressForm','" + Url.Action("HomeAddressView", "Profile") + "')"));
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                return Json(JsonMessages.ErrorMessage(DbCMS, s));
            }
        }

        #endregion

        #region Home Address Language

        public ActionResult HomeAddressLanguageCreate()
        {
            var Result = DropDownList.CountriesPhoneCode();
            ViewBag.JsonCountries = JsonConvert.SerializeObject(Result);
            ViewBag.ddlCountriesPhoneCode = Result.Select(ddl => new { Value = ddl.id, Text = ddl.text }).ToList();

            return PartialView("~/Views/Profile/HomeAddress/_LanguagesUpdateModal.cshtml", new userHomeAddressLanguage { UserGUID = UserGUID });
        }
        public ActionResult HomeAddressLanguageUpdate(string PK)
        {
            if (!string.IsNullOrEmpty(PK))
            {
                Guid _PK = Guid.Parse(PK);
                userHomeAddressLanguage model = Mapper.Map<userHomeAddressLanguage>(DbCMS.userHomeAddressLanguage.Find(_PK));
                return PartialView("~/Views/Profile/HomeAddress/_LanguagesUpdateModal.cshtml", model);
            }
            else
            {
                return PartialView("~/Views/Profile/HomeAddress/_LanguagesUpdateModal.cshtml", new userHomeAddressLanguage());
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult HomeAddressLanguageCreate(userHomeAddressLanguage model)
        {
            if (!ModelState.IsValid || ActiveName(model)) return PartialView("~/Views/Profile/HomeAddress/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbCMS.Create(model, Permissions.HomeAddressLanguage.CreateGuid, ExecutionTime);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.HomeAddressLanguageDataTable,
                   DbCMS.PrimaryKeyControl(model),
                   DbCMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult HomeAddressLanguageUpdate(userHomeAddressLanguage model)
        {
            if (!ModelState.IsValid || ActiveName(model)) return PartialView("~/Views/Profile/HomeAddress/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbCMS.Update(model, Permissions.HomeAddressLanguage.UpdateGuid, ExecutionTime);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.HomeAddressLanguageDataTable,
                    DbCMS.PrimaryKeyControl(model),
                    DbCMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyHomeAddressLanguage(model.HomeAddressLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult HomeAddressLanguageDelete(userHomeAddressLanguage model)
        {
            List<userHomeAddressLanguage> DeletedLanguage = DeleteHomeAddressLanguage(new List<userHomeAddressLanguage> { model });
            try
            {
                int CommitedRows = DbCMS.SaveChanges();
                return Json(DbCMS.SingleDeleteMessage(DeletedLanguage, DataTableNames.HomeAddressLanguageDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyHomeAddressLanguage(model.HomeAddressLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult HomeAddressLanguageRestore(userHomeAddressLanguage model)
        {
            List<userHomeAddressLanguage> RestoredNames = RestoreHomeAddressLanguage(Portal.SingleToList(model));

            try
            {
                int CommitedRows = DbCMS.SaveChanges();
                return Json(DbCMS.SingleRestoreMessage(CommitedRows, RestoredNames, DbCMS.PrimaryKeyControl(RestoredNames.FirstOrDefault()), Url.Action(DataTableNames.HomeAddressLanguageDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", null));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyHomeAddressLanguage(model.HomeAddressLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult HomeAddressLanguageDataTableDelete(List<userHomeAddressLanguage> models)
        {
            List<userHomeAddressLanguage> DeletedNames = DeleteHomeAddressLanguage(models);
            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialDeleteMessage(DeletedNames, models, DataTableNames.HomeAddressLanguageDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult HomeAddressNamesDataTableResotre(List<userHomeAddressLanguage> models)
        {
            List<userHomeAddressLanguage> RestoredLanguages = RestoreHomeAddressLanguage(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.HomeAddressLanguageDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        private List<userHomeAddressLanguage> DeleteHomeAddressLanguage(List<userHomeAddressLanguage> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<userHomeAddressLanguage> DeletedPersonalNames = new List<userHomeAddressLanguage>();

            string query = DbCMS.QueryBuilder(models, Permissions.HomeAddressLanguage.DeleteGuid, SubmitTypes.Delete, "");

            var names = DbCMS.Database.SqlQuery<userHomeAddressLanguage>(query).ToList();

            foreach (var name in names)
            {
                DeletedPersonalNames.Add(DbCMS.Delete(name, ExecutionTime, Permissions.HomeAddressLanguage.DeleteGuid));
            }

            return DeletedPersonalNames;
        }

        private List<userHomeAddressLanguage> RestoreHomeAddressLanguage(List<userHomeAddressLanguage> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<userHomeAddressLanguage> RestoredLanguages = new List<userHomeAddressLanguage>();
            string query = DbCMS.QueryBuilder(models, Permissions.HomeAddressLanguage.DeleteGuid, SubmitTypes.Restore, "");

            var Languages = DbCMS.Database.SqlQuery<userHomeAddressLanguage>(query).ToList();

            foreach (var language in Languages)
            {
                if (!ActiveName(language))
                {
                    RestoredLanguages.Add(DbCMS.Restore(language, Permissions.HomeAddressLanguage.DeleteGuid, Permissions.HomeAddressLanguage.RestoreGuid, ExecutionTime));
                }
            }

            return RestoredLanguages;
        }

        private bool ActiveName(userHomeAddressLanguage model)
        {
            //Language is the constrain
            int Avilable = DbCMS.userHomeAddressLanguage.Where(l => l.LanguageID == model.LanguageID && l.Active && l.UserGUID == model.UserGUID && l.UserGUID != model.UserGUID).Count();
            if (Avilable > 0)
            {
                ModelState.AddModelError("", "Name in selected language already exists."); //From resource ?????? Amer  
            }
            return (Avilable > 0);
        }

        private JsonResult ConcrrencyHomeAddressLanguage(Guid PK)
        {
            userHomeAddressLanguage Language = DbCMS.userHomeAddressLanguage.Where(x => x.HomeAddressLanguageGUID == PK).FirstOrDefault();
            var dbEntity = DbCMS.Entry(Language).GetDatabaseValues().ToObject();
            userHomeAddressLanguage dbLanguage = new userHomeAddressLanguage();
            dbLanguage = Mapper.Map(dbEntity, dbLanguage);
            if (Language.userHomeAddressLanguageRowVersion.SequenceEqual(dbLanguage.userHomeAddressLanguageRowVersion))
            {
                return Json(DbCMS.PermissionError());
            }
            return null;
        }

        #endregion

        #region Work Address

        public ActionResult WorkAddressView()
        {
            var model = (from a in DbCMS.userWorkAddress.Where(wa => wa.UserGUID == UserGUID)
                         join b in DbCMS.codeOfficesLanguages on a.OfficeGUID equals b.OfficeGUID
                         where (b.LanguageID == LAN && b.Active)
                         select new WorkAddressModel
                         {
                             OfficeGUID = b.OfficeDescription,
                             FloorNumber = a.FloorNumber.ToString(),
                             RoomNumber = a.RoomNumber
                         }).FirstOrDefault();

            return PartialView("~/Views/Profile/WorkAddress/_WorkAddressView.cshtml", model);
        }

        public ActionResult WorkAddressUpdate()
        {
            userWorkAddress model = DbCMS.userWorkAddress.Find(UserGUID);
            if (model == null)
            {
                model = new userWorkAddress();
                model.UserGUID = UserGUID;
            }

            return PartialView("~/Views/Profile/WorkAddress/_WorkAddressUpdate.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult WorkAddressUpdate(userWorkAddress model)
        {
            if (!ModelState.IsValid) return PartialView("~/Views/Profile/WorkAddress/_WorkAddressUpdate", model);

            DateTime ExecutionTime = DateTime.Now;

            model.UserGUID = UserGUID;

            if (DbCMS.userWorkAddress.Find(UserGUID) == null)
            {
                DbCMS.Create(model, Permissions.WorkAddress.CreateGuid, ExecutionTime);
            }
            else
            {
                DbCMS.Update(model, Permissions.WorkAddress.UpdateGuid, ExecutionTime);
            }

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(null, null, null, "LoadUrl('PanelForWorkAddressForm','" + Url.Action("WorkAddressView", "Profile") + "')"));
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                return Json(JsonMessages.ErrorMessage(DbCMS, s));
            }
        }
        #endregion

        #region Time Zone

        public ActionResult TimeZoneView()
        {
            string TimeZone = DbCMS.userAccounts.Where(x => x.Active && x.UserGUID == UserGUID).Select(x => x.TimeZone).FirstOrDefault();

            return PartialView("~/Views/Profile/TimeZone/_TimeZoneView.cshtml", TimeZone);
        }

        public ActionResult TimeZoneUpdate()
        {
            string model = DbCMS.userAccounts.Find(UserGUID).TimeZone;
            return PartialView("~/Views/Profile/TimeZone/_TimeZoneUpdate.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TimeZoneUpdate(string TimeZone)
        {
            ReadOnlyCollection<TimeZoneInfo> Zones = TimeZoneInfo.GetSystemTimeZones();
            int Result = Zones.Where(x => x.Id == TimeZone).Count();
            if (Result > 0)
            {
                var User = DbCMS.userAccounts.Where(x => x.Active && x.UserGUID == UserGUID).FirstOrDefault();
                User.TimeZone = TimeZone;

                DbCMS.Update(User, Permissions.Timezone.UpdateGuid, DateTime.Now);
            }
            else
            {
                return Json(DbCMS.ErrorMessage("Illegal Value"));
            }
            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(null, null, null, "LoadUrl('PanelForTimeZoneForm','" + Url.Action("TimeZoneView", "Profile") + "')"));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }
        #endregion

        #region Security Unit Informations

        public ActionResult SecurityUnitInfoView()
        {
            StafFSecurityUnitInfoModel model = new StafFSecurityUnitInfoModel();
            StaffCoreData staffCoreData = (from a in DbCMS.StaffCoreData.AsNoTracking().Where(x => x.UserGUID == UserGUID && x.Active)
                                           select a).FirstOrDefault();
            model.NextOfKinName = staffCoreData.NextOfKinName;
            model.KinMobileNumber = staffCoreData.KinMobileNumber;
            model.BSAFECertAcquired = staffCoreData.BSAFECertAcquired;
            model.BSAFEExpiryDate = staffCoreData.BSAFEExpiryDate;
            model.NumberOfDependants = staffCoreData.NumberOfDependants;
            model.DependantsName = staffCoreData.DependantsName;
            model.NextOfKinName = staffCoreData.NextOfKinName;

            return PartialView("~/Views/Profile/SecurityUnitInfo/_SecurityUnitInfoView.cshtml", model);
        }
        public ActionResult SecurityUnitInfoUpdate()
        {
            StafFSecurityUnitInfoModel model = new StafFSecurityUnitInfoModel();
            StaffCoreData staffCoreData = (from a in DbCMS.StaffCoreData.AsNoTracking().Where(x => x.UserGUID == UserGUID && x.Active)
                                           select a).FirstOrDefault();
            model = Mapper.Map(staffCoreData, new StafFSecurityUnitInfoModel());
            return PartialView("~/Views/Profile/SecurityUnitInfo/_SecurityUnitInfoUpdateModal.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]

        public ActionResult SecurityUnitInfoUpdate(StafFSecurityUnitInfoModel model)
        {
            StaffCoreData staffCoreData = (from a in DbCMS.StaffCoreData.Where(x => x.UserGUID == UserGUID && x.Active)
                                           select a).FirstOrDefault();
            staffCoreData.NextOfKinName = model.NextOfKinName;
            staffCoreData.KinMobileNumber = model.KinMobileNumber;
            staffCoreData.BSAFECertAcquired = model.BSAFECertAcquired;
            staffCoreData.BSAFEExpiryDate = model.BSAFEExpiryDate;
            staffCoreData.NumberOfDependants = model.NumberOfDependants;
            staffCoreData.DependantsName = model.DependantsName;
            staffCoreData.NextOfKinName = model.NextOfKinName;

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(null, null, null, "LoadUrl('SecurityUnitControlersFormControls','" + Url.Action("SecurityUnitInfoView", "Profile") + "')"));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }
        #endregion
        #region Application Info



        public ActionResult GetAppliationInfo(Guid ApplicationGUID)
        {

            AppInfoViewModel model = new AppInfoViewModel();
            var _myApp = DbCMS.codeApplications.Find(ApplicationGUID);
            model.AppAccess = 0;
            var _check = DbCMS.v_currentUserPermissions.Where(x => x.UserProfileGUID == UserProfileGUID && x.ApplicationGUID == ApplicationGUID).FirstOrDefault();
            if (_check != null)
            {
                model.AppAccess = 1;
            }
            model.AboutApplication = _myApp.AboutApplication;
            model.Active = true;
            model.ApplicationGUID = ApplicationGUID;
            model.ApplicationOwnerGUID = _myApp.AppOwnerGUID;
            var _staff = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            var _Owner = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == _myApp.AppOwnerGUID).FirstOrDefault();

            model.StaffName = _staff.FullName;
            model.StaffEmailAddress = _staff.EmailAddress;
            model.AppOwnerEmailAddress = _Owner.EmailAddress;
            model.UserGUID = UserGUID;


            model.ApplicationOwner = _Owner.FullName;
            model.Unit = _myApp.codeApplicationCategory.ApplicationCategoryName;
            model.ApplicationName = _myApp.codeApplicationsLanguages.Where(x => x.LanguageID == LAN && x.Active).FirstOrDefault() != null ? _myApp.codeApplicationsLanguages.Where(x => x.LanguageID == LAN && x.Active).FirstOrDefault().ApplicationDescription : "";
            return PartialView("~/Views/Home/_ApplicationInformation.cshtml", model);

        }
        public ActionResult RequestPermissionCreate(AppInfoViewModel model)
        {
            var _user = DbCMS.userPersonalDetailsLanguage.Where(x => x.UserGUID == UserGUID && x.LanguageID == LAN && x.Active == true).FirstOrDefault();
            var _appFocalPoint = DbCMS.userPersonalDetailsLanguage.Where(x => x.UserGUID == model.ApplicationOwnerGUID && x.LanguageID == LAN && x.Active == true).FirstOrDefault();
            if (_user != null && _appFocalPoint != null)
            {
                dataStaffApplicationAccessRequest _last = DbCMS.dataStaffApplicationAccessRequest.Where(x => x.ApplicationGUID == model.ApplicationGUID
                  && x.UserGUID == model.UserGUID && x.IsLastRequest == true).FirstOrDefault();
                DateTime ExecutionTime = DateTime.Now;
                if (_last != null)
                {
                    _last.IsLastRequest = false;
                    DbCMS.UpdateNoAudit(_last);

                }
                dataStaffApplicationAccessRequest toAdd = new dataStaffApplicationAccessRequest();
                toAdd.StaffApplicationAccessRequestGUID = Guid.NewGuid();
                toAdd.UserGUID = model.UserGUID;
                toAdd.ApplicationGUID = model.ApplicationGUID;
                toAdd.FlowStatusGUID = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7971");
                toAdd.IsLastRequest = true;
                toAdd.RequestDate = ExecutionTime;
                toAdd.RequestDate = ExecutionTime;

                DbCMS.CreateNoAudit(toAdd);
                DbCMS.SaveChanges();
                model.StaffApplicationAccessRequestGUID = toAdd.StaffApplicationAccessRequestGUID;
                new Email().SendApplicationPermissionRequest(model);
                return Json(DbCMS.SingleUpdateMessage(null, null, null, null));
            }
            return Json(DbAHD.PermissionError());
        }


        public ActionResult GrantUserFullAccessToApplication(Guid PK)
        {

            //if (!CMS.HasAction(Permissions.InternationalStaffEntitlements.Access, Apps.AHD))
            //{
            //    return Json(DbAHD.PermissionError());
            //}
            AppInfoViewModel myModel = new AppInfoViewModel();
            DateTime ExecutionTime = DateTime.Now;
            var model = DbCMS.dataStaffApplicationAccessRequest.Where(x => x.StaffApplicationAccessRequestGUID == PK).FirstOrDefault();
            var _myApp = DbCMS.codeApplications.Where(x => x.ApplicationGUID == model.ApplicationGUID).FirstOrDefault();

            if (_myApp.AppOwnerGUID != UserGUID)
            {
                return Json(DbAHD.PermissionError());
            }
            if (model.FlowStatusGUID == Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7972"))
            {
                //already submitted
                myModel.AccessGranted = 4;

                return View("~/Views/Profile/ApplicationAccessConfirmation.cshtml", myModel);
            }
            else if (model.FlowStatusGUID == Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7971"))
            {
                myModel.AccessGranted = 1;
                myModel.UserGUID = (Guid)model.UserGUID;
                var _staff = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == model.UserGUID).FirstOrDefault();
                var _Owner = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == _myApp.AppOwnerGUID).FirstOrDefault();

                myModel.StaffName = _staff.FullName;
                myModel.StaffEmailAddress = _staff.EmailAddress;
                myModel.AppOwnerEmailAddress = _Owner.EmailAddress;

                myModel.ApplicationOwnerGUID = _myApp.AppOwnerGUID;

                myModel.ApplicationOwner = _Owner.FullName;
                myModel.ApplicationName = _myApp.codeApplicationsLanguages.Where(x => x.LanguageID == LAN && x.Active).FirstOrDefault() != null ? _myApp.codeApplicationsLanguages.Where(x => x.LanguageID == LAN && x.Active).FirstOrDefault().ApplicationDescription : "";


                myModel.AccessGranted = 1;
                myModel.AccessGranted = 1;
                model.FlowStatusGUID = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7972");
                model.DecisionDate = ExecutionTime;
                model.DecidedByGUID = UserGUID;
                myModel.AccessType = "Full Access";

                DbCMS.UpdateNoAudit(model);
                DbCMS.SaveChanges();
                new Email().SendApproveAccessNotificationToStaff(myModel);
                return View("~/Views/Profile/ApplicationAccessConfirmation.cshtml", myModel);

            }
            else
            {
                return Json(DbAHD.PermissionError());
            }

        }

        public ActionResult GrantUserReadOnlyAccessToApplication(Guid PK)
        {

            //if (!CMS.HasAction(Permissions.InternationalStaffEntitlements.Access, Apps.AHD))
            //{
            //    return Json(DbAHD.PermissionError());
            //}
            AppInfoViewModel myModel = new AppInfoViewModel();
            DateTime ExecutionTime = DateTime.Now;
            var model = DbCMS.dataStaffApplicationAccessRequest.Where(x => x.StaffApplicationAccessRequestGUID == PK).FirstOrDefault();
            var _myApp = DbCMS.codeApplications.Where(x => x.ApplicationGUID == model.ApplicationGUID).FirstOrDefault();

            if (_myApp.AppOwnerGUID != UserGUID)
            {
                return Json(DbAHD.PermissionError());
            }
            if (model.FlowStatusGUID == Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7972"))
            {

                myModel.AccessGranted = 4;

                return View("~/Views/Profile/ApplicationAccessConfirmation.cshtml", myModel);
            }
            else if (model.FlowStatusGUID == Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7971"))
            {
                myModel.AccessGranted = 1;

                myModel.UserGUID = (Guid)model.UserGUID;
                var _staff = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == model.UserGUID).FirstOrDefault();
                var _Owner = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == _myApp.AppOwnerGUID).FirstOrDefault();

                myModel.StaffName = _staff.FullName;
                myModel.StaffEmailAddress = _staff.EmailAddress;
                myModel.AppOwnerEmailAddress = _Owner.EmailAddress;

                myModel.ApplicationOwnerGUID = _myApp.AppOwnerGUID;

                myModel.ApplicationOwner = _Owner.FullName;
                myModel.ApplicationName = _myApp.codeApplicationsLanguages.Where(x => x.LanguageID == LAN && x.Active).FirstOrDefault() != null ? _myApp.codeApplicationsLanguages.Where(x => x.LanguageID == LAN && x.Active).FirstOrDefault().ApplicationDescription : "";
                myModel.AccessType = "Read only access";

                model.FlowStatusGUID = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7972");
                model.DecisionDate = ExecutionTime;
                model.DecidedByGUID = UserGUID;
                DbCMS.UpdateNoAudit(model);
                DbCMS.SaveChanges();
                new Email().SendApproveAccessNotificationToStaff(myModel);

                return View("~/Views/Profile/ApplicationAccessConfirmation.cshtml", myModel);

            }
            else
            {
                return Json(DbAHD.PermissionError());
            }

        }
        public ActionResult RejectUserAccessToApplication(Guid PK)
        {

            //if (!CMS.HasAction(Permissions.InternationalStaffEntitlements.Access, Apps.AHD))
            //{
            //    return Json(DbAHD.PermissionError());
            //}
            AppInfoViewModel myModel = new AppInfoViewModel();
            DateTime ExecutionTime = DateTime.Now;
            var model = DbCMS.dataStaffApplicationAccessRequest.Where(x => x.StaffApplicationAccessRequestGUID == PK).FirstOrDefault();
            var _myApp = DbCMS.codeApplications.Where(x => x.ApplicationGUID == model.ApplicationGUID).FirstOrDefault();

            if (_myApp.AppOwnerGUID != UserGUID)
            {
                return Json(DbAHD.PermissionError());
            }
            if (model.FlowStatusGUID == Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7972"))
            {
                //already submitted
                myModel.AccessGranted = 4;

                return View("~/Views/Profile/ApplicationAccessConfirmation.cshtml", myModel);
            }
            else if (model.FlowStatusGUID == Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7971"))
            {
                myModel.AccessGranted = 1;
                model.FlowStatusGUID = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7973");
                model.DecisionDate = ExecutionTime;
                myModel.UserGUID = (Guid)model.UserGUID;
                var _staff = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == model.UserGUID).FirstOrDefault();
                var _Owner = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == _myApp.AppOwnerGUID).FirstOrDefault();

                myModel.StaffName = _staff.FullName;
                myModel.StaffEmailAddress = _staff.EmailAddress;
                myModel.AppOwnerEmailAddress = _Owner.EmailAddress;

                myModel.ApplicationOwnerGUID = _myApp.AppOwnerGUID;

                myModel.ApplicationOwner = _Owner.FullName;
                myModel.ApplicationName = _myApp.codeApplicationsLanguages.Where(x => x.LanguageID == LAN && x.Active).FirstOrDefault() != null ? _myApp.codeApplicationsLanguages.Where(x => x.LanguageID == LAN && x.Active).FirstOrDefault().ApplicationDescription : "";



                model.DecidedByGUID = UserGUID;
                DbCMS.UpdateNoAudit(model);
                DbCMS.SaveChanges();
                new Email().SendRejectionAccessNotificationToStaff(myModel);

                return View("~/Views/Profile/ApplicationAccessConfirmation.cshtml", myModel);

            }
            else
            {
                return Json(DbAHD.PermissionError());
            }

        }

        #endregion


    }
}