using AppsPortal.BaseControllers;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.GTP.Controllers
{
    public class ReportsController : GTPBaseController
    {
        // GET: GTP/Reports
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DownloadSuccessfulApplicantsReport()
        {
            string sourceFile = Server.MapPath("~/Areas/GTP/ReportsTemplates/Record of successful applicants.xlsx");

            string receiptFileName = "Record of successful applicants" + DateTime.Now.ToBinary() + ".xlsx";

            string DisFolder = Server.MapPath("~/Areas/GTP/GeneratedReports/" + receiptFileName);

            System.IO.File.Copy(sourceFile, DisFolder);

            var reportData = (from a in DbGTP.dataGTPApplications
                              join b in DbGTP.codeGTPCategories on a.GTPCategoryGUID equals b.GTPCategoryGUID
                              join c in DbGTP.userPersonalDetailsLanguages on a.UserGUID equals c.UserGUID
                              join d in DbGTP.userServiceHistories on a.UserGUID equals d.UserGUID
                              where c.LanguageID == "EN" && c.Active && d.Active && (a.IsEligible.HasValue && a.IsEligible.Value == true)
                              select new
                              {
                                  c.Surname,
                                  c.FirstName,
                                  d.EmailAddress,
                                  a.ReviewedOn,
                                  b.GTPCategoryDescription,
                                  a.GTPApplicationEligibleAs,
                                  a.GTPApplicationExpiryDate

                              }).ToList();
            using (var package = new ExcelPackage(new FileInfo(DisFolder)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                int i = 3;
                foreach (var record in reportData)
                {
                    worksheet.Cells["A" + i].Value = record.Surname;
                    worksheet.Cells["B" + i].Value = record.FirstName;
                    worksheet.Cells["C" + i].Value = record.EmailAddress;
                    worksheet.Cells["D" + i].Value = record.ReviewedOn.Value.ToShortDateString();
                    worksheet.Cells["E" + i].Value = record.GTPCategoryDescription;
                    worksheet.Cells["F" + i].Value = record.GTPApplicationEligibleAs.Value.ToShortDateString();
                    worksheet.Cells["G" + i].Value = record.GTPApplicationExpiryDate.Value.ToShortDateString();
                    i++;
                }

                worksheet.Column(1).AutoFit();
                worksheet.Column(2).AutoFit();
                worksheet.Column(3).AutoFit();
                worksheet.Column(4).AutoFit();
                worksheet.Column(5).AutoFit();
                worksheet.Column(6).AutoFit();
                worksheet.Column(7).AutoFit();

                package.Save();
            }
            byte[] fileBytes = System.IO.File.ReadAllBytes(@"" + DisFolder + "");
            return File(fileBytes, "application/octet-stream", "Record of successful applicants.xlsx");
        }
    }
}