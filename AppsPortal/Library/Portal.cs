using AppsPortal.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Expressions;
using System.Web.Configuration;
using AppsPortal.ViewModels;
using System.Collections.ObjectModel;
using System.Drawing;

namespace AppsPortal.Library
{
    public class Portal
    {

        public Portal()
        {

        }

        public Guid UserID()
        {
            if (HttpContext.Current.Session[SessionKeys.UserGUID] != null)
                return Guid.Parse(HttpContext.Current.Session[SessionKeys.UserGUID].ToString());
            else
                return Guid.Empty;
        }
        public Guid UserProfileGUID()
        {
            if (HttpContext.Current.Session[SessionKeys.UserProfileGUID] != null)
                return Guid.Parse(HttpContext.Current.Session[SessionKeys.UserProfileGUID].ToString());
            else
                return Guid.Empty;
        }

        public string GetUserProfilePhoto(Guid _userGUID)
        {
            string path = "";
            string UserID = _userGUID.ToString();
            string _check = "/Uploads/ORG/StaffPhotos/" + _userGUID + "/" + _userGUID + ".jpg";
            string _check1 = "/Uploads/ORG/StaffPhotos/" + _userGUID + "/" + _userGUID + ".png";
            if (System.IO.File.Exists(_check))
            {

                path = "/Uploads/ORG/StaffPhotos/" + UserID + "/" + UserID + ".jpg";
            }
            else if (System.IO.File.Exists(_check1))

            {
                path = "/Uploads/ORG/StaffPhotos/" + UserID + "/" + UserID + ".jpg";
            }
            else
            {
                path = "/Uploads/ORG/StaffPhotos/" + _userGUID + "/" + _userGUID + ".jpg";
            }
            return path;
        }

        internal string formatDate(DateTime dt)
        {
            IFormatProvider formatProvider = new System.Globalization.CultureInfo(Languages.CurrentCulture());
            return Localization.NumberFormat(dt.ToString(Localization.Date()));
        }

        public string ProfilePhoto()
        {
            string path = "";
            string UserID = this.UserID().ToString();
            if (System.IO.File.Exists(WebConfigurationManager.AppSettings["DataFolder"] + "\\Users\\ProfilePhotos\\" + UserID + ".jpg"))
            {
                path = WebConfigurationManager.AppSettings["MediaURL"] + "Users/ProfilePhotos/" + UserID + ".jpg";
            }
            else
            {
                path = Constants.UserProfilePhotoTemplate;
            }
            return path;
        }

        public string ProfilePhoto(Guid UserGUID)
        {
            string path = "";
            if (System.IO.File.Exists(WebConfigurationManager.AppSettings["DataFolder"] + "\\Users\\ProfilePhotos\\" + UserGUID.ToString() + ".jpg"))
            {
                path = WebConfigurationManager.AppSettings["MediaURL"] + "Users/ProfilePhotos/" + UserGUID.ToString() + ".jpg";
            }
            else
            {
                path = Constants.UserProfilePhotoTemplate;
            }
            return path;
        }

        public string OfficePhoto(Guid OfficeGUID)
        {
            string path = "";
            if (System.IO.File.Exists(WebConfigurationManager.AppSettings["DataFolder"] + "\\Offices\\" + OfficeGUID.ToString() + ".jpg"))
            {
                path = WebConfigurationManager.AppSettings["MediaURL"] + "Offices/" + OfficeGUID.ToString() + ".jpg";
            }
            else
            {
                path = Constants.NoPhotoTemplate;
            }
            return path;
        }

        public string OrganizationLogo(Guid OrganizationGUID)
        {
            string path = "";
            if (System.IO.File.Exists(WebConfigurationManager.AppSettings["DataFolder"] + "\\Logos\\" + OrganizationGUID.ToString() + ".png"))
            {
                path = WebConfigurationManager.AppSettings["MediaURL"] + "logos/" + OrganizationGUID.ToString() + ".png";
            }
            else
            {
                path = Constants.NoPhotoTemplate;
            }
            return path;
        }

        public static object DataTable(int Total, object Result)
        {
            return new
            {
                iTotalRecords = Total,
                iTotalDisplayRecords = Total,
                aaData = Result
            };
        }

        public string GetControllerName(ControllerContext controllerContext)
        {
            return controllerContext.RouteData.Values["controller"].ToString();
        }

        public string GUIDToString(Guid? _GUID)
        {
            return _GUID.ToString().Replace("-", "");
        }

        public Guid StringToGUID(string _str)
        {
            Guid g = Guid.Empty;
            if (Guid.TryParseExact(_str, "N", out g))
            {
                return g;
            }
            else
            {
                //Forged GUID
                throw new HttpException(404, "Not found");
            }
        }

        public string JobTitle(string JobTitle)
        {
            return string.IsNullOrEmpty(JobTitle) ? "Unknown Job Title" : JobTitle;
        }

        public List<T> SingleToList<T>(T model) where T : class
        {
            return new List<T> { model };
        }

        public string Cookie(string CookieName, string Key)
        {
            if (HttpContext.Current.Request.Cookies[CookieName] != null)
            {
                return HttpContext.Current.Server.HtmlEncode(HttpContext.Current.Request.Cookies[CookieName][Key]);
            }
            return "";
        }

        public DateTime? LocalTime(DateTime? value)
        {
            if (value == null) return null;
            TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Syria Standard Time");
            DateTime dtz = TimeZoneInfo.ConvertTimeFromUtc((DateTime)value, tz);
            return dtz;
        }

        public DateTime? UTCTime(DateTime? value)
        {
            if (value == null) return null;
            TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Syria Standard Time");
            DateTime dtz = TimeZoneInfo.ConvertTimeToUtc((DateTime)value, tz);
            return dtz;
        }

        public void CutFile(string source, string destination)
        {
            if (System.IO.File.Exists(source))
            {
                System.IO.File.Copy(HttpContext.Current.Server.MapPath(source), WebConfigurationManager.AppSettings["DataFolder"] + destination);
                System.IO.File.Delete(HttpContext.Current.Server.MapPath(source));
            }
        }

        public List<Guid> CSVToGUIDList(string csv)
        {
            return Array.ConvertAll(csv.Split(','), s => new Guid(s)).ToList();
        }

        public PartialViewModel PartialView(Guid EntityPK, string ActionName, ControllerContext controllerContext, string Container)
        {
            return new PartialViewModel
            {
                PK = EntityPK.ToString(),
                Url = new UrlHelper(HttpContext.Current.Request.RequestContext).Action(ActionName, GetControllerName(controllerContext)),
                Container = Container,
                Active = true
            };
        }
        public PartialViewModel PartialView(Guid EntityPK, string ActionName, string controllerName, string Container)
        {
            return new PartialViewModel
            {
                PK = EntityPK.ToString(),
                Url = new UrlHelper(HttpContext.Current.Request.RequestContext).Action(ActionName, controllerName),
                Container = Container,
                Active = true
            };
        }
        public string TimeZoneCity(string TimeZone)
        {
            ReadOnlyCollection<TimeZoneInfo> Zones = TimeZoneInfo.GetSystemTimeZones();

            string te = Zones.Where(x => x.Id == TimeZone).FirstOrDefault().DisplayName;

            return te;
        }


        #region Image Scale
        public Image ScaleImage(Image image, int maxWidth, int maxHeight)
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



    }
}