using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Web;

namespace slls.Models
{
    [Table("DashboardGadgets")]
    public class DashboardGadget
    {
        [Key]
        public int RecId { get; set; }

        public string Area { get; set; }

        public string Name { get; set; }

        public int Column { get; set; }

        public int Row { get; set; }

        public bool Visible { get; set; }
    }
}