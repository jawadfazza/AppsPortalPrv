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
    
    public partial class userNotificationControls
    {
        public System.Guid UserNotificationControlGUID { get; set; }
        public System.Guid NotificationGUID { get; set; }
        public System.Guid UserGUID { get; set; }
        public Nullable<bool> Block { get; set; }
        public bool Active { get; set; }
        public byte[] userNotificationControlsRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
    
        public virtual codeNotifications codeNotifications { get; set; }
        public virtual userAccounts userAccounts { get; set; }
    }
}
