﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ORG_DAL.Model
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class ORGEntities : DbContext
    {
        public ORGEntities()
            : base("name=ORGEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<dataAdminAccessStaff> dataAdminAccessStaff { get; set; }
        public virtual DbSet<dataPhoneDirectory> dataPhoneDirectory { get; set; }
        public virtual DbSet<dataStaffProfileFeedback> dataStaffProfileFeedback { get; set; }
        public virtual DbSet<dataStaffProfileFeedbackFlow> dataStaffProfileFeedbackFlow { get; set; }
        public virtual DbSet<dataStaffBankAccount> dataStaffBankAccount { get; set; }
        public virtual DbSet<dataStaffCoreDocument> dataStaffCoreDocument { get; set; }
        public virtual DbSet<dataStaffCorePassport> dataStaffCorePassport { get; set; }
        public virtual DbSet<StaffCoreDataBank> StaffCoreDataBank { get; set; }
        public virtual DbSet<StaffCoreDataHistory> StaffCoreDataHistory { get; set; }
        public virtual DbSet<userAccounts> userAccounts { get; set; }
        public virtual DbSet<userPasswords> userPasswords { get; set; }
        public virtual DbSet<userPersonalDetails> userPersonalDetails { get; set; }
        public virtual DbSet<userPersonalDetailsLanguage> userPersonalDetailsLanguage { get; set; }
        public virtual DbSet<userProfiles> userProfiles { get; set; }
        public virtual DbSet<userServiceHistory> userServiceHistory { get; set; }
        public virtual DbSet<codeBank> codeBank { get; set; }
        public virtual DbSet<codeBankLanguage> codeBankLanguage { get; set; }
        public virtual DbSet<codeCountries> codeCountries { get; set; }
        public virtual DbSet<codeCountriesLanguages> codeCountriesLanguages { get; set; }
        public virtual DbSet<codeDepartments> codeDepartments { get; set; }
        public virtual DbSet<codeDepartmentsConfigurations> codeDepartmentsConfigurations { get; set; }
        public virtual DbSet<codeDepartmentsLanguages> codeDepartmentsLanguages { get; set; }
        public virtual DbSet<codeDutyStations> codeDutyStations { get; set; }
        public virtual DbSet<codeDutyStationsConfigurations> codeDutyStationsConfigurations { get; set; }
        public virtual DbSet<codeDutyStationsLanguages> codeDutyStationsLanguages { get; set; }
        public virtual DbSet<codeJobTitles> codeJobTitles { get; set; }
        public virtual DbSet<codeJobTitlesLanguages> codeJobTitlesLanguages { get; set; }
        public virtual DbSet<codeLocations> codeLocations { get; set; }
        public virtual DbSet<codeLocationsLanguages> codeLocationsLanguages { get; set; }
        public virtual DbSet<codeTables> codeTables { get; set; }
        public virtual DbSet<codeTablesValues> codeTablesValues { get; set; }
        public virtual DbSet<codeTablesValuesConfigurations> codeTablesValuesConfigurations { get; set; }
        public virtual DbSet<codeTablesValuesLanguages> codeTablesValuesLanguages { get; set; }
        public virtual DbSet<dataStaffPhone> dataStaffPhone { get; set; }
        public virtual DbSet<v_dataSyriaPhoneDirectory> v_dataSyriaPhoneDirectory { get; set; }
        public virtual DbSet<userPermissions> userPermissions { get; set; }
        public virtual DbSet<dataStaffOnlineTraining> dataStaffOnlineTraining { get; set; }
        public virtual DbSet<dataStaffOnlineTrainingHistory> dataStaffOnlineTrainingHistory { get; set; }
        public virtual DbSet<dataStaffRelative> dataStaffRelative { get; set; }
        public virtual DbSet<v_StaffProfileInformation> v_StaffProfileInformation { get; set; }
        public virtual DbSet<dataStaffServiceProvided> dataStaffServiceProvided { get; set; }
        public virtual DbSet<dataICTServiceAuthUser> dataICTServiceAuthUser { get; set; }
        public virtual DbSet<dataStaffSalaryInAdvance> dataStaffSalaryInAdvance { get; set; }
        public virtual DbSet<dataStaffSalaryInAdvanceFlow> dataStaffSalaryInAdvanceFlow { get; set; }
        public virtual DbSet<dataStaffPosition> dataStaffPosition { get; set; }
        public virtual DbSet<StaffCoreData> StaffCoreData { get; set; }
    
        public virtual int SendEmailHR(string recipients, string copy_recipients, string blind_copy_recipients, string subject, string body, string body_format, string importance, string file_attachments)
        {
            var recipientsParameter = recipients != null ?
                new ObjectParameter("Recipients", recipients) :
                new ObjectParameter("Recipients", typeof(string));
    
            var copy_recipientsParameter = copy_recipients != null ?
                new ObjectParameter("Copy_recipients", copy_recipients) :
                new ObjectParameter("Copy_recipients", typeof(string));
    
            var blind_copy_recipientsParameter = blind_copy_recipients != null ?
                new ObjectParameter("Blind_copy_recipients", blind_copy_recipients) :
                new ObjectParameter("Blind_copy_recipients", typeof(string));
    
            var subjectParameter = subject != null ?
                new ObjectParameter("Subject", subject) :
                new ObjectParameter("Subject", typeof(string));
    
            var bodyParameter = body != null ?
                new ObjectParameter("Body", body) :
                new ObjectParameter("Body", typeof(string));
    
            var body_formatParameter = body_format != null ?
                new ObjectParameter("Body_format", body_format) :
                new ObjectParameter("Body_format", typeof(string));
    
            var importanceParameter = importance != null ?
                new ObjectParameter("Importance", importance) :
                new ObjectParameter("Importance", typeof(string));
    
            var file_attachmentsParameter = file_attachments != null ?
                new ObjectParameter("File_attachments", file_attachments) :
                new ObjectParameter("File_attachments", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SendEmailHR", recipientsParameter, copy_recipientsParameter, blind_copy_recipientsParameter, subjectParameter, bodyParameter, body_formatParameter, importanceParameter, file_attachmentsParameter);
        }
    }
}