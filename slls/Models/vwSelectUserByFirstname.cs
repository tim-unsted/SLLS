using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace slls.Models
{
    [Table("vwSelectUserByFirstname")]
    public class vwSelectUserByFirstname
    {
        [Key]
        public string Id { get; set; }
        public string FullName { get; set; }
    }
}