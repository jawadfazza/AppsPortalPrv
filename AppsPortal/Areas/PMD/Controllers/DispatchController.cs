using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.ViewModels;
using AutoMapper;
using LinqKit;
using PMD_DAL.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace AppsPortal.Areas.PMD.Controllers
{
    public class DispatchController : PMDBaseController
    {
        #region Dispatch

        [Route("PMD/Dispatch/")]
        public ActionResult DispatchIndex()
        {
            if (!CMS.HasAction(Permissions.PMDDispatch.Access, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/PMD/Views/Dispatch/Index.cshtml");
        }

        [Route("PMD/DispatchDataTable/")]
        public JsonResult DispatchDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<DispatchDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<DispatchDataTableModel>(DataTable.Filters);
            }

            var All = (from a in DbPMD.dataPMDDispatch.AsExpandable()
                       join b in DbPMD.codeOrganizationsInstancesLanguages.Where(x => x.Active && x.LanguageID==LAN ) on a.OrganizationInstanceGUID equals b.OrganizationInstanceGUID into LJ1
                       from R1 in LJ1.DefaultIfEmpty()
                       join d in DbPMD.codeOchaLocationGovernorate on a.admin1Pcode equals d.admin1Pcode
                       select new DispatchDataTableModel
                       {
                          DispatchDate=a.DispatchDate,
                          admin1Pcode=d.admin1Name_en,
                          admin4Pcode= d.admin1Name_en,
                           GovernorateGUID =a.admin1Pcode,
                          OrganizationInstanceGUID=R1.OrganizationInstanceGUID.ToString(),
                          DispatchGUID=a.DispatchGUID,
                          OrganizationInstanceDescription=R1.OrganizationInstanceDescription,
                          Active = a.Active,
                          dataPMDDispatchRowVersion = a.dataPMDDispatchRowVersion,
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);
            List<DispatchDataTableModel> Result = Mapper.Map<List<DispatchDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("PMD/Dispatch/Create/")]
        public ActionResult DispatchCreate()
        {
            if (!CMS.HasAction(Permissions.PMDDispatch.Create, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/PMD/Views/Dispatch/Dispatch.cshtml", new DispatchUpdateModel());
        }


        [Route("PMD/Dispatch/Update/{PK}")]
        public ActionResult DispatchUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.PMDDispatch.Access, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            var model = (from x in DbPMD.dataPMDDispatch.Where(x => x.DispatchGUID == PK)
                         select new DispatchUpdateModel
                         {
                             admin1Pcode=x.admin1Pcode,
                             admin4Pcode=x.admin4Pcode,
                             DispatchDate=x.DispatchDate,
                             Comments=x.Comments,
                             DispatchGUID=x.DispatchGUID,
                             GovernorateGUID=x.GovernorateGUID,
                             OrganizationInstanceGUID=x.OrganizationInstanceGUID,
                             PmdWarehouseGUID=x.PmdWarehouseGUID,
                             Active = x.Active,
                             dataPMDDispatchRowVersion = x.dataPMDDispatchRowVersion,
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("Dispatch", "", new { Area = "PMD" }));
            return View("~/Areas/PMD/Views/Dispatch/Dispatch.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DispatchCreate(DispatchUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.PMDDispatch.Create, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            if (!ModelState.IsValid || ActiveDispatch(model)) return PartialView("~/Areas/PMD/Views/Dispatch/_DispatchForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;
            Guid EntityPK = Guid.NewGuid();
            if (model.DispatchGUID != Guid.Empty)
            {
                EntityPK = model.DispatchGUID;
            }

            dataPMDDispatch Dispatch = Mapper.Map(model, new dataPMDDispatch());
            Dispatch.DispatchGUID = EntityPK;
            Dispatch.GovernorateGUID = (Guid)DbPMD.codeOchaLocationGovernorate.Where(x => x.admin1Pcode == model.admin1Pcode).FirstOrDefault().GovernorateGUID;

            DbPMD.Create(Dispatch, Permissions.PMDDispatch.CreateGuid, ExecutionTime, DbCMS);


            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.DispatchDetailDataTable, ControllerContext, "DispatchDetailContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.PMDDispatch.Create, Apps.PMD, new UrlHelper(Request.RequestContext).Action("Dispatch/Create", "", new { Area = "PMD" })), Container = "DispatchFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.PMDDispatch.Update, Apps.PMD), Container = "DispatchFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.PMDDispatch.Delete, Apps.PMD), Container = "DispatchFormControls" });

            try
            {
                DbPMD.SaveChanges();
                return Json(DbPMD.SingleCreateMessage(DbPMD.PrimaryKeyControl(Dispatch), null, Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DispatchUpdate(DispatchUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.PMDDispatch.Update, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            if (!ModelState.IsValid || ActiveDispatch(model)) return PartialView("~/Areas/PMD/Views/Dispatch/_DispatchForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            dataPMDDispatch Dispatch = Mapper.Map(model, new dataPMDDispatch());
            Dispatch.GovernorateGUID = (Guid)DbPMD.codeOchaLocationGovernorate.Where(x => x.admin1Pcode == model.admin1Pcode).FirstOrDefault().GovernorateGUID;

            DbPMD.Update(Dispatch, Permissions.PMDDispatch.UpdateGuid, ExecutionTime, DbCMS);

            var DispatchDetail = DbPMD.dataPMDDispatchDetail.Where(x => x.DispatchGUID == model.DispatchGUID &&  x.Active).FirstOrDefault();

           
            try
            {
                DbPMD.SaveChanges();
                return Json(DbPMD.SingleUpdateMessage(null, null, DbPMD.RowVersionControls(Dispatch, DispatchDetail)));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyDispatch(model.DispatchGUID);
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DispatchDelete(dataPMDDispatch model)
        {
            if (!CMS.HasAction(Permissions.PMDDispatch.Delete, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            List<dataPMDDispatch> DeletedDispatch = DeleteDispatch(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.PMDDispatch.Restore, Apps.PMD), Container = "DispatchFormControls" });

            try
            {
                int CommitedRows = DbPMD.SaveChanges();
                return Json(DbPMD.SingleDeleteMessage(CommitedRows, DeletedDispatch.FirstOrDefault(), "DispatchDetailContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyDispatch(model.DispatchGUID);
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DispatchRestore(dataPMDDispatch model)
        {
            if (!CMS.HasAction(Permissions.PMDDispatch.Restore, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            if (ActiveDispatch(model))
            {
                return Json(DbPMD.RecordExists());
            }

            List<dataPMDDispatch> RestoredDispatch = RestoreDispatch(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.PMDDispatch.Create, Apps.PMD, new UrlHelper(Request.RequestContext).Action("Dispatch/Create", "", new { Area = "PMD" })), Container = "DispatchFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.PMDDispatch.Update, Apps.PMD), Container = "DispatchFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.PMDDispatch.Restore, Apps.PMD), Container = "DispatchFormControls" });

            try
            {
                int CommitedRows = DbPMD.SaveChanges();
                return Json(DbPMD.SingleRestoreMessage(CommitedRows, RestoredDispatch, DbPMD.PrimaryKeyControl(RestoredDispatch.FirstOrDefault()), Url.Action(DataTableNames.DispatchDetailDataTable, Portal.GetControllerName(ControllerContext)), "DispatchDetailContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyDispatch(model.DispatchGUID);
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult DispatchDataTableDelete(List<dataPMDDispatch> models)
        {
            if (!CMS.HasAction(Permissions.PMDDispatch.Delete, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataPMDDispatch> DeletedDispatch = DeleteDispatch(models);

            try
            {
                DbPMD.SaveChanges();
                return Json(DbPMD.PartialDeleteMessage(DeletedDispatch, models, DataTableNames.DispatchDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult DispatchDataTableRestore(List<dataPMDDispatch> models)
        {
            if (!CMS.HasAction(Permissions.PMDDispatch.Restore, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataPMDDispatch> RestoredDispatch = RestoreDispatch(models);

            try
            {
                DbPMD.SaveChanges();
                return Json(DbPMD.PartialRestoreMessage(RestoredDispatch, models, DataTableNames.DispatchDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        private List<dataPMDDispatch> DeleteDispatch(List<dataPMDDispatch> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<dataPMDDispatch> DeletedDispatch = new List<dataPMDDispatch>();

            //THIS IS FOR PERMISSION TEST//
            //DO NOT DELETE THIS CODE
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = (from a in DbPMD.dataPMDDispatch
            //                    from f in DbPMD.dataPMDDispatchFactorForTest
            //                    where a.DispatchGUID == f.DispatchGUID
            //                    select new { a.DispatchGUID, a.dataPMDDispatchRowVersion, f.OperationGUID, f.OrganizationGUID, f.CountryGUID }).AsQueryable().ToString();

            string query = DbPMD.QueryBuilder(models, Permissions.PMDDispatch.DeleteGuid, SubmitTypes.Delete, "");

            var Records = DbPMD.Database.SqlQuery<dataPMDDispatch>(query).ToList();
            foreach (var record in Records)
            {
                DeletedDispatch.Add(DbPMD.Delete(record, ExecutionTime, Permissions.PMDDispatch.DeleteGuid, DbCMS));
            }

            var DispatchDetail = DeletedDispatch.SelectMany(a => a.dataPMDDispatchDetail).Where(l => l.Active).ToList();
            foreach (var dispatchDetail in DispatchDetail)
            {
                DbPMD.Delete(dispatchDetail, ExecutionTime, Permissions.PMDDispatch.DeleteGuid, DbCMS);
            }
            return DeletedDispatch;
        }

        private List<dataPMDDispatch> RestoreDispatch(List<dataPMDDispatch> models)
        {
            Guid DeleteActionGUID = Permissions.PMDDispatch.DeleteGuid;
            Guid RestoreActionGUID = Permissions.PMDDispatch.RestoreGuid;
            DateTime RestoringTime = DateTime.Now;

            List<dataPMDDispatch> RestoredDispatch = new List<dataPMDDispatch>();

            //THIS IS FOR PERMISSION TEST//
            //DO NOT DELETE THIS CODE
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = (from a in DbPMD.dataPMDDispatch
            //                    from f in DbPMD.dataPMDDispatchFactorForTest
            //                    where a.DispatchGUID == f.DispatchGUID
            //                    select new
            //                    {
            //                        a.DispatchGUID,
            //                        a.dataPMDDispatchRowVersion,
            //                        C2 = f.OperationGUID + "," + f.OrganizationGUID,
            //                    }).AsQueryable().ToString();//.Replace("[C2]", "[FactorsToken]");

            string query = DbPMD.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, "");


            var Records = DbPMD.Database.SqlQuery<dataPMDDispatch>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveDispatch(record))
                {
                    RestoredDispatch.Add(DbPMD.Restore(record, DeleteActionGUID, RestoreActionGUID, RestoringTime, DbCMS));
                }
            }

            var DispatchDetail = RestoredDispatch.SelectMany(x => x.dataPMDDispatchDetail.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var dispatchDetail in DispatchDetail)
            {
               
                    DbPMD.Restore(dispatchDetail, DeleteActionGUID, RestoreActionGUID, RestoringTime, DbCMS);
                
            }

            return RestoredDispatch;
        }

        private JsonResult ConcrrencyDispatch(Guid PK)
        {
            DispatchUpdateModel dbModel = new DispatchUpdateModel();

            var Dispatch = DbPMD.dataPMDDispatch.Where(a => a.DispatchGUID == PK).FirstOrDefault();
            var dbDispatch = DbPMD.Entry(Dispatch).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbDispatch, dbModel);

            var DispatchDetail = DbPMD.dataPMDDispatchDetail.Where(x => x.DispatchGUID == PK).Where(p => (p.Active == true ? p.Active : p.DeletedOn == p.dataPMDDispatch.DeletedOn) ).FirstOrDefault();
            var dbDispatchDetail = DbPMD.Entry(DispatchDetail).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbDispatchDetail, dbModel);

            if (Dispatch.dataPMDDispatchRowVersion.SequenceEqual(dbModel.dataPMDDispatchRowVersion) && DispatchDetail.dataPMDDispatchDetailRowVersion.SequenceEqual(dbModel.dataPMDDispatchRowVersion))
            {
                return Json(DbPMD.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbPMD, dbModel, "DispatchDetailContainer"));
        }

        private bool ActiveDispatch(Object model)
        {
            dataPMDDispatch Dispatch = Mapper.Map(model, new dataPMDDispatch());
            dataPMDDispatchDetail DispatchDetail = Mapper.Map(model, new dataPMDDispatchDetail());

          
            return false;
        }

        public ActionResult DispatchAuditHistory(Guid RecordGUID)
        {
            List<Guid> ChildrenGUIDs = new List<Guid>();

            DbPMD.dataPMDDispatchDetail.AsNoTracking().Where(x => x.DispatchGUID == RecordGUID).ToList().ForEach(x => ChildrenGUIDs.Add(x.DispatchDetailGUID));
            ChildrenGUIDs.Add(RecordGUID);

            List<Guid> ChildrenActions = new List<Guid>
            {
                Permissions.PMDDispatch.UpdateGuid
            };
            return new AppsPortal.Controllers.AuditController().GetAuditHistoryGlobalizationVersion(RecordGUID, ChildrenActions, ChildrenGUIDs);
        }

        #endregion 

        #region Dispatch DispatchDetail
        public ActionResult DispatchDetailDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/PMD/Views/Dispatch/_DispatchDetailDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<dataPMDDispatchDetail, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<dataPMDDispatchDetail>(DataTable.Filters);
            }

            var Result =(from a in DbPMD.dataPMDDispatchDetail.AsNoTracking().AsExpandable().Where(x => x.DispatchGUID == PK).Where(Predicate)
                         join b in DbPMD.codePmd2023UnitOfAchievementLanguages.Where(x=>x.LanguageID==LAN && x.Active) on a.UnitOfAchievementGUID equals b.UnitOfAchievementGUID 
                              select new 
                              {
                                  a.Quantity,
                                  b.UnitOfAchievementDescription,
                                  a.DispatchDetailGUID,
                                  a.dataPMDDispatchDetailRowVersion
                              });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result.ToList()), JsonRequestBehavior.AllowGet);
        }

        public ActionResult DispatchDetailCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.PMDDispatch.Create, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/PMD/Views/Dispatch/_DispatchDetailUpdateModal.cshtml",
                new dataPMDDispatchDetail { DispatchGUID = FK });
        }

        public ActionResult DispatchDetailUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.PMDDispatch.Access, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/PMD/Views/Dispatch/_DispatchDetailUpdateModal.cshtml", DbPMD.dataPMDDispatchDetail.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DispatchDetailCreate(dataPMDDispatchDetail model)
        {
            if (!CMS.HasAction(Permissions.PMDDispatch.Create, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveDispatchDetail(model)) return PartialView("~/Areas/PMD/Views/Dispatch/_DispatchDetailUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbPMD.Create(model, Permissions.PMDDispatch.CreateGuid, ExecutionTime,DbCMS);

            try
            {
                DbPMD.SaveChanges();
                return Json(DbPMD.SingleUpdateMessage(DataTableNames.DispatchDetailDataTable,
                   DbPMD.PrimaryKeyControl(model),
                   DbPMD.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DispatchDetailUpdate(dataPMDDispatchDetail model)
        {
            if (!CMS.HasAction(Permissions.PMDDispatch.Update, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveDispatchDetail(model)) return PartialView("~/Areas/PMD/Views/Dispatch/_DispatchDetailUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbPMD.Update(model, Permissions.PMDDispatch.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbPMD.SaveChanges();
                return Json(DbPMD.SingleUpdateMessage(DataTableNames.DispatchDetailDataTable,
                    DbPMD.PrimaryKeyControl(model),
                    DbPMD.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyDispatchDetail(model.DispatchDetailGUID);
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DispatchDetailDelete(dataPMDDispatchDetail model)
        {
            if (!CMS.HasAction(Permissions.PMDDispatch.Delete, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataPMDDispatchDetail> DeletedDispatchDetail = DeleteDispatchDetail(new List<dataPMDDispatchDetail> { model });

            try
            {
                DbPMD.SaveChanges();
                return Json(DbPMD.SingleDeleteMessage(DeletedDispatchDetail, DataTableNames.DispatchDetailDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyDispatchDetail(model.DispatchGUID);
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DispatchDetailRestore(dataPMDDispatchDetail model)
        {
            if (!CMS.HasAction(Permissions.PMDDispatch.Restore, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveDispatchDetail(model))
            {
                return Json(DbPMD.RecordExists());
            }

            List<dataPMDDispatchDetail> RestoredDispatchDetail = RestoreDispatchDetail(Portal.SingleToList(model));

            try
            {
                DbPMD.SaveChanges();
                return Json(DbPMD.SingleRestoreMessage(RestoredDispatchDetail, DataTableNames.DispatchDetailDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyDispatchDetail(model.DispatchDetailGUID);
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult DispatchDetailDataTableDelete(List<dataPMDDispatchDetail> models)
        {
            if (!CMS.HasAction(Permissions.PMDDispatch.Delete, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataPMDDispatchDetail> DeletedDispatchDetail = DeleteDispatchDetail(models);

            try
            {
                DbPMD.SaveChanges();
                return Json(DbPMD.PartialDeleteMessage(DeletedDispatchDetail, models, DataTableNames.DispatchDetailDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult DispatchDetailDataTableRestore(List<dataPMDDispatchDetail> models)
        {

            if (!CMS.HasAction(Permissions.PMDDispatch.Restore, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataPMDDispatchDetail> RestoredDispatchDetail = RestoreDispatchDetail(models);

            try
            {
                DbPMD.SaveChanges();
                return Json(DbPMD.PartialRestoreMessage(RestoredDispatchDetail, models, DataTableNames.DispatchDetailDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        private List<dataPMDDispatchDetail> DeleteDispatchDetail(List<dataPMDDispatchDetail> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataPMDDispatchDetail> DeletedDispatchDetail = new List<dataPMDDispatchDetail>();

            string query = DbPMD.QueryBuilder(models, Permissions.PMDDispatch.DeleteGuid, SubmitTypes.Delete, "");

            var DispatchDetail = DbPMD.Database.SqlQuery<dataPMDDispatchDetail>(query).ToList();

            foreach (var dispatchDetail in DispatchDetail)
            {
                DeletedDispatchDetail.Add(DbPMD.Delete(dispatchDetail, ExecutionTime, Permissions.PMDDispatch.DeleteGuid, DbCMS));
            }

            return DeletedDispatchDetail;
        }

        private List<dataPMDDispatchDetail> RestoreDispatchDetail(List<dataPMDDispatchDetail> models)
        {
            Guid DeleteActionGUID = Permissions.PMDDispatch.DeleteGuid;
            Guid RestoreActionGUID = Permissions.PMDDispatch.RestoreGuid;
            DateTime RestoringTime = DateTime.Now;

            List<dataPMDDispatchDetail> RestoredDispatchDetail = new List<dataPMDDispatchDetail>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbPMD.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, baseQuery);

            var DispatchDetail = DbPMD.Database.SqlQuery<dataPMDDispatchDetail>(query).ToList();
            foreach (var dispatchDetail in DispatchDetail)
            {
                if (!ActiveDispatchDetail(dispatchDetail))
                {
                    RestoredDispatchDetail.Add(DbPMD.Restore(dispatchDetail, DeleteActionGUID, RestoreActionGUID, RestoringTime, DbCMS));
                }
            }

            return RestoredDispatchDetail;
        }

        private JsonResult ConcrrencyDispatchDetail(Guid PK)
        {
            dataPMDDispatchDetail dbModel = new dataPMDDispatchDetail();

            var DispatchDetail = DbPMD.dataPMDDispatchDetail.Where(x => x.DispatchDetailGUID == PK).FirstOrDefault();
            var dbDispatchDetail = DbPMD.Entry(DispatchDetail).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbDispatchDetail, dbModel);

            if (DispatchDetail.dataPMDDispatchDetailRowVersion.SequenceEqual(dbModel.dataPMDDispatchDetailRowVersion))
            {
                return Json(DbPMD.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbPMD, dbModel, "DispatchDetailContainer"));
        }

        private bool ActiveDispatchDetail(dataPMDDispatchDetail model)
        {
            int DispatchDetailID = DbPMD.dataPMDDispatchDetail
                                  .Where(l => 
                                              l.DispatchDetailGUID != model.DispatchDetailGUID &&
                                              l.UnitOfAchievementGUID == model.UnitOfAchievementGUID &&
                                              l.Active).Count();
            if (DispatchDetailID > 0)
            {
                ModelState.AddModelError("UnitOfAchievementGUID", "Item already exists"); //From resource ?????? Amer  
            }

            return (DispatchDetailID > 0);
        }

        #endregion

        #region Bulk Items Dispatch
        public ActionResult DispatchBlukItems(Guid FK)
        {
            if (!CMS.HasAction(Permissions.PMDDispatch.Create, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var model = new DispatchBlukItemUpdateModel()
            {
                 DispatchGUID= FK,
                 dispatchDetails =(from a in DbPMD.codePmd2023UnitOfAchievementLanguages
                 join b in DbPMD.codePmd2023UnitOfAchievement on a.UnitOfAchievementGUID equals b.UnitOfAchievementGUID
                 join c in DbPMD.dataPMDDispatchDetail.Where(x=>x.DispatchGUID==FK && x.Active) on b.UnitOfAchievementGUID equals c.UnitOfAchievementGUID into LJ1
                 from R1 in LJ1.DefaultIfEmpty()
                                  where a.LanguageID == "EN" && b.UnitOfAchievementCategory == "CRI"
                                  select
                   new PMDDispatchItmesDetail
                   {
                       UnitOfAchievementGUID = a.UnitOfAchievementGUID,
                       DispatchDetailGUID= R1.DispatchDetailGUID,
                       UnitOfAchievementGroupingDescription = a.UnitOfAchievementGroupingDescription,
                       UnitOfAchievementDescription = a.UnitOfAchievementDescription,
                       UnitOfAchievementGuidance = a.UnitOfAchievementGuidance,
                       UnitOfAchievementOrder = b.UnitOfAchievementOrder,
                       MeasurementTotal = R1 != null?R1.Quantity: 0
                   }
                ).Distinct().OrderBy(x => x.UnitOfAchievementOrder).ToList()
        };
            return PartialView("~/Areas/PMD/Views/Dispatch/_DispatchBulkDetailUpdateModal.cshtml", model);

        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DispatchBlukItemsCreate(DispatchBlukItemUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.PMDDispatch.Create, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid) return PartialView("~/Areas/PMD/Views/Dispatch/_DispatchBulkDetailUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;
            var dispatch = DbPMD.dataPMDDispatch.Where(x => x.DispatchGUID == model.DispatchGUID).FirstOrDefault();
            foreach (var item in model.dispatchDetails)
            {

                if (item.MeasurementTotal > 0)
                {
                    if (item.DispatchDetailGUID == null)
                    {
                        dataPMDDispatchDetail Destination = new dataPMDDispatchDetail();
                        Destination.DispatchDetailGUID = Guid.NewGuid();
                        Destination.Quantity = item.MeasurementTotal;
                        Destination.UnitOfAchievementGUID = item.UnitOfAchievementGUID;
                        Destination.Active = true;
                        Destination.DispatchGUID = model.DispatchGUID;

                        DbPMD.Create(Destination, Permissions.PMDDispatch.CreateGuid, ExecutionTime, DbCMS);
                    }
                    else
                    {
                        dataPMDDispatchDetail Destination = DbPMD.dataPMDDispatchDetail.Where(x => x.DispatchDetailGUID == item.DispatchDetailGUID &&x.Active).FirstOrDefault();
                        Destination.Quantity = item.MeasurementTotal;
                    }
                }
                else
                {
                    if (item.DispatchDetailGUID != null)
                    {
                        dataPMDDispatchDetail Destination = DbPMD.dataPMDDispatchDetail.Where(x => x.DispatchDetailGUID == item.DispatchDetailGUID && x.Active).FirstOrDefault();
                        Destination.Quantity = 0;
                        Destination.Active = false;
                    }
                }
            }

            try
            {
                DbPMD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbPMD.SingleUpdateMessage(DataTableNames.DispatchDetailDataTable,
                      DbPMD.PrimaryKeyControl(dispatch),
                      DbPMD.RowVersionControls(Portal.SingleToList(dispatch))));
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        #endregion
    }
}