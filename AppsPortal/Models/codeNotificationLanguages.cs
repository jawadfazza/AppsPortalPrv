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
    
    public partial class codeNotificationLanguages
    {
        public System.Guid NotificationLanguageGUID { get; set; }
        public System.Guid NotificationGUID { get; set; }
        public string LanguageID { get; set; }
        public string TitleTemplete { get; set; }
        public string DetailsTemplete { get; set; }
        public string MailTemplete { get; set; }
        public bool Active { get; set; }
        public byte[] codeNotificationLanguagesRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
    
        public virtual codeNotifications codeNotifications { get; set; }
    }
}
