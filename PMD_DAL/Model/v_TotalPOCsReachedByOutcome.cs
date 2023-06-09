
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PMD_DAL.Model
{
    
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using RES_Repo.Globalization;
    using System.ComponentModel.DataAnnotations.Schema;
    using Portal_BL.Library;
    
    public partial class v_TotalPOCsReachedByOutcome
    {
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "OutcomeGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid OutcomeGUID{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Governorate", ResourceType = typeof(resxDbFields))]
    	public string Governorate{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "admin1Name_en", ResourceType = typeof(resxDbFields))]
    	public string admin1Name_en{ get; set; }
    	
        [Display(Name = "RefTotal", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> RefTotal{ get; set; }
    	
        [Display(Name = "IdpTotal", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> IdpTotal{ get; set; }
    	
        [Display(Name = "IdpRetTotal", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> IdpRetTotal{ get; set; }
    	
        [Display(Name = "RefRetTotal", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> RefRetTotal{ get; set; }
    	
        [Display(Name = "AsrTotal", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> AsrTotal{ get; set; }
    	
        [Display(Name = "HostCommunity", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> HostCommunity{ get; set; }
    	
        [Display(Name = "OtherTotalNes", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> OtherTotalNes{ get; set; }
    	
    }
}
