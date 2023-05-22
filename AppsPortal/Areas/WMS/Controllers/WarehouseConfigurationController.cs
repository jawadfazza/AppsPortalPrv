using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.ViewModels;
using AutoMapper;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WMS_DAL.Model;
using WMS_DAL.ViewModels;

namespace AppsPortal.Areas.WMS.Controllers
{
    public class WarehouseConfigurationController : WMSBaseController
    {
        // GET: WMS/WarehouseConfiguration

        #region Item Models

        public ActionResult WarehouseHome()
        {
            if (!CMS.HasAction(Permissions.STIConfiguration.Access, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return View("~/Areas/WMS/Views/WarehouseConfiguration/Index.cshtml");
        }


        [Route("WMS/Warehouses/")]
        public ActionResult WarehouseIndex()
        {
            if (!CMS.HasAction(Permissions.STIConfiguration.Access, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return View("~/Areas/WMS/Views/WarehouseConfiguration/Index.cshtml");
        }

        [Route("WMS/WarehousesDataTable/")]
        public JsonResult WarehousesDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<WarehouseDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<WarehouseDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.STIConfiguration.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix
            var All = (

                from a in DbWMS.codeWarehouse.Where(x => x.Active).AsExpandable()
                join b in DbWMS.codeWarehouseLanguage.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeWarehouse.DeletedOn) && x.LanguageID == LAN) on a.WarehouseGUID equals b.WarehouseGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()

                join d in DbWMS.codeWarehouseLocationLanguage.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeWarehouseLocation.DeletedOn) && x.LanguageID == LAN)
                               on a.WarehouseLocationGUID equals d.WarehouseLocationGUID into LJ2
                from R2 in LJ2.DefaultIfEmpty()


                join e in DbWMS.codeDutyStationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.DutyStationGUID equals e.DutyStationGUID into LJ3
                from R3 in LJ3.DefaultIfEmpty()

                join f in DbWMS.codeOrganizationsInstancesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.OrganizationInstanceGUID equals f.OrganizationInstanceGUID into LJ4
                from R4 in LJ4.DefaultIfEmpty()


                select new WarehouseDataTableModel
                {
                    WarehouseGUID = a.WarehouseGUID.ToString(),
                    Active = a.Active,
                    WarehouseDescription = R1.WarehouseDescription,
                    DutyStationGUID = a.DutyStationGUID.ToString(),
                    OrganizationInstance = R4.OrganizationInstanceDescription,




                    codeWarehouseRowVersion = a.codeWarehouseRowVersion
                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<WarehouseDataTableModel> Result = Mapper.Map<List<WarehouseDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("WMS/Warehouses/Create/")]
        public ActionResult WarehouseCreate()
        {
            if (!CMS.HasAction(Permissions.STIConfiguration.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return View("~/Areas/WMS/Views/WarehouseConfiguration/Warehouses.cshtml", new WarehouseUpdateModel());
        }

        [Route("WMS/Warehouses/Update/{PK}")]
        public ActionResult WarehouseUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.STIConfiguration.Access, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            string en = "EN";
            var model = (from a in DbWMS.codeWarehouse.Where(x => x.Active).WherePK(PK)

                         select new WarehouseUpdateModel
                         {
                             WarehouseGUID = a.WarehouseGUID,
                             OrganizationInstanceGUID = a.OrganizationInstanceGUID,
                             DutyStationGUID = a.DutyStationGUID,
                             LocationGUID = a.LocationGUID,
                             ParentGUID = a.ParentGUID,
                             WarehouseTypeGUID = a.WarehouseTypeGUID,
                             WarehouseLocationGUID = a.WarehouseLocationGUID,
                             WarehouseDescription = a.codeWarehouseLanguage.Where(x => x.LanguageID == en && x.Active).FirstOrDefault().WarehouseDescription,


                             Active = a.Active,
                             codeWarehouseRowVersion = a.codeWarehouseRowVersion,

                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("Warehouses", "Warehouses", new { Area = "WMS" }));

            return View("~/Areas/WMS/Views/WarehouseConfiguration/Warehouses.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult WarehouseCreate(WarehouseUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.STIConfiguration.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (!ModelState.IsValid || ActiveWarehouse(model)) return PartialView("~/Areas/WMS/Views/WarehouseConfiguration/_WarehouseForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            codeWarehouse Warehouse = Mapper.Map(model, new codeWarehouse());
            Warehouse.WarehouseGUID = EntityPK;
            Warehouse.LocationGUID = model.WarehouseLocationGUID;
            DbWMS.Create(Warehouse, Permissions.STIConfiguration.CreateGuid, ExecutionTime, DbCMS);

            codeWarehouseLanguage Language = Mapper.Map(model, new codeWarehouseLanguage());
            Language.WarehouseGUID = EntityPK;
            Language.WarehouseDescription = model.WarehouseDescription;



            DbWMS.Create(Language, Permissions.STIConfiguration.CreateGuid, ExecutionTime, DbCMS);


            //if (model.UploadModelImage != null)
            //{

            //    var filePath = Server.MapPath("~\\Uploads\\WMS\\ItemImages\\");
            //    string extension = System.IO.Path.GetExtension(model.UploadModelImage.FileName);
            //    string fullFileName = filePath + "\\" + EntityPK + extension;
            //    model.UploadModelImage.SaveAs(fullFileName);
            //}

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.WarehouseFocalPointsDataTable, ControllerContext, "WarehouseFocalPointsDataTable"));
            //Partials.Add(Portal.PartialView(EntityPK, DataTableNames.WarehouseDeterminantsDataTable, ControllerContext, "ModelsDeterminantsFormControls"));
            //Partials.Add(Portal.PartialView(EntityPK, DataTableNames.WarehouseWarehouseDataTable, ControllerContext, "ModelsWarehousesFormControls"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.STIConfiguration.Create, Apps.WMS, new UrlHelper(Request.RequestContext).Action("Create", "Warehouses", new { Area = "WMS" })), Container = "WarehouseDetailFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.STIConfiguration.Update, Apps.WMS), Container = "WarehouseDetailFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.STIConfiguration.Delete, Apps.WMS), Container = "WarehouseDetailFormControls" });

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleCreateMessage(DbWMS.PrimaryKeyControl(Warehouse), DbWMS.RowVersionControls(Warehouse, Language), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }



        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult WarehouseUpdate(WarehouseUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.STIConfiguration.Update, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (!ModelState.IsValid) return PartialView("~/Areas/WMS/Views/WarehouseConfiguration/_WarehouseForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            codeWarehouse Warehouse = Mapper.Map(model, new codeWarehouse());
            DbWMS.Update(Warehouse, Permissions.STIConfiguration.UpdateGuid, ExecutionTime, DbCMS);

            var Language = DbWMS.codeWarehouseLanguage.Where(l => l.WarehouseGUID == model.WarehouseGUID && l.Active && l.LanguageID == LAN).FirstOrDefault();




            if (Language == null)
            {
                Language = Mapper.Map(model, Language);
                Language.WarehouseGUID = Warehouse.WarehouseGUID;
                DbWMS.Create(Language, Permissions.STIConfiguration.CreateGuid, ExecutionTime, DbCMS);
            }


            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleUpdateMessage(null, null, DbWMS.RowVersionControls(Warehouse, Language)));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyWarehouse((Guid)model.WarehouseGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult WarehouseDelete(codeWarehouse model)
        {
            if (!CMS.HasAction(Permissions.STIConfiguration.Delete, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<codeWarehouse> DeletedWarehouse = DeleteWarehouse(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.STIConfiguration.Restore, Apps.WMS), Container = "WarehouseFormControls" });

            try
            {
                int CommitedRows = DbWMS.SaveChanges();
                DbWMS.SaveChanges();
                return Json(DbWMS.SingleDeleteMessage(CommitedRows, DeletedWarehouse.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyWarehouse(model.WarehouseGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult WarehouseRestore(codeWarehouse model)
        {
            if (!CMS.HasAction(Permissions.STIConfiguration.Restore, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (ActiveWarehouse(model))
            {
                return Json(DbWMS.RecordExists());
            }

            List<codeWarehouse> RestoredWarehouse = RestoreWarehouse(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.STIConfiguration.Create, Apps.WMS, new UrlHelper(Request.RequestContext).Action("WarehouseCreate", "Configuration", new { Area = "WMS" })), Container = "WarehouseFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.STIConfiguration.Update, Apps.WMS), Container = "WarehouseFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.STIConfiguration.Delete, Apps.WMS), Container = "WarehouseFormControls" });

            try
            {
                int CommitedRows = DbWMS.SaveChanges();
                DbWMS.SaveChanges();
                return Json(DbWMS.SingleRestoreMessage(CommitedRows, RestoredWarehouse, DbWMS.PrimaryKeyControl(RestoredWarehouse.FirstOrDefault()), Url.Action(DataTableNames.WarehouseFocalPointsDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyWarehouse(model.WarehouseGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult WarehousesDataTableDelete(List<codeWarehouse> models)
        {
            if (!CMS.HasAction(Permissions.STIConfiguration.Delete, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<codeWarehouse> DeletedWarehouse = DeleteWarehouse(models);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.PartialDeleteMessage(DeletedWarehouse, models, DataTableNames.WarehousesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult WarehouseDataTableRestore(List<codeWarehouse> models)
        {
            if (!CMS.HasAction(Permissions.STIConfiguration.Restore, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<codeWarehouse> RestoredWarehouse = RestoreWarehouse(models);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.PartialRestoreMessage(RestoredWarehouse, models, DataTableNames.WarehousesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeWarehouse> DeleteWarehouse(List<codeWarehouse> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<codeWarehouse> DeletedWarehouse = new List<codeWarehouse>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = "SELECT WarehouseGUID,CONVERT(varchar(50), WarehouseGUID) as C2 ,codeWarehouseRowVersion FROM code.codeWarehouse where WarehouseGUID in (" + string.Join(",", models.Select(x => "'" + x.WarehouseGUID + "'").ToArray()) + ")";

            string query = DbWMS.QueryBuilder(models, Permissions.STIConfiguration.DeleteGuid, SubmitTypes.Delete, "");

            var Records = DbWMS.Database.SqlQuery<codeWarehouse>(query).ToList();
            foreach (var record in Records)
            {
                DeletedWarehouse.Add(DbWMS.Delete(record, ExecutionTime, Permissions.STIConfiguration.DeleteGuid, DbCMS));
            }

            var Languages = DeletedWarehouse.SelectMany(a => a.codeWarehouseLanguage).Where(l => l.Active).ToList();
            foreach (var language in Languages)
            {
                DbWMS.Delete(language, ExecutionTime, Permissions.STIConfiguration.DeleteGuid, DbCMS);
            }
            return DeletedWarehouse;
        }

        private List<codeWarehouse> RestoreWarehouse(List<codeWarehouse> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<codeWarehouse> RestoredWarehouse = new List<codeWarehouse>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = "SELECT WarehouseGUID,CONVERT(varchar(50), WarehouseGUID) as C2 ,codeWarehouseRowVersion FROM code.codeWarehouse where WarehouseGUID in (" + string.Join(",", models.Select(x => "'" + x.WarehouseGUID + "'").ToArray()) + ")";

            string query = DbWMS.QueryBuilder(models, Permissions.STIConfiguration.DeleteGuid, SubmitTypes.Restore, "");

            var Records = DbWMS.Database.SqlQuery<codeWarehouse>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveWarehouse(record))
                {
                    RestoredWarehouse.Add(DbWMS.Restore(record, Permissions.STIConfiguration.DeleteGuid, Permissions.STIConfiguration.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            var Languages = RestoredWarehouse.SelectMany(x => x.codeWarehouseLanguage.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var language in Languages)
            {
                DbWMS.Restore(language, Permissions.STIConfiguration.DeleteGuid, Permissions.STIConfiguration.RestoreGuid, RestoringTime, DbCMS);
            }

            return RestoredWarehouse;
        }

        private JsonResult ConcurrencyWarehouse(Guid PK)
        {
            WarehouseUpdateModel dbModel = new WarehouseUpdateModel();

            var Warehouse = DbWMS.codeWarehouse.Where(x => x.WarehouseGUID == PK).FirstOrDefault();
            var dbWarehouse = DbWMS.Entry(Warehouse).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbWarehouse, dbModel);

            var Language = DbWMS.codeWarehouseLanguage.Where(x => x.WarehouseGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeWarehouse.DeletedOn)).FirstOrDefault();
            var dbLanguage = DbWMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Warehouse.codeWarehouseRowVersion.SequenceEqual(dbModel.codeWarehouseRowVersion) && Language.codeWarehouseLanguageRowVersion.SequenceEqual(dbModel.codeWarehouseRowVersion))
            {
                return Json(DbWMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbWMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveWarehouse(Object model)
        {
            codeWarehouseLanguage Warehouse = Mapper.Map(model, new codeWarehouseLanguage());
            int ModelDescription = DbWMS.codeWarehouseLanguage
                                    .Where(x => x.WarehouseGUID == Warehouse.WarehouseGUID
                                      && x.WarehouseDescription == Warehouse.WarehouseDescription &&

                                                x.Active).Count();
            if (ModelDescription > 0)
            {
                ModelState.AddModelError("ModelDescription", "Warehouse is already exists");
            }
            return (ModelDescription > 0);
        }

        #endregion

        #region Item Models Language

        //[Route("WMS/WarehouseFocalPointsDataTable/{PK}")]
        public ActionResult WarehouseFocalPointsDataTable(DataTableRecievedOptions options, Guid PK)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<WarehouseFocalPointDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<WarehouseFocalPointDataTableModel>(DataTable.Filters);
            }

            var All = (

                from a in DbWMS.codeWarehouseFocalPoint.Where(x => x.Active && x.WarehouseGUID == PK).AsExpandable()
                join b in DbWMS.userPersonalDetailsLanguage.Where(x => (x.Active == true) && x.LanguageID == LAN) on a.UserGUID equals b.UserGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()

                    //join d in DbWMS.codeWarehouseLocationLanguage.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeWarehouseLocation.DeletedOn) && x.LanguageID == LAN)
                    //               on a.WarehouseLocationGUID equals d.WarehouseLocationGUID into LJ2
                    //from R2 in LJ2.DefaultIfEmpty()


                    //join e in DbWMS.codeDutyStationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.DutyStationGUID equals e.DutyStationGUID into LJ3
                    //from R3 in LJ3.DefaultIfEmpty()


                select new WarehouseFocalPointDataTableModel
                {
                    WarehouseFocalPointGUID = a.WarehouseFocalPointGUID.ToString(),
                    WarehouseLocationGUID = a.WarehouseGUID.ToString(),
                    Active = a.Active,
                    StaffName = R1.FirstName + " " + R1.Surname,

                    codeWarehouseFocalPointRowVersion = a.codeWarehouseFocalPointRowVersion
                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<WarehouseFocalPointDataTableModel> Result = Mapper.Map<List<WarehouseFocalPointDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult WarehouseFocalPointCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.STIConfiguration.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return PartialView("~/Areas/WMS/Views/WarehouseConfiguration/_FocalPointModal.cshtml",
                new codeWarehouseFocalPoint { WarehouseGUID = FK });
        }

        public ActionResult WarehouseFocalPointUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.STIConfiguration.Access, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            codeWarehouseFocalPoint model = DbWMS.codeWarehouseFocalPoint.Find(PK);
            model.IsFocalPoint = false;
            return PartialView("~/Areas/WMS/Views/WarehouseConfiguration/_FocalPointModal.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult WarehouseFocalPointCreate(codeWarehouseFocalPoint model)
        {
            if (!CMS.HasAction(Permissions.STIConfiguration.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (!ModelState.IsValid || ActiveWarehouseFocalPoint(model)) return PartialView("~/Areas/WMS/Views/WarehouseConfiguration/_FocalPointModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbWMS.Create(model, Permissions.STIConfiguration.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleUpdateMessage(DataTableNames.WarehouseFocalPointsDataTable, DbWMS.PrimaryKeyControl(model), DbWMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult WarehouseFocalPointUpdate(codeWarehouseFocalPoint model)
        {
            if (!CMS.HasAction(Permissions.STIConfiguration.Update, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (!ModelState.IsValid || ActiveWarehouseFocalPoint(model)) return PartialView("~/Areas/WMS/Views/WarehouseConfiguration/_FocalPointModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbWMS.Update(model, Permissions.STIConfiguration.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleUpdateMessage(DataTableNames.WarehouseFocalPointsDataTable,
                    DbWMS.PrimaryKeyControl(model),
                    DbWMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyWarehouseFocalPoint(model.WarehouseFocalPointGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult WarehouseFocalPointDelete(codeWarehouseFocalPoint model)
        {
            if (!CMS.HasAction(Permissions.STIConfiguration.Delete, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<codeWarehouseFocalPoint> DeletedLanguages = DeleteWarehouseFocalPoints(new List<codeWarehouseFocalPoint> { model });

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleDeleteMessage(DeletedLanguages, DataTableNames.WarehouseFocalPointsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyWarehouseFocalPoint(model.WarehouseFocalPointGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult WarehouseFocalPointRestore(codeWarehouseFocalPoint model)
        {
            if (!CMS.HasAction(Permissions.STIConfiguration.Restore, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (ActiveWarehouseFocalPoint(model))
            {
                return Json(DbWMS.RecordExists());
            }

            List<codeWarehouseFocalPoint> RestoredLanguages = RestoreWarehouseFocalPoints(Portal.SingleToList(model));

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleRestoreMessage(RestoredLanguages, DataTableNames.WarehouseFocalPointsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyWarehouseFocalPoint(model.WarehouseFocalPointGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult WarehouseFocalPointsDataTableDelete(List<codeWarehouseFocalPoint> models)
        {
            if (!CMS.HasAction(Permissions.STIConfiguration.Delete, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<codeWarehouseFocalPoint> DeletedLanguages = DeleteWarehouseFocalPoints(models);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.WarehouseFocalPointsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult WarehouseFocalPointsDataTableRestore(List<codeWarehouseFocalPoint> models)
        {
            if (!CMS.HasAction(Permissions.STIConfiguration.Restore, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<codeWarehouseFocalPoint> RestoredLanguages = RestoreWarehouseFocalPoints(models);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.WarehouseFocalPointsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeWarehouseFocalPoint> DeleteWarehouseFocalPoints(List<codeWarehouseFocalPoint> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<codeWarehouseFocalPoint> DeletedWarehouseFocalPoints = new List<codeWarehouseFocalPoint>();

            string query = DbWMS.QueryBuilder(models, Permissions.STIConfiguration.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbWMS.Database.SqlQuery<codeWarehouseFocalPoint>(query).ToList();

            foreach (var language in languages)
            {
                DeletedWarehouseFocalPoints.Add(DbWMS.Delete(language, ExecutionTime, Permissions.STIConfiguration.DeleteGuid, DbCMS));
            }

            return DeletedWarehouseFocalPoints;
        }

        private List<codeWarehouseFocalPoint> RestoreWarehouseFocalPoints(List<codeWarehouseFocalPoint> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<codeWarehouseFocalPoint> RestoredLanguages = new List<codeWarehouseFocalPoint>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbWMS.QueryBuilder(models, Permissions.STIConfiguration.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbWMS.Database.SqlQuery<codeWarehouseFocalPoint>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveWarehouseFocalPoint(language))
                {
                    RestoredLanguages.Add(DbWMS.Restore(language, Permissions.STIConfiguration.DeleteGuid, Permissions.STIConfiguration.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyWarehouseFocalPoint(Guid PK)
        {
            codeWarehouseFocalPoint dbModel = new codeWarehouseFocalPoint();

            var Language = DbWMS.codeWarehouseFocalPoint.Where(l => l.WarehouseFocalPointGUID == PK).FirstOrDefault();
            var dbLanguage = DbWMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.codeWarehouseFocalPointRowVersion.SequenceEqual(dbModel.codeWarehouseFocalPointRowVersion))
            {
                return Json(DbWMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbWMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveWarehouseFocalPoint(codeWarehouseFocalPoint model)
        {
            int LanguageID = DbWMS.codeWarehouseFocalPoint
                                  .Where(x => x.UserGUID == model.UserGUID &&
                                              x.WarehouseGUID == model.WarehouseGUID &&

                                              x.Active).Count();
            if (LanguageID > 0)
            {
                //Fix
                ModelState.AddModelError("LanguageID", "Telecom Company Name in selected language already exists");
            }

            return (LanguageID > 0);
        }

        #endregion Language
    }
}