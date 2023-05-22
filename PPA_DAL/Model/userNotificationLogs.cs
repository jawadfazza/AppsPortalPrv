
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PPA_DAL.Model
{
    
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using RES_Repo.Globalization;
    using System.ComponentModel.DataAnnotations.Schema;
    using Portal_BL.Library;
    
    public partial class userNotificationLogs
    {
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "NotificationLogGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid NotificationLogGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "NotificationGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid NotificationGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "UserGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid UserGUID{ get; set; }
    	
        [Display(Name = "NotifedByUserGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> NotifedByUserGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[DataType(DataType.DateTime)]
    	[Display(Name = "NotifiedDatetime", ResourceType = typeof(resxDbFields))]
    	public System.DateTime NotifiedDatetime{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "NotifiedDatetime", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalNotifiedDatetime { get { return new Portal().LocalTime(this.NotifiedDatetime); } }
    	
        [Display(Name = "Showed", ResourceType = typeof(resxDbFields))]
    	public Nullable<bool> Showed{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public bool Active{ get; set; }
    	
        public byte[] userNotificationLogsRowVersion{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }
    	
    
        public virtual userAccounts userAccounts { get; set; }
        public virtual codeNotifications codeNotifications { get; set; }
    }
}