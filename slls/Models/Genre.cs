using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using slls.Localization;

namespace slls.Models
{
    [Table("GenreTypes")]
    public class Genre
    {
        [Key]
        [Column("GenreID")]
        public int GenreId { get; set; }

        [Column("Genre")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [LocalDisplayName("Genres.Genre", "FieldDisplayName")]
        public string Genre1 { get; set; }

        [DisplayName("Can update?")]
        public bool CanUpdate { get; set; }

        [DisplayName("Can Delete?")]
        public bool CanDelete { get; set; }

        [DisplayName("Deleted?")]
        public bool Deleted { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? InputDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? LastModified { get; set; }

        [LocalDisplayName("Genres.Titles", "FieldDisplayName")]
        public virtual ICollection<Title> Titles { get; set; }
    }
}