using AppsPortal.BaseControllers;
using AppsPortal.Data;
using AppsPortal.Models;
using AppsPortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Configuration;
using System.Web.Mvc;
using AppsPortal.Library;
using static AppsPortal.Library.DataTableNames;
using DAS_DAL.Model;
using EMT_DAL.Model;

namespace AppsPortal.Controllers
{
    public class DropDownListController : PortalBaseController
    {
        public JsonResult RemoteApplicationMenus(string PK1)
        {
            if (string.IsNullOrEmpty(PK1)) { return null; }
            var Result = DropDownList.ApplicationMenus(Guid.Parse(PK1));
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoteActions(string PK1)
        {
            if (string.IsNullOrEmpty(PK1)) return null;
            var Result = DropDownList.Actions(Guid.Parse(PK1));
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoteOrganizationInstanceDepartments(string PK1)
        {
            if (string.IsNullOrEmpty(PK1))
            {
                return null;
            }
            Guid OrganizationInstanceGUID = Guid.Parse(PK1);
            var Result = DropDownList.Departments(OrganizationInstanceGUID);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoteWarehouseRequestTypes(string PK1)
        {
            if (string.IsNullOrEmpty(PK1))
            {
                return null;
            }
            Guid PartyTypeGUID = Guid.Parse(PK1);
            if (PartyTypeGUID == WarehouseRequestSourceTypes.Staff)
            {
                var Result = DropDownList.WarehouseStaff(PartyTypeGUID);
                return Json(Result, JsonRequestBehavior.AllowGet);
            }
            else if (PartyTypeGUID == WarehouseRequestSourceTypes.Warehouse)
            {
                var Result = DropDownList.Warehouses();
                return Json(Result, JsonRequestBehavior.AllowGet);
            }
            else if (PartyTypeGUID == WarehouseRequestSourceTypes.OtherRequester)
            {
                var Result = DropDownList.OtherRequester();
                return Json(Result, JsonRequestBehavior.AllowGet);
            }
            else if (PartyTypeGUID == WarehouseRequestSourceTypes.Vehicle)
            {
                var Result = DropDownList.WarehousesVehicals();
                return Json(Result, JsonRequestBehavior.AllowGet);
            }
            else if (PartyTypeGUID == WarehouseRequestSourceTypes.Partner)
            {
                var Result = DropDownList.WarehousePartners();
                return Json(Result, JsonRequestBehavior.AllowGet);
            }

            return null;

        }
        public JsonResult RemoteConsumableWarehouseRequestTypes(string PK1)
        {
            //zz
            if (string.IsNullOrEmpty(PK1))
            {
                return null;
            }
            Guid PartyTypeGUID = Guid.Parse(PK1);
            if (PartyTypeGUID == WarehouseConsumableRequestSourceTypes.Printers)
            {
                var Result = DropDownList.WarehousePrinters();
                return Json(Result, JsonRequestBehavior.AllowGet);
            }
            else if (PartyTypeGUID == WarehouseConsumableRequestSourceTypes.Warehouse)
            {
                var Result = DropDownList.Warehouses();
                return Json(Result, JsonRequestBehavior.AllowGet);
            }


            return null;

        }
        public JsonResult RemoteWarehouseConsumableModelsByWarehouse(string PK1)
        {
            if (string.IsNullOrEmpty(PK1))
            {
                return null;
            }
            Guid warehouseGUID = Guid.Parse(PK1);

            var Result = DropDownList.WarehouseConsumableModels(warehouseGUID);
            return Json(Result, JsonRequestBehavior.AllowGet);



            return null;

        }
        public JsonResult RemoteWarehouseItems(string PK1)
        {
            if (string.IsNullOrEmpty(PK1))
            {
                return null;
            }
            Guid WarehouseItemClassificationUID = Guid.Parse(PK1);
            var Result = DropDownList.WarehouseItems(WarehouseItemClassificationUID);
            return Json(Result, JsonRequestBehavior.AllowGet);


            return null;

        }


        //public JsonResult RemoteWarehouseEntrySourceTypes(string PK1)
        //{
        //    if (string.IsNullOrEmpty(PK1))
        //    {
        //        return null;
        //    }
        //    Guid PartyTypeGUID = Guid.Parse(PK1);
        //    if (PartyTypeGUID == WarehouseRequestSourceTypes.Staff)
        //    {
        //        var Result = DropDownList.UserStaff(PartyTypeGUID);
        //        return Json(Result, JsonRequestBehavior.AllowGet);
        //    }
        //    else if (PartyTypeGUID == WarehouseRequestSourceTypes.Warehouse)
        //    {
        //        var Result = DropDownList.Warehouses();
        //        return Json(Result, JsonRequestBehavior.AllowGet);
        //    }

        //    return null;

        //}

        public JsonResult RemoteOperationDepartments(string PK1, string PK2)
        {
            if (string.IsNullOrEmpty(PK1) || string.IsNullOrEmpty(PK2))
            {
                return null;
            }
            Guid OrganizationInstanceGUID = Guid.Parse(PK1);
            Guid ServiceHistoryGUID = Guid.Parse(PK2);
            Guid OrganizationGUID = DbCMS.userServiceHistory.Find(ServiceHistoryGUID).OrganizationGUID;
            var Result = DropDownList.Departments(OrganizationInstanceGUID);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoteActionsCategories(string PK1, bool PK2 = false)
        {
            if (string.IsNullOrEmpty(PK1)) return null;
            Guid ApplicationGUID = Guid.Parse(PK1);
            var Result = DropDownList.ActionsCategories(ApplicationGUID, PK2);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoteDutyStation(string PK1)
        {
            if (string.IsNullOrEmpty(PK1)) return null;
            Guid InstanceGUID = Guid.Parse(PK1);
            var Result = DropDownList.DutyStations(InstanceGUID);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoteModelDeterminant(string PK1)
        {
            if (string.IsNullOrEmpty(PK1)) return null;
            Guid ItemModelWarehouseGUID = Guid.Parse(PK1);
            var Result = DropDownList.ModelDeterminants(ItemModelWarehouseGUID);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoteFactorsForDependency(string PK1, string PK2)
        {
            if (string.IsNullOrEmpty(PK1) || string.IsNullOrEmpty(PK2)) return null;
            Guid ActionCategoryGUID = Guid.Parse(PK1);
            Guid FactorGUID = Guid.Parse(PK2);
            var Result = DropDownList.FactorsForDependency(ActionCategoryGUID, FactorGUID);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoteOperationsByBureau(string PK1)
        {
            if (string.IsNullOrEmpty(PK1)) return null;
            Guid BureauGUID = Guid.Parse(PK1);
            var Result = DropDownList.Operations(BureauGUID);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoteOrganizationsByOperations(string PK1)
        {
            if (string.IsNullOrEmpty(PK1)) return null;
            var Result = DropDownList.OrganizationsByOperations(Portal.CSVToGUIDList(PK1));
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoteGovernoratesByOperations(string PK1)
        {
            if (string.IsNullOrEmpty(PK1)) return null;

            var Result = DropDownList.Governorates(Portal.CSVToGUIDList(PK1));
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoteLocations(string PK1, string PK2)
        {
            if (string.IsNullOrEmpty(PK1)) return null;
            var Result = DropDownList.Locations(Guid.Parse(PK1), int.Parse(PK2));
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoteCountryLocation(string PK1)
        {
            if (string.IsNullOrEmpty(PK1)) return null;
            var Result = DropDownList.Locations(Guid.Parse(PK1));
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoteLocationsByLocationType(string PK1)
        {
            if (string.IsNullOrEmpty(PK1)) return null;
            var Result = DropDownList.LocationsByLocationType(Guid.Parse(PK1), Guid.Parse("FCF6CDE3-329A-42A2-82A2-D99909949F82"));
            return Json(Result, JsonRequestBehavior.AllowGet);
            //
        }
        public JsonResult RemoteLocationsByGovernorate(string PK1)
        {
            if (string.IsNullOrEmpty(PK1)) return null;
            var Result = DropDownList.LocationsByCountryType(PK1.Split(','), Guid.Parse("C970DDF5-31E5-47A1-BE76-AF15833D4E6A"));
            return Json(Result, JsonRequestBehavior.AllowGet);
            //uz6d-c970ddf5-31e5-47a1-be76-af15833d4e6a
        }

        public JsonResult RemoteOrganizationInstanceDutyStation(string PK1)
        {
            if (string.IsNullOrEmpty(PK1)) return null;
            var Result = DropDownList.DutyStations(Guid.Parse(PK1));
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoteOrganizationInstanceByPartnerCenter(string PK1)
        {
            if (string.IsNullOrEmpty(PK1)) return null;
            var Result = DropDownList.PartnerCentersByDutyStation(PK1);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemotePartnerCenterByOrganizationInstanceByDutyStation(string PK1)
        {
            if (string.IsNullOrEmpty(PK1)) return null;
            var Result = DropDownList.PartnerCenterByOrganizationInstanceByDutyStation(PK1.Split('|')[0], PK1.Split('|')[1]);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RemoteSyriaDutyStation(string PK1)
        {
            // if (string.IsNullOrEmpty(PK1)) return null;
            var Result = DropDownList.SyriaDutyStations();
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoteSyriaDutyStationForPCR(string PK1)
        {
            // if (string.IsNullOrEmpty(PK1)) return null;
            var Result = DropDownList.SyriaDutyStationsForPCR();
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoteOrganizationsInstancesByOrganization(string PK1)
        {
            if (string.IsNullOrEmpty(PK1)) return null;
            var Result = DropDownList.OrganizationsInstancesByOrganization(Guid.Parse(PK1));
            return Json(Result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RemoteOrganizationsInstancesByDepartmentConfiguration(string PK1)
        {
            if (string.IsNullOrEmpty(PK1)) return null;
            var Result = DropDownList.RemoteOrganizationsInstancesByDepartmentConfiguration(Guid.Parse(PK1));
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoteOrganizationsInstancesDepartmentStaff(string PK1)
        {
            if (Guid.Parse(PK1) == Guid.Empty) return null;

            var Result = DropDownList.OrganizationsInstancesDepartmentStaff(Guid.Parse(PK1));
            return Json(Result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RemoteAppointmentTypeCalender(string PK1)
        {
            if (Guid.Parse(PK1) == Guid.Empty) return null;

            var Result = DropDownList.AppointmentTypeCalender(Guid.Parse(PK1));
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoteAppointmentTypesByDepartment(string PK1)
        {
            if (Guid.Parse(PK1) == Guid.Empty) return null;

            var Result = DropDownList.AppointmentTypesByDepartment(Guid.Parse(PK1));
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoteFindUserCurrentProfile([Bind(Prefix = "SearchKey")] string FullName)
        {
            //Return UserProfileGUID and Full Name.
            var Result = (from a in DbCMS.userProfiles.Where(x => x.Active)
                          join b in DbCMS.userServiceHistory.Where(x => x.Active) on a.ServiceHistoryGUID equals b.ServiceHistoryGUID
                          join c in DbCMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && (x.FirstName + " " + x.Surname).Contains(FullName)) on b.UserGUID equals c.UserGUID
                          orderby a.FromDate descending
                          select new
                          {
                              id = a.UserProfileGUID,
                              text = c.FirstName + " " + c.Surname + " (" + b.EmailAddress + ")"
                          }).ToList();

            return Json(new { items = Result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoteFindUserCurrentGUID([Bind(Prefix = "SearchKey")] string FullName)
        {
            //Return UserProfileGUID and Full Name.
            var Result = (from a in DbCMS.userProfiles.Where(x => x.Active)
                          join b in DbCMS.userServiceHistory.Where(x => x.Active) on a.ServiceHistoryGUID equals b.ServiceHistoryGUID
                          join c in DbCMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.FirstName.Contains(FullName) || x.Surname.Contains(FullName)) on b.UserGUID equals c.UserGUID
                          select new
                          {
                              id = c.UserGUID,
                              text = c.FirstName + " " + c.Surname + " (" + b.EmailAddress + ")"
                          }).ToList();

            return Json(new { items = Result }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RemoteFindUserBy([Bind(Prefix = "SearchKey")] string FullName)
        {
            //Return UserProfileGUID and Full Name.
            Guid staffIsActiveGUID = Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E3611");
            var Result = (from a in DbCMS.StaffCoreData.Where(x => x.Active)
                          join b in DbCMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.FirstName.Contains(FullName) || x.Surname.Contains(FullName)) on a.UserGUID equals b.UserGUID
                          where a.StaffStatusGUID == staffIsActiveGUID
                          select new
                          {
                              id = a.UserGUID,
                              text = b.FirstName + " " + b.Surname + " (" + a.EmailAddress + ")"
                          }).ToList();
            List<Guid> UserGuids = Result.Select(x => x.id).ToList();

            var Result1 = (from a in DbCMS.userProfiles.Where(x => x.Active)
                          join b in DbCMS.userServiceHistory.Where(x=> !UserGuids.Contains(x.UserGUID)) on a.ServiceHistoryGUID equals b.ServiceHistoryGUID
                          join c in DbCMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.FirstName.Contains(FullName) || x.Surname.Contains(FullName)) on b.UserGUID equals c.UserGUID
                          orderby a.FromDate descending
                          select new
                          {
                              id = c.UserGUID,
                              text = c.FirstName + " " + c.Surname + " (" + b.EmailAddress + ")"
                          }).ToList();
            Result.AddRange(Result1);
            return Json(new { items = Result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoteFindLocationBy([Bind(Prefix = "SearchKey")] string SearchKey)
        {
            var Result = (from a in DbCMS.codeLocations.Where(x => x.Active && x.LocationlevelID == 4)
                          join b in DbCMS.codeLocationsLanguages.Where(x => x.LanguageID == LAN && x.Active && x.LocationDescription.Contains(SearchKey)) on a.LocationGUID equals b.LocationGUID
                          orderby b.LocationDescription
                          select new
                          {
                              id = a.LocationGUID.ToString(),
                              Text = b.LocationDescription
                          }).ToList();

            return Json(new { items = Result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoteFindUser([Bind(Prefix = "SearchKey")] string FullName)
        {
            //Return UserProfileGUID and Full Name.
            var Result = (from a in DbCMS.userProfiles.Where(x => x.Active)
                          join b in DbCMS.userServiceHistory.Where(x => x.Active) on a.ServiceHistoryGUID equals b.ServiceHistoryGUID
                          join c in DbCMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.FirstName.Contains(FullName) || x.Surname.Contains(FullName)) on b.UserGUID equals c.UserGUID
                          orderby a.FromDate descending
                          select new
                          {
                              id = a.UserProfileGUID,
                              text = c.FirstName + " " + c.Surname
                          }).ToList();

            return Json(new { items = Result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoteFindOrganization([Bind(Prefix = "SearchKey")] string OrganizationName)
        {
            var Result = (from a in DbCMS.codeOrganizationsLanguages.Where(x => x.Active && x.LanguageID == LAN)
                          join b in DbCMS.codeOrganizations.Where(x => x.Active) on a.OrganizationGUID equals b.OrganizationGUID
                          where a.OrganizationDescription.Contains(OrganizationName) || b.OrganizationShortName.Contains(OrganizationName)
                          select new
                          {
                              id = a.OrganizationGUID,
                              text = a.OrganizationDescription
                          }).ToList();

            return Json(new { items = Result }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult UserDDL([Bind(Prefix = "SearchKey")] string PersonName)
        {

            string MediaServer = WebConfigurationManager.AppSettings["MediaURL"] + "Users/ProfilePhotos/";

            var Result = (from a in DbCMS.userServiceHistory.Where(x => x.Active)
                          join b in DbCMS.userProfiles.Where(x => x.Active) on a.ServiceHistoryGUID equals b.ServiceHistoryGUID
                          join c in DbCMS.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN && (x.FirstName + " " + x.Surname).Contains(PersonName)) on a.UserGUID equals c.UserGUID
                          join d in DbCMS.codeJobTitlesLanguages.Where(x => x.Active && x.LanguageID == LAN) on b.JobTitleGUID equals d.JobTitleGUID into  LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          join e in DbCMS.codeOrganizationsInstancesLanguages.Where(x => x.Active && x.LanguageID == LAN) on b.OrganizationInstanceGUID equals e.OrganizationInstanceGUID into LJ2
                          from R2 in LJ2.DefaultIfEmpty()
                          select new UserDropDownList
                          {
                              id = a.UserGUID,
                              name = c.FirstName + " " + c.Surname,
                              PhotoPath = a.userAccounts.HasPhoto.Value ? MediaServer + UserGUID.ToString() + ".jpg" : "/Assets/Images/img.png",
                              JobTitle = R1.JobTitleDescription,
                              Organization = R2.OrganizationInstanceDescription
                          }).ToList();

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoteCountryCodes()
        {
            var Result = DropDownList.CountriesPhoneCode();
            return Json(Result, JsonRequestBehavior.AllowGet);

        }

        public JsonResult RemotePartnerCentersByOrganizationInstance(string PK1)
        {
            var Result = DropDownList.PartnerCentersByOrganizationInstance(PK1);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RemoteAggregationAge(string PK1)
        {
            if (string.IsNullOrEmpty(PK1)) return null;
            var Result = DropDownList.AggregationAge(PK1);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoteAggregationProfile(string PK1)
        {
            if (string.IsNullOrEmpty(PK1)) return null;
            var Result = DropDownList.AggregationProfile(PK1);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoteCategoryPartnerReportLevel2(string PK1)
        {
            if (string.IsNullOrEmpty(PK1)) return null;
            var Result = DropDownList.CategoryPartnerReportLevel2(Guid.Parse(PK1));
            return Json(Result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RemoteCategoryPartnerReportLevel3(string PK1)
        {
            if (string.IsNullOrEmpty(PK1)) return null;
            var Result = DropDownList.CategoryPartnerReportLevel3(Guid.Parse(PK1));
            return Json(Result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RemoteCategoryPartnerReportLevelString2(string PK1)
        {
            if (string.IsNullOrEmpty(PK1)) return null;
            var Result = DropDownList.CategoryPartnerReportLevelString2(Guid.Parse(PK1));
            return Json(Result, JsonRequestBehavior.AllowGet);
        }
        //
        public JsonResult RemoteParentCategoryPartnerReport(string PK1)
        {
            var Result = DropDownList.ParentCategoryPartnerReport(PK1);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemotePartyNameByPartyType(string PK1)
        {
            var Result = DropDownList.ParentCategoryPartnerReport(PK1);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemotePrinterConfigurationOid(string PK1)
        {
            var Result = DropDownList.PrinterOidsIsImport(Guid.Parse(PK1));
            return Json(Result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RemoteOfficesByDutyStation(string PK1)
        {
            var Result = DropDownList.OfficesByDutyStation(Guid.Parse(PK1));
            return Json(Result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RemoteDistricts(string PK1)
        {
            if (string.IsNullOrEmpty(PK1))
            {
                return null;
            }
            Guid GovernorateGUID = Guid.Parse(PK1);
            var Result = DropDownList.Districts(GovernorateGUID);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RemoteSubDistricts(string PK1)
        {
            if (string.IsNullOrEmpty(PK1))
            {
                return null;
            }
            Guid DistrictGUID = Guid.Parse(PK1);
            var Result = DropDownList.SubDistricts(DistrictGUID);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoteCommunities(string PK1)
        {
            if (string.IsNullOrEmpty(PK1))
            {
                return null;
            }
            Guid SubDistrictGUID = Guid.Parse(PK1);
            var Result = DropDownList.Communities(SubDistrictGUID);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        #region EMT 

        public JsonResult RemoteMedicalPharmacyByOrganizationInsatance(string PK1)
        {
            if (string.IsNullOrEmpty(PK1))
            {
                return null;
            }
            Guid OrganizationInstanceGUID = Guid.Parse(PK1);
            var Result = DropDownList.MedicalPharmacyByOrganizationInsatance(OrganizationInstanceGUID, true);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoteMedicalItemInputDetails(string PK1)
        {
            if (string.IsNullOrEmpty(PK1))
            {
                return null;
            }
            var Result = DropDownList.MedicalItemInputDetails(PK1);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        //
        public JsonResult RemoteMedicalItemTransferDetails(string PK1)
        {
            if (string.IsNullOrEmpty(PK1))
            {
                return null;
            }
            ;
            var Result = DropDownList.MedicalItemTransferDetails(PK1);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoteFindBeneficiaryBy([Bind(Prefix = "SearchKey")] string SearchKey)
        {
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.MedicalBeneficiary.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //UNHCR List
            AuthorizedList.Add("E156C022-EC72-4A5A-BE09-163BD85C68EF");
            var Result = (from a in DbCMS.dataMedicalBeneficiary.Where(x => x.Active && (x.UNHCRNumber.StartsWith(SearchKey) || x.IDNumber.StartsWith(SearchKey) || x.RefugeeFullName.Contains(SearchKey))).Where(x => AuthorizedList.Contains(x.OrganizationInstanceGUID.ToString()))

                          select new
                          {
                              id = a.MedicalBeneficiaryGUID.ToString(),
                              Text = "(UNHCR/Card) Number: " + (a.UNHCRNumber == null ? a.IDNumber : a.UNHCRNumber) + " ,Full Name:" + a.RefugeeFullName
                          }).ToList();

            return Json(new { items = Result }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RemotePharmacyByOrganizationInstance(string PK1)
        {
            if (string.IsNullOrEmpty(PK1)) return null;
            var Result = DropDownList.PharmacyByOrganizationInstance(PK1);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoteItemsByManufacturer(string PK1)
        {
            if (string.IsNullOrEmpty(PK1))
            {
                return null;
            }

            var Result = DropDownList.MedicalItemsByManufacturer(PK1);
            return Json(Result, JsonRequestBehavior.AllowGet);

        }

        public JsonResult RemoteMedicalPharmacyGrorp()
        {
            var Result = DropDownList.MedicalPharmacyGrorp();
            return Json(Result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RemoteMedicalItemGrorp()
        {
            var Result = DropDownList.MedicalItemGroupSequance();
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoteItemsByTreatment(string PK1)
        {
            if (string.IsNullOrEmpty(PK1))
            {
                return null;
            }

            var Result = DropDownList.MedicalItemsByTreatment(PK1);
            return Json(Result, JsonRequestBehavior.AllowGet);

        }

        public JsonResult RemoteItemsByTreatmentByPharmacologicalForm(string PK1)
        {
            if (string.IsNullOrEmpty(PK1))
            {
                return null;
            }

            var Result = DropDownList.MedicalItemsByTreatmentByPharmacologicalForm(PK1.Split('|')[0], PK1.Split('|')[1], PK1.Split('|')[2]);
            return Json(Result, JsonRequestBehavior.AllowGet);

        }
        public JsonResult RemoteFindByBrandName([Bind(Prefix = "SearchKey")] string SearchKey)
        {
            var Result = (from a in DbCMS.spMedicalItem("EN").Where(x => x.Active && (x.BrandName.Contains(SearchKey)))
                          select new
                          {
                              id = a.MedicalItemGUID.ToString(),
                              Text = a.BrandName
                          }).ToList();

            return Json(new { items = Result }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RemoteFindMedicalItemsBy(string PK1)
        {
            using (var DbEMT = new EMTEntities())
            {
                var Result = (from a in DbEMT.spMedicalItem("EN").Where(x => x.Active && (x.BrandName.ToLower().Contains(PK1.ToLower())))
                              orderby a.BrandName
                              select new
                              {
                                  Value = a.Sequance.ToString(),
                                  Text = a.BrandName + " - Dose: " + a.DoseQuantity,
                                  Group = a.MedicalManufacturerGUID + "," + a.MedicalTreatmentGUID
                              }).ToList();
                SelectList listItems = new SelectList(Result, "Value", "Text", "Group", -1);
                return Json(listItems, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region DAS

        public JsonResult RemoteFindFileNumberBy([Bind(Prefix = "SearchKey")] string SearchKey)
        {
            var Result = (from a in DbDAS.dataFile.Where(x => x.Active && x.FileNumber.StartsWith(SearchKey))
                          select new
                          {
                              id = a.FileGUID.ToString(),
                              text = a.FileNumber
                          }).Take(10).OrderBy(x => x.text).ToList();

            return Json(new { items = Result }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RemoteDASDocumentTags(string PK1)
        {

            if (string.IsNullOrEmpty(PK1))
            {
                return null;
            }
            string admin1Pcode = PK1;
            var Result = DropDownList.DASDocumentTags(Guid.Parse(PK1));
            return Json(Result, JsonRequestBehavior.AllowGet);
        }
        //new das
        public JsonResult RemoteCabientsShelfs(string PK1)
        {

            if (string.IsNullOrEmpty(PK1))
            {
                return null;
            }
            string admin1Pcode = PK1;
            var Result = DropDownList.DASDocumentCabinetsShelfs(Guid.Parse(PK1));
            return Json(Result, JsonRequestBehavior.AllowGet);
        }
        //new das
        public JsonResult RemoteTemplateDocumentTypes(string PK1)
        {

            if (string.IsNullOrEmpty(PK1))
            {
                return null;
            }
            string admin1Pcode = PK1;
            var Result = DropDownList.DASTemplateDocumentTypeByPK(Guid.Parse(PK1));
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoteTemplateOwnerTypes(string PK1)
        {
            if (string.IsNullOrEmpty(PK1))
            {
                return null;
            }
            DASEntities DbDAS = new DASEntities();
            Guid _guid = Guid.Parse(PK1);
            var _template = DbDAS.codeDASTemplateTypeDocument.Where(x => x.TemplateTypeDocumentGUID == _guid).Select(x => x.codeDASTemplateType.ReferenceLinkTypeGUID).FirstOrDefault();
            Guid PartyTypeGUID = (Guid)_template;
            if (PartyTypeGUID == DASTemplateOwnerTypes.Staff)
            {
                var Result = DropDownList.DASAdminHRStaff();
                return Json(Result, JsonRequestBehavior.AllowGet);
            }
            else if (PartyTypeGUID == DASTemplateOwnerTypes.Refugee)
            {
                var Result = DropDownList.DASDataRefugeeFileTop();
                return Json(Result, JsonRequestBehavior.AllowGet);
            }
            //else if (PartyTypeGUID == DASTemplateOwnerTypes.Agreement)
            //{
            //    var Result = DropDownList.OtherRequester();
            //    return Json(Result, JsonRequestBehavior.AllowGet);
            //}
            //else if (PartyTypeGUID == DASTemplateOwnerTypes.Assets)
            //{
            //    var Result = DropDownList.WarehousesVehicals();
            //    return Json(Result, JsonRequestBehavior.AllowGet);
            //}


            return null;

        }
        public JsonResult RemoteDocumentCustodianType(string PK1)
        {
            if (string.IsNullOrEmpty(PK1))
            {
                return null;
            }
            Guid PartyTypeGUID = Guid.Parse(PK1);
            if (PartyTypeGUID == DASDocumentCustodianType.Staff)
            {
                var Result = DropDownList.WarehouseStaff(PartyTypeGUID);
                return Json(Result, JsonRequestBehavior.AllowGet);
            }
            else if (PartyTypeGUID == DASDocumentCustodianType.UNIT)
            {
                var Result = DropDownList.DASDASDestinationUnit();
                return Json(Result, JsonRequestBehavior.AllowGet);
            }

            return null;

        }
        #endregion

        #region COV
        public JsonResult RemoteCOVDistricts(string PK1)
        {
            if (string.IsNullOrEmpty(PK1))
            {
                return null;
            }
            string admin1Pcode = PK1;
            var Result = DropDownList.COVDistricts(admin1Pcode);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoteCOVSubDistricts(string PK1)
        {
            if (string.IsNullOrEmpty(PK1))
            {
                return null;
            }
            string admin2Pcode = PK1;
            var Result = DropDownList.COVSubDistricts(admin2Pcode);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoteCOVCommunities(string PK1)
        {
            if (string.IsNullOrEmpty(PK1))
            {
                return null;
            }
            string admin3Pcode = PK1;
            var Result = DropDownList.COVCommunities(admin3Pcode);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoteCOVNeighborhoods(string PK1)
        {
            if (string.IsNullOrEmpty(PK1))
            {
                return null;
            }
            string admin4Pcode = PK1;
            var Result = DropDownList.COVNeighborhoods(admin4Pcode);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoteCOVOutputs(string PK1)
        {
            if (string.IsNullOrEmpty(PK1))
            {
                return null;
            }
            Guid ObjectiveGUID = Guid.Parse(PK1);
            var Result = DropDownList.COVOutputs(ObjectiveGUID);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoteCOVIndicators(string PK1)
        {
            if (string.IsNullOrEmpty(PK1))
            {
                return null;
            }
            Guid OutputGUID = Guid.Parse(PK1);
            var Result = DropDownList.COVIndicators(OutputGUID);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoteCOVIndicatorUnit(string PK1)
        {
            if (string.IsNullOrEmpty(PK1))
            {
                return null;
            }
            Guid IndicatorGUID = Guid.Parse(PK1);
            var Result = DropDownList.COVIndicatorUnit(IndicatorGUID);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region WMS
        public JsonResult RemoteSTIStaffItems(string PK1)
        {

            if (string.IsNullOrEmpty(PK1))
            {
                return null;
            }
            string admin1Pcode = PK1;
            var Result = DropDownList.StaffSTIItems(Guid.Parse(PK1));
            return Json(Result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RemoteCIMTOfficeDutyStation(string PK1)
        {

            if (string.IsNullOrEmpty(PK1))
            {
                return null;
            }
            string admin1Pcode = PK1;
            var Result = DropDownList.CIMTOfficeDutyStation(Guid.Parse(PK1));
            return Json(Result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RemoteWarehouseItemFeaturesCascade(string PK1)
        {
            if (string.IsNullOrEmpty(PK1)) return null;
            var Result = DropDownList.CodeWarehouseItemFeatureTypesValues(Guid.Parse(PK1));
            return Json(Result, JsonRequestBehavior.AllowGet);
            //uz6d-c970ddf5-31e5-47a1-be76-af15833d4e6a
        }

        #endregion

        #region GTP

        public JsonResult RemoteGTPCities(string PK1)
        {
            if (string.IsNullOrEmpty(PK1))
            {
                return null;
            }
            Guid CountryGUID = Guid.Parse(PK1);
            var Result = DropDownList.GTPCities(CountryGUID);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region OSA
        public JsonResult RemoteOfficeFloor(string PK1)
        {
            if (string.IsNullOrEmpty(PK1))
            {
                return null;
            }
            Guid guidGUID = Guid.Parse(PK1);
            var Result = DropDownList.OfficeFloorByOffice(guidGUID);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoteOfficeFloorRoom(string PK1)
        {
            if (string.IsNullOrEmpty(PK1))
            {
                return null;
            }
            Guid guidGUID = Guid.Parse(PK1);
            var Result = DropDownList.OfficeFloorRoomByOfficeFloor(guidGUID);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region PMD
        public JsonResult RemotePMDDistricts(string PK1)
        {
            if (string.IsNullOrEmpty(PK1))
            {
                return null;
            }
            string admin1Pcode = PK1;
            var Result = DropDownList.PMDDistricts(admin1Pcode);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemotePMDCommunitiesByAdmin1Pcode(string PK1)
        {
            if (string.IsNullOrEmpty(PK1))
            {
                return null;
            }
            string admin1Pcode = PK1;
            var Result = DropDownList.PMDCommunitiesByAdmin1Pcode(admin1Pcode);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }


      

        public JsonResult RemotePMDSubDistricts(string PK1)
        {
            if (string.IsNullOrEmpty(PK1))
            {
                return null;
            }
            string admin2Pcode = PK1;
            var Result = DropDownList.PMDSubDistricts(admin2Pcode);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemotePMDCommunities(string PK1)
        {
            if (string.IsNullOrEmpty(PK1))
            {
                return null;
            }
            string admin3Pcode = PK1;
            var Result = DropDownList.PMDCommunities(admin3Pcode);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemotePMDCommunityCenters(string PK1)
        {
            if (string.IsNullOrEmpty(PK1))
            {
                return null;
            }
            string admin4Pcode = PK1;
            var Result = DropDownList.PMDCommunityCenters(admin4Pcode);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemotePMDNeighborhoods(string PK1)
        {
            if (string.IsNullOrEmpty(PK1))
            {
                return null;
            }
            string admin4Pcode = PK1;
            var Result = DropDownList.PMDNeighborhoods(admin4Pcode);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemotePMDOutputs(string PK1)
        {
            if (string.IsNullOrEmpty(PK1))
            {
                return null;
            }
            Guid ObjectiveGUID = Guid.Parse(PK1);
            var Result = DropDownList.PMDOutputs(ObjectiveGUID);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemotePMDIndicators(string PK1)
        {
            if (string.IsNullOrEmpty(PK1))
            {
                return null;
            }
            Guid OutputGUID = Guid.Parse(PK1);
            var Result = DropDownList.PMDIndicators(OutputGUID);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemotePMDObjectiveStatuses(string PK1)
        {
            if (string.IsNullOrEmpty(PK1))
            {
                return null;
            }
            Guid ObjectiveGUID = Guid.Parse(PK1);
            var Result = DropDownList.PMDObjectiveStatuses(ObjectiveGUID);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemotePMDWarehouse(string PK1)
        {
            if (string.IsNullOrEmpty(PK1))
            {
                return null;
            }
            using (var DbPMD = new PMD_DAL.Model.PMDEntities()) 
            {
                try
                {
                    string admin1Pcode = PK1.Split('|')[0];
                    Guid OrganizationInstanceGUID = Guid.Parse(PK1.Split('|')[1]);
                    var Result = DropDownList.PMDWarehouse(admin1Pcode, OrganizationInstanceGUID);
                    return Json(Result, JsonRequestBehavior.AllowGet);
                }
                catch { }
                return null;
            }
            
        }

        #endregion

        #region AHD
        public JsonResult RemoteUpdateDirectSupervisor(string PK1)
        {
            if (string.IsNullOrEmpty(PK1))
            {
                return null;
            }

            var Result = DropDownList.UpdateDirectSupervisor(PK1);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}