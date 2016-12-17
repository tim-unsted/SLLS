using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace slls.Models
{
    public class OpacSearchResults
    {
        [Key]
        public int TitleId { get; set; }
        public string Title { get; set; }
        public string Series { get; set; }
        public string Edition { get; set; }
        public string ISBN10 { get; set; }
        public string ISBN { get; set; }
        public string Citation { get; set; }
        public string Source { get; set; }
        public string Description { get; set; }
        public string PlaceOfPublication { get; set; }
        public int MediaID { get; set; }
        public int ClassmarkID { get; set; }
        public int LanguageID { get; set; }
        public int PublisherID { get; set; }
        public int FrequencyID { get; set; }
        public string Data { get; set; }
        public string Keywords { get; set; }
        public string Authors { get; set; }
        public string Editors { get; set; }
        public string Holdings { get; set; }
        public string Label { get; set; }
        public int Rank { get; set; }
        public string Classmark { get; set; }
        public string Year { get; set; }
        public int NonFilingChars { get; set; }
    }
}