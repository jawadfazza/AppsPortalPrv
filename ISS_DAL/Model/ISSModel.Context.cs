﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ISS_DAL.Model
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ISSEntities : DbContext
    {
        public ISSEntities()
            : base("name=ISSEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<userAccounts> userAccounts { get; set; }
        public DbSet<userPersonalDetails> userPersonalDetails { get; set; }
        public DbSet<userPersonalDetailsLanguage> userPersonalDetailsLanguage { get; set; }
        public DbSet<codeISSItem> codeISSItem { get; set; }
        public DbSet<codeISSItemLanguage> codeISSItemLanguage { get; set; }
        public DbSet<codeISSStock> codeISSStock { get; set; }
        public DbSet<codeISSStockLanguage> codeISSStockLanguage { get; set; }
        public DbSet<dataItemOverview> dataItemOverview { get; set; }
        public DbSet<dataItemPipeline> dataItemPipeline { get; set; }
        public DbSet<dataItemPipelineUpload> dataItemPipelineUpload { get; set; }
        public DbSet<dataItemStockBalance> dataItemStockBalance { get; set; }
        public DbSet<dataItemStockEmergencyReserve> dataItemStockEmergencyReserve { get; set; }
        public DbSet<dataItemStockEmergencyUpload> dataItemStockEmergencyUpload { get; set; }
        public DbSet<dataTrackStockUpload> dataTrackStockUpload { get; set; }
    }
}
