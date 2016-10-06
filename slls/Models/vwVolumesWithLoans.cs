using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using slls.Localization;

namespace slls.Models
{
    [Table("dbo.vwVolumesWithLoans")]
    public class vwVolumesWithLoans
    {
        [Key]
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
}