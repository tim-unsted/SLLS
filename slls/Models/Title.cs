using System.ComponentModel;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using slls.Localization;

namespace slls.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Titles")]
    public partial class Title
    {
        public Title()
        {
            //Copies = new HashSet<Copy>();
            //OrderDetails = new HashSet<OrderDetail>();
            //SubjectIndexes = new HashSet<SubjectIndex>();
            //TitleAuthors = new HashSet<TitleAuthor>();
            //TitleAdditionalFieldDatas = new HashSet<TitleAdditionalFieldData>();
            //TitleEditors = new HashSet<TitleEditor>();
            //TitleImages = new HashSet<TitleImage>();
            //TitleLinks = new HashSet<TitleLink>();
        }

        [Key]
        public int TitleID { get; set; }

        [Column("Title")]
        [LocalDisplayName("Titles.Title", "FieldDisplayName")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "A blank title is not allowed. Please enter a valid title.")]
        [StringLength(450)]
        public string Title1 { get; set; }

        [LocalDisplayName("Titles.Non_Filing_Characters", "FieldDisplayName")]
        public int NonFilingChars { get; set; }

        public string FiledTitle
        {
            get
            {
                return NonFilingChars > 0 ? Title1.Substring(NonFilingChars) : Title1;
            }
        }

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

        public int? PubMonth { get; set; }

        public int? PubDay { get; set; }

        [Column(TypeName = "money")]
        [LocalDisplayName("Titles.Price", "FieldDisplayName")]
        public decimal? Price { get; set; }
        
        [StringLength(50)]
        [LocalDisplayName("Titles.ISBN_10", "FieldDisplayName")]
        public string ISBN10 { get; set; }

        [StringLength(50)]
        [LocalDisplayName("Titles.ISBN_13", "FieldDisplayName")]
        public string ISBN13 { get; set; }

        [StringLength(255)]
        [LocalDisplayName("Titles.Citation", "FieldDisplayName")]
        public string Citation { get; set; }

        [StringLength(255)]
        [LocalDisplayName("Titles.Source", "FieldDisplayName")]
        public string Source { get; set; }
        
        [StringLength(255)]
        [LocalDisplayName("Titles.Description", "FieldDisplayName")]
        public string Description { get; set; }

        [LocalDisplayName("Titles.Notes", "FieldDisplayName")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        [Column(TypeName = "smalldatetime")]
        [LocalDisplayName("Titles.Date_Catalogued", "FieldDisplayName")]
        public DateTime? DateCatalogued { get; set; }

        [StringLength(128)]
        [LocalDisplayName("Titles.Catalogued_By", "FieldDisplayName")]
        public string CataloguedBy { get; set; }

        [NotMapped]
        public string DateCataloguedSortable
        {
            get { return string.Format("{0:yyyy-MM-dd}", DateCatalogued); }
        }

        [Column(TypeName = "smalldatetime")]
        [LocalDisplayName("Titles.Date_Last_Modified", "FieldDisplayName")]
        public DateTime? LastModified { get; set; }

        [StringLength(128)]
        [LocalDisplayName("Titles.Modified_By", "FieldDisplayName")]
        public string ModifiedBy { get; set; }

        [DisplayName("Deleted?")]
        public bool Deleted { get; set; }

        [Column(TypeName = "timestamp")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [MaxLength(8)]
        public byte[] RowVersion { get; set; }

        [LocalDisplayName("Titles.Imprint", "FieldDisplayName")]
        public string Imprint
        {
            get
            {
                {
                    return string.Format("{0} {1} {2}", string.IsNullOrEmpty(PlaceofPublication) ? "" : PlaceofPublication + ": ", string.IsNullOrEmpty(Publisher.PublisherName) ? "" : Publisher.PublisherName + ", ", Year);
                }
            }
        }

        [LocalDisplayName("Titles.Catalogue", "FieldDisplayName")]
        public string CatalogueByAuthor
        {
            get
            {
                var catalogue = string.Format("{0} {1} {2}", string.IsNullOrEmpty(AuthorString) ? "" : AuthorString.Trim() + " - ", string.IsNullOrEmpty(Title1) ? "" : Title1.Trim(), string.IsNullOrEmpty(Edition) ? "" : ", " + Edition.Trim());
                return catalogue.Trim();
            }
        }

        [LocalDisplayName("Titles.Catalogue", "FieldDisplayName")]
        public string CatalogueByTitle
        {
            get
            {
                var catalogue = string.Format("{0} {1} {2}", string.IsNullOrEmpty(Title1) ? "" : Title1.Trim(), string.IsNullOrEmpty(AuthorString) ? "" : " - " + AuthorString.Trim(), string.IsNullOrEmpty(Edition) ? "" : ", " + Edition.Trim());
                return catalogue.Trim();
            }
        }

        public string AuthorAndImprint
        {
            get
            {
                var catalogue = string.Format("{0} {1}", string.IsNullOrEmpty(AuthorString) ? "" : AuthorString + " - ", Imprint);
                return catalogue.Trim();
            }
        }

        public string DescriptionAndImprint
        {
            get
            {
                var catalogue = string.Format("{0} {1}", string.IsNullOrEmpty(Description) ? "" : Description + " - ", Imprint);
                return catalogue.Trim();
            }
        }

        public string TitleAndEdition
        {
            get
            {
                var catalogue = string.Format("{0} {1}", Title1, string.IsNullOrEmpty(Edition) ? "" : " (" + Edition + ")");
                return catalogue.Trim();
            }
        }

        [LocalDisplayName("Titles.ISBN_ISSN", "FieldDisplayName")]
        public string Isbn
        {
            get { return string.IsNullOrEmpty(ISBN13) ? ISBN10 : ISBN13; }
        }

        [LocalDisplayName("Titles.Authors", "FieldDisplayName")]
        public string AuthorString
        {
            get
            {
                if (!TitleAuthors.Any()) return "";
                var authors =
                    TitleAuthors.OrderBy(author => author.OrderSeq)
                        .ThenBy(author => author.TitleAuthorId)
                        .Select(author => author.Author.DisplayName)
                        .ToList();
                return string.Join("; ", authors);
            }
        }

        [LocalDisplayName("Titles.Editors", "FieldDisplayName")]
        public string EditorString
        {
            get
            {
                if (!TitleEditors.Any()) return "";
                var editors =
                    TitleEditors.OrderBy(author => author.OrderSeq)
                        .ThenBy(author => author.TitleEditorID)
                        .Select(author => author.Author.DisplayName)
                        .ToList();
                return string.Join("; ", editors);
            }
        }

        public string AuthorEditorString
        {
            get
            {
                var authoreditors = string.Format("{0} {1}", AuthorString, EditorString);
                return authoreditors.Trim();
            }
        }

        [LocalDisplayName("Titles.Keywords", "FieldDisplayName")]
        public string KeywordString
        {
            get
            {
                if (!SubjectIndexes.Any()) return "";
                var keywords =
                    SubjectIndexes.OrderBy(keyword => keyword.Keyword.KeywordTerm)
                        .Select(keyword => keyword.Keyword.KeywordTerm)
                        .ToList();
                return string.Join("; ", keywords);
            }
        }

        [LocalDisplayName("Titles.Links", "FieldDisplayName")]
        public string LinkString
        {
            get
            {
                if (!TitleLinks.Any()) return "";
                var links =
                    TitleLinks.OrderBy(l => l.URL)
                        .Select(l => l.URL)
                        .ToList();
                return string.Join("; ", links);
            }
        }

        [LocalDisplayName("Titles.Long_Texts", "FieldDisplayName")]
        public string TitleTextString
        {
            get
            {
                if (!TitleAdditionalFieldDatas.Any()) return "";
                var texts =
                    TitleAdditionalFieldDatas.Where(x => x.TitleAdditionalFieldDef.IsLongText).OrderBy(t => t.TitleAdditionalFieldDef.FieldName)
                        .Select(t => t.TitleAdditionalFieldDef.FieldName + ": " + t.FieldData)
                        .ToList();
                return string.Join("; ", texts);
            }
        }

        [LocalDisplayName("Titles.Custom_Data", "FieldDisplayName")]
        public string CustomDataString
        {
            get
            {
                if (!TitleAdditionalFieldDatas.Any()) return "";
                var customdata =
                    TitleAdditionalFieldDatas.Where(x => x.TitleAdditionalFieldDef.IsLongText == false).OrderBy(t => t.TitleAdditionalFieldDef.FieldName)
                        .Select(t => t.TitleAdditionalFieldDef.FieldName + ": " + t.FieldData)
                        .ToList();
                return string.Join("; ", customdata);
            }
        }

        public string SearchString
        {
            get
            {
                return string.Join(Title1, AuthorString, EditorString, Citation, Description, Edition, Isbn, KeywordString, Notes, Series, Source, Publisher.PublisherName, LinkString, TitleTextString, CustomDataString);
            }
        }
        
        public virtual Classmark Classmark { get; set; }

        public virtual Publisher Publisher { get; set; }

        [LocalDisplayName("Titles.Copies", "FieldDisplayName")]
        public virtual ICollection<Copy> Copies { get; set; }

        public virtual Frequency Frequency { get; set; }

        public virtual Language Language { get; set; }

        public virtual MediaType MediaType { get; set; }

        [LocalDisplayName("Titles.Order_Details", "FieldDisplayName")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

        [LocalDisplayName("Titles.Keywords", "FieldDisplayName")]
        public virtual ICollection<SubjectIndex> SubjectIndexes { get; set; }

        [LocalDisplayName("Titles.Authors", "FieldDisplayName")]
        public virtual ICollection<TitleAuthor> TitleAuthors { get; set; }

        public virtual ICollection<TitleAdditionalFieldData> TitleAdditionalFieldDatas { get; set; }

        [LocalDisplayName("Titles.Editors", "FieldDisplayName")]
        public virtual ICollection<TitleEditor> TitleEditors { get; set; }

        [LocalDisplayName("Titles.Cover_Images", "FieldDisplayName")]
        public virtual ICollection<TitleImage> TitleImages { get; set; }

        [LocalDisplayName("Titles.Links", "FieldDisplayName")]
        public virtual ICollection<TitleLink> TitleLinks { get; set; }

        [NotMapped]
        public bool HasData { get; set; }

        [NotMapped]
        public string NoDataTitle { get; set; }

        [NotMapped]
        public string NoDataMsg { get; set; }

        [NotMapped]
        public IEnumerable<TitleAdditionalFieldData> CustomFields
        {
            get { return TitleAdditionalFieldDatas.Where(d => d.TitleAdditionalFieldDef.IsLongText == false); }
        }

        [NotMapped]
        public IEnumerable<TitleAdditionalFieldData> OpacCustomFields
        {
            get { return TitleAdditionalFieldDatas.Where(d => d.TitleAdditionalFieldDef.IsLongText == false && d.TitleAdditionalFieldDef.ShowOnOPAC); }
        }

        [NotMapped]
        public IEnumerable<TitleAdditionalFieldData> LongTexts
        {
            get { return TitleAdditionalFieldDatas.Where(d => d.TitleAdditionalFieldDef.IsLongText); }
        }

        [NotMapped]
        public IEnumerable<TitleAdditionalFieldData> OpacLongTexts
        {
            get { return TitleAdditionalFieldDatas.Where(d => d.TitleAdditionalFieldDef.IsLongText && d.TitleAdditionalFieldDef.ShowOnOPAC); }
        }

    }
}
