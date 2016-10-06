namespace slls.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TempExportQuery")]
    public partial class TempExportQuery
    {
        [Key]
        public int ExportQryID { get; set; }

        [Required]
        public string QueryString { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime InputDate { get; set; }
    }
}
