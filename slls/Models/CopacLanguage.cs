using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using slls.Localization;

namespace slls.Models
{
    [Table("CopacLanguages")]
    public class CopacLanguage
    {
        public CopacLanguage()
        {
        }

        [Key]
        public int LanguageID { get; set; }

        [Column("Language")]
        [Required]
        [StringLength(50)]
        [LocalDisplayName("Languages.Language", "FieldDisplayName")]
        public string Language { get; set; }
    }
}