using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using slls.Localization;

namespace slls.Models
{
    [Table("FrequencyTypes")]
    public class Frequency
    {
        public Frequency()
        {
            //Titles = new HashSet<Title>();
        }

        public int FrequencyID { get; set; }

        [Column("Frequency")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [LocalDisplayName("Frequency.Frequency", "FieldDisplayName")]
        [StringLength(255)]
        public string Frequency1 { get; set; }

        [DisplayName("Days Between")]
        [LocalDisplayName("Frequency.DaysBetween", "FieldDisplayName")]
        [RegularExpression("([0-9]+)", ErrorMessage = "Please enter valid number, either 0 (zero) or greater")]
        [Required(ErrorMessage = "Please enter a value for Days Between.")]
        [Range(0, 9999)]
        public int? Days { get; set; }
        
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

        [LocalDisplayName("Frequency.Titles", "FieldDisplayName")]
        public virtual ICollection<Title> Titles { get; set; }
    }
}