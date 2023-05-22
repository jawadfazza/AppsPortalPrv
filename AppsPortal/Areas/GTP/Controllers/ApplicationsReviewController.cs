using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AutoMapper;
using GTP_DAL.Model;
using LinqKit;
using RES_Repo.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.GTP.Controllers
{
    public class ApplicationsReviewController : GTPBaseController
    {


        [Route("GTP/ApplicationsReview/Applications/")]
        public ActionResult Index()
        {
            if (!CMS.HasAction(Permissions.GroupTwoApplicationsReview.Create, Apps.GTP))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/GTP/Views/ApplicationsReview/Index.cshtml");
        }

        [Route("GTP/ApplicationsReview/GroupTwoApplicationsDataTable/")]
        public JsonResult GroupTwoApplicationsDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<GroupTwoApplicationsDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<GroupTwoApplicationsDataTableModel>(DataTable.Filters);
            }

            var All = (from a in DbGTP.dataGTPApplications.AsExpandable()
                       join b in DbGTP.codeGTPCategories on a.GTPCategoryGUID equals b.GTPCategoryGUID
                       join c in DbGTP.userPersonalDetailsLanguages.Where(x => x.Active && x.LanguageID == "EN") on a.UserGUID equals c.UserGUID
                       join d in DbGTP.dataPersonalHistoryConfirmationAndConsents on a.GTPApplicationGUID equals d.GTPApplicationGUID into J1
                       from LJ1 in J1.DefaultIfEmpty()
                       select new GroupTwoApplicationsDataTableModel
                       {
                           GTPApplicationGUID = a.GTPApplicationGUID,
                           GTPCategoryGUID = a.GTPCategoryGUID.ToString(),
                           GTPApplicationCategory = b.GTPCategoryDescription,
                           UpdatedOn = a.UpdatedOn,
                           FullName = c.FirstName + " " + c.Surname,
                           IsReviewed = a.IsReviewed.HasValue ? a.IsReviewed.Value : false,
                           GTPEligibility = a.IsEligible.HasValue ? (a.IsEligible.Value == true ? "Eligible" : "Not Eligible") : "Un-Reviewed",
                           IsEligible = a.IsEligible,
                           Active = a.Active,
                           IsConfirmedByUser = LJ1.IsConfirmedByUser,
                           dataGTPApplicationRowVersion = a.dataGTPApplicationRowVersion
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<GroupTwoApplicationsDataTableModel> Result = Mapper.Map<List<GroupTwoApplicationsDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ApplicationReviewUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.GroupTwoApplicationsReview.Create, Apps.GTP))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            ApplicationReviewUpdateModel model = new ApplicationReviewUpdateModel();

            var result = (from a in DbGTP.dataGTPApplications where a.GTPApplicationGUID == PK select a).FirstOrDefault();
            model.GTPApplicationGUID = result.GTPApplicationGUID;
            model.GTPCategoryGUID = result.GTPCategoryGUID;
            model.IsEligible = result.IsEligible;

            string emailAddress = (from a in DbCMS.userServiceHistory where a.UserGUID == result.UserGUID select a.EmailAddress).FirstOrDefault();
            string fullName = (from a in DbCMS.userPersonalDetailsLanguage where a.UserGUID == result.UserGUID && a.Active && a.LanguageID == "EN" select a.FirstName + " " + a.Surname).FirstOrDefault();
            model.EmailAddress = emailAddress;
            model.FullName = fullName;
            try
            {
                dataPersonalHistoryEmailAddress dataPersonalHistoryEmailAddress = result.dataPersonalHistoryEmailAddresses.FirstOrDefault();
                if (dataPersonalHistoryEmailAddress.PreferedEmailAddress == 1)
                {
                    model.EmailAddress = dataPersonalHistoryEmailAddress.HomeEmailAddress;
                }
                else
                {
                    model.EmailAddress = dataPersonalHistoryEmailAddress.BusinessEmailAddress;
                }
            }
            catch { }
            string Message = resxEmails.GTPEmailForSuccessfulApplicants.Replace("$FullName", fullName).Replace("$ApplicantEmailAddress", model.EmailAddress);
            if (result.GTPApplicationExpiryDate.HasValue)
            {
                model.GTPApplicationExpiryDate = result.GTPApplicationExpiryDate.Value;
                Message = Message.Replace("$ExpiryDate", model.GTPApplicationExpiryDate.Value.ToShortDateString());
            }
            //string _body = "<div style='font-family:Arial;'>" + Message.Replace("\r\n", "<br/>") + "</div>";
            model.EmailBody = Message;

            return PartialView("~/Areas/GTP/Views/ApplicationsReview/_ApplicationsReviewUpdateModal.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ApplicationReviewUpdate(ApplicationReviewUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.GroupTwoApplicationsReview.Create, Apps.GTP))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.GTPApplicationGUID == model.GTPApplicationGUID select a).FirstOrDefault();

            if (dataGTPApplication.dataPersonalHistoryConfirmationAndConsents.FirstOrDefault() == null)
            {
                return Json(DbCMS.ErrorMessage("P11 form for current selection is missing or not completed"));
            }
            else if (dataGTPApplication.dataPersonalHistoryConfirmationAndConsents.FirstOrDefault().IsConfirmedByUser == false)
            {
                return Json(DbCMS.ErrorMessage("P11 form for current selection is missing or not completed"));
            }


            dataGTPApplication.IsEligible = model.IsEligible.Value;
            dataGTPApplication.IsReviewed = true;
            dataGTPApplication.ReviewedBy = UserGUID;
            dataGTPApplication.ReviewedOn = DateTime.Now;
            dataGTPApplication.GTPApplicationEligibleAs = DateTime.Now;
            dataGTPApplication.GTPApplicationExpiryDate = model.GTPApplicationExpiryDate;
            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();
                string emailAddress = (from a in DbCMS.userServiceHistory where a.UserGUID == dataGTPApplication.UserGUID select a.EmailAddress).FirstOrDefault();
                string fullName = (from a in DbCMS.userPersonalDetailsLanguage where a.UserGUID == dataGTPApplication.UserGUID && a.Active && a.LanguageID == "EN" select a.FirstName + " " + a.Surname).FirstOrDefault();

                dataPersonalHistoryEmailAddress dataPersonalHistoryEmailAddress = (from a in DbGTP.dataPersonalHistoryEmailAddresses where a.GTPApplicationGUID == dataGTPApplication.GTPApplicationGUID select a).FirstOrDefault();
                string preferredEamilAddress = "";
                if (dataPersonalHistoryEmailAddress.PreferedEmailAddress == 1)
                {
                    preferredEamilAddress = dataPersonalHistoryEmailAddress.HomeEmailAddress;
                }
                else
                {
                    preferredEamilAddress = dataPersonalHistoryEmailAddress.BusinessEmailAddress;
                }

                if (model.IsEligible.HasValue && model.IsEligible == true)
                {
                    new Email().SendGTPEmailForSuccessfulApplicantsEmail(preferredEamilAddress, emailAddress, fullName, preferredEamilAddress, model.GTPApplicationExpiryDate.Value.ToShortDateString(), model.EmailBody, "Outcome of the Group 2 Eligibility Process");
                }
                else if (model.IsEligible.HasValue && model.IsEligible == false)
                {
                    new Email().SendGTPEmailForUnsuccessfulApplicantsEmail(preferredEamilAddress, emailAddress, fullName, "Outcome of the Group 2 Eligibility Process");
                }
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.GroupTwoApplicationsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ApplicationReviewSetReviewed(ApplicationReviewUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.GroupTwoApplicationsReview.Create, Apps.GTP))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.GTPApplicationGUID == model.GTPApplicationGUID select a).FirstOrDefault();
            if (dataGTPApplication.dataPersonalHistoryConfirmationAndConsents.FirstOrDefault() != null)
            {
                if (dataGTPApplication.dataPersonalHistoryConfirmationAndConsents.FirstOrDefault().IsConfirmedByUser == false)
                {
                    return Json(DbCMS.ErrorMessage("P11 form for current selection is missing"));
                }
            }
            dataGTPApplication.IsReviewed = true;
            dataGTPApplication.ReviewedBy = UserGUID;
            dataGTPApplication.ReviewedOn = DateTime.Now;
            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.GroupTwoApplicationsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }


        public ActionResult ChangeApplicationCategoryUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.GroupTwoApplicationsReview.Create, Apps.GTP))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            ApplicationReviewUpdateModel model = new ApplicationReviewUpdateModel();

            var result = (from a in DbGTP.dataGTPApplications where a.GTPApplicationGUID == PK select a).FirstOrDefault();
            model.GTPApplicationGUID = result.GTPApplicationGUID;
            model.GTPCategoryGUID = result.GTPCategoryGUID;
            model.IsEligible = result.IsEligible;
            string emailAddress = (from a in DbCMS.userServiceHistory where a.UserGUID == result.UserGUID select a.EmailAddress).FirstOrDefault();
            string fullName = (from a in DbCMS.userPersonalDetailsLanguage where a.UserGUID == result.UserGUID && a.Active && a.LanguageID == "EN" select a.FirstName + " " + a.Surname).FirstOrDefault();
            model.EmailAddress = emailAddress;
            model.FullName = fullName;
            return PartialView("~/Areas/GTP/Views/ApplicationsReview/_ApplicationsChangeCategoryUpdateModal.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ChangeApplicationCategoryUpdate(ApplicationReviewUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.GroupTwoApplicationsReview.Create, Apps.GTP))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.GTPApplicationGUID == model.GTPApplicationGUID select a).FirstOrDefault();
            dataGTPApplication.GTPCategoryGUID = model.GTPCategoryGUID;
            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.GroupTwoApplicationsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }


        [Route("GTP/ApplicationsReview/PersonalHistoryFormReview/{PK}")]
        public ActionResult PersonalHistoryFormReview(Guid PK)
        {
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications
                                                     where a.GTPApplicationGUID == PK
                                                     select a).FirstOrDefault();

            GTPPersonalHistoryFormUpdateModel model = new GTPPersonalHistoryFormUpdateModel();
            model.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            model.Active = true;

            dataPersonalHistoryConfirmationAndConsent dataPersonalHistoryConfirmationAndConsent = dataGTPApplication.dataPersonalHistoryConfirmationAndConsents.FirstOrDefault();

            if (dataPersonalHistoryConfirmationAndConsent == null)
            {
                return View("~/Areas/GTP/Views/ApplicationsReview/NoPersonalHistory.cshtml");
            }

            dataPersonalHistoryGeneralInfo dataPersonalHistoryGeneralInfo = dataGTPApplication.dataPersonalHistoryGeneralInfoes.FirstOrDefault();
            model.dataPersonalHistoryGeneralInfoUpdateModel = Mapper.Map(dataPersonalHistoryGeneralInfo, new dataPersonalHistoryGeneralInfoUpdateModel());

            dataPersonalHistoryPersonalInfo dataPersonalHistoryPersonalInfo = dataGTPApplication.dataPersonalHistoryPersonalInfoes.FirstOrDefault();
            model.dataPersonalHistoryPersonalInfoUpdateModel = Mapper.Map(dataPersonalHistoryPersonalInfo, new dataPersonalHistoryPersonalInfoUpdateModel());

            dataPersonalHistoryContactInfo dataPersonalHistoryContactInfo = dataGTPApplication.dataPersonalHistoryContactInfoes.FirstOrDefault();
            model.dataPersonalHistoryContactInfoUpdateModel = Mapper.Map(dataPersonalHistoryContactInfo, new dataPersonalHistoryContactInfoUpdateModel());

            dataPersonalHistoryPhoneNumber dataPersonalHistoryPhoneNumber = dataGTPApplication.dataPersonalHistoryPhoneNumbers.FirstOrDefault();
            model.dataPersonalHistoryPhoneNumberUpdateModel = Mapper.Map(dataPersonalHistoryPhoneNumber, new dataPersonalHistoryPhoneNumberUpdateModel());


            dataPersonalHistoryEmailAddress dataPersonalHistoryEmailAddress = dataGTPApplication.dataPersonalHistoryEmailAddresses.FirstOrDefault();
            model.dataPersonalHistoryEmailAddressUpdateModel = Mapper.Map(dataPersonalHistoryEmailAddress, new dataPersonalHistoryEmailAddressUpdateModel());

            dataPersonalHistoryNationalityInfo dataPersonalHistoryNationalityInfo = dataGTPApplication.dataPersonalHistoryNationalityInfoes.FirstOrDefault();
            model.dataPersonalHistoryNationalityInfoUpdateModel = Mapper.Map(dataPersonalHistoryNationalityInfo, new dataPersonalHistoryNationalityInfoUpdateModel());
            model.dataPersonalHistoryNationalityInfoUpdateModel.ClientNationalityAtBirth = dataPersonalHistoryNationalityInfo.NationalityAtBirth;
            model.dataPersonalHistoryNationalityInfoUpdateModel.ClientCurrentNationality = dataPersonalHistoryNationalityInfo.CurrentNationality;


            dataPersonalHistoryLetterOfInterest dataPersonalHistoryLetterOfInterest = dataGTPApplication.dataPersonalHistoryLetterOfInterests.FirstOrDefault();
            model.dataPersonalHistoryLetterOfInterestUpdateModel = Mapper.Map(dataPersonalHistoryLetterOfInterest, new dataPersonalHistoryLetterOfInterestUpdateModel());


            List<dataPersonalHistoryWorkExperience> dataPersonalHistoryWorkExperiences = new List<dataPersonalHistoryWorkExperience>();
            if (dataGTPApplication.dataPersonalHistoryWorkExperiences != null && dataGTPApplication.dataPersonalHistoryWorkExperiences.Count > 0)
            {
                model.dataPersonalHistoryWorkExperienceUpdateModel = new List<dataPersonalHistoryWorkExperienceUpdateModel>();
            }
            foreach (var item in dataGTPApplication.dataPersonalHistoryWorkExperiences)
            {
                dataPersonalHistoryWorkExperienceUpdateModel dataPersonalHistoryWorkExperienceUpdateModel = new dataPersonalHistoryWorkExperienceUpdateModel();
                dataPersonalHistoryWorkExperienceUpdateModel = Mapper.Map(item, new dataPersonalHistoryWorkExperienceUpdateModel());
                model.dataPersonalHistoryWorkExperienceUpdateModel.Add(dataPersonalHistoryWorkExperienceUpdateModel);
            }

            List<dataPersonalHistorySpecializedTraining> dataPersonalHistorySpecializedTrainings = new List<dataPersonalHistorySpecializedTraining>();
            if (dataGTPApplication.dataPersonalHistorySpecializedTrainings != null && dataGTPApplication.dataPersonalHistorySpecializedTrainings.Count > 0)
            {
                model.dataPersonalHistorySpecializedTrainingUpdateModel = new List<dataPersonalHistorySpecializedTrainingUpdateModel>();

            }
            foreach (var item in dataGTPApplication.dataPersonalHistorySpecializedTrainings)
            {
                dataPersonalHistorySpecializedTrainingUpdateModel dataPersonalHistorySpecializedTrainingUpdateModel = new dataPersonalHistorySpecializedTrainingUpdateModel();
                dataPersonalHistorySpecializedTrainingUpdateModel = Mapper.Map(item, new dataPersonalHistorySpecializedTrainingUpdateModel());
                model.dataPersonalHistorySpecializedTrainingUpdateModel.Add(dataPersonalHistorySpecializedTrainingUpdateModel);
            }

            List<dataPersonalHistoryEducation> dataPersonalHistoryEducations = new List<dataPersonalHistoryEducation>();
            if (dataGTPApplication.dataPersonalHistoryEducations != null && dataGTPApplication.dataPersonalHistoryEducations.Count > 0)
            {
                model.dataPersonalHistoryEducationUpdateModel = new List<dataPersonalHistoryEducationUpdateModel>();

            }
            foreach (var item in dataGTPApplication.dataPersonalHistoryEducations)
            {
                dataPersonalHistoryEducationUpdateModel dataPersonalHistoryEducationUpdateModel = new dataPersonalHistoryEducationUpdateModel();
                dataPersonalHistoryEducationUpdateModel = Mapper.Map(item, new dataPersonalHistoryEducationUpdateModel());
                model.dataPersonalHistoryEducationUpdateModel.Add(dataPersonalHistoryEducationUpdateModel);
            }

            List<dataPersonalHistorySkill> dataPersonalHistorySkills = new List<dataPersonalHistorySkill>();
            if (dataGTPApplication.dataPersonalHistorySkills != null && dataGTPApplication.dataPersonalHistorySkills.Count > 0)
            {
                model.dataPersonalHistorySkillUpdateModel = new List<dataPersonalHistorySkillUpdateModel>();

            }
            foreach (var item in dataGTPApplication.dataPersonalHistorySkills)
            {
                dataPersonalHistorySkillUpdateModel dataPersonalHistorySkillUpdateModel = new dataPersonalHistorySkillUpdateModel();
                dataPersonalHistorySkillUpdateModel = Mapper.Map(item, new dataPersonalHistorySkillUpdateModel());
                model.dataPersonalHistorySkillUpdateModel.Add(dataPersonalHistorySkillUpdateModel);
            }


            dataPersonalHistoryLanguage dataPersonalHistoryLanguage = dataGTPApplication.dataPersonalHistoryLanguages.FirstOrDefault();
            model.dataPersonalHistoryLanguageUpdateModel = Mapper.Map(dataPersonalHistoryLanguage, new dataPersonalHistoryLanguageUpdateModel());


            List<dataPersonalHistoryLicenceAndCertificate> dataPersonalHistoryLicenceAndCertificates = new List<dataPersonalHistoryLicenceAndCertificate>();
            if (dataGTPApplication.dataPersonalHistoryLicenceAndCertificates != null && dataGTPApplication.dataPersonalHistoryLicenceAndCertificates.Count > 0)
            {
                model.dataPersonalHistoryLicenceAndCertificateUpdateModel = new List<dataPersonalHistoryLicenceAndCertificateUpdateModel>();
            }
            foreach (var item in dataGTPApplication.dataPersonalHistoryLicenceAndCertificates)
            {
                dataPersonalHistoryLicenceAndCertificateUpdateModel dataPersonalHistoryLicenceAndCertificateUpdateModel = new dataPersonalHistoryLicenceAndCertificateUpdateModel();
                dataPersonalHistoryLicenceAndCertificateUpdateModel = Mapper.Map(item, new dataPersonalHistoryLicenceAndCertificateUpdateModel());
                model.dataPersonalHistoryLicenceAndCertificateUpdateModel.Add(dataPersonalHistoryLicenceAndCertificateUpdateModel);
            }


            dataPersonalHistoryProfessionalReference dataPersonalHistoryProfessionalReference = dataGTPApplication.dataPersonalHistoryProfessionalReferences.FirstOrDefault();
            model.dataPersonalHistoryProfessionalReferenceUpdateModel = Mapper.Map(dataPersonalHistoryProfessionalReference, new dataPersonalHistoryProfessionalReferenceUpdateModel());

            dataPersonalHistoryQuestionnaire dataPersonalHistoryQuestionnaire = dataGTPApplication.dataPersonalHistoryQuestionnaires.FirstOrDefault();
            model.dataPersonalHistoryQuestionnaireUpdateModel = Mapper.Map(dataPersonalHistoryQuestionnaire, new dataPersonalHistoryQuestionnaireUpdateModel());

            List<dataPersonalHistoryRelative> dataPersonalHistoryRelatives = new List<dataPersonalHistoryRelative>();
            if (dataGTPApplication.dataPersonalHistoryRelatives != null && dataGTPApplication.dataPersonalHistoryRelatives.Count > 0)
            {
                model.dataPersonalHistoryRelativeUpdateModel = new List<dataPersonalHistoryRelativeUpdateModel>();
            }
            foreach (var item in dataGTPApplication.dataPersonalHistoryRelatives)
            {
                dataPersonalHistoryRelativeUpdateModel dataPersonalHistoryRelativeUpdateModel = new dataPersonalHistoryRelativeUpdateModel();
                dataPersonalHistoryRelativeUpdateModel = Mapper.Map(item, new dataPersonalHistoryRelativeUpdateModel());
                model.dataPersonalHistoryRelativeUpdateModel.Add(dataPersonalHistoryRelativeUpdateModel);
            }

            List<dataPersonalHistoryRelativeWorker> dataPersonalHistoryRelativeWorkers = new List<dataPersonalHistoryRelativeWorker>();
            if (dataGTPApplication.dataPersonalHistoryRelativeWorkers != null && dataGTPApplication.dataPersonalHistoryRelativeWorkers.Count > 0)
            {
                model.dataPersonalHistoryRelativeWorkerUpdateModel = new List<dataPersonalHistoryRelativeWorkerUpdateModel>();
            }
            foreach (var item in dataGTPApplication.dataPersonalHistoryRelativeWorkers)
            {
                dataPersonalHistoryRelativeWorkerUpdateModel dataPersonalHistoryRelativeWorkerUpdateModel = new dataPersonalHistoryRelativeWorkerUpdateModel();
                dataPersonalHistoryRelativeWorkerUpdateModel = Mapper.Map(item, new dataPersonalHistoryRelativeWorkerUpdateModel());
                model.dataPersonalHistoryRelativeWorkerUpdateModel.Add(dataPersonalHistoryRelativeWorkerUpdateModel);
            }

            model.dataPersonalHistoryConfirmationAndConsentUpdateModel = Mapper.Map(dataPersonalHistoryConfirmationAndConsent, new dataPersonalHistoryConfirmationAndConsentUpdateModel());

            return View("~/Areas/GTP/Views/ApplicationsReview/PersonalHistory.cshtml", model);
        }


    }
}