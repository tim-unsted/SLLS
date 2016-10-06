using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using slls.Localization;

namespace slls.Models
{
    [Table("LibraryUserSavedSearches")]
    public class LibraryUserSavedSearch
    {
        [Key]
        public int SavedSearchID { get; set; }
        public string UserID { get; set; }
        public string SearchString { get; set; }
        public string SearchField { get; set; }
        public string Scope { get; set; }
        public string ClassmarksFilter { get; set; }
        public string MediaFilter { get; set; }
        public string PublisherFilter { get; set; }
        public string LanguageFilter { get; set; }
        public string KeywordFilter { get; set; }
        public string AuthorFilter { get; set; }
        public string AccountYearsFilter { get; set; }
        public string BudgetCodesFilter { get; set; }
        public string OrderCategoriesFilter { get; set; }
        public string RequestersFilter { get; set; }
        public string SuppliersFilter { get; set; }
        public string Description { get; set; }

        [Column(TypeName = "smalldatetime")]
        [LocalDisplayName("Saved_Searches.Date_Saved", "FieldDisplayName")]
        public DateTime? InputDate { get; set; }

        [NotMapped]
        public string InputDateSortable
        {
            get { return string.Format("{0:yyyy-MM-dd}", InputDate); }
        }
        //public virtual LibraryUser LibraryUser { get; set; }
    }
}