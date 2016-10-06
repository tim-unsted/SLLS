using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using slls.Localization;

namespace slls.Models
{
    [Table("Keywords")]
    public class Keyword
    {
        public Keyword()
        {
            //Keyword1 = new HashSet<Keyword>();
            //SubjectIndexes = new HashSet<SubjectIndex>();
        }

        public int KeywordID { get; set; }

        [Column("KeywordTerm")]
        [StringLength(255)]
        [LocalDisplayName("Keywords.Keyword", "FieldDisplayName")]
        public string KeywordTerm { get; set; }

        [LocalDisplayName("Keywords.Keyword", "FieldDisplayName")]
        public string KeywordDisplay
        {
            get
            {
                return string.IsNullOrEmpty(KeywordTerm) ? "<no name>" : KeywordTerm;
            }
        }

        [LocalDisplayName("Keywords.Parent_Keyword", "FieldDisplayName")]
        public int? ParentKeywordID { get; set; }

        [StringLength(255)]
        public string KeywordHier { get; set; }

        [DisplayName("Deleted?")]
        public bool Deleted { get; set; }

        [DisplayName("Can update?")]
        public bool CanUpdate { get; set; }

        [DisplayName("Can delete?")]
        public bool CanDelete { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? InputDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? LastModified { get; set; }

        [Column(TypeName = "timestamp")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [MaxLength(8)]
        public byte[] RowVersion { get; set; }
        
        public virtual ICollection<Keyword> Keyword1 { get; set; }

        [ForeignKey("ParentKeywordID")]
        public virtual Keyword Keyword2 { get; set; }

        public virtual ICollection<SubjectIndex> SubjectIndexes { get; set; }

        [NotMapped]
        public int TitleCount { get; set; }
    }
}