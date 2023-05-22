using AppsPortal.BaseControllers;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TBS_DAL.Model;
using AppsPortal.Extensions;
using AppsPortal.Library;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using AppsPortal.Areas.TBS.ADHelpers;

namespace AppsPortal.Areas.TBS.Controllers
{
    public class LandLineBillsController : TBSBaseController
    {
        //    6D7397D6-3D7F-48FC-BFD2-18E69673AC92   Damascus
        //    BF5D3BE8-DF6D-460A-86DC-5EB3A03F9E44   Homs
        //    9DC67413-952D-4E46-A13D-3F484BC40956   Sweida
        //    6D7397D6-3D7F-48FC-BFD2-18E69673AC94   Aleppo
        //    569F0F7F-4405-40E9-BEE8-F654FAC55EFA   Qamishli
        //    6CD6D68D-EAC1-440B-904F-7D34B4FD3863   Tartous


        private Guid _BillForTypeGUID_Mobile = Guid.Parse("9d8a1eb9-c2ac-4d78-95ff-874e46074321");
        private Guid _BillForTypeGUID_Land = Guid.Parse("b2980068-688d-428d-9e5c-494656ba9d2c");
        private Guid SyriaCountryGUID = Guid.Parse("710BFD1B-50CC-4F1E-92A9-A70583CFA5E0");

        public ActionResult CalculateLandLineBills()
        {
            return PartialView("~/Areas/TBS/Views/LandLineBills/Calculation/_StartCalculation.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult CalculateLandLineBillsCreate(ProcessLandLineBillsCalculation model)
        {
            DateTime ExecutionTime = DateTime.Now;
            Guid EntityPK = Guid.NewGuid();

            //get unproccessed files 
            List<dataCDRCopiedFile> unproccessedFiles = (from a in DbTBS.dataCDRCopiedFile
                                                         join b in DbTBS.codeCDRLocation on a.CDRLocationGUID equals b.CDRLocationGUID
                                                         where a.IsFileProcessed == false && b.DutyStationGUID == model.DutyStationGUID
                                                         select a).ToList();
            dataBill dataBill = new dataBill();
            dataBill.BillGUID = EntityPK;
            dataBill.DutyStationGUID = model.DutyStationGUID;
            dataBill.BillForYear = model.BillForYear;
            dataBill.BillForMonth = model.BillForMonth;
            dataBill.UploadDate = ExecutionTime;
            dataBill.BillForTypeGUID = _BillForTypeGUID_Land;
            dataBill.IsProcessed = false;
            dataBill.BillPeriodStartDate = DateTime.Now;
            dataBill.TelecomCompanyOperationConfigGUID = Guid.Parse("f7dc72fa-7202-4621-b1d6-d39fcccf1583");

            DbTBS.Create(dataBill, Permissions.BillsManagement.CreateGuid, ExecutionTime, DbCMS);

            if (unproccessedFiles.Count > 0)
            {
                //DbTBS.SaveChanges();
                //DbCMS.SaveChanges();
            }
            else
            {
                new Email().Send("karkoush@unhcr.org", "Landline Calculation Completed", "Dear Amer, there was no new files to process");
            }

            List<codeCallCost> codeCallCosts = (from a in DbTBS.codeCallCost.Where(x => x.Active) select a).ToList();

            List<dataUserBill> dataUserBills = new List<dataUserBill>();
            List<dataUserBillDetail> dataUserBillDetails = new List<dataUserBillDetail>();

            configTelecomCompanyOperationLandColumn LandlineConfiguration = DbTBS.configTelecomCompanyOperation
                .Where(x => x.Active && x.TelecomCompanyOperationConfigGUID == dataBill.TelecomCompanyOperationConfigGUID)
                .FirstOrDefault().configTelecomCompanyOperationLandColumn.Where(x => x.Active).FirstOrDefault();

            ADHelper aDHelper = new ADHelper(model.DutyStationGUID, DbTBS);

            foreach (var item in unproccessedFiles)
            {
                dataBillFile dataBillFile = new dataBillFile();
                dataBillFile.BillFileGUID = Guid.NewGuid();
                dataBillFile.BillGUID = dataBill.BillGUID;
                dataBillFile.FilePath = item.FilePath;

                DbTBS.Create(dataBillFile, Permissions.BillsManagement.CreateGuid, ExecutionTime, DbCMS);


                DataTable dt = CSVReaderHelper.GetCSVData(Server.MapPath(item.FilePath + ".csv"));

                var CallsList = (from a in dt.AsEnumerable()
                                 select new testtest
                                 {
                                     callingPartyNumber = a.Field<string>(LandlineConfiguration.CallingPartyNumberColumnIndex),
                                     originalCalledPartyNumber = a.Field<string>(LandlineConfiguration.OriginalCalledPartyNumberColumnIndex),
                                     dateTimeConnect = a.Field<string>(LandlineConfiguration.DateTimeConnectColumnIndex),
                                     dateTimeDisconnect = a.Field<string>(LandlineConfiguration.DateTimeDisconnectColumnIndex),
                                     originalCalledPartyNumberPartition = a.Field<string>(LandlineConfiguration.OriginalCalledPartyNumberPartitionColumnIndex),
                                     duration = a.Field<string>(LandlineConfiguration.DurationInSecondsColumnIndex.Value),
                                     authCodeDescription = a.Field<string>(LandlineConfiguration.AuthCodeDescriptionColumnIndex),
                                     finalCalledPartyNumber = a.Field<string>(LandlineConfiguration.FinalCalledPartyNumberColumnIndex),
                                     finalCalledPartyNumberPartition = a.Field<string>(LandlineConfiguration.FinalCalledPartyNumberPartitionColumnIndex),
                                 }).Where(x => x.finalCalledPartyNumberPartition != "Internal_PT" && x.finalCalledPartyNumberPartition != "VSAT_PT").Skip(1);

                foreach (var call in CallsList)
                {
                    if (string.IsNullOrWhiteSpace(call.finalCalledPartyNumber) || call.finalCalledPartyNumber.Length == 0)
                    {
                        continue;
                    }
                    DateTime callStartDate = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds((double)int.Parse(call.dateTimeConnect));
                    DateTime callEndDate = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds((double)int.Parse(call.dateTimeDisconnect));
                    string staffEmailAddress = null;
                    ////////If there is transfer
                    //if (call.originalCalledPartyNumber != call.finalCalledPartyNumber)
                    //{
                    //    //get the owner by originalCalledPartyNumber
                    //    staffEmailAddress = aDHelper.GetUserEmailByExtensionNumber(call.originalCalledPartyNumber);
                    //}
                    ////there is no transfer
                    //else
                    //{

                    //    staffEmailAddress = aDHelper.GetUserEmailByNameFromAD(call.authCodeDescription);
                    //}
                    if (call.UserID == "") { continue; }
                    if (call.UserID == " ") { continue; }
                    if (call.UserID.Length == 0) { continue; }
                    if (call.authCodeDescription.ToUpper() == "Invalid Authorization Code".ToUpper())
                    {
                        continue;
                    }
                    staffEmailAddress = aDHelper.GetUserEmailByNameFromAD(call.authCodeDescription);
                    if (staffEmailAddress == "") { continue; }
                    //staffEmailAddress = aDHelper.GetUserEmailByNameFromAD(call.UserID);


                    //get the owner of number
                    Guid _userGUID = (from a in DbTBS.StaffCoreData
                                      where a.EmailAddress.ToUpper() == staffEmailAddress.ToUpper()
                                      select a.UserGUID).FirstOrDefault();
                    if (_userGUID == null || _userGUID == Guid.Empty)
                    {
                        continue;
                    }


                    dataUserBill dataUserBillFound = dataUserBills.Where(x => x.UserGUID == _userGUID && x.BillGUID == dataBill.BillGUID).FirstOrDefault();

                    Guid UserBillGUID = Guid.NewGuid();
                    //if user is not found -> insert new record into dataUserBill
                    if (dataUserBillFound == null)
                    {
                        dataUserBills.Add(new dataUserBill
                        {
                            UserBillGUID = UserBillGUID,
                            BillGUID = dataBill.BillGUID,
                            IsConfirmed = false,
                            Active = true,
                            UserGUID = _userGUID,
                            DoPayInCash = false,
                            PayInCashAmount = 0,
                            DeductFromSalaryAmount = 0
                        });
                    }
                    else
                    {
                        UserBillGUID = dataUserBillFound.UserBillGUID;
                    }

                    //int CallDurationInSeconds = Convert.ToInt32(call.duration);
                    int totalSeconds = Convert.ToInt32(call.duration);
                    TimeSpan timeOfCall = TimeSpan.FromSeconds(totalSeconds);
                    DateTime d1 = new DateTime(1, 1, 1) + timeOfCall;
                    DateTime dd1 = RoundUp(d1, TimeSpan.FromMinutes(1));
                    int totalMinutes = dd1.Minute;

                    Guid BillForTypeGUID = _BillForTypeGUID_Land;

                    dataUserBillDetail dataUserBillDetail = new dataUserBillDetail();
                    dataUserBillDetail.UserBillDetailGUID = Guid.NewGuid();
                    dataUserBillDetail.CallingNumber = call.callingPartyNumber;
                    dataUserBillDetail.CalledNumber = call.finalCalledPartyNumber;

                    ////calculate cost////
                    //four Number Free cost
                    Match caseCost1 = Regex.Match(dataUserBillDetail.CalledNumber, @"\d{4}");
                    if (caseCost1.Success && dataUserBillDetail.CalledNumber.Length == 4)
                    {
                        dataUserBillDetail.CallCost = 0;
                        dataUserBillDetail.DestinationCountryGUID = SyriaCountryGUID;
                        dataUserBillDetail.CallType = MobileCallTypes.Local;
                    }

                    //ten number cost
                    //1- land local
                    Match caseCost2 = Regex.Match(dataUserBillDetail.CalledNumber, @"\d{7}");
                    if (caseCost2.Success && dataUserBillDetail.CalledNumber.Length == 7)
                    {
                        //double min = dataCall.CallDurationInMinutes / 3;
                        //int minutes = Convert.ToInt32(Math.Ceiling(min));
                        dataUserBillDetail.CallCost = totalMinutes * 0.33;
                        dataUserBillDetail.DestinationCountryGUID = SyriaCountryGUID;
                        dataUserBillDetail.CallType = MobileCallTypes.Local;
                        dataUserBillDetail.Service = MobileCallTypes.Local;
                    }
                    //2- land country
                    Match caseCost3 = Regex.Match(dataUserBillDetail.CalledNumber, @"0\d{9}");
                    if (caseCost3.Success && dataUserBillDetail.CalledNumber.Length == 10)
                    {
                        dataUserBillDetail.CallCost = totalMinutes * 3;
                        dataUserBillDetail.DestinationCountryGUID = SyriaCountryGUID;
                        dataUserBillDetail.CallType = MobileCallTypes.CountryWide;
                    }
                    //3- mobile
                    Match caseCost4 = Regex.Match(dataUserBillDetail.CalledNumber, @"09\d{8}");
                    if (caseCost4.Success && dataUserBillDetail.CalledNumber.Length == 10)
                    {
                        dataUserBillDetail.CallCost = totalMinutes * 14;
                        dataUserBillDetail.DestinationCountryGUID = SyriaCountryGUID;
                        dataUserBillDetail.CallType = MobileCallTypes.MobileLine;
                        //dc.CodeCountry = "SYR";
                    }
                    //twelve Number cost
                    Match caseCost5 = Regex.Match(dataUserBillDetail.CalledNumber, @"\d{12}");
                    if (caseCost5.Success && dataUserBillDetail.CalledNumber.Length >= 11)
                    {
                        string zipcode = dataUserBillDetail.CalledNumber.Substring(0, 3);
                        var country = (from a in codeCallCosts where a.DestinationCountryCode.ToString() == zipcode select a).FirstOrDefault();
                        dataUserBillDetail.CallCost = totalMinutes * country.CallCost;
                        dataUserBillDetail.DestinationCountryGUID = country.DestinationCountryGUID;
                        dataUserBillDetail.CallType = MobileCallTypes.International;
                    }
                    ////calculate cost////

                    dataUserBillDetail.dateTimeConnect = callStartDate;
                    dataUserBillDetail.dateTimeDisconnect = callEndDate;
                    dataUserBillDetail.DurationInMinutes = totalMinutes;
                    dataUserBillDetail.DurationInSeconds = totalSeconds;
                    dataUserBillDetail.UserBillGUID = UserBillGUID;
                    dataUserBillDetail.Active = true;
                    dataUserBillDetail.IsConfirmed = false;
                    if (call.authCodeDescription.EndsWith("_P"))
                    {
                        dataUserBillDetail.IsPrivate = true;
                    }
                    else
                    {
                        dataUserBillDetail.IsPrivate = false;
                    }

                    dataUserBillDetails.Add(dataUserBillDetail);
                }

                item.IsFileProcessed = true;

            }

            DbTBS.CreateBulk(dataUserBills, Permissions.BillsManagement.CreateGuid, ExecutionTime, DbCMS);
            DbTBS.CreateBulk(dataUserBillDetails, Permissions.BillsManagement.CreateGuid, ExecutionTime, DbCMS);

            DbTBS.SaveChanges();
            DbCMS.SaveChanges();

            new Email().Send("karkoush@unhcr.org", "Landline Calculation Completed", "Landline Calculation Completed");
            return Json(DbTBS.SingleCreateMessage());
        }
        public class CSVReaderHelper
        {
            public static DataTable GetCSVData(string localDestination)
            {
                //Instantiating Data Table
                var dt = new DataTable();

                try
                {
                    if (System.IO.File.Exists(localDestination))
                    {
                        using (StreamReader streamReader = new StreamReader(localDestination))
                        {
                            string[] headers = streamReader.ReadLine().Split(',');

                            foreach (string header in headers)
                            {
                                dt.Columns.Add(header);
                            }

                            while (!streamReader.EndOfStream)
                            {
                                string[] rows = streamReader.ReadLine().Split(',');

                                if (rows.Length > 1)
                                {
                                    DataRow dr = dt.NewRow();

                                    for (int i = 0; i < headers.Length; i++)
                                    {
                                        dr[i] = rows[i].Trim();
                                    }

                                    dt.Rows.Add(dr);
                                }

                            }
                        }
                    }

                    return dt;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public class testtest
        {
            public string callingPartyNumber { get; set; }
            public string originalCalledPartyNumber { get; set; }
            public string finalCalledPartyNumber { get; set; }
            public string dateTimeConnect { get; set; }
            public string dateTimeDisconnect { get; set; }
            public string originalCalledPartyNumberPartition { get; set; }
            public string finalCalledPartyNumberPartition { get; set; }
            public string duration { get; set; }
            public string authCodeDescription { get; set; }
            public string UserID { get; set; }
        }
        DateTime RoundUp(DateTime dt, TimeSpan d)
        {
            return new DateTime((dt.Ticks + d.Ticks - 1) / d.Ticks * d.Ticks, dt.Kind);
        }
    }
}