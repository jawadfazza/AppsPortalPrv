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
    
    public partial class codeActionsCategoriesLanguages
    {
        public System.Guid ActionCategoryLanguageGUID { get; set; }
        public System.Guid ActionCategoryGUID { get; set; }
        public string LanguageID { get; set; }
        public string ActionCategoryDescription { get; set; }
        public bool Active { get; set; }
        public byte[] codeActionsCategoriesLanguagesRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
    
        public virtual codeActionsCategories codeActionsCategories { get; set; }
    }
}
