using RES_Repo.Globalization;
using AppsPortal.Library;
using AppsPortal.Models;
using ExpressiveAnnotations.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using AppsPortal.AMS.ViewModels;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppsPortal.ViewModels
{
    public class ReturnURL
    {
        public string URL { get; set; }
    }

    public class AccountDetailsModel
    {
        public System.Guid UserRegistrationQueueGUID { get; set; }
        public System.Guid UserGUID { get; set; }
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [Display(Name = "EmailAddress", ResourceType = typeof(resxDbFields))]
        //[Remote("EmailAddress", "RemoteValidation", ErrorMessage = "Email already exists! <a href='www.google.com'>Help</a>")]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        public string EmailAddress { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "TimeZone", ResourceType = typeof(resxDbFields))]
        public string TimeZone { get; set; }
        public string LanguageID { get { return "EN"; } }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        //[RegularExpression(@"^[a-zA-Z]+[\-'\s]?[a-zA-Z ]+$", ErrorMessage = "Only character, space and hyphen allowed")] /*not really good*/
        [Display(Name = "FirstName", ResourceType = typeof(resxDbFields))]
        [StringLength(40, MinimumLength = 3, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(resxHttpErrors))]
        public string FirstName { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        //[RegularExpression(@"^[a-zA-Z]+[\-'\s]?[a-zA-Z ]+$", ErrorMessage = "Only character, space and hyphen allowed")] /*not really good*/
        [Display(Name = "LastName", ResourceType = typeof(resxDbFields))]
        [StringLength(40, MinimumLength = 3, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(resxHttpErrors))]
        public string Surname { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SecurityQuestion", ResourceType = typeof(resxDbFields))]
        public Guid SecurityQuestionGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SecurityAnswer", ResourceType = typeof(resxDbFields))]
        [StringLength(40, MinimumLength = 3, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(resxHttpErrors))]
        public string SecurityAnswer { get; set; }
        public Nullable<int> AccountStatusID { get { return Library.AccountStatus.Step01_RegistrationRequestSubmitted; } set { } }
        public Nullable<System.DateTime> RequestedOn { get { return DateTime.Now; } set { } }
        public string FullName { get { return FirstName + " " + Surname; } set { } }
    }

    public class RegistrationQueueView
    {
        public System.Guid UserRegistrationQueueGUID { get; set; }
        public System.Guid UserGUID { get; set; }

        [Display(Name = "RequestedOn", ResourceType = typeof(resxDbFields))]
        public string RequestedOn { get; set; }

        [Display(Name = "FirstName", ResourceType = typeof(resxDbFields))]
        public string FirstName { get; set; }

        [Display(Name = "Surname", ResourceType = typeof(resxDbFields))]
        public string Surname { get; set; }

        [Display(Name = "EmailAddress", ResourceType = typeof(resxDbFields))]
        public string EmailAddress { get; set; }

        [Display(Name = "JobTitleDescription", ResourceType = typeof(resxDbFields))]
        public string JobTitleDescription { get; set; }

        [Display(Name = "OrganizationInstanceDescription", ResourceType = typeof(resxDbFields))]
        public string OrganizationInstanceDiscription { get; set; }

        [Display(Name = "ConfirmedRegistrationQueue", ResourceType = typeof(resxDbFields))]
        public bool ConfirmedRegistrationQueue { get; set; }

    }

    public class RegisterVerifyEmail
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SecurityAnswer", ResourceType = typeof(resxDbFields))]
        [StringLength(40, MinimumLength = 3, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(resxHttpErrors))]
        public string SecurityAnswer { get; set; }
    }

    public class VerifyEmail
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessageResourceName = "InvalidEmailAddress", ErrorMessageResourceType = typeof(resxMessages))]
        [Display(Name = "EmailAddress", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
    }
    //public class DropDownList
    //{
    //    public string Value { get; set; }
    //    public string Description { get; set; }
    //}
    public class CheckBoxList
    {
        public string Value { get; set; }
        public string Description { get; set; }
        public string GroupDescription { get; set; }
        public string SearchKey { get; set; }
        public bool Checked { get; set; }
    }
    public class CountriesPhoneCode
    {
        public string src { get; set; }
        public string id { get; set; }
        public string text { get; set; }
    }
    public class NavigationMenu
    {
        public Guid MenuID { get; set; }
        public string RootMenuDescription { get; set; }
        public string RootMenuUrl { get; set; }
        public int? RootMenuSortID { get; set; }
        public List<NavigationSubMenu> SubMenu { get; set; }
    }
    public class NavigationSubMenu
    {
        public Guid SubMenuID { get; set; }
        public string SubMenuDescription { get; set; }
        public string SubMenuUrl { get; set; }
        public int? SubMenuSortID { get; set; }
    }
    public class FilterField
    {
        public string LabelText { get; set; }
        public string DbColumnName { get; set; }
        public string FilterForDataType { get; set; }
        public bool MustHasInitValue { get; set; }
        public SelectList DataList { get; set; }
        public bool IsMultiple { get; set; }
        public bool FullWidth { get; set; }

        public bool IsSearchable { get; set; }
    }
    //This Class for the _DataTable Partial View
    public class DataTableControl
    {
        /// <summary>
        /// Datatable name and action should be the same
        /// </summary>
        public string Name { get; set; }
        public string FormController { get; set; }
        public string Area { get; set; }
        public string Type { get; set; }
        public string EditMode { get; set; }
        public Guid? PK { get; set; }
        public Guid? FK { get; set; }
        public int OrderBy { get; set; }
        public bool SelectAll { get; set; }
    }
    public class LoginModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Invalid Email Address")]
        [Display(Name = "EmailAddress", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(resxDbFields))]
        [StringLength(40, MinimumLength = 8, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(resxHttpErrors))]
        public string Password { get; set; }
        [Display(Name = "RememberMe", ResourceType = typeof(resxUIControls))]
        public bool RememberMe { get; set; }
    }
    public class SetupOrganizationModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "OrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
        public Guid OrganizationInstanceGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "OrganizationGUID", ResourceType = typeof(resxDbFields))]
        public Guid OrganizationGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "JobTitleGUID", ResourceType = typeof(resxDbFields))]
        public Guid JobTitleGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SponsorUserProfileGUID", ResourceType = typeof(resxDbFields))]
        public Guid SponsorUserProfileGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.Date)]
        [Display(Name = "FromDate", ResourceType = typeof(resxDbFields))]
        public DateTime? FromDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "ToDate", ResourceType = typeof(resxDbFields))]
        public DateTime? ToDate { get; set; }
    }
    public class SetupPasswordModel
    {
        public System.Guid PasswordGUID { get { return Guid.NewGuid(); } set { } }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "NewPassword", ResourceType = typeof(resxDbFields))]
        [StringLength(30, MinimumLength = 8, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.Password)]
        [RegularExpression("(?=.*?[A-Z])(?=.*?[a-z])((?=.*?[0-9])|(?=.*?[#?!@$%^&*-])).{8,}", ErrorMessageResourceType = typeof(resxValidations), ErrorMessageResourceName = "PasswordValidation")]
        public string Password { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ConfirmPassword", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare(nameof(Password), ErrorMessageResourceType = typeof(resxValidations), ErrorMessageResourceName = "PasswordNotMatch")]
        public string ConfirmPassword { get; set; }
        public Nullable<System.DateTime> ActivationDate { get { return DateTime.Now; } set { } }
    }
    public class ChangePasswordModel
    {
        public System.Guid PasswordGUID { get { return Guid.NewGuid(); } set { } }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "CurrentPassword", ResourceType = typeof(resxDbFields))]
        [StringLength(30, MinimumLength = 8, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.Password)]
        [RegularExpression("(?=.*?[A-Z])(?=.*?[a-z])((?=.*?[0-9])|(?=.*?[#?!@$%^&*-])).{8,}", ErrorMessageResourceType = typeof(resxValidations), ErrorMessageResourceName = "PasswordValidation")]
        public string CurrentPassword { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "NewPassword", ResourceType = typeof(resxDbFields))]
        [StringLength(30, MinimumLength = 8, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.Password)]
        [RegularExpression("(?=.*?[A-Z])(?=.*?[a-z])((?=.*?[0-9])|(?=.*?[#?!@$%^&*-])).{8,}", ErrorMessageResourceType = typeof(resxValidations), ErrorMessageResourceName = "PasswordValidation")]
        public string Password { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ConfirmPassword", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare(nameof(Password), ErrorMessageResourceType = typeof(resxValidations), ErrorMessageResourceName = "PasswordNotMatch")]
        public string ConfirmPassword { get; set; }
        public Nullable<System.DateTime> ActivationDate { get { return DateTime.Now; } set { } }
    }
    public class UserModel
    {
        //This class is only for programming purposes, it is a template for GetUser function
        public Guid UserGUID { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string FullName { get { return FirstName + " " + Surname; } }
        public string EmailAddress { get; set; }
        public string JobTitle { get; set; }
        public string Gender { get; set; }
        public string Organization { get; set; }
        public string Operation { get; set; }
        public string ProfilePhoto { get; set; }
    }
    public class PersonalDetailsModel
    {
        public PersonalDetailsModel()
        {
            MediaPath = Constants.UserProfilePhotoTemplate;
        }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string FullName { get { return FirstName + " " + Surname; } }
        public string Nationality { get; set; }
        public string PreferedLanguage { get; set; }
        public string Gender { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
        public string BloodGroup { get; set; }
        public bool Active { get; set; }
        public string MediaPath { get; set; }
        public string MediaName { get; set; }
    }
    public class HomeAddressModel
    {
        [Display(Name = "HomePhone", ResourceType = typeof(resxDbFields))]
        public string HomePhone { get; set; }
        [Display(Name = "HomeAddress", ResourceType = typeof(resxDbFields))]
        public string HomeAddress { get; set; }
        [Display(Name = "Latitude", ResourceType = typeof(resxDbFields))]
        public string Latitude { get; set; }
        [Display(Name = "Longitude", ResourceType = typeof(resxDbFields))]
        public string Longitude { get; set; }
    }
    public class WorkAddressModel
    {
        [Display(Name = "OfficeGUID", ResourceType = typeof(resxDbFields))]
        public string OfficeGUID { get; set; }
        [Display(Name = "FloorNumber", ResourceType = typeof(resxDbFields))]
        public string FloorNumber { get; set; }
        [StringLength(50, MinimumLength = 3, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "RoomNumber", ResourceType = typeof(resxDbFields))]
        public string RoomNumber { get; set; }
    }
    public class PersonalDetailsUpdateModel
    {
        public PersonalDetailsUpdateModel()
        {
            MediaPath = Constants.UserProfilePhotoTemplate;
        }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "FirstName", ResourceType = typeof(resxDbFields))]
        [RegularExpression(@"^\D+$", ErrorMessageResourceName = "CharacterString", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(50, MinimumLength = 3, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(resxHttpErrors))]
        public string FirstName { get; set; }
        [RegularExpression(@"^\D+$", ErrorMessageResourceName = "CharacterString", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(50, MinimumLength = 3, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "FatherName", ResourceType = typeof(resxDbFields))]
        public string FatherName { get; set; }
        [RegularExpression(@"^\D+$", ErrorMessageResourceName = "CharacterString", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(50, MinimumLength = 3, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "GrandFatherName", ResourceType = typeof(resxDbFields))]
        public string GrandFatherName { get; set; }
        [RegularExpression(@"^\D+$", ErrorMessageResourceName = "CharacterString", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(50, MinimumLength = 3, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Surname", ResourceType = typeof(resxDbFields))]
        public string Surname { get; set; }
        [Display(Name = "UserGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid UserGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "CountryGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> CountryGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "GenderGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> GenderGUID { get; set; }
        [DataType(DataType.Date)]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DateOfBirth", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "PreferedLanguageID", ResourceType = typeof(resxDbFields))]
        public string PreferedLanguageID { get; set; }
        public string MediaPath { get; set; }
        public string MediaName { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "BloodGroup", ResourceType = typeof(resxDbFields))]
        public string BloodGroup { get; set; }
        public bool Active { get; set; }
        public byte[] userPersonalDetailsRowVersion { get; set; }
        public byte[] userPersonalDetailsLanguageRowVersion { get; set; }
    }
    public class HomeAddressUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        public System.Guid UserGUID { get; set; }
        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "HomePhoneNumber", ResourceType = typeof(resxDbFields))]
        public string HomePhoneNumber { get; set; }
        [StringLength(5, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "HomePhoneCountryCode", ResourceType = typeof(resxDbFields))]
        public string HomePhoneCountryCode { get; set; }
        [Display(Name = "Latitude", ResourceType = typeof(resxDbFields))]
        public double Latitude { get; set; }
        [Display(Name = "Longitude", ResourceType = typeof(resxDbFields))]
        public double Longitude { get; set; }
        [StringLength(256)]
        [Display(Name = "HomeAddress", ResourceType = typeof(resxDbFields))]
        public string HomeAddress { get; set; }
        [StringLength(256, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "CountryDescription", ResourceType = typeof(resxDbFields))]
        public string CountryDescription { get; set; }
        public bool Active { get; set; }
        public byte[] userHomeAddressRowVersion { get; set; }
        public byte[] userHomeAddressLanguageRowVersion { get; set; }
    }
    public class WorkAddressUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "UserGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid UserGUID { get; set; }
        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "FloorNumber", ResourceType = typeof(resxDbFields))]
        public string FloorNumber { get; set; }
        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "RoomNumber", ResourceType = typeof(resxDbFields))]
        public string RoomNumber { get; set; }
        [Display(Name = "Latitude", ResourceType = typeof(resxDbFields))]
        public Nullable<double> Latitude { get; set; }
        [Display(Name = "Longitude", ResourceType = typeof(resxDbFields))]
        public Nullable<double> Longitude { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "AreaName", ResourceType = typeof(resxDbFields))]
        public string AreaName { get; set; }
        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "BuildingName", ResourceType = typeof(resxDbFields))]
        public string BuildingName { get; set; }
        [StringLength(256, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "CountryDescription", ResourceType = typeof(resxDbFields))]
        public string CountryDescription { get; set; }
        public bool Active { get; set; }
        public byte[] userWorkAddressRowVersion { get; set; }
        public byte[] userWorkAddressLanguageRowVersion { get; set; }
    }
    public class ServiceHistoryModel
    {
        public string Organization { get; set; }
        public string EmailAddress { get; set; }
        public string IndexNumber { get; set; }
        public string EmployeeNumber { get; set; }
        public string OrganizationInstanceDescription { get; set; }
        //public string OrganizationInstance { get; set; }
        public string DutyStation { get; set; }
        public string Department { get; set; }
        public string Supervisor { get; set; }
        public string ReviewingOfficer { get; set; }
        public string JobTitle { get; set; }
        public string Grade { get; set; }
        public string Step { get; set; }
    }
    public class ServiceHistoryUpdateModel
    {
        //user service history
        public Guid ServiceHistoryGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        public Guid? OrganizationGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        public string EmployeeNumber { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        public string IndexNumber { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Invalid Email Address")]
        [Display(Name = "EmailAddress", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
        public bool Active { get; set; }
        public byte[] userServiceHistoryRowVersion { get; set; }
    }
    public class ApplicationDataTableModel
    {
        public Guid ApplicationGUID { get; set; }
        public byte[] codeApplicationsRowVersion { get; set; }
        public bool Active { get; set; }
        public int ApplicationID { get; set; }
        public string ApplicationAcrynom { get; set; }
        public string ApplicationStatus { get; set; }
        public int SortID { get; set; }
        public string ApplicationDescription { get; set; }
    }

    public class LoginDataTableModel
    {
        public Guid UserAuditGUID { get; set; }
        public string FullName { get; set; }
        public string EmailAddress { get; set; }
        [DataType(DataType.DateTime)]
        public System.DateTime LoginTime { get; set; }
        public Nullable<System.DateTime> LocalLoginTime { get { return new Portal().LocalTime(this.LoginTime); } }
        [DataType(DataType.DateTime)]
        public Nullable<System.DateTime> LogoutTime { get; set; }
        public Nullable<System.DateTime> LocalLogoutTime { get { return new Portal().LocalTime(this.LogoutTime); } }
        public string JobTitleDescription { get; set; }
        public string Browser { get; set; }
        public string Environment { get; set; }
        public string MobileDeviceManufacturer { get; set; }
        public string MobileDeviceModel { get; set; }
        public string ComputerName { get; set; }
        public string UserHostAddress { get; set; }
        public Nullable<bool> IsMobileDevice { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string City { get; set; }
        public string TimeZone { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }

    }

    public class MenuDataTableModel
    {
        public Guid MenuGUID { get; set; }
        public byte[] codeMenusRowVersion { get; set; }
        public string MenuDescription { get; set; }
        public string ApplicationDescription { get; set; }
        public string ActionUrl { get; set; }
        public string ParentMenuDescription { get; set; }
        public bool IsPublic { get; set; }
        public string ActionGUID { get; set; }
        public string SortID { get; set; }
        public bool Active { get; set; }
    }
    public class ApplicationUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ApplicationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ApplicationGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        [Display(Name = "ApplicationID", ResourceType = typeof(resxDbFields))]
        public int ApplicationID { get; set; }
        [RegularExpression(@"^\D+$", ErrorMessageResourceName = "CharacterString", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(50, MinimumLength = 3, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ApplicationAcrynom", ResourceType = typeof(resxDbFields))]
        public string ApplicationAcrynom { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ApplicationStatusGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ApplicationStatusGUID { get; set; }

        [Display(Name = "ServerAccessibility", ResourceType = typeof(resxDbFields))]
        public List<string> ServerAccessibility { get; set; }

        [Display(Name = "ServerAccessibility", ResourceType = typeof(resxDbFields))]
        public string ClientServerAccessibility { get; set; }

        [Display(Name = "SortID", ResourceType = typeof(resxDbFields))]
        public Nullable<int> SortID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }
        public byte[] codeApplicationsRowVersion { get; set; }
        [RegularExpression(@"^\D+$", ErrorMessageResourceName = "CharacterString", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(512, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ApplicationDescription", ResourceType = typeof(resxDbFields))]
        public string ApplicationDescription { get; set; }
        public byte[] codeApplicationsLanguagesRowVersion { get; set; }
    }
    public class MenuUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MenuGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid MenuGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(150, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MenuDescription", ResourceType = typeof(resxDbFields))]
        public string MenuDescription { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ApplicationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ApplicationGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ActionUrl", ResourceType = typeof(resxDbFields))]
        public string ActionUrl { get; set; }
        [Display(Name = "ParentGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ParentGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "IsPublic", ResourceType = typeof(resxDbFields))]
        public bool IsPublic { get; set; }

        [RequiredIf("IsPublic == false", ErrorMessage = "Action category is required when accessablity is set to Authorized")]
        [Display(Name = "ActionCategoryGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ActionCategoryGUID { get; set; }

        [RequiredIf("IsPublic == false", ErrorMessage = "Action is required when accessablity is set to Authorized")]
        [Display(Name = "ActionGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ActionGUID { get; set; }

        [Display(Name = "SortID", ResourceType = typeof(resxDbFields))]
        public Nullable<int> SortID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }
        public byte[] codeMenusRowVersion { get; set; }
        public byte[] codeMenusLanguagesRowVersion { get; set; }
    }
    public class DepartmentDataTableModel
    {
        public Guid DepartmentGUID { get; set; }
        public string DepartmentDescription { get; set; }
        public string DepartmentCode { get; set; }
        public int? SortID { get; set; }
        public bool Active { get; set; }
        public byte[] codeDepartmentsRowVersion { get; set; }
    }
    public class DepartmentUpdateModel
    {
        public Guid DepartmentGUID { get; set; }
        public Guid? FactorGUID { get; set; }
        public Guid? ParentGUID { get; set; }
        [Display(Name = "DepartmentTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid? DepartmentTypeGUID { get; set; }
        [StringLength(150, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [RegularExpression(@"^\D+$", ErrorMessageResourceName = "CharacterString", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DepartmentCode", ResourceType = typeof(resxDbFields))]
        public string DepartmentCode { get; set; }
        [Display(Name = "DepartmentLevel", ResourceType = typeof(resxDbFields))]
        public int? DepartmentLevel { get; set; }
        [Display(Name = "SortID", ResourceType = typeof(resxDbFields))]
        public int? SortID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }
        public byte[] codeDepartmentsRowVersion { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public DateTime? DeletedOn { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(150, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [RegularExpression(@"^\D+$", ErrorMessageResourceName = "CharacterString", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DepartmentDescription", ResourceType = typeof(resxDbFields))]
        public string DepartmentDescription { get; set; }
        public byte[] codeDepartmentsLanguagesRowVersion { get; set; }
    }
    public class CountryDataTableModel
    {
        public Guid CountryGUID { get; set; }
        public string CountryA2Code { get; set; }
        public string CountryA3Code { get; set; }
        public string PhoneCode { get; set; }
        public bool Active { get; set; }
        public byte[] codeCountriesRowVersion { get; set; }
        public string CountryDescription { get; set; }
        public string Nationality { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
    public class CountryUpdateModel
    {
        public Guid CountryGUID { get; set; }
        [StringLength(2, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [RegularExpression(@"^\D+$", ErrorMessageResourceName = "CharacterString", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "CountryA2Code", ResourceType = typeof(resxDbFields))]
        public string CountryA2Code { get; set; }
        [StringLength(3, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [RegularExpression(@"^\D+$", ErrorMessageResourceName = "CharacterString", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "CountryA3Code", ResourceType = typeof(resxDbFields))]
        public string CountryA3Code { get; set; }
        [StringLength(10, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "PhoneCode", ResourceType = typeof(resxDbFields))]
        public string PhoneCode { get; set; }
        [Display(Name = "Latitude", ResourceType = typeof(resxDbFields))]
        public double Latitude { get; set; }
        [Display(Name = "Longitude", ResourceType = typeof(resxDbFields))]
        public double Longitude { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "FactorGUID", ResourceType = typeof(resxDbFields))]
        public Guid FactorGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }
        public byte[] codeCountriesRowVersion { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(150, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [RegularExpression(@"^\D+$", ErrorMessageResourceName = "CharacterString", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "CountryDescription", ResourceType = typeof(resxDbFields))]
        public string CountryDescription { get; set; }
        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [RegularExpression(@"^\D+$", ErrorMessageResourceName = "CharacterString", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Nationality", ResourceType = typeof(resxDbFields))]
        public string Nationality { get; set; }
        public byte[] codeCountriesLanguagesRowVersion { get; set; }
    }
    public class OrganizationDataTableModel
    {
        public Guid OrganizationGUID { get; set; }
        public string OrganizationFocusCode { get; set; }
        public string OrganizationShortName { get; set; }
        public string OrganizationDescription { get; set; }
        public string OrganizationType { get; set; }
        public int OperationsCount { get; set; }
        public bool Active { get; set; }
        public byte[] codeOrganizationsRowVersion { get; set; }
    }
    public class OrganizationInstanceDataTableModel
    {
        public Guid OrganizationInstanceGUID { get; set; }
        public string OrganizationInstanceDescription { get; set; }
        public string OrganizationDescription { get; set; }
        public string OperationDescription { get; set; }
        public bool Active { get; set; }
        public byte[] codeOrganizationsInstancesRowVersion { get; set; }
    }
    public class OrganizationInstanceUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "OrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid OrganizationInstanceGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "OrganizationInstanceDescription", ResourceType = typeof(resxDbFields))]
        public string OrganizationInstanceDescription { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "OrganizationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid OrganizationGUID { get; set; }
        [Display(Name = "OperationGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> OperationGUID { get; set; }
        public byte[] codeOrganizationsInstancesRowVersion { get; set; }
        public byte[] codeOrganizationsInstancesLanguagesRowVersion { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }
    }
    public class OrganizationUpdateModel
    {
        public OrganizationUpdateModel()
        {
            MediaPath = Constants.NoPhotoTemplate;
        }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "OrganizationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid OrganizationGUID { get; set; }
        public string MediaPath { get; set; }
        public string MediaName { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "OrganizationTypeGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> OrganizationTypeGUID { get; set; }
        [RequiredIf("OrganizationTypeGUID != Guid('55369FFB-863A-4167-A20E-DC8F44F7D5F9')", ErrorMessage = "Country is required when organization type is not international")]
        [Display(Name = "CountryGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> CountryGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(60, MinimumLength = 3, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "OrganizationFocusCode", ResourceType = typeof(resxDbFields))]
        public string OrganizationFocusCode { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(60, MinimumLength = 2, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [RegularExpression(@"^\D+$", ErrorMessageResourceName = "CharacterString", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "OrganizationShortName", ResourceType = typeof(resxDbFields))]
        public string OrganizationShortName { get; set; }
        [StringLength(150, MinimumLength = 3, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "OrganizationWebSite", ResourceType = typeof(resxDbFields))]
        public string OrganizationWebSite { get; set; }
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [StringLength(150, MinimumLength = 3, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DirectorEmail", ResourceType = typeof(resxDbFields))]
        public string DirectorEmail { get; set; }
        [StringLength(50, MinimumLength = 3)]
        [Display(Name = "DirectorPhone", ResourceType = typeof(resxDbFields))]
        public string DirectorPhone { get; set; }
        [StringLength(150, MinimumLength = 3, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "HQAddress", ResourceType = typeof(resxDbFields))]
        public string HQAddress { get; set; }
        [StringLength(100, MinimumLength = 3, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [RegularExpression(@"^\D+$", ErrorMessageResourceName = "CharacterString", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DirectorName", ResourceType = typeof(resxDbFields))]
        public string DirectorName { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(100, MinimumLength = 3, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [RegularExpression(@"^\D+$", ErrorMessageResourceName = "CharacterString", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "OrganizationDescription", ResourceType = typeof(resxDbFields))]
        public string OrganizationDescription { get; set; }
        public bool Active { get; set; }
        public byte[] codeOrganizationsRowVersion { get; set; }
        public byte[] codeOrganizationsLanguagesRowVersion { get; set; }
    }
    //public class OrganizationsOperationsUpdateModel
    //{
    //    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
    //    [Display(Name = "OrganizationOperationGUID", ResourceType = typeof(resxDbFields))]
    //    public System.Guid OrganizationOperationGUID { get; set; }
    //    public List<DropDownList> codeOperationsSeleted { get; set; }
    //    [Display(Name = "OrganizationGUID", ResourceType = typeof(resxDbFields))]
    //    public Guid OrganizationGUID { get; set; }
    //}
    //public class OrganizationsOperationsDepartmentDutystationsUpdateModel
    //{
    //    public OrganizationsOperationsDepartmentUpdateModel OrganizationsOperationsDepartments { get; set; }
    //    public OrganizationsOperationsDutystationsUpdateModel OrganizationsOperationsDutystations { get; set; }
    //}
    //public class OrganizationsOperationsDepartmentUpdateModel
    //{
    //    [Display(Name = "OrganizationOperationDepartementGUD", ResourceType = typeof(resxDbFields))]
    //    public System.Guid OrganizationOperationDepartementGUD { get; set; }
    //    [Display(Name = "OrganizationOperationGUID", ResourceType = typeof(resxDbFields))]
    //    public System.Guid OrganizationOperationGUID { get; set; }
    //    public List<DropDownList> codeDepartmentsSeleted { get; set; }
    //}
    //public class OrganizationsOperationsDutystationsUpdateModel
    //{
    //    [Display(Name = "OrganizationOperationDutyStationGUD", ResourceType = typeof(resxDbFields))]
    //    public System.Guid OrganizationOperationDutyStationGUID { get; set; }
    //    [Display(Name = "OrganizationOperationGUID", ResourceType = typeof(resxDbFields))]
    //    public System.Guid OrganizationOperationGUID { get; set; }
    //    public List<DropDownList> codeDutyStationsSeleted { get; set; }
    //}
    public class OperationOrganizationCheckModel
    {
        public System.Guid OrganizationOperationGUID { get; set; }
        public bool Selected { get; set; }
        public string Description { get; set; }
    }
    public class TestDataTable
    {
        public string PK { get; set; }
        public byte[] RV { get; set; }
        public string GivenName { get; set; }
        public string DateOfBirth { get; set; }
        public string sexcode { get; set; }
        public string GenderDescription { get; set; }
        public string OriginCountryCode { get; set; }
        public string RegistrationDate { get; set; }
    }
    public class MasterRecordStatus
    {
        public Guid ParentGUID { get; set; }
        public Guid? OptionalParameter { get; set; }
        public bool IsParentActive { get; set; }
    }
    public class SelectedRecords
    {
        public SelectedRecords()
        {
            SelectedGuids = new List<Guid>();
            RV = new List<string>();
        }
        public List<Guid> SelectedGuids { get; set; }
        public List<string> RV { get; set; } // 0,0,0,0,0,0,0,8
        public string RowVersion
        {
            get
            {
                if (RV == null)
                {
                    return null;
                }
                List<byte[]> byteList = new List<byte[]>();
                string result = "";
                foreach (var item in RV)
                {
                    byteList.Add(item.Split(',').Select(s => byte.Parse(s)).ToArray());
                }
                foreach (var item in byteList)
                {
                    //To match SQL format, 0x0000000000123 , (,) for IN where clause
                    result += "0x" + BitConverter.ToString(item, 0).Replace("-", "") + ",";
                }
                return result.Substring(0, result.Length - 1);
            }
        }
    }
    public class Notify
    {
        public string Type { get; set; }
        public string Message { get; set; }
    }
    public class PartialViewModel
    {
        public string PK { get; set; }
        public bool Active { get; set; }
        public string Url { get; set; }
        public string Container { get; set; }
        public string Operation { get; set; }
    }
    public class RowVersionControl
    {
        public string ControlID { get; set; }
        public string Value { get; set; }
    }
    public class InvalidControl
    {
        public string ControlID { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class PrimaryKeyControl
    {
        public string ControlID { get; set; }
        public string Value { get; set; }
    }
    public class JsonReturn
    {
        public JsonReturn()
        {
            AffectedRecordsGuids = new List<Guid>();
            InvalidControls = new List<InvalidControl>();
        }
        public Notify Notify { get; set; }
        public string RedirectTo { get; set; }
        public string DataTable { get; set; }
        public string NextPageMode { get; set; }
        public bool Concurrency { get; set; }
        public object dbModel { get; set; }
        public string CallbackFunction { get; set; }
        public PrimaryKeyControl PrimaryKey { get; set; }
        public List<PrimaryKeyControl> PrimaryKeys { get; set; }
        public List<RowVersionControl> RowVersions { get; set; }
        public List<InvalidControl> InvalidControls { get; set; }
        public List<PartialViewModel> PartialViews { get; set; }
        public List<Guid> AffectedRecordsGuids { get; set; }
        public List<Guid> DeleteClientSide { get; set; }
        public List<UIButtons> UIButtons { get; set; }

        public List<CalendarEvents> CalendarEvents { get; set; }
        public List<WorkDay> WorkDays { get; set; }
    }

    public class WorkDay
    {
        public int Day { get; set; }
    }

    public class UIButtons
    {
        public string Container { get; set; }
        public string Button { get; set; }

    }

    public class DutyStationsUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid DutyStationGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "CountryGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid CountryGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [RegularExpression(@"^\D+$", ErrorMessageResourceName = "CharacterString", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DutyStationDescription", ResourceType = typeof(resxDbFields))]
        public string DutyStationDescription { get; set; }
        [Display(Name = "Latitude", ResourceType = typeof(resxDbFields))]
        public double Latitude { get; set; }
        [Display(Name = "Longitude", ResourceType = typeof(resxDbFields))]
        public double Longitude { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }
        public byte[] codeDutyStationsRowVersion { get; set; }
        public byte[] codeDutyStationsLanguagesRowVersion { get; set; }
    }

    public class DutyStationDataTableModel
    {
        public System.Guid DutyStationGUID { get; set; }
        public String CountryDescription { get; set; }
        public string DutyStationDescription { get; set; }
        public bool Active { get; set; }
        public byte[] codeDutyStationsRowVersion { get; set; }
    }
    public class LocationsUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "LocationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid LocationGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "LocationTypeGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid LocationTypeGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "CountryGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid CountryGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "FactorGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid FactorGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "LocationParentGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> LocationParentGUID { get; set; }
        [StringLength(150, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "LocationPCode", ResourceType = typeof(resxDbFields))]
        public string LocationPCode { get; set; }
        [Display(Name = "LocationlevelID", ResourceType = typeof(resxDbFields))]
        public int LocationlevelID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Latitude", ResourceType = typeof(resxDbFields))]
        public double Latitude { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Longitude", ResourceType = typeof(resxDbFields))]
        public double Longitude { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(150, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "LocationDescription", ResourceType = typeof(resxDbFields))]
        public string LocationDescription { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }
        public byte[] codeLocationsRowVersion { get; set; }
        public byte[] codeLocationsLanguagesRowVersion { get; set; }
    }
    public class LocationDataTableModel
    {
        public System.Guid LocationGUID { get; set; }
        public string LocationDescription { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public System.String LocationTypeGUID { get; set; }
        public System.String CountryGUID { get; set; }
        public System.String LocationParentGUID { get; set; }
        public Nullable<int> LocationlevelID { get; set; }
        public bool Active { get; set; }
        public byte[] codeLocationsRowVersion { get; set; }
    }
    public class TableValuesUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ValueGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ValueGUID { get; set; }
        [Display(Name = "TableGUID", ResourceType = typeof(resxDbFields))]
        public Guid TableGUID { get; set; }
        [Display(Name = "TableName", ResourceType = typeof(resxDbFields))]
        public string TableName { get; set; }
        [Display(Name = "DetailsSitemapGUID", ResourceType = typeof(resxDbFields))]
        public Guid DetailsSitemapGUID { get; set; }
        [Display(Name = "SortID", ResourceType = typeof(resxDbFields))]
        public Nullable<int> SortID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(256, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ValueDescription", ResourceType = typeof(resxDbFields))]
        public string ValueDescription { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }
        public byte[] codeTablesValuesRowVersion { get; set; }
        public byte[] codeTablesValuesLanguagesRowVersion { get; set; }
    }
    public class TableValuesDataTableModel
    {
        public System.Guid ValueGUID { get; set; }
        public string TableName { get; set; }
        public Nullable<int> SortID { get; set; }
        public string ValueDescription { get; set; }
        public bool Active { get; set; }
        public byte[] codeTablesValuesRowVersion { get; set; }
    }
    public class OperationDataTableModel
    {
        public System.Guid OperationGUID { get; set; }
        public string BureauDescription { get; set; }
        public string CountryDescription { get; set; }
        public bool Active { get; set; }
        public string OperationDescription { get; set; }
        public byte[] codeOperationsRowVersion { get; set; }
    }
    public class OperationUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "OperationDescription", ResourceType = typeof(resxDbFields))]
        public string OperationDescription { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "OperationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid OperationGUID { get; set; }
        [Display(Name = "BureauGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> BureauGUID { get; set; }
        [Display(Name = "CountryGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> CountryGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "FactorGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid FactorGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }
        public byte[] codeOperationsRowVersion { get; set; }
        public byte[] codeOperationsLanguagesRowVersion { get; set; }
    }
    public class JobTitleDataTableModel
    {
        public System.Guid JobTitleGUID { get; set; }
        public bool Active { get; set; }
        public string JobTitleDescription { get; set; }
        public byte[] codeJobTitlesRowVersion { get; set; }
    }
    public class JobTitleUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "JobTitleGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid JobTitleGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }
        public byte[] codeJobTitlesRowVersion { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        //[Remote("ApplicationDescription", "RemoteValidation", AdditionalFields = "JobTitleGUID", ErrorMessage = "Description is exists")]
        [RegularExpression(@"^\D+$", ErrorMessageResourceName = "CharacterString", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(50, MinimumLength = 3, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "JobTitleDescription", ResourceType = typeof(resxDbFields))]
        public string JobTitleDescription { get; set; }

        public byte[] codeJobTitleLanguagesRowVersion { get; set; }
    }
    public class userServiceHistoryModel
    {
        public Guid ServiceHistoryGUID { get; set; }
        public Guid UserGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "OrganizationGUID", ResourceType = typeof(resxDbFields))]
        public Guid OrganizationGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "EmployeeNumber", ResourceType = typeof(resxDbFields))]
        public string EmployeeNumber { get; set; }
        [Display(Name = "IndexNumber", ResourceType = typeof(resxDbFields))]
        public string IndexNumber { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "EmailAddress", ResourceType = typeof(resxDbFields))]
        public string EmailAddress { get; set; }
        public bool Active { get; set; }
        public byte[] userServiceHistoryRowVersion { get; set; }
        public List<userProfilesModel> userProfilesModel { get; set; }
    }
    public class userProfilesModel
    {
        public Guid UserProfileGUID { get; set; }
        public Guid ServiceHistoryGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "OrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
        public Guid OrganizationInstanceGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public Guid DutyStationGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DepartmentGUID", ResourceType = typeof(resxDbFields))]
        public Guid DepartmentGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "JobTitleGUID", ResourceType = typeof(resxDbFields))]
        public Guid JobTitleGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "PositionNumber", ResourceType = typeof(resxDbFields))]
        public string PositionNumber { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Grade", ResourceType = typeof(resxDbFields))]
        public string Grade { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "FromDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        public DateTime? FromDate { get; set; }
        [Display(Name = "ToDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        public DateTime? ToDate { get; set; }
        public bool Active { get; set; }
        public byte[] userProfilesRowVersion { get; set; }
        public List<StepModel> userStepsHistoryModel { get; set; }
        public List<userManagersHistory> userManagersHistoryModel { get; set; }
    }
    public class StepModel
    {
        public Guid UserStepsHistoryGUID { get; set; }
        public Guid UserProfileGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Step", ResourceType = typeof(resxDbFields))]
        public string Step { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "FromDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        public DateTime? FromDate { get; set; }
        [Display(Name = "ToDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        public DateTime? ToDate { get; set; }
        public bool Active { get; set; }
        public byte[] userStepsHistoryRowVersion { get; set; }
    }
    //public class userManagersHistoryModel
    //{
    //    public Guid UserManagersHistoryGUID { get; set; }
    //    public Guid UserProfileGUID { get; set; }
    //    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
    //    [Display(Name = "ManagerTypeGUID", ResourceType = typeof(resxDbFields))]
    //    public Guid ManagerTypeGUID { get; set; }
    //    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
    //    [DataType(DataType.Date)]
    //    [Display(Name = "FromDate", ResourceType = typeof(resxDbFields))]
    //    public DateTime? FromDate { get; set; }
    //    [Display(Name = "ToDate", ResourceType = typeof(resxDbFields))]
    //    [DataType(DataType.Date)]
    //    public DateTime? ToDate { get; set; }
    //    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
    //    [Display(Name = "ManagerGUID", ResourceType = typeof(resxDbFields))]
    //    public Guid ManagerGUID { get; set; }
    //    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
    //    [Display(Name = "IsCconfirmed", ResourceType = typeof(resxDbFields))]
    //    public bool IsCconfirmed { get; set; }
    //    public bool Active { get; set; }
    //    public byte[] userManagersHistoryRowVersion { get; set; }
    //}
    public class OfficeUpdateModel
    {
        public OfficeUpdateModel()
        {
            MediaPath = Constants.NoPhotoTemplate;
        }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "OfficeGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid OfficeGUID { get; set; }
        [StringLength(5, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "OfficePhoneCountryCode", ResourceType = typeof(resxDbFields))]
        public string OfficePhoneCountryCode { get; set; }
        [StringLength(12, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "OfficePhoneNumber", ResourceType = typeof(resxDbFields))]
        public string OfficePhoneNumber { get; set; }
        [StringLength(15, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "VTCIP", ResourceType = typeof(resxDbFields))]
        public string VTCIP { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "OfficeTypeGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid OfficeTypeGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "CountryGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid CountryGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid DutyStationGUID { get; set; }


        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Latitude", ResourceType = typeof(resxDbFields))]
        public double Latitude { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Longitude", ResourceType = typeof(resxDbFields))]
        public double Longitude { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(256, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "OfficeDescription", ResourceType = typeof(resxDbFields))]
        public string OfficeDescription { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(512, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "OfficeAddress", ResourceType = typeof(resxDbFields))]
        public string OfficeAddress { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "OrganizationGUID", ResourceType = typeof(resxDbFields))]
        public Guid OrganizationGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }
        public string MediaPath { get; set; }
        public string MediaName { get; set; }
        public byte[] codeOfficesRowVersion { get; set; }
        public byte[] codeOfficesLanguagesRowVersion { get; set; }
    }
    public class OfficeDataTableModel
    {
        public System.Guid OfficeGUID { get; set; }
        public bool Active { get; set; }
        public byte[] codeOfficesRowVersion { get; set; }
        public string OfficeDescription { get; set; }
        public string OfficeAddress { get; set; }
        public string OfficePhoneNumber { get; set; }
        public string OfficeTypeGUID { get; set; }
        public string CountryGUID { get; set; }
    }
    public class FactorDataTableModel
    {
        public System.Guid FactorGUID { get; set; }
        public string FactorColumnName { get; set; }
        public bool Active { get; set; }
        public byte[] codeFactorsRowVersion { get; set; }
        public string FactorDescription { get; set; }
        public int? SortID { get; set; }
    }
    public class FactorUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "FactorGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid FactorGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(256, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "FactorColumnName", ResourceType = typeof(resxDbFields))]
        public string FactorColumnName { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(128, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "FactorDescription", ResourceType = typeof(resxDbFields))]
        public string FactorDescription { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }
        [Display(Name = "SortID", ResourceType = typeof(resxDbFields))]
        public Nullable<int> SortID { get; set; }
        public byte[] codeFactorsRowVersion { get; set; }
        public byte[] codeFactorsLanguagesRowVersion { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> DeletedOn { get; set; }
    }
    public class ActionCategoriesDataTableModel
    {
        public System.Guid ActionCategoryGUID { get; set; }
        public int ActionCategoryID { get; set; }
        public string ApplicationDescription { get; set; }
        public bool Active { get; set; }
        public byte[] codeActionsCategoriesRowVersion { get; set; }
        public string ActionCategoryDescription { get; set; }
        public int? SortID { get; set; }
    }
    public class ActionCategoryUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ActionCategoryGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ActionCategoryGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ActionCategoryID", ResourceType = typeof(resxDbFields))]
        public int ActionCategoryID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ApplicationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ApplicationGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }
        [Display(Name = "SortID", ResourceType = typeof(resxDbFields))]
        public Nullable<int> SortID { get; set; }
        public byte[] codeActionsCategoriesRowVersion { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(256, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ActionCategoryDescription", ResourceType = typeof(resxDbFields))]
        public string ActionCategoryDescription { get; set; }
        public byte[] codeActionsCategoriesLanguagesRowVersion { get; set; }
    }
    public class ConfigurationModel
    {
        public Guid ValueGuid { get; set; }
        public Guid CTID { get; set; }
        public List<CheckBoxList> CheckBoxList { get; set; }
    }
    public class ActionsDataTableModel
    {
        public System.Guid ActionGUID { get; set; }
        public string ActionVerbDescription { get; set; }
        public int ActionID { get; set; }
        public string ActionCategoryDescription { get; set; }
        public string ApplicationDescription { get; set; }
        public bool Active { get; set; }

        public string ActionType { get; set; }
        public byte[] codeActionsRowVersion { get; set; }
    }
    public class ActionUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ActionGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ActionGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ActionVerbGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ActionVerbGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ActionEntityGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ActionEntityGUID { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "ActionDetails", ResourceType = typeof(resxDbFields))]
        public string ActionDetails { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ActionCategoryGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ActionCategoryGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ApplicationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ApplicationGUID { get; set; }

        [Display(Name = "ActionID", ResourceType = typeof(resxDbFields))]
        public int ActionID { get; set; }

        [Display(Name = "ForAuditPurpose", ResourceType = typeof(resxDbFields))]
        public bool ForAuditPurpose { get; set; }

        public bool Active { get; set; }

        public byte[] codeActionsRowVersion { get; set; }

        public byte[] codeActionsLanguagesRowVersion { get; set; }
    }
    public class OrganizationDepartmentsList
    {
        public Guid OrganizationOperationGUID { get; set; }
        public List<OrganizationDepartmentModel> OrganizationDepartmentModel { get; set; }
    }
    public class OrganizationDepartmentModel
    {
        public bool Checked { get; set; }
        public Guid DepartmentsOrganizationsOperationsGUID { get; set; }
        public string DepartmentDescription { get; set; }
        public string Type { get; set; }
        public string ReportTo { get; set; }
        public bool Active { get; set; }
        public byte[] codeDepartmentsOrganizationsOperationsRowVersion { get; set; }
    }
    //public class OrganizationDepartmentTemplateModel
    //{
    //    public int Index { get; set; }
    //    public OrganizationDepartmentModel OrganizationDepartmentModel { get; set; }
    //}
    public class OrganizationDepartmentUpdateModel
    {
        public Guid OrganizationOperationGUID { get; set; }
        public Guid DepartmentsOrganizationsOperationsGUID { get; set; }
        public string DepartmentDescription { get; set; }
        public Guid Type { get; set; }
        public Guid ReportTo { get; set; }
    }
    public class jsTreeGet
    {
    }
    public class JsTreePost
    {
        public List<JsTreeA_Attr> a_attr { get; set; }
        public string[] children { get; set; }
        public string[] children_d { get; set; }
        public string data { get; set; }
        public bool icon { get; set; }
        public string id { get; set; }
        public JsTreeLI_Attr li_attr { get; set; }
        //public List<original> original { get; set; } Skipped
        public string parent { get; set; }
        public string[] parents { get; set; }
        public JsTreeState state { get; set; }
        public string text { get; set; }
    }
    public class JsTreeA_Attr
    {
        public string href { get; set; }
        public string id { get; set; }
    }
    public class JsTreeLI_Attr
    {
        public string actionGUID { get; set; }
        public string factorstoken { get; set; }
        public bool status { get; set; }
    }
    public class JsTreeState
    {
        //public bool disabled { get; set; }
        //public bool failed { get; set; }
        //public bool loaded { get; set; }
        //public bool loading { get; set; }
        public bool opened { get; set; }
        public bool selected { get; set; }
    }
    public class SitemapUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SitemapGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid SitemapGUID { get; set; }
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ActionUrl", ResourceType = typeof(resxDbFields))]
        public string ActionUrl { get; set; }
        [Display(Name = "ParentGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ParentGUID { get; set; }
        [Display(Name = "SortID", ResourceType = typeof(resxDbFields))]
        public Nullable<int> SortID { get; set; }
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }
        public byte[] codeSitemapsRowVersion { get; set; }
        public byte[] codeSitemapsLanguagesRowVersion { get; set; }
        [StringLength(150, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SitemapDescription", ResourceType = typeof(resxDbFields))]
        public string SitemapDescription { get; set; }
    }
    public class SitemapDataTableModel
    {
        public System.Guid SitemapGUID { get; set; }
        public string ActionUrl { get; set; }
        public string ParentDescription { get; set; }
        public Nullable<int> SortID { get; set; }
        public string SitemapDescription { get; set; }
        public bool Active { get; set; }
        public byte[] codeSitemapsRowVersion { get; set; }
    }
    public class ActionCore
    {
        public Guid ActionGUID { get; set; }
        public int ActionID { get; set; }
        public char Status { get; set; }
    }
    public class PermissionTreeNode
    {
        public Guid ID { get; set; }
        public string Text { get; set; }
        public Guid? ParentID { get; set; }
    }
    public class PermissionTreeCategoryNode : PermissionTreeNode
    {
        public List<FactorArray> FactorsArray { get; set; }
    }
    public class FactorArray
    {
        public Guid FactorTypeID { get; set; }
        public string DependsOn { get; set; }
        public bool Purpose { get; set; }
    }
    public class PermissionsTreeFilter
    {
        public Guid UserProfileGUID { get; set; }
        public Guid ApplicationGUID { get; set; }
        public Guid CategoryGUID { get; set; }
        public Guid ActionGUID { get; set; }
        public Guid UNHCRBureauGUID { get; set; }
        public List<Guid> OperationGUIDs { get; set; }
        public List<Guid> OrganizationGUIDs { get; set; }
        public List<Guid> GovernorateGUIDs { get; set; }
        public List<Guid> CountryGUIDs { get; set; }
    }
    public class MapModel
    {
        public string Height { get; set; }
        public int Zoom { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
    public class Sitemap
    {
        public Guid ID { get; set; }
        public string URL { get; set; }
        public string Description { get; set; }
    }
    public class LookupTable
    {
        public Guid TableID { get; set; }
        public string TableName { get; set; }
        public Guid IndexSitemapID { get; set; }
        public Guid DetailsSitemapID { get; set; }
    }
    public class codeTablesModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "TableGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid TableGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(60, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "TableName", ResourceType = typeof(resxDbFields))]
        public string TableName { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "IndexSitemapDesciption", ResourceType = typeof(resxDbFields))]
        public string IndexSitemapDesciption { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DetailsSitemapDesciption", ResourceType = typeof(resxDbFields))]
        public string DetailsSitemapDesciption { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }
        public byte[] codeTablesRowVersion { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> DeletedOn { get; set; }
    }
    public class JobModel
    {
        public Guid UserProfileGUID { get; set; }
        [DataType(DataType.Date)]
        public DateTime FromDate { get; set; }
        public string JobTitle { get; set; }
        public string Grade { get; set; }
        public string PositionNumber { get; set; }
        public string OrganizationInstance { get; set; }
        public string DutyStation { get; set; }
        public string Department { get; set; }
        public ICollection<userStepsHistory> StepHistoryList { get; set; }
        public ICollection<userManagersHistory> ManagerHistoryList { get; set; }
    }

    public class FormContorlsModel
    {
        public Guid PK { get; set; }
        public bool Active { get; set; }
    }
    public class WorkingDaysModel
    {
        public Guid? WorkingDaysConfigurationGUID { get; set; }

        public Guid? DutyStationConfigurationGUID { get; set; }

        public Guid DayGUID { get; set; }

        public string Day { get; set; }

        //[DataType(DataType.Time)] commented because no values will be posted
        public TimeSpan? FromTime { get; set; }

        //[DataType(DataType.Time)]
        public TimeSpan? ToTime { get; set; }

        public bool Status { get; set; }

        public bool Active { get; set; }

        public byte[] codeWorkingDaysConfigurationsRowVersion { get; set; }
    }

    public class WorkingDaysConfigModel
    {
        public Guid OrganizationInstanceGUID { get; set; }
        public Guid DutyStationsGUID { get; set; }
        public Guid DutyStationConfigurationGUID { get; set; }

        public List<WorkingDaysModel> WorkingDaysModelList { get; set; }
    }
    public class CountryCodeDDL
    {
        public string ControlID { get; set; }
        public string SelectedValue { get; set; }
    }

    public class DutyStationConfiguration
    {
        public string Country { get; set; }
        public List<CheckBoxPair> DutyStations { get; set; }
        public Guid OrganizationInstanceGUID { get; set; }
    }
    public class CheckBoxPair
    {
        public Guid Value { get; set; }
        public string Description { get; set; }
    }

    public class ActionVerbUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ActionVerbGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ActionVerbGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [RegularExpression(@"^\D+$", ErrorMessageResourceName = "CharacterString", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(50, MinimumLength = 3, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ActionVerbDescription", ResourceType = typeof(resxDbFields))]
        public string ActionVerbDescription { get; set; }

        public byte[] codeActionsVerbsRowVersion { get; set; }
        public byte[] codeActionsVerbsLanguagesRowVersion { get; set; }
    }

    public class ActionsVerbsDataTableModel
    {
        public System.Guid ActionVerbGUID { get; set; }
        public bool Active { get; set; }

        public string ActionVerbDescription { get; set; }

        public byte[] codeActionsVerbsRowVersion { get; set; }
    }

    public class ActionEntityUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ActionEntityGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ActionEntityGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(50, MinimumLength = 3, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [RegularExpression(@"^\D+$", ErrorMessageResourceName = "CharacterString", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ActionEntityDescription", ResourceType = typeof(resxDbFields))]
        public string ActionEntityDescription { get; set; }

        public byte[] codeActionsEntitiesRowVersion { get; set; }
        public byte[] codeActionsEntitiesLanguagesRowVersion { get; set; }
    }

    public class ActionsEntitiesDataTableModel
    {
        public System.Guid ActionEntityGUID { get; set; }
        public bool Active { get; set; }

        public string ActionEntityDescription { get; set; }

        public byte[] codeActionsEntitiesRowVersion { get; set; }
    }

    public class UserDropDownList
    {
        public Guid id { get; set; } //UserGUID. Dont change property name, this is a mandatory when using select2 with custom template. Ayas
        public string name { get; set; } //FullName. Dont change property name, this is a mandatory when using select2 with custom template. Ayas
        public string PhotoPath { get; set; }
        public string JobTitle { get; set; }
        public string Organization { get; set; }

    }

    public class AuditModel
    {
        public string ActionDescription { get; set; }
        public Guid ActionGUID { get; set; }
        public string ExecutedBy { get; set; }
        public DateTime ExecutionTime { get; set; }
        public Nullable<System.DateTime> LocalExecutionTime { get { return new Portal().LocalTime(this.ExecutionTime); } }
        public string JobTitleDescription { get; set; }
        public string OrganizationInstanceDescription { get; set; }
        public string jsFunction { get; set; }
        public List<AuditFieldsModel> UpdatedFields { get; set; }
    }

    public class AuditFieldsModel
    {
        public string FieldName { get; set; }
        public string AfterChange { get; set; }
        public string BeforeChange { get; set; }

    }

    public class AuditReportFilterModel
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

        [Display(Name = "ApplicationGUID", ResourceType = typeof(resxDbFields))]
        public Guid? ApplicationGUID { get; set; }
        [Display(Name = "ActionCategoryGUID", ResourceType = typeof(resxDbFields))]
        public Guid? ActionCategoryGUID { get; set; }
        [Display(Name = "ActionGUID", ResourceType = typeof(resxDbFields))]
        public Guid? ActionGUID { get; set; }
        [Display(Name = "ActionVerbGUID", ResourceType = typeof(resxDbFields))]
        public Guid? ActionVerbGUID { get; set; }
        [Display(Name = "ExecutedByUserGUID", ResourceType = typeof(resxDbFields))]
        public Guid? ExecutedByUserGUID { get; set; }

        [Display(Name = "FieldName", ResourceType = typeof(resxDbFields))]
        public string FieldName { get; set; }
        [Display(Name = "BeforeChange", ResourceType = typeof(resxDbFields))]
        public string BeforeChange { get; set; }
        [Display(Name = "AfterChange", ResourceType = typeof(resxDbFields))]
        public string AfterChange { get; set; }
        [Display(Name = "OrganizationGUID", ResourceType = typeof(resxDbFields))]
        public Guid? OrganizationGUID { get; set; }
        [Display(Name = "RankID", ResourceType = typeof(resxDbFields))]
        public int RankID { get; set; }
        [Display(Name = "OrderBy", ResourceType = typeof(resxDbFields))]
        public string OrderBy { get; set; }

        //not used
        public Guid? ExecutedByUserProfileGUID { get; set; }


    }

    public class IPApiResult
    {
        //"geobytesforwarderfor":"",
        //"geobytesremoteip":"37.48.143.65",
        //"geobytesipaddress":"178.52.120.202",
        //"geobytescertainty":"100",
        //"geobytesinternet":"SY",
        //"geobytescountry":"Syria",
        //"geobytesregionlocationcode":"SYDI",
        //"geobytesregion":"Dimashq",
        //"geobytescode":"DI",
        //"geobyteslocationcode":"SYDIDAMA",
        //"geobytesdma":"0",
        //"geobytescity":"Damascus",
        //"geobytescityid":"6738",
        //"geobytesfqcn":"Damascus, DI, Syria",
        //"geobyteslatitude":"33.500000",
        //"geobyteslongitude":"36.299999",
        //"geobytescapital":"Damascus",
        //"geobytestimezone":"+02:00",
        //"geobytesnationalitysingular":"Syrian",
        //"geobytespopulation":"16728808",
        //"geobytesnationalityplural":"Syrians",
        //"geobytesmapreference":"Middle East ",
        //"geobytescurrency":"Syrian Pound",
        //"geobytescurrencycode":"SYP",
        //"geobytestitle":"Syria"

        [JsonProperty(PropertyName = "geobytesinternet")]
        public string CountryCode { get; set; }

        [JsonProperty(PropertyName = "geobytescountry")]
        public string CountryName { get; set; }

        [JsonProperty(PropertyName = "geobytescity")]
        public string City { get; set; }

        [JsonProperty(PropertyName = "geobytestimezone")]
        public string TimeZone { get; set; }

        [JsonProperty(PropertyName = "geobyteslatitude")]
        public string Latitude { get; set; }

        [JsonProperty(PropertyName = "geobyteslongitude")]
        public string Longitude { get; set; }
    }
    public class SponsorMessageModel
    {
        public string RequestID { get; set; }
        public string UserFullName { get; set; }
        public string SponsorFullName { get; set; }
        public string SponsorJobTitle { get; set; }
        public string OrganizationInstance { get; set; }
        public string SponsorEmailAddress { get; set; }

    }

    public class ConnectionUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ConnectionGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ConnectionGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "InstanceDescription", ResourceType = typeof(resxDbFields))]
        public string InstanceDescription { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Provider", ResourceType = typeof(resxDbFields))]
        public string Provider { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Metadata", ResourceType = typeof(resxDbFields))]
        public string Metadata { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DataSource", ResourceType = typeof(resxDbFields))]
        public string DataSource { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "InitialCatalog", ResourceType = typeof(resxDbFields))]
        public string InitialCatalog { get; set; }

        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "UserID", ResourceType = typeof(resxDbFields))]
        public string UserID { get; set; }

        [DataType(DataType.Password)]
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Password", ResourceType = typeof(resxDbFields))]
        public string Password { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "OrganizationsInstancesGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid OrganizationsInstancesGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid DutyStationGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ApplicationGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ApplicationGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] codeConnectionsRowVersion { get; set; }
    }

    public class ConnectionDataTableModel
    {
        public System.Guid ConnectionGUID { get; set; }
        public string InstanceDescription { get; set; }
        public string DataSource { get; set; }
        public string InitialCatalog { get; set; }
        public string OrganizationsInstancesDescription { get; set; }
        public string ApplicationGUID { get; set; }
        public string ApplicationDescription { get; set; }
        public bool DefaultConnection { get; set; }
        public bool Active { get; set; }
        public byte[] codeConnectionsRowVersion { get; set; }
    }

    public class NotificationDataTableModel
    {
        public System.Guid NotificationGUID { get; set; }
        public string ApplicationGUID { get; set; }
        public string TitleTemplete { get; set; }
        public string ApplicationDescription { get; set; }
        public string ApplicationAcrynom { get; set; }
        public byte[] codeNotificationsRowVersion { get; set; }
        public bool Active { get; set; }
    }
    public class UserNotificationViewModel
    {
        public System.Guid NotificationGUID { get; set; }
        public System.Guid ApplicationGUID { get; set; }

        public string TitleTemplate { get; set; }
        public bool Block { get; set; }
    }
    public class NotificationUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "NotificationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid NotificationGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "UserProfileIcon", ResourceType = typeof(resxDbFields))]
        public bool UserProfileIcon { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Icon", ResourceType = typeof(resxDbFields))]
        public string Icon { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ApplicationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ApplicationGUID { get; set; }

        [StringLength(150, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "RedirectURL", ResourceType = typeof(resxDbFields))]
        public string RedirectURL { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "TitleTemplete", ResourceType = typeof(resxDbFields))]
        public string TitleTemplete { get; set; }

        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DetailsTemplete", ResourceType = typeof(resxDbFields))]
        public string DetailsTemplete { get; set; }

        [StringLength(2000, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MailTemplete", ResourceType = typeof(resxDbFields))]
        public string MailTemplete { get; set; }

        public byte[] codeNotificationLanguagesRowVersion { get; set; }
        public byte[] codeNotificationsRowVersion { get; set; }
    }


    public class UsersDataTableModel
    {
        public Guid? UserGUID { get; set; }
        public string FullName { get; set; }
        public string EmailAddress { get; set; }
        public Guid? CountryGUID { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string OrganizationInstanceGUID { get; set; }
        public string OrganizationInstanceDescription { get; set; }
        public string DutyStationDescription { get; set; }
        public string DutyStationGUID { get; internal set; }
        public string DepartmentDescription { get; set; }
        public string JobTitleDescription { get; set; }
        public string Grade { get; set; }
        public DateTime? LastLogIn { get; set; }
        public DateTime? AccountExpiredOn { get; set; }
        public bool Active { get; set; }
        public byte[] userAccountsRowVersion { get; set; }
    }

    public class UsersUpdateModel
    {
        public Guid? UserGUID { get; set; }
        public string UserID { get; set; }
        public string EmailAddress { get; set; }
        public string GivenName { get; set; }
        public string SurName { get; set; }
        public Guid? OrganizationInstanceGUID { get; set; }
        public Guid? DutyStationGUID { get; set; }
        public Guid? DepartmentGUID { get; set; }
        public Guid? JobTitleGUID { get; set; }
        public bool Active { get; set; }
        public string OfficeLandlineCountryCode { get; set; }
        public string OfficeLandlineAreaCode { get; set; }
        public string OfficeLandlineNumber { get; set; }
        public string ExtensionNumber { get; set; }
        public string HiddenPhoneNumber { get; set; }
        public DateTime? AccountExpiredOn { get; set; }
        public Nullable<System.DateTime> PasswordLastUpdate { get; set; }
        public Nullable<System.DateTime> LastLogIn { get; set; }
    }

    public class ActiveDirectoryUser
    {
        public string sn { get; set; }
        public string samaccountname { get; set; }
        public string mail { get; set; }
        public string displayname { get; set; }
        public string telephoneNumber { get; set; }
        public string title { get; set; }
        public string department { get; set; }
        public string givenName { get; set; }
        public string co { get; set; }
        public string l { get; set; }
        public string container { get; set; }
        public string countryCode { get; set; }
        public string areaCode { get; set; }
        public string organizationInstanceGUID { get; set; }
    }


    public class ShowMyInfoModel
    {
        public string BrowserName { get; set; }
        public string UserHostAddress { get; set; }
        public string UserHostName { get; set; }
    }

    public class ApplicationAccessDataTableModel
    {
        public Guid AppAccessAuditGUID { get; set; }
        public Guid UserGUID { get; set; }
        public string FullName { get; set; }
        public string EmailAddress { get; set; }
        public Guid ApplicationGUID { get; set; }
        public string ApplicationDescription { get; set; }
        public string ApplicationAcrynom { get; set; }
        public DateTime AccessedOn { get; set; }
    }

    public class UserPermissionsAuditDataTable
    {
        public Guid UserGUID { get; set; }
        public string FullName { get; set; }
        public string EmailAddress { get; set; }
        public string ActionCategoryDescription { get; set; }
        public string ActionEntityDescription { get; set; }
        public string ActionVerbDescription { get; set; }
        public string ApplicationAcrynom { get; set; }
        public string ApplicationDescription { get; set; }
        public string ApplicationGUID { get; set; }

    }
    #region Staff Profile
    public class StaffProfileReadlOnlyModel
    {
        public StaffProfileReadlOnlyModel()
        {
            MediaPath = "/Assets/Images/img.png";
        }
        public string ActiveDirectorySearchInput { get; set; }

        public string MediaPath { get; set; }

        public string LastConfirmationStatus { get; set; }

        public Guid? LastConfirmationStatusGUID { get; set; }

        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        //[Display(Name = "AssignmentType", ResourceType = typeof(resxDbFields))]

        public Guid UserGUID { get; set; }

        public string RecruitmentType { get; set; }
        public string PlaceOfBirth { get; set; }
        public string Nationality1 { get; set; }
        public string Nationality2 { get; set; }
        public string Nationality3 { get; set; }
        public string JobTitleMoFAEN { get; set; }

        public string ContractType { get; set; }

        public string DutyStation { get; set; }
        public string Department { get; set; }
        public string Grade { get; set; }
        public string ReportTo { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "EmailAddress", ResourceType = typeof(resxDbFields))]
        public string EmailAddress { get; set; }

        [Display(Name = "IsInternational", ResourceType = typeof(resxDbFields))]
        public bool? IsInternational { get; set; }

        [Display(Name = "StaffPhoto", ResourceType = typeof(resxDbFields))]
        public string StaffPhoto { get; set; }


        public HttpPostedFileBase StaffPhotoFile { get; set; }


        [Display(Name = "StaffStatus", ResourceType = typeof(resxDbFields))]
        public string StaffStatus { get; set; }


        [Display(Name = "PositionInOrganigram", ResourceType = typeof(resxDbFields))]
        public string PositionInOrganigram { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "JobTilteMOFAEN", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> JobTitleGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public Guid DutyStationGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public Guid? StaffStatusGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "StaffPrefixGUID", ResourceType = typeof(resxDbFields))]
        public Guid? StaffPrefixGUID { get; set; }




        [Display(Name = "DepartmentGUID", ResourceType = typeof(resxDbFields))]
        public Guid? DepartmentGUID { get; set; }

        [Display(Name = "ReportToGUID", ResourceType = typeof(resxDbFields))]
        public Guid? ReportToGUID { get; set; }

        [Display(Name = "NationalityGUID", ResourceType = typeof(resxDbFields))]
        public Guid? NationalityGUID { get; set; }

        [Display(Name = "Nationality2GUID", ResourceType = typeof(resxDbFields))]
        public Guid? Nationality2GUID { get; set; }

        [Display(Name = "Nationality3GUID", ResourceType = typeof(resxDbFields))]
        public Guid? Nationality3GUID { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]
        [Display(Name = "ExpiryOfResidencyVisa", ResourceType = typeof(resxDbFields))]
        public DateTime? ExpiryOfResidencyVisa { get; set; }

        [NotMapped]
        [Display(Name = "ExpiryOfResidencyVisa", ResourceType = typeof(resxDbFields))]
        public DateTime? LocalExpiryOfResidencyVisa { get { return new Portal().LocalTime(this.ExpiryOfResidencyVisa); } }

        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SyrianNationalIDNumber", ResourceType = typeof(resxDbFields))]
        public string SyrianNationalIDNumber { get; set; }

        [Display(Name = "SyrianNationalIDPhoto", ResourceType = typeof(resxDbFields))]
        public string SyrianNationalIDPhoto { get; set; }

        public HttpPostedFileBase SyrianNationalIDPhotoFile { get; set; }



        [StringLength(10, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "StaffPrefix", ResourceType = typeof(resxDbFields))]
        public string StaffPrefix { get; set; }

        [Display(Name = "PlaceOfBirthGUID", ResourceType = typeof(resxDbFields))]
        public Guid? PlaceOfBirthGUID { get; set; }


        [Display(Name = "EmploymentID", ResourceType = typeof(resxDbFields))]
        public int? EmploymentID { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "UNHCRIDNumber", ResourceType = typeof(resxDbFields))]
        public string UNHCRIDNumber { get; set; }

        [Display(Name = "CallSign", ResourceType = typeof(resxDbFields))]
        public string CallSign { get; set; }

        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "StaffLastWork", ResourceType = typeof(resxDbFields))]
        public string StaffLastWork { get; set; }

        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "UNLPNumber", ResourceType = typeof(resxDbFields))]
        public string UNLPNumber { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]
        [Display(Name = "UNLPDateOfIssue", ResourceType = typeof(resxDbFields))]
        public DateTime? UNLPDateOfIssue { get; set; }

        [NotMapped]
        [Display(Name = "UNLPDateOfIssue", ResourceType = typeof(resxDbFields))]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]
        public DateTime? LocalUNLPDateOfIssue { get { return new Portal().LocalTime(this.UNLPDateOfIssue); } }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]
        [Display(Name = "UNLPDateOfExpiry", ResourceType = typeof(resxDbFields))]
        public DateTime? UNLPDateOfExpiry { get; set; }

        [NotMapped]
        [Display(Name = "UNLPDateOfExpiry", ResourceType = typeof(resxDbFields))]
        public DateTime? LocalUNLPDateOfExpiry { get { return new Portal().LocalTime(this.UNLPDateOfExpiry); } }

        [Display(Name = "UNLPPhoto", ResourceType = typeof(resxDbFields))]
        public string UNLPPhoto { get; set; }
        public HttpPostedFileBase UNLPPhotoFile { get; set; }

        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "NationalPassportNumber", ResourceType = typeof(resxDbFields))]
        public string NationalPassportNumber { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "NationalPassportDateOfIssue", ResourceType = typeof(resxDbFields))]
        public DateTime? NationalPassportDateOfIssue { get; set; }

        [NotMapped]
        [Display(Name = "NationalPassportDateOfIssue", ResourceType = typeof(resxDbFields))]
        public DateTime? LocalNationalPassportDateOfIssue { get { return new Portal().LocalTime(this.NationalPassportDateOfIssue); } }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]
        [Display(Name = "NationalPassportDateOfExpiry", ResourceType = typeof(resxDbFields))]
        public DateTime? NationalPassportDateOfExpiry { get; set; }

        [NotMapped]
        [Display(Name = "NationalPassportDateOfExpiry", ResourceType = typeof(resxDbFields))]
        public DateTime? LocalNationalPassportDateOfExpiry { get { return new Portal().LocalTime(this.NationalPassportDateOfExpiry); } }

        [Display(Name = "NationalPassportPhoto", ResourceType = typeof(resxDbFields))]
        public string NationalPassportPhoto { get; set; }
        public HttpPostedFileBase NationalPassportPhotoFile { get; set; }




        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]

        [Display(Name = "ContractTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid? ContractTypeGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "RecruitmentTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid? RecruitmentTypeGUID { get; set; }

        [Display(Name = "StaffGradeGUID", ResourceType = typeof(resxDbFields))]
        public Guid? StaffGradeGUID { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]
        [Display(Name = "ContractStartDate", ResourceType = typeof(resxDbFields))]
        public DateTime? ContractStartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]
        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ContractEndDate", ResourceType = typeof(resxDbFields))]
        public DateTime? ContractEndDate { get; set; }

        [NotMapped]
        [Display(Name = "ContractEndDate", ResourceType = typeof(resxDbFields))]
        public DateTime? LocalContractEndDate { get { return new Portal().LocalTime(this.ContractEndDate); } }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]
        [Display(Name = "StaffEOD", ResourceType = typeof(resxDbFields))]
        public DateTime? StaffEOD { get; set; }

        [NotMapped]
        [Display(Name = "StaffEOD", ResourceType = typeof(resxDbFields))]
        public DateTime? LocalStaffEOD { get { return new Portal().LocalTime(this.StaffEOD); } }

        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "PositionNumber", ResourceType = typeof(resxDbFields))]
        public string PositionNumber { get; set; }

        [Display(Name = "PermanentAddressEn", ResourceType = typeof(resxDbFields))]
        public string PermanentAddressEn { get; set; }

        [Display(Name = "PermanentAddressAr", ResourceType = typeof(resxDbFields))]
        public string PermanentAddressAr { get; set; }

        [Display(Name = "CurrentAddressEn", ResourceType = typeof(resxDbFields))]
        public string CurrentAddressEn { get; set; }

        [Display(Name = "CurrentAddressAr", ResourceType = typeof(resxDbFields))]
        public string CurrentAddressAr { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "HomeTelephoneNumberLandline", ResourceType = typeof(resxDbFields))]
        public string HomeTelephoneNumberLandline { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "HomeTelephoneNumberMobile", ResourceType = typeof(resxDbFields))]
        public string HomeTelephoneNumberMobile { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "OfficialMobileNumber", ResourceType = typeof(resxDbFields))]
        public string OfficialMobileNumber { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "HQExtensionNumber", ResourceType = typeof(resxDbFields))]
        public string HQExtensionNumber { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DamascusExtensionNumber", ResourceType = typeof(resxDbFields))]
        public string DamascusExtensionNumber { get; set; }


        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "OfficialExtensionNumber", ResourceType = typeof(resxDbFields))]
        public string OfficialExtensionNumber { get; set; }

        [Display(Name = "OfficeGUID", ResourceType = typeof(resxDbFields))]
        public Guid? OfficeGUID { get; set; }

        [Display(Name = "OfficeFloorGUID", ResourceType = typeof(resxDbFields))]
        public Guid? OfficeFloorGUID { get; set; }

        [Display(Name = "OfficeRoomGUID", ResourceType = typeof(resxDbFields))]
        public Guid? OfficeRoomGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] StaffCoreDataRowVersion { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public DateTime? DeletedOn { get; set; }

        //personal details table
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "FirstNameEN", ResourceType = typeof(resxDbFields))]
        public string FirstNameEN { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SurnameEN", ResourceType = typeof(resxDbFields))]
        public string SurnameEN { get; set; }


        [Display(Name = "FirstNameAR", ResourceType = typeof(resxDbFields))]
        public string FirstNameAR { get; set; }


        [Display(Name = "SurnameAR", ResourceType = typeof(resxDbFields))]
        public string SurnameAR { get; set; }

        [Display(Name = "PlaceOfBirthCountryAR", ResourceType = typeof(resxDbFields))]
        public string PlaceOfBirthCountryAR { get; set; }


        [Display(Name = "FatherNameEN", ResourceType = typeof(resxDbFields))]
        public string FatherNameEN { get; set; }


        [Display(Name = "FatherNameAR", ResourceType = typeof(resxDbFields))]
        public string FatherNameAR { get; set; }


        [Display(Name = "MotherNameEN", ResourceType = typeof(resxDbFields))]
        public string MotherNameEN { get; set; }


        [Display(Name = "MotherNameAR", ResourceType = typeof(resxDbFields))]
        public string MotherNameAR { get; set; }


        [Display(Name = "GenderGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> GenderGUID { get; set; }


        [Display(Name = "BloodGroup", ResourceType = typeof(resxDbFields))]
        public string BloodGroup { get; set; }

        [Display(Name = "DateOfBirth", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]


        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "PlaceBirthCityEn", ResourceType = typeof(resxDbFields))]
        public string PlaceBirthCityEn { get; set; }

        [Display(Name = "PlaceBirthCityAr", ResourceType = typeof(resxDbFields))]
        public string PlaceBirthCityAr { get; set; }
        [Display(Name = "LastJobEn", ResourceType = typeof(resxDbFields))]
        public string LastJobEn { get; set; }
        [Display(Name = "LastJobAr", ResourceType = typeof(resxDbFields))]
        public string LastJobAr { get; set; }
        [Display(Name = "DegreeEn", ResourceType = typeof(resxDbFields))]
        public string DegreeEn { get; set; }

        [Display(Name = "JobTilteMOFAAR", ResourceType = typeof(resxDbFields))]
        public string JobTitleAR { get; set; }
        [Display(Name = "DegreeAr", ResourceType = typeof(resxDbFields))]
        public string DegreeAr { get; set; }
        [Display(Name = "OfficeRoomNumberBuilding", ResourceType = typeof(resxDbFields))]
        public string OfficeRoomNumberBuilding { get; set; }
        [Display(Name = "SATPhoneNumber", ResourceType = typeof(resxDbFields))]
        public string SATPhoneNumber { get; set; }

        [Display(Name = "Nationality1Arabic", ResourceType = typeof(resxDbFields))]
        public string Nationality1Arabic { get; set; }
        [Display(Name = "Nationality3Arabic", ResourceType = typeof(resxDbFields))]

        public string Nationality3Arabic { get; set; }
        [Display(Name = "Nationality2Arabic", ResourceType = typeof(resxDbFields))]
        public string Nationality2Arabic { get; set; }

        #region Bank Syria

        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "BankNameSyrGUID", ResourceType = typeof(resxDbFields))]
        public string BankNameSyrGUID { get; set; }



        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "BankNameSyr", ResourceType = typeof(resxDbFields))]
        public string BankNameSyr { get; set; }

        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "BankAccountNumberSyr", ResourceType = typeof(resxDbFields))]
        public string BankAccountNumberSyr { get; set; }

        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "BankAccountHolderNameEn", ResourceType = typeof(resxDbFields))]
        public string BankAccountHolderNameEn { get; set; }

        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "BankAccountHolderNameAr", ResourceType = typeof(resxDbFields))]
        public string BankAccountHolderNameAr { get; set; }


        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "TitleEnglishMSRP", ResourceType = typeof(resxDbFields))]
        public string TitleEnglishMSRP { get; set; }

        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "TitleArabicMSRP", ResourceType = typeof(resxDbFields))]
        public string TitleArabicMSRP { get; set; }

        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "TitleEnglishMOFA", ResourceType = typeof(resxDbFields))]
        public string TitleEnglishMOFA { get; set; }

        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "TitleArabicMOFA", ResourceType = typeof(resxDbFields))]
        public string TitleArabicMOFA { get; set; }




        #endregion

        #region Security Info Section
        [Display(Name = "NextOfKinName", ResourceType = typeof(resxDbFields))]
        public string NextOfKinName { get; set; }

        [Display(Name = "KinMobileNumber", ResourceType = typeof(resxDbFields))]
        public string KinMobileNumber { get; set; }

        [Display(Name = "BSAFECertAcquired", ResourceType = typeof(resxDbFields))]
        public Nullable<bool> BSAFECertAcquired { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "BSAFEExpiryDate", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> BSAFEExpiryDate { get; set; }

        [Display(Name = "NumberOfDependants", ResourceType = typeof(resxDbFields))]
        public Nullable<int> NumberOfDependants { get; set; }

        [Display(Name = "DependantsName", ResourceType = typeof(resxDbFields))]
        public string DependantsName { get; set; }
        #endregion
    }

    public class StafFSecurityUnitInfoModel
    {
        public Guid UserGUID { get; set; }

        [Display(Name = "NextOfKinName", ResourceType = typeof(resxDbFields))]
        public string NextOfKinName { get; set; }

        [Display(Name = "KinMobileNumber", ResourceType = typeof(resxDbFields))]
        public string KinMobileNumber { get; set; }

        [Display(Name = "BSAFECertAcquired", ResourceType = typeof(resxDbFields))]
        public Nullable<bool> BSAFECertAcquired { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "BSAFEExpiryDate", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> BSAFEExpiryDate { get; set; }

        [Display(Name = "NumberOfDependants", ResourceType = typeof(resxDbFields))]
        public Nullable<int> NumberOfDependants { get; set; }

        [Display(Name = "DependantsName", ResourceType = typeof(resxDbFields))]
        public string DependantsName { get; set; }

        public bool Active { get; set; }
    }
    #endregion

    public class RegisterNewUpdateModel
    {
        public Guid UserGUID { get; set; }


        [RegularExpression(@"^\D+$", ErrorMessageResourceName = "CharacterString", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(50, MinimumLength = 3, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "FirstName", ResourceType = typeof(resxDbFields))]
        public string FirstName { get; set; }


        [RegularExpression(@"^\D+$", ErrorMessageResourceName = "CharacterString", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(50, MinimumLength = 3, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Surname", ResourceType = typeof(resxDbFields))]
        public string SurName { get; set; }


        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessageResourceName = "InvalidEmailAddress", ErrorMessageResourceType = typeof(resxMessages))]
        [Display(Name = "EmailAddress", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "NewPassword", ResourceType = typeof(resxDbFields))]
        [StringLength(30, MinimumLength = 8, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.Password)]
        [RegularExpression("(?=.*?[A-Z])(?=.*?[a-z])((?=.*?[0-9])|(?=.*?[#?!@$%^&*-])).{8,}", ErrorMessageResourceType = typeof(resxValidations), ErrorMessageResourceName = "PasswordValidation")]
        public string Password { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ConfirmPassword", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare(nameof(Password), ErrorMessageResourceType = typeof(resxValidations), ErrorMessageResourceName = "PasswordNotMatch")]
        public string ConfirmPassword { get; set; }

        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        //[Display(Name = "OrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
        //public Guid OrganizationInstanceGUID { get; set; }

        public bool Active { get; set; }
    }
    public class AppInfoViewModel
    {
        public Guid ApplicationGUID { get; set; }
        public Guid? StaffApplicationAccessRequestGUID { get; set; }

        public string AccessType { get; set; }

        public int? AccessGranted { get; set; }
        public string StaffName { get; set; }
        public string StaffEmailAddress { get; set; }
        public string AppOwnerEmailAddress { get; set; }

        public Guid UserGUID { get; set; }
        public bool Active { get; set; }
        public string AboutApplication { get; set; }
        public string ApplicationOwner { get; set; }
        public string Unit { get; set; }
        public Guid? ApplicationOwnerGUID { get; set; }
        public string ApplicationName { get; set; }
        public string Description { get; set; }
        public string ApplicationCategoryName { get; set; }
        public string AppOwnerName { get; set; }
        public string StatusClass { get; set; }
        public string URL { get; set; }
        public string Status { get; set; }
        public int? AppAccess { get; set; }

    }

}
