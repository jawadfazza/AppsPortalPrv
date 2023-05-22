using AppsPortal.BaseControllers;
using AppsPortal.Data;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.Models;
using AppsPortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace AppsPortal.Areas.CMS.Controllers
{
    public class MobileController : PortalBaseController
    {
        // GET: CMS/Mobile
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public string LoginMobile(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var User = (from a in DbCMS.userAccounts
                            join b in DbCMS.userPasswords on a.UserGUID equals b.UserGUID
                            join c in DbCMS.userServiceHistory.Where(x => x.EmailAddress == model.EmailAddress) on a.UserGUID equals c.UserGUID
                            join d in DbCMS.userProfiles on c.ServiceHistoryGUID equals d.ServiceHistoryGUID
                            join e in DbCMS.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.UserGUID equals e.UserGUID into LJ1
                            from l1 in LJ1.DefaultIfEmpty()
                            orderby d.FromDate descending, b.ActivationDate descending //Very important to get the latest password.
                            select new
                            {
                                a.UserGUID,
                                c.EmailAddress,
                                d.UserProfileGUID,
                                d.OrganizationInstanceGUID,
                                b.Password,
                                a.AccountStatusID,
                                a.userServiceHistory.FirstOrDefault().ServiceHistoryGUID,
                                FullName = l1.FirstName + " " + l1.Surname
                            }).FirstOrDefault();

                if (User != null)
                {
                    string EncryptedPassword = User.Password; // This is the db encyrpted password
                    string EnteredPassword = HashingHelper.EncryptPassword(model.Password, Portal.GUIDToString(User.UserGUID));

                    if (EncryptedPassword != EnteredPassword)
                    {
                        return null;
                    }
                    #region Check Account Status ID
                    switch (User.AccountStatusID)
                    {
                        case 10:
                            //User Authenticated
                            CMS.SetUserToken(User.UserProfileGUID, Apps.CMS);
                            Session[SessionKeys.CurrentApp] = Apps.CMS; //Initial Apps is the CMS
                            dataAuditLogins dataAuditLogin = new dataAuditLogins();
                            dataAuditLogin.UserProfileGUID = User.UserProfileGUID;
                            dataAuditLogin.UserAuditGUID = Guid.NewGuid();
                            dataAuditLogin.LoginTime = DateTime.Now;
                            DbCMS.dataAuditLogins.Add(dataAuditLogin);
                            DbCMS.SaveChanges();

                            return (new JavaScriptSerializer().Serialize(new List<Object>() { User }));

                        case -1:
                            break;
                    }
                    #endregion
                }

            }
            return "";
        }
    }
}