using AHD_DAL.Model;
using AHD_DAL.ViewModels;
using AppsPortal.Areas.AHD.RDLC;
using AppsPortal.Areas.AHD.RDLC.AHDDataSetTableAdapters;
using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.ViewModels;
using AutoMapper;
using iTextSharp.text;
using iTextSharp.text.pdf;
using LinqKit;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace AppsPortal.Areas.AHD.Controllers
{
    public class StaffRenewalResidencyController : AHDBaseController
    {
        // GET: AHD/StaffRenewalResidency
        #region Forms

        [Route("AHD/StaffRenewalResidency/")]
        public ActionResult StaffRenewalResidencyIndex()
        {
            if (!CMS.HasAction(Permissions.StaffRenewalResidency.Access, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            return View("~/Areas/AHD/Views/StaffRenewalResidency/Index.cshtml");
        }
        [Route("AHD/StaffRenewalResidencyDataTable/")]
        public JsonResult StaffRenewalResidencyDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<StaffRenwalResidencyDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<StaffRenwalResidencyDataTableModel>(DataTable.Filters);
            }
            Guid _nvGuID = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c4e7999");
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.StaffRenewalResidency.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix
            var All = (

                from a in DbAHD.dataStaffRenwalResidency.AsExpandable()
                join b in DbAHD.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN) on a.StaffGUID equals b.UserGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()
                join c in DbAHD.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.codeTablesValues.TableGUID == LookupTables.AHDRenewalStaffRenewalResidencyFormStatus) on a.LastFlowStatusGUID equals c.ValueGUID into LJ2
                from R2 in LJ2.DefaultIfEmpty()
                join d in DbAHD.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN) on a.CreateByGUID equals d.UserGUID into LJ3
                from R3 in LJ3.DefaultIfEmpty()
                join e in DbAHD.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.codeTablesValues.TableGUID == _nvGuID) on a.FormTypeGUID equals e.ValueGUID into LJ4
                from R4 in LJ4.DefaultIfEmpty()
                join f in DbAHD.StaffCoreData.Where(x => x.Active) on a.StaffGUID equals f.UserGUID into LJ5
                from R5 in LJ5.DefaultIfEmpty()
                select new StaffRenwalResidencyDataTableModel
                {
                    StaffRenwalResidencyGUID = a.StaffRenwalResidencyGUID,
                    StaffName = R1.FirstName + " " + R1.Surname,
                    StaffGUID = a.StaffGUID.ToString(),
                    LastFlowStatusGUID = a.LastFlowStatusGUID.ToString(),
                    SentDate = a.SentDate,
                    ReturnDate = a.ReturnDate,
                    //EntryDateToSyria=a.EntryDateToSyria,
                    PassportValidityDate = a.PassportValidityDate,
                    CreateDate = a.CreateDate,
                    SALContractEndDate = R5.ContractEndDate,
                    NextExtensionRequest = a.NextExtensionRequest,


                    Active = a.Active,
                    RefNumber = a.RefNumber,
                    RenewalFormFlowStatus = R2.ValueDescription,
                    DateOfSubmission = a.CreateDate,
                    CreateBy = R3.FirstName + " " + R3.Surname,
                    ContractStartDate = a.ContractStartDate,
                    //ContractEndDate = a.ContractEndDate,
                    ResidencyExpiryDate = a.CurrentResidencyEndDateSent,
                    CurrentResidencyEndDateStampd = a.CurrentResidencyEndDateStampd,
                    FormTypeGUID = a.FormTypeGUID.ToString(),
                    FormType = R4.ValueDescription,


                    dataStaffRenwalResidencyRowVersion = a.dataStaffRenwalResidencyRowVersion
                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<StaffRenwalResidencyDataTableModel> Result = Mapper.Map<List<StaffRenwalResidencyDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("AHD/StaffRenewalResidency/Create/")]
        public ActionResult StaffRenewalResidencyCreate()
        {
            if (!CMS.HasAction(Permissions.StaffRenewalResidency.Create, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            return PartialView("~/Areas/AHD/Views/StaffRenewalResidency/_RenwalResidencyStaffForm.cshtml", new StaffRenwalResidencyModel { StaffRenwalResidencyGUID = Guid.Empty });

        }

           [Route("AHD/StaffRenewalResidency/Update/{PK}")]
        public ActionResult StaffRenewalResidencyUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.StaffRenewalResidency.Access, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            
            var model = (from a in DbAHD.dataStaffRenwalResidency.WherePK(PK)
                        

                         select new StaffRenwalResidencyModel
                         {
                             StaffRenwalResidencyGUID = a.StaffRenwalResidencyGUID,
                             RefNumber = a.RefNumber,
                             PassportNumber = a.PassportNumber,
                             StaffGUID = (Guid)a.StaffGUID,
                             PassportValidityDate = a.PassportValidityDate,
                             ContractStartDate = a.ContractStartDate,
                             ContractEndDate = a.ContractEndDate,
                             CurrentResidencyEndDateSent = a.CurrentResidencyEndDateSent,
                             NextExtensionRequest=a.NextExtensionRequest,
                             CurrentResidencyEndDateStampd = a.CurrentResidencyEndDateStampd,
                             CreateDate = a.CreateDate,
                             LastFlowStatus = a.LastFlowStatus,
                             LastFlowStatusGUID = a.LastFlowStatusGUID,
                             SentDate=a.SentDate,
                             FormTypeGUID=a.FormTypeGUID,
                             DepartureDateFromSyria=a.DepartureDateFromSyria,
                             
                             Active=a.Active,



                             dataStaffRenwalResidencyRowVersion = a.dataStaffRenwalResidencyRowVersion
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("StaffRenewalResidency", "StaffRenewalResidencys", new { Area = "AHD" }));
            return PartialView("~/Areas/AHD/Views/StaffRenewalResidency/_RenwalResidencyStaffForm.cshtml", model);

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffRenewalResidencyCreate(StaffRenwalResidencyModel model)
        {
            if (!CMS.HasAction(Permissions.StaffRenewalResidency.Create, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            if (!ModelState.IsValid || ActiveStaffRenewalResidency(model) || model.FormTypeGUID==Guid.Empty || model.FormTypeGUID==null) return PartialView("~/Areas/AHD/Views/StaffRenewalResidencys/_StaffRenewalResidencyForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();
            var _staffcore = DbAHD.StaffCoreData.Where(x => x.UserGUID == model.StaffGUID).FirstOrDefault();
       
            var _personaldetaillang = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == model.StaffGUID).ToList();
            var _jobtitle = DbAHD.codeJobTitlesLanguages.Where(x => x.Active).ToList();
            var _countris = DbAHD.codeCountriesLanguages.Where(x => x.Active).ToList();

            var _dutystations = DbAHD.codeDutyStationsLanguages.Where(x => x.Active).ToList();
            var _departments = DbAHD.codeDepartmentsLanguages.Where(x => x.Active).ToList();
            dataStaffRenwalResidency StaffRenewalResidency = Mapper.Map(model, new dataStaffRenwalResidency());
            StaffRenewalResidency.StaffRenwalResidencyGUID = EntityPK;
            StaffRenewalResidency.RefNumber = model.RefNumber;
            StaffRenewalResidency.StaffGUID = model.StaffGUID;
            StaffRenewalResidency.FormTypeGUID = model.FormTypeGUID;
            StaffRenewalResidency.CreateByGUID = UserGUID;
            //StaffRenewalResidency.EntryDateToSyria = model.EntryDateToSyria;
            if (model.FormTypeGUID == Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c4e7933"))
            {
                if (model.DepartureDateFromSyria == null)
                {
                    return Json(DbAHD.PermissionError());
                }
                StaffRenewalResidency.DepartureDateFromSyria = model.DepartureDateFromSyria;
            }


            StaffRenewalResidency.EnStaffName = _personaldetaillang.Where(x => x.LanguageID == "EN").FirstOrDefault().FirstName + " " + _personaldetaillang.Where(x => x.LanguageID == "EN").FirstOrDefault().Surname;
            StaffRenewalResidency.ArStaffName = _personaldetaillang.Where(x => x.LanguageID == "AR").FirstOrDefault().FirstName + " " + _personaldetaillang.Where(x => x.LanguageID == "AR").FirstOrDefault().Surname;
            StaffRenewalResidency.EnDeptName = _departments.Where(x => x.LanguageID == "EN" && x.DepartmentGUID == _staffcore.DepartmentGUID).FirstOrDefault().DepartmentDescription;
            StaffRenewalResidency.ArDeptName = _departments.Where(x => x.LanguageID == "AR" && x.DepartmentGUID == _staffcore.DepartmentGUID).FirstOrDefault().DepartmentDescription;
            //StaffRenewalResidency.Gender = DbAHD.codeTablesValuesLanguages.Where(x => x.ValueGUID == _staffcore.Gender).FirstOrDefault().ValueDescription;
            string secondnatioEn= _countris.Where(x => x.CountryGUID == _staffcore.Nationality2GUID && x.LanguageID == "EN").FirstOrDefault()!=null? _countris.Where(x => x.CountryGUID == _staffcore.Nationality2GUID && x.LanguageID == "EN").FirstOrDefault().CountryDescription:"";
            string secondnatioAr = _countris.Where(x => x.CountryGUID == _staffcore.Nationality2GUID && x.LanguageID == "AR").FirstOrDefault() != null ? _countris.Where(x => x.CountryGUID == _staffcore.Nationality2GUID && x.LanguageID == "AR").FirstOrDefault().Nationality : "";
            StaffRenewalResidency.EnNationality = _countris.Where(x => x.CountryGUID == _staffcore.NationalityGUID && x.LanguageID == "EN").FirstOrDefault().CountryDescription;
            if (!string.IsNullOrEmpty(secondnatioEn))
            {

                StaffRenewalResidency.EnNationality = StaffRenewalResidency.EnNationality + " and " + secondnatioEn;
            }
           
            StaffRenewalResidency.ArNationality = _countris.Where(x => x.CountryGUID == _staffcore.NationalityGUID && x.LanguageID=="AR").FirstOrDefault().Nationality;
            if (!string.IsNullOrEmpty(secondnatioAr))
            {

                StaffRenewalResidency.ArNationality = StaffRenewalResidency.ArNationality + " و " + secondnatioAr;
            }
            //StaffRenewalResidency.ReturnDate = model.ReturnDate;
            StaffRenewalResidency.CurrentResidencyEndDateSent = _staffcore.ExpiryOfResidencyVisa;
            if (_staffcore.ExpiryOfResidencyVisa != null)
            {
                StaffRenewalResidency.NextExtensionRequest = _staffcore.ExpiryOfResidencyVisa.Value.AddDays(-42);
            }
            StaffRenewalResidency.SentDate = model.SentDate;
            Guid unlpGUID = Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E3732");
            var _unlp = DbAHD.dataStaffCoreDocument.Where(x=>x.UserGUID== _staffcore.UserGUID && x.DocumentTypeGUID== unlpGUID
            && x.Active).FirstOrDefault();
            StaffRenewalResidency.PassportNumber = _unlp.DocumentNumber;
            StaffRenewalResidency.PassportValidityDate = _unlp.DocumentDateOfExpiry;
            StaffRenewalResidency.EnJobTitle = _jobtitle.Where(x=>x.LanguageID=="EN" && x.JobTitleGUID== _staffcore.JobTitleGUID).FirstOrDefault().JobTitleDescription;
            StaffRenewalResidency.ArJobTitle = _staffcore.Gender == Guid.Parse("688B11E0-24FB-44B8-94CE-D8568C4742C7") ?  _jobtitle.Where(x => x.LanguageID == "AR" && x.JobTitleGUID == _staffcore.JobTitleGUID).FirstOrDefault().JobTitleDescription : _jobtitle.Where(x => x.LanguageID == "AR" && x.JobTitleGUID == _staffcore.JobTitleGUID).FirstOrDefault().JobTitleDescriptionFemale;

            StaffRenewalResidency.EnOffice = _dutystations.Where(x => x.LanguageID == "EN" && x.DutyStationGUID == _staffcore.DutyStationGUID).FirstOrDefault().DutyStationDescription;
            StaffRenewalResidency.ArOffice = _dutystations.Where(x => x.LanguageID == "AR" && x.DutyStationGUID == _staffcore.DutyStationGUID).FirstOrDefault().DutyStationDescription;

            if (_staffcore.Gender == Guid.Parse("688B11E0-24FB-44B8-94CE-D8568C4742C7"))
            {
                StaffRenewalResidency.Gender = "Male";
            }
            else

                StaffRenewalResidency.Gender = "Female";
            StaffRenewalResidency.CreateDate = ExecutionTime;
            StaffRenewalResidency.CreateByGUID = UserGUID;
            StaffRenewalResidency.LastFlowStatusGUID = RenewalResidencyFormStatus.Submitted;
            StaffRenewalResidency.LastFlowStatus = "Submitted";
            StaffRenewalResidency.ContractStartDate = _staffcore.StaffEOD;
            StaffRenewalResidency.ContractEndDate = _staffcore.ContractEndDate;
          

            dataStaffRenwalResidencyFlow flow = new dataStaffRenwalResidencyFlow
            {
                RenwalResidencyStaffFlowGUID = Guid.NewGuid(),
                StaffRenwalResidencyGUID = EntityPK,
                FlowStatusGUID = RenewalResidencyFormStatus.Submitted,
                IsLastAction = true,
                CreateByGUID = UserGUID,
                CreateDate = ExecutionTime,
                OrderId = 1


            };
            DbAHD.Create(StaffRenewalResidency, Permissions.StaffRenewalResidency.CreateGuid, ExecutionTime, DbCMS);
            DbAHD.Create(flow, Permissions.StaffRenewalResidency.CreateGuid, ExecutionTime, DbCMS);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.StaffRenewalResidencyDataTable, ControllerContext, "StaffRenewalResidencyLanguagesFormControls"));
            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.StaffRenewalResidency.Create, Apps.AHD, new UrlHelper(Request.RequestContext).Action("Create", "StaffRenewalResidencys", new { Area = "AHD" })), Container = "StaffRenewalResidencyDetailFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.StaffRenewalResidency.Update, Apps.AHD), Container = "StaffRenewalResidencyDetailFormControls" });

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();

                return Json(DbAHD.SingleUpdateMessage(DataTableNames.StaffRenewalResidencyDataTable, DbAHD.PrimaryKeyControl(StaffRenewalResidency), DbAHD.RowVersionControls(Portal.SingleToList(StaffRenewalResidency))));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffRenewalResidencyUpdate(StaffRenwalResidencyModel model)
        {
            if (!CMS.HasAction(Permissions.StaffRenewalResidency.Update, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            if (!ModelState.IsValid) return PartialView("~/Areas/AHD/Views/StaffRenewalResidencys/_StaffRenewalResidencyForm.cshtml", model);
            if(model.LastFlowStatusGUID==null || model.FormTypeGUID==null || model.FormTypeGUID==Guid.Empty) return PartialView("~/Areas/AHD/Views/StaffRenewalResidencys/_StaffRenewalResidencyForm.cshtml", model);
            DateTime ExecutionTime = DateTime.Now;


            var renewalRes = DbAHD.dataStaffRenwalResidency.Where(x => x.StaffRenwalResidencyGUID == model.StaffRenwalResidencyGUID).FirstOrDefault();
            if (renewalRes.LastFlowStatusGUID != model.LastFlowStatusGUID)
            {
                dataStaffRenwalResidency StaffRenewalResidency = DbAHD.dataStaffRenwalResidency.Where(x => x.StaffRenwalResidencyGUID == model.StaffRenwalResidencyGUID).FirstOrDefault();
                StaffRenewalResidency.LastFlowStatus = DbAHD.codeTablesValuesLanguages.Where(x => x.ValueGUID == model.LastFlowStatusGUID && x.LanguageID == LAN).Select(x => x.ValueDescription).FirstOrDefault();
                StaffRenewalResidency.FormTypeGUID = model.FormTypeGUID;
                var priFlow = DbAHD.dataStaffRenwalResidencyFlow.Where(x => x.StaffRenwalResidencyGUID == model.StaffRenwalResidencyGUID && x.IsLastAction == true).FirstOrDefault();
                priFlow.IsLastAction = false;
                DbAHD.Update(priFlow, Permissions.StaffRenewalResidency.UpdateGuid, ExecutionTime, DbCMS);
                dataStaffRenwalResidencyFlow flow = new dataStaffRenwalResidencyFlow
                {
                    RenwalResidencyStaffFlowGUID = Guid.NewGuid(),
                    StaffRenwalResidencyGUID = model.StaffRenwalResidencyGUID,
                    FlowStatusGUID = RenewalResidencyFormStatus.Submitted,
                    IsLastAction = true,
                    CreateByGUID = UserGUID,
                    CreateDate = ExecutionTime,
                    OrderId = priFlow.OrderId + 1


                };
                StaffRenewalResidency.RefNumber = model.RefNumber;
                StaffRenewalResidency.LastFlowStatusGUID = model.LastFlowStatusGUID;
                
                StaffRenewalResidency.SentDate = model.SentDate;
                //StaffRenewalResidency.EntryDateToSyria = model.EntryDateToSyria;
                DbAHD.Update(StaffRenewalResidency, Permissions.StaffRenewalResidency.UpdateGuid, ExecutionTime, DbCMS);
                if (model.StaffGUID!= StaffRenewalResidency.StaffGUID)
                {
                    var _staffcore = DbAHD.StaffCoreData.Where(x => x.UserGUID == model.StaffGUID).FirstOrDefault();

                    var _personaldetaillang = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == model.StaffGUID).ToList();
                    var _jobtitle = DbAHD.codeJobTitlesLanguages.Where(x => x.Active).ToList();
                    var _countris = DbAHD.codeCountriesLanguages.Where(x => x.Active).ToList();

                    var _dutystations = DbAHD.codeDutyStationsLanguages.Where(x => x.Active).ToList();
                    var _departments = DbAHD.codeDepartmentsLanguages.Where(x => x.Active).ToList();

                    StaffRenewalResidency.RefNumber = model.RefNumber;
                    StaffRenewalResidency.StaffGUID = model.StaffGUID;
                    StaffRenewalResidency.CurrentResidencyEndDateSent = _staffcore.ExpiryOfResidencyVisa;
                    StaffRenewalResidency.NextExtensionRequest = _staffcore.ExpiryOfResidencyVisa.Value.AddDays(-42);
                    StaffRenewalResidency.EnStaffName = _personaldetaillang.Where(x => x.LanguageID == "EN").FirstOrDefault().FirstName + " " + _personaldetaillang.Where(x => x.LanguageID == "EN").FirstOrDefault().Surname;
                    StaffRenewalResidency.ArStaffName = _personaldetaillang.Where(x => x.LanguageID == "AR").FirstOrDefault().FirstName + " " + _personaldetaillang.Where(x => x.LanguageID == "AR").FirstOrDefault().Surname;
                    StaffRenewalResidency.EnDeptName = _departments.Where(x => x.LanguageID == "EN" && x.DepartmentGUID == _staffcore.DepartmentGUID).FirstOrDefault().DepartmentDescription;
                    StaffRenewalResidency.ArDeptName = _departments.Where(x => x.LanguageID == "AR" && x.DepartmentGUID == _staffcore.DepartmentGUID).FirstOrDefault().DepartmentDescription;
                    StaffRenewalResidency.Gender = DbAHD.codeTablesValuesLanguages.Where(x => x.ValueGUID == _staffcore.Gender && x.LanguageID==LAN).FirstOrDefault().ValueDescription;

                    StaffRenewalResidency.EnNationality = _countris.Where(x => x.CountryGUID == _staffcore.NationalityGUID && x.LanguageID == "EN").FirstOrDefault().CountryDescription;
                    StaffRenewalResidency.ArNationality = _countris.Where(x => x.CountryGUID == _staffcore.NationalityGUID && x.LanguageID == "AR").FirstOrDefault().Nationality;
                    //StaffRenewalResidency.ReturnDate = model.ReturnDate;
                    StaffRenewalResidency.SentDate = model.SentDate;
                    Guid unlpGUID = Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E3732");
                    var _unlp = DbAHD.dataStaffCoreDocument.Where(x => x.UserGUID == _staffcore.UserGUID && x.DocumentTypeGUID == unlpGUID
                     && x.Active).FirstOrDefault();
                    StaffRenewalResidency.PassportNumber = _unlp.DocumentNumber;
                    StaffRenewalResidency.PassportValidityDate = _unlp.DocumentDateOfExpiry;
                    //StaffRenewalResidency.PassportNumber = _staffcore.UNLPNumber;
                    //StaffRenewalResidency.PassportValidityDate = _staffcore.UNLPDateOfExpiry;
                    StaffRenewalResidency.EnJobTitle = _jobtitle.Where(x => x.LanguageID == "EN" && x.JobTitleGUID == _staffcore.JobTitleGUID).FirstOrDefault().JobTitleDescription;
                    StaffRenewalResidency.ArJobTitle = _staffcore.Gender == Guid.Parse("688B11E0-24FB-44B8-94CE-D8568C4742C7") ? _jobtitle.Where(x => x.LanguageID == "AR" && x.JobTitleGUID == _staffcore.JobTitleGUID).FirstOrDefault().JobTitleDescription : _jobtitle.Where(x => x.LanguageID == "AR" && x.JobTitleGUID == _staffcore.JobTitleGUID).FirstOrDefault().JobTitleDescriptionFemale;
                    //StaffRenewalResidency.ArJobTitle = _jobtitle.Where(x => x.LanguageID == "AR" && x.JobTitleGUID == _staffcore.JobTitleGUID).FirstOrDefault().JobTitleDescription;

                    StaffRenewalResidency.EnOffice = _dutystations.Where(x => x.LanguageID == "EN" && x.DutyStationGUID == _staffcore.DutyStationGUID).FirstOrDefault().DutyStationDescription;
                    StaffRenewalResidency.ArOffice = _dutystations.Where(x => x.LanguageID == "AR" && x.DutyStationGUID == _staffcore.DutyStationGUID).FirstOrDefault().DutyStationDescription;



                    StaffRenewalResidency.CreateDate = ExecutionTime;
                    StaffRenewalResidency.DepartureDateFromSyria = model.DepartureDateFromSyria;
                    StaffRenewalResidency.CreateByGUID = UserGUID;
                    StaffRenewalResidency.LastFlowStatusGUID = RenewalResidencyFormStatus.Submitted;
                    
                    StaffRenewalResidency.ContractStartDate = _staffcore.StaffEOD;
                    StaffRenewalResidency.ContractEndDate = _staffcore.ContractEndDate;


                }

                DbAHD.Update(StaffRenewalResidency, Permissions.StaffRenewalResidency.UpdateGuid, ExecutionTime, DbCMS);
                DbAHD.Create(flow, Permissions.StaffRenewalResidency.CreateGuid, ExecutionTime, DbCMS);
            }
            else
            {
                var newrenewalRes = DbAHD.dataStaffRenwalResidency.Where(x => x.StaffRenwalResidencyGUID == model.StaffRenwalResidencyGUID).FirstOrDefault();
                newrenewalRes.RefNumber = model.RefNumber;
                newrenewalRes.ReturnDate = model.ReturnDate;
                newrenewalRes.SentDate = model.SentDate;
                newrenewalRes.LastFlowStatusGUID = model.LastFlowStatusGUID;
                //dataStaffRenwalResidency StaffRenewalResidency = Mapper.Map(model, new dataStaffRenwalResidency());
                DbAHD.Update(newrenewalRes, Permissions.StaffRenewalResidency.UpdateGuid, ExecutionTime, DbCMS);
            }




            try
            {

                dataStaffRenwalResidency StaffRenewalResidency = Mapper.Map(model, new dataStaffRenwalResidency());
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.SingleUpdateMessage(DataTableNames.StaffRenewalResidencyDataTable, DbAHD.PrimaryKeyControl(StaffRenewalResidency), DbAHD.RowVersionControls(Portal.SingleToList(StaffRenewalResidency))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyStaffRenewalResidency(model.StaffRenwalResidencyGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffRenewalResidencyDelete(dataStaffRenwalResidency model)
        {
            if (!CMS.HasAction(Permissions.StaffRenewalResidency.Delete, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            List<dataStaffRenwalResidency> DeletedStaffRenewalResidency = DeleteStaffRenewalResidency(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.StaffRenewalResidency.Restore, Apps.AHD), Container = "StaffRenewalResidencyFormControls" });

            try
            {
                int CommitedRows = DbAHD.SaveChanges();
                DbAHD.SaveChanges();
                return Json(DbAHD.SingleDeleteMessage(CommitedRows, DeletedStaffRenewalResidency.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyStaffRenewalResidency(model.StaffRenwalResidencyGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffRenewalResidencyRestore(dataStaffRenwalResidency model)
        {
            if (!CMS.HasAction(Permissions.StaffRenewalResidency.Restore, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            if (ActiveStaffRenewalResidency(model))
            {
                return Json(DbAHD.RecordExists());
            }

            List<dataStaffRenwalResidency> RestoredStaffRenewalResidency = RestoreStaffRenewalResidencys(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.StaffRenewalResidency.Create, Apps.AHD, new UrlHelper(Request.RequestContext).Action("StaffRenewalResidencyCreate", "Configuration", new { Area = "AHD" })), Container = "StaffRenewalResidencyFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.StaffRenewalResidency.Update, Apps.AHD), Container = "StaffRenewalResidencyFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.StaffRenewalResidency.Delete, Apps.AHD), Container = "StaffRenewalResidencyFormControls" });

            try
            {
                int CommitedRows = DbAHD.SaveChanges();
                DbAHD.SaveChanges();
                return Json(DbAHD.SingleRestoreMessage(CommitedRows, RestoredStaffRenewalResidency, DbAHD.PrimaryKeyControl(RestoredStaffRenewalResidency.FirstOrDefault()), Url.Action(DataTableNames.StaffRenewalResidencyDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyStaffRenewalResidency(model.StaffRenwalResidencyGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult StaffRenewalResidencyDataTableDelete(List<dataStaffRenwalResidency> models)
        {
            if (!CMS.HasAction(Permissions.StaffRenewalResidency.Delete, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            List<dataStaffRenwalResidency> DeletedStaffRenewalResidency = DeleteStaffRenewalResidency(models);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.PartialDeleteMessage(DeletedStaffRenewalResidency, models, DataTableNames.StaffRenewalResidencyDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult StaffRenewalResidencyDataTableRestore(List<dataStaffRenwalResidency> models)
        {
            if (!CMS.HasAction(Permissions.StaffRenewalResidency.Restore, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            List<dataStaffRenwalResidency> RestoredStaffRenewalResidency = DeleteStaffRenewalResidency(models);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.PartialRestoreMessage(RestoredStaffRenewalResidency, models, DataTableNames.StaffRenewalResidencyDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        private List<dataStaffRenwalResidency> DeleteStaffRenewalResidency(List<dataStaffRenwalResidency> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<dataStaffRenwalResidency> DeletedStaffRenewalResidency = new List<dataStaffRenwalResidency>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = "SELECT StaffRenwalResidencyGUID,CONVERT(varchar(50), StaffRenwalResidencyGUID) as C2 ,dataStaffRenwalResidencyRowVersion FROM code.dataStaffRenwalResidency where StaffRenwalResidencyGUID in (" + string.Join(",", models.Select(x => "'" + x.StaffRenwalResidencyGUID + "'").ToArray()) + ")";

            string query = DbAHD.QueryBuilder(models, Permissions.StaffRenewalResidency.DeleteGuid, SubmitTypes.Delete, "");

            var Records = DbAHD.Database.SqlQuery<dataStaffRenwalResidency>(query).ToList();
            foreach (var record in Records)
            {
                DeletedStaffRenewalResidency.Add(DbAHD.Delete(record, ExecutionTime, Permissions.StaffRenewalResidency.DeleteGuid, DbCMS));
            }


            return DeletedStaffRenewalResidency;
        }

        private List<dataStaffRenwalResidency> RestoreStaffRenewalResidencys(List<dataStaffRenwalResidency> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<dataStaffRenwalResidency> RestoredStaffRenewalResidency = new List<dataStaffRenwalResidency>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = "SELECT StaffRenwalResidencyGUID,CONVERT(varchar(50), StaffRenwalResidencyGUID) as C2 ,dataStaffRenwalResidencyRowVersion FROM code.dataStaffRenwalResidency where StaffRenwalResidencyGUID in (" + string.Join(",", models.Select(x => "'" + x.StaffRenwalResidencyGUID + "'").ToArray()) + ")";

            string query = DbAHD.QueryBuilder(models, Permissions.StaffRenewalResidency.DeleteGuid, SubmitTypes.Restore, "");

            var Records = DbAHD.Database.SqlQuery<dataStaffRenwalResidency>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveStaffRenewalResidency(record))
                {
                    RestoredStaffRenewalResidency.Add(DbAHD.Restore(record, Permissions.StaffRenewalResidency.DeleteGuid, Permissions.StaffRenewalResidency.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredStaffRenewalResidency;
        }

        private JsonResult ConcurrencyStaffRenewalResidency(Guid PK)
        {
            StaffRenwalResidencyModel dbModel = new StaffRenwalResidencyModel();

            var StaffRenewalResidency = DbAHD.dataStaffRenwalResidency.Where(x => x.StaffRenwalResidencyGUID == PK).FirstOrDefault();
            var dbStaffRenewalResidency = DbAHD.Entry(StaffRenewalResidency).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbStaffRenewalResidency, dbModel);

            if (StaffRenewalResidency.dataStaffRenwalResidencyRowVersion.SequenceEqual(dbModel.dataStaffRenwalResidencyRowVersion))
            {
                return Json(DbAHD.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbAHD, dbModel, "LanguagesContainer"));
        }

        private bool ActiveStaffRenewalResidency(Object model)
        {
            dataStaffRenwalResidency StaffRenewalResidency = Mapper.Map(model, new dataStaffRenwalResidency());
            int ModelDescription = DbAHD.dataStaffRenwalResidency
                                    .Where(x => x.RefNumber == StaffRenewalResidency.RefNumber &&
                                                x.StaffGUID == StaffRenewalResidency.StaffGUID &&
                                                x.Active).Count();
            if (ModelDescription > 0)
            {
                ModelState.AddModelError("Renewal Residency", "Staff Renewal is already exists");
            }
            return (ModelDescription > 0);
        }


        #endregion
        #region Reports
        public ActionResult StaffRenewalResidencyReport(Guid? StaffRenwalResidencyGUID)
        {

            ReportViewer reportViewer = new ReportViewer();
            LocalReport localReport = new LocalReport();
            //localReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"/Reports/VoucherReport.rdlc";
            localReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Areas/AHD/Rdlc/StaffForms\StaffRenewalResidency.rdlc";
            AHDDataSet ResidencyRenewal_En = new AHDDataSet();
            AHDDataSet ResidencyRenewal_Ar = new AHDDataSet();
            sp_StaffRenewalResidencyTableAdapter RP_StaffRenewal_EN = new sp_StaffRenewalResidencyTableAdapter();
            RP_StaffRenewal_EN.Fill(ResidencyRenewal_En.sp_StaffRenewalResidency, StaffRenwalResidencyGUID, "EN");
            var EnResult = RP_StaffRenewal_EN.GetData(StaffRenwalResidencyGUID, "EN").ToList();

            sp_StaffRenewalResidencyTableAdapter RP_StaffRenewal_AR = new sp_StaffRenewalResidencyTableAdapter();
            RP_StaffRenewal_AR.Fill(ResidencyRenewal_Ar.sp_StaffRenewalResidency, StaffRenwalResidencyGUID, "EN");
            var ARResult = RP_StaffRenewal_AR.GetData(StaffRenwalResidencyGUID, "EN").ToList();
            EnResult = EnResult.ToList();
            ARResult = ARResult.ToList();
            if (EnResult == null)
            {
                return PartialView("_Empty");
            }

            DataTable dt = EnResult.ToList().CopyToDataTable();
            DataTable dt1 = ARResult.ToList().CopyToDataTable();
            //DataTable dt1 = asrResults.ToList().CopyToDataTable();
            //DataTable dt2 = NonIraqiRefresults.ToList().CopyToDataTable();
            //DataTable dt3 = summaryReports.ToList().CopyToDataTable();
            //DataTable dt4 = summaryReportHashTotals.ToList().CopyToDataTable();

            localReport.DataSources.Add(new ReportDataSource("SP_StaffRenewalResidency_EN", dt));
            localReport.DataSources.Add(new ReportDataSource("SP_StaffRenewalResidency_AR", dt1));
            //localReport.DataSources.Add(new ReportDataSource("NonIraqiRefugee", dt2));
            //localReport.DataSources.Add(new ReportDataSource("SummaryReport", dt3));
            //localReport.DataSources.Add(new ReportDataSource("summaryReportHashTotals", dt4));

            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension = "pdf";
            //The DeviceInfo settings should be changed based on the reportType 
            string deviceInfo = @"<DeviceInfo>              
                                         <OutputFormat>PDF</OutputFormat>              
                                         <PageWidth>21cm</PageWidth>              
                                         <PageHeight>29.7cm</PageHeight>          
                                         <MarginTop>0cm</MarginTop>          
                                         <MarginLeft>0cm</MarginLeft>            
                                         <MarginRight>0cm</MarginRight>       
                                         <MarginBottom>0cm</MarginBottom></DeviceInfo>";
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;


            renderedBytes = localReport.Render(
                reportType, deviceInfo, out mimeType, out encoding, out fileNameExtension,
                out streams, out warnings);

            var doc = new Document();
            doc.SetPageSize(iTextSharp.text.PageSize.A4);

            var reader = new PdfReader(renderedBytes);
            using (FileStream fs =
                new FileStream(
                    Server.MapPath("~/Uploads/AHD/Temp/Report_" + Convert.ToString(Session[SessionKeys.UserGUID]) +
                                   ".pdf"), FileMode.Create))
            {
                PdfStamper stamper = new PdfStamper(reader, fs);

                stamper.JavaScript =
                    "var pp = getPrintParams();pp.interactive = pp.constants.interactionLevel.automatic;pp.printerName = getPrintParams().printerName;print(pp);\r";
                stamper.Close();
            }

            reader.Close();
            FileStream fss =
                new FileStream(
                    Server.MapPath("~/Uploads/AHD/Temp/Report_" + Convert.ToString(Session[SessionKeys.UserGUID]) +
                                   ".pdf"), FileMode.Open);
            byte[] bytes = new byte[fss.Length];
            fss.Read(bytes, 0, Convert.ToInt32(fss.Length));
            fss.Close();
            System.IO.File.Delete(
                Server.MapPath("~/Uploads/AHD/Temp/Report_" + Convert.ToString(Session[SessionKeys.UserGUID]) +
                               ".pdf"));

            //Here we returns the file result for view(PDF)

            return File(bytes, "application/pdf");







            //return View("~/Areas/AHD/Views/Reports/Index.cshtml", null, null);
        }
        public ActionResult GetStaffOtherInformation(Guid PK)
        {
            StaffRenwalResidencyModel model = new StaffRenwalResidencyModel();

            model = DbAHD.StaffCoreData.Where(x => x.UserGUID == PK).Select(x => new StaffRenwalResidencyModel
            {
                PassportValidityDate = x.UNLPDateOfExpiry,
                CurrentResidencyEndDateSent = x.ExpiryOfResidencyVisa,
                StaffGUID = x.UserGUID
            }).FirstOrDefault();
            return PartialView("~/Areas/AHD/Views/StaffRenewalResidency/_StaffOtherInformation.cshtml", model);
        }

        #endregion


    }
}