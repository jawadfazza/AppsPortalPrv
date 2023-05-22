using AppsPortal.BaseControllers;
using Aspose.Pdf;
using Aspose.Pdf.Devices;
using IronPdf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.DAS.Controllers
{
    public class MigrationController : DASBaseController
    {
        // GET: DAS/Migration
        public ActionResult Index()
        {
            IronPdf.License.LicenseKey = "IRONPDF.JAWADALFAZZAA.25146-2CD5980931-C66TOSMX64ERBTYA-GN2UXPMKZMQM-HWQVZEBH3HG6-6COIT5E6RXAP-X3P4UTDAMXOG-JDGTTD-TJS27HBBR5OBUA-DEPLOYMENT.TRIAL-OXMKG5.TRIAL.EXPIRES.29.SEP.2021";

            // Set Application scope Temp Files Path.  
            // This changes System.IO.Path.GetTempFileName and System.IO.Path.GetTempPath behavior for the entire .NET application
            //var MyTempPath = @"C:\Safe\Path\";
            //Environment.SetEnvironmentVariable("TEMP", MyTempPath, EnvironmentVariableTarget.Process);
            //Environment.SetEnvironmentVariable("TMP", MyTempPath, EnvironmentVariableTarget.Process);
            //// Set IronPDF Temp Path
            //IronPdf.Installation.TempFolderPath = System.IO.Path.Combine(MyTempPath, "IronPdf");


            string pdfPath = Server.MapPath("~/Uploads/DAS/Migration/Files/303-15C00300.pdf");
            //var Pdf = PdfDocument.FromFile(pdfPath);
           
            ////Extract all pages to a folder as image files
            //Pdf.RasterizeToImageFiles(@"C:\image\folder\*.png");

            ////Dimensions and page ranges may be specified
            //Pdf.RasterizeToImageFiles(@"C:\image\folder\thumbnail_*.jpg", 100, 80);


            //Extract all pages as System.Drawing.Bitmap objects
            //System.Drawing.Bitmap[] pageImages = Pdf.ToBitmap(); var ChromePdfRenderer = new ChromePdfRenderer();
            
            
            var pdfDocument = PdfDocument.FromFile(pdfPath);
            //Get all text
            var allText = pdfDocument.ExtractAllText();
            //Get all Images
            var allImages = pdfDocument.ExtractAllImages();
            //Or even find the images and text by page
            for (var index = 0; index < pdfDocument.PageCount; index++)
            {
                var pageNumber = index + 1;
                var pageText = pdfDocument.ExtractTextFromPage(index);
                var pageImages = pdfDocument.ExtractImagesFromPage(index).FirstOrDefault();
                pageImages.Save(@"C:\image\folder\" + index + ".png");
            }





            //TiffImage myTiff = new TiffImage("D:\\Some.tif");

            //// Open PDF document
            //Document pdfDocument = new Document(Server.MapPath("~/Uploads/DAS/Migration/Files/303-15C00300.pdf"));

            //// Loop through each page
            //foreach (var page in pdfDocument.Pages)
            //{
            //    // Create file stream for output image
            //    using (FileStream imageStream = new FileStream(Server.MapPath("~/Uploads/DAS/Migration/Temp/") + string.Format("page_{0}.png", page.Number), FileMode.Create))
            //    {
            //        // Create Resolution object
            //        Resolution resolution = new Resolution(300);

            //        // Create Png device with specified attributes
            //        // Width, Height, Resolution
            //        PngDevice PngDevice = new PngDevice(500, 700, resolution);

            //        // Convert a particular page and save the image to stream
            //        PngDevice.Process(page, imageStream);

            //        //StreamReader sr = new StreamReader(imageStream);
            //        //using (var fileStream = System.IO.File.Create(Server.MapPath("~/Uploads/DAS/Migration")))
            //        //{
            //        //    imageStream.Seek(0, SeekOrigin.Begin);
            //        //    imageStream.CopyTo(fileStream);
            //        //}
            //        // Close stream
            //        imageStream.Close();
            //    }
            //}
            return View();
        }

        public class TiffImage
        {
            private string myPath;
            private Guid myGuid;
            private FrameDimension myDimension;
            public ArrayList myImages = new ArrayList();
            private int myPageCount;
            private Bitmap myBMP;

            public TiffImage(string path)
            {
                MemoryStream ms;
                System.Drawing.Image myImage;

                myPath = path;
                FileStream fs = new FileStream(myPath, FileMode.Open);
                myImage = System.Drawing.Image.FromStream(fs);
                myGuid = myImage.FrameDimensionsList[0];
                myDimension = new FrameDimension(myGuid);
                myPageCount = myImage.GetFrameCount(myDimension);
                for (int i = 0; i < myPageCount; i++)
                {
                    ms = new MemoryStream();
                    myImage.SelectActiveFrame(myDimension, i);
                    myImage.Save(ms, ImageFormat.Bmp);
                    myBMP = new Bitmap(ms);
                    myImages.Add(myBMP);
                    ms.Close();
                }
                fs.Close();
            }
        }
    }
}