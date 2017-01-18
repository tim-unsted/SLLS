using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Dynamic;
using slls.Localization;

namespace slls.Models
{
    [Table("MediaTypes")]
    public class MediaType
    {
        public MediaType()
        {
           // Titles = new HashSet<Title>();
        }

        [Key]
        public int MediaID { get; set; }

        [Required]
        [StringLength(255)]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [LocalDisplayName("MediaTypes.Media_Type", "FieldDisplayName")]
        public string Media { get; set; }

        [LocalDisplayName("MediaTypes.Loan_Type", "FieldDisplayName")]
        public int LoanTypeID { get; set; }

        [LocalDisplayName("MediaTypes.Media_Type", "FieldDisplayName")]
        public string MediaDisplay
        {
            get { return string.IsNullOrEmpty(Media) ? "<no name>" : Media; }
        }

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

        [LocalDisplayName("MediaTypes.Titles", "FieldDisplayName")]
        public virtual ICollection<Title> Titles { get; set; }

        public virtual LoanType LoanType { get; set; }

        [NotMapped]
        public int TitleCount { get; set; }

        [NotMapped]
        public string FilterString { get; set; }
    }
}