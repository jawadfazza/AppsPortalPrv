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
    
    public partial class v_TrackWarehouseHasPendingConfirmationItem
    {
        public string WarehouseDescription { get; set; }
        public System.Guid WarehouseGUID { get; set; }
        public string ModelDescription { get; set; }
        public string WarehouseItemDescription { get; set; }
        public Nullable<System.DateTime> ExpectedStartDate { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<System.DateTime> ExpectedReturenedDate { get; set; }
        public System.Guid ItemOutputDetailGUID { get; set; }
        public Nullable<System.Guid> ItemInputDetailGUID { get; set; }
        public string BarcodeNumber { get; set; }
        public string SerialNumber { get; set; }
        public string IMEI { get; set; }
        public string GSM { get; set; }
        public string MAC { get; set; }
        public string FullDeterminants { get; set; }
    }
}
