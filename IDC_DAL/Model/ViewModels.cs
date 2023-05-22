using RES_Repo.Globalization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDC_DAL.Model
{
    public class CardIndividualInfosDataTableModel
    {
        public System.Guid CardIndividualInfoGUID { get; set; }
        public string IndividualID { get; set; }
        public string CaseNumber { get; set; }
        public string FullName_EN { get; set; }
        public string FullName_AR { get; set; }
        public string Sex { get; set; }
        public Nullable<System.DateTime> DateOfBrith { get; set; }
        public string Category { get; set; }
        public Nullable<System.DateTime> ArrivalDate { get; set; }
        public bool HasIdCard { get; set; }
        public bool Active { get; set; }
        public byte[] dataCardIndividualInfoRowVersion { get; set; }
    }


    public class CardPrintsDataTableModel
    {
        public System.Guid CardIssuedGUID { get; set; }
        public string IndividualID { get; set; }
        public string CaseNumber { get; set; }
        public string FullName_EN { get; set; }
        public string FullName_AR { get; set; }
        public string Sex { get; set; }
        public Nullable<System.DateTime> DateOfBrith { get; set; }
        public string Category { get; set; }
        public Nullable<System.DateTime> ArrivalDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public string PrintBy { get; set; }
        public DateTime? PrintDate { get; set; }
        public string IssueCode { get; set; }
        public bool Active { get; set; }
        public byte[] dataCardIssuedRowVersion { get; set; }
    }

    public class CardValidationsDataTableModel
    {
        public System.Guid CardIssuedGUID { get; set; }
        public string IndividualID { get; set; }
        public string CaseNumber { get; set; }
        public string FullName_EN { get; set; }
        public string FullName_AR { get; set; }
        public string Sex { get; set; }
        public string IssueCode { get; set; }
        public Nullable<System.DateTime> DateOfBrith { get; set; }
        public string Category { get; set; }
        public Nullable<System.DateTime> ArrivalDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public bool Active { get; set; }
        public byte[] dataCardIssuedRowVersion { get; set; }
    }

    public class CardIndividualInfoUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "CardIndividualInfoGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid CardIndividualInfoGUID { get; set; }

        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "IndividualID", ResourceType = typeof(resxDbFields))]
        public string IndividualID { get; set; }

        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "CaseNumber", ResourceType = typeof(resxDbFields))]
        public string CaseNumber { get; set; }

        [StringLength(70, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "FullName_EN", ResourceType = typeof(resxDbFields))]
        public string FullName_EN { get; set; }

        [StringLength(57, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "FullName_AR", ResourceType = typeof(resxDbFields))]
        public string FullName_AR { get; set; }

        [StringLength(2, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Sex", ResourceType = typeof(resxDbFields))]
        public string Sex { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "DateOfBrith", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> DateOfBrith { get; set; }

        [StringLength(15, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Category", ResourceType = typeof(resxDbFields))]
        public string Category { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "ArrivalDate", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> ArrivalDate { get; set; }

        [Display(Name = "ArrivalEstimation", ResourceType = typeof(resxDbFields))]
        public bool? ArrivalEstimation { get; set; }

        [StringLength(10, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "CountyCodeA3", ResourceType = typeof(resxDbFields))]
        public string CountyCodeA3 { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataCardIndividualInfoRowVersion { get; set; }
    }
}
