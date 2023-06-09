
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace COV_DAL.Model
{
    
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using RES_Repo.Globalization;
    using System.ComponentModel.DataAnnotations.Schema;
    using Portal_BL.Library;
    
    public partial class codeTablesValue
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public codeTablesValue()
        {
            this.codeTablesValuesLanguages = new HashSet<codeTablesValuesLanguage>();
            this.codeTablesValuesConfigurations = new HashSet<codeTablesValuesConfiguration>();
        }
    
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ValueGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid ValueGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "TableGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid TableGUID{ get; set; }
    	
        [Display(Name = "FactorGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> FactorGUID{ get; set; }
    	
        [Display(Name = "SortID", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> SortID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public bool Active{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(8, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "codeTablesValuesRowVersion", ResourceType = typeof(resxDbFields))]
    	public byte[] codeTablesValuesRowVersion{ get; set; }
    	
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    
        public virtual codeTable codeTable { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<codeTablesValuesLanguage> codeTablesValuesLanguages { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<codeTablesValuesConfiguration> codeTablesValuesConfigurations { get; set; }
    }
}
