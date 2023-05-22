using AppsPortal.BaseControllers;
using AppsPortal.Data;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.ViewModels;
using AutoMapper;
using GTP_DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.GTP.Controllers
{
    public class ApplicationsController : GTPBaseController
    {
        // GET: GTP/Applications
        public ActionResult Index()
        {
            return View("~/Areas/GTP/Views/Applications/Index.cshtml");
        }

        #region Original
        [Route("GTP/Applications/Create")]
        public ActionResult ApplicationCreate()
        {
            var application = (from a in DbGTP.dataGTPApplications
                               where a.UserGUID == UserGUID
                               select a).FirstOrDefault();

            if (application != null)
            {
                GTPApplicationUpdateModel model = new GTPApplicationUpdateModel();
                model = Mapper.Map(application, new GTPApplicationUpdateModel());
                if (application.dataPersonalHistoryGeneralInfoes != null)
                {
                    model.IsNew = true;

                }
                else
                {
                    model.IsNew = false;
                }
                return View("~/Areas/GTP/Views/Applications/Application.cshtml", model);
            }
            else
            {
                GTPApplicationUpdateModel model = new GTPApplicationUpdateModel();
                model.IsNew = false;
                return View("~/Areas/GTP/Views/Applications/Application.cshtml", model);
            }
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult ApplicationCreate(GTPApplicationUpdateModel model)
        {
            DateTime ExecutionTime = DateTime.Now;

            dataGTPApplication dataGTPApplication = new dataGTPApplication();
            dataGTPApplication.GTPApplicationGUID = Guid.NewGuid();
            dataGTPApplication.GTPCategoryGUID = model.GTPCategoryGUID;
            dataGTPApplication.GTPApplicationDate = DateTime.Now;
            dataGTPApplication.IsReviewed = false;
            dataGTPApplication.Active = true;
            dataGTPApplication.UserGUID = UserGUID;
            dataGTPApplication.UserGUID = UserGUID;
            DbGTP.Create(dataGTPApplication, Permissions.Portalforgrouptwoapplications.CreateGuid, ExecutionTime, DbCMS);

            //dataPersonalHistoryQuestionnaire dataPersonalHistoryQuestionnaire = new dataPersonalHistoryQuestionnaire();
            //dataPersonalHistoryQuestionnaire.GTPPHQuestionnaireGUID = Guid.NewGuid();
            //dataPersonalHistoryQuestionnaire.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            //dataPersonalHistoryQuestionnaire.FirstQuestionAnswer = false;
            //dataPersonalHistoryQuestionnaire.SecondQuestionAnswer = false;
            //dataPersonalHistoryQuestionnaire.ThirdQuestionAnswer = false;
            //dataPersonalHistoryQuestionnaire.FourthQuestionAnswer = false;
            //dataPersonalHistoryQuestionnaire.FifthQuestionAnswer = false;
            //dataPersonalHistoryQuestionnaire.SixthQuestionAnswer = false;
            //dataPersonalHistoryQuestionnaire.SeventhQuestionAnswer = false;
            //dataPersonalHistoryQuestionnaire.EighthQuestionAnswerDetails = false;
            //dataPersonalHistoryQuestionnaire.NinthQuestionAnswer = false;
            //dataPersonalHistoryQuestionnaire.TenthQuestionAnswer = false;
            //dataPersonalHistoryQuestionnaire.ElevenQuestionAnswer = false;
            //dataPersonalHistoryQuestionnaire.TwelveQuestionAnswerDetails = false;
            //dataPersonalHistoryQuestionnaire.Active = true;
            //DbGTP.Create(dataPersonalHistoryQuestionnaire, Permissions.Portalforgrouptwoapplications.CreateGuid, ExecutionTime, DbCMS);


            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();

                //send email

                //string emailAddress = (from a in DbCMS.StaffCoreData where a.UserGUID == UserGUID select a.EmailAddress).FirstOrDefault();
                //string fullName = (from a in DbCMS.userPersonalDetailsLanguage where a.UserGUID == UserGUID && a.Active && a.LanguageID == "EN" select a.FirstName + " " + a.Surname).FirstOrDefault();

                //new Email().SendGTPConfirmReceivingApplicationEmail(emailAddress, null, fullName, "Email confirmation of receiving the application");


                List<PrimaryKeyControl> primaryKeyControls = new List<PrimaryKeyControl>();
                primaryKeyControls.Add(DbGTP.PrimaryKeyControl(dataGTPApplication));
                string callBackFunc = "RedirectToPersonalHistory();";
                model = AutoMapper.Mapper.Map(dataGTPApplication, model);
                model.dataGTPApplicationRowVersion.CopyTo(dataGTPApplication.dataGTPApplicationRowVersion, 0);
                return Json(DbGTP.SingleCreateMessage(primaryKeyControls, DbGTP.RowVersionControls(Portal.SingleToList(model)), null, callBackFunc));

            }
            catch (Exception ex)
            {
                return Json(DbGTP.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult ApplicationUpdate(GTPApplicationUpdateModel model)
        {
            DateTime ExecutionTime = DateTime.Now;

            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.GTPApplicationGUID == model.GTPApplicationGUID select a).FirstOrDefault();

            dataGTPApplication.GTPCategoryGUID = model.GTPCategoryGUID;
            dataGTPApplication.IsReviewed = false;
            dataGTPApplication.UpdatedOn = DateTime.Now;
            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();

                List<PrimaryKeyControl> primaryKeyControls = new List<PrimaryKeyControl>();
                primaryKeyControls.Add(DbGTP.PrimaryKeyControl(dataGTPApplication));
                string callBackFunc = "RedirectToPersonalHistory();";
                model = AutoMapper.Mapper.Map(dataGTPApplication, model);
                model.dataGTPApplicationRowVersion.CopyTo(dataGTPApplication.dataGTPApplicationRowVersion, 0);
                return Json(DbGTP.SingleUpdateMessage(null, null, null, callBackFunc));
            }
            catch (Exception ex)
            {
                return Json(DbGTP.ErrorMessage(ex.Message));
            }
        }

        [Route("GTP/PersonalHistoryForm/Create")]
        public ActionResult PersonalHistoryFormCreate()
        {
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications
                                                     where a.UserGUID == UserGUID
                                                     select a).FirstOrDefault();

            GTPPersonalHistoryFormUpdateModel model = new GTPPersonalHistoryFormUpdateModel();
            model.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            model.Active = true;
            model.dataPersonalHistoryGeneralInfoUpdateModel = new dataPersonalHistoryGeneralInfoUpdateModel();
            model.dataPersonalHistoryGeneralInfoUpdateModel.GTPPHGeneralInfoGUID = Guid.Empty;
            return View("~/Areas/GTP/Views/Applications/PersonalHistory.cshtml", model);
        }

        [Route("GTP/PersonalHistoryForm/Update")]
        public ActionResult PersonalHistoryFormUpdate()
        {
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications
                                                     where a.UserGUID == UserGUID
                                                     select a).FirstOrDefault();

            GTPPersonalHistoryFormUpdateModel model = new GTPPersonalHistoryFormUpdateModel();
            model.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            model.Active = true;

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

            dataPersonalHistoryConfirmationAndConsent dataPersonalHistoryConfirmationAndConsent = dataGTPApplication.dataPersonalHistoryConfirmationAndConsents.FirstOrDefault();
            model.dataPersonalHistoryConfirmationAndConsentUpdateModel = Mapper.Map(dataPersonalHistoryConfirmationAndConsent, new dataPersonalHistoryConfirmationAndConsentUpdateModel());

            return View("~/Areas/GTP/Views/Applications/PersonalHistory.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult PersonalHistoryFormCreate(GTPPersonalHistoryFormUpdateModel model)
        {
            dataPersonalHistoryGeneralInfo dataPersonalHistoryGeneralInfo = new dataPersonalHistoryGeneralInfo();
            dataPersonalHistoryPersonalInfo dataPersonalHistoryPersonalInfo = new dataPersonalHistoryPersonalInfo();
            dataPersonalHistoryContactInfo dataPersonalHistoryContactInfo = new dataPersonalHistoryContactInfo();
            dataPersonalHistoryPhoneNumber dataPersonalHistoryPhoneNumber = new dataPersonalHistoryPhoneNumber();
            dataPersonalHistoryEmailAddress dataPersonalHistoryEmailAddress = new dataPersonalHistoryEmailAddress();
            dataPersonalHistoryNationalityInfo dataPersonalHistoryNationalityInfo = new dataPersonalHistoryNationalityInfo();
            dataPersonalHistoryLetterOfInterest dataPersonalHistoryLetterOfInterest = new dataPersonalHistoryLetterOfInterest();
            List<dataPersonalHistoryWorkExperience> dataPersonalHistoryWorkExperiences = new List<dataPersonalHistoryWorkExperience>();
            List<dataPersonalHistorySpecializedTraining> dataPersonalHistorySpecializedTrainings = new List<dataPersonalHistorySpecializedTraining>();
            List<dataPersonalHistoryEducation> dataPersonalHistoryEducations = new List<dataPersonalHistoryEducation>();
            List<dataPersonalHistorySkill> dataPersonalHistorySkills = new List<dataPersonalHistorySkill>();
            dataPersonalHistoryLanguage dataPersonalHistoryLanguage = new dataPersonalHistoryLanguage();
            List<dataPersonalHistoryLicenceAndCertificate> dataPersonalHistoryLicenceAndCertificates = new List<dataPersonalHistoryLicenceAndCertificate>();
            dataPersonalHistoryProfessionalReference dataPersonalHistoryProfessionalReference = new dataPersonalHistoryProfessionalReference();
            dataPersonalHistoryQuestionnaire dataPersonalHistoryQuestionnaire = new dataPersonalHistoryQuestionnaire();
            List<dataPersonalHistoryRelative> dataPersonalHistoryRelatives = new List<dataPersonalHistoryRelative>();
            List<dataPersonalHistoryRelativeWorker> dataPersonalHistoryRelativeWorkers = new List<dataPersonalHistoryRelativeWorker>();
            dataPersonalHistoryConfirmationAndConsent dataPersonalHistoryConfirmationAndConsent = new dataPersonalHistoryConfirmationAndConsent();


            dataPersonalHistoryGeneralInfo = Mapper.Map(model.dataPersonalHistoryGeneralInfoUpdateModel, new dataPersonalHistoryGeneralInfo());
            dataPersonalHistoryPersonalInfo = Mapper.Map(model.dataPersonalHistoryPersonalInfoUpdateModel, new dataPersonalHistoryPersonalInfo());
            dataPersonalHistoryContactInfo = Mapper.Map(model.dataPersonalHistoryContactInfoUpdateModel, new dataPersonalHistoryContactInfo());
            dataPersonalHistoryPhoneNumber = Mapper.Map(model.dataPersonalHistoryPhoneNumberUpdateModel, new dataPersonalHistoryPhoneNumber());
            dataPersonalHistoryEmailAddress = Mapper.Map(model.dataPersonalHistoryEmailAddressUpdateModel, new dataPersonalHistoryEmailAddress());
            dataPersonalHistoryNationalityInfo = Mapper.Map(model.dataPersonalHistoryNationalityInfoUpdateModel, new dataPersonalHistoryNationalityInfo());
            dataPersonalHistoryLetterOfInterest = Mapper.Map(model.dataPersonalHistoryLetterOfInterestUpdateModel, new dataPersonalHistoryLetterOfInterest());
            //List<dataPersonalHistoryWorkExperience> dataPersonalHistoryWorkExperiences = new List<dataPersonalHistoryWorkExperience>();
            //List<dataPersonalHistorySpecializedTraining> dataPersonalHistorySpecializedTrainings = new List<dataPersonalHistorySpecializedTraining>();
            //List<dataPersonalHistoryEducation> dataPersonalHistoryEducations = new List<dataPersonalHistoryEducation>();
            //List<dataPersonalHistorySkill> dataPersonalHistorySkills = new List<dataPersonalHistorySkill>();
            dataPersonalHistoryLanguage = Mapper.Map(model.dataPersonalHistoryLanguageUpdateModel, new dataPersonalHistoryLanguage());
            //List<dataPersonalHistoryLicenceAndCertificate> dataPersonalHistoryLicenceAndCertificates = new List<dataPersonalHistoryLicenceAndCertificate>();
            dataPersonalHistoryProfessionalReference = Mapper.Map(model.dataPersonalHistoryProfessionalReferenceUpdateModel, new dataPersonalHistoryProfessionalReference());
            dataPersonalHistoryQuestionnaire = Mapper.Map(model.dataPersonalHistoryQuestionnaireUpdateModel, new dataPersonalHistoryQuestionnaire());
            //List<dataPersonalHistoryRelative> dataPersonalHistoryRelatives = new List<dataPersonalHistoryRelative>();
            //List<dataPersonalHistoryRelativeWorker> dataPersonalHistoryRelativeWorkers = new List<dataPersonalHistoryRelativeWorker>();
            dataPersonalHistoryConfirmationAndConsent = Mapper.Map(model.dataPersonalHistoryConfirmationAndConsentUpdateModel, new dataPersonalHistoryConfirmationAndConsent());


            dataPersonalHistoryGeneralInfo.GTPPHGeneralInfoGUID = Guid.NewGuid();
            dataPersonalHistoryGeneralInfo.GTPApplicationGUID = model.GTPApplicationGUID;
            dataPersonalHistoryGeneralInfo.Active = true;
            DbGTP.dataPersonalHistoryGeneralInfoes.Add(dataPersonalHistoryGeneralInfo);

            dataPersonalHistoryPersonalInfo.GTPPHPersonalInfoGUID = Guid.NewGuid();
            dataPersonalHistoryPersonalInfo.GTPApplicationGUID = model.GTPApplicationGUID;
            dataPersonalHistoryPersonalInfo.Active = true;
            DbGTP.dataPersonalHistoryPersonalInfoes.Add(dataPersonalHistoryPersonalInfo);

            dataPersonalHistoryContactInfo.GTPPHContactInfoGUID = Guid.NewGuid();
            dataPersonalHistoryContactInfo.GTPApplicationGUID = model.GTPApplicationGUID;
            dataPersonalHistoryContactInfo.Active = true;
            DbGTP.dataPersonalHistoryContactInfoes.Add(dataPersonalHistoryContactInfo);

            dataPersonalHistoryPhoneNumber.GTPPHPhoneNumberGUID = Guid.NewGuid();
            dataPersonalHistoryPhoneNumber.GTPApplicationGUID = model.GTPApplicationGUID;
            dataPersonalHistoryPhoneNumber.Active = true;
            DbGTP.dataPersonalHistoryPhoneNumbers.Add(dataPersonalHistoryPhoneNumber);

            dataPersonalHistoryEmailAddress.GTPPHEmailAddressGUID = Guid.NewGuid();
            dataPersonalHistoryEmailAddress.GTPApplicationGUID = model.GTPApplicationGUID;
            dataPersonalHistoryEmailAddress.Active = true;
            DbGTP.dataPersonalHistoryEmailAddresses.Add(dataPersonalHistoryEmailAddress);

            dataPersonalHistoryNationalityInfo.GTPPHNationalityInfoGUID = Guid.NewGuid();
            dataPersonalHistoryNationalityInfo.GTPApplicationGUID = model.GTPApplicationGUID;
            dataPersonalHistoryNationalityInfo.Active = true;
            dataPersonalHistoryNationalityInfo.NationalityAtBirth = string.Join(",", model.dataPersonalHistoryNationalityInfoUpdateModel.NationalityAtBirth);
            dataPersonalHistoryNationalityInfo.CurrentNationality = string.Join(",", model.dataPersonalHistoryNationalityInfoUpdateModel.CurrentNationality);
            DbGTP.dataPersonalHistoryNationalityInfoes.Add(dataPersonalHistoryNationalityInfo);

            dataPersonalHistoryLetterOfInterest.GTPPHLetterOfInterestGUID = Guid.NewGuid();
            dataPersonalHistoryLetterOfInterest.GTPApplicationGUID = model.GTPApplicationGUID;
            dataPersonalHistoryLetterOfInterest.Active = true;
            DbGTP.dataPersonalHistoryLetterOfInterests.Add(dataPersonalHistoryLetterOfInterest);

            dataPersonalHistoryLanguage.GTPPHLanguageGUID = Guid.NewGuid();
            dataPersonalHistoryLanguage.GTPApplicationGUID = model.GTPApplicationGUID;
            dataPersonalHistoryLanguage.Active = true;
            DbGTP.dataPersonalHistoryLanguages.Add(dataPersonalHistoryLanguage);

            dataPersonalHistoryProfessionalReference.GTPPHProfReferenceGUID = Guid.NewGuid();
            dataPersonalHistoryProfessionalReference.GTPApplicationGUID = model.GTPApplicationGUID;
            dataPersonalHistoryProfessionalReference.Active = true;
            DbGTP.dataPersonalHistoryProfessionalReferences.Add(dataPersonalHistoryProfessionalReference);

            dataPersonalHistoryQuestionnaire.GTPPHQuestionnaireGUID = Guid.NewGuid();
            dataPersonalHistoryQuestionnaire.GTPApplicationGUID = model.GTPApplicationGUID;
            dataPersonalHistoryQuestionnaire.Active = true;
            DbGTP.dataPersonalHistoryQuestionnaires.Add(dataPersonalHistoryQuestionnaire);

            dataPersonalHistoryConfirmationAndConsent.GTPPHConfirmationConsentGUID = Guid.NewGuid();
            dataPersonalHistoryConfirmationAndConsent.GTPApplicationGUID = model.GTPApplicationGUID;
            dataPersonalHistoryConfirmationAndConsent.Active = true;
            dataPersonalHistoryConfirmationAndConsent.ConfirmationDate = DateTime.Now;
            DbGTP.dataPersonalHistoryConfirmationAndConsents.Add(dataPersonalHistoryConfirmationAndConsent);

            dataPersonalHistoryWorkExperiences = new List<dataPersonalHistoryWorkExperience>();
            foreach (var item in model.dataPersonalHistoryWorkExperienceUpdateModel)
            {
                dataPersonalHistoryWorkExperience dataPersonalHistoryWorkExperience = new dataPersonalHistoryWorkExperience();
                dataPersonalHistoryWorkExperience = Mapper.Map(item, new dataPersonalHistoryWorkExperience());
                dataPersonalHistoryWorkExperience.GTPPHWorkExperienceGUID = Guid.NewGuid();
                dataPersonalHistoryWorkExperience.GTPApplicationGUID = model.GTPApplicationGUID;
                dataPersonalHistoryWorkExperience.Active = true;
                dataPersonalHistoryWorkExperiences.Add(dataPersonalHistoryWorkExperience);
            }
            if (dataPersonalHistoryWorkExperiences.Count > 0)
            {
                DbGTP.dataPersonalHistoryWorkExperiences.AddRange(dataPersonalHistoryWorkExperiences);
            }


            dataPersonalHistorySpecializedTrainings = new List<dataPersonalHistorySpecializedTraining>();
            foreach (var item in model.dataPersonalHistorySpecializedTrainingUpdateModel)
            {
                try
                {
                    if (String.IsNullOrWhiteSpace(item.CourseTitle.Trim()))
                    {
                        continue;
                    }
                }
                catch
                {
                    continue;
                }
                dataPersonalHistorySpecializedTraining dataPersonalHistorySpecializedTraining = new dataPersonalHistorySpecializedTraining();
                dataPersonalHistorySpecializedTraining = Mapper.Map(item, new dataPersonalHistorySpecializedTraining());
                dataPersonalHistorySpecializedTraining.GTPPHSpecializedTrainingGUID = Guid.NewGuid();
                dataPersonalHistorySpecializedTraining.GTPApplicationGUID = model.GTPApplicationGUID;
                dataPersonalHistorySpecializedTraining.Active = true;
                dataPersonalHistorySpecializedTrainings.Add(dataPersonalHistorySpecializedTraining);
            }
            if (dataPersonalHistorySpecializedTrainings.Count > 0)
            {
                DbGTP.dataPersonalHistorySpecializedTrainings.AddRange(dataPersonalHistorySpecializedTrainings);
            }

            dataPersonalHistoryEducations = new List<dataPersonalHistoryEducation>();
            foreach (var item in model.dataPersonalHistoryEducationUpdateModel)
            {
                dataPersonalHistoryEducation dataPersonalHistoryEducation = new dataPersonalHistoryEducation();
                dataPersonalHistoryEducation = Mapper.Map(item, new dataPersonalHistoryEducation());
                dataPersonalHistoryEducation.GTPPHEducationGUID = Guid.NewGuid();
                dataPersonalHistoryEducation.GTPApplicationGUID = model.GTPApplicationGUID;
                dataPersonalHistoryEducation.Active = true;
                dataPersonalHistoryEducations.Add(dataPersonalHistoryEducation);
            }
            if (dataPersonalHistoryEducations.Count > 0)
            {
                DbGTP.dataPersonalHistoryEducations.AddRange(dataPersonalHistoryEducations);
            }

            dataPersonalHistorySkills = new List<dataPersonalHistorySkill>();
            foreach (var item in model.dataPersonalHistorySkillUpdateModel)
            {
                try
                {
                    if (String.IsNullOrWhiteSpace(item.SkillDescription.Trim()))
                    {
                        continue;
                    }
                }
                catch
                {
                    continue;
                }

                dataPersonalHistorySkill dataPersonalHistorySkill = new dataPersonalHistorySkill();
                dataPersonalHistorySkill = Mapper.Map(item, new dataPersonalHistorySkill());
                dataPersonalHistorySkill.GTPPHSkillGUID = Guid.NewGuid();
                dataPersonalHistorySkill.GTPApplicationGUID = model.GTPApplicationGUID;
                dataPersonalHistorySkill.Active = true;
                dataPersonalHistorySkills.Add(dataPersonalHistorySkill);
            }
            if (dataPersonalHistorySkills.Count > 0)
            {
                DbGTP.dataPersonalHistorySkills.AddRange(dataPersonalHistorySkills);
            }

            dataPersonalHistoryLicenceAndCertificates = new List<dataPersonalHistoryLicenceAndCertificate>();
            foreach (var item in model.dataPersonalHistoryLicenceAndCertificateUpdateModel)
            {
                try
                {
                    if (String.IsNullOrWhiteSpace(item.LicenceDescription.Trim()))
                    {
                        continue;
                    }
                }
                catch
                {
                    continue;
                }

                dataPersonalHistoryLicenceAndCertificate dataPersonalHistoryLicenceAndCertificate = new dataPersonalHistoryLicenceAndCertificate();
                dataPersonalHistoryLicenceAndCertificate = Mapper.Map(item, new dataPersonalHistoryLicenceAndCertificate());
                dataPersonalHistoryLicenceAndCertificate.GTPPHLicenceGUID = Guid.NewGuid();
                dataPersonalHistoryLicenceAndCertificate.GTPApplicationGUID = model.GTPApplicationGUID;
                dataPersonalHistoryLicenceAndCertificate.Active = true;
                dataPersonalHistoryLicenceAndCertificates.Add(dataPersonalHistoryLicenceAndCertificate);

            }
            if (dataPersonalHistoryLicenceAndCertificates.Count > 0)
            {
                DbGTP.dataPersonalHistoryLicenceAndCertificates.AddRange(dataPersonalHistoryLicenceAndCertificates);
            }

            dataPersonalHistoryRelatives = new List<dataPersonalHistoryRelative>();
            foreach (var item in model.dataPersonalHistoryRelativeUpdateModel)
            {
                try
                {
                    if (String.IsNullOrWhiteSpace(item.RelativeName.Trim()))
                    {
                        continue;
                    }
                }
                catch
                {
                    continue;
                }

                dataPersonalHistoryRelative dataPersonalHistoryRelative = new dataPersonalHistoryRelative();
                dataPersonalHistoryRelative = Mapper.Map(item, new dataPersonalHistoryRelative());
                dataPersonalHistoryRelative.GTPPHRelativeGUID = Guid.NewGuid();
                dataPersonalHistoryRelative.GTPApplicationGUID = model.GTPApplicationGUID;
                dataPersonalHistoryRelative.Active = true;
                dataPersonalHistoryRelatives.Add(dataPersonalHistoryRelative);
            }
            if (dataPersonalHistoryRelatives.Count > 0)
            {
                DbGTP.dataPersonalHistoryRelatives.AddRange(dataPersonalHistoryRelatives);
            }

            dataPersonalHistoryRelativeWorkers = new List<dataPersonalHistoryRelativeWorker>();
            foreach (var item in model.dataPersonalHistoryRelativeWorkerUpdateModel)
            {
                try
                {
                    if (String.IsNullOrWhiteSpace(item.RelativeName.Trim()))
                    {
                        continue;
                    }
                }
                catch
                {
                    continue;
                }

                dataPersonalHistoryRelativeWorker dataPersonalHistoryRelativeWorker = new dataPersonalHistoryRelativeWorker();
                dataPersonalHistoryRelativeWorker = Mapper.Map(item, new dataPersonalHistoryRelativeWorker());
                dataPersonalHistoryRelativeWorker.GTPPHRelativeWorkerGUID = Guid.NewGuid();
                dataPersonalHistoryRelativeWorker.GTPApplicationGUID = model.GTPApplicationGUID;
                dataPersonalHistoryRelativeWorker.Active = true;
                dataPersonalHistoryRelativeWorkers.Add(dataPersonalHistoryRelativeWorker);
            }
            if (dataPersonalHistoryRelativeWorkers.Count > 0)
            {
                DbGTP.dataPersonalHistoryRelativeWorkers.AddRange(dataPersonalHistoryRelativeWorkers);
            }

            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.GTPApplicationGUID == model.GTPApplicationGUID select a).FirstOrDefault();
            dataGTPApplication.IsReviewed = false;
            dataGTPApplication.UpdatedOn = DateTime.Now;

            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();

                string emailAddress = (from a in DbCMS.userServiceHistory where a.UserGUID == UserGUID && a.Active select a.EmailAddress).FirstOrDefault();
                string fullName = (from a in DbCMS.userPersonalDetailsLanguage where a.UserGUID == UserGUID && a.Active && a.LanguageID == "EN" select a.FirstName + " " + a.Surname).FirstOrDefault();
                string preferredEamilAddress = "";
                if (dataPersonalHistoryEmailAddress.PreferedEmailAddress == 1)
                {
                    preferredEamilAddress = dataPersonalHistoryEmailAddress.HomeEmailAddress;
                }
                else
                {
                    preferredEamilAddress = dataPersonalHistoryEmailAddress.BusinessEmailAddress;
                }

                new Email().SendGTPConfirmReceivingApplicationEmail(preferredEamilAddress, emailAddress, fullName, "Email confirmation of receiving the application");

                return Json(DbGTP.SingleCreateMessage("window.location.href = '/GTP/PersonalHistoryForm/Update'", true));
            }
            catch (Exception ex)
            {
                return Json(DbGTP.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult PersonalHistoryFormUpdate(GTPPersonalHistoryFormUpdateModel model)
        {
            DateTime ExecutionTime = DateTime.Now;

            dataPersonalHistoryGeneralInfo dataPersonalHistoryGeneralInfo = new dataPersonalHistoryGeneralInfo();
            dataPersonalHistoryPersonalInfo dataPersonalHistoryPersonalInfo = new dataPersonalHistoryPersonalInfo();
            dataPersonalHistoryContactInfo dataPersonalHistoryContactInfo = new dataPersonalHistoryContactInfo();
            dataPersonalHistoryPhoneNumber dataPersonalHistoryPhoneNumber = new dataPersonalHistoryPhoneNumber();
            dataPersonalHistoryEmailAddress dataPersonalHistoryEmailAddress = new dataPersonalHistoryEmailAddress();
            dataPersonalHistoryNationalityInfo dataPersonalHistoryNationalityInfo = new dataPersonalHistoryNationalityInfo();
            dataPersonalHistoryLetterOfInterest dataPersonalHistoryLetterOfInterest = new dataPersonalHistoryLetterOfInterest();
            dataPersonalHistoryLanguage dataPersonalHistoryLanguage = new dataPersonalHistoryLanguage();
            dataPersonalHistoryProfessionalReference dataPersonalHistoryProfessionalReference = new dataPersonalHistoryProfessionalReference();
            dataPersonalHistoryQuestionnaire dataPersonalHistoryQuestionnaire = new dataPersonalHistoryQuestionnaire();
            dataPersonalHistoryConfirmationAndConsent dataPersonalHistoryConfirmationAndConsent = new dataPersonalHistoryConfirmationAndConsent();

            //List<dataPersonalHistoryWorkExperience> dataPersonalHistoryWorkExperiences = new List<dataPersonalHistoryWorkExperience>();
            //List<dataPersonalHistorySpecializedTraining> dataPersonalHistorySpecializedTrainings = new List<dataPersonalHistorySpecializedTraining>();
            //List<dataPersonalHistoryEducation> dataPersonalHistoryEducations = new List<dataPersonalHistoryEducation>();
            //List<dataPersonalHistorySkill> dataPersonalHistorySkills = new List<dataPersonalHistorySkill>();
            //List<dataPersonalHistoryLicenceAndCertificate> dataPersonalHistoryLicenceAndCertificates = new List<dataPersonalHistoryLicenceAndCertificate>();
            //List<dataPersonalHistoryRelative> dataPersonalHistoryRelatives = new List<dataPersonalHistoryRelative>();
            //List<dataPersonalHistoryRelativeWorker> dataPersonalHistoryRelativeWorkers = new List<dataPersonalHistoryRelativeWorker>();




            dataPersonalHistoryGeneralInfo = Mapper.Map(model.dataPersonalHistoryGeneralInfoUpdateModel, new dataPersonalHistoryGeneralInfo());
            dataPersonalHistoryGeneralInfo.GTPApplicationGUID = model.GTPApplicationGUID;
            dataPersonalHistoryGeneralInfo.Active = model.Active;
            DbGTP.Update(dataPersonalHistoryGeneralInfo, Permissions.Portalforgrouptwoapplications.UpdateGuid, ExecutionTime, DbCMS);



            dataPersonalHistoryPersonalInfo = Mapper.Map(model.dataPersonalHistoryPersonalInfoUpdateModel, new dataPersonalHistoryPersonalInfo());
            dataPersonalHistoryPersonalInfo.GTPApplicationGUID = model.GTPApplicationGUID;
            dataPersonalHistoryPersonalInfo.Active = model.Active;
            DbGTP.Update(dataPersonalHistoryPersonalInfo, Permissions.Portalforgrouptwoapplications.UpdateGuid, ExecutionTime, DbCMS);


            dataPersonalHistoryContactInfo = Mapper.Map(model.dataPersonalHistoryContactInfoUpdateModel, new dataPersonalHistoryContactInfo());
            dataPersonalHistoryContactInfo.GTPApplicationGUID = model.GTPApplicationGUID;
            dataPersonalHistoryContactInfo.Active = model.Active;
            DbGTP.Update(dataPersonalHistoryContactInfo, Permissions.Portalforgrouptwoapplications.UpdateGuid, ExecutionTime, DbCMS);


            dataPersonalHistoryPhoneNumber = Mapper.Map(model.dataPersonalHistoryPhoneNumberUpdateModel, new dataPersonalHistoryPhoneNumber());
            dataPersonalHistoryPhoneNumber.GTPApplicationGUID = model.GTPApplicationGUID;
            dataPersonalHistoryPhoneNumber.Active = model.Active;
            DbGTP.Update(dataPersonalHistoryPhoneNumber, Permissions.Portalforgrouptwoapplications.UpdateGuid, ExecutionTime, DbCMS);


            dataPersonalHistoryEmailAddress = Mapper.Map(model.dataPersonalHistoryEmailAddressUpdateModel, new dataPersonalHistoryEmailAddress());
            dataPersonalHistoryEmailAddress.GTPApplicationGUID = model.GTPApplicationGUID;
            dataPersonalHistoryEmailAddress.Active = model.Active;
            DbGTP.Update(dataPersonalHistoryEmailAddress, Permissions.Portalforgrouptwoapplications.UpdateGuid, ExecutionTime, DbCMS);

            dataPersonalHistoryNationalityInfo = Mapper.Map(model.dataPersonalHistoryNationalityInfoUpdateModel, new dataPersonalHistoryNationalityInfo());
            dataPersonalHistoryNationalityInfo.GTPApplicationGUID = model.GTPApplicationGUID;
            dataPersonalHistoryNationalityInfo.Active = model.Active;
            dataPersonalHistoryNationalityInfo.NationalityAtBirth = string.Join(",", model.dataPersonalHistoryNationalityInfoUpdateModel.NationalityAtBirth);
            dataPersonalHistoryNationalityInfo.CurrentNationality = string.Join(",", model.dataPersonalHistoryNationalityInfoUpdateModel.CurrentNationality);
            DbGTP.Update(dataPersonalHistoryNationalityInfo, Permissions.Portalforgrouptwoapplications.UpdateGuid, ExecutionTime, DbCMS);

            dataPersonalHistoryLetterOfInterest = Mapper.Map(model.dataPersonalHistoryLetterOfInterestUpdateModel, new dataPersonalHistoryLetterOfInterest());
            dataPersonalHistoryLetterOfInterest.GTPApplicationGUID = model.GTPApplicationGUID;
            dataPersonalHistoryLetterOfInterest.Active = model.Active;
            DbGTP.Update(dataPersonalHistoryLetterOfInterest, Permissions.Portalforgrouptwoapplications.UpdateGuid, ExecutionTime, DbCMS);


            dataPersonalHistoryLanguage = Mapper.Map(model.dataPersonalHistoryLanguageUpdateModel, new dataPersonalHistoryLanguage());
            dataPersonalHistoryLanguage.GTPApplicationGUID = model.GTPApplicationGUID;
            dataPersonalHistoryLanguage.Active = model.Active;
            DbGTP.Update(dataPersonalHistoryLanguage, Permissions.Portalforgrouptwoapplications.UpdateGuid, ExecutionTime, DbCMS);

            dataPersonalHistoryProfessionalReference = Mapper.Map(model.dataPersonalHistoryProfessionalReferenceUpdateModel, new dataPersonalHistoryProfessionalReference());
            dataPersonalHistoryProfessionalReference.GTPApplicationGUID = model.GTPApplicationGUID;
            dataPersonalHistoryProfessionalReference.Active = model.Active;
            DbGTP.Update(dataPersonalHistoryProfessionalReference, Permissions.Portalforgrouptwoapplications.UpdateGuid, ExecutionTime, DbCMS);


            dataPersonalHistoryQuestionnaire = Mapper.Map(model.dataPersonalHistoryQuestionnaireUpdateModel, new dataPersonalHistoryQuestionnaire());
            dataPersonalHistoryQuestionnaire.GTPApplicationGUID = model.GTPApplicationGUID;
            dataPersonalHistoryQuestionnaire.Active = model.Active;
            DbGTP.Update(dataPersonalHistoryQuestionnaire, Permissions.Portalforgrouptwoapplications.UpdateGuid, ExecutionTime, DbCMS);

            dataPersonalHistoryConfirmationAndConsent = Mapper.Map(model.dataPersonalHistoryConfirmationAndConsentUpdateModel, new dataPersonalHistoryConfirmationAndConsent());
            dataPersonalHistoryConfirmationAndConsent.GTPApplicationGUID = model.GTPApplicationGUID;
            dataPersonalHistoryConfirmationAndConsent.Active = model.Active;
            DbGTP.Update(dataPersonalHistoryConfirmationAndConsent, Permissions.Portalforgrouptwoapplications.UpdateGuid, ExecutionTime, DbCMS);



            //List<dataPersonalHistoryWorkExperience> dataPersonalHistoryWorkExperiences = new List<dataPersonalHistoryWorkExperience>();
            foreach (var record in model.dataPersonalHistoryWorkExperienceUpdateModel)
            {
                dataPersonalHistoryWorkExperience dataPersonalHistoryWorkExperience = new dataPersonalHistoryWorkExperience();
                dataPersonalHistoryWorkExperience = Mapper.Map(record, new dataPersonalHistoryWorkExperience());
                dataPersonalHistoryWorkExperience.GTPApplicationGUID = model.GTPApplicationGUID;
                dataPersonalHistoryWorkExperience.Active = model.Active;
                //update
                if (record.GTPPHWorkExperienceGUID != Guid.Empty)
                {
                    DbGTP.Update(dataPersonalHistoryWorkExperience, Permissions.Portalforgrouptwoapplications.UpdateGuid, ExecutionTime, DbCMS);
                }
                //create
                else
                {
                    dataPersonalHistoryWorkExperience.GTPPHWorkExperienceGUID = Guid.NewGuid();
                    dataPersonalHistoryWorkExperience.GTPApplicationGUID = model.GTPApplicationGUID;
                    dataPersonalHistoryWorkExperience.Active = true;
                    DbGTP.Create(dataPersonalHistoryWorkExperience, Permissions.Portalforgrouptwoapplications.UpdateGuid, ExecutionTime, DbCMS);
                }
            }
            List<Guid> workExperiencesGUIDs = model.dataPersonalHistoryWorkExperienceUpdateModel.Select(x => x.GTPPHWorkExperienceGUID).ToList();
            var toDeleteWorkExperience = (from a in DbGTP.dataPersonalHistoryWorkExperiences
                                          where !workExperiencesGUIDs.Contains(a.GTPPHWorkExperienceGUID) && a.GTPApplicationGUID == model.GTPApplicationGUID
                                          select a).ToList();
            if (toDeleteWorkExperience.Count > 0)
            {
                DbGTP.Delete(toDeleteWorkExperience, ExecutionTime, Permissions.Portalforgrouptwoapplications.UpdateGuid, DbCMS);
            }



            //List<dataPersonalHistorySpecializedTraining> dataPersonalHistorySpecializedTrainings = new List<dataPersonalHistorySpecializedTraining>();
            foreach (var record in model.dataPersonalHistorySpecializedTrainingUpdateModel)
            {
                dataPersonalHistorySpecializedTraining dataPersonalHistorySpecializedTraining = new dataPersonalHistorySpecializedTraining();
                dataPersonalHistorySpecializedTraining = Mapper.Map(record, new dataPersonalHistorySpecializedTraining());
                dataPersonalHistorySpecializedTraining.GTPApplicationGUID = model.GTPApplicationGUID;
                dataPersonalHistorySpecializedTraining.Active = model.Active;
                //update
                if (record.GTPPHSpecializedTrainingGUID != Guid.Empty)
                {
                    DbGTP.Update(dataPersonalHistorySpecializedTraining, Permissions.Portalforgrouptwoapplications.UpdateGuid, ExecutionTime, DbCMS);
                }
                //create
                else
                {
                    dataPersonalHistorySpecializedTraining.GTPPHSpecializedTrainingGUID = Guid.NewGuid();
                    dataPersonalHistorySpecializedTraining.GTPApplicationGUID = model.GTPApplicationGUID;
                    dataPersonalHistorySpecializedTraining.Active = true;
                    DbGTP.Create(dataPersonalHistorySpecializedTraining, Permissions.Portalforgrouptwoapplications.UpdateGuid, ExecutionTime, DbCMS);
                }
            }
            List<Guid> trainingsGUIDs = model.dataPersonalHistorySpecializedTrainingUpdateModel.Select(x => x.GTPPHSpecializedTrainingGUID).ToList();
            var toDeleteTrainings = (from a in DbGTP.dataPersonalHistorySpecializedTrainings
                                     where !trainingsGUIDs.Contains(a.GTPPHSpecializedTrainingGUID) && a.GTPApplicationGUID == model.GTPApplicationGUID
                                     select a).ToList();
            if (toDeleteTrainings.Count > 0)
            {
                DbGTP.Delete(toDeleteTrainings, ExecutionTime, Permissions.Portalforgrouptwoapplications.UpdateGuid, DbCMS);
            }


            //List<dataPersonalHistoryEducation> dataPersonalHistoryEducations = new List<dataPersonalHistoryEducation>();
            foreach (var record in model.dataPersonalHistoryEducationUpdateModel)
            {
                dataPersonalHistoryEducation dataPersonalHistoryEducation = new dataPersonalHistoryEducation();
                dataPersonalHistoryEducation = Mapper.Map(record, new dataPersonalHistoryEducation());
                dataPersonalHistoryEducation.GTPApplicationGUID = model.GTPApplicationGUID;
                dataPersonalHistoryEducation.Active = model.Active;
                //update
                if (record.GTPPHEducationGUID != Guid.Empty)
                {
                    DbGTP.Update(dataPersonalHistoryEducation, Permissions.Portalforgrouptwoapplications.UpdateGuid, ExecutionTime, DbCMS);
                }
                //create
                else
                {
                    dataPersonalHistoryEducation.GTPPHEducationGUID = Guid.NewGuid();
                    dataPersonalHistoryEducation.GTPApplicationGUID = model.GTPApplicationGUID;
                    dataPersonalHistoryEducation.Active = true;
                    DbGTP.Create(dataPersonalHistoryEducation, Permissions.Portalforgrouptwoapplications.UpdateGuid, ExecutionTime, DbCMS);
                }
            }
            List<Guid> educationGUIDs = model.dataPersonalHistoryEducationUpdateModel.Select(x => x.GTPPHEducationGUID).ToList();
            var toDeleteEducations = (from a in DbGTP.dataPersonalHistoryEducations
                                      where !educationGUIDs.Contains(a.GTPPHEducationGUID) && a.GTPApplicationGUID == model.GTPApplicationGUID
                                      select a).ToList();
            if (toDeleteEducations.Count > 0)
            {
                DbGTP.Delete(toDeleteEducations, ExecutionTime, Permissions.Portalforgrouptwoapplications.UpdateGuid, DbCMS);
            }



            //List<dataPersonalHistorySkill> dataPersonalHistorySkills = new List<dataPersonalHistorySkill>();
            foreach (var record in model.dataPersonalHistorySkillUpdateModel)
            {
                dataPersonalHistorySkill dataPersonalHistorySkill = new dataPersonalHistorySkill();
                dataPersonalHistorySkill = Mapper.Map(record, new dataPersonalHistorySkill());
                dataPersonalHistorySkill.GTPApplicationGUID = model.GTPApplicationGUID;
                dataPersonalHistorySkill.Active = model.Active;
                //update
                if (record.GTPPHSkillGUID != Guid.Empty)
                {
                    DbGTP.Update(dataPersonalHistorySkill, Permissions.Portalforgrouptwoapplications.UpdateGuid, ExecutionTime, DbCMS);
                }
                //create
                else
                {
                    dataPersonalHistorySkill.GTPPHSkillGUID = Guid.NewGuid();
                    dataPersonalHistorySkill.GTPApplicationGUID = model.GTPApplicationGUID;
                    dataPersonalHistorySkill.Active = true;
                    DbGTP.Create(dataPersonalHistorySkill, Permissions.Portalforgrouptwoapplications.UpdateGuid, ExecutionTime, DbCMS);
                }
            }
            List<Guid> skillsGUIDs = model.dataPersonalHistorySkillUpdateModel.Select(x => x.GTPPHSkillGUID).ToList();
            var toDeleteSkills = (from a in DbGTP.dataPersonalHistorySkills
                                  where !skillsGUIDs.Contains(a.GTPPHSkillGUID) && a.GTPApplicationGUID == model.GTPApplicationGUID
                                  select a).ToList();
            if (toDeleteSkills.Count > 0)
            {
                DbGTP.Delete(toDeleteSkills, ExecutionTime, Permissions.Portalforgrouptwoapplications.UpdateGuid, DbCMS);
            }



            //List<dataPersonalHistoryLicenceAndCertificate> dataPersonalHistoryLicenceAndCertificates = new List<dataPersonalHistoryLicenceAndCertificate>();
            foreach (var record in model.dataPersonalHistoryLicenceAndCertificateUpdateModel)
            {
                dataPersonalHistoryLicenceAndCertificate dataPersonalHistoryLicenceAndCertificate = new dataPersonalHistoryLicenceAndCertificate();
                dataPersonalHistoryLicenceAndCertificate = Mapper.Map(record, new dataPersonalHistoryLicenceAndCertificate());
                dataPersonalHistoryLicenceAndCertificate.GTPApplicationGUID = model.GTPApplicationGUID;
                dataPersonalHistoryLicenceAndCertificate.Active = model.Active;
                //update
                if (record.GTPPHLicenceGUID != Guid.Empty)
                {
                    DbGTP.Update(dataPersonalHistoryLicenceAndCertificate, Permissions.Portalforgrouptwoapplications.UpdateGuid, ExecutionTime, DbCMS);
                }
                //create
                else
                {
                    dataPersonalHistoryLicenceAndCertificate.GTPPHLicenceGUID = Guid.NewGuid();
                    dataPersonalHistoryLicenceAndCertificate.GTPApplicationGUID = model.GTPApplicationGUID;
                    dataPersonalHistoryLicenceAndCertificate.Active = true;
                    DbGTP.Create(dataPersonalHistoryLicenceAndCertificate, Permissions.Portalforgrouptwoapplications.UpdateGuid, ExecutionTime, DbCMS);
                }
            }
            List<Guid> licencesGUIDs = model.dataPersonalHistoryLicenceAndCertificateUpdateModel.Select(x => x.GTPPHLicenceGUID).ToList();
            var toDeleteLicences = (from a in DbGTP.dataPersonalHistoryLicenceAndCertificates
                                    where !licencesGUIDs.Contains(a.GTPPHLicenceGUID) && a.GTPApplicationGUID == model.GTPApplicationGUID
                                    select a).ToList();
            if (toDeleteLicences.Count > 0)
            {
                DbGTP.Delete(toDeleteLicences, ExecutionTime, Permissions.Portalforgrouptwoapplications.UpdateGuid, DbCMS);
            }



            //List<dataPersonalHistoryRelative> dataPersonalHistoryRelatives = new List<dataPersonalHistoryRelative>();
            foreach (var record in model.dataPersonalHistoryRelativeUpdateModel)
            {
                dataPersonalHistoryRelative dataPersonalHistoryRelative = new dataPersonalHistoryRelative();
                dataPersonalHistoryRelative = Mapper.Map(record, new dataPersonalHistoryRelative());
                dataPersonalHistoryRelative.GTPApplicationGUID = model.GTPApplicationGUID;
                dataPersonalHistoryRelative.Active = model.Active;
                //update
                if (record.GTPPHRelativeGUID != Guid.Empty)
                {
                    DbGTP.Update(dataPersonalHistoryRelative, Permissions.Portalforgrouptwoapplications.UpdateGuid, ExecutionTime, DbCMS);
                }
                //create
                else
                {
                    dataPersonalHistoryRelative.GTPPHRelativeGUID = Guid.NewGuid();
                    dataPersonalHistoryRelative.GTPApplicationGUID = model.GTPApplicationGUID;
                    dataPersonalHistoryRelative.Active = true;
                    DbGTP.Create(dataPersonalHistoryRelative, Permissions.Portalforgrouptwoapplications.UpdateGuid, ExecutionTime, DbCMS);
                }
            }
            List<Guid> relativesGUIDs = model.dataPersonalHistoryRelativeUpdateModel.Select(x => x.GTPPHRelativeGUID).ToList();
            var toDeleteRelatives = (from a in DbGTP.dataPersonalHistoryRelatives
                                     where !relativesGUIDs.Contains(a.GTPPHRelativeGUID) && a.GTPApplicationGUID == model.GTPApplicationGUID
                                     select a).ToList();
            if (toDeleteRelatives.Count > 0)
            {
                DbGTP.Delete(toDeleteRelatives, ExecutionTime, Permissions.Portalforgrouptwoapplications.UpdateGuid, DbCMS);
            }



            //List<dataPersonalHistoryRelativeWorker> dataPersonalHistoryRelativeWorkers = new List<dataPersonalHistoryRelativeWorker>();
            foreach (var record in model.dataPersonalHistoryRelativeWorkerUpdateModel)
            {
                dataPersonalHistoryRelativeWorker dataPersonalHistoryRelativeWorker = new dataPersonalHistoryRelativeWorker();
                dataPersonalHistoryRelativeWorker = Mapper.Map(record, new dataPersonalHistoryRelativeWorker());
                dataPersonalHistoryRelativeWorker.GTPApplicationGUID = model.GTPApplicationGUID;
                dataPersonalHistoryRelativeWorker.Active = model.Active;
                //update
                if (record.GTPPHRelativeWorkerGUID != Guid.Empty)
                {
                    DbGTP.Update(dataPersonalHistoryRelativeWorker, Permissions.Portalforgrouptwoapplications.UpdateGuid, ExecutionTime, DbCMS);
                }
                //create
                else
                {
                    dataPersonalHistoryRelativeWorker.GTPPHRelativeWorkerGUID = Guid.NewGuid();
                    dataPersonalHistoryRelativeWorker.GTPApplicationGUID = model.GTPApplicationGUID;
                    dataPersonalHistoryRelativeWorker.Active = true;
                    DbGTP.Create(dataPersonalHistoryRelativeWorker, Permissions.Portalforgrouptwoapplications.UpdateGuid, ExecutionTime, DbCMS);
                }
            }
            List<Guid> relativesWorkersGUIDs = model.dataPersonalHistoryRelativeWorkerUpdateModel.Select(x => x.GTPPHRelativeWorkerGUID).ToList();
            var toDeleteRelativesWorkers = (from a in DbGTP.dataPersonalHistoryRelativeWorkers
                                            where !relativesWorkersGUIDs.Contains(a.GTPPHRelativeWorkerGUID) && a.GTPApplicationGUID == model.GTPApplicationGUID
                                            select a).ToList();
            if (toDeleteRelativesWorkers.Count > 0)
            {
                DbGTP.Delete(toDeleteRelativesWorkers, ExecutionTime, Permissions.Portalforgrouptwoapplications.UpdateGuid, DbCMS);

            }

            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.GTPApplicationGUID == model.GTPApplicationGUID select a).FirstOrDefault();
            dataGTPApplication.IsReviewed = false;
            dataGTPApplication.UpdatedOn = DateTime.Now;

            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();



                return Json(DbGTP.SingleCreateMessage("window.location.href = '/GTP/PersonalHistoryForm/Update'", true));
            }
            catch (Exception ex)
            {
                return Json(DbGTP.ErrorMessage(ex.Message));
            }
        }
        #endregion

        [Route("GTP/PersonalHistoryFormWizard/Create")]
        public ActionResult PersonalHistoryFormWizardCreate()
        {
            //get list of completed sections to change icon on page
            PersonalHistoryFormWizardUpdateModel model = new PersonalHistoryFormWizardUpdateModel();
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            model.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            model.GeneralInfoCompleted = dataGTPApplication.dataPersonalHistoryGeneralInfoes.FirstOrDefault() == null ? false : true;
            model.PersonalInfoCompleted = dataGTPApplication.dataPersonalHistoryPersonalInfoes.FirstOrDefault() == null ? false : true;
            model.ContactInfoCompleted = dataGTPApplication.dataPersonalHistoryContactInfoes.FirstOrDefault() == null ? false : true;
            model.PhoneNumberCompleted = dataGTPApplication.dataPersonalHistoryPhoneNumbers.FirstOrDefault() == null ? false : true;
            model.EmailAddressCompleted = dataGTPApplication.dataPersonalHistoryEmailAddresses.FirstOrDefault() == null ? false : true;
            model.NationalityInfoCompleted = dataGTPApplication.dataPersonalHistoryNationalityInfoes.FirstOrDefault() == null ? false : true;
            model.LetterOfInteresCompleted = dataGTPApplication.dataPersonalHistoryLetterOfInterests.FirstOrDefault() == null ? false : true;
            model.WorkExperienceCompleted = dataGTPApplication.dataPersonalHistoryWorkExperiences.Count() > 0 ? true : false;
            model.SpecializedTrainingCompleted = dataGTPApplication.dataPersonalHistorySpecializedTrainings.Count() > 0 ? true : false;
            model.EducationsCompleted = dataGTPApplication.dataPersonalHistoryEducations.Count() > 0 ? true : false;
            model.SkillsCompleted = dataGTPApplication.dataPersonalHistorySkills.Count() > 0 ? true : false;
            model.LanguagesCompleted = dataGTPApplication.dataPersonalHistoryLanguages.FirstOrDefault() == null ? false : true;
            model.LicenceAndCertificateCompleted = dataGTPApplication.dataPersonalHistoryLicenceAndCertificates.FirstOrDefault() == null ? false : true;
            model.ProfessionalReferencesCompleted = dataGTPApplication.dataPersonalHistoryProfessionalReferences.FirstOrDefault() == null ? false : true;
            model.QuestionnaireCompleted = dataGTPApplication.dataPersonalHistoryQuestionnaires.FirstOrDefault() == null ? false : true;
            dataPersonalHistoryConfirmationAndConsent dataPersonalHistoryConfirmationAndConsent = dataGTPApplication.dataPersonalHistoryConfirmationAndConsents.FirstOrDefault();
            if (dataPersonalHistoryConfirmationAndConsent != null)
            {
                model.ConfirmationAndConsentCompleted = dataPersonalHistoryConfirmationAndConsent.IsConfirmedByUser;
            }
            else
            {
                model.ConfirmationAndConsentCompleted = false;
            }


            return View("~/Areas/GTP/Views/Applications/PersonalHistoryFormWizard/PersonalHistoryFormWizard.cshtml", model);
        }

        #region Section 1 General Information
        [Route("GTP/Applications/GeneralInformationWizard/Create")]
        public ActionResult GeneralInformationWizardCreate()
        {
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistoryGeneralInfoUpdateModel model = new dataPersonalHistoryGeneralInfoUpdateModel();
            model = Mapper.Map(dataGTPApplication.dataPersonalHistoryGeneralInfoes.FirstOrDefault(), new dataPersonalHistoryGeneralInfoUpdateModel());
            if (model == null)
            {
                model = new dataPersonalHistoryGeneralInfoUpdateModel();
            }
            model.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            return PartialView("~/Areas/GTP/Views/Applications/PersonalHistoryFormWizard/_GeneralInformationWizardForm.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult GeneralInformationWizardCreate(dataPersonalHistoryGeneralInfoUpdateModel model)
        {
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistoryGeneralInfo dataPersonalHistoryGeneralInfo = new dataPersonalHistoryGeneralInfo();
            dataPersonalHistoryGeneralInfo = Mapper.Map(model, new dataPersonalHistoryGeneralInfo());
            dataPersonalHistoryGeneralInfo.GTPPHGeneralInfoGUID = Guid.NewGuid();
            dataPersonalHistoryGeneralInfo.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            dataPersonalHistoryGeneralInfo.Active = true;
            DbGTP.dataPersonalHistoryGeneralInfoes.Add(dataPersonalHistoryGeneralInfo);
            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();

                return Json(DbGTP.SingleCreateMessage("WizardHandling();", true));
            }
            catch (Exception ex)
            {
                return Json(DbGTP.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult GeneralInformationWizardUpdate(dataPersonalHistoryGeneralInfoUpdateModel model)
        {
            DateTime ExecutionTime = DateTime.Now;
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistoryGeneralInfo dataPersonalHistoryGeneralInfo = new dataPersonalHistoryGeneralInfo();
            dataPersonalHistoryGeneralInfo = Mapper.Map(model, new dataPersonalHistoryGeneralInfo());
            dataPersonalHistoryGeneralInfo.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            dataPersonalHistoryGeneralInfo.Active = model.Active;

            var original = DbGTP.Set<dataPersonalHistoryGeneralInfo>().Find(dataPersonalHistoryGeneralInfo.GTPPHGeneralInfoGUID);
            DbGTP.Entry(original).CurrentValues.SetValues(dataPersonalHistoryGeneralInfo);
            bool Changed = false;
            var entry = DbGTP.Entry(original);
            var namesOfChangedProperties = entry.CurrentValues.PropertyNames.Where(p => entry.Property(p).IsModified && entry.Property(p).Name.EndsWith("RowVersion") == false && entry.Property(p).Name != "DeletedOn").ToList();
            if (namesOfChangedProperties.Count > 0)
            {
                Changed = true;
                dataPersonalHistoryConfirmationAndConsent dataPersonalHistoryConfirmationAndConsent = dataGTPApplication.dataPersonalHistoryConfirmationAndConsents.FirstOrDefault();
                if (dataPersonalHistoryConfirmationAndConsent != null)
                {
                    dataPersonalHistoryConfirmationAndConsent.IsConfirmedByUser = false;
                }
                dataGTPApplication.IsReviewed = false;
                dataGTPApplication.UpdatedOn = DateTime.Now;

                //DbGTP.Entry(original).State = System.Data.Entity.EntityState.Detached;
                //DbGTP.Entry(dataPersonalHistoryGeneralInfo).State = System.Data.Entity.EntityState.Modified;
            }

            DbGTP.Update(dataPersonalHistoryGeneralInfo, Permissions.Portalforgrouptwoapplications.UpdateGuid, ExecutionTime, DbCMS);


            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();
                if (Changed)
                {
                    return Json(DbGTP.SingleCreateMessage("WizardHandling(); RequireConfirmation();", true));
                }
                return Json(DbGTP.SingleCreateMessage("WizardHandling();", true));
            }
            catch (Exception ex)
            {
                return Json(DbGTP.ErrorMessage(ex.Message));
            }

        }

        #endregion

        #region Section 2 Personal Information
        [Route("GTP/Applications/PersonalInformationWizard/Create")]
        public ActionResult PersonalInformationWizardCreate()
        {
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistoryPersonalInfoUpdateModel model = new dataPersonalHistoryPersonalInfoUpdateModel();
            model = Mapper.Map(dataGTPApplication.dataPersonalHistoryPersonalInfoes.FirstOrDefault(), new dataPersonalHistoryPersonalInfoUpdateModel());
            if (model == null)
            {
                model = new dataPersonalHistoryPersonalInfoUpdateModel();
            }
            model.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            return PartialView("~/Areas/GTP/Views/Applications/PersonalHistoryFormWizard/_PersonalInformationWizardForm.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult PersonalInformationWizardCreate(dataPersonalHistoryPersonalInfoUpdateModel model)
        {
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistoryPersonalInfo dataPersonalHistoryPersonalInfo = new dataPersonalHistoryPersonalInfo();
            dataPersonalHistoryPersonalInfo = Mapper.Map(model, new dataPersonalHistoryPersonalInfo());
            dataPersonalHistoryPersonalInfo.GTPPHPersonalInfoGUID = Guid.NewGuid();
            dataPersonalHistoryPersonalInfo.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            dataPersonalHistoryPersonalInfo.Active = true;
            DbGTP.dataPersonalHistoryPersonalInfoes.Add(dataPersonalHistoryPersonalInfo);
            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();

                return Json(DbGTP.SingleCreateMessage("WizardHandling();", true));
            }
            catch (Exception ex)
            {
                return Json(DbGTP.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult PersonalInformationWizardUpdate(dataPersonalHistoryPersonalInfoUpdateModel model)
        {
            DateTime ExecutionTime = DateTime.Now;
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistoryPersonalInfo dataPersonalHistoryPersonalInfo = new dataPersonalHistoryPersonalInfo();
            dataPersonalHistoryPersonalInfo = Mapper.Map(model, new dataPersonalHistoryPersonalInfo());
            dataPersonalHistoryPersonalInfo.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            dataPersonalHistoryPersonalInfo.Active = model.Active;

            var original = DbGTP.Set<dataPersonalHistoryPersonalInfo>().Find(dataPersonalHistoryPersonalInfo.GTPPHPersonalInfoGUID);
            DbGTP.Entry(original).CurrentValues.SetValues(dataPersonalHistoryPersonalInfo);
            bool Changed = false;
            var entry = DbGTP.Entry(original);
            var namesOfChangedProperties = entry.CurrentValues.PropertyNames.Where(p => entry.Property(p).IsModified && entry.Property(p).Name.EndsWith("RowVersion") == false && entry.Property(p).Name != "DeletedOn").ToList();
            if (namesOfChangedProperties.Count > 0)
            {
                Changed = true;
                dataPersonalHistoryConfirmationAndConsent dataPersonalHistoryConfirmationAndConsent = dataGTPApplication.dataPersonalHistoryConfirmationAndConsents.FirstOrDefault();
                if (dataPersonalHistoryConfirmationAndConsent != null)
                {
                    dataPersonalHistoryConfirmationAndConsent.IsConfirmedByUser = false;
                }
                dataGTPApplication.IsReviewed = false;
                dataGTPApplication.UpdatedOn = DateTime.Now;
            }


            DbGTP.Update(dataPersonalHistoryPersonalInfo, Permissions.Portalforgrouptwoapplications.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();
                if (Changed)
                {
                    return Json(DbGTP.SingleCreateMessage("WizardHandling(); RequireConfirmation();", true));
                }
                return Json(DbGTP.SingleCreateMessage("WizardHandling();", true));
            }
            catch (Exception ex)
            {
                return Json(DbGTP.ErrorMessage(ex.Message));
            }
        }
        #endregion

        #region Section 3 Contanct Information

        [Route("GTP/Applications/ContanctInformationWizard/Create")]
        public ActionResult ContanctInformationWizardCreate()
        {
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistoryContactInfoUpdateModel model = new dataPersonalHistoryContactInfoUpdateModel();
            model = Mapper.Map(dataGTPApplication.dataPersonalHistoryContactInfoes.FirstOrDefault(), new dataPersonalHistoryContactInfoUpdateModel());
            if (model == null)
            {
                model = new dataPersonalHistoryContactInfoUpdateModel();
            }
            model.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            return PartialView("~/Areas/GTP/Views/Applications/PersonalHistoryFormWizard/_ContanctInformationWizardForm.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult ContanctInformationWizardCreate(dataPersonalHistoryContactInfoUpdateModel model)
        {
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistoryContactInfo dataPersonalHistoryContactInfo = new dataPersonalHistoryContactInfo();
            dataPersonalHistoryContactInfo = Mapper.Map(model, new dataPersonalHistoryContactInfo());
            dataPersonalHistoryContactInfo.GTPPHContactInfoGUID = Guid.NewGuid();
            dataPersonalHistoryContactInfo.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            dataPersonalHistoryContactInfo.Active = true;
            DbGTP.dataPersonalHistoryContactInfoes.Add(dataPersonalHistoryContactInfo);
            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();

                return Json(DbGTP.SingleCreateMessage("WizardHandling();", true));
            }
            catch (Exception ex)
            {
                return Json(DbGTP.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult ContanctInformationWizardUpdate(dataPersonalHistoryContactInfoUpdateModel model)
        {
            DateTime ExecutionTime = DateTime.Now;
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistoryContactInfo dataPersonalHistoryContactInfo = new dataPersonalHistoryContactInfo();
            dataPersonalHistoryContactInfo = Mapper.Map(model, new dataPersonalHistoryContactInfo());
            dataPersonalHistoryContactInfo.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            dataPersonalHistoryContactInfo.Active = model.Active;

            var original = DbGTP.Set<dataPersonalHistoryContactInfo>().Find(dataPersonalHistoryContactInfo.GTPPHContactInfoGUID);
            DbGTP.Entry(original).CurrentValues.SetValues(dataPersonalHistoryContactInfo);
            bool Changed = false;
            var entry = DbGTP.Entry(original);
            var namesOfChangedProperties = entry.CurrentValues.PropertyNames.Where(p => entry.Property(p).IsModified && entry.Property(p).Name.EndsWith("RowVersion") == false && entry.Property(p).Name != "DeletedOn").ToList();
            if (namesOfChangedProperties.Count > 0)
            {
                Changed = true;
                dataPersonalHistoryConfirmationAndConsent dataPersonalHistoryConfirmationAndConsent = dataGTPApplication.dataPersonalHistoryConfirmationAndConsents.FirstOrDefault();
                if (dataPersonalHistoryConfirmationAndConsent != null)
                {
                    dataPersonalHistoryConfirmationAndConsent.IsConfirmedByUser = false;
                }
                dataGTPApplication.IsReviewed = false;
                dataGTPApplication.UpdatedOn = DateTime.Now;
            }


            DbGTP.Update(dataPersonalHistoryContactInfo, Permissions.Portalforgrouptwoapplications.UpdateGuid, ExecutionTime, DbCMS);


            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();
                if (Changed)
                {
                    return Json(DbGTP.SingleCreateMessage("WizardHandling(); RequireConfirmation();", true));
                }
                return Json(DbGTP.SingleCreateMessage("WizardHandling();", true));
            }
            catch (Exception ex)
            {
                return Json(DbGTP.ErrorMessage(ex.Message));
            }
        }
        #endregion

        #region Section 4 PHONE NUMBERS
        [Route("GTP/Applications/PhoneNumbersWizard/Create")]
        public ActionResult PhoneNumbersWizardCreate()
        {
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistoryPhoneNumberUpdateModel model = new dataPersonalHistoryPhoneNumberUpdateModel();
            model = Mapper.Map(dataGTPApplication.dataPersonalHistoryPhoneNumbers.FirstOrDefault(), new dataPersonalHistoryPhoneNumberUpdateModel());
            if (model == null)
            {
                model = new dataPersonalHistoryPhoneNumberUpdateModel();
            }
            model.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            return PartialView("~/Areas/GTP/Views/Applications/PersonalHistoryFormWizard/_PhoneNumbersWizardForm.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult PhoneNumbersWizardCreate(dataPersonalHistoryPhoneNumberUpdateModel model)
        {
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistoryPhoneNumber dataPersonalHistoryPhoneNumber = new dataPersonalHistoryPhoneNumber();
            dataPersonalHistoryPhoneNumber = Mapper.Map(model, new dataPersonalHistoryPhoneNumber());
            dataPersonalHistoryPhoneNumber.GTPPHPhoneNumberGUID = Guid.NewGuid();
            dataPersonalHistoryPhoneNumber.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            dataPersonalHistoryPhoneNumber.Active = true;
            DbGTP.dataPersonalHistoryPhoneNumbers.Add(dataPersonalHistoryPhoneNumber);
            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();

                return Json(DbGTP.SingleCreateMessage("WizardHandling();", true));
            }
            catch (Exception ex)
            {
                return Json(DbGTP.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult PhoneNumbersWizardUpdate(dataPersonalHistoryPhoneNumberUpdateModel model)
        {
            DateTime ExecutionTime = DateTime.Now;
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistoryPhoneNumber dataPersonalHistoryPhoneNumber = new dataPersonalHistoryPhoneNumber();
            dataPersonalHistoryPhoneNumber = Mapper.Map(model, new dataPersonalHistoryPhoneNumber());
            dataPersonalHistoryPhoneNumber.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            dataPersonalHistoryPhoneNumber.Active = model.Active;

            var original = DbGTP.Set<dataPersonalHistoryPhoneNumber>().Find(dataPersonalHistoryPhoneNumber.GTPPHPhoneNumberGUID);
            DbGTP.Entry(original).CurrentValues.SetValues(dataPersonalHistoryPhoneNumber);
            bool Changed = false;
            var entry = DbGTP.Entry(original);
            var namesOfChangedProperties = entry.CurrentValues.PropertyNames.Where(p => entry.Property(p).IsModified && entry.Property(p).Name.EndsWith("RowVersion") == false && entry.Property(p).Name != "DeletedOn").ToList();
            if (namesOfChangedProperties.Count > 0)
            {
                Changed = true;
                dataPersonalHistoryConfirmationAndConsent dataPersonalHistoryConfirmationAndConsent = dataGTPApplication.dataPersonalHistoryConfirmationAndConsents.FirstOrDefault();
                if (dataPersonalHistoryConfirmationAndConsent != null)
                {
                    dataPersonalHistoryConfirmationAndConsent.IsConfirmedByUser = false;
                }
                dataGTPApplication.IsReviewed = false;
                dataGTPApplication.UpdatedOn = DateTime.Now;
            }



            DbGTP.Update(dataPersonalHistoryPhoneNumber, Permissions.Portalforgrouptwoapplications.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();
                if (Changed)
                {
                    return Json(DbGTP.SingleCreateMessage("WizardHandling(); RequireConfirmation();", true));
                }
                return Json(DbGTP.SingleCreateMessage("WizardHandling();", true));
            }
            catch (Exception ex)
            {
                return Json(DbGTP.ErrorMessage(ex.Message));
            }
        }
        #endregion

        #region Section 5  EMAIL ADDRESSES
        [Route("GTP/Applications/EmailAddressesWizard/Create")]
        public ActionResult EmailAddressesWizardCreate()
        {
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistoryEmailAddressUpdateModel model = new dataPersonalHistoryEmailAddressUpdateModel();
            model = Mapper.Map(dataGTPApplication.dataPersonalHistoryEmailAddresses.FirstOrDefault(), new dataPersonalHistoryEmailAddressUpdateModel());
            if (model == null)
            {
                model = new dataPersonalHistoryEmailAddressUpdateModel();
            }
            model.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            return PartialView("~/Areas/GTP/Views/Applications/PersonalHistoryFormWizard/_EmailAddressesWizardForm.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult EmailAddressesWizardCreate(dataPersonalHistoryEmailAddressUpdateModel model)
        {
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistoryEmailAddress dataPersonalHistoryEmailAddress = new dataPersonalHistoryEmailAddress();
            dataPersonalHistoryEmailAddress = Mapper.Map(model, new dataPersonalHistoryEmailAddress());
            dataPersonalHistoryEmailAddress.GTPPHEmailAddressGUID = Guid.NewGuid();
            dataPersonalHistoryEmailAddress.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            dataPersonalHistoryEmailAddress.Active = true;
            DbGTP.dataPersonalHistoryEmailAddresses.Add(dataPersonalHistoryEmailAddress);
            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();

                return Json(DbGTP.SingleCreateMessage("WizardHandling();", true));
            }
            catch (Exception ex)
            {
                return Json(DbGTP.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult EmailAddressesWizardUpdate(dataPersonalHistoryEmailAddressUpdateModel model)
        {
            DateTime ExecutionTime = DateTime.Now;
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistoryEmailAddress dataPersonalHistoryEmailAddress = new dataPersonalHistoryEmailAddress();
            dataPersonalHistoryEmailAddress = Mapper.Map(model, new dataPersonalHistoryEmailAddress());
            dataPersonalHistoryEmailAddress.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            dataPersonalHistoryEmailAddress.Active = model.Active;

            var original = DbGTP.Set<dataPersonalHistoryEmailAddress>().Find(dataPersonalHistoryEmailAddress.GTPPHEmailAddressGUID);
            DbGTP.Entry(original).CurrentValues.SetValues(dataPersonalHistoryEmailAddress);
            bool Changed = false;
            var entry = DbGTP.Entry(original);
            var namesOfChangedProperties = entry.CurrentValues.PropertyNames.Where(p => entry.Property(p).IsModified && entry.Property(p).Name.EndsWith("RowVersion") == false && entry.Property(p).Name != "DeletedOn").ToList();
            if (namesOfChangedProperties.Count > 0)
            {
                Changed = true;
                dataPersonalHistoryConfirmationAndConsent dataPersonalHistoryConfirmationAndConsent = dataGTPApplication.dataPersonalHistoryConfirmationAndConsents.FirstOrDefault();
                if (dataPersonalHistoryConfirmationAndConsent != null)
                {
                    dataPersonalHistoryConfirmationAndConsent.IsConfirmedByUser = false;
                }
                dataGTPApplication.IsReviewed = false;
                dataGTPApplication.UpdatedOn = DateTime.Now;
            }


            DbGTP.Update(dataPersonalHistoryEmailAddress, Permissions.Portalforgrouptwoapplications.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();
                if (Changed)
                {
                    return Json(DbGTP.SingleCreateMessage("WizardHandling(); RequireConfirmation();", true));
                }
                return Json(DbGTP.SingleCreateMessage("WizardHandling();", true));
            }
            catch (Exception ex)
            {
                return Json(DbGTP.ErrorMessage(ex.Message));
            }
        }
        #endregion

        #region Section 6   NATIONALITY INFORMATIONS
        [Route("GTP/Applications/NationalityInformationsWizard/Create")]
        public ActionResult NationalityInformationsWizardCreate()
        {
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistoryNationalityInfoUpdateModel model = new dataPersonalHistoryNationalityInfoUpdateModel();
            model = Mapper.Map(dataGTPApplication.dataPersonalHistoryNationalityInfoes.FirstOrDefault(), new dataPersonalHistoryNationalityInfoUpdateModel());
            if (model == null)
            {
                model = new dataPersonalHistoryNationalityInfoUpdateModel();
            }
            model.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            if (dataGTPApplication.dataPersonalHistoryNationalityInfoes.FirstOrDefault() != null)
            {
                model.ClientNationalityAtBirth = dataGTPApplication.dataPersonalHistoryNationalityInfoes.FirstOrDefault().NationalityAtBirth;
                model.ClientCurrentNationality = dataGTPApplication.dataPersonalHistoryNationalityInfoes.FirstOrDefault().CurrentNationality;
            }

            return PartialView("~/Areas/GTP/Views/Applications/PersonalHistoryFormWizard/_NationalityInformationsWizardForm.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult NationalityInformationsWizardCreate(dataPersonalHistoryNationalityInfoUpdateModel model)
        {
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistoryNationalityInfo dataPersonalHistoryNationalityInfo = new dataPersonalHistoryNationalityInfo();
            dataPersonalHistoryNationalityInfo = Mapper.Map(model, new dataPersonalHistoryNationalityInfo());
            dataPersonalHistoryNationalityInfo.GTPPHNationalityInfoGUID = Guid.NewGuid();
            dataPersonalHistoryNationalityInfo.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            dataPersonalHistoryNationalityInfo.Active = true;
            dataPersonalHistoryNationalityInfo.NationalityAtBirth = string.Join(",", model.NationalityAtBirth);
            dataPersonalHistoryNationalityInfo.CurrentNationality = string.Join(",", model.CurrentNationality);
            DbGTP.dataPersonalHistoryNationalityInfoes.Add(dataPersonalHistoryNationalityInfo);
            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();

                return Json(DbGTP.SingleCreateMessage("WizardHandling();", true));
            }
            catch (Exception ex)
            {
                return Json(DbGTP.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult NationalityInformationsWizardUpdate(dataPersonalHistoryNationalityInfoUpdateModel model)
        {
            DateTime ExecutionTime = DateTime.Now;
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistoryNationalityInfo dataPersonalHistoryNationalityInfo = new dataPersonalHistoryNationalityInfo();
            dataPersonalHistoryNationalityInfo = Mapper.Map(model, new dataPersonalHistoryNationalityInfo());
            dataPersonalHistoryNationalityInfo.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            dataPersonalHistoryNationalityInfo.Active = model.Active;
            dataPersonalHistoryNationalityInfo.NationalityAtBirth = string.Join(",", model.NationalityAtBirth);
            dataPersonalHistoryNationalityInfo.CurrentNationality = string.Join(",", model.CurrentNationality);

            var original = DbGTP.Set<dataPersonalHistoryNationalityInfo>().Find(dataPersonalHistoryNationalityInfo.GTPPHNationalityInfoGUID);
            DbGTP.Entry(original).CurrentValues.SetValues(dataPersonalHistoryNationalityInfo);
            bool Changed = false;
            var entry = DbGTP.Entry(original);
            var namesOfChangedProperties = entry.CurrentValues.PropertyNames.Where(p => entry.Property(p).IsModified && entry.Property(p).Name.EndsWith("RowVersion") == false && entry.Property(p).Name != "DeletedOn").ToList();
            if (namesOfChangedProperties.Count > 0)
            {
                Changed = true;
                dataPersonalHistoryConfirmationAndConsent dataPersonalHistoryConfirmationAndConsent = dataGTPApplication.dataPersonalHistoryConfirmationAndConsents.FirstOrDefault();
                if (dataPersonalHistoryConfirmationAndConsent != null)
                {
                    dataPersonalHistoryConfirmationAndConsent.IsConfirmedByUser = false;
                }
                dataGTPApplication.IsReviewed = false;
                dataGTPApplication.UpdatedOn = DateTime.Now;
            }


            DbGTP.Update(dataPersonalHistoryNationalityInfo, Permissions.Portalforgrouptwoapplications.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();
                if (Changed)
                {
                    return Json(DbGTP.SingleCreateMessage("WizardHandling(); RequireConfirmation();", true));
                }
                return Json(DbGTP.SingleCreateMessage("WizardHandling();", true));
            }
            catch (Exception ex)
            {
                return Json(DbGTP.ErrorMessage(ex.Message));
            }
        }
        #endregion

        #region Section 7  LETTER OF INTEREST
        [Route("GTP/Applications/LetterOfInterestWizard/Create")]
        public ActionResult LetterOfInterestWizardCreate()
        {
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistoryLetterOfInterestUpdateModel model = new dataPersonalHistoryLetterOfInterestUpdateModel();
            model = Mapper.Map(dataGTPApplication.dataPersonalHistoryLetterOfInterests.FirstOrDefault(), new dataPersonalHistoryLetterOfInterestUpdateModel());
            if (model == null)
            {
                model = new dataPersonalHistoryLetterOfInterestUpdateModel();
            }
            model.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            return PartialView("~/Areas/GTP/Views/Applications/PersonalHistoryFormWizard/_LetterOfInterestWizardForm.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult LetterOfInterestWizardCreate(dataPersonalHistoryLetterOfInterestUpdateModel model)
        {
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistoryLetterOfInterest dataPersonalHistoryLetterOfInterest = new dataPersonalHistoryLetterOfInterest();
            dataPersonalHistoryLetterOfInterest = Mapper.Map(model, new dataPersonalHistoryLetterOfInterest());
            dataPersonalHistoryLetterOfInterest.GTPPHLetterOfInterestGUID = Guid.NewGuid();
            dataPersonalHistoryLetterOfInterest.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            dataPersonalHistoryLetterOfInterest.Active = true;
            DbGTP.dataPersonalHistoryLetterOfInterests.Add(dataPersonalHistoryLetterOfInterest);
            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();

                return Json(DbGTP.SingleCreateMessage("WizardHandling();", true));
            }
            catch (Exception ex)
            {
                return Json(DbGTP.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult LetterOfInterestWizardUpdate(dataPersonalHistoryLetterOfInterestUpdateModel model)
        {
            DateTime ExecutionTime = DateTime.Now;
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistoryLetterOfInterest dataPersonalHistoryLetterOfInterest = new dataPersonalHistoryLetterOfInterest();
            dataPersonalHistoryLetterOfInterest = Mapper.Map(model, new dataPersonalHistoryLetterOfInterest());
            dataPersonalHistoryLetterOfInterest.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            dataPersonalHistoryLetterOfInterest.Active = model.Active;

            var original = DbGTP.Set<dataPersonalHistoryLetterOfInterest>().Find(dataPersonalHistoryLetterOfInterest.GTPPHLetterOfInterestGUID);
            DbGTP.Entry(original).CurrentValues.SetValues(dataPersonalHistoryLetterOfInterest);
            bool Changed = false;
            var entry = DbGTP.Entry(original);
            var namesOfChangedProperties = entry.CurrentValues.PropertyNames.Where(p => entry.Property(p).IsModified && entry.Property(p).Name.EndsWith("RowVersion") == false && entry.Property(p).Name != "DeletedOn").ToList();
            if (namesOfChangedProperties.Count > 0)
            {
                Changed = true;
                dataPersonalHistoryConfirmationAndConsent dataPersonalHistoryConfirmationAndConsent = dataGTPApplication.dataPersonalHistoryConfirmationAndConsents.FirstOrDefault();
                if (dataPersonalHistoryConfirmationAndConsent != null)
                {
                    dataPersonalHistoryConfirmationAndConsent.IsConfirmedByUser = false;
                }
                dataGTPApplication.IsReviewed = false;
                dataGTPApplication.UpdatedOn = DateTime.Now;
            }


            DbGTP.Update(dataPersonalHistoryLetterOfInterest, Permissions.Portalforgrouptwoapplications.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();
                if (Changed)
                {
                    return Json(DbGTP.SingleCreateMessage("WizardHandling(); RequireConfirmation();", true));
                }
                return Json(DbGTP.SingleCreateMessage("WizardHandling();", true));
            }
            catch (Exception ex)
            {
                return Json(DbGTP.ErrorMessage(ex.Message));
            }
        }
        #endregion

        #region Section 8 Work Experiences
        [Route("GTP/Applications/WorkExperienceWizard/Create")]
        public ActionResult WorkExperienceWizardCreate()
        {
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            List<dataPersonalHistoryWorkExperienceUpdateModel> models = new List<dataPersonalHistoryWorkExperienceUpdateModel>();
            models = Mapper.Map(dataGTPApplication.dataPersonalHistoryWorkExperiences.Where(x => x.Active).ToList(), new List<dataPersonalHistoryWorkExperienceUpdateModel>());
            //if (models == null)
            //{
            //    models = new List<dataPersonalHistoryWorkExperienceUpdateModel>();
            //}
            //model.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            return PartialView("~/Areas/GTP/Views/Applications/PersonalHistoryFormWizard/_WorkExperienceWizardForm.cshtml", models);
        }


        public ActionResult WorkExperienceCreate()
        {
            dataPersonalHistoryWorkExperienceUpdateModel model = new dataPersonalHistoryWorkExperienceUpdateModel();
            return PartialView("~/Areas/GTP/Views/Applications/PersonalHistoryFormWizard/_WorkExperienceWizardUpdateModalForm.cshtml", model);
        }

        public ActionResult WorkExperienceUpdate(Guid PK)
        {
            dataPersonalHistoryWorkExperienceUpdateModel model = new dataPersonalHistoryWorkExperienceUpdateModel();
            dataPersonalHistoryWorkExperience dataPersonalHistoryWorkExperience = (from a in DbGTP.dataPersonalHistoryWorkExperiences where a.GTPPHWorkExperienceGUID == PK select a).FirstOrDefault();
            model = Mapper.Map(dataPersonalHistoryWorkExperience, new dataPersonalHistoryWorkExperienceUpdateModel());
            return PartialView("~/Areas/GTP/Views/Applications/PersonalHistoryFormWizard/_WorkExperienceWizardUpdateModalForm.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult WorkExperienceCreate(dataPersonalHistoryWorkExperienceUpdateModel model)
        {
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistoryWorkExperience dataPersonalHistoryWorkExperience = new dataPersonalHistoryWorkExperience();
            dataPersonalHistoryWorkExperience = Mapper.Map(model, new dataPersonalHistoryWorkExperience());
            dataPersonalHistoryWorkExperience.GTPPHWorkExperienceGUID = Guid.NewGuid();
            dataPersonalHistoryWorkExperience.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            dataPersonalHistoryWorkExperience.Active = true;
            DbGTP.dataPersonalHistoryWorkExperiences.Add(dataPersonalHistoryWorkExperience);

            dataGTPApplication.IsReviewed = false;
            dataGTPApplication.UpdatedOn = DateTime.Now;

            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();

                return Json(DbGTP.SingleCreateMessage("GoToWorkExperiences(); RequireConfirmation();", true));
            }
            catch (Exception ex)
            {
                return Json(DbGTP.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult WorkExperienceUpdate(dataPersonalHistoryWorkExperienceUpdateModel model)
        {
            DateTime ExecutionTime = DateTime.Now;
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistoryWorkExperience dataPersonalHistoryWorkExperience = new dataPersonalHistoryWorkExperience();
            dataPersonalHistoryWorkExperience = Mapper.Map(model, new dataPersonalHistoryWorkExperience());
            dataPersonalHistoryWorkExperience.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            dataPersonalHistoryWorkExperience.Active = model.Active;

            var original = DbGTP.Set<dataPersonalHistoryWorkExperience>().Find(dataPersonalHistoryWorkExperience.GTPPHWorkExperienceGUID);
            DbGTP.Entry(original).CurrentValues.SetValues(dataPersonalHistoryWorkExperience);
            bool Changed = false;
            var entry = DbGTP.Entry(original);
            var namesOfChangedProperties = entry.CurrentValues.PropertyNames.Where(p => entry.Property(p).IsModified && entry.Property(p).Name.EndsWith("RowVersion") == false && entry.Property(p).Name != "DeletedOn").ToList();
            if (namesOfChangedProperties.Count > 0)
            {
                Changed = true;
                dataPersonalHistoryConfirmationAndConsent dataPersonalHistoryConfirmationAndConsent = dataGTPApplication.dataPersonalHistoryConfirmationAndConsents.FirstOrDefault();
                if (dataPersonalHistoryConfirmationAndConsent != null)
                {
                    dataPersonalHistoryConfirmationAndConsent.IsConfirmedByUser = false;
                }
                dataGTPApplication.IsReviewed = false;
                dataGTPApplication.UpdatedOn = DateTime.Now;
            }


            DbGTP.Update(dataPersonalHistoryWorkExperience, Permissions.Portalforgrouptwoapplications.UpdateGuid, ExecutionTime, DbCMS);


            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();
                if (Changed)
                {
                    return Json(DbGTP.SingleCreateMessage("GoToWorkExperiences(); RequireConfirmation();", true));
                }
                return Json(DbGTP.SingleCreateMessage("GoToWorkExperiences();", true));
            }
            catch (Exception ex)
            {
                return Json(DbGTP.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult WorkExperienceDelete(dataPersonalHistoryWorkExperience model)
        {
            DateTime ExecutionTime = DateTime.Now;
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistoryWorkExperience dataPersonalHistoryWorkExperience = (from a in DbGTP.dataPersonalHistoryWorkExperiences where a.GTPPHWorkExperienceGUID == model.GTPPHWorkExperienceGUID select a).FirstOrDefault();
            dataPersonalHistoryWorkExperience.Active = false;
            dataGTPApplication.IsReviewed = false;
            dataGTPApplication.UpdatedOn = DateTime.Now;

            dataPersonalHistoryConfirmationAndConsent dataPersonalHistoryConfirmationAndConsent = dataGTPApplication.dataPersonalHistoryConfirmationAndConsents.FirstOrDefault();
            if (dataPersonalHistoryConfirmationAndConsent != null)
            {
                dataPersonalHistoryConfirmationAndConsent.IsConfirmedByUser = false;
            }

            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbGTP.SingleCreateMessage("GoToWorkExperiences();RequireConfirmation();", true));
            }
            catch (Exception ex)
            {
                return Json(DbGTP.ErrorMessage(ex.Message));
            }
        }
        #endregion

        #region Section 9 SPECIALIZED TRAINING
        [Route("GTP/Applications/SpecializedTrainingWizard/Create")]
        public ActionResult SpecializedTrainingWizardCreate()
        {
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            List<dataPersonalHistorySpecializedTrainingUpdateModel> models = new List<dataPersonalHistorySpecializedTrainingUpdateModel>();
            models = Mapper.Map(dataGTPApplication.dataPersonalHistorySpecializedTrainings.Where(x => x.Active).ToList(), new List<dataPersonalHistorySpecializedTrainingUpdateModel>());
            //if (models == null)
            //{
            //    models = new List<dataPersonalHistoryWorkExperienceUpdateModel>();
            //}
            //model.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            return PartialView("~/Areas/GTP/Views/Applications/PersonalHistoryFormWizard/_SpecializedTrainingWizardForm.cshtml", models);
        }
        public ActionResult SpecializedTrainingCreate()
        {
            dataPersonalHistorySpecializedTrainingUpdateModel model = new dataPersonalHistorySpecializedTrainingUpdateModel();
            return PartialView("~/Areas/GTP/Views/Applications/PersonalHistoryFormWizard/_SpecializedTrainingWizardUpdateModalForm.cshtml", model);
        }
        public ActionResult SpecializedTrainingUpdate(Guid PK)
        {
            dataPersonalHistorySpecializedTrainingUpdateModel model = new dataPersonalHistorySpecializedTrainingUpdateModel();
            dataPersonalHistorySpecializedTraining dataPersonalHistorySpecializedTraining = (from a in DbGTP.dataPersonalHistorySpecializedTrainings where a.GTPPHSpecializedTrainingGUID == PK select a).FirstOrDefault();
            model = Mapper.Map(dataPersonalHistorySpecializedTraining, new dataPersonalHistorySpecializedTrainingUpdateModel());
            return PartialView("~/Areas/GTP/Views/Applications/PersonalHistoryFormWizard/_SpecializedTrainingWizardUpdateModalForm.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult SpecializedTrainingCreate(dataPersonalHistorySpecializedTrainingUpdateModel model)
        {
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistorySpecializedTraining dataPersonalHistorySpecializedTraining = new dataPersonalHistorySpecializedTraining();
            dataPersonalHistorySpecializedTraining = Mapper.Map(model, new dataPersonalHistorySpecializedTraining());
            dataPersonalHistorySpecializedTraining.GTPPHSpecializedTrainingGUID = Guid.NewGuid();
            dataPersonalHistorySpecializedTraining.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            dataPersonalHistorySpecializedTraining.Active = true;
            DbGTP.dataPersonalHistorySpecializedTrainings.Add(dataPersonalHistorySpecializedTraining);

            dataGTPApplication.IsReviewed = false;
            dataGTPApplication.UpdatedOn = DateTime.Now;

            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();

                return Json(DbGTP.SingleCreateMessage("GoToSpecializedTrainings(); RequireConfirmation();", true));
            }
            catch (Exception ex)
            {
                return Json(DbGTP.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult SpecializedTrainingUpdate(dataPersonalHistorySpecializedTrainingUpdateModel model)
        {
            DateTime ExecutionTime = DateTime.Now;
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistorySpecializedTraining dataPersonalHistorySpecializedTraining = new dataPersonalHistorySpecializedTraining();
            dataPersonalHistorySpecializedTraining = Mapper.Map(model, new dataPersonalHistorySpecializedTraining());
            dataPersonalHistorySpecializedTraining.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            dataPersonalHistorySpecializedTraining.Active = model.Active;


            var original = DbGTP.Set<dataPersonalHistorySpecializedTraining>().Find(dataPersonalHistorySpecializedTraining.GTPPHSpecializedTrainingGUID);
            DbGTP.Entry(original).CurrentValues.SetValues(dataPersonalHistorySpecializedTraining);
            bool Changed = false;
            var entry = DbGTP.Entry(original);
            var namesOfChangedProperties = entry.CurrentValues.PropertyNames.Where(p => entry.Property(p).IsModified && entry.Property(p).Name.EndsWith("RowVersion") == false && entry.Property(p).Name != "DeletedOn").ToList();
            if (namesOfChangedProperties.Count > 0)
            {
                Changed = true;
                dataPersonalHistoryConfirmationAndConsent dataPersonalHistoryConfirmationAndConsent = dataGTPApplication.dataPersonalHistoryConfirmationAndConsents.FirstOrDefault();
                if (dataPersonalHistoryConfirmationAndConsent != null)
                {
                    dataPersonalHistoryConfirmationAndConsent.IsConfirmedByUser = false;
                }
                dataGTPApplication.IsReviewed = false;
                dataGTPApplication.UpdatedOn = DateTime.Now;
            }

            DbGTP.Update(dataPersonalHistorySpecializedTraining, Permissions.Portalforgrouptwoapplications.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();
                if (Changed)
                {
                    return Json(DbGTP.SingleCreateMessage("GoToSpecializedTrainings(); RequireConfirmation();", true));
                }
                return Json(DbGTP.SingleCreateMessage("GoToSpecializedTrainings();", true));
            }
            catch (Exception ex)
            {
                return Json(DbGTP.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult SpecializedTrainingDelete(dataPersonalHistorySpecializedTraining model)
        {
            DateTime ExecutionTime = DateTime.Now;
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistorySpecializedTraining dataPersonalHistorySpecializedTraining = (from a in DbGTP.dataPersonalHistorySpecializedTrainings where a.GTPPHSpecializedTrainingGUID == model.GTPPHSpecializedTrainingGUID select a).FirstOrDefault();
            dataPersonalHistorySpecializedTraining.Active = false;
            dataGTPApplication.IsReviewed = false;
            dataGTPApplication.UpdatedOn = DateTime.Now;

            dataPersonalHistoryConfirmationAndConsent dataPersonalHistoryConfirmationAndConsent = dataGTPApplication.dataPersonalHistoryConfirmationAndConsents.FirstOrDefault();
            if (dataPersonalHistoryConfirmationAndConsent != null)
            {
                dataPersonalHistoryConfirmationAndConsent.IsConfirmedByUser = false;
            }

            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();

                return Json(DbGTP.SingleCreateMessage("GoToSpecializedTrainings(); RequireConfirmation();", true));
            }
            catch (Exception ex)
            {
                return Json(DbGTP.ErrorMessage(ex.Message));
            }
        }
        #endregion

        #region Section 10    EDUCATION
        [Route("GTP/Applications/EducationWizard/Create")]
        public ActionResult EducationWizardCreate()
        {
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            List<dataPersonalHistoryEducationUpdateModel> models = new List<dataPersonalHistoryEducationUpdateModel>();
            models = Mapper.Map(dataGTPApplication.dataPersonalHistoryEducations.Where(x => x.Active).ToList(), new List<dataPersonalHistoryEducationUpdateModel>());
            //if (models == null)
            //{
            //    models = new List<dataPersonalHistoryWorkExperienceUpdateModel>();
            //}
            //model.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            return PartialView("~/Areas/GTP/Views/Applications/PersonalHistoryFormWizard/_EducationWizardForm.cshtml", models);
        }

        public ActionResult EducationCreate()
        {
            dataPersonalHistoryEducationUpdateModel model = new dataPersonalHistoryEducationUpdateModel();
            return PartialView("~/Areas/GTP/Views/Applications/PersonalHistoryFormWizard/_EducationWizardUpdateModalForm.cshtml", model);
        }

        public ActionResult EducationUpdate(Guid PK)
        {
            dataPersonalHistoryEducationUpdateModel model = new dataPersonalHistoryEducationUpdateModel();
            dataPersonalHistoryEducation dataPersonalHistoryEducation = (from a in DbGTP.dataPersonalHistoryEducations where a.GTPPHEducationGUID == PK select a).FirstOrDefault();
            model = Mapper.Map(dataPersonalHistoryEducation, new dataPersonalHistoryEducationUpdateModel());
            return PartialView("~/Areas/GTP/Views/Applications/PersonalHistoryFormWizard/_EducationWizardUpdateModalForm.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult EducationCreate(dataPersonalHistoryEducationUpdateModel model)
        {
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistoryEducation dataPersonalHistoryEducation = new dataPersonalHistoryEducation();
            dataPersonalHistoryEducation = Mapper.Map(model, new dataPersonalHistoryEducation());
            dataPersonalHistoryEducation.GTPPHEducationGUID = Guid.NewGuid();
            dataPersonalHistoryEducation.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            dataPersonalHistoryEducation.Active = true;
            DbGTP.dataPersonalHistoryEducations.Add(dataPersonalHistoryEducation);

            dataGTPApplication.IsReviewed = false;
            dataGTPApplication.UpdatedOn = DateTime.Now;

            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();

                return Json(DbGTP.SingleCreateMessage("GoToEducations(); RequireConfirmation();", true));
            }
            catch (Exception ex)
            {
                return Json(DbGTP.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult EducationUpdate(dataPersonalHistoryEducationUpdateModel model)
        {
            DateTime ExecutionTime = DateTime.Now;
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistoryEducation dataPersonalHistoryEducation = new dataPersonalHistoryEducation();
            dataPersonalHistoryEducation = Mapper.Map(model, new dataPersonalHistoryEducation());
            dataPersonalHistoryEducation.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            dataPersonalHistoryEducation.Active = model.Active;


            var original = DbGTP.Set<dataPersonalHistoryEducation>().Find(dataPersonalHistoryEducation.GTPPHEducationGUID);
            DbGTP.Entry(original).CurrentValues.SetValues(dataPersonalHistoryEducation);
            bool Changed = false;
            var entry = DbGTP.Entry(original);
            var namesOfChangedProperties = entry.CurrentValues.PropertyNames.Where(p => entry.Property(p).IsModified && entry.Property(p).Name.EndsWith("RowVersion") == false && entry.Property(p).Name != "DeletedOn").ToList();
            if (namesOfChangedProperties.Count > 0)
            {
                Changed = true;
                dataPersonalHistoryConfirmationAndConsent dataPersonalHistoryConfirmationAndConsent = dataGTPApplication.dataPersonalHistoryConfirmationAndConsents.FirstOrDefault();
                if (dataPersonalHistoryConfirmationAndConsent != null)
                {
                    dataPersonalHistoryConfirmationAndConsent.IsConfirmedByUser = false;
                }
                dataGTPApplication.IsReviewed = false;
                dataGTPApplication.UpdatedOn = DateTime.Now;
            }


            DbGTP.Update(dataPersonalHistoryEducation, Permissions.Portalforgrouptwoapplications.UpdateGuid, ExecutionTime, DbCMS);


            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();
                if (Changed)
                {
                    return Json(DbGTP.SingleCreateMessage("GoToEducations(); RequireConfirmation();", true));
                }
                return Json(DbGTP.SingleCreateMessage("GoToEducations();", true));
            }
            catch (Exception ex)
            {
                return Json(DbGTP.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult EducationDelete(dataPersonalHistoryEducation model)
        {
            DateTime ExecutionTime = DateTime.Now;
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistoryEducation dataPersonalHistoryEducation = (from a in DbGTP.dataPersonalHistoryEducations where a.GTPPHEducationGUID == model.GTPPHEducationGUID select a).FirstOrDefault();
            dataPersonalHistoryEducation.Active = false;
            dataGTPApplication.IsReviewed = false;
            dataGTPApplication.UpdatedOn = DateTime.Now;

            dataPersonalHistoryConfirmationAndConsent dataPersonalHistoryConfirmationAndConsent = dataGTPApplication.dataPersonalHistoryConfirmationAndConsents.FirstOrDefault();
            if (dataPersonalHistoryConfirmationAndConsent != null)
            {
                dataPersonalHistoryConfirmationAndConsent.IsConfirmedByUser = false;
            }

            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();

                return Json(DbGTP.SingleCreateMessage("GoToEducations(); RequireConfirmation();", true));
            }
            catch (Exception ex)
            {
                return Json(DbGTP.ErrorMessage(ex.Message));
            }
        }
        #endregion

        #region Section 11 SKILLS
        [Route("GTP/Applications/SkillsWizard/Create")]
        public ActionResult SkillsWizardCreate()
        {
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            List<dataPersonalHistorySkillUpdateModel> models = new List<dataPersonalHistorySkillUpdateModel>();
            models = Mapper.Map(dataGTPApplication.dataPersonalHistorySkills.Where(x => x.Active).ToList(), new List<dataPersonalHistorySkillUpdateModel>());
            //if (models == null)
            //{
            //    models = new List<dataPersonalHistoryWorkExperienceUpdateModel>();
            //}
            //model.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            return PartialView("~/Areas/GTP/Views/Applications/PersonalHistoryFormWizard/_SkillsWizardForm.cshtml", models);
        }

        public ActionResult SkillsCreate()
        {
            dataPersonalHistorySkillUpdateModel model = new dataPersonalHistorySkillUpdateModel();
            return PartialView("~/Areas/GTP/Views/Applications/PersonalHistoryFormWizard/_SkillsWizardUpdateModalForm.cshtml", model);
        }

        public ActionResult SkillsUpdate(Guid PK)
        {
            dataPersonalHistorySkillUpdateModel model = new dataPersonalHistorySkillUpdateModel();
            dataPersonalHistorySkill dataPersonalHistorySkill = (from a in DbGTP.dataPersonalHistorySkills where a.GTPPHSkillGUID == PK select a).FirstOrDefault();
            model = Mapper.Map(dataPersonalHistorySkill, new dataPersonalHistorySkillUpdateModel());
            return PartialView("~/Areas/GTP/Views/Applications/PersonalHistoryFormWizard/_SkillsWizardUpdateModalForm.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult SkillsCreate(dataPersonalHistorySkillUpdateModel model)
        {
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistorySkill dataPersonalHistorySkill = new dataPersonalHistorySkill();
            dataPersonalHistorySkill = Mapper.Map(model, new dataPersonalHistorySkill());
            dataPersonalHistorySkill.GTPPHSkillGUID = Guid.NewGuid();
            dataPersonalHistorySkill.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            dataPersonalHistorySkill.Active = true;
            DbGTP.dataPersonalHistorySkills.Add(dataPersonalHistorySkill);


            dataGTPApplication.IsReviewed = false;
            dataGTPApplication.UpdatedOn = DateTime.Now;

            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();

                return Json(DbGTP.SingleCreateMessage("GoToSkills(); RequireConfirmation();", true));
            }
            catch (Exception ex)
            {
                return Json(DbGTP.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult SkillsUpdate(dataPersonalHistorySkillUpdateModel model)
        {
            DateTime ExecutionTime = DateTime.Now;
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistorySkill dataPersonalHistorySkill = new dataPersonalHistorySkill();
            dataPersonalHistorySkill = Mapper.Map(model, new dataPersonalHistorySkill());
            dataPersonalHistorySkill.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            dataPersonalHistorySkill.Active = model.Active;


            var original = DbGTP.Set<dataPersonalHistorySkill>().Find(dataPersonalHistorySkill.GTPPHSkillGUID);
            DbGTP.Entry(original).CurrentValues.SetValues(dataPersonalHistorySkill);
            bool Changed = false;
            var entry = DbGTP.Entry(original);
            var namesOfChangedProperties = entry.CurrentValues.PropertyNames.Where(p => entry.Property(p).IsModified && entry.Property(p).Name.EndsWith("RowVersion") == false && entry.Property(p).Name != "DeletedOn").ToList();
            if (namesOfChangedProperties.Count > 0)
            {
                Changed = true;
                dataPersonalHistoryConfirmationAndConsent dataPersonalHistoryConfirmationAndConsent = dataGTPApplication.dataPersonalHistoryConfirmationAndConsents.FirstOrDefault();
                if (dataPersonalHistoryConfirmationAndConsent != null)
                {
                    dataPersonalHistoryConfirmationAndConsent.IsConfirmedByUser = false;
                }
                dataGTPApplication.IsReviewed = false;
                dataGTPApplication.UpdatedOn = DateTime.Now;
            }

            DbGTP.Update(dataPersonalHistorySkill, Permissions.Portalforgrouptwoapplications.UpdateGuid, ExecutionTime, DbCMS);


            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();
                if (Changed)
                {
                    return Json(DbGTP.SingleCreateMessage("GoToSkills(); RequireConfirmation();", true));
                }
                return Json(DbGTP.SingleCreateMessage("GoToSkills();", true));
            }
            catch (Exception ex)
            {
                return Json(DbGTP.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult SkillsDelete(dataPersonalHistorySkill model)
        {
            DateTime ExecutionTime = DateTime.Now;
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistorySkill dataPersonalHistorySkill = (from a in DbGTP.dataPersonalHistorySkills where a.GTPPHSkillGUID == model.GTPPHSkillGUID select a).FirstOrDefault();
            dataPersonalHistorySkill.Active = false;
            dataGTPApplication.IsReviewed = false;
            dataGTPApplication.UpdatedOn = DateTime.Now;

            dataPersonalHistoryConfirmationAndConsent dataPersonalHistoryConfirmationAndConsent = dataGTPApplication.dataPersonalHistoryConfirmationAndConsents.FirstOrDefault();
            if (dataPersonalHistoryConfirmationAndConsent != null)
            {
                dataPersonalHistoryConfirmationAndConsent.IsConfirmedByUser = false;
            }

            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();

                return Json(DbGTP.SingleCreateMessage("GoToSkills(); RequireConfirmation();", true));
            }
            catch (Exception ex)
            {
                return Json(DbGTP.ErrorMessage(ex.Message));
            }
        }
        #endregion

        #region Section 12  LANGUAGES
        [Route("GTP/Applications/LanguagesWizard/Create")]
        public ActionResult LanguagesWizardCreate()
        {
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistoryLanguageUpdateModel model = new dataPersonalHistoryLanguageUpdateModel();
            model = Mapper.Map(dataGTPApplication.dataPersonalHistoryLanguages.FirstOrDefault(), new dataPersonalHistoryLanguageUpdateModel());
            if (model == null)
            {
                model = new dataPersonalHistoryLanguageUpdateModel();
            }
            model.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            return PartialView("~/Areas/GTP/Views/Applications/PersonalHistoryFormWizard/_LanguagesWizardForm.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult LanguagesWizardCreate(dataPersonalHistoryLanguageUpdateModel model)
        {
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistoryLanguage dataPersonalHistoryLanguage = new dataPersonalHistoryLanguage();
            dataPersonalHistoryLanguage = Mapper.Map(model, new dataPersonalHistoryLanguage());
            dataPersonalHistoryLanguage.GTPPHLanguageGUID = Guid.NewGuid();
            dataPersonalHistoryLanguage.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            dataPersonalHistoryLanguage.Active = true;
            DbGTP.dataPersonalHistoryLanguages.Add(dataPersonalHistoryLanguage);
            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();

                return Json(DbGTP.SingleCreateMessage("WizardHandling();", true));
            }
            catch (Exception ex)
            {
                return Json(DbGTP.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult LanguagesWizardUpdate(dataPersonalHistoryLanguageUpdateModel model)
        {
            DateTime ExecutionTime = DateTime.Now;
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistoryLanguage dataPersonalHistoryLanguage = new dataPersonalHistoryLanguage();
            dataPersonalHistoryLanguage = Mapper.Map(model, new dataPersonalHistoryLanguage());
            dataPersonalHistoryLanguage.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            dataPersonalHistoryLanguage.Active = model.Active;

            var original = DbGTP.Set<dataPersonalHistoryLanguage>().Find(dataPersonalHistoryLanguage.GTPPHLanguageGUID);
            DbGTP.Entry(original).CurrentValues.SetValues(dataPersonalHistoryLanguage);
            bool Changed = false;
            var entry = DbGTP.Entry(original);
            var namesOfChangedProperties = entry.CurrentValues.PropertyNames.Where(p => entry.Property(p).IsModified && entry.Property(p).Name.EndsWith("RowVersion") == false && entry.Property(p).Name != "DeletedOn").ToList();
            if (namesOfChangedProperties.Count > 0)
            {
                Changed = true;
                dataPersonalHistoryConfirmationAndConsent dataPersonalHistoryConfirmationAndConsent = dataGTPApplication.dataPersonalHistoryConfirmationAndConsents.FirstOrDefault();
                if (dataPersonalHistoryConfirmationAndConsent != null)
                {
                    dataPersonalHistoryConfirmationAndConsent.IsConfirmedByUser = false;
                }
                dataGTPApplication.IsReviewed = false;
                dataGTPApplication.UpdatedOn = DateTime.Now;
            }

            DbGTP.Update(dataPersonalHistoryLanguage, Permissions.Portalforgrouptwoapplications.UpdateGuid, ExecutionTime, DbCMS);


            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();
                if (Changed)
                {
                    return Json(DbGTP.SingleCreateMessage("WizardHandling(); RequireConfirmation();", true));
                }
                return Json(DbGTP.SingleCreateMessage("WizardHandling();", true));
            }
            catch (Exception ex)
            {
                return Json(DbGTP.ErrorMessage(ex.Message));
            }
        }
        #endregion

        #region Section 13  LICENSES AND CERTIFICATIONS
        [Route("GTP/Applications/LicencesAndCertificatesWizard/Create")]
        public ActionResult LicencesAndCertificatesWizardCreate()
        {
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            List<dataPersonalHistoryLicenceAndCertificateUpdateModel> models = new List<dataPersonalHistoryLicenceAndCertificateUpdateModel>();
            models = Mapper.Map(dataGTPApplication.dataPersonalHistoryLicenceAndCertificates.Where(x => x.Active).ToList(), new List<dataPersonalHistoryLicenceAndCertificateUpdateModel>());
            //if (models == null)
            //{
            //    models = new List<dataPersonalHistoryWorkExperienceUpdateModel>();
            //}
            //model.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            return PartialView("~/Areas/GTP/Views/Applications/PersonalHistoryFormWizard/_LicencesAndCertificatesWizardForm.cshtml", models);
        }

        public ActionResult LicencesAndCertificatesCreate()
        {
            dataPersonalHistoryLicenceAndCertificateUpdateModel model = new dataPersonalHistoryLicenceAndCertificateUpdateModel();
            return PartialView("~/Areas/GTP/Views/Applications/PersonalHistoryFormWizard/_LicencesAndCertificatesWizardUpdateModalForm.cshtml", model);
        }

        public ActionResult LicencesAndCertificatesUpdate(Guid PK)
        {
            dataPersonalHistoryLicenceAndCertificateUpdateModel model = new dataPersonalHistoryLicenceAndCertificateUpdateModel();
            dataPersonalHistoryLicenceAndCertificate dataPersonalHistoryLicenceAndCertificate = (from a in DbGTP.dataPersonalHistoryLicenceAndCertificates where a.GTPPHLicenceGUID == PK select a).FirstOrDefault();
            model = Mapper.Map(dataPersonalHistoryLicenceAndCertificate, new dataPersonalHistoryLicenceAndCertificateUpdateModel());
            return PartialView("~/Areas/GTP/Views/Applications/PersonalHistoryFormWizard/_LicencesAndCertificatesWizardUpdateModalForm.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult LicencesAndCertificatesCreate(dataPersonalHistoryLicenceAndCertificateUpdateModel model)
        {
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistoryLicenceAndCertificate dataPersonalHistoryLicenceAndCertificate = new dataPersonalHistoryLicenceAndCertificate();
            dataPersonalHistoryLicenceAndCertificate = Mapper.Map(model, new dataPersonalHistoryLicenceAndCertificate());
            dataPersonalHistoryLicenceAndCertificate.GTPPHLicenceGUID = Guid.NewGuid();
            dataPersonalHistoryLicenceAndCertificate.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            dataPersonalHistoryLicenceAndCertificate.Active = true;
            DbGTP.dataPersonalHistoryLicenceAndCertificates.Add(dataPersonalHistoryLicenceAndCertificate);

            dataGTPApplication.IsReviewed = false;
            dataGTPApplication.UpdatedOn = DateTime.Now;
            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();

                return Json(DbGTP.SingleCreateMessage("GoToLicencesAndCertificates(); RequireConfirmation();", true));
            }
            catch (Exception ex)
            {
                return Json(DbGTP.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult LicencesAndCertificatesUpdate(dataPersonalHistoryLicenceAndCertificateUpdateModel model)
        {
            DateTime ExecutionTime = DateTime.Now;
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistoryLicenceAndCertificate dataPersonalHistoryLicenceAndCertificate = new dataPersonalHistoryLicenceAndCertificate();
            dataPersonalHistoryLicenceAndCertificate = Mapper.Map(model, new dataPersonalHistoryLicenceAndCertificate());
            dataPersonalHistoryLicenceAndCertificate.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            dataPersonalHistoryLicenceAndCertificate.Active = model.Active;

            var original = DbGTP.Set<dataPersonalHistoryLicenceAndCertificate>().Find(dataPersonalHistoryLicenceAndCertificate.GTPPHLicenceGUID);
            DbGTP.Entry(original).CurrentValues.SetValues(dataPersonalHistoryLicenceAndCertificate);
            bool Changed = false;
            var entry = DbGTP.Entry(original);
            var namesOfChangedProperties = entry.CurrentValues.PropertyNames.Where(p => entry.Property(p).IsModified && entry.Property(p).Name.EndsWith("RowVersion") == false && entry.Property(p).Name != "DeletedOn").ToList();
            if (namesOfChangedProperties.Count > 0)
            {
                Changed = true;
                dataPersonalHistoryConfirmationAndConsent dataPersonalHistoryConfirmationAndConsent = dataGTPApplication.dataPersonalHistoryConfirmationAndConsents.FirstOrDefault();
                if (dataPersonalHistoryConfirmationAndConsent != null)
                {
                    dataPersonalHistoryConfirmationAndConsent.IsConfirmedByUser = false;
                }
                dataGTPApplication.IsReviewed = false;
                dataGTPApplication.UpdatedOn = DateTime.Now;
            }

            DbGTP.Update(dataPersonalHistoryLicenceAndCertificate, Permissions.Portalforgrouptwoapplications.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();
                if (Changed)
                {
                    return Json(DbGTP.SingleCreateMessage("GoToLicencesAndCertificates(); RequireConfirmation();", true));
                }
                return Json(DbGTP.SingleCreateMessage("GoToLicencesAndCertificates();", true));
            }
            catch (Exception ex)
            {
                return Json(DbGTP.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult LicencesAndCertificatesDelete(dataPersonalHistoryLicenceAndCertificate model)
        {
            DateTime ExecutionTime = DateTime.Now;
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistoryLicenceAndCertificate dataPersonalHistoryLicenceAndCertificate = (from a in DbGTP.dataPersonalHistoryLicenceAndCertificates where a.GTPPHLicenceGUID == model.GTPPHLicenceGUID select a).FirstOrDefault();
            dataPersonalHistoryLicenceAndCertificate.Active = false;
            dataGTPApplication.IsReviewed = false;
            dataGTPApplication.UpdatedOn = DateTime.Now;

            dataPersonalHistoryConfirmationAndConsent dataPersonalHistoryConfirmationAndConsent = dataGTPApplication.dataPersonalHistoryConfirmationAndConsents.FirstOrDefault();
            if (dataPersonalHistoryConfirmationAndConsent != null)
            {
                dataPersonalHistoryConfirmationAndConsent.IsConfirmedByUser = false;
            }

            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();

                return Json(DbGTP.SingleCreateMessage("GoToLicencesAndCertificates(); RequireConfirmation();", true));
            }
            catch (Exception ex)
            {
                return Json(DbGTP.ErrorMessage(ex.Message));
            }
        }
        #endregion

        #region Section 14  THREE PROFESSIONAL REFERENCES
        [Route("GTP/Applications/ProfessionalReferencesWizard/Create")]
        public ActionResult ProfessionalReferencesWizardCreate()
        {
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistoryProfessionalReferenceUpdateModel model = new dataPersonalHistoryProfessionalReferenceUpdateModel();
            model = Mapper.Map(dataGTPApplication.dataPersonalHistoryProfessionalReferences.FirstOrDefault(), new dataPersonalHistoryProfessionalReferenceUpdateModel());
            if (model == null)
            {
                model = new dataPersonalHistoryProfessionalReferenceUpdateModel();
            }
            model.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            return PartialView("~/Areas/GTP/Views/Applications/PersonalHistoryFormWizard/_ProfessionalReferencesWizardForm.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult ProfessionalReferencesWizardCreate(dataPersonalHistoryProfessionalReferenceUpdateModel model)
        {
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistoryProfessionalReference dataPersonalHistoryProfessionalReference = new dataPersonalHistoryProfessionalReference();
            dataPersonalHistoryProfessionalReference = Mapper.Map(model, new dataPersonalHistoryProfessionalReference());
            dataPersonalHistoryProfessionalReference.GTPPHProfReferenceGUID = Guid.NewGuid();
            dataPersonalHistoryProfessionalReference.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            dataPersonalHistoryProfessionalReference.Active = true;
            DbGTP.dataPersonalHistoryProfessionalReferences.Add(dataPersonalHistoryProfessionalReference);
            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();

                return Json(DbGTP.SingleCreateMessage("WizardHandling();", true));
            }
            catch (Exception ex)
            {
                return Json(DbGTP.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult ProfessionalReferencesWizardUpdate(dataPersonalHistoryProfessionalReferenceUpdateModel model)
        {
            DateTime ExecutionTime = DateTime.Now;
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistoryProfessionalReference dataPersonalHistoryProfessionalReference = new dataPersonalHistoryProfessionalReference();
            dataPersonalHistoryProfessionalReference = Mapper.Map(model, new dataPersonalHistoryProfessionalReference());
            dataPersonalHistoryProfessionalReference.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            dataPersonalHistoryProfessionalReference.Active = model.Active;

            var original = DbGTP.Set<dataPersonalHistoryProfessionalReference>().Find(dataPersonalHistoryProfessionalReference.GTPPHProfReferenceGUID);
            DbGTP.Entry(original).CurrentValues.SetValues(dataPersonalHistoryProfessionalReference);
            bool Changed = false;
            var entry = DbGTP.Entry(original);
            var namesOfChangedProperties = entry.CurrentValues.PropertyNames.Where(p => entry.Property(p).IsModified && entry.Property(p).Name.EndsWith("RowVersion") == false && entry.Property(p).Name != "DeletedOn").ToList();
            if (namesOfChangedProperties.Count > 0)
            {
                Changed = true;
                dataPersonalHistoryConfirmationAndConsent dataPersonalHistoryConfirmationAndConsent = dataGTPApplication.dataPersonalHistoryConfirmationAndConsents.FirstOrDefault();
                if (dataPersonalHistoryConfirmationAndConsent != null)
                {
                    dataPersonalHistoryConfirmationAndConsent.IsConfirmedByUser = false;
                }
                dataGTPApplication.IsReviewed = false;
                dataGTPApplication.UpdatedOn = DateTime.Now;
            }


            DbGTP.Update(dataPersonalHistoryProfessionalReference, Permissions.Portalforgrouptwoapplications.UpdateGuid, ExecutionTime, DbCMS);


            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();
                if (Changed)
                {
                    return Json(DbGTP.SingleCreateMessage("WizardHandling(); RequireConfirmation();", true));
                }
                return Json(DbGTP.SingleCreateMessage("WizardHandling();", true));
            }
            catch (Exception ex)
            {
                return Json(DbGTP.ErrorMessage(ex.Message));
            }
        }
        #endregion

        #region Section 15  QUESTIONNAIRE
        [Route("GTP/Applications/QuestionnaireWizard/Create")]
        public ActionResult QuestionnaireWizardCreate()
        {
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistoryQuestionnaireWithRelativesUpdateModel model = new dataPersonalHistoryQuestionnaireWithRelativesUpdateModel();
            model = Mapper.Map(dataGTPApplication.dataPersonalHistoryQuestionnaires.FirstOrDefault(), new dataPersonalHistoryQuestionnaireWithRelativesUpdateModel());
            if (model == null)
            {
                model = new dataPersonalHistoryQuestionnaireWithRelativesUpdateModel();
            }

            model.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;

            List<dataPersonalHistoryRelative> dataPersonalHistoryRelatives = dataGTPApplication.dataPersonalHistoryRelatives.Where(x => x.Active).ToList();
            model.dataPersonalHistoryRelatives = new List<dataPersonalHistoryRelativeUpdateModel>();
            model.dataPersonalHistoryRelatives = Mapper.Map(dataPersonalHistoryRelatives, new List<dataPersonalHistoryRelativeUpdateModel>());

            List<dataPersonalHistoryRelativeWorker> dataPersonalHistoryRelativeWorker = dataGTPApplication.dataPersonalHistoryRelativeWorkers.Where(x => x.Active).ToList();
            model.dataPersonalHistoryRelativeWorkers = new List<dataPersonalHistoryRelativeWorkerUpdateModel>();
            model.dataPersonalHistoryRelativeWorkers = Mapper.Map(dataPersonalHistoryRelativeWorker, new List<dataPersonalHistoryRelativeWorkerUpdateModel>());

            return PartialView("~/Areas/GTP/Views/Applications/PersonalHistoryFormWizard/_QuestionnaireWizardForm.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult QuestionnaireWizardCreate(dataPersonalHistoryQuestionnaireWithRelativesUpdateModel model)
        {
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();

            if (model.EighthQuestionAnswerDetails)
            {
                if (dataGTPApplication.dataPersonalHistoryRelatives.Where(x => x.Active).Count() == 0)
                {
                    ModelState.AddModelError("", "record details are required for question 8");
                    return PartialView("~/Areas/GTP/Views/Applications/PersonalHistoryFormWizard/_QuestionnaireWizardForm.cshtml", model);
                }
            }
            if (model.TwelveQuestionAnswerDetails)
            {
                ModelState.AddModelError("", "record details are required for question 12");
                if (dataGTPApplication.dataPersonalHistoryRelativeWorkers.Where(x => x.Active).Count() == 0)
                {
                    return PartialView("~/Areas/GTP/Views/Applications/PersonalHistoryFormWizard/_QuestionnaireWizardForm.cshtml", model);
                }
            }
            dataPersonalHistoryQuestionnaire dataPersonalHistoryQuestionnaire = new dataPersonalHistoryQuestionnaire();
            dataPersonalHistoryQuestionnaire = Mapper.Map(model, new dataPersonalHistoryQuestionnaire());
            dataPersonalHistoryQuestionnaire.GTPPHQuestionnaireGUID = Guid.NewGuid();
            dataPersonalHistoryQuestionnaire.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            dataPersonalHistoryQuestionnaire.Active = true;
            DbGTP.dataPersonalHistoryQuestionnaires.Add(dataPersonalHistoryQuestionnaire);

            if (dataPersonalHistoryQuestionnaire.EighthQuestionAnswerDetails == false)
            {
                var relatives = dataGTPApplication.dataPersonalHistoryRelatives.ToList();
                foreach (var record in relatives)
                {
                    record.Active = false;
                }
            }
            if (dataPersonalHistoryQuestionnaire.TwelveQuestionAnswerDetails == false)
            {
                var relatives = dataGTPApplication.dataPersonalHistoryRelativeWorkers.ToList();
                foreach (var record in relatives)
                {
                    record.Active = false;
                }
            }
            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();

                return Json(DbGTP.SingleCreateMessage("WizardHandling();", true));
            }
            catch (Exception ex)
            {
                return Json(DbGTP.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult QuestionnaireWizardUpdate(dataPersonalHistoryQuestionnaireWithRelativesUpdateModel model)
        {
            DateTime ExecutionTime = DateTime.Now;
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            if (model.EighthQuestionAnswerDetails)
            {
                if (dataGTPApplication.dataPersonalHistoryRelatives.Where(x => x.Active).Count() == 0)
                {
                    ModelState.AddModelError("", "record details are required for question 8");
                    return PartialView("~/Areas/GTP/Views/Applications/PersonalHistoryFormWizard/_QuestionnaireWizardForm.cshtml", model);
                }
            }
            if (model.TwelveQuestionAnswerDetails)
            {
                if (dataGTPApplication.dataPersonalHistoryRelativeWorkers.Where(x => x.Active).Count() == 0)
                {
                    ModelState.AddModelError("", "record details are required for question 12");
                    return PartialView("~/Areas/GTP/Views/Applications/PersonalHistoryFormWizard/_QuestionnaireWizardForm.cshtml", model);
                }
            }
            dataPersonalHistoryQuestionnaire dataPersonalHistoryQuestionnaire = new dataPersonalHistoryQuestionnaire();
            dataPersonalHistoryQuestionnaire = Mapper.Map(model, new dataPersonalHistoryQuestionnaire());
            dataPersonalHistoryQuestionnaire.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            dataPersonalHistoryQuestionnaire.Active = model.Active;

            var original = DbGTP.Set<dataPersonalHistoryQuestionnaire>().Find(dataPersonalHistoryQuestionnaire.GTPPHQuestionnaireGUID);
            DbGTP.Entry(original).CurrentValues.SetValues(dataPersonalHistoryQuestionnaire);
            bool Changed = false;
            var entry = DbGTP.Entry(original);
            var namesOfChangedProperties = entry.CurrentValues.PropertyNames.Where(p => entry.Property(p).IsModified && entry.Property(p).Name.EndsWith("RowVersion") == false && entry.Property(p).Name != "DeletedOn").ToList();
            if (namesOfChangedProperties.Count > 0)
            {
                Changed = true;
                dataPersonalHistoryConfirmationAndConsent dataPersonalHistoryConfirmationAndConsent = dataGTPApplication.dataPersonalHistoryConfirmationAndConsents.FirstOrDefault();
                if (dataPersonalHistoryConfirmationAndConsent != null)
                {
                    dataPersonalHistoryConfirmationAndConsent.IsConfirmedByUser = false;
                }
                dataGTPApplication.IsReviewed = false;
                dataGTPApplication.UpdatedOn = DateTime.Now;
            }

            DbGTP.Update(dataPersonalHistoryQuestionnaire, Permissions.Portalforgrouptwoapplications.UpdateGuid, ExecutionTime, DbCMS);
            if (dataPersonalHistoryQuestionnaire.EighthQuestionAnswerDetails == false)
            {
                var relatives = dataGTPApplication.dataPersonalHistoryRelatives.ToList();
                foreach (var record in relatives)
                {
                    record.Active = false;
                }
            }
            if (dataPersonalHistoryQuestionnaire.TwelveQuestionAnswerDetails == false)
            {
                var relatives = dataGTPApplication.dataPersonalHistoryRelativeWorkers.ToList();
                foreach (var record in relatives)
                {
                    record.Active = false;
                }
            }

            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();
                if (Changed)
                {
                    return Json(DbGTP.SingleCreateMessage("WizardHandling(); RequireConfirmation();", true));
                }
                return Json(DbGTP.SingleCreateMessage("WizardHandling();", true));
            }
            catch (Exception ex)
            {
                return Json(DbGTP.ErrorMessage(ex.Message));
            }
        }

        #region Relatives

        public ActionResult RelativesCreate()
        {
            dataPersonalHistoryRelativeUpdateModel dataPersonalHistoryRelativeUpdateModel = new dataPersonalHistoryRelativeUpdateModel();
            return PartialView("~/Areas/GTP/Views/Applications/PersonalHistoryFormWizard/_RelativesUpdateModalForm.cshtml", dataPersonalHistoryRelativeUpdateModel);
        }

        public ActionResult RelativesUpdate(Guid PK)
        {
            dataPersonalHistoryRelativeUpdateModel model = new dataPersonalHistoryRelativeUpdateModel();
            dataPersonalHistoryRelative dataPersonalHistoryRelative = (from a in DbGTP.dataPersonalHistoryRelatives where a.GTPPHRelativeGUID == PK select a).FirstOrDefault();
            model = Mapper.Map(dataPersonalHistoryRelative, new dataPersonalHistoryRelativeUpdateModel());
            return PartialView("~/Areas/GTP/Views/Applications/PersonalHistoryFormWizard/_RelativesUpdateModalForm.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult RelativesCreate(dataPersonalHistoryRelativeUpdateModel model)
        {
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistoryRelative dataPersonalHistoryRelative = new dataPersonalHistoryRelative();
            dataPersonalHistoryRelative = Mapper.Map(model, new dataPersonalHistoryRelative());
            dataPersonalHistoryRelative.GTPPHRelativeGUID = Guid.NewGuid();
            dataPersonalHistoryRelative.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            dataPersonalHistoryRelative.Active = true;
            DbGTP.dataPersonalHistoryRelatives.Add(dataPersonalHistoryRelative);
            dataGTPApplication.IsReviewed = false;
            dataGTPApplication.UpdatedOn = DateTime.Now;
            dataPersonalHistoryQuestionnaire dataPersonalHistoryQuestionnaire = dataGTPApplication.dataPersonalHistoryQuestionnaires.FirstOrDefault();
            if (dataPersonalHistoryQuestionnaire == null)
            {
                dataPersonalHistoryQuestionnaire = new dataPersonalHistoryQuestionnaire();
                dataPersonalHistoryQuestionnaire.GTPPHQuestionnaireGUID = Guid.NewGuid();
                dataPersonalHistoryQuestionnaire.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
                dataPersonalHistoryQuestionnaire.Active = true;
            }
            else
            {
                dataGTPApplication.dataPersonalHistoryQuestionnaires.FirstOrDefault().EighthQuestionAnswerDetails = true;
            }
            dataPersonalHistoryConfirmationAndConsent dataPersonalHistoryConfirmationAndConsent = dataGTPApplication.dataPersonalHistoryConfirmationAndConsents.FirstOrDefault();
            if (dataPersonalHistoryConfirmationAndConsent != null)
            {
                dataPersonalHistoryConfirmationAndConsent.IsConfirmedByUser = false;
            }

            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();

                //List<RowVersionControl> Result = new List<RowVersionControl>();
                //Result.Add(new RowVersionControl
                //{
                //    ControlID = "dataPersonalHistoryQuestionnaires",
                //    Value = Convert.ToBase64String(dataGTPApplication.dataPersonalHistoryQuestionnaires.FirstOrDefault().dataPersonalHistoryQuestionnaireRowVersion)
                //});

                return Json(DbGTP.SingleCreateMessage("ReloadRelatives(); RequireConfirmation();", true));
            }
            catch (Exception ex)
            {
                return Json(DbGTP.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult RelativesUpdate(dataPersonalHistoryRelativeUpdateModel model)
        {
            DateTime ExecutionTime = DateTime.Now;
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistoryRelative dataPersonalHistoryRelative = new dataPersonalHistoryRelative();
            dataPersonalHistoryRelative = Mapper.Map(model, new dataPersonalHistoryRelative());
            dataPersonalHistoryRelative.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            dataPersonalHistoryRelative.Active = model.Active;

            var original = DbGTP.Set<dataPersonalHistoryRelative>().Find(dataPersonalHistoryRelative.GTPPHRelativeGUID);
            DbGTP.Entry(original).CurrentValues.SetValues(dataPersonalHistoryRelative);
            bool Changed = false;
            var entry = DbGTP.Entry(original);
            var namesOfChangedProperties = entry.CurrentValues.PropertyNames.Where(p => entry.Property(p).IsModified && entry.Property(p).Name.EndsWith("RowVersion") == false && entry.Property(p).Name != "DeletedOn").ToList();
            if (namesOfChangedProperties.Count > 0)
            {
                Changed = true;
                dataPersonalHistoryConfirmationAndConsent dataPersonalHistoryConfirmationAndConsent = dataGTPApplication.dataPersonalHistoryConfirmationAndConsents.FirstOrDefault();
                if (dataPersonalHistoryConfirmationAndConsent != null)
                {
                    dataPersonalHistoryConfirmationAndConsent.IsConfirmedByUser = false;
                }
                dataGTPApplication.IsReviewed = false;
                dataGTPApplication.UpdatedOn = DateTime.Now;
            }

            DbGTP.Update(dataPersonalHistoryRelative, Permissions.Portalforgrouptwoapplications.UpdateGuid, ExecutionTime, DbCMS);

            dataGTPApplication.dataPersonalHistoryQuestionnaires.FirstOrDefault().EighthQuestionAnswerDetails = true;


            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();

                return Json(DbGTP.SingleCreateMessage("ReloadRelatives(); RequireConfirmation(); RefreshQuestionnaireRV('" + Convert.ToBase64String(dataGTPApplication.dataPersonalHistoryQuestionnaires.FirstOrDefault().dataPersonalHistoryQuestionnaireRowVersion) + "');", true));
            }
            catch (Exception ex)
            {
                return Json(DbGTP.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult RelativesDelete(dataPersonalHistoryRelative model)
        {
            DateTime ExecutionTime = DateTime.Now;
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistoryRelative dataPersonalHistoryRelative = (from a in DbGTP.dataPersonalHistoryRelatives where a.GTPPHRelativeGUID == model.GTPPHRelativeGUID select a).FirstOrDefault();
            dataPersonalHistoryRelative.Active = false;
            dataGTPApplication.IsReviewed = false;
            dataGTPApplication.UpdatedOn = DateTime.Now;

            if (dataGTPApplication.dataPersonalHistoryRelatives.Where(x => x.Active).Count() == 0)
            {
                if (dataGTPApplication.dataPersonalHistoryQuestionnaires != null)
                {
                    dataGTPApplication.dataPersonalHistoryQuestionnaires.FirstOrDefault().EighthQuestionAnswerDetails = false;
                }
            }

            dataPersonalHistoryConfirmationAndConsent dataPersonalHistoryConfirmationAndConsent = dataGTPApplication.dataPersonalHistoryConfirmationAndConsents.FirstOrDefault();
            if (dataPersonalHistoryConfirmationAndConsent != null)
            {
                dataPersonalHistoryConfirmationAndConsent.IsConfirmedByUser = false;
            }

            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbGTP.SingleCreateMessage("ReloadRelatives(); RequireConfirmation(); RefreshQuestionnaireRV('" + Convert.ToBase64String(dataGTPApplication.dataPersonalHistoryQuestionnaires.FirstOrDefault().dataPersonalHistoryQuestionnaireRowVersion) + "');", true));
            }
            catch (Exception ex)
            {
                return Json(DbGTP.ErrorMessage(ex.Message));
            }
        }

        [Route("GTP/Applications/ReloadRelativesTable")]
        public ActionResult ReloadRelativesTable()
        {
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistoryQuestionnaireWithRelativesUpdateModel model = new dataPersonalHistoryQuestionnaireWithRelativesUpdateModel();
            model.dataPersonalHistoryRelatives = new List<dataPersonalHistoryRelativeUpdateModel>();
            List<dataPersonalHistoryRelative> dataPersonalHistoryRelatives = (from a in DbGTP.dataPersonalHistoryRelatives where a.GTPApplicationGUID == dataGTPApplication.GTPApplicationGUID select a).ToList();
            model.dataPersonalHistoryRelatives = Mapper.Map(dataPersonalHistoryRelatives, new List<dataPersonalHistoryRelativeUpdateModel>());
            return PartialView("~/Areas/GTP/Views/Applications/PersonalHistoryFormWizard/_RelativesTable.cshtml", model);
        }

        #endregion

        #region Relatives Workers
        public ActionResult RelativesWorkersCreate()
        {
            dataPersonalHistoryRelativeWorkerUpdateModel dataPersonalHistoryRelativeWorkerUpdateModel = new dataPersonalHistoryRelativeWorkerUpdateModel();
            return PartialView("~/Areas/GTP/Views/Applications/PersonalHistoryFormWizard/_RelativesWorkersUpdateModalForm.cshtml", dataPersonalHistoryRelativeWorkerUpdateModel);
        }

        public ActionResult RelativesWorkersUpdate(Guid PK)
        {
            dataPersonalHistoryRelativeWorkerUpdateModel model = new dataPersonalHistoryRelativeWorkerUpdateModel();
            dataPersonalHistoryRelativeWorker dataPersonalHistoryRelativeWorker = (from a in DbGTP.dataPersonalHistoryRelativeWorkers where a.GTPPHRelativeWorkerGUID == PK select a).FirstOrDefault();
            model = Mapper.Map(dataPersonalHistoryRelativeWorker, new dataPersonalHistoryRelativeWorkerUpdateModel());
            return PartialView("~/Areas/GTP/Views/Applications/PersonalHistoryFormWizard/_RelativesWorkersUpdateModalForm.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult RelativesWorkersCreate(dataPersonalHistoryRelativeWorkerUpdateModel model)
        {
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistoryRelativeWorker dataPersonalHistoryRelativeWorker = new dataPersonalHistoryRelativeWorker();
            dataPersonalHistoryRelativeWorker = Mapper.Map(model, new dataPersonalHistoryRelativeWorker());
            dataPersonalHistoryRelativeWorker.GTPPHRelativeWorkerGUID = Guid.NewGuid();
            dataPersonalHistoryRelativeWorker.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            dataPersonalHistoryRelativeWorker.Active = true;
            DbGTP.dataPersonalHistoryRelativeWorkers.Add(dataPersonalHistoryRelativeWorker);
            dataGTPApplication.IsReviewed = false;
            dataGTPApplication.UpdatedOn = DateTime.Now;
            dataPersonalHistoryConfirmationAndConsent dataPersonalHistoryConfirmationAndConsent = dataGTPApplication.dataPersonalHistoryConfirmationAndConsents.FirstOrDefault();
            if (dataPersonalHistoryConfirmationAndConsent != null)
            {
                dataPersonalHistoryConfirmationAndConsent.IsConfirmedByUser = false;
            }
            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();

                return Json(DbGTP.SingleCreateMessage("ReloadRelativesWorkers(); RequireConfirmation();", true));
            }
            catch (Exception ex)
            {
                return Json(DbGTP.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult RelativesWorkersUpdate(dataPersonalHistoryRelativeWorkerUpdateModel model)
        {
            DateTime ExecutionTime = DateTime.Now;
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistoryRelativeWorker dataPersonalHistoryRelativeWorker = new dataPersonalHistoryRelativeWorker();
            dataPersonalHistoryRelativeWorker = Mapper.Map(model, new dataPersonalHistoryRelativeWorker());
            dataPersonalHistoryRelativeWorker.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            dataPersonalHistoryRelativeWorker.Active = model.Active;

            var original = DbGTP.Set<dataPersonalHistoryRelativeWorker>().Find(dataPersonalHistoryRelativeWorker.GTPPHRelativeWorkerGUID);
            DbGTP.Entry(original).CurrentValues.SetValues(dataPersonalHistoryRelativeWorker);
            bool Changed = false;
            var entry = DbGTP.Entry(original);
            var namesOfChangedProperties = entry.CurrentValues.PropertyNames.Where(p => entry.Property(p).IsModified && entry.Property(p).Name.EndsWith("RowVersion") == false && entry.Property(p).Name != "DeletedOn").ToList();
            if (namesOfChangedProperties.Count > 0)
            {
                Changed = true;
                dataPersonalHistoryConfirmationAndConsent dataPersonalHistoryConfirmationAndConsent = dataGTPApplication.dataPersonalHistoryConfirmationAndConsents.FirstOrDefault();
                if (dataPersonalHistoryConfirmationAndConsent != null)
                {
                    dataPersonalHistoryConfirmationAndConsent.IsConfirmedByUser = false;
                }
                dataGTPApplication.IsReviewed = false;
                dataGTPApplication.UpdatedOn = DateTime.Now;
            }

            DbGTP.Update(dataPersonalHistoryRelativeWorker, Permissions.Portalforgrouptwoapplications.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();

                return Json(DbGTP.SingleCreateMessage("ReloadRelativesWorkers(); RequireConfirmation(); RefreshQuestionnaireRV('" + Convert.ToBase64String(dataGTPApplication.dataPersonalHistoryQuestionnaires.FirstOrDefault().dataPersonalHistoryQuestionnaireRowVersion) + "');", true));
            }
            catch (Exception ex)
            {
                return Json(DbGTP.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult RelativesWorkersDelete(dataPersonalHistoryRelativeWorker model)
        {
            DateTime ExecutionTime = DateTime.Now;
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistoryRelativeWorker dataPersonalHistoryRelativeWorker = (from a in DbGTP.dataPersonalHistoryRelativeWorkers where a.GTPPHRelativeWorkerGUID == model.GTPPHRelativeWorkerGUID select a).FirstOrDefault();
            dataPersonalHistoryRelativeWorker.Active = false;
            dataGTPApplication.IsReviewed = false;
            dataGTPApplication.UpdatedOn = DateTime.Now;
            if (dataGTPApplication.dataPersonalHistoryRelativeWorkers.Where(x => x.Active).Count() == 0)
            {
                if (dataGTPApplication.dataPersonalHistoryQuestionnaires != null)
                {
                    dataGTPApplication.dataPersonalHistoryQuestionnaires.FirstOrDefault().TwelveQuestionAnswerDetails = false;
                }
            }

            dataPersonalHistoryConfirmationAndConsent dataPersonalHistoryConfirmationAndConsent = dataGTPApplication.dataPersonalHistoryConfirmationAndConsents.FirstOrDefault();
            if (dataPersonalHistoryConfirmationAndConsent != null)
            {
                dataPersonalHistoryConfirmationAndConsent.IsConfirmedByUser = false;
            }

            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbGTP.SingleCreateMessage("ReloadRelativesWorkers(); RequireConfirmation(); RefreshQuestionnaireRV('" + Convert.ToBase64String(dataGTPApplication.dataPersonalHistoryQuestionnaires.FirstOrDefault().dataPersonalHistoryQuestionnaireRowVersion) + "');", true));
            }
            catch (Exception ex)
            {
                return Json(DbGTP.ErrorMessage(ex.Message));
            }
        }

        [Route("GTP/Applications/ReloadRelativesWorkersTable")]
        public ActionResult ReloadRelativesWorkersTable()
        {
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistoryQuestionnaireWithRelativesUpdateModel model = new dataPersonalHistoryQuestionnaireWithRelativesUpdateModel();
            model.dataPersonalHistoryRelativeWorkers = new List<dataPersonalHistoryRelativeWorkerUpdateModel>();
            List<dataPersonalHistoryRelativeWorker> dataPersonalHistoryRelativeWorkers = (from a in DbGTP.dataPersonalHistoryRelativeWorkers where a.GTPApplicationGUID == dataGTPApplication.GTPApplicationGUID select a).ToList();
            model.dataPersonalHistoryRelativeWorkers = Mapper.Map(dataPersonalHistoryRelativeWorkers, new List<dataPersonalHistoryRelativeWorkerUpdateModel>());
            return PartialView("~/Areas/GTP/Views/Applications/PersonalHistoryFormWizard/_RelativesWorkersTable.cshtml", model);
        }
        #endregion

        #endregion

        #region Section 16  CONFIRMATION AND CONSENT
        [Route("GTP/Applications/ConfirmationAndConsentWizard/Create")]
        public ActionResult ConfirmationAndConsentWizardCreate()
        {
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistoryConfirmationAndConsentUpdateModel model = new dataPersonalHistoryConfirmationAndConsentUpdateModel();
            model = Mapper.Map(dataGTPApplication.dataPersonalHistoryConfirmationAndConsents.FirstOrDefault(), new dataPersonalHistoryConfirmationAndConsentUpdateModel());
            if (model == null)
            {
                model = new dataPersonalHistoryConfirmationAndConsentUpdateModel();
            }
            model.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            model.FormHasError = false;
            //1 general info
            if (dataGTPApplication.dataPersonalHistoryGeneralInfoes.Where(x => x.Active).FirstOrDefault() == null)
            {
                model.FormHasError = true;
                ModelState.AddModelError("", "General Information Missing!");
            }
            //2 personal info
            if (dataGTPApplication.dataPersonalHistoryPersonalInfoes.Where(x => x.Active).FirstOrDefault() == null)
            {
                model.FormHasError = true;
                ModelState.AddModelError("", "Personal Information Missing!");
            }
            //1 contact info
            if (dataGTPApplication.dataPersonalHistoryContactInfoes.Where(x => x.Active).FirstOrDefault() == null)
            {
                model.FormHasError = true;
                ModelState.AddModelError("", "Contact Information Missing!");
            }
            //1 phone info
            if (dataGTPApplication.dataPersonalHistoryPhoneNumbers.Where(x => x.Active).FirstOrDefault() == null)
            {
                model.FormHasError = true;
                ModelState.AddModelError("", "Phone Numbers Missing!");
            }
            //1 email info
            if (dataGTPApplication.dataPersonalHistoryEmailAddresses.Where(x => x.Active).FirstOrDefault() == null)
            {
                model.FormHasError = true;
                ModelState.AddModelError("", "Email Addresses Missing!");
            }
            //1 nationalities info
            if (dataGTPApplication.dataPersonalHistoryNationalityInfoes.Where(x => x.Active).FirstOrDefault() == null)
            {
                model.FormHasError = true;
                ModelState.AddModelError("", "Nationality Information Missing!");
            }
            //1 letter of interest info
            //1 work experience info
            //1 specialized trainings info
            //1 education info
            if (dataGTPApplication.dataPersonalHistoryEducations.Where(x => x.Active).FirstOrDefault() == null)
            {
                model.FormHasError = true;
                ModelState.AddModelError("", "Education Missing!");
            }
            //1 skills info
            //1 languages info
            if (dataGTPApplication.dataPersonalHistoryLanguages.Where(x => x.Active).FirstOrDefault() == null)
            {
                model.FormHasError = true;
                ModelState.AddModelError("", "Languages Missing!");
            }
            //1 licences info
            //1 professionals info
            if (dataGTPApplication.dataPersonalHistoryProfessionalReferences.Where(x => x.Active).FirstOrDefault() == null)
            {
                model.FormHasError = true;
                ModelState.AddModelError("", "Professional References Missing!");
            }
            //1 questionair info
            //1 relatives info
            //1 relative workers info
            var questionnaire = dataGTPApplication.dataPersonalHistoryQuestionnaires.Where(x => x.Active).FirstOrDefault();
            if (questionnaire == null)
            {
                model.FormHasError = true;
                ModelState.AddModelError("", "Questionnaire Missing!");
            }
            if (questionnaire != null && questionnaire.EighthQuestionAnswerDetails)
            {
                if (dataGTPApplication.dataPersonalHistoryRelatives.Where(x => x.Active).Count() == 0)
                {
                    model.FormHasError = true;
                    ModelState.AddModelError("", "Record details are required for question 8 in questionnaire!");
                }
            }
            if (questionnaire != null && questionnaire.TwelveQuestionAnswerDetails)
            {
                if (dataGTPApplication.dataPersonalHistoryRelativeWorkers.Where(x => x.Active).Count() == 0)
                {
                    model.FormHasError = true;
                    ModelState.AddModelError("", "Record details are required for question 12 in questionnaire!");
                }
            }


            return PartialView("~/Areas/GTP/Views/Applications/PersonalHistoryFormWizard/_ConfirmationAndConsentWizardForm.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult ConfirmationAndConsentWizardCreate(dataPersonalHistoryConfirmationAndConsentUpdateModel model)
        {
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            //Check if P11 is finalized
            model.FormHasError = false;
            //1 general info
            if (dataGTPApplication.dataPersonalHistoryGeneralInfoes.Where(x => x.Active).FirstOrDefault() == null)
            {
                model.FormHasError = true;
                ModelState.AddModelError("", "General Information Missing!");
            }
            //2 personal info
            if (dataGTPApplication.dataPersonalHistoryPersonalInfoes.Where(x => x.Active).FirstOrDefault() == null)
            {
                model.FormHasError = true;
                ModelState.AddModelError("", "Personal Information Missing!");
            }
            //1 contact info
            if (dataGTPApplication.dataPersonalHistoryContactInfoes.Where(x => x.Active).FirstOrDefault() == null)
            {
                model.FormHasError = true;
                ModelState.AddModelError("", "Contact Information Missing!");
            }
            //1 phone info
            if (dataGTPApplication.dataPersonalHistoryPhoneNumbers.Where(x => x.Active).FirstOrDefault() == null)
            {
                model.FormHasError = true;
                ModelState.AddModelError("", "Phone Numbers Missing!");
            }
            //1 email info
            if (dataGTPApplication.dataPersonalHistoryEmailAddresses.Where(x => x.Active).FirstOrDefault() == null)
            {
                model.FormHasError = true;
                ModelState.AddModelError("", "Email Addresses Missing!");
            }
            //1 nationalities info
            if (dataGTPApplication.dataPersonalHistoryNationalityInfoes.Where(x => x.Active).FirstOrDefault() == null)
            {
                model.FormHasError = true;
                ModelState.AddModelError("", "Nationality Information Missing!");
            }
            //1 letter of interest info
            //1 work experience info
            //1 specialized trainings info
            //1 education info
            if (dataGTPApplication.dataPersonalHistoryEducations.Where(x => x.Active).FirstOrDefault() == null)
            {
                model.FormHasError = true;
                ModelState.AddModelError("", "Education Missing!");
            }
            //1 skills info
            //1 languages info
            if (dataGTPApplication.dataPersonalHistoryLanguages.Where(x => x.Active).FirstOrDefault() == null)
            {
                model.FormHasError = true;
                ModelState.AddModelError("", "Languages Missing!");
            }
            //1 licences info
            //1 professionals info
            if (dataGTPApplication.dataPersonalHistoryProfessionalReferences.Where(x => x.Active).FirstOrDefault() == null)
            {
                model.FormHasError = true;
                ModelState.AddModelError("", "Professional References Missing!");
            }
            //1 questionair info
            //1 relatives info
            //1 relative workers info
            var questionnaire = dataGTPApplication.dataPersonalHistoryQuestionnaires.Where(x => x.Active).FirstOrDefault();
            if (questionnaire == null)
            {
                model.FormHasError = true;
                ModelState.AddModelError("", "Questionnaire Missing!");
            }
            if (questionnaire != null && questionnaire.EighthQuestionAnswerDetails)
            {
                if (dataGTPApplication.dataPersonalHistoryRelatives.Where(x => x.Active).Count() == 0)
                {
                    model.FormHasError = true;
                    ModelState.AddModelError("", "Record details are required for question 8 in questionnaire!");
                }
            }
            if (questionnaire != null && questionnaire.TwelveQuestionAnswerDetails)
            {
                if (dataGTPApplication.dataPersonalHistoryRelativeWorkers.Where(x => x.Active).Count() == 0)
                {
                    model.FormHasError = true;
                    ModelState.AddModelError("", "Record details are required for question 12 in questionnaire!");
                }
            }
            if (model.FormHasError)
            {
                return PartialView("~/Areas/GTP/Views/Applications/PersonalHistoryFormWizard/_ConfirmationAndConsentWizardForm.cshtml", model);
            }


            dataPersonalHistoryConfirmationAndConsent dataPersonalHistoryConfirmationAndConsent = new dataPersonalHistoryConfirmationAndConsent();
            dataPersonalHistoryConfirmationAndConsent = Mapper.Map(model, new dataPersonalHistoryConfirmationAndConsent());
            dataPersonalHistoryConfirmationAndConsent.GTPPHConfirmationConsentGUID = Guid.NewGuid();
            dataPersonalHistoryConfirmationAndConsent.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            dataPersonalHistoryConfirmationAndConsent.Active = true;
            dataPersonalHistoryConfirmationAndConsent.ConfirmationDate = DateTime.Now;

            DbGTP.dataPersonalHistoryConfirmationAndConsents.Add(dataPersonalHistoryConfirmationAndConsent);

            dataGTPApplication.IsReviewed = false;
            dataGTPApplication.UpdatedOn = DateTime.Now;
            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();

                //send email

                string emailAddress = (from a in DbCMS.userServiceHistory where a.UserGUID == UserGUID select a.EmailAddress).FirstOrDefault();
                string fullName = (from a in DbCMS.userPersonalDetailsLanguage where a.UserGUID == UserGUID && a.Active && a.LanguageID == "EN" select a.FirstName + " " + a.Surname).FirstOrDefault();
                dataPersonalHistoryEmailAddress dataPersonalHistoryEmailAddress = (from a in DbGTP.dataPersonalHistoryEmailAddresses where a.GTPApplicationGUID == dataGTPApplication.GTPApplicationGUID select a).First();
                string preferredEamilAddress = "";
                if (dataPersonalHistoryEmailAddress.PreferedEmailAddress == 1)
                {
                    preferredEamilAddress = dataPersonalHistoryEmailAddress.HomeEmailAddress;
                }
                else
                {
                    preferredEamilAddress = dataPersonalHistoryEmailAddress.BusinessEmailAddress;
                }
                new Email().SendGTPConfirmReceivingApplicationEmail(preferredEamilAddress, emailAddress, fullName, "Email confirmation of receiving the application");

                string prefix = "";
                string gender = dataGTPApplication.dataPersonalHistoryPersonalInfoes.FirstOrDefault().Gender;
                if (gender == "M")
                {
                    prefix = "Mr. ";
                }
                else if (gender == "F")
                {
                    prefix = "Ms. ";
                }
                //Notify to HR
                new Email().SendGTPNewApplicationHRNotification("SYRDAHUMANRES@unhcr.org", null, prefix + fullName, "New group 2 application is completed");

                return Json(DbGTP.SingleCreateMessage("GoToConfirmation();", true));
            }
            catch (Exception ex)
            {
                return Json(DbGTP.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult ConfirmationAndConsentWizardUpdate(dataPersonalHistoryConfirmationAndConsentUpdateModel model)
        {
            DateTime ExecutionTime = DateTime.Now;
            dataGTPApplication dataGTPApplication = (from a in DbGTP.dataGTPApplications where a.UserGUID == UserGUID select a).FirstOrDefault();
            dataPersonalHistoryConfirmationAndConsent dataPersonalHistoryConfirmationAndConsent = new dataPersonalHistoryConfirmationAndConsent();
            dataPersonalHistoryConfirmationAndConsent = Mapper.Map(model, new dataPersonalHistoryConfirmationAndConsent());
            dataPersonalHistoryConfirmationAndConsent.GTPApplicationGUID = dataGTPApplication.GTPApplicationGUID;
            dataPersonalHistoryConfirmationAndConsent.Active = model.Active;
            dataPersonalHistoryConfirmationAndConsent.ConfirmationDate = DateTime.Now;

            var original = DbGTP.Set<dataPersonalHistoryConfirmationAndConsent>().Find(dataPersonalHistoryConfirmationAndConsent.GTPPHConfirmationConsentGUID);
            DbGTP.Entry(original).CurrentValues.SetValues(dataPersonalHistoryConfirmationAndConsent);
            bool Changed = false;
            var entry = DbGTP.Entry(original);
            var namesOfChangedProperties = entry.CurrentValues.PropertyNames.Where(p => entry.Property(p).IsModified && entry.Property(p).Name.EndsWith("RowVersion") == false && entry.Property(p).Name != "DeletedOn").ToList();
            if (namesOfChangedProperties.Count > 0)
            {
                foreach (var item in namesOfChangedProperties)
                {
                    if (item == "IsConfirmedByUser")
                    {
                        Changed = true;
                        dataGTPApplication.IsReviewed = false;
                        dataGTPApplication.UpdatedOn = DateTime.Now;
                    }
                }

            }

            DbGTP.Update(dataPersonalHistoryConfirmationAndConsent, Permissions.Portalforgrouptwoapplications.UpdateGuid, ExecutionTime, DbCMS);

            if (Changed)
            {
                string fullName = (from a in DbCMS.userPersonalDetailsLanguage where a.UserGUID == UserGUID && a.Active && a.LanguageID == "EN" select a.FirstName + " " + a.Surname).FirstOrDefault();

                string prefix = "";
                string gender = dataGTPApplication.dataPersonalHistoryPersonalInfoes.FirstOrDefault().Gender;
                if (gender == "M")
                {
                    prefix = "Mr. ";
                }
                else if (gender == "F")
                {
                    prefix = "Ms. ";
                }
                //Notify to HR
                new Email().SendGTPUpdateApplicationHRNotification("SYRDAHUMANRES@unhcr.org", null, prefix + fullName, "Group 2 application is updated");

            }

            try
            {
                DbGTP.SaveChanges();
                DbCMS.SaveChanges();

                return Json(DbGTP.SingleCreateMessage("GoToConfirmationUpdate();", true));
            }
            catch (Exception ex)
            {
                return Json(DbGTP.ErrorMessage(ex.Message));
            }
        }
        #endregion



        #region Helpers
        public JsonResult GetCitiesByCountry(Guid CountryGUID)
        {
            var Result = DropDownList.GTPCities(CountryGUID);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}