using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace slls.Models
{
    [Table("vwSelectOpacPublisher")]
    public class vwSelectPublisherOpac
    {
        [Key]
        public int PublisherID { get; set; }
        public string PublisherName { get; set; }
        public int Titles { get; set; }
    }
}