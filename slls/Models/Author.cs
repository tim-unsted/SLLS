using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using slls.Localization;

namespace slls.Models
{
    [Table("Authors")]
    public class Author
    {
        public Author()
        {
            //TitleAuthors = new HashSet<TitleAuthor>();
            //TitleEditors = new HashSet<TitleEditor>();
        }

        public int AuthorID { get; set; }

        [StringLength(100)]
        [LocalDisplayName("Authors.Title", "FieldDisplayName")]
        public string Title { get; set; }

        [StringLength(255)]
        [LocalDisplayName("Authors.Display_Name", "FieldDisplayName")]
        public string DisplayName { get; set; }

        [StringLength(255)]
        [LocalDisplayName("Authors.Firstnames", "FieldDisplayName")]
        public string Firstnames { get; set; }

        [StringLength(255)]
        [LocalDisplayName("Authors.Lastnames", "FieldDisplayName")]
        public string Lastnames { get; set; }

        [StringLength(1)]
        [LocalDisplayName("Authors.Author_Type", "FieldDisplayName")]
        public string AuthType { get; set; }

        [LocalDisplayName("Authors.Notes", "FieldDisplayName")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }
        
        [DisplayName("Deleted?")]
        public bool Deleted { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? InputDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? LastModified { get; set; }

        [Column(TypeName = "timestamp")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [MaxLength(8)]
        public byte[] RowVersion { get; set; }

        [LocalDisplayName("Authors.Author_Titles", "FieldDisplayName")]
        public virtual ICollection<TitleAuthor> TitleAuthors { get; set; }

        [LocalDisplayName("Authors.Editor_Titles", "FieldDisplayName")]
        public virtual ICollection<TitleEditor> TitleEditors { get; set; }

        public Dictionary<string, string> AuthorType { get; set; }

        [LocalDisplayName("Authors.Author_Type", "FieldDisplayName")]
        public string AuthTypeDisplay
        {
            get { return AuthType.ToLower() == "c" ? "Corporate" : "Personal"; }
        }

        [NotMapped]
        public int TitleCount { get; set; }

        [NotMapped]
        public string Index { get; set; }

    }


}