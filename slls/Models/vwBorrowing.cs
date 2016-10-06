using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace slls.Models
{
    [Table("dbo.vwBorrowing")]
    public partial class vwBorrowing
    {
        [Key]
        public int BorrowID { get; set; }

        public int VolumeID { get; set; }

        public Nullable<int> CopyID { get; set; }
        
        public string BorrowerUser_ID { get; set; }

        public string Barcode { get; set; }

        [DisplayName("Label Text")]
        public string LabelText { get; set; }

        public Nullable<System.DateTime> Borrowed { get; set; }

        [DisplayName("Return Due")]
        public Nullable<System.DateTime> ReturnDue { get; set; }

        public Nullable<System.DateTime> Returned { get; set; }

        [DisplayName("Renewal?")]
        public Boolean Renewal { get; set; }

        void Foo(){}

        public string EmailAddress { get; set; }

        [DisplayName("Borrowed By")]
        public string Fullname { get; set; }
    }
}