using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace slls.Models
{
    [Table("vwSelectOpacClassmark")]
    public class vwSelectClassmarkOpac
    {
        [Key]
        [Column("ClassmarkID")]
        public int ClassmarkId { get; set; }
        public string Classmark { get; set; }
        public string Code { get; set; }
        public int Titles { get; set; }
    }
}