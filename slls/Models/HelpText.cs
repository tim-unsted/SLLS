namespace slls.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("HelpTexts")]
    public partial class HelpText
    {
        [Key]
        [StringLength(50)]
        public string HelpId { get; set; }

        [StringLength(255)]
        public string HelpTitle { get; set; }

        [Column("HelpText")]
        public string HelpTextString { get; set; }

        public bool System { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? InputDate { get; set; }
    }
}
