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
        
        [Required]
        [LocalDisplayName("CopyItems.Barcode", "FieldDisplayName")]
        public string Barcode { get; set; }

        [Required(ErrorMessage = "Please select a Borrower.")]
        public string UserID { get; set; }

        [LocalDisplayName("Borrowing.Borrowed", "FieldDisplayName")]
        public DateTime Borrowed { get; set; }

        [LocalDisplayName("Borrowing.Date_Return_Due", "FieldDisplayName")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
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
        [Required]
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
    }

    public class RenewLoanViewModel
    {
        [Required]
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

    public class ConfirmationRenewReturnViewModel
    {
        public ConfirmationRenewReturnViewModel()
        {
            ConfirmButtonText = "OK";
            ConfirmButtonClass = "btn-success";
            CancelButtonText = "Cancel";
            Glyphicon = "glyphicon-ok";
        }

        public int BorrowID { get; set; }
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
}