using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.ViewModels;
using AutoMapper;
using IDC_DAL.Model;
using LinqKit;
using Spire.Barcode;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.IDC.Controllers
{
    public class CardIssuedsController : IDCBaseController
    {
        #region Card Issueds

        //[Route("IDC/CardIssuedsDataTable/{PK}")]
        public ActionResult CardIssuedsDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/IDC/Views/CardIndividualInfos/_CardIssuedsDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<dataCardIssued, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<dataCardIssued>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.CardIssued.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();

            var Result = DbIDC.dataCardIssued.AsNoTracking().AsExpandable().Where(x => x.CardIndividualInfoGUID == PK).Where(Predicate).Where(x => AuthorizedList.Contains("e156c022-ec72-4a5a-be09-163bd85c68ef,"+x.DutyStationGUID.ToString()))
                              .Select(x => new
                              {
                                  x.CardIndividualInfoGUID,
                                  x.CardIssuedGUID,
                                  x.DutyStationGUID,
                                  x.ExpirtionDate,
                                  x.IssueDate,
                                  IssueCode=DbIDC.codeCardIssueReason.Where(y=>y.IssueCode == x.IssueCode).FirstOrDefault().IssueDescription,
                                  Valid=x.Valid.Value? ((x.Valid.Value && x.PrintedBy==null)?x.Approved.Value? "Pending Print" : "Pending Validation" : "Valid"):"Collected",
                                  x.dataCardIssuedRowVersion,
                                  PrintedBy=x.PrintedBy,
                                  CreatedBy=x.CreateBy
                              });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult CardIssuedCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.CardIssued.Create, Apps.IDC))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            //Access is authorized by Access Action
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.CardIssued.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            string DutystationGUID= AuthorizedList.FirstOrDefault().Split(',')[1];
            var Ind = DbIDC.dataCardIndividualInfo.Where(x => x.CardIndividualInfoGUID == FK).FirstOrDefault();
            DateTime ExpirtionDate = new DateTime();
            //Damascues
            if (DutystationGUID.ToUpper() == "6D7397D6-3D7F-48FC-BFD2-18E69673AC92") {
                ExpirtionDate = Ind.Category == "REF" ? DateTime.Now.AddYears(1) : DateTime.Now.AddMonths(6);
            }
            else
            {
                ExpirtionDate = DateTime.Now.AddYears(1);
            }
          
            return PartialView("~/Areas/IDC/Views/CardIndividualInfos/_CardIssuedUpdateModal.cshtml",
                new dataCardIssued { CardIndividualInfoGUID = FK , IssueDate=DateTime.Now,
                    ExpirtionDate= ExpirtionDate,
                    DutyStationGUID=Guid.Parse(DutystationGUID)});
        }

        public ActionResult CardIssuedUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.CardIssued.Access, Apps.IDC))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/IDC/Views/CardIndividualInfos/_CardIssuedUpdateModal.cshtml", DbIDC.dataCardIssued.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CardIssuedCreate(dataCardIssued model)
        {
            if (!CMS.HasAction(Permissions.CardIssued.Create, Apps.IDC))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveCardIssued(model)) return PartialView("~/Areas/IDC/Views/CardIndividualInfos/_CardIssuedUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;
            model.IssueDate = DateTime.Now;
            model.Approved = DbIDC.codeCardIssueReason.Where(x => x.IssueCode == model.IssueCode).FirstOrDefault().Approval;
            model.Valid = true;
            model.Sequance = DbCMS.codeDutyStationsLanguages.Where(x => x.DutyStationGUID == model.DutyStationGUID).FirstOrDefault().DutyStationDescription.Substring(0, 3) + (DbIDC.dataCardIssued.Count()+1);
            model.CreateBy = CMS.GetFullName(UserGUID, LAN);
            model.CreateDate = DateTime.Now;

            var indivilualValue = DbIDC.dataCardIndividualInfo.Where(x => x.CardIndividualInfoGUID == model.CardIndividualInfoGUID).FirstOrDefault();
            model.Barcode = CreateBarcode(indivilualValue.IndividualID + "\t" + indivilualValue.CaseNumber + "\t" + indivilualValue.FullName_EN +  "\t" + indivilualValue.CountyCodeA3 + "\t" + indivilualValue.Category + "\t" +indivilualValue.Sex + "\t" + indivilualValue.DateOfBrith.Value.ToString("dd MMM yyyy") + "\t" + model.Sequance);
            model.Photo = DbPRG.dataPhotograph.Where(x => x.IndividualGUID == indivilualValue.CardIndividualInfoGUID && x.PhotoActive).FirstOrDefault().Photo;

            DbIDC.Create(model, Permissions.CardIssued.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbIDC.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbIDC.SingleUpdateMessage(DataTableNames.CardIssuedsDataTable, DbIDC.PrimaryKeyControl(model), DbIDC.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbIDC.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CardIssuedUpdate(dataCardIssued model)
        {
            if (!CMS.HasAction(Permissions.CardIssued.Update, Apps.IDC))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveCardIssued(model)) return PartialView("~/Areas/IDC/Views/CardIndividualInfos/_CardIssuedUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbIDC.Update(model, Permissions.CardIssued.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbIDC.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbIDC.SingleUpdateMessage(DataTableNames.CardIssuedsDataTable,
                    DbIDC.PrimaryKeyControl(model),
                    DbIDC.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyCardIssued(model.CardIssuedGUID);
            }
            catch (Exception ex)
            {
                return Json(DbIDC.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CardIssuedDelete(dataCardIssued model)
        {
            if (!CMS.HasAction(Permissions.CardIssued.Delete, Apps.IDC))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataCardIssued> DeletedCardIssueds = DeleteCardIssueds(new List<dataCardIssued> { model });

            try
            {
                DbIDC.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbIDC.SingleDeleteMessage(DeletedCardIssueds, DataTableNames.CardIssuedsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyCardIssued(model.CardIssuedGUID);
            }
            catch (Exception ex)
            {
                return Json(DbIDC.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CardIssuedRestore(dataCardIssued model)
        {
            if (!CMS.HasAction(Permissions.CardIssued.Restore, Apps.IDC))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveCardIssued(model))
            {
                return Json(DbIDC.RecordExists());
            }

            List<dataCardIssued> RestoredCardIssueds = RestoreCardIssueds(Portal.SingleToList(model));

            try
            {
                DbIDC.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbIDC.SingleRestoreMessage(RestoredCardIssueds, DataTableNames.CardIssuedsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyCardIssued(model.CardIssuedGUID);
            }
            catch (Exception ex)
            {
                return Json(DbIDC.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult CardIssuedsDataTableDelete(List<dataCardIssued> models)
        {
            if (!CMS.HasAction(Permissions.CardIssued.Delete, Apps.IDC))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataCardIssued> DeletedCardIssueds = DeleteCardIssueds(models);

            try
            {
                DbIDC.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbIDC.PartialDeleteMessage(DeletedCardIssueds, models, DataTableNames.CardIssuedsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbIDC.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult CardIssuedsDataTableRestore(List<dataCardIssued> models)
        {
            if (!CMS.HasAction(Permissions.CardIssued.Restore, Apps.IDC))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataCardIssued> RestoredCardIssueds = RestoreCardIssueds(models);

            try
            {
                DbIDC.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbIDC.PartialRestoreMessage(RestoredCardIssueds, models, DataTableNames.CardIssuedsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbIDC.ErrorMessage(ex.Message));
            }
        }

        private List<dataCardIssued> DeleteCardIssueds(List<dataCardIssued> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataCardIssued> DeletedCardIssueds = new List<dataCardIssued>();

            string query = DbIDC.QueryBuilder(models, Permissions.CardIssued.DeleteGuid, SubmitTypes.Delete, "");

            var CardIssueds = DbIDC.Database.SqlQuery<dataCardIssued>(query).ToList();

            foreach (var CardIssued in CardIssueds)
            {
                DeletedCardIssueds.Add(DbIDC.Delete(CardIssued, ExecutionTime, Permissions.CardIndividualInfo.DeleteGuid, DbCMS));
            }

            return DeletedCardIssueds;
        }

        private List<dataCardIssued> RestoreCardIssueds(List<dataCardIssued> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataCardIssued> RestoredCardIssueds = new List<dataCardIssued>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbIDC.QueryBuilder(models, Permissions.CardIssued.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var CardIssueds = DbIDC.Database.SqlQuery<dataCardIssued>(query).ToList();
            foreach (var CardIssued in CardIssueds)
            {
                if (!ActiveCardIssued(CardIssued))
                {
                    RestoredCardIssueds.Add(DbIDC.Restore(CardIssued, Permissions.CardIssued.DeleteGuid, Permissions.CardIssued.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredCardIssueds;
        }

        private JsonResult ConcrrencyCardIssued(Guid PK)
        {
            dataCardIssued dbModel = new dataCardIssued();

            var CardIssued = DbIDC.dataCardIssued.Where(l => l.CardIssuedGUID == PK).FirstOrDefault();
            var dbCardIssued = DbIDC.Entry(CardIssued).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbCardIssued, dbModel);

            if (CardIssued.dataCardIssuedRowVersion.SequenceEqual(dbModel.dataCardIssuedRowVersion))
            {
                return Json(DbIDC.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbIDC, dbModel, "CardIssuedsContainer"));
        }

        private bool ActiveCardIssued(dataCardIssued model)
        {
            model.IssueDate = DateTime.Now;
            int CardIssuedID = 0;
          
            if (model.IssueCode == "CC" || model.IssueCode == "PC")
            {
                CardIssuedID = DbIDC.dataCardIssued
                               .Where(x =>
                                           x.CardIndividualInfoGUID == model.CardIndividualInfoGUID &&
                                           (x.IssueCode == "VC" || x.IssueCode == "LC" || x.IssueCode == "NC" || x.IssueCode == "RSD") &&
                                           (x.IssueDate.Value.Year == model.IssueDate.Value.Year && x.IssueDate.Value.Month == model.IssueDate.Value.Month && x.IssueDate.Value.Day == model.IssueDate.Value.Day)&&
                                           x.CardIssuedGUID != model.CardIssuedGUID &&
                                           x.Active).Count();
                if (CardIssuedID == 0)
                {
                    ModelState.AddModelError("CardIssuedID", "[Correction Card or Printing Error Card] not valid This option should be in the same day of issuing the ID Card"); //From resource ?????? Amer 
                    CardIssuedID = 1;
                }
                else
                {
                    CardIssuedID = 0;
                }
            }
            if (model.IssueCode == "RC" || model.IssueCode == "NC")
            {
                CardIssuedID = DbIDC.dataCardIssued
                                .Where(x =>
                                            x.CardIndividualInfoGUID == model.CardIndividualInfoGUID &&

                                            model.IssueDate >= x.IssueDate && model.IssueDate < x.ExpirtionDate &&
                                            x.CardIssuedGUID != model.CardIssuedGUID &&
                                            x.Valid.Value &&
                                            x.Active).Count();
                if (CardIssuedID > 0)
                {
                    ModelState.AddModelError("CardIssuedID", "There is a valid card already exists!"); //From resource ?????? Amer  
                }

            }
            var ProcessStatusCode = (from c in DbPRG.dataIndividual
                                     where c.IndividualGUID == model.CardIndividualInfoGUID
                                     select c.ProcessStatusCode).FirstOrDefault();
            if (ProcessStatusCode != "A")
            {
                CardIssuedID++;
                ModelState.AddModelError("CardIssuedID", "The Individual Not Active!"); //From resource ?????? Amer  
                
            }
           

            return (CardIssuedID > 0);
        }

        public byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            return ms.ToArray();
        }
        public byte[] CreateBarcode(string Individual)
        {
            //set the configuration of barcode
            BarcodeSettings settings = new BarcodeSettings();
            string data = Individual;
            string type = "QRCode";
            Image barcode;

            settings.Data2D = data;
            settings.Data = data;
            settings.Type = (BarCodeType)Enum.Parse(typeof(BarCodeType), type);

            short fontSize = 12;
            string font = "SimSun";
            settings.TextFont = new System.Drawing.Font(font, fontSize, FontStyle.Bold);
            short barHeight = 15;
            settings.BarHeight = barHeight;
            settings.ShowText = false;
            //generate the barcode use the settings
            BarCodeGenerator generator = new BarCodeGenerator(settings);
            barcode = generator.GenerateImage();

            MemoryStream ms = new MemoryStream();
            barcode.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            return ms.ToArray();

        }
        #endregion
    }
}