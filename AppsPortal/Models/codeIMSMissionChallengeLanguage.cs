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
    
    public partial class codeIMSMissionChallengeLanguage
    {
        public System.Guid MissionChallengeLanguageGUID { get; set; }
        public Nullable<System.Guid> MissionChallengeGUID { get; set; }
        public string LanguageID { get; set; }
        public string MissionChallengeDescription { get; set; }
        public bool Active { get; set; }
        public byte[] codeMissionChallengeLanguageRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<bool> Archived { get; set; }
    
        public virtual codeIMSMissionChallenge codeIMSMissionChallenge { get; set; }
    }
}
