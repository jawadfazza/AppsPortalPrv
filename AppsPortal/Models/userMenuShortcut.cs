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
    
    public partial class userMenuShortcut
    {
        public System.Guid UserMenuShortcutGUID { get; set; }
        public Nullable<System.Guid> MenuGUID { get; set; }
        public Nullable<System.Guid> UserGUID { get; set; }
        public Nullable<int> SortID { get; set; }
        public bool Active { get; set; }
        public byte[] userMenuShortcutRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
    
        public virtual codeMenus codeMenus { get; set; }
        public virtual userAccounts userAccounts { get; set; }
    }
}
