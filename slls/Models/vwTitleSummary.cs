using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace slls.Models
{
    [Table("vwTitleSummary")]
    public class vwTitleSummary
    {
        [Column("TitleID")]
        [Key]
        public int TitleId { get; set; }
        public string Title { get; set; }
        public int NonFilingChars { get; set; }
        public int CopiesCount { get; set; }
        public int TitleLinksCount { get; set; }
        public int OrdersCount { get; set; }
        public int SubjectsCount { get; set; }
        public int LongTextsCount { get; set; }
        public int ImagesCount { get; set; } 

    }
}