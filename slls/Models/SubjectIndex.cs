using System.ComponentModel;

namespace slls.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SubjectIndex")]
    public partial class SubjectIndex
    {
        public int SubjectIndexID { get; set; }

        public int TitleID { get; set; }

        public int KeywordID { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? InputDate { get; set; }

        [DisplayName("Deleted?")]
        public bool Deleted { get; set; }

        public virtual Keyword Keyword { get; set; }

        public virtual Title Title { get; set; }
    }
}
