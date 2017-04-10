using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace slls.Models
{
    [Table("vwSelectOpacLanguage")]
    public class vwSelectLanguageOpac
    {
        [Key]
        public int LanguageID { get; set; }
        public string Language { get; set; }
        public int Titles { get; set; }
    }
}