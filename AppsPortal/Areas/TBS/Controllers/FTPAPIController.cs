using AppsPortal.BaseControllers;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TBS_DAL.Model;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.Areas.TBS.FTPHelpers;
using System.Text.RegularExpressions;

namespace AppsPortal.Areas.TBS.Controllers
{
    public class FTPAPIController : TBSBaseController
    {
        public FTPAPIController()
        {

        }

        [Route("TBS/FTPAPI/ProcessNewCDRFiles")]
        public ActionResult ProcessNewCDRFiles()
        {
            return PartialView("~/Areas/TBS/Views/FTPAPI/_ProcessNewCDRFilesModal.cshtml");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ProcessNewCDRFilesCreate(ProcessNewCDRModel model)
        {
            DateTime executionTime = DateTime.Now;

            codeCDRLocation codeCDRLocation = (from a in DbTBS.codeCDRLocation.Where(x => x.Active && x.DutyStationGUID == model.DutyStationGUID)
                                               select a).FirstOrDefault();

            List<dataCDRCopiedFile> dataCDRCopiedFiles = (from a in DbTBS.dataCDRCopiedFile.Where(x => x.Active && x.CDRLocationGUID == codeCDRLocation.CDRLocationGUID)
                                                          select a).ToList();

            List<dataCDRCopiedFile> newDataCDRCopiedFiles = new List<dataCDRCopiedFile>();
            FTPHelper fTPHelper = new FTPHelper("ftp://" + codeCDRLocation.FTPPath, codeCDRLocation.FTPPort, codeCDRLocation.FTPUsr, codeCDRLocation.FTPPass);
            string[] strArray = fTPHelper.DirectoryListDetailed("/");

            string fileName = "";

            for (int i = 0; i < strArray.Length; i++)
            {
                Match match = Regex.Match(strArray[i], "cdr_StandAloneCluster");
                if (match.Success)
                {
                    fileName = strArray[i].Substring(match.Index);
                    if (!new FileInfo(Server.MapPath("~/Areas/TBS/Upload/" + codeCDRLocation.FTPUsr) + fileName + ".csv").Exists && dataCDRCopiedFiles.Where(a => a.FileName == fileName).FirstOrDefault() == null)
                    {
                        fTPHelper.DownloadFileThroughFTP("/" + fileName, Server.MapPath("~/Areas/TBS/Uploads/" + codeCDRLocation.FTPUsr + "/") + fileName + ".csv");
                        newDataCDRCopiedFiles.Add(new dataCDRCopiedFile
                        {
                            CDRFileGUID = Guid.NewGuid(),
                            CDRLocationGUID = codeCDRLocation.CDRLocationGUID,
                            FileName = fileName,
                            FileDownloadedOn = executionTime,
                            FileDownloadedBy = UserGUID,
                            Active = true,
                            IsFileProcessed = false,
                            FilePath = "~/Areas/TBS/Uploads/" + codeCDRLocation.FTPUsr.ToUpper() + "/" + fileName
                        });
                    }
                }
                if (i == 30) break;
            }
            try
            {
                DbTBS.dataCDRCopiedFile.AddRange(newDataCDRCopiedFiles);
                DbTBS.SaveChanges();
            }
            catch (Exception ex) { }

            new Email().Send("karkoush@unhcr.org", model.EmailToCC, null, "CDR Files Copy Completed", "CDR Files Copy Completed", "HTML", "Normal", null);

            return Json(DbTBS.SuccessMessage("CDR Files Copy Completed"));

        }

        [Route("TBS/FTPAPI/GetCDRFTPInfo")]
        public JsonResult GetCDRFTPInfo(Guid DutyStationGUID)
        {
            var result = (from a in DbTBS.codeCDRLocation.Where(x => x.Active && x.DutyStationGUID == DutyStationGUID)
                          select new
                          {
                              a.CDRLocationGUID,
                              a.FTPPath,
                              a.FTPUsr
                          }).FirstOrDefault();
            return Json(new
            {
                CDRLocationGUID = result.CDRLocationGUID,
                FTPPath = result.FTPPath,
                FTPUsr = result.FTPUsr
            }, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult TestFTPS()
        //{
        //    string host = @"10.240.231.142:9876";
        //    string username = "testsftp";
        //    string password = @"123";
        //    using (SftpClient sftp = new SftpClient("cdr.unhcrsyria.org", 9876, username, password))
        //    {
        //        try
        //        {
        //            sftp.Connect();
        //            var files = sftp.ListDirectory("/");

        //            foreach (var file in files)
        //            {
        //                if (file.Name == "test.txt")
        //                {
        //                    string pathLocalFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "download_sftp_file.txt");
        //                    using (Stream fileStream = System.IO.File.OpenWrite("C:\\SSH\\download_sftp_file.txt"))
        //                    {
        //                        sftp.DownloadFile("/test.txt", fileStream);
        //                    }

        //                }
        //                Console.WriteLine(file.Name);
        //            }

        //            sftp.Disconnect();
        //        }
        //        catch (Exception e)
        //        {
        //            Console.WriteLine("An exception has been caught " + e.ToString());
        //        }
        //    }
        //    return null;
        //}


        #region  OLD CODE
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ProcessNewCDRFilesCreateXXX(ProcessNewCDRModel model)
        {
            codeCDRLocation codeCDRLocation = (from a in DbTBS.codeCDRLocation.Where(x => x.Active && x.DutyStationGUID == model.DutyStationGUID)
                                               select a).FirstOrDefault();

            List<dataCDRCopiedFile> dataCDRCopiedFiles = (from a in DbTBS.dataCDRCopiedFile.Where(x => x.Active && x.CDRLocationGUID == codeCDRLocation.CDRLocationGUID)
                                                          select a).ToList();

            DownloadFilesThroughFTP("ftp://" + codeCDRLocation.FTPPath, codeCDRLocation.FTPUsr, codeCDRLocation.FTPPass, codeCDRLocation, dataCDRCopiedFiles);


            return null;
        }

        public void DownloadFilesThroughFTP(string host, string usr, string pass, codeCDRLocation codeCDRLocation, List<dataCDRCopiedFile> severDataCDRCopiedFiles)
        {
            DateTime executionDate = DateTime.Now;
            List<dataCDRCopiedFile> ftpDataCDRCopiedFiles = new List<dataCDRCopiedFile>();
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(host);
            request.Credentials = new NetworkCredential(usr, pass);
            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = true;



            request.Method = WebRequestMethods.Ftp.ListDirectory; // "RETR";

            FtpWebResponse response = request.GetResponse() as FtpWebResponse;
            Stream responseStream = response.GetResponseStream();


            //... FTP commands
            List<string> myServerFiles = new List<string>();
            List<string> ftpServerFiles = new List<string>();
            StreamReader reader = new StreamReader(responseStream);

            while (!reader.EndOfStream)
            {
                string _fileName = reader.ReadLine();
                ftpDataCDRCopiedFiles.Add(new dataCDRCopiedFile
                {
                    CDRFileGUID = Guid.NewGuid(),
                    CDRLocationGUID = codeCDRLocation.CDRLocationGUID,
                    FileName = _fileName,
                    FileDownloadedOn = executionDate,
                    FileDownloadedBy = UserGUID,
                    Active = true,
                    IsFileProcessed = false,
                    FilePath = "~/Uploads/" + codeCDRLocation.FTPUsr.ToUpper() + "/" + _fileName
                });
            }

            myServerFiles = severDataCDRCopiedFiles.Select(x => x.FileName).ToList();
            //ftpServerFiles = ftpDataCDRCopiedFiles.Select(x => x.FileName).ToList();

            List<dataCDRCopiedFile> newDataCDRCopiedFile = new List<dataCDRCopiedFile>();

            newDataCDRCopiedFile = (from a in ftpDataCDRCopiedFiles
                                    where !myServerFiles.Contains(a.FileName)
                                    select a).ToList();
            reader.Close();

            DbTBS.dataCDRCopiedFile.AddRange(newDataCDRCopiedFile);

            //DbTBS.CreateBulk(newDataCDRCopiedFile, Permissions.CDRFTPLocationsManagement.CreateGuid, DateTime.Now, DbCMS);

            //ftp.download("/" + fileName, "D:\\WebData\\CSV\\" + fileName + ".csv");



            foreach (var file in newDataCDRCopiedFile)
            {
                request = FtpWebRequest.Create(host + "/" + file.FileName) as FtpWebRequest;

                //FileStream fileStream = new FileStream(Server.MapPath(file.FilePath + ".csv"), FileMode.Create);
                FileStream fileStream = new FileStream("C:\\TFS\\AppsPortal\\AppsPortal\\Areas\\TBS\\Uploads\\SYRDA\\" + file.FileName + ".csv", FileMode.Create);
                //
                byte[] buffer = new byte[2048];
                int count = responseStream.Read(buffer, 0, 2048);
                try
                {
                    for (; count > 0; count = responseStream.Read(buffer, 0, 2048))
                        fileStream.Write(buffer, 0, count);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }









            DbTBS.SaveChanges();

            responseStream.Close();
            response.Close(); //Closes the connection to the server
            request = (FtpWebRequest)null;
            //FtpWebResponse ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
            //FtpWebResponse response = ftpRequest.GetResponse() as FtpWebResponse;
            //Stream responseStream = ftpResponse.GetResponseStream();
            //StreamReader reader = new StreamReader(responseStream);

            //Stream ftpStream = ftpResponse.GetResponseStream();
            //FileStream fileStream = new FileStream("localFile", FileMode.Create);

        }
        #endregion

    }
}