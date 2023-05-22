
using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.Library.MimeDetective;
using AppsPortal.ViewModels;
using AutoMapper;
using DocumentFormat.OpenXml.Drawing.ChartDrawing;
using FineUploader;
using iTextSharp.text.pdf.qrcode;
using LinqKit;
using PCA_DAL.Model;
using RES_Repo.Globalization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.PCA.Controllers
{
    public class PartnersCapacityAssessmentController : PCABaseController
    {
        public ActionResult PartnersCapacityAssessmentIndex()
        {
            if (!CMS.HasAction(Permissions.PartnersCapacityAssessment.Access, Apps.PCA))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/PCA/Views/PartnersCapacityAssessment/Index.cshtml");
        }

        [Route("PCA/PartnersCapacityAssessmentsDataTable/")]
        public JsonResult PartnerCentersDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<PartnersCapacityAssessmentsTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<PartnersCapacityAssessmentsTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.PartnersCapacityAssessment.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).Distinct().ToList();

            var All = (from a in DbPCA.dataPartnersCapacityAssessment.AsExpandable()
                       
                       select new PartnersCapacityAssessmentsTableModel
                       {
                           dataPartnersCapacityAssessmentRowVersion=a.dataPartnersCapacityAssessmentRowVersion,
                           Active=a.Active,
                           EmailAddress=a.EmailAddress,
                           PartnersCapacityAssessmentGUID=a.PartnersCapacityAssessmentGUID,
                           IssueDate=a.IssueDate,
                           Name_JobTitleOfPersone=a.Name_JobTitleOfPersone,
                           PartnerName=a.PartnerName,
                           AgancyConfirm=a.AgancyConfirm,
                           PartnerConfirm=a.PartnerConfirm,
                           AgancyEvaluation=a.dataPartnersCapacityAssessmentDocEvaluation.Where(x=>x.AgancyEvaluation.Value).Count(),
                           PartnerEvaluation = a.dataPartnersCapacityAssessmentDocEvaluation.Where(x => x.PartnerEvaluation.Value).Count()

                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<PartnersCapacityAssessmentsTableModel> Result = Mapper.Map<List<PartnersCapacityAssessmentsTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("PCA/PartnersCapacityAssessment/Update/{PK}")]
        public ActionResult PartnerCenterUpdate(Guid PK)
        {
            //if (!CMS.HasAction(Permissions.PartnersCapacityAssessment.Access, Apps.PCA))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            Session["PartnersCapacityAssessmentGUID"] = PK;
            var assessment = DbPCA.dataPartnersCapacityAssessment.Where(x => x.PartnersCapacityAssessmentGUID == PK).FirstOrDefault();
            //partner
            if (Session[SessionKeys.UserProfileGUID] == null)
            {
                if (assessment.AgancyConfirm && assessment.AgancyConfirm)
                {
                    return RedirectToAction("AssessmentCompleted");
                }
                if (assessment.AgancyConfirm && !assessment.AgancyConfirm)
                {

                    var model = (from a in DbPCA.codePartnersCapacityAssessmentDocLanguage.Where(x => x.Active && x.LanguageID == LAN)
                                 join b in DbPCA.dataPartnersCapacityAssessmentDocEvaluation.Where(x => x.Active && x.PartnersCapacityAssessmentGUID == PK) on a.PartnersCapacityAssessmentDocGUID equals b.PartnersCapacityAssessmentDocGUID
                                 select new PartnerConfirmUpdateModel
                                 {
                                     AssessmentTitle = a.Title,
                                     Comment = b.Comment,
                                     PartnerEvaluation = b.PartnerEvaluation,
                                     AgancyEvaluation = b.AgancyEvaluation,
                                     PartnersCapacityAssessmentGUID = b.PartnersCapacityAssessmentGUID.Value
                                 }
                  ).ToList();
                    return View("~/Areas/PCA/Views/PartnersCapacityAssessment/PartnersConfirm.cshtml", model);
                }
                else //Agency
                {
                    if (assessment.AgancyConfirm && assessment.AgancyConfirm)
                    {
                        var model = (from a in DbPCA.codePartnersCapacityAssessmentDocLanguage.Where(x => x.Active && x.LanguageID == LAN)
                                     join b in DbPCA.dataPartnersCapacityAssessmentDocEvaluation.Where(x => x.Active && x.PartnersCapacityAssessmentGUID == PK) on a.PartnersCapacityAssessmentDocGUID equals b.PartnersCapacityAssessmentDocGUID
                                     select new PartnerConfirmUpdateModel
                                     {
                                         AssessmentTitle = a.Title,
                                         Comment = b.Comment,
                                         PartnerEvaluation = b.PartnerEvaluation,
                                         AgancyEvaluation = b.AgancyEvaluation,
                                         PartnersCapacityAssessmentGUID = b.PartnersCapacityAssessmentGUID.Value
                                     }
                 ).ToList();
                        return View("~/Areas/PCA/Views/PartnersCapacityAssessment/PartnersConfirm.cshtml", model);
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                       
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult AssessmentCompleted()
        {
            Session["PartnersCapacityAssessmentGUID"] = null;
            Session["Sort"] = null;
            return View("~/Areas/PCA/Views/PartnersCapacityAssessment/AssessmentCompleted.cshtml");
        }
        public ActionResult Index()
        {
            Guid PartnersCapacityAssessmentGUID = Guid.Empty;
            if (Session["PartnersCapacityAssessmentGUID"] != null)
            {
                PartnersCapacityAssessmentGUID = Guid.Parse(Session["PartnersCapacityAssessmentGUID"].ToString());
            }
            //
            string[] Agancys = new string[]
            {
                "UNHCR", "UNICEF", "UNICEF", "WFP", "WHO", "FAO", "UNDP", "OCHA", "UNMAS", "UN-HABITAT", "ILO", "OTHER","UNFPA"
            };
            var modelAgency = (from a in DbPCA.codeOrganizations.Where(x => Agancys.Contains( x.OrganizationShortName.ToString().ToLower() )&& x.Active)
                               join b in DbPCA.dataPartnersCapacityAssessmentPartnershipAgency.Where(x => x.PartnersCapacityAssessmentGUID == PartnersCapacityAssessmentGUID) on a.OrganizationGUID equals b.OrganizationGUID into LJ1
                               from R1 in LJ1.DefaultIfEmpty()
                               select new PartnersCapacityAssessmentPartnershipAgency
                               {
                                   OrganizationGUID = a.OrganizationGUID,
                                   OrganizationShortName = a.OrganizationShortName,
                                   Checked = R1 != null ? R1.Checked : false
                               }).OrderBy(x => x.OrganizationShortName).ToList();

            var model = new PartnersCapacityAssessmentUpdateModel()
            {
                PartnersCapacityAssessmentPartnershipAgency = modelAgency
            };
            if (Session["PartnersCapacityAssessmentGUID"] != null)
            {
                model = DbPCA.dataPartnersCapacityAssessment.Where(x => x.PartnersCapacityAssessmentGUID == PartnersCapacityAssessmentGUID).Select(x =>
              new PartnersCapacityAssessmentUpdateModel()
              {
                  Active = x.Active,
                  PartnerConfirm = x.PartnerConfirm,
                  AgancyConfirm = x.AgancyConfirm,
                  EmailAddress = x.EmailAddress,
                  Name_JobTitleOfPersone = x.Name_JobTitleOfPersone,
                  PartnerName = x.PartnerName,
                  PartnersCapacityAssessmentGUID = x.PartnersCapacityAssessmentGUID,
              }).FirstOrDefault();
                model.PartnersCapacityAssessmentPartnershipAgency = modelAgency;
            }

            return View("~/Areas/PCA/Views/PartnersCapacityAssessment/SetupOrganization.cshtml", model);
        }

        [HttpPost]
        public ActionResult SetupOrganizationCreate(PartnersCapacityAssessmentUpdateModel model)
        {
            DateTime ExecutionTime = DateTime.Now;
            if (!ModelState.IsValid || ActivePartnersCapacityAssessment(model)) return View("~/Areas/PCA/Views/PartnersCapacityAssessment/SetupOrganization.cshtml", model);
            Guid PartnersCapacityAssessmentGUID = Guid.Empty;
            if (Session["PartnersCapacityAssessmentGUID"] != null)
            {
                 PartnersCapacityAssessmentGUID = Guid.Parse(Session["PartnersCapacityAssessmentGUID"].ToString());
            }

            var partnersCapacityAssessment = DbPCA.dataPartnersCapacityAssessment.Where(x=>x.PartnersCapacityAssessmentGUID==PartnersCapacityAssessmentGUID).FirstOrDefault();
            if (partnersCapacityAssessment==null)
            {
                partnersCapacityAssessment = new dataPartnersCapacityAssessment();
                Mapper.Map(model, partnersCapacityAssessment);
                partnersCapacityAssessment.IssueDate = DateTime.Now;
                DbPCA.CreateNoAudit(partnersCapacityAssessment);
            }
            else
            {
                Mapper.Map(model, partnersCapacityAssessment);
            }

            foreach (var row in model.PartnersCapacityAssessmentPartnershipAgency)
            {
                var partnershipAgency = DbPCA.dataPartnersCapacityAssessmentPartnershipAgency.Where(x => x.PartnersCapacityAssessmentGUID == PartnersCapacityAssessmentGUID && x.OrganizationGUID==row.OrganizationGUID).FirstOrDefault();

                if (partnershipAgency == null)
                {
                    partnershipAgency = new dataPartnersCapacityAssessmentPartnershipAgency();
                    partnershipAgency.Active = true;
                    partnershipAgency.Checked = row.Checked;
                    partnershipAgency.OrganizationGUID = row.OrganizationGUID;
                    partnershipAgency.PartnersCapacityAssessmentGUID = partnersCapacityAssessment.PartnersCapacityAssessmentGUID;
                    partnershipAgency.PartnersCapacityAssessmentPartnershipAgencyGUID = Guid.NewGuid();
                   
                    DbPCA.CreateNoAudit(partnershipAgency);
                }
                else
                {
                    partnershipAgency.Checked = row.Checked;
                }
            }
            try
            {
                DbPCA.SaveChanges();
                //DbCMS.SaveChanges();
                if (Session["PartnersCapacityAssessmentGUID"] == null)
                {
                    Session["PartnersCapacityAssessmentGUID"] = partnersCapacityAssessment.PartnersCapacityAssessmentGUID;
                }
                Session["Sort"] = 1;
                return RedirectToAction("Assessment");
            }
            catch (Exception ex)
            {
                return Json(DbPCA.ErrorMessage(ex.Message));
            }
        }

        private bool ActivePartnersCapacityAssessment(PartnersCapacityAssessmentUpdateModel model)
        {
            int error = 0;
            if (Session["PartnersCapacityAssessmentGUID"] == null)
            {
                var FoundRecord = DbPCA.dataPartnersCapacityAssessment.Where(x => x.EmailAddress == model.EmailAddress && x.Active).FirstOrDefault();
                if (FoundRecord != null)
                {

                    ModelState.AddModelError("EmailAddress", LAN == "EN" ? "You already submitted the assessment using this email address, Please contact assessment foical point." : "لقد أرسلت التقييم بالفعل باستخدام عنوان البريد الإلكتروني هذا ، يرجى الاتصال بجهة تنسيق التقييم.");
                    error++;
                }
            }

            return error > 0;
        }

        public ActionResult Assessment()
        {
            Guid PartnersCapacityAssessmentGUID = Guid.Empty;
            if (Session["PartnersCapacityAssessmentGUID"] != null)
            {
                PartnersCapacityAssessmentGUID = Guid.Parse(Session["PartnersCapacityAssessmentGUID"].ToString());
            }
            int Sort =  int.Parse(Session["Sort"].ToString());
            var model =(from a in  DbPCA.codePartnersCapacityAssessmentDocLanguage.Where(x => x.LanguageID==LAN && x.codePartnersCapacityAssessmentDoc.Sort == Sort && x.Active)
                        join b in  DbPCA.dataPartnersCapacityAssessmentDocEvaluation.Where(x=>x.PartnersCapacityAssessmentGUID== PartnersCapacityAssessmentGUID) on a.PartnersCapacityAssessmentDocGUID equals b.PartnersCapacityAssessmentDocGUID into LJ1
                        from R1 in LJ1.DefaultIfEmpty()
                        select 
                 new PartnersCapacityAssessmentDocUpdateModel
                 {

                     PartnersCapacityAssessmentDocGUID = a.codePartnersCapacityAssessmentDoc.PartnersCapacityAssessmentDocGUID,
                     Sort = a.codePartnersCapacityAssessmentDoc.Sort,
                     AssessmentDescription = a.Description,
                     AssessmentTitle = a.Title,
                     AgancyEvaluation = R1.AgancyEvaluation != null ? R1.AgancyEvaluation : false,
                     PartnerEvaluation = R1.PartnerEvaluation != null ? R1.PartnerEvaluation : false,
                     Comment = R1.Comment != null ? R1.Comment : "",
                     AgancyConfirm = R1.dataPartnersCapacityAssessment != null ? R1.dataPartnersCapacityAssessment.AgancyConfirm : false,
                     PartnerConfirm = R1.dataPartnersCapacityAssessment != null ? R1.dataPartnersCapacityAssessment.PartnerConfirm : false,
                     partnersCapacityAssessmentDocTitle = a.codePartnersCapacityAssessmentDoc.codePartnersCapacityAssessmentDocTitle.Select(y =>
                    new PartnersCapacityAssessmentDocTitleUpdateModel()
                    {
                        Checked = (y.dataPartnersCapacityAssessmentDocAttach.Where(z => z.PartnersCapacityAssessmentGUID == PartnersCapacityAssessmentGUID && z.PartnersCapacityAssessmentDocTitleGUID == y.PartnersCapacityAssessmentDocTitleGUID).FirstOrDefault() != null ?
                        y.dataPartnersCapacityAssessmentDocAttach.Where(z => z.PartnersCapacityAssessmentGUID == PartnersCapacityAssessmentGUID && z.PartnersCapacityAssessmentDocTitleGUID == y.PartnersCapacityAssessmentDocTitleGUID).FirstOrDefault().Checked.Value : false),
                        PartnersCapacityAssessmentDocTitleGUID = y.PartnersCapacityAssessmentDocTitleGUID,
                        Sort = y.Sort,
                        SupportDocTitle = y.codePartnersCapacityAssessmentDocTitleLanguage.Where(z => z.Active && z.Language == LAN).FirstOrDefault().Description,

                    }).ToList()

                 }).FirstOrDefault();

            return View("~/Areas/PCA/Views/PartnersCapacityAssessment/PartnersCapacityAssessment.cshtml", model);
        }

        [HttpPost]
        public ActionResult AssessmentCreate(PartnersCapacityAssessmentDocUpdateModel model)
        {
            DateTime ExecutionTime = DateTime.Now;
            bool error = false;
            Guid PartnersCapacityAssessmentGUID = Guid.Empty;
            if (Session["PartnersCapacityAssessmentGUID"] != null)
            {
                PartnersCapacityAssessmentGUID = Guid.Parse(Session["PartnersCapacityAssessmentGUID"].ToString());
            }

            var docEvaluation = DbPCA.dataPartnersCapacityAssessmentDocEvaluation.Where(x => x.Active &&
             x.PartnersCapacityAssessmentGUID == PartnersCapacityAssessmentGUID && x.PartnersCapacityAssessmentDocGUID == model.PartnersCapacityAssessmentDocGUID).FirstOrDefault();
            if (docEvaluation == null)
            {
                docEvaluation = new dataPartnersCapacityAssessmentDocEvaluation();
                Mapper.Map(model, docEvaluation);
                docEvaluation.PartnersCapacityAssessmentGUID = PartnersCapacityAssessmentGUID;
                DbPCA.CreateNoAudit(docEvaluation);
            }
            else
            {
                Mapper.Map(model, docEvaluation);
            }
            
            int index = 0;
            foreach (var row in model.partnersCapacityAssessmentDocTitle)
            {
                error = false;
                dataPartnersCapacityAssessmentDocAttach partnershipAgency = DbPCA.dataPartnersCapacityAssessmentDocAttach.
                    Where(x => x.PartnersCapacityAssessmentDocTitleGUID == row.PartnersCapacityAssessmentDocTitleGUID &&
                    x.PartnersCapacityAssessmentGUID == PartnersCapacityAssessmentGUID).FirstOrDefault();
                if (partnershipAgency == null)
                {
                    partnershipAgency = new dataPartnersCapacityAssessmentDocAttach();
                    partnershipAgency.Active = true;
                    partnershipAgency.Checked = row.Checked;
                    partnershipAgency.PartnersCapacityAssessmentDocTitleGUID = row.PartnersCapacityAssessmentDocTitleGUID;
                    partnershipAgency.PartnersCapacityAssessmentGUID = Guid.Parse(Session["PartnersCapacityAssessmentGUID"].ToString());
                    partnershipAgency.PartnersCapacityAssessmentDocAttachGUID = Guid.NewGuid();
                    partnershipAgency.Ext = row.SupportFile != null ? row.SupportFile.FileName.Split('.')[1] : "";
                    DbPCA.CreateNoAudit(partnershipAgency);
                }
                else
                {
                    partnershipAgency.Checked = row.Checked;
                    if (partnershipAgency.Ext == "" && row.SupportFile != null)
                    {
                        partnershipAgency.Ext = row.SupportFile != null ? row.SupportFile.FileName.Split('.')[1] : "";
                    }
                }
                DbPCA.SaveChanges();
                error = Valdate(row, partnershipAgency.Ext, model.Comment);
                if (error)
                {
                    row.Checked = false;
                }
                index++;
            }
            try
            {
               
                if (!ModelState.IsValid) return View("~/Areas/PCA/Views/PartnersCapacityAssessment/PartnersCapacityAssessment.cshtml", model);
                DbPCA.SaveChanges();
                DbCMS.SaveChanges();
                model.Sort=model.Sort + model.index;
                Session["Sort"] = model.Sort;
                if (model.Sort==0)
                {
                    return RedirectToAction("Index");
                }
                if (model.Sort == 9)
                {
                    string messagePartner = "";
                    string messageAgency = "";
                    string FoaclPointStaffs = string.Join(";", (from a in DbCMS.configFocalPointStaff.Where(x => x.configFocalPoint.ApplicationGUID == Apps.PCA && x.Active && x.configFocalPoint.Active)
                                                                join b in DbCMS.userServiceHistory on a.UserGUID equals b.UserGUID
                                                                select b.EmailAddress).Distinct().ToArray());
                    var assessment = DbPCA.dataPartnersCapacityAssessment.Where(x => x.PartnersCapacityAssessmentGUID == PartnersCapacityAssessmentGUID).FirstOrDefault();
                    if (Session[SessionKeys.UserProfileGUID] != null)
                    {
                        if (CMS.HasAction(Permissions.PartnersCapacityAssessment.Access, Apps.PCA))
                        {
                            assessment.AgancyConfirm = true;
                            string URL = AppSettingsKeys.Domain + "PCA/PartnerConfirm/" + PartnersCapacityAssessmentGUID;
                            //string Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.VerifyEmail + "</a>";
                            string Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
                            //messagePartner += Anchor + "<br />";
                            messagePartner += Link + "<br />";
                            DbPCA.SaveChanges();
                            int AgancyEvaluation = assessment.dataPartnersCapacityAssessmentDocEvaluation.Where(x => x.AgancyEvaluation.Value).Count();

                            messagePartner = resxEmails.UNAssessmentPart_Partner
                                .Replace("@Name", assessment.Name_JobTitleOfPersone)
                                .Replace("@Link", Link)
                                .Replace("@Score", AgancyEvaluation.ToString())
                                .Replace("@Rate", (AgancyEvaluation <= 5 ? (LAN.ToLower() == "en" ? "low" : "منخفضة") : (AgancyEvaluation >= 6 && AgancyEvaluation <= 7) ? (LAN.ToLower() == "en" ? "medium" : "متوسطة") : (LAN.ToLower() == "en" ? "high" : "عالية")));
                                
                            //    "Dear "+ assessment.Name_JobTitleOfPersone+ "," + " <br /> " + "<br />" +
                            //"The PSEA Task Force has now completed the UN assessment phase of your organization’s PSEA capacity." + "<br />" +
                            //"Your final score is " + AgancyEvaluation + " which indicates that your organization has " + (AgancyEvaluation <= 5 ? "low" : (AgancyEvaluation >= 6 && AgancyEvaluation <= 7) ? "medium" : "high") + " capacity on PSEA." + "<br />" +
                            //"You may now access the final assessment following this link " + Link + " to review the Task Force scoring and comments. If you agree with the final assessment, kindly confirm by clicking on the confirm button at the bottom of the assessment." + "<br />" +
                            //"You may want to reach out to the PSEA Task Force for more clarification or review of the final assessment." + "<br />" + "<br />" +

                            //"Warm regards," + "<br />";
                             new Email().SendpartnersCapacityAssessment(assessment.EmailAddress, "", "", "UN assessment ", messagePartner);

                            messageAgency = resxEmails.UNAssessmentPart_Agency
                               .Replace("@PartnerName", assessment.PartnerName);
                               
                            //    "Dear PSEA assessment task force members," + " <br /> " +
                            //" This is a confirmation that you’ve completed the assessment of "+ assessment.PartnerName + ". " + " <br /> " +
                            //"A link was shared with the focal point for his confirmation and agreement on the assessment you provided." + " <br /> " +
                            //"Once the partner confirms, you will receive a confirmation email." + " <br /> " +

                            //"Warm regards," + " <br /> ";




                            new Email().SendpartnersCapacityAssessment(FoaclPointStaffs, "", "", "UN assessment", messageAgency);
                            return RedirectToAction("PartnersCapacityAssessmentIndex");
                        }
                    }
                    else
                    {
                        if (assessment != null)
                        {
                            if (!assessment.AgancyConfirm)
                            {
                                string URL = AppSettingsKeys.Domain + "PCA/PartnersCapacityAssessment/Update/" + PartnersCapacityAssessmentGUID;
                                string Host = "<a href='" + AppSettingsKeys.Domain + "' target='_blank'>" + AppSettingsKeys.Domain + "</a>";
                                string Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";

                                messagePartner = resxEmails.PartnersAssessmentPart_Partner
                                    .Replace("@Name", assessment.Name_JobTitleOfPersone)
                                    .Replace("@Link", Link);
                                // messagePartner = "Dear " + assessment.Name_JobTitleOfPersone + "<br />" + "<br />" +
                                // "Thank you very much for completing the partner’s PSEA self-assessment. You can now access your self-assessment following the below link. " + "<br />" +
                                // Link + "<br />" +
                                //"Your assessment will now be reviewed by the PSEA assessment task force and you’ll receive an email once the review is completed." + "<br />" +
                                // "PSEA assessment task force members may get in touch with you in the upcoming days to seek clarification on the answers you provided." + "<br />" + "<br />" +

                                // "Warm regards,";
                                new Email().SendpartnersCapacityAssessment(assessment.EmailAddress, "", "", "Partners assessment ", messagePartner);

                                messageAgency = resxEmails.PartnersAssessmentPart_Agency
                                    .Replace("@Name", assessment.Name_JobTitleOfPersone)
                                    .Replace("@PartnerName", assessment.PartnerName)
                                    .Replace("@Link", Host);
                                //messageAgency = "Dear PSEA assessment task force members," + "<br />" + "<br />" +
                                //assessment.Name_JobTitleOfPersone + " who is PSEA focal point of " + assessment.PartnerName + " has completed the self-assessment of his organization’s PSEA capacity.You may now access the assessment following the below link " + "<br />" + Host + "." + "<br />" +
                                //"Kindly review carefully the answers provided by the partner, and add your own answer and comments on every question of the assessment.You may want to reach out to " + assessment.Name_JobTitleOfPersone + " to seek more clarification before completing the assessment." + "<br />" + "<br />" +

                                //"Warm regards,";
                                new Email().SendpartnersCapacityAssessment(FoaclPointStaffs, "", "", "Partners assessment ", messageAgency);
                            }
                        }
                        
                        return RedirectToAction("AssessmentCompleted");
                    }
                }
                else
                {
                    return RedirectToAction("Assessment");
                }
                return RedirectToAction("Assessment");
            }
            catch (Exception ex)
            {
                return Json(DbPCA.ErrorMessage(ex.Message));
            }
        }

        public bool Valdate(PartnersCapacityAssessmentDocTitleUpdateModel model, string Ext, string comment)
        {
            int ErrorCount = 0;
            Guid PartnersCapacityAssessmentGUID = Guid.Empty;
            if (Session["PartnersCapacityAssessmentGUID"] != null)
            {
                PartnersCapacityAssessmentGUID = Guid.Parse(Session["PartnersCapacityAssessmentGUID"].ToString());
            }
            var partnershipAgency = DbPCA.dataPartnersCapacityAssessmentDocAttach.
                   Where(x => x.PartnersCapacityAssessmentDocTitleGUID == model.PartnersCapacityAssessmentDocTitleGUID &&
                   x.PartnersCapacityAssessmentGUID == PartnersCapacityAssessmentGUID).FirstOrDefault();
            if (model.Checked && partnershipAgency.Ext == "")
            {
                ModelState.AddModelError(string.Empty, resxPages.SupportDocIsMissing);
                ErrorCount++;
            }
            if (Session[SessionKeys.UserProfileGUID] != null && comment==null )
            {
                ModelState.AddModelError("Comment", LAN.ToLower()=="en"?"Field Required":"الحقل مطلوب");
                ErrorCount++;
            }

                return (ErrorCount > 0);
        }
        [Route("PCA/PartnersCapacityAssessment/Upload/{PK}")]
        public ActionResult Upload(Guid PK)
        {
            var model = (from a in DbPCA.codePartnersCapacityAssessmentDocTitle.WherePK(PK)
                         select new PartnersCapacityAssessmentDocTitleUpdateModel
                         {
                            PartnersCapacityAssessmentDocGUID=a.PartnersCapacityAssessmentDocGUID,
                            PartnersCapacityAssessmentDocTitleGUID=a.PartnersCapacityAssessmentDocTitleGUID,
                            SupportDocTitle=a.SupportDocTitle
                            
                         }).FirstOrDefault();


            return PartialView("~/Areas/PCA/Views/PartnersCapacityAssessment/_FileUpload.cshtml", model);
        }


        [HttpPost, ValidateAntiForgeryToken]///SHM/ShuttleRequests/UploadFiles
        public FineUploaderResult UploadFiles(FineUpload upload,Guid PK,string SupportDocTitle)
        {

            string error = "";
            Guid PartnersCapacityAssessmentGUID = Guid.Empty;
            if (Session["PartnersCapacityAssessmentGUID"] != null)
            {
                PartnersCapacityAssessmentGUID = Guid.Parse(Session["PartnersCapacityAssessmentGUID"].ToString());
            }
            if (FileTypeValidator.IsPDF(upload.InputStream) || FileTypeValidator.IsExcel(upload.InputStream) || FileTypeValidator.IsImage(upload.InputStream) || FileTypeValidator.IsWord(upload.InputStream) || FileTypeValidator.IsText(upload.InputStream))
            {

                string FilePath = ConfigurationManager.AppSettings["DataFolder"] + "\\Uploads\\PCA\\" + PartnersCapacityAssessmentGUID+"\\"+ PK;
                Directory.CreateDirectory(@FilePath);
                DirectoryInfo dirInfo = new DirectoryInfo(FilePath);
                try
                {
                    string Path = FilePath + "\\" + PK +"_"+dirInfo.GetFiles().Count()+ "." + upload.FileName.Split('.')[1];
                    //if (!System.IO.File.Exists(Path))
                    //{
                        using (var fileStream = System.IO.File.Create(Path))
                        {
                            upload.InputStream.Seek(0, SeekOrigin.Begin);
                            upload.InputStream.CopyTo(fileStream);
                            dataPartnersCapacityAssessmentDocAttach partnershipAgency = DbPCA.dataPartnersCapacityAssessmentDocAttach.Where(x => x.PartnersCapacityAssessmentDocTitleGUID == PK && x.PartnersCapacityAssessmentGUID == PartnersCapacityAssessmentGUID).FirstOrDefault();
                            if (partnershipAgency == null)
                            {
                                partnershipAgency = new dataPartnersCapacityAssessmentDocAttach();
                                partnershipAgency.SupportDocTitle = SupportDocTitle;
                                partnershipAgency.Active = true;
                                partnershipAgency.Checked = true;
                                partnershipAgency.PartnersCapacityAssessmentDocTitleGUID = PK;
                                partnershipAgency.PartnersCapacityAssessmentGUID = Guid.Parse(Session["PartnersCapacityAssessmentGUID"].ToString());
                                partnershipAgency.PartnersCapacityAssessmentDocAttachGUID = Guid.NewGuid();
                                partnershipAgency.Ext = upload != null ? upload.FileName.Split('.')[1] : "";
                                DbPCA.CreateNoAudit(partnershipAgency);
                            }
                            else
                            {
                                partnershipAgency.Checked = true;
                                if (partnershipAgency.Ext == "" && upload != null)
                                {
                                    partnershipAgency.SupportDocTitle = SupportDocTitle;
                                    partnershipAgency.Ext = upload != null ? upload.FileName.Split('.')[1] : "";
                                }
                            }
                            try
                            {
                                DbPCA.SaveChanges();
                                DbCMS.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    //}
                }
                catch (Exception ex)
                {
                }
            }
            else
            {
                error = "File Type Error!";
            }
            return new FineUploaderResult((error == "" ? true : false), new { Error = error, path = "", success = (error == "" ? true : false) });
        }
        public ActionResult DownloadFile(string PK)
        {
            Guid PartnersCapacityAssessmentGUID = Guid.Empty;
            if (Session["PartnersCapacityAssessmentGUID"] != null)
            {
                PartnersCapacityAssessmentGUID = Guid.Parse(Session["PartnersCapacityAssessmentGUID"].ToString());
            }
            var partnershipAgency = DbPCA.dataPartnersCapacityAssessmentDocAttach.Where(x => x.PartnersCapacityAssessmentDocTitleGUID.ToString() == PK && x.PartnersCapacityAssessmentGUID == PartnersCapacityAssessmentGUID).FirstOrDefault();
            string FilePath = ConfigurationManager.AppSettings["DataFolder"] + "\\Uploads\\PCA\\" + PartnersCapacityAssessmentGUID.ToString() + "\\" + PK /*+ "." + partnershipAgency.Ext*/;
            //byte[] fileBytes = System.IO.File.ReadAllBytes(@FilePath);
            //return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, partnershipAgency.SupportDocTitle + "."+ partnershipAgency.Ext);

            List<FileInfo> listFiles = new List<FileInfo>();
            DirectoryInfo dirInfo = new DirectoryInfo(FilePath);
            int i = 0;

            foreach (var item in dirInfo.GetFiles())
            {
                listFiles.Add(new FileInfo()
                {
                    FileId = i + 1,
                    FileName = item.Name,
                    FilePath = dirInfo.FullName + @"\" + item.Name
                });

                i = i + 1;
            }

            using (var memoryStream = new MemoryStream())
            {
                using (var ziparchive = new System.IO.Compression.ZipArchive(memoryStream, System.IO.Compression.ZipArchiveMode.Create, true))
                {
                    for (int z = 0; z < listFiles.Count; z++)
                    {
                        ziparchive.CreateEntryFromFile(listFiles[z].FilePath, listFiles[z].FileName);
                    }
                }

                return File(memoryStream.ToArray(), "application/zip", partnershipAgency.SupportDocTitle+".zip");
            }
        }
        public class FileInfo
        {
            public int FileId { get; set; }
            public string FileName { get; set; }
            public string FilePath { get; set; }
        }

        [Route("PCA/PartnerConfirm/{PK}")]
        public ActionResult PartnerConfirm(Guid PK)
        {
            var assessment = DbPCA.dataPartnersCapacityAssessment.Where(x => x.PartnersCapacityAssessmentGUID == PK).FirstOrDefault();
            if (assessment.PartnerConfirm)
            {
                return RedirectToAction("AssessmentCompleted");
            }
            else
            {
                var model = (from a in DbPCA.codePartnersCapacityAssessmentDocLanguage.Where(x => x.Active && x.LanguageID == LAN)
                             join b in DbPCA.dataPartnersCapacityAssessmentDocEvaluation.Where(x => x.Active && x.PartnersCapacityAssessmentGUID == PK) on a.PartnersCapacityAssessmentDocGUID equals b.PartnersCapacityAssessmentDocGUID
                             select new PartnerConfirmUpdateModel
                             {
                                 AssessmentTitle = a.Title,
                                 Comment = b.Comment,
                                 PartnerEvaluation = b.PartnerEvaluation,
                                 AgancyEvaluation = b.AgancyEvaluation,
                                 PartnersCapacityAssessmentGUID = b.PartnersCapacityAssessmentGUID.Value
                             }
                       ).ToList();
                return View("~/Areas/PCA/Views/PartnersCapacityAssessment/PartnersConfirm.cshtml", model);
            }
          
        }

        [HttpPost]
        public ActionResult PartnerConfirmResult(Guid PartnersCapacityAssessmentGUID)
        {
            DateTime ExecutionTime = DateTime.Now;
            string FoaclPointStaffs = string.Join(";", (from a in DbCMS.configFocalPointStaff.Where(x => x.configFocalPoint.ApplicationGUID == Apps.PCA && x.Active && x.configFocalPoint.Active)
                                                        join b in DbCMS.userServiceHistory on a.UserGUID equals b.UserGUID
                                                        select b.EmailAddress).Distinct().ToArray());
            try
            {

                DbPCA.dataPartnersCapacityAssessment.Where(x=>x.PartnersCapacityAssessmentGUID== PartnersCapacityAssessmentGUID).FirstOrDefault().PartnerConfirm = true;
                DbPCA.SaveChanges();
                var assessment = DbPCA.dataPartnersCapacityAssessment.Where(x => x.PartnersCapacityAssessmentGUID == PartnersCapacityAssessmentGUID).FirstOrDefault();
                int AgancyEvaluation = assessment.dataPartnersCapacityAssessmentDocEvaluation.Where(x => x.AgancyEvaluation.Value).Count();
                
                
                string messagePartner = resxEmails.FinalApproval_Partner
                    .Replace("@Name", assessment.Name_JobTitleOfPersone)
                     .Replace("@Score", AgancyEvaluation.ToString())
                     .Replace("@Rate", (AgancyEvaluation <= 5 ? (LAN.ToLower() == "en" ? "low" : "منخفضة") : (AgancyEvaluation >= 6 && AgancyEvaluation <= 7) ? (LAN.ToLower() == "en" ? "medium" : "متوسطة") : (LAN.ToLower() == "en" ? "high" : "عالية")));

                //    "Dear "+ assessment.Name_JobTitleOfPersone + "<br />" + "<br />" +
                //"This is a confirmation that you have accepted the final assessment of your organization’s PSEA capacity." + "<br />" +
                //"Your final score is " + AgancyEvaluation + " which indicates that your organization has " + (AgancyEvaluation <= 5 ? "low" : (AgancyEvaluation >= 6 && AgancyEvaluation <= 7) ? "medium" : "high") + " capacity on PSEA." + "<br />" +
                //(AgancyEvaluation <= 7?"You will be contacted in the upcoming weeks by PSEA assessment task force for the development of a PSEA capacity enhancement plan.":"") + "<br />" + "<br />" +

                //"Warm regards," + "<br />";
                new Email().SendpartnersCapacityAssessment(assessment.EmailAddress, "", "", "Final approval from partner", messagePartner);



                string messageAgency = resxEmails.FinalApproval_Agency
                    .Replace("@Score", AgancyEvaluation.ToString())
                     .Replace("@Rate", (AgancyEvaluation <= 5 ? (LAN.ToLower()=="en"?"low":"منخفضة") : (AgancyEvaluation >= 6 && AgancyEvaluation <= 7) ? (LAN.ToLower() == "en" ? "medium":"متوسطة") : (LAN.ToLower() == "en" ? "high":"عالية")))
                .Replace("@PartnerName", assessment.PartnerName)
                .Replace("@Name", assessment.Name_JobTitleOfPersone);
                //    "Dear PSEA assessment task force members" + "<br />" + "<br />" +
                //"The partner "+assessment.PartnerName+" has now approved the final assessment of PSEA capacity." + "<br />" +
                //"The final score is " + AgancyEvaluation + " which indicates that Partenr has" + (AgancyEvaluation <= 5 ? " low " : (AgancyEvaluation >= 6 && AgancyEvaluation <= 7) ? "medium" : "high") + "capacity on PSEA." + "<br />" +
                //(AgancyEvaluation <= 7 ? "You may want to proceed with the development of a PSEA capacity enhancement plan.":"") + "<br />" + "<br />" +

                //"Warm regards," + "<br />";
                new Email().SendpartnersCapacityAssessment(FoaclPointStaffs, "", "", "Final approval from partner", messageAgency);
                DbPCA.SaveChanges();
                DbCMS.SaveChanges();                
            }
            catch (Exception ex)
            {
                return Json(DbPCA.ErrorMessage(ex.Message));
            }
            return RedirectToAction("AssessmentCompleted");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult PartnersCapacityAssessmentsDataTableDelete(List<dataPartnersCapacityAssessment> models)
        {
            if (!CMS.HasAction(Permissions.PartnersCapacityAssessment.Delete, Apps.PCA))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataPartnersCapacityAssessment> DeletedPartnersCapacityAssessments = DeletePartnersCapacityAssessments(models);

            try
            {
                DbPCA.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbPCA.PartialDeleteMessage(DeletedPartnersCapacityAssessments, models, DataTableNames.PartnersCapacityAssessmentsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbPCA.ErrorMessage(ex.Message));
            }
        }

        
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult PartnersCapacityAssessmentsDataTableRestore(List<dataPartnersCapacityAssessment> models)
        {
            if (!CMS.HasAction(Permissions.PartnersCapacityAssessment.Restore, Apps.PCA))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataPartnersCapacityAssessment> RestoredPartnersCapacityAssessments = RestorePartnersCapacityAssessments(models);

            try
            {
                DbPCA.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbPCA.PartialRestoreMessage(RestoredPartnersCapacityAssessments, models, DataTableNames.PartnersCapacityAssessmentsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbPCA.ErrorMessage(ex.Message));
            }
        }

        private List<dataPartnersCapacityAssessment> DeletePartnersCapacityAssessments(List<dataPartnersCapacityAssessment> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<dataPartnersCapacityAssessment> DeletedPartnersCapacityAssessments = new List<dataPartnersCapacityAssessment>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbPCA.QueryBuilder(models, Permissions.PartnersCapacityAssessment.DeleteGuid, SubmitTypes.Delete, baseQuery);

            var Records = DbPCA.Database.SqlQuery<dataPartnersCapacityAssessment>(query).ToList();
            foreach (var record in Records)
            {
                DeletedPartnersCapacityAssessments.Add(DbPCA.Delete(record, ExecutionTime, Permissions.PartnersCapacityAssessment.DeleteGuid, DbCMS));
            }

           
            return DeletedPartnersCapacityAssessments;
        }

        private List<dataPartnersCapacityAssessment> RestorePartnersCapacityAssessments(List<dataPartnersCapacityAssessment> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<dataPartnersCapacityAssessment> RestoredPartnersCapacityAssessments = new List<dataPartnersCapacityAssessment>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbPCA.QueryBuilder(models, Permissions.PartnersCapacityAssessment.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Records = DbPCA.Database.SqlQuery<dataPartnersCapacityAssessment>(query).ToList();
            foreach (var record in Records)
            {

                    RestoredPartnersCapacityAssessments.Add(DbPCA.Restore(record, Permissions.PartnersCapacityAssessment.DeleteGuid, Permissions.PartnersCapacityAssessment.RestoreGuid, RestoringTime, DbCMS));

            }

           

            return RestoredPartnersCapacityAssessments;
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SetupOrganizationCreateDelete(dataPartnersCapacityAssessment model)
        {

            dataPartnersCapacityAssessment DeletedPartnersCapacityAssessment = DbPCA.dataPartnersCapacityAssessment.Where(x => x.PartnersCapacityAssessmentGUID == model.PartnersCapacityAssessmentGUID).FirstOrDefault();

            try
            {
                DbPCA.dataPartnersCapacityAssessment.Remove(DeletedPartnersCapacityAssessment);
                int CommitedRows = DbPCA.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbPCA.SingleDeleteMessage(new List<dataPartnersCapacityAssessment>() { DeletedPartnersCapacityAssessment }, null, "window.location.replace('/PCA/PartnersCapacityAssessment/AssessmentCompleted');"));
            }
            catch (DbUpdateConcurrencyException)
            {
                return null;
            }
            catch (Exception ex)
            {
                return Json(DbPCA.ErrorMessage(ex.Message));
            }
        }

        public ActionResult DownloadAllFiles(string PK)
        {
            Guid PartnersCapacityAssessmentGUID = Guid.Empty;
            if (Session["PartnersCapacityAssessmentGUID"] != null)
            {
                PartnersCapacityAssessmentGUID = Guid.Parse(Session["PartnersCapacityAssessmentGUID"].ToString());
            }
            var pcad = DbPCA.codePartnersCapacityAssessmentDoc.Where(x =>  x.PartnersCapacityAssessmentDocGUID.ToString() == PK).FirstOrDefault();
            string FilePath = ConfigurationManager.AppSettings["DataFolder"] + "Uploads\\PCA\\";
            //byte[] fileBytes = System.IO.File.ReadAllBytes(@FilePath);
            //return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, partnershipAgency.SupportDocTitle + "."+ partnershipAgency.Ext);

            var partners = DbPCA.dataPartnersCapacityAssessment.ToList();
            List<FileInfo> listFiles = new List<FileInfo>();
            DirectoryInfo dirInfo = new DirectoryInfo(FilePath);
            int i = 0;
            foreach (var item in dirInfo.GetDirectories())//D:\WebData\Uploads\PCA
            {
                var partner = partners.Where(x => x.PartnersCapacityAssessmentGUID.ToString() == item.Name).FirstOrDefault();
                if (partner != null)
                {
                    string PartnerName = partners.Where(x => x.PartnersCapacityAssessmentGUID.ToString() == item.Name).FirstOrDefault().PartnerName;
                    DirectoryInfo dirInfoSub = new DirectoryInfo(item.FullName);
                    string[] includedFiles = DbPCA.codePartnersCapacityAssessmentDocTitle.Where(x => x.PartnersCapacityAssessmentDocGUID.ToString() == PK).Select(x => x.PartnersCapacityAssessmentDocTitleGUID.ToString()).ToArray<string>();
                    foreach (var itemSub in dirInfoSub.GetDirectories())
                    {
                        if (includedFiles.Contains(itemSub.Name))
                        {
                            foreach (var file in itemSub.GetFiles())
                            {

                                listFiles.Add(new FileInfo()
                                {
                                    FileId = i + 1,
                                    FileName = partner.PartnerName + file.Extension,
                                    FilePath = file.FullName
                                });
                            }
                        }


                        i = i + 1;
                    }
                }

            }

            using (var memoryStream = new MemoryStream())
            {
                using (var ziparchive = new System.IO.Compression.ZipArchive(memoryStream, System.IO.Compression.ZipArchiveMode.Create, true))
                {
                    for (int z = 0; z < listFiles.Count; z++)
                    {
                        ziparchive.CreateEntryFromFile(listFiles[z].FilePath, listFiles[z].FileName);
                    }
                }

                return File(memoryStream.ToArray(), "application/zip", pcad.AssessmentTitle + ".zip");
            }
        }
    }
}