using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using slls.Localization;

namespace slls.Models
{
    [Table("Departments")]
    public class Department
    {
        public Department()
        {
            //LibraryUsers = new HashSet<ApplicationUser>();
        }

        public int DepartmentID { get; set; }

        [Column("Department")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [LocalDisplayName("Departments.Department", "FieldDisplayName")]
        [StringLength(255)]
        public string Department1 { get; set; }

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

        [LocalDisplayName("Departments.Department_Users", "FieldDisplayName")]
        public virtual ICollection<ApplicationUser> LibraryUsers { get; set; }
    }
}