using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.ViewModels;
using AutoMapper;
using LinqKit;
using AppsPortal.Areas.PRG.Models;
using PRG_DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using AppsPortal.Library;

namespace AppsPortal.Areas.PRG.Controllers
{
    public class ProgresController : PRGBaseController
    {
        [Route("PRG/Progres/Search/")]
        public ActionResult Index()
        {
            return View("~/Areas/PRG/Views/Progres/Index.cshtml");
        }

        [Route("PRG/Progres/IndividualsDataTable/")]
        public JsonResult IndividualsDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<IndividualDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<IndividualDataTableModel>(DataTable.Filters);
            }

            var All = (from a in DbPRG.dataIndividual.AsExpandable()
                       join b in DbPRG.dataIndividualProcessGroup on a.IndividualGUID equals b.IndividualGUID 
                       join c in DbPRG.dataProcessGroup on b.ProcessingGroupGUID equals c.ProcessingGroupGUID 

                       select new IndividualDataTableModel
                       {
                           IndividualGUID = a.IndividualGUID,
                           IndividualID = a.IndividualID,
                           ProcessingGroupNumber = c.ProcessingGroupNumber,
                           FamilyName = a.FamilyName,
                           GivenName = a.GivenName,
                           MaidenName = a.MaidenName,
                           MiddleName = a.MiddleName,
                           MotherName = a.MotherName,
                           VerbalName = a.VerbalName,
                           DateofBirth = a.DateofBirth,
                           IndividualAge = a.IndividualAge,
                           NationalityCode = a.NationalityCode,
                           OriginCountryCode = a.OriginCountryCode,
                           SexCode = a.SexCode,
                           ProcessStatusCode = a.ProcessStatusCode,
                           RegistrationDate = a.RegistrationDate,
                           RefugeeStatusCode = a.RefugeeStatusCode
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<IndividualDataTableModel> Result = Mapper.Map<List<IndividualDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("PRG/Progres/LoadingPhoto/{FK}")]
        public JsonResult LoadingPhoto(Guid FK)
        {
            var result = DbPRG.dataPhotograph.Where(x => x.IndividualGUID == FK && x.PhotoActive).Select(x => x.Photo).FirstOrDefault();
            return  Json(new  { Photo = result}, JsonRequestBehavior.AllowGet);
        }

        [Route("PRG/Progres/DocumentsDataTable/{FK}")]
        public ActionResult DocumentsDataTable(DataTableRecievedOptions options, Guid FK)
        {
            if (options.columns == null) return PartialView("~/Areas/PRG/Views/Progres/_DocumentsDataTable.cshtml", new MasterRecordStatus { ParentGUID = FK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<dataDocument, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<dataDocument>(DataTable.Filters);
            }

            var Result = DbPRG.dataDocument.AsNoTracking().AsExpandable().Where(x => x.IndividualGUID == FK)
                          .Where(Predicate)
                              .Select(x => new
                              {
                                  x.DocumentGUID,
                                  DocumentTypeText = DbPRG.codeDocumentTypeText.Where(y=>y.DocumentTypeCode==x.DocumentType).FirstOrDefault().DocumentTypeText.Replace("(no event)", ""),
                                  x.DocumentNumber,
                                  x.DocumentDateIssued,
                                  x.DocumentDateExpire,
                                  x.DocumentStatusCode
                              });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result.ToList()), JsonRequestBehavior.AllowGet);

        }
    }
}