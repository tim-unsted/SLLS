using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using slls.Localization;

namespace slls.Models
{
    [Table("TitleAuthors")]
    public class TitleAuthor
    {
        [Key]
        public int TitleAuthorId { get; set; }

        public int TitleId { get; set; }
        
        public int AuthorId { get; set; }

        [LocalDisplayName("TitleAuthors.Order_Sequence", "FieldDisplayName")]
        public int? OrderSeq { get; set; }

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

        public virtual Author Author { get; set; }
        public virtual Title Title { get; set; }
    }
}