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
    [Table("UserTypes")]
    public class UserType
    {
        [Key]
        [Column("UserTypeID")]
        public int UserTypeId { get; set; }

        [Column("UserType")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [LocalDisplayName("UserTypes.User_Type", "FieldDisplayName")]
        public string UserType1 { get; set; }

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

        [LocalDisplayName("UserTypes.Users", "FieldDisplayName")]
        public virtual ICollection<ApplicationUser> LibraryUsers { get; set; }
    }
}