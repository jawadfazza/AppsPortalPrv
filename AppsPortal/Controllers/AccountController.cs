using AppsPortal.BaseControllers;
using AppsPortal.Data;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.Models;
using AppsPortal.ViewModels;
using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace AppsPortal.Controllers
{
    public class AccountController : PortalBaseController
    {

      
        //public string Encrypt(string clearText)
        //{
        //    string password = "MAKV2SPBNI99212";
        //    byte[] bytes = Encoding.Unicode.GetBytes(clearText);
        //    using (Aes aes = Aes.Create())
        //    {
        //        Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, new byte[13]
        //        {
        //  (byte) 73,
        //  (byte) 118,
        //  (byte) 97,
        //  (byte) 110,
        //  (byte) 32,
        //  (byte) 77,
        //  (byte) 101,
        //  (byte) 100,
        //  (byte) 118,
        //  (byte) 101,
        //  (byte) 100,
        //  (byte) 101,
        //  (byte) 118
        //        });
        //        aes.Key = rfc2898DeriveBytes.GetBytes(32);
        //        aes.IV = rfc2898DeriveBytes.GetBytes(16);
        //        using (MemoryStream memoryStream = new MemoryStream())
        //        {
        //            using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
        //            {
        //                cryptoStream.Write(bytes, 0, bytes.Length);
        //                cryptoStream.Close();
        //            }
        //            clearText = Convert.ToBase64String(memoryStream.ToArray());
        //        }
        //    }
        //    return clearText;
        //}
        public ActionResult Index()
        {
            
            HttpCookie EmailAddress = this.ControllerContext.HttpContext.Request.Cookies["EmailAddress"];
            if (EmailAddress != null)
            {
                HttpCookie Password = this.ControllerContext.HttpContext.Request.Cookies["Password"];
                LoginModel model = new LoginModel();
                model.EmailAddress = EmailAddress.Value.ToString();
                model.Password = Password.Value.ToString();
                model.RememberMe = true;
                return Login(model, null);
            }
            else
            {
                if (Session[SessionKeys.UserGUID] != null)
                {
                    return RedirectToAction("Index", "Home", false);
                }
                else
                {
                    if (Request.IsAjaxRequest())
                    {
                        return PartialView("~/Views/Account/LoginModal.cshtml");
                    }
                    else
                    {
                        return View("Login");
                    }
                }
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string GK)
        {
            if (ModelState.IsValid)
            {
                bool isADauthentication = model.EmailAddress.ToLower().EndsWith("@unhcr.org");

                Guid CMSAppGUID = Apps.CMS;
                bool bSucceeded = false;
                string strAuthenticatedBy;
                string strError;
                if (isADauthentication)
                {
                    //authenticate from AD

                    using (DirectoryEntry adsEntry = new DirectoryEntry("LDAP://unhcr.local/OU=Organisation,DC=UNHCR,DC=LOCAL", model.EmailAddress.Split('@')[0].ToString(), model.Password, AuthenticationTypes.Secure))
                    {


                        using (DirectorySearcher adsSearcher = new DirectorySearcher(adsEntry))
                        {
                            ///////////adsSearcher.Filter = "(&(objectClass=user)(objectCategory=person))";
                            //adsSearcher.Filter = "(sAMAccountName=" + model.EmailAddress.Split('@')[0] + ")";

                            try
                            {
                                //SearchResult adsSearchResult = adsSearcher.FindOne();
                                bSucceeded = true;

                                strAuthenticatedBy = "Active Directory";
                                strError = "User has been authenticated by Active Directory.";
                            }
                            catch (Exception ex)
                            {
                                // Failed to authenticate. Most likely it is caused by unknown user
                                // id or bad strPassword.
                                strError = ex.Message;
                                ModelState.AddModelError("EmailAddress", "Username or Password is incorrect");
                                return View("Login");
                            }
                            finally
                            {
                                adsEntry.Close();
                            }
                        }
                    }
                }


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
                    if (!isADauthentication)
                    {
                        string EncryptedPassword = User.Password; // This is the db encyrpted password
                        string EnteredPassword = HashingHelper.EncryptPassword(model.Password, Portal.GUIDToString(User.UserGUID));

                        if (EncryptedPassword != EnteredPassword)
                        {
                            ModelState.AddModelError("Password", "Username or Password is incorrect");
                            return View("Login");
                        }
                    }


                    //User GUID is needed for Accept term and complete profile info.
                    System.Web.HttpContext.Current.Session[SessionKeys.UserGUID] = User.UserGUID;
                    System.Web.HttpContext.Current.Session[SessionKeys.OrganizationInstanceGUID] = User.OrganizationInstanceGUID;
                    System.Web.HttpContext.Current.Session[SessionKeys.FullName] = User.FullName;
                    System.Web.HttpContext.Current.Session[SessionKeys.UserProfileGUID] = User.UserProfileGUID;
                    System.Web.HttpContext.Current.Session[SessionKeys.ServiceHistoryGUID] = User.ServiceHistoryGUID;
                    System.Web.HttpContext.Current.Session[SessionKeys.EmailAddress] = User.EmailAddress;
                    System.Web.HttpContext.Current.Session["FirstLogin"] = "True";
                    List<Guid> OnlineUsers = (List<Guid>)System.Web.HttpContext.Current.Application["OnlineUsers"];
                    if (OnlineUsers.Where(x => x == User.UserGUID).Count() == 0)
                    {
                        OnlineUsers.Add(User.UserGUID);
                    }
                    System.Web.HttpContext.Current.Application["OnlineUsers"] = OnlineUsers;



                    #region Check Account Status ID
                    switch (User.AccountStatusID)
                    {
                        case 10:

                            //User Authenticated
                            CMS.SetUserToken(User.UserProfileGUID, Apps.CMS);
                            System.Web.HttpContext.Current.Session[SessionKeys.CurrentApp] = Apps.CMS;
                            CMS.BuildUserMenus(User.UserGUID, LAN);


                            //model.BrowserName = Request.UserAgent;
                            //model.UserHostAddress = Request.UserHostAddress;
                            //model.UserHostName = Request.UserHostName;


                            #region New code for audit login

                            #endregion

                            #region OLD SLOW CODE
                            //IPApiResult geoIP = new IPApiResult();
                            //try
                            //{
                            //    HttpWebRequest ApiRequest = (HttpWebRequest)WebRequest.Create("http://getcitydetails.geobytes.com/GetCityDetails?fqcn=" + Request.UserHostAddress);

                            //    ApiRequest.Method = "GET";
                            //    ApiRequest.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                            //    HttpWebResponse ApiResponse = (HttpWebResponse)ApiRequest.GetResponse();

                            //    string ApiContent = string.Empty;
                            //    using (Stream stream = ApiResponse.GetResponseStream())
                            //    {
                            //        using (StreamReader sr = new StreamReader(stream))
                            //        {
                            //            ApiContent = sr.ReadToEnd();
                            //        }
                            //    }

                            //    geoIP = JsonConvert.DeserializeObject<IPApiResult>(ApiContent);
                            //}
                            //catch
                            //{
                            //    //Do thing.
                            //}
                            #endregion



                            HttpBrowserCapabilitiesBase request = Request.Browser;

                            Models.dataAuditLogins dataAuditLogin = new Models.dataAuditLogins()
                            {
                                UserAuditGUID = Guid.NewGuid(),
                                UserProfileGUID = User.UserProfileGUID,
                                LoginTime = DateTime.Now,
                                IsMobileDevice = request.IsMobileDevice,
                                Browser = Request.Browser.Browser + " " + Request.Browser.Version,
                                Environment = GetUserPlatform(Request),
                                MobileDeviceManufacturer = request.MobileDeviceManufacturer,
                                MobileDeviceModel = request.MobileDeviceModel,



                                UserHostAddress = Request.UserHostAddress,
                                ComputerName = GetComputerName(Request.UserHostAddress),
                                //CountryCode = geoIP.CountryCode,
                                //CountryName = geoIP.CountryName,
                                //City = geoIP.City,
                                //TimeZone = geoIP.TimeZone,
                                //Latitude = geoIP.Latitude,
                                //Longitude = geoIP.Longitude



                            };

                            DbCMS.dataAuditLogins.Add(dataAuditLogin);
                            DbCMS.SaveChanges();

                            if (Request.IsAjaxRequest())
                            {
                                JsonReturn jr = new JsonReturn()
                                {
                                    Notify = new Notify { Type = MessageTypes.Success, Message = "Successfully login...Keep working" }
                                };
                                return Json(jr);
                            }
                            else
                            {
                                if (model.RememberMe)
                                {
                                    //add Cookie to remeber the user
                                    HttpCookie EmailAddress = new HttpCookie("EmailAddress");
                                    EmailAddress.Value = model.EmailAddress;
                                    EmailAddress.Expires = DateTime.Now.AddMonths(1);
                                    EmailAddress.Secure = true;

                                    HttpCookie Password = new HttpCookie("Password");
                                    Password.Value = model.Password;
                                    Password.Expires = DateTime.Now.AddMonths(1);
                                    Password.Secure = true;

                                    this.ControllerContext.HttpContext.Response.Cookies.Add(EmailAddress);
                                    this.ControllerContext.HttpContext.Response.Cookies.Add(Password);
                                }
                                try
                                {
                                    string RedirectURL = System.Web.HttpContext.Current.Session["RedirectURL"].ToString();
                                    if (RedirectURL != "")
                                    {
                                        System.Web.HttpContext.Current.Session["RedirectURL"] = "";
                                        return Redirect(RedirectURL);
                                    }
                                    else
                                    {
                                        return RedirectToAction("Index", "Home", false);
                                    }

                                }
                                catch
                                {

                                }

                                return RedirectToAction("Index", "Home", false);
                            }
                        case 5:
                            return RedirectToAction("TermOfUse", "Account");
                        case 0:
                            break;
                        case -1:
                            break;
                    }
                    #endregion


                }
                else
                {
                    ModelState.AddModelError("EmailAddress", "Username or Password is incorrect");
                    return View("Login");
                }
            }
            else //Model is not valid
            {
                return View("Login");
            }
            return View(); //This will never reached, it only to avoid the error not all code paths return a value
        }

        public ActionResult Logout()
        {
            if (UserProfileGUID != Guid.Empty)
            {
                var dataAuditLogin = (from a in DbCMS.dataAuditLogins
                                      where a.UserProfileGUID == UserProfileGUID
                                      orderby a.LoginTime descending
                                      select a).FirstOrDefault();
                dataAuditLogin.LogoutTime = DateTime.Now;
                DbCMS.SaveChanges();

                //remove cookie to forget the user
                HttpCookie EmailAddress = this.ControllerContext.HttpContext.Request.Cookies["EmailAddress"];
                if (EmailAddress != null)
                {
                    EmailAddress.Expires = DateTime.Now.AddMonths(-1);
                    HttpCookie Password = this.ControllerContext.HttpContext.Request.Cookies["Password"];
                    Password.Expires = DateTime.Now.AddMonths(-1);

                    this.ControllerContext.HttpContext.Response.Cookies.Add(EmailAddress);
                    this.ControllerContext.HttpContext.Response.Cookies.Add(Password);
                }
            }
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Index", "Account");
        }

        public ActionResult SessionTimeout()
        {
            return View();
        }

        public ActionResult TermOfUse()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult TermOfUse(string ImJustNothing)
        {
            return RedirectToAction("CompleteProfile", "Account");
        }

        public ActionResult CompleteProfile()

        {
            var model = (from a in DbCMS.userPersonalDetails.Where(x => x.UserGUID == UserGUID && x.Active)
                         join b in DbCMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN) on a.UserGUID equals b.UserGUID into LJ1
                         from R1 in LJ1.DefaultIfEmpty()
                         select new PersonalDetailsUpdateModel
                         {
                             UserGUID = a.UserGUID,
                             FirstName = R1.FirstName,
                             Surname = R1.Surname,
                             Active = a.Active,
                             userPersonalDetailsRowVersion = a.userPersonalDetailsRowVersion,
                             userPersonalDetailsLanguageRowVersion = R1.userPersonalDetailsLanguageRowVersion,
                         }).FirstOrDefault();

            return View("~/Views/Account/CompleteProfile.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CompleteProfile(PersonalDetailsUpdateModel model)
        {
            if (!ModelState.IsValid) return PartialView("~/Views/Account/CompleteProfile.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            model.UserGUID = UserGUID;

            Models.userPersonalDetails userPersonalDetails = new Models.userPersonalDetails();

            Mapper.Map(model, userPersonalDetails);

            DbCMS.Update(userPersonalDetails, Guid.Parse("00000000-0000-0000-0000-000000000001")
                , ExecutionTime);

            var UserDetailsLanguage = DbCMS.userPersonalDetailsLanguage.Where(u => u.UserGUID == UserGUID && u.LanguageID == LAN).FirstOrDefault();

            if (UserDetailsLanguage != null)
            {
                Mapper.Map(model, UserDetailsLanguage);
                DbCMS.Update(UserDetailsLanguage, Guid.Parse("00000000-0000-0000-0000-000000000001"), ExecutionTime);
            }
            else
            {
                UserDetailsLanguage = Mapper.Map<Models.userPersonalDetailsLanguage>(model);
                UserDetailsLanguage.PersonalDetailsLanguageGUID = Guid.NewGuid();
                UserDetailsLanguage.LanguageID = LAN;
                UserDetailsLanguage.UserGUID = UserGUID;
                DbCMS.Create(UserDetailsLanguage, Guid.Parse("00000000-0000-0000-0000-000000000001"), ExecutionTime);
            }

            Guid UserGuid = Guid.Parse(Session[SessionKeys.UserGUID].ToString());
            var user = DbCMS.userAccounts.Find(UserGuid);
            user.AccountStatusID = 10;
            Session[SessionKeys.CurrentApp] = Apps.CMS;

            try
            {
                DbCMS.SaveChanges();
                CMS.BuildUserMenus(UserGUID, LAN);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                return Json(JsonMessages.ErrorMessage(DbCMS, s));
            }


        }

        [HttpGet]
        public ActionResult ForgetPassword()
        {

            return null;
            return View();
        }

        [HttpPost]
        public ActionResult VerifyEmail(VerifyEmail model)
        {
            var userModel = new UserModel();

            var userServiceHistory = DbCMS.userServiceHistory.Where(x => x.EmailAddress == model.EmailAddress).FirstOrDefault();
            var PersonalDetails = userServiceHistory.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active).FirstOrDefault();
            Mapper.Map(userServiceHistory, userModel);
            Mapper.Map(PersonalDetails, userModel);

            new Email().SendVerifyEmailForgetPassword(userModel);
            return RedirectToAction("EmailVerificationMessage", "Account", new { PK = Portal.GUIDToString(userModel.UserGUID) });
        }

        [HttpGet]
        public ActionResult SetupPassword()
        {
            return View();
        }

        public ActionResult SetupPassword(SetupPasswordModel model, string PK)
        {
            Guid resetToken = Portal.StringToGUID(PK);
            var account = DbCMS.userAccounts.Where(x => x.PasswordResetToken == resetToken).FirstOrDefault();
            if (account != null)
            {
               
                var oldPassword = DbCMS.userPasswords.Where(up => up.UserGUID == account.UserGUID && up.Active).OrderByDescending(up => up.ActivationDate).FirstOrDefault();
                if (oldPassword.Password == HashingHelper.EncryptPassword(model.Password, Portal.GUIDToString(account.UserGUID))){
                    return Json(JsonMessages.ErrorMessage(DbCMS, "To maintain account security, choose unique password not used before."));
                }
                else {
                    oldPassword.Active = false;

                    userPasswords newPassword = new userPasswords();
                    newPassword.ActivationDate = DateTime.Now;
                    newPassword.Password = HashingHelper.EncryptPassword(model.Password, Portal.GUIDToString(account.UserGUID));
                    newPassword.UserGUID = account.UserGUID;
                    newPassword.Active = true;
                    DbCMS.userPasswords.Add(newPassword);
                    try
                    {
                        DbCMS.SaveChanges();
                        return RedirectToAction("Index", "Account");
                    }
                    catch (Exception ex)
                    {
                        string s = ex.Message;
                        return Json(JsonMessages.ErrorMessage(DbCMS, s));
                    }
                }
            }
            else
            {
                return Json(JsonMessages.ErrorMessage(DbCMS, "Account Not Found!"));
            }
        }

        public ActionResult EmailVerificationMessage(Guid PK)
        {
            var userModel = new UserModel();

            var userServiceHistory = DbCMS.userServiceHistory.Where(x => x.UserGUID == PK).FirstOrDefault();
            var PersonalDetails = userServiceHistory.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active).FirstOrDefault();
            Mapper.Map(userServiceHistory, userModel);
            Mapper.Map(PersonalDetails, userModel);
            return View(userModel);
        }

        public static string GetComputerName(string IP)
        {
            try
            {
                IPAddress myIP = IPAddress.Parse(IP);
                IPHostEntry GetIPHost = Dns.GetHostEntry(myIP);
                return GetIPHost.HostName.ToString().Split('.').First();
            }
            catch
            {
                return "Unknown";
            }
        }

        public String GetUserEnvironment(HttpRequestBase request)
        {
            var browser = request.Browser;
            var platform = GetUserPlatform(request);
            return string.Format("{0} {1} / {2}", browser.Browser, browser.Version, platform);
        }

        public String GetUserPlatform(HttpRequestBase request)
        {
            var ua = request.UserAgent;

            if (ua.Contains("Android"))
                return string.Format("Android {0}", GetMobileVersion(ua, "Android"));

            if (ua.Contains("iPad"))
                return string.Format("iPad OS {0}", GetMobileVersion(ua, "OS"));

            if (ua.Contains("iPhone"))
                return string.Format("iPhone OS {0}", GetMobileVersion(ua, "OS"));

            if (ua.Contains("Linux") && ua.Contains("KFAPWI"))
                return "Kindle Fire";

            if (ua.Contains("RIM Tablet") || (ua.Contains("BB") && ua.Contains("Mobile")))
                return "Black Berry";

            if (ua.Contains("Windows Phone"))
                return string.Format("Windows Phone {0}", GetMobileVersion(ua, "Windows Phone"));

            if (ua.Contains("Mac OS"))
                return "Mac OS";

            if (ua.Contains("Windows NT 5.1") || ua.Contains("Windows NT 5.2"))
                return "Windows XP";

            if (ua.Contains("Windows NT 6.0"))
                return "Windows Vista";

            if (ua.Contains("Windows NT 6.1"))
                return "Windows 7";

            if (ua.Contains("Windows NT 6.2"))
                return "Windows 8";

            if (ua.Contains("Windows NT 6.3"))
                return "Windows 8.1";

            if (ua.Contains("Windows NT 10"))
                return "Windows 10";

            //fallback to basic platform:
            return request.Browser.Platform + (ua.Contains("Mobile") ? " Mobile " : "");
        }

        public String GetMobileVersion(string userAgent, string device)
        {
            var temp = userAgent.Substring(userAgent.IndexOf(device) + device.Length).TrimStart();
            var version = string.Empty;

            foreach (var character in temp)
            {
                var validCharacter = false;
                int test = 0;

                if (Int32.TryParse(character.ToString(), out test))
                {
                    version += character;
                    validCharacter = true;
                }

                if (character == '.' || character == '_')
                {
                    version += '.';
                    validCharacter = true;
                }

                if (validCharacter == false)
                    break;
            }

            return version;
        }
    }
}