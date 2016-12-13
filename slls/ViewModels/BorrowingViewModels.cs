using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using slls.Localization;
using slls.Models;

namespace slls.ViewModels
{
    public class BorrowingIndexViewModel
    {
        public IEnumerable<Borrowing> Loans { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public List<string> Years { get; set; }
        public List<string> Months { get; set; }
    }
    
    public class NewLoanViewModel
    {
        [LocalDisplayName("CopyItems.Barcode", "FieldDisplayName")]
        public int? VolumeId { get; set; }

        [Required(ErrorMessage = "Please tell us which item you wish to borrow!")]
        [LocalDisplayName("CopyItems.Barcode", "FieldDisplayName")]
        public string Barcode { get; set; }

        [Required(ErrorMessage = "Please select a Borrower.")]
        public string UserID { get; set; }

        public string Borrower { get; set; }

        [LocalDisplayName("Borrowing.Borrowed", "FieldDisplayName")]
        [Column(TypeName = "smalldatetime")]
        public DateTime Borrowed { get; set; }

        [LocalDisplayName("Borrowing.Date_Return_Due", "FieldDisplayName")]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Column(TypeName = "smalldatetime")]
        [Required(ErrorMessage = "Please enter a return date.")]
        public DateTime ReturnDue { get; set; }

        public IEnumerable<SelectListItem> Users { get; set; }
        public IEnumerable<SelectListItem> Copies { get; set; }
        public IEnumerable<SelectListItem> Titles { get; set; }
        public IEnumerable<SelectListItem> Volumes { get; set; }
        public List<Menu> SeeAlso { get; set; }
    }

    public class ReturnLoanViewModel
    {
        [Required(ErrorMessage = "Please tell us which item you wish to return!")]
        [LocalDisplayName("CopyItems.Barcode", "FieldDisplayName")]
        public string Barcode { get; set; }

        public string UserID { get; set; }

        [LocalDisplayName("Borrowing.Borrowed_By", "FieldDisplayName")]
        public string BorrowedBy { get; set; }

        [LocalDisplayName("Borrowing.Borrowed", "FieldDisplayName")]
        public DateTime Borrowed { get; set; }

        [LocalDisplayName("Borrowing.Date_Return_Due", "FieldDisplayName")]
        public DateTime ReturnDue { get; set; }

        public IEnumerable<SelectListItem> Copies { get; set; }
        public IEnumerable<SelectListItem> Titles { get; set; }
        public IEnumerable<SelectListItem> Volumes { get; set; }
        public List<Menu> SeeAlso { get; set; }
    }

    public class RenewLoanViewModel
    {
        [Required(ErrorMessage = "Please tell us which item you wish to renew!")]
        [LocalDisplayName("CopyItems.Barcode", "FieldDisplayName")]
        public string Barcode { get; set; }

        public string UserID { get; set; }

        [LocalDisplayName("Borrowing.Borrowed_By", "FieldDisplayName")]
        public string BorrowedBy { get; set; }

        [LocalDisplayName("Borrowing.Borrowed", "FieldDisplayName")]
        public DateTime Borrowed { get; set; }
        
        [LocalDisplayName("Borrowing.Date_Return_Due", "FieldDisplayName")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Please enter a return date.")]
        public DateTime ReturnDue { get; set; }

        public IEnumerable<SelectListItem> Copies { get; set; }
        public IEnumerable<SelectListItem> Titles { get; set; }
        public IEnumerable<SelectListItem> Volumes { get; set; }
        public List<Menu> SeeAlso { get; set; }
    }

    public class EditLoanViewModel
    {
        public int BorrowID { get; set; }
        public string Title { get; set; }
        public int CopyNumber { get; set; }
        public string Barcode { get; set; }
        public int VolumeId { get; set; }
        public string BorrowerUserId { get; set; }
        public string UserID { get; set; }
        public bool Renewal { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? Borrowed { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? ReturnDue { get; set; }
        
    }

    public class ConfirmNewLoanRenewReturnViewModel
    {
        public ConfirmNewLoanRenewReturnViewModel()
        {
            ConfirmButtonText = "OK";
            ConfirmButtonClass = "btn-success";
            CancelButtonText = "Cancel";
            Glyphicon = "glyphicon-ok";
        }

        public int VolumeID { get; set; }
        public int BorrowID { get; set; }
        public ApplicationUser BorrowerUser { get; set; }
        public string BorrowerId { get; set; }
        public string PostConfirmController { get; set; }
        public string PostConfirmAction { get; set; }
        public string HeaderText { get; set; }
        [AllowHtml]
        public string DetailsText { get; set; }
        public string ConfirmButtonText { get; set; }
        public string CancelButtonText { get; set; }
        [AllowHtml]
        public string ConfirmationText { get; set; }
        public string ConfirmButtonClass { get; set; }
        public string Glyphicon { get; set; }
        public string Title { get; set; }
        public string Borrower { get; set; }
        public string Borrowed { get; set; }
        public string ReturnDue { get; set; }
    }

    public class LoansReportsViewModel
    {
        public LoansReportsViewModel()
        {
            NoDataTitle = "No Data!";
            NoDataMsg = "There is no data available for this report.";
        }

        public IEnumerable<Borrowing> Loans { get; set; }
        public IEnumerable<Title> Titles { get; set; }
        public IEnumerable<Copy> Copies { get; set; }
        public IEnumerable<Volume> Volumes { get; set; }
        public IEnumerable<ApplicationUser> Borrowers { get; set; }
        public ApplicationUser Borrower { get; set; }
        public string BorrowerName { get; set; }
        public bool HasData { get; set; }
        public string NoDataTitle { get; set; }
        public string NoDataMsg { get; set; }
        public string NoDataOk { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TitleId { get; set; }
        public int CopyId { get; set; }
        public string Barcode { get; set; }
        public string Title { get; set; }
        
    }

    public class LoansSelectorViewModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool DatesProvided { get; set; }
        public string DateRangeType { get; set; }   
        public string Barcode { get; set; }
        public string Title { get; set; }
        public int Weeks { get; set; }
        public int TitleId { get; set; }
        public int CopyId { get; set; }
        public int VolumeId { get; set; }

        public IEnumerable<SelectListItem> SelectCopies { get; set; }
        public IEnumerable<SelectListItem> SelectTitles { get; set; }
        public IEnumerable<SelectListItem> SelectVolumes { get; set; }
    }
}