using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace AppsPortal.Services
{
    public class Images
    {
        public void DeleteImage(string ImagesPath)
        {
                System.IO.File.Delete(ImagesPath);
            
        }

    
        public void EncryptionImage(HttpFileCollectionBase files, string Imagespath,byte[] Key)
        {
            Directory.CreateDirectory(Imagespath);
            RijndaelManaged rmCryp = new RijndaelManaged();
            //encryption image
            for (var i = 0; i < files.Count; i++)
            {
                HttpPostedFileBase file = files[i];
                if (file.ContentLength > 0 && file != null)
                {
                    string fileName = Path.GetFileName(file.FileName);
                    string ImagePath = Path.Combine(Imagespath, fileName);
                    byte[] fileBytes = new byte[file.ContentLength];
                    file.InputStream.Read(fileBytes, 0, file.ContentLength);

                    FileStream fs = new FileStream(ImagePath, FileMode.Create);
                    CryptoStream cs = new CryptoStream(fs, rmCryp.CreateEncryptor(Key, Key), CryptoStreamMode.Write);
                    foreach (var data in fileBytes)
                    {
                        cs.WriteByte((byte)data);
                    }
                    cs.Close();
                    fs.Close();
                    ////end encryption
                }
            }
        }
        public void DecryptionImages(string[] files, string pathDecryption, byte[] Key)
        {
            RijndaelManaged rmCryp = new RijndaelManaged();
            int data = 0;
            if (!Directory.Exists(pathDecryption)) { Directory.CreateDirectory(pathDecryption); }
            foreach (var file in files)
            {
                FileStream fs = new FileStream(file, FileMode.Open);
                string outputfile = Path.Combine(pathDecryption, Path.GetFileName(file));
                FileStream fsOutput = new FileStream(outputfile, FileMode.Create);
                rmCryp.Padding = PaddingMode.Zeros;
                CryptoStream cs = new CryptoStream(fs, rmCryp.CreateDecryptor(Key, Key), CryptoStreamMode.Read);
                try
                {
                    while ((data = cs.ReadByte()) != -1)
                    {
                        fsOutput.WriteByte((byte)data);
                    }
                    cs.Close();
                    fs.Close();
                    fsOutput.Close();
                }
                catch (Exception e)
                {
                
                }
                finally
                {
                    cs.Close();
                    fs.Close();
                    fsOutput.Close();
                }
            }

        }

        public void DecryptionWithResizeImages(string[] files, string pathDecryption, byte[] Key,double scaleVal)
        {
            RijndaelManaged rmCryp = new RijndaelManaged();
            if (!Directory.Exists(pathDecryption)) { Directory.CreateDirectory(pathDecryption); }
            int data = 0;
            foreach (var file in files)
            {
                FileStream fs = new FileStream(file, FileMode.Open);
                string outputfile = Path.Combine(pathDecryption, Path.GetFileName(file));
                FileStream fsOutput = new FileStream(outputfile, FileMode.Create);
                rmCryp.Padding = PaddingMode.Zeros;
                CryptoStream cs = new CryptoStream(fs, rmCryp.CreateDecryptor(Key, Key), CryptoStreamMode.Read);
                byte[] img = new byte[(int)fs.Length];
                cs.Read(img, 0, (int)fs.Length);
                byte[] imgResized = Resize2Max50Kbytes(img, scaleVal);
                try
                {
                    fsOutput.Write(imgResized, 0, imgResized.Length);
                    fsOutput.Close();
                    fs.Close();
                    cs.Close();
                }
                catch (Exception e)
                {

                }
                finally
                {
                    cs.Close();
                    fs.Close();
                    fsOutput.Close();
                }
            }

        }

        public void SaveImage(HttpFileCollectionBase files, string path)
        {
            Directory.CreateDirectory(path);
            for (var i = 0; i < files.Count; i++)
            {
                HttpPostedFileBase file = files[i];
                if (file.ContentLength > 0 && file != null)
                {
                    string fileName = Path.GetFileName(file.FileName);
                    string ImagePath = Path.Combine(path, fileName);
                    byte[] fileBytes = new byte[file.ContentLength];
                    file.SaveAs(ImagePath);

                }
            }

        }
        public void SaveImageWithResizeImages(HttpFileCollectionBase files, string path)
        {
            Directory.CreateDirectory(path);
            for (var i = 0; i < files.Count; i++)
            {
                HttpPostedFileBase file = files[i];
                if (file.ContentLength > 0 && file != null)
                {
                    string fileName = Path.GetFileName(file.FileName);
                    string ImagePath = Path.Combine(path, fileName);
                    byte[] fileBytes = new byte[file.ContentLength];
                    file.SaveAs(ImagePath);

                    FileStream fs = new FileStream(ImagePath, FileMode.Open);
                    byte[] img = new byte[(int)fs.Length];
                    fs.Read(img, 0, (int)fs.Length);
                    fs.Close();
                    // string outputfile = Path.Combine(ImagePath, Path.GetFileName(file));
                    FileStream fsOutput = new FileStream(ImagePath, FileMode.Create);

                    
                    byte[] imgResized = Resize2Max50Kbytes(img, 0.1f);
                    //fs.Write(fileBytes, 0, fileBytes.Length);
                    try
                    {
                        fsOutput.Write(imgResized, 0, imgResized.Length);
                        fsOutput.Close();
                        fs.Close();
                    }
                    catch (Exception e)
                    {

                    }
                }
            }

        }
        public static byte[] Resize2Max50Kbytes(byte[] byteImageIn, double scaleVal)
        {
            byte[] currentByteImageArray = byteImageIn;
            double scale = scaleVal;

            MemoryStream inputMemoryStream = new MemoryStream(byteImageIn);
            Image fullsizeImage = Image.FromStream(inputMemoryStream);

            while (currentByteImageArray.Length > 50000)
            {
                Bitmap fullSizeBitmap = new Bitmap(fullsizeImage, new Size((int)(fullsizeImage.Width * scale), (int)(fullsizeImage.Height * scale)));
                MemoryStream resultStream = new MemoryStream();

                fullSizeBitmap.Save(resultStream, fullsizeImage.RawFormat);

                currentByteImageArray = resultStream.ToArray();
                resultStream.Dispose();
                resultStream.Close();

                scale -= 0.05f;
            }

            return currentByteImageArray;
        }
    }
}