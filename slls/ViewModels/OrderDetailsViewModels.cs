using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using slls.Localization;
using slls.Models;

namespace slls.ViewModels
{
    public class OrderDetailsListViewModel
    {
        public IEnumerable<OrderDetail> Orders { get; set; }
        //public IEnumerable<Supplier> Suppliers { get; set; }
        //public IEnumerable<BudgetCode> BudgetCodes { get; set; }
        //public IEnumerable<MediaType> MediaTypes { get; set; }
        //public IEnumerable<OrderCategory> OrderCategories { get; set; }
        //public IEnumerable<Title> Titles { get; set; }
        //public IEnumerable<Copy> Copies { get; set; }
        public IEnumerable<ApplicationUser> AllUsers { get; set; }
        //public IEnumerable<ApplicationUser> Requestors { get; set; }
        //public IEnumerable<ApplicationUser> Authorisers { get; set; }
        //public IEnumerable<SelectListItem> ListAuthorisers { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }   

    }

    //public class OrderDetailsIndexViewModel
    //{
    //    [Key]
    //    public int OrderID { get; set; }

    //    [LocalDisplayName("Orders.Order_Number", "FieldDisplayName")]
    //    public string OrderNo { get; set; }

    //}
    
    public class OrderDetailsAddViewModel
    {
        public OrderDetailsAddViewModel()
        {
            CallingAction = "Create";
            DateWarningMsg = "<p><strong>Note: </strong>The dates highlighted in <span style=\"color: #3c763d;\"><strong>green</strong></span> have been auto-filled for you.  Please check to ensure that the dates are correct for your order.</p>"; ;
        }
        
        [Key]
        public int OrderID { get; set; }
        
        [Required(ErrorMessage = "An Order Number is required!")]
        [StringLength(50)]
        [LocalDisplayName("Orders.Order_Number", "FieldDisplayName")]
        public string OrderNo { get; set; }

        [Required(ErrorMessage = "An Order Date is required!")]
        [Column(TypeName = "smalldatetime")]
        [LocalDisplayName("Orders.Date_Ordered", "FieldDisplayName")]
        public DateTime? OrderDate { get; set; }

        [Required(ErrorMessage = "Please select a Supplier!")]
        [LocalDisplayName("Orders.Supplier", "FieldDisplayName")]
        public int? SupplierID { get; set; }
        
        [Required(ErrorMessage = "Please select a Title to order!")]
        [LocalDisplayName("Orders.Title", "FieldDisplayName")]
        public int? TitleID { get; set; }

        [LocalDisplayName("Orders.Budget_Code", "FieldDisplayName")]
        public int? BudgetCodeID { get; set; }

        [StringLength(450)]
        [LocalDisplayName("Titles.Title", "FieldDisplayName")]
        public string Title { get; set; }

        [StringLength(255)]
        [LocalDisplayName("Orders.Requirements", "FieldDisplayName")]
        public string Item { get; set; }

        [LocalDisplayName("Orders.Number_Copies_Ordered", "FieldDisplayName")]
        public int? NumCopies { get; set; }

        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [LocalDisplayName("Orders.Price", "FieldDisplayName")]
        public decimal? Price { get; set; }

        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [LocalDisplayName("Orders.VAT", "FieldDisplayName")]
        public decimal? VAT { get; set; }

        [LocalDisplayName("Orders.Authorised_By", "FieldDisplayName")]
        public string Authority { get; set; }

        [LocalDisplayName("Orders.Requested_By", "FieldDisplayName")]
        public string RequestedBy { get; set; }

        [Column(TypeName = "smalldatetime")]
        [LocalDisplayName("Orders.Date_Expected", "FieldDisplayName")]
        public DateTime? Expected { get; set; }
        
        [LocalDisplayName("Orders.Category", "FieldDisplayName")]
        public int? OrderCategoryID { get; set; }

        [Column(TypeName = "smalldatetime")]
        [LocalDisplayName("Orders.Date_Received", "FieldDisplayName")]
        public DateTime? ReceivedDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        [LocalDisplayName("Orders.Date_On_Invoice", "FieldDisplayName")]
        public DateTime? InvoiceDate { get; set; }

        [StringLength(255)]
        [LocalDisplayName("Orders.Invoice_Reference", "FieldDisplayName")]
        public string InvoiceRef { get; set; }

        [Column(TypeName = "smalldatetime")]
        [LocalDisplayName("Orders.Date_Invoice_Passed", "FieldDisplayName")]
        public DateTime? Passed { get; set; }

        [StringLength(1000)]
        [LocalDisplayName("Orders.Link_URL", "FieldDisplayName")]
        public string Link { get; set; }

        [LocalDisplayName("Orders.Notes", "FieldDisplayName")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        public IEnumerable<SelectListItem> Titles { get; set; }
        public IEnumerable<SelectListItem> Suppliers { get; set; }
        public IEnumerable<SelectListItem> RequestUsers { get; set; }
        public IEnumerable<SelectListItem> AuthorityUsers { get; set; }
        public IEnumerable<SelectListItem> BudgetCodes { get; set; }
        public IEnumerable<SelectListItem> OrderCategories { get; set; }
        
        public string CallingAction { get; set; }

        public string DateWarningMsg { get; set; }
    }

    public class OrderDetailsEditViewModel
    {
        public OrderDetailsEditViewModel()
        {
            InfoMsg = "";
            WarningMsg = "";
            SelectedTab = "#orderdetails";
            CallingAction = "Edit";
        }
        
        [Key]
        public int OrderID { get; set; }

        [StringLength(50)]
        [LocalDisplayName("Orders.Order_Number", "FieldDisplayName")]
        public string OrderNo { get; set; }

        [Required(ErrorMessage = "An Order Date is required!")]
        [Column(TypeName = "smalldatetime")]
        [LocalDisplayName("Orders.Date_Ordered", "FieldDisplayName")]
        public DateTime? OrderDate { get; set; }

        [Required(ErrorMessage = "Please select a Supplier!")]
        [LocalDisplayName("Orders.Supplier", "FieldDisplayName")]
        public int? SupplierID { get; set; }

        public IEnumerable<SelectListItem> Titles { get; set; }
        public IEnumerable<SelectListItem> Suppliers { get; set; }
        public IEnumerable<SelectListItem> RequestUsers { get; set; }
        public IEnumerable<SelectListItem> AuthorityUsers { get; set; }

        [Required(ErrorMessage = "Please select a Title to order!")]
        [LocalDisplayName("Orders.Title", "FieldDisplayName")]
        public int TitleID { get; set; }

        [StringLength(255)]
        [LocalDisplayName("Orders.Requirements", "FieldDisplayName")]
        public string Item { get; set; }

        [LocalDisplayName("Orders.Number_Copies_Ordered", "FieldDisplayName")]
        public int? NumCopies { get; set; }

        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [LocalDisplayName("Orders.Price", "FieldDisplayName")]
        public decimal? Price { get; set; }

        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [LocalDisplayName("Orders.VAT", "FieldDisplayName")]
        public decimal? VAT { get; set; }

        [LocalDisplayName("Orders.Authorised_By", "FieldDisplayName")]
        public string Authority { get; set; }

        [LocalDisplayName("Orders.Requested_By", "FieldDisplayName")]
        public string RequestedBy { get; set; }

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

        [DataType(DataType.MultilineText)]
        [LocalDisplayName("Orders.Notes", "FieldDisplayName")]
        public string Notes { get; set; }

        public string CallingAction { get; set; }

        public string SelectedTab { get; set; } 

        [AllowHtml]
        public string InfoMsg { get; set; }

        [AllowHtml]
        public string WarningMsg { get; set; }

    }

    public class OrderReceiptsViewModel
    {
        public OrderReceiptsViewModel()
        {
            DateWarningMsg = "";
        }
        
        [Key]
        public int OrderID { get; set; }

        [StringLength(50)]
        [LocalDisplayName("Orders.Order_Number", "FieldDisplayName")]
        public string OrderNo { get; set; }

        [LocalDisplayName("Orders.Title", "FieldDisplayName")]
        public int TitleID { get; set; }

        [LocalDisplayName("Orders.Title", "FieldDisplayName")]
        public string Title { get; set; }

        [Column(TypeName = "smalldatetime")]
        [LocalDisplayName("Orders.Date_Received", "FieldDisplayName")]
        public DateTime? ReceivedDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        [LocalDisplayName("Orders.Date_On_Invoice", "FieldDisplayName")]
        public DateTime? InvoiceDate { get; set; }

        [StringLength(255)]
        [LocalDisplayName("Orders.Invoice_Reference", "FieldDisplayName")]
        public string InvoiceRef { get; set; }

        [Column(TypeName = "smalldatetime")]
        [LocalDisplayName("Orders.Date_Invoice_Passed", "FieldDisplayName")]
        public DateTime? Passed { get; set; }

        [StringLength(1000)]
        [LocalDisplayName("Orders.Link_URL", "FieldDisplayName")]
        public string Link { get; set; }

        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [LocalDisplayName("Orders.Price", "FieldDisplayName")]
        public decimal? Price { get; set; }

        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [LocalDisplayName("Orders.VAT", "FieldDisplayName")]
        public decimal? VAT { get; set; }

        [LocalDisplayName("Orders.Account_Year", "FieldDisplayName")]
        public int? AccountYearID { get; set; }

        [LocalDisplayName("Orders.Category", "FieldDisplayName")]
        public int? OrderCategoryID { get; set; }

        [LocalDisplayName("Orders.Budget_Code", "FieldDisplayName")]
        public int? BudgetCodeID { get; set; }

        [DataType(DataType.MultilineText)]
        [LocalDisplayName("Orders.Notes", "FieldDisplayName")]
        public string Notes { get; set; }

        public string CallingController { get; set; }

        public string CallingAction { get; set; }

        [AllowHtml]
        public string DateWarningMsg { get; set; }
    }

    public class OrderDetailsDeleteViewModel
    {
        public OrderDetailsDeleteViewModel()
        {
            CallingController = "";
        }

        [Key]
        public int OrderID { get; set; }

        [LocalDisplayName("Orders.Order_Number", "FieldDisplayName")]
        public string OrderNo { get; set; }

        public int TitleID { get; set; }

        [LocalDisplayName("Titles.Title", "FieldDisplayName")]
        public string Title { get; set; }

        [LocalDisplayName("Orders.Supplier", "FieldDisplayName")]
        public string Supplier { get; set; }

        [LocalDisplayName("Orders.Date_Ordered", "FieldDisplayName")]
        public DateTime? OrderDate { get; set; }

        [LocalDisplayName("Orders.Date_Expected", "FieldDisplayName")]
        public DateTime? Expected { get; set; }

        [LocalDisplayName("Orders.Date_Received", "FieldDisplayName")]
        public DateTime? ReceivedDate { get; set; }

        public string CallingController { get; set; }
        
    }

    public class SelectOrderViewmodel
    {
        public SelectOrderViewmodel()
        {
            Tab = "#orderdetails";
        }
        
        public string Title { get; set; }
        public string Message { get; set; }
        public string BtnText { get; set; }

        [AllowHtml]
        public string HelpText { get; set; }

        public string ReturnAction { get; set; }
        public IEnumerable<SelectListItem> Orders { get; set; }

        [Range(1, Int32.MaxValue, ErrorMessage = "Please select an Order!")]
        [Required(ErrorMessage = "Please select an Order!")]
        public int OrderID { get; set; }

        public string Tab { get; set; } 
    }
    
}