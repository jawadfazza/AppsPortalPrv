using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.ViewModels;
using AutoMapper;
using LinqKit;
using RMS_DAL.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.RMS.Controllers
{
    public class OidController : RMSBaseController
    {
        #region OID

        public ActionResult Index()
        {
            return View();
        }

        [Route("RMS/Oids/")]
        public ActionResult OidsIndex()
        {
            if (!CMS.HasAction(Permissions.Oid.Access, Apps.RMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/RMS/Views/Oids/Index.cshtml");
        }

        //[Route("RMS/OidsDataTable/")]
        public JsonResult OidsDataTable(DataTableRecievedOptions options)
        {
            var app = DbRMS.codeOID.ToList();
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<OidsDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<OidsDataTableModel>(DataTable.Filters);
            }
            

            var All = (from a in DbRMS.codeOID.AsExpandable()
                       join b in DbRMS.codeTablesValuesLanguages.Where(x=>x.LanguageID==LAN) on a.OIDTypeGUID equals b.ValueGUID
                       join c in DbRMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN) on a.PrinterTypeGUID equals c.ValueGUID
                       join d in DbRMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN) on a.PrinterModelGUID equals d.ValueGUID

                       select new OidsDataTableModel
                       {
                          OIDGUID=a.OidGUID,
                          Active=a.Active,
                          codeOIDRowVersion=a.codeOIDRowVersion,
                          OIDDescription=a.OIDDescription,
                          ValueType=a.ValueType,
                          OIDNumber=a.OIDNumber,
                          OIDTypeDescription=b.ValueDescription,
                          PrinterModelDescription= d.ValueDescription,
                          PrinterTypeDescription= c.ValueDescription,
                          OIDTypeGUID=b.ValueGUID.ToString(),
                          PrinterModelGUID=d.ValueGUID.ToString(),
                          PrinterTypeGUID=c.ValueGUID.ToString()
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<OidsDataTableModel> Result = Mapper.Map<List<OidsDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("RMS/Oids/Create/")]
        public ActionResult OidCreate()
        {
            if (!CMS.HasAction(Permissions.Oid.Create, Apps.RMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/RMS/Views/Oids/Oid.cshtml", new OidUpdateModel());
        }

        [Route("RMS/Oids/Update/{PK}")]
        public ActionResult OidUpdate(Guid PK)
        {
            //if (!CMS.HasAction(Permissions.Oid.Access, Apps.RMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            var model = (from a in DbRMS.codeOID.WherePK(PK)
                         select new OidUpdateModel
                         {
                             OidGUID=a.OidGUID,
                             Active=a.Active,
                             codeOIDRowVersion=a.codeOIDRowVersion,
                             OIDDescription=a.OIDDescription,
                             OIDNumber=a.OIDNumber,
                             ValueType=a.ValueType,
                             OIDTypeGUID=a.OIDTypeGUID,
                             PrinterModelGUID=a.PrinterModelGUID,
                             PrinterTypeGUID=a.PrinterTypeGUID,
                            IsImport=a.IsImport
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("Oid", "Oids", new { Area = "RMS" }));

            return View("~/Areas/RMS/Views/Oids/Oid.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OidCreate(OidUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.Oid.Create, Apps.RMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveOid(model)) return PartialView("~/Areas/RMS/Views/Oids/_OidForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            codeOID Oid = Mapper.Map(model, new codeOID());
            Oid.OidGUID = EntityPK;
            DbRMS.Create(Oid, Permissions.Oid.CreateGuid, ExecutionTime, DbCMS);


            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.Oid.Create, Apps.RMS, new UrlHelper(Request.RequestContext).Action("Create", "Oids", new { Area = "RMS" })), Container = "OidFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.Oid.Update, Apps.RMS), Container = "OidFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.Oid.Delete, Apps.RMS), Container = "OidFormControls" });

            try
            {
                DbRMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbRMS.SingleCreateMessage(DbRMS.PrimaryKeyControl(Oid), DbRMS.RowVersionControls(Portal.SingleToList(Oid)), null, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbRMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OidUpdate(OidUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.Oid.Update, Apps.RMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveOid(model)) return PartialView("~/Areas/RMS/Views/Oids/_OidForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            codeOID Oid = Mapper.Map(model, new codeOID());
            DbRMS.Update(Oid, Permissions.Oid.UpdateGuid, ExecutionTime, DbCMS);
            try
            {
                DbRMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbRMS.SingleUpdateMessage(null, null, DbRMS.RowVersionControls(DbRMS.RowVersionControls(Portal.SingleToList(Oid)))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyOid(model.OidGUID);
            }
            catch (Exception ex)
            {
                return Json(DbRMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OidDelete(codeOID model)
        {
            if (!CMS.HasAction(Permissions.Oid.Delete, Apps.RMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeOID> DeletedOid = DeleteOids(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.Oid.Restore, Apps.RMS), Container = "OidFormControls" });

            try
            {
                int CommitedRows = DbRMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbRMS.SingleDeleteMessage(CommitedRows, DeletedOid.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyOid(model.OidGUID);
            }
            catch (Exception ex)
            {
                return Json(DbRMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OidRestore(codeOID model)
        {
            if (!CMS.HasAction(Permissions.Oid.Restore, Apps.RMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveOid(model))
            {
                return Json(DbRMS.RecordExists());
            }

            List<codeOID> RestoredOids = RestoreOids(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.Oid.Create, Apps.RMS, new UrlHelper(Request.RequestContext).Action("Oids/Create", "Oid", new { Area = "RMS" })), Container = "OidFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.Oid.Update, Apps.RMS), Container = "OidFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.Oid.Delete, Apps.RMS), Container = "OidFormControls" });

            try
            {
                int CommitedRows = DbRMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbRMS.SingleRestoreMessage(CommitedRows, RestoredOids, DbRMS.PrimaryKeyControl(RestoredOids.FirstOrDefault()), null, null, UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyOid(model.OidGUID);
            }
            catch (Exception ex)
            {
                return Json(DbRMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult OidsDataTableDelete(List<codeOID> models)
        {
            if (!CMS.HasAction(Permissions.Oid.Delete, Apps.RMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeOID> DeletedOids = DeleteOids(models);

            try
            {
                DbRMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbRMS.PartialDeleteMessage(DeletedOids, models, DataTableNames.OidsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbRMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult OidsDataTableRestore(List<codeOID> models)
        {
            if (!CMS.HasAction(Permissions.Oid.Restore, Apps.RMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeOID> RestoredOids = RestoreOids(models);

            try
            {
                DbRMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbRMS.PartialRestoreMessage(RestoredOids, models, DataTableNames.OidsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbRMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeOID> DeleteOids(List<codeOID> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<codeOID> DeletedOids = new List<codeOID>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbRMS.QueryBuilder(models, Permissions.Oid.DeleteGuid, SubmitTypes.Delete, baseQuery);

            var Records = DbRMS.Database.SqlQuery<codeOID>(query).ToList();
            foreach (var record in Records)
            {
                DeletedOids.Add(DbRMS.Delete(record, ExecutionTime, Permissions.Oid.DeleteGuid, DbCMS));
            }

           
            return DeletedOids;
        }

        private List<codeOID> RestoreOids(List<codeOID> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<codeOID> RestoredOids = new List<codeOID>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbRMS.QueryBuilder(models, Permissions.Oid.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Records = DbRMS.Database.SqlQuery<codeOID>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveOid(record))
                {
                    RestoredOids.Add(DbRMS.Restore(record, Permissions.Oid.DeleteGuid, Permissions.Oid.RestoreGuid, RestoringTime, DbCMS));
                }
            }

           

            return RestoredOids;
        }

        private JsonResult ConcurrencyOid(Guid PK)
        {
            OidUpdateModel dbModel = new OidUpdateModel();

            var Oid = DbRMS.codeOID.Where(x => x.OidGUID == PK).FirstOrDefault();
            var dbOid = DbRMS.Entry(Oid).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbOid, dbModel);

            if (Oid.codeOIDRowVersion.SequenceEqual(dbModel.codeOIDRowVersion))
            {
                return Json(DbRMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbRMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveOid(Object model)
        {
            codeOID Oid = Mapper.Map(model, new codeOID());
            int OidDescription = DbRMS.codeOID
                                    .Where(x => x.OIDDescription == Oid.OIDDescription &&
                                                x.OIDNumber != Oid.OIDNumber &&
                                                x.Active).Count();
            if (OidDescription > 0)
            {
                ModelState.AddModelError("OidDescription", "Oid is already exists");
            }
            return (OidDescription > 0);
        }

        #endregion
    }
}