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
    
    public partial class dataItemPipeline
    {
        public System.Guid ItemPipelineGUID { get; set; }
        public Nullable<System.Guid> ItemPipelineUploadGUID { get; set; }
        public Nullable<System.Guid> ItemGUID { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<System.DateTime> ETA { get; set; }
        public bool Active { get; set; }
        public byte[] dataItemPipelineRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<bool> Archived { get; set; }
        public Nullable<System.Guid> StockGUID { get; set; }
    
        public virtual codeISSItem codeISSItem { get; set; }
        public virtual codeISSStock codeISSStock { get; set; }
        public virtual dataItemPipelineUpload dataItemPipelineUpload { get; set; }
    }
}
