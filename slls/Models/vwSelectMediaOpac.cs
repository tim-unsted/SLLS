using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace slls.Models
{
    [Table("vwSelectOpacMedia")]
    public class vwSelectMediaOpac
    {
        [Key]
        public int MediaID { get; set; }
        public string Media { get; set; }
        public int Titles { get; set; }
    }
}