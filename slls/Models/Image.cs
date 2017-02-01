using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using slls.Localization;

namespace slls.Models
{
    [Table("Images")]
    public class CoverImage
    {
        [Key]
        public int ImageId { get; set; }

        [StringLength(255)]
        [LocalDisplayName("Images.Source", "FieldDisplayName")]
        public string Source { get; set; }

        [StringLength(255)]
        [LocalDisplayName("Images.Name", "FieldDisplayName")]
        public string Name { get; set; }

        [LocalDisplayName("Images.Type", "FieldDisplayName")]
        public string Type { get; set; }

        [LocalDisplayName("Images.Size", "FieldDisplayName")]
        public Int64 Size { get; set; }

        [Required]
        [LocalDisplayName("Images.Image", "FieldDisplayName")]
        public byte[] Image { get; set; }

        [Column(TypeName = "smalldatetime")]
        [LocalDisplayName("Images.Date_Uploaded", "FieldDisplayName")]
        public DateTime? InputDate { get; set; }

        [DisplayName("Deleted?")]
        public bool Deleted { get; set; }

        [Column(TypeName = "timestamp")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [MaxLength(8)]
        public byte[] RowVersion { get; set; }

        [LocalDisplayName("Images.Titles", "FieldDisplayName")]
        public virtual ICollection<TitleImage> TitleImages { get; set; }

        [NotMapped]
        public string InputDateSortable
        {
            get { return string.Format("{0:yyyy-MM-dd}", InputDate); }
        }
    }
}