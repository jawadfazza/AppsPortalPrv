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
    
    public partial class codePmdUnitOfAchievementLanguages
    {
        public System.Guid UnitOfAchievementLanguageGUID { get; set; }
        public System.Guid UnitOfAchievementGUID { get; set; }
        public string LanguageID { get; set; }
        public string UnitOfAchievementDescription { get; set; }
        public string UnitOfAchievementGuidance { get; set; }
        public bool Active { get; set; }
        public byte[] codePmdUnitOfAchievementLanguagesRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
    
        public virtual codePmdUnitOfAchievement codePmdUnitOfAchievement { get; set; }
    }
}
