
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TBS_DAL.Model
{
    
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using RES_Repo.Globalization;
    using System.ComponentModel.DataAnnotations.Schema;
    using Portal_BL.Library;
    
    public partial class spFactorsBlackBox_Result
    {
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(36, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "FactorTypeID", ResourceType = typeof(resxDbFields))]
    	public string FactorTypeID{ get; set; }
    	
        [Display(Name = "ID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> ID{ get; set; }
    	
        [Display(Name = "Text", ResourceType = typeof(resxDbFields))]
    	public string Text{ get; set; }
    	
        [StringLength(36, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ParentID", ResourceType = typeof(resxDbFields))]
    	public string ParentID{ get; set; }
    	
    }
}
