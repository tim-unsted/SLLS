using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using slls.Localization;

namespace slls.Models
{
    [Table("PartsReceived")]
    public class PartsReceived
    {
        [Key]
        public int PartID { get; set; }
        public int? CopyID { get; set; }
        //public int? TitleID { get; set; }
        //public int? CopyNumber { get; set; }

        [StringLength(255)]
        [LocalDisplayName("CheckIn.Part_Issue_Desc", "FieldDisplayName")]
        public string PartReceived { get; set; }

        [Column(TypeName = "smalldatetime")]
        [LocalDisplayName("CheckIn.Date_Received", "FieldDisplayName")]
        public DateTime? DateReceived { get; set; }

        public string DateReceivedSortable
        {
            get { return string.Format("{0:yyyy-MM-dd}", DateReceived); }
        }

        [LocalDisplayName("CheckIn.Print_Circulation_List", "FieldDisplayName")]
        public bool PrintList { get; set; }

        [LocalDisplayName("CheckIn.Returned", "FieldDisplayName")]
        public bool Returned { get; set; }

        [DisplayName("Deleted?")]
        public bool Deleted { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? LastModified { get; set; }

        [Column(TypeName = "timestamp")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [MaxLength(8)]
        public byte[] RowVersion { get; set; }

        public virtual Copy Copy { get; set; }

        
    }
}