using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using slls.Localization;
using slls.Models;
using Westwind.Globalization;
using Roles = slls.Utils.Helpers.Roles;

namespace slls.ViewModels
{
    public class TitleClassmarks
    {
        public int ClassmarkID { get; set; }
        public string Classmark { get; set; }
        public int TitleCount { get; set; }
    }

    public class TitlesListViewModel
    {
        public List<Title> Titles { get; set; }
        //public IEnumerable<Title> OpacTitles { get; set; }
        public List<MediaType> MediaTypes { get; set; }
        public List<Classmark> Classmarks { get; set; }
        public List<Publisher> Publishers { get; set; }
        public List<Keyword> Keywords { get; set; }
        public List<Language> Languages { get; set; }
        public List<Author> Authors { get; set; }
        public Dictionary<string, string> Filters { get; set; }
        public IEnumerable<Int32> TitleIDs { get; set; }

        public int ClassmarkId { get; set; }
        public int MediaId { get; set; }
        public int PublisherId { get; set; }
        public int LanguageId { get; set; }
        public int KeywordId { get; set; }
        public int AuthorId { get; set; }
        public string SelectedKeyword { get; set; }
        public string SelectItem { get; set; }

        //Stuff to handle the alphabetical paging links on the index view ...
        public List<string> FirstLetters { get; set; }
        public string SelectedLetter { get; set; }

        //Is the current user an admin user
        public bool LibraryStaff { get; set; }

        public string CurrentView { get; set; }

    }

    public class NewTitlesListViewModel
    {
        //[Key]
        public int Id { get; set; }

        public int CopyId { get; set; }

        public int TitleId { get; set; }

        [LocalDisplayName("Titles.Title", "FieldDisplayName")]
        public string Title { get; set; }

        [LocalDisplayName("Titles.Authors", "FieldDisplayName")]
        public string Author { get; set; }

        [LocalDisplayName("Copies.Date_Added_To_New_Titles_List", "FieldDisplayName")]
        public DateTime DateAdded { get; set; }

        [LocalDisplayName("Titles.Edition", "FieldDisplayName")]
        public string Edition { get; set; }

        [LocalDisplayName("Titles.Publisher", "FieldDisplayName")]
        public string Publisher { get; set; }

        [LocalDisplayName("Titles.ISBN_ISSN", "FieldDisplayName")]
        public string ISBN { get; set; }

        [LocalDisplayName("Titles.Published_Year", "FieldDisplayName")]
        public string Year { get; set; }

        [LocalDisplayName("Copies.Copy", "FieldDisplayName")]
        public int Copy { get; set; }

        [LocalDisplayName("Copies.Location", "FieldDisplayName")]
        public string Location { get; set; }

        [Column(TypeName = "smalldatetime")]
        [LocalDisplayName("Titles.Date_Catalogued", "FieldDisplayName")]
        public DateTime? DateCatalogued { get; set; }

        [LocalDisplayName("Titles.Catalogued_By", "FieldDisplayName")]
        public string CataloguedBy { get; set; }

        public string DateCataloguedSortable
        {
            get { return string.Format("{0:yyyy-MM-dd}", DateCatalogued); }
        }

    }

    public class NewTitlesSimpleViewModel
    {
        public int TitleId { get; set; }
        public string Title { get; set; }
        public int NonFilingChars { get; set; }
        public DateTime? Commenced { get; set; }
    }

    public class TitleAddViewModel
    {
        public TitleAddViewModel()
        {
            this.Authors = new List<Author>();
            this.Editors = new List<Author>();
            this.Step = 1;
        }

        [Column("Title")]
        [StringLength(450)]
        [LocalDisplayName("Titles.Title", "FieldDisplayName")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "A blank title is not allowed. Please enter a valid title.")]
        public string Title1 { get; set; }

        [LocalDisplayName("MediaTypes.Media_Type", "FieldDisplayName")]
        public int MediaID { get; set; }

        [LocalDisplayName("Classmarks.Classmark", "FieldDisplayName")]
        public int ClassmarkID { get; set; }

        [LocalDisplayName("Titles.Publisher", "FieldDisplayName")]
        public int PublisherID { get; set; }

        [LocalDisplayName("Frequency.Frequency", "FieldDisplayName")]
        public int FrequencyID { get; set; }

        [LocalDisplayName("Languages.Language", "FieldDisplayName")]
        public int LanguageID { get; set; }

        [StringLength(255)]
        [LocalDisplayName("Titles.Series", "FieldDisplayName")]
        public string Series { get; set; }

        [StringLength(255)]
        [LocalDisplayName("Titles.Edition", "FieldDisplayName")]
        public string Edition { get; set; }

        [StringLength(50)]
        [LocalDisplayName("Titles.Place_of_Publication", "FieldDisplayName")]
        public string PlaceofPublication { get; set; }

        [StringLength(50)]
        [LocalDisplayName("Titles.Published_Year", "FieldDisplayName")]
        public string Year { get; set; }

        [StringLength(50)]
        [LocalDisplayName("Titles.ISBN_ISSN", "FieldDisplayName")]
        public string Isbn { get; set; }

        [StringLength(50)]
        [LocalDisplayName("Titles.ISBN_10", "FieldDisplayName")]
        public string ISBN10 { get; set; }

        [StringLength(50)]
        [LocalDisplayName("Titles.ISBN_13", "FieldDisplayName")]
        public string ISBN13 { get; set; }

        [StringLength(255)]
        [LocalDisplayName("Titles.Source", "FieldDisplayName")]
        public string Source { get; set; }

        [StringLength(255)]
        [LocalDisplayName("Titles.Description", "FieldDisplayName")]
        public string Description { get; set; }

        [Column(TypeName = "money")]
        [LocalDisplayName("Titles.Price", "FieldDisplayName")]
        public decimal? Price { get; set; }

        [StringLength(255)]
        [DataType(DataType.MultilineText)]
        [LocalDisplayName("Titles.Citation", "FieldDisplayName")]
        public string Citation { get; set; }

        [LocalDisplayName("Titles.Non_Filing_Characters", "FieldDisplayName")]
        public int NonFilingChars { get; set; }

        [LocalDisplayName("Titles.Notes", "FieldDisplayName")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        [LocalDisplayName("Titles.Authors", "FieldDisplayName")]
        public List<Author> Authors { get; set; }

        [LocalDisplayName("Titles.Editors", "FieldDisplayName")]
        public List<Author> Editors { get; set; }

        public int Step { get; set; }

    }

    public sealed class TitleEditViewModel
    {
        //public Models.Title _title { get; set; }

        public int TitleID { get; set; }

        [Column("Title")]
        [StringLength(450)]
        [LocalDisplayName("Titles.Title", "FieldDisplayName")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "A blank title is not allowed. Please enter a valid title.")
        ]
        public string Title1 { get; set; }

        public string SelectTitle { get; set; }
        public string SelectTitleID { get; set; }
        
        public ICollection<TitleAdditionalFieldData> TitleAdditionalFieldDatas { get; set; }

        [LocalDisplayName("Titles.Keywords", "FieldDisplayName")]
        public ICollection<SubjectIndex> SubjectIndexes { get; set; }

        [LocalDisplayName("Titles.Copies", "FieldDisplayName")]
        public ICollection<Copy> Copies { get; set; }

        public bool HasCopies { get; set; }

        [LocalDisplayName("Titles.Order_Details", "FieldDisplayName")]
        public ICollection<OrderDetail> OrderDetails { get; set; }

        [LocalDisplayName("Titles.Links", "FieldDisplayName")]
        public ICollection<TitleLink> TitleLinks { get; set; }

        [LocalDisplayName("Titles.Cover_Images", "FieldDisplayName")]
        public ICollection<TitleImage> TitleImages { get; set; }
        
        public bool IsFinanceAdmin { get; set; }

        public string SelectTitleTip { get; set; }

        public string SelectTitlePlaceHolder { get; set; }

        public string RedirectAction { get; set; }

        public TitleEditViewModel()
        {
            IsFinanceAdmin = Roles.IsUserInRole("Finance Admin");
            SelectTitleTip = "To view/edit another item, start typing part of the title ...";
            SelectTitlePlaceHolder = "Start typing the term you are looking for ...";
            RedirectAction = "Index";
        }
    }

    public class TitleDetailsViewModel
    {
        public Models.Title _title { get; set; }

        public int TitleID { get; set; }

        [Column("Title")]
        [StringLength(450)]
        [LocalDisplayName("Titles.Title", "FieldDisplayName")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "A blank title is not allowed. Please enter a valid title.")]
        public string Title1 { get; set; }

        [LocalDisplayName("MediaTypes.Media_Type", "FieldDisplayName")]
        public int MediaID { get; set; }

        [LocalDisplayName("Classmarks.Classmark", "FieldDisplayName")]
        public int ClassmarkID { get; set; }

        [LocalDisplayName("Titles.Publisher", "FieldDisplayName")]
        public int PublisherID { get; set; }

        [LocalDisplayName("Frequency.Frequency", "FieldDisplayName")]
        public int FrequencyID { get; set; }

        [LocalDisplayName("Languages.Language", "FieldDisplayName")]
        public int LanguageID { get; set; }

        [StringLength(255)]
        [LocalDisplayName("Titles.Series", "FieldDisplayName")]
        public string Series { get; set; }

        [StringLength(255)]
        [LocalDisplayName("Titles.Edition", "FieldDisplayName")]
        public string Edition { get; set; }

        [StringLength(50)]
        [LocalDisplayName("Titles.Place_of_Publication", "FieldDisplayName")]
        public string PlaceofPublication { get; set; }

        [StringLength(50)]
        [LocalDisplayName("Titles.Published_Year", "FieldDisplayName")]
        public string Year { get; set; }

        [StringLength(50)]
        [LocalDisplayName("Titles.ISBN_10", "FieldDisplayName")]
        public string ISBN10 { get; set; }

        [StringLength(50)]
        [LocalDisplayName("Titles.ISBN_13", "FieldDisplayName")]
        public string ISBN13 { get; set; }

        [StringLength(255)]
        [LocalDisplayName("Titles.Source", "FieldDisplayName")]
        public string Source { get; set; }

        [StringLength(255)]
        [LocalDisplayName("Titles.Description", "FieldDisplayName")]
        public string Description { get; set; }

        [StringLength(255)]
        [LocalDisplayName("Titles.Citation", "FieldDisplayName")]
        public string Citation { get; set; }

        [LocalDisplayName("Titles.Non_Filing_Characters", "FieldDisplayName")]
        public int NonFilingChars { get; set; }

        [LocalDisplayName("Titles.Notes", "FieldDisplayName")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        public Classmark Classmark { get; set; }
        public Supplier Supplier { get; set; }
        public Frequency Frequency { get; set; }
        public Language Language { get; set; }
        public MediaType MediaType { get; set; }

        [LocalDisplayName("Titles.Authors", "FieldDisplayName")]
        public ICollection<TitleAuthor> TitleAuthors { get; set; }

        [LocalDisplayName("Titles.Editors", "FieldDisplayName")]
        public ICollection<TitleEditor> TitleEditors { get; set; }

        [Editable(false)]
        [LocalDisplayName("Titles.Authors", "FieldDisplayName")]
        public string authors { get; set; }

        [Editable(false)]
        [LocalDisplayName("Titles.Editors", "FieldDisplayName")]
        public string editors { get; set; }

    }

    public class TitleDeleteViewModel
    {
        public int TitleId { get; set; }

        [LocalDisplayName("Titles.Title", "FieldDisplayName")]
        public string Title1 { get; set; }

        [LocalDisplayName("Titles.Edition", "FieldDisplayName")]
        public string Edition { get; set; }

        [LocalDisplayName("Titles.Published_Year", "FieldDisplayName")]
        public string Year { get; set; }

        [LocalDisplayName("Titles.Description", "FieldDisplayName")]
        public string Description { get; set; }

        [LocalDisplayName("Titles.Notes", "FieldDisplayName")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        [LocalDisplayName("Titles.ISBN_ISSN", "FieldDisplayName")]
        public string Isbn { get; set; }

        public string CurrentViewName { get; set; }
    }

    public class BarcodeEnquiryViewModel
    {
        [Required]
        [LocalDisplayName("CopyItems.Barcode", "FieldDisplayName")]
        [Remote("BarcodeExists", "Searching", HttpMethod = "POST", ErrorMessage = "The Barcode you entered does not exist!")]
        public string Barcode { get; set; }
    }

    public class IsbnLookupViewModel
    {
        [Required]
        [LocalDisplayName("Titles.ISBN_ISSN", "FieldDisplayName")]
        [Remote("IsbnExists", "Searching", HttpMethod = "POST", ErrorMessage = "The ISBN/ISSN you have entered does not currently exist!")]
        public string Isbn { get; set; }
    }

    public class OpacBarcodeEnquiryViewModel
    {
        [Required]
        [LocalDisplayName("CopyItems.Barcode", "FieldDisplayName")]
        [Remote("BarcodeExists", "Home", HttpMethod = "POST", ErrorMessage = "The Barcode you entered does not exist!")]
        public string Barcode { get; set; }
    }

    public class AddTitleWithAutoCatViewModel
    {
        public AddTitleWithAutoCatViewModel()
        {
            this.EntityName = "Title";
        }
        [Key]
        public int TitleID { get; set; }

        public string Who { get; set; }

        [DataType(DataType.MultilineText)]
        [LocalDisplayName("Titles.ISBN_ISSN", "FieldDisplayName")]
        public string IsbnInput { get; set; }

        [LocalDisplayName("Titles.ISBN_ISSN", "FieldDisplayName")]
        //[Remote("IsbnUnique", "Titles", HttpMethod = "POST", ErrorMessage = "You already have an item with this ISBN in the database!")]
        public string Isbn { get; set; }

        public List<string> Sources { get; set; }
        public IEnumerable<string> IsbnList { get; set; }

        public IEnumerable<SelectListItem> ErrorList { get; set; }

        public bool HasErrors { get; set; }

        //public string ErrorMessage { get; set; }

        public string EntityName { get; set; }
    }

    public class CopacSearchCriteria
    {
        [Key]
        public int SearchId { get; set; }

        [LocalDisplayName("Titles.Authors", "FieldDisplayName")]
        public string Author { get; set; }

        [LocalDisplayName("Titles.Publisher", "FieldDisplayName")]
        public string Publisher { get; set; }

        [LocalDisplayName("Titles.Title", "FieldDisplayName")]
        public string Title { get; set; }

        [LocalDisplayName("Titles.Published_Year", "FieldDisplayName")]
        public string PubYear { get; set; }

        [LocalDisplayName("Titles.ISBN_ISSN", "FieldDisplayName")]
        public string CopacIsbn { get; set; }

        [LocalDisplayName("Languages.Language", "FieldDisplayName")]
        public string Language { get; set; }

        public string Library { get; set; }
    }

    public class DuplicateTitleViewModel
    {
        public DuplicateTitleViewModel()
        {
            Source = "";
        }

        public int TitleId { get; set; }

        [LocalDisplayName("Titles.Title", "FieldDisplayName")]
        public string Title1 { get; set; }

        [LocalDisplayName("Titles.Edition", "FieldDisplayName")]
        public string Edition { get; set; }

        [LocalDisplayName("Titles.Published_Year", "FieldDisplayName")]
        public string Year { get; set; }

        [LocalDisplayName("Titles.Description", "FieldDisplayName")]
        public string Description { get; set; }

        [LocalDisplayName("Titles.Notes", "FieldDisplayName")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        [LocalDisplayName("Titles.ISBN_ISSN", "FieldDisplayName")]
        public string Isbn { get; set; }

        public string CurrentViewName { get; set; }

        public string Source { get; set; }
    }

    public class SelectTitleViewmodel
    {
        public SelectTitleViewmodel()
        {

        }
        public string Title { get; set; }
        public string Message { get; set; }
        public string BtnText { get; set; }

        [AllowHtml]
        public string HelpText { get; set; }

        public string PlaceHolderText { get; set; }

        public string ReturnAction { get; set; }
        public IEnumerable<SelectListItem> Titles { get; set; }
        
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select a Title!")]
        [Required(ErrorMessage = "Please select a Title!")]
        public int TitleID { get; set; }

        public string Tab { get; set; }
    }

    public class SelectTitles
    {
        public int TitleID { get; set; }
        public string Title { get; set; }
        public string Year { get; set; }
        public string Edition { get; set; }
        public string Authors { get; set; }
        public int NonFilingChars { get; set; }
    }

}