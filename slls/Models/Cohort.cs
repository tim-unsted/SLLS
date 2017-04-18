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
    [Table("Cohorts")]
    public class Cohort
    {
        [Key]
        [Column("CohortID")]
        public int CohortId { get; set; }

        [Column("Cohort")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [LocalDisplayName("Cohorts.Cohort", "FieldDisplayName")]
        public string Cohort1 { get; set; }

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

        [LocalDisplayName("Cohorts.Users", "FieldDisplayName")]
        public virtual ICollection<ApplicationUser> LibraryUsers { get; set; }
    }
}