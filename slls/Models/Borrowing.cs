using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using slls.Localization;

namespace slls.Models
{
    [Table("Borrowing")]
    public class Borrowing
    {
        public Borrowing()
        {
            //BorrowerUser = new ApplicationUser();
        }
        
        [Key]
        public int BorrowID { get; set; }

        [LocalDisplayName("CopyItems.Barcode", "FieldDisplayName")]
        public int VolumeID { get; set; }

        //public string BorrowerUser_Id { get; set; }

        [Column(TypeName = "smalldatetime")]
        [LocalDisplayName("Borrowing.Borrowed", "FieldDisplayName")]
        public DateTime? Borrowed { get; set; }

        [Column(TypeName = "smalldatetime")]
        [LocalDisplayName("Borrowing.Date_Return_Due", "FieldDisplayName")]
        public DateTime? ReturnDue { get; set; }

        [Column(TypeName = "smalldatetime")]
        [LocalDisplayName("Borrowing.Date_Returned", "FieldDisplayName")]
        public DateTime? Returned { get; set; }

        [LocalDisplayName("Borrowing.Is_Renewal", "FieldDisplayName")]
        public bool Renewal { get; set; }

        [DisplayName("Deleted?")]
        public bool Deleted { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? InputDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? LastModified { get; set; }

        [Column(TypeName = "timestamp")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [MaxLength(8)]
        public byte[] RowVersion { get; set; }

        [NotMapped]
        public string BorrowedDateSortable
        {
            get { return string.Format("{0:yyyy-MM-dd}", Borrowed); }
        }

        [NotMapped]
        public string ReturnDueDateSortable
        {
            get { return string.Format("{0:yyyy-MM-dd}", ReturnDue); }
        }

        [NotMapped]
        public string ReturnedDateSortable
        {
            get { return string.Format("{0:yyyy-MM-dd}", Returned); }
        }
        
        public virtual Volume Volume { get; set; }
        public virtual ApplicationUser BorrowerUser { get; set; }
    }
}