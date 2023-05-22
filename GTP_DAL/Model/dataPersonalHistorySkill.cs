
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GTP_DAL.Model
{
    
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using RES_Repo.Globalization;
    using System.ComponentModel.DataAnnotations.Schema;
    using Portal_BL.Library;
    
    public partial class dataPersonalHistorySkill
    {
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "GTPPHSkillGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid GTPPHSkillGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "GTPApplicationGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid GTPApplicationGUID{ get; set; }
    	
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "SkillDescription", ResourceType = typeof(resxDbFields))]
    	public string SkillDescription{ get; set; }
    	
        [Display(Name = "SkillLevelGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> SkillLevelGUID{ get; set; }
    	
        [Display(Name = "SkillNumberOfYears", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> SkillNumberOfYears{ get; set; }
    	
        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
    	public string Comments{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public bool Active{ get; set; }
    	
        public byte[] dataPersonalHistorySkillRowVersion{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }
    	
    
        public virtual dataGTPApplication dataGTPApplication { get; set; }
    }
}