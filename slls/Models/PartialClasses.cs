using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Dynamic;
using System.Linq;
using System.Web;

namespace slls.Models
{
    //[MetadataType(typeof(AccountYearMetadata))]
    //public partial class AccountYear
    //{
    //    public class AccountYearMetadata
    //    {
    //        [DisplayName("Account Year")]
    //        public string AccountYear1 { get; set; }

    //        [DisplayName("Year Start")]
    //        [DataType(DataType.DateTime)]
    //        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    //        public DateTime? YearStartDate { get; set; }

    //        [DisplayName("Year End")]
    //        [DataType(DataType.DateTime)]
    //        //[DisplayFormat(NullDisplayText = "", DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    //        public DateTime? YearEndDate { get; set; }
    //    }
    //}

    //[MetadataType(typeof(ActivityTypesMetadata))]
    //public partial class ActivityType
    //{
    //    public class ActivityTypesMetadata
    //    {
    //        [DisplayName("Activity Type")]
    //        public string Activity { get; set; }
    //    }
    //}

    //[MetadataType(typeof(AuthorMetadata))]
    //public partial class Author
    //{
    //    public class AuthorMetadata
    //    {
    //        [DisplayName("Firstname(s)")]
    //        public string Firstnames { get; set; }

    //        [DisplayName("Lastname(s)")]
    //        public string Lastnames { get; set; }

    //        [DisplayName("Type")]
    //        public string AuthType { get; set; }
    //    }
    //}
    

    //[MetadataType(typeof(ClassmarkMetadata))]
    //public partial class Classmark
    //{
    //    public class ClassmarkMetadata
    //    {
    //        [DisplayName("Classmark")]
    //        public string Classmark1 { get; set; }

    //        [DisplayName("Short Code")]
    //        public string Code { get; set; }
    //    }
    //}

    //[MetadataType(typeof(SupplierMetadata))]
    //public partial class Supplier
    //{
    //    public class SupplierMetadata
    //    {
    //        [DisplayName("Supplier name")]
    //        public string SupplierName { get; set; }
            
    //    }
    //}

    //[MetadataType(typeof(CopyMetadata))]
    //public partial class Copy
    //{
    //    public class CopyMetadata
    //    {
    //        [DisplayName("Copy No.")]
    //        public int CopyNumber { get; set; }

    //        [DisplayName("On Acquisitions List?")]
    //        public bool AcquisitionsList { get; set; }

    //        [DisplayName("Print Label?")]
    //        public bool PrintLabel { get; set; }

    //        [DisplayName("Account Year")]
    //        public Nullable<int> AccountYear { get; set; }

    //        [DisplayName("Standing Order?")]
    //        public Nullable<bool> StandingOrder { get; set; }

    //        [DisplayName("Date Added to Acquisitions List")]
    //        public Nullable<System.DateTime> AddedToAcquisitions { get; set; }
    //    }
    //}

    //[MetadataType(typeof(DepartmentMetadata))]
    //public partial class Department
    //{
    //    public class DepartmentMetadata
    //    {
    //        [DisplayName("Department")]
    //        public string Department1 { get; set; }

    //        [ScaffoldColumn(false)]
    //        public Nullable<System.DateTime> InputDate { get; set; }

    //        [ScaffoldColumn(false)]
    //        public Nullable<System.DateTime> LastModified { get; set; }

    //        [ScaffoldColumn(false)]
    //        public byte[] RowVersion { get; set; }
    //    }
    //}

    //[MetadataType(typeof(FrequencyMetadata))]
    //public partial class Frequency
    //{
    //    public class FrequencyMetadata
    //    {
    //        [DisplayName("Description")]
    //        public string Frequency1 { get; set; }

    //        [DisplayName("Days Between")]
    //        [RegularExpression("([0-9]+)", ErrorMessage = "Please enter valid number, either 0 (zero) or greater")]
    //        [Required(ErrorMessage = "Please enter a value for Days Between.")]
    //        [Range(0, 9999)]
    //        public int Days { get; set; }
    //    }
    //}

    //[MetadataType(typeof(OfficeMetadata))]
    //public partial class Office
    //{
    //    public class OfficeMetadata
    //    {
    //        [DisplayName("Office/Branch")]
    //        public string Office1 { get; set; }
    //    }
    //}


    //[MetadataType(typeof(BudgetCodeMetadata))]
    //public partial class BudgetCode
    //{
    //    public class BudgetCodeMetadata
    //    {
    //        [DisplayName("Budget Code")]
    //        public string BudgetCode1 { get; set; }

    //        [DisplayFormat(DataFormatString = "{0:C}")]
    //        [DisplayName("Subs Allocation")]
    //        public decimal AllocationSubs { get; set; }

    //        [DisplayFormat(DataFormatString = "{0:C}")]
    //        [DisplayName("One-offs Allocation")]
    //        public decimal AllocationOneOffs { get; set; }
    //    }
    //}

    //[MetadataType(typeof(KeywordsMetadata))]
    //public partial class Keyword
    //{
    //    public class KeywordsMetadata
    //    {
    //        [DisplayName("Keyword Term")]
    //        public string Keyword1 { get; set; }
            
    //        [DisplayName("Parent Keyword")]
    //        public string ParentKeywordID { get; set; }
    //    }
    //}

    //[MetadataType(typeof(BorrowingMetadata))]
    //public partial class Borrowing
    //{
    //    public class BorrowingMetadata
    //    {
    //        [Key]
    //        public int BorrowID { get; set; }
    //    }
    //}

    //[MetadataType(typeof(CatalogueDefaultMetadata))]
    //public partial class CatalogueDefault
    //{
    //    public class CatalogueDefaultMetadata
    //    {
    //        [Key]
    //        public int RowID { get; set; }
    //    }
    //}

    //[MetadataType(typeof(CatalogueSearchMetadata))]
    //public partial class CatalogueSearch
    //{
    //    public class CatalogueSearchMetadata
    //    {
    //        [Key]
    //        public int SearchID { get; set; }
    //    }
    //}

    //[MetadataType(typeof(CirculationMessageMetadata))]
    //public partial class CirculationMessage
    //{
    //    public class CirculationMessageMetadata
    //    {
    //        [Key]
    //        public int CirculationMsgID { get; set; }
    //    }
    //}

    //[MetadataType(typeof(CommMethodTypeMetadata))]
    //public partial class CommMethodType
    //{
    //    public class CommMethodTypeMetadata
    //    {
    //        [Key]
    //        public int MethodID { get; set; }
    //    }
    //}

    //[MetadataType(typeof(SupplierAddressMetadata))]
    //public partial class SupplierAddress
    //{
    //    public class SupplierAddressMetadata
    //    {
    //        [Key]
    //        public int AddressID { get; set; }
    //    }
    //}

    //[MetadataType(typeof(SupplierPeopleMetadata))]
    //public partial class SupplierPeople
    //{
    //    public class SupplierPeopleMetadata
    //    {
    //        [Key]
    //        public int ContactID { get; set; }
    //    }
    //}

    //[MetadataType(typeof(SupplierPeopleCommMetadata))]
    //public partial class SupplierPeopleComm
    //{
    //    public class SupplierPeopleCommMetadata
    //    {
    //        [Key]
    //        public int ContactID { get; set; }
    //    }
    //}

    //[MetadataType(typeof(EmailLogMetadata))]
    //public partial class EmailLog
    //{
    //    public class EmailLogMetadata
    //    {
    //        [Key]
    //        public int EmailID { get; set; }
    //    }
    //}

    //[MetadataType(typeof(EmailLogAttachmentMetadata))]
    //public partial class EmailLogAttachment
    //{
    //    public class EmailLogAttachmentMetadata
    //    {
    //        [Key]
    //        public int EmailID { get; set; }
    //    }
    //}

    //[MetadataType(typeof(ErrorLogMetadata))]
    //public partial class ErrorLog
    //{
    //    public class ErrorLogMetadata
    //    {
    //        [Key]
    //        public int ErrLogID { get; set; }
    //    }
    //}

    //[MetadataType(typeof(HelpTextMetadata))]
    //public partial class HelpText
    //{
    //    public class HelpTextMetadata
    //    {
    //        [Key]
    //        public int helpid { get; set; }
    //    }
    //}

    //[MetadataType(typeof(LanguageMetadata))]
    //public partial class Language
    //{
    //    public class LanguageMetadata
    //    {
    //        [Key]
    //        public int LanguageID { get; set; }
    //        [DisplayName("Language")]
    //        public string Language1 { get; set; }
    //    }
    //}

    //[MetadataType(typeof(LibraryUserMetadata))]
    //public partial class LibraryUser
    //{
    //    public class LibraryUserMetadata
    //    {
    //        [Key]
    //        public int UserID { get; set; }

    //        [DisplayName("Last name")]
    //        public string Lastnames { get; set; }

    //        [DisplayName("First name(s)")]
    //        public string Firstnames { get; set; }

    //        [DisplayName("Department")]
    //        public Nullable<int> DepartmentID { get; set; }

    //        [DisplayName("Branch/Office")]
    //        public Nullable<int> OfficeID { get; set; }

    //        [DisplayName("Can self loan?")]
    //        public Nullable<bool> CanSelfLoan { get; set; }

    //        [DisplayName("User record found in Active Directory?")]
    //        public bool FoundInAD { get; set; }

    //        [DisplayName("Exclude user from Active Directory updates?")]
    //        public bool IgnoreAD { get; set; }

    //        [DisplayName("Notes")]
    //        public string UserNotes { get; set; }

    //        [Required(ErrorMessage = "A Password is required")]
    //        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    //        [DataType(DataType.Password)]
    //        //[Display(Name = "Password")]
    //        public string Password { get; set; }

    //        [Required(ErrorMessage = "Please confirm the Password")]
    //        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    //        [DataType(DataType.Password)]
    //        //[Display(Name = "Confirm Password")]
    //        [Compare("Password")]
    //        public string ConfirmPassword { get; set; }
    //    }
    //}

    //[MetadataType(typeof(LibraryUserEmailAddressMetadata))]
    //public partial class LibraryUserEmailAddress
    //{
    //    public class LibraryUserEmailAddressMetadata
    //    {
    //        [Key]
    //        public int EmailID { get; set; }
    //    }
    //}

    //[MetadataType(typeof(LoanReturnMetadata))]
    //public partial class LoanReturn
    //{
    //    public class LoanReturnMetadata
    //    {
    //        [Key]
    //        public int ReturnID { get; set; }
    //    }
    //}

    //[MetadataType(typeof(MediaTypeMetadata))]
    //public partial class MediaType
    //{
    //    public class MediaTypeMetadata
    //    {
    //        [Key]
    //        public int MediaID { get; set; }
    //    }
    //}

    //[MetadataType(typeof(OrderDetailMetadata))]
    //public partial class OrderDetail
    //{
    //    public class OrderDetailMetadata
    //    {
    //        [Key]
    //        public int OrderID { get; set; }
    //    }
    //}

    //[MetadataType(typeof(PartsReceivedMetadata))]
    //public partial class PartsReceived
    //{
    //    public class PartsReceivedMetadata
    //    {
    //        [Key]
    //        public int PartID { get; set; }
    //    }
    //}

    //[MetadataType(typeof(ReportTypeMetadata))]
    //public partial class ReportType
    //{
    //    public class ReportTypeMetadata
    //    {
    //        [Key]
    //        public int ReportID { get; set; }
    //    }
    //}

    //[MetadataType(typeof(SearchOperatorTypeMetadata))]
    //public partial class SearchOperatorType
    //{
    //    public class SearchOperatorTypeMetadata
    //    {
    //        [Key]
    //        public int RecID { get; set; }
    //    }
    //}

    //[MetadataType(typeof(StatusTypeMetadata))]
    //public partial class StatusType
    //{
    //    public class StatusTypeMetadata
    //    {
    //        [Key]
    //        public int StatusID { get; set; }
    //    }
    //}

    //[MetadataType(typeof(TitleMetadata))]
    //public partial class Title
    //{
    //    public class TitleMetadata
    //    {
    //        [Key]
    //        public int TitleID { get; set; }

    //        [DisplayName("Title")]
    //        public string Title1 { get; set; }

    //        [DisplayName("Place of Publication")]
    //        public string PlaceofPublication { get; set; }

    //        [DisplayName("Author(s)")]
    //        public virtual ICollection<TitleAuthor> TitleAuthors { get; set; }

    //        [DisplayName("Editor(s)")]
    //        public virtual ICollection<TitleEditor> TitleEditors { get; set; }

    //        [DisplayName("Published Year")]
    //        public string Year { get; set; }

    //        [DisplayName("ISBN-10")]
    //        public string ISBN10 { get; set; }

    //        [DisplayName("ISBN-13")]
    //        public string ISBN13 { get; set; }

    //        [DisplayName("Non-Filing characters")]
    //        public Nullable<int> NonFilingChars { get; set; }

    //        [DisplayName("Date Catalogued")]
    //        public Nullable<System.DateTime> DateCatalogued { get; set; }

    //        [DisplayName("Publisher")]
    //        public virtual Supplier Supplier { get; set; }

    //        [DisplayName("Keyword(s)")]
    //        public virtual ICollection<SubjectIndex> SubjectIndexes { get; set; }

    //        [DisplayName("Media Type")]
    //        public int MediaID { get; set; }

    //        [DisplayName("Classmark")]
    //        public int ClassmarkID { get; set; }

    //        [DisplayName("Publisher")]
    //        public int SupplierID { get; set; }

    //        [DisplayName("Frequency")]
    //        public int FrequencyID { get; set; }

    //        [DisplayName("Language")]
    //        public int LanguageID { get; set; }
    //    }
    //}

    //[MetadataType(typeof(TitleAdditionalFieldDataMetadata))]
    //public partial class TitleAdditionalFieldData
    //{
    //    public class TitleAdditionalFieldDataMetadata
    //    {
    //        [Key, Column(Order=1)]
    //        public int FieldID { get; set; }
    //        [Key, Column(Order=2)]
    //        public int TitleID { get; set; }
    //    }
    //}

    //[MetadataType(typeof(TitleAdditionalFieldDefMetadata))]
    //public partial class TitleAdditionalFieldDef
    //{
    //    public class TitleAdditionalFieldDefMetadata
    //    {
    //        [Key]
    //        public int FieldID { get; set; }
    //    }
    //}

    //[MetadataType(typeof(TitleLabelMetadata))]
    //public partial class TitleLabel
    //{
    //    public class TitleLabelMetadata
    //    {
    //        [Key]
    //        public int VolumeID { get; set; }
    //    }
    //}

    //[MetadataType(typeof(UsersADMaintenanceLogMetadata))]
    //public partial class UsersADMaintenanceLog
    //{
    //    public class UsersADMaintenanceLogMetadata
    //    {
    //        [Key]
    //        public int RecID { get; set; }
    //    }
    //}

    //[MetadataType(typeof(VolumeMetadata))]
    //public partial class Volume
    //{
    //    public class VolumeMetadata
    //    {
    //        [Key]
    //        public int VolumeID { get; set; }
    //    }
    //}

    //[MetadataType(typeof(VolumesLabelMetadata))]
    //public partial class VolumesLabel
    //{
    //    public class VolumesLabelMetadata
    //    {
    //        [Key]
    //        public int VolumeID { get; set; }
    //    }
    //}

    //[MetadataType(typeof(XXXMetadata))]
    //public partial class XXX
    //{
    //    public class XXXXXXMetadata
    //    {
    //        [DisplayName("XXX Term")]
    //        public string Keyword { get; set; }
    //    }
    //}

   
}