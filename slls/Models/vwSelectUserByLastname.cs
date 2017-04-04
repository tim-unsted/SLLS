using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace slls.Models
{
    [Table("vwSelectUserByLastname")]
    public class vwSelectUserByLastname
    {
        [Key]
        public string Id { get; set; }
        public string FullNameRev { get; set; }
        public bool IsLive { get; set; }
    }
}