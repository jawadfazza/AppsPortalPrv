
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EMT_DAL.Model
{
    
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using RES_Repo.Globalization;
    using System.ComponentModel.DataAnnotations.Schema;
    using Portal_BL.Library;
    
    public partial class spAuditHistory_Result
    {
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ExecutionTime", ResourceType = typeof(resxDbFields))]
    	public System.DateTime ExecutionTime{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ActionDescription", ResourceType = typeof(resxDbFields))]
    	public string ActionDescription{ get; set; }
    	
        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "FieldName", ResourceType = typeof(resxDbFields))]
    	public string FieldName{ get; set; }
    	
        [Display(Name = "BeforeChange", ResourceType = typeof(resxDbFields))]
    	public string BeforeChange{ get; set; }
    	
        [Display(Name = "AfterChange", ResourceType = typeof(resxDbFields))]
    	public string AfterChange{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(101, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ExecutedBy", ResourceType = typeof(resxDbFields))]
    	public string ExecutedBy{ get; set; }
    	
        [StringLength(512, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "JobTitleDescription", ResourceType = typeof(resxDbFields))]
    	public string JobTitleDescription{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(256, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "OrganizationInstanceDescription", ResourceType = typeof(resxDbFields))]
    	public string OrganizationInstanceDescription{ get; set; }
    	
    }
}
