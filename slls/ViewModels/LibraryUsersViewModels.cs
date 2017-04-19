using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using slls.App_Settings;
using slls.Localization;
using slls.Models;

namespace slls.ViewModels
{
    public class LibraryUserAddViewModel
    {
        public string Id { get; set; }

        public int LibraryUserId { get; set; }

        [Required]
        [StringLength(255)]
        [LocalDisplayName("Users.Username", "FieldDisplayName")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [RegularExpression("^[a-z0-9_\\+-]+(\\.[a-z0-9_\\+-]+)*@[a-z0-9-]+(\\.[a-z0-9]+)*\\.([a-z]{2,4})$", ErrorMessage = "Invalid email address!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A Password is required")]
        [LocalDisplayName("Users.Password", "FieldDisplayName")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm the Password")]
        [DataType(DataType.Password)]
        [LocalDisplayName("Users.Confirm_Password", "FieldDisplayName")]
        [System.ComponentModel.DataAnnotations.Compare("Password")]
        public string ConfirmPassword { get; set; }

        public int PasswordMinLength { get; set; }
        public int PasswordMaxLength { get; set; }
        public bool PasswordRequireNonLetterOrDigit { get; set; }
        public bool PasswordRequireDigit { get; set; }
        public bool PasswordRequireLowercase { get; set; }
        public bool PasswordRequireUppercase { get; set; }

        [AllowHtml]
        public string PasswordTip { get; set; }

        [Required]
        [LocalDisplayName("Users.Firstnames", "FieldDisplayName")]
        public string Firstname { get; set; }

        [Required]
        [LocalDisplayName("Users.Lastnames", "FieldDisplayName")]
        public string Lastname { get; set; }

        [LocalDisplayName("Users.Barcode", "FieldDisplayName")]
        public string UserBarcode { get; set; }

        [LocalDisplayName("Genders.Gender", "FieldDisplayName")]
        public int? GenderId { get; set; }

        [LocalDisplayName("Users.Is_Live", "FieldDisplayName")]
        public bool IsLive { get; set; }

        [LocalDisplayName("Locations.Location", "FieldDisplayName")]
        public int? LocationId { get; set; }

        [LocalDisplayName("Departments.Department", "FieldDisplayName")]
        public int? DepartmentId { get; set; }

        [LocalDisplayName("Classes.Class", "FieldDisplayName")]
        public int? ClassId { get; set; }

        [LocalDisplayName("Cohorts.Cohort", "FieldDisplayName")]
        public int? CohortId { get; set; }

        [LocalDisplayName("UserType.UserType", "FieldDisplayName")]
        public int? UserTypeID { get; set; }

        [LocalDisplayName("Users.Reading_Group", "FieldDisplayName")]
        public int? ReadingGroupID { get; set; }

        [LocalDisplayName("Users.Position", "FieldDisplayName")]
        public string Position { get; set; }

        [LocalDisplayName("Users.DoB", "FieldDisplayName")]
        public DateTime? DoB { get; set; }

        public int? DobDay { get; set; }
        public int? DobMonth { get; set; }
        public int? DobYear { get; set; }

        public string DobString { get; set; }

        [LocalDisplayName("Users.Can_Self_Loan", "FieldDisplayName")]
        public bool SelfLoansAllowed { get; set; }

        [LocalDisplayName("Users.Ignore_AD", "FieldDisplayName")]
        public bool IgnoreAd { get; set; }

        [DataType(DataType.MultilineText)]
        [LocalDisplayName("Users.Notes", "FieldDisplayName")]
        public string Notes { get; set; }

        [LocalDisplayName("Users.Address_Line_1", "FieldDisplayName")]
        public string AddressLine1 { get; set; }

        [LocalDisplayName("Users.Address_Line_2", "FieldDisplayName")]
        public string AddressLine2 { get; set; }

        [LocalDisplayName("Users.Address_Town_City", "FieldDisplayName")]
        public string AddressTownCity { get; set; }

        [LocalDisplayName("Users.Address_County_State", "FieldDisplayName")]
        public string AddressCountyState { get; set; }

        [LocalDisplayName("Users.Address_Post_ZipCode", "FieldDisplayName")]
        public string AddressPostZipCode { get; set; }

        [LocalDisplayName("Users.Address_Country", "FieldDisplayName")]
        public string AddressCountry { get; set; }

        [LocalDisplayName("Users.Alt_Phone_Number", "FieldDisplayName")]
        public string AltPhoneNumber { get; set; }

        [LocalDisplayName("Users.Alt_Email_Address", "FieldDisplayName")]
        public string AltEmailAddress { get; set; }

        [DisplayName("Roles/Permissions")]
        public string Roles { get; set; }

        public IEnumerable<SelectListItem> RolesList { get; set; }

        public IEnumerable<SelectListItem> GenderList { get; set; }

        public LibraryUserAddViewModel()
        {
            UserTypeID = 1;
            CohortId = 1;
            ClassId = 1;
            UserTypeID = 1;
            SelfLoansAllowed = true;
            IgnoreAd = false;
            IsLive = true;
        }
    }

    public class LibraryUserEditViewModel
    {
        public string Id { get; set; }

        public int LibraryUserID { get; set; }

        [StringLength(255)]
        [LocalDisplayName("Users.Username", "FieldDisplayName")]
        public string UserName { get; set; }

        //[Required(ErrorMessage = "An email address is required")]
        [EmailAddress]
        [RegularExpression( "^[a-z0-9_\\+-]+(\\.[a-z0-9_\\+-]+)*@[a-z0-9-]+(\\.[a-z0-9]+)*\\.([a-z]{2,4})$" , ErrorMessage = "Invalid email address!")]
        public string Email { get; set; }

        [LocalDisplayName("Users.Firstnames", "FieldDisplayName")]
        public string Firstname { get; set; }

        [LocalDisplayName("Users.Lastnames", "FieldDisplayName")]
        public string Lastname { get; set; }

        [LocalDisplayName("Genders.Gender", "FieldDisplayName")]
        public int? GenderId { get; set; }

        [LocalDisplayName("Users.DoB", "FieldDisplayName")]
        public DateTime? DoB { get; set; }

        public int? DobDay { get; set; }
        public int? DobMonth { get; set; }
        public int? DobYear { get; set; }

        public string DobString { get; set; }

        [LocalDisplayName("Users.Barcode", "FieldDisplayName")]
        public string UserBarcode { get; set; }

        [LocalDisplayName("Users.Is_Live", "FieldDisplayName")]
        public bool IsLive { get; set; }

        [LocalDisplayName("Locations.Location", "FieldDisplayName")]
        public int? LocationId { get; set; }

        [LocalDisplayName("Classes.Class", "FieldDisplayName")]
        public int? ClassId { get; set; }

        [LocalDisplayName("Cohorts.Cohort", "FieldDisplayName")]
        public int? CohortId { get; set; }

        [LocalDisplayName("UserType.UserType", "FieldDisplayName")]
        public int? UserTypeID { get; set; }

        [LocalDisplayName("Users.Reading_Group", "FieldDisplayName")]
        public int? ReadingGroupID { get; set; }

        [LocalDisplayName("Departments.Department", "FieldDisplayName")]
        public int? DepartmentId { get; set; }

        [LocalDisplayName("Users.Position", "FieldDisplayName")]
        public string Position { get; set; }

        [LocalDisplayName("Users.Can_Self_Loan", "FieldDisplayName")]
        public bool SelfLoansAllowed { get; set; }

        [DisplayName("Exclude user from Active Directory updates?")]
        [LocalDisplayName("Users.Ignore_AD", "FieldDisplayName")]
        public bool IgnoreAd { get; set; }

        [DataType(DataType.MultilineText)]
        [LocalDisplayName("Users.Notes", "FieldDisplayName")]
        public string Notes { get; set; }

        [LocalDisplayName("Users.Address_Line_1", "FieldDisplayName")]
        public string AddressLine1 { get; set; }

        [LocalDisplayName("Users.Address_Line_2", "FieldDisplayName")]
        public string AddressLine2 { get; set; }

        [LocalDisplayName("Users.Address_Town_City", "FieldDisplayName")]
        public string AddressTownCity { get; set; }

        [LocalDisplayName("Users.Address_County_State", "FieldDisplayName")]
        public string AddressCountyState { get; set; }

        [LocalDisplayName("Users.Address_Post_ZipCode", "FieldDisplayName")]
        public string AddressPostZipCode { get; set; }

        [LocalDisplayName("Users.Address_Country", "FieldDisplayName")]
        public string AddressCountry { get; set; }

        [LocalDisplayName("Users.Alt_Phone_Number", "FieldDisplayName")]
        public string AltPhoneNumber { get; set; }

        [LocalDisplayName("Users.Alt_Email_Address", "FieldDisplayName")]
        public string AltEmailAddress { get; set; }

        [DisplayName("Roles/Permissions")]
        public string Roles { get; set; }

        public IEnumerable<SelectListItem> RolesList { get; set; }

        public IEnumerable<SelectListItem> GenderList { get; set; }
    }

    public class LibraryUserResetPasswordViewModel
    {
        [Display(Name = "Library User")]
        public IEnumerable<SelectListItem> LibraryUsers { get; set; }

        [Display(Name = "Library User")]
        public string Fullname { get; set; }

        [Required(ErrorMessage = "Please select a Library User.")]
        public string UserId { get; set; }

        [AllowHtml]
        public string PasswordTip { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LibraryUsersIndexViewModel
    {
        // A list of users ...
        public IEnumerable<ApplicationUser> LibraryUsers { get; set; }

        //Stuff to handle the alphabetical paging links on the index view ...
        public List<string> FirstLetters { get; set; }
        public string SelectedLetter { get; set; }

        //Show Live-only or All users?
        public bool ShowAll { get; set; }
    }

    public class LibraryUserBookmarkViewModel
    {
        [Key]
        public int BookmarkId { get; set; }

        public int TitleId { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class LibraryUserSavedSearchViewModel
    {
        [Key]
        public int SavedSearchId { get; set; }
        public string UserId { get; set; }
        public string SearchString { get; set; }
        public string SearchField { get; set; }
        public string Scope { get; set; }
        public string ClassmarksFilter { get; set; }
        public string MediaFilter { get; set; }
        public string PublisherFilter { get; set; }
        public string LanguageFilter { get; set; }
        public string KeywordFilter { get; set; }
        public string AuthorFilter { get; set; }
        public string AccountYearsFilter { get; set; }
        public string BudgetCodesFilter { get; set; }
        public string OrderCategoriesFilter { get; set; }
        public string RequestersFilter { get; set; }
        public string SuppliersFilter { get; set; }
        public string Description { get; set; }
    }
}