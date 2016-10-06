using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using slls.Localization;

namespace slls.Models
{
    [Table("Localization")]
    public class Localization
    {
        [Key]
        public int pk { get; set; }

        [StringLength(1024)]
        [LocalDisplayName("Localization.Resource", "FieldDisplayName")]
        public string ResourceId { get; set; }

        [LocalDisplayName("Localization.Value", "FieldDisplayName")]
        public string Value { get; set; }

        [StringLength(10)]
        [LocalDisplayName("Localization.Locale", "FieldDisplayName")]
        public string LocaleId { get; set; }

        [StringLength(512)]
        [LocalDisplayName("Localization.ResourceSet", "FieldDisplayName")]
        public string ResourceSet { get; set; }

        [StringLength(512)]
        public string Type { get; set; }

        public string BinFile { get; set; }
        public string TextFile { get; set; }

        [StringLength(128)]
        public string Filename { get; set; }

        [StringLength(512)]
        public string Comment { get; set; }

        public int? ValueType { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? Updated { get; set; }
    }
}