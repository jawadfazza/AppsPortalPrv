//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AppsPortal.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class dataAuditFields
    {
        public System.Guid AuditFieldGUID { get; set; }
        public System.Guid AuditGUID { get; set; }
        public string TableName { get; set; }
        public string FieldName { get; set; }
        public string BeforeChange { get; set; }
        public string AfterChange { get; set; }
    
        public virtual dataAuditActions dataAuditActions { get; set; }
    }
}
