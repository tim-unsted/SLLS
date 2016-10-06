using System.ComponentModel;
using slls.Localization;

namespace slls.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TitleAdditionalFieldData")]
    public partial class TitleAdditionalFieldData
    {
        [Key]
        public int RecID { get; set; }

        [DisplayName("Type")]
        public int FieldID { get; set; }

        [DisplayName("Title")]
        public int TitleID { get; set; }

        [DisplayName("Text")]
        public string FieldData { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? InputDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? LastModified { get; set; }

        [Column(TypeName = "timestamp")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [MaxLength(8)]
        public byte[] RowVersion { get; set; }

        public virtual Title Title { get; set; }

        public virtual TitleAdditionalFieldDef TitleAdditionalFieldDef { get; set; }
    }
}
