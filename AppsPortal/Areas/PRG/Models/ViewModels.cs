using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PRG_DAL.ViewModels
{
    public class IndividualDataTableModel
    {
        public System.Guid IndividualGUID { get; set; }
        public string IndividualID { get; set; }
        public string FamilyName { get; set; }
        public string GivenName { get; set; }
        public string MiddleName { get; set; }
        public string MaidenName { get; set; }
        public string VerbalName { get; set; }
        public System.DateTime RegistrationDate { get; set; }
        public Nullable<System.DateTime> DateofBirth { get; set; }
        public string OriginCountryCode { get; set; }
        public string SexCode { get; set; }
        public Nullable<short> IndividualAge { get; set; }
        public string NationalityCode { get; set; }
        public string MotherName { get; set; }
        public string ProcessingGroupNumber { get; set; }
        public string ProcessStatusCode { get; set; }
        public string RefugeeStatusCode { get; set; }

    }
}