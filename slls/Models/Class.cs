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
    [Table("Classes")]
    public class Class
    {
        [Key]
        [Column("ClassID")]
        public int ClassId { get; set; }

        [Column("Class")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [LocalDisplayName("Classes.Class", "FieldDisplayName")]
        public string Class1 { get; set; }

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

        [LocalDisplayName("Classes.Users", "FieldDisplayName")]
        public virtual ICollection<ApplicationUser> LibraryUsers { get; set; }
    }
}