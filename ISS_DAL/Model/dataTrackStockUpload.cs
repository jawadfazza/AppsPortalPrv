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
    
    public partial class dataTrackStockUpload
    {
        public dataTrackStockUpload()
        {
            this.dataItemStockBalance = new HashSet<dataItemStockBalance>();
        }
    
        public System.Guid TrackStockUploadGUID { get; set; }
        public Nullable<System.DateTime> UploadDate { get; set; }
        public Nullable<bool> IsLastUpload { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.Guid> CreatedByGUID { get; set; }
        public bool Active { get; set; }
        public byte[] dataTrackStockUploadRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<bool> Archived { get; set; }
    
        public virtual userAccounts userAccounts { get; set; }
        public virtual ICollection<dataItemStockBalance> dataItemStockBalance { get; set; }
    }
}
