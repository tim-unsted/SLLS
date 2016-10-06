using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using slls.Localization;
using slls.Models;

namespace slls.ViewModels
{
    public class ExportDataViewModel
    {
        public ExportDataViewModel()
        {
            ConfirmButtonText = "Export Catalogue Data";
            HeaderText = "Export Data";
            ExportWhat = "catalogue";
        }

        public string HeaderText { get; set; }
        public string DetailsText { get; set; }
        public string ConfirmButtonText { get; set; }
        public string  ExportWhat { get; set; }

        [DisplayName("Level of data")]
        public string ExportType { get; set; }

        [DisplayName("Format")]
        public string ExportFormat { get; set; }

        [LocalDisplayName("Classmarks.Classmark", "FieldDisplayName")]
        public int ClassmarkId { get; set; }

        [LocalDisplayName("MediaTypes.Media_Type", "FieldDisplayName")]
        public int MediaId { get; set; }

        [LocalDisplayName("StatusTypes.Status_Type", "FieldDisplayName")]
        public int StatusId { get; set; }

        [LocalDisplayName("Locations.Location", "FieldDisplayName")]
        public int LocationId { get; set; }

        [LocalDisplayName("Languages.Language", "FieldDisplayName")]
        public int LanguageId { get; set; }

        [LocalDisplayName("Publishers.Publisher", "FieldDisplayName")]
        public int PublisherId { get; set; }

        [LocalDisplayName("AccountYears.Account_Year", "FieldDisplayName")]
        public int AccountYearId { get; set; }

        [LocalDisplayName("Suppliers.Supplier", "FieldDisplayName")]
        public int SupplierId { get; set; }

        [LocalDisplayName("Orders.Requested_By", "FieldDisplayName")]
        public string RequesterId { get; set; }

        [LocalDisplayName("Orders.Authorised_By", "FieldDisplayName")]
        public string AuthoriserId { get; set; }

        [LocalDisplayName("BudgetCode.Budget_Code", "FieldDisplayName")]
        public int BudgetCodeId { get; set; }

        [LocalDisplayName("OrderCategories.Order_Category", "FieldDisplayName")]
        public int OrderCategoryId { get; set; }
        
        public List<MediaType> MediaTypes { get; set; }
        public List<Classmark> Classmarks { get; set; }
        public List<Publisher> Publishers { get; set; }
        public List<Location> Locations { get; set; }
        public List<Language> Languages { get; set; }
        public List<StatusType> StatusTypes { get; set; }

        public List<AccountYear> AccountYears { get; set; }
        public List<BudgetCode> BudgetCodes { get; set; }
        public List<ApplicationUser> Requesters { get; set; }
        public List<ApplicationUser> Authorisers { get; set; }
        public List<OrderCategory> OrderCategories { get; set; }
        public List<Supplier> Suppliers { get; set; }

        public bool IncludeSubjects { get; set; }
        public bool IncludeTitleTexts { get; set; }
        public bool IncludeLinks { get; set; }
    }

    public class ExportJustTitlesViewModel
    {
        [DisplayName("Title ID")]
        public int TitleId { get; set; }

        [LocalDisplayName("Titles.Title", "FieldDisplayName")]
        public string Title { get; set; }

        [LocalDisplayName("Titles.Authors", "FieldDisplayName")]
        public string Author { get; set; }

        [LocalDisplayName("Titles.Edition", "FieldDisplayName")]
        public string Edition { get; set; }

        [LocalDisplayName("Titles.ISBN_ISSN", "FieldDisplayName")]
        public string ISBN { get; set; }

        [LocalDisplayName("Titles.Series", "FieldDisplayName")]
        public string Series { get; set; }

        [LocalDisplayName("Titles.Publisher", "FieldDisplayName")]
        public string Publisher { get; set; }

        [LocalDisplayName("Titles.Published_Year", "FieldDisplayName")]
        public string Year { get; set; }

        [LocalDisplayName("Titles.Place_of_Publication", "FieldDisplayName")]
        public string PlaceofPublication { get; set; }

        [LocalDisplayName("MediaTypes.Media_Type", "FieldDisplayName")]
        public string Media { get; set; }

        [LocalDisplayName("Classmarks.Classmark", "FieldDisplayName")]
        public string Classmark { get; set; }
        
        [LocalDisplayName("Frequency.Frequency", "FieldDisplayName")]
        public string Frequency { get; set; }

        [LocalDisplayName("Languages.Language", "FieldDisplayName")]
        public string Language { get; set; }

        [LocalDisplayName("Titles.Keywords", "FieldDisplayName")]
        public string Keywords { get; set; }

        [LocalDisplayName("Titles.Links", "FieldDisplayName")]
        public string Links { get; set; }

        [LocalDisplayName("Titles.Long_Texts", "FieldDisplayName")]
        public string TitleTexts { get; set; }

        [LocalDisplayName("Titles.Copies", "FieldDisplayName")]
        public int Copies { get; set; }

        [LocalDisplayName("Titles.Date_Catalogued", "FieldDisplayName")]
        public string DateCatalogued { get; set; }
    }

    public class ExportTitlesCopiesViewModel
    {
        [DisplayName("Title ID")]
        public int TitleId { get; set; }

        [LocalDisplayName("Titles.Title", "FieldDisplayName")]
        public string Title { get; set; }

        [LocalDisplayName("Titles.Authors", "FieldDisplayName")]
        public string Author { get; set; }

        [LocalDisplayName("Titles.Edition", "FieldDisplayName")]
        public string Edition { get; set; }

        [LocalDisplayName("Titles.ISBN_ISSN", "FieldDisplayName")]
        public string ISBN { get; set; }

        [LocalDisplayName("Titles.Series", "FieldDisplayName")]
        public string Series { get; set; }

        [LocalDisplayName("Titles.Publisher", "FieldDisplayName")]
        public string Publisher { get; set; }

        [LocalDisplayName("Titles.Published_Year", "FieldDisplayName")]
        public string Year { get; set; }

        [LocalDisplayName("Titles.Place_of_Publication", "FieldDisplayName")]
        public string PlaceofPublication { get; set; }

        [LocalDisplayName("MediaTypes.Media_Type", "FieldDisplayName")]
        public string Media { get; set; }

        [LocalDisplayName("Classmarks.Classmark", "FieldDisplayName")]
        public string Classmark { get; set; }

        [LocalDisplayName("Frequency.Frequency", "FieldDisplayName")]
        public string Frequency { get; set; }

        [LocalDisplayName("Languages.Language", "FieldDisplayName")]
        public string Language { get; set; }

        [LocalDisplayName("Titles.Keywords", "FieldDisplayName")]
        public string Keywords { get; set; }

        [LocalDisplayName("Titles.Links", "FieldDisplayName")]
        public string Links { get; set; }

        [LocalDisplayName("Titles.Long_Texts", "FieldDisplayName")]
        public string TitleTexts { get; set; }

        [LocalDisplayName("Copies.Copy", "FieldDisplayName")]
        public int Copy { get; set; }

        [LocalDisplayName("Copies.Location", "FieldDisplayName")]
        public string Location { get; set; }

        [LocalDisplayName("Copies.Status", "FieldDisplayName")]
        public string Status { get; set; }

        [LocalDisplayName("Copies.Holdings", "FieldDisplayName")]
        public string Holdings { get; set; }

        [LocalDisplayName("CopyItems", "EntityType")]
        public int Volumes { get; set; }

        [LocalDisplayName("Titles.Date_Catalogued", "FieldDisplayName")]
        public string DateCatalogued { get; set; }

    }

    public class ExportTitlesCopiesVolumesViewModel
    {
        [DisplayName("Title ID")]
        public int TitleId { get; set; }

        [LocalDisplayName("Titles.Title", "FieldDisplayName")]
        public string Title { get; set; }

        [LocalDisplayName("Titles.Authors", "FieldDisplayName")]
        public string Author { get; set; }

        [LocalDisplayName("Titles.Edition", "FieldDisplayName")]
        public string Edition { get; set; }

        [LocalDisplayName("Titles.ISBN_ISSN", "FieldDisplayName")]
        public string ISBN { get; set; }

        [LocalDisplayName("Titles.Series", "FieldDisplayName")]
        public string Series { get; set; }

        [LocalDisplayName("Titles.Publisher", "FieldDisplayName")]
        public string Publisher { get; set; }

        [LocalDisplayName("Titles.Published_Year", "FieldDisplayName")]
        public string Year { get; set; }

        [LocalDisplayName("Titles.Place_of_Publication", "FieldDisplayName")]
        public string PlaceofPublication { get; set; }

        [LocalDisplayName("MediaTypes.Media_Type", "FieldDisplayName")]
        public string Media { get; set; }

        [LocalDisplayName("Classmarks.Classmark", "FieldDisplayName")]
        public string Classmark { get; set; }

        [LocalDisplayName("Frequency.Frequency", "FieldDisplayName")]
        public string Frequency { get; set; }

        [LocalDisplayName("Languages.Language", "FieldDisplayName")]
        public string Language { get; set; }

        [LocalDisplayName("Titles.Keywords", "FieldDisplayName")]
        public string Keywords { get; set; }

        [LocalDisplayName("Titles.Links", "FieldDisplayName")]
        public string Links { get; set; }

        [LocalDisplayName("Titles.Long_Texts", "FieldDisplayName")]
        public string TitleTexts { get; set; }

        [LocalDisplayName("Copies.Copy", "FieldDisplayName")]
        public int Copy { get; set; }

        [LocalDisplayName("Copies.Location", "FieldDisplayName")]
        public string Location { get; set; }

        [LocalDisplayName("Copies.Status", "FieldDisplayName")]
        public string Status { get; set; }

        [LocalDisplayName("Copies.Holdings", "FieldDisplayName")]
        public string Holdings { get; set; }

        [LocalDisplayName("CopyItems.Barcode", "FieldDisplayName")]
        public string Barcode { get; set; }

        [LocalDisplayName("CopyItems.Label_Text", "FieldDisplayName")]
        public string LabelText { get; set; }

        //[LocalDisplayName("CopyItems.Is_Ref_Only", "FieldDisplayName")]
        //public bool RefOnly { get; set; }

        [LocalDisplayName("CopyItems.Loan_Type", "FieldDisplayName")]
        public string LoanType { get; set; }

        [LocalDisplayName("CopyItems.Is_On_Loan", "FieldDisplayName")]
        public bool OnLoan { get; set; }

        [LocalDisplayName("Titles.Date_Catalogued", "FieldDisplayName")]
        public string DateCatalogued { get; set; }

    }

    public class ExportFinanceViewModel
    {
        public int OrderID { get; set; }

        [LocalDisplayName("Orders.Order_Number", "FieldDisplayName")]
        public string OrderNo { get; set; }

        [LocalDisplayName("Orders.Date_Ordered", "FieldDisplayName")]
        public string OrderDate { get; set; }

        [LocalDisplayName("Orders.Supplier", "FieldDisplayName")]
        public string Supplier { get; set; }

        [LocalDisplayName("Orders.Title", "FieldDisplayName")]
        public string Title { get; set; }

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

        [LocalDisplayName("Orders.Date_Expected", "FieldDisplayName")]
        public string Expected { get; set; }

        [LocalDisplayName("Orders.Account_Year", "FieldDisplayName")]
        public string AccountYear { get; set; }

        [LocalDisplayName("Orders.Category", "FieldDisplayName")]
        public string OrderCategory { get; set; }

        [LocalDisplayName("Orders.Budget_Code", "FieldDisplayName")]
        public string BudgetCode { get; set; }

        [LocalDisplayName("Orders.Date_Cancelled", "FieldDisplayName")]
        public string Cancelled { get; set; }

        [LocalDisplayName("Orders.Date_Chased", "FieldDisplayName")]
        public string Chased { get; set; }

        [LocalDisplayName("Orders.Report", "FieldDisplayName")]
        public string Report { get; set; }

        [LocalDisplayName("Orders.Date_Received", "FieldDisplayName")]
        public string ReceivedDate { get; set; }

        [LocalDisplayName("Orders.Accepted", "FieldDisplayName")]
        public bool Accepted { get; set; }

        [LocalDisplayName("Orders.Date_Returned", "FieldDisplayName")]
        public string ReturnedDate { get; set; }

        [LocalDisplayName("Orders.Invoice_Reference", "FieldDisplayName")]
        public string InvoiceRef { get; set; }

        [LocalDisplayName("Orders.Date_Invoice_Passed", "FieldDisplayName")]
        public string Passed { get; set; }

        [LocalDisplayName("Orders.Month_Sub_Due", "FieldDisplayName")]
        public int? MonthSubDue { get; set; }

        [LocalDisplayName("Orders.Date_On_Invoice", "FieldDisplayName")]
        public string InvoiceDate { get; set; }

        [StringLength(1000)]
        [LocalDisplayName("Orders.Link_URL", "FieldDisplayName")]
        public string Link { get; set; }

        [LocalDisplayName("Orders.Notes", "FieldDisplayName")]
        public string Notes { get; set; }
    }
}