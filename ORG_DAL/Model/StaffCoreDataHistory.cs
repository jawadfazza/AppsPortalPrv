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
    
    public partial class StaffCoreDataHistory
    {
        public System.Guid UserHistoryGUID { get; set; }
        public System.Guid UserGUID { get; set; }
        public string EmailAddress { get; set; }
        public Nullable<bool> IsInternational { get; set; }
        public string StaffPhoto { get; set; }
        public string PositionInOrganigram { get; set; }
        public System.Guid JobTitleGUID { get; set; }
        public System.Guid DutyStationGUID { get; set; }
        public System.Guid DepartmentGUID { get; set; }
        public Nullable<System.Guid> ReportToGUID { get; set; }
        public Nullable<System.Guid> NationalityGUID { get; set; }
        public Nullable<System.Guid> Nationality2GUID { get; set; }
        public Nullable<System.Guid> Nationality3GUID { get; set; }
        public Nullable<System.DateTime> ExpiryOfResidencyVisa { get; set; }
        public string SyrianNationalIDNumber { get; set; }
        public string SyrianNationalIDPhoto { get; set; }
        public string StaffPrefix { get; set; }
        public Nullable<System.Guid> PlaceOfBirthGUID { get; set; }
        public string EmploymentID { get; set; }
        public string UNHCRIDNumber { get; set; }
        public string CallSign { get; set; }
        public string StaffLastWork { get; set; }
        public string UNLPNumber { get; set; }
        public Nullable<System.DateTime> UNLPDateOfIssue { get; set; }
        public Nullable<System.DateTime> UNLPDateOfExpiry { get; set; }
        public string UNLPPhoto { get; set; }
        public string NationalPassportNumber { get; set; }
        public Nullable<System.DateTime> NationalPassportDateOfIssue { get; set; }
        public Nullable<System.DateTime> NationalPassportDateOfExpiry { get; set; }
        public string NationalPassportPhoto { get; set; }
        public string BankNameLeb { get; set; }
        public string BankAccountNumberLeb { get; set; }
        public string BankNameSyr { get; set; }
        public string BankAccountNumberSyr { get; set; }
        public string BankAccountHolderNameEn { get; set; }
        public string BankAccountHolderNameAr { get; set; }
        public Nullable<System.Guid> ContractTypeGUID { get; set; }
        public Nullable<System.Guid> RecruitmentTypeGUID { get; set; }
        public Nullable<System.Guid> StaffGradeGUID { get; set; }
        public Nullable<System.DateTime> ContractStartDate { get; set; }
        public Nullable<System.DateTime> ContractEndDate { get; set; }
        public Nullable<System.DateTime> StaffEOD { get; set; }
        public string PositionNumber { get; set; }
        public string PermanentAddressEn { get; set; }
        public string PermanentAddressAr { get; set; }
        public string CurrentAddressEn { get; set; }
        public string CurrentAddressAr { get; set; }
        public string HomeTelephoneNumberLandline { get; set; }
        public string HomeTelephoneNumberMobile { get; set; }
        public string OfficialMobileNumber { get; set; }
        public string OfficialExtensionNumber { get; set; }
        public Nullable<System.Guid> OfficeGUID { get; set; }
        public Nullable<System.Guid> OfficeFloorGUID { get; set; }
        public Nullable<System.Guid> OfficeRoomGUID { get; set; }
        public Nullable<bool> IsOld { get; set; }
        public Nullable<System.Guid> Gender { get; set; }
        public string IDNumber { get; set; }
        public string PersonalIDPhoto { get; set; }
        public string PassportPhoto { get; set; }
        public string AttendancePhoto { get; set; }
        public string DangerPay { get; set; }
        public string WifeHusbandName { get; set; }
        public string AssignmentDuration { get; set; }
        public string BankAccountNumber { get; set; }
        public string CurrentLocationByDate { get; set; }
        public Nullable<bool> StaffMemberOnGTA { get; set; }
        public Nullable<System.DateTime> GTAStartDate { get; set; }
        public Nullable<System.DateTime> GTAEndDate { get; set; }
        public string Accommodation { get; set; }
        public Nullable<System.DateTime> AccommodationStartingDate { get; set; }
        public Nullable<System.DateTime> StartOfMissionAssignmentDate { get; set; }
        public Nullable<System.DateTime> EndOfMissionAssignmentDate { get; set; }
        public string DetailsOfMissionAssignment { get; set; }
        public string DurationOfStayInSyria { get; set; }
        public Nullable<System.Guid> EmploymentType { get; set; }
        public Nullable<System.Guid> StaffAccessCategoryGUID { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public string PlaceBirthCityEn { get; set; }
        public string PlaceBirthCityAr { get; set; }
        public string LastJobEn { get; set; }
        public string LastJobAr { get; set; }
        public string SATPhoneNumber { get; set; }
        public string OfficeRoomNumberBuilding { get; set; }
        public string DegreeAr { get; set; }
        public string DegreeEn { get; set; }
        public Nullable<System.DateTime> ReturnDateFromLastRAndRLeave { get; set; }
        public Nullable<bool> StaffStatus { get; set; }
        public string BloodGroup { get; set; }
        public Nullable<System.Guid> LastUpdatedByGUID { get; set; }
        public Nullable<System.DateTime> LastUpdatedOn { get; set; }
        public Nullable<System.Guid> LastConfirmedByGUID { get; set; }
        public Nullable<System.DateTime> LastConfirmedOn { get; set; }
        public Nullable<System.Guid> ConfirmationStatusGUID { get; set; }
        public string ConfirmationComment { get; set; }
        public bool Active { get; set; }
        public byte[] StaffCoreDataHistoryRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
    
        public virtual codeCountries codeCountries { get; set; }
        public virtual codeCountries codeCountries1 { get; set; }
        public virtual codeCountries codeCountries2 { get; set; }
        public virtual codeCountries codeCountries3 { get; set; }
        public virtual codeDepartments codeDepartments { get; set; }
        public virtual codeDutyStations codeDutyStations { get; set; }
        public virtual codeJobTitles codeJobTitles { get; set; }
    }
}
