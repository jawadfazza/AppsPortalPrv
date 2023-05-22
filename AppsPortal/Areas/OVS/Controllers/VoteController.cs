using System;
using System.Collections.Generic;
using System.Linq;
//using System.ServiceModel.Activities;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using AppsPortal.BaseControllers;
using AppsPortal.Extensions;

using AppsPortal.Library;
using AppsPortal.ViewModels;
using AppsPortal.OVS.ViewModels;
using AutoMapper;
using LinqKit;
using OVS_DAL.Model;
using RES_Repo.Globalization;

namespace AppsPortal.Areas.OVS.Controllers
{
    public class VoteController : OVSBaseController
    {
        
        public ActionResult VoteChecker(string accessKey)
        {
            dataElectionStaff staffelection = DbOVS.dataElectionStaff.FirstOrDefault(a =>
                (!String.IsNullOrEmpty(accessKey)) && a.AccessKey.Equals(accessKey));
            VoteCheckerModel model = new VoteCheckerModel();
            if (accessKey == "01")
            {
                model.ActiveElection = true;
                model.MessageTitle = resxMessages.VoteComplete;
                model.MessageBody = resxMessages.VoteSucessMessage;
                return View("~/Areas/OVS/Views/Vote/VoteChecker.cshtml", model);
            }
            if (staffelection != null )
            {
                TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(staffelection.dataElection.TimeZone);
                DateTime dtzCloseDatetime = TimeZoneInfo.ConvertTimeFromUtc((DateTime)staffelection.dataElection.CloseDate, tz);


                int totalVots = 0;
                if (staffelection != null)
                    totalVots = DbOVS.dataStaffVote.Count(x => x.ElectionStaffGUID == staffelection.ElectionStaffGUID);
                
              

                model.ActiveElection = false;
                if (staffelection == null)
                {
                    model.ActiveElection = false;
                    model.MessageTitle = resxMessages.VoteError;
                    model.MessageBody = resxMessages.VoteNotExist;
                    return View("~/Areas/OVS/Views/Vote/VoteChecker.cshtml", model);
                }

                else if (staffelection.VoteDate != null || totalVots > 0)
                {
                    model.ActiveElection = false;
                    model.MessageTitle = resxMessages.VoteComplete;
                    model.MessageBody = resxMessages.VoteAlreadyDone;
                    //you have to change to page you vote before
                    return View("~/Areas/OVS/Views/Vote/VoteChecker.cshtml", model);
                }
                else if (dtzCloseDatetime < DateTime.Now)
                {
                    model.ActiveElection = false;
                    model.MessageTitle = resxMessages.VoteError;
                    model.MessageBody = resxMessages.VoteExpired;
                    //you have to change to page you vote before
                    return View("~/Areas/OVS/Views/Vote/VoteChecker.cshtml", model);
                }
                else if (staffelection != null && staffelection.VoteDate == null && dtzCloseDatetime >= DateTime.Now)
                {
                    Session[SessionKeys.ElectionStaffGuid] = staffelection.ElectionStaffGUID.ToString();

                    model.ActiveElection = true;
                    model.AccessKey = accessKey;
                    model.MessageTitle = resxMessages.ElectionWelcomeMessage+" " + staffelection.dataElection.dataElectionLanguage
                                             .FirstOrDefault(x => x.LanguageID == LAN && x.Active).Title;
                    model.MessageBody += staffelection.dataElection.dataElectionLanguage.FirstOrDefault(x => x.LanguageID == LAN).Details == null ? resxMessages.ElectionTotalCandidates +
                                         staffelection.dataElection.dataElectionCandidate.Count(x => x.Active) :
                        staffelection.dataElection.dataElectionLanguage.FirstOrDefault(x => x.LanguageID == LAN).Details+ "\n\n";
                    model.MessageBody += resxMessages.ElectionStartMessage;
                    return View("~/Areas/OVS/Views/Vote/VoteChecker.cshtml", model);
                }
            }
            else
            {
                model.ActiveElection = false;
                model.MessageTitle = resxMessages.VoteError;
                model.MessageBody = resxMessages.VoteNotExist;
                //you have to change to page you vote before
                return View("~/Areas/OVS/Views/Vote/VoteChecker.cshtml", model);
            }


            return View("Errors/Error");
        }

       

        // GET: VOT/Vote
        public ActionResult VoteCreate(string FK)
        {
            StaffVoteElectionModel model = new StaffVoteElectionModel();
            model.dataElectionStaffs = DbOVS.dataElectionStaff.FirstOrDefault(x => (!String.IsNullOrEmpty(FK)) && x.AccessKey.Equals(FK));

            VoteCheckerModel checkerModel = new VoteCheckerModel();
            if (model.dataElectionStaffs == null)
            {
                checkerModel.ActiveElection = false;
                checkerModel.MessageTitle = resxMessages.VoteError;
                checkerModel.MessageBody = resxMessages.VoteAlreadyDone;
                //you have to change to page you vote before
                return View("~/Areas/OVS/Views/Vote/VoteChecker.cshtml", checkerModel);
            }

            if (model.dataElectionStaffs.VoteDate != null)
            {
                checkerModel.ActiveElection = false;
                checkerModel.MessageTitle = resxMessages.VoteError;
                checkerModel.MessageBody = resxMessages.VoteAlreadyDone;
                //you have to change to page you vote before
                return View("~/Areas/OVS/Views/Vote/VoteChecker.cshtml", checkerModel);
            }

            Session[SessionKeys.ElectionStaffGuid] = model.dataElectionStaffs.ElectionStaffGUID.ToString();
            checkerModel.ActiveElection = true;
            string photoPath =  "~\\Uploads\\OVS\\CanidiatePhotos\\";

            model = DbOVS.dataElectionLanguage.Where(x =>
                    x.Active && x.LanguageID == LAN && x.ElectionGUID == model.dataElectionStaffs.ElectionGUID)
                .Select(x => new StaffVoteElectionModel
                {
                    ElectionTitle = x.Title,
                    ElectionGUID = model.dataElectionStaffs.ElectionGUID,
                    electionCandidateModel = DbOVS.dataElectionCandidate
                        .Where(z => z.ElectionGUID == model.dataElectionStaffs.ElectionGUID && z.Active)
                        .Select(z => new ElectionCandidateModel
                        {
                            ElectionCandidateGUID = z.ElectionCandidateGUID,
                            CandidatePhoto= photoPath +"XS_"+ z.ElectionCandidateGUID + ".jpg" ,
                            FullName = z.FullName,
                            EmailAddress = z.EmailAddress,
                            CampaignPlan = z.CampaignPlan,
                            IsVerified = false
                        }).ToList(),
                }).FirstOrDefault();
            return View("~/Areas/OVS/Views/Vote/Index.cshtml", model);
        }

        public ActionResult ElectionCandidatesDataTable(DataTableRecievedOptions options, Guid PK)
        {
            dataElectionStaff electionStaff = DbOVS.dataElectionStaff.FirstOrDefault(a => a.ElectionStaffGUID == PK);
            //if (options.columns == null)
            //    return PartialView("~/Areas/OVS/Views/Election/_CandidatesDataTable.cshtml",
            //        new MasterRecordStatus { ParentGUID = accessKey, IsParentActive = true });
            if (electionStaff != null)
            {
                //change the message
                if (electionStaff.VoteDate != null)
                {
                    throw new HttpException(401, "Unauthorized access");
                }

                DataTableOptions DataTable = ConvertOptions.Fill(options);
                var Result =
                (from a in DbOVS.dataElectionCandidate.AsExpandable()
                        .Where(x => x.ElectionGUID == electionStaff.ElectionGUID && x.Active)
                    select new
                    {
                        ElectionCandidateGUID = a.ElectionCandidateGUID,
                        EmailAddress = a.EmailAddress,
                        FullName = a.FullName,
                        //CampaignPlan = a.CampaignPlan,
                        GenderGUID = a.GenderGUID,
                        dataElectionCandidatesRowVersion = a.dataElectionCandidateRowVersion
                    });
                Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);
                return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
            }

            throw new HttpException(401, "Unauthorized access");
        }

        public ActionResult ElectionCandidatesDataTableVote(List<dataElectionCandidate> models)
        {
            if (models != null && models.Count == 0) ModelState.AddModelError("", resxMessages.NoCandidateChosen);
            var electionStaffGuid = Guid.Parse(Session[SessionKeys.ElectionStaffGuid].ToString());
            VoteCheckerModel modelChecker = new VoteCheckerModel();
            modelChecker.status = true;
            var electionStaff =
                DbOVS.dataElectionStaff.FirstOrDefault(x =>
                    x.ElectionStaffGUID == electionStaffGuid && x.VoteDate == null );
            if (DbOVS.dataStaffVote.Count(x => x.dataElectionStaff.ElectionStaffGUID== electionStaffGuid) > 0)
            {
                return Json(DbOVS.ErrorMessage(resxMessages.NotAllowedToVote));
            }

            
            var electionConditions =
                DbOVS.dataElectionCondition.Where(x => x.ElectionGUID == electionStaff.ElectionGUID && x.Active);
            if (electionConditions.Any())
            {
                dataElectionStaff model =
                    DbOVS.dataElectionStaff.FirstOrDefault(x => x.ElectionGUID == electionStaff.ElectionGUID);
                List<dataElectionCandidate> allElectionCanidates = DbOVS.dataElectionCandidate
                    .Where(x => x.ElectionGUID == electionStaff.ElectionGUID)
                    .ToList();
                foreach (var myCandidate in models)
                {
                    myCandidate.GenderGUID = DbOVS.dataElectionCandidate
                        .FirstOrDefault(x => x.ElectionCandidateGUID == myCandidate.ElectionCandidateGUID)
                        .GenderGUID;
                }
                modelChecker = ActiveStaffVote(electionConditions.ToList(), models, allElectionCanidates);
                
            }
            if (modelChecker.status == false)
            {
                return Json(DbOVS.ErrorMessage(modelChecker.MessageBody));

            }

            else if (electionStaff != null && modelChecker.status)
            {
                var staffVotes = new List<dataStaffVote>();
                var ExecutionTime = DateTime.Now;
                electionStaff.VoteDate = ExecutionTime;
               // DbOVS.Update(electionStaff, Permissions.ElectionsManagement.UpdateGuid, ExecutionTime, DbCMS);
                foreach (var model in models)
                {
                    var dataStaffVote = new dataStaffVote();
                    dataStaffVote.StaffVoteGUID = Guid.NewGuid();
                    dataStaffVote.Active = true;
                    dataStaffVote.ElectionCandidateGUID = model.ElectionCandidateGUID;
                    dataStaffVote.ElectionStaffGUID = electionStaffGuid;
                    staffVotes.Add(dataStaffVote);
                }
                
                DbOVS.dataStaffVote.AddRange(staffVotes);
                try
                {
                    DbOVS.SaveChanges();
                    DbCMS.SaveChanges();
                    JsonReturn JsonResult = new JsonReturn();
                    JsonResult.Notify = new Notify {Type = MessageTypes.Success, Message = "Vote Added Successfully"};
                    JsonResult.CallbackFunction = "window.location.href = '/OVS/vote/voteChecker?accessKey=01'";
                    return Json(JsonResult);
                }
                catch (Exception ex)
                {
                    return Json(DbCMS.ErrorMessage(ex.Message));
                }
            }

            else
                throw new HttpException(401, "Unauthorized access");
        }

        private VoteCheckerModel ActiveStaffVote(List<dataElectionCondition> electionconditions,
            List<dataElectionCandidate> selectedCandidateses, List<dataElectionCandidate> allElectionCanidates)
        {
            VoteCheckerModel model=new VoteCheckerModel();
            model.status = true;
            
            foreach (var item in electionconditions)
            {
                //check Minimum number of allowed votes
                if (item.ConditionTypeGUID == Guid.Parse("00000000-0000-0000-0000-000000000001"))
                {
                    if (selectedCandidateses.Count() < int.Parse(item.ConditionValue))
                    {
                        model.status = false;
                        model.MessageBody += resxMessages.ChoseAppropriateNumberOFCanidates +" "+ item.ConditionValue+" " + resxDbFields.OfCandidates+",";
                    }
                }
                //check maximum number of allowed votes
                else if (item.ConditionTypeGUID == Guid.Parse("00000000-0000-0000-0000-000000000002"))
                {
                    if (selectedCandidateses.Count() > int.Parse(item.ConditionValue))
                    {
                        model.status = false;
                        model.MessageBody += resxMessages.ElectionExceedAllowedCandidates + " " + item.ConditionValue + ",";
                    }
                }
                //check total female 
                else if (item.ConditionTypeGUID == Guid.Parse("00000000-0000-0000-0000-000000000003"))
                {
                    if (selectedCandidateses
                            .Where(x => x.GenderGUID == Guid.Parse("DBF9D307-CE9F-4029-BD1E-D7AF6739975C"))
                            .Count() < int.Parse(item.ConditionValue))
                    {
                        model.status = false;
                        model.MessageBody += resxMessages.ChoseAppropriateNumberOFCanidates + " " + item.ConditionValue + " " + resxDbFields.OfFemaleCandidates + ",";
                    }
                }
                //check total Males 
                else if (item.ConditionTypeGUID == Guid.Parse("00000000-0000-0000-0000-000000000004"))
                {
                    if (selectedCandidateses
                            .Where(x => x.GenderGUID == Guid.Parse("688B11E0-24FB-44B8-94CE-D8568C4742C7"))
                            .Count() < int.Parse(item.ConditionValue))
                    {
                        model.status = false;
                        model.MessageBody += resxMessages.ChoseAppropriateNumberOFCanidates + " " + item.ConditionValue + " " + resxDbFields.OfMaleCandidates + ",";
                    }
                }
            }

            return model;
        }

        public ActionResult GetCandidateInfo(Guid PK)
        {
            var model = DbOVS.dataElectionCandidate.Find(PK);

            return PartialView("~/Areas/OVS/Views/Vote/_CandidateInfo.cshtml", model);
        }
   
    }
}