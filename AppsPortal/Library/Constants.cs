using System;
using System.Collections.Generic;

namespace AppsPortal.Library
{
    public class Constants
    {
        public const string NoPhotoTemplate = "/Assets/Images/NoPhotoTemplate.png";
        public const string UserProfilePhotoTemplate = "/Assets/Images/img.png";
    }

    public class LookupTables
    {
        public static Guid SecurityQuestions = Guid.Parse("00000000-0000-0000-0000-000000000001");
        public static Guid OrganizationTypes = Guid.Parse("00000000-0000-0000-0000-000000000002");
        public static Guid Gender = Guid.Parse("00000000-0000-0000-0000-000000000003");
        public static Guid DepartmentTypes = Guid.Parse("00000000-0000-0000-0000-000000000004");
        public static Guid ApplicationStatus = Guid.Parse("00000000-0000-0000-0000-000000000005");
        public static Guid LocationTypes = Guid.Parse("00000000-0000-0000-0000-000000000006");
        public static Guid ActivityStatus = Guid.Parse("00000000-0000-0000-0000-000000000007");
        public static Guid ManagerTypes = Guid.Parse("00000000-0000-0000-0000-000000000008");
        public static Guid OfficeTypes = Guid.Parse("00000000-0000-0000-0000-000000000009");
        public static Guid UNHCRBureaus = Guid.Parse("00000000-0000-0000-0000-000000000010");
        public static Guid WorkingDays = Guid.Parse("00000000-0000-0000-0000-000000000011");
        public static Guid LocationLevels = Guid.Parse("00000000-0000-0000-0000-000000000012");
        public static Guid MenusStatus = Guid.Parse("00000000-0000-0000-0000-000000000013");
        public static Guid FactorValuePurpose = Guid.Parse("00000000-0000-0000-0000-000000000014");
        public static Guid ManagerConfirmation = Guid.Parse("00000000-0000-0000-0000-000000000015");
        public static Guid PhoneType = Guid.Parse("00000000-0000-0000-0000-000000000024");
        public static Guid TokenColour = Guid.Parse("00000000-0000-0000-0000-000000000025");
        public static Guid MissionCategory = Guid.Parse("00000000-0000-0000-0000-000000000030");
        public static Guid OrganizationMissionType = Guid.Parse("00000000-0000-0000-0000-000000000031");
        public static Guid PPAType = Guid.Parse("F572674D-1688-406C-87FC-67B7F267125C");
        public static Guid PPAStatus = Guid.Parse("760B91E7-9BFC-41F5-A2AE-D8268116E13F");
        public static Guid PPAFileVersionStatus = Guid.Parse("A40EC252-622E-4FF1-9EF4-E328C7A3CEC5");
        public static Guid PPAFilesCategories = Guid.Parse("09FEAA17-8FF3-46D6-A446-FE8242FED332");
        public static Guid StaffLeaveType = Guid.Parse("c479de72-c6f3-49a9-9d3c-140ad1217177");
    
        public static Guid AHDRenewalStaffRenewalResidencyFormStatus = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3669");

        #region DAS
        public static Guid DASTemplateOwnerType = Guid.Parse("29591f83-32a4-49a5-b284-2a8efbc447c2");
        public static Guid DASSoftFieldType = Guid.Parse("d000c992-4871-4986-8f82-72c27ee2021a");
        public static Guid DASDocumentVerificaitonStatus = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7588");
        public static Guid DASDocumentCustodianType = Guid.Parse("09feaa17-8ff3-46d6-a446-fe8242fed382");
        public static Guid DASproGresSiteOwner = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3133");
        #endregion

        #region WMS
        public static Guid WarehouseConsumableRequsterTypes = Guid.Parse("d5e2a4df-c67e-4f4d-a242-7e33c381e789");
        public static Guid WarehouseRequsterTypes = Guid.Parse("d5e2a4df-c67e-4f4d-a242-7e33c381ec11");
        public static Guid WarehouseItemReservingType = Guid.Parse("2e8646e2-ccec-447e-840e-2a43686f498a");
        public static Guid WarehouseEntrySourceTypes = Guid.Parse("d5e2a4df-c67e-4f4d-a242-7e33c381ec99");
        public static Guid WarehouseModelDamagedTypes = Guid.Parse("0c44822f-a898-476d-b291-caf1b0551ac7");
        public static Guid WarehousePorpouseofUse = Guid.Parse("8A2AE01E-0F57-4178-B05E-E3021A2301A9");
        public static Guid WarehouseFlowType = Guid.Parse("0c44822f-a898-476d-b291-caf1b055aac6");
        public static Guid ModelDeterminants = Guid.Parse("D5E2A4DF-C67E-4F4D-A242-7E33C981EC11");
        public static Guid ReleaseTypes = Guid.Parse("D5E2A4DF-C67E-4F4D-A242-7E33C981EC12");
        public static Guid WarehouseNotifyStaff = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7854");
        public static Guid ItemStatus = Guid.Parse("675de853-151b-4c2f-93f4-da1434eee761");
        public static Guid InventoryStatus = Guid.Parse("675de853-151b-4c2f-93f4-da1434eee765");
        public static Guid SIMPackageSize = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7777");
        public static Guid PriceTypes = Guid.Parse("6ffd5528-a577-41e3-960a-013d152dbab3");
        public static Guid ModelDeliveryStatus = Guid.Parse("0C44822F-A898-476D-B291-CAF1B055AAC6");
        public static Guid ColorNames = Guid.Parse("A40EC252-622E-4FF1-9EF4-E328C7A3CEC4");
        public static Guid ItemRelationTypes = Guid.Parse("A40EC252-622E-4FF1-9EF4-E328C7A3CEA7");
        public static Guid ItemRelationMainKit = Guid.Parse("A40EC252-622E-4FF1-9EF4-E328C7A3CE14");
        public static Guid ItemRelationChildrenKit = Guid.Parse("A40EC252-622E-4FF1-9EF4-E328C7A3CE18");
        public static Guid ItemRequestTypeLongTerm = Guid.Parse("D5E2A4DF-C67E-4F4D-A242-7E33C385EC77");
        public static Guid ItemRequestTypeShortTerm = Guid.Parse("D5E2A4DF-C67E-4F4D-A242-7E33C583EC88");
        public static Guid ItemServiceStatus = Guid.Parse("a40ec252-622e-4ff1-9ef4-e328c7a3cec8");
        public static Guid ItemConditions = Guid.Parse("a40ec252-622e-4ff1-9ef4-e328c7a3cec9");
        public static Guid ItemVerificationStatus = Guid.Parse("d5e2a4df-c67e-4f4d-a242-7e33c381e733");
        public static Guid WarehoueItemDocumentType = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3677");


        public static Guid WarehoueLicenseSubscriptionContractClass = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3112");//licence -subscription 
        public static Guid WarehoueLicenseSubscriptionContractType = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3113");//Perment -Renewal
        public static Guid WarehoueLicenseSubscriptionContractCategory = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3114");//Service ,application,Service profider
        public static Guid WarehoueLicenseSubscriptionRemindDateType = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3116");//day,month

        public static Guid WarehouePOFileTypes = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e7456");
        public static Guid WarehoueDamagedReportFlowStatus = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7111");
        public static Guid WarehouseLicenseType = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7873");
        public static Guid WarehouseCostCenter = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7625");
        public static Guid WarehouseDamageLossType = Guid.Parse("0C44822F-A898-476D-B291-CAF1B0551AC7");




        #endregion

        #region ORG
        public static Guid StaffRecruitmentType = Guid.Parse("8A2AE01E-0F57-4178-B05E-E3021A9701A6");
        public static Guid StaffContractTypes = Guid.Parse("1234FD7E-3A6D-4A8A-9218-4C4CF227B288");
        public static Guid StaffGradeTypes = Guid.Parse("36DDC772-6AAD-4FAD-B317-7F4C2F9F1D2B");
        public static Guid StaffSalaryInAdvanceStatus = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7357");

        #endregion

        #region IMS
        public static Guid IMSMissionStatus = Guid.Parse("0c44822f-a898-476d-b291-caf1b013aac6");
        public static Guid IMSMissionActionTakenStatus = Guid.Parse("a40ec252-622e-4ff1-9ef4-e328c1d3cec3");
        #endregion

        #region AHD

        public static Guid DangerPayStaffStatus = Guid.Parse("A40EC252-622E-4FF1-9EF4-E323C7A3CEC9");
        public static Guid AbsenceType = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e9936");

        #endregion


        public static Guid OfficeSiteCategories = Guid.Parse("75081599-A461-4CF5-99A9-784358A196BE");
        public static Guid PPAAreasOfImplementation = Guid.Parse("1CDA65F3-A9DB-425F-B5C7-071CB80B83DC");
        public static Guid UserHelpDeskApprovalStatuses = Guid.Parse("42F9797E-B238-4DCE-BF4E-AEC1C0A1D78A");
        public static Guid UserHelpDeskEstimatedTime = Guid.Parse("A947170F-FC5B-414C-9BA7-60E8D9184310");
        public static Guid UserHelpDeskCriticality = Guid.Parse("9C90408E-3C28-4517-95CD-6DB390EDDE82");
        public static Guid HelpDeskWorkGroups = Guid.Parse("1A17F70E-8C5C-4B8E-94F6-29A2C1ECEDE7");
        public static Guid HelpDeskConfigItems = Guid.Parse("92BF0D56-BB61-4B8C-B698-537672AF7E39");
        public static Guid HelpDeskRequestStatus = Guid.Parse("25455260-C3C2-49C6-9ED0-594475425FA6");
        public static Guid WarehouseItemsDeterminations = Guid.Parse("D5E2A4DF-C67E-4F4D-A242-7E33C981EC11");
        public static Guid AHDVehicleType = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3633");
        public static Guid AHDVechileModel = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3637");
        public static Guid AHDVehileColor = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3638");
        public static Guid AHDVehicleMaintenanceRequestFlowStatus = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3699");
        public static Guid PrinterModel = Guid.Parse("00000000-0000-0000-0000-000000000027");
        public static Guid PrinterType = Guid.Parse("00000000-0000-0000-0000-000000000028");
        public static Guid OidType = Guid.Parse("00000000-0000-0000-0000-000000000029");

        #region EMT
        public static Guid MedicalDoseUnit = Guid.Parse("ABC00000-0000-0000-0000-000000000001");
        public static Guid MedicalPharmacologicalForm = Guid.Parse("ABC00000-0000-0000-0000-000000000002");
        public static Guid MedicalPackingUnit = Guid.Parse("ABC00000-0000-0000-0000-000000000003");
        public static Guid MedicalTreatment = Guid.Parse("ABC00000-0000-0000-0000-000000000004");
        public static Guid MedicalRouteAdministration = Guid.Parse("ABC00000-0000-0000-0000-000000000005");
        public static Guid Diagnosis = Guid.Parse("ABC00000-0000-0000-0000-000000000006");
        public static Guid BeneficiaryType = Guid.Parse("abc00000-0000-0000-0000-000000000007");
        #endregion


        #region TBS
        public static Guid StaffPhoneTypes = Guid.Parse("FA75A573-3B42-4E43-A65B-45A38DAAC952");
        public static Guid StaffPhoneUsageType = Guid.Parse("4A3C0AF5-50F7-4C38-9220-58CC93B3A47A");
        public static Guid PhoneCallTypes = Guid.Parse("BAE71943-8CE0-41E2-BBED-1AA37486F16D");
        public static Guid BillForType = Guid.Parse("4A3200E3-78C4-4AB4-BE30-4B811EEDEB86");
        #endregion

        #region SIP
        public static Guid SIPIndicatorSourceType = Guid.Parse("E4938754-BA3C-43C8-A7A2-42645895A729");
        public static Guid SIPAnalyticalValueType = Guid.Parse("68f36572-0d92-40d2-bf89-7ac208f2d10d");
        public static Guid SIPIndicatorFrequency = Guid.Parse("89df2dec-811a-44d5-a53a-e1495e5c39c0");
        #endregion

        #region SAS
        public static Guid SASDailyJustification = Guid.Parse("062E287E-2FB1-4F02-AEF6-B3600A857887");
        public static Guid SASDayState = Guid.Parse("C5D22E2F-D372-4AD1-B640-D40E6471DE17");
        #endregion

        #region HR
        public static Guid StaffRecruitmentTypes = Guid.Parse("8A2AE01E-0F57-4178-B05E-E3021A9701A6");
        public static Guid StaffGrades = Guid.Parse("36DDC772-6AAD-4FAD-B317-7F4C2F9F1D2B");
        #endregion

        #region TTT
        public static Guid TenderTypes = Guid.Parse("D881E890-3217-4047-9F32-C18EF22367B6");
        public static Guid TenderBudgetSources = Guid.Parse("327862C1-6751-42AA-8CA4-00116F61C5AB");
        public static Guid TenderIdentity = Guid.Parse("5288DEB4-C29E-40A5-8FDD-52C6C7847D31");
        public static Guid TenderEndorsmentRequired = Guid.Parse("D7D8B1FC-ECE8-40AF-89C7-F4B71AAB9F99");
        public static Guid TenderHoFoSupplyRep = Guid.Parse("98FD4B0B-950C-457C-ACD2-36374E7A76D2");
        public static Guid TenderAwardedCompanies = Guid.Parse("39CA2261-5E49-4CD9-8F81-587A8BFD1D3E");
        public static Guid TenderFA1 = Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E3624");
        public static Guid TenderResolutions = Guid.Parse("5879D099-3B59-4C68-BF75-B6A18ECF7A9F");
        public static Guid TenderStatuses = Guid.Parse("5337E23F-0C0A-4E30-822E-E278F18DA3E2");

        #endregion

        #region DAS
        public static Guid DASPaperFormat = Guid.Parse("a7ba6b74-dc7e-4c2b-a9a1-322ba5188339");
        public static Guid DASResolution = Guid.Parse("E39B1861-B31F-4595-8384-728593ED43D3");
        public static Guid DASPaperSize = Guid.Parse("fe42f846-f671-4230-ba02-cfb283bfba84");
        public static Guid DASScanningType = Guid.Parse("f2152b55-2fea-4f69-9257-d4411fda8224");
        public static Guid DASColorMod = Guid.Parse("315c18c9-8d43-4d4c-9948-fb6524f8a51e");
        public static Guid DASFileMovementStatus = Guid.Parse("A40EC252-622E-4FF1-9EF4-E328C7A9CEC8");
        public static Guid DASDocumentSourceTypes = Guid.Parse("d000c992-4871-4986-8f82-72c27ee20987");
        public static Guid DASSoftFieldListSource = Guid.Parse("29591f83-32a4-49a5-b284-2a8efbc44786");
        #endregion

        #region  IDC
        public const string CardIssuedsDataTable = "CardIssuedsDataTable";
        public const string CardIndividualInfosDataTable = "CardIndividualInfosDataTable";
        public const string CardPrintsDataTable = "CardPrintsDataTable";
        public const string CardValidationsDataTable = "CardValidationsDataTable";
        #endregion

        #region ORG
        public const string StaffAccountBankDataTable = "StaffAccountBankDataTable";
        public const string StaffNationalPassportDataTable = "StaffNationalPassportDataTable";
        public const string StaffBankAccountDataTable = "StaffBankAccountDataTable";
        
       
        public static class StaffContractManagerPermssion
        {
            public static Guid UNOPS = Guid.Parse("B49A0E2F-7819-4558-B165-473E4FC19630");
            public static Guid UNHCR = Guid.Parse("9001BA39-6996-4042-A651-36F68912C5A7");

        }
        #endregion


        #region COV
        public static Guid COVResponseTechnicalUnit = Guid.Parse("FD0EBCB6-3947-4FD0-8212-40D252F0098A");

        #endregion

        #region GTP
        public static Guid GTPMaritalStatus = Guid.Parse("d1274991-4439-4514-bfff-6a6b9d55b148");
        public static Guid GTPEmploymentTypes = Guid.Parse("b1eca6f9-fae4-4884-845a-db0527829982");
        public static Guid GTPTypeOfBusiness = Guid.Parse("00a90db4-a85b-4a41-b672-c90a1f1cb438");
        public static Guid GTPContractType = Guid.Parse("7b2c884c-e82a-4380-897e-8f001f280ab4");
        public static Guid GTPTrainingMethodology = Guid.Parse("15f75ca6-8130-43d6-8715-3bad29b1aed8");
        public static Guid GTPEducationLevel = Guid.Parse("8af2cbd0-5ff9-4f52-bfed-957257fd9f72");
        public static Guid GTPSkillLevel = Guid.Parse("2e3deb09-3549-42f0-b45f-5b36e7edc563");
        public static Guid GTPLanguageLevel = Guid.Parse("4d5238b4-1722-4034-a0ab-7ffac99644c3");
        public static Guid GTPVacancyHear = Guid.Parse("8b83114f-5583-4b70-bf3f-ed1d1c71aa7e");
        #endregion

        #region PMD
        public static Guid PMDCriLocationTypes = Guid.Parse("FD03DB44-0A7A-4FBF-8EB0-D2C3AACFE89C");
        public static Guid PMDCriModalities = Guid.Parse("2AC0944A-6E12-40DE-B0C5-31D72FCF2185");
        public static Guid PMDCriResponseTypes = Guid.Parse("F39CF7B6-CED8-4121-ADD0-7F7EC444936C");
        #endregion
    }

    public class UNHCR
    {
        public readonly static Guid GUID = Guid.Parse("F9BD9237-CA5B-4753-A0EB-B1E4C33B1490");
        public const string ShortName = "UNHCR";
        public const string FullName = "United Nations High Commissioner for Refugees";
        public readonly static Guid SyriaOrganizationInstanceGUID = Guid.Parse("e156c022-ec72-4a5a-be09-163bd85c68ef");
    }

    public static class AccountStatus
    {


        public const int Step01_RegistrationRequestSubmitted = 1;       //Stay on the page
        public const int Step02_EmailVerified = 2;                      //Move to Organization setup page
        public const int Step03_OperationAndOrganizationSubmitted = 3;  //Stay in organization setup page
        public const int Step04_SponsorApproved = 4;                    //Move to password setup page
        public const int Step05_PendingAcceptTermOfUse = 5;             //First Login
        public const int Ready = 10;
        public const int PendingChangePassword = 0;
        public const int Inactive = -1;
    }

    public static class DataTableNames
    {
        #region CMS
        public const string UsersDataTable = "UsersDataTable";
        public const string TestDataTable = "TestDataTable";
        public const string ApplicationsDataTable = "ApplicationsDataTable";
        public const string ApplicationLanguagesDataTable = "ApplicationLanguagesDataTable";
        public const string MenusDataTable = "MenusDataTable";
        public const string MenuLanguagesDataTable = "MenuLanguagesDataTable";
        public const string OrganizationsDataTable = "OrganizationsDataTable";
        public const string OrganizationLanguagesDataTable = "OrganizationLanguagesDataTable";
        public const string OrganizationOperationsDataTable = "OrganizationOperationsDataTable";
        public const string OrganizationsInstancesDataTable = "OrganizationsInstancesDataTable";
        public const string OrganizationInstanceLanguagesDataTable = "OrganizationInstanceLanguagesDataTable";
        public const string OperationsDataTable = "OperationsDataTable";
        public const string OperationLanguagesDataTable = "OperationLanguagesDataTable";
        public const string DepartmentsDataTable = "DepartmentsDataTable";
        public const string DepartmentLanguagesDataTable = "DepartmentLanguagesDataTable";
        public const string DutyStationsDataTable = "DutyStationsDataTable";
        public const string DutyStationLanguagesDataTable = "DutyStationLanguagesDataTable";
        public const string CountriesDataTable = "CountriesDataTable";
        public const string CountryLanguagesDataTable = "CountryLanguagesDataTable";
        public const string CountryOperationsDataTable = "CountryOperationsDataTable";
        public const string LocationsDataTable = "LocationsDataTable";
        public const string LocationLanguagesDataTable = "LocationLanguagesDataTable";
        public const string PersonalDetailsNamesDataTable = "PersonalDetailsNamesDataTable";
        public const string HomeAddressLanguageDataTable = "HomeAddressLanguageDataTable";
        public const string ServiceHistoryDataTable = "ServiceHistoryDataTable";
        public const string JobsHistoryDataTable = "JobsHistoryDataTable";
        public const string PermissionsDataTable = "PermissionsDataTable";
        public const string PermissionLanguagesDataTable = "PermissionLanguagesDataTable";
        public const string TablesDataTable = "TablesDataTable";
        public const string TableValuesDataTable = "TableValuesDataTable";
        public const string TableValuesLanguagesDataTable = "TableValuesLanguagesDataTable";
        public const string JobTitlesDataTable = "JobTitlesDataTable";
        public const string JobTitleLanguagesDataTable = "JobTitleLanguagesDataTable";
        public const string WorkAddressLanguageDataTable = "WorkAddressLanguageDataTable";
        public const string OfficesDataTable = "OfficesDataTable";
        public const string OfficeLanguagesDataTable = "OfficeLanguagesDataTable";
        public const string FactorsDataTable = "FactorsDataTable";
        public const string FactorLanguagesDataTable = "FactorLanguagesDataTable";
        public const string ActionCategoryFactorsDataTable = "ActionCategoryFactorsDataTable";
        public const string ActionCategoriesDataTable = "ActionCategoriesDataTable";
        public const string ActionCategoryLanguagesDataTable = "ActionCategoryLanguagesDataTable";
        public const string FactorsDependencyDataTable = "FactorsDependencyDataTable";
        public const string ActionsDataTable = "ActionsDataTable";
        public const string ActionLanguagesDataTable = "ActionLanguagesDataTable";
        public const string OrganizationsConfigDataTable = "OrganizationsConfigDataTable";
        public const string CountriesConfigDataTable = "CountriesConfigDataTable";
        public const string LookupValuesConfigDataTable = "LookupValuesConfigDataTable";
        public const string DutyStationsConfigDataTable = "DutyStationsConfigDataTable";
        public const string DepartmentsConfigDataTable = "DepartmentsConfigDataTable";
        public const string SitemapsDataTable = "SitemapsDataTable";
        public const string SitemapLanguagesDataTable = "SitemapLanguagesDataTable";
        public const string codeTablesDataTable = "codeTablesDataTable";
        public const string GenderConfigDataTable = "GenderConfigDataTable";
        public const string ActionsVerbsDataTable = "ActionsVerbsDataTable";
        public const string ActionVerbLanguagesDataTable = "ActionVerbLanguagesDataTable";
        public const string ActionsEntitiesDataTable = "ActionsEntitiesDataTable";
        public const string ActionEntityLanguageDataTable = "ActionEntityLanguageDataTable";
        public const string AuditLoginsDataTable = "AuditLoginsDataTable";
        public const string IndividualsDataTable = "IndividualsDataTable";
        public const string DocumentsDataTable = "DocumentsDataTable";
        public const string ConnectionsDataTable = "ConnectionsDataTable";
        public const string ConnectionsAccountSettingDataTable = "ConnectionsAccountSettingDataTable";
        public const string NotificationsDataTable = "NotificationsDataTable";
        public const string NotificationLanguagesDataTable = "NotificationLanguagesDataTable";
        public const string NotificationsAccountSettingDataTable = "NotificationsAccountSettingDataTable";
        public const string ApplicationAccessAuditDataTable = "ApplicationAccessAuditDataTable";
        public const string UserPermissionsAuditDataTable = "UserPermissionsAuditDataTable";

        #endregion

        #region OVS
        public const string ElectionConditionsDataTable = "ElectionConditionsDataTable";
        public const string ElectionCandidatesDataTable = "ElectionCandidatesDataTable";
        public const string ElectionStaffsDataTable = "ElectionStaffsDataTable";
        public const string ElectionResultsDataTable = "ElectionResultsDataTable";
        public const string ElectionCorrespondencesDataTable = "ElectionCorrespondencesDataTable";
        public const string ElectionsDataTable = "ElectionsDataTable";
        public const string ElectionLanguagesDataTable = "ElectionLanguagesDataTable";
        public const string ConditionTypesDataTable = "ConditionTypesDataTable";
        public const string ConditionTypeLanguagesDataTable = "ConditionTypeLanguagesDataTable";
        #endregion

        #region TBS
        public const string TelecomCompaniesDataTable = "TelecomCompaniesDataTable";
        public const string TelecomCompanyLanguagesDataTable = "TelecomCompanyLanguagesDataTable";
        public const string TelecomCompanyOperationsDataTable = "TelecomCompanyOperationsDataTable";
        public const string DataBillsDataTable = "DataBillsDataTable";
        public const string DataLandLinsBillsDataTable = "DataLandLinsBillsDataTable";
        public const string StaffPhonesDataTable = "StaffPhonesDataTable";
        public const string UserBillsDataTableX = "UserBillsDataTable";
        public const string CDRFTPLocationDataTable = "CDRFTPLocationDataTable";
        public const string MyPhoneBillsDataTable = "MyPhoneBillsDataTable";
        public const string PendingBillsDataTable = "PendingBillsDataTable";
        #endregion


        #region AST
        public const string ASTStationaryItemReleaseDataTable = "ASTStationaryItemReleaseDataTable";
        public const string ASTStationaryItemEntryDataTable = "ASTStationaryItemEntryDataTable";
        public const string ASTItemEntryFormDataTable = "ASTItemEntryFormDataTable";
        public const string ASTItemModelsDataTable = "ASTItemModelsDataTable";
        public const string ASTItemModelLanguagesDataTable = "ASTItemModelLanguagesDataTable";
        public const string ASTItemModelWarehouseDataTable = "ASTItemModelWarehouseDataTable";
        #endregion


        #region IMS
        public const string MissionReportFormDataTable = "MissionReportFormDataTable";
        public const string MissionReportFormHistoryDataTable = "MissionReportFormHistoryDataTable";
        public const string MissionReportFormTrackingDataTable = "MissionReportFormTrackingDataTable";
        public const string MissionTempActionsDataTable = "MissionTempActionsDataTable";
        public const string MissionActionOverviewDataTable = "MissionActionOverviewDataTable";
        public const string MissionActionOngoingDataTable = "MissionActionOngoingDataTable";
        public const string MissionActionsDataTable = "MissionActionsDataTable";
        public const string MissionReportDocumentDataTable = "MissionReportDocumentDataTable";



        #endregion

        #region WMS

        public const string WarehouseItemEntriesDataTable = "WarehouseItemEntriesDataTable";
        public const string WarehouseItemEntryDetailsDataTable = "WarehouseItemEntryDetailsDataTable";

        public const string WarehouseItemReleasesDataTable = "WarehouseItemReleasesDataTable";
        public const string WarehouseItemReleaseDeatilsDataTable = "WarehouseItemReleaseDeatilsDataTable";

        public const string WarehosueModelMovementsDataTable = "WarehosueModelMovementsDataTable";

        public const string ItemModelsDataTable = "ItemModelsDataTable";
        public const string ItemModelLanguagesDataTable = "ItemModelLanguagesDataTable";
        public const string ItemModelDeterminantsDataTable = "ItemModelDeterminantsDataTable";
        public const string ItemModelWarehouseDataTable = "ItemModelWarehouseDataTable";
        //public const string ModelManagementsDataTable = "ModelManagementsDataTable";
        public const string WarehouseItemRequestsDataTable = "WarehouseItemRequestsDataTable";
        public const string WarehouseItemRequestDetailsDataTable = "WarehouseItemRequestDetailsDataTable";
        public const string WarehouseModelDamagedMovementDataTable = "WarehouseModelDamagedMovementDataTable";
        public const string DamagedItemRequestsByItemDataTable = "DamagedItemRequestsByItemDataTable";

        public const string WarehouseModelEntryMovementsDataTable = "WarehouseModelEntryMovementsDataTable";
        public const string WarehouseModelReleaseMovementsDataTable = "WarehouseModelReleaseMovementsDataTable";
        public const string WarehouseModelReservationMovementDataTable = "WarehouseModelReservationMovementDataTable";
        public const string WarehousePendingVerificationMyWarehouseDataTable = "WarehousePendingVerificationMyWarehouseDataTable";
        public const string WarehousePendingVerificationOtherDataTable = "WarehousePendingVerificationOtherDataTable";
        public const string WarehousePendingVerificationStaffDataTable = "WarehousePendingVerificationStaffDataTable";
        public const string WarehouseItemVerficationDataTable = "WarehouseItemVerficationDataTable";
        public const string ItemVerificationPeriodsDataTable = "ItemVerificationPeriodsDataTable";

        public const string ItemClassificationsDataTable = "ItemClassificationsDataTable";
        public const string ItemsDataTable = "ItemsDataTable";
        public const string WarehouseItemKitChildrenDataTable = "WarehouseItemKitChildrenDataTable";
        public const string WarehouseModelTrackFlowMovementDataTable = "WarehouseModelTrackFlowMovementDataTable";
        public const string WarehouseItemTrackFlowDailyDataTable = "WarehouseItemTrackFlowDailyDataTable";
        public const string WarehouseItemTrackFlowWeeklyDataTable = "WarehouseItemTrackFlowWeeklyDataTable";
        public const string WarehouseItemTrackFlowMonthlyDataTable = "WarehouseItemTrackFlowMonthlyDataTable";
        public const string WarehouseItemTrackFlowALLDataTable = "WarehouseItemTrackFlowALLDataTable";
        public const string WarehouseConsumableItemOverviewDataTable = "WarehouseConsumableItemOverviewDataTable";
        public const string WarehouseConsumableItemEntryDetailDataTable = "WarehouseConsumableItemEntryDetailDataTable";
        public const string WarehouseConsumableItemReleaseDetailDataTable = "WarehouseConsumableItemReleaseDetailDataTable";
        public const string WarehouseConsumableItemEntryDetailInfoDataTable = "WarehouseConsumableItemEntryDetailInfoDataTable";
        public const string WarehouseConsumableItemReleaseDetailInfoDataTable = "WarehouseConsumableItemReleaseDetailInfoDataTable";
        public const string WarehouseConsumablePendingConfirmationoDataTable = "WarehouseConsumablePendingConfirmationoDataTable";

        public const string WarehouseModelTrackReleaseHistoricals = "WarehouseModelTrackReleaseHistoricals";
        public const string WarehouseTrackModelVerifiationPerSite = "WarehouseTrackModelVerifiationPerSite";
        public const string WarehouseItemUploadedDocumentsDataTable = "WarehouseItemUploadedDocumentsDataTable";
        public const string STIContactsDataTable = "STIContactsDataTable";

        public const string WMSLicenseSubscriptionContractDataTable = "WMSLicenseSubscriptionContractDataTable";

        public const string WMSContractPOHistoryDataTable = "WMSContractPOHistoryDataTable";
        public const string WMSContractSTIItemsDataTable = "WMSContractSTIItemsDataTable";

        public const string WMSContractPOFileUploadHistoryDataTable = "WMSContractPOFileUploadHistoryDataTable";

        public const string StaffCustodyWarehosueItemsDataTable = "StaffCustodyWarehosueItemsDataTable";
        

        public const string StaffGrantedSoftwareApplicationsDataTable = "StaffGrantedSoftwareApplicationsDataTable";
        public const string WarehouseItemDetailFeatureDataTable = "WarehouseItemDetailFeatureDataTable";
        public const string WarehouseFocalPointsDataTable = "WarehouseFocalPointsDataTable";


        public const string WarehousesDataTable = "WarehousesDataTable";
        public const string STIItemDamagedTrackDataTable = "STIItemDamagedTrackDataTable";
        public const string AllDamagedItemRequestsDataTable = "AllDamagedItemRequestsDataTable";

        #endregion

        #region ISS
        public const string ISSStockItemDistributionDataTable = "ISSStockItemDistributionDataTable";
        public const string ISSItemPipelineDataTable = "ISSItemPipelineDataTable";
        public const string ISSStockItemOverviewDataTable = "ISSStockItemOverviewDataTable";
        public const string ISSItemOverviewDataTable = "ISSItemOverviewDataTable";
        public const string ISSItemDataTable = "ISSItemDataTable";




        #endregion

        #region RMS
        public const string PrintersConfigurationDataTable = "PrintersConfigurationDataTable";
        public const string PrinterOidsDataTable = "PrinterOIDsDataTable";
        public const string OidsDataTable = "OidsDataTable";

        #endregion

        #region REF
        public const string ReferralsDataTable = "ReferralsDataTable";
        public const string ReferralLanguagesDataTable = "ReferralLanguagesDataTable";

        public const string ReferralStepsDataTable = "ReferralStepsDataTable";
        public const string ReferralStepLanguagesDataTable = "ReferralStepLanguagesDataTable";

        public const string ReferralStatusDataTable = "ReferralStatusDataTable";
        public const string ReferralStatusLanguagesDataTable = "ReferralStatusLanguagesDataTable";

        public const string ReferralNotificationsDataTable = "ReferralNotificationsDataTable";
        public const string ReferralStepUsersDataTable = "ReferralStepUsersDataTable";

        public const string FocalPointsDataTable = "FocalPointsDataTable";
        public const string FocalPointStaffsDataTable = "FocalPointStaffsDataTable";


        #endregion

        #region AMS
        public const string AppointmentTypesDataTable = "AppointmentTypesDataTable";
        public const string AppointmentTypeLanguagesDataTable = "AppointmentTypeLanguagesDataTable";

        public const string AppointmentsDataTable = "AppointmentsDataTable";
        public const string CasesDataTable = "CasesDataTable";

        public const string ContactInfosDataTable = "ContactInfosDataTable";
        public const string AppointmentTypeCalendarsDataTable = "AppointmentTypeCalendarsDataTable";
        #endregion

        #region PPA
        public const string PPADataTable = "PPADataTable";
        public const string PPAFilesDataTable = "PPAFilesDataTable";
        public const string PPADistributionListDataTable = "PPADistributionListDataTable";
        public const string PPAAccessListDataTable = "PPAAccessListDataTable";
        public const string PPAForUserDataTable = "PPAForUserDataTable";
        public const string PPAUserFilesDataTable = "PPAUserFilesDataTable";
        public const string PPAReviewerListDataTable = "PPAReviewerListDataTable";
        #endregion

        #region SRS
        public const string ApplicationRequestsDataTable = "ApplicationRequestsDataTable";
        public const string ApplicationRequestAttachementDataTable = "ApplicationRequestAttachementDataTable";

        public const string ApplicationEnhancementsDataTable = "ApplicationEnhancementsDataTable";
        public const string ApplicationEnhancementAttachementDataTable = "ApplicationEnhancementAttachementDataTable";

        public const string BugReportsDataTable = "BugReportsDataTable";
        public const string BugReportsAttachementDataTable = "BugReportsAttachementDataTable";


        public const string HelpDeskDataTable = "HelpDeskDataTable";
        public const string HelpDeskAttachementDataTable = "HelpDeskAttachementDataTable";
        #endregion

        #region PCR
        public const string PartnerCentersDataTable = "PartnerCentersDataTable";
        public const string PartnerCenterLanguagesDataTable = "PartnerCenterLanguagesDataTable";
        public const string PartnerReportsDataTable = "PartnerReportsDataTable";
        public const string PartnerReportCompiledsDataTable = "PartnerReportCompiledsDataTable";

        #endregion

        #region SHM
        public const string ShuttlesDataTable = "ShuttlesDataTable";
        public const string ShuttleStaffsDataTable = "ShuttleStaffsDataTable";
        public const string ShuttleRoutesDataTable = "ShuttleRoutesDataTable";
        public const string ShuttleVehiclesDataTable = "ShuttleVehiclesDataTable";
        public const string VehiclesDataTable = "VehiclesDataTable";
        public const string ShuttleRequestsDataTable = "ShuttleRequestsDataTable";
        public const string ShuttleTravelPurposesDataTable = "ShuttleTravelPurposesDataTable";
        public const string ShuttleTravelPurposeLanguagesDataTable = "ShuttleTravelPurposeLanguagesDataTable";
        public const string ShuttleRequestAdminDataTable = "ShuttleRequestAdminDataTable";
        public const string ShuttleRequestStaffsDataTable = "ShuttleRequestStaffsDataTable";
        public const string ShuttleRequestRoutesDataTable = "ShuttleRequestRoutesDataTable";
        public const string ShuttleRequestRouteStepsDataTable = "ShuttleRequestRouteStepsDataTable";
        //ShuttleRequestRouteStepsDataTable
        #endregion

        #region  MRS
        public const string NoteVerbaleStaffsDataTable = "NoteVerbaleStaffsDataTable";
        public const string NoteVerbalesDataTable = "NoteVerbalesDataTable";
        public const string NoteVerbaleVehiclesDataTable = "NoteVerbaleVehiclesDataTable";
        public const string NoteVerbaleOrganizationsDataTable = "NoteVerbaleOrganizationsDataTable";
        //NoteVerbaleVehiclesDataTable
        #endregion

        #region ORG
        public const string StaffProfileDataTable = "StaffProfileDataTable";
        public const string StaffContactsInformationDataTable = "StaffContactsInformationDataTable";
        public const string StaffAddressesInformationDataTable = "StaffAddressesInformationDataTable";
        public static class StaffCategoryAccess
        {

            public static Guid UNHCRSyriaStaff = Guid.Parse("71812257-b6eb-4063-837a-9823b2863397");
            public static Guid NONUNHCRSyriaStaff = Guid.Parse("71812257-b6eb-4063-837a-9823b2863392");
            public static Guid ExternalUser = Guid.Parse("71812257-b6eb-4063-837a-9823b2863393");
            public static Guid FuncationalMailUser = Guid.Parse("71812257-b6eb-4063-837a-9823b2863394");

        }
        public static class StaffGender
        {
            public static Guid Male = Guid.Parse("688B11E0-24FB-44B8-94CE-D8568C4742C7");
            public static Guid Female = Guid.Parse("DBF9D307-CE9F-4029-BD1E-D7AF6739975C");

        }
        #endregion
        #region HIMS
        public const string RefugeeMedicalReferralsDataTable = "RefugeeMedicalReferralsDataTable";
        #endregion
        #region SDS
        public const string SurveyTemplateFormDataTable = "SurveyTemplateFormDataTable";
        public const string TemplateEntryFormInstanceDataTable = "TemplateEntryFormInstanceDataTable";
        public const string SurveyTemplateConfigurationDataTable = "SurveyTemplateConfigurationDataTable";
        public const string FormLanguageDataTable = "FormLanguageDataTable";
        public const string FormSectionDataTable = "FormSectionDataTable";
        public const string TemplateFormStatusDataTable = "TemplateFormStatusDataTable";
        public const string TemplateFormStatusAuthorizedUserDataTable = "TemplateFormStatusAuthorizedUserDataTable";
        public const string TemplateFormFieldDataTable = "TemplateFormFieldDataTable";
        public const string TemplateFormConditionDataTable = "TemplateFormConditionDataTable";

        #endregion

        #region RTS

        public const string RTSReportTemplateDataTable = "RTSReportTemplateDataTable";
        #endregion
        #region RAMS
        public const string CashCycleDataTable = "CashCycleDataTable";
        public const string CashReferralDataTable = "CashReferralDataTable";
        public const string BankPlanSequenceMainDataTable = "BankPlanSequenceMainDataTable";
        public const string BankPlanListDataTableModel = "BankPlanListDataTableModel";
        public const string BankPlanCaseDataTableModel = "BankPlanCaseDataTableModel";

        #endregion
        #region AHD
        public
        const string VehicleRegistrationRenewalDataTable = "VehicleRegistrationRenewalDataTable";
        public const string StaffRenewalResidencyDataTable = "StaffRenewalResidencyDataTable";
        public const string NationalStaffDangerPayManagementDataTable = "NationalStaffDangerPayManagementDataTable";
        public const string NationalStaffDangerPayDataTable = "NationalStaffDangerPayDataTable";
        public const string NationalStaffDangerPayDetailDataTable = "NationalStaffDangerPayDetailDataTable";
        public const string NationalStaffAllDangerPayDataTable = "NationalStaffAllDangerPayDataTable";
        public const string TrackStaffDangerPaymentDataTable = "TrackStaffDangerPaymentDataTable";
        public const string TrackNationalStaffAllDangerPayDataTable = "TrackNationalStaffAllDangerPayDataTable";
        public const string StaffRAndRLeaveRequestDataTable = "StaffRAndRLeaveRequestDataTable";
        public const string InternationalStaffAttendanceDetailDataTable = "InternationalStaffAttendanceDetailDataTable";
        public const string RAndRDocumentsDataTable = "RAndRDocumentsDataTable";
        public const string VehicleMaintenceRequestDataTable = "VehicleMaintenceRequestDataTable";
        public const string InternationalTempStaffRAndRDatesDataTable = "InternationalTempStaffRAndRDatesDataTable";
        public const string InternationalStaffEntitlementPeriodDataTable = "InternationalStaffEntitlementPeriodDataTable";
        public const string InternationalStaffHistoricalEntitlementDataTable = "InternationalStaffHistoricalEntitlementDataTable";
        public const string InternationalStaffEntitlementDataTable = "InternationalStaffEntitlementDataTable";
        public const string InternationalMyStaffEntitlementDataTable = "InternationalMyStaffEntitlementDataTable";
        public const string InternationalStaffEntitlementDetailDataTable = "InternationalStaffEntitlementDetailDataTable";
        public const string InternationalMyStaffEntitlementPriviousDataTable = "InternationalMyStaffEntitlementPriviousDataTable";
        public const string BlomShuttleDelegationDateDataTable = "BlomShuttleDelegationDateDataTable";
        public const string BlomShuttleDelegationTravelerDataTable = "BlomShuttleDelegationTravelerDataTable";
        public const string BlomShuttleDelegationStaffRequestDataTable = "BlomShuttleDelegationStaffRequestDataTable";
        public const string SalaryCyclePeriodDataTable = "SalaryCyclePeriodDataTable";
        public const string StaffOvertimeDataTable = "StaffOvertimeDataTable";
        public const string StaffHQSalaryDataTable = "StaffHQSalaryDataTable";
        public const string StaffMedicalPaymentDataTable = "StaffMedicalPaymentDataTable";
        public const string StaffSalaryBankDestenationDataTable = "StaffSalaryBankDestenationDataTable";


        public const string StaffSalaryCyclePaymentDataTable = "StaffSalaryCyclePaymentDataTable";
        public const string CycleSalaryFlowStepDataTable = "CycleSalaryFlowStepDataTable";

        public const string CycleSalaryStaffBankDestenationDataTable = "CycleSalaryStaffBankDestenationDataTable";
        public const string CycleSalaryNationalStaffAllDangerPayDataTable = "CycleSalaryNationalStaffAllDangerPayDataTable";
        public const string CycleSalaryStaffBillingDataTable = "CycleSalaryStaffBillingDataTable";
        public const string StaffMyOvertimefDataTable = "StaffMyOvertimefDataTable";

        public const string AllOvertimeMonthCycleDataTable = "AllOvertimeMonthCycleDataTable";
        public const string AllStaffCycleOvertimeMonthDataTable = "AllStaffCycleOvertimeMonthDataTable";
        public const string OvertimeMonthCycleDataTable = "OvertimeMonthCycleDataTable";
        public const string StaffOvertimeForSalaryDataTable = "StaffOvertimeForSalaryDataTable";
        public const string StaffMedicalPaymentForSalaryDataTable = "StaffMedicalPaymentForSalaryDataTable";
        public const string StaffMissionRequestDataTable = "StaffMissionRequestDataTable";
        public const string StaffAllMissionRequestDataTable = "StaffAllMissionRequestDataTable";

        public const string MissionRequestItineraryDataTable = "MissionRequestItineraryDataTable";
        public const string MissionRequestTravelerDataTable = "MissionRequestTravelerDataTable";
        public const string MissionRequestDocumentDataTable = "MissionRequestDocumentDataTable";

        public const string EntitlementsInitCalacuationDataTable = "EntitlementsInitCalacuationDataTable";
        public const string MissionRORequestItineraryDataTable = "MissionRORequestItineraryDataTable";
        public const string MissionRORequestDocumentDataTable = "MissionRORequestDocumentDataTable";
        public const string MissionRORequestTravelerDataTable = "MissionRORequestTravelerDataTable";
        public const string EntitlementDocumentsDataTable = "EntitlementDocumentsDataTable";
        public const string StaffSeparationChecklistDataTable = "StaffSeparationChecklistDataTable";
        public const string StaffSepartionCheckListConfirmationDataTable = "StaffSepartionCheckListConfirmationDataTable";
        public const string StaffChecklistDetailDataTable = "StaffChecklistDetailDataTable";
        public const string CheckListFocalPointsDataTable = "CheckListFocalPointsDataTable";

        public const string SeparationChecklistTypesDataTable = "SeparationChecklistTypesDataTable";
        public const string SeparationChecklistFocalPointDataTable = "SeparationChecklistFocalPointDataTable";
        public const string StaffAbsencesDataTable = "StaffAbsencesDataTable";
        public const string StaffAbsenceBalancesDataTable = "StaffAbsenceBalancesDataTable";
        public const string StaffAbsencesConfirmingDataTable = "StaffAbsencesConfirmingDataTable";

        
        public const string IsReturnToDutyStation = "b9cd375c-a576-4aa4-8af4-ff3c1c4e3111";


        public static class OvertimeFlowStatus
        {
            //a40ec252-622e-4ff1-9ef4-e328c7a3cec8
            public readonly static Guid PendingSupervisorReview = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7369");
            public readonly static Guid PendingCertifying = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7362");

            public readonly static Guid Canceled = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7363");
            public readonly static Guid Approved = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7367");
            public readonly static Guid Submitted = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7311");

        }


        public static class IsReturnToDutyStationValues
        {
            public static Guid Yes = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3111");
            public static Guid No = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3112");
        }
      

        #endregion

        #region DAS
        public const string RefugeeUploadScannedDocumentDataTable = "RefugeeUploadScannedDocumentDataTable";
        public const string RefugeeScannedDocumentDataTable = "RefugeeScannedDocumentDataTable";
   
        public const string FileTransferHistoryDataTable = "FileTransferHistoryDataTable";
        public const string RefugeeCasesHaveScheduledAppointmentDataTable = "RefugeeCasesHaveScheduledAppointmentDataTable";
        public const string UnitFocalPointsDataTable = "UnitFocalPointsDataTable";
        public const string UnitsDataTable = "UnitsDataTable";
        public const string FileLocationMovementDataTable = "FileLocationMovementDataTable";

        public const string DASUserFilesDataTable = "DASUserFilesDataTable";

        public const string DASOLDFTSFileMovementsDataHistoryDataTable = "DASOLDFTSFileMovementsDataHistoryDataTable";
        public const string TemplateTypeDataTable = "TemplateTypeDataTable";
        public const string TemplateTypeDocumentsDataTable = "TemplateTypeDocumentsDataTable";
        public const string TemplateTypeDocumentTagsDataTable = "TemplateTypeDocumentTagsDataTable";
        public const string TemplateTypeDocumentSoftFieldsDataTable = "TemplateTypeDocumentSoftFieldsDataTable";

        public const string ArchiveTemplateDataTable = "ArchiveTemplateDataTable";
        public const string ArchiveTemplateDocumentDataTable = "ArchiveTemplateDocumentDataTable";

        public const string DocumentCabinetShelfsDataTable = "DocumentCabinetShelfsDataTable";
        public const string HistorysDataTable = "HistorysDataTable";
        public const string DocumentLinksDataTable = "DocumentLinksDataTable";
        //HistorysDataTable
        #endregion


        #region ORG
        public const string CodeBanksDataTable = "CodeBanksDataTable";
        public const string CodeBankLanguagesDataTable = "CodeBankLanguagesDataTable";
        public const string StaffAccountBankDataTable = "StaffAccountBankDataTable";
        public const string StaffNationalPassportDataTable = "StaffNationalPassportDataTable";
        public const string StaffCoreDocumentDataTable = "StaffCoreDocumentDataTable";
        public const string StaffBankAccountDataTable = "StaffBankAccountDataTable";
        public const string StaffPhoneDirectoryDataTable = "StaffPhoneDirectoryDataTable";
        public const string StaffProfileFeedbackDataTable = "StaffProfileFeedbackDataTable";
        public const string StaffOnlineTrainingDataTable = "StaffOnlineTrainingDataTable";
        public const string StaffRelativeDataTable = "StaffRelativeDataTable";
        public const string StaffServiceProvidedDataTable = "StaffServiceProvidedDataTable";
        public const string StaffSalaryInAdvanceDataTable = "StaffSalaryInAdvanceDataTable";


        #endregion

        #region  EMT
        public const string MedicalPharmacysDataTable = "MedicalPharmacysDataTable";
        public const string MedicalPharmacyLanguagesDataTable = "MedicalPharmacyLanguagesDataTable";
        public const string MedicalItemsDataTable = "MedicalItemsDataTable";
        public const string ItemQuantityThresholdsDataTable = "ItemQuantityThresholdsDataTable";
        public const string MedicalManufacturersDataTable = "MedicalManufacturersDataTable";
        public const string MedicalManufacturerLanguagesDataTable = "MedicalManufacturerLanguagesDataTable";
        public const string MedicalGenericNamesDataTable = "MedicalGenericNamesDataTable";
        public const string MedicalGenericNameLanguagesDataTable = "MedicalGenericNameLanguagesDataTable";
        public const string MedicalItemInputsDataTable = "MedicalItemInputsDataTable";
        public const string MedicalItemInputDetailsDataTable = "MedicalItemInputDetailsDataTable";
        public const string MedicalItemInputDetailsViewDataTable = "MedicalItemInputDetailsViewDataTable";
        public const string MedicalItemTransfersDataTable = "MedicalItemTransfersDataTable";
        public const string MedicalItemTransferDetailsDataTable = "MedicalItemTransferDetailsDataTable";
        public const string MedicalBeneficiaryItemOutsDataTable = "MedicalBeneficiaryItemOutsDataTable";
        public const string MedicalBeneficiaryItemOutDetailsDataTable = "MedicalBeneficiaryItemOutDetailsDataTable";
        public const string MedicalBeneficiarysDataTable = "MedicalBeneficiarysDataTable";
        public const string MedicalItemInputSupplysDataTable = "MedicalItemInputSupplysDataTable";
        public const string MedicalItemInputSupplyDetailsDataTable = "MedicalItemInputSupplyDetailsDataTable";
        public const string MedicalItemInputSupplyDetailsViewDataTable = "MedicalItemInputSupplyDetailsViewDataTable";
        public const string MedicalAvailableItemsDataTable = "MedicalAvailableItemsDataTable";
        public const string MedicalDiscrepancysDataTable = "MedicalDiscrepancysDataTable";
        public const string MedicalDiscrepancyDetailsDataTable = "MedicalDiscrepancyDetailsDataTable";
        public const string MedicalDistributionRestrictionsDataTable = "MedicalDistributionRestrictionsDataTable";
        public const string MedicalWarehouseDataTable = "MedicalWarehouseDataTable";
        public const string MedicalPharmacyDataTable = "MedicalPharmacyDataTable";
        public const string MedicalItemInputDetailsPharmacyTransferDataTable = "MedicalItemInputDetailsPharmacyTransferDataTable";
        public const string MedicalItemInputDetailsWarehouseTransferDataTable = "MedicalItemInputDetailsWarehouseTransferDataTable";
        public const string MedicalItemTransferDetailsViewDataTable = "MedicalItemTransferDetailsViewDataTable";
        #endregion

        #region  IDC
        public const string CardIssuedsDataTable = "CardIssuedsDataTable";
        public const string CardIndividualInfosDataTable = "CardIndividualInfosDataTable";
        public const string CardPrintsDataTable = "CardPrintsDataTable";
        public const string CardValidationsDataTable = "CardValidationsDataTable";
        #endregion

        #region TTT
        public const string TendersDataTable = "TendersDataTable";
        public const string TenderBOCsDataTable = "TenderBOCsDataTable";
        public const string PurchasingReportsDataTable = "PurchasingReportsDataTable";
        #endregion

        public const string AttendanceShiftGroupStaffDataTable = "AttendanceShiftGroupStaffDataTable";
        public const string HPDevicesDataTable = "HPDevicesDataTable";
        public const string AttendanceShiftGroupsDataTable = "AttendanceShiftGroupsDataTable";
        public const string OrganizationHolidaysDataTable = "OrganizationHolidaysDataTable";

        #region FWS
        public const string PartnersContributionsDataTable = "PartnersContributionsDataTable";
        #endregion

        #region COV
        public const string CovidUNHCRResponseDataTable = "CovidUNHCRResponseDataTable";
        #endregion

        #region OSA
        public const string OfficeStaffAttendancesDataTable = "OfficeStaffAttendancesDataTable";
        public const string OfficeStaffAttendanceConfirmationsDataTable = "OfficeStaffAttendanceConfirmationsDataTable";
        #endregion

        #region GTP
        public const string GroupTwoPortalDataTable = "GroupTwoPortalDataTable";
        public const string GroupTwoApplicationsDataTable = "GroupTwoApplicationsDataTable";
        #endregion

        #region PMD
        public const string PartnerMonitoringDatabaseDataTable = "PartnerMonitoringDatabaseDataTable";
        public const string PartnerMonitoringDatabase2022DataTable = "PartnerMonitoringDatabase2022DataTable";
        public const string PartnerMonitoringDatabaseAttachementDataTable = "PartnerMonitoringDatabaseAttachementDataTable";
        public const string PartnerMonitoringDatabase2022AttachementDataTable = "PartnerMonitoringDatabase2022AttachementDataTable";

        public const string PMDPPADataTable = "PMDPPADataTable";

        public const string DispatchDataTable = "DispatchDataTable";
        public const string DispatchDetailDataTable = "DispatchDetailDataTable";
        public const string ItemsTransferDetailDataTable = "ItemsTransferDetailDataTable";
        public const string ItemsTransferDataTable = "ItemsTransferDataTable";
        public const string DamagedLostDistributionDetailDataTable = "DamagedLostDistributionDetailDataTable";
        public const string DamagedLostDistributionDataTable = "DamagedLostDistributionDataTable";
        public const string PMDWarehousesDataTable = "PMDWarehousesDataTable";
        public const string PMDWarehouseLanguagesDataTable = "PMDWarehouseLanguagesDataTable";

        //DispatchDataTable
        #endregion

        #region PCA

        public const string PartnersCapacityAssessmentsDataTable = "PartnersCapacityAssessmentsDataTable";
        #endregion
    }

    public static class DataTableTypes
    {
        public const string Index = "Index";
        public const string Field = "Field";
    }

    public static class DataTableEditMode
    {
        public const string Modal = "<i class='openModal'>...</i>";
        public const string Page = "<i class='fa fa-angle-right fa-15x dt-arrow'></i>";
        public const string None = "";
        public const string NoCheckBox = "NoCheckBox";
    }

    public static class FilterDataTypes //or DataTableFilterDataTypes? also we have DataTableOptions.cs should we put them there !!
    {
        public const string Number = "Number";
        public const string Date = "Date";
        public const string DateTime = "DateTime";
        public const string Text = "Text";
        public const string Options = "Options";
        public const string Boolean = "Boolean";
    }

    public static class SubmitTypes
    {
        public const string Create = "Create";
        public const string Update = "Update";
        public const string Delete = "Delete";
        public const string Restore = "Restore";
    }

    public static class MessageTypes
    {
        public const string Success = "success";
        public const string Warning = "warning";
        public const string Error = "error";
        public const string Info = "info";
    }

    public static class TransactionTypes
    {
        public const int Grant = 0;
        public const int Revoke = 1;
    }

    public static class OrganizationsTypes
    {
        public readonly static Guid International = Guid.Parse("55369FFB-863A-4167-A20E-DC8F44F7D5F9");
    }

    public static class FactorTypes
    {
        //This codes should match the black box stored procedure 
        public readonly static Guid Bureaus = Guid.Parse("00000000-0000-0000-0000-000000000001");
        public readonly static Guid Operations = Guid.Parse("00000000-0000-0000-0000-000000000002");
        public readonly static Guid Organizations = Guid.Parse("00000000-0000-0000-0000-000000000003");
        public readonly static Guid Countries = Guid.Parse("00000000-0000-0000-0000-000000000004");
        public readonly static Guid Locations = Guid.Parse("00000000-0000-0000-0000-000000000004");
        public readonly static Guid DutyStations = Guid.Parse("00000000-0000-0000-0000-000000000005");
        public readonly static Guid ContractTypes = Guid.Parse("00000000-0000-0000-0000-000000000006");
        public readonly static Guid LocationLevelID = Guid.Parse("00000000-0000-0000-0000-000000000007");
        public readonly static Guid Tables = Guid.Parse("00000000-0000-0000-0000-000000000008");
        public readonly static Guid OrganizationsInstances = Guid.Parse("00000000-0000-0000-0000-000000000009");
        public readonly static Guid PMDObjectives = Guid.Parse("305f1344-c135-4ed3-affb-f924fe29c6aa");
        public readonly static Guid PMDOutcomes = Guid.Parse("AD64D9F9-B099-4900-83E3-2AB5ABFFBCE1");
        public readonly static Guid PMDOutputs = Guid.Parse("DD807851-939F-4BEE-80B4-C639F323430A");
        public readonly static Guid PMDIndicators = Guid.Parse("9185C2E0-354B-47F1-A8C1-89950F438E57");
    }

    public static class Apps
    {
        public readonly static Guid CMS = Guid.Parse("00000000-0000-0000-0000-000000000001");
        public readonly static Guid OVS = Guid.Parse("00000000-0000-0000-0000-000000000002");
        public readonly static Guid TBS = Guid.Parse("00000000-0000-0000-0000-000000000003");
        public readonly static Guid AMS = Guid.Parse("00000000-0000-0000-0000-000000000004");
        public readonly static Guid ATS = Guid.Parse("00000000-0000-0000-0000-000000000005");
        public readonly static Guid VMS = Guid.Parse("00000000-0000-0000-0000-000000000006");
        public readonly static Guid WMS = Guid.Parse("00000000-0000-0000-0000-000000000007");
        public readonly static Guid PPE = Guid.Parse("00000000-0000-0000-0000-000000000008");
        public readonly static Guid PQC = Guid.Parse("00000000-0000-0000-0000-000000000009");
        public readonly static Guid PRG = Guid.Parse("00000000-0000-0000-0000-000000000010");
        public readonly static Guid CDT = Guid.Parse("00000000-0000-0000-0000-000000000012");
        public readonly static Guid PTD = Guid.Parse("00000000-0000-0000-0000-000000000013");
        public readonly static Guid REF = Guid.Parse("00000000-0000-0000-0000-000000000014");
        public readonly static Guid IDC = Guid.Parse("00000000-0000-0000-0000-000000000015");
        public readonly static Guid MVT = Guid.Parse("00000000-0000-0000-0000-000000000016");
        public readonly static Guid SHM = Guid.Parse("00000000-0000-0000-0000-000000000017");
        public readonly static Guid STI = Guid.Parse("00000000-0000-0000-0000-000000000018");
        public readonly static Guid PPA = Guid.Parse("238E29C6-EEC9-43BF-B899-B9C031A9A707");
        public readonly static Guid SRS = Guid.Parse("AA9D1B43-6396-4A12-94A5-F788D07526F8");
        public readonly static Guid PCR = Guid.Parse("23297CD9-7ABB-4B92-8302-A4E5F3A58AAC");
        public readonly static Guid RMS = Guid.Parse("5155E3CB-9741-43FD-B7FD-45E9B2BB4784");
        public readonly static Guid IMS = Guid.Parse("9230D3CD-6E2D-4BAA-ABAD-189D1727CBC6"); 
      
        public readonly static Guid MRS = Guid.Parse("B6A8F4EB-82A1-4C8B-9ECB-1D80258EAA45");
        public readonly static Guid ORG = Guid.Parse("B2CD1671-ECF4-4905-8FFA-F486CBA09D2A");
        public readonly static Guid SAS = Guid.Parse("5AC9FEED-E3EA-4DD9-A563-3885ABC8A986");
        public readonly static Guid SIP = Guid.Parse("4D137169-65FE-41AF-A4B6-2E6EC7F5F246");
        public readonly static Guid AHD = Guid.Parse("F18DEA05-B3E5-43E4-B1BC-900D0B66D77E");
        public readonly static Guid ISS = Guid.Parse("B203D219-5565-443D-BC1C-AB2539308839");
        public readonly static Guid EMT = Guid.Parse("1A52FE1D-4C1F-4DA5-8A8D-DF0BB4E7B09C");
        public readonly static Guid DAS = Guid.Parse("49E50148-212E-4AEC-9E72-14DFED4967A8");
        public readonly static Guid TTT = Guid.Parse("77078B91-535F-44FA-9D11-5946316B416E");
        public readonly static Guid FWS = Guid.Parse("D223FBB7-EA2A-4B67-8CBC-620906C62468");
        public readonly static Guid COV = Guid.Parse("943C769A-32C9-4F4B-A1A8-F7EB368293FE");
        public readonly static Guid OSA = Guid.Parse("650F8B4D-A4FD-4DCB-8C6E-32F6771B3C1D");
        public readonly static Guid RTS = Guid.Parse("943c769a-32c9-4f4b-a1a8-f7eb36829892");

        public readonly static Guid GTP = Guid.Parse("B572C037-26C5-4C63-888F-00087DB5ED3A");

        public readonly static Guid PMD = Guid.Parse("70f25445-df45-4f17-a409-12cb1636090e");
        public readonly static Guid HIMS = Guid.Parse("943c769a-32c9-4f4b-a1a8-f7eb36829111");
        public readonly static Guid RAMS = Guid.Parse("E800BBDA-FBB0-480A-B1AE-715394389D24");
        public readonly static Guid PCA = Guid.Parse("EA1D92E5-26CB-422D-B17A-2A5F364367EE");
        public readonly static Guid AST = Guid.Parse("943c769a-32c9-4f4b-a1a8-f7eb36829123");
        public readonly static Guid SDS = Guid.Parse("943c769a-32c9-4f4b-a1a8-f7eb36828888");
    }

    #region WMS

    public static class DamagedReportFlowStatus
    {
        public readonly static Guid Draft = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7111");
        public readonly static Guid Submitted = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7112");
        public readonly static Guid ICTVerified = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7113");
        public readonly static Guid AdminVerified = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7114");
        public readonly static Guid FinanceVerifid = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7115");
        public readonly static Guid Closed = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7697");

    }
    public static class STIDamagedItemReimbursementDecision
    {
        public readonly static Guid StaffHasToReimburse = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7596");
        public readonly static Guid NoReimburse = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7592");


    }
    public static class ItemVerificationStatus
    {
        public readonly static Guid Pending = Guid.Parse("d5e2a4df-c67e-4f4d-a242-7e33c381e734");
        public readonly static Guid Confirmed = Guid.Parse("d5e2a4df-c67e-4f4d-a242-7e33c381e735");

    }
    public static class WarehouseRequestSourceTypes
    {
        public readonly static Guid Staff = Guid.Parse("D5E2A4DF-C67E-4F4D-A242-7E33C381EC11");
        public readonly static Guid Warehouse = Guid.Parse("D5E2A4DF-C67E-4F4D-A242-7E33C381EC12");
        public readonly static Guid OtherRequester = Guid.Parse("d5e2a4df-c67e-4f4d-a242-7e33c381ec16");
        public readonly static Guid Vehicle = Guid.Parse("d5e2a4df-c67e-4f4d-a242-7e33c381ec17");
        public readonly static Guid Partner = Guid.Parse("d5e2a4df-c67e-4f4d-a242-7e33c381e777");
        public readonly static Guid GS45 = Guid.Parse("D5E2A4DF-C67E-4F4D-A242-7E33C381EC79");
    }
    public static class WarehouseConsumableRequestSourceTypes
    {
        public readonly static Guid Warehouse = Guid.Parse("d5e2a4df-c67e-4f4d-a242-7e33c381e781");
        public readonly static Guid Printers = Guid.Parse("d5e2a4df-c67e-4f4d-a242-7e33c381e782");
    }

    public static class ItemDeterminants
    {
        public readonly static Guid Barcode = Guid.Parse("D5E2A4DF-C67E-4F4D-A242-7E33C981EC11");
        public readonly static Guid SerialNumber = Guid.Parse("D5E2A4DF-C67E-4F4D-A242-7E33C981EC12");
        public readonly static Guid IME1 = Guid.Parse("D5E2A4DF-C67E-4F4D-A242-7E33C981EC13");
        public readonly static Guid IME2 = Guid.Parse("d5e2a4df-c67e-4f4d-a242-3e33c981ec17");
        public readonly static Guid GSM = Guid.Parse("D5E2A4DF-C67E-4F4D-A242-7E33C987EC11");
        public readonly static Guid MAC = Guid.Parse("D5E2A4DF-C67E-4F4D-A242-7E33C981EC17");
        public readonly static Guid Phone = Guid.Parse("d5e2a4df-c67e-4f4d-a242-7e33c981ec15");
        public readonly static Guid MSRPID = Guid.Parse("d5e2a4df-c67e-4f4d-a242-7e99c981ec11");
        public readonly static Guid SeqNumber = Guid.Parse("D5E2A4DF-C67E-4F4D-A242-7E33C981E897"); 
    }
    public static class WarehouseItemStatus
    {
        //inService
        //inService
        public readonly static Guid Functionting = Guid.Parse("675DE853-151B-4C2F-93F4-DA1435EEE761");
        public readonly static Guid Lost = Guid.Parse("675DE853-151B-4C2F-93F4-DA1434EEE765");
        public readonly static Guid Damaged = Guid.Parse("675DE853-151B-4C2F-93F4-DA1434EEE766");
        public readonly static Guid Maintaince = Guid.Parse("675DE853-151B-4C2F-93F4-DA1434EEE769");
        public readonly static Guid GS45 = Guid.Parse("675DE853-151B-4C2F-93F4-DA1434EEE767");
        public readonly static Guid ForDisposal = Guid.Parse("675de853-151b-4c2f-93f4-da1432eee767");
        public readonly static Guid Disposed = Guid.Parse("675de853-151b-4c2f-93f4-da1434eee768");
        public readonly static Guid Stopped = Guid.Parse("675DE853-151B-4C2F-93F4-DA1434EEE788");

        public readonly static Guid Reserved = Guid.Parse("675DE853-151B-4C2F-93F4-DA1434EEE789");
        public readonly static Guid Donated = Guid.Parse("675DE853-151B-4C2F-93F4-DA1434EEE789");
        public readonly static Guid ForDonation = Guid.Parse("675DE853-151B-4C2F-93F4-DA1434EEE789");
        public readonly static Guid NotFunctioning = Guid.Parse("675de853-151b-4c2f-93f4-da1434eee797");
        public readonly static Guid New = Guid.Parse("675de853-151b-4c2f-93f4-da1434ee4736");
        //public readonly static Guid SpareParts = Guid.Parse("675de853-151b-4c2f-93f4-da1434eee738");

        //public readonly static Guid Transferred = Guid.Parse("f35d454a-33f1-4e8f-8438-aaaee8d0fc5c");
        //public readonly static Guid ForDonationReserved = Guid.Parse("b9f1fed0-062e-4fcd-9eab-c656d42dc454");




        public readonly static Guid ForDisposalDamaged = Guid.Parse("ae062ad4-b062-4dae-acc8-01437c19575d");
        public readonly static Guid ForDisposalsparepart = Guid.Parse("dafbb1d8-88b9-4ce5-84b0-222dd983801b");
        public readonly static Guid ForDisposalTheft = Guid.Parse("d667def6-cfdb-4095-943a-337e1e7dc0dd");
        public readonly static Guid ForDisposalSale = Guid.Parse("9c39a0ab-4f52-4b7a-b6e4-e13b88e69d1f");

    }
    public static class WarehouseServiceItemStatus
    {
        //a40ec252-622e-4ff1-9ef4-e328c7a3cec8
        public readonly static Guid InStock = Guid.Parse("a40ec252-622e-4ff1-9ef4-e328c7a3cec7");
        public readonly static Guid InService = Guid.Parse("a40ec252-622e-4ff1-9ef4-e328c7a3cec9");
        //public readonly static Guid ForDisposal = Guid.Parse("a40ec252-622e-4ff1-9ef4-e378c7a3cec7");
        public readonly static Guid GS45 = Guid.Parse("a40ec252-622e-4ff1-9ef4-e328c7a3cec3");
        //public readonly static Guid Stopped = Guid.Parse("a40ec252-622e-4ff1-9ef4-e328c7a3ce77");

    }

    public static class WarehouseItemCondition
    {
        //tableGUId a40ec252-622e-4ff1-9ef4-e328c7a3cec9
        public readonly static Guid New = Guid.Parse("a40ec252-622e-4ff1-9ef4-e328c7f3cec9");
        public readonly static Guid Used = Guid.Parse("a40ec252-622e-4ff1-9ef4-e323c7a3cec9");


    }
    public static class DisposalItemStatus
    {
        public readonly static Guid Donation = Guid.Parse("0A31225F-2064-4F02-93D2-FE1519CF6AC7");
        public readonly static Guid Sale = Guid.Parse("0A31225F-2064-4F02-93D2-FE1519CF6AC2");
        public readonly static Guid SaleAsScrap = Guid.Parse("0A31225F-2064-4F02-93D2-FE1519CF6AC3");
        public readonly static Guid Scrap = Guid.Parse("0A31225F-2064-4F02-93D2-FE1519CF6AC4");
        public readonly static Guid SparePart = Guid.Parse("0A31225F-2064-4F02-93D2-FE1549CF6AC5");
    }

    public static class WarehouseInputSource
    {
        public readonly static Guid DirectPurchase = Guid.Parse("D5E2A4DF-C67E-4F4D-A242-7E33C331EC99");
        public readonly static Guid HQOrder = Guid.Parse("D5E2A4DF-C67E-4F4D-A242-7E33C381EC99");
        public readonly static Guid LocalVendorSupplier = Guid.Parse("D5E2A4DF-C67E-4F4D-A242-7E33C381EC93");

    }



    public static class WarhosueModelDamagedTypes
    {
        public readonly static Guid Damaged = Guid.Parse("1C44822F-A898-476D-B291-CAF1B0551AC7");
        public readonly static Guid Lost = Guid.Parse("2C44822F-A898-476D-B291-CAF1B0551AC7");
        public readonly static Guid MRS = Guid.Parse("B6A8F4EB-82A1-4C8B-9ECB-1D80258EAA45");
        public readonly static Guid TTT = Guid.Parse("77078B91-535F-44FA-9D11-5946316B416E");
    }
    public static class ItemReservingType
    {
        public readonly static Guid Staff = Guid.Parse("2e8646e2-ccec-447e-840e-2a43686f498a");
        public readonly static Guid Unit = Guid.Parse("2e8646e2-ccec-447e-840e-2a43686f4982");

    }
    public static class WarehouseRequestFlowType
    {
        public readonly static Guid Requested = Guid.Parse("0c44822f-a898-476d-b291-caf1b055aac8");
        public readonly static Guid PendingConfirmed = Guid.Parse("0C44822F-A898-476D-B291-CAF1B055AAC6");
        public readonly static Guid Confirmed = Guid.Parse("0C44822F-A898-476D-B291-CAF1B055AAC2");
        public readonly static Guid Returned = Guid.Parse("0C44822F-A898-476D-B291-CAF1B055AAC3");
        public readonly static Guid Reserved = Guid.Parse("0c44822f-a898-476d-b291-caf1b055aac5");
        public readonly static Guid Cancelled = Guid.Parse("0c44822f-a898-476d-b291-caf1b055aac9");

    }
    public static class WarehosueNotificationType
    {
        public readonly static Guid Notification = Guid.Parse("a40ec252-622e-4ff1-9ef4-e32837a3ce17");
        public readonly static Guid Reminder = Guid.Parse("a40ec252-622e-4ff1-9ef4-e328c7a3ce12");
        public readonly static Guid AutoReminder = Guid.Parse("a40ec252-622e-4ff1-9ef4-e328c755ce12");
    }
    #endregion

    #region AHD

    public static class CodeCheckListNextConfirmationType
    {
        //a40ec252-622e-4ff1-9ef4-e328c7a3cec8
        public readonly static Guid StaffThenFocalPoint = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e9123");
        public readonly static Guid FocalPoint = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e9166");



    }
    public static class CycleSalaryFlowStep
    {
        //a40ec252-622e-4ff1-9ef4-e328c7a3cec8
        //page
        public readonly static Guid NotifyStaffToConfirmBankInformation = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7359");
        //page
        public readonly static Guid ImportHQSalary = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7333");
        //modal
        public readonly static Guid DangerPay = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7354");
        //page
        public readonly static Guid Overtime = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7335");
        public readonly static Guid MIP = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7356");
        public readonly static Guid Billing = Guid.Parse("66cd375c-a576-4aa4-8af4-f43c1c5e7357");
        public readonly static Guid SalaryOverview = Guid.Parse("66cd375c-a576-4aa4-8af4-ff371c5e7359");

    }
    public static class StaffSalaryFlowStatus
    {
        //a40ec252-622e-4ff1-9ef4-e328c7a3cec8
        public readonly static Guid PendingBankConfirmation = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7339");
        public readonly static Guid BankConfirmed = Guid.Parse("66cd375c-a576-4aa4-8af4-ff6c1c5e7356");



    }
    public static class OvertimeFlowStatus
    {
        //a40ec252-622e-4ff1-9ef4-e328c7a3cec8
        public readonly static Guid PendingSupervisorReview = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7369");
        public readonly static Guid PendingCertifying = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7362");

        public readonly static Guid Canceled = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7363");
        public readonly static Guid Approved = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7367");
        public readonly static Guid Submitted = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7311");

    }

    public static class InternationalStaffEntitlmentFlowStatus
    {
        //a40ec252-622e-4ff1-9ef4-e328c7a3cec8
        public readonly static Guid Submitted = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3676");
        public readonly static Guid PendingVerification = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3671");

        public readonly static Guid PendingCertify = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3672");
        public readonly static Guid PendingFinanceApproval = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3674");
        public readonly static Guid Closed = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3675");
        public readonly static Guid ConfirmReceipt = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3691");

    }
    public static class InternationalStaffRAndRLeaveFlowStatus
    {
        public readonly static Guid PendingHRApproval = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3632");
        public readonly static Guid PendingSupervisorApproval = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3634");
        public readonly static Guid PendingHRReview = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3666");
        public readonly static Guid PendingRepresentativeApproval = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e9996");
        public readonly static Guid Approved = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e9991");
        public readonly static Guid Closed = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e9992");
        public readonly static Guid RejectedBySupervior = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c773777");
        public readonly static Guid RejectedByRepresentative = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c773772");


    }
    public static class VehicleMaintenanceRequestFlowStatus
    {
        public readonly static Guid Submitted = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3614");
        public readonly static Guid Closed = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3615");
        public readonly static Guid Cancelled = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3444");


    }
    public static class AHDMissionStatusFlow
    {
        //29591f83-32a4-49a5-b284-2a8efbc44453
        public readonly static Guid Draft = Guid.Parse("29591f83-32a4-49a5-b284-2a8efbc44453");
        public readonly static Guid Submitted = Guid.Parse("29591f83-32a4-49a5-b284-2a8efbc44452");
        public readonly static Guid Verified = Guid.Parse("29591f83-32a4-49a5-b284-2a8efbc44454");
        public readonly static Guid Reviewed = Guid.Parse("29591f83-32a4-49a5-b284-2a8efbc44449");
        public readonly static Guid Approved = Guid.Parse("29591f83-32a4-49a5-b284-2a8efbc44781");
        public readonly static Guid Canceled = Guid.Parse("29591f83-32a4-49a5-b284-2a8efbc44331");
        public readonly static Guid Returned = Guid.Parse("29591f83-32a4-49a5-b284-2a8efbc44388");
        public readonly static Guid Rejected = Guid.Parse("29591f83-32a4-49a5-b284-2a8efbc44324");


    }
    public static class SeparationFormStatus
    {
        //a40ec252-622e-4ff1-9ef4-e328c7a3cec8
        public readonly static Guid Pending = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e9196");
        public readonly static Guid Closed = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e9133");

    }
    public static class SeparationCheckListStatus
    {
        //a40ec252-622e-4ff1-9ef4-e328c7a3cec8
        public readonly static Guid PendingStaffConfirmation = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e9147");
        public readonly static Guid ClosedStatus = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e9135");
        public readonly static Guid PendingFPConfirmation = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e9333");



    }
    //public static class SeparationCheckListStatus
    //{
    //    //a40ec252-622e-4ff1-9ef4-e328c7a3cec8
    //    public readonly static Guid Pending = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e9147");
    //    public readonly static Guid Closed = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e9135");

    //}
    public static class AHDActionFlowStatus
    {
        public readonly static Guid Pending = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7547");
        public readonly static Guid Confirmed = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7547");
        public readonly static Guid Cancelled = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7547");


    }
    public static class InternationalStaffAttendanceType
    {
        public readonly static Guid AnnualLeave = Guid.Parse("13AFCA94-4FA0-479A-85DB-8AEF4BC64CBB");
        public readonly static Guid UncertificatedSickLeave = Guid.Parse("04D675FE-FE7E-4680-A33D-AD28BD795532");

        public readonly static Guid CertificatedSickLeave = Guid.Parse("4E8AC711-5335-4583-B3FC-B29367ABD22A");
        public readonly static Guid R_R_leave = Guid.Parse("67979D6D-5B7B-4A85-AA6D-CD604A1BDF75");
        public readonly static Guid TeleWorking = Guid.Parse("67979d6d-5b7b-4a85-aa6d-cd604a1b4567");


    }
    public static class coddeInternationalStaffAttendanceTypeAttendanceTable
    {
        public readonly static Guid AnnualLeave = Guid.Parse("13AFCA94-4FA0-479A-85DB-8AEF4BC64CBB");
        public readonly static Guid UncerticSickLeave = Guid.Parse("04D675FE-FE7E-4680-A33D-AD28BD795532");
        public readonly static Guid CertificatedSickLeave = Guid.Parse("4E8AC711-5335-4583-B3FC-B29367ABD22A");
        public readonly static Guid TravelTime = Guid.Parse("A1A0A314-388C-4E21-AB91-B919439AA794");
        public readonly static Guid OfficialHoliday = Guid.Parse("A1A0A314-388C-4E21-AB91-B919439AA797");
        public readonly static Guid Weekend = Guid.Parse("A1A0A314-388C-4E21-AB91-B919439AA798");
        public readonly static Guid TeleCommuniting = Guid.Parse("67979D6D-5B7B-4A85-AA6D-CD604A1B4567");
        public readonly static Guid RR = Guid.Parse("67979D6D-5B7B-4A85-AA6D-CD604A1BDF75");
        public readonly static Guid SLWFP = Guid.Parse("04d675fe-fe7e-4680-a33d-ad28bd795533");

        public readonly static Guid Mission = Guid.Parse("67979d6d-5b7b-4a85-aa6d-cd604a1bdf69");
        public readonly static Guid Quarantine = Guid.Parse("67979d6d-5b7b-4a85-aa6d-cd604a1bdf88");
        public readonly static Guid TravelPurpose = Guid.Parse("67979d6d-5b7b-4a85-aa6d-cd604a1bdf33");
        public readonly static Guid FamilyLeave = Guid.Parse("67979d6d-5b7b-4a85-aa6d-cd604a1bdf77");
        public readonly static Guid HomeLeave = Guid.Parse("67979d6d-5b7b-4a85-aa6d-cd604a1bdf87");
        public readonly static Guid PaternityLeave = Guid.Parse("67979d6d-5b7b-4a85-aa6d-cd604a1bdf86");

    }
    public static class InternationalStaffEntitlementType
    {
        public readonly static Guid AddedRecorvery = Guid.Parse("DC4FF146-2806-4BC0-89B7-5259D876E4F8");
        public readonly static Guid RentalDeduction = Guid.Parse("C328588B-33C5-4E38-85B0-6409704BE434");

        public readonly static Guid DangerPayPerDay = Guid.Parse("f6dc8b13-029a-42bc-bab5-6f8fa691823c");
        public readonly static Guid R_RTicket = Guid.Parse("4943b228-e39e-457d-9ae8-c1e0bcce6119");
        public readonly static Guid DeductedRecovery = Guid.Parse("3620cd9b-76d3-4d77-bfd4-db4a17492931");
        public readonly static Guid BreakfastDeduction = Guid.Parse("79A9657F-18DE-4676-A9CB-59A7F470D449");

    }


    public static class RenewalResidencyFormStatus
    {
        public readonly static Guid ApprovedbyMOFA = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3669");
        public readonly static Guid Cancelled = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3662");
        public readonly static Guid StampedOnUNLP = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3663");
        public readonly static Guid Submitted = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3664");
        public readonly static Guid NotApproved = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3667");

    }



    public static class NationalStaffDangerPaConfirmationStatus
    {
        //a40ec252-622e-4ff1-9ef4-e328c7a3cec8
        public readonly static Guid Pending = Guid.Parse("a40ec252-622e-4ff1-9ef4-e323c7a3cec5");
        public readonly static Guid Confirmed = Guid.Parse("a40ec252-622e-4ff1-9ef4-e323c7a3cec7");

    }
    public static class StaffProfileConfirmation
    {
        //a40ec252-622e-4ff1-9ef4-e328c7a3cec8
        public readonly static Guid Pending = Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E3747");
        public readonly static Guid Confirmed = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3742");

    }

    public static class NationalStaffLeaveType
    {
        //a40ec252-622e-4ff1-9ef4-e328c7a3cec8
        public readonly static Guid MissionInsideSyria = Guid.Parse("c479de72-c6f3-49a9-9d3c-140ad1217999");
        public readonly static Guid Mission = Guid.Parse("c479de72-c6f3-49a9-9d3c-140ad1217177");
        public readonly static Guid AnnualLeave = Guid.Parse("c479de72-c6f3-49a9-9d3c-140ad1217178");
        public readonly static Guid MaternityPaternity = Guid.Parse("c479de72-c6f3-49a9-9d3c-140ad1217179");
        public readonly static Guid SickLeave = Guid.Parse("c479de72-c6f3-49a9-9d3c-140ad1217176");
        public readonly static Guid Weekends = Guid.Parse("c479de72-c6f3-49a9-9d3c-140ad1817179");
        public readonly static Guid TelecommutingOutSide = Guid.Parse("c479de72-c6f3-49a9-9d3c-140ad1217988");
        public readonly static Guid SLWOP = Guid.Parse("c479de72-c6f3-49a9-9d3c-140ad1217967");

    }
    #endregion

    #region IMS
    public static class MissionActionTakenStatus
    {
        //a40ec252-622e-4ff1-9ef4-e328c7a3cec8
        public readonly static Guid Planning = Guid.Parse("a40ec252-622e-4ff1-9ef4-e328c7d3cec3");
        public readonly static Guid Ongoing = Guid.Parse("a40ec252-622e-4ff1-9ef4-e324c1d3cec3");
        public readonly static Guid Completed = Guid.Parse("a40ec252-622e-4ff1-9ef4-e328c3d3cec3");
    }
    #endregion

    #region DAS
    public static class ScanDocumentStatus
    {
        //a40ec252-622e-4ff1-9ef4-e328c7a3cec8
        public readonly static Guid Pending = Guid.Parse("a40ec252-622e-4ff1-9ef4-e328c7a9cec1");
        public readonly static Guid Confirmed = Guid.Parse("a40ec252-622e-4ff1-9ef4-e328c7a9cec2");

    }
    public static class DocumentVerificationStatus
    {
        //a40ec252-622e-4ff1-9ef4-e328c7a3cec8
        public readonly static Guid Submitted = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7588");
        public readonly static Guid PendingVerification = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7582");
        public readonly static Guid Confirmed = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7583");
        public readonly static Guid PendingReview = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7584");

    }

    public static class FileRequestStatus
    {
        //a40ec252-622e-4ff1-9ef4-e328c7a3cec8
        public readonly static Guid Requested = Guid.Parse("09feaa17-8ff3-46d6-a446-fe8242fed991");
        public readonly static Guid Delivered = Guid.Parse("09feaa17-8ff3-46d6-a446-fe8242fed992");
        public readonly static Guid Canclled = Guid.Parse("09feaa17-8ff3-46d6-a446-fe8242fed999");
        public readonly static Guid Closed = Guid.Parse("09feaa17-8ff3-46d6-a446-fe8242fed936");
    }

    public static class ScanDocumentTransferFlowStatus
    {
        //a40ec252-622e-4ff1-9ef4-e328c7a3cec8
        public readonly static Guid Pending = Guid.Parse("a40ec252-622e-4ff1-9ef4-e328c7a9cec8");
        public readonly static Guid Confirmed = Guid.Parse("a40ec252-622e-4ff1-9ef4-e328c7a9cec5");
        public readonly static Guid Returned = Guid.Parse("a40ec252-622e-4ff1-9ef4-e328c7a9ce77");

    }

    public static class DASDocumentCustodianType
    {
        public readonly static Guid Staff = Guid.Parse("09feaa17-8ff3-46d6-a446-fe8242fed382");
        public readonly static Guid UNIT = Guid.Parse("09feaa17-8ff3-46d6-a446-fe8242fed383");
    }
    public static class DASTemplateOwnerTypes
    {
        public readonly static Guid Staff = Guid.Parse("29591f83-32a4-49a5-b284-2a8efbc447c2");
        public readonly static Guid Refugee = Guid.Parse("29591f83-32a4-49a5-b284-2a8efbc447c4");
        public readonly static Guid Agreement = Guid.Parse("29591f83-32a4-49a5-b284-2a8efbc447c5");
        public readonly static Guid Assets = Guid.Parse("29591f83-32a4-49a5-b284-2a8efbc447c6");
    }

    public static class DASDocumentUnitName
    {
        public readonly static Guid FilingUnit = Guid.Parse("a753c510-a13d-4350-af7c-0001b806dfc3");

    }

    public class DASProGresSiteNames
    {

        public static Guid SYRDA = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3133");
        public static Guid SYRM1 = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3132");
        public static Guid SYRM2 = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3179");
        public static Guid SYRTS = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3176");
        public static Guid SYRHM = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3122");
    }

    public class ScannerSettings
    {

        public static Guid PaperFormat = Guid.Parse("a7ba6b74-dc7e-4c2b-a9a1-322ba5188339");
        public static Guid Resolution = Guid.Parse("E39B1861-B31F-4595-8384-728593ED43D3");
        public static Guid PaperSize = Guid.Parse("fe42f846-f671-4230-ba02-cfb283bfba84");
        public static Guid ScanningType = Guid.Parse("f2152b55-2fea-4f69-9257-d4411fda8224");
        public static Guid ColorMod = Guid.Parse("315c18c9-8d43-4d4c-9948-fb6524f8a51e");
    }

    public static class FileTypes
    {
        public readonly static Guid Excel = Guid.Parse("00000000-0000-0000-0000-000000000001");
        public readonly static Guid Word = Guid.Parse("00000000-0000-0000-0000-000000000002");
        public readonly static Guid PDF = Guid.Parse("00000000-0000-0000-0000-000000000003");
        public readonly static Guid Other = Guid.Parse("00000000-9999-0000-0000-000000000000");
    }

    #endregion

 

    public static class PPAFileStatus
    {
        public readonly static Guid Pending = Guid.Parse("00000000-0000-0000-0000-000000000001");
    }

    public static class PPAUserAccessType
    {
        public readonly static Guid FullAccess = Guid.Parse("00000000-0000-0000-0000-000000000001");
        public readonly static Guid CC1WithAccess = Guid.Parse("00000000-0000-0000-0000-000000000002");
        public readonly static Guid CC2WithoutAccess = Guid.Parse("00000000-0000-0000-0000-000000000003");
    }

    public static class FileVersionStatuses
    {
        public readonly static Guid Pending = Guid.Parse("536CC824-8B86-4211-A65D-109B9489EE0E");
        public readonly static Guid Downloaded = Guid.Parse("ECD33FE1-3AED-4E0C-ABAA-61C87983ED7F");
        public readonly static Guid Completed = Guid.Parse("FA20C61B-149A-402B-BB88-24FF624F3020");
        public readonly static Guid Approved = Guid.Parse("C37090AB-8C42-45BB-B346-62DCA59833A5");
        public readonly static Guid Rejected = Guid.Parse("5DE6D979-FB77-41AC-A8EB-9BC3A3C4ABBD");
        public readonly static Guid Uploaded = Guid.Parse("E3C8B996-3149-42AA-9FCF-0B5E8D761F7A");
    }

    public static class ReferralStepConstants
    {
        public readonly static Guid AppointmentCompleted = Guid.Parse("16a79a66-da57-4952-8724-02b6b3ce44c9");
        public readonly static Guid AppointmentRescheduled = Guid.Parse("c5e08cfc-de8d-420c-a042-6c3fba4013ac");
        public readonly static Guid AppointmentArrivedCanceled = Guid.Parse("fa7547ca-b5c8-4b60-8212-96dd13afba84");
        public readonly static Guid AppointmentAssigned = Guid.Parse("bc949991-efe6-40b1-b422-c7e6cc25fadd");

    }

    public static class ReferralStatusConstants
    {
        public readonly static Guid Pending = Guid.Parse("cd236c9f-7918-4af1-9ca2-dcb42f9668f5");
        public readonly static Guid NotStarted = Guid.Parse("1295b8f4-f79c-404d-90d1-39c636daa12e");
        public readonly static Guid Complete = Guid.Parse("51a9d9e6-cbdd-4161-bd4e-58fa940b3d08");
        public readonly static Guid Closed = Guid.Parse("e6e80945-55ec-449f-b1c4-d57bf64e8cc7");
        public readonly static Guid InProgress = Guid.Parse("8022ba39-0b94-48a5-9a86-da5f078871b1");
    }

    public static class Permissions
    {
        public static class PMDWarehouse
        {
            // Access PMDWarehouse
            public readonly static int Access = 39;
            public readonly static Guid AccessGuid = Guid.Parse("769b50fd-d721-48e0-9227-0b9b2025ef3f");

            // Create PMDWarehouse
            public readonly static int Create = 40;
            public readonly static Guid CreateGuid = Guid.Parse("4d5f8c7f-747a-49b0-a939-8b3749f0cbc2");

            // Delete PMDWarehouse
            public readonly static int Delete = 41;
            public readonly static Guid DeleteGuid = Guid.Parse("368a12bd-7702-4cda-8c07-ec6d008e4d31");

            // Restore PMDWarehouse
            public readonly static int Restore = 42;
            public readonly static Guid RestoreGuid = Guid.Parse("d1c02341-daff-4f92-b809-d053d4406a28");

            // Update PMDWarehouse
            public readonly static int Update = 43;
            public readonly static Guid UpdateGuid = Guid.Parse("71d85474-32aa-4596-b882-fa1736c1eff4");

        }
        public static class PMDDamagedLostDistribution
        {
            // Access PMD Damaged Lost Distribution
            public readonly static int Access = 34;
            public readonly static Guid AccessGuid = Guid.Parse("bb2544a3-bd6d-43fd-9b85-f775d909edb6");

            // Create PMD Damaged Lost Distribution
            public readonly static int Create = 35;
            public readonly static Guid CreateGuid = Guid.Parse("669ce16f-72a9-41c9-8953-60065b6eb7da");

            // Delete PMD Damaged Lost Distribution
            public readonly static int Delete = 36;
            public readonly static Guid DeleteGuid = Guid.Parse("88b393da-d5f2-4ee7-9b88-70642aad5dda");

            // Restore PMD Damaged Lost Distribution
            public readonly static int Restore = 37;
            public readonly static Guid RestoreGuid = Guid.Parse("ded9d03b-dd93-411f-85f2-038592df21f4");

            // Update PMD Damaged Lost Distribution
            public readonly static int Update = 38;
            public readonly static Guid UpdateGuid = Guid.Parse("f05d0c70-c3cf-416a-83a7-48c743d23cc5");

        }
        public static class PMDItemsTransfer
        {
            // Access PMD Items Transfer
            public readonly static int Access = 29;
            public readonly static Guid AccessGuid = Guid.Parse("27c3d00f-3aef-48b2-bc61-f535fa0bde02");

            // Create PMD Items Transfer
            public readonly static int Create = 30;
            public readonly static Guid CreateGuid = Guid.Parse("12f162d8-aa84-46b5-9233-4387019ff920");

            // Delete PMD Items Transfer
            public readonly static int Delete = 31;
            public readonly static Guid DeleteGuid = Guid.Parse("5f49e677-8470-45e8-b7cf-f31dfeab6dba");

            // Restore PMD Items Transfer
            public readonly static int Restore = 32;
            public readonly static Guid RestoreGuid = Guid.Parse("3ab04c08-fec6-4b43-90df-86ad041aad63");

            // Update PMD Items Transfer
            public readonly static int Update = 33;
            public readonly static Guid UpdateGuid = Guid.Parse("9bf4f585-59a4-44ea-88fa-a20d59a7f41d");

        }
        public static class PMDDispatch
        {
            // Access PMD Dispatch
            public readonly static int Access = 24;
            public readonly static Guid AccessGuid = Guid.Parse("cc0e78a0-4ef3-411f-a751-22a38de890b3");

            // Create PMD Dispatch
            public readonly static int Create = 25;
            public readonly static Guid CreateGuid = Guid.Parse("0c25fdfa-8785-4dce-becd-b78d8480cb1b");

            // Delete PMD Dispatch
            public readonly static int Delete = 26;
            public readonly static Guid DeleteGuid = Guid.Parse("071ef817-b359-44b6-9b8b-7576557bb504");

            // Restore PMD Dispatch
            public readonly static int Restore = 27;
            public readonly static Guid RestoreGuid = Guid.Parse("7c8568a2-c73e-4184-ba5e-07cd6d9e4386");

            // Update PMD Dispatch
            public readonly static int Update = 28;
            public readonly static Guid UpdateGuid = Guid.Parse("62d6fc13-518e-4a4a-91d7-576203e4bc83");

        }
        public static class StaffAbsence
        {
            // Access Staff Absence
            public readonly static int Access = 110;
            public readonly static Guid AccessGuid = Guid.Parse("116a1304-07d1-4af7-a4b1-f2a1182ea540");

            // Create Staff Absence
            public readonly static int Create = 111;
            public readonly static Guid CreateGuid = Guid.Parse("b44567cc-3f56-4ccf-9f61-98f9e57948d7");

            // Delete Staff Absence
            public readonly static int Delete = 112;
            public readonly static Guid DeleteGuid = Guid.Parse("f8f058a7-b988-4dc1-b8d3-da3a7d9b26b0");

            // Restore Staff Absence
            public readonly static int Restore = 113;
            public readonly static Guid RestoreGuid = Guid.Parse("dec2bd63-e799-4a1f-9c90-a8a25a9ba942");

            // Update Staff Absence
            public readonly static int Update = 114;
            public readonly static Guid UpdateGuid = Guid.Parse("3401d399-30fd-4b81-8989-9d5a3e37cd11");

            // Confirm Staff Absence
            public readonly static int Confirm = 115;
            public readonly static Guid ConfirmGuid = Guid.Parse("14d98160-11d9-4670-97bf-8a8f773c1f68");

            // FullAccess Staff Absence
            public readonly static int FullAccess = 116;
            public readonly static Guid FullAccessGuid = Guid.Parse("47c37cca-3817-46e6-b79a-bb89b229a20c");

        }
        public static class StaffAbsenceBalance
        {
            // Access Staff Absence Balance
            public readonly static int Access = 117;
            public readonly static Guid AccessGuid = Guid.Parse("636c6066-181c-4d7e-990d-9b66364a82a6");

            // Create Staff Absence Balance
            public readonly static int Create = 118;
            public readonly static Guid CreateGuid = Guid.Parse("4b72ab94-e55f-45a1-ae58-51bbff73ae64");

            // Delete Staff Absence Balance
            public readonly static int Delete = 119;
            public readonly static Guid DeleteGuid = Guid.Parse("4e639658-370b-4620-b98a-53bf0ad5fa9f");

            // Restore Staff Absence Balance
            public readonly static int Restore = 120;
            public readonly static Guid RestoreGuid = Guid.Parse("fe6b414a-80e5-4909-a9a3-b9dc32cb6732");

            // Update Staff Absence Balance
            public readonly static int Update = 121;
            public readonly static Guid UpdateGuid = Guid.Parse("8a6abe83-5bfa-4346-ada7-fdf14035e930");

        }

        #region RTS
        public static class SystemUserReports
        {
            // Access System User Reports
            public readonly static int Access = 1;
            public readonly static Guid AccessGuid = Guid.Parse("38f0ce24-342c-4307-a0f4-2e00255b66db");

        }
        public static class UnitReports
        {
            // Access Unit Reports
            public readonly static int Access = 2;
            public readonly static Guid AccessGuid = Guid.Parse("3aad92d8-f53c-4aed-be6f-d4a2d89ee59b");

        }
        public static class proGresQualityControl
        {
            // Access proGres Quality Control
            public readonly static int Access = 3;
            public readonly static Guid AccessGuid = Guid.Parse("15b1d973-34bb-45fc-9abd-4fb5c692406a");

        }
        #endregion
        #region RAMS
        public static class CaseManagement
        {
            // Access Case Management
            public readonly static int Access = 6;
            public readonly static Guid AccessGuid = Guid.Parse("b74437c7-a7bd-4d0d-999d-e266c63a34b2");

            // Create Case Management
            public readonly static int Create = 7;
            public readonly static Guid CreateGuid = Guid.Parse("d4d4a482-2983-46bb-8691-4670436d182b");

        }
        public static class CashReferrals
        {
            // Access Cash Referrals
            public readonly static int Access = 14;
            public readonly static Guid AccessGuid = Guid.Parse("b1ec722b-9e35-48bf-aa74-d6949ac47863");

            // Create Cash Referrals
            public readonly static int Create = 15;
            public readonly static Guid CreateGuid = Guid.Parse("b8ffc879-1f94-43a3-a802-66e2d2ef9b0e");

        }

        public static class CardsandVouchers
        {
            // Access Cards and Vouchers
            public readonly static int Access = 8;
            public readonly static Guid AccessGuid = Guid.Parse("4a8a566e-08b1-484e-885a-955432738d08");

            // Create Cards and Vouchers
            public readonly static int Create = 9;
            public readonly static Guid CreateGuid = Guid.Parse("aa456244-bc7f-499b-9fa5-e5766f1922fc");

        }
        public static class AssistanceManagement
        {
            // Access Assistance Management
            public readonly static int Access = 12;
            public readonly static Guid AccessGuid = Guid.Parse("141df7f6-5f40-42f2-9367-f4ef502cee28");

            // Create Assistance Management
            public readonly static int Create = 13;
            public readonly static Guid CreateGuid = Guid.Parse("178ed5d5-b673-4760-b6aa-3b999d986439");

        }
        public static class Counseling
        {
            // Access Counseling
            public readonly static int Access = 10;
            public readonly static Guid AccessGuid = Guid.Parse("a55796d9-6d2c-475c-9a5b-0f1bfce1ffa7");

            // Create Counseling
            public readonly static int Create = 11;
            public readonly static Guid CreateGuid = Guid.Parse("e6135f26-0570-4e89-8183-024e093d2b6b");

        }
        public static class CycleManagement
        {
            // Access Cycle Management
            public readonly static int Access = 3;
            public readonly static Guid AccessGuid = Guid.Parse("d62fdd63-3639-4045-abbb-c31bb95f038c");

            // Create Cycle Management
            public readonly static int Create = 4;
            public readonly static Guid CreateGuid = Guid.Parse("fd340dbb-0a84-483c-9572-eba0d2dd3391");

            // Confirm Cycle Management
            public readonly static int Confirm = 5;
            public readonly static Guid ConfirmGuid = Guid.Parse("634f21d1-2c93-4031-9502-a7b0f82619e6");

        }
        #endregion
        #region AST
        public static class StationaryConfiguration
        {
            // Access Stationary Configuration
            public readonly static int Access = 9;
            public readonly static Guid AccessGuid = Guid.Parse("03b9ce79-9fb7-4c5d-82ae-f0223ab8b761");

            // Create Stationary Configuration
            public readonly static int Create = 10;
            public readonly static Guid CreateGuid = Guid.Parse("4c5deb06-a378-47ba-8626-2547bf40d3e7");

        }
        public static class StationaryItemEntry
        {
            // Access Stationary Item Entry
            public readonly static int Access = 1;
            public readonly static Guid AccessGuid = Guid.Parse("4b1318df-f39d-4815-8233-5ca63c5beb46");

            // Create Stationary Item Entry
            public readonly static int Create = 2;
            public readonly static Guid CreateGuid = Guid.Parse("4cbcbf99-d95f-4c56-a976-da0edaba01a2");

            // Delete Stationary Item Entry
            public readonly static int Delete = 3;
            public readonly static Guid DeleteGuid = Guid.Parse("850480b9-44c0-4ba7-aa22-b062b2e91203");

            // Upload Stationary Item Entry
            public readonly static int Upload = 4;
            public readonly static Guid UploadGuid = Guid.Parse("5bc68736-4d95-48a6-8199-6e5607a9eb1f");

        }
        public static class StationaryItemRelease
        {
            // Access Stationary Item Release
            public readonly static int Access = 5;
            public readonly static Guid AccessGuid = Guid.Parse("ba745af4-a707-4b10-ac81-f3978221aeab");

            // Create Stationary Item Release
            public readonly static int Create = 6;
            public readonly static Guid CreateGuid = Guid.Parse("38bca149-03c0-415c-b8d1-d2cead17db1d");

            // Delete Stationary Item Release
            public readonly static int Delete = 7;
            public readonly static Guid DeleteGuid = Guid.Parse("a88bc8fa-561f-4fe7-9863-67566e276f76");

            // Update Stationary Item Release
            public readonly static int Update = 8;
            public readonly static Guid UpdateGuid = Guid.Parse("0f3006c8-472e-4569-9fb0-da2d681f6148");

        }
        public static class StationaryReports
        {
            // Access Stationary Reports
            public readonly static int Access = 11;
            public readonly static Guid AccessGuid = Guid.Parse("2da3096f-9341-4f9e-82c6-7f6205ce4c6e");

        }
        #endregion
        public static class DocumentCabinetShelf
        {
            // Create Document Cabinet Shelf
            public readonly static int Create = 15;
            public readonly static Guid CreateGuid = Guid.Parse("31bf4b1a-5505-4652-8c7c-3278c5c5a242");

            // Delete Document Cabinet Shelf
            public readonly static int Delete = 16;
            public readonly static Guid DeleteGuid = Guid.Parse("5959c665-3703-4dcf-99fd-1db896f70a8d");

            // Restore Document Cabinet Shelf
            public readonly static int Restore = 17;
            public readonly static Guid RestoreGuid = Guid.Parse("e0598e46-fd38-4a1f-9e16-4154c78580df");

            // Update Document Cabinet Shelf
            public readonly static int Update = 18;
            public readonly static Guid UpdateGuid = Guid.Parse("ba5a2578-8b18-4d8b-a0c8-dd805a13f37a");

            // Access Document Cabinet Shelf
            public readonly static int Access = 19;
            public readonly static Guid AccessGuid = Guid.Parse("14476aef-25a9-42b4-902f-ae4a2e941fc6");

        }
        public static class ShuttleRequestRoute
        {
            // Access Shuttle Request Route
            public readonly static int Access = 32;
            public readonly static Guid AccessGuid = Guid.Parse("2a59c886-7f73-4963-99b0-e384139122b9");

            // Create Shuttle Request Route
            public readonly static int Create = 33;
            public readonly static Guid CreateGuid = Guid.Parse("7e79761e-c745-4e65-93b9-cc5695bfb6d3");

            // Delete Shuttle Request Route
            public readonly static int Delete = 34;
            public readonly static Guid DeleteGuid = Guid.Parse("96308b88-4708-45be-885c-a451ead17764");

            // Restore Shuttle Request Route
            public readonly static int Restore = 35;
            public readonly static Guid RestoreGuid = Guid.Parse("3022eaf4-b769-4a4d-9c48-9e48e9a5ae4d");

            // Update Shuttle Request Route
            public readonly static int Update = 36;
            public readonly static Guid UpdateGuid = Guid.Parse("dc0030a0-6e9b-4004-8b4c-c66eb0652043");

        }
        public static class PurchasingReport
        {
            // Create Purchasing Report
            public readonly static int Create = 12;
            public readonly static Guid CreateGuid = Guid.Parse("5f633535-9c1a-44d9-955d-357c4b5142e1");

            // Delete Purchasing Report
            public readonly static int Delete = 13;
            public readonly static Guid DeleteGuid = Guid.Parse("309bdb25-bd45-4fdd-ac8a-e5380f58d2c3");

            // Restore Purchasing Report
            public readonly static int Restore = 14;
            public readonly static Guid RestoreGuid = Guid.Parse("7b51cbb9-a108-4f59-bc7b-7f255df47212");

            // Access Purchasing Report
            public readonly static int Access = 15;
            public readonly static Guid AccessGuid = Guid.Parse("c4873790-eb2a-44cf-94d4-ccd5c6d3ec1e");

        }
        public static class OfficeStaffAttendance
        {
            // Access Office Staff Attendance
            public readonly static int Access = 1;
            public readonly static Guid AccessGuid = Guid.Parse("5773fac0-8232-4e34-b2d7-cb8cae8334df");

            // Create Office Staff Attendance
            public readonly static int Create = 2;
            public readonly static Guid CreateGuid = Guid.Parse("3a9d74fe-da90-4dde-9a7e-b3f16694080d");

            // Delete Office Staff Attendance
            public readonly static int Delete = 3;
            public readonly static Guid DeleteGuid = Guid.Parse("a86289bc-2710-495f-8f85-05bd79e6dab9");

            // Restore Office Staff Attendance
            public readonly static int Restore = 4;
            public readonly static Guid RestoreGuid = Guid.Parse("b86610ac-78a5-43c4-9111-af789d250d41");

            // Update Office Staff Attendance
            public readonly static int Update = 5;
            public readonly static Guid UpdateGuid = Guid.Parse("912096f1-ce7c-47a5-8695-6cf6ddbd408c");

            // Confirm Office Staff Attendance
            public readonly static int Confirm = 6;
            public readonly static Guid ConfirmGuid = Guid.Parse("0ce5741b-ffb1-40e1-ada2-5a894bbfbe98");

            // Send Office Staff Attendance
            public readonly static int Send = 7;
            public readonly static Guid SendGuid = Guid.Parse("01f34fd3-1169-4fea-88a3-fc1976457bd9");

            // Print Office Staff Attendance
            public readonly static int Print = 8;
            public readonly static Guid PrintGuid = Guid.Parse("48033c7c-67e6-4074-93f0-2cd0e3815479");

            // ValidateData Office Staff Attendance
            public readonly static int ValidateData = 9;
            public readonly static Guid ValidateDataGuid = Guid.Parse("d64b5b6c-1c19-4358-8657-ff49fc44c044");

            // Import Office Staff Attendance
            public readonly static int Import = 10;
            public readonly static Guid ImportGuid = Guid.Parse("ad4aeefb-2049-4eef-b314-d1392e8b3849");

            // Remove Office Staff Attendance
            public readonly static int Remove = 11;
            public readonly static Guid RemoveGuid = Guid.Parse("ebbd8a54-811a-4ba1-bbf4-61d4d03bee08");
        }
        public static class AccessOnlineVotingSystem
        {
            // Access Test for Voting System
            public readonly static int Access = 1;
            public readonly static Guid AccessGuid = Guid.Parse("85517c3b-7e1e-48cc-98eb-e7d407e64345");

        }
        public static class AccountSettings
        {
            // Update Account Passwords
            public readonly static Guid UpdateGuid = Guid.Parse("cbdb3aaf-3fdb-4e70-a5c3-b7ae333c2dde");

            // Upload Interval Type Languages
            public readonly static int Upload = 109;
            public readonly static Guid UploadGuid = Guid.Parse("d132fe63-8ff6-46c4-9edc-57288fdf37c6");

            // Access Interval Type Languages
            public readonly static int Access = 110;
            public readonly static Guid AccessGuid = Guid.Parse("2aff10c1-6940-4fb2-abaa-59b269cfd6a9");

        }
        public static class ActionCategories
        {
            // Access Action Categories Codes
            public readonly static int Access = 6;
            public readonly static Guid AccessGuid = Guid.Parse("3f1552be-5fde-48f1-bc40-e5f864ef6da1");

            // Create Action Categories Codes
            public readonly static int Create = 7;
            public readonly static Guid CreateGuid = Guid.Parse("af7e7275-6abe-47cc-90a0-19bf345eee02");

            // Update Action Categories Codes
            public readonly static int Update = 8;
            public readonly static Guid UpdateGuid = Guid.Parse("2b0be514-b6a8-4f66-9785-e3bb180dd8c7");

            // Delete Action Categories Codes
            public readonly static int Delete = 9;
            public readonly static Guid DeleteGuid = Guid.Parse("21bc5387-8fa7-4be2-8edf-55bfb260ead6");

            // Restore Action Categories Codes
            public readonly static int Restore = 10;
            public readonly static Guid RestoreGuid = Guid.Parse("f07ccbc6-90af-4f4e-91e9-7a38df23acbd");

        }
        public static class ActionCategoriesLanguages
        {
            // Create Action Categories Codes Language
            public readonly static Guid CreateGuid = Guid.Parse("8223418c-de1c-4671-aec4-27123bcb9a99");

            // Restore Action Categories Codes Language
            public readonly static Guid RestoreGuid = Guid.Parse("1d86882b-344a-4334-886c-0ea33e2ce710");

            // Update Action Categories Codes Language
            public readonly static Guid UpdateGuid = Guid.Parse("35216730-4d2f-417e-93bc-b9e53cf80f97");

            // Delete Action Categories Codes Language
            public readonly static Guid DeleteGuid = Guid.Parse("1ea2ecc1-1679-4ffe-a18b-bb2d92cca09c");

        }
        public static class Actions
        {
            // Access Actions Codes
            public readonly static int Access = 11;
            public readonly static Guid AccessGuid = Guid.Parse("fe2c8ff8-de77-45b2-b10d-cb97e184aefb");

            // Create Actions Codes
            public readonly static int Create = 12;
            public readonly static Guid CreateGuid = Guid.Parse("6a8ed675-75d5-4141-942e-1fd7dab6de4e");

            // Update Actions Codes
            public readonly static int Update = 13;
            public readonly static Guid UpdateGuid = Guid.Parse("76ba5d81-07c2-4af4-920a-931133afd24e");

            // Delete Actions Codes
            public readonly static int Delete = 14;
            public readonly static Guid DeleteGuid = Guid.Parse("bf09aed9-cbad-4622-ac27-a27568694c27");

            // Restore Actions Codes
            public readonly static int Restore = 15;
            public readonly static Guid RestoreGuid = Guid.Parse("e0f6d290-d4f2-47ec-b0df-1f1d412557e9");

        }
        public static class ActionsEntities
        {
            // Access Actions Entities
            public readonly static int Access = 97;
            public readonly static Guid AccessGuid = Guid.Parse("f418fc59-2d49-4306-a958-f6a4432aca95");

            // Create Actions Entities
            public readonly static int Create = 98;
            public readonly static Guid CreateGuid = Guid.Parse("05dc597a-f21c-406e-953e-4632033ab30a");

            // Delete Actions Entities
            public readonly static int Delete = 99;
            public readonly static Guid DeleteGuid = Guid.Parse("fe8fbe50-7156-41e3-9234-b18aa642eee5");

            // Restore Actions Entities
            public readonly static int Restore = 100;
            public readonly static Guid RestoreGuid = Guid.Parse("b2b970c9-88aa-47ae-a06a-ebf9d9223768");

            // Update Actions Entities
            public readonly static int Update = 101;
            public readonly static Guid UpdateGuid = Guid.Parse("e1cb0424-bc93-42b8-a102-6c05aa0447ba");

        }
        public static class ActionsLanguages
        {
            // Create Actions Codes Language
            public readonly static Guid CreateGuid = Guid.Parse("4da5e3c4-b093-4bb1-8855-c718e48d9d66");

            // Delete Actions Codes Language
            public readonly static Guid DeleteGuid = Guid.Parse("3c19452e-038e-4714-84bd-6bcc834ed73c");

            // Restore Actions Codes Language
            public readonly static Guid RestoreGuid = Guid.Parse("cba40ecb-d509-476c-b7ab-4924ed4f74d5");

            // Update Actions Codes Language
            public readonly static Guid UpdateGuid = Guid.Parse("53d37cb3-5ed5-44d7-b56d-3b80a374fcb8");

        }
        public static class ActionsVerbs
        {
            // Access Actions Verbs
            public readonly static int Access = 92;
            public readonly static Guid AccessGuid = Guid.Parse("b57a3c48-b01f-46bb-a331-1dd5a8ba1da1");

            // Create Actions Verbs
            public readonly static int Create = 93;
            public readonly static Guid CreateGuid = Guid.Parse("e9e8d588-f1ee-4c10-aa75-c0b654d73aa7");

            // Delete Actions Verbs
            public readonly static int Delete = 94;
            public readonly static Guid DeleteGuid = Guid.Parse("57470532-e2f1-4073-a168-2ee1cd817275");

            // Restore Actions Verbs
            public readonly static int Restore = 95;
            public readonly static Guid RestoreGuid = Guid.Parse("0a0ddfb7-2426-489f-ab35-75cf5cb6fcdc");

            // Update Actions Verbs
            public readonly static int Update = 96;
            public readonly static Guid UpdateGuid = Guid.Parse("12124a0c-fe7c-4e0a-8097-9cc84e7757da");

        }
        public static class Applications
        {
            // Access Applications Codes
            public readonly static int Access = 1;
            public readonly static Guid AccessGuid = Guid.Parse("ccd61041-c8b9-424f-9a20-39a7d27be59c");

            // Create Applications Codes
            public readonly static int Create = 2;
            public readonly static Guid CreateGuid = Guid.Parse("6c58fb67-ac4d-463e-aa22-343003615277");

            // Update Applications Codes
            public readonly static int Update = 3;
            public readonly static Guid UpdateGuid = Guid.Parse("e5efe6a8-909a-4416-8665-0e159ccdb5bb");

            // Delete Applications Codes
            public readonly static int Delete = 4;
            public readonly static Guid DeleteGuid = Guid.Parse("25a7151d-63d3-47da-b3ff-691f2da51520");

            // Restore Applications Codes
            public readonly static int Restore = 5;
            public readonly static Guid RestoreGuid = Guid.Parse("82422b9c-475f-462d-b40f-05b9447e1451");

        }
        public static class ApplicationsLanguages
        {
            // Restore Applications Codes Language
            public readonly static Guid RestoreGuid = Guid.Parse("1eaf6ab6-aba8-4f7e-90d9-462d355399b3");

            // Create Applications Codes Language
            public readonly static Guid CreateGuid = Guid.Parse("af8aa754-11e0-4dd8-aa75-0e53c8efdfea");

            // Update Applications Codes Language
            public readonly static Guid UpdateGuid = Guid.Parse("0c6525ca-6293-4b05-b780-01f7c6f8097f");

            // Delete Applications Codes Language
            public readonly static Guid DeleteGuid = Guid.Parse("475fa508-8520-4768-b561-79a7c3ae2b7a");

        }
        public static class Appointment
        {
            // Access Appointment
            public readonly static int Access = 11;
            public readonly static Guid AccessGuid = Guid.Parse("fa708880-8e8f-4d5c-9d46-4ee89e6cf56e");

            // Create Appointment
            public readonly static int Create = 12;
            public readonly static Guid CreateGuid = Guid.Parse("18c380be-8d94-42f4-ae0a-9c037651953a");

            // Delete Appointment
            public readonly static int Delete = 13;
            public readonly static Guid DeleteGuid = Guid.Parse("5e71ee87-dfb7-438b-b154-15b52ae71237");

            // Restore Appointment
            public readonly static int Restore = 14;
            public readonly static Guid RestoreGuid = Guid.Parse("ef14bd41-31d4-4572-9f00-27f0d869aa46");

            // Update Appointment
            public readonly static int Update = 15;
            public readonly static Guid UpdateGuid = Guid.Parse("5b9caa0c-e119-408f-bb84-c63dff8a4fd8");

        }
        public static class AppointmentType
        {
            // Access Appointment Type
            public readonly static int Access = 1;
            public readonly static Guid AccessGuid = Guid.Parse("717da5e8-312f-4bc6-bd0f-e8c32ffe0824");

            // Create Appointment Type
            public readonly static int Create = 2;
            public readonly static Guid CreateGuid = Guid.Parse("dac7664a-dbea-4641-84a2-c5abe16daeb7");

            // Delete Appointment Type
            public readonly static int Delete = 3;
            public readonly static Guid DeleteGuid = Guid.Parse("c48ba838-f6d3-452c-b99b-fb866734d8ba");

            // Restore Appointment Type
            public readonly static int Restore = 4;
            public readonly static Guid RestoreGuid = Guid.Parse("0ac3da64-04dc-41a6-b034-a29a3d372b4b");

            // Update Appointment Type
            public readonly static int Update = 5;
            public readonly static Guid UpdateGuid = Guid.Parse("dbb2a931-7578-475a-b8f2-b6fd6a121c56");

        }
        public static class AppointmentTypeCalendar
        {
            // Access Appointment Type Calendar
            public readonly static int Access = 21;
            public readonly static Guid AccessGuid = Guid.Parse("3144cb34-560a-4b26-9a9b-456ecd2df2e6");

            // Create Appointment Type Calendar
            public readonly static int Create = 22;
            public readonly static Guid CreateGuid = Guid.Parse("9bfd72e2-26c4-487d-888b-be6b4c4e40ba");

            // Delete Appointment Type Calendar
            public readonly static int Delete = 23;
            public readonly static Guid DeleteGuid = Guid.Parse("8b48c968-6ccd-485e-9b82-658690ba8545");

            // Restore Appointment Type Calendar
            public readonly static int Restore = 24;
            public readonly static Guid RestoreGuid = Guid.Parse("f1aea30f-a845-4195-b97e-c19ea8a1d21f");

            // Update Appointment Type Calendar
            public readonly static int Update = 25;
            public readonly static Guid UpdateGuid = Guid.Parse("2514a1cd-d25e-45af-a4b1-ee1a87eb6156");

        }
        public static class AttendanceRecords
        {
            // Access Attendance Records
            public readonly static int Access = 21;
            public readonly static Guid AccessGuid = Guid.Parse("6f5200a7-34dd-4fee-a657-965049da374c");

            // Create Attendance Records
            public readonly static int Create = 22;
            public readonly static Guid CreateGuid = Guid.Parse("4774fa7f-be56-46d5-8014-f8cc9442f4a7");

            // Update Attendance Records
            public readonly static int Update = 23;
            public readonly static Guid UpdateGuid = Guid.Parse("5c6a5897-81cd-47b3-9279-6a4c307c33d6");

            // Delete Attendance Records
            public readonly static int Delete = 24;
            public readonly static Guid DeleteGuid = Guid.Parse("a998b2e3-34a2-4391-8bf1-2bbb3e7b63da");

            // Restore Attendance Records
            public readonly static int Restore = 25;
            public readonly static Guid RestoreGuid = Guid.Parse("7927a918-41b8-490a-ac97-34ad8f2583f7");

        }
        public static class AttendanceShiftGroups
        {
            // Access Attendance Shift Groups
            public readonly static int Access = 6;
            public readonly static Guid AccessGuid = Guid.Parse("18a210a0-e316-4280-b50b-1c2fd64dfa03");

            // Create Attendance Shift Groups
            public readonly static int Create = 7;
            public readonly static Guid CreateGuid = Guid.Parse("528e0a52-6a16-44cf-89d6-1c30a1d28bce");

            // Update Attendance Shift Groups
            public readonly static int Update = 8;
            public readonly static Guid UpdateGuid = Guid.Parse("6e698d0e-bba1-4d4e-8470-efcc66f5b8c7");

            // Delete Attendance Shift Groups
            public readonly static int Delete = 9;
            public readonly static Guid DeleteGuid = Guid.Parse("d3a24c12-e5df-4cd4-84b3-c96ef786b5d7");

            // Restore Attendance Shift Groups
            public readonly static int Restore = 10;
            public readonly static Guid RestoreGuid = Guid.Parse("9d07419a-d714-4af0-9c6e-18ae8eb16d46");

        }
        public static class AuditManagement
        {
            // Access Login Audit
            public readonly static int Access = 102;
            public readonly static Guid AccessGuid = Guid.Parse("0624e1d9-e509-4ebb-abb4-1305748cc816");

        }
        public static class BillsManagement
        {
            // Access Bills Management
            public readonly static int Access = 11;
            public readonly static Guid AccessGuid = Guid.Parse("b62e7551-eecc-4868-abcf-7ae3c2f91f49");

            // Create Bills Management
            public readonly static int Create = 12;
            public readonly static Guid CreateGuid = Guid.Parse("a6d193f2-04d2-4b15-b88a-3131861d03a5");

            // Update Bills Management
            public readonly static int Update = 13;
            public readonly static Guid UpdateGuid = Guid.Parse("89ea7b15-c2be-4ae6-b0eb-71741aa4cbc8");

            // Delete Bills Management
            public readonly static int Delete = 14;
            public readonly static Guid DeleteGuid = Guid.Parse("1dc6ec7f-833a-4ef2-bd5e-7780fc200546");

            // Restore Bills Management
            public readonly static int Restore = 15;
            public readonly static Guid RestoreGuid = Guid.Parse("b7c4f1a9-d85a-428c-902f-91457969b323");

        }
        public static class Bugreportmanagement
        {
            // Update Bug report management
            public readonly static int Update = 10;
            public readonly static Guid UpdateGuid = Guid.Parse("479faf0c-b863-40b4-9b57-3cfcd1615926");

            // Delete Bug report management
            public readonly static int Delete = 11;
            public readonly static Guid DeleteGuid = Guid.Parse("b7ea2946-c062-40a8-9620-be1bfd17135c");

            // Restore Bug report management
            public readonly static int Restore = 12;
            public readonly static Guid RestoreGuid = Guid.Parse("730d9034-2c82-48f3-aa98-5376119a2a1e");

        }
        public static class CardIndividualInfo
        {
            // Access Card Individual Info
            public readonly static int Access = 1;
            public readonly static Guid AccessGuid = Guid.Parse("9170c1e4-d41c-462f-b80b-833cdd30a73d");

            // Create Card Individual Info
            public readonly static int Create = 2;
            public readonly static Guid CreateGuid = Guid.Parse("1c1ad875-66a1-45a7-a01a-219e51877075");

            // Delete Card Individual Info
            public readonly static int Delete = 3;
            public readonly static Guid DeleteGuid = Guid.Parse("15f49010-c1f0-402a-a97f-9182f062e739");

            // Restore Card Individual Info
            public readonly static int Restore = 4;
            public readonly static Guid RestoreGuid = Guid.Parse("932d49bd-5932-4223-9e3b-d44d8db29585");

            // Update Card Individual Info
            public readonly static int Update = 5;
            public readonly static Guid UpdateGuid = Guid.Parse("03fc2d41-f354-479c-aa90-7aa86af9fb48");

        }
        public static class CardIssued
        {
            // Access Card Issued
            public readonly static int Access = 6;
            public readonly static Guid AccessGuid = Guid.Parse("a8b51f1a-0c83-4b0a-98e6-8620cae9e322");

            // Create Card Issued
            public readonly static int Create = 7;
            public readonly static Guid CreateGuid = Guid.Parse("699e03ad-6539-4731-85a1-da25363e84b5");

            // Delete Card Issued
            public readonly static int Delete = 8;
            public readonly static Guid DeleteGuid = Guid.Parse("f2638417-4fc7-4ad4-ac32-dfc447272630");

            // Restore Card Issued
            public readonly static int Restore = 9;
            public readonly static Guid RestoreGuid = Guid.Parse("647289a7-186d-4942-964e-645328966ba9");

            // Update Card Issued
            public readonly static int Update = 10;
            public readonly static Guid UpdateGuid = Guid.Parse("0144e0fc-4e4b-4ea5-a873-0934df934e71");

        }
        public static class CardPrint
        {
            // Access Card Print
            public readonly static int Access = 14;
            public readonly static Guid AccessGuid = Guid.Parse("760e67ad-5551-42c0-bd5f-abd6325a8662");

            // Print Card Print
            public readonly static int Print = 15;
            public readonly static Guid PrintGuid = Guid.Parse("f94e5b52-534d-4c30-afe7-231859cf8d74");

        }
        public static class CardValidation
        {
            // Access Card Validation
            public readonly static int Access = 11;
            public readonly static Guid AccessGuid = Guid.Parse("e4efa6f8-2482-43bc-aba3-50edf633ce6d");

            // Confirm Card Validation
            public readonly static int Confirm = 13;
            public readonly static Guid ConfirmGuid = Guid.Parse("0fba2487-1c46-451e-96c1-f22638e00372");

        }
        public static class Case
        {
            // Access Case
            public readonly static int Access = 6;
            public readonly static Guid AccessGuid = Guid.Parse("02a6a0f9-a0aa-4e28-87a5-f9467be96ea4");

            // Create Case
            public readonly static int Create = 7;
            public readonly static Guid CreateGuid = Guid.Parse("22372519-cba7-4b2b-8cae-f23f65260e65");

            // Delete Case
            public readonly static int Delete = 8;
            public readonly static Guid DeleteGuid = Guid.Parse("79a3c674-d1e2-452b-8f45-679cb4e7357a");

            // Restore Case
            public readonly static int Restore = 9;
            public readonly static Guid RestoreGuid = Guid.Parse("2854056e-e21e-4455-af97-f5ddf0a724c3");

            // Update Case
            public readonly static int Update = 10;
            public readonly static Guid UpdateGuid = Guid.Parse("18378f9f-4e64-4978-804f-81fcaa9e55c9");

        }
        public static class CDRFTPLocationsManagement
        {
            // Access CDR FTP Locations
            public readonly static int Access = 16;
            public readonly static Guid AccessGuid = Guid.Parse("17ae12d1-1001-4fef-b428-0ba57e4c3dbc");

            // Create CDR FTP Locations
            public readonly static int Create = 17;
            public readonly static Guid CreateGuid = Guid.Parse("4e14ab46-4e4c-474c-b273-bfeb824506d2");

            // Update CDR FTP Locations
            public readonly static int Update = 18;
            public readonly static Guid UpdateGuid = Guid.Parse("58fb86d0-57df-4602-9944-6ccdd40f7dfa");

            // Delete CDR FTP Locations
            public readonly static int Delete = 19;
            public readonly static Guid DeleteGuid = Guid.Parse("ef0267f4-d9bf-41f2-8c94-975b0722b165");

            // Restore CDR FTP Locations
            public readonly static int Restore = 20;
            public readonly static Guid RestoreGuid = Guid.Parse("31ec38f6-06a9-4d01-9d7e-aae113ce0030");

        }
        public static class CodeTables
        {
            // Access Code Table
            public readonly static int Access = 16;
            public readonly static Guid AccessGuid = Guid.Parse("551d77a8-0c02-4189-b312-705b4fe8ccbe");

            // Create Code Table
            public readonly static int Create = 17;
            public readonly static Guid CreateGuid = Guid.Parse("af5c2a33-e9b4-45d5-9b77-54e220a89203");

            // Update Code Table
            public readonly static int Update = 18;
            public readonly static Guid UpdateGuid = Guid.Parse("b05816b3-6431-464c-b8a7-59834b31656b");

            // Delete Code Table
            public readonly static int Delete = 19;
            public readonly static Guid DeleteGuid = Guid.Parse("0bdc168c-d9ad-498d-9315-36987da380a0");

            // Restore Code Table
            public readonly static int Restore = 20;
            public readonly static Guid RestoreGuid = Guid.Parse("645a4dbc-a674-457d-b4c7-19aeae9d2772");

        }
        public static class CommunityCenterCode
        {
            // Access Partner Center
            public readonly static int Access = 1;
            public readonly static Guid AccessGuid = Guid.Parse("e92d6ab6-141e-4364-a7ef-3dc8da113dfa");

            // Create Partner Center
            public readonly static int Create = 2;
            public readonly static Guid CreateGuid = Guid.Parse("5f5fc744-532f-4d60-aef6-e7aa7e64bcc5");

            // Delete Partner Center
            public readonly static int Delete = 3;
            public readonly static Guid DeleteGuid = Guid.Parse("5e8a9076-ae3b-4c2e-a169-3cb967930f55");

            // Restore Partner Center
            public readonly static int Restore = 4;
            public readonly static Guid RestoreGuid = Guid.Parse("39b62fcc-33cc-43a5-a6d2-30db39c085e7");

            // Update Partner Center
            public readonly static int Update = 5;
            public readonly static Guid UpdateGuid = Guid.Parse("67ce94d7-5009-473c-a4b8-534df0b0ea0a");

        }
        public static class ConditionTypeLanguages
        {
            // Delete Condition Type Languages
            public readonly static int Delete = 12;
            public readonly static Guid DeleteGuid = Guid.Parse("61d59e64-d289-43fc-8341-dcbd747e2239");

            // Create Condition Type Languages
            public readonly static int Create = 13;
            public readonly static Guid CreateGuid = Guid.Parse("1db1ce09-a814-4eb8-9a89-1084781d3d63");

            // Update Condition Type Languages
            public readonly static int Update = 14;
            public readonly static Guid UpdateGuid = Guid.Parse("d3e51ce2-3f2f-4599-ba83-870788c61c06");

            // Restore Condition Type Languages
            public readonly static int Restore = 15;
            public readonly static Guid RestoreGuid = Guid.Parse("4ca0aac3-fa68-44d9-a401-6657562d0cd8");

        }
        public static class ConditionTypes
        {
            // Access Condition Types
            public readonly static int Access = 7;
            public readonly static Guid AccessGuid = Guid.Parse("05679a89-981b-45f5-8a0d-3196f07ab600");

            // Create Condition Types
            public readonly static int Create = 8;
            public readonly static Guid CreateGuid = Guid.Parse("3427940a-636b-44de-ab96-fbc2f5bad609");

            // Delete Condition Types
            public readonly static int Delete = 9;
            public readonly static Guid DeleteGuid = Guid.Parse("54163a62-1db2-4fd3-ba22-344d487673f8");

            // Restore Condition Types
            public readonly static int Restore = 10;
            public readonly static Guid RestoreGuid = Guid.Parse("3d0151e5-a8fd-48dc-8f35-5d828588eb20");

            // Update Condition Types
            public readonly static int Update = 11;
            public readonly static Guid UpdateGuid = Guid.Parse("9c5e6fe0-6b2d-45cf-95a1-c5f3ec80c85c");

        }
        public static class Connection
        {
            // Create Connection
            public readonly static int Create = 106;
            public readonly static Guid CreateGuid = Guid.Parse("3e585a09-178b-4727-9219-34673966ffab");

            // Access Connection
            public readonly static int Access = 112;
            public readonly static Guid AccessGuid = Guid.Parse("069bd94b-9a9a-421a-8692-8b4cafff2fc6");

            // Update Connection
            public readonly static int Update = 115;
            public readonly static Guid UpdateGuid = Guid.Parse("61ef0aef-e509-4fa6-b2b4-bf2b1768ecc8");

            // Delete Connection
            public readonly static int Delete = 116;
            public readonly static Guid DeleteGuid = Guid.Parse("7ea7a733-e7f9-408d-bc99-c9e1a4673189");

            // Restore Connection
            public readonly static int Restore = 117;
            public readonly static Guid RestoreGuid = Guid.Parse("98716c59-886f-43db-89bf-f2f88bc6b6fc");

        }
        public static class ContactDetails
        {
            // Update Contacts Details
            public readonly static Guid UpdateGuid = Guid.Parse("8d97f664-155e-4d19-8705-15b2732ac41d");

            // Create Contacts Details
            public readonly static Guid CreateGuid = Guid.Parse("75a33a55-98f7-47c0-8b4d-39e19735cae2");

        }
        public static class ContactInfo
        {
            // Access Contact
            public readonly static int Access = 16;
            public readonly static Guid AccessGuid = Guid.Parse("375ead36-61f1-45cf-97c3-c22d5c53cf39");

            // Create Contact
            public readonly static int Create = 17;
            public readonly static Guid CreateGuid = Guid.Parse("1d35d624-18aa-4268-bbf1-a49569c2c9f9");

            // Delete Contact
            public readonly static int Delete = 18;
            public readonly static Guid DeleteGuid = Guid.Parse("95cb0d92-ec0e-4950-8782-cfbb0f2f653e");

            // Restore Contact
            public readonly static int Restore = 19;
            public readonly static Guid RestoreGuid = Guid.Parse("84450d57-536a-4ddd-a0d3-3800d1ab4a9d");

            // Update Contact
            public readonly static int Update = 20;
            public readonly static Guid UpdateGuid = Guid.Parse("faabef66-d212-470a-a5b5-a7ffa77e4d10");

        }
        public static class Countries
        {
            // Access Countries Codes
            public readonly static int Access = 46;
            public readonly static Guid AccessGuid = Guid.Parse("1b3edb76-7caf-4df5-af33-176d5a0630de");

            // Create Countries Codes
            public readonly static int Create = 47;
            public readonly static Guid CreateGuid = Guid.Parse("fe108646-b6b7-48a9-a109-556ea0e8cedd");

            // Update Countries Codes
            public readonly static int Update = 48;
            public readonly static Guid UpdateGuid = Guid.Parse("80ce7761-09c4-42df-a00f-21c6fb3035c8");

            // Delete Countries Codes
            public readonly static int Delete = 49;
            public readonly static Guid DeleteGuid = Guid.Parse("f36dbb05-2239-4948-b7b6-fac51a26c345");

            // Restore Countries Codes
            public readonly static int Restore = 50;
            public readonly static Guid RestoreGuid = Guid.Parse("90fc39df-8e1c-4ec8-bc62-a360fcee9217");

        }
        public static class CountriesConfigurations
        {
            // Remove Countries Configurations
            public readonly static Guid RemoveGuid = Guid.Parse("2449497a-3e90-42e4-88a2-499d2ded4baf");

            // Create Countries Configurations
            public readonly static Guid CreateGuid = Guid.Parse("73626508-3046-4c0f-9cca-cefd4f9d3d23");

        }
        public static class CountriesLanguages
        {
            // Restore Countries Codes Languages
            public readonly static Guid RestoreGuid = Guid.Parse("00eed7e7-1b44-4664-bc6e-d21736a4850c");

            // Delete Countries Codes Languages
            public readonly static Guid DeleteGuid = Guid.Parse("e0ee3aa1-895c-4e4a-b247-47d275fca4ca");

            // Update Countries Codes Languages
            public readonly static Guid UpdateGuid = Guid.Parse("cfa8cdea-b08c-4cdb-9e3d-2bd68e9439fb");

            // Create Countries Codes Languages
            public readonly static Guid CreateGuid = Guid.Parse("63892dbf-640e-4201-8c12-067ce49fc908");

        }
        public static class CovidRecordFeedback
        {
            // Create Covid Record Feedback
            public readonly static int Create = 7;
            public readonly static Guid CreateGuid = Guid.Parse("0122b3de-6d6f-4239-a4bb-2e46f02e4ba9");

        }
        public static class COVIDRecordVerify
        {
            // Create Covid Record Verify
            public readonly static int Create = 6;
            public readonly static Guid CreateGuid = Guid.Parse("fc85d69b-e086-47b0-8cbe-14a0f7cda059");

        }
        public static class COVIDUNHCRResponse
        {
            // Access COVID UNHCR Response
            public readonly static int Access = 1;
            public readonly static Guid AccessGuid = Guid.Parse("fecf2ea3-37d5-41f9-86c5-507ddf930c33");

            // Create COVID UNHCR Response
            public readonly static int Create = 2;
            public readonly static Guid CreateGuid = Guid.Parse("c644cf12-637e-4426-8fa7-6f6889bf82df");

            // Update COVID UNHCR Response
            public readonly static int Update = 3;
            public readonly static Guid UpdateGuid = Guid.Parse("8fcdbdf6-2dd7-42bb-bf37-c816eeefd98c");

            // Delete COVID UNHCR Response
            public readonly static int Delete = 4;
            public readonly static Guid DeleteGuid = Guid.Parse("db10f106-bc25-41ad-903d-aa6d2af75a15");

            // Restore COVID UNHCR Response
            public readonly static int Restore = 5;
            public readonly static Guid RestoreGuid = Guid.Parse("ac7423c7-8156-443a-8e7c-fb55c4fe65cc");

        }
        public static class DASConfiguration
        {
            // Access DAS Configuration
            public readonly static int Access = 6;
            public readonly static Guid AccessGuid = Guid.Parse("9f736ff7-08ee-478b-bb86-e3159712e5ef");

            // Create DAS Configuration
            public readonly static int Create = 7;
            public readonly static Guid CreateGuid = Guid.Parse("ac979d0e-bf4e-4e37-ba9d-68f4279dcd73");

            // Delete DAS Configuration
            public readonly static int Delete = 8;
            public readonly static Guid DeleteGuid = Guid.Parse("daec9602-cc13-4b02-b76e-2e3695bcaeb4");

            // Restore DAS Configuration
            public readonly static int Restore = 9;
            public readonly static Guid RestoreGuid = Guid.Parse("9c9df4b5-cb06-4046-969f-08032bfab4a4");

            // Update DAS Configuration
            public readonly static int Update = 11;
            public readonly static Guid UpdateGuid = Guid.Parse("b9f45ab6-fd7e-4bb2-b5f5-07985d033540");

        }
        public static class DASDashboard
        {
            // Access DAS Dashboard
            public readonly static int Access = 10;
            public readonly static Guid AccessGuid = Guid.Parse("594b2b06-56df-472b-9f90-317bf699a396");

        }
        public static class Departments
        {
            // Access Departments Codes
            public readonly static int Access = 21;
            public readonly static Guid AccessGuid = Guid.Parse("f859f1fc-9b14-4586-8368-b8ce939e2ab7");

            // Create Departments Codes
            public readonly static int Create = 22;
            public readonly static Guid CreateGuid = Guid.Parse("a67ae503-3f18-4556-9118-e78d4b72cc45");

            // Update Departments Codes
            public readonly static int Update = 23;
            public readonly static Guid UpdateGuid = Guid.Parse("b6c880a1-cb2e-4a04-af8a-d7a1933a167c");

            // Delete Departments Codes
            public readonly static int Delete = 24;
            public readonly static Guid DeleteGuid = Guid.Parse("965e5fa4-1ff8-4f70-92d8-07d1c1a35cc6");

            // Restore Departments Codes
            public readonly static int Restore = 25;
            public readonly static Guid RestoreGuid = Guid.Parse("76fa9383-5b34-4954-85ff-e37d4baddd45");

        }
        public static class DepartmentsConfigurations
        {
            // Remove Departments Configurations
            public readonly static Guid RemoveGuid = Guid.Parse("c5542b91-19c6-474b-86eb-561581f45d16");

            // Create Departments Configurations
            public readonly static Guid CreateGuid = Guid.Parse("74b4cfdc-ac1a-457c-8536-53e1942c0fbb");

        }
        public static class DepartmentsLanguages
        {
            // Delete Departments Codes Languages
            public readonly static Guid DeleteGuid = Guid.Parse("3fbc329e-b7dd-4e1c-81b5-5923a237eb15");

            // Create Departments Codes Languages
            public readonly static Guid CreateGuid = Guid.Parse("d0ff0208-a4f2-47a1-8eb2-67bea354fa7b");

            // Update Departments Codes Languages
            public readonly static Guid UpdateGuid = Guid.Parse("a1fbf04c-a49f-459c-82c6-f0f5c72a13c6");

            // Restore Departments Codes Languages
            public readonly static Guid RestoreGuid = Guid.Parse("a3510dee-da55-47e0-a7e0-ed25b0dc8aac");

        }
        public static class DutyStations
        {
            // Access Duty Stations Codes
            public readonly static int Access = 26;
            public readonly static Guid AccessGuid = Guid.Parse("c4092b23-2e7c-4f85-aecc-c3c088b84b72");

            // Create Duty Stations Codes
            public readonly static int Create = 27;
            public readonly static Guid CreateGuid = Guid.Parse("7e791947-3bf1-4120-8a33-dbcaa3f67b29");

            // Update Duty Stations Codes
            public readonly static int Update = 28;
            public readonly static Guid UpdateGuid = Guid.Parse("28882f4d-2519-43ad-8d82-0cc4ff03db06");

            // Delete Duty Stations Codes
            public readonly static int Delete = 29;
            public readonly static Guid DeleteGuid = Guid.Parse("2fde98e4-4bb3-4a40-9343-3bb6e8ff0752");

            // Restore Duty Stations Codes
            public readonly static int Restore = 30;
            public readonly static Guid RestoreGuid = Guid.Parse("6b3db315-79d6-413c-a9e5-93b5b71e3fd1");

        }
        public static class DutyStationsConfigurations
        {
            // Remove Duty Stations Configurations
            public readonly static Guid RemoveGuid = Guid.Parse("5083f7ba-2e5c-4f3c-bec6-822e1a8a2251");

            // Create Duty Stations Configurations
            public readonly static Guid CreateGuid = Guid.Parse("7aad5995-6285-4139-b8d7-f382ab07ef63");

        }
        public static class DutyStationsLanguages
        {
            // Delete Duty Stations Codes Languages
            public readonly static Guid DeleteGuid = Guid.Parse("3d0abef8-be88-4b65-8f48-f6797c28f4cd");

            // Update Duty Stations Codes Languages
            public readonly static Guid UpdateGuid = Guid.Parse("f18174b1-36bb-4064-a26f-e54434515e0d");

            // Create Duty Stations Codes Languages
            public readonly static Guid CreateGuid = Guid.Parse("034a5cbe-d2dd-4860-b887-c117525152df");

            // Restore Duty Stations Codes Languages
            public readonly static Guid RestoreGuid = Guid.Parse("2c714263-81b3-42b0-af18-1bf4d30beb3c");

        }
        public static class ElectionLanguages
        {
            // Create Election Languages
            public readonly static int Create = 26;
            public readonly static Guid CreateGuid = Guid.Parse("61c1da13-2118-4b7d-a49e-3d7661f4aa93");

            // Delete Election Languages
            public readonly static int Delete = 27;
            public readonly static Guid DeleteGuid = Guid.Parse("2361cf74-57a8-4f9f-ae88-a0b6008f1695");

            // Update Election Languages
            public readonly static int Update = 28;
            public readonly static Guid UpdateGuid = Guid.Parse("c1d1e86a-7e13-401d-9c22-4c5313185a79");

            // Restore Election Languages
            public readonly static int Restore = 29;
            public readonly static Guid RestoreGuid = Guid.Parse("ea10597e-6968-4897-8e7a-94ea0216e5cd");

            // Remove Election Languages
            public readonly static int Remove = 30;
            public readonly static Guid RemoveGuid = Guid.Parse("e10574ca-00d9-43c2-bb60-5b46d2742e7d");

        }
        public static class ElectionsManagement
        {
            // Access Election
            public readonly static int Access = 2;
            public readonly static Guid AccessGuid = Guid.Parse("aa24e4a0-cec2-410b-b855-8c70730eff86");

            // Create Election
            public readonly static int Create = 3;
            public readonly static Guid CreateGuid = Guid.Parse("0e224bbd-bbe8-4ce1-9e3a-a63195482e62");

            // Delete Election
            public readonly static int Delete = 4;
            public readonly static Guid DeleteGuid = Guid.Parse("bfb683e5-45bc-40f3-aaf0-34ac2f22f3ce");

            // Update Election
            public readonly static int Update = 5;
            public readonly static Guid UpdateGuid = Guid.Parse("6067c555-8ea4-4764-bdc2-b7cac1f85968");

            // Restore Election
            public readonly static int Restore = 6;
            public readonly static Guid RestoreGuid = Guid.Parse("4ab6da12-55db-4e93-b975-12988bb242a5");

        }
        public static class Factors
        {
            // Create Factors Codes
            public readonly static int Create = 32;
            public readonly static Guid CreateGuid = Guid.Parse("f9922c80-c1b1-4fe4-8aad-3f16505d970a");

            // Update Factors Codes
            public readonly static int Update = 33;
            public readonly static Guid UpdateGuid = Guid.Parse("fcafcae4-d90b-47f5-bf93-f3324d3dcbf6");

            // Delete Factors Codes
            public readonly static int Delete = 34;
            public readonly static Guid DeleteGuid = Guid.Parse("d979ea42-51fe-4007-be78-1bbe3996c410");

            // Restore Factors Codes
            public readonly static int Restore = 35;
            public readonly static Guid RestoreGuid = Guid.Parse("933df15a-1f0e-4f8a-8a3a-8c3e1c7ecb69");

            // Access Factors Codes
            public readonly static int Access = 114;
            public readonly static Guid AccessGuid = Guid.Parse("eb6189b1-3b87-4035-8563-ad916a1e44cb");

        }
        public static class FactorsLanguages
        {
            // Delete Factors Codes Languages
            public readonly static Guid DeleteGuid = Guid.Parse("facd3531-83bd-4cbd-a76e-4b50f5d5e1a1");

            // Create Factors Codes Languages
            public readonly static Guid CreateGuid = Guid.Parse("bdc28cde-66a5-4789-98cf-4301aba31f02");

            // Update Factors Codes Languages
            public readonly static Guid UpdateGuid = Guid.Parse("b127bad5-27be-4397-85b6-67a2635186fb");

            // Restore Factors Codes Languages
            public readonly static Guid RestoreGuid = Guid.Parse("bcaa75b5-eba5-4380-9f1f-2eb2f9ac95b3");

        }
        public static class FocalPointsManagement
        {
            // Access Focal Points
            public readonly static int Access = 21;
            public readonly static Guid AccessGuid = Guid.Parse("6abba00e-dbdd-4b00-83e3-866e298fec8d");

            // Create Focal Points
            public readonly static int Create = 22;
            public readonly static Guid CreateGuid = Guid.Parse("15fb07bf-eba3-48a1-9424-a31d41213643");

            // Delete Focal Points
            public readonly static int Delete = 23;
            public readonly static Guid DeleteGuid = Guid.Parse("4918ceb1-276e-4fbd-8c31-148663050a8d");

            // Restore Focal Points
            public readonly static int Restore = 24;
            public readonly static Guid RestoreGuid = Guid.Parse("dcee7c44-5917-4c4b-8ca8-e30cfddd9525");

            // Update Focal Points
            public readonly static int Update = 25;
            public readonly static Guid UpdateGuid = Guid.Parse("ae70904f-3d10-4b1e-af9b-ddbd81e52620");

        }
        public static class GenderConfigurations
        {
            // Remove Genders Configurations
            public readonly static Guid RemoveGuid = Guid.Parse("2338cf98-c5b5-4a15-bf17-539f5460f494");

            // Create Genders Configurations
            public readonly static Guid CreateGuid = Guid.Parse("54ec5e9d-d2a6-4a1a-bfe9-43187288a625");

        }
        public static class HandpunchDevicesConfiguration
        {
            // Access Handpunch Devices
            public readonly static int Access = 1;
            public readonly static Guid AccessGuid = Guid.Parse("0cb1f0fa-6d0f-43bd-a690-1e374df3e2f0");

            // Create Handpunch Devices
            public readonly static int Create = 2;
            public readonly static Guid CreateGuid = Guid.Parse("3acf5f67-2fc9-4fca-8323-987c09a93720");

            // Update Handpunch Devices
            public readonly static int Update = 3;
            public readonly static Guid UpdateGuid = Guid.Parse("80a4c14f-5761-4448-9748-936dce98b01a");

            // Delete Handpunch Devices
            public readonly static int Delete = 4;
            public readonly static Guid DeleteGuid = Guid.Parse("c37fbd9a-4927-4276-9c81-d67295e94e36");

            // Restore Handpunch Devices
            public readonly static int Restore = 5;
            public readonly static Guid RestoreGuid = Guid.Parse("ae94c010-e4c4-4821-886c-a100c6ca8762");

        }
        public static class HolidaysManagement
        {
            // Access Holidays
            public readonly static int Access = 11;
            public readonly static Guid AccessGuid = Guid.Parse("569b5a80-5066-4793-b131-ce5e18cebabb");

            // Create Holidays
            public readonly static int Create = 12;
            public readonly static Guid CreateGuid = Guid.Parse("8e183ea3-3cf2-4bc2-b6f8-397af3811765");

            // Update Holidays
            public readonly static int Update = 13;
            public readonly static Guid UpdateGuid = Guid.Parse("3e70472f-59ca-4628-af88-fecc3d41655a");

            // Delete Holidays
            public readonly static int Delete = 14;
            public readonly static Guid DeleteGuid = Guid.Parse("2d84a3db-a779-49ef-b335-aec4469fce66");

            // Restore Holidays
            public readonly static int Restore = 15;
            public readonly static Guid RestoreGuid = Guid.Parse("8d6efc29-4b7f-4c0e-b57e-7364838300f0");

        }
        public static class HomeAddress
        {
            // Create Home Address
            public readonly static Guid CreateGuid = Guid.Parse("9446a1f4-d9b2-4a12-936b-38669be3a086");

            // Update Home Address
            public readonly static Guid UpdateGuid = Guid.Parse("f5bd8e11-a6f8-4325-845b-4329572c3d58");

        }
        public static class HomeAddressLanguage
        {
            // Create Home Address Languages
            public readonly static Guid CreateGuid = Guid.Parse("b2de5f9e-37df-4d51-9ac9-38a61ee81e81");

            // Update Home Address Languages
            public readonly static Guid UpdateGuid = Guid.Parse("707879f4-8411-47b8-848e-25deb300c87f");

            // Restore Home Address Languages
            public readonly static Guid RestoreGuid = Guid.Parse("3ff0f0dc-aef0-4a5f-b28d-1976108e495f");

            // Delete Home Address Languages
            public readonly static Guid DeleteGuid = Guid.Parse("5ebb782f-7efa-4190-a29e-737ad63eb9be");

        }
        

        public static class OvertimeReviewForDrivers
        {
            // Access Overtime Review For Drivers
            public readonly static int Access = 86;
            public readonly static Guid AccessGuid = Guid.Parse("39e60b62-4a88-41ec-b70c-2902fcc4df8c");

            // Create Overtime Review For Drivers
            public readonly static int Create = 87;
            public readonly static Guid CreateGuid = Guid.Parse("88daa5c6-dfeb-4895-8af2-487d6acb7f24");

            // Confirm Overtime Review For Drivers
            public readonly static int Confirm = 88;
            public readonly static Guid ConfirmGuid = Guid.Parse("760b94d0-aa77-4b79-aef6-e7e40bede1dc");

            // ValidateData Overtime Review For Drivers
            public readonly static int ValidateData = 89;
            public readonly static Guid ValidateDataGuid = Guid.Parse("c86dc3b1-898b-4a05-8f7f-946d342ad2c1");

            // Update Overtime Review For Drivers
            public readonly static int Update = 90;
            public readonly static Guid UpdateGuid = Guid.Parse("b8c90631-fb92-4bc0-8a8a-f85d9939bc51");

        }
        public static class OvertimeReviewForStaff
        {
            // Access Overtime Review For Staff
            public readonly static int Access = 91;
            public readonly static Guid AccessGuid = Guid.Parse("12bc49a1-2104-42af-9cc2-d919bc56db2e");

            // Create Overtime Review For Staff
            public readonly static int Create = 92;
            public readonly static Guid CreateGuid = Guid.Parse("91f00ce1-cefb-4041-824f-f71e5fe50cb3");

            // Update Overtime Review For Staff
            public readonly static int Update = 93;
            public readonly static Guid UpdateGuid = Guid.Parse("075521dc-2ce2-4d0c-a2c7-795f289abb32");

            // Confirm Overtime Review For Staff
            public readonly static int Confirm = 94;
            public readonly static Guid ConfirmGuid = Guid.Parse("f6c90666-4258-43c2-8125-fe95a6f9056f");

            // ValidateData Overtime Review For Staff
            public readonly static int ValidateData = 95;
            public readonly static Guid ValidateDataGuid = Guid.Parse("a0251338-f05d-4185-8c20-e7ed70127ac5");

        }
        public static class StaffPrivacyDataManagement
        {
            // Access Staff Privacy Data Management
            public readonly static int Access = 19;
            public readonly static Guid AccessGuid = Guid.Parse("835f5f66-0257-4ab3-8a44-2d84957fe44a");

            // Create Staff Privacy Data Management
            public readonly static int Create = 20;
            public readonly static Guid CreateGuid = Guid.Parse("fbba9c0c-aaf3-477d-a5d6-377f7fa6e375");

            // Confirm Staff Privacy Data Management
            public readonly static int Confirm = 21;
            public readonly static Guid ConfirmGuid = Guid.Parse("2a638cc1-d161-4431-8633-c1faa78fce87");

        }
        public static class NationalDangerPaymentFocalPointAccess
        {
            // Access National Danger Payment Focal Point Access
            public readonly static int Access = 109;
            public readonly static Guid AccessGuid = Guid.Parse("c54598db-8e23-4c8f-9dae-6757f0177202");

        }
        public static class HRRestandRecuperationLeave
        {
            // Access HR Rest and Recuperation Leave
            public readonly static int Access = 23;
            public readonly static Guid AccessGuid = Guid.Parse("164da249-d403-445e-a22c-85b90dd0eb3d");

            // Create HR Rest and Recuperation Leave
            public readonly static int Create = 24;
            public readonly static Guid CreateGuid = Guid.Parse("f130c9e4-9ab1-43a0-b53c-2aa99dee771a");

            // Delete HR Rest and Recuperation Leave
            public readonly static int Delete = 25;
            public readonly static Guid DeleteGuid = Guid.Parse("efe5d93b-abab-45f1-b3c3-48855737b020");

            // Restore HR Rest and Recuperation Leave
            public readonly static int Restore = 26;
            public readonly static Guid RestoreGuid = Guid.Parse("6bf39a60-fa1e-4287-bedb-4d76712e2dc2");

            // Update HR Rest and Recuperation Leave
            public readonly static int Update = 27;
            public readonly static Guid UpdateGuid = Guid.Parse("610e9d4c-d716-4d9b-a974-8c31590188f3");

            // Remove HR Rest and Recuperation Leave
            public readonly static int Remove = 28;
            public readonly static Guid RemoveGuid = Guid.Parse("cc717332-487a-4f4d-8587-40ef263c91ed");

        }
        public static class InternationalStaffAttendancePresence
        {
            // Access International Staff Attendance Presence
            public readonly static int Access = 33;
            public readonly static Guid AccessGuid = Guid.Parse("5c247ca4-a68f-4669-921c-bb6a49d79733");

            // Create International Staff Attendance Presence
            public readonly static int Create = 34;
            public readonly static Guid CreateGuid = Guid.Parse("556b9b59-107f-41e6-a082-4bf9bec2ae6b");

            // Delete International Staff Attendance Presence
            public readonly static int Delete = 35;
            public readonly static Guid DeleteGuid = Guid.Parse("15699167-f333-439d-9609-82e9214da6d4");

            // Restore International Staff Attendance Presence
            public readonly static int Restore = 36;
            public readonly static Guid RestoreGuid = Guid.Parse("ce9b3811-f869-4bf3-9c7a-aeb332e8f4d7");

            // Update International Staff Attendance Presence
            public readonly static int Update = 37;
            public readonly static Guid UpdateGuid = Guid.Parse("cd871e06-df1d-4784-be6e-856c9d14c701");

        }
        public static class StaffShuttleDelegation
        {
            // Access Staff Shuttle Delegation
            public readonly static int Access = 54;
            public readonly static Guid AccessGuid = Guid.Parse("65ac5bd8-1d00-47d2-85d5-20dfe3f3db16");

            // Create Staff Shuttle Delegation
            public readonly static int Create = 55;
            public readonly static Guid CreateGuid = Guid.Parse("8a679478-c43e-45cf-b248-3deb5250ac8c");

            // Delete Staff Shuttle Delegation
            public readonly static int Delete = 56;
            public readonly static Guid DeleteGuid = Guid.Parse("d982bd49-5e70-4d2d-a7fa-ec6a0bcdac3d");

            // Restore Staff Shuttle Delegation
            public readonly static int Restore = 57;
            public readonly static Guid RestoreGuid = Guid.Parse("34e57941-eea9-442f-b9a1-23f418f833b6");

            // Update Staff Shuttle Delegation
            public readonly static int Update = 58;
            public readonly static Guid UpdateGuid = Guid.Parse("3ede09c8-74cf-45ee-91b8-f75920886b36");
            public readonly static int Print = 59;
            public readonly static Guid PrintGuid = Guid.Parse("d049622e-3771-45c2-827b-6d2a384836cd");

        }
        public static class HRUNHCRContractsManagement
        {
            // Access HR UNHCR Contracts Management
            public readonly static int Access = 45;
            public readonly static Guid AccessGuid = Guid.Parse("9001ba39-6996-4042-a651-36f68912c5a7");

            // Create HR UNHCR Contracts Management
            public readonly static int Create = 46;
            public readonly static Guid CreateGuid = Guid.Parse("3ed5929c-665c-488a-92a5-3cbb7f804f19");

        }
        public static class HRUNOPSContractsManagement
        {
            // Access HR UNOPS Contracts Management
            public readonly static int Access = 47;
            public readonly static Guid AccessGuid = Guid.Parse("b49a0e2f-7819-4558-b165-473e4fc19630");

            // Create HR UNOPS Contracts Management
            public readonly static int Create = 48;
            public readonly static Guid CreateGuid = Guid.Parse("bd9e9b49-46fd-4829-91ba-7b8c09cd9e23");

        }
        public static class InternationalStaffEntitlements
        {
            // Access International Staff Entitlements
            public readonly static int Access = 38;
            public readonly static Guid AccessGuid = Guid.Parse("5ed224c3-5c93-448d-8e0c-d1987bf0247a");

            // Create International Staff Entitlements
            public readonly static int Create = 39;
            public readonly static Guid CreateGuid = Guid.Parse("e3921170-0946-433b-9c34-82c64c6385e8");

            // Delete International Staff Entitlements
            public readonly static int Delete = 40;
            public readonly static Guid DeleteGuid = Guid.Parse("13e37ffd-6789-4ebf-93ae-854dbb07da3a");

            // Update International Staff Entitlements
            public readonly static int Update = 41;
            public readonly static Guid UpdateGuid = Guid.Parse("6d0db95d-0276-4e63-b5d5-a05e3aa18da1");

            // Restore International Staff Entitlements
            public readonly static int Restore = 42;
            public readonly static Guid RestoreGuid = Guid.Parse("ac59e043-0249-4681-9193-62588d006c51");

        }
        public static class InternationalStaffEntitlementsCertifying
        {
            // Access International Staff Entitlements Certifying
            public readonly static int Access = 80;
            public readonly static Guid AccessGuid = Guid.Parse("0a463fe4-fa3f-4000-8f9e-d7217afdfa7c");

            // Create International Staff Entitlements Certifying
            public readonly static int Create = 81;
            public readonly static Guid CreateGuid = Guid.Parse("008478dc-7086-4261-a711-fcb0d3cf1787");

            // Confirm International Staff Entitlements Certifying
            public readonly static int Confirm = 82;
            public readonly static Guid ConfirmGuid = Guid.Parse("b08322f1-5fcf-4f63-95b3-0c51f7ec8e85");

            // Delete International Staff Entitlements Certifying
            public readonly static int Delete = 83;
            public readonly static Guid DeleteGuid = Guid.Parse("85a10496-320c-447f-be60-2c2a32255649");

            // Restore International Staff Entitlements Certifying
            public readonly static int Restore = 84;
            public readonly static Guid RestoreGuid = Guid.Parse("bc721076-4288-4c8f-a5b5-05e115a1fcb8");

            // Update International Staff Entitlements Certifying
            public readonly static int Update = 85;
            public readonly static Guid UpdateGuid = Guid.Parse("58374a58-f6ff-4896-92f1-a46679a1ac8b");

        }
        public static class InternationalStaffR_RRepresentativeReview
        {
            // Access International Staff R_R Representative Review
            public readonly static int Access = 43;
            public readonly static Guid AccessGuid = Guid.Parse("fc0b6b8c-b599-4851-a56b-640ae76eaa29");

            // Create International Staff R_R Representative Review
            public readonly static int Create = 44;
            public readonly static Guid CreateGuid = Guid.Parse("b3bdf367-3f74-45e4-af0a-1aee013c0e81");

        }
        public static class InternationalStaffRestAndRecuperationLeave
        {
            // Access International Staff Rest And Recuperation Leave
            public readonly static int Access = 17;
            public readonly static Guid AccessGuid = Guid.Parse("dc9341d4-618c-4364-bdc2-8e008e9da319");

            // Create International Staff Rest And Recuperation Leave
            public readonly static int Create = 18;
            public readonly static Guid CreateGuid = Guid.Parse("5599b3b3-24a2-4034-9138-c67abfaed174");

            // Delete International Staff Rest And Recuperation Leave
            public readonly static int Delete = 19;
            public readonly static Guid DeleteGuid = Guid.Parse("c9aba0da-9d0a-464f-a545-e51482ef305c");

            // Remove International Staff Rest And Recuperation Leave
            public readonly static int Remove = 20;
            public readonly static Guid RemoveGuid = Guid.Parse("11c67d8e-3bf4-4290-84c6-3a5eb7cc3e28");

            // Restore International Staff Rest And Recuperation Leave
            public readonly static int Restore = 21;
            public readonly static Guid RestoreGuid = Guid.Parse("d1bc8e01-eb88-4b0f-a233-dc163ca501ee");

            // Update International Staff Rest And Recuperation Leave
            public readonly static int Update = 22;
            public readonly static Guid UpdateGuid = Guid.Parse("5fca5020-5482-4422-bf65-dc7227560d19");

        }
        public static class InternationalStaffRestAndRecuperationLeaveHRReview
        {
            // Access International Staff R_R Leave HR Review
            public readonly static int Access = 31;
            public readonly static Guid AccessGuid = Guid.Parse("de411f27-0ea8-49fb-964d-00833bb012f8");

            // Create International Staff R_R Leave HR Review
            public readonly static int Create = 32;
            public readonly static Guid CreateGuid = Guid.Parse("6102e16b-e9b5-4e6f-ae65-7d2e994e64b7");

        }
        public static class IntervalTypes
        {
            // Access Interval Types
            public readonly static int Access = 16;
            public readonly static Guid AccessGuid = Guid.Parse("d796fa88-a239-4b6f-8156-dd638708e5bc");

            // Create Interval Types
            public readonly static int Create = 17;
            public readonly static Guid CreateGuid = Guid.Parse("a028ea37-5bf7-42d8-a9fb-32574f661d53");

            // Delete Interval Types
            public readonly static int Delete = 18;
            public readonly static Guid DeleteGuid = Guid.Parse("713b8494-b21e-499a-aa2f-48fe028086de");

            // Remove Interval Types
            public readonly static int Remove = 19;
            public readonly static Guid RemoveGuid = Guid.Parse("99cd0235-d86b-4da1-837e-eefedceed6a2");

            // Restore Interval Types
            public readonly static int Restore = 20;
            public readonly static Guid RestoreGuid = Guid.Parse("4ec1e764-1282-4c71-b4d3-00069d0801ba");

            // Update Interval Types
            public readonly static int Update = 21;
            public readonly static Guid UpdateGuid = Guid.Parse("1c27a28d-3044-4dda-8006-bd3a4693cdae");

        }
        public static class IntervalTypesLanguages
        {
            // Create Interval Type Languages
            public readonly static int Create = 22;
            public readonly static Guid CreateGuid = Guid.Parse("fdcc4377-2f76-4727-a66e-8fd2a6c0abc9");

            // Delete Interval Type Languages
            public readonly static int Delete = 23;
            public readonly static Guid DeleteGuid = Guid.Parse("9fba5c5d-d1c1-4755-b3e9-273701e800fe");

            // Update Interval Type Languages
            public readonly static int Update = 24;
            public readonly static Guid UpdateGuid = Guid.Parse("26cd5590-c99b-43ab-8ff7-a5648fb15e2c");

            // Restore Interval Type Languages
            public readonly static int Restore = 25;
            public readonly static Guid RestoreGuid = Guid.Parse("7be4d452-56fc-4c0c-8f39-fec6428bfe36");

        }
        public static class ItemModel
        {
            // Access Warehouse Item Model
            public readonly static int Access = 6;
            public readonly static Guid AccessGuid = Guid.Parse("026afb70-e12b-4f9c-b1e1-77a3823d8ce9");

            // Create Warehouse Item Model
            public readonly static int Create = 7;
            public readonly static Guid CreateGuid = Guid.Parse("13502d2c-b11a-4b0c-bf8c-332752b1e74d");

            // Delete Warehouse Item Model
            public readonly static int Delete = 8;
            public readonly static Guid DeleteGuid = Guid.Parse("1fadc2c9-e23f-4c6e-ac72-4c1e5d38b59d");

            // Update Warehouse Item Model
            public readonly static int Update = 9;
            public readonly static Guid UpdateGuid = Guid.Parse("b7b760bc-abf7-4d22-bbe6-771cf6f2b980");

            // Restore Warehouse Item Model
            public readonly static int Restore = 10;
            public readonly static Guid RestoreGuid = Guid.Parse("2951e21a-8d3d-44ac-acfd-235b0044d0d0");

        }
        public static class ItemOverAllStock
        {
            // Access Item Over All Stock
            public readonly static int Access = 6;
            public readonly static Guid AccessGuid = Guid.Parse("ba12c14a-8e6e-4dc7-b917-59956970d7ae");

            // Create Item Over All Stock
            public readonly static int Create = 7;
            public readonly static Guid CreateGuid = Guid.Parse("b24c7a37-d10e-43d9-9638-5cd0bf79c768");

            // Delete Item Over All Stock
            public readonly static int Delete = 8;
            public readonly static Guid DeleteGuid = Guid.Parse("29303af9-4fcf-4282-a3f7-ef6d9bd0b0f3");

            // Restore Item Over All Stock
            public readonly static int Restore = 9;
            public readonly static Guid RestoreGuid = Guid.Parse("a8c6df0f-f5e8-40b3-bbdb-fe618fecd663");

            // Update Item Over All Stock
            public readonly static int Update = 10;
            public readonly static Guid UpdateGuid = Guid.Parse("ebb771c7-f2a5-405e-953b-487d35a3c420");

        }
        public static class JobTitles
        {
            // Access Job Titles Codes
            public readonly static int Access = 36;
            public readonly static Guid AccessGuid = Guid.Parse("f3395e9d-7c79-4e74-88d9-08ca89005072");

            // Create Job Titles Codes
            public readonly static int Create = 37;
            public readonly static Guid CreateGuid = Guid.Parse("d7b40f72-46ee-4614-b0ba-df7093609df7");

            // Update Job Titles Codes
            public readonly static int Update = 38;
            public readonly static Guid UpdateGuid = Guid.Parse("433f78df-4eba-4519-a976-ccfd7b3b762f");

            // Delete Job Titles Codes
            public readonly static int Delete = 39;
            public readonly static Guid DeleteGuid = Guid.Parse("0e779721-4465-426d-acd3-20d18eb2ce17");

            // Restore Job Titles Codes
            public readonly static int Restore = 40;
            public readonly static Guid RestoreGuid = Guid.Parse("a9ba992a-fbee-4d46-a195-0b94870e7a78");

        }
        public static class JobTitlesLanguages
        {
            // Restore Job Titles Codes Languages
            public readonly static Guid RestoreGuid = Guid.Parse("7f4b50dc-42ed-46c8-99f3-70950827f3ec");

            // Update Job Titles Codes Languages
            public readonly static Guid UpdateGuid = Guid.Parse("91c9e062-2cfe-4bfb-9ef9-8f76405ef694");

            // Create Job Titles Codes Languages
            public readonly static Guid CreateGuid = Guid.Parse("e3404546-d3d5-4ced-a724-83144b783563");

            // Delete Job Titles Codes Languages
            public readonly static Guid DeleteGuid = Guid.Parse("3bec163c-9631-4b30-9d6b-efa8bf320184");

        }
        public static class Jobs
        {
            // Remove Jobs
            public readonly static Guid RemoveGuid = Guid.Parse("4d01c138-59ba-46fd-a34e-f5ffaf28c43b");

            // Create Jobs
            public readonly static Guid CreateGuid = Guid.Parse("c69da20f-6b40-4a55-b941-aad7b6fdf800");

            // Update Jobs
            public readonly static Guid UpdateGuid = Guid.Parse("b07602af-085d-4be2-822b-03b988d04a5c");

        }
        public static class Locations
        {
            // Access Locations Codes
            public readonly static int Access = 7;
            public readonly static Guid AccessGuid = Guid.Parse("35abf751-34d1-45cf-9d63-67f96d09f1ee");

            // Create Locations Codes
            public readonly static int Create = 42;
            public readonly static Guid CreateGuid = Guid.Parse("10d28a04-4fd2-4117-bbdf-6c02421db3ac");

            // Update Locations Codes
            public readonly static int Update = 43;
            public readonly static Guid UpdateGuid = Guid.Parse("7f19a986-141f-453d-879c-82f466557dd3");

            // Delete Locations Codes
            public readonly static int Delete = 44;
            public readonly static Guid DeleteGuid = Guid.Parse("a8eddf7b-ead6-4dba-a0ba-5248008eb9bf");

            // Restore Locations Codes
            public readonly static int Restore = 45;
            public readonly static Guid RestoreGuid = Guid.Parse("44ba969a-18e3-46c1-b150-947e05818841");

        }
        public static class LocationsLanguages
        {
            // Delete Locations Codes Languages
            public readonly static Guid DeleteGuid = Guid.Parse("fdcce3af-d7c8-4d00-b8f9-74b9ee7dee76");

            // Create Locations Codes Languages
            public readonly static Guid CreateGuid = Guid.Parse("70e0b352-ae8c-473d-b97e-4c2d4b284597");

            // Restore Locations Codes Languages
            public readonly static Guid RestoreGuid = Guid.Parse("5db6e3bd-16af-4305-9a8c-3eac9f0731ed");

            // Update Locations Codes Languages
            public readonly static Guid UpdateGuid = Guid.Parse("9e6b508f-246c-40b9-8de2-1137aa19d7a8");

        }
        public static class LookupValues
        {
            // Access Lookup Values Codes
            public readonly static int Access = 81;
            public readonly static Guid AccessGuid = Guid.Parse("3fd92a7a-6204-4cd3-b947-72798d9ed590");

            // Create Lookup Values Codes
            public readonly static int Create = 82;
            public readonly static Guid CreateGuid = Guid.Parse("fdb40faf-67f0-4ff9-ba65-22cddb8e1da5");

            // Update Lookup Values Codes
            public readonly static int Update = 83;
            public readonly static Guid UpdateGuid = Guid.Parse("7408f1d3-913b-4beb-b7c2-96a97cc96604");

            // Delete Lookup Values Codes
            public readonly static int Delete = 84;
            public readonly static Guid DeleteGuid = Guid.Parse("ff1116d3-980d-4f19-930f-fa76f82c2efe");

            // Restore Lookup Values Codes
            public readonly static int Restore = 85;
            public readonly static Guid RestoreGuid = Guid.Parse("97fc38a8-8349-4249-9b82-0a35d152f8e2");

        }
        public static class LookupValuesLanguages
        {
            // Restore Lookup Values Codes Lanaguages
            public readonly static Guid RestoreGuid = Guid.Parse("ed1e809d-53cf-4a87-8222-6c715beb04e8");

            // Delete Lookup Values Codes Lanaguages
            public readonly static Guid DeleteGuid = Guid.Parse("f6af3308-7b9a-4ed6-ad46-70772c4bf838");

            // Create Lookup Values Codes Lanaguages
            public readonly static Guid CreateGuid = Guid.Parse("0e88f32a-a8ff-4ccc-80f9-3c5c70363fb7");

            // Upload Lookup Values Codes Lanaguages
            public readonly static int Upload = 86;
            public readonly static Guid UploadGuid = Guid.Parse("571536eb-88c1-48ba-94e7-e7a409231de3");

        }
        public static class Main
        {
            // Access FWS Main
            public readonly static int Access = 1;
            public readonly static Guid AccessGuid = Guid.Parse("164acac9-780f-48a9-ab31-7420aa495d38");

            // Create FWS Main
            public readonly static int Create = 2;
            public readonly static Guid CreateGuid = Guid.Parse("fdd54029-ea58-41fd-a02f-d50e56a8a3fe");

            // Update FWS Main
            public readonly static int Update = 3;
            public readonly static Guid UpdateGuid = Guid.Parse("53718c8c-2b39-447b-aca3-aadf943ee79c");

            // Delete FWS Main
            public readonly static int Delete = 4;
            public readonly static Guid DeleteGuid = Guid.Parse("0df69354-2947-4337-9a4b-01ca60421d74");

            // Restore FWS Main
            public readonly static int Restore = 5;
            public readonly static Guid RestoreGuid = Guid.Parse("8f8e9be6-64f9-4f8b-bb70-90520089e54c");

        }
        public static class Managers
        {
            // Update Employee Managers
            public readonly static Guid UpdateGuid = Guid.Parse("20070a5f-8b02-408a-aa48-d4f7e9c423e8");

            // Remove Employee Managers
            public readonly static Guid RemoveGuid = Guid.Parse("4b15d8e6-8895-4bde-bab0-919144cb1174");

            // Create Employee Managers
            public readonly static Guid CreateGuid = Guid.Parse("05ad15cf-0ac5-45bc-9b48-2cf310bb0680");

        }
        public static class MedicalBeneficiary
        {
            // Access Medical Beneficiary
            public readonly static int Access = 37;
            public readonly static Guid AccessGuid = Guid.Parse("eb75df43-85a7-4f27-b0ed-c94f594f4a7c");

            // Create Medical Beneficiary
            public readonly static int Create = 38;
            public readonly static Guid CreateGuid = Guid.Parse("f6ef742e-0d4c-4c7a-8060-6d48bc13ad42");

            // Delete Medical Beneficiary
            public readonly static int Delete = 39;
            public readonly static Guid DeleteGuid = Guid.Parse("ea4dcff6-caa2-423f-8d50-886b8f7cbcb3");

            // Restore Medical Beneficiary
            public readonly static int Restore = 40;
            public readonly static Guid RestoreGuid = Guid.Parse("3e81228a-db20-4320-97e9-521c54262335");

            // Update Medical Beneficiary
            public readonly static int Update = 41;
            public readonly static Guid UpdateGuid = Guid.Parse("64c74085-113f-4010-b373-793aacfa771f");

        }
        public static class MedicalBeneficiaryItemOut
        {
            // Access Medical Beneficiary Item Out
            public readonly static int Access = 31;
            public readonly static Guid AccessGuid = Guid.Parse("8ca60d08-d9bb-461e-aed7-9d646bfaccc2");

            // Create Medical Beneficiary Item Out
            public readonly static int Create = 32;
            public readonly static Guid CreateGuid = Guid.Parse("5042526a-a299-4c55-a345-aefd27560904");

            // Delete Medical Beneficiary Item Out
            public readonly static int Delete = 33;
            public readonly static Guid DeleteGuid = Guid.Parse("d59d3e5a-3aac-48fe-868c-72ab2f7c314a");

            // Restore Medical Beneficiary Item Out
            public readonly static int Restore = 35;
            public readonly static Guid RestoreGuid = Guid.Parse("39680853-faf6-4a3c-874c-197345219428");

            // Update Medical Beneficiary Item Out
            public readonly static int Update = 36;
            public readonly static Guid UpdateGuid = Guid.Parse("1d37a33d-24cf-4ead-b06b-615420a178e5");

            // Confirm Medical Beneficiary Item Out
            public readonly static int Confirm = 48;
            public readonly static Guid ConfirmGuid = Guid.Parse("a0a869c1-bbe7-4eff-a376-7a0ec291bff8");

        }
        public static class MedicalDiscrepancy
        {
            // Access Medical Discrepancy
            public readonly static int Access = 49;
            public readonly static Guid AccessGuid = Guid.Parse("89f78ae6-8eed-41ca-9303-1d49fe50dae1");

            // Create Medical Discrepancy
            public readonly static int Create = 50;
            public readonly static Guid CreateGuid = Guid.Parse("92434f5d-f94f-42bb-899c-b2a2b70c9345");

            // Delete Medical Discrepancy
            public readonly static int Delete = 51;
            public readonly static Guid DeleteGuid = Guid.Parse("352bdd77-e497-4a7e-9234-7b63aef63d8c");

            // Restore Medical Discrepancy
            public readonly static int Restore = 52;
            public readonly static Guid RestoreGuid = Guid.Parse("137f4338-6141-4303-9656-47ff4b915133");

            // Update Medical Discrepancy
            public readonly static int Update = 53;
            public readonly static Guid UpdateGuid = Guid.Parse("9984e0b7-9321-491a-bf1d-ee351cb59c80");

        }
        public static class MedicalDistributionRestriction
        {
            // Access Medical Distribution Restriction
            public readonly static int Access = 54;
            public readonly static Guid AccessGuid = Guid.Parse("baba1b03-2f93-4f18-815e-c4f62938d78d");

            // Create Medical Distribution Restriction
            public readonly static int Create = 55;
            public readonly static Guid CreateGuid = Guid.Parse("2a78d807-c429-4eef-b815-7ca3b63bf2dc");

            // Delete Medical Distribution Restriction
            public readonly static int Delete = 56;
            public readonly static Guid DeleteGuid = Guid.Parse("fcd3ed02-655c-417c-a107-4931d8cc1e30");

            // Restore Medical Distribution Restriction
            public readonly static int Restore = 57;
            public readonly static Guid RestoreGuid = Guid.Parse("4b8c29a1-45d7-4674-a75e-85c4c3d4ea27");

            // Update Medical Distribution Restriction
            public readonly static int Update = 58;
            public readonly static Guid UpdateGuid = Guid.Parse("8d0606af-4c0f-4a30-b28c-29748f4a12ad");

        }
        public static class MedicalGenericName
        {
            // Access Medical Generic Name
            public readonly static int Access = 16;
            public readonly static Guid AccessGuid = Guid.Parse("15a104ac-869e-4612-bce3-f04c91fc9736");

            // Create Medical Generic Name
            public readonly static int Create = 17;
            public readonly static Guid CreateGuid = Guid.Parse("749e3106-7df0-4b51-a61e-564c7de05094");

            // Delete Medical Generic Name
            public readonly static int Delete = 18;
            public readonly static Guid DeleteGuid = Guid.Parse("f8cbc986-2d29-4d44-bf16-f2b7597c7007");

            // Restore Medical Generic Name
            public readonly static int Restore = 19;
            public readonly static Guid RestoreGuid = Guid.Parse("1853650f-70a5-46d5-ae20-00ff56106126");

            // Update Medical Generic Name
            public readonly static int Update = 20;
            public readonly static Guid UpdateGuid = Guid.Parse("6e5c5cdd-4842-4270-b19b-edd74121f68f");

        }
        public static class MedicalItem
        {
            // Access Medical Item
            public readonly static int Access = 6;
            public readonly static Guid AccessGuid = Guid.Parse("d53a798f-7c82-4973-abdc-104d8df842af");

            // Create Medical Item
            public readonly static int Create = 7;
            public readonly static Guid CreateGuid = Guid.Parse("9bf4ba0d-67d5-4273-9bc3-2e14a293d802");

            // Delete Medical Item
            public readonly static int Delete = 8;
            public readonly static Guid DeleteGuid = Guid.Parse("0e1eb16b-938f-4639-a4ca-a253c8edf4c3");

            // Restore Medical Item
            public readonly static int Restore = 9;
            public readonly static Guid RestoreGuid = Guid.Parse("c5b35e63-4290-4e33-ac8d-13c1542cf431");

            // Update Medical Item
            public readonly static int Update = 10;
            public readonly static Guid UpdateGuid = Guid.Parse("368014b8-7f93-4cf0-9267-ad2cae9e1596");

        }
        public static class MedicalItemInput
        {
            // Access Medical Item Input
            public readonly static int Access = 21;
            public readonly static Guid AccessGuid = Guid.Parse("2e43b5d7-d210-4b69-8a82-db8b44f3068f");

            // Create Medical Item Input
            public readonly static int Create = 22;
            public readonly static Guid CreateGuid = Guid.Parse("64f6dfee-c9b5-414f-aa24-0fbaa03e18f2");

            // Delete Medical Item Input
            public readonly static int Delete = 23;
            public readonly static Guid DeleteGuid = Guid.Parse("3f25c831-3048-43e0-9f01-bda1506a71d5");

            // Restore Medical Item Input
            public readonly static int Restore = 24;
            public readonly static Guid RestoreGuid = Guid.Parse("11361fb5-35b2-4974-896b-6eeb835667bc");

            // Update Medical Item Input
            public readonly static int Update = 25;
            public readonly static Guid UpdateGuid = Guid.Parse("e6de7e23-af30-455f-b6e7-504bf9204d0c");

            // Confirm Medical Item Input
            public readonly static int Confirm = 47;
            public readonly static Guid ConfirmGuid = Guid.Parse("a4194702-913b-4b48-9b83-1b20a5894ca8");

        }
        public static class MedicalItemInputSupply
        {
            // Access Medical Item Input Supply
            public readonly static int Access = 42;
            public readonly static Guid AccessGuid = Guid.Parse("bc92fd2d-eb8c-43a4-9bb0-5661bac97026");

            // Create Medical Item Input Supply
            public readonly static int Create = 43;
            public readonly static Guid CreateGuid = Guid.Parse("f6ce6c4d-f4b1-4f9c-a3c2-72c36c41edea");

            // Delete Medical Item Input Supply
            public readonly static int Delete = 44;
            public readonly static Guid DeleteGuid = Guid.Parse("9e92b250-3f91-49bc-ab42-d1296d26ee93");

            // Restore Medical Item Input Supply
            public readonly static int Restore = 45;
            public readonly static Guid RestoreGuid = Guid.Parse("d79a821a-d243-4caa-a22a-107c605ff971");

            // Update Medical Item Input Supply
            public readonly static int Update = 46;
            public readonly static Guid UpdateGuid = Guid.Parse("dbcad9b5-0b03-4f3d-a954-ea68edf749af");

        }
        public static class MedicalItemTransfer
        {
            // Access Medical Item Transfer
            public readonly static int Access = 26;
            public readonly static Guid AccessGuid = Guid.Parse("c1232c47-5e55-4b3c-9e12-42148e9c0ceb");

            // Create Medical Item Transfer
            public readonly static int Create = 27;
            public readonly static Guid CreateGuid = Guid.Parse("e9dbb7c7-fda8-4d32-84f9-b7bfbed49e28");

            // Delete Medical Item Transfer
            public readonly static int Delete = 28;
            public readonly static Guid DeleteGuid = Guid.Parse("31100544-bcbb-4d89-ae93-f73fbe556346");

            // Restore Medical Item Transfer
            public readonly static int Restore = 29;
            public readonly static Guid RestoreGuid = Guid.Parse("4c4911ed-b150-4cca-87a8-649f986152ac");

            // Update Medical Item Transfer
            public readonly static int Update = 30;
            public readonly static Guid UpdateGuid = Guid.Parse("2619fa2e-d59b-45ad-8d5c-4fc3f77dd46e");

        }
        public static class MedicalManufacturer
        {
            // Access Medical Manufacturer
            public readonly static int Access = 11;
            public readonly static Guid AccessGuid = Guid.Parse("dd96baab-01ac-4cbf-9acd-5bfb833283cd");

            // Create Medical Manufacturer
            public readonly static int Create = 12;
            public readonly static Guid CreateGuid = Guid.Parse("c7bb3927-2ec2-48d9-a92d-1597d900fd10");

            // Delete Medical Manufacturer
            public readonly static int Delete = 13;
            public readonly static Guid DeleteGuid = Guid.Parse("02d3275c-c433-454e-ac28-585484d03d2c");

            // Restore Medical Manufacturer
            public readonly static int Restore = 14;
            public readonly static Guid RestoreGuid = Guid.Parse("7a3f8fd2-9ce8-443f-a852-1b09b8dcde37");

            // Update Medical Manufacturer
            public readonly static int Update = 15;
            public readonly static Guid UpdateGuid = Guid.Parse("334a965c-5cf8-460d-8188-899a3354b16e");

        }
        public static class MedicalPharmacy
        {
            // Access Medical Pharmacy
            public readonly static int Access = 1;
            public readonly static Guid AccessGuid = Guid.Parse("b4ea5aad-7bb6-4ef8-8a34-8e2892f594df");

            // Create Medical Pharmacy
            public readonly static int Create = 2;
            public readonly static Guid CreateGuid = Guid.Parse("0447bdf3-3cfe-44f6-85d5-46c0900712c4");

            // Delete Medical Pharmacy
            public readonly static int Delete = 3;
            public readonly static Guid DeleteGuid = Guid.Parse("90431335-6f43-43ae-ae2e-e01096331279");

            // Restore Medical Pharmacy
            public readonly static int Restore = 4;
            public readonly static Guid RestoreGuid = Guid.Parse("df438974-1a88-4fbb-8132-c4490998a293");

            // Update Medical Pharmacy
            public readonly static int Update = 5;
            public readonly static Guid UpdateGuid = Guid.Parse("0a3ec96e-39a9-4441-8b36-09f090032298");

        }
        public static class MissionReportActionsForm
        {
            // Access Mission Report Action Form
            public readonly static int Access = 10;
            public readonly static Guid AccessGuid = Guid.Parse("b3e83413-2a1f-4cc3-9f21-07cafeb8e7c7");

            // Create Mission Report Action Form
            public readonly static int Create = 11;
            public readonly static Guid CreateGuid = Guid.Parse("bf2eec0a-f9d1-427a-91cb-d12a204f5d22");

            // Update Mission Report Action Form
            public readonly static int Update = 12;
            public readonly static Guid UpdateGuid = Guid.Parse("c54fbb7d-53d8-49b7-97c0-8326381804cb");

            // Delete Mission Report Action Form
            public readonly static int Delete = 13;
            public readonly static Guid DeleteGuid = Guid.Parse("4a7fe7e7-dcaf-4361-9d3b-52be894196e0");

            // Restore Mission Report Action Form
            public readonly static int Restore = 14;
            public readonly static Guid RestoreGuid = Guid.Parse("53433ad4-f8ed-4ffb-83ae-3392a0553a6d");

        }
        public static class MissionReportForm
        {
            // Access Mission Report Form
            public readonly static Guid AccessGuid = Guid.Parse("b98057ca-3318-4452-a4bf-d8d6c2905d61");

            // Restore Mission Report Form
            public readonly static Guid RestoreGuid = Guid.Parse("5318cf80-efe3-4d9f-bd1e-960246e95168");

            // Delete Mission Report Form
            public readonly static int Delete = 1;
            public readonly static Guid DeleteGuid = Guid.Parse("d1fbd7bb-0ad9-4c56-a061-2e0ad9c57e52");

            // Create Mission Report Form
            public readonly static int Create = 7;
            public readonly static Guid CreateGuid = Guid.Parse("59f262ed-c6bc-484b-97a6-f87783169604");

            // Update Mission Report Form
            public readonly static int Update = 8;
            public readonly static Guid UpdateGuid = Guid.Parse("fcccbe0d-e8d9-4a54-a216-5c9047d72a17");

        }
        public static class MissionTrackingActionDetails
        {
            // Access Mission Tracking Action Details
            public readonly static int Access = 15;
            public readonly static Guid AccessGuid = Guid.Parse("a2ba64a5-d4b1-4b45-a46c-bff6ef2a37ad");

            // Create Mission Tracking Action Details
            public readonly static int Create = 16;
            public readonly static Guid CreateGuid = Guid.Parse("3496dc45-80c7-4085-85c4-e109a2a8e652");

            // Delete Mission Tracking Action Details
            public readonly static int Delete = 17;
            public readonly static Guid DeleteGuid = Guid.Parse("2f2f8613-2970-40a3-b212-d9a0900c799f");

            // Update Mission Tracking Action Details
            public readonly static int Update = 18;
            public readonly static Guid UpdateGuid = Guid.Parse("28bc45a4-d4f5-405b-8367-c301a86bae45");

        }
        public static class NationalStaffDangerPayManagement
        {
            // Access National Staff Danger Pay Management
            public readonly static int Access = 6;
            public readonly static Guid AccessGuid = Guid.Parse("995d1dc5-543e-413d-98dd-d86786dd1553");

            // Create National Staff Danger Pay Management
            public readonly static int Create = 7;
            public readonly static Guid CreateGuid = Guid.Parse("44695421-3f3f-4aab-a564-d7d1812f97b0");

            // Delete National Staff Danger Pay Management
            public readonly static int Delete = 8;
            public readonly static Guid DeleteGuid = Guid.Parse("7198e0d4-69d7-483f-9440-b3717a593707");

            // Restore National Staff Danger Pay Management
            public readonly static int Restore = 9;
            public readonly static Guid RestoreGuid = Guid.Parse("9ec79214-314e-4748-9203-0e786c7e26c2");

            // Update National Staff Danger Pay Management
            public readonly static int Update = 10;
            public readonly static Guid UpdateGuid = Guid.Parse("945c4a0a-37db-416e-a33b-923485f98bd2");

        }
        public static class NavigationMenus
        {
            // Access Navigation Menus Codes
            public readonly static int Access = 76;
            public readonly static Guid AccessGuid = Guid.Parse("7e8083a6-c546-41ba-8eb8-3dcb655d28b2");

            // Create Navigation Menus Codes
            public readonly static int Create = 77;
            public readonly static Guid CreateGuid = Guid.Parse("009ba7a5-45ff-4d0c-807c-00cc8134f9e0");

            // Update Navigation Menus Codes
            public readonly static int Update = 78;
            public readonly static Guid UpdateGuid = Guid.Parse("4724c46b-7179-47d2-8ad0-fc78ff206b6d");

            // Delete Navigation Menus Codes
            public readonly static int Delete = 79;
            public readonly static Guid DeleteGuid = Guid.Parse("b8495b5e-d347-48ac-9f7b-2c703f57912f");

            // Restore Navigation Menus Codes
            public readonly static int Restore = 80;
            public readonly static Guid RestoreGuid = Guid.Parse("98ebc5eb-acbc-4da6-b086-eee7c0533721");

        }
        public static class NavigationMenusLanguages
        {
            // Restore Navigation Menus Codes Language
            public readonly static Guid RestoreGuid = Guid.Parse("cc7bc246-c440-4137-9f29-3d1c829cdd0e");

            // Update Navigation Menus Codes Language
            public readonly static Guid UpdateGuid = Guid.Parse("b5573f63-2f42-4ce6-99bc-39713088faf5");

            // Delete Navigation Menus Codes Language
            public readonly static Guid DeleteGuid = Guid.Parse("73894797-eed3-43f7-9953-0447e58ae4a2");

            // Create Navigation Menus Codes Language
            public readonly static Guid CreateGuid = Guid.Parse("d980863d-352c-4dbf-8d6d-d96226a205f7");

        }
        public static class Newservicerequest
        {
            // Create New services requests
            public readonly static Guid CreateGuid = Guid.Parse("b507136f-3b77-45ab-8fc1-a0dea0014c94");

            // Update New services requests
            public readonly static int Update = 1;
            public readonly static Guid UpdateGuid = Guid.Parse("3df8ef34-9685-4be9-954c-a4bac3545172");

            // Delete New services requests
            public readonly static int Delete = 2;
            public readonly static Guid DeleteGuid = Guid.Parse("132b5ec0-c4cd-4341-810b-39877ffbc901");

            // Restore New services requests
            public readonly static int Restore = 3;
            public readonly static Guid RestoreGuid = Guid.Parse("36772aa1-5674-4591-b473-0244b982ce94");

        }
        public static class NoteVerbale
        {
            // Access Note Verbale
            public readonly static int Access = 1;
            public readonly static Guid AccessGuid = Guid.Parse("d2f447af-c4c7-487b-8cf2-47db5743d65a");

            // Create Note Verbale
            public readonly static int Create = 2;
            public readonly static Guid CreateGuid = Guid.Parse("3bd83b36-ea54-4968-ae27-6d9658a4f895");

            // Delete Note Verbale
            public readonly static int Delete = 3;
            public readonly static Guid DeleteGuid = Guid.Parse("34156471-5432-436d-ab5d-76d07f3f9b32");

            // Restore Note Verbale
            public readonly static int Restore = 4;
            public readonly static Guid RestoreGuid = Guid.Parse("b8b59b62-7d84-44d9-8e7e-38019ce0b73e");

            // Update Note Verbale
            public readonly static int Update = 5;
            public readonly static Guid UpdateGuid = Guid.Parse("0ef47ec3-48de-4d64-9b95-95e2fe475a4e");

        }
        public static class Notifications
        {
            // Update Notifications
            public readonly static int Update = 103;
            public readonly static Guid UpdateGuid = Guid.Parse("b2257097-5d9a-4296-96df-17fed1bdf0e5");

            // Delete Notifications
            public readonly static int Delete = 105;
            public readonly static Guid DeleteGuid = Guid.Parse("0aa1a1b9-8f96-45b7-99f5-26ccb0e32155");

            // Create Notifications
            public readonly static int Create = 107;
            public readonly static Guid CreateGuid = Guid.Parse("ae875297-6124-4a93-9138-3faadcb01267");

            // Restore Notifications
            public readonly static int Restore = 108;
            public readonly static Guid RestoreGuid = Guid.Parse("e4e802c9-4e4f-4f5b-b619-5566a08c0558");

            // Access Notifications
            public readonly static int Access = 113;
            public readonly static Guid AccessGuid = Guid.Parse("d5f14cd4-8fa5-444b-8276-93c6dfdedd32");

        }
        public static class Offices
        {
            // Access Offices Codes
            public readonly static int Access = 51;
            public readonly static Guid AccessGuid = Guid.Parse("c4e6d1c1-adf7-4ac1-84ce-a502c4711af2");

            // Create Offices Codes
            public readonly static int Create = 52;
            public readonly static Guid CreateGuid = Guid.Parse("6bc4d96e-4f74-476e-9c05-7557a0fe3a91");

            // Update Offices Codes
            public readonly static int Update = 53;
            public readonly static Guid UpdateGuid = Guid.Parse("5507508d-9349-40d3-bd1c-e6d578dff5e8");

            // Delete Offices Codes
            public readonly static int Delete = 54;
            public readonly static Guid DeleteGuid = Guid.Parse("a448d830-9e2f-4f9f-9b3e-aaadda92a68d");

            // Restore Offices Codes
            public readonly static int Restore = 55;
            public readonly static Guid RestoreGuid = Guid.Parse("13d4b24f-7f40-462a-aa5c-7b3bf3d04627");

        }
        public static class OfficesLanguages
        {
            // Create Offices Codes Languauges
            public readonly static Guid CreateGuid = Guid.Parse("f5bd8419-750e-41e9-9a91-e90f06918597");

            // Update Offices Codes Languauges
            public readonly static Guid UpdateGuid = Guid.Parse("e4e7023a-f2fb-4124-a271-f7eda2caf620");

            // Restore Offices Codes Languauges
            public readonly static Guid RestoreGuid = Guid.Parse("af58ee72-4daa-454d-810f-9a97136415f2");

            // Delete Offices Codes Languauges
            public readonly static Guid DeleteGuid = Guid.Parse("257f8354-a52d-48d0-99da-933c4188bf9d");

        }
        public static class Oid
        {
            // Access Oid
            public readonly static int Access = 6;
            public readonly static Guid AccessGuid = Guid.Parse("f2559577-6567-4275-a49b-5d1e9a6211d1");

            // Create Oid
            public readonly static int Create = 7;
            public readonly static Guid CreateGuid = Guid.Parse("512ac8e1-257d-483d-be46-74bd14c2152b");

            // Delete Oid
            public readonly static int Delete = 8;
            public readonly static Guid DeleteGuid = Guid.Parse("c01342a0-b576-4140-8783-cf4047b2b266");

            // Restore Oid
            public readonly static int Restore = 9;
            public readonly static Guid RestoreGuid = Guid.Parse("12477dad-afda-42fd-8da8-3a3888f5ef8f");

            // Update Oid
            public readonly static int Update = 10;
            public readonly static Guid UpdateGuid = Guid.Parse("0276417a-5c9f-4f97-b2ff-67b9c61decb5");

        }
        public static class Operations
        {
            // Access Operations Codes
            public readonly static int Access = 56;
            public readonly static Guid AccessGuid = Guid.Parse("b200a3dc-2471-4729-9524-a31609a75ca6");

            // Create Operations Codes
            public readonly static int Create = 57;
            public readonly static Guid CreateGuid = Guid.Parse("cb86f0a1-3cf8-4322-85da-98646ec92ed7");

            // Update Operations Codes
            public readonly static int Update = 58;
            public readonly static Guid UpdateGuid = Guid.Parse("6a40e3ef-51b0-4966-a698-b1229ec4bff5");

            // Delete Operations Codes
            public readonly static int Delete = 59;
            public readonly static Guid DeleteGuid = Guid.Parse("4b3e5d71-1a28-4261-9f06-7fc0a5ad06d7");

            // Restore Operations Codes
            public readonly static int Restore = 60;
            public readonly static Guid RestoreGuid = Guid.Parse("d966d797-d771-4d83-b906-8f186af90994");

        }
        public static class OperationsLanguages
        {
            // Update Operations Codes Languages
            public readonly static Guid UpdateGuid = Guid.Parse("849d1b3e-f0e6-4c45-bb1b-7a6de8fd6384");

            // Restore Operations Codes Languages
            public readonly static Guid RestoreGuid = Guid.Parse("2ab35bf6-6c4f-419d-9b87-9b100423ed06");

            // Create Operations Codes Languages
            public readonly static Guid CreateGuid = Guid.Parse("132673e0-25b2-410b-a6cb-3bb04cd7a0d6");

            // Delete Operations Codes Languages
            public readonly static Guid DeleteGuid = Guid.Parse("8c44c891-32fa-446e-801d-5200c6e4b068");

        }
        public static class OrganizationalExperience
        {
            // Remove Organizational Experience
            public readonly static Guid RemoveGuid = Guid.Parse("abc36eaf-d0fc-4b78-a8d0-499ef1d6cb1e");

            // Update Organizational Experience
            public readonly static Guid UpdateGuid = Guid.Parse("76b8fa71-d946-4790-8fc2-1b6245194682");

            // Create Organizational Experience
            public readonly static Guid CreateGuid = Guid.Parse("51a95136-f50d-43d2-b18c-a673ed7c9b74");

        }
        public static class Organizations
        {
            // Access Organizations Codes
            public readonly static int Access = 66;
            public readonly static Guid AccessGuid = Guid.Parse("8cf50fbc-3017-4453-90c9-26a13cc14dfe");

            // Create Organizations Codes
            public readonly static int Create = 67;
            public readonly static Guid CreateGuid = Guid.Parse("4f25c941-0811-42a6-9631-6d1d41815419");

            // Update Organizations Codes
            public readonly static int Update = 68;
            public readonly static Guid UpdateGuid = Guid.Parse("b84cf162-2ac7-4581-92bb-69bb446f82bf");

            // Delete Organizations Codes
            public readonly static int Delete = 69;
            public readonly static Guid DeleteGuid = Guid.Parse("6c350c14-02c4-4209-88d8-38998b7eca05");

            // Restore Organizations Codes
            public readonly static int Restore = 70;
            public readonly static Guid RestoreGuid = Guid.Parse("9b29bb70-1dc6-4a23-abc7-91c7257c7972");

        }
        public static class OrganizationsInstances
        {
            // Access Organization Instances Codes
            public readonly static int Access = 61;
            public readonly static Guid AccessGuid = Guid.Parse("07a78f96-95a7-4eb9-89f6-6c1e1bbe615c");

            // Create Organization Instances Codes
            public readonly static int Create = 62;
            public readonly static Guid CreateGuid = Guid.Parse("0c9213b7-e14d-40da-8b4d-d995232091b5");

            // Update Organization Instances Codes
            public readonly static int Update = 63;
            public readonly static Guid UpdateGuid = Guid.Parse("d2ee8a8d-a29e-4b58-aa2c-e266aaa52c75");

            // Delete Organization Instances Codes
            public readonly static int Delete = 64;
            public readonly static Guid DeleteGuid = Guid.Parse("0f0216be-a5e6-44ed-a54d-31d34b903641");

            // Restore Organization Instances Codes
            public readonly static int Restore = 65;
            public readonly static Guid RestoreGuid = Guid.Parse("aff2a5e7-b7c1-45b9-bcf4-595ce68a6304");

        }
        public static class OrganizationsInstancesConfigurations
        {
            // Access Organizations Instances Configurations
            public readonly static int Access = 87;
            public readonly static Guid AccessGuid = Guid.Parse("0083baff-b460-4f97-a3bd-75ea8273755e");

            // Create Organizations Instances Configurations
            public readonly static int Create = 88;
            public readonly static Guid CreateGuid = Guid.Parse("ee53794a-5dd0-4ed5-8958-c5ea543dccc1");

            // Update Organizations Instances Configurations
            public readonly static int Update = 89;
            public readonly static Guid UpdateGuid = Guid.Parse("6bab0bf3-b1fa-4534-9c7d-867092ef1ece");

            // Remove Organizations Instances Configurations
            public readonly static int Remove = 90;
            public readonly static Guid RemoveGuid = Guid.Parse("dd85d3ea-aca1-4718-abd6-fec611e7b6d0");

        }
        public static class OrganizationsInstancesLanguages
        {
            // Restore Organization Instances Codes Languages
            public readonly static Guid RestoreGuid = Guid.Parse("67a4e818-1499-43c2-b240-c78ddf2f3737");

            // Update Organization Instances Codes Languages
            public readonly static Guid UpdateGuid = Guid.Parse("8053c6d0-d6b4-4f94-bb23-bc6ee264b19e");

            // Create Organization Instances Codes Languages
            public readonly static Guid CreateGuid = Guid.Parse("bca316e4-d740-4a14-950b-29c5f4aadd37");

            // Delete Organization Instances Codes Languages
            public readonly static Guid DeleteGuid = Guid.Parse("878a1a28-bb9c-44f2-9edd-20185024a69a");

        }
        public static class OrganizationsLanguages
        {
            // Update Organizations Codes Languages
            public readonly static Guid UpdateGuid = Guid.Parse("e5ec40ce-b946-4355-8c33-1a10c0314eb5");

            // Create Organizations Codes Languages
            public readonly static Guid CreateGuid = Guid.Parse("cf386188-b429-4e31-a7ef-0202dd7616ee");

            // Delete Organizations Codes Languages
            public readonly static Guid DeleteGuid = Guid.Parse("905bf9ac-5e69-45a7-b4a2-6a0717773695");

            // Restore Organizations Codes Languages
            public readonly static Guid RestoreGuid = Guid.Parse("8f48cb8d-14ac-4e44-8a03-e05fd9ccdee4");

        }
        public static class PartnersContribution
        {
            // Access Partners Contribution
            public readonly static int Access = 6;
            public readonly static Guid AccessGuid = Guid.Parse("29d8b9bc-d7ed-451d-b912-ceea69c867ad");

            // Create Partners Contribution
            public readonly static int Create = 7;
            public readonly static Guid CreateGuid = Guid.Parse("459d85af-7bbf-4055-92d1-4fb5c5819ead");

            // Upload Partners Contribution
            public readonly static int Upload = 8;
            public readonly static Guid UploadGuid = Guid.Parse("26f15b14-81bc-4b24-b08e-1bae76dea8e6");

            // Update Partners Contribution
            public readonly static int Update = 9;
            public readonly static Guid UpdateGuid = Guid.Parse("95d195b5-8aac-432a-9e74-4f0c668823b6");

            // Delete Partners Contribution
            public readonly static int Delete = 10;
            public readonly static Guid DeleteGuid = Guid.Parse("e5f0d192-5f29-4270-a63d-0bffadf0e197");

            // Restore Partners Contribution
            public readonly static int Restore = 11;
            public readonly static Guid RestoreGuid = Guid.Parse("9467fb76-f9ef-40a2-8f9c-24ff939f8dd3");

            // ValidateData Partners Contribution
            public readonly static int ValidateData = 12;
            public readonly static Guid ValidateDataGuid = Guid.Parse("e9b3cd28-2adc-405b-b26e-bc91ffd65e83");

        }
        public static class PermissionsManagement
        {
            // Revoke Permissions
            public readonly static Guid RevokeGuid = Guid.Parse("28fdf3ed-8135-4719-806b-5b1464452b08");

            // Grant Permissions
            public readonly static Guid GrantGuid = Guid.Parse("0c3e459d-1d70-4ac0-a845-1ee0f8f4e156");

            // Manage Permissions
            public readonly static int Manage = 91;
            public readonly static Guid ManageGuid = Guid.Parse("7d99c7ce-7066-4077-9097-39ad4ca827dd");

            // Access Permissions
            public readonly static int Access = 111;
            public readonly static Guid AccessGuid = Guid.Parse("593b910a-9f7c-422d-b738-706c80c70420");

        }
        public static class PersonalDetails
        {
            // Upload Profile Photo
            public readonly static Guid UploadGuid = Guid.Parse("f3b59045-babc-4a34-a6aa-7af118162e54");

            // Update Personal Information
            public readonly static Guid UpdateGuid = Guid.Parse("8dd1f8de-b631-4fe8-8cea-8f06652e4346");

            // Remove Profile Photop
            public readonly static Guid RemoveGuid = Guid.Parse("1f4b4449-8ee6-4afe-a695-4028b84b4270");

        }
        public static class PersonalDetailsLanguage
        {
            // Restore Personal Information Languages
            public readonly static Guid RestoreGuid = Guid.Parse("97137ee9-d06d-4b24-bc48-5e235b48affd");

            // Update Personal Information Languages
            public readonly static Guid UpdateGuid = Guid.Parse("edd143ab-811e-4ba4-8a23-16296ece1afe");

            // Delete Personal Information Languages
            public readonly static Guid DeleteGuid = Guid.Parse("accb2722-ac87-4a15-b870-c31003f85ffe");

            // Create Personal Information Languages
            public readonly static Guid CreateGuid = Guid.Parse("4e94ce20-0a2c-47cc-87e7-ef59d2dee663");

        }
        public static class PPAManagement
        {
            // Access Project Partnership Agreement
            public readonly static int Access = 1;
            public readonly static Guid AccessGuid = Guid.Parse("fa7f51a6-ff86-4ab4-a8db-3d70d8f5a8df");

            // Create Project Partnership Agreement
            public readonly static int Create = 2;
            public readonly static Guid CreateGuid = Guid.Parse("b99477c9-48bd-48e3-8681-9b82c21c28ee");

            // Update Project Partnership Agreement
            public readonly static int Update = 3;
            public readonly static Guid UpdateGuid = Guid.Parse("7a007606-39fd-4c84-958b-bf8b02b9c2cc");

            // Delete Project Partnership Agreement
            public readonly static int Delete = 4;
            public readonly static Guid DeleteGuid = Guid.Parse("e74892fc-3621-4fc6-8bd3-98fb21b9bf08");

            // Restore Project Partnership Agreement
            public readonly static int Restore = 5;
            public readonly static Guid RestoreGuid = Guid.Parse("b77f61ab-fceb-4ed1-804e-2f972b80b679");

            // FullReadWrite Project Partnership Agreement
            public readonly static int FullReadWrite = 6;
            public readonly static Guid FullReadWriteGuid = Guid.Parse("aab196d3-d68d-47fc-86a6-863b4521c00f");

            // FullAccess Project Partnership Agreement
            public readonly static int FullAccess = 7;
            public readonly static Guid FullAccessGuid = Guid.Parse("de046f8f-2a57-4d4c-99c5-ded198928d5a");

        }
        public static class PrintersConfiguration
        {
            // Access Printers Configuration
            public readonly static int Access = 1;
            public readonly static Guid AccessGuid = Guid.Parse("f0b7a0ff-5f26-40a0-9f12-5799e60025d5");

            // Create Printers Configuration
            public readonly static int Create = 2;
            public readonly static Guid CreateGuid = Guid.Parse("6ac68aa0-22af-4db3-8fe7-6c6109c2594b");

            // Delete Printers Configuration
            public readonly static int Delete = 3;
            public readonly static Guid DeleteGuid = Guid.Parse("f58bfa75-70a1-4f0c-9ae9-991c43288239");

            // Restore Printers Configuration
            public readonly static int Restore = 4;
            public readonly static Guid RestoreGuid = Guid.Parse("11298cdb-7000-41ec-9229-509d62ae85fb");

            // Update Printers Configuration
            public readonly static int Update = 5;
            public readonly static Guid UpdateGuid = Guid.Parse("d91e2788-008e-4403-a09c-c38f75dc9404");

        }
        public static class PublishReports
        {
            // Access Publish Reports
            public readonly static int Access = 12;
            public readonly static Guid AccessGuid = Guid.Parse("0f46a614-90cf-4877-8baf-4674888ecb3f");

        }
        public static class ReferralConfigManagement
        {
            // Access Referral
            public readonly static int Access = 1;
            public readonly static Guid AccessGuid = Guid.Parse("34a198c0-63e4-4a01-9f2c-a7ded3cc7eae");

            // Create Referral
            public readonly static int Create = 2;
            public readonly static Guid CreateGuid = Guid.Parse("d4366712-446b-413b-8bb5-67113dc885d3");

            // Delete Referral
            public readonly static int Delete = 3;
            public readonly static Guid DeleteGuid = Guid.Parse("761d8624-2264-4e25-96c9-8b79c03871d3");

            // Restore Referral
            public readonly static int Restore = 4;
            public readonly static Guid RestoreGuid = Guid.Parse("2befb293-04db-4621-a08e-d6dbadd177f9");

            // Update Referral
            public readonly static int Update = 5;
            public readonly static Guid UpdateGuid = Guid.Parse("07006981-2300-4563-b573-cdf6de202a44");

        }
        public static class ReferralHistory
        {
            // Access Referral History
            public readonly static int Access = 31;
            public readonly static Guid AccessGuid = Guid.Parse("278d1ab4-280a-4c58-9837-e153f1e5489b");

            // Create Referral History
            public readonly static int Create = 32;
            public readonly static Guid CreateGuid = Guid.Parse("d26bd7ed-74dc-4e1b-98bc-5efd4e00f741");

            // Delete Referral History
            public readonly static int Delete = 33;
            public readonly static Guid DeleteGuid = Guid.Parse("649c033c-1621-4c69-a772-813dd6366807");

            // Restore Referral History
            public readonly static int Restore = 34;
            public readonly static Guid RestoreGuid = Guid.Parse("792f2a2e-6b8d-416d-b2a0-542a875dcb2d");

            // Update Referral History
            public readonly static int Update = 35;
            public readonly static Guid UpdateGuid = Guid.Parse("90df404b-18d8-4782-ba6d-18fbf6be3c9b");

        }
        public static class ReferralNotification
        {
            // Access Referral Notification
            public readonly static int Access = 16;
            public readonly static Guid AccessGuid = Guid.Parse("cb0a0fa2-0831-4004-a5e9-a729e5cc737c");

            // Create Referral Notification
            public readonly static int Create = 17;
            public readonly static Guid CreateGuid = Guid.Parse("6d37e2eb-f754-4179-8024-a93efcdee9b7");

            // Delete Referral Notification
            public readonly static int Delete = 18;
            public readonly static Guid DeleteGuid = Guid.Parse("c6d5d545-6202-4593-b0e8-31d31d38248a");

            // Restore Referral Notification
            public readonly static int Restore = 19;
            public readonly static Guid RestoreGuid = Guid.Parse("c2b6f9f7-8aff-47c9-8f9d-9eafd673c6d1");

            // Update Referral Notification
            public readonly static int Update = 20;
            public readonly static Guid UpdateGuid = Guid.Parse("2c72278a-757d-47e5-916d-9c3032b973a9");

        }
        public static class ReferralStatus
        {
            // Access Referral Status
            public readonly static int Access = 11;
            public readonly static Guid AccessGuid = Guid.Parse("f7f4c1e0-c9b4-40da-abc0-eddc89113341");

            // Create Referral Status
            public readonly static int Create = 12;
            public readonly static Guid CreateGuid = Guid.Parse("3dcea6b6-f711-4b6e-ac89-ae8d4de7c50a");

            // Delete Referral Status
            public readonly static int Delete = 13;
            public readonly static Guid DeleteGuid = Guid.Parse("128ff5ed-7a88-4383-8937-f1e0e45d4fe7");

            // Restore Referral Status
            public readonly static int Restore = 14;
            public readonly static Guid RestoreGuid = Guid.Parse("5f664c70-7726-4b28-bd18-88ac2cee4279");

            // Update Referral Status
            public readonly static int Update = 15;
            public readonly static Guid UpdateGuid = Guid.Parse("f4b87c87-fcc5-4ec7-8401-e261940a1007");

        }
        public static class ReferralStep
        {
            // Access Referral Step
            public readonly static int Access = 6;
            public readonly static Guid AccessGuid = Guid.Parse("7da8d914-6952-443a-b4b2-4120f494b199");

            // Create Referral Step
            public readonly static int Create = 7;
            public readonly static Guid CreateGuid = Guid.Parse("4a4822ba-b8a2-4bbd-abc9-7912168be9af");

            // Delete Referral Step
            public readonly static int Delete = 8;
            public readonly static Guid DeleteGuid = Guid.Parse("57e9ffcf-30db-44a5-a942-ad7b5ead234b");

            // Restore Referral Step
            public readonly static int Restore = 9;
            public readonly static Guid RestoreGuid = Guid.Parse("40cb78e0-e8cd-43eb-8367-c721e43e5b39");

            // Update Referral Step
            public readonly static int Update = 10;
            public readonly static Guid UpdateGuid = Guid.Parse("2509311a-98f0-4bac-a767-491790cee68a");

        }
        public static class ReferralStepUser
        {
            // Access Referral Step User
            public readonly static int Access = 26;
            public readonly static Guid AccessGuid = Guid.Parse("b60867b0-5842-4a6b-befa-f48970927f3e");

            // Create Referral Step User
            public readonly static int Create = 27;
            public readonly static Guid CreateGuid = Guid.Parse("470f9b5a-744d-47ce-b908-a66af5463e12");

            // Delete Referral Step User
            public readonly static int Delete = 28;
            public readonly static Guid DeleteGuid = Guid.Parse("c45b104c-8973-4932-ba14-8906fc03cc9f");

            // Restore Referral Step User
            public readonly static int Restore = 29;
            public readonly static Guid RestoreGuid = Guid.Parse("a3b64490-59ef-4736-8c4e-621d46760e78");

            // Update Referral Step User
            public readonly static int Update = 30;
            public readonly static Guid UpdateGuid = Guid.Parse("c067cafb-42cc-499c-ba8f-aaf0d425f797");

        }
        public static class RefugeeScannedDocument
        {
            // Access Refugee Scanned Document
            public readonly static int Access = 1;
            public readonly static Guid AccessGuid = Guid.Parse("33d5b523-5260-43ad-b4e3-4d5566f4bee9");

            // Create Refugee Scanned Document
            public readonly static int Create = 2;
            public readonly static Guid CreateGuid = Guid.Parse("f2b67e42-e676-439b-ba80-75e48ed8951e");

            // Delete Refugee Scanned Document
            public readonly static int Delete = 3;
            public readonly static Guid DeleteGuid = Guid.Parse("7eeb38da-e4ab-4835-bf1e-75caec276825");

            // Restore Refugee Scanned Document
            public readonly static int Restore = 4;
            public readonly static Guid RestoreGuid = Guid.Parse("d368dbc9-60fc-4908-a7f4-2a6e14cdaf13");

            // Update Refugee Scanned Document
            public readonly static int Update = 5;
            public readonly static Guid UpdateGuid = Guid.Parse("cbc658e7-0639-44d6-9c67-5e259fd816d2");

            // ValidateData Refugee Scanned Document
            public readonly static int ValidateData = 12;
            public readonly static Guid ValidateDataGuid = Guid.Parse("9d9606c2-9a21-4c73-968c-024d7fc0027f");

            // Print Refugee Scanned Document
            public readonly static int Print = 13;
            public readonly static Guid PrintGuid = Guid.Parse("c871e460-f2d9-4073-8cbf-5e08f312a2e5");

            // Download Refugee Scanned Document
            public readonly static int Download = 14;
            public readonly static Guid DownloadGuid = Guid.Parse("c82fbe66-c103-47ff-8cff-cc1fdcad1e40");
        }
        public static class ReportDocuments
        {
            // Access Report Documents
            public readonly static int Access = 1;
            public readonly static Guid AccessGuid = Guid.Parse("f601f427-a0de-4131-b9e6-6f78cdb481cd");

            // Create Report Documents
            public readonly static int Create = 2;
            public readonly static Guid CreateGuid = Guid.Parse("714b9061-4b53-4feb-8728-26db76913ef4");

            // Update Report Documents
            public readonly static int Update = 3;
            public readonly static Guid UpdateGuid = Guid.Parse("c16f7b0e-40ff-45ba-a106-b4b5aa297dca");

            // Delete Report Documents
            public readonly static int Delete = 4;
            public readonly static Guid DeleteGuid = Guid.Parse("fd6810a9-dfbc-483e-917b-847cc677b42b");

            // Restore Report Documents
            public readonly static int Restore = 5;
            public readonly static Guid RestoreGuid = Guid.Parse("c9c24634-d5d5-4bdb-8373-34d3fce288ed");

        }
        public static class SearchTool
        {
            // Access Search Tool
            public readonly static int Access = 1;
            public readonly static Guid AccessGuid = Guid.Parse("ded5d841-babe-40db-a495-6bd33ae4a17a");

        }
        public static class Serviceenhancementrequest
        {
            // Update Service enhancement request
            public readonly static int Update = 5;
            public readonly static Guid UpdateGuid = Guid.Parse("91e03297-f776-41df-996f-51b5c8902345");

            // Delete Service enhancement request
            public readonly static int Delete = 6;
            public readonly static Guid DeleteGuid = Guid.Parse("637a2651-556f-45aa-b329-036227f5a3a3");

            // Restore Service enhancement request
            public readonly static int Restore = 7;
            public readonly static Guid RestoreGuid = Guid.Parse("846ec63a-7c09-4c34-9de9-5ac22dcf12d0");

        }
        public static class Shuttle
        {
            // Access Shuttle
            public readonly static int Access = 1;
            public readonly static Guid AccessGuid = Guid.Parse("2f1bb65b-ed02-4cca-ae93-477bb8c329f2");

            // Create Shuttle
            public readonly static int Create = 2;
            public readonly static Guid CreateGuid = Guid.Parse("410ab159-eeed-43c8-870f-13a5a431fd88");

            // Delete Shuttle
            public readonly static int Delete = 3;
            public readonly static Guid DeleteGuid = Guid.Parse("45c38325-18aa-4dc0-a7f4-9175313a0033");

            // Restore Shuttle
            public readonly static int Restore = 4;
            public readonly static Guid RestoreGuid = Guid.Parse("871c03bb-fef1-4cf1-b967-108505af779c");

            // Update Shuttle
            public readonly static int Update = 5;
            public readonly static Guid UpdateGuid = Guid.Parse("ec0a6640-c05f-41c3-a8b8-d39790d55acb");

            // Send Shuttle
            public readonly static int Send = 31;
            public readonly static Guid SendGuid = Guid.Parse("f16b1163-3c30-495f-b8e8-4e0a5f0f64e8");

        }
        public static class ShuttleRequest
        {
            // Delete Shuttle Request
            public readonly static Guid DeleteGuid = Guid.Parse("88e82071-0d74-44f0-bec4-8edd18a498a2");

            // Restore Shuttle Request
            public readonly static Guid RestoreGuid = Guid.Parse("45cf3192-7d8c-4e03-9ac5-1f1b89a374c6");

        }
        public static class ShuttleRoute
        {
            // Access Shuttle Route
            public readonly static int Access = 11;
            public readonly static Guid AccessGuid = Guid.Parse("867b7f2c-ce01-41c6-8051-e1e7f4aef1ec");

            // Create Shuttle Route
            public readonly static int Create = 12;
            public readonly static Guid CreateGuid = Guid.Parse("8b94426b-78f5-480d-8725-6f6040b2c3e0");

            // Delete Shuttle Route
            public readonly static int Delete = 13;
            public readonly static Guid DeleteGuid = Guid.Parse("4c9dde7b-d4c8-4bd0-9d32-116ac77c5a48");

            // Restore Shuttle Route
            public readonly static int Restore = 14;
            public readonly static Guid RestoreGuid = Guid.Parse("79627269-b2dd-408b-acab-87b722498c07");

            // Update Shuttle Route
            public readonly static int Update = 15;
            public readonly static Guid UpdateGuid = Guid.Parse("4e9ab4d0-d9bb-4e98-8b75-df7f63b95749");

        }
        public static class ShuttleStaff
        {
            // Access Shuttle Staff
            public readonly static int Access = 6;
            public readonly static Guid AccessGuid = Guid.Parse("da288609-e842-41ca-853b-d04e5a318771");

            // Create Shuttle Staff
            public readonly static int Create = 7;
            public readonly static Guid CreateGuid = Guid.Parse("67e7ef2b-d0da-486e-a236-b6015a134bd7");

            // Delete Shuttle Staff
            public readonly static int Delete = 8;
            public readonly static Guid DeleteGuid = Guid.Parse("0ba33072-4041-4659-be74-73bbf916db60");

            // Restore Shuttle Staff
            public readonly static int Restore = 9;
            public readonly static Guid RestoreGuid = Guid.Parse("444814d9-3a4c-4c0b-9500-2ed785f0e494");

            // Update Shuttle Staff
            public readonly static int Update = 10;
            public readonly static Guid UpdateGuid = Guid.Parse("7ebf0091-4a01-4485-b3c9-972ef506c75e");

        }
        public static class ShuttleTravelPurpose
        {
            // Access Shuttle Travel Purpose
            public readonly static int Access = 26;
            public readonly static Guid AccessGuid = Guid.Parse("e0b4e280-11a4-4f9e-830e-36b02c3db6c2");

            // Create Shuttle Travel Purpose
            public readonly static int Create = 27;
            public readonly static Guid CreateGuid = Guid.Parse("f43baab7-cc37-4c1c-ba5f-6851139609b0");

            // Delete Shuttle Travel Purpose
            public readonly static int Delete = 28;
            public readonly static Guid DeleteGuid = Guid.Parse("ef6d5029-81e4-4389-bb73-07446498c439");

            // Restore Shuttle Travel Purpose
            public readonly static int Restore = 29;
            public readonly static Guid RestoreGuid = Guid.Parse("2a970a99-f111-4e87-aa8d-0353f78cd565");

            // Update Shuttle Travel Purpose
            public readonly static int Update = 30;
            public readonly static Guid UpdateGuid = Guid.Parse("9db56914-f872-4541-9c35-48333016454c");

        }
        public static class ShuttleVehicle
        {
            // Access Shuttle Vehicle
            public readonly static int Access = 16;
            public readonly static Guid AccessGuid = Guid.Parse("dc401d16-76c6-4cc5-8eb9-33c75eb6b4cd");

            // Create Shuttle Vehicle
            public readonly static int Create = 17;
            public readonly static Guid CreateGuid = Guid.Parse("f6fec38c-3d60-4b2e-8ac8-ba7960fb6172");

            // Delete Shuttle Vehicle
            public readonly static int Delete = 18;
            public readonly static Guid DeleteGuid = Guid.Parse("ad5b24b4-2464-4bff-b77d-795c0b195081");

            // Restore Shuttle Vehicle
            public readonly static int Restore = 19;
            public readonly static Guid RestoreGuid = Guid.Parse("19b55f52-2117-4072-9c0d-d78b8066cecc");

            // Update Shuttle Vehicle
            public readonly static int Update = 20;
            public readonly static Guid UpdateGuid = Guid.Parse("fd9a77df-9782-473b-a7ce-bb5ff366f78c");

        }
        public static class Sitemap
        {
            // Access Sitemap Codes
            public readonly static int Access = 71;
            public readonly static Guid AccessGuid = Guid.Parse("2eacccb6-cd66-42ab-8820-2abf2da65cb1");

            // Create Sitemap Codes
            public readonly static int Create = 72;
            public readonly static Guid CreateGuid = Guid.Parse("9ff4156d-4943-43f4-97aa-14e68549c549");

            // Update Sitemap Codes
            public readonly static int Update = 73;
            public readonly static Guid UpdateGuid = Guid.Parse("28bb96ed-4ee3-462a-96b1-2762127e4a45");

            // Delete Sitemap Codes
            public readonly static int Delete = 74;
            public readonly static Guid DeleteGuid = Guid.Parse("1a776e53-647e-4821-ad65-f86f09167e51");

            // Restore Sitemap Codes
            public readonly static int Restore = 75;
            public readonly static Guid RestoreGuid = Guid.Parse("2fadd30a-e648-4458-b145-4325d7dae78e");

        }
        public static class SitemapLanguages
        {
            // Create Sitemap Codes Languages
            public readonly static Guid CreateGuid = Guid.Parse("e5a70942-4b7b-4d8f-af07-f228e48ff023");

            // Delete Sitemap Codes Languages
            public readonly static Guid DeleteGuid = Guid.Parse("42583b83-c4a6-4b2d-9188-f0f84b891bbf");

            // Update Sitemap Codes Languages
            public readonly static Guid UpdateGuid = Guid.Parse("4ba1ff95-5cee-4581-b7a1-a949d6053979");

            // Restore Sitemap Codes Languages
            public readonly static Guid RestoreGuid = Guid.Parse("c4d2d9d7-63f9-4034-a45f-8267c062b774");

        }
        public static class StaffPhones
        {
            // Access Staff Phones
            public readonly static int Access = 6;
            public readonly static Guid AccessGuid = Guid.Parse("3c40d870-8078-4f2f-8294-8d5f256775aa");

            // Create Staff Phones
            public readonly static int Create = 7;
            public readonly static Guid CreateGuid = Guid.Parse("c6fdff8a-827a-4256-a6c5-cebecd26e771");

            // Update Staff Phones
            public readonly static int Update = 8;
            public readonly static Guid UpdateGuid = Guid.Parse("2c60d276-5107-4d5c-be9a-3d1e68d795bf");

            // Delete Staff Phones
            public readonly static int Delete = 9;
            public readonly static Guid DeleteGuid = Guid.Parse("52f6699c-53e7-4a1b-978a-794d1997dc71");

            // Restore Staff Phones
            public readonly static int Restore = 10;
            public readonly static Guid RestoreGuid = Guid.Parse("f8fa23ed-4963-45f6-b0ab-a7e021f0435d");

        }
        public static class StaffProfile
        {
            // Access Staff Profile
            public readonly static int Access = 1;
            public readonly static Guid AccessGuid = Guid.Parse("1b9200b6-60b6-4cdf-8ab8-0d3dfb94821b");

            // Create Staff Profile
            public readonly static int Create = 2;
            public readonly static Guid CreateGuid = Guid.Parse("78a66b19-068a-4942-a650-62a2d8d9c541");

            // Update Staff Profile
            public readonly static int Update = 3;
            public readonly static Guid UpdateGuid = Guid.Parse("817f8bb4-735a-4f91-a2b9-41d1c8f443b1");

            // Delete Staff Profile
            public readonly static int Delete = 4;
            public readonly static Guid DeleteGuid = Guid.Parse("b847b90c-1f2e-4047-b88f-2f1039551406");

            // Restore Staff Profile
            public readonly static int Restore = 5;
            public readonly static Guid RestoreGuid = Guid.Parse("0444b9d3-4b97-47f3-997a-5cc03b228445");

        }
        public static class MissionRequestAdminApproval
        {
            // Access Mission Request Admin Approval
            public readonly static int Access = 96;
            public readonly static Guid AccessGuid = Guid.Parse("83e988ee-a280-4f77-b606-60761b914fbc");

            // Create Mission Request Admin Approval
            public readonly static int Create = 97;
            public readonly static Guid CreateGuid = Guid.Parse("050b7eab-351f-46c6-bf7e-09620b7d1900");

            // Confirm Mission Request Admin Approval
            public readonly static int Confirm = 98;
            public readonly static Guid ConfirmGuid = Guid.Parse("dceb06ff-4844-4084-917b-8dfae457b9ec");

            // Delete Mission Request Admin Approval
            public readonly static int Delete = 99;
            public readonly static Guid DeleteGuid = Guid.Parse("238f5796-098e-4a7a-9674-ddd69e96f8b2");

            // Update Mission Request Admin Approval
            public readonly static int Update = 100;
            public readonly static Guid UpdateGuid = Guid.Parse("67c9af11-8862-4587-b5f3-3bb594c3a45f");

            // ValidateData Mission Request Admin Approval
            public readonly static int ValidateData = 101;
            public readonly static Guid ValidateDataGuid = Guid.Parse("0a97e12b-f8ef-4427-96d0-8d80c83e1b6d");

        }

        public static class StaffMissionRequest
        {
            // Access Staff Mission Request
            public readonly static int Access = 49;
            public readonly static Guid AccessGuid = Guid.Parse("a19d697e-5985-4094-bb9e-345c31ba4f5e");

            // Create Staff Mission Request
            public readonly static int Create = 50;
            public readonly static Guid CreateGuid = Guid.Parse("1f9b7910-d575-4754-a851-90b96758f875");

            // Delete Staff Mission Request
            public readonly static int Delete = 51;
            public readonly static Guid DeleteGuid = Guid.Parse("1f9ca2eb-3b5c-4aa9-a0f0-da0156734eb7");

            // Restore Staff Mission Request
            public readonly static int Restore = 52;
            public readonly static Guid RestoreGuid = Guid.Parse("9e38190a-f431-42cb-9e1d-b267d524c21c");

            // Update Staff Mission Request
            public readonly static int Update = 53;
            public readonly static Guid UpdateGuid = Guid.Parse("9ac85f61-9db3-4461-8a58-5e82471740c3");

        }

        #region Profile Staff section 

        public static class StaffProfileAdminSection
        {
            // Access Staff Profile Admin Section
            public readonly static int Access = 11;
            public readonly static Guid AccessGuid = Guid.Parse("4edd5dbb-3348-4c58-b246-0cfb46d8b207");

            // Update Staff Profile Admin Section
            public readonly static int Update = 12;
            public readonly static Guid UpdateGuid = Guid.Parse("8316602a-46f4-4997-9a8e-4888fd587dc0");

        }
        public static class StaffProfileFinanceSection
        {
            // Access Staff Profile Finance Section
            public readonly static int Access = 13;
            public readonly static Guid AccessGuid = Guid.Parse("f8c4523b-ff7f-4ff4-b445-490a48e29920");

            // Update Staff Profile Finance Section
            public readonly static int Update = 14;
            public readonly static Guid UpdateGuid = Guid.Parse("78a7054e-2bb0-4dae-be6d-684e79906f11");

        }
        public static class StaffProfileSecuritySection
        {
            // Access Staff Profile Security Section
            public readonly static int Access = 15;
            public readonly static Guid AccessGuid = Guid.Parse("30cf6eb8-3690-4d78-ac3d-d5c9291db2af");

            // Update Staff Profile Security Section
            public readonly static int Update = 16;
            public readonly static Guid UpdateGuid = Guid.Parse("43d2b90c-8c05-4321-b298-b86d4cf32562");

        }
        public static class StaffProfileHRSection
        {
            // Access Staff Profile HR Section
            public readonly static int Access = 6;
            public readonly static Guid AccessGuid = Guid.Parse("788f6bcd-44ce-41b5-baed-f971e7816b93");

            // Create Staff Profile HR Section
            public readonly static int Create = 7;
            public readonly static Guid CreateGuid = Guid.Parse("fa151ff8-cec9-4c8c-8ef4-514b6511eed1");

            // Update Staff Profile HR Section
            public readonly static int Update = 8;
            public readonly static Guid UpdateGuid = Guid.Parse("19198ab8-6430-4884-98df-d71e8d210980");

            // Delete Staff Profile HR Section
            public readonly static int Delete = 9;
            public readonly static Guid DeleteGuid = Guid.Parse("5ac8e8b9-b087-4a80-af09-6abb42f3deaa");

            // Restore Staff Profile HR Section
            public readonly static int Restore = 10;
            public readonly static Guid RestoreGuid = Guid.Parse("b473740d-715e-433b-84ed-0490501d02a9");

        }
        public static class StaffProfileICTSection
        {
            // Access Staff Profile ICT Section
            public readonly static int Access = 17;
            public readonly static Guid AccessGuid = Guid.Parse("6a32ebe4-1aaa-4169-8ec0-209c3774446c");

            // Update Staff Profile ICT Section
            public readonly static int Update = 18;
            public readonly static Guid UpdateGuid = Guid.Parse("d799b938-9f08-4768-96d6-e71292456d44");

        }



        #endregion

        #region Staff Check List

        public static class StaffSeparationChecklistManagement
        {
            // Access Staff Separation Checklist Management
            public readonly static int Access = 102;
            public readonly static Guid AccessGuid = Guid.Parse("5f49ec07-5732-41f3-a3cb-faef44e827a4");

            // Create Staff Separation Checklist Management
            public readonly static int Create = 103;
            public readonly static Guid CreateGuid = Guid.Parse("8c99c23f-e784-43cb-9af6-6e87806050e7");

            // Confirm Staff Separation Checklist Management
            public readonly static int Confirm = 104;
            public readonly static Guid ConfirmGuid = Guid.Parse("28a33048-3c2b-4543-9401-84151caccc90");

            // Delete Staff Separation Checklist Management
            public readonly static int Delete = 105;
            public readonly static Guid DeleteGuid = Guid.Parse("d19a7ea5-3b58-418b-9e8f-3ea29e3f7f4a");

            // Restore Staff Separation Checklist Management
            public readonly static int Restore = 106;
            public readonly static Guid RestoreGuid = Guid.Parse("f73c12ab-ce0b-4ac5-abad-cc0dcb6066b2");

            // Update Staff Separation Checklist Management
            public readonly static int Update = 107;
            public readonly static Guid UpdateGuid = Guid.Parse("e2547369-1357-45d8-88f0-662e9e1411e5");

            // ValidateData Staff Separation Checklist Management
            public readonly static int ValidateData = 108;
            public readonly static Guid ValidateDataGuid = Guid.Parse("400f28ab-18c1-4c87-937b-e5bc7a5cc1e9");

        }
        #endregion

        #region Salary  Module
        public static class SalaryCycle
        {
            // Access Salary Cycle
            public readonly static int Access = 60;
            public readonly static Guid AccessGuid = Guid.Parse("24f438b7-b941-4fe6-a6e9-e2c794c67ffd");

            // Create Salary Cycle
            public readonly static int Create = 61;
            public readonly static Guid CreateGuid = Guid.Parse("c86bdc0c-1264-441e-9b3e-986f37afd48c");

            // Delete Salary Cycle
            public readonly static int Delete = 62;
            public readonly static Guid DeleteGuid = Guid.Parse("bf4cfa34-e336-4d45-9a32-6733dc1a798d");

            // Restore Salary Cycle
            public readonly static int Restore = 63;
            public readonly static Guid RestoreGuid = Guid.Parse("db6b8baa-edf3-4a71-9e02-986b793f58a2");

            // Update Salary Cycle
            public readonly static int Update = 64;
            public readonly static Guid UpdateGuid = Guid.Parse("54e14f5c-39d3-4644-a3ea-e3bf597b77f2");

        }
        public static class SalaryInAdvance
        {
            // Access Salary In-Advance
            public readonly static int Access = 70;
            public readonly static Guid AccessGuid = Guid.Parse("74f75c4d-b32e-4077-9227-492050aa9f50");

            // Create Salary In-Advance
            public readonly static int Create = 71;
            public readonly static Guid CreateGuid = Guid.Parse("1ebdad29-589f-419b-9d7e-4d7e7edbd114");

            // Delete Salary In-Advance
            public readonly static int Delete = 72;
            public readonly static Guid DeleteGuid = Guid.Parse("618a9fc0-16d7-4933-836f-41384da6279f");

            // Restore Salary In-Advance
            public readonly static int Restore = 73;
            public readonly static Guid RestoreGuid = Guid.Parse("a02be203-4199-43f1-8573-ac339cf32dc9");

            // Update Salary In-Advance
            public readonly static int Update = 74;
            public readonly static Guid UpdateGuid = Guid.Parse("78147edd-1df3-47c7-9941-1ffbdf5733f1");

        }
        public static class StaffSalaryProcess
        {
            // Access Staff Salary Process
            public readonly static int Access = 65;
            public readonly static Guid AccessGuid = Guid.Parse("770c748b-8028-436f-af6c-94e23cbc4d2c");

            // Create Staff Salary Process
            public readonly static int Create = 66;
            public readonly static Guid CreateGuid = Guid.Parse("78ce1ef6-fc8b-496c-937e-e48d55bf6954");

            // Delete Staff Salary Process
            public readonly static int Delete = 67;
            public readonly static Guid DeleteGuid = Guid.Parse("1e856c57-d5e5-474b-9392-1f36978d9e63");

            // Restore Staff Salary Process
            public readonly static int Restore = 68;
            public readonly static Guid RestoreGuid = Guid.Parse("d504a999-71ee-4833-9bad-40cfa6285abe");

            // Update Staff Salary Process
            public readonly static int Update = 69;
            public readonly static Guid UpdateGuid = Guid.Parse("500c3aed-d449-4ad3-a9d2-648bc3a26977");

        }
        public static class StaffOvertime
        {
            // Access Staff Overtime
            public readonly static int Access = 75;
            public readonly static Guid AccessGuid = Guid.Parse("a4daaef1-965a-4a7e-8e72-6077307be4aa");

            // Create Staff Overtime
            public readonly static int Create = 76;
            public readonly static Guid CreateGuid = Guid.Parse("3260e610-8182-4c57-9412-8cb15315cce3");

            // Delete Staff Overtime
            public readonly static int Delete = 77;
            public readonly static Guid DeleteGuid = Guid.Parse("3a326037-0087-4db4-a7f2-986be58973dd");

            // Restore Staff Overtime
            public readonly static int Restore = 78;
            public readonly static Guid RestoreGuid = Guid.Parse("e9d4d8a3-4c51-4804-bdd8-436fc308f2fc");

            // Update Staff Overtime
            public readonly static int Update = 79;
            public readonly static Guid UpdateGuid = Guid.Parse("b70578a3-3f0b-4299-8116-058bf8062311");

        }

        
        



        #endregion
        public static class StaffRenewalResidency
        {
            // Access Staff Renewal Residency
            public readonly static int Access = 1;
            public readonly static Guid AccessGuid = Guid.Parse("4ff97566-02a2-4c67-afba-e9bf8ef05d47");

            // Create Staff Renewal Residency
            public readonly static int Create = 2;
            public readonly static Guid CreateGuid = Guid.Parse("400d38de-6312-4db6-bf52-1e0bfc37a2f8");

            // Delete Staff Renewal Residency
            public readonly static int Delete = 3;
            public readonly static Guid DeleteGuid = Guid.Parse("a3ac3703-58e7-4005-a00c-8f1ab17d8da3");

            // Update Staff Renewal Residency
            public readonly static int Update = 4;
            public readonly static Guid UpdateGuid = Guid.Parse("7d2eced5-08f4-40c7-889a-ffafff37e429");

            // Restore Staff Renewal Residency
            public readonly static int Restore = 5;
            public readonly static Guid RestoreGuid = Guid.Parse("7692e108-cc7d-4840-8a28-6b6783bccb27");

        }
        public static class StandardReport
        {
            // Access Stander Report
            public readonly static int Access = 11;
            public readonly static Guid AccessGuid = Guid.Parse("0de014a3-6f81-473e-b328-17033551ab56");

        }
        public static class Steps
        {
            // Update Steps
            public readonly static Guid UpdateGuid = Guid.Parse("3bda3c24-b2db-444a-aae8-2911cd11891f");

            // Remove Steps
            public readonly static Guid RemoveGuid = Guid.Parse("80509ea2-832f-4687-8ccf-71786f5b7d10");

            // Create Steps
            public readonly static Guid CreateGuid = Guid.Parse("fd04fafa-8c23-4815-8eb4-f3fe5734db79");

        }
        public static class STIConfiguration
        {
            // Access STI Configuration
            public readonly static int Access = 28;
            public readonly static Guid AccessGuid = Guid.Parse("cd41fd59-642e-4e50-ac2b-f092b9004903");

            // Create STI Configuration
            public readonly static int Create = 29;
            public readonly static Guid CreateGuid = Guid.Parse("c03f3f1f-4f2f-4dac-ab3a-7069af5e526c");

            // Update STI Configuration
            public readonly static int Update = 30;
            public readonly static Guid UpdateGuid = Guid.Parse("0bd0e935-4524-4188-9b35-d5842e2e1a33");

            // Delete STI Configuration
            public readonly static int Delete = 31;
            public readonly static Guid DeleteGuid = Guid.Parse("e8d4ec06-c9d7-4ff8-98c5-9609bd5b44d0");
            // Restore STI Configuration
            public readonly static int Restore = 32;
            public readonly static Guid RestoreGuid = Guid.Parse("da64101b-b1d3-4094-9614-22edf31e7899");


        }
        public static class STIContacts
        {
            // Access STI Contacts
            public readonly static int Access = 17;
            public readonly static Guid AccessGuid = Guid.Parse("29aa7b35-e492-4c2e-b400-db4f6bfdbe51");

            // Create STI Contacts
            public readonly static int Create = 18;
            public readonly static Guid CreateGuid = Guid.Parse("43594850-773c-4621-b63e-4c6450373539");

            // Delete STI Contacts
            public readonly static int Delete = 19;
            public readonly static Guid DeleteGuid = Guid.Parse("130e931f-048a-4bec-8f0b-3b174b61f73c");

            // Update STI Contacts
            public readonly static int Update = 20;
            public readonly static Guid UpdateGuid = Guid.Parse("54ceef66-441c-4ed7-bc91-f78602bc12b6");

            // Restore STI Contacts
            public readonly static int Restore = 21;
            public readonly static Guid RestoreGuid = Guid.Parse("7bafea29-9f92-4474-97c5-b1fad9e35849");

        }
        public static class StockItemDistribution
        {
            // Access Stock Item Distribution
            public readonly static int Access = 1;
            public readonly static Guid AccessGuid = Guid.Parse("9ece9038-88e4-4225-b59c-32775d2510cc");

            // Create Stock Item Distribution
            public readonly static int Create = 2;
            public readonly static Guid CreateGuid = Guid.Parse("4a270d9d-d31a-4ada-8723-704b7702b730");

            // Delete Stock Item Distribution
            public readonly static int Delete = 3;
            public readonly static Guid DeleteGuid = Guid.Parse("9ec56dfd-6554-4e78-b6ca-731029b1fbc1");

            // Restore Stock Item Distribution
            public readonly static int Restore = 4;
            public readonly static Guid RestoreGuid = Guid.Parse("e8f2033c-2353-4ce5-a216-e25a22647c7d");

            // Update Stock Item Distribution
            public readonly static int Update = 5;
            public readonly static Guid UpdateGuid = Guid.Parse("450b1a51-e43e-4887-8234-d9a678013f0d");

        }
        public static class SyncStaffBetweenDevices
        {
            // Access Sync Staff Between Devices
            public readonly static int Access = 16;
            public readonly static Guid AccessGuid = Guid.Parse("e79d0335-fa74-46d0-85fe-63c8cc17bb0a");

            // Create Sync Staff Between Devices
            public readonly static int Create = 17;
            public readonly static Guid CreateGuid = Guid.Parse("caf3fe2a-b9e4-4e82-850d-463ced5051d6");

            // Update Sync Staff Between Devices
            public readonly static int Update = 18;
            public readonly static Guid UpdateGuid = Guid.Parse("1d165824-f720-4c8a-b9c0-8ade4db02685");

            // Delete Sync Staff Between Devices
            public readonly static int Delete = 19;
            public readonly static Guid DeleteGuid = Guid.Parse("2deca43c-b633-4fec-8d6f-38a3a012b96d");

            // Restore Sync Staff Between Devices
            public readonly static int Restore = 20;
            public readonly static Guid RestoreGuid = Guid.Parse("5e578315-c3f0-4cf1-8ec3-326a038d9460");

        }
        public static class TelecomCompanies
        {
            // Access Telecom Companies
            public readonly static int Access = 1;
            public readonly static Guid AccessGuid = Guid.Parse("8c2d1da5-74c1-4eb6-b19b-1187787d84b5");

            // Create Telecom Companies
            public readonly static int Create = 2;
            public readonly static Guid CreateGuid = Guid.Parse("020e6403-65e2-468c-a5a3-eaa4d9f197ba");

            // Update Telecom Companies
            public readonly static int Update = 3;
            public readonly static Guid UpdateGuid = Guid.Parse("05e3f033-b69c-483f-8e16-c754659050f6");

            // Delete Telecom Companies
            public readonly static int Delete = 4;
            public readonly static Guid DeleteGuid = Guid.Parse("f77f8565-0573-4ede-9aa1-0e5247ca57d0");

            // Restore Telecom Companies
            public readonly static int Restore = 5;
            public readonly static Guid RestoreGuid = Guid.Parse("9d0caf34-a965-484f-9c13-90d1b87b00e9");

        }
        public static class TelecomCompaniesLanguages
        {
            // Create Telecom Company Languages
            public readonly static Guid CreateGuid = Guid.Parse("acd3caa4-c2ef-4b9a-be46-e7cd72323e0c");

            // Delete Telecom Company Languages
            public readonly static Guid DeleteGuid = Guid.Parse("645f9154-5aa8-4bb7-a715-9782b403b192");

            // Restore Telecom Company Languages
            public readonly static Guid RestoreGuid = Guid.Parse("05783733-287e-48e3-ba79-18bad70e845b");

            // Update Telecom Company Languages
            public readonly static Guid UpdateGuid = Guid.Parse("45afab13-9463-4c16-992d-5c6f9c25dde0");

        }
        public static class TenderBOCs
        {
            // Access Tender BOCs
            public readonly static int Access = 7;
            public readonly static Guid AccessGuid = Guid.Parse("446819ff-6a95-4672-9bb1-50bd94ecd742");

            // Create Tender BOCs
            public readonly static int Create = 8;
            public readonly static Guid CreateGuid = Guid.Parse("ed67a732-2f73-4d51-bfff-b68fa4073f2c");

            // Update Tender BOCs
            public readonly static int Update = 9;
            public readonly static Guid UpdateGuid = Guid.Parse("b4023dea-5c01-4438-b01c-3a49385510ee");

            // Delete Tender BOCs
            public readonly static int Delete = 10;
            public readonly static Guid DeleteGuid = Guid.Parse("3e926e63-5dd6-4277-8f0a-c5948e39ab5b");

            // Restore Tender BOCs
            public readonly static int Restore = 11;
            public readonly static Guid RestoreGuid = Guid.Parse("a3a46ba7-a98f-49aa-8e3f-a2479519703e");

        }
        public static class TenderReports
        {
            // Access Tender Reports
            public readonly static int Access = 6;
            public readonly static Guid AccessGuid = Guid.Parse("931e3263-cc69-44e4-9bda-3211ec96669a");

        }
        public static class Tenders
        {
            // Access Tenders
            public readonly static int Access = 1;
            public readonly static Guid AccessGuid = Guid.Parse("5f7f4227-7625-4742-8753-e8197dca6410");

            // Create Tenders
            public readonly static int Create = 2;
            public readonly static Guid CreateGuid = Guid.Parse("9a8a568a-a3fe-490f-8693-0fbd334f733a");

            // Update Tenders
            public readonly static int Update = 3;
            public readonly static Guid UpdateGuid = Guid.Parse("29ec99e0-91e7-45ce-8562-f3b202859dfd");

            // Delete Tenders
            public readonly static int Delete = 4;
            public readonly static Guid DeleteGuid = Guid.Parse("ab266f04-17b4-489c-9ad5-9f42ccef5322");

            // Restore Tenders
            public readonly static int Restore = 5;
            public readonly static Guid RestoreGuid = Guid.Parse("da317fec-8813-4713-b4c2-c2c2eeae0e64");

        }
        public static class Timezone
        {
            // Update Time Zone
            public readonly static Guid UpdateGuid = Guid.Parse("d2aaad5e-e0ab-436f-999e-91162da9b789");

        }
        public static class UploadPartnerReport
        {
            // Access Upload Partner Report
            public readonly static int Access = 6;
            public readonly static Guid AccessGuid = Guid.Parse("95b2b609-d80f-42ca-9758-9d9b4d144255");

            // Create Upload Partner Report
            public readonly static int Create = 7;
            public readonly static Guid CreateGuid = Guid.Parse("7c210286-f3fd-417c-95b7-ca638ff5d23c");

            // Delete Upload Partner Report
            public readonly static int Delete = 8;
            public readonly static Guid DeleteGuid = Guid.Parse("d40491fd-0110-41ed-8535-5cfc9c779b0b");

            // Restore Upload Partner Report
            public readonly static int Restore = 9;
            public readonly static Guid RestoreGuid = Guid.Parse("38fd45d2-430f-4b1e-8336-2ac30657505c");

            // Update Upload Partner Report
            public readonly static int Update = 10;
            public readonly static Guid UpdateGuid = Guid.Parse("0d31132a-bde9-4294-a0fc-f4171fe34d4b");

        }
        public static class UserAccountsManagement
        {
            // Access User Accounts
            public readonly static int Access = 118;
            public readonly static Guid AccessGuid = Guid.Parse("d4d62e8a-4450-4a62-b445-e9499099f1be");

            // Create User Accounts
            public readonly static int Create = 119;
            public readonly static Guid CreateGuid = Guid.Parse("0f0c5bd0-141d-41a2-b7ac-a352ce2cd63e");

            // Update User Accounts
            public readonly static int Update = 120;
            public readonly static Guid UpdateGuid = Guid.Parse("b365a1de-1471-47c8-9de7-d45ee87f6eba");

            // Delete User Accounts
            public readonly static int Delete = 121;
            public readonly static Guid DeleteGuid = Guid.Parse("9a8c1e02-c782-48b7-94c1-31b4d510470b");

            // Restore User Accounts
            public readonly static int Restore = 122;
            public readonly static Guid RestoreGuid = Guid.Parse("f28ad47b-d084-4965-bb85-9a9b269e8bf9");

        }
        public static class Vehicle
        {
            // Access Vehicle
            public readonly static int Access = 21;
            public readonly static Guid AccessGuid = Guid.Parse("5d19fd71-759b-47e4-b5f1-b73288366d01");

            // Create Vehicle
            public readonly static int Create = 22;
            public readonly static Guid CreateGuid = Guid.Parse("7a5062d1-b574-44f1-8d7d-36b985dbcdd4");

            // Delete Vehicle
            public readonly static int Delete = 23;
            public readonly static Guid DeleteGuid = Guid.Parse("e98d570f-fb60-4548-9a48-ff80c05611f1");

            // Restore Vehicle
            public readonly static int Restore = 24;
            public readonly static Guid RestoreGuid = Guid.Parse("1a5cfca0-1a84-4d39-9259-9aabf07d845a");

            // Update Vehicle
            public readonly static int Update = 25;
            public readonly static Guid UpdateGuid = Guid.Parse("f2fbb635-78db-4d6c-acee-80b551deb760");

        }
        public static class VehicleMaintenanceRequest
        {
            // Access Vehicle Maintenance Request
            public readonly static int Access = 11;
            public readonly static Guid AccessGuid = Guid.Parse("9a823cd1-6874-4da1-b621-448e97800982");

            // Create Vehicle Maintenance Request
            public readonly static int Create = 12;
            public readonly static Guid CreateGuid = Guid.Parse("f6865626-c516-488c-b7da-722f978a74d1");

            // Delete Vehicle Maintenance Request
            public readonly static int Delete = 13;
            public readonly static Guid DeleteGuid = Guid.Parse("560e5853-c63e-4cd5-bb68-88dc7d7d0f71");

            // Remove Vehicle Maintenance Request
            public readonly static int Remove = 14;
            public readonly static Guid RemoveGuid = Guid.Parse("faf54d86-1f08-4bc0-9a6a-d1314214cbce");

            // Restore Vehicle Maintenance Request
            public readonly static int Restore = 15;
            public readonly static Guid RestoreGuid = Guid.Parse("d114afa0-3171-45eb-be52-67631d6ea5c1");

            // Update Vehicle Maintenance Request
            public readonly static int Update = 16;
            public readonly static Guid UpdateGuid = Guid.Parse("546a84c5-599a-4248-9e68-d7142a14600b");

        }
        public static class Vote
        {
            // Update Vote
            public readonly static Guid UpdateGuid = Guid.Parse("3170f493-3559-44ef-bc70-923de64cf97f");

            // Create Vote
            public readonly static Guid CreateGuid = Guid.Parse("7cee8ef3-5acf-4110-aa0a-33eb296cfb05");

        }
        public static class WarehouseDashboard
        {
            // Access Warehouse Dashboard
            public readonly static int Access = 16;
            public readonly static Guid AccessGuid = Guid.Parse("f7f701ef-9026-4bf4-b842-7e42a4256fe3");

        }
        public static class LicenseandSubscriptionContracts
        {
            // Access License and Subscription Contracts
            public readonly static int Access = 22;
            public readonly static Guid AccessGuid = Guid.Parse("2ed840a5-2a8a-4329-b9f1-a8e72537fa4a");

            // Create License and Subscription Contracts
            public readonly static int Create = 23;
            public readonly static Guid CreateGuid = Guid.Parse("2a0fe4cd-dc50-4fbd-a189-95dc51414464");

            // Delete License and Subscription Contracts
            public readonly static int Delete = 24;
            public readonly static Guid DeleteGuid = Guid.Parse("d6d2fc14-8460-469c-b815-ce6d14ae6897");

            // Restore License and Subscription Contracts
            public readonly static int Restore = 25;
            public readonly static Guid RestoreGuid = Guid.Parse("f3203bcb-24a8-458d-a341-4179bdd93552");

            // Update License and Subscription Contracts
            public readonly static int Update = 26;
            public readonly static Guid UpdateGuid = Guid.Parse("bfde0075-95a3-4882-bc95-5f04c397ef01");

            // Upload License and Subscription Contracts
            public readonly static int Upload = 27;
            public readonly static Guid UploadGuid = Guid.Parse("141d302e-4fb6-4f45-ac58-2b8f4fcd8685");

        }
        public static class STIDamagedItemAdminApproval
        {
            // Access STI Damaged Item Admin Approval
            public readonly static int Access = 28;
            public readonly static Guid AccessGuid = Guid.Parse("f21af64e-b17e-4b3a-9c8a-de25b759ed1c");

            // Confirm STI Damaged Item Admin Approval
            public readonly static int Confirm = 29;
            public readonly static Guid ConfirmGuid = Guid.Parse("10c0e8dc-9443-4d00-9adc-8a457f829467");

        }
        public static class STIDamagedItemFinanceApproval
        {
            // Access STI Damaged Item Finance Approval
            public readonly static int Access = 30;
            public readonly static Guid AccessGuid = Guid.Parse("f6ab45dc-955c-46df-b88f-562a6452a29e");

            // Confirm STI Damaged Item Finance Approval
            public readonly static int Confirm = 31;
            public readonly static Guid ConfirmGuid = Guid.Parse("edb30416-81dd-4e1e-8235-5406cd613309");

        }
        public static class WarehouseItemsEntry
        {
            // Access Warehouse Item Entry
            public readonly static int Access = 1;
            public readonly static Guid AccessGuid = Guid.Parse("d2b7f2ef-7846-4606-a393-679f77ebd41b");

            // Create Warehouse Item Entry
            public readonly static int Create = 2;
            public readonly static Guid CreateGuid = Guid.Parse("6bcb8619-b15d-4af3-839a-5691e351551f");

            // Delete Warehouse Item Entry
            public readonly static int Delete = 3;
            public readonly static Guid DeleteGuid = Guid.Parse("2b69a4d7-9712-4d4f-b0d5-16cf5b6dae11");

            // Restore Warehouse Item Entry
            public readonly static int Restore = 4;
            public readonly static Guid RestoreGuid = Guid.Parse("1a5f81c8-269a-4b91-a342-400f42bb58c4");

            // Update Warehouse Item Entry
            public readonly static int Update = 5;
            public readonly static Guid UpdateGuid = Guid.Parse("14847029-e3c2-4a2f-8ba5-8a24648f3340");

        }
        public static class WarehouseItemsRelease
        {
            // Access Warehouse Item Release
            public readonly static int Access = 11;
            public readonly static Guid AccessGuid = Guid.Parse("e69ecc5c-afdc-46d5-bbf6-a0fb86d13587");

            // Create Warehouse Item Release
            public readonly static int Create = 12;
            public readonly static Guid CreateGuid = Guid.Parse("13116fac-e6aa-4014-949c-9d7d8bf3fa4e");

            // Delete Warehouse Item Release
            public readonly static int Delete = 13;
            public readonly static Guid DeleteGuid = Guid.Parse("ae4e2e36-6b21-41bf-a9cd-5fbe39180fa8");

            // Restore Warehouse Item Release
            public readonly static int Restore = 14;
            public readonly static Guid RestoreGuid = Guid.Parse("958f1be7-9f70-4c0f-bb54-56154ea774e9");

            // Update Warehouse Item Release
            public readonly static int Update = 15;
            public readonly static Guid UpdateGuid = Guid.Parse("fdb4b5ff-ae57-42bd-b8aa-9a58e4414863");

        }
        public static class WarehouseItemsStaffRequest
        {
            // Create Warehouse Items Staff Request
            public readonly static Guid CreateGuid = Guid.Parse("bb0241af-2bc6-4d6d-84af-a82be8fa42c0");

            // Update Warehouse Items Staff Request
            public readonly static Guid UpdateGuid = Guid.Parse("31e89eb4-3629-4341-a900-75678722c0bd");

            // Access Warehouse Items Staff Request
            public readonly static Guid AccessGuid = Guid.Parse("80b90709-f370-46f3-b582-1bb24f428e16");

            // Delete Warehouse Items Staff Request
            public readonly static Guid DeleteGuid = Guid.Parse("e68f7493-a856-4670-a2c0-0bd6b55b92c1");

            // Restore Warehouse Items Staff Request
            public readonly static Guid RestoreGuid = Guid.Parse("c6e0ae55-90c9-4204-947e-2446a3ed4113");

            // Remove Warehouse Items Staff Request
            public readonly static Guid RemoveGuid = Guid.Parse("961f9519-bea4-4244-91db-f9baa6545f09");

        }
        public static class WorkAddress
        {
            // Create Work Address
            public readonly static Guid CreateGuid = Guid.Parse("c826831f-4a6a-4c07-ad5f-ed6df97fa28f");

            // Update Work Address
            public readonly static Guid UpdateGuid = Guid.Parse("f264591e-12e3-4539-9afe-9b137087c0a7");

        }
        public static class WorkingDaysConfigurations
        {
            // Create Duty Stations Working Days Configurations
            public readonly static Guid CreateGuid = Guid.Parse("6cfd146b-43c7-4f21-ba3c-79a2f7b2b7a8");

            // Remove Duty Stations Working Days Configurations
            public readonly static Guid RemoveGuid = Guid.Parse("2cd743bc-16bc-4bcd-815a-3f3af45b55c9");

            // Update Duty Stations Working Days Configurations
            public readonly static Guid UpdateGuid = Guid.Parse("ff95dd44-1f06-424f-b585-4d561eac9fca");

        }


        public static class GroupTwoApplicationsReview
        {
            // Access Group Twp Applications Review
            public readonly static Guid AccessGuid = Guid.Parse("a39af121-c7a2-43e5-bf4f-6e78aab026a9");

            // Create Group Twp Applications Review
            public readonly static int Create = 1;
            public readonly static Guid CreateGuid = Guid.Parse("beb620e2-a080-4aec-97a7-e5c089e07fa6");

            // Update Group Twp Applications Review
            public readonly static int Update = 2;
            public readonly static Guid UpdateGuid = Guid.Parse("bf9e516f-f04f-4b2e-8b99-a85b2aadedf7");

            // Delete Group Twp Applications Review
            public readonly static int Delete = 3;
            public readonly static Guid DeleteGuid = Guid.Parse("de9f2f2e-cd42-427a-aaef-bd85164c2cbf");

            // Restore Group Twp Applications Review
            public readonly static int Restore = 4;
            public readonly static Guid RestoreGuid = Guid.Parse("20fb114a-9f65-4ffb-8969-9e26508f6dcb");

        }

        public static class Portalforgrouptwoapplications
        {
            // Update Portal for group two applications
            public readonly static Guid UpdateGuid = Guid.Parse("5367c543-12a4-487a-bd80-f2d1c765468f");

            // Create Portal for group two applications
            public readonly static Guid CreateGuid = Guid.Parse("f4598b2a-f0b2-4515-a3f0-df56a2e20ac8");

            // Access Portal for group two applications
            public readonly static Guid AccessGuid = Guid.Parse("fc888c9c-78a4-4042-a587-ac6f9643945e");

        }

        public static class PartnerMonitoringDatabase
        {
            // Access Partner Monitoring Database
            public readonly static int Access = 1;
            public readonly static Guid AccessGuid = Guid.Parse("a0334781-d560-4b4d-9228-8973c1442611");

            // Create Partner Monitoring Database
            public readonly static int Create = 2;
            public readonly static Guid CreateGuid = Guid.Parse("ee5e343e-ee41-46ec-8b99-dbbda77949c5");

            // Update Partner Monitoring Database
            public readonly static int Update = 3;
            public readonly static Guid UpdateGuid = Guid.Parse("df05dcba-6579-4378-ac38-13d75a54e028");

            // Delete Partner Monitoring Database
            public readonly static int Delete = 4;
            public readonly static Guid DeleteGuid = Guid.Parse("9bfba82e-d37d-4c35-b376-228dacf55fa8");

            // Restore Partner Monitoring Database
            public readonly static int Restore = 5;
            public readonly static Guid RestoreGuid = Guid.Parse("49e58784-b588-4f78-a23a-1d00eff4ddc8");

        }
        public static class PartnerMonitoringDatabaseCountryTechVerify
        {
            // Create Partner Monitoring Database Country Tech Approve
            public readonly static int Create = 9;
            public readonly static Guid CreateGuid = Guid.Parse("d864d103-fc58-4d08-b055-761acddba681");

        }
        public static class PartnerMonitoringDatabaseFieldTechVerify
        {
            // Create Partner Monitoring Database Field Tech Verify
            public readonly static int Create = 7;
            public readonly static Guid CreateGuid = Guid.Parse("f7588fef-8b10-4073-b42c-f7826c3c40c6");

        }
        public static class PartnerMonitoringDatabaseProgrammeApprove
        {
            // Create Partner Monitoring Database Programme Approve
            public readonly static int Create = 8;
            public readonly static Guid CreateGuid = Guid.Parse("fae9b4ba-a7b2-4513-b61e-a9229a269edc");

        }
        public static class PartnerMonitoringDatabaseSpecificIndicator
        {
            // Access Partner Monitoring Database Specific Indicator
            public readonly static int Access = 14;
            public readonly static Guid AccessGuid = Guid.Parse("8bcbf7ec-0115-4ef2-8069-b919a05171ce");

            // Create Partner Monitoring Database Specific Indicator
            public readonly static int Create = 15;
            public readonly static Guid CreateGuid = Guid.Parse("e3415050-dea7-4e94-977a-120c756b8106");

            // Update Partner Monitoring Database Specific Indicator
            public readonly static int Update = 16;
            public readonly static Guid UpdateGuid = Guid.Parse("713e872a-6646-4681-a476-c33d21db8b95");

            // Delete Partner Monitoring Database Specific Indicator
            public readonly static int Delete = 17;
            public readonly static Guid DeleteGuid = Guid.Parse("017d59d6-0790-41fc-8d03-36614ebda4d5");

            // Restore Partner Monitoring Database Specific Indicator
            public readonly static int Restore = 18;
            public readonly static Guid RestoreGuid = Guid.Parse("c38fd7e3-b16c-424c-8c92-13b69d90874d");

        }
        public static class PartnerMonitoringDatabaseSpecificObjective
        {
            // Access Partner Monitoring Database Specific Objective
            public readonly static int Access = 10;
            public readonly static Guid AccessGuid = Guid.Parse("01be8e62-70d3-425f-b4ab-086ad1287c45");

            // Create Partner Monitoring Database Specific Objective
            public readonly static int Create = 11;
            public readonly static Guid CreateGuid = Guid.Parse("7acd5c96-b808-45de-a25f-1851aa5a1391");

            // Delete Partner Monitoring Database Specific Objective
            public readonly static int Delete = 12;
            public readonly static Guid DeleteGuid = Guid.Parse("5d655586-d693-470a-8c07-3000b8906ec5");

            // Restore Partner Monitoring Database Specific Objective
            public readonly static int Restore = 13;
            public readonly static Guid RestoreGuid = Guid.Parse("51631d38-a7eb-4f09-a34d-dd334f045557");

        }
        public static class PartnerMonitoringDatabaseVerify
        {
            // Create Partner Monitoring Database Verify
            public readonly static int Create = 6;
            public readonly static Guid CreateGuid = Guid.Parse("849e2d23-7b7d-4f13-971e-677db68e6423");

        }
        public static class PartnersCapacityAssessment
        {
            // Access Partners Capacity Assessment
            public readonly static int Access = 1;
            public readonly static Guid AccessGuid = Guid.Parse("4017a71d-0fb3-4154-b134-cdb15ce986f5");

            // Create Partners Capacity Assessment
            public readonly static int Create = 2;
            public readonly static Guid CreateGuid = Guid.Parse("9d98d213-329a-4f82-a161-c5f3a40d89ac");

            // Delete Partners Capacity Assessment
            public readonly static int Delete = 3;
            public readonly static Guid DeleteGuid = Guid.Parse("1ba4b998-63c6-49ab-ab8c-504a4dff64a6");

            // Restore Partners Capacity Assessment
            public readonly static int Restore = 4;
            public readonly static Guid RestoreGuid = Guid.Parse("5693b69e-9bdf-4666-9375-1fa25167fafb");

            // Confirm Partners Capacity Assessment
            public readonly static int Confirm = 5;
            public readonly static Guid ConfirmGuid = Guid.Parse("546ad113-539c-4c16-9b99-bcf5e56b0d9e");

        }

        public static class PMDPPA
        {
            // Access PMD PPA
            public readonly static int Access = 19;
            public readonly static Guid AccessGuid = Guid.Parse("d18b6d76-bbb7-424f-99cb-9b17d59189b0");

            // Create PMD PPA
            public readonly static int Create = 20;
            public readonly static Guid CreateGuid = Guid.Parse("44df54f8-6941-45ac-a511-001c6c8c9d6c");

            // Update PMD PPA
            public readonly static int Update = 21;
            public readonly static Guid UpdateGuid = Guid.Parse("4a17464d-eeb7-4dc8-a559-e8a3e5db5e4a");

            // Delete PMD PPA
            public readonly static int Delete = 22;
            public readonly static Guid DeleteGuid = Guid.Parse("1755482d-63ca-49cf-81fe-1e1bdfa14fcf");

            // Restore PMD PPA
            public readonly static int Restore = 23;
            public readonly static Guid RestoreGuid = Guid.Parse("f193e148-eb18-417a-ab3f-25dc0262bf35");

        }
        #region HIMS
        public static class HealthCentersReferral
        {
            // Access Health Centers Referral
            public readonly static int Access = 1;
            public readonly static Guid AccessGuid = Guid.Parse("359490eb-ca50-4e3b-a5bf-5faac4127683");

            // Create Health Centers Referral
            public readonly static int Create = 2;
            public readonly static Guid CreateGuid = Guid.Parse("697a209c-31dd-479e-9028-d9d129297b31");

            // Print Health Centers Referral
            public readonly static int Print = 3;
            public readonly static Guid PrintGuid = Guid.Parse("f9c1e1ed-cf1e-45b7-83f8-02db9b2d491f");

        }
        public static class HealthHospitalReferralTracking
        {
            // Access Health Hospital Referral Tracking
            public readonly static int Access = 4;
            public readonly static Guid AccessGuid = Guid.Parse("2cd43e3c-ae3d-4545-a5de-8f275128fff2");

            // Create Health Hospital Referral Tracking
            public readonly static int Create = 5;
            public readonly static Guid CreateGuid = Guid.Parse("cbc06fad-db1b-442c-a6f0-37249201d2c7");

        }
        public static class HealthReferralGeneral
        {
            // Access Health Referral General
            public readonly static int Access = 11;
            public readonly static Guid AccessGuid = Guid.Parse("6b362c1a-4f18-4c2d-a9fd-b9c337dd6aa8");

        }
        public static class HealthReferralMinistryManagement
        {
            // Access Health Referral Ministry Management
            public readonly static int Access = 6;
            public readonly static Guid AccessGuid = Guid.Parse("f0f46838-1c42-4d58-aa13-3ed39669981a");

            // Create Health Referral Ministry Management
            public readonly static int Create = 7;
            public readonly static Guid CreateGuid = Guid.Parse("72181be4-1bb8-4c75-93a1-8862b5dfa4bb");

        }
        public static class HealthReferralUNHCRManagement
        {
            // Access Health Referral UNHCR Management
            public readonly static int Access = 8;
            public readonly static Guid AccessGuid = Guid.Parse("0892306c-ad56-40b0-951e-19bd10cbb316");

            // Create Health Referral UNHCR Management
            public readonly static int Create = 9;
            public readonly static Guid CreateGuid = Guid.Parse("5c6e3c62-1f02-45c6-b781-7819911a4d99");

            // ValidateData Health Referral UNHCR Management
            public readonly static int ValidateData = 10;
            public readonly static Guid ValidateDataGuid = Guid.Parse("bc4aa3fc-b7be-449d-a04a-e1a786c04dea");

        }
        #endregion
        #region SDS
        public static class TemplateSurveyConfiguration
        {
            // Access Template Survey Configuration
            public readonly static int Access = 1;
            public readonly static Guid AccessGuid = Guid.Parse("2a96f719-a8bd-41f4-a383-c8d56fa2a76e");

            // Create Template Survey Configuration
            public readonly static int Create = 2;
            public readonly static Guid CreateGuid = Guid.Parse("d5a894f4-6df5-424d-914b-1eb1e4ca45a9");

            // Delete Template Survey Configuration
            public readonly static int Delete = 3;
            public readonly static Guid DeleteGuid = Guid.Parse("b670b8e2-694e-4be5-88b4-a9d4f785e40e");

            // Restore Template Survey Configuration
            public readonly static int Restore = 4;
            public readonly static Guid RestoreGuid = Guid.Parse("31bff684-2b61-4a49-a228-ca9f18fc82be");

            // Remove Template Survey Configuration
            public readonly static int Remove = 5;
            public readonly static Guid RemoveGuid = Guid.Parse("58f873c4-0708-4882-97c6-28b8d4081aed");

            // Update Template Survey Configuration
            public readonly static int Update = 6;
            public readonly static Guid UpdateGuid = Guid.Parse("bd7e0e64-ba04-4a99-ad05-ec774d80cde7");

            // Upload Template Survey Configuration
            public readonly static int Upload = 7;
            public readonly static Guid UploadGuid = Guid.Parse("94574f35-2860-40c2-9b75-8cd006814aa7");

        }
        #endregion
    }

}