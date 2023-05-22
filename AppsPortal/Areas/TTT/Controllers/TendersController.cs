using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AutoMapper;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using TTT_DAL.Model;
using AppsPortal.ViewModels;
using DocumentFormat.OpenXml.Packaging;
using System.IO;
using System.Text.RegularExpressions;


namespace AppsPortal.Areas.TTT.Controllers
{
    public class TendersController : TTTBaseController
    {
        public ActionResult Index()
        {
            return View("~/Areas/TTT/Views/Tenders/Index.cshtml");
        }

        [Route("TTT/Tenders/TendersDataTable/")]
        public JsonResult TendersDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<TendersDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<TendersDataTableModel>(DataTable.Filters);
            }

            var All = (from a in DbTTT.dataTender.AsExpandable()
                       join b in DbTTT.codeTablesValuesLanguages.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on a.TenderTypeGUID equals b.ValueGUID
                       join c in DbTTT.codeDutyStationsLanguages.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on a.ProcurementPlanLine equals c.DutyStationGUID
                       join d in DbTTT.codeTablesValuesLanguages.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on a.TenderStatusGUID equals d.ValueGUID
                       join e in DbTTT.userPersonalDetailsLanguage.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on a.CreatedBy equals e.UserGUID into LJCreated
                       join f in DbTTT.userPersonalDetailsLanguage.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on a.LastUpdatedBy equals f.UserGUID into LJUpdated
                       from RJCreatedBy in LJCreated.DefaultIfEmpty()
                       from RJLastUpdatedBy in LJUpdated.DefaultIfEmpty()
                       select new TendersDataTableModel
                       {
                           TenderGUID = a.TenderGUID,
                           TenderSequence = a.TenderSequence,
                           TenderReference = a.TenderReference,
                           TenderType = b.ValueDescription,
                           TenderTypeGUID = a.TenderTypeGUID.ToString(),
                           TenderSubject = a.TenderSubject,
                           //TenderYear = a.TenderYear.ToString(),
                           ProcurementPlanLine = c.DutyStationDescription,
                           TenderStatusDescription = d.ValueDescription,
                           CreatedByGUID = RJCreatedBy.UserGUID.ToString(),
                           CreatedBy = RJCreatedBy.FirstName + " " + RJCreatedBy.Surname,
                           LastUpdatedBy = RJLastUpdatedBy.FirstName + " " + RJLastUpdatedBy.Surname,
                           Active = a.Active,
                           dataTenderRowVersion = a.dataTenderRowVersion
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<TendersDataTableModel> Result = Mapper.Map<List<TendersDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);

        }

        [Route("TTT/Tenders/Create/")]
        public ActionResult TenderCreate()
        {
            if (!CMS.HasAction(Permissions.Tenders.Create, Apps.TTT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            TenderUpdateModel model = new TenderUpdateModel
            {
                TenderReminderDays = 1,
            };
            return View("~/Areas/TTT/Views/Tenders/Tender.cshtml", model);
        }

        [Route("TTT/Tenders/Update/{PK}")]
        public ActionResult TenderUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.Tenders.Update, Apps.TTT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            TenderUpdateModel model = new TenderUpdateModel();
            var result = (from a in DbTTT.dataTender.AsNoTracking().Where(x => x.TenderGUID == PK)
                          join b in DbTTT.dataTenderRequisition.AsNoTracking() on a.TenderGUID equals b.TenderGUID into LJb
                          from Rb in LJb.DefaultIfEmpty()
                          join c in DbTTT.dataTenderingAndEvaluation.AsNoTracking() on a.TenderGUID equals c.TenderGUID into LJc
                          from Rc in LJc.DefaultIfEmpty()
                          join d in DbTTT.dataTenderEndorsementsAndApprovals.AsNoTracking() on a.TenderGUID equals d.TenderGUID into LJd
                          from Rd in LJd.DefaultIfEmpty()
                          join e in DbTTT.dataTenderContractInformation.AsNoTracking() on a.TenderGUID equals e.TenderGUID into LJe
                          from Re in LJe.DefaultIfEmpty()
                          join f in DbTTT.dataTenderObservationAndRemarks.AsNoTracking() on a.TenderGUID equals f.TenderGUID into LJf
                          from Rf in LJf.DefaultIfEmpty()
                          join j in DbTTT.dataTenderAwardedCompanies.AsNoTracking().Where(x => x.Active) on a.TenderGUID equals j.TenderGUID into LJj
                          from Rj in LJj.DefaultIfEmpty()
                          join h in DbTTT.dataTenderRequisitionFocalPoints.AsNoTracking().Where(x => x.Active) on Rb.TenderRequisitionGUID equals h.TenderRequisitionGUID into LJh
                          from Rh in LJh.DefaultIfEmpty()
                          select new
                          {
                              dataTender = a,
                              tenderRequisition = Rb,
                              tenderingAndEvaluation = Rc,
                              tenderEndorsementsAndApprovals = Rd,
                              tenderContractInformation = Re,
                              tenderObservationAndRemarks = Rf,
                              tenderAwardedCompanies = Rj,
                              tenderRequisitionFocalPoints = Rh
                          }).ToList();



            model = Mapper.Map(result.FirstOrDefault().dataTender, model);
            model = Mapper.Map(result.FirstOrDefault().tenderRequisition, model);
            model = Mapper.Map(result.FirstOrDefault().tenderingAndEvaluation, model);
            model = Mapper.Map(result.FirstOrDefault().tenderEndorsementsAndApprovals, model);
            model = Mapper.Map(result.FirstOrDefault().tenderContractInformation, model);
            model = Mapper.Map(result.FirstOrDefault().tenderObservationAndRemarks, model);

            model.TenderReferenceDisplay = result.FirstOrDefault().dataTender.TenderReference;
            model.TenderSequenceDisplay = result.FirstOrDefault().dataTender.TenderSequence;
            model.TenderBoxNumbers = result.FirstOrDefault().tenderingAndEvaluation.TenderBoxNumber.ToString().Split(',').ToList();
            model.TenderBoxNumber = result.FirstOrDefault().tenderingAndEvaluation.TenderBoxNumber.ToString();

            try
            {
                model.AwardedCompanyGUIDs = result.Select(x => x.tenderAwardedCompanies.AwardedCompanyGUID).ToList();
            }
            catch { }

            try
            {
                model.FocalPointRUGUID = result.Select(x => x.tenderRequisitionFocalPoints.FocalPointRUGUID).ToList();

                //if(model.FocalPointRUGUID != null)
                //{
                //    (from a in DbCMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active)
                //     where model.FocalPointRUGUID.Contains(a.UserGUID)
                //     orderby a.FirstName, a.Surname
                //     select new
                //     {
                //         Value = a.UserGUID.ToString(),
                //         Text = a.FirstName + " " + a.Surname
                //     }).ToList();
                //}

            }
            catch { }

            model.FirstOpenCheck = true;
            return View("~/Areas/TTT/Views/Tenders/Tender.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult TenderCreate(TenderUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.Tenders.Create, Apps.TTT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ValidateTender(model))
            {
                model.Active = true;
                model.TenderGUID = Guid.Empty;
                model.TenderReferenceDisplay = model.TenderReference;
                model.TenderSequenceDisplay = model.TenderSequence;
                return PartialView("~/Areas/TTT/Views/Tenders/_TenderForm.cshtml", model);
            }


            DateTime ExecutionTime = DateTime.Now;
            Guid EntityPK = Guid.NewGuid();
            if (model.TenderGUID != Guid.Empty)
            {
                EntityPK = model.TenderGUID;
            }

            TenderRefAndSeq refAndSeq = GetReferenceNumber(model.TenderTypeGUID);
            dataTender dataTender = Mapper.Map(model, new dataTender());
            dataTender.TenderGUID = EntityPK;
            dataTender.CreatedBy = UserGUID;
            dataTender.TenderSequence = refAndSeq.TenderSequence;
            dataTender.TenderReference = refAndSeq.TenderReference;
            dataTender.TenderYear = DateTime.Now.Year;
            //dataTender.TenderSequence = current;
            //var tenderRef = dataTender.TenderReference.Split('-').ToList();
            //tenderRef[tenderRef.Count - 1] = current.ToString();
            //dataTender.TenderReference = string.Join("-", tenderRef);
            DbTTT.Create(dataTender, Permissions.Tenders.CreateGuid, ExecutionTime, DbCMS);

            dataTenderRequisition dataTenderRequisition = Mapper.Map(model, new dataTenderRequisition());
            dataTenderRequisition.TenderGUID = EntityPK;
            DbTTT.Create(dataTenderRequisition, Permissions.Tenders.CreateGuid, ExecutionTime, DbCMS);

            dataTenderingAndEvaluation dataTenderingAndEvaluation = Mapper.Map(model, new dataTenderingAndEvaluation());
            dataTenderingAndEvaluation.TenderGUID = EntityPK;
            string tenderBoxNumbers = string.Join(",", model.TenderBoxNumbers);
            dataTenderingAndEvaluation.TenderBoxNumber = tenderBoxNumbers;
            DbTTT.Create(dataTenderingAndEvaluation, Permissions.Tenders.CreateGuid, ExecutionTime, DbCMS);

            dataTenderEndorsementsAndApprovals dataTenderEndorsementsAndApprovals = Mapper.Map(model, new dataTenderEndorsementsAndApprovals());
            dataTenderEndorsementsAndApprovals.TenderGUID = EntityPK;
            DbTTT.Create(dataTenderEndorsementsAndApprovals, Permissions.Tenders.CreateGuid, ExecutionTime, DbCMS);

            dataTenderContractInformation dataTenderContractInformation = Mapper.Map(model, new dataTenderContractInformation());
            dataTenderContractInformation.TenderGUID = EntityPK;
            DbTTT.Create(dataTenderContractInformation, Permissions.Tenders.CreateGuid, ExecutionTime, DbCMS);

            dataTenderObservationAndRemarks dataTenderObservationAndRemarks = Mapper.Map(model, new dataTenderObservationAndRemarks());
            dataTenderObservationAndRemarks.TenderGUID = EntityPK;
            DbTTT.Create(dataTenderObservationAndRemarks, Permissions.Tenders.CreateGuid, ExecutionTime, DbCMS);

            if (model.AwardedCompanyGUIDs != null)
            {
                List<dataTenderAwardedCompanies> dataTenderAwardedCompaniesList = new List<dataTenderAwardedCompanies>();
                foreach (var item in model.AwardedCompanyGUIDs)
                {
                    dataTenderAwardedCompanies dataTenderAwardedCompanies = new dataTenderAwardedCompanies();
                    dataTenderAwardedCompanies.TenderGUID = EntityPK;
                    dataTenderAwardedCompanies.AwardedCompanyGUID = item;
                    dataTenderAwardedCompaniesList.Add(dataTenderAwardedCompanies);
                }
                DbTTT.CreateBulk(dataTenderAwardedCompaniesList, Permissions.Tenders.CreateGuid, ExecutionTime, DbCMS);
            }

            if (model.FocalPointRUGUID != null)
            {
                List<dataTenderRequisitionFocalPoints> dataTenderRequisitionFocalPointsList = new List<dataTenderRequisitionFocalPoints>();
                foreach (var item in model.FocalPointRUGUID)
                {
                    dataTenderRequisitionFocalPoints dataTenderRequisitionFocalPoints = new dataTenderRequisitionFocalPoints();
                    dataTenderRequisitionFocalPoints.TenderRequisitionGUID = dataTenderRequisition.TenderRequisitionGUID;
                    dataTenderRequisitionFocalPoints.FocalPointRUGUID = item;
                    dataTenderRequisitionFocalPointsList.Add(dataTenderRequisitionFocalPoints);
                }
                DbTTT.CreateBulk(dataTenderRequisitionFocalPointsList, Permissions.Tenders.CreateGuid, ExecutionTime, DbCMS);

            }

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.Tenders.Create, Apps.TTT, new UrlHelper(Request.RequestContext).Action("Tenders/Create", "Tenders", new { Area = "TTT" })), Container = "TenderFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.Tenders.Update, Apps.TTT), Container = "TenderFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.Tenders.Delete, Apps.TTT), Container = "TenderFormControls" });

            try
            {
                DbTTT.SaveChanges();
                DbCMS.SaveChanges();

                var createdByRecord = (from a in DbTTT.StaffCoreData.Where(x => x.Active && x.UserGUID == UserGUID)
                                       join b in DbTTT.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == "EN") on a.UserGUID equals b.UserGUID
                                       select new
                                       {
                                           FullName = b.FirstName + " " + b.Surname,
                                           EmailAddress = a.EmailAddress
                                       }).FirstOrDefault();

                var BuyerRecord = (from a in DbTTT.StaffCoreData.Where(x => x.Active && x.UserGUID == dataTenderingAndEvaluation.BuyerGUID)
                                   join b in DbTTT.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == "EN") on a.UserGUID equals b.UserGUID
                                   select new
                                   {
                                       FullName = b.FirstName + " " + b.Surname,
                                       EmailAddress = a.EmailAddress
                                   }).FirstOrDefault();

                var superVisorRecord = (from a in DbTTT.StaffCoreData.Where(x => x.Active && x.UserGUID == dataTenderingAndEvaluation.SupervisorGUID)
                                        join b in DbTTT.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == "EN") on a.UserGUID equals b.UserGUID
                                        select new
                                        {
                                            FullName = b.FirstName + " " + b.Surname,
                                            EmailAddress = a.EmailAddress
                                        }).FirstOrDefault();
                string recipientEmail = createdByRecord.EmailAddress + ";" + BuyerRecord.EmailAddress + ";" + superVisorRecord.EmailAddress;

                new Email().SendCreateTenderConfirmation(recipientEmail, null, dataTender.TenderReference, DateTime.Now.ToShortDateString(), createdByRecord.FullName, "Tenders", "New Tender Created", dataTender.TenderGUID);

                List<PrimaryKeyControl> primaryKeyControls = new List<PrimaryKeyControl>();
                primaryKeyControls.Add(DbTTT.PrimaryKeyControl(dataTender));
                primaryKeyControls.Add(new PrimaryKeyControl { ControlID = "TenderRequisitionGUID", Value = dataTenderRequisition.TenderRequisitionGUID.ToString() });
                primaryKeyControls.Add(new PrimaryKeyControl { ControlID = "TenderingAndEvaluationGUID", Value = dataTenderingAndEvaluation.TenderingAndEvaluationGUID.ToString() });
                primaryKeyControls.Add(new PrimaryKeyControl { ControlID = "TenderEndorsementsAndApprovalsGUID", Value = dataTenderEndorsementsAndApprovals.TenderEndorsementsAndApprovalsGUID.ToString() });
                primaryKeyControls.Add(new PrimaryKeyControl { ControlID = "TenderContractInformationGUID", Value = dataTenderContractInformation.TenderContractInformationGUID.ToString() });
                primaryKeyControls.Add(new PrimaryKeyControl { ControlID = "TenderObservationAndRemarksGUID", Value = dataTenderObservationAndRemarks.TenderObservationAndRemarksGUID.ToString() });

                model.dataTenderRowVersion = dataTender.dataTenderRowVersion;
                model.dataTenderRequisitionRowVersion = dataTenderRequisition.dataTenderRequisitionRowVersion;
                model.dataTenderingAndEvaluationRowVersion = dataTenderingAndEvaluation.dataTenderingAndEvaluationRowVersion;
                model.dataTenderEndorsementsAndApprovalsRowVersion = dataTenderEndorsementsAndApprovals.dataTenderEndorsementsAndApprovalsRowVersion;
                model.dataTenderContractInformationRowVersion = dataTenderContractInformation.dataTenderContractInformationRowVersion;
                model.dataTenderObservationAndRemarksRowVersion = dataTenderObservationAndRemarks.dataTenderObservationAndRemarksRowVersion;

                string callBackFunc = "$('#TenderSequenceDisplay').prop('disabled', true);";
                callBackFunc += "$('#TenderSequenceDisplay').prop('readonly', true);";
                callBackFunc += "$('#TenderReferenceDisplay').prop('disabled', true);";
                callBackFunc += "$('#TenderReferenceDisplay').prop('readonly', true);";
                callBackFunc += "RefreshTenderReferences('" + dataTender.TenderReference + "','" + dataTender.TenderSequence + "');";

                return Json(DbTTT.SingleCreateMessage(primaryKeyControls, DbTTT.RowVersionControls(Portal.SingleToList(model)), null, callBackFunc, UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult TenderUpdate(TenderUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.Tenders.Update, Apps.TTT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            //if (!ModelState.IsValid || ActiveApplication(model)) return PartialView("~/Areas/CMS/Views/Codes/Applications/_ApplicationForm.cshtml", model);
            DateTime ExecutionTime = DateTime.Now;
            if (!ValidateTender(model))
            {
                return PartialView("~/Areas/TTT/Views/Tenders/_TenderForm.cshtml", model);
            }

            //model.TenderBoxNumber = string.Join(",", model.TenderBoxNumbers);

            dataTender dataTender = Mapper.Map(model, new dataTender());
            dataTender.LastUpdatedBy = UserGUID;



            var tenderTypeDesc = (from a in DbTTT.codeTablesValuesLanguages
                                  where a.Active && a.LanguageID == "EN" && a.ValueGUID == model.TenderTypeGUID
                                  select a.ValueDescription).FirstOrDefault();
            var temp = dataTender.TenderReference.Split('-');

            dataTender.TenderReference = tenderTypeDesc + "-" + temp[1] + "-" + temp[2] + "-" + temp[3] + "-" + temp[4];
            dataTender.TenderYear = Convert.ToInt32(temp[3]);

            DbTTT.Update(dataTender, Permissions.Tenders.UpdateGuid, ExecutionTime, DbCMS);

            dataTenderRequisition dataTenderRequisition = Mapper.Map(model, new dataTenderRequisition());
            DbTTT.Update(dataTenderRequisition, Permissions.Tenders.UpdateGuid, ExecutionTime, DbCMS);

            dataTenderingAndEvaluation dataTenderingAndEvaluation = Mapper.Map(model, new dataTenderingAndEvaluation());
            string tenderBoxNumbers = string.Join(",", model.TenderBoxNumbers);
            dataTenderingAndEvaluation.TenderBoxNumber = tenderBoxNumbers;
            DbTTT.Update(dataTenderingAndEvaluation, Permissions.Tenders.UpdateGuid, ExecutionTime, DbCMS);

            dataTenderEndorsementsAndApprovals dataTenderEndorsementsAndApprovals = Mapper.Map(model, new dataTenderEndorsementsAndApprovals());
            DbTTT.Update(dataTenderEndorsementsAndApprovals, Permissions.Tenders.UpdateGuid, ExecutionTime, DbCMS);

            dataTenderContractInformation dataTenderContractInformation = Mapper.Map(model, new dataTenderContractInformation());
            DbTTT.Update(dataTenderContractInformation, Permissions.Tenders.UpdateGuid, ExecutionTime, DbCMS);

            dataTenderObservationAndRemarks dataTenderObservationAndRemarks = Mapper.Map(model, new dataTenderObservationAndRemarks());
            DbTTT.Update(dataTenderObservationAndRemarks, Permissions.Tenders.UpdateGuid, ExecutionTime, DbCMS);

            List<dataTenderAwardedCompanies> dataTenderAwardedCompanies = (from a in DbTTT.dataTenderAwardedCompanies.AsNoTracking().Where(x => x.Active)
                                                                           where a.TenderGUID == model.TenderGUID
                                                                           select a).ToList();

            List<dataTenderRequisitionFocalPoints> dataTenderRequisitionFocalPoints = (from a in DbTTT.dataTenderRequisitionFocalPoints.AsNoTracking().Where(x => x.Active)
                                                                                       where a.TenderRequisitionGUID == model.TenderRequisitionGUID
                                                                                       select a).ToList();




            if (model.AwardedCompanyGUIDs != null)
            {
                List<dataTenderAwardedCompanies> toAddCompanies = new List<dataTenderAwardedCompanies>();
                foreach (var item in model.AwardedCompanyGUIDs)
                {
                    if (!dataTenderAwardedCompanies.Select(x => x.AwardedCompanyGUID).Contains(item))
                    {
                        toAddCompanies.Add(new dataTenderAwardedCompanies
                        {
                            TenderGUID = model.TenderGUID,
                            AwardedCompanyGUID = item,
                        });
                    }
                }
                DbTTT.CreateBulk(toAddCompanies, Permissions.Tenders.UpdateGuid, ExecutionTime, DbCMS);

                foreach (var item in dataTenderAwardedCompanies)
                {
                    if (!model.AwardedCompanyGUIDs.Contains(item.AwardedCompanyGUID))
                    {
                        DbTTT.Delete(item, ExecutionTime, Permissions.Tenders.UpdateGuid, DbCMS);
                    }
                }
            }
            else
            {
                foreach (var item in dataTenderAwardedCompanies)
                {
                    DbTTT.Delete(item, ExecutionTime, Permissions.Tenders.UpdateGuid, DbCMS);
                }
            }



            if (model.FocalPointRUGUID != null)
            {
                List<dataTenderRequisitionFocalPoints> toAddFocalPoints = new List<dataTenderRequisitionFocalPoints>();
                foreach (var item in model.FocalPointRUGUID)
                {
                    if (!dataTenderRequisitionFocalPoints.Select(x => x.FocalPointRUGUID).Contains(item))
                    {
                        toAddFocalPoints.Add(new dataTenderRequisitionFocalPoints
                        {
                            TenderRequisitionGUID = model.TenderRequisitionGUID,
                            FocalPointRUGUID = item,
                        });
                    }
                }
                DbTTT.CreateBulk(toAddFocalPoints, Permissions.Tenders.UpdateGuid, ExecutionTime, DbCMS);

                foreach (var item in dataTenderRequisitionFocalPoints)
                {
                    if (!model.FocalPointRUGUID.Contains(item.FocalPointRUGUID))
                    {
                        DbTTT.Delete(item, ExecutionTime, Permissions.Tenders.UpdateGuid, DbCMS);
                    }
                }
            }
            else
            {
                foreach (var item in dataTenderRequisitionFocalPoints)
                {
                    DbTTT.Delete(item, ExecutionTime, Permissions.Tenders.UpdateGuid, DbCMS);
                }
            }


            try
            {
                DbTTT.SaveChanges();
                DbCMS.SaveChanges();

                if (dataTender.TenderStatusGUID == Guid.Parse("1d4e460c-02f3-4fa6-9dc8-f85ab3591aa9"))
                {
                    var UpdatedByRecord = (from a in DbTTT.StaffCoreData.Where(x => x.Active && x.UserGUID == UserGUID)
                                           join b in DbTTT.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == "EN") on a.UserGUID equals b.UserGUID
                                           select new
                                           {
                                               FullName = b.FirstName + " " + b.Surname,
                                               EmailAddress = a.EmailAddress
                                           }).FirstOrDefault();

                    var BuyerRecord = (from a in DbTTT.StaffCoreData.Where(x => x.Active && x.UserGUID == dataTenderingAndEvaluation.BuyerGUID)
                                       join b in DbTTT.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == "EN") on a.UserGUID equals b.UserGUID
                                       select new
                                       {
                                           FullName = b.FirstName + " " + b.Surname,
                                           EmailAddress = a.EmailAddress
                                       }).FirstOrDefault();

                    var superVisorRecord = (from a in DbTTT.StaffCoreData.Where(x => x.Active && x.UserGUID == dataTenderingAndEvaluation.SupervisorGUID)
                                            join b in DbTTT.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == "EN") on a.UserGUID equals b.UserGUID
                                            select new
                                            {
                                                FullName = b.FirstName + " " + b.Surname,
                                                EmailAddress = a.EmailAddress
                                            }).FirstOrDefault();

                    var bocList = (from a in DbTTT.dataTenderBOCs.Where(x => x.Active && x.DutyStationGUID == dataTender.ProcurementPlanLine)
                                   join b in DbTTT.StaffCoreData.Where(x => x.Active) on a.UserGUID equals b.UserGUID
                                   select new
                                   {
                                       b.EmailAddress
                                   }).ToList();

                    string recipientEmail = UpdatedByRecord.EmailAddress + ";" + BuyerRecord.EmailAddress + ";" + superVisorRecord.EmailAddress + ";" + string.Join(";", bocList);

                    new Email().SendCloseTenderConfirmation(recipientEmail, null, dataTender.TenderReference, DateTime.Now.ToShortDateString(), UpdatedByRecord.FullName, "Tenders", "Tender Closed", dataTender.TenderGUID);

                }


                List<PrimaryKeyControl> primaryKeyControls = new List<PrimaryKeyControl>();
                primaryKeyControls.Add(DbTTT.PrimaryKeyControl(dataTender));
                primaryKeyControls.Add(new PrimaryKeyControl { ControlID = "TenderRequisitionGUID", Value = dataTenderRequisition.TenderRequisitionGUID.ToString() });
                primaryKeyControls.Add(new PrimaryKeyControl { ControlID = "TenderingAndEvaluationGUID", Value = dataTenderingAndEvaluation.TenderingAndEvaluationGUID.ToString() });
                primaryKeyControls.Add(new PrimaryKeyControl { ControlID = "TenderEndorsementsAndApprovalsGUID", Value = dataTenderEndorsementsAndApprovals.TenderEndorsementsAndApprovalsGUID.ToString() });
                primaryKeyControls.Add(new PrimaryKeyControl { ControlID = "TenderContractInformationGUID", Value = dataTenderContractInformation.TenderContractInformationGUID.ToString() });
                primaryKeyControls.Add(new PrimaryKeyControl { ControlID = "TenderObservationAndRemarksGUID", Value = dataTenderObservationAndRemarks.TenderObservationAndRemarksGUID.ToString() });


                string callBackFunc = "$('#TenderSequenceDisplay').prop('disabled', true);";
                callBackFunc += "$('#TenderSequenceDisplay').prop('readonly', true);";
                callBackFunc += "$('#TenderReferenceDisplay').prop('disabled', true);";
                callBackFunc += "$('#TenderReferenceDisplay').prop('readonly', true);";


                return Json(DbTTT.SingleUpdateMessage(true, null, primaryKeyControls, DbTTT.RowVersionControls(Portal.SingleToList(model)), callBackFunc));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        private bool ValidateTender(TenderUpdateModel model)
        {
            bool success = true;
            if (model.TenderDateForBidderConference < model.TenderLaunchingDate)
            {
                ModelState.AddModelError("TenderDateForBidderConference", "Tender Date For Bidder Conference must be greater than Tender Launching Date");
                success = false;
            }
            if (model.TenderDeadlineForClarification < model.TenderLaunchingDate)
            {
                ModelState.AddModelError("TenderDeadlineForClarification", "Tender Deadline For Clarification must be greater than Tender Launching Date");
                success = false;
            }
            if (model.TenderSiteVisitDate < model.TenderLaunchingDate)
            {
                ModelState.AddModelError("TenderSiteVisitDate", "Tender Site Visit Date must be after the Tender Launching Date");
                success = false;
            }
            if (model.TenderClosingDate < model.TenderLaunchingDate)
            {
                ModelState.AddModelError("TenderClosingDate", "Tender Closing Date Technical must after the than Tender Launching Date");
                success = false;
            }
            if (model.TenderBidOpeningDateTechnical < model.TenderLaunchingDate)
            {
                ModelState.AddModelError("TenderBidOpeningDateTechnical", "Tender Bid Opening Date Technical must be after the Tender Launching Date");
                success = false;
            }
            if (model.TenderTechnicalEvaluationDate < model.TenderLaunchingDate)
            {
                ModelState.AddModelError("TenderTechnicalEvaluationDate", "Tender Technical Evaluation Date must be after the Tender Launching Date");
                success = false;
            }
            if (model.TenderBidOpeningDateFinancial < model.TenderLaunchingDate)
            {
                ModelState.AddModelError("TenderBidOpeningDateFinancial", "Tender Bid Opening Date Financial must be after the Tender Launching Date");
                success = false;
            }
            if (model.TenderFinancialEvaluationDate < model.TenderLaunchingDate)
            {
                ModelState.AddModelError("TenderFinancialEvaluationDate", "Tender Financial Evaluation Date must be after the Tender Launching Date");
                success = false;
            }
            if (model.TenderDateForBidderConference > model.TenderClosingDate)
            {
                ModelState.AddModelError("TenderDateForBidderConference", "Tender Date For Bidder Conference must be before the Tender Closing Date");
                success = false;
            }
            if (model.TenderDeadlineForClarification > model.TenderClosingDate)
            {
                ModelState.AddModelError("TenderDeadlineForClarification", "Tender Deadline For Clarification must be before the Tender Closing Date");
                success = false;
            }
            if (model.TenderSiteVisitDate > model.TenderClosingDate)
            {
                ModelState.AddModelError("TenderSiteVisitDate", "Tender Site Visit Date must be before the Tender Closing Date");
                success = false;
            }
            if (model.TenderBidOpeningDateTechnical < model.TenderClosingDate)
            {
                ModelState.AddModelError("TenderBidOpeningDateTechnical", "Tender Bid Opening Date Technical must be after or the same day of the Tender Closing Date");
                success = false;
            }
            if (model.TenderTechnicalEvaluationDate <= model.TenderClosingDate)
            {
                ModelState.AddModelError("TenderTechnicalEvaluationDate", "Tender Technical Evaluation Date must be after the Tender Closing Date");
                success = false;
            }
            if (model.TenderBidOpeningDateFinancial <= model.TenderClosingDate)
            {
                ModelState.AddModelError("TenderBidOpeningDateFinancial", "Tender Bid Opening Date Financial Date must be after the Tender Closing Date");
                success = false;
            }
            if (model.TenderFinancialEvaluationDate <= model.TenderClosingDate)
            {
                ModelState.AddModelError("TenderFinancialEvaluationDate", "Tender Financial Evaluation Date must be after the Tender Closing Date");
                success = false;
            }
            if (model.RequestRecieved > model.TenderLaunchingDate)
            {
                ModelState.AddModelError("RequestRecieved", "Tender Request Date must be before the Tender Launching Date");
                success = false;
            }
            //try
            //{
            //    if (DbTTT.dataTender
            //        .Where(x =>
            //        x.TenderReference == model.TenderReference
            //        && x.TenderGUID != model.TenderGUID
            //        && x.Active).Count() > 0)
            //    {
            //        ModelState.AddModelError("", "Tender with the same reference already exists, please refresh the page and try again.");
            //        success = false;
            //    }
            //}
            //catch { }


            return success;
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TenderDelete(dataTender model)
        {
            if (!CMS.HasAction(Permissions.Tenders.Delete, Apps.TTT))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            List<dataTender> DeletedTenders = DeleteTenders(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.Tenders.Restore, Apps.TTT), Container = "TenderFormControls" });

            try
            {
                int CommitedRows = DbTTT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbTTT.SingleDeleteMessage(CommitedRows, DeletedTenders.FirstOrDefault(), null, UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TenderRestore(dataTender model)
        {
            if (!CMS.HasAction(Permissions.Tenders.Restore, Apps.TTT))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            //if (ActiveApplication(model))
            //{
            //    return Json(DbCMS.RecordExists());
            //}

            List<dataTender> RestoredTenders = RestoreTenders(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.Tenders.Create, Apps.TTT, new UrlHelper(Request.RequestContext).Action("Tenders/Create", "Tenders", new { Area = "TTT" })), Container = "TenderFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.Tenders.Update, Apps.TTT), Container = "TenderFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.Tenders.Restore, Apps.TTT), Container = "TenderFormControls" });

            try
            {
                int CommitedRows = DbTTT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbTTT.SingleRestoreMessage(CommitedRows, RestoredTenders, DbTTT.PrimaryKeyControl(RestoredTenders.FirstOrDefault()), "", null, UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }


        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult TendersDataTableDelete(List<dataTender> models)
        {
            if (!CMS.HasAction(Permissions.Tenders.Delete, Apps.TTT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataTender> DeletedTenders = DeleteTenders(models);

            try
            {
                DbTTT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbTTT.PartialDeleteMessage(DeletedTenders, models, DataTableNames.TendersDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult TendersDataTableRestore(List<dataTender> models)
        {
            if (!CMS.HasAction(Permissions.Tenders.Restore, Apps.TTT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataTender> RestoredTenders = RestoreTenders(models);

            try
            {
                DbTTT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbTTT.PartialRestoreMessage(RestoredTenders, models, DataTableNames.TendersDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        private List<dataTender> DeleteTenders(List<dataTender> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<dataTender> DeletedTenders = new List<dataTender>();

            //THIS IS FOR PERMISSION TEST//
            //DO NOT DELETE THIS CODE
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = (from a in DbCMS.codeApplications
            //                    from f in DbCMS.codeApplicationsFactorForTest
            //                    where a.ApplicationGUID == f.ApplicationGUID
            //                    select new { a.ApplicationGUID, a.codeApplicationsRowVersion, f.OperationGUID, f.OrganizationGUID, f.CountryGUID }).AsQueryable().ToString();

            string query = DbTTT.QueryBuilder(models, Permissions.Tenders.DeleteGuid, SubmitTypes.Delete, "");

            var Records = DbTTT.Database.SqlQuery<dataTender>(query).ToList();

            foreach (var record in Records)
            {
                DeletedTenders.Add(DbTTT.Delete(record, ExecutionTime, Permissions.Tenders.DeleteGuid, DbCMS));
            }

            var dataTenderRequisition = DeletedTenders.SelectMany(a => a.dataTenderRequisition).Where(l => l.Active).ToList();
            foreach (var item in dataTenderRequisition)
            {
                DbTTT.Delete(item, ExecutionTime, Permissions.Tenders.DeleteGuid, DbCMS);
            }

            var dataTenderRequisitionFocalPoints = dataTenderRequisition.SelectMany(a => a.dataTenderRequisitionFocalPoints).Where(l => l.Active).ToList();
            foreach (var item in dataTenderRequisitionFocalPoints)
            {
                DbTTT.Delete(item, ExecutionTime, Permissions.Tenders.DeleteGuid, DbCMS);
            }

            var dataTenderingAndEvaluation = DeletedTenders.SelectMany(a => a.dataTenderingAndEvaluation).Where(l => l.Active).ToList();
            foreach (var item in dataTenderingAndEvaluation)
            {
                DbTTT.Delete(item, ExecutionTime, Permissions.Tenders.DeleteGuid, DbCMS);
            }

            var dataTenderEndorsementsAndApprovals = DeletedTenders.SelectMany(a => a.dataTenderEndorsementsAndApprovals).Where(l => l.Active).ToList();
            foreach (var item in dataTenderEndorsementsAndApprovals)
            {
                DbTTT.Delete(item, ExecutionTime, Permissions.Tenders.DeleteGuid, DbCMS);
            }

            var dataTenderContractInformation = DeletedTenders.SelectMany(a => a.dataTenderContractInformation).Where(l => l.Active).ToList();
            foreach (var item in dataTenderContractInformation)
            {
                DbTTT.Delete(item, ExecutionTime, Permissions.Tenders.DeleteGuid, DbCMS);
            }

            var dataTenderObservationAndRemarks = DeletedTenders.SelectMany(a => a.dataTenderObservationAndRemarks).Where(l => l.Active).ToList();
            foreach (var item in dataTenderObservationAndRemarks)
            {
                DbTTT.Delete(item, ExecutionTime, Permissions.Tenders.DeleteGuid, DbCMS);
            }

            var dataTenderAwardedCompanies = DeletedTenders.SelectMany(a => a.dataTenderAwardedCompanies).Where(l => l.Active).ToList();
            foreach (var item in dataTenderAwardedCompanies)
            {
                DbTTT.Delete(item, ExecutionTime, Permissions.Tenders.DeleteGuid, DbCMS);
            }

            return DeletedTenders;
        }
        private List<dataTender> RestoreTenders(List<dataTender> models)
        {
            Guid DeleteActionGUID = Permissions.Tenders.DeleteGuid;
            Guid RestoreActionGUID = Permissions.Tenders.RestoreGuid;
            DateTime RestoringTime = DateTime.Now;

            List<dataTender> RestoredTenders = new List<dataTender>();

            //THIS IS FOR PERMISSION TEST//
            //DO NOT DELETE THIS CODE
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = (from a in DbCMS.codeApplications
            //                    from f in DbCMS.codeApplicationsFactorForTest
            //                    where a.ApplicationGUID == f.ApplicationGUID
            //                    select new
            //                    {
            //                        a.ApplicationGUID,
            //                        a.codeApplicationsRowVersion,
            //                        C2 = f.OperationGUID + "," + f.OrganizationGUID,
            //                    }).AsQueryable().ToString();//.Replace("[C2]", "[FactorsToken]");


            string query = DbTTT.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, "");

            var Records = DbTTT.Database.SqlQuery<dataTender>(query).ToList();
            foreach (var record in Records)
            {
                //if (!ActiveApplication(record))
                //{
                //    RestoredApplications.Add(DbCMS.Restore(record, DeleteActionGUID, RestoreActionGUID, RestoringTime));
                //}
                RestoredTenders.Add(DbTTT.Restore(record, DeleteActionGUID, RestoreActionGUID, RestoringTime, DbCMS));
            }

            var dataTenderRequisitions = RestoredTenders.SelectMany(x => x.dataTenderRequisition.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var item in dataTenderRequisitions)
            {
                DbTTT.Restore(item, DeleteActionGUID, RestoreActionGUID, RestoringTime, DbCMS);
            }

            var dataTenderRequisitionFocalPoints = dataTenderRequisitions.SelectMany(x => x.dataTenderRequisitionFocalPoints.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var item in dataTenderRequisitionFocalPoints)
            {
                DbTTT.Restore(item, DeleteActionGUID, RestoreActionGUID, RestoringTime, DbCMS);
            }


            var dataTenderingAndEvaluation = RestoredTenders.SelectMany(x => x.dataTenderingAndEvaluation.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var item in dataTenderingAndEvaluation)
            {
                DbTTT.Restore(item, DeleteActionGUID, RestoreActionGUID, RestoringTime, DbCMS);
            }

            var dataTenderEndorsementsAndApprovals = RestoredTenders.SelectMany(x => x.dataTenderEndorsementsAndApprovals.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var item in dataTenderEndorsementsAndApprovals)
            {
                DbTTT.Restore(item, DeleteActionGUID, RestoreActionGUID, RestoringTime, DbCMS);
            }

            var dataTenderContractInformation = RestoredTenders.SelectMany(x => x.dataTenderContractInformation.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var item in dataTenderContractInformation)
            {
                DbTTT.Restore(item, DeleteActionGUID, RestoreActionGUID, RestoringTime, DbCMS);
            }

            var dataTenderObservationAndRemarks = RestoredTenders.SelectMany(x => x.dataTenderObservationAndRemarks.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var item in dataTenderObservationAndRemarks)
            {
                DbTTT.Restore(item, DeleteActionGUID, RestoreActionGUID, RestoringTime, DbCMS);
            }

            var dataTenderAwardedCompanies = RestoredTenders.SelectMany(x => x.dataTenderAwardedCompanies.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var item in dataTenderAwardedCompanies)
            {
                DbTTT.Restore(item, DeleteActionGUID, RestoreActionGUID, RestoringTime, DbCMS);
            }

            return RestoredTenders;
        }

        #region Processing Methods
        [Route("TTT/Tenders/GetReferenceNumber/")]
        public JsonResult GetReferenceNumber()
        {
            var result = (from a in DbTTT.dataTender.AsNoTracking()
                          orderby a.TenderYear descending, a.TenderSequence descending
                          where a.Active
                          select new
                          {
                              a.TenderReference,
                              a.TenderSequence
                          }).FirstOrDefault();

            int current = 1;
            try
            {
                int last = Convert.ToInt32(result.TenderReference.Split('-').Last());
                current = last + 1;
            }
            catch
            {

            }
            if ((from a in DbTTT.dataTender.AsNoTracking() where a.TenderYear == 2021 select a).FirstOrDefault() == null)
            {
                current = 1;
            }
            //RFQ-HCR-SYR-19-27
            string refNumber = "HCR-SYR-" + DateTime.Now.Year.ToString() + "-" + current.ToString();
            return Json(new { refNumber = refNumber, refSequence = current }, JsonRequestBehavior.AllowGet);
        }



        public class TenderRefAndSeq
        {
            public string TenderReference { get; set; }
            public int TenderSequence { get; set; }
        }
        public TenderRefAndSeq GetReferenceNumber(Guid TenderTypeGUID)
        {
            var tenderTypeDesc = (from a in DbTTT.codeTablesValuesLanguages
                                  where a.Active && a.LanguageID == "EN" && a.ValueGUID == TenderTypeGUID
                                  select a.ValueDescription).FirstOrDefault();

            var result = (from a in DbTTT.dataTender.AsNoTracking()
                          orderby a.TenderYear descending, a.TenderSequence descending
                          where a.Active
                          select new
                          {
                              a.TenderReference,
                              a.TenderSequence
                          }).FirstOrDefault();

            int current = 1;
            try
            {
                int last = Convert.ToInt32(result.TenderReference.Split('-').Last());
                current = last + 1;
            }
            catch
            {

            }
            if ((from a in DbTTT.dataTender.AsNoTracking() where a.TenderYear == 2021 select a).FirstOrDefault() == null)
            {
                current = 1;
            }
            return new TenderRefAndSeq
            {
                TenderReference = tenderTypeDesc + "-" + "HCR-SYR-" + DateTime.Now.Year.ToString() + "-" + current.ToString(),
                TenderSequence = current

            };
        }

        private Guid _ITB = Guid.Parse("9032B0F4-2950-4ECE-BC90-85944A6B7B34");
        private Guid _RFQ = Guid.Parse("FC66EE7D-B065-4A97-B481-F5813901645E");
        private Guid _RFP = Guid.Parse("1AE1D667-99EB-40AA-9A50-C4C78F00B1FD");


        [Route("TTT/Tenders/TestDocument/")]
        public ActionResult TestDocument()
        {
            string templatePath = "~/Areas/TTT/Templates/ITB_Form.docx";
            var serverPath = Server.MapPath(templatePath);

            string destinationPath = "~/Areas/TTT/GeneratedDocuments/ITB-HCR-SYR-2019-1.docx";
            var destinationServerPath = Server.MapPath(destinationPath);

            //DateFromMyCode DD/MM/YYYY
            //YYYYNUMBERFromMyCode   YYYY/NUMBER
            //ClosingDateFromMyCode DD/MM/YYYY
            //BuyerEmailFromMyCode
            //BuyerFromMyCode Name
            //BuyerTitleFromMyCode Title
            //DeadLineFromMyCode DD/MM/YYYY
            //TenderRefYearFromMyCode YYY
            //TenderRefNumberFromMyCode XXX
            //TenderBoxNumberFromMyCode number

            System.IO.File.Copy(serverPath, destinationServerPath);

            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(destinationServerPath, true))
            {
                string docText = null;
                using (StreamReader sr = new StreamReader(wordDoc.MainDocumentPart.GetStream()))
                {
                    docText = sr.ReadToEnd();
                }

                docText = new Regex("DateFromMyCode", RegexOptions.IgnoreCase).Replace(docText, "111111");
                docText = new Regex("YYYYNUMBERFromMyCode", RegexOptions.IgnoreCase).Replace(docText, "222222");
                docText = new Regex("ClosingDateFromMyCode", RegexOptions.IgnoreCase).Replace(docText, "33333");
                docText = new Regex("BuyerEmailFromMyCode", RegexOptions.IgnoreCase).Replace(docText, "44444");
                docText = new Regex("BuyerFromMyCode", RegexOptions.IgnoreCase).Replace(docText, "55555");
                docText = new Regex("BuyerTitleFromMyCode", RegexOptions.IgnoreCase).Replace(docText, "6666");
                docText = new Regex("DeadLineFromMyCode", RegexOptions.IgnoreCase).Replace(docText, "7777");
                docText = new Regex("TenderRefYearFromMyCode", RegexOptions.IgnoreCase).Replace(docText, "8888");
                docText = new Regex("TenderRefNumberFromMyCode", RegexOptions.IgnoreCase).Replace(docText, "9999");
                docText = new Regex("TenderBoxNumberFromMyCode", RegexOptions.IgnoreCase).Replace(docText, "0000");


                using (StreamWriter sw = new StreamWriter(wordDoc.MainDocumentPart.GetStream(FileMode.Create)))
                {
                    sw.Write(docText);
                }
            }

            return null;
        }

        [Route("TTT/Tenders/GenerateCoverLetter/{PK}")]
        public string GenerateCoverLetter(Guid PK)
        {
            //[Launching Date] ----
            //[REFERENCE] ----
            //[GOODS or SERVICES/SUBJECT] ----
            //[Closing Date] 15:30 hrs Local Syrian Time ----
            //[goods or services/subject] ----
            //[goods or services/subject] ----
            //[Closing Date] at 15:30 hrs Syrian Local Time ----
            //[Email of Supply Staff Assigned]  ----
            //[Email of Supervisor of Supply Staff Assigned] ----
            //[REFERENCE] ----
            //Samples should be sent to the following address on or before [date] – 15:30 hrs Syrian Local Time.  Technical
            //[Email of Supply Staff Assigned]  ----
            //[Email of Supervisor of Supply Staff Assigned]. ----
            //The deadline for receipt of questions is [date] at 16:00 hrs Syrian Local Time. (clarification) ----

            string destinationPath = "";

            var model = (from a in DbTTT.dataTender.AsNoTracking()
                         join b in DbTTT.dataTenderRequisition.AsNoTracking() on a.TenderGUID equals b.TenderGUID
                         join c in DbTTT.dataTenderingAndEvaluation.AsNoTracking() on a.TenderGUID equals c.TenderGUID
                         join d in DbTTT.dataTenderEndorsementsAndApprovals.AsNoTracking() on a.TenderGUID equals d.TenderGUID
                         join e in DbTTT.dataTenderContractInformation.AsNoTracking() on a.TenderGUID equals e.TenderGUID
                         join f in DbTTT.dataTenderObservationAndRemarks.AsNoTracking() on a.TenderGUID equals f.TenderGUID
                         join g in DbTTT.StaffCoreData.AsNoTracking() on c.BuyerGUID equals g.UserGUID
                         join h in DbTTT.StaffCoreData.AsNoTracking() on c.SupervisorGUID equals h.UserGUID
                         where a.TenderGUID == PK
                         select new
                         {
                             a,
                             b,
                             c,
                             d,
                             e,
                             f,
                             g,
                             h,
                         }).FirstOrDefault();

            if (model.a.TenderTypeGUID == _RFQ)
            {
                string templatePath = "~/Areas/TTT/Templates/RFQ_Template.docx";
                var serverPath = Server.MapPath(templatePath);

                destinationPath = "~/Areas/TTT/GeneratedDocuments/" + model.a.TenderReference + ".docx";
                var destinationServerPath = Server.MapPath(destinationPath);

                System.IO.File.Copy(serverPath, destinationServerPath, true);

                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(destinationServerPath, true))
                {
                    string docText = null;
                    using (StreamReader sr = new StreamReader(wordDoc.MainDocumentPart.GetStream()))
                    {
                        docText = sr.ReadToEnd();
                    }
                    string launchingDate = model.c.TenderLaunchingDate.ToString("dd/MM/yyyy");
                    string reference = model.a.TenderReference.Split('-').Last().ToString();
                    string subject = model.a.TenderSubject;
                    string closingDate = model.c.TenderClosingDate.ToString("dd/MM/yyyy HH:mm");
                    Guid SupplyStaffAssigned = model.c.BuyerGUID;
                    Guid SupervisorOfSupplyStaff = model.c.SupervisorGUID;
                    string supplyStaffEmail = model.g.EmailAddress;
                    string supervisorOfSupplyStaffEmail = model.h.EmailAddress;
                    string sampleSubmissionDeadline = model.c.TenderSampleSubmissionDeadline.Value.ToString("dd/MM/yyyy HH:mm");
                    string deadlineForClarification = model.a.TenderDeadlineForClarification.ToString("dd/MM/yyyy HH:mm");
                    docText = new Regex("LaunchingDateFromCode", RegexOptions.IgnoreCase).Replace(docText, launchingDate);
                    docText = new Regex("LaunchDateFromCode", RegexOptions.IgnoreCase).Replace(docText, launchingDate);
                    docText = new Regex("REFERENCEFromCode", RegexOptions.IgnoreCase).Replace(docText, reference);
                    docText = new Regex("Ref2FromCode", RegexOptions.IgnoreCase).Replace(docText, reference);
                    docText = new Regex("SubjectFromCode", RegexOptions.IgnoreCase).Replace(docText, subject);
                    docText = new Regex("ClosingDateFromCode", RegexOptions.IgnoreCase).Replace(docText, closingDate);
                    docText = new Regex("ClosingDateFromMyCode", RegexOptions.IgnoreCase).Replace(docText, closingDate);
                    docText = new Regex("SuppMailFromCode", RegexOptions.IgnoreCase).Replace(docText, supplyStaffEmail);
                    docText = new Regex("SupVisorMailFromCode", RegexOptions.IgnoreCase).Replace(docText, supervisorOfSupplyStaffEmail);
                    docText = new Regex("SampleSubDateFromCode", RegexOptions.IgnoreCase).Replace(docText, sampleSubmissionDeadline);
                    docText = new Regex("ClarificationDateFromCode", RegexOptions.IgnoreCase).Replace(docText, deadlineForClarification);
                    using (StreamWriter sw = new StreamWriter(wordDoc.MainDocumentPart.GetStream(FileMode.Create)))
                    {
                        sw.Write(docText);
                    }
                }
            }

            return destinationPath;

        }


        public FileResult CoverLetterDownload(Guid PK)
        {
            GenerateCoverLetter(PK);
            var tenderReference = (from a in DbTTT.dataTender.Where(x => x.TenderGUID == PK)
                                   select a.TenderReference).FirstOrDefault();

            string FolderPath = Server.MapPath("~/Areas/TTT/GeneratedDocuments");
            string FileName = tenderReference + ".docx";
            string DownloadURL = FolderPath + "\\" + FileName;
            byte[] fileBytes = System.IO.File.ReadAllBytes(@"" + DownloadURL + "");
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, FileName);
        }

        //public FileResult CoverLetterDownload(string destinationPath)
        //{
        //    string FolderPath = Server.MapPath(destinationPath);
        //    string FileName = destinationPath.Split('/').Last();
        //   // string DownloadURL = FolderPath + "\\" + FileName;
        //    byte[] fileBytes = System.IO.File.ReadAllBytes(@"" + FolderPath + "");
        //    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, FileName);
        //}
        #endregion


        #region Tender Box Validation
        public JsonResult CheckTenderBoxAvailability(Guid TenderGUID, DateTime TenderClosingDate, Guid ProcurementPlanLine)
        {
            DateTime minDate = TenderClosingDate.AddDays(-7);
            DateTime maxDate = TenderClosingDate.AddDays(7);

            var tenderBoxNumbers = DropDownList.TenderBoxNumbers();
            if (ProcurementPlanLine == Guid.Parse("6D7397D6-3D7F-48FC-BFD2-18E69673AC92"))//damascus
            {
                var reservedBoxNumbers = (from a in DbTTT.dataTenderingAndEvaluation.AsNoTracking()
                                          join b in DbTTT.dataTender.AsNoTracking() on a.TenderGUID equals b.TenderGUID
                                          where a.TenderClosingDate >= minDate
                                          && a.TenderClosingDate <= maxDate
                                          && a.TenderGUID != TenderGUID
                                          && b.ProcurementPlanLine == ProcurementPlanLine
                                          && b.Active == true
                                          select new
                                          {
                                              b.TenderSubject,
                                              a.TenderBoxNumber
                                          }).ToList();
                return Json(new { TenderBoxNumbers = tenderBoxNumbers, ReservedBoxNumbers = reservedBoxNumbers });
            }
            else
            {
                return Json(new { TenderBoxNumbers = tenderBoxNumbers, ReservedBoxNumbers = "" });
            }


        }
        #endregion
    }
}