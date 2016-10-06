using System.ComponentModel;
using slls.Localization;

namespace slls.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ReleaseHeader")]
    public partial class ReleaseHeader
    {
        [Key]
        public int ReleaseId { get; set; }

        [Required]
        [StringLength(50)]
        [DisplayName("Release Number")]
        public string ReleaseNumber { get; set; }

        [Required]
        [StringLength(24)]
        [DisplayName("Release Date")]
        public string ReleaseDate { get; set; }

        [StringLength(1000)]
        public string Comments { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? InputDate { get; set; }

        [DisplayName("Release Notes")]
        public virtual ICollection<ReleaseNote> ReleaseNotes { get; set; }
    }
}
