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
    
    public partial class userStepsHistory
    {
        public System.Guid UserStepsHistoryGUID { get; set; }
        public System.Guid UserProfileGUID { get; set; }
        public string Step { get; set; }
        public Nullable<System.DateTime> FromDate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
        public bool Active { get; set; }
        public byte[] userStepsHistoryRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
    
        public virtual userProfiles userProfiles { get; set; }
    }
}