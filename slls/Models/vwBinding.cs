using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace slls.Models
{
    [Table("dbo.vwBinding")]
    public partial class vwBinding
    {
        [Key]
        public int CopyID { get; set; }

        public int TitleID { get; set;  }

        public string Title { get; set; }

        public string Publisher { get; set; }

        public string Media { get; set; }

        public string Classmark { get; set; }

        public string Citation { get; set; }

        public string Office { get; set; }

        public string Location { get; set; }

        [DisplayName("Copy No")]
        public int CopyNumber { get; set; }

        [Column("Acquisitions No")]
        [DisplayName("Acquisitions No")]
        public string AcquisitionsNo { get; set; }

        public string Holdings { get; set; }

        [Column("Copy Notes")]
        [DisplayName("Copy Notes")]
        public string CopyNotes { get; set; }

    }
}