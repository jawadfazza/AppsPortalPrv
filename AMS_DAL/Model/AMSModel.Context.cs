﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AMS_DAL.Model
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class AMSEntities : DbContext
    {
        public AMSEntities()
            : base("name=AMSEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<dataContactInfo> dataContactInfo { get; set; }
        public virtual DbSet<codeAppointmentType> codeAppointmentType { get; set; }
        public virtual DbSet<codeAppointmentTypeLanguage> codeAppointmentTypeLanguage { get; set; }
        public virtual DbSet<codeDepartments> codeDepartments { get; set; }
        public virtual DbSet<codeDepartmentsLanguages> codeDepartmentsLanguages { get; set; }
        public virtual DbSet<codeCountries> codeCountries { get; set; }
        public virtual DbSet<codeCountriesLanguages> codeCountriesLanguages { get; set; }
        public virtual DbSet<codeDutyStations> codeDutyStations { get; set; }
        public virtual DbSet<codeDutyStationsLanguages> codeDutyStationsLanguages { get; set; }
        public virtual DbSet<codeOrganizationsInstances> codeOrganizationsInstances { get; set; }
        public virtual DbSet<codeOrganizationsLanguages> codeOrganizationsLanguages { get; set; }
        public virtual DbSet<codeTablesValues> codeTablesValues { get; set; }
        public virtual DbSet<codeTablesValuesLanguages> codeTablesValuesLanguages { get; set; }
        public virtual DbSet<codeWorkingDaysConfigurations> codeWorkingDaysConfigurations { get; set; }
        public virtual DbSet<dataAppointmentCalendarHolded> dataAppointmentCalendarHolded { get; set; }
        public virtual DbSet<dataAppointmentReschedul> dataAppointmentReschedul { get; set; }
        public virtual DbSet<userPermissions> userPermissions { get; set; }
        public virtual DbSet<dataAppointment> dataAppointment { get; set; }
        public virtual DbSet<dataCase> dataCase { get; set; }
        public virtual DbSet<v_CaseAppointments> v_CaseAppointments { get; set; }
        public virtual DbSet<dataAppointmentTypeCalendar> dataAppointmentTypeCalendar { get; set; }
        public virtual DbSet<v_AppointmentContactNumber> v_AppointmentContactNumber { get; set; }
        public virtual DbSet<dataFile> dataFile { get; set; }
        public virtual DbSet<StaffCoreData> StaffCoreData { get; set; }
    }
}
