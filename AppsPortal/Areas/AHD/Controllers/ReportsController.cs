using AppsPortal.Areas.AHD.RDLC;
using AppsPortal.Areas.AHD.RDLC.AHDDataSetTableAdapters;
using AppsPortal.Areas.WMS.RDLC.DataSet1TableAdapters;
using AppsPortal.BaseControllers;
using AppsPortal.Library;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using FolkerKinzel.Contacts;
using FolkerKinzel.Contacts.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Reporting.WebForms;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Name = FolkerKinzel.Contacts.Name;

namespace AppsPortal.Areas.AHD.Controllers
{
    public class ReportsController : AHDBaseController
    {
        // GET: AHD/Reports
        public ActionResult Index()
        {
            return View();
        }
        #region Word Documents Renewal Forms 

        public ActionResult PrintNVsForms(Guid PK)
        {
            var _form = DbAHD.dataStaffRenwalResidency.Where(x => x.StaffRenwalResidencyGUID == PK).FirstOrDefault();
            if (_form.FormTypeGUID == Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c4e7999"))
            {
                return RedirectToAction("PrintStaffRenewalForm", "Reports", new { PK = PK });
                //PrintStaffRenewalForm(PK);
            }
            else if (_form.FormTypeGUID == Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c4e7992"))
            {
                return RedirectToAction("PrintStaffReturnForm", "Reports", new { PK = PK });
                //PrintStaffReturnlForm(PK);
            }
            else if (_form.FormTypeGUID == Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c4e7993"))
            {

                return RedirectToAction("PrintStaffStartWorkForm", "Reports", new { PK = PK });
                //PrintStaffReturnlForm(PK);
            }
            else if (_form.FormTypeGUID == Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c4e7933"))
            {

                return RedirectToAction("PrintEndOfWorkNotificationForm", "Reports", new { PK = PK });
                //PrintStaffReturnlForm(PK);
            }
            return View("~/Areas/AHD/Views/StaffRenewalResidency/Index.cshtml", null, null);
        }

        public ActionResult PrintEndOfWorkNotificationForm(Guid PK)
        {
            var _renewal = DbAHD.dataStaffRenwalResidency.Where(x => x.StaffRenwalResidencyGUID == PK).FirstOrDefault();
            // var notVerbaleStaff_AR = DbMRS.RP_NoteVerbaleStaff(PK, "AR").ToList();
            //  var notVerbaleVehicle_AR = DbMRS.RP_NoteVerbaleVehicle(PK, "AR").FirstOrDefault();

            string templatePath = "~/Areas/AHD/Templates/StaffResidencyRenewal/EndofWorkNotification.docx";
            var serverPath = Server.MapPath(templatePath);

            string destinationPath = "~/Areas/AHD/Temp/" + PK + ".docx";
            var destinationServerPath = Server.MapPath(destinationPath);

            System.IO.File.Copy(serverPath, destinationServerPath, true);

            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(destinationServerPath, true))
            {
                MainDocumentPart mainDocumentPart = wordDoc.MainDocumentPart;
                var SDTRun = mainDocumentPart.Document.Descendants<SdtRun>();
                var _staffperslan = DbAHD.userPersonalDetailsLanguage.Where(x => x.Active && x.UserGUID == _renewal.StaffGUID).ToList();
                #region Lables
                string _artasks = "";
                string _entasks = "";
                string _arreturn = "";
                string _arhiscountry = "";
                string _argrnated = "";
                string _engrnated = "";
                string _ArHeIs = "";
                string _ArCarry = "";
                string _enheShe = "";
                string _arstaffMr = "";
                string _enstaffMr = "";
                string _arassigned = "";
                string _enassigned = "";
                string _arEntered = "";
                string _thePersonar = "";
                string _archarge = "";
                string _arendDuty = "";

                if (_renewal.Gender == "Male")
                {
                    _artasks = "مهامه";
                    _arEntered = "دخل";
                    _arendDuty = "أنهى مهامه وغادر";
                    _arassigned = "تعيينه";
                    _entasks = "his";
                    _archarge = "مسؤول";
                    _arreturn = "عاد";
                    _arhiscountry = "عقده";
                    _argrnated = "السيد" + " " + _staffperslan.Where(x => x.LanguageID == "AR").FirstOrDefault().Surname;
                    _engrnated = "Mr" + ". " + _staffperslan.Where(x => x.LanguageID == "EN").FirstOrDefault().Surname;

                    _arstaffMr = "السيد" + " " + _staffperslan.Where(x => x.LanguageID == "AR").FirstOrDefault().FirstName + " " + _staffperslan.Where(x => x.LanguageID == "AR").FirstOrDefault().Surname;
                    _enstaffMr = "Mr" + ". " + _staffperslan.Where(x => x.LanguageID == "EN").FirstOrDefault().FirstName + " " + _staffperslan.Where(x => x.LanguageID == "EN").FirstOrDefault().Surname; ;
                    _ArHeIs = "أنه";
                    _ArCarry = " يحمل";
                    _enheShe = "he";
                    _thePersonar = "الذي";
                }
                else
                {
                    _archarge = "مسؤولة";
                    _thePersonar = "التي";
                    _arendDuty = "أنهت مهامها وغادرت";
                    _artasks = "مهامها";
                    _arassigned = "تعيينها";
                    _arEntered = "دخلت";
                    _entasks = "her";
                    _arreturn = "عادت";
                    _arhiscountry = "عقدها";
                    _argrnated = "السيدة" + " " + _staffperslan.Where(x => x.LanguageID == "AR").FirstOrDefault().Surname;
                    _engrnated = "Ms" + ". " + _staffperslan.Where(x => x.LanguageID == "EN").FirstOrDefault().Surname;

                    _arstaffMr = "السيدة" + " " + _staffperslan.Where(x => x.LanguageID == "AR").FirstOrDefault().FirstName + " " + _staffperslan.Where(x => x.LanguageID == "AR").FirstOrDefault().Surname;
                    _enstaffMr = "Ms" + ". " + _staffperslan.Where(x => x.LanguageID == "EN").FirstOrDefault().FirstName + " " + _staffperslan.Where(x => x.LanguageID == "EN").FirstOrDefault().Surname; ;
                    _ArHeIs = "أنها";
                    _ArCarry = " تحمل";
                    _enheShe = "she";
                }



                #endregion

                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("RefNumber"))
             .First<SdtRun>().Descendants<Text>().First().Text = _renewal.RefNumber;


                Guid unlpGUID = Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E3732");
                var _unlp = DbAHD.dataStaffCoreDocument.Where(x => x.UserGUID == _renewal.StaffGUID && x.DocumentTypeGUID == unlpGUID
                 && x.Active).FirstOrDefault();


                #region Arabic 
                //Arabic Renewal
                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("ArStaffName"))
                    .First<SdtRun>().Descendants<Text>().First().Text = _arstaffMr;

                //SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_archarge"))
                //  .First<SdtRun>().Descendants<Text>().First().Text = _archarge;

                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("arDept"))
            .First<SdtRun>().Descendants<Text>().First().Text = _renewal.ArJobTitle;

                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_dutyStation"))
              .First<SdtRun>().Descendants<Text>().First().Text = _renewal.ArOffice;


                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_arendDuty"))
            .First<SdtRun>().Descendants<Text>().First().Text = _arendDuty;








                if (_renewal.DepartureDateFromSyria != null)
                {



                    string _startcontractDay = _renewal.DepartureDateFromSyria.Value.ToString("dd", new CultureInfo("ar-SY")).ToString();
                    string _startcontractMonth = _renewal.DepartureDateFromSyria.Value.ToString("MMMM", new CultureInfo("ar-SY")).ToString();
                    string _startcontractYear = _renewal.DepartureDateFromSyria.Value.ToString("yyyy", new CultureInfo("ar-SY")).ToString();

                    //string formated = string.Format("{0}{1}{2}", _startcontractDay, _startcontractMonth, _startcontractYear);




                    SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("dayEndDate"))
                 .First<SdtRun>().Descendants<Text>().First().Text = _startcontractDay;
                    SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("monthEndDate"))
               .First<SdtRun>().Descendants<Text>().First().Text = _startcontractMonth;
                    SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("yearEndDate"))
               .First<SdtRun>().Descendants<Text>().First().Text = _startcontractYear;


                }
                if (_renewal.SentDate != null)
                {
                    string _startcontractDay = _renewal.SentDate.Value.ToString("dd").ToString();
                    string _startcontractMonth = _renewal.SentDate.Value.ToString("MM").ToString();
                    string _startcontractYear = _renewal.SentDate.Value.ToString("yyyy").ToString();

                    //string formated = string.Format("{0}{1}{2}", _startcontractDay, _startcontractMonth, _startcontractYear);






                    var lefttoright = ((Char)0x200E).ToString();
                    var result = _startcontractYear + "/" + _startcontractMonth + lefttoright + "/" + _startcontractDay;

                    SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_enSentDate"))
                        .First<SdtRun>().Descendants<Text>().First().Text = result;


                }




                #endregion













                mainDocumentPart.Document.Save();


            }

            byte[] filedata = System.IO.File.ReadAllBytes(destinationServerPath);
            string contentType = MimeMapping.GetMimeMapping(destinationServerPath);

            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = _renewal.RefNumber + "-" + _renewal.SentDate.Value.ToString("dd MMMM yyyy") + ".docx",
                Inline = true,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());

            return File(filedata, contentType);
        }
        public ActionResult PrintStaffRenewalForm(Guid PK)
        {
            var _renewal = DbAHD.dataStaffRenwalResidency.Where(x => x.StaffRenwalResidencyGUID == PK).FirstOrDefault();
            // var notVerbaleStaff_AR = DbMRS.RP_NoteVerbaleStaff(PK, "AR").ToList();
            //  var notVerbaleVehicle_AR = DbMRS.RP_NoteVerbaleVehicle(PK, "AR").FirstOrDefault();

            string templatePath = "~/Areas/AHD/Templates/StaffResidencyRenewal/StaffRenewal.docx";
            var serverPath = Server.MapPath(templatePath);

            string destinationPath = "~/Areas/AHD/Temp/" + PK + ".docx";
            var destinationServerPath = Server.MapPath(destinationPath);

            System.IO.File.Copy(serverPath, destinationServerPath, true);

            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(destinationServerPath, true))
            {
                MainDocumentPart mainDocumentPart = wordDoc.MainDocumentPart;
                var SDTRun = mainDocumentPart.Document.Descendants<SdtRun>();
                var _staffperslan = DbAHD.userPersonalDetailsLanguage.Where(x => x.Active && x.UserGUID == _renewal.StaffGUID).ToList();
                #region Lables
                string _artasks = "";
                string _entasks = "";
                string _arreturn = "";
                string _arhiscountry = "";
                string _argrnated = "";
                string _engrnated = "";
                string _ArHeIs = "";
                string _ArCarry = "";
                string _enheShe = "";
                string arWork = "";
                string _Artravel = "";
                string _archarge = "";
                string _arResendcy = "";
                string _engStaffName = "";
                string _herHis = "";
                string _arworkslike = "";
                string _arcarryof = "";
                if (_renewal.Gender == "Male")
                {
                    arWork = "للسيد";
                    _artasks = "مهامه";
                    _Artravel = "لسفره" + " ";
                    _archarge = "مسؤول";
                    _entasks = "his";
                    _arreturn = "عاد";
                    _arhiscountry = "عقده";
                    _argrnated = "السيد" + " " + _staffperslan.Where(x => x.LanguageID == "AR").FirstOrDefault().Surname;
                    _engrnated = "Mr" + ". " + _staffperslan.Where(x => x.LanguageID == "EN").FirstOrDefault().Surname;
                    _engStaffName = "Mr" + ". " + _renewal.EnStaffName;
                    _ArHeIs = "أنه";
                    _ArCarry = "هو";
                    _enheShe = "he";
                    _arResendcy = "إقامته";
                    _herHis = "his";
                    _arworkslike = "يعمل";
                    _arcarryof = "يحمل";
                }
                else
                {
                    arWork = "للسيدة";
                    _artasks = "مهامها";
                    _Artravel = "لسفرها" + " ";
                    _archarge = "مسؤولة";
                    _entasks = "her";
                    _arreturn = "عادت";
                    _arhiscountry = "عقدها";
                    _argrnated = "السيدة" + " " + _staffperslan.Where(x => x.LanguageID == "AR").FirstOrDefault().Surname;
                    _engrnated = "Ms " + ". " + _staffperslan.Where(x => x.LanguageID == "EN").FirstOrDefault().Surname;
                    _engStaffName = "Ms" + ". " + _renewal.EnStaffName;
                    _ArHeIs = "أنها";
                    _ArCarry = "هي";
                    _enheShe = "she";
                    _arResendcy = "إقامتها";
                    _herHis = "her";
                    _arworkslike = "تعمل";
                    _arcarryof = "تحمل";
                }



                #endregion

                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("RefNumber"))
              .First<SdtRun>().Descendants<Text>().First().Text = _renewal.RefNumber;

                #region Arabic 
                //Arabic 
                var test = SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("ArWork"));

                // SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("ArWork"))
                //.First<SdtRun>().Descendants<Text>().First().Text = arWork;

                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("ArStaffName"))
                    .First<SdtRun>().Descendants<Text>().First().Text = arWork + " " + _renewal.ArStaffName;

                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("Artravel"))
               .First<SdtRun>().Descendants<Text>().First().Text = _Artravel;

                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("ArNationality"))
              .First<SdtRun>().Descendants<Text>().First().Text = _renewal.ArNationality;

                //Guid unlpGUID = Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E3732");
                //var _unlp = DbAHD.dataStaffCoreDocument.Where(x => x.UserGUID == _renewal.StaffGUID && x.DocumentTypeGUID == unlpGUID
                // && x.Active).FirstOrDefault();

                //StaffRenewalResidency.PassportNumber = _unlp.DocumentNumber;
                //StaffRenewalResidency.DocumentDateOfExpiry = _unlp.DocumentDateOfExpiry;


                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("ArPassportNumber"))
               .First<SdtRun>().Descendants<Text>().First().Text = _renewal.PassportNumber;

                if (_renewal.PassportValidityDate != null)
                {




                    string _startcontractDay = _renewal.PassportValidityDate.Value.ToString("dd", new CultureInfo("ar-SY")).ToString();
                    string _startcontractMonth = _renewal.PassportValidityDate.Value.ToString("MMMM", new CultureInfo("ar-SY")).ToString();
                    string _startcontractYear = _renewal.PassportValidityDate.Value.ToString("yyyy", new CultureInfo("ar-SY")).ToString();

                    //string formated = string.Format("{0}{1}{2}", _startcontractDay, _startcontractMonth, _startcontractYear);




                    SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("dayArPassportValid"))
                 .First<SdtRun>().Descendants<Text>().First().Text = _startcontractDay;
                    SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("monthArPassportValid"))
               .First<SdtRun>().Descendants<Text>().First().Text = _startcontractMonth;
                    SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("yearArPassportValid"))
               .First<SdtRun>().Descendants<Text>().First().Text = _startcontractYear;
                }

                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("Arworkslike"))
                .First<SdtRun>().Descendants<Text>().First().Text = _arworkslike;

                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_arCarryAR"))
             .First<SdtRun>().Descendants<Text>().First().Text = _arcarryof;

                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("ArJobName"))
        .First<SdtRun>().Descendants<Text>().First().Text = _renewal.ArJobTitle;


                //SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("ArDeptName"))
                //  .First<SdtRun>().Descendants<Text>().First().Text = _renewal.ArDeptName;

                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("ArOffice"))
                .First<SdtRun>().Descendants<Text>().First().Text = _renewal.ArOffice;

                string leftToRight = ((char)0x200E).ToString();

                if (_renewal.ContractStartDate != null)
                {
                    string _startcontractDay = _renewal.ContractStartDate.Value.ToString("dd", new CultureInfo("ar-SY")).ToString();
                    string _startcontractMonth = _renewal.ContractStartDate.Value.ToString("MMMM", new CultureInfo("ar-SY")).ToString();
                    string _startcontractYear = _renewal.ContractStartDate.Value.ToString("yyyy", new CultureInfo("ar-SY")).ToString();

                    //string formated = string.Format("{0}{1}{2}", _startcontractDay, _startcontractMonth, _startcontractYear);

                    var lefttoright = ((Char)0x200E).ToString();
                    var result = _startcontractYear + " " + _startcontractMonth + lefttoright + " " + _startcontractDay;




                    SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_arcontractstartdate"))
                        .First<SdtRun>().Descendants<Text>().First().Text = result;

                }

                if (_renewal.ContractEndDate != null)
                {
                    string _startcontractDay = _renewal.ContractEndDate.Value.ToString("dd", new CultureInfo("ar-SY")).ToString();
                    string _startcontractMonth = _renewal.ContractEndDate.Value.ToString("MMMM", new CultureInfo("ar-SY")).ToString();
                    string _startcontractYear = _renewal.ContractEndDate.Value.ToString("yyyy", new CultureInfo("ar-SY")).ToString();

                    //string formated = string.Format("{0}{1}{2}", _startcontractDay, _startcontractMonth, _startcontractYear);


                    var lefttoright = ((Char)0x200E).ToString();
                    var result = _startcontractYear + " " + _startcontractMonth + lefttoright + " " + _startcontractDay;

                    SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_arcontractenddate"))
                        .First<SdtRun>().Descendants<Text>().First().Text = result;
                }

                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_arResendcy"))
                 .First<SdtRun>().Descendants<Text>().First().Text = _arResendcy;

                if (_renewal.CurrentResidencyEndDateSent != null)
                {
                    string _startcontractDay = _renewal.CurrentResidencyEndDateSent.Value.ToString("dd", new CultureInfo("ar-SY")).ToString();
                    string _startcontractMonth = _renewal.CurrentResidencyEndDateSent.Value.ToString("MMMM", new CultureInfo("ar-SY")).ToString();
                    string _startcontractYear = _renewal.CurrentResidencyEndDateSent.Value.ToString("yyyy", new CultureInfo("ar-SY")).ToString();

                    //string formated = string.Format("{0}{1}{2}", _startcontractDay, _startcontractMonth, _startcontractYear);


                    var lefttoright = ((Char)0x200E).ToString();
                    var result = _startcontractYear + " " + _startcontractMonth + lefttoright + " " + _startcontractDay;

                    SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_ARResedencyEndDate"))
                        .First<SdtRun>().Descendants<Text>().First().Text = result;
                }

                //if (_renewal.SentDate != null)
                //{
                //    SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_arsentdate"))
                //      .First<SdtRun>().Descendants<Text>().First().Text = _renewal.SentDate.Value.ToString("dd/MM/yyyy", new CultureInfo("ar-SY"));
                //}






                #endregion


                #region English
                //English Renewal 

                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_enstaffname"))
                   .First<SdtRun>().Descendants<Text>().First().Text = _engStaffName;

                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("enHerBack"))
               .First<SdtRun>().Descendants<Text>().First().Text = _entasks;



                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("EnNationality"))
              .First<SdtRun>().Descendants<Text>().First().Text = _renewal.EnNationality;
                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("EnPassportNumber"))
              .First<SdtRun>().Descendants<Text>().First().Text = _renewal.PassportNumber;

                if (_renewal.PassportValidityDate != null)
                {
                    SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("EnPassportValid"))
                 .First<SdtRun>().Descendants<Text>().First().Text = _renewal.PassportValidityDate.Value.ToString("dd MMMM yyyy");
                }
                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_enJob"))
                .First<SdtRun>().Descendants<Text>().First().Text = _renewal.EnJobTitle;
                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_enoffice"))
               .First<SdtRun>().Descendants<Text>().First().Text = _renewal.EnOffice;
                if (_renewal.ContractStartDate != null)
                {
                    SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_enStartContract"))
                  .First<SdtRun>().Descendants<Text>().First().Text = _renewal.ContractStartDate.Value.ToString("dd MMMM yyyy");
                }

                if (_renewal.ContractEndDate != null)
                {
                    SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_enEndContract"))
                   .First<SdtRun>().Descendants<Text>().First().Text = _renewal.ContractEndDate.Value.ToString("dd MMMM yyyy");
                }

                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_enHisHerStaff"))
                     .First<SdtRun>().Descendants<Text>().First().Text = _herHis;


                if (_renewal.CurrentResidencyEndDateSent != null)
                {
                    SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_enVisaExpiryDate"))
                        .First<SdtRun>().Descendants<Text>().First().Text = _renewal.CurrentResidencyEndDateSent.Value.ToString("dd MMMM yyyy");
                }



                if (_renewal.SentDate != null)
                {
                    SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_enSentDate"))
                   .First<SdtRun>().Descendants<Text>().First().Text = _renewal.SentDate.Value.ToString("dd/MM/yyyy");
                }

                #endregion











                mainDocumentPart.Document.Save();


            }

            byte[] filedata = System.IO.File.ReadAllBytes(destinationServerPath);
            string contentType = MimeMapping.GetMimeMapping(destinationServerPath);

            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = _renewal.RefNumber + "-" + _renewal.SentDate.Value.ToString("dd MMMM yyyy") + ".docx",
                Inline = true,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());

            return File(filedata, contentType);
        }


        public ActionResult PrintStaffReturnForm(Guid PK)
        {
            var _renewal = DbAHD.dataStaffRenwalResidency.Where(x => x.StaffRenwalResidencyGUID == PK).FirstOrDefault();
            // var notVerbaleStaff_AR = DbMRS.RP_NoteVerbaleStaff(PK, "AR").ToList();
            //  var notVerbaleVehicle_AR = DbMRS.RP_NoteVerbaleVehicle(PK, "AR").FirstOrDefault();

            string templatePath = "~/Areas/AHD/Templates/StaffResidencyRenewal/StaffReturnToSyria.docx";
            var serverPath = Server.MapPath(templatePath);

            string destinationPath = "~/Areas/AHD/Temp/" + PK + ".docx";
            var destinationServerPath = Server.MapPath(destinationPath);

            System.IO.File.Copy(serverPath, destinationServerPath, true);

            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(destinationServerPath, true))
            {
                MainDocumentPart mainDocumentPart = wordDoc.MainDocumentPart;
                var SDTRun = mainDocumentPart.Document.Descendants<SdtRun>();
                var _staffperslan = DbAHD.userPersonalDetailsLanguage.Where(x => x.Active && x.UserGUID == _renewal.StaffGUID).ToList();
                #region Lables
                string _artasks = "";
                string _entasks = "";
                string _arreturn = "";
                string _arhiscountry = "";
                string _argrnated = "";
                string _engrnated = "";
                string _ArHeIs = "";
                string _ArCarry = "";
                string _enheShe = "";

                if (_renewal.Gender == "Male")
                {
                    _artasks = "مهامه";
                    _entasks = "his";
                    _arreturn = "عاد";
                    _arhiscountry = "عقده";
                    _argrnated = "السيد" + " " + _staffperslan.Where(x => x.LanguageID == "AR").FirstOrDefault().Surname;
                    _engrnated = "Mr" + ". " + _staffperslan.Where(x => x.LanguageID == "EN").FirstOrDefault().Surname;
                    _ArHeIs = "أنه";
                    _ArCarry = "يحمل";
                    _enheShe = "he";
                }
                else
                {
                    _artasks = "مهامها";
                    _entasks = "her";
                    _arreturn = "عادت";
                    _arhiscountry = "عقدها";
                    _argrnated = "السيدة" + " " + _staffperslan.Where(x => x.LanguageID == "AR").FirstOrDefault().Surname;
                    _engrnated = "Ms" + ". " + _staffperslan.Where(x => x.LanguageID == "EN").FirstOrDefault().Surname;
                    _ArHeIs = "أنها";
                    _ArCarry = "تحمل";
                    _enheShe = "she";
                }



                #endregion
                Guid unlpGUID = Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E3732");
                var _unlp = DbAHD.dataStaffCoreDocument.Where(x => x.UserGUID == _renewal.StaffGUID && x.DocumentTypeGUID == unlpGUID
                 && x.Active).FirstOrDefault();
                #region Arabic 
                //Arabic Renewal
                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("ArStaffName"))
                    .First<SdtRun>().Descendants<Text>().First().Text = _renewal.ArStaffName;
                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_arOffice"))
                   .First<SdtRun>().Descendants<Text>().First().Text = _renewal.ArOffice;
                //if (_renewal.ReturnDate != null)
                //{
                //    string _startcontractDay = _renewal.ReturnDate.Value.ToString("dd", new CultureInfo("ar-SY")).ToString();
                //    string _startcontractMonth = _renewal.ReturnDate.Value.ToString("MMMM", new CultureInfo("ar-SY")).ToString();
                //    string _startcontractYear = _renewal.ReturnDate.Value.ToString("yyyy", new CultureInfo("ar-SY")).ToString();
                //    var lefttoright = ((Char)0x200E).ToString();
                //    var result = _startcontractYear + " " + _startcontractMonth + lefttoright + " " + _startcontractDay;

                //    SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("ArReturnDate"))
                //        .First<SdtRun>().Descendants<Text>().First().Text = result;
                //}

                if (_renewal.ReturnDate != null)
                {
                    string _startcontractDay = _renewal.ReturnDate.Value.ToString("dd", new CultureInfo("ar-SY")).ToString();
                    string _startcontractMonth = _renewal.ReturnDate.Value.ToString("MMMM", new CultureInfo("ar-SY")).ToString();
                    string _startcontractYear = _renewal.ReturnDate.Value.ToString("yyyy", new CultureInfo("ar-SY")).ToString();
                    var lefttoright = ((Char)0x200E).ToString();
                    //var result = _startcontractDay + lefttoright + " " + _startcontractMonth + lefttoright + " " + _startcontractYear;



                    SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_dayReturnDate"))
                 .First<SdtRun>().Descendants<Text>().First().Text = _startcontractDay;
                    SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_monthReturnDate"))
               .First<SdtRun>().Descendants<Text>().First().Text = _startcontractMonth;
                    SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_yearReturnDate"))
               .First<SdtRun>().Descendants<Text>().First().Text = _startcontractYear;
                }

                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("ArTasks"))
                 .First<SdtRun>().Descendants<Text>().First().Text = _artasks;
                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("ArReturn"))
                .First<SdtRun>().Descendants<Text>().First().Text = _arreturn;
                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("ArChargeOf"))
                 .First<SdtRun>().Descendants<Text>().First().Text = _renewal.ArJobTitle;
                //SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("ArDeptname"))
                //.First<SdtRun>().Descendants<Text>().First().Text = _renewal.ArDeptName;
                //SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("ArDeptname"))
                //.First<SdtRun>().Descendants<Text>().First().Text = _renewal.ArDeptName;
                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("ArHCont"))
                 .First<SdtRun>().Descendants<Text>().First().Text = _arhiscountry;
                //if (_renewal.ContractEndDate != null)
                //{
                //    string _startcontractDay = _renewal.ContractEndDate.Value.ToString("dd", new CultureInfo("ar-SY")).ToString();
                //    string _startcontractMonth = _renewal.ContractEndDate.Value.ToString("MMMM", new CultureInfo("ar-SY")).ToString();
                //    string _startcontractYear = _renewal.ContractEndDate.Value.ToString("yyyy", new CultureInfo("ar-SY")).ToString();
                //    var lefttoright = ((Char)0x200E).ToString();
                //    var result = _startcontractYear + " " + _startcontractMonth + lefttoright + " " + _startcontractDay;


                //    SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("ArContractEndDate"))
                //     .First<SdtRun>().Descendants<Text>().First().Text = result;
                //}

                if (_renewal.ContractEndDate != null)
                {
                    string _startcontractDay = _renewal.ContractEndDate.Value.ToString("dd", new CultureInfo("ar-SY")).ToString();
                    string _startcontractMonth = _renewal.ContractEndDate.Value.ToString("MMMM", new CultureInfo("ar-SY")).ToString();
                    string _startcontractYear = _renewal.ContractEndDate.Value.ToString("yyyy", new CultureInfo("ar-SY")).ToString();
                    var lefttoright = ((Char)0x200E).ToString();
                    //var result = _startcontractDay + lefttoright + " " + _startcontractMonth + lefttoright + " " + _startcontractYear;



                    SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_dayContractEndDate"))
                 .First<SdtRun>().Descendants<Text>().First().Text = _startcontractDay;
                    SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_monthContractEndDate"))
               .First<SdtRun>().Descendants<Text>().First().Text = _startcontractMonth;
                    SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_yearContractEndDate"))
               .First<SdtRun>().Descendants<Text>().First().Text = _startcontractYear;
                }



                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("ArGrantMrLastName"))
                  .First<SdtRun>().Descendants<Text>().First().Text = _argrnated;

                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("ArHeIs"))
                  .First<SdtRun>().Descendants<Text>().First().Text = _ArHeIs;
                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("ArNationality"))
                  .First<SdtRun>().Descendants<Text>().First().Text = _renewal.ArNationality;
                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("ArCarry"))
                   .First<SdtRun>().Descendants<Text>().First().Text = _ArCarry;
                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("ArCarry"))
                  .First<SdtRun>().Descendants<Text>().First().Text = _ArCarry;
                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("ArPassportNumber"))
                   .First<SdtRun>().Descendants<Text>().First().Text = _unlp.DocumentNumber;
                if (_unlp.DocumentDateOfExpiry != null)
                {
                    string _startcontractDay = _unlp.DocumentDateOfExpiry.Value.ToString("dd", new CultureInfo("ar-SY")).ToString();
                    string _startcontractMonth = _unlp.DocumentDateOfExpiry.Value.ToString("MMMM", new CultureInfo("ar-SY")).ToString();
                    string _startcontractYear = _unlp.DocumentDateOfExpiry.Value.ToString("yyyy", new CultureInfo("ar-SY")).ToString();
                    var lefttoright = ((Char)0x200E).ToString();
                    var result = _startcontractDay + lefttoright + " " + _startcontractMonth + lefttoright + " " + _startcontractYear;



                    SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_dayPassExpiry"))
                 .First<SdtRun>().Descendants<Text>().First().Text = _startcontractDay;
                    SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_monthPassExpiry"))
               .First<SdtRun>().Descendants<Text>().First().Text = _startcontractMonth;
                    SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_yearPassExpiry"))
               .First<SdtRun>().Descendants<Text>().First().Text = _startcontractYear;
                }


                #endregion


                #region English
                //English Renewal 
                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("RefNumber"))
                  .First<SdtRun>().Descendants<Text>().First().Text = _renewal.RefNumber;
                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("EnStaffName"))
                   .First<SdtRun>().Descendants<Text>().First().Text = _renewal.EnStaffName;
                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_enOffice"))
                 .First<SdtRun>().Descendants<Text>().First().Text = _renewal.EnOffice;
                if (_renewal.ReturnDate != null)
                {
                    SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("EnReturnDate"))
                  .First<SdtRun>().Descendants<Text>().First().Text = _renewal.ReturnDate.Value.ToString("dd MMMM yyyy");
                }
                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("EnTasks"))
                  .First<SdtRun>().Descendants<Text>().First().Text = _entasks;
                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("EnDeptName"))
                  .First<SdtRun>().Descendants<Text>().First().Text = _renewal.EnDeptName;
                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("EnHCon"))
                  .First<SdtRun>().Descendants<Text>().First().Text = _entasks;
                if (_renewal.ContractEndDate != null)
                {

                    SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("EnContractEndDate"))
                  .First<SdtRun>().Descendants<Text>().First().Text = _renewal.ContractEndDate.Value.ToString("dd MMMM yyyy");
                }
                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("EnGrantLastName"))
                  .First<SdtRun>().Descendants<Text>().First().Text = _engrnated;
                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("EnHeIs"))
                  .First<SdtRun>().Descendants<Text>().First().Text = _enheShe;
                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("EnNationality"))
                  .First<SdtRun>().Descendants<Text>().First().Text = _renewal.EnNationality;
                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("EnPassportNumber"))
                  .First<SdtRun>().Descendants<Text>().First().Text = _unlp.DocumentNumber;
                if (_unlp.DocumentDateOfExpiry != null)
                {
                    SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("EnPassportValid"))
                 .First<SdtRun>().Descendants<Text>().First().Text = _unlp.DocumentDateOfExpiry.Value.ToString("dd MMMM yyyy");
                }
                if (_renewal.ContractEndDate != null)
                {
                    SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("EnContractEndDate"))
                   .First<SdtRun>().Descendants<Text>().First().Text = _renewal.ContractEndDate.Value.ToString("dd MMMM yyyy");
                }
                if (_renewal.SentDate != null)
                {
                    SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("EnDateToday"))
                   .First<SdtRun>().Descendants<Text>().First().Text = _renewal.SentDate.Value.ToString("dd MMMM yyyy");
                }

                #endregion











                mainDocumentPart.Document.Save();


            }

            byte[] filedata = System.IO.File.ReadAllBytes(destinationServerPath);
            string contentType = MimeMapping.GetMimeMapping(destinationServerPath);

            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = _renewal.RefNumber + "-" + _renewal.SentDate.Value.ToString("dd MMMM yyyy") + ".docx",
                Inline = true,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());

            return File(filedata, contentType);
        }

        public ActionResult PrintStaffStartWorkForm(Guid PK)
        {
            var _renewal = DbAHD.dataStaffRenwalResidency.Where(x => x.StaffRenwalResidencyGUID == PK).FirstOrDefault();
            // var notVerbaleStaff_AR = DbMRS.RP_NoteVerbaleStaff(PK, "AR").ToList();
            //  var notVerbaleVehicle_AR = DbMRS.RP_NoteVerbaleVehicle(PK, "AR").FirstOrDefault();

            string templatePath = "~/Areas/AHD/Templates/StaffResidencyRenewal/StaffStartWork.docx";
            var serverPath = Server.MapPath(templatePath);

            string destinationPath = "~/Areas/AHD/Temp/" + PK + ".docx";
            var destinationServerPath = Server.MapPath(destinationPath);

            System.IO.File.Copy(serverPath, destinationServerPath, true);

            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(destinationServerPath, true))
            {
                MainDocumentPart mainDocumentPart = wordDoc.MainDocumentPart;
                var SDTRun = mainDocumentPart.Document.Descendants<SdtRun>();
                var _staffperslan = DbAHD.userPersonalDetailsLanguage.Where(x => x.Active && x.UserGUID == _renewal.StaffGUID).ToList();
                #region Lables
                string _artasks = "";
                string _entasks = "";
                string _arreturn = "";
                string _arhiscountry = "";
                string _argrnated = "";
                string _engrnated = "";
                string _ArHeIs = "";
                string _ArCarry = "";
                string _enheShe = "";
                string _arstaffMr = "";
                string _enstaffMr = "";
                string _arassigned = "";
                string _enassigned = "";
                string _arEntered = "";
                string _thePersonar = "";

                if (_renewal.Gender == "Male")
                {
                    _artasks = "مهامه";
                    _arEntered = "دخل";
                    _arassigned = "تعيينه";
                    _entasks = "his";
                    _arreturn = "عاد";
                    _arhiscountry = "عقده";
                    _argrnated = "السيد" + " " + _staffperslan.Where(x => x.LanguageID == "AR").FirstOrDefault().Surname;
                    _engrnated = "Mr" + ". " + _staffperslan.Where(x => x.LanguageID == "EN").FirstOrDefault().Surname;

                    _arstaffMr = "السيد" + " " + _staffperslan.Where(x => x.LanguageID == "AR").FirstOrDefault().FirstName + " " + _staffperslan.Where(x => x.LanguageID == "AR").FirstOrDefault().Surname;
                    _enstaffMr = "Mr" + ". " + _staffperslan.Where(x => x.LanguageID == "EN").FirstOrDefault().FirstName + " " + _staffperslan.Where(x => x.LanguageID == "EN").FirstOrDefault().Surname; ;
                    _ArHeIs = "أنه";
                    _ArCarry = " يحمل";
                    _enheShe = "he";
                    _thePersonar = "الذي";
                }
                else
                {
                    _thePersonar = "التي";
                    _artasks = "مهامها";
                    _arassigned = "تعيينها";
                    _arEntered = "دخلت";
                    _entasks = "her";
                    _arreturn = "عادت";
                    _arhiscountry = "عقدها";
                    _argrnated = "السيدة" + " " + _staffperslan.Where(x => x.LanguageID == "AR").FirstOrDefault().Surname;
                    _engrnated = "Ms" + ". " + _staffperslan.Where(x => x.LanguageID == "EN").FirstOrDefault().Surname;

                    _arstaffMr = "السيدة" + " " + _staffperslan.Where(x => x.LanguageID == "AR").FirstOrDefault().FirstName + " " + _staffperslan.Where(x => x.LanguageID == "AR").FirstOrDefault().Surname;
                    _enstaffMr = "Ms" + ". " + _staffperslan.Where(x => x.LanguageID == "EN").FirstOrDefault().FirstName + " " + _staffperslan.Where(x => x.LanguageID == "EN").FirstOrDefault().Surname; ;
                    _ArHeIs = "أنها";
                    _ArCarry = " تحمل";
                    _enheShe = "she";
                }



                #endregion
                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("RefNumber"))
             .First<SdtRun>().Descendants<Text>().First().Text = _renewal.RefNumber;
                Guid unlpGUID = Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E3732");
                var _unlp = DbAHD.dataStaffCoreDocument.Where(x => x.UserGUID == _renewal.StaffGUID && x.DocumentTypeGUID == unlpGUID
                 && x.Active).FirstOrDefault();
                #region Arabic 
                //Arabic Renewal
                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("ArStaffName"))
                    .First<SdtRun>().Descendants<Text>().First().Text = _arstaffMr;

                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_arassigned"))
                      .First<SdtRun>().Descendants<Text>().First().Text = _arassigned;
                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_arDutyStation"))
                       .First<SdtRun>().Descendants<Text>().First().Text = _renewal.ArOffice;
                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_arentered"))
                    .First<SdtRun>().Descendants<Text>().First().Text = _arEntered;

                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_arJob"))
                 .First<SdtRun>().Descendants<Text>().First().Text = _renewal.ArJobTitle;

                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("thePersonar"))
              .First<SdtRun>().Descendants<Text>().First().Text = _thePersonar;



                if (_renewal.ContractStartDate != null)
                {


                    string _startcontractDay = _renewal.ContractStartDate.Value.ToString("dd", new CultureInfo("ar-SY")).ToString();
                    string _startcontractMonth = _renewal.ContractStartDate.Value.ToString("MMMM", new CultureInfo("ar-SY")).ToString();
                    string _startcontractYear = _renewal.ContractStartDate.Value.ToString("yyyy", new CultureInfo("ar-SY")).ToString();
                    var lefttoright = ((Char)0x200E).ToString();
                    var result = _startcontractYear + " " + _startcontractMonth + lefttoright + " " + _startcontractDay;
                    SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_arContractStartDay"))
                 .First<SdtRun>().Descendants<Text>().First().Text = _startcontractDay;
                    SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_arContractStartMonth"))
               .First<SdtRun>().Descendants<Text>().First().Text = _startcontractMonth;
                    SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_arContractStartYear"))
               .First<SdtRun>().Descendants<Text>().First().Text = _startcontractYear;

                }

                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_arherhisContract"))
                    .First<SdtRun>().Descendants<Text>().First().Text = _arhiscountry;


                if (_renewal.ContractEndDate != null)
                {

                    //string _startcontractDay = _renewal.ContractEndDate.Value.ToString("dd", new CultureInfo("ar-SY")).ToString();
                    //string _startcontractMonth = _renewal.ContractEndDate.Value.ToString("MMMM", new CultureInfo("ar-SY")).ToString();
                    //string _startcontractYear = _renewal.ContractEndDate.Value.ToString("yyyy", new CultureInfo("ar-SY")).ToString();
                    //var lefttoright = ((Char)0x200E).ToString();
                    //var result = _startcontractYear + " " + _startcontractMonth + lefttoright + " " + _startcontractDay;


                    //SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_arContractEndate"))
                    //  .First<SdtRun>().Descendants<Text>().First().Text = result;


                    string _startcontractDay = _renewal.ContractEndDate.Value.ToString("dd", new CultureInfo("ar-SY")).ToString();
                    string _startcontractMonth = _renewal.ContractEndDate.Value.ToString("MMMM", new CultureInfo("ar-SY")).ToString();
                    string _startcontractYear = _renewal.ContractEndDate.Value.ToString("yyyy", new CultureInfo("ar-SY")).ToString();
                    var lefttoright = ((Char)0x200E).ToString();
                    var result = _startcontractYear + " " + _startcontractMonth + lefttoright + " " + _startcontractDay;
                    SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_arContractdayEndate"))
                 .First<SdtRun>().Descendants<Text>().First().Text = _startcontractDay;
                    SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_arContractMonthEndate"))
               .First<SdtRun>().Descendants<Text>().First().Text = _startcontractMonth;
                    SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_arContractyeardate"))
               .First<SdtRun>().Descendants<Text>().First().Text = _startcontractYear;


                }

                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("ArGrantMrLastName"))
                      .First<SdtRun>().Descendants<Text>().First().Text = _argrnated;

                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_arHerHis"))
                  .First<SdtRun>().Descendants<Text>().First().Text = _ArHeIs;

                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_arNationality"))
                .First<SdtRun>().Descendants<Text>().First().Text = _renewal.ArNationality;

                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_arCarry"))
                     .First<SdtRun>().Descendants<Text>().First().Text = _ArCarry;
                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_arPassportNumber"))
                  .First<SdtRun>().Descendants<Text>().First().Text = _unlp.DocumentNumber;
                if (_unlp.DocumentDateOfExpiry != null)
                {
                    string _startcontractDay = _unlp.DocumentDateOfExpiry.Value.ToString("dd", new CultureInfo("ar-SY")).ToString();
                    string _startcontractMonth = _unlp.DocumentDateOfExpiry.Value.ToString("MMMM", new CultureInfo("ar-SY")).ToString();
                    string _startcontractYear = _unlp.DocumentDateOfExpiry.Value.ToString("yyyy", new CultureInfo("ar-SY")).ToString();
                    var lefttoright = ((Char)0x200E).ToString();
                    var result = _startcontractYear + " " + _startcontractMonth + lefttoright + " " + _startcontractDay;


                    //string formated = string.Format("{0}{1}{2}", _startcontractDay, _startcontractMonth, _startcontractYear);




                    SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_arcontractDay"))
                 .First<SdtRun>().Descendants<Text>().First().Text = _startcontractDay;
                    SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_arcontractMonth"))
               .First<SdtRun>().Descendants<Text>().First().Text = _startcontractMonth;
                    SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_arcontractYear"))
               .First<SdtRun>().Descendants<Text>().First().Text = _startcontractYear;

                    //SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_arPassportValidty"))
                    //.First<SdtRun>().Descendants<Text>().First().Text = result;
                }

                //if (_renewal.SentDate != null)
                //{
                //    string _startcontractDay = _renewal.SentDate.Value.ToString("dd", new CultureInfo("ar-SY")).ToString();
                //    string _startcontractMonth = _renewal.SentDate.Value.ToString("MMMM", new CultureInfo("ar-SY")).ToString();
                //    string _startcontractYear = _renewal.SentDate.Value.ToString("yyyy", new CultureInfo("ar-SY")).ToString();
                //    var lefttoright = ((Char)0x200E).ToString();
                //    var result = _startcontractYear + " " + _startcontractMonth + lefttoright + " " + _startcontractDay;


                //    SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("ArDateToday"))
                //      .First<SdtRun>().Descendants<Text>().First().Text = result;
                //}

                #endregion


                #region English
                //English Renewal 

                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("EnStaffName"))
                   .First<SdtRun>().Descendants<Text>().First().Text = _enstaffMr;

                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_EnJob"))
                         .First<SdtRun>().Descendants<Text>().First().Text = _renewal.EnJobTitle;


                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_enDutyStation"))
                        .First<SdtRun>().Descendants<Text>().First().Text = _renewal.EnOffice;



                if (_renewal.ContractStartDate != null)
                {
                    SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_enEntryDate"))
                  .First<SdtRun>().Descendants<Text>().First().Text = _renewal.ContractStartDate.Value.ToString("dd MMMM yyyy");
                }

                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_enHisHer"))
                  .First<SdtRun>().Descendants<Text>().First().Text = _entasks;

                if (_renewal.ContractEndDate != null)
                {

                    SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_EnContractEndDate"))
                  .First<SdtRun>().Descendants<Text>().First().Text = _renewal.ContractEndDate.Value.ToString("dd MMMM yyyy");

                }

                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("EnGrantLastName"))
                     .First<SdtRun>().Descendants<Text>().First().Text = _engrnated;


                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("EnHeIs"))
                  .First<SdtRun>().Descendants<Text>().First().Text = _enheShe;

                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("EnNationality"))
                  .First<SdtRun>().Descendants<Text>().First().Text = _renewal.EnNationality;

                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("EnPassportNumber"))
                  .First<SdtRun>().Descendants<Text>().First().Text = _unlp.DocumentNumber;
                if (_unlp.DocumentDateOfExpiry != null)
                {
                    SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("EnPassportValid"))
                 .First<SdtRun>().Descendants<Text>().First().Text = _unlp.DocumentDateOfExpiry.Value.ToString("dd MMMM yyyy");
                }

                if (_renewal.SentDate != null)
                {
                    SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("EnDateToday"))
                   .First<SdtRun>().Descendants<Text>().First().Text = _renewal.SentDate.Value.ToString("dd MMMM yyyy");
                }

                #endregion











                mainDocumentPart.Document.Save();


            }

            byte[] filedata = System.IO.File.ReadAllBytes(destinationServerPath);
            string contentType = MimeMapping.GetMimeMapping(destinationServerPath);

            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = _renewal.RefNumber + "-" + _renewal.SentDate.Value.ToString("dd MMMM yyyy") + ".docx",
                Inline = true,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());

            return File(filedata, contentType);
        }



        #endregion

        #region Vehicle Forms 
        public ActionResult PrintVehicleMaintenanceForm(Guid PK)
        {
            var _vehicle = DbAHD.v_VehicleMaintenanceRequest.Where(x => x.VehicleMaintenanceRequestGUID == PK).FirstOrDefault();
            // var notVerbaleStaff_AR = DbMRS.RP_NoteVerbaleStaff(PK, "AR").ToList();
            //  var notVerbaleVehicle_AR = DbMRS.RP_NoteVerbaleVehicle(PK, "AR").FirstOrDefault();

            string templatePath = "~/Areas/AHD/Templates/Vehicles/VehicleMaintenance.docx";
            var serverPath = Server.MapPath(templatePath);

            string destinationPath = "~/Areas/AHD/Temp/" + PK + ".docx";
            var destinationServerPath = Server.MapPath(destinationPath);

            System.IO.File.Copy(serverPath, destinationServerPath, true);

            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(destinationServerPath, true))
            {
                MainDocumentPart mainDocumentPart = wordDoc.MainDocumentPart;
                var SDTRun = mainDocumentPart.Document.Descendants<SdtRun>();



                #region Arabic 
                //Arabic Renewal
                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("RequestNumber"))
                    .First<SdtRun>().Descendants<Text>().First().Text = _vehicle.RequestNumber;
                if (_vehicle.RequestDate != null)
                {
                    SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_RequestDate"))
                        .First<SdtRun>().Descendants<Text>().First().Text = _vehicle.RequestDate.Value.ToString("dd/MM/yyyy", new CultureInfo("ar-SY"));
                }
                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_vehicleType"))
               .First<SdtRun>().Descendants<Text>().First().Text = _vehicle.VehicleType;

                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_vehicleType"))
               .First<SdtRun>().Descendants<Text>().First().Text = _vehicle.VehicleType;

                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_vehicleModel"))
                .First<SdtRun>().Descendants<Text>().First().Text = _vehicle.VehicleModel;

                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_vehicleColor"))
                .First<SdtRun>().Descendants<Text>().First().Text = _vehicle.VehicleColor;

                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_YearOfProducation"))
                .First<SdtRun>().Descendants<Text>().First().Text = _vehicle.ManufacturingYear.Value.ToString();

                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_ChassisNumber"))
                .First<SdtRun>().Descendants<Text>().First().Text = _vehicle.ChassisNumber;

                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_PlateNumber"))
                .First<SdtRun>().Descendants<Text>().First().Text = _vehicle.PlateNumber.ToString();

                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_EngineNumber"))
                .First<SdtRun>().Descendants<Text>().First().Text = _vehicle.EngineNumber;





                if (_vehicle.LastRenewalDate != null)
                {
                    string _startcontractDay = _vehicle.LastRenewalDate.Value.ToString("dd", new CultureInfo("ar-SY")).ToString();
                    string _startcontractMonth = _vehicle.LastRenewalDate.Value.ToString("MMMM", new CultureInfo("ar-SY")).ToString();
                    string _startcontractYear = _vehicle.LastRenewalDate.Value.ToString("yyyy", new CultureInfo("ar-SY")).ToString();

                    //string formated = string.Format("{0}{1}{2}", _startcontractDay, _startcontractMonth, _startcontractYear);

                    var lefttoright = ((Char)0x200E).ToString();
                    var result = _startcontractYear + " " + _startcontractMonth + lefttoright + " " + _startcontractDay;


                    SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("_LastRenewalDate"))
                     .First<SdtRun>().Descendants<Text>().First().Text = result;
                }


                #endregion



                mainDocumentPart.Document.Save();


            }

            byte[] filedata = System.IO.File.ReadAllBytes(destinationServerPath);
            string contentType = MimeMapping.GetMimeMapping(destinationServerPath);

            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = _vehicle.RequestNumber + "-" + _vehicle.RequestDate.Value.ToString("dd MMMM yyyy") + ".docx",
                Inline = true,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());

            return File(filedata, contentType);
        }


        #endregion

        #region Delgation Report 
        public ActionResult PrintBLOMDelegationReport(Guid PK)
        {
            var _staff = DbAHD.dataBlomShuttleDelegationStaffRequest.Where(x => x.BlomShuttleDelegationStaffRequestGUID == PK).FirstOrDefault();


            string templatePath = "~/Areas/AHD/Templates/BLOMDelegation/DelegationLetterArabic.docx";
            var serverPath = Server.MapPath(templatePath);

            string destinationPath = "~/Areas/AHD/Temp/" + PK + ".docx";
            var destinationServerPath = Server.MapPath(destinationPath);

            System.IO.File.Copy(serverPath, destinationServerPath, true);

            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(destinationServerPath, true))
            {
                MainDocumentPart mainDocumentPart = wordDoc.MainDocumentPart;
                var SDTRun = mainDocumentPart.Document.Descendants<SdtRun>();



                #region Arabic 
                //Arabic Renewal
                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("arFullName"))
                    .First<SdtRun>().Descendants<Text>().First().Text = _staff.StaffName;

                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("arStaffSyrianID"))
               .First<SdtRun>().Descendants<Text>().First().Text = _staff.SyrianIDNumber;

                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("arNationalIDNumber"))
               .First<SdtRun>().Descendants<Text>().First().Text = _staff.NationalIDNumber;

                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("arAccountNumber"))
                .First<SdtRun>().Descendants<Text>().First().Text = _staff.AccountNumber;

                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("arBranchName"))
                .First<SdtRun>().Descendants<Text>().First().Text = _staff.BranchName;

                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("arTraverlName"))
                .First<SdtRun>().Descendants<Text>().First().Text = _staff.dataBlomShuttleDelegationTraveler.TravelerName;

                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("arTraverlerSyrianIdNumber"))
                .First<SdtRun>().Descendants<Text>().First().Text = _staff.dataBlomShuttleDelegationTraveler.SyrianIDNumber;

                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("artraverlNationalIDNumber"))
                .First<SdtRun>().Descendants<Text>().First().Text = _staff.dataBlomShuttleDelegationTraveler.NationalIDNumber.ToString();

                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("arAmountUSD"))
                .First<SdtRun>().Descendants<Text>().First().Text = _staff.AmountUSD.ToString();
                #endregion



                mainDocumentPart.Document.Save();


            }

            byte[] filedata = System.IO.File.ReadAllBytes(destinationServerPath);
            string contentType = MimeMapping.GetMimeMapping(destinationServerPath);

            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = _staff.StaffName + "-" + _staff.dataBlomShuttleDelegationDate.StartDate.Value.ToString("dd MMMM yyyy") + ".docx",
                Inline = true,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());

            return File(filedata, contentType);
        }


        public ActionResult PrintUploadedBLOMDelegationStaffReport(Guid PK)
        {
            var _staff = DbAHD.dataBlomShuttleDelegationStaffRequest.Where(x => x.BlomShuttleDelegationStaffRequestGUID == PK).FirstOrDefault();
            string sourceFile = Server.MapPath("~\\Areas\\AHD\\Templates\\BLOMDelegation\\StaffDelegations\\" + _staff.BlomShuttleDelegationDateGUID.ToString() + "\\" + _staff.BlomShuttleDelegationStaffRequestGUID.ToString() + ".pdf");
            string DisFolder =
                Server.MapPath("~/Areas/AHD/temp/" + DateTime.Now.ToBinary() + ".PDF");
            System.IO.File.Copy(sourceFile, DisFolder);
            byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);
            string fileName = _staff.StaffName + DateTime.Now.ToString("yyMMdd") + ".PDF";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);



            //string templatePath = "~/Areas/AHD/Templates/BLOMDelegation/DelegationLetterArabic.docx";
            //var serverPath = Server.MapPath(templatePath);

            //string destinationPath = "~/Areas/AHD/Temp/" + PK + ".docx";
            //var destinationServerPath = Server.MapPath(destinationPath);

            //System.IO.File.Copy(serverPath, destinationServerPath, true);



            //byte[] filedata = System.IO.File.ReadAllBytes(destinationServerPath);
            //string contentType = MimeMapping.GetMimeMapping(destinationServerPath);

            //var cd = new System.Net.Mime.ContentDisposition
            //{
            //    FileName = _staff.StaffName + "-" + _staff.dataBlomShuttleDelegationDate.StartDate.Value.ToString("dd MMMM yyyy") + ".docx",
            //    Inline = true,
            //};

            //Response.AppendHeader("Content-Disposition", cd.ToString());

            //return File(filedata, contentType);
        }

        #endregion

        #region Reports Staff
        [HttpGet]
        //public ActionResult ReportGenerate()
        //{
        //    ReportViewer reportViewer = new ReportViewer();
        //    ViewBag.ReportViewer = reportViewer;
        //    return View("~/Areas/AHD/Views/Reports/ReportsBoard.cshtml", new SHMReportParameters());
        //}
        public ActionResult StaffRenewalResidencyReport(Guid PK)
        {
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.ZoomMode = ZoomMode.Percent;
            reportViewer.ZoomPercent = 100;
            reportViewer.Width = System.Web.UI.WebControls.Unit.Pixel(1372);
            reportViewer.Height = System.Web.UI.WebControls.Unit.Pixel(800);

            reportViewer.AsyncRendering = true;
            reportViewer.LocalReport.DataSources.Clear();
            AHDDataSet ResidencyRenewal_En = new AHDDataSet();
            AHDDataSet ResidencyRenewal_Ar = new AHDDataSet();

            sp_StaffRenewalResidencyTableAdapter RP_StaffRenewal_EN = new sp_StaffRenewalResidencyTableAdapter();
            RP_StaffRenewal_EN.Fill(ResidencyRenewal_En.sp_StaffRenewalResidency, PK, "EN");
            ReportDataSource reportDataSource_EN = new ReportDataSource("ResidencyRenewal_En", ResidencyRenewal_En.Tables["sp_StaffRenewalResidency"]);
            reportViewer.LocalReport.DataSources.Add(reportDataSource_EN);

            sp_StaffRenewalResidencyTableAdapter RP_StaffRenewal_Ar = new sp_StaffRenewalResidencyTableAdapter();

            RP_StaffRenewal_Ar.Fill(ResidencyRenewal_Ar.sp_StaffRenewalResidency, PK, "AR");
            ReportDataSource reportDataSource_AR = new ReportDataSource("ResidencyRenewal_Ar", ResidencyRenewal_Ar.Tables["sp_StaffRenewalResidency"]);
            reportViewer.LocalReport.DataSources.Add(reportDataSource_AR);


            //ReportDataSource reportDataSource_EN = new ReportDataSource("RP_NoteVerbale_EN", amsDataSet_EN.Tables["RP_NoteVerbale"]);


            //RP_NoteVerbaleTableAdapter RP_NoteVerbale_AR = new RP_NoteVerbaleTableAdapter();

            //RP_NoteVerbale_EN.Fill(amsDataSet_AR.RP_NoteVerbale, PK, "AR");
            //ReportDataSource reportDataSource_AR = new ReportDataSource("RP_NoteVerbale_AR", amsDataSet_AR.Tables["RP_NoteVerbale"]);
            //reportViewer.LocalReport.DataSources.Add(reportDataSource_AR);


            //reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", res));

            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Areas/AHD/Rdlc/StaffForms\StaffRenewalResidency.rdlc";
            ViewBag.ReportViewer = reportViewer;

            return View("~/Areas/AHD/Views/Reports/Index.cshtml", null, null);
        }



        #endregion

        #region RAndRLeave Report
        public ActionResult InternationalRAndRLeaveReport(Guid PK)
        {

            ReportViewer reportViewer = new ReportViewer();
            LocalReport localReport = new LocalReport();
            //localReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"/Reports/VoucherReport.rdlc";
            //localReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Areas/AHD/Rdlc/VehicalForms\VehicleMaintenanceRequest.rdlc";
            localReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Areas/AHD/Rdlc/R_R\InternationalStaffR_RForm.rdlc";

            v_InternationalStaffRestAndRecperationLeaveTableAdapter result = new v_InternationalStaffRestAndRecperationLeaveTableAdapter();
            var results = result.GetData().Where(c => c.RestAndRecuperationLeaveGUID == PK).ToList();

            results = results.ToList();

            if (results == null)
            {
                return PartialView("_Empty");
            }

            DataTable dt = results.ToList().CopyToDataTable();



            localReport.DataSources.Add(new ReportDataSource("DataSet1", dt));


            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension = "pdf";
            //The DeviceInfo settings should be changed based on the reportType 
            string deviceInfo = @"<DeviceInfo>              
                                         <OutputFormat>PDF</OutputFormat>              
                                         <PageWidth>21cm</PageWidth>              
                                         <PageHeight>29.7cm</PageHeight>          
                                         <MarginTop>0cm</MarginTop>          
                                         <MarginLeft>0cm</MarginLeft>            
                                         <MarginRight>0cm</MarginRight>       
                                         <MarginBottom>0cm</MarginBottom></DeviceInfo>";
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;


            renderedBytes = localReport.Render(
                reportType, deviceInfo, out mimeType, out encoding, out fileNameExtension,
                out streams, out warnings);

            var doc = new iTextSharp.text.Document();
            doc.SetPageSize(iTextSharp.text.PageSize.A4);

            var reader = new PdfReader(renderedBytes);
            using (FileStream fs =
                new FileStream(
                    Server.MapPath("~/Uploads/AHD/Temp/Report_" + Convert.ToString(Session[SessionKeys.UserGUID]) +
                                   ".pdf"), FileMode.Create))
            {
                PdfStamper stamper = new PdfStamper(reader, fs);

                stamper.JavaScript =
                    "var pp = getPrintParams();pp.interactive = pp.constants.interactionLevel.automatic;pp.printerName = getPrintParams().printerName;print(pp);\r";
                stamper.Close();
            }

            reader.Close();
            FileStream fss =
                new FileStream(
                    Server.MapPath("~/Uploads/AHD/Temp/Report_" + Convert.ToString(Session[SessionKeys.UserGUID]) +
                                   ".pdf"), FileMode.Open);
            byte[] bytes = new byte[fss.Length];
            fss.Read(bytes, 0, Convert.ToInt32(fss.Length));
            fss.Close();
            System.IO.File.Delete(
                Server.MapPath("~/Uploads/AHD/Temp/Report_" + Convert.ToString(Session[SessionKeys.UserGUID]) +
                               ".pdf"));

            //Here we returns the file result for view(PDF)

            return File(bytes, "application/pdf");

        }

        #endregion

        #region Entitlements
        public ActionResult PrintInternationalEntitlementReport(Guid PK)
        {

            ReportViewer reportViewer = new ReportViewer();
            LocalReport localReport = new LocalReport();
            //localReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"/Reports/VoucherReport.rdlc";
            //localReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Areas/AHD/Rdlc/VehicalForms\VehicleMaintenanceRequest.rdlc";
            localReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Areas/AHD/Rdlc/Entitlements\Entitlements.rdlc";

            v_InterntionalStaffEntitlementTableAdapter result = new v_InterntionalStaffEntitlementTableAdapter();
            var results = result.GetData().Where(c => c.InternationalStaffEntitlementGUID == PK).ToList();

            results = results.ToList();

            if (results == null)
            {
                return PartialView("_Empty");
            }

            DataTable dt = results.ToList().CopyToDataTable();



            localReport.DataSources.Add(new ReportDataSource("DataSet1", dt));


            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension = "pdf";
            //The DeviceInfo settings should be changed based on the reportType 
            string deviceInfo = @"<DeviceInfo>              
                                         <OutputFormat>PDF</OutputFormat>              
                                         <PageWidth>21cm</PageWidth>              
                                         <PageHeight>29.7cm</PageHeight>          
                                         <MarginTop>0cm</MarginTop>          
                                         <MarginLeft>0cm</MarginLeft>            
                                         <MarginRight>0cm</MarginRight>       
                                         <MarginBottom>0cm</MarginBottom></DeviceInfo>";
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;


            renderedBytes = localReport.Render(
                reportType, deviceInfo, out mimeType, out encoding, out fileNameExtension,
                out streams, out warnings);

            var doc = new iTextSharp.text.Document();
            doc.SetPageSize(iTextSharp.text.PageSize.A4);

            var reader = new PdfReader(renderedBytes);
            using (FileStream fs =
                new FileStream(
                    Server.MapPath("~/Uploads/AHD/Temp/Report_" + Convert.ToString(Session[SessionKeys.UserGUID]) +
                                   ".pdf"), FileMode.Create))
            {
                PdfStamper stamper = new PdfStamper(reader, fs);

                stamper.JavaScript =
                    "var pp = getPrintParams();pp.interactive = pp.constants.interactionLevel.automatic;pp.printerName = getPrintParams().printerName;print(pp);\r";
                stamper.Close();
            }

            reader.Close();
            FileStream fss =
                new FileStream(
                    Server.MapPath("~/Uploads/AHD/Temp/Report_" + Convert.ToString(Session[SessionKeys.UserGUID]) +
                                   ".pdf"), FileMode.Open);
            byte[] bytes = new byte[fss.Length];
            fss.Read(bytes, 0, Convert.ToInt32(fss.Length));
            fss.Close();
            System.IO.File.Delete(
                Server.MapPath("~/Uploads/AHD/Temp/Report_" + Convert.ToString(Session[SessionKeys.UserGUID]) +
                               ".pdf"));

            //Here we returns the file result for view(PDF)

            return File(bytes, "application/pdf");

        }

        #endregion

        #region Report for entitlements excel 

        public ActionResult ExportAllStaffEntitlements()
        {
            if (!CMS.HasAction(Permissions.InternationalStaffEntitlements.Update, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var result = DbAHD.dataAHDInternationalStaffEntitlement.ToList();

            if (result.Count > 0)
            {
                string sourceFile = Server.MapPath("~/Areas/AHD/Templates/InternationalStaffEntitlementsAllPeriods.xlsx");
                string DisFolder =
                    Server.MapPath("~/Areas/AHD/Temp/ListEntitlementStaff" + DateTime.Now.ToBinary() + ".xlsx");
                System.IO.File.Copy(sourceFile, DisFolder);
                using (ExcelPackage package = new ExcelPackage(new FileInfo(DisFolder)))
                {
                    var cx = package.Workbook.Worksheets.ToList();
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[1];
                    DataTable dt = new DataTable();
                    dt.Columns.Add("Period", typeof(string));
                    dt.Columns.Add("Duty Station", typeof(string));
                    dt.Columns.Add("Staff Name", typeof(string));
                    dt.Columns.Add("Status", typeof(string));

                    dt.Columns.Add("Total Entitlements", typeof(decimal));
                    dt.Columns.Add("Danger Pay Per Day", typeof(decimal));
                    dt.Columns.Add("R&R Ticket Amount", typeof(decimal));
                    dt.Columns.Add("Added Reimbursement", typeof(decimal));
                    dt.Columns.Add("Total Added Amount", typeof(decimal));
                    dt.Columns.Add("Rental Deduction", typeof(decimal));
                    dt.Columns.Add("Deducted Recovery", typeof(decimal));
                    dt.Columns.Add("Total Dducation Amount", typeof(decimal));

                    dt.Columns.Add("Payment Method", typeof(string));
                    dt.Columns.Add("Staff Comments", typeof(string));
                    dt.Columns.Add("Prepared By", typeof(string));
                    dt.Columns.Add("Prepared Date", typeof(DateTime));
                    dt.Columns.Add("Approved By", typeof(string));
                    dt.Columns.Add("Approved Date", typeof(DateTime));

                    dt.Columns.Add("Certified By", typeof(string));
                    dt.Columns.Add("Certified Date", typeof(DateTime));
                    dt.Columns.Add("Certifier Comments", typeof(string));
                    dt.Columns.Add("Finance Approved By", typeof(string));
                    //dt.Columns.Add("Finance Approved Date", typeof(DateTime));

                    var _types = DbAHD.codeAHDEntitlementType.Where(x => x.Active).ToList();
                    var _users = DbAHD.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN).ToList();
                    var _staffCores = DbAHD.StaffCoreData.Where(x => x.Active).ToList();
                    var _periods = DbAHD.dataAHDPeriodEntitlement.ToList();
                    var _entitlemntsDet = DbAHD.dataAHDInternationalStaffEntitlementDetail.ToList();
                    var _dutyStations = DbAHD.codeDutyStationsLanguages.Where(x => x.LanguageID == LAN && x.Active).ToList();

                    Guid _paymentMethod = Guid.Parse("66CD375C-A576-4AA4-8AF4-FF3C1C5E7579");
                    var _valuesPayment = DbAHD.codeTablesValuesLanguages.Where(x => x.codeTablesValues.TableGUID == _paymentMethod
                                              && x.LanguageID == LAN).ToList();
                    Guid _entiStatus = Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E3676");
                    var _EntStatus = DbAHD.codeTablesValuesLanguages.Where(x => x.codeTablesValues.TableGUID == _entiStatus
                                        && x.LanguageID == LAN).ToList();
                    foreach (var item in result.OrderByDescending(x => x.dataAHDPeriodEntitlement.OrderId).ThenBy(x => x.StaffName))
                    {

                        DataRow dr;
                        dr = dt.NewRow();
                        #region calc
                        var _currStaff = _staffCores.Where(x => x.UserGUID == item.StaffGUID).FirstOrDefault();

                        var _period = _periods.Where(x => x.PeriodEntitlementGUID == item.PeriodEntitlementGUID).FirstOrDefault();
                        var _totalDet = _entitlemntsDet.Where(x => x.InternationalStaffEntitlementGUID == item.InternationalStaffEntitlementGUID).ToList();
                        var _totalPlus = _totalDet.Where(x => x.IsToAdd == true).Select(x => x.TotalAmount).FirstOrDefault();
                        var _totalMinus = _totalDet.Where(x => x.IsToAdd == false).Select(x => x.TotalAmount).FirstOrDefault();

                        var _totalDP = _totalDet.Where(x => x.EntitlementTypeGUID ==
                                        InternationalStaffEntitlementType.DangerPayPerDay).Select(x => x.TotalAmount).FirstOrDefault();
                        var _totalRR = _totalDet.Where(x => x.EntitlementTypeGUID ==
                                        InternationalStaffEntitlementType.R_RTicket).Select(x => x.TotalAmount).FirstOrDefault();
                        var _totalAddedReimbur = _totalDet.Where(x => x.EntitlementTypeGUID ==
                                        InternationalStaffEntitlementType.AddedRecorvery).Select(x => x.TotalAmount).FirstOrDefault();

                        var _totalRental = _totalDet.Where(x => x.EntitlementTypeGUID ==
                                        InternationalStaffEntitlementType.RentalDeduction).Select(x => x.TotalAmount).FirstOrDefault();
                        var _totalDed = _totalDet.Where(x => x.EntitlementTypeGUID ==
                                        InternationalStaffEntitlementType.DeductedRecovery).Select(x => x.TotalAmount).FirstOrDefault();


                        #endregion

                        dr[0] = _period.MonthName;
                        dr[1] = _dutyStations.Where(x => x.DutyStationGUID == _currStaff.DutyStationGUID).FirstOrDefault().DutyStationDescription;
                        dr[2] = item.StaffName;
                        dr[3] = _EntStatus.Where(x => x.ValueGUID == item.FlowStatusGUID).FirstOrDefault().ValueDescription;
                        dr[4] = _totalPlus ?? 0 + _totalMinus ?? 0;
                        dr[5] = _totalDP ?? 0;
                        dr[6] = _totalRR ?? 0;
                        dr[7] = _totalAddedReimbur ?? 0;
                        dr[8] = _totalPlus ?? 0;
                        dr[9] = _totalRental ?? 0;
                        dr[10] = _totalDed ?? 0;
                        dr[11] = _totalMinus ?? 0;
                        dr[12] = _valuesPayment.Where(x => x.ValueGUID == item.PaymentMethodGUID).FirstOrDefault() != null ? _valuesPayment.Where(x => x.ValueGUID == item.PaymentMethodGUID).FirstOrDefault().ValueDescription : "";
                        dr[13] = item.StaffComment;
                        dr[14] = _users.Where(x => x.UserGUID == item.PreparedByGUID).FirstOrDefault() != null ?
                                 _users.Where(x => x.UserGUID == item.PreparedByGUID).FirstOrDefault().FirstName + " " +
                                 _users.Where(x => x.UserGUID == item.PreparedByGUID).FirstOrDefault().FirstName
                                 : "";
                        dr[15] = item.PreparedDate == null ? (object)DBNull.Value : Convert.ToDateTime(item.PreparedDate).ToShortDateString();

                        dr[16] = _users.Where(x => x.UserGUID == item.ApprovedByGUID).FirstOrDefault() != null ?
                                 _users.Where(x => x.UserGUID == item.ApprovedByGUID).FirstOrDefault().FirstName + " " +
                                 _users.Where(x => x.UserGUID == item.ApprovedByGUID).FirstOrDefault().FirstName
                                 : "";
                        dr[17] = item.ApprovedDate == null ? (object)DBNull.Value : Convert.ToDateTime(item.ApprovedDate).ToShortDateString();


                        dr[18] = _users.Where(x => x.UserGUID == item.CertifiedByGUID).FirstOrDefault() != null ?
                                 _users.Where(x => x.UserGUID == item.CertifiedByGUID).FirstOrDefault().FirstName + " " +
                                 _users.Where(x => x.UserGUID == item.CertifiedByGUID).FirstOrDefault().FirstName
                                 : "";
                        dr[19] = item.CertifiedDate == null ? (object)DBNull.Value : Convert.ToDateTime(item.CertifiedDate).ToShortDateString();
                        dr[20] = item.CertifierComment;
                        dr[21] = _users.Where(x => x.UserGUID == item.FinanceApprovedByGUID).FirstOrDefault() != null ?
                                 _users.Where(x => x.UserGUID == item.FinanceApprovedByGUID).FirstOrDefault().FirstName + " " +
                                 _users.Where(x => x.UserGUID == item.FinanceApprovedByGUID).FirstOrDefault().FirstName
                                 : "";
                        //dr[22] = item.FinanceApprovedDate == null ? (object)DBNull.Value : Convert.ToDateTime(item.FinanceApprovedDate).ToShortDateString();



                        dt.Rows.Add(dr);
                    }
                    workSheet.Cells["B4"].LoadFromDataTable(dt, true);
                    //workSheet.Cells["A1"].Value = "_List of  national staff danger pay as of " + result.Select(x => x.PaymentDurationName).FirstOrDefault();

                    package.Save();
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);

                string fileName = " Staff Entitlements Information" + Guid.NewGuid() + ".xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }

            string success = "No Data Available for this period";
            return Json(new { success = success }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult ExportEntitlementsReport(Guid id)
        {
            if (!CMS.HasAction(Permissions.InternationalStaffEntitlements.Access, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var result = DbAHD.v_InterntionalStaffEntitlement.Where(x => x.PeriodEntitlementGUID == id).
                Select(x => new { x.StaffComment, x.InternationalStaffEntitlementGUID, x.PeriodName, x.PaymentMethod, x.FullName, x.DutyStation, x.FlowStatusGUID, x.TotalAmount, x.PreparedBy, x.CertifiedBy, x.FinanceApprovedBy }).Distinct().ToList();
            var _resDet = DbAHD.dataAHDInternationalStaffEntitlementDetail.Where(x => x.dataAHDInternationalStaffEntitlement.PeriodEntitlementGUID == id && x.Active == true).ToList();

            if (result.Count > 0)
            {
                string sourceFile = Server.MapPath("~/Areas/AHD/Templates/InternationalStaffEntitlements.xlsx");
                string DisFolder =
                    Server.MapPath("~/Areas/AHD/Temp/InternationalStaffEntitlements" + DateTime.Now.ToBinary() + ".xlsx");
                System.IO.File.Copy(sourceFile, DisFolder);
                using (ExcelPackage package = new ExcelPackage(new FileInfo(DisFolder)))
                {
                    var cx = package.Workbook.Worksheets.ToList();
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[1];

                    DataTable dt = new DataTable();
                    dt.Columns.Add("Staff Name", typeof(string));
                    dt.Columns.Add("Duty Station", typeof(string));
                    dt.Columns.Add("Status", typeof(string));
                    dt.Columns.Add("Total Amount", typeof(string));
                    dt.Columns.Add("DP Amount", typeof(string));
                    dt.Columns.Add("R&R Ticket Amount", typeof(string));
                    dt.Columns.Add("Added Recovery Amount", typeof(string));

                    dt.Columns.Add("Rental Amount", typeof(string));
                    dt.Columns.Add("Breakfast Amount", typeof(string));

                    dt.Columns.Add("Deducted Recovery Amount", typeof(string));
                    dt.Columns.Add("Staff Comment", typeof(string));
                    dt.Columns.Add("Prepared By", typeof(string));
                    dt.Columns.Add("Certified By", typeof(string));
                    dt.Columns.Add("Finance Approved By", typeof(string));
                    dt.Columns.Add("Payment Method", typeof(string));
                    //dt.Columns.Add("SAT Phone ", typeof(string));
                    //dt.Columns.Add("HQ EXT", typeof(string));
                    //dt.Columns.Add("Duty Station Ext", typeof(string));
                    //dt.Columns.Add("Call Sign", typeof(string));


                    foreach (var item in result.OrderBy(x => x.DutyStation).ThenBy(x => x.FullName))
                    {

                        DataRow dr;
                        dr = dt.NewRow();
                        dr[0] = item.FullName;
                        dr[1] = item.DutyStation;
                        dr[2] = item.FlowStatusGUID == Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3676") ? "Submitted" :
                            item.FlowStatusGUID == Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3671") ? "Pending Staff Verification " :
                            item.FlowStatusGUID == Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3672") ? "Pending Certifying" :
                            item.FlowStatusGUID == Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3674") ? "Pending Finance Approval" :
                            "Closed";
                        dr[3] = item.TotalAmount;
                        dr[4] = _resDet.Where(x => x.InternationalStaffEntitlementGUID == item.InternationalStaffEntitlementGUID
                                                && x.EntitlementTypeGUID == InternationalStaffEntitlementType.DangerPayPerDay).Select(x => x.TotalAmount).Sum();
                        dr[5] = _resDet.Where(x => x.InternationalStaffEntitlementGUID == item.InternationalStaffEntitlementGUID
                                           && x.EntitlementTypeGUID == InternationalStaffEntitlementType.R_RTicket).Select(x => x.TotalAmount).Sum();
                        dr[6] = _resDet.Where(x => x.InternationalStaffEntitlementGUID == item.InternationalStaffEntitlementGUID
                                           && x.EntitlementTypeGUID == InternationalStaffEntitlementType.AddedRecorvery).Select(x => x.TotalAmount).Sum();


                        dr[7] = _resDet.Where(x => x.InternationalStaffEntitlementGUID == item.InternationalStaffEntitlementGUID
                                                && x.EntitlementTypeGUID == InternationalStaffEntitlementType.RentalDeduction).Select(x => x.TotalAmount).Sum();
                        dr[8] = _resDet.Where(x => x.InternationalStaffEntitlementGUID == item.InternationalStaffEntitlementGUID
                                                && x.EntitlementTypeGUID == InternationalStaffEntitlementType.BreakfastDeduction).Select(x => x.TotalAmount).Sum();


                        dr[9] = _resDet.Where(x => x.InternationalStaffEntitlementGUID == item.InternationalStaffEntitlementGUID
                                               && x.EntitlementTypeGUID == InternationalStaffEntitlementType.DeductedRecovery).Select(x => x.TotalAmount).Sum();
                        dr[10] = item.StaffComment;
                        dr[11] = item.PreparedBy;
                        dr[12] = item.CertifiedBy;
                        dr[13] = item.FinanceApprovedBy;
                        dr[14] = item.PaymentMethod;

                        dt.Rows.Add(dr);
                    }
                    workSheet.Cells["B4"].LoadFromDataTable(dt, true);

                    workSheet.Cells["B2"].Value = " " + result.Select(x => x.PeriodName).FirstOrDefault();

                    package.Save();
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);

                string fileName = "International Staff Entitlements" + Guid.NewGuid() + ".xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }

            string success = "No Data Available for this period";
            return Json(new { success = success }, JsonRequestBehavior.AllowGet);

        }
        #endregion







        #region Reports Staff


        [HttpGet]
        public ActionResult StaffPhotoDirectoryReport()
        {
            return View("~/Areas/AHD/Views/Reports/StaffDirectory.cshtml");

        }

        [HttpPost]
        public void GenerateStaffPhotoDirectoryReport(string[] UserGUIDs, string[] DutyStationGUIDs, string[] DepartmentGUIDs)
        {
            Guid OrganizationInstanceGUID = Session[SessionKeys.OrganizationInstanceGUID] != null ? Guid.Parse(Session[SessionKeys.OrganizationInstanceGUID].ToString()) : Guid.Empty;
            string DutyStationGUIDSTR = "";
            string UserGUIDSTR = "";
            string DepartmentGUIDSTR = "";
            string filename = DutyStationGUIDs == null ? "All" : (DutyStationGUIDs.Count() >= 2 ? "Multi" : DbAHD.codeDutyStationsLanguages.Where(x => x.Active && x.LanguageID == LAN && DutyStationGUIDs.Contains(x.DutyStationGUID.ToString())).Select(x => x.DutyStationDescription).FirstOrDefault());
            filename += "-" + (DepartmentGUIDs == null ? "All" : (DepartmentGUIDs.Count() >= 2 ? "Multi" : DbAHD.codeDepartmentsLanguages.Where(x => x.Active && x.LanguageID == LAN && DepartmentGUIDs.Contains(x.DepartmentGUID.ToString())).Select(x => x.DepartmentDescription).FirstOrDefault()));

            DutyStationGUIDSTR = (DutyStationGUIDs == null ? string.Join(",", DropDownList.SyriaDutyStations().Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray()) : string.Join(",", DutyStationGUIDs));
            UserGUIDSTR = (UserGUIDs == null ? string.Join(",", DropDownList.ShuttlePassanger().Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray()) : string.Join(",", UserGUIDs));
            DepartmentGUIDSTR = (DepartmentGUIDs == null ? string.Join(",", DropDownList.Departments().Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray()) : string.Join(",", DepartmentGUIDs));

            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.ZoomMode = ZoomMode.Percent;
            reportViewer.ZoomPercent = 100;
            reportViewer.Width = System.Web.UI.WebControls.Unit.Pixel(1372);
            reportViewer.Height = System.Web.UI.WebControls.Unit.Pixel(800);

            reportViewer.AsyncRendering = true;
            reportViewer.LocalReport.DataSources.Clear();
            AHDDataSet StaffDirectory = new AHDDataSet();


            StaffPhotoDirectoryTableAdapter RP_StaffRenewal_EN = new StaffPhotoDirectoryTableAdapter();
            RP_StaffRenewal_EN.Fill(StaffDirectory.StaffPhotoDirectory, DutyStationGUIDSTR, DepartmentGUIDSTR, UserGUIDSTR, OrganizationInstanceGUID);
            ReportDataSource reportDataSource_0 = new ReportDataSource("StaffPhotoDirectory", StaffDirectory.Tables["StaffPhotoDirectory"]);
            reportViewer.LocalReport.DataSources.Add(reportDataSource_0);



            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Areas/AHD/Rdlc/StaffForms\StaffPhotoDirectory.rdlc";
            //ViewBag.ReportViewer = reportViewer;
            Warning[] warnings;
            string[] streamIds;
            string contentType;
            string encoding;
            string extension;


            //Export the RDLC Report to Byte Array.
            byte[] bytes = reportViewer.LocalReport.Render("PDF", null, out contentType, out encoding, out extension, out streamIds, out warnings);


            this.Response.ClearContent();
            this.Response.ClearHeaders();
            this.Response.Clear();
            this.Response.AddHeader("content-disposition", "attachment; filename= " + filename + "." + extension);
            this.Response.ContentType = "Application/pdf";
            this.Response.BinaryWrite(bytes);
            this.Response.Flush();
            this.Response.End();
        }



        #endregion

        #region Generate vcf file
        public FileResult DownloadFile()
        {
            string FilePath = Server.MapPath("\\Uploads\\AHD\\Contact.vcf");
            // Initialize data (see code-example above):
            string[] StatusGUIDs = { "B9CD375C-A576-4AA4-8AF4-FF3C1C4E3445", "B9CD375C-A576-4AA4-8AF4-FF3C1C4E3611", "B9CD375C-A576-4AA4-8AF4-FF3C1C4E3622" };
            var Result = DbAHD.v_staffCoreDataOverview.Where(x=> StatusGUIDs.Contains(x.StaffStatusGUID.ToString().ToUpper())).Select(x => x).ToList();
            List<Contact> contactArr = new List<Contact>();

            Result.ForEach(x =>
              contactArr.Add(new Contact
              {
                  DisplayName = x.FullName,
                  Person = new Person
                  {

                      Name = new Name
                      {
                          FirstName = x.FirstName,
                          MiddleName = "",
                          LastName = x.SurName,
                          Suffix = "",

                      },

                      //BirthDay = ,
                      //Spouse = "Myriam Malky",
                      //Anniversary = new DateTime(2001, 6, 15)
                  },

                  //Work = new Work
                  //{
                  //    JobTitle = x.JobTitle,
                  //    Company = "UNHCR"
                  //},

                  PhoneNumbers = new PhoneNumber[]
                 {
                        new PhoneNumber
                        {
                            Value = x.OfficialMobileNumber,
                            IsMobile = true
                        },
                        new PhoneNumber
                        {
                            Value ="+963 11 2181"+ x.OfficialExtensionNumber,
                            IsWork = true,
                        }
                 },

                 // EmailAddresses = new string[]
                 //{
                 //       x.EmailAddress
                 //},
                 // AddressHome = new Address
                 // {
                 //     City = x.DutyStation,
                 //     Street = x.PermanentAddressEn,

                 // }

              })
                 );




            const string fileName = "Contact.vcf";

            // Save all contacts to a common VCF file:
            contactArr.SaveVcf(FilePath);


            byte[] fileBytes = System.IO.File.ReadAllBytes(@FilePath);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        #endregion
    }
}