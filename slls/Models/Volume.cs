using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using slls.Localization;

namespace slls.Models
{
    [Table("Volumes")]
    public class Volume
    {
        public Volume()
        {
            //Borrowings = new HashSet<Borrowing>();
        }

        [Key]
        public int VolumeID { get; set; }
        public int CopyID { get; set; }

        [StringLength(255)]
        [LocalDisplayName("CopyItems.Label_Text", "FieldDisplayName")]
        public string LabelText { get; set; }

        [LocalDisplayName("CopyItems.Is_On_Loan", "FieldDisplayName")]
        public bool OnLoan { get; set; }

        [LocalDisplayName("CopyItems.Print_Label", "FieldDisplayName")]
        public bool PrintLabel { get; set; }

        [LocalDisplayName("CopyItems.Is_Ref_Only", "FieldDisplayName")]
        public bool RefOnly { get; set; }

        [LocalDisplayName("CopyItems.Loan_Type", "FieldDisplayName")]
        public int? LoanTypeID { get; set; }

        [StringLength(50)]
        [LocalDisplayName("CopyItems.Barcode", "FieldDisplayName")]
        public string Barcode { get; set; }

        [LocalDisplayName("Locations.Location", "FieldDisplayName")]
        public string BarcodeWithLabelText
        {
            get { return LabelText == null ? Barcode : Barcode + " - " + LabelText; }
        }

        public bool IsBarcodeEdited { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? InputDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? LastModified { get; set; }

        [DisplayName("Deleted?")]
        public bool Deleted { get; set; }
        
        [Column(TypeName = "timestamp")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [MaxLength(8)]
        public byte[] RowVersion { get; set; }

        [LocalDisplayName("CopyItems.Loans", "FieldDisplayName")]
        public virtual ICollection<Borrowing> Borrowings { get; set; }
        public virtual Copy Copy { get; set; }
        public virtual LoanType LoanType { get; set; }

        [LocalDisplayName("CopyItems.Details", "FieldDisplayName")]
        public string VolumeDetailsByLabelText
        {
            get
            {
                var volumeDetails = string.Format("{0} {1}", string.IsNullOrEmpty(LabelText) ? "" : LabelText.Trim() + " - ", string.IsNullOrEmpty(Barcode) ? "" : Barcode.Trim());
                return volumeDetails.Trim();
            }
        }

        [LocalDisplayName("CopyItems.Details", "FieldDisplayName")]
        public string VolumeDetailsByBarcode
        {
            get
            {
                var volumeDetails = string.Format("{0} {1}", string.IsNullOrEmpty(Barcode) ? "" : Barcode.Trim(), string.IsNullOrEmpty(LabelText) ? "" : " - "  + LabelText.Trim());
                return volumeDetails.Trim();
            }
        }

    }
}