﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GTP_DAL.Model
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class GTPEntities : DbContext
    {
        public GTPEntities()
            : base("name=GTPEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<codeGTPCategory> codeGTPCategories { get; set; }
        public virtual DbSet<userPersonalDetail> userPersonalDetails { get; set; }
        public virtual DbSet<userPersonalDetailsLanguage> userPersonalDetailsLanguages { get; set; }
        public virtual DbSet<userServiceHistory> userServiceHistories { get; set; }
        public virtual DbSet<dataGTPApplication> dataGTPApplications { get; set; }
        public virtual DbSet<dataPersonalHistoryConfirmationAndConsent> dataPersonalHistoryConfirmationAndConsents { get; set; }
        public virtual DbSet<dataPersonalHistoryContactInfo> dataPersonalHistoryContactInfoes { get; set; }
        public virtual DbSet<dataPersonalHistoryEducation> dataPersonalHistoryEducations { get; set; }
        public virtual DbSet<dataPersonalHistoryEmailAddress> dataPersonalHistoryEmailAddresses { get; set; }
        public virtual DbSet<dataPersonalHistoryGeneralInfo> dataPersonalHistoryGeneralInfoes { get; set; }
        public virtual DbSet<dataPersonalHistoryLanguage> dataPersonalHistoryLanguages { get; set; }
        public virtual DbSet<dataPersonalHistoryLetterOfInterest> dataPersonalHistoryLetterOfInterests { get; set; }
        public virtual DbSet<dataPersonalHistoryLicenceAndCertificate> dataPersonalHistoryLicenceAndCertificates { get; set; }
        public virtual DbSet<dataPersonalHistoryNationalityInfo> dataPersonalHistoryNationalityInfoes { get; set; }
        public virtual DbSet<dataPersonalHistoryPersonalInfo> dataPersonalHistoryPersonalInfoes { get; set; }
        public virtual DbSet<dataPersonalHistoryPhoneNumber> dataPersonalHistoryPhoneNumbers { get; set; }
        public virtual DbSet<dataPersonalHistoryProfessionalReference> dataPersonalHistoryProfessionalReferences { get; set; }
        public virtual DbSet<dataPersonalHistoryQuestionnaire> dataPersonalHistoryQuestionnaires { get; set; }
        public virtual DbSet<dataPersonalHistoryRelative> dataPersonalHistoryRelatives { get; set; }
        public virtual DbSet<dataPersonalHistoryRelativeWorker> dataPersonalHistoryRelativeWorkers { get; set; }
        public virtual DbSet<dataPersonalHistorySkill> dataPersonalHistorySkills { get; set; }
        public virtual DbSet<dataPersonalHistorySpecializedTraining> dataPersonalHistorySpecializedTrainings { get; set; }
        public virtual DbSet<dataPersonalHistoryWorkExperience> dataPersonalHistoryWorkExperiences { get; set; }
    }
}
