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
    
    public partial class codeNotifications
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public codeNotifications()
        {
            this.userNotificationControls = new HashSet<userNotificationControls>();
            this.userNotificationLogs = new HashSet<userNotificationLogs>();
            this.codeNotificationLanguages = new HashSet<codeNotificationLanguages>();
        }
    
        public System.Guid NotificationGUID { get; set; }
        public bool UserProfileIcon { get; set; }
        public string Icon { get; set; }
        public System.Guid ApplicationGUID { get; set; }
        public string RedirectURL { get; set; }
        public bool Active { get; set; }
        public byte[] codeNotificationsRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<userNotificationControls> userNotificationControls { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<userNotificationLogs> userNotificationLogs { get; set; }
        public virtual codeApplications codeApplications { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<codeNotificationLanguages> codeNotificationLanguages { get; set; }
    }
}
