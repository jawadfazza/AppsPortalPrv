using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.ViewModels;
using AutoMapper;
using LinqKit;
using OVS_DAL.Model;
using AppsPortal.OVS.ViewModels;


namespace AppsPortal.Areas.OVS.Controllers
{
    public class CandidateController : OVSBaseController
    {
        #region Election Candidate

        public ActionResult ElectionCandidatesDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null)
                return PartialView("~/Areas/OVS/Views/Candidate/_CandidatesDataTable.cshtml",
                    new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<dataElectionCandidate, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<dataElectionCandidate>(DataTable.Filters);
            }

            var Result = (from a in DbOVS.dataElectionCandidate.AsExpandable()
                    .Where(x => x.ElectionGUID == PK).Where(Predicate)

                          select new
                          {
                              ElectionCandidateGUID = a.ElectionCandidateGUID,
                              EmailAddress = a.EmailAddress,
                              FullName = a.FullName,
                              CampaignPlan = a.CampaignPlan,
                              dataElectionCandidateRowVersion = a.dataElectionCandidateRowVersion
                          });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ElectionCandidateCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Create, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            return PartialView("~/Areas/OVS/Views/Candidate/_CandidateUpdateModal.cshtml",
                new dataElectionCandidate { ElectionGUID = FK });
        }

        public ActionResult ElectionCandidateUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Access, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            var model = DbOVS.dataElectionCandidate.Find(PK);
            
            return PartialView("~/Areas/OVS/Views/Candidate/_CandidateUpdateModal.cshtml", model);
        }

        [HttpPost]
        public ActionResult ElectionCandidateCreate(ElectionCandidateModel upload)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Create, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            DateTime ExecutionTime = DateTime.Now;
            dataElectionCandidate model = Mapper.Map(upload, new dataElectionCandidate());
           

            if (model.ElectionCandidateGUID == Guid.Empty)
            {
                if (!ModelState.IsValid || ActiveElectionCandidate(model)) return PartialView("~/Areas/OVS/Views/Candidate/_CandidateUpdateModal.cshtml", model);
                List<dataElectionCondition> conditions = DbOVS.dataElectionCondition.Where(x => x.ElectionGUID == upload.ElectionGUID).ToList();
                List<dataElectionCandidate> candidateses = DbOVS.dataElectionCandidate.AsNoTracking().Where(x => x.ElectionGUID == upload.ElectionGUID).ToList();
                candidateses.Add(model);
                string checkMessage = new ConditionController().checkElectionCanidates(conditions, candidateses);
                if (checkMessage != "1")
                {
                    return Json(DbOVS.ErrorMessage(checkMessage));
                }
                DbOVS.Create(model, Permissions.ElectionsManagement.CreateGuid, ExecutionTime, DbCMS);
                UploadImage(upload.InputStream, model.ElectionCandidateGUID);
                try
                {
                    DbOVS.SaveChanges();
                    DbCMS.SaveChanges();
                    return Json(DbOVS.SingleUpdateMessage(DataTableNames.ElectionCandidatesDataTable, DbOVS.PrimaryKeyControl(model), DbOVS.RowVersionControls(Portal.SingleToList(model))));
                }
                catch (Exception ex)
                {
                    return Json(DbOVS.ErrorMessage(ex.Message));
                }
            }
            else
            {
                UploadImage(upload.InputStream, model.ElectionCandidateGUID);

            }
            return null;
        }

        private void UploadImage(Stream inputStream, Guid electionCandidateGUID)
        {
            #region Upload Image
            try
            {
                //to change it to take from Webconfig
                Image image = Image.FromFile(Server.MapPath(@"\\Uploads") + "\\OVS\\Templete\\00000000-0000-0000-0000-000000000000.jpg");

                if (inputStream != null)
                {
                    image = Image.FromStream(inputStream);
                    //Original Size
                    var dir = new FileInfo(Server.MapPath(@"\\Uploads") + "OVS\\CanidiatePhotos\\").Directory;
                    if (dir != null) dir.Create();
                    image.Save(Server.MapPath(@"\\Uploads") + "\\OVS\\CanidiatePhotos\\LG_" + electionCandidateGUID + ".jpg", ImageFormat.Jpeg);
                    //1024 x 1024 pixel
                    using (var newImage = new Portal().ScaleImage(image, 300, 300))
                    {
                        var directory = new FileInfo(Server.MapPath(@"\\Uploads") + "\\OVS\\CanidiatePhotos\\").Directory;
                        if (directory != null) directory.Create();
                        newImage.Save(Server.MapPath(@"\\Uploads") + "\\OVS\\CanidiatePhotos\\" + electionCandidateGUID + ".jpg", ImageFormat.Jpeg);
                    }
                    //100 x 100 pixel
                    using (var newImage = new Portal().ScaleImage(image, 100, 100))
                    {
                        var directory = new FileInfo(Server.MapPath(@"\\Uploads") + "\\OVS\\CanidiatePhotos\\").Directory;
                        if (directory != null) directory.Create();
                        newImage.Save(Server.MapPath(@"\\Uploads") + "\\OVS\\CanidiatePhotos\\XS_" + electionCandidateGUID + ".jpg", ImageFormat.Jpeg);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            #endregion
        }

        private void UpoloadImage(Stream inputStream)
        {
           
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ElectionCandidateUpdate(dataElectionCandidate model)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Update, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            if (!ModelState.IsValid)
                return PartialView("~/Areas/OVS/Views/Candidate/_CandidateUpdateModal.cshtml", model);


            DateTime ExecutionTime = DateTime.Now;

            DbOVS.Update(model, Permissions.ElectionsManagement.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbOVS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbOVS.SingleUpdateMessage(DataTableNames.ElectionCandidatesDataTable,
                    DbOVS.PrimaryKeyControl(model),
                    DbOVS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyElectionCandidate(model.ElectionCandidateGUID);
            }
            catch (Exception ex)
            {
                return Json(DbOVS.ErrorMessage(ex.Message));
            }


        }

      


        public ActionResult ElectionCandidateDelete(dataElectionCandidate model)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Delete, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            List<dataElectionCandidate> DeletedCandidate =
                DeleteElectionCandidates(new List<dataElectionCandidate> { model });

            try
            {
                DbOVS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbOVS.SingleDeleteMessage(DeletedCandidate, DataTableNames.ElectionCandidatesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyElectionCandidate(model.ElectionCandidateGUID);
            }
            catch (Exception ex)
            {
                return Json(DbOVS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ElectionCandidateRestore(dataElectionCandidate model)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Restore, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            if (ActiveElectionCandidate(model))
            {
                return Json(DbOVS.RecordExists());
            }

            List<dataElectionCandidate> RestoredCandidates = RestoreElectionCandidate(Portal.SingleToList(model));

            try
            {
                DbOVS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbOVS.SingleRestoreMessage(RestoredCandidates, DataTableNames.ElectionLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyElectionCandidate(model.ElectionCandidateGUID);
            }
            catch (Exception ex)
            {
                return Json(DbOVS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ElectionCandidatesDataTableDelete(List<dataElectionCandidate> models)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Delete, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            List<dataElectionCandidate> DeletedCandidates = DeleteElectionCandidates(models);

            try
            {
                DbOVS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbOVS.PartialDeleteMessage(DeletedCandidates, models,
                    DataTableNames.ElectionCandidatesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbOVS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ElectionCandidatesDataTableRestore(List<dataElectionCandidate> models)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Restore, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            List<dataElectionCandidate> RestoredCandidates = RestoreElectionCandidate(models);

            try
            {
                DbOVS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbOVS.PartialRestoreMessage(RestoredCandidates, models,
                    DataTableNames.ElectionCandidatesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbOVS.ErrorMessage(ex.Message));
            }
        }

        private List<dataElectionCandidate> DeleteElectionCandidates(List<dataElectionCandidate> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataElectionCandidate> DeletedElectionCandidates = new List<dataElectionCandidate>();

            string query = DbOVS.QueryBuilder(models, Permissions.ElectionsManagement.DeleteGuid, SubmitTypes.Delete, "");

            var candidates = DbOVS.Database.SqlQuery<dataElectionCandidate>(query).ToList();

            foreach (var candidate in candidates)
            {
                DeletedElectionCandidates.Add(DbOVS.Delete(candidate, ExecutionTime,
                    Permissions.ElectionsManagement.DeleteGuid, DbCMS));
            }

            return DeletedElectionCandidates;
        }

        private List<dataElectionCandidate> RestoreElectionCandidate(List<dataElectionCandidate> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataElectionCandidate> RestoredCandidates = new List<dataElectionCandidate>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbOVS.QueryBuilder(models, Permissions.ElectionLanguages.DeleteGuid, SubmitTypes.Restore,
                baseQuery);

            var candidates = DbOVS.Database.SqlQuery<dataElectionCandidate>(query).ToList();
            foreach (var candidate in candidates)
            {
                if (!ActiveElectionCandidate(candidate))
                {
                    RestoredCandidates.Add(DbOVS.Restore(candidate, Permissions.ElectionsManagement.DeleteGuid,
                        Permissions.ElectionsManagement.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredCandidates;
        }

        private JsonResult ConcrrencyElectionCandidate(Guid PK)
        {
            dataElectionCandidate dbModel = new dataElectionCandidate();

            var Candidate = DbOVS.dataElectionCandidate.Where(l => l.ElectionCandidateGUID == PK).FirstOrDefault();
            var dbCandidate = DbOVS.Entry(Candidate).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbCandidate, dbModel);

            if (Candidate.dataElectionCandidateRowVersion.SequenceEqual(dbModel.dataElectionCandidateRowVersion))
            {
                return Json(DbOVS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbOVS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveElectionCandidate(dataElectionCandidate model)
        {
            int electionCandidate = DbOVS.dataElectionCandidate
                .Where(x => x.EmailAddress == model.EmailAddress &&
                            x.ElectionGUID == model.ElectionGUID &&

                            x.Active).Count();
            if (electionCandidate > 0)
            {
                ModelState.AddModelError("Condition", "Candidate is already exists");
            }

            return (electionCandidate > 0);
        }

        #endregion
    }
}