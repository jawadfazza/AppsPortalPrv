//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ORG_DAL.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class codeTablesValuesConfigurations
    {
        public System.Guid TableValueConfigurationGUID { get; set; }
        public System.Guid ValueGUID { get; set; }
        public System.Guid OrganizationInstanceGUID { get; set; }
        public bool Active { get; set; }
        public byte[] codeTablesValuesConfigurationsRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
    
        public virtual codeTablesValues codeTablesValues { get; set; }
    }
}