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
    
    public partial class codeCardIssueReason
    {
        public string IssueCode { get; set; }
        public string IssueDescription { get; set; }
        public Nullable<int> Seq { get; set; }
        public Nullable<bool> Approval { get; set; }
        public bool Active { get; set; }
        public byte[] codeCardIssueReasonRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
    }
}