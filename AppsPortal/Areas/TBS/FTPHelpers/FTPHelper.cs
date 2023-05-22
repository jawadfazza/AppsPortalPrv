using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace AppsPortal.Areas.TBS.FTPHelpers
{



    public class FTPHelper
    {
        private string _host = null;
        private int _port = 0;
        private string _user = null;
        private string _pass = null;
        private FtpWebRequest ftpRequest = null;
        private FtpWebResponse ftpResponse = null;
        private Stream ftpStream = null;
        private int bufferSize = 2048;

        public FTPHelper(string host, int port, string user, string pass)
        {
            this._host = host;
            this._port = port;
            this._user = user;
            this._pass = pass;
        }

        public string[] DirectoryListDetailed(string directory)
        {
            try
            {
                this.ftpRequest = (FtpWebRequest)WebRequest.Create(this._host + "/" + directory);
                this.ftpRequest.Credentials = new NetworkCredential(this._user, this._pass);
                this.ftpRequest.UseBinary = true;
                this.ftpRequest.UsePassive = true;
                this.ftpRequest.KeepAlive = false;
                this.ftpRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                this.ftpResponse = (FtpWebResponse)this.ftpRequest.GetResponse();
                this.ftpStream = this.ftpResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(this.ftpStream);
                string str = (string)null;
                try
                {
                    while (streamReader.Peek() != -1)
                        str = str + streamReader.ReadLine() + "|";
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                streamReader.Close();
                this.ftpStream.Close();
                this.ftpResponse.Close();
                this.ftpStream.Dispose();
                this.ftpResponse.Dispose();
                this.ftpRequest.Abort();
                this.ftpRequest = null;
                this.ftpResponse = null;
                try
                {
                    return str.Split("|".ToCharArray());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return new string[1] { "" };
        }

        public void DownloadFileThroughFTP(string RemoteFilePath, string LocalFilePath)
        {
            try
            {
                this.ftpRequest = (FtpWebRequest)WebRequest.Create(this._host + "/" + RemoteFilePath);
                this.ftpRequest.Credentials = new NetworkCredential(this._user, this._pass);
                this.ftpRequest.UseBinary = true;
                this.ftpRequest.UsePassive = true;
                this.ftpRequest.KeepAlive = false;
                ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                this.ftpResponse = (FtpWebResponse)this.ftpRequest.GetResponse();
                this.ftpStream = this.ftpResponse.GetResponseStream();
                FileStream fileStream = new FileStream(LocalFilePath, FileMode.Create);
                byte[] buffer = new byte[this.bufferSize];
                int count = this.ftpStream.Read(buffer, 0, this.bufferSize);
                try
                {
                    for (; count > 0; count = this.ftpStream.Read(buffer, 0, this.bufferSize))
                        fileStream.Write(buffer, 0, count);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                fileStream.Close();
                this.ftpStream.Close();
                this.ftpResponse.Close();
                this.ftpStream.Dispose();
                this.ftpResponse.Dispose();
                this.ftpRequest.Abort();
                this.ftpRequest = null;
                this.ftpResponse = null;
            }
            catch (Exception ex)
            {
                this.ftpStream.Close();
                this.ftpResponse.Close();
                this.ftpStream.Dispose();
                this.ftpResponse.Dispose();
                this.ftpRequest.Abort();
                this.ftpRequest = null;
                this.ftpResponse = null;
                Console.WriteLine(ex.ToString());
            }
        }
    }
}