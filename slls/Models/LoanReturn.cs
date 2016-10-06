namespace slls.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LoanReturn")]
    public partial class LoanReturn
    {
        [Key]
        public int ReturnID { get; set; }

        public int VolumeID { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? Returned { get; set; }
    }
}
