﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PCA_DAL.Model
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class PCAEntities : DbContext
    {
        public PCAEntities()
            : base("name=PCAEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<codePartnersCapacityAssessmentDoc> codePartnersCapacityAssessmentDoc { get; set; }
        public virtual DbSet<codePartnersCapacityAssessmentDocTitle> codePartnersCapacityAssessmentDocTitle { get; set; }
        public virtual DbSet<codeOrganizations> codeOrganizations { get; set; }
        public virtual DbSet<codePartnersCapacityAssessmentDocLanguage> codePartnersCapacityAssessmentDocLanguage { get; set; }
        public virtual DbSet<codePartnersCapacityAssessmentDocTitleLanguage> codePartnersCapacityAssessmentDocTitleLanguage { get; set; }
        public virtual DbSet<dataPartnersCapacityAssessment> dataPartnersCapacityAssessment { get; set; }
        public virtual DbSet<dataPartnersCapacityAssessmentDocAttach> dataPartnersCapacityAssessmentDocAttach { get; set; }
        public virtual DbSet<dataPartnersCapacityAssessmentDocEvaluation> dataPartnersCapacityAssessmentDocEvaluation { get; set; }
        public virtual DbSet<dataPartnersCapacityAssessmentPartnershipAgency> dataPartnersCapacityAssessmentPartnershipAgency { get; set; }
    }
}
