using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace slls.Models
{
    [Table("Images")]
    public class CoverImage
    {
        [Key]
        public int ImageId { get; set; }

        [StringLength(255)]
        public string Source { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        public string Type { get; set; }

        public Int64 Size { get; set; }

        [Required]
        public byte[] Image { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? InputDate { get; set; }

        [DisplayName("Deleted?")]
        public bool Deleted { get; set; }

        [Column(TypeName = "timestamp")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [MaxLength(8)]
        public byte[] RowVersion { get; set; }
    }
}