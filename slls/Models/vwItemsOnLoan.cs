using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace slls.Models
{
    [Table("dbo.vwItemsOnLoan")]
    public partial class vwItemsOnLoan
    {
        [Key]
        public int BorrowID { get; set; }

        public int VolumeID { get; set; }
        
        public string BorrowerUser_ID { get; set; }
        
        public Nullable<DateTime> Borrowed { get; set; }

        [DisplayName("Return Due")]
        public Nullable<DateTime> ReturnDue { get; set; }

        public string EmailAddress { get; set; }

        [DisplayName("Borrowed By")]
        public string Fullname { get; set; }
    }
}