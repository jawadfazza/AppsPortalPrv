using AppsPortal.Areas.IDC.RDLC;
using AppsPortal.Areas.IDC.RDLC.IDCDataSetTableAdapters;
using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AutoMapper;
using IDC_DAL.Model;
using LinqKit;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.IDC.Controllers
{
    public class CardPrintsController : IDCBaseController
    {
        #region Card Issueds

        [Route("IDC/CardPrints/")]
        public ActionResult CardPrintsIndex()
        {
            if (!CMS.HasAction(Permissions.CardPrint.Access, Apps.IDC))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/IDC/Views/CardPrints/Index.cshtml");
        }

        [Route("IDC/CardPrintsDataTable/")]
        public JsonResult CardPrintsDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<CardPrintsDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<CardPrintsDataTableModel>(DataTable.Filters);
            }
            bool filterOption = false;
            //Access is authorized by Access Action

            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.CardIssued.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();

            IQueryable<CardPrintsDataTableModel> All =null;

            foreach (var filter in DataTable.Filters.FilterRules)
            {
                if (filter.Field == "CaseNumber")
                {
                     All = (from a in DbIDC.dataCardIndividualInfo.AsExpandable()
                               join b in DbIDC.dataCardIssued on a.CardIndividualInfoGUID equals b.CardIndividualInfoGUID
                               where b.Approved.Value && b.PrintedBy!=null && AuthorizedList.Contains("e156c022-ec72-4a5a-be09-163bd85c68ef," + b.DutyStationGUID.ToString())
                               select new CardPrintsDataTableModel
                               {
                                   CardIssuedGUID = b.CardIssuedGUID,
                                   CaseNumber = a.CaseNumber,
                                   ArrivalDate = a.ArrivalDate,
                                   Category = a.Category,
                                   DateOfBrith = a.DateOfBrith,
                                   FullName_AR = a.FullName_AR,
                                   FullName_EN = a.FullName_EN,
                                   IndividualID = a.IndividualID,
                                   Sex = a.Sex,
                                   CreateBy = b.CreateBy,
                                   CreateDate = b.CreateDate,
                                   PrintBy = b.PrintedBy,
                                   PrintDate = b.PrintedDate,
                                   Active = b.Active,
                                   IssueCode = DbIDC.codeCardIssueReason.Where(y => y.IssueCode == b.IssueCode).FirstOrDefault().IssueDescription,
                                   dataCardIssuedRowVersion = b.dataCardIssuedRowVersion
                               }).Where(Predicate);
                    filterOption = true;
                }
            }
            if (!filterOption)
            {
                All = (from a in DbIDC.dataCardIndividualInfo.AsExpandable()
                       join b in DbIDC.dataCardIssued on a.CardIndividualInfoGUID equals b.CardIndividualInfoGUID
                       where b.Approved.Value && b.PrintedBy == null && AuthorizedList.Contains("e156c022-ec72-4a5a-be09-163bd85c68ef," + b.DutyStationGUID.ToString())
                       select new CardPrintsDataTableModel
                       {
                           CardIssuedGUID = b.CardIssuedGUID,
                           CaseNumber = a.CaseNumber,
                           ArrivalDate = a.ArrivalDate,
                           Category = a.Category,
                           DateOfBrith = a.DateOfBrith,
                           FullName_AR = a.FullName_AR,
                           FullName_EN = a.FullName_EN,
                           IndividualID = a.IndividualID,
                           Sex = a.Sex,
                           CreateBy = b.CreateBy,
                           CreateDate = b.CreateDate,
                           PrintBy = b.PrintedBy,
                           PrintDate = b.PrintedDate,
                           IssueCode = DbIDC.codeCardIssueReason.Where(y => y.IssueCode == b.IssueCode).FirstOrDefault().IssueDescription,
                           Active = b.Active,
                           dataCardIssuedRowVersion = b.dataCardIssuedRowVersion
                       }).Where(Predicate);
            }
         

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<CardPrintsDataTableModel> Result = Mapper.Map<List<CardPrintsDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult CardPrintsDataTableDelete(List<dataCardIssued> models)
        {
            if (!CMS.HasAction(Permissions.CardIssued.Delete, Apps.IDC))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataCardIssued> DeletedCardPrints = DeleteCardPrints(models);

            try
            {
                DbIDC.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbIDC.PartialDeleteMessage(DeletedCardPrints, models, DataTableNames.CardPrintsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbIDC.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult CardPrintsDataTableRestore(List<dataCardIssued> models)
        {
            if (!CMS.HasAction(Permissions.CardIssued.Restore, Apps.IDC))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataCardIssued> RestoredCardPrints = RestoreCardPrints(models);

            try
            {
                DbIDC.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbIDC.PartialRestoreMessage(RestoredCardPrints, models, DataTableNames.CardPrintsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbIDC.ErrorMessage(ex.Message));
            }
        }

        private List<dataCardIssued> DeleteCardPrints(List<dataCardIssued> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataCardIssued> DeletedCardPrints = new List<dataCardIssued>();

            string query = DbIDC.QueryBuilder(models, Permissions.CardIssued.DeleteGuid, SubmitTypes.Delete, "");

            var CardPrints = DbIDC.Database.SqlQuery<dataCardIssued>(query).ToList();

            foreach (var CardIssued in CardPrints)
            {
                DeletedCardPrints.Add(DbIDC.Delete(CardIssued, ExecutionTime, Permissions.CardIndividualInfo.DeleteGuid, DbCMS));
            }

            return DeletedCardPrints;
        }

        private List<dataCardIssued> RestoreCardPrints(List<dataCardIssued> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataCardIssued> RestoredCardPrints = new List<dataCardIssued>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbIDC.QueryBuilder(models, Permissions.CardIssued.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var CardPrints = DbIDC.Database.SqlQuery<dataCardIssued>(query).ToList();
            foreach (var CardIssued in CardPrints)
            {
                if (!ActiveCardIssued(CardIssued))
                {
                    RestoredCardPrints.Add(DbIDC.Restore(CardIssued, Permissions.CardIssued.DeleteGuid, Permissions.CardIssued.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredCardPrints;
        }
        private bool ActiveCardIssued(dataCardIssued model)
        {
            int CardIssuedID = DbIDC.dataCardIssued
                                  .Where(x =>
                                              x.CardIndividualInfoGUID == model.CardIndividualInfoGUID &&
                                              x.IssueCode == model.IssueCode &&
                                              x.IssueDate == model.IssueDate &&
                                              x.CardIssuedGUID != model.CardIssuedGUID &&
                                              x.Active).Count();
            if (CardIssuedID > 0)
            {
                ModelState.AddModelError("CardIssuedID", "Card Issued with same Reason already exists"); //From resource ?????? Amer  
            }

            return (CardIssuedID > 0);
        }
        #endregion

        #region Print Card Request
        public void PrintCardRequest(Guid PK)
        {
            if (!CMS.HasAction(Permissions.CardPrint.Print, Apps.IDC))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var Card = DbIDC.dataCardIssued.Where(x => x.CardIssuedGUID == PK).FirstOrDefault();
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.ZoomMode = ZoomMode.Percent;
            reportViewer.ZoomPercent = 90;
            reportViewer.Width = System.Web.UI.WebControls.Unit.Pixel(850);
            reportViewer.Height = System.Web.UI.WebControls.Unit.Pixel(600);
            reportViewer.AsyncRendering = true;
            reportViewer.LocalReport.DataSources.Clear();

            CardDataTableAdapter appointmentsSlipTableAdapter = new CardDataTableAdapter();
            IDCDataSet IDCDataSet = new IDCDataSet();
            appointmentsSlipTableAdapter.Fill(IDCDataSet.CardData, PK);
            ReportDataSource reportDataSource = new ReportDataSource("CardData", IDCDataSet.Tables["CardData"]);
            reportViewer.LocalReport.DataSources.Add(reportDataSource);
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Areas/IDC/Rdlc\IndividualCard_"+ Card.dataCardIndividualInfo.Category + ".rdlc";

            Warning[] warnings;
            string[] streamIds;
            string contentType;
            string encoding;
            string extension;

            //Export the RDLC Report to Byte Array.
            byte[] bytes = reportViewer.LocalReport.Render("PDF", null, out contentType, out encoding, out extension, out streamIds, out warnings);

            //Update the card
            if (Card.PrintedBy == null)
            {
                Card.PrintedBy = CMS.GetFullName(UserGUID, LAN);
                Card.PrintedDate = DateTime.Now;
                Card.Photo = null;
                Card.Barcode = null;
                DbIDC.dataCardIssued.Where(x => x.CardIndividualInfoGUID == Card.CardIndividualInfoGUID && x.CardIssuedGUID != Card.CardIssuedGUID).ToList().ForEach(x => x.Valid = false);
                DbIDC.SaveChanges();
                string EmailAddress = CMS.GetCurrentUserEmail(DbCMS.userPersonalDetailsLanguage.Where(x => (x.FirstName + " " + x.Surname) == Card.CreateBy).FirstOrDefault().UserGUID);
                string SecurityUserID = DbPRG.configSecurityUser.Where(x => x.SecurityUserEmail == EmailAddress).FirstOrDefault().SecurityUserID;
                //Update Progres
                var doc = (from d in DbPRG.dataDocument where d.IndividualGUID == Card.CardIndividualInfoGUID && d.DocumentType == "UNID" && d.DocumentStatusCode == "A" select d).FirstOrDefault();
                if (doc == null)
                {
                    DbPRG.rspUpdateDocument(Card.dataCardIndividualInfo.IndividualID, SecurityUserID, "IDREF00", Card.Sequance, Card.ExpirtionDate.Value.ToString("M/d/yyyy"));
                }
                else
                {
                    if (doc.DocumentDateIssued.Value.ToShortDateString() != DateTime.Now.ToShortDateString())
                    {
                        DbPRG.rspUpdateDocument(Card.dataCardIndividualInfo.IndividualID, SecurityUserID, "IDREF00", Card.Sequance, Card.ExpirtionDate.Value.ToString("M/d/yyyy"));

                        doc.DocumentStatusCode = "I";
                        doc.DocumentComments = Card.codeCardIssueReason.IssueDescription;
                        DbPRG.SaveChanges();
                    }
                }
            }

            this.Response.ClearContent();
            this.Response.ClearHeaders();
            this.Response.Clear();
            this.Response.AddHeader("content-disposition", "inline;filename=" + PK + "PDF");  //Filename example (1.pdf)
            this.Response.ContentType = "Application/pdf";
            this.Response.BinaryWrite(bytes);
            this.Response.Flush();
            this.Response.End();

        }
        #endregion
    }
}