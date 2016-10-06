using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using slls.Localization;

namespace slls.Models
{
    [Table("Languages")]
    public class Language
    {
        public Language()
        {
            //Titles = new HashSet<Title>();
        }

        [Key]
        public int LanguageID { get; set; }

        [Column("Language")]
        [Required]
        [StringLength(255)]
        [LocalDisplayName("Languages.Language", "FieldDisplayName")]
        public string Language1 { get; set; }

        [DisplayName("Can update?")]
        public bool CanUpdate { get; set; }

        [DisplayName("Can delete?")]
        public bool CanDelete { get; set; }

        [DisplayName("Deleted?")]
        public bool Deleted { get; set; }

        public int ListPos { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? InputDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? LastModified { get; set; }

        [Column(TypeName = "timestamp")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [MaxLength(8)]
        public byte[] RowVersion { get; set; }

        [LocalDisplayName("Languages.Titles", "FieldDisplayName")]
        public virtual ICollection<Title> Titles { get; set; }

        [NotMapped]
        public int TitleCount { get; set; }
    }
}