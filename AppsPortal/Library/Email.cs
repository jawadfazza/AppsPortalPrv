using AppsPortal.Data;
using AppsPortal.Extensions;
using RES_Repo.Globalization;
using AppsPortal.Models;
using AppsPortal.ViewModels;
using System;
using System.Linq;
using AMS_DAL.Model;
using REF_DAL.Model;
using System.Collections.Generic;

using userRegistrationQueue = AppsPortal.Models.userRegistrationQueue;
using System.Configuration;

namespace AppsPortal.Library
{

    public class Email
    {
        private CMSEntities DbCMS;
        private CMS CMS;
        private string LAN;
        public Email()
        {
            DbCMS = new CMSEntities();
            CMS = new CMS(DbCMS);
            LAN = Languages.CurrentLanguage().ToUpper();
        }

        public void Send(string recipients, string copy_recipients, string blind_copy_recipients, string subject, string body, string body_format, string importance, string file_attachments, string from_address = null)
        {

            DbCMS.SendEmail(recipients, copy_recipients, blind_copy_recipients, subject, body, body_format, importance, file_attachments, from_address);
        }
        //Use this function to send mail from any app
        public void Send(string recipients, string subject, string body, string from_address = null)
        {
            string copy_recipients = null;
            string blind_copy_recipients = null;
            string body_format = "HTML";
            string importance = "Normal";
            string file_attachments = null;
            string _body = "<div style='font-family:Arial;'>" + body.Replace("\r\n", "<br/>") + "</div>";
            if (LAN == "AR") { _body = "<p align='right'>" + _body + "</p>"; }
            DbCMS.SendEmail(recipients, copy_recipients, blind_copy_recipients, subject, _body, body_format, importance, file_attachments, from_address);
        }

        public void SendVerifyPrimaryEmail(userRegistrationQueue model)
        {

            string URL = AppSettingsKeys.Domain + "Registration/RegisterVerifyEmail/" + new Portal().GUIDToString(model.UserRegistrationQueueGUID);
            string Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.VerifyEmail + "</a>";
            string Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";

            string _message = resxEmails.PrimaryEmailVerification
                .Replace("$FullName", model.Surname)
                .Replace("$VerifyEmailAddressLink", Anchor)
                .Replace("$Link", Link);
            if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
            Send(model.EmailAddress, resxEmails.PrimaryEmailVerificationSubject, _message);
        }

        public void SendVerifyEmailForgetPassword(UserModel model)
        {

            string URL = AppSettingsKeys.Domain + "Account/SetupPassword/" + new Portal().GUIDToString(model.UserGUID);
            string Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ChangePassword + "</a>";
            string Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";

            string _message = resxEmails.ForgetPasswordEmailVerify
                .Replace("$FullName", model.Surname)
                .Replace("$EmailAddress", model.EmailAddress)
                .Replace("$VerifyEmailAddressLink", Anchor)
                .Replace("$Link", Link);
            if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
            Send(model.EmailAddress, resxEmails.ForgetPasswordEmailVerifySubject, _message);
        }

        public void SendSponsorConfirmedAccount(userRegistrationQueue model)
        {
            string URL = AppSettingsKeys.Domain + "Registration/SetupPassword/" + new Portal().GUIDToString(model.UserRegistrationQueueGUID);
            string Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.VerifyEmail + "</a>";
            string Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";

            string _message = resxEmails.SponsorConfirmedAccount
                .Replace("$FullName", model.Surname)
                .Replace("$SponsorName", CMS.GetUser(model.SponsorUserProfileGUID.Value).FullName)
                .Replace("$VerifyEmailAddressLink", Anchor)
                .Replace("$Link", Link);
            if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
            Send(model.EmailAddress, resxEmails.SponsorConfirmedAccountSubject, _message);
        }

        public void SendAccessRequest(Guid UserGUID, Guid SponsorGUID)
        {
            string URL = string.Empty;
            string Anchor = string.Empty;
            string Link = string.Empty;
            string Subject = string.Empty;
            string Message = string.Empty;


            var user = DbCMS.userRegistrationQueue
                .Where(u => u.UserGUID == UserGUID)
                .Select(u => new { u.FirstName, u.Surname, u.EmailAddress, u.OrganizationGUID }).FirstOrDefault();

            UserModel sponsor = CMS.GetUser(SponsorGUID);

            // Send to the User //////////////////////////////////////////////////////////////////////////////////////////////////////////
            URL = AppSettingsKeys.Domain + "Registration/SetupOrganization/" + new Portal().GUIDToString(UserGUID);
            Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ChangeSponsor + "</a>";
            Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
            Subject = resxEmails.AccessRequestToUserSubject;

            Message = resxEmails.AccessRequestToUser
                .Replace("$UserFullName", user.FirstName + " " + user.Surname)
                .Replace("$SponsorFullName ", sponsor.FullName)
                .Replace("$SponsorEmailAddress", sponsor.EmailAddress)
                .Replace("$SponsorJobTitle", sponsor.JobTitle)
                .Replace("$SponsorOrganizationFullName", sponsor.Organization)
                .Replace("$SponsorFirstName", sponsor.FirstName)
                .Replace("$UserFullOrganizationName", sponsor.Organization)
                .Replace("$ChangeSponsor", Anchor)
                .Replace("$Link", Link);
            if (LAN == "AR") { Message = "<p align='right'>" + Message + "</p>"; }
            Send(user.EmailAddress, Subject, Message);

            // Send to the Sponsor //////////////////////////////////////////////////////////////////////////////////////////////////////////
            URL = AppSettingsKeys.Domain + "Registration/ReviewRequest/" + new Portal().GUIDToString(UserGUID);
            Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ReviewRequest + "</a>";
            Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
            Subject = resxEmails.AccessRequestToSponsorSubject;
            string Org = DbCMS.codeOrganizationsLanguages.Where(o => o.OrganizationGUID == user.OrganizationGUID && o.LanguageID == LAN).FirstOrDefault().OrganizationDescription;

            Message = resxEmails.AccessRequestToSponsor
                .Replace("$SponsorFullName", sponsor.FullName)
                .Replace("$UserFullName", user.FirstName + " " + user.Surname)
                .Replace("$UserFullOrganizationName", Org)
                .Replace("$ReviewRequest", Anchor)
                .Replace("$Link", Link);
            if (LAN == "AR") { Message = "<p align='right'>" + Message + "</p>"; }
            Send(sponsor.EmailAddress, Subject, Message);

        }

        internal void SendChangeSponser(userRegistrationQueue user)
        {
            string URL = string.Empty;
            string Anchor = string.Empty;
            string Link = string.Empty;
            string Subject = string.Empty;
            string Message = string.Empty;


            UserModel sponsor = CMS.GetUser(user.SponsorUserProfileGUID.Value);
            // Send to the User //////////////////////////////////////////////////////////////////////////////////////////////////////////
            URL = AppSettingsKeys.Domain + "Registration/SetupOrganization/" + new Portal().GUIDToString(user.UserRegistrationQueueGUID);
            Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ChangeSponsor + "</a>";
            Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
            Subject = resxEmails.AccessRequestToSponsorFailedSubject;

            Message = resxEmails.AccessRequestToSponsorFailed
                .Replace("$UserFullName", user.FirstName + " " + user.Surname)
                .Replace("$ReviewRequest", Anchor)
                .Replace("$SponsorFullName ", sponsor.FullName)
                .Replace("$Link", Link);
            if (LAN == "AR") { Message = "<p align='right'>" + Message + "</p>"; }
            Send(user.EmailAddress, Subject, Message);
        }

        internal dataAppointment AppointmentNotify(dataAppointment AppLocal, string FileNumber, List<Guid> ReferralSteps)
        {
            using (var DbREF = new REFEntities())
            {
                bool SendMessage = false;
                foreach (var ReferralStepGUID in ReferralSteps)
                {
                    if (AppLocal.AppointmentWithUserGUID != null)
                    {
                        Guid UserGUID = AppLocal.AppointmentWithUserGUID.Value;
                        var ReferralStep = DbREF.configReferralStep.Where(x => x.ReferralStepGUID == ReferralStepGUID).FirstOrDefault();
                        string NotificationResxKey = string.Empty;
                        string URL = string.Empty;
                        string UserFullName = CMS.GetFullName(UserGUID, LAN);
                        string Link = string.Empty;
                        string Subject = string.Empty;
                        string Message = string.Empty;
                        
                        if (ReferralStepConstants.AppointmentAssigned == ReferralStepGUID)
                        {
                            using (var DbAMS = new AMSEntities())
                            {
                                var AppDb = DbAMS.dataAppointment.Where(x => x.AppointmentGUID == AppLocal.AppointmentGUID).FirstOrDefault();
                                if (AppDb != null)
                                {
                                    if (AppLocal.AppointmentWithUserGUID != AppDb.AppointmentWithUserGUID)
                                    {
                                        SendMessage = true;
                                        AppLocal.StatusGUID = ReferralStatusConstants.Pending;
                                    }
                                }
                                else
                                {
                                    if (AppLocal.AppointmentWithUserGUID != Guid.Empty)
                                    {
                                        SendMessage = true;
                                        AppLocal.StatusGUID = ReferralStatusConstants.Pending;
                                    } 
                                }
                            }
                        }
                        if (ReferralStepConstants.AppointmentArrivedCanceled == ReferralStepGUID)
                        {
                            if (AppLocal.Arrived)
                            {
                                SendMessage = true;
                                AppLocal.StatusGUID = ReferralStatusConstants.InProgress;
                            }
                            if (AppLocal.Cancelled)
                            {
                                SendMessage = true;
                                AppLocal.StatusGUID = ReferralStatusConstants.Closed;
                            }
                        }
                        if (ReferralStepConstants.AppointmentCompleted == ReferralStepGUID && AppLocal.Arrived)
                        {
                            if (AppLocal.StatusGUID == ReferralStatusConstants.Complete)
                            {
                                SendMessage = true;
                            }
                        }
                        if (ReferralStepConstants.AppointmentRescheduled == ReferralStepGUID )
                        {
                            SendMessage = true;
                            AppLocal.StatusGUID = ReferralStatusConstants.Pending;
                        }
                        var Notifiaction = ReferralStep.configReferralNotification.Where(x => x.ReferralStatusGUID == AppLocal.StatusGUID).FirstOrDefault();
                        if (Notifiaction != null) {
                            URL = AppSettingsKeys.Domain + Notifiaction.PageURL + "/" + AppLocal.CaseGUID;
                            NotificationResxKey = Notifiaction.NotificationResxKey;
                            Link = "<a href='" + URL + "' target='_blank'>Here</a>";
                            resxEmails emails = new resxEmails();
                            Subject = emails.GetType().GetProperty(NotificationResxKey + "Subject").GetValue(emails, null).ToString()
                                .Replace("$FileNumber", FileNumber);
                            Message = emails.GetType().GetProperty(NotificationResxKey + "Body").GetValue(emails, null).ToString()
                                .Replace("$UserFullName", UserFullName)
                                .Replace("$FileNumber", FileNumber)
                                .Replace("$URL", URL)
                                .Replace("$Link", Link);
                            if (LAN == "AR") { Message = "<p align='right'>" + Message + "</p>"; }
                            // Send to the User //////////////////////////////////////////////////////////////////////////////////////////////////////////
                            if (SendMessage) { Send(CMS.GetCurrentUserEmail(UserGUID), Subject, Message); }
                        }
                    }
                }
            }
            return AppLocal;
        }

        public void SendUploadedPPAFilesEmail(string RecipientEmail,string CCList, string RecipientFullName, string Deadline,string PPACreator, string subject,Guid PPAGUID)
        {
            string URL = "http://10.240.224.199/PPA/FilesProcess/Update/" + PPAGUID.ToString();
            string Anchor = "<a href='" + URL + "' target='_blank'>PPA Link</a>";
            string Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
            string body_format = "HTML";
            string importance = "Normal";

            string Message = resxEmails.PPAUploadEmail
                .Replace("$RecipientFullName", RecipientFullName)
                .Replace("$PPADeadline", Deadline)
                .Replace("$PPACreator", PPACreator)
                .Replace("$PPALink", Anchor)
                .Replace("$Link", Link);

            string _body = "<div style='font-family:Arial;'>" + Message.Replace("\r\n", "<br/>") + "</div>";
            Send(RecipientEmail, CCList, null, subject, _body, body_format, importance, null);

        }

        public void SendHelpDeskNotify(string RecipientEmail, string RecipientName, string CCList, string Type, string Action, string subject, Guid PK, int RequestNumber)
        {

            string domain = "https://prv.unhcrsyria.org/";
            string URL = domain + "/SRS/" + Action + "/Update/" + PK.ToString();
            string Anchor = "<a href='" + URL + "' target='_blank'>Request Link</a>";
            string Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
            string body_format = "HTML";
            string importance = "Normal";

            string Message = resxEmails.HelpDeskNotifyEmail
                .Replace("$Requestor", RecipientName)
                .Replace("$RequestNumber", RequestNumber.ToString())
                .Replace("$ServiceRequestType", Type)
                .Replace("$HelpDeskLink", Anchor)
                .Replace("$Link", Link);

            string _body = "<div style='font-family:Arial;'>" + Message.Replace("\r\n", "<br/>") + "</div>";
            Send(RecipientEmail, CCList, null, subject, _body, body_format, importance, null);
        }

        public void SendHelpDeskRequest(string RecipientEmail, string CCList, string Type, string Action, string subject, Guid PK)
        {

            string domain = "https://prv.unhcrsyria.org/";
            string URL = domain + "/SRS/" + Action + "/Update/" + PK.ToString();
            string Anchor = "<a href='" + URL + "' target='_blank'>Request Link</a>";
            string Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
            string body_format = "HTML";
            string importance = "Normal";

            string Message = resxEmails.HelpDeskEmail
                .Replace("$ServiceRequestType", Type)
                .Replace("$HelpDeskLink", Anchor)
                .Replace("$Link", Link);

            string _body = "<div style='font-family:Arial;'>" + Message.Replace("\r\n", "<br/>") + "</div>";
            Send(RecipientEmail, CCList, null, subject, _body, body_format, importance, null);
        }

        public void SendHelpDeskAssignRequest(string RecipientEmail, string RecipientName, string CCList, string Type, string Action, string subject, Guid PK)
        {
            string domain = "https://prv.unhcrsyria.org/";
            string URL = domain +"/SRS/" + Action + "/Update/" + PK.ToString();
            string Anchor = "<a href='" + URL + "' target='_blank'>Request Link</a>";
            string Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
            string body_format = "HTML";
            string importance = "Normal";

            string Message = resxEmails.HelpDeskAssignEmail
                .Replace("$AssignedTo", RecipientName)
                .Replace("$ServiceRequestType", Type)
                .Replace("$HelpDeskLink", Anchor)
                .Replace("$Link", Link);

            string _body = "<div style='font-family:Arial;'>" + Message.Replace("\r\n", "<br/>") + "</div>";
            Send(RecipientEmail, CCList, null, subject, _body, body_format, importance, null);
        }

       

        #region SHM
        public void SendShuttleRequestUserReceiveNotificationStatusPinding(Guid UserGUID)
        {
            string Subject = string.Empty;
            string Message = string.Empty;

            // Send to the User //////////////////////////////////////////////////////////////////////////////////////////////////////////
            Subject = resxEmails.ShuttleRequestUserReceiveNotificationStatusPindingSubject;

            Message = resxEmails.ShuttleRequestUserReceiveNotificationStatusPindingBody
                .Replace("$FullName", CMS.GetFullName(UserGUID, LAN));
            if (LAN == "AR") { Message = "<p align='right'>" + Message + "</p>"; }
            Send(CMS.GetCurrentUserEmail(UserGUID), Subject, Message, "Shuttle Fleet Management<dldamascus-cotranspor@unhcr.org>");
        }

        public void SendShuttleRequestFocallPointReceiveNotificationStatusPinding(Guid SuttleRequestGUID, Guid UserProfileGUID)
        {

            string URL = AppSettingsKeys.Domain + "/SHM/ShuttleRequests/Update/" + SuttleRequestGUID;
            string Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.View + "</a>";
            string Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
            string Subject = resxEmails.ShuttleRequestUserReceiveNotificationStatusPindingSubject;

            var userProfile = DbCMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID).FirstOrDefault();
            var FoaclPointStaffs = DbCMS.configFocalPointStaff.Where(x => x.configFocalPoint.ApplicationGUID == Apps.SHM
                                         && x.configFocalPoint.DutyStationGUID == userProfile.DutyStationGUID && x.Active && x.configFocalPoint.Active).ToList();
            foreach (var fp in FoaclPointStaffs)
            {
                UserModel FoaclPointStaff = (from a in DbCMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active)
                                             join d in DbCMS.userServiceHistory.Where(x => x.Active) on a.UserGUID equals d.UserGUID
                                             where a.UserGUID == fp.UserGUID
                                             select new UserModel
                                             {
                                                 EmailAddress = d.EmailAddress,
                                                 FirstName = a.FirstName,
                                                 Surname = a.Surname
                                             }
                                  ).FirstOrDefault();
                string _message = resxEmails.ShuttleRequestUserReceiveNotificationStatusCompletedBody
                    .Replace("$FullName", FoaclPointStaff.Surname)
                    .Replace("$ShuttleRequestLink ", Anchor)
                    .Replace("$Link", Link);
                if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                Send(FoaclPointStaff.EmailAddress, Subject, _message, "Shuttle Fleet Management<dldamascus-cotranspor@unhcr.org>");
            }
        }
        public void SendShuttleCompleted(string RecipientEmail, string copy_recipients, string blind_copy_recipients, string table, DateTime shuttleDate)
        {
            string Subject = string.Empty;
            string Message = string.Empty;
            string body_format = "HTML";
            string importance = "Normal";

            // Send to the User
            Subject = resxEmails.ShuttleRequestCompletedSubject + " " + shuttleDate.ToString("dd-MMM-yyyy");
            Message = table;

            if (LAN == "AR") { Message = "<p align='right'>" + Message + "</p>"; }
            //var FoaclPointStaffs =(from a in DbCMS.configFocalPointStaff.Where(x => x.configFocalPoint.ApplicationGUID == Apps.SHM
            //                 && x.configFocalPoint.DutyStationGUID == userProfile.DutyStationGUID && x.Active && x.configFocalPoint.Active).ToList();

            Send(RecipientEmail, copy_recipients, blind_copy_recipients, Subject, Message, body_format, importance, "", "Shuttle Fleet Management<dldamascus-cotranspor@unhcr.org>");
        }


        public void ShuttleNotifyStaffRequest(string RecipientEmail, string copy_recipients, string table)
        {
            string Subject = string.Empty;
            string Message = string.Empty;
            string body_format = "HTML";
            string importance = "Normal";

            // Send to the User
            Subject = "Complete Shuttle Request Form  (Return back From Travel) Section";
            Message = table;

            if (LAN == "AR") { Message = "<p align='right'>" + Message + "</p>"; }
            Send(RecipientEmail, copy_recipients, "", Subject, Message, body_format, importance, "", "Shuttle Fleet Management<dldamascus-cotranspor@unhcr.org>");
        }
        #endregion

        public void SendHelpDeskResolution(string RecipientEmail, string RecipientName, string CCList, string RequestNumber, string Action, string Subject, Guid PK)
        {
            string domain = "https://prv.unhcrsyria.org/";
            string URL = domain + "/SRS/" + Action + "/Update/" + PK.ToString();
            string Anchor = "<a href='" + URL + "' target='_blank'>Request Link</a>";
            string Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
            string body_format = "HTML";
            string importance = "Normal";

            string Message = resxEmails.HelpDeskResolutionEmail
                .Replace("$Requestor", RecipientName)
                .Replace("$RequestNumber", RequestNumber)
                .Replace("$HelpDeskLink", Anchor)
                .Replace("$Link", Link);

            string _body = "<div style='font-family:Arial;'>" + Message.Replace("\r\n", "<br/>") + "</div>";
            Send(RecipientEmail, CCList, null, Subject, _body, body_format, importance, null);


        }

        #region TTT
        public void SendCreateTenderConfirmation(string RecipientEmail, string CCList, string TenderReference,string CreatedOn,string CreatedBy, string Action, string Subject, Guid PK)
        {
            string domain = ConfigurationManager.AppSettings["Domain"];
            string URL = domain + "/TTT/" + Action + "/Update/" + PK.ToString();
            string Anchor = "<a href='" + URL + "' target='_blank'>Here</a>";
            string Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
            string body_format = "HTML";
            string importance = "Normal";

            string Message = resxEmails.TendreCreateConfirmationEmail
               .Replace("$TenderReference", TenderReference)
               .Replace("$CreatedOn", CreatedOn)
               .Replace("$CreatedBy ", CreatedBy)
               .Replace("$TenderLink", Anchor)
               .Replace("$Link", Link);

            string _body = "<div style='font-family:Arial;'>" + Message.Replace("\r\n", "<br/>") + "</div>";
            Send(RecipientEmail, CCList, null, Subject, _body, body_format, importance, null, "Tender Tracking Tool");
        }

        public void SendCloseTenderConfirmation(string RecipientEmail, string CCList, string TenderReference, string UpdatedOn, string UpdatedBy, string Action, string Subject, Guid PK)
        {
            string domain = ConfigurationManager.AppSettings["Domain"];
            string URL = domain + "/TTT/" + Action + "/Update/" + PK.ToString();
            string Anchor = "<a href='" + URL + "' target='_blank'>Here</a>";
            string Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
            string body_format = "HTML";
            string importance = "Normal";

            string Message = resxEmails.TenderClosingConfirmationEmail
               .Replace("$TenderReference", TenderReference)
               .Replace("$UpdatedOn", UpdatedOn)
               .Replace("$UpdatedBy ", UpdatedBy)
               .Replace("$TenderLink", Anchor)
               .Replace("$Link", Link);

            string _body = "<div style='font-family:Arial;'>" + Message.Replace("\r\n", "<br/>") + "</div>";
            Send(RecipientEmail, CCList, null, Subject, _body, body_format, importance, null, "Tender Tracking Tool");
        }
        #endregion

        #region TBS
        public void SendBillReadyEmail(string RecipientEmail, string RecipientName, string CCList, string Subject, string BillMonthYear, string Deadline, Guid PK)
        {
            string URL = AppSettingsKeys.Domain + "/TBS/UserBills/Update/" + PK;
            string Anchor = "<a href='" + URL + "' target='_blank'>Here</a>";
            string Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
            string UserGuidURL = "https://unhcr365.sharepoint.com/teams/mena-syr-ICTUnit/Shared%20Documents/ICT%20Unit/Documentation/Guides%20User/Telephone%20Bills_V2_Usermanual.pdf";
            string UserGuidAnchor = "<a href='" + UserGuidURL + "' target='_blank'>Link</a>";
            string body_format = "HTML";
            string importance = "Normal";

            string Message = resxEmails.TBSBillReadyEmail
                .Replace("$FullName", RecipientName)
                .Replace("$BillMonthYear", BillMonthYear)
                .Replace("$Deadline", Deadline)
                .Replace("$IdentifyBillLinkIn", Anchor)
                .Replace("$UserGuidLink", UserGuidAnchor);

            string _body = "<div style='font-family:Arial;'>" + Message.Replace("\r\n", "<br/>") + "</div>";
            Send(RecipientEmail, CCList, null, Subject, _body, body_format, importance, null, "Telephony Billing System");


        }
        #endregion

        #region COV
        public void SendCovFeedbackEmail(string RecipientEmail, string CCList, string Subject, Guid PK)
        {
            string URL = AppSettingsKeys.Domain + "/COV/CovidUNHCRResponse/Update/" + PK;
            string Anchor = "<a href='" + URL + "' target='_blank'>Here</a>";
            string Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
            string body_format = "HTML";
            string importance = "Normal";

            string Message = resxEmails.COVFeedbackEmail
                .Replace("$RecordLink", Anchor);

            string _body = "<div style='font-family:Arial;'>" + Message.Replace("\r\n", "<br/>") + "</div>";
            Send(RecipientEmail, CCList, null, Subject, _body, body_format, importance, null, "COVID-19 UNHCR Response Strategy Online Tool");
        }
        #endregion

     
        #region GTP
        public void SendGTPConfirmReceivingApplicationEmail(string RecipientEmail, string CCList, string FullName, string Subject)
        {
            string body_format = "HTML";
            string importance = "Normal";

            string Message = resxEmails.GTPConfirmReceivingApplication.Replace("$FullName", FullName);

            string _body = "<div style='font-family:Arial;'>" + Message.Replace("\r\n", "<br/>") + "</div>";

            Send(RecipientEmail, CCList, null, Subject, _body, body_format, importance, null, "Group 2 Applications Online Portal");
        }

        public void SendGTPEmailForSuccessfulApplicantsEmail(string RecipientEmail, string CCList, string FullName, string ApplicantEmailAddress, string ExpiryDate, string Subject)
        {
            string body_format = "HTML";
            string importance = "Normal";

            string Message = resxEmails.GTPEmailForSuccessfulApplicants
                .Replace("$FullName", FullName)
                .Replace("$ExpiryDate", ExpiryDate)
               .Replace("$ApplicantEmailAddress", ApplicantEmailAddress);
            string _body = "<div style='font-family:Arial;'>" + Message.Replace("\r\n", "<br/>") + "</div>";

            Send(RecipientEmail, CCList, null, Subject, _body, body_format, importance, null, "Group 2 Applications Online Portal");
        }

        public void SendGTPEmailForSuccessfulApplicantsEmail(string RecipientEmail, string CCList, string FullName, string ApplicantEmailAddress, string ExpiryDate, string emailBody, string Subject)
        {
            string body_format = "HTML";
            string importance = "Normal";

            string Message = emailBody
                .Replace("$FullName", FullName)
                .Replace("$ExpiryDate", ExpiryDate)
               .Replace("$ApplicantEmailAddress", ApplicantEmailAddress);
            string _body = "<div style='font-family:Arial;'>" + Message.Replace("\r\n", "<br/>") + "</div>";

            Send(RecipientEmail, CCList, null, Subject, _body, body_format, importance, null, "Group 2 Applications Online Portal");
        }

        public void SendGTPEmailForUnsuccessfulApplicantsEmail(string RecipientEmail, string CCList, string FullName, string Subject)
        {
            string body_format = "HTML";
            string importance = "Normal";

            string Message = resxEmails.GTPEmailForUnsuccessfulApplicants.Replace("$FullName", FullName);

            string _body = "<div style='font-family:Arial;'>" + Message.Replace("\r\n", "<br/>") + "</div>";

            Send(RecipientEmail, CCList, null, Subject, _body, body_format, importance, null, "Group 2 Applications Online Portal");
        }

        public void SendGTPVerifyEmail(string userguid, string fullname, string emailAddress)
        {
            string body_format = "HTML";
            string importance = "Normal";
            string URL = AppSettingsKeys.Domain + "/Registration/RegisterVerifyEmailNew/" + userguid;
            string Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.VerifyEmail + "</a>";
            string Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";

            string _message = resxEmails.PrimaryEmailVerification
                .Replace("$FullName", fullname)
                .Replace("$VerifyEmailAddressLink", Anchor)
                .Replace("$Link", Link);

            string _body = "<div style='font-family:Arial;'>" + _message.Replace("\r\n", "<br/>") + "</div>";

            Send(emailAddress, null, null, "UNHCR Portal Email Verification", _body, body_format, importance, null, "Group 2 Applications Online Portal"); ; ;
            Send(emailAddress, resxEmails.PrimaryEmailVerificationSubject, _message);
        }

        public void SendGTPNewApplicationHRNotification(string RecipientEmail, string CCList, string ApplicantName, string Subject)
        {
            string body_format = "HTML";
            string importance = "Normal";

            string Message = resxEmails.GTPNewApplicationHRNotification.Replace("$FullName", ApplicantName);

            string _body = "<div style='font-family:Arial;'>" + Message.Replace("\r\n", "<br/>") + "</div>";

            Send(RecipientEmail, CCList, null, Subject, _body, body_format, importance, null, "Group 2 Applications Online Portal");
        }

        public void SendGTPUpdateApplicationHRNotification(string RecipientEmail, string CCList, string ApplicantName, string Subject)
        {
            string body_format = "HTML";
            string importance = "Normal";

            string Message = resxEmails.GTPUpdateApplicationHRNotification.Replace("$FullName", ApplicantName);

            string _body = "<div style='font-family:Arial;'>" + Message.Replace("\r\n", "<br/>") + "</div>";

            Send(RecipientEmail, CCList, null, Subject, _body, body_format, importance, null, "Group 2 Applications Online Portal");
        }
        #endregion

        #region ORG
        public void SendApplicationPermissionRequest(AppInfoViewModel model)
        {
            string subject = "Access request for " + model.StaffName;

            string body_format = "HTML";
            string importance = "Normal";
            string URLFullAccess = AppSettingsKeys.Domain + "/Profile/GrantUserFullAccessToApplication/?PK=" + new Portal().GUIDToString(model.StaffApplicationAccessRequestGUID);
            string FullAccessAnchor = "<a href='" + URLFullAccess + "' target='_blank'>Click here to approve full access</a>";
            string URLreadAccess = AppSettingsKeys.Domain + "/Profile/GrantUserReadOnlyAccessToApplication/?PK=" + new Portal().GUIDToString(model.StaffApplicationAccessRequestGUID);
            string FullreadAnchor = "<a href='" + URLreadAccess + "' target='_blank'>Click here to approve read only access</a>";

            string URLrejectccess = AppSettingsKeys.Domain + "/Profile/RejectUserAccessToApplication/?PK=" + new Portal().GUIDToString(model.StaffApplicationAccessRequestGUID);
            string rejectAccessAnchor = "<a href='" + URLrejectccess + "' target='_blank'>Click here to reject the request</a>";
            string Message = resxEmails.StaffRequestPermissionMail
                .Replace("$AppOwner", model.ApplicationOwner)
                .Replace("$StaffName", model.StaffName)
                .Replace("$fullAccess", FullAccessAnchor)
                .Replace("$readAccess", FullreadAnchor)
                .Replace("$rejectAccess", rejectAccessAnchor)
                .Replace("$app", model.ApplicationName);
            if (LAN == "AR") { Message = "<p align='right'>" + Message + "</p>"; }
            string _body = "<div style='font-family:Arial;'>" + Message.Replace("\r\n", "<br/>") + "</div>";
            Send(model.AppOwnerEmailAddress, model.StaffEmailAddress, null, subject, _body, body_format, importance, null);

        }

        public void SendApproveAccessNotificationToStaff(AppInfoViewModel model)
        {
            string subject = "Access request for " + model.StaffName;

            string body_format = "HTML";
            string importance = "Normal";

            string Message = resxEmails.ConfirmAccessToApplication
                .Replace("$AppOwner", model.AppOwnerName)
                .Replace("$StaffName", model.StaffName)
                .Replace("$accesstype", model.AccessType)
                .Replace("$app", model.ApplicationName)
                //.Replace("$fullAccess", FullAccessAnchor)
                //.Replace("$readAccess", FullreadAnchor)
                //.Replace("$rejectAccess", rejectAccessAnchor)
                ;
            if (LAN == "AR") { Message = "<p align='right'>" + Message + "</p>"; }
            string _body = "<div style='font-family:Arial;'>" + Message.Replace("\r\n", "<br/>") + "</div>";
            string copyEmails = string.Join(" ;", model.StaffEmailAddress);
            string copyAllEmails = string.Join(";",model.StaffEmailAddress, model.AppOwnerEmailAddress, "isac@unhcr.org");

            string copyapps = string.Join(" ;", "Shaban@unhcr.org", "KARKOUSH@unhcr.org", "ALFAZZAA@unhcr.org", "maksoud@unhcr.org");


            Send(copyapps, copyAllEmails, null, subject, _body, body_format, importance, null);

        }

       
        public void SendRejectionAccessNotificationToStaff(AppInfoViewModel model)
        {
            string subject = "Access request for " + model.StaffName;

            string body_format = "HTML";
            string importance = "Normal";

            string Message = resxEmails.RejectAccessToApplication
                .Replace("$AppOwner", model.AppOwnerName)
                .Replace("$StaffName", model.StaffName)
                //.Replace("$fullAccess", FullAccessAnchor)
                //.Replace("$readAccess", FullreadAnchor)
                //.Replace("$rejectAccess", rejectAccessAnchor)
                .Replace("$app", model.ApplicationName);
            if (LAN == "AR") { Message = "<p align='right'>" + Message + "</p>"; }
            string _body = "<div style='font-family:Arial;'>" + Message.Replace("\r\n", "<br/>") + "</div>";
            Send(model.StaffEmailAddress, model.AppOwnerEmailAddress, null, subject, _body, body_format, importance, null);

        }
        #endregion

        #region PCA
        public void SendpartnersCapacityAssessment(string RecipientEmail, string copy_recipients, string blind_copy_recipients, string subject, string message)
        {
            string Subject = string.Empty;
            string Message = string.Empty;
            string body_format = "HTML";
            string importance = "Normal";

            // Send to the User
            Subject = subject;
            Message = message;

            if (LAN == "AR") { Message = "<p align='right'>" + Message + "</p>"; }

            Send(RecipientEmail, copy_recipients, blind_copy_recipients, Subject, Message, body_format, importance, "", "Partners Capacity Assessment");
        }
        #endregion

        #region PMD
        public void SendPMDRejectEmail(string RecipientEmail, string RecipientName, string ActionType, string Reason, string CCList, string Subject, Guid PK)
        {

            string URL = AppSettingsKeys.Domain + "/PMD/PartnerMonitoringDatabase/Update/" + PK;
            string Anchor = "<a href='" + URL + "' target='_blank'>Here</a>";
            string Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
            string body_format = "HTML";
            string importance = "Normal";

            string Message = resxEmails.SendPMDRejectEmail
               .Replace("$FullName", RecipientName)
                .Replace("$ActionType", ActionType)
                .Replace("$Reason", Reason)
                .Replace("$RecordLink", Anchor);


            string _body = "<div style='font-family:Arial;'>" + Message.Replace("\r\n", "<br/>") + "</div>";
            Send(RecipientEmail, CCList, null, Subject, _body, body_format, importance, null, "Project Reporting and Monitoring Database");



        }
        #endregion

        #region AHD
        public void InformSupervisor(Guid UserGUID, Guid ReportToGUID)
        {
            string Subject = string.Empty;
            string Message = string.Empty;

            var staffInfo = DbCMS.userPersonalDetailsLanguage.Where(x => x.UserGUID == UserGUID && x.Active && x.LanguageID == LAN).FirstOrDefault();
            string StaffName = staffInfo.FirstName + " " + staffInfo.Surname;
            // Send to the User //////////////////////////////////////////////////////////////////////////////////////////////////////////
            Subject = resxEmails.InformSupervisorSubject;

            Message = resxEmails.InformSupervisorMessage
                .Replace("$FullName", CMS.GetFullName(ReportToGUID, LAN))
                .Replace("$UserGUID", "para=" + UserGUID.ToString())
                .Replace("$StaffName", StaffName)
                .Replace("$Host", "prv.unhcrsyria.org");
            if (LAN == "AR") { Message = "<p align='right'>" + Message + "</p>"; }

            string body_format = "HTML";
            string importance = "Normal";
            string _body = "<div style='font-family:Arial;'>" + Message.Replace("\r\n", "<br/>") + "</div>";
            Send(CMS.GetCurrentUserEmail(ReportToGUID), CMS.GetCurrentUserEmail(UserGUID), "", Subject, _body, body_format, importance, "", "Staff Absence Management<HR-UNOPS@unhcr.org>");
        }

        public void SupervisorConfirmation(Guid UserGUID, Guid ReportToGUID, string AbsenceType)
        {
            string Subject = string.Empty;
            string Message = string.Empty;

            Subject = resxEmails.SupervisorConfirmingSubject;

            Message = resxEmails.SupervisorConfirmingMessage
                .Replace("$FullName", CMS.GetFullName(UserGUID, LAN))
                .Replace("$StaffNam", CMS.GetFullName(ReportToGUID, LAN))
                .Replace("$AbsenceType", AbsenceType)
                .Replace("$Host", "prv.unhcrsyria.org");
            if (LAN == "AR") { Message = "<p align='right'>" + Message + "</p>"; }

            string body_format = "HTML";
            string importance = "Normal";
            string _body = "<div style='font-family:Arial;'>" + Message.Replace("\r\n", "<br/>") + "</div>";
            Send(CMS.GetCurrentUserEmail(UserGUID), CMS.GetCurrentUserEmail(ReportToGUID), "", Subject, _body, body_format, importance, "", "Staff Absence Management<HR-UNOPS@unhcr.org>");
        }
        public void SupervisorCancelAbsence(Guid confirmedUserGUID, Guid directSupervisor)
        {
            string Subject = string.Empty;
            string Message = string.Empty;

            Subject = resxEmails.SupervisorConfirmingSubject;

            Message = "Dear " + CMS.GetFullName(confirmedUserGUID, LAN);
            Message += "<br/><br/>kindly check with your direct supervisor to clarify your absence Leave,";
            Message += "<br/><br/>Best Regards,";
            Message += "<br/>UNHCR Syria";
            if (LAN == "AR") { Message = "<p align='right'>" + Message + "</p>"; }

            string body_format = "HTML";
            string importance = "Normal";
            string _body = "<div style='font-family:Arial;'>" + Message.Replace("\r\n", "<br/>") + "</div>";
            Send(CMS.GetCurrentUserEmail(confirmedUserGUID), CMS.GetCurrentUserEmail(directSupervisor), "", Subject, _body, body_format, importance, "", "Staff Absence Management<HR-UNOPS@unhcr.org>");
        }

        #endregion

        #region OSA
        public void InformSupervisorAttendances(Guid UserGUID, Guid ReportToGUID,DateTime date)
        {
            string Subject = string.Empty;
            string Message = string.Empty;

            // Send to the User //////////////////////////////////////////////////////////////////////////////////////////////////////////
            Subject = resxEmails.InformStaffAttendanceSubject;

            Message = resxEmails.InformStaffAttendanceMessage
                .Replace("$ReportTo", CMS.GetFullName(ReportToGUID, LAN))
                .Replace("$Para", "Date=" + date.Year+"-"+date.Month+"&Guid="+ UserGUID)
                .Replace("$StaffName", CMS.GetFullName(UserGUID, LAN))
                .Replace("$Date", date.Year + "-" + date.AddMonths(1).ToString("MMMM")+"-04")
                .Replace("$Host", "Localhost");
            if (LAN == "AR") { Message = "<p align='right'>" + Message + "</p>"; }

            string body_format = "HTML";
            string importance = "Normal";
            string _body = "<div style='font-family:Arial;'>" + Message.Replace("\r\n", "<br/>") + "</div>";
            Send(/*CMS.GetCurrentUserEmail(ReportToGUID)*/"Alfazzaa@unhcr.org", "", "", Subject, _body, body_format, importance, "", "Staff Attendance Management<HR-UNOPS@unhcr.org>");
            Send(CMS.GetCurrentUserEmail(UserGUID), CMS.GetCurrentUserEmail(ReportToGUID), "", Subject, "Your direct Supervisor was notified in a separate email, You will be informed once attendance is Confirmed.<br/><br/>Best Regards,", body_format, importance, "", "Staff Attendance Management<HR-UNOPS@unhcr.org>");
        }
       

        public void SupervisorAttendancesConfirmation(Guid UserGUID, Guid ReportToGUID)
        {
            string Subject = string.Empty;
            string Message = string.Empty;

            //var staffInfo = DbCMS.userPersonalDetailsLanguage.Where(x => x.UserGUID == UserGUID && x.Active && x.LanguageID == LAN).FirstOrDefault();
            //string StaffName = staffInfo.FirstName + " " + staffInfo.Surname;
            // Send to the User //////////////////////////////////////////////////////////////////////////////////////////////////////////
            Subject = resxEmails.SupervisorConfirmingAttendanceSubject;

            Message = resxEmails.SupervisorConfirmingAttendanceMessage
                .Replace("$ReportTo", CMS.GetFullName(ReportToGUID, LAN))
                .Replace("$StaffName", CMS.GetFullName(UserGUID, LAN));
            if (LAN == "AR") { Message = "<p align='right'>" + Message + "</p>"; }

            string body_format = "HTML";
            string importance = "Normal";
            string _body = "<div style='font-family:Arial;'>" + Message.Replace("\r\n", "<br/>") + "</div>";
            Send(CMS.GetCurrentUserEmail(UserGUID), CMS.GetCurrentUserEmail(ReportToGUID), "", Subject, _body, body_format, importance, "", "Staff Attendance Management<HR-UNOPS@unhcr.org>");
        }
        public void SupervisorAttendancesNoConfirmation(Guid confirmedUserGUID)
        {

            string Subject = string.Empty;
            string Message = string.Empty;

            Subject = resxEmails.SupervisorConfirmingAttendanceSubject;

            Message = "Dear "+ CMS.GetFullName(confirmedUserGUID, LAN);
            Message += "<br/><br/>kindly check with your direct supervisor to clarify your attendance,";
            Message += "<br/><br/>Best Regards,";
            Message += "<br/>UNHCR Syria";
            if (LAN == "AR") { Message = "<p align='right'>" + Message + "</p>"; }

            string body_format = "HTML";
            string importance = "Normal";
            string _body = "<div style='font-family:Arial;'>" + Message.Replace("\r\n", "<br/>") + "</div>";
            Send(CMS.GetCurrentUserEmail(confirmedUserGUID), "", "", Subject, Message, body_format, importance, "", "Staff Attendance Management<HR-UNOPS@unhcr.org>");

        }

        public void BroadCastMail(DateTime date, Guid confirmedUserGUID)
        {

            string Subject = string.Empty;
            string Message = string.Empty;

            Subject = date.ToString("MMMM") + " " + date.Year + " Travel Allowance For Office Attendance";

            Message = "Dear " + CMS.GetFullName(confirmedUserGUID, LAN);
            Message += "<br/><br/>As part of the travel allowance Pay cycle calculations, you are kindly requested to follow the link below and update attandance dates inside the office,";
            Message += "<br/>During the TA cycle of " + date.AddMonths(1).ToString("MMMM") + " " + date.Year + ".";
            Message += "<br/>Failing to update your attendance details on the online system by COB, " + date.ToString("MMMM") + ",5 " + date.Year + " will put your travel allowance on-hold to the following month.";
            //Message += "<br/>Failing to update your attendance details on the online system by COB, 27 Dec 2022 will put your travel allowance on-hold to the following month.";
            Message += "<br/><br/><a href='https://prv.unhcrsyria.org/OSA/OfficeStaffAttendances/OfficeStaffAttendanceStaffCalendar/?Date=" + date.Year + "-" + date.Month + "'>Click here to review and inform your direct supervisor</a>";
            Message += "<br/><br/>Best Regards,";
            Message += "<br/>UNHCR Syria";
            if (LAN == "AR") { Message = "<p align='right'>" + Message + "</p>"; }

            string body_format = "HTML";
            string importance = "Normal";
            string _body = "<div style='font-family:Arial;'>" + Message.Replace("\r\n", "<br/>") + "</div>";
            Send(CMS.GetCurrentUserEmail(confirmedUserGUID), "", "", Subject, Message, body_format, importance, "", "Staff Attendance Management<HR-UNOPS@unhcr.org>");

        }
        #endregion
    }
}