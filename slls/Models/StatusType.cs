using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using slls.Localization;

namespace slls.Models
{
    [Table("StatusTypes")]
    public class StatusType
    {
        public StatusType()
        {
            //Copies = new HashSet<Copy>();
        }

        [Key]
        public int StatusID { get; set; }

        [StringLength(255)]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [LocalDisplayName("StatusTypes.Status_Type", "FieldDisplayName")]
        public string Status { get; set; }

        [LocalDisplayName("StatusTypes.Opac", "FieldDisplayName")]
        public bool Opac { get; set; }

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

        public virtual ICollection<Copy> Copies { get; set; }
    }
}