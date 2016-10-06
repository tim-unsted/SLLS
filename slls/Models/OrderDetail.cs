using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using slls.Localization;

namespace slls.Models
{
    [Table("Orders")]
    public partial class OrderDetail
    {
        public OrderDetail()
        {
        }
        
        [Key]
        public int OrderID { get; set; }

        [StringLength(50)]
        [LocalDisplayName("Orders.Order_Number", "FieldDisplayName")]
        public string OrderNo { get; set; }
        
        [Column(TypeName = "smalldatetime")]
        [LocalDisplayName("Orders.Date_Ordered", "FieldDisplayName")]
        public DateTime? OrderDate { get; set; }
        
        [LocalDisplayName("Orders.Supplier", "FieldDisplayName")]
        public int? SupplierID { get; set; }

        [LocalDisplayName("Orders.Title", "FieldDisplayName")]
        public int TitleID { get; set; }

        [StringLength(255)]
        [LocalDisplayName("Orders.Requirements", "FieldDisplayName")]
        public string Item { get; set; }

        [LocalDisplayName("Orders.Number_Copies_Ordered", "FieldDisplayName")]
        public int? NumCopies { get; set; }

        [Column(TypeName = "money")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:c}")]
        [LocalDisplayName("Orders.Price", "FieldDisplayName")]
        public decimal? Price { get; set; }

        [Column(TypeName = "money")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:c}")]
        [LocalDisplayName("Orders.VAT", "FieldDisplayName")]
        public decimal? VAT { get; set; }
        
        [LocalDisplayName("Orders.Is_On_Approval", "FieldDisplayName")]
        public bool OnApproval { get; set; }

        [Column(TypeName = "smalldatetime")]
        [LocalDisplayName("Orders.Date_Expected", "FieldDisplayName")]
        public DateTime? Expected { get; set; }

        [LocalDisplayName("Orders.Account_Year", "FieldDisplayName")]
        public int? AccountYearID { get; set; }

        [LocalDisplayName("Orders.Category", "FieldDisplayName")]
        public int? OrderCategoryID { get; set; }

        [LocalDisplayName("Orders.Budget_Code", "FieldDisplayName")]
        public int? BudgetCodeID { get; set; }

        [Column(TypeName = "smalldatetime")]
        [LocalDisplayName("Orders.Date_Cancelled", "FieldDisplayName")]
        public DateTime? Cancelled { get; set; }

        [Column(TypeName = "smalldatetime")]
        [LocalDisplayName("Orders.Date_Chased", "FieldDisplayName")]
        public DateTime? Chased { get; set; }

        [StringLength(255)]
        [LocalDisplayName("Orders.Report", "FieldDisplayName")]
        public string Report { get; set; }

        [Column(TypeName = "smalldatetime")]
        [LocalDisplayName("Orders.Date_Received", "FieldDisplayName")]
        public DateTime? ReceivedDate { get; set; }

        [LocalDisplayName("Orders.Accepted", "FieldDisplayName")]
        public bool Accepted { get; set; }

        [Column(TypeName = "smalldatetime")]
        [LocalDisplayName("Orders.Date_Returned", "FieldDisplayName")]
        public DateTime? ReturnedDate { get; set; }

        [StringLength(255)]
        [LocalDisplayName("Orders.Invoice_Reference", "FieldDisplayName")]
        public string InvoiceRef { get; set; }

        [Column(TypeName = "smalldatetime")]
        [LocalDisplayName("Orders.Date_Invoice_Passed", "FieldDisplayName")]
        public DateTime? Passed { get; set; }

        [LocalDisplayName("Orders.Month_Sub_Due", "FieldDisplayName")]
        public int? MonthSubDue { get; set; }

        [Column(TypeName = "smalldatetime")]
        [LocalDisplayName("Orders.Date_On_Invoice", "FieldDisplayName")]
        public DateTime? InvoiceDate { get; set; }

        [StringLength(1000)]
        [LocalDisplayName("Orders.Link_URL", "FieldDisplayName")]
        public string Link { get; set; }

        [LocalDisplayName("Orders.Notes", "FieldDisplayName")]
        public string Notes { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? LastModified { get; set; }

        [DisplayName("Deleted?")]
        public bool Deleted { get; set; }

        [Column(TypeName = "timestamp")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [MaxLength(8)]
        public byte[] RowVersion { get; set; }

        [NotMapped]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:c}")]
        [LocalDisplayName("Orders.PriceGross", "FieldDisplayName")]
        public decimal PriceGross
        {
            get
            {
                return Price ?? 0 + VAT ?? 0;
            }
        }

        [NotMapped]
        public string SelectOrder
        {
            get
            {
               // return string.Format("{0}:   ({1}:  {2})", Title.Title1, Supplier.SupplierName, OrderDate);
                return Title.Title1;
            }
        }

        [NotMapped]
        public string OrderDateSortable
        {
            get { return string.Format("{0:yyyy-MM-dd}", OrderDate); }
        }

        [NotMapped]
        public string ExpectedDateSortable
        {
            get { return string.Format("{0:yyyy-MM-dd}", Expected); }
        }

        [NotMapped]
        public string ReceivedDateSortable
        {
            get { return string.Format("{0:yyyy-MM-dd}", ReceivedDate); }
        }

        [NotMapped]
        public string InvoiceDateSortable
        {
            get { return string.Format("{0:yyyy-MM-dd}", InvoiceDate); }
        }

        [NotMapped]
        public string PassedDateSortable
        {
            get { return string.Format("{0:yyyy-MM-dd}", Passed); }
        }

        [NotMapped]
        public string ReturnedDateSortable
        {
            get { return string.Format("{0:yyyy-MM-dd}", ReturnedDate); }
        }

        //[ForeignKey("AuthoriserUser_Id")]
        [InverseProperty("AuthorisedOrders")]
        public virtual ApplicationUser AuthoriserUser { get; set; }

        //[ForeignKey("RequesterUser_Id")]
        [InverseProperty("RequestedOrders")]
        public virtual ApplicationUser RequesterUser { get; set; }

        public virtual AccountYear AccountYear { get; set; }
        public virtual BudgetCode BudgetCode { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual OrderCategory OrderCategory { get; set; }
        public virtual Title Title { get; set; }
    }
}