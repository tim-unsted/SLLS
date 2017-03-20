using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace slls.Models
{
    [Table("vwSelectTitleToBorrow")]
    public class vwSelectTitleToBorrow
    {
        [Key]
        [Column("TitleID")]
        public int TitleId { get; set; }
        public string Title { get; set; }
        public string Year { get; set; }
        public string Edition { get; set; }
        public string AuthorString { get; set; }
        public int NonFilingChars { get; set; }
    }
}