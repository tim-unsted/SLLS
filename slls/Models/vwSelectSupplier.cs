using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace slls.Models
{
    [Table("vwSelectSupplier")]
    public class vwSelectSupplier
    {
        [Column("SupplierID")]
        [Key]
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
    }
}