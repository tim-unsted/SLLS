using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using slls.Localization;

namespace slls.Models
{
    [Table("LibraryUserEmailAddresses")]
    public class LibraryUserEmailAddress
    {
        [Key]
        public int EmailID { get; set; }

        public string UserID { get; set; }

        [StringLength(255)]
        [LocalDisplayName("Users.Email_Address", "FieldDisplayName")]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [LocalDisplayName("Users.Is_Primary_Address", "FieldDisplayName")]
        public bool IsPrimary { get; set; }
        
        [DisplayName("Deleted?")]
        public bool Deleted { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? InputDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? LastModified { get; set; }
        
        public virtual ApplicationUser LibraryUser { get; set; }
    }
}