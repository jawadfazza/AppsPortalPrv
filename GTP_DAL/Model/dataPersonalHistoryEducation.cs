
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
    
    public partial class dataPersonalHistoryEducation
    {
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "GTPPHEducationGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid GTPPHEducationGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "GTPApplicationGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid GTPApplicationGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ExactTitleOfDegreeOrCertification", ResourceType = typeof(resxDbFields))]
    	public string ExactTitleOfDegreeOrCertification{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[DataType(DataType.DateTime)]
    	[Display(Name = "BegintDate", ResourceType = typeof(resxDbFields))]
    	public System.DateTime BegintDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "BegintDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalBegintDate { get { return new Portal().LocalTime(this.BegintDate); } }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "EndDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> EndDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "EndDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalEndDate { get { return new Portal().LocalTime(this.EndDate); } }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(500, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "MainMajorOrTopic", ResourceType = typeof(resxDbFields))]
    	public string MainMajorOrTopic{ get; set; }
    	
        [StringLength(500, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "OtherMajorsOrTopics", ResourceType = typeof(resxDbFields))]
    	public string OtherMajorsOrTopics{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "EducationLevelGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid EducationLevelGUID{ get; set; }
    	
        [Display(Name = "IfOtherEducationDescription", ResourceType = typeof(resxDbFields))]
    	public string IfOtherEducationDescription{ get; set; }
    	
        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "MainLanguageOfStudyGUID", ResourceType = typeof(resxDbFields))]
    	public string MainLanguageOfStudyGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "SchoolNameDescription", ResourceType = typeof(resxDbFields))]
    	public string SchoolNameDescription{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "CityGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid CityGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "CountryGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid CountryGUID{ get; set; }
    	
        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
    	public string Comments{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "IsEducationCompleted", ResourceType = typeof(resxDbFields))]
    	public bool IsEducationCompleted{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public bool Active{ get; set; }
    	
        public byte[] dataPersonalHistoryEducationRowVersion{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }
    	
    
        public virtual dataGTPApplication dataGTPApplication { get; set; }
    }
}
