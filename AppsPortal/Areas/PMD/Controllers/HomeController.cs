using AppsPortal.BaseControllers;
using AppsPortal.Library;
using OfficeOpenXml;
using PMD_DAL.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.PMD.Controllers
{
    public class HomeController : PMDBaseController
    {
        // GET: PMD/Home
        public ActionResult Index()
        {
            CMS.SetUserToken(UserProfileGUID, Apps.PMD);
            Session[SessionKeys.CurrentApp] = Apps.PMD;
            CMS.BuildUserMenus(UserGUID, LAN);
            return View();
        }

        public class TempCodes
        {
            public string ObjectiveEN { get; set; } //A
            public string OutputEN { get; set; }        //C
            public string IndicatorEN { get; set; }      //E
            public string GuidanceEN { get; set; }         //G

            public string ObjectiveAR { get; set; }  //B
            public string OutputAR { get; set; }     //D
            public string IndicatorAR { get; set; }        //F
            public string GuidanceAR { get; set; }        //H
        }



        public ActionResult InsertCodes()
        {

            FileInfo locationInfo = new FileInfo(Server.MapPath("~/Areas/PMD/Templates/indicators.xlsx"));

            List<TempCodes> tempCodes = new List<TempCodes>();
            List<codePmdObjective> codePmdObjectives = new List<codePmdObjective>();
            List<codePmdObjectiveLanguage> codePmdObjectiveLanguages = new List<codePmdObjectiveLanguage>();
            List<codePmdOutput> codePmdOutputs = new List<codePmdOutput>();
            List<codePmdOutputLanguage> codePmdOutputLanguages = new List<codePmdOutputLanguage>();
            List<codePmdIndicator> codePmdIndicators = new List<codePmdIndicator>();
            List<codePmdIndicatorLanguage> codePmdIndicatorLanguages = new List<codePmdIndicatorLanguage>();

            using (var package = new ExcelPackage(locationInfo))
            {
                ExcelWorksheet workSheet = package.Workbook.Worksheets["Sheet1"];
                int totalRows = workSheet.Dimension.End.Row;
                for (int i = 1; i <= totalRows; i++)
                {
                    string objectiveEN = workSheet.Cells["A" + i].Value.ToString();
                    string outputEN = workSheet.Cells["C" + i].Value.ToString();
                    string indicatorEN = workSheet.Cells["E" + i].Value.ToString();
                    string guidanceEN = "";
                    try
                    {
                        guidanceEN = workSheet.Cells["G" + i].Value.ToString();

                    }
                    catch
                    {

                    }
                    string objectiveAR = workSheet.Cells["B" + i].Value.ToString();
                    string outputAR = workSheet.Cells["D" + i].Value.ToString();
                    string indicatorAR = workSheet.Cells["F" + i].Value.ToString();
                    string guidanceAR = "";
                    try
                    {
                        guidanceAR = workSheet.Cells["H" + i].Value.ToString();

                    }
                    catch
                    {

                    }
                    tempCodes.Add(new TempCodes
                    {
                        ObjectiveEN = objectiveEN.Trim(),
                        OutputEN = outputEN.Trim(),
                        IndicatorEN = indicatorEN.Trim(),
                        GuidanceEN = guidanceEN.Trim(),
                        ObjectiveAR = objectiveAR.Trim(),
                        OutputAR = outputAR.Trim(),
                        IndicatorAR = indicatorAR.Trim(),
                        GuidanceAR = guidanceAR.Trim()
                    });
                }
            }

            var objectives = (from a in tempCodes
                              group a by a.ObjectiveEN into g
                              select new
                              {
                                  g.Key,
                                  g
                              }).ToList();

            var obj = (from a in tempCodes select new { a.ObjectiveEN, a.ObjectiveAR }).Distinct().ToList();
            foreach (var item in obj)
            {
                Guid ObjectiveGUID = Guid.NewGuid();
                codePmdObjectives.Add(new codePmdObjective
                {
                    ObjectiveGUID = ObjectiveGUID,
                    Active = true
                });
                codePmdObjectiveLanguages.Add(new codePmdObjectiveLanguage
                {
                    ObjectiveLanguageGUID = Guid.NewGuid(),
                    ObjectiveGUID = ObjectiveGUID,
                    LanguageID = "EN",
                    ObjectiveDescription = item.ObjectiveEN,
                    Active = true
                });
                codePmdObjectiveLanguages.Add(new codePmdObjectiveLanguage
                {
                    ObjectiveLanguageGUID = Guid.NewGuid(),
                    ObjectiveGUID = ObjectiveGUID,
                    LanguageID = "AR",
                    ObjectiveDescription = item.ObjectiveAR,
                    Active = true
                });
            }

            foreach (var item in tempCodes)
            {
                if (codePmdOutputLanguages.Where(x => x.OutputDescription == item.OutputEN).Count() > 0)
                {
                    continue;
                }
                Guid ObjectiveGUID = (from a in codePmdObjectiveLanguages where a.ObjectiveDescription == item.ObjectiveEN select a.ObjectiveGUID).FirstOrDefault();
                Guid OutputGUID = Guid.NewGuid();
                codePmdOutputs.Add(new codePmdOutput
                {
                    OutputGUID = OutputGUID,
                    ObjectiveGUID = ObjectiveGUID,
                    Active = true
                });
                codePmdOutputLanguages.Add(new codePmdOutputLanguage
                {
                    OutputLanguageGUID = Guid.NewGuid(),
                    OutputGUID = OutputGUID,
                    LanguageID = "EN",
                    OutputDescription = item.OutputEN,
                    Active = true
                });
                codePmdOutputLanguages.Add(new codePmdOutputLanguage
                {
                    OutputLanguageGUID = Guid.NewGuid(),
                    OutputGUID = OutputGUID,
                    LanguageID = "AR",
                    OutputDescription = item.OutputAR,
                    Active = true
                });
            }

            foreach (var item in tempCodes)
            {
                if (codePmdIndicatorLanguages.Where(x => x.IndicatorDescription == item.IndicatorEN).Count() > 0)
                {
                    continue;
                }
                Guid OutputGUID = (from a in codePmdOutputLanguages where a.OutputDescription == item.OutputEN select a.OutputGUID).FirstOrDefault();
                Guid IndicatorGUID = Guid.NewGuid();
                codePmdIndicators.Add(new codePmdIndicator
                {
                    IndicatorGUID = IndicatorGUID,
                    OutputGUID = OutputGUID,
                    Active = true
                });
                codePmdIndicatorLanguages.Add(new codePmdIndicatorLanguage
                {
                    IndicatorLanguageGUID = Guid.NewGuid(),
                    IndicatorGUID = IndicatorGUID,
                    LanguageID = "EN",
                    IndicatorDescription = item.IndicatorEN,
                    IndicatorGuidance = item.GuidanceEN,
                    Active = true
                });
                codePmdIndicatorLanguages.Add(new codePmdIndicatorLanguage
                {
                    IndicatorLanguageGUID = Guid.NewGuid(),
                    IndicatorGUID = IndicatorGUID,
                    LanguageID = "AR",
                    IndicatorDescription = item.IndicatorAR,
                    IndicatorGuidance = item.GuidanceAR,
                    Active = true
                });
            }

            //DbPMD.codePmdObjectives.AddRange(codePmdObjectives);
            //DbPMD.codePmdObjectiveLanguages.AddRange(codePmdObjectiveLanguages);
            //DbPMD.codePmdOutputs.AddRange(codePmdOutputs);
            //DbPMD.codePmdOutputLanguages.AddRange(codePmdOutputLanguages);
            //DbPMD.codePmdIndicators.AddRange(codePmdIndicators);
            //DbPMD.codePmdIndicatorLanguages.AddRange(codePmdIndicatorLanguages);
            //DbPMD.SaveChanges();
            return null;
        }
        public ActionResult InsertCodesUpdate()
        {

            FileInfo locationInfo = new FileInfo(Server.MapPath("~/Areas/PMD/Templates/indicators_update.xlsx"));

            List<TempCodes> tempCodes = new List<TempCodes>();
            List<codePmdObjective> codePmdObjectives = new List<codePmdObjective>();
            List<codePmdObjectiveLanguage> codePmdObjectiveLanguages = new List<codePmdObjectiveLanguage>();
            List<codePmdOutput> codePmdOutputs = new List<codePmdOutput>();
            List<codePmdOutputLanguage> codePmdOutputLanguages = new List<codePmdOutputLanguage>();
            List<codePmdIndicator> codePmdIndicators = new List<codePmdIndicator>();
            List<codePmdIndicatorLanguage> codePmdIndicatorLanguages = new List<codePmdIndicatorLanguage>();

            using (var package = new ExcelPackage(locationInfo))
            {
                ExcelWorksheet workSheet = package.Workbook.Worksheets["Sheet1"];
                int totalRows = workSheet.Dimension.End.Row;
                for (int i = 1; i <= totalRows; i++)
                {
                    string objectiveEN = workSheet.Cells["A" + i].Value.ToString();
                    string outputEN = workSheet.Cells["C" + i].Value.ToString();
                    string indicatorEN = workSheet.Cells["E" + i].Value.ToString();
                    string guidanceEN = "";
                    try
                    {
                        guidanceEN = workSheet.Cells["G" + i].Value.ToString();

                    }
                    catch
                    {

                    }
                    string objectiveAR = workSheet.Cells["B" + i].Value.ToString();
                    string outputAR = workSheet.Cells["D" + i].Value.ToString();
                    string indicatorAR = workSheet.Cells["F" + i].Value.ToString();
                    string guidanceAR = "";
                    try
                    {
                        guidanceAR = workSheet.Cells["H" + i].Value.ToString();

                    }
                    catch
                    {

                    }
                    tempCodes.Add(new TempCodes
                    {
                        ObjectiveEN = objectiveEN.Trim(),
                        OutputEN = outputEN.Trim(),
                        IndicatorEN = indicatorEN.Trim(),
                        GuidanceEN = guidanceEN.Trim(),
                        ObjectiveAR = objectiveAR.Trim(),
                        OutputAR = outputAR.Trim(),
                        IndicatorAR = indicatorAR.Trim(),
                        GuidanceAR = guidanceAR.Trim()
                    });
                }
            }

            var objectives = (from a in tempCodes
                              group a by a.ObjectiveEN into g
                              select new
                              {
                                  g.Key,
                                  g
                              }).ToList();

            var obj = (from a in tempCodes select new { a.ObjectiveEN, a.ObjectiveAR }).Distinct().ToList();
            foreach (var item in obj)
            {
                Guid ObjectiveGUID = Guid.NewGuid();
                codePmdObjectives.Add(new codePmdObjective
                {
                    ObjectiveGUID = ObjectiveGUID,
                    Active = true
                });
                codePmdObjectiveLanguages.Add(new codePmdObjectiveLanguage
                {
                    ObjectiveLanguageGUID = Guid.NewGuid(),
                    ObjectiveGUID = ObjectiveGUID,
                    LanguageID = "EN",
                    ObjectiveDescription = item.ObjectiveEN,
                    Active = true
                });
                codePmdObjectiveLanguages.Add(new codePmdObjectiveLanguage
                {
                    ObjectiveLanguageGUID = Guid.NewGuid(),
                    ObjectiveGUID = ObjectiveGUID,
                    LanguageID = "AR",
                    ObjectiveDescription = item.ObjectiveAR,
                    Active = true
                });
            }

            foreach (var item in tempCodes)
            {
                if (codePmdOutputLanguages.Where(x => x.OutputDescription == item.OutputEN).Count() > 0)
                {
                    continue;
                }
                Guid ObjectiveGUID = (from a in codePmdObjectiveLanguages where a.ObjectiveDescription == item.ObjectiveEN select a.ObjectiveGUID).FirstOrDefault();
                Guid OutputGUID = Guid.NewGuid();
                codePmdOutputs.Add(new codePmdOutput
                {
                    OutputGUID = OutputGUID,
                    ObjectiveGUID = ObjectiveGUID,
                    Active = true
                });
                codePmdOutputLanguages.Add(new codePmdOutputLanguage
                {
                    OutputLanguageGUID = Guid.NewGuid(),
                    OutputGUID = OutputGUID,
                    LanguageID = "EN",
                    OutputDescription = item.OutputEN,
                    Active = true
                });
                codePmdOutputLanguages.Add(new codePmdOutputLanguage
                {
                    OutputLanguageGUID = Guid.NewGuid(),
                    OutputGUID = OutputGUID,
                    LanguageID = "AR",
                    OutputDescription = item.OutputAR,
                    Active = true
                });
            }

            foreach (var item in tempCodes)
            {
                if (codePmdIndicatorLanguages.Where(x => x.IndicatorDescription == item.IndicatorEN).Count() > 0)
                {
                    continue;
                }
                Guid OutputGUID = (from a in codePmdOutputLanguages where a.OutputDescription == item.OutputEN select a.OutputGUID).FirstOrDefault();
                Guid IndicatorGUID = Guid.NewGuid();
                codePmdIndicators.Add(new codePmdIndicator
                {
                    IndicatorGUID = IndicatorGUID,
                    OutputGUID = OutputGUID,
                    Active = true
                });
                codePmdIndicatorLanguages.Add(new codePmdIndicatorLanguage
                {
                    IndicatorLanguageGUID = Guid.NewGuid(),
                    IndicatorGUID = IndicatorGUID,
                    LanguageID = "EN",
                    IndicatorDescription = item.IndicatorEN,
                    IndicatorGuidance = item.GuidanceEN,
                    Active = true
                });
                codePmdIndicatorLanguages.Add(new codePmdIndicatorLanguage
                {
                    IndicatorLanguageGUID = Guid.NewGuid(),
                    IndicatorGUID = IndicatorGUID,
                    LanguageID = "AR",
                    IndicatorDescription = item.IndicatorAR,
                    IndicatorGuidance = item.GuidanceAR,
                    Active = true
                });
            }

            //DbPMD.codePmdObjectives.AddRange(codePmdObjectives);
            //DbPMD.codePmdObjectiveLanguages.AddRange(codePmdObjectiveLanguages);
            //DbPMD.codePmdOutputs.AddRange(codePmdOutputs);
            //DbPMD.codePmdOutputLanguages.AddRange(codePmdOutputLanguages);
            //DbPMD.codePmdIndicators.AddRange(codePmdIndicators);
            //DbPMD.codePmdIndicatorLanguages.AddRange(codePmdIndicatorLanguages);
            //DbPMD.SaveChanges();
            return null;
        }
        public ActionResult InsertStatuses()
        {
            FileInfo locationInfo = new FileInfo(Server.MapPath("~/Areas/PMD/Templates/status.xlsx"));
            List<codePmdObjectiveStatus> codePmdObjectiveStatuses = new List<codePmdObjectiveStatus>();
            List<codePmdObjectiveStatusLanguage> codePmdObjectiveStatusLanguages = new List<codePmdObjectiveStatusLanguage>();


            using (var package = new ExcelPackage(locationInfo))
            {
                ExcelWorksheet workSheet = package.Workbook.Worksheets["Sheet1"];
                int totalRows = workSheet.Dimension.End.Row;
                for (int i = 1; i <= totalRows; i++)
                {
                    string statusEN = workSheet.Cells["A" + i].Value.ToString();
                    string statusAR = workSheet.Cells["B" + i].Value.ToString();
                    Guid PKGUID = Guid.NewGuid();
                    codePmdObjectiveStatuses.Add(new codePmdObjectiveStatus
                    {
                        PmdObjectiveStatusGUID = PKGUID,
                        Active = true,

                    });
                    codePmdObjectiveStatusLanguages.Add(new codePmdObjectiveStatusLanguage
                    {
                        ObjectiveStatusLanguageGUID = Guid.NewGuid(),
                        PmdObjectiveStatusGUID = PKGUID,
                        LanguageID = "EN",
                        StatusDescription = statusEN.Trim(),
                        Active = true,
                    });
                    codePmdObjectiveStatusLanguages.Add(new codePmdObjectiveStatusLanguage
                    {
                        ObjectiveStatusLanguageGUID = Guid.NewGuid(),
                        PmdObjectiveStatusGUID = PKGUID,
                        LanguageID = "AR",
                        StatusDescription = statusAR.Trim(),
                        Active = true,
                    });

                }
            }

            DbPMD.codePmdObjectiveStatus.AddRange(codePmdObjectiveStatuses);
            DbPMD.codePmdObjectiveStatusLanguage.AddRange(codePmdObjectiveStatusLanguages);
            //DbPMD.SaveChanges();
            return null;
        }
        //public ActionResult InsertStatus()
        //{
        //    var objectives = (from a in DbPMD.codePmdObjectiveLanguages where a.LanguageID == "EN" select a).Distinct().ToList();
        //    List<dataPartnerMonitoringObjectiveStatus> dataPartnerMonitoringObjectiveStatuses = new List<dataPartnerMonitoringObjectiveStatus>();
        //    var objectiveStatusesForShelter = (from a in DbPMD.codePmdObjectiveStatusLanguage
        //                                       where a.LanguageID == "EN"
        //                                       && a.StatusDescription != "Ongoing"
        //                                       && a.StatusDescription != "Assessed"
        //                                       select a).Distinct().ToList();

        //    var objectiveStatusesForOther = (from a in DbPMD.codePmdObjectiveStatusLanguage
        //                                     where a.LanguageID == "EN"
        //                                     &&
        //                                     (a.StatusDescription == "Ongoing"
        //                                      || a.StatusDescription == "Assessed"
        //                                      || a.StatusDescription == "Completed"
        //                                      || a.StatusDescription == "Cancelled"
        //                                       )
        //                                     select a).Distinct().ToList();

        //    foreach (var item in objectives)
        //    {
        //        if (item.ObjectiveDescription == "Shelter and infrastructure established, improved and maintained")
        //        {
        //            foreach (var status in objectiveStatusesForShelter)
        //            {
        //                dataPartnerMonitoringObjectiveStatuses.Add(new dataPartnerMonitoringObjectiveStatus
        //                {
        //                    DataObjectiveStatusGUID = Guid.NewGuid(),
        //                    ObjectiveGUID = item.ObjectiveGUID,
        //                    ObjectiveStatusGUID = status.PmdObjectiveStatusGUID,
        //                    Active = true,
        //                });
        //            }
        //        }
        //        else
        //        {
        //            foreach (var status in objectiveStatusesForOther)
        //            {
        //                dataPartnerMonitoringObjectiveStatuses.Add(new dataPartnerMonitoringObjectiveStatus
        //                {
        //                    DataObjectiveStatusGUID = Guid.NewGuid(),
        //                    ObjectiveGUID = item.ObjectiveGUID,
        //                    ObjectiveStatusGUID = status.PmdObjectiveStatusGUID,
        //                    Active = true,
        //                });
        //            }
        //        }
        //    }

        //    DbPMD.dataPartnerMonitoringObjectiveStatus.AddRange(dataPartnerMonitoringObjectiveStatuses);
        //    //DbPMD.SaveChanges();
        //    return null;
        //}

        public ActionResult InsertHealthUnits()
        {
            FileInfo locationInfo = new FileInfo(Server.MapPath("~/Areas/PMD/Templates/units.xlsx"));
            using (var package = new ExcelPackage(locationInfo))
            {
                ExcelWorksheet workSheet = null;
                try
                {
                    workSheet = package.Workbook.Worksheets[1];
                }
                catch (Exception)
                {
                    workSheet = package.Workbook.Worksheets[1];
                }

                //ExcelWorksheet workSheet = package.Workbook.Worksheets["Sheet1"];
                int totalRows = workSheet.Dimension.End.Row;

                List<codePmdUnitOfAchievement> codePmdUnitOfAchievements = new List<codePmdUnitOfAchievement>();
                List<codePmdUnitOfAchievementLanguages> codePmdUnitOfAchievementLanguages = new List<codePmdUnitOfAchievementLanguages>();

                for (int i = 1; i <= totalRows; i++)
                {
                    string UnitEN = workSheet.Cells["B" + i].Value.ToString();
                    string GuidanceEN = " ";
                    try
                    {
                        GuidanceEN = workSheet.Cells["C" + i].Value.ToString();
                    }
                    catch { }
                    string GuidanceAR = workSheet.Cells["D" + i].Value.ToString();
                    string UnitAR = workSheet.Cells["E" + i].Value.ToString();

                    Guid EntityPK = Guid.NewGuid();

                    codePmdUnitOfAchievements.Add(new codePmdUnitOfAchievement
                    {
                        UnitOfAchievementGUID = EntityPK,
                        UnitOfAchievementCategory = "Health",
                        Active = true
                    });
                    codePmdUnitOfAchievementLanguages.Add(new codePmdUnitOfAchievementLanguages
                    {
                        UnitOfAchievementLanguageGUID = Guid.NewGuid(),
                        UnitOfAchievementGUID = EntityPK,
                        LanguageID = "EN",
                        UnitOfAchievementDescription = UnitEN,
                        UnitOfAchievementGuidance = GuidanceEN
                    });
                    codePmdUnitOfAchievementLanguages.Add(new codePmdUnitOfAchievementLanguages
                    {
                        UnitOfAchievementLanguageGUID = Guid.NewGuid(),
                        UnitOfAchievementGUID = EntityPK,
                        LanguageID = "AR",
                        UnitOfAchievementDescription = UnitAR,
                        UnitOfAchievementGuidance = GuidanceAR
                    });
                }

                DbPMD.codePmdUnitOfAchievement.AddRange(codePmdUnitOfAchievements);
                DbPMD.codePmdUnitOfAchievementLanguages.AddRange(codePmdUnitOfAchievementLanguages);
                //DbPMD.SaveChanges();

            }

            return null;
        }
        public ActionResult InsertDomUnits()
        {
            FileInfo locationInfo = new FileInfo(Server.MapPath("~/Areas/PMD/Templates/domUnits.xlsx"));
            using (var package = new ExcelPackage(locationInfo))
            {
                ExcelWorksheet workSheet = null;
                try
                {
                    workSheet = package.Workbook.Worksheets[1];
                }
                catch (Exception)
                {
                    workSheet = package.Workbook.Worksheets[1];
                }

                //ExcelWorksheet workSheet = package.Workbook.Worksheets["Sheet1"];
                int totalRows = workSheet.Dimension.End.Row;

                List<codePmdUnitOfAchievement> codePmdUnitOfAchievements = new List<codePmdUnitOfAchievement>();
                List<codePmdUnitOfAchievementLanguages> codePmdUnitOfAchievementLanguages = new List<codePmdUnitOfAchievementLanguages>();

                for (int i = 1; i <= totalRows; i++)
                {
                    string UnitEN = workSheet.Cells["A" + i].Value.ToString();
                    string GuidanceEN = " ";
                    string GuidanceAR = " ";
                    string UnitAR = workSheet.Cells["A" + i].Value.ToString();

                    Guid EntityPK = Guid.NewGuid();

                    codePmdUnitOfAchievements.Add(new codePmdUnitOfAchievement
                    {
                        UnitOfAchievementGUID = EntityPK,
                        UnitOfAchievementCategory = "Domestic",
                        Active = true
                    });
                    codePmdUnitOfAchievementLanguages.Add(new codePmdUnitOfAchievementLanguages
                    {
                        UnitOfAchievementLanguageGUID = Guid.NewGuid(),
                        UnitOfAchievementGUID = EntityPK,
                        LanguageID = "EN",
                        UnitOfAchievementDescription = UnitEN,
                        UnitOfAchievementGuidance = GuidanceEN
                    });
                    codePmdUnitOfAchievementLanguages.Add(new codePmdUnitOfAchievementLanguages
                    {
                        UnitOfAchievementLanguageGUID = Guid.NewGuid(),
                        UnitOfAchievementGUID = EntityPK,
                        LanguageID = "AR",
                        UnitOfAchievementDescription = UnitAR,
                        UnitOfAchievementGuidance = GuidanceAR
                    });
                }

                DbPMD.codePmdUnitOfAchievement.AddRange(codePmdUnitOfAchievements);
                DbPMD.codePmdUnitOfAchievementLanguages.AddRange(codePmdUnitOfAchievementLanguages);
                //DbPMD.SaveChanges();

            }

            return null;
        }
    }
}