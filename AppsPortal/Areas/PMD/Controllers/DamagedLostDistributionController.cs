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
    public class DamagedLostDistributionController : PMDBaseController
    {
        #region Damaged Lost Distribution

        [Route("PMD/DamagedLostDistribution/")]
        public ActionResult DamagedLostDistributionIndex()
        {
            if (!CMS.HasAction(Permissions.PMDDamagedLostDistribution.Access, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/PMD/Views/DamagedLostDistribution/Index.cshtml");
        }

        [Route("PMD/DamagedLostDistributionDataTable/")]
        public JsonResult DamagedLostDistributionDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<DamagedLostDistributionDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<DamagedLostDistributionDataTableModel>(DataTable.Filters);
            }

            var All = (from a in DbPMD.dataPMDDamagedLostDistribution.AsExpandable()
                       join b in DbPMD.codeOrganizationsInstancesLanguages.Where(x => x.Active && x.LanguageID==LAN ) on a.OrganizationInstanceGUID equals b.OrganizationInstanceGUID into LJ1
                       from R1 in LJ1.DefaultIfEmpty()
                       join d in DbPMD.codeOchaLocationGovernorate on a.admin1Pcode equals d.admin1Pcode
                       select new DamagedLostDistributionDataTableModel
                       {
                           DamagedLostDistributionDate = a.DamagedLostDistributionDate,
                           admin1Pcode = d.admin1Name_en,
                           admin4Pcode = d.admin1Name_en,
                           GovernorateGUID = a.admin1Pcode,
                           OrganizationInstanceGUID = R1.OrganizationInstanceGUID.ToString(),
                           DamagedLostDistributionGUID = a.DamagedLostDistributionGUID,
                           OrganizationInstanceDescription = R1.OrganizationInstanceDescription,
                           Active = a.Active,
                           dataPMDDamagedLostDistributionRowVersion = a.dataPMDDamagedLostDistributionRowVersion,
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);
            List<DamagedLostDistributionDataTableModel> Result = Mapper.Map<List<DamagedLostDistributionDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("PMD/DamagedLostDistribution/Create/")]
        public ActionResult DamagedLostDistributionCreate()
        {
            if (!CMS.HasAction(Permissions.PMDDamagedLostDistribution.Create, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/PMD/Views/DamagedLostDistribution/DamagedLostDistribution.cshtml", new DamagedLostDistributionUpdateModel());
        }


        [Route("PMD/DamagedLostDistribution/Update/{PK}")]
        public ActionResult DamagedLostDistributionUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.PMDDamagedLostDistribution.Access, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            var model = (from x in DbPMD.dataPMDDamagedLostDistribution.Where(x => x.DamagedLostDistributionGUID == PK)
                         select new DamagedLostDistributionUpdateModel
                         {
                             admin1Pcode=x.admin1Pcode,
                             admin4Pcode=x.admin4Pcode,
                             DamagedLostDistributionDate=x.DamagedLostDistributionDate,
                             Comments=x.Comments,
                             DamagedLostDistributionGUID=x.DamagedLostDistributionGUID,
                             GovernorateGUID=x.GovernorateGUID,
                             OrganizationInstanceGUID=x.OrganizationInstanceGUID,
                             PmdWarehouseGUID = x.PmdWarehouseGUID,
                             Active = x.Active,
                             dataPMDDamagedLostDistributionRowVersion = x.dataPMDDamagedLostDistributionRowVersion,
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("DamagedLostDistribution", "", new { Area = "PMD" }));
            return View("~/Areas/PMD/Views/DamagedLostDistribution/DamagedLostDistribution.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DamagedLostDistributionCreate(DamagedLostDistributionUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.PMDDamagedLostDistribution.Create, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            if (!ModelState.IsValid || ActiveDamagedLostDistribution(model)) return PartialView("~/Areas/PMD/Views/DamagedLostDistribution/_DamagedLostDistributionForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;
            Guid EntityPK = Guid.NewGuid();
            if (model.DamagedLostDistributionGUID != Guid.Empty)
            {
                EntityPK = model.DamagedLostDistributionGUID;
            }

            dataPMDDamagedLostDistribution DamagedLostDistribution = Mapper.Map(model, new dataPMDDamagedLostDistribution());
            DamagedLostDistribution.DamagedLostDistributionGUID = EntityPK;
            DamagedLostDistribution.GovernorateGUID = (Guid)DbPMD.codeOchaLocationGovernorate.Where(x => x.admin1Pcode == model.admin1Pcode).FirstOrDefault().GovernorateGUID;

            DbPMD.Create(DamagedLostDistribution, Permissions.PMDDamagedLostDistribution.CreateGuid, ExecutionTime, DbCMS);


            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.DamagedLostDistributionDetailDataTable, ControllerContext, "DamagedLostDistributionDetailContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.PMDDamagedLostDistribution.Create, Apps.PMD, new UrlHelper(Request.RequestContext).Action("DamagedLostDistribution/Create", "", new { Area = "PMD" })), Container = "DamagedLostDistributionFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.PMDDamagedLostDistribution.Update, Apps.PMD), Container = "DamagedLostDistributionFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.PMDDamagedLostDistribution.Delete, Apps.PMD), Container = "DamagedLostDistributionFormControls" });

            try
            {
                DbPMD.SaveChanges();
                return Json(DbPMD.SingleCreateMessage(DbPMD.PrimaryKeyControl(DamagedLostDistribution), null, Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DamagedLostDistributionUpdate(DamagedLostDistributionUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.PMDDamagedLostDistribution.Update, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            if (!ModelState.IsValid || ActiveDamagedLostDistribution(model)) return PartialView("~/Areas/PMD/Views/DamagedLostDistribution/_DamagedLostDistributionForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            dataPMDDamagedLostDistribution DamagedLostDistribution = Mapper.Map(model, new dataPMDDamagedLostDistribution());
            DamagedLostDistribution.GovernorateGUID = (Guid)DbPMD.codeOchaLocationGovernorate.Where(x => x.admin1Pcode == model.admin1Pcode).FirstOrDefault().GovernorateGUID;

            DbPMD.Update(DamagedLostDistribution, Permissions.PMDDamagedLostDistribution.UpdateGuid, ExecutionTime, DbCMS);

            var DamagedLostDistributionDetail = DbPMD.dataPMDDamagedLostDistributionDetail.Where(x => x.DamagedLostDistributionGUID == model.DamagedLostDistributionGUID &&  x.Active).FirstOrDefault();

           
            try
            {
                DbPMD.SaveChanges();
                return Json(DbPMD.SingleUpdateMessage(null, null, DbPMD.RowVersionControls(DamagedLostDistribution, DamagedLostDistributionDetail)));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyDamagedLostDistribution(model.DamagedLostDistributionGUID);
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DamagedLostDistributionDelete(dataPMDDamagedLostDistribution model)
        {
            if (!CMS.HasAction(Permissions.PMDDamagedLostDistribution.Delete, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            List<dataPMDDamagedLostDistribution> DeletedDamagedLostDistribution = DeleteDamagedLostDistribution(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.PMDDamagedLostDistribution.Restore, Apps.PMD), Container = "DamagedLostDistributionFormControls" });

            try
            {
                int CommitedRows = DbPMD.SaveChanges();
                return Json(DbPMD.SingleDeleteMessage(CommitedRows, DeletedDamagedLostDistribution.FirstOrDefault(), "DamagedLostDistributionDetailContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyDamagedLostDistribution(model.DamagedLostDistributionGUID);
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DamagedLostDistributionRestore(dataPMDDamagedLostDistribution model)
        {
            if (!CMS.HasAction(Permissions.PMDDamagedLostDistribution.Restore, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            if (ActiveDamagedLostDistribution(model))
            {
                return Json(DbPMD.RecordExists());
            }

            List<dataPMDDamagedLostDistribution> RestoredDamagedLostDistribution = RestoreDamagedLostDistribution(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.PMDDamagedLostDistribution.Create, Apps.PMD, new UrlHelper(Request.RequestContext).Action("DamagedLostDistribution/Create", "", new { Area = "PMD" })), Container = "DamagedLostDistributionFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.PMDDamagedLostDistribution.Update, Apps.PMD), Container = "DamagedLostDistributionFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.PMDDamagedLostDistribution.Restore, Apps.PMD), Container = "DamagedLostDistributionFormControls" });

            try
            {
                int CommitedRows = DbPMD.SaveChanges();
                return Json(DbPMD.SingleRestoreMessage(CommitedRows, RestoredDamagedLostDistribution, DbPMD.PrimaryKeyControl(RestoredDamagedLostDistribution.FirstOrDefault()), Url.Action(DataTableNames.DamagedLostDistributionDetailDataTable, Portal.GetControllerName(ControllerContext)), "DamagedLostDistributionDetailContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyDamagedLostDistribution(model.DamagedLostDistributionGUID);
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult DamagedLostDistributionDataTableDelete(List<dataPMDDamagedLostDistribution> models)
        {
            if (!CMS.HasAction(Permissions.PMDDamagedLostDistribution.Delete, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataPMDDamagedLostDistribution> DeletedDamagedLostDistribution = DeleteDamagedLostDistribution(models);

            try
            {
                DbPMD.SaveChanges();
                return Json(DbPMD.PartialDeleteMessage(DeletedDamagedLostDistribution, models, DataTableNames.DamagedLostDistributionDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult DamagedLostDistributionDataTableRestore(List<dataPMDDamagedLostDistribution> models)
        {
            if (!CMS.HasAction(Permissions.PMDDamagedLostDistribution.Restore, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataPMDDamagedLostDistribution> RestoredDamagedLostDistribution = RestoreDamagedLostDistribution(models);

            try
            {
                DbPMD.SaveChanges();
                return Json(DbPMD.PartialRestoreMessage(RestoredDamagedLostDistribution, models, DataTableNames.DamagedLostDistributionDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        private List<dataPMDDamagedLostDistribution> DeleteDamagedLostDistribution(List<dataPMDDamagedLostDistribution> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<dataPMDDamagedLostDistribution> DeletedDamagedLostDistribution = new List<dataPMDDamagedLostDistribution>();

            //THIS IS FOR PERMISSION TEST//
            //DO NOT DELETE THIS CODE
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = (from a in DbPMD.dataPMDDamagedLostDistribution
            //                    from f in DbPMD.dataPMDDamagedLostDistributionFactorForTest
            //                    where a.DamagedLostDistributionGUID == f.DamagedLostDistributionGUID
            //                    select new { a.DamagedLostDistributionGUID, a.dataPMDDamagedLostDistributionRowVersion, f.OperationGUID, f.OrganizationGUID, f.CountryGUID }).AsQueryable().ToString();

            string query = DbPMD.QueryBuilder(models, Permissions.PMDDamagedLostDistribution.DeleteGuid, SubmitTypes.Delete, "");

            var Records = DbPMD.Database.SqlQuery<dataPMDDamagedLostDistribution>(query).ToList();
            foreach (var record in Records)
            {
                DeletedDamagedLostDistribution.Add(DbPMD.Delete(record, ExecutionTime, Permissions.PMDDamagedLostDistribution.DeleteGuid, DbCMS));
            }

            var DamagedLostDistributionDetail = DeletedDamagedLostDistribution.SelectMany(a => a.dataPMDDamagedLostDistributionDetail).Where(l => l.Active).ToList();
            foreach (var damagedLostDistributionDetail in DamagedLostDistributionDetail)
            {
                DbPMD.Delete(damagedLostDistributionDetail, ExecutionTime, Permissions.PMDDamagedLostDistribution.DeleteGuid, DbCMS);
            }
            return DeletedDamagedLostDistribution;
        }

        private List<dataPMDDamagedLostDistribution> RestoreDamagedLostDistribution(List<dataPMDDamagedLostDistribution> models)
        {
            Guid DeleteActionGUID = Permissions.PMDDamagedLostDistribution.DeleteGuid;
            Guid RestoreActionGUID = Permissions.PMDDamagedLostDistribution.RestoreGuid;
            DateTime RestoringTime = DateTime.Now;

            List<dataPMDDamagedLostDistribution> RestoredDamagedLostDistribution = new List<dataPMDDamagedLostDistribution>();

            //THIS IS FOR PERMISSION TEST//
            //DO NOT DELETE THIS CODE
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = (from a in DbPMD.dataPMDDamagedLostDistribution
            //                    from f in DbPMD.dataPMDDamagedLostDistributionFactorForTest
            //                    where a.DamagedLostDistributionGUID == f.DamagedLostDistributionGUID
            //                    select new
            //                    {
            //                        a.DamagedLostDistributionGUID,
            //                        a.dataPMDDamagedLostDistributionRowVersion,
            //                        C2 = f.OperationGUID + "," + f.OrganizationGUID,
            //                    }).AsQueryable().ToString();//.Replace("[C2]", "[FactorsToken]");

            string query = DbPMD.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, "");


            var Records = DbPMD.Database.SqlQuery<dataPMDDamagedLostDistribution>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveDamagedLostDistribution(record))
                {
                    RestoredDamagedLostDistribution.Add(DbPMD.Restore(record, DeleteActionGUID, RestoreActionGUID, RestoringTime, DbCMS));
                }
            }

            var DamagedLostDistributionDetail = RestoredDamagedLostDistribution.SelectMany(x => x.dataPMDDamagedLostDistributionDetail.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var damagedLostDistributionDetail in DamagedLostDistributionDetail)
            {
               
                    DbPMD.Restore(damagedLostDistributionDetail, DeleteActionGUID, RestoreActionGUID, RestoringTime, DbCMS);
                
            }

            return RestoredDamagedLostDistribution;
        }

        private JsonResult ConcrrencyDamagedLostDistribution(Guid PK)
        {
            DamagedLostDistributionUpdateModel dbModel = new DamagedLostDistributionUpdateModel();

            var DamagedLostDistribution = DbPMD.dataPMDDamagedLostDistribution.Where(a => a.DamagedLostDistributionGUID == PK).FirstOrDefault();
            var dbDamagedLostDistribution = DbPMD.Entry(DamagedLostDistribution).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbDamagedLostDistribution, dbModel);

            var DamagedLostDistributionDetail = DbPMD.dataPMDDamagedLostDistributionDetail.Where(x => x.DamagedLostDistributionGUID == PK).Where(p => (p.Active == true ? p.Active : p.DeletedOn == p.dataPMDDamagedLostDistribution.DeletedOn) ).FirstOrDefault();
            var dbDamagedLostDistributionDetail = DbPMD.Entry(DamagedLostDistributionDetail).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbDamagedLostDistributionDetail, dbModel);

            if (DamagedLostDistribution.dataPMDDamagedLostDistributionRowVersion.SequenceEqual(dbModel.dataPMDDamagedLostDistributionRowVersion) && DamagedLostDistributionDetail.dataPMDDamagedLostDistributionDetailRowVersion.SequenceEqual(dbModel.dataPMDDamagedLostDistributionRowVersion))
            {
                return Json(DbPMD.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbPMD, dbModel, "DamagedLostDistributionDetailContainer"));
        }

        private bool ActiveDamagedLostDistribution(Object model)
        {
            dataPMDDamagedLostDistribution DamagedLostDistribution = Mapper.Map(model, new dataPMDDamagedLostDistribution());
            dataPMDDamagedLostDistributionDetail DamagedLostDistributionDetail = Mapper.Map(model, new dataPMDDamagedLostDistributionDetail());

           
          
            return false;
        }

        public ActionResult DamagedLostDistributionAuditHistory(Guid RecordGUID)
        {
            List<Guid> ChildrenGUIDs = new List<Guid>();

            DbPMD.dataPMDDamagedLostDistributionDetail.AsNoTracking().Where(x => x.DamagedLostDistributionGUID == RecordGUID).ToList().ForEach(x => ChildrenGUIDs.Add(x.DamagedLostDistributionDetailGUID));
            ChildrenGUIDs.Add(RecordGUID);

            List<Guid> ChildrenActions = new List<Guid>
            {
                Permissions.PMDDamagedLostDistribution.UpdateGuid
            };
            return new AppsPortal.Controllers.AuditController().GetAuditHistoryGlobalizationVersion(RecordGUID, ChildrenActions, ChildrenGUIDs);
        }

        #endregion 

        #region Damaged Lost Distribution Damaged Lost Distribution Detail
        public ActionResult DamagedLostDistributionDetailDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/PMD/Views/DamagedLostDistribution/_DamagedLostDistributionDetailDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<dataPMDDamagedLostDistributionDetail, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<dataPMDDamagedLostDistributionDetail>(DataTable.Filters);
            }

            var Result =(from a in DbPMD.dataPMDDamagedLostDistributionDetail.AsNoTracking().AsExpandable().Where(x => x.DamagedLostDistributionGUID == PK).Where(Predicate)
                         join b in DbPMD.codePmd2023UnitOfAchievementLanguages.Where(x=>x.LanguageID==LAN && x.Active) on a.UnitOfAchievementGUID equals b.UnitOfAchievementGUID 
                              select new 
                              {
                                  a.Quantity,
                                  b.UnitOfAchievementDescription,
                                  a.DamagedLostDistributionDetailGUID,
                                  a.dataPMDDamagedLostDistributionDetailRowVersion
                              });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result.ToList()), JsonRequestBehavior.AllowGet);
        }

        public ActionResult DamagedLostDistributionDetailCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.PMDDamagedLostDistribution.Create, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/PMD/Views/DamagedLostDistribution/_DamagedLostDistributionDetailUpdateModal.cshtml",
                new dataPMDDamagedLostDistributionDetail { DamagedLostDistributionGUID = FK });
        }

        public ActionResult DamagedLostDistributionDetailUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.PMDDamagedLostDistribution.Access, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/PMD/Views/DamagedLostDistribution/_DamagedLostDistributionDetailUpdateModal.cshtml", DbPMD.dataPMDDamagedLostDistributionDetail.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DamagedLostDistributionDetailCreate(dataPMDDamagedLostDistributionDetail model)
        {
            if (!CMS.HasAction(Permissions.PMDDamagedLostDistribution.Create, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveDamagedLostDistributionDetail(model)) return PartialView("~/Areas/PMD/Views/DamagedLostDistribution/_DamagedLostDistributionDetailUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbPMD.Create(model, Permissions.PMDDamagedLostDistribution.CreateGuid, ExecutionTime,DbCMS);

            try
            {
                DbPMD.SaveChanges();
                return Json(DbPMD.SingleUpdateMessage(DataTableNames.DamagedLostDistributionDetailDataTable,
                   DbPMD.PrimaryKeyControl(model),
                   DbPMD.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DamagedLostDistributionDetailUpdate(dataPMDDamagedLostDistributionDetail model)
        {
            if (!CMS.HasAction(Permissions.PMDDamagedLostDistribution.Update, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveDamagedLostDistributionDetail(model)) return PartialView("~/Areas/PMD/Views/DamagedLostDistribution/_DamagedLostDistributionDetailUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbPMD.Update(model, Permissions.PMDDamagedLostDistribution.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbPMD.SaveChanges();
                return Json(DbPMD.SingleUpdateMessage(DataTableNames.DamagedLostDistributionDetailDataTable,
                    DbPMD.PrimaryKeyControl(model),
                    DbPMD.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyDamagedLostDistributionDetail(model.DamagedLostDistributionDetailGUID);
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DamagedLostDistributionDetailDelete(dataPMDDamagedLostDistributionDetail model)
        {
            if (!CMS.HasAction(Permissions.PMDDamagedLostDistribution.Delete, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataPMDDamagedLostDistributionDetail> DeletedDamagedLostDistributionDetail = DeleteDamagedLostDistributionDetail(new List<dataPMDDamagedLostDistributionDetail> { model });

            try
            {
                DbPMD.SaveChanges();
                return Json(DbPMD.SingleDeleteMessage(DeletedDamagedLostDistributionDetail, DataTableNames.DamagedLostDistributionDetailDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyDamagedLostDistributionDetail(model.DamagedLostDistributionGUID);
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DamagedLostDistributionDetailRestore(dataPMDDamagedLostDistributionDetail model)
        {
            if (!CMS.HasAction(Permissions.PMDDamagedLostDistribution.Restore, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveDamagedLostDistributionDetail(model))
            {
                return Json(DbPMD.RecordExists());
            }

            List<dataPMDDamagedLostDistributionDetail> RestoredDamagedLostDistributionDetail = RestoreDamagedLostDistributionDetail(Portal.SingleToList(model));

            try
            {
                DbPMD.SaveChanges();
                return Json(DbPMD.SingleRestoreMessage(RestoredDamagedLostDistributionDetail, DataTableNames.DamagedLostDistributionDetailDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyDamagedLostDistributionDetail(model.DamagedLostDistributionDetailGUID);
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult DamagedLostDistributionDetailDataTableDelete(List<dataPMDDamagedLostDistributionDetail> models)
        {
            if (!CMS.HasAction(Permissions.PMDDamagedLostDistribution.Delete, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataPMDDamagedLostDistributionDetail> DeletedDamagedLostDistributionDetail = DeleteDamagedLostDistributionDetail(models);

            try
            {
                DbPMD.SaveChanges();
                return Json(DbPMD.PartialDeleteMessage(DeletedDamagedLostDistributionDetail, models, DataTableNames.DamagedLostDistributionDetailDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult DamagedLostDistributionDetailDataTableRestore(List<dataPMDDamagedLostDistributionDetail> models)
        {

            if (!CMS.HasAction(Permissions.PMDDamagedLostDistribution.Restore, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataPMDDamagedLostDistributionDetail> RestoredDamagedLostDistributionDetail = RestoreDamagedLostDistributionDetail(models);

            try
            {
                DbPMD.SaveChanges();
                return Json(DbPMD.PartialRestoreMessage(RestoredDamagedLostDistributionDetail, models, DataTableNames.DamagedLostDistributionDetailDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        private List<dataPMDDamagedLostDistributionDetail> DeleteDamagedLostDistributionDetail(List<dataPMDDamagedLostDistributionDetail> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataPMDDamagedLostDistributionDetail> DeletedDamagedLostDistributionDetail = new List<dataPMDDamagedLostDistributionDetail>();

            string query = DbPMD.QueryBuilder(models, Permissions.PMDDamagedLostDistribution.DeleteGuid, SubmitTypes.Delete, "");

            var DamagedLostDistributionDetail = DbPMD.Database.SqlQuery<dataPMDDamagedLostDistributionDetail>(query).ToList();

            foreach (var damagedLostDistributionDetail in DamagedLostDistributionDetail)
            {
                DeletedDamagedLostDistributionDetail.Add(DbPMD.Delete(damagedLostDistributionDetail, ExecutionTime, Permissions.PMDDamagedLostDistribution.DeleteGuid, DbCMS));
            }

            return DeletedDamagedLostDistributionDetail;
        }

        private List<dataPMDDamagedLostDistributionDetail> RestoreDamagedLostDistributionDetail(List<dataPMDDamagedLostDistributionDetail> models)
        {
            Guid DeleteActionGUID = Permissions.PMDDamagedLostDistribution.DeleteGuid;
            Guid RestoreActionGUID = Permissions.PMDDamagedLostDistribution.RestoreGuid;
            DateTime RestoringTime = DateTime.Now;

            List<dataPMDDamagedLostDistributionDetail> RestoredDamagedLostDistributionDetail = new List<dataPMDDamagedLostDistributionDetail>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbPMD.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, baseQuery);

            var DamagedLostDistributionDetail = DbPMD.Database.SqlQuery<dataPMDDamagedLostDistributionDetail>(query).ToList();
            foreach (var damagedLostDistributionDetail in DamagedLostDistributionDetail)
            {
                if (!ActiveDamagedLostDistributionDetail(damagedLostDistributionDetail))
                {
                    RestoredDamagedLostDistributionDetail.Add(DbPMD.Restore(damagedLostDistributionDetail, DeleteActionGUID, RestoreActionGUID, RestoringTime, DbCMS));
                }
            }

            return RestoredDamagedLostDistributionDetail;
        }

        private JsonResult ConcrrencyDamagedLostDistributionDetail(Guid PK)
        {
            dataPMDDamagedLostDistributionDetail dbModel = new dataPMDDamagedLostDistributionDetail();

            var DamagedLostDistributionDetail = DbPMD.dataPMDDamagedLostDistributionDetail.Where(x => x.DamagedLostDistributionDetailGUID == PK).FirstOrDefault();
            var dbDamagedLostDistributionDetail = DbPMD.Entry(DamagedLostDistributionDetail).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbDamagedLostDistributionDetail, dbModel);

            if (DamagedLostDistributionDetail.dataPMDDamagedLostDistributionDetailRowVersion.SequenceEqual(dbModel.dataPMDDamagedLostDistributionDetailRowVersion))
            {
                return Json(DbPMD.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbPMD, dbModel, "DamagedLostDistributionDetailContainer"));
        }

        private bool ActiveDamagedLostDistributionDetail(dataPMDDamagedLostDistributionDetail model)
        {
            int DamagedLostDistributionDetailID = DbPMD.dataPMDDamagedLostDistributionDetail
                                  .Where(l => 
                                              l.DamagedLostDistributionGUID != model.DamagedLostDistributionGUID &&
                                              l.UnitOfAchievementGUID == model.UnitOfAchievementGUID &&
                                              l.Active).Count();
            if (DamagedLostDistributionDetailID > 0)
            {
                ModelState.AddModelError("UnitOfAchievementGUID", "Item already exists"); //From resource ?????? Amer  
            }

            return (DamagedLostDistributionDetailID > 0);
        }

        #endregion

        #region Bulk Items Damaged Lost Distribution
        public ActionResult DamagedLostDistributionBlukItems(Guid FK)
        {
            if (!CMS.HasAction(Permissions.PMDDamagedLostDistribution.Create, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var model = new DamagedLostDistributionBlukItemUpdateModel()
            {
                DamagedLostDistributionGUID = FK,
                damagedLostDistributionDetails = (from a in DbPMD.codePmd2023UnitOfAchievementLanguages
                                   join b in DbPMD.codePmd2023UnitOfAchievement on a.UnitOfAchievementGUID equals b.UnitOfAchievementGUID
                                   join c in DbPMD.dataPMDDamagedLostDistributionDetail.Where(x => x.DamagedLostDistributionGUID == FK && x.Active) on b.UnitOfAchievementGUID equals c.UnitOfAchievementGUID into LJ1
                                   from R1 in LJ1.DefaultIfEmpty()
                                   where a.LanguageID == "EN" && b.UnitOfAchievementCategory == "CRI"
                                   select
                    new PMDDamagedLostDistributionItmesDetail
                    {
                        UnitOfAchievementGUID = a.UnitOfAchievementGUID,
                        DamagedLostDistributionDetailGUID = R1.DamagedLostDistributionDetailGUID,
                        UnitOfAchievementGroupingDescription = a.UnitOfAchievementGroupingDescription,
                        UnitOfAchievementDescription = a.UnitOfAchievementDescription,
                        UnitOfAchievementGuidance = a.UnitOfAchievementGuidance,
                        UnitOfAchievementOrder = b.UnitOfAchievementOrder,
                        MeasurementTotal = R1 != null ? R1.Quantity : 0
                    }
                ).Distinct().OrderBy(x => x.UnitOfAchievementOrder).ToList()
            };
            return PartialView("~/Areas/PMD/Views/DamagedLostDistribution/_DamagedLostDistributionBulkDetailUpdateModal.cshtml", model);

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DamagedLostDistributionBlukItemsCreate(DamagedLostDistributionBlukItemUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.PMDDamagedLostDistribution.Create, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid) return PartialView("~/Areas/PMD/Views/DamagedLostDistribution/_DamagedLostDistributionBulkDetailUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;
            var DamagedLostDistribution = DbPMD.dataPMDDamagedLostDistribution.Where(x => x.DamagedLostDistributionGUID == model.DamagedLostDistributionGUID).FirstOrDefault();
            foreach (var item in model.damagedLostDistributionDetails)
            {

                if (item.MeasurementTotal > 0)
                {
                    if (item.DamagedLostDistributionDetailGUID == null)
                    {
                        dataPMDDamagedLostDistributionDetail Destination = new dataPMDDamagedLostDistributionDetail();
                        Destination.DamagedLostDistributionDetailGUID = Guid.NewGuid();
                        Destination.Quantity = item.MeasurementTotal;
                        Destination.UnitOfAchievementGUID = item.UnitOfAchievementGUID;
                        Destination.Active = true;
                        Destination.DamagedLostDistributionGUID = model.DamagedLostDistributionGUID;

                        DbPMD.Create(Destination, Permissions.PMDDamagedLostDistribution.CreateGuid, ExecutionTime, DbCMS);
                    }
                    else
                    {
                        dataPMDDamagedLostDistributionDetail Destination = DbPMD.dataPMDDamagedLostDistributionDetail.Where(x => x.DamagedLostDistributionDetailGUID == item.DamagedLostDistributionDetailGUID && x.Active).FirstOrDefault();
                        Destination.Quantity = item.MeasurementTotal;
                    }
                }
                else
                {
                    if (item.DamagedLostDistributionDetailGUID != null)
                    {
                        dataPMDDamagedLostDistributionDetail Destination = DbPMD.dataPMDDamagedLostDistributionDetail.Where(x => x.DamagedLostDistributionDetailGUID == item.DamagedLostDistributionDetailGUID && x.Active).FirstOrDefault();
                        Destination.Quantity = 0;
                        Destination.Active = false;
                    }
                }
            }

            try
            {
                DbPMD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbPMD.SingleUpdateMessage(DataTableNames.DamagedLostDistributionDetailDataTable,
                      DbPMD.PrimaryKeyControl(DamagedLostDistribution),
                      DbPMD.RowVersionControls(Portal.SingleToList(DamagedLostDistribution))));
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        #endregion
    }
}