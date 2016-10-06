using System.ComponentModel;

namespace slls.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ReleaseNotes")]
    public partial class ReleaseNote
    {
        [Key]
        public int ReleaseNoteId { get; set; }
        
        [DisplayName("Release Header ID")]
        public int ReleaseId { get; set; }

        [Required]
        [DisplayName("Item")]
        public int SequenceNo { get; set; }

        [DataType(DataType.MultilineText)]
        public string Detail { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? InputDate { get; set; }

        public virtual ReleaseHeader ReleaseHeader { get; set; }
    }
}
