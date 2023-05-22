﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PPA_DAL.Model
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class PPAEntities : DbContext
    {
        public PPAEntities()
            : base("name=PPAEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<dataAuditActions> dataAuditActions { get; set; }
        public virtual DbSet<dataAuditFields> dataAuditFields { get; set; }
        public virtual DbSet<dataAuditLogins> dataAuditLogins { get; set; }
        public virtual DbSet<dataAuditPermissions> dataAuditPermissions { get; set; }
        public virtual DbSet<sysDataTablesProperties> sysDataTablesProperties { get; set; }
        public virtual DbSet<Test> Test { get; set; }
        public virtual DbSet<TestLookUp> TestLookUp { get; set; }
        public virtual DbSet<userAccounts> userAccounts { get; set; }
        public virtual DbSet<userApplicationToken> userApplicationToken { get; set; }
        public virtual DbSet<userConnections> userConnections { get; set; }
        public virtual DbSet<userContactDetails> userContactDetails { get; set; }
        public virtual DbSet<userHomeAddress> userHomeAddress { get; set; }
        public virtual DbSet<userHomeAddressLanguage> userHomeAddressLanguage { get; set; }
        public virtual DbSet<userManagersHistory> userManagersHistory { get; set; }
        public virtual DbSet<userMenuShortcut> userMenuShortcut { get; set; }
        public virtual DbSet<userNotificationControls> userNotificationControls { get; set; }
        public virtual DbSet<userNotificationLogs> userNotificationLogs { get; set; }
        public virtual DbSet<userPasswords> userPasswords { get; set; }
        public virtual DbSet<userPermissions> userPermissions { get; set; }
        public virtual DbSet<userPersonalDetails> userPersonalDetails { get; set; }
        public virtual DbSet<userPersonalDetailsLanguage> userPersonalDetailsLanguage { get; set; }
        public virtual DbSet<userProfiles> userProfiles { get; set; }
        public virtual DbSet<userRegistrationQueue> userRegistrationQueue { get; set; }
        public virtual DbSet<userServiceHistory> userServiceHistory { get; set; }
        public virtual DbSet<userStepsHistory> userStepsHistory { get; set; }
        public virtual DbSet<userWorkAddress> userWorkAddress { get; set; }
        public virtual DbSet<userWorkAddressLanguage> userWorkAddressLanguage { get; set; }
        public virtual DbSet<codeActions> codeActions { get; set; }
        public virtual DbSet<codeActionsCategories> codeActionsCategories { get; set; }
        public virtual DbSet<codeActionsCategoriesFactors> codeActionsCategoriesFactors { get; set; }
        public virtual DbSet<codeActionsCategoriesLanguages> codeActionsCategoriesLanguages { get; set; }
        public virtual DbSet<codeActionsEntities> codeActionsEntities { get; set; }
        public virtual DbSet<codeActionsEntitiesLanguages> codeActionsEntitiesLanguages { get; set; }
        public virtual DbSet<codeActionsLanguages> codeActionsLanguages { get; set; }
        public virtual DbSet<codeActionsVerbs> codeActionsVerbs { get; set; }
        public virtual DbSet<codeActionsVerbsLanguages> codeActionsVerbsLanguages { get; set; }
        public virtual DbSet<codeApplications> codeApplications { get; set; }
        public virtual DbSet<codeApplicationsLanguages> codeApplicationsLanguages { get; set; }
        public virtual DbSet<codeAppointmentType> codeAppointmentType { get; set; }
        public virtual DbSet<codeAppointmentTypeLanguage> codeAppointmentTypeLanguage { get; set; }
        public virtual DbSet<codeCallCost> codeCallCost { get; set; }
        public virtual DbSet<codeConditionType> codeConditionType { get; set; }
        public virtual DbSet<codeConditionTypeLanguage> codeConditionTypeLanguage { get; set; }
        public virtual DbSet<codeConnections> codeConnections { get; set; }
        public virtual DbSet<codeCountries> codeCountries { get; set; }
        public virtual DbSet<codeCountriesConfigurations> codeCountriesConfigurations { get; set; }
        public virtual DbSet<codeCountriesLanguages> codeCountriesLanguages { get; set; }
        public virtual DbSet<codeDepartments> codeDepartments { get; set; }
        public virtual DbSet<codeDepartmentsConfigurations> codeDepartmentsConfigurations { get; set; }
        public virtual DbSet<codeDepartmentsLanguages> codeDepartmentsLanguages { get; set; }
        public virtual DbSet<codeDutyStations> codeDutyStations { get; set; }
        public virtual DbSet<codeDutyStationsConfigurations> codeDutyStationsConfigurations { get; set; }
        public virtual DbSet<codeDutyStationsLanguages> codeDutyStationsLanguages { get; set; }
        public virtual DbSet<codeElectionCorrespondenceType> codeElectionCorrespondenceType { get; set; }
        public virtual DbSet<codeElectionCorrespondenceTypeLanguage> codeElectionCorrespondenceTypeLanguage { get; set; }
        public virtual DbSet<codeFactors> codeFactors { get; set; }
        public virtual DbSet<codeFactorsLanguages> codeFactorsLanguages { get; set; }
        public virtual DbSet<codeIntervalType> codeIntervalType { get; set; }
        public virtual DbSet<codeIntervalTypeLanguage> codeIntervalTypeLanguage { get; set; }
        public virtual DbSet<codeJobTitles> codeJobTitles { get; set; }
        public virtual DbSet<codeJobTitlesLanguages> codeJobTitlesLanguages { get; set; }
        public virtual DbSet<codeLanguages> codeLanguages { get; set; }
        public virtual DbSet<codeLocations> codeLocations { get; set; }
        public virtual DbSet<codeLocationsLanguages> codeLocationsLanguages { get; set; }
        public virtual DbSet<codeMenus> codeMenus { get; set; }
        public virtual DbSet<codeMenusLanguages> codeMenusLanguages { get; set; }
        public virtual DbSet<codeNotificationLanguages> codeNotificationLanguages { get; set; }
        public virtual DbSet<codeNotifications> codeNotifications { get; set; }
        public virtual DbSet<codeOffices> codeOffices { get; set; }
        public virtual DbSet<codeOfficesConfigurations> codeOfficesConfigurations { get; set; }
        public virtual DbSet<codeOfficesLanguages> codeOfficesLanguages { get; set; }
        public virtual DbSet<codeOperations> codeOperations { get; set; }
        public virtual DbSet<codeOperationsLanguages> codeOperationsLanguages { get; set; }
        public virtual DbSet<codeOrganizations> codeOrganizations { get; set; }
        public virtual DbSet<codeOrganizationsInstances> codeOrganizationsInstances { get; set; }
        public virtual DbSet<codeOrganizationsInstancesLanguages> codeOrganizationsInstancesLanguages { get; set; }
        public virtual DbSet<codeOrganizationsLanguages> codeOrganizationsLanguages { get; set; }
        public virtual DbSet<codePrinters> codePrinters { get; set; }
        public virtual DbSet<codeReferralStatus> codeReferralStatus { get; set; }
        public virtual DbSet<codeReferralStatusLanguage> codeReferralStatusLanguage { get; set; }
        public virtual DbSet<codeSitemaps> codeSitemaps { get; set; }
        public virtual DbSet<codeSitemapsLanguages> codeSitemapsLanguages { get; set; }
        public virtual DbSet<codeTables> codeTables { get; set; }
        public virtual DbSet<codeTablesValues> codeTablesValues { get; set; }
        public virtual DbSet<codeTablesValuesConfigurations> codeTablesValuesConfigurations { get; set; }
        public virtual DbSet<codeTablesValuesLanguages> codeTablesValuesLanguages { get; set; }
        public virtual DbSet<codeTelecomCompany> codeTelecomCompany { get; set; }
        public virtual DbSet<codeTelecomCompanyLanguages> codeTelecomCompanyLanguages { get; set; }
        public virtual DbSet<codeTelecomCompanyOperation> codeTelecomCompanyOperation { get; set; }
        public virtual DbSet<codeWorkingDaysConfigurations> codeWorkingDaysConfigurations { get; set; }
        public virtual DbSet<configFocalPoint> configFocalPoint { get; set; }
        public virtual DbSet<configFocalPointStaff> configFocalPointStaff { get; set; }
        public virtual DbSet<configTelecomCompanyOperation> configTelecomCompanyOperation { get; set; }
        public virtual DbSet<PPADependantList> PPADependantList { get; set; }
        public virtual DbSet<PPADistributionList> PPADistributionList { get; set; }
        public virtual DbSet<PPAFileAccess> PPAFileAccess { get; set; }
        public virtual DbSet<PPAFileVersion> PPAFileVersion { get; set; }
        public virtual DbSet<PPAFileVersionStatusHistory> PPAFileVersionStatusHistory { get; set; }
        public virtual DbSet<PPAOriginalFile> PPAOriginalFile { get; set; }
        public virtual DbSet<PPAPredefinedSteps> PPAPredefinedSteps { get; set; }
        public virtual DbSet<PPAReviewerList> PPAReviewerList { get; set; }
        public virtual DbSet<ProjectPartnershipAgreement> ProjectPartnershipAgreement { get; set; }
    }
}