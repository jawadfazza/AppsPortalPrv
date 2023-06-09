
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
    
    public partial class dataAuditFields
    {
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "AuditFieldGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid AuditFieldGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "AuditGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid AuditGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(256, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "TableName", ResourceType = typeof(resxDbFields))]
    	public string TableName{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "FieldName", ResourceType = typeof(resxDbFields))]
    	public string FieldName{ get; set; }
    	
        [Display(Name = "BeforeChange", ResourceType = typeof(resxDbFields))]
    	public string BeforeChange{ get; set; }
    	
        [Display(Name = "AfterChange", ResourceType = typeof(resxDbFields))]
    	public string AfterChange{ get; set; }
    	
    
        public virtual dataAuditActions dataAuditActions { get; set; }
    }
}
