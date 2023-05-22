using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web.Configuration;
using System.Web.Helpers;
using System.Web.Mvc;

namespace FineUploader
{
    public class UploadController : Controller
    {
        [HttpPost]
        public FineUploaderResult UploadOrganizationLogo(FineUpload upload, string PK)
        {

            //When Create New Organization
            string FileNameOnNew = Guid.NewGuid().ToString() + GetFileExtention(upload.FileName);
            string SavePathOnNew = Server.MapPath(@"\\Uploads\\" + FileNameOnNew);

            //When Updating an Exists Organization
            string FileNameOnEdit = PK + ".png"; //GetFileExtention(upload.FileName);
            string SavePathOnEdit = WebConfigurationManager.AppSettings["MediaURL"] + "\\Logos\\" + FileNameOnEdit;

            string SavePath = "";
            string ReturnPath = "";

            //New
            if (Guid.Parse(PK) == Guid.Empty)
            {
                SavePath = SavePathOnNew;
                ReturnPath = "/Uploads/" + FileNameOnNew;
            }
            //Update
            else
            {
                SavePath = SavePathOnEdit;
                ReturnPath = WebConfigurationManager.AppSettings["MediaURL"] + "Logos/" + FileNameOnEdit;
            }
            try
            {
                using (var image = Image.FromStream(upload.InputStream))
                using (var newImage = ScaleImage(image, 200, 200))
                {
                    var directory = new FileInfo(SavePath).Directory;
                    if (directory != null) directory.Create();
                    newImage.Save(SavePath, ImageFormat.Png);
                }
            }
            catch (Exception ex)
            {
                return new FineUploaderResult(false, error: ex.Message);
            }

            // the anonymous object in the result below will be convert to json and set back to the browser
            return new FineUploaderResult(true, new { path = ReturnPath });
        }

        [HttpPost]
        public JsonResult UploadProfilePhoto(FineUpload upload)
        {
            string UserID = new AppsPortal.Library.Portal().UserID().ToString();

            try
            {
                using (var image = Image.FromStream(upload.InputStream))
                {
                    //Original Size
                    var dir = new FileInfo(Server.MapPath(WebConfigurationManager.AppSettings["MediaURL"] + "\\Users\\ProfilePhotos\\")).Directory;
                    if (dir != null) dir.Create();
                    image.Save(Server.MapPath(WebConfigurationManager.AppSettings["MediaURL"] + "\\Users\\ProfilePhotos\\LG_" + UserID + ".jpg"), ImageFormat.Jpeg);

                    //1024 x 1024 pixel
                    using (var newImage = ScaleImage(image, 300, 300))
                    {
                        var directory = new FileInfo(Server.MapPath(WebConfigurationManager.AppSettings["MediaURL"] + "\\Users\\ProfilePhotos\\")).Directory;
                        if (directory != null) directory.Create();
                        newImage.Save(Server.MapPath(WebConfigurationManager.AppSettings["MediaURL"] + "\\Users\\ProfilePhotos\\" + UserID + ".jpg"), ImageFormat.Jpeg);
                    }
                    //100 x 100 pixel
                    using (var newImage = ScaleImage(image, 100, 100))
                    {
                        var directory = new FileInfo(Server.MapPath(WebConfigurationManager.AppSettings["MediaURL"] + "\\Users\\ProfilePhotos\\")).Directory;
                        if (directory != null) directory.Create();
                        newImage.Save(Server.MapPath(WebConfigurationManager.AppSettings["MediaURL"] + "\\Users\\ProfilePHotos\\XS_" + UserID + ".jpg"), ImageFormat.Jpeg);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { Error = ex.Message });
            }

            string HeaderPath = WebConfigurationManager.AppSettings["MediaURL"] + "Users/ProfilePhotos/XS_" + UserID + ".jpg";
            string ProfilePath = WebConfigurationManager.AppSettings["MediaURL"] + "Users/ProfilePhotos/" + UserID + ".jpg";

            // the anonymous object in the result below will be convert to json and set back to the browser
            return Json(new { HeaderPath = HeaderPath, ProfilePath = ProfilePath });
        }



        [HttpPost]
        public FineUploaderResult UploadOfficePhoto(FineUpload upload, string PK)
        {
            //When Create New
            string FileNameOnNew = Guid.NewGuid().ToString() + ".jpg";
            string SavePathOnNew = Server.MapPath(@"\\Uploads\\" + FileNameOnNew);

            //When Updating an Exists 
            string FileNameOnEdit = PK + ".jpg";
            string SavePathOnEdit = WebConfigurationManager.AppSettings["MediaURL"] + "\\Offices\\" + FileNameOnEdit;

            string SavePath = "";
            string ReturnPath = "";

            //New
            if (Guid.Parse(PK) == Guid.Empty)
            {
                SavePath = SavePathOnNew;
                ReturnPath = "/Uploads/" + FileNameOnNew;
            }
            //Update
            else
            {
                SavePath = SavePathOnEdit;
                ReturnPath = WebConfigurationManager.AppSettings["MediaURL"] + "Offices/" + FileNameOnEdit;
            }
            try
            {
                using (var image = Image.FromStream(upload.InputStream))
                {
                    var directory = new FileInfo(SavePath).Directory;
                    if (directory != null) directory.Create();
                    image.Save(SavePath, ImageFormat.Jpeg);
                }
            }
            catch (Exception ex)
            {
                return new FineUploaderResult(false, error: ex.Message);
            }

            // the anonymous object in the result below will be convert to json and set back to the browser
            return new FineUploaderResult(true, new { path = ReturnPath });
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

        private string GetFileExtention(string FileName)
        {
            string Extension = "";
            if (FileName.Length > 0)
            {
                string segment = FileName.Substring(FileName.Length - 5, 5);
                Extension = segment.Substring(segment.IndexOf("."), (segment.Length - segment.IndexOf(".")));
            }
            return Extension;
        }

        #region PPA

        #endregion
    }
}