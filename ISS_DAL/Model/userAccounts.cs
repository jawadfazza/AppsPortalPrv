//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class userAccounts
    {
        public userAccounts()
        {
            this.dataItemPipelineUpload = new HashSet<dataItemPipelineUpload>();
            this.dataItemStockEmergencyUpload = new HashSet<dataItemStockEmergencyUpload>();
            this.dataTrackStockUpload = new HashSet<dataTrackStockUpload>();
        }
    
        public System.Guid UserGUID { get; set; }
        public string TimeZone { get; set; }
        public System.DateTime RequestedOn { get; set; }
        public System.Guid SecurityQuestionGUID { get; set; }
        public string SecurityAnswer { get; set; }
        public int AccountStatusID { get; set; }
        public bool IsFirstLogin { get; set; }
        public Nullable<bool> HasPhoto { get; set; }
        public bool Active { get; set; }
        public byte[] userAccountsRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
    
        public virtual ICollection<dataItemPipelineUpload> dataItemPipelineUpload { get; set; }
        public virtual ICollection<dataItemStockEmergencyUpload> dataItemStockEmergencyUpload { get; set; }
        public virtual ICollection<dataTrackStockUpload> dataTrackStockUpload { get; set; }
        public virtual userPersonalDetails userPersonalDetails { get; set; }
    }
}
