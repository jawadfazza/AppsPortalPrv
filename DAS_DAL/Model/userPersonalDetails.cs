
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DAS_DAL.Model
{
    
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using RES_Repo.Globalization;
    using System.ComponentModel.DataAnnotations.Schema;
    using Portal_BL.Library;
    
    public partial class userPersonalDetails
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public userPersonalDetails()
        {
            this.userPersonalDetailsLanguage = new HashSet<userPersonalDetailsLanguage>();
        }
    
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "UserGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid UserGUID{ get; set; }
    	
        [Display(Name = "CountryGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> CountryGUID{ get; set; }
    	
        [Display(Name = "GenderGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> GenderGUID{ get; set; }
    	
        [DataType(DataType.Date)]
    	[Display(Name = "DateOfBirth", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DateOfBirth{ get; set; }
    	
        [StringLength(5, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "PreferedLanguageID", ResourceType = typeof(resxDbFields))]
    	public string PreferedLanguageID{ get; set; }
    	
        [StringLength(5, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "BloodGroup", ResourceType = typeof(resxDbFields))]
    	public string BloodGroup{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public bool Active{ get; set; }
    	
        public byte[] userPersonalDetailsRowVersion{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }
    	
    
        public virtual userAccounts userAccounts { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<userPersonalDetailsLanguage> userPersonalDetailsLanguage { get; set; }
    }
}
