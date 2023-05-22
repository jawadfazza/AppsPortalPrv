using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Security.Cryptography;
using System.Text;

namespace AppsPortal.Services
{
    public class ConvertToPDF
    {
        public void ConvertToPdf( string ImagesPath, string PDFPath,string PDFName)

        {
            //for define PDFWriter
            string FileName = PDFPath + PDFName;
            //Create a new document
            iTextSharp.text.Document Doc = new iTextSharp.text.Document(PageSize.A4);
            // FileStream fs = new FileStream(pdfpath, FileMode.Create, FileAccess.Write, FileShare.None));
            PdfWriter writer = PdfWriter.GetInstance(Doc, new FileStream(FileName, FileMode.Create));

            //Open the PDF for writing
            Doc.Open();

            //give folder from your local system that contains images
            string[] Files = System.IO.Directory.GetFiles(ImagesPath);
            foreach (var file in Files)
            {

                var image = Image.GetInstance(file);
                image.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
                if (image.Height > image.Width)
                {
                    //Maximum height is 800 pixels.
                    float percentage = 0.0f;
                    percentage = 700 / image.Height;
                    image.ScalePercent(percentage * 100);
                }
                else
                {
                    //Maximum width is 600 pixels.
                    float percentage = 0.0f;
                    percentage = 540 / image.Width;
                    image.ScalePercent(percentage * 100);
                }
                //  image.SetAbsolutePosition((PageSize.A4.Width - image.ScaledWidth) / 2, (PageSize.A4.Height - image.ScaledHeight) / 2);
                Doc.Add(image);
            }


            //Close the PDF
            Doc.Close();


        }
    }
}