using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AutoCat.ViewModels
{
    public class AutoCatNewTitle
    {
        [StringLength(450)]
        public string Title { get; set; }
        
        public string Media { get; set; }

        public string Classmark { get; set; }

        public string Publisher { get; set; }

        public string Language { get; set; }

        public string Frequency { get; set; }

        [StringLength(255)]
        public string Series { get; set; }

        [StringLength(255)]
        public string Edition { get; set; }

        [StringLength(50)]
        public string PlaceofPublication { get; set; }

        public string PublicationDate { get; set; }

        public string Year { get; set; }

        public string Isbn { get; set; }

        public string ISBN10 { get; set; }

        public string ISBN13 { get; set; }

        public string Source { get; set; }

        public string Description { get; set; }

        public string Citation { get; set; }

        public string Contents { get; set; }

        public string Reviews { get; set; }

        public List<string> Author { get; set; }

        public List<string> Keywords { get; set; }

        public int NonFilingChars { get; set; }

        public string Notes { get; set; }

        public string ErrorMessage { get; set; }

        public string ImageUrl { get; set; }
    }

    

    public class CopacRecordViewModel
    {
        [Key]
        public int RecordId { get; set; }

        [DisplayName("Author(s)")]
        public string Author { get; set; }
        [DisplayName("Publisher")]
        public string Publisher { get; set; }
        public string Title { get; set; }
        [DisplayName("Sub Title")]
        public string SubTitle { get; set; }
        [DisplayName("Year")]
        public string PubYear { get; set; }
        [DisplayName("ISBN/ISSN")]
        public string Isbn { get; set; }
        [DisplayName("ISBN/ISSN")]
        public string Isbn10 { get; set; }
        [DisplayName("ISBN/ISSN")]
        public string Isbn13 { get; set; }
        public string Language { get; set; }
        public string Library { get; set; }
        public string Edition { get; set; }
        [DisplayName("Place of Publication")]
        public string Place { get; set; }
        [DisplayName("Year")]
        public string DateIssued { get; set; }
        public string NamePart { get; set; }
        public string Series { get; set; }
        public string Personal { get; set; }
        public bool AddTitle { get; set; }

        public CopacRecordViewModel(int id)
        {
            RecordId = id;
        }

        public CopacRecordViewModel() : this(0) { }
    }


    public class CopacSearchResults
    {
        public List<CopacRecordViewModel> CopacRecords { get; set; }

        public CopacSearchResults()
        {
            CopacRecords = new List<CopacRecordViewModel>();
        }

        public bool HasErrors { get; set; }
        public int ResultsCount { get; set; }
        public string Who { get; set; }
    }
}