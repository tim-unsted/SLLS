using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using slls.Localization;
using slls.Models;

namespace slls.ViewModels
{
    public class VolumesIndexViewModel
    {
        public IEnumerable<Volume> Volumes { get; set; }
        public int TitleId { get; set; }
        public bool UsePreprintedBarcodes { get; set; }
    }
    
    public class VolumesAddViewModel
    {
        public VolumesAddViewModel()
        {
            ReturnAction = "Edit";
            ReturnController = "Copies";
        }
        
        [Required(ErrorMessage = "Please select a Title")]
        public int TitleId { get; set; }
        
        [Required(ErrorMessage = "Please select a Copy")]
        public int CopyId { get; set; }
        
        [StringLength(255)]
        [LocalDisplayName("CopyItems.Label_Text", "FieldDisplayName")]
        public string LabelText { get; set; }

        [StringLength(50)]
        [LocalDisplayName("CopyItems.Barcode", "FieldDisplayName")]
        public string Barcode { get; set; }

        [LocalDisplayName("CopyItems.Print_Label", "FieldDisplayName")]
        public bool PrintLabel { get; set; }

        [LocalDisplayName("CopyItems.Is_Ref_Only", "FieldDisplayName")]
        public bool RefOnly { get; set; }

        [LocalDisplayName("Titles.Title", "FieldDisplayName")]
        [StringLength(450)]
        public string Title { get; set; }

        [LocalDisplayName("Copies.Copy_Number", "FieldDisplayName")]
        [StringLength(450)]
        public string CopyNumber { get; set; }

        [LocalDisplayName("Borrowing.Loan_Type", "FieldDisplayName")]
        public int LoanTypeId { get; set; }

        public int Step { get; set; }

        public string ReturnAction { get; set; }

        public string ReturnController { get; set; }

        public bool AddMore { get; set; }

        public bool UsePreprintedBarcodes { get; set; }
    }

    public class VolumesEditViewModel
    {
        public int VolumeId { get; set; }
        
        public int CopyId { get; set; }

        public int TitleId { get; set; }

        [LocalDisplayName("Copies.Copy_Number", "FieldDisplayName")]
        public int CopyNumber { get; set; }

        [StringLength(255)]
        [LocalDisplayName("CopyItems.Label_Text", "FieldDisplayName")]
        public string LabelText { get; set; }

        [LocalDisplayName("CopyItems.Print_Label", "FieldDisplayName")]
        public bool PrintLabel { get; set; }

        [LocalDisplayName("CopyItems.Is_Ref_Only", "FieldDisplayName")]
        public bool RefOnly { get; set; }

        [StringLength(50)]
        [LocalDisplayName("CopyItems.Barcode", "FieldDisplayName")]
        public string Barcode { get; set; }

        [LocalDisplayName("Titles.Title", "FieldDisplayName")]
        [StringLength(450)]
        public string Title { get; set; }

        [LocalDisplayName("Borrowing.Loan_Type", "FieldDisplayName")]
        public int LoanTypeId { get; set; }

        public bool IsBarcodeEdited { get; set; }

        public bool UsePreprintedBarcodes { get; set; }

        public bool BarcodeNeedsEditing { get; set; }
    }

    public class VolumesWithLoansViewModel
    {
        public int VolumeID { get; set; }

        public int CopyID { get; set; }

        public int CopyNumber { get; set; }

        [LocalDisplayName("CopyItems.Barcode", "FieldDisplayName")]
        public string Barcode { get; set; }

        [LocalDisplayName("CopyItems.Label_Text", "FieldDisplayName")]
        public string LabelText { get; set; }

        [LocalDisplayName("CopyItems.Is_On_Loan", "FieldDisplayName")]
        public bool OnLoan { get; set; }

        [LocalDisplayName("CopyItems.Is_Ref_Only", "FieldDisplayName")]
        public bool RefOnly { get; set; }

        [LocalDisplayName("LoanTypes.Loan_Type", "FieldDisplayName")]
        public string LoanType { get; set; }

        [LocalDisplayName("CopyItems.Print_Label", "FieldDisplayName")]
        public bool PrintLabel { get; set; }

        public int? BorrowID { get; set; }

        public string BorrowerUser_ID { get; set; }

        [LocalDisplayName("Borrowing.Borrowed", "FieldDisplayName")]
        public DateTime? Borrowed { get; set; }

        [LocalDisplayName("Borrowing.Date_Return_Due", "FieldDisplayName")]
        public DateTime? ReturnDue { get; set; }

        [LocalDisplayName("Users.Email_Address", "FieldDisplayName")]
        public string EmailAddress { get; set; }

        [LocalDisplayName("Borrowing.Borrowed_By", "FieldDisplayName")]
        public string Fullname { get; set; }
    }

    public class VolumesWithLoansIndexViewModel
    {
        public List<VolumesWithLoansViewModel> VolumesWithLoans { get; set; }
    }
}
