
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MRS_DAL.Model
{
    
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using RES_Repo.Globalization;
    using System.ComponentModel.DataAnnotations.Schema;
    using Portal_BL.Library;
    
    public partial class RP_NoteVerbaleVehicle_Result
    {
        [Display(Name = "DriversDetail", ResourceType = typeof(resxDbFields))]
    	public string DriversDetail{ get; set; }
    	
        [Display(Name = "VehicleNumber", ResourceType = typeof(resxDbFields))]
    	public string VehicleNumber{ get; set; }
    	
        [Display(Name = "VehicalCount", ResourceType = typeof(resxDbFields))]
    	public string VehicalCount{ get; set; }
    	
    }
}