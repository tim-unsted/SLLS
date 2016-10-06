using System.ComponentModel;

namespace slls.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ReportTypes")]
    public partial class ReportType
    {
        [Key]
        public int ReportID { get; set; }

        [StringLength(128)]
        [DisplayName("Friendly Name")]
        public string FriendlyName { get; set; }

        [StringLength(128)]
        [DisplayName("Report Name")]
        public string ReportName { get; set; }

        [StringLength(128)]
        [DisplayName("Report Area")]
        public string ReportArea { get; set; }

        [StringLength(255)]
        [DisplayName("Filter")]
        public string Filter { get; set; }
    }
}
