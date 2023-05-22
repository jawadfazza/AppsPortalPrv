﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PMD_DAL.Model
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class PMDEntities : DbContext
    {
        public PMDEntities()
            : base("name=PMDEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<codedatatest> codedatatest { get; set; }
        public virtual DbSet<dataDashboardFullAccess> dataDashboardFullAccess { get; set; }
        public virtual DbSet<dataDashboardPermission> dataDashboardPermission { get; set; }
        public virtual DbSet<dataHealthWhoCategoryAggregation> dataHealthWhoCategoryAggregation { get; set; }
        public virtual DbSet<dataHealthWhoDiagnosis> dataHealthWhoDiagnosis { get; set; }
        public virtual DbSet<dataPartnerMonitoring2022Attachement> dataPartnerMonitoring2022Attachement { get; set; }
        public virtual DbSet<dataPartnerMonitoring2022DB> dataPartnerMonitoring2022DB { get; set; }
        public virtual DbSet<dataPartnerMonitoring2022IndicatorStatus> dataPartnerMonitoring2022IndicatorStatus { get; set; }
        public virtual DbSet<dataPartnerMonitoringAttachement> dataPartnerMonitoringAttachement { get; set; }
        public virtual DbSet<dataPartnerMonitoringDB> dataPartnerMonitoringDB { get; set; }
        public virtual DbSet<dataPartnerMonitoringIndicatorStatus> dataPartnerMonitoringIndicatorStatus { get; set; }
        public virtual DbSet<dataPartnerMonitoringObjectiveStatus> dataPartnerMonitoringObjectiveStatus { get; set; }
        public virtual DbSet<dataPMD2022DomesticUnitOfAchievement> dataPMD2022DomesticUnitOfAchievement { get; set; }
        public virtual DbSet<dataPMD2022HealthUnitOfAchievement> dataPMD2022HealthUnitOfAchievement { get; set; }
        public virtual DbSet<dataPMD2022HealthWHOAchievement> dataPMD2022HealthWHOAchievement { get; set; }
        public virtual DbSet<dataPMDBulkUpload> dataPMDBulkUpload { get; set; }
        public virtual DbSet<dataPMDClosingBalance> dataPMDClosingBalance { get; set; }
        public virtual DbSet<dataPMDDamagedLostDistributionDetail> dataPMDDamagedLostDistributionDetail { get; set; }
        public virtual DbSet<dataPMDDispatchDetail> dataPMDDispatchDetail { get; set; }
        public virtual DbSet<dataPMDDomesticUnitOfAchievement> dataPMDDomesticUnitOfAchievement { get; set; }
        public virtual DbSet<dataPMDHealthUnitOfAchievement> dataPMDHealthUnitOfAchievement { get; set; }
        public virtual DbSet<dataPMDItemsTransferDetail> dataPMDItemsTransferDetail { get; set; }
        public virtual DbSet<dataPPAForm> dataPPAForm { get; set; }
        public virtual DbSet<dataPPAFormHistory> dataPPAFormHistory { get; set; }
        public virtual DbSet<dataTempAchievementsConfig> dataTempAchievementsConfig { get; set; }
        public virtual DbSet<datatest> datatest { get; set; }
        public virtual DbSet<v_AchievementVsTargetsMain> v_AchievementVsTargetsMain { get; set; }
        public virtual DbSet<v_Bulk2022Extraction> v_Bulk2022Extraction { get; set; }
        public virtual DbSet<v_BulkExtraction> v_BulkExtraction { get; set; }
        public virtual DbSet<v_dataPartnerMonitoringDatabase2022DataTable> v_dataPartnerMonitoringDatabase2022DataTable { get; set; }
        public virtual DbSet<v_dataPartnerMonitoringDatabaseDataTable> v_dataPartnerMonitoringDatabaseDataTable { get; set; }
        public virtual DbSet<v_flatCodes> v_flatCodes { get; set; }
        public virtual DbSet<v_flatCRIItems> v_flatCRIItems { get; set; }
        public virtual DbSet<v_flatHealthCommunityItems> v_flatHealthCommunityItems { get; set; }
        public virtual DbSet<v_flatHealthHealthItems> v_flatHealthHealthItems { get; set; }
        public virtual DbSet<v_flatHealthItems> v_flatHealthItems { get; set; }
        public virtual DbSet<v_PMDCodes> v_PMDCodes { get; set; }
        public virtual DbSet<v_PMDImplementingPartners> v_PMDImplementingPartners { get; set; }
        public virtual DbSet<v_PRMDFlatTable_PowerBI> v_PRMDFlatTable_PowerBI { get; set; }
        public virtual DbSet<v_PRMDFlatTable2022_PowerBI> v_PRMDFlatTable2022_PowerBI { get; set; }
        public virtual DbSet<v_PRMDFlatTableCRI_PowerBI> v_PRMDFlatTableCRI_PowerBI { get; set; }
        public virtual DbSet<v_PRMDFlatTableHealthItmes_PowerBI> v_PRMDFlatTableHealthItmes_PowerBI { get; set; }
        public virtual DbSet<v_tempCodesPMD> v_tempCodesPMD { get; set; }
        public virtual DbSet<v_tempCodesPMD2> v_tempCodesPMD2 { get; set; }
        public virtual DbSet<v_TotalPOCsReachedByOutcome> v_TotalPOCsReachedByOutcome { get; set; }
        public virtual DbSet<v_UpdatedAfterNotVerified> v_UpdatedAfterNotVerified { get; set; }
        public virtual DbSet<codeOrganizationsInstances> codeOrganizationsInstances { get; set; }
        public virtual DbSet<codeOrganizationsInstancesLanguages> codeOrganizationsInstancesLanguages { get; set; }
        public virtual DbSet<codePmd2022CommunityCenter> codePmd2022CommunityCenter { get; set; }
        public virtual DbSet<codePmd2022Indicator> codePmd2022Indicator { get; set; }
        public virtual DbSet<codePmd2022IndicatorLanguage> codePmd2022IndicatorLanguage { get; set; }
        public virtual DbSet<codePmd2022IndicatorStatus> codePmd2022IndicatorStatus { get; set; }
        public virtual DbSet<codePmd2022IndicatorStatusLanguage> codePmd2022IndicatorStatusLanguage { get; set; }
        public virtual DbSet<codePmd2022Output> codePmd2022Output { get; set; }
        public virtual DbSet<codePmd2022OutputLanguage> codePmd2022OutputLanguage { get; set; }
        public virtual DbSet<codePmd2022UnitOfAchievement> codePmd2022UnitOfAchievement { get; set; }
        public virtual DbSet<codePmd2022UnitOfAchievementLanguages> codePmd2022UnitOfAchievementLanguages { get; set; }
        public virtual DbSet<codePmdCommunityCenter> codePmdCommunityCenter { get; set; }
        public virtual DbSet<codePmdExcelColumns2022Config> codePmdExcelColumns2022Config { get; set; }
        public virtual DbSet<codePmdExcelColumnsConfig> codePmdExcelColumnsConfig { get; set; }
        public virtual DbSet<codePMDHealthWhoDiagnosis> codePMDHealthWhoDiagnosis { get; set; }
        public virtual DbSet<codePmdIndicator> codePmdIndicator { get; set; }
        public virtual DbSet<codePmdIndicatorLanguage> codePmdIndicatorLanguage { get; set; }
        public virtual DbSet<codePmdObjective> codePmdObjective { get; set; }
        public virtual DbSet<codePmdObjectiveLanguage> codePmdObjectiveLanguage { get; set; }
        public virtual DbSet<codePmdObjectiveStatus> codePmdObjectiveStatus { get; set; }
        public virtual DbSet<codePmdObjectiveStatusLanguage> codePmdObjectiveStatusLanguage { get; set; }
        public virtual DbSet<codePmdOutcome> codePmdOutcome { get; set; }
        public virtual DbSet<codePmdOutcomeLanguage> codePmdOutcomeLanguage { get; set; }
        public virtual DbSet<codePmdOutput> codePmdOutput { get; set; }
        public virtual DbSet<codePmdOutputLanguage> codePmdOutputLanguage { get; set; }
        public virtual DbSet<codePmdPartner> codePmdPartner { get; set; }
        public virtual DbSet<codePmdPartnerLanguage> codePmdPartnerLanguage { get; set; }
        public virtual DbSet<codePmdPillar> codePmdPillar { get; set; }
        public virtual DbSet<codePmdPillarOutcome> codePmdPillarOutcome { get; set; }
        public virtual DbSet<codePmdUnitOfAchievement> codePmdUnitOfAchievement { get; set; }
        public virtual DbSet<codePmdUnitOfAchievementLanguages> codePmdUnitOfAchievementLanguages { get; set; }
        public virtual DbSet<codeOchaLocation> codeOchaLocation { get; set; }
        public virtual DbSet<v_OchaLocationsLevelOneFour> v_OchaLocationsLevelOneFour { get; set; }
        public virtual DbSet<codeOchaLocationGovernorate> codeOchaLocationGovernorate { get; set; }
        public virtual DbSet<codePmdWarehouseLanguage> codePmdWarehouseLanguage { get; set; }
        public virtual DbSet<dataPMDDamagedLostDistribution> dataPMDDamagedLostDistribution { get; set; }
        public virtual DbSet<dataPMDDispatch> dataPMDDispatch { get; set; }
        public virtual DbSet<dataPMDItemsTransfer> dataPMDItemsTransfer { get; set; }
        public virtual DbSet<codePmd2023UnitOfAchievement> codePmd2023UnitOfAchievement { get; set; }
        public virtual DbSet<codePmd2023UnitOfAchievementLanguages> codePmd2023UnitOfAchievementLanguages { get; set; }
        public virtual DbSet<codePmdWarehouse> codePmdWarehouse { get; set; }
    
        public virtual int CalculateClosingBalance(Nullable<int> yearVal, Nullable<System.DateTime> startDate, Nullable<System.DateTime> endDate, Nullable<System.Guid> organizationInstanceGUID)
        {
            var yearValParameter = yearVal.HasValue ?
                new ObjectParameter("YearVal", yearVal) :
                new ObjectParameter("YearVal", typeof(int));
    
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("StartDate", startDate) :
                new ObjectParameter("StartDate", typeof(System.DateTime));
    
            var endDateParameter = endDate.HasValue ?
                new ObjectParameter("EndDate", endDate) :
                new ObjectParameter("EndDate", typeof(System.DateTime));
    
            var organizationInstanceGUIDParameter = organizationInstanceGUID.HasValue ?
                new ObjectParameter("OrganizationInstanceGUID", organizationInstanceGUID) :
                new ObjectParameter("OrganizationInstanceGUID", typeof(System.Guid));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("CalculateClosingBalance", yearValParameter, startDateParameter, endDateParameter, organizationInstanceGUIDParameter);
        }
    
        public virtual ObjectResult<PMD_ClosingBalancePivotTable_Result> PMD_ClosingBalancePivotTable(Nullable<int> year, Nullable<System.Guid> organizationInstanceGUID, string governorates)
        {
            var yearParameter = year.HasValue ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(int));
    
            var organizationInstanceGUIDParameter = organizationInstanceGUID.HasValue ?
                new ObjectParameter("OrganizationInstanceGUID", organizationInstanceGUID) :
                new ObjectParameter("OrganizationInstanceGUID", typeof(System.Guid));
    
            var governoratesParameter = governorates != null ?
                new ObjectParameter("Governorates", governorates) :
                new ObjectParameter("Governorates", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<PMD_ClosingBalancePivotTable_Result>("PMD_ClosingBalancePivotTable", yearParameter, organizationInstanceGUIDParameter, governoratesParameter);
        }
    
        public virtual ObjectResult<PMD_DamagedLostDistributionDetails_Result> PMD_DamagedLostDistributionDetails(Nullable<System.DateTime> startDate, Nullable<System.DateTime> endDate, string organizationInstances, string governorates)
        {
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("StartDate", startDate) :
                new ObjectParameter("StartDate", typeof(System.DateTime));
    
            var endDateParameter = endDate.HasValue ?
                new ObjectParameter("EndDate", endDate) :
                new ObjectParameter("EndDate", typeof(System.DateTime));
    
            var organizationInstancesParameter = organizationInstances != null ?
                new ObjectParameter("OrganizationInstances", organizationInstances) :
                new ObjectParameter("OrganizationInstances", typeof(string));
    
            var governoratesParameter = governorates != null ?
                new ObjectParameter("Governorates", governorates) :
                new ObjectParameter("Governorates", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<PMD_DamagedLostDistributionDetails_Result>("PMD_DamagedLostDistributionDetails", startDateParameter, endDateParameter, organizationInstancesParameter, governoratesParameter);
        }
    
        public virtual ObjectResult<PMD_DamagedLostDistributionPivotTable_Result> PMD_DamagedLostDistributionPivotTable(Nullable<System.DateTime> startDate, Nullable<System.DateTime> endDate, Nullable<System.Guid> organizationInstanceGUID, string governorates)
        {
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("StartDate", startDate) :
                new ObjectParameter("StartDate", typeof(System.DateTime));
    
            var endDateParameter = endDate.HasValue ?
                new ObjectParameter("EndDate", endDate) :
                new ObjectParameter("EndDate", typeof(System.DateTime));
    
            var organizationInstanceGUIDParameter = organizationInstanceGUID.HasValue ?
                new ObjectParameter("OrganizationInstanceGUID", organizationInstanceGUID) :
                new ObjectParameter("OrganizationInstanceGUID", typeof(System.Guid));
    
            var governoratesParameter = governorates != null ?
                new ObjectParameter("Governorates", governorates) :
                new ObjectParameter("Governorates", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<PMD_DamagedLostDistributionPivotTable_Result>("PMD_DamagedLostDistributionPivotTable", startDateParameter, endDateParameter, organizationInstanceGUIDParameter, governoratesParameter);
        }
    
        public virtual ObjectResult<PMD_DispatchDetails_Result> PMD_DispatchDetails(Nullable<System.DateTime> startDate, Nullable<System.DateTime> endDate, string organizationInstances, string governorates)
        {
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("StartDate", startDate) :
                new ObjectParameter("StartDate", typeof(System.DateTime));
    
            var endDateParameter = endDate.HasValue ?
                new ObjectParameter("EndDate", endDate) :
                new ObjectParameter("EndDate", typeof(System.DateTime));
    
            var organizationInstancesParameter = organizationInstances != null ?
                new ObjectParameter("OrganizationInstances", organizationInstances) :
                new ObjectParameter("OrganizationInstances", typeof(string));
    
            var governoratesParameter = governorates != null ?
                new ObjectParameter("Governorates", governorates) :
                new ObjectParameter("Governorates", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<PMD_DispatchDetails_Result>("PMD_DispatchDetails", startDateParameter, endDateParameter, organizationInstancesParameter, governoratesParameter);
        }
    
        public virtual ObjectResult<PMD_DispatchPivotTable_Result> PMD_DispatchPivotTable(Nullable<System.DateTime> startDate, Nullable<System.DateTime> endDate, Nullable<System.Guid> organizationInstanceGUID, string governorates)
        {
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("StartDate", startDate) :
                new ObjectParameter("StartDate", typeof(System.DateTime));
    
            var endDateParameter = endDate.HasValue ?
                new ObjectParameter("EndDate", endDate) :
                new ObjectParameter("EndDate", typeof(System.DateTime));
    
            var organizationInstanceGUIDParameter = organizationInstanceGUID.HasValue ?
                new ObjectParameter("OrganizationInstanceGUID", organizationInstanceGUID) :
                new ObjectParameter("OrganizationInstanceGUID", typeof(System.Guid));
    
            var governoratesParameter = governorates != null ?
                new ObjectParameter("Governorates", governorates) :
                new ObjectParameter("Governorates", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<PMD_DispatchPivotTable_Result>("PMD_DispatchPivotTable", startDateParameter, endDateParameter, organizationInstanceGUIDParameter, governoratesParameter);
        }
    
        public virtual ObjectResult<PMD_DistributionDetails_Result> PMD_DistributionDetails(Nullable<System.DateTime> startDate, Nullable<System.DateTime> endDate, string organizationInstances, string governorates)
        {
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("StartDate", startDate) :
                new ObjectParameter("StartDate", typeof(System.DateTime));
    
            var endDateParameter = endDate.HasValue ?
                new ObjectParameter("EndDate", endDate) :
                new ObjectParameter("EndDate", typeof(System.DateTime));
    
            var organizationInstancesParameter = organizationInstances != null ?
                new ObjectParameter("OrganizationInstances", organizationInstances) :
                new ObjectParameter("OrganizationInstances", typeof(string));
    
            var governoratesParameter = governorates != null ?
                new ObjectParameter("Governorates", governorates) :
                new ObjectParameter("Governorates", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<PMD_DistributionDetails_Result>("PMD_DistributionDetails", startDateParameter, endDateParameter, organizationInstancesParameter, governoratesParameter);
        }
    
        public virtual ObjectResult<PMD_DistributionPivotTable_Result> PMD_DistributionPivotTable(Nullable<System.DateTime> startDate, Nullable<System.DateTime> endDate, Nullable<System.Guid> organizationInstanceGUID, string governorates)
        {
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("StartDate", startDate) :
                new ObjectParameter("StartDate", typeof(System.DateTime));
    
            var endDateParameter = endDate.HasValue ?
                new ObjectParameter("EndDate", endDate) :
                new ObjectParameter("EndDate", typeof(System.DateTime));
    
            var organizationInstanceGUIDParameter = organizationInstanceGUID.HasValue ?
                new ObjectParameter("OrganizationInstanceGUID", organizationInstanceGUID) :
                new ObjectParameter("OrganizationInstanceGUID", typeof(System.Guid));
    
            var governoratesParameter = governorates != null ?
                new ObjectParameter("Governorates", governorates) :
                new ObjectParameter("Governorates", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<PMD_DistributionPivotTable_Result>("PMD_DistributionPivotTable", startDateParameter, endDateParameter, organizationInstanceGUIDParameter, governoratesParameter);
        }
    
        public virtual ObjectResult<PMD_ItemsTransferDetails_Result> PMD_ItemsTransferDetails(Nullable<System.DateTime> startDate, Nullable<System.DateTime> endDate, string organizationInstances, string governorates)
        {
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("StartDate", startDate) :
                new ObjectParameter("StartDate", typeof(System.DateTime));
    
            var endDateParameter = endDate.HasValue ?
                new ObjectParameter("EndDate", endDate) :
                new ObjectParameter("EndDate", typeof(System.DateTime));
    
            var organizationInstancesParameter = organizationInstances != null ?
                new ObjectParameter("OrganizationInstances", organizationInstances) :
                new ObjectParameter("OrganizationInstances", typeof(string));
    
            var governoratesParameter = governorates != null ?
                new ObjectParameter("Governorates", governorates) :
                new ObjectParameter("Governorates", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<PMD_ItemsTransferDetails_Result>("PMD_ItemsTransferDetails", startDateParameter, endDateParameter, organizationInstancesParameter, governoratesParameter);
        }
    
        public virtual ObjectResult<PMD_ItemsTransferPivotTable_Result> PMD_ItemsTransferPivotTable(Nullable<System.DateTime> startDate, Nullable<System.DateTime> endDate, Nullable<System.Guid> organizationInstanceGUID, string governorates)
        {
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("StartDate", startDate) :
                new ObjectParameter("StartDate", typeof(System.DateTime));
    
            var endDateParameter = endDate.HasValue ?
                new ObjectParameter("EndDate", endDate) :
                new ObjectParameter("EndDate", typeof(System.DateTime));
    
            var organizationInstanceGUIDParameter = organizationInstanceGUID.HasValue ?
                new ObjectParameter("OrganizationInstanceGUID", organizationInstanceGUID) :
                new ObjectParameter("OrganizationInstanceGUID", typeof(System.Guid));
    
            var governoratesParameter = governorates != null ?
                new ObjectParameter("Governorates", governorates) :
                new ObjectParameter("Governorates", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<PMD_ItemsTransferPivotTable_Result>("PMD_ItemsTransferPivotTable", startDateParameter, endDateParameter, organizationInstanceGUIDParameter, governoratesParameter);
        }
    
        public virtual int CalculateClosingBalanceMonthly(Nullable<int> yearVal, Nullable<int> monthVal, Nullable<System.DateTime> startDate, Nullable<System.DateTime> endDate, Nullable<System.Guid> organizationInstanceGUID)
        {
            var yearValParameter = yearVal.HasValue ?
                new ObjectParameter("YearVal", yearVal) :
                new ObjectParameter("YearVal", typeof(int));
    
            var monthValParameter = monthVal.HasValue ?
                new ObjectParameter("MonthVal", monthVal) :
                new ObjectParameter("MonthVal", typeof(int));
    
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("StartDate", startDate) :
                new ObjectParameter("StartDate", typeof(System.DateTime));
    
            var endDateParameter = endDate.HasValue ?
                new ObjectParameter("EndDate", endDate) :
                new ObjectParameter("EndDate", typeof(System.DateTime));
    
            var organizationInstanceGUIDParameter = organizationInstanceGUID.HasValue ?
                new ObjectParameter("OrganizationInstanceGUID", organizationInstanceGUID) :
                new ObjectParameter("OrganizationInstanceGUID", typeof(System.Guid));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("CalculateClosingBalanceMonthly", yearValParameter, monthValParameter, startDateParameter, endDateParameter, organizationInstanceGUIDParameter);
        }
    }
}