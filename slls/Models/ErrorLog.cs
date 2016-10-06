namespace slls.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ErrorLog")]
    public partial class ErrorLog
    {
        [Key]
        public int ErrLogID { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? ErrDate { get; set; }

        [StringLength(255)]
        public string ErrType { get; set; }

        [StringLength(255)]
        public string ErrURL { get; set; }

        [StringLength(1000)]
        public string ErrMessage { get; set; }

        public string ErrStackTrace { get; set; }

        public bool Digest { get; set; }
    }
}
