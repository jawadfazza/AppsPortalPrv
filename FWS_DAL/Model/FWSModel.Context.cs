﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FWS_DAL.Model
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class FWSEntities : DbContext
    {
        public FWSEntities()
            : base("name=FWSEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<code4WSAccessStatus> code4WSAccessStatus { get; set; }
        public virtual DbSet<code4WSActivity> code4WSActivity { get; set; }
        public virtual DbSet<code4WSActivityTag> code4WSActivityTag { get; set; }
        public virtual DbSet<code4WSBeneficiaryType> code4WSBeneficiaryType { get; set; }
        public virtual DbSet<code4WSCamps> code4WSCamps { get; set; }
        public virtual DbSet<code4WSFunding> code4WSFunding { get; set; }
        public virtual DbSet<code4WSHub> code4WSHub { get; set; }
        public virtual DbSet<code4WSLocation> code4WSLocation { get; set; }
        public virtual DbSet<code4WSOrgTypeByHub> code4WSOrgTypeByHub { get; set; }
        public virtual DbSet<code4WSPartner> code4WSPartner { get; set; }
        public virtual DbSet<code4WSSeverityRanking> code4WSSeverityRanking { get; set; }
        public virtual DbSet<v_MasterTable> v_MasterTable { get; set; }
        public virtual DbSet<userPersonalDetailsLanguage> userPersonalDetailsLanguages { get; set; }
        public virtual DbSet<code4WSFacility> code4WSFacility { get; set; }
        public virtual DbSet<code4WSSubSector> code4WSSubSector { get; set; }
        public virtual DbSet<dataMasterTable> dataMasterTables { get; set; }
        public virtual DbSet<dataPartnerContribution> dataPartnerContributions { get; set; }
        public virtual DbSet<dataPartnerContributionFile> dataPartnerContributionFiles { get; set; }
    }
}
