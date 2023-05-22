using AppsPortal.Areas.MRS.RDLC;
using AppsPortal.Areas.MRS.RDLC.MRSDataSetTableAdapters;
using AppsPortal.BaseControllers;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

using Microsoft.Reporting.WebForms;
using MRS_DAL.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;


namespace AppsPortal.Areas.MRS.Controllers
{
    public class ReportsController : MRSBaseController
    {
        #region Note Verbale
        public ActionResult NVReport(Guid PK)
        {
            var notVerbale_EN = DbMRS.RP_NoteVerbale(PK, "EN").FirstOrDefault();
            var notVerbaleStaff_EN = DbMRS.RP_NoteVerbaleStaff(PK, "EN").ToList();
            var notVerbaleVehicle_EN = DbMRS.RP_NoteVerbaleVehicle(PK, "EN").FirstOrDefault();
            var notVerbaleOrg_EN = DbMRS.RP_NoteVerbaleOrg(PK, "EN").FirstOrDefault();

            var notVerbale_AR = DbMRS.RP_NoteVerbale(PK, "AR").FirstOrDefault();
            var notVerbaleStaff_AR = DbMRS.RP_NoteVerbaleStaff(PK, "AR").ToList();
            var notVerbaleVehicle_AR = DbMRS.RP_NoteVerbaleVehicle(PK, "AR").FirstOrDefault();
            var notVerbaleOrg_AR = DbMRS.RP_NoteVerbaleOrg(PK, "AR").FirstOrDefault();


            var Gov = DbMRS.codeDutyStationsLanguages.Where(x => x.Active && x.DutyStationGUID == notVerbale_AR.DutyStationGUID).ToList();

            string templatePath = "~/Areas/MRS/Templates/Template_NV.docx";
            var serverPath = Server.MapPath(templatePath);

            string destinationPath = "~/Areas/MRS/Temp/" + PK + ".docx";
            var destinationServerPath = Server.MapPath(destinationPath);

            System.IO.File.Copy(serverPath, destinationServerPath, true);

            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(destinationServerPath, true))
            {
                MainDocumentPart mainDocumentPart = wordDoc.MainDocumentPart;
                var SDTRun = mainDocumentPart.Document.Descendants<SdtRun>();

                //EN Not Verbale
                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("RefranceNV_EN"))
                    .First<SdtRun>().Descendants<Text>().First().Text = notVerbale_EN.Reference.Trim()+" /"+ notVerbale_EN.NoteVerbaleDate.Value.Year;
                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("NoteVerbaleDate_EN"))
                    .First<SdtRun>().Descendants<Text>().First().Text = notVerbale_EN.NoteVerbaleDate.Value.ToString("dd MMMM yyyy");

                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("LocationNV_EN"))
                     .First<SdtRun>().Descendants<Text>().First().Text = notVerbale_EN.Location.Trim();
                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("GevernerateNV_EN"))
                   .First<SdtRun>().Descendants<Text>().First().Text = notVerbale_EN.Gevernerate.Trim();
                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("VisitDateNV_EN"))
                   .First<SdtRun>().Descendants<Text>().First().Text = notVerbale_EN.VisitDate.ToString("dd MMMM yyyy");
                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("VisitPurposeNV_EN"))
                   .First<SdtRun>().Descendants<Text>().First().Text = notVerbale_EN.VisitPurpose_EN.Trim();
                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("NoteVerbaleDateNV_EN"))
                   .First<SdtRun>().Descendants<Text>().First().Text = notVerbale_EN.NoteVerbaleDate.Value.ToString("dd MMMM yyyy");
                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("Organization_EN"))
                .First<SdtRun>().Descendants<Text>().First().Text = notVerbaleOrg_EN.Trim();
                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("Governorate_EN"))
                .First<SdtRun>().Descendants<Text>().First().Text = Gov.Count() !=0?Gov.Where(x=>x.LanguageID=="EN").FirstOrDefault().DutyStationDescription: "Rural Damascus";


                //AR Not Verbale

                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("LocationNV_AR"))
                     .First<SdtRun>().Descendants<Text>().First().Text = notVerbale_AR.Location.Trim();
                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("GevernerateNV_AR"))
                   .First<SdtRun>().Descendants<Text>().First().Text = notVerbale_AR.Gevernerate.Trim();
                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("VisitDateNV_AR"))
                   .First<SdtRun>().Descendants<Text>().First().Text = notVerbale_AR.VisitDate.ToString("dd MMMM yyyy", new CultureInfo("ar-SY"));
                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("VisitPurposeNV_AR"))
                   .First<SdtRun>().Descendants<Text>().First().Text = notVerbale_AR.VisitPurpose_AR.Trim();
                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("NoteVerbaleDateNV_AR"))
                   .First<SdtRun>().Descendants<Text>().First().Text = notVerbale_AR.NoteVerbaleDate.Value.ToString("dd MMMM yyyy", new CultureInfo("ar-SY"));
                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("Organization_AR"))
               .First<SdtRun>().Descendants<Text>().First().Text = notVerbaleOrg_AR.Trim();
                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("Governorate_AR"))
               .First<SdtRun>().Descendants<Text>().First().Text = Gov.Count() != 0 ? Gov.Where(x => x.LanguageID == "AR").FirstOrDefault().DutyStationDescription : "ريف دمشق";


                //EN Not Verbale Vehicles
                string DriversDetailNVV_EN =  notVerbaleVehicle_EN.DriversDetail.Remove(notVerbaleVehicle_EN.DriversDetail.Length - 1);
                string VehicleNumber_EN = "(" + notVerbaleVehicle_EN.VehicleNumber.Remove(notVerbaleVehicle_EN.VehicleNumber.Length - 2) + ")";
                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("VehicleNumberNVV_EN"))
                    .First<SdtRun>().Descendants<Text>().First().Text = VehicleNumber_EN;
                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("DriversDetailNVV_EN"))
                    .First<SdtRun>().Descendants<Text>().First().Text = DriversDetailNVV_EN.Substring(4, DriversDetailNVV_EN.Length - 4);



                //AR Not Verbale Vehicles
                string DriversDetailNVV_AR = notVerbaleVehicle_AR.DriversDetail.Remove(notVerbaleVehicle_AR.DriversDetail.Length - 1) ;
                string VehicleNumber_AR = "(" + notVerbaleVehicle_AR.VehicleNumber.Remove(notVerbaleVehicle_AR.VehicleNumber.Length - 2) + ")";
                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("VehicleNumberNVV_AR"))
                    .First<SdtRun>().Descendants<Text>().First().Text = VehicleNumber_AR.Replace(",", "،");
                SDTRun.Where(run => run.SdtProperties.GetFirstChild<Tag>().Val.Value.Contains("DriversDetailNVV_AR"))
                    .First<SdtRun>().Descendants<Text>().First().Text = DriversDetailNVV_AR.Replace(",", "،").Substring(2, DriversDetailNVV_AR.Length - 2);


                AddStaffList(notVerbaleStaff_AR, notVerbaleStaff_AR.Count, mainDocumentPart, "AR");
                AddStaffList(notVerbaleStaff_EN, notVerbaleStaff_EN.Count, mainDocumentPart, "EN");

                mainDocumentPart.Document.Save();
            }

            byte[] filedata = System.IO.File.ReadAllBytes(destinationServerPath);
            string contentType = MimeMapping.GetMimeMapping(destinationServerPath);

            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = notVerbale_EN.Reference +"-" +notVerbale_EN.VisitDate.ToString("dd MMMM yyyy")+".docx",
                Inline = true,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());

            return File(filedata, contentType);
        }

        private void AddStaffList(List<RP_NoteVerbaleStaff_Result> notVerbaleStaff, int count, MainDocumentPart mainDocumentPart,string Lan)
        {
            List<SdtBlock> sdtList = mainDocumentPart.Document.Descendants<SdtBlock>().ToList();
            SdtBlock sdtA = null;
            foreach (SdtBlock sdt in sdtList)
            {
                if (sdt.SdtProperties.GetFirstChild<Tag>() != null) {
                    if (sdt.SdtProperties.GetFirstChild<Tag>().Val.Value == "lstStaff" + Lan)
                    {
                        sdtA = sdt;
                        break;
                    }
                }
            }
            OpenXmlElement sdtc = sdtA.GetFirstChild<SdtContentBlock>();
            OpenXmlElementList elements = sdtA.ChildElements;
            int index = -1;
            foreach (OpenXmlElement elem in elements)
            {

                string innerxml = elem.InnerText;
                if (innerxml.Length > 0)
                {
                    foreach (var chl in elem)
                    {
                        index++;
                        if (index  >= count)
                        {
                            chl.RemoveAllChildren(); 
                        }
                        else
                        {
                            foreach (var chlP in chl)
                            {
                                foreach (var chlR in chlP)
                                {

                                    foreach (var chlT in chlR)
                                    {
                                        Text sdtT = chlT.GetFirstChild<Text>();
                                        if (sdtT != null)
                                        {
                                            string Prefix = notVerbaleStaff[index].Gender == "M" ? Lan == "EN" ? "Mr. " : "السيد " : Lan == "EN" ? "Ms. " : "السيدة ";
                                            string NatStrLan = Lan == "EN" ? "national of " : "من الجنسية ";
                                            sdtT.Text = Prefix+ notVerbaleStaff[index].Fullname+(Lan == "EN" ? ", " :"، ")+ notVerbaleStaff[index].JobTitleDescription+ (Lan == "EN" ? ", " :"، ") + NatStrLan + notVerbaleStaff[index].Nationality+ (Lan == "EN" ? ";" : "،");
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                       

                    }
                }

            }
        }


        //public void NVReport(Guid PK)
        //{
        //    ReportViewer reportViewer = new ReportViewer();
        //    reportViewer.ProcessingMode = ProcessingMode.Local;
        //    reportViewer.ZoomMode = ZoomMode.Percent;
        //    reportViewer.ZoomPercent = 100;
        //    reportViewer.Width = System.Web.UI.WebControls.Unit.Pixel(1372);
        //    reportViewer.Height = System.Web.UI.WebControls.Unit.Pixel(800);

        //    reportViewer.AsyncRendering = true;
        //    reportViewer.LocalReport.DataSources.Clear();
        //    MRSDataSet amsDataSet_EN = new MRSDataSet();
        //    MRSDataSet amsDataSet_AR = new MRSDataSet();

        //    RP_NoteVerbaleTableAdapter RP_NoteVerbale_EN = new RP_NoteVerbaleTableAdapter();
        //    RP_NoteVerbale_EN.Fill(amsDataSet_EN.RP_NoteVerbale, PK, "EN");
        //    ReportDataSource reportDataSource_EN = new ReportDataSource("RP_NoteVerbale_EN", amsDataSet_EN.Tables["RP_NoteVerbale"]);
        //    reportViewer.LocalReport.DataSources.Add(reportDataSource_EN);

        //    RP_NoteVerbaleTableAdapter RP_NoteVerbale_AR = new RP_NoteVerbaleTableAdapter();

        //    RP_NoteVerbale_EN.Fill(amsDataSet_AR.RP_NoteVerbale, PK, "AR");
        //    ReportDataSource reportDataSource_AR = new ReportDataSource("RP_NoteVerbale_AR", amsDataSet_AR.Tables["RP_NoteVerbale"]);
        //    reportViewer.LocalReport.DataSources.Add(reportDataSource_AR);


        //    RP_NoteVerbaleStaffTableAdapter RP_NoteVerbaleStaff_EN = new RP_NoteVerbaleStaffTableAdapter();
        //    RP_NoteVerbaleStaff_EN.Fill(amsDataSet_EN.RP_NoteVerbaleStaff, PK, "EN");
        //    reportDataSource_EN = new ReportDataSource("RP_NoteVerbaleStaff_EN", amsDataSet_EN.Tables["RP_NoteVerbaleStaff"]);
        //    reportViewer.LocalReport.DataSources.Add(reportDataSource_EN);

        //    RP_NoteVerbaleStaffTableAdapter RP_NoteVerbaleStaff_AR = new RP_NoteVerbaleStaffTableAdapter();
        //    RP_NoteVerbaleStaff_AR.Fill(amsDataSet_AR.RP_NoteVerbaleStaff, PK, "AR");
        //    reportDataSource_AR = new ReportDataSource("RP_NoteVerbaleStaff_AR", amsDataSet_AR.Tables["RP_NoteVerbaleStaff"]);
        //    reportViewer.LocalReport.DataSources.Add(reportDataSource_AR);

        //    RP_NoteVerbaleVehicleTableAdapter RP_NoteVerbaleVehicle_EN = new RP_NoteVerbaleVehicleTableAdapter();
        //    RP_NoteVerbaleVehicle_EN.Fill(amsDataSet_EN.RP_NoteVerbaleVehicle, PK, "EN");
        //    reportDataSource_EN = new ReportDataSource("RP_NoteVerbaleVehicle_EN", amsDataSet_EN.Tables["RP_NoteVerbaleVehicle"]);
        //    reportViewer.LocalReport.DataSources.Add(reportDataSource_EN);

        //    RP_NoteVerbaleVehicleTableAdapter RP_NoteVerbaleVehicle_AR = new RP_NoteVerbaleVehicleTableAdapter();
        //    RP_NoteVerbaleVehicle_AR.Fill(amsDataSet_AR.RP_NoteVerbaleVehicle, PK, "AR");
        //    reportDataSource_AR = new ReportDataSource("RP_NoteVerbaleVehicle_AR", amsDataSet_AR.Tables["RP_NoteVerbaleVehicle"]);
        //    reportViewer.LocalReport.DataSources.Add(reportDataSource_AR);

        //    RP_NoteVerbaleOrganizationTableAdapter RP_NoteVerbaleOrganization_EN = new RP_NoteVerbaleOrganizationTableAdapter();
        //    RP_NoteVerbaleOrganization_EN.Fill(amsDataSet_EN.RP_NoteVerbaleOrganization, PK, "EN");
        //    reportDataSource_EN = new ReportDataSource("RP_NoteVerbaleOrganization_EN", amsDataSet_EN.Tables["RP_NoteVerbaleOrganization"]);
        //    reportViewer.LocalReport.DataSources.Add(reportDataSource_EN);

        //    RP_NoteVerbaleOrganizationTableAdapter RP_NoteVerbaleOrganization_AR = new RP_NoteVerbaleOrganizationTableAdapter();
        //    RP_NoteVerbaleOrganization_AR.Fill(amsDataSet_AR.RP_NoteVerbaleOrganization, PK, "AR");
        //    reportDataSource_AR = new ReportDataSource("RP_NoteVerbaleOrganization_AR", amsDataSet_AR.Tables["RP_NoteVerbaleOrganization"]);
        //    reportViewer.LocalReport.DataSources.Add(reportDataSource_AR);


        //    reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Areas/MRS/Rdlc\ReportNV.rdlc";
        //    ViewBag.ReportViewer = reportViewer;

        //    Warning[] warnings;
        //    string[] streamIds;
        //    string contentType;
        //    string encoding;
        //    string extension;

        //    //Export the RDLC Report to Byte Array.
        //    byte[] bytes = reportViewer.LocalReport.Render("WORD", null, out contentType, out encoding, out extension, out streamIds, out warnings);

        //    //Download the RDLC Report in Word, Excel, PDF and Image formats.
        //    Response.Clear();
        //    Response.Buffer = true;
        //    Response.Charset = "";
        //    Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //    Response.ContentType = contentType;
        //    Response.AppendHeader("Content-Disposition", "attachment; filename="+ PK + "." + extension);
        //    Response.BinaryWrite(bytes);
        //    Response.Flush();
        //    Response.End();
        //}
        #endregion
    }
}