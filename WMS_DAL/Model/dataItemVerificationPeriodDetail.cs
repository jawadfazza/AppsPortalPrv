//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WMS_DAL.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class dataItemVerificationPeriodDetail
    {
        public System.Guid ItemVerificationPeriodDetailGUID { get; set; }
        public Nullable<System.Guid> VerificationWarehousePeriodGUID { get; set; }
        public Nullable<System.Guid> ItemInputDetailGUID { get; set; }
        public Nullable<System.DateTime> VerifyDate { get; set; }
        public Nullable<System.Guid> CreateByGUID { get; set; }
        public string Comments { get; set; }
        public bool Active { get; set; }
        public byte[] dataItemVerificationPeriodDetailRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<bool> Archived { get; set; }
    
        public virtual userAccounts userAccounts { get; set; }
        public virtual dataItemInputDetail dataItemInputDetail { get; set; }
        public virtual dataItemVerificationWarehousePeriod dataItemVerificationWarehousePeriod { get; set; }
    }
}
