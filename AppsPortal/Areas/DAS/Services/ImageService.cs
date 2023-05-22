using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace AppsPortal.Services
{
    public class ImageService
    {

        public byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            return ms.ToArray();
        }
        public Image ByteArrayToImage(byte[] byteArrayIn,Guid individualGuid,string path)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            //IndividualImages
            returnImage.Save(HttpContext.Current.Server.MapPath("~/"+path+"/" + individualGuid.ToString() + ".jpg"));
            return returnImage;
        }
    }
}