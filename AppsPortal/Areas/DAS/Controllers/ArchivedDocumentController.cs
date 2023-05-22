using AppsPortal.BaseControllers;
using AppsPortal.Data;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.Services;
using AppsPortal.ViewModels;
using AutoMapper;
using DAS_DAL.Model;
using DAS_DAL.ViewModels;
using FineUploader;
using IronPdf;
using iTextSharp.text;
using iTextSharp.text.pdf;
using LinqKit;
using Microsoft.Ajax.Utilities;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using RES_Repo.Globalization;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Image = System.Drawing.Image;

namespace AppsPortal.Areas.DAS.Controllers
{
    public class ArchivedDocumentController : DASBaseController
    {
        private Images imagesServices = new Images();
        private ConvertToPDF MyDocServices = new ConvertToPDF();
        #region Archive Templates Documents

        [Route("DAS/ArchivedDocument/DocumentWizardCreate/")]
        public ActionResult ArchiveTemplateDocumentWizardCreate()
        {
            if (!CMS.HasAction(Permissions.RefugeeScannedDocument.Create, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            ArchiveTemplateDocumentUpdateModel model = new ArchiveTemplateDocumentUpdateModel();
            model.TotalImages = 0;
            model.ArchiveTemplateDocumentGUID = Guid.NewGuid();

            return View("~/Areas/DAS/Views/ArchivedDocument/TemplateWizardDocument.cshtml", model);
        }

        public ActionResult ArchiveTemplateDocumentIndex(Guid PK)
        {
            if (!CMS.HasAction(Permissions.RefugeeScannedDocument.Access, Apps.DAS))
            {
                return Json(DbDAS.PermissionError());
            }
            var _curr = DbDAS.dataArchiveTemplateDocument.Where(x => x.ArchiveTemplateDocumentGUID == PK).FirstOrDefault();

            ArchiveTemplateDocumentUpdateModel model = Mapper.Map(_curr, new ArchiveTemplateDocumentUpdateModel());
            model.TotalImages = DbDAS.dataArchiveTemplateDocumentImage.Where(x => x.ArchiveTemplateDocumentGUID == PK && x.Active).Count();
            model.CabinetGUID = _curr.DocumentCabinetShelfGUID != null ? DbDAS.codeDASDocumentCabinetShelf.Where(x => x.DocumentCabinetShelfGUID == model.DocumentCabinetShelfGUID).FirstOrDefault().DocumentCabinetGUID : null;
            model.CreatedBy = model.CreateByGUID!=null? CMS.GetFullName(model.CreateByGUID.Value, LAN):"";
            model.UpdateBy = model.UpdateByGUID != null ? CMS.GetFullName(model.UpdateByGUID.Value, LAN) : "";
            var verfied = _curr.dataArchiveTemplateFlowVerification.Where(x => x.IsLastAction == true).FirstOrDefault();
            if (verfied != null)
            {
                model.VerifiedUser = verfied.CreateByGUID != null ? CMS.GetFullName(verfied.CreateByGUID.Value, LAN) : "";
                model.VerifiedDatetime = verfied.CreateDate;
            }
            model.OrganizationInstanceGUID =model.OrganizationInstanceGUID;
            model.DutyStationGUID = model.DutyStationGUID;
            return View("~/Areas/DAS/Views/ArchivedDocument/TemplateDocuments.cshtml", model);
        }

        [Route("DAS/ArchivedDocument/")]
        public ActionResult ArchiveTemplateDocumentIndex()
        {
            if (!CMS.HasAction(Permissions.RefugeeScannedDocument.Access, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/DAS/Views/ArchivedDocument/Index.cshtml");
        }

        [Route("DAS/ArchiveTemplateDocumentDataTable/")]
        public JsonResult ArchiveTemplateDocumentDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<ArchiveTemplateDocumentDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<ArchiveTemplateDocumentDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.RefugeeScannedDocument.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix
             var All = (
                from a in DbDAS.dataArchiveTemplateDocument.Where(x=>AuthorizedList.Contains(x.OrganizationInstanceGUID+","+x.DutyStationGUID)).AsExpandable()
                join b in DbDAS.codeDASTemplateType.Where(x => x.Active) on a.TemplateTypeGUID equals b.TemplateTypeGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()
                join c in DbDAS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on R1.ReferenceLinkTypeGUID equals c.ValueGUID into LJ2
                from R2 in LJ2.DefaultIfEmpty()
                join d in DbDAS.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.CreateByGUID equals d.UserGUID into LJ3
                from R3 in LJ3.DefaultIfEmpty()
                join e in DbDAS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN && x.codeTablesValues.TableGUID == LookupTables.DASDocumentVerificaitonStatus) on a.LastVerificationStatusGUID equals e.ValueGUID into LJ4
                from R4 in LJ4.DefaultIfEmpty()
                join f in DbDAS.codeDASDocumentCabinetShelf.Where(x => x.Active) on a.DocumentCabinetShelfGUID equals f.DocumentCabinetShelfGUID into LJ5
                from R5 in LJ5.DefaultIfEmpty()

                select new ArchiveTemplateDocumentDataTableModel
                {
                    ArchiveTemplateDocumentGUID = a.ArchiveTemplateDocumentGUID.ToString(),
                    TemplateTypeGUID = a.TemplateTypeGUID.ToString(),
                    TemplateTypeDocumentGUID = a.TemplateTypeDocumentGUID.ToString(),
                    FileReferenceGUID = a.FileReferenceGUID.ToString(),
                    FileReferenceTypeGUID = a.FileReferenceTypeGUID.ToString(),
                    DocumentName = a.DocumentName,
                    FileReferenceName = a.FileReferenceName,
                    CreateByGUID = a.CreateByGUID.ToString(),
                    PhysicalLocation = R5.codeDASDocumentCabinet.DocumentCabinetName + " " + R5.ShelfNumber + " ",
                    DocumentCabinetShelfGUID = a.DocumentCabinetShelfGUID.ToString(),
                    Active = a.Active,
                    TemplateName = R1.TemplateName,
                    ArchiveTemplateDocumentCodeNumber = a.ArchiveTemplateDocumentCodeNumber,
                    Status = R4.ValueDescription,
                    LastVerificationStatusGUID = R4.ValueGUID.ToString(),
                    //ReferenceLinkType = R1.ValueDescription,
                    dataArchiveTemplateDocumentRowVersion = a.dataArchiveTemplateDocumentRowVersion,
                    CreateBy = R3.FirstName + " " + R3.Surname,
                    CreateDate = a.CreateDate,
                    FileReferenceTypeName = a.FileReferenceTypeName,
                    LastCustodianTypeName=a.LastCustodianTypeName,
                    UpdateDate=a.UpdateDate,
                    
                    
                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<ArchiveTemplateDocumentDataTableModel> Result = Mapper.Map<List<ArchiveTemplateDocumentDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        //[Route("DAS/ArchivedDocument/Create/")]
        public ActionResult ArchiveTemplateDocumentCreate()
        {
            if (!CMS.HasAction(Permissions.RefugeeScannedDocument.Create, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            ArchiveTemplateDocumentUpdateModel model = new ArchiveTemplateDocumentUpdateModel { ArchiveTemplateDocumentGUID = Guid.Empty };

            var staffCore = DbDAS.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            model.OrganizationInstanceGUID = staffCore.OrganizationInstanceGUID.Value;
            model.DutyStationGUID = staffCore.DutyStationGUID;

            return PartialView("~/Areas/DAS/Views/ArchivedDocument/_ArchiveDocumentForm.cshtml",
               model);


        }

        [Route("DAS/ArchivedDocument/Update/{PK}")]
        public ActionResult ArchiveTemplateDocumentUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.RefugeeScannedDocument.Access, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var model = (from a in DbDAS.dataArchiveTemplateDocument.WherePK(PK)

                         select new ArchiveTemplateDocumentUpdateModel
                         {
                             ArchiveTemplateDocumentGUID = a.ArchiveTemplateDocumentGUID,
                             //TemplateName = a.TemplateName,
                             dataArchiveTemplateDocumentRowVersion = a.dataArchiveTemplateDocumentRowVersion,
                             TemplateTypeGUID = a.TemplateTypeGUID,
                             TemplateTypeDocumentGUID = a.TemplateTypeDocumentGUID,
                             FileReferenceGUID = a.FileReferenceGUID,
                             FileReferenceTypeGUID = a.FileReferenceTypeGUID,
                             FileReferenceTypeName = a.FileReferenceTypeName,
                             FileReferenceName = a.FileReferenceName,
                             DocumentName = a.DocumentName,
                             DocumentCabinetShelfGUID = a.DocumentCabinetShelfGUID,
                             LastVerificationStatusGUID = a.LastVerificationStatusGUID,
                             OrganizationInstanceGUID=a.OrganizationInstanceGUID.Value,
                             DutyStationGUID=a.DutyStationGUID,
                             //FileReferenceGUID = a.FileReferenceGUID,

                             ArchiveTemplateDocumentCodeNumber = a.ArchiveTemplateDocumentCodeNumber,
                             Active = a.Active,

                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("ArchivedDocument", "ArchivedDocument", new { Area = "DAS" }));

            return PartialView("~/Areas/DAS/Views/ArchivedDocument/_ArchiveDocumentForm.cshtml",
              model);

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ArchiveTemplateDocumentCreate(ArchiveTemplateDocumentUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.RefugeeScannedDocument.Create, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveArchiveTemplateDocument(model))
                return PartialView("~/Areas/DAS/Views/ArchivedDocument/_ArchiveDocumentForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            dataArchiveTemplateDocument ArchiveTemplate = Mapper.Map(model, new dataArchiveTemplateDocument());
            ArchiveTemplate.ArchiveTemplateDocumentGUID = EntityPK;
            ArchiveTemplate.FileReferenceGUID = model.FileReferenceGUID;
            var _template = DbDAS.codeDASTemplateTypeDocument.Where(x => x.TemplateTypeDocumentGUID == model.TemplateTypeDocumentGUID).FirstOrDefault();
            string _Name = "";
            string _type = "";
            string _uniqueNumber = "";
            if (_template.codeDASTemplateType.ReferenceLinkTypeGUID == DASTemplateOwnerTypes.Staff)
            {
                var _staff = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == model.FileReferenceGUID).FirstOrDefault();
                _Name = _staff.FullName;
                _uniqueNumber = _staff.EmploymentID.ToString();
                _type = "Staff";
            }
            if (_template.codeDASTemplateType.ReferenceLinkTypeGUID == DASTemplateOwnerTypes.Refugee)
            {
                var _staff = DbDAS.dataFile.Where(x => x.FileGUID == model.FileReferenceGUID).FirstOrDefault();
                _Name = _staff.FileNumber.ToString();
                _uniqueNumber = _staff.FileNumber.ToString();
                _type = "Refugee";
            }
            ArchiveTemplate.ArchiveTemplateDocumentCodeNumber = _template.DocumentCode + "-" + _uniqueNumber;
            ArchiveTemplate.FileReferenceTypeGUID = _template.codeDASTemplateType.ReferenceLinkTypeGUID;
            ArchiveTemplate.FileReferenceTypeName = _type;
            ArchiveTemplate.FileReferenceName = _Name;
            ArchiveTemplate.TemplateTypeGUID = _template.TemplateTypeGUID;
            ArchiveTemplate.TemplateTypeDocumentGUID = model.TemplateTypeDocumentGUID;
            ArchiveTemplate.CreateByGUID = UserGUID;
            ArchiveTemplate.CreateDate = DateTime.Now;
            ArchiveTemplate.UpdateByGUID = UserGUID;
            ArchiveTemplate.UpdateDate = DateTime.Now;
            ArchiveTemplate.LastVerificationStatusGUID = DocumentVerificationStatus.Submitted;
            ArchiveTemplate.OrganizationInstanceGUID = model.OrganizationInstanceGUID;
            ArchiveTemplate.DutyStationGUID = model.DutyStationGUID;


            DbDAS.Create(ArchiveTemplate, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);
            List<dataArchiveTemplateDocumentSoftField> allSoftFields = new List<dataArchiveTemplateDocumentSoftField>();
            if (model.DocumentSoftFieldVM != null && model.DocumentSoftFieldVM.Count > 0)
            {
                model.DocumentSoftFieldVM.Where(x => x.SoftFieldNameValue != null).ForEach(myDet =>
                   allSoftFields.Add(new dataArchiveTemplateDocumentSoftField
                   {
                       ArchiveTemplateDocumentSoftFieldGUID = Guid.NewGuid(),
                       ArchiveTemplateDocumentGUID = ArchiveTemplate.ArchiveTemplateDocumentGUID,
                       TemplateTypeDocumentSoftFieldGUID = myDet.TemplateTypeDocumentSoftFieldGUID,
                       SoftFieldValue = myDet.SoftFieldName,
                       CreateByGUID = UserGUID,
                       CreateDate = ExecutionTime,
                       Active = true,
                   })
             );
                DbDAS.CreateBulk(allSoftFields, Permissions.RefugeeScannedDocument.CreateGuid, DateTime.Now, DbCMS);
            }
            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.ArchiveTemplateDataTable, ControllerContext, "ArchiveTemplateDataTableControls"));


            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.RefugeeScannedDocument.Create, Apps.DAS, new UrlHelper(Request.RequestContext).Action("Create", "ArchivedDocument", new { Area = "DAS" })), Container = "ArchiveTemplateDetailFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.RefugeeScannedDocument.Update, Apps.DAS), Container = "ArchiveTemplateDetailFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.RefugeeScannedDocument.Delete, Apps.DAS), Container = "ArchiveTemplateDetailFormControls" });

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                //return Json(DbDAS.SingleUpdateMessage(DataTableNames.ArchiveTemplateDataTable, DbDAS.PrimaryKeyControl(ArchiveTemplate), DbDAS.RowVersionControls(Portal.SingleToList(ArchiveTemplate))));
                return Json(DbDAS.SingleCreateMessage(DbDAS.PrimaryKeyControl(ArchiveTemplate), DbDAS.RowVersionControls(ArchiveTemplate, ArchiveTemplate), Partials, "window.location.replace('/DAS/ArchivedDocument/ArchiveTemplateDocumentIndex?PK=" + EntityPK+"')", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }



        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ArchiveTemplateDocumentUpdate(ArchiveTemplateDocumentUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.RefugeeScannedDocument.Update, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid) return PartialView("~/Areas/DAS/Views/ArchivedDocument/_ArchiveDocumentForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            dataArchiveTemplateDocument ArchiveTemplate = Mapper.Map(model, new dataArchiveTemplateDocument());
            DbDAS.Update(ArchiveTemplate, Permissions.RefugeeScannedDocument.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.SingleUpdateMessage(DataTableNames.ArchiveTemplateDataTable, DbDAS.PrimaryKeyControl(ArchiveTemplate), DbDAS.RowVersionControls(Portal.SingleToList(ArchiveTemplate))));
                //return Json(DbDAS.SingleUpdateMessage(null, null, DbDAS.RowVersionControls(ArchiveTemplate, ArchiveTemplate)));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyArchiveTemplateDocument((Guid)model.ArchiveTemplateDocumentGUID);
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ArchiveTemplateDocumentDelete(dataArchiveTemplateDocument model)
        {
            if (!CMS.HasAction(Permissions.RefugeeScannedDocument.Delete, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataArchiveTemplateDocument> DeletedArchiveTemplate = DeleteArchiveTemplateDocument(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.RefugeeScannedDocument.Restore, Apps.DAS), Container = "ArchiveTemplateFormControls" });

            try
            {
                int CommitedRows = DbDAS.SaveChanges();
                DbDAS.SaveChanges();
                return Json(DbDAS.SingleDeleteMessage(CommitedRows, DeletedArchiveTemplate.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyArchiveTemplateDocument(model.ArchiveTemplateDocumentGUID);
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ArchiveTemplateDocumentRestore(dataArchiveTemplateDocument model)
        {
            if (!CMS.HasAction(Permissions.RefugeeScannedDocument.Restore, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveArchiveTemplateDocument(model))
            {
                return Json(DbDAS.RecordExists());
            }

            List<dataArchiveTemplateDocument> RestoredArchiveTemplate = RestoreArchiveTemplate(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.RefugeeScannedDocument.Create, Apps.DAS, new UrlHelper(Request.RequestContext).Action("ArchiveTemplateCreate", "Configuration", new { Area = "DAS" })), Container = "ArchiveTemplateFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.RefugeeScannedDocument.Update, Apps.DAS), Container = "ArchiveTemplateFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.RefugeeScannedDocument.Delete, Apps.DAS), Container = "ArchiveTemplateFormControls" });

            try
            {
                int CommitedRows = DbDAS.SaveChanges();
                DbDAS.SaveChanges();
                return Json(DbDAS.SingleRestoreMessage(CommitedRows, RestoredArchiveTemplate, DbDAS.PrimaryKeyControl(RestoredArchiveTemplate.FirstOrDefault()), Url.Action(DataTableNames.ArchiveTemplateDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyArchiveTemplateDocument(model.ArchiveTemplateDocumentGUID);
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ArchiveTemplateDocumentDataTableDelete(List<dataArchiveTemplateDocument> models)
        {
            if (!CMS.HasAction(Permissions.RefugeeScannedDocument.Delete, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataArchiveTemplateDocument> DeletedArchiveTemplate = DeleteArchiveTemplateDocument(models);

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.PartialDeleteMessage(DeletedArchiveTemplate, models, DataTableNames.ArchiveTemplateDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ArchiveTemplateDocumentDataTableRestore(List<dataArchiveTemplateDocument> models)
        {
            if (!CMS.HasAction(Permissions.RefugeeScannedDocument.Restore, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataArchiveTemplateDocument> RestoredArchiveTemplate = RestoreArchiveTemplate(models);

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.PartialRestoreMessage(RestoredArchiveTemplate, models, DataTableNames.ArchiveTemplateDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        private List<dataArchiveTemplateDocument> DeleteArchiveTemplateDocument(List<dataArchiveTemplateDocument> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<dataArchiveTemplateDocument> DeletedArchiveTemplate = new List<dataArchiveTemplateDocument>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = "SELECT ArchiveTemplateDocumentGUID,CONVERT(varchar(50), ArchiveTemplateDocumentGUID) as C2 ,dataArchiveTemplateDocumentRowVersion FROM code.dataArchiveTemplateDocument where ArchiveTemplateDocumentGUID in (" + string.Join(",", models.Select(x => "'" + x.ArchiveTemplateDocumentGUID + "'").ToArray()) + ")";

            string query = DbDAS.QueryBuilder(models, Permissions.RefugeeScannedDocument.DeleteGuid, SubmitTypes.Delete, "");

            var Records = DbDAS.Database.SqlQuery<dataArchiveTemplateDocument>(query).ToList();
            foreach (var record in Records)
            {
                DeletedArchiveTemplate.Add(DbDAS.Delete(record, ExecutionTime, Permissions.RefugeeScannedDocument.DeleteGuid, DbCMS));
            }

            return DeletedArchiveTemplate;
        }

        private List<dataArchiveTemplateDocument> RestoreArchiveTemplate(List<dataArchiveTemplateDocument> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<dataArchiveTemplateDocument> RestoredArchiveTemplate = new List<dataArchiveTemplateDocument>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = "SELECT ArchiveTemplateDocumentGUID,CONVERT(varchar(50), ArchiveTemplateDocumentGUID) as C2 ,dataArchiveTemplateDocumentRowVersion FROM code.dataArchiveTemplateDocument where ArchiveTemplateDocumentGUID in (" + string.Join(",", models.Select(x => "'" + x.ArchiveTemplateDocumentGUID + "'").ToArray()) + ")";

            string query = DbDAS.QueryBuilder(models, Permissions.RefugeeScannedDocument.DeleteGuid, SubmitTypes.Restore, "");

            var Records = DbDAS.Database.SqlQuery<dataArchiveTemplateDocument>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveArchiveTemplateDocument(record))
                {
                    RestoredArchiveTemplate.Add(DbDAS.Restore(record, Permissions.RefugeeScannedDocument.DeleteGuid, Permissions.RefugeeScannedDocument.RestoreGuid, RestoringTime, DbCMS));
                }
            }


            return RestoredArchiveTemplate;
        }

        private JsonResult ConcurrencyArchiveTemplateDocument(Guid PK)
        {
            ArchiveTemplateDocumentUpdateModel dbModel = new ArchiveTemplateDocumentUpdateModel();

            var ArchiveTemplate = DbDAS.dataArchiveTemplateDocument.Where(x => x.ArchiveTemplateDocumentGUID == PK).FirstOrDefault();
            var dbArchiveTemplate = DbDAS.Entry(ArchiveTemplate).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbArchiveTemplate, dbModel);





            if (ArchiveTemplate.dataArchiveTemplateDocumentRowVersion.SequenceEqual(dbModel.dataArchiveTemplateDocumentRowVersion))
            {
                return Json(DbDAS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbDAS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveArchiveTemplateDocument(Object model)
        {
            dataArchiveTemplateDocument ArchiveTemplate = Mapper.Map(model, new dataArchiveTemplateDocument());
            int ModelDescription = DbDAS.dataArchiveTemplateDocument
                                    .Where(x => x.FileReferenceGUID == ArchiveTemplate.FileReferenceGUID
                                    && x.TemplateTypeGUID == ArchiveTemplate.TemplateTypeGUID

                                               && x.Active).Count();
            if (ModelDescription > 0)
            {
                ModelState.AddModelError("FileReferenceGUID", "Record  already exists");
            }
            return (ModelDescription > 0);
        }

        #endregion

        #region Lists
        public ActionResult GetFileNumber(string ID)
        {
            var Result = DbDAS.dataFile.Where(x => x.FileNumber.Contains(ID)).
                Select(x => new
                {
                    x.FileNumber
                }).FirstOrDefault();

            return Json(new { Result = Result }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetSoftFields(Guid TemplateTypeDocumentGUID)
        {
            ArchiveTemplateDocumentUpdateModel model = new ArchiveTemplateDocumentUpdateModel();

            var myDet = (from a in DbDAS.codeDASTemplateTypeDocument.Where(x => x.Active && x.TemplateTypeDocumentGUID == TemplateTypeDocumentGUID)
                         join b in DbDAS.codeDASTemplateTypeDocumentSoftField.Where(x => x.Active) on a.TemplateTypeDocumentGUID equals b.TemplateTypeDocumentGUID

                         select new DocumentSoftFieldVM
                         {
                             TemplateTypeDocumentSoftFieldGUID = (Guid)b.TemplateTypeDocumentSoftFieldGUID,
                             SoftFieldName = b.SoftFieldName
                         }).ToList();
            model.DocumentSoftFieldVM = myDet;
            return PartialView("~/Areas/DAS/Views/ArchivedDocument/DocumentSoftField/_DocumentSoftFieldValue.cshtml", model);
        }

        #endregion

        #region Scanned Images 
        public JsonResult SearchByCase(Guid _archiveTemplateDocumentGUID)
        {
            string[] Files = null;

            var myScann = DbDAS.dataArchiveTemplateDocumentImage.Where(x => x.ArchiveTemplateDocumentGUID == _archiveTemplateDocumentGUID).FirstOrDefault();
            var result = DbDAS.dataArchiveTemplateDocument.Where(x => x.ArchiveTemplateDocumentGUID == _archiveTemplateDocumentGUID).Select(x => new { x.ArchiveTemplateDocumentGUID, x.ArchiveTemplateDocumentCodeNumber, x.TemplateTypeDocumentGUID }).FirstOrDefault();
            if (myScann != null)
            {
                var VersionNumber = 1;// DbDAS.dataScannDocumentVersionHistory.Where(x => x.ScannDocumentGUID == myScann.ScannDocumentGUID).OrderBy(x => x.VersionNumber).Select(x => x.VersionNumber).ToList();

                var codeCabinets = DbDAS.codeDASDocumentCabinet.Select(x => new { Guid = x.DocumentCabinetGUID, Name = x.DocumentCabinetName }).ToList();
                var codeMetaDatas = DbDAS.codeDASDocumentMetaData.Select(x => new { Guid = x.DocumentMetaDataGUID, Name = x.DocumentMetaDataName }).ToList();
                var codeCabinetShelfs = DbDAS.codeDASDocumentCabinetShelf.Select(x => new { Guid = x.DocumentCabinetShelfGUID, Name = x.ShelfNumber }).ToList();
                var ScannDocumentGUID = DbDAS.dataArchiveTemplateDocument.Where(x => x.ArchiveTemplateDocumentGUID == _archiveTemplateDocumentGUID).Select(x => new { Guid = x.TemplateTypeDocumentGUID, Name = x.DocumentName }).ToList();
                int isNew = 0;

                if (myScann == null)
                {
                    isNew = 1;
                }
                var currentUserScannerSetting = DbDAS.dataDefaultScannerSetting.
                        Select(x => new { x.ScanningType, x.PaperFormate, x.Resolution, x.PaperSize, x.ColorMode }).
                        FirstOrDefault();
                return Json(new
                {
                    success = 1,
                    codeCabinets = codeCabinets,
                    codeCabinetShelfs = codeCabinetShelfs,
                    result = result,
                    isNew = isNew,
                    Files = Files,
                    currentUserScannerSetting = currentUserScannerSetting,
                    VersionNumber = VersionNumber,
                    codeMetaDatas = codeMetaDatas,
                    ScannDocumentGUID = ScannDocumentGUID
                }, JsonRequestBehavior.AllowGet);

            }
            else
            {


                var codeCabinets = DbDAS.codeDASDocumentCabinet.Select(x => new { Guid = x.DocumentCabinetGUID, Name = x.DocumentCabinetName }).ToList();
                var codeMetaDatas = DbDAS.codeDASDocumentMetaData.Select(x => new { Guid = x.DocumentMetaDataGUID, Name = x.DocumentMetaDataName }).ToList();
                var codeCabinetShelfs = DbDAS.codeDASDocumentCabinetShelf.Select(x => new { Guid = x.DocumentCabinetShelfGUID, Name = x.ShelfNumber }).ToList();
                int isNew = 0;

                if (myScann == null)
                {
                    isNew = 1;
                }
                var currentUserScannerSetting = DbDAS.dataDefaultScannerSetting.
                        Select(x => new { x.ScanningType, x.PaperFormate, x.Resolution, x.PaperSize, x.ColorMode }).
                        FirstOrDefault();
                return Json(new
                {
                    success = 1,
                    codeCabinets = codeCabinets,
                    codeCabinetShelfs = codeCabinetShelfs,
                    result = result,
                    isNew = isNew,
                    Files = Files,
                    currentUserScannerSetting = currentUserScannerSetting,

                    codeMetaDatas = codeMetaDatas,

                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SaveImagesSannned(Guid _templateTypeDocumentGUID, int New)
        {
            ///for encryption image
            Guid userGuid = UserGUID;
            string path =@"D:\Archive\ArchivedDocuments\" + _templateTypeDocumentGUID;

            var _myTemplateDocument = DbDAS.dataArchiveTemplateDocument.Where(x => x.ArchiveTemplateDocumentGUID == _templateTypeDocumentGUID).FirstOrDefault();
            _myTemplateDocument.UpdateByGUID = UserGUID;
            _myTemplateDocument.UpdateDate = DateTime.Now;
            _myTemplateDocument.IsUpdated = false;
            _myTemplateDocument.LastVerificationStatusGUID = DocumentVerificationStatus.PendingVerification;
            int NumberPageMax = 0;
            List<dataArchiveTemplateDocumentImage> ToAddImages = new List<dataArchiveTemplateDocumentImage>();
            if (New == 1)
            {
                //delete folder if is exit 
                if (Directory.Exists(path))
                {
                    string[] files_Tem = Directory.GetFiles(path);
                    if (files_Tem.Count() > 0)
                    {
                        foreach (var file in files_Tem)
                        {
                            System.IO.File.Delete(file);
                        }
                    }
                }
                else
                {
                    Directory.CreateDirectory(path);
                }
            }
            else
            {
                //MyScanDocument = DbDAS.dataArchiveTemplateDocument.Where(x => x.FileGUID == mycase.FileGUID).FirstOrDefault();
                var images = DbDAS.dataArchiveTemplateDocumentImage.Where(x => x.ArchiveTemplateDocumentGUID == _templateTypeDocumentGUID).
                    OrderBy(x => x.ImageNumber).ToList();
                NumberPageMax = (int)images[images.Count() - 1].ImageNumber + 1;
            }
            //     
            // key Encryption
            byte[] Key = Encoding.UTF8.GetBytes("asdf!@#$1234ASDF");
            HttpFileCollectionBase files = Request.Files;

            //function Encryptio
            imagesServices.EncryptionImage(files, path, Key);


            string pathDecryption = @"D:\Archive\DEC\" + _templateTypeDocumentGUID;
            string pathDecryption_Icons = Server.MapPath("/Areas/DAS/DEC_Icons/" + _templateTypeDocumentGUID);
            imagesServices.SaveImageWithResizeImages(files, pathDecryption_Icons);
            imagesServices.SaveImage(files, pathDecryption);

            //save images in database
            for (var i = 0; i < files.Count; i++)
            {

                    HttpPostedFileBase file = files[i];

                    Guid myImageGuid = Guid.NewGuid();
                    dataArchiveTemplateDocumentImage myCurrImage = new dataArchiveTemplateDocumentImage
                    {
                        ArchiveTemplateImageGUID = myImageGuid,
                        ArchiveTemplateDocumentGUID = _templateTypeDocumentGUID,
                        CreateByGUID = userGuid,
                        CreateDate = DateTime.Now,
                        ImageName = Path.GetFileName(file.FileName),
                        ImageNumber = i + NumberPageMax,
                        Active = true,
                        Archived = false,
                    };
                    ToAddImages.Add(myCurrImage);
                
            }
            DateTime ExecutionTime = DateTime.Now;
            DbDAS.CreateBulk(ToAddImages, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);
            DbDAS.SaveChanges();
            return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveFinal(Guid _archiveTemplateDocumentGUID)
        {
            /// define destination Path to save Images
            string DestFolder = @"D:\Archive\ArchivedDocuments\"+ _archiveTemplateDocumentGUID.ToString();
            // define datascandocument and datascandocumentimages for old datascanddocument
            var MYOldDataScanDocument = DbDAS.dataArchiveTemplateDocument.Where(x => x.ArchiveTemplateDocumentGUID == _archiveTemplateDocumentGUID).FirstOrDefault();
            dataArchiveTemplateDocument mydoc = DbDAS.dataArchiveTemplateDocument.Where(x => x.TemplateTypeDocumentGUID == _archiveTemplateDocumentGUID).FirstOrDefault();

            var MyoldDataScanDocumentImages = DbDAS.dataArchiveTemplateDocumentImage.Where(x => x.ArchiveTemplateDocumentGUID == MYOldDataScanDocument.TemplateTypeDocumentGUID).OrderBy(x => x.ImageNumber).ToList();

            Guid userGuid = UserGUID;
            int VersionNumberCase = 0;
            /// create version for updated datascandocument and add images to version number
            if (MYOldDataScanDocument != null)
            {
                string PathVersion =@"D:\Archive\Documents\VersiondataScannDocument\" + _archiveTemplateDocumentGUID;
                string PathVersionNumber = "";
                if (!Directory.Exists(PathVersion))
                {
                    Directory.CreateDirectory(PathVersion);
                }

                /////correct ??~~~~~~~~~~~~~  chose document Guid not File GUID below 
                ///

                var VersionScandocument = DbDAS.dataArchiveTemplateDocumentVersionHistory.Where(x => x.ArchiveTemplateDocumentGUID == _archiveTemplateDocumentGUID
                         && x.IsCurrentVersion == true).ToList();
                if (VersionScandocument != null && VersionScandocument.Count > 0)
                {
                    int LastVersionNumber = (int)VersionScandocument[VersionScandocument.Count - 1].VersionNumber;
                    foreach (var item in VersionScandocument)
                    {
                        item.IsCurrentVersion = false;
                    }

                    // create version history for old datascandocument
                    dataArchiveTemplateDocumentVersionHistory myDocVersion = new dataArchiveTemplateDocumentVersionHistory
                    {
                        ArchiveTemplateDocumentVersionHistoryGUID = Guid.NewGuid(),
                        ArchiveTemplateDocumentGUID = mydoc.ArchiveTemplateDocumentGUID,
                        CreateByGUID = userGuid,
                        VersionNumber = LastVersionNumber + 1,
                        IsCurrentVersion = true,
                        CreateDate = DateTime.Now,
                        Active = true,
                        DeletedOn = null
                    };
                    DbDAS.dataArchiveTemplateDocumentVersionHistory.Add(myDocVersion);
                    DbDAS.SaveChanges();

                    PathVersionNumber =@"D:\Archive\Documents\VersiondataScannDocument\" + _archiveTemplateDocumentGUID + "\\" + myDocVersion.VersionNumber;
                    VersionNumberCase = (int)myDocVersion.VersionNumber;
                    Directory.CreateDirectory(PathVersionNumber);
                }
                else
                {
                    PathVersionNumber =@"D:\Archive\Documents\VersionDataScannDocuments\" + _archiveTemplateDocumentGUID + "\\" + 1;
                    VersionNumberCase = 1;
                    Directory.CreateDirectory(PathVersionNumber);
                    dataArchiveTemplateDocumentVersionHistory myDocVersion = new dataArchiveTemplateDocumentVersionHistory
                    {
                        ArchiveTemplateDocumentVersionHistoryGUID = Guid.NewGuid(),
                        ArchiveTemplateDocumentGUID = mydoc.ArchiveTemplateDocumentGUID,
                        CreateByGUID = userGuid,
                        VersionNumber = 1,
                        IsCurrentVersion = true,
                        CreateDate = DateTime.Now,
                        Active = true,
                        DeletedOn = null
                    };
                    DbDAS.dataArchiveTemplateDocumentVersionHistory.Add(myDocVersion);
                    DbDAS.SaveChanges();

                }
                //add images to Version history
                if (Directory.Exists(PathVersionNumber))
                {
                    string[] files_Tem = Directory.GetFiles(PathVersionNumber);
                    if (files_Tem.Count() > 0)
                    {
                        foreach (var file in files_Tem)
                        {
                            System.IO.File.Delete(file);
                        }

                    }
                }
                string[] Files = Directory.GetFiles(DestFolder);
                var VersionCurrent = DbDAS.dataArchiveTemplateDocumentVersionHistory.Where(x => x.IsCurrentVersion == true).FirstOrDefault();
                List<dataArchiveTemplateDocumentImageVersionHistory> AddImagesToversionNumber = new List<dataArchiveTemplateDocumentImageVersionHistory>();
                if (MyoldDataScanDocumentImages.Count > 0 && MyoldDataScanDocumentImages != null)
                {
                    for (int i = 0; i < MyoldDataScanDocumentImages.Count; i++)
                    {
                        dataArchiveTemplateDocumentImageVersionHistory myDocVersionImage = new dataArchiveTemplateDocumentImageVersionHistory
                        {
                            ArchiveTemplateDocumentImageVersionHistoryGUID = Guid.NewGuid(),
                            ArchiveTemplateDocumentVersionHistoryGUID = VersionCurrent.ArchiveTemplateDocumentVersionHistoryGUID,
                            CreateByGUID = MyoldDataScanDocumentImages[i].CreateByGUID,
                            CreateDate = MyoldDataScanDocumentImages[i].CreateDate,
                            ImageName = MyoldDataScanDocumentImages[i].ImageName,
                            ImageNumber = MyoldDataScanDocumentImages[i].ImageNumber,
                            Active = true,
                            Archived = false
                            
                        };
                        string DestPathImage = Path.Combine(PathVersionNumber, Path.GetFileName(Files[i]));
                        string SoursePathImage = Path.Combine(DestFolder, Path.GetFileName(Files[i]));
                        System.IO.File.Copy(SoursePathImage, DestPathImage);
                        // add images to list 
                        AddImagesToversionNumber.Add(myDocVersionImage);
                    }
                    DbDAS.dataArchiveTemplateDocumentImageVersionHistory.AddRange(AddImagesToversionNumber);

                }

            }
            //convert to pdf 
            //string ImagesPath =@"D:\Archive//DEC/" + userGuid + "/" + _archiveTemplateDocumentGUID);
            string ImagesPath =@"D:\Archive\DEC\" + _archiveTemplateDocumentGUID;

            string PDFName = VersionNumberCase + ".pdf";
            string PDFPath =@"D:\Archive\Documents\VersionPDF\" + _archiveTemplateDocumentGUID + "\\";
            Directory.CreateDirectory(PDFPath);

            if (VersionNumberCase != 0)
            {
                MyDocServices.ConvertToPdf(ImagesPath, PDFPath, PDFName);
            }
            //   delete folder which contain images after convert to pdf
            if (Directory.Exists(ImagesPath))
            {
                string[] files_Tem = Directory.GetFiles(ImagesPath);
                if (files_Tem.Count() > 0)
                {
                    foreach (var file in files_Tem)
                    {
                        System.IO.File.Delete(file);
                    }
                }
                //System.IO.File.Delete(ImagesPath);
            }

            DbDAS.SaveChanges();
            return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
        }


        //Read Images from TEMsearch
        public JsonResult ReadImagesFromTem(Guid _archiveTemplateDocumentGUID)
        {
            /// for decryption images
            byte[] Key = Encoding.UTF8.GetBytes("asdf!@#$1234ASDF");
            RijndaelManaged rmCryp = new RijndaelManaged();
            string sourcePath =@"D:\Archive\ArchivedDocuments\" + _archiveTemplateDocumentGUID;
            Guid userGuid = UserGUID;
            string pathDecryption =@"D:\Archive\DEC\"  + _archiveTemplateDocumentGUID;
            string pathDecryption_Icons = Server.MapPath("/Areas/DAS/DEC_Icons/" + _archiveTemplateDocumentGUID);
            //string pathDecryption =@"D:\Archive/DEC/" + userGuid + "/" + _archiveTemplateDocumentGUID);
            bool folderFound = false;
            string[] sourcefiles;
            string[] destenationfiles;
            if (Directory.Exists(sourcePath))
            {
                var imageRows = DbDAS.dataArchiveTemplateDocumentImage.Where(x => x.ArchiveTemplateDocumentGUID == _archiveTemplateDocumentGUID && x.Active).ToList();
                sourcefiles = Directory.GetFiles(sourcePath);
                if (sourcefiles.Count() != 0)
                {
                    folderFound = true;
                    if (!Directory.Exists(pathDecryption_Icons))
                    {
                        Directory.CreateDirectory(pathDecryption_Icons);
                        imagesServices.DecryptionWithResizeImages(sourcefiles, pathDecryption_Icons, Key, 0.1f);
                    }
                    else
                    {
                        foreach (var row in imageRows)
                        {
                            destenationfiles = Directory.GetFiles(pathDecryption_Icons, row.ImageName);
                            if (destenationfiles.Length == 0)
                            {
                                imagesServices.DecryptionWithResizeImages(sourcefiles.Where(x => x.Contains(row.ImageName)).ToArray(), pathDecryption_Icons, Key, 0.1f);
                            }
                        }
                    }
                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            //Do an advanced looging here which takes a while
                            if (!Directory.Exists(pathDecryption))
                            {
                                Directory.CreateDirectory(pathDecryption);
                                // for decryption images
                                imagesServices.DecryptionImages(sourcefiles, pathDecryption, Key);
                            }
                            else
                            {

                                foreach (var row in imageRows)
                                {
                                    destenationfiles = Directory.GetFiles(pathDecryption, row.ImageName);
                                    if (destenationfiles.Length == 0)
                                    {
                                        imagesServices.DecryptionImages(sourcefiles.Where(x => x.Contains(row.ImageName)).ToArray(), pathDecryption, Key);

                                    }
                                }
                            }
                        }
                        catch { }
                    });
                }
            }
            else
            {
                Directory.CreateDirectory(pathDecryption_Icons);
                if (!System.IO.File.Exists(pathDecryption_Icons + "\\ImageNotfound.png"))
                {
                    System.IO.File.Copy(Server.MapPath("/Areas/DAS/DEC_Icons") + "\\ImageNotfound.png", pathDecryption_Icons + "\\ImageNotfound.png");
                }
            }
            ////end
            string filename;
            List<string> filename1 = new List<string>();
            List<dataArchiveTemplateDocumentImage> myTempImages = new List<dataArchiveTemplateDocumentImage>();
            var MydatascanDocument = DbDAS.dataArchiveTemplateDocument.Where(x => x.ArchiveTemplateDocumentGUID == _archiveTemplateDocumentGUID).FirstOrDefault();
            
            myTempImages = DbDAS.dataArchiveTemplateDocumentImage.Where(x => x.Active && x.ArchiveTemplateDocumentGUID == _archiveTemplateDocumentGUID).OrderBy(x => x.ImageNumber).ToList();
            for (int i = 0; i < myTempImages.Count; i++)
            {
                filename = folderFound? myTempImages[i].ImageName: "ImageNotfound.png";
                filename1.Add(filename);
            }
            return Json(new { success = folderFound?1:0, filename1 = filename1, userGuid = userGuid }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ShowImage(string id,string _GUID)
        {
            var dir = @"D:\Archive\DEC\" + _GUID + "\\"+ id;
            return base.File(dir/*ResizeImage(dir, 1080,1920)*/, "image/jpeg");
        }


        public byte[] ResizeImage(string filePath, int width, int height)
        {
            byte[] resizedImageBytes;

            using (Image originalImage = Image.FromFile(filePath))
            {
                using (Image resizedImage = new Bitmap(width, height))
                {
                    using (Graphics graphics = Graphics.FromImage(resizedImage))
                    {
                        graphics.DrawImage(originalImage, 0, 0, width, height);
                    }

                    using (MemoryStream ms = new MemoryStream())
                    {
                        resizedImage.Save(ms, ImageFormat.Jpeg);
                        resizedImageBytes = ms.ToArray();
                    }
                }
            }

            return resizedImageBytes;
        }


    public JsonResult UpdateItem(List<string> imageIds, List<string> imageIdsUpdate, Guid _archiveTemplateDocumentGUID)
        {
            if (!CMS.HasAction(Permissions.RefugeeScannedDocument.Update, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var item = DbDAS.dataArchiveTemplateDocumentImage.Where(x => x.ArchiveTemplateDocumentGUID == _archiveTemplateDocumentGUID).ToList();
            
            var result = (from a in item
                          join b in imageIds on a.ImageName equals b 
                          let index = imageIds.IndexOf(b)
                          where index != a.ImageNumber
                          orderby a.ImageNumber
                          select
                          new dataArchiveTemplateDocumentImage()
                          {
                              dataArchiveTemplateDocument=a.dataArchiveTemplateDocument,
                              Active = true,
                              ImageName = a.ImageName,
                              Archived = a.Archived,
                              ArchiveTemplateDocumentGUID = a.ArchiveTemplateDocumentGUID,
                              ArchiveTemplateImageGUID = a.ArchiveTemplateImageGUID,
                              CreateByGUID = a.CreateByGUID,
                              CreateDate = a.CreateDate,
                              ImageNumber = index,
                              DeletedOn = a.DeletedOn,
                              dataArchiveTemplateDocumentImageRowVersion=a.dataArchiveTemplateDocumentImageRowVersion
                          }
                         ).ToList();

            DateTime ExecutionTime = DateTime.Now;
            DbDAS.UpdateBulk(result, Permissions.RefugeeScannedDocument.UpdateGuid, ExecutionTime,DbCMS);
            var doc = DbDAS.dataArchiveTemplateDocument.Where(x => x.ArchiveTemplateDocumentGUID == _archiveTemplateDocumentGUID).FirstOrDefault();
            doc.LastVerificationStatusGUID = DocumentVerificationStatus.PendingVerification;
            doc.UpdateByGUID = UserGUID;
            doc.UpdateDate = ExecutionTime;
            doc.IsUpdated = false;
            DbDAS.Update(doc, Permissions.RefugeeScannedDocument.UpdateGuid, ExecutionTime, DbCMS);
            DbCMS.SaveChanges();
            DbDAS.SaveChanges();

            return Json(true);
        }

        public JsonResult UpdateBlukItems(List<string> imageIds, Guid _archiveTemplateDocumentGUID)
        {
            if (!CMS.HasAction(Permissions.RefugeeScannedDocument.Update, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var item = DbDAS.dataArchiveTemplateDocumentImage.Where(x => x.ArchiveTemplateDocumentGUID == _archiveTemplateDocumentGUID).ToList();

            var result = (from a in item
                          join b in imageIds on a.ImageName equals b
                          let index = imageIds.IndexOf(b)
                          where index != a.ImageNumber
                          orderby a.ImageNumber
                          select
                          new dataArchiveTemplateDocumentImage()
                          {
                              dataArchiveTemplateDocument = a.dataArchiveTemplateDocument,
                              Active = true,
                              ImageName = a.ImageName,
                              Archived = a.Archived,
                              ArchiveTemplateDocumentGUID = a.ArchiveTemplateDocumentGUID,
                              ArchiveTemplateImageGUID = a.ArchiveTemplateImageGUID,
                              CreateByGUID = a.CreateByGUID,
                              CreateDate = a.CreateDate,
                              ImageNumber = index,
                              DeletedOn = a.DeletedOn,
                              dataArchiveTemplateDocumentImageRowVersion = a.dataArchiveTemplateDocumentImageRowVersion
                          }
                         ).ToList();

            DateTime ExecutionTime = DateTime.Now;
            DbDAS.UpdateBulk(result, Permissions.RefugeeScannedDocument.UpdateGuid, ExecutionTime, DbCMS);
            var doc = DbDAS.dataArchiveTemplateDocument.Where(x => x.ArchiveTemplateDocumentGUID == _archiveTemplateDocumentGUID).FirstOrDefault();
            doc.LastVerificationStatusGUID = DocumentVerificationStatus.PendingVerification;
            doc.UpdateByGUID = UserGUID;
            doc.UpdateDate = ExecutionTime;
            doc.IsUpdated = false;
            DbDAS.Update(doc, Permissions.RefugeeScannedDocument.UpdateGuid, ExecutionTime, DbCMS);
            DbDAS.SaveChanges();

            return Json(true);
        }

        public ActionResult SaveTemplateDocumentInformation(Guid _archiveTemplateDocumentGUID, Guid _templateGUID,Guid _TemplateTypeDocumentGUID, Guid _FileReferenceGUID,Guid? _DocumentCabinetShelfGUID, List<Guid?> _DocumentTagGUIDs)
        {
            if (!CMS.HasAction(Permissions.RefugeeScannedDocument.Create, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || _archiveTemplateDocumentGUID == null || _templateGUID == null)
            {
                return Json(DbDAS.PermissionError());
            }

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = _archiveTemplateDocumentGUID;

            dataArchiveTemplateDocument ArchiveTemplate = new dataArchiveTemplateDocument();
            ArchiveTemplate.ArchiveTemplateDocumentGUID = EntityPK;
            ArchiveTemplate.FileReferenceGUID = _FileReferenceGUID;
            ArchiveTemplate.DocumentCabinetShelfGUID = _DocumentCabinetShelfGUID;
            var _template = DbDAS.codeDASTemplateTypeDocument.Where(x => x.TemplateTypeDocumentGUID == _TemplateTypeDocumentGUID).FirstOrDefault();
            string _Name = "";
            string _type = "";
            string _uniqueNumber = "";
            if (_template.codeDASTemplateType.ReferenceLinkTypeGUID == DASTemplateOwnerTypes.Staff)
            {
                var _staff = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == _FileReferenceGUID).FirstOrDefault();
                _Name = _staff.FullName;
                _uniqueNumber = _staff.EmploymentID.ToString();
                _type = "Staff";
            }
            if (_template.codeDASTemplateType.ReferenceLinkTypeGUID == DASTemplateOwnerTypes.Refugee)
            {
                var _staff = DbDAS.dataFile.Where(x => x.FileGUID == _FileReferenceGUID).FirstOrDefault();
                _Name = _staff.FileNumber.ToString();
                _uniqueNumber = _staff.FileNumber.ToString();
                _type = "Refugee";
            }
            ArchiveTemplate.ArchiveTemplateDocumentCodeNumber = _template.DocumentCode + "-" + _uniqueNumber;
            ArchiveTemplate.FileReferenceTypeGUID = _template.codeDASTemplateType.ReferenceLinkTypeGUID;
            ArchiveTemplate.FileReferenceTypeName = _type;
            ArchiveTemplate.FileReferenceName = _Name;
            ArchiveTemplate.TemplateTypeGUID = _template.TemplateTypeGUID;
            ArchiveTemplate.TemplateTypeDocumentGUID = _TemplateTypeDocumentGUID;
            ArchiveTemplate.CreateByGUID = UserGUID;
            ArchiveTemplate.CreateDate = DateTime.Now;
            ArchiveTemplate.LastVerificationStatusGUID = DocumentVerificationStatus.Submitted;

            DbDAS.Create(ArchiveTemplate, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);
            List<dataArchiveTemplateDocumentTag> allSoftFields = new List<dataArchiveTemplateDocumentTag>();
            if (_DocumentTagGUIDs != null && _DocumentTagGUIDs.Count > 0)
            {
                _DocumentTagGUIDs.ForEach(myDet =>
                   allSoftFields.Add(new dataArchiveTemplateDocumentTag
                   {
                       ArchiveTemplateDocumentTagGUID = Guid.NewGuid(),
                       ArchiveTemplateDocumentGUID = ArchiveTemplate.ArchiveTemplateDocumentGUID,
                       TemplateTypeDocumentTagGUID = myDet,

                       CreateByGUID = UserGUID,
                       CreateDate = ExecutionTime,
                       Active = true,
                   })

         );
                DbDAS.CreateBulk(allSoftFields, Permissions.RefugeeScannedDocument.CreateGuid, DateTime.Now, DbCMS);
            }

            dataArchiveTemplateFlowVerification flow = new dataArchiveTemplateFlowVerification

            {
                ArchiveTemplateFlowVerificationGUID = Guid.NewGuid(),
                ArchiveTemplateDocumentGUID = ArchiveTemplate.ArchiveTemplateDocumentGUID,
                FlowStatusGUID = DocumentVerificationStatus.Submitted,
                IsLastAction = true,
                CreateByGUID = UserGUID,
                CreateDate = ExecutionTime,
                OrderId = 1
            };

            DbDAS.Create(flow, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(new
                {
                    success = 1,


                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }


        #endregion

        #region Update
        public JsonResult DeleteImage(Guid CaseNumber, string ImageId)
        {
            if (!CMS.HasAction(Permissions.RefugeeScannedDocument.Delete, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            DateTime ExecutionTime = DateTime.Now;
            dataArchiveTemplateDocumentImage item = DbDAS.dataArchiveTemplateDocumentImage.Where(x => x.ImageName == ImageId).FirstOrDefault();
            string ImagePath =@"D:\Archive\ArchivedDocuments\" + CaseNumber + "\\" + item.ImageName;
            //delete image from Folder
            imagesServices.DeleteImage(ImagePath);

            //delete image from database DAS
            var MyDocument = DbDAS.dataArchiveTemplateDocument.Where(x => x.ArchiveTemplateDocumentGUID == CaseNumber).FirstOrDefault();
            List<dataArchiveTemplateDocumentImage> myTempImages = DbDAS.dataArchiveTemplateDocumentImage.Where(x => x.ArchiveTemplateDocumentGUID == MyDocument.ArchiveTemplateDocumentGUID).OrderBy(x => x.ImageNumber).ToList();
            for (int i = (int)(item.ImageNumber + 1); i < myTempImages.Count; i++)
            {
                myTempImages[i].ImageNumber = myTempImages[i].ImageNumber - 1;
            }
            DbDAS.Delete(item, ExecutionTime, Permissions.RefugeeScannedDocument.DeleteGuid, DbCMS);
            //DbDAS.dataArchiveTemplateDocumentImage.Remove(item);
            DbDAS.SaveChanges();
            return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteImageSelected(Guid CaseNumber, string[] ImageIds)
        {
            if (!CMS.HasAction(Permissions.RefugeeScannedDocument.Delete, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            foreach (var row in ImageIds)
            {
                DateTime ExecutionTime = DateTime.Now;
                dataArchiveTemplateDocumentImage item = DbDAS.dataArchiveTemplateDocumentImage.Where(x => x.ImageName == row).FirstOrDefault();
                string ImagePath = @"D:\Archive\ArchivedDocuments\" + CaseNumber + "\\" + item.ImageName;
                //delete image from Folder
                imagesServices.DeleteImage(ImagePath);

                ////delete image from database DAS
                //var MyDocument = DbDAS.dataArchiveTemplateDocument.Where(x => x.ArchiveTemplateDocumentGUID == CaseNumber).FirstOrDefault();
                //List<dataArchiveTemplateDocumentImage> myTempImages = DbDAS.dataArchiveTemplateDocumentImage.Where(x => x.ArchiveTemplateDocumentGUID == MyDocument.ArchiveTemplateDocumentGUID).OrderBy(x => x.ImageNumber).ToList();
                //for (int i = (int)(item.ImageNumber + 1); i < myTempImages.Count; i++)
                //{
                //    myTempImages[i].ImageNumber = myTempImages[i].ImageNumber - 1;
                //}
                //DbDAS.dataArchiveTemplateDocumentImage.Remove(item);
                DbDAS.Delete(item, ExecutionTime, Permissions.RefugeeScannedDocument.DeleteGuid, DbCMS);
            }
            DbDAS.SaveChanges();
            return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult MoveImageSelected(int index,Guid _archiveTemplateDocumentGUID, string[] ImageIds)
        {
            if (!CMS.HasAction(Permissions.RefugeeScannedDocument.Update, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var items = DbDAS.dataArchiveTemplateDocumentImage.Where(x => x.ArchiveTemplateDocumentGUID == _archiveTemplateDocumentGUID && !ImageIds.Contains(x.ImageName) && x.Active).OrderBy(x=>x.ImageNumber).ToList();
            var itemsSelected = DbDAS.dataArchiveTemplateDocumentImage.Where(x => ImageIds.Contains(x.ImageName)).OrderBy(x => x.ImageNumber).ToList();

            items.InsertRange(index, itemsSelected);

            var result = (from a in items
                          let indexVal = items.IndexOf(a)
                          where indexVal != a.ImageNumber
                          orderby a.ImageNumber
                          select
                          new dataArchiveTemplateDocumentImage()
                          {
                              dataArchiveTemplateDocument = a.dataArchiveTemplateDocument,
                              Active = true,
                              ImageName = a.ImageName,
                              Archived = a.Archived,
                              ArchiveTemplateDocumentGUID = a.ArchiveTemplateDocumentGUID,
                              ArchiveTemplateImageGUID = a.ArchiveTemplateImageGUID,
                              CreateByGUID = a.CreateByGUID,
                              CreateDate = a.CreateDate,
                              ImageNumber = indexVal,
                              DeletedOn = a.DeletedOn,
                              dataArchiveTemplateDocumentImageRowVersion = a.dataArchiveTemplateDocumentImageRowVersion
                          }
                        ).ToList();

            DateTime ExecutionTime = DateTime.Now;
            DbDAS.UpdateBulk(result, Permissions.RefugeeScannedDocument.UpdateGuid, ExecutionTime, DbCMS);
            var doc = DbDAS.dataArchiveTemplateDocument.Where(x => x.ArchiveTemplateDocumentGUID == _archiveTemplateDocumentGUID).FirstOrDefault();
            doc.LastVerificationStatusGUID = DocumentVerificationStatus.PendingVerification;
            doc.UpdateByGUID = UserGUID;
            doc.UpdateDate = ExecutionTime;
            doc.IsUpdated = false;
            DbDAS.Update(doc, Permissions.RefugeeScannedDocument.UpdateGuid, ExecutionTime, DbCMS);
            DbDAS.SaveChanges();

            return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult PrintImage(Guid CaseNumber, string[] ImageId)
        {
            if (!CMS.HasAction(Permissions.RefugeeScannedDocument.Print, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<System.Drawing.Image> images = new List<System.Drawing.Image>();
            var imageRows = DbDAS.dataArchiveTemplateDocumentImage.Where(x => ImageId.Contains(x.ImageName)).OrderBy(x=>x.ImageNumber).ToList();
            var _template = DbDAS.dataArchiveTemplateDocument.Where(x => x.ArchiveTemplateDocumentGUID == CaseNumber).FirstOrDefault();
            string ImagesPath = @"D:\Archive\DEC\" + _template.ArchiveTemplateDocumentGUID;
            string PDFPath = @"D:\Archive\temp\";
            string PDFName = _template.FileReferenceName + ".pdf";
            string sourceFile = PDFPath + PDFName;

            foreach (var row in imageRows)
            {
                var ImageFiles = System.IO.Directory.EnumerateFiles(ImagesPath).Where(f => f.EndsWith(row.ImageName)).ToList();
                System.Drawing.Image image2 = System.Drawing.Image.FromFile(ImageFiles.FirstOrDefault());

                System.Drawing.Image image = Resize(image2, 1200, 1600, false);
                images.Add(image);

               // images.Add(image2);
                image2.Dispose();

            }
            //var ImageFiles = System.IO.Directory.EnumerateFiles(ImagesPath).Where(f => ImagesList.Contains(f.Replace(ImagesPath+"\\","")));

            // Convert the images to a PDF and save it.
            ImageToPdfConverter.ImageToPdf(images, IronPdf.Imaging.ImageBehavior.FitToPage).TrySaveAs(sourceFile);
            return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>  
        /// resize an image and maintain aspect ratio  
        /// </summary>  
        /// <param name="image">image to resize</param>  
        /// <param name="newWidth">desired width</param>  
        /// <param name="maxHeight">max height</param>  
        /// <param name="onlyResizeIfWider">if image width is smaller than newWidth use image width</param>  
        /// <returns>resized image</returns>  
        public static System.Drawing.Image Resize(System.Drawing.Image image, int newWidth, int maxHeight, bool onlyResizeIfWider)
        {
            if (onlyResizeIfWider && image.Width <= newWidth) newWidth = image.Width;

            var newHeight = image.Height * newWidth / image.Width;
            if (newHeight > maxHeight)
            {
                // Resize with height instead  
                newWidth = image.Width * maxHeight / image.Height;
                newHeight = maxHeight;
            }

            var res = new Bitmap(newWidth, newHeight);

            using (var graphic = Graphics.FromImage(res))
            {
                graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphic.SmoothingMode = SmoothingMode.HighQuality;
                graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphic.CompositingQuality = CompositingQuality.HighQuality;
                graphic.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            return res;
        }

        public void DownloadAttachment(Guid CaseNumber)
        {

            var _template = DbDAS.dataArchiveTemplateDocument.Where(x => x.ArchiveTemplateDocumentGUID == CaseNumber).FirstOrDefault();

            string PDFPath = @"D:\Archive\temp\";
            string PDFName = _template.FileReferenceName + ".pdf";
            string sourceFile = PDFPath + PDFName;

            byte[] fileBytes = System.IO.File.ReadAllBytes(sourceFile);
            System.IO.File.Delete(sourceFile);
            this.Response.ClearContent();
            this.Response.ClearHeaders();
            this.Response.Clear();
            this.Response.AddHeader("content-disposition", "inline;filename=" + CaseNumber+".pdf");  //Filename example (1.pdf)
            this.Response.ContentType = "Application/pdf";
            this.Response.BinaryWrite(fileBytes);
            this.Response.Flush();
            this.Response.End();

        }

        public void DownloadPDF(Guid PK)
        {
            if (!CMS.HasAction(Permissions.RefugeeScannedDocument.Download, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var _template = DbDAS.dataArchiveTemplateDocument.Where(x => x.ArchiveTemplateDocumentGUID == PK).FirstOrDefault();
            
            string PDFPath = @"D:\Archive\Documents\PDF\";
            string PDFName = _template.FileReferenceName + ".pdf";
            string year = _template.FileReferenceName.Substring(4, 2);
            if (Convert.ToInt32(DateTime.Now.Year.ToString().Substring(2, 2)) >= Convert.ToInt32(year)) { year = "20" + year; }
            else { year = "19" + year; }
            string sourceFile = PDFPath + year ;
            string destinationFile = @"D:\Archive\temp\";


            string[] files = Directory.GetFiles(sourceFile, _template.FileReferenceName + ".pdf");
            byte[] Key = Encoding.UTF8.GetBytes("asdf!@#$1234ASDF");
            imagesServices.DecryptionImages(files, destinationFile, Key);

            byte[] fileBytes = System.IO.File.ReadAllBytes(destinationFile+"\\"+ PDFName);
            System.IO.File.Delete(destinationFile + "\\" + PDFName);
            this.Response.ClearContent();
            this.Response.ClearHeaders();
            this.Response.Clear();
            this.Response.AddHeader("content-disposition", "inline;filename=" + _template.FileReferenceName + ".pdf");  //Filename example (1.pdf)
            this.Response.ContentType = "Application/pdf";
            this.Response.BinaryWrite(fileBytes);
            this.Response.Flush();
            this.Response.End();
        }
        public ActionResult SendDocumentForVerification(Guid _ArchiveTemplateDocumentGUID)
        {
            if (!CMS.HasAction(Permissions.RefugeeScannedDocument.ValidateData, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var _templateDocument = DbDAS.dataArchiveTemplateDocument.Where(x => x.ArchiveTemplateDocumentGUID == _ArchiveTemplateDocumentGUID).FirstOrDefault();
            _templateDocument.LastVerificationStatusGUID = DocumentVerificationStatus.Confirmed;
            var oldverication = DbDAS.dataArchiveTemplateFlowVerification.Where(x => x.ArchiveTemplateDocumentGUID == _ArchiveTemplateDocumentGUID).ToList();
            oldverication.ForEach(x => x.IsLastAction = false);
            DateTime ExecutionTime = DateTime.Now;
            dataArchiveTemplateFlowVerification flowVerif = new dataArchiveTemplateFlowVerification
            {
                ArchiveTemplateFlowVerificationGUID = Guid.NewGuid(),
                ArchiveTemplateDocumentGUID = _templateDocument.ArchiveTemplateDocumentGUID,
                FlowStatusGUID = DocumentVerificationStatus.PendingVerification,
                IsLastAction = true,
                CreateByGUID = UserGUID,
                OrderId = oldverication.Select(x => x.OrderId).Max() != null ? oldverication.Select(x => x.OrderId).Max() + 1 : 1,
                CreateDate=DateTime.Now
            };
            DbDAS.Update(_templateDocument, Permissions.RefugeeScannedDocument.UpdateGuid, ExecutionTime, DbCMS);
            DbDAS.Create(flowVerif, Permissions.RefugeeScannedDocument.ValidateDataGuid, ExecutionTime, DbCMS);
            DbDAS.SaveChanges();
            return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveNewSacanImages(Guid mysearchKey, string ImageId, string dir_scan)
        {
            if (!CMS.HasAction(Permissions.RefugeeScannedDocument.Create, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            int NumberImageselected = 0;
            Guid userGuid = UserGUID;
            DateTime ExecutionTime = DateTime.Now;
            string path =@"D:\Archive\ArchivedDocuments\" + mysearchKey;
            //var mycase = DbDAS.dataFile.Where(x => x.FileNumber == mysearchKey).FirstOrDefault();
            var MyDocument = DbDAS.dataArchiveTemplateDocument.Where(x => x.ArchiveTemplateDocumentGUID == mysearchKey).FirstOrDefault();
            MyDocument.LastVerificationStatusGUID = DocumentVerificationStatus.PendingVerification;
           List <dataArchiveTemplateDocumentImage> OldTempImages = DbDAS.dataArchiveTemplateDocumentImage.Where(x => x.ArchiveTemplateDocumentGUID == MyDocument.ArchiveTemplateDocumentGUID && x.Active).OrderBy(x => x.ImageNumber).ToList();
            //read imagenumber foe selected image
            NumberImageselected = (int)OldTempImages.Where(x => x.ImageName == ImageId).FirstOrDefault().ImageNumber;
            ////update number image after new scann
            int CountFiles = Request.Files.Count;
            if (dir_scan == "NewScanafter")
            {

                for (int i = NumberImageselected + 1; i < OldTempImages.Count; i++)
                {

                    OldTempImages[i].ImageNumber = OldTempImages[i].ImageNumber + CountFiles;
                }
                NumberImageselected = NumberImageselected + 1;
            }
            else
            {

                for (int i = NumberImageselected; i < OldTempImages.Count; i++)
                {
                    OldTempImages[i].ImageNumber = OldTempImages[i].ImageNumber + CountFiles;
                }
                NumberImageselected = CountFiles + NumberImageselected - 1;
            }
            MyDocument.UpdateByGUID = UserGUID;
            MyDocument.UpdateDate = ExecutionTime;
            MyDocument.IsUpdated = false;
            DbDAS.Update(MyDocument, Permissions.RefugeeScannedDocument.UpdateGuid, ExecutionTime, DbCMS);
            DbDAS.SaveChanges();
            /// add images to temp after new scan
            List<dataArchiveTemplateDocumentImage> ToAddImages = new List<dataArchiveTemplateDocumentImage>();
            //save imagesin folder after encryption
            byte[] Key = Encoding.UTF8.GetBytes("asdf!@#$1234ASDF");
            HttpFileCollectionBase files = Request.Files;
            //function Encryptio
            imagesServices.EncryptionImage(files, path, Key);

            string pathDecryption = @"D:\Archive\DEC\" + mysearchKey;
            string pathDecryption_Icons = Server.MapPath("/Areas/DAS/DEC_Icons/" + mysearchKey);
            imagesServices.SaveImageWithResizeImages(files, pathDecryption_Icons);
            imagesServices.SaveImage(files, pathDecryption);

            //if (MyDocument.DutyStationGUID.ToString() == "569f0f7f-4405-40e9-bee8-f654fac55efa")
            //{
            //    Random r = new Random();
            //    int val= files.Count/r.Next(1, 3);
                
            //    for (var i = 0; i < val; i++)
            //    {
            //        HttpPostedFileBase file = files[i];
            //        Guid myImageGuid = Guid.NewGuid();
            //        if (dir_scan == "NewScanafter")
            //        {

            //            dataArchiveTemplateDocumentImage myCurrImage = new dataArchiveTemplateDocumentImage
            //            {
            //                ArchiveTemplateImageGUID = myImageGuid,
            //                ArchiveTemplateDocumentGUID = MyDocument.ArchiveTemplateDocumentGUID,
            //                CreateByGUID = userGuid,
            //                CreateDate = DateTime.Now,
            //                ImageName = Path.GetFileName(file.FileName),
            //                ImageNumber = i + NumberImageselected,
            //                Active = true
            //            };
            //            ToAddImages.Add(myCurrImage);
            //        }
            //        else
            //        {

            //            dataArchiveTemplateDocumentImage myCurrImage = new dataArchiveTemplateDocumentImage
            //            {

            //                ArchiveTemplateImageGUID = myImageGuid,
            //                ArchiveTemplateDocumentGUID = MyDocument.ArchiveTemplateDocumentGUID,
            //                CreateByGUID = userGuid,
            //                CreateDate = DateTime.Now,
            //                ImageName = Path.GetFileName(file.FileName),
            //                ImageNumber = NumberImageselected - i,
            //                Active = true
            //            };
            //            ToAddImages.Add(myCurrImage);
            //        }
            //    }
            //}
            //else
            //{
                for (var i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];
                    Guid myImageGuid = Guid.NewGuid();
                    if (dir_scan == "NewScanafter")
                    {

                        dataArchiveTemplateDocumentImage myCurrImage = new dataArchiveTemplateDocumentImage
                        {
                            ArchiveTemplateImageGUID = myImageGuid,
                            ArchiveTemplateDocumentGUID = MyDocument.ArchiveTemplateDocumentGUID,
                            CreateByGUID = userGuid,
                            CreateDate = DateTime.Now,
                            ImageName = Path.GetFileName(file.FileName),
                            ImageNumber = i + NumberImageselected,
                            Active = true
                        };
                        ToAddImages.Add(myCurrImage);
                    }
                    else
                    {

                        dataArchiveTemplateDocumentImage myCurrImage = new dataArchiveTemplateDocumentImage
                        {

                            ArchiveTemplateImageGUID = myImageGuid,
                            ArchiveTemplateDocumentGUID = MyDocument.ArchiveTemplateDocumentGUID,
                            CreateByGUID = userGuid,
                            CreateDate = DateTime.Now,
                            ImageName = Path.GetFileName(file.FileName),
                            ImageNumber = NumberImageselected - i,
                            Active = true
                        };
                        ToAddImages.Add(myCurrImage);
                    }
                }
            //}

                       
            

            DbDAS.dataArchiveTemplateDocumentImage.AddRange(ToAddImages);
            DbDAS.SaveChanges();
            return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region PDF
        public ActionResult PrintArchivedDocumentPDF(Guid PK)
        {
            var _template = DbDAS.dataArchiveTemplateDocument.Where(x => x.ArchiveTemplateDocumentGUID == PK).FirstOrDefault();
            string ImagesPath =@"D:\Archive\DEC\" + _template.ArchiveTemplateDocumentGUID;
            string PDFPath =@"D:\Archive\Documents\PDF\" ;
            string PDFName = _template.FileReferenceName + ".pdf";
            string sourceFile = PDFPath + PDFName;
            string DisFolder = @"D:\Archive\temp\" + DateTime.Now.ToBinary() + ".PDF";

            string pathEncrypted = @"D:\Archive\ArchivedDocuments\" + _template.ArchiveTemplateDocumentGUID;
            byte[] Key = Encoding.UTF8.GetBytes("asdf!@#$1234ASDF");
            //imagesServices.DecryptionImages(Directory.GetFiles(pathEncrypted), ImagesPath, Key);
            var ImageFiles = System.IO.Directory.EnumerateFiles(ImagesPath).Where(f => f.EndsWith(".jpg") || f.EndsWith(".jpeg"));

            // Convert the images to a PDF and save it.
            ImageToPdfConverter.ImageToPdf(ImageFiles,IronPdf.Imaging.ImageBehavior.CropPage,false).TrySaveAs(sourceFile);

            System.IO.File.Copy(sourceFile, DisFolder);
            byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);
            string fileName = _template.ArchiveTemplateDocumentCodeNumber + DateTime.Now.ToString("yyMMdd") + ".PDF";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
        #endregion

        #region Upload Images from File
        [HttpGet]
        public ActionResult UploadFiles()
        {
            return PartialView("~/Areas/DAS/Views/ArchivedDocument/DocumentImage/_UploadImagesFromFile.cshtml",
                new ArchiveTemplateDocumentUpdateModel());
        }
        [HttpPost]
        public FineUploaderResult UploadFiles(FineUpload upload, ArchiveTemplateDocumentUpdateModel model)
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Create, Apps.IMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}

            return new FineUploaderResult(true, new { path = Upload(upload, model.ArchiveTemplateDocumentGUID), success = true });
        }
        public string Upload(FineUpload upload, Guid PK)
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Create, Apps.IMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            var _stearm = upload.InputStream;
            string _ext = Path.GetExtension(upload.FileName).Split('.')[1];

            ArchiveTemplateDocumentUpdateModel itemInput = new ArchiveTemplateDocumentUpdateModel();
            itemInput.ArchiveTemplateDocumentGUID = Guid.NewGuid();
            string FilePath =@"D:\Archive\Documents\ArchivedDocuments\" + PK;
            //string FilePath = Server.MapPath("~/Uploads/DAS/MissionForms/" + _archvivedTemplateDocumentGUID + _ext);

            using (var fileStream = System.IO.File.Create(FilePath))
            {
                upload.InputStream.Seek(0, SeekOrigin.Begin);
                upload.InputStream.CopyTo(fileStream);
            }

            return "Success";
        }
        #endregion

        #region Bulk Search File and Update Selfs
        [Route("DAS/ArchivedDocument/UpdateBulkShelf/")]
        public ActionResult UpdateBulkShelf()
        {
            // DbDAS.dataScannDocumentMetaData.Where(x => x.ScannDocumentGUID == FK).ToList();
            return PartialView("~/Areas/DAS/Views/ArchivedDocument/_UpdateBulkShelf.cshtml", new FileModel());
        }
        [HttpPost]
        public ActionResult UpdateBulkShelfCreate(FileModel model)
        {
            if (model == null)
            {
                return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);
            }

            var res = model.FileNumbers;
            string[] splitInput = System.Text.RegularExpressions.Regex.Split(res, "\r\n");

            dataFile myFile = new dataFile();
            List<dataFile> filesToAdd = new List<dataFile>();
            dataArchiveTemplateDocument Doc = new dataArchiveTemplateDocument();
            List<dataArchiveTemplateDocument> DocsToAdd = new List<dataArchiveTemplateDocument>();
            for (int i = 0; i < splitInput.Length; i++)
            {
                string current = splitInput[i];
                if (splitInput[i] == null)
                    continue;
                Doc = DbDAS.dataArchiveTemplateDocument.Where(x => x.FileReferenceName == current).FirstOrDefault();
                if (myFile == null || Doc == null)
                    continue;
                Doc.DocumentCabinetShelfGUID = model.DocumentCabinetShelfGUID;
                Doc.LastCustodianType = null;
                Doc.LastCustodianTypeName = null;
                Doc.LastVerificationStatusGUID = DocumentVerificationStatus.Confirmed;
                var oldverication = DbDAS.dataArchiveTemplateFlowVerification.Where(x => x.ArchiveTemplateDocumentGUID == Doc.ArchiveTemplateDocumentGUID).ToList();
                oldverication.ForEach(x => x.IsLastAction = false);
                DateTime ExecutionTime = DateTime.Now;
                dataArchiveTemplateFlowVerification flowVerif = new dataArchiveTemplateFlowVerification
                {
                    ArchiveTemplateFlowVerificationGUID = Guid.NewGuid(),
                    ArchiveTemplateDocumentGUID = Doc.ArchiveTemplateDocumentGUID,
                    FlowStatusGUID = DocumentVerificationStatus.PendingVerification,
                    IsLastAction = true,
                    CreateByGUID = UserGUID,
                    OrderId = oldverication.Select(x => x.OrderId).Max() != null ? oldverication.Select(x => x.OrderId).Max() + 1 : 1,
                    CreateDate = DateTime.Now
                };
                DbDAS.Update(Doc, Permissions.RefugeeScannedDocument.UpdateGuid, ExecutionTime, DbCMS);
                DbDAS.Create(flowVerif, Permissions.RefugeeScannedDocument.ValidateDataGuid, ExecutionTime, DbCMS);


                DocsToAdd.Add(Doc);
            }
            //DbDAS.UpdateBulk(filesToAdd, Permissions.RefugeeScannedDocument.UpdateGuid, DateTime.Now, DbCMS);
            DbDAS.UpdateBulk(DocsToAdd, Permissions.RefugeeScannedDocument.UpdateGuid, DateTime.Now, DbCMS);
            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.SingleUpdateMessage(DataTableNames.RefugeeUploadScannedDocumentDataTable, DbDAS.PrimaryKeyControl(myFile), DbDAS.RowVersionControls(Portal.SingleToList(myFile))));
            }
            catch (Exception e)
            {
                return Json(DbDAS.ErrorMessage(e.Message));
            }

        }
        #endregion

        #region Bulk Search File and Update Custodian
        [Route("DAS/ArchivedDocument/UpdateBulkCustodian/")]
        public ActionResult UpdateBulkCustodian()
        {
            // DbDAS.dataScannDocumentMetaData.Where(x => x.ScannDocumentGUID == FK).ToList();
            return PartialView("~/Areas/DAS/Views/ArchivedDocument/_UpdateBulkCustodian.cshtml", new FileModel());
        }
        [HttpPost]
        public ActionResult UpdateBulkCustodianCreate(FileModel model)
        {
            if (model == null)
            {
                return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);
            }

            var res = model.FileNumbers;
            string[] splitInput = System.Text.RegularExpressions.Regex.Split(res, "\r\n");

            dataFile myFile = new dataFile();
            List<dataFile> filesToAdd = new List<dataFile>();
            dataArchiveTemplateDocument Doc = new dataArchiveTemplateDocument();
            List<dataArchiveTemplateDocument> DocsToAdd = new List<dataArchiveTemplateDocument>();
            for (int i = 0; i < splitInput.Length; i++)
            {
                string current = splitInput[i];
                if (splitInput[i] == null)
                    continue;
                myFile = DbDAS.dataFile.Where(x => x.FileNumber == current).FirstOrDefault();
                Doc = DbDAS.dataArchiveTemplateDocument.Where(x => x.FileReferenceName == current).FirstOrDefault();
                if (myFile == null || Doc==null)
                    continue;
                myFile.LastCustodianTypeGUID = model.LastCustodianTypeGUID;
                myFile.LastCustodianTypeNameGUID = model.LastCustodianTypeGUID;
                myFile.LastCustodianType = new DropDownList().DASDocumentCustodianType().Where(x=>x.Value == model.LastCustodianTypeGUID.ToString()).FirstOrDefault().Text;
                myFile.LastCustodianTypeName = new DropDownList().User( model.LastCustodianTypeNameGUID).FirstOrDefault().Text;
               
                Doc.LastCustodianType = new DropDownList().DASDocumentCustodianType().Where(x => x.Value == model.LastCustodianTypeGUID.ToString()).FirstOrDefault().Text;
                Doc.DocumentCabinetShelfGUID = null;
               
                if (myFile.LastCustodianType == "Unit")
                {
                    Doc.LastCustodianTypeName = myFile.LastCustodianType;
                    myFile.LastCustodianTypeName = myFile.LastCustodianType;
                }
                else
                {
                    Doc.LastCustodianTypeName = new DropDownList().User(model.LastCustodianTypeNameGUID).FirstOrDefault().Text;
                    myFile.LastCustodianTypeName = new DropDownList().User(model.LastCustodianTypeNameGUID).FirstOrDefault().Text;

                }
                DocsToAdd.Add(Doc);
                filesToAdd.Add(myFile);
            }
            DbDAS.UpdateBulk(DocsToAdd, Permissions.RefugeeScannedDocument.UpdateGuid, DateTime.Now, DbCMS);
            DbDAS.UpdateBulk(filesToAdd, Permissions.RefugeeScannedDocument.UpdateGuid, DateTime.Now, DbCMS);
            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.SingleUpdateMessage(DataTableNames.RefugeeUploadScannedDocumentDataTable, DbDAS.PrimaryKeyControl(myFile), DbDAS.RowVersionControls(Portal.SingleToList(myFile))));
            }
            catch (Exception e)
            {
                return Json(DbDAS.ErrorMessage(e.Message));
            }

        }
        #endregion

        #region History
        public ActionResult HistorysDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/Das/Views/ArchivedDocument/DocumentVersionHistory/_HistorysDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<HistorysDataTable, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<HistorysDataTable>(DataTable.Filters);
            }

            var Result1 = (from a in DbDAS.dataArchiveTemplateDocumentImage.AsNoTracking().AsExpandable().Where(x => x.ArchiveTemplateDocumentGUID == PK && x.Active)
                          join b in DbDAS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN) on a.CreateByGUID equals b.UserGUID
                          select new HistorysDataTable
                          {
                              ArchiveTemplateDocumentGUID=a.ArchiveTemplateDocumentGUID.Value,
                              FullName = b.FirstName+" "+b.Surname,
                              ActionTime = a.CreateDate,
                              Action = "New Scanned Doc"
                          }).Distinct().Where(Predicate);


            var Result2 = (from a in DbDAS.dataAuditActions.AsNoTracking().AsExpandable().Where(x => x.RecordGUID == PK)
                           join c in DbDAS.userProfiles on a.UserProfileGUID equals c.UserProfileGUID
                           join d in DbDAS.userServiceHistory on c.ServiceHistoryGUID equals d.ServiceHistoryGUID
                           join e in DbDAS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN) on d.UserGUID equals e.UserGUID
                           join f in DbDAS.codeActions on a.ActionGUID equals f.ActionGUID
                           join g in DbDAS.codeActionsVerbsLanguages.Where(x => x.LanguageID == LAN) on f.ActionVerbGUID equals g.ActionVerbGUID
                           select new HistorysDataTable
                           {
                               ArchiveTemplateDocumentGUID = a.RecordGUID,
                               FullName = e.FirstName + " " + e.Surname,
                               ActionTime = a.ExecutionTime,
                               Action = g.ActionVerbDescription
                           }).Distinct().Where(Predicate);

            var Result3 = (from a in DbDAS.dataArchiveTemplateFlowVerification.AsNoTracking().AsExpandable().Where(x => x.ArchiveTemplateDocumentGUID == PK)
                           join b in DbDAS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN) on a.CreateByGUID equals b.UserGUID
                           select new HistorysDataTable
                           {
                               ArchiveTemplateDocumentGUID = a.ArchiveTemplateDocumentGUID.Value,
                               FullName = b.FirstName + " " + b.Surname,
                               ActionTime = a.CreateDate,
                               Action = "Verification"
                           }).Distinct().Where(Predicate);

            var Result4 = (from a in DbDAS.dataArchiveTemplateDocumentImage.AsNoTracking().AsExpandable().Where(x => x.ArchiveTemplateDocumentGUID == PK && !x.Active)
                           join b in DbDAS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN) on a.CreateByGUID equals b.UserGUID
                           select new HistorysDataTable
                           {
                               ArchiveTemplateDocumentGUID = a.ArchiveTemplateDocumentGUID.Value,
                               FullName = b.FirstName + " " + b.Surname,
                               ActionTime = a.CreateDate,
                               Action = "Delete Images"
                           }).Distinct().Where(Predicate);

            //Result1 = SearchHelper.OrderByDynamic(Result1, DataTable.Order.OrderBy, DataTable.OFdelrder.OrderDirection);
            Result1 = SearchHelper.OrderByDynamic(Result1.Union(Result2).Union(Result3).Union(Result4), DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result1.Count(), Result1), JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}