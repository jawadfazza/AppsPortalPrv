//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AppsPortal.Areas.PRG.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class dataIndividualProcessGroup
    {
        public System.Guid IndividualProcessingGroupGUID { get; set; }
        public System.Guid IndividualGUID { get; set; }
        public System.Guid ProcessingGroupGUID { get; set; }
        public bool IndividualProcessingGroupActive { get; set; }
        public short IndividualSequenceNumber { get; set; }
        public bool PrincipalRepresentative { get; set; }
        public bool Representative { get; set; }
        public string RelationshipToPrincipalRepresentative { get; set; }
        public bool Archived { get; set; }
        public string UserIDCreate { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string UserIDUpdate { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string SiteIDCreate { get; set; }
        public string SiteIDUpdate { get; set; }
        public string SiteIDOwner { get; set; }
        public System.DateTime OwnerDate { get; set; }
        public string SiteIDReplicate { get; set; }
    
        public virtual dataIndividual dataIndividual { get; set; }
        public virtual dataProcessGroup dataProcessGroup { get; set; }
    }
}