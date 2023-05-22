using AHD_DAL.Model;
using AHD_DAL.ViewModels;
using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.Library.MimeDetective;
using AppsPortal.ViewModels;
using AutoMapper;
using DocumentFormat.OpenXml.Bibliography;
using FineUploader;
using iTextSharp.text.pdf.qrcode;
using LinqKit;

using RES_Repo.Globalization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using WebGrease.Css.Ast;

namespace AppsPortal.Areas.AHD.Controllers
{
    public class MissionRequestController : AHDBaseController
    {
        #region All Missions
        [Route("AHD/AllStaffMissionsIndex/")]
        public ActionResult AllStaffMissionsIndex()
        {
            if (!CMS.HasAction(Permissions.StaffMissionRequest.Access, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }

            return View("~/Areas/AHD/Views/MissionRequest/MissionReview/Index.cshtml");
        }

        [Route("AHD/StaffAllMissionRequestDataTable/")]
        public JsonResult StaffAllMissionRequestDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<StaffMissionRequestDataTable, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<StaffMissionRequestDataTable>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.StaffSalaryProcess.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix

            var All = (
                from a in DbAHD.dataMissionRequest.Where(x => x.Active && x.ReferenceNumber != null && (x.LastFlowStatusGUID != AHDMissionStatusFlow.Draft) && x.LastFlowStatusGUID != null).AsExpandable()
                join b in DbAHD.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.LastFlowStatusGUID equals b.ValueGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()
                join c in DbAHD.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.StaffGUID equals c.UserGUID into LJ2
                from R2 in LJ2.DefaultIfEmpty()
                join d in DbAHD.v_StaffProfileInformation on a.StaffGUID equals d.UserGUID into LJ3
                from R3 in LJ3.DefaultIfEmpty()

                join e in DbAHD.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.TypeOfTravelGUID equals e.ValueGUID into LJ4
                from R4 in LJ4.DefaultIfEmpty()

                join f in DbAHD.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.TypeOfMissionGUID equals f.ValueGUID into LJ5
                from R5 in LJ5.DefaultIfEmpty()
                select new StaffMissionRequestDataTable
                {
                    MissionRequestGUID = a.MissionRequestGUID.ToString(),
                    Active = a.Active,
                    SequenceNumber = a.SequenceNumber,
                    ReferenceNumber = a.ReferenceNumber,
                    StaffGUID = a.StaffGUID.ToString(),
                    DepartmentGUID = a.DepartmentGUID.ToString(),
                    DutyStationGUID = a.DutyStationGUID.ToString(),
                    VersionNumber = a.VersionNumber,
                    MissionYear = a.MissionYear.ToString(),
                    DepartureDate = a.DepartureDate,
                    ReturnDate = a.ReturnDate,
                    LastFlowStatusGUID = a.LastFlowStatusGUID.ToString(),
                    LastFlowStatus = a.LastFlowStatus,
                    CreateDate = a.CreateDate,
                    PurposeOfMission = a.PurposeOfMission,

                    TypeOfMission = R4.ValueDescription,
                    TypeOfTravel = R5.ValueDescription,
                    StaffName = R3.FirstName + " " + R3.SurName,
                    dataMissionRequestRowVersion = a.dataMissionRequestRowVersion
                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<StaffMissionRequestDataTable> Result = Mapper.Map<List<StaffMissionRequestDataTable>>(All.Skip(DataTable.Start).Take(DataTable.Length).OrderByDescending(x => x.DepartureDate).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }
        #endregion
        // GET: AHD/MissionRequest
        #region Staff Missions

        [Route("AHD/MissionRequest/WorkplaceStaffMissionRequestIndex/")]
        public ActionResult StaffMissionRequestIndex()
        {
            //if (!CMS.HasAction(Permissions.StaffMissionRequest.Access, Apps.AHD))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}

            return View("~/Areas/AHD/Views/MissionRequest/StaffMissions/Index.cshtml");
        }
        [Route("AHD/StaffMissionRequestDataTable/")]
        public JsonResult StaffMissionRequestDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<StaffMissionRequestDataTable, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<StaffMissionRequestDataTable>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.StaffSalaryProcess.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix

            var All = (
                from a in DbAHD.dataMissionRequest.Where(x => x.Active && x.StaffGUID == UserGUID && x.ReferenceNumber != null && x.LastFlowStatusGUID != null).AsExpandable()
                join b in DbAHD.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.LastFlowStatusGUID equals b.ValueGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()
                    //join c in DbAHD.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.StaffGUID equals c.UserGUID into LJ2
                    //from R2 in LJ2.DefaultIfEmpty()
                join d in DbAHD.v_StaffProfileInformation on a.StaffGUID equals d.UserGUID into LJ3
                from R3 in LJ3.DefaultIfEmpty()
                select new StaffMissionRequestDataTable
                {
                    MissionRequestGUID = a.MissionRequestGUID.ToString(),


                    Active = a.Active,
                    SequenceNumber = a.SequenceNumber,
                    ReferenceNumber = a.ReferenceNumber,
                    StaffGUID = a.StaffGUID.ToString(),
                    DepartmentGUID = R3.DepartmentGUID.ToString(),
                    DutyStationGUID = R3.DutyStationGUID.ToString(),
                    VersionNumber = a.VersionNumber,
                    MissionYear = a.MissionYear.ToString(),
                    DepartureDate = a.DepartureDate,
                    ReturnDate = a.ReturnDate,
                    LastFlowStatusGUID = a.LastFlowStatusGUID.ToString(),
                    LastFlowStatus = a.LastFlowStatus,
                    CreateDate = a.CreateDate,



                    StaffName = R3.FirstName + " " + R3.SurName,





                    dataMissionRequestRowVersion = a.dataMissionRequestRowVersion
                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<StaffMissionRequestDataTable> Result = Mapper.Map<List<StaffMissionRequestDataTable>>(All.Skip(DataTable.Start).Take(DataTable.Length).OrderByDescending(x => x.DepartureDate).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }
        //[Route("AHD/StaffMissionRequest/Create/")]

        [Route("AHD/StaffMissionRequest/Create/")]
        public ActionResult StaffMissionRequestCreate()
        {
            //if (!CMS.HasAction(Permissions.StaffSalaryProcess.Create, Apps.AHD))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            var _staff = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            StaffMissionRequestDataTableModel model = new StaffMissionRequestDataTableModel
            {
                MissionRequestGUID = Guid.NewGuid(),
                HasDamascusTransit = false,
                IsMissionCombinedWithLeave = false,
                IsMissionCombinedWithRR = false,
                AccessLevel = 1,
                CurrentStep = 1,
                StaffGUID = UserGUID,
                StaffName = _staff.FullName,
                //Grade= _staff.Grade,
                JobTitle = _staff.JobTitle,
                DepartmentGUID = _staff.DepartmentGUID,
                DutyStationGUID = _staff.DutyStationGUID,
                MissionYear = DateTime.Now.Year,
                RecruitmentTypeGUID = _staff.RecruitmentTypeGUID,



            };
            dataMissionRequest StaffMissionRequest = Mapper.Map(model, new dataMissionRequest());
            StaffMissionRequest.CreateDate = DateTime.Now;
            StaffMissionRequest.CreateByGUID = UserGUID;
            DbAHD.CreateNoAudit(StaffMissionRequest);

            try
            {

                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return View("~/Areas/AHD/Views/MissionRequest/StaffMissions/StaffMissionRequestForm.cshtml", model);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }


        }

        public ActionResult UpdateMissionRequestFirstStep(Guid _MissionRequestGUID, Guid _TypeOfMissionGUID, Guid _TypeOfTravelGUID,
                                                          bool _HasDamascusTransit, bool _IsMissionCombinedWithLeave,
                                                          bool _IsMissionCombinedWithRR, Guid? _RestAndRecuperationLeaveGUID,
                                                          string _PurposeOfMission,
                                                       DateTime _DepartureDate, DateTime _ReturnDate)
        {
            var allMissions = DbAHD.dataMissionRequest.Where(x => x.Active && x.StaffGUID == UserGUID).ToList();
            int _hasConflicatdate = 0;
            foreach (var model2 in allMissions)
            {
                if ((model2.DepartureDate <= _ReturnDate && (model2.ReturnDate >= _DepartureDate)))
                {
                    _hasConflicatdate = 1;
                }


            }

            var _mission = DbAHD.dataMissionRequest.Where(x => x.MissionRequestGUID == _MissionRequestGUID).FirstOrDefault();
            DateTime ExecutionTime = DateTime.Now;
            if (_DepartureDate > _ReturnDate)
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (_DepartureDate.Date < ExecutionTime.Date || _ReturnDate.Date < ExecutionTime.Date)
            {
                ModelState.AddModelError("Error: ", " Date should be Greater and not Bigger  than today ");
                return View("~/Areas/AHD/Views/MissionRequest/StaffMissions/StaffMissionRequestForm.cshtml", _mission);
            }
            //if (!CMS.HasAction(Permissions.MissionRequestAdminApproval.Access, Apps.AHD))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            if (!ModelState.IsValid || _TypeOfMissionGUID == null || _TypeOfTravelGUID == null)
            {
                return Json(DbAHD.PermissionError());
            }

            _mission.TypeOfMissionGUID = _TypeOfMissionGUID;
            _mission.PurposeOfMission = _PurposeOfMission;
            _mission.TypeOfTravelGUID = _TypeOfTravelGUID;
            _mission.HasDamascusTransit = _HasDamascusTransit;
            _mission.IsMissionCombinedWithLeave = _IsMissionCombinedWithLeave;
            _mission.IsMissionCombinedWithRR = _IsMissionCombinedWithRR;
            _mission.RestAndRecuperationLeaveGUID = _RestAndRecuperationLeaveGUID;
            _mission.DepartureDate = _DepartureDate;
            _mission.ReturnDate = _ReturnDate;
            //_mission.Comments = _Comments;
            var _staff = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            _mission.DutyStationGUID = _staff.DutyStationGUID;
            _mission.DepartmentGUID = _staff.DepartmentGUID;



            if (string.IsNullOrEmpty(_mission.ReferenceNumber) || _mission.ReferenceNumber == null)
            {
                var _lastseq = DbAHD.dataMissionRequest.Where(x => x.Active == true).Select(x => x.SequenceNumber).Max();

                var useroffice = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == UserGUID)
              .FirstOrDefault();

                var office = useroffice.DutyStation;
                _mission.SequenceNumber = _lastseq != null ? _lastseq + 1 : 1;
                string _office = "";
                if (useroffice.DutyStationGUID == Guid.Parse("6D7397D6-3D7F-48FC-BFD2-18E69673AC94"))
                {
                    _office = "ALP";
                }
                else
                {


                    _office = office.Substring(0, 3).ToUpper();
                }

                _mission.ReferenceNumber = "SYR" + "-" + _office + "-" + DateTime.Now.Year.ToString().Substring(0, 4) + "-" +
                                            _mission.SequenceNumber;
            }
            //StaffMissionRequest.RequestYear = model.RequestDate.Value.Year;
            if (_mission.LastFlowStatusGUID == null)
            {
                _mission.LastFlowStatusGUID = AHDMissionStatusFlow.Draft;
                _mission.LastFlowStatus = "Draft";
                dataMissionRequestFlow flow = new dataMissionRequestFlow

                {
                    MissionRequestFlowGUID = Guid.NewGuid(),
                    MissionRequestGUID = _mission.MissionRequestGUID,
                    FlowStatusGUID = AHDMissionStatusFlow.Draft,
                    IsLastAction = true,
                    CreateByGUID = UserGUID,
                    CreateDate = ExecutionTime,
                    OrderId = 1
                };
                DbAHD.CreateNoAudit(flow);
            }


            DbAHD.UpdateNoAudit(_mission);
            dataMissionRequestTraveler newTaveler = new dataMissionRequestTraveler
            {
                MissionRequestTravelerGUID = Guid.NewGuid(),
                MissionRequestGUID = _mission.MissionRequestGUID,
                TraverlerTypeGUID = Guid.Parse("2dac5d96-e6a3-48c1-b3f5-17bfd9f62966"),
                StaffGUID = UserGUID,
                StaffName = _mission.StaffName,


            };
            DbAHD.CreateNoAudit(newTaveler);


            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(new
                {
                    success = 1,
                    _hasConflicatdate = _hasConflicatdate


                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }


        //[Route("AHD/StaffMissionRequest/Update/{PK}")]


        public ActionResult ReOpenMission(Guid PK)
        {
            var _mission = DbAHD.dataMissionRequest.Where(x => x.MissionRequestGUID == PK).FirstOrDefault();
            if (_mission.LastFlowStatusGUID == AHDMissionStatusFlow.Submitted)
            {
                _mission.LastFlowStatusGUID = AHDMissionStatusFlow.Draft;
                _mission.LastFlowStatus = "Draft";
            }
            DbAHD.UpdateNoAudit(_mission);
            DbAHD.SaveChanges();
            DbCMS.SaveChanges();


            var model = (from a in DbAHD.dataMissionRequest.WherePK(PK)
                         select new StaffMissionRequestDataTableModel
                         {
                             MissionRequestGUID = a.MissionRequestGUID,

                             MissionYear = a.MissionYear,
                             ReferenceNumber = a.ReferenceNumber,
                             SequenceNumber = a.SequenceNumber,
                             StaffGUID = a.StaffGUID,
                             StaffName = a.StaffName,
                             DepartmentGUID = a.DepartmentGUID,
                             DutyStationGUID = a.DutyStationGUID,
                             VersionNumber = a.VersionNumber,
                             TypeOfTravelGUID = a.TypeOfTravelGUID,
                             TypeOfMissionGUID = a.TypeOfMissionGUID,
                             PurposeOfMission = a.PurposeOfMission,
                             AdminApprovedByGUID = a.AdminApprovedByGUID,
                             AuthByDeputyRepresentativeGUID = a.AuthByDeputyRepresentativeGUID,
                             HeadOfUnitApprovedByGUID = a.HeadOfUnitApprovedByGUID,
                             HasDamascusTransit = (bool)a.HasDamascusTransit,
                             DepartureDate = a.DepartureDate,
                             ReturnDate = a.ReturnDate,
                             IsMissionCombinedWithLeave = (bool)a.IsMissionCombinedWithLeave,
                             Comments = a.Comments,
                             IsMissionCombinedWithRR = (bool)a.IsMissionCombinedWithRR,


                             RestAndRecuperationLeaveGUID = a.RestAndRecuperationLeaveGUID,
                             LastFlowStatusGUID = a.LastFlowStatusGUID,
                             LastFlowStatus = a.LastFlowStatus,

                             JobTitle = a.JobTitle,
                             AccessLevel = 1,
                             CurrentStep = 2,
                             AdminComments = a.AdminComments,
                             HeadOfUnitComments = a.HeadOfUnitComments,
                             AuthorizedComment = a.AuthorizedComment,



                             dataMissionRequestRowVersion = a.dataMissionRequestRowVersion,

                             Active = a.Active,
                             //DangerPaymentConfirmationStatus= R1.ValueDescription,


                         }).FirstOrDefault();

            model.TypeOfMission = DbAHD.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.ValueGUID == model.TypeOfMissionGUID).FirstOrDefault().ValueDescription;
            model.TypeOfTravel = DbAHD.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.ValueGUID == model.TypeOfTravelGUID).FirstOrDefault().ValueDescription;

            if (model.StaffGUID != UserGUID)
            {
                throw new HttpException(401, "Unauthorized access");
            }
            model.RequestStage = model.LastFlowStatusGUID == AHDMissionStatusFlow.Submitted ? 2 :
                   (model.LastFlowStatusGUID == AHDMissionStatusFlow.Verified ? 3 :
                   (model.LastFlowStatusGUID == AHDMissionStatusFlow.Reviewed ? 4 :
                   (model.LastFlowStatusGUID == AHDMissionStatusFlow.Approved ? 5 : 0
                   ))
                   );
            if (model.HeadOfUnitApprovedByGUID != null && model.HeadOfUnitApprovedByGUID != Guid.Empty)
            {
                model.HeadOfUnitApprovedName = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == model.HeadOfUnitApprovedByGUID).FirstOrDefault().FullName;
            }
            if (model.AuthByDeputyRepresentativeGUID != null && model.AuthByDeputyRepresentativeGUID != Guid.Empty)
            {
                model.AuthByDeputyRepresentativeName = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == model.AuthByDeputyRepresentativeGUID).FirstOrDefault().FullName;
            }
            if (model.AdminApprovedByGUID != null && model.AdminApprovedByGUID != Guid.Empty)
            {
                model.AdminApprovedBy = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == model.AdminApprovedByGUID).FirstOrDefault().FullName;
            }


            ViewBag.Stage = model.RequestStage;
            //ViewBag.MissionRequestGUID = PK;
            //ViewBag.TotalStaffNotConfirmed = DbAHD.dataMissionRequest.Where(a => a.FlowStatusGUID == NationalStaffDangerPaConfirmationStatus.Pending && a.MissionRequestGUID == PK).Count();
            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("StaffMissionRequest", "StaffMissionRequests", new { Area = "AHD" }));
            return View("~/Areas/AHD/Views/MissionRequest/StaffMissions/EditStaffMissionRequestForm.cshtml", model);

        }
        public ActionResult StaffMissionRequestAdminUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.MissionRequestAdminApproval.Confirm, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }

            var _mission = DbAHD.dataMissionRequest.Where(x => x.MissionRequestGUID == PK).FirstOrDefault();
            StaffMissionRequestDataTableModel model = Mapper.Map(_mission, new StaffMissionRequestDataTableModel());
            ViewBag.StaffName = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == model.StaffGUID).FirstOrDefault().FullName;
            model.TypeOfMission = DbAHD.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.ValueGUID == _mission.TypeOfMissionGUID).FirstOrDefault().ValueDescription;
            model.TypeOfTravel = DbAHD.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.ValueGUID == _mission.TypeOfTravelGUID).FirstOrDefault().ValueDescription;
            model.CurrentUserGUID = UserGUID;
            model.HeadOfUnitApprovedName = _mission.HeadOfUnitApprovedByGUID != null ? DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == _mission.HeadOfUnitApprovedByGUID).FirstOrDefault().FullName : "";
            model.AuthByDeputyRepresentativeName = _mission.AuthByDeputyRepresentativeGUID != null ? DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == _mission.AuthByDeputyRepresentativeGUID).FirstOrDefault().FullName : "";
            model.AdminApprovedBy = _mission.AdminApprovedByGUID != null ? DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == _mission.AdminApprovedByGUID).FirstOrDefault().FullName : "";
            model.CurrentUserGUID = UserGUID;
            model.RequestStage = _mission.LastFlowStatusGUID == AHDMissionStatusFlow.Submitted ? 2 :
                               (_mission.LastFlowStatusGUID == AHDMissionStatusFlow.Verified ? 3 :
                               (_mission.LastFlowStatusGUID == AHDMissionStatusFlow.Reviewed ? 4 :
                               (_mission.LastFlowStatusGUID == AHDMissionStatusFlow.Approved ? 5 : 0
                               ))
                               );
            ViewBag.Stage = model.RequestStage;
            return View("~/Areas/AHD/Views/MissionRequest/MissionReview/ReviewStaffMissionRequestForm.cshtml", model);


        }

        public ActionResult StaffMissionRequestUpdate(Guid PK)
        {

            var model = (from a in DbAHD.dataMissionRequest.WherePK(PK)
                         select new StaffMissionRequestDataTableModel
                         {
                             MissionRequestGUID = a.MissionRequestGUID,

                             MissionYear = a.MissionYear,
                             ReferenceNumber = a.ReferenceNumber,
                             SequenceNumber = a.SequenceNumber,
                             StaffGUID = a.StaffGUID,
                             StaffName = a.StaffName,
                             DepartmentGUID = a.DepartmentGUID,
                             DutyStationGUID = a.DutyStationGUID,
                             VersionNumber = a.VersionNumber,
                             TypeOfTravelGUID = a.TypeOfTravelGUID,
                             TypeOfMissionGUID = a.TypeOfMissionGUID,
                             PurposeOfMission = a.PurposeOfMission,
                             AdminApprovedByGUID = a.AdminApprovedByGUID,
                             AuthByDeputyRepresentativeGUID = a.AuthByDeputyRepresentativeGUID,
                             HeadOfUnitApprovedByGUID = a.HeadOfUnitApprovedByGUID,
                             HasDamascusTransit = (bool)a.HasDamascusTransit,
                             DepartureDate = a.DepartureDate,
                             ReturnDate = a.ReturnDate,
                             IsMissionCombinedWithLeave = (bool)a.IsMissionCombinedWithLeave,
                             Comments = a.Comments,
                             IsMissionCombinedWithRR = (bool)a.IsMissionCombinedWithRR,


                             RestAndRecuperationLeaveGUID = a.RestAndRecuperationLeaveGUID,
                             LastFlowStatusGUID = a.LastFlowStatusGUID,
                             LastFlowStatus = a.LastFlowStatus,

                             JobTitle = a.JobTitle,
                             AccessLevel = 1,
                             CurrentStep = 2,
                             AdminComments = a.AdminComments,
                             HeadOfUnitComments = a.HeadOfUnitComments,
                             AuthorizedComment = a.AuthorizedComment,



                             dataMissionRequestRowVersion = a.dataMissionRequestRowVersion,

                             Active = a.Active,
                             //DangerPaymentConfirmationStatus= R1.ValueDescription,


                         }).FirstOrDefault();

            model.TypeOfMission = DbAHD.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.ValueGUID == model.TypeOfMissionGUID).FirstOrDefault().ValueDescription;
            model.TypeOfTravel = DbAHD.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.ValueGUID == model.TypeOfTravelGUID).FirstOrDefault().ValueDescription;

            if (model.StaffGUID != UserGUID)
            {
                throw new HttpException(401, "Unauthorized access");
            }
            model.RequestStage = model.LastFlowStatusGUID == AHDMissionStatusFlow.Submitted ? 2 :
                   (model.LastFlowStatusGUID == AHDMissionStatusFlow.Verified ? 3 :
                   (model.LastFlowStatusGUID == AHDMissionStatusFlow.Reviewed ? 4 :
                   (model.LastFlowStatusGUID == AHDMissionStatusFlow.Approved ? 5 : 0
                   ))
                   );
            if (model.HeadOfUnitApprovedByGUID != null && model.HeadOfUnitApprovedByGUID != Guid.Empty)
            {
                model.HeadOfUnitApprovedName = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == model.HeadOfUnitApprovedByGUID).FirstOrDefault().FullName;
            }
            if (model.AuthByDeputyRepresentativeGUID != null && model.AuthByDeputyRepresentativeGUID != Guid.Empty)
            {
                model.AuthByDeputyRepresentativeName = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == model.AuthByDeputyRepresentativeGUID).FirstOrDefault().FullName;
            }
            if (model.AdminApprovedByGUID != null && model.AdminApprovedByGUID != Guid.Empty)
            {
                model.AdminApprovedBy = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == model.AdminApprovedByGUID).FirstOrDefault().FullName;
            }


            ViewBag.Stage = model.RequestStage;
            //ViewBag.MissionRequestGUID = PK;
            //ViewBag.TotalStaffNotConfirmed = DbAHD.dataMissionRequest.Where(a => a.FlowStatusGUID == NationalStaffDangerPaConfirmationStatus.Pending && a.MissionRequestGUID == PK).Count();
            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("StaffMissionRequest", "StaffMissionRequests", new { Area = "AHD" }));
            return View("~/Areas/AHD/Views/MissionRequest/StaffMissions/EditStaffMissionRequestForm.cshtml", model);

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffMissionRequestCreate(StaffMissionRequestDataTableModel model)
        {
            //if (!CMS.HasAction(Permissions.StaffSalaryProcess.Create, Apps.AHD))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            if (!ModelState.IsValid || ActiveStaffMissionRequest(model)) return PartialView("~/Areas/AHD/Vsiews/StaffMissionRequests/_StaffMissionRequestForm.cshtml", model);


            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            dataMissionRequest StaffMissionRequest = Mapper.Map(model, new dataMissionRequest());
            StaffMissionRequest.MissionRequestGUID = EntityPK;

            //StaffMissionRequest.LastFlowStatus = "Draft";



            DbAHD.UpdateNoAudit(StaffMissionRequest);
            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();

                return Json(DbAHD.SingleUpdateMessage(DataTableNames.StaffMissionRequestDataTable, DbAHD.PrimaryKeyControl(StaffMissionRequest), DbAHD.RowVersionControls(Portal.SingleToList(StaffMissionRequest))));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        //[HttpPost, ValidateAntiForgeryToken]
        //public ActionResult StaffMissionRequestUpdate(StaffMissionRequestDataTableModel model)
        //{
        //    //if (!CMS.HasAction(Permissions.StaffSalaryProcess.Update, Apps.AHD))
        //    //{
        //    //    throw new HttpException(401, "Unauthorized access");
        //    //}
        //    if (!ModelState.IsValid) return PartialView("~/Areas/AHD/Views/StaffMissionRequests/_StaffMissionRequestForm.cshtml", model);
        //    DateTime ExecutionTime = DateTime.Now;

        //    dataMissionRequest StaffMissionRequest = Mapper.Map(model, new dataMissionRequest());

        //    DbAHD.Update(StaffMissionRequest, Permissions.StaffSalaryProcess.UpdateGuid, ExecutionTime, DbCMS);
        //    try
        //    {
        //        DbAHD.SaveChanges();
        //        DbCMS.SaveChanges();
        //        return Json(DbAHD.SingleUpdateMessage(DataTableNames.StaffMissionRequestDataTable, DbAHD.PrimaryKeyControl(StaffMissionRequest), DbAHD.RowVersionControls(Portal.SingleToList(StaffMissionRequest))));
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        return ConcurrencyStaffMissionRequest(model.MissionRequestGUID);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(DbAHD.ErrorMessage(ex.Message));
        //    }
        //}

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffMissionRequestDelete(dataMissionRequest model)
        {
            //if (!CMS.HasAction(Permissions.StaffSalaryProcess.Delete, Apps.AHD))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            List<dataMissionRequest> DeletedStaffMissionRequest = DeleteStaffMissionRequest(Portal.SingleToList(model));
            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.StaffSalaryProcess.Restore, Apps.AHD), Container = "StaffMissionRequestFormControls" });
            try
            {
                int CommitedRows = DbAHD.SaveChanges();
                DbAHD.SaveChanges();
                return Json(DbAHD.SingleDeleteMessage(CommitedRows, DeletedStaffMissionRequest.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyStaffMissionRequest(model.MissionRequestGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffMissionRequestRestore(dataMissionRequest model)
        {
            //if (!CMS.HasAction(Permissions.StaffSalaryProcess.Restore, Apps.AHD))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            if (ActiveStaffMissionRequest(model))
            {
                return Json(DbAHD.RecordExists());
            }
            List<dataMissionRequest> RestoredStaffMissionRequest = RestoreStaffMissionRequests(Portal.SingleToList(model));
            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.StaffSalaryProcess.Create, Apps.AHD, new UrlHelper(Request.RequestContext).Action("StaffMissionRequestCreate", "Configuration", new { Area = "AHD" })), Container = "StaffMissionRequestFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.StaffSalaryProcess.Update, Apps.AHD), Container = "StaffMissionRequestFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.StaffSalaryProcess.Delete, Apps.AHD), Container = "StaffMissionRequestFormControls" });
            try
            {
                int CommitedRows = DbAHD.SaveChanges();
                DbAHD.SaveChanges();
                return Json(DbAHD.SingleRestoreMessage(CommitedRows, RestoredStaffMissionRequest, DbAHD.PrimaryKeyControl(RestoredStaffMissionRequest.FirstOrDefault()), Url.Action(DataTableNames.StaffMissionRequestDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyStaffMissionRequest(model.MissionRequestGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult StaffMissionRequestDataTableDelete(List<dataMissionRequest> models)
        {
            //if (!CMS.HasAction(Permissions.StaffSalaryProcess.Delete, Apps.AHD))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            List<dataMissionRequest> DeletedStaffMissionRequest = DeleteStaffMissionRequest(models);
            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.PartialDeleteMessage(DeletedStaffMissionRequest, models, DataTableNames.StaffMissionRequestDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult StaffMissionRequestDataTableRestore(List<dataMissionRequest> models)
        {
            //if (!CMS.HasAction(Permissions.StaffSalaryProcess.Restore, Apps.AHD))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            List<dataMissionRequest> RestoredStaffMissionRequest = DeleteStaffMissionRequest(models);
            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.PartialRestoreMessage(RestoredStaffMissionRequest, models, DataTableNames.StaffMissionRequestDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        private List<dataMissionRequest> DeleteStaffMissionRequest(List<dataMissionRequest> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<dataMissionRequest> DeletedStaffMissionRequest = new List<dataMissionRequest>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = "SELECT MissionRequestGUID,CONVERT(varchar(50), MissionRequestGUID) as C2 ,dataMissionRequestRowVersion FROM code.dataMissionRequest where MissionRequestGUID in (" + string.Join(",", models.Select(x => "'" + x.MissionRequestGUID + "'").ToArray()) + ")";
            string query = DbAHD.QueryBuilder(models, Permissions.StaffSalaryProcess.DeleteGuid, SubmitTypes.Delete, "");
            var Records = DbAHD.Database.SqlQuery<dataMissionRequest>(query).ToList();
            foreach (var record in Records)
            {
                DeletedStaffMissionRequest.Add(DbAHD.Delete(record, ExecutionTime, Permissions.StaffSalaryProcess.DeleteGuid, DbCMS));
            }
            return DeletedStaffMissionRequest;
        }
        private List<dataMissionRequest> RestoreStaffMissionRequests(List<dataMissionRequest> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<dataMissionRequest> RestoredStaffMissionRequest = new List<dataMissionRequest>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = "SELECT MissionRequestGUID,CONVERT(varchar(50), MissionRequestGUID) as C2 ,dataMissionRequestRowVersion FROM code.dataMissionRequest where MissionRequestGUID in (" + string.Join(",", models.Select(x => "'" + x.MissionRequestGUID + "'").ToArray()) + ")";

            string query = DbAHD.QueryBuilder(models, Permissions.StaffSalaryProcess.DeleteGuid, SubmitTypes.Restore, "");

            var Records = DbAHD.Database.SqlQuery<dataMissionRequest>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveStaffMissionRequest(record))
                {
                    RestoredStaffMissionRequest.Add(DbAHD.Restore(record, Permissions.StaffSalaryProcess.DeleteGuid, Permissions.StaffSalaryProcess.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredStaffMissionRequest;
        }

        private JsonResult ConcurrencyStaffMissionRequest(Guid PK)
        {
            StaffMissionRequestDataTable dbModel = new StaffMissionRequestDataTable();

            var StaffMissionRequest = DbAHD.dataMissionRequest.Where(x => x.MissionRequestGUID == PK).FirstOrDefault();
            var dbStaffMissionRequest = DbAHD.Entry(StaffMissionRequest).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbStaffMissionRequest, dbModel);

            if (StaffMissionRequest.dataMissionRequestRowVersion.SequenceEqual(dbModel.dataMissionRequestRowVersion))
            {
                return Json(DbAHD.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbAHD, dbModel, "LanguagesContainer"));
        }

        private bool ActiveStaffMissionRequest(Object model)
        {
            dataMissionRequest StaffMissionRequest = Mapper.Map(model, new dataMissionRequest());
            int ModelDescription = DbAHD.dataMissionRequest
                                    .Where(x => x.MissionCode == StaffMissionRequest.MissionCode &&
                                                x.MissionYear == StaffMissionRequest.MissionYear &&
                                                x.Active).Count();
            if (ModelDescription > 0)
            {
                ModelState.AddModelError("Record ", " already exists");
            }
            return (ModelDescription > 0);
        }


        #endregion

        #region Itinerary


        //[Route("AHD/MissionRequestItineraryDataTable/{PK}")]

        public ActionResult MissionRORequestItineraryDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/AHD/Views/MissionRequest/Itinerary/_MissionRequestItineraryDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<MissionRequestItineraryDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<MissionRequestItineraryDataTableModel>(DataTable.Filters);
            }

            // Guid myDocumentTypeGUID = Guid.Parse("2d0b59f3-347b-4fa1-8793-a7741d4c35bd");
            Guid _nvGUID = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7891");
            var Result = (
                  from a in DbAHD.dataMissionRequestItinerary.AsExpandable().Where(x => x.Active && (x.MissionRequestGUID == PK))

                  join b in DbAHD.codeTablesValuesLanguages.AsExpandable().Where(x => x.Active && x.LanguageID == LAN
                  && x.codeTablesValues.TableGUID == _nvGUID) on a.ItineraryTypeGUID equals b.ValueGUID into LJ1
                  from R1 in LJ1.DefaultIfEmpty()
                  select new MissionRequestItineraryDataTableModel
                  {
                      MissionRequestItineraryGUID = a.MissionRequestItineraryGUID.ToString(),
                      MissionRequestGUID = a.MissionRequestGUID.ToString(),
                      TravelDate = a.TravelDate,
                      ItineraryType = R1.ValueDescription,
                      FromLocationGUID = a.FromLocationGUID.ToString(),
                      ToLocationGUID = a.ToLocationGUID.ToString(),
                      FromLocationName = a.FromLocationName,
                      ToLocationName = a.ToLocationName,
                      TravelModeGUID = a.TravelModeGUID.ToString(),
                      //TravelMode = a.TravelMode,
                      IsPrivate = a.IsPrivate,
                      AccommodationProvidedGUID = a.AccommodationProvidedGUID.ToString(),
                      //a.AccommodationProvided,
                      MealsProvidedGUID = a.MealsProvidedGUID.ToString(),
                      //a.MealsProvidedName,
                      Comments = a.Comments,
                      //Description = a.Description,
                      dataMissionRequestItineraryRowVersion = a.dataMissionRequestItineraryRowVersion,

                      Active = a.Active,


                  }).Where(Predicate);


            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult MissionRequestItineraryDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/AHD/Views/MissionRequest/Itinerary/_MissionRequestItineraryDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<MissionRequestItineraryDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<MissionRequestItineraryDataTableModel>(DataTable.Filters);
            }

            // Guid myDocumentTypeGUID = Guid.Parse("2d0b59f3-347b-4fa1-8793-a7741d4c35bd");
            Guid _nvGUID = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7891");
            var Result = (
                  from a in DbAHD.dataMissionRequestItinerary.AsExpandable().Where(x => x.Active && (x.MissionRequestGUID == PK))

                  join b in DbAHD.codeTablesValuesLanguages.AsExpandable().Where(x => x.Active && x.LanguageID == LAN
                  && x.codeTablesValues.TableGUID == _nvGUID) on a.ItineraryTypeGUID equals b.ValueGUID into LJ1
                  from R1 in LJ1.DefaultIfEmpty()
                  select new MissionRequestItineraryDataTableModel
                  {
                      MissionRequestItineraryGUID = a.MissionRequestItineraryGUID.ToString(),
                      MissionRequestGUID = a.MissionRequestGUID.ToString(),
                      TravelDate = a.TravelDate,
                      ItineraryType = R1.ValueDescription,
                      FromLocationGUID = a.FromLocationGUID.ToString(),
                      ToLocationGUID = a.ToLocationGUID.ToString(),
                      FromLocationName = a.FromLocationName,
                      ToLocationName = a.ToLocationName,
                      TravelModeGUID = a.TravelModeGUID.ToString(),
                      //TravelMode = a.TravelMode,
                      IsPrivate = a.IsPrivate,
                      AccommodationProvidedGUID = a.AccommodationProvidedGUID.ToString(),
                      //a.AccommodationProvided,
                      MealsProvidedGUID = a.MealsProvidedGUID.ToString(),
                      //a.MealsProvidedName,
                      Comments = a.Comments,
                      //Description = a.Description,
                      dataMissionRequestItineraryRowVersion = a.dataMissionRequestItineraryRowVersion,

                      Active = a.Active,


                  }).Where(Predicate);


            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }
        //[Route("AHD/TemplateConfiguration/MissionRequestItineraryCreate/")]

        public ActionResult MissionRequestItinerarySeqmentDateCreate(Guid FK)
        {
            //if (!CMS.HasAction(Permissions.StaffMissionRequest.Create, Apps.AHD))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}

            MissionRequestItineraryUpdateModel model = new MissionRequestItineraryUpdateModel
            {
                MissionRequestGUID = FK,
                IsPrivate = false,
                IsMissionOwner = true,


                //ItineraryTypeGUID = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7891"),
            };
            var _allitinerary = DbAHD.dataMissionRequestItinerary.Where(x => x.MissionRequestGUID == FK).OrderByDescending(x => x.OrderId).ToList();
            if (_allitinerary.Count > 0)
            {
                model.FromLocationGUID = _allitinerary.FirstOrDefault().ToLocationGUID;
            }

            return PartialView("~/Areas/AHD/Views/MissionRequest/Itinerary/_MissionRequestItineraryUpdateModal.cshtml",
               model);
        }



        public ActionResult MissionRequestItineraryUpdate(Guid PK)
        {
            //if (!CMS.HasAction(Permissions.StaffMissionRequest.Access, Apps.AHD))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            var _mission = DbAHD.dataMissionRequestItinerary.Where(x => x.MissionRequestItineraryGUID == PK).FirstOrDefault();
            MissionRequestItineraryUpdateModel model = DbAHD.dataMissionRequestItinerary.Where(a => a.MissionRequestItineraryGUID == PK).
                Select(a => new MissionRequestItineraryUpdateModel
                {
                    MissionRequestItineraryGUID = a.MissionRequestItineraryGUID,
                    MissionRequestGUID = a.MissionRequestGUID,
                    FromLocationGUID = a.FromLocationGUID,
                    TravelTypeGUID = a.TravelTypeGUID,
                    ReturnDate = a.ReturnDate,
                    IsMissionOwner = _mission.dataMissionRequest.StaffGUID == UserGUID,

                    TravelDate = (DateTime)a.TravelDate,
                    ToLocationGUID = (Guid)a.ToLocationGUID,
                    FromLocationName = a.FromLocationName,
                    ToLocationName = a.ToLocationName,
                    TravelModeGUID = (Guid)a.TravelModeGUID,
                    IsPrivate = (bool)a.IsPrivate,
                    AccommodationProvidedGUID = a.AccommodationProvidedGUID,
                    MealsProvidedGUID = a.MealsProvidedGUID,
                    Comments = a.Comments,
                    Active = a.Active,
                    ItineraryTypeGUID = a.ItineraryTypeGUID


                }
                ).FirstOrDefault();
            return PartialView("~/Areas/AHD/Views/MissionRequest/Itinerary/_MissionRequestItineraryUpdateModal.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MissionRequestItineraryCreate(MissionRequestItineraryUpdateModel model)
        {
            DateTime ExecutionTime = DateTime.Now;
            //if (!CMS.HasAction(Permissions.StaffMissionRequest.Create, Apps.AHD))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            dataMissionRequestItinerary _tag = Mapper.Map(model, new dataMissionRequestItinerary());
            var _checkpri = DbAHD.dataMissionRequestItinerary.Where(x => x.MissionRequestGUID == model.MissionRequestGUID
             ).ToList();
            var _checkPriBound = _checkpri.Where(x => x.ItineraryTypeGUID == Guid.Parse("66CD375C-A576-4AA4-8AF4-FF3C1C5E7891")).ToList();
            var _mission = DbAHD.dataMissionRequest.Where(x => x.MissionRequestGUID == model.MissionRequestGUID).FirstOrDefault();
            //if (_checkPriBound!=null && _checkPriBound.Count > 0)
            //{
            //    var maxdate = _checkPriBound.Select(x => x.TravelDate).Max();
            //    if(model.TravelDate< maxdate)
            //    {
            //        ModelState.AddModelError("Error: ", "Return date should be after the Outbound Travel ");
            //        return PartialView("~/Areas/AHD/Views/MissionRequest/Itinerary/_MissionRequestItineraryUpdateModal.cshtml", model);
            //    }
            //}
            //else if ((_checkPriBound == null ) && model.ItineraryTypeGUID == Guid.Parse("66CD375C-A576-4AA4-8AF4-FF3C1C5E7892"))
            //{
            //    ModelState.AddModelError("Error: ", "please indicate your outbound travel details");
            //    return PartialView("~/Areas/AHD/Views/MissionRequest/Itinerary/_MissionRequestItineraryUpdateModal.cshtml", model);
            //}

            if (!ModelState.IsValid || model.TravelTypeGUID == null || model.TravelDate == null || model.FromLocationGUID == null || model.ToLocationGUID == null || ActiveMissionRequestItinerary(_tag)) return PartialView("~/Areas/AHD/Views/MissionRequest/Itinerary/_MissionRequestItineraryUpdateModal.cshtml", model);


            else if (model.TravelDate == null)
            {
                ModelState.AddModelError("Error: ", "Travel Date date is required ");
                return PartialView("~/Areas/AHD/Views/MissionRequest/Itinerary/_MissionRequestItineraryUpdateModal.cshtml", model);
            }
            else if (model.TravelDate.Value.Date < ExecutionTime.Date)
            {
                ModelState.AddModelError("Error: ", "Travel Date should be Greater and not Bigger  than today ");
                return PartialView("~/Areas/AHD/Views/MissionRequest/Itinerary/_MissionRequestItineraryUpdateModal.cshtml", model);
            }
            else if (model.TravelTypeGUID == null || model.TravelTypeGUID == Guid.Empty)
            {
                ModelState.AddModelError("Error: ", "Travel Type is required ");
                return PartialView("~/Areas/AHD/Views/MissionRequest/Itinerary/_MissionRequestItineraryUpdateModal.cshtml", model);
            }
            else if (model.TravelTypeGUID == null || model.TravelTypeGUID == Guid.Empty)
            {
                ModelState.AddModelError("Error: ", "From Location is required ");
                return PartialView("~/Areas/AHD/Views/MissionRequest/Itinerary/_MissionRequestItineraryUpdateModal.cshtml", model);
            }
            else if (model.ToLocationGUID == null || model.ToLocationGUID == Guid.Empty)
            {
                ModelState.AddModelError("Error: ", "Location To is required ");
                return PartialView("~/Areas/AHD/Views/MissionRequest/Itinerary/_MissionRequestItineraryUpdateModal.cshtml", model);
            }
            //else if (model.TravelModeGUID == null || model.TravelModeGUID == Guid.Empty)
            //{
            //    ModelState.AddModelError("Error: ", "Travel Mode is required ");
            //    return PartialView("~/Areas/AHD/Views/MissionRequest/Itinerary/_MissionRequestItineraryUpdateModal.cshtml", model);
            //}
            //if (model.TravelDate<_mission.DepartureDate)
            //{
            //    ModelState.AddModelError("Error: ", "Travel  date should not be before the event start date ");
            //    PartialView("~/Areas/AHD/Views/MissionRequest/Itinerary/_MissionRequestItineraryUpdateModal.cshtml", model);
            //}
            //else if (model.TravelDate > _mission.ReturnDate)
            //{
            //    ModelState.AddModelError("Error: ", "Travel  date should not be after the official event end date ");
            //    return PartialView("~/Areas/AHD/Views/MissionRequest/Itinerary/_MissionRequestItineraryUpdateModal.cshtml", model);
            //}

            //else if (model.ItineraryTypeGUID==Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7892") && model.TravelDate<_mission.DepartureDate)
            //{
            //    ModelState.AddModelError("Error: ", "Return travel should  be after the start of the event date");
            //    return PartialView("~/Areas/AHD/Views/MissionRequest/Itinerary/_MissionRequestItineraryUpdateModal.cshtml", model);
            //}
            var _allitinerary = DbAHD.dataMissionRequestItinerary.Where(x => x.MissionRequestGUID == model.MissionRequestGUID).OrderByDescending(x => x.OrderId).ToList();
            if (_allitinerary.Count > 0)
            {
                _tag.OrderId = _allitinerary.FirstOrDefault().OrderId + 1;
            }
            else
                _tag.OrderId = 1;

            //_tag.ItineraryTypeGUID=
            _tag.FromLocationName = DbAHD.codeDutyStationsLanguages.Where(x => x.DutyStationGUID == model.FromLocationGUID && x.LanguageID == LAN && x.Active == true).FirstOrDefault().DutyStationDescription;
            _tag.ToLocationName = DbAHD.codeDutyStationsLanguages.Where(x => x.DutyStationGUID == model.ToLocationGUID && x.LanguageID == LAN && x.Active == true).FirstOrDefault().DutyStationDescription;
            //_tag.ItineraryTypeGUID = model.ItineraryTypeGUID;

            DbAHD.CreateNoAudit(_tag);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                @ViewBag.checkIternity = DbAHD.dataMissionRequestItinerary.Where(x => x.MissionRequestGUID == model.MissionRequestGUID).Count();
                return Json(DbAHD.SingleUpdateMessage(DataTableNames.MissionRequestItineraryDataTable, DbAHD.PrimaryKeyControl(_tag), DbAHD.RowVersionControls(Portal.SingleToList(_tag))));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        public ActionResult MissionRequestItineraryOutboundCreate(Guid FK)
        {
            //if (!CMS.HasAction(Permissions.StaffMissionRequest.Create, Apps.AHD))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}

            MissionRequestItineraryUpdateModel model = new MissionRequestItineraryUpdateModel
            {
                MissionRequestGUID = FK,
                IsPrivate = false,
                IsMissionOwner = true,


                ItineraryTypeGUID = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7891"),
            };
            var _allitinerary = DbAHD.dataMissionRequestItinerary.Where(x => x.MissionRequestGUID == FK).OrderByDescending(x => x.OrderId).ToList();
            if (_allitinerary.Count > 0)
            {
                model.FromLocationGUID = _allitinerary.FirstOrDefault().ToLocationGUID;
            }

            return PartialView("~/Areas/AHD/Views/MissionRequest/Itinerary/_MissionRequestItineraryUpdateModal.cshtml",
               model);
        }

        public ActionResult MissionRequestItineraryReturnCreate(Guid FK)
        {
            //if (!CMS.HasAction(Permissions.StaffMissionRequest.Create, Apps.AHD))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}

            var _checkpri = DbAHD.dataMissionRequestItinerary.Where(x => x.MissionRequestGUID == FK
                 ).ToList();

            var _checkPriBound = _checkpri.Where(x => x.ItineraryTypeGUID == Guid.Parse("66CD375C-A576-4AA4-8AF4-FF3C1C5E7891")).ToList();
            //var _mission = DbAHD.dataMissionRequest.Where(x => x.MissionRequestGUID == FK).FirstOrDefault();
            MissionRequestItineraryUpdateModel model = new MissionRequestItineraryUpdateModel
            {
                MissionRequestGUID = FK,
                IsPrivate = false,
                IsMissionOwner = true,
                // StaffGUID = _mission.StaffGUID,
                //IsMissionOwner=_mission.StaffGUID==UserGUID,
                ItineraryTypeGUID = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7892"),
            };

            var _allitinerary = DbAHD.dataMissionRequestItinerary.Where(x => x.MissionRequestGUID == FK).OrderByDescending(x => x.OrderId).ToList();
            if (_allitinerary.Count > 0)
            {
                model.FromLocationGUID = _allitinerary.FirstOrDefault().ToLocationGUID;
            }
            if (_checkPriBound.Count == 0 || (_checkPriBound == null))
            {

                ModelState.AddModelError("Error: ", "please indicate your outbound travel details");
                return PartialView("~/Areas/AHD/Views/MissionRequest/Itinerary/_MissionRequestItineraryUpdateModal.cshtml", model);
            }



            return PartialView("~/Areas/AHD/Views/MissionRequest/Itinerary/_MissionRequestItineraryUpdateModal.cshtml",
               model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MissionRequestItineraryUpdate(MissionRequestItineraryUpdateModel model)
        {
            //if (!CMS.HasAction(Permissions.StaffMissionRequest.Update, Apps.AHD))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            DateTime ExecutionTime = DateTime.Now;
            dataMissionRequestItinerary _tag = Mapper.Map(model, new dataMissionRequestItinerary());
            if (!ModelState.IsValid || ActiveMissionRequestItinerary(_tag)) return PartialView("~/Areas/AHD/Views/MissionRequest/Itinerary/_MissionRequestItineraryUpdateModal.cshtml", model);
            var _mission = DbAHD.dataMissionRequest.Where(x => x.MissionRequestGUID == model.MissionRequestGUID).FirstOrDefault();
            if (model.TravelDate == null)
            {
                ModelState.AddModelError("Error: ", "Travel date date is required ");
                PartialView("~/Areas/AHD/Views/MissionRequest/Itinerary/_MissionRequestItineraryUpdateModal.cshtml", model);
            }
            else if (model.TravelDate.Value.Date < ExecutionTime.Date)
            {
                ModelState.AddModelError("Error: ", "Travel Date should be Greater and not Bigger  than today ");
                return PartialView("~/Areas/AHD/Views/MissionRequest/Itinerary/_MissionRequestItineraryUpdateModal.cshtml", model);
            }

            else if (model.TravelTypeGUID == null || model.TravelTypeGUID == Guid.Empty)
            {
                ModelState.AddModelError("Error: ", "Travel type is required ");
                PartialView("~/Areas/AHD/Views/MissionRequest/Itinerary/_MissionRequestItineraryUpdateModal.cshtml", model);
            }
            else if (model.TravelTypeGUID == null || model.TravelTypeGUID == Guid.Empty)
            {
                ModelState.AddModelError("Error: ", "From location is required ");
                PartialView("~/Areas/AHD/Views/MissionRequest/Itinerary/_MissionRequestItineraryUpdateModal.cshtml", model);
            }
            else if (model.ToLocationGUID == null || model.ToLocationGUID == Guid.Empty)
            {
                ModelState.AddModelError("Error: ", "Location to is required ");
                PartialView("~/Areas/AHD/Views/MissionRequest/Itinerary/_MissionRequestItineraryUpdateModal.cshtml", model);
            }
            //else if (model.TravelModeGUID == null || model.TravelModeGUID == Guid.Empty)
            //{
            //    ModelState.AddModelError("Error: ", "Travel Mode is required ");
            //    PartialView("~/Areas/AHD/Views/MissionRequest/Itinerary/_MissionRequestItineraryUpdateModal.cshtml", model);
            //}

            else if (model.TravelDate > _mission.ReturnDate)
            {
                ModelState.AddModelError("Error: ", "Travel  date should not be after the mission return date ");
                PartialView("~/Areas/AHD/Views/MissionRequest/Itinerary/_MissionRequestItineraryUpdateModal.cshtml", model);
            }
            else if (model.TravelTypeGUID == Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7892") && model.TravelDate < _mission.ReturnDate)
            {
                ModelState.AddModelError("Error: ", "Return travel should  be after the start of the event date");
                return PartialView("~/Areas/AHD/Views/MissionRequest/Itinerary/_MissionRequestItineraryUpdateModal.cshtml", model);
            }

            _tag.FromLocationName = DbAHD.codeDutyStationsLanguages.Where(x => x.DutyStationGUID == model.FromLocationGUID && x.LanguageID == LAN && x.Active == true).FirstOrDefault().DutyStationDescription;
            _tag.ToLocationName = DbAHD.codeDutyStationsLanguages.Where(x => x.DutyStationGUID == model.ToLocationGUID && x.LanguageID == LAN && x.Active == true).FirstOrDefault().DutyStationDescription;
            _tag.ItineraryTypeGUID = model.ItineraryTypeGUID;


            DbAHD.Update(_tag, Permissions.StaffMissionRequest.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                @ViewBag.checkIternity = DbAHD.dataMissionRequestItinerary.Where(x => x.MissionRequestGUID == model.MissionRequestGUID).Count();
                return Json(DbAHD.SingleUpdateMessage(DataTableNames.MissionRequestItineraryDataTable,
                    DbAHD.PrimaryKeyControl(_tag),
                    DbAHD.RowVersionControls(Portal.SingleToList(_tag))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyMissionRequestItinerary(model.MissionRequestItineraryGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MissionRequestItineraryDelete(dataMissionRequestItinerary model)
        {
            //if (!CMS.HasAction(Permissions.StaffMissionRequest.Delete, Apps.AHD))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            List<dataMissionRequestItinerary> DeletedLanguages = DeleteMissionRequestItinerary(new List<dataMissionRequestItinerary> { model });

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.SingleDeleteMessage(DeletedLanguages, DataTableNames.MissionRequestItineraryDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyMissionRequestItinerary(model.MissionRequestItineraryGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MissionRequestItineraryRestore(dataMissionRequestItinerary model)
        {
            //if (!CMS.HasAction(Permissions.StaffMissionRequest.Restore, Apps.AHD))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            if (ActiveMissionRequestItinerary(model))
            {
                return Json(DbAHD.RecordExists());
            }

            List<dataMissionRequestItinerary> RestoredLanguages = RestoreMissionRequestItinerary(Portal.SingleToList(model));

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.SingleRestoreMessage(RestoredLanguages, DataTableNames.MissionRequestItineraryDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyMissionRequestItinerary(model.MissionRequestItineraryGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MissionRequestItineraryDataTableDelete(List<dataMissionRequestItinerary> models)
        {
            //if (!CMS.HasAction(Permissions.StaffMissionRequest.Delete, Apps.AHD))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            List<dataMissionRequestItinerary> DeletedLanguages = DeleteMissionRequestItinerary(models);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.MissionRequestItineraryDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MissionRequestItineraryDataTableRestore(List<dataMissionRequestItinerary> models)
        {
            //if (!CMS.HasAction(Permissions.StaffMissionRequest.Restore, Apps.AHD))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            List<dataMissionRequestItinerary> RestoredLanguages = RestoreMissionRequestItinerary(models);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.MissionRequestItineraryDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        private List<dataMissionRequestItinerary> DeleteMissionRequestItinerary(List<dataMissionRequestItinerary> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataMissionRequestItinerary> DeletedMissionRequestItinerary = new List<dataMissionRequestItinerary>();

            string query = DbAHD.QueryBuilder(models, Permissions.StaffMissionRequest.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbAHD.Database.SqlQuery<dataMissionRequestItinerary>(query).ToList();

            foreach (var language in languages)
            {
                DeletedMissionRequestItinerary.Add(DbAHD.Delete(language, ExecutionTime, Permissions.StaffMissionRequest.DeleteGuid, DbCMS));
            }

            return DeletedMissionRequestItinerary;
        }

        private List<dataMissionRequestItinerary> RestoreMissionRequestItinerary(List<dataMissionRequestItinerary> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataMissionRequestItinerary> RestoredLanguages = new List<dataMissionRequestItinerary>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbAHD.QueryBuilder(models, Permissions.StaffMissionRequest.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbAHD.Database.SqlQuery<dataMissionRequestItinerary>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveMissionRequestItinerary(language))
                {
                    RestoredLanguages.Add(DbAHD.Restore(language, Permissions.StaffMissionRequest.DeleteGuid, Permissions.StaffMissionRequest.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyMissionRequestItinerary(Guid PK)
        {
            dataMissionRequestItinerary dbModel = new dataMissionRequestItinerary();

            var Language = DbAHD.dataMissionRequestItinerary.Where(l => l.MissionRequestItineraryGUID == PK).FirstOrDefault();
            var dbLanguage = DbAHD.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.dataMissionRequestItineraryRowVersion.SequenceEqual(dbModel.dataMissionRequestItineraryRowVersion))
            {
                return Json(DbAHD.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbAHD, dbModel, "LanguagesContainer"));
        }

        private bool ActiveMissionRequestItinerary(dataMissionRequestItinerary model)
        {
            int LanguageID = DbAHD.dataMissionRequestItinerary
                                  .Where(x =>
                                              x.TravelDate == model.TravelDate &&
                                              x.FromLocationGUID == model.FromLocationGUID &&
                                              x.ToLocationGUID == model.ToLocationGUID &&
                                              x.MissionRequestGUID == model.MissionRequestGUID &&
                                              x.Active).Count();
            if (LanguageID > 0)
            {
                //Fix
                ModelState.AddModelError("LanguageID", "Already exits");
            }

            return (LanguageID > 0);
        }

        #endregion Language

        #region Traveler


        //[Route("AHD/MissionRequestTravelerDataTable/{PK}")]
        public ActionResult MissionRequestTravelerDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/AHD/Views/MissionRequest/Traveler/_MissionRequestTravelerDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<MissionRequestTravelerDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<MissionRequestTravelerDataTableModel>(DataTable.Filters);
            }
            Guid _traverlTypeGUID = Guid.Parse("2DAC5D96-E6A3-48C1-B3F5-17BFD9F62966");
            var Result = (
                from a in DbAHD.dataMissionRequestTraveler.AsExpandable().Where(x => x.Active && (x.MissionRequestGUID == PK))

                join b in DbAHD.codeTablesValuesLanguages.AsExpandable().Where(x => x.Active && x.LanguageID == LAN && x.codeTablesValues.TableGUID == _traverlTypeGUID) on a.TraverlerTypeGUID equals b.ValueGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()
                select new MissionRequestTravelerDataTableModel
                {
                    MissionRequestTravelerGUID = a.MissionRequestTravelerGUID.ToString(),
                    MissionRequestGUID = a.MissionRequestGUID.ToString(),
                    TravelerName = a.StaffGUID != null ? a.StaffName : a.FamilyMemberName,
                    FamilyMemberName = a.FamilyMemberName,
                    DateOfBirth = a.DateOfBirth,
                    GenderGUID = a.GenderGUID.ToString(),
                    RelationGUID = a.RelationGUID.ToString(),
                    StaffGUID = a.StaffGUID.ToString(),
                    StaffName = a.StaffName,
                    JobTitle = a.JobTitle,
                    DepartmentGUID = a.DepartmentGUID.ToString(),
                    DutyStationGUID = a.DutyStationGUID.ToString(),
                    Comments = a.Comments,
                    DutyStation = "",
                    Department = "",
                    Active = a.Active,

                    TravelerType = R1.ValueDescription,

                    //Description = x.Description,
                    dataMissionRequestTravelerRowVersion = a.dataMissionRequestTravelerRowVersion

                }).Where(Predicate);



            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult MissionRORequestTravelerDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/AHD/Views/MissionRequest/Traveler/_MissionRequestTravelerDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<MissionRequestTravelerDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<MissionRequestTravelerDataTableModel>(DataTable.Filters);
            }
            Guid _traverlTypeGUID = Guid.Parse("2DAC5D96-E6A3-48C1-B3F5-17BFD9F62966");
            var Result = (
                from a in DbAHD.dataMissionRequestTraveler.AsExpandable().Where(x => x.Active && (x.MissionRequestGUID == PK))

                join b in DbAHD.codeTablesValuesLanguages.AsExpandable().Where(x => x.Active && x.LanguageID == LAN && x.codeTablesValues.TableGUID == _traverlTypeGUID) on a.TraverlerTypeGUID equals b.ValueGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()
                select new MissionRequestTravelerDataTableModel
                {
                    MissionRequestTravelerGUID = a.MissionRequestTravelerGUID.ToString(),
                    MissionRequestGUID = a.MissionRequestGUID.ToString(),
                    TravelerName = a.StaffGUID != null ? a.StaffName : a.FamilyMemberName,
                    FamilyMemberName = a.FamilyMemberName,
                    DateOfBirth = a.DateOfBirth,
                    GenderGUID = a.GenderGUID.ToString(),
                    RelationGUID = a.RelationGUID.ToString(),
                    StaffGUID = a.StaffGUID.ToString(),
                    StaffName = a.StaffName,
                    JobTitle = a.JobTitle,
                    DepartmentGUID = a.DepartmentGUID.ToString(),
                    DutyStationGUID = a.DutyStationGUID.ToString(),
                    Comments = a.Comments,
                    DutyStation = "",
                    Department = "",
                    Active = a.Active,

                    TravelerType = R1.ValueDescription,

                    //Description = x.Description,
                    dataMissionRequestTravelerRowVersion = a.dataMissionRequestTravelerRowVersion

                }).Where(Predicate);



            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }
        //[Route("AHD/TemplateConfiguration/MissionRequestTravelerCreate/")]
        public ActionResult MissionRequestTravelerCreate(Guid FK)
        {
            var _mission = DbAHD.dataMissionRequest.Where(x => x.MissionRequestGUID == FK).FirstOrDefault();
            var _staffcore = DbAHD.StaffCoreData.Where(x => x.UserGUID == _mission.StaffGUID).FirstOrDefault();
            if (_mission.StaffGUID != UserGUID && !CMS.HasAction(Permissions.MissionRequestAdminApproval.Confirm, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            MissionRequestTravelerUpdateModel model = new MissionRequestTravelerUpdateModel
            {
                MissionRequestGUID = FK,
                IsMissionOwner = true,
                MissionOwnerGenderGUID = _staffcore.Gender,
                MissionOwnerDutyStationGUID = _staffcore.DutyStationGUID,
            };

            return PartialView("~/Areas/AHD/Views/MissionRequest/Traveler/_MissionRequestTravelerUpdateModal.cshtml",
               model);
        }

        public ActionResult MissionRequestTravelerUpdate(Guid PK)
        {
            //var _mission = DbAHD.dataMissionRequest.Where(x => x.MissionRequestGUID == FK).FirstOrDefault();
            //if (_mission.StaffGUID != UserGUID && !CMS.HasAction(Permissions.MissionRequestAdminApproval.Confirm, Apps.AHD))
            //{
            //    return Json(DbAHD.PermissionError());
            //}

            var _mission = DbAHD.dataMissionRequestTraveler.Where(x => x.MissionRequestTravelerGUID == PK).FirstOrDefault();

            MissionRequestTravelerUpdateModel model = DbAHD.dataMissionRequestTraveler.Where(a => a.MissionRequestTravelerGUID == PK).
                Select(a => new MissionRequestTravelerUpdateModel
                {
                    MissionRequestTravelerGUID = a.MissionRequestTravelerGUID,
                    MissionRequestGUID = a.MissionRequestGUID,
                    IsMissionOwner = _mission.dataMissionRequest.StaffGUID == UserGUID,


                    TraverlerTypeGUID = a.TraverlerTypeGUID,
                    FamilyMemberName = a.FamilyMemberName,
                    DateOfBirth = a.DateOfBirth,
                    GenderGUID = a.GenderGUID,
                    RelationGUID = a.RelationGUID,

                    StaffGUID = a.StaffGUID,
                    StaffName = a.StaffName,
                    JobTitle = a.JobTitle,
                    DepartmentGUID = a.DepartmentGUID,
                    DutyStationGUID = a.DutyStationGUID,
                    Comments = a.Comments,
                    Active = a.Active


                }
                ).FirstOrDefault();
            return PartialView("~/Areas/AHD/Views/MissionRequest/Traveler/_MissionRequestTravelerUpdateModal.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MissionRequestTravelerCreate(MissionRequestTravelerUpdateModel model)
        {
            //if (!CMS.HasAction(Permissions.StaffMissionRequest.Create, Apps.AHD))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            if (model.TraverlerTypeGUID == null)
                model.TraverlerTypeGUID = Guid.Parse("2DAC5D96-E6A3-48C1-B3F5-17BFD9F62966");
            dataMissionRequestTraveler _traverl = Mapper.Map(model, new dataMissionRequestTraveler());
            if (!ModelState.IsValid) return PartialView("~/Areas/AHD/Views/MissionRequest/Traveler/_MissionRequestTravelerUpdateModal.cshtml", model);
            var checktravellers = DbAHD.dataMissionRequestTraveler.Where(x =>
              x.MissionRequestGUID == model.MissionRequestGUID
              && (model.TraverlerTypeGUID != null && x.TraverlerTypeGUID != model.TraverlerTypeGUID)).ToList();

            var _check = DbAHD.dataMissionRequest.Where(x => x.MissionRequestGUID ==
            model.MissionRequestGUID && x.StaffGUID == model.StaffGUID).FirstOrDefault();
            if (_check != null)
            {
                ModelState.AddModelError("Error: ", "Record already exists ");
                return PartialView("~/Areas/AHD/Views/MissionRequest/Traveler/_MissionRequestTravelerUpdateModal.cshtml", model);

            }

            if (checktravellers.Count > 0)
            {
                ModelState.AddModelError("Error: ", "Not Allowed to include in the same mission Staff with Infant child ");
                return PartialView("~/Areas/AHD/Views/MissionRequest/Traveler/_MissionRequestTravelerUpdateModal.cshtml", model);

            }
            DateTime ExecutionTime = DateTime.Now;
            _traverl.Comments = model.Comments;
            if (model.StaffGUID != null)
            {
                _traverl.StaffName = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == model.StaffGUID).FirstOrDefault().FullName;
                _traverl.JobTitle = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == model.StaffGUID).FirstOrDefault().FullName;
                _traverl.DutyStationGUID = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == model.StaffGUID).FirstOrDefault().DutyStationGUID;
                _traverl.DepartmentGUID = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == model.StaffGUID).FirstOrDefault().DepartmentGUID;
            }



            DbAHD.CreateNoAudit(_traverl);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.SingleUpdateMessage(DataTableNames.MissionRequestTravelerDataTable, DbAHD.PrimaryKeyControl(_traverl), DbAHD.RowVersionControls(Portal.SingleToList(_traverl))));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MissionRequestTravelerUpdate(MissionRequestTravelerUpdateModel model)
        {
            //if (!CMS.HasAction(Permissions.StaffMissionRequest.Update, Apps.AHD))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            dataMissionRequestTraveler _tag = Mapper.Map(model, new dataMissionRequestTraveler());
            if (!ModelState.IsValid ) return PartialView("~/Areas/AHD/Views/MissionRequest/Traveler/_MissionRequestTravelerUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbAHD.Update(_tag, Permissions.StaffMissionRequest.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.SingleUpdateMessage(DataTableNames.MissionRequestTravelerDataTable,
                    DbAHD.PrimaryKeyControl(_tag),
                    DbAHD.RowVersionControls(Portal.SingleToList(_tag))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyMissionRequestTraveler(model.MissionRequestTravelerGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MissionRequestTravelerDelete(dataMissionRequestTraveler model)
        {
            //if (!CMS.HasAction(Permissions.StaffMissionRequest.Delete, Apps.AHD))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            List<dataMissionRequestTraveler> DeletedLanguages = DeleteMissionRequestTraveler(new List<dataMissionRequestTraveler> { model });

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.SingleDeleteMessage(DeletedLanguages, DataTableNames.MissionRequestTravelerDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyMissionRequestTraveler(model.MissionRequestTravelerGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MissionRequestTravelerRestore(dataMissionRequestTraveler model)
        {
            //if (!CMS.HasAction(Permissions.StaffMissionRequest.Restore, Apps.AHD))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            if (ActiveMissionRequestTraveler(model))
            {
                return Json(DbAHD.RecordExists());
            }

            List<dataMissionRequestTraveler> RestoredLanguages = RestoreMissionRequestTraveler(Portal.SingleToList(model));

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.SingleRestoreMessage(RestoredLanguages, DataTableNames.MissionRequestTravelerDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyMissionRequestTraveler(model.MissionRequestTravelerGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MissionRequestTravelerDataTableDelete(List<dataMissionRequestTraveler> models)
        {
            //if (!CMS.HasAction(Permissions.StaffMissionRequest.Delete, Apps.AHD))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            List<dataMissionRequestTraveler> DeletedLanguages = DeleteMissionRequestTraveler(models);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.MissionRequestTravelerDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MissionRequestTravelerDataTableRestore(List<dataMissionRequestTraveler> models)
        {
            //if (!CMS.HasAction(Permissions.StaffMissionRequest.Restore, Apps.AHD))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            List<dataMissionRequestTraveler> RestoredLanguages = RestoreMissionRequestTraveler(models);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.MissionRequestTravelerDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        private List<dataMissionRequestTraveler> DeleteMissionRequestTraveler(List<dataMissionRequestTraveler> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataMissionRequestTraveler> DeletedMissionRequestTraveler = new List<dataMissionRequestTraveler>();

            string query = DbAHD.QueryBuilder(models, Permissions.StaffMissionRequest.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbAHD.Database.SqlQuery<dataMissionRequestTraveler>(query).ToList();

            foreach (var language in languages)
            {
                DeletedMissionRequestTraveler.Add(DbAHD.Delete(language, ExecutionTime, Permissions.StaffMissionRequest.DeleteGuid, DbCMS));
            }

            return DeletedMissionRequestTraveler;
        }

        private List<dataMissionRequestTraveler> RestoreMissionRequestTraveler(List<dataMissionRequestTraveler> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataMissionRequestTraveler> RestoredLanguages = new List<dataMissionRequestTraveler>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbAHD.QueryBuilder(models, Permissions.StaffMissionRequest.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbAHD.Database.SqlQuery<dataMissionRequestTraveler>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveMissionRequestTraveler(language))
                {
                    RestoredLanguages.Add(DbAHD.Restore(language, Permissions.StaffMissionRequest.DeleteGuid, Permissions.StaffMissionRequest.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyMissionRequestTraveler(Guid PK)
        {
            dataMissionRequestTraveler dbModel = new dataMissionRequestTraveler();

            var Language = DbAHD.dataMissionRequestTraveler.Where(l => l.MissionRequestTravelerGUID == PK).FirstOrDefault();
            var dbLanguage = DbAHD.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.dataMissionRequestTravelerRowVersion.SequenceEqual(dbModel.dataMissionRequestTravelerRowVersion))
            {
                return Json(DbAHD.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbAHD, dbModel, "LanguagesContainer"));
        }

        private bool ActiveMissionRequestTraveler(dataMissionRequestTraveler model)
        {
            int LanguageID = DbAHD.dataMissionRequestTraveler
                                  .Where(x =>
                                              x.TraverlerTypeGUID == model.TraverlerTypeGUID &&
                                              (x.FamilyMemberName == model.FamilyMemberName ||
                                              x.StaffGUID == model.StaffGUID) &&
                                              x.TraverlerTypeGUID == model.TraverlerTypeGUID &&
                                              x.MissionRequestGUID == model.MissionRequestGUID &&
                                              x.Active).Count();
            if (LanguageID > 0)
            {
                //Fix
                ModelState.AddModelError("LanguageID", "Already exits");
            }

            return (LanguageID > 0);
        }

        #endregion Language


        #region Mission  Documents



        public ActionResult MissionRequestDocumentDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/AHD/Views/MissionRequest/Documents/_DocumentDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<MissionRequestDocumentDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<MissionRequestDocumentDataTableModel>(DataTable.Filters);
            }

            // Guid myDocumentTypeGUID = Guid.Parse("2d0b59f3-347b-4fa1-8793-a7741d4c35bd");
            Guid _nvGUID = Guid.Parse("29591f83-32a4-49a5-b284-2a8efbc46616");
            var Result = (
                  from a in DbAHD.dataMissionRequestDocument.AsExpandable().Where(x => x.Active && (x.MissionRequestGUID == PK))

                  join b in DbAHD.codeTablesValuesLanguages.AsExpandable().Where(x => x.Active && x.LanguageID == LAN
                  && x.codeTablesValues.TableGUID == _nvGUID) on a.DocumentTypeGUID equals b.ValueGUID into LJ1
                  from R1 in LJ1.DefaultIfEmpty()
                  select new MissionRequestDocumentDataTableModel
                  {
                      MissionRequestDocumentGUID = a.MissionRequestDocumentGUID.ToString(),
                      MissionRequestGUID = a.MissionRequestGUID.ToString(),

                      DocumentTypeGUID = a.DocumentTypeGUID.ToString(),
                      DocumentType = R1.ValueDescription,
                      Comments = a.Comments,

                      Active = a.Active,
                      dataMissionRequestDocumentRowVersion = a.dataMissionRequestDocumentRowVersion

                  }).Where(Predicate);


            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult MissionRORequestDocumentDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/AHD/Views/MissionRequest/Documents/_DocumentDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<MissionRequestDocumentDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<MissionRequestDocumentDataTableModel>(DataTable.Filters);
            }

            // Guid myDocumentTypeGUID = Guid.Parse("2d0b59f3-347b-4fa1-8793-a7741d4c35bd");
            Guid _nvGUID = Guid.Parse("29591f83-32a4-49a5-b284-2a8efbc46616");
            var Result = (
                  from a in DbAHD.dataMissionRequestDocument.AsExpandable().Where(x => x.Active && (x.MissionRequestGUID == PK))

                  join b in DbAHD.codeTablesValuesLanguages.AsExpandable().Where(x => x.Active && x.LanguageID == LAN
                  && x.codeTablesValues.TableGUID == _nvGUID) on a.DocumentTypeGUID equals b.ValueGUID into LJ1
                  from R1 in LJ1.DefaultIfEmpty()
                  select new MissionRequestDocumentDataTableModel
                  {
                      MissionRequestDocumentGUID = a.MissionRequestDocumentGUID.ToString(),
                      MissionRequestGUID = a.MissionRequestGUID.ToString(),

                      DocumentTypeGUID = a.DocumentTypeGUID.ToString(),
                      DocumentType = R1.ValueDescription,
                      Comments = a.Comments,

                      Active = a.Active,
                      dataMissionRequestDocumentRowVersion = a.dataMissionRequestDocumentRowVersion

                  }).Where(Predicate);


            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult MissionRequestDocumentCreate(Guid FK)
        {
            //if (!CMS.HasAction(Permissions.StaffMissionRequest.Create, Apps.AHD))
            //{
            //    return Json(DbAHD.PermissionError());
            //}
            //var _mission = DbAHD.dataMissionRequest.Where(x => x.MissionRequestGUID == FK).FirstOrDefault();

            return PartialView("~/Areas/AHD/Views/MissionRequest/Documents/_DocumentUpdateModal.cshtml",
                new MissionRequestDocumentUpdateModel { MissionRequestGUID = FK, IsMissionOwner = true });
        }

        public ActionResult DownloadStaffMissionDocumentFile(Guid id)
        {

            var model = DbAHD.dataMissionRequestDocument.Where(x => x.MissionRequestDocumentGUID == id).FirstOrDefault();
            var fullPath = model.MissionRequestDocumentGUID + "." + model.DocumentExtension;


            string sourceFile = Server.MapPath("~/Areas/AHD/UploadedDocuments/StaffMissionRequestDocuments/" + fullPath);


            byte[] fileBytes = System.IO.File.ReadAllBytes(sourceFile);

            string fileName = DateTime.Now.ToString("yyMMdd") + fullPath;
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);








            // return Json(DbCMS.SingleUpdateMessage(null, null, null, null, "Please Wait...."));
        }



        //[HttpPost]
        //public FineUploaderResult UploadMissionRequestDocument(FineUpload upload, Guid MissionRequestGUID, Guid DocumentTypeGUID, string comments)
        //{

        //    return new FineUploaderResult(true, new { path = UploadDocument(upload, MissionRequestGUID, DocumentTypeGUID, comments), success = true });
        //}

        [HttpPost]
        public FineUploaderResult UploadMissionRequestDocument(FineUpload upload, Guid MissionRequestGUID, Guid DocumentTypeGUID, string comments)
        {
            string error = "Error ";
            if (FileTypeValidator.IsPDF(upload.InputStream) || FileTypeValidator.IsImage(upload.InputStream) ||
                FileTypeValidator.IsExcel(upload.InputStream) ||
                FileTypeValidator.IsWord(upload.InputStream)
                )
            {
                return new FineUploaderResult(true, new { path = UploadDocument(upload, MissionRequestGUID, DocumentTypeGUID, comments), success = true });
            }

            return new FineUploaderResult((error == "" ? true : false), new { Error = error, path = "", success = (error == "" ? true : false) });
        }

        public string UploadDocument(FineUpload upload, Guid MissionRequestGUID, Guid DocumentTypeGUID, string comments)
        {
            var _stearm = upload.InputStream;
            DateTime ExecutionTime = DateTime.Now;
            //string _ext = Path.GetExtension(upload.FileName).Split('.')[1];
            dataMissionRequestDocument documentUplod = new dataMissionRequestDocument();
            documentUplod.MissionRequestDocumentGUID = Guid.NewGuid();
            //string FilePath = Server.MapPath("~/Areas/AHD/UploadedDocuments/" + documentUplod.ItemIntpuDetailUploadedDocumentGUID + _ext);

            string _ext = Path.GetExtension(upload.FileName).Split('.')[1];

            string FolderPath = Server.MapPath("~/Areas/AHD/UploadedDocuments/StaffMissionRequestDocuments/");
            Directory.CreateDirectory(FolderPath);
            //int LatestFileVersion = 0;
            //try { LatestFileVersion = (from a in DbPPA.PPAFileVersion.Where(x => x.PPAOriginalFileGUID == PPAOriginalFileGUID && x.FileActionByUserGUID == UserGUID) select a.FileVersion).Max(); } catch { }
            //if (LatestFileVersion == -1) LatestFileVersion = 0;



            string FilePath = FolderPath + "/" + documentUplod.MissionRequestDocumentGUID.ToString() + "." + _ext;

            using (var fileStream = System.IO.File.Create(FilePath))
            {
                upload.InputStream.Seek(0, SeekOrigin.Begin);
                upload.InputStream.CopyTo(fileStream);
            }

            documentUplod.MissionRequestGUID = MissionRequestGUID;

            documentUplod.DocumentExtension = _ext;
            documentUplod.DocumentTypeGUID = DocumentTypeGUID;
            documentUplod.Comments = comments;
            documentUplod.CreatedByGUID = UserGUID;
            documentUplod.Createdate = ExecutionTime;


            //documentUplod.Comments = ItemInputDetailGUID;
            //documentUplod.CreatedByGUID = UserGUID;
            //documentUplod.CreatedDate = ExecutionTime;
            DbAHD.CreateNoAudit(documentUplod);
            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }


            //Server.MapPath("~/Areas/AHD/temp/Standard_Operating_Procedure_for_issuing_ICT_equipment_to_staff" + DateTime.Now.ToBinary() + ".pdf");


            return "~/Areas/AHD/UploadedDocuments/StaffMissionRequestDocuments/" + documentUplod.MissionRequestDocumentGUID + ".xlsx";
        }




        public ActionResult MissionRequestDocumentUpdate(Guid PK)
        {
            //if (!CMS.HasAction(Permissions.StaffMissionRequest.Access, Apps.AHD))
            //{
            //    return Json(DbAHD.PermissionError());
            //}
            var _mission = DbAHD.dataMissionRequestDocument.Where(x => x.MissionRequestDocumentGUID == PK).FirstOrDefault();
            MissionRequestDocumentUpdateModel model = DbAHD.dataMissionRequestDocument.Where(x => x.MissionRequestDocumentGUID == PK).Select(f => new MissionRequestDocumentUpdateModel
            {

                DocumentTypeGUID = (Guid)f.DocumentTypeGUID,
                MissionRequestDocumentGUID = (Guid)f.MissionRequestDocumentGUID,

                IsMissionOwner = _mission.dataMissionRequest.StaffGUID == UserGUID,
                CreatedByGUID = f.CreatedByGUID,
                CreateDate = f.Createdate,
                Comments = f.Comments,
                Active = f.Active

            }).FirstOrDefault();
            return PartialView("~/Areas/AHD/Views/MissionRequest/Documents/_DocumentUpdateModal.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MissionRequestDocumentCreate(dataMissionRequestDocument model)
        {
            var _mission = DbAHD.dataMissionRequest.Where(x => x.MissionRequestGUID == model.MissionRequestGUID).FirstOrDefault();
            if (_mission.StaffGUID != UserGUID && !CMS.HasAction(Permissions.MissionRequestAdminApproval.Confirm, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            if (!ModelState.IsValid || (model.DocumentTypeGUID == null)) return PartialView("~/Areas/AHD/Views/MissionRequest/Documents/_DocumentUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbAHD.CreateNoAudit(model);//, Permissions.StaffMissionRequest.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.SingleUpdateMessage(DataTableNames.MissionRequestDocumentDataTable, DbAHD.PrimaryKeyControl(model), DbAHD.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MissionRequestDocumentUpdate(dataMissionRequestDocument model)
        {
            //if (!CMS.HasAction(Permissions.StaffMissionRequest.Update, Apps.AHD))
            //{
            //    return Json(DbAHD.PermissionError());
            //}
            if (!ModelState.IsValid || model.DocumentTypeGUID == null) return PartialView("~/Areas/AHD/Views/MissionRequest/Documents/_DocumentUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbAHD.UpdateNoAudit(model);//, Permissions.StaffMissionRequest.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.SingleUpdateMessage(DataTableNames.MissionRequestDocumentDataTable,
                    DbAHD.PrimaryKeyControl(model),
                    DbAHD.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyMissionRequestDocument(model.MissionRequestDocumentGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MissionRequestDocumentDelete(dataMissionRequestDocument model)
        {
            //if (!CMS.HasAction(Permissions.StaffMissionRequest.Delete, Apps.AHD))
            //{
            //    return Json(DbAHD.PermissionError());
            //}
            List<dataMissionRequestDocument> DeletedLanguages = DeleteMissionRequestDocument(new List<dataMissionRequestDocument> { model });

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.SingleDeleteMessage(DeletedLanguages, DataTableNames.MissionRequestDocumentDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyMissionRequestDocument(model.MissionRequestDocumentGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MissionRequestDocumentRestore(dataMissionRequestDocument model)
        {
            //if (!CMS.HasAction(Permissions.StaffMissionRequest.Restore, Apps.AHD))
            //{
            //    return Json(DbAHD.PermissionError());
            //}
            if (ActiveMissionRequestDocument(model))
            {
                return Json(DbAHD.RecordExists());
            }

            List<dataMissionRequestDocument> RestoredLanguages = RestoreStaffDocument(Portal.SingleToList(model));

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.SingleRestoreMessage(RestoredLanguages, DataTableNames.MissionRequestDocumentDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyMissionRequestDocument(model.MissionRequestDocumentGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MissionRequestDocumentDataTableDelete(List<dataMissionRequestDocument> models)
        {
            //if (!CMS.HasAction(Permissions.StaffMissionRequest.Delete, Apps.AHD))
            //{
            //    return Json(DbAHD.PermissionError());
            //}
            List<dataMissionRequestDocument> DeletedLanguages = DeleteMissionRequestDocument(models);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.MissionRequestDocumentDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MissionRequestDocumentDataTableModelRestore(List<dataMissionRequestDocument> models)
        {
            //if (!CMS.HasAction(Permissions.StaffMissionRequest.Restore, Apps.AHD))
            //{
            //    return Json(DbAHD.PermissionError());
            //}
            List<dataMissionRequestDocument> RestoredLanguages = RestoreStaffDocument(models);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.MissionRequestDocumentDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        private List<dataMissionRequestDocument> DeleteMissionRequestDocument(List<dataMissionRequestDocument> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataMissionRequestDocument> DeletedStaffBankAccount = new List<dataMissionRequestDocument>();

            string query = DbAHD.QueryBuilder(models, Permissions.StaffMissionRequest.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbAHD.Database.SqlQuery<dataMissionRequestDocument>(query).ToList();

            foreach (var language in languages)
            {
                DeletedStaffBankAccount.Add(DbAHD.Delete(language, ExecutionTime, Permissions.StaffMissionRequest.DeleteGuid, DbCMS));
            }

            return DeletedStaffBankAccount;
        }

        private List<dataMissionRequestDocument> RestoreStaffDocument(List<dataMissionRequestDocument> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataMissionRequestDocument> RestoredLanguages = new List<dataMissionRequestDocument>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbAHD.QueryBuilder(models, Permissions.StaffMissionRequest.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbAHD.Database.SqlQuery<dataMissionRequestDocument>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveMissionRequestDocument(language))
                {
                    RestoredLanguages.Add(DbAHD.Restore(language, Permissions.StaffMissionRequest.DeleteGuid, Permissions.StaffMissionRequest.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyMissionRequestDocument(Guid PK)
        {
            dataMissionRequestDocument dbModel = new dataMissionRequestDocument();

            var Language = DbAHD.dataMissionRequestDocument.Where(l => l.MissionRequestDocumentGUID == PK).FirstOrDefault();
            var dbLanguage = DbAHD.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.dataMissionRequestDocumentRowVersion.SequenceEqual(dbModel.dataMissionRequestDocumentRowVersion))
            {
                return Json(DbAHD.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbAHD, dbModel, "LanguagesContainer"));
        }

        private bool ActiveMissionRequestDocument(dataMissionRequestDocument model)
        {
            int LanguageID = DbAHD.dataMissionRequestDocument
                                  .Where(x =>
                                              x.MissionRequestDocumentGUID == model.MissionRequestDocumentGUID &&
                                              x.DocumentTypeGUID == model.DocumentTypeGUID &&

                                              x.Active).Count();
            if (LanguageID > 0)
            {
                //Fix
                ModelState.AddModelError("LanguageID", "Already Exists");
            }

            return (LanguageID > 0);
        }



        #endregion MissionDocuments

        #region Mission Approval

        public ActionResult SaveMissionAsDraft(Guid _MissionRequestGUID)
        {
            var _mission = DbAHD.dataMissionRequest.Where(x => x.MissionRequestGUID == _MissionRequestGUID).FirstOrDefault();
            if (_mission.LastFlowStatusGUID == null || _mission.LastFlowStatusGUID == Guid.Empty)
            {
                DateTime ExecutionTime = DateTime.Now;

                _mission.LastFlowStatusGUID = AHDMissionStatusFlow.Draft;
                _mission.LastFlowStatus = "Draft";




                var toChange = DbAHD.dataMissionRequestFlow.Where(x => x.MissionRequestGUID == _mission.MissionRequestGUID

                    && x.IsLastAction == true).FirstOrDefault();
                toChange.IsLastAction = false;
                dataMissionRequestFlow flow = new dataMissionRequestFlow

                {
                    MissionRequestFlowGUID = Guid.NewGuid(),
                    MissionRequestGUID = _mission.MissionRequestGUID,
                    FlowStatusGUID = AHDMissionStatusFlow.Draft,
                    IsLastAction = true,
                    CreateByGUID = UserGUID,
                    CreateDate = ExecutionTime,
                    OrderId = toChange.OrderId + 1
                };

                DbAHD.UpdateNoAudit(_mission);
                DbAHD.UpdateNoAudit(toChange);
                DbAHD.CreateNoAudit(flow);
            }

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                //check here
                //SubmitMissionRequestEmail(_mission, Guid.Parse(""));
                return Json(new
                {
                    success = 1,


                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }
        public ActionResult CancelMissionRequest(Guid _MissionRequestGUID)
        {
            var _mission = DbAHD.dataMissionRequest.Where(x => x.MissionRequestGUID == _MissionRequestGUID).FirstOrDefault();
            //if (!CMS.HasAction(Permissions.RefugeeScannedDocument.Create, Apps.AHD))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            //if (!ModelState.IsValid || _TypeOfMissionGUID == null || _TypeOfTravelGUID == null)
            //{
            //    return Json(DbAHD.PermissionError());
            //}

            DateTime ExecutionTime = DateTime.Now;
            if (_mission.LastFlowStatusGUID == AHDMissionStatusFlow.Draft || _mission.LastFlowStatusGUID == AHDMissionStatusFlow.Submitted)
            {
                _mission.LastFlowStatusGUID = AHDMissionStatusFlow.Canceled;
                _mission.LastFlowStatus = "Canceled";
                _mission.CanceledByGUID = UserGUID;
                _mission.CaneledDate = DateTime.Now;

            }

            var toChange = DbAHD.dataMissionRequestFlow.Where(x => x.MissionRequestGUID == _mission.MissionRequestGUID

                && x.IsLastAction == true).FirstOrDefault();
            toChange.IsLastAction = false;
            dataMissionRequestFlow flow = new dataMissionRequestFlow

            {
                MissionRequestFlowGUID = Guid.NewGuid(),
                MissionRequestGUID = _mission.MissionRequestGUID,
                FlowStatusGUID = AHDMissionStatusFlow.Canceled,
                IsLastAction = true,
                CreateByGUID = UserGUID,
                CreateDate = ExecutionTime,
                OrderId = toChange.OrderId + 1
            };

            DbAHD.UpdateNoAudit(_mission);
            DbAHD.UpdateNoAudit(toChange);
            DbAHD.CreateNoAudit(flow);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                //check here
                //SubmitMissionRequestEmail(_mission, Guid.Parse(""));
                return Json(new
                {
                    success = 1,


                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        public ActionResult CancelMissionRequestByAdmin(Guid _MissionRequestGUID, string _adminComments)
        {
            if (string.IsNullOrEmpty(_adminComments))
            {
                return Json(DbAHD.PermissionError());
            }

            var _mission = DbAHD.dataMissionRequest.Where(x => x.MissionRequestGUID == _MissionRequestGUID).FirstOrDefault();
            //if (!CMS.HasAction(Permissions.RefugeeScannedDocument.Create, Apps.AHD))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            //if (!ModelState.IsValid || _TypeOfMissionGUID == null || _TypeOfTravelGUID == null)
            //{
            //    return Json(DbAHD.PermissionError());
            //}

            DateTime ExecutionTime = DateTime.Now;
            if (_mission.LastFlowStatusGUID == AHDMissionStatusFlow.Draft || _mission.LastFlowStatusGUID == AHDMissionStatusFlow.Submitted)
            {
                _mission.LastFlowStatusGUID = AHDMissionStatusFlow.Canceled;
                _mission.AdminComments = _adminComments;
                _mission.LastFlowStatus = "Canceled";
                _mission.CanceledByGUID = UserGUID;
                _mission.CaneledDate = DateTime.Now;

            }

            var toChange = DbAHD.dataMissionRequestFlow.Where(x => x.MissionRequestGUID == _mission.MissionRequestGUID

                && x.IsLastAction == true).FirstOrDefault();
            toChange.IsLastAction = false;
            dataMissionRequestFlow flow = new dataMissionRequestFlow

            {
                MissionRequestFlowGUID = Guid.NewGuid(),
                MissionRequestGUID = _mission.MissionRequestGUID,
                FlowStatusGUID = AHDMissionStatusFlow.Canceled,
                IsLastAction = true,
                CreateByGUID = UserGUID,
                CreateDate = ExecutionTime,
                OrderId = toChange.OrderId + 1
            };

            DbAHD.UpdateNoAudit(_mission);
            DbAHD.UpdateNoAudit(toChange);
            DbAHD.CreateNoAudit(flow);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                SubmitMissionRequestEmail(_mission, Guid.Parse("dceb06ff-4844-4084-917b-8dfae457b9ec"), AHDMissionStatusFlow.Canceled, 0);
                //check here
                //SubmitMissionRequestEmail(_mission, Guid.Parse(""));
                return Json(new
                {
                    success = 1,


                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }




        public ActionResult SubmitMissionRequestToAdmin(Guid _MissionRequestGUID)
        {
            var _mission = DbAHD.dataMissionRequest.Where(x => x.MissionRequestGUID == _MissionRequestGUID).FirstOrDefault();
            //if (!CMS.HasAction(Permissions.RefugeeScannedDocument.Create, Apps.AHD))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            //if (!ModelState.IsValid || _TypeOfMissionGUID == null || _TypeOfTravelGUID == null)
            //{
            //    return Json(DbAHD.PermissionError());
            //}

            DateTime ExecutionTime = DateTime.Now;
            if (_mission.LastFlowStatusGUID == AHDMissionStatusFlow.Draft || _mission.LastFlowStatusGUID == null
                || _mission.LastFlowStatusGUID == AHDMissionStatusFlow.Returned)
            {
                _mission.LastFlowStatusGUID = AHDMissionStatusFlow.Submitted;
                _mission.LastFlowStatus = "Submitted";
                var toChange = DbAHD.dataMissionRequestFlow.Where(x => x.MissionRequestGUID == _mission.MissionRequestGUID

              && x.IsLastAction == true).FirstOrDefault();
                toChange.IsLastAction = false;
                dataMissionRequestFlow flow = new dataMissionRequestFlow

                {
                    MissionRequestFlowGUID = Guid.NewGuid(),
                    MissionRequestGUID = _mission.MissionRequestGUID,
                    FlowStatusGUID = AHDMissionStatusFlow.Submitted,
                    IsLastAction = true,
                    CreateByGUID = UserGUID,
                    CreateDate = ExecutionTime,
                    OrderId = toChange.OrderId + 1
                };

                DbAHD.UpdateNoAudit(_mission);
                DbAHD.UpdateNoAudit(toChange);
                DbAHD.CreateNoAudit(flow);

            }



            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                //check here
                //admin confirm permission dceb06ff-4844-4084-917b-8dfae457b9ec
                SubmitMissionRequestEmail(_mission, Guid.Parse("dceb06ff-4844-4084-917b-8dfae457b9ec"), AHDMissionStatusFlow.Submitted, 0);
                return Json(new
                {
                    success = 1,


                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }
        public void SubmitMissionRequestEmail(dataMissionRequest _mission, Guid _permissionGUID, Guid _statusGUID, int type)
        {

            var currStaff = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == _mission.StaffGUID
                                                                              && x.LanguageID == LAN).FirstOrDefault();
            var tempPermGUIDs = DbCMS.userPermissions.Where(x => (x.ActionGUID == _permissionGUID && x.Active == true
                             ) && x.Active).Select(x => x.UserProfileGUID).Distinct().ToList();

            var _userGuids = DbCMS.userProfiles.Where(x => tempPermGUIDs.Contains(x.UserProfileGUID)).Select(x => x.userServiceHistory.UserGUID).ToList();

            var _backupUsers = DbCMS.userServiceHistory.Where(x => _userGuids.Contains(x.UserGUID)).Select(x => x.EmailAddress).Distinct().ToList();


            string copyEmails = string.Join(" ;", _backupUsers);
            var _staff = DbAHD.userServiceHistory.Where(x => x.UserGUID == _mission.StaffGUID).FirstOrDefault();

            //var _currManagerAccount = DbAHD.userServiceHistory.Where(x => x.UserGUID == staffCore.ReportToGUID).ToList();
            string URL = "";
            string Anchor = "";
            string Link = "";
            string _staffName = currStaff.FirstName + " " + currStaff.Surname;
            var _pre = DbAHD.StaffCoreData.Where(x => x.UserGUID == _staff.UserGUID).FirstOrDefault();
            string _staffPrefix = "";
            if (_pre.StaffPrefixGUID == Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E3771"))
            {
                _staffPrefix = "MR.";
            }
            else
            {
                _staffPrefix = "Mrs.";
            }
            string SubjectMessage = resxEmails.MissionRequestByStaffSubject.Replace("$reference", _mission.ReferenceNumber).Replace("$StaffName", _staffName);
            //Staff Submitted done
            if (_statusGUID == AHDMissionStatusFlow.Submitted)
            {
                URL = AppSettingsKeys.Domain + "/AHD/MissionRequest/VerifyMissionRequest/?PK=" + new Portal().GUIDToString(_mission.MissionRequestGUID);
                Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ClickHere + "</a>";
                Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";

                string _message = resxEmails.MissionRequestAdminCheckForConfirmation
                    //.Replace("$FullName", _financeFirstName + " " + _financeSurName)
                    .Replace("$VerifyLink", Anchor)
                    .Replace("$StaffName", _staffName)

                    .Replace("$prefix", _staffPrefix)
                    .Replace("$reference", _mission.ReferenceNumber);

                if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                int isRec = 1;
                //var myEmail = currAccount.Select(x => x.EmailAddress).FirstOrDefault();
                string copy_recipients = _staff.EmailAddress;
                Send(copyEmails, SubjectMessage, _message, isRec, null);

                //Message for staff 
                string _message_extra = resxEmails.MissionRequestForStaffAfterSubmission
                  //.Replace("$FullName", _financeFirstName + " " + _financeSurName)
                  .Replace("$VerifyLink", Anchor)
                  .Replace("$StaffName", _staffName)

                  .Replace("$prefix", _staffPrefix)
                  .Replace("$reference", _mission.ReferenceNumber);

                if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }

                //var myEmail = currAccount.Select(x => x.EmailAddress).FirstOrDefault();

                Send(_staff.EmailAddress, SubjectMessage, _message_extra, isRec, null);
            }
            //admin approval done
            else if (_statusGUID == AHDMissionStatusFlow.Verified)
            {
                URL = AppSettingsKeys.Domain + "/AHD/MissionRequest/VerifyMissionRequest/?PK=" + new Portal().GUIDToString(_mission.MissionRequestGUID);
                Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ClickHere + "</a>";
                Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
                var _headOfunit = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == _mission.HeadOfUnitApprovedByGUID).FirstOrDefault();
                string _message = resxEmails.MissionRequestFromAdminToHeadOfUnit

                    //.Replace("$FullName", _financeFirstName + " " + _financeSurName)
                    .Replace("$VerifyLink", Anchor)
                    .Replace("$StaffName", _staffName)
                    .Replace("$prefix", _staffPrefix)
                    .Replace("$HeadOfOffice", _headOfunit.FullName)
                    .Replace("$reference", _mission.ReferenceNumber);

                if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                int isRec = 1;
                //var myEmail = currAccount.Select(x => x.EmailAddress).FirstOrDefault();
                string staffEmail = _staff.EmailAddress;
                string copyAllEmails = string.Join(";", copyEmails);
                Send(_headOfunit.EmailAddress, SubjectMessage, _message, isRec, copyAllEmails);


                //Message for staff 
                string _message_extra = resxEmails.MissionRequestVerifiedByAdminToSMCCAdminTeam
                  //.Replace("$FullName", _financeFirstName + " " + _financeSurName)
                  .Replace("$VerifyLink", Anchor)
                  .Replace("$StaffName", _staffName)

                  .Replace("$prefix", _staffPrefix)
                  .Replace("$reference", _mission.ReferenceNumber);

                if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }

                //var myEmail = currAccount.Select(x => x.EmailAddress).FirstOrDefault();

                Send(_staff.EmailAddress, SubjectMessage, _message_extra, isRec, copyEmails);
            }
            //canceled by Admin
            else if (_statusGUID == AHDMissionStatusFlow.Canceled)
            {

                URL = AppSettingsKeys.Domain + "/AHD/MissionRequest/StaffMissionRequestUpdate/?PK=" + new Portal().GUIDToString(_mission.MissionRequestGUID);
                Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ClickHere + "</a>";
                Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";

                string _message = resxEmails.MissionRequestCanceledMissionByAdmin
                    //.Replace("$FullName", _financeFirstName + " " + _financeSurName)
                    .Replace("$VerifyLink", Anchor)
                    .Replace("$StaffName", _staffName)
                    .Replace("$adminComment", _mission.AdminComments)
                    //.Replace("$prefix", _staffPrefix)
                    .Replace("$reference", _mission.ReferenceNumber);

                if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                int isRec = 1;
                //var myEmail = currAccount.Select(x => x.EmailAddress).FirstOrDefault();
                //string copy_recipients = _staff.EmailAddress;
                Send(_staff.EmailAddress, SubjectMessage, _message, isRec, copyEmails);
            }

            //admin returnd 
            else if (_statusGUID == AHDMissionStatusFlow.Returned && type == 1)
            {

                URL = AppSettingsKeys.Domain + "/AHD/MissionRequest/StaffMissionRequestUpdate/?PK=" + new Portal().GUIDToString(_mission.MissionRequestGUID);
                Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ClickHere + "</a>";
                Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";

                string _message = resxEmails.MissionRequestReturnToStaffToReConfirmation
                    //.Replace("$FullName", _financeFirstName + " " + _financeSurName)
                    .Replace("$VerifyLink", Anchor)
                    .Replace("$StaffName", _staffName)
                            .Replace("$adminComment", _mission.AdminComments)
                    //.Replace("$prefix", _staffPrefix)
                    .Replace("$reference", _mission.ReferenceNumber);

                if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                int isRec = 1;
                //var myEmail = currAccount.Select(x => x.EmailAddress).FirstOrDefault();
                //string copy_recipients = _staff.EmailAddress;
                Send(_staff.EmailAddress, SubjectMessage, _message, isRec, copyEmails);
            }
            //HoO/U approved 
            else if (_statusGUID == AHDMissionStatusFlow.Reviewed)
            {
                URL = AppSettingsKeys.Domain + "/AHD/MissionRequest/VerifyMissionRequest/?PK=" + new Portal().GUIDToString(_mission.MissionRequestGUID);
                Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ClickHere + "</a>";
                Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
                //if () { }
                var _Rep = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == _mission.AuthByDeputyRepresentativeGUID).FirstOrDefault();
                var _headoffice = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == _mission.HeadOfUnitApprovedByGUID).FirstOrDefault();
                string _message = resxEmails.MissionRequestHeadOfOfficeForConfirmation
                    //.Replace("$FullName", _financeFirstName + " " + _financeSurName)
                    .Replace("$VerifyLink", Anchor)
                    .Replace("$StaffName", _staffName)
                    .Replace("$prefix", _staffPrefix)
                    .Replace("$RepName", _Rep.FullName)
                    .Replace(" $headOffice", _headoffice.FullName)


                    .Replace("$reference", _mission.ReferenceNumber);

                if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                int isRec = 1;
                var myHeadOfOffice = _headoffice.EmailAddress;
                string staffEmail = _staff.EmailAddress;
                string copyAllEmails = string.Join(";", myHeadOfOffice);
                Send(_Rep.EmailAddress, SubjectMessage, _message, isRec, copyAllEmails);

                //Message for staff 
                string _message_extra = resxEmails.MissionRequestInformStaffAfterHeadOfficeApproval
                  //.Replace("$FullName", _financeFirstName + " " + _financeSurName)
                  .Replace("$VerifyLink", Anchor)
                  .Replace("$StaffName", _staffName)
                    .Replace(" $headOffice", _headoffice.FullName)
                  .Replace("$prefix", _staffPrefix)
                  .Replace("$reference", _mission.ReferenceNumber);

                if (LAN == "AR") { _message_extra = "<p align='right'>" + _message_extra + "</p>"; }

                //var myEmail = currAccount.Select(x => x.EmailAddress).FirstOrDefault();

                Send(_staff.EmailAddress, SubjectMessage, _message_extra, isRec, _headoffice.EmailAddress);
            }
            //head of unit returned 
            else if (_statusGUID == AHDMissionStatusFlow.Returned && type == 2)
            {

                URL = AppSettingsKeys.Domain + "/AHD/MissionRequest/StaffMissionRequestUpdate/?PK=" + new Portal().GUIDToString(_mission.MissionRequestGUID);
                Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ClickHere + "</a>";
                Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
                var _headOfunit = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == _mission.HeadOfUnitApprovedByGUID).FirstOrDefault();
                string _message = resxEmails.MissionRequestReturnToStaffByHeadOfOffice
                    //.Replace("$FullName", _financeFirstName + " " + _financeSurName)
                    .Replace("$VerifyLink", Anchor)
                    .Replace("$StaffName", _staffName)
                    .Replace("$headofoffice", _headOfunit.FullName)
                    .Replace("$officecomment", _mission.HeadOfUnitComments)
                    //.Replace("$prefix", _staffPrefix)
                    .Replace("$reference", _mission.ReferenceNumber);

                if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                int isRec = 1;
                //var myEmail = currAccount.Select(x => x.EmailAddress).FirstOrDefault();
                //string copy_recipients = _staff.EmailAddress;
                // string copyAllEmails = string.Join(_headOfunit.EmailAddress, copyEmails);
                Send(_staff.EmailAddress, SubjectMessage, _message, isRec, _headOfunit.EmailAddress);
            }



            //HoO/U Rejected
            else if (_statusGUID == AHDMissionStatusFlow.Rejected && type == 2)
            {
                URL = AppSettingsKeys.Domain + "/AHD/MissionRequest/VerifyMissionRequest/?PK=" + new Portal().GUIDToString(_mission.MissionRequestGUID);
                Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ClickHere + "</a>";
                Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
                //if () { }
                // var _Rep = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == _mission.AuthByDeputyRepresentativeGUID).FirstOrDefault();
                string _message = resxEmails.MissionRequestRejectedMissionByHeadOfOffice
                    //.Replace("$FullName", _financeFirstName + " " + _financeSurName)
                    .Replace("$VerifyLink", Anchor)
                    .Replace("$StaffName", _staffName)
                    .Replace("$prefix", _staffPrefix)
                      .Replace("$officecomment", _mission.HeadOfUnitComments)

                    .Replace("$reference", _mission.ReferenceNumber);

                if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                int isRec = 1;
                var myHeadOfOffice = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == _mission.HeadOfUnitApprovedByGUID).Select(x => x.EmailAddress).FirstOrDefault();
                string staffEmail = _staff.EmailAddress;
                //string copyAllEmails = string.Join( copyEmails, myHeadOfOffice);
                Send(_staff.EmailAddress, SubjectMessage, _message, isRec, myHeadOfOffice);
            }
            //Rejected by OIC
            //OIC approved
            else if (_statusGUID == AHDMissionStatusFlow.Approved)
            {
                URL = AppSettingsKeys.Domain + "/AHD/MissionRequest/VerifyMissionRequest/?PK=" + new Portal().GUIDToString(_mission.MissionRequestGUID);
                Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ClickHere + "</a>";
                Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
                //if () { }
                var _Rep = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == _mission.AuthByDeputyRepresentativeGUID).FirstOrDefault();
                string _message = resxEmails.MissionRequestApprovedByOIC
                    //.Replace("$FullName", _financeFirstName + " " + _financeSurName)
                    .Replace("$VerifyLink", Anchor)
                    .Replace("$StaffName", _staffName)
                    //.Replace("$prefix", _staffPrefix)
                    //.Replace("$HeadOfOffice", _Rep.FullName)
                    .Replace("$reference", _mission.ReferenceNumber);

                if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                int isRec = 1;
                var myHeadOfOffice = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == _mission.HeadOfUnitApprovedByGUID).Select(x => x.EmailAddress).FirstOrDefault();
                string staffEmail = _staff.EmailAddress;
                string copyAllEmails = string.Join(_Rep.EmailAddress, myHeadOfOffice);
                Send(_staff.EmailAddress, SubjectMessage, _message, isRec, copyAllEmails);

                string _message_extra = resxEmails.MissionRequestInformAdminByRepApproval
                    //.Replace("$FullName", _financeFirstName + " " + _financeSurName)
                    .Replace("$VerifyLink", Anchor)
                    .Replace("$StaffName", _staffName)

                    .Replace("$prefix", _staffPrefix)
            .Replace("$reference", _mission.ReferenceNumber);

                if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }

                //var myEmail = currAccount.Select(x => x.EmailAddress).FirstOrDefault();

                Send(copyEmails, SubjectMessage, _message_extra, isRec, null);
            }
            else if (_statusGUID == AHDMissionStatusFlow.Rejected && type == 3)
            {
                URL = AppSettingsKeys.Domain + "/AHD/MissionRequest/VerifyMissionRequest/?PK=" + new Portal().GUIDToString(_mission.MissionRequestGUID);
                Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ClickHere + "</a>";
                Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
                //if () { }
                var _Rep = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == _mission.AuthByDeputyRepresentativeGUID).FirstOrDefault();
                string _message = resxEmails.MissionRequestRejectedMissionByOIC
                    //.Replace("$FullName", _financeFirstName + " " + _financeSurName)
                    .Replace("$VerifyLink", Anchor)
                    .Replace("$StaffName", _staffName)
                    .Replace("$OICName", _Rep.FullName)
                    .Replace("$repComments", _mission.AuthorizedComment)

                    .Replace("$reference", _mission.ReferenceNumber);

                if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                int isRec = 1;
                var myHeadOfOffice = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == _mission.HeadOfUnitApprovedByGUID).Select(x => x.EmailAddress).FirstOrDefault();
                string staffEmail = _staff.EmailAddress;
                string copyAllEmails = string.Join(copyEmails, myHeadOfOffice, _Rep.EmailAddress);
                Send(_staff.EmailAddress, SubjectMessage, _message, isRec, copyAllEmails);
            }



        }
        public ActionResult VerifyMissionRequest(Guid PK)
        {

            //if (!CMS.HasAction(Permissions.InternationalStaffEntitlements.Access, Apps.AHD))
            //{
            //    return Json(DbAHD.PermissionError());
            //}


            var _mission = DbAHD.dataMissionRequest.Where(x => x.MissionRequestGUID == PK).FirstOrDefault();
            StaffMissionRequestDataTableModel model = Mapper.Map(_mission, new StaffMissionRequestDataTableModel());
            model.CurrentUserGUID = UserGUID;
            model.TypeOfMission = DbAHD.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.ValueGUID == _mission.TypeOfMissionGUID).FirstOrDefault().ValueDescription;
            model.TypeOfTravel = DbAHD.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.ValueGUID == _mission.TypeOfTravelGUID).FirstOrDefault().ValueDescription;
            if (_mission.HeadOfUnitApprovedByGUID != null || _mission.HeadOfUnitApprovedByGUID == Guid.Empty)
            {
                model.HeadOfUnitApprovedName = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == model.HeadOfUnitApprovedByGUID).FirstOrDefault().FullName;
            }
            if (_mission.AuthByDeputyRepresentativeGUID != null || _mission.AuthByDeputyRepresentativeGUID == Guid.Empty)
            {
                model.AuthByDeputyRepresentativeName = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == model.AuthByDeputyRepresentativeGUID).FirstOrDefault().FullName;

            }
            if (_mission.AdminApprovedByGUID != null || _mission.AdminApprovedByGUID == Guid.Empty)
            {
                model.AdminApprovedBy = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == model.AdminApprovedByGUID).FirstOrDefault().FullName;

            }

            model.RequestStage = _mission.LastFlowStatusGUID == AHDMissionStatusFlow.Submitted ? 2 :
                       (_mission.LastFlowStatusGUID == AHDMissionStatusFlow.Verified ? 3 :
                       (_mission.LastFlowStatusGUID == AHDMissionStatusFlow.Reviewed ? 4 :
                       (_mission.LastFlowStatusGUID == AHDMissionStatusFlow.Approved ? 5 : 0
                       ))
                       );
            ViewBag.Stage = model.RequestStage;

            ViewBag.StaffName = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == model.StaffGUID).FirstOrDefault().FullName;
            if (_mission.StaffGUID == UserGUID)
            {
                return View("~/Areas/AHD/Views/MissionRequest/StaffMissions/EditStaffMissionRequestForm.cshtml", model);
            }
            if (_mission.LastFlowStatusGUID == AHDMissionStatusFlow.Submitted)
            {
                if (!CMS.HasAction(Permissions.MissionRequestAdminApproval.Confirm, Apps.AHD))
                {
                    return Json(DbAHD.PermissionError());
                }

                return View("~/Areas/AHD/Views/MissionRequest/MissionReview/ReviewStaffMissionRequestForm.cshtml", model);
            }
            else if (_mission.LastFlowStatusGUID == AHDMissionStatusFlow.Verified)
            {
                if (_mission.HeadOfUnitApprovedByGUID != UserGUID)
                {
                    return Json(DbAHD.PermissionError());
                }

                return View("~/Areas/AHD/Views/MissionRequest/MissionReview/HeadOfficeReviewStaffMissionRequestForm.cshtml", model);
            }
            else if (_mission.LastFlowStatusGUID == AHDMissionStatusFlow.Reviewed)
            {
                if (_mission.AuthByDeputyRepresentativeGUID != UserGUID && _mission.AuthorizedByGUID != UserGUID)
                {
                    return Json(DbAHD.PermissionError());
                }
                //return Json(DbAHD.PermissionError());
                return View("~/Areas/AHD/Views/MissionRequest/MissionReview/RepReviewStaffMissionRequestForm.cshtml", model);
            }
            //model.MissionRequestGUID = PK;
            //model.StaffGUID = results.StaffGUID;
            //model.StaffName = results.StaffName;
            //model.PeriodName = results.dataAHDPeriodEntitlement.MonthName;
            //model.FlowStatusGUID = results.FlowStatusGUID;
            //model.LastFlowStatusName = results.LastFlowStatusName;

            return Json(DbAHD.PermissionError());
            //return View("~/Areas/AHD/Views/MissionRequest/MissionReview/ReviewStaffMissionRequestForm.cshtml", model);
        }

        public void Send(string recipients, string subject, string body, int? isRec, string copy_recipients)
        {
            //string copy_recipients = "";
            string blind_copy_recipients = null;
            string body_format = "HTML";
            string importance = "Normal";
            string file_attachments = null;
            string _body = "<div style='font-family:Arial;'>" + body.Replace("\r\n", "<br/>") + "</div>";
            if (LAN == "AR") { _body = "<p align='right'>" + _body + "</p>"; }
            //DbAHD.SendEmailHR("maksoud@unhcr.org", "", blind_copy_recipients, subject, _body, body_format, importance, file_attachments);
             DbCMS.SendEmailHR(recipients, copy_recipients, blind_copy_recipients, subject, _body, body_format, importance, file_attachments);
        }

        #endregion

        #region Work Flow 
        #region Admin Action
        public ActionResult ConfirmMissionRequestByAdmin(Guid _MissionRequestGUID, Guid? _HeadOfUnitApprovedByGUID,

                                                   Guid? _AuthByDeputyRepresentativeGUID,
                                                   //Guid? _AuthByRepresentativeGUID,
                                                   string _AdminComments)
        {
            var _mission = DbAHD.dataMissionRequest.Where(x => x.MissionRequestGUID == _MissionRequestGUID).FirstOrDefault();
            if (!CMS.HasAction(Permissions.MissionRequestAdminApproval.Confirm, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            //if (!ModelState.IsValid || _TypeOfMissionGUID == null || _TypeOfTravelGUID == null)
            //{
            //    return Json(DbAHD.PermissionError());
            //}

            DateTime ExecutionTime = DateTime.Now;
            if (_mission.LastFlowStatusGUID == AHDMissionStatusFlow.Submitted)
            {
                _mission.LastFlowStatusGUID = AHDMissionStatusFlow.Verified;
                _mission.LastFlowStatus = "Verified";

            }
            _mission.AdminApprovedByGUID = UserGUID;
            _mission.AdminApprovedDate = ExecutionTime;
            _mission.HeadOfUnitApprovedByGUID = _HeadOfUnitApprovedByGUID;
            _mission.AuthByDeputyRepresentativeGUID = _AuthByDeputyRepresentativeGUID;
            //_mission.AuthByRepresentativeGUID = _AuthByRepresentativeGUID;
            _mission.AdminComments = _AdminComments;

            var toChange = DbAHD.dataMissionRequestFlow.Where(x => x.MissionRequestGUID == _mission.MissionRequestGUID

                && x.IsLastAction == true).FirstOrDefault();
            toChange.IsLastAction = false;
            dataMissionRequestFlow flow = new dataMissionRequestFlow

            {
                MissionRequestFlowGUID = Guid.NewGuid(),
                MissionRequestGUID = _mission.MissionRequestGUID,
                FlowStatusGUID = AHDMissionStatusFlow.Verified,
                IsLastAction = true,
                CreateByGUID = UserGUID,
                CreateDate = ExecutionTime,
                OrderId = toChange.OrderId + 1
            };

            DbAHD.UpdateNoAudit(_mission);
            DbAHD.UpdateNoAudit(toChange);
            DbAHD.CreateNoAudit(flow);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                //check here
                //admin confirm permission dceb06ff-4844-4084-917b-8dfae457b9ec

                SubmitMissionRequestEmail(_mission, Guid.Parse("dceb06ff-4844-4084-917b-8dfae457b9ec"), AHDMissionStatusFlow.Verified, 0);

                return Json(new
                {
                    success = 1,


                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }
        public ActionResult ReturnMissionRequestByAdmin(Guid _MissionRequestGUID, string _adminComments)
        {
            DateTime ExecutionTime = DateTime.Now;
            var _mission = DbAHD.dataMissionRequest.Where(x => x.MissionRequestGUID == _MissionRequestGUID).FirstOrDefault();
            if (!CMS.HasAction(Permissions.MissionRequestAdminApproval.Access, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (string.IsNullOrEmpty(_adminComments))
            {
                return Json(DbAHD.PermissionError());
            }
            if (!ModelState.IsValid || string.IsNullOrEmpty(_adminComments))
            {
                return Json(DbAHD.PermissionError());
            }

            _mission.AdminComments = _adminComments;
            //_mission.Comments = _Comments;

            _mission.LastFlowStatusGUID = AHDMissionStatusFlow.Draft;
            var _staff = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            _mission.DutyStationGUID = _staff.DutyStationGUID;
            _mission.DepartmentGUID = _staff.DepartmentGUID;
            var toChange = DbAHD.dataMissionRequestFlow.Where(x => x.MissionRequestGUID == _mission.MissionRequestGUID
                             && x.IsLastAction == true).FirstOrDefault();
            toChange.IsLastAction = false;

            dataMissionRequestFlow flow = new dataMissionRequestFlow

            {
                MissionRequestFlowGUID = Guid.NewGuid(),
                MissionRequestGUID = _mission.MissionRequestGUID,
                FlowStatusGUID = AHDMissionStatusFlow.Returned,
                IsLastAction = true,
                CreateByGUID = UserGUID,
                CreateDate = ExecutionTime,
                OrderId = toChange.OrderId + 1
            };

            DbAHD.CreateNoAudit(flow);
            DbAHD.UpdateNoAudit(_mission);
            DbAHD.UpdateNoAudit(toChange);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                SubmitMissionRequestEmail(_mission, Guid.Parse("29591f83-32a4-49a5-b284-2a8efbc44388"), AHDMissionStatusFlow.Returned, 1);
                return Json(new
                {
                    success = 1,


                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }




        #endregion

        #region HoO/U
        public ActionResult ConfirmMissionRequestByHeadOffice(Guid _MissionRequestGUID, string _HeadOfUnitComments)
        {
            var _mission = DbAHD.dataMissionRequest.Where(x => x.MissionRequestGUID == _MissionRequestGUID).FirstOrDefault();
            if (_mission.LastFlowStatusGUID != AHDMissionStatusFlow.Verified || _mission.HeadOfUnitApprovedByGUID != UserGUID)
            {
                throw new HttpException(401, "Unauthorized access");
            }
            //if (!ModelState.IsValid || _TypeOfMissionGUID == null || _TypeOfTravelGUID == null)
            //{
            //    return Json(DbAHD.PermissionError());
            //}

            DateTime ExecutionTime = DateTime.Now;
            if (_mission.LastFlowStatusGUID == AHDMissionStatusFlow.Verified)
            {
                _mission.LastFlowStatusGUID = AHDMissionStatusFlow.Reviewed;
                _mission.LastFlowStatus = "Reviewed";

            }

            _mission.HeadOfUnitComments = _HeadOfUnitComments;
            _mission.HeadOfUnitApprovedDate = ExecutionTime;
            _mission.HeadOfUnitApprovedByGUID = UserGUID;

            var toChange = DbAHD.dataMissionRequestFlow.Where(x => x.MissionRequestGUID == _mission.MissionRequestGUID

                                 && x.IsLastAction == true).FirstOrDefault();
            toChange.IsLastAction = false;
            dataMissionRequestFlow flow = new dataMissionRequestFlow

            {
                MissionRequestFlowGUID = Guid.NewGuid(),
                MissionRequestGUID = _mission.MissionRequestGUID,
                FlowStatusGUID = AHDMissionStatusFlow.Reviewed,
                IsLastAction = true,
                CreateByGUID = UserGUID,
                CreateDate = ExecutionTime,
                OrderId = toChange.OrderId + 1
            };

            DbAHD.UpdateNoAudit(_mission);
            DbAHD.UpdateNoAudit(toChange);
            DbAHD.CreateNoAudit(flow);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                //check here
                //admin confirm permission dceb06ff-4844-4084-917b-8dfae457b9ec

                SubmitMissionRequestEmail(_mission, Guid.Parse("dceb06ff-4844-4084-917b-8dfae457b9ec"), AHDMissionStatusFlow.Reviewed, 0);
                return Json(new
                {
                    success = 1,


                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        public ActionResult RejectMissionRequestByHeadOffice(Guid _MissionRequestGUID, string _HeadOfUnitComments)
        {
            if (string.IsNullOrEmpty(_HeadOfUnitComments))
            {
                return Json(DbAHD.PermissionError());
            }
            var _mission = DbAHD.dataMissionRequest.Where(x => x.MissionRequestGUID == _MissionRequestGUID).FirstOrDefault();
            if (_mission.LastFlowStatusGUID != AHDMissionStatusFlow.Verified || _mission.HeadOfUnitApprovedByGUID != UserGUID)
            {
                throw new HttpException(401, "Unauthorized access");
            }
            //if (!ModelState.IsValid || _TypeOfMissionGUID == null || _TypeOfTravelGUID == null)
            //{
            //    return Json(DbAHD.PermissionError());
            //}

            DateTime ExecutionTime = DateTime.Now;
            if (_mission.LastFlowStatusGUID == AHDMissionStatusFlow.Verified)
            {
                _mission.LastFlowStatusGUID = AHDMissionStatusFlow.Rejected;
                _mission.LastFlowStatus = "Rejected";


            }

            _mission.HeadOfUnitComments = _HeadOfUnitComments;


            var toChange = DbAHD.dataMissionRequestFlow.Where(x => x.MissionRequestGUID == _mission.MissionRequestGUID

                                 && x.IsLastAction == true).FirstOrDefault();
            toChange.IsLastAction = false;
            dataMissionRequestFlow flow = new dataMissionRequestFlow

            {
                MissionRequestFlowGUID = Guid.NewGuid(),
                MissionRequestGUID = _mission.MissionRequestGUID,
                FlowStatusGUID = AHDMissionStatusFlow.Rejected,
                IsLastAction = true,
                CreateByGUID = UserGUID,
                CreateDate = ExecutionTime,
                OrderId = toChange.OrderId + 1
            };

            DbAHD.UpdateNoAudit(_mission);
            DbAHD.UpdateNoAudit(toChange);
            DbAHD.CreateNoAudit(flow);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                //check here
                //admin confirm permission dceb06ff-4844-4084-917b-8dfae457b9ec

                SubmitMissionRequestEmail(_mission, Guid.Parse("dceb06ff-4844-4084-917b-8dfae457b9ec"), AHDMissionStatusFlow.Rejected, 2);
                return Json(new
                {
                    success = 1,


                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        public ActionResult ReturnMissionRequestToStaffByHeadOfUnit(Guid _MissionRequestGUID, string _HeadOfUnitComments)
        {
            if (string.IsNullOrEmpty(_HeadOfUnitComments))
            {
                return Json(DbAHD.PermissionError());
            }
            var _mission = DbAHD.dataMissionRequest.Where(x => x.MissionRequestGUID == _MissionRequestGUID).FirstOrDefault();
            if (_mission.LastFlowStatusGUID != AHDMissionStatusFlow.Verified || _mission.HeadOfUnitApprovedByGUID != UserGUID)
            {
                throw new HttpException(401, "Unauthorized access");
            }
            //if (!ModelState.IsValid || _TypeOfMissionGUID == null || _TypeOfTravelGUID == null)
            //{
            //    return Json(DbAHD.PermissionError());
            //}

            DateTime ExecutionTime = DateTime.Now;
            if (_mission.LastFlowStatusGUID == AHDMissionStatusFlow.Verified)
            {
                _mission.LastFlowStatusGUID = AHDMissionStatusFlow.Returned;
                _mission.LastFlowStatus = "Returned";

            }

            _mission.HeadOfUnitComments = _HeadOfUnitComments;


            var toChange = DbAHD.dataMissionRequestFlow.Where(x => x.MissionRequestGUID == _mission.MissionRequestGUID

                                 && x.IsLastAction == true).FirstOrDefault();
            toChange.IsLastAction = false;
            dataMissionRequestFlow flow = new dataMissionRequestFlow

            {
                MissionRequestFlowGUID = Guid.NewGuid(),
                MissionRequestGUID = _mission.MissionRequestGUID,
                FlowStatusGUID = AHDMissionStatusFlow.Returned,
                IsLastAction = true,
                CreateByGUID = UserGUID,
                CreateDate = ExecutionTime,
                OrderId = toChange.OrderId + 1
            };

            DbAHD.UpdateNoAudit(_mission);
            DbAHD.UpdateNoAudit(toChange);
            DbAHD.CreateNoAudit(flow);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                //check here
                //admin confirm permission dceb06ff-4844-4084-917b-8dfae457b9ec

                SubmitMissionRequestEmail(_mission, Guid.Parse("dceb06ff-4844-4084-917b-8dfae457b9ec"), AHDMissionStatusFlow.Returned, 2);
                return Json(new
                {
                    success = 1,


                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }
        #endregion

        #region OIC
        public ActionResult ConfirmMissionRequestByOIC(Guid _MissionRequestGUID, string _AuthorizedComment)
        {
            var _mission = DbAHD.dataMissionRequest.Where(x => x.MissionRequestGUID == _MissionRequestGUID).FirstOrDefault();
            if (_mission.LastFlowStatusGUID != AHDMissionStatusFlow.Reviewed || _mission.AuthByDeputyRepresentativeGUID != _mission.AuthByDeputyRepresentativeGUID)
            {
                throw new HttpException(401, "Unauthorized access");
            }
            //if (!ModelState.IsValid || _TypeOfMissionGUID == null || _TypeOfTravelGUID == null)
            //{
            //    return Json(DbAHD.PermissionError());
            //}

            DateTime ExecutionTime = DateTime.Now;
            if (_mission.LastFlowStatusGUID == AHDMissionStatusFlow.Reviewed)
            {
                _mission.LastFlowStatusGUID = AHDMissionStatusFlow.Approved;

                _mission.LastFlowStatus = "Approved";

            }

            _mission.AuthByDeputyRepresentativeGUID = UserGUID;
            _mission.AuthByDeputyRepDate = ExecutionTime;
            _mission.AuthorizedComment = _AuthorizedComment;

            var toChange = DbAHD.dataMissionRequestFlow.Where(x => x.MissionRequestGUID == _mission.MissionRequestGUID

                && x.IsLastAction == true).FirstOrDefault();
            toChange.IsLastAction = false;
            dataMissionRequestFlow flow = new dataMissionRequestFlow

            {
                MissionRequestFlowGUID = Guid.NewGuid(),
                MissionRequestGUID = _mission.MissionRequestGUID,
                FlowStatusGUID = AHDMissionStatusFlow.Approved,
                IsLastAction = true,
                CreateByGUID = UserGUID,
                CreateDate = ExecutionTime,
                OrderId = toChange.OrderId + 1
            };

            DbAHD.UpdateNoAudit(_mission);
            DbAHD.UpdateNoAudit(toChange);
            DbAHD.CreateNoAudit(flow);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                //check here
                //admin confirm permission dceb06ff-4844-4084-917b-8dfae457b9ec

                SubmitMissionRequestEmail(_mission, Guid.Parse("dceb06ff-4844-4084-917b-8dfae457b9ec"), AHDMissionStatusFlow.Approved, 0);
                return Json(new
                {
                    success = 1,


                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        public ActionResult RejectMissionRequestByOIC(Guid _MissionRequestGUID, string _AuthorizedComment)
        {
            if (string.IsNullOrEmpty(_AuthorizedComment))
            {
                return Json(DbAHD.PermissionError());
            }
            var _mission = DbAHD.dataMissionRequest.Where(x => x.MissionRequestGUID == _MissionRequestGUID).FirstOrDefault();
            if (_mission.LastFlowStatusGUID != AHDMissionStatusFlow.Reviewed || _mission.AuthByDeputyRepresentativeGUID != UserGUID)
            {
                throw new HttpException(401, "Unauthorized access");
            }
            //if (!ModelState.IsValid || _TypeOfMissionGUID == null || _TypeOfTravelGUID == null)
            //{
            //    return Json(DbAHD.PermissionError());
            //}

            DateTime ExecutionTime = DateTime.Now;
            if (_mission.LastFlowStatusGUID == AHDMissionStatusFlow.Reviewed)
            {
                _mission.LastFlowStatusGUID = AHDMissionStatusFlow.Rejected;
                _mission.LastFlowStatus = "Rejected";

            }

            _mission.AuthorizedComment = _AuthorizedComment;


            var toChange = DbAHD.dataMissionRequestFlow.Where(x => x.MissionRequestGUID == _mission.MissionRequestGUID

                                 && x.IsLastAction == true).FirstOrDefault();
            toChange.IsLastAction = false;
            dataMissionRequestFlow flow = new dataMissionRequestFlow

            {
                MissionRequestFlowGUID = Guid.NewGuid(),
                MissionRequestGUID = _mission.MissionRequestGUID,
                FlowStatusGUID = AHDMissionStatusFlow.Rejected,
                IsLastAction = true,
                CreateByGUID = UserGUID,
                CreateDate = ExecutionTime,
                OrderId = toChange.OrderId + 1
            };

            DbAHD.UpdateNoAudit(_mission);
            DbAHD.UpdateNoAudit(toChange);
            DbAHD.CreateNoAudit(flow);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                //check here
                //admin confirm permission dceb06ff-4844-4084-917b-8dfae457b9ec

                SubmitMissionRequestEmail(_mission, Guid.Parse("dceb06ff-4844-4084-917b-8dfae457b9ec"), AHDMissionStatusFlow.Rejected, 3);
                return Json(new
                {
                    success = 1,


                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }
        #endregion
        #endregion

        #region Locations
        [HttpPost]
        public ActionResult LocationCreate(LocationsUpdateModel model)
        {
            //if (!CMS.HasAction(Permissions.Locations.Create, Apps.CMS, model.CountryGUID.ToString()))
            //{
            //    // throw new HttpException(401, "Unauthorized access");
            //    return Json(DbCMS.ErrorMessage("401 - Unauthorized access"));
            //}
            if (!ModelState.IsValid) return Json(DbCMS.ErrorMessage("Model Error"));
            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            codeLocations Location = Mapper.Map(model, new codeLocations());

            Location.LocationGUID = EntityPK;
            DbAHD.Create(Location, Permissions.Locations.CreateGuid, ExecutionTime, DbCMS);

            codeLocationsLanguages Language = Mapper.Map(model, new codeLocationsLanguages());
            Language.LocationGUID = EntityPK;
            DbAHD.Create(Language, Permissions.LocationsLanguages.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleCreateMessage(DbCMS.PrimaryKeyControl(Location), null, null, "", null));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }
        #endregion



    }
}