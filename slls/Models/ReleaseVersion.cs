namespace slls.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ReleaseVersion")]
    public partial class ReleaseVersion
    {
        [Key]
        public int RecID { get; set; }

        [StringLength(50)]
        public string VersionNumber { get; set; }

        public DateTime? ReleaseDate { get; set; }

        [Column("Script ran", TypeName = "smalldatetime")]
        public DateTime? Script_ran { get; set; }

        [StringLength(20)]
        public string Scope { get; set; }
    }
}
