using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace slls.Models
{
    [Table("TitleImages")]
    public class TitleImage
    {
        [Key]
        public int TitleImageId { get; set; }

        public int ImageId { get; set; }

        public int TitleId { get; set; }
                
        public string Alt { get; set; }

        public string HoverText { get; set; }

        public bool IsPrimary { get; set; }
        
        [Column(TypeName = "smalldatetime")]
        public DateTime? InputDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? LastModified { get; set; }

        [DisplayName("Deleted?")]
        public bool Deleted { get; set; }

        [Column(TypeName = "timestamp")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [MaxLength(8)]
        public byte[] RowVersion { get; set; }

        public virtual Title Title { get; set; }
    }
}