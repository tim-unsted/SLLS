using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using slls.Localization;

namespace slls.Models
{
    [Table("Classmarks")]
    public class Classmark
    {
        public Classmark()
        {
            //Titles = new HashSet<Title>();
        }

        public int ClassmarkID { get; set; }

        [Column("Classmark")]
        [StringLength(255)]
        [LocalDisplayName("Classmarks.Classmark", "FieldDisplayName")]
        public string Classmark1 { get; set; }

        [StringLength(50)]
        [LocalDisplayName("Classmarks.Short_Code", "FieldDisplayName")]
        public string Code { get; set; }

        [LocalDisplayName("Classmarks.Classmark", "FieldDisplayName")]
        public string ClassmarkDisplay
        {
            get { return string.IsNullOrEmpty(Classmark1) ? "<no name>" : Classmark1; }
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

        [LocalDisplayName("Classmarks.Titles", "FieldDisplayName")]
        public virtual ICollection<Title> Titles { get; set; }

        [NotMapped]
        public int TitleCount{ get; set; }
        
    }
}