using AppsPortal.BaseControllers;
using AppsPortal.Data;
using AppsPortal.Extensions;
using RES_Repo.Globalization;
using AppsPortal.Library;
using AppsPortal.Models;
using AppsPortal.ViewModels;
using AutoMapper;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Controllers
{
    [AllowAnonymous]
    public class RegistrationController : PortalBaseController
    {
        [HttpGet]
        public ActionResult Register()
        {
            return null;
            ViewBag.Questions = DropDownList.SecurityQuestions();
            ViewBag.TimeZones = DropDownList.TimeZones();
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Register(AccountDetailsModel model)
        {
            return null;
            if (ModelState.IsValid)
            {
                model.UserRegistrationQueueGUID = Guid.NewGuid();
                model.UserGUID = Guid.NewGuid();
                
                userRegistrationQueue RegRequest = Mapper.Map<userRegistrationQueue>(model);
                RegRequest.SponsorConfirmed = false;
                //No Audit Involved, Add data to the db directly
                DbCMS.userRegistrationQueue.Add(RegRequest);

                try
                {
                    DbCMS.SaveChanges();
                    new Email().SendVerifyPrimaryEmail(RegRequest);
                    return RedirectToAction("RegisterEmailVerificationMessage", "Registration", new { PK = Portal.GUIDToString(model.UserRegistrationQueueGUID) });
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorSummary = ex.Message;
                    ViewBag.Questions = DropDownList.SecurityQuestions();
                    ViewBag.TimeZones = DropDownList.TimeZones();
                    return View("Register", model);
                }
            }
            else
            {
                return Register(model);
            }
        }

        public ActionResult RegisterEmailVerificationMessage(string PK)
        {
            Guid RequestGUID = Portal.StringToGUID(PK);
            userRegistrationQueue model = DbCMS.userRegistrationQueue.Find(RequestGUID);
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult RegisterVerifyEmail(RegisterVerifyEmail model, string PK)
        {
            if (ModelState.IsValid)
            {
                Session[SessionKeys.EmailVerificationCounter] = Session[SessionKeys.EmailVerificationCounter] ?? 0;

                Guid RequestGUID = Portal.StringToGUID(PK);
                var Request = DbCMS.userRegistrationQueue.Find(RequestGUID);

                if (model.SecurityAnswer.ToUpper() == Request.SecurityAnswer.ToUpper())
                {
                    if (Request.EmailAddress.Contains("@unhcr.org"))
                    {
                        //User is unhcr, go to setup operation and no need for sponsor
                        Request.OrganizationGUID = UNHCR.GUID;
                        Request.AccountStatusID = AccountStatus.Step02_EmailVerified;
                    }
                    else
                    {
                        //user does not have unhcr email so ask for his organization
                        //SetupOrganizationModel soModel = new SetupOrganizationModel();
                        Request.AccountStatusID = AccountStatus.Step02_EmailVerified;
                    }
                    DbCMS.SaveChanges();
                    return RedirectToAction("SetupOrganization", new { PK = PK });
                }
                else
                {
                    //Try again up to 3 times then delete the request // user is trying to access someone else request
                    Session[SessionKeys.EmailVerificationCounter] = (int)(Session[SessionKeys.EmailVerificationCounter]) + 1;
                    if ((int)Session[SessionKeys.EmailVerificationCounter] > 3)
                    {
                        //Delete the record from the data base and tell the user he fails to continue, 
                        //if he is gandon forget his answer, he needs to register again and remember his security answer
                        var IntroderOrIdiot = DbCMS.userRegistrationQueue.Where(u => u.UserRegistrationQueueGUID == RequestGUID && u.AccountStatusID == AccountStatus.Step01_RegistrationRequestSubmitted).FirstOrDefault();
                        DbCMS.userRegistrationQueue.Remove(IntroderOrIdiot);
                        DbCMS.SaveChanges();
                        //Rest the session!
                        Session[SessionKeys.EmailVerificationCounter] = 0;
                        return RedirectToAction("RegistrationFailed");
                    }
                    else
                    {
                        //start over and give another chance.
                        ViewBag.ErrorSummary = "Security Answer is not correct. Try Number #" + Session[SessionKeys.EmailVerificationCounter];//Get string from the resource. Ayas
                        return RegisterVerifyEmail(PK);
                    }
                }
            }
            return View();
        } //Passed

        public ActionResult RegisterVerifyEmail(string PK) //Passed
        {
            Guid RequestGUID = Guid.Parse(PK);
            userRegistrationQueue Request = DbCMS.userRegistrationQueue.Find(RequestGUID);
            if (Request != null)
            {
                ViewBag.Question = DbCMS.codeTablesValuesLanguages.Where(x => x.ValueGUID == Request.SecurityQuestionGUID && x.LanguageID == LAN).Select(q => new { q.ValueDescription }).FirstOrDefault().ValueDescription;
                switch (Request.AccountStatusID)
                {
                    case 1:
                        return View();
                    case 2:
                        return RedirectToAction("SetupOrganization", new { id = Portal.GUIDToString(RequestGUID) });
                    case 3:
                        return RedirectToAction("SetupOrganization", new { id = Portal.GUIDToString(RequestGUID) });
                    case 4:
                        return RedirectToAction("SetupPassword", new { id = Portal.GUIDToString(RequestGUID) });
                    default:
                        return View();
                }
            }
            else
            {
                throw new HttpException(404, "Request is not exists");
            }
        }

        public ActionResult ReviewRequest(string PK)
        {
            Guid RequestGUID = Guid.Parse(PK);
            var userRegistrationQueue = DbCMS.userRegistrationQueue.Where(x => x.UserGUID == RequestGUID).FirstOrDefault();
            if (userRegistrationQueue == null) { return View("RemoveRequest"); }
            if (userRegistrationQueue.SponsorConfirmed) { return View("ReviewRequestCompleted"); }
            var model =
              (from a in DbCMS.userRegistrationQueue.Where(x => x.UserGUID == RequestGUID)
               join b in DbCMS.codeJobTitlesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.JobTitleGUID equals b.JobTitleGUID into LJ1
               from R1 in LJ1.DefaultIfEmpty()
               join b in DbCMS.codeOrganizationsInstancesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.OrganizationInstanceGUID equals b.OrganizationInstanceGUID into LJ2
               from R2 in LJ2.DefaultIfEmpty()
               select new RegistrationQueueView
               {
                   ConfirmedRegistrationQueue=false,
                   EmailAddress=a.EmailAddress,
                   FirstName=a.FirstName,
                   Surname=a.Surname,
                   RequestedOn=a.RequestedOn.ToString(),
                   UserGUID=a.UserGUID,
                   UserRegistrationQueueGUID=a.UserRegistrationQueueGUID,
                   JobTitleDescription=R1.JobTitleDescription,
                   OrganizationInstanceDiscription=R2.OrganizationInstanceDescription
               }
              ).FirstOrDefault();
            if (Request != null)
            {
                return View("ReviewRequest", model);
            }
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult RegisterConfirm(RegistrationQueueView model)
        {
            if (model.ConfirmedRegistrationQueue)
            {
                var userRegistrationQueue = DbCMS.userRegistrationQueue.Find(model.UserRegistrationQueueGUID);
                new Email().SendSponsorConfirmedAccount(userRegistrationQueue);
                userRegistrationQueue.SponsorConfirmed = true;
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(null, null, null, "LoadUrl('' , '" + Url.Action("ReviewRequestCompleted", "Registration") + "')"));
            }
            else
            {
                return Json(DbCMS.ErrorMessage(resxMessages.ConfirmResponsibility));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult RegisterDeny(RegistrationQueueView model)
        {
            userRegistrationQueue userRegistrationQueue = DbCMS.userRegistrationQueue.Find(model.UserRegistrationQueueGUID);
            new Email().SendChangeSponser(userRegistrationQueue);
            userRegistrationQueue.SponsorConfirmed = true;
            DbCMS.SaveChanges();
            return Json(DbCMS.SingleUpdateMessage(null, null, null, "LoadUrl('' , '" + Url.Action("ReviewRequestCompleted", "Registration") + "')"));
        }

        public ActionResult ReviewRequestCompleted()
        {
           return View("ReviewRequestCompleted");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SetupPassword(SetupPasswordModel model, string PK)
        {
            if (ModelState.IsValid)
            {
                LoginModel lm = new LoginModel();
                lm.Password = model.Password;

                Guid RequestGUID = Portal.StringToGUID(PK);
                userRegistrationQueue Request = DbCMS.userRegistrationQueue.Find(RequestGUID);
                if (Request == null) { return View("RegistrationFailed"); }

                //Get Password Ready = Password + Salt(UserGUID) + Papper
                //model.Password = Convert.ToBase64String(HashingHelper.GetHashedPasswordBytes(model.Password, Portal.GUIDToString(Request.UserGUID)));

                model.Password = HashingHelper.EncryptPassword(model.Password, Portal.GUIDToString(Request.UserGUID));
                Request.AccountStatusID = AccountStatus.Step05_PendingAcceptTermOfUse;

                userAccounts UserAccount = Mapper.Map<userAccounts>(Request);
                userPersonalDetails UserPersonalDetails = Mapper.Map<userPersonalDetails>(Request);
                userPersonalDetailsLanguage UserPersonalDetailsLanguage = Mapper.Map<userPersonalDetailsLanguage>(Request);
                userServiceHistory UserServiceHistory = Mapper.Map<userServiceHistory>(Request);
                userPasswords Password = Mapper.Map<SetupPasswordModel, userPasswords>(model);
                userProfiles userProfiles = Mapper.Map<userProfiles>(Request);
                userProfiles.UserProfileGUID = Guid.NewGuid();
                Guid ServiceHistoryGUID = Guid.NewGuid();
                UserServiceHistory.ServiceHistoryGUID = ServiceHistoryGUID;
                userProfiles.ServiceHistoryGUID = ServiceHistoryGUID;
                Password.UserGUID = (Guid)Request.UserGUID;

                //Missing Fields
                UserPersonalDetailsLanguage.PersonalDetailsLanguageGUID = Guid.NewGuid();
                UserPersonalDetailsLanguage.LanguageID = LAN;
                //UserServiceHistory.ServiceHistoryGUID = Guid.NewGuid();

                DbCMS.userAccounts.Add(UserAccount); UserAccount.Active = true;
                DbCMS.userPasswords.Add(Password); Password.Active = true;

                DbCMS.userPersonalDetails.Add(UserPersonalDetails); UserPersonalDetails.Active = true;
                DbCMS.userPersonalDetailsLanguage.Add(UserPersonalDetailsLanguage); UserPersonalDetailsLanguage.Active = true;
                DbCMS.userServiceHistory.Add(UserServiceHistory); UserServiceHistory.Active = true;
                DbCMS.userProfiles.Add(userProfiles); userProfiles.Active = true;
                DbCMS.userRegistrationQueue.Remove(Request);//Clear the request from the Queue.
                DbCMS.SaveChanges();

                AccountController ac = new AccountController();

                lm.EmailAddress = UserServiceHistory.EmailAddress;
                return ac.Login(lm, "");
                return RedirectToAction("Index", "Account");
            }
            else
            {
                return SetupPassword();
            }
        }

        public ActionResult SetupPassword()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SetupOrganization(SetupOrganizationModel model, string PK)
        {
            if (ModelState.IsValid && ValidateJobInfo(model))
            {
                Guid RequestGUID = Portal.StringToGUID(PK);
                userRegistrationQueue QueuedRequest = DbCMS.userRegistrationQueue.Find(RequestGUID);
                QueuedRequest.SponsorConfirmed = false;
                Mapper.Map(model, QueuedRequest);
                QueuedRequest.AccountStatusID = AccountStatus.Step03_OperationAndOrganizationSubmitted;

                if (model.OrganizationGUID == UNHCR.GUID)
                {
                    QueuedRequest.AccountStatusID = AccountStatus.Step04_SponsorApproved;
                    DbCMS.SaveChanges();
                    return RedirectToAction("SetupPassword", new { PK = PK });
                }
                else
                {
                    DbCMS.SaveChanges();
                    new Email().SendAccessRequest(QueuedRequest.UserGUID, model.SponsorUserProfileGUID);
                    return RedirectToAction("RequestSentMessage");
                }
            }
            else
            {
                return SetupOrganization(PK);
            }
        }
        private bool ValidateJobInfo(SetupOrganizationModel model)
        {
            bool valid = true;
            if (model.FromDate >= model.ToDate)
            {
                ModelState.AddModelError("", "Job start must be less than job end date.");
                valid = false;
            }
            return valid;
        }
        public ActionResult SetupOrganization(string PK)
        {
            Guid RequestGUID = Portal.StringToGUID(PK);

            var userRegistrationQueue = DbCMS.userRegistrationQueue.Find(RequestGUID);
            if (userRegistrationQueue != null)
            {
                string EmailAddress = userRegistrationQueue.EmailAddress;

                ViewBag.IsUNHCR = false;

                if (EmailAddress.ToLower().Contains("@unhcr.org"))
                {
                    ViewBag.IsUNHCR = true;
                }
                return View("SetupOrganization");
            }
            else
            {
                return View("RemoveRequest");
            }

            
        }

        public ActionResult RemoveRequest(string PK)
        {
            Guid RequestID = Portal.StringToGUID(PK);
            var Request = DbCMS.userAccounts.SingleOrDefault(x => x.UserGUID == RequestID && x.AccountStatusID == AccountStatus.Step01_RegistrationRequestSubmitted); //-4 Means the request is still fresh!
            if (Request != null)
            {
                DbCMS.userAccounts.Remove(Request);
                DbCMS.SaveChanges();
                ViewBag.Message = resxPages.AccountRemoved;
            }
            else
            {
                ViewBag.Message = resxPages.RequestProcessed;
            }

            return View();
        }

        public ActionResult RegistrationFailed()
        {
            return View();
        }

        public ActionResult RequestSentMessage()
        {
            return View();
        }

        public ActionResult SponsorSentMessage(string PK)
        {
            Guid RequestGUID = Portal.StringToGUID(PK);
            userRegistrationQueue model = DbCMS.userRegistrationQueue.Find(RequestGUID);
            var Sponsor = (from a in DbCMS.userProfiles.Where(x => x.Active && x.UserProfileGUID == model.SponsorUserProfileGUID)
                           join b in DbCMS.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.userServiceHistory.UserGUID equals b.UserGUID
                           join c in DbCMS.codeJobTitlesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.JobTitleGUID equals c.JobTitleGUID
                           //join d in DbCMS.codeOrganizationsInstancesLanguages.Where(x=>x.Active && x.LanguageID == LAN) on a.userServiceHistory.OrganizationGUID equals d.us
                           select new SponsorMessageModel
                           {
                               SponsorFullName = b.FirstName + " " + b.Surname,
                               SponsorJobTitle = c.JobTitleDescription
                           }).FirstOrDefault();
            Sponsor.UserFullName = model.FirstName + " " + model.Surname;
            Sponsor.RequestID = PK;


            return View(Sponsor);
        }

        public ActionResult RegisterNew()
        {
            return null;
            return PartialView("~/Views/Registration/_RegisterNewUpdateModal.cshtml", new RegisterNewUpdateModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult RegisterNewCreate(RegisterNewUpdateModel model)
        {
            return null;
            bool userFound = (from a in DbCMS.userServiceHistory where a.EmailAddress == model.EmailAddress && a.Active select a).Count() > 0;
            if (userFound)
            {
                ModelState.AddModelError("", "User is already exists");
                return PartialView("~/Views/Registration/_RegisterNewUpdateModal.cshtml", model);
            }
            Guid NewUserGUID = Guid.NewGuid();
            userAccounts userAccounts = new userAccounts();
            userPersonalDetails userPersonalDetails = new userPersonalDetails();
            userPersonalDetailsLanguage userPersonalDetailsLanguage = new userPersonalDetailsLanguage();
            userServiceHistory userServiceHistory = new userServiceHistory();
            userProfiles userProfiles = new userProfiles();
            userPasswords userPasswords = new userPasswords();
            userContactDetails userContactDetails = new userContactDetails();

            userAccounts.UserGUID = NewUserGUID;
            userAccounts.IsFirstLogin = false;
            userAccounts.Active = true;
            userAccounts.RequestedOn = DateTime.Now;

            userAccounts.TimeZone = "Syria Standard Time";
            userAccounts.AccountStatusID = 10;
            userAccounts.SecurityQuestionGUID = Guid.Parse("EE813F52-8768-4E4A-8E07-42F43093859E"); //Who is your favorite artist?
            userAccounts.SecurityAnswer = "Automated security answer! Change it please";
            userAccounts.Active = true;
            userAccounts.AccountStatusID = 11;

            userPersonalDetails.UserGUID = NewUserGUID;
            userPersonalDetails.PreferedLanguageID = "EN";

            userPersonalDetailsLanguage.PersonalDetailsLanguageGUID = Guid.NewGuid();
            userPersonalDetailsLanguage.UserGUID = NewUserGUID;
            userPersonalDetailsLanguage.LanguageID = "EN";
            userPersonalDetailsLanguage.FirstName = model.FirstName;
            userPersonalDetailsLanguage.Surname = model.SurName;
            userPersonalDetailsLanguage.Active = true;

            Guid NewServiceHistoryGUiD = Guid.NewGuid();
            userServiceHistory.ServiceHistoryGUID = NewServiceHistoryGUiD;
            userServiceHistory.UserGUID = NewUserGUID;
            //Guid organizationGuid = (from a in DbCMS.codeOrganizationsInstances where a.OrganizationInstanceGUID == model.OrganizationInstanceGUID select a.OrganizationGUID).FirstOrDefault();
            userServiceHistory.OrganizationGUID = Guid.Parse("4F74C544-89A7-47EE-8A03-6BDF4ADA7269");  // Users with no organization
            userServiceHistory.EmailAddress = model.EmailAddress;
            userServiceHistory.Active = true;

            userProfiles.UserProfileGUID = Guid.NewGuid();
            userProfiles.ServiceHistoryGUID = NewServiceHistoryGUiD;
            //userProfiles.OrganizationInstanceGUID = model.OrganizationInstanceGUID;
            userProfiles.OrganizationInstanceGUID = Guid.Empty;
            //userProfiles.DutyStationGUID = model.DutyStationGUID;
            //userProfiles.DepartmentGUID = model.DepartmentGUID;
            //userProfiles.JobTitleGUID = model.JobTitleGUID;
            userProfiles.FromDate = new DateTime(1900, 1, 1);
            userProfiles.Active = true;

            userPasswords.PasswordGUID = Guid.NewGuid();
            userPasswords.UserGUID = NewUserGUID;
            userPasswords.ActivationDate = DateTime.Now;
            userPasswords.Active = true;
            userPasswords.Password = HashingHelper.EncryptPassword(model.Password, Portal.GUIDToString(NewUserGUID));

            userContactDetails.ContactDetailsGUID = Guid.NewGuid();
            userContactDetails.UserGUID = NewUserGUID;
            //userContactDetails.OfficeLandlineCountryCode = model.OfficeLandlineCountryCode;
            //userContactDetails.OfficeLandlineAreaCode = model.OfficeLandlineAreaCode;
            //userContactDetails.OfficeLandlineNumber = model.OfficeLandlineNumber;
            userContactDetails.Active = true;

            DbCMS.userAccounts.Add(userAccounts);
            DbCMS.userPersonalDetails.Add(userPersonalDetails);
            DbCMS.userPersonalDetailsLanguage.Add(userPersonalDetailsLanguage);
            DbCMS.userServiceHistory.Add(userServiceHistory);
            DbCMS.userProfiles.Add(userProfiles);
            DbCMS.userPasswords.Add(userPasswords);
            DbCMS.userContactDetails.Add(userContactDetails);

            try
            {
                DbCMS.SaveChanges();

                var User = (from a in DbCMS.userAccounts
                            join b in DbCMS.userPasswords on a.UserGUID equals b.UserGUID into LJP
                            from RJP in LJP.DefaultIfEmpty()
                            join c in DbCMS.userServiceHistory.Where(x => x.EmailAddress == model.EmailAddress) on a.UserGUID equals c.UserGUID
                            join d in DbCMS.userProfiles on c.ServiceHistoryGUID equals d.ServiceHistoryGUID
                            join e in DbCMS.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.UserGUID equals e.UserGUID into LJ1
                            from l1 in LJ1.DefaultIfEmpty()
                            orderby d.FromDate descending, RJP.ActivationDate descending //Very important to get the latest password.
                            select new
                            {
                                c.EmailAddress,
                                a.UserGUID,
                                d.UserProfileGUID,
                                d.OrganizationInstanceGUID,
                                RJP.Password,
                                a.AccountStatusID,
                                a.userServiceHistory.FirstOrDefault().ServiceHistoryGUID,
                                FullName = l1.FirstName + " " + l1.Surname
                            }).FirstOrDefault();

                if (User != null)
                {
                    string EncryptedPassword = User.Password; // This is the db encyrpted password
                    string EnteredPassword = HashingHelper.EncryptPassword(model.Password, Portal.GUIDToString(User.UserGUID));

                    System.Web.HttpContext.Current.Session[SessionKeys.UserGUID] = User.UserGUID;
                    System.Web.HttpContext.Current.Session[SessionKeys.OrganizationInstanceGUID] = User.OrganizationInstanceGUID;
                    System.Web.HttpContext.Current.Session[SessionKeys.FullName] = User.FullName;
                    System.Web.HttpContext.Current.Session[SessionKeys.UserProfileGUID] = User.UserProfileGUID;
                    System.Web.HttpContext.Current.Session[SessionKeys.ServiceHistoryGUID] = User.ServiceHistoryGUID;
                    System.Web.HttpContext.Current.Session["FirstLogin"] = "True";
                    System.Web.HttpContext.Current.Session[SessionKeys.EmailAddress] = User.EmailAddress;

                    string emailAddress = System.Web.HttpContext.Current.Session[SessionKeys.EmailAddress].ToString();
                    string fullname = (from a in DbCMS.userPersonalDetailsLanguage
                                       where a.UserGUID == User.UserGUID && a.LanguageID == "EN" && a.Active
                                       select a.FirstName + " " + a.Surname).FirstOrDefault();
                    new Email().SendGTPVerifyEmail(User.UserGUID.ToString(), fullname, emailAddress);

                    return Json(DbCMS.SingleCreateMessage("window.location.href = '/Registration/RegisterVerifyEmailNew'", true));

                }

                return Json(DbCMS.SingleUpdateMessage(null, null, null, null, "Record has been added, please click the link in your inbox to verify your email address"));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [Route("Registration/RegisterVerifyEmailNew")]
        public ActionResult RegisterVerifyEmailNew()
        {
            return View("~/Views/Registration/RegisterVerifyEmailNew.cshtml");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SendVerifyEmailNew()
        {
            string emailAddress = System.Web.HttpContext.Current.Session[SessionKeys.EmailAddress].ToString();
            string fullname = (from a in DbCMS.userPersonalDetailsLanguage
                               where a.UserGUID == UserGUID && a.LanguageID == "EN" && a.Active
                               select a.FirstName + " " + a.Surname).FirstOrDefault();
            new Email().SendGTPVerifyEmail(UserGUID.ToString(), fullname, emailAddress);
            return Json(DbCMS.SingleUpdateMessage(null, null, null, null, "Verification email has been sent"));
        }

        public ActionResult RegisterVerifyEmailNew(string pk)
        {
            Guid _userguid = Guid.Parse(pk);
            userAccounts userAccount = (from a in DbCMS.userAccounts where a.UserGUID == _userguid select a).FirstOrDefault();
            userAccount.AccountStatusID = 10;
            DbCMS.SaveChanges();
            return RedirectToAction("Index", "Account");
        }
    }
}