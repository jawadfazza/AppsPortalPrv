
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
    
    public partial class v_MyOwnBillsDetailed
    {
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "EDMXID", ResourceType = typeof(resxDbFields))]
    	public int EDMXID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "BillGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid BillGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "IsConfirmed", ResourceType = typeof(resxDbFields))]
    	public bool IsConfirmed{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(256, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "BillForTypeDescription", ResourceType = typeof(resxDbFields))]
    	public string BillForTypeDescription{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "UserBillGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid UserBillGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "UserGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid UserGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "BillForMonth", ResourceType = typeof(resxDbFields))]
    	public int BillForMonth{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "BillForYear", ResourceType = typeof(resxDbFields))]
    	public int BillForYear{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "TelecomCompanyDescription", ResourceType = typeof(resxDbFields))]
    	public string TelecomCompanyDescription{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(150, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "OperationDescription", ResourceType = typeof(resxDbFields))]
    	public string OperationDescription{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "CallingNumber", ResourceType = typeof(resxDbFields))]
    	public string CallingNumber{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "CalledNumber", ResourceType = typeof(resxDbFields))]
    	public string CalledNumber{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "CallSource", ResourceType = typeof(resxDbFields))]
    	public int CallSource{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "CallTypeGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid CallTypeGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(256, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "CallTypeDesciption", ResourceType = typeof(resxDbFields))]
    	public string CallTypeDesciption{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "dateTimeConnect", ResourceType = typeof(resxDbFields))]
    	public System.DateTime dateTimeConnect{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "dateTimeDisconnect", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> dateTimeDisconnect{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "dateTimeDisconnect", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocaldateTimeDisconnect { get { return new Portal().LocalTime(this.dateTimeDisconnect); } }
    	
        [Display(Name = "DestinationCountryGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> DestinationCountryGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "DurationInSeconds", ResourceType = typeof(resxDbFields))]
    	public int DurationInSeconds{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "DurationInMinutes", ResourceType = typeof(resxDbFields))]
    	public int DurationInMinutes{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "CallCost", ResourceType = typeof(resxDbFields))]
    	public double CallCost{ get; set; }
    	
        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
    	public string Comments{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "IsPrivate", ResourceType = typeof(resxDbFields))]
    	public bool IsPrivate{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(5, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "BillForTypeLAN", ResourceType = typeof(resxDbFields))]
    	public string BillForTypeLAN{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(5, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "OperationsLanguagesLAN", ResourceType = typeof(resxDbFields))]
    	public string OperationsLanguagesLAN{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(5, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "TelecomCompanyLanguagesLAN", ResourceType = typeof(resxDbFields))]
    	public string TelecomCompanyLanguagesLAN{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(5, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "CallTypeLAN", ResourceType = typeof(resxDbFields))]
    	public string CallTypeLAN{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "dataBillActive", ResourceType = typeof(resxDbFields))]
    	public bool dataBillActive{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "dataUserBillActive", ResourceType = typeof(resxDbFields))]
    	public bool dataUserBillActive{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "dataUserBillDetailActive", ResourceType = typeof(resxDbFields))]
    	public bool dataUserBillDetailActive{ get; set; }
    	
    }
}