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
    [Table("AudienceTypes")]
    public class Audience
    {
        [Key]
        [Column("AudienceID")]
        public int AudienceId { get; set; }

        [Column("Audience")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [LocalDisplayName("Audiences.Audience", "FieldDisplayName")]
        public string Audience1 { get; set; }

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

        [LocalDisplayName("Audiences.Titles", "FieldDisplayName")]
        public virtual ICollection<Title> Titles { get; set; }
    }
}