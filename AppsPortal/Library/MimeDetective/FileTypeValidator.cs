using FineUploader;
using MimeDetective;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace AppsPortal.Library.MimeDetective
{
    public static class FileTypeValidator
    {
        public static bool IsExcel(Stream upload)
        {
            var fileName = Path.GetTempFileName();
            try
            {
                using (var fileStream = System.IO.File.Create(fileName))
                {
                    upload.Seek(0, SeekOrigin.Begin);
                    upload.CopyTo(fileStream);
                }
                FileInfo f = new FileInfo(fileName);
                if (MimeTypes.IsType(f, MimeTypes.EXCEL) || MimeTypes.IsType(f, MimeTypes.EXCELX))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            finally
            {
                System.IO.File.Delete(fileName);
            }
        }
        public static bool IsWord(Stream upload)
        {
            var fileName = Path.GetTempFileName();
            try
            {
                using (var fileStream = System.IO.File.Create(fileName))
                {
                    upload.Seek(0, SeekOrigin.Begin);
                    upload.CopyTo(fileStream);
                }
                FileInfo f = new FileInfo(fileName);
                if (MimeTypes.IsType(f, MimeTypes.WORDX) || MimeTypes.IsType(f, MimeTypes.WORD))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            finally
            {
                System.IO.File.Delete(fileName);
            }
        }

        public static bool IsPDF(Stream upload)
        {
            var fileName = Path.GetTempFileName();
            try
            {
                using (var fileStream = System.IO.File.Create(fileName))
                {
                    upload.Seek(0, SeekOrigin.Begin);
                    upload.CopyTo(fileStream);
                }
                FileInfo f = new FileInfo(fileName);
                if (MimeTypes.IsType(f, MimeTypes.PDF))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            finally
            {
                System.IO.File.Delete(fileName);
            }
        }

        public static bool IsImage(Stream upload)
        {
            var fileName = Path.GetTempFileName();
            try
            {
                using (var fileStream = System.IO.File.Create(fileName))
                {
                    upload.Seek(0, SeekOrigin.Begin);
                    upload.CopyTo(fileStream);
                }
                FileInfo f = new FileInfo(fileName);
                if (MimeTypes.IsType(f, MimeTypes.JPEG)|| MimeTypes.IsType(f, MimeTypes.PNG)|| MimeTypes.IsType(f, MimeTypes.GIF) || MimeTypes.IsType(f, MimeTypes.BMP)|| MimeTypes.IsType(f, MimeTypes.ICO))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            finally
            {
                System.IO.File.Delete(fileName);
            }
        }

        public static bool IsText(Stream upload)
        {
            var fileName = Path.GetTempFileName();
            try
            {
                using (var fileStream = System.IO.File.Create(fileName))
                {
                    upload.Seek(0, SeekOrigin.Begin);
                    upload.CopyTo(fileStream);
                }
                FileInfo f = new FileInfo(fileName);
                if (MimeTypes.IsType(f, MimeTypes.TXT))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            finally
            {
                System.IO.File.Delete(fileName);
            }
        }
    }

}