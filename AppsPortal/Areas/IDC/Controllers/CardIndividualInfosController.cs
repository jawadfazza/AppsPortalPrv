using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.ViewModels;
using AutoMapper;
using IDC_DAL.Model;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.IDC.Controllers
{
    public class CardIndividualInfosController : IDCBaseController
    {
        #region Card Individual Info

        public ActionResult Index()
        {
            return View();
        }

        [Route("IDC/CardIndividualInfos/")]
        public ActionResult CardIndividualInfosIndex()
        {
            if (!CMS.HasAction(Permissions.CardIndividualInfo.Access, Apps.IDC))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/IDC/Views/CardIndividualInfos/Index.cshtml");
        }

        [Route("IDC/CardIndividualInfosDataTable/")]
        public JsonResult CardIndividualInfosDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<CardIndividualInfosDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<CardIndividualInfosDataTableModel>(DataTable.Filters);
            }
            ///
            foreach (var filter in DataTable.Filters.FilterRules)
            {
                if (filter.Field == "CaseNumber")
                {
                    var CardIndividual = (from a in DbIDC.dataCardIndividualInfo where a.CaseNumber == filter.FieldData select a).ToList();
                    var PrgresIndiviuals = (from a in DbPRG.dataProcessGroup
                                            join b in DbPRG.dataIndividualProcessGroup on a.ProcessingGroupGUID equals b.ProcessingGroupGUID
                                            join c in DbPRG.dataIndividual on b.IndividualGUID equals c.IndividualGUID
                                            where a.ProcessingGroupNumber == filter.FieldData /*&& c.RefugeeStatusCode != "NOC"*/
                                            select new dataCardIndividualInfo
                                            {
                                                CardIndividualInfoGUID = c.IndividualGUID,
                                                IndividualID = c.IndividualID,
                                                ArrivalDate = c.ArrivalDate,
                                                ArrivalEstimation = c.ArrivalDateEstimate,
                                                CaseNumber = a.ProcessingGroupNumber,
                                                Category = c.RefugeeStatusCode,
                                                DateOfBrith = c.DateofBirth,
                                                FullName_AR = c.VerbalName,
                                                FullName_EN = c.GivenName + " " + c.FamilyName,
                                                Sex = c.SexCode,
                                                CountyCodeA3 = c.OriginCountryCode,
                                                Active = c.ProcessStatusCode == "A" ? true : false,
                                                ShowNationality= c.OriginCountryCode==c.NationalityCode?true:false
                                                
                                            }).ToList();
                    var CardList = CardIndividual.Select(x => x.CardIndividualInfoGUID).ToList();
                    var PrgresToAdd = PrgresIndiviuals.Where(x => !CardList.Contains(x.CardIndividualInfoGUID) && x.Active).ToList();
                    var PrgresToUpdate  = PrgresIndiviuals.Where(x => CardList.Contains(x.CardIndividualInfoGUID) ).ToList();

                    //update individuals
                    foreach (var ind in PrgresToUpdate)
                    {
                        var found = (from a in CardIndividual where a.CardIndividualInfoGUID == ind.CardIndividualInfoGUID select a).FirstOrDefault();
                        found.FullName_AR = ind.FullName_AR;
                        found.FullName_EN = ind.FullName_EN;
                        found.IndividualID = ind.IndividualID;
                        found.CaseNumber = ind.CaseNumber;
                        found.ArrivalDate = ind.ArrivalDate;
                        found.DateOfBrith = ind.DateOfBrith;
                        found.Category = ind.Category;
                        found.Sex = ind.Sex;
                        found.CountyCodeA3 = ind.CountyCodeA3;
                        found.ArrivalEstimation = ind.ArrivalEstimation;
                        found.ShowNationality = ind.ShowNationality;
                        found.Active = ind.Active;
                        DbIDC.Update(found, Permissions.CardIndividualInfo.UpdateGuid, DateTime.Now, DbCMS);
                    }
                    //Add New individual
                    foreach (var ind in PrgresToAdd)
                    {
                        var found = (from a in DbIDC.dataCardIndividualInfo where a.CardIndividualInfoGUID == ind.CardIndividualInfoGUID select a).FirstOrDefault();
                        if (found == null)
                        {
                            DbIDC.Create(ind, Permissions.CardIndividualInfo.CreateGuid, DateTime.Now, DbCMS);
                        }
                        else//Split case
                        {
                            found.FullName_AR = ind.FullName_AR;
                            found.FullName_EN = ind.FullName_EN;
                            found.IndividualID = ind.IndividualID;
                            found.CaseNumber = ind.CaseNumber;
                            found.ArrivalDate = ind.ArrivalDate;
                            found.DateOfBrith = ind.DateOfBrith;
                            found.Category = ind.Category;
                            found.Sex = ind.Sex;
                            found.CountyCodeA3 = ind.CountyCodeA3;
                            found.ArrivalEstimation = ind.ArrivalEstimation;
                            found.ShowNationality = ind.ShowNationality;
                            found.Active = ind.Active;
                        }
                    }
                      
                }

            }
            DbIDC.SaveChanges();

            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.CardIndividualInfo.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();

            var All = (from a in DbIDC.dataCardIndividualInfo.AsExpandable().Where(x=>x.Category!= "NOC")
                       select new CardIndividualInfosDataTableModel
                       {
                           CardIndividualInfoGUID=a.CardIndividualInfoGUID,
                           CaseNumber = a.CaseNumber,
                           ArrivalDate = a.ArrivalDate,
                           Category = a.Category,
                           DateOfBrith = a.DateOfBrith,
                           FullName_AR = a.FullName_AR,
                           FullName_EN = a.FullName_EN,
                           IndividualID = a.IndividualID,
                           Sex = a.Sex,
                           HasIdCard=DbIDC.dataCardIssued.Where(x=>x.Valid.Value && DateTime.Now >= x.IssueDate.Value  && DateTime.Now<=x.ExpirtionDate.Value && x.CardIndividualInfoGUID==a.CardIndividualInfoGUID && x.Active).FirstOrDefault()!=null?true:false,
                           Active = a.Active,
                           dataCardIndividualInfoRowVersion = a.dataCardIndividualInfoRowVersion
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<CardIndividualInfosDataTableModel> Result = Mapper.Map<List<CardIndividualInfosDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("IDC/CardIndividualInfos/Create/")]
        public ActionResult CardIndividualInfoCreate()
        {
            if (!CMS.HasAction(Permissions.CardIndividualInfo.Create, Apps.IDC))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/IDC/Views/CardIndividualInfos/CardIndividualInfo.cshtml", new CardIndividualInfoUpdateModel());
        }

        [Route("IDC/CardIndividualInfos/Update/{PK}")]
        public ActionResult CardIndividualInfoUpdate(Guid PK)
        {
            //if (!CMS.HasAction(Permissions.CardIndividualInfo.Access, Apps.IDC))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            var model = (from a in DbIDC.dataCardIndividualInfo.WherePK(PK)
                         join b in DbIDC.dataCardIssued.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.dataCardIndividualInfo.DeletedOn))
                         on a.CardIndividualInfoGUID equals b.CardIndividualInfoGUID into LJ1
                         from R1 in LJ1.DefaultIfEmpty()
                         select new CardIndividualInfoUpdateModel
                         {
                             CardIndividualInfoGUID = a.CardIndividualInfoGUID,
                             FullName_EN = a.FullName_EN,
                             FullName_AR = a.FullName_AR,
                             ArrivalDate = a.ArrivalDate,
                             Sex = a.Sex,
                             IndividualID = a.IndividualID,
                             DateOfBrith = a.DateOfBrith,
                             Category = a.Category,
                             ArrivalEstimation = a.ArrivalEstimation,
                             CaseNumber=a.CaseNumber,
                             CountyCodeA3=a.CountyCodeA3,
                             Active = a.Active,
                             dataCardIndividualInfoRowVersion = a.dataCardIndividualInfoRowVersion,
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("CardIndividualInfo", "CardIndividualInfos", new { Area = "IDC" }));

            return View("~/Areas/IDC/Views/CardIndividualInfos/CardIndividualInfo.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CardIndividualInfoCreate(CardIndividualInfoUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.CardIndividualInfo.Create, Apps.IDC))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveCardIndividualInfo(model)) return PartialView("~/Areas/IDC/Views/CardIndividualInfos/_CardIndividualInfoForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            dataCardIndividualInfo CardIndividualInfo = Mapper.Map(model, new dataCardIndividualInfo());
            CardIndividualInfo.CardIndividualInfoGUID = EntityPK;
            DbIDC.Create(CardIndividualInfo, Permissions.CardIndividualInfo.CreateGuid, ExecutionTime, DbCMS);

            dataCardIssued CardIssued = Mapper.Map(model, new dataCardIssued());
            CardIssued.CardIndividualInfoGUID = EntityPK;

            DbIDC.Create(CardIssued, Permissions.CardIndividualInfo.CreateGuid, ExecutionTime, DbCMS);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.CardIssuedsDataTable, ControllerContext, "CardIssuedsContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.CardIndividualInfo.Create, Apps.IDC, new UrlHelper(Request.RequestContext).Action("Create", "CardIndividualInfos", new { Area = "IDC" })), Container = "CardIndividualInfoFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.CardIndividualInfo.Update, Apps.IDC), Container = "CardIndividualInfoFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.CardIndividualInfo.Delete, Apps.IDC), Container = "CardIndividualInfoFormControls" });

            try
            {
                DbIDC.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbIDC.SingleCreateMessage(DbIDC.PrimaryKeyControl(CardIndividualInfo), DbIDC.RowVersionControls(CardIndividualInfo, CardIssued), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbIDC.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CardIndividualInfoUpdate(CardIndividualInfoUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.CardIndividualInfo.Update, Apps.IDC))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveCardIndividualInfo(model)) return PartialView("~/Areas/IDC/Views/CardIndividualInfos/_CardIndividualInfoForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            dataCardIndividualInfo CardIndividualInfo = Mapper.Map(model, new dataCardIndividualInfo());
            DbIDC.Update(CardIndividualInfo, Permissions.CardIndividualInfo.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbIDC.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbIDC.SingleUpdateMessage(null, null, DbIDC.RowVersionControls(new List<dataCardIndividualInfo>() { CardIndividualInfo })));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyCardIndividualInfo(model.CardIndividualInfoGUID);
            }
            catch (Exception ex)
            {
                return Json(DbIDC.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CardIndividualInfoDelete(dataCardIndividualInfo model)
        {
            if (!CMS.HasAction(Permissions.CardIndividualInfo.Delete, Apps.IDC))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataCardIndividualInfo> DeletedCardIndividualInfo = DeleteCardIndividualInfos(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.CardIndividualInfo.Restore, Apps.IDC), Container = "CardIndividualInfoFormControls" });

            try
            {
                int CommitedRows = DbIDC.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbIDC.SingleDeleteMessage(CommitedRows, DeletedCardIndividualInfo.FirstOrDefault(), "CardIssuedsContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyCardIndividualInfo(model.CardIndividualInfoGUID);
            }
            catch (Exception ex)
            {
                return Json(DbIDC.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CardIndividualInfoRestore(dataCardIndividualInfo model)
        {
            if (!CMS.HasAction(Permissions.CardIndividualInfo.Restore, Apps.IDC))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveCardIndividualInfo(model))
            {
                return Json(DbIDC.RecordExists());
            }

            List<dataCardIndividualInfo> RestoredCardIndividualInfos = RestoreCardIndividualInfos(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.CardIndividualInfo.Create, Apps.IDC, new UrlHelper(Request.RequestContext).Action("CardIndividualInfoCreate", "Configuration", new { Area = "IDC" })), Container = "CardIndividualInfoFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.CardIndividualInfo.Update, Apps.IDC), Container = "CardIndividualInfoFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.CardIndividualInfo.Delete, Apps.IDC), Container = "CardIndividualInfoFormControls" });

            try
            {
                int CommitedRows = DbIDC.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbIDC.SingleRestoreMessage(CommitedRows, RestoredCardIndividualInfos, DbIDC.PrimaryKeyControl(RestoredCardIndividualInfos.FirstOrDefault()), Url.Action(DataTableNames.CardIssuedsDataTable, Portal.GetControllerName(ControllerContext)), "CardIssuedsContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyCardIndividualInfo(model.CardIndividualInfoGUID);
            }
            catch (Exception ex)
            {
                return Json(DbIDC.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult CardIndividualInfosDataTableDelete(List<dataCardIndividualInfo> models)
        {
            if (!CMS.HasAction(Permissions.CardIndividualInfo.Delete, Apps.IDC))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataCardIndividualInfo> DeletedCardIndividualInfos = DeleteCardIndividualInfos(models);

            try
            {
                DbIDC.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbIDC.PartialDeleteMessage(DeletedCardIndividualInfos, models, DataTableNames.CardIndividualInfosDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbIDC.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult CardIndividualInfosDataTableRestore(List<dataCardIndividualInfo> models)
        {
            if (!CMS.HasAction(Permissions.CardIndividualInfo.Restore, Apps.IDC))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataCardIndividualInfo> RestoredCardIndividualInfos = RestoreCardIndividualInfos(models);

            try
            {
                DbIDC.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbIDC.PartialRestoreMessage(RestoredCardIndividualInfos, models, DataTableNames.CardIndividualInfosDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbIDC.ErrorMessage(ex.Message));
            }
        }

        private List<dataCardIndividualInfo> DeleteCardIndividualInfos(List<dataCardIndividualInfo> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<dataCardIndividualInfo> DeletedCardIndividualInfos = new List<dataCardIndividualInfo>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbIDC.QueryBuilder(models, Permissions.CardIndividualInfo.DeleteGuid, SubmitTypes.Delete, baseQuery);

            var Records = DbIDC.Database.SqlQuery<dataCardIndividualInfo>(query).ToList();
            foreach (var record in Records)
            {
                DeletedCardIndividualInfos.Add(DbIDC.Delete(record, ExecutionTime, Permissions.CardIndividualInfo.DeleteGuid, DbCMS));
            }

            var CardIssueds = DeletedCardIndividualInfos.SelectMany(a => a.dataCardIssued).Where(l => l.Active).ToList();
            foreach (var CardIssued in CardIssueds)
            {
                DbIDC.Delete(CardIssued, ExecutionTime, Permissions.CardIndividualInfo.DeleteGuid, DbCMS);
            }
            return DeletedCardIndividualInfos;
        }

        private List<dataCardIndividualInfo> RestoreCardIndividualInfos(List<dataCardIndividualInfo> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<dataCardIndividualInfo> RestoredCardIndividualInfos = new List<dataCardIndividualInfo>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbIDC.QueryBuilder(models, Permissions.CardIndividualInfo.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Records = DbIDC.Database.SqlQuery<dataCardIndividualInfo>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveCardIndividualInfo(record))
                {
                    RestoredCardIndividualInfos.Add(DbIDC.Restore(record, Permissions.CardIndividualInfo.DeleteGuid, Permissions.CardIndividualInfo.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            var CardIssueds = RestoredCardIndividualInfos.SelectMany(x => x.dataCardIssued.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var CardIssued in CardIssueds)
            {
                DbIDC.Restore(CardIssued, Permissions.CardIndividualInfo.DeleteGuid, Permissions.CardIndividualInfo.RestoreGuid, RestoringTime, DbCMS);
            }

            return RestoredCardIndividualInfos;
        }

        private JsonResult ConcurrencyCardIndividualInfo(Guid PK)
        {
            CardIndividualInfoUpdateModel dbModel = new CardIndividualInfoUpdateModel();

            var CardIndividualInfo = DbIDC.dataCardIndividualInfo.Where(x => x.CardIndividualInfoGUID == PK).FirstOrDefault();
            var dbCardIndividualInfo = DbIDC.Entry(CardIndividualInfo).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbCardIndividualInfo, dbModel);

            var CardIssued = DbIDC.dataCardIssued.Where(x => x.CardIndividualInfoGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.dataCardIndividualInfo.DeletedOn) ).FirstOrDefault();
            var dbCardIssued = DbIDC.Entry(CardIssued).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbCardIssued, dbModel);

            if (CardIndividualInfo.dataCardIndividualInfoRowVersion.SequenceEqual(dbModel.dataCardIndividualInfoRowVersion) )
            {
                return Json(DbIDC.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbIDC, dbModel, "CardIssuedsContainer"));
        }

        private bool ActiveCardIndividualInfo(Object model)
        {
            //dataCardIssued CardIndividualInfo = Mapper.Map(model, new dataCardIssued());
            //int CardIndividualInfoDescription = DbIDC.dataCardIssued
            //                        .Where(x => x.i == MedicalPharmacy.MedicalPharmacyDescription &&
            //                                    x.CardIndividualInfoGUID == CardIndividualInfo.CardIndividualInfoGUID &&
            //                                    x.Active).Count();
            //if (CardIndividualInfoDescription > 0)
            //{
            //    ModelState.AddModelError("CardIndividualInfoDescription", "IndividualInfo is already exists");
            //}
            return false;
        }

        #endregion
    }
}